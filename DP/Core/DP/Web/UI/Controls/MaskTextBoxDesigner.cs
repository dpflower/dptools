using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;

using System.Text;
using System.Web.UI.Design;
using System.Security.Permissions;
using System.Web.UI.Design.WebControls;
using System.Web.UI.WebControls;
using System.IO;
using System.Globalization;


namespace DP.Web.UI.Controls
{
    [SupportsPreviewControl(true), SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class MaskTextBoxDesigner : ControlDesigner
    {
        public MaskTextBoxDesigner()
        {
            
        }

        protected override string GetErrorDesignTimeHtml(Exception e)
        {
            return e.ToString();
        }

        public override string GetDesignTimeHtml()
        {
            //string strRel = string.Empty;
            ////MaskTextBox txt = (MaskTextBox)ViewControl;
            ////switch (txt.TextMode)
            ////{
            ////    case TextBoxMode.MultiLine:
            ////        {
            ////            strRel = "<textarea name=\"MaskTextBox1\" rows=\"2\" cols=\"20\" id=\"MaskTextBox1\"></textarea>";
            ////        }
            ////        break;
            ////    case TextBoxMode.Password:
            ////        {
            ////            strRel = "<input name=\"MaskTextBox1\" type=\"password\" id=\"MaskTextBox1\" />";
            ////        }
            ////        break;
            ////    case TextBoxMode.SingleLine:
            ////        {
            ////            strRel = "<input name=\"MaskTextBox1\" type=\"text\" id=\"MaskTextBox1\" />";
            ////        }
            ////        break;
            ////}
            //strRel = "<input name=\"MaskTextBox1\" type=\"text\" id=\"MaskTextBox1\" />";
            //if (string.IsNullOrEmpty(strRel))
            //{
            //    return base.GetEmptyDesignTimeHtml();
            //}
            //return strRel;


            #region MyRegion

            StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            DesignTimeHtmlTextWriter writer2 = new DesignTimeHtmlTextWriter(writer);
            string errorDesignTimeHtml = null;
            bool flag = false;
            bool visible = true;
            Control viewControl = null;
            try
            {
                viewControl = base.ViewControl;
                visible = viewControl.Visible;
                if (!visible)
                {
                    viewControl.Visible = true;
                    flag = !this.UsePreviewControl;
                }
                viewControl.RenderControl(writer2);
                errorDesignTimeHtml = writer.ToString();
            }
            catch (Exception exception)
            {
                errorDesignTimeHtml = this.GetErrorDesignTimeHtml(exception);
            }
            finally
            {
                if (flag)
                {
                    viewControl.Visible = visible;
                }
            }
            if ((errorDesignTimeHtml != null) && (errorDesignTimeHtml.Length != 0))
            {
                return errorDesignTimeHtml;
            }
            return this.GetEmptyDesignTimeHtml(); 
            #endregion
            
        }


        public override void UpdateDesignTimeHtml()
        {          
            base.UpdateDesignTimeHtml();

        }


        protected override bool UsePreviewControl
        {
            get
            {
                return base.UsePreviewControl;
            }
        }


        public override void Initialize(IComponent component)
        {
            if (!(component is Control) && !(component is INamingContainer))
            {
                throw new ArgumentException("Component must be a container control.", "component");
            }
            base.Initialize(component);
        }



        protected override void PreFilterProperties(System.Collections.IDictionary properties)
        {
            base.PreFilterProperties(properties);
        }

        protected override void PreFilterAttributes(System.Collections.IDictionary attributes)
        {
            base.PreFilterAttributes(attributes);
        }

        protected override void PostFilterProperties(System.Collections.IDictionary properties)
        {
            base.PostFilterProperties(properties);
        }

        //protected override bool UsePreviewControl
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}


    }
}
