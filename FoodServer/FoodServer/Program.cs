using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using FlyTcpFramework;
using FoodServer.Class;
using System.Text;
using FoodServer.TCPServ;
namespace FoodServer
{
    public class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        /// 

        [STAThread]
        static void Main()
        {
          
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login.Login());
        }
    }
}
