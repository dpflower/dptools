using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DP.Net.AsyncSocket;

namespace DP.AsyncSocketClientStudy
{
    public partial class AsyncSocketClient2 : Form
    {
        
        private delegate void SendMessageToFormEventhandler(string message);

        private SendMessageToFormEventhandler SendInfo;

        List<TcpSocketClient> socketClientList =new List<TcpSocketClient>();
        TcpSocketClient socketClient;
        public AsyncSocketClient2()
        {
            InitializeComponent();
            this.SendInfo = new SendMessageToFormEventhandler(this.OnSendInfo);
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            //socketClient.onMessage = new MessageEventHandler(ShowMessage);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            //socketClientList = 
            for(int i = 0; i < int.Parse(txtNumber.Text.Trim()); i ++)
            {
                socketClient = new TcpSocketClient();
                socketClient.onMessage = new MessageEventHandler(ShowMessage);
                socketClient.Connect(txtIP.Text.Trim(), int.Parse(txtPort.Text.Trim()));
                socketClientList.Add(socketClient);
            }

            //socketClient.Connect(txtIP.Text.Trim(), int.Parse(txtPort.Text.Trim()));
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            foreach (TcpSocketClient socketClient in socketClientList)
            {
                socketClient.SendDataToServer(txtContent.Text.Trim());
            }
            //socketClient.SendDataToServer(txtContent.Text.Trim());
        }


        private void btnSendEnd_Click(object sender, EventArgs e)
        {

        }

        private void btnSendBegin_Click(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            foreach (TcpSocketClient socketClient in socketClientList)
            {
                socketClient.Disconnect();
            }
            //socketClient.Disconnect();
        }


        private void ShowMessage(string message)
        {
            DoSendInfo(DateTime.Now.ToString("HH:mm:ss.fff") + ":" + message);
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
            if (this.InvokeRequired)
                this.BeginInvoke(SendInfo, message);
            else
                SendInfo(message);
        }



    }
}
