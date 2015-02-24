using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using DP.Common;
using System.Text.RegularExpressions;

namespace DP.Web.UI.Controls
{
    public class WebForms 
    {
        public delegate Dictionary<string, string> DataSourceBindingEventhandler(object sender, string dataSourceString, string dataSourceKey);
        public delegate DataTable RelationEventhandler(object sender, string relationString);
        public delegate void ChildControlChangedEventHandler(object sender, ChildControl childControl, ChildControlType type, string name, string value);



        public enum ChildControlType
        {
            TextBox = 0,
            DropDownList = 1,
            RadioButton = 2,
            CheckBox = 3,
            DataTimePicker = 5,
            RichTextBox= 10,
            DropDownListAndButton = 11,
            TextBoxAndButton = 12
        }

        public class ChildControl
        {
            string _description = string.Empty;

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            /// <value>The description.</value>
            public string Description
            {
                get { return _description; }
                set { _description = value; }
            }
            string _name = string.Empty;

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }
            ChildControlType _type = ChildControlType.TextBox;

            public ChildControlType Type
            {
                get { return _type; }
                set { _type = value; }
            }
            int _width = 80;

            public int Width
            {
                get { return _width; }
                set { _width = value; }
            }
            int _maxLength = 0;

            public int MaxLength
            {
                get { return _maxLength; }
                set { _maxLength = value; }
            }
            bool _enabled = true;

            public bool Enabled
            {
                get { return _enabled; }
                set { _enabled = value; }
            }

            bool _isPostBack = false;

            public bool IsPostBack
            {
                get { return _isPostBack; }
                set { _isPostBack = value; }
            }

            bool _exclusiveRow = false;

            public bool ExclusiveRow
            {
                get { return _exclusiveRow; }
                set { _exclusiveRow = value; }
            }

            Dictionary<string, string> _dataSource = new Dictionary<string, string>();

            public Dictionary<string, string> DataSource
            {
                get { return _dataSource; }
                set { _dataSource = value; }
            }

            string _fromat = string.Empty;

            public string Fromat
            {
                get { return _fromat; }
                set { _fromat = value; }
            }

            string _defaultValue = string.Empty;

            public string DefaultValue
            {
                get { return _defaultValue; }
                set { _defaultValue = value; }
            }

            string _relation = string.Empty;

            public string Relation
            {
                get { return _relation; }
                set { _relation = value; }
            }





            public ChildControl()
            {

            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ChildControl"/> class.
            /// </summary>
            /// <param name="controlsDescription">The controls description.</param>
            /// <param name="controlsName">Name of the controls.</param>
            /// <param name="controlsType">Type of the controls.</param>
            /// <param name="controlsWidth">Width of the controls.</param>
            /// <param name="maxLength">Length of the max.</param>
            /// <param name="enabled">if set to <c>true</c> [enabled].</param>
            /// <param name="ControlsDataSource">The controls value list.</param>
            /// <param name="controlsFromat">The controls fromat.</param>
            public ChildControl(string description, string name, ChildControlType type, int width, int maxLength, bool enabled, Dictionary<string, string> dataSource, string fromat)
            {
                _description = description;
                _name = name;
                _type = type;
                _width = width;
                _maxLength = maxLength;
                _enabled = enabled;
                _dataSource = dataSource;
                _fromat = fromat;
            }
            public ChildControl(string description, string name, ChildControlType type, int width, int maxLength, bool enabled, Dictionary<string, string> dataSource, string fromat, string defaultValue)
            {
                _description = description;
                _name = name;
                _type = type;
                _width = width;
                _maxLength = maxLength;
                _enabled = enabled;
                _dataSource = dataSource;
                _fromat = fromat;
                _defaultValue = defaultValue;
            }
            public ChildControl(string description, string name, ChildControlType type, int width, int maxLength, bool enabled, Dictionary<string, string> dataSource, string fromat, bool exclusiveRow)
            {
                _description = description;
                _name = name;
                _type = type;
                _width = width;
                _maxLength = maxLength;
                _enabled = enabled;
                _dataSource = dataSource;
                _fromat = fromat;
                _exclusiveRow = exclusiveRow;
            }
            public ChildControl(string description, string name, ChildControlType type, int width, int maxLength, bool enabled, Dictionary<string, string> dataSource, string fromat, bool exclusiveRow, string defaultValue)
            {
                _description = description;
                _name = name;
                _type = type;
                _width = width;
                _maxLength = maxLength;
                _enabled = enabled;
                _dataSource = dataSource;
                _fromat = fromat;
                _exclusiveRow = exclusiveRow;
                _defaultValue = defaultValue;
            }            
            public ChildControl(string description, string name, ChildControlType type, int width, int maxLength, bool enabled, Dictionary<string, string> dataSource, string fromat, bool exclusiveRow, string defaultValue, bool isPostBack)
            {
                _description = description;
                _name = name;
                _type = type;
                _width = width;
                _maxLength = maxLength;
                _enabled = enabled;
                _dataSource = dataSource;
                _fromat = fromat;
                _exclusiveRow = exclusiveRow;
                _defaultValue = defaultValue;
                _isPostBack = isPostBack;
            }
            public ChildControl(string description, string name, ChildControlType type, int width, int maxLength, bool enabled, Dictionary<string, string> dataSource, string fromat, bool exclusiveRow, string defaultValue, bool isPostBack, string relation)
            {
                _description = description;
                _name = name;
                _type = type;
                _width = width;
                _maxLength = maxLength;
                _enabled = enabled;
                _dataSource = dataSource;
                _fromat = fromat;
                _exclusiveRow = exclusiveRow;
                _defaultValue = defaultValue;
                _isPostBack = isPostBack;
                _relation = relation;
            }
            public ChildControl(string description, string name, ChildControlType type)
            {
                _description = description;
                _name = name;
                _type = type;
            }



        }

        #region 变量
        int _descriptionWidth = 100;
        int _columnCount = 2;
        string _descriptionSeparated = "：";
        string _descriptionTDCssClass = string.Empty;
        string _tDCssClass = string.Empty;
        string _tableCssClass = string.Empty;
        string _captionCssClass = string.Empty;
        List<ChildControl> _childControls = new List<ChildControl>();
        DataTable _dataTable = null;
        Panel _panel = null;

        public event DataSourceBindingEventhandler DataSourceBinding;
        public event ChildControlChangedEventHandler ChildControlChanged;
        public event RelationEventhandler Relation;
        #endregion

        #region 属性

        /// <summary>
        /// 表单 控件描述宽度 Gets or sets the width of the controls description.
        /// </summary>
        /// <value>The width of the controls description.</value>
        public int DescriptionWidth
        {
            get { return _descriptionWidth; }
            set { _descriptionWidth = value; }
        }

        /// <summary>
        /// 表单 控件列数 Gets or sets the controls column count.
        /// </summary>
        /// <value>The controls column count.</value>
        public int ColumnCount
        {
            get { return _columnCount; }
            set { _columnCount = value; }
        }

        /// <summary>
        /// 表单 控件描述分隔符 Gets or sets the controls description separated.
        /// </summary>
        /// <value>The controls description separated.</value>
        public string DescriptionSeparated
        {
            get { return _descriptionSeparated; }
            set { _descriptionSeparated = value; }
        }

        /// <summary>
        /// 表单 控件描述 TD 样式  Gets or sets the controls description TD CSS class.
        /// </summary>
        /// <value>The controls description TD CSS class.</value>
        public string DescriptionTDCssClass
        {
            get { return _descriptionTDCssClass; }
            set { _descriptionTDCssClass = value; }
        }

        /// <summary>
        /// 表单 TD 样式 Gets or sets the controls TD CSS class.
        /// </summary>
        /// <value>The controls TD CSS class.</value>
        public string TDCssClass
        {
            get { return _tDCssClass; }
            set { _tDCssClass = value; }
        }

        /// <summary>
        /// 表单 TABLE 样式 Gets or sets the controls table CSS class.
        /// </summary>
        /// <value>The controls table CSS class.</value>
        public string TableCssClass
        {
            get { return _tableCssClass; }
            set { _tableCssClass = value; }
        }

        /// <summary>
        /// 表单 标题 样式 Gets or sets the controls caption CSS class.
        /// </summary>
        /// <value>The controls caption CSS class.</value>
        public string CaptionCssClass
        {
            get { return _captionCssClass; }
            set { _captionCssClass = value; }
        }

        /// <summary>
        /// 表单控件列表 Gets or sets the controls list.
        /// </summary>
        /// <value>The controls list.</value>
        public List<ChildControl> ChildControls
        {
            get { return _childControls; }
            set
            {
                _childControls = value;
            }
        }

        /// <summary>
        /// 表单数据 Gets or sets the data table.
        /// </summary>
        /// <value>The data table.</value>
        public DataTable DataTable
        {
            get { return _dataTable; }
            set { _dataTable = value; }
        }

        /// <summary>
        /// 表单控件容器 Gets or sets the panel.
        /// </summary>
        /// <value>The panel.</value>
        public Panel Panel
        {
            get { return _panel; }
            set { _panel = value; }
        } 
        #endregion

        #region 构造函数
        public WebForms()
        {

        }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化控件
        /// </summary>
        /// <param name="panel">控件容器</param>
        public void InitControls(Panel panel)
        {
            _panel = panel;

            HtmlTable ht = new HtmlTable();
            ht.Attributes.Add("width", "100%");
            ht.Attributes.Add("class", _tableCssClass);

            HtmlTableRow r = new HtmlTableRow();
            HtmlTableCell c = new HtmlTableCell();
            TextBox txt;
            DropDownList ddl;
            Button btn;
            CheckBox cb;
            RadioButton rb;
            int col = 0;

            for (int i = 0; i < _childControls.Count; i++)
            {
                col++;
                WebForms.ChildControl fc = _childControls[i];
                if (col == 1)
                {
                    r = new HtmlTableRow();
                }

                if (fc.ExclusiveRow)
                {
                    if (col != 1)
                    {
                        for (int j = col; j <= _columnCount; j++)
                        {
                            c = new HtmlTableCell();
                            c.Attributes.Add("class", _descriptionTDCssClass);
                            c.Attributes.Add("width", _descriptionWidth.ToString() + "px");
                            c.Controls.Add(new LiteralControl(""));
                            r.Cells.Add(c);
                            c = new HtmlTableCell();
                            c.Attributes.Add("class", _tDCssClass);
                            r.Cells.Add(c);
                        }
                        ht.Rows.Add(r);
                        r = new HtmlTableRow();
                        col = _columnCount;
                    }
                }

                c = new HtmlTableCell();
                c.Attributes.Add("class", _descriptionTDCssClass);
                c.Attributes.Add("width", _descriptionWidth.ToString() + "px");
                c.Controls.Add(new LiteralControl(fc.Description + _descriptionSeparated));
                r.Cells.Add(c);

                c = new HtmlTableCell();
                c.Attributes.Add("class", _tDCssClass);
                if (fc.ExclusiveRow)
                {
                    c.ColSpan = 2 * _columnCount - 1;
                    col = _columnCount;
                }

                #region 创建控件
                switch (fc.Type)
                {
                    case ChildControlType.TextBox:
                        {
                            txt = new TextBox();
                            txt.ID = _panel.ID.ToLower() + "_" + fc.Name.ToLower();
                            txt.Width = fc.Width;
                            if (fc.IsPostBack)
                            {
                                txt.TextChanged += new EventHandler(txt_TextChanged);
                                txt.AutoPostBack = fc.IsPostBack;
                            }
                            c.Controls.Add(txt);
                        }
                        break;
                    case ChildControlType.DropDownList:
                        {
                            ddl = new DropDownList();
                            ddl.ID = _panel.ID.ToLower() + "_" + fc.Name.ToLower();
                            ddl.Width = fc.Width;
                            ddl.DataSource = fc.DataSource;
                            ddl.DataValueField = "Key";
                            ddl.DataTextField = "Value";
                            ddl.DataBind();
                            if (fc.IsPostBack)
                            {
                                ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
                                ddl.AutoPostBack = fc.IsPostBack;
                            }
                            c.Controls.Add(ddl);
                        }
                        break;
                    case ChildControlType.DataTimePicker:
                        {
                            txt = new TextBox();
                            txt.ID = _panel.ID.ToLower() + "_" + fc.Name.ToLower();
                            txt.Width = fc.Width;
                            PageHelper.SetPopCalender(txt);
                            if (fc.IsPostBack)
                            {
                                txt.TextChanged += new EventHandler(txt_TextChanged);
                                txt.AutoPostBack = fc.IsPostBack;
                            }
                            c.Controls.Add(txt);
                        }
                        break;
                    case ChildControlType.CheckBox:
                        {
                            string[] keys = DictionaryHelper.GetDictionaryKeys(fc.DataSource);
                            for (int keyIndex = 0; keyIndex < keys.Length; keyIndex++)
                            {
                                cb = new CheckBox();
                                cb.ID = _panel.ID.ToLower() + "_" + fc.Name.ToLower() + "_" + keys[keyIndex].ToString();
                                cb.Text = fc.DataSource[keys[keyIndex]].ToString();
                                if (fc.IsPostBack)
                                {
                                    cb.CheckedChanged += new EventHandler(cb_CheckedChanged);
                                    cb.AutoPostBack = fc.IsPostBack;
                                }
                                c.Controls.Add(cb);
                            }
                        }
                        break;
                    case ChildControlType.DropDownListAndButton:
                        {
                            ddl = new DropDownList();
                            ddl.ID = _panel.ID.ToLower() + "_" + fc.Name.ToLower();
                            ddl.Width = fc.Width;
                            ddl.DataSource = fc.DataSource;
                            ddl.DataValueField = "Key";
                            ddl.DataTextField = "Value";
                            ddl.DataBind();
                            c.Controls.Add(ddl);
                            btn = new Button();
                            btn.ID = _panel.ID.ToLower() + "_btn_" + fc.Name.ToLower();
                            if (fc.IsPostBack)
                            {
                                btn.Click += new EventHandler(btn_Click);
                               
                            }
                            c.Controls.Add(btn);
                        }
                        break;
                    case ChildControlType.RadioButton:
                        {
                            string[] keys = DictionaryHelper.GetDictionaryKeys(fc.DataSource);
                            for (int keyIndex = 0; keyIndex < keys.Length; keyIndex++)
                            {
                                rb = new RadioButton();
                                rb.ID = _panel.ID.ToLower() + "_" + fc.Name.ToLower() + "_" + keys[keyIndex].ToString();
                                rb.Text = fc.DataSource[keys[keyIndex]].ToString();
                                rb.GroupName = _panel.ID.ToLower() + "_" + fc.Name.ToLower();
                                if (fc.IsPostBack)
                                {
                                    rb.CheckedChanged += new EventHandler(rb_CheckedChanged);
                                    rb.AutoPostBack = fc.IsPostBack;
                                }
                                c.Controls.Add(rb);
                            }

                        }
                        break;
                    case ChildControlType.RichTextBox:
                        {
                            txt = new TextBox();
                            txt.ID = _panel.ID.ToLower() + "_" + fc.Name.ToLower();
                            txt.Width = fc.Width;
                            txt.TextMode = TextBoxMode.MultiLine;
                            if (fc.MaxLength == 0)
                            {
                                txt.Rows = 3;
                            }
                            else
                            {
                                txt.Rows = IntHelper.divideMax(fc.MaxLength * 12, fc.Width);
                            }
                            if (fc.IsPostBack)
                            {
                                txt.TextChanged += new EventHandler(txt_TextChanged);
                                txt.AutoPostBack = fc.IsPostBack;
                            }
                            c.Controls.Add(txt);
                        }
                        break;
                    case ChildControlType.TextBoxAndButton:
                        {
                            txt = new TextBox();
                            txt.ID = _panel.ID.ToLower() + "_" + fc.Name.ToLower();
                            txt.Width = fc.Width;
                            c.Controls.Add(txt);
                            btn = new Button();
                            btn.ID = _panel.ID.ToLower() + "_btn_" + fc.Name.ToLower();
                            if (fc.IsPostBack)
                            {
                                btn.Click += new EventHandler(btn_Click);
                            }
                            c.Controls.Add(btn);
                        }
                        break;
                    default:
                        {

                        }
                        break;
                }

                #endregion

                r.Cells.Add(c);


                if ((i + 1) == _childControls.Count)
                {
                    for (int j = col; j < _columnCount; j++)
                    {
                        c = new HtmlTableCell();
                        c.Attributes.Add("class", _descriptionTDCssClass);
                        c.Attributes.Add("width", _descriptionWidth.ToString() + "px");
                        c.Controls.Add(new LiteralControl(""));
                        r.Cells.Add(c);
                        c = new HtmlTableCell();
                        c.Attributes.Add("class", _tDCssClass);
                        r.Cells.Add(c);
                    }
                    ht.Rows.Add(r);
                    continue;
                }

                if (col == _columnCount)
                {
                    ht.Rows.Add(r);
                    col -= _columnCount;
                }
            }

            for (int i = 0; i < _childControls.Count; i++)
            {
                WebForms.ChildControl fc = _childControls[i];
                if (!String.IsNullOrEmpty(fc.Relation))
                {
                    OnRelation(fc, fc.Relation);
                }
            }

            _panel.Controls.Add(ht);
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="dt">表单数据</param>
        public void LoadData(DataTable dt)
        {
            _dataTable = dt;
            if (dt.Rows.Count == 0)
            {
                return;
            }

            TextBox txt;
            DropDownList ddl;
            foreach (ChildControl fc in _childControls)
            {
                switch (fc.Type)
                {
                    case ChildControlType.TextBox:
                        {
                            txt = _panel.FindControl(_panel.ID.ToLower() + "_" + fc.Name.ToLower()) as TextBox;
                            if (txt != null)
                            {
                                if (dt.Columns.Contains(fc.Name.ToLower()))
                                {
                                    txt.Text = dt.Rows[0][fc.Name.ToLower()].ToString();
                                }
                            }
                        }
                        break;
                    case ChildControlType.DropDownList:
                        {
                            ddl = _panel.FindControl(_panel.ID.ToLower() + "_" + fc.Name.ToLower()) as DropDownList;
                            if (ddl != null)
                            {
                                if (dt.Columns.Contains(fc.Name.ToLower()))
                                {
                                    ddl.SelectedValue = dt.Rows[0][fc.Name.ToLower()].ToString();
                                }
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 获取表单数据。
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetChildControlsData()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (ChildControl fc in _childControls)
            {
                dict.Add(fc.Name.ToLower(), GetChildControlData(fc));
            }

            return dict;
        }

        public string GetChildControlData(string name)
        {
            string rel = string.Empty;
            ChildControl fc = this.ChildControls.Find(delegate(ChildControl f) { if (f.Name.ToLower().Equals(name.ToLower())) return true; else return false; });
            if (fc != null)
            {
                rel = GetChildControlData(fc);
            }
            return rel;
        }

        public string GetChildControlData(ChildControl childControl)
        {
            TextBox txt;
            DropDownList ddl;
            string rel = string.Empty;
            switch (childControl.Type)
            {
                case ChildControlType.TextBox:
                    {
                        txt = _panel.FindControl(_panel.ID.ToLower() + "_" + childControl.Name.ToLower()) as TextBox;
                        if (txt != null)
                        {
                            rel = txt.Text;
                        }
                        
                    }
                    break;
                case ChildControlType.DropDownList:
                    {
                        ddl = _panel.FindControl(_panel.ID.ToLower() + "_" + childControl.Name.ToLower()) as DropDownList;
                        if (ddl != null)
                        {
                            rel = ddl.SelectedValue;
                        }

                    }
                    break;
                case ChildControlType.DataTimePicker:
                    {

                    }
                    break;
                case ChildControlType.CheckBox:
                    {

                    }
                    break;
                case ChildControlType.DropDownListAndButton:
                    {

                    }
                    break;
                case ChildControlType.RadioButton:
                    {


                    }
                    break;
                case ChildControlType.RichTextBox:
                    {

                    }
                    break;
                case ChildControlType.TextBoxAndButton:
                    {

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
        /// 将控件添加到控件列表
        /// </summary>
        /// <param name="list">控件列表</param>
        /// <param name="fc">控件</param>
        public void ChildControlsAdd(List<ChildControl> list, ChildControl fc)
        {
            if (list.Exists(delegate(ChildControl f) { if (f.Name.ToLower().Equals(fc.Name.ToLower())) return true; else return false; }))
            {
                throw new Exception("WebForms.ChildControlListAdd(List<ChildControl> list, ChildControl fc)，控件名<" + fc.Name + ">以存在！");
            }
            list.Add(fc);
        }

        /// <summary>
        /// 生成控件列表
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="vLevel">显示级别：bit 0在详细资料中显示 bit1在列表中显示，bit2在弹出中显示</param>
        /// <param name="nMask">显示类型：BIT0,代表公共字段 BIT1,代表类型1  BIT2，代表代表类型2</param>
        /// <returns></returns>
        public List<ChildControl> GenerationControlsList(DataTable dt, int vLevel, int nMask)
        {
            string fld_name = "";
            string fld_value;
            string fld_cbo_list;
            string fld_def_value;
            string fld_null;
            string fld_format;
            string fld_default;
            string fld_relation;

            int fld_cbo;
            int fld_vlevels;
            int fld_rdonly;
            int fld_group;
            int fld_width;
            int fld_length;
            bool bNewLine = false;
            bool bisPostBack = false;

            List<ChildControl> list = new List<ChildControl>();
            ChildControl fc;

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                fld_name = DataTableHelper.GetColumnValueToString(dt, i, "FLD_NAME");
                fld_value = DataTableHelper.GetColumnValueToString(dt, i, "FLD_VALUE");
                fld_width = DataTableHelper.GetColumnValueToInt(dt, i, "FLD_WIDTH");
                fld_length = DataTableHelper.GetColumnValueToInt(dt, i, "FLD_LEN");
                fld_cbo = DataTableHelper.GetColumnValueToInt(dt, i, "FLD_CBO");
                fld_cbo_list = DataTableHelper.GetColumnValueToString(dt, i, "FLD_CBO_LIST");
                fld_vlevels = DataTableHelper.GetColumnValueToInt(dt, i, "FLD_VLEVELS");
                fld_rdonly = DataTableHelper.GetColumnValueToInt(dt, i, "FLD_RDONLY");
                fld_group = DataTableHelper.GetColumnValueToInt(dt, i, "FLD_GROUP");
                fld_def_value = DataTableHelper.GetColumnValueToString(dt, i, "FLD_DEF_VALUE");
                fld_null = DataTableHelper.GetColumnValueToString(dt, i, "FLD_NULL");

                fld_format = DataTableHelper.GetColumnValueToString(dt, i, "FLD_FORMAT");
                fld_default = DataTableHelper.GetColumnValueToString(dt, i, "FLD_DEFAULT");
                fld_relation = DataTableHelper.GetColumnValueToString(dt, i, "FLD_RELATION");

                //HighByte用于显示列表控件,如果HighByte=0，使用LowByte值
                fld_cbo = (fld_cbo & 0xff);
                fld_width = (fld_width & 0xffff);

                if ((fld_vlevels & vLevel) != vLevel) continue;     //显示级别：bit 0在详细资料中显示 bit1在列表中显示，bit2在弹出中显示
                if ((fld_group & nMask) == 0) continue;             //BIT1代表个人字段  BIT2，代表公司字段
                if (fld_name.StartsWith("\\") == true)
                {
                    fld_name = fld_name.Replace("\\", "");
                    bNewLine = true;
                }
                else
                {
                    bNewLine = false;
                }
                
                if ((fld_name.StartsWith("#") == true) || (fld_name.StartsWith("-") == true))
                {                    
                    continue;
                }                

                ChildControlType fct = (ChildControlType)fld_cbo;
                switch (fct)
                {
                    case ChildControlType.CheckBox:
                        {
                            if (String.IsNullOrEmpty(fld_relation))
                            {
                                bisPostBack = false;
                            }
                            else
                            {
                                bisPostBack = true;
                            }
                        }
                        break;
                    case ChildControlType.DataTimePicker:
                        {
                            if (String.IsNullOrEmpty(fld_relation))
                            {
                                bisPostBack = false;
                            }
                            else
                            {
                                bisPostBack = true;
                            }
                        }
                        break;
                    case ChildControlType.DropDownList:
                        {
                            if (String.IsNullOrEmpty(fld_relation))
                            {
                                bisPostBack = false;
                            }
                            else
                            {
                                bisPostBack = true;
                            }
                        }
                        break;
                    case ChildControlType.DropDownListAndButton:
                        {
                            bisPostBack = true;
                        }
                        break;
                    case ChildControlType.RadioButton:
                        {
                            if (String.IsNullOrEmpty(fld_relation))
                            {
                                bisPostBack = false;
                            }
                            else
                            {
                                bisPostBack = true;
                            }
                        }
                        break;
                    case ChildControlType.RichTextBox:
                        {
                            if (String.IsNullOrEmpty(fld_relation))
                            {
                                bisPostBack = false;
                            }
                            else
                            {
                                bisPostBack = true;
                            }
                        }
                        break;
                    case ChildControlType.TextBox:
                        {
                            if (String.IsNullOrEmpty(fld_relation))
                            {
                                bisPostBack = false;
                            }
                            else
                            {
                                bisPostBack = true;
                            }
                        }
                        break;
                    case ChildControlType.TextBoxAndButton:
                        {
                            bisPostBack = true;
                        }
                        break;

                }
                fc = new ChildControl(fld_name.Trim(), fld_value.Trim(), fct, fld_width, fld_length, (fld_rdonly.Equals(1) ? true : false), null, fld_format, bNewLine, fld_default, bisPostBack, fld_relation);
                fc.DataSource = OnDataSourceBinding(fc, fld_cbo_list.Trim(), GetSelectKey(fld_cbo_list, fld_value.Trim()));
                list.Add(fc);
            }

            return list;
        }

        private static string GetSelectKey(string fld_cbo_list, string fld_value)
        {
            string strTemp = string.Empty;
            string sqlString = fld_cbo_list.Trim();

            if (sqlString.StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase) == true)
            {
                sqlString = sqlString.ToUpper();
                strTemp = StringHelper.Substring(sqlString, "SELECT", "FROM").Trim();
                if (strTemp == "*")
                {
                    strTemp = fld_value.Trim();
                }
                //将"SELECT x1 as A,x2 as B FROM"-> "A,B"
                else if (Regex.IsMatch(strTemp, @"\s+AS\s+\w+") == true)
                {
                    string strField_list = "";
                    MatchCollection mcTemp = Regex.Matches(strTemp, @"\s+AS\s+\w+");
                    foreach (Match mtMade in mcTemp)
                    {
                        if (strField_list.Length > 0) strField_list += ",";
                        strField_list += Regex.Replace(mtMade.Value, @"\s+AS\s+", "").Trim();
                    }
                    strTemp = strField_list;
                }
            }
            return strTemp;
        }

        public void SetDataSource(string name, Dictionary<string, string> dict)
        {
            ChildControl fc = _childControls.Find(delegate(ChildControl f) { if (f.Name.ToLower().Equals(name.ToLower())) return true; else return false; });
            if (fc != null)
            {
                fc.DataSource = dict;
            }
        }

        #region 控件数据绑定事件
        protected virtual Dictionary<string, string> OnDataSourceBinding(object sender, string dataSourceString, string dataSourceKey)
        {
            if (DataSourceBinding != null)
            {
                return DataSourceBinding(sender, dataSourceString, dataSourceKey);
            }
            else
            {
                return DoDataSourceBinding(sender, dataSourceString, dataSourceKey);
            }

        }

        public Dictionary<string, string> DoDataSourceBinding(object sender, string dataSourceString, string dataSourceKey)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (String.IsNullOrEmpty(dataSourceString))
            {
                return dict;
            }

            if (dataSourceString.StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase) == true)
            {
                return dict;
            }
            string[] strItems = dataSourceString.Split(',');
            for (int j = 0; j < strItems.Length; j++)
            {
                string[] strItems_sub = strItems[j].Split('-');
                if (strItems_sub.Length == 2)
                {
                    if (StringHelper.IsNumber(strItems_sub[0]) == true)
                    {
                        dict.Add(strItems_sub[0], strItems_sub[1]);
                    }
                }
                else
                {
                    break;
                }
            }


            if (dict.Count == strItems.Length)
            {
                return dict;
            }
            else
            {
                dict.Clear();
                for (int i = 0; i < strItems.Length; i++)
                {
                    dict.Add(i.ToString(), strItems[i].ToString());
                }
            }
            return dict;

        }

        public Dictionary<string, string> DoDataSourceBinding(object sender, DataTable dt, string dataSourceKey)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] keys = dataSourceKey.Split(',');
            if (keys.Length >= 2)
            {
                dict = DictionaryHelper.ListToDictionary(dt, keys[0], keys[1]);
            }
            else if (keys.Length == 1)
            {
                dict = DictionaryHelper.ListToDictionary(dt, string.Empty, keys[0]);
            }
            return dict;
        }

        protected virtual void OnRelation(object sender, string relationString)
        {
            if (Relation != null)
            {
                string value = StringHelper.Substring(relationString, "VALUE=", ";").Replace("VALUE=", "");
                string field = StringHelper.Substring(relationString, "FIELD=", ";").Replace("FIELD=", "");
                string type = StringHelper.Substring(relationString, "TYPE=", ";").Replace("TYPE=", "");

                string strTemp = string.Empty;

                if (Regex.IsMatch(value, @"\[\w+\]") == true)
                {
                    string strField_list = "";
                    MatchCollection mcTemp = Regex.Matches(value, @"\[\w+\]");
                    foreach (Match mtMade in mcTemp)
                    {
                        if (strField_list.Length > 0) strField_list += ",";
                        strField_list += mtMade.Value;
                    }
                    strTemp = strField_list;
                }

                string[] strArray = strTemp.Split(',');
                foreach (string s in strArray)
                {
                    value = value.Replace(s, GetChildControlData(s.Replace("[", "").Replace("]", "")));  

                }





                //this.Relation(fc, fc.Relation, fc.Relation);
            }
        }

        #endregion

        #region 控件触发事件
        /// <summary>
        /// Handles the CheckedChanged event of the rb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void rb_CheckedChanged(object sender, EventArgs e)
        {
            if (ChildControlChanged != null)
            {
                RadioButton rb = sender as RadioButton;
                if (rb != null)
                {
                    string[] rbNameAndKey = rb.ID.Replace(this._panel.ID.ToLower() + "_", "").Split('_');
                    if (rbNameAndKey.Length != 2)
                    {
                        return;
                    }
                    string name = rbNameAndKey[0];
                    string key = rbNameAndKey[1];

                    ChildControl childControl = this.ChildControls.Find(delegate(ChildControl cc) { if (cc.Name.ToUpper().Equals(name.ToUpper())) return true; else return false; });
                    this.ChildControlChanged(sender, childControl, childControl.Type, name.ToUpper(), key);
                }
            }
        }

        void cb_CheckedChanged(object sender, EventArgs e)
        {
            if (ChildControlChanged != null)
            {
                CheckBox cb = sender as CheckBox;
                if (cb != null)
                {
                    string[] cbNameAndKey = cb.ID.Replace(this._panel.ID.ToLower() + "_", "").Split('_');
                    if (cbNameAndKey.Length != 2)
                    {
                        return;
                    }
                    string name = cbNameAndKey[0];
                    string key = cbNameAndKey[1];

                    ChildControl childControl = this.ChildControls.Find(delegate(ChildControl cc) { if (cc.Name.ToUpper().Equals(name.ToUpper())) return true; else return false; });
                    this.ChildControlChanged(sender, childControl, childControl.Type, name.ToUpper(), key);
                }
            }
        }

        void btn_Click(object sender, EventArgs e)
        {
            if (ChildControlChanged != null)
            {
                Button btn = sender as Button;
                if (btn != null)
                {
                    string name = btn.ID.Replace(this._panel.ID.ToLower() + "_btn_", "");
                    string value = String.Empty;
                    ChildControl childControl = this.ChildControls.Find(delegate(ChildControl cc) { if (cc.Name.ToUpper().Equals(name.ToUpper())) return true; else return false; });
                    switch (childControl.Type)
                    {
                        case ChildControlType.TextBoxAndButton:
                            {
                                value = GetChildControlData(childControl);
                            }
                            break;
                        case ChildControlType.DropDownListAndButton:
                            {
                                value = GetChildControlData(childControl);
                            }
                            break;
                    }

                    this.ChildControlChanged(sender, childControl, childControl.Type, name.ToUpper(), value);
                }
            }
        }

        void ddl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ChildControlChanged != null)
            {
                DropDownList ddl = sender as DropDownList;
                if (ddl != null)
                {
                    string name = ddl.ID.Replace(this._panel.ID.ToLower() + "_", "");
                    ChildControl childControl = this.ChildControls.Find(delegate(ChildControl cc) { if (cc.Name.ToUpper().Equals(name.ToUpper())) return true; else return false; });
                    this.ChildControlChanged(sender, childControl, childControl.Type, name.ToUpper(), ddl.SelectedValue);
                }
            }
        }

        void txt_TextChanged(object sender, EventArgs e)
        {
            if (ChildControlChanged != null)
            {
                TextBox txt = sender as TextBox;
                if (txt != null)
                {
                    string name = txt.ID.Replace(this._panel.ID.ToLower() + "_", "");
                    ChildControl childControl = this.ChildControls.Find(delegate(ChildControl cc) { if (cc.Name.ToLower().Equals(name.ToLower())) return true; else return false; });
                    this.ChildControlChanged(sender, childControl, childControl.Type, name.ToUpper(), txt.Text);
                }
            }
        }
        #endregion
        #endregion
        
    }
}
