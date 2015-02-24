using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web;
using System.Security.Permissions;
using System.IO;

namespace DP.Web.UI.Controls
{
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class DesignTimeHtmlTextWriter : HtmlTextWriter
    {
        public DesignTimeHtmlTextWriter(TextWriter writer)
            : base(writer)
        {

        }
        public DesignTimeHtmlTextWriter(TextWriter writer, string tabString)
            : base(writer, tabString)
        {

        }
        public override void AddAttribute(HtmlTextWriterAttribute key, string value)
        {
            if (((key == HtmlTextWriterAttribute.Src) || (key == HtmlTextWriterAttribute.Href)) || (key == HtmlTextWriterAttribute.Background))
            {
                base.AddAttribute(key.ToString(), value, key);
            }
            else
            {
                base.AddAttribute(key, value);
            }

        }

    }
}
