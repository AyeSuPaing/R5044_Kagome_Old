/*
=========================================================================================================
  Module      : 商品一覧表示設定入力クラス (ProductListDispSettingInput.cs)
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
using w2.Domain.ProductListDispSetting;

/// <summary>
/// 商品一覧表示設定マスタ入力クラス
/// </summary>
public class ProductListDispSettingInput : InputBase<ProductListDispSettingModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public ProductListDispSettingInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public ProductListDispSettingInput(ProductListDispSettingModel model)
		: this()
	{
		this.SettingId = model.SettingId;
		this.SettingName = model.SettingName;
		this.DispEnable = model.DispEnable;
		this.DispNo = model.DispNo.ToString();
		this.DefaultDispFlg = model.DefaultDispFlg;
		this.Description = model.Description;
		this.DateChanged = model.DateChanged.ToString();
		this.LastChanged = model.LastChanged;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override ProductListDispSettingModel CreateModel()
	{
		var model = new ProductListDispSettingModel
		{
			SettingId = this.SettingId,
			SettingName = this.SettingName,
			DispEnable = this.DispEnable,
			DispNo = int.Parse(this.DispNo),
			DefaultDispFlg = this.DefaultDispFlg,
			Description = this.Description,
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
		var errorMessage = Validator.Validate("ProductListDispSetting", this.DataSource);
		return errorMessage;
	}
	#endregion

	#region プロパティ
	/// <summary>設定ID</summary>
	public string SettingId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_ID] = value; }
	}
	/// <summary>表示名</summary>
	public string SettingName
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_NAME]; }
		set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_SETTING_NAME] = value; }
	}
	/// <summary>表示／非表示</summary>
	public string DispEnable
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DISP_ENABLE]; }
		set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DISP_ENABLE] = value; }
	}
	/// <summary>表示順</summary>
	public string DispNo
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DISP_NO]; }
		set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DISP_NO] = value; }
	}
	/// <summary>デフォルト表示フラグ</summary>
	public string DefaultDispFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DEFAULT_DISP_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DEFAULT_DISP_FLG] = value; }
	}
	/// <summary>説明</summary>
	public string Description
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DESCRIPTION]; }
		set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DESCRIPTION] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_PRODUCTLISTDISPSETTING_LAST_CHANGED] = value; }
	}
	#endregion
}
