using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
//http://blog.csdn.net/diyoosjtu/article/details/7837813
namespace FoodServer.Class
{

    /// <summary>
    /// 数据头  
    /// 数据长度
    /// 业务数据类型
    /// 监测仪器接入码
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct MSGHEAD
    {
        public UInt32 Msg_Length; //msg id
        public UInt32 SN;
        public UInt16 Msg_ID;
        public UInt32 Msg_GNSSCenter;

    }
    //视频调阅消息
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct VIDIOMES
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] deviceid;//用户名  4字节
        public UInt64 batch_id;//批次号ID
        public UInt32 task_id;//检测任务ID

    }
    /// <summary>
    ///   数据结构体
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct MSG
    {
        //public byte flag;
        public MSGHEAD msg_header;
        public byte[] msg_body;
        public UInt16 crc;
        // public byte endflag;
    };

    public enum MessageKeep
    {
        Mess_Keep = 0x0001,
        Mess_Ask = 0x0002,//链路连接保持应答消息
        Mess_TastSend = 0x0003,//批量任务下发消息
        Mess_Result = 0x0004,//测结果上传消息
        Mess_AVstart = 0x0005,//开始实时视频监控消息
        Mess_AVstop = 0x0006,//结束实时视频监控消息
        Mess_AVup = 0x0007,//录制视频上传
        Mess_AvupAsk = 0x0008,//录制视频上传应答
        Mess_SeeAV = 0x0009,//远程调阅视频
        Mess_ConfQur = 0x000a,//远程状态和工作参数查询
        Mess_ConfQurAsk = 0x000b,//远程状态和工作参数查询应答
        Mess_Conf = 0x000c,//远程参数配置
        Mess_Data = 0x000d,//远程数据管理
        Mess_Device = 0x000e,//远程设备管理
        Mess_Update = 0x000f,//远程版本升级
        Mess_UpdateAsk = 0x0010,//远程版本升级应答
        Mess_ComAsk = 0xf000,//通用应答

    }

    public enum MessageType
    {
        DetecTastStar = 0x8001,
        DetecTastEnd = 0x8002,
        StartAV = 0x8003,
        StopAV = 0x8004,
    }

    /// <summary>
    /// 链路登陆请求	监控程序->中心站	0x0001
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct Msg_LoadAsk
    {
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public UInt32 nameId;  //用户名
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] password;//用户密码  8字节
    }


    /// <summary>
    /// 链路登陆应答	中心站<-监控程序	0x0002
    /// 00 成功
    /// 01密码不正确
    /// 02 没有注册
    /// 03密码错误
    /// 04 资源紧张
    /// 05 其他
    /// </summary>
    public struct Msg_loadAns
    {
        public byte result;//
    }
    //任务信息
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct TASK_INFO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] p_name;		//检测项目名称
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public byte[] type;		//样品类型
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public byte[] sample_no;		//样品编号
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] sites;		//产地
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] submission_unit;	//送检单位
    }
    /// <summary>
    ///   检测项目及样品信息参数
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct FINFOR_INFO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public byte[] ftesttimes;		//检测项目名称
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] fsample;		//检测项目名称
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] wavemode;		//
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] wavemajor;		//
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] waveminor;		//
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] formulaC;		//
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] formulaB;		//
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] formulaA;		//
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] interdilute;		//
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] unit;		//
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public byte[] standards;		//
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] comparemode;		//
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] comparemin;		//
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] comparemax;		//
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] tesrangemax;		//
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] tesrangemin;		//

    }

    public struct FINFOR_INFOSTR
    {
      
        public string ftesttimes;		//检测项目名称

        public string fsample;		//检测项目名称

        public string wavemode;		//

        public string wavemajor;		//

        public string waveminor;		//

        public string formulaC;		//

        public string formulaB;		//

        public string formulaA;		//

        public string interdilute;		//

        public string unit;		//

        public string standards;		//

        public string comparemode;		//

        public string comparemin;		//

        public string comparemax;		//

        public string tesrangemax;		//
        
        public string tesrangemin;		//

        public string  red;
        public string  yellow;
        public string  pitch;

    }
    //视频配置
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct VIDIOCONFI
    {
        public UInt32 m_width;
        public UInt32 m_height;
        public UInt32 m_frame;
        public UInt32 m_bitrate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] m_format;		//

    }

    //照片配置


    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PICCONFI
    {
        public UInt32 m_width;
        public UInt32 m_height;
    }
    //配置结构体
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct SYSTEMCONFMSG
    {
        public UInt32 m_configtype;
        public UInt32 m_confnum;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 324)]
        public byte[] m_para;		//端口

    }
    //服务器
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ONLINE_INFO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public byte[] ipasddr;		//服务器IP
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] port;		//端口

    }

    /// <summary>
    ///   检测结果
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct DETECTION_INFO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] batch_id;//批次号ID
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] task_id;//检测任务ID
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] p_name;		//检测项目名称
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public Byte[] type;		//样品类型

        public UInt32 channel;			//测量通道
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public Byte[] ftest_result;		//检测判定结果
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public Byte[] test_result;		//测量结果
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public Byte[] result_unit;		//结果单位
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public Byte[] standard;		//参考标准
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public Byte[] abs_result;		//吸光度
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public Byte[] sample_no;		//样品编号
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public Byte[] sample_name;		//样品名称
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public Byte[] sites;		//产地
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public Byte[] submission_unit;	//送检单位
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public Byte[] test_operator;	//操作人员
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public Byte[] test_unit;		//测试单位
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public Byte[] test_time;		//测试单位
    }
    public class TastInfo
    {
        ///结构体转byte(小端模式）
        public byte[] StructToByte(object structObj)
        {

            int size = Marshal.SizeOf(structObj);//得到结构体的大小
            IntPtr buffer = Marshal.AllocHGlobal(size);//分配结构体大小的内存空间
            try
            {
                Marshal.StructureToPtr(structObj, buffer, false);
                byte[] bytes = new byte[size];//创建byte数组
                Marshal.Copy(buffer, bytes, 0, size); //从内存空间拷贝到bytes
                return bytes; //返回bytes数组
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);// //释放内存空间
            }
        }

        ///bytes数组转结构体字节数组转结构体(按小端模式)
        public object BytesToStruct(byte[] bytes, Type type)
        {
            //得到结构体大小
            int size = Marshal.SizeOf(type);
            //byte 数组长度小于结构体大小
            //             if (size > bytes.Length)
            //             {
            //                 return null;
            //             }

            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组考到分配好的内存空间
            // Marshal.Copy(bytes, 0, structPtr, size);

            try
            {
                Marshal.Copy(bytes, 0, structPtr, size);
                //将内存空间转换为目标结构体
                object obj = Marshal.PtrToStructure(structPtr, type);
                return obj;
            }
            finally
            {
                Marshal.FreeHGlobal(structPtr);//释放内存
            }
        }

        ///结构体转字节数组（大端模式）
        public byte[] StructureToByteArrayEndian(object obj)
        {
            object thisBoxed = obj;   //copy ，将 struct 装箱
            Type test = thisBoxed.GetType();

            int offset = 0;
            byte[] data = new byte[Marshal.SizeOf(thisBoxed)];

            object fieldValue;
            TypeCode typeCode;
            byte[] temp;
            // 列举结构体的每个成员，并Reverse
            foreach (var field in test.GetFields())
            {
                fieldValue = field.GetValue(thisBoxed); // Get value

                typeCode = Type.GetTypeCode(fieldValue.GetType());  // get type

                switch (typeCode)
                {
                    case TypeCode.Single: // float
                        {
                            temp = BitConverter.GetBytes((Single)fieldValue);
                            Array.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Single));
                            break;
                        }
                    case TypeCode.Int32:
                        {
                            temp = BitConverter.GetBytes((Int32)fieldValue);
                            Array.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Int32));
                            break;
                        }
                    case TypeCode.UInt32:
                        {
                            temp = BitConverter.GetBytes((UInt32)fieldValue);
                            Array.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(UInt32));
                            break;
                        }
                    case TypeCode.Int16:
                        {
                            temp = BitConverter.GetBytes((Int16)fieldValue);
                            Array.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Int16));
                            break;
                        }
                    case TypeCode.UInt16:
                        {
                            temp = BitConverter.GetBytes((UInt16)fieldValue);
                            Array.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(UInt16));
                            break;
                        }
                    case TypeCode.Int64:
                        {
                            temp = BitConverter.GetBytes((Int64)fieldValue);
                            Array.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Int64));
                            break;
                        }
                    case TypeCode.UInt64:
                        {
                            temp = BitConverter.GetBytes((UInt64)fieldValue);
                            Array.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(UInt64));
                            break;
                        }
                    case TypeCode.Double:
                        {
                            temp = BitConverter.GetBytes((Double)fieldValue);
                            Array.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Double));
                            break;
                        }
                    case TypeCode.Byte:
                        {
                            data[offset] = (Byte)fieldValue;
                            break;
                        }
                    default:
                        {
                            //System.Diagnostics.Debug.Fail("No conversion provided for this type : " + typeCode.ToString());
                            break;
                        }
                }; // switch
                if (typeCode == TypeCode.Object)
                {
                    int length = ((byte[])fieldValue).Length;
                    Array.Copy(((byte[])fieldValue), 0, data, offset, length);
                    offset += length;
                }
                else
                {
                    offset += Marshal.SizeOf(fieldValue);
                }
            } // foreach

            return data;
        } // Swap



        /// <summary>
        /// 字节数组转结构体(按大端模式)
        /// </summary>
        /// <param name="bytearray">字节数组</param>
        /// <param name="obj">目标结构体</param>
        /// <param name="startoffset">bytearray内的起始位置</param>
        public void ByteArrayToStructureEndian(byte[] bytearray, ref object obj, int startoffset)
        {
            int len = Marshal.SizeOf(obj);
            IntPtr i = Marshal.AllocHGlobal(len);
            byte[] temparray = (byte[])bytearray.Clone();
            // 从结构体指针构造结构体
            obj = Marshal.PtrToStructure(i, obj.GetType());
            // 做大端转换
            object thisBoxed = obj;
            Type test = thisBoxed.GetType();
            int reversestartoffset = startoffset;
            // 列举结构体的每个成员，并Reverse
            foreach (var field in test.GetFields())
            {
                object fieldValue = field.GetValue(thisBoxed); // Get value

                TypeCode typeCode = Type.GetTypeCode(fieldValue.GetType());  //Get Type
                if (typeCode != TypeCode.Object)  //如果为值类型
                {
                    Array.Reverse(temparray, reversestartoffset, Marshal.SizeOf(fieldValue));
                    reversestartoffset += Marshal.SizeOf(fieldValue);
                }
                else  //如果为引用类型
                {
                    reversestartoffset += ((byte[])fieldValue).Length;
                }
            }
            try
            {
                //将字节数组复制到结构体指针
                Marshal.Copy(temparray, startoffset, i, len);
            }
            catch (Exception ex) { Console.WriteLine("ByteArrayToStructure FAIL: error " + ex.ToString()); }
            obj = Marshal.PtrToStructure(i, obj.GetType());
            Marshal.FreeHGlobal(i);  //释放内存
        }
        /*
            public StructType ConverBytesToStructure<StructType>(byte[] bytesBuffer)  
                {  
                    // 检查长度。  
                    if (bytesBuffer.Length != Marshal.SizeOf(typeof(StructType)))  
                    {  
                        throw new ArgumentException("bytesBuffer参数和structObject参数字节长度不一致。");  
                    }  
  
                    IntPtr bufferHandler = Marshal.AllocHGlobal(bytesBuffer.Length);  
                    for (int index = 0; index < bytesBuffer.Length; index++)  
                    {  
                        Marshal.WriteByte(bufferHandler, index, bytesBuffer[index]);  
                    }  
                    StructType structObject = (StructType)Marshal.PtrToStructure(bufferHandler, typeof(StructType));  
                    Marshal.FreeHGlobal(bufferHandler);  
                    return structObject;  
                }  */

        /*
                使用示例一：

        ... ...
        byte[] packet = new byte[73]{...};
        StructPlane structPlane = new StructPlane();             
        object structType = structPlane;
        ByteArrayToStructure(packet, ref structType, 0);
        复制代码
        使用示例二：

        StructPlane structPlane = new StructPlane();
        structPlane.serialNum = ...;
        structPlane.time = ...;
        structPlane.pitch = ...;
        ... ...
        byte[] datas = StructureToByteArray(structPlane);
         * */
    }
}
