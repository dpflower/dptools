using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Web.UI;

namespace DP.Common
{
    public static class ListHelper
    {
        /// <summary>
        /// List《T》转换成 DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(List<T> list)
        {
            DataTable dt = new DataTable();

            if (list == null || list.Count == 0)
            {
                return dt;
            }

            PropertyInfo[] propertiest = list[0].GetType().GetProperties();
            foreach (PropertyInfo property in propertiest)
            {
                DataColumn dc = new DataColumn();
                dc.ColumnName = property.Name;

                Type columnType = property.PropertyType;
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    columnType = property.PropertyType.GetGenericArguments()[0];
                }

                dc.DataType = columnType;
                dt.Columns.Add(dc);
            }

            foreach (T obj in list)
            {
                DataRow dr = dt.NewRow();
                PropertyInfo[] properties = obj.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    dr[property.Name] = property.GetValue(obj, null).ToString();
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        /// <summary>
        /// List《T》转换成 DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(Page page, List<T> list)
        {
            DataTable dt = new DataTable();

            if (list == null || list.Count == 0)
            {
                return dt;
            }

            PropertyInfo[] propertiest = list[0].GetType().GetProperties();
            foreach (PropertyInfo property in propertiest)
            {
                DataColumn dc = new DataColumn();
                dc.ColumnName = property.Name;

                Type columnType = property.PropertyType;
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    columnType = property.PropertyType.GetGenericArguments()[0];
                }

                dc.DataType = columnType;
                dt.Columns.Add(dc);
            }

            foreach (T obj in list)
            {
                DataRow dr = dt.NewRow();
                PropertyInfo[] properties = obj.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    dr[property.Name] = property.GetValue(obj, null).ToString();
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

    }
}
