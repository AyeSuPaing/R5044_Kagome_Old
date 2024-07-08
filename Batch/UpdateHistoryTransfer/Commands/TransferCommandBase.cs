/*
=========================================================================================================
  Module      : 転送コマンド基底クラス(TransferCommandBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Text;
using w2.Common.Logger;
using w2.App.Common;

namespace w2.Commerce.Batch.UpdateHistoryTransfer.Commands
{
	/// <summary>
	/// 転送処理基底クラス
	/// </summary>
	public abstract class TransferCommandBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary
		public TransferCommandBase()
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 転送処理実行
		/// </summary>
		public void Execute()
		{
			// 開始メール送信
			this.BeginDate = DateTime.Now;
			SendMail();

			// 継承クラスの処理
			Exec();

			// 終了メール送信
			this.EndDate = DateTime.Now;
			SendMail();
		}

		/// <summary>
		/// 継承クラスの処理
		/// </summary>
		protected abstract void Exec();

		/// <summary>
		/// 開始・完了メール送信
		/// </summary>
		protected void SendMail()
		{
			var messages = string.Format("「{0}」{1}\r\n", this.Action, (this.EndDate == null) ? "処理を開始しました。" : "処理が終了しました。")
				+ "\r\n"
				+ string.Format("処理開始時間：{0}\r\n", this.BeginDate.ToString())
				+ string.Format("処理終了時間：{0}\r\n", (this.EndDate == null) ? "-" : this.EndDate.ToString())
				+ "\r\n";

			using (var sendMail = new MailSendUtility(Constants.MailSendMethod.Manual))
			{
				sendMail.SetSubject(Constants.MAIL_SUBJECTHEAD);
				sendMail.SetFrom(Constants.MAIL_FROM.Address);
				Constants.MAIL_TO_LIST.ForEach(mail => sendMail.AddTo(mail.Address));
				Constants.MAIL_CC_LIST.ForEach(mail => sendMail.AddCC(mail.Address));
				Constants.MAIL_BCC_LIST.ForEach(mail => sendMail.AddBcc(mail.Address));
				sendMail.SetBody(messages.ToString() + this.Messages);
				if (sendMail.SendMail() == false)
				{
					FileLogger.WriteError(sendMail.Body, sendMail.MailSendException);
				}
			}
		}
		#endregion

		#region プロパティ
		/// <summary>テーブル名</summary>
		protected string TableName { get; set; }
		/// <summary>取込開始日時</summary>
		private DateTime? BeginDate { get; set; }
		/// <summary>取込終了日時</summary>
		private DateTime? EndDate { get; set; }
		/// <summary>処理</summary>
		public string Action { get; set; }
		/// <summary>メッセージ</summary>
		public string Messages { get; set; }
		#endregion
	}
}