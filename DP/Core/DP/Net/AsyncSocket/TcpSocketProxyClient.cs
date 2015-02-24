using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace DP.Net.AsyncSocket
{
    public class TcpSocketProxyClient
    {
        public MessageEventHandler onMessage;
        public ReceiveCompletedEventHandler onReceiveCompleted;
        public SendCompletedEventHandler onSendCompleted;
        public ConnectedEventHandler onConnected;
        public ServerDisconnectEventHandler onServerDisconnect;

        #region 私有变量
        /// <summary>
        /// 连接信号量
        /// </summary>
        ManualResetEvent _clientDone = new ManualResetEvent(false);
        /// <summary>
        /// 客户端连接Socket
        /// </summary>
        Socket _clientSocket;
        /// <summary>
        /// 连接状态
        /// </summary>
        bool _connected = false;
        /// <summary>
        /// 客户端设置类 对象
        /// </summary>
        SocketClientSettings _settings;
        /// <summary>
        /// 计数器由服务器收到的字节总数为
        /// </summary>
        int _totalBytesRead;
        /// <summary>
        /// 发送 SAEA 对像
        /// </summary>
        SocketAsyncEventArgs _sendSocketEventArgs;
        /// <summary>
        /// 接收 SAEA 对像
        /// </summary>
        SocketAsyncEventArgs _receiveSocketEventArgs;
        #endregion

        #region 属性
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool Connected
        {
            get { return _connected; }
            set { _connected = value; }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpSocketClient"/> class.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        public TcpSocketProxyClient()
        {
            _settings = new SocketClientSettings();
            _settings.Encoding = Encoding.UTF8;
            _settings.ReceiveBufferSize = 32768;
        }

        #region 公共方法
        /// <summary>
        /// 连接服务端
        /// Connects this instance.
        /// </summary>
        /// <returns></returns>
        public bool Connect(string ip, int port)
        {
            if (_connected)
            {
                return true;
            }

            IPAddress _ipAddress = IPAddress.Parse(ip);
            _settings.RemoteEndPoint = new IPEndPoint(_ipAddress, port);
            _clientSocket = new Socket(_settings.RemoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _sendSocketEventArgs = new SocketAsyncEventArgs();
            _sendSocketEventArgs.UserToken = new AsyncUserToken();
            _receiveSocketEventArgs = new SocketAsyncEventArgs();
            _receiveSocketEventArgs.UserToken = new AsyncUserToken();

            (_sendSocketEventArgs.UserToken as AsyncUserToken).Socket = this._clientSocket;
            (_sendSocketEventArgs.UserToken as AsyncUserToken).PackStrategy = _settings.PackStrategy;
            _sendSocketEventArgs.RemoteEndPoint = _settings.RemoteEndPoint;
            _sendSocketEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
            _clientSocket.ConnectAsync(_sendSocketEventArgs);

            _clientDone.WaitOne();

            if (_sendSocketEventArgs.SocketError == SocketError.Success)
            {
                byte[] receiveBuffer = new byte[_settings.ReceiveBufferSize];
                (_receiveSocketEventArgs.UserToken as AsyncUserToken).Socket = this._clientSocket;
                (_receiveSocketEventArgs.UserToken as AsyncUserToken).PackStrategy = _settings.PackStrategy;
                _receiveSocketEventArgs.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);
                _receiveSocketEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                this._clientSocket.ReceiveAsync(_receiveSocketEventArgs);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 断开连接
        /// Disconnects this instance.
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            if (!_connected)
            {
                return false;
            }
            _clientSocket.DisconnectAsync(_sendSocketEventArgs);
            //_clientSocket.Disconnect(false);
            //_connected = false;
            return true;
        }

        /// <summary>
        /// 发送数据到服务器
        /// Sends the data to server.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public bool SendDataToServer(string data)
        {
            if (!_connected)
            {
                return false;
            }
            if (String.IsNullOrEmpty(data))
            {
                return false;
            }
            byte[] buffer = _settings.Encoding.GetBytes(data);
            AsyncUserToken token = _sendSocketEventArgs.UserToken as AsyncUserToken;
            buffer = token.SendDataHandle(buffer);
            _sendSocketEventArgs.SetBuffer(buffer, 0, buffer.Length);
            bool willRaiseEvent = _clientSocket.SendAsync(_sendSocketEventArgs);
            if (!willRaiseEvent)
            {
                ProcessSend(_sendSocketEventArgs);
            }
            return true;
        }
        #endregion

        #region 事件
        /// <summary>
        /// This method is called whenever a receive or send opreation is completed on a socket
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">SocketAsyncEventArg associated with the completed receive operation</param>
        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            //  AsyncUserToken token = (AsyncUserToken)e.UserToken;
            //  determine which type of operation just completed and call the associated handler
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    {
                        ProcessConnect(e);
                    }
                    break;
                case SocketAsyncOperation.Receive:
                    {
                        ProcessReceive(e);
                    }
                    break;
                case SocketAsyncOperation.Send:
                    {
                        ProcessSend(e);
                    }
                    break;
                case SocketAsyncOperation.Disconnect:
                    {
                        ProcessDisconnect(e);
                    }
                    break;
                default:
                    {
                        throw new ArgumentException("The last operation completed on the socket was not a receive or send");
                    }
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// Processes the connect.
        /// </summary>
        /// <param name="e">The <see cref="System.Net.Sockets.SocketAsyncEventArgs"/> instance containing the event data.</param>
        private void ProcessConnect(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            _clientDone.Set();
            _connected = (e.SocketError == SocketError.Success);
            if (_connected)
            {
                if (this.onConnected != null && token != null)
                {
                    this.onConnected(e, token.Socket.RemoteEndPoint.ToString());
                }
                if (this.onMessage != null && token != null)
                {
                    this.onMessage(String.Format("服务器 {0} 连接成功！。", token.Socket.RemoteEndPoint.ToString()));
                }
            }
            else
            {
                if (this.onMessage != null && token != null)
                {
                    this.onMessage(String.Format("服务器 连接失败！。"));
                }

            }
        }
        /// <summary>
        /// Processes the receive.
        /// </summary>
        /// <param name="e">The <see cref="System.Net.Sockets.SocketAsyncEventArgs"/> instance containing the event data.</param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                byte[] data = new byte[e.BytesTransferred];
                Array.Copy(e.Buffer, e.Offset, data, 0, data.Length);

                List<byte[]> list = token.ReceivedDataHandle(data);

                foreach (byte[] receivedArray in list)
                {
                    if (this.onReceiveCompleted != null && token != null)
                    {
                        this.onReceiveCompleted(e, token.Socket.RemoteEndPoint.ToString(), _settings.Encoding.GetString(receivedArray, 0, receivedArray.Length));
                    }
                    if (this.onMessage != null && token != null)
                    {
                        this.onMessage(String.Format("接受服务端 {0} 数据， 数据内容为：{1}　！ ", token.Socket.RemoteEndPoint.ToString(), _settings.Encoding.GetString(receivedArray, 0, receivedArray.Length)));
                    }

                }

                //increment the count of the total bytes receive by the server
                Interlocked.Add(ref _totalBytesRead, e.BytesTransferred);

                //echo the data received back to the client
                bool willRaiseEvent = token.Socket.ReceiveAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessSend(e);
                }

            }
            else
            {
                CloseClientSocket(e);
            }

        }
        /// <summary>
        /// Processes the send.
        /// </summary>
        /// <param name="e">The <see cref="System.Net.Sockets.SocketAsyncEventArgs"/> instance containing the event data.</param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                byte[] data = new byte[e.BytesTransferred];
                Array.Copy(e.Buffer, e.Offset, data, 0, data.Length);
                List<byte[]> list = token.ReceivedDataHandle(data);
                foreach (byte[] receivedArray in list)
                {
                    if (this.onSendCompleted != null && token != null)
                    {
                        this.onSendCompleted(e, token.Socket.RemoteEndPoint.ToString(), _settings.Encoding.GetString(receivedArray, 0, receivedArray.Length));
                    }
                    if (this.onMessage != null && token != null)
                    {
                        this.onMessage(String.Format("向服务端 {0} 发送数据， 数据内容为：{1}　！ ", token.Socket.RemoteEndPoint.ToString(), _settings.Encoding.GetString(receivedArray, 0, receivedArray.Length)));
                    }
                }
            }
            else
            {
                CloseClientSocket(e);
            }
        }
        /// <summary>
        /// Processes the disconnect.
        /// </summary>
        /// <param name="e">The <see cref="System.Net.Sockets.SocketAsyncEventArgs"/> instance containing the event data.</param>
        private void ProcessDisconnect(SocketAsyncEventArgs e)
        {
            _connected = false;
            AsyncUserToken token = e.UserToken as AsyncUserToken;

            string remoteEndPoint = token.Socket.RemoteEndPoint.ToString();

            if (this.onServerDisconnect != null && token != null)
            {
                this.onServerDisconnect(e, remoteEndPoint);
            }
            if (this.onMessage != null && token != null)
            {
                this.onMessage(String.Format("服务器 {0} 已断开。", remoteEndPoint));
            }

        }
        /// <summary>
        /// Closes the client socket.
        /// </summary>
        /// <param name="e">The <see cref="System.Net.Sockets.SocketAsyncEventArgs"/> instance containing the event data.</param>
        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;

            string remoteEndPoint = "";
            // close the socket associated with the client
            //与客户密切相关的插座
            try
            {
                remoteEndPoint = token.Socket.RemoteEndPoint.ToString();
                token.Socket.Shutdown(SocketShutdown.Send);
            }
            // throws if client process has already closed
            //如果客户端进程抛出已关闭
            catch (Exception ex)
            {

            }
            token.Socket.Close();
            _connected = false;

            if (this.onServerDisconnect != null)
            {
                this.onServerDisconnect(e, remoteEndPoint);
            }
            if (this.onMessage != null)
            {
                this.onMessage(String.Format("服务器 {0} 已断开。", remoteEndPoint));
            }

        }
        #endregion
    }
}
