<%--
=========================================================================================================
  Module      : 再計算(Recalculation.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="Botchan.Order.Recalculation" %>
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BotchanApi;
using w2.App.Common.Order.Botchan;
using w2.App.Common;
using w2.App.Common.Botchan;
using w2.App.Common.Global.Region;
using Validator = w2.Common.Util.Validator;

namespace Botchan.Order
{
	/// <summary>
	/// 再計算
	/// </summary>
	public class Recalculation : BotchanApiBase, IHttpHandler
	{
		/// <summary>
		/// Process request
		/// </summary>
		/// <param name="context">Context</param>
		public void ProcessRequest(HttpContext context)
		{
			BotChanApiProcess(context, w2.App.Common.Constants.BOTCHAN_API_NAME_RECALCULATION);
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
				var recalculationRequest = JsonConvert.DeserializeObject<RecalculationRequest>(requestContents);
				var recalculationApiFacade = new RecalculationApiFacade();

				var cartObject = recalculationApiFacade.CreateCartByRequest(recalculationRequest, ref errorList, ref memo);
				if (errorList.Count > 0) return new RecalculationResponse();
				var response = recalculationApiFacade.CreateResponseByCart(cartObject);

				return response;
		}

		/// <summary>
		/// BOTCHAN共通チェック
		/// </summary>
		/// <param name="requestContents">リクエスト文字列</param>
		/// <param name="apiName">API名</param>
		/// <returns>エラーリスト</returns>
		protected override List<BotchanMessageManager.MessagesCode> BotChanUtilityValidate(string requestContents, string apiName)
		{
			var recalculationRequest = JsonConvert.DeserializeObject<RecalculationRequest>(requestContents);
			var validate = new Hashtable { { "AuthText", recalculationRequest.AuthText } };
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
			var recalculationRequest = JsonConvert.DeserializeObject<RecalculationRequest>(requestContents);
			var requestWithoutProduct = CreateDataForValidationCheck(recalculationRequest);

			var errorList = Validator.Validate(
				Constants.CHECK_KBN_BOTCHAN_RECALCULATION,
				requestWithoutProduct,
				Constants.GLOBAL_OPTION_ENABLE ? RegionManager.GetInstance().Region.LanguageLocaleId : "",
				"");

			foreach (var product in recalculationRequest.OrderProductLists)
			{
				var orderProducts = new Hashtable
				{
					{ Constants.REQUEST_KEY_PRODUCT_ID, product.ProductId },
					{ Constants.REQUEST_KEY_VARIATION_ID, product.VariationId },
					{ Constants.REQUEST_KEY_PRODUCT_COUNT, product.ProductCount.ToString() },
				};
				var productErrorList = Validator.Validate(
					Constants.CHECK_KBN_BOTCHAN_RECALCULATION,
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
		protected Hashtable CreateDataForValidationCheck(RecalculationRequest request)
		{
			var result = new Hashtable
			{
				{ Constants.REQUEST_KEY_CART_ID, request.CartObject.CartId ?? "" },
				{ Constants.REQUEST_KEY_USER_ID, request.CartObject.UserId ?? "" },
				{ Constants.REQUEST_KEY_ORDER_DIVISION, request.CartObject.OrderDivision ?? "" },
				{ Constants.REQUEST_KEY_ORDER_KBN, request.CartObject.OrderKbn ?? "" },
				{ Constants.REQUEST_KEY_ADD_NOVELTY_FLAG, request.CartObject.AddNoveltyFlag ?? "" },
				{ Constants.REQUEST_KEY_RECOMMEND_FLAG, request.CartObject.RecommendFlag },
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
				{ Constants.REQUEST_KEY_SHIPPING_METHOD, request.OrderShippingObject.ShippingMethod ?? "" },
			};

			if (request.CartObject.OrderDivision == Constants.FLAG_ORDER_DIVISION_FIXED)
			{
				result.Add(Constants.REQUEST_KEY_FIXED_PURCHASE_KBN, request.OrderShippingObject.FixedPurchaseKbn ?? "");
				result.Add(Constants.REQUEST_KEY_COURSE_BUY_SETTING, request.OrderShippingObject.CourseBuySetting ?? "");
			}

			if (string.IsNullOrEmpty(request.CartObject.UserId)
				&& (request.OrderOwnerObject != null))
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
			return result;
		}

		/// <summary>Is Reusable</summary>
		public bool IsReusable
		{
			get { return false; }
		}
	}
}
