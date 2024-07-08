/*
=========================================================================================================
  Module      : 商品マスタモデル (ProductModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.Common.Util;
using w2.Domain.MallExhibitsConfig;
using w2.Domain.ProductExtend;
using w2.Domain.ProductFixedPurchaseDiscountSetting;
using w2.Domain.ProductPrice;
using w2.Domain.ProductStock;
using w2.Domain.ProductTag;
using w2.Domain.ProductVariation;

namespace w2.Domain.Product
{
	/// <summary>
	/// 商品マスタモデル
	/// </summary>
	public partial class ProductModel
	{
		#region メソッド
		/// <summary>
		/// 利用不可の決済種別か
		/// </summary>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>利用不可か</returns>
		public bool IsLimitedPaymentId(string paymentId)
		{
			var isLimited = this.LimitedPaymentIdList.Any(p => p == paymentId);
			return isLimited;
		}

		/// <summary>
		/// 利用不可の定期購入配送周期月か
		/// </summary>
		/// <param name="month">月</param>
		/// <returns>利用不可か</returns>
		public bool IsLimitedFixedPurchaseSetting1(string month)
		{
			var isLimited = this.LimitedFixedPurchaseKbn1Setting.Split(',').Contains(month);
			return isLimited;
		}

		/// <summary>
		/// 利用不可の定期購入配送周期日か
		/// </summary>
		/// <param name="intervalDay">日</param>
		/// <returns>利用不可か</returns>
		public bool IsLimitedFixedPurchaseSetting3(string intervalDay)
		{
			var isLimited = this.LimitedFixedPurchaseKbn3Setting.Split(',').Contains(intervalDay);
			return isLimited;
		}

		/// <summary>
		/// 利用不可の定期購入配送周期週か
		/// </summary>
		/// <param name="intervalWeek">週</param>
		/// <returns>利用不可か</returns>
		public bool IsLimitedFixedPurchaseSetting4(string intervalWeek)
		{
			var isLimited = this.LimitedFixedPurchaseKbn4Setting.Split(',').Contains(intervalWeek);
			return isLimited;
		}
		#endregion

		#region プロパティ
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
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER] = value; }
		}
		/// <summary>商品セールID</summary>
		public string ProductsaleId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]); }
			set { this.DataSource[Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID] = value; }
		}
		/// <summary>商品セール価格</summary>
		public decimal SalePrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE] = value; }
		}
		/// <summary>セール情報が有効か（有効期限、有効フラグ考慮）</summary>
		public string Validity
		{
			get { return (string)this.DataSource["validity"]; }
			set { this.DataSource["validity"] = value; }
		}
		/// <summary>商品在庫数</summary>
		public int Stock
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK] = value; }
		}
		/// <summary>商品在庫安全基準</summary>
		public int StockAlert
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT] = value; }
		}
		/// <summary>実在庫数</summary>
		public int Realstock
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK] = value; }
		}
		/// <summary>実在庫数B</summary>
		public int RealstockB
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B] = value; }
		}
		/// <summary>実在庫数C</summary>
		public int RealstockC
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C] = value; }
		}
		/// <summary>引当済実在庫数</summary>
		public int RealstockReserved
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED] = value; }
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
		public string StockMessage7
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE7]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE7] = value; }
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
		/// <summary>配送種別ID</summary>
		public string ShippingId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID] = value; }
		}
		/// <summary>定期購入設定可能フラグ</summary>
		public string ShippingFixedPurchaseFlg
		{
			get { return (string)this.DataSource["shipping_fixed_purchase_flg"]; }
			set { this.DataSource["shipping_fixed_purchase_flg"] = value; }
		}
		/// <summary>のし利用フラグ</summary>
		public string WrappingPaperFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG] = value; }
		}
		/// <summary>のし種類</summary>
		public string WrappingPaperTypes
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_TYPES]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_TYPES] = value; }
		}
		/// <summary>包装利用フラグ</summary>
		public string WrappingBagFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG] = value; }
		}
		/// <summary>包装種類</summary>
		public string WrappingBagTypes
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_TYPES]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_TYPES] = value; }
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
		/// <summary>会員ランク価格</summary>
		public decimal? MemberRankPrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE] = value; }

		}
		/// <summary>会員ランク価格(バリエーション単位)</summary>
		public decimal? MemberRankPriceVariation
		{
			get
			{
				if (this.DataSource["member_rank_price_variation"] == DBNull.Value) return null;
				return (decimal?)this.DataSource["member_rank_price_variation"];
			}
			set { this.DataSource["member_rank_price_variation"] = value; }
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
		/// <summary>選択された索引値1</summary>
		public int SelectedIndex1 { get; set; }
		/// <summary>選択された索引値2</summary>
		public int SelectedIndex2 { get; set; }
		/// <summary>AmazonSKU</summary>
		public string AmazonSKU { get; set; }
		/// <summary>出荷作業日数（Amazon在庫連携に使用）</summary>
		public int FulfillmentLatency { get; set; }
		/// <summary>＆mallの予約商品フラグ</summary>
		public string VariationAndmallReservationFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG] = value; }
		}
		/// <summary>決済利用不可リスト</summary>
		public string[] LimitedPaymentIdList
		{
			get { return this.LimitedPaymentIds.Split(','); }
		}
		/// <summary>在庫管理しない商品？</summary>
		public bool IsStockUnmanaged
		{
			get { return (this.StockManagementKbn == Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED); }
		}
		/// <summary>メール便可？</summary>
		public bool CanShippingMail
		{
			get { return (this.ShippingSizeKbn == Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_MAIL); }
		}
		/// <summary>バリエーションが存在するか？</summary>
		public bool HasProductVariation
		{
			get { return (this.UseVariationFlg == Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE); }
		}
		/// <summary>バリエーションごとの商品重量(g)</summary>
		public int VariationWeightGram
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM]; }
		}

		/// <summary>購入可能？</summary>
		public bool IsBuyable
		{
			get
			{
				return ((this.ValidFlg == Constants.FLG_PRODUCT_VALID_FLG_VALID)
					&& (this.SellTo >= DateTime.Now || (this.SellTo == null))
					&& (this.SellFrom <= DateTime.Now));
			}
		}

		/// <summary>Product stock stock</summary>
		public int ProductStockStock
		{
			get { return (int)this.DataSource[Constants.FLG_PRODUCTSTOCK_PREFIX + Constants.FIELD_PRODUCTSTOCK_STOCK]; }
		}
		/// <summary>Product stock real stock</summary>
		public int ProductStockRealstock
		{
			get { return (int)this.DataSource[Constants.FLG_PRODUCTSTOCK_PREFIX + Constants.FIELD_PRODUCTSTOCK_REALSTOCK]; }
		}
		/// <summary>Product stock real stock B</summary>
		public int ProductStockRealstockB
		{
			get { return (int)this.DataSource[Constants.FLG_PRODUCTSTOCK_PREFIX + Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B]; }
		}
		/// <summary>Product stock real stock C</summary>
		public int ProductStockRealstockC
		{
			get { return (int)this.DataSource[Constants.FLG_PRODUCTSTOCK_PREFIX + Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C]; }
		}
		/// <summary>Product stock real stock reserved</summary>
		public int ProductStockRealstockReserved
		{
			get { return (int)this.DataSource[Constants.FLG_PRODUCTSTOCK_PREFIX + Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED]; }
		}
		/// <summary>Product stock date created</summary>
		public DateTime ProductStockDateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FLG_PRODUCTSTOCK_PREFIX + Constants.FIELD_PRODUCTSTOCK_DATE_CREATED]; }
		}
		/// <summary>Product stock date changed</summary>
		public DateTime ProductStockDateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FLG_PRODUCTSTOCK_PREFIX + Constants.FIELD_PRODUCTSTOCK_DATE_CHANGED]; }
		}
		/// <summary>Product stock message name</summary>
		public string ProductStockMessageName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_NAME]); }
		}
		/// <summary>Shop shipping name</summary>
		public string ShopShippingName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME]); }
		}
		/// <summary>Product category name 1</summary>
		public string ProductCategoryName1
		{
			get { return StringUtility.ToEmpty(this.DataSource["category_name1"]); }
		}
		/// <summary>Product category name 2</summary>
		public string ProductCategoryName2
		{
			get { return StringUtility.ToEmpty(this.DataSource["category_name2"]); }
		}
		/// <summary>Product category name 3</summary>
		public string ProductCategoryName3
		{
			get { return StringUtility.ToEmpty(this.DataSource["category_name3"]); }
		}
		/// <summary>Product category name 4</summary>
		public string ProductCategoryName4
		{
			get { return StringUtility.ToEmpty(this.DataSource["category_name4"]); }
		}
		/// <summary>Product category name 5</summary>
		public string ProductCategoryName5
		{
			get { return StringUtility.ToEmpty(this.DataSource["category_name5"]); }
		}
		/// <summary>Product brand name 1</summary>
		public string ProductBrandName1
		{
			get { return StringUtility.ToEmpty(this.DataSource["brand_name1"]); }
		}
		/// <summary>Product brand name 2</summary>
		public string ProductBrandName2
		{
			get { return StringUtility.ToEmpty(this.DataSource["brand_name2"]); }
		}
		/// <summary>Product brand name 3</summary>
		public string ProductBrandName3
		{
			get { return StringUtility.ToEmpty(this.DataSource["brand_name3"]); }
		}
		/// <summary>Product brand name 4</summary>
		public string ProductBrandName4
		{
			get { return StringUtility.ToEmpty(this.DataSource["brand_name4"]); }
		}
		/// <summary>Product brand name 5</summary>
		public string ProductBrandName5
		{
			get { return StringUtility.ToEmpty(this.DataSource["brand_name5"]); }
		}
		/// <summary>Product tax category name</summary>
		public string ProductTaxCategoryName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_TAX_CATEGORY_NAME]); }
		}
		/// <summary>Product prices</summary>
		public ProductPriceModel[] ProductPrices
		{
			get { return (ProductPriceModel[])this.DataSource[Constants.FIELD_PRODUCT_PRODUCTPRICE_EXTEND]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCTPRICE_EXTEND] = value; }
		}
		/// <summary>Has product prices</summary>
		public bool HasProductPrices
		{
			get { return ((this.ProductPrices != null) && (this.ProductPrices.Length != 0)); }
		}
		/// <summary>Product variations</summary>
		public ProductVariationModel[] ProductVariations
		{
			get { return (ProductVariationModel[])this.DataSource[Constants.FIELD_PRODUCT_PRODUCTVARIATION_EXTEND]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCTVARIATION_EXTEND] = value; }
		}
		/// <summary>Has product variations</summary>
		public bool HasProductVariations
		{
			get { return ((this.ProductVariations != null) && (this.ProductVariations.Length != 0)); }
		}
		/// <summary>Product tag</summary>
		public ProductTagModel ProductTag
		{
			get { return (ProductTagModel)this.DataSource[Constants.FIELD_PRODUCT_PRODUCTTAG_EXTEND]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCTTAG_EXTEND] = value; }
		}
		/// <summary>Has product tag</summary>
		public bool HasProductTag
		{
			get { return (this.ProductTag != null); }
		}
		/// <summary>Product extend</summary>
		public ProductExtendModel ProductExtend
		{
			get { return (ProductExtendModel)this.DataSource[Constants.FIELD_PRODUCT_PRODUCTEXTEND_EXTEND]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCTEXTEND_EXTEND] = value; }
		}
		/// <summary>Has product extend</summary>
		public bool HasProductExtend
		{
			get { return (this.ProductExtend != null); }
		}
		/// <summary>Mall exhibits config</summary>
		public MallExhibitsConfigModel MallExhibitsConfig
		{
			get { return (MallExhibitsConfigModel)this.DataSource[Constants.FIELD_PRODUCT_MALLEXHIBITSCONFIG_EXTEND]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_MALLEXHIBITSCONFIG_EXTEND] = value; }
		}
		/// <summary>Has mall exhibits config</summary>
		public bool HasMallExhibitsConfig
		{
			get { return (this.MallExhibitsConfig != null); }
		}
		/// <summary>Product stocks</summary>
		public ProductStockModel[] ProductStocks
		{
			get { return (ProductStockModel[])this.DataSource[Constants.FIELD_PRODUCT_PRODUCTSTOCK_EXTEND]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCTSTOCK_EXTEND] = value; }
		}
		/// <summary>Has product stocks</summary>
		public bool HasProductStocks
		{
			get { return ((this.ProductStocks != null) && (this.ProductStocks.Length != 0)); }
		}
		/// <summary>Product fixed purchase discount settings</summary>
		public ProductFixedPurchaseDiscountSettingModel[] ProductFixedPurchaseDiscountSettings
		{
			get { return (ProductFixedPurchaseDiscountSettingModel[])this.DataSource[Constants.FIELD_PRODUCT_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_EXTEND]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_EXTEND] = value; }
		}
		/// <summary>Has product fixed purchase discount settings</summary>
		public bool HasProductFixedPurchaseDiscountSettings
		{
			get { return ((this.ProductFixedPurchaseDiscountSettings != null) && (this.ProductFixedPurchaseDiscountSettings.Length != 0)); }
		}
		#endregion
	}
}
