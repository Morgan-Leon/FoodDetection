using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlyTcpFramework;
using FoodServer.Class;
using System.Data;
using System.Net.Sockets;
using System.Diagnostics;
using GHCS.DataBase;
using GHCS;
using MySql.Data.MySqlClient;

//using FoodServer.foodDataSetTableAdapters;

namespace FoodServer.TCPServ
{

   public class TcpServerEvent
    {
        MySqlDataBase dbMySql = MySqlDataBase.getInstance();
        string databaseName;
        IDataBase database = MySqlDataBase.getInstance();
        MySqlDataAdapter myDataAdpter = new MySqlDataAdapter();
        DataSet myDataSet = new DataSet();
       /// <summary>
       /// 定义数据头长度
       /// 4-2-4（lenth，id，centercode）
       /// </summary>
       /// 
      static  public  int MsgHeadLength = 14;
      static public  int Msgoffset = 15;
      static public  int MsgTotalLen = 18;
      static public bool Is_register = false;
 
       public  TcpServerEvent()
       {
           //初始化数据
//            foodDataSetTableAdapters.userTableAdapter tb_UserTableAdapter;
//            foodDataSetTableAdapters.detectioninfoTableAdapter tb_DetectionInfoTableAdapter;
// 
//            tb_UserTableAdapter = new FoodServer.foodDataSetTableAdapters.userTableAdapter();
//            tb_UserTableAdapter.Fill(foodDataSet.user);
// 
//            tb_DetectionInfoTableAdapter = new foodDataSetTableAdapters.detectioninfoTableAdapter();
//            tb_DetectionInfoTableAdapter.Fill(foodDataSet.detectioninfo);
              databaseName = dbMySql.GetDatabaseName();
       }
       

       public void ClientConn(object sender, NetEventArgs e)
       {
           string info = string.Format("A Client:{0} connect server Session:{1}. Socket Handle:{2}",
               e.Client.ClientSocket.RemoteEndPoint.ToString(),
               e.Client.ID, e.Client.ClientSocket.Handle);

           Console.WriteLine(info);

        //   Console.Write(">");
       }

       public  void ServerFull(object sender, NetEventArgs e)
       {
           string info = string.Format("Server is full.the Client:{0} is refused",
               e.Client.ClientSocket.RemoteEndPoint.ToString());

           
           //Must do it
           //服务器满了,必须关闭新来的客户端连接
           e.Client.Close();

           Console.WriteLine(info);

         //  Console.Write(">");

       }

       public void ClientClose(object sender, NetEventArgs e)
       {
           string info;

           if (e.Client.TypeOfExit == Session.ExitType.ExceptionExit)
           {
               info = string.Format("A Client Session:{0} Exception Closed.",
                   e.Client.ID);
           }
           else
           {
               info = string.Format("A Client Session:{0} Normal Closed.",
                   e.Client.ID);
           }

           Console.WriteLine(info);

         //  Console.Write(">");
       }


       /// <summary>
       ///   一帧数据 5-27
       /// </summary>
       /// <summary>
       /// 
       bool packetComplteted;
       private int onePacket(List<byte> buf)
       {
           packetComplteted = false;
           int idx = 0, k = 0;
           for (idx = 0; idx < buf.Count; ++idx )
           {
               if (buf[idx] == 0x5b)
               {
                   packetComplteted = true;
                   break;
               }
           }

           if (packetComplteted == true)
           {
               idx = 0;
               k = 0;
               while (buf[idx] != 0x5d)
               {

               }
           }
           return 0;
       }

       /// <summary>
       ///   
       /// </summary>
       public void LogIn(object sender, NetEventArgs e)
       {
           Is_register = true;
       }
     static  public int GetLen(byte[] buf)
       {
           int len = Encoding.Default.GetString(buf).Length;
          
               for (int i = 0; i < len - 1; i++)
               {
                   if (buf[i] == '\0')
                   {
                       len = i;
                       break;
                   }
               }
          
           return len;
       }

     private System.Data.DataTable LoadDetectionInfo()
     {
         DataSet dataset = new DataSet();
         dataset.Clear();
         string sql = "SELECT * FROM detectioninfo";
         int ret = database.ReadDataBase(sql, "deinfo", dataset);
         if (ret == 0)
         {
             System.Data.DataTable table = new System.Data.DataTable();
             table = dataset.Tables["deinfo"];
             return table;
         }
         return null;
     }
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
       public void RecvData(object sender, NetEventArgs e)
       {

         //  string info = string.Format("recv data:{0} from:{1}.", e.Client.Datagram, e.Client);
           TcpSvr svr = (TcpSvr)sender;

           byte[] recivebuffer = System.Text.Encoding.UTF8.GetBytes(e.Client.Datagram);
           byte[] buffer = new byte[MsgHeadLength];
           Array.Copy(recivebuffer, 1, buffer, 0, MsgHeadLength);
           TastInfo Info = new TastInfo();
           MSGHEAD head = new MSGHEAD();
           object headType = head;
           Info.ByteArrayToStructureEndian(buffer, ref headType, 0);
           head = (MSGHEAD)headType;
           string centercode = head.Msg_GNSSCenter.ToString();
           switch (head.Msg_ID)
           {
  
                   //用于心跳监测
               case 03:
                   svr.SendMessage(e.Client, "", 0x0004, centercode);
                  // svr.SendMessage(e.Client, "", 0x0003, centercode);
                   break;
               //检测结果上传消息
                   //TEST NUM	4	Unint32_t	结果个数
                   //TEST INFO	804	Octet_string	检测结果信息
               
               case 06:
                   byte[] test_num = new byte[4];
                   Array.Copy(recivebuffer, MsgHeadLength + 1, test_num,0,4);
                   Array.Reverse(test_num);
                  
                   UInt32 num = (UInt32)BitConverter.ToInt32(test_num, 0);
                   byte[] buffer3 = new byte[head.Msg_Length-18-4];
                   Array.Copy(recivebuffer, MsgHeadLength + 1+4, buffer3, 0, head.Msg_Length-18-4);
                   DETECTION_INFO detInfo = new DETECTION_INFO();
                   object detInfoType = detInfo;

                   Info.ByteArrayToStructureEndian(buffer3, ref detInfoType, 0);
                   detInfo = (DETECTION_INFO)detInfoType;

//                     DataTable tbInfo = LoadDetectionInfo();
//                     DataSet ds = new DataSet();
//                     
//                     DataRow r = tbInfo.NewRow();
//                     r["p_name"] = System.Text.Encoding.UTF8.GetString(detInfo.p_name, 0, GetLen(detInfo.p_name)).Trim();
//                     r["sample_name"] = System.Text.Encoding.UTF8.GetString(detInfo.sample_name,0,GetLen(detInfo.sample_name)).Trim();
//                     r["channel"] = detInfo.channel;
//                     r["type"] = System.Text.Encoding.UTF8.GetString(detInfo.type, 0, GetLen(detInfo.type));
//                     r["standard"] = System.Text.Encoding.UTF8.GetString(detInfo.standard, 0, GetLen(detInfo.standard));
//                     r["sample_no"] = System.Text.Encoding.UTF8.GetString(detInfo.sample_no, 0, GetLen(detInfo.sample_no)).Trim();
//                     r["sites"] = System.Text.Encoding.UTF8.GetString(detInfo.sites, 0, GetLen(detInfo.sites)).Trim();
//                     r["submission_unit"] = System.Text.Encoding.UTF8.GetString(detInfo.submission_unit, 0, GetLen(detInfo.submission_unit));
//                     r["test_operator"] = System.Text.Encoding.UTF8.GetString(detInfo.test_operator, 0, GetLen(detInfo.test_operator));
//                     
//                     r["test_unit"] = System.Text.Encoding.UTF8.GetString(detInfo.test_unit, 0, GetLen(detInfo.test_unit));
//                     r["test_time"] = System.Text.Encoding.UTF8.GetString(detInfo.test_time, 0, GetLen(detInfo.test_time));
//                   
//                     r["User_Id"] = e.Client.UserId;
// 
//                     //并将数据写入到数据库
//                     tbInfo.Rows.Add(r);
                    
                    string p_name = System.Text.Encoding.UTF8.GetString(detInfo.p_name, 0, GetLen(detInfo.p_name)).Trim();
                    string sample_name = System.Text.Encoding.UTF8.GetString(detInfo.sample_name,0,GetLen(detInfo.sample_name)).Trim();
                    int channel = (int)detInfo.channel;
                    string type = System.Text.Encoding.UTF8.GetString(detInfo.type, 0, GetLen(detInfo.type));
                    string standard = System.Text.Encoding.UTF8.GetString(detInfo.standard, 0, GetLen(detInfo.standard));
                    string sample_no = System.Text.Encoding.UTF8.GetString(detInfo.sample_no, 0, GetLen(detInfo.sample_no)).Trim();
                    string sites = System.Text.Encoding.UTF8.GetString(detInfo.sites, 0, GetLen(detInfo.sites)).Trim();
                    string submission_unit = System.Text.Encoding.UTF8.GetString(detInfo.submission_unit, 0, GetLen(detInfo.submission_unit));
                    string test_operator = System.Text.Encoding.UTF8.GetString(detInfo.test_operator, 0, GetLen(detInfo.test_operator));
                    string test_unit = System.Text.Encoding.UTF8.GetString(detInfo.test_unit, 0, GetLen(detInfo.test_unit));
                    string test_time = System.Text.Encoding.UTF8.GetString(detInfo.test_time, 0, GetLen(detInfo.test_time));
                    DateTime dt = DateTime.ParseExact(test_time, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    int User_Id= (int)e.Client.UserId;
                    string  strsql = "Insert Into " +
                    "detectioninfo(p_name,sample_name,channel,type,standard,sample_no,sites,submission_unit,test_operator,test_unit,test_time,User_Id)"
                    + "values('" + p_name + "','" + sample_name + "'," + channel + ",'" + type + "','" + standard + "','" + sample_no + "','" + sites + "','" + submission_unit + "','" + test_operator + "','" + test_unit + "','" + test_time + "',"+User_Id+")";
              
                    dbMySql.Open(databaseName);
                    dbMySql.ExcuteNonQuery(databaseName, strsql);
                    dbMySql.Close(databaseName);

                    break;
               case 09:
                   //视频上传
                   svr.SendMessage(e.Client, 0, 0x000a, centercode);
                   break;
               case 0x000e:
               //远程状态和工作参数查询应答消息
                   //具体参数不明确
                   break;
              

               default:
                   break;
           }


           //测试把收到的数据返回给客户端
           //svr.SendText(e.Client, e.Client.Datagram);
           //MessageBox.Show(message.ToString());


           //svr.SendFile(this.textBox1.Text);

       }

    }
}
