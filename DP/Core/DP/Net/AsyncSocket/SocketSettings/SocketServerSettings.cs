using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using DP.Net.AsyncSocket.Packet;


namespace DP.Net.AsyncSocket
{

    /// <summary>
    /// 
    /// </summary>
    public class SocketServerSettings : ISocketSettings
    {
        //byte[] _packetHeader = new byte[] { byte.MaxValue, byte.MinValue, byte.MaxValue, byte.MinValue, byte.MaxValue, byte.MinValue };

        IPackStrategy _packStrategy = new DefaultPackStrategy();

        int _maxConnections = 1000;

        int _receiveBufferSize = 1024;

        Encoding _encoding = Encoding.UTF8;

        int _opsToPreAlloc = 2;

        int _listenBacklog = 100;

        IPEndPoint _localEndPoint;

        /// <summary>
        /// 本地监听IP端口
        /// Gets or sets the local end point.
        /// </summary>
        /// <value>The local end point.</value>
        public IPEndPoint LocalEndPoint
        {
            get { return _localEndPoint; }
            set { _localEndPoint = value; }
        }

        /// <summary>
        /// 侦听 挂起连接队列的最大长度
        /// 开始与服务器听取 连接积压
        /// </summary>
        /// <value>The listen backlog.</value>
        public int ListenBacklog
        {
            get { return _listenBacklog; }
            set { _listenBacklog = value; }
        }

        /// <summary>
        /// 读，写（为接受不alloc缓冲空间）
        /// Gets the ops to pre alloc.
        /// </summary>
        /// <value>The ops to pre alloc.</value>
        public int OpsToPreAlloc
        {
            get { return _opsToPreAlloc; }
            //set { _opsToPreAlloc = value; }
        }

        /// <summary>
        /// 字节编码
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        /// <summary> 
        /// 缓冲区大小为每个套接字I / O操作
        /// Gets or sets the size of the receive buffer.
        /// </summary>
        /// <value>The size of the receive buffer.</value>
        public int ReceiveBufferSize
        {
            get { return _receiveBufferSize; }
            set { _receiveBufferSize = value; }
        }

        /// <summary>
        /// 最大连接数
        /// 连接的最大数量的样本设计为同时处理
        /// Gets or sets the max connections.
        /// </summary>
        /// <value>The max connections.</value>
        public int MaxConnections
        {
            get { return _maxConnections; }
            set { _maxConnections = value; }
        }

        /// <summary>
        ///  包策略
        /// </summary>
        public IPackStrategy PackStrategy
        {
            get { return _packStrategy; }
            set { _packStrategy = value; }
        }

        public SocketServerSettings()
        {

        }


    }
}
