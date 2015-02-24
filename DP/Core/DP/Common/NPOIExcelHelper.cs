using System;
using System.Collections.Generic;

using System.Text;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.IO;
using DP.Common;
using NPOI.HPSF;
using System.Data;
using System.Web.UI;
using System.Web;

namespace DP.Common
{
        /*
    [Serializable] 
    public class NPOIExcelHelper
    {
         
        #region 变量
        private static int _sheetMaxRecord = 55000;
        private static int _memoryStreamMaxRows = 10000;
        
        #endregion

        #region 属性

        /// <summary>
        /// 每个Sheet最大记录数
        /// Gets or sets the sheel max record.
        /// </summary>
        /// <value>
        /// The sheel max record.
        /// </value>
        public static int SheetMaxRecord
        {
            get { return _sheetMaxRecord; }
            set { _sheetMaxRecord = value; }
        }

        /// <summary>
        /// 内存流输出的最大记录数
        /// </summary>
        /// <value>
        /// The memory stream max rows.
        /// </value>
        public static int MemoryStreamMaxRows
        {
            get { return NPOIExcelHelper._memoryStreamMaxRows; }
            set { NPOIExcelHelper._memoryStreamMaxRows = value; }
        } 
        #endregion


        /// <summary>
        /// DataTable  数据导出到 Excel
        /// Datas the table to excel.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <param name="dicts">The dicts.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void DataTableToExcel(DataTable dataTable, Dictionary<string, string> dicts, string fileName)
        {

            Page page = HttpContext.Current.Handler as Page;
            if (page == null) return;

            int count = SheetMaxRecord;
            int dataTableRowCount = dataTable.Rows.Count;
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            for (int i = 0; i < IntHelper.divideMax(dataTableRowCount, count); i++)
            {
                SetSheet(dataTable, dicts, hssfworkbook, "sheet" + i.ToString(), i * count, count);
            }

            if (dataTableRowCount <= MemoryStreamMaxRows)
            {
                //直接下载数据流
                MemoryStream file = new MemoryStream();
                hssfworkbook.Write(file);
                hssfworkbook.Dispose();


                page.Response.Clear();
                page.Response.Charset = "UTF-8";
                page.Response.ContentEncoding = System.Text.Encoding.UTF8;
                page.Response.AddHeader("Content-Disposition", "attachment; filename=" + page.Server.UrlEncode(fileName));//对文件名编码
                //page.Response.AddHeader("Content-Length", file.Length.ToString());
                page.Response.ContentType = "application/vnd.xls";//2003
                //Response.ContentType = "application/vnd.ms-excel";//2007
                page.Response.BinaryWrite(file.GetBuffer());
                //page.Response.WriteFile(file.FullName);
                page.Response.Flush();
                file.Close();
                //page.Response.End();
            }
            else
            {
                //保存到文件 然后下载
                var _file = GetTempFile(page);
                FileStream Hfile = new FileStream(_file, FileMode.Create);
                hssfworkbook.Write(Hfile);
                Hfile.Close();
                hssfworkbook.Dispose();

                System.IO.FileInfo file = new System.IO.FileInfo(_file);

                page.Response.Clear();
                page.Response.Charset = "UTF-8";
                page.Response.ContentEncoding = System.Text.Encoding.UTF8;
                page.Response.AddHeader("Content-Disposition", "attachment; filename=" + page.Server.UrlEncode(fileName));
                //对文件名编码
                page.Response.AddHeader("Content-Length", file.Length.ToString());
                page.Response.ContentType = "application/vnd.xls"; //2003
                //Response.ContentType = "application/vnd.ms-excel";//2007
                //page.Response.BinaryWrite(file.GetBuffer());
                page.Response.WriteFile(file.FullName);
                page.Response.Flush();
                //file.Close();
                //page.Response.End();
            }
        }

        /// <summary>
        /// DataTable  数据导出到 Excel
        /// Datas the table to excel.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <param name="fieldInfos">The field infos.</param>
        /// <param name="fileName">Name of the file.</param>
        public static string DataTableToExcel(DataTable dataTable, List<FieldInfo> fieldInfos, string fileName)
        {
            string fileFullName = string.Empty;
            Page page = HttpContext.Current.Handler as Page;
            if (page == null) return fileFullName;


            int count = SheetMaxRecord;
            int dataTableRowCount = dataTable.Rows.Count;
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            for (int i = 0; i < IntHelper.divideMax(dataTableRowCount, count); i++)
            {
                SetSheet(dataTable, fieldInfos, hssfworkbook, "sheet" + i.ToString(), i * count, count);
            }

            if (dataTableRowCount <= MemoryStreamMaxRows)
            {
                //直接下载数据流
                MemoryStream file = new MemoryStream();
                hssfworkbook.Write(file);
                hssfworkbook.Dispose();


                page.Response.Clear();
                page.Response.Charset = "UTF-8";
                page.Response.ContentEncoding = System.Text.Encoding.UTF8;
                page.Response.AddHeader("Content-Disposition", "attachment; filename=" + page.Server.UrlEncode(fileName));//对文件名编码
                //page.Response.AddHeader("Content-Length", file.Length.ToString());
                page.Response.ContentType = "application/vnd.xls";//2003
                //Response.ContentType = "application/vnd.ms-excel";//2007
                page.Response.BinaryWrite(file.GetBuffer());
                //page.Response.WriteFile(file.FullName);
                page.Response.Flush();
                file.Close();
                //page.Response.End();


            }
            else
            {
                //保存到文件 然后下载
                fileFullName = GetTempFile(page, fileName);
                FileStream Hfile = new FileStream(fileFullName, FileMode.Create);
                hssfworkbook.Write(Hfile);
                Hfile.Close();
                hssfworkbook.Dispose();

                System.IO.FileInfo file = new System.IO.FileInfo(fileFullName);

                page.Response.Clear();
                page.Response.Charset = "UTF-8";
                page.Response.ContentEncoding = System.Text.Encoding.UTF8;
                page.Response.AddHeader("Content-Disposition", "attachment; filename=" + page.Server.UrlEncode(fileName));
                //对文件名编码
                page.Response.AddHeader("Content-Length", file.Length.ToString());
                page.Response.ContentType = "application/vnd.xls"; //2003
                //Response.ContentType = "application/vnd.ms-excel";//2007
                //page.Response.BinaryWrite(file.GetBuffer());
                page.Response.WriteFile(file.FullName);
                page.Response.Flush();
                //file.Close();
                //page.Response.End();


            }
            return fileFullName;
        }

        /// <summary>
        /// Lists to excel.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        public static void ListToExcel<T>(List<T> list, Dictionary<string, string> dict)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page == null) return;














        }



        /// <summary>
        /// 获取临时文件名
        /// Gets the temp file.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        private static string GetTempFile(Page page)
        {
            string _path = page.Server.MapPath("~/Temp");
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            string _file = _path.TrimEnd('\\') + "\\Temp" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            return _file;
        }

        /// <summary>
        /// 获取临时文件名
        /// Gets the temp file.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        private static string GetTempFile(Page page, string filename)
        {
            string _path = page.Server.MapPath("~/Temp");
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            string _file = _path.TrimEnd('\\') + "\\" + filename.Replace(".xls", "") + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            return _file;
        }

        /// <summary>
        /// Generators the title cell style.
        /// </summary>
        /// <param name="hssfworkbook">The hssfworkbook.</param>
        /// <returns></returns>
        private static CellStyle GeneratorTitleCellStyle(HSSFWorkbook hssfworkbook)
        {
            CellStyle style = hssfworkbook.CreateCellStyle();

            style.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            //style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREEN.index;//背景色
            //style.FillPattern = FillPatternType.BRICKS;
            //style.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.GREEN.index;
            Font font = hssfworkbook.CreateFont();
            font.Boldweight = 600;
            font.Color = NPOI.HSSF.Util.HSSFColor.BLACK.index;
            font.FontHeightInPoints = 12;
            style.SetFont(font);
            return style;
        }

        /// <summary>
        /// Generators the cell style.
        /// </summary>
        /// <param name="hssfworkbook">The hssfworkbook.</param>
        /// <returns></returns>
        private static CellStyle GeneratorCellStyle(HSSFWorkbook hssfworkbook)
        {
            CellStyle style = hssfworkbook.CreateCellStyle();
            style.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            //style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREEN.index;//背景色
            //style.FillPattern = FillPatternType.BRICKS;
            //style.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.GREEN.index;
            Font font = hssfworkbook.CreateFont();
            font.FontHeightInPoints = 10;//字体高度
            font.Color = NPOI.HSSF.Util.HSSFColor.BLACK.index;//字体颜色
            style.SetFont(font);
            return style;
        }

        /// <summary>
        /// 设置Sheet内容
        /// Sets the sheet.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <param name="dicts">The dicts.</param>
        /// <param name="hssfworkbook">The hssfworkbook.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        private static void SetSheet(DataTable dataTable, Dictionary<string, string> dicts, HSSFWorkbook hssfworkbook, string sheetName, int begin, int count)
        {
            Sheet sheet = hssfworkbook.CreateSheet(sheetName);

            Row row;
            Cell cell;
            int index = 0;
            DateTime dateTime = DateTime.Now;
            string strTemp;

            #region 表头
            row = sheet.CreateRow(0);
            index = 0;
            foreach (var dict in dicts)
            {
                cell = row.CreateCell(index++);
                cell.CellStyle = GeneratorTitleCellStyle(hssfworkbook);
                cell.SetCellValue(dict.Value);
            }

            #endregion

            #region 内容

            int skip = (dataTable.Rows.Count - begin) > count ? count : dataTable.Rows.Count - begin;

            for (int i = 0; i < skip; i++)
            {
                row = sheet.CreateRow(i + 1);
                index = 0;
                foreach (var dict in dicts)
                {
                    cell = row.CreateCell(index++);
                    strTemp = dataTable.Rows[i + begin][dict.Key].ToString();
                    switch (dataTable.Columns[dict.Key].DataType.Name)
                    {
                        case "DateTime":
                            {
                                if (DateTime.TryParse(strTemp, out dateTime))
                                {
                                    cell.SetCellValue(dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                else
                                {
                                    cell.SetCellValue(strTemp);
                                }
                            }
                            break;
                        default:
                            {
                                cell.SetCellValue(strTemp);
                            }
                            break;
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// 设置Sheet内容
        /// Sets the sheet.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <param name="fieldInfos">The field infos.</param>
        /// <param name="hssfworkbook">The hssfworkbook.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="begin">The begin.</param>
        /// <param name="count">The count.</param>
        private static void SetSheet(DataTable dataTable, List<FieldInfo> fieldInfos, HSSFWorkbook hssfworkbook, string sheetName, int begin, int count)
        {
            Sheet sheet = hssfworkbook.CreateSheet(sheetName);

            Row row;
            Cell cell;
            int index = 0;
            DateTime dateTime = DateTime.Now;
            string strTemp;

            #region 表头
            row = sheet.CreateRow(0);
            index = 0;
            foreach (var fieldInfo in fieldInfos)
            {
                cell = row.CreateCell(index++);
                cell.CellStyle = GeneratorTitleCellStyle(hssfworkbook);
                cell.SetCellValue(fieldInfo.FieldDescription);
            }

            #endregion

            #region 内容

            int skip = (dataTable.Rows.Count - begin) > count ? count : dataTable.Rows.Count - begin;

            for (int i = 0; i < skip; i++)
            {
                row = sheet.CreateRow(i + 1);
                index = 0;
                foreach (var fieldInfo in fieldInfos)
                {
                    cell = row.CreateCell(index++);
                    strTemp = dataTable.Rows[i + begin][fieldInfo.FieldName].ToString();
                    switch (dataTable.Columns[fieldInfo.FieldName].DataType.Name)
                    {
                        case "DateTime":
                            {
                                if (DateTime.TryParse(strTemp, out dateTime))
                                {
                                    cell.SetCellValue(dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                else
                                {
                                    if (fieldInfo.FieldValues.Count == 0)
                                    {
                                        cell.SetCellValue(strTemp);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            strTemp = fieldInfo.FieldValues[strTemp];
                                        }
                                        catch
                                        {

                                        }
                                        cell.SetCellValue(strTemp);
                                    }
                                }
                            }
                            break;
                        default:
                            {
                                if (fieldInfo.FieldValues.Count == 0)
                                {
                                    cell.SetCellValue(strTemp);
                                }
                                else
                                {
                                    try
                                    {
                                        strTemp = fieldInfo.FieldValues[strTemp];
                                    }
                                    catch
                                    {

                                    }
                                    cell.SetCellValue(strTemp);
                                }
                            }
                            break;
                    }
                }
            }

            #endregion
        }



        /// <summary>
        /// Excels to data table.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="sheet">The sheet.</param>
        /// <returns></returns>
        public static DataTable ExcelToDataTable(string fileName, string sheetName)
        {
            DataTable dt = new DataTable();

            HSSFWorkbook workbook;
            try
            {
                using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    workbook = new HSSFWorkbook(file);
                }

                Sheet sheet = workbook.GetSheet(sheetName.TrimEnd('$'));
                if (sheet == null)
                {
                    return dt;
                }

                //获取sheet的首行
                Row headerRow = sheet.GetRow(0);

                //一行最后一个方格的编号 即总的列数
                int cellCount = headerRow.LastCellNum;
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    dt.Columns.Add(column);
                }

                //最后一列的标号  即总的行数
                int rowCount = sheet.LastRowNum;
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    Row row = sheet.GetRow(i);
                    DataRow dataRow = dt.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            dataRow[j] = row.GetCell(j).ToString();
                        }

                    }
                    dt.Rows.Add(dataRow);
                }
                workbook.Dispose();
            }
            catch (Exception ex)
            {

            }

            return dt;
        }

        /// <summary>
        /// Gets the sheets.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetSheets(string fileName)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            HSSFWorkbook workbook;
            try
            {
                using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    workbook = new HSSFWorkbook(file);
                }
                int numSheets = workbook.NumberOfSheets;
                for (int i = 0; i < numSheets; i++)
                {
                    dict.Add(i.ToString(), workbook.GetSheetName(i));
                }

                workbook.Dispose();
            }
            catch (Exception ex)
            {

            }

            return dict;

        }


        /// <summary>
        /// 
        /// </summary>
        public class FieldInfo
        {
            public string FieldName { get; set; }
            public string FieldDescription { get; set; }
            public Dictionary<string, string> FieldValues { get; set; }

            public FieldInfo(string fieldName, string fieldDescription)
            {
                FieldName = fieldName;
                FieldDescription = fieldDescription;
                FieldValues = new Dictionary<string, string>();
            }

            public FieldInfo(string fieldName, string fieldDescription, Dictionary<string, string> fieldValues)
            {
                FieldName = fieldName;
                FieldDescription = fieldDescription;
                FieldValues = fieldValues;
            }
        }
    }
        */
}
