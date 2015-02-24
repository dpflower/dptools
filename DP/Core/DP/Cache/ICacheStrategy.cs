using System;
using System.Collections.Generic;
using System.Text;

namespace DP.Cache
{
    public interface ICacheStrategy
    {
        bool IsCompress{get;set;}
        bool Add(string key, object obj);
        object Get(string key);
        T Get<T>(string key);
        void Remove(string key);
    }
}
