<%--
=========================================================================================================
  Module      : 注文登録(OrderRegister.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="Botchan.Order.OrderRegister" %>
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using BotchanApi;
using w2.App.Common;
using w2.App.Common.Botchan;
using w2.App.Common.Global.Region;
using w2.App.Common.Order.Botchan;
using w2.App.Common.Order;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.Payment;
using Validator = w2.Common.Util.Validator;

namespace Botchan.Order
{
	/// <summary>
	/// 注文登録
	/// </summary>
	public class OrderRegister : BotchanApiBase, IHttpHandler
	{
		/// <summary>
		/// Process Request
		/// </summary>
		/// <param name="context">Context</param>
		public void ProcessRequest(HttpContext context)
		{
			BotChanApiProcess(context, w2.App.Common.Constants.BOTCHAN_API_NAME_ORDER_REGISTER);
		}

		/// <summary>
		/// レスポンス取得
		/// </summary>
		/// <param name="context">httpコンテキスト</param>
		/// <param name="requestContents">リクエスト文字列</param>
		/// <param name="errorList">エラーリスト</param>
		/// <param name="memo">メモ</param>
		/// <returns>レスポンス</returns>
		protected override object GetResponseData(
			HttpContext context,
			string requestContents,
			ref List<BotchanMessageManager.MessagesCode> errorList,
			ref string memo)
		{
			var orderRegisterRequest = JsonConvert.DeserializeObject<OrderRegisterRequest>(requestContents);
			var orderRegisterApiFacade = new OrderRegisterApiFacade(true);

			orderRegisterRequest.OwnerKbn = (Constants.SMARTPHONE_OPTION_ENABLED
				&& (orderRegisterRequest.CartObject.OrderKbn == Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE))
					? string.IsNullOrEmpty(orderRegisterRequest.CartObject.UserId)
						? Constants.FLG_ORDEROWNER_OWNER_KBN_SMARTPHONE_GUEST
						: Constants.FLG_ORDEROWNER_OWNER_KBN_SMARTPHONE_USER
					: string.IsNullOrEmpty(orderRegisterRequest.CartObject.UserId)
						? Constants.FLG_ORDEROWNER_OWNER_KBN_PC_GUEST
						: Constants.FLG_ORDEROWNER_OWNER_KBN_PC_USER;

			var afterOrderRecommendFlag = orderRegisterRequest.CartObject.AfterOrderRecommendFlag;
			var orderRegisterResponse = new OrderRegisterResponse();
			var oldOrder = new OrderModel();
			var otherInfo = new Hashtable();

			switch (afterOrderRecommendFlag)
			{
				case Constants.FLAG_AFTER_ORDER_RECOMMEND_NONE:
					{
						var cart = orderRegisterApiFacade.CreateCartByRequest(orderRegisterRequest, errorList, ref memo);
						if ((cart == null) || (errorList.Count > 0)) return orderRegisterResponse;
						orderRegisterResponse = orderRegisterApiFacade.RegisterOrderByCart(cart, errorList, ref memo);

						break;
					}
				case Constants.FLAG_AFTER_ORDER_RECOMMEND_PLAN:
					{
						var cart = orderRegisterApiFacade.CreateCartByOldOrder(orderRegisterRequest, errorList, ref memo, ref oldOrder, ref otherInfo);
						if ((cart == null) || (errorList.Count > 0)) return orderRegisterResponse;
						orderRegisterResponse = orderRegisterApiFacade.CreateResponseByRecommendItem(cart, errorList, ref memo);

						break;
					}
				case Constants.FLAG_AFTER_ORDER_RECOMMEND_CONFIRMATION:
					{
						var cart = orderRegisterApiFacade.CreateCartByOldOrder(orderRegisterRequest, errorList, ref memo,
							ref oldOrder, ref otherInfo);
						if ((cart == null) || (errorList.Count > 0)) return orderRegisterResponse;

						var isReauthComplete = false;
						try
						{

							orderRegisterResponse = orderRegisterApiFacade.RegisterOrderByRecommend(cart, errorList, oldOrder, otherInfo, ref memo, ref isReauthComplete);
						}
						catch (Exception exception)
						{
							FileLogger.WriteError(
								string.Format(
									"注文後レコメンド処理に失敗しました。order_id: {0}",
									oldOrder.OrderId),
								exception);

							if (isReauthComplete)
							{
								SendMail(oldOrder);
							}
						}

						break;
					}
				default:
					{
						var cart = orderRegisterApiFacade.CreateCartByRequest(orderRegisterRequest, errorList, ref memo);
						if ((cart == null) || (errorList.Count > 0)) return orderRegisterResponse;
						orderRegisterResponse = orderRegisterApiFacade.RegisterOrderByCart(cart, errorList, ref memo);

						break;
					}
			}

			return orderRegisterResponse;
		}

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="oldOrder">旧受注情報</param>
		private void SendMail(OrderModel oldOrder)
		{
			if (oldOrder != null)
			{
				var title = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.BOTCHAN_AFTER_REAUTH_ERROR_MAIL_TITLE.ToString());
				var body = string.Format(
					MessageManager.GetMessages(BotchanMessageManager.MessagesCode.BOTCHAN_AFTER_REAUTH_ERROR_MAIL_BODY.ToString()),
					oldOrder.UserId,
					new PaymentService().GetPaymentName(oldOrder.ShopId, oldOrder.OrderPaymentKbn),
					oldOrder.OrderId);

				if (new OrderEventBinder().SendMail(title, body) == false)
				{
					FileLogger.WriteError(
						string.Format("レコメンド処理でエラーメール送信失敗しました！\r\n受注ID：{0}",
						oldOrder.OrderId));
				}
			}
		}

		/// <summary>
		/// BOTCHAN共通チェック
		/// </summary>
		/// <param name="requestContents">リクエスト文字列</param>
		/// <param name="apiName">API名</param>
		/// <returns>エラーリスト</returns>
		protected override List<BotchanMessageManager.MessagesCode> BotChanUtilityValidate(string requestContents, string apiName)
		{
			var productSearchRequest = JsonConvert.DeserializeObject<OrderRegisterRequest>(requestContents);
			var validate = new Hashtable { { "AuthText", productSearchRequest.AuthText } };
			var errorList = BotChanUtility.ValidateRequest(validate, apiName);
			return errorList;
		}

		/// <summary>
		/// パラメータバリエーション
		/// </summary>
		/// <param name="requestContents">リクエスト文字列</param>
		/// <returns>エラーリスト</returns>
		protected override Validator.ErrorMessageList ParametersValidate(string requestContents)
		{
			var orderRegisterRequest = JsonConvert.DeserializeObject<OrderRegisterRequest>(requestContents);
			var requestWithoutProduct = CreateDataForValidationCheck(orderRegisterRequest);

			var errorList = Validator.Validate(
					Constants.CHECK_KBN_BOTCHAN_ORDER_REGISTER,
					requestWithoutProduct,
					Constants.GLOBAL_OPTION_ENABLE ? RegionManager.GetInstance().Region.LanguageLocaleId : "",
					"");

			var afterOrderRecommendFlag = orderRegisterRequest.CartObject.AfterOrderRecommendFlag;
			if ((afterOrderRecommendFlag == Constants.FLAG_AFTER_ORDER_RECOMMEND_PLAN)
				|| (afterOrderRecommendFlag == Constants.FLAG_AFTER_ORDER_RECOMMEND_CONFIRMATION))
			{
				return errorList;
			}

			foreach (var product in orderRegisterRequest.OrderProductLists)
			{
				var orderProducts = new Hashtable
				{
					{Constants.REQUEST_KEY_PRODUCT_ID, product.ProductId},
					{Constants.REQUEST_KEY_VARIATION_ID, product.VariationId},
					{Constants.REQUEST_KEY_PRODUCT_COUNT, product.ProductCount.ToString()},

				};
				var productErrorList = Validator.Validate(
					Constants.CHECK_KBN_BOTCHAN_ORDER_REGISTER,
					orderProducts,
					Constants.GLOBAL_OPTION_ENABLE ? RegionManager.GetInstance().Region.LanguageLocaleId : "",
					"");
				errorList.AddRange(productErrorList
					.Select(error => new KeyValuePair<string, string>(
						string.Format(
							"[{0}]{1}",
							product.ProductId,
							error.Key),
						error.Value)));
			}

			return errorList;
		}

		/// <summary>
		/// バリデーションチェック用データ作成
		/// </summary>
		/// <param name="request">リクエスト</param>
		/// <returns>データ</returns>
		protected Hashtable CreateDataForValidationCheck(OrderRegisterRequest request)
		{

			Hashtable result;

			var afterOrderRecommendFlag = request.CartObject.AfterOrderRecommendFlag;
			if ((afterOrderRecommendFlag == Constants.FLAG_AFTER_ORDER_RECOMMEND_PLAN)
				|| (afterOrderRecommendFlag == Constants.FLAG_AFTER_ORDER_RECOMMEND_CONFIRMATION))
			{
				result = new Hashtable
				{
					{ Constants.REQUEST_KEY_AFTER_ORDER_RECOMMEND_FLAG, request.CartObject.AfterOrderRecommendFlag ?? "" },
					{ Constants.REQUEST_KEY_OLD_ORDER_ID, request.CartObject.OldOrderId ?? "" },
				};
			}
			else
			{
				result = new Hashtable
				{
					{ Constants.REQUEST_KEY_CART_ID, request.CartObject.CartId ?? "" },
					{ Constants.REQUEST_KEY_USER_ID, request.CartObject.UserId ?? "" },
					{ Constants.REQUEST_KEY_ORDER_DIVISION, request.CartObject.OrderDivision ?? "" },
					{ Constants.REQUEST_KEY_ORDER_KBN, request.CartObject.OrderKbn ?? "" },
					{ Constants.REQUEST_KEY_ADD_NOVELTY_FLAG, request.CartObject.AddNoveltyFlag ?? "" },
					{ Constants.REQUEST_KEY_ADV_CODE, request.CartObject.AdvCode ?? "" },
					{ Constants.REQUEST_KEY_NAME, request.OrderShippingObject.Name ?? "" },
					{ Constants.REQUEST_KEY_SHIPPING_ZIP, request.OrderShippingObject.ShippingZip ?? "" },
					{ Constants.REQUEST_KEY_SHIPPING_NAME, request.OrderShippingObject.ShippingName ?? "" },
					{ Constants.REQUEST_KEY_SHIPPING_NAME1, request.OrderShippingObject.ShippingName1 ?? "" },
					{ Constants.REQUEST_KEY_SHIPPING_NAME2, request.OrderShippingObject.ShippingName2 ?? "" },
					{ Constants.REQUEST_KEY_SHIPPING_NAME_KANA, request.OrderShippingObject.ShippingNameKana ?? "" },
					{ Constants.REQUEST_KEY_SHIPPING_NAME_KANA1, request.OrderShippingObject.ShippingNameKana1 ?? "" },
					{ Constants.REQUEST_KEY_SHIPPING_NAME_KANA2, request.OrderShippingObject.ShippingNameKana2 ?? "" },
					{ Constants.REQUEST_KEY_SHIPPING_ADDR1, request.OrderShippingObject.ShippingAddr1 ?? "" },
					{ Constants.REQUEST_KEY_SHIPPING_ADDR2, request.OrderShippingObject.ShippingAddr2 ?? "" },
					{ Constants.REQUEST_KEY_SHIPPING_ADDR3, request.OrderShippingObject.ShippingAddr3 ?? "" },
					{ Constants.REQUEST_KEY_SHIPPING_ADDR4, request.OrderShippingObject.ShippingAddr4 ?? "" },
					{ Constants.REQUEST_KEY_SHIPPING_TEL1, request.OrderShippingObject.ShippingTel1 ?? "" },
					{ Constants.REQUEST_KEY_SHIPPING_COMPANY_NAME, request.OrderShippingObject.ShippingCompanyName ?? "" },
					{ Constants.REQUEST_KEY_SHIPPING_COMPANY_POST_NAME, request.OrderShippingObject.ShippingCompanyPostName ?? "" },
					{ Constants.REQUEST_KEY_PAYMENT_ID, request.OrderPaymentObject.PaymentId ?? "" },
					{ Constants.REQUEST_KEY_RECEIPT_FLG, request.OrderPaymentObject.ReceiptFlg ?? "" },
					{ Constants.REQUEST_KEY_ORDER_POINT_USE, (request.DiscountInfoObject != null) ? request.DiscountInfoObject.OrderPointUse ?? "" : null },
					{ Constants.REQUEST_KEY_COUPON_CODE, (request.DiscountInfoObject != null) ? request.DiscountInfoObject.CouponCode ?? "" : null },
					{ Constants.REQUEST_KEY_AFTER_ORDER_RECOMMEND_FLAG, request.CartObject.AfterOrderRecommendFlag ?? "" },
					{ Constants.REQUEST_KEY_SHIPPING_METHOD, request.OrderShippingObject.ShippingMethod ?? "" },
				};

				if (request.CartObject.OrderDivision == Constants.FLAG_ORDER_DIVISION_FIXED)
				{
					result.Add(Constants.REQUEST_KEY_FIXED_PURCHASE_KBN, request.OrderShippingObject.FixedPurchaseKbn ?? "");
					result.Add(Constants.REQUEST_KEY_COURSE_BUY_SETTING, request.OrderShippingObject.CourseBuySetting ?? "");
				}

				if (string.IsNullOrEmpty(request.CartObject.UserId))
				{
					result.Add(Constants.REQUEST_KEY_ORDER_OWNER_NAME1, request.OrderOwnerObject.Name1 ?? "");
					result.Add(Constants.REQUEST_KEY_ORDER_OWNER_NAME2, request.OrderOwnerObject.Name2 ?? "");
					result.Add(Constants.REQUEST_KEY_ORDER_OWNER_NAME_KANA1, request.OrderOwnerObject.NameKana1 ?? "");
					result.Add(Constants.REQUEST_KEY_ORDER_OWNER_NAME_KANA2, request.OrderOwnerObject.NameKana2 ?? "");
					result.Add(Constants.REQUEST_KEY_ORDER_OWNER_BIRTH, request.OrderOwnerObject.Birth ?? "");
					result.Add(Constants.REQUEST_KEY_ORDER_OWNER_SEX, request.OrderOwnerObject.Sex ?? "");
					result.Add(Constants.REQUEST_KEY_ORDER_MAIL_ADDR, request.OrderOwnerObject.MailAddr ?? "");
					result.Add(Constants.REQUEST_KEY_ORDER_ZIP, request.OrderOwnerObject.Zip ?? "");
					result.Add(Constants.REQUEST_KEY_ORDER_ADDR1, request.OrderOwnerObject.Addr1 ?? "");
					result.Add(Constants.REQUEST_KEY_ORDER_ADDR2, request.OrderOwnerObject.Addr2 ?? "");
					result.Add(Constants.REQUEST_KEY_ORDER_ADDR3, request.OrderOwnerObject.Addr3 ?? "");
					result.Add(Constants.REQUEST_KEY_ORDER_ADDR4, request.OrderOwnerObject.Addr4 ?? "");
					result.Add(Constants.REQUEST_KEY_ORDER_TEL1, request.OrderOwnerObject.Tel1 ?? "");
				}

				if (request.OrderPaymentObject.PaymentId == w2.Database.Common.Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				{
					if (request.OrderPaymentObject.BranchNo == 0)
					{
						result.Add(Constants.REQUEST_KEY_CREDIT_TOKEN, request.OrderPaymentObject.CreditToken ?? "");
						result.Add(Constants.REQUEST_KEY_CREDIT_CARD_NO, request.OrderPaymentObject.CreditCardNo ?? "");
						result.Add(Constants.REQUEST_KEY_EXPIRATION_MONTH, (request.OrderPaymentObject.ExpirationMonth != 0)
							? request.OrderPaymentObject.ExpirationMonth.ToString("00")
							: "");
						result.Add(Constants.REQUEST_KEY_EXPIRATION_YEAR, (request.OrderPaymentObject.ExpirationYear != 0)
							? request.OrderPaymentObject.ExpirationYear.ToString("00")
							: "");
					}
					result.Add(Constants.REQUEST_KEY_AUTHOR_NAME, request.OrderPaymentObject.AuthorName ?? "");
					result.Add(Constants.REQUEST_KEY_CREDIT_SECURITY_CODE, request.OrderPaymentObject.CreditSecurityCode ?? "");
					result.Add(Constants.REQUEST_KEY_CREDIT_INSTALLMENTS, request.OrderPaymentObject.CreditInstallments ?? "");
					result.Add(Constants.REQUEST_KEY_CREDIT_REGIST_FLAG, request.OrderPaymentObject.CreditRegistrationFlag ?? "");
				}
			}
			return result;
		}

		/// <summary>Is Reusable</summary>
		public bool IsReusable
		{
			get { return false; }
		}
	}
}
