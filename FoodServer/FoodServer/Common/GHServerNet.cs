using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

using GHCS;
using GHCS.Common;

namespace GHServerNet
{
    class GHNetServer
    {
        private TcpListener Listener; 

        private string ipAddrStr = "127.0.0.1";
        private int portNumber = 30000;

        public GHNetServer()
        {
            LoadIni();

            IPAddress ipa = IPAddress.Parse(ipAddrStr);
            Listener = new TcpListener(ipa, portNumber);
            //Listener.Start();
        }

        /// <summary>
        /// 加载ini文件中的服务器信息
        /// </summary>
        private void LoadIni()
        {
            IniAc ini = new IniAc();
            ipAddrStr = ini.ReadValue("NET", "ip");

            string portStr = ini.ReadValue("NET", "port");
            portNumber = System.Convert.ToInt32(portStr);
        }

        /// <summary>
        /// 监听客户端的连接
        /// </summary>
        public void StartListen()
        {
            Listener.Start();
            while (true)
            {
                try 
                {
                    Socket s = Listener.AcceptSocket();
                    NetConnection newconn = new NetConnection(s);
                    Thread t = new Thread(new ThreadStart(newconn.connThreadProc));
                    t.Start(); 
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

        }        
    }

    class NetConnection
    {
        DataParse dataParse = new DataParse();

        public NetConnection(Socket socket)
        {
            sock = socket;
            state = CONNSTATE.CONN_INITED;
            packetBuffer = new byte[1500];
            gatewayFlagBytes = new byte[16];
            registered = false;
            packetCompleted = false;
            
        }

        public void connThreadProc()
        {
            int nread, packetLen;
            byte[] readBuffer = new byte[1500];
            List<byte> dataBuffer = new List<byte>();
            while (true)
            {
                state = CONNSTATE.CONN_READING;
                nread = sock.Receive(readBuffer);
                if (nread == 0 || nread < 0) 
                {
                    Console.WriteLine("client close the connection");
                    sock.Close();
                    break;
                }
                state = CONNSTATE.CONN_IDLE;

                printDebugInfo(readBuffer);

                // wangdong 
                if (packetCompleted == true)
                {
                    dataBuffer.Clear();// 这个地方是否每次都需要清空
                }

                dataBuffer.AddRange(readBuffer);
                packetLen = onePacket(dataBuffer);
                if (packetCompleted)
                {
                    int res = processGateWayData(packetLen);
                    if (res == -1)
                    {
                        sock.Close();
                        return;
                    }
                }
                if (!sock.Connected)
                    break;
            }
            
        }

        private int onePacket(List<byte> buf)
        {
            packetCompleted = false;
            int idx = 0, k = 0;
            for (idx = 0; idx < buf.Count; ++idx)
            {
                if (buf[idx] == 0xc0)
                {
                    packetCompleted = true;
                    break;
                }               
            }
            if (packetCompleted == true)
            {
                idx = 0; k = 0;
                while (buf[idx] != 0xc0)
                {
                    if (buf[idx] != 0xdb)
                    {
                        packetBuffer[k++] = buf[idx];
                    }
                    else if (buf[idx+1] == 0xdc)
                    {
                        ++idx;
                        packetBuffer[k++] = 0xc0;
                    }
                    else if (buf[idx+1] == 0x0dd)
                    {
                        ++idx;
                        packetBuffer[k++] = 0xdb;
                    }
                    else 
                    {
                        Console.WriteLine("packet format error");
                        packetCompleted = false;
                    }
                    ++idx;
                }
                buf.RemoveRange(0, idx+1);
                if (packetCompleted && getCrc16(packetBuffer, k) != 0)
                {
                    Console.WriteLine("crc16 check error");
                    packetCompleted = false;
                    return 0;
                }
                return k - 2;
            }
            return 0;
        }

        private int processGateWayData(int packetLen)
        {
            int cmd = packetBuffer[0];
            int res = 0;
            if (!registered && !(cmd == 0x24 || cmd == 0x23))
            {
                D("gateway has not been registered");
                return 0;
            }
            switch (cmd)
            {
                //处理上传的采集数据
                case 0x05:
                    procUploadedSampleData();
                    break;
                //处理上传的状态数据
                case 0x06:
                    procUploadedStateData();
                    break;
                //网关身份验证
                case 0x23:
                    res = verifyGatewayIdentity();
                    if (res == -1)
                        return -1;
                    break;
                //网关注册
                case 0x24:
                    gatewayRegister();
                    break;
                case 0x68:
                    procControlAck();
                    break;
                case 0x70:
                    procConfigSynAck(packetLen);
                    break;
                default:
                    break;
            };
            return 0;
        }
        //0x23
        int verifyGatewayIdentity()
        {
            if (~packetBuffer[1] < 0)
            {
                if ((~packetBuffer[1] + 256) == packetBuffer[2])
                    return 0;
            }
            else
            {
                if (~packetBuffer[1]  == packetBuffer[2])
                    return 0;
            }
            return -1;
        }
        // 0x24
        int gatewayRegister()
        {
            //if (packetBuffer.Length == 11)
            {
                gatewayFlagStr = "";
                for (int i = 0; i < 8; ++i)
                {
                    gatewayFlagBytes[i] = packetBuffer[i + 1];
                    gatewayFlagStr += string.Format("{0:X2}", gatewayFlagBytes[i]); 
                }

                // 获取网关的地址
                dataParse.gatewayAddress = gatewayFlagStr;
                //gatewayFlagStr = System.Text.Encoding.Default.GetString(gatewayFlagBytes);
                Utility.setGatewaySockMap(gatewayFlagStr, this);
                registered = true;
                //获取当前日期，并发送给网关
                byte cmd = 0x26; //设置网关时间
                DateTime time = DateTime.Now;
                string datetimestr = string.Format("{0:u}", time);
                byte[] datetimebytes = Encoding.ASCII.GetBytes(datetimestr);
                datetimebytes[19] = 0x00;
                sendPackage(cmd, datetimebytes);
            }
            //else
            //    registered = false;
            return 0;
        }
        //0x05
        private int procUploadedSampleData()
        {
            char[] DateTimeBytes = new char[20];
            int[] portSign = new int[300];
			int[] collectType = new int[300];
			float[] collectValue = new float[300];
            //the first byte is the cmd, start from the second byte
            int collectorNum = packetBuffer[1];
            string debugstr = "采集器数量: " + collectorNum;
            D(debugstr);
            int dataOffIdx = 2;
            for (int i = 0; i < collectorNum; ++i)
            {
                //get the collector id
                int collectorId = packetBuffer[dataOffIdx++];
                debugstr = "采集器Id: " + collectorId;
                D(debugstr);

                //get collect time
                for (int k = 0; k < 20; ++k)
                {
                    DateTimeBytes[k] = (char)packetBuffer[dataOffIdx++];
                }
                string collectTime = new string(DateTimeBytes, 0, 19);
                debugstr = "collectime: " + collectTime;
                D(debugstr);

                //get the number of sample data items
                int dataItemsNum = packetBuffer[dataOffIdx++];
                D("采集数据项数目: "+  dataItemsNum);

                // 获取数据内容(包括端口、数据类型、数据值)
			    D("采集数据项的数据值如下：");
                for (int n = 0; n < dataItemsNum; ++n)
                {
                    portSign[n] = packetBuffer[dataOffIdx++];
                    collectType[n] = packetBuffer[dataOffIdx++];
                    short interpart = getShortFrom2Bytes(packetBuffer[dataOffIdx+1], packetBuffer[dataOffIdx]);
                    dataOffIdx += 2;
                    short decimalpart = packetBuffer[dataOffIdx++];
                    int devided = 10;
                    if (decimalpart >= 10)
                        devided = 100;
                    else if (decimalpart >= 100)
                        devided = 1000;
                    collectValue[n] = interpart + (float)decimalpart / (float)devided;
                    debugstr = string.Format("端口号: 0x{0:x},采集类型: 0x{1:x}, 数值: {2:g}", 
                        portSign[n], collectType[n], collectValue[n]);
                    D(debugstr);

                    //在此添加数据库插入操作
                    dataParse.InsertSampleData(collectorId,collectType[n],collectValue[n],collectTime);
                }

            }
            return 0;
        }  
        
        //0x06
        private int procUploadedStateData()
        {
            char[] DateTimeBytes = new char[20];
            int portSign, sensorState;

            int collectorNum = packetBuffer[1];
            string debugstr = "采集器数量: " + collectorNum;
            D(debugstr);
            int dataOffIdx = 2;
            for (int i = 0; i < collectorNum; ++i)
            {
                int collectorId = packetBuffer[dataOffIdx++];
                debugstr = "采集器Id: " + collectorId;
                D(debugstr);
                for (int k = 0; k < 20; ++k)
                {
                    DateTimeBytes[k] = (char)packetBuffer[dataOffIdx++];
                }
                string collectTime = new string(DateTimeBytes, 0, 19);
                debugstr = "collectime: " + collectTime;
                D(debugstr);

                int electricityRemain = packetBuffer[dataOffIdx++];
                debugstr = "剩余电量: %" + electricityRemain;
                D(debugstr);

                int dataItemsNum = packetBuffer[dataOffIdx++];
                D("状态数据项数目: " + dataItemsNum);
                // 获取数据内容(包括端口、数据类型、数据值)
                D("状态数据项的数据值如下：");

                for (int n = 0; n < dataItemsNum; ++n)
                {
                    portSign = packetBuffer[dataOffIdx++];
                    sensorState = packetBuffer[dataOffIdx++];
                  
                    debugstr = string.Format("端口号: 0x{0:x},传感器状态: 0x{1:x}",
                        portSign, sensorState);
                    D(debugstr);
                }

            }
            return 0;
        }

        public int procControlAck()
        {
			byte controllerId = packetBuffer[1];
			byte channelId = packetBuffer[2];
            int res = (int)packetBuffer[3];
            byte status = packetBuffer[4];
            if (res != -1)
            {
                dataParse.InsertControlStatus(controllerId, channelId, status,res);
            }
            else
                Console.WriteLine("cmd execute time out");

            return 0;
        }

        public void procConfigSynAck(int packetLen)
        {
            int res = (int)packetBuffer[1];
            byte[] cachebuf = new byte[packetLen-2];
            for (int k = 2; k < packetLen; ++k)
            {
                cachebuf[k - 2] = packetBuffer[k];
            }
            string synLogIdStr = System.Text.Encoding.ASCII.GetString(cachebuf);

            if (res != -1)
            {
                string sqlcmd = "delete from SynLog where synLogId = ";
                sqlcmd += synLogIdStr;
                dataParse.ExecuteCmd(sqlcmd);
            }
            else
            {
                Console.WriteLine("Synchronize the configure with synlogid " + synLogIdStr + "failed");
            }
        }

        public int sendPackage(byte cmd, byte[] buf)
        {
            int n = 0, datalen = 0;
            byte[] databuf = new byte[(buf.Length + 4)];
            databuf[n++] = cmd;
            int i = 0;
            for (i = 0; i < buf.Length; ++i)
                databuf[n++] = buf[i];

            UInt16 crcCheck = getCrc16(databuf, n);
            byte[] crcbytes = BitConverter.GetBytes(crcCheck);
            databuf[n++] = crcbytes[0];
            databuf[n++] = crcbytes[1];
            //encode the packet
            datalen = n; 
            n = 0;
            byte[] sendbuf = new byte[2 * datalen+4];
            for (int k = 0; k < datalen; ++k)
            {
                if (databuf[k] == 0xc0)
                {
                    sendbuf[n++] = 0xdb;
                    sendbuf[n++] = 0xdc;
                }
                else if (databuf[k] == 0xdb)
                {
                    sendbuf[n++] = 0xdb;
                    sendbuf[n++] = 0xdd;
                }
                else
                    sendbuf[n++] = databuf[k];
            }

            sendbuf[n++] = 0xc0;
            byte[] towritebuf = new byte[n];
            for (int idx = 0; idx < n; ++idx)
                towritebuf[idx] = sendbuf[idx];
            while (state == CONNSTATE.CONN_WRITING) ;
            state = CONNSTATE.CONN_WRITING;
            try
            {
                sock.Send(towritebuf);
            }
            catch (Exception e)
            {
                Console.WriteLine("write exception " + e.ToString());
                sock.Close();
                state = CONNSTATE.CONN_IDLE;
                return -1;
            }
            finally {
                state = CONNSTATE.CONN_IDLE;
            }
            return n;
        }


        private void D(string debuginfo)
        {
            Console.WriteLine(debuginfo);
        }

        private void printDebugInfo(byte[] buf)
        {
            Console.WriteLine(">>>>>>>>>>>receive the data below<<<<<<<<<<<<<<");
            //Console.Write("{0:X}", buf);
            for (int i = 0; i < buf.Length; ++i)
            {
                Console.Write("{0:X}", buf[i]);
                if ((i + 1) % 20 == 0)
                    Console.Write("\n");
            }
        }

        public static UInt16 getCrc16(byte[] array, int len)
        {
            UInt16[] crc16_table = 
            {
                0x0000, 0xCC01, 0xD801, 0x1400, 
                0xF001, 0x3C00, 0x2800, 0xE401,
                0xA001, 0x6C00, 0x7800, 0xB401,
                0x5000, 0x9C01, 0x8801, 0x4400
            };

            UInt16 reg = 0x00;
            int r1;
            int idx = 0;
            while (idx < len)
            {
                r1 = crc16_table[reg & 0x0F];
                reg = (UInt16)((reg >> 4) & 0x0FFF);
                reg = (UInt16)(reg ^ r1 ^ crc16_table[array[idx] & 0x0F]);

                r1 = crc16_table[reg & 0x0F];
                reg = (UInt16)((reg >> 4) & 0x0FFF);
                reg = (UInt16)(reg ^ r1 ^ crc16_table[(array[idx] >> 4) & 0x0F]);

                ++idx;
            }
            return reg;
        }

        private short getShortFrom2Bytes(byte b1, byte b2)
        {
            string shortStr = "";
            shortStr += string.Format("{0:x2}", b1);
            shortStr += string.Format("{0:x2}", b2);
            short word = short.Parse(shortStr, System.Globalization.NumberStyles.HexNumber);
            return word;
        }

        public enum CONNSTATE
        {
            CONN_INITED = 0x00, CONN_READING = 0x01,
            CONN_WRITING = 0x02, CONN_IDLE = 0x04
        }

        private Socket sock;
        private volatile CONNSTATE state;
        private string gatewayFlagStr;
        private byte[] gatewayFlagBytes;
        private byte[] packetBuffer;
        private bool   packetCompleted;
        private bool registered;
    }

    class Utility
    {
        public static void setGatewaySockMap(string gatewaySign, NetConnection conn)
        {
            lock(lockobj)
            {
                if (connMap.ContainsKey(gatewaySign))
                    connMap.Remove(gatewaySign);
                connMap[gatewaySign] = conn;
            }
        }

        public static NetConnection getGatewaySockMap(string gatewaySign)
        {
            lock (lockobj)
            {
                if (connMap.ContainsKey(gatewaySign))
                    return connMap[gatewaySign];
            }
            return null;
        }

        public static Dictionary<string, NetConnection> connMap = new Dictionary<string, NetConnection>();
        private static object lockobj = new object();
    }
}
