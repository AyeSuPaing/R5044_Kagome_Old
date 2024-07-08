/*
=========================================================================================================
  Module      : SMSテンプレート入力クラス (GlobalSMSTemplateInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using w2.App.Common.Input;
using w2.Domain.GlobalSMS;

/// <summary>
/// SMSテンプレート入力クラス
/// </summary>
[Serializable]
public class GlobalSMSTemplateInput : InputBase<GlobalSMSTemplateModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public GlobalSMSTemplateInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public GlobalSMSTemplateInput(GlobalSMSTemplateModel model)
		: this()
	{
		this.ShopId = model.ShopId;
		this.MailId = model.MailId;
		this.PhoneCarrier = model.PhoneCarrier;
		this.SmsText = model.SmsText;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
		this.LastChanged = model.LastChanged;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override GlobalSMSTemplateModel CreateModel()
	{
		var model = new GlobalSMSTemplateModel
		{
			ShopId = this.ShopId,
			MailId = this.MailId,
			PhoneCarrier = this.PhoneCarrier,
			SmsText = this.SmsText,
			LastChanged = this.LastChanged,
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string Validate()
	{
		// キャリア別にチェックできるように
		this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_SMS_TEXT + "_" + this.PhoneCarrier] = this.SmsText;
		var errorMessage = Validator.Validate("GlobalSMSTemplate", this.DataSource);
		return errorMessage;
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_SHOP_ID] = value; }
	}
	/// <summary>メールテンプレートID</summary>
	public string MailId
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_MAIL_ID]; }
		set { this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_MAIL_ID] = value; }
	}
	/// <summary>キャリア</summary>
	public string PhoneCarrier
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_PHONE_CARRIER]; }
		set { this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_PHONE_CARRIER] = value; }
	}
	/// <summary>SMS本文</summary>
	public string SmsText
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_SMS_TEXT]; }
		set { this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_SMS_TEXT] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_GLOBALSMSTEMPLATE_LAST_CHANGED] = value; }
	}
	#endregion
}
