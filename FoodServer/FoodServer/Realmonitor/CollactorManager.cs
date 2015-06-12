using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GHCS.DataBase;
using GHCS;
using MySql.Data.MySqlClient;
using FoodServer.realmonitor;

namespace FoodServer.Realmonitor
{
    public partial class CollactorManager : Form
    {
        MySqlDataBase dbMySql = MySqlDataBase.getInstance();
        string databaseName;
        IDataBase database = MySqlDataBase.getInstance();
        MySqlDataAdapter myDataAdapter = new MySqlDataAdapter();//定义一个数据适配器
        DataSet myDataSet = new DataSet();
        public MainForm fath;
        public CollactorManager()
        {
            InitializeComponent();
            
        }

        public class AlarmParameter
        {
            public AlarmParameter()
            {
                Title = "insert";
                fuc = "insert";
                spRow = null;
                selectIndex = 0;
            }
            public string Title { get; set; }
            public string fuc { get; set; }
            public DataRow spRow { get; set; }
            public int selectIndex { get; set; }

        }
        public CollactorManager(string para)
        {

        }
        /// <summary>
        ///   Loadheader
        /// </summary>
        private void LoadHeader()
        {
            this.dataGridView1.Columns["Device_ID"].HeaderText = "采集点编号";
            this.dataGridView1.Columns["Device_ID"].Width = 120;
            this.dataGridView1.Columns["Device_PassWord"].HeaderText = "采集点密码";
            this.dataGridView1.Columns["Device_PassWord"].Width = 120;
            this.dataGridView1.Columns["Device_Code"].HeaderText = "采集点接入码";
            this.dataGridView1.Columns["Device_Code"].Width = 120;
            this.dataGridView1.Columns["Device_Name"].HeaderText = "采集点名称";
            this.dataGridView1.Columns["Device_Name"].Width = 120;
            this.dataGridView1.Columns["Device_Addr"].HeaderText = "采集点地址";
            this.dataGridView1.Columns["Device_Addr"].Width = 120;
            this.dataGridView1.Columns["Device_Type"].HeaderText = "采集点型号";
            this.dataGridView1.Columns["Device_Type"].Width = 120;
            this.dataGridView1.Columns["Device_Ver"].HeaderText = "采集点版本";
            this.dataGridView1.Columns["Device_Ver"].Width = 120;
        }

        public void Device_Load()
        {
            databaseName = dbMySql.GetDatabaseName();//

            DataSet dataSet = new DataSet();
            dataSet.Clear();
            string sql = "SELECT Device_ID,Device_PassWord,Device_Code, Device_Name,Device_Addr,Device_Type,Device_Ver From device";
            int ret = database.ReadDataBase(sql, "device", dataSet);

            if (ret == 0)
            {
                System.Data.DataTable dtInfo = new System.Data.DataTable();
                this.dataGridView1.DataSource = dataSet.Tables["device"];
            }
            LoadHeader();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            Device_Load();
        }
        //得到DataGridView当前行(按照个人习惯，将实际的行号从零开始)：
        public int getRow()
        {
            if (this.dataGridView1.Rows.Count > 0 && this.dataGridView1.CurrentRow.Index >= 0)
                return this.dataGridView1.CurrentRow.Index;
            else
                return 0;
        }
        //删除DataGridView指定行(传入的row是实际行号加1，还是个人习惯)：
        public void deleteRow(int row)
        {
            if (row < 0 || row >= this.dataGridView1.Rows.Count)
                return;
            //用DataGridViewrow的DataBoundItem属性得到当前绑定的原始数据行，是一个DataRowview对象
            //再用这个对象得到对应的DataRow
            //(this.dataGridView1.Rows[row].Cells[0].Value as DataRowView).Row.Delete();
            //  (dataGridView1.Rows[row].DataBoundItem as DataRowView).Row.Delete();
            this.dataGridView1.Rows.Remove(this.dataGridView1.Rows[row]);
        }
    
        int rowNum = 0;
        private void button_del_Click(object sender, EventArgs e)
        {
            rowNum = getRow();//获取datagriadview行号
            string nodeName = dataGridView1.Rows[rowNum].Cells["Device_Name"].Value.ToString();
            if (dataGridView1.CurrentCell.RowIndex >= 0)
            {
                if (MessageBox.Show("你确定要删除吗？", "确定", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string sql = "DELETE FROM device  WHERE Device_ID = ";
                    sql += "'" + dataGridView1.Rows[rowNum].Cells[0].Value.ToString() + "'";

                    dbMySql.Open(databaseName);
                    dbMySql.ExcuteNonQuery(databaseName, sql);
                    dbMySql.Close(databaseName);
                    deleteRow(rowNum);
                    dataGridView1.Update();

                    
                    TreeExXML.TreeExXMLCls xt2 = new TreeExXML.TreeExXMLCls();
                    xt2.DeleteXmlNodeByXPath("D:\\MyVidio\\myxml.xml", null, nodeName);

                   
                    fath.ReloadTreeView();
                }
                else
                {

                    MessageBox.Show("没有选中的记录，请选择!");

                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_Update_Click(object sender, EventArgs e)
        {
            AlarmParameter temp = new AlarmParameter();
            temp.fuc = "update";
            temp.Title = "update";

            temp.selectIndex = dataGridView1.CurrentCell.RowIndex;
            string nodeName = dataGridView1.Rows[temp.selectIndex].Cells["Device_Name"].Value.ToString();
            AddCollection add = new AddCollection(temp, this);
            add.Owner = this;
            if (add.ShowDialog() == DialogResult.OK)
            {
                if (add.newDeviceName != nodeName)
                {
                    TreeExXML.TreeExXMLCls xt2 = new TreeExXML.TreeExXMLCls();
                    xt2.UpdateXmlNodeByXPath("D:\\MyVidio\\myxml.xml", add.newDeviceName, nodeName);
                    fath.ReloadTreeView();
                }
                
            }
   
        }
    }
}
