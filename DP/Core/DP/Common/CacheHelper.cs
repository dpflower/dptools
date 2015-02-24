using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using Enyim.Caching;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

namespace DP.Common
{
    public abstract class CacheHelper
    {
        #region 缓存操作
        /// <summary>
        /// 设置 Session
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="obj">The obj.</param>
        public static void SetSession(string key, object obj)
        {
            if (HttpContext.Current == null)
            {
                return;
            }
            if (HttpContext.Current.Session == null)
            {
                return;
            }     
            HttpContext.Current.Session[key] = obj;
        }

        /// <summary>
        /// 清除 Session 数据
        /// </summary>
        /// <param name="key">The key.</param>
        public static void ClearSession(string key)
        {
            if (HttpContext.Current == null)
            {
                return;
            }
            if (HttpContext.Current.Session == null)
            {
                return;
            }    
            HttpContext.Current.Session.Remove(key);
        }

        /// <summary>
        /// 获取 Session 数据
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static object GetSession(string key)
        {
            if (HttpContext.Current == null)
            {
                return null;
            }
            if (HttpContext.Current.Session == null)
            {
                return null;
            }    
            return HttpContext.Current.Session[key];
        }

        /// <summary>
        /// 设置 Cache 缓存
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="obj">The obj.</param>
        public static void SetCache(string key, object obj)
        {
            SetCache(key, obj, CacheType.HttpRuntimeCache);            
        }
        /// <summary>
        /// Sets the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="cacheType">Type of the cache.</param>
        public static void SetCache(string key, object obj, CacheType cacheType)
        {
            switch (cacheType)
            {
                case CacheType.HttpRuntimeCache:
                    {
                        HttpRuntime.Cache[key] = obj;
                    }
                    break;
                case CacheType.MemCached:
                    {
                        MemcachedClient client = new MemcachedClient("enyim.com/memcached");
                        client.Store(Enyim.Caching.Memcached.StoreMode.Set, key, obj);
                        //using (MemoryStream ms = new MemoryStream())
                        //{
                        //    new BinaryFormatter().Serialize(ms, obj);
                        //    byte[] data = ms.GetBuffer();
                        //    DateTime dtBegin = DateTime.Now;
                        //    byte[] outData = CompressionHelper.Compress(data);
                        //    DateTime dtEnd = DateTime.Now;
                        //    client.Store(Enyim.Caching.Memcached.StoreMode.Set, key, outData);
                        //    LogHelper.WriteLog("MemcachedClient_Compression", String.Format("data.Leight={0};outData.Leight={1};Pre={3};Diff={2}", data.Length, outData.Length, dtEnd.Subtract(dtBegin).ToString(), (decimal)outData.Length/(decimal)data.Length));
                        //}
                    }
                    break;
                default:
                    {

                    }
                    break;
            }
        }

        /// <summary>
        /// 清空 Cache 缓存
        /// </summary>
        /// <param name="key">The key.</param>
        public static void ClearCache(string key)
        {
            ClearCache(key, CacheType.HttpRuntimeCache);
        }
        /// <summary>
        /// 清空 Cache 缓存
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cacheType">Type of the cache.</param>
        public static void ClearCache(string key, CacheType cacheType)
        {
            switch (cacheType)
            {
                case CacheType.HttpRuntimeCache:
                    {
                        HttpRuntime.Cache.Remove(key);
                    }
                    break;
                case CacheType.MemCached:
                    {
                        MemcachedClient client = new MemcachedClient("enyim.com/memcached");
                        client.Remove(key);
                    }
                    break;
                default:
                    {

                    }
                    break;
            }
        }

        /// <summary>
        /// 获取 Cache 缓存 数据
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static object GetCache(string key)
        {
            return GetCache(key, CacheType.HttpRuntimeCache);
        }
        /// <summary>
        /// 获取 Cache 缓存 数据
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cacheType">Type of the cache.</param>
        /// <returns></returns>
        public static object GetCache(string key, CacheType cacheType)
        {
            object obj = null;
            switch (cacheType)
            {
                case CacheType.HttpRuntimeCache:
                    {
                        obj = HttpRuntime.Cache[key];
                    }
                    break;
                case CacheType.MemCached:
                    {
                        MemcachedClient client = new MemcachedClient("enyim.com/memcached");
                        obj = client.Get(key);
                        //byte[] data = client.Get<byte[]>(key);
                        //DateTime dtBegin = DateTime.Now;
                        //byte[] outData = CompressionHelper.Decompress(data);
                        //DateTime dtEnd = DateTime.Now;
                        //using (MemoryStream ms = new MemoryStream(outData))
                        //{
                        //    obj = new BinaryFormatter().Deserialize(ms);
                        //} 
                        //LogHelper.WriteLog("MemcachedClient_Compression", String.Format("Diff={0}", dtEnd.Subtract(dtBegin).ToString()));
                       
                    }
                    break;
                default:
                    {

                    }
                    break;
            }
            return obj;
        }

        /// <summary>
        /// 获取 Cache 缓存 数据
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static T GetCache<T>(string key)
        {
            return GetCache<T>(key, CacheType.HttpRuntimeCache);
        }
        /// <summary>
        /// 获取 Cache 缓存 数据
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cacheType">Type of the cache.</param>
        /// <returns></returns>
        public static T GetCache<T>(string key, CacheType cacheType)
        {
            object obj = GetCache(key, cacheType);
            if (obj is T)
            {
                return (T)obj;
            }
            return Activator.CreateInstance<T>();
            
        }

        /// <summary>
        /// 设置 Page.Items 缓存
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="key">键</param>
        /// <param name="obj">值</param>
        public static void SetPageItem(Page page, string key, object obj)
        {
            if (page == null)
            {
                return;
            }
            page.Items[key] = obj;
        }

        /// <summary>
        /// 清空 Page.Items 缓存
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="key">键</param>
        public static void ClearPageItem(Page page, string key)
        {
            if (page == null)
            {
                return;
            }
            page.Items.Remove(key);
        }

        /// <summary>
        /// 获取 Page.Items 缓存 数据
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static object GetPageItem(Page page, string key)
        {
            if (page == null)
            {
                return null;
            }
            return page.Items[key];
        }


        #endregion
    }
}
