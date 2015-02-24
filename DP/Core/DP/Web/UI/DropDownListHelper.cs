using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace DP.Web.UI
{
    public static class DropDownListHelper
    {
        /// <summary>
        /// Items the selected by value.
        /// </summary>
        /// <param name="ddl">The DDL.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool ItemSelectedByValue(DropDownList ddl, string value)
        {
            if (ddl == null)
            {
                return false;
            }
            bool rel = true;
            ddl.SelectedIndex = -1;
            if (ddl.Items.FindByValue(value) != null)
            {
                ddl.Items.FindByValue(value).Selected = true;
            }
            else
            {
                rel = false;
            }
            return rel;
        }

        /// <summary>
        /// Items the selected by text.
        /// </summary>
        /// <param name="ddl">The DDL.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static bool ItemSelectedByText(DropDownList ddl, string text)
        {
            if (ddl == null)
            {
                return false;
            }
            bool rel = true;
            ddl.SelectedIndex = -1;
            if (ddl.Items.FindByText(text) != null)
            {
                ddl.Items.FindByText(text).Selected = true;
            }
            else
            {
                rel = false;
            }
            return rel;
        }




    }
}
