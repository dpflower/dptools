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
    public partial class AsyncSocketClientProxy : Form
    {
        private delegate void SendMessageToFormEventhandler(string message);

        private SendMessageToFormEventhandler SendInfo;

        TcpSocketProxyClient socketClient = new TcpSocketProxyClient();
        public AsyncSocketClientProxy()
        {
            InitializeComponent();
            this.SendInfo = new SendMessageToFormEventhandler(this.OnSendInfo);
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            socketClient.onMessage = new MessageEventHandler(ShowMessage);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            socketClient.Connect(txtIP.Text.Trim(), int.Parse(txtPort.Text.Trim()));
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            socketClient.SendDataToServer(txtContent.Text.Trim());
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            socketClient.Disconnect();
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

        private void button1_Click(object sender, EventArgs e)
        {
            //byte[] _packetHeader = new byte[] { byte.MaxValue, byte.MinValue, byte.MaxValue, byte.MinValue, byte.MaxValue, byte.MinValue };

            //byte[] bbb = new byte[8];

            //for (int i = 0; i < 8; i++)
            //{
            //    if ((i % 2) == 0)
            //    {
            //        bbb[i] = byte.MaxValue;
            //    }
            //    else
            //    {
            //        bbb[i] = byte.MinValue;
            //    }
            //}

            //string sssss = UTF8Encoding.UTF8.GetString(bbb);
            //sssss = UTF7Encoding.UTF7.GetString(bbb);
            //sssss = UTF32Encoding.UTF7.GetString(bbb);
            //sssss = ASCIIEncoding.ASCII.GetString(bbb);



            //AsyncUserToken token = new AsyncUserToken();
            //int count = 5;
            //for (int i = 0; i < count; i++)
            //{
            //    string s = "String" + i.ToString();
            //    byte[] b = System.Text.UTF8Encoding.UTF8.GetBytes(s);
            //    token.ReceivedDataHandle(b);
            //}
            //string sss = UTF8Encoding.UTF8.GetString(token.ReceivedBuffer);

            //token.Remove(7,7);

            //string sss2 = UTF8Encoding.UTF8.GetString(token.ReceivedBuffer);

        }

    }
}
