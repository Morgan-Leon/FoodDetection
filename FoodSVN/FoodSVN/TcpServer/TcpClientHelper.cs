using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
namespace FoodSVN.TcpServer
{
    public class TcpClientHelper : IDisposable
    {
        TcpClient client;
        NetworkStream netstream;
        string _serverip = "127.0.0.1";
        int _port = 5632;

        TcpCommon tcpCommon = new TcpCommon();

        #region TcpClientHelper constructor
        public TcpClientHelper(string strServerIP, int serverPort)
        {
            _serverip = strServerIP;
            _port = serverPort;

        }
        #endregion

        public void Start()
        {
            client = new TcpClient(_serverip, _port);
            netstream = client.GetStream();
        }

        public void Stop()
        {
            if (netstream != null)
            {
                netstream.Close();
            }

            if (client != null)
            {
                client.Close();
            }
        }

        #region TcpCommon所有方法
        public string CalcFileHash(string FilePath)
        {
            return tcpCommon.CalcFileHash(FilePath);
        }

        public bool SendFile(string filePath)
        {
            return tcpCommon.SendFile(filePath, netstream);
        }


        public bool ReceiveFile(string filePath)
        {
            return tcpCommon.ReceiveFile(filePath, netstream);
        }


        public bool SendMessage(string message)
        {
            return tcpCommon.SendMessage(message, netstream);
        }

        public string ReadMessage()
        {
            return tcpCommon.ReadMessage(netstream);
        }
        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            if (netstream != null)
            {
                netstream.Close();
            }

            if (client != null)
            {
                client.Close();
            }
        }

        #endregion
    }
}

