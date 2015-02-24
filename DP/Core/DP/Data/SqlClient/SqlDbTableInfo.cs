using System;
using System.Collections.Generic;
using System.Text;
using DP.Data.Common;
using System.Configuration;

namespace DP.Data.SqlClient
{
    public class SqlDbTableInfo : DbTableInfo
    {
        protected SqlServerVerson _SqlVerson;

        public SqlServerVerson SqlVerson
        {
            get { return _SqlVerson; }
        }

        public SqlDbTableInfo(Type type)
            : base(type)
        {
            SqlTableAttribute attr = Attribute.GetCustomAttribute(type, typeof(SqlTableAttribute), true) as SqlTableAttribute;
            if (attr == null)
            {
                attr = new SqlTableAttribute();
            }
            _SqlVerson = attr.SqlVerson;
            _ClassName = type.Name;
            _TableName = String.IsNullOrEmpty(attr.TableName) ? type.Name : attr.TableName;
            try
            {
                _ConnectionString = ConfigurationManager.ConnectionStrings[attr.ConnectionStringKey].ConnectionString;
            }
            catch
            {

            }
            _TableOrView = attr.TableOrView;
        }

    }
}
