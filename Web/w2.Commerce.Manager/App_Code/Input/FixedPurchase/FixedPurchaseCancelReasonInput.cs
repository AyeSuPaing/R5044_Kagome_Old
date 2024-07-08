/*
=========================================================================================================
  Module      : 定期解約理由区分設定入力クラス (FixedPurchaseCancelReasonInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Input;
using w2.Domain.FixedPurchase;
using w2.Domain.NameTranslationSetting;

/// <summary>
/// 定期解約理由区分設定入力クラス
/// </summary>
public class FixedPurchaseCancelReasonInput : InputBase<FixedPurchaseCancelReasonModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public FixedPurchaseCancelReasonInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public FixedPurchaseCancelReasonInput(FixedPurchaseCancelReasonModel model)
		: this()
	{
		this.CancelReasonId = model.CancelReasonId;
		this.CancelReasonName = model.CancelReasonName;
		this.DisplayOrder = model.DisplayOrder.ToString();
		this.DisplayKbn = model.DisplayKbn;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
		this.LastChanged = model.LastChanged;
		this.IsNew = false;
		this.IsDelete = false;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override FixedPurchaseCancelReasonModel CreateModel()
	{
		var model = new FixedPurchaseCancelReasonModel
		{
			CancelReasonId = this.CancelReasonId,
			CancelReasonName = this.CancelReasonName,
			DisplayOrder = int.Parse(this.DisplayOrder),
			DisplayKbn = this.DisplayKbn,
			LastChanged = this.LastChanged,
			DateCreated = DateTime.Parse(this.DateCreated),
			DateChanged = DateTime.Parse(this.DateChanged)
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string Validate()
	{
		 var errorMessage = Validator.Validate("FixedPurchaseCancelReason", this.DataSource).Replace("@@ 1 @@",  this.No.ToString());
		 return errorMessage;
	}
	#endregion

	#region プロパティ
	/// <summary>解約理由区分ID</summary>
	public string CancelReasonId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_CANCEL_REASON_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_CANCEL_REASON_ID] = value; }
	}
	/// <summary>解約理由区分名</summary>
	public string CancelReasonName
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_CANCEL_REASON_NAME]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_CANCEL_REASON_NAME] = value; }
	}
	/// <summary>表示順</summary>
	public string DisplayOrder
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_DISPLAY_ORDER]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_DISPLAY_ORDER] = value; }
	}
	/// <summary>表示区分</summary>
	public string DisplayKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_DISPLAY_KBN]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_DISPLAY_KBN] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_DATE_CHANGED] = value; }
	}
	/// <summary>更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASECANCELREASON_LAST_CHANGED] = value; }
	}
	/// <summary>No</summary>
	public int No { get; set; }
	/// <summary>新規か</summary>
	public bool IsNew { get; set;}
	/// <summary>削除か</summary>
	public bool IsDelete { get; set; }
	/// <summary>表示区分：PC利用</summary>
	public bool IsValidDisplayKbnPc
	{
		get
		{
			foreach (var value in this.DisplayKbn.Split(','))
			{
				if (value == FixedPurchaseCancelReasonModel.DisplayKbnValue.PC.ToString()) return true;
			}
			return false;
		}
	}
	/// <summary>表示区分：EC利用</summary>
	public bool IsValidDisplayKbnEc
	{
		get
		{
			foreach (var value in this.DisplayKbn.Split(','))
			{
				if (value == FixedPurchaseCancelReasonModel.DisplayKbnValue.EC.ToString()) return true;
			}
			return false;
		}
	}
	/// <summary>解約理由区分翻訳設定情報</summary>
	public NameTranslationSettingModel[] CancelReasonNameTranslationData
	{
		get { return (NameTranslationSettingModel[])this.DataSource["cancel_reason_name_translation_data"]; }
		set { this.DataSource["cancel_reason_name_translation_data"] = value; }
	}
	#endregion
}