using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;



//数据库连接
namespace GHCS
{
    class DBConn
    {
        private static DBConn dbconn;


        private MySqlConnection conn;
        private string dbserver = "127.0.0.1";
        private string dbuser = "root";
        private string dbpwd = "pwd";
        private string database = "ghcs";
        private string dbport = "3306";


        private DBConn()
        {
            loadIni();

        }
        /***
         * 加载配置文件
         * **/
        private void loadIni()
        {
            IniAc ini=new IniAc();
            dbserver=ini.ReadValue("DBMS","server");
            dbuser=ini.ReadValue("DBMS","user");
            dbpwd=ini.ReadValue("DBMS", "pwd");
           database=ini.ReadValue("DBMS","database");
           dbport = ini.ReadValue("DBMS","port");
        
        
        }


        public static DBConn getDBCInstance()
        {

            if (null == dbconn)
                dbconn = new DBConn();
            return dbconn;

        }



        public MySqlConnection getConnection()
        {



            string myConnectionString;
            //拼连接串
            myConnectionString = "server=" +dbserver+";Port="+dbport+";uid="+dbuser+";pwd="+dbpwd+";database="+database+";";

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                switch (ex.Number)
                {
                       
                    case 0:
                        System.Windows.Forms.MessageBox.Show("无法连接数据库服务器","错误");
                        break;

                     //MessageBox.Show("Cannot connect to server.  Contact administrator");
                    case 1045:
                       System.Windows.Forms.MessageBox.Show("用户名或密码无效","错误");
                       //System.Windows.Forms.MessageBox.Show("Invalid username/password, please try again");
                    break;    
                 


                }
                return null;

            }


            return conn;

        }
    }
}
