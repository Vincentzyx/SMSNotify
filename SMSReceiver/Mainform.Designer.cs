namespace SMSReceiver
{
    partial class Mainform
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mainform));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip_notifyIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem_exit = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox_log = new System.Windows.Forms.GroupBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.richTextBox_log = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.contextMenuStrip_notifyIcon.SuspendLayout();
            this.groupBox_log.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip_notifyIcon;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "短信同步助手";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // contextMenuStrip_notifyIcon
            // 
            this.contextMenuStrip_notifyIcon.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_notifyIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_exit});
            this.contextMenuStrip_notifyIcon.Name = "contextMenuStrip_notifyIcon";
            this.contextMenuStrip_notifyIcon.Size = new System.Drawing.Size(139, 28);
            // 
            // ToolStripMenuItem_exit
            // 
            this.ToolStripMenuItem_exit.Name = "ToolStripMenuItem_exit";
            this.ToolStripMenuItem_exit.Size = new System.Drawing.Size(138, 24);
            this.ToolStripMenuItem_exit.Text = "退出程序";
            this.ToolStripMenuItem_exit.Click += new System.EventHandler(this.ToolStripMenuItem_exit_Click);
            // 
            // groupBox_log
            // 
            this.groupBox_log.Controls.Add(this.statusStrip1);
            this.groupBox_log.Controls.Add(this.richTextBox_log);
            this.groupBox_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_log.Location = new System.Drawing.Point(3, 3);
            this.groupBox_log.Name = "groupBox_log";
            this.groupBox_log.Size = new System.Drawing.Size(545, 307);
            this.groupBox_log.TabIndex = 3;
            this.groupBox_log.TabStop = false;
            this.groupBox_log.Text = "日志";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_status});
            this.statusStrip1.Location = new System.Drawing.Point(3, 278);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(539, 26);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel_status
            // 
            this.toolStripStatusLabel_status.Name = "toolStripStatusLabel_status";
            this.toolStripStatusLabel_status.Size = new System.Drawing.Size(54, 20);
            this.toolStripStatusLabel_status.Text = "未连接";
            // 
            // richTextBox_log
            // 
            this.richTextBox_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_log.Location = new System.Drawing.Point(3, 21);
            this.richTextBox_log.Name = "richTextBox_log";
            this.richTextBox_log.ReadOnly = true;
            this.richTextBox_log.Size = new System.Drawing.Size(539, 283);
            this.richTextBox_log.TabIndex = 1;
            this.richTextBox_log.Text = "";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox_log, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(551, 313);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // Mainform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 313);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Mainform";
            this.ShowInTaskbar = false;
            this.Text = "短信同步助手";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Mainform_FormClosing);
            this.Load += new System.EventHandler(this.Mainform_Load);
            this.Shown += new System.EventHandler(this.Mainform_Shown);
            this.contextMenuStrip_notifyIcon.ResumeLayout(false);
            this.groupBox_log.ResumeLayout(false);
            this.groupBox_log.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.GroupBox groupBox_log;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RichTextBox richTextBox_log;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_notifyIcon;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_exit;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_status;
    }
}

