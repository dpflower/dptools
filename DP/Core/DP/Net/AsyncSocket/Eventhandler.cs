using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace DP.Net.AsyncSocket
{
    public delegate void MessageEventHandler(string message);
    public delegate void ServerStartEventHandler();
    public delegate void ServerStopEventHandler();
    public delegate void ReceiveCompletedEventHandler(SocketAsyncEventArgs e, string remoteEndPoint, string receive);
    public delegate void SendCompletedEventHandler(SocketAsyncEventArgs e, string remoteEndPoint, string send);
    public delegate void ClientConnectionRequestEventHandler(SocketAsyncEventArgs e, string remoteEndPoint, int ConnectedSockets);
    public delegate void ClientDisconnectEventHandler(SocketAsyncEventArgs e, string remoteEndPoint, int ConnectedSockets);
    public delegate void ConnectedEventHandler(SocketAsyncEventArgs e, string remoteEndPoint);
    public delegate void ServerDisconnectEventHandler(SocketAsyncEventArgs e, string remoteEndPoint);

}
