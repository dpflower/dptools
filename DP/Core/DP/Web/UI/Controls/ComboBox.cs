using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Globalization;

namespace DP.Web.UI.Controls
{
    [ToolboxData("<{0}:ComboBox runat=server></{0}:ComboBox>")]
    [ParseChildren(true, "Items")]
    [ControlValueProperty("SelectedValue")]
    [SupportsEventValidation()]
    [ValidationProperty("Text")]
    [Designer("System.Web.UI.Design.WebControls.ListControlDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public sealed partial class ComboBox : ListControl, INamingContainer, IPostBackDataHandler
    {

        #region 私有 成员
        private string cachedSelectedValue;
        private int cachedSelectedIndex;
        private bool _TextIsSelected = true;

        private TextBox _txtBox;
        private Image _img;


        private Style _txtBxStyle;
        private Style _imgStyle;
        #endregion

        #region 常量
        private const string _jsListBoxShow = "ComboBox_ListBoxShow({0},{1});";
        private const string _jsListBoxHide = "ComboBox_ListBoxHide({0});";
        private const string _jsListBoxKeyup = "ComboBox_ListBoxKeyup({0},{1},{2});";
        private const string _jsListBoxDisplaySelectedText = "ComboBox_ListBoxDisplaySelectedText({0},{1});";
        private const string _jsTextBoxKeyup = "ComboBox_TextBoxKeyup({0},{1},event)";


        private string _jsNormal1 = "javascript:if({1}!=_activeComboBoxTextBox) {1}.className='{0}';";
        private string _jsNormal2 = "javascript:{1}.className='{0}';SetActiveComboBoxTextBox(null);";
        private string _jsHover = "javascript:if({1}!=_activeComboBoxTextBox) {1}.className='{0}';";
        private string _jsActive = "javascript:{1}.className='{0}';SetActiveComboBoxTextBox({1});";

        private const string _selectionOutOfRange = "'{0}' has a {1} which is invalid because it does not exist in the list of items.";
        private const string _mutuallyExclusive = "The '{0}' and '{1}' attributes are mutually exclusive.";

        #endregion

        public string ListBoxClientID
        {
            get
            {
                return String.Format("{0}_ListBox", this.ClientID);
            }
        }
        public override string ClientID
        {
            get
            {
                return base.ClientID + "_Wrapper";
            }
        }

        #region 私有 属性

        private string ListBoxWrapperClientID
        {
            get
            {
                return this.ClientID + "_ListBoxWrapper";
            }
        }
        private string ListBoxShowClientFunction
        {
            get
            {
                EnsureChildControls();
                return String.Format(_jsListBoxShow, this.ClientID, ListBoxWrapperClientID);
            }
        }
        private string ListBoxHideClientFunction
        {
            get
            {
                EnsureChildControls();
                return String.Format(_jsListBoxHide, ListBoxClientID + ".offsetParent");
            }
        }
        private string ListBoxKeyupClientFuction
        {
            get
            {
                EnsureChildControls();
                return String.Format(_jsListBoxKeyup, ListBoxClientID, _txtBox.ClientID, "event");
            }
        }
        private string ListBoxDisplaySelectedTextClientFunction
        {
            get
            {
                EnsureChildControls();
                return String.Format(_jsListBoxDisplaySelectedText, ListBoxClientID, _txtBox.ClientID);
            }
        }
        private string TextBoxKeyupClientFuction
        {
            get
            {
                EnsureChildControls();
                return String.Format(_jsTextBoxKeyup, _txtBox.ClientID, ListBoxClientID);
            }
        }

        private bool HasDataSource
        {
            get
            {
                return (!String.IsNullOrEmpty(DataSourceID) && DataSource == null);
            }
        }

        #endregion

        public override ControlCollection Controls
        {
            get
            {
                EnsureChildControls();
                return base.Controls;
            }
        }

        [Bindable(false)]
        [Browsable(true)]
        [Themeable(false)]
        [DefaultValue("")]
        [Category("Client Events")]
        public string OnClientChange
        {
            get
            {
                object obj = ViewState["OnClientChanage"];
                return (obj == null) ? String.Empty : (string)obj;
            }
            set
            {
                ViewState["OnClientChanage"] = value;
            }
        }

        public bool EnableFreeText
        {
            get
            {
                object obj = ViewState["EnableFreeText"];
                return (obj == null) ? true : (bool)obj;
            }
            set
            {
                ViewState["EnableFreeText"] = value;
            }
        }

        #region 公共 过时 属性
        [Obsolete("This property is not used")]
        [Themeable(false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override System.Drawing.Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }
        [Obsolete("This property is not used")]
        [Themeable(false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override BorderStyle BorderStyle
        {
            get
            {
                return base.BorderStyle;
            }
            set
            {
                base.BorderStyle = value;
            }
        }
        [Obsolete("This property is not used")]
        [Themeable(false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Unit BorderWidth
        {
            get
            {
                return base.BorderWidth;
            }
            set
            {
                base.BorderWidth = value;
            }
        }
        [Obsolete("This property is not used")]
        [Themeable(false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override System.Drawing.Color BorderColor
        {
            get
            {
                return base.BorderColor;
            }
            set
            {
                base.BorderColor = value;
            }
        }
        [Obsolete("This property is not used")]
        [Themeable(false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override FontInfo Font
        {
            get
            {
                return base.Font;
            }
        }
        [Obsolete("This property is not used")]
        [Themeable(false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override System.Drawing.Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
            }
        }
        [Obsolete("This property is not used")]
        [Themeable(false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string CssClass
        {
            get
            {
                return base.CssClass;
            }
            set
            {
                base.CssClass = value;
            }
        }
        [Obsolete("This property is not used")]
        [Themeable(false)]
        [Browsable(false)]
        [DefaultValue("")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Unit Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                base.Height = value;
            }
        }
        #endregion

        #region 行为属性
        [Bindable(false)]
        [Browsable(true)]
        [Themeable(false)]
        [DefaultValue(false)]
        [Category("Behavior")]
        public override bool AutoPostBack
        {
            get
            {
                EnsureChildControls();
                return _txtBox.AutoPostBack;
            }
            set
            {
                EnsureChildControls();
                _txtBox.AutoPostBack = value;
            }
        }

        [Bindable(false)]
        [Browsable(true)]
        [Themeable(false)]
        [DefaultValue("")]
        [Category("Behavior")]
        public override string ValidationGroup
        {
            get
            {
                EnsureChildControls();
                return _txtBox.ValidationGroup;
            }
            set
            {
                EnsureChildControls();

                _txtBox.ValidationGroup = value;
            }
        }

        [Bindable(true)]
        [Browsable(false)]
        [Themeable(true)]
        [DefaultValue("")]
        [Category("Behavior")]
        public override string ToolTip
        {
            get
            {
                EnsureChildControls();
                return _txtBox.ToolTip;
            }
            set
            {
                EnsureChildControls();
                _txtBox.ToolTip = value;
            }
        }

        [Bindable(true)]
        [Browsable(true)]
        [Themeable(false)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                EnsureChildControls();
                base.Enabled = value;
            }
        }

        [Bindable(true)]
        [Browsable(true)]
        [Themeable(false)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get
            {
                object obj = ViewState["ReadOnly"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["ReadOnly"] = value;
            }
        }

        [Bindable(false)]
        [Browsable(false)]
        [Themeable(false)]
        [DefaultValue("")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get
            {
                EnsureChildControls();
                return _txtBox.Text;
            }
            set
            {
                EnsureChildControls();
                _txtBox.Text = value;
            }
        }
        #endregion

        #region 外观与风格特性

        [Themeable(true)]
        [Browsable(true)]
        [DefaultValue("")]
        [Category("Layout")]
        public override Unit Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                base.Width = value;
            }
        }

        [Bindable(false)]
        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue(5)]
        [Category("Appearance")]
        public int Rows
        {
            get
            {
                object obj = ViewState["Rows"];
                return (obj == null || (int)obj <= 0) ? 5 : (int)obj;
            }
            set
            {
                ViewState["Rows"] = value;
            }
        }

        [Bindable(true)]
        [Browsable(true)]
        [Themeable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [UrlProperty]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        public string ImageUrl
        {
            get
            {
                object obj = ViewState["ImageUrl"];
                return (obj == null) ? String.Empty : (string)obj;
            }
            set
            {
                ViewState["ImageUrl"] = value;
            }
        }

        [Bindable(false)]
        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue("")]
        [Category("Style")]
        public string TextBoxHoverCssClass
        {
            get
            {
                object obj = ViewState["HoverCssClass"];
                return (obj == null) ? CssClass : (string)obj;
            }
            set
            {
                ViewState["HoverCssClass"] = value;
            }
        }

        [Bindable(false)]
        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue("")]
        [Category("Style")]
        public string TextBoxActiveCssClass
        {
            get
            {
                object obj = ViewState["ActiveCssClass"];
                return (obj == null) ? CssClass : (string)obj;
            }
            set
            {
                ViewState["ActiveCssClass"] = value;
            }
        }

        [Bindable(false)]
        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue("")]
        [Category("Style")]
        public string TextBoxReadOnlyCssClass
        {
            get
            {
                object obj = ViewState["TextBoxReadOnlyCssClass"];
                return (obj == null) ? CssClass : (string)obj;
            }
            set
            {
                ViewState["TextBoxReadOnlyCssClass"] = value;
            }
        }

        [Category("Style")]
        [Themeable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
        [PersistenceMode(PersistenceMode.Attribute)]
        public Style TextBoxStyle
        {
            get
            {
                if (_txtBxStyle == null)
                {
                    _txtBxStyle = new Style();
                    if (IsTrackingViewState)
                        ((IStateManager)_txtBxStyle).TrackViewState();
                }
                return _txtBxStyle;
            }
        }

        //[Category("Style")]
        //[Themeable(true)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        //[NotifyParentProperty(true)]
        //[PersistenceMode(PersistenceMode.Attribute)]
        //public Style ListBoxStyle
        //{
        //    get
        //    {
        //        if (_lstBxStyle == null)
        //        {
        //            _lstBxStyle = new Style();
        //            if (IsTrackingViewState)
        //                ((IStateManager)_lstBxStyle).TrackViewState();
        //        }
        //        return _lstBxStyle;
        //    }
        //}

        [Category("Style")]
        [Themeable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
        [PersistenceMode(PersistenceMode.Attribute)]
        public Style ImageStyle
        {
            get
            {
                if (_imgStyle == null)
                {
                    _imgStyle = new Style();
                    if (IsTrackingViewState)
                        ((IStateManager)_imgStyle).TrackViewState();
                }
                return _imgStyle;
            }
        }
        #endregion

        #region 选定Xxx属性

        [Category("Behavior")]
        [DefaultValue(0)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override int SelectedIndex
        {
            get
            {
                int selectedIndex = base.SelectedIndex;
                if ((selectedIndex < 0) && (this.Items.Count > 0))
                {
                    this.Items[0].Selected = true;
                    selectedIndex = 0;
                }
                return selectedIndex;
            }
            set
            {
                base.SelectedIndex = value;
                this.cachedSelectedIndex = value;
            }
        }

        [Browsable(false)]
        [Themeable(false)]
        [DefaultValue("")]
        [Bindable(true, BindingDirection.TwoWay)]
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string SelectedValue
        {
            get
            {
                if (_TextIsSelected)
                {
                    return base.SelectedValue;
                }
                return FindValueByText(this.Text);
            }
            set
            {
                base.SelectedValue = value;
                this.cachedSelectedValue = value;
            }
        }

        [Bindable(false)]
        [Browsable(false)]
        [Themeable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedValueInt32
        {
            get
            {
                EnsureChildControls();
                int value;
                return (!int.TryParse(this.SelectedValue, out value)) ? -1 : value;
            }
            set
            {
                EnsureChildControls();
                this.SelectedValue = value.ToString();
            }
        }
        #endregion

        #region 杂项 属性

        [Bindable(false)]
        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue(true)]
        [Category("Misc")]
        public bool EnableDefaultItem
        {
            get
            {
                object obj = ViewState["EnableDefaultItem"];
                return (obj == null) ? true : (bool)ViewState["EnableDefaultItem"];
            }
            set
            {
                ViewState["EnableDefaultItem"] = value;
            }
        }

        [Bindable(false)]
        [Browsable(true)]
        [Themeable(true)]
        [DefaultValue("")]
        [Category("Misc")]
        public string DefaultItemValue
        {
            get
            {
                object obj = ViewState["DefaultItemValue"];
                return (obj == null) ? "" : (string)ViewState["DefaultItemValue"];
            }
            set
            {
                ViewState["DefaultItemValue"] = value;
            }
        }

        [Bindable(false)]
        [Browsable(true)]
        [Themeable(true)]
        [Localizable(true)]
        [DefaultValue("")]
        [Category("Misc")]
        public string DefaultItemText
        {
            get
            {
                object obj = ViewState["DefaultItemText"];
                return (obj == null) ? "" : (string)ViewState["DefaultItemText"];
            }
            set
            {
                ViewState["DefaultItemText"] = value;
            }
        }
        #endregion

        #region 公共覆盖方法
        public override void ClearSelection()
        {
            //EnsureChildControls();
            //_txtBox.Text = String.Empty;
            base.ClearSelection();
        }
        #endregion

        #region 保护覆盖方法

        protected override void PerformDataBinding(System.Collections.IEnumerable dataSource)
        {
            if (dataSource != null)
            {
                bool flag1 = false;
                bool flag2 = false;
                string text1 = this.DataTextField;
                string text2 = this.DataValueField;
                string text3 = this.DataTextFormatString;
                if (!this.AppendDataBoundItems)
                {
                    this.Items.Clear();
                }
                if (EnableDefaultItem)
                    this.Items.Insert(0, new ListItem(this.DefaultItemText, this.DefaultItemValue));

                ICollection collection1 = dataSource as ICollection;
                if (collection1 != null)
                    this.Items.Capacity = collection1.Count + this.Items.Count;

                if ((text1.Length != 0) || (text2.Length != 0))
                    flag1 = true;

                if (text3.Length != 0)
                    flag2 = true;

                foreach (object obj1 in dataSource)
                {
                    ListItem item1 = new ListItem();
                    if (flag1)
                    {
                        if (text1.Length > 0)
                        {
                            item1.Text = DataBinder.GetPropertyValue(obj1, text1, text3);
                        }
                        if (text2.Length > 0)
                        {
                            item1.Value = DataBinder.GetPropertyValue(obj1, text2, null);
                        }
                    }
                    else
                    {
                        if (flag2)
                        {
                            item1.Text = string.Format(CultureInfo.CurrentCulture, text3, new object[] { obj1 });
                        }
                        else
                        {
                            item1.Text = obj1.ToString();
                        }
                        item1.Value = obj1.ToString();
                    }
                    this.Items.Add(item1);
                }
            }

            if (this.cachedSelectedValue != null)
            {
                int num1 = -1;
                num1 = this.FindByValueInternal(this.cachedSelectedValue, true);
                if (-1 == num1)
                {
                    throw new ArgumentOutOfRangeException("value", String.Format(_selectionOutOfRange, new object[] { this.ID, "SelectedValue" }));
                }
                //if ((this.cachedSelectedIndex != -1) && (this.cachedSelectedIndex != num1))
                //{
                //    throw new ArgumentException(String.Format(_mutuallyExclusive, new object[] { "SelectedIndex", "SelectedValue" }));
                //}
                this.SelectedIndex = num1;
                this.cachedSelectedValue = null;
                this.cachedSelectedIndex = -1;
            }
            else if (this.cachedSelectedIndex != -1)
            {
                this.SelectedIndex = this.cachedSelectedIndex;
                this.cachedSelectedIndex = -1;
            }

        }

        protected override void CreateChildControls()
        {
            Controls.Clear();

            _txtBox = new TextBox();
            _txtBox.ID = "ComboBoxTextBox";
            _txtBox.Width = Unit.Percentage(100);

            _img = new Image();
            _img.ID = "ComboBoxPopupImage";


            this.Controls.Add(_txtBox);
            this.Controls.Add(_img);

        }

        protected override void LoadViewState(object savedState)
        {
            if (savedState == null)
                base.LoadViewState(savedState);
            else
            {
                object[] state = (object[])savedState;
                if (state.Length != 3)
                    throw new ArgumentException("Invalid view state");

                base.LoadViewState(state[0]);
                if (state[1] != null)
                    ((IStateManager)_txtBxStyle).LoadViewState(state[1]);
                if (state[2] != null)
                    ((IStateManager)_imgStyle).LoadViewState(state[3]);
            }
        }

        protected override object SaveViewState()
        {
            object[] state = new object[3];
            state[0] = base.SaveViewState();

            state[1] = (_txtBxStyle != null) ? ((IStateManager)_txtBxStyle).SaveViewState() : null;
            state[2] = (_imgStyle != null) ? ((IStateManager)_imgStyle).SaveViewState() : null;

            //Make sure that we are returning a state that contains something
            for (int i = 0; i < 3; i++)
            {
                if (state[i] != null)
                    return state;
            }
            //otherwise return null
            return null;
        }

        protected override void TrackViewState()
        {
            base.TrackViewState();
            if (_txtBxStyle != null)
                ((IStateManager)_txtBxStyle).TrackViewState();
            if (_imgStyle != null)
                ((IStateManager)_imgStyle).TrackViewState();
        }

        protected override void OnPreRender(EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), "DP.Resources.JavaScript.ComboBoxUtil.js");
            if (String.IsNullOrEmpty(this.ImageUrl))
            {
                this.ImageUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "DP.Resources.Images.ComboBoxDropArrow.gif");
            }
            base.OnPreRender(e);
        }
        protected override void OnPagePreLoad(object sender, EventArgs e)
        {
            //Add Default Item
            if (this.Page != null && !this.HasDataSource && this.EnableDefaultItem)
            {
                if (!this.Page.IsPostBack)
                {
                    Items.Insert(0, new ListItem(this.DefaultItemText, this.DefaultItemValue));
                }
                else if (!this.EnableViewState && this.Page.IsPostBack)
                {
                    Items.Insert(0, new ListItem(this.DefaultItemText, this.DefaultItemValue));
                }
                if (String.IsNullOrEmpty(this.SelectedValue) && !this.Page.IsPostBack)
                {
                    this.SelectedIndex = 0;
                    this.Text = this.DefaultItemText;
                }
                if (this.Page.IsPostBack)
                {
                    if (this.Text != this.SelectedItem.Text)
                    {
                        _TextIsSelected = false;
                    }
                    else
                    {
                        _TextIsSelected = true;
                    }
                }
            }
            base.OnPagePreLoad(sender, e);
        }

        #endregion

        #region Render
        private void PrepareChildControlsForRender()
        {
            _img.Enabled = this.Enabled;
            _txtBox.Enabled = this.Enabled;

            //Apply ReadOnly
            _txtBox.ReadOnly = this.ReadOnly;
            if (!ReadOnly)
            {
                string jsNormal1 = String.Format(_jsNormal1, this.TextBoxStyle.CssClass, _txtBox.ClientID);
                string jsHover = String.Format(_jsHover, TextBoxHoverCssClass, _txtBox.ClientID);
                string jsActive = String.Format(_jsActive, TextBoxActiveCssClass, _txtBox.ClientID);
                string jsNormal2 = String.Format(_jsNormal2, this.TextBoxStyle.CssClass, _txtBox.ClientID);

                _txtBox.Attributes["onkeydown"] = "if(event.keyCode == 9){" + ListBoxDisplaySelectedTextClientFunction + "return false;}";
                _txtBox.Attributes["onkeyup"] = TextBoxKeyupClientFuction;
                _txtBox.Attributes["onmouseover"] = jsHover;
                _txtBox.Attributes["onmouseout"] = jsNormal1;
                _txtBox.Attributes["onfocus"] = jsActive;
                _txtBox.Attributes["onblur"] = jsNormal2 + String.Format("ComboBox_TextBoxBlur(this,{0})", ListBoxWrapperClientID);
            }
            else
            {
                this.TextBoxStyle.CssClass = TextBoxReadOnlyCssClass;
            }
            //Apply Styles
            if (_txtBxStyle != null)
                _txtBox.ApplyStyle(_txtBxStyle);
            _txtBox.Width = this.Width;

            if (_imgStyle != null)
                _img.ApplyStyle(_imgStyle);

            _img.ImageUrl = this.ImageUrl;
            _img.ImageAlign = ImageAlign.AbsMiddle;

            if (!EnableFreeText)
                _txtBox.Attributes.Add("readonly", "");
            else
            {
                if (!ReadOnly)
                    _txtBox.Attributes.Remove("readonly");
            }

            if (this.Enabled && !this.ReadOnly)
            {//Render onclick event handler for ListBox drop down if the control is Enabled or not ReadOnly
                _img.Attributes["onclick"] = ListBoxShowClientFunction;

            }

            //以TextBox 内容为主，
            if (this.Text != this.SelectedItem.Text)
            {
                //this.SelectedValue = string.Empty;
                //this.SelectedIndex = -1;
            }
            //if (this.SelectedValue != String.Empty)
            //    this.Text = this.SelectedItem.Text; 
            
        }
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            string jsNormal1 = String.Format(_jsNormal1, this.TextBoxStyle.CssClass, _txtBox.ClientID);
            string jsHover = String.Format(_jsHover, TextBoxHoverCssClass, _txtBox.ClientID);
            string jsActive = String.Format(_jsActive, TextBoxActiveCssClass, _txtBox.ClientID);
            string jsNormal2 = String.Format(_jsNormal2, this.TextBoxStyle.CssClass, _txtBox.ClientID);
            string jsOnclick = ListBoxDisplaySelectedTextClientFunction;
            string jsOnChange = (!String.IsNullOrEmpty(this.OnClientChange)) ? String.Format("{0}(this);", this.OnClientChange) : "";

            writer.AddAttribute(HtmlTextWriterAttribute.Size, this.Rows.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ListBoxClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            writer.AddAttribute(HtmlTextWriterAttribute.Onclick, jsOnclick);
            writer.AddAttribute(HtmlTextWriterAttribute.Onchange, jsOnChange);
            writer.AddAttribute("onkeyup", ListBoxKeyupClientFuction);
            writer.AddAttribute("onblur", ListBoxHideClientFunction + jsNormal2);
            writer.AddAttribute("onmouseover", jsHover);
            writer.AddAttribute("onmouseout", jsNormal1);
            writer.AddAttribute("onfocus", jsActive);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            if (this.DesignMode)
            {
                EnsureChildControls();
            }
            PrepareChildControlsForRender();
            //Render TextBox & Dropdown Image

            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Nowrap, "nowrap");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "auto");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "inline");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            _txtBox.RenderControl(writer);
            _img.RenderControl(writer);

            //Render ListBox
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ListBoxWrapperClientID);
            writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "absolute");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Visibility, "hidden");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Top, "-100px");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Left, "-100px");

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            AddAttributesToRender(writer);
            writer.RenderBeginTag(this.TagName);

            base.RenderContents(writer);
            writer.RenderEndTag();

            writer.RenderEndTag();
            writer.RenderEndTag();
            //writer.Write("&nbsp;");
        }
        #endregion

        #region IPostBackDataHandler
        bool IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            if (!base.IsEnabled)
            {
                return false;
            }
            string[] stringArray = postCollection.GetValues(postDataKey);
            this.EnsureDataBound();
            if (stringArray != null)
            {
                this.ValidateEvent(postDataKey, stringArray[0]);
                int num1 = this.FindByValueInternal(stringArray[0], false);
                if (this.SelectedIndex != num1)
                {
                    base.SetPostDataSelection(num1);
                    return true;
                }
            }
            return false;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            this.OnSelectedIndexChanged(EventArgs.Empty);
        }
        #endregion

        #region Helpers
        private void ValidateEvent(string uniqueID, string eventArgument)
        {
            if (this.Page != null && this.EnableViewState)
            {
                this.Page.ClientScript.ValidateEvent(uniqueID, eventArgument);
            }
        }

        private int FindByValueInternal(string value, bool includeDisabled)
        {
            int num1 = 0;
            foreach (ListItem itm in this.Items)
            {
                if (itm.Value.Equals(value) && (includeDisabled || itm.Enabled))
                {
                    return num1;
                }
                num1++;
            }
            return -1;
        }

        private string FindValueByText(string text)
        {
            foreach (ListItem itm in this.Items)
            {
                if (itm.Text.Equals(text) && itm.Enabled)
                {
                    return itm.Value;
                }
            }
            return null;
        }
        #endregion
    }
}
