using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using FlyTcpFramework;
using FoodServer.Class;
using GHCS.DataBase;
using MySql.Data.MySqlClient;
using GHCS;
using System.Data;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
namespace PPServer
{
    /// <summary>
    /// 测试TcpSvr的类
    /// </summary>
    public class TestTcpSvr
    {
        static public int MsgHeadLength = 14;
        static public int Msgoffset = 15;
        static public int MsgTotalLen = 18;
        static public bool Is_register = false;

        public string path = @"D:\MyVidio";//目录文件夹
        //用于维护用户注册
        UInt32 deviceid;
        //
        MySqlDataBase dbMySql = MySqlDataBase.getInstance();
        string databasename;
        IDataBase database = MySqlDataBase.getInstance();
        MySqlDataAdapter myDataAdpter = new MySqlDataAdapter();
        DataSet myDataSet = new DataSet();


        // public TcpSvr svr;
        public TestTcpSvr()
        {
            databasename = dbMySql.GetDatabaseName();
            CreateConnection();
            OpenDataBase();
        }
        private int CreateConnection()
        {
            return database.CreateConnection(null);
        }

        private void OpenDataBase()
        {
            database.Open();
        }
        //“[”(0x5B)或“]”(0x5D)，
        public static string GetHostName()
        {
            return Dns.GetHostName();
        }

        public static IPAddress[] GetLocalIP()
        {
            string name = Dns.GetHostName();
            IPHostEntry me = Dns.GetHostEntry(name);
            return me.AddressList;
        }
        public IPAddress GetFirstIP()
        {
            IPAddress[] ips = GetLocalIP();
            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    continue;
                return ip;
            }
            return ips != null && ips.Length > 0 ? ips[0] : new IPAddress(0x0);
        }
        public static string getLocalIP()
        {
            string strHostName = Dns.GetHostName(); //得到本机的主机名
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName); //取得本机IP
            string strAddr = ipEntry.AddressList[1].ToString();
            return (strAddr);
        }
     

        private static void TimeEvent(object source, ElapsedEventArgs e)
        {

        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitSys()
        {
            string sql = "UPDATE device SET Device_Status = 0 ";
            database.Open();
            database.ExcuteNonQuery(sql);
            database.Close();
        }
        [STAThread]
        static void Main()
        {
            while (true)
            {
                try
                {

                    Console.WriteLine("Begin to Start TcpSvr SVN...");

                    TestTcpSvr tts = new TestTcpSvr();
                    tts.InitSys();
                    #region 定时器事件
                    System.Timers.Timer aTimer = new System.Timers.Timer();
                    aTimer.Elapsed += new ElapsedEventHandler(TimeEvent);
                    aTimer.Interval = 5 * 1000;    //配置文件中配置的秒数
                    aTimer.Enabled = true;
                    #endregion
                    string ipaddr = getLocalIP();
                    Console.WriteLine(ipaddr);
                    ushort portNumber = 5632;
                    IniAc ini = new IniAc();
                    ipaddr = ini.ReadValue("NET", "ip");
                    Console.WriteLine(ipaddr);
                    string portStr = ini.ReadValue("NET", "port");
                    portNumber = System.Convert.ToUInt16(portStr);

                    //TcpSvr svr = new TcpSvr(9050,4);//默认使用Encoding.Default编码方式
                    TcpSvr svr = new TcpSvr(IPAddress.Parse(ipaddr), 5632, 80, new Coder(Coder.EncodingMothord.Unicode), "D:\\MyVidio");

                    svr.Resovlver = new DatagramResolver("]");//0x5d

                    //定义服务器的4个事件

                    //服务器满
                    svr.ServerFull += new NetEvent(tts.ServerFull);

                    //新客户端连接
                    svr.ClientConn += new NetEvent(tts.ClientConn);

                    //客户端关闭
                    svr.ClientClose += new NetEvent(tts.ClientClose);

                    //接收到数据
                    svr.RecvData += new NetEvent(tts.RecvData);

                    //  svr.LogIn += new NetEvent(this.LogIn);

                    svr.Start();

                    Console.WriteLine("Server is listen...{0}",
                        svr.ServerSocket.LocalEndPoint.ToString());
                    //命令控制循环
                    #region

                    while (true)
                    {
                        Console.Write(">");

                        string cmd = Console.ReadLine();

                        //退出测试程序
                        if (cmd.ToLower() == "exit")
                        {
                            break;
                        }

                        //停止服务器程序
                        if (cmd.ToLower() == "stop")
                        {
                            svr.Stop();

                            Console.WriteLine("Server is Stop.");

                            continue;
                        }

                        //运行服务器程序
                        if (cmd.ToLower() == "start")
                        {
                            svr.Start();

                            Console.WriteLine("Server is listen...{0}",
                                svr.ServerSocket.LocalEndPoint.ToString());

                            continue;
                        }

                        //察看服务器在线客户端数目和容量
                        if (cmd.ToLower() == "count")
                        {
                            Console.WriteLine("Current count of Client is {0}/{1}",
                                svr.SessionCount, svr.Capacity);
                            continue;
                        }

                        //发送数据到客户端格式:send [Session] [stringData]
                        if (cmd.ToLower().IndexOf("send") != -1)
                        {
                            cmd = cmd.ToLower();

                            string[] para = cmd.Split(' ');

                            if (para.Length == 3)
                            {

                                Session client = (Session)svr.SessionTable[new SessionId(int.Parse
                                    (para[1]))];

                                if (client != null)
                                {
                                    svr.SendText(client, para[2]);
                                }
                                else
                                {
                                    Console.WriteLine("The Session is Null");
                                }

                            }
                            else
                            {
                                Console.WriteLine("Error Command");
                            }

                            continue;
                        }

                        //从服务器上踢掉一个客户端
                        if (cmd.ToLower().IndexOf("kick") != -1)
                        {
                            cmd = cmd.ToLower();

                            string[] para = cmd.Split(' ');

                            if (para.Length == 2)
                            {
                                Session client = (Session)svr.SessionTable[new SessionId(int.Parse
                                    (para[1]))];

                                if (client != null)
                                {
                                    svr.CloseSession(client);
                                }
                                else
                                {
                                    Console.WriteLine("The Session is Null");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error command");
                            }

                            continue;

                        }

                        //列出服务器上所有的客户端信息
                        if (cmd.ToLower() == "list")
                        {
                            int i = 0;

                            foreach (Session Client in svr.SessionTable.Values)
                            {
                                if (Client != null)
                                {
                                    i++;
                                    string info = string.Format("{0} Client:{1} connected server Session:{2}. Socket Handle:{3}",
                                        i,
                                        Client.ClientSocket.RemoteEndPoint.ToString(),
                                        Client.ID,
                                        Client.ClientSocket.Handle);

                                    Console.WriteLine(info);
                                }
                                else
                                {
                                    i++;

                                    string info = string.Format("{0} null Client", i);
                                    Console.WriteLine(info);

                                }
                            }

                            continue;

                        }

                        Console.WriteLine("Unkown Command");


                    }//end of while
                    #endregion
                    Console.WriteLine("End service");
                }
                catch (Exception ex)
                {
                    string sql = "UPDATE device SET Device_Status = 0,Sockets = ''";
                    IDataBase database = MySqlDataBase.getInstance();
                    database.Open();
                    database.ExcuteNonQuery(sql);
                    database.Close();

                    Console.WriteLine(ex.ToString());

                }
            }

        }

        void ClientConn(object sender, NetEventArgs e)
        {
            string info = string.Format("A Client:{0} connect server Session:{1}. Socket Handle:{2}",
                e.Client.ClientSocket.RemoteEndPoint.ToString(),
                e.Client.ID, e.Client.ClientSocket.Handle);

            Console.WriteLine(info);

            Console.Write(">");
        }

        void ServerFull(object sender, NetEventArgs e)
        {
            string info = string.Format("Server is full.the Client:{0} is refused",
                e.Client.ClientSocket.RemoteEndPoint.ToString());

            //Must do it
            //服务器满了,必须关闭新来的客户端连接
            e.Client.Close();
         
            Console.WriteLine(info);

            Console.Write(">");

        }

        void ClientClose(object sender, NetEventArgs e)
        {
            string info;

            if (e.Client.TypeOfExit == Session.ExitType.ExceptionExit)
            {
                info = string.Format("A Client Session:{0} Exception Closed.",
                    e.Client.ID);
            }
            else
            {
                info = string.Format("A Client Session:{0} Normal Closed.",
                    e.Client.ID);
            }
            //更新数据库状态
            string sql = "UPDATE device SET Device_Status = 0 WHERE Device_ID = '" + e.Client.DeviceId + "'";
            database.Open();
            database.ExcuteNonQuery(sql);
            database.Close();
            Console.WriteLine(info);

            Console.Write(">");
        }
        /// <summary>
        ///   获取文件头
        /// </summary>
        static public MSGHEAD Get_MSGHeader(string str)
        {

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
            TastInfo Info = new TastInfo();
            MSGHEAD head = new MSGHEAD();
            object headType = head;
            Info.ByteArrayToStructureEndian(buffer, ref headType, 0);
            head = (MSGHEAD)headType;


            return head;
        }
        /// <summary>
        ///   获取文件头
        /// </summary>
        static public MSGHEAD Get_MSGHeader(byte[] buffer)
        {

            //  byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
            TastInfo Info = new TastInfo();
            MSGHEAD head = new MSGHEAD();
            object headType = head;
            Info.ByteArrayToStructureEndian(buffer, ref headType, 0);
            head = (MSGHEAD)headType;


            return head;
        }

        /// <summary>
        ///   载入检测结果
        /// </summary>
        private System.Data.DataTable LoadDetectionInfo(string deviceid)
        {
            DataSet dataset = new DataSet();
            dataset.Clear();
            string sql = "SELECT * FROM detectioninfo " +
                         " WHERE Device_ID = '" + deviceid + "'";
            int ret = database.ReadDataBase(sql, "deinfo", dataset);
            if (ret == 0)
            {
                System.Data.DataTable table = new System.Data.DataTable();
                table = dataset.Tables["deinfo"];
                return table;
            }
            return null;
        }
        /// <summary>
        ///   载入设备表
        /// </summary>
        private System.Data.DataTable LoadDevice()
        {
            DataSet dataset = new DataSet();
            dataset.Clear();
            string sql = "SELECT * FROM device";
            int ret = database.ReadDataBase(sql, "device", dataset);
            if (ret == 0)
            {
                System.Data.DataTable table = new System.Data.DataTable();
                table = dataset.Tables["device"];
                return table;
            }
            return null;
        }

        /// <summary>
        ///   
        /// </summary>
        public void LogIn(object sender, NetEventArgs e)
        {
            Is_register = true;
        }
        static public int GetLen(byte[] buf)
        {
            int len = Encoding.Default.GetString(buf).Length;

            for (int i = len - 1; i >= 0; i--)
            {
                if (buf[i] != '\0')
                {
                    len = i;
                    break;
                }
            }

            return len + 1;
        }
        public struct REGISTER
        {
            public UInt32 nameId;  //用户名
            public UInt32 centercode;
            public byte result;

        }


        #region 结果上传
        public bool UpLoad(byte[] buffer, string deviceid)
        {
            DETECTION_INFO detInfo = new DETECTION_INFO();//共计866字节
            object detinfoType = detInfo;
            TastInfo Tinfo = new TastInfo();
            Tinfo.ByteArrayToStructureEndian(buffer, ref detinfoType, 0);
            detInfo = (DETECTION_INFO)detinfoType;


            //写入数据库
            string batch_id = System.Text.Encoding.UTF8.GetString(detInfo.batch_id, 0, GetLen(detInfo.batch_id)).Trim();
            string task_id = System.Text.Encoding.UTF8.GetString(detInfo.task_id, 0, GetLen(detInfo.task_id)).Trim();
            //             UInt64 batch_id = detInfo.batch_id;
            //             UInt32 task_id = detInfo.task_id;
            string p_name = System.Text.Encoding.UTF8.GetString(detInfo.p_name, 0, GetLen(detInfo.p_name)).Trim();
            string type = System.Text.Encoding.UTF8.GetString(detInfo.type, 0, GetLen(detInfo.type));
            int channel = (int)detInfo.channel;
            string ftest_result = System.Text.Encoding.UTF8.GetString(detInfo.ftest_result, 0, GetLen(detInfo.ftest_result)).Trim();
            string test_result = System.Text.Encoding.UTF8.GetString(detInfo.test_result, 0, GetLen(detInfo.test_result)).Trim();
            string result_unit = System.Text.Encoding.UTF8.GetString(detInfo.result_unit, 0, GetLen(detInfo.result_unit)).Trim();
            string standard = System.Text.Encoding.UTF8.GetString(detInfo.standard, 0, GetLen(detInfo.standard)).Trim();
            string abs_result = System.Text.Encoding.UTF8.GetString(detInfo.abs_result, 0, GetLen(detInfo.abs_result)).Trim();
            string sample_no = System.Text.Encoding.UTF8.GetString(detInfo.sample_no, 0, GetLen(detInfo.sample_no)).Trim();
            string sample_name = System.Text.Encoding.UTF8.GetString(detInfo.sample_name, 0, GetLen(detInfo.sample_name)).Trim();
            string sites = System.Text.Encoding.UTF8.GetString(detInfo.sites, 0, GetLen(detInfo.sites)).Trim();
            string submission_unit = System.Text.Encoding.UTF8.GetString(detInfo.submission_unit, 0, GetLen(detInfo.submission_unit)).Trim();
            string test_operator = System.Text.Encoding.UTF8.GetString(detInfo.test_operator, 0, GetLen(detInfo.test_operator)).Trim();
            string test_unit = System.Text.Encoding.UTF8.GetString(detInfo.test_unit, 0, GetLen(detInfo.test_unit)).Trim();
            string test_time = System.Text.Encoding.UTF8.GetString(detInfo.test_time, 0, GetLen(detInfo.test_time)).Trim();
            // DateTime dt = DateTime.ParseExact(test_time, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            int User_Id = 1234;
            if (deviceid != null)
            {
                User_Id = int.Parse(deviceid);
            }

            string strsql = "Insert Into " +
            "detectioninfo(batch_id,task_id,p_name,type,channel,Juge_result,Detect_result,result_unit,standard,Absolut_result,sample_no,sample_name,sites,submission_unit,test_operator,test_unit,test_time,Device_ID)"
            + "values('" + batch_id + "','" + task_id + "','" + p_name + "','" + type + "'," + channel + ",'" + ftest_result + "','" + test_result + "','" + result_unit + "','" + standard + "','" + abs_result + "','" + sample_no + "','" + sample_name + "','" + sites + "','" + submission_unit + "','" + test_operator + "','" + test_unit + "','" + test_time + "'," + User_Id + ")";
            try
            {
                dbMySql.Open(databasename);
                dbMySql.ExcuteNonQuery(databasename, strsql);
                dbMySql.Close(databasename);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }

        }
        #endregion
        #region 注册判定
        /// <summary>
        ///   是否注册
        /// </summary>
        public REGISTER IsRegest(MSGHEAD head, byte[] buf)
        {
            REGISTER regiester = new REGISTER();
            TastInfo Info = new TastInfo();

            Msg_LoadAsk user23 = new Msg_LoadAsk();
            object user23Type = user23;
            Info.ByteArrayToStructureEndian(buf, ref user23Type, 0);
            user23 = (Msg_LoadAsk)user23Type;
            UInt32 nameId = user23.nameId;
            //此处有小问题，如何从八位bytes完整提取为string类型。2013-5-29
            string password = System.Text.Encoding.UTF8.GetString(user23.password, 0, GetLen(user23.password)).Trim();//
            UInt32 centercode = head.Msg_GNSSCenter;
            DataTable table = LoadDevice();
            DataRow[] dr = table.Select("Device_ID =" + nameId);
            //查询用户
            if (dr.Length == 1)
            {
                if (dr[0]["Device_PassWord"].ToString().Equals(password))
                {
                    if (dr[0]["Device_Code"].ToString().Equals(head.Msg_GNSSCenter.ToString()))
                    {
                        regiester.nameId = nameId;
                        regiester.centercode = head.Msg_GNSSCenter;
                        regiester.result = 0x00;
                        //验证通过 ，发送消息

                    }
                    else
                    {
                        //接入码不对
                        regiester.centercode = centercode;
                        regiester.result = 0x01;
                    }
                }
                else
                {
                    //密码不对
                    regiester.centercode = centercode;
                    regiester.result = 0x03;
                }
            }
            else
            {
                //没有注册
                regiester.centercode = centercode;
                regiester.result = 0x02;
            }
            return regiester;
        }
        #endregion

        #region 创建文件夹



        public string CreateFilePath(string deviceid)
        {
            string filename = null;
            try
            {
                // Determine whether the directory exists.

                filename = path + "\\" + deviceid + "\\" + DateTime.Now.ToString("yyyy-MM-dd");
                if (!Directory.Exists(filename))
                {
                    // Create the directory it does not exist.
                    Directory.CreateDirectory(filename);
                }

                return filename;

            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            return null;
        }
        #endregion
        void RecvData(object sender, NetEventArgs e)
        {
            //  string info = string.Format("recv data:{0} from:{1}.", e.Client.Datagram, e.Client);
            //  Console.WriteLine(info);
            TcpSvr svr = (TcpSvr)sender;
            
            //测试把收到的数据返回给客户端
            // svr.SendText(e.Client, e.Client.Datagram);
            //svr.SendFile(this.textBox1.Text);
         //   Console.Write(">");

            DETECTION_INFO ff = new DETECTION_INFO();
            int si = Marshal.SizeOf(ff);

            //获取接受到的消息,已经去头，但是包括尾
            // byte[] recivebuffer = System.Text.Encoding.Default.GetBytes(e.Client.Datagram);
            List<Byte> lbyte = new List<Byte>();

            byte[] recivebuffer = e.Client.RvBufer;

            for (int k = 0; k < e.Client.RvBufer.Length - 1; k++)
            {
                byte b = e.Client.RvBufer[k];
                byte c = e.Client.RvBufer[k + 1];
                if (b == 0x5a && c == 0x01)
                {
                    lbyte.Add(0x5b);
                }
                else if (b == 0x5a && c == 0x02)
                {
                    lbyte.Add(0x5a);
                }
                else if (b == 0x5e && c == 0x01)
                {
                    lbyte.Add(0x5d);
                }
                else if (b == 0x5e && c == 0x02)
                {
                    lbyte.Add(0x5e);
                }
                else
                {
                    lbyte.Add(b);
                }
            }
            for (int i = 0; i < lbyte.Count; i++)
            {
                recivebuffer[i] = lbyte[i];
            }

            string batch_id = null;
            string task_id = null;
            //解析消息头
            MSGHEAD head = new MSGHEAD();
            byte[] headbuffer = new byte[MsgHeadLength];
            Array.Copy(recivebuffer, 1, headbuffer, 0, MsgHeadLength);
            head = Get_MSGHeader(headbuffer);
            // head = Get_MSGHeader(e.Client.Datagram.Substring(0, MsgHeadLength));
            //消息体数据

            int Meg_len = (int)head.Msg_Length - MsgTotalLen;
            byte[] buffer = new byte[Meg_len];//
            Array.Copy(recivebuffer, 15, buffer, 0, Meg_len);

//             for (int i = 0; i < head.Msg_Length; ++i)
//             {
//                 byte t = recivebuffer[i];
//                 Console.Write("{0:X}", t);//这个就是主要，填充
//                 Console.Write(" ");
// 
//             }
           // Console.Write("?");
            //接入码s
            UInt32 centercode = head.Msg_GNSSCenter;
            TastInfo Tinfo = new TastInfo();
            byte[] MesBody = null;
            //空数据
            byte[] nu = new byte[0];
            //判定是不是注册
            REGISTER Is_re = new REGISTER();
            //根据不同消息进行处理
            
            Session sendDataSession = svr.FindSession(e.Client.ClientSocket);
            ICloneable copySession = (ICloneable)sendDataSession;
            Session clientSession = (Session)copySession.Clone();
        //    Console.Write("*"+head.Msg_ID);
            switch (head.Msg_ID)
            {
                 
                //中心站只发过一个空包，或者包括简单的配置要求，服务器端组帧。
                //链路登陆请求消息
                case 0x0001:
                    Is_re = IsRegest(head, buffer);
                    // object structType = Is_re;
                    byte[] btArray = new byte[1];
                    btArray[0] = Is_re.result;
                    //MesBody = BitConverter.GetBytes(Is_re.result);
                    svr.SendMessage(e.Client, btArray, 0x0002, centercode);//登录应答
                    if (Is_re.result == 0)
                    {
                        foreach (DictionaryEntry de in svr._sessionTable) //ht为一个Hashtable实例
                        {
                            if (de.Value.Equals(clientSession))
                            {
                                clientSession.DeviceId = Is_re.nameId;
                                SessionId id = clientSession.ID;
                                svr._sessionTable.Remove(clientSession.ID);
                                svr._sessionTable.Add(id, clientSession);
                                break;
                            }
                        }

                      
                        //更新数据库状态
                        string sql = "UPDATE device SET Device_Status = 1,Sockets = '" + clientSession.ClientSocket.RemoteEndPoint + "' WHERE Device_ID = '" + clientSession.DeviceId + "'";
                        database.Open();
                        database.ExcuteNonQuery(sql);
                        database.Close();

                    }
                    else
                    {
                         //Session sendDataSession = svr.FindSession(e.Client.ClientSocket);
                        //更新数据库状态
                        string sql = "UPDATE device SET Device_Status = 0 WHERE Device_ID = '" + clientSession.DeviceId + "'";
                        database.Open();
                        database.ExcuteNonQuery(sql);
                        database.Close();
                    }
                    break;
                //用于心跳监测
                case 0x0003:
                    
                    svr.SendMessage(clientSession, nu, 0x0004, centercode);//开启视频

                   // Console.Write("|" + buffer[1]);
                    break;

                //批量任务下发（中心站往终端）
                //TEST NUM	4	Unint32_t	结果个数
                //TEST INFO	804	Octet_string	检测结果信息
                case 0x0005:
                    //获取任务个数
                    byte[] numbuffer = new byte[4];//
                    Array.Copy(recivebuffer, 14, numbuffer, 0, 4);//获取个数
                    //                     Array.Reverse(numbuffer);
                    //                     UInt32 num = BitConverter.ToUInt32(numbuffer,0);
                    //组帧
                    TASK_INFO taskinfo = new TASK_INFO();
                    object structT = taskinfo;
                    MesBody = Tinfo.StructureToByteArrayEndian(structT);
                    foreach (Session cliSession in svr._sessionTable.Values)
                    {
                        if (cliSession.DeviceId == deviceid)
                        {
                            svr.SendMessage(cliSession, nu, 0x0005, centercode);//开启视频
                            break;
                        }
                    }
                    
                    //发送

                    break;
                //检测结果上传消息

                //检测结果上传（终端往中心站）
                case 0x0006:
                    sendDataSession = svr.FindSession(e.Client.ClientSocket);

                    deviceid = sendDataSession.DeviceId;
                    UpLoad(buffer, deviceid.ToString());
                    break;

                //开启实时监控
                case 07:
                    //获取发过来的设备id，根据id找到socket
                    deviceid = UInt32.Parse(System.Text.Encoding.UTF8.GetString(buffer));//提取控制的deviceid

                    foreach (Session cliSession in svr._sessionTable.Values)
                    {
                        if (cliSession.DeviceId == deviceid)
                        {
                            svr.SendMessage(cliSession, nu, 0x0007, centercode);//开启视频
                            break;
                        }
                    }
                    
                    break;

                //中心站往检测仪结束实时视频

                case 08:
                    deviceid = UInt32.Parse(System.Text.Encoding.UTF8.GetString(buffer));//提取控制的deviceid
                    foreach (Session cliSession in svr._sessionTable.Values)
                    {
                        if (cliSession.DeviceId == deviceid)
                        {
                            svr.SendMessage(cliSession, nu, 0x0008, centercode);//开启视频
                            break;
                        }
                    }

                    break;
                case 09:
                    //视频上传（检测仪主动请求，中心站返回消息）
                    UInt32 res = 0;
                    byte[] meg = BitConverter.GetBytes(res);
                    Array.Reverse(meg);
                    svr.SendMessage(e.Client, meg, 0x000a, centercode);//录制视频上传应答
                    svr.FileBegine = true;
                    svr.IsPic = false;
                    break;
                
                //证件上传请求
                case 0x0000b:
                    UInt32 resu = 0;
                    byte[] megs = BitConverter.GetBytes(resu);
                    Array.Reverse(megs);
                    svr.SendMessage(e.Client, nu, 0x000c, centercode);//上传应答
                    svr.FileBegine = true;
                    svr.IsPic = true;
                    foreach (Session cliSession in svr._sessionTable.Values)
                    {
                        if (cliSession.ClientSocket == e.Client.ClientSocket)
                        {
                            svr.device_id = cliSession.DeviceId.ToString();
                        }
                    }
                    break;

                //视频调阅请求
                case 0x0000d:
                    byte[] dev = new byte[12];
                    deviceid = UInt32.Parse(System.Text.Encoding.UTF8.GetString(buffer, 0, 4));//提取控制的deviceid
                    batch_id = System.Text.Encoding.UTF8.GetString(buffer, 4, 8);//提取控制的deviceid
                    task_id = System.Text.Encoding.UTF8.GetString(buffer, 12, 4);//提取控制的deviceid
                    Array.Copy(buffer, 4, dev, 0, 12);

                    foreach (Session cliSession in svr._sessionTable.Values)
                    {
                        if (cliSession.DeviceId == deviceid)
                        {
                            svr.SendMessage(cliSession, nu, 0x0008, centercode);//开启视频
                            break;
                        }
                    }
                    break;
                //视频调阅
                case 0x000e:
                    //将文件存储路径插入数据库
                    string filepath = System.Text.Encoding.UTF8.GetString(buffer, 0, GetLen(buffer)).Trim();
                    //更新数据库状态
                    string sql2 = "UPDATE detectioninfo SET video_path = '" + filepath + "' WHERE batch_id = '" + batch_id + "' AND task_id = '" + task_id + "'";
                    database.Open();
                    database.ExcuteNonQuery(sql2);
                    database.Close();
                    break;
                //工作参数
                case 0x0000f:
                    break;
                //状态和工作参数
                case 0x0010:

                    break;
                //参数配置
                case 0x0011:
                    byte[] aa = new byte[4];
                    Array.Copy(buffer, 0, aa, 0, 4);
                    byte[] Msg = new byte[buffer.Length - 4];
                    Array.Copy(buffer, 4, Msg, 0, buffer.Length - 4);
                    deviceid = UInt32.Parse(System.Text.Encoding.UTF8.GetString(aa));//提取控制的deviceid
                    foreach (Session cliSession in svr._sessionTable.Values)
                    {
                        if (cliSession.DeviceId == deviceid)
                        {
                            svr.SendMessage(cliSession, Msg, 0x0012, centercode);//开启视频
                            break;
                        }
                    }

                    break;
                //数据管理
                case 0x0012:

                    break;
                //设备管理消息
                case 0x0013:

                    break;
                //远程版本升级
                case 0x0014:

                    break;
                //版本升级应答
                case 0x0015:

                    break;
                //通用消息
                case 0xf000:

                    break;
                default:
                    break;


            }

        }

    }
}
