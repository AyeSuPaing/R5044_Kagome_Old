/*
=========================================================================================================
  Module      : ユーザ電子発票管理情報入力クラス (TwUserInvoiceInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using w2.App.Common.Input;
using w2.Domain.TwUserInvoice;

/// <summary>
/// ユーザ配送先情報入力クラス
/// </summary>
public class TwUserInvoiceInput : InputBase<TwUserInvoiceModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public TwUserInvoiceInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public TwUserInvoiceInput(TwUserInvoiceModel model)
		: this()
	{
		this.UserId = model.UserId;
		this.TwInvoiceNo = model.TwInvoiceNo.ToString();
		this.TwInvoiceName = model.TwInvoiceName;
		this.TwUniformInvoice = model.TwUniformInvoice;
		this.TwUniformInvoiceOption1 = model.TwUniformInvoiceOption1;
		this.TwUniformInvoiceOption2 = model.TwUniformInvoiceOption2;
		this.TwCarryType = model.TwCarryType;
		this.TwCarryTypeOption = model.TwCarryTypeOption;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override TwUserInvoiceModel CreateModel()
	{
		var model = new TwUserInvoiceModel
		{
			UserId = this.UserId,
			TwInvoiceNo = int.Parse(this.TwInvoiceNo),
			TwInvoiceName = this.TwInvoiceName,
			TwUniformInvoice = this.TwUniformInvoice,
			TwUniformInvoiceOption1 = StringUtility.ToEmpty(this.TwUniformInvoiceOption1),
			TwUniformInvoiceOption2 = StringUtility.ToEmpty(this.TwUniformInvoiceOption2),
			TwCarryType = StringUtility.ToEmpty(this.TwCarryType),
			TwCarryTypeOption = StringUtility.ToEmpty(this.TwCarryTypeOption)
		};

		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string Validate()
	{
		var input = (Hashtable)this.DataSource.Clone();
		var carryType = StringUtility.ToEmpty(input[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE]);
		switch (carryType)
		{
			case Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE:
				input[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE_OPTION + "_8"] = this.TwCarryTypeOption;
				break;

			case Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE:
				input[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE_OPTION + "_16"] = this.TwCarryTypeOption;
				break;
		}

		var uniformInvoice = StringUtility.ToEmpty(input[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE]);
		switch (uniformInvoice)
		{
			case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
				input[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE_OPTION1 + "_8"] = this.TwUniformInvoiceOption1;
				input[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE_OPTION2] = this.TwUniformInvoiceOption2;
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
				input[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE_OPTION1 + "_3"] = this.TwUniformInvoiceOption1;
				break;
		}
		var errorMessages = Validator.Validate(
			"TwUserInvoice", input);

		return errorMessages;
	}
	#endregion

	#region プロパティ
	/// <summary>ユーザID</summary>
	public string UserId
	{
		get { return (string)this.DataSource[Constants.FIELD_TWUSERINVOICE_USER_ID]; }
		set { this.DataSource[Constants.FIELD_TWUSERINVOICE_USER_ID] = value; }
	}
	/// <summary>電子発票管理枝番</summary>
	public string TwInvoiceNo
	{
		get { return (string)this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NO]; }
		set { this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NO] = value; }
	}
	/// <summary>電子発票情報名</summary>
	public string TwInvoiceName
	{
		get { return (string)this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NAME]; }
		set { this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NAME] = value; }
	}
	/// <summary>電子発票種別</summary>
	public string TwUniformInvoice
	{
		get { return (string)this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE]; }
		set { this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE] = value; }
	}
	/// <summary>電子発票項目1</summary>
	public string TwUniformInvoiceOption1
	{
		get { return (string)this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE_OPTION1]; }
		set { this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE_OPTION1] = value; }
	}
	/// <summary>電子発票項目2</summary>
	public string TwUniformInvoiceOption2
	{
		get { return (string)this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE_OPTION2]; }
		set { this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE_OPTION2] = value; }
	}
	/// <summary>載具種別</summary>
	public string TwCarryType
	{
		get { return (string)this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE]; }
		set { this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE] = value; }
	}
	/// <summary>載具項目</summary>
	public string TwCarryTypeOption
	{
		get { return (string)this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE_OPTION]; }
		set { this.DataSource[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE_OPTION] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_TWUSERINVOICE_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_TWUSERINVOICE_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_TWUSERINVOICE_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_TWUSERINVOICE_DATE_CHANGED] = value; }
	}
	#endregion
}
