using System;
using System.Collections.Generic;
using System.Text;
using Enyim.Caching;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DP.Common;

namespace DP.Cache
{
    public class EnyimMemCachedProvider : ICacheStrategy
    {
        MemcachedClient client = new MemcachedClient("enyim.com/memcached");
        private bool _IsCompress = false;

        public bool IsCompress
        {
            get { return _IsCompress; }
            set { _IsCompress = value; }
        }

        public bool Add(string key, object obj)
        {
            bool rel = true;
            if (_IsCompress)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    new BinaryFormatter().Serialize(ms, obj);
                    byte[] data = ms.GetBuffer();
                    //DateTime dtBegin = DateTime.Now;
                    data = CompressionHelper.Compress(data);
                    //DateTime dtEnd = DateTime.Now;
                    rel = client.Store(Enyim.Caching.Memcached.StoreMode.Set, key, data);
                    //LogHelper.WriteLog("MemcachedClient_Compression", String.Format("data.Leight={0};outData.Leight={1};Pre={3};Diff={2}", data.Length, outData.Length, dtEnd.Subtract(dtBegin).ToString(), (decimal)outData.Length / (decimal)data.Length));
                }
            }
            else
            {
                rel = client.Store(Enyim.Caching.Memcached.StoreMode.Set, key, obj);
            }
            return rel;
        }

        public object Get(string key)
        {
            object obj = null;
            if (_IsCompress)
            {
                byte[] data = client.Get<byte[]>(key);
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
                obj = client.Get(key);
            }
            return obj;
        }

        public T Get<T>(string key)
        {
            object obj = Get(key);           
            if (obj is T)
            {
                return (T)obj;
            }
            return Activator.CreateInstance<T>();
        }

        public void Remove(string key)
        {
            client.Remove(key);
        }

    }
}
