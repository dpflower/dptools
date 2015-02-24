using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections;
using System.Runtime.Serialization;


namespace DP.Web.UI.Controls
{
    [Serializable]
    public abstract class UrlManager
    {
        protected int currentPageIndex;			    // 当前页码
        protected int recordCount;					// 记录总数
        protected int pageSize;						// 分页大小
        protected int pageCount;					// 总页数
        protected string queryParam;				// 传递页数的参数名称

        protected UrlManager(int recordCount, int pageSize, string queryParam)
        {

            if (recordCount < 0)
                throw new ArgumentOutOfRangeException("recordCount 应该大于等于 0 !");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize 应该大于 0 ！");
            if (String.IsNullOrEmpty(queryParam))
                throw new ArgumentNullException("queryParam 不能为空！");

            // 设置私有变量
            this.recordCount = recordCount;
            this.pageSize = pageSize;
            this.queryParam = queryParam;
            this.pageCount = getPageCount(recordCount, pageSize);
            this.currentPageIndex = getPageIndex(recordCount, pageCount, queryParam);
        }


        // 获取页码总数
        private int getPageCount(int recordCount, int pageSize)
        {
            int pageCount;

            // 如果记录数为0，也认为有一页(因为至少需要进行一个显示)
            if (recordCount == 0)
            {
                pageCount = 1;
            }
            else
            {
                // 计算总页数
                if (recordCount % pageSize == 0)
                    pageCount = recordCount / pageSize;
                else
                    pageCount = (recordCount / pageSize) + 1;
            }

            return pageCount;
        }

        // 从Url参数中获得但前页码
        private int getPageIndex(int recordCount, int pageCount, string queryParam)
        {
            if (recordCount == 0)
                return 1;		// 如果记录数为0，则显示为第一页

            int pageIndex;

            // 从Url参数获得当前页码
            string queryIndex =
                HttpContext.Current.Request.QueryString[queryParam];

            // 对页码进行一些校验
            if (string.IsNullOrEmpty(queryIndex))
                pageIndex = 1;		// 显示第一页
            else
            {
                try
                {
                    pageIndex = Math.Abs(int.Parse(queryIndex));

                    if (pageIndex == 0)
                        pageIndex = 1;

                    // 如果当前页大于总页数，设当前页为最后一页
                    if (pageIndex > pageCount)
                        pageIndex = pageCount;
                }
                catch
                {
                    pageIndex = 1;	// 显示第一页
                }
            }

            return pageIndex;
        }

        public int PageCount
        {
            get { return pageCount; }
        }

        public int RecordCount
        {
            get { return recordCount; }
        }

        public int PageSize
        {
            get { return pageSize; }
        }

        public int CurrentPageIndex
        {
            get { return currentPageIndex; }
        }

        public abstract string GetPageUrl(int pageIndex);

        public abstract string GetPageUrl(string pageIndex);
    }
}
