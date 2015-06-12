using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace FoodServer.statistic
{
    public partial class Analysis : Form
    {
        public Analysis()
        {
            InitializeComponent();
            CreateImage();
        }



        /// <summary>
        ///   柱状图 
        /// </summary>
        private void CreateImage()
        {

            int height = pictureBox1.Width;
            int width = pictureBox1.Height;

            Bitmap image = new Bitmap(width, height);

            //创建Graphics
            Graphics g = Graphics.FromImage(image);

            try
            {
                //清空北京
                g.Clear(Color.White);

                Font font = new Font("Arial", 10, FontStyle.Regular);
                Font font1 = new Font("宋体", 20, FontStyle.Bold);

                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height),
                    Color.Blue, Color.BlueViolet, 1.2f, true);

                g.FillRectangle(Brushes.WhiteSmoke, 0, 0, width, height);


                //   g.DrawString(this.ddlTaget.SelectedItem.Text + " " + this.ddlYear.SelectedItem.Text +
                //" 成绩统计柱状图", font1, brush, new PointF(70, 30));
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Blue), 0, 0, image.Width - 1, image.Height - 1);


                Pen mypen = new Pen(brush, 1);


                int x = 100;

                for (int i = 0; i < 14; i++)
                {
                    g.DrawLine(mypen, x, 80, x, 340);
                    x = x + 40;
                }

                Pen mypen1 = new Pen(Color.Blue, 2);
                x = 60;

                g.DrawLine(mypen1, x, 80, x, 340);


                int y = 106;
                for (int i = 0; i < 9; i++)
                {
                    g.DrawLine(mypen, 60, y, 620, y);
                    y = y + 26;
                }
                g.DrawLine(mypen1, 60, y, 620, y);


                //X
                String[] n = { "yi", "er", "san", "si", "wu", "liu", "qi" };

                x = 78;

                for (int i = 0; i < 7; i++)
                {
                    g.DrawString(n[i].ToString(), font, Brushes.Blue, x, 348);
                }

                //y
                String[] m = {"250","225", "200", "175", "150", "125", "100", " 75",
" 50", " 25", " 0"};

                y = 72;

                for (int i = 0; i < 10; i++)
                {
                    g.DrawString(m[i].ToString(), font, Brushes.Blue, 25, y);
                    y += 26;
                }



                int[] Count1 = new int[7];
                int[] Count2 = new int[7];

//                 DataTable db = foodDataSet1.detectioninfo;
//                 foreach (DataRow r in db.Rows)
//                 {
//                     Count1[0] = Convert.ToInt32(r["channel"].ToString());
// 
//                 }


                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                this.pictureBox1.Image = Image.FromStream(ms);

            }
            catch (System.Exception ex)
            {

            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }



        /// <summary>
        ///    扇形统计图的绘制
        /// </summary>

        private void CreateImageP()
        {
            /*
        //把连接字串指定为一个常量
        SqlConnection Con = new SqlConnection("Server=(Local);
        Database=committeeTraining;Uid=sa;Pwd=**");
        Con.Open();
        string cmdtxt = selectString; // "select * from ##Count"; //
        //SqlCommand Com = new SqlCommand(cmdtxt, Con);
        DataSet ds = new DataSet();
        SqlDataAdapter Da = new SqlDataAdapter(cmdtxt, Con);
        Da.Fill(ds);
        Con.Close();
        float Total = 0.0f, Tmp;

        //转换成单精度。也可写成Convert.ToInt32
        Total = Convert.ToSingle(ds.Tables[0].Rows[0][this.count[0]]);

        // Total=Convert.ToSingle(ds.Tables[0].Rows[0][this.count[0]]);
        //设置字体，fonttitle为主标题的字体
        Font fontlegend = new Font("verdana", 9);
        Font fonttitle = new Font("verdana", 10, FontStyle.Bold);

        //背景宽
        int width = 350;
        int bufferspace = 15;
        int legendheight = fontlegend.Height * 10 + bufferspace; //高度
        int titleheight = fonttitle.Height + bufferspace;
        int height = width + legendheight + titleheight + bufferspace;//白色背景高
        int pieheight = width;
        Rectangle pierect = new Rectangle(0, titleheight, width, pieheight);

        //加上各种随机色
        ArrayList colors = new ArrayList();
        Random rnd = new Random();
        for (int i = 0; i < 2; i++)
        colors.Add(new SolidBrush(Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255))));

        //创建一个bitmap实例
        Bitmap objbitmap = new Bitmap(width, height);
        Graphics objgraphics = Graphics.FromImage(objbitmap);

        //画一个白色背景
        objgraphics.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);

        //画一个亮黄色背景 
        objgraphics.FillRectangle(new SolidBrush(Color.Beige), pierect);

        //以下为画饼图(有几行row画几个)
        float currentdegree = 0.0f;

        //画通过人数
        objgraphics.FillPie((SolidBrush)colors[1], pierect, currentdegree,
        Convert.ToSingle(ds.Tables[0].Rows[0][this.count[1]]) / Total * 360);
        currentdegree += Convert.ToSingle(ds.Tables[0].Rows[0][this.count[1]]) / Total * 360;

        //未通过人数饼状图
        objgraphics.FillPie((SolidBrush)colors[0], pierect, currentdegree,
        ((Convert.ToSingle(ds.Tables[0].Rows[0][this.count[0]]))-(Convert.ToSingle(ds.Tables[0].Rows[0][this.count[1]]))) / Total * 360);
        currentdegree += ((Convert.ToSingle(ds.Tables[0].Rows[0][this.count[0]])) - 
        (Convert.ToSingle(ds.Tables[0].Rows[0][this.count[1]]))) / Total * 360;


        //以下为生成主标题
        SolidBrush blackbrush = new SolidBrush(Color.Black);
        SolidBrush bluebrush = new SolidBrush(Color.Blue);
        string title = " 机关单位成绩统计饼状图: "
        + "\n \n\n";
        StringFormat stringFormat = new StringFormat();
        stringFormat.Alignment = StringAlignment.Center;
        stringFormat.LineAlignment = StringAlignment.Center;

        objgraphics.DrawString(title, fonttitle, blackbrush,
        new Rectangle(0, 0, width, titleheight), stringFormat);

        //列出各字段与得数目
        objgraphics.DrawRectangle(new Pen(Color.Red, 2), 0, height + 10 - legendheight, width, legendheight + 50);

        objgraphics.DrawString("----------------统计信息------------------", 
        fontlegend, bluebrush, 20, height - legendheight + fontlegend.Height * 1 + 1);
        objgraphics.DrawString("统计单位: " + this.ddlTaget.SelectedItem.Text, 
        fontlegend, blackbrush, 20, height - legendheight + fontlegend.Height * 3 + 1);
        objgraphics.DrawString("统计年份: " + this.ddlYear.SelectedItem.Text, 
        fontlegend, blackbrush, 20, height - legendheight + fontlegend.Height * 4 + 1);
        objgraphics.DrawString("统计期数: " + this.ddlSpan.SelectedItem.Text, 
        fontlegend, blackbrush, 20, height - legendheight + fontlegend.Height * 5 + 1);

        objgraphics.FillRectangle((SolidBrush)colors[1], 5,height - legendheight + fontlegend.Height * 8 + 1, 10, 10);
        objgraphics.DrawString("报名总人数: " + Convert.ToString(Convert.ToSingle(ds.Tables[0].Rows[0][this.count[0]])), 
        fontlegend, blackbrush, 20, height - legendheight + fontlegend.Height * 7 + 1);
        objgraphics.FillRectangle((SolidBrush)colors[0], 5, height - legendheight + fontlegend.Height * 9 + 1, 10, 10);
        objgraphics.DrawString("通过总人数: " + Convert.ToString(Convert.ToSingle(ds.Tables[0].Rows[0][this.count[1]])), 
        fontlegend, blackbrush, 20, height - legendheight + fontlegend.Height * 8 + 1);
        objgraphics.DrawString("未通过人数: " + ((Convert.ToSingle(ds.Tables[0].Rows[0][this.count[0]])) - 
        (Convert.ToSingle(ds.Tables[0].Rows[0][this.count[1]]))), fontlegend, blackbrush, 20, height - legendheight + fontlegend.Height * 9 + 1);

        objgraphics.DrawString("通过率: " + Convert.ToString((Convert.ToSingle(ds.Tables[0].Rows[0][this.count[1]]) / 
        Convert.ToSingle(ds.Tables[0].Rows[0][this.count[0]])) * 100)+ " %", fontlegend, 
        blackbrush, 20, height - legendheight + fontlegend.Height * 10 + 1);

        Response.ContentType = "image/Jpeg";
        objbitmap.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
        objgraphics.Dispose();
        objbitmap.Dispose();
            */
        }
    }
}
