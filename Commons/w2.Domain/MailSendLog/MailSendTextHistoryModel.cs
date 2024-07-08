/*
=========================================================================================================
  Module      : メール配信時文章履歴モデル (MailSendTextHistoryModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.MailSendLog
{
	/// <summary>
	/// メール配信時文章履歴モデル
	/// </summary>
	[Serializable]
	public partial class MailSendTextHistoryModel : ModelBase<MailSendTextHistoryModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MailSendTextHistoryModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MailSendTextHistoryModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MailSendTextHistoryModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>メール配信時文章履歴NO</summary>
		public long? TextHistoryNo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_TEXT_HISTORY_NO] == DBNull.Value) return null;
				return (long?)this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_TEXT_HISTORY_NO];
			}
		}
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_DEPT_ID] = value; }
		}
		/// <summary>メール文章ID</summary>
		public string MailtextId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAILTEXT_ID]; }
			set { this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAILTEXT_ID] = value; }
		}
		/// <summary>メール配信設定ID</summary>
		public string MaildistId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAILDIST_ID]; }
			set { this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAILDIST_ID] = value; }
		}
		/// <summary>メール配信設定名</summary>
		public string MaildistName
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAILDIST_NAME]; }
			set { this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAILDIST_NAME] = value; }
		}
		/// <summary>メール本文</summary>
		public string MailBody
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAIL_BODY]; }
			set { this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAIL_BODY] = value; }
		}
		/// <summary>メール本文HTML</summary>
		public string MailBodyHtml
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAIL_BODY_HTML]; }
			set { this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAIL_BODY_HTML] = value; }
		}
		/// <summary>メール文章モバイル</summary>
		public string MailtextBodyMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAILTEXT_BODY_MOBILE]; }
			set { this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAILTEXT_BODY_MOBILE] = value; }
		}
		/// <summary>メール文章デコメ</summary>
		public string MailtextDecome
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAILTEXT_DECOME]; }
			set { this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAILTEXT_DECOME] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_DATE_CREATED] = value; }
		}
		/// <summary>メール文章タグ置換情報</summary>
		public string MailtextReplaceTags
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAILTEXT_REPLACE_TAGS]; }
			set { this.DataSource[Constants.FIELD_MAILSENDTEXTHISTORY_MAILTEXT_REPLACE_TAGS] = value; }
		}
		#endregion
	}
}
