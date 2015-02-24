using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DP.Common;

namespace DP.Web.UI.Controls
{

    [DefaultProperty("Text")]
    [ToolboxData("<{0}:WebGridView runat=server></{0}:WebGridView>")]
    public class WebGridView : GridView, INamingContainer
    {
        #region 排序属性
        /// <summary>
        /// Gets or sets the sort direction value.
        /// </summary>
        /// <value>The sort direction value.</value>
        [Description("排序－方向"), Category("扩展"), DefaultValue("ASC")]
        public string SortDirectionValue
        {
            get
            {
                object o = ViewState["SortDirectionValue"];
                return (o != null ? o.ToString() : "ASC");
            }
            set
            {
                ViewState["SortDirectionValue"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the sort expression value.
        /// </summary>
        /// <value>The sort expression value.</value>
        [Description("排序－字段"), Category("扩展"), DefaultValue("")]
        public string SortExpressionValue
        {
            get
            {
                object o = ViewState["SortExpressionValue"];
                return (o != null ? o.ToString() : "");
            }
            set
            {
                ViewState["SortExpressionValue"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the sort conditions.
        /// </summary>
        /// <value>The sort conditions.</value>
        [Description("排序－初始排序条件"), Category("扩展"), DefaultValue("")]
        public string SortConditions
        {
            get
            {
                object o = ViewState["SortConditions"];
                return (o != null ? o.ToString() : "");
            }
            set
            {
                if (String.IsNullOrEmpty(SortExpressionValue))
                {
                    ViewState["SortConditions"] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the sort direction tags.
        /// </summary>
        /// <value>The sort direction tags.</value>
        [Description("排序－链接地址方向标识"), Category("扩展"), DefaultValue("SortDirection")]
        public string SortDirectionTags
        {
            get
            {
                object o = ViewState["SortDirectionTags"];
                return (o != null ? o.ToString() : "SortDirection");
            }
            set
            {
                ViewState["SortDirectionTags"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the sort expression tags.
        /// </summary>
        /// <value>The sort expression tags.</value>
        [Description("排序－链接地址字段标识"), Category("扩展"), DefaultValue("SortExpression")]
        public string SortExpressionTags
        {
            get
            {
                object o = ViewState["SortExpressionTags"];
                return (o != null ? o.ToString() : "SortExpression");
            }
            set
            {
                ViewState["SortExpressionTags"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the image asc URL.
        /// </summary>
        /// <value>The image asc URL.</value>
        [Description("排序－升序图标"), Category("扩展"), DefaultValue("")]
        public string ImageAscUrl
        {
            get
            {
                object o = ViewState["ImageAscUrl"];
                return (o != null ? o.ToString() : "");
            }
            set
            {
                ViewState["ImageAscUrl"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the image desc URL.
        /// </summary>
        /// <value>The image desc URL.</value>
        [Description("排序－降序图标"), Category("扩展"), DefaultValue("")]
        public string ImageDescUrl
        {
            get
            {
                object o = ViewState["ImageDescUrl"];
                return (o != null ? o.ToString() : "");
            }
            set
            {
                ViewState["ImageDescUrl"] = value;
            }
        } 
        #endregion

        #region 分页属性
        /// <summary>
        /// Gets or sets the page tags.
        /// </summary>
        /// <value>The page tags.</value>
        [Description("分页－链接地址分页标识"), Category("扩展"), DefaultValue("Page")]
        public string PageTags
        {
            get
            {
                object o = ViewState["PageTags"];
                return (o != null ? o.ToString() : "Page");
            }
            set
            {
                ViewState["PageTags"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the record connt.
        /// </summary>
        /// <value>The record connt.</value>
        [Description("分页－记录总数"), Category("扩展"), DefaultValue(0)]
        public int RecordCount
        {
            get
            {
                object o = ViewState["RecordConnt"];
                return (o != null ? (int)o : 0);
            }
            set
            {
                ViewState["RecordConnt"] = value;
                AllPageCount = (RecordCount / PageSize) + (((RecordCount % PageSize) > 0) ? 1 : 0);
                if (AllPageCount > 0)
                {
                    if (CurrentPageIndex > AllPageCount - 1)
                    {
                        CurrentPageIndex = AllPageCount - 1;
                    }
                }
            }
        }

        /// <summary>
        /// 设置当前页之前显示的最大链接数
        /// </summary>
        [Description("分页－前序页面数"), Category("扩展"), DefaultValue(5)]
        public int PreviousPageCount
        {
            get
            {
                object o = ViewState["PreviousPageCount"];
                return (o != null ? (int)o : 5);
            }
            set
            {
                ViewState["PreviousPageCount"] = value;
            }
        }

        /// <summary>
        /// 设置当前页之后显示的最大链接数
        /// </summary>
        [Description("分页－后序页面数"), Category("扩展"), DefaultValue(5)]
        public int AfterPageCount
        {
            get
            {
                object o = ViewState["AfterPageCount"];
                return (o != null ? (int)o : 5);
            }
            set
            {
                ViewState["AfterPageCount"] = value;
            }
        }

        /// <summary>
        /// Gets or sets all page count.
        /// </summary>
        /// <value>All page count.</value>
        [Description("分页－页面总数"), Category("扩展"), DefaultValue(0)]
        public int AllPageCount
        {
            get
            {
                object o = ViewState["AllPageCount"];
                return (o != null ? (int)o : 0);
            }
            set
            {
                ViewState["AllPageCount"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the current page.
        /// </summary>
        /// <value>The index of the current page.</value>
        [Description("分页－当前面索引"), Category("扩展"), DefaultValue(0)]
        public int CurrentPageIndex
        {
            get
            {
                object o = ViewState["CurrentPageIndex"];
                return (o != null ? (int)o : 0);
            }
            set
            {
                ViewState["CurrentPageIndex"] = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Description("分页－是否显示分页信息"), Category("扩展"), DefaultValue(true)]
        public bool IsShowPagerInfo
        {
            get
            {
                return (ViewState["IsShowPagerInfo"] == null) ? true : (bool)ViewState["IsShowPagerInfo"];
            }
            set
            {
                ViewState["IsShowPagerInfo"] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Description("分页－是否显示跳转页面按钮"), Category("扩展"), DefaultValue(true)]
        public bool IsShowJumpPage
        {
            get
            {
                return (ViewState["IsShowJumpPage"] == null) ? true : (bool)ViewState["IsShowJumpPage"];
            }
            set
            {
                ViewState["IsShowJumpPage"] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Description("分页－是否显示前一页按钮"), Category("扩展"), DefaultValue(true)]
        public bool IsShowPreviousPage
        {
            get
            {
                return (ViewState["IsShowPreviousPage"] == null) ? true : (bool)ViewState["IsShowPreviousPage"];
            }
            set
            {
                ViewState["IsShowPreviousPage"] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Description("分页－是否显示后一页按钮"), Category("扩展"), DefaultValue(true)]
        public bool IsShowNextPage
        {
            get
            {
                return (ViewState["IsShowNextPage"] == null) ? true : (bool)ViewState["IsShowNextPage"];
            }
            set
            {
                ViewState["IsShowNextPage"] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Description("分页－是否显示数字按钮"), Category("扩展"), DefaultValue(true)]
        public bool IsShowDigitalPage
        {
            get
            {
                return (ViewState["IsShowDigitalPage"] == null) ? true : (bool)ViewState["IsShowDigitalPage"];
            }
            set
            {
                ViewState["IsShowDigitalPage"] = value;
            }
        }


        #endregion

        #region 重写 GridView 事件
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            GetSortAndPageConditions();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnRowCreated(GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                case DataControlRowType.Header:
                    {
                        Sort(e);
                    }
                    break;
                case DataControlRowType.DataRow:
                    {
                        e.Row.Attributes.Add("id", this.ID + "_trIndex_" + e.Row.RowIndex.ToString());
                    }
                    break;
            }
        }

        protected override void InitializePager(GridViewRow row, int columnSpan, PagedDataSource pagedDataSource)
        {
            Pager(row, columnSpan);
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
        }

        protected override void LoadControlState(object savedState)
        {
            base.LoadControlState(savedState);
        }

        protected override void InitializeRow(GridViewRow row, DataControlField[] fields)
        {
            base.InitializeRow(row, fields);
        }

        protected override void OnPreRender(EventArgs e)
        {
            LoadResource();
            base.OnPreRender(e);
        }        

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        protected override void OnDataBound(EventArgs e)
        {
            base.OnDataBound(e);
            if (this.AllowPaging)
            {
                if (this.Rows.Count != 0)
                {
                    Control table = this.Controls[0];
                    int count = table.Controls.Count;
                    table.Controls[count - 1].Visible = true;
                }
            }
        } 
        #endregion

        #region 公共
        /// <summary>
        /// 获取 排序 及 分页 条件 Gets the sort and page conditions.
        /// </summary>
        private void GetSortAndPageConditions()
        {
            int _page = 0;
            if (System.Web.HttpContext.Current != null)
            {
                if (!String.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString[PageTags]))
                {
                    _page = StringHelper.ToInt(System.Web.HttpContext.Current.Request.QueryString[PageTags]);

                    if (_page <= 0)
                    {
                        this.CurrentPageIndex = 0;
                    }
                    else
                    {
                        this.CurrentPageIndex = _page - 1;
                    }
                }

                if (!String.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString[SortDirectionTags]))
                {
                    SortDirectionValue = System.Web.HttpContext.Current.Request.QueryString[SortDirectionTags].ToString();
                }
                if (!String.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString[SortExpressionTags]))
                {
                    SortExpressionValue = System.Web.HttpContext.Current.Request.QueryString[SortExpressionTags].ToString();
                }
                if (!String.IsNullOrEmpty(SortExpressionValue))
                {
                    if (!String.IsNullOrEmpty(SortDirectionValue) && SortDirectionValue.ToUpper().Equals("DESC"))
                    {
                        ViewState["SortConditions"] = SortExpressionValue + " DESC ";
                    }
                    else
                    {
                        ViewState["SortConditions"] = SortExpressionValue + " ASC ";
                    }
                }
            }
        }

        /// <summary>
        /// 加载资源文件
        /// </summary>
        private void LoadResource()
        {
            Page page = System.Web.HttpContext.Current.Handler as Page;
            if (page == null)
            {
                return;
            }
            string includeTemplate = "<link rel='stylesheet' text='text/css' href='{0}' />";
            string includeLocation = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "DP.Resources.CSS.GridViewStyle.css");
            //LiteralControl include = new LiteralControl(String.Format(includeTemplate, includeLocation));
            //page.Header.Controls.Add(include);
            page.ClientScript.RegisterClientScriptBlock(this.GetType(), "GridViewStyle", String.Format(includeTemplate, includeLocation));
            page.ClientScript.RegisterClientScriptResource(this.GetType(), "DP.Resources.JavaScript.JSUtil.js");
            page.ClientScript.RegisterClientScriptResource(this.GetType(), "DP.Resources.JavaScript.PagerControl.js");
            page.ClientScript.RegisterClientScriptResource(this.GetType(), "DP.Resources.JavaScript.MaskTextBox.js");
        } 
        #endregion

        #region 排序相关方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void Sort(GridViewRowEventArgs e)
        {
            if (System.Web.HttpContext.Current == null)
            {
                return;
            }
            if (String.IsNullOrEmpty(this.ImageAscUrl))
            {
                this.ImageAscUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "DP.Resources.Images.Asc.gif");
            }
            else
            {
                this.ImageAscUrl = Page.ResolveClientUrl(this.ImageAscUrl);
            }
            if (String.IsNullOrEmpty(this.ImageDescUrl))
            {
                this.ImageDescUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "DP.Resources.Images.Desc.gif");
            }
            else
            {
                this.ImageDescUrl = Page.ResolveClientUrl(this.ImageDescUrl);
            }

            for (int i = 0; i < this.Columns.Count; i++)
            {
                if (!String.IsNullOrEmpty(this.Columns[i].SortExpression))
                {
                    string _url = System.Web.HttpContext.Current.Request.RawUrl;
                    _url = StringHelper.UrlReplare(_url, this.SortExpressionTags, this.Columns[i].SortExpression);
                    if (this.Columns[i].SortExpression.ToUpper().Equals(SortExpressionValue.ToUpper()) && SortDirectionValue.ToUpper().Equals("ASC"))
                    {
                        _url = StringHelper.UrlReplare(_url, this.SortDirectionTags, "DESC");
                    }
                    else
                    {
                        _url = StringHelper.UrlReplare(_url, this.SortDirectionTags, "ASC");
                    }

                    _url = StringHelper.UrlReplare(_url, this.PageTags, "1");

                    HyperLink hl = new HyperLink();
                    hl.ID = this.Columns[i].SortExpression;
                    hl.NavigateUrl = _url;
                    hl.Text = this.Columns[i].HeaderText;
                    e.Row.Cells[i].Style.Add("white-space", "nowrap");
                    e.Row.Cells[i].Controls.Clear();
                    e.Row.Cells[i].Controls.Add(hl);

                    if (this.Columns[i].SortExpression.ToUpper().Equals(SortExpressionValue.ToUpper()))
                    {
                        if (SortDirectionValue.ToUpper().Equals("ASC"))
                        {
                            e.Row.Cells[i].Controls.Add(new LiteralControl("&nbsp;<img src='" + this.ImageAscUrl + "' class='asc' border='0' />&nbsp; "));
                        }
                        else
                        {
                            e.Row.Cells[i].Controls.Add(new LiteralControl("&nbsp;<img src='" + this.ImageDescUrl + "' class='asc' border='0' />&nbsp; "));
                        }
                    }

                }
            }
        }

        #endregion

        #region 分页相关方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnSpan"></param>
        private void Pager(GridViewRow row, int columnSpan)
        {
            if (System.Web.HttpContext.Current == null)
            {
                return;
            }

            AllPageCount = (RecordCount / PageSize) + (((RecordCount % PageSize) > 0) ? 1 : 0);
            row.Controls.Clear();

            TableCell tc = new TableCell();
            tc.Height = 25;
            tc.ColumnSpan = columnSpan;
            string _url = System.Web.HttpContext.Current.Request.RawUrl;

            PagerFirstAndPreviousLink(tc, _url);
            PagerDigitalLink(tc, _url);
            PagerLastAndNextLink(tc, _url);
            PageJumpLink(tc, _url);
            PagerInfo(tc);

            row.Controls.Add(tc);
        }

        /// <summary>
        /// 获取分页开始页 Gets the start page.
        /// </summary>
        /// <returns></returns>
        private int GetStartPage()
        {
            if (CurrentPageIndex <= PreviousPageCount)
            {
                return 0;
            }
            if (AllPageCount <= (PreviousPageCount + AfterPageCount))
            {
                return 0;
            }
            if ((AllPageCount - CurrentPageIndex) <= AfterPageCount)
            {
                return AllPageCount - (PreviousPageCount + AfterPageCount + 1);
            }
            return CurrentPageIndex - PreviousPageCount;
        }

        /// <summary>
        /// 获取分页结束页 Gets the end page.
        /// </summary>
        /// <returns></returns>
        private int GetEndPage()
        {
            if ((AllPageCount - CurrentPageIndex) <= AfterPageCount)
            {
                return AllPageCount - 1;
            }
            if (AllPageCount <= (PreviousPageCount + AfterPageCount))
            {
                return AllPageCount - 1;
            }
            if (CurrentPageIndex <= PreviousPageCount)
            {
                return (PreviousPageCount + AfterPageCount);
            }
            return CurrentPageIndex + AfterPageCount;
        }

        /// <summary>
        /// 设置 跳转 链接 
        /// </summary>
        /// <param name="tc">链接容器 TableCell</param>
        /// <param name="_url">页面链接地址</param>
        private void PageJumpLink(TableCell tc, string _url)
        {
            if (!IsShowJumpPage)
            {
                return;
            }
            tc.Controls.Add(new LiteralControl(String.Format("<input id=\"txtPageNumber\" type=\"Text\" maxlength=\"5\"  style=\"width: 40px;\" onKeyUp=\"isPositiveInteger(this);\" /> <a href=\"{0}\" value=\"{0}\" onclick=\"return JumpPage('txtPageNumber',this);\" >转至</a>", StringHelper.UrlReplare(_url, this.PageTags, "{0}"))));

        }

        /// <summary>
        /// 设置 首页 和 上一页 链接 
        /// </summary>
        /// <param name="tc">链接容器 TableCell</param>
        /// <param name="_url">页面链接地址</param>
        private void PagerFirstAndPreviousLink(TableCell tc, string _url)
        {
            if (!IsShowPreviousPage)
            {
                return;
            }
            HyperLink First = new HyperLink();
            HyperLink Prev = new HyperLink();
            if (!String.IsNullOrEmpty(PagerSettings.FirstPageImageUrl))
            {
                //First.Text = "首页";
                First.Text = "<img src='" + ResolveUrl(PagerSettings.FirstPageImageUrl) + "' border='0'/>";
            }
            else
            {
                First.Text = PagerSettings.FirstPageText;
            }
            First.CssClass = "First";
            if (!String.IsNullOrEmpty(PagerSettings.PreviousPageImageUrl))
            {
                //Prev.Text = "上一页";
                Prev.Text = "<img src='" + ResolveUrl(PagerSettings.PreviousPageImageUrl) + "' border='0'/>";
            }
            else
            {
                Prev.Text = PagerSettings.PreviousPageText;
            }
            Prev.CssClass = "Prev";

            if (CurrentPageIndex > 0)
            {
                First.NavigateUrl = StringHelper.UrlReplare(_url, this.PageTags, "1");
                Prev.NavigateUrl = StringHelper.UrlReplare(_url, this.PageTags, String.Format("{0}", CurrentPageIndex));
            }
            tc.Controls.Add(First);
            tc.Controls.Add(Prev);
        }

        /// <summary>
        /// 设置 数字 链接 Digitals the link.
        /// </summary>
        /// <param name="tc">链接容器 TableCell</param>
        /// <param name="_url">页面链接地址</param>
        private void PagerDigitalLink(TableCell tc, string _url)
        {
            if (!IsShowDigitalPage)
            {
                return;
            }
            HyperLink hl = new HyperLink();
            string pageIndex = "1";
            int startPage = GetStartPage();
            int endPage = GetEndPage();
            for (int i = startPage; i <= endPage; i++)
            {
                pageIndex = String.Format("{0}", i + 1);
                hl = new HyperLink();
                hl.ID = "hl_" + i.ToString();
                if (i == this.CurrentPageIndex)
                {
                    hl.CssClass = "CurrentPage";

                }
                hl.NavigateUrl = StringHelper.UrlReplare(_url, this.PageTags, pageIndex);
                hl.Text = pageIndex;
                tc.Controls.Add(hl);
            }
        }

        /// <summary>
        /// 设置 下一页 和 末页 链接 
        /// </summary>
        /// <param name="tc">链接容器 TableCell</param>
        /// <param name="_url">页面链接地址</param>
        private void PagerLastAndNextLink(TableCell tc, string _url)
        {
            if (!IsShowNextPage)
            {
                return;
            }

            HyperLink Next = new HyperLink();
            HyperLink Last = new HyperLink();
            if (!String.IsNullOrEmpty(PagerSettings.NextPageImageUrl))
            {
                //Next.Text = "下一页";
                Next.Text = "<img src='" + ResolveUrl(PagerSettings.NextPageImageUrl) + "' border='0'/>";
            }
            else
            {
                Next.Text = PagerSettings.NextPageText;
            }
            Next.CssClass = "Next";
            if (!String.IsNullOrEmpty(PagerSettings.LastPageImageUrl))
            {
                //Last.Text = "末页";
                Last.Text = "<img src='" + ResolveUrl(PagerSettings.LastPageImageUrl) + "' border='0'/>";
            }
            else
            {
                Last.Text = PagerSettings.LastPageText;
            }
            Last.CssClass = "Last";
            if ((CurrentPageIndex + 1) < AllPageCount)
            {
                Next.NavigateUrl = StringHelper.UrlReplare(_url, this.PageTags, String.Format("{0}", CurrentPageIndex + 2));
                Last.NavigateUrl = StringHelper.UrlReplare(_url, this.PageTags, AllPageCount.ToString());
            }
            tc.Controls.Add(Next);
            tc.Controls.Add(Last);
        }

        /// <summary>
        /// 设置 页面信息
        /// </summary>
        /// <param name="tc">链接容器 TableCell</param>
        private void PagerInfo(TableCell tc)
        {
            if (!IsShowPagerInfo)
            {
                return;
            }
            tc.Controls.Add(new LiteralControl(String.Format("<a class='pageInfo'>共&nbsp;<b>{0}</b>&nbsp;条记录 ( 第&nbsp;<b>{1}</b>&nbsp;页/共&nbsp;<b>{2}</b>&nbsp;页 ) </a>", this.RecordCount, this.CurrentPageIndex + 1, this.AllPageCount)));
        }
        #endregion



    }
}
