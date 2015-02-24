using System;
using System.Collections.Generic;
using System.Text;
using DP.Data.Common;

namespace DP.Data.SqlClient
{
    public class SqlTableAttribute : TableAttribute
    {
        public SqlTableAttribute()
        {

        }

        SqlServerVerson _sqlVerson = SqlServerVerson.Sql2000;

        public SqlServerVerson SqlVerson
        {
            get { return _sqlVerson; }
            set { _sqlVerson = value; }
        }

    }
}
