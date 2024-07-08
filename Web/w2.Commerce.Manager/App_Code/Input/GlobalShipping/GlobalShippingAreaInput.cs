/*
=========================================================================================================
  Module      : 海外配送エリア入力クラス (GlobalShippingAreaInput.cs)
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
using w2.Domain.GlobalShipping;

/// <summary>
/// 海外配送エリア入力クラス
/// </summary>
[Serializable]
public class GlobalShippingAreaInput : InputBase<GlobalShippingAreaModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public GlobalShippingAreaInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public GlobalShippingAreaInput(GlobalShippingAreaModel model)
		: this()
	{
		this.GlobalShippingAreaId = model.GlobalShippingAreaId;
		this.GlobalShippingAreaName = model.GlobalShippingAreaName;
		this.SortNo = model.SortNo.ToString();
		this.ValidFlg = model.ValidFlg;
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
	public override GlobalShippingAreaModel CreateModel()
	{
		var model = new GlobalShippingAreaModel
		{
			GlobalShippingAreaId = this.GlobalShippingAreaId,
			GlobalShippingAreaName = this.GlobalShippingAreaName,
			SortNo = int.Parse(this.SortNo),
			ValidFlg = this.ValidFlg,
			LastChanged = this.LastChanged,
		};
		return model;
	}

	/// <summary>
	/// 登録検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string ValidateRegister()
	{
		var errorMessage = Validator.Validate("GlobalShippingAreaRegister", this.DataSource);
		return errorMessage;
	}

	/// <summary>
	/// 更新検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string ValidateUpdate()
	{
		var errorMessage = Validator.Validate("GlobalShippingAreaUpdate", this.DataSource);
		return errorMessage;
	}

	#endregion

	#region プロパティ
	/// <summary>海外配送エリアID</summary>
	public string GlobalShippingAreaId
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_GLOBAL_SHIPPING_AREA_ID]; }
		set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_GLOBAL_SHIPPING_AREA_ID] = value; }
	}
	/// <summary>海外配送エリア名</summary>
	public string GlobalShippingAreaName
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_GLOBAL_SHIPPING_AREA_NAME]; }
		set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_GLOBAL_SHIPPING_AREA_NAME] = value; }
	}
	/// <summary>表示順</summary>
	public string SortNo
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_SORT_NO]; }
		set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_SORT_NO] = value; }
	}
	/// <summary>有効フラグ</summary>
	public string ValidFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_VALID_FLG]; }
		set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_VALID_FLG] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_GLOBALSHIPPINGAREA_LAST_CHANGED] = value; }
	}
	#endregion
}
