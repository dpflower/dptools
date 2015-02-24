using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DP.AsyncSocketClientStudy
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AsyncSocketClient());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            DP.Common.LogHelper.WriteLog("CurrentDomain_UnhandledException", e.ExceptionObject.ToString());
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            DP.Common.LogHelper.WriteLog("Application_ThreadException", String.Format("\r\n{0}\r\n{1}", e.Exception.Message, e.Exception.StackTrace));
        }
    }
}
