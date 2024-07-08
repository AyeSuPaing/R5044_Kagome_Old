/*
=========================================================================================================
  Module      : メール送信ログモデル (MailSendLogModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.MailSendLog
{
	/// <summary>
	/// メール送信ログモデル
	/// </summary>
	[Serializable]
	public partial class MailSendLogModel : ModelBase<MailSendLogModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MailSendLogModel()
		{
			this.ReadFlg = Constants.FLG_MAILSENDLOG_READ_FLG_UNREAD;
			this.TextHistoryNo = 0;
			this.MailAddrKbn = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MailSendLogModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MailSendLogModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ログNO</summary>
		public long? LogNo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MAILSENDLOG_LOG_NO] == DBNull.Value) return null;
				return (long?)this.DataSource[Constants.FIELD_MAILSENDLOG_LOG_NO];
			}
		}
		/// <summary>ユーザーID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_USER_ID]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_USER_ID] = value; }
		}
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_DEPT_ID] = value; }
		}
		/// <summary>メール文章ID</summary>
		public string MailtextId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_MAILTEXT_ID]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_MAILTEXT_ID] = value; }
		}
		/// <summary>メール文章名</summary>
		public string MailtextName
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_MAILTEXT_NAME]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_MAILTEXT_NAME] = value; }
		}
		/// <summary>メール配信設定ID</summary>
		public string MaildistId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_MAILDIST_ID]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_MAILDIST_ID] = value; }
		}
		/// <summary>メール配信設定名</summary>
		public string MaildistName
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_MAILDIST_NAME]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_MAILDIST_NAME] = value; }
		}
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_SHOP_ID] = value; }
		}
		/// <summary>メールテンプレートID</summary>
		public string MailId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_ID]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_ID] = value; }
		}
		/// <summary>メールテンプレート名</summary>
		public string MailName
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_NAME]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_NAME] = value; }
		}
		/// <summary>送信元名</summary>
		public string MailFromName
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_FROM_NAME]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_FROM_NAME] = value; }
		}
		/// <summary>メールFrom</summary>
		public string MailFrom
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_FROM]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_FROM] = value; }
		}
		/// <summary>メールTo</summary>
		public string MailTo
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_TO]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_TO] = value; }
		}
		/// <summary>メールCC</summary>
		public string MailCc
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_CC]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_CC] = value; }
		}
		/// <summary>メールBCC</summary>
		public string MailBcc
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_BCC]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_BCC] = value; }
		}
		/// <summary>メールタイトル</summary>
		public string MailSubject
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_SUBJECT]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_SUBJECT] = value; }
		}
		/// <summary>メール本文</summary>
		public string MailBody
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_BODY]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_BODY] = value; }
		}
		/// <summary>メール本文HTML</summary>
		public string MailBodyHtml
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_BODY_HTML]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_BODY_HTML] = value; }
		}
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_ERROR_MESSAGE]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_ERROR_MESSAGE] = value; }
		}
		/// <summary>送信日時</summary>
		public DateTime DateSendMail
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MAILSENDLOG_DATE_SEND_MAIL]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_DATE_SEND_MAIL] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MAILSENDLOG_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_DATE_CREATED] = value; }
		}
		/// <summary>既読フラグ</summary>
		public string ReadFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_READ_FLG]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_READ_FLG] = value; }
		}
		/// <summary>既読日</summary>
		public DateTime? DateReadMail
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MAILSENDLOG_DATE_READ_MAIL] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_MAILSENDLOG_DATE_READ_MAIL];
			}
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_DATE_READ_MAIL] = value; }
		}
		/// <summary>メール配信時文章履歴NO</summary>
		public long TextHistoryNo
		{
			get { return (long)this.DataSource[Constants.FIELD_MAILSENDLOG_TEXT_HISTORY_NO]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_TEXT_HISTORY_NO] = value; }
		}
		/// <summary>メールアドレス区分</summary>
		public string MailAddrKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_ADDR_KBN]; }
			set { this.DataSource[Constants.FIELD_MAILSENDLOG_MAIL_ADDR_KBN] = value; }
		}
		/// <summary>メール文章タグ置換情報</summary>
		public string MailtextReplaceTags
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAILTEXT_REPLACE_TAGS] == DBNull.Value) return string.Empty;
				return (string)this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAILTEXT_REPLACE_TAGS];
			}
			set { this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAILTEXT_REPLACE_TAGS] = value; }
		}
		#endregion
	}
}
