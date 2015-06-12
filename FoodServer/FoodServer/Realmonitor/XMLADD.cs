using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TreeExXML;

namespace FoodServer.Realmonitor
{
    public partial class XMLADD : Form
    {
        public MainForm fa;//父窗口变量
        public string FirstName;
        public string SecendName;
        public XMLADD()
        {
          //  this.comboBox1.SelectedIndex = 0;
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TreeExXMLCls xt = new TreeExXMLCls();
            
            xt.addTreeNode(fa.xmlstr, fa.SelectNode.Text,SecendName);
            fa.treeView1.Nodes.Clear();
            xt.XMLToTree(fa.xmlstr, fa.treeView1);
            this.Close();
        }
        List<string> Lis = new List<string>();
        private void First_selectchanged(object sender, EventArgs e)
        {
            Lis.Clear();
            TreeExXMLCls xt = new TreeExXMLCls();
            Lis = xt.GetTreeNode(fa.xmlstr, comboBox1.Text);
            this.comboBox2.Items.Clear();
            for (int i = 0; i < Lis.Count; ++i)
            {
                this.comboBox2.Items.Add(Lis[i]);
            }
            Lis.Clear();
              
        }

        private void Second_Selcetchange(object sender, EventArgs e)
        {
            SecendName = comboBox2.SelectedItem.ToString();
        }
    }
}
