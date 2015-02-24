using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DP.Data.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class DbTableSchema
    {
        public DbTableSchema(Type type)
        {
            //TableAttribute attr = Attribute.GetCustomAttribute(type, typeof(TableAttribute), true) as TableAttribute;
            //if (attr == null)
            //{
            //    attr = new TableAttribute();
            //}

        }

        #region MyRegion
        /// <summary>
        /// 
        /// </summary>
        protected string _TableName;
        /// <summary>
        /// 
        /// </summary>
        protected string _ClassName;
        /// <summary>
        /// 
        /// </summary>
        protected string _ConnectionString;
        /// <summary>
        /// 
        /// </summary>
        protected TableType _TableOrView; 
        #endregion

        #region MyRegion
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <remarks></remarks>
        public string TableName
        {
            get { return _TableName; }
        }

        /// <summary>
        /// Gets the name of the class.
        /// </summary>
        /// <remarks></remarks>
        public string ClassName
        {
            get { return _ClassName; }
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <remarks></remarks>
        public string ConnectionString
        {
            get { return _ConnectionString; }
        }

        /// <summary>
        /// Gets the table or view.
        /// </summary>
        /// <remarks></remarks>
        public TableType TableOrView
        {
            get { return _TableOrView; }
        } 
        #endregion

        
    }
}
