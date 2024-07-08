/*
=========================================================================================================
  Module      : メッセージ集計向けレポートマトリクス行モデル(ReportMatrixRowModelForMessage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.App.Common.Cs.Reports
{
	/// <summary>
	///  メッセージ集計向けレポートマトリクス行モデル
	/// </summary>
	[Serializable]
	public class ReportMatrixRowModelForMessage : ReportRowModel
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ReportMatrixRowModelForMessage()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ReportMatrixRowModelForMessage(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ReportMatrixRowModelForMessage(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>トータルカウント</summary>
		public new int? Count
		{
			get
			{
				if (this.TelReceiveCount.HasValue || this.TelSendCount.HasValue || this.MailReceiveCount.HasValue || this.MailSendCount.HasValue || this.OthersReceiveCount.HasValue || this.OthersSendCount.HasValue)
				{
					return (this.TelReceiveCount ?? 0) + (this.TelSendCount ?? 0) + (this.MailReceiveCount ?? 0) + (this.MailSendCount ?? 0) + (this.OthersReceiveCount ?? 0) + (this.OthersSendCount ?? 0);
				}
				return null;
			}
		}
		/// <summary>電話受信件数</summary>
		public int? TelReceiveCount
		{
			get { return (int?)this.DataSource["TelReceiveCount"]; }
			set { this.DataSource["TelReceiveCount"] = value; }
		}
		/// <summary>電話発信件数</summary>
		public int? TelSendCount
		{
			get { return (int?)this.DataSource["TelSendCount"]; }
			set { this.DataSource["TelSendCount"] = value; }
		}
		/// <summary>メール受信件数</summary>
		public int? MailReceiveCount
		{
			get { return (int?)this.DataSource["MailReceiveCount"]; }
			set { this.DataSource["MailReceiveCount"] = value; }
		}
		/// <summary>メール発信件数</summary>
		public int? MailSendCount
		{
			get { return (int?)this.DataSource["MailSendCount"]; }
			set { this.DataSource["MailSendCount"] = value; }
		}
		/// <summary>その他受信件数</summary>
		public int? OthersReceiveCount
		{
			get { return (int?)this.DataSource["OthersReceiveCount"]; }
			set { this.DataSource["OthersReceiveCount"] = value; }
		}
		/// <summary>その他発信件数</summary>
		public int? OthersSendCount
		{
			get { return (int?)this.DataSource["OthersSendCount"]; }
			set { this.DataSource["OthersSendCount"] = value; }
		}
		#endregion
	}
}
