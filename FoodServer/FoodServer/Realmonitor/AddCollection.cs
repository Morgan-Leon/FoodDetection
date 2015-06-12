using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FoodServer;
using FlyTcpFramework;
using GHCS.DataBase;
using GHCS;
using GHCS.Common;
using FoodServer.Realmonitor;
namespace FoodServer.realmonitor
{
    public partial class AddCollection : Form
    {
       // public delegate void SendFunc(bool str);
        //public event SendFunc SendToParent;
        public MainForm fa;//父窗口变量
        MySqlDataBase dbMySql = MySqlDataBase.getInstance();
        IDataBase database = MySqlDataBase.getInstance();
        string databasename;
        string User_Name = "";
        int IDNum =0;
        CollactorManager parent;
        CollactorManager.AlarmParameter parameter;

        public string newDeviceName;
        public AddCollection()
        {
            InitializeComponent();
            databasename = dbMySql.GetDatabaseName();
            parameter = new CollactorManager.AlarmParameter();
           
        }
        
        public AddCollection(CollactorManager.AlarmParameter pa,CollactorManager cm)
        {
            InitializeComponent();
            databasename = dbMySql.GetDatabaseName();
            parameter = pa;
            parent = cm;
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
           
                ToolTip tt = new ToolTip();
                tt.IsBalloon = true;   //如果false就是一个方块型的提示框   
                tt.SetToolTip(this.textBox_name, "请输入非数字名称！");
                tt.Show("请输入非数字名称！", this.textBox_name);
               // return;
           


            if (parameter.fuc == "update")
            {
                string strsql = "Update device SET "
                    + "Device_ID = '" + textBox_ID.Text.ToString() + "',"
                    + "Device_PassWord = '" + textBox_Password.Text.ToString() + "',"
                    + "Device_Code = '" + textBox_Code.Text.ToString() + "',"
                    + "Device_Name = '" + textBox_name.Text.ToString() + "',"
                    + "Device_Addr = '" + textBox_Addr.Text.ToString() + "',"
                    + "Device_Type = '" + textBox_Type.Text.ToString() + "',"
                    + "Device_Ver = '" + textBox_version.Text.ToString() + "'"
                    + "WHERE Device_ID = " + IDNum + "";

                dbMySql.Open(databasename);
                dbMySql.ExcuteNonQuery(databasename, strsql);
                dbMySql.Close(databasename);
                this.Close();
                parent.Device_Load();
                newDeviceName = textBox_name.Text.ToString();
                this.DialogResult = DialogResult.OK;
                MessageBox.Show("修改成功");
            }
            else
            {
                if (string.IsNullOrEmpty(textBox_name.Text.Trim()))
                {
                    MessageBox.Show("请输入终端名称！");
                    return;
                }
                else
                {
                    DataSet dataSet = new DataSet();
                    string sql = "SELECT * FROM device";
                    int ret = database.ReadDataBase(sql, "device", dataSet);
                    FoodServer.MainForm.m_csName = this.textBox_name.Text.Trim().ToString();
                    if (ret == 0)
                    {
                        foreach (DataRow row in dataSet.Tables["device"].Rows)
                        {
                            if (row["Device_Name"].ToString() == FoodServer.MainForm.m_csName)
                            {
                                MessageBox.Show("该终端已经存在，请重新输入终端名称！");
                                return;
                            }
                        }
                    }
                }

                // FoodServer.MainForm.m_userId = User_Name;
                FoodServer.MainForm.m_csAddress = textBox_Addr.Text.Trim().ToString();
                FoodServer.MainForm.m_csDeviceType = textBox_Type.Text.Trim().ToString();
                FoodServer.MainForm.m_csSoftwareVersion = textBox_version.Text.Trim().ToString();
                int userid = 0;
                int group_id = 0;
                int x = 300;
                int y = 150;
                string strsql = "Insert Into " +
                    "device(Device_Group_ID,Device_Name,device.Device_Addr,device.Device_Type,device.Device_Ver, device.Device_X,device.Device_Y)"
                    + "values('" + group_id + "','" + FoodServer.MainForm.m_csName + "','" + FoodServer.MainForm.m_csAddress + "','" + FoodServer.MainForm.m_csDeviceType + "','" + FoodServer.MainForm.m_csSoftwareVersion + "'," + x + "," + y + ")";
                try
                {
                    dbMySql.Open(databasename);
                    dbMySql.ExcuteNonQuery(databasename, strsql);
                    dbMySql.Close(databasename);

                    DataSet dataSet1 = new DataSet();
                    string sql1 = "SELECT * FROM device";
                    int re = database.ReadDataBase(sql1, "device2", dataSet1);
                    if (re == 0)
                    {
                        foreach (DataRow r in dataSet1.Tables["device2"].Rows)
                        {
                            if (r["Device_Name"].ToString() == FoodServer.MainForm.m_csName)
                            {
                                userid = int.Parse(r["Device_ID"].ToString());
                            }
                        }

                    }

                    ImagControl mark = new ImagControl();
                    mark.Location = new Point(150, 150);
                    mark.PicboxShow = "room-b.png";
                    mark.Center = mark.Location;
                    mark.Cname = this.textBox_name.Text.ToString();
                    fa.pictureBox1.Controls.Add(mark);
                    fa.treeView1.Refresh();
                    fa.reloadview();
                    
                    // fa.dataGridView1.Refresh();

                    FoodServer.MainForm.DrawInfo line = new FoodServer.MainForm.DrawInfo();
                    line.imc = mark;
                    line.user_id = userid;
                    line.name = FoodServer.MainForm.m_csName;
                    fa.Pics.Add(line);
                    fa.PicsSave.Add(line);
                    fa.Redraw(line, mark.Center);

                    mark.MouseUp += new System.Windows.Forms.MouseEventHandler(fa.Mouse_Up);
                    mark.MouseDown += new System.Windows.Forms.MouseEventHandler(fa.MouseDown);
                    mark.MouseMove += new System.Windows.Forms.MouseEventHandler(fa.MouseMove);
                    mark.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(fa.MouseDoubleClick);
                    mark.MouseHover += new System.EventHandler(fa.MouseHover);
                    //Adds Line object to an arraylist


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                this.DialogResult = DialogResult.OK;
            }

            this.Close();
       
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void ComBox_Init()
        {
            //加载user下拉框
//             IDataBase database = MySqlDataBase.getInstance();
//             ComboBoxDataBase boxDataBase = new ComboBoxDataBase();
//             string sql = "SELECT User_ID FROM user";
//             boxDataBase.BindDataBase(ref this.comboBox_User, ref database, sql);
        }
        private void AddCollection_Load(object sender, EventArgs e)
        {
            ComBox_Init();
            if (parameter.fuc =="update")
            {
                int rowNum = parent.getRow();//获取datagriadview行号
                //DataGridViewRow r = parent.dataGridView1.Rows[rowNum];
                this.textBox_ID.Text = parent.dataGridView1.Rows[rowNum].Cells["Device_ID"].Value.ToString();
                this.textBox_Password.Text = parent.dataGridView1.Rows[rowNum].Cells["Device_PassWord"].Value.ToString();
                this.textBox_Code.Text = parent.dataGridView1.Rows[rowNum].Cells["Device_Code"].Value.ToString();
                this.textBox_name.Text = parent.dataGridView1.Rows[rowNum].Cells["Device_Name"].Value.ToString();
                this.textBox_Addr.Text = parent.dataGridView1.Rows[rowNum].Cells["Device_Addr"].Value.ToString();
                this.textBox_Type.Text = parent.dataGridView1.Rows[rowNum].Cells["Device_Type"].Value.ToString();
                this.textBox_version.Text = parent.dataGridView1.Rows[rowNum].Cells["Device_Ver"].Value.ToString();
                IDNum = int.Parse(parent.dataGridView1.Rows[rowNum].Cells["Device_ID"].Value.ToString());
            }
            
        }

        private void ComBox_UserSelectChanged(object sender, EventArgs e)
        {

          //  User_Name = this.comboBox_User.Text.ToString();
        }

      

      
    }
}
