using System;
using System.Collections.Generic;
using System.Text;

namespace DP.Web.UI
{
    [Serializable]
    public class ClientInfo
    {
        string _IP = string.Empty;
        string _Name = string.Empty;
        string _Browser = string.Empty;
        string _Language = string.Empty;
        string _Request_Method = string.Empty;
        string _OS = string.Empty;
        string _MacAddress = string.Empty;


        /// <summary>
        /// 
        /// </summary>
        public string OS
        {
            get { return _OS; }
            set { _OS = value; }
        }


        /// <summary>
        /// HTTP请求的方法(Post或Get)
        /// </summary>
        public string Request_Method
        {
            get { return _Request_Method; }
            set { _Request_Method = value; }
        }
        
        /// <summary>
        /// 客户端的语言环境
        /// </summary>
        public string Language
        {
            get { return _Language; }
            set { _Language = value; }
        }


        /// <summary>
        /// 客户端浏览器信息
        /// </summary>
        public string Browser
        {
            get { return _Browser; }
            set { _Browser = value; }
        }

        /// <summary>
        /// 客户端主机名称
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        /// <summary>
        /// 客户端IP
        /// </summary>
        public string IP
        {
            get { return _IP; }
            set { _IP = value; }
        }
        /// <summary>
        /// Gets or sets the mac address.
        /// </summary>
        /// <value>
        /// The mac address.
        /// </value>
        public string MacAddress
        {
            get { return _MacAddress; }
            set { _MacAddress = value; }
        }

    }
}
