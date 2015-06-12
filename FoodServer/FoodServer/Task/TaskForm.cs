using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FlyTcpFramework;
using System.Collections;
using GHCS.DataBase;
using GHCS;
using MySql.Data.MySqlClient;
using FoodServer.Query;
using System.IO;
using System.Diagnostics;


namespace FoodServer.Task
{
    public partial class TaskForm : Form
    {
        public MainForm father;//父窗口变量
        FoodServer.MainForm.DrawInfo drwInfo = new FoodServer.MainForm.DrawInfo();
       
        UInt32 user_id;
        MySqlDataBase dbMySql = MySqlDataBase.getInstance();
        string databaseName;
        IDataBase database = MySqlDataBase.getInstance();
        MySqlDataAdapter myDataAdpter = new MySqlDataAdapter();
        DataSet myDataSet = new DataSet();
        string path = @"D:\\MyVidio";
        public TaskForm()
        {
            InitializeComponent();
            
        }
        public  TaskForm(FoodServer.MainForm.DrawInfo d )
        {
            InitializeComponent();
            this.comboBox1.Text = d.name.Trim().ToString();
            drwInfo = d;
            DetectQuray frm= new DetectQuray(d.user_id.ToString()); 
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None; 
            frm.Parent = this.panel1;  
           // frm.SetBounds(0,0,this.panel1.Width, this.panel1.Height);  
            frm.Show();  
           
        }
        /// <summary>
        /// 查找指定文件夹下指定后缀名的文件
        /// </summary>
        /// <param name="directory">文件夹</param>
        /// <param name="pattern">后缀名</param>
        /// <returns>文件路径</returns>
        public static List<string> GetFiles(DirectoryInfo directory, string pattern)
        {
            List<string> result = new List<string>();
            if (directory.Exists || pattern.Trim() != string.Empty)
            {
                try
                {
                    foreach (FileInfo info in directory.GetFiles(pattern))
                    {
                        result.Add(info.FullName.ToString());
                        //num++;
                    }
                }
                catch { }
                foreach (DirectoryInfo info in directory.GetDirectories())
                {
                    GetFiles(info, pattern);
                }
            }
            return result;
        }
        private void TaskForm_Load(object sender, EventArgs e)
        {
            DirectoryInfo theFolder = new DirectoryInfo(path);
//             if (theFolder.Exists)
//             {
// 
// 
//                 List<string> FindResult = GetFiles(theFolder, "*.*");
//                 this.listView1.View = View.List;
//                 this.listView1.BeginUpdate();
//                 for (int i = 0; i < FindResult.Count; i++)
//                 {
//                     ListViewItem lvi = new ListViewItem();
//                     lvi.Text = FindResult[i];
//                     this.listView1.Items.Add(lvi);
//                 }
//                 this.listView1.EndUpdate();
//             }
//             else
//             {
//                 MessageBox.Show("请核查保存目录！");
//             }
        }

        private string GetIp(string device_id)
        {
            string ip = "1234";
            DataSet dataSet  = new DataSet();
            int id = int.Parse(device_id);
            string sql = " SELECT Sockets from device WHERE Device_ID =  "+id+"";
            int ret = database.ReadDataBase(sql, "devi", dataSet);
            if (ret == 0)
            {
                if (dataSet.Tables["devi"].Rows.Count == 1)
                {
                    string ipadd = dataSet.Tables["devi"].Rows[0]["Sockets"].ToString();
                    int index = ipadd.IndexOf(":");
                    ip = ipadd.Substring(0,index);
                    return ip;
                }
            }
       
                return null;
         
           
        }
        public string ipad;
        public string Url;
        private void button_Start_Click(object sender, EventArgs e)
        {
            
             string device_id = drwInfo.user_id.ToString();
             ipad = GetIp(device_id);
             Url = "http://" + ipad + ":8090/test.swf";
             byte[] buf = System.Text.Encoding.UTF8.GetBytes(device_id);
             webBrowser1.Url = new Uri(Url);
             //数据库查询deviceid
             MainForm.cli.SendMessage(buf, 0x0007, "1");
             System.Diagnostics.Process.Start(Url);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string device_id = drwInfo.user_id.ToString();
            byte[] buf = System.Text.Encoding.UTF8.GetBytes(device_id);
            //数据库查询deviceid
            MainForm.cli.SendMessage(buf, 0x0008, "1");
            webBrowser1.Url = null;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string device_id = drwInfo.user_id.ToString();
            byte[] buf = System.Text.Encoding.UTF8.GetBytes(device_id);
            //数据库查询deviceid
            MainForm.cli.SendMessage(buf, 0x000d, "1");
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MouseDoubleClick(object sender, MouseEventArgs e)
        {
//             int index = listView1.SelectedIndices[0];//获取当前选中的索引
//             string sysFolder =
//             Environment.GetFolderPath(Environment.SpecialFolder.System);
//             //Create a new ProcessStartInfo structure.
//             ProcessStartInfo pInfo = new ProcessStartInfo();
//             //Set the file name member. 
//             pInfo.FileName = listView1.Items[index].Text;
//             //UseShellExecute is true by default. It is set here for illustration.
//             pInfo.UseShellExecute = true;
//             Process p = Process.Start(pInfo);
          
         
        }

        private void Tab_SelectIndexChanged(object sender, EventArgs e)
        {
 
        }

      
    }
}
