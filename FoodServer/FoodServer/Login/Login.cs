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

namespace FoodServer.Login
{
    public partial class Login : Form
    {
        MySqlDataBase dbMySql = MySqlDataBase.getInstance();
        string databaseName;
        IDataBase database = MySqlDataBase.getInstance();
        string m_name;
        string m_pass;

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
        public Login()
        {
            InitializeComponent();
           // this.FormBorderStyle = FormBorderStyle.None;
            databaseName = dbMySql.GetDatabaseName();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //登录
        int i = 3;
        private void button1_Click(object sender, EventArgs e)
        {
            if (i == 0)
            {
                CloseDataBase();
                this.Close();
            }
            CreateConnection();
            OpenDataBase();
            try
            {
                m_name = this.textBox_name.Text.Trim();
                m_pass = this.textBox_Password.Text.Trim();
                DataSet dataSet = new DataSet();
                string sql = "SELECT * from `user` WHERE User_ID = '" + m_name + "' AND `PassWord` = '" + m_pass + "'";
                int ret = database.ReadDataBase(sql, "user", dataSet);
                if (ret == 0)
                {
                    if (dataSet.Tables["user"].Rows.Count == 1)
                    {
                        // this.Close();
                         MainForm mainform = new MainForm();
                         mainform.Show();
                         this.Visible = false;
                         i = 0;
                        
                    }
                    else
                    {
                        i--;
                        CloseDataBase();
                        MessageBox.Show("密码或者用户名错误！还有"+i+"次机会");
                        this.textBox_name.Text = "";
                        this.textBox_Password.Text = "";
                    }
                    
                }
            }
            catch (System.Exception ex)
            {
            	CloseDataBase();
                MessageBox.Show("网络故障，稍后再试！");
            }
          
        }
    }
}
