/*
=========================================================================================================
  Module      : 商品マスタモデル (ProductModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Product
{
	/// <summary>
	/// 商品マスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductModel : ModelBase<ProductModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductModel()
		{
			this.OutlineKbn = Constants.FLG_PRODUCT_DESC_DETAIL_TEXT;
			this.OutlineKbnMobile = Constants.FLG_PRODUCT_DESC_DETAIL_TEXT;
			this.DescDetailKbn1 = Constants.FLG_PRODUCT_DESC_DETAIL_TEXT;
			this.DescDetailKbn2 = Constants.FLG_PRODUCT_DESC_DETAIL_TEXT;
			this.DescDetailKbn3 = Constants.FLG_PRODUCT_DESC_DETAIL_TEXT;
			this.DescDetailKbn4 = Constants.FLG_PRODUCT_DESC_DETAIL_TEXT;
			this.DescDetailKbn1Mobile = Constants.FLG_PRODUCT_DESC_DETAIL_TEXT;
			this.DescDetailKbn2Mobile = Constants.FLG_PRODUCT_DESC_DETAIL_TEXT;
			this.DisplayPrice = 0;
			this.DisplaySpecialPrice = null;
			this.TaxIncludedFlg = Constants.FLG_PRODUCT_TAX_INCLUDED_PRETAX;
			this.TaxRate = 0;
			this.TaxRoundType = Constants.FLG_ORDER_ORDER_TAX_EXCLUDED_FRACTION_ROUNDING_ROUND_DOWN;
			this.PricePretax = 0;
			this.PriceShipping = null;
			this.ShippingSizeKbn = null;
			this.PriceCost = null;
			this.PointKbn1 = Constants.FLG_PRODUCT_POINT_KBN1_INVALID;
			this.Point1 = 0;
			this.PointKbn2 = Constants.FLG_PRODUCT_POINT_KBN2_INVALID;
			this.Point2 = 0;
			this.PointKbn3 = Constants.FLG_PRODUCT_POINT_KBN3_INVALID;
			this.Point3 = 0;
			this.CampaignFrom = null;
			this.CampaignTo = null;
			this.CampaignPointKbn = Constants.FLG_PRODUCT_CAMPAIGN_POINT_KBN_INVALID;
			this.CampaignPoint = 0;
			this.DisplayTo = null;
			this.SellTo = null;
			this.BeforeSaleDisplayKbn = Constants.FLG_PRODUCT_BEFORE_SALE_DISPLAY_KBN_INVALID;
			this.AfterSaleDisplayKbn = Constants.FLG_PRODUCT_AFTER_SALE_DISPLAY_KBN_INVALID;
			this.MaxSellQuantity = 100;
			this.StockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED;
			this.StockDispKbn = Constants.FLG_PRODUCT_STOCK_DISP_KBN_NUM;
			this.DisplayKbn = Constants.FLG_PRODUCT_DISPLAY_DISP_ALL;
			this.IconFlg1 = Constants.FLG_PRODUCT_ICON_OFF;
			this.IconTermEnd1 = null;
			this.IconFlg2 = Constants.FLG_PRODUCT_ICON_OFF;
			this.IconTermEnd2 = null;
			this.IconFlg3 = Constants.FLG_PRODUCT_ICON_OFF;
			this.IconTermEnd3 = null;
			this.IconFlg4 = Constants.FLG_PRODUCT_ICON_OFF;
			this.IconTermEnd4 = null;
			this.IconFlg5 = Constants.FLG_PRODUCT_ICON_OFF;
			this.IconTermEnd5 = null;
			this.IconFlg6 = Constants.FLG_PRODUCT_ICON_OFF;
			this.IconTermEnd6 = null;
			this.IconFlg7 = Constants.FLG_PRODUCT_ICON_OFF;
			this.IconTermEnd7 = null;
			this.IconFlg8 = Constants.FLG_PRODUCT_ICON_OFF;
			this.IconTermEnd8 = null;
			this.IconFlg9 = Constants.FLG_PRODUCT_ICON_OFF;
			this.IconTermEnd9 = null;
			this.IconFlg10 = Constants.FLG_PRODUCT_ICON_OFF;
			this.IconTermEnd10 = null;
			this.MobileDispFlg = Constants.FLG_PRODUCT_MOBILE_DISP_FLG_ALL;
			this.UseVariationFlg = Constants.FLG_PRODUCT_VALID_FLG_INVALID;
			this.ReservationFlg = Constants.FLG_PRODUCT_RESERVATION_FLG_INVALID;
			this.FixedPurchaseFlg = Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID;
			this.MallCooperatedFlg = Constants.FLG_PRODUCT_MALL_COOPERATED_FLG_INVALID;
			this.AddCartUrlLimitFlg = Constants.FLG_PRODUCT_ADD_CART_URL_LIMIT_FLG_INVALID;
			this.ValidFlg = Constants.FLG_PRODUCT_VALID_FLG_VALID;
			this.DelFlg = Constants.FLG_PRODUCT_DELFLG_UNDELETED;
			this.MemberRankDiscountFlg = Constants.FLG_PRODUCT_MEMBER_RANK_DISCOUNT_FLG_INVALID;
			this.GoogleShoppingFlg = Constants.FLG_PRODUCT_GOOGLE_SHOPPING_FLG_INVALID;
			this.ArrivalMailValidFlg = Constants.FLG_PRODUCT_ARRIVAL_MAIL_VALID_FLG_INVALID;
			this.ReleaseMailValidFlg = Constants.FLG_PRODUCT_RELEASE_MAIL_VALID_FLG_INVALID;
			this.ResaleMailValidFlg = Constants.FLG_PRODUCT_RESALE_MAIL_VALID_FLG_INVALID;
			this.SelectVariationKbn = Constants.FLG_PRODUCT_SELECT_VARIATION_KBN_STANDARD;
			this.SelectVariationKbnMobile = Constants.FLG_PRODUCT_SELECT_VARIATION_KBN_STANDARD;
			this.UseRecommendFlg = Constants.FLG_PRODUCT_USE_RECOMMEND_FLG_VALID;
			this.GiftFlg = Constants.FLG_PRODUCT_GIFT_FLG_INVALID;
			this.AgeLimitFlg = Constants.FLG_PRODUCT_AGE_LIMIT_FLG_INVALID;
			this.PluralShippingPriceFreeFlg = Constants.FLG_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG_INVALID;
			this.DigitalContentsFlg = Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_INVALID;
			this.DisplaySellFlg = Constants.FLG_PRODUCT_DISPLAY_SELL_FLG_UNDISP;
			this.DisplayPriority = 999;
			this.FixedPurchaseFirsttimePrice = null;
			this.FixedPurchasePrice = null;
			this.BundleItemDisplayType = Constants.FLG_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE_VALID;
			this.ProductType = Constants.FLG_PRODUCT_PRODUCT_TYPE_PRODUCT;
			this.Price = 0;
			this.SpecialPrice = null;
			this.DisplayOrder = 1;
			this.TaxCategoryId = Constants.DEFAULT_PRODUCT_TAXCATEGORY_ID;
			this.VariationFixedPurchaseFirsttimePrice = null;
			this.VariationFixedPurchasePrice = null;
			this.SalePrice = 0;
			this.Validity = Constants.FLG_PRODUCTSALE_VALIDITY_INVALID;
			this.Stock = 0;
			this.StockAlert = 0;
			this.Realstock = 0;
			this.RealstockB = 0;
			this.RealstockC = 0;
			this.RealstockReserved = 0;
			this.CheckFixedProductOrderFlg = Constants.FLG_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG_INVALID;
			this.CheckProductOrderFlg = Constants.FLG_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG_INVALID;
			this.StockDatum1 = null;
			this.StockDatum2 = null;
			this.StockDatum3 = null;
			this.StockDatum4 = null;
			this.StockDatum5 = null;
			this.StockDatum6 = null;
			this.StockDatum7 = null;
			this.AndmallReservationFlg = Constants.FLG_PRODUCT_ANDMALL_RESERVATION_FLG_COMMON;
			this.VariationAndmallReservationFlg = Constants.FLG_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG_COMMON;
			this.DisplayOnlyFixedPurchaseMemberFlg = Constants.FLG_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG_OFF;
			this.SellOnlyFixedPurchaseMemberFlg = Constants.FLG_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG_OFF;
			this.ProductWeightGram = 0;
			this.FixedPurchaseNextShippingProductId = string.Empty;
			this.FixedPurchaseNextShippingVariationId = string.Empty;
			this.FixedPurchaseNextShippingItemQuantity = 0;
			this.NextShippingItemFixedPurchaseKbn = string.Empty;
			this.NextShippingItemFixedPurchaseSetting = string.Empty;
			this.ProductSizeFactor = 1;
			this.CooperationId1 = string.Empty;
			this.CooperationId2 = string.Empty;
			this.CooperationId3 = string.Empty;
			this.CooperationId4 = string.Empty;
			this.CooperationId5 = string.Empty;
			this.CooperationId6 = string.Empty;
			this.CooperationId7 = string.Empty;
			this.CooperationId8 = string.Empty;
			this.CooperationId9 = string.Empty;
			this.CooperationId10 = string.Empty;
			this.FixedPurchasedCancelableCount = 0;
			this.SubscriptionBoxFlg = Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID;
			this.DisplayFrom = DateTime.Now;
			this.SellFrom = DateTime.Now;
			this.MemberRankPointExcludeFlg = Constants.FLG_PRODUCT_CHECK_MEMBER_RANK_POINT_EXCLUDE_FLG_INVALID;
			this.DateCreated = DateTime.Now;
			this.DateChanged = DateTime.Now;
			this.StorePickupFlg = Constants.FLG_OFF;
			this.ExcludeFreeShippingFlg = Constants.FLG_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG_INVALID;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_SHOP_ID] = value; }
		}
		/// <summary>サプライヤID</summary>
		public string SupplierId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SUPPLIER_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_SUPPLIER_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_ID] = value; }
		}
		/// <summary>商品連携ID1</summary>
		public string CooperationId1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID1]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID1] = value; }
		}
		/// <summary>商品連携ID2</summary>
		public string CooperationId2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID2]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID2] = value; }
		}
		/// <summary>商品連携ID3</summary>
		public string CooperationId3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID3]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID3] = value; }
		}
		/// <summary>商品連携ID4</summary>
		public string CooperationId4
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID4]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID4] = value; }
		}
		/// <summary>商品連携ID5</summary>
		public string CooperationId5
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID5]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID5] = value; }
		}
		/// <summary>モール拡張商品ID</summary>
		public string MallExProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID] = value; }
		}
		/// <summary>メーカーID</summary>
		public string MakerId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_MAKER_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_MAKER_ID] = value; }
		}
		/// <summary>メーカー名</summary>
		public string MakerName
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_MAKER_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_MAKER_NAME] = value; }
		}
		/// <summary>商品名</summary>
		public string Name
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_NAME] = value; }
		}
		/// <summary>商品名かな</summary>
		public string NameKana
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NAME_KANA]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_NAME_KANA] = value; }
		}
		/// <summary>商品名2</summary>
		public string Name2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NAME2]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_NAME2] = value; }
		}
		/// <summary>商品名2かな</summary>
		public string Name2Kana
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NAME2_KANA]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_NAME2_KANA] = value; }
		}
		/// <summary>SEOキーワード</summary>
		public string SeoKeywords
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SEO_KEYWORDS]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_SEO_KEYWORDS] = value; }
		}
		/// <summary>キャッチコピー</summary>
		public string Catchcopy
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATCHCOPY]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_CATCHCOPY] = value; }
		}
		/// <summary>モバイルキャッチコピー</summary>
		public string CatchcopyMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATCHCOPY_MOBILE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_CATCHCOPY_MOBILE] = value; }
		}
		/// <summary>商品概要HTML区分</summary>
		public string OutlineKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_OUTLINE_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_OUTLINE_KBN] = value; }
		}
		/// <summary>商品概要</summary>
		public string Outline
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_OUTLINE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_OUTLINE] = value; }
		}
		/// <summary>モバイル商品概要HTML区分</summary>
		public string OutlineKbnMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_OUTLINE_KBN_MOBILE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_OUTLINE_KBN_MOBILE] = value; }
		}
		/// <summary>モバイル商品概要</summary>
		public string OutlineMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_OUTLINE_MOBILE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_OUTLINE_MOBILE] = value; }
		}
		/// <summary>商品詳細１HTML区分</summary>
		public string DescDetailKbn1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1] = value; }
		}
		/// <summary>商品詳細１</summary>
		public string DescDetail1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL1]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL1] = value; }
		}
		/// <summary>商品詳細2HTML区分</summary>
		public string DescDetailKbn2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2] = value; }
		}
		/// <summary>商品詳細2</summary>
		public string DescDetail2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL2]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL2] = value; }
		}
		/// <summary>商品詳細3HTML区分</summary>
		public string DescDetailKbn3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN3]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN3] = value; }
		}
		/// <summary>商品詳細3</summary>
		public string DescDetail3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL3]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL3] = value; }
		}
		/// <summary>商品詳細4HTML区分</summary>
		public string DescDetailKbn4
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN4]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN4] = value; }
		}
		/// <summary>商品詳細4</summary>
		public string DescDetail4
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL4]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL4] = value; }
		}
		/// <summary>返品交換文言</summary>
		public string ReturnExchangeMessage
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE] = value; }
		}
		/// <summary>モバイル商品詳細１HTML区分</summary>
		public string DescDetailKbn1Mobile
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1_MOBILE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1_MOBILE] = value; }
		}
		/// <summary>モバイル商品詳細１</summary>
		public string DescDetail1Mobile
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL1_MOBILE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL1_MOBILE] = value; }
		}
		/// <summary>モバイル商品詳細2HTML区分</summary>
		public string DescDetailKbn2Mobile
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2_MOBILE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2_MOBILE] = value; }
		}
		/// <summary>モバイル商品詳細2</summary>
		public string DescDetail2Mobile
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL2_MOBILE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL2_MOBILE] = value; }
		}
		/// <summary>モバイル返品交換文言</summary>
		public string ReturnExchangeMessageMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE_MOBILE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE_MOBILE] = value; }
		}
		/// <summary>備考</summary>
		public string Note
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NOTE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_NOTE] = value; }
		}
		/// <summary>商品表示価格</summary>
		public decimal DisplayPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_PRICE] = value; }
		}
		/// <summary>商品表示特別価格</summary>
		public decimal? DisplaySpecialPrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE] = value; }
		}
		/// <summary>税込フラグ</summary>
		public string TaxIncludedFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG] = value; }
		}
		/// <summary>税率</summary>
		/// <remarks>使用しない</remarks>  
		public decimal TaxRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PRODUCT_TAX_RATE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_TAX_RATE] = value; }
		}
		/// <summary>税計算方法</summary>
		public string TaxRoundType
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_TAX_ROUND_TYPE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_TAX_ROUND_TYPE] = value; }
		}
		/// <summary>税込販売価格</summary>
		public decimal PricePretax
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PRODUCT_PRICE_PRETAX]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_PRICE_PRETAX] = value; }
		}
		/// <summary>配送料</summary>
		public decimal? PriceShipping
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_PRICE_SHIPPING] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCT_PRICE_SHIPPING];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_PRICE_SHIPPING] = value; }
		}
		/// <summary>配送料種別</summary>
		public string ShippingType
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SHIPPING_TYPE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_SHIPPING_TYPE] = value; }
		}
		/// <summary>配送サイズ区分</summary>
		public string ShippingSizeKbn
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN] = value; }
		}
		/// <summary>商品原価</summary>
		public decimal? PriceCost
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_PRICE_COST] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCT_PRICE_COST];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_PRICE_COST] = value; }
		}
		/// <summary>付与ポイント区分１</summary>
		public string PointKbn1
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_POINT_KBN1] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_PRODUCT_POINT_KBN1];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_POINT_KBN1] = value; }
		}
		/// <summary>付与ポイント１</summary>
		public decimal Point1
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PRODUCT_POINT1]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_POINT1] = value; }
		}
		/// <summary>付与ポイント区分２</summary>
		public string PointKbn2
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_POINT_KBN2] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_PRODUCT_POINT_KBN2];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_POINT_KBN2] = value; }
		}
		/// <summary>付与ポイント２</summary>
		public decimal Point2
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PRODUCT_POINT2]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_POINT2] = value; }
		}
		/// <summary>付与ポイント区分３</summary>
		public string PointKbn3
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_POINT_KBN3] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_PRODUCT_POINT_KBN3];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_POINT_KBN3] = value; }
		}
		/// <summary>付与ポイント３</summary>
		public decimal Point3
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PRODUCT_POINT3]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_POINT3] = value; }
		}
		/// <summary>キャンペーン期間(FROM)</summary>
		public DateTime? CampaignFrom
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_CAMPAIGN_FROM] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_PRODUCT_CAMPAIGN_FROM];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_CAMPAIGN_FROM] = value; }
		}
		/// <summary>キャンペーン期間(TO)</summary>
		public DateTime? CampaignTo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_CAMPAIGN_TO] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_PRODUCT_CAMPAIGN_TO];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_CAMPAIGN_TO] = value; }
		}
		/// <summary>キャンペーン付与ポイント区分</summary>
		public string CampaignPointKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CAMPAIGN_POINT_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_CAMPAIGN_POINT_KBN] = value; }
		}
		/// <summary>キャンペーン付与ポイント</summary>
		public int CampaignPoint
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCT_CAMPAIGN_POINT]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_CAMPAIGN_POINT] = value; }
		}
		/// <summary>表示期間(FROM)</summary>
		public DateTime DisplayFrom
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_FROM]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_FROM] = value; }
		}
		/// <summary>表示期間(TO)</summary>
		public DateTime? DisplayTo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_TO] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_TO];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_TO] = value; }
		}
		/// <summary>販売期間(FROM)</summary>
		public DateTime SellFrom
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCT_SELL_FROM]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_SELL_FROM] = value; }
		}
		/// <summary>販売期間(TO)</summary>
		public DateTime? SellTo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_SELL_TO] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_PRODUCT_SELL_TO];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_SELL_TO] = value; }
		}
		/// <summary>販売期間前表示フラグ</summary>
		public string BeforeSaleDisplayKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_BEFORE_SALE_DISPLAY_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_BEFORE_SALE_DISPLAY_KBN] = value; }
		}
		/// <summary>販売期間後表示フラグ</summary>
		public string AfterSaleDisplayKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_AFTER_SALE_DISPLAY_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_AFTER_SALE_DISPLAY_KBN] = value; }
		}
		/// <summary>販売可能数</summary>
		public int MaxSellQuantity
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY] = value; }
		}
		/// <summary>在庫管理方法</summary>
		public string StockManagementKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN] = value; }
		}
		/// <summary>在庫表示区分</summary>
		public string StockDispKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_STOCK_DISP_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_STOCK_DISP_KBN] = value; }
		}
		/// <summary>商品在庫文言ID</summary>
		public string StockMessageId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID] = value; }
		}
		/// <summary>紹介URL</summary>
		public string Url
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_URL]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_URL] = value; }
		}
		/// <summary>問い合わせ用メールアドレス</summary>
		public string InquireEmail
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_INQUIRE_EMAIL]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_INQUIRE_EMAIL] = value; }
		}
		/// <summary>問い合わせ用電話番号</summary>
		public string InquireTel
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_INQUIRE_TEL]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_INQUIRE_TEL] = value; }
		}
		/// <summary>検索時表示区分</summary>
		public string DisplayKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_KBN] = value; }
		}
		/// <summary>カテゴリID1</summary>
		public string CategoryId1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID1]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID1] = value; }
		}
		/// <summary>カテゴリID2</summary>
		public string CategoryId2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID2]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID2] = value; }
		}
		/// <summary>カテゴリID3</summary>
		public string CategoryId3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID3]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID3] = value; }
		}
		/// <summary>カテゴリID4</summary>
		public string CategoryId4
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID4]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID4] = value; }
		}
		/// <summary>カテゴリID5</summary>
		public string CategoryId5
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID5]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID5] = value; }
		}
		/// <summary>関連商品ID1（クロスセル）</summary>
		public string RelatedProductIdCs1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1] = value; }
		}
		/// <summary>関連商品ID2（クロスセル）</summary>
		public string RelatedProductIdCs2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS2]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS2] = value; }
		}
		/// <summary>関連商品ID3（クロスセル）</summary>
		public string RelatedProductIdCs3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS3]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS3] = value; }
		}
		/// <summary>関連商品ID4（クロスセル）</summary>
		public string RelatedProductIdCs4
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS4]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS4] = value; }
		}
		/// <summary>関連商品ID5（クロスセル）</summary>
		public string RelatedProductIdCs5
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS5]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS5] = value; }
		}
		/// <summary>関連商品ID1（アップセル）</summary>
		public string RelatedProductIdUs1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1] = value; }
		}
		/// <summary>関連商品ID2（アップセル）</summary>
		public string RelatedProductIdUs2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US2]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US2] = value; }
		}
		/// <summary>関連商品ID3（アップセル）</summary>
		public string RelatedProductIdUs3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US3]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US3] = value; }
		}
		/// <summary>関連商品ID4（アップセル）</summary>
		public string RelatedProductIdUs4
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US4]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US4] = value; }
		}
		/// <summary>関連商品ID5（アップセル）</summary>
		public string RelatedProductIdUs5
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US5]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US5] = value; }
		}
		/// <summary>商品画像名ヘッダ</summary>
		public string ImageHead
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_IMAGE_HEAD]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_IMAGE_HEAD] = value; }
		}
		/// <summary>モバイル商品画像名</summary>
		public string ImageMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_IMAGE_MOBILE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_IMAGE_MOBILE] = value; }
		}
		/// <summary>アイコンフラグ1</summary>
		public string IconFlg1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG1]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG1] = value; }
		}
		/// <summary>アイコン有効期限1</summary>
		public DateTime? IconTermEnd1
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END1] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END1];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END1] = value; }
		}
		/// <summary>アイコンフラグ2</summary>
		public string IconFlg2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG2]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG2] = value; }
		}
		/// <summary>アイコン有効期限2</summary>
		public DateTime? IconTermEnd2
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END2] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END2];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END2] = value; }
		}
		/// <summary>アイコンフラグ3</summary>
		public string IconFlg3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG3]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG3] = value; }
		}
		/// <summary>アイコン有効期限3</summary>
		public DateTime? IconTermEnd3
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END3] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END3];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END3] = value; }
		}
		/// <summary>アイコンフラグ4</summary>
		public string IconFlg4
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG4]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG4] = value; }
		}
		/// <summary>アイコン有効期限4</summary>
		public DateTime? IconTermEnd4
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END4] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END4];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END4] = value; }
		}
		/// <summary>アイコンフラグ5</summary>
		public string IconFlg5
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG5]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG5] = value; }
		}
		/// <summary>アイコン有効期限5</summary>
		public DateTime? IconTermEnd5
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END5] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END5];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END5] = value; }
		}
		/// <summary>アイコンフラグ6</summary>
		public string IconFlg6
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG6]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG6] = value; }
		}
		/// <summary>アイコン有効期限6</summary>
		public DateTime? IconTermEnd6
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END6] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END6];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END6] = value; }
		}
		/// <summary>アイコンフラグ7</summary>
		public string IconFlg7
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG7]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG7] = value; }
		}
		/// <summary>アイコン有効期限7</summary>
		public DateTime? IconTermEnd7
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END7] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END7];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END7] = value; }
		}
		/// <summary>アイコンフラグ8</summary>
		public string IconFlg8
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG8]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG8] = value; }
		}
		/// <summary>アイコン有効期限8</summary>
		public DateTime? IconTermEnd8
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END8] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END8];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END8] = value; }
		}
		/// <summary>アイコンフラグ9</summary>
		public string IconFlg9
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG9]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG9] = value; }
		}
		/// <summary>アイコン有効期限9</summary>
		public DateTime? IconTermEnd9
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END9] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END9];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END9] = value; }
		}
		/// <summary>アイコンフラグ10</summary>
		public string IconFlg10
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG10]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG10] = value; }
		}
		/// <summary>アイコン有効期限10</summary>
		public DateTime? IconTermEnd10
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END10] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END10];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END10] = value; }
		}
		/// <summary>モバイル表示フラグ</summary>
		public string MobileDispFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_MOBILE_DISP_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_MOBILE_DISP_FLG] = value; }
		}
		/// <summary>複数バリエーション使用フラグ</summary>
		public string UseVariationFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_USE_VARIATION_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_USE_VARIATION_FLG] = value; }
		}
		/// <summary>予約商品フラグ</summary>
		public string ReservationFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RESERVATION_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_RESERVATION_FLG] = value; }
		}
		/// <summary>定期購入フラグ</summary>
		public string FixedPurchaseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] = value; }
		}
		/// <summary>定期商品購入制限チェックフラグ</summary>
		public string CheckFixedProductOrderFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG] = value; }
		}
		/// <summary>通常商品購入制限チェックフラグ</summary>
		public string CheckProductOrderFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG] = value; }
		}
		/// <summary>モール連携済フラグ</summary>
		public string MallCooperatedFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_MALL_COOPERATED_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_MALL_COOPERATED_FLG] = value; }
		}
		/// <summary>カート投入URL制限フラグ</summary>
		public string AddCartUrlLimitFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCT_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCT_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_LAST_CHANGED] = value; }
		}
		/// <summary>検索キーワード</summary>
		public string SearchKeyword
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SEARCH_KEYWORD]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_SEARCH_KEYWORD] = value; }
		}
		/// <summary>会員ランク割引対象フラグ</summary>
		public string MemberRankDiscountFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG] = value; }
		}
		/// <summary>閲覧可能会員ランク</summary>
		public string DisplayMemberRank
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK] = value; }
		}
		/// <summary>購入可能会員ランク</summary>
		public string BuyableMemberRank
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK] = value; }
		}
		/// <summary>Googleショッピング連携フラグ</summary>
		public string GoogleShoppingFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG] = value; }
		}
		/// <summary>商品付帯情報設定</summary>
		public string ProductOptionSettings
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_OPTION_SETTINGS]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_OPTION_SETTINGS] = value; }
		}
		/// <summary>再入荷通知メール有効フラグ</summary>
		public string ArrivalMailValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG] = value; }
		}
		/// <summary>販売開始通知メール有効フラグ</summary>
		public string ReleaseMailValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG] = value; }
		}
		/// <summary>再販売通知メール有効フラグ</summary>
		public string ResaleMailValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG] = value; }
		}
		/// <summary>PC用商品詳細バリエーション選択方法</summary>
		public string SelectVariationKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN] = value; }
		}
		/// <summary>モバイル用商品バリエーション選択方法</summary>
		public string SelectVariationKbnMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN_MOBILE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN_MOBILE] = value; }
		}
		/// <summary>ブランドID1</summary>
		public string BrandId1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID1]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID1] = value; }
		}
		/// <summary>ブランドID2</summary>
		public string BrandId2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID2]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID2] = value; }
		}
		/// <summary>ブランドID3</summary>
		public string BrandId3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID3]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID3] = value; }
		}
		/// <summary>ブランドID4</summary>
		public string BrandId4
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID4]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID4] = value; }
		}
		/// <summary>ブランドID5</summary>
		public string BrandId5
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID5]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID5] = value; }
		}
		/// <summary>外部レコメンド利用フラグ</summary>
		public string UseRecommendFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG] = value; }
		}
		/// <summary>ギフト購入フラグ</summary>
		public string GiftFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_GIFT_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_GIFT_FLG] = value; }
		}
		/// <summary>年齢制限フラグ</summary>
		public string AgeLimitFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_AGE_LIMIT_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_AGE_LIMIT_FLG] = value; }
		}
		/// <summary>配送料金複数個無料フラグ</summary>
		public string PluralShippingPriceFreeFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG] = value; }
		}
		/// <summary>デジタルコンテンツ商品フラグ</summary>
		public string DigitalContentsFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG] = value; }
		}
		/// <summary>ダウンロードURL</summary>
		public string DownloadUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DOWNLOAD_URL]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DOWNLOAD_URL] = value; }
		}
		/// <summary>販売期間表示フラグ</summary>
		public string DisplaySellFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SELL_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SELL_FLG] = value; }
		}
		/// <summary>表示優先順</summary>
		public int DisplayPriority
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_PRIORITY]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_PRIORITY] = value; }
		}
		/// <summary>決済利用不可</summary>
		public string LimitedPaymentIds
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS] = value; }
		}
		/// <summary>定期初回購入価格</summary>
		public decimal? FixedPurchaseFirsttimePrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE] = value; }
		}
		/// <summary>定期購入価格</summary>
		public decimal? FixedPurchasePrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE] = value; }
		}
		/// <summary>同梱商品明細表示フラグ</summary>
		public string BundleItemDisplayType
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE] = value; }
		}
		/// <summary>商品区分</summary>
		public string ProductType
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_TYPE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_TYPE] = value; }
		}
		/// <summary>利用不可配送周期月</summary>
		public string LimitedFixedPurchaseKbn1Setting
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING] = value; }
		}
		/// <summary>利用不可配送周期日</summary>
		public string LimitedFixedPurchaseKbn3Setting
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING] = value; }
		}
		/// <summary>商品連携ID6</summary>
		public string CooperationId6
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID6]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID6] = value; }
		}
		/// <summary>商品連携ID7</summary>
		public string CooperationId7
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID7]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID7] = value; }
		}
		/// <summary>商品連携ID8</summary>
		public string CooperationId8
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID8]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID8] = value; }
		}
		/// <summary>商品連携ID9</summary>
		public string CooperationId9
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID9]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID9] = value; }
		}
		/// <summary>商品連携ID10</summary>
		public string CooperationId10
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID10]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID10] = value; }
		}
		/// <summary>＆mallの予約商品フラグ</summary>
		public string AndmallReservationFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG] = value; }
		}
		/// <summary>定期会員限定フラグ（閲覧）</summary>
		public string DisplayOnlyFixedPurchaseMemberFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG] = value; }
		}
		/// <summary>定期会員限定フラグ（購入）</summary>
		public string SellOnlyFixedPurchaseMemberFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG] = value; }
		}
		/// <summary>商品重量(g）</summary>
		public int ProductWeightGram
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM] = value; }
		}
		/// <summary>税率カテゴリID</summary>
		public string TaxCategoryId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_TAX_CATEGORY_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_TAX_CATEGORY_ID] = value; }
		}
		/// <summary>定期解約可能回数</summary>
		public int FixedPurchasedCancelableCount
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT] = value; }
		}
		/// <summary>定期購入利用不可ユーザー管理レベル</summary>
		public string FixedPurchasedLimitedUserLevelIds
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS] = value; }
		}
		/// <summary>定期購入2回目以降配送商品ID</summary>
		public string FixedPurchaseNextShippingProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID] = value; }
		}
		/// <summary>定期購入2回目以降配送商品バリエーションID</summary>
		public string FixedPurchaseNextShippingVariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID] = value; }
		}
		/// <summary>定期購入2回目以降配送商品注文個数</summary>
		public int FixedPurchaseNextShippingItemQuantity
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY] = value; }
		}
		/// <summary>定期購入スキップ制限回数</summary>
		public int? FixedPurchaseLimitedSkippedCount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT] = value; }
		}
		/// <summary>定期購入2回目以降配送商品 定期購入区分</summary>
		public string NextShippingItemFixedPurchaseKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_KBN] = value; }
		}
		/// <summary>定期購入2回目以降配送商品 定期購入設定</summary>
		public string NextShippingItemFixedPurchaseSetting
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING] = value; }
		}
		/// <summary>レコメンド用商品ID</summary>
		public string RecommendProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RECOMMEND_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_RECOMMEND_PRODUCT_ID] = value; }
		}
		/// <summary>商品サイズ係数</summary>
		public int ProductSizeFactor
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR] = value; }
		}
		/// <summary>利用不可配送周期週</summary>
		public string LimitedFixedPurchaseKbn4Setting
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING] = value; }
		}
		/// <summary>商品カラーID</summary>
		public string ProductColorId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID] = value; }
		}
		/// <summary>頒布会購入フラグ</summary>
		public string SubscriptionBoxFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG] = value; }
		}
		/// <summary>会員ランクポイント付与率除外フラグ</summary>
		public string MemberRankPointExcludeFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_MEMBER_RANK_POINT_EXCLUDE_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_MEMBER_RANK_POINT_EXCLUDE_FLG] = value; }
		}
		/// <summary>店舗受取可能フラグ</summary>
		public string StorePickupFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_STOREPICKUP_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_STOREPICKUP_FLG] = value; }
		}
		/// <summary>配送料無料適用外フラグ</summary>
		public string ExcludeFreeShippingFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG] = value; }
		}
		#endregion
	}
}
