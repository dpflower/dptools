using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

#region Assembly Resource Attribute
[assembly: System.Web.UI.WebResource("DP.Resources.CSS.PagerControlStyle.css", "text/css")]
#endregion

namespace DP.Web.UI.Controls
{
    /// <summary>
    /// Repeater分页控件可进行Ajax分页(但Ajax分页不能传参数)
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:RepeaterPages runat=server></{0}:RepeaterPages>")]
    public class RepeaterPages : Repeater
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public RepeaterPages() : base()
        {
            // 当前页之前可以显示的最多链接数，大于此条链接将被隐藏
            ViewState["PreviousPageCount"] = 5;

            // 当前页之后可以显示的最多链接数，大于此条链接将被隐藏
            ViewState["AfterPageCount"] = 4;

            // 供客户端设置样式
            ViewState["CssClass"] = "Pager";
        }

        
        private int currentPage;					// 当前页的页码
        private int pageCount;						// 总页数

        private bool showPrevious = false;		// 是否显示 上一页、第一页 的链接
        private bool showNext = false;			// 是否显示 下一页、最末页 的链接

        private int startPage;						// 显示的第一页 的 页码
        private int endPage;							// 显示的最末页 的 页码

        /// <summary>
        /// CSS样式类 默认 “GreenStyle”、“Pager” 
        /// </summary>
        [Bindable(false), Category("Behavior"), DefaultValue("Pager"), TypeConverter(typeof(PagerCssConverter)), Description("分页样式")]
        public new string CssClass
        {
            get { return ViewState["CssClass"].ToString(); }
            set { ViewState["CssClass"] = value; }
        }

        /// <summary>
        /// 设置当前页之前显示的最大链接数
        /// </summary>
        public int PreviousPageCount
        {
            get { return (int)ViewState["PreviousPageCount"]; }
            set { ViewState["PreviousPageCount"] = value; }
        }

        /// <summary>
        /// 设置当前页之后显示的最大链接数
        /// </summary>
        public int AfterPageCount
        {
            get { return (int)ViewState["AfterPageCount"]; }
            set { ViewState["AfterPageCount"] = value; }
        }

        /// <summary>
        /// Url 管理对象
        /// </summary>
        public UrlManager UrlManager
        {
            get { return (UrlManager)ViewState["UrlManager"]; }
            set { ViewState["UrlManager"] = value; }
        }

        // 添加“第一页”，“上一页”的连接
        private void AddPreviousLink(UrlManager UrlManager, HtmlTextWriter output)
        {

            output.AddAttribute(HtmlTextWriterAttribute.Class, "PagerIcon");
            output.AddAttribute(HtmlTextWriterAttribute.Title, "第一页");
            output.AddAttribute(HtmlTextWriterAttribute.Href, UrlManager.GetPageUrl(1));
            output.RenderBeginTag(HtmlTextWriterTag.A);
            output.Write("&lt;&lt;");
            output.RenderEndTag();

            output.AddAttribute(HtmlTextWriterAttribute.Class, "PagerIcon");
            output.AddAttribute(HtmlTextWriterAttribute.Title, "上一页");
            output.AddAttribute(HtmlTextWriterAttribute.Href, UrlManager.GetPageUrl(currentPage - 1));
            output.RenderBeginTag(HtmlTextWriterTag.A);
            output.Write("&lt;");
            output.RenderEndTag();

            showPrevious = false;	// 只显示一次
        }


        // 添加 “下一页”、“最末页” 的链接
        private void AddNextLink(UrlManager UrlManager, HtmlTextWriter output)
        {

            output.AddAttribute(HtmlTextWriterAttribute.Class, "PagerIcon");
            output.AddAttribute(HtmlTextWriterAttribute.Title, "下一页");
            output.AddAttribute(HtmlTextWriterAttribute.Href, UrlManager.GetPageUrl(currentPage + 1));
            output.RenderBeginTag(HtmlTextWriterTag.A);
            output.Write("&gt;");
            output.RenderEndTag();

            output.AddAttribute(HtmlTextWriterAttribute.Class, "PagerIcon");
            output.AddAttribute(HtmlTextWriterAttribute.Title, "最末页");
            output.AddAttribute(HtmlTextWriterAttribute.Href, UrlManager.GetPageUrl(pageCount));
            output.RenderBeginTag(HtmlTextWriterTag.A);
            output.Write("&gt;&gt;");
            output.RenderEndTag();

            showNext = false;	// 可有可无，程序会跳出循环
        }


        // 根据当前页，当前页之前可以显示的页数，算得从第几页开始进行显示
        private void SetStartPage()
        {

            // 如果当前页小于它前面所可以显示的条目数，
            // 那么显示第一页就是实际的第一页
            if (currentPage <= PreviousPageCount)
            {
                startPage = 1;
            }
            else
            // 这种情况下 currentPage 前面总是能显示完，
            // 要根据后面的长短确定是不是前面应该多显示
            {
                if (currentPage > PreviousPageCount + 1)
                    showPrevious = true;

                int linkLength = (pageCount - currentPage + 1) + PreviousPageCount;

                int startPage = currentPage - PreviousPageCount;

                while (linkLength < PreviousPageCount + AfterPageCount + 1 && startPage > 1)
                {
                    linkLength++;
                    startPage--;
                }

                this.startPage = startPage;
            }
        }

        // 根据CurrentPage、总页数、当前页之后长度 算得显示的最末页是 第几页
        private void SetEndPage()
        {
            // 如果当前页加上它之后可以显示的页数 大于 总页数，
            // 那么显示的最末页就是实际的最末页
            if (currentPage + AfterPageCount >= pageCount)
            {
                endPage = pageCount;
            }
            else
            {

                // 这种情况下 currentPage后面的总是可以显示完，
                // 要根据前面的长短确定是不是后面应该多显示

                int linkLength = (currentPage - startPage + 1) + AfterPageCount;

                int endPage = currentPage + AfterPageCount;

                while (linkLength < PreviousPageCount + AfterPageCount + 1 && endPage < pageCount)
                {
                    linkLength++;
                    endPage++;
                }

                if (endPage < pageCount)
                    showNext = true;

                this.endPage = endPage;
            }
        }
        
        /// <summary>
        /// 设置CSS的引用
        /// </summary>
        private void SetCssLink()
        {
            string includeTemplate = "<link rel='stylesheet' text='text/css' href='{0}' />";
            string includeLocation = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "DP.Resources.CSS.PagerControlStyle.css");
            //LiteralControl include = new LiteralControl(String.Format(includeTemplate, includeLocation));
            //page.Header.Controls.Add(include);
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "PagerControlStyle", String.Format(includeTemplate, includeLocation));

            //string includeTemplate = "<link rel='stylesheet' text='text/css' href='{0}' />";
            //string includeLocation = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "DP.Resources.CSS.PagerControlStyle.css");
            //LiteralControl include = new LiteralControl(String.Format(includeTemplate, includeLocation));
            //this.Page.Header.Controls.Add(include);            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.SetCssLink();
        }

        /// <summary>
        /// 显示在页面上
        /// </summary>
        /// <param name="output"></param>
        protected override void Render(HtmlTextWriter output)
        {
            base.Render(output);

            output.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
            output.RenderBeginTag(HtmlTextWriterTag.Div);

            if (UrlManager == null)
            {
                return;
                //throw new ArgumentNullException("UrlManager 不能为 Null");
            }

            // 获取当前页
            currentPage = UrlManager.CurrentPageIndex;

            // 获取总页数
            pageCount = UrlManager.PageCount;

            SetStartPage();
            SetEndPage();

            // 循环打印链接
            for (int i = startPage; i <= endPage; i++)
            {
                if (showPrevious)			 // 如果需要显示前一页、第一页链接
                    AddPreviousLink(UrlManager, output);


                if (i == currentPage)
                {
                    output.AddAttribute(HtmlTextWriterAttribute.Class, "CurrentPage");
                }

                output.AddAttribute(HtmlTextWriterAttribute.Href, UrlManager.GetPageUrl(i));
                output.RenderBeginTag(HtmlTextWriterTag.A);
                output.Write(i);
                output.RenderEndTag();	// A

                if (i == endPage && showNext)	// 如果需要显示 下一页、最末页 链接
                    AddNextLink(UrlManager, output);
            }

            output.RenderBeginTag(HtmlTextWriterTag.Span);
            output.Write(String.Format(" ( 第<b>{0}</b>页/共<b>{1}</b>页 )", currentPage, pageCount));
            output.RenderEndTag();	// Span

            output.RenderEndTag();	// Div
        }




    }
}
