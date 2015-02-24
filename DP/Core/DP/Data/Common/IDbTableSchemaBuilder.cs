using System;
namespace DP.Data.Common
{
    interface IDbTableSchemaBuilder
    {
        DbTableSchema Generate(Type type);
    }
}
