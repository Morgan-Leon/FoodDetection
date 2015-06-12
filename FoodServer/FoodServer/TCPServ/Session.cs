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

namespace FlyTcpFramework
{
    /// <summary> 
    /// �ͻ����������֮��ĻỰ�� 
    /// 
    /// ˵��: 
    /// �Ự�����Զ��ͨѶ�˵�״̬,��Щ״̬����Socket,��������, 
    /// �ͻ����˳�������(�����ر�,ǿ���˳���������) 
    /// </summary> 
    public class Session : ICloneable
    {
        #region �ֶ�

        /// <summary> 
        /// �ỰID 
        /// </summary> 
        private SessionId _id;
        private UInt32 _userid;//�����û�ID
        private string centercode;

        /// <summary> 
        /// �ͻ��˷��͵��������ı��� 
        /// ע��:����Щ����±��Ŀ���ֻ�Ǳ��ĵ�Ƭ�϶������� 
        /// </summary> 
        private string _datagram;

        /// <summary> 
        /// �ͻ��˵�Socket 
        /// </summary> 
        private Socket _cliSock;

        /// <summary> 
        /// �ͻ��˵��˳����� 
        /// </summary> 
        private ExitType _exitType;

        /// <summary> 
        /// �˳�����ö�� 
        /// </summary> 
        public enum ExitType
        {
            NormalExit,
            ExceptionExit
        };

        #endregion

        #region ����

        /// <summary> 
        /// ���ػỰ��ID 
        /// </summary> 
        public SessionId ID
        {
            get
            {
                return _id;
            }
        }

        /// <summary> 
        /// ��ȡ�Ự�ı��� 
        /// </summary> 
        public string Datagram
        {
            get
            {
                return _datagram;
            }
            set
            {
                _datagram = value;
            }
        }
        private byte[] _rvbuffer = new byte[4 * 1024 * 1024];
        public byte[] RvBufer
        {
            get
            {
                return _rvbuffer;
            }
            set
            {
                _rvbuffer = value;
            }
        }
        /// <summary>
        ///   ��ȡ�û�Id
        /// </summary>
        public UInt32 UserId
        {
            get
            {
                return _userid;
            }
            set
            {
                _userid = value;
            }
        }

        /// <summary>
        ///   centercode
        /// </summary>
        public string CenterCode
        {
            get
            {
                return centercode;

            }
            set
            {
                centercode = value;
            }
        }
        /// <summary> 
        /// �����ͻ��˻Ự������Socket���� 
        /// </summary> 
        public Socket ClientSocket
        {
            get
            {
                return _cliSock;
            }
        }

        /// <summary> 
        /// ��ȡ�ͻ��˵��˳���ʽ 
        /// </summary> 
        public ExitType TypeOfExit
        {
            get
            {
                return _exitType;
            }

            set
            {
                _exitType = value;
            }
        }

        #endregion

        #region ����

        /// <summary> 
        /// ʹ��Socket�����Handleֵ��ΪHashCode,���������õ���������. 
        /// </summary> 
        /// <returns></returns> 
        public override int GetHashCode()
        {
            return (int)_cliSock.Handle;
        }

        /// <summary> 
        /// ��������Session�Ƿ����ͬһ���ͻ��� 
        /// </summary> 
        /// <param name="obj"></param> 
        /// <returns></returns> 
        public override bool Equals(object obj)
        {
            Session rightObj = (Session)obj;

            return (int)_cliSock.Handle == (int)rightObj.ClientSocket.Handle;

        }

        /// <summary> 
        /// ����ToString()����,����Session��������� 
        /// </summary> 
        /// <returns></returns> 
        public override string ToString()
        {
            string result = string.Format("Session:{0},IP:{1}",
                _id, _cliSock.RemoteEndPoint.ToString());

            //result.C 
            return result;
        }

        /// <summary> 
        /// ���캯�� 
        /// </summary> 
        /// <param name="cliSock">�Ựʹ�õ�Socket����</param> 
        public Session(Socket cliSock)
        {
            Debug.Assert(cliSock != null);

            _cliSock = cliSock;
            
            _id = new SessionId((int)cliSock.Handle);
        }

        /// <summary> 
        /// �رջỰ 
        /// </summary> 
        public void Close()
        {
            Debug.Assert(_cliSock != null);

            //�ر����ݵĽ��ܺͷ��� 
            _cliSock.Shutdown(SocketShutdown.Both);

            //������Դ 
            _cliSock.Close();
        }

        #endregion

        #region ICloneable ��Ա

        object System.ICloneable.Clone()
        {
            Session newSession = new Session(_cliSock);
            newSession.Datagram = _datagram;
            newSession.TypeOfExit = _exitType;
            newSession.UserId = _userid;
            newSession.CenterCode = centercode;

            return newSession;
        }

        #endregion
    }


    /// <summary> 
    /// Ψһ�ı�־һ��Session,����Session������Hash��������ض����� 
    /// </summary> 
    public class SessionId
    {
        /// <summary> 
        /// ��Session�����Socket�����Handleֵ��ͬ,���������ֵ����ʼ���� 
        /// </summary> 
        private int _id;

        /// <summary> 
        /// ����IDֵ 
        /// </summary> 
        public int ID
        {
            get
            {
                return _id;
            }
        }

        /// <summary> 
        /// ���캯�� 
        /// </summary> 
        /// <param name="id">Socket��Handleֵ</param> 
        public SessionId(int id)
        {
            _id = id;
        }

        /// <summary> 
        /// ����.Ϊ�˷���Hashtable��ֵ���� 
        /// </summary> 
        /// <param name="obj"></param> 
        /// <returns></returns> 
        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                SessionId right = (SessionId)obj;

                return _id == right._id;
            }
            else if (this == null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary> 
        /// ����.Ϊ�˷���Hashtable��ֵ���� 
        /// </summary> 
        /// <returns></returns> 
        public override int GetHashCode()
        {
            return _id;
        }

        /// <summary> 
        /// ����,Ϊ�˷�����ʾ��� 
        /// </summary> 
        /// <returns></returns> 
        public override string ToString()
        {
            return _id.ToString();
        }

    }

}
