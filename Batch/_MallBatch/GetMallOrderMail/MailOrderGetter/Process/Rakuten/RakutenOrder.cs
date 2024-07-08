/*
=========================================================================================================
  Module      : 楽天注文情報：注文クラス (RakutenOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Sql;
using w2.Domain.Order;
using w2.App.Common.Option;
using w2.App.Common.Mall.RakutenApi;

namespace w2.Commerce.MallBatch.MailOrderGetter.Process.Rakuten
{
	///**************************************************************************************
	/// <summary>
	/// 楽天注文情報：注文クラス
	/// </summary>
	///**************************************************************************************
	public class RakutenOrder : OrderModel
	{
		/// <summary>日本国内の決済通貨コード</summary>
		private const string CURRENCY_CODE_JPY = "JPY";

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="rakutenOrder">楽天注文情報</param>
		/// <param name="rakutenOrderItems">楽天注文商品リスト</param>
		public RakutenOrder(string shopId, RakutenApiOrder rakutenOrder, List<RakutenOrderItem> rakutenOrderItems)
		{
			this.OrderId = rakutenOrder.OrderNumber;
			this.ShopId = shopId;
			this.OrderKbn = GetOrderKbn(rakutenOrder.CarrierCode);
			this.OrderPaymentKbn = GetOrderPaymentKbn(rakutenOrder.SettlementModel.SettlementMethod);
			this.OrderDate = rakutenOrder.OrderDatetime;
			this.OrderItemCount = rakutenOrderItems.Count;
			this.OrderProductCount = rakutenOrderItems.Sum(item => item.ItemQuantity);
			this.OrderPriceSubtotal = rakutenOrderItems.Sum(item => item.ItemPrice);
			this.OrderPriceSubtotalTax = rakutenOrderItems.Sum(item => item.ItemPriceTax);
			this.OrderPriceShipping = ChangeDefaultPriceToZero(rakutenOrder.PostagePrice);
			this.OrderPriceExchange = ChangeDefaultPriceToZero(rakutenOrder.DeliveryPrice)
				+ ChangeDefaultPriceToZero(rakutenOrder.PaymentCharge)
				+ ChangeDefaultPriceToZero(rakutenOrder.AdditionalFeeOccurAmountToUser);
			this.OrderPriceTotal = ChangeDefaultPriceToZero(rakutenOrder.RequestPrice);
			this.OrderPriceRegulation = GetOrderPriceRegulation();
			this.CardInstruments = GetCardInstallment(rakutenOrder.SettlementModel.CardInstallmentDesc);
			this.Memo = CreateMemo(rakutenOrder);
			this.RelationMemo = CreateRelationMemo(rakutenOrder, rakutenOrderItems);
			this.RegulationMemo = CreateRegulationMemo(rakutenOrder);
			this.ShippingTaxRate = Constants.CONST_SHIPPING_TAXRATE;
			this.PaymentTaxRate = Constants.CONST_PAYMENT_TAXRATE;
			this.OrderPriceByTaxRates = CreatePriceByTaxRate(rakutenOrderItems);
			this.OrderPriceTax = this.OrderPriceByTaxRates.Sum(priceByTaxRate => priceByTaxRate.TaxPriceByRate);
			this.SettlementCurrency = CURRENCY_CODE_JPY; // Global未対応のためJPY固定
		}

		/// <summary>
		/// コンストラクター（注文メモ更新用）
		/// </summary>
		/// <param name="rakutenOrder">楽天注文情報</param>
		public RakutenOrder(RakutenApiOrder rakutenOrder)
		{
			this.OrderId = rakutenOrder.OrderNumber;
			this.Memo = CreateMemo(rakutenOrder);
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 注文区分取得
		/// </summary>
		/// <param name="carrierCode">利用端末</param>
		/// <returns>注文区分</returns>
		private string GetOrderKbn(int carrierCode)
		{
			switch ((Constants.RakutenApiCarrierCode)carrierCode)
			{
				case Constants.RakutenApiCarrierCode.Pc:
				case Constants.RakutenApiCarrierCode.TabletAndroid:
				case Constants.RakutenApiCarrierCode.TabletIpad:
				case Constants.RakutenApiCarrierCode.TabletOther:
				case Constants.RakutenApiCarrierCode.Other:
					return Constants.FLG_ORDER_ORDER_KBN_PC;

				case Constants.RakutenApiCarrierCode.SmartphoneAndroid:
				case Constants.RakutenApiCarrierCode.SmartphoneIphone:
				case Constants.RakutenApiCarrierCode.SmartphoneOther:
					return Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE;

				case Constants.RakutenApiCarrierCode.MobileDocomo:
				case Constants.RakutenApiCarrierCode.MobileKddi:
				case Constants.RakutenApiCarrierCode.MobileSoftbank:
				case Constants.RakutenApiCarrierCode.MobileWillcom:
					return Constants.FLG_ORDER_ORDER_KBN_MOBILE;

				default:
					return Constants.FLG_ORDER_ORDER_KBN_PC;
			}
		}

		/// <summary>
		/// 決済種別取得
		/// </summary>
		/// <param name="settlementMethod">支払方法名</param>
		/// <returns>決済種別</returns>
		private string GetOrderPaymentKbn(string settlementMethod)
		{
			Hashtable htPaymentKbn = new Hashtable();
			htPaymentKbn[Constants.ORDER_PAYMENT_KBN_NAME_CREDIT] = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT;
			htPaymentKbn[Constants.ORDER_PAYMENT_KBN_NAME_CASH_ON_DELIVERY] = Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT;
			htPaymentKbn["楽天バンク決済"] = Constants.FLG_PAYMENT_PAYMENT_ID_BANK_PRE;
			htPaymentKbn[Constants.ORDER_PAYMENT_KBN_NAME_BANK_TRANSFER_CASH_BEFORE_DELIVERY] = Constants.FLG_PAYMENT_PAYMENT_ID_BANK_PRE;
			htPaymentKbn[Constants.ORDER_PAYMENT_KBN_NAME_BANK_TRANSFER_DEFERRED_PAYMENT] = Constants.FLG_PAYMENT_PAYMENT_ID_BANK_DEF;
			htPaymentKbn[Constants.ORDER_PAYMENT_KBN_NAME_CONVENIENCE_STORE_CASH_BEFORE_DELIVERY] = Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE;
			htPaymentKbn[Constants.ORDER_PAYMENT_KBN_NAME_CONVENIENCE_STORE_DEFERRED_PAYMENT] = Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF;
			htPaymentKbn[Constants.ORDER_PAYMENT_KBN_NAME_POST_TRANSFER_CASH_BEFORE_DELIVERY] = Constants.FLG_PAYMENT_PAYMENT_ID_POST_PRE;
			htPaymentKbn[Constants.ORDER_PAYMENT_KBN_NAME_POST_TRANSFER_DEFERRED_PAYMENT] = Constants.FLG_PAYMENT_PAYMENT_ID_POST_DEF;
			htPaymentKbn["まとめてau支払い"] = Constants.FLG_PAYMENT_PAYMENT_ID_AUMATOMETE_ORG;
			htPaymentKbn[Constants.ORDER_PAYMENT_KBN_NAME_NON_PAYMENT] = Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT;
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("Payment", "GetPaymentMallList"))
			{
				Hashtable htPayment = new Hashtable();
				htPayment.Add(Constants.FIELD_PAYMENT_SHOP_ID, this.ShopId);

				DataView dvPayment = statement.SelectSingleStatement(accessor, htPayment);
				foreach (DataRowView drv in dvPayment)
				{
					htPaymentKbn[(string)drv[Constants.FIELD_PAYMENT_PAYMENT_NAME]] =
						(string)drv[Constants.FIELD_PAYMENT_PAYMENT_ID];
				}
			}

			return (string)htPaymentKbn[settlementMethod];
		}

		/// <summary>
		/// 調整金額取得
		/// </summary>
		/// <returns>調整金額</returns>
		private decimal GetOrderPriceRegulation()
		{
			// 調整金額 = (小計 + 配送料金 + 決済手数料 - 総合計) * -1
			var orderPriceRegulation =
				(TaxCalculationUtility.GetPriceTaxIncluded(this.OrderPriceSubtotal,this.OrderPriceSubtotalTax)
					+ this.OrderPriceShipping
					+ this.OrderPriceExchange
					- this.OrderPriceTotal) * -1;

			return orderPriceRegulation;
		}

		/// <summary>
		/// 決済カード支払回数文言取得
		/// </summary>
		/// <param name="cardInstallmentDesc">クレジットカード支払い回数</param>
		/// <returns>決済カード支払回数文言</returns>
		private string GetCardInstallment(string cardInstallmentDesc)
		{
			if (string.IsNullOrEmpty(cardInstallmentDesc)) return "";

			switch (cardInstallmentDesc)
			{
				case Constants.RAKUTEN_API_CARD_INSTALLMENT_DESC_3:
					return "3回払い";

				case Constants.RAKUTEN_API_CARD_INSTALLMENT_DESC_5:
					return "5回払い";

				case Constants.RAKUTEN_API_CARD_INSTALLMENT_DESC_6:
					return "6回払い";

				case Constants.RAKUTEN_API_CARD_INSTALLMENT_DESC_10:
					return "10回払い";

				case Constants.RAKUTEN_API_CARD_INSTALLMENT_DESC_12:
					return "12回払い";

				case Constants.RAKUTEN_API_CARD_INSTALLMENT_DESC_15:
					return "15回払い";

				case Constants.RAKUTEN_API_CARD_INSTALLMENT_DESC_18:
					return "18回払い";

				case Constants.RAKUTEN_API_CARD_INSTALLMENT_DESC_20:
					return "20回払い";

				case Constants.RAKUTEN_API_CARD_INSTALLMENT_DESC_24:
					return "24回払い";

				default:
					return "";
			}
		}

		/// <summary>
		/// 注文メモ作成
		/// </summary>
		/// <param name="rakutenOrder">楽天注文情報</param>
		/// <returns>注文メモ</returns>
		private string CreateMemo(RakutenApiOrder rakutenOrder)
		{
			var memo = new StringBuilder();
			memo.Append("[配送方法] ").Append(rakutenOrder.DeliveryModel.DeliveryName).Append("\r\n");
			memo.Append("[備考] ").Append(rakutenOrder.Remarks);

			return memo.ToString();
		}

		/// <summary>
		/// 外部連携メモ作成
		/// </summary>
		/// <param name="rakutenOrder">楽天注文情報</param>
		/// <returns>外部連携メモ</returns>
		private string CreateRelationMemo(RakutenApiOrder rakutenOrder, List<RakutenOrderItem> rakutenOrderItems)
		{
			var relationMemo = new StringBuilder();
			// 予約購入
			if (rakutenOrder.OrderType.ToString() == "6")
			{
				relationMemo.Append("[ご利用サービス] ").Append("予約購入").Append("\r\n");
				relationMemo.Append("[申込番号] ").Append(rakutenOrder.ReserveNumber).Append("\r\n");
				relationMemo.Append("[申込日時] ").Append(rakutenOrder.OrderDatetimeString).Append("\r\n");
			}

			// ポイント利用
			if (rakutenOrder.PointModel.UsedPoint <= 0)
			{
				relationMemo.Append("[ポイント利用有無] なし").Append("\r\n");
			}
			else
			{
				relationMemo.Append("[ポイント利用有無] あり").Append("\r\n");
				relationMemo.Append("[ポイント] ").Append(rakutenOrder.PointModel.UsedPoint * -1).Append("円")
					.Append("\r\n");
			}

			// クーポン利用
			if (rakutenOrder.CouponAllTotalPrice <= 0)
			{
				relationMemo.Append("[クーポン利用有無] なし");
			}
			else
			{
				relationMemo.Append("[クーポン利用有無] あり").Append("\r\n");
				relationMemo.Append("[クーポン] ").Append(rakutenOrder.CouponAllTotalPrice * -1).Append("円");
			}

			// ラッピング1利用
			if (rakutenOrder.WrappingModel1 != null)
			{
				relationMemo.Append("\r\n").Append("["+ rakutenOrder.WrappingModel1.Name +"] ").Append(rakutenOrder.WrappingModel1.Price).Append("円");
			}

			// ラッピング2利用
			if (rakutenOrder.WrappingModel2 != null)
			{
				relationMemo.Append("\r\n").Append("[" + rakutenOrder.WrappingModel2.Name + "] ").Append(rakutenOrder.WrappingModel2.Price).Append("円");
			}

			// のし
			if (string.IsNullOrEmpty(rakutenOrder.PackageModelList[0].Noshi) == false)
			{
				relationMemo.Append("\r\n").Append("[のし] ").Append(rakutenOrder.PackageModelList[0].Noshi);
			}

			if (Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION)
			{
				relationMemo.Append("\r\n注文詳細:\r\n");
				var index = 0;
				foreach (var rakutenApiItem in rakutenOrder.PackageModelList[0].ItemModelList)
				{
					relationMemo
						.Append("商品ID:")
						.Append(rakutenOrderItems[index].ProductId)
						.Append("\r\n")
						.Append("バリエーションID:");

					var variationId = rakutenOrderItems[index].VariationId.StartsWith(rakutenOrderItems[index].ProductId)
						? rakutenOrderItems[index].VariationId.Substring(rakutenOrderItems[index].ProductId.Length)
						: rakutenOrderItems[index].VariationId;

					if (string.IsNullOrEmpty(variationId))
					{
						relationMemo
							.Append("-")
							.Append("\r\n")
							.Append("SKU管理番号:")
							.Append(rakutenApiItem.SkuModelList[0].VariantId);
					}
					else
					{
						relationMemo
							.Append(variationId)
							.Append("\r\n")
							.Append("SKU管理番号:")
							.Append(rakutenApiItem.SkuModelList[0].VariantId)
							.Append("\r\n")
							.Append("\r\n")
							.Append(rakutenApiItem.SkuModelList[0].SkuInfo);
					}
					relationMemo
						.Append("\r\n")
						.Append("------------------------------")
						.Append("\r\n");
					index++;
				}
			}

			return relationMemo.ToString();
		}

		/// <summary>
		/// 調整金額メモ作成
		/// </summary>
		/// <param name="rakutenOrder">楽天注文情報</param>
		/// <returns>調整金額メモ</returns>
		private string CreateRegulationMemo(RakutenApiOrder rakutenOrder)
		{
			var regulationMemo = new StringBuilder();
			// ポイント利用
			if (rakutenOrder.PointModel.UsedPoint > 0)
			{
				regulationMemo.Append("[ポイント利用] ").Append(rakutenOrder.PointModel.UsedPoint * -1).Append("円");
			}

			// クーポン利用
			if (rakutenOrder.CouponAllTotalPrice > 0)
			{
				regulationMemo.Append("\r\n").Append("[クーポン利用] ").Append(rakutenOrder.CouponAllTotalPrice * -1).Append("円");
			}

			// ラッピング1利用
			if (rakutenOrder.WrappingModel1 != null)
			{
				regulationMemo.Append("\r\n").Append("[" + rakutenOrder.WrappingModel1.Name + "] ").Append(rakutenOrder.WrappingModel1.Price).Append("円");
			}

			// ラッピング2利用
			if (rakutenOrder.WrappingModel2 != null)
			{
				regulationMemo.Append("\r\n").Append("[" + rakutenOrder.WrappingModel2.Name + "] ").Append(rakutenOrder.WrappingModel2.Price).Append("円");
			}

			return regulationMemo.ToString();
		}

		/// <summary>
		/// 税率毎価格情報作成
		/// </summary>
		/// <param name="rakutenOrderItems">楽天注文商品リスト</param>
		/// <returns>税率毎価格情報</returns>
		private OrderPriceByTaxRateModel[] CreatePriceByTaxRate(List<RakutenOrderItem> rakutenOrderItems)
		{
			var stackedDiscountAmount = 0m;
			var priceTotal = rakutenOrderItems.Sum(item => item.ProductPricePretax * item.ItemQuantity);
			// 調整金額適用対象金額取得
			var shippingPrice = this.OrderPriceShipping;
			var paymentPrice = this.OrderPriceExchange;
			priceTotal += paymentPrice;
			priceTotal += shippingPrice;
			if (priceTotal != 0)
			{
				rakutenOrderItems.ForEach(
					item =>
					{
						item.ItemPriceRegulation = (item.ProductPricePretax * item.ItemQuantity) / priceTotal * this.OrderPriceRegulation;
						stackedDiscountAmount += item.ItemPriceRegulation;
					});
			}
			var shippingRegulationPrice = Math.Floor(shippingPrice / priceTotal * this.OrderPriceRegulation);
			stackedDiscountAmount += shippingRegulationPrice;

			var paymentRegulationPrice = Math.Floor(paymentPrice / priceTotal * this.OrderPriceRegulation);
			stackedDiscountAmount += paymentRegulationPrice;

			var fractionAmount = this.OrderPriceRegulation - stackedDiscountAmount;
			if (fractionAmount != 0)
			{
				var weightItem =
					rakutenOrderItems.FirstOrDefault(item => (item.ProductPricePretax * item.ItemQuantity) > 0);
				if (weightItem != null)
				{
					weightItem.ItemPriceRegulation += fractionAmount;
				}
				else if (shippingPrice != 0)
				{
					shippingRegulationPrice += fractionAmount;
				}
				else
				{
					paymentRegulationPrice += fractionAmount;
				}
			}

			var priceInfo = new List<Hashtable>();
			// 税率毎の購入金額を算出する
			priceInfo.AddRange(rakutenOrderItems
				.Select(item => new Hashtable
				{
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , item.ProductTaxRate },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE , (item.ProductPricePretax * item.ItemQuantity) + item.ItemPriceRegulation },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE , 0m },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , 0m },
				}).ToList());

			priceInfo.Add(new Hashtable
			{
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , this.ShippingTaxRate },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE , 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE , shippingPrice + shippingRegulationPrice },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , 0m },
			});
			priceInfo.Add(new Hashtable
			{ 
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , this.PaymentTaxRate },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE , 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE , 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , paymentPrice + paymentRegulationPrice },
			});

			var groupedItem = priceInfo.GroupBy(item => new
			{
				taxRate = (decimal)item[Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE]
			});
			var priceByTaxRate = groupedItem.Select(
				item => new OrderPriceByTaxRateModel
				{
					KeyTaxRate = item.Key.taxRate,
					PriceSubtotalByRate = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]),
					PriceShippingByRate = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]),
					PricePaymentByRate = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE]),
					PriceTotalByRate = item.Sum(itemKey =>
						((decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE])),
					TaxPriceByRate = TaxCalculationUtility.GetTaxPrice(item.Sum(itemKey =>
							((decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]
								+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]
								+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE])),
						(decimal)item.Key.taxRate,
						Constants.TAX_ROUNDTYPE,
						true)
				}).ToArray();
			foreach (var orderPriceByTaxRateModel in priceByTaxRate)
			{
				orderPriceByTaxRateModel.OrderId = this.OrderId;
			}
			return priceByTaxRate;
		}

		/// <summary>
		/// 楽天の価格未確定時のデフォルト値を0に置き換える
		/// </summary>
		/// <param name="price">価格</param>
		/// <returns>置換後の価格</returns>
		private static decimal ChangeDefaultPriceToZero(decimal price)
		{
			var result = (price != Constants.RAKUTEN_API_DEFAULT_PRICE) ? price : 0m;
			return result;
		}
		#endregion
	}
}
