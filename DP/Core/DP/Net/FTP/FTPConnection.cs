using System;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Data;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

namespace DP.Net.FTP
{
    /// <summary>
    ///简要说明的FTP连接
    /// </summary>
    public class FTPConnection
    {
        public delegate void ShowMessageEventHandler(object sender, ArrayList messageList);
        public ShowMessageEventHandler ShowMessage;

        private TcpClient tcpClient;
        private static int BLOCK_SIZE = 512;
        private static int DEFAULT_REMOTE_PORT = 21;
        private static int DATA_PORT_RANGE_FROM = 1500;
        private static int DATA_PORT_RANGE_TO = 65000;
        private FTPMode mode;

        public FTPMode Mode
        {
            get { return mode; }
        }
        private int activeConnectionsCount;
        private string remoteHost;

        private ArrayList messageList = new ArrayList();
        private bool logMessages;


        #region 构造函数
        public FTPConnection()
        {
            this.activeConnectionsCount = 0;
            this.mode = FTPMode.Active;
            this.logMessages = false;
        } 
        #endregion

        protected virtual void OnShowMessage(ArrayList messageList)
        {
            if (ShowMessage != null)
            {
                ShowMessage(this, messageList);
            }
        } 

        public ArrayList MessageList
        {
            get
            {
                return this.messageList;
            }
        }

        public bool LogMessages
        {
            get
            {
                return this.logMessages;
            }

            set
            {
                if (!value)
                {
                    this.messageList = new ArrayList();
                }

                this.logMessages = value;
            }
        }

        /// <summary>
        /// 打开链接
        /// </summary>
        /// <param name="remoteHost"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public virtual void Open(string remoteHost, string user, string password)
        {
            Open(remoteHost, DEFAULT_REMOTE_PORT, user, password, FTPMode.Active);
        }
        /// <summary>
        /// 打开链接
        /// </summary>
        /// <param name="remoteHost"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="mode"></param>
        public virtual void Open(string remoteHost, string user, string password, FTPMode mode)
        {
            Open(remoteHost, DEFAULT_REMOTE_PORT, user, password, mode);
        }
        /// <summary>
        /// 打开链接
        /// </summary>
        /// <param name="remoteHost"></param>
        /// <param name="remotePort"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public virtual void Open(string remoteHost, int remotePort, string user, string password)
        {
            Open(remoteHost, remotePort, user, password, FTPMode.Active);
        }
        /// <summary>
        /// 打开链接
        /// </summary>
        /// <param name="remoteHost"></param>
        /// <param name="remotePort"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="pMode"></param>
        public virtual void Open(string remoteHost, int remotePort, string user, string password, FTPMode pMode)
        {
            ArrayList tempMessageList = new ArrayList();
            int returnValue;

            this.mode = pMode;
            this.tcpClient = new TcpClient();
            this.remoteHost = remoteHost;

            //由于我们无法检测到本地地址从TCPClient类，转换“ 127.0.0.1 ”和“ localhost ”的，以
            //的DNS记录的这台机器，这将确保连接地址和端口命令处理
            //是相同的。这一修正错误854919 。
            if (remoteHost == "localhost" || remoteHost == "127.0.0.1")
            {
                remoteHost = GetLocalAddressList()[0].ToString();
            }

            //连接
            try
            {
                this.tcpClient.Connect(remoteHost, remotePort);
            }
            catch (Exception)
            {
                throw new IOException("无法连接到远程服务器!");
            }
            tempMessageList = Read();
            OnShowMessage(tempMessageList);
            returnValue = GetMessageReturnValue((string)tempMessageList[0]);
            if (returnValue != 220)
            {
                Close();
                throw new Exception((string)tempMessageList[0]);
            }

            //SEND USER
            tempMessageList = SendCommand("USER " + (String.IsNullOrEmpty(user.Trim()) ? "Anonymous" : user.Trim()));
            OnShowMessage(tempMessageList);
            returnValue = GetMessageReturnValue((string)tempMessageList[0]);
            if (!(returnValue == 331 || returnValue == 202))
            {
                Close();
                throw new Exception((string)tempMessageList[0]);
            }

            //SEND PASSWORD
            if (returnValue == 331)
            {
                tempMessageList = SendCommand("PASS " + password);
                OnShowMessage(tempMessageList);
                returnValue = GetMessageReturnValue((string)tempMessageList[0]);
                if (!(returnValue == 230 || returnValue == 202))
                {
                    Close();
                    throw new Exception((string)tempMessageList[0]);
                }
            }
        }
        /// <summary>
        /// 关闭链接
        /// </summary>
        public virtual void Close()
        {
            ArrayList messageList = new ArrayList();

            if (this.tcpClient != null)
            {
                messageList = SendCommand("QUIT");
                this.tcpClient.Close();
            }
        }
        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        public ArrayList Dir(String mask)
        {
            ArrayList tmpList = Dir();

            DataTable table = new DataTable();
            table.Columns.Add("Name");
            for (int i = 0; i < tmpList.Count; i++)
            {
                DataRow row = table.NewRow();
                row["Name"] = (string)tmpList[i];
                table.Rows.Add(row);
            }

            DataRow[] rowList = table.Select("Name LIKE '" + mask + "'", "", DataViewRowState.CurrentRows);
            tmpList = new ArrayList();
            for (int i = 0; i < rowList.Length; i++)
            {
                tmpList.Add((string)rowList[i]["Name"]);
            }

            return tmpList;
        }
        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <returns></returns>
        public ArrayList Dir()
        {
            LockTcpClient();
            TcpListener listner = null;
            TcpClient client = null;
            NetworkStream networkStream = null;
            ArrayList tempMessageList = new ArrayList();
            int returnValue = 0;
            string returnValueMessage = "";
            ArrayList fileList = new ArrayList();

            SetTransferType(FTPFileTransferType.ASCII);

            if (this.mode == FTPMode.Active)
            {
                listner = CreateDataListner();
                listner.Start();
            }
            else
            {
                client = CreateDataClient();
            }

            tempMessageList = new ArrayList();
            tempMessageList = SendCommand("NLST");
            OnShowMessage(tempMessageList);
            returnValue = GetMessageReturnValue((string)tempMessageList[0]);
            if (!(returnValue == 150 || returnValue == 125 || returnValue == 550))
            {
                throw new Exception((string)tempMessageList[0]);
            }

            if (returnValue == 550) //No files found
            {
                return fileList;
            }

            if (this.mode == FTPMode.Active)
            {
                client = listner.AcceptTcpClient();
            }
            networkStream = client.GetStream();

            fileList = ReadLines(networkStream);

            if (tempMessageList.Count == 1)
            {
                tempMessageList = Read();
                OnShowMessage(tempMessageList);
                returnValue = GetMessageReturnValue((string)tempMessageList[0]);
                returnValueMessage = (string)tempMessageList[0];
            }
            else
            {
                returnValue = GetMessageReturnValue((string)tempMessageList[1]);
                returnValueMessage = (string)tempMessageList[1];
            }

            if (!(returnValue == 226))
            {
                throw new Exception(returnValueMessage);
            }

            networkStream.Close();
            client.Close();

            if (this.mode == FTPMode.Active)
            {
                listner.Stop();
            }
            UnlockTcpClient();
            return fileList;
        }
        /// <summary>
        /// 获取目录和文件列表
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="onColumnIndex"></param>
        /// <returns></returns>
        public List<FilesAndFolderInfo> XDir(String mask)
        {
            List<FilesAndFolderInfo> oldList = XDir();
            List<FilesAndFolderInfo> newList = oldList.FindAll(delegate(FilesAndFolderInfo f) { if (f.FullName.IndexOf(mask) >= 0) return true; else return false; });
            return newList;

            //ArrayList tmpList = null;// XDir();

            //DataTable table = new DataTable();
            //table.Columns.Add("Name");
            //for (int i = 0; i < tmpList.Count; i++)
            //{
            //    DataRow row = table.NewRow();
            //    row["Name"] = (string)((ArrayList)tmpList[i])[onColumnIndex];
            //    table.Rows.Add(row);
            //}

            //DataRow[] rowList = table.Select("Name LIKE '" + mask + "'", "", DataViewRowState.CurrentRows);
            //ArrayList newList = new ArrayList();
            //for (int i = 0; i < rowList.Length; i++)
            //{
            //    for (int j = 0; j < tmpList.Count; j++)
            //    {
            //        if ((string)rowList[i]["Name"] == (string)((ArrayList)tmpList[j])[onColumnIndex])
            //        {
            //            newList.Add((ArrayList)tmpList[j]);
            //        }
            //    }
            //}

            //return newList;
        }
        /// <summary>
        /// 获取目录和文件列表
        /// </summary>
        /// <returns></returns>
        public List<FilesAndFolderInfo> XDir()
        {
            LockTcpClient();
            TcpListener listner = null;
            TcpClient client = null;
            NetworkStream networkStream = null;
            ArrayList tempMessageList = new ArrayList();
            int returnValue = 0;
            string returnValueMessage = "";
            List<FilesAndFolderInfo> list = null;
            //ArrayList filesAndFolderList = new ArrayList();
            //ArrayList tmpFilesAndFolderList = new ArrayList();

            SetTransferType(FTPFileTransferType.ASCII);

            if (this.mode == FTPMode.Active)
            {
                listner = CreateDataListner();
                listner.Start();
            }
            else
            {
                client = CreateDataClient();
            }

            tempMessageList = new ArrayList();
            tempMessageList = SendCommand("LIST");
            OnShowMessage(tempMessageList);
            returnValue = GetMessageReturnValue((string)tempMessageList[0]);
            if (!(returnValue == 150 || returnValue == 125))
            {
                throw new Exception((string)tempMessageList[0]);
            }

            if (this.mode == FTPMode.Active)
            {
                client = listner.AcceptTcpClient();
            }
            networkStream = client.GetStream();

            list = ReadFilesAndFolderInfo(networkStream);
            
            if (tempMessageList.Count == 1)
            {
                tempMessageList = Read();
                OnShowMessage(tempMessageList);
                returnValue = GetMessageReturnValue((string)tempMessageList[0]);
                returnValueMessage = (string)tempMessageList[0];
            }
            else
            {
                returnValue = GetMessageReturnValue((string)tempMessageList[1]);
                returnValueMessage = (string)tempMessageList[1];
            }

            if (!(returnValue == 226))
            {
                throw new Exception(returnValueMessage);
            }

            networkStream.Close();
            client.Close();

            if (this.mode == FTPMode.Active)
            {
                listner.Stop();
            }

            UnlockTcpClient();
           
            return list;
        }
        /// <summary>
        /// 发送数据流
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="remoteFileName"></param>
        /// <param name="type"></param>
        public void SendStream(Stream stream, string remoteFileName, FTPFileTransferType type)
        {
            LockTcpClient();
            TcpListener listner = null;
            TcpClient client = null;
            NetworkStream networkStream = null;
            ArrayList tempMessageList = new ArrayList();
            int returnValue = 0;
            string returnValueMessage = "";
            tempMessageList = new ArrayList();

            SetTransferType(type);

            if (this.mode == FTPMode.Active)
            {
                listner = CreateDataListner();
                listner.Start();
            }
            else
            {
                client = CreateDataClient();
            }

            tempMessageList = SendCommand("STOR " + remoteFileName);
            OnShowMessage(tempMessageList);
            returnValue = GetMessageReturnValue((string)tempMessageList[0]);
            if (!(returnValue == 150 || returnValue == 125))
            {
                throw new Exception((string)tempMessageList[0]);
            }

            if (this.mode == FTPMode.Active)
            {
                client = listner.AcceptTcpClient();
            }

            networkStream = client.GetStream();

            Byte[] buffer = new Byte[BLOCK_SIZE];
            int bytes = 0;
            int totalBytes = 0;

            while (totalBytes < stream.Length)
            {
                bytes = (int)stream.Read(buffer, 0, BLOCK_SIZE);
                totalBytes = totalBytes + bytes;
                networkStream.Write(buffer, 0, bytes);
            }

            networkStream.Close();
            client.Close();

            if (this.mode == FTPMode.Active)
            {
                listner.Stop();
            }

            if (tempMessageList.Count == 1)
            {
                tempMessageList = Read();
                OnShowMessage(tempMessageList);
                returnValue = GetMessageReturnValue((string)tempMessageList[0]);
                returnValueMessage = (string)tempMessageList[0];
            }
            else
            {
                returnValue = GetMessageReturnValue((string)tempMessageList[1]);
                returnValueMessage = (string)tempMessageList[1];
            }

            if (!(returnValue == 226))
            {
                throw new Exception(returnValueMessage);
            }
            UnlockTcpClient();
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="localFileName"></param>
        /// <param name="type"></param>
        public virtual void SendFile(string localFileName, FTPFileTransferType type)
        {
            SendFile(localFileName, Path.GetFileName(localFileName), type);
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="localFileName"></param>
        /// <param name="remoteFileName"></param>
        /// <param name="type"></param>
        public virtual void SendFile(string localFileName, string remoteFileName, FTPFileTransferType type)
        {
            FileStream fs = new FileStream(localFileName, FileMode.Open);
            SendStream(fs, remoteFileName, type);
            fs.Close();
        }
        /// <summary>
        /// 获取数据流
        /// </summary>
        /// <param name="remoteFileName"></param>
        /// <param name="stream"></param>
        /// <param name="type"></param>
        public void GetStream(string remoteFileName, Stream stream, FTPFileTransferType type)
        {
            TcpListener listner = null;
            TcpClient client = null;
            NetworkStream networkStream = null;
            ArrayList tempMessageList = new ArrayList();
            int returnValue = 0;
            string returnValueMessage = "";

            LockTcpClient();

            SetTransferType(type);

            if (this.mode == FTPMode.Active)
            {
                listner = CreateDataListner();
                listner.Start();
            }
            else
            {
                client = CreateDataClient();
            }

            tempMessageList = new ArrayList();
            tempMessageList = SendCommand("RETR " + remoteFileName);
            OnShowMessage(tempMessageList);
            returnValue = GetMessageReturnValue((string)tempMessageList[0]);
            if (!(returnValue == 150 || returnValue == 125))
            {
                throw new Exception((string)tempMessageList[0]);
            }

            if (this.mode == FTPMode.Active)
            {
                client = listner.AcceptTcpClient();
            }

            networkStream = client.GetStream();

            Byte[] buffer = new Byte[BLOCK_SIZE];
            int bytes = 0;

            bool read = true;
            while (read)
            {
                bytes = (int)networkStream.Read(buffer, 0, buffer.Length);
                stream.Write(buffer, 0, bytes);
                if (bytes == 0)
                {
                    read = false;
                }
            }

            networkStream.Close();
            client.Close();

            if (this.mode == FTPMode.Active)
            {
                listner.Stop();
            }

            if (tempMessageList.Count == 1)
            {
                tempMessageList = Read();
                OnShowMessage(tempMessageList);
                returnValue = GetMessageReturnValue((string)tempMessageList[0]);
                returnValueMessage = (string)tempMessageList[0];
            }
            else
            {
                returnValue = GetMessageReturnValue((string)tempMessageList[1]);
                returnValueMessage = (string)tempMessageList[1];
            }

            if (!(returnValue == 226))
            {
                throw new Exception(returnValueMessage);
            }

            UnlockTcpClient();
        }
        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="remoteFileName"></param>
        /// <param name="type"></param>
        public virtual void GetFile(string remoteFileName, FTPFileTransferType type)
        {
            GetFile(remoteFileName, Path.GetFileName(remoteFileName), type);
        }
        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="remoteFileName"></param>
        /// <param name="localFileName"></param>
        /// <param name="type"></param>
        public virtual void GetFile(string remoteFileName, string localFileName, FTPFileTransferType type)
        {
            FileStream fs = new FileStream(localFileName, FileMode.Create);
            GetStream(remoteFileName, fs, type);
            fs.Close();
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="remoteFileName"></param>
        public virtual void DeleteFile(String remoteFileName)
        {
            System.Threading.Monitor.Enter(this.tcpClient);
            ArrayList tempMessageList = new ArrayList();
            int returnValue = 0;
            tempMessageList = SendCommand("DELE " + remoteFileName);
            OnShowMessage(tempMessageList);
            returnValue = GetMessageReturnValue((string)tempMessageList[0]);
            if (returnValue != 250)
            {
                throw new Exception((string)tempMessageList[0]);
            }
            System.Threading.Monitor.Exit(this.tcpClient);
        }
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="remoteFileName"></param>
        /// <param name="toRemotePath"></param>
        public virtual void MoveFile(string remoteFileName, string toRemotePath)
        {
            if (toRemotePath.Length > 0 && toRemotePath.Substring(toRemotePath.Length - 1, 1) != "/")
            {
                toRemotePath = toRemotePath + "/";
            }

            RenameFile(remoteFileName, toRemotePath + remoteFileName);
        }
        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="fromRemoteFileName"></param>
        /// <param name="toRemoteFileName"></param>
        public virtual void RenameFile(string fromRemoteFileName, string toRemoteFileName)
        {
            System.Threading.Monitor.Enter(this.tcpClient);
            ArrayList tempMessageList = new ArrayList();
            int returnValue = 0;
            tempMessageList = SendCommand("RNFR " + fromRemoteFileName);
            OnShowMessage(tempMessageList);
            returnValue = GetMessageReturnValue((string)tempMessageList[0]);
            if (returnValue != 350)
            {
                throw new Exception((string)tempMessageList[0]);
            }
            tempMessageList = SendCommand("RNTO " + toRemoteFileName);
            OnShowMessage(tempMessageList);
            returnValue = GetMessageReturnValue((string)tempMessageList[0]);
            if (returnValue != 250)
            {
                throw new Exception((string)tempMessageList[0]);
            }
            System.Threading.Monitor.Exit(this.tcpClient);
        }
        /// <summary>
        /// 设置当前目录
        /// </summary>
        /// <param name="remotePath"></param>
        public virtual void SetCurrentDirectory(String remotePath)
        {
            LockTcpClient();

            ArrayList tempMessageList = new ArrayList();
            int returnValue = 0;
            tempMessageList = SendCommand("CWD " + remotePath);
            OnShowMessage(tempMessageList);
            returnValue = GetMessageReturnValue((string)tempMessageList[0]);
            if (returnValue != 250)
            {
                throw new Exception((string)tempMessageList[0]);
            }
            UnlockTcpClient();
        }
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="directoryName"></param>
        public virtual void MakeDir(string directoryName)
        {
            LockTcpClient();

            ArrayList tempMessageList = new ArrayList();
            int returnValue = 0;

            tempMessageList = SendCommand("MKD " + directoryName);
            OnShowMessage(tempMessageList);
            returnValue = GetMessageReturnValue((string)tempMessageList[0]);
            if (returnValue != 257)
            {
                throw new Exception((string)tempMessageList[0]);
            }

            UnlockTcpClient();
        }
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="directoryName"></param>
        public virtual void RemoveDir(string directoryName)
        {
            LockTcpClient();

            ArrayList tempMessageList = new ArrayList();
            int returnValue = 0;

            tempMessageList = SendCommand("RMD " + directoryName);
            OnShowMessage(tempMessageList);
            returnValue = GetMessageReturnValue((string)tempMessageList[0]);
            if (returnValue != 250)
            {
                throw new Exception((string)tempMessageList[0]);
            }

            UnlockTcpClient();
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public ArrayList SendCommand(String command)
        {
            NetworkStream stream = this.tcpClient.GetStream();
            
            this.activeConnectionsCount++;

            Byte[] cmdBytes = Encoding.Default.GetBytes((command + "\r\n").ToCharArray());
            stream.Write(cmdBytes, 0, cmdBytes.Length);

            this.activeConnectionsCount--;

            return Read();
        }

        #region 私有函数
        private void SetTransferType(FTPFileTransferType type)
        {
            switch (type)
            {
                case FTPFileTransferType.ASCII:
                    SetMode("TYPE A");
                    break;
                case FTPFileTransferType.Binary:
                    SetMode("TYPE I");
                    break;
                default:
                    throw new Exception("Invalid File Transfer Type");
            }
        }

        private void SetMode(string mode)
        {
            LockTcpClient();

            ArrayList tempMessageList = new ArrayList();
            int returnValue = 0;
            tempMessageList = SendCommand(mode);
            OnShowMessage(tempMessageList);
            returnValue = GetMessageReturnValue((string)tempMessageList[0]);
            if (returnValue != 200)
            {
                throw new Exception((string)tempMessageList[0]);
            }
            UnlockTcpClient();
        }

        private TcpListener CreateDataListner()
        {
            int port = GetPortNumber();
            SetDataPort(port);

            IPHostEntry localHost = Dns.Resolve(Dns.GetHostName());
            TcpListener listner = new TcpListener(localHost.AddressList[0], port);

            //TcpListener listner = new TcpListener(port);
            return listner;
        }

        private TcpClient CreateDataClient()
        {
            int port = GetPortNumber();

            //IPEndPoint ep = new IPEndPoint(GetLocalAddressList()[0], port);

            TcpClient client = new TcpClient();

            //client.Connect(ep);
            client.Connect(this.remoteHost, port);

            return client;
        }

        private void SetDataPort(int portNumber)
        {
            LockTcpClient();

            ArrayList tempMessageList = new ArrayList();
            int returnValue = 0;
            int portHigh = portNumber >> 8;
            int portLow = portNumber & 255;

            tempMessageList = SendCommand("PORT "
                + GetLocalAddressList()[0].ToString().Replace(".", ",")
                + "," + portHigh.ToString() + "," + portLow);

            OnShowMessage(tempMessageList);
            returnValue = GetMessageReturnValue((string)tempMessageList[0]);
            if (returnValue != 200)
            {
                throw new Exception((string)tempMessageList[0]);
            }
            UnlockTcpClient();

        }

        private ArrayList Read()
        {
            NetworkStream stream = this.tcpClient.GetStream();
            ArrayList messageList = new ArrayList();
            ArrayList tempMessage = ReadLines(stream);

            int tryCount = 0;
            while (tempMessage.Count == 0)
            {
                if (tryCount == 10)
                {
                    throw new Exception("Server does not return message to the message");
                }

                Thread.Sleep(1000);
                tryCount++;
                tempMessage = ReadLines(stream);
            }

            while (((string)tempMessage[tempMessage.Count - 1]).Substring(3, 1) == "-")
            {
                messageList.AddRange(tempMessage);
                tempMessage = ReadLines(stream);
            }
            messageList.AddRange(tempMessage);

            AddMessagesToMessageList(messageList);

            return messageList;
        }

        private ArrayList ReadLines(NetworkStream stream)
        {
            ArrayList messageList = new ArrayList();
            char[] seperator = { '\n' };
            char[] toRemove = { '\r' };
            Byte[] buffer = new Byte[BLOCK_SIZE];
            int bytes = 0;
            string tmpMes = "";

            while (stream.DataAvailable)
            {
                bytes = stream.Read(buffer, 0, buffer.Length);
                tmpMes += Encoding.Default.GetString(buffer, 0, bytes);
            }

            string[] mess = tmpMes.Split(seperator);
            for (int i = 0; i < mess.Length; i++)
            {
                if (mess[i].Length > 0)
                {
                    messageList.Add(mess[i].Trim(toRemove));
                }
            }

            return messageList;
        }

        private List<FilesAndFolderInfo> ReadFilesAndFolderInfo(NetworkStream stream)
        {
            List<FilesAndFolderInfo> list = new List<FilesAndFolderInfo>();

            char[] seperator = { '\n' };
            char[] toRemove = { '\r' };
            Byte[] buffer = new Byte[BLOCK_SIZE];
            int bytes = 0;
            string tmpMes = "";

            while (stream.DataAvailable)
            {
                bytes = stream.Read(buffer, 0, buffer.Length);
                tmpMes += Encoding.Default.GetString(buffer, 0, bytes);
            }

            string[] mess = tmpMes.Split(seperator);
            for (int i = 0; i < mess.Length; i++)
            {
                if (mess[i].Length > 0)
                {
                    if (FilesAndFolderInfo.GetMatchingRegex(mess[i].Trim(toRemove)) != null)
                    {
                        list.Add(new FilesAndFolderInfo(mess[i].Trim(toRemove), "/"));
                    }
                }
            }
            return list;

        }

        private int GetMessageReturnValue(string message)
        {
            return int.Parse(message.Substring(0, 3));
        }

        private int GetPortNumber()
        {
            LockTcpClient();
            int port = 0;
            switch (this.mode)
            {
                case FTPMode.Active:
                    Random rnd = new Random((int)DateTime.Now.Ticks);
                    port = DATA_PORT_RANGE_FROM + rnd.Next(DATA_PORT_RANGE_TO - DATA_PORT_RANGE_FROM);
                    break;
                case FTPMode.Passive:
                    ArrayList tempMessageList = new ArrayList();
                    int returnValue = 0;
                    tempMessageList = SendCommand("PASV");
                    OnShowMessage(tempMessageList);
                    returnValue = GetMessageReturnValue((string)tempMessageList[0]);
                    if (returnValue != 227)
                    {
                        if (((string)tempMessageList[0]).Length > 4)
                        {
                            throw new Exception((string)tempMessageList[0]);
                        }
                        else
                        {
                            throw new Exception((string)tempMessageList[0] + " Passive Mode not implemented");
                        }
                    }
                    string message = (string)tempMessageList[0];
                    int index1 = message.IndexOf(",", 0);
                    int index2 = message.IndexOf(",", index1 + 1);
                    int index3 = message.IndexOf(",", index2 + 1);
                    int index4 = message.IndexOf(",", index3 + 1);
                    int index5 = message.IndexOf(",", index4 + 1);
                    int index6 = message.IndexOf(")", index5 + 1);
                    port = 256 * int.Parse(message.Substring(index4 + 1, index5 - index4 - 1)) + int.Parse(message.Substring(index5 + 1, index6 - index5 - 1));
                    break;
            }
            UnlockTcpClient();
            return port;
        }

        private void AddMessagesToMessageList(ArrayList messages)
        {
            if (this.logMessages)
            {
                this.messageList.AddRange(messages);
            }
        }

        private IPAddress[] GetLocalAddressList()
        {
            return Dns.Resolve(Dns.GetHostName()).AddressList;
        }

        private void LockTcpClient()
        {
            System.Threading.Monitor.Enter(this.tcpClient);
        }

        private void UnlockTcpClient()
        {
            System.Threading.Monitor.Exit(this.tcpClient);
        }

        private ArrayList GetTokens(String text, String delimiter)
        {
            int next;

            ArrayList tokens = new ArrayList();

            next = text.IndexOf(delimiter);
            while (next != -1)
            {
                string item = text.Substring(0, next);
                if (item.Length > 0)
                {
                    tokens.Add(item);
                }

                text = text.Substring(next + 1);
                next = text.IndexOf(delimiter);
            }

            if (text.Length > 0)
            {
                tokens.Add(text);
            }

            return tokens;
        } 
        #endregion

        #region 类
        public class FilesAndFolderInfo
        {
            private static string[] _FileFormat = new string[] { @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\w+\s+\w+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{4})\s+(?<name>.+)", @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\d+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{4})\s+(?<name>.+)", @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\d+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{1,2}:\d{2})\s+(?<name>.+)", @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\w+\s+\w+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{1,2}:\d{2})\s+(?<name>.+)", @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})(\s+)(?<size>(\d+))(\s+)(?<ctbit>(\w+\s\w+))(\s+)(?<size2>(\d+))\s+(?<timestamp>\w+\s+\d+\s+\d{2}:\d{2})\s+(?<name>.+)", @"(?<timestamp>\d{2}\-\d{2}\-\d{2}\s+\d{2}:\d{2}[Aa|Pp][mM])\s+(?<dir>\<\w+\>){0,1}(?<size>\d+){0,1}\s+(?<name>.+)" };
            
            public static Match GetMatchingRegex(string line)
            {
                for (int i = 0; i <= (_FileFormat.Length - 1); i++)
                {
                    Match match = new Regex(_FileFormat[i]).Match(line);
                    if (match.Success)
                    {
                        return match;
                    }
                }
                return null;
            }

            public FilesAndFolderInfo(string line, string path)
            {
                Match matchingRegex = GetMatchingRegex(line);
                if (matchingRegex == null)
                {
                    throw new ApplicationException("Unable to parse line: " + line);
                }
                this._FullName = matchingRegex.Groups["name"].Value;
                this._ParentPath = path;
                long.TryParse(matchingRegex.Groups["size"].Value, out this._Size);
                this._Permission = matchingRegex.Groups["permission"].Value;
                string str = matchingRegex.Groups["dir"].Value;
                if ((str != "") && (str != "-"))
                {
                    this._Properties = FtpFileType.Folder;
                }
                else
                {
                    this._Properties = FtpFileType.File;
                }
                try
                {
                    this._ModifyDateTime = DateTime.Parse(matchingRegex.Groups["timestamp"].Value);
                }
                catch (Exception)
                {
                    this._ModifyDateTime = Convert.ToDateTime((string)null);
                }

            }

            string _FullName = string.Empty;

            public string FullName
            {
                get { return _FullName; }
                set { _FullName = value; }
            }

            string _ExtensionName = string.Empty;

            public string ExtensionName
            {
                get { return _ExtensionName; }
                set { _ExtensionName = value; }
            }

            long _Size = 0;

            public long Size
            {
                get { return _Size; }
                set { _Size = value; }
            }

            string _ParentPath = string.Empty;

            public string ParentPath
            {
                get { return _ParentPath; }
                set { _ParentPath = value; }
            }

            DateTime _ModifyDateTime;

            public DateTime ModifyDateTime
            {
                get { return _ModifyDateTime; }
                set { _ModifyDateTime = value; }
            }

            FtpFileType _Properties = FtpFileType.File;

            public FtpFileType Properties
            {
                get { return _Properties; }
                set { _Properties = value; }
            }

            string _Permission = string.Empty;

            public string Permission
            {
                get { return _Permission; }
                set { _Permission = value; }
            }
        } 
        #endregion

        #region 枚举
        public enum FtpFileType
        {
            File = 1,
            Folder = 2
        }

        public enum FTPMode : int
        {
            Passive = 1,
            Active = 2
        }

        public enum FTPFileTransferType : int
        {
            ASCII = 1,
            Binary = 2
        } 
        #endregion
    }
}
