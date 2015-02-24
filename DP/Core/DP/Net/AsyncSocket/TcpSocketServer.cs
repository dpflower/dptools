using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Collections;
using DP.Net.AsyncSocket.Packet;

namespace DP.Net.AsyncSocket
{

    public class TcpSocketServer
    {
        public MessageEventHandler onMessage;
        public ServerStartEventHandler onServerStart;
        public ServerStopEventHandler onServerStop;
        public ReceiveCompletedEventHandler onReceiveCompleted;
        public SendCompletedEventHandler onSendCompleted;
        public ClientConnectionRequestEventHandler onClientConnectionRequest;
        public ClientDisconnectEventHandler onClientDisconnect;

        #region 私有变量
        /// <summary>
        /// 缓冲区管理
        /// 代表了所有的大型缓冲器可重复使用的设置套接字操作
        /// </summary>
        BufferManager _bufferManager;

        /// <summary>
        /// 服务器端设置
        /// </summary>
        SocketServerSettings _settings;

        /// <summary>
        /// 计数器由服务器收到的字节总数为 #
        /// </summary>
        int _totalBytesRead;

        /// <summary>
        /// 客户总数的连接到服务器
        /// </summary>
        int _numConnectedSockets;

        /// <summary>
        /// 套接字用于侦听传入的连接请求
        /// </summary>
        Socket listenSocket;

        /// <summary>
        /// SAEA object pool
        /// </summary>
        DoubleSocketAsyncEventArgsPool _readWritePool;

        /// <summary>
        /// 一个信号有两个参数，可用插槽最初数量和插槽的最大数量。
        /// 我们会让他们一样。
        /// 这个信号是用来避免使自己超过最大连接＃。 
        /// （这是不是真的在这里控制线程。）
        /// </summary>
        Semaphore _maxNumberAcceptedClients;

        /// <summary>
        /// 服务同步锁
        /// </summary>
        private static Mutex mutex = new Mutex();

        /// <summary>
        /// 服务状态
        /// </summary>
        ServerStates _serverState = ServerStates.Stoped;

        /// <summary>
        /// 在线客户端 double SAEA 对像
        /// </summary>
        Hashtable _doubleSocketAsyncEventArgs;
        #endregion

        public ServerStates ServerState
        {
            get { return _serverState; }
            //set { _serverState = value; }
        }

        /// <summary>
        /// 创建一个未初始化的服务器实例。要启动服务器侦听连接请求调用init方法，其次是开始的方法
        /// Initializes a new instance of the <see cref="TcpSocketServer"/> class.
        /// </summary>
        /// <param name="numConnections">连接的最大数量的样本设计为同时处理 The num connections.</param>
        /// <param name="receiveBufferSize">缓冲区大小为每个套接字I / O操作 Size of the receive buffer.</param>
        public TcpSocketServer(int numConnections, int receiveBufferSize)
        {
            _settings = new SocketServerSettings();
            _serverState = ServerStates.Initialing;
            _totalBytesRead = 0;
            _numConnectedSockets = 0;
            _settings.MaxConnections = numConnections;
            _settings.ReceiveBufferSize = receiveBufferSize;
            // allocate buffers such that the maximum number of sockets can have one outstanding read and 
            //write posted to the socket simultaneously  
            _bufferManager = new BufferManager(_settings.ReceiveBufferSize * _settings.MaxConnections * _settings.OpsToPreAlloc, receiveBufferSize);

            _readWritePool = new DoubleSocketAsyncEventArgsPool(numConnections);
            _doubleSocketAsyncEventArgs = Hashtable.Synchronized(new Hashtable()); //
            _maxNumberAcceptedClients = new Semaphore(numConnections, numConnections);

            Init();
        }

        #region 公共方法

        /// <summary>
        /// 启动服务器，这样它是传入的连接请求听。
        /// </summary>
        /// <param name="localEndPoint">The endpoint which the server will listening for conenction requests on</param>
        public void Start(IPEndPoint localEndPoint)
        {
            _settings.LocalEndPoint = localEndPoint;

            listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(localEndPoint);

            listenSocket.Listen(_settings.ListenBacklog);

            // post accepts on the listening socket
            StartAccept(null);

            _serverState = ServerStates.Running;

            if (this.onMessage != null)
            {
                this.onMessage(String.Format("开始TCP监听{0}！", listenSocket.LocalEndPoint.ToString()));
            }
            if (this.onServerStart != null)
            {
                this.onServerStart();
            }
            mutex.WaitOne();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            ArrayList arr = new ArrayList();
            foreach (DictionaryEntry de in _doubleSocketAsyncEventArgs)
            {
                arr.Add(de.Key.ToString());
            }
            for (int i = 0; i < arr.Count; i++)
            {
                DoubleSocketAsyncEventArgs dou = _doubleSocketAsyncEventArgs[arr[i]] as DoubleSocketAsyncEventArgs;
                if (dou != null)
                {
                    (dou.SendSAEA.UserToken as AsyncUserToken).Socket.DisconnectAsync(dou.SendSAEA);
                    //CloseClientSocket(dou.SendSAEA);
                }
            }
            //foreach (DoubleSocketAsyncEventArgs dou in _doubleSocketAsyncEventArgs.Values)
            //{
            //    if (dou != null)
            //    {
            //        (dou.SendSAEA.UserToken as AsyncUserToken).Socket.DisconnectAsync(dou.SendSAEA);
            //        //CloseClientSocket(dou.SendSAEA);
            //    }
            //}
            if (listenSocket != null)
            {
                listenSocket.Close();
            }
            listenSocket = null;

            mutex.ReleaseMutex();
            _serverState = ServerStates.Stoped;

            if (this.onServerStop != null)
            {
                this.onServerStop();
            }
            if (this.onMessage != null)
            {
                this.onMessage(String.Format("停止TCP监听！"));
            }
        }

        /// <summary>
        /// 向全部在线客户端发送信息
        /// Sends the data to client.
        /// </summary>
        /// <param name="data">The data.</param>
        public void SendDataToClient(string data)
        {
            foreach (string remoteEndPoint in _doubleSocketAsyncEventArgs.Keys)
            {
                SendDataToClient(remoteEndPoint, data);
            }
        }

        /// <summary>
        /// 向指定客户面发送信息
        /// Sends the data to client.
        /// </summary>
        /// <param name="remoteEndPoint">The remote end point.</param>
        /// <param name="data">The data.</param>
        public void SendDataToClient(string remoteEndPoint, string data)
        {
            DoubleSocketAsyncEventArgs eventArgs = _doubleSocketAsyncEventArgs[remoteEndPoint] as DoubleSocketAsyncEventArgs;
            if (eventArgs != null)
            {
                byte[] buffer = _settings.Encoding.GetBytes(data);
                AsyncUserToken token = eventArgs.SendSAEA.UserToken as AsyncUserToken;
                buffer = token.SendDataHandle(buffer);
                eventArgs.SendSAEA.SetBuffer(buffer, 0, buffer.Length);
                bool willRaiseEvent = (eventArgs.SendSAEA.UserToken as AsyncUserToken).Socket.SendAsync(eventArgs.SendSAEA);
                if (!willRaiseEvent)
                {
                    ProcessSend(eventArgs.SendSAEA);
                }
            }
        }

        /// <summary>
        /// 在线客户端列表
        /// Gets the client list.
        /// </summary>
        /// <returns></returns>
        public Hashtable GetClientList()
        {
            return _doubleSocketAsyncEventArgs;
        }

        /// <summary>
        /// 在线客户端数
        /// Gets the client count.
        /// </summary>
        /// <returns></returns>
        public long GetClientCount()
        {
            if (_doubleSocketAsyncEventArgs == null)
            {
                return 0;
            }
            return _doubleSocketAsyncEventArgs.Count;
        }

        #endregion

        #region 事件

        /// <summary>
        /// 开始接受来自客户端的连接请求 
        /// </summary>
        /// <param name="acceptEventArg">The context object to use when issuing the accept operation on the 
        /// server's listening socket</param>
        public void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                // socket must be cleared since the context object is being reused
                acceptEventArg.AcceptSocket = null;
            }

            _maxNumberAcceptedClients.WaitOne();
            bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArg);
            }
        }

        /// <summary>
        /// This method is called whenever a receive or send opreation is completed on a socket
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">SocketAsyncEventArg associated with the completed receive operation</param>
        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                case SocketAsyncOperation.Disconnect:

                    break;
                default:
                    {
                        throw new ArgumentException("The last operation completed on the socket was not a receive or send");
                    }
            }
        }

        /// <summary>
        /// 此方法是回调方法和与业务有关的Socket.AcceptAsync调用
        /// 当一个接受操作完成
        /// </summary>
        void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            AsyncUserToken userToken = e.UserToken as AsyncUserToken;
            if (this.onMessage != null && userToken != null)
            {
                this.onMessage(String.Format("客户端 {0} 请求连接！", userToken.Socket.LocalEndPoint.ToString()));
            }
            ProcessAccept(e);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 通过预先分配初始化缓冲区和可重复使用的上下文对象的服务器。这些对象不
        /// 初始化需要预分配或重复使用，通过这种方式来完成说明如何使用的API可以很容易地
        /// 创建可重用的对象来提高服务器的性能。
        /// </summary>
        private void Init()
        {
            // Allocates one large byte buffer which all I/O operations use a piece of.  This gaurds 
            // against memory fragmentation
            _bufferManager.InitBuffer();

            // preallocate pool of SocketAsyncEventArgs objects
            DoubleSocketAsyncEventArgs readWriteEventArg;

            for (int i = 0; i < _settings.MaxConnections; i++)
            {
                //Pre-allocate a set of reusable SocketAsyncEventArgs
                readWriteEventArg = new DoubleSocketAsyncEventArgs();
                readWriteEventArg.ReceiveSAEA.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                readWriteEventArg.SendSAEA.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                readWriteEventArg.ReceiveSAEA.UserToken = new AsyncUserToken(_settings);
                readWriteEventArg.SendSAEA.UserToken = new AsyncUserToken(_settings);

                // assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
                _bufferManager.SetBuffer(readWriteEventArg.ReceiveSAEA);

                // add SocketAsyncEventArg to the pool
                _readWritePool.Push(readWriteEventArg);
            }
            _serverState = ServerStates.Inited;
        }

        /// <summary>
        /// 数据接收后处理方法
        /// Processes the receive.
        /// </summary>
        /// <param name="e">The <see cref="System.Net.Sockets.SocketAsyncEventArgs"/> instance containing the event data.</param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            // check if the remote host closed the connection
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                AsyncUserToken token = (AsyncUserToken)e.UserToken;
                byte[] data = new byte[e.BytesTransferred];
                Array.Copy(e.Buffer, e.Offset, data, 0, data.Length);
                List<byte[]> list = token.ReceivedDataHandle(data);

                foreach (byte[] receivedArray in list)
                {
                    if (this.onReceiveCompleted != null && token != null)
                    {
                        this.onReceiveCompleted(e, token.Socket.RemoteEndPoint.ToString(), _settings.Encoding.GetString(receivedArray, 0, receivedArray.Length));
                    }
                }

                //increment the count of the total bytes receive by the server
                Interlocked.Add(ref _totalBytesRead, e.BytesTransferred);

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
        /// 数据发送后处理方法
        /// Processes the send.
        /// </summary>
        /// <param name="e">The <see cref="System.Net.Sockets.SocketAsyncEventArgs"/> instance containing the event data.</param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                AsyncUserToken token = (AsyncUserToken)e.UserToken;
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
                        this.onMessage(String.Format("向客户端 {0} 发送数据， 数据内容为：{1}　！ ", token.Socket.RemoteEndPoint.ToString(), _settings.Encoding.GetString(receivedArray, 0, receivedArray.Length)));
                    }
                }
            }
            else
            {
                CloseClientSocket(e);
            }
        }

        /// <summary>
        /// Processes the accept.
        /// </summary>
        /// <param name="e">The <see cref="System.Net.Sockets.SocketAsyncEventArgs"/> instance containing the event data.</param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (e.LastOperation != SocketAsyncOperation.Accept)
            {
                return;
            }
            if (e.AcceptSocket == null || e.AcceptSocket.RemoteEndPoint == null)    //检查发送的长度是否大于0,不是就返回
            {
                return;
            }

            Interlocked.Increment(ref _numConnectedSockets);
            if (this.onMessage != null)
            {
                this.onMessage(String.Format("接受客户端连接。有连接 {0} 客户连到服务器, SocketAsyncEventArgsPool : {1} !", _numConnectedSockets, this._readWritePool.Count));
            }

            //创建双向SAEA 对像。
            DoubleSocketAsyncEventArgs readEventArgs = _readWritePool.Pop();
            AsyncUserToken token = new AsyncUserToken();
            token.PackStrategy = _settings.PackStrategy;
            token.Socket = e.AcceptSocket;
            readEventArgs.ReceiveSAEA.UserToken = token;
            readEventArgs.SendSAEA.UserToken = token;
            readEventArgs.UID = e.AcceptSocket.RemoteEndPoint.ToString();
            readEventArgs.ConnectedTime = DateTime.Now;
            _doubleSocketAsyncEventArgs.Add(e.AcceptSocket.RemoteEndPoint.ToString(), readEventArgs);


            if (this.onClientConnectionRequest != null)
            {
                this.onClientConnectionRequest(e, e.AcceptSocket.RemoteEndPoint.ToString(), _numConnectedSockets);
            }

            // As soon as the client is connected, post a receive to the connection
            bool willRaiseEvent = e.AcceptSocket.ReceiveAsync(readEventArgs.ReceiveSAEA);
            if (!willRaiseEvent)
            {
                ProcessReceive(readEventArgs.ReceiveSAEA);
            }

            StartAccept(e);
        }

        /// <summary>
        /// 关闭客户端 Socket 
        /// Closes the client socket.
        /// </summary>
        /// <param name="e">The <see cref="System.Net.Sockets.SocketAsyncEventArgs"/> instance containing the event data.</param>
        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;
            string remoteEndPoint = token.Socket.RemoteEndPoint.ToString();

            // close the socket associated with the client
            //与客户密切相关的插座
            try
            {
                token.Socket.Shutdown(SocketShutdown.Send);
            }
            // throws if client process has already closed
            //如果客户端进程抛出已关闭
            catch (Exception)
            {

            }
            token.Socket.Close();

            // decrement the counter keeping track of the total number of clients connected to the server
            // 递减的客户总数计数器保持跟踪连接到服务器
            Interlocked.Decrement(ref _numConnectedSockets);
            _maxNumberAcceptedClients.Release();

            if (this.onMessage != null && token != null)
            {
                this.onMessage(String.Format("客户端 {1} 已断开服务器。有连接 {0} 客户连到服务器", _numConnectedSockets, remoteEndPoint));
            }

            if (this.onClientDisconnect != null && token != null)
            {
                this.onClientDisconnect(e, remoteEndPoint, _numConnectedSockets);
            }
            // Free the SocketAsyncEventArg so they can be reused by another client
            //免费SocketAsyncEventArg，使他们能够重复使用另一个客户端
            DoubleSocketAsyncEventArgs readWriteEventArg = _doubleSocketAsyncEventArgs[remoteEndPoint] as DoubleSocketAsyncEventArgs;
            _doubleSocketAsyncEventArgs.Remove(remoteEndPoint);
            readWriteEventArg.ConnectedTime = DateTime.MinValue;
            readWriteEventArg.UID = String.Empty;
            _readWritePool.Push(readWriteEventArg);
        }
        #endregion

    }
}
