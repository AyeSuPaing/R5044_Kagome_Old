/*
=========================================================================================================
  Module      : ヤマトKWC クレジット認証API(PaymentYamatoKwcCreditAuthApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;
using w2.Common.Util;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC クレジット認証API
	/// </summary>
	public class PaymentYamatoKwcCreditAuthApi : PaymentYamatoKwcApiBase
	{
		/// <summary>トークンを利用するか</summary>
		private readonly bool m_useToken;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="useToken">トークン利用するか</param>
		/// <param name="reserve1">予備項目設定値(テスト用）</param>
		public PaymentYamatoKwcCreditAuthApi(bool useToken, string reserve1 = "")
			: base(
				useToken ? PaymentYamatoKwcFunctionDiv.A08 : PaymentYamatoKwcFunctionDiv.A01,
				reserve1)
		{
			m_useToken = useToken;
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="deviceDiv">端末区分</param>
		/// <param name="paymentOrderId">決済注文番号</param>
		/// <param name="goodsName">商品名</param>
		/// <param name="settlePrice">決済金額</param>
		/// <param name="buyerNameKanji">購入者名（漢字）</param>
		/// <param name="buyerTel">購入者TEL</param>
		/// <param name="buyerEmail">購入者E-Mail</param>
		/// <param name="payWay">支払い回数（リボは0）</param>
		/// <param name="cardOptionServiceParam">オプションサービスパラメタ</param>
		/// <param name="orderId">注文ID</param>
		/// <returns>結果</returns>
		public PaymentYamatoKwcCreditAuthResponseData Exec(
			PaymentYamatoKwcDeviceDiv deviceDiv,
			string paymentOrderId,
			string goodsName,
			decimal settlePrice,
			string buyerNameKanji,
			string buyerTel,
			string buyerEmail,
			int payWay,
			PaymentYamatoKwcCreditOptionServiceParamBase cardOptionServiceParam,
			string orderId = "")
		{
			var param = m_useToken
				? CreateParamForUseToken(
					deviceDiv,
					paymentOrderId,
					goodsName,
					settlePrice,
					buyerNameKanji,
					buyerTel,
					buyerEmail,
					payWay,
					cardOptionServiceParam)
				: CreateParamForUseRegisterdCreditCard(
					deviceDiv,
					paymentOrderId,
					goodsName,
					settlePrice,
					buyerNameKanji,
					buyerTel,
					buyerEmail,
					payWay,
					cardOptionServiceParam,
					orderId);

			var resultString = PostHttpRequest(param);
			var responseData = new PaymentYamatoKwcCreditAuthResponseData(resultString);

			WriteLog(
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentProcessingType.CreditAuth,
				responseData.Success,
				new KeyValuePair<string, string>("paymentOrderId", paymentOrderId),
				new KeyValuePair<string, string>("cardOptionServiceParam", cardOptionServiceParam.ToString()),
				new KeyValuePair<string, string>("Error", string.Format("{0}({1})", responseData.ErrorMessage, responseData.ErrorCode)),
				new KeyValuePair<string, string>("CreditError", string.Format("{0}({1})", responseData.CreditErrorMessage, responseData.CreditErrorCode)),
				new KeyValuePair<string, string>("threeDAuthHtml", string.Format("{0}({1})", responseData.ThreeDAuthHtml, responseData.ThreeDAuthHtml)),
				new KeyValuePair<string, string>("threeDToken", string.Format("{0}({1})", responseData.ThreeDToken, responseData.ThreeDToken)));
			return responseData;
		}

		/// <summary>
		/// パラメタ作成（トークン利用）
		/// </summary>
		/// <param name="deviceDiv">端末区分</param>
		/// <param name="paymentOrderId">決済注文番号</param>
		/// <param name="goodsName">商品名</param>
		/// <param name="settlePrice">決済金額</param>
		/// <param name="buyerNameKanji">購入者名（漢字）</param>
		/// <param name="buyerTel">購入者TEL</param>
		/// <param name="buyerEmail">購入者E-Mail</param>
		/// <param name="payWay">支払い回数（リボは0）</param>
		/// <param name="cardOptionServiceParam">オプションサービスパラメタ</param>
		/// <returns>パラメタ</returns>
		private string[][] CreateParamForUseToken(
			PaymentYamatoKwcDeviceDiv deviceDiv,
			string paymentOrderId,
			string goodsName,
			decimal settlePrice,
			string buyerNameKanji,
			string buyerTel,
			string buyerEmail,
			int payWay,
			PaymentYamatoKwcCreditOptionServiceParamBase cardOptionServiceParam)
		{
			var param = new[]
			{
				new[] {"function_div",  this.FunctionDiv.ToString()},
				new[] {"trader_code", Constants.PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE},
				new[] {"device_div", ((int)deviceDiv).ToString()},
				new[] {"order_no", paymentOrderId},
				new[] {"settle_price", settlePrice.ToPriceString()},
				new[] {"buyer_name_kanji", buyerNameKanji},
				new[] {"buyer_tel", buyerTel},
				new[] {"buyer_email", buyerEmail},
				new[] {"pay_way", payWay.ToString()},
				new[] {"goods_name", goodsName},
				new[] {"card_code_api", StringUtility.ToEmpty(cardOptionServiceParam.CardCodeApi)},
				new[] {"token", cardOptionServiceParam.Token},
				new[] {"trader_ec_url", Constants.SOCIAL_LOGIN_PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC + Constants.PAGE_FRONT_PAYMENT_YAMATO_3DS_RESULT},
			};
			return param;
		}

		/// <summary>
		/// パラメタ作成（登録クレジットカード利用）
		/// </summary>
		/// <param name="deviceDiv">端末区分</param>
		/// <param name="paymentOrderId">決済注文番号</param>
		/// <param name="goodsName">商品名</param>
		/// <param name="settlePrice">決済金額</param>
		/// <param name="buyerNameKanji">購入者名（漢字）</param>
		/// <param name="buyerTel">購入者TEL</param>
		/// <param name="buyerEmail">購入者E-Mail</param>
		/// <param name="payWay">支払い回数（リボは0）</param>
		/// <param name="cardOptionServiceParam">オプションサービスパラメタ</param>
		/// <param name="orderId">注文ID</param>
		/// <returns>パラメタ</returns>
		private string[][] CreateParamForUseRegisterdCreditCard(
			PaymentYamatoKwcDeviceDiv deviceDiv,
			string paymentOrderId,
			string goodsName,
			decimal settlePrice,
			string buyerNameKanji,
			string buyerTel,
			string buyerEmail,
			int payWay,
			PaymentYamatoKwcCreditOptionServiceParamBase cardOptionServiceParam,
			string orderId)
		{
			var param = new[]
			{
				new[] { "function_div", this.FunctionDiv.ToString() },
				new[] { "trader_code", Constants.PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE },
				new[] { "device_div", ((int)deviceDiv).ToString() },
				new[] { "order_no", paymentOrderId },
				new[] { "settle_price", settlePrice.ToPriceString() },
				new[] { "buyer_name_kanji", buyerNameKanji },
				new[] { "buyer_tel", buyerTel },
				new[] { "buyer_email", buyerEmail },
				new[] { "auth_div", cardOptionServiceParam.AuthDiv },
				new[] { "pay_way", payWay.ToString() },
				new[] { "option_service_div", cardOptionServiceParam.OptionServiceDiv },
				new[] { "goods_name", goodsName },
				new[] { "card_code_api", StringUtility.ToEmpty(cardOptionServiceParam.CardCodeApi) },
				new[] { "card_no", string.Empty },
				new[] { "security_code", StringUtility.ToEmpty(cardOptionServiceParam.SecurityCode) },
				new[] { "card_owner", string.Empty },
				new[] { "card_exp", string.Empty },
				new[]
				{
					"trader_ec_url",
					new UrlCreator(Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC + Constants.PAGE_FRONT_ORDER_SETTLEMENT)
						.AddParam("odid", orderId)
						.CreateUrl()
				},
				new[] { "member_id", StringUtility.ToEmpty(cardOptionServiceParam.MembderId) },
				new[] { "authentication_key", StringUtility.ToEmpty(cardOptionServiceParam.AuthenticationKey) },
				new[] { "card_key", cardOptionServiceParam.CardKey },
				new[] { "last_credit_date", cardOptionServiceParam.LastCreditDate },
				new[] { "scheduled_shipping_date", string.Empty },	// 予約販売のみ
				new[] { "check_sum", StringUtility.ToEmpty(cardOptionServiceParam.CheckSum) },
				new[] { "reserve_1", this.Reserve1},
			};
			return param;
		}
	}
}
