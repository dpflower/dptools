using System;
namespace DP.Net.AsyncSocket
{
    public interface ISocketSettings
    {
        System.Text.Encoding Encoding { get; set; }
        DP.Net.AsyncSocket.Packet.IPackStrategy PackStrategy { get; set; }
    }
}
