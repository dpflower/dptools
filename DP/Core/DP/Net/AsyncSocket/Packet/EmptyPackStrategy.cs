using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DP.Common;

namespace DP.Net.AsyncSocket.Packet
{
    public class EmptyPackStrategy : IPackStrategy
    {       
        public void PacketSegmentation(ref List<byte[]> list, ref byte[] receivedArray)
        {
            list.Add(receivedArray);
            ByteHelper.Remove(ref receivedArray, receivedArray.Length);
        }

        public byte[] Packet(byte[] sendArray)
        {
            return sendArray;
        }




       
    }
}
