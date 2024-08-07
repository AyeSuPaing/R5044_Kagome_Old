/*
=========================================================================================================
  Module      : 商品在庫文言マスタモデル (ProductStockMessageModel.cs)
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
	/// 商品在庫文言マスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductStockMessageModel : ModelBase<ProductStockMessageModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductStockMessageModel()
		{
			this.StockMessageId = string.Empty;
			this.StockMessageName = string.Empty;
			this.StockDatum1 = null;
			this.StockDatum2 = null;
			this.StockDatum3 = null;
			this.StockDatum4 = null;
			this.StockDatum5 = null;
			this.StockDatum6 = null;
			this.StockDatum7 = null;
			this.StockDatum8 = null;
			this.StockDatum9 = null;
			this.DelFlg = Constants.FLG_PRODUCTSTOCKMESSAGE_DEL_FLG_OFF;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductStockMessageModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductStockMessageModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGE_SHOP_ID)]
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_SHOP_ID] = value; }
		}
		/// <summary>商品在庫文言ID</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID)]
		public string StockMessageId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID] = value; }
		}
		/// <summary>在庫文言名</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_NAME)]
		public string StockMessageName
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_NAME] = value; }
		}
		/// <summary>標準在庫文言</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF)]
		public string StockMessageDef
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF] = value; }
		}
		/// <summary>在庫基準1</summary>
		public int? StockDatum1
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM1] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM1];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM1] = value; }
		}
		/// <summary>在庫文言1</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE1)]
		public string StockMessage1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE1]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE1] = value; }
		}
		/// <summary>在庫基準2</summary>
		public int? StockDatum2
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM2] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM2];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM2] = value; }
		}
		/// <summary>在庫文言2</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE2)]
		public string StockMessage2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE2]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE2] = value; }
		}
		/// <summary>在庫基準3</summary>
		public int? StockDatum3
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM3] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM3];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM3] = value; }
		}
		/// <summary>在庫文言3</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE3)]
		public string StockMessage3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE3]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE3] = value; }
		}
		/// <summary>在庫基準4</summary>
		public int? StockDatum4
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM4] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM4];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM4] = value; }
		}
		/// <summary>在庫文言4</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE4)]
		public string StockMessage4
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE4]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE4] = value; }
		}
		/// <summary>在庫基準5</summary>
		public int? StockDatum5
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM5] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM5];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM5] = value; }
		}
		/// <summary>在庫文言5</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE5)]
		public string StockMessage5
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE5]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE5] = value; }
		}
		/// <summary>在庫基準6</summary>
		public int? StockDatum6
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM6] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM6];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM6] = value; }
		}
		/// <summary>在庫文言6</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE6)]
		public string StockMessage6
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE6]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE6] = value; }
		}
		/// <summary>在庫基準7</summary>
		public int? StockDatum7
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM7] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM7];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM7] = value; }
		}
		/// <summary>在庫文言7</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE7)]
		public string StockMessage7
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE7]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE7] = value; }
		}
		/// <summary>在庫基準8</summary>
		public int? StockDatum8
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM8] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM8];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM8] = value; }
		}
		/// <summary>在庫文言8</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE8)]
		public string StockMessage8
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE8]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE8] = value; }
		}
		/// <summary>在庫基準9</summary>
		public int? StockDatum9
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM9] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM9];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM9] = value; }
		}
		/// <summary>在庫文言9</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE9)]
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
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_DATE_CHANGED]; }
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
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LANGUAGE_CODE)]
		public string LanguageCode
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_CODE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_CODE] = value; }
		}
		/// <summary>言語ロケールID</summary>
		[DbMapName(Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LANGUAGE_LOCALE_ID)]
		public string LanguageLocaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_LOCALE_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_LOCALE_ID] = value; }
		}
		#endregion
	}
}
