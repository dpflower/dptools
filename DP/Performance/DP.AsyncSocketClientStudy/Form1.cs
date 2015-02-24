using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace AsyncSocketClientStudy
{
    public partial class Form1 : Form
    {
        private delegate void SendMessageToFormEventhandler(string message);
        private SendMessageToFormEventhandler SendInfo;

        static ManualResetEvent clientDone = new ManualResetEvent(false);
        IPAddress destinationAddr = null;          // IP Address of server to connect to
        int destinationPort = 0;                   // Port number of server
        SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            destinationAddr = IPAddress.Parse(txtIP.Text);
            destinationPort = int.Parse(txtPort.Text);
            Socket sock = new Socket(destinationAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(SocketEventArg_Completed);
            socketEventArg.RemoteEndPoint = new IPEndPoint(destinationAddr, destinationPort);
            socketEventArg.UserToken = sock;
            sock.ConnectAsync(socketEventArg);
            clientDone.WaitOne();

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(txtContent.Text.Trim());
            socketEventArg.SetBuffer(buffer, 0, buffer.Length);
            Socket sock = socketEventArg.UserToken as Socket;
            bool willRaiseEvent = sock.SendAsync(socketEventArg);
            if (!willRaiseEvent)
            {
                ProcessSend(socketEventArg);
            }
        }

        /// <summary>
        /// A single callback is used for all socket operations. This method forwards execution on to the correct handler 
        /// based on the type of completed operation
        /// </summary>
        static void SocketEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            //LogHelper.WriteLog("Log", "SocketEventArg_Completed Begin" + "\r\n");
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    {
                        //LogHelper.WriteLog("Log", "SocketEventArg_Completed Connect" + "\r\n");
                        ProcessConnect(e);
                    }
                    break;
                case SocketAsyncOperation.Receive:
                    {
                        //LogHelper.WriteLog("Log", "SocketEventArg_Completed Receive" + "\r\n");
                        ProcessReceive(e);
                    }
                    break;
                case SocketAsyncOperation.Send:
                    {
                        //LogHelper.WriteLog("Log", "SocketEventArg_Completed Send" + "\r\n");
                        ProcessSend(e);
                    }
                    break;
                default:
                    throw new Exception("Invalid operation completed");
            }
            //LogHelper.WriteLog("Log", "SocketEventArg_Completed End" + "\r\n");
        }

        /// <summary>
        /// Called when a ConnectAsync operation completes
        /// </summary>
        private static void ProcessConnect(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                //Console.WriteLine("Successfully connected to the server");

                // Send 'Hello World' to the server
                //byte[] buffer = Encoding.UTF8.GetBytes("Hello World");
                //e.SetBuffer(buffer, 0, buffer.Length);
                //Socket sock = e.UserToken as Socket;
                //bool willRaiseEvent = sock.SendAsync(e);
                //if (!willRaiseEvent)
                //{
                //    ProcessSend(e);
                //}

                clientDone.Set();
            }
            else
            {
                throw new SocketException((int)e.SocketError);
            }
        }

        /// <summary>
        /// Called when a ReceiveAsync operation completes
        /// </summary>
        private static void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                byte[] data = new byte[e.BytesTransferred];
                Array.Copy(e.Buffer, e.Offset, data, 0, data.Length);
                string strData = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);


                //LogHelper.WriteLog("Log", "ProcessReceive:" + strData + "\r\n");
                //Console.WriteLine("Received from server: {0}", Encoding.UTF8.GetString(e.Buffer, 0, e.BytesTransferred));

                // Data has now been sent and received from the server. Disconnect from the server
                Socket sock = e.UserToken as Socket;
                //bool willRaiseEvent = sock.SendAsync(e);
                //if (!willRaiseEvent)
                //{
                //    //LogHelper.WriteLog("Log", "ProcessSend(e)" + "\r\n");
                //    ProcessSend(e);
                //}
                //sock.Shutdown(SocketShutdown.Send);
                //sock.Close();
                clientDone.Set();
            }
            else
            {
                throw new SocketException((int)e.SocketError);
            }
        }


        /// <summary>
        /// Called when a SendAsync operation completes
        /// </summary>
        private static void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                //Console.WriteLine("Sent 'Hello World' to the server");

                //Read data sent from the server
                //Socket sock = e.UserToken as Socket;
                //bool willRaiseEvent = sock.SendAsync(e);
                //if (!willRaiseEvent)
                //{
                //    ProcessReceive(e);
                //}
            }
            else
            {
                throw new SocketException((int)e.SocketError);
            }
        }

        /// <summary>
        /// Called when [send info].
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnSendInfo(string message)
        {
            if (lbMainMessage.Items.Count >= 30)
            {
                lbMainMessage.Items.RemoveAt(0);
            }
            lbMainMessage.Items.Add(message);
        }

        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="message">The message.</param>
        public void DoSendInfo(string message)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(SendInfo, message);
            else
                SendInfo(message);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            this.SendInfo = new SendMessageToFormEventhandler(this.OnSendInfo);
        }
    }
}
