using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GHCS.Common;
using GHCS;
using GHCS.DataBase;
using MySql.Data.MySqlClient;
using FoodServer.Class;


namespace FoodServer.CheckPro
{
    public partial class ChekPro : Form
    {
        MySqlDataBase dbMySql = MySqlDataBase.getInstance();
        string databaseName;
       
        MySqlDataAdapter myDataAdapter = new MySqlDataAdapter();//定义一个数据适配器
        DataSet myDataSet = new DataSet();
        //创建数据库的对象
        IDataBase database = MySqlDataBase.getInstance();



        //id索引标记
        string tinfoid = null;
        public ChekPro()
        {
            InitializeComponent();
            databaseName = dbMySql.GetDatabaseName();
        }


        //表格头
        private void LoadConHeader()
        {
            this.dataGridView1.Columns[0].Visible = false;
            this.dataGridView1.Columns[1].HeaderText = "检测项目";
            this.dataGridView1.Columns[2].HeaderText = "样品名称";
            this.dataGridView1.Columns[3].HeaderText = "模式";
            this.dataGridView1.Columns[4].HeaderText = "主波";
            this.dataGridView1.Columns[5].HeaderText = "次波";
            this.dataGridView1.Columns[6].HeaderText = "公式C ";
            this.dataGridView1.Columns[7].HeaderText = "公式B ";
            this.dataGridView1.Columns[8].HeaderText = "公式A ";
            this.dataGridView1.Columns[9].HeaderText = "稀释倍数 ";
            this.dataGridView1.Columns[10].HeaderText = "单位 ";
            this.dataGridView1.Columns[11].HeaderText = "参考标准 ";
            this.dataGridView1.Columns[12].HeaderText = "比较方式";
            this.dataGridView1.Columns[13].HeaderText = "比较最小值 ";
            this.dataGridView1.Columns[14].HeaderText = "比较最大值 ";
            this.dataGridView1.Columns[15].HeaderText = "测量范围最大值";
            this.dataGridView1.Columns[16].HeaderText = "测量范围最小值";

            this.dataGridView1.Columns[17].HeaderText = "红色报警";
            this.dataGridView1.Columns[18].HeaderText = "粉色报警";
            this.dataGridView1.Columns[19].HeaderText = "黄色报警";
        }

        //载入表格数据
        public void LoadConfigInfo()
        {
            DataSet dataset = new DataSet();
            dataset.Clear();

            string str = " SELECT * FROM ftinfor ";
            int ret = database.ReadDataBase(str, "ftinfo", dataset);
            if (ret == 0)
            {
                this.dataGridView1.DataSource = dataset.Tables["ftinfo"];
            }
            LoadConHeader();
        }

        void ComBox_Init()
        {
            LoadConfigInfo();


        }
        int OldRow;
        private void button1_Click(object sender, EventArgs e)
        {

            FoodServer.checkedUnit.CheckedUnit.AlarmParameter temp = new FoodServer.checkedUnit.CheckedUnit.AlarmParameter();
            temp.fuc = "update";
            temp.Title = "update";
            try
            {
                temp.selectIndex = dataGridView1.CurrentCell.RowIndex;
                AddCheck add = new AddCheck(temp, this);
                add.Owner = this;
                add.Show();
            }
            catch
            {
                MessageBox.Show("请选择要修改的行数据");
            }
           

        }

        private void Form_Load(object sender, EventArgs e)
        {
            ComBox_Init();
            
        }
        List<string> list = new List<string>();
        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            list.Clear();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ProSelectchange(object sender, EventArgs e)
        {
        }

        private void button_ok_Click(object sender, EventArgs e)
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
            this.dataGridView1.Rows.Remove(this.dataGridView1.Rows[row]);
        }
        private void button_Del_Click(object sender, EventArgs e)
        {
            int rowNum = getRow();
            if (dataGridView1.CurrentCell.RowIndex >= 0)
            {
                if (MessageBox.Show("你确定要删除吗？", "确定", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string sql = "DELETE FROM ftinfor  WHERE id = '" + tinfoid + "' ";

                    dbMySql.Open(databaseName);
                    dbMySql.ExcuteNonQuery(databaseName, sql);
                    dbMySql.Close(databaseName);
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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            OldRow = dataGridView1.RowCount;
            //获得当前选中的行 
            int i = e.RowIndex;
            tinfoid = dataGridView1.Rows[i].Cells[0].Value.ToString();
        }

        private void button_Add_Click(object sender, EventArgs e)
        {

            FoodServer.checkedUnit.CheckedUnit.AlarmParameter temp = new FoodServer.checkedUnit.CheckedUnit.AlarmParameter();
            AddCheck add = new AddCheck(temp, this);
            add.Owner = this;
            add.Show();
        }
    }
}
