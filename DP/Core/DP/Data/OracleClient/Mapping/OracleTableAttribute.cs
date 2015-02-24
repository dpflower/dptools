using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DP.Data.Common.Mapping;
using DP.Data.Common;

namespace DP.Data.OracleClient.Mapping
{
    public class OracleTableAttribute : TableAttribute
    {
        public OracleTableAttribute()
        {

        }

        OracleVerson _oracleVerson = OracleVerson.Oracle_10g;

        public OracleVerson OracleVerson
        {
            get { return _oracleVerson; }
            set { _oracleVerson = value; }
        }
    }
}
