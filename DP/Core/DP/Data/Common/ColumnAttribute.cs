using System;
using System.Collections.Generic;
using System.Text;

namespace DP.Data.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class ColumnAttribute : Attribute
    {

        /// <summary>
        /// 初始化 <see cref="T:System.Attribute"/> 类的新实例。
        /// </summary>
        /// <remarks></remarks>
        public ColumnAttribute()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        string _columnName = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        string _columnDescription = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        bool _ignore = false;
        /// <summary>
        /// 
        /// </summary>
        bool _primaryKey = false;
        /// <summary>
        /// 
        /// </summary>
        bool _incrementPrimaryKey = false;


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
        /// Gets or sets a value indicating whether this <see cref="ColumnAttribute"/> is ignore.
        /// </summary>
        /// <value><c>true</c> if ignore; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool Ignore
        {
            get { return _ignore; }
            set { _ignore = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [primary key].
        /// </summary>
        /// <value><c>true</c> if [primary key]; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool PrimaryKey
        {
            get { return _primaryKey; }
            set { _primaryKey = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [increment primary key].
        /// </summary>
        /// <value><c>true</c> if [increment primary key]; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool IncrementPrimaryKey
        {
            get { return _incrementPrimaryKey; }
            set { _incrementPrimaryKey = value; }
        }
    }
}
