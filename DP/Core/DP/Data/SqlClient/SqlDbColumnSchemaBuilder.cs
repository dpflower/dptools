using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DP.Data.Common;
using System.Web;
using System.Reflection;
using DP.Common;

namespace DP.Data.SqlClient
{
    public class SqlDbColumnSchemaBuilder
    {
        private static SqlDbColumnSchemaBuilder instance; 
        private static object _lock = new object();
        private readonly string SqlDbColumnSchemaBuilderKey = "DP.Data.SqlClient.SqlDbColumnSchemaBuilder_";

        private SqlDbColumnSchemaBuilder()
        {
        
        }

        public static SqlDbColumnSchemaBuilder GetInstance()
        {
            if (instance == null)
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new SqlDbColumnSchemaBuilder();
                    }
                }
            }
            return instance;
        }

        public List<DbColumnSchema> Generate(Type type)
        {
            string key = SqlDbColumnSchemaBuilderKey + type.FullName;
            List<DbColumnSchema> list = new List<DbColumnSchema>();
            list = CacheHelper.GetCache(key) as List<DbColumnSchema>;
            //list = HttpRuntime.Cache.Get(key) as List<DbColumnSchema>;
            if (list == null)
            {
                list = new List<DbColumnSchema>();
                PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach(PropertyInfo p in properties)
                {
                    list.Add(new SqlDbColumnSchema(p));
                }
                CacheHelper.SetCache(key, list);
                //HttpRuntime.Cache.Insert(key, list);
            }
            return list;
        }
    
    }
}
