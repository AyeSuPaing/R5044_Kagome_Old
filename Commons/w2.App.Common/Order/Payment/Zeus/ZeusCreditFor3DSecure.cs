/*
=========================================================================================================
  Module      : 3Dセキュア向けゼウスクレジット決済モジュール(PaymentZeusCredit.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using w2.App.Common.Extensions.Currency;
using w2.Common.Extensions;
using w2.Common.Helper;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.User.Helper;
using w2.Domain.UserShipping;

namespace w2.App.Common.Order.Payment.Zeus
{
	/// <summary>
	/// 3Dセキュア向けゼウスクレジット決済モジュール
	/// </summary>
	public class ZeusCreditFor3DSecure : ZeusApiBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ZeusCreditFor3DSecure()
			: base(Constants.PAYMENT_SETTING_ZEUS_SECURE_API_AUTH_SERVER_URL)
		{
			// 加盟店認証キーセット
			this.SecureApiAuthKey = Constants.PAYMENT_SETTING_ZEUS_SECURE_API_AUTH_KEY;
		}

		/// <summary>
		/// SecureApi決済で本人確認
		/// </summary>
		/// <param name="htOrder">注文情報</param>
		/// <param name="coCart">カート情報</param>
		/// <returns>True:成功 False:失敗</returns>
		/// <remarks></remarks>
		public bool SecureApiAuth(Hashtable htOrder, CartObject coCart)
		{
			// 送信用XML作成
			XmlDocument xdSendData = CreateOrderInfoXml(coCart);

			// POST送信
			XmlDocument xdPostDataResult = SendXml(xdSendData);

			// レスポンス解析
			string strPostDataResult = xdPostDataResult.SelectSingleNode("./response/result/status")?.InnerText;

			bool blSuccess = false;

			var orderId = (string)htOrder[Constants.FIELD_ORDERCOUPON_ORDER_ID];
			switch (strPostDataResult)
			{
				// 成功
				case "success":
					blSuccess = true;

					var threeDs2Flag = xdPostDataResult.SelectSingleNode("./response/threeDS2flag")?.InnerText;

					// レスポンスの値を保存
					new OrderService().Update3DSecureInfo(
						(string)htOrder[Constants.FIELD_ORDER_ORDER_ID],
						xdPostDataResult.SelectSingleNode("./response/xid")?.InnerText,
						(threeDs2Flag == ZeusFlag.On.ToText())
							? xdPostDataResult.SelectSingleNode("./response/iframeUrl")?.InnerText
							: xdPostDataResult.SelectSingleNode("./response/redirection/acs_url")?.InnerText,
						xdPostDataResult.SelectSingleNode("./response/redirection/PaReq")?.InnerText,
						Constants.FLG_LASTCHANGED_USER,
						UpdateHistoryAction.DoNotInsert);

					WriteLog(
						PaymentFileLogger.PaymentProcessingType.ThreeDSecureRequest,
						blSuccess,
						new KeyValuePair<string, string>("orderId", orderId),
						new KeyValuePair<string, string>("PriceTotal", coCart.SendingAmount.ToPriceString()));
					break;

				// 3Dセキュア対象外
				case "outside":
					// 決済処理
					blSuccess = Auth(xdPostDataResult.SelectSingleNode("./response/xid")?.InnerText);
					WriteLog(
						PaymentFileLogger.PaymentProcessingType.Outside,
						blSuccess,
						new KeyValuePair<string, string>("orderId", orderId),
						new KeyValuePair<string, string>("PriceTotal", coCart.SendingAmount.ToPriceString()));
					break;

				// その他（失敗）
				default:
					WriteLog(
						PaymentFileLogger.PaymentProcessingType.ThreeDSecureRequest,
						blSuccess,
						new KeyValuePair<string, string>("orderId", orderId),
						new KeyValuePair<string, string>("PriceTotal", coCart.SendingAmount.ToPriceString()));
					// ログ出力
					var errorCode = xdPostDataResult.SelectSingleNode("./response/result/code")?.InnerText;
					this.ErrorMessage = "注文情報送信:" + GetSecureApiErrorMessage(strPostDataResult, errorCode);
					this.ErrorTypeCode = errorCode?.Substring(errorCode.Length - 3);
					PaymentFileLogger.WritePaymentLog(
						false,
						Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
						PaymentFileLogger.PaymentType.Zeus,
						PaymentFileLogger.PaymentProcessingType.IdentificationBySecureApi,
						this.ErrorMessage);
					break;
			}
			return blSuccess;
		}

		/// <summary>
		/// 本人認証結果チェック
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="str3DSecureAuthResult">本人認証結果</param>
		/// <param name="strTranId">ゼウストランザクションID</param>
		/// <returns>True:本人認証成功 False:本人認証失敗</returns>
		public bool Check3DSecureAuthResult(string orderId, string str3DSecureAuthResult, string strTranId)
		{
			// 送信用XML作成
			XmlDocument xdSendData = Create3DSecureAuthResultXml(str3DSecureAuthResult, strTranId);

			// POST送信
			XmlDocument xdPostDataResult = SendXml(xdSendData);

			// レスポンス解析
			string strPostDataResult = xdPostDataResult.SelectSingleNode("./response/result/status").InnerText;

			if (strPostDataResult == "success")
			{
				// 成功
				WriteLog(
					PaymentFileLogger.PaymentProcessingType.CreditAuth,
					true,
					new KeyValuePair<string, string>("orderId", orderId));
				return true;
			}
			else
			{
				// 失敗
				// ログ出力
				this.ErrorMessage = "本人認証結果チェック:" + GetSecureApiErrorMessage(
					strPostDataResult,
					xdPostDataResult.SelectSingleNode("./response/result/code").InnerText);
				AppLogger.WriteError(this.ErrorMessage);
				WriteLog(
					PaymentFileLogger.PaymentProcessingType.Cancel,
					false,
					new KeyValuePair<string, string>("orderId", orderId),
					new KeyValuePair<string, string>("ErrorMessage", this.ErrorMessage));
				return false;
			}
		}

		/// <summary>
		/// オーソリ処理（3Dセキュア対象外のときもここに来る）
		/// </summary>
		/// <param name="strTranId">ゼウストランザクションID</param>
		/// <returns>True:オーソリ成功 False:オーソリ失敗</returns>
		public bool Auth(string strTranId)
		{
			// 送信用XML作成
			XmlDocument xdSendData = CreatePaymentRequestXml(strTranId);

			// POST送信
			XmlDocument xdPostDataResult = SendXml(xdSendData);

			// レスポンス解析
			string strPostDataResult = xdPostDataResult.SelectSingleNode("./response/result/status").InnerText;

			if (strPostDataResult == "success")
			{
				// 成功
				// ZeusオーダーIDをセット
				this.ZeusOrderId = xdPostDataResult.SelectSingleNode("./response/order_number").InnerText;
				WriteLog(
					PaymentFileLogger.PaymentProcessingType.OthoriProcessing,
					true,
					new KeyValuePair<string, string>("strTranId", strTranId),
					new KeyValuePair<string, string>("ZeusOrderId", this.ZeusOrderId));
				return true;
			}
			else
			{
				// 失敗
				// ログ出力
				this.ErrorMessage = "オーソリ処理:" + GetSecureApiErrorMessage(strPostDataResult, xdPostDataResult.SelectSingleNode("./response/result/code").InnerText);
				WriteLog(
					PaymentFileLogger.PaymentProcessingType.OthoriProcessing,
					false,
					new KeyValuePair<string, string>("strTranId", strTranId),
					new KeyValuePair<string, string>("ErrorMessage", this.ErrorMessage));
				return false;
			}
		}

		/// <summary>
		/// 注文情報送信用XML作成
		/// </summary>
		/// <param name="coCart">カート情報</param>
		/// <returns>注文情報送信用XML</returns>
		private XmlDocument CreateOrderInfoXml(CartObject coCart)
		{
			XElement xeOrderInfo;

			// クレジット決済方法設定を取得
			Constants.PaymentCreditCardPaymentMethod? paymentMethod = (Constants.DIGITAL_CONTENTS_OPTION_ENABLED && coCart.HasDigitalContents) ? Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD_FORDIGITALCONTENTS : Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD;

			var use3Ds2Flag = (Constants.PAYMENT_SETTING_ZEUS_3DSECURE2
				? ZeusFlag.On
				: ZeusFlag.Off).ToText();

			// 新規カード？
			if (coCart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
			{
				xeOrderInfo = new XElement(
					new XElement("request",
						new XElement("service", "secure_link_3d"),
						new XElement("action", "enroll"),
						new XElement("authentication",
							new XElement("clientip", this.ClientIP),
							new XElement("key", this.SecureApiAuthKey)),
						new XElement("token_key", coCart.Payment.CreditToken.Token),
						new XElement("payment",
							new XElement("amount", paymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH ? "0" : coCart.SendingAmount.ToPriceString()),
							new XElement("count", coCart.Payment.CreditInstallmentsCode)),
						new XElement("user",
							new XElement("telno",
								coCart.Owner.Tel1_1 + coCart.Owner.Tel1_2 + coCart.Owner.Tel1_3,
								new XElement("validation", "strict")),
							new XElement("email",
								coCart.Owner.MailAddr,
								new XElement("language", "japanese"))),
						new XElement("uniq_key",
							new XElement("sendid", coCart.Payment.UserCreditCard.CooperationInfo.ZeusSendId)),
						new XElement("use_3ds2_flag", use3Ds2Flag)));
			}
			// 登録済みカード
			else
			{
				xeOrderInfo = new XElement(
					new XElement("request",
						new XElement("service", "secure_link_3d"),
						new XElement("action", "enroll"),
						new XElement("authentication",
							new XElement("clientip", this.ClientIP),
							new XElement("key", this.SecureApiAuthKey)),
						new XElement("card",
							new XElement("history",
								new XElement("action", "send_email"),
								new XElement("key", "sendid"),
								new XElement("key", "telno")),
							((OrderCommon.CreditSecurityCodeEnable) ? new XElement("cvv", coCart.Payment.CreditSecurityCode) : null),
							new XElement("name", coCart.Payment.CreditAuthorName)),
					//new XElement("token_key", coCart.Payment.CreditToken.Token),	// セキュリティカードを利用しない場合は省略可能
						new XElement("payment",
							new XElement("amount", (paymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH) ? "0" : coCart.SendingAmount.ToPriceString()),
							new XElement("count", coCart.Payment.CreditInstallmentsCode)),
						new XElement("user",
							new XElement("telno",
								coCart.Payment.UserCreditCard.CooperationInfo.ZeusTelNo,
								new XElement("validation", "strict")),
							new XElement("email",
								coCart.Owner.MailAddr,
								new XElement("language", "japanese"))),
						new XElement("uniq_key",
							new XElement("sendid", coCart.Payment.UserCreditCard.CooperationInfo.ZeusSendId)),
						new XElement("use_3ds2_flag", use3Ds2Flag)));
			}

			// 3Dセキュア2.0対応のため、リスクベース認証用パラメータ追加
			if (Constants.PAYMENT_SETTING_ZEUS_3DSECURE2 && (this.IsTestDevelopment == false))
			{
				xeOrderInfo.Add(CreateCardholderInfoXml(coCart));
				xeOrderInfo.Add(CreateAcctInfoXml(coCart));
				xeOrderInfo.Add(CreateMerchantRiskIndicatorXml(coCart));
			}

			return XmlExtensions.ToXmlDocument(new XDocument(xeOrderInfo));
		}
		/// <summary>
		/// カード持ち主情報XML作成（3Dセキュア2.0対応）
		/// </summary>
		/// <param name="coCart">カート情報</param>
		/// <returns>カード持ち主情報XML</returns>
		private XElement CreateCardholderInfoXml(CartObject coCart)
		{
			// 0A0A始まりのものは携帯電話番号とする
			const string MOBILE_PHONE_TEL_PATERN = "0[1-9]0[1-9][0-9]{6,11}";

			// 0AA始まりのものは固定電話番号とする
			const string HOME_PHONE_TEL_PATERN = "0[1-9][1-9][0-9]{7,12}";

			var cartShipping = coCart.GetShipping();
			var countryIsoCode = (coCart.Owner.IsAddrJp
				? CountryIsoCode.Japan
				: coCart.Owner.IsAddrTw
					? CountryIsoCode.Taiwan
					: coCart.Owner.IsAddrUs
						? CountryIsoCode.America
						: CountryIsoCode.Other).ToText();

			var ownerTel1 = coCart.Owner.Tel1.Replace("-", string.Empty);
			var ownerTel2 = coCart.Owner.Tel2.Replace("-", string.Empty);

			var mobiPhoneTel = Regex.IsMatch(ownerTel1, MOBILE_PHONE_TEL_PATERN)
				? Regex.Match(ownerTel1, MOBILE_PHONE_TEL_PATERN)
				: Regex.Match(ownerTel2, MOBILE_PHONE_TEL_PATERN);
			var homePhoneTel = Regex.IsMatch(ownerTel1, HOME_PHONE_TEL_PATERN)
				? Regex.Match(ownerTel1, HOME_PHONE_TEL_PATERN)
				: Regex.Match(ownerTel2, HOME_PHONE_TEL_PATERN);

			var cardHolderInfo = new XElement(
				"cardHolderInfo",
				new XElement(
					"addrMatch",
					(coCart.IsShippingAddressOwner())
						? "Y"
						: "N"),
				new XElement("billAddrCity", coCart.Owner.Addr2),
				new XElement("billAddrCountry", countryIsoCode),
				new XElement("billAddrLine1", coCart.Owner.Addr3),
				new XElement("billAddrLine2", coCart.Owner.Addr4),
				new XElement("billAddrLine3", string.Empty),
				new XElement("billAddrPostCode", coCart.Owner.Zip),
				new XElement("billAddrState", Regex.Match(PrefectureMapping.GetSubdivisionCode(Constants.FIELD_ZIPCODE_PREFECTURE, coCart.Owner.Addr1), @"\d+").Value),
				new XElement("cardholderName", coCart.Payment.CreditAuthorName),
				new XElement("email", coCart.Owner.MailAddr),
				string.IsNullOrEmpty(homePhoneTel.Value)
					? null
					: new XElement(
						"homePhone",
						new XElement("cc", CountryIsoCode.Japan.ToText()),
						new XElement("subscriber", homePhoneTel.Value)),
				string.IsNullOrEmpty(mobiPhoneTel.Value)
					? null
					: new XElement(
						"mobilePhone",
						new XElement("cc", CountryIsoCode.Japan.ToText()),
						new XElement("subscriber", mobiPhoneTel.Value)),
				new XElement("shipAddrCity", cartShipping.Addr2),
				new XElement("shipAddrCountry", countryIsoCode),
				new XElement("shipAddrline1", cartShipping.Addr3),
				new XElement("shipAddrLine2", cartShipping.Addr4),
				new XElement("shipAddrLine3", string.Empty),
				new XElement("shipAddrPostCode", cartShipping.Zip),
				new XElement("shipAddrState", Regex.Match(PrefectureMapping.GetSubdivisionCode(Constants.FIELD_ZIPCODE_PREFECTURE, cartShipping.Addr1), @"\d+").Value));
			return cardHolderInfo;
		}

		/// <summary>
		/// 会員情報XML作成（3Dセキュア2.0対応）
		/// </summary>
		/// <param name="coCart">カート情報</param>
		/// <returns>会員情報XML</returns>
		private XElement CreateAcctInfoXml(CartObject coCart)
		{
			const string FORMAT_DATE = "yyyyMMdd";

			var user = new UserService().Get(coCart.CartUserId);
			if (user == null) return null;

			var bankAccountCreatedDate = DateTime.Now;
			var userCreatedDays = (DateTime.Now - user.DateCreated).Days;
			string userAccountCreatedPeriod, bankAccountCreatedPeriod;

			var orders = new OrderService().GetOrderInfosByUserId(user.UserId).ToArray();

			// 加盟店サイトのアカウントを保有している期間、決済口座が登録されてからの経過期間を判定し、値をセットする
			if (coCart.IsGuestUser)
			{
				userAccountCreatedPeriod = bankAccountCreatedPeriod = PeriodType.NotUser.ToText();
			}
			else
			{
				if (coCart.Payment.CreditCardBranchNo != CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
				{
					bankAccountCreatedDate = coCart.Payment.UserCreditCard.DateCreated;
					var bankAccountCreatedDays = (DateTime.Now - bankAccountCreatedDate).Days;

					userAccountCreatedPeriod = GetInd(userCreatedDays);
					bankAccountCreatedPeriod = GetInd(bankAccountCreatedDays);
				}
				else
				{
					userAccountCreatedPeriod = bankAccountCreatedPeriod = PeriodType.Now.ToText();
				}
			}

			var cardHolderInfo = new XElement(
				"acctInfo",
				new XElement("chAccAgeInd", userAccountCreatedPeriod),
				new XElement("chAccChange", string.Empty),
				new XElement("chAccChangeInd", string.Empty),
				new XElement("chAccDate", user.DateCreated.ToString(FORMAT_DATE)),
				new XElement("chAccPwChange", string.Empty),
				new XElement("chAccPwChangeInd", string.Empty),
				new XElement("nbPurchaseAccount", orders.Count(order => (order.DateCreated >= DateTime.Now.AddMonths(-6)))),
				new XElement("paymentAccAge", bankAccountCreatedDate.ToString(FORMAT_DATE)),
				new XElement("paymentAccInd", bankAccountCreatedPeriod),
				new XElement("provisionAttemptsDay", string.Empty),
				new XElement("shipAddressUsage", string.Empty),
				new XElement("shipAddressUsageInd", string.Empty),
				new XElement(
					"shipNameIndicator",
					(coCart.GetShipping().Name == user.Name)
						? "01"
						: "02"),
				new XElement("suspiciousAccActivity", string.Empty),
				new XElement("txnActivityDay", orders.Count(order => (order.DateCreated >= DateTime.Now.AddHours(-24)))),
				new XElement("txnActivityYear", orders.Count(order => (order.DateCreated.Year == (DateTime.Now.Year - 1)))));
			return cardHolderInfo;

			// 保有している期間を判定する
			string GetInd(int days) =>
				((days <= 30)
					? PeriodType.OneMonthLess
					: (days <= 60)
						? PeriodType.TwoMonthsLess
						: PeriodType.TwoMonthsMore).ToText();
		}

		/// <summary>
		/// 加盟店情報XML作成（3Dセキュア2.0対応）
		/// </summary>
		/// <param name="coCart">カート情報</param>
		/// <returns>加盟店情報XML</returns>
		private XElement CreateMerchantRiskIndicatorXml(CartObject coCart)
		{
			var shippingList = new UserShippingService().GetAllOrderByShippingNoDesc(coCart.CartUserId);

			// 注文の請求先住所はアドレス帳に存在するか
			var isExistsUserShippingAddress = shippingList
				.Any(
					ship =>
						coCart.GetShipping().ConcatenateAddressWithoutCountryName() == (AddressHelper.ConcatenateAddressWithoutCountryName(
							ship.ShippingAddr1,
							ship.ShippingAddr2,
							ship.ShippingAddr3,
							ship.ShippingAddr4)));

			var shipIndicator = coCart.IsDigitalContentsOnly
				? ShippingIndicatorType.DigitalProduct.ToText()
				: (coCart.IsGuestUser || coCart.IsShippingAddressOwner())
					? ShippingIndicatorType.UserAddress.ToText()
					: isExistsUserShippingAddress
						? ShippingIndicatorType.InAddressBook.ToText()
						: ShippingIndicatorType.NotInAddressBook.ToText();

			var merchantRiskIndicator = new XElement(
				"merchantRiskIndicator",
				new XElement("deliveryEmailAddress", coCart.IsDigitalContentsOnly ? coCart.Owner.MailAddr : string.Empty),
				new XElement("deliveryTimeframe", coCart.IsDigitalContentsOnly ? "01" : string.Empty), // 01:電子デリバリー
				new XElement("giftCardAmount", string.Empty),
				new XElement("giftCardCount", string.Empty),
				new XElement("giftCardCurr", string.Empty),
				new XElement("preOrderDate", string.Empty),
				new XElement("preOrderPurchaseInd", "01"), // 01:販売されている商品
				new XElement("reorderItemsInd", string.Empty),
				new XElement("shipIndicator", shipIndicator));
			return merchantRiskIndicator;
		}

		/// <summary>
		/// 本人認証結果送信用XML作成
		/// </summary>
		/// <param name="str3DSecureAuthResult">本人認証結果</param>
		/// <param name="strTranId">ゼウストランザクションID</param>
		/// <returns>本人認証結果送信用XML</returns>
		private XmlDocument Create3DSecureAuthResultXml(string str3DSecureAuthResult, string strTranId)
		{
			XDocument xd = new XDocument(
				new XElement("request",
					new XElement("service", "secure_link_3d"),
					new XElement("action", "authentication"),
					new XElement("xid", strTranId),
					new XElement("PaRes", str3DSecureAuthResult)));
			return XmlExtensions.ToXmlDocument(xd);
		}

		/// <summary>
		/// オーソリ指示送信用XML作成
		/// </summary>
		/// <param name="strTranId">ゼウストランザクションID</param>
		/// <returns>オーソリ指示送信用XML</returns>
		private XmlDocument CreatePaymentRequestXml(string strTranId)
		{
			XDocument xd = new XDocument(
				new XElement("request",
					new XElement("service", "secure_link_3d"),
					new XElement("action", "payment"),
					new XElement("xid", strTranId)));
			return XmlExtensions.ToXmlDocument(xd);
		}

		/// <summary>
		/// POST送信
		/// </summary>
		/// <param name="xdSendData">送信データ</param>
		/// <returns>レスポンス</returns>
		public XmlDocument SendXml(XmlDocument xdSendData)
		{
			byte[] btPostDatas = Encoding.ASCII.GetBytes(xdSendData.InnerXml);

			// リクエストの作成
			WebRequest wrWebRequest = WebRequest.Create(this.ServerUrl);
			wrWebRequest.Method = "POST";
			wrWebRequest.ContentType = "text/xml";
			wrWebRequest.ContentLength = btPostDatas.Length;

			// POSTデータの書込み
			using (Stream sPostStream = wrWebRequest.GetRequestStream())
			{
				sPostStream.Write(btPostDatas, 0, btPostDatas.Length);
			}

			// レスポンスの取得と読込み
			var xdPostDataResult = new XmlDocument();
			using (var wrWebResponse = wrWebRequest.GetResponse())
			using (var sRes = wrWebResponse.GetResponseStream())
			using (var streamReader = new StreamReader(sRes))
			{
				var responseString = streamReader.ReadToEnd();
				try
				{
					xdPostDataResult.LoadXml(responseString);
				}
				catch (Exception ex)
				{
					throw new Exception(
						"XMLの変換に失敗しました：\r\n"
							+ this.ServerUrl + "\r\n"
							+ "-----------\r\n"
							+ xdSendData.OuterXml + "\r\n"
							+ "-----------\r\n"
							+ responseString
							+ "-----------\r\n",
						ex);
				}
			}
			return xdPostDataResult;
		}

		/// <summary>
		/// SecureAPIエラーメッセージ取得
		/// </summary>
		/// <param name="strStatus">ステータス</param>
		/// <param name="strCode">エラーコード</param>
		/// <returns>SecureAPIエラーメッセージ</returns>
		private string GetSecureApiErrorMessage(string strStatus, string strCode)
		{
			// エラーメッセージXML読み込み
			XmlDocument xdMessages = new XmlDocument();
			string strMessage = null;
			xdMessages.LoadXml(Properties.Resources.ZeusSecureApiMessages);

			// 該当エラーメッセージ取得
			XmlNode xnTarget = xdMessages.SelectSingleNode("ZeusSecureApiMessages/E" + strCode);
			if (xnTarget != null)
			{
				strMessage = xnTarget.Attributes["message"].Value;
			}

			return strStatus + " / " + strCode + " " + strMessage;
		}

		/// <summary>SecureAPI認証キー</summary>
		private string SecureApiAuthKey { get; set; }
		/// <summary>ZeusオーダーID</summary>
		public string ZeusOrderId { get; private set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; private set; }
		/// <summary>エラー種別コード(末尾から3桁)</summary>
		public string ErrorTypeCode { get; private set; }
		/// <summary>テスト環境ならTRUE</summary>
		public bool IsTestDevelopment
		{
			get
			{
				return this.ServerUrl.Contains("secure2-sandbox.cardservice.co.jp");
			}
		}
	}
}
