/*
=========================================================================================================
  Module      : 返品交換注文商品情報クラス(ReturnOrderItem.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.App.Common.Global;
using w2.App.Common.Option;
using w2.App.Common.Product;
using w2.App.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 返品交換注文商品情報クラス
	/// </summary>
	[Serializable]
	public class ReturnOrderItem
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ReturnOrderItem()
		{
			this.DataSource = new Hashtable();
			this.ReturnStatus = string.Empty;
			this.ShopId = string.Empty;
			this.ProductId = string.Empty;
			this.VId = string.Empty;
			this.SupplierId = string.Empty;
			this.ProductName = string.Empty;
			this.ProductNameKana = string.Empty;
			this.ProductPrice = 0m;
			this.ItemPriceTax = 0m;
			this.ItemQuantity = 1;
			this.ProductSaleId = string.Empty;
			this.NoveltyId = string.Empty;
			this.RecommendId = string.Empty;
			this.BrandId = string.Empty;
			this.DownloadUrl = string.Empty;
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
			this.OrderSetPromotionNo = string.Empty;
			this.FixedPurchaseProductFlg = string.Empty;
			this.ProductPricePretax = 0m;
			this.ProductTaxIncludedFlg = string.Empty;
			this.ProductTaxRate = 0m;
			this.ProductTaxRoundType = string.Empty;
			this.ProductOptionValue = string.Empty;
			this.ItemReturnExchangeKbn = string.Empty;
			this.ProductBundleId = string.Empty;
			this.BundleItemDisplayType = string.Empty;
			this.OrderShippingNo = string.Empty;
			this.ShippingName = string.Empty;
			this.ShippingName1 = string.Empty;
			this.ShippingName2 = string.Empty;
			this.ShippingNameKana = string.Empty;
			this.ShippingNameKana1 = string.Empty;
			this.ShippingNameKana2 = string.Empty;
			this.ShippingCountryIsoCode = string.Empty;
			this.ShippingCountryName = string.Empty;
			this.ShippingZip = string.Empty;
			this.ShippingAddr1 = string.Empty;
			this.ShippingAddr2 = string.Empty;
			this.ShippingAddr3 = string.Empty;
			this.ShippingAddr4 = string.Empty;
			this.ShippingAddr5 = string.Empty;
			this.ShippingCompanyName = string.Empty;
			this.ShippingCompanyPostName = string.Empty;
			this.ShippingTel1 = string.Empty;
			this.ShippingCheckNo = string.Empty;
			this.SenderName = string.Empty;
			this.SenderNameKana = string.Empty;
			this.SenderCountryIsoCode = string.Empty;
			this.SenderCountryName = string.Empty;
			this.SenderZip = string.Empty;
			this.SenderAddr1 = string.Empty;
			this.SenderAddr2 = string.Empty;
			this.SenderAddr3 = string.Empty;
			this.SenderAddr4 = string.Empty;
			this.SenderAddr5 = string.Empty;
			this.SenderCompanyName = string.Empty;
			this.SenderCompanyPostName = string.Empty;
			this.SenderTel1 = string.Empty;
			this.WrappingPaperType = string.Empty;
			this.WrappingPaperName = string.Empty;
			this.WrappingBagType = string.Empty;
			this.DiscountedPrice = 0m;
			this.SubscriptionBoxCourseId = string.Empty;
			this.SubscriptionBoxFixedAmount = null;
		}

		/// <summary>
		/// 商品付帯情報一覧を取得する
		/// </summary>
		/// <returns>商品付帯情報一覧</returns>
		/// <remarks>
		/// 商品付帯情報の選択値をもとに商品付帯情報一覧のデフォルト値を設定して取得している
		/// </remarks>
		public ProductOptionSettingList GetProductOptionSettingList()
		{
			var productOptionListSetting = new ProductOptionSettingList(this.ShopId, this.ProductId);
			productOptionListSetting.SetDefaultValueFromProductOptionTexts(this.ProductOptionValue);
			return productOptionListSetting;
		}

		#region プロパティ
		/// <summary>データソース</summary>
		public Hashtable DataSource { get; set; }
		/// <summary>返品交換ステータス</summary>
		public string ReturnStatus
		{
			get { return (string)this.DataSource[Constants.CONST_ORDER_RETURN_STATUS]; }
			set { this.DataSource[Constants.CONST_ORDER_RETURN_STATUS] = value; }
		}
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_ID] = value; }
		}
		/// <summary>商品IDを含まないバリエーションID</summary>
		public string VId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_V_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_V_ID] = value; }
		}
		/// <summary>サプライヤID</summary>
		public string SupplierId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_SUPPLIER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_SUPPLIER_ID] = value; }
		}
		/// <summary>商品名</summary>
		public string ProductName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_NAME] = value; }
		}
		/// <summary>商品名かな</summary>
		public string ProductNameKana
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_NAME_KANA]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_NAME_KANA] = value; }
		}
		/// <summary>商品単価</summary>
		public decimal ProductPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE] = value; }
		}
		/// <summary>明細金額（税金額）</summary>
		public decimal ItemPriceTax
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_PRICE_TAX]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_PRICE_TAX] = value; }
		}
		/// <summary>注文数</summary>
		public int ItemQuantity
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_QUANTITY] = value; }
		}
		/// <summary>商品セールID</summary>
		public string ProductSaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCTSALE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCTSALE_ID] = value; }
		}
		/// <summary>ノベルティID</summary>
		public string NoveltyId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_NOVELTY_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_NOVELTY_ID] = value; }
		}
		/// <summary>レコメンドID</summary>
		public string RecommendId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_RECOMMEND_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_RECOMMEND_ID] = value; }
		}
		/// <summary>ブランドID</summary>
		public string BrandId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_BRAND_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_BRAND_ID] = value; }
		}
		/// <summary>ダウンロードURL</summary>
		public string DownloadUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_DOWNLOAD_URL]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_DOWNLOAD_URL] = value; }
		}
		/// <summary>商品連携ID1</summary>
		public string CooperationId1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID1]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID1] = value; }
		}
		/// <summary>商品連携ID2</summary>
		public string CooperationId2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID2]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID2] = value; }
		}
		/// <summary>商品連携ID3</summary>
		public string CooperationId3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID3]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID3] = value; }
		}
		/// <summary>商品連携ID4</summary>
		public string CooperationId4
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID4]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID4] = value; }
		}
		/// <summary>商品連携ID5</summary>
		public string CooperationId5
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID5]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID5] = value; }
		}
		/// <summary>商品連携ID6</summary>
		public string CooperationId6
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID6]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID6] = value; }
		}
		/// <summary>商品連携ID7</summary>
		public string CooperationId7
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID7]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID7] = value; }
		}
		/// <summary>商品連携ID8</summary>
		public string CooperationId8
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID8]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID8] = value; }
		}
		/// <summary>商品連携ID9</summary>
		public string CooperationId9
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID9]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID9] = value; }
		}
		/// <summary>商品連携ID10</summary>
		public string CooperationId10
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID10]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID10] = value; }
		}
		/// <summary>セットプロモーション枝番</summary>
		public string OrderSetPromotionNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] = value; }
		}
		/// <summary>定期商品フラグ</summary>
		public string FixedPurchaseProductFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG] = value; }
		}
		/// <summary>税込販売価格</summary>
		public decimal ProductPricePretax
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_PRETAX]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_PRETAX] = value; }
		}
		/// <summary>税込フラグ</summary>
		public string ProductTaxIncludedFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG] = value; }
		}
		/// <summary>税率</summary>
		public decimal ProductTaxRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE] = value; }
		}
		/// <summary>税計算方法</summary>
		public string ProductTaxRoundType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_ROUND_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_ROUND_TYPE] = value; }
		}
		/// <summary>商品付帯情報選択値</summary>
		public string ProductOptionValue
		{
			get { return (string) this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS] = value; }
		}
		/// <summary>返品交換区分</summary>
		public string ItemReturnExchangeKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN] = value; }
		}
		/// <summary>商品同梱ID</summary>
		public string ProductBundleId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_BUNDLE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_BUNDLE_ID] = value; }
		}
		/// <summary>同梱商品明細表示フラグ</summary>
		public string BundleItemDisplayType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE] = value; }
		}
		/// <summary>配送先枝番</summary>
		public string OrderShippingNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO] = value; }
		}
		/// <summary>配送先氏名</summary>
		public string ShippingName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME] = value; }
		}
		/// <summary>配送先氏名1</summary>
		public string ShippingName1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1] = value; }
		}
		/// <summary>配送先氏名2</summary>
		public string ShippingName2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2] = value; }
		}
		/// <summary>配送先氏名かな</summary>
		public string ShippingNameKana
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA] = value; }
		}
		/// <summary>配送先氏名かな1</summary>
		public string ShippingNameKana1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1] = value; }
		}
		/// <summary>配送先氏名かな2</summary>
		public string ShippingNameKana2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2] = value; }
		}
		/// <summary>配送先住所国ISOコード</summary>
		public string ShippingCountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE] = value; }
		}
		/// <summary>配送先住所国名</summary>
		public string ShippingCountryName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME] = value; }
		}
		/// <summary>郵便番号</summary>
		public string ShippingZip
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP] = value; }
		}
		/// <summary>住所1</summary>
		public string ShippingAddr1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1] = value; }
		}
		/// <summary>住所2</summary>
		public string ShippingAddr2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2] = value; }
		}
		/// <summary>住所3</summary>
		public string ShippingAddr3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3] = value; }
		}
		/// <summary>住所4</summary>
		public string ShippingAddr4
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4] = value; }
		}
		/// <summary>名称記載</summary>
		public string ShippingAddr5
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5] = value; }
		}
		/// <summary>配送先企業名</summary>
		public string ShippingCompanyName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME] = value; }
		}
		/// <summary>配送先部署名</summary>
		public string ShippingCompanyPostName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME] = value; }
		}
		/// <summary>電話番号1</summary>
		public string ShippingTel1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1] = value; }
		}
		/// <summary>配送伝票番号</summary>
		public string ShippingCheckNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO] = value; }
		}
		/// <summary>送り主氏名</summary>
		public string SenderName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME] = value; }
		}
		/// <summary>送り主氏名かな</summary>
		public string SenderNameKana
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA] = value; }
		}
		/// <summary>送り主住所国ISOコード</summary>
		public string SenderCountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE] = value; }
		}
		/// <summary>送り主住所国名</summary>
		public string SenderCountryName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_NAME] = value; }
		}
		/// <summary>送り主郵便番号</summary>
		public string SenderZip
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ZIP]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ZIP] = value; }
		}
		/// <summary>送り主住所1</summary>
		public string SenderAddr1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1] = value; }
		}
		/// <summary>送り主住所2</summary>
		public string SenderAddr2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2] = value; }
		}
		/// <summary>送り主住所3</summary>
		public string SenderAddr3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3] = value; }
		}
		/// <summary>送り主住所4</summary>
		public string SenderAddr4
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4] = value; }
		}
		/// <summary>送り主住所5</summary>
		public string SenderAddr5
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5] = value; }
		}
		/// <summary>送り主企業名</summary>
		public string SenderCompanyName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME] = value; }
		}
		/// <summary>送り主部署名</summary>
		public string SenderCompanyPostName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME] = value; }
		}
		/// <summary>送り主電話番号1</summary>
		public string SenderTel1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_TEL1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_TEL1] = value; }
		}
		/// <summary>のし種類</summary>
		public string WrappingPaperType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE] = value; }
		}
		/// <summary>のし差出人</summary>
		public string WrappingPaperName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME] = value; }
		}
		/// <summary>包装種類</summary>
		public string WrappingBagType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE] = value; }
		}
		/// <summary>画面操作時の商品通番</summary>
		public string ItemIndex
		{
			get { return (string)this.DataSource[Constants.HASH_KEY_ORDER_ITEM_OPERATING_INDEX]; }
			set { this.DataSource[Constants.HASH_KEY_ORDER_ITEM_OPERATING_INDEX] = value; }
		}
		/// <summary>割引後金額</summary>
		public decimal DiscountedPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERITEM_DISCOUNTED_PRICE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_DISCOUNTED_PRICE] = value; }
		}
		/// <summary>商品連携IDリスト</summary>
		public string[] CooperationIds
		{
			get
			{
				var cooperationIds = new []
				{
					this.CooperationId1,
					this.CooperationId2,
					this.CooperationId3,
					this.CooperationId4,
					this.CooperationId5,
					this.CooperationId6,
					this.CooperationId7,
					this.CooperationId8,
					this.CooperationId9,
					this.CooperationId10
				};
				return cooperationIds;
			}
			set
			{
				this.CooperationId1 = value[0];
				this.CooperationId2 = value[1];
				this.CooperationId3 = value[2];
				this.CooperationId4 = value[3];
				this.CooperationId5 = value[4];
				this.CooperationId6 = value[5];
				this.CooperationId7 = value[6];
				this.CooperationId8 = value[7];
				this.CooperationId9 = value[8];
				this.CooperationId10 = value[9];
			}
		}
		/// <summary>付帯情報の価格</summary>
		public decimal OptionPrice
		{
			get
			{
				var result = ProductOptionSettingHelper.ExtractOptionPriceFromProductOptionTexts(this.ProductOptionValue);
				return result;
			}
		}
		/// <summary>明細金額（小計）</summary>
		public decimal ItemPrice
		{
			get
			{
				var productPrice = this.ProductPrice;
				if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED)
				{
					productPrice += this.OptionPrice;
				}
				return PriceCalculator.GetItemPrice(productPrice, this.ItemQuantity);
			}
		}
		/// <summary>明細金額（小計）がマイナスか</summary>
		public bool IsItemPriceMinus => this.ItemPrice < 0;
		/// <summary>税込商品小計</summary>
		public decimal ItemPriceIncludedTax
		{
			get
			{
				var productPrice = this.IsDutyFree ? this.ProductPrice : this.ProductPricePretax;
				if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED)
				{
					productPrice += this.IsDutyFree ? this.OptionPrice : this.OptionPricePretax;
				}
				return PriceCalculator.GetItemPrice(productPrice, this.ItemQuantity);
			}
		}
		/// <summary>バリエーションID</summary>
		public string VariationId
		{
			get { return this.ProductId + this.VId; }
		}
		/// <summary>非課税フラグ</summary>
		public bool IsDutyFree { get { return (TaxCalculationUtility.CheckShippingPlace(this.ShippingCountryIsoCode, this.ShippingAddr5) == false); } }
		/// <summary>配送先は日本住所か</summary>
		public bool IsShippingAddJp
		{
			get { return GlobalAddressUtil.IsCountryJp(this.ShippingCountryIsoCode); }
		}
		/// <summary>付帯情報の税額</summary>
		public decimal OptionPriceTax
		{
			get
			{
				return TaxCalculationUtility.GetTaxPrice(this.OptionPrice, this.ProductTaxRate, Constants.TAX_EXCLUDED_FRACTION_ROUNDING);
			}
		}
		/// <summary>付帯情報価格の税込販売価格</summary>
		public decimal OptionPricePretax
		{
			get
			{
				return TaxCalculationUtility.GetPriceTaxIncluded(
					this.OptionPrice,
					this.OptionPriceTax);
			}
		}
		/// <summary>付帯情報価格込み商品価格</summary>
		public decimal OptionIncludedProductPrice
		{
			get
			{
				return this.ProductPrice + this.OptionPrice;
			}
		}
		/// <summary>頒布会コースID</summary>
		public string SubscriptionBoxCourseId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID] = value; }
		}
		/// <summary>頒布会定額価格</summary>
		public decimal? SubscriptionBoxFixedAmount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT];
			}
			set { this.DataSource[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT] = value; }
		}
		/// <summary>頒布会商品か</summary>
		public bool IsSubscriptionBox => string.IsNullOrEmpty(this.SubscriptionBoxCourseId) == false;
		/// <summary>頒布会定額コース商品か</summary>
		public bool IsSubscriptionBoxFixedAmount => this.SubscriptionBoxFixedAmount.HasValue;
		/// <summary>返品対象か</summary>
		public bool IsReturnTarget => this.ReturnStatus == Constants.FLG_ORDER_RETURN_STATUS_RETURN_TARGET;
		#endregion
	}
}
