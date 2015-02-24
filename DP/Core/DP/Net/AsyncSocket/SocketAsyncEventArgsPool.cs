using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace DP.Net.AsyncSocket
{
    /// <summary>
    /// Represents a collection of resusable SocketAsyncEventArgs objects.  
    /// </summary>
    internal class SocketAsyncEventArgsPool : IDisposable
    {
        Stack<SocketAsyncEventArgs> _pool;

        /// <summary>
        /// 初始化对象池到指定大小
        /// </summary>
        /// <param name="capacity">The maximum number of SocketAsyncEventArgs objects the pool can hold</param>
        internal SocketAsyncEventArgsPool(int capacity)
        {
            _pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        /// <summary>
        /// 添加SocketAsyncEventArg实例池
        /// </summary>
        /// <param name="item">The SocketAsyncEventArgs instance to add to the pool</param>
        internal void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgs cannot be null");
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
        internal SocketAsyncEventArgs Pop()
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
