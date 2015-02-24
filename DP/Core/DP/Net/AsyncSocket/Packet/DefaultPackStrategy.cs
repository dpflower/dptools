using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DP.Common;

namespace DP.Net.AsyncSocket.Packet
{
    public class DefaultPackStrategy : IPackStrategy
    {
        IAsyncUserToken _asyncUserToken;

        public IAsyncUserToken AsyncUserToken
        {
            get { return _asyncUserToken; }
            set { _asyncUserToken = value; }
        }

        byte[] _packetHeader = new byte[] { byte.MaxValue, byte.MinValue, byte.MaxValue, byte.MinValue, byte.MaxValue, byte.MinValue };
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

        public void PacketSegmentation(ref List<byte[]> list, ref byte[] receivedArray)
        {
            if (receivedArray.Length > _packetHeader.Length + 4)
            {
                #region 判断包头是否相同。不相同，则整包丢弃
                for (int i = 0; i < _packetHeader.Length; i++)
                {
                    if (receivedArray[i] != _packetHeader[i])
                    {
                        DP.Common.LogHelper.WriteLogAsync("LostPacket", String.Format("LostPacket.Length:{0}|LostPacket.Content:{1}", receivedArray.Length, Encoding.UTF8.GetString(receivedArray)));
                        ByteHelper.Remove(ref receivedArray, receivedArray.Length);
                        return;
                    }
                } 
                #endregion

                byte[] msgLenArray = new byte[4];
                Array.Copy(receivedArray, _packetHeader.Length, msgLenArray, 0, 4);
                Array.Reverse(msgLenArray);

                int messageLength = BitConverter.ToInt32(msgLenArray, 0);
                if (receivedArray.Length >= messageLength)
                {
                    byte[] temp = new byte[messageLength - _packetHeader.Length - 4];
                    Array.Copy(receivedArray, _packetHeader.Length + 4, temp, 0, temp.Length);
                    list.Add(temp);
                    ByteHelper.Remove(ref receivedArray, messageLength);
                    PacketSegmentation(ref list, ref receivedArray);
                }
            }
        }

        public byte[] Packet(byte[] sendArray)
        {
            int messageLength = _packetHeader.Length + 4 + sendArray.Length;
            byte[] msgLenArray = BitConverter.GetBytes(messageLength);
            Array.Reverse(msgLenArray);
            byte[] sendData = new byte[messageLength];

            int i = 0;
            Array.Copy(_packetHeader, 0, sendData, i, _packetHeader.Length);
            i +=  _packetHeader.Length;
            Array.Copy(msgLenArray, 0, sendData, i, msgLenArray.Length);
            i += msgLenArray.Length;
            Array.Copy(sendArray, 0, sendData, i, sendArray.Length);
            return sendData;
        }
    }
}
