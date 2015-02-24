using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DP.Data.Common
{
    public class Condition
    {

        public Condition(string key, object value)
            : this(key, value, QueryOperator.Equal)
        {

        }
        public Condition(string key, object value, QueryOperator queryOperator)
        {
            _key = key;
            _value = value;
            _queryOperator = queryOperator;
        }

        public Condition(string key, short[] values)
            : this(key, values, QueryOperator.In)
        {

        }
        public Condition(string key, short[] values, QueryOperator queryOperator)
        {
            if (queryOperator != Common.QueryOperator.In)
            {
                throw new Exception("键值为数组时，QueryOperator 必须为In!");
            }
            _key = key;
            List<object> list = new List<object>();
            foreach (short v in values)
            {
                list.Add(v);
            }
            _values = list.ToArray();
            _queryOperator = QueryOperator.In;
        }

        public Condition(string key, int[] values)
            : this(key, values, QueryOperator.In)
        {

        }
        public Condition(string key, int[] values, QueryOperator queryOperator)
        {
            if (queryOperator != Common.QueryOperator.In)
            {
                throw new Exception("键值为数组时，QueryOperator 必须为In!");
            }
            _key = key;
            List<object> list = new List<object>();
            foreach (int v in values)
            {
                list.Add(v);
            }
            _values = list.ToArray();
            _queryOperator = QueryOperator.In;
        }

        public Condition(string key, long[] values)
            : this(key, values, QueryOperator.In)
        {

        }
        public Condition(string key, long[] values, QueryOperator queryOperator)
        {
            if (queryOperator != Common.QueryOperator.In)
            {
                throw new Exception("键值为数组时，QueryOperator 必须为In!");
            }
            _key = key;
            List<object> list = new List<object>();
            foreach (long v in values)
            {
                list.Add(v);
            }
            _values = list.ToArray();
            _queryOperator = QueryOperator.In;
        }

        public Condition(string key, string[] values)
            : this(key, values, QueryOperator.In)
        {

        }
        public Condition(string key, string[] values, QueryOperator queryOperator)
        {
            if (queryOperator != Common.QueryOperator.In)
            {
                throw new Exception("键值为数组时，QueryOperator 必须为In!");
            }
            _key = key;
            List<object> list = new List<object>();
            foreach (string v in values)
            {
                list.Add(v);
            }
            _values = list.ToArray();
            _queryOperator = QueryOperator.In;
        }


        string _key = string.Empty;
        object _value = string.Empty;
        object[] _values = null;

        QueryOperator _queryOperator = QueryOperator.Equal;


        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public object[] Values
        {
            get { return _values; }
            set { _values = value; }
        }

        public QueryOperator QueryOperator
        {
            get { return _queryOperator; }
            set { _queryOperator = value; }
        }
    }
}
