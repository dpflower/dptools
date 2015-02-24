using System;
using System.Collections.Generic;
using System.Text;

namespace DP.Common
{
    public static class IntHelper
    {
        /// <summary>
        /// 整除 余数 进
        /// </summary>
        /// <param name="divisor"></param>
        /// <param name="dividend"></param>
        /// <returns></returns>
        public static int divideMax(int divisor, int dividend)
        {
            int rel = 0;
            if(dividend.Equals(0))
            {
                return rel;
            }
            rel = (divisor / dividend) + ((divisor % dividend) > 0 ? 1 : 0);
            return rel;
        }

        /// <summary>
        /// 整除 余数 舍
        /// </summary>
        /// <param name="divisor"></param>
        /// <param name="dividend"></param>
        /// <returns></returns>
        public static int divideMin(int divisor, int dividend)
        {
            int rel = 0;
            if (dividend.Equals(0))
            {
                return rel;
            }
            rel = (divisor / dividend);
            return rel;
        }
    }
}
