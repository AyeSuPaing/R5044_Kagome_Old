/*
=========================================================================================================
  Module      : メール配信文章マスタモデル (MailDistTextModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.MailDist
{
	/// <summary>
	/// メール配信文章マスタモデル
	/// </summary>
	[Serializable]
	public partial class MailDistTextModel : ModelBase<MailDistTextModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MailDistTextModel()
		{
			// TODO:定数を利用するよう書き換えてください。
			this.SendhtmlFlg = "0";
			this.SenddecomeFlg = "0";
			this.DelFlg = "0";
			this.LanguageCode = string.Empty;
			this.LanguageLocaleId = string.Empty;
			this.SmsUseFlg = SMS_USE_FLG_OFF;
			this.LineUseFlg = LINE_USE_FLG_OFF;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MailDistTextModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MailDistTextModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_DEPT_ID] = value; }
		}
		/// <summary>メール文章ID</summary>
		public string MailtextId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID] = value; }
		}
		/// <summary>メール文章名</summary>
		public string MailtextName
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_MAILTEXT_NAME]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_MAILTEXT_NAME] = value; }
		}
		/// <summary>メールタイトル</summary>
		public string MailtextSubject
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT] = value; }
		}
		/// <summary>メールタイトルモバイル</summary>
		public string MailtextSubjectMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT_MOBILE]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT_MOBILE] = value; }
		}
		/// <summary>メール文章テキスト</summary>
		public string MailtextBody
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY] = value; }
		}
		/// <summary>メール文章HTML</summary>
		public string MailtextHtml
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_MAILTEXT_HTML]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_MAILTEXT_HTML] = value; }
		}
		/// <summary>メール文章モバイル</summary>
		public string MailtextBodyMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY_MOBILE]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY_MOBILE] = value; }
		}
		/// <summary>メール文章デコメ</summary>
		public string MailtextDecome
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_MAILTEXT_DECOME]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_MAILTEXT_DECOME] = value; }
		}
		/// <summary>HTMLメール送信フラグ</summary>
		public string SendhtmlFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_SENDHTML_FLG]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_SENDHTML_FLG] = value; }
		}
		/// <summary>デコメ送信フラグ</summary>
		public string SenddecomeFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_SENDDECOME_FLG]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_SENDDECOME_FLG] = value; }
		}
		/// <summary>メールFROM名</summary>
		public string MailFromName
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_MAIL_FROM_NAME]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_MAIL_FROM_NAME] = value; }
		}
		/// <summary>メールFROM</summary>
		public string MailFrom
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_MAIL_FROM]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_MAIL_FROM] = value; }
		}
		/// <summary>メールCC</summary>
		public string MailCc
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_MAIL_CC]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_MAIL_CC] = value; }
		}
		/// <summary>メールBCC</summary>
		public string MailBcc
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_MAIL_BCC]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_MAIL_BCC] = value; }
		}
		/// <summary>添付ファイル</summary>
		public string MailAttachmentfilePath
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_MAIL_ATTACHMENTFILE_PATH]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_MAIL_ATTACHMENTFILE_PATH] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MAILDISTTEXT_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MAILDISTTEXT_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_LAST_CHANGED] = value; }
		}
		/// <summary>言語コード</summary>
		public string LanguageCode
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_LANGUAGE_CODE]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_LANGUAGE_CODE] = value; }
		}
		/// <summary>言語ロケールID</summary>
		public string LanguageLocaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_LANGUAGE_LOCALE_ID]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_LANGUAGE_LOCALE_ID] = value; }
		}
		/// <summary>SMS利用フラグ</summary>
		public string SmsUseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_SMS_USE_FLG]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_SMS_USE_FLG] = value; }
		}
		/// <summary>LINE利用フラグ</summary>
		public string LineUseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_LINE_USE_FLG]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXT_LINE_USE_FLG] = value; }
		}
		#endregion
	}
}
