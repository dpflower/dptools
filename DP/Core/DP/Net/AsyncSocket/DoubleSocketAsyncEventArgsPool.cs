using System;
using System.Collections.Generic;
using System.Text;

namespace DP.Net.AsyncSocket
{
    /// <summary>
    /// Represents a collection of resusable SocketAsyncEventArgs objects.  
    /// </summary>
    internal class DoubleSocketAsyncEventArgsPool : IDisposable
    {
        Stack<DoubleSocketAsyncEventArgs> _pool;

        /// <summary>
        /// 初始化对象池到指定大小
        /// </summary>
        /// <param name="capacity">The maximum number of SocketAsyncEventArgs objects the pool can hold</param>
        internal DoubleSocketAsyncEventArgsPool(int capacity)
        {
            _pool = new Stack<DoubleSocketAsyncEventArgs>(capacity);
        }

        /// <summary>
        /// 添加SocketAsyncEventArg实例池
        /// </summary>
        /// <param name="item">The SocketAsyncEventArgs instance to add to the pool</param>
        internal void Push(DoubleSocketAsyncEventArgs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Items added to a DoubleSocketAsyncEventArgs cannot be null");
            }
            lock (_pool)
            {
                _pool.Push(item);
            }
        }

        /// <summary>
        /// 从池中移除一个实例的SocketAsyncEventArgs
        /// </summary>
        /// <returns>The object removed from the pool</returns>
        internal DoubleSocketAsyncEventArgs Pop()
        {
            lock (_pool)
            {
                return _pool.Pop();
            }
        }

        /// <summary>
        /// The number of SocketAsyncEventArgs instances in the pool
        /// </summary>
        internal int Count
        {
            get { return _pool.Count; }
        }


        #region IDisposable 成员

        public void Dispose()
        {
            _pool.Clear();
            _pool = null;
        }

        #endregion
    }
}
