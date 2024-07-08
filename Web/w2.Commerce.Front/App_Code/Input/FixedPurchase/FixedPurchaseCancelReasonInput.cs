/*
=========================================================================================================
  Module      : 定期購入情報解約理由入力クラス (FixedPurchaseCancelReasonInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 定期購入情報解約理由入力クラス
/// </summary>
[Serializable]
public class FixedPurchaseCancelReasonInput
{
	/// <summary>データソース</summary>
	public Hashtable DataSource { get; set; }

	#region コンストラクタ
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public FixedPurchaseCancelReasonInput()
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
	/// <summary>解約理由区分ID</summary>
	public string CancelReasonId
	{
		get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_FIXEDPURCHASE_CANCEL_REASON_ID]); }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_CANCEL_REASON_ID] = value; }
	}
	/// <summary>解約理由区分名</summary>
	public string CancelReasonName
	{
		get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_CANCEL_REASON_NAME]); }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_CANCEL_REASON_NAME] = value; }
	}
	/// <summary>解約メモ</summary>
	public string CancelMemo
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_CANCEL_MEMO]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_CANCEL_MEMO] = value; }
	}
	#endregion
}