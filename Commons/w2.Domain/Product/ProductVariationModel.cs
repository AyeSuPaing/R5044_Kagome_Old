/*
=========================================================================================================
  Module      : 商品バリエーションマスタモデル (ProductVariationModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.ProductPrice;

namespace w2.Domain.ProductVariation
{
	/// <summary>
	/// 商品バリエーションマスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductVariationModel : ModelBase<ProductVariationModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductVariationModel()
		{
			this.ShopId = Constants.CONST_DEFAULT_SHOP_ID;
			this.ProductId = "";
			this.VariationId = "";
			this.VariationCooperationId1 = "";
			this.VariationCooperationId2 = "";
			this.VariationCooperationId3 = "";
			this.VariationCooperationId4 = "";
			this.VariationCooperationId5 = "";
			this.MallVariationId1 = "";
			this.MallVariationId2 = "";
			this.MallVariationType = "";
			this.VariationName1 = "";
			this.VariationName2 = "";
			this.VariationName3 = "";
			this.Price = 0;
			this.SpecialPrice = null;
			this.VariationImageHead = "";
			this.VariationImageMobile = "";
			this.ShippingType = "";
			this.ShippingSizeKbn = "";
			this.DisplayOrder = 1;
			this.VariationMallCooperatedFlg = "0";
			this.ValidFlg = Constants.FLG_PRODUCTVARIATION_VALID_FLG_INVALID;
			this.DelFlg = Constants.FLG_PRODUCTVARIATION_DEL_FLG_UNDELETED;
			this.LastChanged = string.Empty;
			this.VariationDownloadUrl = string.Empty;
			this.VariationFixedPurchaseFirsttimePrice = null;
			this.VariationFixedPurchasePrice = null;
			this.VariationCooperationId6 = "";
			this.VariationCooperationId7 = "";
			this.VariationCooperationId8 = "";
			this.VariationCooperationId9 = "";
			this.VariationCooperationId10 = "";
			this.VariationColorId = "";
			this.VariationAndmallReservationFlg = Constants.FLG_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG_COMMON;
			this.VariationWeightGram = 0;
			this.VariationAddCartUrlLimitFlg = Constants.FLG_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG_INVALID;
			this.ProductPrices = new ProductPriceModel[0];
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductVariationModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductVariationModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID] = value; }
		}
		/// <summary>商品バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID] = value; }
		}
		/// <summary>商品バリエーション連携ID1</summary>
		public string VariationCooperationId1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1] = value; }
		}
		/// <summary>商品バリエーション連携ID2</summary>
		public string VariationCooperationId2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2] = value; }
		}
		/// <summary>商品バリエーション連携ID3</summary>
		public string VariationCooperationId3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3] = value; }
		}
		/// <summary>商品バリエーション連携ID4</summary>
		public string VariationCooperationId4
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4] = value; }
		}
		/// <summary>商品バリエーション連携ID5</summary>
		public string VariationCooperationId5
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5] = value; }
		}
		/// <summary>モールバリエーションID1</summary>
		public string MallVariationId1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1] = value; }
		}
		/// <summary>モールバリエーションID2</summary>
		public string MallVariationId2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID2]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID2] = value; }
		}
		/// <summary>モールバリエーション種別</summary>
		public string MallVariationType
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_TYPE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_TYPE] = value; }
		}
		/// <summary>バリエーション名1</summary>
		public string VariationName1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1] = value; }
		}
		/// <summary>バリエーション名2</summary>
		public string VariationName2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2] = value; }
		}
		/// <summary>バリエーション名3</summary>
		public string VariationName3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3] = value; }
		}
		/// <summary>販売価格</summary>
		public decimal Price
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE] = value; }
		}
		/// <summary>特別価格</summary>
		public decimal? SpecialPrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] = value; }
		}
		/// <summary>バリエーション画像名ヘッダ</summary>
		public string VariationImageHead
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD] = value; }
		}
		/// <summary>モバイルバリエーション画像名</summary>
		public string VariationImageMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_MOBILE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_MOBILE] = value; }
		}
		/// <summary>配送料種別</summary>
		public string ShippingType
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_SHIPPING_TYPE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_SHIPPING_TYPE] = value; }
		}
		/// <summary>配送サイズ区分</summary>
		public string ShippingSizeKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_SHIPPING_SIZE_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_SHIPPING_SIZE_KBN] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER] = value; }
		}
		/// <summary>バリエーションモール連携済フラグ</summary>
		public string VariationMallCooperatedFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_MALL_COOPERATED_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_MALL_COOPERATED_FLG] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTVARIATION_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTVARIATION_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_LAST_CHANGED] = value; }
		}
		/// <summary>ダウンロードURL</summary>
		public string VariationDownloadUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL] = value; }
		}
		/// <summary>定期初回購入価格</summary>
		public decimal? VariationFixedPurchaseFirsttimePrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE] = value; }
		}
		/// <summary>定期購入価格</summary>
		public decimal? VariationFixedPurchasePrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] = value; }
		}
		/// <summary>商品バリエーション連携ID6</summary>
		public string VariationCooperationId6
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6] = value; }
		}
		/// <summary>商品バリエーション連携ID7</summary>
		public string VariationCooperationId7
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7] = value; }
		}
		/// <summary>商品バリエーション連携ID8</summary>
		public string VariationCooperationId8
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8] = value; }
		}
		/// <summary>商品バリエーション連携ID9</summary>
		public string VariationCooperationId9
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9] = value; }
		}
		/// <summary>商品バリエーション連携ID10</summary>
		public string VariationCooperationId10
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10] = value; }
		}
		/// <summary>商品カラーID</summary>
		public string VariationColorId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID] = value; }
		}
		/// <summary>＆mallの予約商品フラグ</summary>
		public string VariationAndmallReservationFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG] = value; }
		}
		/// <summary>商品バリエーション重量（g）</summary>
		public int VariationWeightGram
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM] = value; }
		}
		/// <summary>カート投入URL制限フラグ</summary>
		public string VariationAddCartUrlLimitFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG] = value; }
		}
		#endregion
	}
}
