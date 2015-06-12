///////////////////////////////////////////////////////
//NSTCPFramework
//版本：1.0.0.1
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
    /// 客户端与服务器之间的会话类 
    /// 
    /// 说明: 
    /// 会话类包含远程通讯端的状态,这些状态包括Socket,报文内容, 
    /// 客户端退出的类型(正常关闭,强制退出两种类型) 
    /// </summary> 
    public class Session : ICloneable
    {
        #region 字段

        /// <summary> 
        /// 会话ID 
        /// </summary> 
        private SessionId _id;
        private UInt32 _userid;//保存用户ID
        private string centercode;

        /// <summary> 
        /// 客户端发送到服务器的报文 
        /// 注意:在有些情况下报文可能只是报文的片断而不完整 
        /// </summary> 
        private string _datagram;

        /// <summary> 
        /// 客户端的Socket 
        /// </summary> 
        private Socket _cliSock;

        /// <summary> 
        /// 客户端的退出类型 
        /// </summary> 
        private ExitType _exitType;

        /// <summary> 
        /// 退出类型枚举 
        /// </summary> 
        public enum ExitType
        {
            NormalExit,
            ExceptionExit
        };

        #endregion

        #region 属性

        /// <summary> 
        /// 返回会话的ID 
        /// </summary> 
        public SessionId ID
        {
            get
            {
                return _id;
            }
        }

        /// <summary> 
        /// 存取会话的报文 
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
        ///   存取用户Id
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
        /// 获得与客户端会话关联的Socket对象 
        /// </summary> 
        public Socket ClientSocket
        {
            get
            {
                return _cliSock;
            }
        }

        /// <summary> 
        /// 存取客户端的退出方式 
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

        #region 方法

        /// <summary> 
        /// 使用Socket对象的Handle值作为HashCode,它具有良好的线性特征. 
        /// </summary> 
        /// <returns></returns> 
        public override int GetHashCode()
        {
            return (int)_cliSock.Handle;
        }

        /// <summary> 
        /// 返回两个Session是否代表同一个客户端 
        /// </summary> 
        /// <param name="obj"></param> 
        /// <returns></returns> 
        public override bool Equals(object obj)
        {
            Session rightObj = (Session)obj;

            return (int)_cliSock.Handle == (int)rightObj.ClientSocket.Handle;

        }

        /// <summary> 
        /// 重载ToString()方法,返回Session对象的特征 
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
        /// 构造函数 
        /// </summary> 
        /// <param name="cliSock">会话使用的Socket连接</param> 
        public Session(Socket cliSock)
        {
            Debug.Assert(cliSock != null);

            _cliSock = cliSock;
            
            _id = new SessionId((int)cliSock.Handle);
        }

        /// <summary> 
        /// 关闭会话 
        /// </summary> 
        public void Close()
        {
            Debug.Assert(_cliSock != null);

            //关闭数据的接受和发送 
            _cliSock.Shutdown(SocketShutdown.Both);

            //清理资源 
            _cliSock.Close();
        }

        #endregion

        #region ICloneable 成员

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
    /// 唯一的标志一个Session,辅助Session对象在Hash表中完成特定功能 
    /// </summary> 
    public class SessionId
    {
        /// <summary> 
        /// 与Session对象的Socket对象的Handle值相同,必须用这个值来初始化它 
        /// </summary> 
        private int _id;

        /// <summary> 
        /// 返回ID值 
        /// </summary> 
        public int ID
        {
            get
            {
                return _id;
            }
        }

        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="id">Socket的Handle值</param> 
        public SessionId(int id)
        {
            _id = id;
        }

        /// <summary> 
        /// 重载.为了符合Hashtable键值特征 
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
        /// 重载.为了符合Hashtable键值特征 
        /// </summary> 
        /// <returns></returns> 
        public override int GetHashCode()
        {
            return _id;
        }

        /// <summary> 
        /// 重载,为了方便显示输出 
        /// </summary> 
        /// <returns></returns> 
        public override string ToString()
        {
            return _id.ToString();
        }

    }

}
