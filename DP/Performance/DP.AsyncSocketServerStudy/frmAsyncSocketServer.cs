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
using System.Collections;
using DP.Net.AsyncSocket;

namespace DP.AsyncSocketServerStudy
{
    public partial class frmAsyncSocketServer : Form
    {
        private delegate void AsyncWorkEventHandler(object obj, AsyncOperation asyncOperation, SendOrPostCallback startMethod, SendOrPostCallback progressMethod, SendOrPostCallback completionMethod);
        private delegate void SendMessageToFormEventhandler(string message);

        private AsyncWorkEventHandler asyncWrok;

        private SendOrPostCallback startedMethodHandler;
        private SendOrPostCallback progressMethodHandler;
        private SendOrPostCallback completionMethodHandler;

        private SendMessageToFormEventhandler SendInfo;
        private SendMessageToFormEventhandler ClientAdd;
        private SendMessageToFormEventhandler ClientRemove;

        TcpSocketServer server;

        int messageCount = 0;

        public frmAsyncSocketServer()
        {
            InitializeComponent();
            //server.onMessage = new MessageEventHandler(ShowMessage);
        }


        private void frmAsyncSocketServer_Load(object sender, EventArgs e)
        {
            this.startedMethodHandler = new System.Threading.SendOrPostCallback(this.OnStarted);
            this.progressMethodHandler = new System.Threading.SendOrPostCallback(this.OnPorgress);
            this.completionMethodHandler = new System.Threading.SendOrPostCallback(this.OnCompletion);

            this.SendInfo = new SendMessageToFormEventhandler(this.OnSendInfo);
            this.ClientAdd = new SendMessageToFormEventhandler(this.OnClientAdd);
            this.ClientRemove = new SendMessageToFormEventhandler(this.OnClientRemove);

            BeginAsyncWork();
        }



        private void btnListen_Click(object sender, EventArgs e)
        {
            server = new TcpSocketServer(int.Parse(txtConnections.Text.Trim()), int.Parse(txtBufferSize.Text.Trim()));            
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, int.Parse(txtPort.Text.Trim()));
            server.onMessage = new MessageEventHandler(ShowMessage);
            server.onReceiveCompleted = new ReceiveCompletedEventHandler(ReceiveCompleted);
            server.onClientConnectionRequest = new ClientConnectionRequestEventHandler(ClientConnectionRequest);
            server.onClientDisconnect = new ClientDisconnectEventHandler(ClientDisconnect);
            server.Start(localEndPoint);
            btnListen.Enabled = false;
            btnStop.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            server.Stop();
            server = null;
            btnListen.Enabled = true;
            btnStop.Enabled = false;

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (server == null || server.ServerState == ServerStates.Stoped)
            {
                return;
            }
            server.SendDataToClient(txtSendData.Text.Trim());
        }










        public void BeginAsyncWork()
        {            

            //this.asyncWrok = new AsyncWorkEventHandler(this.DoAsyncWork);
            //this.asyncWrok.BeginInvoke(null, System.ComponentModel.AsyncOperationManager.CreateOperation(Guid.NewGuid()), this.startedMethodHandler, this.progressMethodHandler, this.completionMethodHandler, null, null);

        }

        private void ShowMessage(string message)
        {
            DoSendInfo(DateTime.Now.ToString("HH:mm:ss.fff") + ":" + message);
        }

        private void ClientConnectionRequest(SocketAsyncEventArgs e, string remoteEndPoint, int ConnectedSockets)
        {
            DoClientAdd(remoteEndPoint);
        }

        private void ClientDisconnect(SocketAsyncEventArgs e, string remoteEndPoint, int ConnectedSockets)
        {
            DoClientRemove(remoteEndPoint);
            //lbClientList.Items.Remove(remoteEndPoint);
        }

        private void ReceiveCompleted(SocketAsyncEventArgs e, string remoteEndPoint, string receive)
        {
            DoSendInfo(DateTime.Now.ToString("HH:mm:ss.fff") + ":" + String.Format("接受客户端 {0} 数据， 数据内容为：{1}　！ ", remoteEndPoint, receive));
            //lbClientList.Items.Remove(remoteEndPoint);
        }

        /// <summary>
        /// 异步执行方法
        /// 
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="asyncOperation">The async operation.</param>
        /// <param name="startMethod">The start method.</param>
        /// <param name="progressMethod">The progress method.</param>
        /// <param name="completionMethod">The completion method.</param>
        public void DoAsyncWork(object obj, AsyncOperation asyncOperation, SendOrPostCallback startMethod, SendOrPostCallback progressMethod, SendOrPostCallback completionMethod)
        {
            //TcpSocketServer server = new TcpSocketServer(100, 1024);
            //server.Start(localEndPoint);

            completionMethod(obj);
        }

        /// <summary>
        /// 开始方法
        /// </summary>
        /// <param name="paras"></param>
        private void OnStarted(object paras)
        {
            DoSendInfo("监听开始！");

        }

        /// <summary>
        /// 进程方法
        /// </summary>
        /// <param name="paras"></param>
        private void OnPorgress(object paras)
        {

        }

        /// <summary>
        /// 完成方法
        /// </summary>
        /// <param name="paras"></param>
        private void OnCompletion(object paras)
        {
            DoSendInfo("监听结束！");

        }

        /// <summary>
        /// Called when [send info].
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnSendInfo(string message)
        {
            if (lbMainMessage.Items.Count >= 20)
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
            DP.Common.LogHelper.WriteLogAsync("WriteLogAsync", message);
            if (messageCount++ % 100 == 0 || messageCount < 100)
            {
                if (this.InvokeRequired)
                    this.BeginInvoke(SendInfo, message + "Count" + messageCount.ToString());
                else
                    SendInfo(message + "Count" + messageCount.ToString());
            }
        }



        /// <summary>
        /// Called when [send info].
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnClientAdd(string message)
        {
            lbClientList.Items.Add(message);
        }

        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="message">The message.</param>
        public void DoClientAdd(string message)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(ClientAdd, message);
            else
                ClientAdd(message);
        }


        /// <summary>
        /// Called when [send info].
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnClientRemove(string message)
        {
            lbClientList.Items.Remove(message);
        }

        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="message">The message.</param>
        public void DoClientRemove(string message)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(ClientRemove, message);
            else
                ClientRemove(message);
        }

        private void mnuParament_Click(object sender, EventArgs e)
        {
            server.SendDataToClient("右左右");
        }

        private void mnuQuit_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void btnClientCount_Click(object sender, EventArgs e)
        {
            Hashtable ht = server.GetClientList();
            MessageBox.Show(server.GetClientCount().ToString());
        }

    }
}
