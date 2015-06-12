using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.POIFS;
using NPOI.Util;
using System.Windows.Forms;

namespace GHCS.Common
{
    public class ExcelHelper
    {
        /// <summary>   
        /// DataTable导出到Excel文件   
        /// </summary>   
        /// <param name="dtSource">源DataTable</param>   
        /// <param name="strHeaderText">表头文本</param>   
        /// <param name="strFileName">保存位置</param>   

        public static void Export(DataTable dtSource, string strHeaderText, string strFileName)
        {
            using (MemoryStream ms = Export(dtSource, strHeaderText))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();

                }
            }
        }

        /// <summary>   
        /// DataTable导出到Excel的MemoryStream   
        /// </summary>   
        /// <param name="dtSource">源DataTable</param>   
        /// <param name="strHeaderText">表头文本</param>   

        public static MemoryStream Export(DataTable dtSource, string strHeaderText)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet();

            #region 右击文件 属性信息
            //{
            //    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            //    dsi.Company = "http://www.yongfa365.com/";
            //    workbook.DocumentSummaryInformation = dsi;

            //    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            //    si.Author = "柳永法"; //填加xls文件作者信息   
            //    si.ApplicationName = "NPOI测试程序"; //填加xls文件创建程序信息   
            //    si.LastAuthor = "柳永法2"; //填加xls文件最后保存者信息   
            //    si.Comments = "说明信息"; //填加xls文件作者信息   
            //    si.Title = "NPOI测试"; //填加xls文件标题信息   
            //    si.Subject = "NPOI测试Demo";//填加文件主题信息   
            //    si.CreateDateTime = DateTime.Now;
            //    workbook.SummaryInformation = si;
            //}
            #endregion

            HSSFCellStyle dateStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFDataFormat format = (HSSFDataFormat)workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            //取得列宽   
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }



            int rowIndex = 0;

            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = (HSSFSheet)workbook.CreateSheet();
                    }

                    #region 表头及样式
                    //{
                    //    HSSFRow headerRow = (HSSFRow)sheet.CreateRow(0);
                    //    headerRow.HeightInPoints = 25;
                    //    headerRow.CreateCell(0).SetCellValue(strHeaderText);

                    //    HSSFCellStyle headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                    //    //cq
                    //   // headStyle.Alignment = CellHorizontalAlignment.CENTER;
                    //    HSSFFont font = (HSSFFont)workbook.CreateFont();
                    //    font.FontHeightInPoints = 20;
                    //    font.Boldweight = 700;
                    //    headStyle.SetFont(font);

                    //    headerRow.GetCell(0).CellStyle = headStyle;

                    //    //cq
                    //    //sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                    //    //headerRow.Dispose();
                    //}
                    #endregion


                    #region 列头及样式
                    {
                        HSSFRow headerRow = (HSSFRow)sheet.CreateRow(0);


                        HSSFCellStyle headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                        //headStyle.Alignment = CellHorizontalAlignment.CENTER;
                        HSSFFont font = (HSSFFont)workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);


                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                            //设置列宽   
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);

                        }
                        //headerRow.Dispose();
                    }
                    #endregion

                    rowIndex = 1;
                }
                #endregion


                #region 填充内容
                HSSFRow dataRow = (HSSFRow)sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    HSSFCell newCell = (HSSFCell)dataRow.CreateCell(column.Ordinal);

                    string drValue = row[column].ToString();

                    switch (column.DataType.ToString())
                    {
                        case "System.String"://字符串类型   
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型   
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);

                            newCell.CellStyle = dateStyle;//格式化显示   
                            break;
                        case "System.Boolean"://布尔型   
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16"://整型   
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal"://浮点型   
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull"://空值处理   
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }

                }
                #endregion


                rowIndex++;
            }


            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //sheet.Dispose();
                //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet   
                return ms;
            }

        }

    }
}
