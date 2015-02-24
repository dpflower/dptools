using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DP.Data.Common.Mapping;

namespace DP.Data.OracleClient.Mapping
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
