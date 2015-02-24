using System;
using System.Collections.Generic;
using System.Text;
using DP.Common;
using System.Net.Sockets;
using DP.Net.AsyncSocket.Packet;

namespace DP.Net.AsyncSocket
{
    /// <summary>
    /// 这个类的目的是为将要分配给SocketAsyncEventArgs.UserToken属性对象的使用。
    /// This class is designed for use as the object to be assigned to the SocketAsyncEventArgs.UserToken property.
    /// </summary>
    public class AsyncUserToken : IAsyncUserToken
    {
        Socket m_socket;

        byte[] _packetHeader = new byte[] { byte.MaxValue, byte.MinValue, byte.MaxValue, byte.MinValue, byte.MaxValue, byte.MinValue };

        byte[] _receivedBuffer;

        object objLock = new object();
        object _syncRoot = new object();
        public object SyncRoot
        {
            get
            {
                return _syncRoot;
            }
        }

        /// <summary>
        /// Gets or sets the received buffer.
        /// </summary>
        /// <value>The received buffer.</value>
        public byte[] ReceivedBuffer
        {
            get { return _receivedBuffer; }
            set { _receivedBuffer = value; }
        }

        /// <summary>
        /// 数据包头
        /// Gets or sets the packet header.
        /// </summary>
        /// <value>The packet header.</value>
        public byte[] PacketHeader
        {
            get { return _packetHeader; }
            set { _packetHeader = value; }
        }

        /// <summary>
        /// 包策略
        /// </summary>
        IPackStrategy _packStrategy;

        /// <summary>
        /// 包策略
        /// </summary>
        public IPackStrategy PackStrategy
        {
            get { return _packStrategy; }
            set { _packStrategy = value; }
        }



        /// <summary>
        /// 接收数据处理
        /// Receiveds the data handle.
        /// </summary>
        /// <param name="receivedData">The received data.</param>
        /// <returns></returns>
        public List<byte[]> ReceivedDataHandle(byte[] receivedData)
        {
            List<byte[]> list = new List<byte[]>();
            lock (_syncRoot)
            {
                ByteHelper.Merge(ref _receivedBuffer, receivedData);
            }
            _packStrategy.PacketSegmentation(ref list, ref _receivedBuffer);            
            return list;
        }

        /// <summary>
        /// 发送数据处理
        /// Sends the data handle.
        /// </summary>
        /// <param name="sendData">The send data.</param>
        /// <returns></returns>
        public byte[] SendDataHandle(byte[] sendData)
        {
            return _packStrategy.Packet(sendData);
        }

        /// <summary>
        /// 通过 递归 对接收缓存 进行分包
        /// Packets the segmentation.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="receivedArray">The received array.</param>
        private void PacketSegmentation(ref List<byte[]> list, byte[] receivedArray)
        {
            if (receivedArray.Length > _packetHeader.Length + 2)
            {
                for (int i = 0; i < _packetHeader.Length; i++)
                {
                    if (receivedArray[i] != _packetHeader[i])
                    {
                        DP.Common.LogHelper.WriteLogAsync("LostPacket", String.Format("LostPacket.Length:{0}|LostPacket.Content:{1}", _receivedBuffer.Length, Encoding.UTF8.GetString(_receivedBuffer)));
                        if (_receivedBuffer.Length >= receivedArray.Length)
                        {
                            lock (objLock)
                            {
                                if (_receivedBuffer.Length >= receivedArray.Length)
                                {
                                    ByteHelper.Remove(ref _receivedBuffer, receivedArray.Length);
                                }
                            }
                        }
                        else
                        {
                            lock (objLock)
                            {
                                ByteHelper.Remove(ref _receivedBuffer, _receivedBuffer.Length);
                            }
                        }
                        return;
                    }
                }

                int messageLength = (receivedArray[_packetHeader.Length] << 8) + receivedArray[_packetHeader.Length + 1];
                if (messageLength == receivedArray.Length)
                {
                    byte[] temp = new byte[messageLength - _packetHeader.Length - 2];
                    Array.Copy(receivedArray, _packetHeader.Length + 2, temp, 0, temp.Length);
                    list.Add(temp);
                    if (_receivedBuffer.Length >= messageLength)
                    {
                        lock (objLock)
                        {
                            if (_receivedBuffer.Length >= messageLength)
                            {
                                ByteHelper.Remove(ref _receivedBuffer, messageLength);
                            }
                        }
                    }
                    else
                    {
                        //DP.Common.LogHelper.WriteLogAsync("received", String.Format("Buffer:{0}|messageLength:{1}", _receivedBuffer.Length, messageLength));
                        lock (objLock)
                        {
                            ByteHelper.Remove(ref _receivedBuffer, _receivedBuffer.Length);
                        }
                    }
                }
                else if (messageLength < receivedArray.Length)
                {
                    byte[] temp = new byte[messageLength - _packetHeader.Length - 2];
                    Array.Copy(receivedArray, _packetHeader.Length + 2, temp, 0, temp.Length);
                    list.Add(temp);
                    if (_receivedBuffer.Length >= messageLength)
                    {
                        lock (objLock)
                        {
                            if (_receivedBuffer.Length >= messageLength)
                            {
                                ByteHelper.Remove(ref _receivedBuffer, messageLength);
                            }
                        }
                    }
                    else
                    {
                        //DP.Common.LogHelper.WriteLogAsync("received", String.Format("Buffer:{0}|messageLength:{1}", _receivedBuffer.Length, messageLength));
                        lock (objLock)
                        {
                            ByteHelper.Remove(ref _receivedBuffer, _receivedBuffer.Length);
                        }
                    }
                    PacketSegmentation(ref list, _receivedBuffer);
                }
            }
        }

        public AsyncUserToken() : this(null, null) { }

        public AsyncUserToken(Socket socket) : this(socket, null) { }

        public AsyncUserToken(ISocketSettings settings) : this(null, settings) { }

        public AsyncUserToken(Socket socket, ISocketSettings settings)
        {
            m_socket = socket;
            if (settings != null)
            {
                _packStrategy = settings.PackStrategy;
            }
        }


        public Socket Socket
        {
            get { return m_socket; }
            set { m_socket = value; }
        }



    }
}
