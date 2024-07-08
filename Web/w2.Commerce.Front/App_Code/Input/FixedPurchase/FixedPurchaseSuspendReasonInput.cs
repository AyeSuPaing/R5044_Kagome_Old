/*
=========================================================================================================
  Module      : 定期購入情報休止理由入力クラス (FixedPurchaseSuspendReasonInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.App.Common;

/// <summary>
/// 定期購入情報休止理由入力クラス
/// </summary>
[Serializable]
public class FixedPurchaseSuspendReasonInput
{
	#region コンストラクタ
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public FixedPurchaseSuspendReasonInput()
	{
		this.DataSource = new Hashtable();
	}
	#endregion

	#region メソッド
	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public Dictionary<string, string> Validate()
	{
		return Validator.ValidateAndGetErrorContainer("FixedPurchaseModifyInput", this.DataSource);
	}
	#endregion

	#region プロパティ
	/// <summary>データソース</summary>
	public Hashtable DataSource { get; set; }
	/// <summary>定期再開予定日(年)</summary>
	public string ResumeDateYear
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_RESUME_DATE + "_year"]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_RESUME_DATE + "_year"] = value; }
	}
	/// <summary>定期再開予定日(月)</summary>
	public string ResumeDateMonth
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_RESUME_DATE + "_month"]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_RESUME_DATE + "_month"] = value; }
	}
	/// <summary>定期再開予定日(日)</summary>
	public string ResumeDateDay
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_RESUME_DATE + "_day"]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_RESUME_DATE + "_day"] = value; }
	}
	/// <summary>定期再開予定日</summary>
	public string ResumeDate
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_RESUME_DATE]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_RESUME_DATE] = value; }
	}
	/// <summary>次回配送日(年)</summary>
	public string NextShippingDateYear
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE + "_year"]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE + "_year"] = value; }
	}
	/// <summary>次回配送日(月)</summary>
	public string NextShippingDateMonth
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE + "_month"]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE + "_month"] = value; }
	}
	/// <summary>次回配送日(日)</summary>
	public string NextShippingDateDay
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE + "_day"]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE + "_day"] = value; }
	}
	/// <summary>次回配送日</summary>
	public string NextShippingDate
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE] = value; }
	}
	/// <summary>休止理由</summary>
	public string SuspendReason
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_SUSPEND_REASON]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_SUSPEND_REASON] = value; }
	}
	#endregion
}