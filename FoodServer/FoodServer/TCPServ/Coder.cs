///////////////////////////////////////////////////////
//NSTCPFramework
//�汾��1.0.0.1
//////////////////////////////////////////////////////
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using FoodServer.Class;

namespace FlyTcpFramework
{
    /// <summary> 
    /// ͨѶ�����ʽ�ṩ��,ΪͨѶ�����ṩ����ͽ������ 
    /// ������ڼ̳����ж����Լ��ı��뷽ʽ��:���ݼ��ܴ���� 
    /// </summary> 
    public class Coder
    {
        /// <summary> 
        /// ���뷽ʽ 
        /// </summary> 
        private EncodingMothord _encodingMothord;
     
        protected Coder()
        {

        }

        public Coder(EncodingMothord encodingMothord)
        {
            _encodingMothord = encodingMothord;
        }

        public enum EncodingMothord
        {
            Default = 0,
            Unicode,
            UTF8,
            ASCII,
        }
        /// <summary>
        ///   ���ݽ������գ�
        /// </summary>
        /// <summary> 
        /// 
   

        ///bytes����ת�ṹ���ֽ�����ת�ṹ��(��С��ģʽ)
        public static object BytesToStruct(byte[] bytes, Type type)
        {
            //�õ��ṹ���С
            int size = Marshal.SizeOf(type);
            //byte ���鳤��С�ڽṹ���С
            if (size > bytes.Length)
            {
                return null;
            }

            //����ṹ���С���ڴ�ռ�
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //��byte���鿼������õ��ڴ�ռ�
            Marshal.Copy(bytes, 0, structPtr, size);

            //���ڴ�ռ�ת��ΪĿ��ṹ��
            object obj = Marshal.PtrToStructure(structPtr, type);

            //�ͷ��ڴ�
            Marshal.FreeHGlobal(structPtr);
            return obj;

        }
        public virtual MSGHEAD GetNetData(byte[] dataBytes, int start, int size)
        {
            MSGHEAD msg = new MSGHEAD();
            
             byte[] dataByte = new byte[10];
             Array.Copy(dataBytes, start, dataByte, 0, size);
             msg = (MSGHEAD)BytesToStruct(dataByte, typeof(MSGHEAD));

            return msg;
        }
        
        /// ͨѶ���ݽ��� 
        /// </summary> 
        /// <param name="dataBytes">��Ҫ���������</param> 
        /// <returns>����������</returns> 
        public virtual string GetEncodingString(byte[] dataBytes,int start, int size)
        {
            switch (_encodingMothord)
            {
                case EncodingMothord.Default:
                    {
                        return Encoding.Default.GetString(dataBytes, start, size);
                    }
                case EncodingMothord.Unicode:
                    {
                        return Encoding.Unicode.GetString(dataBytes, start, size);
                    }
                case EncodingMothord.UTF8:
                    {
                        return Encoding.UTF8.GetString(dataBytes, start, size);
                    }
                case EncodingMothord.ASCII:
                    {
                        return Encoding.ASCII.GetString(dataBytes, start, size);
                    }
                default:
                    {
                        throw (new Exception("δ����ı����ʽ"));
                    }
            }

        }
        /// <summary>
        ///   
        /// </summary>
        /// <summary>
        public static byte[] ReceiveVarData(Socket s) // return array that store the received data.  
        {
            int total = 0;
            int recv;
            byte[] datasize = new byte[4];
            recv = s.Receive(datasize, 0, 4, SocketFlags.None);//receive the size of data array for initialize a array.  
            int size = BitConverter.ToInt32(datasize, 0);
            int dataleft = size;
            byte[] data = new byte[size];

            while (total < size)
            {
                recv = s.Receive(data, total, dataleft, SocketFlags.None);
                if (recv == 0)
                {
                    data = null;
                    break;
                }
                total += recv;
                dataleft -= recv;
            }

            return data;

        }  
        /// Saves the file.
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        /// <param name="Result">The result.</param>
        public void SaveFile(string FileName, byte[] Result)
        {
			FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate);
            fs.Write(Result, 5+Result[1], Result[2] *65536+Result[3] * 256 + Result[4]);//
            fs.Flush();
            fs.Close();

        }
 
        /// <summary> 
        /// ���ݱ��� 
        /// </summary> 
        /// <param name="datagram">��Ҫ����ı���</param> 
        /// <returns>����������</returns> 
        public virtual byte[] GetTextBytes(string datagram)
        {
            byte[] rbyte = new byte[Encoding.UTF8.GetBytes(datagram).Length + 1];
            rbyte[0] = 0x5b;
            switch (_encodingMothord)
            {
                case EncodingMothord.Default:
                    {
                        Encoding.Default.GetBytes(datagram, 0, datagram.Length, rbyte, 1);
                        return rbyte;
                    }
                case EncodingMothord.Unicode:
                    {
                        Encoding.Unicode.GetBytes(datagram, 0, datagram.Length, rbyte, 1);
                        return rbyte;
                    }
                case EncodingMothord.UTF8:
                    {
                        Encoding.UTF8.GetBytes(datagram, 0, datagram.Length, rbyte, 1);
                        return rbyte;
                    }
                case EncodingMothord.ASCII:
                    {
                        Encoding.ASCII.GetBytes(datagram, 0, datagram.Length, rbyte, 1);
                        return rbyte;
                    }
                default:
                    {
                        throw (new Exception("δ����ı����ʽ"));
                    }
            }

        }

        public virtual byte[] GetFileBytes(string FilePath)
        {
            if (File.Exists(FilePath))
            {
				string fileName=Path.GetFileName(FilePath);
				byte[] bytFileName=this.GetTextBytes(fileName);
                
                
                FileStream fs = new FileStream(FilePath, FileMode.Open);
                UInt64 LnName = (UInt64)bytFileName.Length;
                UInt64 LngTol = (UInt64)LnName + 17 + (UInt64)fs.Length;
                Byte[] RByte = new byte[fs.Length + 17 +bytFileName.Length];
                byte[] aa = BitConverter.GetBytes(LngTol);
                Array.Reverse(aa);
                aa.CopyTo(RByte, 1);
                byte[] bb = BitConverter.GetBytes(LnName);
                Array.Reverse(bb);
                bb.CopyTo(RByte, 9);

                RByte[0] = 0x66;
// 				RByte[1] = (byte)(bytFileName.Length);
// 				RByte[2] = (byte)(fs.Length / 65536);
// 				RByte[3] = (byte)(fs.Length / 256);
//                 RByte[4] = (byte)(fs.Length % 256);
				bytFileName.CopyTo(RByte,17);
                fs.Read(RByte, 17+bytFileName.Length, (int)fs.Length);
                return RByte;

//                 string fileName = Path.GetFileName(FilePath);
//                 byte[] bytFileName = this.GetTextBytes(fileName);
//                 FileStream fs = new FileStream(FilePath, FileMode.Open);
//                 Byte[] RByte = new byte[fs.Length + 5 + bytFileName.Length];
//                 RByte[0] = 0x66;
//                 RByte[1] = (byte)(bytFileName.Length);
//                 RByte[2] = (byte)(fs.Length / 65536);
//                 RByte[3] = (byte)(fs.Length / 256);
//                 RByte[4] = (byte)(fs.Length % 256);
//                 bytFileName.CopyTo(RByte, 5);
//                 fs.Read(RByte, 5 + bytFileName.Length, (int)fs.Length);
//                 return RByte;
            }
            else
            {
                throw (new Exception("�ļ�������"));
            }
        }

    } 
}
