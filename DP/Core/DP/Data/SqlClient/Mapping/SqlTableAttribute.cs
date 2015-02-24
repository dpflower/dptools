using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DP.Data.Common;
using DP.Data.Common.Mapping;

namespace DP.Data.SqlClient.Mapping
{
    public class SqlTableAttribute : TableAttribute
    {
        public SqlTableAttribute()
        {

        }

        SqlServerVerson _sqlVerson = SqlServerVerson.Sql2005;

        public SqlServerVerson SqlVerson
        {
            get { return _sqlVerson; }
            set { _sqlVerson = value; }
        }

    }
}
