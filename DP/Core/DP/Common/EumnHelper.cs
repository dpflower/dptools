using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Web.UI.WebControls;

namespace DP.Common
{
    public static class EnumHelper
    {
        /// <summary>
        /// 将枚举转换成ArrayList
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static IList EnumToList(Type enumType)
        {
            ArrayList list = new ArrayList();
            foreach (int i in Enum.GetValues(enumType))
            {
                ListItem listitem = new ListItem(Enum.GetName(enumType, i), i.ToString());
                list.Add(listitem);
            }
            return list;
        }

        /// <summary>
        /// 将枚举转换成 Dictionary<int, string>
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static Dictionary<int, string> EnumToDictionary(Type enumType)
        {
            Dictionary<int, string> list = new Dictionary<int, string>();
            foreach (int i in Enum.GetValues(enumType))
            {
                string name = Enum.GetName(enumType, i);
                list.Add(i, name);
            }
            return list;
        }

        /// <summary>
        /// 将枚举绑定到ListControl
        /// </summary>
        /// <param name="listControl">ListControl</param>
        /// <param name="enumType">枚举类型</param>
        public static void FillListControl(ListControl listControl, Type enumType)
        {
            listControl.Items.Clear();
            listControl.DataSource = EnumToList(enumType);
            listControl.DataValueField = "value";
            listControl.DataTextField = "text";
            listControl.DataBind();

        }
        


    }
}
