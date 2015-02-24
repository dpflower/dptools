using System;
using System.Text;
using System.Net.Mail;
using System.Net;



namespace DP.Net
{
    /// <summary>
    /// 发送邮件类
    /// </summary>
    public class StmpEmail
    {
        #region 变量
        private string host = null;             //发件人的邮箱的服务器位置（可以是IP或者smtp的域名）
        private string sender = null;           //发件人
        private string senderpw = null;         //发件人邮箱密码（用于验证） 
        #endregion

        #region 属性
        public string Host
        {
            get { return host; }
            set { host = value; }
        }

        public string Sener
        {
            get { return sender; }
            set { sender = value; }
        }

        public string Senderpw
        {
            get { return senderpw; }
            set { senderpw = value; }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public StmpEmail()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Host">发送邮件的服务器位置</param>
        public StmpEmail(string Host)
        {
            this.host = Host;
        }

        public StmpEmail(string Host, string Sender, string Senderpw)
        {
            this.host = Host;
            this.sender = Sender;
            this.senderpw = Senderpw;
        } 
        #endregion

        /// <summary>
        /// 发送邮件方法
        /// </summary>
        /// <param name="Sender">发送人邮箱</param>
        /// <param name="Senderpw">发送人邮箱的密码</param>
        /// <param name="Receiver">收信人</param>
        /// <param name="Subject">邮件的主题</param>
        /// <param name="body">邮件的内容</param>
        public bool SendMail(string Sender, string Senderpw, string Receiver, string Subject, string Body, string Filename)
        {
            if (this.host == string.Empty)
            {
                return false;
            }

            Send(Sender, Senderpw, Receiver, Subject, Body, Filename);

            return true;
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="Host">邮箱服务器位置</param>
        /// <param name="Sender">发件人</param>
        /// <param name="Senderpw">发件人邮箱密码</param>
        /// <param name="Receiver">收件人</param>
        /// <param name="Subject">邮件主题</param>
        /// <param name="Body">邮件内容</param>
        /// <param name="Filename">附件名字</param>
        /// <returns>是否成功</returns>
        public bool SendMail(string Host, string Sender, string Senderpw, string Receiver, string Subject, string Body, string Filename)
        {
            if (Host == string.Empty)
            {
                return false;
            }

            this.host = Host;

            Send(Sender, Senderpw, Receiver, Subject, Body, Filename);

            return true;
        }
        /// <summary>
        /// 发送邮件（host默认，sender默认，senderpw默认）
        /// </summary>
        /// <param name="Receiver">收件人地址</param>
        /// <param name="Subject">邮件主题</param>
        /// <param name="Body">邮件内容</param>
        /// <param name="Filename">附件</param>
        /// <returns>是否成功</returns>
        public bool SendMail(string Receiver, string Subject, string Body, string Filename)
        {
            if (this.host == string.Empty)
            {
                return false;
            }

            Send(this.sender, this.senderpw, Receiver, Subject, Body, Filename);

            return true;
        }

        private void Send(string Sender, string Senderpw, string Receiver, string Subject, string Body, string Filename)
        {
            SmtpClient mail = new SmtpClient(this.host);

            mail.Credentials = new System.Net.NetworkCredential(Sender, Senderpw);

            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(Sender, Receiver);

            message.BodyEncoding = Encoding.Default;
            message.Body = Body;
            message.Subject = Subject;

            //message.Sender = new MailAddress(Sender);

            //foreach (MailAddress address in message.CC)
            //{
            //    message.Body += address.Address;
            //}

            if (!string.IsNullOrEmpty(Filename))//(Filename != null && Filename != string.Empty)
            {
                //Attachment attachment = new Attachment(Filename);
                message.Attachments.Add(new Attachment(Filename));
            }
            mail.Send(message);

            //message.Attachments.Dispose();
        }




    }
}
