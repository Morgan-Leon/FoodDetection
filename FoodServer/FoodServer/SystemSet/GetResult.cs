using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GHCS.DataBase;
using GHCS;
using MySql.Data.MySqlClient;
using System.Data;

namespace FoodServer.SystemSet
{
    
    //获取每个监测点的检测结果数
    class GetResult
    {
        //数据库操作
        MySqlDataBase dbMySql = MySqlDataBase.getInstance();
        string databaseName;
        IDataBase database = MySqlDataBase.getInstance();
        MySqlDataAdapter myDataAdpter = new MySqlDataAdapter();
        DataSet myDataSet = new DataSet();
        
        public GetResult()
        {
            databaseName = dbMySql.GetDatabaseName();
        }
        public struct DetectInfo
        {
            public int  m_num;//检测总数
            public int m_good;//合格总数
            public int m_yellownum;//检测 黄色报警数量
            public int m_fensenum;//检测 粉色报警数量
            public int m_rednum;//检测 红色报警数量
           
            public double m_rate;//合格率
            public DataTable table;
        }
        /// <summary>
        /// 读取数据的条数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private int ReadHistoryDataCount(string sql)
        {
            int value = 0;
            List<string> list = new List<string>();

            database.QueryTable(sql, list, 1);
            if (list.Count > 0)
            {
                value = System.Convert.ToInt32(list[0]);
            }

            return value;

        }

       
        //获取时间段t1-t2之间监测点deviceID检测结果数以及检测结果
        public DetectInfo GetResultInfo(string DeviceID, DateTime t1, DateTime t2)
        {
            DetectInfo d_info = new DetectInfo();
            //保存总检测记录数
            string sqlall = "SELECT COUNT(*) FROM detectioninfo  WHERE Device_ID = '" + DeviceID + "'"+
                            " AND test_time BETWEEN  '" + t1 + "'"+
                            " AND '" + t2 + "'";
            d_info.m_num = ReadHistoryDataCount(sqlall);
            //保存合格记录数
           string sql = "SELECT COUNT(*) FROM detectioninfo  WHERE Device_ID = '" + DeviceID + "'" +
                            " AND test_time BETWEEN  '" + t1 + "'" +
                            " AND '" + t2 + "'" +
                            " AND Juge_result  = '合格'";
            
            d_info.m_good = ReadHistoryDataCount(sql);
            //保存黄色记录数
         /*   SystemSet ss = new SystemSet();*/
         


            //读取黄色警报数量
            sql = "SELECT COUNT(*) FROM detectioninfo INNER JOIN ftinfor ON (detectioninfo.p_name = ftinfor.ftestitems AND detectioninfo.type = ftinfor.fsample AND detectioninfo.standard = ftinfor.standards)  WHERE Device_ID = '" + DeviceID + "'" +
                            " AND test_time BETWEEN '" + t1 + "'" +
                            " AND '" + t2 + "'" +
                            " AND Juge_result  = '不合格'" +
                            " AND (Detect_result-comparemax) >=  comparemax * alarm_yellow  AND (Detect_result-comparemax) < comparemax * alarm_pitch ";
            d_info.m_yellownum = ReadHistoryDataCount(sql);

            //读取粉色警报数量
            sql = "SELECT COUNT(*) FROM detectioninfo INNER JOIN ftinfor ON (detectioninfo.p_name = ftinfor.ftestitems AND detectioninfo.type = ftinfor.fsample AND detectioninfo.standard = ftinfor.standards)   WHERE Device_ID = '" + DeviceID + "'" +
                            " AND test_time BETWEEN '" + t1 + "'" +
                            " AND '" + t2 + "'" +
                            " AND Juge_result  = '不合格'" +
                            " AND (Detect_result-comparemax) >= comparemax * alarm_pitch  AND (Detect_result-comparemax) < comparemax* alarm_red";
            d_info.m_fensenum = ReadHistoryDataCount(sql);
            //读取红色色警报数量
            sql = "SELECT COUNT(*) FROM detectioninfo INNER JOIN ftinfor ON (detectioninfo.p_name = ftinfor.ftestitems AND detectioninfo.type = ftinfor.fsample AND detectioninfo.standard = ftinfor.standards)   WHERE Device_ID = '" + DeviceID + "'" +
                            " AND test_time BETWEEN '" + t1 + "'" +
                            " AND '" + t2 + "'" +
                            " AND Juge_result  = '不合格'" +
                            " AND (Detect_result-comparemax) >= comparemax* alarm_red";
            d_info.m_rednum = ReadHistoryDataCount(sql);

          

            d_info.m_rate = (double)d_info.m_good/ (double)d_info.m_num;

            DataSet dataSet = new DataSet();
            int ret = database.ReadDataBase(sqlall, "company", dataSet);

            if (ret == 0)
            {
                System.Data.DataTable dtInfo = new System.Data.DataTable();
                d_info.table = dtInfo;
            }     
            return d_info;
        }
    }
}
