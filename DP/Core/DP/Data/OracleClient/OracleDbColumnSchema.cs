using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DP.Data.OracleClient.Mapping;
using System.Reflection;
using DP.Data.Common;

namespace DP.Data.OracleClient
{
    public class OracleDbColumnSchema: DbColumnSchema
    {
        protected string _SequenceName;

        /// <summary>
        /// 序列
        /// Gets the name of the sequence.
        /// </summary>
        /// <value>
        /// The name of the sequence.
        /// </value>
        public string SequenceName
        {
            get { return _SequenceName; }
        }


        public OracleDbColumnSchema(PropertyInfo prop)
            : base(prop)
        {
            OracleColumnAttribute attr = Attribute.GetCustomAttribute(prop, typeof(OracleColumnAttribute), true) as OracleColumnAttribute;
            if (attr == null)
            {
                attr = new OracleColumnAttribute();
            }
            _SequenceName = attr.SequenceName;
            _isPrimaryKey = attr.IsPrimaryKey;
            _ColumnName = String.IsNullOrEmpty(attr.ColumnName) ? prop.Name : attr.ColumnName;
            _ColumnDescription = attr.ColumnDescription;
            _isDbGenerated = attr.IsDbGenerated;
            _isIgnore = attr.IsIgnore;
        }


    }
}
