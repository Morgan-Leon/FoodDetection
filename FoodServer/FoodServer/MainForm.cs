using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FoodServer.realmonitor;
using System.Data.SqlClient;
using FoodServer.Account;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using FoodServer.statistic;
using WindowsApplication1;
using FoodServer.TCPServ;
using System.Net;
using FlyTcpFramework;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using GHCS;
using GHCS.DataBase;
using MySql.Data.MySqlClient;
using System.Net.Sockets;
using FoodServer.Task;
using System.Reflection;
using FoodServer.Query;
using FoodServer.Realmonitor;
using FoodServer.checkedUnit;
using System.Timers;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using FoodServer.SystemSet;
using FoodServer.Class;
using TreeExXML;


namespace FoodServer
{
    public partial class MainForm : Form
    {
        #region 变量
        //数据库操作
        MySqlDataBase dbMySql = MySqlDataBase.getInstance();
        string databaseName;
        IDataBase database = MySqlDataBase.getInstance();
        MySqlDataAdapter myDataAdpter = new MySqlDataAdapter();
        DataSet myDataSet = new DataSet();
        string databasename;

        private AddCollection AddChileFrm;//定义子窗口对象
        private AddSVN AddChileSvn;
        static public string ProName;
        private TaskForm taskForm;//任务窗口
        public int interval = 1;//设置刷新时间，包括绘制报警以及报警信息显示
        private Account.account AccountFrm;//
        static public string m_csName;
        static public string m_userId;
        static public string m_csAddress;
        static public string m_csDeviceType;
        static public string m_csSoftwareVersion;
        //服务器
        //  TcpServerEvent tts;
       // public TcpSvr svr;

        //中心位置
        public struct DrawInfo
        {
            public Point m_point;
            public Size m_site;	//在整个窗口中的比例
            public bool m_bIsSelect;
            public Image img;
            public ImagControl imc;
            public string name;
            public int user_id;
        };
        int _X;
        int _Y;

        private bool isSelected;
        public ArrayList Pics = null;
        public ArrayList PicsSave;
        Bitmap bmpBack = null;
        string bkfile;
        Image imgr = Image.FromFile("room-r.png");
        Image imgb = Image.FromFile("room-b.png");
        Image imgg = Image.FromFile("room-g.png");
        Image imgy = Image.FromFile("room-y.png");
        Image imgn = Image.FromFile("room-n.png");

        //右键编辑节点
        TreeView treev;//右键选择的项
        string oldName = "";//修改前的节点名称
        TreeNode newTN;//新建的 节点
        public TreeNode SelectNode;//选择的节点
        //保存xml treeview
        public string xmlstr = "D:\\MyVidio\\myxml.xml";
        #endregion

        #region 服务器类
        public static TcpCli cli = null;
        private void MyTcpClient()
        {

            cli = new TcpCli(new Coder(Coder.EncodingMothord.UTF8), "D:\\");

            cli.Resovlver = new DatagramResolver("]");

            cli.ReceivedDatagram += new NetEvent(this.RecvData);

            cli.DisConnectedServer += new NetEvent(this.ClientClose);

            cli.ConnectedServer += new NetEvent(this.ClientConn);
        }
        void ClientConn(object sender, NetEventArgs e)
        {
            try
            {
                string info = string.Format("客户端:{0} 成功连接到服务器 :{1}", e.Client,
                e.Client.ClientSocket.RemoteEndPoint.ToString());
                MessageBox.Show(info);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("连接失败！");
            }

            
            // lblStatus.Text = info;
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

            // lblStatus.Text = info;
        }

        void RecvData(object sender, NetEventArgs e)
        {
            string info = string.Format("recv data:{0} from:{1}.", e.Client.Datagram, e.Client);

            //label1.Text = info;

        }


        #endregion

        #region 检测线程


        /// <summary>
        ///   检测监测点状态
        ///   1）绿色表示完全正常；
        ///   2）灰色表示该采集点状态无法获取（检测仪未开机，或者网络不通）；
        ///   3）黄色表示检测结果轻度不达标；
        ///   4）红色深度表示检测结果严重不达标。
        /// </summary>


        private void ThreadStart()
        {
            //重新加载绘图采集点

            //报告监测点状态

            int red = 0;
            int yello = 0;
            int black = 0;
            int pitch = 0;
            try
            {
                {
                    DrawInfo dr = new DrawInfo();
                    for (int i = 0; i < PicsSave.Count; ++i)
                    {
                        dr = (DrawInfo)PicsSave[i];
                        string device_id = dr.user_id.ToString();

                        string sqls = "SELECT Device_Status from device WHERE Device_ID = '" + device_id + "'";
                        DataSet dataSet = new DataSet();
                        dataSet.Clear();
                        int ret = database.ReadDataBase(sqls, "device", dataSet);
                        foreach (DataRow r in dataSet.Tables["device"].Rows)
                        {

                            if (int.Parse(r[0].ToString()) == 0)
                            {
                                dr.img = imgb;
                                dr.imc.PicboxShow = "room-b.png";
                                black++;
                            }
                            else
                            {

                                DateTime dt = DateTime.Now;
                                FoodServer.SystemSet.GetResult.DetectInfo di = new FoodServer.SystemSet.GetResult.DetectInfo();
                                GetResult gr = new GetResult();
                                di = gr.GetResultInfo(device_id, dt.AddYears(-1), dt);
                                if (di.m_rednum > 0)
                                {
                                    red++;
                                    dr.imc.PicboxShow = "room-r.png";
                                }

                                else if (di.m_fensenum > 0)
                                {
                                    pitch++;
                                    dr.imc.PicboxShow = "room-n.png";
                                }
                                else if (di.m_yellownum > 0)
                                {
                                    yello++;
                                    dr.imc.PicboxShow = "room-y.png";
                                }
                                else
                                {
                                    black++;
                                }

                            }

                        }

                        #region
                        /*
                        // if (TcpSvr.Usertable.ContainsKey(dr.user_id))
                        {
                            DataSet dt = new DataSet();
                            string sql = "SELECT p_name,channel,sample_name,submission_unit,Juge_result,Detect_result, Absolut_result FROM detectioninfo WHERE Device_ID = '" + dr.user_id + "'";
                            int ret = database.ReadDataBase(sql, "deteresult", dt);
                            if (ret == 0)
                            {

                                foreach (DataRow r in dt.Tables["deteresult"].Rows)
                                {
                                    double d_result = double.Parse(r["Detect_result"].ToString());
                                    double j_result = double.Parse(r["standard"].ToString());
                                    if (Math.Abs(d_result - j_result) > 0.5 && Math.Abs(d_result - j_result) < 1.0)
                                    {
                                        dr.imc.PicboxShow = "room-y.png";
                                        yello++;
                                    }
                                    else if (Math.Abs(d_result - j_result) >= 1.0)
                                    {
                                        dr.imc.PicboxShow = "room-r.png";
                                        red++;
                                    }
                                    else
                                    {
                                        dr.imc.PicboxShow = "room-g.png";

                                    }

                                    DrawImg(dr);

                                    //将数据更新到报警区域
                                }


                            }
                        }*/

                        #endregion

                        // if (TcpSvr.Usertable.ContainsKey(dr.user_id))


                    }
                    //委托开始
                    CrossDelegate dl = new CrossDelegate(Done);

                    StringBuilder sb = new StringBuilder();
                    sb.Append("\r\n");
                    sb.AppendFormat("采集点总数量: {0}\r\n", PicsSave.Count);
                    sb.Append("\r\n");
                    sb.AppendFormat("灰色报警数量: {0}\r\n", black);
                    sb.Append("\r\n");
                    sb.AppendFormat("黄色报警数量: {0}\r\n", yello);
                    sb.Append("\r\n");
                    sb.AppendFormat("粉色报警数量: {0}\r\n", pitch);
                    sb.Append("\r\n");
                    sb.AppendFormat("红色报警数量: {0}\r\n", red);
                    string text = sb.ToString();
                    this.BeginInvoke(dl, text); // 异步调用委托,调用后立即返回并立即执行下面的语句
                    //this.Invoke(dl, text); // 等待工作线程完成后, 才接着执行下面的语句.

                }
            }
            catch (System.Exception ex)
            {

            }

        }

        private void Done(string text)
        {
            label_AlarmInfo.Text = text;
        }
        private void Check_Status()
        {

            while (true)
            {
                Thread.Sleep(interval * 1000);
                ThreadStart();
            }


        }

        #region
        /*
        private void LoadAlarmHeader()
        {

            this.dataGridView1.Columns[0].HeaderText = "测试工程";
            this.dataGridView1.Columns[1].HeaderText = "通道";
            this.dataGridView1.Columns[2].HeaderText = "样品名称";
            this.dataGridView1.Columns[3].HeaderText = "送检单位";
            this.dataGridView1.Columns[4].HeaderText = "判定结果";
            this.dataGridView1.Columns[5].HeaderText = "测量结果";
            this.dataGridView1.Columns[6].HeaderText = "报警级别";
        }
        */

        //  private DataTable _dtAdd = new DataTable();
        //加载报警
        /* private void AlarmShow()
         {
             this.dataGridView1.DataSource = null;
             DataSet dt = new DataSet();
             string sql = "SELECT p_name,channel,sample_name,submission_unit,Juge_result,Detect_result ,abs(Detect_result - standard) FROM detectioninfo WHERE abs(Detect_result - standard) > 0.5 ";
             int ret = database.ReadDataBase(sql, "deteresult", dt);

             if (ret == 0)
             {

                 this.dataGridView1.DataSource = dt.Tables["deteresult"];

             }

             LoadAlarmHeader();//

         }*/
        /*
        /// <summary>
        ///   报警检测线程
        /// </summary>
        delegate void LoadAlarmDele();
        private void AlarmThread()
        {
            while (true)
            {
                Thread.Sleep(interval * 1000);
                LoadAlarm();
            }
        }

        private void LoadAlarm()
        {
            // 判断是否在线程中访问
            if (!dataGridView1.InvokeRequired)
            {
                // 不是的话直接操作控件
                this.dataGridView1.DataSource = null;
                DataSet dt = new DataSet();
                string sql = "SELECT p_name,channel,sample_name,submission_unit,Juge_result,Detect_result,abs(Detect_result - standard) FROM detectioninfo WHERE abs(Detect_result - standard) > 0.5 ";
                int ret = database.ReadDataBase(sql, "deteresult", dt);

                if (ret == 0)
                {
                    DataTable dd = new DataTable();
                    dd = dt.Tables["deteresult"].Clone();
                    double re = 0.0;
                    foreach (DataRow r in dd.Rows)
                    {
                        re = double.Parse(r[0].ToString());

//                         if (((re - 0.5) < 0.5))
//                         {
//                             
//                         }
//                         else if ()
//                         {
//                         }
//                         else
//                         {
// 
//                         }
                    }
                    this.dataGridView1.DataSource = dt.Tables["deteresult"];
                    

                }

                LoadAlarmHeader();//
            }
            else
            {
                // 是的话启用delegate访问
                LoadAlarmDele showProgress = new LoadAlarmDele(LoadAlarm);
                // 如使用Invoke会等到函数调用结束，而BeginInvoke不会等待直接往后走
              //  IntPtr i = this.Handle;
                if (this.IsHandleCreated) 
                this.BeginInvoke(showProgress);
            }
        }
        */
        #endregion
        #endregion

        #region 委托
        //用于信息刷新
        public delegate void CrossDelegate(string text);
        #endregion
        #region 主窗体
        public MainForm()
        {
            InitializeComponent();
         

            Pics = new ArrayList();
            PicsSave = new ArrayList();

            bmpBack = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics.FromImage(bmpBack).Clear(Color.White);
            bmpBack = (Bitmap)Bitmap.FromFile("BEIJING.bmp");
            pictureBox1.Image = (Bitmap)bmpBack.Clone();

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            //检测状态
            Thread Detectthread = new Thread(new ThreadStart(Check_Status));
            Detectthread.Start();
            Detectthread.IsBackground = true;


            //             Thread Alarmthread = new Thread(new ThreadStart(AlarmThread));
            //             Alarmthread.Start();
            //             Alarmthread.IsBackground = true;

            //  AddChileSvn.FormClosed += new FormClosedEventHandler(AddChileSvn.childForm_FormClosed);


        }

//         protected override void WndProc(ref Message m)
//         {
// 
//              if (m.Msg == 0x0014) // 禁掉清除背景消息
//                  return;
//  
//              base.WndProc(ref m);
// 
//         }

        private int CreateConnection()
        {
            return database.CreateConnection(null);
        }

        private void OpenDataBase()
        {
            database.Open();
        }
        private void CloseDataBase()
        {
            database.Close();
        }
        private void LoadAlarmInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\r\n");
            sb.AppendFormat("采集点总数量: {0}\r\n", 0);
            sb.Append("\r\n");
            sb.AppendFormat("灰色报警数量: {0}\r\n", 0);
            sb.Append("\r\n");
            sb.AppendFormat("黄色报警数量: {0}\r\n", 0);
            sb.Append("\r\n");
            sb.AppendFormat("红色报警数量: {0}\r\n", 0);
            this.label_AlarmInfo.Text = sb.ToString();
        }
        /// <summary>
        ///   主窗体加载
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            databasename = dbMySql.GetDatabaseName();
           
            //加载图片
            LoadPictrue();
            //加载树
        
            LoadTreeViewAll();
            //加载
            LoadResut();
            //加载报警
            LoadAlarmInfo();
            //AlarmShow();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

            PicsSave.Clear();
            lock (PicsSave)
            {
                for (int i = 0; i < Pics.Count; i++)
                {
                    PicsSave.Add(Pics[i]);

                }
                Redraw(PicsSave);
            }


            //开启接受数据
            MyTcpClient();
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否要关闭", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                e.Cancel = true; //这里表示取消退出 
            else
            {
                if (cli.IsConnected)
                {
                    cli.Close();
                }
                database.Close();
                System.Environment.Exit(0);
                
            }
        }
        #endregion

        #region 监测点信息统计
        /// <summary>
        /// 读取数据的条数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private int ReadDataCount(string sql)
        {
            int value = 0;
            List<string> list = new List<string>();

            database.QueryTable(sql, list, 1);
            if (list.Count > 0)
            {
                value = System.Convert.ToInt32(list[0]);
            }

            return value;

        }
        //中心位置
        public struct CollaterInfo
        {
            public string m_companyName;//企业名称
            public string m_devicetype;//检测仪器型号
            public string m_deviceVer;//检测仪器版本
            public int m_statues_total;//当天检测状态;总检测记录
            public int m_statues_good;//当天检测状态，合格记录
            public string m_result;//检测结果综合评价
        }
        private CollaterInfo GetCollaterInfo(string deviceid)
        {
            DateTime currenttime = System.DateTime.Now;//当天日期，暂时屏蔽
            CollaterInfo cinfo = new CollaterInfo();
            DataSet dataSet = new DataSet();
            dataSet.Clear();
            //获取当天信息
            string sql = "SELECT user.Company,device.Device_Type,device.Device_Ver,detectioninfo.Juge_result,detectioninfo.Detect_result,detectioninfo.Absolut_result " +
                         " FROM detectioninfo  INNER JOIN companyinfo ON detectioninfo.submission_unit = companyinfo.Company_Name " +
                         " INNER JOIN device ON detectioninfo.Device_ID = device.Device_ID INNER JOIN `user` ON detectioninfo.test_unit = `user`.Company " +
                         " WHERE  detectioninfo.Device_ID='" + deviceid + "'";// +
            // " AND detectioninfo.test_time = '" + currenttime.Date + "'";
            //获取当天检测条数
            string sqlCount = "SELECT COUNT(*) " +
                         " FROM detectioninfo  INNER JOIN companyinfo ON detectioninfo.submission_unit = companyinfo.Company_Name " +
                         " INNER JOIN device ON detectioninfo.Device_ID = device.Device_ID INNER JOIN `user` ON detectioninfo.test_unit = `user`.Company " +
                         " WHERE  detectioninfo.Device_ID='" + deviceid + "'";// +
            //  " AND detectioninfo.test_time = '" + currenttime.Date + "'";
            //获取合格检测条数
            string sqlGoodCount = "SELECT COUNT(*) " +
                         " FROM detectioninfo  INNER JOIN companyinfo ON detectioninfo.submission_unit = companyinfo.Company_Name " +
                         " INNER JOIN device ON detectioninfo.Device_ID = device.Device_ID INNER JOIN `user` ON detectioninfo.test_unit = `user`.Company " +
                         " WHERE  detectioninfo.Device_ID='" + deviceid + "'" +
                // " AND detectioninfo.test_time = '" + currenttime.Date + "'" +
                         " AND Juge_result ='合格'";
           // database.Open();
            database.ReadDataBase(sql, "Info", dataSet);
            int totalnum = ReadDataCount(sqlCount);
            int goodnum = ReadDataCount(sqlGoodCount);
            //database.Close();
            if (dataSet.Tables["Info"].Rows.Count != 0)
            {
                DataRow r = dataSet.Tables["Info"].Rows[0];
                // StringBuilder sb = new StringBuilder();
                cinfo.m_companyName = r["Company"].ToString();
                cinfo.m_devicetype = r["Device_Type"].ToString();
                cinfo.m_deviceVer = r["Device_Ver"].ToString();
                cinfo.m_result = ((double)((float)goodnum / (float)totalnum) * 100.0).ToString() + "%";
                cinfo.m_statues_good = goodnum;
                cinfo.m_statues_total = totalnum;
            }
            return cinfo;
        }
        #endregion


        #region 杂项
        public Dictionary<string, int> DictionaryColumns(DataGridView dgv, int n)
        {
            Dictionary<string, int> dct = new Dictionary<string, int>();
            string str = "";
            for (int i = 0; i < dgv.Rows.Count - 1; i++)
            {
                str = Convert.ToString(dgv.Rows[i].Cells[n].Value);

                if (dct.ContainsKey(str))//如果字典中已存在这个键，给这个键值加1
                {
                    dct[str]++;
                }
                else
                {
                    dct.Add(str, 1);//字典中不存在这个键，加入这个键
                }
            }
            return dct;
        }
        #endregion

        #region 树状图
        private void contextmenu_Click(object sender, EventArgs e)
        {

            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null)
            {
                switch (item.Text)
                {
                    case "添加单位":
                        AddSVN ad = new AddSVN();
                        ad.Show();
                        ad.fath = this;
                        break;
                    case "移除采集点":
                        TreeExXMLCls xt2 = new TreeExXMLCls();
                        TreeNode no = SelectNode.Parent;
                        xt2.DeleteXmlNodeByXPath(xmlstr, no,SelectNode.Text);
                        ReloadTreeView();
                        break;
                    case "添加采集点":
                        if (AddChileFrm == null || AddChileFrm.IsDisposed)
                        {
                            AddChileFrm = new AddCollection();
                            AddChileFrm.fa = this;
                        }

                        AddChileFrm.ShowDialog();
                        AddChileFrm.Focus();

                        break;
                    case "采集点属性":
                        XMLADD xd = new XMLADD();
                        xd.fa = this;
                        xd.Show();
                        break;
                    case "更换平面图":
                        //  DelNode();
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.InitialDirectory = "c:";//注意这里写路径时要用c:而不是c:　
                        openFileDialog.Filter = "jpg文件|*.jpg|png文件|*.png|bmp文件|*.bmp";
                        openFileDialog.RestoreDirectory = true;
                        openFileDialog.FilterIndex = 1;

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            bkfile = openFileDialog.FileName;

                        }
                        bmpBack = (Bitmap)Bitmap.FromFile(bkfile);
                        pictureBox1.Image = (Bitmap)bmpBack.Clone();

                        break;
                    default:
                        break;

                }

            }
        }



    
        //系统配置
        private void SystemSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SystemSet.SystemSet ss = new SystemSet.SystemSet();
            ss.ShowDialog();
        }
        #endregion

        #region 加载table

        /// <summary>
        ///   加载device 获取table
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
        ///   重新加载treeview
        /// </summary>
        /// 
        public void reloadview()
        {
            treeView1.Nodes.Clear();
            TreeExXMLCls xt = new TreeExXMLCls();
            if (File.Exists(xmlstr))
            {
                xt.addTreeNode(xmlstr, m_csName);
                xt.XMLToTree(xmlstr, treeView1);
            }
        }

        public void ReloadTreeView()
        {
            treeView1.Nodes.Clear();
            TreeExXMLCls xt = new TreeExXMLCls();
            if (File.Exists(xmlstr))
            {
                xt.XMLToTree(xmlstr, treeView1);
            }
        }
        /// <summary>
        ///   加载DeviceGroup
        /// </summary>
        private System.Data.DataTable LoadDeviceGroup()
        {
            DataSet dataset = new DataSet();
            dataset.Clear();
            string sql = "SELECT * FROM devicegroup";
            int ret = database.ReadDataBase(sql, "devicegroup", dataset);
            if (ret == 0)
            {
                System.Data.DataTable table = new System.Data.DataTable();
                table = dataset.Tables["devicegroup"];
                return table;
            }
            return null;
        }
        #region


        #endregion
        //递归遍历删除。
        protected void DeleteTreeNode2(TreeNode node)
        {
            //后序遍历
            foreach (TreeNode child in node.Nodes)
            {
                DeleteTreeNode2(child);
            }
            node.Remove();
        }
        /// <summary>
        ///   结构树数据载入
        ///   2013-06-27
        /// </summary>
        private void LoadTreeViewAll()
        {
            TreeNode allGroup = new TreeNode();
            DataSet datadevice = new DataSet();
            datadevice.Clear();
            string sql = "SELECT * FROM device";
            //判定是否存在xml文件
            TreeExXMLCls xt = new TreeExXMLCls();
            if (File.Exists(xmlstr))
            {
                xt.XMLToTree(xmlstr, treeView1);
            }
            else
            {
                //database.Open();
                database.ReadDataBase(sql, "device", datadevice);
                int countdevice = datadevice.Tables["device"].Rows.Count;
                TreeNode ParentNode = new TreeNode();
                ParentNode = treeView1.Nodes.Add("全部采集点");
                //显示数据
                for (int i = 0; i < countdevice; i++)
                {
                    string DeviceName = datadevice.Tables["device"].Rows[i]["Device_Name"].ToString();

                    allGroup = ParentNode.Nodes.Add(DeviceName);

                }
                //重点关注区域
                TreeNode SqureNode = new TreeNode();
                SqureNode = treeView1.Nodes.Add("重点区域采集点");

                TreeNode CompanyNode = new TreeNode();
                CompanyNode = treeView1.Nodes.Add("重点企业采集点");
                DataSet datagroup = new DataSet();
                datagroup.Clear();
                string sqls = "SELECT * FROM devicegroup";
                database.ReadDataBase(sqls, "devicegroup", datagroup);

                foreach (DataRow r in datagroup.Tables["devicegroup"].Rows)
                {
                    string ProjectName = r["Project_Name"].ToString();
                    if (r["Is_Squre"].Equals(0))//关注企业
                    {
                        allGroup = CompanyNode.Nodes.Add(ProjectName);
                    }
                    else //区域
                    {
                        allGroup = SqureNode.Nodes.Add(ProjectName);
                    }

                }

                //database.Close();
                //将数据保存到xml
                xt.TreeToXML(treeView1, xmlstr);

            }
            allGroup.ExpandAll();
        }
        /// <summary>
        ///   加载deviceresultinfo
        /// </summary>
        private void LoadResut()
        {
            DataSet dataset = new DataSet();
            string sql = "SELECT p_name,type,Juge_result,Info_Id,ftinfor.comparemax,standard,Detect_result,channel,result_unit,sample_no, sample_name,sites,submission_unit,test_operator,test_unit,test_time,detectioninfo.Device_ID  ,alarm_yellow,alarm_pitch,alarm_red FROM detectioninfo  INNER JOIN ftinfor ON (detectioninfo.p_name = ftinfor.ftestitems AND detectioninfo.type = ftinfor.fsample AND detectioninfo.standard = ftinfor.standards) INNER JOIN companyinfo ON detectioninfo.submission_unit = companyinfo.Company_Name " +
                        " INNER JOIN device ON detectioninfo.Device_ID = device.Device_ID INNER JOIN `user` ON detectioninfo.test_unit = `user`.Company  WHERE  Juge_result ='不合格' ORDER BY test_time DESC  ";
            int ret = database.ReadDataBase(sql, "result", dataset);

            if (ret == 0)
            {
                dataGridView2.DataSource = dataset.Tables["result"];
                for (int i = 0; i < dataset.Tables[0].Rows.Count; i++)
                {
                    double comparemax = Convert.ToDouble(dataset.Tables[0].Rows[i]["comparemax"].ToString());
                    double Detect_result = Convert.ToDouble(dataset.Tables[0].Rows[i]["Detect_result"].ToString());
                    double alarm_yellow = Convert.ToDouble(dataset.Tables[0].Rows[i]["alarm_yellow"].ToString());
                    double alarm_pitch = Convert.ToDouble(dataset.Tables[0].Rows[i]["alarm_pitch"].ToString());
                    double alarm_red = Convert.ToDouble(dataset.Tables[0].Rows[i]["alarm_red"].ToString());
                    SystemSet.SystemSet ss = new SystemSet.SystemSet();

                    //根据检测结果设置gridview背景颜色
                    if (((Detect_result - comparemax) >= comparemax * alarm_yellow) && ((Detect_result - comparemax) < comparemax * alarm_pitch))
                    {
                        this.dataGridView2.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                    }
                    else if (((Detect_result - comparemax) >= comparemax * alarm_pitch) && ((Detect_result - comparemax) < comparemax * alarm_red))
                    {
                        this.dataGridView2.Rows[i].DefaultCellStyle.BackColor = Color.Pink;
                    }
                    else if ((Detect_result - comparemax) >= comparemax * alarm_red)
                    {
                        this.dataGridView2.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    }
                    else
                    {
                        //this.dataGridView2.Rows[i].DefaultCellStyle.BackColor = Color.
                    }
                }
            }

            LoadResultHeader();

        }

        private void LoadResultHeader()
        {
            this.dataGridView2.Columns["Info_Id"].Visible = false;

            this.dataGridView2.Columns["p_name"].HeaderText = "检测项目";
            this.dataGridView2.Columns["type"].HeaderText = "样品类型";
            this.dataGridView2.Columns["channel"].HeaderText = "测量通道";
            this.dataGridView2.Columns["result_unit"].HeaderText = "结果单位";
            this.dataGridView2.Columns["standard"].HeaderText = "参考标准";
            this.dataGridView2.Columns["sample_no"].HeaderText = "样品编号";
            this.dataGridView2.Columns["sample_name"].HeaderText = "样品名称";
            this.dataGridView2.Columns["sites"].HeaderText = "产地";
            this.dataGridView2.Columns["submission_unit"].HeaderText = "送检单位";
            this.dataGridView2.Columns["test_operator"].HeaderText = "操作员";
            this.dataGridView2.Columns["test_unit"].HeaderText = "检测单位";
            this.dataGridView2.Columns["test_time"].HeaderText = "检测时间";
            this.dataGridView2.Columns["Device_ID"].HeaderText = "设备ID";
            this.dataGridView2.Columns["Detect_result"].HeaderText = "测量结果";
            this.dataGridView2.Columns["Juge_result"].HeaderText = "判定结果";
            this.dataGridView2.Columns["comparemax"].HeaderText = "参考最大值";

            this.dataGridView2.Columns["alarm_yellow"].HeaderText = "黄色报警比值";
            this.dataGridView2.Columns["alarm_pitch"].HeaderText = "粉色报警比值";
            this.dataGridView2.Columns["alarm_red"].HeaderText = "红色报警比值";

        }
        private PictureBox picbox = new PictureBox();
        /// <summary>
        ///   加载平面图以及各个终端节点
        /// </summary>
        private void LoadPictrue()
        {
            Pics.Clear();
            DataTable table = LoadDevice();
            Bitmap backeBuffer = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(backeBuffer);
            //Image img = Image.FromFile("room-b.png");
            foreach (DataRow r in table.Rows)
            {

                int x = int.Parse(r["Device_X"].ToString());
                int y = int.Parse(r["Device_Y"].ToString());
                // g.DrawLine(Pens.Red, x + 10, y + 10, pictureBox1.Width / 2, pictureBox1.Height / 2);

                ImagControl imgCo = new ImagControl();
                imgCo.Location = new Point((int)x, (int)y);

                imgCo.Cname = r["Device_Name"].ToString();

                DateTime dt = DateTime.Now;
                FoodServer.SystemSet.GetResult.DetectInfo di = new FoodServer.SystemSet.GetResult.DetectInfo();
                GetResult gr = new GetResult();
                di = gr.GetResultInfo(r["Device_ID"].ToString(), dt.AddYears(-1), dt);
                if (di.m_num == di.m_good)
                {
                    imgCo.PicboxShow = "room-g.png";
                }

                else if (di.m_rednum > 0)
                {
                    imgCo.PicboxShow = "room-r.png";
                }
                else if (di.m_fensenum > 0)
                {
                    imgCo.PicboxShow = "room-n.png";
                }
                else if (di.m_yellownum > 0)
                {
                    imgCo.PicboxShow = "room-y.png";
                }
                else
                {
                    imgCo.PicboxShow = "room-b.png";
                }
                this.pictureBox1.Controls.Add(imgCo);
                DrawInfo pic = new DrawInfo();
                // pic.img = img;
                pic.imc = imgCo;
                pic.m_point = imgCo.Location;
                pic.m_bIsSelect = false;
                pic.name = r["Device_Name"].ToString();
                pic.user_id = int.Parse(r["Device_ID"].ToString());


                Pics.Add(pic);
                pictureBox1.BackgroundImage = backeBuffer;
                pictureBox1.Refresh();
                imgCo.MouseMove += new MouseEventHandler(this.MouseMove);
                imgCo.MouseUp += new MouseEventHandler(this.Mouse_Up);
                imgCo.MouseDown += new MouseEventHandler(this.MouseDown);
                imgCo.MouseDoubleClick += new MouseEventHandler(this.MouseDoubleClick);
                imgCo.MouseHover += new EventHandler(this.MouseHover);


                //  Redraw(pic,pic.imc.Center);
            }
        }


        #endregion



        #region  菜单

        private void 多维度统计分析ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HistoryData historyData = new HistoryData();
            historyData.ShowDialog();
        }
        private void 添加中心站ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (AddChileSvn == null || AddChileSvn.IsDisposed)
            {
                AddChileSvn = new AddSVN();
                AddChileSvn.fath = this;
            }

            AddChileSvn.ShowDialog();
            AddChileSvn.Focus();

        }

        private void 添加采集点ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (AddChileFrm == null || AddChileFrm.IsDisposed)
            {
                AddChileFrm = new AddCollection();
                AddChileFrm.fa = this;
            }

            AddChileFrm.ShowDialog();
            AddChileFrm.Focus();
        }
        private void 账户管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AccountFrm == null || AccountFrm.IsDisposed)
            {
                AccountFrm = new Account.account();

            }

            AccountFrm.ShowDialog();
            AccountFrm.Focus();
        }

        private void 更换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "请选择文件";

            openFileDialog.Filter = "jpg|*.jpg|bmp|*.bmp|png|*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = openFileDialog.FileName;
                this.pictureBox1.Image = Image.FromFile(file);
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
            System.Environment.Exit(0);
        }


        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            关于本系统 aout = new 关于本系统();
            aout.ShowDialog();
        }

        private void 检测数据查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //             DetectQuray detectQury = new DetectQuray();
            //             detectQury.ShowDialog();
        }

        private void 统计报表ToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }
        private Point Position = new Point(0, 0);

        private void 采集点管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CollactorManager cm = new CollactorManager();
            cm.fath = this;
            cm.ShowDialog();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            BackUp.BackUp bu = new BackUp.BackUp();
            bu.ShowDialog();
        }

        private void 实时监测ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 送检单位管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckedUnit cu = new CheckedUnit();
            cu.ShowDialog();
        }

        private void 检测项目管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckPro.ChekPro cp = new CheckPro.ChekPro();
            cp.ShowDialog();
        }

        private void 报表管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*Report report = new Report();
            report.Show();*/
        }

        #endregion

        #region
        #endregion


        /// <summary>
        ///   删除节点
        /// </summary>
        private void DelNode(TreeNode node)
        {
            string msg = "您确定要删除选定的记录吗？";
            if (MessageBox.Show(msg, "删除工程", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                return;
            try
            {
                if (node.Parent == null)
                {
                    treeView1.Nodes.Remove(node);
                }
                else
                {
                    ((TreeNode)node.Parent).Nodes.Remove(node);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("删除记录错误信息： " + ex.ToString(), " 删除工程", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            string strsql = "DELETE FROM  " +
               "devicegroup WHERE Project_Name = '" + node.Text + "'";

            dbMySql.Open(databasename);
            dbMySql.ExcuteNonQuery(databasename, strsql);
            dbMySql.Close(databasename);
            //database.Close();
        }



        #region 鼠标事件
        /// <summary>
        ///   双击查询TreeView用户相关信息，并显示在gridview中
        /// </summary>
        private void Double_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        ///   鼠标动作管理
        ///   
        /// </summary>
        public void MouseHover(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            ImagControl mcl = (ImagControl)sender;
            mcl.Invalidate();
            DrawInfo d = getPicByControl(mcl);
            CollaterInfo ci = new CollaterInfo();
            ci = GetCollaterInfo(d.user_id.ToString());
            if (ci.m_statues_total == 0)
            {
                //
                DataSet dataSet = new DataSet();
                dataSet.Clear();
                string sql = "SELECT * FROM detectioninfo  INNER JOIN companyinfo ON detectioninfo.submission_unit = companyinfo.Company_Name  INNER JOIN device ON detectioninfo.Device_ID = device.Device_ID INNER JOIN `user` ON detectioninfo.test_unit = `user`.Company WHERE  detectioninfo.Device_ID='" + d.user_id + "'";
                database.ReadDataBase(sql, "Info", dataSet);

                if (dataSet.Tables["Info"].Rows.Count != 0)
                {
                    DataRow r = dataSet.Tables["Info"].Rows[0];

                    sb.AppendFormat("设备ID:{0}\r\n", d.user_id);
                    sb.AppendFormat("公司名称:{0}\r\n", r["test_unit"]);
                    //sb.AppendFormat("公司电话:{0}\r\n", r["Mobile"]);
                    // sb.AppendFormat("公司Email:{0}\r\n", r["Email"]);
                    sb.AppendFormat("设备型号:{0}\r\n", r["Device_Type"]);
                    sb.AppendFormat("检测点名称:{0}\r\n", r["Device_Name"]);

                }
                else
                {

                }
            }
            else
            {
                sb.AppendFormat("设备ID:{0}\r\n", d.user_id);
                sb.AppendFormat("公司名称:{0}\r\n", ci.m_companyName);
                sb.AppendFormat("设备型号:{0}\r\n", ci.m_devicetype);
                sb.AppendFormat("设备版本:{0}\r\n", ci.m_deviceVer);
                sb.AppendFormat("检测总量:{0}\r\n", ci.m_statues_total);
                sb.AppendFormat("检测结果评价:{0}\r\n", ci.m_result);

            }

            ToolTip ttpSettings = new ToolTip();
            ttpSettings.InitialDelay = 200;
            ttpSettings.AutoPopDelay = 5000;
            ttpSettings.ReshowDelay = 200;
            ttpSettings.ShowAlways = true;
            ttpSettings.IsBalloon = true;
            ttpSettings.ToolTipTitle = "采集点信息";
            string tipOverwrite = sb.ToString();
            ttpSettings.SetToolTip(mcl, tipOverwrite); // ckbOverwrite is a checkbox
        }
        private Point downPoint;//单击picturebox的坐标
        public void MouseDown(object sender, MouseEventArgs e)
        {
            this.SuspendLayout();
            isSelected = true;
            _X = e.X;
            _Y = e.Y;

            DataTable tbDevice = LoadDevice();
            DataSet ds = new DataSet();
            // deviceTableAdapter dt = new deviceTableAdapter();
            Point P = this.splitContainer2.PointToClient(Control.MousePosition);
            if (e.Button == MouseButtons.Left)
            {
                ImagControl mcl = (ImagControl)sender;
                mcl.Invalidate();
                DrawInfo d = getPicByControl(mcl);
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.Cells["Device_ID"].Value.ToString() == d.user_id.ToString())
                    {
                        row.Visible = true;

                    }
                    else
                    {
                        CurrencyManager cm = (CurrencyManager)BindingContext[dataGridView2.DataSource];

                        cm.SuspendBinding(); //挂起数据绑定

                        row.Visible = false;

                        cm.ResumeBinding(); //恢复数据绑定
                        // row.Visible = false; 
                    }
                }

            }

            if (e.Button == MouseButtons.Right)
            {
                ImagControl mcl = (ImagControl)sender;
                mcl.Invalidate();
                DrawInfo d = getPicByControl(mcl);
                // MessageBox.Show("dd");
                CollactorConfig CC = new CollactorConfig(d.user_id.ToString());
                CC.father = this;
                CC.Show();

            }

        }
        public void MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ImagControl mcl = (ImagControl)sender;
            mcl.Invalidate();

            DrawInfo d = getPicByControl(mcl);
            if (taskForm == null || taskForm.IsDisposed)
            {
                taskForm = new TaskForm(d);
                taskForm.father = this;

            }

            taskForm.ShowDialog();
            taskForm = null;

        }
        public void MouseMove(object sender, MouseEventArgs e)
        {
            //设置单击picturebox的坐标
            if (e.Button == MouseButtons.Left)
            {
                ((Control)sender).Location = new Point(
              ((Control)sender).Location.X + e.X - downPoint.X,
              ((Control)sender).Location.Y + e.Y - downPoint.Y);

                ImagControl mcl = (ImagControl)sender;
                mcl.Invalidate();
                //   DrawInfo d = getPicByControl(mcl);

                //  Point p = new Point(e.X - _X + mcl.Left, e.Y - _Y + mcl.Top);
                // mcl.Location = p;
                // Redraw(d, p);
            }

            // Redraw(d, p);
        }

        public void Mouse_Up(object sender, MouseEventArgs e)
        {

            isSelected = false;
            ResumeLayout();

            ImagControl mcl = (ImagControl)sender;

            // mcl.Invalidate();

            DrawInfo d = getPicByControl(mcl);
            Point p = pictureBox1.PointToClient(Control.MousePosition);

            d.imc.Center = p;
            if (p.X > 0 && p.X < pictureBox1.Width && p.Y > 0 && p.Y <= pictureBox1.Height)
            {
                //并更新坐标点
                //database.Open();
                string sql = "UPDATE device ";
                sql += " SET Device_X=" + d.imc.Center.X + ",";
                sql += "Device_Y=" + d.imc.Center.Y;
                sql += " WHERE Device_ID= " + d.user_id;
                database.ExcuteNonQuery(sql);
                //database.Close();
            }
            Redraw(d, p);

        }
        #endregion

        #region


        private Size beforeResizeSize = Size.Empty;

        protected override void OnResizeBegin(EventArgs e)
        {
            base.OnResizeBegin(e);
            beforeResizeSize = this.Size;
        }
        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            //窗口resize之后的大小
            Size endResizeSize = this.Size;
            //获得变化比例
            float percentWidth = (float)endResizeSize.Width / beforeResizeSize.Width;
            float percentHeight = (float)endResizeSize.Height / beforeResizeSize.Height;
            foreach (Control control in this.Controls)
            {
                if (control is DataGridView)
                    continue;
                //按比例改变控件大小
                control.Width = (int)(control.Width * percentWidth);
                control.Height = (int)(control.Height * percentHeight);
                //为了不使控件之间覆盖 位置也要按比例变化
                control.Left = (int)(control.Left * percentWidth);
                control.Top = (int)(control.Top * percentHeight);
            }
        }

        private DrawInfo getPicByControl(ImagControl m)
        {
         
            foreach (DrawInfo i in PicsSave)
            {
                if (i.imc == m)
                {
                    return i;
                   
                }
            }

            throw new Exception("No imagine found");
        }

        delegate void ShowpicDelegate(DrawInfo img);//委托。多线程访问
        private void DrawImg(DrawInfo img)
        {
            // Graphics g = pictureBox1.CreateGraphics();
            // 判断是否在线程中访问
            if (!pictureBox1.InvokeRequired)
            {
                // 不是的话直接操作控件
                Bitmap backBuffer = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                Graphics g = Graphics.FromImage(backBuffer);

                g.DrawImage(pictureBox1.Image, img.imc.Center.X, img.imc.Center.Y);

                //并更新坐标点

                //this.pictureBox1.Refresh();
                g.Dispose();
                backBuffer.Dispose();
            }
            else
            {
                //pictureBox1.Refresh();
                // 是的话启用delegate访问
                ShowpicDelegate showProgress = new ShowpicDelegate(DrawImg);
                // 如使用Invoke会等到函数调用结束，而BeginInvoke不会等待直接往后走
                this.BeginInvoke(showProgress, new object[] { img });
            }

        }
        //Returns the region to update
        private Region getRegionByLine(DrawInfo l, Point p)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(new Point[] { l.imc.Center, p, l.imc.Center });

            RectangleF rf = gp.GetBounds();
            gp.Dispose();

            rf.Inflate(100f, 100f);

            return new Region(rf);
        }
        public void Redraw(DrawInfo line, Point p)
        {
            foreach (DrawInfo l in PicsSave)
            {
                this.pictureBox1.Controls.Add(l.imc);
                DrawImg(l);

            }

            Region r = getRegionByLine(line, p);
            pictureBox1.Invalidate(r);
            pictureBox1.Refresh();
        }
        public void Redraw(ArrayList Pic)
        {
            this.pictureBox1.Invalidate();
            this.pictureBox1.Update();
            foreach (DrawInfo l in Pic)
            {
                this.pictureBox1.Controls.Add(l.imc);
                DrawImg(l);

            }

            pictureBox1.Refresh();
        }


        /// <summary>
        ///   加载用户信息
        /// </summary>
        private void LoadUserInfo()
        {

            DataSet dataset = new DataSet();

        }
        UInt32 user_id;
        public Session valuse;

        private void 数据查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DetectQuray dq = new DetectQuray();
            dq.Show();
        }
        public static string getLocalIP()
        {
            string strHostName = Dns.GetHostName(); //得到本机的主机名
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName); //取得本机IP
            string strAddr = ipEntry.AddressList[1].ToString();
            return (strAddr);
        }

        private void 连接服务器ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
           
            string ipAddrStr = getLocalIP();
            ushort portNumber = 5632;
            IniAc ini = new IniAc();
            ipAddrStr = ini.ReadValue("NET", "ip");

            string portStr = ini.ReadValue("NET", "port");
            portNumber = System.Convert.ToUInt16(portStr);
      
 
            try
            {
                cli.Connect(ipAddrStr, portNumber);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void 断开连接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cli.IsConnected)
            {
                cli.Close();
            }

        }

        //int[]到string[]的转换  

        public string IntTostr(int[] arry)
        {

            string ss = "";
            string[] str_array = Array.ConvertAll(arry, new Converter<int, string>(IntToString));

            foreach (string s in str_array)
            {
                ss += s;
            }
            return ss;
        }

        public static string IntToString(int i)
        {
            return i.ToString();
        }

        /// <summary> 
        /// 字符串转16进制字节数组 
        /// </summary> 
        /// <param name="hexString"></param> 
        /// <returns></returns> 
        private static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary> 
        /// 字节数组转16进制字符串 
        /// </summary> 
        /// <param name="bytes"></param> 
        /// <returns></returns> 
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        /// <summary> 
        /// 从汉字转换到16进制 
        /// </summary> 
        /// <param name="s"></param> 
        /// <param name="charset">编码,如"utf-8","gb2312"</param> 
        /// <param name="fenge">是否每字符用逗号分隔</param> 
        /// <returns></returns> 
        public static string ToHex(string s, string charset, bool fenge)
        {
            if ((s.Length % 2) != 0)
            {
                s += " ";//空格 
                //throw new ArgumentException("s is not valid chinese string!"); 
            }
            System.Text.Encoding chs = System.Text.Encoding.GetEncoding(charset);
            byte[] bytes = chs.GetBytes(s);
            string str = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                str += string.Format("{0:X}", bytes[i]);
                if (fenge && (i != bytes.Length - 1))
                {
                    str += string.Format("{0}", ",");
                }
            }
            return str.ToLower();
        }

        ///<summary> 
        /// 从16进制转换成汉字 
        /// </summary> 
        /// <param name="hex"></param> 
        /// <param name="charset">编码,如"utf-8","gb2312"</param> 
        /// <returns></returns> 
        public static string UnHex(string hex, string charset)
        {
            if (hex == null)
                throw new ArgumentNullException("hex");
            hex = hex.Replace(",", "");
            hex = hex.Replace("\n", "");
            hex = hex.Replace("\\", "");
            hex = hex.Replace(" ", "");
            if (hex.Length % 2 != 0)
            {
                hex += "20";//空格 
            }
            // 需要将 hex 转换成 byte 数组。 
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    // 每两个字符是一个 byte。 
                    bytes[i] = byte.Parse(hex.Substring(i * 2, 2),
                    System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    // Rethrow an exception with custom message. 
                    throw new ArgumentException("hex is not a valid hex number!", "hex");
                }
            }
            System.Text.Encoding chs = System.Text.Encoding.GetEncoding(charset);
            return chs.GetString(bytes);
        }

       

 
        #endregion

    

        private void TreeView_MouseDown(object sender, MouseEventArgs e)
        {
           

        }

        private void TreeView_MouseUp(object sender, MouseEventArgs e)
        {
            TreeView treev = sender as TreeView;
            Point point = treev.PointToClient(Cursor.Position);
            TreeViewHitTestInfo info = treev.HitTest(point.X, point.Y);
            TreeNode node = info.Node;
           // ContextMenu contextMenu = new ContextMenu();
            if (node != null && MouseButtons.Right == e.Button)
            {

                treev.SelectedNode = node;
                if ("全部采集点" == node.Text)
                {

                    this.contextMenuStrip2.Items.Clear();
                    this.contextMenuStrip2.Items.Add("添加采集点");

                    for (int i = 0; i < this.contextMenuStrip2.Items.Count; i++)
                    {
                        this.contextMenuStrip2.Items[i].Click += new EventHandler(contextmenu_Click);
                    }
                   
                    treev.ContextMenuStrip = this.contextMenuStrip2;
                }
                else if (node.Parent != null && "全部采集点" == node.Parent.Text)
                {
                    this.contextMenuStrip2.Items.Clear();
                    this.contextMenuStrip2.Items.Add("采集点属性");
                    SelectNode = node;
                    for (int i = 0; i < this.contextMenuStrip2.Items.Count; i++)
                    {
                        this.contextMenuStrip2.Items[i].Click += new EventHandler(contextmenu_Click);
                    }

                    treev.ContextMenuStrip = this.contextMenuStrip2;
                }
                else if(node.Level == 0)
                {

                    this.contextMenuStrip2.Items.Clear();
                    SelectNode = node;
                    this.contextMenuStrip2.Items.Add("添加单位");
                    this.contextMenuStrip2.Items.Add("更换平面图");

                    for (int i = 0; i < this.contextMenuStrip2.Items.Count; i++)
                    {
                        this.contextMenuStrip2.Items[i].Click += new EventHandler(contextmenu_Click);
                    }
                    treev.ContextMenuStrip = this.contextMenuStrip2;
                }
                else if (node.Level == 1)
                {

                    this.contextMenuStrip2.Items.Clear();
                    this.contextMenuStrip2.Items.Add("更换平面图");

                    for (int i = 0; i < this.contextMenuStrip2.Items.Count; i++)
                    {
                        this.contextMenuStrip2.Items[i].Click += new EventHandler(contextmenu_Click);
                    }
                    treev.ContextMenuStrip = this.contextMenuStrip2;
                }
                else if (node.Level == 2)
                {
                    this.contextMenuStrip2.Items.Clear();
                    SelectNode = node;
                    this.contextMenuStrip2.Items.Add("移除采集点");

                    for (int i = 0; i < this.contextMenuStrip2.Items.Count; i++)
                    {
                        this.contextMenuStrip2.Items[i].Click += new EventHandler(contextmenu_Click);
                    }
                    treev.ContextMenuStrip = this.contextMenuStrip2;
                }
                else
                {
                    this.contextMenuStrip2.Items.Clear();
                    treev.ContextMenuStrip = null;
                }
              
            }
        }

        private void TreeView_DoubleClick(object sender, MouseEventArgs e)
        {
            if (this.treeView1.SelectedNode.Nodes.Count == 0)
            {
                return;
            }
            //pictureBox1.Refresh();
            PicsSave.Clear();
            this.pictureBox1.Controls.Clear();
            try
            {
                if (this.treeView1.SelectedNode != null)
                {
                    TreeNode tnode = treeView1.SelectedNode;
                    string name = tnode.Text;
                    int node_level = 0;
                    node_level = this.treeView1.SelectedNode.Level;

                    //全部显示
                    if (tnode.Parent == null)
                    {
                        //所有的都显示
                        PicsSave.Clear();
                        this.pictureBox1.Controls.Clear();
                        for (int i = 0; i < Pics.Count; i++)
                        {
                            PicsSave.Add(Pics[i]);

                        }

                        Redraw(PicsSave);
                        //pictureBox1.Refresh();
                    }
                    else if (node_level == 1)
                    {
                        PicsSave.Clear();
                        this.pictureBox1.Controls.Clear();
                        //只显示所在分组
                        TreeNodeCollection nodes = tnode.Nodes;
                        if (nodes.Count > 0)
                            foreach (TreeNode tn in nodes)
                            {
                                foreach (DrawInfo i in Pics)
                                {
                                    if (i.name == tn.Text)
                                    {
                                        PicsSave.Add(i);
                                    }
                                    else
                                    {
                                        this.pictureBox1.Controls.Remove(i.imc);
                                    }
                                }

                            }
                        Redraw(PicsSave);
                    }
                    else
                    {

                    }
                }
            }
            catch
            {

            }
        }



    }
}
