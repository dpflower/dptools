using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace DP.Net
{
    public static class WebRequestHelper
    {
        /// <summary>
        /// Post 请求
        /// Posts the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        public static string Post(string url, string data, System.Text.Encoding encoding)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            byte[] bytes = encoding.GetBytes(data);
            request.ContentLength = bytes.Length;

            Stream stream = request.GetRequestStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Close();

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            if (response == null) return null;

            StreamReader streamReader = new StreamReader(response.GetResponseStream());
            return streamReader.ReadToEnd().Trim();
        }

        /// <summary>
        /// Post 请求
        /// Posts the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string Post(string url, string data)
        {
            return Post(url, data, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Get 请求
        /// Gets the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        public static string Get(string url, string data, System.Text.Encoding encoding)
        {
            HttpWebRequest request = WebRequest.Create(url + (String.IsNullOrEmpty(data) ? String.Empty : "?" + data)) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "text/html";

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, encoding);
            string retString = streamReader.ReadToEnd();
            streamReader.Close();
            stream.Close();

            return retString;
        }

        /// <summary>
        /// Get 请求
        /// Gets the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string Get(string url, string data)
        {
            return Get(url, data, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 获取远程文件大小
        /// </summary>
        /// <param name="fileUrl">远程文件URL</param>
        /// <returns>远程文件大小</returns>
        public static long GetRemoteFileSize(string fileUrl)
        {
            long ContentLength = 0;
            try
            {
                WebRequest req = HttpWebRequest.Create(fileUrl);
                req.Method = "HEAD";
                WebResponse resp = req.GetResponse();
                long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength);
            }
            catch (Exception ex)
            {
                ContentLength = -1;
            }
            return ContentLength;
        }

    }
}
