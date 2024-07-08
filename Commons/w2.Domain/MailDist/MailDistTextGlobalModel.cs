/*
=========================================================================================================
  Module      : メール配信文章グローバルマスタモデル (MailDistTextGlobalModel.cs)
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
	/// メール配信文章グローバルマスタモデル
	/// </summary>
	[Serializable]
	public partial class MailDistTextGlobalModel : ModelBase<MailDistTextGlobalModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MailDistTextGlobalModel()
		{
			// TODO:定数を利用するよう書き換えてください。
			this.SendhtmlFlg = "0";
			this.SmsUseFlg = MailDistTextModel.SMS_USE_FLG_OFF;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MailDistTextGlobalModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MailDistTextGlobalModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_DEPT_ID] = value; }
		}
		/// <summary>メール文章ID</summary>
		public string MailtextId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_MAILTEXT_ID]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_MAILTEXT_ID] = value; }
		}
		/// <summary>言語コード</summary>
		public string LanguageCode
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_LANGUAGE_CODE]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_LANGUAGE_CODE] = value; }
		}
		/// <summary>言語ロケールID</summary>
		public string LanguageLocaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_LANGUAGE_LOCALE_ID]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_LANGUAGE_LOCALE_ID] = value; }
		}
		/// <summary>メール文章名</summary>
		public string MailtextName
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_MAILTEXT_NAME]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_MAILTEXT_NAME] = value; }
		}
		/// <summary>メールタイトル</summary>
		public string MailtextSubject
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_MAILTEXT_SUBJECT]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_MAILTEXT_SUBJECT] = value; }
		}
		/// <summary>メール文章テキスト</summary>
		public string MailtextBody
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_MAILTEXT_BODY]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_MAILTEXT_BODY] = value; }
		}
		/// <summary>メール文章HTML</summary>
		public string MailtextHtml
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_MAILTEXT_HTML]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_MAILTEXT_HTML] = value; }
		}
		/// <summary>HTMLメール送信フラグ</summary>
		public string SendhtmlFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_SENDHTML_FLG]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_SENDHTML_FLG] = value; }
		}
		/// <summary>メールFROM名</summary>
		public string MailFromName
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_MAIL_FROM_NAME]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_MAIL_FROM_NAME] = value; }
		}
		/// <summary>メールFROM</summary>
		public string MailFrom
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_MAIL_FROM]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_MAIL_FROM] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_LAST_CHANGED] = value; }
		}
		/// <summary>SMS利用フラグ</summary>
		public string SmsUseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_SMS_USE_FLG]; }
			set { this.DataSource[Constants.FIELD_MAILDISTTEXTGLOBAL_SMS_USE_FLG] = value; }
		}
		#endregion
	}
}
