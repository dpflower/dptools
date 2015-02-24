using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DP.Data.Common;
using System.Web;
using DP.Common;

namespace DP.Data.SqlClient
{
    public class SqlDbTableSchemaBuilder
    {
        private static SqlDbTableSchemaBuilder instance; 
        private static object _lock = new object();
        private readonly string SqlDbTableSchemaBuilderKey = "DP.Data.SqlClient.SqlDbTableSchemaBuilder_";

        private SqlDbTableSchemaBuilder()
        {
        
        }

        public static SqlDbTableSchemaBuilder GetInstance()
        {
            if (instance == null)
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new SqlDbTableSchemaBuilder();
                    }
                }
            }
            return instance;
        }

        public DbTableSchema Generate(Type type)
        {
            string key = SqlDbTableSchemaBuilderKey + type.FullName;
            DbTableSchema dbTableSchema = null;
            //dbTableSchema = HttpRuntime.Cache.Get(key) as DbTableSchema;
            dbTableSchema = CacheHelper.GetCache(key) as DbTableSchema;
            if (dbTableSchema == null)
            {
                dbTableSchema = new SqlDbTableSchema(type);
                //HttpRuntime.Cache.Insert(key, dbTableSchema);
                CacheHelper.SetCache(key, dbTableSchema);
            }
            return dbTableSchema;
        }
    }
}
