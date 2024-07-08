/*
=========================================================================================================
  Module      : 商品拡張拡張項目入力クラス (ProductExtendInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using w2.App.Common.Input;
using w2.Domain.ProductExtend;

/// <summary>
/// 商品拡張拡張項目入力クラス
/// </summary>
[Serializable]
public class ProductExtendInput :  InputBase<ProductExtendModel>
{
	/// <summary>Extend name base</summary>
	public const string EXTEND_NAME_BASE = "name_";

	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public ProductExtendInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public ProductExtendInput(ProductExtendModel model)
		: this()
	{
		this.ShopId = model.ShopId;
		this.ProductId = model.ProductId;
		this.Extend1 = model.Extend1;
		this.Extend2 = model.Extend2;
		this.Extend3 = model.Extend3;
		this.Extend4 = model.Extend4;
		this.Extend5 = model.Extend5;
		this.Extend6 = model.Extend6;
		this.Extend7 = model.Extend7;
		this.Extend8 = model.Extend8;
		this.Extend9 = model.Extend9;
		this.Extend10 = model.Extend10;
		this.Extend11 = model.Extend11;
		this.Extend12 = model.Extend12;
		this.Extend13 = model.Extend13;
		this.Extend14 = model.Extend14;
		this.Extend15 = model.Extend15;
		this.Extend16 = model.Extend16;
		this.Extend17 = model.Extend17;
		this.Extend18 = model.Extend18;
		this.Extend19 = model.Extend19;
		this.Extend20 = model.Extend20;
		this.Extend21 = model.Extend21;
		this.Extend22 = model.Extend22;
		this.Extend23 = model.Extend23;
		this.Extend24 = model.Extend24;
		this.Extend25 = model.Extend25;
		this.Extend26 = model.Extend26;
		this.Extend27 = model.Extend27;
		this.Extend28 = model.Extend28;
		this.Extend29 = model.Extend29;
		this.Extend30 = model.Extend30;
		this.Extend31 = model.Extend31;
		this.Extend32 = model.Extend32;
		this.Extend33 = model.Extend33;
		this.Extend34 = model.Extend34;
		this.Extend35 = model.Extend35;
		this.Extend36 = model.Extend36;
		this.Extend37 = model.Extend37;
		this.Extend38 = model.Extend38;
		this.Extend39 = model.Extend39;
		this.Extend40 = model.Extend40;
		this.Extend41 = model.Extend41;
		this.Extend42 = model.Extend42;
		this.Extend43 = model.Extend43;
		this.Extend44 = model.Extend44;
		this.Extend45 = model.Extend45;
		this.Extend46 = model.Extend46;
		this.Extend47 = model.Extend47;
		this.Extend48 = model.Extend48;
		this.Extend49 = model.Extend49;
		this.Extend50 = model.Extend50;
		this.Extend51 = model.Extend51;
		this.Extend52 = model.Extend52;
		this.Extend53 = model.Extend53;
		this.Extend54 = model.Extend54;
		this.Extend55 = model.Extend55;
		this.Extend56 = model.Extend56;
		this.Extend57 = model.Extend57;
		this.Extend58 = model.Extend58;
		this.Extend59 = model.Extend59;
		this.Extend60 = model.Extend60;
		this.Extend61 = model.Extend61;
		this.Extend62 = model.Extend62;
		this.Extend63 = model.Extend63;
		this.Extend64 = model.Extend64;
		this.Extend65 = model.Extend65;
		this.Extend66 = model.Extend66;
		this.Extend67 = model.Extend67;
		this.Extend68 = model.Extend68;
		this.Extend69 = model.Extend69;
		this.Extend70 = model.Extend70;
		this.Extend71 = model.Extend71;
		this.Extend72 = model.Extend72;
		this.Extend73 = model.Extend73;
		this.Extend74 = model.Extend74;
		this.Extend75 = model.Extend75;
		this.Extend76 = model.Extend76;
		this.Extend77 = model.Extend77;
		this.Extend78 = model.Extend78;
		this.Extend79 = model.Extend79;
		this.Extend80 = model.Extend80;
		this.Extend81 = model.Extend81;
		this.Extend82 = model.Extend82;
		this.Extend83 = model.Extend83;
		this.Extend84 = model.Extend84;
		this.Extend85 = model.Extend85;
		this.Extend86 = model.Extend86;
		this.Extend87 = model.Extend87;
		this.Extend88 = model.Extend88;
		this.Extend89 = model.Extend89;
		this.Extend90 = model.Extend90;
		this.Extend91 = model.Extend91;
		this.Extend92 = model.Extend92;
		this.Extend93 = model.Extend93;
		this.Extend94 = model.Extend94;
		this.Extend95 = model.Extend95;
		this.Extend96 = model.Extend96;
		this.Extend97 = model.Extend97;
		this.Extend98 = model.Extend98;
		this.Extend99 = model.Extend99;
		this.Extend100 = model.Extend100;
		this.Extend101 = model.Extend101;
		this.Extend102 = model.Extend102;
		this.Extend103 = model.Extend103;
		this.Extend104 = model.Extend104;
		this.Extend105 = model.Extend105;
		this.Extend106 = model.Extend106;
		this.Extend107 = model.Extend107;
		this.Extend108 = model.Extend108;
		this.Extend109 = model.Extend109;
		this.Extend110 = model.Extend110;
		this.Extend111 = model.Extend111;
		this.Extend112 = model.Extend112;
		this.Extend113 = model.Extend113;
		this.Extend114 = model.Extend114;
		this.Extend115 = model.Extend115;
		this.Extend116 = model.Extend116;
		this.Extend117 = model.Extend117;
		this.Extend118 = model.Extend118;
		this.Extend119 = model.Extend119;
		this.Extend120 = model.Extend120;
		this.Extend121 = model.Extend121;
		this.Extend122 = model.Extend122;
		this.Extend123 = model.Extend123;
		this.Extend124 = model.Extend124;
		this.Extend125 = model.Extend125;
		this.Extend126 = model.Extend126;
		this.Extend127 = model.Extend127;
		this.Extend128 = model.Extend128;
		this.Extend129 = model.Extend129;
		this.Extend130 = model.Extend130;
		this.Extend131 = model.Extend131;
		this.Extend132 = model.Extend132;
		this.Extend133 = model.Extend133;
		this.Extend134 = model.Extend134;
		this.Extend135 = model.Extend135;
		this.Extend136 = model.Extend136;
		this.Extend137 = model.Extend137;
		this.Extend138 = model.Extend138;
		this.Extend139 = model.Extend139;
		this.Extend140 = model.Extend140;
		this.DelFlg = model.DelFlg;
		this.LastChanged = model.LastChanged;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override ProductExtendModel CreateModel()
	{
		var model = new ProductExtendModel
		{
			ShopId = StringUtility.ToEmpty(this.ShopId),
			ProductId = StringUtility.ToEmpty(this.ProductId),
			Extend1 = StringUtility.ToEmpty(this.Extend1),
			Extend2 = StringUtility.ToEmpty(this.Extend2),
			Extend3 = StringUtility.ToEmpty(this.Extend3),
			Extend4 = StringUtility.ToEmpty(this.Extend4),
			Extend5 = StringUtility.ToEmpty(this.Extend5),
			Extend6 = StringUtility.ToEmpty(this.Extend6),
			Extend7 = StringUtility.ToEmpty(this.Extend7),
			Extend8 = StringUtility.ToEmpty(this.Extend8),
			Extend9 = StringUtility.ToEmpty(this.Extend9),
			Extend10 = StringUtility.ToEmpty(this.Extend10),
			Extend11 = StringUtility.ToEmpty(this.Extend11),
			Extend12 = StringUtility.ToEmpty(this.Extend12),
			Extend13 = StringUtility.ToEmpty(this.Extend13),
			Extend14 = StringUtility.ToEmpty(this.Extend14),
			Extend15 = StringUtility.ToEmpty(this.Extend15),
			Extend16 = StringUtility.ToEmpty(this.Extend16),
			Extend17 = StringUtility.ToEmpty(this.Extend17),
			Extend18 = StringUtility.ToEmpty(this.Extend18),
			Extend19 = StringUtility.ToEmpty(this.Extend19),
			Extend20 = StringUtility.ToEmpty(this.Extend20),
			Extend21 = StringUtility.ToEmpty(this.Extend21),
			Extend22 = StringUtility.ToEmpty(this.Extend22),
			Extend23 = StringUtility.ToEmpty(this.Extend23),
			Extend24 = StringUtility.ToEmpty(this.Extend24),
			Extend25 = StringUtility.ToEmpty(this.Extend25),
			Extend26 = StringUtility.ToEmpty(this.Extend26),
			Extend27 = StringUtility.ToEmpty(this.Extend27),
			Extend28 = StringUtility.ToEmpty(this.Extend28),
			Extend29 = StringUtility.ToEmpty(this.Extend29),
			Extend30 = StringUtility.ToEmpty(this.Extend30),
			Extend31 = StringUtility.ToEmpty(this.Extend31),
			Extend32 = StringUtility.ToEmpty(this.Extend32),
			Extend33 = StringUtility.ToEmpty(this.Extend33),
			Extend34 = StringUtility.ToEmpty(this.Extend34),
			Extend35 = StringUtility.ToEmpty(this.Extend35),
			Extend36 = StringUtility.ToEmpty(this.Extend36),
			Extend37 = StringUtility.ToEmpty(this.Extend37),
			Extend38 = StringUtility.ToEmpty(this.Extend38),
			Extend39 = StringUtility.ToEmpty(this.Extend39),
			Extend40 = StringUtility.ToEmpty(this.Extend40),
			Extend41 = StringUtility.ToEmpty(this.Extend41),
			Extend42 = StringUtility.ToEmpty(this.Extend42),
			Extend43 = StringUtility.ToEmpty(this.Extend43),
			Extend44 = StringUtility.ToEmpty(this.Extend44),
			Extend45 = StringUtility.ToEmpty(this.Extend45),
			Extend46 = StringUtility.ToEmpty(this.Extend46),
			Extend47 = StringUtility.ToEmpty(this.Extend47),
			Extend48 = StringUtility.ToEmpty(this.Extend48),
			Extend49 = StringUtility.ToEmpty(this.Extend49),
			Extend50 = StringUtility.ToEmpty(this.Extend50),
			Extend51 = StringUtility.ToEmpty(this.Extend51),
			Extend52 = StringUtility.ToEmpty(this.Extend52),
			Extend53 = StringUtility.ToEmpty(this.Extend53),
			Extend54 = StringUtility.ToEmpty(this.Extend54),
			Extend55 = StringUtility.ToEmpty(this.Extend55),
			Extend56 = StringUtility.ToEmpty(this.Extend56),
			Extend57 = StringUtility.ToEmpty(this.Extend57),
			Extend58 = StringUtility.ToEmpty(this.Extend58),
			Extend59 = StringUtility.ToEmpty(this.Extend59),
			Extend60 = StringUtility.ToEmpty(this.Extend60),
			Extend61 = StringUtility.ToEmpty(this.Extend61),
			Extend62 = StringUtility.ToEmpty(this.Extend62),
			Extend63 = StringUtility.ToEmpty(this.Extend63),
			Extend64 = StringUtility.ToEmpty(this.Extend64),
			Extend65 = StringUtility.ToEmpty(this.Extend65),
			Extend66 = StringUtility.ToEmpty(this.Extend66),
			Extend67 = StringUtility.ToEmpty(this.Extend67),
			Extend68 = StringUtility.ToEmpty(this.Extend68),
			Extend69 = StringUtility.ToEmpty(this.Extend69),
			Extend70 = StringUtility.ToEmpty(this.Extend70),
			Extend71 = StringUtility.ToEmpty(this.Extend71),
			Extend72 = StringUtility.ToEmpty(this.Extend72),
			Extend73 = StringUtility.ToEmpty(this.Extend73),
			Extend74 = StringUtility.ToEmpty(this.Extend74),
			Extend75 = StringUtility.ToEmpty(this.Extend75),
			Extend76 = StringUtility.ToEmpty(this.Extend76),
			Extend77 = StringUtility.ToEmpty(this.Extend77),
			Extend78 = StringUtility.ToEmpty(this.Extend78),
			Extend79 = StringUtility.ToEmpty(this.Extend79),
			Extend80 = StringUtility.ToEmpty(this.Extend80),
			Extend81 = StringUtility.ToEmpty(this.Extend81),
			Extend82 = StringUtility.ToEmpty(this.Extend82),
			Extend83 = StringUtility.ToEmpty(this.Extend83),
			Extend84 = StringUtility.ToEmpty(this.Extend84),
			Extend85 = StringUtility.ToEmpty(this.Extend85),
			Extend86 = StringUtility.ToEmpty(this.Extend86),
			Extend87 = StringUtility.ToEmpty(this.Extend87),
			Extend88 = StringUtility.ToEmpty(this.Extend88),
			Extend89 = StringUtility.ToEmpty(this.Extend89),
			Extend90 = StringUtility.ToEmpty(this.Extend90),
			Extend91 = StringUtility.ToEmpty(this.Extend91),
			Extend92 = StringUtility.ToEmpty(this.Extend92),
			Extend93 = StringUtility.ToEmpty(this.Extend93),
			Extend94 = StringUtility.ToEmpty(this.Extend94),
			Extend95 = StringUtility.ToEmpty(this.Extend95),
			Extend96 = StringUtility.ToEmpty(this.Extend96),
			Extend97 = StringUtility.ToEmpty(this.Extend97),
			Extend98 = StringUtility.ToEmpty(this.Extend98),
			Extend99 = StringUtility.ToEmpty(this.Extend99),
			Extend100 = StringUtility.ToEmpty(this.Extend100),
			Extend101 = StringUtility.ToEmpty(this.Extend101),
			Extend102 = StringUtility.ToEmpty(this.Extend102),
			Extend103 = StringUtility.ToEmpty(this.Extend103),
			Extend104 = StringUtility.ToEmpty(this.Extend104),
			Extend105 = StringUtility.ToEmpty(this.Extend105),
			Extend106 = StringUtility.ToEmpty(this.Extend106),
			Extend107 = StringUtility.ToEmpty(this.Extend107),
			Extend108 = StringUtility.ToEmpty(this.Extend108),
			Extend109 = StringUtility.ToEmpty(this.Extend109),
			Extend110 = StringUtility.ToEmpty(this.Extend110),
			Extend111 = StringUtility.ToEmpty(this.Extend111),
			Extend112 = StringUtility.ToEmpty(this.Extend112),
			Extend113 = StringUtility.ToEmpty(this.Extend113),
			Extend114 = StringUtility.ToEmpty(this.Extend114),
			Extend115 = StringUtility.ToEmpty(this.Extend115),
			Extend116 = StringUtility.ToEmpty(this.Extend116),
			Extend117 = StringUtility.ToEmpty(this.Extend117),
			Extend118 = StringUtility.ToEmpty(this.Extend118),
			Extend119 = StringUtility.ToEmpty(this.Extend119),
			Extend120 = StringUtility.ToEmpty(this.Extend120),
			Extend121 = StringUtility.ToEmpty(this.Extend121),
			Extend122 = StringUtility.ToEmpty(this.Extend122),
			Extend123 = StringUtility.ToEmpty(this.Extend123),
			Extend124 = StringUtility.ToEmpty(this.Extend124),
			Extend125 = StringUtility.ToEmpty(this.Extend125),
			Extend126 = StringUtility.ToEmpty(this.Extend126),
			Extend127 = StringUtility.ToEmpty(this.Extend127),
			Extend128 = StringUtility.ToEmpty(this.Extend128),
			Extend129 = StringUtility.ToEmpty(this.Extend129),
			Extend130 = StringUtility.ToEmpty(this.Extend130),
			Extend131 = StringUtility.ToEmpty(this.Extend131),
			Extend132 = StringUtility.ToEmpty(this.Extend132),
			Extend133 = StringUtility.ToEmpty(this.Extend133),
			Extend134 = StringUtility.ToEmpty(this.Extend134),
			Extend135 = StringUtility.ToEmpty(this.Extend135),
			Extend136 = StringUtility.ToEmpty(this.Extend136),
			Extend137 = StringUtility.ToEmpty(this.Extend137),
			Extend138 = StringUtility.ToEmpty(this.Extend138),
			Extend139 = StringUtility.ToEmpty(this.Extend139),
			Extend140 = StringUtility.ToEmpty(this.Extend140),
			DelFlg = StringUtility.ToEmpty(this.DelFlg),
			LastChanged = StringUtility.ToEmpty(this.LastChanged)
		};
		return model;
	}

	/// <summary>
	/// Validate
	/// </summary>
	/// <returns>Error messages as dictionary</returns>
	public Dictionary<string, string> Validate()
	{
		var errorMessagesContainer = Validator.ValidateAndGetErrorContainer("ProductExtendRegist", this.DataSource);
		if (errorMessagesContainer.Count == 0) return errorMessagesContainer;

		var errorKeys = new List<string>(errorMessagesContainer.Keys);
		foreach (var key in errorKeys)
		{
			var extendName = StringUtility.ToEmpty(this.DataSource[EXTEND_NAME_BASE + key]);
			errorMessagesContainer[key] = errorMessagesContainer[key].Replace("@@ 1 @@", extendName);
		}
		return errorMessagesContainer;
	}

	/// <summary>
	/// Get product extend value with data source
	/// </summary>
	/// <param name="fieldName">Field name</param>
	/// <returns>A json string</returns>
	public string GetProductExtendValueWithDataSource(string fieldName)
	{
		var result = (string.IsNullOrEmpty(fieldName) == false)
			? StringUtility.ToEmpty(this.DataSource[fieldName])
			: string.Empty;
		return result;
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_SHOP_ID)]
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_SHOP_ID] = value; }
	}
	/// <summary>商品ID</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_PRODUCT_ID)]
	public string ProductId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_PRODUCT_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_PRODUCT_ID] = value; }
	}
	/// <summary>拡張項目1</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND1)]
	public string Extend1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND1]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND1] = value; }
	}
	/// <summary>拡張項目2</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND2)]
	public string Extend2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND2]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND2] = value; }
	}
	/// <summary>拡張項目3</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND3)]
	public string Extend3
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND3]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND3] = value; }
	}
	/// <summary>拡張項目4</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND4)]
	public string Extend4
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND4]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND4] = value; }
	}
	/// <summary>拡張項目5</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND5)]
	public string Extend5
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND5]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND5] = value; }
	}
	/// <summary>拡張項目6</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND6)]
	public string Extend6
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND6]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND6] = value; }
	}
	/// <summary>拡張項目7</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND7)]
	public string Extend7
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND7]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND7] = value; }
	}
	/// <summary>拡張項目8</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND8)]
	public string Extend8
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND8]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND8] = value; }
	}
	/// <summary>拡張項目9</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND9)]
	public string Extend9
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND9]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND9] = value; }
	}
	/// <summary>拡張項目10</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND10)]
	public string Extend10
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND10]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND10] = value; }
	}
	/// <summary>拡張項目11</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND11)]
	public string Extend11
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND11]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND11] = value; }
	}
	/// <summary>拡張項目12</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND12)]
	public string Extend12
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND12]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND12] = value; }
	}
	/// <summary>拡張項目13</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND13)]
	public string Extend13
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND13]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND13] = value; }
	}
	/// <summary>拡張項目14</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND14)]
	public string Extend14
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND14]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND14] = value; }
	}
	/// <summary>拡張項目15</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND15)]
	public string Extend15
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND15]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND15] = value; }
	}
	/// <summary>拡張項目16</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND16)]
	public string Extend16
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND16]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND16] = value; }
	}
	/// <summary>拡張項目17</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND17)]
	public string Extend17
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND17]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND17] = value; }
	}
	/// <summary>拡張項目18</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND18)]
	public string Extend18
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND18]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND18] = value; }
	}
	/// <summary>拡張項目19</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND19)]
	public string Extend19
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND19]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND19] = value; }
	}
	/// <summary>拡張項目20</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND20)]
	public string Extend20
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND20]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND20] = value; }
	}
	/// <summary>拡張項目21</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND21)]
	public string Extend21
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND21]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND21] = value; }
	}
	/// <summary>拡張項目22</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND22)]
	public string Extend22
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND22]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND22] = value; }
	}
	/// <summary>拡張項目23</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND23)]
	public string Extend23
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND23]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND23] = value; }
	}
	/// <summary>拡張項目24</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND24)]
	public string Extend24
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND24]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND24] = value; }
	}
	/// <summary>拡張項目25</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND25)]
	public string Extend25
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND25]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND25] = value; }
	}
	/// <summary>拡張項目26</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND26)]
	public string Extend26
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND26]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND26] = value; }
	}
	/// <summary>拡張項目27</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND27)]
	public string Extend27
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND27]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND27] = value; }
	}
	/// <summary>拡張項目28</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND28)]
	public string Extend28
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND28]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND28] = value; }
	}
	/// <summary>拡張項目29</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND29)]
	public string Extend29
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND29]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND29] = value; }
	}
	/// <summary>拡張項目30</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND30)]
	public string Extend30
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND30]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND30] = value; }
	}
	/// <summary>拡張項目31</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND31)]
	public string Extend31
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND31]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND31] = value; }
	}
	/// <summary>拡張項目32</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND32)]
	public string Extend32
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND32]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND32] = value; }
	}
	/// <summary>拡張項目33</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND33)]
	public string Extend33
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND33]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND33] = value; }
	}
	/// <summary>拡張項目34</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND34)]
	public string Extend34
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND34]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND34] = value; }
	}
	/// <summary>拡張項目35</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND35)]
	public string Extend35
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND35]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND35] = value; }
	}
	/// <summary>拡張項目36</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND36)]
	public string Extend36
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND36]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND36] = value; }
	}
	/// <summary>拡張項目37</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND37)]
	public string Extend37
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND37]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND37] = value; }
	}
	/// <summary>拡張項目38</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND38)]
	public string Extend38
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND38]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND38] = value; }
	}
	/// <summary>拡張項目39</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND39)]
	public string Extend39
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND39]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND39] = value; }
	}
	/// <summary>拡張項目40</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND40)]
	public string Extend40
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND40]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND40] = value; }
	}
	/// <summary>拡張項目41</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND41)]
	public string Extend41
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND41]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND41] = value; }
	}
	/// <summary>拡張項目42</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND42)]
	public string Extend42
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND42]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND42] = value; }
	}
	/// <summary>拡張項目43</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND43)]
	public string Extend43
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND43]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND43] = value; }
	}
	/// <summary>拡張項目44</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND44)]
	public string Extend44
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND44]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND44] = value; }
	}
	/// <summary>拡張項目45</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND45)]
	public string Extend45
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND45]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND45] = value; }
	}
	/// <summary>拡張項目46</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND46)]
	public string Extend46
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND46]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND46] = value; }
	}
	/// <summary>拡張項目47</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND47)]
	public string Extend47
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND47]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND47] = value; }
	}
	/// <summary>拡張項目48</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND48)]
	public string Extend48
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND48]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND48] = value; }
	}
	/// <summary>拡張項目49</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND49)]
	public string Extend49
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND49]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND49] = value; }
	}
	/// <summary>拡張項目50</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND50)]
	public string Extend50
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND50]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND50] = value; }
	}
	/// <summary>拡張項目51</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND51)]
	public string Extend51
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND51]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND51] = value; }
	}
	/// <summary>拡張項目52</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND52)]
	public string Extend52
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND52]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND52] = value; }
	}
	/// <summary>拡張項目53</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND53)]
	public string Extend53
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND53]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND53] = value; }
	}
	/// <summary>拡張項目54</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND54)]
	public string Extend54
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND54]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND54] = value; }
	}
	/// <summary>拡張項目55</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND55)]
	public string Extend55
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND55]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND55] = value; }
	}
	/// <summary>拡張項目56</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND56)]
	public string Extend56
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND56]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND56] = value; }
	}
	/// <summary>拡張項目57</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND57)]
	public string Extend57
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND57]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND57] = value; }
	}
	/// <summary>拡張項目58</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND58)]
	public string Extend58
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND58]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND58] = value; }
	}
	/// <summary>拡張項目59</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND59)]
	public string Extend59
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND59]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND59] = value; }
	}
	/// <summary>拡張項目60</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND60)]
	public string Extend60
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND60]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND60] = value; }
	}
	/// <summary>拡張項目61</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND61)]
	public string Extend61
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND61]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND61] = value; }
	}
	/// <summary>拡張項目62</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND62)]
	public string Extend62
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND62]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND62] = value; }
	}
	/// <summary>拡張項目63</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND63)]
	public string Extend63
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND63]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND63] = value; }
	}
	/// <summary>拡張項目64</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND64)]
	public string Extend64
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND64]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND64] = value; }
	}
	/// <summary>拡張項目65</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND65)]
	public string Extend65
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND65]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND65] = value; }
	}
	/// <summary>拡張項目66</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND66)]
	public string Extend66
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND66]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND66] = value; }
	}
	/// <summary>拡張項目67</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND67)]
	public string Extend67
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND67]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND67] = value; }
	}
	/// <summary>拡張項目68</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND68)]
	public string Extend68
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND68]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND68] = value; }
	}
	/// <summary>拡張項目69</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND69)]
	public string Extend69
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND69]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND69] = value; }
	}
	/// <summary>拡張項目70</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND70)]
	public string Extend70
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND70]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND70] = value; }
	}
	/// <summary>拡張項目71</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND71)]
	public string Extend71
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND71]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND71] = value; }
	}
	/// <summary>拡張項目72</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND72)]
	public string Extend72
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND72]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND72] = value; }
	}
	/// <summary>拡張項目73</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND73)]
	public string Extend73
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND73]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND73] = value; }
	}
	/// <summary>拡張項目74</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND74)]
	public string Extend74
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND74]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND74] = value; }
	}
	/// <summary>拡張項目75</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND75)]
	public string Extend75
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND75]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND75] = value; }
	}
	/// <summary>拡張項目76</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND76)]
	public string Extend76
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND76]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND76] = value; }
	}
	/// <summary>拡張項目77</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND77)]
	public string Extend77
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND77]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND77] = value; }
	}
	/// <summary>拡張項目78</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND78)]
	public string Extend78
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND78]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND78] = value; }
	}
	/// <summary>拡張項目79</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND79)]
	public string Extend79
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND79]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND79] = value; }
	}
	/// <summary>拡張項目80</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND80)]
	public string Extend80
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND80]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND80] = value; }
	}
	/// <summary>拡張項目81</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND81)]
	public string Extend81
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND81]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND81] = value; }
	}
	/// <summary>拡張項目82</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND82)]
	public string Extend82
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND82]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND82] = value; }
	}
	/// <summary>拡張項目83</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND83)]
	public string Extend83
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND83]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND83] = value; }
	}
	/// <summary>拡張項目84</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND84)]
	public string Extend84
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND84]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND84] = value; }
	}
	/// <summary>拡張項目85</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND85)]
	public string Extend85
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND85]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND85] = value; }
	}
	/// <summary>拡張項目86</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND86)]
	public string Extend86
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND86]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND86] = value; }
	}
	/// <summary>拡張項目87</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND87)]
	public string Extend87
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND87]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND87] = value; }
	}
	/// <summary>拡張項目88</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND88)]
	public string Extend88
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND88]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND88] = value; }
	}
	/// <summary>拡張項目89</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND89)]
	public string Extend89
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND89]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND89] = value; }
	}
	/// <summary>拡張項目90</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND90)]
	public string Extend90
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND90]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND90] = value; }
	}
	/// <summary>拡張項目91</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND91)]
	public string Extend91
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND91]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND91] = value; }
	}
	/// <summary>拡張項目92</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND92)]
	public string Extend92
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND92]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND92] = value; }
	}
	/// <summary>拡張項目93</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND93)]
	public string Extend93
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND93]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND93] = value; }
	}
	/// <summary>拡張項目94</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND94)]
	public string Extend94
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND94]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND94] = value; }
	}
	/// <summary>拡張項目95</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND95)]
	public string Extend95
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND95]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND95] = value; }
	}
	/// <summary>拡張項目96</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND96)]
	public string Extend96
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND96]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND96] = value; }
	}
	/// <summary>拡張項目97</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND97)]
	public string Extend97
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND97]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND97] = value; }
	}
	/// <summary>拡張項目98</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND98)]
	public string Extend98
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND98]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND98] = value; }
	}
	/// <summary>拡張項目99</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND99)]
	public string Extend99
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND99]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND99] = value; }
	}
	/// <summary>拡張項目100</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND100)]
	public string Extend100
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND100]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND100] = value; }
	}
	/// <summary>拡張項目101</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND101)]
	public string Extend101
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND101]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND101] = value; }
	}
	/// <summary>拡張項目102</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND102)]
	public string Extend102
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND102]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND102] = value; }
	}
	/// <summary>拡張項目103</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND103)]
	public string Extend103
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND103]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND103] = value; }
	}
	/// <summary>拡張項目104</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND104)]
	public string Extend104
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND104]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND104] = value; }
	}
	/// <summary>拡張項目105</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND105)]
	public string Extend105
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND105]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND105] = value; }
	}
	/// <summary>拡張項目106</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND106)]
	public string Extend106
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND106]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND106] = value; }
	}
	/// <summary>拡張項目107</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND107)]
	public string Extend107
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND107]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND107] = value; }
	}
	/// <summary>拡張項目108</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND108)]
	public string Extend108
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND108]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND108] = value; }
	}
	/// <summary>拡張項目109</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND109)]
	public string Extend109
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND109]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND109] = value; }
	}
	/// <summary>拡張項目110</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND110)]
	public string Extend110
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND110]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND110] = value; }
	}
	/// <summary>拡張項目111</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND111)]
	public string Extend111
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND111]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND111] = value; }
	}
	/// <summary>拡張項目112</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND112)]
	public string Extend112
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND112]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND112] = value; }
	}
	/// <summary>拡張項目113</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND113)]
	public string Extend113
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND113]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND113] = value; }
	}
	/// <summary>拡張項目114</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND114)]
	public string Extend114
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND114]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND114] = value; }
	}
	/// <summary>拡張項目115</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND115)]
	public string Extend115
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND115]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND115] = value; }
	}
	/// <summary>拡張項目116</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND116)]
	public string Extend116
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND116]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND116] = value; }
	}
	/// <summary>拡張項目117</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND117)]
	public string Extend117
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND117]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND117] = value; }
	}
	/// <summary>拡張項目118</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND118)]
	public string Extend118
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND118]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND118] = value; }
	}
	/// <summary>拡張項目119</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND119)]
	public string Extend119
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND119]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND119] = value; }
	}
	/// <summary>拡張項目120</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND120)]
	public string Extend120
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND120]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND120] = value; }
	}
	/// <summary>拡張項目121</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND121)]
	public string Extend121
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND121]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND121] = value; }
	}
	/// <summary>拡張項目122</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND122)]
	public string Extend122
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND122]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND122] = value; }
	}
	/// <summary>拡張項目123</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND123)]
	public string Extend123
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND123]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND123] = value; }
	}
	/// <summary>拡張項目124</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND124)]
	public string Extend124
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND124]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND124] = value; }
	}
	/// <summary>拡張項目125</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND125)]
	public string Extend125
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND125]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND125] = value; }
	}
	/// <summary>拡張項目126</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND126)]
	public string Extend126
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND126]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND126] = value; }
	}
	/// <summary>拡張項目127</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND127)]
	public string Extend127
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND127]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND127] = value; }
	}
	/// <summary>拡張項目128</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND128)]
	public string Extend128
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND128]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND128] = value; }
	}
	/// <summary>拡張項目129</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND129)]
	public string Extend129
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND129]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND129] = value; }
	}
	/// <summary>拡張項目130</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND130)]
	public string Extend130
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND130]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND130] = value; }
	}
	/// <summary>拡張項目131</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND131)]
	public string Extend131
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND131]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND131] = value; }
	}
	/// <summary>拡張項目132</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND132)]
	public string Extend132
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND132]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND132] = value; }
	}
	/// <summary>拡張項目133</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND133)]
	public string Extend133
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND133]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND133] = value; }
	}
	/// <summary>拡張項目134</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND134)]
	public string Extend134
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND134]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND134] = value; }
	}
	/// <summary>拡張項目135</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND135)]
	public string Extend135
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND135]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND135] = value; }
	}
	/// <summary>拡張項目136</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND136)]
	public string Extend136
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND136]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND136] = value; }
	}
	/// <summary>拡張項目137</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND137)]
	public string Extend137
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND137]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND137] = value; }
	}
	/// <summary>拡張項目138</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND138)]
	public string Extend138
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND138]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND138] = value; }
	}
	/// <summary>拡張項目139</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND139)]
	public string Extend139
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND139]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND139] = value; }
	}
	/// <summary>拡張項目140</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_EXTEND140)]
	public string Extend140
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND140]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_EXTEND140] = value; }
	}

	/// <summary>拡張項目1 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND1)]
	public string NameExtend1
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND1]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND1] = value; }
	}
	/// <summary>拡張項目2 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND2)]
	public string NameExtend2
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND2]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND2] = value; }
	}
	/// <summary>拡張項目3 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND3)]
	public string NameExtend3
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND3]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND3] = value; }
	}
	/// <summary>拡張項目4 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND4)]
	public string NameExtend4
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND4]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND4] = value; }
	}
	/// <summary>拡張項目5 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND5)]
	public string NameExtend5
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND5]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND5] = value; }
	}
	/// <summary>拡張項目6 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND6)]
	public string NameExtend6
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND6]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND6] = value; }
	}
	/// <summary>拡張項目7 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND7)]
	public string NameExtend7
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND7]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND7] = value; }
	}
	/// <summary>拡張項目8 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND8)]
	public string NameExtend8
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND8]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND8] = value; }
	}
	/// <summary>拡張項目9 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND9)]
	public string NameExtend9
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND9]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND9] = value; }
	}
	/// <summary>拡張項目10 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND10)]
	public string NameExtend10
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND10]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND10] = value; }
	}
	/// <summary>拡張項目11 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND11)]
	public string NameExtend11
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND11]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND11] = value; }
	}
	/// <summary>拡張項目12 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND12)]
	public string NameExtend12
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND12]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND12] = value; }
	}
	/// <summary>拡張項目13 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND13)]
	public string NameExtend13
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND13]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND13] = value; }
	}
	/// <summary>拡張項目14 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND14)]
	public string NameExtend14
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND14]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND14] = value; }
	}
	/// <summary>拡張項目15 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND15)]
	public string NameExtend15
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND15]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND15] = value; }
	}
	/// <summary>拡張項目16 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND16)]
	public string NameExtend16
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND16]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND16] = value; }
	}
	/// <summary>拡張項目17 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND17)]
	public string NameExtend17
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND17]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND17] = value; }
	}
	/// <summary>拡張項目18 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND18)]
	public string NameExtend18
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND18]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND18] = value; }
	}
	/// <summary>拡張項目19 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND19)]
	public string NameExtend19
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND19]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND19] = value; }
	}
	/// <summary>拡張項目20 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND20)]
	public string NameExtend20
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND20]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND20] = value; }
	}
	/// <summary>拡張項目21 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND21)]
	public string NameExtend21
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND21]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND21] = value; }
	}
	/// <summary>拡張項目22 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND22)]
	public string NameExtend22
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND22]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND22] = value; }
	}
	/// <summary>拡張項目23 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND23)]
	public string NameExtend23
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND23]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND23] = value; }
	}
	/// <summary>拡張項目24 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND24)]
	public string NameExtend24
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND24]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND24] = value; }
	}
	/// <summary>拡張項目25 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND25)]
	public string NameExtend25
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND25]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND25] = value; }
	}
	/// <summary>拡張項目26 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND26)]
	public string NameExtend26
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND26]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND26] = value; }
	}
	/// <summary>拡張項目27 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND27)]
	public string NameExtend27
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND27]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND27] = value; }
	}
	/// <summary>拡張項目28 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND28)]
	public string NameExtend28
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND28]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND28] = value; }
	}
	/// <summary>拡張項目29 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND29)]
	public string NameExtend29
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND29]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND29] = value; }
	}
	/// <summary>拡張項目30 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND30)]
	public string NameExtend30
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND30]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND30] = value; }
	}
	/// <summary>拡張項目31 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND31)]
	public string NameExtend31
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND31]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND31] = value; }
	}
	/// <summary>拡張項目32 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND32)]
	public string NameExtend32
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND32]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND32] = value; }
	}
	/// <summary>拡張項目33 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND33)]
	public string NameExtend33
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND33]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND33] = value; }
	}
	/// <summary>拡張項目34 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND34)]
	public string NameExtend34
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND34]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND34] = value; }
	}
	/// <summary>拡張項目35 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND35)]
	public string NameExtend35
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND35]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND35] = value; }
	}
	/// <summary>拡張項目36 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND36)]
	public string NameExtend36
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND36]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND36] = value; }
	}
	/// <summary>拡張項目37 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND37)]
	public string NameExtend37
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND37]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND37] = value; }
	}
	/// <summary>拡張項目38 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND38)]
	public string NameExtend38
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND38]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND38] = value; }
	}
	/// <summary>拡張項目39 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND39)]
	public string NameExtend39
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND39]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND39] = value; }
	}
	/// <summary>拡張項目40 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND40)]
	public string NameExtend40
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND40]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND40] = value; }
	}
	/// <summary>拡張項目41 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND41)]
	public string NameExtend41
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND41]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND41] = value; }
	}
	/// <summary>拡張項目42 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND42)]
	public string NameExtend42
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND42]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND42] = value; }
	}
	/// <summary>拡張項目43 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND43)]
	public string NameExtend43
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND43]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND43] = value; }
	}
	/// <summary>拡張項目44 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND44)]
	public string NameExtend44
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND44]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND44] = value; }
	}
	/// <summary>拡張項目45 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND45)]
	public string NameExtend45
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND45]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND45] = value; }
	}
	/// <summary>拡張項目46 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND46)]
	public string NameExtend46
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND46]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND46] = value; }
	}
	/// <summary>拡張項目47 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND47)]
	public string NameExtend47
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND47]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND47] = value; }
	}
	/// <summary>拡張項目48 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND48)]
	public string NameExtend48
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND48]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND48] = value; }
	}
	/// <summary>拡張項目49 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND49)]
	public string NameExtend49
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND49]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND49] = value; }
	}
	/// <summary>拡張項目50 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND50)]
	public string NameExtend50
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND50]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND50] = value; }
	}
	/// <summary>拡張項目51 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND51)]
	public string NameExtend51
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND51]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND51] = value; }
	}
	/// <summary>拡張項目52 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND52)]
	public string NameExtend52
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND52]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND52] = value; }
	}
	/// <summary>拡張項目53 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND53)]
	public string NameExtend53
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND53]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND53] = value; }
	}
	/// <summary>拡張項目54 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND54)]
	public string NameExtend54
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND54]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND54] = value; }
	}
	/// <summary>拡張項目55 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND55)]
	public string NameExtend55
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND55]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND55] = value; }
	}
	/// <summary>拡張項目56 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND56)]
	public string NameExtend56
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND56]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND56] = value; }
	}
	/// <summary>拡張項目57 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND57)]
	public string NameExtend57
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND57]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND57] = value; }
	}
	/// <summary>拡張項目58 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND58)]
	public string NameExtend58
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND58]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND58] = value; }
	}
	/// <summary>拡張項目59 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND59)]
	public string NameExtend59
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND59]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND59] = value; }
	}
	/// <summary>拡張項目60 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND60)]
	public string NameExtend60
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND60]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND60] = value; }
	}
	/// <summary>拡張項目61 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND61)]
	public string NameExtend61
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND61]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND61] = value; }
	}
	/// <summary>拡張項目62 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND62)]
	public string NameExtend62
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND62]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND62] = value; }
	}
	/// <summary>拡張項目63 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND63)]
	public string NameExtend63
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND63]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND63] = value; }
	}
	/// <summary>拡張項目64 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND64)]
	public string NameExtend64
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND64]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND64] = value; }
	}
	/// <summary>拡張項目65 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND65)]
	public string NameExtend65
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND65]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND65] = value; }
	}
	/// <summary>拡張項目66 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND66)]
	public string NameExtend66
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND66]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND66] = value; }
	}
	/// <summary>拡張項目67 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND67)]
	public string NameExtend67
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND67]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND67] = value; }
	}
	/// <summary>拡張項目68 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND68)]
	public string NameExtend68
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND68]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND68] = value; }
	}
	/// <summary>拡張項目69 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND69)]
	public string NameExtend69
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND69]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND69] = value; }
	}
	/// <summary>拡張項目70 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND70)]
	public string NameExtend70
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND70]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND70] = value; }
	}
	/// <summary>拡張項目71 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND71)]
	public string NameExtend71
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND71]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND71] = value; }
	}
	/// <summary>拡張項目72 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND72)]
	public string NameExtend72
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND72]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND72] = value; }
	}
	/// <summary>拡張項目73 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND73)]
	public string NameExtend73
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND73]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND73] = value; }
	}
	/// <summary>拡張項目74 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND74)]
	public string NameExtend74
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND74]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND74] = value; }
	}
	/// <summary>拡張項目75 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND75)]
	public string NameExtend75
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND75]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND75] = value; }
	}
	/// <summary>拡張項目76 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND76)]
	public string NameExtend76
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND76]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND76] = value; }
	}
	/// <summary>拡張項目77 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND77)]
	public string NameExtend77
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND77]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND77] = value; }
	}
	/// <summary>拡張項目78 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND78)]
	public string NameExtend78
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND78]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND78] = value; }
	}
	/// <summary>拡張項目79 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND79)]
	public string NameExtend79
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND79]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND79] = value; }
	}
	/// <summary>拡張項目80 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND80)]
	public string NameExtend80
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND80]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND80] = value; }
	}
	/// <summary>拡張項目81 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND81)]
	public string NameExtend81
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND81]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND81] = value; }
	}
	/// <summary>拡張項目82 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND82)]
	public string NameExtend82
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND82]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND82] = value; }
	}
	/// <summary>拡張項目83 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND83)]
	public string NameExtend83
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND83]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND83] = value; }
	}
	/// <summary>拡張項目84 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND84)]
	public string NameExtend84
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND84]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND84] = value; }
	}
	/// <summary>拡張項目85 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND85)]
	public string NameExtend85
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND85]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND85] = value; }
	}
	/// <summary>拡張項目86 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND86)]
	public string NameExtend86
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND86]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND86] = value; }
	}
	/// <summary>拡張項目87 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND87)]
	public string NameExtend87
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND87]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND87] = value; }
	}
	/// <summary>拡張項目88 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND88)]
	public string NameExtend88
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND88]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND88] = value; }
	}
	/// <summary>拡張項目89 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND89)]
	public string NameExtend89
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND89]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND89] = value; }
	}
	/// <summary>拡張項目90 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND90)]
	public string NameExtend90
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND90]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND90] = value; }
	}
	/// <summary>拡張項目91 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND91)]
	public string NameExtend91
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND91]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND91] = value; }
	}
	/// <summary>拡張項目92 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND92)]
	public string NameExtend92
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND92]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND92] = value; }
	}
	/// <summary>拡張項目93 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND93)]
	public string NameExtend93
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND93]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND93] = value; }
	}
	/// <summary>拡張項目94 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND94)]
	public string NameExtend94
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND94]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND94] = value; }
	}
	/// <summary>拡張項目95 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND95)]
	public string NameExtend95
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND95]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND95] = value; }
	}
	/// <summary>拡張項目96 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND96)]
	public string NameExtend96
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND96]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND96] = value; }
	}
	/// <summary>拡張項目97 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND97)]
	public string NameExtend97
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND97]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND97] = value; }
	}
	/// <summary>拡張項目98 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND98)]
	public string NameExtend98
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND98]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND98] = value; }
	}
	/// <summary>拡張項目99 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND99)]
	public string NameExtend99
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND99]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND99] = value; }
	}
	/// <summary>拡張項目100 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND100)]
	public string NameExtend100
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND100]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND100] = value; }
	}
	/// <summary>拡張項目101 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND101)]
	public string NameExtend101
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND101]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND101] = value; }
	}
	/// <summary>拡張項目102 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND102)]
	public string NameExtend102
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND102]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND102] = value; }
	}
	/// <summary>拡張項目103 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND103)]
	public string NameExtend103
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND103]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND103] = value; }
	}
	/// <summary>拡張項目104 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND104)]
	public string NameExtend104
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND104]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND104] = value; }
	}
	/// <summary>拡張項目105 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND105)]
	public string NameExtend105
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND105]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND105] = value; }
	}
	/// <summary>拡張項目106 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND106)]
	public string NameExtend106
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND106]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND106] = value; }
	}
	/// <summary>拡張項目107 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND107)]
	public string NameExtend107
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND107]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND107] = value; }
	}
	/// <summary>拡張項目108 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND108)]
	public string NameExtend108
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND108]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND108] = value; }
	}
	/// <summary>拡張項目109 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND109)]
	public string NameExtend109
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND109]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND109] = value; }
	}
	/// <summary>拡張項目110 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND110)]
	public string NameExtend110
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND110]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND110] = value; }
	}
	/// <summary>拡張項目111 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND111)]
	public string NameExtend111
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND111]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND111] = value; }
	}
	/// <summary>拡張項目112 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND112)]
	public string NameExtend112
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND112]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND112] = value; }
	}
	/// <summary>拡張項目113 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND113)]
	public string NameExtend113
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND113]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND113] = value; }
	}
	/// <summary>拡張項目114 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND114)]
	public string NameExtend114
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND114]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND114] = value; }
	}
	/// <summary>拡張項目115 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND115)]
	public string NameExtend115
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND115]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND115] = value; }
	}
	/// <summary>拡張項目116 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND116)]
	public string NameExtend116
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND116]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND116] = value; }
	}
	/// <summary>拡張項目117 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND117)]
	public string NameExtend117
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND117]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND117] = value; }
	}
	/// <summary>拡張項目118 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND118)]
	public string NameExtend118
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND118]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND118] = value; }
	}
	/// <summary>拡張項目119 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND119)]
	public string NameExtend119
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND119]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND119] = value; }
	}
	/// <summary>拡張項目120 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND120)]
	public string NameExtend120
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND120]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND120] = value; }
	}
	/// <summary>拡張項目121 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND121)]
	public string NameExtend121
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND121]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND121] = value; }
	}
	/// <summary>拡張項目122 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND122)]
	public string NameExtend122
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND122]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND122] = value; }
	}
	/// <summary>拡張項目123 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND123)]
	public string NameExtend123
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND123]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND123] = value; }
	}
	/// <summary>拡張項目124 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND124)]
	public string NameExtend124
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND124]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND124] = value; }
	}
	/// <summary>拡張項目125 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND125)]
	public string NameExtend125
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND125]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND125] = value; }
	}
	/// <summary>拡張項目126 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND126)]
	public string NameExtend126
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND126]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND126] = value; }
	}
	/// <summary>拡張項目127 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND127)]
	public string NameExtend127
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND127]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND127] = value; }
	}
	/// <summary>拡張項目128 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND128)]
	public string NameExtend128
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND128]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND128] = value; }
	}
	/// <summary>拡張項目129 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND129)]
	public string NameExtend129
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND129]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND129] = value; }
	}
	/// <summary>拡張項目130 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND130)]
	public string NameExtend130
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND130]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND130] = value; }
	}
	/// <summary>拡張項目131 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND131)]
	public string NameExtend131
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND131]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND131] = value; }
	}
	/// <summary>拡張項目132 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND132)]
	public string NameExtend132
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND132]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND132] = value; }
	}
	/// <summary>拡張項目133 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND133)]
	public string NameExtend133
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND133]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND133] = value; }
	}
	/// <summary>拡張項目134 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND134)]
	public string NameExtend134
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND134]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND134] = value; }
	}
	/// <summary>拡張項目135 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND135)]
	public string NameExtend135
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND135]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND135] = value; }
	}
	/// <summary>拡張項目136 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND136)]
	public string NameExtend136
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND136]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND136] = value; }
	}
	/// <summary>拡張項目137 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND137)]
	public string NameExtend137
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND137]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND137] = value; }
	}
	/// <summary>拡張項目138 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND138)]
	public string NameExtend138
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND138]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND138] = value; }
	}
	/// <summary>拡張項目139</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND139)]
	public string NameExtend139
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND139]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND139] = value; }
	}
	/// <summary>拡張項目140 (Name)</summary>
	[JsonProperty(EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND140)]
	public string NameExtend140
	{
		get { return (string)this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND140]; }
		set { this.DataSource[EXTEND_NAME_BASE + Constants.FIELD_PRODUCTEXTEND_EXTEND140] = value; }
	}
	/// <summary>削除フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_DEL_FLG)]
	public string DelFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_DEL_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_DEL_FLG] = value; }
	}
	/// <summary>最終更新者</summary>
	[JsonProperty(Constants.FIELD_PRODUCTEXTEND_LAST_CHANGED)]
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTEXTEND_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_PRODUCTEXTEND_LAST_CHANGED] = value; }
	}
	#endregion
}