/*
=========================================================================================================
  Module      : メールテンプレート入力クラス (MailTemplateInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using w2.Domain.MailTemplate;
using w2.App.Common.Input;

/// <summary>
/// メールテンプレートマスタ入力クラス
/// </summary>
[Serializable]
public class MailTemplateInput : InputBase<MailTemplateModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public MailTemplateInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public MailTemplateInput(MailTemplateModel model)
		: this()
	{
		this.ShopId = model.ShopId;
		this.MailId = model.MailId;
		this.MailName = model.MailName;
		this.MailFrom = model.MailFrom;
		this.MailTo = model.MailTo;
		this.MailCc = model.MailCc;
		this.MailBcc = model.MailBcc;
		this.MailSubject = model.MailSubject;
		this.MailBody = model.MailBody;
		this.MailSubjectMobile = model.MailSubjectMobile;
		this.MailBodyMobile = model.MailBodyMobile;
		this.MailAttachmentfilePath = model.MailAttachmentfilePath;
		this.DelFlg = model.DelFlg;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
		this.LastChanged = model.LastChanged;
		this.MailFromName = model.MailFromName;
		this.MailCategory = model.MailCategory;
		this.AutoSendFlg = model.AutoSendFlg;
		this.LanguageCode = model.LanguageCode;
		this.LanguageLocaleId = model.LanguageLocaleId;
		this.SmsUseFlg = model.SmsUseFlg;
		this.LineUseFlg = model.LineUseFlg;
		this.MailTextHtml = model.MailTextHtml;
		this.SendHtmlFlg = model.SendHtmlFlg;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override MailTemplateModel CreateModel()
	{
		var model = new MailTemplateModel
		{
			ShopId = this.ShopId,
			MailId = this.MailId,
			MailName = this.MailName,
			MailFrom = this.MailFrom,
			MailTo = this.MailTo,
			MailCc = this.MailCc,
			MailBcc = this.MailBcc,
			MailSubject = this.MailSubject,
			MailBody = this.MailBody,
			MailSubjectMobile = this.MailSubjectMobile,
			MailBodyMobile = this.MailBodyMobile,
			MailAttachmentfilePath = (this.MailAttachmentfilePath != null) ? this.MailAttachmentfilePath : "",
			DelFlg = (this.DelFlg != null) ? this.DelFlg : Constants.FLG_MAILTEMPLATE_DELFLG_UNDELETED,
			LastChanged = this.LastChanged,
			MailFromName = this.MailFromName,
			MailCategory = this.MailCategory,
			AutoSendFlg = this.AutoSendFlg,
			LanguageCode = this.LanguageCode,
			LanguageLocaleId = this.LanguageLocaleId,
			SmsUseFlg = this.SmsUseFlg,
			LineUseFlg = this.LineUseFlg,
			MailTextHtml = this.MailTextHtml,
			SendHtmlFlg = this.SendHtmlFlg,
		};
		return model;
	}

	/// <summary>
	/// グローバルモデル作成
	/// </summary>
	/// <returns>グローバルモデル</returns>
	public MailTemplateGlobalModel CreateGlobalModel()
	{
		var model = new MailTemplateGlobalModel
		{
			ShopId = this.ShopId,
			MailId = this.MailId,
			MailName = this.MailName,
			MailFrom = this.MailFrom,
			MailTo = this.MailTo,
			MailCc = this.MailCc,
			MailBcc = this.MailBcc,
			MailSubject = this.MailSubject,
			MailBody = this.MailBody,
			MailAttachmentfilePath = (this.MailAttachmentfilePath != null) ? this.MailAttachmentfilePath : "",
			LastChanged = this.LastChanged,
			MailFromName = this.MailFromName,
			MailCategory = this.MailCategory,
			AutoSendFlg = this.AutoSendFlg,
			LanguageCode = this.LanguageCode,
			LanguageLocaleId = this.LanguageLocaleId,
			MailTextHtml = this.MailTextHtml,
			SendHtmlFlg = this.SendHtmlFlg,
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <param name="validatorName">バリデータ名</param>
	/// <returns>エラーメッセージ</returns>
	public string Validate(string validatorName)
	{
		// 入力チェック
		var errorMessages = new StringBuilder(Validator.Validate(validatorName, this.DataSource));

		var errorMessage = "";
		// 送信元メールに「@@mail_addr@@」が含まれない場合はメールアドレス形式チェック
		if (this.MailFrom != "@@mail_addr@@")
		{
			errorMessage = Validator.CheckMailAddrInput("送信元メールアドレス", this.MailFrom);
			if (errorMessage != "") errorMessages.Append(errorMessage).Append("<br />");
		}

		// 送信先メールアドレス形式チェック（カンマ区切りで入力できる）
		if (this.MailTo.Length != 0)
		{
			errorMessage = Validator.CheckMailAddrInputs("送信先メールアドレス", this.MailTo);
			if (errorMessage != "") errorMessages.Append(errorMessage).Append("<br />");
		}

		// Ccメールアドレス形式チェック（カンマ区切りで入力できる）
		if (this.MailCc.Length != 0)
		{
			errorMessage = Validator.CheckMailAddrInputs("Ccメールアドレス", this.MailCc);
			if (errorMessage != "") errorMessages.Append(errorMessage).Append("<br />");
		}

		// Bccメールアドレス形式チェック（カンマ区切りで入力できる）
		if (this.MailBcc.Length != 0)
		{
			errorMessage = Validator.CheckMailAddrInputs("Bccメールアドレス", this.MailBcc);
			if (errorMessage != "") errorMessages.Append(errorMessage).Append("<br />");
		}

		// 言語コード登録重複チェック
		if (Constants.GLOBAL_OPTION_ENABLE && validatorName == "MailTemplateRegist")
		{
			if (new MailTemplateService().CheckLanguageCodeDupulication(this.ShopId, this.MailId, this.LanguageCode, this.LanguageLocaleId) == false)
			{
				errorMessage = WebMessages.GetMessages(WebMessages.INPUTCHECK_DUPLICATION).Replace("@@ 1 @@", "言語コード");
				if (errorMessage != "") errorMessages.Append(errorMessage).Append("<br />");
			}
		}		

		return errorMessages.ToString();
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_SHOP_ID] = value; }
	}
	/// <summary>メールテンプレートID</summary>
	public string MailId
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_ID]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_ID] = value; }
	}
	/// <summary>メールテンプレート名</summary>
	public string MailName
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_NAME]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_NAME] = value; }
	}
	/// <summary>メールFrom</summary>
	public string MailFrom
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_FROM]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_FROM] = value; }
	}
	/// <summary>メールTo</summary>
	public string MailTo
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_TO]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_TO] = value; }
	}
	/// <summary>メールCC</summary>
	public string MailCc
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_CC]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_CC] = value; }
	}
	/// <summary>メールBCC</summary>
	public string MailBcc
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_BCC]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_BCC] = value; }
	}
	/// <summary>メールタイトル</summary>
	public string MailSubject
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_SUBJECT]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_SUBJECT] = value; }
	}
	/// <summary>メール本文</summary>
	public string MailBody
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_BODY]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_BODY] = value; }
	}
	/// <summary>メールタイトル（モバイル用）</summary>
	public string MailSubjectMobile
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_SUBJECT_MOBILE]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_SUBJECT_MOBILE] = value; }
	}
	/// <summary>メール本文（モバイル用）</summary>
	public string MailBodyMobile
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_BODY_MOBILE]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_BODY_MOBILE] = value; }
	}
	/// <summary>メール添付ファイルパス</summary>
	public string MailAttachmentfilePath
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_ATTACHMENTFILE_PATH]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_ATTACHMENTFILE_PATH] = value; }
	}
	/// <summary>削除フラグ</summary>
	public string DelFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_DEL_FLG]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_DEL_FLG] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_LAST_CHANGED] = value; }
	}
	/// <summary>送信元名</summary>
	public string MailFromName
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_FROM_NAME]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_FROM_NAME] = value; }
	}
	/// <summary>メールカテゴリ</summary>
	public string MailCategory
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_CATEGORY]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_CATEGORY] = value; }
	}
	/// <summary>自動送信フラグ</summary>
	public string AutoSendFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_AUTO_SEND_FLG]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_AUTO_SEND_FLG] = value; }
	}
	/// <summary>言語コード</summary>
	public string LanguageCode
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_LANGUAGE_CODE]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_LANGUAGE_CODE] = value; }
	}
	/// <summary>言語ロケールID</summary>
	public string LanguageLocaleId
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_LANGUAGE_LOCALE_ID]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_LANGUAGE_LOCALE_ID] = value; }
	}
	/// <summary>メールテンプレート設定テーブル名(w2_MailTemplate/w2_MailTemplateGlobal)</summary>
	public string MailTemplateSettingTableName
	{
		get { return (string)this.DataSource["setting_table_name"]; }
		set { this.DataSource["setting_table_name"] = value; }
	}
	/// <summary>SMS利用フラグ</summary>
	public string SmsUseFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_SMS_USE_FLG]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_SMS_USE_FLG] = value; }
	}
	/// <summary>LINE送信フラグ</summary>
	public string LineUseFlg
	{
		get
		{
			var result = string.IsNullOrEmpty((string) this.DataSource[Constants.FIELD_MAILTEMPLATE_LINE_USE_FLG])
				? MailTemplateModel.LINE_USE_FLG_OFF
				: (string) this.DataSource[Constants.FIELD_MAILTEMPLATE_LINE_USE_FLG];
			return result;
		}
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_LINE_USE_FLG] = value; }
	}
	/// <summary>Mail Text Html</summary>
	public string MailTextHtml
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_TEXT_HTML]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_MAIL_TEXT_HTML] = value; }
	}
	/// <summary>Send Html Flg</summary>
	public string SendHtmlFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILTEMPLATE_SEND_HTML_FLG]; }
		set { this.DataSource[Constants.FIELD_MAILTEMPLATE_SEND_HTML_FLG] = value; }
	}
	#endregion
}