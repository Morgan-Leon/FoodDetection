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
namespace FoodServer.checkedUnit
{
    public partial class EditChecked : Form
    {
        MySqlDataBase dbMySql = MySqlDataBase.getInstance();
        string databaseName;
        IDataBase database = MySqlDataBase.getInstance();
        MySqlDataAdapter myDataAdapter = new MySqlDataAdapter();//定义一个数据适配器
        DataSet myDataSet = new DataSet();
        CheckedUnit.AlarmParameter parameter = new CheckedUnit.AlarmParameter();
        int  IDNum = 0;
        public EditChecked()
        {
            InitializeComponent();
            databaseName = dbMySql.GetDatabaseName();
        }
        CheckedUnit parent;
        public EditChecked(CheckedUnit.AlarmParameter par, CheckedUnit cu )
        {
            InitializeComponent();
            databaseName = dbMySql.GetDatabaseName();
            parameter = par;
            parent = cu;
        }
        
        private void button_OK_Click(object sender, EventArgs e)
        {
            if (textBox_Name.Text == "")
            {
                MessageBox.Show("请输入送检单位名称！");
                return;
            }
            string strsql;
            if (parameter.fuc == "update")
            {
                  strsql = "Update companyinfo SET "
                   + "Company_Name = '" + textBox_Name.Text.ToString() + "',"
                   + "Company_Addr = '" + textBox_Addre.Text.ToString() + "',"
                   + "Company_Email = '" + textBox_Email.Text.ToString() + "',"
                   + "Company_Class = '" + textBox_Class.Text.ToString() + "',"
                   + "Company_Tel = '" + textBox_Tel.Text.ToString() + "'"
                   + "WHERE Company_ID = " + IDNum + "";

 
            }
            else
            {
                 if (string.IsNullOrEmpty(textBox_Name.Text.Trim()))
                {
                    MessageBox.Show("请输入终端名称！");
                    return;
                }
                else
                {
                    DataSet dataSet = new DataSet();
                    string sql = "SELECT * FROM companyinfo";
                    int ret = database.ReadDataBase(sql, "companyinfo", dataSet);
                    
                    if (ret == 0)
                    {
                        foreach (DataRow row in dataSet.Tables["companyinfo"].Rows)
                        {
                            if (row["Company_Name"].ToString() == textBox_Name.Text.Trim().ToString())
                            {
                                MessageBox.Show("该送检单位已经存在，请重新输入送检单位！");
                                return;
                            }
                        }
                    }
                }

                // FoodServer.MainForm.m_userId = User_Name;
                string Name  = textBox_Name.Text.Trim().ToString();
                string Addr  = textBox_Addre.Text.Trim().ToString();
                string email = textBox_Email.Text.Trim().ToString();
                string Tel = textBox_Tel.Text.Trim().ToString();
                string Clas= textBox_Class.Text.Trim().ToString();
           
                strsql = "Insert Into " +
                    "companyinfo(Company_Name,Company_Addr,Company_Tel,Company_Email,Company_Class)"
                    + "values('" + Name + "','" + Addr + "','" + Tel + "','" + email + "','" + Clas + "')";
              
            }
            dbMySql.Open(databaseName);
            dbMySql.ExcuteNonQuery(databaseName, strsql);
            dbMySql.Close(databaseName);
            this.Close();
            parent.Unit_Load();
            //MessageBox.Show("修改成功");
        }

        private void EditChecked_Load(object sender, EventArgs e)
        {
            
            if (parameter.fuc == "update")
            {
                int rowNum = parent.getRow();//获取datagriadview行号
                //DataGridViewRow r = parent.dataGridView1.Rows[rowNum];
                this.textBox_Name.Text = parent.dataGridView1.Rows[rowNum].Cells["Company_Name"].Value.ToString();
                this.textBox_Addre.Text = parent.dataGridView1.Rows[rowNum].Cells["Company_Addr"].Value.ToString();
                this.textBox_Email.Text = parent.dataGridView1.Rows[rowNum].Cells["Company_Email"].Value.ToString();
                this.textBox_Tel.Text = parent.dataGridView1.Rows[rowNum].Cells["Company_Tel"].Value.ToString();
                this.textBox_Class.Text = parent.dataGridView1.Rows[rowNum].Cells["Company_Class"].Value.ToString();
                IDNum = int.Parse(parent.dataGridView1.Rows[rowNum].Cells["Company_ID"].Value.ToString());
            }
        }

        private void button_Canncel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

   /// <summary>
   ///   加载头
   /// </summary>

  
    }
}
