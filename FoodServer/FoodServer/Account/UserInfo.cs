using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using GHCS.DataBase;
using GHCS;
using MySql.Data.MySqlClient;
using FoodServer.checkedUnit;

namespace FoodServer.Account
{
    public partial class UserInfo : Form
    {
        private string strOwnParam;
        private account UserFatherWindow;
        private DataRow UserRow;
        private DataTable dtUser;
        MySqlDataBase dbMySql = MySqlDataBase.getInstance();
        string databaseName;
        IDataBase database = MySqlDataBase.getInstance();
        MySqlDataAdapter myDataAdapter = new MySqlDataAdapter();//定义一个数据适配器
        DataSet myDataSet = new DataSet();
        CheckedUnit.AlarmParameter parameter = new CheckedUnit.AlarmParameter();
        public UserInfo()
        {
            InitializeComponent();
        }

         account parent;
         public UserInfo(CheckedUnit.AlarmParameter par, account cu)
        {
            InitializeComponent();
            databaseName = dbMySql.GetDatabaseName();
            parameter = par;
            strOwnParam = par.fuc;
            parent = cu;
        }
        public UserInfo(string Para)
        {
            strOwnParam = Para;
           
            InitializeComponent();
        }
       public string strsql;
        private void button_OK_Click(object sender, EventArgs e)
        {
            int id;
            string password;
            string city;
            string mobile;
            string centercode;
            string email;
            string company;
            int j = parent.dataGridView1.CurrentCell.RowIndex;
            string tinfoid = parent.dataGridView1.Rows[j].Cells[0].Value.ToString();
            if (parameter.fuc == "update")
            {
              //  ShowAll();
                id = int.Parse(this.textBox_Account.Text.Trim());
                password = this.textBox_Code.Text.Trim();
                city = this.textBox_Address.Text.Trim();
                mobile = this.textBox_cell.Text.Trim();
                email = this.textBox_Email.Text.Trim();
                centercode = this.textBox_CeenterCode.Text.Trim();
                company = this.textBox_Company.Text.Trim();
                strsql = " UPDATE user set " +
                           " PassWord = '" + password + "'," +
                           " City= '" + city + "'," +
                           "  Mobile= '" + mobile + "'," +
                           " Email= '" + email + "'," +
                           " CenterCode= '" + centercode + "'," +
                           " Company= '" + company + "'" +
                           " WHERE User_ID = '" + tinfoid + "'";
            }
            else
            {
                if (string.IsNullOrEmpty(textBox_Account.Text.Trim()))
                {
                    MessageBox.Show("请输入用户账户！");
                    return;
                }
                else
                {
                    DataSet dataSet = new DataSet();
                    string sql = "SELECT * FROM user";
                    int ret = database.ReadDataBase(sql, "companyinfo", dataSet);

                    if (ret == 0)
                    {
                        foreach (DataRow row in dataSet.Tables["companyinfo"].Rows)
                        {
                            if (row["User_ID"].ToString() == textBox_Account.Text.Trim().ToString())
                            {
                                MessageBox.Show("该账户已经存在，请重新输入！");
                                return;
                            }
                        }
                    }
                }

                // FoodServer.MainForm.m_userId = User_Name;
                id = int.Parse(this.textBox_Account.Text.Trim());
                password = this.textBox_Code.Text.Trim();
                city = this.textBox_Address.Text.Trim();
                mobile = this.textBox_cell.Text.Trim();
                email = this.textBox_Email.Text.Trim();
                centercode = this.textBox_CeenterCode.Text.Trim();
                company = this.textBox_Company.Text.Trim();


                if (DataToRow())
                {
                    strsql = "Insert Into " +
                    "user(User_ID,PassWord,City,Mobile,Email,CenterCode,Company) "
                    + "values(" + id + ",'" + password + "','" + city + "','" + mobile + "','" + email + "','" + centercode + "','" + company + "')";
                }
             
                
            }
            dbMySql.Open();
            dbMySql.ExcuteNonQuery(databaseName, strsql);
            dbMySql.Close();
            parent.dataGridView1.Refresh();
            parent.re_Load();
            this.Close();
        }

        private void UserInfo_Load(object sender, EventArgs e)
        {
            //databaseName = dbMySql.GetDatabaseName();//
            UserFatherWindow = (account)this.Owner;

            DataTable table = UserFatherWindow.LoadUser();
            dtUser = table;
            if (strOwnParam == "insert")
            {
                UserRow = dtUser.NewRow();

            }
            else
            {
                UserRow = (UserFatherWindow.dataGridView1.CurrentRow.DataBoundItem as DataRowView).Row as DataRow;
                ShowAll();
            }
        }

        private void ShowAll()
        {
            textBox_Account.Text = UserRow["User_ID"].ToString();
            textBox_Code.Text = UserRow["PassWord"].ToString();
            textBox_Address.Text = UserRow["City"].ToString();
            textBox_cell.Text = UserRow["Mobile"].ToString();
         
            textBox_Email.Text = UserRow["Email"].ToString();
            textBox_CeenterCode.Text = UserRow["CenterCode"].ToString();
            textBox_Company.Text = UserRow["Company"].ToString();

       
        }


        private bool DataToRow()
        {
            string strAccount = textBox_Account.Text.Trim();
            bool AccountComplict = false;
            foreach (DataRow r in UserFatherWindow.LoadUser().Rows)
            {
                if (r["User_ID"].ToString() == strAccount && r["User_ID"].ToString() != UserRow["User_ID"].ToString())
                {
                    AccountComplict = true;
                    break;
                }
            }
            if (AccountComplict == true)
            {
                MessageBox.Show("有重复的帐号，请换一个帐号名!");
                return false;
            }
            UserRow["User_ID"] = int.Parse(strAccount);
            UserRow["PassWord"] = textBox_Code.Text.Trim();
            UserRow["City"] = textBox_Address.Text.Trim();
            UserRow["Mobile"] = textBox_cell.Text.Trim();
            UserRow["Email"] = textBox_Email.Text.Trim();
            UserRow["CenterCode"] = textBox_CeenterCode.Text.Trim();
            UserRow["Company"] = textBox_Company.Text.Trim();


            return true;
        }

        private void button_Canncel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
