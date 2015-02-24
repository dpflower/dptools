using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using DP.Common;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.IO;

namespace DP.Web.UI
{
    public static class PageHelper
    {
        #region 显示提示信息
        /// <summary>
        /// 显示提示信息框
        /// </summary>
        /// <param name="message">显示信息</param>
        public static void ShowMessage(string message)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                page.ClientScript.RegisterStartupScript(page.GetType(), "msg", "alert('" + StringHelper.ReplaceEnter(message) + "');", true);
            }
        }

        /// <summary>
        /// 显示提示信息框
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="message">显示信息</param>
        public static void ShowMessage(Page page, string message)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "msg", "alert('" + StringHelper.ReplaceEnter(message) + "');", true);
        }

        /// <summary>
        /// 显示提示信息框，并跳转页面。
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="message">显示信息</param>
        /// <param name="url">跳转页面</param>
        public static void ShowMessage(Page page, string message, string url)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "msg", "alert('" + StringHelper.ReplaceEnter(message) + "'); window.location.href='" + url + "'; ", true);
        }

        /// <summary>
        /// 显示提示信息框，并跳转页面。
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="message">提示信息内容</param>
        /// <param name="back">是否跳转回 上一页面。 true 则跳转回上一页面，，false 则不跳转</param>
        public static void ShowMessage(Page page, string message, bool back)
        {
            if (back)
            {
                object url = page.Header.Attributes[Constant.PrePageUrl];
                if (url != null)
                {
                    page.ClientScript.RegisterStartupScript(page.GetType(), "msg", "alert('" + StringHelper.ReplaceEnter(message) + "'); window.location.href='" + url.ToString() + "';", true);
                }
                else
                {
                    page.ClientScript.RegisterStartupScript(page.GetType(), "msg", "alert('" + StringHelper.ReplaceEnter(message) + "'); window.history.go(-1); ", true);
                }
            }
            else
            {
                page.ClientScript.RegisterStartupScript(page.GetType(), "msg", "alert('" + StringHelper.ReplaceEnter(message) + "');", true);
            }
        }

        /// <summary>
        /// 显示提示信息框
        /// </summary>
        /// <param name="control"></param>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public static void ShowMessage(Control control, Type type, string message)
        {
            ScriptManager.RegisterClientScriptBlock(control, type, "error", "alert('" + StringHelper.ReplaceEnter(message) + "'); ", true);
        }

        /// <summary>
        /// 显示提示信息框，并跳转页面。
        /// </summary>
        /// <param name="control"></param>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="url"></param>
        public static void ShowMessage(Control control, Type type, string message, string url)
        {
            ScriptManager.RegisterClientScriptBlock(control, type, "error", "alert('" + StringHelper.ReplaceEnter(message) + "'); window.location.href='" + url + "'; ", true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="closeSelf"></param>
        /// <param name="refreshOpener"></param>
        public static void ShowMessage(string message, bool closeSelf, bool refreshOpener)
        {
            string preCommand = "alert('" + StringHelper.ReplaceEnter(message) + "');";
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                if (refreshOpener)
                {
                    preCommand += "opener.location.href=opener.location.href;";
                }
                if (closeSelf)
                {
                    Close(preCommand);
                }
                else
                {
                    ExecuteJS(preCommand);
                }
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="refreshSelf"></param>
        public static void ShowMessage(string message, bool refreshSelf)
        {
            string preCommand = "alert('" + StringHelper.ReplaceEnter(message) + "');";
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                if (refreshSelf)
                {
                    Refresh(preCommand);
                }
                else
                {
                    ShowMessage(message);
                }
            }
        }
        #endregion  

        #region 关闭页面
        /// <summary>
        /// 关闭页面
        /// </summary>
        public static void Close()
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                page.ClientScript.RegisterStartupScript(page.GetType(), "close", "window.opener=null;window.open('','_self');window.close();", true);
            }
        }
        /// <summary>
        /// 关闭页面
        /// </summary>
        /// <param name="page">当前页面</param>
        public static void Close(Page page)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "close", "window.opener=null;window.open('','_self');window.close();", true);
        }
        /// <summary>
        /// 关闭页面  AJAX页面
        /// </summary>
        /// <param name="control"></param>
        public static void Close(Control control)
        {
            ScriptManager.RegisterClientScriptBlock(control, control.GetType(), "close", "window.opener=null;window.open('','_self');window.close();", true);
        }
        /// <summary>
        /// 关闭页面
        /// </summary>
        /// <param name="preCommand"></param>
        public static void Close(string preCommand)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                page.ClientScript.RegisterStartupScript(page.GetType(), "close", preCommand + "window.opener=null;window.open('','_self');window.close();", true);
            }
        }
        /// <summary>
        /// 执行JS 关闭页面
        /// </summary>
        /// <param name="preCommand">当前页面</param>
        /// <param name="page"></param>
        public static void Close(string preCommand, Page page)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "close", preCommand + "window.opener=null;window.open('','_self');window.close();", true);
        }
        /// <summary>
        /// 执行JS 关闭页面  AJAX页面
        /// </summary>
        /// <param name="preCommand"></param>
        /// <param name="control"></param>
        public static void Close(string preCommand, Control control)
        {
            ScriptManager.RegisterClientScriptBlock(control, control.GetType(), "close", preCommand + "window.opener=null;window.open('','_self');window.close();", true);
        } 
        #endregion

        #region 执行JS代码
        public static void ExecuteJS(string preCommand)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                page.ClientScript.RegisterStartupScript(page.GetType(), "execute", preCommand, true);
            }
        }
        /// <summary>
        /// 执行JS代码
        /// </summary>
        /// <param name="preCommand"></param>
        /// <param name="page"></param>
        public static void ExecuteJS(string preCommand, Page page)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "execute", preCommand , true);
        }
        /// <summary>
        /// 执行JS代码  AJAX页面
        /// </summary>
        /// <param name="preCommand"></param>
        /// <param name="control"></param>
        public static void ExecuteJS(string preCommand, Control control)
        {
            ScriptManager.RegisterClientScriptBlock(control, control.GetType(), "execute", preCommand, true);
        } 
        #endregion

        #region JS刷新
        /// <summary>
        /// 刷新当前页面
        /// </summary>
        public static void Refresh()
        {
            ExecuteJS("location.href=location.href;");
        }  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="preCommand"></param>
        public static void Refresh(string preCommand)
        {
            ExecuteJS(preCommand + " location.href=location.href;");
        }
        /// <summary>
        /// 刷新 Parent
        /// </summary>
        public static void RefreshParent()
        {
            ExecuteJS("parent.location.href=parent.location.href;");
        }
        /// <summary>
        /// 刷新 Parent
        /// </summary>
        public static void RefreshParent(string preCommand)
        {
            ExecuteJS(preCommand + "parent.location.href=parent.location.href;");
        }
        /// <summary>
        /// 刷新 Opener
        /// </summary>
        /// <param name="closeSelf"></param>
        public static void RefreshOpener(bool closeSelf)
        {
            if (closeSelf)
            {
                Close("opener.location.href=opener.location.href;");
            }
            else
            {
                ExecuteJS("opener.location.href=opener.location.href;");
            }
        }
        /// <summary>
        /// 刷新 Opener
        /// </summary>
        /// <param name="closeSelf"></param>
        public static void RefreshOpener(string preCommand, bool closeSelf)
        {
            if (closeSelf)
            {
                Close(preCommand + "opener.location.href=opener.location.href;");
            }
            else
            {
                ExecuteJS(preCommand + "opener.location.href=opener.location.href;");
            }
        }


        #endregion

        #region 打开新页面
        /// <summary>
        /// Opens the specified page URL.
        /// </summary>
        /// <param name="pageUrl">弹出窗口的文件名The page URL.</param>
        /// <param name="title">弹出窗口的名字The title.</param>
        public static void Open(string pageUrl, string title)
        {
            Open(pageUrl, title, "", "", "", "", false, false, true, false, false, true);
        }

        /// <summary>
        /// Opens the specified page URL.
        /// </summary>
        /// <param name="pageUrl">弹出窗口的文件名The page URL.</param>
        /// <param name="title">弹出窗口的名字The title.</param>
        /// <param name="height">窗口高度The height.</param>
        /// <param name="width">窗口宽度The width.</param>
        /// <param name="top">窗口距离屏幕上方的象素值The top.</param>
        /// <param name="left">窗口距离屏幕左侧的象素值The left.</param>
        public static void Open(string pageUrl, string title, string height, string width, string top, string left)
        {
            Open(pageUrl, title, height, width, top, left, false, false, true, false, false, true);
        }

        /// <summary>
        /// "window.open ('PupopSearchInsurance.aspx', 'newwindow', 'height=400, width=400, top=250, left=300, toolbar=no, menubar=no, scrollbars=no, resizable=no,location=no, status=no');"
        /// </summary>
        /// <param name="pageUrl">弹出窗口的文件名</param>
        /// <param name="title">弹出窗口的文件名</param>
        /// <param name="height">窗口高度</param>
        /// <param name="width">窗口宽度</param>
        /// <param name="top">窗口距离屏幕上方的象素值</param>
        /// <param name="left">窗口距离屏幕左侧的象素值 </param>
        /// <param name="toolbar">是否显示工具栏，True为显示</param>
        /// <param name="menubar">表示菜单栏</param>
        /// <param name="scrollbars">滚动栏</param>
        /// <param name="location">是否显示地址栏，True为允许</param>
        /// <param name="status">是否显示状态栏内的信息（通常是文件已经打开），True为允许</param>
        /// <param name="resizable">否允许改变窗口大小，True为允许</param>
        public static void Open(string pageUrl, string title, string height, string width, string top, string left, bool toolbar, bool menubar, bool scrollbars, bool location, bool status, bool resizable)
        {
            //  <SCRIPT LANGUAGE="javascript"> js脚本开始；
            //window.open 弹出新窗口的命令；
            //'page.html' 弹出窗口的文件名；
            //'newwindow' 弹出窗口的名字（不是文件名），非必须，可用空''代替；
            //height=100 窗口高度；
            //width=400 窗口宽度；
            //top=0 窗口距离屏幕上方的象素值；
            //left=0 窗口距离屏幕左侧的象素值；
            //toolbar=no 是否显示工具栏，yes为显示；
            //menubar，scrollbars 表示菜单栏和滚动栏。
            //resizable=no 是否允许改变窗口大小，yes为允许；
            //location=no 是否显示地址栏，yes为允许；
            //status=no 是否显示状态栏内的信息（通常是文件已经打开），yes为允许；
            //</SCRIPT>
            StringBuilder sbOpenString = new StringBuilder();
            sbOpenString.Append("window.open('").Append(pageUrl).Append("','").Append(title).Append("','");

            #region toolbar=no 是否显示工具栏，yes为显示；
            if (toolbar)
            {
                sbOpenString.Append("toolbar=").Append("yes");
            }
            else
            {
                sbOpenString.Append("toolbar=").Append("no");
            }
            #endregion

            #region menubar 表示菜单栏
            if (menubar)
            {
                sbOpenString.Append(",menubar=").Append("yes");
            }
            else
            {
                sbOpenString.Append(",menubar=").Append("no");
            }
            #endregion

            #region scrollbars 滚动栏。
            if (scrollbars)
            {
                sbOpenString.Append(",scrollbars=").Append("yes");
            }
            else
            {
                sbOpenString.Append(",scrollbars=").Append("no");
            }
            #endregion

            #region location=no 是否显示地址栏，yes为允许；
            if (location)
            {
                sbOpenString.Append(",location=").Append("yes");
            }
            else
            {
                sbOpenString.Append(",location=").Append("no");
            }
            #endregion

            #region status=no 是否显示状态栏内的信息（通常是文件已经打开），yes为允许；
            if (status)
            {
                sbOpenString.Append(",status=").Append("yes");
            }
            else
            {
                sbOpenString.Append(",status=").Append("no");
            }
            #endregion

            #region resizable=no 是否允许改变窗口大小，yes为允许；
            if (resizable)
            {
                sbOpenString.Append(",resizable=").Append("yes");
            }
            else
            {
                sbOpenString.Append(",resizable=").Append("no");
            }
            #endregion

            #region height 窗口高度；
            if (!String.IsNullOrEmpty(height))
            {
                sbOpenString.Append(",height=").Append(height);
            }
            #endregion

            #region width 窗口宽度；
            if (!String.IsNullOrEmpty(width))
            {
                sbOpenString.Append(",width=").Append(width);
            }
            #endregion

            #region top 窗口距离屏幕上方的象素值；
            if (!String.IsNullOrEmpty(top))
            {
                sbOpenString.Append(",top=").Append(top);
            }
            #endregion

            #region left 窗口距离屏幕左侧的象素值；
            if (!String.IsNullOrEmpty(left))
            {
                sbOpenString.Append(",left=").Append(left);
            }
            #endregion

            sbOpenString.Append("')");

            ExecuteJS(sbOpenString.ToString());

        }
        #endregion

        /// <summary>
        /// 注册  MaskTextBox.js 到系统。
        /// </summary>
        /// <param name="page"></param>
        public static void RegisterMaskTextBoxJS()
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                page.ClientScript.RegisterClientScriptResource(page.GetType(), "DP.Resources.JavaScript.MaskTextBox.js");
            }
        }

        #region 设置返回按钮的上次访问页面地址
        /// <summary>
        /// 设置后退按钮 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="btn"></param>
        public static void SetGoBackUrl(System.Web.UI.HtmlControls.HtmlInputButton btn)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                if (!page.IsPostBack)
                {
                    if (btn == null)
                    {
                        return;
                    }
                    if (page.Request.UrlReferrer == null)
                    {
                        btn.Attributes.Add("onclick", "window.history.go(-1);");
                    }
                    else
                    {
                        btn.Attributes.Add("onclick", "window.location.href='" + InitChineseUrl(page.Request.UrlReferrer) + "'");
                    }
                }
            }
        }
        /// <summary>
        /// 设置后退按钮 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="btn"></param>
        public static void SetGoBackUrl(Page page, System.Web.UI.HtmlControls.HtmlInputButton btn)
        {
            if (!page.IsPostBack)
            {
                if (btn == null)
                {
                    return;
                }
                if (page.Request.UrlReferrer == null)
                {
                    btn.Attributes.Add("onclick", "window.history.go(-1);");
                }
                else
                {
                    btn.Attributes.Add("onclick", "window.location.href='" + InitChineseUrl(page.Request.UrlReferrer) + "'");
                }
            }
        }
        /// <summary>
        /// 设置后退按钮 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="btn"></param>
        public static void SetGoBackUrl(System.Web.UI.HtmlControls.HtmlButton btn)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                if (!page.IsPostBack)
                {
                    if (btn == null)
                    {
                        return;
                    }
                    if (page.Request.UrlReferrer == null)
                    {
                        btn.Attributes.Add("onclick", "window.history.go(-1);");
                    }
                    else
                    {
                        btn.Attributes.Add("onclick", "window.location.href='" + InitChineseUrl(page.Request.UrlReferrer) + "'");
                    }
                }
            }
        }
        /// <summary>
        /// 设置后退按钮 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="btn"></param>
        public static void SetGoBackUrl(Page page, System.Web.UI.HtmlControls.HtmlButton btn)
        {
            if (!page.IsPostBack)
            {
                if (btn == null)
                {
                    return;
                }
                if (page.Request.UrlReferrer == null)
                {
                    btn.Attributes.Add("onclick", "window.history.go(-1);");
                }
                else
                {
                    btn.Attributes.Add("onclick", "window.location.href='" + InitChineseUrl(page.Request.UrlReferrer) + "'");
                }
            }
        }
        /// <summary>
        /// 设置后退按钮 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="btn"></param>
        public static void SetGoBackUrl(System.Web.UI.HtmlControls.HtmlContainerControl btn)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                if (!page.IsPostBack)
                {
                    if (btn == null)
                    {
                        return;
                    }
                    if (page.Request.UrlReferrer == null)
                    {
                        btn.Attributes.Add("onclick", "window.history.go(-1);");
                    }
                    else
                    {
                        btn.Attributes.Add("onclick", "window.location.href='" + InitChineseUrl(page.Request.UrlReferrer) + "'");
                    }
                }
            }
        } 
        /// <summary>
        /// 设置后退按钮 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="btn"></param>
        public static void SetGoBackUrl(Page page, System.Web.UI.HtmlControls.HtmlContainerControl btn)
        {
            if (!page.IsPostBack)
            {
                if (btn == null)
                {
                    return;
                }
                if (page.Request.UrlReferrer == null)
                {
                    btn.Attributes.Add("onclick", "window.history.go(-1);");
                }
                else
                {
                    btn.Attributes.Add("onclick", "window.location.href='" + InitChineseUrl(page.Request.UrlReferrer) + "'");
                }
            }
        }

        /// <summary>
        /// 设置后退按钮 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="btn"></param>
        public static void SetGoBackUrl(System.Web.UI.HtmlControls.HtmlInputButton btn, string[] NotBackUrl)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                if (!page.IsPostBack)
                {
                    if (btn == null)
                    {
                        return;
                    }
                    if (page.Request.UrlReferrer == null)
                    {
                        btn.Attributes.Add("onclick", "window.history.go(-1);");
                        return;
                    }
                    foreach (string s in NotBackUrl)
                    {
                        if (page.Request.UrlReferrer.PathAndQuery.ToLower().IndexOf(s.ToLower()) >= 0)
                        {
                            btn.Attributes.Add("onclick", "window.history.go(-1);");
                            return;
                        }
                    }
                    btn.Attributes.Add("onclick", "window.location.href='" + InitChineseUrl(page.Request.UrlReferrer) + "'");
                }
            }
        }
        /// <summary>
        /// 设置后退按钮 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="btn"></param>
        public static void SetGoBackUrl(Page page, System.Web.UI.HtmlControls.HtmlInputButton btn, string[] NotBackUrl)
        {
            if (!page.IsPostBack)
            {
                if (btn == null)
                {
                    return;
                }
                if (page.Request.UrlReferrer == null)
                {
                    btn.Attributes.Add("onclick", "window.history.go(-1);");
                    return;
                }
                foreach (string s in NotBackUrl)
                {
                    if (page.Request.UrlReferrer.PathAndQuery.ToLower().IndexOf(s.ToLower()) >= 0)
                    {
                        btn.Attributes.Add("onclick", "window.history.go(-1);");
                        return;
                    }
                }
                btn.Attributes.Add("onclick", "window.location.href='" + InitChineseUrl(page.Request.UrlReferrer) + "'");
            }
        }
        /// <summary>
        /// 设置后退按钮 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="btn"></param>
        public static void SetGoBackUrl(System.Web.UI.HtmlControls.HtmlButton btn, string[] NotBackUrl)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                if (!page.IsPostBack)
                {
                    if (btn == null)
                    {
                        return;
                    }
                    if (page.Request.UrlReferrer == null)
                    {
                        btn.Attributes.Add("onclick", "window.history.go(-1);");
                        return;
                    }
                    foreach (string s in NotBackUrl)
                    {
                        if (page.Request.UrlReferrer.PathAndQuery.ToLower().IndexOf(s.ToLower()) >= 0)
                        {
                            btn.Attributes.Add("onclick", "window.history.go(-1);");
                            return;
                        }
                    }
                    btn.Attributes.Add("onclick", "window.location.href='" + InitChineseUrl(page.Request.UrlReferrer) + "'");
                }
            }
        }
        /// <summary>
        /// 设置后退按钮 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="btn"></param>
        public static void SetGoBackUrl(Page page, System.Web.UI.HtmlControls.HtmlButton btn, string[] NotBackUrl)
        {
            if (!page.IsPostBack)
            {
                if (btn == null)
                {
                    return;
                }
                if (page.Request.UrlReferrer == null)
                {
                    btn.Attributes.Add("onclick", "window.history.go(-1);");
                    return;
                }
                foreach (string s in NotBackUrl)
                {
                    if (page.Request.UrlReferrer.PathAndQuery.ToLower().IndexOf(s.ToLower()) >= 0)
                    {
                        btn.Attributes.Add("onclick", "window.history.go(-1);");
                        return;
                    }
                }
                btn.Attributes.Add("onclick", "window.location.href='" + InitChineseUrl(page.Request.UrlReferrer) + "'");
            }
        }
        /// <summary>
        /// 设置后退按钮 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="btn"></param>
        public static void SetGoBackUrl(System.Web.UI.HtmlControls.HtmlContainerControl btn, string[] NotBackUrl)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                if (!page.IsPostBack)
                {
                    if (btn == null)
                    {
                        return;
                    }
                    if (page.Request.UrlReferrer == null)
                    {
                        btn.Attributes.Add("onclick", "window.history.go(-1);");
                        return;
                    }
                    foreach (string s in NotBackUrl)
                    {
                        if (page.Request.UrlReferrer.PathAndQuery.ToLower().IndexOf(s.ToLower()) >= 0)
                        {
                            btn.Attributes.Add("onclick", "window.history.go(-1);");
                            return;
                        }
                    }
                    btn.Attributes.Add("onclick", "window.location.href='" + InitChineseUrl(page.Request.UrlReferrer) + "'");
                }
            }
        }
        /// <summary>
        /// 设置后退按钮 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="btn"></param>
        public static void SetGoBackUrl(Page page, System.Web.UI.HtmlControls.HtmlContainerControl btn, string[] NotBackUrl)
        {
            if (!page.IsPostBack)
            {
                if (btn == null)
                {
                    return;
                }
                if (page.Request.UrlReferrer == null)
                {
                    btn.Attributes.Add("onclick", "window.history.go(-1);");
                    return;
                }
                foreach (string s in NotBackUrl)
                {
                    if (page.Request.UrlReferrer.PathAndQuery.ToLower().IndexOf(s.ToLower()) >= 0)
                    {
                        btn.Attributes.Add("onclick", "window.history.go(-1);");
                        return;
                    }
                }
                btn.Attributes.Add("onclick", "window.location.href='" + InitChineseUrl(page.Request.UrlReferrer) + "'");
            }
        } 

        #endregion

        #region 设置获取上次访问的页面地址
        /// <summary>
        /// 设置上一次页面地址
        /// </summary>
        /// <param name="page"></param>
        public static void SetPrePageUrl()
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                if (!page.IsPostBack)
                {
                    if (page.Request.UrlReferrer == null)
                    {
                        return;
                    }
                    else
                    {
                        page.Header.Attributes[Constant.PrePageUrl] = InitChineseUrl(page.Request.UrlReferrer);
                    }
                }
            }
        }
        /// <summary>
        /// 设置上一次页面地址
        /// </summary>
        /// <param name="page"></param>
        public static void SetPrePageUrl(Page page)
        {
            if (!page.IsPostBack)
            {
                if (page.Request.UrlReferrer == null)
                {
                    return;
                }
                else
                {
                    page.Header.Attributes[Constant.PrePageUrl] = InitChineseUrl(page.Request.UrlReferrer);
                }
            }
        }

        /// <summary>
        /// 获取上一次页面地址
        /// </summary>
        /// <param name="page"></param>
        public static string GetPrePageUrl()
        {
            string _PrePageUrl = String.Empty;
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                _PrePageUrl = String.IsNullOrEmpty(page.Header.Attributes[Constant.PrePageUrl]) ? "" : page.Header.Attributes[Constant.PrePageUrl].ToString();
                _PrePageUrl = _PrePageUrl.ToUpper().Contains("LOGOUT.ASPX") ? "" : _PrePageUrl;
                _PrePageUrl = _PrePageUrl.ToUpper().Contains("LOGIN.ASPX") ? "" : _PrePageUrl;
               
            } 
            return _PrePageUrl;
        }

        /// <summary>
        /// 获取上一次页面地址
        /// </summary>
        /// <param name="page"></param>
        public static string GetPrePageUrl(Page page)
        {
            string _PrePageUrl = String.IsNullOrEmpty(page.Header.Attributes[Constant.PrePageUrl]) ? "" : page.Header.Attributes[Constant.PrePageUrl].ToString();
            _PrePageUrl = _PrePageUrl.ToUpper().Contains("LOGOUT.ASPX") ? "" : _PrePageUrl;
            _PrePageUrl = _PrePageUrl.ToUpper().Contains("LOGIN.ASPX") ? "" : _PrePageUrl;
            return _PrePageUrl;
        }

        public static string InitChineseUrl(Uri chineseUri)
        {
            System.Collections.Specialized.NameValueCollection nv = System.Web.HttpUtility.ParseQueryString(chineseUri.Query, System.Text.Encoding.GetEncoding("utf-8"));
            string query = "";
            for (int i = 0; i < nv.Count; i++)
            {
                if (query.Trim() == string.Empty)
                {
                    query = "?" + nv.Keys[i] + "=" + HttpUtility.UrlEncode(nv[i], System.Text.Encoding.GetEncoding("utf-8"));
                }
                else
                {
                    query += "&" + nv.Keys[i] + "=" + HttpUtility.UrlEncode(nv[i], System.Text.Encoding.GetEncoding("utf-8"));
                }
            }
            string u = chineseUri.ToString().Split('?')[0] + query;
            return u;
        }

        public static string InitChineseUrl(string chineseUrl)
        {
            Uri url = new Uri(chineseUrl);
            System.Collections.Specialized.NameValueCollection nv = System.Web.HttpUtility.ParseQueryString(url.Query, System.Text.Encoding.GetEncoding("utf-8"));
            string query = "";
            for (int i = 0; i < nv.Count; i++)
            {
                if (query.Trim() == string.Empty)
                {
                    query = "?" + nv.Keys[i] + "=" + HttpUtility.UrlEncode(nv[i], System.Text.Encoding.GetEncoding("utf-8"));
                }
                else
                {
                    query += "&" + nv.Keys[i] + "=" + HttpUtility.UrlEncode(nv[i], System.Text.Encoding.GetEncoding("utf-8"));
                }
            }
            string u = chineseUrl.Split('?')[0] + query;
            return u;
        } 
        #endregion

        #region 设置关闭按钮
        /// <summary>
        /// 设置关闭按钮 
        /// </summary>
        /// <param name="btn">The BTN.</param>
        public static void SetClose(System.Web.UI.HtmlControls.HtmlInputButton btn)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                SetClose(page, btn);
            }
        }


        /// <summary>
        /// 设置关闭按钮
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="btn">The BTN.</param>
        public static void SetClose(Page page, System.Web.UI.HtmlControls.HtmlInputButton btn)
        {
            if (btn == null)
            {
                return;
            }
            if (page.Request.UrlReferrer == null)
            {
                btn.Attributes.Add("onclick", "if(window.opener != null && window.opener != undefined ) { window.opener=null;window.open('','_self');window.close(); } else { window.history.go(-1); } ");
            }
            else
            {
                btn.Attributes.Add("onclick", "if(window.opener != null && window.opener != undefined ) { window.opener=null;window.open('','_self');window.close(); } else { window.location.href='" + page.Request.UrlReferrer.ToString() + "'; } ");
            }
        } 
        #endregion

        #region 设置重置按钮
        /// <summary>
        /// 设置重置按钮
        /// </summary>
        /// <param name="btn">The BTN.</param>
        public static void SetFormReset(System.Web.UI.HtmlControls.HtmlInputButton btn)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                SetFormReset(page, btn);
            }
        }

        /// <summary>
        /// 设置重置按钮
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="btn">The BTN.</param>
        public static void SetFormReset(Page page, System.Web.UI.HtmlControls.HtmlInputButton btn)
        {
            if (btn == null)
            {
                return;
            }
            btn.Attributes.Add("onclick", "form.reset(); return false; ");
        } 
        #endregion

        #region 设置Open新页面按钮
        /// <summary>
        /// 设置Open新页面按钮
        /// </summary>
        /// <param name="btn">The BTN.</param>
        /// <param name="page">The page.</param>
        /// <param name="title">The title.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="top">The top.</param>
        /// <param name="left">The left.</param>
        /// <param name="toolbar">if set to <c>true</c> [toolbar].</param>
        /// <param name="menubar">if set to <c>true</c> [menubar].</param>
        /// <param name="scrollbars">if set to <c>true</c> [scrollbars].</param>
        /// <param name="location">if set to <c>true</c> [location].</param>
        /// <param name="status">if set to <c>true</c> [status].</param>
        /// <param name="resizable">if set to <c>true</c> [resizable].</param>
        public static void SetOpenPage(System.Web.UI.HtmlControls.HtmlInputButton btn, string pageUrl, string title, string height, string width, string top, string left, bool toolbar, bool menubar, bool scrollbars, bool location, bool status, bool resizable)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                SetOpenPage(page, btn, pageUrl, title, height, width, top, left, toolbar, menubar, scrollbars, location, status, resizable);
            }
        }

        /// <summary>
        /// 设置Open新页面按钮
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="btn">The BTN.</param>
        /// <param name="page">The page.</param>
        /// <param name="title">The title.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="top">The top.</param>
        /// <param name="left">The left.</param>
        /// <param name="toolbar">if set to <c>true</c> [toolbar].</param>
        /// <param name="menubar">if set to <c>true</c> [menubar].</param>
        /// <param name="scrollbars">if set to <c>true</c> [scrollbars].</param>
        /// <param name="location">if set to <c>true</c> [location].</param>
        /// <param name="status">if set to <c>true</c> [status].</param>
        /// <param name="resizable">if set to <c>true</c> [resizable].</param>
        public static void SetOpenPage(Page page, System.Web.UI.HtmlControls.HtmlInputButton btn, string pageUrl, string title, string height, string width, string top, string left, bool toolbar, bool menubar, bool scrollbars, bool location, bool status, bool resizable)
        {
            StringBuilder sbOpenString = new StringBuilder();
            sbOpenString.Append("window.open('").Append(pageUrl).Append("','").Append(title).Append("','");

            #region toolbar=no 是否显示工具栏，yes为显示；
            if (toolbar)
            {
                sbOpenString.Append("toolbar=").Append("yes");
            }
            else
            {
                sbOpenString.Append("toolbar=").Append("no");
            }
            #endregion

            #region menubar 表示菜单栏
            if (menubar)
            {
                sbOpenString.Append(",menubar=").Append("yes");
            }
            else
            {
                sbOpenString.Append(",menubar=").Append("no");
            }
            #endregion

            #region scrollbars 滚动栏。
            if (scrollbars)
            {
                sbOpenString.Append(",scrollbars=").Append("yes");
            }
            else
            {
                sbOpenString.Append(",scrollbars=").Append("no");
            }
            #endregion

            #region location=no 是否显示地址栏，yes为允许；
            if (location)
            {
                sbOpenString.Append(",location=").Append("yes");
            }
            else
            {
                sbOpenString.Append(",location=").Append("no");
            }
            #endregion

            #region status=no 是否显示状态栏内的信息（通常是文件已经打开），yes为允许；
            if (status)
            {
                sbOpenString.Append(",status=").Append("yes");
            }
            else
            {
                sbOpenString.Append(",status=").Append("no");
            }
            #endregion

            #region resizable=no 是否允许改变窗口大小，yes为允许；
            if (resizable)
            {
                sbOpenString.Append(",resizable=").Append("yes");
            }
            else
            {
                sbOpenString.Append(",resizable=").Append("no");
            }
            #endregion

            #region height 窗口高度；
            if (!String.IsNullOrEmpty(height))
            {
                sbOpenString.Append(",height=").Append(height);
            }
            #endregion

            #region width 窗口宽度；
            if (!String.IsNullOrEmpty(width))
            {
                sbOpenString.Append(",width=").Append(width);
            }
            #endregion

            #region top 窗口距离屏幕上方的象素值；
            if (!String.IsNullOrEmpty(top))
            {
                sbOpenString.Append(",top=").Append(top);
            }
            #endregion

            #region left 窗口距离屏幕左侧的象素值；
            if (!String.IsNullOrEmpty(left))
            {
                sbOpenString.Append(",left=").Append(left);
            }
            #endregion

            sbOpenString.Append("')");

            btn.Attributes.Add("onclick", sbOpenString.ToString());

        }
        
        #endregion

        #region 设置弹出日期控件
        /// <summary>
        /// 控件 弹出 日历的绑定
        /// </summary>
        /// <param name="txt">The TXT.</param>
        public static void SetPopCalender(System.Web.UI.WebControls.TextBox txt)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                txt.Attributes.Add("onFocus", "WdatePicker({isShowClear:true,errDealMode:1})");
            }
        }

        /// <summary>
        /// 控件 弹出 日历的绑定
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="txt">The TXT.</param>
        public static void SetPopCalender(Page page, System.Web.UI.WebControls.TextBox txt)
        {
            txt.Attributes.Add("onFocus", "WdatePicker({isShowClear:true,errDealMode:1})");
        }

        /// <summary>
        /// 控件 弹出 日历的绑定
        /// </summary>
        /// <param name="txt">The TXT.</param>
        /// <param name="format">The format.</param>
        public static void SetPopCalender(System.Web.UI.WebControls.TextBox txt, string format)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                txt.Attributes.Add("onFocus", "WdatePicker({isShowClear:true,errDealMode:1,dateFmt:'" + format + "'})");
            }
        }

        /// <summary>
        /// 控件 弹出 日历的绑定
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="txt">The TXT.</param>
        /// <param name="format">The format.</param>
        public static void SetPopCalender(Page page, System.Web.UI.WebControls.TextBox txt, string format)
        {
            txt.Attributes.Add("onFocus", "WdatePicker({isShowClear:true,errDealMode:1,dateFmt:'" + format + "'})");
        }


        /// <summary>
        /// 控件 弹出 日历的绑定
        /// </summary>
        /// <param name="txt">The TXT.</param>
        /// <param name="format">The format.</param>
        /// <param name="quickSel">The quick sel.</param>
        public static void SetPopCalender(System.Web.UI.WebControls.TextBox txt, string format, string quickSel)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                txt.Attributes.Add("onFocus", "WdatePicker({isShowClear:true,errDealMode:1,dateFmt:'" + format + "',qsEnabled:true,quickSel:[" + quickSel + "]})");
            }
        }

        /// <summary>
        /// 控件 弹出 日历的绑定
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="txt">The TXT.</param>
        /// <param name="format">The format.</param>
        /// <param name="quickSel">The quick sel.</param>
        public static void SetPopCalender(Page page, System.Web.UI.WebControls.TextBox txt, string format, string quickSel)
        {
            txt.Attributes.Add("onFocus", "WdatePicker({isShowClear:true,errDealMode:1,dateFmt:'" + format + "',qsEnabled:true,quickSel:[" + quickSel + "]})");
        }

        /// <summary>
        /// 控件 弹出 日历的绑定
        /// </summary>
        /// <param name="txt">The TXT.</param>
        /// <param name="format">The format.</param>
        /// <param name="quickSel">The quick sel.</param>
        /// <param name="minDate">The min date.</param>
        /// <param name="maxDate">The max date.</param>
        public static void SetPopCalender(System.Web.UI.WebControls.TextBox txt, string format, string quickSel, string minDate, string maxDate)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                txt.Attributes.Add("onFocus", "WdatePicker({isShowClear:true,errDealMode:1,dateFmt:'" + format + "',qsEnabled:true,quickSel:[" + quickSel + "],minDate:'" + minDate + "',maxDate:'" + maxDate + "'})");
            }
        }


        /// <summary>
        /// 控件 弹出 日历的绑定
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="txt">The TXT.</param>
        /// <param name="format">The format.</param>
        /// <param name="quickSel">The quick sel.</param>
        /// <param name="minDate">The min date.</param>
        /// <param name="maxDate">The max date.</param>
        public static void SetPopCalender(Page page, System.Web.UI.WebControls.TextBox txt, string format, string quickSel, string minDate, string maxDate)
        {
            txt.Attributes.Add("onFocus", "WdatePicker({isShowClear:true,errDealMode:1,dateFmt:'" + format + "',qsEnabled:true,quickSel:[" + quickSel + "],minDate:'" + minDate + "',maxDate:'" + maxDate + "'})");
        }
        
        #endregion

        #region 设置控件JS执行事件
        /// <summary>
        /// 设置 客户端事件
        /// </summary>
        /// <param name="c"></param>
        /// <param name="ClientFunctionName"></param>
        public static void SetOnClientClick(System.Web.UI.WebControls.WebControl c, string ClientFunctionName)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                c.Attributes.Add("onclick", ClientFunctionName);
            }
        }
        /// <summary>
        /// 设置 客户端事件
        /// </summary>
        /// <param name="page"></param>
        /// <param name="c"></param>
        /// <param name="ClientFunctionName"></param>
        public static void SetOnClientClick(Page page, System.Web.UI.WebControls.WebControl c, string ClientFunctionName)
        {
            c.Attributes.Add("onclick", ClientFunctionName);
        }
        /// <summary>
        /// 设置 客户端改变 事件
        /// </summary>
        /// <param name="page"></param>
        /// <param name="c"></param>
        /// <param name="ClientFunctionName"></param>
        public static void SetOnClientChanged(System.Web.UI.WebControls.WebControl c, string ClientFunctionName)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                c.Attributes.Add("onchange", ClientFunctionName);
            }
        }
        /// <summary>
        /// 设置 客户端改变 事件
        /// </summary>
        /// <param name="page"></param>
        /// <param name="c"></param>
        /// <param name="ClientFunctionName"></param>
        public static void SetOnClientChanged(Page page, System.Web.UI.WebControls.WebControl c, string ClientFunctionName)
        {
            c.Attributes.Add("onchange", ClientFunctionName);
        }
        
        #endregion

        #region 设置控件属性
        /// <summary>
        /// 设置 控件  Enabled 属性
        /// </summary>
        /// <param name="control"></param>
        /// <param name="b"></param>
        public static void SetControlEnabled(Control control, bool b)
        {
            foreach (Control c in control.Controls)
            {
                if (c is TextBox)
                {
                    (c as TextBox).Enabled = b;
                }
                else if (c is DropDownList)
                {
                    (c as DropDownList).Enabled = b;
                }
                else if (c is CheckBox)
                {
                    (c as CheckBox).Enabled = b;
                }
                else if (c is RadioButton)
                {
                    (c as RadioButton).Enabled = b;
                }
                else if (c.HasControls())
                {
                    SetControlEnabled(c, b);
                }
            }
        }

        /// <summary>
        /// 设置 控件 TextBox  Write 属性
        /// </summary>
        /// <param name="control"></param>
        /// <param name="b"></param>
        public static void SetControlWrite(Control control, bool b)
        {
            foreach (Control c in control.Controls)
            {
                if (c is TextBox)
                {
                    (c as TextBox).ReadOnly = !b;
                }
                else if (c is DropDownList)
                {
                    (c as DropDownList).Enabled = b;
                }
                else if (c is CheckBox)
                {
                    (c as CheckBox).Enabled = b;
                }
                else if (c is RadioButton)
                {
                    (c as RadioButton).Enabled = b;
                }
                else if (c.HasControls())
                {
                    SetControlEnabled(c, b);
                }
            }
        }
        #endregion

        /// <summary>
        /// 获取链接地址内容 存入 控件
        /// TextBox、DropDownList、RadioButton、CheckBox、
        /// </summary>
        /// <param name="control">父容器</param>
        public static void GetUrlQueryToControl(Control control)
        {
            Page page = System.Web.HttpContext.Current.Handler as Page;
            if (page == null)
            {
                return;
            }
            string _id = string.Empty;
            foreach (Control c in control.Controls)
            {
                if (c is TextBox)
                {
                    TextBox txt = c as TextBox;
                    if (txt.ID.IndexOf("txt") == 0)
                    {
                        _id = txt.ID.Remove(0, 3);
                    }
                    if (!String.IsNullOrEmpty(page.Request.QueryString[_id]))
                    {
                        txt.Text = page.Request.QueryString[_id].Trim();
                    }
                }
                else if (c is DropDownList)
                {
                    DropDownList ddl = c as DropDownList;
                    if (ddl.ID.IndexOf("ddl") == 0)
                    {
                        _id = ddl.ID.Remove(0, 3);
                    }
                    if (!String.IsNullOrEmpty(page.Request.QueryString[_id]))
                    {
                        ddl.SelectedValue = page.Request.QueryString[_id].Trim();
                    }
                }
                else if (c is RadioButton)
                {
                    RadioButton rb = c as RadioButton;
                    if (rb.GroupName.IndexOf("rb") == 0)
                    {
                        _id = rb.GroupName.Remove(0, 2);
                    }
                    if (!String.IsNullOrEmpty(page.Request.QueryString[_id]))
                    {
                        if (rb.ID.ToUpper().Equals((rb.GroupName + page.Request.QueryString[_id].Trim()).ToUpper()))
                        {
                            rb.Checked = true;
                        }
                        else
                        {
                            rb.Checked = false;
                        }
                    }
                }
                else if (c is CheckBox)
                {
                    CheckBox cb = c as CheckBox;
                    if (cb.ValidationGroup.IndexOf("cb") == 0)
                    {
                        _id = cb.ValidationGroup.Remove(0, 2);
                    }
                    if (!String.IsNullOrEmpty(page.Request.QueryString[_id]))
                    {
                        string[] cbs = page.Request.QueryString[_id].Trim().Split(',');
                        foreach (string s in cbs)
                        {
                            if (cb.ID.ToUpper().Equals((cb.ValidationGroup + s.Trim()).ToUpper()))
                            {
                                cb.Checked = true;
                            }
                        }
                    }
                }
                else if (c.HasControls())
                {
                    GetUrlQueryToControl(c);
                }
            }
        }

        /// <summary>
        /// 获取界面控件内容 传入 连接地址
        /// TextBox、DropDownList、RadioButton、CheckBox、
        /// </summary>
        /// <param name="control">父容器</param>
        /// <param name="url"></param>
        public static void GetControlToUrlQuery(Control control, ref string url)
        {
            Page page = System.Web.HttpContext.Current.Handler as Page;
            if (page == null)
            {
                return;
            }
            string _id = string.Empty;
            foreach (Control c in control.Controls)
            {
                if (c is TextBox)
                {
                    TextBox txt = c as TextBox;
                    if (txt.ID.IndexOf("txt") == 0)
                    {
                        _id = txt.ID.Remove(0, 3);
                    }
                    else
                    {
                        _id = txt.ID;
                    }
                    url = StringHelper.UrlReplare(url, _id, txt.Text.Trim());
                }
                else if (c is DropDownList)
                {
                    DropDownList ddl = c as DropDownList;
                    if (ddl.ID.IndexOf("ddl") == 0)
                    {
                        _id = ddl.ID.Remove(0, 3);
                    }
                    else
                    {
                        _id = ddl.ID;
                    }
                    url = StringHelper.UrlReplare(url, _id, ddl.SelectedValue.Trim());
                }
                else if (c is RadioButton)
                {
                    RadioButton rb = c as RadioButton;
                    if (rb.GroupName.IndexOf("rb") == 0)
                    {
                        _id = rb.GroupName.Remove(0, 2);
                    }
                    else
                    {
                        _id = rb.GroupName;
                    }
                    if (rb.Checked)
                    {
                        url = StringHelper.UrlReplare(url, _id, rb.ID.ToUpper().Replace(rb.GroupName.ToUpper(), ""));
                    }
                }
                else if (c is CheckBox)
                {
                    CheckBox cb = c as CheckBox;
                    if (String.IsNullOrEmpty(cb.ValidationGroup))
                    {
                        continue;
                    }
                    StringBuilder sb = CacheHelper.GetPageItem(page, cb.ValidationGroup.Trim()) as StringBuilder;
                    if (sb == null)
                    {
                        sb = new StringBuilder();
                    }
                    if (cb.ValidationGroup.IndexOf("cb") == 0)
                    {
                        _id = cb.ValidationGroup.Remove(0, 2);
                    }
                    else
                    {
                        _id = cb.ValidationGroup;
                    }
                    if (cb.Checked)
                    {
                       if(sb.Length > 0)
                       {
                           sb.Append(",");
                       }
                       sb.Append(cb.ID.ToUpper().Replace(cb.ValidationGroup.ToUpper(), ""));
                    }
                    CacheHelper.SetPageItem(page, cb.ValidationGroup.Trim(), sb);
                    url = StringHelper.UrlReplare(url, _id, sb.ToString());
                }
                else if (c.HasControls())
                {
                    GetControlToUrlQuery(c, ref url);
                }
            }

        }

        /// <summary>
        /// 获取控件内容，存入Hashtable中。
        /// Key 小写字符
        /// </summary>
        /// <param name="control">父容器</param>
        /// <param name="ht">Hashtable Key 小写字符</param>
        public static void GetControlValue(Control control, ref Hashtable ht)
        {
            Page page = System.Web.HttpContext.Current.Handler as Page;
            if (page == null)
            {
                return;
            }
            string _id = string.Empty;
            foreach (Control c in control.Controls)
            {
                if (c is TextBox)
                {
                    TextBox txt = c as TextBox;
                    if (txt.ID.IndexOf("txt") == 0)
                    {
                        _id = txt.ID.Remove(0, 3);
                    }
                    try
                    {
                        ht.Remove(_id.ToLower());
                        ht.Add(_id.ToLower(), txt.Text.Trim());
                    }
                    catch(Exception ex)
                    {
                        LogHelper.WriteLog("Log_DP_Control", ex.Message);
                    }
                }
                else if (c is DropDownList)
                {
                    DropDownList ddl = c as DropDownList;
                    if (ddl.ID.IndexOf("ddl") == 0)
                    {
                        _id = ddl.ID.Remove(0, 3);
                    }
                    try
                    {
                        ht.Remove(_id.ToLower());
                        ht.Add(_id.ToLower(), ddl.SelectedValue.Trim());
                     }
                    catch(Exception ex)
                    {
                        LogHelper.WriteLog("Log_DP_Control", ex.Message);
                    }
                }
                else if (c is RadioButton)
                {
                    RadioButton rb = c as RadioButton;
                    if (rb.GroupName.IndexOf("rb") == 0)
                    {
                        _id = rb.GroupName.Remove(0, 2);
                    }
                    StringBuilder sb = CacheHelper.GetPageItem(page, _id) as StringBuilder;
                    if (sb == null)
                    {
                        sb = new StringBuilder();
                    }
                    if (rb.Checked)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(",");
                        }
                        sb.Append(rb.ID.ToUpper().Replace(rb.GroupName.ToUpper(), ""));
                    }
                    CacheHelper.SetPageItem(page, _id, sb);
                    ht.Remove(_id.ToLower());
                    ht.Add(_id.ToLower(), sb.ToString());

                }
                else if (c is CheckBox)
                {
                    CheckBox cb = c as CheckBox;
                    if (cb.ValidationGroup.IndexOf("cb") == 0)
                    {
                        _id = cb.ValidationGroup.Remove(0, 2);
                    }
                    StringBuilder sb = CacheHelper.GetPageItem(page, _id) as StringBuilder;
                    if (sb == null)
                    {
                        sb = new StringBuilder();
                    }
                    if (cb.Checked)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(",");
                        }
                        sb.Append(cb.ID.ToUpper().Replace(cb.ValidationGroup.ToUpper(), ""));
                    }
                    CacheHelper.SetPageItem(page, _id, sb);
                    ht.Remove(_id.ToLower());
                    ht.Add(_id.ToLower(), sb.ToString());
                }
                else if (c.HasControls())
                {
                    GetControlValue(c, ref ht);
                }
            }
        }

        /// <summary>
        /// 获取控件内容，存入实体类 T  obj 中。
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="control">父容器</param>
        /// <param name="obj">实体类对象</param>
        public static string GetControlValue<T>(Control control, ref T obj)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Hashtable ht = new Hashtable();
            GetControlValue(control, ref ht);

            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (ht.ContainsKey(property.Name.ToLower()))
                {
                    try
                    {
                        Object objValue = property.GetValue(obj, null);
                        if (objValue != null)
                        {
                            DateTime dateTime = DateTime.Now;
                            if (DateTime.TryParse(objValue.ToString(), out dateTime))
                            {
                                if (dateTime.ToString("yyyy-MM-dd HH:mm:ss") != ht[property.Name.ToLower()].ToString())
                                {
                                    property.SetValue(obj, ReflectionHelper.ChangeType(ht[property.Name.ToLower()].ToString(), property.PropertyType), null);
                                    stringBuilder.Append(property.Name).Append(":")
                                        .Append(dateTime.ToString("yyyy-MM-dd HH:mm:ss"))
                                        .Append("->")
                                        .Append(ht[property.Name.ToLower()].ToString())
                                        .Append("|");
                                }
                            }
                            else
                            {
                                if (objValue.ToString().Trim() != ht[property.Name.ToLower()].ToString().Trim())
                                {
                                    property.SetValue(obj, ReflectionHelper.ChangeType(ht[property.Name.ToLower()].ToString(), property.PropertyType), null);
                                    stringBuilder.Append(property.Name).Append(":")
                                        .Append(objValue.ToString())
                                        .Append("->")
                                        .Append(ht[property.Name.ToLower()].ToString())
                                        .Append("|");
                                }
                            }
                        }
                        else
                        {
                            property.SetValue(obj, ReflectionHelper.ChangeType(ht[property.Name.ToLower()].ToString(), property.PropertyType), null);
                            stringBuilder.Append(property.Name).Append(":")
                                .Append("->")
                                .Append(ht[property.Name.ToLower()].ToString())
                                .Append("|");
                        }

                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog("PageHelper_GetControlValue", String.Format("{0}", ex.Message.ToString()));
                    }
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Hashtable 内容，存入 页面控件。
        /// Key 小写字符
        /// </summary>
        /// <param name="control">父容器</param>
        /// <param name="ht">Hashtable Key 小写字符</param>
        public static void SetControlValue(Control control, Hashtable ht)
        {
            Page page = System.Web.HttpContext.Current.Handler as Page;
            if (page == null)
            {
                return;
            }
            string _id = string.Empty;
            foreach (Control c in control.Controls)
            {
                if (c is TextBox)
                {
                    TextBox txt = c as TextBox;
                    if (txt.ID.IndexOf("txt") == 0)
                    {
                        _id = txt.ID.Remove(0, 3);
                    }
                    if (ht.ContainsKey(_id.ToLower()))
                    {
                        txt.Text = ht[_id.ToLower()].ToString();
                    }
                }
                else if (c is DropDownList)
                {
                    DropDownList ddl = c as DropDownList;
                    if (ddl.ID.IndexOf("ddl") == 0)
                    {
                        _id = ddl.ID.Remove(0, 3);
                    }
                    if (ht.ContainsKey(_id.ToLower()))
                    {
                        try
                        {
                            ddl.SelectedValue = ht[_id.ToLower()].ToString();
                        }
                        catch { }
                    }
                }
                else if (c is RadioButton)
                {
                    RadioButton rb = c as RadioButton;
                    if (rb.GroupName.IndexOf("rb") == 0)
                    {
                        _id = rb.GroupName.Remove(0, 2);
                    }
                    if (ht.ContainsKey(_id.ToLower()))
                    {
                        if (rb.ID.ToUpper().Equals((rb.GroupName + ht[_id.ToLower()].ToString()).ToUpper()))
                        {
                            rb.Checked = true;
                        }
                        else
                        {
                            rb.Checked = false;
                        }                        
                    }
                }
                else if (c is CheckBox)
                {
                    CheckBox cb = c as CheckBox;
                    if (cb.ValidationGroup.IndexOf("cb") == 0)
                    {
                        _id = cb.ValidationGroup.Remove(0, 2);
                    }
                    if (ht.ContainsKey(_id.ToLower()))
                    {
                        string[] cbs = ht[_id.ToLower()].ToString().Split(',');
                        foreach (string s in cbs)
                        {
                            if (cb.ID.ToUpper().Equals((cb.ValidationGroup + s.Trim()).ToUpper()))
                            {
                                cb.Checked = true;
                            }
                        }
                    }
                }
                else if (c.HasControls())
                {
                    SetControlValue(c, ht);
                }
            }
        }


        /// <summary>
        /// 存入实体类 T  obj 内容，存入 页面控件。
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="control">父容器</param>
        /// <param name="obj">实体类对象</param>
        /// <param name="DateFormatString">日期字段格式</param>
        public static void SetControlValue<T>(Control control, T obj, string dateFormatString)
        {
            Hashtable ht = new Hashtable();
            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.GetValue(obj, null) != null)
                {
                    log4net.ILog log = log4net.LogManager.GetLogger("DP_Web_Logger");
                    if (ReflectionHelper.TypeFullName(property.PropertyType) == "System.DateTime")
                    {
                        log.Info(DateTimeHelper.Format(property.GetValue(obj, null).ToString(), dateFormatString));
                        ht.Add(property.Name.ToLower(), DateTimeHelper.Format(property.GetValue(obj, null).ToString(), dateFormatString));
                    }
                    else
                    {
                        ht.Add(property.Name.ToLower(), property.GetValue(obj, null).ToString());
                    }
                }
            }
            SetControlValue(control, ht);
        }

        /// <summary>
        /// 存入实体类 T  obj 内容，存入 页面控件。
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="control">父容器</param>
        /// <param name="obj">实体类对象</param>
        public static void SetControlValue<T>(Control control, T obj)
        {
            SetControlValue<T>(control, obj, "yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 清空容器内容
        /// Clears the control value.
        /// </summary>
        /// <param name="control">The control.</param>
        public static void ClearControlValue(Control control)
        {
            Page page = System.Web.HttpContext.Current.Handler as Page;
            if (page == null)
            {
                return;
            }
            string _id = string.Empty;
            foreach (Control c in control.Controls)
            {
                if (c is TextBox)
                {
                    TextBox txt = c as TextBox;
                    txt.Text = "";
                }
                else if (c is DropDownList)
                {
                    DropDownList ddl = c as DropDownList;
                    ddl.SelectedIndex = -1;
                }
                else if (c is RadioButton)
                {
                    RadioButton rb = c as RadioButton;
                    rb.Checked = false;
                }
                else if (c is CheckBox)
                {
                    CheckBox cb = c as CheckBox;
                    cb.Checked = false;
                }
                else if (c.HasControls())
                {
                    ClearControlValue(c);
                }
            }
        }

        #region 获取CheckBox选中值

        /// <summary>
        /// 获取CheckBox选中值
        /// </summary>
        /// <param name="preName"></param>
        /// <param name="cont"></param>
        /// <returns></returns>
        public static string GetCheckBoxSelectValue(string preName, Control cont)
        {
            string ret = "";
            GetCheckBoxSelectValue(preName, cont, ref ret);
            return ret;
        }

        /// <summary>
        /// 获取CheckBox选中值
        /// </summary>
        /// <param name="preName"></param>
        /// <param name="cont"></param>
        /// <param name="ret"></param>
        public static void GetCheckBoxSelectValue(string preName, Control cont, ref string ret)
        {
            foreach (Control c in cont.Controls)
            {
                if (c is CheckBox)
                {
                    if ((c as CheckBox).Checked && c.ID.IndexOf(preName).Equals(0))
                    {
                        if (ret.Length > 0)
                        {
                            ret += ",";
                        }
                        ret += c.ID.Replace(preName, "");
                    }
                }
                else if (c.HasControls())
                {
                    GetCheckBoxSelectValue(preName, c, ref ret);
                }
            }
        }

        /// <summary>
        /// 设置CheckBox选中的值
        /// </summary>
        /// <param name="preName"></param>
        /// <param name="cont"></param>
        /// <param name="values"></param>
        public static void SetCheckBoxSelectValue(string preName, Control cont, string values)
        {
            string[] temp = values.Split(',');
            foreach (string s in temp)
            {
                Control c = cont.FindControl(preName + s);
                if (c != null)
                {
                    if (c is CheckBox)
                    {
                        (c as CheckBox).Checked = true;
                    }
                }
            }
        }

        #endregion

        #region 获取链接字符串参数
        /// <summary>
        /// 获取 连接字段串 参数
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetQueryString(Page page, string key)
        {
            return (page.Request.QueryString[key] == null) ? String.Empty : System.Web.HttpUtility.UrlDecode(page.Request.QueryString[key].ToString());
        }

        /// <summary>
        /// 获取 连接字段串 参数
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetQueryString(Page page, string key, Encoding encoding)
        {
            Hashtable ht = GetQueryString(page, encoding);
            if (ht.Count > 0)
            {
                if (ht.ContainsKey(key.ToLower()))
                {
                    return ht[key.ToLower()].ToString();
                }
            }
            return String.Empty;
            //return (page.Request.QueryString[key] == null) ? String.Empty : System.Web.HttpUtility.UrlDecode(page.Request.QueryString[key].ToString());
        }

        /// <summary>
        /// 获取 连接字段串 参数
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <returns></returns>
        public static Hashtable GetQueryString(Page page)
        {
            Hashtable htRet = new Hashtable();
            string strKey, strValue;

            NameValueCollection nvc = page.Request.QueryString;

            for (int i = 0; i < nvc.Count; i++)
            {
                strKey = nvc.GetKey(i);
                if (strKey == null) strKey = "Default";
                strValue = System.Web.HttpUtility.UrlDecode(nvc[i]);

                HashtableHelper.SetValue(ref htRet, strKey, strValue);
            }
            return htRet;
        }

        /// <summary>
        /// 获取 连接字段串 参数
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <returns></returns>
        public static Hashtable GetQueryString(Page page, Encoding encoding)
        {
            Hashtable htRet = new Hashtable();
            string strKey, strValue;
            string queryString = page.Request.Url.Query;

            NameValueCollection nvc = HttpUtility.ParseQueryString(queryString, encoding);

            for (int i = 0; i < nvc.Count; i++)
            {
                strKey = nvc.GetKey(i);
                if (strKey == null) strKey = "Default";
                strValue = System.Web.HttpUtility.UrlDecode(nvc[i]);

                HashtableHelper.SetValue(ref htRet, strKey, strValue);
            }
            return htRet;
        }

        /// <summary>
        /// 获取 Post 参数
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetForm(Page page, string key)
        {
            return (page.Request.Form[key] == null) ? String.Empty : System.Web.HttpUtility.UrlDecode(page.Request.Form[key].ToString());
        }

        /// <summary>
        /// 获取 Post 参数
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <param name="key">The key.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        public static string GetForm(Page page, string key, Encoding encoding)
        {
            return (page.Request.Form[key] == null) ? String.Empty : System.Web.HttpUtility.UrlDecode(page.Request.Form[key].ToString(), encoding);
        }

        /// <summary>
        /// 获取 Post 参数
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <returns></returns>
        public static Hashtable GetForm(Page page)
        {
            Hashtable htRet = new Hashtable();
            string strKey, strValue;

            NameValueCollection nvc = page.Request.Form;

            for (int i = 0; i < nvc.Count; i++)
            {
                strKey = nvc.GetKey(i);
                if (strKey == null) strKey = "Default";
                strValue = System.Web.HttpUtility.UrlDecode(nvc[i]);

                HashtableHelper.SetValue(ref htRet, strKey, strValue);
            }
            return htRet;
        }


        

        #endregion

        #region 获取页面相关的字符串
        /// <summary>
        /// 取控件客户端前缀
        /// </summary>
        /// <param name="page"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string GetControlClientPrefix(Page page, System.Web.UI.WebControls.WebControl c)
        {
            return c.ClientID.Substring(0, (c.ClientID.Length - c.ID.Length));
        }

        /// <summary>
        /// 获取当前页面 客户端 路径
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <returns></returns>
        public static string GetCurrentUri(Page page)
        {
            string appRelativePath = "";
            if (page.AppRelativeTemplateSourceDirectory.IndexOf("~/") == 0)
            {
                appRelativePath = page.AppRelativeTemplateSourceDirectory.Substring(2);
            }
            return GetRootUri(page) + appRelativePath;
        }

        /// <summary>
        /// 获取当前程序的根目录
        /// </summary>
        /// <param name="page">当前页面</param>
        /// <returns></returns>
        public static string GetRootUri(Page page)
        {
            string AppPath = "";
            HttpContext HttpCurrent = HttpContext.Current;
            HttpRequest Req;
            if (HttpCurrent != null)
            {
                Req = page.Request;

                string UrlAuthority = Req.Url.GetLeftPart(UriPartial.Authority);
                if (Req.ApplicationPath == null || Req.ApplicationPath == "/")
                {
                    //直接安装在   Web   站点  
                    AppPath = UrlAuthority;
                }
                else
                {
                    //安装在虚拟子目录下  
                    AppPath = UrlAuthority + Req.ApplicationPath;
                }
            }
            if (!String.IsNullOrEmpty(AppPath) && !AppPath[AppPath.Length - 1].Equals('/'))
            {
                AppPath += "/";
            }
            return AppPath;
        } 
        #endregion

        #region List To DataTable
        /// <summary>
        /// List《T》转换成 DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(List<T> list)
        {
            DataTable dt = new DataTable();

            if (list == null || list.Count == 0)
            {
                return dt;
            }

            PropertyInfo[] propertiest = list[0].GetType().GetProperties();
            foreach (PropertyInfo property in propertiest)
            {
                DataColumn dc = new DataColumn();
                dc.ColumnName = property.Name;

                Type columnType = property.PropertyType;
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    columnType = property.PropertyType.GetGenericArguments()[0];
                }

                dc.DataType = columnType;
                dt.Columns.Add(dc);
            }

            foreach (T obj in list)
            {
                DataRow dr = dt.NewRow();
                PropertyInfo[] properties = obj.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    dr[property.Name] = property.GetValue(obj, null).ToString();
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        /// <summary>
        /// List《T》转换成 DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(Page page, List<T> list)
        {
            DataTable dt = new DataTable();

            if (list == null || list.Count == 0)
            {
                return dt;
            }

            PropertyInfo[] propertiest = list[0].GetType().GetProperties();
            foreach (PropertyInfo property in propertiest)
            {
                DataColumn dc = new DataColumn();
                dc.ColumnName = property.Name;

                Type columnType = property.PropertyType;
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    columnType = property.PropertyType.GetGenericArguments()[0];
                }

                dc.DataType = columnType;
                dt.Columns.Add(dc);
            }

            foreach (T obj in list)
            {
                DataRow dr = dt.NewRow();
                PropertyInfo[] properties = obj.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    dr[property.Name] = property.GetValue(obj, null).ToString();
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }
        
        #endregion

        #region Output To Word
        /// <summary>
        /// 字符串输出到Word 文件
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="text">The text.</param>
        public static void OutputWord(string fileName, string text)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                page.Response.ContentEncoding = System.Text.Encoding.Default;
                page.Response.ClearContent();
                page.Response.ClearHeaders();
                page.Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".doc");
                page.Response.AddHeader("Content-type", "application");
                //page.Response.ContentType = "application/ms-html";
                page.Response.ContentType = "application/ms-word";
                page.Response.Write(text);
                page.Response.Flush();
                page.Response.Close();
            }
        }

        /// <summary>
        /// 字符串输出到Word 文件
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="text">The text.</param>
        public static void OutputWord(Page page, string fileName, string text)
        {
            page.Response.ContentEncoding = System.Text.Encoding.Default;
            page.Response.ClearContent();
            page.Response.ClearHeaders();
            page.Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".doc");
            page.Response.AddHeader("Content-type", "application");
            //page.Response.ContentType = "application/ms-html";
            page.Response.ContentType = "application/ms-word";
            page.Response.Write(text);
            page.Response.Flush();
            page.Response.Close();
        } 
        #endregion

        #region Get Template Text
        /// <summary>
        /// Gets the template text.
        /// </summary>
        /// <param name="templateName">Name of the template.</param>
        /// <returns></returns>
        public static string GetTemplateText(string templateName)
        {
            StreamReader sr = null;
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                string TemplateFilePath = page.Server.MapPath(templateName);
                if (!File.Exists(TemplateFilePath))
                {
                    throw new Exception("模板文件不存在！");
                }
                sr = new StreamReader(TemplateFilePath, Encoding.Default);               
            } 
            return sr.ReadToEnd();
        }
        /// <summary>
        /// Gets the template text.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="templateName">Name of the template.</param>
        /// <returns></returns>
        public static string GetTemplateText(Page page, string templateName)
        {
            string TemplateFilePath = page.Server.MapPath(templateName);
            if (!File.Exists(TemplateFilePath))
            {
                throw new Exception("模板文件不存在！");
            }
            StreamReader sr = new StreamReader(TemplateFilePath, Encoding.Default);
            return sr.ReadToEnd();

        } 
        #endregion

        #region Output To Excel
        /// <summary>
        /// 字符串输出到Excel 文件
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="text">The text.</param>
        public static void OutputExcel(Page page, string fileName, string text)
        {
            page.Response.ContentEncoding = System.Text.Encoding.UTF8;
            page.Response.ClearContent();
            page.Response.ClearHeaders();
            page.Response.AddHeader("content-disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + ".xls");
            page.Response.AddHeader("Content-type", "application");
            //page.Response.ContentType = "application/ms-html";
            page.Response.ContentType = "application/ms-excel";
            page.Response.Write("<meta http-equiv=Content-Type content=text/html; charset=gb2312>");
            page.Response.Write(text);
            page.Response.Flush();
            page.Response.Close();
        }

        /// <summary>
        /// DataTable  转出 Excel
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="dt">The dt.</param>
        public static void OutputExcel(Page page, string fileName, DataTable dt)
        {
            OutputExcel(page, fileName, DataTableToString(page, dt));
        }

        /// <summary>
        /// DataTable  转出 Excel
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="dt">The dt.</param>
        /// <param name="htTitle">The ht title.</param>
        public static void OutputExcel(Page page, string fileName, DataTable dt, Hashtable htTitle)
        {
            OutputExcel(page, fileName, DataTableToString(page, dt, htTitle));
        }

        /// <summary>
        /// DataTable  转出 Excel
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="dt">The dt.</param>
        /// <param name="htTitle">The ht title.</param>
        /// <param name="referenceHashtable">if set to <c>true</c> [reference hashtable].</param>
        public static void OutputExcel(Page page, string fileName, DataTable dt, Hashtable htTitle, bool referenceHashtable)
        {
            OutputExcel(page, fileName, DataTableToString(page, dt, htTitle, referenceHashtable));
        }

        /// <summary>
        /// 字符串输出到Excel 文件
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="text">The text.</param>
        public static void OutputExcel(string fileName, string text)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                page.Response.ContentEncoding = System.Text.Encoding.Default;
                page.Response.ClearContent();
                page.Response.ClearHeaders();
                page.Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xls");
                page.Response.AddHeader("Content-type", "application");
                //page.Response.ContentType = "application/ms-html";
                page.Response.ContentType = "application/ms-excel";
                page.Response.Write(text);
                page.Response.Flush();
                page.Response.Close();
            }
        }

        /// <summary>
        /// DataTable  转出 Excel
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="dt">The dt.</param>
        public static void OutputExcel(string fileName, DataTable dt)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                OutputExcel(page, fileName, DataTableToString(page, dt));
            }
        }

        /// <summary>
        /// DataTable  转出 Excel
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="dt">The dt.</param>
        /// <param name="htTitle">The ht title.</param>
        /// <param name="referenceHashtable">if set to <c>true</c> [reference hashtable].</param>
        public static void OutputExcel(string fileName, DataTable dt, Hashtable htTitle, bool referenceHashtable)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                OutputExcel(page, fileName, DataTableToString(page, dt, htTitle, referenceHashtable));
            }
        }
        
        #endregion

        #region DataTable To String
        /// <summary>
        /// DataTable 转到 CVS String
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="dt">The dt.</param>
        /// <returns></returns>
        public static string DataTableToString(Page page, DataTable dt)
        {
            return DataTableToString(page, dt, null);
        }

        /// <summary>
        /// DataTable 转到 CVS String
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="dt">The dt.</param>
        /// <param name="htTitle">The ht title.</param>
        /// <returns></returns>
        public static string DataTableToString(Page page, DataTable dt, Hashtable htTitle)
        {
            StringBuilder sb = new StringBuilder();
            Hashtable ht = new Hashtable();
            sb.Append("<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\"   style=\"font-size: 13;\"  >");
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ht.Add(i, dt.Columns[i].ColumnName);
            }
            sb.Append("<tr style=\"font-weight: bold; white-space: nowrap;\">");
            for (int i = 0; i < ht.Count; i++)
            {
                sb.Append("<td>");
                if (htTitle != null && htTitle.Count > 0)
                {
                    if (htTitle.ContainsKey(ht[i].ToString().ToLower()))
                    {
                        sb.Append(StringHelper.ReplaceEnter(htTitle[ht[i].ToString().ToLower()].ToString()));
                    }
                    else
                    {
                        sb.Append(StringHelper.ReplaceEnter(ht[i].ToString()));
                    }
                }
                else
                {
                    sb.Append(StringHelper.ReplaceEnter(ht[i].ToString()));
                }
                sb.Append("</td>");
            }
            sb.Append("</tr>");

            for (int j = 0; j < dt.Rows.Count; j++)
            {
                sb.Append("<tr>");
                for (int i = 0; i < ht.Count; i++)
                {
                    sb.Append("<td style=\"vnd.ms-excel.numberformat:@\">").Append(StringHelper.ReplaceEnter(dt.Rows[j][ht[i].ToString()].ToString())).Append("</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }


        /// <summary>
        /// DataTable 转到 CVS String
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="dt">The dt.</param>
        /// <param name="htTitle">The ht title.</param>
        /// <param name="referenceHashtable">if set to <c>true</c> [reference hashtable].</param>
        /// <returns></returns>
        public static string DataTableToString(Page page, DataTable dt, Hashtable htTitle, bool referenceHashtable)
        {
            string temp = string.Empty;
            if (!referenceHashtable)
            {
                return DataTableToString(page, dt, htTitle);
            }

            StringBuilder sb = new StringBuilder();
            Hashtable ht = new Hashtable();
            sb.Append("<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\" style=\"font-size: 13;\" >");
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (htTitle != null && htTitle.Count > 0)
                {
                    if (htTitle.ContainsKey(ht[i].ToString().ToLower()))
                    {
                        ht.Add(i, dt.Columns[i].ColumnName);
                    }
                }
                else
                {
                    ht.Add(i, dt.Columns[i].ColumnName);
                }
            }
            sb.Append("<tr style=\"font-weight: bold; white-space: nowrap;\">");
            for (int i = 0; i < ht.Count; i++)
            {
                sb.Append("<td>");
                if (htTitle != null && htTitle.Count > 0)
                {
                    if (htTitle.ContainsKey(ht[i].ToString().ToLower()))
                    {
                        sb.Append(StringHelper.ReplaceEnter(htTitle[ht[i].ToString().ToLower()].ToString()));
                    }
                    else
                    {
                        sb.Append(StringHelper.ReplaceEnter(ht[i].ToString()));
                    }
                }
                else
                {
                    sb.Append(StringHelper.ReplaceEnter(ht[i].ToString()));
                }
                sb.Append("</td>");
            }
            sb.Append("</tr>");

            for (int j = 0; j < dt.Rows.Count; j++)
            {
                sb.Append("<tr>");
                for (int i = 0; i < ht.Count; i++)
                {
                    sb.Append("<td style=\"vnd.ms-excel.numberformat:@\">").Append(StringHelper.ReplaceEnter(dt.Rows[j][ht[i].ToString()].ToString())).Append("</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }

        /// <summary>
        /// DataTable 转到 CVS String
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns></returns>
        public static string DataTableToString(DataTable dt)
        {
            return DataTableToString(dt, null);
        }

        //public static string DataTableToString(DataTable dt, GridView gv)
        //{
        //    Dictionary<string, string> dict = new Dictionary<string, string>();
        //    foreach (DataControlField dcf in gv.Columns)
        //    {
        //        if (dcf.Visible)
        //        {
        //            BoundColumn bc = dcf as BoundColumn;
        //            TemplateColumn tc = dcf as TemplateColumn;
        //            dict.Add(dcf, dcf.HeaderText);
        //        }
        //    }
        //}

        /// <summary>
        /// DataTable 转到 CVS String
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="htTitle">The ht title.</param>
        /// <returns></returns>
        public static string DataTableToString(DataTable dt, Hashtable htTitle)
        {
            StringBuilder sb = new StringBuilder();
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                Hashtable ht = new Hashtable();
                sb.Append("<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\"   style=\"font-size: 13;\" >");
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ht.Add(i, dt.Columns[i].ColumnName);
                }
                sb.Append("<tr style=\"font-weight: bold; white-space: nowrap;\">");
                for (int i = 0; i < ht.Count; i++)
                {
                    sb.Append("<td>");
                    if (htTitle != null && htTitle.Count > 0)
                    {
                        if (htTitle.ContainsKey(ht[i].ToString().ToLower()))
                        {
                            sb.Append(StringHelper.ReplaceEnter(htTitle[ht[i].ToString().ToLower()].ToString()));
                        }
                        else
                        {
                            sb.Append(StringHelper.ReplaceEnter(ht[i].ToString()));
                        }
                    }
                    else
                    {
                        sb.Append(StringHelper.ReplaceEnter(ht[i].ToString()));
                    }
                    sb.Append("</td>");
                }
                sb.Append("</tr>");

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    sb.Append("<tr>");
                    for (int i = 0; i < ht.Count; i++)
                    {
                        sb.Append("<td style=\"vnd.ms-excel.numberformat:@\">").Append(StringHelper.ReplaceEnter(dt.Rows[j][ht[i].ToString()].ToString())).Append("</td>");
                    }
                    sb.Append("</tr>");
                }
                sb.Append("</table>");
            }
            return sb.ToString();
        }
        
        #endregion

        #region Get Client Info
        /// <summary>
        /// 获取客户端信息
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static ClientInfo GetClientInfo(Page page)
        {
            ClientInfo c = new ClientInfo();
            string _Http_User_Agent = string.Empty;
            //获取IP地址
            c.IP = page.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(c.IP))
            {
                c.IP = page.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(c.IP))
            {
                c.IP = page.Request.UserHostAddress;
            }
            c.Name = page.Request.ServerVariables["Remote_Host"];
            if (string.IsNullOrEmpty(c.Name))
            {
                c.Name = page.Request.UserHostName;
            }
            c.Language = page.Request.ServerVariables["Http_Accept_Language"];
            c.Request_Method = page.Request.ServerVariables["Request_Method"];
            _Http_User_Agent = page.Request.ServerVariables["Http_User_Agent"];

            #region 获取OS信息
            if (_Http_User_Agent.IndexOf("Windows NT 5.1") >= 0)
            {
                c.OS = "Windows XP";
            }
            else if (_Http_User_Agent.IndexOf("Windows NT 6.0") >= 0)
            {
                c.OS = "Windows Vista";
            }
            else if (_Http_User_Agent.IndexOf("Windows NT 5.0") >= 0)
            {
                c.OS = "Windows 2000";
            }
            else if (_Http_User_Agent.IndexOf("Windows NT 5.2") >= 0)
            {
                c.OS = "Windows 2003";
            }
            else if (_Http_User_Agent.IndexOf("98") >= 0)
            {
                c.OS = "Windows 98";
            }
            else if (_Http_User_Agent.IndexOf("Longhorn") >= 0)
            {
                c.OS = "Windows Longhorn";
            }
            else if (_Http_User_Agent.IndexOf("Mac") >= 0)
            {
                c.OS = "Mac OS";
            }
            else if (_Http_User_Agent.IndexOf("Linux") >= 0)
            {
                c.OS = "Linux";
            }
            else if (_Http_User_Agent.IndexOf("AIX") >= 0)
            {
                c.OS = "AIX";
            }
            else if (_Http_User_Agent.IndexOf("AmigaOS") >= 0)
            {
                c.OS = "Amiga";
            }
            else if (_Http_User_Agent.IndexOf("BEOS") >= 0)
            {
                c.OS = "BeOS";
            }
            else if (_Http_User_Agent.IndexOf("FreeBSD") >= 0)
            {
                c.OS = "FreeBSD";
            }
            else if (_Http_User_Agent.IndexOf("HP-UX") >= 0)
            {
                c.OS = "HP Unix";
            }
            else if (_Http_User_Agent.IndexOf("IRIX") >= 0)
            {
                c.OS = "IRIX";
            }
            else if (_Http_User_Agent.IndexOf("WebTV") >= 0)
            {
                c.OS = "MSN TV (WebTV)";
            }
            else if (_Http_User_Agent.IndexOf("OpenBSD") >= 0)
            {
                c.OS = "OpenBSD";
            }
            else if (_Http_User_Agent.IndexOf("OS/2") >= 0)
            {
                c.OS = "OS/2";
            }
            else if (_Http_User_Agent.IndexOf("OSF1") >= 0)
            {
                c.OS = "OSF1";
            }
            else if (_Http_User_Agent.IndexOf("SUN") >= 0)
            {
                c.OS = "Sun OS";
            }
            else if (_Http_User_Agent.IndexOf("Windows 3.1") >= 0)
            {
                c.OS = "Windows 3.x";
            }
            else if (_Http_User_Agent.IndexOf("95") >= 0)
            {
                c.OS = "Windows 95";
            }
            else if (_Http_User_Agent.IndexOf("Blackcomb") >= 0)
            {
                c.OS = "Windows Blackcomb";
            }
            else if (_Http_User_Agent.IndexOf("Windows CE") >= 0)
            {
                c.OS = "Windows CE";
            }
            else if (_Http_User_Agent.IndexOf("ME") >= 0)
            {
                c.OS = "Windows ME";
            }
            else if (_Http_User_Agent.IndexOf("Win32") >= 0)
            {
                c.OS = "Windows Win32s";
            }
            else if (_Http_User_Agent.IndexOf("X Window") >= 0)
            {
                c.OS = "X Windows";
            }
            else if (_Http_User_Agent.IndexOf("WinNT4") >= 0)
            {
                c.OS = "Windows NT";
            }
            else if (_Http_User_Agent.IndexOf("Windows NT") >= 0)
            {
                c.OS = "Windows NT";
            }
            #endregion

            #region 获取浏览器信息
            if (_Http_User_Agent.IndexOf("MSIE") >= 0)
            {
                c.Browser = "IE";
            }
            else if (_Http_User_Agent.IndexOf("Chrome") >= 0)
            {
                c.Browser = "Chrome";
            }
            else if (_Http_User_Agent.IndexOf("Safari") >= 0)
            {
                c.Browser = "Safari";
            }
            else if (_Http_User_Agent.IndexOf("Opera") >= 0)
            {
                c.Browser = "Opera";
            }
            else if (_Http_User_Agent.IndexOf("Firefox") >= 0)
            {
                c.Browser = "Firefox";
            }
            else if (_Http_User_Agent.IndexOf("") >= 0)
            {
                c.Browser = "";
            }
            #endregion

            return c;
        }
        /// <summary>
        /// 获取客户端信息
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static ClientInfo GetClientInfo()
        {
            ClientInfo c = new ClientInfo();
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                c = GetClientInfo(page);
            }
            return c;
        }
        
        #endregion

    }

    
}
