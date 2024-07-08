/*
=========================================================================================================
  Module      : 商品在庫文言入力クラス (ProductStockMessageInput.cs)
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
using w2.Domain.ProductStockMessage;

/// <summary>
/// 商品在庫文言マスタ入力クラス
/// </summary>
public class ProductStockMessageInput : InputBase<ProductStockMessageModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public ProductStockMessageInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public ProductStockMessageInput(ProductStockMessageModel model)
		: this()
	{
		this.ShopId = model.ShopId;
		this.StockMessageId = model.StockMessageId;
		this.StockMessageName = model.StockMessageName;
		this.StockMessageDef = model.StockMessageDef;
		this.StockDatum1 = (model.StockDatum1 != null) ? model.StockDatum1.ToString() : null;
		this.StockMessage1 = model.StockMessage1;
		this.StockDatum2 = (model.StockDatum2 != null) ? model.StockDatum2.ToString() : null;
		this.StockMessage2 = model.StockMessage2;
		this.StockDatum3 = (model.StockDatum3 != null) ? model.StockDatum3.ToString() : null;
		this.StockMessage3 = model.StockMessage3;
		this.StockDatum4 = (model.StockDatum4 != null) ? model.StockDatum4.ToString() : null;
		this.StockMessage4 = model.StockMessage4;
		this.StockDatum5 = (model.StockDatum5 != null) ? model.StockDatum5.ToString() : null;
		this.StockMessage5 = model.StockMessage5;
		this.StockDatum6 = (model.StockDatum6 != null) ? model.StockDatum6.ToString() : null;
		this.StockMessage6 = model.StockMessage6;
		this.StockDatum7 = (model.StockDatum7 != null) ? model.StockDatum7.ToString() : null;
		this.StockMessage7 = model.StockMessage7;
		this.StockDatum8 = (model.StockDatum8 != null) ? model.StockDatum8.ToString() : null;
		this.StockMessage8 = model.StockMessage8;
		this.StockDatum9 = (model.StockDatum9 != null) ? model.StockDatum9.ToString() : null;
		this.StockMessage9 = model.StockMessage9;
		this.DelFlg = model.DelFlg;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
		this.LastChanged = model.LastChanged;
		this.StockMessageDefMobile = model.StockMessageDefMobile;
		this.StockMessageMobile1 = model.StockMessageMobile1;
		this.StockMessageMobile2 = model.StockMessageMobile2;
		this.StockMessageMobile3 = model.StockMessageMobile3;
		this.StockMessageMobile4 = model.StockMessageMobile4;
		this.StockMessageMobile5 = model.StockMessageMobile5;
		this.StockMessageMobile6 = model.StockMessageMobile6;
		this.StockMessageMobile7 = model.StockMessageMobile7;
		this.StockMessageMobile8 = model.StockMessageMobile8;
		this.StockMessageMobile9 = model.StockMessageMobile9;
		this.LanguageCode = model.LanguageCode;
		this.LanguageLocaleId = model.LanguageLocaleId;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override ProductStockMessageModel CreateModel()
	{
		var model = new ProductStockMessageModel
		{
			ShopId = this.ShopId,
			StockMessageId = this.StockMessageId,
			StockMessageName = this.StockMessageName,
			StockMessageDef = this.StockMessageDef,
			StockDatum1 = (this.StockDatum1 != null) ? int.Parse(this.StockDatum1) : (int?)null,
			StockMessage1 = this.StockMessage1,
			StockDatum2 = (this.StockDatum2 != null) ? int.Parse(this.StockDatum2) : (int?)null,
			StockMessage2 = this.StockMessage2,
			StockDatum3 = (this.StockDatum3 != null) ? int.Parse(this.StockDatum3) : (int?)null,
			StockMessage3 = this.StockMessage3,
			StockDatum4 = (this.StockDatum4 != null) ? int.Parse(this.StockDatum4) : (int?)null,
			StockMessage4 = this.StockMessage4,
			StockDatum5 = (this.StockDatum5 != null) ? int.Parse(this.StockDatum5) : (int?)null,
			StockMessage5 = this.StockMessage5,
			StockDatum6 = (this.StockDatum6 != null) ? int.Parse(this.StockDatum6) : (int?)null,
			StockMessage6 = this.StockMessage6,
			StockDatum7 = (this.StockDatum7 != null) ? int.Parse(this.StockDatum7) : (int?)null,
			StockMessage7 = this.StockMessage7,
			StockDatum8 = (this.StockDatum8 != null) ? int.Parse(this.StockDatum8) : (int?)null,
			StockMessage8 = this.StockMessage8,
			StockDatum9 = (this.StockDatum9 != null) ? int.Parse(this.StockDatum9) : (int?)null,
			StockMessage9 = this.StockMessage9,
			DelFlg = this.DelFlg,
			LastChanged = this.LastChanged,
			StockMessageDefMobile = this.StockMessageDefMobile,
			StockMessageMobile1 = this.StockMessageMobile1,
			StockMessageMobile2 = this.StockMessageMobile2,
			StockMessageMobile3 = this.StockMessageMobile3,
			StockMessageMobile4 = this.StockMessageMobile4,
			StockMessageMobile5 = this.StockMessageMobile5,
			StockMessageMobile6 = this.StockMessageMobile6,
			StockMessageMobile7 = this.StockMessageMobile7,
			StockMessageMobile8 = this.StockMessageMobile8,
			StockMessageMobile9 = this.StockMessageMobile9,
			LanguageCode = this.LanguageCode,
			LanguageLocaleId = this.LanguageLocaleId,
		};
		return model;
	}

	/// <summary>
	/// グローバルモデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public ProductStockMessageGlobalModel CreateGlobalModel()
	{
		var model = new ProductStockMessageGlobalModel
		{
			ShopId = this.ShopId,
			StockMessageId = this.StockMessageId,
			StockMessageName = this.StockMessageName,
			StockMessageDef = this.StockMessageDef,
			StockMessage1 = this.StockMessage1,
			StockMessage2 = this.StockMessage2,
			StockMessage3 = this.StockMessage3,
			StockMessage4 = this.StockMessage4,
			StockMessage5 = this.StockMessage5,
			StockMessage6 = this.StockMessage6,
			StockMessage7 = this.StockMessage7,
			LastChanged = this.LastChanged,
			LanguageCode = this.LanguageCode,
			LanguageLocaleId = this.LanguageLocaleId,
		};
		return model;
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_SHOP_ID] = value; }
	}
	/// <summary>商品在庫文言ID</summary>
	public string StockMessageId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID] = value; }
	}
	/// <summary>在庫文言名</summary>
	public string StockMessageName
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_NAME]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_NAME] = value; }
	}
	/// <summary>標準在庫文言</summary>
	public string StockMessageDef
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF] = value; }
	}
	/// <summary>在庫基準1</summary>
	public string StockDatum1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM1]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM1] = value; }
	}
	/// <summary>在庫文言1</summary>
	public string StockMessage1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE1]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE1] = value; }
	}
	/// <summary>在庫基準2</summary>
	public string StockDatum2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM2]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM2] = value; }
	}
	/// <summary>在庫文言2</summary>
	public string StockMessage2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE2]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE2] = value; }
	}
	/// <summary>在庫基準3</summary>
	public string StockDatum3
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM3]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM3] = value; }
	}
	/// <summary>在庫文言3</summary>
	public string StockMessage3
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE3]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE3] = value; }
	}
	/// <summary>在庫基準4</summary>
	public string StockDatum4
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM4]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM4] = value; }
	}
	/// <summary>在庫文言4</summary>
	public string StockMessage4
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE4]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE4] = value; }
	}
	/// <summary>在庫基準5</summary>
	public string StockDatum5
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM5]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM5] = value; }
	}
	/// <summary>在庫文言5</summary>
	public string StockMessage5
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE5]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE5] = value; }
	}
	/// <summary>在庫基準6</summary>
	public string StockDatum6
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM6]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM6] = value; }
	}
	/// <summary>在庫文言6</summary>
	public string StockMessage6
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE6]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE6] = value; }
	}
	/// <summary>在庫基準7</summary>
	public string StockDatum7
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM7]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM7] = value; }
	}
	/// <summary>在庫文言7</summary>
	public string StockMessage7
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE7]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE7] = value; }
	}
	/// <summary>在庫基準8</summary>
	public string StockDatum8
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM8]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM8] = value; }
	}
	/// <summary>在庫文言8</summary>
	public string StockMessage8
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE8]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE8] = value; }
	}
	/// <summary>在庫基準9</summary>
	public string StockDatum9
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM9]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM9] = value; }
	}
	/// <summary>在庫文言9</summary>
	public string StockMessage9
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE9]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE9] = value; }
	}
	/// <summary>削除フラグ</summary>
	public string DelFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_DEL_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_DEL_FLG] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_LAST_CHANGED] = value; }
	}
	/// <summary>モバイル用標準在庫文言</summary>
	public string StockMessageDefMobile
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF_MOBILE]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF_MOBILE] = value; }
	}
	/// <summary>モバイル用在庫文言1</summary>
	public string StockMessageMobile1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE1]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE1] = value; }
	}
	/// <summary>モバイル用在庫文言2</summary>
	public string StockMessageMobile2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE2]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE2] = value; }
	}
	/// <summary>モバイル用在庫文言3</summary>
	public string StockMessageMobile3
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE3]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE3] = value; }
	}
	/// <summary>モバイル用在庫文言4</summary>
	public string StockMessageMobile4
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE4]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE4] = value; }
	}
	/// <summary>モバイル用在庫文言5</summary>
	public string StockMessageMobile5
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE5]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE5] = value; }
	}
	/// <summary>モバイル用在庫文言6</summary>
	public string StockMessageMobile6
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE6]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE6] = value; }
	}
	/// <summary>モバイル用在庫文言7</summary>
	public string StockMessageMobile7
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE7]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE7] = value; }
	}
	/// <summary>モバイル用在庫文言8</summary>
	public string StockMessageMobile8
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE8]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE8] = value; }
	}
	/// <summary>モバイル用在庫文言9</summary>
	public string StockMessageMobile9
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE9]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE9] = value; }
	}
	/// <summary>言語コード</summary>
	public string LanguageCode
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_CODE]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_CODE] = value; }
	}
	/// <summary>言語ロケールID</summary>
	public string LanguageLocaleId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_LOCALE_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_LOCALE_ID] = value; }
	}
	#endregion
}
