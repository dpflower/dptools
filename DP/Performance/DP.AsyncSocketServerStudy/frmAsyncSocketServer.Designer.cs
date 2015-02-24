namespace DP.AsyncSocketServerStudy
{
    partial class frmAsyncSocketServer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.设定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuParament = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.lbMainMessage = new System.Windows.Forms.ListBox();
            this.操作 = new System.Windows.Forms.GroupBox();
            this.txtSendData = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnListen = new System.Windows.Forms.Button();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtBufferSize = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtConnections = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl = new System.Windows.Forms.Label();
            this.gbClient = new System.Windows.Forms.GroupBox();
            this.lbClientList = new System.Windows.Forms.ListBox();
            this.gbMessage = new System.Windows.Forms.GroupBox();
            this.btnClientCount = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.操作.SuspendLayout();
            this.gbClient.SuspendLayout();
            this.gbMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.设定ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(834, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 设定ToolStripMenuItem
            // 
            this.设定ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuParament,
            this.mnuQuit});
            this.设定ToolStripMenuItem.Name = "设定ToolStripMenuItem";
            this.设定ToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.设定ToolStripMenuItem.Text = "系统操作";
            // 
            // mnuParament
            // 
            this.mnuParament.Name = "mnuParament";
            this.mnuParament.Size = new System.Drawing.Size(118, 22);
            this.mnuParament.Text = "参数设定";
            this.mnuParament.Click += new System.EventHandler(this.mnuParament_Click);
            // 
            // mnuQuit
            // 
            this.mnuQuit.Name = "mnuQuit";
            this.mnuQuit.Size = new System.Drawing.Size(118, 22);
            this.mnuQuit.Text = "退出系统";
            this.mnuQuit.Click += new System.EventHandler(this.mnuQuit_Click);
            // 
            // lbMainMessage
            // 
            this.lbMainMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbMainMessage.FormattingEnabled = true;
            this.lbMainMessage.ItemHeight = 12;
            this.lbMainMessage.Location = new System.Drawing.Point(3, 17);
            this.lbMainMessage.Name = "lbMainMessage";
            this.lbMainMessage.Size = new System.Drawing.Size(561, 304);
            this.lbMainMessage.TabIndex = 3;
            // 
            // 操作
            // 
            this.操作.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.操作.Controls.Add(this.btnClientCount);
            this.操作.Controls.Add(this.txtSendData);
            this.操作.Controls.Add(this.label3);
            this.操作.Controls.Add(this.btnStop);
            this.操作.Controls.Add(this.btnSend);
            this.操作.Controls.Add(this.btnListen);
            this.操作.Controls.Add(this.txtPort);
            this.操作.Controls.Add(this.txtBufferSize);
            this.操作.Controls.Add(this.label2);
            this.操作.Controls.Add(this.txtConnections);
            this.操作.Controls.Add(this.label1);
            this.操作.Controls.Add(this.lbl);
            this.操作.Location = new System.Drawing.Point(0, 26);
            this.操作.Name = "操作";
            this.操作.Size = new System.Drawing.Size(834, 132);
            this.操作.TabIndex = 4;
            this.操作.TabStop = false;
            this.操作.Text = "操作";
            // 
            // txtSendData
            // 
            this.txtSendData.Location = new System.Drawing.Point(87, 54);
            this.txtSendData.Name = "txtSendData";
            this.txtSendData.Size = new System.Drawing.Size(297, 21);
            this.txtSendData.TabIndex = 4;
            this.txtSendData.Text = "要虽中霜叶";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "SendData";
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(476, 20);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "停止";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(393, 53);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "发送";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnListen
            // 
            this.btnListen.Location = new System.Drawing.Point(393, 20);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new System.Drawing.Size(75, 23);
            this.btnListen.TabIndex = 2;
            this.btnListen.Text = "监听";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Click += new System.EventHandler(this.btnListen_Click);
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(323, 21);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(61, 21);
            this.txtPort.TabIndex = 1;
            this.txtPort.Text = "8002";
            // 
            // txtBufferSize
            // 
            this.txtBufferSize.Location = new System.Drawing.Point(224, 21);
            this.txtBufferSize.Name = "txtBufferSize";
            this.txtBufferSize.Size = new System.Drawing.Size(61, 21);
            this.txtBufferSize.TabIndex = 1;
            this.txtBufferSize.Text = "1000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(289, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Port";
            // 
            // txtConnections
            // 
            this.txtConnections.Location = new System.Drawing.Point(87, 21);
            this.txtConnections.Name = "txtConnections";
            this.txtConnections.Size = new System.Drawing.Size(61, 21);
            this.txtConnections.TabIndex = 1;
            this.txtConnections.Text = "1000";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(154, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "BufferSize";
            // 
            // lbl
            // 
            this.lbl.AutoSize = true;
            this.lbl.Location = new System.Drawing.Point(12, 26);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(71, 12);
            this.lbl.TabIndex = 0;
            this.lbl.Text = "Connections";
            // 
            // gbClient
            // 
            this.gbClient.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbClient.Controls.Add(this.lbClientList);
            this.gbClient.Location = new System.Drawing.Point(0, 162);
            this.gbClient.Name = "gbClient";
            this.gbClient.Size = new System.Drawing.Size(261, 330);
            this.gbClient.TabIndex = 5;
            this.gbClient.TabStop = false;
            this.gbClient.Text = "客户端";
            // 
            // lbClientList
            // 
            this.lbClientList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lbClientList.FormattingEnabled = true;
            this.lbClientList.ItemHeight = 12;
            this.lbClientList.Location = new System.Drawing.Point(3, 17);
            this.lbClientList.Name = "lbClientList";
            this.lbClientList.Size = new System.Drawing.Size(255, 304);
            this.lbClientList.TabIndex = 0;
            // 
            // gbMessage
            // 
            this.gbMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbMessage.Controls.Add(this.lbMainMessage);
            this.gbMessage.Location = new System.Drawing.Point(267, 162);
            this.gbMessage.Name = "gbMessage";
            this.gbMessage.Size = new System.Drawing.Size(567, 330);
            this.gbMessage.TabIndex = 6;
            this.gbMessage.TabStop = false;
            this.gbMessage.Text = "消息";
            // 
            // btnClientCount
            // 
            this.btnClientCount.Location = new System.Drawing.Point(476, 54);
            this.btnClientCount.Name = "btnClientCount";
            this.btnClientCount.Size = new System.Drawing.Size(75, 23);
            this.btnClientCount.TabIndex = 5;
            this.btnClientCount.Text = "Count";
            this.btnClientCount.UseVisualStyleBackColor = true;
            this.btnClientCount.Click += new System.EventHandler(this.btnClientCount_Click);
            // 
            // frmAsyncSocketServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 491);
            this.Controls.Add(this.gbMessage);
            this.Controls.Add(this.gbClient);
            this.Controls.Add(this.操作);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmAsyncSocketServer";
            this.Text = "AsyncSocketServer";
            this.Load += new System.EventHandler(this.frmAsyncSocketServer_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.操作.ResumeLayout(false);
            this.操作.PerformLayout();
            this.gbClient.ResumeLayout(false);
            this.gbMessage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ListBox lbMainMessage;
        private System.Windows.Forms.ToolStripMenuItem 设定ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuParament;
        private System.Windows.Forms.ToolStripMenuItem mnuQuit;
        private System.Windows.Forms.GroupBox 操作;
        private System.Windows.Forms.TextBox txtConnections;
        private System.Windows.Forms.Label lbl;
        private System.Windows.Forms.TextBox txtBufferSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnListen;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSendData;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.GroupBox gbClient;
        private System.Windows.Forms.GroupBox gbMessage;
        private System.Windows.Forms.ListBox lbClientList;
        private System.Windows.Forms.Button btnClientCount;
    }
}