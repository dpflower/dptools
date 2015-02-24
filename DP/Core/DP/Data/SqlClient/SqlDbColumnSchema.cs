using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using DP.Data.Common;
using DP.Data.SqlClient.Mapping;

namespace DP.Data.SqlClient
{
    public class SqlDbColumnSchema: DbColumnSchema
    {
        public SqlDbColumnSchema(PropertyInfo prop)
            : base(prop)
        {
            SqlColumnAttribute attr = Attribute.GetCustomAttribute(prop, typeof(SqlColumnAttribute), true) as SqlColumnAttribute;
            if (attr == null)
            {
                attr = new SqlColumnAttribute();
            }
            _isPrimaryKey = attr.IsPrimaryKey;
            _ColumnName = String.IsNullOrEmpty(attr.ColumnName) ? prop.Name : attr.ColumnName;
            _ColumnDescription = attr.ColumnDescription;
            _isDbGenerated = attr.IsDbGenerated;
            _isIgnore = attr.IsIgnore;
        }
    }
}
