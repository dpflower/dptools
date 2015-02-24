using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel.Design;
using System.ComponentModel;

using System.Security.Permissions;
using System.Collections.Specialized;
using System.Web;
using System.Globalization;
using System.Drawing.Design;

namespace DP.Web.UI.Controls
{
    [ControlValueProperty("Text"), ControlBuilder(typeof(TextBoxControlBuilder)), SupportsEventValidation, DataBindingHandler("System.Web.UI.Design.TextDataBindingHandler, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultProperty("Text"), ValidationProperty("Text"), DefaultEvent("TextChanged"), Designer(typeof(DP.Web.UI.Controls.TextBoxDesigner)), ParseChildren(true, "Text"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class TextBox : WebControl, IPostBackDataHandler, IEditableTextControl, ITextControl
    {
        // Fields
        private const string _textBoxKeyHandlerCall = "if (WebForm_TextBoxKeyHandler(event) == false) return false;";
        private const int DefaultMutliLineColumns = 20;
        private const int DefaultMutliLineRows = 2;
        private static readonly object EventTextChanged;

        //// Events
        //[Description("TextBox_OnTextChanged"), Category("Action")]
        //public event EventHandler TextChanged;

        // Methods
        static TextBox()
        {
            EventTextChanged = new object();
        }

        public TextBox()
            : base(HtmlTextWriterTag.Input)
        {
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            Page page = this.Page;
            if (page != null)
            {
                page.VerifyRenderingInServerForm(this);
            }
            string uniqueID = this.UniqueID;
            if (uniqueID != null)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Name, uniqueID);
            }
            TextBoxMode textMode = this.TextMode;
            switch (textMode)
            {
                case TextBoxMode.MultiLine:
                    {
                        int rows = this.Rows;
                        int columns = this.Columns;
                        bool flag = false;
                        //if (!base.EnableLegacyRendering)
                        //{
                        if (rows == 0)
                        {
                            rows = 2;
                        }
                        if (columns == 0)
                        {
                            columns = 20;
                        }
                        //}
                        if ((rows > 0) || flag)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Rows, rows.ToString(NumberFormatInfo.InvariantInfo));
                        }
                        if ((columns > 0) || flag)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Cols, columns.ToString(NumberFormatInfo.InvariantInfo));
                        }
                        if (!this.Wrap)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Wrap, "off");
                        }
                        goto Label_024E;
                    }
                case TextBoxMode.SingleLine:
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
                        if (((this.AutoCompleteType != AutoCompleteType.None) && (this.Context != null)) && (this.Context.Request.Browser["supportsVCard"] == "true"))
                        {
                            if (this.AutoCompleteType == AutoCompleteType.Disabled)
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.AutoComplete, "off");
                            }
                            else if (this.AutoCompleteType == AutoCompleteType.Search)
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.VCardName, "search");
                            }
                            else if (this.AutoCompleteType == AutoCompleteType.HomeCountryRegion)
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.VCardName, "HomeCountry");
                            }
                            else if (this.AutoCompleteType == AutoCompleteType.BusinessCountryRegion)
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.VCardName, "BusinessCountry");
                            }
                            else
                            {
                                string str2 = Enum.Format(typeof(AutoCompleteType), this.AutoCompleteType, "G");
                                if (str2.StartsWith("Business", StringComparison.Ordinal))
                                {
                                    str2 = str2.Insert(8, ".");
                                }
                                else if (str2.StartsWith("Home", StringComparison.Ordinal))
                                {
                                    str2 = str2.Insert(4, ".");
                                }
                                writer.AddAttribute(HtmlTextWriterAttribute.VCardName, "vCard." + str2);
                            }
                        }
                        string text = this.Text;
                        if (text.Length > 0)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Value, text);
                        }
                        break;
                    }
                case TextBoxMode.Password:
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "password");
                    break;
            }
            int maxLength = this.MaxLength;
            if (maxLength > 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Maxlength, maxLength.ToString(NumberFormatInfo.InvariantInfo));
            }
            maxLength = this.Columns;
            if (maxLength > 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Size, maxLength.ToString(NumberFormatInfo.InvariantInfo));
            }
        Label_024E:
            if (this.ReadOnly)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.ReadOnly, "readonly");
            }
            if ((this.AutoPostBack && (page != null)) && ClientSupportsJavaScript(page))
            {
                string str4 = null;
                if (base.HasAttributes)
                {
                    str4 = base.Attributes["onchange"];
                    if (str4 != null)
                    {
                        str4 = EnsureEndWithSemiColon(str4);
                        base.Attributes.Remove("onchange");
                    }
                }
                PostBackOptions options = new PostBackOptions(this, string.Empty);
                if (this.CausesValidation)
                {
                    options.PerformValidation = true;
                    options.ValidationGroup = this.ValidationGroup;
                }
                if (page.Form != null)
                {
                    options.AutoPostBack = true;
                }
                str4 = MergeScript(str4, page.ClientScript.GetPostBackEventReference(options, true));
                writer.AddAttribute(HtmlTextWriterAttribute.Onchange, str4);
                if (textMode != TextBoxMode.MultiLine)
                {
                    string str5 = "if (WebForm_TextBoxKeyHandler(event) == false) return false;";
                    if (base.HasAttributes)
                    {
                        string str6 = base.Attributes["onkeypress"];
                        if (str6 != null)
                        {
                            str5 = str5 + str6;
                            base.Attributes.Remove("onkeypress");
                        }
                    }
                    writer.AddAttribute("onkeypress", str5);
                }
                //if (base.EnableLegacyRendering)
                //{
                writer.AddAttribute("language", "javascript", false);
                //}
            }
            else if (page != null)
            {
                page.ClientScript.RegisterForEventValidation(this.UniqueID, string.Empty);
            }
            if (this.Enabled && !base.IsEnabled)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
            }
            base.AddAttributesToRender(writer);
        }

        protected override void AddParsedSubObject(object obj)
        {
            if (!(obj is LiteralControl))
            {
                //throw new HttpException(SR.GetString("Cannot_Have_Children_Of_Type", new object[] { "TextBox", obj.GetType().Name.ToString(CultureInfo.InvariantCulture) }));

                throw new HttpException("不能有子类型");
            }
            this.Text = ((LiteralControl)obj).Text;
        }

        protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            //base.ValidateEvent(postDataKey);
            string text = this.Text;
            string str2 = postCollection[postDataKey];
            if (!this.ReadOnly && !text.Equals(str2, StringComparison.Ordinal))
            {
                this.Text = str2;
                return true;
            }
            return false;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Page page = this.Page;
            if ((page != null) && base.IsEnabled)
            {
                if (!this.SaveTextViewState)
                {
                    //page.RegisterEnabledControl(this);

                }
                if (this.AutoPostBack)
                {

                    //page.RegisterWebFormsScript();
                    //page.RegisterPostBackScript();
                    //page.RegisterFocusScript();

                }
            }
        }

        protected virtual void OnTextChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)base.Events[EventTextChanged];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaisePostDataChangedEvent()
        {
            if (this.AutoPostBack && !this.Page.IsPostBackEventControlRegistered)
            {
                this.Page.AutoPostBackControl = this;
                if (this.CausesValidation)
                {
                    this.Page.Validate(this.ValidationGroup);
                }
            }
            this.OnTextChanged(EventArgs.Empty);
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

        protected override object SaveViewState()
        {
            if (!this.SaveTextViewState)
            {
                this.ViewState.SetItemDirty("Text", false);
            }
            return base.SaveViewState();
        }

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            return this.LoadPostData(postDataKey, postCollection);
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            this.RaisePostDataChangedEvent();
        }

        // Properties
        [Description("TextBox_AutoCompleteType"), DefaultValue(0), Themeable(false), Category("Behavior")]
        public virtual AutoCompleteType AutoCompleteType
        {
            get
            {
                object obj2 = this.ViewState["AutoCompleteType"];
                if (obj2 != null)
                {
                    return (AutoCompleteType)obj2;
                }
                return AutoCompleteType.None;
            }
            set
            {
                if ((value < AutoCompleteType.None) || (value > AutoCompleteType.Search))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.ViewState["AutoCompleteType"] = value;
            }
        }

        [Themeable(false), Category("Behavior"), Description("TextBox_AutoPostBack"), DefaultValue(false)]
        public virtual bool AutoPostBack
        {
            get
            {
                object obj2 = this.ViewState["AutoPostBack"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                this.ViewState["AutoPostBack"] = value;
            }
        }

        [Description("AutoPostBackControl_CausesValidation"), DefaultValue(false), Themeable(false), Category("Behavior")]
        public virtual bool CausesValidation
        {
            get
            {
                object obj2 = this.ViewState["CausesValidation"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                this.ViewState["CausesValidation"] = value;
            }
        }

        [Category("Appearance"), DefaultValue(0), Description("TextBox_Columns")]
        public virtual int Columns
        {
            get
            {
                object obj2 = this.ViewState["Columns"];
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
                    throw new ArgumentOutOfRangeException("Columns", "文本框无效列");
                }
                this.ViewState["Columns"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(0), Description("TextBox_MaxLength"), Themeable(false)]
        public virtual int MaxLength
        {
            get
            {
                object obj2 = this.ViewState["MaxLength"];
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
                    throw new ArgumentOutOfRangeException("value");
                }
                this.ViewState["MaxLength"] = value;
            }
        }

        [DefaultValue(false), Themeable(false), Category("Behavior"), Description("TextBox_ReadOnly"), Bindable(true)]
        public virtual bool ReadOnly
        {
            get
            {
                object obj2 = this.ViewState["ReadOnly"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                this.ViewState["ReadOnly"] = value;
            }
        }

        [Themeable(false), Category("Behavior"), Description("TextBox_Rows"), DefaultValue(0)]
        public virtual int Rows
        {
            get
            {
                object obj2 = this.ViewState["Rows"];
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
                    throw new ArgumentOutOfRangeException("Rows", "文本框无效行");
                }
                this.ViewState["Rows"] = value;
            }
        }

        private bool SaveTextViewState
        {
            get
            {
                if (this.TextMode == TextBoxMode.Password)
                {
                    return false;
                }
                if (((base.Events[EventTextChanged] == null) && base.IsEnabled) && ((this.Visible && !this.ReadOnly) && (base.GetType() == typeof(TextBox))))
                {
                    return false;
                }
                return true;
            }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                if (this.TextMode == TextBoxMode.MultiLine)
                {
                    return HtmlTextWriterTag.Textarea;
                }
                return HtmlTextWriterTag.Input;
            }
        }

        [Bindable(true, BindingDirection.TwoWay), PersistenceMode(PersistenceMode.EncodedInnerDefaultProperty), Editor("System.ComponentModel.Design.MultilineStringEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), Localizable(true), Category("Appearance"), DefaultValue(""), Description("TextBox_Text")]
        public virtual string Text
        {
            get
            {
                string str = (string)this.ViewState["Text"];
                if (str != null)
                {
                    return str;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["Text"] = value;
            }
        }

        [Themeable(false), Description("TextBox_TextMode"), DefaultValue(0), Category("Behavior")]
        public virtual TextBoxMode TextMode
        {
            get
            {
                object obj2 = this.ViewState["Mode"];
                if (obj2 != null)
                {
                    return (TextBoxMode)obj2;
                }
                return TextBoxMode.SingleLine;
            }
            set
            {
                if ((value < TextBoxMode.SingleLine) || (value > TextBoxMode.Password))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.ViewState["Mode"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(""), Description("PostBackControl_ValidationGroup"), Themeable(false)]
        public virtual string ValidationGroup
        {
            get
            {
                string str = (string)this.ViewState["ValidationGroup"];
                if (str != null)
                {
                    return str;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["ValidationGroup"] = value;
            }
        }

        [Category("Layout"), Description("TextBox_Wrap"), DefaultValue(true)]
        public virtual bool Wrap
        {
            get
            {
                object obj2 = this.ViewState["Wrap"];
                if (obj2 != null)
                {
                    return (bool)obj2;
                }
                return true;
            }
            set
            {
                this.ViewState["Wrap"] = value;
            }
        }

        [Description("TextBox_OnTextChanged"), Category("Action")]
        public event EventHandler TextChanged
        {
            add
            {
                base.Events.AddHandler(EventTextChanged, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventTextChanged, value);
            }
        }
 

 



        /////////////////////////////////////////


        public static string EnsureEndWithSemiColon(string value)
        {
            if (value != null)
            {
                int length = value.Length;
                if ((length > 0) && (value[length - 1] != ';'))
                {
                    return (value + ";");
                }
            }
            return value;
        }

        public static string MergeScript(string firstScript, string secondScript)
        {
            if (!string.IsNullOrEmpty(firstScript))
            {
                return (firstScript + secondScript);
            }
            if (secondScript.TrimStart(new char[0]).StartsWith("javascript:", StringComparison.Ordinal))
            {
                return secondScript;
            }
            return ("javascript:" + secondScript);
        }


        public static bool ClientSupportsJavaScript(Page page)
        {
            Version JavascriptMinimumVersion = new Version("1.0");
            return ((page.Request != null) && (page.Request.Browser.EcmaScriptVersion >= JavascriptMinimumVersion));

        }

 





    }
}
