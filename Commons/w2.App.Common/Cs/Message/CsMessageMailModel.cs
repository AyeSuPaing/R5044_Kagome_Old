/*
=========================================================================================================
  Module      : メッセージメールモデル(CsMessageMailModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.Message
{
	/// <summary>
	/// メッセージメールモデル
	/// </summary>
	[Serializable]
	public partial class CsMessageMailModel : ModelBase<CsMessageMailModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsMessageMailModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">データ</param>
		public CsMessageMailModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">データ</param>
		public CsMessageMailModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAIL_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAIL_DEPT_ID] = value; }
		}
		/// <summary>メールID</summary>
		public string MailId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAIL_MAIL_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAIL_MAIL_ID] = value; }
		}
		/// <summary>メール区分</summary>
		public string MailKbn
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAIL_MAIL_KBN]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAIL_MAIL_KBN] = value; }
		}
		/// <summary>メールFROM</summary>
		public string MailFrom
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAIL_MAIL_FROM]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAIL_MAIL_FROM] = value; }
		}
		/// <summary>メールTO</summary>
		public string MailTo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAIL_MAIL_TO]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAIL_MAIL_TO] = value; }
		}
		/// <summary>メールCC</summary>
		public string MailCc
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAIL_MAIL_CC]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAIL_MAIL_CC] = value; }
		}
		/// <summary>メールBCC</summary>
		public string MailBcc
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAIL_MAIL_BCC]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAIL_MAIL_BCC] = value; }
		}
		/// <summary>メール件名</summary>
		public string MailSubject
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAIL_MAIL_SUBJECT]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAIL_MAIL_SUBJECT] = value; }
		}
		/// <summary>メール本文</summary>
		public string MailBody
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAIL_MAIL_BODY]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAIL_MAIL_BODY] = value; }
		}
		/// <summary>送信オペレータID</summary>
		public string SendOperatorId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAIL_SEND_OPERATOR_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAIL_SEND_OPERATOR_ID] = value; }
		}
		/// <summary>送信日時</summary>
		public DateTime? SendDatetime
		{
			get
			{
				if (this.DataSource[Constants.FIELD_CSMESSAGEMAIL_SEND_DATETIME] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_CSMESSAGEMAIL_SEND_DATETIME];
			}
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAIL_SEND_DATETIME] = value; }
		}
		/// <summary>受信日時</summary>
		public DateTime? ReceiveDatetime
		{
			get
			{
				if (this.DataSource[Constants.FIELD_CSMESSAGEMAIL_RECEIVE_DATETIME] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_CSMESSAGEMAIL_RECEIVE_DATETIME];
			}
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAIL_RECEIVE_DATETIME] = value; }
		}
		/// <summary>Message-Id</summary>
		public string MessageId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAIL_MESSAGE_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAIL_MESSAGE_ID] = value; }
		}
		/// <summary>In-Reply-To</summary>
		public string InReplyTo
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAIL_IN_REPLY_TO]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAIL_IN_REPLY_TO] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAIL_DEL_FLG]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAIL_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSMESSAGEMAIL_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAIL_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSMESSAGEMAIL_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAIL_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEMAIL_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEMAIL_LAST_CHANGED] = value; }
		}
		#endregion

	}
}
