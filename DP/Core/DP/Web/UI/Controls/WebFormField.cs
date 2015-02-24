using System;
using System.Collections.Generic;
using System.Text;

namespace DP.Web.UI.Controls
{
    [Serializable]
    public class WebFormField
    {
        #region 变量
        int _index = 0;
        string _description = string.Empty;
        string _name = string.Empty;
        string _cssClass = string.Empty;
        int _colSpan = 1;
        int _width = 80;
        int _maxLength = 0;
        bool _isEnabled = true;
        bool _isPostBack = false;
        bool _isBlankRow = false;
        bool _exclusiveRow = false;
        string _fromat = string.Empty;
        string _defaultValue = string.Empty;
        string _relationFields = string.Empty;
        string _commandText = string.Empty;
        string _dataSourceInfo = string.Empty;
        bool _isPrimaryKey = false;
        WebFormField _eventSource = null;
        string _eventSourceValue = string.Empty;

   

        DataSourceType _dataSourceType = DataSourceType.String;
        WebFormFieldType _type = WebFormFieldType.TextBox;
        
        List<FieldDataOptions> _dataSource = new List<FieldDataOptions>();


        #endregion

        #region 属性
        /// <summary>
        /// 顺序索引
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }
        /// <summary>
        /// 描述
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        /// <summary>
        /// 名称
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// 样式
        /// Gets or sets the style.
        /// </summary>
        /// <value>
        /// The style.
        /// </value>
        public string CssClass
        {
            get { return _cssClass; }
            set { _cssClass = value; }
        }
        /// <summary>
        /// 列数
        /// Gets or sets the col span.
        /// </summary>
        /// <value>
        /// The col span.
        /// </value>
        public int ColSpan
        {
            get { return _colSpan; }
            set { _colSpan = value; }
        }
        /// <summary>
        /// 宽度
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        /// <summary>
        /// 最大长度
        /// Gets or sets the length of the max.
        /// </summary>
        /// <value>
        /// The length of the max.
        /// </value>
        public int MaxLength
        {
            get { return _maxLength; }
            set { _maxLength = value; }
        }
        /// <summary>
        /// 是否起用
        /// Gets or sets a value indicating whether this <see cref="WebFormField"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }
        /// <summary>
        /// 是否回传
        /// Gets or sets a value indicating whether this instance is post back.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is post back; otherwise, <c>false</c>.
        /// </value>
        public bool IsPostBack
        {
            get { return _isPostBack; }
            set { _isPostBack = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is primary key.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is primary key; otherwise, <c>false</c>.
        /// </value>
        public bool IsPrimaryKey
        {
            get { return _isPrimaryKey; }
            set { _isPrimaryKey = value; }
        }
        /// <summary>
        /// 下接列表是否插入空行
        /// Gets or sets a value indicating whether this instance is blank row.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is blank row; otherwise, <c>false</c>.
        /// </value>
        public bool IsBlankRow
        {
            get { return _isBlankRow; }
            set { _isBlankRow = value; }
        }
        /// <summary>
        /// 独占行
        /// Gets or sets a value indicating whether [exclusive row].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [exclusive row]; otherwise, <c>false</c>.
        /// </value>
        public bool ExclusiveRow
        {
            get { return _exclusiveRow; }
            set { _exclusiveRow = value; }
        }
        /// <summary>
        /// 格式化
        /// Gets or sets the fromat.
        /// </summary>
        /// <value>
        /// The fromat.
        /// </value>
        public string Fromat
        {
            get { return _fromat; }
            set { _fromat = value; }
        }
        /// <summary>
        /// 默认值
        /// Gets or sets the default value.
        /// </summary>
        /// <value>
        /// The default value.
        /// </value>
        public string DefaultValue
        {
            get { return _defaultValue; }
            set { _defaultValue = value; }
        }
        /// <summary>
        /// 关联字段名
        /// Gets or sets the relation.
        /// </summary>
        /// <value>
        /// The relation.
        /// </value>
        public string RelationFields
        {
            get { return _relationFields; }
            set { _relationFields = value; }
        }
        /// <summary>
        /// 命令按钮显示文字
        /// Gets or sets the command text.
        /// </summary>
        /// <value>
        /// The command text.
        /// </value>
        public string CommandText
        {
            get { return _commandText; }
            set { _commandText = value; }
        }
        /// <summary>
        /// 数据源ID
        /// Gets or sets the data source id.
        /// </summary>
        /// <value>
        /// The data source id.
        /// </value>
        public string DataSourceInfo
        {
            get { return _dataSourceInfo; }
            set { _dataSourceInfo = value; }
        }
        /// <summary>
        /// 数据源类型
        /// Gets or sets the type of the data source.
        /// </summary>
        /// <value>
        /// The type of the data source.
        /// </value>
        public DataSourceType DataSourceType
        {
            get { return _dataSourceType; }
            set { _dataSourceType = value; }
        }
        /// <summary>
        /// 控件类型
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public WebFormFieldType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        /// <value>
        /// The data source.
        /// </value>
        public List<FieldDataOptions> DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }
        /// <summary>
        /// Gets or sets the event object.
        /// </summary>
        /// <value>
        /// The event object.
        /// </value>
        public WebFormField EventSource
        {
            get { return _eventSource; }
            set { _eventSource = value; }
        }
        /// <summary>
        /// Gets or sets the event source value.
        /// </summary>
        /// <value>
        /// The event source value.
        /// </value>
        public string EventSourceValue
        {
            get { return _eventSourceValue; }
            set { _eventSourceValue = value; }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// Initializes a new instance of the <see cref="WebFormField"/> class.
        /// </summary>
        public WebFormField()
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
        public WebFormField(string description, string name, WebFormFieldType type, int width, int maxLength, bool enabled, List<FieldDataOptions> dataSource, string fromat)
        {
            _description = description;
            _name = name;
            _type = type;
            _width = width;
            _maxLength = maxLength;
            _isEnabled = enabled;
            _dataSource = dataSource;
            _fromat = fromat;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WebFormField"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="width">The width.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="fromat">The fromat.</param>
        /// <param name="defaultValue">The default value.</param>
        public WebFormField(string description, string name, WebFormFieldType type, int width, int maxLength, bool enabled, List<FieldDataOptions> dataSource, string fromat, string defaultValue)
        {
            _description = description;
            _name = name;
            _type = type;
            _width = width;
            _maxLength = maxLength;
            _isEnabled = enabled;
            _dataSource = dataSource;
            _fromat = fromat;
            _defaultValue = defaultValue;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WebFormField"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="width">The width.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="fromat">The fromat.</param>
        /// <param name="exclusiveRow">if set to <c>true</c> [exclusive row].</param>
        public WebFormField(string description, string name, WebFormFieldType type, int width, int maxLength, bool enabled, List<FieldDataOptions> dataSource, string fromat, bool exclusiveRow)
        {
            _description = description;
            _name = name;
            _type = type;
            _width = width;
            _maxLength = maxLength;
            _isEnabled = enabled;
            _dataSource = dataSource;
            _fromat = fromat;
            _exclusiveRow = exclusiveRow;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WebFormField"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="width">The width.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="fromat">The fromat.</param>
        /// <param name="exclusiveRow">if set to <c>true</c> [exclusive row].</param>
        /// <param name="defaultValue">The default value.</param>
        public WebFormField(string description, string name, WebFormFieldType type, int width, int maxLength, bool enabled, List<FieldDataOptions> dataSource, string fromat, bool exclusiveRow, string defaultValue)
        {
            _description = description;
            _name = name;
            _type = type;
            _width = width;
            _maxLength = maxLength;
            _isEnabled = enabled;
            _dataSource = dataSource;
            _fromat = fromat;
            _exclusiveRow = exclusiveRow;
            _defaultValue = defaultValue;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WebFormField"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="width">The width.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="fromat">The fromat.</param>
        /// <param name="exclusiveRow">if set to <c>true</c> [exclusive row].</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="isPostBack">if set to <c>true</c> [is post back].</param>
        public WebFormField(string description, string name, WebFormFieldType type, int width, int maxLength, bool enabled, List<FieldDataOptions> dataSource, string fromat, bool exclusiveRow, string defaultValue, bool isPostBack)
        {
            _description = description;
            _name = name;
            _type = type;
            _width = width;
            _maxLength = maxLength;
            _isEnabled = enabled;
            _dataSource = dataSource;
            _fromat = fromat;
            _exclusiveRow = exclusiveRow;
            _defaultValue = defaultValue;
            _isPostBack = isPostBack;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WebFormField"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="width">The width.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="fromat">The fromat.</param>
        /// <param name="exclusiveRow">if set to <c>true</c> [exclusive row].</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="isPostBack">if set to <c>true</c> [is post back].</param>
        /// <param name="relation">The relation.</param>
        public WebFormField(string description, string name, WebFormFieldType type, int width, int maxLength, bool enabled, List<FieldDataOptions> dataSource, string fromat, bool exclusiveRow, string defaultValue, bool isPostBack, string relation)
        {
            _description = description;
            _name = name;
            _type = type;
            _width = width;
            _maxLength = maxLength;
            _isEnabled = enabled;
            _dataSource = dataSource;
            _fromat = fromat;
            _exclusiveRow = exclusiveRow;
            _defaultValue = defaultValue;
            _isPostBack = isPostBack;
            _relationFields = relation;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WebFormField"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        public WebFormField(string description, string name, WebFormFieldType type)
        {
            _description = description;
            _name = name;
            _type = type;
        }

        #endregion


    }

}
