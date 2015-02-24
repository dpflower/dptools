using System;
using System.Collections.Generic;
using System.Text;

namespace DP.Data.Common
{
    public enum TableType
    {
        Table = 0,
        View = 1
    }

    public enum SqlServerVerson
    {
        Sql2000 = 0,
        Sql2005 = 1,
        Sql2008 = 2
    }

    public enum OracleVerson
    {
        Oracle_9i = 0,
        Oracle_10g = 1,
        Oracle_11g = 2
    }

    public enum PrimaryKeyType
    {
        Auto = 0,
        Sequence = 1,
        Guid = 2,
        Other = 9
    }

    public enum QueryOperator
    {
        Equal = 0,
        NotEqual = 1,
        Like = 2,
        Greater = 3,
        GreaterOrEqual = 4,
        Less  = 5,
        LessOrEqual = 6,
        IsNull = 7,
        IsNotNull = 8,
        In = 9,
        LikeStart = 10
    }

    public enum OrderDirection
    {
        ASC = 0,
        DESC = 1
    }

    public enum PopulateMode
    {
        Emit = 0,
        Reflection = 1
    }


}
