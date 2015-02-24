using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace DP.Web.UI.Controls
{
    public class PagerCssConverter : StringConverter
    {/// <summary>
        /// 构造函数
        /// </summary>
        public PagerCssConverter() { }

        /// <summary>
        /// 分页样式选择器编辑属性
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// 获取标准值列表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            ArrayList CssArray = new ArrayList();
            CssArray.Add("Pager");
            CssArray.Add("GreenStyle");
            return new StandardValuesCollection(CssArray);

        }

        /// <summary>
        /// return ture的话只能选,return flase可选可填
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }
}
