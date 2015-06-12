using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GHCS.DataBase;
using MySql.Data.MySqlClient;
using GHCS;

namespace FoodServer.checkedUnit
{
    public partial class CheckedUnit : Form
    {
        MySqlDataBase dbMySql = MySqlDataBase.getInstance();
        string databaseName;
        IDataBase database = MySqlDataBase.getInstance();
        MySqlDataAdapter myDataAdapter = new MySqlDataAdapter();//定义一个数据适配器
        DataSet myDataSet = new DataSet();

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

        public CheckedUnit()
        {
            databaseName = dbMySql.GetDatabaseName();
            InitializeComponent();
        }

        private void button_Add_Click(object sender, EventArgs e)
        {
            AlarmParameter temp = new AlarmParameter();
            EditChecked add = new EditChecked(temp,this);
            add.Owner = this;
            add.Show();
        }
        public void LoadHeader()
        {
            this.dataGridView1.Columns["Company_ID"].Visible = false;

            this.dataGridView1.Columns["Company_Name"].HeaderText = "单位名称";
            this.dataGridView1.Columns["Company_Name"].Width = 120;
            this.dataGridView1.Columns["Company_Tel"].HeaderText = "单位电话";
            this.dataGridView1.Columns["Company_Tel"].Width = 120;
            this.dataGridView1.Columns["Company_Email"].HeaderText = "单位Email";
            this.dataGridView1.Columns["Company_Email"].Width = 120;
            this.dataGridView1.Columns["Company_Addr"].HeaderText = "单位地址";
            this.dataGridView1.Columns["Company_Addr"].Width = 120;
            this.dataGridView1.Columns["Company_Class"].HeaderText = "单位行业";
            this.dataGridView1.Columns["Company_Class"].Width = 120;
        }
        public void Unit_Load()
        {
           // databaseName = dbMySql.GetDatabaseName();//
           // database.Open();
            DataSet dataSet = new DataSet();
            dataSet.Clear();
            string sql = "SELECT Company_ID,Company_Name, Company_Tel,Company_Email,Company_Addr,Company_Class From companyinfo";
            int ret = database.ReadDataBase(sql, "company", dataSet);
            //database.Close();
            if (ret == 0)
            {
                System.Data.DataTable dtInfo = new System.Data.DataTable();
                this.dataGridView1.DataSource = dataSet.Tables["company"];
            }
            LoadHeader();
        }
        private void CheckedUnit_Load(object sender, EventArgs e)
        {
            Unit_Load();
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            this.Close();
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
        private void button_Del_Click(object sender, EventArgs e)
        {
            rowNum = getRow();//获取datagriadview行号
            if (dataGridView1.CurrentCell.RowIndex >= 0)
            {
                if (MessageBox.Show("你确定要删除吗？", "确定", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string sql = "DELETE FROM companyinfo  WHERE Company_ID = ";
                    sql += "'" + dataGridView1.Rows[rowNum].Cells[0].Value.ToString() + "'";

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

        private void button_Update_Click(object sender, EventArgs e)
        {
            AlarmParameter temp = new AlarmParameter();
            temp.fuc = "update";
            temp.Title = "update";

            temp.selectIndex = dataGridView1.CurrentCell.RowIndex;
            EditChecked add = new EditChecked(temp, this);
            add.Owner = this;
            add.Show();
        }

    }
}
