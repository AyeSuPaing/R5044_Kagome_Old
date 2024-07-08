/*
=========================================================================================================
  Module      : ユーザー履歴（メール送信ログ）クラス(UserHistoryMailSendLog.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Util;
using w2.Domain.MailSendLog;

namespace w2.App.Common.Cs.UserHistory
{
	/// <summary>
	/// ユーザー履歴（メール送信ログ）クラス
	/// </summary>
	public class UserHistoryMailSendLog : UserHistoryBase
	{
		private const string KBN_STRING = "メール送信";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="info">情報</param>
		public UserHistoryMailSendLog(MailSendLogModel info)
			: base(info.DataSource)
		{
			this.MailSendLog = info;
		}

		/// <summary>
		/// 情報セット
		/// </summary>
		protected override void SetInfo()
		{
			// 送信日時をセット
			this.DateTime = this.DateSendMail;
			this.KbnString = KBN_STRING;
		}

		#region プロパティ
		/// <summary>メール送信ログ情報</summary>
		public MailSendLogModel MailSendLog { get; private set; }
		/// <summary>送信日時</summary>
		public DateTime DateSendMail
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MAILSENDLOG_DATE_SEND_MAIL]; }
		}
		#endregion
	}
}