using System;
/*
=========================================================================================================
  Module      : 定期購入電子発票情報入力クラス (TwFixedPurchaseInvoiceInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.App.Common.Input;
using w2.Domain.TwFixedPurchaseInvoice;

/// <summary>
/// 定期購入電子発票情報入力クラス
/// </summary>
[Serializable]
public class TwFixedPurchaseInvoiceInput : InputBase<TwFixedPurchaseInvoiceModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public TwFixedPurchaseInvoiceInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public TwFixedPurchaseInvoiceInput(TwFixedPurchaseInvoiceModel model)
		: this()
	{
		this.FixedPurchaseId = model.FixedPurchaseId;
		this.FixedPurchaseShippingNo = model.FixedPurchaseShippingNo.ToString();
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
	public override TwFixedPurchaseInvoiceModel CreateModel()
	{
		var model = new TwFixedPurchaseInvoiceModel
		{
			FixedPurchaseId = this.FixedPurchaseId,
			FixedPurchaseShippingNo = int.Parse(this.FixedPurchaseShippingNo),
			TwUniformInvoice = this.TwUniformInvoice,
			TwUniformInvoiceOption1 = this.TwUniformInvoiceOption1,
			TwUniformInvoiceOption2 = this.TwUniformInvoiceOption2,
			TwCarryType = this.TwCarryType,
			TwCarryTypeOption = this.TwCarryTypeOption,
		};

		return model;
	}
	#endregion

	#region プロパティ
	/// <summary>定期購入ID</summary>
	public string FixedPurchaseId
	{
		get { return (string)this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_FIXED_PURCHASE_ID]; }
		set { this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_FIXED_PURCHASE_ID] = value; }
	}
	/// <summary>定期購入電子発票枝番</summary>
	public string FixedPurchaseShippingNo
	{
		get { return (string)this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_FIXED_PURCHASE_SHIPPING_NO]; }
		set { this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_FIXED_PURCHASE_SHIPPING_NO] = value; }
	}
	/// <summary>電子発票種別</summary>
	public string TwUniformInvoice
	{
		get { return (string)this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE]; }
		set { this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE] = value; }
	}
	/// <summary>電子発票項目1</summary>
	public string TwUniformInvoiceOption1
	{
		get { return (string)this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE_OPTION1]; }
		set { this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE_OPTION1] = value; }
	}
	/// <summary>電子発票項目2</summary>
	public string TwUniformInvoiceOption2
	{
		get { return (string)this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE_OPTION2]; }
		set { this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE_OPTION2] = value; }
	}
	/// <summary>載具種別</summary>
	public string TwCarryType
	{
		get { return (string)this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_CARRY_TYPE]; }
		set { this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_CARRY_TYPE] = value; }
	}
	/// <summary>載具項目</summary>
	public string TwCarryTypeOption
	{
		get { return (string)this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_CARRY_TYPE_OPTION]; }
		set { this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_CARRY_TYPE_OPTION] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_DATE_CHANGED] = value; }
	}
	#endregion
}
