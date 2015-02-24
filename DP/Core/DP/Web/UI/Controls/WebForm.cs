using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using DP.Common;

namespace DP.Web.UI.Controls
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:WebForm runat=server></{0}:WebForm>")]
    public class WebForm : CompositeControl
    {
        #region 属性

        [Description("表单标题"), Category("扩展"), DefaultValue("")]
        public string Title
        {
            get
            {
                object o = ViewState["Title"];
                return (o != null ? o.ToString() : "");
            }
            set
            {
                ViewState["Title"] = value;
            }
        }
        [Description("控件描述宽度"), Category("扩展"), DefaultValue(100)]
        public int DescriptionWidth
        {
            get
            {
                object o = ViewState["DescriptionWidth"];
                return (o != null ? (int)o : 100);
            }
            set
            {
                ViewState["DescriptionWidth"] = value;
            }
        }
        [Description("控件列数"), Category("扩展"), DefaultValue(1)]
        public int ColumnCount
        {
            get
            {
                object o = ViewState["ColumnCount"];
                return (o != null ? (int)o : 1);
            }
            set
            {
                ViewState["ColumnCount"] = value;
            }
        }
        [Description("控件描述分隔符"), Category("扩展"), DefaultValue("：")]
        public string DescriptionSeparated
        {
            get
            {
                object o = ViewState["DescriptionSeparated"];
                return (o != null ? o.ToString() : "：");
            }
            set
            {
                ViewState["DescriptionSeparated"] = value;
            }
        }
        [Description("控件描述 TD 样式"), Category("样式"), DefaultValue("")]
        public string DescriptionTDCssClass
        {
            get
            {
                object o = ViewState["DescriptionTDCssClass"];
                return (o != null ? o.ToString() : "");
            }
            set
            {
                ViewState["DescriptionTDCssClass"] = value;
            }
        }
        [Description("TD 样式"), Category("样式"), DefaultValue("")]
        public string TDCssClass
        {
            get
            {
                object o = ViewState["TDCssClass"];
                return (o != null ? o.ToString() : "");
            }
            set
            {
                ViewState["TDCssClass"] = value;
            }
        }
        [Description("TABLE 样式"), Category("样式"), DefaultValue("")]
        public string TableCssClass
        {
            get
            {
                object o = ViewState["TableCssClass"];
                return (o != null ? o.ToString() : "");
            }
            set
            {
                ViewState["TableCssClass"] = value;
            }
        }
        [Description("标题 样式"), Category("样式"), DefaultValue("")]
        public string CaptionCssClass
        {
            get
            {
                object o = ViewState["CaptionCssClass"];
                return (o != null ? o.ToString() : "");
            }
            set
            {
                ViewState["CaptionCssClass"] = value;
            }
        }
        [Description("TR 样式"), Category("样式"), DefaultValue("")]
        public string TRCssClass
        {
            get
            {
                object o = ViewState["TRCssClass"];
                return (o != null ? o.ToString() : "");
            }
            set
            {
                ViewState["TRCssClass"] = value;
            }
        }
        [Description("字段数据源"), Category("数据"), DefaultValue(null)]
        public List<WebFormField> FieldDataSource
        {
            get
            {
                object o = ViewState["FieldDataSource"];
                return (o != null ? o as List<WebFormField> : null);
            }
            set
            {
                ViewState["FieldDataSource"] = value;
            }
        }        
        [Description("表单主键"), Category("数据"), DefaultValue(null)]
        public List<string> PrimaryKeys
        {
            get
            {
                object o = ViewState["PrimaryKeys"];
                return (o != null ? o as List<string> : null);
            }
            set
            {
                ViewState["PrimaryKeys"] = value;
            }
        }
        [Description("表单表名"), Category("数据"), DefaultValue("")]
        public string TableName
        {
            get
            {
                object o = ViewState["TableName"];
                return (o != null ? o.ToString() : "");
            }
            set
            {
                ViewState["TableName"] = value;
            }
        }
        [Description("表单数据"), Category("数据"), DefaultValue(null)]
        public Dictionary<string, string> FormDataSource
        {
            get
            {
                object o = ViewState["FormDataSource"];
                return (o != null ? o as Dictionary<string, string> : null);
            }
            set
            {
                ViewState["FormDataSource"] = value;
            }
        }
        [Description("是否成功加载数据"), Category("数据"), DefaultValue(false)]
        public bool IsSuccessfullyLoadedData
        {
            get
            {
                object o = ViewState["IsSuccessfullyLoadedData"];
                return (o != null ? (bool)o : false);
            }
            set
            {
                ViewState["IsSuccessfullyLoadedData"] = value;
            }
        }
        #endregion
        
        #region 事件
        public event DataSourceBindingEventhandler DataSourceBinding;
        public event WebFormFieldChangedEventHandler FieldChanged;
        public event DebugMessage ShowMessage;

        protected virtual List<FieldDataOptions> OnDataSourceBinding(object sender, WebFormField field)
        {
            List<FieldDataOptions> list = field.DataSource;
            if (list == null || list.Count == 0)
            {
                switch (field.DataSourceType)
                {
                    case DataSourceType.String:
                        {
                            list = StringDataSourceBinding(sender, field);
                        }
                        break;
                    case DataSourceType.Enum:
                        {
                            list = EnumDataSourceBinding(sender, field);
                        }
                        break;
                    case DataSourceType.Custom:
                        {
                            if (DataSourceBinding != null)
                            {
                                list = DataSourceBinding(sender, field);
                            }
                        }
                        break;

                }
            }
            return list;
        }

        protected void OnShowMessage(object sender, string message)
        {
            if (ShowMessage != null)
            {
                ShowMessage(sender, message);
            }
        }

        #region 控件触发事件
        /// <summary>
        /// Handles the CheckedChanged event of the rb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void rb_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null)
            {
                string[] rbNameAndKey = rb.ID.Replace(this.Parent.ID.ToLower() + "_", "").Split('_');
                if (rbNameAndKey.Length != 2)
                {
                    return;
                }
                string name = rbNameAndKey[0];
                string key = rbNameAndKey[1];

                WebFormField field = FieldDataSource.Find(delegate(WebFormField cc) { if (cc.Name.ToUpper().Equals(name.ToUpper())) return true; else return false; });

                if (FieldChanged != null)
                {
                    this.FieldChanged(sender, field, field.Type, name.ToUpper(), key);
                }
                //OnShowMessage(field, string.Format("{0}--Name:{1}", "ddl_SelectedIndexChanged", field.Name));
                List<WebFormField> list = RelationFields(field);
                foreach (WebFormField f in list)
                {
                    //OnShowMessage(f, string.Format("{0}--Name:{1}--FieldName:{2}", "RelationFields", f.Name, field.Name));
                    ChildControlsDataBind(f);
                }
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the cb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void cb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                string[] cbNameAndKey = cb.ID.Replace(this.Parent.ID.ToLower() + "_", "").Split('_');
                if (cbNameAndKey.Length != 2)
                {
                    return;
                }
                string name = cbNameAndKey[0];
                string key = GetFieldValue(name);

                WebFormField field = FieldDataSource.Find(delegate(WebFormField cc) { if (cc.Name.ToUpper().Equals(name.ToUpper())) return true; else return false; });

                if (FieldChanged != null)
                {
                    this.FieldChanged(sender, field, field.Type, name.ToUpper(), key);
                }
                //OnShowMessage(field, string.Format("{0}--Name:{1}", "ddl_SelectedIndexChanged", field.Name));
                List<WebFormField> list = RelationFields(field);
                foreach (WebFormField f in list)
                {
                    //OnShowMessage(f, string.Format("{0}--Name:{1}--FieldName:{2}", "RelationFields", f.Name, field.Name));
                    ChildControlsDataBind(f);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the btn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                string name = btn.ID.Replace(this.Parent.ID.ToLower() + "_btn_", "");
                string value = String.Empty;
                WebFormField field = FieldDataSource.Find(delegate(WebFormField cc) { if (cc.Name.ToUpper().Equals(name.ToUpper())) return true; else return false; });
                switch (field.Type)
                {
                    case WebFormFieldType.TextBoxAndButton:
                        {
                            value = GetFieldValue(field);
                        }
                        break;
                    case WebFormFieldType.DropDownListAndButton:
                        {
                            value = GetFieldValue(field);
                        }
                        break;
                }
                if (FieldChanged != null)
                {
                    this.FieldChanged(sender, field, field.Type, name.ToUpper(), value);
                }
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void ddl_SelectedIndexChanged(object sender, EventArgs e)
        {
                DropDownList ddl = sender as DropDownList;
                if (ddl != null)
                {
                    string name = ddl.ID.Replace(this.Parent.ID.ToLower() + "_", "");
                    WebFormField field = FieldDataSource.Find(delegate(WebFormField cc) { if (cc.Name.ToUpper().Equals(name.ToUpper())) return true; else return false; });                    
                    if (FieldChanged != null)
                    {
                        this.FieldChanged(sender, field, field.Type, name.ToUpper(), ddl.SelectedValue);
                    }
                    //OnShowMessage(field, string.Format("{0}--Name:{1}", "ddl_SelectedIndexChanged", field.Name));
                    List<WebFormField> list = RelationFields(field);
                    foreach (WebFormField f in list)
                    {
                        //OnShowMessage(f, string.Format("{0}--Name:{1}--FieldName:{2}", "RelationFields", f.Name, field.Name));
                        f.EventSource = field;
                        f.EventSourceValue = ddl.SelectedValue.Trim();
                        ChildControlsDataBind(f);
                    }
                }
        }

        /// <summary>
        /// Handles the TextChanged event of the txt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void txt_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt != null)
            {
                string name = txt.ID.Replace(this.Parent.ID.ToLower() + "_", "");
                WebFormField field = FieldDataSource.Find(delegate(WebFormField cc) { if (cc.Name.ToLower().Equals(name.ToLower())) return true; else return false; });
                if (FieldChanged != null)
                {
                    this.FieldChanged(sender, field, field.Type, name.ToUpper(), txt.Text);
                } 
                //OnShowMessage(field, string.Format("{0}--Name:{1}", "ddl_SelectedIndexChanged", field.Name));
                List<WebFormField> list = RelationFields(field);
                foreach (WebFormField f in list)
                {
                    //OnShowMessage(f, string.Format("{0}--Name:{1}--FieldName:{2}", "RelationFields", f.Name, field.Name));
                    ChildControlsDataBind(f);
                }
            }
        }
        #endregion

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        protected override void TrackViewState()
        {
            base.TrackViewState();
        }
        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
        }
        protected override void EnsureChildControls()
        {
            base.EnsureChildControls();
        }
        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
        }
        /// <summary>
        /// 由 ASP.NET 页面框架调用，以通知使用基于合成的实现的服务器控件创建它们包含的任何子控件，以便为回发或呈现做准备。
        /// </summary>
        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            if ((Site != null) && Site.DesignMode)
            {
                //设计时
                CreateDesignControls();
                return;
            }
            if (FieldDataSource == null)
            {
                return;
            }
            HtmlTable ht = new HtmlTable();
            ht.Attributes.Add("class", this.TableCssClass);
            HtmlTableRow r = new HtmlTableRow();
            WebFormField field ;           
            int col = 0;
            if (FieldDataSource == null)
            {
                return;
            }
            for (int i = 0; i < FieldDataSource.Count; i++)
            {
                field = FieldDataSource[i];
                if (String.IsNullOrEmpty(field.Name.Trim()))  //判断是不是要要重起一行。
                {
                    
                    if (col != 0)  //判断当前行。是不是以添加过 Feild 如果有 则补冲空白 Td 
                    {
                        SupplementTD(r, ref col);
                        SetTrEnd(ht, ref r, col);
                        SetTrBegin(ref r, col);
                    }
                }
                else
                {
                    SetTrBegin(ref r, col);    //判断是不是要创建新 Tr 
                    field.ColSpan = (field.ColSpan > ColumnCount) ? ColumnCount : field.ColSpan;  //判断 Td 所跨列数是不是大于总列数，如果大于，刚把总列数赋予当然 Td 所跨的列数
                    if ((ColumnCount - col) >= field.ColSpan)   //判断当前行所剩的列数。是否够 Td 的列数。
                    {
                        SetTD(r, ref col, field);
                    }
                    else   //当前行所剩的列数不够，则给当前行。补冲空行 Td  并把  当前行。添加到 Table中。并重新创建一新行，添加Field。
                    {
                        SupplementTD(r, ref col);
                        SetTrEnd(ht, ref r, col);
                        SetTrBegin(ref r, col);
                        SetTD(r, ref col, field);
                    }
                    col = col % ColumnCount;
                    if (i == (FieldDataSource.Count - 1))   //最后一行。补足 Td
                    {
                        SupplementTD(r, ref col);
                    }
                    SetTrEnd(ht, ref r, col);
                }
            }
            BlankTr(ht, ColumnCount);
            this.Controls.Add(ht);
            ChildControlsDataBind();
            ChildControlsCreated = true;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }
        protected override void RenderContents(HtmlTextWriter output)
        {
            base.RenderContents(output);
            // if ((Site != null) && Site.DesignMode)
            //{
            //    //设计时
            //    base.RenderContents(output);
            //    return;
            //}
            //else
            //{
            //    //运行时
            //    base.RenderContents(output);
            //    //output.Write(output);
            //}
            //output.Write(Text);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }
        protected override void LoadControlState(object savedState)
        {
            base.LoadControlState(savedState);
        }
        protected override void RecreateChildControls()
        {
            base.RecreateChildControls();
        }

        
        #region 函数

        #region 数据源绑定相关
        /// <summary>
        /// Childs the controls data bind.
        /// </summary>
        private void ChildControlsDataBind()
        {
            for (int i = 0; i < FieldDataSource.Count; i++)
            {
                ChildControlsDataBind(FieldDataSource[i]);
            }
        }
        /// <summary>
        /// Childs the controls data bind.
        /// </summary>
        /// <param name="field">The field.</param>
        private void ChildControlsDataBind(WebFormField field)
        {
            //OnShowMessage(field, string.Format("{0}--Name:{1}", "ChildControlsDataBind", field.Name));
            switch (field.Type)
            {
                case WebFormFieldType.DropDownList:
                case WebFormFieldType.DropDownListAndButton:
                    {
                        DropDownListBinding(field);
                    }
                    break;
                case WebFormFieldType.TextBox:
                case WebFormFieldType.TextBoxAndButton:
                case WebFormFieldType.RichTextBox:
                case WebFormFieldType.DataTimePicker:
                    {

                    }
                    break;
                case WebFormFieldType.CheckBox:
                    {
                        CheckBoxBinding(field);
                    }
                    break;
                case WebFormFieldType.RadioButton:
                    {
                        RadioButtonBinding(field);
                    }
                    break;
                default:
                    {

                    }
                    break;

            }
        }
        /// <summary>
        /// 下拉列表数据绑定
        /// Drops down list binding.
        /// </summary>
        /// <param name="field">The field.</param>
        private void DropDownListBinding(WebFormField field)
        {
            DropDownList ddl = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as DropDownList;
            if (ddl != null)
            {
                List<FieldDataOptions> list = OnDataSourceBinding(this, field);
                if (field.IsBlankRow)
                {
                    //list.Insert(0, new FieldDataOptions("", ""));
                }
                ddl.DataSource = list;
                ddl.DataTextField = "Text";
                ddl.DataValueField = "Value";
                ddl.DataBind();
                if (field.IsBlankRow)
                {
                    ddl.Items.Insert(0, new ListItem("", ""));
                }
                try
                {
                    DropDownListHelper.ItemSelectedByValue(ddl, field.DefaultValue); 
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("Control", String.Format("FieldName:{0}--FieldValue:{3}\r\nMessage:{1}\r\nStackTrace:{2}", field.Name, ex.Message, ex.StackTrace, field.DefaultValue));
                }
                string value = GetFormDataValue(field.Name);
                if (!String.IsNullOrEmpty(value))
                {
                    DropDownListHelper.ItemSelectedByValue(ddl, value);
                }
                OnShowMessage(field, string.Format("{0}--Name:{1}", "DropDownListBinding", field.Name));
                ddl_SelectedIndexChanged(ddl, new EventArgs());
            }
        }
        /// <summary>
        /// Checks the box binding.
        /// </summary>
        /// <param name="field">The field.</param>
        private void CheckBoxBinding(WebFormField field)
        {
            PlaceHolder ph = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as PlaceHolder;
            if (ph != null)
            {
                CheckBox cb;
                List<FieldDataOptions> list = OnDataSourceBinding(this, field);
                foreach (FieldDataOptions options in list)
                {
                    cb = new CheckBox();
                    cb.ValidationGroup = ph.ID;
                    cb.Text = options.Text;
                    cb.ID = ph.ID + "_" + options.Value;
                    if (options.Value.ToLower() == field.DefaultValue.ToLower())
                    {
                        cb.Checked = true;
                    }
                    if (field.IsPostBack)
                    {
                        cb.AutoPostBack = field.IsPostBack;
                        cb.CheckedChanged += new EventHandler(cb_CheckedChanged);
                    }
                    ph.Controls.Add(cb);
                }
                cb_CheckedChanged(ph.Controls[0], new EventArgs());
            }
        }
        /// <summary>
        /// Radioes the button binding.
        /// </summary>
        /// <param name="field">The field.</param>
        private void RadioButtonBinding(WebFormField field)
        {
            PlaceHolder ph = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as PlaceHolder;
            if (ph != null)
            {
                RadioButton rb;
                List<FieldDataOptions> list = OnDataSourceBinding(this, field);
                foreach (FieldDataOptions options in list)
                {
                    rb = new RadioButton();
                    rb.ValidationGroup = ph.ID;
                    rb.GroupName = ph.ID;
                    rb.Text = options.Text;
                    rb.ID = ph.ID + "_" + options.Value;
                    if (options.Value.ToLower() == field.DefaultValue.ToLower())
                    {
                        rb.Checked = true;
                    } 
                    if (field.IsPostBack)
                    {
                        rb.AutoPostBack = field.IsPostBack;
                        rb.CheckedChanged += new EventHandler(rb_CheckedChanged);
                    }
                    ph.Controls.Add(rb);
                }
                rb_CheckedChanged(ph.Controls[0], new EventArgs());
            }
        }
        /// <summary>
        /// 字段串
        /// Strings the data source binding.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public List<FieldDataOptions> StringDataSourceBinding(object sender, WebFormField field)
        {
            List<FieldDataOptions> list = StringToList(field.DataSourceInfo);;
            return list;
        }
        /// <summary>
        /// Enums the data source binding.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public List<FieldDataOptions> EnumDataSourceBinding(object sender, WebFormField field)
        {
            List<FieldDataOptions> list = new List<FieldDataOptions>();
            if(String.IsNullOrEmpty(field.DataSourceInfo))
            {
                return list;
            }
            try
            {
                if (!Type.GetType(field.DataSourceInfo).IsEnum)
                {
                    return list;
                }
                list = EnumToList(Type.GetType(field.DataSourceInfo));
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Control", String.Format("FieldName:{0}\r\nMessage:{1}\r\nStackTrace:{2}", field.Name, ex.Message, ex.StackTrace));
            }
            return list;
        }

        #endregion

        #region Table 生成相关

        /// <summary>
        /// 创建设计时显示控件
        /// Creates the design controls.
        /// </summary>
        private void CreateDesignControls()
        {
            HtmlTable ht = new HtmlTable();
            ht.Border = 1;
            ht.Width = "100%";

            HtmlTableRow r = new HtmlTableRow();
            HtmlTableCell c = new HtmlTableCell();
            for (int i = 0; i < 3; i++)
            {
                r = new HtmlTableRow();
                for (int j = 0; j < 3; j++)
                {
                    c = new HtmlTableCell();
                    c.InnerHtml = "&nbsp;";
                    r.Cells.Add(c);
                }
                ht.Rows.Add(r);
            }
            this.Controls.Add(ht);
        }

        /// <summary>
        /// 把  TR 添加到 Table 中
        /// Sets the tr end.
        /// </summary>
        /// <param name="ht">The ht.</param>
        /// <param name="r">The r.</param>
        /// <param name="col">The col.</param>
        private void SetTrEnd(HtmlTable ht, ref HtmlTableRow r, int col)
        {
            if (col == 0)
            {
                ht.Rows.Add(r);
            }
        }

        /// <summary>
        /// 创建 新 TR
        /// Sets the tr begin.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="col">The col.</param>
        private void SetTrBegin(ref HtmlTableRow r, int col)
        {
            if (col == 0)
            {
                r = new HtmlTableRow();
                r.Attributes.Add("class", TRCssClass);
            }
        }

        /// <summary>
        /// 在 TR 中活加  TD
        /// Sets the TD.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="col">The col.</param>
        /// <param name="field">The field.</param>
        private void SetTD(HtmlTableRow r, ref int col, WebFormField field)
        {
            HtmlTableCell c = new HtmlTableCell();
            c.Attributes.Add("class", DescriptionTDCssClass);
            c.InnerHtml = "<div class=\"nowrap\">" + field.Description + this.DescriptionSeparated + "</div>";
            r.Cells.Add(c);
            c = new HtmlTableCell();
            c.Attributes.Add("class", TDCssClass);
            c.ColSpan = field.ColSpan * 2 - 1;
            GenerateControl(field, c);
            r.Cells.Add(c);


            col = col + field.ColSpan;
        }

        /// <summary>
        /// 在 TD 中添加控件
        /// Generates the control.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="c">The c.</param>
        private void GenerateControl(WebFormField field, HtmlTableCell c)
        {
            switch (field.Type)
            {
                case WebFormFieldType.TextBox:
                    {
                        if (field.IsPostBack)
                        {
                            UpdatePanel up = new UpdatePanel();
                            up.ID = this.Parent.ID.ToLower() + "_up_" + field.Name.ToLower();
                            TextBox txt = new TextBox();
                            txt.ID = this.Parent.ID.ToLower() + "_" + field.Name.ToLower();
                            txt.CssClass = field.CssClass;
                            txt.MaxLength = field.MaxLength;
                            txt.Text = field.DefaultValue;
                            txt.Enabled = field.IsEnabled;
                            if (field.IsPostBack)
                            {
                                txt.TextChanged += new EventHandler(txt_TextChanged);
                                txt.AutoPostBack = field.IsPostBack;
                            }
                            up.ContentTemplateContainer.Controls.Add(txt);
                            c.Controls.Add(up);
                        }
                        else
                        {
                            TextBox txt = new TextBox();
                            txt.ID = this.Parent.ID.ToLower() + "_" + field.Name.ToLower();
                            txt.CssClass = field.CssClass;
                            txt.MaxLength = field.MaxLength;
                            txt.Text = field.DefaultValue;
                            txt.Enabled = field.IsEnabled;
                            if (field.IsPostBack)
                            {
                                txt.TextChanged += new EventHandler(txt_TextChanged);
                                txt.AutoPostBack = field.IsPostBack;
                            }
                            c.Controls.Add(txt);
                        }
                       
                    }
                    break;
                case WebFormFieldType.DataTimePicker:
                    {
                        if (field.IsPostBack)
                        {
                            UpdatePanel up = new UpdatePanel();
                            up.ID = this.Parent.ID.ToLower() + "_up_" + field.Name.ToLower();
                            TextBox txt = new TextBox();
                            txt.ID = this.Parent.ID.ToLower() + "_" + field.Name.ToLower();
                            PageHelper.SetPopCalender(txt);
                            txt.MaxLength = field.MaxLength;
                            txt.Text = field.DefaultValue;
                            txt.Enabled = field.IsEnabled;
                            if (field.IsPostBack)
                            {
                                txt.TextChanged += new EventHandler(txt_TextChanged);
                                txt.AutoPostBack = field.IsPostBack;
                            }
                            up.ContentTemplateContainer.Controls.Add(txt);
                            c.Controls.Add(up);
                        }
                        else
                        {
                            TextBox txt = new TextBox();
                            txt.ID = this.Parent.ID.ToLower() + "_" + field.Name.ToLower();
                            PageHelper.SetPopCalender(txt);
                            txt.MaxLength = field.MaxLength;
                            txt.Text = field.DefaultValue;
                            txt.Enabled = field.IsEnabled;
                            if (field.IsPostBack)
                            {
                                txt.TextChanged += new EventHandler(txt_TextChanged);
                                txt.AutoPostBack = field.IsPostBack;
                            }
                            c.Controls.Add(txt);
                        }
                    }
                    break;
                case WebFormFieldType.RichTextBox:
                    {
                        if (field.IsPostBack)
                        {
                            UpdatePanel up = new UpdatePanel();
                            up.ID = this.Parent.ID.ToLower() + "_up_" + field.Name.ToLower();

                            TextBox txt = new TextBox();
                            txt.TextMode = TextBoxMode.MultiLine;
                            txt.ID = this.Parent.ID.ToLower() + "_" + field.Name.ToLower();
                            txt.CssClass = field.CssClass;
                            txt.MaxLength = field.MaxLength;
                            txt.Text = field.DefaultValue;
                            txt.Enabled = field.IsEnabled;
                            if (field.IsPostBack)
                            {
                                txt.TextChanged += new EventHandler(txt_TextChanged);
                                txt.AutoPostBack = field.IsPostBack;
                            }

                            up.ContentTemplateContainer.Controls.Add(txt);
                            c.Controls.Add(up);
                        }
                        else
                        {

                            TextBox txt = new TextBox();
                            txt.TextMode = TextBoxMode.MultiLine;
                            txt.ID = this.Parent.ID.ToLower() + "_" + field.Name.ToLower();
                            txt.CssClass = field.CssClass;
                            txt.MaxLength = field.MaxLength;
                            txt.Text = field.DefaultValue;
                            txt.Enabled = field.IsEnabled;
                            if (field.IsPostBack)
                            {
                                txt.TextChanged += new EventHandler(txt_TextChanged);
                                txt.AutoPostBack = field.IsPostBack;
                            }

                            c.Controls.Add(txt);
                        }
                    }
                    break;
                case WebFormFieldType.TextBoxAndButton:
                    {
                        if (field.IsPostBack)
                        {
                            UpdatePanel up = new UpdatePanel();
                            up.ID = this.Parent.ID.ToLower() + "_up_" + field.Name.ToLower();

                            TextBox txt = new TextBox();
                            txt.ID = this.Parent.ID.ToLower() + "_" + field.Name.ToLower();
                            txt.MaxLength = field.MaxLength;
                            txt.Text = field.DefaultValue;
                            txt.Enabled = field.IsEnabled;
                            Button btn = new Button();
                            btn.ID = this.Parent.ID.ToLower() + "_btn_" + field.Name.ToLower();
                            btn.Text = field.CommandText;
                            btn.Enabled = field.IsEnabled;
                            if (field.IsPostBack)
                            {
                                btn.Click += new EventHandler(btn_Click);
                            }

                            up.ContentTemplateContainer.Controls.Add(txt);
                            up.ContentTemplateContainer.Controls.Add(btn);
                            c.Controls.Add(up);
                        }
                        else
                        {

                            TextBox txt = new TextBox();
                            txt.ID = this.Parent.ID.ToLower() + "_" + field.Name.ToLower();
                            txt.MaxLength = field.MaxLength;
                            txt.Text = field.DefaultValue;
                            txt.Enabled = field.IsEnabled;
                            c.Controls.Add(txt);
                            Button btn = new Button();
                            btn.ID = this.Parent.ID.ToLower() + "_btn_" + field.Name.ToLower();
                            btn.Text = field.CommandText;
                            btn.Enabled = field.IsEnabled;
                            if (field.IsPostBack)
                            {
                                btn.Click += new EventHandler(btn_Click);
                            }
                            c.Controls.Add(btn);
                        }
                    }
                    break;
                case WebFormFieldType.CheckBox:
                    {
                        if (field.IsPostBack)
                        {
                            UpdatePanel up = new UpdatePanel();
                            up.ID = this.Parent.ID.ToLower() + "_up_" + field.Name.ToLower();

                            PlaceHolder ph = new PlaceHolder();
                            ph.ID = this.Parent.ID.ToLower() + "_" + field.Name.ToLower();
                            
                            up.ContentTemplateContainer.Controls.Add(ph);
                            c.Controls.Add(up);
                        }
                        else
                        {

                            PlaceHolder ph = new PlaceHolder();
                            ph.ID = this.Parent.ID.ToLower() + "_" + field.Name.ToLower();
                            c.Controls.Add(ph);
                        }
                    }
                    break;
                case WebFormFieldType.RadioButton:
                    {
                        if (field.IsPostBack)
                        {
                            UpdatePanel up = new UpdatePanel();
                            up.ID = this.Parent.ID.ToLower() + "_up_" + field.Name.ToLower();

                            PlaceHolder ph = new PlaceHolder();
                            ph.ID = this.Parent.ID.ToLower() + "_" + field.Name.ToLower();

                            up.ContentTemplateContainer.Controls.Add(ph);
                            c.Controls.Add(up);
                        }
                        else
                        {
                            PlaceHolder ph = new PlaceHolder();
                            ph.ID = this.Parent.ID.ToLower() + "_" + field.Name.ToLower();
                            c.Controls.Add(ph);
                        }
                    }
                    break;
                case WebFormFieldType.DropDownList:
                    {
                        if (field.IsPostBack)
                        {
                            UpdatePanel up = new UpdatePanel();
                            up.ID = this.Parent.ID.ToLower() + "_up_" + field.Name.ToLower();

                            DropDownList ddl = new DropDownList();
                            ddl.ID = this.Parent.ID.ToLower() + "_" + field.Name.ToLower();
                            ddl.CssClass = field.CssClass;
                            ddl.Enabled = field.IsEnabled;
                            //ddl.SelectedValue = field.DefaultValue;
                            if (field.IsPostBack)
                            {
                                ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
                                ddl.AutoPostBack = field.IsPostBack;
                            }
                            up.ContentTemplateContainer.Controls.Add(ddl);
                            c.Controls.Add(up);
                        }
                        else
                        {
                            DropDownList ddl = new DropDownList();
                            ddl.ID = this.Parent.ID.ToLower() + "_" + field.Name.ToLower();
                            ddl.CssClass = field.CssClass;
                            ddl.Enabled = field.IsEnabled;
                            //ddl.SelectedValue = field.DefaultValue;
                            if (field.IsPostBack)
                            {
                                ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
                                ddl.AutoPostBack = field.IsPostBack;
                            }

                            c.Controls.Add(ddl);
                        }
                    }
                    break;
                case WebFormFieldType.DropDownListAndButton:
                    {
                        if (field.IsPostBack)
                        {
                            UpdatePanel up = new UpdatePanel();
                            up.ID = this.Parent.ID.ToLower() + "_up_" + field.Name.ToLower();

                            DropDownList ddl = new DropDownList();
                            ddl.ID = this.Parent.ID.ToLower() + "_" + field.Name.ToLower();
                            ddl.CssClass = field.CssClass;
                            ddl.Enabled = field.IsEnabled;
                            //ddl.SelectedValue = field.DefaultValue;
                            //if (field.IsPostBack)
                            //{
                            //    ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
                            //    ddl.AutoPostBack = field.IsPostBack;
                            //}
                            Button btn = new Button();
                            btn.ID = this.Parent.ID.ToLower() + "_btn_" + field.Name.ToLower();
                            btn.Text = field.CommandText;
                            btn.Enabled = field.IsEnabled;
                            if (field.IsPostBack)
                            {
                                btn.Click += new EventHandler(btn_Click);
                            }
                            up.ContentTemplateContainer.Controls.Add(ddl);
                            up.ContentTemplateContainer.Controls.Add(btn);
                            c.Controls.Add(up);
                            
                        }
                        else
                        {
                            DropDownList ddl = new DropDownList();
                            ddl.ID = this.Parent.ID.ToLower() + "_" + field.Name.ToLower();
                            ddl.CssClass = field.CssClass;
                            ddl.Enabled = field.IsEnabled;
                            //ddl.SelectedValue = field.DefaultValue;
                            //if (field.IsPostBack)
                            //{
                            //    ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
                            //    ddl.AutoPostBack = field.IsPostBack;
                            //}
                            c.Controls.Add(ddl);
                            Button btn = new Button();
                            btn.ID = this.Parent.ID.ToLower() + "_btn_" + field.Name.ToLower();
                            btn.Text = field.CommandText;
                            btn.Enabled = field.IsEnabled;
                            if (field.IsPostBack)
                            {
                                btn.Click += new EventHandler(btn_Click);
                            }
                            c.Controls.Add(btn);
                        }
                    }
                    break;
                    

            }
        }

        /// <summary>
        /// 在 TR  中补充空白  TD
        /// Supplements the TD.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="col">The col.</param> 
        private void SupplementTD(HtmlTableRow r, ref int col)
        {
            HtmlTableCell c = new HtmlTableCell();
            while (col != 0)
            {
                c = new HtmlTableCell();
                c.Attributes.Add("class", DescriptionTDCssClass);
                c.InnerHtml = "&nbsp;";
                r.Cells.Add(c);
                c = new HtmlTableCell();
                c.Attributes.Add("class", TDCssClass);
                c.InnerHtml = "&nbsp;";
                r.Cells.Add(c);
                col++;
                col = col % ColumnCount;
            }
        }

        /// <summary>
        /// 添加空白行
        /// Blanks the tr.
        /// </summary>
        /// <param name="ht">The ht.</param>
        /// <param name="count">The count.</param>
        private void BlankTr(HtmlTable ht, int count)
        {

            HtmlTableRow r = new HtmlTableRow();
            r.Attributes.Add("style", "line-height: 0px; height: 0px; ");
            HtmlTableCell c;
            for (int i = 0; i < count; i++)
            {
                c = new HtmlTableCell();
                r.Cells.Add(c);
                c = new HtmlTableCell();
                c.Attributes.Add("style", "min-width:100px;");
                r.Cells.Add(c);
            }
            ht.Rows.Add(r);
        }
        #endregion

        #region 生成数据选项 List
        /// <summary>
        /// Enums to list.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <returns></returns>
        public List<FieldDataOptions> EnumToList(Type enumType)
        {
            List<FieldDataOptions> list = new List<FieldDataOptions>();
            foreach (int i in Enum.GetValues(enumType))
            {
                FieldDataOptions options = new FieldDataOptions(i.ToString(), Enum.GetName(enumType, i));
                list.Add(options);
            }
            return list;
        }

        /// <summary>
        /// Strings to list.
        /// </summary>
        /// <param name="stringValue">The string value.</param>
        /// <returns></returns>
        public List<FieldDataOptions> StringToList(String stringValue)
        {
            List<FieldDataOptions> list = new List<FieldDataOptions>();
            string[] strItems = stringValue.Split(',');
            for (int j = 0; j < strItems.Length; j++)
            {
                string[] strItems_sub = strItems[j].Split('-');
                if (strItems_sub.Length == 2)
                {
                    if (StringHelper.IsNumber(strItems_sub[0]) == true)
                    {
                        list.Add(new FieldDataOptions(strItems_sub[0], strItems_sub[1]));
                    }
                }
                else
                {
                    break;
                }
            }
            if (list.Count == strItems.Length)
            {
                return list;
            }
            else
            {
                list.Clear();
                for (int i = 0; i < strItems.Length; i++)
                {
                    list.Add(new FieldDataOptions(i.ToString(), strItems[i].ToString()));
                }
            }
            return list;
        }

        /// <summary>
        /// Relations the fields.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        private List<WebFormField> RelationFields(WebFormField field)
        {
            WebFormField f;
            List<WebFormField> list = new List<WebFormField>();
            if (field == null)
            {
                return list;
            }
            string[] strItems = field.RelationFields.Split(',');
            foreach (string item in strItems)
            {
                f = FieldDataSource.Find(delegate(WebFormField cc) { if (cc.Name.ToUpper().Equals(item.ToUpper())) return true; else return false; });
                if (f != null)
                {
                    list.Add(f);
                }
            }
            return list;
        }
        #endregion

        #region 获取字段值
        /// <summary>
        /// Gets the form data value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public string GetFormDataValue(string fieldName)
        {
            if (FormDataSource == null)
            {
                return string.Empty;
            }
            if (!FormDataSource.ContainsKey(fieldName.ToLower()))
            {
                return string.Empty;
            }
            return FormDataSource[fieldName.ToLower()];
        }
        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        private string GetFieldValue(string fieldName)
        {
            string rel = string.Empty;
            WebFormField field = this.FieldDataSource.Find(delegate(WebFormField f) { if (f.Name.ToLower().Equals(fieldName.ToLower())) return true; else return false; });
            if (field != null)
            {
                rel = GetFieldValue(field);
            }
            return rel;
        }
        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        private string GetFieldValue(WebFormField field)
        {
            TextBox txt;
            DropDownList ddl;
            PlaceHolder ph;
            CheckBox cb;
            RadioButton rb;
            string rel = string.Empty;
            switch (field.Type)
            {
                case WebFormFieldType.TextBox:
                    {
                        txt = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as TextBox;
                        if (txt != null)
                        {
                            rel = txt.Text;
                        }
                    }
                    break;
                case WebFormFieldType.DataTimePicker:
                    {
                        txt = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as TextBox;
                        if (txt != null)
                        {
                            rel = txt.Text;
                        }
                    }
                    break;
                case WebFormFieldType.RichTextBox:
                    {
                        txt = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as TextBox;
                        if (txt != null)
                        {
                            rel = txt.Text;
                        }
                    }
                    break;
                case WebFormFieldType.TextBoxAndButton:
                    {
                        txt = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as TextBox;
                        if (txt != null)
                        {
                            rel = txt.Text;
                        }
                    }
                    break;
                case WebFormFieldType.DropDownList:
                    {
                        ddl = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as DropDownList;
                        if (ddl != null)
                        {
                            rel = ddl.SelectedValue;
                        }
                    }
                    break;
                case WebFormFieldType.DropDownListAndButton:
                    {
                        ddl = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as DropDownList;
                        if (ddl != null)
                        {
                            rel = ddl.SelectedValue;
                        }
                    }
                    break;
                case WebFormFieldType.CheckBox:
                    {
                        ph = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as PlaceHolder;
                        foreach (Control c in ph.Controls)
                        {
                            if (c is CheckBox)
                            {
                                cb = c as CheckBox;
                                if (cb.Checked)
                                {
                                    if (rel.Length != 0)
                                    {
                                        rel += ",";
                                    }
                                    rel += cb.ID.Replace(ph.ID + "_", "");
                                }
                            }
                        }
                    }
                    break;
                case WebFormFieldType.RadioButton:
                    {
                        ph = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as PlaceHolder;
                        foreach (Control c in ph.Controls)
                        {
                            if (c is RadioButton)
                            {
                                rb = c as RadioButton;
                                if (rb.Checked)
                                {
                                    if (rel.Length != 0)
                                    {
                                        rel += ",";
                                    }
                                    rel += rb.ID.Replace(ph.ID + "_", "");
                                }
                            }
                        }
                    }
                    break;
                default:
                    {

                    }
                    break;
            }
            return rel;
        }        
        /// <summary>
        /// Gets the field values.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetFieldValues()
        {
            Dictionary<string, string> dicts = new Dictionary<string, string>();            
            foreach (WebFormField field in this.FieldDataSource)
            {
                if (!dicts.ContainsKey(field.Name.ToLower()))
                {
                    dicts.Add(field.Name.ToLower(), GetFieldValue(field));
                }
            }
            return dicts;
        }

        /// <summary>
        /// Gets the field values.
        /// </summary>
        /// <param name="dicts">The dicts.</param>
        /// <returns></returns>
        public string GetFieldValues(ref Dictionary<string, string> dicts)
        {
            StringBuilder message = new StringBuilder();
            string newValue = string.Empty;
            string oldValue = string.Empty;
            foreach (WebFormField field in this.FieldDataSource)
            {
                if (!dicts.ContainsKey(field.Name.ToLower()))
                {
                    oldValue = string.Empty;
                    newValue =  GetFieldValue(field);
                    dicts.Add(field.Name.ToLower(), newValue);
                    if (oldValue != newValue)
                    {
                        message.AppendFormat("{0}:{1}-->{2}|", field.Description, oldValue, newValue);
                    }
                }
                else
                {
                    oldValue = dicts[field.Name.ToLower()].ToString();
                    newValue = GetFieldValue(field);
                    if (oldValue != newValue)
                    {
                        dicts.Remove(field.Name.ToLower());
                        dicts.Add(field.Name.ToLower(), newValue);
                        message.AppendFormat("{0}:{1}-->{2}|", field.Description, oldValue, newValue);
                    }
                }
            }

            return message.ToString();
        }
        #endregion

        #region 设置字段值
        /// <summary>
        /// Sets the field value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private bool SetFieldValue(string fieldName, string value)
        {
            bool rel = true;
            WebFormField field = this.FieldDataSource.Find(delegate(WebFormField f) { if (f.Name.ToLower().Equals(fieldName.ToLower())) return true; else return false; });
            if (field != null)
            {
                rel = SetFieldValue(field, value);
            }
            return rel;
        }
        /// <summary>
        /// Sets the field value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private bool SetFieldValue(WebFormField field, string value)
        {
            bool rel = true;
            TextBox txt;
            DropDownList ddl;
            PlaceHolder ph;
            CheckBox cb;
            RadioButton rb;
            switch (field.Type)
            {
                case WebFormFieldType.TextBox:
                    {
                        txt = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as TextBox;
                        if (txt != null)
                        {
                            txt.Text = value;
                        }
                        else
                        {
                            rel = false;
                        }

                    }
                    break;
                case WebFormFieldType.DataTimePicker:
                    {
                        txt = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as TextBox;
                        if (txt != null)
                        {
                            txt.Text = value;
                        }
                        else
                        {
                            rel = false;
                        }
                    }
                    break;
                case WebFormFieldType.RichTextBox:
                    {
                        txt = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as TextBox;
                        if (txt != null)
                        {
                            txt.Text = value;
                        }
                        else
                        {
                            rel = false;
                        }
                    }
                    break;
                case WebFormFieldType.TextBoxAndButton:
                    {
                        txt = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as TextBox;
                        if (txt != null)
                        {
                            txt.Text = value;
                        }
                        else
                        {
                            rel = false;
                        }
                    }
                    break;
                case WebFormFieldType.DropDownList:
                    {
                        ddl = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as DropDownList;
                        if (ddl != null)
                        {
                            rel = DropDownListHelper.ItemSelectedByValue(ddl, value);
                        }
                        else
                        {
                            rel = false;
                        }
                    }
                    break;
                case WebFormFieldType.DropDownListAndButton:
                    {
                        ddl = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as DropDownList;
                        if (ddl != null)
                        {
                            rel = DropDownListHelper.ItemSelectedByValue(ddl, value);
                        }
                        else
                        {
                            rel = false;
                        }
                    }
                    break;
                case WebFormFieldType.CheckBox:
                    {
                        ph = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as PlaceHolder;
                        foreach (Control c in ph.Controls)
                        {
                            if (c is CheckBox)
                            {
                                cb = c as CheckBox;
                                cb.Checked = false;
                            }
                        }
                        string[] items = value.Split(',');
                        foreach (string item in items)
                        {
                            cb = ph.FindControl(ph.ID + "_" + item) as CheckBox;
                            cb.Checked = true;
                        }
                    }
                    break;
                case WebFormFieldType.RadioButton:
                    {
                        ph = this.FindControl(this.Parent.ID.ToLower() + "_" + field.Name.ToLower()) as PlaceHolder;
                        foreach (Control c in ph.Controls)
                        {
                            if (c is RadioButton)
                            {
                                rb = c as RadioButton;
                                rb.Checked = false;
                            }
                        }
                        string[] items = value.Split(',');
                        foreach (string item in items)
                        {
                            rb = ph.FindControl(ph.ID + "_" + item) as RadioButton;
                            rb.Checked = true;
                        }
                    }
                    break;
                default:
                    {

                    }
                    break;
            }
            List<WebFormField> list = RelationFields(field);
            foreach (WebFormField f in list)
            {
                //OnShowMessage(f, string.Format("{0}--Name:{1}--FieldName:{2}", "RelationFields", f.Name, field.Name));
                f.EventSource = field;
                f.EventSourceValue = value;
                ChildControlsDataBind(f);
            }
            return rel;
        }
        /// <summary>
        /// Datas the loaded.
        /// </summary>
        /// <param name="dicts">The dicts.</param>
        /// <returns></returns>
        public bool DataLoaded(Dictionary<string, string> dicts)
        {
            bool rel = true;
            IsSuccessfullyLoadedData = true;
            FormDataSource = dicts;
            foreach (WebFormField field in this.FieldDataSource)
            {
                if (!String.IsNullOrEmpty(field.Name.Trim()))
                {
                    if (dicts.ContainsKey(field.Name.ToLower()))
                    {
                        if (!SetFieldValue(field, dicts[field.Name.ToLower()]))
                        {
                            rel = false;
                        }
                    }
                }
            }
            return rel;
        }
        /// <summary>
        /// Datas the loaded.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns></returns>
        public bool DataLoaded(DataTable dt)
        {
            bool rel = true;
            if (dt == null)
            {
                return false;
            }
            if (dt.Rows.Count < 1)
            {
                return false;
            }
            Dictionary<string, string> dicts = DataTableHelper.GetRow(dt, 0);
            rel = DataLoaded(dicts);
            return rel;
        }

        #endregion

        #endregion
    }

    

    


}
