/*
=========================================================================================================
  Module      : 電子発票情報入力クラス (TwInvoiceInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Input;
using w2.Domain.TwInvoice;

/// <summary>
/// 電子発票情報入力クラス
/// </summary>
public class TwInvoiceInput : InputBase<TwInvoiceModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public TwInvoiceInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public TwInvoiceInput(TwInvoiceModel model)
		: this()
	{
		this.TwInvoiceId = model.TwInvoiceId;
		this.TwInvoiceDateStart = model.TwInvoiceDateStart.ToString();
		this.TwInvoiceDateEnd = model.TwInvoiceDateEnd.ToString();
		this.TwInvoiceCode = model.TwInvoiceCode;
		this.TwInvoiceTypeName = model.TwInvoiceTypeName;
		this.TwInvoiceCodeName = model.TwInvoiceCodeName;
		this.TwInvoiceNoStart = model.TwInvoiceNoStart.ToString();
		this.TwInvoiceNoEnd = model.TwInvoiceNoEnd.ToString();
		this.TwInvoiceNo = (model.TwInvoiceNo != null) ? model.TwInvoiceNo.ToString() : null;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override TwInvoiceModel CreateModel()
	{
		var model = new TwInvoiceModel
		{
			TwInvoiceId = this.TwInvoiceId,
			TwInvoiceDateStart = DateTime.Parse(this.TwInvoiceDateStart),
			TwInvoiceDateEnd = DateTime.Parse(this.TwInvoiceDateEnd),
			TwInvoiceCode = this.TwInvoiceCode,
			TwInvoiceTypeName = this.TwInvoiceTypeName,
			TwInvoiceCodeName = this.TwInvoiceCodeName,
			TwInvoiceNoStart = decimal.Parse(this.TwInvoiceNoStart),
			TwInvoiceNoEnd = decimal.Parse(this.TwInvoiceNoEnd),
			TwInvoiceNo = (this.TwInvoiceNo != null) ? decimal.Parse(this.TwInvoiceNo) : (decimal?)null,
		};

		return model;
	}
	#endregion

	#region プロパティ
	/// <summary>電子発票管理Id</summary>
	public string TwInvoiceId
	{
		get { return (string)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_ID]; }
		set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_ID] = value; }
	}
	/// <summary>発番開始日</summary>
	public string TwInvoiceDateStart
	{
		get { return (string)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_DATE_START]; }
		set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_DATE_START] = value; }
	}
	/// <summary>発番終了日</summary>
	public string TwInvoiceDateEnd
	{
		get { return (string)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_DATE_END]; }
		set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_DATE_END] = value; }
	}
	/// <summary>発票種別コード</summary>
	public string TwInvoiceCode
	{
		get { return (string)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_CODE]; }
		set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_CODE] = value; }
	}
	/// <summary>発票種別名</summary>
	public string TwInvoiceTypeName
	{
		get { return (string)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_TYPE_NAME]; }
		set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_TYPE_NAME] = value; }
	}
	/// <summary>発票種別コード名</summary>
	public string TwInvoiceCodeName
	{
		get { return (string)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_CODE_NAME]; }
		set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_CODE_NAME] = value; }
	}
	/// <summary>発票開始番号</summary>
	public string TwInvoiceNoStart
	{
		get { return (string)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_NO_START]; }
		set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_NO_START] = value; }
	}
	/// <summary>発票終了番号</summary>
	public string TwInvoiceNoEnd
	{
		get { return (string)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_NO_END]; }
		set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_NO_END] = value; }
	}
	/// <summary>最終発票番号</summary>
	public string TwInvoiceNo
	{
		get { return (string)this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_NO]; }
		set { this.DataSource[Constants.FIELD_TWINVOICE_TW_INVOICE_NO] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_TWINVOICE_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_TWINVOICE_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_TWINVOICE_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_TWINVOICE_DATE_CHANGED] = value; }
	}
	#endregion
}
