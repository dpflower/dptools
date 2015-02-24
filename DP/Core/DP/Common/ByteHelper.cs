using System;
using System.Collections.Generic;
using System.Text;

namespace DP.Common
{
    public class ByteHelper
    {
        /// <summary>
        /// 合并字节数组
        /// Merges the specified source array.
        /// </summary>
        /// <param name="sourceArray">The source array.</param>
        /// <param name="destinationArray">The destination array.</param>
        public static void Merge(ref byte[] sourceArray, byte[] destinationArray)
        {
            if (destinationArray != null)
            {
                if (sourceArray == null)
                {
                    sourceArray = new byte[destinationArray.Length];
                    sourceArray = destinationArray;
                }
                else
                {
                    lock (sourceArray.SyncRoot)
                    {
                        Array.Resize(ref sourceArray, sourceArray.Length + destinationArray.Length);
                        Array.Copy(destinationArray, 0, sourceArray, sourceArray.Length - destinationArray.Length, destinationArray.Length);
                    }
                }
            }
        }

        /// <summary>
        /// 移除字节数组前 指定长度字节数据。
        /// Removes the specified received array.
        /// </summary>
        /// <param name="receivedArray">The received array.</param>
        /// <param name="length">The length.</param>
        public static void Remove(ref byte[] receivedArray, int length)
        {
            Remove(ref receivedArray, 0, length);
        }

        /// <summary>
        /// 移除字节数组 指定 位置 指定长的字节数据。
        /// Removes the specified received array.
        /// </summary>
        /// <param name="receivedArray">The received array.</param>
        /// <param name="index">The index.</param>
        /// <param name="length">The length.</param>
        public static void Remove(ref byte[] receivedArray, int index, int length)
        {
            lock (receivedArray.SyncRoot)
            {
                if (receivedArray.Length >= length + index)
                {
                    Array.Copy(receivedArray, length + index, receivedArray, index, receivedArray.Length - length - index);
                    Array.Resize(ref receivedArray, receivedArray.Length - length);
                }
                else if (receivedArray.Length >= index)
                {
                    Array.Resize(ref receivedArray, index);
                }
            }
        }




    }
}
