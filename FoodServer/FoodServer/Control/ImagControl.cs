using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FoodServer.Task;

namespace FoodServer
{
    public partial class ImagControl : UserControl
    {
        //  private Container components = null;
        private string _name;
        private string _bmpname = "room-b.png";//
        private Point p = new Point();
        public ImagControl()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(0);
        }

        public Point Center
        {

            get
            {
                // return new Point();
                return p;
            }
            set
            {
                p = value;
            }
        }

        public string PicboxShow
        {
            get
            {
                return _bmpname;
            }
            set
            {
                _bmpname = value;
            }
        }

        public string Cname
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            
            base.OnPaint(e);
            e.Dispose();
            if (_bmpname == "")
            {
                e.Dispose();
            }
            else
            {
                Image img = Image.FromFile(_bmpname);
                if (img != null)
                {
                    e.Graphics.DrawImage(img, 10, 0, 20, 20);
                    e.Graphics.DrawString(_name, new Font("Arial", 8), Brushes.Red, new Point(0, 20));
                }
                else
                {

                    e.Dispose();
                }
            }

        }


    }

}
