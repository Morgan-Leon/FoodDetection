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
using FoodServer.Class;
using System.Collections.Generic;
using System.Threading;
using GHCS.DataBase;
using GHCS;
using MySql.Data.MySqlClient;
using System.Data;

namespace FlyTcpFramework
{
    /// <summary> 
    /// �ṩTCP���ӷ���ķ������� 
    /// 
    /// �ص�: 
    /// 1.ʹ��hash�������������ӿͻ��˵�״̬���յ�����ʱ��ʵ�ֿ��ٲ���.ÿ�� 
    /// ��һ���µĿͻ������Ӿͻ����һ���µĻỰ(Session).��Session�����˿� 
    /// ���˶���. 
    /// 2.ʹ���첽��Socket�¼���Ϊ�������������ͨѶ����. 
    /// 3.֧�ִ���ǵ����ݱ��ĸ�ʽ��ʶ��,����ɴ����ݱ��ĵĴ������Ӧ���ӵ��� 
    /// �绷��.�����涨����֧�ֵ�������ݱ���Ϊ640K(��һ�����ݰ��Ĵ�С���ܴ��� 
    /// 640K,���������������Զ�ɾ����������,��Ϊ�ǷǷ�����),��ֹ��Ϊ���ݱ��� 
    /// �����Ƶ����������·��������� 
    /// 4.ͨѶ��ʽĬ��ʹ��Encoding.Default��ʽ�����Ϳ��Ժ���ǰ32λ����Ŀͻ��� 
    /// ͨѶ.Ҳ����ʹ��U-16��U-8�ĵ�ͨѶ��ʽ����.�����ڸ�DatagramResolver��� 
    /// �̳��������ر���ͽ��뺯��,�Զ�����ܸ�ʽ����ͨѶ.��֮ȷ���ͻ�������� 
    /// ����ʹ����ͬ��ͨѶ��ʽ 
    /// 5.ʹ��C# native code,��������Ч�ʵĿ��ǿ��Խ�C++����д�ɵ�32λdll������ 
    /// C#���Ĵ���, ��������ȱ������ֲ��,������Unsafe����(�����C++����Ҳ����) 
    /// 6.�������Ʒ�����������½�ͻ�����Ŀ 
    /// 7.��ʹ��TcpListener�ṩ���Ӿ�ϸ�Ŀ��ƺ͸���ǿ���첽���ݴ���Ĺ���,����Ϊ 
    /// TcpListener������� 
    /// 8.ʹ���첽ͨѶģʽ,��ȫ���õ���ͨѶ�������߳�����,���뿼��ͨѶ��ϸ�� 
    /// 
    /// </summary> 
    public class TcpSvr
    {
        MySqlDataBase dbMySql = MySqlDataBase.getInstance();
        string databasename;
        IDataBase database = MySqlDataBase.getInstance();
        MySqlDataAdapter myDataAdpter = new MySqlDataAdapter();
        DataSet myDataSet = new DataSet();
        #region �����ֶ�

        /// <summary> 
        /// Ĭ�ϵķ�����������ӿͻ��˶����� 
        /// </summary> 
        public const int DefaultMaxClient = 100;

        /// <summary> 
        /// �������ݻ�������С8K 
        /// </summary> 
        public const int DefaultBufferSize = 8 * 1024;

        /// <summary> 
        /// ������ݱ��Ĵ�С 
        /// </summary> 
        public const int MaxDatagramSize = 4 * 1024 * 1024;

        /// <summary> 
        /// ���Ľ����� 
        /// </summary> 
        private DatagramResolver _resolver;

        /// <summary> 
        /// ͨѶ��ʽ��������� 
        /// </summary> 
        private Coder _coder;

        public bool FileBegine = false;
        public string device_id;
        public bool IsPic = false;//��Ƭor��Ƶ
        /// <summary>
        /// ���������������IP��ַ
        /// </summary>
        private IPAddress _serverIP;
        /// <summary> 
        /// ����������ʹ�õĶ˿� 
        /// </summary> 
        private ushort _port;

        /// <summary> 
        /// ������������������ͻ��������� 
        /// </summary> 
        private ushort _maxClient;

        /// <summary> 
        /// ������������״̬ 
        /// </summary> 
        private bool _isRun = false;

        /// <summary> 
        /// �������ݻ����� 
        /// </summary> 
        private byte[] _recvDataBuffer = new byte[DefaultBufferSize];
        public byte[] GetRcvBuffer
        {
            get
            {
                return _recvDataBuffer;
            }

        }
        //����ά���û�ע��
        //  public Hashtable Usertable = new Hashtable(100);
        /// <summary> 
        /// ������ʹ�õ��첽Socket��, 
        /// </summary> 
        private Socket _svrSock = null;

        /// <summary> 
        /// �������пͻ��˻Ự�Ĺ�ϣ�� 
        /// </summary> 
        public Hashtable _sessionTable;

        /// <summary> 
        /// ��ǰ�����ӵĿͻ����� 
        /// </summary> 
        private ushort _clientCount;

        /// <summary>
        /// ���������ļ�����·��
        /// </summary>
        private string _filePath;

        #endregion

        #region �¼�����

        /// <summary> 
        /// �ͻ��˽��������¼� 
        /// </summary> 
        public event NetEvent ClientConn;

        /// <summary> 
        /// �ͻ��˹ر��¼� 
        /// </summary> 
        public event NetEvent ClientClose;

        /// <summary> 
        /// �������Ѿ����¼� 
        /// </summary> 
        public event NetEvent ServerFull;

        /// <summary> 
        /// ���������յ������¼� 
        /// </summary> 
        public event NetEvent RecvData;

        #endregion

        #region ���캯��

        /// <summary> 
        /// ���캯�� 
        /// </summary> 
        /// <param name="port">�������˼����Ķ˿ں�</param> 
        /// <param name="maxClient">�����������ɿͻ��˵��������</param> 
        /// <param name="encodingMothord">ͨѶ�ı��뷽ʽ</param> 
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
        /// ���캯��(Ĭ��ʹ��Default���뷽ʽ) 
        /// </summary> 
        /// <param name="port">�������˼����Ķ˿ں�</param> 
        /// <param name="maxClient">�����������ɿͻ��˵��������</param> 
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
        /// ���캯��(Ĭ��ʹ��Default���뷽ʽ��DefaultMaxClient(100)���ͻ��˵�����) 
        /// </summary> 
        /// <param name="port">�������˼����Ķ˿ں�</param> 
        public TcpSvr(IPAddress serverIP, ushort port, string filePath)
            : this(serverIP, port, DefaultMaxClient, filePath)
        {
        }

        #endregion

        #region ����

        /// <summary> 
        /// ��������Socket���� 
        /// </summary> 
        public Socket ServerSocket
        {
            get
            {
                return _svrSock;
            }
        }

        /// <summary> 
        /// ���ݱ��ķ����� 
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
        /// �ͻ��˻Ự����,�������еĿͻ���,������Ը���������ݽ����޸� 
        /// </summary> 
        public Hashtable SessionTable
        {
            get
            {
                return _sessionTable;
            }
        }

        /// <summary> 
        /// �������������ɿͻ��˵�������� 
        /// </summary> 
        public int Capacity
        {
            get
            {
                return _maxClient;
            }
        }

        /// <summary> 
        /// ��ǰ�Ŀͻ��������� 
        /// </summary> 
        public int SessionCount
        {
            get
            {
                return _clientCount;
            }
        }

        /// <summary> 
        /// ����������״̬ 
        /// </summary> 
        public bool IsRun
        {
            get
            {
                return _isRun;
            }

        }
        /// <summary> 
        /// ���������ļ�����·�� 
        /// </summary> 
        public string FilePath
        {
            get
            {
                return _filePath;
            }


        }

        #endregion

        #region ���з���
        private bool CreateServerSocket()
        {
            try
            {

                _svrSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // m_serverSocket.Bind(new IPEndPoint(IPAddress.Any, m_servertPort));
                _svrSock.Bind(new IPEndPoint(_serverIP, _port));
                _svrSock.Listen(10);
                Console.Write("start");
                return true;
            }
            catch (Exception err)
            {
                // this.OnServerException(err);
                return false;
            }
        }

        private bool CheckSocketIP(Socket clientSocket)
        {

            IPEndPoint iep = (IPEndPoint)clientSocket.RemoteEndPoint;
            string ip = iep.Address.ToString();

            if (ip.Substring(0, 7) == "127.0.0")   // local machine
            {
                return true;
            }

            lock (_sessionTable)
            {
                int sameIPCount = 0;
                foreach (Session session in _sessionTable.Values)
                {
                    if (session.ToString() == ip)
                    {
                        sameIPCount++;
                        if (sameIPCount > 100)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public void ReceiveDatagram()
        {
            lock (this)
            {
                //                 if (this.State != TSessionState.Active)
                //                 {
                //                     return;
                //                 }

                try  // һ���ͻ������������� �����Ӻ������Ͽ��������ڸô���������ϵͳ����Ϊ�Ǵ���
                {
                    // ��ʼ�������Ըÿͻ��˵�����
                    int bufferOffset = 0;
                    //_svrSock.BeginReceive(_recvDataBuffer, bufferOffset, _recvDataBuffer.Length, SocketFlags.None, ReceiveData, this);
                    _svrSock.BeginReceive(_recvDataBuffer, 0, _recvDataBuffer.Length, SocketFlags.None,
                    new AsyncCallback(ReceiveData), _svrSock);
                }
                catch (Exception err)  // �� Socket �쳣��׼���رոûỰ
                {
                    //                     this.DisconnectType = TDisconnectType.Exception;
                    //                     this.State = TSessionState.Inactive;
                    // 
                    //                     this.OnSessionReceiveException(err);
                    Console.Write(err.ToString());
                }
            }
        }
        /// <summary>
        /// ����һ���Ự����
        /// </summary>
        private void AddSession(Socket clientSocket)
        {
            //Interlocked.Increment(ref m_sessionSequenceNo);

            Session session = new Session(clientSocket);
            //session.Initiate(m_maxDatagramSize, m_sessionSequenceNo, clientSocket, m_databaseObj, m_bufferManager);

            // session.DatagramDelimiterError += new EventHandler<TSessionEventArgs>(this.OnDatagramDelimiterError);
            // session.DatagramOversizeError += new EventHandler<TSessionEventArgs>(this.OnDatagramOversizeError);
            // session.DatagramError += new EventHandler<TSessionEventArgs>(this.OnDatagramError);
            // session.DatagramAccepted += new EventHandler<TSessionEventArgs>(this.OnDatagramAccepted);
            //  session.DatagramHandled += new EventHandler<TSessionEventArgs>(this.OnDatagramHandled);
            //  session.SessionReceiveException += new EventHandler<TSessionExceptionEventArgs>(this.OnSessionReceiveException);
            // session.SessionSendException += new EventHandler<TSessionExceptionEventArgs>(this.OnSessionSendException);

            //session.ShowDebugMessage += new EventHandler<TExceptionEventArgs>(this.ShowDebugMessage);

            lock (_sessionTable)
            {
                _sessionTable.Add(session.ID, session);
            }
            ReceiveDatagram();

            this.OnSessionConnected(session);
        }
        protected virtual void OnSessionConnected(Session session)
        {
            //             Interlocked.Increment(ref _maxClient);
            // 
            //             EventHandler<TSessionEventArgs> handler = this.SessionConnected;
            //             if (handler != null)
            //             {
            //                 TSessionEventArgs e = new TSessionEventArgs(session);
            //                 handler(this, e);
            //             }
        }
        /// <summary> 
        /// ��������������,��ʼ�����ͻ������� 
        /// </summary> 
        public virtual void Start()
        {
            if (_isRun)
            {
                throw (new ApplicationException("TcpSvr�Ѿ�������."));
            }
            _sessionTable = new Hashtable(53);

            _recvDataBuffer = new byte[DefaultBufferSize];


            // allDone.Reset();

            //��ʼ��socket 
            _svrSock = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            //�󶨶˿� 
            IPEndPoint iep = new IPEndPoint(_serverIP, _port);
            try
            {
                _svrSock.Bind(iep);

                //��ʼ���� 
                _svrSock.Listen(10);

                //   while (true)
                {
                    // Set the event to nonsignaled state.
                    // allDone.Reset();
                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection...");
                    _svrSock.BeginAccept(new AsyncCallback(AcceptConn), _svrSock);

                    // Wait until a connection is made before continuing.
                    //  allDone.WaitOne();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


            //�����첽�������ܿͻ������� 
            //                 _svrSock.BeginAccept(new AsyncCallback(AcceptConn), _svrSock);
            //                // allDone.WaitOne();
            _isRun = true;

            //                 Console.WriteLine("\nPress ENTER to continue...");
            //                 Console.Read();




        }

        /// <summary> 
        /// ֹͣ����������,������ͻ��˵����ӽ��ر� 
        /// </summary> 
        public virtual void Stop()
        {
            if (!_isRun)
            {
                throw (new ApplicationException("TcpSvr�Ѿ�ֹͣ"));
            }

            //���������䣬һ��Ҫ�ڹر����пͻ�����ǰ���� 
            //������EndConn����ִ��� 
            _isRun = false;

            //�ر���������,����ͻ��˻���Ϊ��ǿ�ƹر����� 
            if (_svrSock.Connected)
            {
                _svrSock.Shutdown(SocketShutdown.Both);
            }

            CloseAllClient();

            //������Դ 
            _svrSock.Close();

            _sessionTable = null;

        }


        /// <summary> 
        /// �ر����еĿͻ��˻Ự,�����еĿͻ������ӻ�Ͽ� 
        /// </summary> 
        public virtual void CloseAllClient()
        {
            foreach (Session client in _sessionTable.Values)
            {
                client.Close();

            }
            // Usertable.Clear();//���
            //�������ݿ�״̬
            string sql = "UPDATE device SET Device_Status = 0 ";
            database.Open();
            database.ExcuteNonQuery(sql);
            database.Close();
            _sessionTable.Clear();
        }


        /// <summary> 
        /// �ر�һ����ͻ���֮��ĻỰ 
        /// </summary> 
        /// <param name="closeClient">��Ҫ�رյĿͻ��˻Ự����</param> 
        public virtual void CloseSession(Session closeClient)
        {
            Debug.Assert(closeClient != null);

            if (closeClient != null)
            {

                closeClient.Datagram = null;

                #region
                //�������ݿ�״̬
                string sql = "UPDATE device SET Device_Status = 0 WHERE Device_ID = '" + closeClient.DeviceId + "'";
                database.Open();
                database.ExcuteNonQuery(sql);
                database.Close();


                #endregion
                _sessionTable.Remove(closeClient.ID);

                _clientCount--;

                //�ͻ���ǿ�ƹر����� 
                if (ClientClose != null)
                {
                    ClientClose(this, new NetEventArgs(closeClient));
                }

                closeClient.Close();
            }
        }



        //������Ϣ
        public virtual void SendMessage(Session recvDataClient, byte[] datagram, UInt16 MsgId, UInt32 centercode)
        {
            //|5b|4-4-2-4|..........|2|5d| == 18
            //
            //��ȡ��Ϣ����
            // byte[] data = System.Text.Encoding.UTF8.GetBytes(datagram);
            //string str = System.Text.Encoding.UTF8.GetString(data);
            TastInfo info = new TastInfo();
            byte[] msg = new byte[18 + datagram.Length];
            msg[0] = 0x5b;//ͷ
            MSGHEAD head = new MSGHEAD();
            head.Msg_ID = MsgId;
            head.Msg_Length = (UInt32)msg.Length;
            head.Msg_GNSSCenter = centercode;
            //ͷ����
            byte[] Head = info.StructureToByteArrayEndian(head);

            datagram.CopyTo(msg, 15);
            Head.CopyTo(msg, 1);
            //����CRCУ��5


            msg[msg.Length - 1] = 0x5d;//β

            recvDataClient.ClientSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None,
              new AsyncCallback(SendDataEnd), recvDataClient.ClientSocket);
        }
        /// <summary>
        ///   ������Ϣ  
        ///   2013-05-19 jxb
        ///   ���������壬�������֡
        /// </summary>
        /// <summary> 
        /// 
        public virtual void SendMessage(Session recvDataClient, string datagram, UInt16 MsgId, string centercode)
        {
            //|5b|4-4-2-4|..........|2|5d| == 18
            //

            //��ȡ��Ϣ����
            byte[] data = Encoding.UTF8.GetBytes(datagram);
            TastInfo info = new TastInfo();
            byte[] msg = new byte[18 + data.Length];
            msg[0] = 0x5b;//ͷ
            MSGHEAD head = new MSGHEAD();
            head.Msg_ID = MsgId;
            head.Msg_Length = (UInt32)msg.Length;
            head.Msg_GNSSCenter = UInt32.Parse(centercode);
            //ͷ����
            byte[] Head = info.StructureToByteArrayEndian(head);
            //����Ϣ���ݽ�����֡
            byte[] MesBody = new byte[data.Length];


            MesBody.CopyTo(msg, 15);
            Head.CopyTo(msg, 1);
            //����CRCУ��5


            msg[msg.Length - 1] = 0x5d;//β
            recvDataClient.ClientSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None,
              new AsyncCallback(SendDataEnd), recvDataClient.ClientSocket);
        }

        /// <summary> 
        /// �������� 
        /// </summary> 
        /// <param name="recvDataClient">�������ݵĿͻ��˻Ự</param> 
        /// <param name="datagram">���ݱ���</param> 
        public virtual void SendText(Session recvDataClient, string datagram)
        {
            //������ݱ��� 
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
                throw new Exception("�ļ�������");
            }
        }
        #endregion

        #region �ܱ�������
        /// <summary> 
        /// �ر�һ���ͻ���Socket,������Ҫ�ر�Session 
        /// </summary> 
        /// <param name="client">Ŀ��Socket����</param> 
        /// <param name="exitType">�ͻ����˳�������</param> 
        protected virtual void CloseClient(Socket client, Session.ExitType exitType)
        {
            Debug.Assert(client != null);

            //���Ҹÿͻ����Ƿ����,���������,�׳��쳣 
            Session closeClient = FindSession(client);

            closeClient.TypeOfExit = exitType;

            if (closeClient != null)
            {
                CloseSession(closeClient);
            }
            else
            {
                throw (new ApplicationException("��Ҫ�رյ�Socket���󲻴���"));
            }
        }
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        // State object for reading client data asynchronously
        public class StateObject
        {
            // Client  socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 8 * 1024;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder sb = new StringBuilder();
        }

        /// <summary> 
        /// �ͻ������Ӵ����� 
        /// </summary> 
        /// <param name="iar">���������������ӵ�Socket����</param> 
        protected virtual void AcceptConn(IAsyncResult iar)
        {
            // Signal the main thread to continue.
            // allDone.Set();
            //���������ֹͣ�˷���,�Ͳ����ٽ����µĿͻ��� 
            if (!_isRun)
            {
                return;
            }

            //����һ���ͻ��˵��������� 
            Socket server = (Socket)iar.AsyncState;
            Socket client = server.EndAccept(iar);

            //����Ƿ�ﵽ��������Ŀͻ�����Ŀ 
            if (_clientCount == _maxClient)
            {
                //����������,����֪ͨ 
                if (ServerFull != null)
                {
                    ServerFull(this, new NetEventArgs(new Session(client)));
                }

            }
            else
            {
                if (CheckSocketIP(client))
                {


                    Session newSession = new Session(client);

                    _sessionTable.Add(newSession.ID, newSession);

                    //�ͻ������ü���+1 
                    _clientCount++;
                    Console.Write(_clientCount);
                    //��ʼ�������Ըÿͻ��˵����� 
                    try
                    {

                        client.BeginReceive(_recvDataBuffer, 0, _recvDataBuffer.Length, SocketFlags.None,
                     new AsyncCallback(ReceiveData), client);

                    }
                    catch (System.Exception ex)
                    {
                        Console.Write("TCP���ӻص�����" + ex.Message);

                    }


                    //�µĿͻ�������,����֪ͨ 
                    if (ClientConn != null)
                    {
                        ClientConn(this, new NetEventArgs(newSession));
                    }
                }

                //�������ܿͻ��� 
                _svrSock.BeginAccept(new AsyncCallback(AcceptConn), _svrSock);
            }
        }

        /// <summary> 
        /// ͨ��Socket�������Session���� 
        /// </summary> 
        /// <param name="client"></param> 
        /// <returns>�ҵ���Session����,���Ϊnull,˵���������ڸûػ�</returns> 
        public Session FindSession(Socket client)
        {
            SessionId id = new SessionId((int)client.Handle);

            return (Session)_sessionTable[id];
        }
        /// <summary>
        ///   �����ļ� 7-11
        /// </summary>

        private void Receive(Socket socket)
        {
            NetworkStream ns = new NetworkStream(socket);
            FileStream fs = new FileStream("c:\\file.txt", FileMode.OpenOrCreate);
            bool isRead = true;
            while (isRead)
            {
                int count = ns.Read(this._recvDataBuffer, 0, this._recvDataBuffer.Length);
                int datanum = 0;
                datanum = BitConverter.ToInt32(this._recvDataBuffer, 0);  //��buffer�е�ǰ4���ֽڶ���count
                if (datanum > 0)                                      //ȷ��ÿ��Ҫ���ܶ����ֽ���
                {
                    fs.Write(this._recvDataBuffer, 4, datanum);
                }
                else                              //��������ֽ���Ϊ0 ���Ƴ�
                {
                    isRead = false;
                }
            }
            //this.txtFile.Text = "�ļ�����ɹ�";
            fs.Close();
        }

        /// <summary>
        /// �������
        /// </summary>
        // State object for reading client data asynchronously
        //         public class StateObject
        //         {
        //             // Client  socket.
        //             public Socket workSocket = null;
        //             // Size of receive buffer.
        //             public const int BufferSize = DefaultBufferSize;
        //             // Receive buffer.
        //             public byte[] buffer = new byte[BufferSize];
        //             // Received data string.
        //             public StringBuilder sb = new StringBuilder();
        //         }
        // 
        //         private static ManualResetEvent receiveDone =
        //      new ManualResetEvent(false);
        //         // The response from the remote device.
        //         private static String response = String.Empty;

        //         public virtual void ReceiveCallback(IAsyncResult ar)
        //         {
        //             try
        //             {
        //                 // Retrieve the state object and the client socket 
        //                 // from the asynchronous state object.
        //                 StateObject state = (StateObject)ar.AsyncState;
        //                 Socket client = state.workSocket;
        // 
        //                 // Read data from the remote device.
        //                 int bytesRead = client.EndReceive(ar);
        // 
        //                 if (bytesRead > 0)
        //                 {
        //                     // There might be more data, so store the data received so far.
        //                     state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
        // 
        //                     // Get the rest of the data.
        //                     client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
        //                         new AsyncCallback(ReceiveCallback), state);
        //                 }
        //                 else
        //                 {
        //                     // All the data has arrived; put it in response.
        //                     if (state.sb.Length > 1)
        //                     {
        //                         response = state.sb.ToString();
        //                     }
        //                     // Signal that all bytes have been received.
        //                     receiveDone.Set();
        //                 }
        //             }
        //             catch (Exception e)
        //             {
        //                 Console.WriteLine(e.ToString());
        //             }
        // 
        //             
        //         }

        /// <summary> 
        /// ����������ɴ��������첽�����Ծ���������������У� 
        /// �յ����ݺ󣬻��Զ�����Ϊ�ַ������� 
        /// </summary> 
        /// <param name="iar">Ŀ��ͻ���Socket</param> 
        Int64 TotalLng = 0;
        Int64 Toal = 0;
        string filename;
        FileStream fs;
        int TimeIndex = 0;
        protected void ReceiveData(IAsyncResult iar)
        {
            Socket client = (Socket)iar.AsyncState;
            try
            {
                int recv = client.EndReceive(iar);
                if (recv == 0)
                {
                    // CloseClient(client, Session.ExitType.NormalExit);

                    return;
                }

                string receivedData = String.Empty;


                if (_recvDataBuffer[0] == 0x5b)
                {
                    Console.Write(">");
                    // Console.Write(recv);
                    receivedData = System.Text.Encoding.UTF8.GetString(_recvDataBuffer, 0, recv - 1);
                    //  Array.Copy(_recvDataBuffer,0, rcbuffer, 0, recv - 1);
                    if (RecvData != null)
                    {
                        Session sendDataSession = FindSession(client);

                        Debug.Assert(sendDataSession != null);
                        ICloneable copySession = (ICloneable)sendDataSession;

                        Session clientSession = (Session)copySession.Clone();

                        clientSession.Datagram = receivedData;

                        // clientSession.RvBufer = _recvDataBuffer;
                        Array.Copy(_recvDataBuffer, clientSession.RvBufer, recv);//jxb

                        RecvData(this, new NetEventArgs(clientSession));
                    }
                }
                else
                {
                    Console.Write(recv);
                    Console.Write("#");
                    byte[] ab = new byte[16];
                    Array.Copy(_recvDataBuffer, 0, ab, 0, 16);
                    FileHeader fileinfo = new FileHeader();
                    TastInfo Info = new TastInfo();
                    object fileinfoType = fileinfo;

                    if (FileBegine)
                    {
                        filename = null;
                        _filePath = "D:\\MyVidio\\";
                        Info.ByteArrayToStructureEndian(ab, ref fileinfoType, 0);
                        fileinfo = (FileHeader)fileinfoType;
                        filename = System.Text.Encoding.UTF8.GetString(_recvDataBuffer, 16, (int)fileinfo.fileNameLng);
                        _filePath += System.Text.Encoding.UTF8.GetString(_recvDataBuffer, 16, (int)fileinfo.fileNameLng);
                        fs = new FileStream(_filePath, FileMode.Create);
                        //  fs.Close();
                        //  �����ݿ�
                        TotalLng = 0;
                        Toal = (int)(fileinfo.fileLng);
                        FileBegine = false;
                        string tim = System.DateTime.Now.ToString();
                        if (IsPic)
                        {
                            //�������ݿ�״̬

                            string sq = "INSERT INTO picinfo (Device_ID, Pic_Name, test_time) VALUES ('" + device_id + "','" + filename + "','" + tim + "')";
                            database.Open();
                            database.ExcuteNonQuery(sq);
                            database.Close();
                        }
                        else
                        {
                            //�������ݿ�״̬

                            string sq = "UPDATE detectioninfo SET video_path = '" + filename + "' WHERE Device_ID = '" + device_id + "'";
                            database.Open();
                            database.ExcuteNonQuery(sq);
                            database.Close();
                        }
                        TimeIndex = 0;
                    }
                    else
                    {
                        //���㵱ǰҪ��ȡ�Ŀ�Ĵ�С
                        int currentBlockLength = 0;
                        Console.Write("+");
                        Console.Write(TimeIndex++);
                        Console.Write("\nToal=" + Toal);
                        TotalLng += recv;
                        if (TotalLng < Toal)
                        {
                            FileBegine = false;
                            Console.Write("\nTotalLng= " + TotalLng);
                            fs.Write(_recvDataBuffer, 0, recv);
                            fs.Flush();
                        }
                        else if (TotalLng == Toal)
                        {
                            fs.Write(_recvDataBuffer, 0, recv);
                            Console.Write("ok");
                            FileBegine = false;
                            filename = null;
                            fs.Close();


                        }
                        else
                        {
                            Console.Write("error");
                            FileBegine = false;
                            filename = null;
                            fs.Close();
                        }


                    }
                }
                // Not all data received. Get more.
                client.BeginReceive(_recvDataBuffer, 0, _recvDataBuffer.Length, SocketFlags.None,
                  new AsyncCallback(ReceiveData), client);
            }
            catch (SocketException ex)
            {
                //�ͻ����˳� 
                if (10054 == ex.ErrorCode)
                {
                    //�ͻ���ǿ�ƹر� 

                    CloseClient(client, Session.ExitType.ExceptionExit);
                }

            }
            catch (ObjectDisposedException ex)
            {
                //�����ʵ�ֲ������� 
                //������CloseSession()ʱ,��������ݽ���,�������ݽ��� 
                //�����л����int recv = client.EndReceive(iar); 
                //�ͷ�����CloseSession()�Ѿ����õĶ��� 
                //����������ʵ�ַ���Ҳ�����˴��ŵ�. 
                if (ex != null)
                {
                    ex = null;
                    //DoNothing; 
                }
            }

        }


        /// <summary> 
        /// ����������ɴ����� 
        /// </summary> 
        /// <param name="iar">Ŀ��ͻ���Socket</param> 
        protected virtual void SendDataEnd(IAsyncResult iar)
        {
            Socket client = (Socket)iar.AsyncState;

            int sent = client.EndSend(iar);
        }


        #endregion
    }
}
