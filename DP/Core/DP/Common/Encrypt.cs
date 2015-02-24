using System;
using System.Collections.Generic;
using System.Text;

namespace DP.Common
{
    public class Encrypt
    {
        #region MD5
        /// <summary>
        /// 用MD5加密字符串
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <returns>加密好返回的字符串</returns>
        public static string EncryptMD5(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            byte[] bytValue, bytHash;
            bytValue = System.Text.Encoding.UTF8.GetBytes(input);
            bytHash = md5.ComputeHash(bytValue);
            md5.Clear();
            string sTemp = "";
            for (int i = 0; i < bytHash.Length; i++)
            {
                sTemp += bytHash[i].ToString("x").PadLeft(2, '0');
            }

            return sTemp;

            //return input;

        }
        #endregion

        #region DES
        /// <summary>
        /// 默认密钥向量
        /// </summary>
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = System.Text.Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = System.Text.Encoding.UTF8.GetBytes(encryptString);

                //创建一个DES算法的加密类
                System.Security.Cryptography.DESCryptoServiceProvider dCSP = new System.Security.Cryptography.DESCryptoServiceProvider();

                System.IO.MemoryStream mStream = new System.IO.MemoryStream();

                //从DES算法的加密类对象的CreateEncryptor方法,创建一个加密转换接口对象
                //第一个参数的含义是：对称算法的机密密钥(长度为64位,也就是8个字节)
                //可以人工输入,也可以随机生成方法是：MyServiceProvider.GenerateKey();
                //第二个参数的含义是：对称算法的初始化向量(长度为64位,也就是8个字节)
                //可以人工输入,也可以随机生成方法是：MyServiceProvider.GenerateIV();
                //CryptoStream对象的作用是将数据流连接到加密转换的流
                System.Security.Cryptography.CryptoStream cStream = new System.Security.Cryptography.CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), System.Security.Cryptography.CryptoStreamMode.Write);
                //将字节数组中的数据写入到加密流中
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = System.Text.Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                System.Security.Cryptography.DESCryptoServiceProvider DCSP = new System.Security.Cryptography.DESCryptoServiceProvider();

                System.IO.MemoryStream mStream = new System.IO.MemoryStream();
                //从DES算法的加密类对象的CreateEncryptor方法,创建一个解密转换接口对象
                //[对称算法的机密密钥]必须是加密时候的[对称算法的机密密钥]
                //[对称算法的初始化向量]必须是加密时候的[对称算法的初始化向量]
                //如果不一样，则会抛出一个异常。
                System.Security.Cryptography.CryptoStream cStream = new System.Security.Cryptography.CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), System.Security.Cryptography.CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return System.Text.Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        } 
        #endregion
    }
}
