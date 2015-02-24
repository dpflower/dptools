using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DP.Common;

namespace DP.Cache
{
    public class HttpRuntimeCacheProvider : ICacheStrategy
    {
        private bool _IsCompress = false;

        public bool IsCompress
        {
            get { return _IsCompress; }
            set { _IsCompress = value; }
        }

        public bool Add(string key, object obj)
        {
            bool rel = true;
            try
            {
                if (_IsCompress)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        new BinaryFormatter().Serialize(ms, obj);
                        byte[] data = ms.GetBuffer();
                        //DateTime dtBegin = DateTime.Now;
                        data = CompressionHelper.Compress(data);
                        //DateTime dtEnd = DateTime.Now;
                        HttpRuntime.Cache[key] = data;
                        //LogHelper.WriteLog("MemcachedClient_Compression", String.Format("data.Leight={0};outData.Leight={1};Pre={3};Diff={2}", data.Length, outData.Length, dtEnd.Subtract(dtBegin).ToString(), (decimal)outData.Length / (decimal)data.Length));
                    }
                }
                else
                {
                    HttpRuntime.Cache[key] = obj;
                }                
            }
            catch(Exception ex)
            {
                rel = false;
            }
            return rel;
        }

        public object Get(string key)
        {
            object obj = null;
            if (_IsCompress)
            {
                byte[] data = HttpRuntime.Cache.Get(key) as byte[];
                //DateTime dtBegin = DateTime.Now;
                data = CompressionHelper.Decompress(data);
                //DateTime dtEnd = DateTime.Now;
                using (MemoryStream ms = new MemoryStream(data))
                {
                    obj = new BinaryFormatter().Deserialize(ms);
                }
                //LogHelper.WriteLog("MemcachedClient_Compression", String.Format("Diff={0}", dtEnd.Subtract(dtBegin).ToString()));
            }
            else
            {
                obj = HttpRuntime.Cache.Get(key);
            }
            return obj;
        }

        public T Get<T>(string key)
        {
            object obj  = Get(key);            
            if (obj is T)
            {
                return (T)obj;
            }
            return Activator.CreateInstance<T>();
        }

        public void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }

    }
}
