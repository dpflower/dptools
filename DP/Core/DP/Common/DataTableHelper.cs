using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web.UI;
using System.Web;
using System.Collections;

namespace DP.Common
{
    public static class DataTableHelper
    {
        #region ��ȡDataTable ָ����  ָ���е�ֵ
        /// <summary>
        /// ��ȡ DataTable  ָ����  ָ���е�ֵ
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string GetColumnValueToString(DataTable dt, int rowIndex, string columnName)
        {
            if (dt.Columns.Contains(columnName))
            {
                if (dt.Rows.Count > rowIndex)
                {
                    return dt.Rows[rowIndex][columnName].ToString();
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// ��ȡ DataTable  ָ����  ָ���е�ֵ
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int GetColumnValueToInt(DataTable dt, int rowIndex, string columnName)
        {
            if (dt.Columns.Contains(columnName))
            {
                if (dt.Rows.Count > rowIndex)
                {
                    return StringHelper.ToInt(dt.Rows[rowIndex][columnName].ToString());
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// ��ȡ DataTable  ָ����  ָ���е�ֵ
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static long GetColumnValueToLong(DataTable dt, int rowIndex, string columnName)
        {
            if (dt.Columns.Contains(columnName))
            {
                if (dt.Rows.Count > rowIndex)
                {
                    return StringHelper.ToLong(dt.Rows[rowIndex][columnName].ToString());
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// ��ȡ DataTable  ָ����  ָ���е�ֵ
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static decimal GetColumnValueToDecimal(DataTable dt, int rowIndex, string columnName)
        {
            if (dt.Columns.Contains(columnName))
            {
                if (dt.Rows.Count > rowIndex)
                {
                    return StringHelper.ToDecimal(dt.Rows[rowIndex][columnName].ToString());
                }
                else
                {
                    return (decimal)0;
                }
            }
            else
            {
                return (decimal)0;
            }
        } 
        #endregion

        /// <summary>
        /// Gets the row.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetRow(DataTable dt, int rowIndex)
        {
            Dictionary<string, string> dicts = new Dictionary<string, string>();
            if (dt == null)
            {
                return dicts;
            }
            if (dt.Rows.Count == 0)
            {
                return dicts;
            }
            if (dt.Rows.Count < rowIndex)
            {
                return dicts;
            }
            string fieldKey;
            string fieldValue;
            foreach (DataColumn column in dt.Columns)
            {
                fieldKey = column.ColumnName;
                if (column.DataType == Type.GetType("System.DateTime"))
                {
                    fieldValue = DateTimeHelper.Format(dt.Rows[rowIndex][column.ColumnName].ToString(), "yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    fieldValue = dt.Rows[rowIndex][column.ColumnName].ToString();
                }
                dicts.Add(fieldKey.ToLower(), fieldValue);
            }
            return dicts;

        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DataTable GenerationTestDataTable()
        {
            DataTable dt = new DataTable();


            return dt;
        }

        #region ������Excel��
        /// <summary>
        /// �� DataTable  ���ݵ��� �� Excel��
        /// </summary>
        /// <param name="fileName">Excel�ļ���</param>
        /// <param name="dt">Ҫ�ĳ������ݱ�</param>
        /// <param name="dictHead">Ҫ��������������Ӧ�õ��ֶ�����</param>
        public static void OutputExcel(string fileName, DataTable dt, Dictionary<string, string> dictHead)
        {
            OutputExcel(fileName, dt, dictHead, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// �� DataTable  ���ݵ��� �� Excel��
        /// </summary>
        /// <param name="fileName">Excel�ļ���</param>
        /// <param name="dt">Ҫ�ĳ������ݱ�</param>
        /// <param name="dictHead">Ҫ��������������Ӧ�õ��ֶ�����</param>
        /// <param name="encoding">�������ַ�����</param>
        public static void OutputExcel(string fileName, DataTable dt, Dictionary<string, string> dictHead, Encoding encoding)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                OutputExcel(page, fileName, dt, dictHead, encoding);
            }
        }

        /// <summary>
        /// �� DataTable  ���ݵ��� �� Excel��
        /// </summary>
        /// <param name="page">��ǰҳ�����</param>
        /// <param name="fileName">Excel�ļ���</param>
        /// <param name="dt">Ҫ�ĳ������ݱ�</param>
        /// <param name="dictHead">Ҫ��������������Ӧ�õ��ֶ�����</param>
        /// <param name="encoding">�������ַ�����</param>
        public static void OutputExcel(Page page, string fileName, DataTable dt, Dictionary<string, string> dictHead, Encoding encoding)
        {
            if (dictHead != null)
            {
                OutputExcel(page, fileName, DataTableToString(dt, dictHead), encoding);
            }
            else
            {
                OutputExcel(page, fileName, DataTableToString(dt), encoding);
            }
        }

        /// <summary>
        /// �� DataTable  ���ݵ��� �� Excel��
        /// </summary>
        /// <param name="page">��ǰҳ�����</param>
        /// <param name="fileName">Excel�ļ���</param>
        /// <param name="text">Ҫ�ĳ��������ַ���</param>
        /// <param name="encoding">�������ַ�����</param>
        public static void OutputExcel(Page page, string fileName, string text, Encoding encoding)
        {
            if (page == null)
            {
                return;
            }
            page.Response.ContentEncoding = encoding;
            page.Response.ClearContent();
            page.Response.ClearHeaders();
            page.Response.AddHeader("content-disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(fileName, encoding) + ".xls");
            page.Response.AddHeader("Content-type", "application");
            //page.Response.ContentType = "application/ms-html";
            page.Response.ContentType = "application/ms-excel";
            page.Response.Write("<meta http-equiv=Content-Type content=text/html; charset=" + encoding.BodyName + ">");
            page.Response.Write(text);
            page.Response.Flush();
            page.Response.Close();
        } 
        #endregion

        #region ������Word��
        /// <summary>
        /// �� DataTable  ���ݵ��� �� Word��
        /// </summary>
        /// <param name="fileName">Word�ļ���</param>
        /// <param name="dt">Ҫ�ĳ������ݱ�</param>
        /// <param name="dictHead">Ҫ��������������Ӧ�õ��ֶ�����</param>
        public static void OutputWord(string fileName, DataTable dt, Dictionary<string, string> dictHead)
        {
            OutputWord(fileName, dt, dictHead, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// �� DataTable  ���ݵ��� �� Word��
        /// </summary>
        /// <param name="fileName">Word�ļ���</param>
        /// <param name="dt">Ҫ�ĳ������ݱ�</param>
        /// <param name="dictHead">Ҫ��������������Ӧ�õ��ֶ�����</param>
        /// <param name="encoding">�������ַ�����</param>
        public static void OutputWord(string fileName, DataTable dt, Dictionary<string, string> dictHead, Encoding encoding)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                OutputWord(page, fileName, dt, dictHead, encoding);
            }
        }

        /// <summary>
        /// �� DataTable  ���ݵ��� �� Word��
        /// </summary>
        /// <param name="page">��ǰҳ�����</param>
        /// <param name="fileName">Word�ļ���</param>
        /// <param name="dt">Ҫ�ĳ������ݱ�</param>
        /// <param name="dictHead">Ҫ��������������Ӧ�õ��ֶ�����</param>
        /// <param name="encoding">�������ַ�����</param>
        public static void OutputWord(Page page, string fileName, DataTable dt, Dictionary<string, string> dictHead, Encoding encoding)
        {
            if (dictHead != null)
            {
                OutputWord(page, fileName, DataTableToString(dt, dictHead), encoding);
            }
            else
            {
                OutputWord(page, fileName, DataTableToString(dt), encoding);
            }
        }

        /// <summary>
        /// �� DataTable  ���ݵ��� �� Word��
        /// </summary>
        /// <param name="page">��ǰҳ�����</param>
        /// <param name="fileName">Word�ļ���</param>
        /// <param name="text">Ҫ�ĳ��������ַ���</param>
        /// <param name="encoding">�������ַ�����</param>
        public static void OutputWord(Page page, string fileName, string text, Encoding encoding)
        {
            if (page == null)
            {
                return;
            }
            page.Response.ContentEncoding = encoding;
            page.Response.ClearContent();
            page.Response.ClearHeaders();
            page.Response.AddHeader("content-disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(fileName, encoding) + ".xls");
            page.Response.AddHeader("Content-type", "application");
            page.Response.ContentType = "application/ms-html";
            //page.Response.ContentType = "application/ms-excel";
            page.Response.Write("<meta http-equiv=Content-Type content=text/html; charset=" + encoding.BodyName + ">");
            page.Response.Write(text);
            page.Response.Flush();
            page.Response.Close();
        } 
        #endregion

        #region DataTable��ʽ�����ַ���
        /// <summary>
        /// ���� �ֵ���� ��Ӧ�õ� DataTable ���� ��ʽ����  string
        /// </summary>
        /// <param name="dt">Ҫ��ʽ�������ݱ�</param>
        /// <param name="dictHead">Ҫ��ʽ������������Ӧ�õ��ֶ�����</param>
        /// <returns></returns>
        public static string DataTableToString(DataTable dt, Dictionary<string, string> dictHead)
        {
            string td = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\"   style=\"font-size: 13;\"  >");
            sb.Append("<tr style=\"font-weight: bold; white-space: nowrap;\">");
            foreach (KeyValuePair<string, string> head in dictHead)
            {
                sb.Append("<td>");
                sb.Append(StringHelper.ReplaceEnter(head.Value));
                sb.Append("</td>");
            }
            sb.Append("</tr>");
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append("<tr>");
                foreach (KeyValuePair<string, string> head in dictHead)
                {
                    try
                    {
                        td = StringHelper.ReplaceEnter(dr[head.Key.ToLower()].ToString());
                    }
                    catch
                    {

                    }
                    sb.Append("<td style=\"vnd.ms-excel.numberformat:@\">").Append(td).Append("</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }

        /// <summary>
        /// �� DataTable ���� ��ʽ����  string
        /// </summary>
        /// <param name="dt">Ҫ��ʽ�������ݱ�</param>
        /// <returns></returns>
        public static string DataTableToString(DataTable dt)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dict.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
            }
            return DataTableToString(dt, dict);
        } 
        #endregion




    }
}
