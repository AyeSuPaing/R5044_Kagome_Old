/*
=========================================================================================================
  Module      : 後払い変更注文API (AtobaraicomModifyOrderApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
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
	/// 後払い変更注文API
	/// </summary>
	public class AtobaraicomModifyOrderApi : AtobaraicomBaseApi
	{
		#region 注文を変更
		/// <summary>
		/// 注文の変更を実行
		/// </summary>
		/// <param name="input">User model</param>
		/// <param name="order">Order model</param>
		/// <returns>注文の変更を実行</returns>
		[Obsolete("このメソッドは未使用です。")]
		public bool ExecModifyOrder(UserModel input, OrderModel order)
		{
			var requestLog = new StringBuilder();
			var result = false;
			try
			{
				var responseData = new AtobaraicomModifyOrderResponse();
				var request = CreateParam(input, order);
				var responseString = this.PostHttpRequest(request,
					Constants.PAYMENT_SETTING_ATOBARAICOM_MODIFICATION_URL);
				var breadLine = "\r\n";
				var paramString = string.Join(
					",",
					request.Select(p => string.Format("{0}={1}", p[0], p[1], Encoding.UTF8)));
				requestLog.Append(breadLine).Append("Request API Modify: ").Append(breadLine);
				requestLog.Append("URL: " + Constants.PAYMENT_SETTING_ATOBARAICOM_CANCELATION_URL).Append(breadLine);
				requestLog.Append(paramString).Append(breadLine);
				WriteLog(
					requestLog.ToString(),
					null,
					string.Empty,
					Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
					PaymentFileLogger.PaymentProcessingType.Atobaraicom);

				responseData.HandleMessages(responseString);
			}
			catch (Exception ex)
			{
				w2.Common.Logger.FileLogger.WriteError("注文の変更を実行に失敗しました。", ex);
				result = false;
			}
			return result;
		}

		/// <summary>
		/// パラメータを作成
		/// </summary>
		/// <param name="user">ユーザーモデル</param>
		/// <param name="order">注文モデル</param>
		/// <returns>パラメータを作成</returns>
		private string[][] CreateParam(UserModel user, OrderModel order)
		{
			var lstParam = new List<string[]>()
			{
				new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_MERCHANT_ID,
					Constants.PAYMENT_SETTING_ATOBARAICOM_ENTERPRISED },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_API_USER_ID,
					Constants.PAYMENT_SETTING_ATOBARAICOM_API_USER_ID },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_ORDER_ID, order.OrderId },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_OPTIONAL_ORDER_NUMBER, order.PaymentOrderId }
			};

			var shippings = order.Shippings.FirstOrDefault();
			if (shippings != null)
			{
				lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_SHIPPING_NAME_KANA,
					shippings.ShippingName });
				lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_SHIPPING_STREET_ADDRESS,
					string.Format("{0}{1}{2}{3}{4}{5}",
						shippings.ShippingAddr1,
						shippings.ShippingAddr2,
						shippings.ShippingAddr2,
						shippings.ShippingAddr3,
						shippings.ShippingAddr4,
						shippings.ShippingAddr5) });
				lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_SHIPPING_POSTAL_CODE,
					shippings.ShippingZip });
			}

			var index = 1;
			var itemCarts = order.Items;
			if ((itemCarts != null) && itemCarts.Any())
			{
				foreach (var itemchild in itemCarts)
				{
					if (itemchild != null)
					{
						lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_PURCHASED_ITEM + index,
							itemchild.ProductName });
						lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_UNIT + index,
							itemchild.ProductPrice.ToString() });
						lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_QUANTITY + index,
							itemchild.ProductSetCount.ToString() });
						lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_COMSUMPTION_TAX_RATE + index,
							itemchild.ProductTaxRate.ToString() });
					}
					index++;
				}
			}
			return lstParam.ToArray();
		}
		#endregion

		#region すべてのアイテムの注文を変更
		/// <summary>
		/// 実行 後払い変更注文(後払い) 決済取消
		/// </summary>
		/// <param name="order">注文モデル</param>
		/// <returns>すべてのアイテムの注文変更を実行</returns>
		public bool ExecModifyOrderAllItem(OrderModel order)
		{
			var requestLog = new StringBuilder();
			var responseLog = new StringBuilder();
			var result = false;
			this.Messages = AtobaraicomConstants.ATOBARAICOM_API_AUTH_RESULT_STATUS_OK;
			var responseData = new AtobaraicomModifyOrderAllItemResponse();
			try
			{
				var request = CreateParamAllItem(order);
				var responseString = this.PostHttpRequest(request,
					Constants.PAYMENT_SETTING_ATOBARAICOM_ALL_MODIFICATION_URL);
				var breadLine = "\r\n";
				var paramString = string.Join(
					",",
					request.Select(p => string.Format("{0}={1}", p[0], p[1], Encoding.UTF8)));
				requestLog.Append(breadLine).Append("Request API All Item: ").Append(breadLine);
				requestLog.Append("URL: " + Constants.PAYMENT_SETTING_ATOBARAICOM_ALL_MODIFICATION_URL).Append(breadLine);
				requestLog.Append(paramString).Append(breadLine);
				WriteLog(
					requestLog.ToString(),
					null,
					string.Empty,
					Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
					PaymentFileLogger.PaymentProcessingType.Atobaraicom);
				responseData.HandleMessages(responseString);

				responseLog.Append(breadLine).Append("Request Response: ").Append(breadLine);
				responseLog.Append(responseString).Append(breadLine);
				WriteLog(
					responseLog.ToString(),
					null,
					string.Empty,
					Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
					PaymentFileLogger.PaymentProcessingType.Atobaraicom);

				if (string.IsNullOrEmpty(responseString)
					|| responseData.IsError)
				{
					this.Messages = responseData.Messages.ToString();
					this.Status = responseData.Status;
				}

				if (responseData.IsAuthorizeOK)
				{
					this.OrderStatus = responseData.OrderStatus;
					result = true;
				}
				else if (responseData.IsAuthorizeNG)
				{
					this.OrderStatus = responseData.OrderStatus;
				}
				else if (responseData.IsAuthorizeHold)
				{
					this.OrderStatus = responseData.OrderStatus;
				}
			}
			catch (Exception ex)
			{
				this.Messages = "API Fail " + ex.Message;
				w2.Common.Logger.FileLogger.WriteError("注文の変更を実行に失敗しました。", ex);
			}

			PaymentFileLogger.WritePaymentLog(
				true,
				order.OrderId,
				PaymentFileLogger.PaymentType.Atobaraicom,
				PaymentFileLogger.PaymentProcessingType.ExecPayment,
				this.Messages,
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, order.OrderId },
					{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, order.PaymentOrderId },
				});

			return result;
		}

		/// <summary>
		/// パラメータ作成 後払い変更注文(後払い) 決済取消
		/// </summary>
		/// <param name="order">注文モデル</param>
		/// <returns>すべてのパラメータを作成</returns>
		private string[][] CreateParamAllItem(OrderModel order)
		{
			var lstParam = new List<string[]>()
			{
				new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_MERCHANT_ID,
					Constants.PAYMENT_SETTING_ATOBARAICOM_ENTERPRISED },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_API_USER_ID,
					Constants.PAYMENT_SETTING_ATOBARAICOM_API_USER_ID },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_ORDER_DAY, DateTime.Now.ToString("yyyyMMdd") },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_ORDER_ID, order.PaymentOrderId },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_RECEPTION_SITE_ID,
					Constants.PAYMENT_SETTING_ATOBARAICOM_SITE },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_OPTIONAL_ORDER_NUMBER, order.OrderId },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_POSTAL_CODE, order.Owner.OwnerZip },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_ADDRESS,
					string.Format("{0}{1}",
						order.Owner.ConcatenateAddressWithoutCountryName(),
						order.Owner.OwnerAddrCountryName) },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_ORDER_NAME, order.Owner.OwnerName },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_NAME_KANA, order.Owner.OwnerNameKana },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_TEL_NO, order.Owner.OwnerTel1 },
				new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_MAIL_ADDRESS, order.Owner.OwnerMailAddr },
			};

			var shippings = order.Shippings.FirstOrDefault();
			if (shippings != null)
			{
				lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_SHIPPING_POSTAL_CODE,
					shippings.ShippingZip });
				lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_SHIPPING_STREET_ADDRESS,
					string.Format("{0}{1}{2}{3}{4}{5}",
						shippings.ShippingAddr1,
						shippings.ShippingAddr2,
						shippings.ShippingAddr2,
						shippings.ShippingAddr3,
						shippings.ShippingAddr4,
						shippings.ShippingAddr5) });
				lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_SHIPPING_NAME, shippings.ShippingName });
				lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_SHIPPING_TEL_NO, shippings.ShippingTel1 });
			}

			var index = 1;
			var itemCarts = order.Items;
			if ((itemCarts != null) && itemCarts.Any())
			{
				foreach (var itemchild in itemCarts)
				{
					if (itemchild != null)
					{
						lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_PURCHASED_ITEM + index,
							itemchild.ProductName });

						// Calculate product price tax
						var itemPriceTax = TaxCalculationUtility.GetTaxPrice(
							itemchild.ProductPrice,
							itemchild.ProductTaxRate,
							Constants.TAX_EXCLUDED_FRACTION_ROUNDING);
						lstParam.Add(
							new[]
							{
								AtobaraicomConstants.ATOBARAICOM_API_MODIFY_UNIT + index,
								Math.Truncate(TaxCalculationUtility.GetPriceTaxIncluded(itemchild.ProductPrice, itemPriceTax)).ToString()
							});
						lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_QUANTITY + index,
							itemchild.ItemQuantity.ToString() });
						lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_CONSUMPTION_TAX_RATE + index,
							StringUtility.ToEmpty(itemchild.ProductTaxRate) });
					}
					index++;
				}

				// 送料以外の値引き額算出
				var discountAmount = (order.OrderPriceDiscountTotal - order.SetpromotionShippingChargeDiscountAmount);
				if (discountAmount > 0)
				{
					lstParam.Add(
						new[]
						{
							AtobaraicomConstants.ATOBARAICOM_API_MODIFY_PURCHASED_ITEM + index,
							AtobaraicomConstants.ATOBARAICOM_API_TEXT_REGIST_PURCHASED_ITEM
						});

					lstParam.Add(
						new[]
						{
							AtobaraicomConstants.ATOBARAICOM_API_MODIFY_UNIT + index,
							Math.Truncate(discountAmount * -1).ToString()
						});

					lstParam.Add(
						new[]
						{
							AtobaraicomConstants.ATOBARAICOM_API_MODIFY_QUANTITY + index,
							AtobaraicomConstants.ATOBARAICOM_API_TEXT_REGIST_QUANTITY
						});

					index++;
				}

				lstParam.Add(
					new[]
					{
						AtobaraicomConstants.ATOBARAICOM_API_MODIFY_PURCHASED_ITEM + index,
						AtobaraicomConstants.ATOBARAICOM_API_TEXT_ITEM_NAME_PRICE_REGULATION
					});

				lstParam.Add(
					new[]
					{
						AtobaraicomConstants.ATOBARAICOM_API_MODIFY_UNIT + index,
						Math.Truncate(order.OrderPriceRegulationTotal).ToString()
					});

				lstParam.Add(
					new[]
					{
						AtobaraicomConstants.ATOBARAICOM_API_MODIFY_QUANTITY + index,
						AtobaraicomConstants.ATOBARAICOM_API_TEXT_REGIST_QUANTITY
					});

				lstParam.Add(
					new[]
					{
						AtobaraicomConstants.ATOBARAICOM_API_MODIFY_PRODUCT_SHIPPING,
						Math.Truncate(order.OrderPriceShipping - order.SetpromotionShippingChargeDiscountAmount).ToString()
					});

				lstParam.Add(
					new[]
					{
						AtobaraicomConstants.ATOBARAICOM_API_MODIFY_STORE_FEES,
						Math.Truncate(order.OrderPriceExchange).ToString()
					});

				lstParam.Add(
					new[]
					{
						AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_TOTAL_BILLED_AMOUNT,
						Math.Truncate(order.OrderPriceTotal).ToString()
					});
			}

			// 請求書同梱サービス利用時は、同梱フラグを見て制御
			if (Constants.PAYMENT_SETTING_ATOBARAICOM_USE_INVOICE_BUNDLE_SERVICE == true)
			{
				lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_REGIST_SEPARATE_INVOICE,
					(order.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON ? "0" : "1") });
			}

			var isSameAddress = (order.Owner.ConcatenateAddressWithoutCountryName() == shippings.ConcatenateAddressWithoutCountryName());
			lstParam.Add(new[] { AtobaraicomConstants.ATOBARAICOM_API_MODIFY_ALL_SPECIFY_ANOTHER_DELIVERY_DESTINATION, (isSameAddress ? "0" : "1") });

			return lstParam.ToArray();
		}

		/// <summary>応答文字列</summary>
		public string ResponseString { get; set; }
		/// <summary>メッセージ</summary>
		public string Messages { get; set; }
		/// <summary>Status</summary>
		public string Status { get; set; }
		/// <summary>Order status</summary>
		public string OrderStatus { get; set; }
		/// <summary>Is authorize NG</summary>
		public bool IsAuthorizeNG
		{
			get
			{
				return (this.OrderStatus == AtobaraicomConstants.ATOBARAICOM_API_AUTH_RESULT_STATUS_NG);
			}
		}
		/// <summary>Is during authorize</summary>
		public bool IsAuthorizeHold
		{
			get
			{
				return (this.OrderStatus == AtobaraicomConstants.ATOBARAICOM_API_AUTH_RESULT_STATUS_HOLD);
			}
		}
		#endregion
	}
}
