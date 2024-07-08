/*
=========================================================================================================
  Module      : 注文返品交換済商品入力クラス (OrderReturnExchangedItemInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Option;
using w2.App.Common.Util;

namespace w2.App.Common.Input.Order.OrderReturnExchange
{
	/// <summary>
	/// 注文返品交換済商品入力クラス
	/// </summary>
	[Serializable]
	public class OrderReturnExchangedItemInput : InputBaseDto
	{
		#region プロパティ
		/// <summary>返品商品かどうか</summary>
		public bool IsReturnProduct { get; set; }
		/// <summary>返品交換ステータス（更新前）</summary>
		public string ReturnStatusBefore { get; set; }
		/// <summary>商品ID</summary>
		public string ProductId { get; set; }
		/// <summary>商品IDを含まないバリエーションID</summary>
		public string VId { get; set; }
		/// <summary>サプライヤID</summary>
		public string SupplierId { get; set; }
		/// <summary>商品名</summary>
		public string ProductName { get; set; }
		/// <summary>商品名かな</summary>
		public string ProductNameKana { get; set; }
		/// <summary>商品単価</summary>
		public string ProductPrice { get; set; }
		/// <summary>注文数</summary>
		public string ItemQuantity { get; set; }
		/// <summary>商品セールID</summary>
		public string ProductSaleId { get; set; }
		/// <summary>ノベルティID</summary>
		public string NoveltyId { get; set; }
		/// <summary>レコメンドID</summary>
		public string RecommendId { get; set; }
		/// <summary>商品同梱ID</summary>
		public string ProductBundleId { get; set; }
		/// <summary>同梱商品明細表示フラグ</summary>
		public string BundleItemDisplayType { get; set; }
		/// <summary>ブランドID</summary>
		public string BrandId { get; set; }
		/// <summary>ダウンロードURL</summary>
		public string DownloadUrl { get; set; }
		/// <summary>商品連携ID1</summary>
		public string CooperationId1 { get; set; }
		/// <summary>商品連携ID2</summary>
		public string CooperationId2 { get; set; }
		/// <summary>商品連携ID3</summary>
		public string CooperationId3 { get; set; }
		/// <summary>商品連携ID4</summary>
		public string CooperationId4 { get; set; }
		/// <summary>商品連携ID5</summary>
		public string CooperationId5 { get; set; }
		/// <summary>商品連携ID6</summary>
		public string CooperationId6 { get; set; }
		/// <summary>商品連携ID7</summary>
		public string CooperationId7 { get; set; }
		/// <summary>商品連携ID8</summary>
		public string CooperationId8 { get; set; }
		/// <summary>商品連携ID9</summary>
		public string CooperationId9 { get; set; }
		/// <summary>商品連携ID10</summary>
		public string CooperationId10 { get; set; }
		/// <summary>商品付帯情報</summary>
		public string ProductOptionValue { get; set; }
		/// <summary>配送先枝番</summary>
		public string OrderShippingNo { get; set; }
		/// <summary>配送先氏名</summary>
		public string ShippingName { get; set; }
		/// <summary>配送先氏名かな</summary>
		public string ShippingNameKana { get; set; }
		/// <summary>電話番号1</summary>
		public string ShippingTel1 { get; set; }
		/// <summary>送り主住所国ISOコード</summary>
		public string ShippingCountryIsoCode { get; set; }
		/// <summary>送り主住所国名</summary>
		public string ShippingCountryName { get; set; }
		/// <summary>送り主郵便番号</summary>
		public string ShippingZip { get; set; }
		/// <summary>送り主住所1</summary>
		public string ShippingAddr1 { get; set; }
		/// <summary>送り主住所2</summary>
		public string ShippingAddr2 { get; set; }
		/// <summary>送り主住所3</summary>
		public string ShippingAddr3 { get; set; }
		/// <summary>送り主住所4</summary>
		public string ShippingAddr4 { get; set; }
		/// <summary>送り主住所5</summary>
		public string ShippingAddr5 { get; set; }
		/// <summary>配送先企業名</summary>
		public string ShippingCompanyName { get; set; }
		/// <summary>配送先部署名</summary>
		public string ShippingCompanyPostName { get; set; }
		/// <summary>税込販売価格</summary>
		public string ProductPricePretax { get; set; }
		/// <summary>税込フラグ</summary>
		public string ProductTaxIncludedFlg { get; set; }
		/// <summary>税率</summary>
		public string ProductTaxRate { get; set; }
		/// <summary>税計算方法</summary>
		public string ProductTaxRoundType { get; set; }
		/// <summary>セットプロモーション枝番</summary>
		public string OrderSetPromotionNo { get; set; }
		/// <summary>定期商品フラグ</summary>
		public string FixedPurchaseProductFlg { get; set; }
		/// <summary>バリエーションID</summary>
		public string VariationId
		{
			get { return this.ProductId + this.VId; }
		}
		/// <summary>明細金額（税金額）</summary>
		public string ItemPriceTax
		{
			get
			{
				var itemPriceTax = PriceCalculator.GetItemPrice(TaxCalculationUtility.GetTaxPrice(
						decimal.Parse(this.ProductPrice),
						decimal.Parse(this.ProductTaxRate),
						this.ShippingCountryIsoCode,
						this.ShippingAddr5,
						Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
					int.Parse(this.ItemQuantity));
				return itemPriceTax.ToString();
				;
			}
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
		}
		/// <summary>返品交換ステータス</summary>
		public string ReturnStatus
		{
			get
			{
				var status = string.Empty;
				if ((this.ReturnStatusBefore != Constants.FLG_ORDER_RETURN_STATUS_RETURN_COMPLETE)
				    && (this.ReturnStatusBefore != Constants.FLG_ORDER_RETURN_STATUS_RETURN_EXCHANGED))
				{
					status = this.IsReturnProduct
						? Constants.FLG_ORDER_RETURN_STATUS_RETURN_TARGET
						: Constants.FLG_ORDER_RETURN_STATUS_RETURN_UNTARGET;
				}
				else
				{
					status = Constants.FLG_ORDER_RETURN_STATUS_RETURN_COMPLETE;
				}

				return status;
			}
		}
		/// <summary>明細金額(割引後価格)</summary>
		public string DiscountedPrice { get; set; }
		/// <summary>頒布会コースID</summary>
		public string SubscriptionBoxCourseId { get; set; }
		/// <summary>頒布会定額価格</summary>
		public string SubscriptionBoxFixedAmount { get; set; }
		/// <summary>頒布会定額コース商品か</summary>
		public bool IsSubscriptionBoxFixedAmount => string.IsNullOrEmpty(this.SubscriptionBoxFixedAmount) == false;
		#endregion
	}
}
