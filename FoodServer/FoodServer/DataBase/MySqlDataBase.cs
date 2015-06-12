using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MySql.Data;
using MySql.Data.MySqlClient;

/***
 * 
 * 2012.7.29  增加数据库服务端口　Pan
 * **/
namespace GHCS.DataBase
{
    class MySqlDataBase:IDataBase
    {
        #region "COMMON"
        private static MySqlDataBase dbconn; 

        private MySqlConnection connect = null; // 数据库连接
        private MySqlConnection connectInfo = null; //数据库information_schema连接
        private MySqlDataReader data = null;    // 读取的数据记录
        private MySqlDataReader dataInfo = null;
        private int DataBaseError = 0;      // 0表示数据库正常，1表示异常 
        //MySqlDataAdapter adapter = null; //读取数据的适配器

        private static object lockobj = new object();
       

        private string dbserver = "127.0.0.1";
        private string dbuser = "root";
        private string dbpwd = "pwd";
        private string database = "food";
        private string info = "information_schema";
        private string dbport = "3306";

        /// <summary>
        /// 
        /// </summary>
        private MySqlDataBase()
        {
            LoadIni();
        }

        /// <summary>
        /// 加载ini文件中的服务器信息
        /// </summary>
        private void LoadIni()
        {
            IniAc ini = new IniAc();
            dbserver = ini.ReadValue("DBMS", "server");
            dbuser = ini.ReadValue("DBMS", "user");
            dbpwd = ini.ReadValue("DBMS", "pwd");
            database = ini.ReadValue("DBMS", "database");
            info = ini.ReadValue("DBMS", "info");
            dbport = ini.ReadValue("DBMS", "port");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static MySqlDataBase getInstance()
        {
            if (null == dbconn)
                dbconn = new MySqlDataBase();
            return dbconn;

        }
        #endregion

        #region "原函数"
        /// <summary>
        /// 创建数据库连接
        /// </summary>
        /// <param name="connectString"></param>
        /// <returns></returns>
        public int CreateConnection(string connectString)
        {
            if (connectString == null || connectString.CompareTo("") == 0)
            {
                
                connectString = "Server=";
                connectString += dbserver;
                connectString += ";";
                connectString += "Port=";

               // connectString += ":";
                connectString += dbport;
                connectString += ";UserId=";
                connectString += dbuser;
                connectString += ";Password=";
                connectString += dbpwd;
                connectString += ";Database=";
                connectString += database;

                //connectString += ";";
                //connectString += "MultipleActiveResultSets=true";

            }            

            try
            {
                if (connect == null)
                {
                    // 连接到数据库
                    connect = new MySqlConnection(connectString);
                }                
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                DataBaseError = 1;

                // 错误信息，需要写入到日志
                const string caption = "数据库异常";

                MessageBox.Show(ex.Message, caption,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return -1;
            }

            return 0;
        }



        /// <summary>
        /// 打开数据库，成功返回0
        /// </summary>
        /// <returns></returns>
        public int Open()
        {
            if (connect.State == ConnectionState.Open)
            {
                return 0;
            }

            try
            {
                connect.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                DataBaseError = 1;

                // 错误信息，需要写入到日志
                const string caption = "数据库异常";

                MessageBox.Show(ex.Message, caption,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return -1;
            }

            return 0;
        }

 
        /// <summary>
        /// 关闭数据库
        /// </summary>
        public int Close()
        {
            if (connect.State == ConnectionState.Open)
            {
                connect.Close();
            }

            return 0;
        }

        /// <summary>
        /// 根据sql语句从数据库中读取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="srcTable"></param>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public int ReadDataBase(string sql, string srcTable, DataSet dataSet)
        {
            lock (lockobj)
            {

                if (DataBaseError == 1)
                {
                    return -1;
                }

                dataSet.Clear();


                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                try
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sql, connect);

                    adapter.Fill(dataSet, srcTable);
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    DataBaseError = 1;

                    // 错误信息，需要写入到日志
                    const string caption = "数据库异常";

                    MessageBox.Show(ex.Message, caption,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return -1;
                }


                return 0;
            }

        }

        /// <summary>
        /// 关闭读取数据的适配器
        /// </summary>
        //public void CloseDataAdapter()
        //{
        //    adapter.Dispose();
        //}

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExcuteNonQuery(string sql)
        {
            lock (lockobj)
            {
                if (DataBaseError == 1)
                {
                    return -2;
                }

                int ret = 0;
                try
                {
                    MySqlCommand command = new MySqlCommand(sql, connect);

                    ret = command.ExecuteNonQuery();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    DataBaseError = 1;

                    // 错误信息，需要写入到日志
                    const string caption = "数据库异常";

                    MessageBox.Show(ex.Message, caption,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ret = -2;
                }
                return ret;
            }            
        }

        /// <summary>
        /// 执行sql语句，并将记录添加到datareader中
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteReader(string sql)
        {
            if (DataBaseError == 1)
            {
                return -1;
            }

            if (data != null && data.IsClosed == false)
            {
                CloseDataReader();
            }

            if (connect.State == ConnectionState.Closed)
            {
                connect.Open();
            }

            try
            {
                MySqlCommand command = new MySqlCommand(sql, connect);
                data = command.ExecuteReader();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                DataBaseError = 1;

                // 错误信息，需要写入到日志
                const string caption = "数据库异常";

                MessageBox.Show(ex.Message, caption,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return 0;
        }

        /// <summary>
        /// 读取一条记录并将记录的值放入到list中供用户使用
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="values"></param>
        /// <param name="num"></param>
        public int QueryTable(string sql,List<string> values,int columns)
        {
            int rows = 0;
            lock (lockobj)
            {
                ExecuteReader(sql);
                while (Read())
                {
                    rows++;
                    for (int i = 0; i < columns; i++)
                    {
                        values.Add(this.GetString(i));
                    }
                }

                CloseDataReader();
            }
            return rows;
        }
 
        #endregion

        #region "重载函数"
        /// <summary>
        /// 创建数据库连接，name为数据库名称
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connectString"></param>
        /// <returns></returns>
        public int CreateConnection(string name, string connectString)
        {
            string dbName;

            if (this.IsValidName(name) == false)
            {
                return -1;
            }
            else
            {
                dbName = name;
            }

            if (connectString == null || connectString.CompareTo("") == 0)
            {
                connectString = "Server=";
                connectString += dbserver;
                connectString += ";Port=";
                connectString += dbport;
                connectString += ";UserId=";
                connectString += dbuser;
                connectString += ";Password=";
                connectString += dbpwd;
                connectString += ";Database=";
                connectString += dbName;
            }
            try
            {
                // 连接到数据库
                if (name == this.GetDatabaseName())
                {
                    connect = new MySqlConnection(connectString);
                }
                else if (name == this.GetInfoName())
                {
                    connectInfo = new MySqlConnection(connectString);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                DataBaseError = 1;

                // 错误信息，需要写入到日志
                const string caption = "数据库异常";

                MessageBox.Show(ex.Message, caption,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return -1;
            }

            return 0;
        }

        /// <summary>
        /// 打开数据库,name为数据库名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int Open(string name)
        {
            if (this.IsValidName(name) == false)
            {
                return -1;
            }
            //MessageBox.Show(connectName.ToString());
            
            if (name == this.GetDatabaseName() && connect.State == ConnectionState.Open)
            {
                return 0;
            }
            
            if (name == this.GetInfoName() && connectInfo.State == ConnectionState.Open)
            {
                return 0;
            }
            
            try
            {
                if (name == this.GetDatabaseName())
                {
                    connect.Open();
                }
                else if (name == this.GetInfoName())
                {
                    connectInfo.Open();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                DataBaseError = 1;

                // 错误信息，需要写入到日志
                const string caption = "数据库异常";

                MessageBox.Show(ex.Message, caption,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return -1;
            }

            return 0;
        }
        
        /// <summary>
        /// 关闭数据库
        /// </summary>
        public int Close(string name)
        {
            if (this.IsValidName(name) == false)
            {
                return -1;
            }

            if (name == this.GetDatabaseName() && connect.State == ConnectionState.Open)
            {
                connect.Close();
            }
            else if (name == this.GetInfoName() && connectInfo.State == ConnectionState.Open)
            {
                connectInfo.Close();
            }
            return 0;
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExcuteNonQuery(string name, string sql)
        {
            if (this.IsValidName(name) == false)
            {
                return -1;
            }

            if (DataBaseError == 1)
            {
                return -2;
            }

            int ret = 0;
            try
            {
                MySqlCommand command = null;
                if (name == this.GetDatabaseName())
                {
                    command = new MySqlCommand(sql, connect);
                }
                else if (name == this.GetInfoName())
                {
                    command = new MySqlCommand(sql, connectInfo);
                }

                ret = command.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                DataBaseError = 1;

                // 错误信息，需要写入到日志
                const string caption = "数据库异常";

                MessageBox.Show(ex.Message, caption,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ret = -2;
            }

            return ret;
        }

        public int ExecuteReader(string name, string sql)
        {
            if (this.IsValidName(name) == false)
            {
                return -1;
            }

            if (DataBaseError == 1)
            {
                return -1;
            }

            try
            {
                MySqlCommand command = null;
                if (name == this.GetDatabaseName())
                {
                    command = new MySqlCommand(sql, connect);
                    data = command.ExecuteReader();
                }
                else if (name == this.GetInfoName())
                {
                    command = new MySqlCommand(sql, connectInfo);
                    dataInfo = command.ExecuteReader();
                }           

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                DataBaseError = 1;

                // 错误信息，需要写入到日志
                const string caption = "数据库异常";

                MessageBox.Show(ex.Message, caption,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return 0;
        }
         

        #endregion

        #region "其他"
        /// <summary>
        /// 是否有数据可读
        /// </summary>
        /// <returns></returns>
        public bool Read()
        {
            if (data.IsClosed == true)
            {
                return false;
            }
            return data.Read();
        }

        public bool Read(string name)
        {
            if (name == this.GetDatabaseName())
            {
                if (data.IsClosed == true)
                {
                    return false;
                }
                return data.Read();
            }
            else if (name == this.GetInfoName())
            {
                if (dataInfo.IsClosed == true)
                {
                    return false;
                }
                return dataInfo.Read();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 返回读取的某一列数据
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public string GetString(int column)
        {
            if(data.IsDBNull(column))
            {
                return "";
            }
            else
            {
                return data.GetString(column);
            }
        }

        public string GetString(string name, int column)
        {
            if (name == this.GetDatabaseName())
            {
                if (data.IsDBNull(column))
                {
                    return "";
                }
                else
                {
                    return data.GetString(column);
                }
            }
            else if (name == this.GetInfoName())
            {
                if (dataInfo.IsDBNull(column))
                {
                    return "";
                }
                else
                {
                    return dataInfo.GetString(column);
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 关闭datareader
        /// </summary>
        public void CloseDataReader()
        {
            if (data.IsClosed)
            {
                return;
            }
            data.Close();
        }

        public void CloseDataReader(string name)
        {
            if (name == this.GetDatabaseName())
            {
                if (data.IsClosed)
                {
                    return;
                }
                data.Close();
            }
            else if (name == this.GetInfoName())
            {
                if (dataInfo.IsClosed)
                {
                    return;
                }
                dataInfo.Close();
            }
            else
            {
                return;
            }
        }
        /// <summary>
        /// 得到连接
        /// </summary>
        /// <returns></returns>
        public MySqlConnection GetConnection()
        {
            return connect;
        }

        /// <summary>
        /// 得到DataReader
        /// </summary>
        /// <returns></returns>
        public MySqlDataReader GetDataReader()
        {
            return data;
        }

        public MySqlDataReader GetDataReader(string name)
        {
            if (name == this.GetDatabaseName())
            {
                return data;
            }
            else if (name == this.GetInfoName())
            {
                return dataInfo;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 返回主数据库名
        /// </summary>
        /// <returns></returns>
        public string GetDatabaseName()
        {
            return this.database;
        }

        /// <summary>
        /// 返回info数据库名
        /// </summary>
        /// <returns></returns>
        public string GetInfoName()
        {
            return this.info;
        }

        /// <summary>
        /// 判断数据库名称是否正确
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool IsValidName(string name)
        {
            if (name == database || name == info)
            {
                return true;
            }
            else
            {
                MessageBox.Show("无效的数据库名", "错误",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 判断表table的某一列column是否为主键或外键
        /// 返回information_schema中表key_column_usage的constraint_name键值
        /// refTableName保存referenced_table_name
        /// </summary>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public void KeyType(string table, string column,ref string keyType, ref string refTableName)
        {
            this.CreateConnection(this.info, null);

            this.Open(this.info);

            string sql = "SELECT constraint_name,referenced_table_name FROM " + this.info + ".key_column_usage ";
            sql += "WHERE table_name = '" + table + "'";
            sql += " AND column_name ='" + column + "'";
            //MessageBox.Show(sql);

            this.ExecuteReader(this.info, sql);

            while (this.Read(this.info))
            {
                keyType = this.GetString(this.info, 0);
                refTableName = this.GetString(this.info, 1);
            }
            //MessageBox.Show(str);
            this.CloseDataReader(this.info);
            this.Close(this.info);

            if (keyType == null)
            {
                keyType = "";
            }
        }

        #endregion

    }
}
