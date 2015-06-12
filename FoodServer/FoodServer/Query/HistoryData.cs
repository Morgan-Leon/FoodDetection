using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using MySql.Data.MySqlClient;
using GHCS.DataBase;
using GHCS;
using GHCS.Common;

namespace FoodServer.Query
{
    public partial class HistoryData : Form
    {
        private int pageCount = 1;

        private List<HistoryCurve> hcList = new List<HistoryCurve>();
        private HistoryCurve hc = null;

        //创建数据库的对象
        IDataBase database = MySqlDataBase.getInstance();

        //文件导入导出对象
        FileOperations fileOperate = new FileOperations();

        int processIncrease = 0;
        //控件中值的数组
        string[] condition = { "", "", "", "", "", "", "" };
        string[] condition1 = { "", "", "", "", "", "", "" };
        string test_uint = "";//企业名称

        //曲线名称标志
        int[] lineNameFlag = { 0, 0, 0, 0, 0, 0, 0, };//检测结果数，合格率，检测结果平均值，最大值,最小值
        int[] lineNameFlag1 = { 0, 0, 0, 0, 0, 0, 0 };
        Color[] lineColorFlag = { Color.Red, Color.Blue, Color.Green, Color.GreenYellow, Color.HotPink, Color.LightBlue, Color.LightGreen };

        //枚举控件的编号
        private enum control
        {
            proName,//检测项目
            testUnit,//送检单位
            testType,//样品类型
            comany_Class,//企业分类
            UbsubUnit,//检测单位
            dataTimePicker1,//采集时间
            dataTimePicker2,//采集时间
            dataTimePicker3,//采集时间
            dataTimePicker4//采集时间
        }

        Page page1 = new Page();
        string sqlInit;


        Page page2 = new Page();
        string sqlInit1;

        // Page page3 = new Page();
        string sqlInit2;
        //page2中的两个时间
        string fromdate;
        string todate;
        //绘图时颜色的变化
        //Color[] color = { Color.Blue, Color.Red, Color.Green, Color.Gray, Color.IndianRed };
        //int flag = 0;
        //int flag1 = 0;

        public HistoryData()
        {
            InitializeComponent();
        }

        private void historyForm_Load(object sender, EventArgs e)
        {
            // 创建数据库连接
            database.CreateConnection(null);
            //database.Open();

            //控件绑定数据并且初始化控件
            Init();

            //加载企业排名头
            LoadHeader();

            zedGraphControl1.ContextMenuBuilder += MyContextMenuBuilder;
            InitLine(zedGraphControl1);
            zedGraphControl1.PanModifierKeys = Keys.None;

        }

        /// <summary>
        /// 初始化控件和控件绑定数据
        /// </summary>
        private void Init()
        {
            //comboBox控件绑定数据
            ComboBoxDataBase boxDataBase = new ComboBoxDataBase();

            string sql = null;
            sql = "SELECT DISTINCT p_name FROM detectioninfo";
            boxDataBase.BindDataBase(ref this.ProName, ref database, sql);

            sql = "SELECT DISTINCT Company_Name FROM companyinfo";
            boxDataBase.BindDataBase(ref this.TestUnit, ref database, sql);

            sql = "SELECT DISTINCT type FROM detectioninfo ";
            boxDataBase.BindDataBase(ref this.TestType, ref database, sql);



            //企业排名
            sql = "SELECT DISTINCT type FROM detectioninfo";
            boxDataBase.BindDataBase(ref this.comboBox_style, ref database, sql);

            sql = "SELECT DISTINCT p_name FROM detectioninfo";
            boxDataBase.BindDataBase(ref this.detect_Pro, ref database, sql);

            sql = "SELECT DISTINCT Company_Class FROM companyinfo ";
            boxDataBase.BindDataBase(ref this.collect_class, ref database, sql);



            //时间控件的初始化
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy-MM-dd";
            condition[(int)control.dataTimePicker1] = dateTimePicker1.Text;


            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "yyyy-MM-dd";
            condition[(int)control.dataTimePicker2] = dateTimePicker2.Value.ToString();

            //时间控件的初始化
            dateTimePicker3.Format = DateTimePickerFormat.Custom;
            dateTimePicker3.CustomFormat = "yyyy-MM-dd";
            condition1[(int)control.dataTimePicker1] = dateTimePicker3.Text;


            dateTimePicker4.Format = DateTimePickerFormat.Custom;
            dateTimePicker4.CustomFormat = "yyyy-MM-dd";
            condition1[(int)control.dataTimePicker2] = dateTimePicker4.Value.ToString();


            dateTimePicker7.Format = DateTimePickerFormat.Custom;
            dateTimePicker7.CustomFormat = "yyyy-MM-dd";



            dateTimePicker8.Format = DateTimePickerFormat.Custom;
            dateTimePicker8.CustomFormat = "yyyy-MM-dd";
            fromdate = dateTimePicker7.Text;
            todate = dateTimePicker8.Value.ToString(); ;


            //comboBox控件设置

            ProName.Text = "";
            TestUnit.Text = "";
            TestType.Text = "";

            //comboBox控件设置



        }


        #region "采集器界面"

        private void CreateHistoryDataHeader()
        {
            List<string> list = new List<string>(6);
            list.Add("场地");
            list.Add("采集器");
            list.Add("采集数据类型");
            list.Add("采集数据值");
            list.Add("单位");
            list.Add("采集数据时间");


            //             this.dataGridView1.Rows.Clear();
            //             this.dataGridView1.Columns.Clear();
            //             for (int j = 0; j < 6; j++)
            //             {
            //                 DataGridViewTextBoxColumn dgv = new DataGridViewTextBoxColumn();
            //                 this.dataGridView1.Columns.Add(dgv);
            //                 dgv.Name = list[j];
            //             }

        }
        /// <summary>
        /// dataGridView标题
        /// </summary>
        private void HistoryDataHeader()
        {
            //             if (this.dataGridView1.ColumnCount > 0)
            //             {
            //                 this.dataGridView1.Columns[0].HeaderText = "场地";
            //                 //this.dataGridView1.Columns[0].Width = 130;
            //                 this.dataGridView1.Columns[1].HeaderText = "采集器";
            //                 //this.dataGridView1.Columns[1].Width = 100;
            //                 this.dataGridView1.Columns[2].HeaderText = "采集数据类型";
            //                 //this.dataGridView1.Columns[2].Width = 120;
            //                 this.dataGridView1.Columns[3].HeaderText = "采集数据值";
            //                 //this.dataGridView1.Columns[3].Width = 120;
            //                 this.dataGridView1.Columns[4].HeaderText="单位";
            //                 //this.dataGridView1.Columns[4].Width = 60;
            //                 this.dataGridView1.Columns[5].HeaderText = "采集数据时间";
            //                 //this.dataGridView1.Columns[5].Width = 200;
            //                 this.dataGridView1.Columns[5].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss ";
            //             }
            //             else
            //             {
            //                 return;
            //             }
        }

        /// <summary>
        /// 翻页控件的按键监听
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*  private void bindingNavigator1_ItemClicked_1(object sender, ToolStripItemClickedEventArgs e)
          {
              if (e.ClickedItem.Text == "上一页")
              {
                  page1.pageCurrent--;
                  if (page1.pageCurrent <= 0)
                  {
                    
                      MessageBox.Show("已经是第一页，请点击“下一页”查看！","提示信息"
                          , MessageBoxButtons.OK,
                      MessageBoxIcon.Warning);


                      return;
                  }
                  else
                  {
                      page1.nCurrent = page1.pageSize * (page1.pageCurrent - 1);
                  }

                  //page1.LoadData(dataGridView1, bindingSource1, bindingNavigator1, sqlInit,page1);
                 // page1.LoadData(dataGridView1, sqlInit, 6, page1);
              }
              if (e.ClickedItem.Text == "下一页")
              {
                  page1.pageCurrent++;
                  if (page1.pageCurrent > page1.pageCount)
                  {
                      MessageBox.Show("已经是最后一页，请点击“上一页”查看！", "提示信息"
                          , MessageBoxButtons.OK,
                      MessageBoxIcon.Warning);


                      return;
                  }
                  else
                  {
                      page1.nCurrent = page1.pageSize * (page1.pageCurrent - 1);

                  }

                  //page1.LoadData(dataGridView1, bindingSource1, bindingNavigator1, sqlInit,page1);
                //  page1.LoadData(dataGridView1, sqlInit, 6, page1);
               
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

                  //page1.LoadData(dataGridView1, bindingSource1, bindingNavigator1, sqlInit,page1);
                 // page1.LoadData(dataGridView1, sqlInit, 6, page1);
               
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
                
                  //page1.LoadData(dataGridView1, bindingSource1, bindingNavigator1, sqlInit,page1);
                //  page1.LoadData(dataGridView1, sqlInit, 6, page1);
               
              }
              if (e.ClickedItem.Text == "跳转")
              {
                  page1.pageCurrent = Convert.ToInt32(txtCurrentPage.Text);
                  if (page1.pageCurrent > page1.pageCount||page1.pageCurrent==0)
                  {
                      MessageBox.Show("没有此页数据！", "提示信息"
                          , MessageBoxButtons.OK,
                      MessageBoxIcon.Warning);

                      return;
                  }
                  else
                  {
                      page1.nCurrent = page1.pageSize * (page1.pageCurrent - 1);

                  }

                  //page1.LoadData(dataGridView1, bindingSource1, bindingNavigator1, sqlInit,page1);
                  page1.LoadData(dataGridView1, sqlInit, 6, page1);
               
              }
              txtCurrentPage.Text = Convert.ToString(page1.pageCurrent);//当前页数
              totalPageCount.Text = Convert.ToString(page1.pageCount);//总页数
          }
          */






        /// <summary>
        /// 采集时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                MessageBox.Show("日期不合理，请重新填写！！！", "警告", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }
            condition[(int)control.dataTimePicker1] = dateTimePicker1.Text;

        }

        /// <summary>
        /// 到采集时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

            if (dateTimePicker2.Value < dateTimePicker1.Value)
            {
                MessageBox.Show("日期不合理，请重新填写！！！", "警告", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            condition[(int)control.dataTimePicker2] = dateTimePicker2.Value.ToString();
        }

        /// <summary>
        /// 清除画板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearPanelClick(object sender, EventArgs e)
        {
            zedGraphControl1.GraphPane.CurveList.Clear(); //清除画板
            Refresh();

            pageCount = 1;
            hcList.Clear();
        }


        /// <summary>
        /// 导出数据的按键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportData_Click(object sender, EventArgs e)
        {
            string fileName = "未命名";
            //fileOperate.DataBaseToExcel1(dataGridView1,sqlInit,fileName);

            DataSet ds = new DataSet();

            database.ReadDataBase(sqlInit, "historydata", ds);
            System.Data.DataTable dtInfo = new System.Data.DataTable();
            dtInfo = ds.Tables["historydata"];

            if (dtInfo.Rows.Count <= 0)
            {
                MessageBox.Show("没有数据！！！", "提示信息", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            dtInfo.Columns[0].ColumnName = "场地";
            dtInfo.Columns[1].ColumnName = "采集器";
            dtInfo.Columns[2].ColumnName = "采集数据类型";
            dtInfo.Columns[3].ColumnName = "采集数据值";
            dtInfo.Columns[4].ColumnName = "单位";
            dtInfo.Columns[5].ColumnName = "采集数据时间";



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


            MessageBox.Show("导出完毕", "提示信息", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);


        }



        /// <summary>
        /// 跳转到实时数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMonitor_Click(object sender, EventArgs e)
        {
            //             if (MainForm.monitorForm == null)
            //             {             
            //                 MainForm.monitorForm = new MonitorForm();
            //                 MainForm.monitorForm.Show();
            //             }
            //             else
            //             {
            //                 MainForm.monitorForm.Show();
            //                 MainForm.monitorForm.Focus();
            //             }
        }


        #endregion

        #region "气象站界面"











        /// <summary>
        /// 气象站数据采集时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DateTimePicker3_ValueChanged(object sender, EventArgs e)
        {

            if (dateTimePicker3.Value > dateTimePicker4.Value)
            {
                MessageBox.Show("日期不合理，请重新填写！！！", "警告", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            condition1[(int)control.dataTimePicker1] = dateTimePicker3.Value.ToString("yyyy-MM-dd");

        }

        /// <summary>
        /// 气象站数据采集时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DateTimePicker4_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker4.Value < dateTimePicker3.Value)
            {
                MessageBox.Show("日期不合理，请重新填写！！！", "警告", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            condition1[(int)control.dataTimePicker2] = dateTimePicker4.Value.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 清除面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearPanel1_Click(object sender, EventArgs e)
        {
            // zedGraphControl2.GraphPane.CurveList.Clear(); //清除画板
            Refresh();
        }


        /// <summary>
        /// 跳转到实时数据界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMonitor1_Click(object sender, EventArgs e)
        {

            //             if (MainForm.monitorForm == null)
            //             {
            // 
            //                 MainForm.monitorForm = new MonitorForm();
            //                 MainForm.monitorForm.Show();
            //             }
            //             else
            //             {
            //                 MainForm.monitorForm.Show();
            //                 MainForm.monitorForm.Focus();
            //                 
            //             }      
        }


        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportWeather_Click(object sender, EventArgs e)
        {
            //string fileName = "未命名";
            //fileOperate.DataBaseToExcel1(dataGridView2,sqlInit1,fileName);

            string fileName = "未命名";


            DataSet ds = new DataSet();

            database.ReadDataBase(sqlInit1, "historydata", ds);
            System.Data.DataTable dtInfo = new System.Data.DataTable();
            dtInfo = ds.Tables["historydata"];

            if (dtInfo.Rows.Count <= 0)
            {
                MessageBox.Show("没有数据！！！", "提示信息"
                    , MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            dtInfo.Columns[0].ColumnName = "气象站名称";
            dtInfo.Columns[1].ColumnName = "采集数据类型";
            dtInfo.Columns[2].ColumnName = "采集数据值";
            dtInfo.Columns[3].ColumnName = "采集数据时间";




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


            MessageBox.Show("导出完毕", "提示信息", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

        }

        #endregion

        #region "公用方法"

        /// <summary>
        /// 读取数据的条数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private int ReadHistoryDataCount(string sql)
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

        /// <summary>
        /// 生成sql语句的函数
        /// </summary>
        /// <param name="i">控件的编号</param>
        /// <returns></returns>

        private string CreateSql(string[] array, int i)
        {
            //根据字符串生成sql语句
            string temp = null;

            switch (i)
            {
                case (int)control.proName:

                    temp = "detectioninfo.p_name='" + array[(int)control.proName] + "'";
                    break;

                case (int)control.testUnit:

                    temp = "detectioninfo.submission_unit='" + array[(int)control.testUnit] + "'";
                    break;

                case (int)control.testType:

                    temp = "detectioninfo.type='" + array[(int)control.testType] + "'";
                    break;
                case (int)control.comany_Class:

                    temp = "companyinfo.Company_Class='" + array[(int)control.comany_Class] + "'";
                    break;
                case (int)control.dataTimePicker1:

                    temp = "detectioninfo.test_time between '" + array[(int)control.dataTimePicker1] + "'";
                    break;

                case (int)control.dataTimePicker2:

                    temp = "'" + array[(int)control.dataTimePicker2] + "'";
                    break;

                case (int)control.dataTimePicker3:

                    temp = "detectioninfo.test_time between '" + array[(int)control.dataTimePicker3] + "'";
                    break;

                case (int)control.dataTimePicker4:

                    temp = "'" + array[(int)control.dataTimePicker4] + "'";
                    break;

                //                 case (int)control.UbsubUnit:
                // 
                //                     temp = "user.Company='" + array[(int)control.UbsubUnit] + "'";
                //                     break;
            }
            return temp;
        }

        /// <summary>
        /// 绘制曲线图形
        /// </summary>
        /// <param name="zgc"></param>
        /// <param name="sql"></param>
        public void InitLine(ZedGraphControl zgc)
        {
            GraphPane graphPane = zgc.GraphPane;

            //graphPane.CurveList.Clear();
            //graphPane.GraphObjList.Clear();


            //设置图标标题和x、y轴标题
            graphPane.Title.Text = "检测结果情况";
            graphPane.XAxis.Title.Text = "时间";
            graphPane.YAxis.Title.Text = "数值";

            //填充图表颜色
            graphPane.Fill = new Fill(Color.GreenYellow, Color.FromArgb(200, 200, 255), 45.0f);

            List<string> listXAxis = new List<string>();
            listXAxis.Add(dateTimePicker1.Text);
            listXAxis.Add(dateTimePicker2.Text);
            string[] labels = listXAxis.ToArray();

            graphPane.XAxis.Scale.TextLabels = labels; //X轴文本取值
            graphPane.XAxis.Type = AxisType.Text;   //X轴类型



            graphPane.XAxis.Color = Color.LightGray;
            graphPane.YAxis.Color = Color.LightGray;

            //画到zedGraphControl1控件中，此句必加
            zgc.AxisChange();

            //重绘控件
            Refresh();
        }

        /// <summary>
        ///   生成5条曲线
        /// </summary>
        public void CreateLineByList(ZedGraphControl zgc, List<string> list)
        {
            if (list == null)
            {
                return;
            }

            GraphPane graphPane = zgc.GraphPane;
            PointPairList pointListTest = new PointPairList();

            hc = new HistoryCurve();

            hc.pointList = pointListTest;

            CreateCurveSql();

            //设置图标标题和x、y轴标题
            graphPane.Title.Text = "历史数据情况";
            graphPane.XAxis.Title.Text = "时间";
            graphPane.YAxis.Title.Text = "数值";

            this.zedGraphControl1.GraphPane.XAxis.Type = ZedGraph.AxisType.DateAsOrdinal;
            this.zedGraphControl1.GraphPane.XAxis.Scale.Format = "yyyy-MM-dd HH:mm:ss";

            // 曲线名称
            string lineName = "";
            if (collectType.SelectedIndex < 0)
            {
                collectType.SelectedIndex = 0;
            }
            Color linecolor = lineColorFlag[collectType.SelectedIndex];


            int rows = 0;


            rows = list.Count / 6;

            if (rows == 0)
            {
                zedGraphControl1.GraphPane.CurveList.Clear(); //清除画板
                Refresh();
            }

            List<string> listXAxis = new List<string>();

            for (int i = 0, j = 0; i <= 6 * (rows - 1); i = i + 6)
            {
                double date = j++;

                double value = System.Convert.ToDouble(list[i + 3]);

                listXAxis.Add(list[i + 5]);

                DateTime dt;
                DateTime.TryParse(list[i + 5], out dt);
                date = (double)new XDate(dt);
                pointListTest.Add(date, value);

            }
            if (lineNameFlag[collectType.SelectedIndex] == 0)
            {
                lineName = collectType.Text;
                lineNameFlag[collectType.SelectedIndex] = 1;
            }

            // 生成一条曲线
            LineItem curve = graphPane.AddCurve(lineName, pointListTest, linecolor, SymbolType.Circle);
            curve.Line.IsSmooth = true;

            //填充图表颜色
            graphPane.Fill = new Fill(Color.GreenYellow, Color.FromArgb(200, 200, 255), 45.0f);


            graphPane.XAxis.Color = Color.LightGray;
            graphPane.YAxis.Color = Color.LightGray;

            //画到zedGraphControl1控件中，此句必加
            zgc.AxisChange();

            //重绘控件
            Refresh();
        }


        public void AddLineDataByList(ZedGraphControl zgc, PointPairList pointList, List<string> list)
        {
            GraphPane graphPane = zgc.GraphPane;

            int rows = 0;

            if (list == null)
            {
                return;
            }
            rows = list.Count / 6;

            if (rows == 0)
            {
                return;
            }

            List<string> listXAxis = new List<string>();

            for (int i = 0, j = 0; i <= 6 * (rows - 1); i = i + 6)
            {
                double date = j++;

                double value = System.Convert.ToDouble(list[i + 3]);

                listXAxis.Add(list[i + 5]);

                DateTime dt;
                DateTime.TryParse(list[i + 5], out dt);
                date = (double)new XDate(dt);
                pointList.Add(date, value);

            }


            //画到zedGraphControl1控件中，此句必加
            zgc.AxisChange();

            //重绘控件
            Refresh();
        }

        /// <summary>
        /// 采集器绘制曲线图形
        /// </summary>
        /// <param name="zgc"></param>
        /// <param name="sql"></param>
        public void CreateLine(ZedGraphControl zgc, string sql)
        {
            GraphPane graphPane = zgc.GraphPane;

            //graphPane.CurveList.Clear();
            //graphPane.GraphObjList.Clear();


            //设置图标标题和x、y轴标题
            graphPane.Title.Text = "历史数据情况";
            graphPane.XAxis.Title.Text = "时间";
            graphPane.YAxis.Title.Text = "数值";

            // 曲线名称
            string lineName = "";
            if (collectType.SelectedIndex < 0)
            {
                collectType.SelectedIndex = 0;
            }
            Color linecolor = lineColorFlag[collectType.SelectedIndex];



            PointPairList pointList = new PointPairList();

            try
            {
                List<string> list = new List<string>();
                int rows = 0;
                rows = database.QueryTable(sql, list, 6);

                if (rows == 0)
                {
                    zedGraphControl1.GraphPane.CurveList.Clear(); //清除画板
                    Refresh();
                }

                List<string> listXAxis = new List<string>();

                for (int i = 0, j = 0; i <= 6 * (rows - 1); i = i + 6)
                {
                    //if (lineName.CompareTo("") == 0)
                    //{
                    //    lineName = list[i + 2];
                    //}


                    double date = j++;

                    double value = System.Convert.ToDouble(list[i + 3]);

                    listXAxis.Add(list[i + 5]);
                    pointList.Add(date, value);


                }

                string[] labels = listXAxis.ToArray();

                graphPane.XAxis.Scale.TextLabels = labels; //X轴文本取值
                graphPane.XAxis.Type = AxisType.Text;   //X轴类型


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine("Error " + ex.Number + " : " + ex.Message);
            }


            if (lineNameFlag[collectType.SelectedIndex] == 0)
            {
                lineName = collectType.Text;
                lineNameFlag[collectType.SelectedIndex] = 1;
            }



            // 生成一条曲线
            LineItem curve = graphPane.AddCurve(lineName, pointList, linecolor, SymbolType.Circle);
            curve.Line.IsSmooth = true;

            //填充图表颜色
            graphPane.Fill = new Fill(Color.GreenYellow, Color.FromArgb(200, 200, 255), 45.0f);


            graphPane.XAxis.Color = Color.LightGray;
            graphPane.YAxis.Color = Color.LightGray;

            //画到zedGraphControl1控件中，此句必加
            zgc.AxisChange();

            //重绘控件
            Refresh();
        }

        /// <summary>
        /// 气象站绘制曲线
        /// </summary>
        /// <param name="zgc"></param>
        /// <param name="sql"></param>
        public void CreateLine1(ZedGraphControl zgc, string sql)
        {
            GraphPane graphPane = zgc.GraphPane;

            //graphPane.CurveList.Clear();
            //graphPane.GraphObjList.Clear();


            //设置图标标题和x、y轴标题
            graphPane.Title.Text = "历史数据情况";
            graphPane.XAxis.Title.Text = "时间";
            graphPane.YAxis.Title.Text = "数值";

            // 曲线名称
            string lineName = "";
            Color linecolor = lineColorFlag[detect_Pro.SelectedIndex];

            PointPairList pointList = new PointPairList();

            try
            {

                //database.ExecuteReader(sql);
                //MySqlDataReader myData = database.GetDataReader();

                //if (myData.HasRows)
                //{
                //    List<string> listXAxis = new List<string>();
                //    int i = 0;
                //    // 读取数据并显示出来
                //    while (myData.Read())
                //    {
                //        if (lineName.CompareTo("") == 0)
                //        {
                //            lineName = myData.GetString(1);
                //        }
                //        double date = i++;

                //        double value = System.Convert.ToDouble(myData.GetString(2));

                //        listXAxis.Add(myData.GetString(3));
                //        pointList.Add(date, value);

                //    }

                //    string[] labels = listXAxis.ToArray();

                //    graphPane.XAxis.Scale.TextLabels = labels; //X轴文本取值
                //    graphPane.XAxis.Type = AxisType.Text;   //X轴类型

                //}
                //myData.Close();

                List<string> list = new List<string>();
                int rows = 0;
                rows = database.QueryTable(sql, list, 4);


                if (rows == 0)
                {
                    //zedGraphControl2.GraphPane.CurveList.Clear(); //清除画板
                    Refresh();
                }



                List<string> listXAxis = new List<string>();

                for (int i = 0, j = 0; i <= 4 * (rows - 1); i = i + 4)
                {
                    //if (lineName.CompareTo("") == 0)
                    //{
                    //    lineName = list[i + 1];
                    //}
                    double date = j++;

                    double value = System.Convert.ToDouble(list[i + 2]);

                    listXAxis.Add(list[i + 3]);
                    pointList.Add(date, value);


                }

                string[] labels = listXAxis.ToArray();

                graphPane.XAxis.Scale.TextLabels = labels; //X轴文本取值
                graphPane.XAxis.Type = AxisType.Text;   //X轴类型

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine("Error " + ex.Number + " : " + ex.Message);
            }



            if (lineNameFlag1[detect_Pro.SelectedIndex] == 0)
            {
                lineName = detect_Pro.Text;
                lineNameFlag1[detect_Pro.SelectedIndex] = 1;
            }

            // 生成一条曲线
            LineItem curve = graphPane.AddCurve(lineName, pointList, linecolor, SymbolType.Circle);

            curve.Line.IsSmooth = true;

            //填充图表颜色
            graphPane.Fill = new Fill(Color.GreenYellow, Color.FromArgb(200, 200, 255), 45.0f);


            graphPane.XAxis.Color = Color.LightGray;
            graphPane.YAxis.Color = Color.LightGray;

            //画到zedGraphControl1控件中，此句必加
            zgc.AxisChange();

            //重绘控件
            Refresh();
        }



        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HistoryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //DialogResult result = MessageBox.Show("你确定要关闭吗！", "提示信息", MessageBoxButtons.OKCancel,
            //    MessageBoxIcon.Question);
            //if (result == DialogResult.OK)
            //{
            //    e.Cancel = false;//点击OK
            //    MainForm.historyForm = null;
            //}
            //else
            //{
            //    e.Cancel = true;
            //}

        }


        /// <summary>
        /// zedGraph右键显示中文的设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="menuStrip"></param>
        /// <param name="mousePt"></param>
        /// <param name="objState"></param>
        private void MyContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, System.Drawing.Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
        {
            foreach (ToolStripMenuItem item in menuStrip.Items)
            {


                if ((string)item.Tag == "undo_all")
                {
                    menuStrip.Items.Remove(item);
                    item.Visible = false;
                    break;

                }


            }


            foreach (ToolStripMenuItem item in menuStrip.Items)
            {
                switch (item.Name)
                {
                    case "copied_to_clip":
                        item.Text = @"复制到剪贴板";
                        break;
                    case "copy":
                        item.Text = @"复制";
                        break;
                    case "page_setup":
                        item.Text = @"页面设置...";
                        break;
                    case "print":
                        item.Text = @"打印...";
                        break;
                    case "save_as":
                        item.Text = @"另存图表...";
                        break;
                    case "set_default":
                        item.Text = @"恢复默认大小";
                        break;
                    case "show_val":
                        item.Text = @"显示节点数值";
                        break;
                    case "title_def":
                        item.Text = @"标题";
                        break;
                    case "undo_all":
                        item.Text = @"还原缩放/移动";
                        break;

                    case "unpan":
                        item.Text = @"还原移动";
                        break;

                    case "unzoom":
                        item.Text = @"还原缩放";
                        break;

                    case "x_title_def":
                        item.Text = @"X 轴";
                        break;
                    case "y_title_def":
                        item.Text = @"Y 轴";
                        break;


                }


            }
        }


        //当日的极值和均值
        public void Evaluation()
        {

            int valueTypeId = 0;
            int valueSiteId = 0;

            string sqlType = "select dataTypeId from collectdatatype where dataTypeName=";

            List<string> typeName = new List<string>();
            database.QueryTable(sqlType, typeName, 1);
            if (typeName.Count > 0)
            {
                valueTypeId = System.Convert.ToInt32(typeName[0]);
            }



            string sqlSite = "select siteId from site where proName=";

            List<string> proName = new List<string>();
            database.QueryTable(sqlSite, proName, 1);
            if (proName.Count > 0)
            {
                valueSiteId = System.Convert.ToInt32(proName[0]);
            }


            string sqlValue = "SELECT site.proName,collectdatatype.dataTypeName,"
                              + "round(AVG(historydata.collectValue),2),round(MAX(historydata.collectValue),2),"
                              + "round(MIN(historydata.collectValue),2)"
                              + " FROM (((historydata INNER JOIN devicesaddress ON historydata.collectorSign=devicesaddress.devicesAddressSign)"
                              + " INNER JOIN collector ON devicesaddress.devicesAddressId=collector.devicesAddressId)"
                              + " INNER JOIN collectdatatype ON historydata.dataTypeId=collectdatatype.dataTypeId )"
                              + " INNER JOIN site ON site.siteId=collector.siteId"
                              + " WHERE" + " site.siteId=" + valueSiteId + " AND " + " historydata.dataTypeId=" + valueTypeId + " AND "

                              + " AND  collectValue > 0 "
                              + " GROUP BY site.siteId,historydata.dataTypeId "
                              + " order by site.proName,collectdatatype.dataTypeName ASC";


            //MessageBox.Show(valueSiteId+"");
            List<string> values = new List<string>();
            int rows = 0;
            string valueSite = null;
            string valueType = null;
            string avgValue = null;
            string maxValue = null;
            string minValue = null;


            rows = database.QueryTable(sqlValue, values, 5);

            //             if (rows <= 0)
            //             {
            //                 
            //                 AvgValue.Text = "";
            //                 MaxValue.Text = "";
            //                 MinValue.Text = "";
            //             }

            for (int i = 0; i < 4 * rows; i = i + 5)
            {

                valueSite = values[0];
                valueType = values[1];
                avgValue = values[2];
                maxValue = values[3];
                minValue = values[4];
                //MessageBox.Show(avgValue + "," + maxValue + "," + minValue);

                //                  AvgValue.Text = avgValue;
                //                  MaxValue.Text = maxValue;
                //                  MinValue.Text = minValue;
            }

        }

        //极值和均值的界面
        public void MMAValue()
        {
            int valueTypeId = 0;
            int valueSiteId = 0;

            string sqlType = "select dataTypeId from collectdatatype where dataTypeName=";

            List<string> typeName = new List<string>();
            database.QueryTable(sqlType, typeName, 1);
            if (typeName.Count > 0)
            {
                valueTypeId = System.Convert.ToInt32(typeName[0]);
            }


            string sqlSite = "select siteId from site where proName=";

            List<string> proName = new List<string>();
            database.QueryTable(sqlSite, proName, 1);
            if (proName.Count > 0)
            {
                valueSiteId = System.Convert.ToInt32(proName[0]);
            }


            string MMASql = "SELECT (site.proName)场地,(collectdatatype.dataTypeName)采集类型,"
                              + "round(maxValue,2)最大值,DATE_FORMAT(maxTime,'%X-%m-%d %H:%i:%s')最大值采集时间,round(minValue,2)最小值,DATE_FORMAT(minTime,'%X-%m-%d %H:%i:%s')最小值采集时间,"
                              + "round(AVG(avgValue),2)平均值,(collectdatatype.remarks)单位,DATE_FORMAT(maxmindata.collectTime,'%X-%m-%d')采集日期"
                              + " FROM (((maxmindata INNER JOIN devicesaddress ON maxmindata.collectorSign=devicesaddress.devicesAddressSign)"
                              + " INNER JOIN collector ON devicesaddress.devicesAddressId=collector.devicesAddressId)"
                              + " INNER JOIN collectdatatype ON maxmindata.dataTypeId=collectdatatype.dataTypeId )"
                              + " INNER JOIN site ON site.siteId=collector.siteId"
                              + " WHERE" + " site.siteId=" + valueSiteId + " AND " + " maxmindata.dataTypeId=" + valueTypeId + " AND "
                              + "maxmindata.collectTime BETWEEN  '" + dateTimePicker7.Text
                              + "' AND '" + dateTimePicker8.Value.ToString() + "'"
                              + " GROUP BY site.siteId,maxmindata.dataTypeId,DATE_FORMAT(maxmindata.collectTime,'%X-%m-%d')"
                              + " order by site.proName,collectdatatype.dataTypeName ASC";


            sqlInit2 = MMASql;

            // ReadData(dataGridView3, MMASql);

            CreateLine2(zedGraphControl3, sqlInit2);
        }




        #endregion

        #region "极值和均值界面"



        private void ReadData(DataGridView dgv, string sql)
        {
            DataSet ds = new DataSet();
            database.ReadDataBase(sql, "dataTable", ds);
            dgv.DataSource = ds.Tables["dataTable"];
        }


        //均值和极值的界面
        private void ValueSelect_Click(object sender, EventArgs e)
        {
            MMAValue();
            CreateLine2(zedGraphControl3, sqlInit2);
        }

        private void dateTimePicker7_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker7.Value > dateTimePicker8.Value)
            {
                MessageBox.Show("日期不合理，请重新填写！！！", "警告", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }
            fromdate = dateTimePicker7.Text;

        }

        private void dateTimePicker8_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker8.Value < dateTimePicker7.Value)
            {
                MessageBox.Show("日期不合理，请重新填写！！！", "警告", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            todate = dateTimePicker8.Text;
        }





        /// <summary>
        /// 均值绘制曲线
        /// </summary>
        /// <param name="zgc"></param>
        /// <param name="sql"></param>
        public void CreateLine2(ZedGraphControl zgc, string sql)
        {
            GraphPane graphPane = zgc.GraphPane;

            graphPane.CurveList.Clear();
            graphPane.GraphObjList.Clear();


            //设置图标标题和x、y轴标题
            graphPane.Title.Text = "历史数据情况";
            graphPane.XAxis.Title.Text = "时间";
            graphPane.YAxis.Title.Text = "数值";

            // 曲线名称
            string lineName = "";

            PointPairList pointList = new PointPairList();
            PointPairList pointListMax = new PointPairList();
            PointPairList pointListMin = new PointPairList();

            try
            {

                //database.ExecuteReader(sql);
                //MySqlDataReader myData = database.GetDataReader();

                //if (myData.HasRows)
                //{
                //    List<string> listXAxis = new List<string>();
                //    int i = 0;
                //    // 读取数据并显示出来
                //    while (myData.Read())
                //    {
                //        if (lineName.CompareTo("") == 0)
                //        {
                //            lineName = myData.GetString(1);
                //        }
                //        double date = i++;

                //        double value = System.Convert.ToDouble(myData.GetString(2));

                //        listXAxis.Add(myData.GetString(3));
                //        pointList.Add(date, value);

                //    }

                //    string[] labels = listXAxis.ToArray();

                //    graphPane.XAxis.Scale.TextLabels = labels; //X轴文本取值
                //    graphPane.XAxis.Type = AxisType.Text;   //X轴类型

                //}
                //myData.Close();

                List<string> list = new List<string>();
                int rows = 0;
                rows = database.QueryTable(sql, list, 9);


                if (rows == 0)
                {
                    zedGraphControl3.GraphPane.CurveList.Clear(); //清除画板
                    Refresh();
                }



                List<string> listXAxis = new List<string>();

                for (int i = 0, j = 0; i <= 9 * (rows - 1); i = i + 9)
                {
                    if (lineName.CompareTo("") == 0)
                    {
                        lineName = list[i + 1];
                    }
                    double date = j++;

                    double value = System.Convert.ToDouble(list[i + 6]);

                    listXAxis.Add(list[i + 8]);
                    pointList.Add(date, value);

                    value = System.Convert.ToDouble(list[i + 2]);
                    pointListMax.Add(date, value);

                    value = System.Convert.ToDouble(list[i + 4]);
                    pointListMin.Add(date, value);
                }

                string[] labels = listXAxis.ToArray();

                graphPane.XAxis.Scale.TextLabels = labels; //X轴文本取值
                graphPane.XAxis.Type = AxisType.Text;   //X轴类型

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine("Error " + ex.Number + " : " + ex.Message);
            }





            // 生成一条曲线
            LineItem curve = graphPane.AddCurve("均值", pointList, Color.Blue, SymbolType.Circle);

            curve.Line.IsSmooth = true;

            LineItem curveMax = graphPane.AddCurve("最大值", pointListMax, Color.Red, SymbolType.Circle);

            curve.Line.IsSmooth = true;

            LineItem curveMin = graphPane.AddCurve("最小值", pointListMin, Color.Green, SymbolType.Circle);

            curve.Line.IsSmooth = true;

            //填充图表颜色
            graphPane.Fill = new Fill(Color.GreenYellow, Color.FromArgb(200, 200, 255), 45.0f);


            graphPane.XAxis.Color = Color.LightGray;
            graphPane.YAxis.Color = Color.LightGray;

            //画到zedGraphControl1控件中，此句必加
            zgc.AxisChange();

            //重绘控件
            Refresh();
        }


        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedIndex = 2;


        }


        int flag = 0;
        /// <summary>
        /// 放大事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        private void zedGraphControl1_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            //查询数据按键
            //int mcount = 0;//统计条件的个数
            //int i;

            //string sqlModel = "SELECT site.proName," +
            // "collector.collectorName,collectdatatype.dataTypeName," +
            // "historydata.collectValue,collectdatatype.remarks,historydata.collectTime " +
            // "  FROM (((historydata INNER JOIN devicesaddress ON historydata.gatewaySign=devicesaddress.devicesAddressSign " +
            // " OR historydata.collectorSign=devicesAddress.devicesAddressSign)" +
            // "  INNER JOIN collector ON devicesaddress.devicesAddressId=collector.devicesAddressId) " +
            // "  INNER JOIN collectdatatype ON historydata.dataTypeId=collectdatatype.dataTypeId) " +
            // "  INNER JOIN site ON collector.siteId=site.siteId ";

            ////检查comboBox是空的情况          
            //if (collectNum.Text.Equals(""))
            //{
            //    condition[(int)control.collectNum] = collectNum.Text;
            //}
            //if (collectType.Text.Equals(""))
            //{
            //    condition[(int)control.collectType] = collectType.Text;
            //}
            //if (siteNum.Text.Equals(""))
            //{
            //    condition[(int)control.siteNum] = siteNum.Text;
            //}


            ////根据复选框的条件来拼接sql语句，默认情况下时间控件的值是当天的时间值
            //for (i = 0; i < condition.Length; i++)
            //{
            //    if (condition[i].Equals(""))
            //    {
            //        continue;

            //    }
            //    else
            //    {

            //        if (mcount == 0)
            //        {
            //            sqlModel += " WHERE ";
            //            sqlModel += CreateSql(condition, i);

            //        }
            //        else
            //        {
            //            sqlModel += " and ";
            //            sqlModel += CreateSql(condition, i);
            //        }
            //        mcount++;
            //    }
            //}

            ////sqlModel += " ORDER BY historydata.collectTime asc ";
            //string sql = sqlModel;

            //sql += " LIMIT ";

            //// 第一次只加载20条
            //if (pageCount == 1)
            //{
            //    sql += System.Convert.ToString(pageCount * 20);
            //}
            //else
            //{
            //    sql += System.Convert.ToString((pageCount - 1) * 10000 + 20);
            //}

            //sql += " ,10000";

            string sql = null;
            for (int i = 0; i < hcList.Count; i++)
            {
                sql = hcList[i].CreateCurveSql(pageCount);

                List<string> list = new List<string>();
                database.QueryTable(sql, list, 6);

                if (!condition[(int)control.testType].Equals(""))
                {
                    AddLineDataByList(zedGraphControl1, hcList[i].pointList, list);
                }
            }
            pageCount++;
        }

        private void CreateCurveSql()
        {
            //查询数据按键
            int mcount = 0;//统计条件的个数
            int i;

            string sqlModel = "SELECT site.proName," +
             "collector.collectorName,collectdatatype.dataTypeName," +
             "historydata.collectValue,collectdatatype.remarks,historydata.collectTime " +
             "  FROM (((historydata INNER JOIN devicesaddress ON historydata.gatewaySign=devicesaddress.devicesAddressSign " +
             " OR historydata.collectorSign=devicesAddress.devicesAddressSign)" +
             "  INNER JOIN collector ON devicesaddress.devicesAddressId=collector.devicesAddressId) " +
             "  INNER JOIN collectdatatype ON historydata.dataTypeId=collectdatatype.dataTypeId) " +
             "  INNER JOIN site ON collector.siteId=site.siteId ";

            //检查comboBox是空的情况          
            if (ProName.Text.Equals(""))
            {
                condition[(int)control.proName] = ProName.Text;
            }
            if (TestUnit.Text.Equals(""))
            {
                condition[(int)control.testUnit] = TestUnit.Text;
            }
            if (TestType.Text.Equals(""))
            {
                condition[(int)control.testType] = TestType.Text;
            }
            //             if (UbsubUnit.Text.Equals(""))
            //             {
            //                 condition[(int)control.UbsubUnit] = UbsubUnit.Text;
            //             }

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
                        sqlModel += CreateSql(condition, i);

                    }
                    else
                    {
                        sqlModel += " and ";
                        sqlModel += CreateSql(condition, i);
                    }
                    mcount++;
                }
            }

            //sqlModel += " ORDER BY historydata.collectTime asc ";
            string sql = sqlModel;

            sql += " LIMIT ";

            hc.sql = sql;
            hcList.Add(hc);
        }

        private void ValueSelect_Click_1(object sender, EventArgs e)
        {
            zedGraphControl3.GraphPane.CurveList.Clear(); //清除画板
            GraphPane myPane = zedGraphControl1.GraphPane;


            // string sql = "SELECT p_name,type,channel,result_unit,standard,sample_no,sample_name,sites,submission_unit,test_operator,test_time,test_unit,Juge_result,Detect_result,Absolut_result  FROM detectioninfo";

            //查询数据
            string sql = "SELECT COUNT(test_unit) ,test_unit  FROM detectioninfo WHERE Juge_result ='不合格'  AND  test_time between  '" + fromdate + "' and '" + todate + "' GROUP BY test_unit";



            foodPieGraph(zedGraphControl3, sql);
        }

        private void clearPanel1_Click_1(object sender, EventArgs e)
        {
            zedGraphControl2.GraphPane.CurveList.Clear(); //清除画板
            Refresh();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            zedGraphControl1.GraphPane.CurveList.Clear(); //清除画板
            Refresh();
        }
        /// <summary>
        ///   用于查询分析趋势
        /// </summary>
        private void selectData_Click_1(object sender, EventArgs e)
        {

            //查询数据
            int mcount = 0;//统计条数
            int i;

            string sqlModel = "SELECT detectioninfo.p_name,detectioninfo.channel,detectioninfo.sample_name,detectioninfo.submission_unit,detectioninfo.Juge_result,detectioninfo.Detect_result FROM detectioninfo JOIN ftinfor ON (detectioninfo.p_name = ftinfor.ftestitems AND detectioninfo.type = ftinfor.fsample) ";

            string sql = "SELECT detectioninfo.p_name,detectioninfo.channel,detectioninfo.sample_name,detectioninfo.submission_unit,detectioninfo.Juge_result,detectioninfo.Detect_result FROM detectioninfo JOIN ftinfor ON (detectioninfo.p_name = ftinfor.ftestitems AND detectioninfo.type = ftinfor.fsample AND detectioninfo.Juge_result ='合格') ";

            string sqlModelCount = "SELECT COUNT(*)" +
            "   FROM detectioninfo JOIN ftinfor ON (detectioninfo.p_name = ftinfor.ftestitems AND detectioninfo.type = ftinfor.fsample AND detectioninfo.Juge_result='合格’ )";
            string sqlCount = "SELECT COUNT(*) FROM detectioninfo JOIN ftinfor ON (detectioninfo.p_name = ftinfor.ftestitems AND detectioninfo.type = ftinfor.fsample) ";

            //检查comboBox是空的情况          
            if (ProName.Text.Equals(""))
            {
                condition[(int)control.proName] = ProName.Text;
            }
            if (TestUnit.Text.Equals(""))
            {
                condition[(int)control.testUnit] = TestUnit.Text;
            }
            if (TestType.Text.Equals(""))
            {
                condition[(int)control.testType] = TestType.Text;
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
                        sqlModel += CreateSql(condition, i);
                        sqlModelCount += CreateSql(condition, i);

                    }
                    else
                    {
                        sqlModel += " and ";
                        sqlModelCount += " and ";
                        sqlModel += CreateSql(condition, i);
                        sqlModelCount += CreateSql(condition, i);
                        sql += " and ";
                        sqlCount += " and ";
                        sql += CreateSql(condition, i);
                        sqlCount += CreateSql(condition, i);

                    }
                    mcount++;
                }
            }

            sqlInit = sqlModel;

            LineGraph(zedGraphControl1, sqlInit);




        }

        private void ProName_Selectedindexchang(object sender, EventArgs e)
        {
            condition[(int)control.proName] = ProName.Text;
            string sql = null;
            string testUnit;
            testUnit = ProName.Text;


            // 根据用户选择的场地，对应出相应的采集器
            ComboBoxDataBase boxDataBase = new ComboBoxDataBase();
            sql = "SELECT submission_unit FROM detectioninfo WHERE p_name = '" + testUnit + "'";

            boxDataBase.BindDataBase(ref this.TestUnit, ref database, sql);


        }

        Dictionary<int, string> dic = new Dictionary<int, string>();
        /// <summary>
        ///   绘制不合各食品来源饼状图
        ///   不合格数+单位名称
        /// </summary>
        int i = 0;
        public void foodPieGraph(ZedGraphControl zgc, string sql)
        {
            dic.Clear();
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();
            myPane.GraphObjList.Clear();
            // Set the pane title
            myPane.Title.Text = "不合格食品来源\n (单位名称)";

            
            //查询数据
            DataSet dataset = new DataSet();
            System.Data.DataTable table = new System.Data.DataTable();

            int ret = database.ReadDataBase(sql, "result", dataset);
            table = dataset.Tables["result"];
            this.dataGridView3.DataSource = table;
            dataGridView3.Columns[0].HeaderText = "检测数量";
            dataGridView3.Columns[1].HeaderText = "检测单位";
            int numrow = table.Rows.Count;
            if (numrow > 0)
            {
                i = 0;
                double[] values = new double[numrow];
                double[] values2 = new double[numrow];
                Color[] colors = { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Gold, Color.Thistle, Color.Tomato, Color.OldLace };
                double[] displacement = new double[numrow];
                string[] labels2 = new string[numrow];
                if (ret == 0)
                {

                    foreach (DataRow r in table.Rows)
                    {
                        // dic.Add(int.Parse(r[0].ToString()), r[1].ToString());
                        values2[i] = double.Parse(r[0].ToString());
                        labels2[i] = r[1].ToString();
                        i++;
                    }
                }
                // Enter some data values

                // Fill the pane and axis background with solid color
                myPane.Fill = new Fill(Color.Cornsilk);
                myPane.Chart.Fill = new Fill(Color.Cornsilk);
                myPane.Legend.Position = LegendPos.Right;

                // Add some more slices as an array
                PieItem[] slices = new PieItem[dic.Count];
                slices = myPane.AddPieSlices(values2, labels2);

                // Modify the slice label types
                for (int j = 0; j < numrow; ++j)
                {
                    ((PieItem)slices[j]).LabelType = PieLabelType.Name_Value_Percent;
                    ((PieItem)slices[j]).Displacement = 0.2;
                }

                // Sum up the values																					
                CurveList curves = myPane.CurveList;
                double total = 0;
                for (int x = 0; x < curves.Count; x++)
                    total += ((PieItem)curves[x]).Value;

                // Add a text item to highlight total sales
                TextObj text = new TextObj("总计不合格数量 - " + " " + total.ToString() + "：条", 0.85F, 0.80F, CoordType.PaneFraction);
                text.Location.AlignH = AlignH.Center;
                text.Location.AlignV = AlignV.Bottom;
                text.FontSpec.Border.IsVisible = false;
                text.FontSpec.Fill = new Fill(Color.White, Color.PowderBlue, 45F);
                text.FontSpec.StringAlignment = StringAlignment.Center;
                myPane.GraphObjList.Add(text);

                // Add a colored background behind the pie
                BoxObj box = new BoxObj(0, 0, 1, 1, Color.Empty, Color.PeachPuff);
                box.Location.CoordinateFrame = CoordType.ChartFraction;
                box.Border.IsVisible = false;
                box.Location.AlignH = AlignH.Left;
                box.Location.AlignV = AlignV.Top;
                box.ZOrder = ZOrder.E_BehindCurves;
                myPane.GraphObjList.Add(box);
                zgc.AxisChange();
                Refresh();
            }
            else
            {
                MessageBox.Show("该时间段没有相关记录");
            }

        }
        /// <summary>
        ///   绘制饼图
        /// </summary>
        public void PieGraph(ZedGraphControl zgc, string sql)
        {
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();
            myPane.GraphObjList.Clear();
            // Set the pane title
            myPane.Title.Text = "2004 ZedGraph Sales by Region\n ($M)";

            // Enter some data values
            double[] values = { 15, 15, 40, 20 };
            double[] values2 = { 250, 50, 400, 50 };
            Color[] colors = { Color.Red, Color.Blue, Color.Green, Color.Yellow };
            double[] displacement = { .0, .0, .0, .0 };
            string[] labels2 = { "Europe", "Pac Rim", "South America", "Africa" };

            // Fill the pane and axis background with solid color
            myPane.Fill = new Fill(Color.Cornsilk);
            myPane.Chart.Fill = new Fill(Color.Cornsilk);
            myPane.Legend.Position = LegendPos.Right;

            // Create some pie slices
            //             PieItem segment1 = myPane.AddPieSlice(20, Color.Navy, .20, "North");
            //             PieItem segment2 = myPane.AddPieSlice(40, Color.Salmon, 0, "South");
            //             PieItem segment3 = myPane.AddPieSlice(30, Color.Yellow, .0, "East");
            //             PieItem segment4 = myPane.AddPieSlice(10.21, Color.LimeGreen, 0, "West");
            //             PieItem segment5 = myPane.AddPieSlice(10.5, Color.Aquamarine, .3, "Canada");

            // Add some more slices as an array
            PieItem[] slices = new PieItem[values2.Length];
            slices = myPane.AddPieSlices(values2, labels2);

            // Modify the slice label types
            ((PieItem)slices[0]).LabelType = PieLabelType.Name_Value;
            ((PieItem)slices[1]).LabelType = PieLabelType.Name_Value_Percent;
            ((PieItem)slices[2]).LabelType = PieLabelType.Name_Value;
            ((PieItem)slices[3]).LabelType = PieLabelType.Name_Value;
            ((PieItem)slices[1]).Displacement = .2;
            //             segment1.LabelType = PieLabelType.Name_Percent;
            //             segment2.LabelType = PieLabelType.Name_Value;
            //             segment3.LabelType = PieLabelType.Percent;
            //             segment4.LabelType = PieLabelType.Value;
            //             segment5.LabelType = PieLabelType.Name_Value;
            //             segment2.LabelDetail.FontSpec.FontColor = Color.Red;

            // Sum up the values																					
            CurveList curves = myPane.CurveList;
            double total = 0;
            for (int x = 0; x < curves.Count; x++)
                total += ((PieItem)curves[x]).Value;

            // Add a text item to highlight total sales
            TextObj text = new TextObj("Total 2004 Sales - " + "$" + total.ToString() + "M", 0.85F, 0.80F, CoordType.PaneFraction);
            text.Location.AlignH = AlignH.Center;
            text.Location.AlignV = AlignV.Bottom;
            text.FontSpec.Border.IsVisible = false;
            text.FontSpec.Fill = new Fill(Color.White, Color.PowderBlue, 45F);
            text.FontSpec.StringAlignment = StringAlignment.Center;
            myPane.GraphObjList.Add(text);

            // Add a colored background behind the pie
            BoxObj box = new BoxObj(0, 0, 1, 1, Color.Empty, Color.PeachPuff);
            box.Location.CoordinateFrame = CoordType.ChartFraction;
            box.Border.IsVisible = false;
            box.Location.AlignH = AlignH.Left;
            box.Location.AlignV = AlignV.Top;
            box.ZOrder = ZOrder.E_BehindCurves;
            myPane.GraphObjList.Add(box);
            zgc.AxisChange();
            Refresh();

        }
        /// <summary>
        ///   折线图
        /// </summary>
        /// 
        int time_inter;

        //                 ListViewItem[] itms = new ListViewItem[table1.Rows.Count];
        //                 int k = 0;
        // 
        //                 foreach (DataRow r in table1.Rows)
        //                 {
        //                     arr[0] = r[2].ToString();
        //                     arr[1] = r[1].ToString();
        //                     arr[2] = r[0].ToString();
        //                     arr[3] = r[7].ToString();
        //                     arr[4] = numcount.ToString();
        //                     arr[5] = ((double)goodcount / (double)numcount * 100.0).ToString() + "%";
        //                     arr[6] = r[6].ToString();
        //                     arr[7] = r[3].ToString();
        //                     itms[k] = new ListViewItem(arr);
        //                     listView1.Items.Add(itms[k]);
        //                     k++;
        //     }
        //         
        //             this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。  
        string[] arr2 = new string[6];
        public void LineGraph(ZedGraphControl zgc, string sql)
        {

            LoadResultHeader();

            int goodcount = 0;
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();
            myPane.GraphObjList.Clear();
            // Set the title and axis labels
            myPane.Title.Text = "检测结果趋势分析\n";
            myPane.XAxis.Title.Text = "时间";
            myPane.YAxis.Title.Text = "检测数值";

            List<double> sum_result = new List<double>();
            List<double> avg_result = new List<double>();
            List<double> min_result = new List<double>();
            List<double> max_result = new List<double>();
            List<double> rate_result = new List<double>();
            //查询数据
            DataSet dataSet = new DataSet();
            dataSet.Clear();
            System.Data.DataTable table = new System.Data.DataTable();
            int ret = database.ReadDataBase(sql, "result", dataSet);
            table = dataSet.Tables["result"];


            //             ViewTable = table.Copy();
            //             ViewTable.ImportRow(table.Rows[0]);
            List<string> listXAxis = new List<string>();
            string[] labels = listXAxis.ToArray();
            int m = (dateTimePicker2.Value - dateTimePicker1.Value).Days;

            int temp = 1;

            switch (collectType.SelectedIndex)
            {
                case 0:

                    //  listXAxis.Add(dateTimePicker1.Text);
                    time_inter = m;
                    ListViewItem[] itms = new ListViewItem[time_inter];//
                    for (int i = 0; i < time_inter; i++)
                    {
                        string date = dateTimePicker1.Value.AddDays(i).ToString("yyyy-MM-dd") + "%";
                        listXAxis.Add(dateTimePicker1.Value.AddDays(i).ToString("MM-dd"));
                        string sl;//查询总的数据
                        sl = "SELECT count(Detect_result),AVG( Detect_result),MAX(Detect_result),MIN(Detect_result),test_time FROM detectioninfo WHERE  EXISTS ( " + sql + ")" +
                            "AND test_time like '" + date + "'";
                        string slmodel;//查询合格的
                        slmodel = "SELECT COUNT(Detect_result)，test_time FROM detectioninfo WHERE  EXISTS ( "
                            + sql + " AND detectioninfo.Juge_result ='合格' " + ")" +
                            "AND test_time like '" + date + "'";
                        goodcount = ReadHistoryDataCount(slmodel);
                        DataSet dataSet1 = new DataSet();
                        dataSet1.Clear();
                        System.Data.DataTable table1 = new System.Data.DataTable();
                        int ret1 = database.ReadDataBase(sl, "res", dataSet1);
                        table1 = dataSet1.Tables["res"];

                        if (table1.Rows.Count > 0)
                        {
                            foreach (DataRow r in table1.Rows)
                            {
                                if (r[1].ToString() == "")
                                {
                                    max_result.Add(0.0);//max
                                    avg_result.Add(0.0);//avg
                                    sum_result.Add(0.0);//count
                                    min_result.Add(0.0);//min
                                    rate_result.Add(0.0);//rate
                                }
                                else
                                {
                                    max_result.Add(double.Parse(r[2].ToString()));
                                    rate_result.Add((double)((double)goodcount / double.Parse(r[0].ToString())));
                                    sum_result.Add(double.Parse(r[0].ToString()));
                                    min_result.Add(double.Parse(r[3].ToString()));
                                    avg_result.Add(double.Parse(r[1].ToString()));

                                    {
                                        arr2[0] = r[0].ToString();
                                        arr2[1] = r[1].ToString();
                                        arr2[2] = r[2].ToString();
                                        arr2[3] = r[3].ToString();
                                        arr2[4] = r[4].ToString();
                                        arr2[5] = ((float)((float)goodcount / float.Parse(r[0].ToString()))).ToString();
                                        itms[i] = new ListViewItem(arr2);
                                        listView2.Items.Add(itms[i]);
                                    }

                                }
                            }

                        }
                        else
                        {
                            max_result.Add(0.0);
                            avg_result.Add(0.0);
                            sum_result.Add(0.0);
                            min_result.Add(0.0);
                            rate_result.Add(0.0);

                        }


                    }
                    //   listXAxis.Add(dateTimePicker2.Text);

                    break;
                case 1:

                    time_inter = m / 7;
                    for (int i = 0; i <= time_inter; i++)
                    {
                        listXAxis.Add(dateTimePicker1.Value.AddDays(7 * i).ToString("MM-dd"));
                    }

                    //listXAxis.Add(dateTimePicker2.Value.Date.ToString("MM-dd"));
                    //   SELECT * FROM detectioninfo WHERE month(test_time) = month( now( ) ) ; 

                    break;
                case 2:
                    listXAxis.Add(dateTimePicker1.Value.Month.ToString() + "月份");
                    m = m / 30;
                    for (int i = 0; i < m; i++)
                    {

                        listXAxis.Add(((dateTimePicker1.Value.Month + i) % 12 + 1).ToString() + "月份");
                    }

                    // listXAxis.Add(dateTimePicker2.Value.Month.ToString() + "月份");

                    break;
                case 3:
                    time_inter = m / 365;
                    for (int i = 0; i < time_inter; i++)
                    {
                        listXAxis.Add((dateTimePicker1.Value.Year + i).ToString() + "年份");
                    }
                    //listXAxis.Add(dateTimePicker2.Value.Year.ToString() + "年份");

                    break;
                default:
                    break;
            }


            labels = listXAxis.ToArray();
            myPane.XAxis.Scale.TextLabels = labels; //X轴文本取值
            myPane.XAxis.Type = AxisType.Text;   //X轴类型


            // Make up some data arrays based on the Sine function
            PointPairList list1 = new PointPairList();
            PointPairList list2 = new PointPairList();
            PointPairList list3 = new PointPairList();
            PointPairList list4 = new PointPairList();
            PointPairList list5 = new PointPairList();

            for (int i = 0; i < time_inter; i++)
            {
                double x = (double)i;
                double y1 = 1.5 + Math.Sin((double)i * 0.2);
                double y2 = 3.0 * (1.5 + Math.Sin((double)i * 0.2));
                double y3 = 2.5 + Math.Sin((double)i * 0.2);
                double y4 = 4.0 * (1.5 + Math.Sin((double)i * 0.2));
                double y5 = 3.5 * (1.5 + Math.Sin((double)i * 0.2));
                list1.Add(x, sum_result[i]);
                list1.Add(x, rate_result[i]);
                list2.Add(x, avg_result[i]);
                list3.Add(x, max_result[i]);
                list4.Add(x, min_result[i]);

            }

            // Generate a red curve with diamond
            // symbols, and "Porsche" in the legend
            LineItem myCurve = myPane.AddCurve("检测结果数",
                list1, Color.Red, SymbolType.Diamond);
            LineItem myCurve2 = myPane.AddCurve("合格率",
                list2, Color.Blue, SymbolType.Circle);
            LineItem myCurve3 = myPane.AddCurve("检测结果均值",
               list3, Color.Green, SymbolType.Triangle);
            LineItem myCurve4 = myPane.AddCurve("最大值",
                list4, Color.Pink, SymbolType.Plus);
            LineItem myCurve5 = myPane.AddCurve("最小值",
               list5, Color.Black, SymbolType.Star);

            zgc.AxisChange();
            Refresh();
        }
        /// <summary>
        ///   绘制柱状
        /// </summary>
        public void BarGraph(ZedGraphControl zgc, string sql)
        {
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();
            myPane.GraphObjList.Clear();
            // Set the titles and axis labels
            myPane.Title.Text = "企业排名";
            myPane.XAxis.Title.Text = "企业名称";
            myPane.YAxis.Title.Text = "检测合格率";
            double[] y = { 100, 115, 75, 22, 98, 40 };
            double[] y2 = { 90, 100, 95, 35, 80, 35 };
            double[] y3 = { 80, 110, 65, 15, 54, 67 };
            double[] y4 = { 120, 125, 100, 40, 105, 75 };
            // Make up some random data points
            List<string> lsName = new List<string>();
            foreach (ListViewItem lv in listView1.Items)
            {
                string name = lv.SubItems[0].ToString();
                if (lsName.Contains(name))
                {
                    continue;
                }
                else
                {
                    lsName.Add(name);
                }
            }
            string[] labels = new string[lsName.Count];
            int j = 0;
            foreach (string str in lsName)
            {
                BarItem myBar = myPane.AddBar(str.ToString(), null, y, Color.Red);
                myBar.Bar.Fill = new Fill(Color.Red, Color.White, Color.Red);

                labels[j] = str;
                j++;
            }



            //             // Generate a red bar with "Curve 1" in the legend
            //             BarItem myBar = myPane.AddBar("Curve 1", null, y, Color.Red);
            //             myBar.Bar.Fill = new Fill(Color.Red, Color.White, Color.Red);
            // 
            //             // Generate a blue bar with "Curve 2" in the legend
            //             myBar = myPane.AddBar("Curve 2", null, y2, Color.Blue);
            //             myBar.Bar.Fill = new Fill(Color.Blue, Color.White, Color.Blue);
            // 
            //             // Generate a green bar with "Curve 3" in the legend
            //             myBar = myPane.AddBar("Curve 3", null, y3, Color.Green);
            //             myBar.Bar.Fill = new Fill(Color.Green, Color.White, Color.Green);
            // 
            //             // Generate a black line with "Curve 4" in the legend
            //             LineItem myCurve = myPane.AddCurve("Curve 4",
            //                 null, y4, Color.Black, SymbolType.Circle);
            //             myCurve.Line.Fill = new Fill(Color.White, Color.LightSkyBlue, -45F);
            // 
            //             // Fix up the curve attributes a little
            //             myCurve.Symbol.Size = 8.0F;
            //             myCurve.Symbol.Fill = new Fill(Color.White);
            //             myCurve.Line.Width = 2.0F;

            // Draw the X tics between the labels instead of at the labels
            myPane.XAxis.MajorTic.IsBetweenLabels = true;

            // Set the XAxis labels
            myPane.XAxis.Scale.TextLabels = labels;
            // Set the XAxis to Text type
            myPane.XAxis.Type = AxisType.Text;

            // Fill the axis area with a gradient
            myPane.Chart.Fill = new Fill(Color.White,
                Color.FromArgb(255, 255, 166), 90F);
            // Fill the pane area with a solid color
            myPane.Fill = new Fill(Color.FromArgb(250, 250, 255));
            zgc.AxisChange();
            Refresh();
        }
        /// <summary>
        ///   绘图类型
        /// </summary>
        private void Selected_Type(object sender, EventArgs e)
        {
            int i = this.collectType.SelectedIndex;
            zedGraphControl1.GraphPane.CurveList.Clear(); //清除画板
            GraphPane myPane = zedGraphControl1.GraphPane;
            //             switch (i)
            //             {
            //                 case 0:
            // 
            //                     BarGraph(zedGraphControl1, null);
            // 
            //                    
            //                     break;
            //                 case 1:
            //                     PieGraph(zedGraphControl1, null);
            //                     break;
            //                 case 2:
            //                     LineGraph(zedGraphControl1, null);
            //                     break;
            // 
            //                     default:
            //                     break;
            //             }


        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            zedGraphControl3.GraphPane.CurveList.Clear(); //清除画板
            Refresh();
        }

        private void Date1_ValueChange(object sender, EventArgs e)
        {

            if (dateTimePicker7.Value > dateTimePicker8.Value)
            {
                MessageBox.Show("日期不合理，请重新填写！！！", "警告", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }
            fromdate = dateTimePicker7.Text;
        }

        private void Date2_Valuechange(object sender, EventArgs e)
        {
            if (dateTimePicker8.Value < dateTimePicker7.Value)
            {
                MessageBox.Show("日期不合理，请重新填写！！！", "警告", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            todate = dateTimePicker8.Value.ToString();
        }

        private void Type_SelelctChange(object sender, EventArgs e)
        {
            condition[(int)control.testType] = TestType.Text;

            string siteType;
            string sql;

            siteType = TestType.Text;

            // 根据用户选择的检测类型，对应出相应的检测项目
            ComboBoxDataBase boxDataBase = new ComboBoxDataBase();
            sql = "SELECT p_name FROM detectioninfo WHERE type = '" + siteType + "'";

            boxDataBase.BindDataBase(ref this.ProName, ref database, sql);
        }

        private void exportData_Click_1(object sender, EventArgs e)
        {
            string fileName = "未命名";
            //fileOperate.DataBaseToExcel1(dataGridView1,sqlInit,fileName);

            DataSet ds = new DataSet();

            database.ReadDataBase(sqlInit, "historydata", ds);
            System.Data.DataTable dtInfo = new System.Data.DataTable();
            dtInfo = ds.Tables["historydata"];

            if (dtInfo.Rows.Count <= 0)
            {
                MessageBox.Show("没有数据！！！", "提示信息", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            dtInfo.Columns[0].ColumnName = "检测项目";
            dtInfo.Columns[1].ColumnName = "检测通道";
            dtInfo.Columns[2].ColumnName = "样品名称";
            dtInfo.Columns[3].ColumnName = "送检单位";
            dtInfo.Columns[4].ColumnName = "判定结果";
            dtInfo.Columns[5].ColumnName = "检测结果";



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


            MessageBox.Show("导出完毕", "提示信息", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
        }

        private void collectType1_SelectedIndexChanged(object sender, EventArgs e)
        {
            condition1[(int)control.proName] = detect_Pro.Text;
            string sql = null;
            string testUnit;
            testUnit = detect_Pro.Text;


            // 根据用户选择的场地，对应出相应的采集器
            ComboBoxDataBase boxDataBase = new ComboBoxDataBase();
            sql = "SELECT DISTINCT Company_Class FROM detectioninfo,companyinfo WHERE detectioninfo.submission_unit = companyinfo.Company_Name AND detectioninfo.p_name = '" + testUnit + "'";

            boxDataBase.BindDataBase(ref this.collect_class, ref database, sql);

        }

        /// <summary>
        ///   趋势分析
        /// </summary>
        /// <summary>
        /// 
        private void LoadResultHeader()
        {
            listView2.Clear();
            listView2.View = View.Details;
            listView2.GridLines = true;
            listView2.FullRowSelect = true;

            //Add column header
            listView2.Columns.Add("检测总数量", 120);
            listView2.Columns.Add("检测结果平均值", 120);
            listView2.Columns.Add("检测结果最大值", 120);
            listView2.Columns.Add("检测结果最小值", 120);
            listView2.Columns.Add("检测时间", 120);
            listView2.Columns.Add("检测结果合格率", 120);

        }
        ///   企业排名表头
        /// </summary>
        void LoadHeader()
        {

            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;

            //Add column header
            listView1.Columns.Add("企业名称", 120);
            listView1.Columns.Add("样品类型", 120);
            listView1.Columns.Add("检测项目", 120);
            listView1.Columns.Add("企业分类", 120);
            listView1.Columns.Add("检测数量", 120);
            listView1.Columns.Add("合格率", 120);
            listView1.Columns.Add("吸光度", 120);
            listView1.Columns.Add("检测时间", 120);

        }
        /// <summary>
        ///   返回合格率
        /// </summary>
        private double GetPercent(string sql, DateTime dt1, DateTime dt2)
        {
            double m_percent = 0.0;
            DataSet dataSet = new DataSet();
            dataSet.Clear();
            System.Data.DataTable table = new System.Data.DataTable();
            int ret = database.ReadDataBase(sql, "Company", dataSet);
            table = dataSet.Tables["Company"];


            return m_percent;
        }
        /// <summary>
        ///   得到时间段
        /// </summary>
        private void GetTime(string sql)
        {
            int goodcount = 0;
            int numcount = 0;
            string sqlmodel = "";
            string sqlmodelcount = "";
            //查询数据
            DataSet dataSet = new DataSet();
            dataSet.Clear();
            System.Data.DataTable table = new System.Data.DataTable();
            int ret = database.ReadDataBase(sql, "result", dataSet);
            table = dataSet.Tables["result"];
            #region

            this.listView1.Items.Clear();
            this.listView1.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度 


            #endregion
            string[] arr = new string[8];
            //            switch (this.comboBox1.SelectedIndex)
            //            {
            // 
            //                case 0:
            //                    string date = dateTimePicker3.Value.ToString("yyyy-MM-dd");
            //所有的
            sqlmodel = sql;
            /*" AND test_time = '" + date + "'";*/
            //合格的
            sqlmodelcount = sqlmodel +
                         " AND Juge_result ='合格'";

            DataSet dataS = new DataSet();
            dataS.Clear();
            System.Data.DataTable table2 = new System.Data.DataTable();
            int ret2 = database.ReadDataBase(sqlmodelcount, "res", dataS);
            table2 = dataS.Tables["res"];
            goodcount = table2.Rows.Count;

            DataSet dataSet1 = new DataSet();
            dataSet1.Clear();
            System.Data.DataTable table1 = new System.Data.DataTable();
            int ret1 = database.ReadDataBase(sqlmodel, "res", dataSet1);
            table1 = dataSet1.Tables["res"];
            numcount = table1.Rows.Count;
            if (table1.Rows.Count > 0)
            {
                ListViewItem[] itms = new ListViewItem[table1.Rows.Count];
                int k = 0;

                foreach (DataRow r in table1.Rows)
                {
                    arr[0] = r[2].ToString();
                    arr[1] = r[1].ToString();
                    arr[2] = r[0].ToString();
                    arr[3] = r[7].ToString();
                    arr[4] = numcount.ToString();
                    arr[5] = ((double)goodcount / (double)numcount * 100.0).ToString() + "%";
                    arr[6] = r[6].ToString();
                    arr[7] = r[3].ToString();
                    itms[k] = new ListViewItem(arr);
                    listView1.Items.Add(itms[k]);
                    k++;
                }
            }

            this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。  
        }
        /// <summary>
        ///   对企业排名进行统计分析
        /// </summary
        private void CompanySelectData_Click(object sender, EventArgs e)
        {
            //查询数据
            int mcount = 0;//统计条数
            int i;

            string sqlModel = "SELECT p_name,type,submission_unit,test_time,Detect_result,Juge_result,Absolut_result,companyinfo.Company_Class FROM detectioninfo  " +
                            " INNER JOIN ftinfor ON (detectioninfo.p_name = ftinfor.ftestitems AND detectioninfo.type = ftinfor.fsample AND detectioninfo.standard = ftinfor.standards) INNER JOIN companyinfo ON detectioninfo.submission_unit = companyinfo.Company_Name  ";

            //  " WHERE type = '食品' AND p_name = '测试' AND Company_Class = '食品'";


            //检查comboBox是空的情况          
            if (comboBox_style.Text.Equals(""))
            {
                condition1[(int)control.testType] = comboBox_style.Text;
            }
            if (detect_Pro.Text.Equals(""))
            {
                condition1[(int)control.proName] = detect_Pro.Text;
            }
            if (collect_class.Text.Equals(""))
            {
                condition1[(int)control.comany_Class] = collect_class.Text;
            }


            //根据复选框的条件来拼接sql语句，默认情况下时间控件的值是当天的时间值
            for (i = 0; i < condition.Length; i++)
            {
                if (condition1[i].Equals(""))
                {
                    continue;

                }
                else
                {

                    if (mcount == 0)
                    {
                        sqlModel += " WHERE ";

                        sqlModel += CreateSql(condition1, i);


                    }
                    else
                    {
                        sqlModel += " and ";
                        sqlModel += CreateSql(condition1, i);
                    }
                    mcount++;
                }
            }

            // sqlModel += "GROUP BY submission_unit ";
            GetTime(sqlModel);
            //  DataGridViewRow r = new DataGridViewRow();
            BarGraph(zedGraphControl2, null);

        }

        private void Type_Selectch(object sender, EventArgs e)
        {
            condition1[(int)control.testType] = comboBox_style.Text;

            string siteType;
            string sql;

            siteType = comboBox_style.Text;

            // 根据用户选择的检测类型，对应出相应的检测项目
            ComboBoxDataBase boxDataBase = new ComboBoxDataBase();
            sql = "SELECT DISTINCT p_name FROM detectioninfo WHERE type = '" + siteType + "'";

            boxDataBase.BindDataBase(ref this.detect_Pro, ref database, sql);
        }

        private void Company_ClassSelect(object sender, EventArgs e)
        {
            condition1[(int)control.comany_Class] = collect_class.Text;

            string siteType;
           

            siteType = collect_class.Text;

            //             // 根据用户选择的检测类型，对应出相应的检测项目
            //             ComboBoxDataBase boxDataBase = new ComboBoxDataBase();
            //             sql = "SELECT Company_Class FROM companyinfo WHERE Company_Class = '" + siteType + "'";
            // 
            //             boxDataBase.BindDataBase(ref this.collect_class, ref database, sql);
        }

        private void exportWeather_Click_1(object sender, EventArgs e)
        {

        }

    }

    class HistoryCurve
    {
        public PointPairList pointList;
        public string sql;

        public string CreateCurveSql(int pageCount)
        {
            string sqlReturn = sql;
            // 第一次只加载20条
            if (pageCount == 1)
            {
                sqlReturn += System.Convert.ToString(pageCount * 20);
            }
            else
            {
                sqlReturn += System.Convert.ToString((pageCount - 1) * 10000 + 20);
            }

            sqlReturn += " ,10000";

            return sqlReturn;
        }
    }
}
