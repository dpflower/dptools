using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DP.Common
{
    public delegate void MessageEventHandler(string message);

    public class ProcessHelper
    {
        public MessageEventHandler onMessage;

        /// <summary>
        /// 开起进程
        /// Res the start process.
        /// </summary>
        /// <param name="pathName">Name of the path.</param>
        public void StartProcess(string pathName)
        {
            StartProcess(pathName, false);
        }

        /// <summary>
        /// Res the start process.
        /// </summary>
        /// <param name="pathName">Name of the path.</param>
        /// <param name="killOldProcess">if set to <c>true</c> [kill old process].</param>
        public void StartProcess(string pathName, bool killOldProcess)
        {
            string fileName = pathName.Substring(pathName.LastIndexOf('\\') + 1, pathName.Length - pathName.LastIndexOf('\\') - 1);
            fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
            Process[] prc = Process.GetProcesses();
            List<Process> list = new List<Process>();

            foreach (Process p in prc)
            {
                if (p.ProcessName.ToLower().Equals(fileName.ToLower()))
                {
                    list.Add(p);
                }
            }

            if (list.Count > 0)
            {
                if (killOldProcess)
                {
                    foreach (Process p in list)
                    {
                        p.Kill();
                        if (onMessage != null)
                        {
                            onMessage(String.Format("杀死 {0}({1}) 进程成功！", p.ProcessName, p.Id));
                        }
                    }
                }
                else
                {
                    return;
                }
            }

            Process process = new Process();
            process.StartInfo.FileName = pathName;
            process.Start();

            if (onMessage != null)
            {
                onMessage(String.Format("开始 {0} 进程成功！", pathName));
            }
        }

        /// <summary>
        /// Kills the name of the process by process.
        /// </summary>
        /// <param name="processName">Name of the process.</param>
        public void KillProcessByProcessName(string processName)
        {
            Process[] prc = Process.GetProcesses();
            List<Process> list = new List<Process>();

            foreach (Process p in prc)
            {
                if (p.ProcessName.ToLower().Equals(processName.ToLower()))
                {
                    list.Add(p);
                }
            }

            foreach (Process p in list)
            {
                p.Kill();
                if (onMessage != null)
                {
                    onMessage(String.Format("杀死 {0}({1}) 进程成功！", p.ProcessName, p.Id));
                }
            }
        }

        /// <summary>
        /// Kills the name of the process by file.
        /// </summary>
        /// <param name="pathName">Name of the path.</param>
        public void KillProcessByFileName(string pathName)
        {
            string fileName = pathName.Substring(pathName.LastIndexOf('\\') + 1, pathName.Length - pathName.LastIndexOf('\\') - 1);
            fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
            KillProcessByProcessName(fileName);
        }



    }
}
