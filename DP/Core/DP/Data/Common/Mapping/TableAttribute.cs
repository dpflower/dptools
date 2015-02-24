using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DP.Data.Common.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public abstract class TableAttribute : Attribute
    {
        /// <summary>
        /// 初始化 <see cref="T:System.Attribute"/> 类的新实例。
        /// </summary>
        /// <remarks></remarks>
        public TableAttribute()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        string _tableName = "";
        /// <summary>
        /// 
        /// </summary>
        string _connectionStringKey = "";
        /// <summary>
        /// 
        /// </summary>
        TableType _tableOrView = TableType.Table;


        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        /// <remarks></remarks>
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        /// <summary>
        /// Gets or sets the table or view.
        /// </summary>
        /// <value>The table or view.</value>
        /// <remarks></remarks>
        public TableType TableOrView
        {
            get { return _tableOrView; }
            set { _tableOrView = value; }
        }

        /// <summary>
        /// Gets or sets the connection string key.
        /// </summary>
        /// <value>The connection string key.</value>
        /// <remarks></remarks>
        public string ConnectionStringKey
        {
            get { return _connectionStringKey; }
            set { _connectionStringKey = value; }
        }
    }
}
