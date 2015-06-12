using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GHCS.DataBase;

namespace GHCS.Common
{
    class DataParse
    {
        IDataBase database = MySqlDataBase.getInstance();

        public string gatewayAddressId;// 网关地址id
        public string gatewayAddress;  // 网关地址
        public string gatewaySign;     // 网关编号
        public string collectorSign;   // 采集器编号
        public string dataType;        // 数据类型
        public string dataValue;       // 数据值
        public string collectTime;     // 采集时间

        private string monitorDataId;

        public DataParse()
        {
            CreateConnection();
            OpenDataBase();
        }
        /// <summary>
        /// 创建数据库连接
        /// </summary>
        private int CreateConnection()
        {
            return database.CreateConnection(null);
        }

        /// <summary>
        /// 打开数据库
        /// </summary>
        private void OpenDataBase()
        {
            database.Open();
        }


        /// <summary>
        /// 插入采样数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        public void InsertSampleData(int id, int type, float value, string time)
        {
            string sql = "";

            GetGatewaySign();

            if (gatewaySign.CompareTo("") == 0)
            {
                return;
            }

            collectorSign = System.Convert.ToString(id);
            dataType = System.Convert.ToString(type);
            dataValue = System.Convert.ToString(value);
            collectTime = System.Convert.ToString(time);

            GetMonitorDataId();

            if (monitorDataId == "")
            {
                sql = "INSERT INTO MonitorData";
                sql += "(gatewaySign,collectorSign,dataTypeId,collectValue,collectTime)";
                sql += " VALUES(";
                sql += gatewaySign + ",";
                sql += collectorSign + ",";
                sql += dataType + ",";
                sql += dataValue + ",";
                sql += "'" + collectTime + "'" + ")";
            }else
            {
                sql = "UPDATE MonitorData";
                sql += " SET gatewaySign=" + gatewaySign + ",";
                sql += " collectorSign=" + collectorSign + ",";
                sql += " dataTypeId=" + dataType + ",";
                sql += " collectValue=" + dataValue + ",";
                sql += " collectTime='" + collectTime + "'";
                sql += " WHERE monitorDataId=";
                sql += monitorDataId;
            }            

            database.ExcuteNonQuery(sql);

            sql = "INSERT INTO HistoryData";
            sql += "(gatewaySign,collectorSign,dataTypeId,collectValue,collectTime)";
            sql += " VALUES(";
            sql += gatewaySign + ",";
            sql += collectorSign + ",";
            sql += dataType + ",";
            sql += dataValue + ",";
            sql += "'" + collectTime + "'" + ")";

            database.ExcuteNonQuery(sql);
        }

        /// <summary>
        /// 获取采集数据的id
        /// </summary>
        /// <returns></returns>
        public string GetMonitorDataId()
        {
            string sql;

            sql = "SELECT monitorDataId  FROM MonitorData WHERE gatewaySign=";
            sql += gatewaySign ;
            sql += " AND collectorSign=" + collectorSign ;
            sql += " AND dataTypeId=" + dataType;

            monitorDataId = "";
            List<string> list = new List<string>(1);
            database.QueryTable(sql, list, 1);
            if (list.Count > 0)
            {
                monitorDataId = list[0];
            }

            return monitorDataId;
            
        }

        /// <summary>
        /// 通过网关地址 获取网关标识符
        /// </summary>
        public void GetGatewaySign()
        {
            string sql;
            sql = "SELECT devicesAddressSign,devicesAddressId FROM DevicesAddress WHERE devicesAddress=";
            sql += gatewayAddress;

            List<string> list = new List<string>(2);

            database.QueryTable(sql, list, 2);

            if (list.Count > 0)
            {
                gatewaySign = list[0];
                gatewayAddressId = list[1];
            }
        }

        /// <summary>
        /// 插入控制状态
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="channel"></param>
        /// <param name="status"></param>
        public void InsertControlStatus(byte controller,byte channel,byte status,int result)
        {
            GetGatewaySign();

            ////////////////////////////控制状态////////////////////////////////////
            string controlledDeviceId = "";
            string sql;
            sql = "SELECT channel.controlledDeviceId FROM (((controller"
                + " INNER JOIN gateway ON gateway.gatewayId=controller.gatewayId AND gateway.devicesAddressId="
                + gatewayAddressId
                + ") INNER JOIN devicesaddress ON controller.devicesAddressId=devicesAddress.devicesAddressId AND devicesAddress.devicesAddressSign="
                + System.Convert.ToString(controller)
                + ") INNER JOIN channel ON controller.controllerId=channel.controllerId AND channel.channelSign="
                + System.Convert.ToString(channel) + ")";

            List<string> list = new List<string>(1);

            database.QueryTable(sql, list, 1);

            if (list.Count > 0)
            {
                controlledDeviceId = list[0];
            }

            sql = "REPLACE INTO ControlledDevicesState(controlledDeviceId,controlledDeviceStatus,controlledTime)";
            sql += " VALUES(";
            sql += controlledDeviceId + ",";
            sql += "'" + System.Convert.ToString(status) + "'" + ",";
            sql += "'" + DateTime.Now.ToString() + "'" + ")";

            database.ExcuteNonQuery(sql);
            //////////////////////////////////////////////////////////////////////////

            ////////////////////////////////////控制日志//////////////////////////////
            ControlLog log = new ControlLog();
            log.ControlledDeviceId = controlledDeviceId;
            if (result == 0)
            {
                log.ControlResult = "远程控制失败";
            }
            else if (result == 1)
            {
                log.ControlResult = "远程控制成功";
            }

            log.UpdateStatus();
            //////////////////////////////////////////////////////////////////////////
        }

        public void ExecuteCmd(string sql)
        {
            database.ExcuteNonQuery(sql);
        }
    }
}
