/*
=========================================================================================================
  Module      : メインフォームデザイナ(Form1.Designer.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
namespace w2.MarketingPlanner.Win.ScheduleManager
{
	partial class Form1
	{
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナで生成されたコード

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.contextMenuStripIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.ToolStripMenuItemClose = new System.Windows.Forms.ToolStripMenuItem();
			this.tbMessage = new System.Windows.Forms.TextBox();
			this.timerDisplayMessage = new System.Windows.Forms.Timer(this.components);
			this.btnClose = new System.Windows.Forms.Button();
			this.btnHide = new System.Windows.Forms.Button();
			this.timerWriteMessage = new System.Windows.Forms.Timer(this.components);
			this.btnCopyClipboad = new System.Windows.Forms.Button();
			this.contextMenuStripIcon.SuspendLayout();
			this.SuspendLayout();
			// 
			// notifyIcon
			// 
			this.notifyIcon.ContextMenuStrip = this.contextMenuStripIcon;
			this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
			this.notifyIcon.Text = "スケジュールマネージャ";
			this.notifyIcon.Visible = true;
			this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
			// 
			// contextMenuStripIcon
			// 
			this.contextMenuStripIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemClose});
			this.contextMenuStripIcon.Name = "contextMenuStrip1";
			this.contextMenuStripIcon.Size = new System.Drawing.Size(95, 26);
			// 
			// ToolStripMenuItemClose
			// 
			this.ToolStripMenuItemClose.Name = "ToolStripMenuItemClose";
			this.ToolStripMenuItemClose.Size = new System.Drawing.Size(94, 22);
			this.ToolStripMenuItemClose.Text = "終了";
			this.ToolStripMenuItemClose.Click += new System.EventHandler(this.ToolStripMenuItemClose_Click);
			// 
			// tbMessage
			// 
			this.tbMessage.Location = new System.Drawing.Point(3, 3);
			this.tbMessage.Multiline = true;
			this.tbMessage.Name = "tbMessage";
			this.tbMessage.ReadOnly = true;
			this.tbMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbMessage.Size = new System.Drawing.Size(471, 289);
			this.tbMessage.TabIndex = 1;
			this.tbMessage.TextChanged += new System.EventHandler(this.tbMessage_TextChanged);
			// 
			// timerDisplayMessage
			// 
			this.timerDisplayMessage.Enabled = true;
			this.timerDisplayMessage.Interval = 1000;
			this.timerDisplayMessage.Tick += new System.EventHandler(this.timerDisplayMessage_Tick);
			// 
			// btnClose
			// 
			this.btnClose.Location = new System.Drawing.Point(399, 298);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(75, 23);
			this.btnClose.TabIndex = 2;
			this.btnClose.Text = "終了";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// btnHide
			// 
			this.btnHide.Location = new System.Drawing.Point(318, 297);
			this.btnHide.Name = "btnHide";
			this.btnHide.Size = new System.Drawing.Size(75, 23);
			this.btnHide.TabIndex = 3;
			this.btnHide.Text = "隠す";
			this.btnHide.UseVisualStyleBackColor = true;
			this.btnHide.Click += new System.EventHandler(this.btnHide_Click);
			// 
			// timerWriteMessage
			// 
			this.timerWriteMessage.Enabled = true;
			this.timerWriteMessage.Interval = 10000;
			this.timerWriteMessage.Tick += new System.EventHandler(this.timerWriteMessage_Tick);
			// 
			// btnCopyClipboad
			// 
			this.btnCopyClipboad.Location = new System.Drawing.Point(12, 298);
			this.btnCopyClipboad.Name = "btnCopyClipboad";
			this.btnCopyClipboad.Size = new System.Drawing.Size(115, 23);
			this.btnCopyClipboad.TabIndex = 4;
			this.btnCopyClipboad.Text = "クリップボードコピー";
			this.btnCopyClipboad.UseVisualStyleBackColor = true;
			this.btnCopyClipboad.Click += new System.EventHandler(this.btnCopyClipboad_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(478, 325);
			this.Controls.Add(this.btnCopyClipboad);
			this.Controls.Add(this.tbMessage);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnHide);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Form1";
			this.Text = "スケジュールマネージャ";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.contextMenuStripIcon.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NotifyIcon notifyIcon;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripIcon;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemClose;
		private System.Windows.Forms.TextBox tbMessage;
		private System.Windows.Forms.Timer timerDisplayMessage;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btnHide;
		private System.Windows.Forms.Timer timerWriteMessage;
		private System.Windows.Forms.Button btnCopyClipboad;
	}
}

