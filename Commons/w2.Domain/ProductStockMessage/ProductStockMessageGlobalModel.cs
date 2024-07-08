/*
=========================================================================================================
  Module      : 商品在庫文言グローバルマスタモデル (ProductStockMessageGlobalModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.ProductStockMessage
{
	/// <summary>
	/// 商品在庫文言グローバルマスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductStockMessageGlobalModel : ModelBase<ProductStockMessageGlobalModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductStockMessageGlobalModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductStockMessageGlobalModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductStockMessageGlobalModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_SHOP_ID)]
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_SHOP_ID] = value; }
		}
		/// <summary>商品在庫文言ID</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE_ID)]
		public string StockMessageId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE_ID] = value; }
		}
		/// <summary>言語コード</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LANGUAGE_CODE)]
		public string LanguageCode
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LANGUAGE_CODE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LANGUAGE_CODE] = value; }
		}
		/// <summary>言語ロケールID</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LANGUAGE_LOCALE_ID)]
		public string LanguageLocaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LANGUAGE_LOCALE_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LANGUAGE_LOCALE_ID] = value; }
		}
		/// <summary>在庫文言名</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE_NAME)]
		public string StockMessageName
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE_NAME] = value; }
		}
		/// <summary>標準在庫文言</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE_DEF)]
		public string StockMessageDef
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE_DEF]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE_DEF] = value; }
		}
		/// <summary>在庫文言1</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE1)]
		public string StockMessage1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE1]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE1] = value; }
		}
		/// <summary>在庫文言2</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE2)]
		public string StockMessage2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE2]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE2] = value; }
		}
		/// <summary>在庫文言3</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE3)]
		public string StockMessage3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE3]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE3] = value; }
		}
		/// <summary>在庫文言4</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE4)]
		public string StockMessage4
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE4]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE4] = value; }
		}
		/// <summary>在庫文言5</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE5)]
		public string StockMessage5
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE5]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE5] = value; }
		}
		/// <summary>在庫文言6</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE6)]
		public string StockMessage6
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE6]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE6] = value; }
		}
		/// <summary>在庫文言7</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE7)]
		public string StockMessage7
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE7]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE7] = value; }
		}
		/// <summary>作成日</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_DATE_CREATED)]
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_DATE_CHANGED)]
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LAST_CHANGED)]
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
