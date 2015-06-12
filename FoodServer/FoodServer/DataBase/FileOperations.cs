using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Office.Interop.Excel;
using System.Collections;
using ZedGraph;
using System.IO;
using MySql.Data;
using MySql.Data.MySqlClient;
using GHCS.DataBase;
using System.Data.OleDb;
using GHCS.Common;

namespace GHCS
{
    class FileOperations
    {
        //创建数据库的对象
        IDataBase database = MySqlDataBase.getInstance();


        /// <summary>
        /// 通过dataGridView把数据库中的数据导入到Excel中
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="sql"></param>
        /// <param name="isShowExcle"></param>
        /// <returns></returns>
        public bool DataBaseToExcel(DataGridView dgv, string sql, bool isShowExcle)
        {

            DataSet ds = new DataSet();

            database.ReadDataBase(sql, "historydata", ds);
            dgv.DataSource = ds.Tables["historydata"];

            if (dgv.Rows.Count == 0)
                return false;

            //建立Excel对象   
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            excel.Application.Workbooks.Add(true);
            excel.Visible = isShowExcle;

            //生成字段名称   
            for (int i = 0; i < dgv.ColumnCount; i++)
            {
                excel.Cells[1, i + 1] = dgv.Columns[i].HeaderText;
            }

            //填充数据   
            for (int i = 0; i < dgv.RowCount; i++)
            {
                for (int j = 0; j < dgv.ColumnCount; j++)
                {
                    if (dgv[j, i].ValueType == typeof(string))
                    {
                        excel.Cells[i + 2, j + 1] = "'" + dgv[j, i].Value.ToString();
                    }
                    else
                    {
                        excel.Cells[i + 2, j + 1] = dgv[j, i].Value.ToString();
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 通过dataTable把数据库中数据导出到Excel中
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="sql"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public void DataBaseToExcel1(DataGridView dgv, string sql,string fileName)
        {

            DataSet ds = new DataSet();

            database.ReadDataBase(sql, "historydata", ds);
            System.Data.DataTable dtInfo = new System.Data.DataTable();
            dtInfo = ds.Tables["historydata"];

            if (dtInfo.Rows.Count == 0)
            {
                MessageBox.Show("没有数据！！！","提示信息");
                return;
            }

            string saveFileName = "";
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xls";
            saveDialog.Filter = "Excel文件|*.xls";
            saveDialog.FileName = fileName;
            saveDialog.ShowDialog();
            saveFileName = saveDialog.FileName;

            if (saveFileName.IndexOf(":") < 0)
                return ;

            //建立Excel对象   
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

            if (excel == null)
            {
                MessageBox.Show("无法创建Excel对象，可能您的机子未安装Excel","警告");
                return;
            }

            System.Reflection.Missing miss = System.Reflection.Missing.Value;   
            Microsoft.Office.Interop.Excel.Workbooks workbooks = excel.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook=workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet;
            worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得sheet1 

            int countItem = 0;//记录条数
            int k = 0;


            //生成字段名称   
            for (int i = 0; i < dgv.ColumnCount; i++)
            {
                excel.Cells[1, i + 1] = dgv.Columns[i].HeaderText;
            }

            //填充数据   
            for (int i = 0; i < dtInfo.Rows.Count; i++)
            {

                countItem = i;
                if (countItem % 65535 == 0 && countItem > 0)//一个sheet最多容纳数据条数65535
                {
                    //新加sheet
                    worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.Add(miss, miss, miss, miss);
                    //生成字段名称   
                    for (int ihead = 0; ihead < dgv.ColumnCount; ihead++)
                    {
                        excel.Cells[1, ihead + 1] = dgv.Columns[ihead].HeaderText;
                    }
                    k = 0;


                }  
                for (int j = 0; j < dtInfo.Columns.Count; j++)
                {
                    if (dtInfo.Rows[i][j] == typeof(string))
                    {
                          worksheet.Cells[k + 2, j + 1] = "'" + dtInfo.Rows[i][j].ToString();

                    }
                    else
                    {
                         worksheet.Cells[k + 2, j + 1] = dtInfo.Rows[i][j].ToString();

                    }

                }
                k++;
                
               
            }
            
            if (saveFileName != "")
            {
                try
                {
                    workbook.Saved = true;
                    
                    workbook.SaveCopyAs(saveFileName);

                    
                    
                    MessageBox.Show(saveFileName + "文件保存成功", "提示");

                }
                catch (Exception ex)
                {
                    MessageBox.Show("导出文件时出错,文件可能正被打开！\n" + ex.Message);

                }

            }
            excel.Quit();
            GC.Collect();//垃圾回收

          
        }


        /// <summary>
        /// 把DataGridview控件中的数据导出到excel表中，这个方法使用了Microsoft.Office.Interop.Excel
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="isShowExcle"></param>
        /// <returns></returns>
        public bool DataGridviewShowToExcel(DataGridView dgv, bool isShowExcle)
        {
            if (dgv.Rows.Count == 0)
                return false;

            //建立Excel对象   
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            excel.Application.Workbooks.Add(true);
            excel.Visible = isShowExcle;

            //生成字段名称   
            for (int i = 0; i < dgv.ColumnCount; i++)
            {
                excel.Cells[1, i + 1] = dgv.Columns[i].HeaderText;
            }
            //填充数据   
            for (int i = 0; i < dgv.RowCount - 1; i++)
            {
                for (int j = 0; j < dgv.ColumnCount; j++)
                {
                    if (dgv[j, i].ValueType == typeof(string))
                    {
                        excel.Cells[i + 2, j + 1] = "'" + dgv[j, i].Value.ToString();
                    }
                    else
                    {
                        excel.Cells[i + 2, j + 1] = dgv[j, i].Value.ToString();
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 把DataGridview控件中的数据导出到excel表中，这个方法没有使用Microsoft.Office.Interop.Excel
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="isShowExcle"></param>
        /// <returns></returns>
        public void DataGridViewToExcel(DataGridView dgv)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Execl files (*.xls)|*.xls";
            dlg.FilterIndex = 0;
            dlg.RestoreDirectory = true;
            dlg.CreatePrompt = true;
            dlg.Title = "保存为Excel文件";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Stream myStream;
                myStream = dlg.OpenFile();
                StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding(-0));
                string columnTitle = "";
                try
                {
                    //写入列标题  
                    for (int i = 0; i < dgv.ColumnCount; i++)
                    {
                        if (i > 0)
                        {
                            columnTitle += "\t";
                        }
                        columnTitle += dgv.Columns[i].HeaderText;
                    }
                    sw.WriteLine(columnTitle);

                    //写入列内容  
                    for (int j = 0; j < dgv.Rows.Count; j++)
                    {
                        string columnValue = "";
                        for (int k = 0; k < dgv.Columns.Count; k++)
                        {
                            if (k > 0)
                            {
                                columnValue += "\t";
                            }
                            if (dgv.Rows[j].Cells[k].Value == null)
                                columnValue += "";
                            else
                                columnValue += dgv.Rows[j].Cells[k].Value.ToString().Trim();
                        }
                        sw.WriteLine(columnValue);
                    }
                    sw.Close();
                    myStream.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
                finally
                {
                    sw.Close();
                    myStream.Close();
                }
            }
        }

        /// <summary>
        /// 导入数据到DataGridView控件中
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="dgv"></param>
        public int EcxelToDataGridView(string filePath, DataGridView dgv)
        {
            //根据路径打开一个Excel文件并将数据填充到DataSet中
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source = " + filePath + ";Extended Properties ='Excel 8.0;HDR=YES;IMEX=1'";//HDR=YES 有两个值:YES/NO,表示第一行是否字段名,默认是YES,第一行是字段名
            OleDbConnection conn1 = new OleDbConnection(strConn);
            conn1.Open();
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            strExcel = "select  * from   [sheet1$]";
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            myCommand.Fill(ds, "table1");

            //根据DataGridView的列构造一个新的DataTable
            System.Data.DataTable tb = new System.Data.DataTable();
            foreach (DataGridViewColumn dgvc in dgv.Columns)
            {
                if (dgvc.Visible && dgvc.CellType != typeof(DataGridViewCheckBoxCell))
                {
                    DataColumn dc = new DataColumn();
                    dc.ColumnName = dgvc.DataPropertyName;
                    //dc.DataType = dgvc.ValueType;//若需要限制导入时的数据类型则取消注释，前提是DataGridView必须先绑定一个数据源那怕是空的DataTable
                    tb.Columns.Add(dc);
                }
            }

            //根据Excel的行逐一对上面构造的DataTable的列进行赋值
            foreach (DataRow excelRow in ds.Tables[0].Rows)
            {
                int i = 0;
                DataRow dr = tb.NewRow();

                foreach (DataColumn dc in tb.Columns)
                {
                    dr[dc] = excelRow[i];

                    i++;
                }
                tb.Rows.Add(dr);
            }
            //在DataGridView中显示导入的数据
            dgv.DataSource = tb;

            return 1;
        }

        /// <summary>
        /// 把Excel表的数据导入到数据库表中
        /// 数据库中需要有一个确定的表
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public int EcxelToDataBase(string filePath)
        {
            //根据路径打开一个Excel文件并将数据填充到DataSet中
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source = " + filePath + ";Extended Properties ='Excel 8.0;HDR=YES;IMEX=1'";//HDR=YES 有两个值:YES/NO,表示第一行是否字段名,默认是YES,第一行是字段名
            OleDbConnection conn1 = new OleDbConnection(strConn);
            conn1.Open();
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            strExcel = "select  * from   [sheet1$]";
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            myCommand.Fill(ds, "table1");



            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string sql = "replace into new values (" + dr["实时数据ID"] + ",'" + dr["网关"] + "'," + dr["采集数据值"] + ",'" + dr["采集数据时间"] + "')";
                database.ExcuteNonQuery(sql);

            }

            MessageBox.Show("导入数据成功！！！");

            return 1;
        }

        /// <summary>
        /// 打开Excel表格
        /// </summary>
        /// <returns></returns>
        public string OpenExcel()
        {
            //打开一个文件选择框
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Excel文件";
            ofd.FileName = "";
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);//为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            ofd.Filter = "Excel文件(*.xls)|*.xls";
            ofd.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            ofd.CheckFileExists = true;  //验证路径有效性
            ofd.CheckPathExists = true; //验证文件有效性


            string strName = string.Empty;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                strName = ofd.FileName;
            }

            if (strName == "")
            {
                MessageBox.Show("没有选择Excel文件！无法进行数据导入");
                return "";
            }

            return strName;
        }

      
    }
}
