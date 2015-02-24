using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Web.UI.WebControls;

namespace DP.Common
{
    public static class HashtableHelper
    {
        /// <summary>
        /// 设置 Hashtable 键 值
        /// </summary>
        /// <param name="ht">The ht.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static int SetValue(ref Hashtable ht, string key, object value)
        {
            if (ht == null)
            {
                ht = new Hashtable();
                ht.Add(key.ToLower(), value);
                return 0;
            }
            if (ht.ContainsKey(key.ToLower()))
            {
                ht[key.ToLower()] = value;
                return 1;
            }
            else
            {
                ht.Add(key.ToLower(), value);
                return 0;
            }
        }

        /// <summary>
        /// 获取 Hashtable 指定 键的值
        /// </summary>
        /// <param name="ht">The ht.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static object GetValue(Hashtable ht, string key)
        {
            return ht[key.ToLower()];
        }

        /// <summary>
        /// Hashtable 转换成 ArrayList
        /// </summary>
        /// <param name="ht">Hashtable</param>
        /// <returns></returns>
        public static IList HashtableToList(Hashtable ht)
        {
            ArrayList list = new ArrayList();
            foreach (DictionaryEntry de in ht)
            {
                ListItem listitem = new ListItem(de.Value.ToString(), de.Key.ToString());
                list.Add(listitem);
            }
            return list;
        }

        /// <summary>
        /// Hashtable  转换成 Dictionary<string, string>
        /// </summary>
        /// <param name="ht">Hashtable</param>
        /// <returns></returns>
        public static Dictionary<string, string> HashtableToDictionary(Hashtable ht)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (DictionaryEntry de in ht)
            {
                list.Add(de.Key.ToString(), de.Value.ToString());
            }
            return list;
        }

        /// <summary>
        /// 将Hashtable绑定到ListControl
        /// </summary>
        /// <param name="listControl">ListControl</param>
        /// <param name="ht">The ht.</param>
        public static void FillListControl(ListControl listControl, Hashtable ht)
        {
            listControl.Items.Clear();
            listControl.DataSource = HashtableToList(ht);
            listControl.DataValueField = "value";
            listControl.DataTextField = "text";
            listControl.DataBind();

        }
    }
}
