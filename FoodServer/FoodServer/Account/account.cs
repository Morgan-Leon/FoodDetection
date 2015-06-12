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

namespace FoodServer.Account
{
    public partial class account : Form
    {
        MySqlDataBase dbMySql = MySqlDataBase.getInstance();
        string databaseName;
        IDataBase database = MySqlDataBase.getInstance();
        MySqlDataAdapter myDataAdpter = new MySqlDataAdapter();
        DataSet myDataSet = new DataSet();
        public account()
        {
            InitializeComponent();
        }
        /// <summary>
        ///   
        /// </summary>
        public System.Data.DataTable LoadUser()
        {
            //database.Open();
            DataSet dataset = new DataSet();
            dataset.Clear();
            string sql = "SELECT * FROM user";
            int ret = database.ReadDataBase(sql, "user", dataset);
            //database.Close();
            if (ret == 0)
            {
                System.Data.DataTable table = new System.Data.DataTable();
                table = dataset.Tables["user"];
                return table;
            }
            
            return null;
        }
        private void LoadUserHeader()
        {  
            this.dataGridView1.Columns["User_ID"].HeaderText = "账号";

            this.dataGridView1.Columns["PassWord"].HeaderText = "密码";
            this.dataGridView1.Columns["City"].HeaderText = "区域";
            this.dataGridView1.Columns["Company"].HeaderText = "公司";
            this.dataGridView1.Columns["Mobile"].HeaderText = "电话";
            this.dataGridView1.Columns["Is_Admin"].Visible = false;
            this.dataGridView1.Columns["Desc"].Visible = false;
            this.dataGridView1.Columns["User_Name"].HeaderText = "用户名";
            this.dataGridView1.Columns["Email"].HeaderText = "邮件";
            this.dataGridView1.Columns["CenterCode"].HeaderText = "区接入码域";
        
        }

      public  void re_Load()
      {
          
          DataTable table = LoadUser();
          this.dataGridView1.DataSource = table;
          dataGridView1.Refresh();
          LoadUserHeader();
      }
        /// <summary>
        ///   数据初始化
        /// </summary>
        
        public void account_Load(object sender, EventArgs e)
        {
           
            databaseName = dbMySql.GetDatabaseName();
            DataTable table = LoadUser();
            this.dataGridView1.DataSource = table;
            LoadUserHeader();
           

        }
        /// <summary>
        ///   添加数据
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            FoodServer.checkedUnit.CheckedUnit.AlarmParameter temp = new FoodServer.checkedUnit.CheckedUnit.AlarmParameter();
            temp.fuc = "insert";
            temp.Title = "insert";
            UserInfo add = new UserInfo(temp, this);
            add.Owner = this;
            add.Show();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        ///   修改数据
        /// </summary>

        private void button2_Click(object sender, EventArgs e)
        {
          
            FoodServer.checkedUnit.CheckedUnit.AlarmParameter temp = new FoodServer.checkedUnit.CheckedUnit.AlarmParameter();
            temp.fuc = "update";
            temp.Title = "update";
            try
            {
                temp.selectIndex = dataGridView1.CurrentCell.RowIndex;
                UserInfo add = new UserInfo(temp, this);
                add.Owner = this;
                add.Show();
            }
            catch
            {
                MessageBox.Show("请选择要修改的行数据");
            }
        }
        /// <summary>
        ///   删除数据
        /// </summary>
        private void Button__Del_Click(object sender, EventArgs e)
        {

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
        
        string id;
        private void button_del_Click(object sender, EventArgs e)
        {
            int rowNum = getRow();
            if (dataGridView1.CurrentCell.RowIndex >= 0)
            {
                if (MessageBox.Show("你确定要删除吗？", "确定", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string sql = "DELETE FROM user  WHERE User_ID = '" + id + "' ";
                   
                    dbMySql.Open();
                    dbMySql.ExcuteNonQuery(databaseName, sql);
                    dbMySql.Close();
                    deleteRow(rowNum);
                    dataGridView1.Update();
                }
//                 else
//                 {
// 
//                     MessageBox.Show("没有选中的记录，请选择!");
// 
//                 }
            }
        }

        private void DateCellClick(object sender, DataGridViewCellEventArgs e)
        {
           // OldRow = dataGridView1.RowCount;
            //获得当前选中的行 
            int i = e.RowIndex;
            if(i >= 0)
            id = dataGridView1.Rows[i].Cells[0].Value.ToString();
        }
    }
}
