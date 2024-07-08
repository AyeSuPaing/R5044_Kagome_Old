/*
=========================================================================================================
  Module      : メール配信文章入力クラス (MailDistTextInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Input;
using w2.Domain.MailDist;

/// <summary>
/// メール配信文章マスタ入力クラス
/// </summary>
public class MailDistTextInput : InputBase<MailDistTextModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public MailDistTextInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public MailDistTextInput(MailDistTextModel model)
		: this()
	{
		this.DeptId = model.DeptId;
		this.MailtextId = model.MailtextId;
		this.MailtextName = model.MailtextName;
		this.MailtextSubject = model.MailtextSubject;
		this.MailtextSubjectMobile = model.MailtextSubjectMobile;
		this.MailtextBody = model.MailtextBody;
		this.MailtextHtml = model.MailtextHtml;
		this.MailtextBodyMobile = model.MailtextBodyMobile;
		this.MailtextDecome = model.MailtextDecome;
		this.SendhtmlFlg = model.SendhtmlFlg;
		this.SenddecomeFlg = model.SenddecomeFlg;
		this.MailFromName = model.MailFromName;
		this.MailFrom = model.MailFrom;
		this.MailCc = model.MailCc;
		this.MailBcc = model.MailBcc;
		this.MailAttachmentfilePath = model.MailAttachmentfilePath;
		this.DelFlg = model.DelFlg;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
		this.LastChanged = model.LastChanged;
		this.LanguageCode = model.LanguageCode;
		this.LanguageLocaleId = model.LanguageLocaleId;
		this.SmsUseFlg = model.SmsUseFlg;
		this.LineUseFlg = model.LineUseFlg;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override MailDistTextModel CreateModel()
	{
		var model = new MailDistTextModel
		{
			DeptId = this.DeptId,
			MailtextId = this.MailtextId,
			MailtextName = this.MailtextName,
			MailtextSubject = this.MailtextSubject,
			MailtextSubjectMobile = this.MailtextSubjectMobile,
			MailtextBody = this.MailtextBody,
			MailtextHtml = this.MailtextHtml,
			MailtextBodyMobile = this.MailtextBodyMobile,
			MailtextDecome = this.MailtextDecome,
			SendhtmlFlg = this.SendhtmlFlg,
			SenddecomeFlg = this.SenddecomeFlg,
			MailFromName = this.MailFromName,
			MailFrom = this.MailFrom,
			MailCc = this.MailCc,
			MailBcc = this.MailBcc,
			MailAttachmentfilePath = this.MailAttachmentfilePath,
			DelFlg = this.DelFlg,
			LastChanged = this.LastChanged,
			LanguageCode = this.LanguageCode,
			LanguageLocaleId = this.LanguageLocaleId,
			SmsUseFlg = this.SmsUseFlg,
			LineUseFlg = this.LineUseFlg,
		};
		return model;
	}

	/// <summary>
	/// グローバルモデル作成
	/// </summary>
	/// <returns>グローバルモデル</returns>
	public MailDistTextGlobalModel CreateGlobalModel()
	{
		var model = new MailDistTextGlobalModel
		{
			DeptId = this.DeptId,
			MailtextId = this.MailtextId,
			LanguageCode = this.LanguageCode,
			LanguageLocaleId = this.LanguageLocaleId,
			MailtextName = this.MailtextName,
			MailtextSubject = this.MailtextSubject,
			MailtextBody = this.MailtextBody,
			MailtextHtml = this.MailtextHtml,
			SendhtmlFlg = this.SendhtmlFlg,
			MailFromName = this.MailFromName,
			MailFrom = this.MailFrom,
			LastChanged = this.LastChanged,
			SmsUseFlg = this.SmsUseFlg,
		};
		return model;
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
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_MAILDISTTEXT_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTTEXT_DATE_CHANGED]; }
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
