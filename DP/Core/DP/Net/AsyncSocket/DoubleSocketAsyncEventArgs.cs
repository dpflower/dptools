using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace DP.Net.AsyncSocket
{
    public class DoubleSocketAsyncEventArgs : IDisposable
    {
        private string _uID = string.Empty;
        private DateTime _connectedTime = DateTime.MinValue;
        private SocketAsyncEventArgs _receiveSAEA;
        private SocketAsyncEventArgs _sendSAEA;

        /// <summary>
        /// DoubleSocketAsyncEventArgs 对像UID
        /// Gets or sets the UID.
        /// </summary>
        /// <value>The UID.</value>
        public string UID
        {
            get { return _uID; }
            set { _uID = value; }
        }

        /// <summary>
        /// Gets or sets the connected time.
        /// </summary>
        /// <value>The connected time.</value>
        public DateTime ConnectedTime
        {
            get { return _connectedTime; }
            set { _connectedTime = value; }
        }

        /// <summary>
        /// 接收 SocketAsyncEventArgs 对像
        /// Gets or sets the receive SAEA.
        /// </summary>
        /// <value>The receive SAEA.</value>
        public SocketAsyncEventArgs ReceiveSAEA
        {
            get { return _receiveSAEA; }
            set { _receiveSAEA = value; }
        }

        /// <summary>
        /// 发送 SocketAsyncEventArgs 对像 
        /// Gets or sets the send SAEA.
        /// </summary>
        /// <value>The send SAEA.</value>
        public SocketAsyncEventArgs SendSAEA
        {
            get { return _sendSAEA; }
            set { _sendSAEA = value; }
        }

        public DoubleSocketAsyncEventArgs()
        {
            _receiveSAEA = new SocketAsyncEventArgs();
            _sendSAEA = new SocketAsyncEventArgs();
        }




        #region IDisposable 成员

        public void Dispose()
        {
            _uID = string.Empty;
            _sendSAEA.Dispose();
            _receiveSAEA.Dispose();
        }

        #endregion
    }
}
