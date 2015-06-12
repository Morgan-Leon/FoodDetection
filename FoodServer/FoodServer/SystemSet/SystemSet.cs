using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GHCS;
using GHCS.DataBase;
using MySql.Data.MySqlClient;
using FoodServer.CheckPro;

namespace FoodServer.SystemSet
{
    public partial class SystemSet : Form
    {
        MySqlDataBase dbMySql = MySqlDataBase.getInstance();
        string databaseName;
        IDataBase database = MySqlDataBase.getInstance();
        MySqlDataAdapter myDataAdapter = new MySqlDataAdapter();//定义一个数据适配器
        DataSet myDataSet = new DataSet();

      
        IniAc ini = new IniAc();
       
        public SystemSet()
        {
            InitializeComponent();
            databaseName = dbMySql.GetDatabaseName();

//             SetYellow = (double.Parse(ini.ReadValue("NET", "yellow")) / 100.0);
//             SetFense = (double.Parse(ini.ReadValue("NET", "fense")) / 100.0);
//             SetRed = (double.Parse(ini.ReadValue("NET", "red")) / 100.0);
        }
        private void LoadBar()
        {
            //设置滑块属性
            //滑块一
        /*    trackBar_Yellow.Minimum = 0;
            trackBar_Yellow.Maximum = 100;
            trackBar_Yellow.TickFrequency = 5;
            trackBar_Yellow.SmallChange = 5;
            trackBar_Yellow.LargeChange = 20;
            //滑块二
            trackBar_fense.Minimum = 0;
            trackBar_fense.Maximum = 100;
            trackBar_fense.TickFrequency = 10;
            trackBar_fense.SmallChange = 5;
            trackBar_fense.LargeChange = 20;
            //滑块三
            trackBar_red.Minimum = 0;
            trackBar_red.Maximum = 100;
            trackBar_red.TickFrequency = 5;
            trackBar_red.SmallChange = 5;
            trackBar_red.LargeChange = 20;
         * */
        }

    

        private void Form_Load(object sender, EventArgs e)
        {
            string ipAddrStr = "127.0.0.1";
           
            ipAddrStr = ini.ReadValue("NET", "ip");
            string portStr = ini.ReadValue("NET", "port");
            this.textBoxIP.Text = ipAddrStr;
            this.textBoxcode.Text = portStr;
         //
           
          //  LoadBar();
        }

        private void Track1_Scroll(object sender, EventArgs e)
        {
           // label_Yellow.Text = trackBar_Yellow.Value.ToString()+"%";
        }

        private void Track2_Scroll(object sender, EventArgs e)
        {
           // label_fense.Text = trackBar_fense.Value.ToString() + "%";
        }

        private void Track3_Scroll(object sender, EventArgs e)
        {
          //  label_red.Text = trackBar_red.Value.ToString() + "%";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            IniAc ini = new IniAc();
            string ip = this.textBoxIP.Text;
            string port = this.textBoxcode.Text;
         
            ini.WritValue("NET", "ip", ip);
            ini.WritValue("NET", "port", port);
          
            this.Close();

           

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }



    }
}
