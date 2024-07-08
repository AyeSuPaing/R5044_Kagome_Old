/*
=========================================================================================================
  Module      : 取引登録のリクエスト値(AtobaraicomRegistationApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Option;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Atobaraicom;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.User;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 後払い登録API
	/// </summary>
	public class AtobaraicomRegistationApi : AtobaraicomBaseApi
	{
		/// <summary>
		/// 実行 後払いドットコム決済取消
		/// </summary>
		/// <param name="input">入力</param>
		/// <param name="carts">カート</param>
		/// <param name="orderModel">Order model</param>
		/// <returns>応答データ</returns>
		public AtobaraicomRegistationResponse ExecRegistation(
			Hashtable input,
			CartObject carts,
			OrderModel orderModel = null)
		{
			var requestLog = new StringBuilder();
			var responseLog = new StringBuilder();
			var responseData = new AtobaraicomRegistationResponse();
			this.Messages = AtobaraicomConstants.ATOBARAICOM_API_AUTH_RESULT_STATUS_OK;
			try
			{
				// リクエストパラメータ作成
				var request = CreateParam(input, carts, orderModel);
				var stringData = this.PostHttpRequest(request, Constants.PAYMENT_SETTING_ATOBARAICOM_REGISTATION_URL);
				responseData.HandleMessages(stringData);
				var breadLine = "\r\n";
				var paramString = string.Join(
					",",
					request.Select(p => string.Format("{0}={1}", p[0], p[1], Encoding.UTF8)));
				requestLog.Append(breadLine).Append("Request Post: ").Append(breadLine);
				requestLog.Append("URL: " + Constants.PAYMENT_SETTING_ATOBARAICOM_REGISTATION_URL).Append(breadLine);
				requestLog.Append(paramString).Append(breadLine);
				WriteLog(
					requestLog.ToString(),
					null,
					string.Empty,
					Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
					PaymentFileLogger.PaymentProcessingType.Atobaraicom);

				responseLog.Append(breadLine).Append("Request Response: ").Append(breadLine);
				responseLog.Append(stringData).Append(breadLine);
				WriteLog(
					responseLog.ToString(),
					null,
					string.Empty,
					Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
					PaymentFileLogger.PaymentProcessingType.Atobaraicom);

				if (string.IsNullOrEmpty(stringData)
					|| responseData.IsError)
				{
					return responseData;
				}

				if (responseData.IsAuthorizeNG)
				{
					this.Messages = responseData.OrderStatus;
				}
				else if (responseData.IsAuthorizeHold)
				{
					this.Messages = responseData.OrderStatus;
				}
				else if (responseData.IsSuccess
					&& (responseData.OrderStatusCdCode == AtobaraicomConstants.GET_AUTH_RESULT_AUTH_RESULT_HOLD))
				{
					this.Messages = responseData.OrderStatus;
				}
				else
				{
					responseData.Messages = responseData.Status;
				}
			}
			catch (Exception ex)
			{
				w2.Common.Logger.FileLogger.WriteError("実行 後払い.com 決済に失敗しました。", ex);
				this.Messages = "API Fail " + ex.Message;
				responseData.Messages = Messages;
			}
			return responseData;
		}

		/// <summary>
		/// パラメータを作成
		/// </summary>
		/// <param name="input">入力</param>
		/// <param name="carts">カートオブジェクト</param>
		/// <param name="orderModel">注文モデル</param>
		/// <returns>パラメータを作成</returns>
		private string[][] CreateParam(Hashtable input, CartObject carts, OrderModel orderModel = null)
		{
			var orderFlag = (orderModel != null);
			var shipping = (carts != null) ? carts.GetShipping() : null;
			var lstParam = new List<string[]>()
			{
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_ORDER_DATE, DateTime.Now.ToString("yyyyMMdd") },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_RECEPTIONIST_ID,
					Constants.PAYMENT_SETTING_ATOBARAICOM_ENTERPRISED },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_RECEPTION_SITE_ID,
					Constants.PAYMENT_SETTING_ATOBARAICOM_SITE },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_API_USER_ID,
					Constants.PAYMENT_SETTING_ATOBARAICOM_API_USER_ID },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_OPTIONAL_ORDER_NUMBER,
					orderFlag ? orderModel.OrderId : carts.OrderId },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_REMARKS_MEMO,
					orderFlag ? orderModel.ShippingMemo : carts.ShippingMemo },

				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_POSTAL_CODE_ORDER,
					orderFlag ? orderModel.Owner.OwnerZip : carts.Owner.Zip },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_ADDRESS_ORDER, orderFlag
					? string.Format("{0}{1}{2}{3}",
						orderModel.Owner.OwnerAddr1,
						orderModel.Owner.OwnerAddr2,
						orderModel.Owner.OwnerAddr3,
						orderModel.Owner.OwnerAddr4)
					: string.Format("{0}{1}{2}{3}",
						carts.Owner.Addr1,
						carts.Owner.Addr2,
						carts.Owner.Addr3,
						carts.Owner.Addr4) },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_NAME_ORDER,
					orderFlag ? orderModel.Owner.OwnerName : carts.Owner.Name },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_NAME_ORDER_KANA,
					orderFlag ? orderModel.Owner.OwnerNameKana : carts.Owner.NameKana },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_PHONE_NUMBER_ORDER, 
					orderFlag ? orderModel.Owner.OwnerTel1 : carts.Owner.Tel1},
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_MAIL_ADDRESS,
					orderFlag ? orderModel.Owner.OwnerMailAddr : carts.Owner.MailAddr },

				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_SPEC_DELIVERY_DESTINATION,
					orderFlag ? orderModel.Shippings.First().AnotherShippingFlg : shipping.AnotherShippingFlag },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_POSTAL_CODE_SHIPPING,
					orderFlag ? orderModel.Shippings.First().ShippingZip : shipping.Zip},
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_ADDRESS_SHIPING, orderFlag
					? string.Format("{0}{1}{2}{3}",
						orderModel.Shippings.First().ShippingAddr1,
						orderModel.Shippings.First().ShippingAddr2,
						orderModel.Shippings.First().ShippingAddr3,
						orderModel.Shippings.First().ShippingAddr4)
					: string.Format("{0}{1}{2}{3}", shipping.Addr1, shipping.Addr2, shipping.Addr3, shipping.Addr4) },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_NAME_SHIPPING,
					orderFlag ? orderModel.Shippings.First().ShippingName : shipping.Name},
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_NAME_SHIPPING_KANA,
					orderFlag ? orderModel.Shippings.First().ShippingNameKana : shipping.NameKana},
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_PHONE_NUMBER_SHIPPING,
					orderFlag ? orderModel.Shippings.First().ShippingTel1 : shipping.Tel1 },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_PRODUCT_SHIPPING,
					orderFlag ? orderModel.Shippings.First().ShippingNameKana : shipping.NameKana},
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_STORE_FEES,
					orderFlag ? orderModel.Shippings.First().ShippingTel1 : shipping.Tel1 },
			};

			var useAmount = 0m;
			var discountAmount = 0m;
			var index = 1;
			if (orderFlag)
			{
				if (orderModel.IsReturnOrder || orderModel.IsExchangeOrder)
				{
					CreateItemForOrderReturnExchange(orderModel);
				}

				foreach (var itemchild in orderModel.Items)
				{
					lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_PURCHASED_ITEM + index,
						itemchild.ProductName });

					// Calculate product price tax
					var itemPriceTax = TaxCalculationUtility.GetTaxPrice(
						itemchild.ProductPrice,
						itemchild.ProductTaxRate,
						Constants.TAX_EXCLUDED_FRACTION_ROUNDING);
					lstParam.Add(
						new[]
						{
							AtobaraicomConstants.ATOBARAICOM_API_REGIST_UNIT + index,
							Math.Truncate(TaxCalculationUtility.GetPriceTaxIncluded(itemchild.ProductPrice, itemPriceTax)).ToString()
						});
					lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_QUANTITY + index,
						itemchild.ItemQuantity.ToString() });
					lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_COMSUMPTION_TAX_RATE + index,
						itemchild.ProductTaxRate.ToString() });
					useAmount += TaxCalculationUtility.GetPriceTaxIncluded(
						(itemchild.ItemQuantity * itemchild.ProductPrice),
						(itemchild.ItemQuantity * itemPriceTax));
					index++;
				}

				// 送料以外の値引き額算出
				discountAmount = orderModel.MemberRankDiscountPrice
					+ orderModel.OrderCouponUse
					+ orderModel.SetpromotionPaymentChargeDiscountAmount
					+ orderModel.SetpromotionProductDiscountAmount
					+ orderModel.OrderPointUse
					+ orderModel.FixedPurchaseMemberDiscountAmount
					+ carts.FixedPurchaseDiscount;
				if (discountAmount > 0)
				{
					lstParam.Add(
						new[]
						{
							AtobaraicomConstants.ATOBARAICOM_API_REGIST_PURCHASED_ITEM + index,
							AtobaraicomConstants.ATOBARAICOM_API_TEXT_REGIST_PURCHASED_ITEM
						});
					lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_UNIT + index,
						Math.Truncate(discountAmount * -1).ToString() });
					lstParam.Add(
						new[]
						{
							AtobaraicomConstants.ATOBARAICOM_API_REGIST_QUANTITY + index,
							AtobaraicomConstants.ATOBARAICOM_API_TEXT_REGIST_QUANTITY
						});

					useAmount -= discountAmount;
					index++;
				}

				if (orderModel.OrderPriceRegulationTotal != 0
					|| (orderModel.IsReturnOrder
							&& (orderModel.LastBilledAmount > 0)
							&& (orderModel.Items.Length == 0)))
				{
					lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_PURCHASED_ITEM + index, "調整金額" });
					lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_UNIT + index,
						((orderModel.IsReturnOrder || orderModel.IsExchangeOrder)
							&& (orderModel.LastBilledAmount > 0))
								? Math.Truncate(orderModel.LastBilledAmount
									- orderModel.OrderPriceShipping
									- orderModel.OrderPriceExchange
									- useAmount).ToString()
								: Math.Truncate(orderModel.OrderPriceRegulationTotal).ToString() });
					lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_QUANTITY + index, "1" });

					useAmount += ((orderModel.IsReturnOrder || orderModel.IsExchangeOrder)
						&& (orderModel.LastBilledAmount > 0))
							? (orderModel.LastBilledAmount
								- orderModel.OrderPriceShipping
								- orderModel.OrderPriceExchange
								- useAmount)
							: orderModel.OrderPriceRegulationTotal;
					index++;
				}

				lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_PRODUCT_SHIPPING,
					Math.Truncate(orderModel.OrderPriceShipping
						- orderModel.SetpromotionShippingChargeDiscountAmount).ToString() });
				lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_STORE_FEES,
					Math.Truncate(orderModel.OrderPriceExchange).ToString() });

				useAmount += (orderModel.OrderPriceShipping
					+ orderModel.OrderPriceExchange
					- orderModel.SetpromotionShippingChargeDiscountAmount);
				lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_TOTAL_BILLED_AMOUNT,
					Math.Truncate(useAmount).ToString() });

				// 請求書同梱サービス利用時は、同梱フラグを見て制御
				if (Constants.PAYMENT_SETTING_ATOBARAICOM_USE_INVOICE_BUNDLE_SERVICE == true)
				{
					lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_SEPARATE_INVOICE,
						(orderModel.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON ? "0" : "1") });
				}

				return lstParam.ToArray();
			}
			else
			{
				var cartItems = carts.Items;
				if (cartItems != null)
				{
					// Add order price tax
					useAmount = TaxCalculationUtility.GetPriceTaxIncluded(useAmount, carts.PriceSubtotalTax);

					foreach (var itemchild in cartItems)
					{
						lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_PURCHASED_ITEM + index,
							itemchild.ProductName });
						lstParam.Add(
							new[]
							{
								AtobaraicomConstants.ATOBARAICOM_API_REGIST_UNIT + index,
								Math.Truncate(TaxCalculationUtility.GetPriceTaxIncluded(itemchild.Price, itemchild.PriceTax)).ToString()
							});
						lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_QUANTITY + index,
							itemchild.Count.ToString() });
						lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_COMSUMPTION_TAX_RATE + index,
							itemchild.TaxRate.ToString() });

						useAmount += (itemchild.Count * itemchild.Price);
						index++;
					}

					// 送料以外の値引き額算出
					discountAmount = carts.MemberRankDiscount
						+ carts.UseCouponPrice
						+ carts.SetPromotions.PaymentChargeDiscountAmount
						+ carts.SetPromotions.ProductDiscountAmount
						+ carts.UsePoint
						+ carts.FixedPurchaseMemberDiscountAmount
						+ carts.FixedPurchaseDiscount;
					if (discountAmount > 0)
					{
						lstParam.Add(
							new[]
							{
								AtobaraicomConstants.ATOBARAICOM_API_REGIST_PURCHASED_ITEM + index,
								AtobaraicomConstants.ATOBARAICOM_API_TEXT_REGIST_PURCHASED_ITEM
							});
						lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_UNIT + index,
							Math.Truncate(discountAmount * -1).ToString() });
						lstParam.Add(
							new[]
							{
								AtobaraicomConstants.ATOBARAICOM_API_REGIST_QUANTITY + index,
								AtobaraicomConstants.ATOBARAICOM_API_TEXT_REGIST_QUANTITY
							});

						useAmount -= discountAmount;
						index++;
					}

					if (carts.PriceRegulationTotal != 0)
					{
						lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_PURCHASED_ITEM + index, "調整金額" });
						lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_UNIT + index,
							Math.Truncate(carts.PriceRegulationTotal).ToString() });
						lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_QUANTITY + index, "1" });

						useAmount += carts.PriceRegulationTotal;
						index++;
					}
				}
				lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_PRODUCT_SHIPPING,
					Math.Truncate(carts.PriceShipping - carts.SetPromotions.ShippingChargeDiscountAmount).ToString() });
				lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_STORE_FEES,
					Math.Truncate(carts.PaymentPrice).ToString() });

				useAmount += (carts.PriceShipping
					+ carts.PaymentPrice
					- carts.SetPromotions.ShippingChargeDiscountAmount);
				lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_TOTAL_BILLED_AMOUNT,
					Math.Truncate(useAmount).ToString() });

				// 請求書同梱サービス利用時は、同梱フラグを見て制御
				if (Constants.PAYMENT_SETTING_ATOBARAICOM_USE_INVOICE_BUNDLE_SERVICE == true)
				{
					lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_SEPARATE_INVOICE,
						(carts.GetInvoiceBundleFlg() == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON ? "0" : "1") });
				}
			}

			if (input != null)
			{
				var user = new UserService().Get(StringUtility.ToEmpty(input[Constants.FIELD_USER_USER_ID]));
				lstParam.Add(new[] {
					AtobaraicomConstants.ATOBARAICOM_API_REGIST_IP_ADDRESS,
					(((string)input[Constants.FIELD_ORDER_ORDER_KBN] == Constants.FLG_ORDER_ORDER_KBN_TEL)
						|| string.IsNullOrEmpty(user.RemoteAddr))
							? StringUtility.ToEmpty(input[Constants.FIELD_ORDER_REMOTE_ADDR])
							: user.RemoteAddr });
			}
			return lstParam.ToArray();
		}

		/// <summary>
		/// Create item for order return exchange
		/// </summary>
		/// <param name="orderModel">Order model</param>
		public void CreateItemForOrderReturnExchange(OrderModel orderModel)
		{
			var orderService = new OrderService();
			var order = orderService.Get(orderModel.OrderIdOrg);
			var orderItems = orderService.GetRelatedOrderItems(orderModel.OrderIdOrg);

			var modelList = new List<OrderItemModel>();
			foreach (var item in orderItems)
			{
				if (modelList.Any(data => (data.ProductId == item.ProductId)
					&& (data.VariationId == item.VariationId)) == false)
				{
					modelList.Add(item);
				}
				else
				{
					modelList.Where(data => (data.ProductId == item.ProductId)
						&& (data.VariationId == item.VariationId))
						.Select(data => data.ItemQuantity += item.ItemQuantity).ToArray();
				}
			}

			// For return exchange order
			if (order.OrderId != orderModel.OrderId)
			{
				foreach (var item in orderModel.Shippings[0].Items)
				{
					if (modelList.Any(data => (data.ProductId == item.ProductId)
						&& (data.VariationId == item.VariationId)) == false)
					{
						modelList.Add(item);
					}
					else
					{
						modelList.Where(data => (data.ProductId == item.ProductId)
							&& (data.VariationId == item.VariationId))
							.Select(data => data.ItemQuantity += item.ItemQuantity).ToArray();
					}
				}
			}

			orderModel.Items = modelList.Where(data => (data.ItemQuantity > 0)).ToArray();
			orderModel.OrderPriceShipping = order.OrderPriceShipping;
			orderModel.OrderPriceExchange = order.OrderPriceExchange;
			orderModel.MemberRankDiscountPrice = order.MemberRankDiscountPrice;
			orderModel.OrderCouponUse = order.OrderCouponUse;
			orderModel.SetpromotionPaymentChargeDiscountAmount = order.SetpromotionPaymentChargeDiscountAmount;
			orderModel.SetpromotionProductDiscountAmount = order.SetpromotionProductDiscountAmount;
			orderModel.OrderPointUse = order.OrderPointUse;
			orderModel.FixedPurchaseMemberDiscountAmount = order.FixedPurchaseMemberDiscountAmount;
		}
		/// <summary>メッセージ</summary>
		public string Messages { get; set; }
	}
}
