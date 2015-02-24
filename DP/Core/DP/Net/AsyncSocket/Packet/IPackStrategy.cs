using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DP.Net.AsyncSocket.Packet
{
    public interface IPackStrategy
    {
        void PacketSegmentation(ref List<byte[]> list, ref byte[] receivedArray);
        byte[] Packet(byte[] sendArray);
    }
}
