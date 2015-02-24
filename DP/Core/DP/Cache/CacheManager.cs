using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace DP.Cache
{
    public class CacheManager
    {
        private static ICacheStrategy cs;
        private static volatile CacheManager instance = null;
        private static object lockHelper = new object();

        /// <summary>
        /// Initializes the <see cref="CacheManager"/> class.
        /// </summary>
        static CacheManager()
        {

            string type = GetCacheStrategy();
            switch (type)
            {
                case "2":
                    {
                        cs = new EnyimMemCachedProvider();
                        cs.IsCompress = GetIsCompress();                        
                    }
                    break;
                default:
                    {
                        cs = new HttpRuntimeCacheProvider();
                        cs.IsCompress = GetIsCompress();
                    }
                    break;
            }

        }

        /// <summary>
        /// Gets the is compress.
        /// </summary>
        /// <returns></returns>
        private static bool GetIsCompress()
        {
            string IsCompress = "false";
            try
            {
                IsCompress = ConfigurationManager.AppSettings["IsCompress"];
            }
            catch (Exception ex)
            {
                IsCompress = "false";
            }
            return IsCompress == "true" ? true : false;
        }
        /// <summary>
        /// Gets the cache strategy.
        /// </summary>
        /// <returns></returns>
        private static string GetCacheStrategy()
        {
            string type = "";
            try
            {
                type = ConfigurationManager.AppSettings["CacheStrategy"];
            }
            catch (Exception ex)
            {

            }
            return type;
        }

        /// <summary>
        /// Gets the cache service.
        /// </summary>
        /// <returns></returns>
        public static CacheManager GetCacheService()
        {
            if (instance == null)
            {
                lock (lockHelper)
                {
                    if (instance == null)
                    {
                        instance = new CacheManager();
                    }
                }
            }
            return instance;
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public bool Add(string key, object obj)
        {
            bool rel = true;
            if (String.IsNullOrEmpty(key) || obj == null)
            {
                return false;
            }
            lock (lockHelper)
            {
                rel= cs.Add(key, obj);
            }
            return rel;
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public object Get(string key)
        {
            object obj = null;
            if (String.IsNullOrEmpty(key))
            {
                return obj;
            }
            lock (lockHelper)
            {
                obj = cs.Get(key);
            }
            return obj;
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            T obj = default(T);
            if (String.IsNullOrEmpty(key))
            {
                return obj;
            }
            lock (lockHelper)
            {
                obj = cs.Get<T>(key);
            }
            return obj;
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(string key)
        {
            lock (lockHelper)
            {
                cs.Remove(key);
            }
        }
        
    }
}
