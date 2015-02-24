using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.Design;
using System.ComponentModel;
using System.Web.UI;

namespace DP.Web.UI.Controls
{
    public class TextBoxDesigner : ControlDesigner
    {
        // Methods
        public TextBoxDesigner()
        {
        }


        public override string GetDesignTimeHtml()
        {
            ControlCollection controls = ((DP.Web.UI.Controls.TextBox)base.Component).Controls;
            return base.GetDesignTimeHtml();
        }





        protected override string GetErrorDesignTimeHtml(Exception e)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("发生错误：" + e.Message + "\r\n请");
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
