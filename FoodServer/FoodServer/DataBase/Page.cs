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
using System.IO;
using MySql.Data;
using MySql.Data.MySqlClient;
using GHCS.DataBase;
using System.Data.OleDb;
using GHCS.Common;


namespace GHCS
{
    class Page
    {
        //公有成员
        public int pageSize = 0;//每页显示的行数
        public int nMax = 0;//总记录数
        public int pageCount = 0;//页数=总记录数/每页显示行数
        public int pageCurrent = 0;//当前页号
        public int nCurrent = 0;//当前记录行数

        //创建数据库的对象
        IDataBase database = MySqlDataBase.getInstance();


        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitDataSet()
        {
            pageCount = (nMax / pageSize);//计算出总页数
            if ((nMax % pageSize) > 0)
                pageCount++;
            if (pageCount <= 0)
            {
                pageCurrent = 0;//当没有记录时当前页数从0开始
            }
            else
            {
                pageCurrent = 1;//当前页数从1开始
            }
            nCurrent = 0;//当前记录数从0开始

        }

        /// <summary>
        /// 载入数据
        /// </summary>
        public void LoadData(DataGridView dgv, BindingSource bis, BindingNavigator bin, string sql,Page page)
        {
            int nStartPos = 0;   //当前页面开始记录行
            int nEndPos = 0;     //当前页面结束记录行

            string sqlExec = sql + " LIMIT " + page.nCurrent + "," + page.pageSize + "";

            DataSet ds = new DataSet();
            System.Data.DataTable dtInfo = new System.Data.DataTable();

            database.ReadDataBase(sqlExec, "historydata", ds);

            //dataGridView1.DataSource = ds.Tables["historydata"];    
            dgv.DataSource = ds.Tables["historydata"];
            dtInfo = ds.Tables["historydata"];




            System.Data.DataTable dtTemp = dtInfo.Clone();   //克隆DataTable结构框架


            nEndPos = dtInfo.Rows.Count;

            //txtCurrentPage.Text = Convert.ToString(pageCurrent);
            //totalPageCount.Text = Convert.ToString(pageCount);


            //从元数据源复制记录行
            for (int i = nStartPos; i < nEndPos; i++)
            {
                dtTemp.ImportRow(dtInfo.Rows[i]);
                nCurrent++;
            }
            bis.DataSource = dtTemp;
            bin.BindingSource = bis;
            dgv.DataSource = bis;

        }


        /// <summary>
        /// 数据加载到界面
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="sql"></param>
        /// <param name="page"></param>
        public List<string> LoadData(DataGridView dgv, string sql, int columns, Page page)
        {
            string sqlExec = sql + " LIMIT " + page.nCurrent + "," + page.pageSize + "";

            List<string> list = new List<string>();

            int rows = database.QueryTable(sqlExec, list, columns);

            if (list.Count <= 0)
            {
                return null;
            }

            dgv.Rows.Clear();
            for (int i = 0; i < rows; i++)
            {
                dgv.Rows.Add();
                for (int j = 0; j < columns; j++)
                {
                    dgv.Rows[i].Cells[j].Value = list[i * columns + j];
                }
            }

            return list;
        }
    }
}
