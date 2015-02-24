using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Globalization;
using System.Security.Permissions;
using System.Design;

namespace DP.Web.UI.Controls
{
    /// <summary>
    /// 
    /// </summary>       
    public enum MaskTextBoxType
    {
        None = 0,
        PositiveInteger = 1,
        Integer = 2,
        Decimal = 3,
        OnlyEnglish = 4,
        OnlyEnglishAndDigital = 5,
        OnlyChina = 6,
        FullWidthCharacters = 7,


        Regex = 9
    }

    [ToolboxData("<{0}:MaskTextBox runat=server></{0}:MaskTextBox>")]
    [DefaultProperty("Text")]
    [Designer(typeof(MaskTextBoxDesigner))]
    [AspNetHostingPermission(SecurityAction.Demand,Level = AspNetHostingPermissionLevel.Minimal),AspNetHostingPermission(SecurityAction.InheritanceDemand,Level=AspNetHostingPermissionLevel.Minimal)]
    public class MaskTextBox : TextBox
    {
        #region Behavior 属性
        [Bindable(true), Category("Behavior"), DefaultValue(MaskTextBoxType.None), Description(""), ReadOnly(false)]
        public virtual MaskTextBoxType ClientMaskType
        {
            get
            {
                object obj = ViewState["ClientMaskType"];
                return (obj == null) ? MaskTextBoxType.None : (MaskTextBoxType)obj;

            }
            set
            {

                ViewState["ClientMaskType"] = value;

            }
        }

        [Browsable(true), Category("Behavior"), DefaultValue(""), ReadOnly(false)]
        public virtual string RegexFormat
        {
            get
            {
                object obj = ViewState["RegexFormat"];
                return (obj == null) ? "" : (string)obj;
            }
            set
            {
                ViewState["RegexFormat"] = value;
            }
        } 
        #endregion

        #region Base Override
        protected override void OnPreRender(EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), "DP.Resources.JavaScript.MaskTextBox.js");

            base.OnPreRender(e);

            this.SetMask();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            this.RenderBeginTag(writer);
            if (this.TextMode == TextBoxMode.MultiLine)
            {
                HttpUtility.HtmlEncode(this.Text, writer);
            }
            this.RenderEndTag(writer);

        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void EnsureChildControls()
        {
            base.EnsureChildControls();
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
        }

        protected override void LoadControlState(object savedState)
        {
            base.LoadControlState(savedState);
        }

        protected override void TrackViewState()
        {
            base.TrackViewState();
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);
        }

        protected override void SetDesignModeState(IDictionary data)
        {
            base.SetDesignModeState(data);
        } 
        #endregion

        /// <summary>
        /// 设置 掩码
        /// </summary>
        private void SetMask()
        {
            string strMask = String.Empty;
            this.Attributes.Add("o_value", this.Text);
            this.Attributes.Add("t_value", this.Text);
            switch (ClientMaskType)
            {
                case MaskTextBoxType.None:
                    {

                    }
                    break;
                case MaskTextBoxType.PositiveInteger:
                    {
                        this.Attributes.Add("onKeyUp", "isPositiveInteger(this);");
                        this.Attributes.Add("onbeforepaste", "isClipboardPositiveInteger();");
                    }
                    break;
                case MaskTextBoxType.Integer:
                    {
                        strMask = "^[+-]?\\d*?\\d*?$";
                        this.Attributes.Add("onKeyUp", "isInteger(this);");
                        this.Attributes.Add("onbeforepaste", "isMatchClipboard('" + strMask.Replace("\\", "\\\\") + "');");
                    }
                    break;
                case MaskTextBoxType.Decimal:
                    {
                        strMask = "^[+-]?\\d*?\\.?\\d*?$";
                        this.Attributes.Add("onKeyUp", "isDecimal(this);");
                        this.Attributes.Add("onbeforepaste", "isMatchClipboard('" + strMask.Replace("\\", "\\\\") + "');");
                    }
                    break;
                case MaskTextBoxType.OnlyEnglish:
                    {
                        strMask = "^[a-zA-Z]*$";
                        this.Attributes.Add("onKeyUp", "isEnglist(this);");
                        this.Attributes.Add("onbeforepaste", "isMatchClipboard('" + strMask.Replace("\\", "\\\\") + "');");
                    }
                    break;
                case MaskTextBoxType.OnlyEnglishAndDigital:
                    {
                        strMask = "^[\\w]*$";
                        this.Attributes.Add("onKeyUp", "isLetters(this);");
                        this.Attributes.Add("onbeforepaste", "isMatchClipboard('" + strMask.Replace("\\", "\\\\") + "');");
                    }
                    break;
                case MaskTextBoxType.OnlyChina:
                    {
                        strMask = "^[\\u4E00-\\u9FA5]*$";         
                        this.Attributes.Add("onKeyUp", "isChinese(this);");
                        this.Attributes.Add("onbeforepaste", "isMatchClipboard('" + strMask.Replace("\\", "\\\\") + "');");
                    }
                    break;
                case MaskTextBoxType.FullWidthCharacters:
                    {
                        strMask = "^[\\uFF00-\\uFFFF]*$";
                        this.Attributes.Add("onKeyUp", "isFullWidthCharacters(this);");
                        this.Attributes.Add("onbeforepaste", "isMatchClipboard('" + strMask.Replace("\\", "\\\\") + "');");
                    }
                    break;
                case MaskTextBoxType.Regex:
                    {
                        if (!String.IsNullOrEmpty(RegexFormat))
                        {
                            strMask = RegexFormat.Trim();
                            this.Attributes.Add("onKeyUp", "isMatch(this, '" + strMask.Replace("\\", "\\\\") + "');");
                            this.Attributes.Add("onbeforepaste", "isMatchClipboard('" + strMask.Replace("\\", "\\\\") + "');");
                        }
                    }
                    break;
            }
        }

        
    }
}
