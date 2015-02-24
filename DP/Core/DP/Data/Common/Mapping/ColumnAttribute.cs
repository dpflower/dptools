using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DP.Data.Common.Mapping
{
    public abstract class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 初始化 <see cref="T:System.Attribute"/> 类的新实例。
        /// </summary>
        /// <remarks></remarks>
        public ColumnAttribute()
        {

        }

        /// <summary>
        /// 列名称
        /// </summary>
        string _columnName = string.Empty;
        /// <summary>
        /// 列描述
        /// </summary>
        string _columnDescription = string.Empty;
        /// <summary>
        /// 忽略
        /// </summary>
        bool _isIgnore = false;
        /// <summary>
        /// 整个主键或部分主键的列。
        /// </summary>
        bool _isPrimaryKey = false;
        /// <summary>
        /// 自动生成的值
        /// </summary>
        bool _isDbGenerated = false;


        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        /// <remarks></remarks>
        public string ColumnName
        {
            get { return _columnName; }
            set { _columnName = value; }
        }
        /// <summary>
        /// Gets or sets the column description.
        /// </summary>
        /// <value>The column description.</value>
        /// <remarks></remarks>
        public string ColumnDescription
        {
            get { return _columnDescription; }
            set { _columnDescription = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is ignore.
        /// </summary>
        /// <value><c>true</c> if this instance is ignore; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool IsIgnore
        {
            get { return _isIgnore; }
            set { _isIgnore = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is primary key.
        /// </summary>
        /// <value><c>true</c> if this instance is primary key; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool IsPrimaryKey
        {
            get { return _isPrimaryKey; }
            set { _isPrimaryKey = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is db generated.
        /// </summary>
        /// <value><c>true</c> if this instance is db generated; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool IsDbGenerated
        {
            get { return _isDbGenerated; }
            set { _isDbGenerated = value; }
        }
    }
}
