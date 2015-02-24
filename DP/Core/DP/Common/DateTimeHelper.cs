using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace DP.Common
{
    public class DateTimeHelper
    {
        public static IFormatProvider culture = new CultureInfo("zh-CN", true);

        /// <summary>
        /// 对时间字符串做格式化   Formats the specified input.
        /// </summary>
        /// <param name="input">The input.时间字符串</param>
        /// <param name="format">The format.格式化的格式</param>
        /// <returns></returns>
        public static string Format(string input, string format)
        {
            DateTime temp = DateTime.Now;
            if (ToDateTime(input, out temp))
            {
                return temp.ToString(format);
            }
            else
            {
                return input;
            }

        }

        /// <summary>
        /// 将字符串转换成 DateTime 
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>
        public static bool ToDateTime(string input, out DateTime datetime)
        {
            string format = string.Empty;
            return ToDateTime(input, out datetime, out format);            
        }

        /// <summary>
        /// 将字符串转换成 DateTime
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="datetime">The datetime.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static bool ToDateTime(string input, out DateTime datetime, out string format)
        {
            input = input.Replace("T", " ");
            bool rel = false;
            //2010-06-06 12:23:23
            switch (input.Length)
            {
                case 23:
                    {
                        if (DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm:ss.fff", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyy-MM-dd HH:mm:ss.fff";
                            return rel;
                        }
                        if (DateTime.TryParseExact(input, "yyyy/MM/dd HH:mm:ss.fff", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyy/MM/dd HH:mm:ss.fff";
                            return rel;
                        }
                        if (DateTime.TryParseExact(input, "MM/dd/yyyy HH:mm:ss.fff", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "MM/dd/yyyy HH:mm:ss.fff";
                            return rel;
                        }
                    }
                    break;
                case 19:
                    {
                        if (DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm:ss", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyy-MM-dd HH:mm:ss";
                            return rel;
                        }
                        if (DateTime.TryParseExact(input, "yyyy/MM/dd HH:mm:ss", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyy/MM/dd HH:mm:ss";
                            return rel;
                        }
                        if (DateTime.TryParseExact(input, "MM/dd/yyyy HH:mm:ss", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "MM/dd/yyyy HH:mm:ss";
                            return rel;
                        }
                    }
                    break;
                case 18:
                    {
                        if (DateTime.TryParseExact(input, "yyyy-MM-dd H:mm:ss", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyy-MM-dd H:mm:ss";
                            return rel;
                        } 
                        if (DateTime.TryParseExact(input, "yyyy/MM/dd H:mm:ss", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyy/MM/dd H:mm:ss";
                            return rel;
                        }
                        if (DateTime.TryParseExact(input, "MM/dd/yyyy H:mm:ss", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "MM/dd/yyyy H:mm:ss";
                            return rel;
                        }
                    }
                    break;
                case 16:
                    {
                        if (DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyy-MM-dd HH:mm";
                            return rel;
                        }
                        if (DateTime.TryParseExact(input, "yyyy/MM/dd HH:mm", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyy/MM/dd HH:mm";
                            return rel;
                        }
                        if (DateTime.TryParseExact(input, "MM/dd/yyyy HH:mm", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "MM/dd/yyyy HH:mm";
                            return rel;
                        }
                    }
                    break;
                case 14:
                    {
                        if (DateTime.TryParseExact(input, "yyyyMMddHHmmss", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyyMMddHHmmss";
                            return rel;
                        }
                    }
                    break;
                case 13:
                    {
                        if (DateTime.TryParseExact(input, "yyyy-MM-dd HH", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyy-MM-dd HH";
                            return rel;
                        }
                        if (DateTime.TryParseExact(input, "yyyy/MM/dd HH", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyy/MM/dd HH";
                            return rel;
                        }
                        if (DateTime.TryParseExact(input, "MM/dd/yyyy HH", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "MM/dd/yyyy HH";
                            return rel;
                        }
                    }
                    break;
                case 12:
                    {
                        if (DateTime.TryParseExact(input, "yyyyMMddHHmm", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyyMMddHHmm";
                            return rel;
                        }
                    }
                    break;
                case 10:
                    {
                        if (DateTime.TryParseExact(input, "yyyy-MM-dd", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyy-MM-dd";
                            return rel;
                        } 
                        if (DateTime.TryParseExact(input, "yyyy/MM/dd", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyy/MM/dd";
                            return rel;
                        }
                        if (DateTime.TryParseExact(input, "MM/dd/yyyy", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "MM/dd/yyyy";
                            return rel;
                        }
                        if (DateTime.TryParseExact(input, "yyyyMMddHH", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyyMMddHH";
                            return rel;
                        }
                    }
                    break;
                case 8:
                    {
                        if (DateTime.TryParseExact(input, "yyyyMMdd", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyyMMdd";
                            return rel;
                        }
                        if (DateTime.TryParseExact(input, "HH:mm:ss", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "HH:mm:ss";
                            return rel;
                        }
                    }
                    break;
                case 7:
                    {
                        if (DateTime.TryParseExact(input, "yyyy-MM", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyy-MM";
                            return rel;
                        }
                        if (DateTime.TryParseExact(input, "yyyy/MM", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyy/MM";
                            return rel;
                        }
                    }
                    break;
                case 6:
                    {
                        if (DateTime.TryParseExact(input, "HHmmss", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "HHmmss";
                            return rel;
                        }
                        if (DateTime.TryParseExact(input, "yyyyMM", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "yyyyMM";
                            return rel;
                        }
                    }
                    break;
                case 5:
                    {
                        if (DateTime.TryParseExact(input, "HH:mm", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "HH:mm";
                            return rel;
                        }
                        if (DateTime.TryParseExact(input, "mm:ss", culture, DateTimeStyles.None, out datetime))
                        {
                            rel = true;
                            format = "mm:ss";
                            return rel;
                        }
                    }
                    break;
                default:
                    {
                        if (DateTime.TryParse(input, out datetime))
                        {
                            rel = true;
                            format = "";
                            return rel;
                        }
                        else
                        {
                            datetime = DateTime.Now;
                            format = "";
                            return false;
                        }
                    }
            }
            datetime = DateTime.Now;
            format = "";
            return false;
        }

        /// <summary>
        /// 将字符串转换成 DateTime 
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static DateTime? ToDateTime(string input)
        {
            DateTime temp = DateTime.Now;
            if (ToDateTime(input, out temp))
            {
                return temp;
            }
            else
            {
                return null;
            }
        }

    }
}
