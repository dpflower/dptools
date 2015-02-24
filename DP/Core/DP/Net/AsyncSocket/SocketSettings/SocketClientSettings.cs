using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using DP.Net.AsyncSocket.Packet;

namespace DP.Net.AsyncSocket
{
    public class SocketClientSettings : ISocketSettings
    {
        IPackStrategy _packStrategy = new DefaultPackStrategy();     

        int _receiveBufferSize = 1024;

        Encoding _encoding = Encoding.UTF8;

        IPEndPoint _remoteEndPoint;

        /// <summary>
        /// 远程IP 端口
        /// Gets or sets the remote end point.
        /// </summary>
        /// <value>The remote end point.</value>
        public IPEndPoint RemoteEndPoint
        {
            get { return _remoteEndPoint; }
            set { _remoteEndPoint = value; }
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
        ///  包策略
        /// </summary>
        public IPackStrategy PackStrategy
        {
            get { return _packStrategy; }
            set { _packStrategy = value; }
        }

        public SocketClientSettings()
        {

        }
    }
}
