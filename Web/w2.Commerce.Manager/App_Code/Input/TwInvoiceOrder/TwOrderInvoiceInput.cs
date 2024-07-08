/*
=========================================================================================================
  Module      : 注文電子発票情報入力クラス (TwOrderInvoiceInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Input;
using w2.Domain.TwOrderInvoice;

/// <summary>
/// 注文電子発票情報入力クラス
/// </summary>
[Serializable]
public class TwOrderInvoiceInput : InputBase<TwOrderInvoiceModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public TwOrderInvoiceInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public TwOrderInvoiceInput(TwOrderInvoiceModel model)
		: this()
	{
		this.OrderId = model.OrderId;
		this.OrderShippingNo = model.OrderShippingNo.ToString();
		this.TwUniformInvoice = model.TwUniformInvoice;
		this.TwUniformInvoiceOption1 = model.TwUniformInvoiceOption1;
		this.TwUniformInvoiceOption2 = model.TwUniformInvoiceOption2;
		this.TwCarryType = model.TwCarryType;
		this.TwCarryTypeOption = model.TwCarryTypeOption;
		this.TwInvoiceNo = model.TwInvoiceNo;
		this.TwInvoiceDate = (model.TwInvoiceDate != null) ? model.TwInvoiceDate.ToString() : null;
		this.TwInvoiceStatus = model.TwInvoiceStatus;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override TwOrderInvoiceModel CreateModel()
	{
		var model = new TwOrderInvoiceModel
		{
			OrderId = this.OrderId,
			OrderShippingNo = int.Parse(this.OrderShippingNo),
			TwUniformInvoice = this.TwUniformInvoice,
			TwUniformInvoiceOption1 = this.TwUniformInvoiceOption1,
			TwUniformInvoiceOption2 = this.TwUniformInvoiceOption2,
			TwCarryType = this.TwCarryType,
			TwCarryTypeOption = this.TwCarryTypeOption,
			TwInvoiceNo = this.TwInvoiceNo,
			TwInvoiceDate = (this.TwInvoiceDate != null) ? DateTime.Parse(this.TwInvoiceDate) : (DateTime?)null,
			TwInvoiceStatus = this.TwInvoiceStatus,
		};

		return model;
	}

	/// <summary>
	/// Update
	/// </summary>
	/// <param name="twOrderInvoice">twOrderInvoice</param>
	public void Update(TwOrderInvoiceInput twOrderInvoice)
	{
		// TwOrderInvoice
		this.UpdateTwOrderInvoice(twOrderInvoice);
	}

	/// <summary>
	/// Update TwOrderInvoice
	/// </summary>
	/// <param name="twOrderInvoice">twOrderInvoice</param>
	private void UpdateTwOrderInvoice(TwOrderInvoiceInput twOrderInvoice)
	{
		this.TwCarryTypeOption = twOrderInvoice.TwCarryTypeOption;
		this.TwUniformInvoice = twOrderInvoice.TwUniformInvoice;
		this.TwUniformInvoiceOption1 = twOrderInvoice.TwUniformInvoiceOption1;
		this.TwUniformInvoiceOption2 = twOrderInvoice.TwUniformInvoiceOption2;
		this.TwCarryType = twOrderInvoice.TwCarryType;
	}
	#endregion

	#region プロパティ
	/// <summary>注文ID</summary>
	public string OrderId
	{
		get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_ORDER_ID]; }
		set { this.DataSource[Constants.FIELD_TWORDERINVOICE_ORDER_ID] = value; }
	}
	/// <summary>電子発票枝番</summary>
	public string OrderShippingNo
	{
		get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_ORDER_SHIPPING_NO]; }
		set { this.DataSource[Constants.FIELD_TWORDERINVOICE_ORDER_SHIPPING_NO] = value; }
	}
	/// <summary>電子発票種別</summary>
	public string TwUniformInvoice
	{
		get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE]; }
		set { this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE] = value; }
	}
	/// <summary>電子発票項目1</summary>
	public string TwUniformInvoiceOption1
	{
		get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION1]; }
		set { this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION1] = value; }
	}
	/// <summary>電子発票項目2</summary>
	public string TwUniformInvoiceOption2
	{
		get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION2]; }
		set { this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION2] = value; }
	}
	/// <summary>載具種別</summary>
	public string TwCarryType
	{
		get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE]; }
		set { this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE] = value; }
	}
	/// <summary>載具項目</summary>
	public string TwCarryTypeOption
	{
		get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE_OPTION]; }
		set { this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE_OPTION] = value; }
	}
	/// <summary>発票番号</summary>
	public string TwInvoiceNo
	{
		get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_NO]; }
		set { this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_NO] = value; }
	}
	/// <summary>発票日時</summary>
	public string TwInvoiceDate
	{
		get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_DATE]; }
		set { this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_DATE] = value; }
	}
	/// <summary>発票ステータス</summary>
	public string TwInvoiceStatus
	{
		get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS]; }
		set { this.DataSource[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_TWORDERINVOICE_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_TWORDERINVOICE_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_TWORDERINVOICE_DATE_CHANGED] = value; }
	}
	#endregion
}
