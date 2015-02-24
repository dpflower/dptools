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
using System.Drawing;
using System.Text.RegularExpressions;

namespace DP.Common
{
    public static class StringHelper
    {
        #region 字符串过滤
        /// <summary>
        ///  替换到 字符串中的  \n  \r  “'”
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ReplaceEnter(string input)
        {
            string temp = input;
            temp = temp.Replace("\'", " ");
            temp = temp.Replace("\r", " ");
            temp = temp.Replace("\n", " ");
            return temp;
        }
        #endregion

        #region 字符串类型验证
        /// <summary>
        /// 是否是数字
        /// </summary>
        /// <param name="szInput"></param>
        /// <returns></returns>
        public static bool IsDecimal(string input)
        {
            Regex regex = new Regex(@"^(-?\d+)(\.\d+)?$");
            return regex.IsMatch(input);
        }
        /// <summary>
        /// 是否是整数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumber(string input)
        {
            Regex regex = new Regex(@"^-?\d+$");
            return regex.IsMatch(input);
        }
        /// <summary>
        /// 是不是IP地址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsIP(string input)
        {
            Regex regex = new Regex(@"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");
            return regex.IsMatch(input);
        }
        /// <summary>
        /// 是不是Email地址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsEmail(string input)
        {
            Regex regex = new Regex(@"^(w+((-w+)|(.w+))*)+w+((-w+)|(.w+))*@[A-Za-z0-9]+((.|-)[A-Za-z0-9]+)*.[A-Za-z0-9]+$");
            return regex.IsMatch(input);
        }
        #endregion

        #region 字符串转型
        /// <summary>
        /// 字符串转短整数
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="defaultValue">The defalut value.</param>
        /// <returns></returns>
        public static short ToShort(string input, short defaultValue)
        {
            if (IsNumber(input))
            {
                return System.Convert.ToInt16(input);
            }
            return defaultValue;
        }
        /// <summary>
        /// 字符串转短整数
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static short ToShort(string input)
        {
            return ToShort(input, 0);
        }
        /// <summary>
        /// 字符串转整数
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static int ToInt(string input, int defaultValue)
        {
            if (IsNumber(input))
            {
                return System.Convert.ToInt32(input);
            }
            return defaultValue;
        }
        /// <summary>
        /// 字符串转整数
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static int ToInt(string input)
        {
            return ToInt(input, 0);
        }
        /// <summary>
        /// 字符串转长整数
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static long ToLong(string input, long defaultValue)
        {
            if (IsNumber(input))
            {
                return System.Convert.ToInt64(input);
            }
            return defaultValue;
        }
        /// <summary>
        /// 字符串转长整数
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static long ToLong(string input)
        {
            return ToLong(input, 0);
        }
        /// <summary>
        /// 字符串转短整数
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="defaultValue">The defalut value.</param>
        /// <returns></returns>
        public static ushort ToUShort(string input, ushort defaultValue)
        {
            if (IsNumber(input))
            {
                return System.Convert.ToUInt16(input);
            }
            return defaultValue;
        }
        /// <summary>
        /// 字符串转短整数
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static ushort ToUShort(string input)
        {
            return ToUShort(input, 0);
        }
        /// <summary>
        /// 字符串转整数
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static uint ToUInt(string input, uint defaultValue)
        {
            if (IsNumber(input))
            {
                return System.Convert.ToUInt32(input);
            }
            return defaultValue;
        }
        /// <summary>
        /// 字符串转整数
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static uint ToUInt(string input)
        {
            return ToUInt(input, 0);
        }
        /// <summary>
        /// 字符串转长整数
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static ulong ToULong(string input, ulong defaultValue)
        {
            if (IsNumber(input))
            {
                return System.Convert.ToUInt64(input);
            }
            return defaultValue;
        }
        /// <summary>
        /// 字符串转长整数
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static ulong ToULong(string input)
        {
            return ToULong(input, 0);
        }
        /// <summary>
        /// 字符串转小数
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="defalutValue">The defalut value.</param>
        /// <returns></returns>
        public static decimal ToDecimal(string input, decimal defaultValue)
        {
            if (IsDecimal(input))
            {
                return System.Convert.ToDecimal(input);
            }
            return defaultValue;
        }
        /// <summary>
        /// 字符串转小数
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static decimal ToDecimal(string input)
        {
            return ToDecimal(input, 0);
        }
        /// <summary>
        /// 字符串转小数
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="defaultValue">The defalut value.</param>
        /// <returns></returns>
        public static double ToDouble(string input, double defaultValue)
        {
            if (IsDecimal(input))
            {
                return System.Convert.ToDouble(input);
            }
            return defaultValue;
        }
        /// <summary>
        /// 字符串转小数
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static double ToDouble(string input)
        {
            return ToDouble(input, 0);
        }
        /// <summary>
        /// 字符串转小数
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="defaultValue">The defalut value.</param>
        /// <returns></returns>
        public static float ToFloat(string input, float defaultValue)
        {
            if (IsDecimal(input))
            {
                return System.Convert.ToSingle(input);
            }
            return defaultValue;
        }
        /// <summary>
        /// 字符串转小数
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static float ToFloat(string input)
        {
            return ToFloat(input, 0);
        }

                
        #endregion

        #region 获取字符串长度
        ///// <summary>
        ///// 获取字符串的存储长度
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public static int GetByteLen(string input)
        //{
        //    byte[] bytes = new ASCIIEncoding().GetBytes(input);
        //    int num = 0;
        //    for (int i = 0; i <= (bytes.Length - 1); i++)
        //    {
        //        if (bytes[i] == 0x3f)
        //        {
        //            num++;
        //        }
        //        num++;
        //    }
        //    return num;

        //}

        /// <summary>
        /// 获取字段串的存储的字节长度
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static int GetbyteLen(string Text)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            return GetbyteLen(Text, utf8);
        }

        /// <summary>
        /// 获取字段串的存储的字节长度
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static int GetbyteLen(string Text, Encoding encoding)
        {
            if (String.IsNullOrEmpty(Text))
            {
                return 0;
            }
            return encoding.GetByteCount(Text);
        }
        #endregion

        #region 获取汉字拼音 首字母
        /// <summary>
        /// 获取字符串的每个汉字的拼音 首字母
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static string GetChineseSpell(string strText)
        {
            int len = strText.Length;
            string myStr = "";
            for (int i = 0; i < len; i++)
            {
                myStr += getSpell(strText.Substring(i, 1));
            }
            return myStr;
        }

        /// <summary>
        /// 获取单个汉字的拼音 首字母
        /// </summary>
        /// <param name="cnChar"></param>
        /// <returns></returns>
        private static string getSpell(string cnChar)
        {
            byte[] arrCN = System.Text.Encoding.Default.GetBytes(cnChar);
            if (arrCN.Length > 1)
            {
                int area = (short)arrCN[0];
                int pos = (short)arrCN[1];
                int code = (area << 8) + pos;
                int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };
                for (int i = 0; i < 26; i++)
                {
                    int max = 55290;
                    if (i != 25) max = areacode[i + 1];
                    if (areacode[i] <= code && code < max)
                    {
                        return System.Text.Encoding.Default.GetString(new byte[] { (byte)(65 + i) });
                    }
                }
                return "";
            }
            else return cnChar;
        }
        #endregion  

        #region 验证码
        /// <summary>
        /// 生成随机验证码
        /// </summary>
        /// <param name="nMax_num">验证码位数</param>
        /// <returns>验证码</returns>
        public static string Get_Random(int nMax_num)
        {
            string str = "";
            Random random = new Random();
            for (int i = 0; i < nMax_num; i++)
            {
                str = str + random.Next(0, 10).ToString();
            }
            return str;
        }

        /// <summary>
        /// 字符串转换成图片
        /// </summary>
        /// <param name="checkCode">字符串</param>
        /// <param name="Height">设置图片高度</param>
        /// <param name="NoiseLine">获取或设置背景噪音线数</param>
        /// <param name="NoiseDot">获取或设置前景噪音点数</param>
        /// <param name="Border">设置是否有边框</param>
        /// <returns>Bitmap图片</returns>    
        public static Bitmap StringToImage(string checkCode, int Height, int NoiseLine, int NoiseDot, bool Border)
        {
            if (checkCode == null || checkCode.Trim() == String.Empty)
                return null;

            System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling((checkCode.Length * 12.5)), Height);
            Graphics g = Graphics.FromImage(image);

            try
            {
                //生成随机生成器
                Random random = new Random();

                //清空图片背景色
                g.Clear(Color.White);

                //画图片的背景噪音线
                for (int i = 0; i < NoiseLine; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);

                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }

                Font font = new System.Drawing.Font("Arial", 12, (System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic));
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(checkCode, font, brush, 2, 2);


                //画图片的前景噪音点
                for (int i = 0; i < NoiseDot; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);

                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }



                //画图片的边框线
                if (Border)
                {
                    g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                }

                return image;
            }
            catch
            {
                image.Dispose();
                throw;
            }
            finally
            {
                g.Dispose();
            }

        }
        #endregion

        /// <summary>
        /// 替换URL  参数
        /// </summary>
        /// <param name="rawUrl">The raw URL.</param>
        /// <param name="queryParam">The query param.</param>
        /// <param name="queryValue">The query value.</param>
        /// <returns></returns>
        public static string UrlReplare(string rawUrl, string queryParam, string queryValue)
        {
            string _RawUrl = rawUrl;
            if (String.IsNullOrEmpty(rawUrl))
            {
                return string.Empty;
            }
            string pattern = @"(?<=[?&]" + queryParam + @"=)([^&]*)";
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);

            if (reg.IsMatch(_RawUrl))
            {
                _RawUrl = reg.Replace(_RawUrl, queryValue);
            }
            else
            {
                if (_RawUrl.IndexOf("?") < 0)
                    _RawUrl += "?" + queryParam + "=" + queryValue;
                else
                    _RawUrl += "&" + queryParam + "=" + queryValue;
            }
            return _RawUrl;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string Substring(string str, string begin, string end)
        {
            int ibegin = 0;
            int iend = 0;
            ibegin = (str.IndexOf(begin) > 0) ? str.IndexOf(begin) : 0;
            iend = (str.IndexOf(end) > 0) ? str.IndexOf(end, ibegin) : str.Length;
            return str.Substring(ibegin, iend - ibegin);
        }

        /// <summary>
        /// 第一个字符串，是否存在于第二个字符串数据中。不区分大小写。
        /// Determines whether the specified STR is exist.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <param name="strs">The STRS.</param>
        /// <returns>
        ///   <c>true</c> if the specified STR is exist; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsExist(string str, string[] strs)
        {
            foreach (string s in strs)
            {
                if (s.ToLower() == str.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

    }
}
