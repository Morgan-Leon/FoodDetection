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
        #region �����ֶ�

        /// <summary> 
        /// Ĭ�ϵķ�����������ӿͻ��˶����� 
        /// </summary> 
        public const int DefaultMaxClient = 100;

        /// <summary> 
        /// �������ݻ�������С64K 
        /// </summary> 
        public const int DefaultBufferSize = 40 * 1024 * 1024;

        /// <summary> 
        /// ������ݱ��Ĵ�С 
        /// </summary> 
        public const int MaxDatagramSize = 40 * 1024 * 1024;

        /// <summary> 
        /// ���Ľ����� 
        /// </summary> 
        private DatagramResolver _resolver;

        /// <summary> 
        /// ͨѶ��ʽ��������� 
        /// </summary> 
        private Coder _coder;

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
        private bool _isRun;

        /// <summary> 
        /// �������ݻ����� 
        /// </summary> 
        private byte[] _recvDataBuffer;

        /// <summary> 
        /// ������ʹ�õ��첽Socket��, 
        /// </summary> 
        private Socket _svrSock;

        /// <summary> 
        /// �������пͻ��˻Ự�Ĺ�ϣ�� 
        /// </summary> 
        private Hashtable _sessionTable;

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

            //��ʼ��socket 
            _svrSock = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            //�󶨶˿� 
            IPEndPoint iep = new IPEndPoint(_serverIP, _port);
            _svrSock.Bind(iep);

            //��ʼ���� 
            _svrSock.Listen(5);

            //�����첽�������ܿͻ������� 
            _svrSock.BeginAccept(new AsyncCallback(AcceptConn), _svrSock);

            _isRun = true;

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
            byte[] data = Encoding.UTF8.GetBytes(datagram);//��ȡ��Ϣ
            byte[] msg = new byte[18 + data.Length];
            msg[0] = 0x5b;//ͷ
            MSGHEAD head = new MSGHEAD();
            head.Msg_ID = MsgId;
            head.Msg_Length = (UInt32)msg.Length;

            head.Msg_GNSSCenter = UInt32.Parse(centercode); ;
            TastInfo info = new TastInfo();
            byte[] Head = info.StructureToByteArrayEndian(head); // ���ṹת�����ֽ�������

            data.CopyTo(msg, 15);
            Head.CopyTo(msg, 1);
            //����CRCУ��5
            byte[] crcbuf = new byte[msg.Length - 4];
            Array.Copy(crcbuf, 0, msg, 1, (uint)crcbuf.Length);


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


        /// <summary> 
        /// �ͻ������Ӵ����� 
        /// </summary> 
        /// <param name="iar">���������������ӵ�Socket����</param> 
        protected virtual void AcceptConn(IAsyncResult iar)
        {
            //���������ֹͣ�˷���,�Ͳ����ٽ����µĿͻ��� 
            if (!_isRun)
            {
                return;
            }

            //����һ���ͻ��˵��������� 
            Socket oldserver = (Socket)iar.AsyncState;

            Socket client = oldserver.EndAccept(iar);

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

                Session newSession = new Session(client);

                _sessionTable.Add(newSession.ID, newSession);

                //�ͻ������ü���+1 
                _clientCount++;

                //��ʼ�������Ըÿͻ��˵����� 
                client.BeginReceive(_recvDataBuffer, 0, _recvDataBuffer.Length, SocketFlags.None,
                    new AsyncCallback(ReceiveData), client);

                //�µĿͻ�������,����֪ͨ 
                if (ClientConn != null)
                {
                    ClientConn(this, new NetEventArgs(newSession));
                }
            }

            //�������ܿͻ��� 
            _svrSock.BeginAccept(new AsyncCallback(AcceptConn), _svrSock);
        }


        /// <summary> 
        /// ͨ��Socket�������Session���� 
        /// </summary> 
        /// <param name="client"></param> 
        /// <returns>�ҵ���Session����,���Ϊnull,˵���������ڸûػ�</returns> 
        private Session FindSession(Socket client)
        {
            SessionId id = new SessionId((int)client.Handle);

            return (Session)_sessionTable[id];
        }


        /// <summary> 
        /// ����������ɴ��������첽�����Ծ���������������У� 
        /// �յ����ݺ󣬻��Զ�����Ϊ�ַ������� 
        /// </summary> 
        /// <param name="iar">Ŀ��ͻ���Socket</param> 
        protected virtual void ReceiveData(IAsyncResult iar)
        {
            Socket client = (Socket)iar.AsyncState;

            try
            {
                //������ο�ʼ���첽�Ľ���,���Ե��ͻ����˳���ʱ�� 
                //������ִ��EndReceive 

                int recv = client.EndReceive(iar);

                if (recv == 0)
                {
                    //�����Ĺر� 
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
                //�����յ����ݵ��¼� 
                if (RecvData != null)
                {
                    Session sendDataSession = FindSession(client);

                    Debug.Assert(sendDataSession != null);

                    //��������˱��ĵ�β���,��Ҫ�����ĵĶ������ 
                    if (_resolver != null)
                    {
                        if (sendDataSession.Datagram != null &&
                            sendDataSession.Datagram.Length != 0)
                        {
                            //�������һ��ͨѶʣ��ı���Ƭ�� 
                            receivedData = sendDataSession.Datagram + receivedData;
                            rcbuffer.CopyTo(sendDataSession.RvBufer, rcbuffer.Length);//jxb
                            // RecvData(this, new NetEventArgs(sendDataSession));//jxb
                        }

                        string[] recvDatagrams = _resolver.Resolve(ref receivedData);


                        foreach (string newDatagram in recvDatagrams)
                        {
                            //���,Ϊ�˱���Datagram�Ķ����� 
                            ICloneable copySession = (ICloneable)sendDataSession;

                            Session clientSession = (Session)copySession.Clone();

                            clientSession.Datagram = newDatagram;
                            //
                            Array.Copy(rcbuffer, clientSession.RvBufer, rcbuffer.Length);//jxb
                            //����һ��������Ϣ 
                            RecvData(this, new NetEventArgs(clientSession));
                        }

                        //ʣ��Ĵ���Ƭ��,�´ν��յ�ʱ��ʹ�� 
                        sendDataSession.Datagram = receivedData;
                        Array.Copy(rcbuffer, sendDataSession.RvBufer, rcbuffer.Length);//jxb
                        if (sendDataSession.Datagram.Length > MaxDatagramSize)
                        {
                            sendDataSession.Datagram = null;
                            sendDataSession.RvBufer = null;//jxb
                        }

                    }
                    //û�ж��屨�ĵ�β���,ֱ�ӽ�����Ϣ������ʹ�� 
                    else
                    {
                        ICloneable copySession = (ICloneable)sendDataSession;

                        Session clientSession = (Session)copySession.Clone();

                        clientSession.Datagram = receivedData;
                        Array.Copy(rcbuffer, clientSession.RvBufer, rcbuffer.Length);//jxb
                        RecvData(this, new NetEventArgs(clientSession));
                    }

                }//end of if(RecvData!=null) 

                //���������������ͻ��˵����� 
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
