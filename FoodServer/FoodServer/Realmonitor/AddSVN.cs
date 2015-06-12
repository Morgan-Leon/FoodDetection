using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GHCS.DataBase;
using MySql.Data.MySqlClient;
using GHCS;
using TreeExXML;

namespace FoodServer.realmonitor
{
    public partial class AddSVN : Form
    {
       
        public MainForm fath;
        public AddSVN()
        {
            InitializeComponent();


        }
        public AddSVN(MainForm parent)
        {
            InitializeComponent();

            fath = parent;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public delegate void MyDelegate(Label myControl, string myArg);
        private void button1_Click(object sender, EventArgs e)
        {

            MainForm.ProName = this.textBox_SVN.Text.Trim();

            TreeExXMLCls xt = new TreeExXMLCls();
            xt.AddTreeNode(fath.xmlstr, fath.SelectNode.Text, MainForm.ProName);
            fath.treeView1.ExpandAll();
            fath.ReloadTreeView();

            this.Close();
        }

    }
}
