using System;
using System.Collections.Generic;
using System.Text;
using DP.Data.Common;
using System.Reflection;

namespace DP.Data.OracleClient
{
    public class OracleDbColumnInfo : DbColumnInfo
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


        public OracleDbColumnInfo(PropertyInfo prop)
            : base(prop)
        {
            OracleColumnAttribute attr = Attribute.GetCustomAttribute(prop, typeof(OracleColumnAttribute), true) as OracleColumnAttribute;
            if (attr == null)
            {
                attr = new OracleColumnAttribute();
            }
            _SequenceName = attr.SequenceName;
            _PrimaryKey = attr.PrimaryKey;
            _ColumnName = String.IsNullOrEmpty(attr.ColumnName) ? prop.Name : attr.ColumnName;
            _ColumnDescription = attr.ColumnDescription;
            _IncrementPrimaryKey = attr.IncrementPrimaryKey;
            _Ignore = attr.Ignore;
        }


    }
}
