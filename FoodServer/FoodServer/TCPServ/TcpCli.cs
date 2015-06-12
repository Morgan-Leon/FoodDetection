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
using System.Windows.Forms;
using System.Threading;

namespace FlyTcpFramework
{
    /// <summary> 
    /// �ṩTcp�������ӷ���Ŀͻ����� 
    /// 
    /// ԭ��: 
    /// 1.ʹ���첽SocketͨѶ�����������һ����ͨѶ��ʽͨѶ,��ע�����������ͨ 
    /// Ѷ��ʽһ��Ҫһ��,���������ɷ������������,��������û�п˷�,��ô��byte[] 
    /// �ж����ı����ʽ 
    /// 2.֧�ִ���ǵ����ݱ��ĸ�ʽ��ʶ��,����ɴ����ݱ��ĵĴ������Ӧ���ӵ��� 
    /// �绷��. 
    /// </summary> 
    public class TcpCli
    {
        #region �ֶ�

        /// <summary> 
        /// �ͻ����������֮��ĻỰ�� 
        /// </summary> 
        private Session _session;

        /// <summary> 
        /// �ͻ����Ƿ��Ѿ����ӷ����� 
        /// </summary> 
        private bool _isConnected = false;

        /// <summary> 
        /// �������ݻ�������С64K 
        /// </summary> 
        public const int DefaultBufferSize = 4 * 1024*1024;

        /// <summary> 
        /// ���Ľ����� 
        /// </summary> 
        private DatagramResolver _resolver;

        /// <summary> 
        /// ͨѶ��ʽ��������� 
        /// </summary> 
        private Coder _coder;

        /// <summary> 
        /// �������ݻ����� 
        /// </summary> 
        private byte[] _recvDataBuffer = new byte[DefaultBufferSize];

		/// <summary>
		/// �ͻ����ļ�����·��
		/// </summary>
		private string _filePath;

        #endregion

        #region �¼�����

        //��Ҫ�����¼������յ��¼���֪ͨ������������˳�������ȡ������ 

        /// <summary> 
        /// �Ѿ����ӷ������¼� 
        /// </summary> 
        public event NetEvent ConnectedServer;

        /// <summary> 
        /// ���յ����ݱ����¼� 
        /// </summary> 
        public event NetEvent ReceivedDatagram;

        /// <summary> 
        /// ���ӶϿ��¼� 
        /// </summary> 
        public event NetEvent DisConnectedServer;
        #endregion

        #region ����

        /// <summary> 
        /// ���ؿͻ����������֮��ĻỰ���� 
        /// </summary> 
        public Session ClientSession
        {
            get
            {
                return _session;
            }
        }

        /// <summary> 
        /// ���ؿͻ����������֮�������״̬ 
        /// </summary> 
        public bool IsConnected
        {
            get
            {
                return _isConnected;
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
        /// ��������� 
        /// </summary> 
        public Coder ServerCoder
        {
            get
            {
                return _coder;
            }
        }
		/// <summary> 
		/// �ͻ����ļ�����·�� 
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
        /// Ĭ�Ϲ��캯��,ʹ��Ĭ�ϵı����ʽ 
        /// </summary> 
        public TcpCli(string saveFilePath)
        {
            _coder = new Coder(Coder.EncodingMothord.Default);
			_filePath=saveFilePath;
        }

        /// <summary> 
        /// ���캯��,ʹ��һ���ض��ı���������ʼ�� 
        /// </summary> 
        /// <param name="_coder">���ı�����</param> 
        public TcpCli(Coder coder,string saveFilePath)
        {
            _coder = coder;
			_filePath=saveFilePath;
        }

        /// <summary> 
        /// ���ӷ����� 
        /// </summary> 
        /// <param name="ip">������IP��ַ</param> 
        /// <param name="port">�������˿�</param> 
        public virtual void Connect(string ip, int port)
        {
            if (IsConnected)
            {
                //�������� 
                Debug.Assert(_session != null);

                Close();
            }
            try
            {
                Socket newsock = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

                IPEndPoint iep = new IPEndPoint(IPAddress.Parse(ip), port);
                newsock.BeginConnect(iep, new AsyncCallback(Connected), newsock);
               // Thread.Sleep(5000);
                

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("�������޷�����");
            }
            
        }
        //������Ϣ

        public virtual void SendMessage( string datagram, UInt16 MsgId, string centercode)
        {
            //|5b|4-4-2-4|..........|2|5d| == 18
            //

            byte[] data = System.Text.Encoding.UTF8.GetBytes(datagram);//��ȡ��Ϣ
            Array.Reverse(data);
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
            //����CRCУ��
            msg[msg.Length - 1] = 0x5d;//β
            _session.ClientSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None,
               new AsyncCallback(SendDataEnd), _session.ClientSocket);
        }


        //������Ϣ

        /// <summary>
        ///   ������Ϣ  
        ///   2013-05-19 jxb
        ///   ���������壬�������֡
        /// </summary>
        /// <summary> 
        /// 
        public virtual void SendMessage( byte[] datagram, UInt16 MsgId, string centercode)
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
            head.Msg_GNSSCenter = UInt32.Parse(centercode);
            //ͷ����
            byte[] Head = info.StructureToByteArrayEndian(head);
 
            datagram.CopyTo(msg, 15);
            Head.CopyTo(msg, 1);
            //����CRCУ��5


            msg[msg.Length - 1] = 0x5d;//β
            _session.ClientSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None,
               new AsyncCallback(SendDataEnd), _session.ClientSocket);
        }
        /// <summary> 
        /// �������ݱ��� 
        /// </summary> 
        /// <param name="datagram"></param> 
        public virtual void SendText(string datagram)
        {
            if (datagram.Length == 0)
            {
                return;
            }

            if (!_isConnected)
            {
                throw (new ApplicationException("û�����ӷ����������ܷ�������"));
            }

            //��ñ��ĵı����ֽ� 
            byte[] data = _coder.GetTextBytes(datagram);

            _session.ClientSocket.BeginSend(data, 0, data.Length, SocketFlags.None,
                new AsyncCallback(SendDataEnd), _session.ClientSocket);
        }
        public virtual void SendFile(string FilePath)
        {
            if (FilePath.Length == 0)
            {
                return;
            }

            if (!_isConnected)
            {
                throw (new ApplicationException("û�����ӷ����������ܷ�������"));
            }

            if (File.Exists(FilePath))
            {
                byte[] data = _coder.GetFileBytes(FilePath);

                _session.ClientSocket.BeginSend(data, 0, data.Length, SocketFlags.None,
                    new AsyncCallback(SendDataEnd), _session.ClientSocket);
            }
            else
            {
                throw new Exception("�ļ�������");
            }
        }

        /// <summary> 
        /// �ر����� 
        /// </summary> 
        public virtual void Close()
        {
            if (!_isConnected)
            {
                return;
            }

            _session.Close();

            _session = null;

            _isConnected = false;
        }

        #endregion

        #region �ܱ�������

        /// <summary> 
        /// ���ݷ�����ɴ����� 
        /// </summary> 
        /// <param name="iar"></param> 
        protected virtual void SendDataEnd(IAsyncResult iar)
        {
            Socket remote = (Socket)iar.AsyncState;
            int sent = remote.EndSend(iar);
            Debug.Assert(sent != 0);

        }

        /// <summary> 
        /// ����Tcp���Ӻ������ 
        /// </summary> 
        /// <param name="iar">�첽Socket</param> 
        protected virtual void Connected(IAsyncResult iar)
        {
            
            Socket socket = (Socket)iar.AsyncState;
           if (!socket.Connected)
           {
               MessageBox.Show("����ʧ�ܣ�");
               _isConnected = false;
               return;
           }
            socket.EndConnect(iar);

            //�����µĻỰ 
            _session = new Session(socket);

            _isConnected = true;

            //�������ӽ����¼� 
            if (ConnectedServer != null)
            {
                ConnectedServer(this, new NetEventArgs(_session));
            }
            else
            {
                MessageBox.Show("����ʧ�ܣ�");
            }
            //�������Ӻ�Ӧ�������������� 
            _session.ClientSocket.BeginReceive(_recvDataBuffer, 0,
                DefaultBufferSize, SocketFlags.None,
                new AsyncCallback(RecvData), socket);
        }

        /// <summary> 
        /// ���ݽ��մ����� 
        /// </summary> 
        /// <param name="iar">�첽Socket</param> 
        protected virtual void RecvData(IAsyncResult iar)
        {
            Socket remote = (Socket)iar.AsyncState;

            try
            {
                int recv = remote.EndReceive(iar);

                //�������˳� 
                if (recv == 0)
                {
                    _session.TypeOfExit = Session.ExitType.NormalExit;

                    if (DisConnectedServer != null)
                    {
                        DisConnectedServer(this, new NetEventArgs(_session));
                    }

                    return;
                }
                string receivedData;
                if (_recvDataBuffer[0] == 0x5b)
                {
                    receivedData = _coder.GetEncodingString(_recvDataBuffer,1, recv-1);
                }
                else
                {
					string fileName = this._coder.GetEncodingString(_recvDataBuffer,6,_recvDataBuffer[1]-1);
					_coder.SaveFile(_filePath + fileName, _recvDataBuffer);
					receivedData = "Receive File:" + fileName + "]";
                }

                //ͨ���¼������յ��ı��� 
                if (ReceivedDatagram != null)
                {
                    //ͨ�����Ľ��������������� 
                    //��������˱��ĵ�β���,��Ҫ�����ĵĶ������ 
                    if (_resolver != null)
                    {
                        if (_session.Datagram != null &&
                            _session.Datagram.Length != 0)
                        {
                            //�������һ��ͨѶʣ��ı���Ƭ�� 
                            receivedData = _session.Datagram + receivedData;
                        }

                        string[] recvDatagrams = _resolver.Resolve(ref receivedData);


                        foreach (string newDatagram in recvDatagrams)
                        {
                            //Need Deep Copy.��Ϊ��Ҫ��֤�����ͬ���Ķ������� 
                            ICloneable copySession = (ICloneable)_session;

                            Session clientSession = (Session)copySession.Clone();

                            clientSession.Datagram = newDatagram;

                            //����һ��������Ϣ 
                            ReceivedDatagram(this, new NetEventArgs(clientSession));
                        }

                        //ʣ��Ĵ���Ƭ��,�´ν��յ�ʱ��ʹ�� 
                        _session.Datagram = receivedData;
                    }
                    //û�ж��屨�ĵ�β���,ֱ�ӽ�����Ϣ������ʹ�� 
                    else
                    {
                        ICloneable copySession = (ICloneable)_session;

                        Session clientSession = (Session)copySession.Clone();

                        clientSession.Datagram = receivedData;

                        ReceivedDatagram(this, new NetEventArgs(clientSession));

                    }


                }//end of if(ReceivedDatagram != null) 

                //������������ 
                _session.ClientSocket.BeginReceive(_recvDataBuffer, 0, DefaultBufferSize, SocketFlags.None,
                    new AsyncCallback(RecvData), _session.ClientSocket);
            }
            catch (SocketException ex)
            {
                //�ͻ����˳� 
                if (10054 == ex.ErrorCode)
                {
                    //������ǿ�ƵĹر����ӣ�ǿ���˳� 
                    _session.TypeOfExit = Session.ExitType.ExceptionExit;

                    if (DisConnectedServer != null)
                    {
                        DisConnectedServer(this, new NetEventArgs(_session));
                    }
                }
                else
                {
                    throw (ex);
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

        #endregion


    } 
}
