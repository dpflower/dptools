using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

using DP.Common;

namespace DP.Net
{
    public class Ping
    {
        private string _IP;
        private PingStatus _Status = PingStatus.未知;

        public string IP
        {
            get { return _IP; }
            set
            {
                if (StringHelper.IsIP(value))
                {
                    _IP = value;
                }
                else
                {
                    throw new Exception("不是有效的IP地址！");
                }
            }
        }

        public PingStatus Status
        {
            get { return _Status; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IP"></param>
        public Ping(string IP)
        {
            this.IP = IP;
        }

        /// <summary>
        /// 开始 Ping
        /// </summary>
        public string Start()
        {
            Process process = new Process();
            process.StartInfo.FileName = "ping.exe";
            process.StartInfo.Arguments = "-n 1 " + _IP;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            string str = "";
            if (process.Start())
            {
                str = process.StandardOutput.ReadToEnd();
                if (str.IndexOf("(0% loss)") != -1)
                {
                    _Status = PingStatus.连接;
                }
                else if (str.IndexOf("Destination host unreachable.") != -1)
                {
                    _Status = PingStatus.无法到达目的主机;
                }
                else if (str.IndexOf("Request timed out.") != -1)
                {
                    _Status = PingStatus.超时;
                }
                else if (str.IndexOf("Unknown host") != -1)
                {
                    _Status = PingStatus.无法解析主机;
                }
                else
                {
                    _Status = PingStatus.未知;
                }
                process.Close();
            }
            return str;
        }

        public enum PingStatus
        {
            未知 = 0,
            连接 = 1,
            无法到达目的主机 = 2,
            超时 = 3,
            无法解析主机 = 4
        }

    }
}
