using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FoodServer.realmonitor
{
    public partial class camare : Form
    {
        public camare()
        {
            InitializeComponent();
        }
        public camare(string IP)
        {
            //string  url = "http://"+IP192.168.1.230:8090/test.swf
            webBrowser1.Url = new System.Uri("http://192.168.1.230:8090/test.swf", System.UriKind.Absolute);
        }
    }
}
