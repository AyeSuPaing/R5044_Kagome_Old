/*
=========================================================================================================
  Module      : 注文返品交換入力クラス (OrderReturnExchangeInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.DataCacheController;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Util;
using w2.Domain;
using w2.Domain.Order;

namespace w2.App.Common.Input.Order.OrderReturnExchange
{
	/// <summary>
	/// 注文返品交換入力クラス
	/// </summary>
	[Serializable]
	public class OrderReturnExchangeInput : InputBaseDto
	{
		#region メソッド
		/// <summary>
		/// 商品を全返品しているか？
		/// </summary>
		/// <returns>商品を全返品している：true、商品を全返品していない：false</returns>
		public bool IsReturnAllItems()
		{
			// 返品交換区分が「返品」以外の場合はfalse
			if (this.OrderReturnExchangeKbn != Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN) return false;

			// 全返品ではない場合はfalse
			var orderItems = CreateAllReturnOrderItems();
			if (orderItems.Any(x => (x.ReturnStatus == Constants.FLG_ORDER_RETURN_STATUS_RETURN_UNTARGET))) return false;

			return true;
		}

		/// <summary>
		/// 返品商品情報作成
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="items">商品情報</param>
		/// <returns>返品商品情報</returns>
		public ReturnOrderItem[] CreateReturnOrderItems(
			string shopId,
			OrderReturnExchangedItemInput[] items)
		{
			var returnOrderItems = items
				.Select(
					item =>
					{
						var returnOrderItem = new ReturnOrderItem();

						returnOrderItem.ItemReturnExchangeKbn = Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN;
						returnOrderItem.ReturnStatus = item.ReturnStatus;
						returnOrderItem.ShopId = shopId;
						returnOrderItem.ProductId = item.ProductId;
						returnOrderItem.VId = item.VId;
						returnOrderItem.SupplierId = item.SupplierId;
						returnOrderItem.ProductName = item.ProductName;
						returnOrderItem.ProductNameKana = item.ProductNameKana;

						if (item.IsSubscriptionBoxFixedAmount == false)
						{
							returnOrderItem.ProductPrice =
								decimal.TryParse(item.ProductPrice, out var productPrice)
									? productPrice
									: 0m;
							returnOrderItem.ItemPriceTax =
								decimal.TryParse(item.ItemPriceTax, out var itemPriceTax)
									? itemPriceTax
									: 0m;
							returnOrderItem.ProductPricePretax =
								decimal.TryParse(item.ProductPricePretax, out var productPricePretax)
									? productPricePretax
									: 0m;
							returnOrderItem.DiscountedPrice =
								decimal.TryParse(item.DiscountedPrice, out var discountedPrice)
									? discountedPrice
									: 0m;
						}
						else
						{
							returnOrderItem.ProductPrice = 0m;
							returnOrderItem.ItemPriceTax = 0m;
							returnOrderItem.ProductPricePretax = 0m;
							returnOrderItem.DiscountedPrice = 0m;
						}

						int.TryParse(item.ItemQuantity, out var itemQuantity);
						returnOrderItem.ItemQuantity = itemQuantity;

						returnOrderItem.ProductSaleId = item.ProductSaleId;
						returnOrderItem.BrandId = item.BrandId;
						returnOrderItem.DownloadUrl = item.DownloadUrl;
						returnOrderItem.CooperationIds = item.CooperationIds;
						returnOrderItem.ProductOptionValue = item.ProductOptionValue;
						returnOrderItem.OrderShippingNo = item.OrderShippingNo;
						returnOrderItem.ShippingName = item.ShippingName;
						returnOrderItem.ShippingNameKana = item.ShippingNameKana;
						returnOrderItem.ShippingTel1 = item.ShippingTel1;
						returnOrderItem.ShippingCountryIsoCode = item.ShippingCountryIsoCode;
						returnOrderItem.ShippingCountryName = item.ShippingCountryName;
						returnOrderItem.ShippingZip = item.ShippingZip;
						returnOrderItem.ShippingAddr1 = item.ShippingAddr1;
						returnOrderItem.ShippingAddr2 = item.ShippingAddr2;
						returnOrderItem.ShippingAddr3 = item.ShippingAddr3;
						returnOrderItem.ShippingAddr4 = item.ShippingAddr4;
						returnOrderItem.ShippingAddr5 = item.ShippingAddr5;
						returnOrderItem.ShippingCompanyName = item.ShippingCompanyName;
						returnOrderItem.ShippingCompanyPostName = item.ShippingCompanyPostName;
						returnOrderItem.ProductTaxIncludedFlg = item.ProductTaxIncludedFlg;

						decimal.TryParse(item.ProductTaxRate, out var productTaxRate);
						returnOrderItem.ProductTaxRate = productTaxRate;
						returnOrderItem.ProductTaxRoundType = item.ProductTaxRoundType;
						returnOrderItem.OrderSetPromotionNo = item.OrderSetPromotionNo;
						returnOrderItem.NoveltyId = item.NoveltyId;
						returnOrderItem.RecommendId = item.RecommendId;
						returnOrderItem.ProductBundleId = item.ProductBundleId;
						returnOrderItem.BundleItemDisplayType = item.BundleItemDisplayType;
						returnOrderItem.FixedPurchaseProductFlg = item.FixedPurchaseProductFlg;
						returnOrderItem.SubscriptionBoxCourseId = item.SubscriptionBoxCourseId;
						returnOrderItem.SubscriptionBoxFixedAmount =
							decimal.TryParse(item.SubscriptionBoxFixedAmount, out var fixedAmount)
								? fixedAmount
								: (decimal?)null;

						return returnOrderItem;
					})
				.ToArray();

			return returnOrderItems;
		}

		/// <summary>
		/// 交換商品情報作成
		/// </summary>
		/// <param name="returnExchangeShippingNo">返品交換対象配送先ID</param>
		/// <param name="haveOnlyOneFixedAmountCourseItem">1種類の頒布会定額コースの商品のみが含まれるか</param>
		/// <returns>交換注文商品情報</returns>
		public ReturnOrderItem[] CreateExchangeOrderItems(
			string returnExchangeShippingNo,
			bool haveOnlyOneFixedAmountCourseItem = false)
		{
			var orderItemImputs = this.ExchangeItems
				.Select(
					item =>
					{
						var exchangeOrderItem = new ReturnOrderItem();
						exchangeOrderItem.ShopId = this.ShopId;
						exchangeOrderItem.ProductId = item.ProductId;
						exchangeOrderItem.VId = item.VId;
						exchangeOrderItem.SupplierId = item.SupplierId;
						exchangeOrderItem.ProductName = item.ProductName;
						exchangeOrderItem.ProductNameKana = string.Empty;

						if (haveOnlyOneFixedAmountCourseItem == false)
						{
							exchangeOrderItem.ProductPrice =
								decimal.TryParse(item.ProductPrice, out var productPrice)
									? productPrice
									: 0m;
							exchangeOrderItem.ItemPriceTax =
								decimal.TryParse(item.ItemPriceTax, out var itemPriceTax)
									? itemPriceTax
									: 0m;
							exchangeOrderItem.ProductPricePretax =
								decimal.TryParse(item.ProductPricePretax, out var productPricePretax)
									? productPricePretax
									: 0m;
							exchangeOrderItem.ProductTaxRate =
								decimal.TryParse(item.ProductTaxRate, out var productTaxRate)
									? productTaxRate
									: 0m;
							exchangeOrderItem.DiscountedPrice =
								decimal.TryParse(item.DiscountedPrice, out var discountedPrice)
									? discountedPrice
									: 0m;
							exchangeOrderItem.SubscriptionBoxCourseId = string.Empty;
							exchangeOrderItem.SubscriptionBoxFixedAmount = null;
						}
						else
						{
							exchangeOrderItem.ProductPrice = 0m;
							exchangeOrderItem.ItemPriceTax = 0m;
							exchangeOrderItem.ProductPricePretax = 0m;
							exchangeOrderItem.ProductTaxRate = 0m;
							exchangeOrderItem.DiscountedPrice = 0m;
							exchangeOrderItem.SubscriptionBoxCourseId = item.SubscriptionBoxCourseId;
							exchangeOrderItem.SubscriptionBoxFixedAmount =
								decimal.TryParse(item.SubscriptionBoxFixedAmount, out var fixedAmount)
									? fixedAmount
									: (decimal?)null;
						}

						int.TryParse(item.ItemQuantity, out var itemQuantity);
						exchangeOrderItem.ItemQuantity = itemQuantity;
						exchangeOrderItem.ProductSaleId = item.ProductSaleId;

						exchangeOrderItem.ProductOptionValue = item.ProductOptionValue.Trim();
						exchangeOrderItem.ProductTaxIncludedFlg = TaxCalculationUtility.GetPrescribedOrderItemTaxIncludedFlag();

						exchangeOrderItem.ProductTaxRoundType = Constants.TAX_EXCLUDED_FRACTION_ROUNDING;
						exchangeOrderItem.ItemIndex = item.ItemIndex;
						exchangeOrderItem.OrderSetPromotionNo = item.OrderSetPromotionNo;
						exchangeOrderItem.OrderShippingNo = returnExchangeShippingNo ?? string.Empty;
						exchangeOrderItem.NoveltyId = item.NoveltyId;
						var recommendId = Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false)
							? item.RecommendId
							: item.RecommendId2;
						exchangeOrderItem.RecommendId = recommendId;
						exchangeOrderItem.FixedPurchaseProductFlg = item.FixedPurchaseProductFlg;
						var productBundleId = Constants.PRODUCTBUNDLE_OPTION_ENABLED
							? item.ProductBundleId
							: string.Empty;
						exchangeOrderItem.ProductBundleId = productBundleId;

						return exchangeOrderItem;
					})
				.ToArray();

			return orderItemImputs;
		}

		/// <summary>
		/// 返品商品情報作成(元注文・交換済み注文全て)
		/// </summary>
		/// <returns>返品注文商品情報</returns>
		public ReturnOrderItem[] CreateAllReturnOrderItems()
		{
			var items = new List<ReturnOrderItem>();
			items.AddRange(CreateReturnOrderItems(this.ShopId, this.ReturnItems.SelectMany(x => x).ToArray()));
			items.AddRange(CreateReturnOrderItems(this.ShopId, this.ExchangedItems.SelectMany(x => x).ToArray()));
			return items.ToArray();
		}

		/// <summary>
		/// 税率毎返品用金額補正入力値から税率毎価格情報モデルを取得
		/// </summary>
		/// <returns>税率毎返品用金額補正の配列</returns>
		public List<OrderPriceByTaxRateModel> GetPriceInfoFromPriceCorrectionBox()
		{
			var result = this.PriceCorrectionItems.Select(item =>
				{
					var model = new OrderPriceByTaxRateModel
					{
						KeyTaxRate = decimal.Parse(item.ProductTaxRate),
						PriceSubtotalByRate = 0m,
						PriceShippingByRate = 0m,
						PricePaymentByRate = 0m,
						PriceTotalByRate = 0m,
						TaxPriceByRate = 0m,
						ReturnPriceCorrectionByRate = decimal.Parse(item.PriceCorrection),
					};
					return model;
				})
				.ToList();
			return result;
		}

		/// <summary>
		/// 税率毎調整価格入力ボックス設定
		/// </summary>
		/// <param name="orderIdOrg">元注文ID</param>
		/// <param name="orderPriceByTaxRateOrg">元注文価格情報</param>
		/// <param name="exchangeOrderItems">交換対象商品リスト</param>
		public List<OrderPriceByTaxRateModel> SetProductCorrectionPriceBox(
			string orderIdOrg,
			List<OrderPriceByTaxRateModel> orderPriceByTaxRateOrg,
			List<ReturnOrderItem> exchangeOrderItems)
		{
			var taxList = new List<OrderPriceByTaxRateModel>();
			taxList.AddRange(orderPriceByTaxRateOrg);

			// 頒布会定額コース以外
			taxList.AddRange(
				exchangeOrderItems
					.Where(item => item.IsSubscriptionBoxFixedAmount == false)
					.GroupBy(
						item => new
						{
							TaxRate = item.ProductTaxRate,
						})
					.Where(
						itemTax =>
						{
							var isMatchTaxRate = orderPriceByTaxRateOrg.Any(
								priceInfoByTaxRate => priceInfoByTaxRate.KeyTaxRate == itemTax.Key.TaxRate);

							return isMatchTaxRate == false;
						})
					.Select(
						itemTax => new OrderPriceByTaxRateModel
						{
							OrderId = orderIdOrg,
							KeyTaxRate = itemTax.Key.TaxRate,
							PriceSubtotalByRate = 0m,
							PriceShippingByRate = 0m,
							PricePaymentByRate = 0m,
							PriceTotalByRate = 0m,
							TaxPriceByRate = 0m,
							ReturnPriceCorrectionByRate = 0m,
						})
					.ToList());

			// 頒布会定額コース商品のみで計算
			if (exchangeOrderItems.Any(item => item.IsSubscriptionBoxFixedAmount))
			{
				var itemsGroupByFixedAmountCourse = exchangeOrderItems
					.Where(item => item.IsSubscriptionBoxFixedAmount)
					.GroupBy(item => item.SubscriptionBoxCourseId);
				foreach (var courseGroup in itemsGroupByFixedAmountCourse)
				{
					var subscriptionBox = DataCacheControllerFacade.GetSubscriptionBoxCacheController().Get(courseGroup.Key);
					var taxRate = DomainFacade.Instance.ProductTaxCategoryService.Get(subscriptionBox.TaxCategoryId).TaxRate;

					if (taxList.Any(orderPrices => orderPrices.KeyTaxRate == taxRate)) continue;

					taxList.Add(
						new OrderPriceByTaxRateModel
						{
							OrderId = orderIdOrg,
							KeyTaxRate = taxRate,
							PriceSubtotalByRate = 0m,
							PriceShippingByRate = 0m,
							PricePaymentByRate = 0m,
							PriceTotalByRate = 0m,
							TaxPriceByRate = 0m,
							ReturnPriceCorrectionByRate = 0m,
						});
				}
			}

			// 商品価格調整金額の設定
			foreach (var item in this.PriceCorrectionItems)
			{
				var targetPriceInfoByTaxRate = taxList.FirstOrDefault(
					priceInfoByTaxRate => priceInfoByTaxRate.KeyTaxRate == decimal.Parse(item.ProductTaxRate));

				if (targetPriceInfoByTaxRate == null) continue;

				targetPriceInfoByTaxRate.ReturnPriceCorrectionByRate = decimal.Parse(item.PriceCorrection);
			}

			return taxList.OrderBy(priceInfo => priceInfo.KeyTaxRate).ToList();
		}

		/// <summary>
		/// 最終利用ポイント入力チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string CheckLastOrderPointUseInput()
		{
			var input = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_POINT_USE, this.LastOrderPointUse}
			};
			var pointErrorMessage = Validator.Validate("OrderReturnExchangePoint", input);
			if (pointErrorMessage.Length != 0) return pointErrorMessage;

			return string.Empty;
		}

		/// <summary>
		/// 返金金額の入力チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string CheckInputOrderPriceRepayment()
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_PRICE_REPAYMENT, this.OrderPriceRepayment },
			};
			var errorMessage = Validator.Validate("OrderReturnExchangeInput", ht);
			return errorMessage;
		}

		/// <summary>
		/// 税率毎商品価格調整金の入力チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string CheckInputProductCorrectionPrice()
		{
			var errorMessage = new StringBuilder();
			foreach (var item in this.PriceCorrectionItems)
			{
				// 税率毎商品価格調整金
				errorMessage.Append(Validator.Validate("OrderPriceByTaxRateReturnExchangeInput", item.DataSource)
					.Replace("@@ 1 @@", "税率" + item.ProductTaxRate + "%)"));
			}

			return errorMessage.ToString();
		}

		/// <summary>
		/// 返品商品合計を計算します。
		/// </summary>
		/// <param name="returnOrderItems">返品注文商品情報</param>
		/// <param name="allReturnFixedAmountCourseIds">全返品を行う頒布会定額コースID配列</param>
		/// <returns>返品商品合計</returns>
		public decimal CalculateReturnOrderPriceSubTotal(
			ReturnOrderItem[] returnOrderItems,
			string[] allReturnFixedAmountCourseIds)
		{
			// 頒布会定額コース商品を除いて計算
			var excludeFixedAmountItem = returnOrderItems
				.Where(item => item.IsReturnTarget && (item.IsSubscriptionBoxFixedAmount == false))
				.Sum(item => item.ItemPrice);

			// 頒布会定額コース分
			var fixedAmountSubtotal = returnOrderItems
				.Where(
					item => item.IsReturnTarget
						&& allReturnFixedAmountCourseIds.Contains(item.SubscriptionBoxCourseId))
				.GroupBy(item => item.SubscriptionBoxCourseId)
				.Sum(item => item.First().SubscriptionBoxFixedAmount.Value);

			return excludeFixedAmountItem + fixedAmountSubtotal;
		}

		/// <summary>
		/// 返品交換商品合計を計算します。
		/// </summary>
		/// <param name="returnOrderPriceSubTotal">返品商品合計</param>
		/// <param name="exchangeOrderPriceSubTotal">交換商品合計</param>
		/// <returns>返品交換商品合計</returns>
		public decimal CalculateReturnExchangeOrderPriceSubTotal(
			decimal returnOrderPriceSubTotal,
			decimal exchangeOrderPriceSubTotal)
		{
			var total = returnOrderPriceSubTotal + exchangeOrderPriceSubTotal;
			return total;
		}

		/// <summary>
		/// 返品交換商品消費税合計を計算します。
		/// </summary>
		/// <param name="returnOrderPriceTaxSubTotal">返品商品消費税合計</param>
		/// <param name="exchangeOrderPriceTaxSubTotal">交換商品消費税合計</param>
		/// <returns>返品交換商品消費税合計</returns>
		public decimal CalculateReturnExchangeOrderPriceTaxSubTotal(
			decimal returnOrderPriceTaxSubTotal,
			decimal exchangeOrderPriceTaxSubTotal)
		{
			var total = returnOrderPriceTaxSubTotal + exchangeOrderPriceTaxSubTotal;
			return total;
		}

		/// <summary>
		/// 最終合計金額を計算します。
		/// </summary>
		/// <param name="returnExchangeOrderPriceSubTotal">返品交換商品合計</param>
		/// <param name="returnExchangeOrderPriceTaxSubTotal">返品交換商品税額合計</param>
		/// <param name="adjustmentPointPrice">ポイント調整金額</param>
		/// <param name="returnPriceCorrectionTotal">返品用金額補正合計</param>
		/// <returns>最終合計金額</returns>
		public decimal CalculateReturnExchangeOrderPriceTotal(
			decimal returnExchangeOrderPriceSubTotal,
			decimal returnExchangeOrderPriceTaxSubTotal,
			decimal adjustmentPointPrice,
			decimal returnPriceCorrectionTotal)
		{
			var total = TaxCalculationUtility.GetPriceTaxIncluded(returnExchangeOrderPriceSubTotal, returnExchangeOrderPriceTaxSubTotal) 
				- adjustmentPointPrice 
				+ returnPriceCorrectionTotal;
			return total;
		}

		/// <summary>
		/// 最終請求金額を計算します。
		/// </summary>
		/// <param name="returnExchangeOrderLastBilledAmount">前回の最終請求金額</param>
		/// <param name="returnExchangeOrderPriceTotal">最終合計金額</param>
		/// <returns>最終請求金額</returns>
		public decimal CalculateReturnExchangeLastBilledAmount(
			decimal returnExchangeOrderLastBilledAmount,
			decimal returnExchangeOrderPriceTotal)
		{
			var total = returnExchangeOrderLastBilledAmount + returnExchangeOrderPriceTotal;
			return total;
		}

		/// <summary>
		/// 対象の頒布会定額コースに含まれる注文商品個数を取得（返品商品）
		/// </summary>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <returns>対象コースに含まれる注文商品個数</returns>
		public int GetFixedAmountCourseItemQuantityOnReturnItems(string subscriptionBoxCourseId)
		{
			var result = this.ReturnItems
				.SelectMany(item => item)
				.Where(
					item => item.IsReturnProduct
						&& item.IsSubscriptionBoxFixedAmount
						&& (item.SubscriptionBoxCourseId == subscriptionBoxCourseId))
				.Sum(item => int.Parse(item.ItemQuantity));
			return result;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId { get; set; }
		/// <summary>処理区分</summary>
		public string ExecuteType { get; set; }
		/// <summary>返品交換区分</summary>
		public string OrderReturnExchangeKbn { get; set; }
		/// <summary>返品交換受付日</summary>
		public string OrderReturnExchangeReceiptDate { get; set; }
		/// <summary>返品交換理由	</summary>
		public string ReturnExchangeReasonMemo { get; set; }
		/// <summary>返品交換都合区分</summary>
		public string ReturnExchangeReasonKbn { get; set; }
		/// <summary>決済種別</summary>
		public string OrderPaymentKbn { get; set; }
		/// <summary>クレジットトークン</summary>
		public string CreditToken { get; set; }
		/// <summary>カード番号下４桁</summary>
		public string LastFourDigit { get; set; }
		/// <summary>利用クレジットカード</summary>
		public string UserCreditCard { get; set; }
		/// <summary>カード会社</summary>
		public string CreditCardCompany { get; set; }
		/// <summary>永久トークン</summary>
		public string CreditCardNo1 { get; set; }
		/// <summary>クレジットカード有効期限(月)</summary>
		public string CreditExpireMonth { get; set; }
		/// <summary>クレジットカード有効期限(年)</summary>
		public string CreditExpireYear { get; set; }
		/// <summary>カード名義人</summary>
		public string CreditAuthorName { get; set; }
		/// <summary>セキュリティコード</summary>
		public string CreditSecurityCode { get; set; }
		/// <summary>支払回数(コード)</summary>
		public string CreditInstallments { get; set; }
		/// <summary>クレジットカードを登録するか？</summary>
		public bool IsRegisterCreditCard { get; set; }
		/// <summary>登録クレジットカード名</summary>
		public string UserCreditCardName { get; set; }
		/// <summary>全返品するか？</summary>
		public bool IsReturnProductAll { get; set; }
		/// <summary>返品注文商品リスト</summary>
		public OrderReturnExchangedItemInput[][] ReturnItems { get; set; }
		/// <summary>付与ポイント自動計算ありか？</summary>
		public bool IsOrderPointAddReCalculate { get; set; }
		/// <summary>通常 付与ポイント</summary>
		public string OrderBasePointAdd { get; set; }
		/// <summary>通常 付与ポイント（更新前）</summary>
		public string OrderBasePointAddBefore { get; set; }
		/// <summary>期間限定 付与ポイント</summary>
		public string OrderLimitPointAdd { get; set; }
		/// <summary>期間限定 付与ポイント（更新前）</summary>
		public string OrderLimitPointAddBefore { get; set; }
		/// <summary>最終利用ポイント</summary>
		public string LastOrderPointUse { get; set; }
		/// <summary>最終利用ポイント（更新前）</summary>
		public string LastOrderPointUseBefore { get; set; }
		/// <summary>交換済注文商品リスト</summary>
		public OrderReturnExchangedItemInput[][] ExchangedItems { get; set; }
		/// <summary>交換注文商品リスト</summary>
		public OrderReturnExchangeItemInput[] ExchangeItems { get; set; }
		/// <summary>返品金額補正リスト</summary>
		public OrderReturnExchangePriceCorrectionInput[] PriceCorrectionItems { get; set; }
		/// <summary>金額補正の自動計算をするか？</summary>
		public bool IsOrderPriceCorrectionReCalculate { get; set; }
		/// <summary>調整金額メモ</summary>
		public string RegulationMemo { get; set; }
		/// <summary>交換時ポイント発行</summary>
		public string ExchangeAddPoint { get; set; }
		/// <summary>返金金額</summary>
		public string OrderPriceRepayment { get; set; }
		/// <summary>返金メモ</summary>
		public string RepaymentMemo { get; set; }
		/// <summary>注文返品交換仮ポイントリスト</summary>
		public OrderReturnExchangeTempPointInput[] OrderTempPoints { get; set; }
		/// <summary>ポイント調整金額</summary>
		public decimal AdjustmentPointPrice
		{
			get
			{
				decimal lastOrderPointUse;
				decimal.TryParse(this.LastOrderPointUse, out lastOrderPointUse);
				
				decimal lastOrderPointUseBefore;
				decimal.TryParse(this.LastOrderPointUseBefore, out lastOrderPointUseBefore);

				var adjustmentPointPrice = Constants.W2MP_POINT_OPTION_ENABLED
					? lastOrderPointUse - lastOrderPointUseBefore
					: 0m;
				return adjustmentPointPrice;
			}
		}
		/// <summary>合計返品用金額補正</summary>
		public decimal ReturnPriceCorrectionTotal
		{
			get
			{
				var totalPriceCorrection =
					this.PriceCorrectionItems.Sum(x =>
					{
						decimal priceCorrection;
						decimal.TryParse(x.PriceCorrection, out priceCorrection);
						return priceCorrection;
					});
				return totalPriceCorrection;
			}
		}
		/// <summary>返金先銀行コード</summary>
		public string RepaymentBankCode { get; set; }
		/// <summary>返金先銀行名</summary>
		public string RepaymentBankName { get; set; }
		/// <summary>返金先銀行支店名</summary>
		public string RepaymentBankBranch { get; set; }
		/// <summary>返金先銀行口座情報</summary>
		public string RepaymentBankAccountNo { get; set; }
		/// <summary>返金先銀行口座名</summary>
		public string RepaymentBankAccountName { get; set; }
		#endregion
	}
}
