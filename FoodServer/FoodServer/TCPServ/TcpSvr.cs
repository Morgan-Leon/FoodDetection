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
using FoodServer.Class;

namespace FlyTcpFramework
{
    /// <summary> 
    /// 提供TCP连接服务的服务器类 
    /// 
    /// 特点: 
    /// 1.使用hash表保存所有已连接客户端的状态，收到数据时能实现快速查找.每当 
    /// 有一个新的客户端连接就会产生一个新的会话(Session).该Session代表了客 
    /// 户端对象. 
    /// 2.使用异步的Socket事件作为基础，完成网络通讯功能. 
    /// 3.支持带标记的数据报文格式的识别,以完成大数据报文的传输和适应恶劣的网 
    /// 络环境.初步规定该类支持的最大数据报文为640K(即一个数据包的大小不能大于 
    /// 640K,否则服务器程序会自动删除报文数据,认为是非法数据),防止因为数据报文 
    /// 无限制的增长而导致服务器崩溃 
    /// 4.通讯格式默认使用Encoding.Default格式这样就可以和以前32位程序的客户端 
    /// 通讯.也可以使用U-16和U-8的的通讯方式进行.可以在该DatagramResolver类的 
    /// 继承类中重载编码和解码函数,自定义加密格式进行通讯.总之确保客户端与服务 
    /// 器端使用相同的通讯格式 
    /// 5.使用C# native code,将来出于效率的考虑可以将C++代码写成的32位dll来代替 
    /// C#核心代码, 但这样做缺乏可移植性,而且是Unsafe代码(该类的C++代码也存在) 
    /// 6.可以限制服务器的最大登陆客户端数目 
    /// 7.比使用TcpListener提供更加精细的控制和更加强大异步数据传输的功能,可作为 
    /// TcpListener的替代类 
    /// 8.使用异步通讯模式,完全不用担心通讯阻塞和线程问题,无须考虑通讯的细节 
    /// 
    /// </summary> 
    public class TcpSvr
    {
        #region 定义字段

        /// <summary> 
        /// 默认的服务器最大连接客户端端数据 
        /// </summary> 
        public const int DefaultMaxClient = 100;

        /// <summary> 
        /// 接收数据缓冲区大小64K 
        /// </summary> 
        public const int DefaultBufferSize = 40 * 1024 * 1024;

        /// <summary> 
        /// 最大数据报文大小 
        /// </summary> 
        public const int MaxDatagramSize = 40 * 1024 * 1024;

        /// <summary> 
        /// 报文解析器 
        /// </summary> 
        private DatagramResolver _resolver;

        /// <summary> 
        /// 通讯格式编码解码器 
        /// </summary> 
        private Coder _coder;

        /// <summary>
        /// 服务器程序监听的IP地址
        /// </summary>
        private IPAddress _serverIP;
        /// <summary> 
        /// 服务器程序使用的端口 
        /// </summary> 
        private ushort _port;

        /// <summary> 
        /// 服务器程序允许的最大客户端连接数 
        /// </summary> 
        private ushort _maxClient;

        /// <summary> 
        /// 服务器的运行状态 
        /// </summary> 
        private bool _isRun;

        /// <summary> 
        /// 接收数据缓冲区 
        /// </summary> 
        private byte[] _recvDataBuffer;

        /// <summary> 
        /// 服务器使用的异步Socket类, 
        /// </summary> 
        private Socket _svrSock;

        /// <summary> 
        /// 保存所有客户端会话的哈希表 
        /// </summary> 
        private Hashtable _sessionTable;

        /// <summary> 
        /// 当前的连接的客户端数 
        /// </summary> 
        private ushort _clientCount;

        /// <summary>
        /// 服务器端文件保存路径
        /// </summary>
        private string _filePath;

        #endregion

        #region 事件定义

        /// <summary> 
        /// 客户端建立连接事件 
        /// </summary> 
        public event NetEvent ClientConn;

        /// <summary> 
        /// 客户端关闭事件 
        /// </summary> 
        public event NetEvent ClientClose;

        /// <summary> 
        /// 服务器已经满事件 
        /// </summary> 
        public event NetEvent ServerFull;

        /// <summary> 
        /// 服务器接收到数据事件 
        /// </summary> 
        public event NetEvent RecvData;

        #endregion

        #region 构造函数

        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="port">服务器端监听的端口号</param> 
        /// <param name="maxClient">服务器能容纳客户端的最大能力</param> 
        /// <param name="encodingMothord">通讯的编码方式</param> 
        public TcpSvr(IPAddress serverIP, ushort port, ushort maxClient, Coder coder, string filePath)
        {
            _serverIP = serverIP;
            _port = port;
            _maxClient = maxClient;
            _coder = coder;
            if (!filePath.EndsWith("\\"))
                filePath = filePath + "\\";
            _filePath = filePath;
        }


        /// <summary> 
        /// 构造函数(默认使用Default编码方式) 
        /// </summary> 
        /// <param name="port">服务器端监听的端口号</param> 
        /// <param name="maxClient">服务器能容纳客户端的最大能力</param> 
        public TcpSvr(IPAddress serverIP, ushort port, ushort maxClient, string filePath)
        {
            _serverIP = serverIP;
            _port = port;
            _maxClient = maxClient;
            _coder = new Coder(Coder.EncodingMothord.Default);
            if (!filePath.EndsWith("\\"))
                filePath = filePath + "\\";
            _filePath = filePath;
        }


        // <summary> 
        /// 构造函数(默认使用Default编码方式和DefaultMaxClient(100)个客户端的容量) 
        /// </summary> 
        /// <param name="port">服务器端监听的端口号</param> 
        public TcpSvr(IPAddress serverIP, ushort port, string filePath)
            : this(serverIP, port, DefaultMaxClient, filePath)
        {
        }

        #endregion

        #region 属性

        /// <summary> 
        /// 服务器的Socket对象 
        /// </summary> 
        public Socket ServerSocket
        {
            get
            {
                return _svrSock;
            }
        }

        /// <summary> 
        /// 数据报文分析器 
        /// </summary> 
        public DatagramResolver Resovlver
        {
            get
            {
                return _resolver;
            }
            set
            {
                _resolver = value;
            }
        }

        /// <summary> 
        /// 客户端会话数组,保存所有的客户端,不允许对该数组的内容进行修改 
        /// </summary> 
        public Hashtable SessionTable
        {
            get
            {
                return _sessionTable;
            }
        }

        /// <summary> 
        /// 服务器可以容纳客户端的最大能力 
        /// </summary> 
        public int Capacity
        {
            get
            {
                return _maxClient;
            }
        }

        /// <summary> 
        /// 当前的客户端连接数 
        /// </summary> 
        public int SessionCount
        {
            get
            {
                return _clientCount;
            }
        }

        /// <summary> 
        /// 服务器运行状态 
        /// </summary> 
        public bool IsRun
        {
            get
            {
                return _isRun;
            }

        }
        /// <summary> 
        /// 服务器端文件保存路径 
        /// </summary> 
        public string FilePath
        {
            get
            {
                return _filePath;
            }

        }

        #endregion

        #region 公有方法

        /// <summary> 
        /// 启动服务器程序,开始监听客户端请求 
        /// </summary> 
        public virtual void Start()
        {
            if (_isRun)
            {
                throw (new ApplicationException("TcpSvr已经在运行."));
            }

            _sessionTable = new Hashtable(53);

            _recvDataBuffer = new byte[DefaultBufferSize];

            //初始化socket 
            _svrSock = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            //绑定端口 
            IPEndPoint iep = new IPEndPoint(_serverIP, _port);
            _svrSock.Bind(iep);

            //开始监听 
            _svrSock.Listen(5);

            //设置异步方法接受客户端连接 
            _svrSock.BeginAccept(new AsyncCallback(AcceptConn), _svrSock);

            _isRun = true;

        }

        /// <summary> 
        /// 停止服务器程序,所有与客户端的连接将关闭 
        /// </summary> 
        public virtual void Stop()
        {
            if (!_isRun)
            {
                throw (new ApplicationException("TcpSvr已经停止"));
            }

            //这个条件语句，一定要在关闭所有客户端以前调用 
            //否则在EndConn会出现错误 
            _isRun = false;

            //关闭数据连接,负责客户端会认为是强制关闭连接 
            if (_svrSock.Connected)
            {
                _svrSock.Shutdown(SocketShutdown.Both);
            }

            CloseAllClient();

            //清理资源 
            _svrSock.Close();

            _sessionTable = null;

        }


        /// <summary> 
        /// 关闭所有的客户端会话,与所有的客户端连接会断开 
        /// </summary> 
        public virtual void CloseAllClient()
        {
            foreach (Session client in _sessionTable.Values)
            {
                client.Close();
            }

            _sessionTable.Clear();
        }


        /// <summary> 
        /// 关闭一个与客户端之间的会话 
        /// </summary> 
        /// <param name="closeClient">需要关闭的客户端会话对象</param> 
        public virtual void CloseSession(Session closeClient)
        {
            Debug.Assert(closeClient != null);

            if (closeClient != null)
            {

                closeClient.Datagram = null;

                _sessionTable.Remove(closeClient.ID);

                _clientCount--;

                //客户端强制关闭链接 
                if (ClientClose != null)
                {
                    ClientClose(this, new NetEventArgs(closeClient));
                }

                closeClient.Close();
            }
        }
        //发送消息

        /// <summary>
        ///   发送消息  
        ///   2013-05-19 jxb
        ///   传入数据体，组成数据帧
        /// </summary>
        /// <summary> 
        /// 
        public virtual void SendMessage(Session recvDataClient, string datagram, UInt16 MsgId, string centercode)
        {
            //|5b|4-4-2-4|..........|2|5d| == 18
            //
            byte[] data = Encoding.UTF8.GetBytes(datagram);//获取消息
            byte[] msg = new byte[18 + data.Length];
            msg[0] = 0x5b;//头
            MSGHEAD head = new MSGHEAD();
            head.Msg_ID = MsgId;
            head.Msg_Length = (UInt32)msg.Length;

            head.Msg_GNSSCenter = UInt32.Parse(centercode); ;
            TastInfo info = new TastInfo();
            byte[] Head = info.StructureToByteArrayEndian(head); // 将结构转换成字节数组大端

            data.CopyTo(msg, 15);
            Head.CopyTo(msg, 1);
            //加入CRC校验5
            byte[] crcbuf = new byte[msg.Length - 4];
            Array.Copy(crcbuf, 0, msg, 1, (uint)crcbuf.Length);


            msg[msg.Length - 1] = 0x5d;//尾
            recvDataClient.ClientSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None,
               new AsyncCallback(SendDataEnd), recvDataClient.ClientSocket);
        }

        /// <summary> 
        /// 发送数据 
        /// </summary> 
        /// <param name="recvDataClient">接收数据的客户端会话</param> 
        /// <param name="datagram">数据报文</param> 
        public virtual void SendText(Session recvDataClient, string datagram)
        {
            //获得数据编码 
            byte[] data = _coder.GetTextBytes(datagram);

            recvDataClient.ClientSocket.BeginSend(data, 0, data.Length, SocketFlags.None,
                new AsyncCallback(SendDataEnd), recvDataClient.ClientSocket);

        }
        public virtual void SendFile(Session recvDataClient, string FilePath)
        {
            if (File.Exists(FilePath))
            {
                byte[] data = _coder.GetFileBytes(FilePath);

                recvDataClient.ClientSocket.BeginSend(data, 0, data.Length, SocketFlags.None,
                    new AsyncCallback(SendDataEnd), recvDataClient.ClientSocket);
            }
            else
            {
                throw new Exception("文件不存在");
            }
        }
        #endregion

        #region 受保护方法
        /// <summary> 
        /// 关闭一个客户端Socket,首先需要关闭Session 
        /// </summary> 
        /// <param name="client">目标Socket对象</param> 
        /// <param name="exitType">客户端退出的类型</param> 
        protected virtual void CloseClient(Socket client, Session.ExitType exitType)
        {
            Debug.Assert(client != null);

            //查找该客户端是否存在,如果不存在,抛出异常 
            Session closeClient = FindSession(client);

            closeClient.TypeOfExit = exitType;

            if (closeClient != null)
            {
                CloseSession(closeClient);
            }
            else
            {
                throw (new ApplicationException("需要关闭的Socket对象不存在"));
            }
        }


        /// <summary> 
        /// 客户端连接处理函数 
        /// </summary> 
        /// <param name="iar">欲建立服务器连接的Socket对象</param> 
        protected virtual void AcceptConn(IAsyncResult iar)
        {
            //如果服务器停止了服务,就不能再接收新的客户端 
            if (!_isRun)
            {
                return;
            }

            //接受一个客户端的连接请求 
            Socket oldserver = (Socket)iar.AsyncState;

            Socket client = oldserver.EndAccept(iar);

            //检查是否达到最大的允许的客户端数目 
            if (_clientCount == _maxClient)
            {
                //服务器已满,发出通知 
                if (ServerFull != null)
                {
                    ServerFull(this, new NetEventArgs(new Session(client)));
                }

            }
            else
            {

                Session newSession = new Session(client);

                _sessionTable.Add(newSession.ID, newSession);

                //客户端引用计数+1 
                _clientCount++;

                //开始接受来自该客户端的数据 
                client.BeginReceive(_recvDataBuffer, 0, _recvDataBuffer.Length, SocketFlags.None,
                    new AsyncCallback(ReceiveData), client);

                //新的客户段连接,发出通知 
                if (ClientConn != null)
                {
                    ClientConn(this, new NetEventArgs(newSession));
                }
            }

            //继续接受客户端 
            _svrSock.BeginAccept(new AsyncCallback(AcceptConn), _svrSock);
        }


        /// <summary> 
        /// 通过Socket对象查找Session对象 
        /// </summary> 
        /// <param name="client"></param> 
        /// <returns>找到的Session对象,如果为null,说明并不存在该回话</returns> 
        private Session FindSession(Socket client)
        {
            SessionId id = new SessionId((int)client.Handle);

            return (Session)_sessionTable[id];
        }


        /// <summary> 
        /// 接受数据完成处理函数，异步的特性就体现在这个函数中， 
        /// 收到数据后，会自动解析为字符串报文 
        /// </summary> 
        /// <param name="iar">目标客户端Socket</param> 
        protected virtual void ReceiveData(IAsyncResult iar)
        {
            Socket client = (Socket)iar.AsyncState;

            try
            {
                //如果两次开始了异步的接收,所以当客户端退出的时候 
                //会两次执行EndReceive 

                int recv = client.EndReceive(iar);

                if (recv == 0)
                {
                    //正常的关闭 
                    CloseClient(client, Session.ExitType.NormalExit);
                    return;
                }
                string receivedData;
                byte[] rcbuffer = new byte[recv - 1];
                if (_recvDataBuffer[0] == 0x5b)
                {
                    receivedData = System.Text.Encoding.UTF8.GetString(_recvDataBuffer, 1, recv - 1);
                    // Session DataSession = FindSession(client);
                    //DataSession.RvBufer = _recvDataBuffer;
                    Array.Copy(_recvDataBuffer, 1, rcbuffer, 0, recv - 1);
                }
                else
                {
                    string fileName = System.Text.Encoding.UTF8.GetString(_recvDataBuffer, 6, _recvDataBuffer[1] - 1);
                    _coder.SaveFile(_filePath + fileName, _recvDataBuffer);
                    receivedData = "Receive File:" + fileName + "]";
                }
                //发布收到数据的事件 
                if (RecvData != null)
                {
                    Session sendDataSession = FindSession(client);

                    Debug.Assert(sendDataSession != null);

                    //如果定义了报文的尾标记,需要处理报文的多种情况 
                    if (_resolver != null)
                    {
                        if (sendDataSession.Datagram != null &&
                            sendDataSession.Datagram.Length != 0)
                        {
                            //加上最后一次通讯剩余的报文片断 
                            receivedData = sendDataSession.Datagram + receivedData;
                            rcbuffer.CopyTo(sendDataSession.RvBufer, rcbuffer.Length);//jxb
                            // RecvData(this, new NetEventArgs(sendDataSession));//jxb
                        }

                        string[] recvDatagrams = _resolver.Resolve(ref receivedData);


                        foreach (string newDatagram in recvDatagrams)
                        {
                            //深拷贝,为了保持Datagram的对立性 
                            ICloneable copySession = (ICloneable)sendDataSession;

                            Session clientSession = (Session)copySession.Clone();

                            clientSession.Datagram = newDatagram;
                            //
                            Array.Copy(rcbuffer, clientSession.RvBufer, rcbuffer.Length);//jxb
                            //发布一个报文消息 
                            RecvData(this, new NetEventArgs(clientSession));
                        }

                        //剩余的代码片断,下次接收的时候使用 
                        sendDataSession.Datagram = receivedData;
                        Array.Copy(rcbuffer, sendDataSession.RvBufer, rcbuffer.Length);//jxb
                        if (sendDataSession.Datagram.Length > MaxDatagramSize)
                        {
                            sendDataSession.Datagram = null;
                            sendDataSession.RvBufer = null;//jxb
                        }

                    }
                    //没有定义报文的尾标记,直接交给消息订阅者使用 
                    else
                    {
                        ICloneable copySession = (ICloneable)sendDataSession;

                        Session clientSession = (Session)copySession.Clone();

                        clientSession.Datagram = receivedData;
                        Array.Copy(rcbuffer, clientSession.RvBufer, rcbuffer.Length);//jxb
                        RecvData(this, new NetEventArgs(clientSession));
                    }

                }//end of if(RecvData!=null) 

                //继续接收来自来客户端的数据 
                client.BeginReceive(_recvDataBuffer, 0, _recvDataBuffer.Length, SocketFlags.None,
                    new AsyncCallback(ReceiveData), client);

            }
            catch (SocketException ex)
            {
                //客户端退出 
                if (10054 == ex.ErrorCode)
                {
                    //客户端强制关闭 
                    CloseClient(client, Session.ExitType.ExceptionExit);
                }

            }
            catch (ObjectDisposedException ex)
            {
                //这里的实现不够优雅 
                //当调用CloseSession()时,会结束数据接收,但是数据接收 
                //处理中会调用int recv = client.EndReceive(iar); 
                //就访问了CloseSession()已经处置的对象 
                //我想这样的实现方法也是无伤大雅的. 
                if (ex != null)
                {
                    ex = null;
                    //DoNothing; 
                }
            }

        }


        /// <summary> 
        /// 发送数据完成处理函数 
        /// </summary> 
        /// <param name="iar">目标客户端Socket</param> 
        protected virtual void SendDataEnd(IAsyncResult iar)
        {
            Socket client = (Socket)iar.AsyncState;

            int sent = client.EndSend(iar);
        }

        #endregion

    }
}
