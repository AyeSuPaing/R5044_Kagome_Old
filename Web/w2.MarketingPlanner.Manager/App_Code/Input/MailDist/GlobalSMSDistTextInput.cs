/*
=========================================================================================================
  Module      : SMS配信文言入力クラス (GlobalSMSDistTextInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Input;
using w2.Domain.GlobalSMS;

/// <summary>
/// SMS配信文言入力クラス
/// </summary>
[Serializable]
public class GlobalSMSDistTextInput : InputBase<GlobalSMSDistTextModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public GlobalSMSDistTextInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public GlobalSMSDistTextInput(GlobalSMSDistTextModel model)
		: this()
	{
		this.DeptId = model.DeptId;
		this.MailtextId = model.MailtextId;
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
	public override GlobalSMSDistTextModel CreateModel()
	{
		var model = new GlobalSMSDistTextModel
		{
			DeptId = this.DeptId,
			MailtextId = this.MailtextId,
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
		this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_SMS_TEXT + "_" + this.PhoneCarrier] = this.SmsText;
		var errorMessage = Validator.Validate("GlobalSMSDistText", this.DataSource);
		return errorMessage;
	}
	#endregion

	#region プロパティ
	/// <summary>識別ID</summary>
	public string DeptId
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_DEPT_ID]; }
		set { this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_DEPT_ID] = value; }
	}
	/// <summary>メール文章ID</summary>
	public string MailtextId
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_MAILTEXT_ID]; }
		set { this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_MAILTEXT_ID] = value; }
	}
	/// <summary>キャリア</summary>
	public string PhoneCarrier
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_PHONE_CARRIER]; }
		set { this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_PHONE_CARRIER] = value; }
	}
	/// <summary>SMS本文</summary>
	public string SmsText
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_SMS_TEXT]; }
		set { this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_SMS_TEXT] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_GLOBALSMSDISTTEXT_LAST_CHANGED] = value; }
	}
	#endregion
}
