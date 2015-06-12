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
using FlyTcpFramework;
using FoodServer.Class;
using System.Runtime.InteropServices;
using System.Data.SqlClient;


namespace FoodServer.Task
{
    public partial class CollactorConfig : Form
    {

        public MainForm father;//父窗口变量
        MySqlDataBase dbMySql = MySqlDataBase.getInstance();
        string databaseName;
        IDataBase database = MySqlDataBase.getInstance();
        MySqlDataAdapter myDataAdapter = new MySqlDataAdapter();//定义一个数据适配器
        DataSet myDataSet = new DataSet();
        //变量
        //视频
        string resolution = "320*240";
        UInt32 frame = 25;
        UInt32 Bitrate = 400;
        string vidio_form = "avi";
        UInt32 vido_width = 320;
        UInt32 vido_heigt = 240;
        //照片
        UInt32 Pic_Width = 320;
        UInt32 Pic_Heigt = 240;
        //
        string Device_ID;
        TastInfo Info = new TastInfo();//

        //id索引标记
        string tinfoid = null;

        FINFOR_INFOSTR ftinfostr = new FINFOR_INFOSTR();


        #region
        //表格头
        private void LoadConHeader()
        {
            this.dataGridView1.Columns[0].Visible = false;
            this.dataGridView1.Columns[1].HeaderText = "检测项目";
            this.dataGridView1.Columns[2].HeaderText = "样品名称";
            this.dataGridView1.Columns[3].HeaderText = "模式";
            this.dataGridView1.Columns[4].HeaderText = "主波";
            this.dataGridView1.Columns[5].HeaderText = "次波";
            this.dataGridView1.Columns[6].HeaderText = "公式C ";
            this.dataGridView1.Columns[7].HeaderText = "公式B ";
            this.dataGridView1.Columns[8].HeaderText = "公式A ";
            this.dataGridView1.Columns[9].HeaderText = "稀释倍数 ";
            this.dataGridView1.Columns[10].HeaderText = "单位 ";
            this.dataGridView1.Columns[11].HeaderText = "参考标准 ";
            this.dataGridView1.Columns[12].HeaderText = "比较方式";
            this.dataGridView1.Columns[13].HeaderText = "比较最小值 ";
            this.dataGridView1.Columns[14].HeaderText = "比较最大值 ";
            this.dataGridView1.Columns[15].HeaderText = "测量范围最大值";
            this.dataGridView1.Columns[16].HeaderText = "测量范围最小值";

            this.dataGridView1.Columns[17].HeaderText = "红色报警";
            this.dataGridView1.Columns[18].HeaderText = "粉色报警";
            this.dataGridView1.Columns[19].HeaderText = "黄色报警";
        }

        //载入表格数据
        public void LoadConfigInfo()
        {
            DataSet dataset = new DataSet();
            dataset.Clear();

            string str = " SELECT * FROM ftinfor ";
            int ret = database.ReadDataBase(str, "ftinfo", dataset);
            if (ret == 0)
            {
                this.dataGridView1.DataSource = dataset.Tables["ftinfo"];
            }
            LoadConHeader();
        }

        #endregion

        int OldRow;
        public CollactorConfig()
        {
            //  this.checkBox1.Visible = false;
            InitializeComponent();
            databaseName = dbMySql.GetDatabaseName();
        }

        public CollactorConfig(string Device_id)
        {
            // this.checkBox1.Visible = false;
            InitializeComponent();
            databaseName = dbMySql.GetDatabaseName();
            if (Device_id != "")
            {
                Device_ID = Device_id;
            }
            else
            {
                MessageBox.Show("请选择合适采集点!");
            }
        }


        //窗体载入
        private void CollactorConfig_Load(object sender, EventArgs e)
        {
            //载入配置表
            LoadConfigInfo();
        }
        #region 配置

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            int NumRow = 0;//
            SYSTEMCONFMSG sysconf = new SYSTEMCONFMSG();
            byte[] buffer = new byte[Marshal.SizeOf(sysconf)];//分配空间大小
            byte[] Mesg = new byte[4 + buffer.Length];

            string device_id = Device_ID;
            byte[] buf = System.Text.Encoding.UTF8.GetBytes(device_id);

            string strsql = "UPDATE device set ";
           /* switch (tabControl1.SelectedIndex)
            {
                case 0:
                    //选择了配置视频
                    strsql += " Resolution = '" + resolution + "', Frames = " + frame + ", Bit_Rate=" + Bitrate + " , Form = '" + vidio_form + "'";
                    strsql += " WHERE Device_ID = '" + Device_ID + "'";
                    string[] arr = resolution.Split('*');
                    vido_width = UInt32.Parse(arr[0]);
                    vido_heigt = UInt32.Parse(arr[1]);

                    VIDIOCONFI vidioconf = new VIDIOCONFI();
                    vidioconf.m_width = vido_width;
                    vidioconf.m_height = vido_heigt;
                    vidioconf.m_frame = frame;
                    vidioconf.m_bitrate = Bitrate;
                    vidioconf.m_format = System.Text.Encoding.UTF8.GetBytes(vidio_form);
                    //转成byte

                    object structType = vidioconf;
                    byte[] vidobuff = Info.StructureToByteArrayEndian(structType);

                    sysconf.m_configtype = 0;
                    sysconf.m_confnum = 0;
                    sysconf.m_para = vidobuff;
                    //Array.Copy(vidobuff,sysconf.m_para,vidobuff.Length);


                    //发送一个配置信号;
                    object sysvi = sysconf;
                    buffer = Info.StructureToByteArrayEndian(sysvi);

                    buf.CopyTo(Mesg, 0);
                    buffer.CopyTo(Mesg, 4);
                    //数据库查询deviceid
                    MainForm.cli.SendMessage(Mesg, 0x0011, "1");


                    dbMySql.Open(databaseName);
                    dbMySql.ExcuteNonQuery(databaseName, strsql);
                    dbMySql.Close(databaseName);
                    MessageBox.Show("配置消息下发成功!");
                    break;
                case 1:
                    //选择了配置照片
                    strsql += " Pic_Width =" + Pic_Width + ", Pic_Heigt =" + Pic_Heigt + " ";
                    strsql += " WHERE Device_ID = '" + Device_ID + "'";
                    PICCONFI picnf = new PICCONFI();
                    picnf.m_width = Pic_Width;
                    picnf.m_height = Pic_Heigt;
                    //转byte
                    object structT = picnf;
                    byte[] picbuff = Info.StructureToByteArrayEndian(structT);
                    sysconf.m_configtype = 1;
                    sysconf.m_confnum = 0;
                    sysconf.m_para = picbuff;


                    //发送一个配置信号;
                    object sysType = sysconf;
                    buffer = Info.StructureToByteArrayEndian(sysType);

                    buf.CopyTo(Mesg, 0);
                    buffer.CopyTo(Mesg, 4);
                    //数据库查询deviceid
                    MainForm.cli.SendMessage(Mesg, 0x0011, "1");
                    dbMySql.Open(databaseName);
                    dbMySql.ExcuteNonQuery(databaseName, strsql);
                    dbMySql.Close(databaseName);
                    MessageBox.Show("配置消息下发成功!");

                    break;

                case 2://配置参数表
            */
                    NumRow = dataGridView1.RowCount;//由于允许用户自己添加最后一行，最后一行数据为空
                    FINFOR_INFO ftinfo = new FINFOR_INFO();
                    sysconf.m_configtype = 2;
                    sysconf.m_confnum = (UInt32)NumRow;
                    for (int i = 0; i < NumRow; i++)
                    {
                        ftinfo.ftesttimes = new byte[50];
                        System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[1].Value.ToString()).CopyTo(ftinfo.ftesttimes, 0);
                        ftinfo.fsample = new byte[100];
                        System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[2].Value.ToString()).CopyTo(ftinfo.fsample,0);
                         ftinfo.wavemode = new byte[4];
                         System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[3].Value.ToString()).CopyTo(ftinfo.wavemode, 0);


                      //  ftinfo.ftesttimes = System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[1].Value.ToString().PadRight(50,'\0'));
                     //   ftinfo.fsample = System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[2].Value.ToString().PadRight(100,'\0'));
                        //ftinfo.wavemode = System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[3].Value.ToString().PadRight(4, '\0'));
                        ftinfo.wavemajor = System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[4].Value.ToString().PadRight(10, '\0'));
                        ftinfo.waveminor = System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[5].Value.ToString().PadRight(10, '\0'));
                        ftinfo.formulaC = System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[6].Value.ToString().PadRight(10, '\0'));
                        ftinfo.formulaB = System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[7].Value.ToString().PadRight(10, '\0'));
                        ftinfo.formulaA = System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[8].Value.ToString().PadRight(10, '\0'));
                        ftinfo.interdilute = System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[9].Value.ToString().PadRight(10, '\0'));
                        ftinfo.unit = System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[10].Value.ToString().PadRight(10, '\0'));
                        ftinfo.standards = System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[11].Value.ToString().PadRight(50, '\0'));
                        ftinfo.comparemode = System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[12].Value.ToString().PadRight(10, '\0'));
                        ftinfo.comparemin = System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[13].Value.ToString().PadRight(10, '\0'));
                        ftinfo.comparemax = System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[14].Value.ToString().PadRight(10, '\0'));
                        ftinfo.tesrangemax = System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[15].Value.ToString().PadRight(10, '\0'));
                        ftinfo.tesrangemin = System.Text.Encoding.UTF8.GetBytes(dataGridView1.Rows[i].Cells[16].Value.ToString().PadRight(10, '\0'));
                        object structinfo = ftinfo;
                        byte[] infobuf = Info.StructureToByteArrayEndian(structinfo);
                        sysconf.m_para = infobuf;

                        //发送一个配置信号;
                        object sysco = sysconf;
                        buffer = Info.StructureToByteArrayEndian(sysco);

                        buf.CopyTo(Mesg, 0);
                        buffer.CopyTo(Mesg, 4);
                        //数据库查询deviceid
                        MainForm.cli.SendMessage(Mesg, 0x0011, "1");
                    }
                    MessageBox.Show("配置消息下发成功!");
// 
//                     break;
//                 default:
//                     //选择了其他的tabPage
//                     break;
//             }


            this.Close();
        }

        private void Resolution_Select(object sender, EventArgs e)
        {
//             if (this.comboBox_reso.Text != "")
//             {
//                 resolution = this.comboBox_reso.Text;
//                 if (this.comboBox_reso.SelectedIndex == 0)
//                 {
//                     vido_width = 320;
//                     vido_width = 240;
//                 }
//                 else
//                 {
//                     vido_width = 640;
//                     vido_width = 480;
//                 }
//             }
        }

        private void Fram_Selectchange(object sender, EventArgs e)
        {
//             if (this.comboBox_frame.Text != "")
//             {
//                 frame = UInt32.Parse(this.comboBox_frame.Text);
//             }
        }

        private void Bit_Selectchange(object sender, EventArgs e)
        {
//             if (this.comboBox_bit.Text != "")
//             {
//                 Bitrate = UInt32.Parse(this.comboBox_bit.Text);
//             }
        }

        private void WidthSlelcted(object sender, EventArgs e)
        {
//             if (this.comboBox5.Text != "")
//             {
//                 Pic_Width = UInt32.Parse(comboBox5.Text);
//             }
        }

        private void Heig(object sender, EventArgs e)
        {
//             if (this.comboBox6.Text != "")
//             {
//                 Pic_Heigt = UInt32.Parse(comboBox6.Text);
//             }
        }

        private void FormSele(object sender, EventArgs e)
        {
//             if (this.comboBox_form.Text != "")
//             {
//                 vidio_form = comboBox_form.Text;
//             }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //pnlBottom.Enabled = true;
            OldRow = dataGridView1.RowCount;
            //获得当前选中的行 
            int i = e.RowIndex;
            tinfoid = dataGridView1.Rows[i].Cells[0].Value.ToString();

            try
            {
                ftinfostr.ftesttimes = dataGridView1.Rows[i].Cells[1].Value.ToString();
                ftinfostr.fsample = dataGridView1.Rows[i].Cells[2].Value.ToString();
                ftinfostr.wavemode = dataGridView1.Rows[i].Cells[3].Value.ToString();
                ftinfostr.wavemajor = dataGridView1.Rows[i].Cells[4].Value.ToString();
                ftinfostr.waveminor = dataGridView1.Rows[i].Cells[5].Value.ToString();
                ftinfostr.formulaC = dataGridView1.Rows[i].Cells[6].Value.ToString();
                ftinfostr.formulaB = dataGridView1.Rows[i].Cells[7].Value.ToString();
                ftinfostr.formulaA = dataGridView1.Rows[i].Cells[8].Value.ToString();
                ftinfostr.interdilute = dataGridView1.Rows[i].Cells[9].Value.ToString();
                ftinfostr.unit = dataGridView1.Rows[i].Cells[10].Value.ToString();
                ftinfostr.standards = dataGridView1.Rows[i].Cells[11].Value.ToString();
                ftinfostr.comparemode = dataGridView1.Rows[i].Cells[12].Value.ToString();
                ftinfostr.comparemin = dataGridView1.Rows[i].Cells[13].Value.ToString();
                ftinfostr.comparemax = dataGridView1.Rows[i].Cells[14].Value.ToString();
                ftinfostr.tesrangemax = dataGridView1.Rows[i].Cells[15].Value.ToString();
                ftinfostr.tesrangemin = dataGridView1.Rows[i].Cells[16].Value.ToString();

            }
            catch (Exception exc) { }

        }

        private void TabSelcetChang(object sender, EventArgs e)
        {

        }
        //得到DataGridView当前行(按照个人习惯，将实际的行号从零开始)：
        public int getRow()
        {
            if (this.dataGridView1.Rows.Count > 0 && this.dataGridView1.CurrentRow.Index >= 0)
                return this.dataGridView1.CurrentRow.Index;
            else
                return 0;
        }
        //删除DataGridView指定行(传入的row是实际行号加1，还是个人习惯)：
        public void deleteRow(int row)
        {
            if (row < 0 || row >= this.dataGridView1.Rows.Count)
                return;
            //用DataGridViewrow的DataBoundItem属性得到当前绑定的原始数据行，是一个DataRowview对象
            //再用这个对象得到对应的DataRow
            //(this.dataGridView1.Rows[row].Cells[0].Value as DataRowView).Row.Delete();
            //  (dataGridView1.Rows[row].DataBoundItem as DataRowView).Row.Delete();
            this.dataGridView1.Rows.Remove(this.dataGridView1.Rows[row]);
        }
        private void button_Del_Click(object sender, EventArgs e)
        {
            int rowNum = getRow();
            if (dataGridView1.CurrentCell.RowIndex >= 0)
            {
                if (MessageBox.Show("你确定要删除吗？", "确定", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string sql = "DELETE FROM ftinfor  WHERE id = '" + tinfoid + "' ";

                    dbMySql.Open(databaseName);
                    dbMySql.ExcuteNonQuery(databaseName, sql);
                    dbMySql.Close(databaseName);
                    deleteRow(rowNum);
                    dataGridView1.Update();
                }
                else
                {

                    MessageBox.Show("没有选中的记录，请选择!");

                }
            }
        }

        private void button_Add_Click(object sender, EventArgs e)
        {

        }
    }
        #endregion
}
