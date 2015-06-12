using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GHCS;
using GHCS.DataBase;
using GHCS.Common;

namespace FoodServer.Query
{
    public partial class DetectQuray : Form
    {
        //创建数据库的对象
        IDataBase database = MySqlDataBase.getInstance();
        //文件导出数据的对象
        FileOperations fileOperate = new FileOperations();

        //控件中值的数组
        string[] condition = { "", "", "", "" };

        //枚举控件的编号
        private enum control
        {
            detectProj,//检测项目名称
            Test_Unit,//检测单位
            dataTimePicker1,//记录时间
            dataTimePicker2//记录时间
        }
        string sqlInit;
        string Device_Id;
        Page page1 = new Page();

        bool Is_Qure = false;
        public DetectQuray()
        {
            InitializeComponent();
            Is_Qure = false;
        }
        public DetectQuray(string userid)
        {
            InitializeComponent();
            Is_Qure = true;
            Device_Id = userid;
        }
        /// <summary>
        /// 读取数据的条数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private int ReadDataCount(string sql)
        {
            int value = 0;
            List<string> list = new List<string>();

            database.QueryTable(sql, list, 1);
            if (list.Count > 0)
            {
                value = System.Convert.ToInt32(list[0]);
            }

            return value;

        }

          /// </summary>
        private void LogResultHeader()
        {
            if (this.dataGridView4.ColumnCount > 0)
            {

                this.dataGridView4.Columns[0].HeaderText = "检测项目名称";
                this.dataGridView4.Columns[0].Width = 80;

                this.dataGridView4.Columns[1].HeaderText = "样品类型";
                this.dataGridView4.Columns[1].Width = 80;

                this.dataGridView4.Columns[2].HeaderText = "测量通道";
                this.dataGridView4.Columns[2].Width = 80;

                this.dataGridView4.Columns[3].HeaderText = "判定结果";
                this.dataGridView4.Columns[3].Width = 80;
                this.dataGridView4.Columns[4].HeaderText = "测量结果";
                this.dataGridView4.Columns[4].Width = 80;

                this.dataGridView4.Columns[5].HeaderText = "结果单位";
                this.dataGridView4.Columns[5].Width = 80;

                this.dataGridView4.Columns[6].HeaderText = "参考标准";
                this.dataGridView4.Columns[6].Width = 80;

                this.dataGridView4.Columns[7].HeaderText = "吸光度结果";
                this.dataGridView4.Columns[7].Width = 80;

                this.dataGridView4.Columns[8].HeaderText = "样品编号";
                this.dataGridView4.Columns[8].Width = 80;

                this.dataGridView4.Columns[9].HeaderText = "样品名称";
                this.dataGridView4.Columns[9].Width = 80;

                this.dataGridView4.Columns[10].HeaderText = "产地";
                this.dataGridView4.Columns[10].Width = 80;

                this.dataGridView4.Columns[11].HeaderText = "送检单位";
                this.dataGridView4.Columns[11].Width = 80;

                this.dataGridView4.Columns[12].HeaderText = "操作人员";
                this.dataGridView4.Columns[12].Width = 80;

                this.dataGridView4.Columns[13].HeaderText = "测试单位";
                this.dataGridView4.Columns[13].Width = 80;

                this.dataGridView4.Columns[14].HeaderText = "测试时间";
                this.dataGridView4.Columns[14].Width = 80;

                this.dataGridView4.Columns[15].HeaderText = "检测仪型号";
                this.dataGridView4.Columns[15].Width = 80;
                this.dataGridView4.Columns[16].HeaderText = "检测仪版本";
                this.dataGridView4.Columns[16].Width = 80;

                this.dataGridView4.Columns[17].Visible = false;

            }
            else
            {
                return;
            }
        }
        
        /// <summary>
        /// 列标题
        /// </summary>
        private void LogHeader()
        {
            if (this.dataGridView1.ColumnCount > 0)
            {

                this.dataGridView1.Columns[0].HeaderText = "检测项目名称";
                this.dataGridView1.Columns[0].Width = 80;

                this.dataGridView1.Columns[1].HeaderText = "样品类型";
                this.dataGridView1.Columns[1].Width = 80;

                this.dataGridView1.Columns[2].HeaderText = "测量通道";
                this.dataGridView1.Columns[2].Width = 80;

                this.dataGridView1.Columns[3].HeaderText = "判定结果";
                this.dataGridView1.Columns[3].Width = 80;
                this.dataGridView1.Columns[4].HeaderText = "测量结果";
                this.dataGridView1.Columns[4].Width = 80;

                this.dataGridView1.Columns[5].HeaderText = "结果单位";
                this.dataGridView1.Columns[5].Width = 80;

                this.dataGridView1.Columns[6].HeaderText = "参考标准";
                this.dataGridView1.Columns[6].Width = 80;

                this.dataGridView1.Columns[7].HeaderText = "吸光度结果";
                this.dataGridView1.Columns[7].Width = 80;

                this.dataGridView1.Columns[8].HeaderText = "样品编号";
                this.dataGridView1.Columns[8].Width = 80;

                this.dataGridView1.Columns[9].HeaderText = "样品名称";
                this.dataGridView1.Columns[9].Width = 80;

                this.dataGridView1.Columns[10].HeaderText = "产地";
                this.dataGridView1.Columns[10].Width = 80;

                this.dataGridView1.Columns[11].HeaderText = "送检单位";
                this.dataGridView1.Columns[11].Width = 80;

                this.dataGridView1.Columns[12].HeaderText = "操作人员";
                this.dataGridView1.Columns[12].Width = 80;

                this.dataGridView1.Columns[13].HeaderText = "测试单位";
                this.dataGridView1.Columns[13].Width = 80;

                this.dataGridView1.Columns[14].HeaderText = "测试时间";
                this.dataGridView1.Columns[14].Width = 80;

                this.dataGridView1.Columns[15].HeaderText = "检测仪型号";
                this.dataGridView1.Columns[15].Width = 80;
                this.dataGridView1.Columns[16].HeaderText = "检测仪版本";
                this.dataGridView1.Columns[16].Width = 80;

                this.dataGridView1.Columns[17].Visible = false;

            }
            else
            {
                return;
            }
        }
        /// <summary>
        /// 生成sql语句的函数
        /// </summary>
        /// <param name="i">控件的编号</param>
        /// <returns></returns>

        private string CreateSql(int i)
        {
            //根据字符串生成sql语句
            string temp = null;

            switch (i)
            {
                case (int)control.detectProj:

                    temp = "detectioninfo.p_name='" + condition[(int)control.detectProj] + "'";
                    break;

                case (int)control.dataTimePicker1:

                    temp = "detectioninfo.test_time between '" + condition[(int)control.dataTimePicker1] + "'";
                    break;

                case (int)control.dataTimePicker2:

                    temp = "'" + condition[(int)control.dataTimePicker2] + "'";
                    break;

                case (int)control.Test_Unit:

                    temp = "detectioninfo.test_unit='" + condition[(int)control.Test_Unit] + "'";
                    break;
            }
            return temp;
        }
        /// <summary>
        /// 初始化控件
        /// </summary>
        private void Init()
        {
            ComboBoxDataBase boxDataBase = new ComboBoxDataBase();

            string sql = "SELECT DISTINCT p_name FROM detectioninfo";
            boxDataBase.BindDataBase(ref this.deviceNum, ref database, sql);

            sql = "SELECT DISTINCT Company FROM user";
            boxDataBase.BindDataBase(ref this.siteNum, ref database, sql);

            //时间控件的初始化
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy-MM-dd";
            dateTimePicker1.Text = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")).AddMonths(-1).ToShortDateString();
            condition[(int)control.dataTimePicker1] = dateTimePicker1.Text;

            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "yyyy-MM-dd";

            condition[(int)control.dataTimePicker2] = dateTimePicker2.Value.ToString("yyyy-MM-dd HH:mm:ss");

            //comboBox控件设置
            deviceNum.Text = "";
            siteNum.Text = "";

        }
        string sqlModel;
        string sqlModelCount;
        private void selectLog_Click(object sender, EventArgs e)
        {
            //查询数据按键
            int mcount = 0;//统计条件的个数
            int i;

            if (Is_Qure)
            {
                sqlModel = "SELECT detectioninfo.p_name," +
                                    "detectioninfo.type,detectioninfo.channel,detectioninfo.Juge_result,detectioninfo.Detect_result,detectioninfo.result_unit," +
                                    "detectioninfo.standard,detectioninfo.Absolut_result,detectioninfo.sample_no,detectioninfo.sites,detectioninfo.sample_name,detectioninfo.submission_unit,test_operator,test_unit," +
                                    "test_time,device.Device_Type,Device.Device_Ver ,device.Device_ID FROM ( detectioninfo INNER JOIN device ON detectioninfo.Device_ID = device.Device_ID  AND device.Device_ID ='" + Device_Id + "')";

                sqlModelCount = "SELECT COUNT(*)FROM ( detectioninfo INNER JOIN device ON detectioninfo.Device_ID = device.Device_ID  AND device.Device_ID ='" + Device_Id + "')";

            }
            else
            {
                sqlModel = "SELECT detectioninfo.p_name," +
                     "detectioninfo.type,detectioninfo.channel,detectioninfo.Juge_result,detectioninfo.Detect_result,detectioninfo.result_unit," +
                     "detectioninfo.standard,detectioninfo.Absolut_result,detectioninfo.sample_no,detectioninfo.sites,detectioninfo.sample_name,detectioninfo.submission_unit,test_operator,test_unit," +
                     "test_time,device.Device_Type,Device.Device_Ver ,device.Device_ID FROM ( detectioninfo INNER JOIN device ON detectioninfo.Device_ID = device.Device_ID  )";

                sqlModelCount = "SELECT COUNT(*)FROM ( detectioninfo INNER JOIN device ON detectioninfo.Device_ID = device.Device_ID  )";

            }


            //检查comboBox是空的情况
            if (deviceNum.Text.Equals(""))
            {
                condition[(int)control.detectProj] = deviceNum.Text;
            }
            if (siteNum.Text.Equals(""))
            {
                condition[(int)control.Test_Unit] = siteNum.Text;
            }


            //根据复选框的条件来拼接sql语句，默认情况下时间控件的值是当天的时间值
            for (i = 0; i < condition.Length; i++)
            {
                if (condition[i].Equals(""))
                {
                    continue;

                }
                else
                {

                    if (mcount == 0)
                    {
                        sqlModel += " WHERE ";
                        sqlModelCount += " WHERE ";
                        sqlModel += CreateSql(i);

                        sqlModelCount += CreateSql(i);


                    }
                    else
                    {
                        sqlModel += " and ";
                        sqlModelCount += " and ";
                        sqlModel += CreateSql(i);
                        sqlModelCount += CreateSql(i);

                    }
                    mcount++;
                }
            }


            page1.nMax = ReadDataCount(sqlModelCount);//得到获取数据的条数

            page1.InitDataSet();//初始化分页的全局变量

            sqlModel += " ORDER BY test_time desc ";
            sqlInit = sqlModel;
            page1.LoadData(dataGridView1, bindingSource1, bindingNavigator1, sqlInit, page1);
            LogHeader();
            txtCurrentPage.Text = Convert.ToString(page1.pageCurrent);//当前页数
            totalPageCount.Text = Convert.ToString(page1.pageCount);//总页数
        }

        private void Quaru_Load(object sender, EventArgs e)
        {
            // 创建数据库连接
            database.CreateConnection(null);
            //database.Open();

            Init();
            sqlInit = "SELECT detectioninfo.p_name," +
                "detectioninfo.type,detectioninfo.channel,detectioninfo.Juge_result,Detect_result,result_unit," +
                "standard,Absolut_result,sample_no,sites,sample_name,submission_unit,test_operator,test_unit," +
                "test_time,device.Device_Type,Device.Device_Ver ,device.Device_ID FROM ( detectioninfo INNER JOIN device ON detectioninfo.Device_ID = device.Device_ID AND device.Device_ID ='" + Device_Id + "')";
            string sqlCount = "SELECT COUNT(*)FROM ( detectioninfo INNER JOIN device ON detectioninfo.Device_ID = device.Device_ID AND device.Device_ID ='" + Device_Id + "')";

            page1.pageSize = 20;//每页显示的条数
            page1.nMax = ReadDataCount(sqlCount);

            if (page1.nMax < page1.pageSize)
                toolStripLabel2.Text = "/" + Convert.ToString(page1.nMax);

            page1.InitDataSet();
            page1.LoadData(dataGridView1, bindingSource1, bindingNavigator1, sqlInit, page1);
            LogHeader();
            txtCurrentPage.Text = Convert.ToString(page1.pageCurrent);//当前页数
            totalPageCount.Text = Convert.ToString(page1.pageCount);//总页数
        }

        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            condition[(int)control.dataTimePicker1]
           = dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void DateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            condition[(int)control.dataTimePicker2]
               = dateTimePicker2.Value.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void DeviceNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            condition[(int)control.detectProj] = deviceNum.Text;
        }

        private void SiteNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            condition[(int)control.Test_Unit] = siteNum.Text;
        }

        private void BindingNavigator1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "上一页")
            {
                page1.pageCurrent--;
                if (page1.pageCurrent <= 0)
                {
                    MessageBox.Show("已经是第一页，请点击“下一页”查看！",
                        "提示信息", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                    return;
                }
                else
                {
                    page1.nCurrent = page1.pageSize * (page1.pageCurrent - 1);
                }

                page1.LoadData(dataGridView1, bindingSource1, bindingNavigator1, sqlInit, page1);

            }
            if (e.ClickedItem.Text == "下一页")
            {
                page1.pageCurrent++;
                if (page1.pageCurrent > page1.pageCount || page1.pageCurrent == 0)
                {
                    MessageBox.Show("已经是最后一页，请点击“上一页”查看！",
                        "提示信息", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);


                    return;
                }
                else
                {
                    page1.nCurrent = page1.pageSize * (page1.pageCurrent - 1);

                }

                page1.LoadData(dataGridView1, bindingSource1, bindingNavigator1, sqlInit, page1);

            }
            if (e.ClickedItem.Text == "首页")
            {

                if (page1.pageCount <= 0)
                {
                    page1.pageCurrent = 0;//当没有记录时当前页数从0开始
                    page1.nCurrent = 0;
                }
                else
                {
                    page1.pageCurrent = 1;//当前页数从1开始
                    page1.nCurrent = page1.pageSize * (page1.pageCurrent - 1);
                }

                page1.LoadData(dataGridView1, bindingSource1, bindingNavigator1, sqlInit, page1);

            }
            if (e.ClickedItem.Text == "尾页")
            {
                page1.pageCurrent = page1.pageCount;

                if (page1.pageCount == 0)
                {
                    page1.nCurrent = 0;
                }
                else
                {

                    page1.nCurrent = page1.pageSize * (page1.pageCurrent - 1);
                }

                page1.LoadData(dataGridView1, bindingSource1, bindingNavigator1, sqlInit, page1);

            }
            if (e.ClickedItem.Text == "跳转")
            {
                page1.pageCurrent = Convert.ToInt32(txtCurrentPage.Text);
                if (page1.pageCurrent > page1.pageCount || page1.pageCurrent == 0)
                {
                    MessageBox.Show("没有此页数据！", "提示信息",
                        MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                    return;
                }
                else
                {
                    page1.nCurrent = page1.pageSize * (page1.pageCurrent - 1);

                }


                page1.LoadData(dataGridView1, bindingSource1, bindingNavigator1, sqlInit, page1);

            }
            if (e.ClickedItem.Text == "清空记录")
            {
                page1.pageCurrent = 0;
                page1.pageCount = 0;
                string sql = "delete from controllog";

                DialogResult result = MessageBox.Show("你确定清空吗！", "提示信息", MessageBoxButtons.OKCancel,
              MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    database.ExcuteNonQuery(sql);
                    page1.pageSize = 20;//每页显示的条数
                    page1.nMax = 0;
                    page1.InitDataSet();
                    page1.LoadData(dataGridView1, bindingSource1, bindingNavigator1, sqlInit, page1);
                    LogHeader();
                    txtCurrentPage.Text = Convert.ToString(page1.pageCurrent);//当前页数
                    totalPageCount.Text = Convert.ToString(page1.pageCount);//总页数
                    MessageBox.Show("所有记录全部清空！", "成功", MessageBoxButtons.OK,
              MessageBoxIcon.Information);
                }
                else
                {
                    return;
                }

            }
            if (e.ClickedItem.Text == "导出记录")
            {


                //string fileName = "未命名";
                //fileOperate.DataBaseToExcel1(dataGridView1, sqlInit, fileName);

                string fileName = "未命名";
                //fileOperate.DataBaseToExcel1(dataGridView1,sqlInit,fileName);

                DataSet ds = new DataSet();

                database.ReadDataBase(sqlInit, "historydata", ds);
                System.Data.DataTable dtInfo = new System.Data.DataTable();
                dtInfo = ds.Tables["historydata"];

                if (dtInfo.Rows.Count <= 0)
                {
                    MessageBox.Show("没有数据！！！", "提示信息",
                        MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                    return;
                }

                dtInfo.Columns[0].ColumnName = "检测开始时间";
                dtInfo.Columns[1].ColumnName = "检测结束时间";
                dtInfo.Columns[2].ColumnName = "检测项目名称";
                dtInfo.Columns[3].ColumnName = "样品类型";
                dtInfo.Columns[4].ColumnName = "测量通道";
                dtInfo.Columns[5].ColumnName = "判定结果";
                dtInfo.Columns[6].ColumnName = "测量结果";
                dtInfo.Columns[7].ColumnName = "结果单位";
                dtInfo.Columns[8].ColumnName = "参考标准";
                dtInfo.Columns[9].ColumnName = "吸光度结果";
                dtInfo.Columns[10].ColumnName = "样品编号";
                dtInfo.Columns[11].ColumnName = "产地";
                dtInfo.Columns[12].ColumnName = "样品名称";
                dtInfo.Columns[13].ColumnName = "送检单位";
                dtInfo.Columns[14].ColumnName = "操作人员";
                dtInfo.Columns[15].ColumnName = "测试单位";
                dtInfo.Columns[16].ColumnName = "测试时间";
                dtInfo.Columns[17].ColumnName = "检测仪编号";
               // dtInfo.Columns[18].ColumnName = "检测仪编号";

                string saveFileName = "";
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.DefaultExt = "xls";
                saveDialog.Filter = "Excel文件|*.xls";
                saveDialog.FileName = fileName;
                saveDialog.ShowDialog();
                saveFileName = saveDialog.FileName;

                if (saveFileName.IndexOf(":") < 0)
                    return;

                ExcelHelper.Export(dtInfo, "历史数据", saveFileName);


                MessageBox.Show("导出完毕", "提示信息",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);



            }

            txtCurrentPage.Text = Convert.ToString(page1.pageCurrent);//当前页数
            totalPageCount.Text = Convert.ToString(page1.pageCount);//总页数
        }

#region 添加网址
//       //  首先在原有列上增加一列DataGridViewLinkColumn
//     private void AddLinkColumn()
//         {
//             DataGridViewLinkColumn links = new DataGridViewLinkColumn();
//             links.HeaderText = "网址";
//             links.DataPropertyName = "网址";
//             links.ActiveLinkColor = Color.White;
//             links.LinkBehavior = LinkBehavior.SystemDefault;
//             links.LinkColor = Color.Blue;
//             links.TrackVisitedState = true;
//             links.VisitedLinkColor = Color.YellowGreen;
//             dataGridView1.Columns.Add(links);
//         }

 
#endregion

        private void GradView1DoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = dataGridView1.CurrentCell.RowIndex;
            DataTable tb1 = (DataTable)bindingSource1.DataSource;
            DataTable tb2 = new DataTable();
            tb2 = tb1.Copy();
            tb2.Clear();
            DataRow dr = tb2.NewRow();
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                dr[i] = dataGridView1.Rows[index].Cells[i].Value;
            }
            int Deviceid = (int)dataGridView1.Rows[index].Cells["Device_ID"].Value;
            tb2.Rows.Add(dr);
            this.dataGridView4.DataSource = tb2;
            LogResultHeader();
            
            this.dataGridView4.Update();

            //照片2
            DataSet datese = new DataSet();
            string sql = " SELECT detectioninfo.batch_id, detectioninfo.task_id,picinfo.Pic_Name FROM detectioninfo,picinfo WHERE detectioninfo.Device_ID = picinfo.Device_ID AND detectioninfo.Device_ID = '"+Deviceid+"'";
            database.ReadDataBase(sql, "history", datese); 
            dataGridView2.DataSource = datese.Tables["history"];
            foreach (DataRow r in datese.Tables["history"].Rows)
            {
                byte[] batchid = System.Text.Encoding.UTF8.GetBytes(r[0].ToString());
                byte[] taskid = System.Text.Encoding.UTF8.GetBytes(r[1].ToString());
                byte[] device_id = System.Text.Encoding.UTF8.GetBytes(Device_Id);
                byte[] msg = new byte[batchid.Length + taskid.Length + device_id.Length];
                device_id.CopyTo(msg, 0);
                batchid.CopyTo(msg, device_id.Length);
                taskid.CopyTo(msg, device_id.Length + batchid.Length);
                //MainForm.cli.SendMessage(msg, 0x000d, "1");
            }

            database.ReadDataBase(sql, "history", datese);
            dataGridView2.DataSource = datese.Tables["history"];
            dataGridView2.Update();
            dataGridView2.Columns[0].HeaderText = "批次ID";
            dataGridView2.Columns[1].HeaderText = "任务ID";
            dataGridView2.Columns[2].HeaderText = "照片位置";
            dataGridView2.Columns[2].Width = 250;

            //视频3
            DataSet datese2 = new DataSet();
            sql = "SELECT batch_id, task_id,video_path FROM detectioninfo WHERE  Device_ID = '" + Deviceid + "'";

            database.ReadDataBase(sql, "historyd", datese2);
            dataGridView3.DataSource = datese2.Tables["historyd"];
            //发送消息到终端


            database.ReadDataBase(sql, "historyd", datese2);
            dataGridView3.DataSource = datese2.Tables["historyd"];
            dataGridView3.Update();
            dataGridView3.Columns[0].HeaderText = "批次ID";
            dataGridView3.Columns[1].HeaderText = "任务ID";
            dataGridView3.Columns[2].HeaderText = "视频位置";
            
            dataGridView3.Columns[2].Width = 250;

            this.tabControl1.SelectedIndex = 1;
        }
        private string GetHttp()
        {
            string htp;
            IniAc ini = new IniAc();
            string ipAddrStr = ini.ReadValue("NET", "ip");

            string portStr = ini.ReadValue("NET", "port");
            htp = "http://"+ipAddrStr+"/";
      
            return htp;
        }
        private void PicContetClick(object sender, DataGridViewCellEventArgs e)
        {
           if (e.ColumnIndex == 2)
            {
                System.Diagnostics.Process.Start("iexplore.exe", GetHttp()+this.dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
            }
        }

        private void VidioContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                System.Diagnostics.Process.Start("iexplore.exe", GetHttp()+this.dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
            }
        }

        private void exportLog_Click(object sender, EventArgs e)
        {

        }



    }
}
