using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.Design;
using System.Web.UI;
using System.ComponentModel;

namespace DP.Web.UI.Controls
{
    public class ValidateTextBoxDesigner : ControlDesigner
    {
        // Methods
        public ValidateTextBoxDesigner()
        {
        }


        public override string GetDesignTimeHtml()
        {
            ControlCollection controls = ((ValidateTextBox)base.Component).Controls;
            return base.GetDesignTimeHtml();
        }


        protected override string GetErrorDesignTimeHtml(Exception e)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("发生错误：" + e.Message + "\r\n");
            return base.CreatePlaceHolderDesignTimeHtml(builder.ToString());
        }


        public override void Initialize(IComponent component)
        {
            if (!(component is Control) && !(component is INamingContainer))
            {
                throw new ArgumentException("Component must be a container control.", "component");
            }
            base.Initialize(component);
        }

 

 

    }

 

}
