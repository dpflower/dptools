using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace DP.Common
{
    public class IniFile
    {
        #region 变量、属性
        /// <summary>
        /// 最大读取字节数
        /// </summary>
        private const int MaxSize = 256;
        /// <summary>
        /// ini 文件路径
        /// </summary>
        private string _FilePath = String.Empty;
        /// <summary>
        /// ini 文件路径
        /// </summary>
        public string FilePath
        {
            get { return _FilePath; }
            set { _FilePath = value; }
        } 
        #endregion

        #region 构造函数
        public IniFile()
        {

        }

        public IniFile(string filePath)
        {
            _FilePath = filePath;
        }
        
        #endregion

        [DllImport("kernel32.dll")]
        private static extern uint WritePrivateProfileString(string appName, string keyName, string value, string fileName);
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string appName, string keyName, string defaultValue, StringBuilder returned, int size, string fileName);

        #region 方法
        /// <summary>
        /// 读 .ini 文件
        /// </summary>
        /// <param name="appName">小节名</param>
        /// <param name="keyName">关键值</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns></returns>
        public string ReadValue(string appName, string keyName, string defaultValue)
        {
            if (String.IsNullOrEmpty(_FilePath))
            {
                throw new Exception(".ini 文件路径为空！");
            }
            StringBuilder sb = new StringBuilder();
            GetPrivateProfileString(appName, keyName, defaultValue, sb, MaxSize, _FilePath);
            return sb.ToString();
        }

        /// <summary>
        /// 写 .ini 文件
        /// </summary>
        /// <param name="appName">小节名</param>
        /// <param name="keyName">关键值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public uint WriteValue(string appName, string keyName, string value)
        {
            if (String.IsNullOrEmpty(_FilePath))
            {
                throw new Exception(".ini 文件路径为空！");
            }
            return WritePrivateProfileString(appName, keyName, value, _FilePath);
        } 
        #endregion

    }
}
