using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace GHCS
{
    /// <summary>
    /// 数据库接口类
    /// </summary>
    public interface IDataBase
    {
        // 创建数据库连接
        int CreateConnection(string connectString);

        // 创建数据库连接，name为数据库名称
        int CreateConnection(string name, string connectString);

        // 打开和关闭数据库
        int Open();
        int Close();
        // 打开和关闭数据库,name为数据库名称
        int Open(string name);
        int Close(string name);

        // 是否有数据可读
        bool Read();

        // 返回某一列数据
        string GetString(int column);
        int QueryTable(string sql, List<string> values, int columns);

        // 执行sql语句，例如插入、删除、更新等
        int ExcuteNonQuery(string sql);
        int ExcuteNonQuery(string name, string sql);

        // 执行sql语句，并形成表srcTable，通过数据集读取
        int ReadDataBase(string sql, string srcTable, DataSet dataSet);
        //void CloseDataAdapter();

        // 执行sql语句，并获取记录
        int ExecuteReader(string sql);
        int ExecuteReader(string name, string sql);
        MySqlDataReader GetDataReader();
        void CloseDataReader();

        //判断表table的某一列column是否为主键或外键
        //List<string> KeyType(string table, string column);
    }
}
