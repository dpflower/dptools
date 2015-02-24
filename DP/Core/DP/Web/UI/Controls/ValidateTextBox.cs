using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Drawing;
using System.ComponentModel;
using System.Web;

namespace DP.Web.UI.Controls
{
    public enum LengthMode
    {
        BigCharEqualOne,
        BigCharEqualTwo
    }

    public enum ValidType
    {
        Int,
        PositiveInt,
        NegativeInt,
        Float,
        PositiveFloat,
        NegativeFloat,
        Email,
        URL,
        Color,
        Chinese,
        ASCII,
        ZipCode,
        Tel,
        Mobile,
        ImageFile,
        ZipFile,
        Date,
        Capitalize,
        Uppercase,
        Lowercase,
        Time,
        IPAdress
    }


    [ToolboxData("<{0}:ValidateTextBox runat=server></{0}:ValidateTextBox>"), ToolboxBitmap(typeof(ValidateTextBox), "ValidteTextBox.bmp"), ValidationProperty("Text"), DefaultEvent("TextChanged"), DefaultProperty("ValidteMode"), Designer(typeof(ValidateTextBoxDesigner))]
    public class ValidateTextBox : WebControl, INamingContainer
    {
        // Fields
        private static readonly object EventTextChanged;
        private Label label;
        private Style labelStyle;
        private TableStyle outerStyle;
        private bool renderClientScript;
        private const string scriptEnd = "\r\n// -->\r\n</script>";
        private const string scriptStart = "<script language = 'javascript'>\r\n<!--\r\n";
        private TextBox textbox;
        private Style textboxStyle;

        // Events
        [Description("Raised when the user change the textbox 's text."), Category("Action")]
        public event EventHandler TextChanged
        {
            add
            {
                this.EnsureChildControls();
                this.textbox.TextChanged += value;
            }
            remove
            {
                this.EnsureChildControls();
                this.textbox.TextChanged -= value;
            }
        }
 

 


        // Methods
        static ValidateTextBox()
        {
            EventTextChanged = new object();
        }

        public ValidateTextBox()
        {
        }

        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            this.textbox = new TextBox();
            this.textbox.ID = "textbox";
            this.label = new Label();
            this.label.ID = "label";
            this.Controls.Add(this.textbox);
            this.Controls.Add(this.label);
        }

        private void DetermineRenderClintScript()
        {
            this.renderClientScript = false;
            if (((this.Page != null) && (this.Page.Request != null)) && this.EnableClientScript)
            {
                HttpBrowserCapabilities browser = this.Page.Request.Browser;
                bool flag = browser.EcmaScriptVersion.CompareTo(new Version(1, 2)) >= 0;
                bool flag2 = browser.MSDomVersion.Major >= 4;
                this.renderClientScript = flag && flag2;
            }
        }

        protected override void LoadViewState(object savedState)
        {
            if (savedState == null)
            {
                base.LoadViewState(null);
            }
            else
            {
                object[] objArray = (object[])savedState;
                if (objArray.Length != 3)
                {
                    throw new ArgumentException("Invalid view state.");
                }
                if (objArray[0] != null)
                {
                    base.LoadViewState(objArray[0]);
                }
                if (objArray[1] != null)
                {
                    ((IStateManager)this.textboxStyle).LoadViewState(objArray[1]);
                }
                if (objArray[2] != null)
                {
                    ((IStateManager)this.labelStyle).LoadViewState(objArray[2]);
                }
            }
        }

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            bool flag = false;
            EventHandler handler = (EventHandler)base.Events[EventTextChanged];
            if (handler != null)
            {
                handler(this, args);
                flag = true;
            }
            return flag;
        }

        protected override void OnPreRender(EventArgs e)
        {
            this.DetermineRenderClintScript();
            if (this.renderClientScript)
            {
                if (!this.Page.IsStartupScriptRegistered("OverrideMS"))
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append("<script language = 'javascript'>\r\n<!--\r\n");
                    builder.Append("\r\n\r\nfunction GetForm(obj)\r\n{\r\n\tif(obj.tagName == 'FORM')\r\n\t\treturn obj;\r\n\telse\r\n\t\tGetForm(obj.parentElement);\r\n\treturn document.forms[0];\r\n}\r\nValidatorCommonOnSubmit = function() {\r\n\tif(Page_BlockSubmit)\r\n\t{\r\n\t\tvar temp = !Page_BlockSubmit\r\n\t\tevent.returnValue = !Page_BlockSubmit;\r\n\t\tPage_BlockSubmit = false;\r\n\t\treturn temp;\r\n\t}\r\n}\r\n __doPostBack = function(eventTarget, eventArgument) {\r\n\t\t\r\n\t\tvar theform = GetForm(event.srcElement);\r\n\t\ttheform.__EVENTTARGET.value = eventTarget;\r\n\t\ttheform.__EVENTARGUMENT.value = eventArgument;\r\n\t\tvar validated = true;\r\n\t\tif(typeof(validateLength)=='function')\r\n\t\t\tvalidated = validated && validateLength();\r\n\t\tif(typeof(ValidateRegular) == 'function')\r\n\t\t\tvalidated = validated && ValidateRegular();\r\n\t\tif (typeof(Page_ClientValidate) == 'function') \r\n\t\t\tvalidated = validated && Page_ClientValidate();\r\n\t\tevent.returnValue = true;\r\n\t\tif(validated)\r\n\t\t\ttheform.submit();\r\n\t}\r\n");
                    builder.Append("\r\n// -->\r\n</script>");
                    this.Page.RegisterStartupScript("OverrideMS", builder.ToString());
                }
                StringBuilder builder2 = new StringBuilder();
                if (this.IsLimitLength)
                {
                    builder2.Append("<script language = 'javascript'>\r\n<!--\r\n");
                    builder2.Append("function IsBigChar(str)\r\n\t\t\t{\r\n\t\t\t\tvar reg = str.match(/[\\x00-\\xff]/);\r\n\t\t\t\tif(reg == null)\r\n\t\t\t\t{\r\n\t\t\t\t\treturn true;\r\n\t\t\t\t}\r\n\t\t\t\treturn false;\r\n\t\t\t}\r\n\t\t\tfunction CountLength(str,equaltwo)\r\n\t\t\t{\r\n\t\t\t\tvar leng = 0;\r\n\t\t\t\tif(equaltwo)\r\n\t\t\t\t{\r\n\t\t\t\t\tfor(var i=0;i<str.length;i++)\r\n\t\t\t\t\t{\r\n\t\t\t\t\t\tvar substr = str.substring(i,i+1);\r\n\t\t\t\t\t\tif(IsBigChar(substr))\r\n\t\t\t\t\t\t\tleng +=2;\r\n\t\t\t\t\t\telse\r\n\t\t\t\t\t\t\tleng++;\r\n\t\t\t\t\t}\r\n\t\t\t\t}\r\n\t\t\t\telse\r\n\t\t\t\t{\r\n\t\t\t\t\tleng = str.length;\r\n\t\t\t\t}\t\t\r\n\t\t\t\treturn leng;\r\n\t\t\t}\r\n\t\t\tfunction validateLength()\r\n\t\t\t{\r\n\t\t\t\tvar result = true;\r\n\t\t\t\tfor(var i=0;i<boxlist.length;i++)\r\n\t\t\t\t{\r\n\t\t\t\t\tvar textbox = document.getElementById(boxlist[i]);\r\n\t\t\t\t\tvar message = document.getElementById(messagelabel[i]);\r\n\t\t\t\t\tvar min = limitMin[i]*1;\r\n\t\t\t\t\tvar max = limitMax[i]*1;\r\n\t\t\t\t\tvar equal = (wideEqual[i]*1 != 0)?true:false;\r\n\t\t\t\t\tvar length = CountLength(textbox.value,equal);\r\n\t\t\t\t\tif(max == 0)\r\n\t\t\t\t\t\tmax = length + 1;\r\n\t\t\t\t\tif(length < min || length > max)\r\n\t\t\t\t\t{\r\n\t\t\t\t\t\tresult = false;\r\n\t\t\t\t\t\tmessage.style.display = 'block';\r\n\t\t\t\t\t}\r\n\t\t\t\t\telse\r\n\t\t\t\t\t{\r\n\t\t\t\t\t\tmessage.style.display = 'none';\r\n\t\t\t\t\t}\r\n\t\t\t\t}\r\n\t\t\t\tif(!result)\r\n\t\t\t\t{\r\n\t\t\t\t\tevent.returnValue = false;\r\n\t\t\t\t\treturn false;\r\n\t\t\t\t}\r\n\t\t\t\treturn true;\r\n\t\t\t}\r\n");
                    builder2.Append("\r\n// -->\r\n</script>");
                    if (!this.Page.IsClientScriptBlockRegistered("LimitLength"))
                    {
                        this.Page.RegisterClientScriptBlock("LimitLength", builder2.ToString());
                    }
                    this.Page.RegisterArrayDeclaration("boxlist", "\"" + this.textbox.ClientID + "\"");
                    this.Page.RegisterArrayDeclaration("limitMin", "\"" + this.MinLength.ToString() + "\"");
                    this.Page.RegisterArrayDeclaration("limitMax", "\"" + this.MaxLength.ToString() + "\"");
                    this.Page.RegisterArrayDeclaration("messagelabel", "\"" + this.label.ClientID + "\"");
                    this.Page.RegisterArrayDeclaration("wideEqual", "\"" + ((int)this.CountLengthMode).ToString() + "\"");
                    this.Page.RegisterOnSubmitStatement("validateLength", "validateLength();");
                }
                StringBuilder builder3 = new StringBuilder();
                builder3.Append("<script language = 'javascript'>\r\n<!--\r\n");
                builder3.Append("\r\nfunction ValidateRegular()\r\n{\r\n\tvar validated = true;\r\n\tfor(var i = 0; i < validateList.length ; i++)\r\n\t{\r\n\t\tvar box = document.getElementById(validateList[i]);\r\n\t\tvar reg = regularList[i];\r\n\t\tvar msg = document.getElementById(regularError[i]);\r\n\t\tif(!(reg.test(box.value)))\r\n\t\t{\r\n\t\t\tvalidated = false;\r\n\t\t\tmsg.style.display = 'block';\r\n\t\t}\r\n\t}\r\n\tif(!validated)\r\n\t{\r\n\t\tevent.returnValue = false;\r\n\t\treturn false;\r\n\t}\r\n\treturn true;\r\n}\r\n");
                builder3.Append("\r\n// -->\r\n</script>");
                new StringBuilder();
                switch (this.TextValidType)
                {
                    case ValidType.PositiveInt:
                        this.Page.RegisterArrayDeclaration("regularList", @"/^([+]?)\d+$/");
                        break;

                    case ValidType.NegativeInt:
                        this.Page.RegisterArrayDeclaration("regularList", @"/^-\d+$/");
                        break;

                    case ValidType.Float:
                        this.Page.RegisterArrayDeclaration("regularList", @"/^([+-]?)\d*\.\d+$/");
                        break;

                    case ValidType.PositiveFloat:
                        this.Page.RegisterArrayDeclaration("regularList", @"/^([+]?)\d*\.\d+$/");
                        break;

                    case ValidType.NegativeFloat:
                        this.Page.RegisterArrayDeclaration("regularList", @"/^-\d*\.\d+$/");
                        break;

                    case ValidType.Email:
                        this.Page.RegisterArrayDeclaration("regularList", @"/^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/");
                        break;

                    case ValidType.URL:
                        this.Page.RegisterArrayDeclaration("regularList", @"/^http:\/\/.*/");
                        break;

                    case ValidType.Color:
                        this.Page.RegisterArrayDeclaration("regularList", "/^#[a-fA-F0-9]{6}$/");
                        break;

                    case ValidType.Chinese:
                    case ValidType.ASCII:
                    case ValidType.Capitalize:
                    case ValidType.Uppercase:
                    case ValidType.Lowercase:
                        return;

                    case ValidType.ZipCode:
                        this.Page.RegisterArrayDeclaration("regularList", @"/^\d{6}$/");
                        break;

                    case ValidType.Tel:
                        this.Page.RegisterArrayDeclaration("regularList", @"/^((\((\+|00)?\d{1,3}\))|(\+|00)?\d{1,3})?0?\d{3}-?\d{7,8}$/");
                        break;

                    case ValidType.Mobile:
                        this.Page.RegisterArrayDeclaration("regularList", "/^0{0,1}13[0-9]{9}$/");
                        break;

                    case ValidType.ImageFile:
                        this.Page.RegisterArrayDeclaration("regularList", @"/(.*)\.(jpg|bmp|gif|ico|pcx|jpeg|tif|png|raw|tga)$/");
                        break;

                    case ValidType.ZipFile:
                        this.Page.RegisterArrayDeclaration("regularList", @"/(.*)\.(rar|zip|7zip|tgz)$/");
                        break;

                    case ValidType.Date:
                        this.Page.RegisterArrayDeclaration("regularList", @"/^\d{4}(\-|\/|\.)[0-1]?[0-9]\1[0-3]?[0-9]$/");
                        break;

                    case ValidType.Time:
                        this.Page.RegisterArrayDeclaration("regularList", @"/^[0-2]?[0-9](\-|:|\.)[0-5]?[0-9]\1[0-5][0-9]$/");
                        break;

                    case ValidType.IPAdress:
                        this.Page.RegisterArrayDeclaration("regularList", @"/^(([0-1]?\d{0,2})|(2[0-5]{0,2}))\.(([0-1]?\d{0,2})|(2[0-5]{0,2}))\.(([0-1]?\d{0,2})|(2[0-5]{0,2}))\.(([0-1]?\d{0,2})|(2[0-5]{0,2}))$/");
                        break;

                    default:
                        return;
                }
                if (!this.Page.IsClientScriptBlockRegistered("ValidateRegular"))
                {
                    this.Page.RegisterClientScriptBlock("ValidateRegular", builder3.ToString());
                }
                this.Page.RegisterArrayDeclaration("validateList", "\"" + this.textbox.ClientID + "\"");
                this.Page.RegisterArrayDeclaration("regularError", "\"" + this.label.ClientID + "\"");
                this.Page.RegisterOnSubmitStatement("Regular", "ValidateRegular();");
            }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine("<!-- THINControls.WebControls.THINTextBox.ValidateTextBox   begin -->");
            if (this.renderClientScript)
            {
                this.AddAttributesToRender(writer);
                if (this.outerStyle != null)
                {
                    this.outerStyle.AddAttributesToRender(writer);
                }
                writer.RenderBeginTag(HtmlTextWriterTag.Table);
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                if (this.textboxStyle != null)
                {
                    this.textbox.ApplyStyle(this.textboxStyle);
                }
                if (this.TextValidType == ValidType.ASCII)
                {
                    this.textbox.Style.Add("ime-mode", "disabled");
                    this.textbox.Attributes.Add("onpropertychange", @"with(this)if(/[^\x00-\xff]/ig.test(value))value=value.replace(/[^\x00-\xff]/ig,'')");
                }
                if (this.TextValidType == ValidType.Chinese)
                {
                    this.textbox.Attributes.Add("onpropertychange", @"with(this)if(/[^\u3447-\uFA29]/ig.test(value))value=value.replace(/[^\u3447-\uFA29]/ig,'')");
                }
                if (this.TextValidType == ValidType.Capitalize)
                {
                    this.textbox.Style.Add("text-transform", "capitalize");
                }
                if (this.TextValidType == ValidType.Color)
                {
                    this.textbox.Style.Add("ime-mode", "disabled");
                    this.textbox.Attributes.Add("onpropertychange", "with(this)if(/[^#0-9a-fA-F]/ig.test(value))value=value.replace(/[^#0-9a-fA-F]/ig,'')");
                }
                if (this.TextValidType == ValidType.Date)
                {
                    this.textbox.Style.Add("ime-mode", "disabled");
                    this.textbox.Attributes.Add("onpropertychange", @"with(this)if(/[^\.\-\/0-9]/ig.test(value))value=value.replace(/[^\.\-\/0-9]/ig,'')");
                }
                if (this.TextValidType == ValidType.Email)
                {
                    this.textbox.Style.Add("ime-mode", "disabled");
                    this.textbox.Attributes.Add("onpropertychange", @"with(this)if(/[^a-zA-Z0-9@_\-\.]/ig.test(value))value=value.replace(/[^a-zA-Z0-9@_\-\.]/ig,'')");
                }
                if (this.TextValidType == ValidType.Float)
                {
                    this.textbox.Style.Add("ime-mode", "disabled");
                    this.textbox.Attributes.Add("onpropertychange", @"with(this)if(/[^0-9\.\+\-]/ig.test(value))value=value.replace(/[^0-9\.\+\-]/ig,'')");
                }
                if (this.TextValidType == ValidType.Int)
                {
                    this.textbox.Style.Add("ime-mode", "disabled");
                    this.textbox.Attributes.Add("onpropertychange", @"with(this)if(/[^0-9\+\-]/ig.test(value))value=value.replace(/[^0-9\+\-]/ig,'')");
                }
                if (this.TextValidType == ValidType.IPAdress)
                {
                    this.textbox.Style.Add("ime-mode", "disabled");
                    this.textbox.Attributes.Add("onpropertychange", @"with(this)if(/[^0-9\.]/ig.test(value))value=value.replace(/[^0-9\.]/ig,'')");
                }
                if (this.TextValidType == ValidType.Lowercase)
                {
                    this.textbox.Style.Add("text-transform", "lowercase");
                }
                if (this.TextValidType == ValidType.Mobile)
                {
                    this.textbox.Style.Add("ime-mode", "disabled");
                    this.textbox.Attributes.Add("onpropertychange", @"with(this)if(/[^0-9\(\)\+\-]/ig.test(value))value=value.replace(/[^0-9\(\)\+\-]/ig,'')");
                }
                if (this.TextValidType == ValidType.NegativeFloat)
                {
                    this.textbox.Style.Add("ime-mode", "disabled");
                    this.textbox.Attributes.Add("onpropertychange", @"with(this)if(/[^0-9\.\-]/ig.test(value))value=value.replace(/[^0-9\.\-]/ig,'')");
                }
                if (this.TextValidType == ValidType.NegativeInt)
                {
                    this.textbox.Style.Add("ime-mode", "disabled");
                    this.textbox.Attributes.Add("onpropertychange", @"with(this)if(/[^0-9\-]/ig.test(value))value=value.replace(/[^0-9\-]/ig,'')");
                }
                if (this.TextValidType == ValidType.PositiveFloat)
                {
                    this.textbox.Style.Add("ime-mode", "disabled");
                    this.textbox.Attributes.Add("onpropertychange", @"with(this)if(/[^0-9\.\+]/ig.test(value))value=value.replace(/[^0-9\.\+]/ig,'')");
                }
                if (this.TextValidType == ValidType.PositiveInt)
                {
                    this.textbox.Style.Add("ime-mode", "disabled");
                    this.textbox.Attributes.Add("onpropertychange", @"with(this)if(/[^0-9\+]/ig.test(value))value=value.replace(/[^0-9\+]/ig,'')");
                }
                if (this.TextValidType == ValidType.Tel)
                {
                    this.textbox.Style.Add("ime-mode", "disabled");
                    this.textbox.Attributes.Add("onpropertychange", @"with(this)if(/[^0-9\(\)\+\-]/ig.test(value))value=value.replace(/[^0-9\(\)\+\-]/ig,'')");
                }
                if (this.TextValidType == ValidType.Time)
                {
                    this.textbox.Style.Add("ime-mode", "disabled");
                    this.textbox.Attributes.Add("onpropertychange", @"with(this)if(/[^0-9\.:\-]/ig.test(value))value=value.replace(/[^0-9\.:\-]/ig,'')");
                }
                if (this.TextValidType == ValidType.Uppercase)
                {
                    this.textbox.Style.Add("text-transform", "uppercase");
                }
                if (this.TextValidType == ValidType.ZipCode)
                {
                    this.textbox.Style.Add("ime-mode", "disabled");
                    this.textbox.Attributes.Add("onpropertychange", "with(this)if(/[^0-9]/ig.test(value))value=value.replace(/[^0-9]/ig,'')");
                }
                this.textbox.Attributes.Add("ondrop", "return false;");
                this.textbox.RenderControl(writer);
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                this.label.ApplyStyle(this.labelStyle);
                this.label.Style.Add("display", "none");
                this.label.RenderControl(writer);
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.WriteLine();
                writer.WriteLine("<!-- thin37421@126.com end -->");
            }
            else
            {
                this.AddAttributesToRender(writer);
                if (this.outerStyle != null)
                {
                    this.outerStyle.AddAttributesToRender(writer);
                }
                writer.RenderBeginTag(HtmlTextWriterTag.Table);
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                if (this.textboxStyle != null)
                {
                    this.textbox.ApplyStyle(this.textboxStyle);
                }
                this.textbox.RenderControl(writer);
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                this.label.ApplyStyle(this.labelStyle);
                this.label.RenderControl(writer);
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.WriteLine();
            }
        }

        protected override object SaveViewState()
        {
            object[] objArray = new object[] { base.SaveViewState(), (this.textboxStyle == null) ? null : ((IStateManager)this.textboxStyle).SaveViewState(), (this.labelStyle == null) ? null : ((IStateManager)this.labelStyle).SaveViewState() };
            for (int i = 0; i < objArray.Length; i++)
            {
                if (objArray[i] != null)
                {
                    return objArray;
                }
            }
            return null;
        }

        protected override void TrackViewState()
        {
            base.TrackViewState();
            if (this.textboxStyle != null)
            {
                ((IStateManager)this.textboxStyle).TrackViewState();
            }
            if (this.labelStyle != null)
            {
                ((IStateManager)this.labelStyle).TrackViewState();
            }
        }

 

 


        // Properties
        [DefaultValue(false), Category("Behavior"), Description("Whether auto post back of the TextBox.")]
        public bool AutoPostBack
        {
            get
            {
                this.EnsureChildControls();
                return this.textbox.AutoPostBack;
            }
            set
            {
                this.EnsureChildControls();
                this.textbox.AutoPostBack = value;
            }
        }


        public override ControlCollection Controls
        {
            get
            {
                this.EnsureChildControls();
                return base.Controls;
            }
        }


        [TypeConverter(typeof(EnumConverter)), Description("Wide character equal one or two when count text length."), DefaultValue(0), Category("Behavior")]
        public LengthMode CountLengthMode
        {
            get
            {
                object obj2 = this.ViewState["LengthMode"];
                if (obj2 != null)
                {
                    return (LengthMode)obj2;
                }
                return LengthMode.BigCharEqualOne;
            }
            set
            {
                if ((value < LengthMode.BigCharEqualOne) || (value > LengthMode.BigCharEqualTwo))
                {
                    throw new ArgumentOutOfRangeException("CountLengthMode");
                }
                this.ViewState["LengthMode"] = value;
            }
        }
 


        [Category("Behavior"), DefaultValue(true), Description("Whether to enable the client script based HTML editor.")]
        public bool EnableClientScript
        {
            get
            {
                object obj2 = this.ViewState["EnableClientScript"];
                if (obj2 != null)
                {
                    return (bool)obj2;
                }
                return true;
            }
            set
            {
                this.ViewState["EnableClientScript"] = value;
            }
        }

        [Category("Appearence"), DefaultValue(""), Description("Message should show when the text is invalid.")]
        public string ErrorMessage
        {
            get
            {
                this.EnsureChildControls();
                return this.label.Text;
            }
            set
            {
                this.EnsureChildControls();
                this.label.Text = value;
            }
        }
        [Category("Behavior"), TypeConverter(typeof(BooleanConverter)), Description("Whether limit the text length."), DefaultValue(false)]
        public bool IsLimitLength
        {
            get
            {
                object obj2 = this.ViewState["LimitLength"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                this.ViewState["LimitLength"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(100), TypeConverter(typeof(Int16Converter)), Description("Text 's length must less than this setting.")]
        public int MaxLength
        {
            get
            {
                object obj2 = this.ViewState["MaxLength"];
                if (obj2 != null)
                {
                    return (int)obj2;
                }
                return 100;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("MaxLength");
                }
                this.ViewState["MaxLength"] = value;
            }
        }


        [Category("Style"), Description("The style to be applied to error message."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.Attribute)]
        public Style MessageStyle
        {
            get
            {
                if (this.textboxStyle == null)
                {
                    this.labelStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager)this.labelStyle).TrackViewState();
                    }
                }
                return this.labelStyle;
            }
        }


        [DefaultValue(0), TypeConverter(typeof(Int16Converter)), Category("Behavior"), Description("Text 's length must mroe than this setting.")]
        public int MinLength
        {
            get
            {
                object obj2 = this.ViewState["MinLength"];
                if (obj2 != null)
                {
                    return (int)obj2;
                }
                return 0;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("MinLength");
                }
                this.ViewState["MinLength"] = value;
            }
        }



        [PersistenceMode(PersistenceMode.Attribute), Description("The style to be applied to Control outer."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), Category("Style")]
        public TableStyle OuterStyle
        {
            get
            {
                if (this.outerStyle == null)
                {
                    this.outerStyle = new TableStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager)this.outerStyle).TrackViewState();
                    }
                }
                return this.outerStyle;
            }
        }

        [DefaultValue(""), Category("Appearence"), Description("Text of the textbox.")]
        public string Text
        {
            get
            {
                this.EnsureChildControls();
                return this.textbox.Text;
            }
            set
            {
                this.EnsureChildControls();
                this.textbox.Text = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Description("The style to be applied to Textbox."), Category("Style"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.Attribute)]
        public Style TextboxStyle
        {
            get
            {
                if (this.textboxStyle == null)
                {
                    this.textboxStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager)this.textboxStyle).TrackViewState();
                    }
                }
                return this.textboxStyle;
            }
        }

        [DefaultValue(0), Category("Behavior"), TypeConverter(typeof(EnumConverter)), Description("TextBox render mode.")]
        public TextBoxMode TextMode
        {
            get
            {
                this.EnsureChildControls();
                return this.textbox.TextMode;
            }
            set
            {
                this.EnsureChildControls();
                this.textbox.TextMode = value;
            }
        }


        [TypeConverter(typeof(EnumConverter)), DefaultValue(10), Description("Which valid type on textbox 's text."), Category("Behavior")]
        public ValidType TextValidType
        {
            get
            {
                object obj2 = this.ViewState["ValidType"];
                if (obj2 != null)
                {
                    return (ValidType)obj2;
                }
                return ValidType.ASCII;
            }
            set
            {
                if ((value < ValidType.Int) || (value > ValidType.IPAdress))
                {
                    throw new ArgumentOutOfRangeException("TextValidType");
                }
                this.ViewState["ValidType"] = value;
            }
        }
 

 

    }



}
