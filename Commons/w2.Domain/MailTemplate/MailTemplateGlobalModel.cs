/*
=========================================================================================================
  Module      : メールテンプレートグローバルマスタモデル (MailTemplateGlobalModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.MailTemplate
{
	/// <summary>
	/// メールテンプレートグローバルマスタモデル
	/// </summary>
	[Serializable]
	public partial class MailTemplateGlobalModel : ModelBase<MailTemplateGlobalModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MailTemplateGlobalModel()
		{
			this.AutoSendFlg = Constants.FLG_MAILTEMPLATE_AUTOSENDFLG_NOTSEND;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MailTemplateGlobalModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MailTemplateGlobalModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_SHOP_ID] = value; }
		}
		/// <summary>メールテンプレートID</summary>
		public string MailId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_ID]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_ID] = value; }
		}
		/// <summary>言語コード</summary>
		public string LanguageCode
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_LANGUAGE_CODE]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_LANGUAGE_CODE] = value; }
		}
		/// <summary>言語ロケールID</summary>
		public string LanguageLocaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_LANGUAGE_LOCALE_ID]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_LANGUAGE_LOCALE_ID] = value; }
		}
		/// <summary>メールテンプレート名</summary>
		public string MailName
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_NAME]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_NAME] = value; }
		}
		/// <summary>メールFrom</summary>
		public string MailFrom
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_FROM]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_FROM] = value; }
		}
		/// <summary>メールTo</summary>
		public string MailTo
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_TO]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_TO] = value; }
		}
		/// <summary>メールCC</summary>
		public string MailCc
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_CC]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_CC] = value; }
		}
		/// <summary>メールBCC</summary>
		public string MailBcc
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_BCC]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_BCC] = value; }
		}
		/// <summary>メールタイトル</summary>
		public string MailSubject
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_SUBJECT]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_SUBJECT] = value; }
		}
		/// <summary>メール本文</summary>
		public string MailBody
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_BODY]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_BODY] = value; }
		}
		/// <summary>メール添付ファイルパス</summary>
		public string MailAttachmentfilePath
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_ATTACHMENTFILE_PATH]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_ATTACHMENTFILE_PATH] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_LAST_CHANGED] = value; }
		}
		/// <summary>送信元名</summary>
		public string MailFromName
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_FROM_NAME]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_FROM_NAME] = value; }
		}
		/// <summary>メールカテゴリ</summary>
		public string MailCategory
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_CATEGORY]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_MAIL_CATEGORY] = value; }
		}
		/// <summary>自動送信フラグ</summary>
		public string AutoSendFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_AUTO_SEND_FLG]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATEGLOBAL_AUTO_SEND_FLG] = value; }
		}
		/// <summary>メール文章HTML</summary>
		public string MailTextHtml
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_TEXT_HTML]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_TEXT_HTML] = value; }
		}
		/// <summary>HTMLメール送信フラグ</summary>
		public string SendHtmlFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_SEND_HTML_FLG]; }
			set { this.DataSource[Constants.FIELD_MAILTEMPLATE_SEND_HTML_FLG] = value; }
		}
		#endregion
	}
}
