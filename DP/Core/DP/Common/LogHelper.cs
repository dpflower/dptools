using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace DP.Common
{
    public static class LogHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string DefaultLogName
        {
            get
            {
                return "Log_DP_DATA";
            }
        }

        /// <summary>
        /// Writes the log.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static bool WriteLog(string message)
        {
            string _message = String.Format("DateTime:{0}\nMessage:{1}\n\r", DateTime.Now, message); 
            string logName = GetLogName();
            if (String.IsNullOrEmpty(logName))
            {
                return false;
            }
            return WriteFile(logName, _message);

        }

        public static bool WriteLog(string LogNamePre, string message)
        {
            string _message = String.Format("DateTime:{0}\nMessage:{1}\n\r", DateTime.Now, message);
            string logName = GetLogName(LogNamePre);
            if (String.IsNullOrEmpty(logName))
            {
                return false;
            }
            return WriteFile(logName, _message);
        }

        public static void WriteLogAsync(string message)
        {
            Tuple<string, string> info = new Tuple<string, string>(DefaultLogName, message);
            ThreadPool.QueueUserWorkItem(new WaitCallback(WriteLog), info);            
        }

        public static void WriteLogAsync(string LogNamePre, string message)
        {
            Tuple<string, string> info = new Tuple<string, string>(LogNamePre, message);
            ThreadPool.QueueUserWorkItem(new WaitCallback(WriteLog), info);     
        }

        private static void WriteLog(object info)
        {
            Tuple<string, string> logInfo = info as Tuple<string, string>;
            if (logInfo != null)
            {
                string _message = String.Format("DateTime:{0}\nMessage:{1}\n\r", DateTime.Now, logInfo.Item2);
                string logName = GetLogName(logInfo.Item1);
                if (String.IsNullOrEmpty(logName))
                {
                    return;
                }
                WriteFile(logName, _message);
            }
        }

        /// <summary>
        /// Gets the name of the log.
        /// </summary>
        /// <returns></returns>
        public static string GetLogName(string logNamePre)
        {
            string fileFullName = string.Empty;
            try
            {
                string path = "";
                if (HttpContext.Current == null)
                {
                    path = System.Threading.Thread.GetDomain().BaseDirectory.ToString().TrimEnd('\\') + "\\Log\\" + logNamePre;
                }
                else
                {
                    path = HttpContext.Current.Request.MapPath("~/Log/" + logNamePre).TrimEnd('/').TrimEnd('\\');
                }
                if (!CreateDirectory(path))
                {
                    return string.Empty;
                }
                DateTime datetime = DateTime.Now;
                string fileName = logNamePre + "_" + datetime.ToString("yyyyMMdd") + ".log";
                fileFullName = path + "\\" + fileName;
                if (!CreateFile(fileFullName))
                {
                    return string.Empty;
                }
            }
            catch
            {
                fileFullName = string.Empty;
            }
            return fileFullName;
        }

        public static string GetLogName()
        {
            return GetLogName(DefaultLogName);
        }


        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static bool WriteFile(string fileName, string message)
        {
            bool rel = false;           
            try
            {
                File.AppendAllText(fileName, message, Encoding.UTF8);
                rel = true;
            }
            catch
            {
                rel = false;
            }
            return rel;
        }

        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static bool CreateDirectory(string path)
        {
            bool rel = false;
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                rel = true;
            }
            catch
            {
                rel = false;
            }
            return rel;
        }

        /// <summary>
        /// Creates the file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static bool CreateFile(string filename)
        {
            bool rel = false;
            try
            {
                if (!File.Exists(filename))
                {
                    StreamWriter sw = File.CreateText(filename);
                    sw.Close();
                } rel = true;
            }
            catch
            {
                rel = false;
            }
            return rel;
        }

        public static void Test(string filename, string message)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(filename);
            
            log.Debug(message);
            log.Info(message);
            log.Warn(message);
            log.Error(message);
            log.Fatal(message);

        }
    }
}
