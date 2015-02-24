using System;
using System.Collections.Generic;
using System.Text;
using DP.Data.Common;

namespace DP.Data.OracleClient
{
    public class OracleColumnAttribute : ColumnAttribute
    {

        string _sequenceName = string.Empty;

        public string SequenceName
        {
            get { return _sequenceName; }
            set { _sequenceName = value; }
        }

        public OracleColumnAttribute()
        {

        }





    }
}
