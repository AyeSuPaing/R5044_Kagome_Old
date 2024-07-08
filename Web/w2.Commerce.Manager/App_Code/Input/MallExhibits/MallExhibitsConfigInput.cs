/*
=========================================================================================================
  Module      : モール出品設定入力クラス (MallExhibitsConfigInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using w2.App.Common.Input;
using w2.Domain.MallExhibitsConfig;

/// <summary>
/// モール出品設定入力クラス
/// </summary>
[Serializable]
public class MallExhibitsConfigInput : InputBase<MallExhibitsConfigModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public MallExhibitsConfigInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public MallExhibitsConfigInput(MallExhibitsConfigModel model)
		: this()
	{
		this.ShopId = model.ShopId;
		this.ProductId = model.ProductId;
		this.ExhibitsFlg1 = model.ExhibitsFlg1;
		this.ExhibitsFlg2 = model.ExhibitsFlg2;
		this.ExhibitsFlg3 = model.ExhibitsFlg3;
		this.ExhibitsFlg4 = model.ExhibitsFlg4;
		this.ExhibitsFlg5 = model.ExhibitsFlg5;
		this.ExhibitsFlg6 = model.ExhibitsFlg6;
		this.ExhibitsFlg7 = model.ExhibitsFlg7;
		this.ExhibitsFlg8 = model.ExhibitsFlg8;
		this.ExhibitsFlg9 = model.ExhibitsFlg9;
		this.ExhibitsFlg10 = model.ExhibitsFlg10;
		this.ExhibitsFlg11 = model.ExhibitsFlg11;
		this.ExhibitsFlg12 = model.ExhibitsFlg12;
		this.ExhibitsFlg13 = model.ExhibitsFlg13;
		this.ExhibitsFlg14 = model.ExhibitsFlg14;
		this.ExhibitsFlg15 = model.ExhibitsFlg15;
		this.ExhibitsFlg16 = model.ExhibitsFlg16;
		this.ExhibitsFlg17 = model.ExhibitsFlg17;
		this.ExhibitsFlg18 = model.ExhibitsFlg18;
		this.ExhibitsFlg19 = model.ExhibitsFlg19;
		this.ExhibitsFlg20 = model.ExhibitsFlg20;
		this.LastChanged = model.LastChanged;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override MallExhibitsConfigModel CreateModel()
	{
		var model = new MallExhibitsConfigModel
		{
			ShopId = StringUtility.ToEmpty(this.ShopId),
			ProductId = StringUtility.ToEmpty(this.ProductId),
			ExhibitsFlg1 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg1),
			ExhibitsFlg2 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg2),
			ExhibitsFlg3 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg3),
			ExhibitsFlg4 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg4),
			ExhibitsFlg5 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg5),
			ExhibitsFlg6 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg6),
			ExhibitsFlg7 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg7),
			ExhibitsFlg8 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg8),
			ExhibitsFlg9 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg9),
			ExhibitsFlg10 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg10),
			ExhibitsFlg11 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg11),
			ExhibitsFlg12 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg12),
			ExhibitsFlg13 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg13),
			ExhibitsFlg14 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg14),
			ExhibitsFlg15 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg15),
			ExhibitsFlg16 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg16),
			ExhibitsFlg17 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg17),
			ExhibitsFlg18 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg18),
			ExhibitsFlg19 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg19),
			ExhibitsFlg20 = EnsureExhibitsFlgDefaultValue(this.ExhibitsFlg20),
			LastChanged = StringUtility.ToEmpty(this.LastChanged),
			DateChanged = DateTime.Now
		};
		return model;
	}

	/// <summary>
	/// Ensure exhibits flag default value
	/// </summary>
	/// <param name="data">Data</param>
	/// <returns>A value string</returns>
	private string EnsureExhibitsFlgDefaultValue(string data)
	{
		if (string.IsNullOrEmpty(data)) return Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON;
		return StringUtility.ToEmpty(data);
	}

	/// <summary>
	/// Get exhibits flag
	/// </summary>
	/// <param name="index">An index</param>
	/// <returns>An exhibits flag value</returns>
	public string GetExhibitsFlg(int index)
	{
		return (string)this.DataSource[string.Format("exhibits_flg{0}", index)];
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_SHOP_ID)]
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_SHOP_ID] = value; }
	}
	/// <summary>商品ID</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_PRODUCT_ID)]
	public string ProductId
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_PRODUCT_ID]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_PRODUCT_ID] = value; }
	}
	/// <summary>出品FLG1</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG1)]
	public string ExhibitsFlg1
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG1]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG1] = value; }
	}
	/// <summary>出品FLG2</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG2)]
	public string ExhibitsFlg2
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG2]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG2] = value; }
	}
	/// <summary>出品FLG3</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG3)]
	public string ExhibitsFlg3
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG3]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG3] = value; }
	}
	/// <summary>出品FLG4</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG4)]
	public string ExhibitsFlg4
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG4]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG4] = value; }
	}
	/// <summary>出品FLG5</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG5)]
	public string ExhibitsFlg5
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG5]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG5] = value; }
	}
	/// <summary>出品FLG6</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG6)]
	public string ExhibitsFlg6
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG6]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG6] = value; }
	}
	/// <summary>出品FLG7</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG7)]
	public string ExhibitsFlg7
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG7]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG7] = value; }
	}
	/// <summary>出品FLG8</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG8)]
	public string ExhibitsFlg8
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG8]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG8] = value; }
	}
	/// <summary>出品FLG9</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG9)]
	public string ExhibitsFlg9
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG9]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG9] = value; }
	}
	/// <summary>出品FLG10</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG10)]
	public string ExhibitsFlg10
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG10]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG10] = value; }
	}
	/// <summary>出品FLG11</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG11)]
	public string ExhibitsFlg11
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG11]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG11] = value; }
	}
	/// <summary>出品FLG12</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG12)]
	public string ExhibitsFlg12
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG12]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG12] = value; }
	}
	/// <summary>出品FLG13</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG13)]
	public string ExhibitsFlg13
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG13]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG13] = value; }
	}
	/// <summary>出品FLG14</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG14)]
	public string ExhibitsFlg14
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG14]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG14] = value; }
	}
	/// <summary>出品FLG15</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG15)]
	public string ExhibitsFlg15
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG15]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG15] = value; }
	}
	/// <summary>出品FLG16</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG16)]
	public string ExhibitsFlg16
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG16]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG16] = value; }
	}
	/// <summary>出品FLG17</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG17)]
	public string ExhibitsFlg17
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG17]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG17] = value; }
	}
	/// <summary>出品FLG18</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG18)]
	public string ExhibitsFlg18
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG18]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG18] = value; }
	}
	/// <summary>出品FLG19</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG19)]
	public string ExhibitsFlg19
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG19]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG19] = value; }
	}
	/// <summary>出品FLG20</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG20)]
	public string ExhibitsFlg20
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG20]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG20] = value; }
	}
	/// <summary>最終更新者</summary>
	[JsonProperty(Constants.FIELD_MALLEXHIBITSCONFIG_LAST_CHANGED)]
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_LAST_CHANGED] = value; }
	}
	#endregion
}