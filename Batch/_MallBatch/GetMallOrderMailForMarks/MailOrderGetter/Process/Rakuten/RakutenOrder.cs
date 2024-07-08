/*
=========================================================================================================
  Module      : 楽天注文情報：注文クラス (RakutenOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.App.Common.Mall.Rakuten;
using w2.Common.Sql;
using w2.Domain.Order;
using w2.App.Common.Option;

namespace w2.Commerce.MallBatch.MailOrderGetter.Process.Rakuten
{
	///**************************************************************************************
	/// <summary>
	/// 楽天注文情報：注文クラス
	/// </summary>
	///**************************************************************************************
	public class RakutenOrder : OrderModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="rakutenOrder">楽天注文情報</param>
		/// <param name="rakutenOrderItems">楽天注文商品リスト</param>
		public RakutenOrder(string shopId, orderModel rakutenOrder, List<RakutenOrderItem> rakutenOrderItems)
		{
			var postagePrice = (rakutenOrder.postagePrice == -9999) ? 0 : rakutenOrder.postagePrice;
			var paymentCharge = ((rakutenOrder.RBankModel != null) && (rakutenOrder.RBankModel.rbCommissionPayer == 0))
				? rakutenOrder.RBankModel.transferCommission
				: 0;
			this.OrderId = rakutenOrder.orderNumber;
			this.ShopId = shopId;
			this.OrderKbn = GetOrderKbn(rakutenOrder.carrierCode);
			this.OrderPaymentKbn = GetOrderPaymentKbn(rakutenOrder.settlementModel.settlementName);
			this.OrderDate = rakutenOrder.orderDate;
			this.OrderItemCount = rakutenOrderItems.Count;
			this.OrderProductCount = rakutenOrderItems.Sum(item => item.ItemQuantity);
			this.OrderPriceSubtotal = rakutenOrderItems.Sum(item => item.ItemPrice);
			this.OrderPriceSubtotalTax = rakutenOrderItems.Sum(item => item.ItemPriceTax);
			this.OrderPriceShipping = postagePrice;
			this.OrderPriceExchange = rakutenOrder.deliveryPrice + paymentCharge;
			this.OrderPriceTotal = rakutenOrder.firstAmount;
			this.OrderPriceRegulation = GetOrderPriceRegulation();
			this.CardInstruments = (rakutenOrder.settlementModel.cardModel != null)
				? GetCardInstallment(rakutenOrder.settlementModel.cardModel.installmentDesc)
				: "";
			this.Memo = CreateMemo(rakutenOrder);
			this.RelationMemo = CreateRelationMemo(rakutenOrder);
			this.RegulationMemo = CreateRegulationMemo(rakutenOrder);
			this.ShippingTaxRate = Constants.CONST_SHIPPING_TAXRATE;
			this.PaymentTaxRate = Constants.CONST_PAYMENT_TAXRATE;
			this.OrderPriceByTaxRates = CreatePriceByTaxRate(rakutenOrderItems);
			this.OrderPriceTax = this.OrderPriceByTaxRates.Sum(priceByTaxRate => priceByTaxRate.TaxPriceByRate);
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
				(TaxCalculationUtility.GetPriceTaxIncluded(this.OrderPriceSubtotal, this.OrderPriceSubtotalTax)
					+ this.OrderPriceShipping + this.OrderPriceExchange - this.OrderPriceTotal) * -1;
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
		private string CreateMemo(orderModel rakutenOrder)
		{
			// 備考欄の内容取得
			var notes = ToArrayLineBreak(rakutenOrder.option).ToList();

			// あす楽の注文？
			var isAsuraku = (rakutenOrder.asurakuFlg == Constants.RAKUTEN_API_ASURAKU_FLG_ON);
			// 予約購入？
			var isReserveOrder = (rakutenOrder.orderType == Constants.RAKUTEN_API_RESERVE_ORDER_TYPE);

			// 配送日時指定を取得
			var shippingDateTime = notes.GetRange(0, (isAsuraku || isReserveOrder) ? 2 : 3)
				.Where(x => (string.IsNullOrEmpty(x) == false))
				.ToList();

			// 備考
			notes.RemoveRange(0, (isAsuraku || isReserveOrder) ? 2 : 3);

			// 配送日時指定があれば、備考に追加
			if (shippingDateTime.Count > 1) notes.InsertRange(0, shippingDateTime);

			// 配送方法、備考欄の内容
			var memo = string.Format(
				"[配送方法] {0}\r\n{1}",
				rakutenOrder.deliveryModel.deliveryName,
				string.Join("\r\n", notes.Where(x => string.IsNullOrEmpty(x) == false)));
			return memo;
		}

		/// <summary>
		/// 外部連携メモ作成
		/// </summary>
		/// <param name="rakutenOrder">楽天注文情報</param>
		/// <returns>外部連携メモ</returns>
		private string CreateRelationMemo(orderModel rakutenOrder)
		{
			var relationMemo = new StringBuilder();
			// 予約購入
			if (rakutenOrder.orderType == Constants.RAKUTEN_API_RESERVE_ORDER_TYPE)
			{
				relationMemo.Append("[ご利用サービス] 予約購入\r\n");
				relationMemo.AppendFormat("[申込番号] {0}\r\n", rakutenOrder.normalOrderModel.reserveNumber);
				relationMemo.AppendFormat("[申込日時] {0:yyyy/MM/dd}\r\n", rakutenOrder.normalOrderModel.reserveDatetime);
			}

			// ポイント利用
			if ((rakutenOrder.pointModel != null) && (rakutenOrder.pointModel.usedPoint > 0))
			{
				relationMemo.AppendFormat(
					"[ポイント利用有無] あり\r\n[ポイント] {0}円\r\n",
					rakutenOrder.pointModel.usedPoint * -1);
			}
			else
			{
				relationMemo.Append("[ポイント利用有無] なし\r\n");
			}

			// クーポン利用
			if (rakutenOrder.couponAllTotalPrice <= 0)
			{
				relationMemo.Append("[クーポン利用有無] なし\r\n");
			}
			else
			{
				relationMemo.AppendFormat(
					"[クーポン利用有無] あり\r\n[クーポン] {0}円\r\n",
					rakutenOrder.couponAllTotalPrice * -1);
			}

			// ラッピング1利用
			if (rakutenOrder.wrappingModel1 != null)
			{
				relationMemo.AppendFormat(
					"[{0}] {1}円\r\n",
					rakutenOrder.wrappingModel1.name,
					rakutenOrder.wrappingModel1.price);
			}

			// ラッピング2利用
			if (rakutenOrder.wrappingModel2 != null)
			{
				relationMemo.AppendFormat(
					"[{0}] {1}円\r\n",
					rakutenOrder.wrappingModel2.name,
					rakutenOrder.wrappingModel2.price);
			}

			// のし
			if (string.IsNullOrEmpty(rakutenOrder.packageModel[0].noshi) == false)
			{
				relationMemo.AppendFormat("[のし] {0}\r\n", rakutenOrder.packageModel[0].noshi);
			}

			return relationMemo.ToString();
		}

		/// <summary>
		/// 調整金額メモ作成
		/// </summary>
		/// <param name="rakutenOrder">楽天注文情報</param>
		/// <returns>調整金額メモ</returns>
		private string CreateRegulationMemo(orderModel rakutenOrder)
		{
			var regulationMemo = new StringBuilder();
			// ポイント利用
			if ((rakutenOrder.pointModel != null) && (rakutenOrder.pointModel.usedPoint > 0))
			{
				regulationMemo.AppendFormat("[ポイント利用] {0}円", rakutenOrder.pointModel.usedPoint * -1);
			}

			// クーポン利用
			var usedCoupon = rakutenOrder.couponAllTotalPrice;
			if (usedCoupon > 0)
			{
				regulationMemo.AppendFormat("{0}[クーポン利用] {1}円",
					(regulationMemo.Length > 0) ? "\r\n" : "",
					usedCoupon * -1);
			}

			// ラッピング1利用
			if (rakutenOrder.wrappingModel1 != null)
			{
				regulationMemo.AppendFormat(
					"{0}[{1}] {2}円",
					(regulationMemo.Length > 0) ? "\r\n" : "",
					rakutenOrder.wrappingModel1.name,
					rakutenOrder.wrappingModel1.price);
			}

			// ラッピング2利用
			if (rakutenOrder.wrappingModel2 != null)
			{
				regulationMemo.AppendFormat(
					"{0}[{1}] {2}円",
					(regulationMemo.Length > 0) ? "\r\n" : "",
					rakutenOrder.wrappingModel2.name,
					rakutenOrder.wrappingModel2.price);
			}

			return regulationMemo.ToString();
		}

		/// <summary>
		/// 文字列を改行ベースで配列化する
		/// </summary>
		/// <param name="line">文字列（改行あり）</param>
		/// <returns>改行ごとに区切られた文字配列</returns>
		private string[] ToArrayLineBreak(string line)
		{
			return line.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
		}
		#endregion

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
						item.Key.taxRate,
						Constants.TAX_ROUNDTYPE,
						true)
				}).ToArray();
			foreach (var orderPriceByTaxRateModel in priceByTaxRate)
			{
				orderPriceByTaxRateModel.OrderId = this.OrderId;
			}
			return priceByTaxRate;
		}
	}
}