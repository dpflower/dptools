using System;
using System.Collections.Generic;
using System.Text;
using DP.Data.Common;
using System.Reflection;

namespace DP.Data.SqlClient
{
    public class SqlDbColumnInfo : DbColumnInfo
    {
        public SqlDbColumnInfo(PropertyInfo prop)
            : base(prop)
        {
            SqlColumnAttribute attr = Attribute.GetCustomAttribute(prop, typeof(SqlColumnAttribute), true) as SqlColumnAttribute;
            if (attr == null)
            {
                attr = new SqlColumnAttribute();
            }
            _PrimaryKey = attr.PrimaryKey;
            _ColumnName = String.IsNullOrEmpty(attr.ColumnName) ? prop.Name : attr.ColumnName;
            _ColumnDescription = attr.ColumnDescription;
            _IncrementPrimaryKey = attr.IncrementPrimaryKey;
            _Ignore = attr.Ignore;
        }
    }
}
