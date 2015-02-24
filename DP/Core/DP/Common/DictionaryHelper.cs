using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Data;

namespace DP.Common
{
    public static class DictionaryHelper
    {
        /// <summary>
        /// 将枚举转换成 Dictionary<string, string>
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static Dictionary<string, string> EnumToDictionary(Type enumType)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (int i in Enum.GetValues(enumType))
            {
                string name = Enum.GetName(enumType, i);
                list.Add(i.ToString(), name);
            }
            return list;
        }

        /// <summary>
        /// 将List《T》转换成  Dictionary<string, string>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ListToDictionary<T>(List<T> list, string key, string value)
        {
            string strKey = string.Empty;
            string strValue = string.Empty;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (T obj in list)
            {
                strKey = string.Empty;
                strValue = string.Empty;
                PropertyInfo[] properties = obj.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if (property.GetValue(obj, null) != null)
                    {
                        if (property.Name.ToLower().Equals(key.ToLower()))
                        {
                            strKey = property.GetValue(obj, null).ToString();
                        }
                        if (property.Name.ToLower().Equals(value.ToLower()))
                        {
                            strValue = property.GetValue(obj, null).ToString();
                        }
                    }
                }
                if (!String.IsNullOrEmpty(strKey) && !String.IsNullOrEmpty(strValue))
                {
                    dict.Add(strKey, strValue);
                }
            }
            return dict;
        }

        /// <summary>
        /// 将DataTable 转换成  Dictionary<string, string>
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ListToDictionary(DataTable dt, string key, string value)
        {
            string strKey = string.Empty;
            string strValue = string.Empty;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                strKey = string.Empty;
                strValue = string.Empty;
                if (String.IsNullOrEmpty(key))
                {
                    strKey = i.ToString();
                }
                else
                {
                    strKey = dr[key].ToString();
                }
                if (String.IsNullOrEmpty(value))
                {
                    strValue = "";
                }
                else
                {
                    strValue = dr[value].ToString();
                }
                if (!String.IsNullOrEmpty(strKey) && !String.IsNullOrEmpty(strValue))
                {
                    dict.Add(strKey, strValue);
                }

            }
            return dict;
        }

        /// <summary>
        /// 将Dictionary《string,string》绑定到ListControl
        /// </summary>
        /// <param name="listControl">ListControl</param>
        /// <param name="enumType">Dictionary《string,string》</param>
        public static void FillListControl(ListControl listControl, Dictionary<string, string> dict)
        {
            listControl.Items.Clear();
            listControl.DataSource = dict;
            listControl.DataValueField = "key";
            listControl.DataTextField = "value";
            listControl.DataBind();

        }

        /// <summary>
        /// 获取 Keys 的字符串数组
        /// </summary>
        /// <param name="dict">The dict.</param>
        /// <returns></returns>
        public static string[] GetDictionaryKeys(Dictionary<string, string> dict)
        {
            string[] keys = new string[dict.Count];
            dict.Keys.CopyTo(keys, 0);
            return keys;
        }

        /// <summary>
        /// 获取 Values 的字符串数组
        /// </summary>
        /// <param name="dict">The dict.</param>
        /// <returns></returns>
        public static string[] GetDictionaryValues(Dictionary<string, string> dict)
        {
            string[] values = new string[dict.Count];
            dict.Values.CopyTo(values, 0);
            return values;
        }

    }
}
