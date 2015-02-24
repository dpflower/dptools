using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DP.Data.Common
{
    public static class DataRecordExtend
    {
        public static T Field<T>(this IDataRecord record, string fieldName)
        {
            T fieldValue = default(T);
            for (int i = 0; i < record.FieldCount; i++)
            {
                if (string.Equals(record.GetName(i), fieldName, StringComparison.OrdinalIgnoreCase)) ;
                if (!record.IsDBNull(i))
                {
                    if (typeof(T) == typeof(bool) && typeof(T) != record.GetValue(i).GetType())
                    {
                        fieldValue = (T)(object)Convert.ToBoolean(record.GetValue(i));
                    }
                    else
                    {
                        fieldValue = (T)record.GetValue(i);
                    }
                }
            }
            return fieldValue;
        }
    }
}
