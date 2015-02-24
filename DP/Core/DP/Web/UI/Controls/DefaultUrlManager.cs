using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections;
using System.Runtime.Serialization;

namespace DP.Web.UI.Controls
{
    [Serializable]
    public class DefaultUrlManager : UrlManager
    {

        public DefaultUrlManager(int recordCount, int pageSize, string queryParam)
            : base(recordCount, pageSize, queryParam) { }

        public DefaultUrlManager(int recordCount, int pageSize)
            : this(recordCount, pageSize, "Page") { }

        public DefaultUrlManager(int recordCount)
            : this(recordCount, 10) { }

        // 获得页面Url
        public override string GetPageUrl(int pageIndex)
        {
            string pageUrl = HttpContext.Current.Request.RawUrl;
            pageUrl = pageUrl.TrimEnd('?');

            string pattern = @"(?<=[?&]" + queryParam + @"=)(\d+)\b";
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);

            // 如果找到匹配，也就是URL中含有类似 ?page=3 或者 &page=4 这样的字符串
            // 则对后面的数值进行替换
            if (reg.IsMatch(pageUrl))
            {
                pageUrl = reg.Replace(pageUrl, pageIndex.ToString());
            }
            else
            {
                string queryString = string.Empty;
                try
                {
                    queryString = HttpContext.Current.Request.Url.Query;
                }
                catch
                {
                    queryString = HttpContext.Current.Request.UrlReferrer.Query;
                }

                if (string.IsNullOrEmpty(queryString))
                    pageUrl += "?" + queryParam + "=" + pageIndex.ToString();
                else
                    pageUrl += "&" + queryParam + "=" + pageIndex.ToString();
            }

            return pageUrl;
        }

        // 获得页面Url
        public override string GetPageUrl(string pageIndex)
        {
            string pageUrl = HttpContext.Current.Request.RawUrl;
            pageUrl = pageUrl.TrimEnd('?');

            string pattern = @"(?<=[?&]" + queryParam + @"=)(\d+)\b";
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);

            // 如果找到匹配，也就是URL中含有类似 ?page=3 或者 &page=4 这样的字符串
            // 则对后面的数值进行替换
            if (reg.IsMatch(pageUrl))
            {
                pageUrl = reg.Replace(pageUrl, pageIndex.ToString());
            }
            else
            {
                string queryString = string.Empty;
                try
                {
                    queryString = HttpContext.Current.Request.Url.Query;
                }
                catch
                {
                    queryString = HttpContext.Current.Request.UrlReferrer.Query;
                }
                if (string.IsNullOrEmpty(queryString))
                    pageUrl += "?" + queryParam + "=" + pageIndex.ToString();
                else
                    pageUrl += "&" + queryParam + "=" + pageIndex.ToString();
            }

            return pageUrl;
        }
    }
}
