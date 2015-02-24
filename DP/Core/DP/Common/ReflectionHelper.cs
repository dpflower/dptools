using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace DP.Common
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static object ChangeType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
                if (String.IsNullOrEmpty(value.ToString()) && conversionType.IsValueType)
                {
                    return null;
                }
            }
            return Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// 获取类型全称
        /// Types the full name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string TypeFullName(Type type)
        {
            string fullName = string.Empty;
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                NullableConverter nullableConverter = new NullableConverter(type);
                fullName = nullableConverter.UnderlyingType.FullName;
            }
            else
            {
                fullName = type.FullName;
            }
            return fullName;
        }

    }
}
