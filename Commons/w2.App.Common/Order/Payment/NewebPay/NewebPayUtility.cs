/*
=========================================================================================================
  Module      : Neweb Pay Utility(NewebPayUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using w2.App.Common.Global.Region.Currency;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.Order;
using w2.Domain.Order.Helper;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserCreditCard;

namespace w2.App.Common.Order.Payment.NewebPay
{
	/// <summary>
	/// NewebPay Utility Class
	/// </summary>
	public class NewebPayUtility
	{
		/// <summary>Credit Card Once Time Inst Flag</summary>
		private const string CREDIT_CARD_ONCE_TIME_INST_FLAG = "0";

		/// <summary>
		/// Create Regist Request
		/// </summary>
		/// <param name="cart">Cart Object</param>
		/// <returns>NewebPay Request</returns>
		public static NewebPayRequest CreateRegistRequest(CartObject cart)
		{
			var registRequestTemp = CreateTradeInfoRegistRequest(cart);
			var parameter = CreateTradeInfoParametersRegist(registRequestTemp);
			var tradeInfo = EncryptAES256(parameter);

			var hashKey = CreateHashKeyAndHashIV(
				NewebPayConstants.CONST_REQUEST_KEY_HASH_KEY,
				Constants.NEWEBPAY_PAYMENT_HASHKEY);
			var hashIV = CreateHashKeyAndHashIV(
				NewebPayConstants.CONST_REQUEST_KEY_HASH_IV,
				Constants.NEWEBPAY_PAYMENT_HASHIV);

			var url = string.Format(
				"{0}&{1}&{2}",
				hashKey,
				tradeInfo,
				hashIV);
			var tradeSha = GetHashSha256(url);

			var registRequest = new NewebPayRequest
			{
				MerchantId = Constants.NEWEBPAY_PAYMENT_MERCHANTID,
				TradeInfo = tradeInfo,
				TradeSha = tradeSha,
				Version = NewebPayConstants.CONST_VERSION_API
			};
			return registRequest;
		}

		/// <summary>
		/// Create Trade Info Regist Request
		/// </summary>
		/// <param name="cart">Cart</param>
		/// <returns>NewebPay Request</returns>
		private static NewebPayParameterRequest CreateTradeInfoRegistRequest(CartObject cart)
		{
			var externalPaymentType = cart.Payment.ExternalPaymentType;
			var baseUrl = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				string.IsNullOrEmpty(Constants.WEBHOOK_DOMAIN)
					? Constants.SITE_DOMAIN
					: Constants.WEBHOOK_DOMAIN,
				Constants.PATH_ROOT_FRONT_PC);

			var paramForReturn = string.Join(
				",",
				NewebPayConstants.CONST_RETURN_URL,
				cart.OrderId,
				cart.CartId,
				cart.IsLandingUseNewebPay);
			var returnURL = new UrlCreator(baseUrl + Constants.PAGE_FRONT_PAYMENT_NEWEBPAY_ORDER_RESULT)
				.AddParam(Constants.REQUEST_KEY_NO, paramForReturn)
				.CreateUrl();

			var paramForClientBack = string.Join(
				",",
				NewebPayConstants.CONST_CLIENT_BACK_URL,
				cart.OrderId,
				cart.CartId,
				cart.IsLandingUseNewebPay);
			var clientBackUrl = new UrlCreator(baseUrl + Constants.PAGE_FRONT_PAYMENT_NEWEBPAY_ORDER_RESULT)
				.AddParam(Constants.REQUEST_KEY_NO, paramForClientBack)
				.CreateUrl();

			var amount = CurrencyManager.GetSettlementAmount(
				cart.PriceTotal,
				cart.SettlementRate,
				cart.SettlementCurrency);

			var registRequest = new NewebPayParameterRequest
			{
				MerchantId = Constants.NEWEBPAY_PAYMENT_MERCHANTID,
				RespondType = NewebPayConstants.CONST_JSON_RESPONDTYPE,
				TimeStamp = StringUtility.ToEmpty(GetTimeStamp()),
				Version = NewebPayConstants.CONST_VERSION_API,
				MerchantOrderNo = cart.OrderId,
				Amt = (int)amount,
				ItemDesc = Constants.NEWEBPAY_PAYMENT_ITEMNAME,
				ReturnURL = returnURL,
				NotifyURL = CreateUrlApiForNotifyOrCustomer(baseUrl, cart.CartId, true),
				CustomerURL = CreateUrlApiForNotifyOrCustomer(baseUrl, cart.CartId),
				ClientBackURL = clientBackUrl,
				Email = cart.Owner.MailAddr,
				LoginType = Constants.NEWEBPAY_PAYMENT_LOGINTYPE,
				Credit = CreateValidPaymentTypeFlg(
					externalPaymentType,
					Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT),
				LinePay = CreateValidPaymentTypeFlg(
					externalPaymentType,
					Constants.FLG_PAYMENT_TYPE_NEWEBPAY_LINEPAY),
				WebATM = CreateValidPaymentTypeFlg(
					externalPaymentType,
					Constants.FLG_PAYMENT_TYPE_NEWEBPAY_WEBATM),
				Vacc = CreateValidPaymentTypeFlg(
					externalPaymentType,
					Constants.FLG_PAYMENT_TYPE_NEWEBPAY_ATM),
				CVS = CreateValidPaymentTypeFlg(
					externalPaymentType,
					Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CVS),
				Barcode = CreateValidPaymentTypeFlg(
					externalPaymentType,
					Constants.FLG_PAYMENT_TYPE_NEWEBPAY_BARCODE),
				CVSCOM = CreateValidPaymentTypeFlg(
					externalPaymentType,
					Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CVSCOM)
			};

			if (registRequest.IsCredit)
			{
				if ((string.IsNullOrEmpty(cart.Payment.NewebPayCreditInstallmentsCode) == false)
					&& (cart.Payment.NewebPayCreditInstallmentsCode != NewebPayConstants.FLG_CREDIT_CARD_ONCE_TIME))
				{
					registRequest.InstFlag = cart.Payment.NewebPayCreditInstallmentsCode;
					registRequest.Credit = 0;
				}
				else
				{
					registRequest.InstFlag = CREDIT_CARD_ONCE_TIME_INST_FLAG;
					registRequest.Credit = registRequest.Credit;
				}
				registRequest.TokenTerm = CreateTokenTerm(cart.OrderUserId);
			}
			return registRequest;
		}

		/// <summary>
		/// Create Hash Key And Hash IV
		/// </summary>
		/// <param name="key">Key</param>
		/// <param name="value">Value</param>
		/// <returns>String Of Hash Key Or Hash IV</returns>
		public static string CreateHashKeyAndHashIV(string key, string value)
		{
			var result = string.Format("{0}={1}", key, value);
			return result;
		}

		/// <summary>
		/// Create Valid Payment Type Flg
		/// </summary>
		/// <param name="externalPaymentType">External Payment Type</param>
		/// <param name="externalPaymentTypeFlg">External Payment Type</param>
		/// <returns>If valid: 1, Otherwise: 0</returns>
		private static int CreateValidPaymentTypeFlg(
			string externalPaymentType,
			string externalPaymentTypeFlg)
		{
			var valid = (externalPaymentType == externalPaymentTypeFlg)
				? ((externalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CVSCOM)
					? NewebPayConstants.FLG_NEWEBPAY_PAYMENT_CVSCOM_TYPE_ON
					: NewebPayConstants.FLG_NEWEBPAY_PAYMENT_TYPE_ON)
				: NewebPayConstants.FLG_NEWEBPAY_PAYMENT_TYPE_OFF;
			return valid;
		}

		/// <summary>
		/// Create Token Term
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <returns>Token Term</returns>
		private static string CreateTokenTerm(string userId)
		{
			var userCreditCard = new UserCreditCardService();
			var branchNo = userCreditCard.GetMaxBranchNoByUserIdAndCooperationType(
				userId,
				Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_NEWEBPAY);
			var userCrediCardInfo = userCreditCard.Get(userId, branchNo);
			return (userCrediCardInfo != null)
				? userCrediCardInfo.CooperationId
				: userId;
		}

		/// <summary>
		/// Create Url Api For Notify Or Customer
		/// </summary>
		/// <param name="host">Host</param>
		/// <param name="cartId">Cart Id</param>
		/// <param name="isNotify">Is Notify</param>
		/// <returns>Url</returns>
		private static string CreateUrlApiForNotifyOrCustomer(
			string host,
			string cartId,
			bool isNotify = false)
		{
			var paramUrl = isNotify
				? NewebPayConstants.CONST_NOTIFY_URL
				: NewebPayConstants.CONST_CUSTOMER_URL;
			var param = string.Join(",", paramUrl, cartId);
			var urlCreator = new UrlCreator(host + Constants.PAGE_FRONT_PAYMENT_NEWEBPAY_RECEIVE)
				.AddParam(Constants.REQUEST_KEY_NO, param)
				.CreateUrl();
			return urlCreator;
		}

		/// <summary>
		/// Create Cancel Refund Capture Request
		/// </summary>
		/// <param name="order">Order Model</param>
		/// <param name="isCancel">Is Cancel</param>
		/// <param name="isRefund">Is Refund</param>
		/// <param name="refundAmount">Refund Amount</param>
		/// <returns>NewebPay Request</returns>
		public static NewebPayRequest CreateCancelRefundCaptureRequest(
			OrderModel order,
			bool isCancel = true,
			bool isRefund = false,
			decimal refundAmount = 0)
		{
			var cancelRefundCaptureRequestTemp = CreateTradeInfoCancelRefundCaptureRequest(order, isCancel, isRefund, refundAmount);
			var parameter = CreateTradeInfoParametersCancelRefundCapture(cancelRefundCaptureRequestTemp);
			var postData = EncryptAES256(parameter);
			var request = new NewebPayRequest
			{
				MerchantIdCancelAndSaleRefund = Constants.NEWEBPAY_PAYMENT_MERCHANTID,
				PostData = postData
			};
			return request;
		}

		/// <summary>
		/// Create Trade Info Cancel Refund Capture Request
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="isCancel">Is Cancel</param>
		/// <param name="isRefund">Is Refund</param>
		/// <param name="refundAmount">Refund Amount</param>
		/// <returns>NewebPay Request</returns>
		private static NewebPayParameterRequest CreateTradeInfoCancelRefundCaptureRequest(
			OrderModel order,
			bool isCancel,
			bool isRefund,
			decimal refundAmount)
		{
			var cancelRefundCaptureRequest = new NewebPayParameterRequest
			{
				RespondType = NewebPayConstants.CONST_STRING_RESPONDTYPE,
				Version = NewebPayConstants.CONST_VERSION_API,
				Amt = (int)order.SettlementAmount,
				TradeNo = order.CardTranId,
				IndexType = NewebPayConstants.CONST_INDEX_TYPE,
				TimeStamp = StringUtility.ToEmpty(GetTimeStamp()),
			};

			if (isCancel == false)
			{
				cancelRefundCaptureRequest.MerchantOrderNo = (isRefund)
					? order.OrderIdOrg
					: order.OrderId;
				cancelRefundCaptureRequest.CloseType = (isRefund)
					? NewebPayConstants.CONST_CLOSE_TYPE_REFUND
					: NewebPayConstants.CONST_CLOSE_TYPE_SALE;
			}

			if (isRefund) cancelRefundCaptureRequest.Amt = (int)refundAmount;
			return cancelRefundCaptureRequest;
		}

		/// <summary>
		/// Create Trade Info Parameters Regist
		/// </summary>
		/// <param name="request">NewebPay Request</param>
		/// <returns>Parameters</returns>
		private static string CreateTradeInfoParametersRegist(NewebPayParameterRequest request)
		{
			var parameters = new Dictionary<string, string>
			{
				{ NewebPayConstants.PARAM_MERCHANT_ID, request.MerchantId },
				{ NewebPayConstants.PARAM_RESPOND_TYPE, request.RespondType },
				{ NewebPayConstants.PARAM_TIME_STAMP, request.TimeStamp },
				{ NewebPayConstants.PARAM_VERSION, request.Version },
				{ NewebPayConstants.PARAM_MERCHANT_ORDER_NO, request.MerchantOrderNo },
				{ NewebPayConstants.PARAM_AMOUNT, StringUtility.ToEmpty(request.Amt) },
				{ NewebPayConstants.PARAM_ITEM_DESC, request.ItemDesc },
				{ NewebPayConstants.PARAM_RETURN_URL, request.ReturnURL },
				{ NewebPayConstants.PARAM_NOTIFY_URL, request.NotifyURL },
				{ NewebPayConstants.PARAM_CUSTOMER_URL, request.CustomerURL },
				{ NewebPayConstants.PARAM_CLIENT_BACK_URL, request.ClientBackURL },
				{ NewebPayConstants.PARAM_EMAIL, request.Email },
				{ NewebPayConstants.PARAM_LOGIN_TYPE, StringUtility.ToEmpty(request.LoginType) },
				{ NewebPayConstants.PARAM_CREDIT, StringUtility.ToEmpty(request.Credit) },
				{ NewebPayConstants.PARAM_LINEPAY, StringUtility.ToEmpty(request.LinePay) },
				{ NewebPayConstants.PARAM_WEBATM, StringUtility.ToEmpty(request.WebATM) },
				{ NewebPayConstants.PARAM_ATM, StringUtility.ToEmpty(request.Vacc) },
				{ NewebPayConstants.PARAM_CVS, StringUtility.ToEmpty(request.CVS) },
				{ NewebPayConstants.PARAM_BARCODE, StringUtility.ToEmpty(request.Barcode) },
				{ NewebPayConstants.PARAM_CVSCOM, StringUtility.ToEmpty(request.CVSCOM) },
			};

			if (request.Credit == NewebPayConstants.FLG_NEWEBPAY_PAYMENT_TYPE_ON)
			{
				parameters.Add(NewebPayConstants.PARAM_TOKEN_TERM, request.TokenTerm);
			}
			if (request.InstFlag != CREDIT_CARD_ONCE_TIME_INST_FLAG)
			{
				parameters.Add(NewebPayConstants.PARAM_INST_FLAG, request.InstFlag);
			}

			// Remove item null and sort
			var result = CreateParameters(parameters.ToArray());
			return result;
		}

		/// <summary>
		/// Create Trade Info Parameters Cancel Refund Capture
		/// </summary>
		/// <param name="request">NewebPay Request</param>
		/// <returns>Parameters</returns>
		private static string CreateTradeInfoParametersCancelRefundCapture(NewebPayParameterRequest request)
		{
			var parameters = new Dictionary<string, string>
			{
				{ NewebPayConstants.PARAM_RESPOND_TYPE, request.RespondType },
				{ NewebPayConstants.PARAM_VERSION, request.Version },
				{ NewebPayConstants.PARAM_AMOUNT, StringUtility.ToEmpty(request.Amt) },
				{ NewebPayConstants.PARAM_TRADE_NO, request.TradeNo },
				{ NewebPayConstants.PARAM_INDEX_TYPE, request.IndexType },
				{ NewebPayConstants.PARAM_TIME_STAMP, StringUtility.ToEmpty(request.TimeStamp) },
				{ NewebPayConstants.PARAM_MERCHANT_ORDER_NO, request.MerchantOrderNo },
				{ NewebPayConstants.PARAM_CLOSE_TYPE, StringUtility.ToEmpty(request.CloseType) },
			};

			// Remove item null and sort
			var result = CreateParameters(parameters.ToArray());
			return result;
		}

		/// <summary>
		/// Create Parameters
		/// </summary>
		/// <param name="parameters">The Parameters</param>
		/// <returns>A Parameters As String</returns>
		private static string CreateParameters(KeyValuePair<string, string>[] parameters)
		{
			var result = string.Join(
				"&",
				parameters.Select(item =>
					string.Format(
						"{0}={1}",
						item.Key,
						item.Value)));
			return result;
		}

		/// <summary>
		/// Create Payment Memo
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="memo">Memo</param>
		/// <param name="totalAmount">Total Amount</param>
		/// <param name="isAlreadyUpdated">Check If Is Already Updated</param>
		/// <returns>Payment Memo</returns>
		public static string CreatePaymentMemo(OrderModel order, string memo, decimal totalAmount, bool isAlreadyUpdated = false)
		{
			var tempMemo = OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
				order.OrderId,
				order.OrderPaymentKbn,
				order.CardTranId,
				isAlreadyUpdated
					? "与信"
					: memo,
				totalAmount);
			var finalMemo = OrderExternalPaymentUtility.SetExternalPaymentMemo(
				StringUtility.ToEmpty(order.PaymentMemo),
				tempMemo);
			return finalMemo;
		}

		/// <summary>
		/// Create Barcode Url
		/// </summary>
		/// <param name="barcode1">Barcode 1</param>
		/// <param name="barcode2">Barcode 2</param>
		/// <param name="barcode3">Barcode 3</param>
		/// <returns>Barcode Url</returns>
		public static string CreateBarcodeUrl(string barcode1, string barcode2, string barcode3)
		{
			var baseUrl = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				Constants.PATH_ROOT_FRONT_PC);
			var barcodeUrl = new UrlCreator(baseUrl + Constants.PAGE_FRONT_PAYMENT_NEWEBPAY_BARCODE)
				.AddParam("code1", barcode1)
				.AddParam("code2", barcode2)
				.AddParam("code3", barcode3)
				.CreateUrl();
			return barcodeUrl;
		}

		/// <summary>
		/// Export Log
		/// </summary>
		/// <param name="message">Message</param>
		/// <param name="orderId">Order Id</param>
		/// <param name="paymentOrderId">Payment Order Id</param>
		/// <param name="result">Result</param>
		/// <param name="paymentId">Payment Id</param>
		public static void ExportLog(
			string message,
			string orderId,
			string paymentOrderId,
			bool result,
			string paymentId)
		{
			var idDictionary = new Dictionary<string, string>
			{
				{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.ToEmpty(orderId) },
				{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, StringUtility.ToEmpty(paymentOrderId) },
			};
			PaymentFileLogger.WritePaymentLog(
				result,
				paymentId,
				PaymentFileLogger.PaymentType.NewebPay,
				PaymentFileLogger.PaymentProcessingType.ExecPayment,
				message,
				idDictionary);
		}

		/// <summary>
		/// Encrypt AES256
		/// </summary>
		/// <param name="source">source</param>
		/// <returns>Hash String</returns>
		public static string EncryptAES256(string source)
		{
			var sourceBytes = AddPKCS7Padding(Encoding.UTF8.GetBytes(source), 32);
			var aes256 = new RijndaelManaged()
			{
				Key = Encoding.UTF8.GetBytes(Constants.NEWEBPAY_PAYMENT_HASHKEY),
				IV = Encoding.UTF8.GetBytes(Constants.NEWEBPAY_PAYMENT_HASHIV),
				Mode = CipherMode.CBC,
				Padding = PaddingMode.None,
			};
			var transform = aes256.CreateEncryptor();
			var hashString = ByteArrayToHex(transform
				.TransformFinalBlock(
					sourceBytes,
					0,
					sourceBytes.Length))
				.ToLower();
			return hashString;
		}

		/// <summary>
		/// SHA256暗号化
		/// </summary>
		/// <param name="text">Text</param>
		/// <returns>String With Endoce Sha256</returns>
		public static string GetHashSha256(string text)
		{
			var bytes = Encoding.UTF8.GetBytes(text);
			var hash = new SHA256Managed().ComputeHash(bytes);
			var hashStringUpper = string.Empty;

			foreach (var value in hash)
			{
				hashStringUpper += string.Format("{0:x2}", value);
			}
			return hashStringUpper.ToUpper();
		}

		/// <summary>
		/// Add PKCS7 Padding
		/// </summary>
		/// <param name="data">Data</param>
		/// <param name="blockSize">Block Size</param>
		/// <returns>List Byte</returns>
		private static byte[] AddPKCS7Padding(byte[] data, int blockSize)
		{
			var length = data.Length;
			var padding = (byte)(blockSize - (length % blockSize));
			var output = new byte[length + padding];
			Buffer.BlockCopy(data, 0, output, 0, length);

			for (var index = length; index < output.Length; index++)
			{
				output[index] = (byte)padding;
			}
			return output;
		}

		/// <summary>
		/// Byte Array To Hex String
		/// </summary>
		/// <param name="byteArray">Byte Array</param>
		/// <returns>String Hex</returns>
		private static string ByteArrayToHex(byte[] byteArray)
		{
			var charUnit = new char[byteArray.Length * 2];
			for (var index = 0; index < byteArray.Length; index++)
			{
				var byteUnit = ((byte)(byteArray[index] >> 4));
				charUnit[index * 2] = (char)((byteUnit > 9)
					? (byteUnit + 0x37)
					: (byteUnit + 0x30));
				byteUnit = ((byte)(byteArray[index] & 0xF));
				charUnit[(index * 2) + 1] = (char)((byteUnit > 9)
					? (byteUnit + 0x37)
					: (byteUnit + 0x30));
			}
			return new string(charUnit);
		}

		/// <summary>
		/// 復号化
		/// </summary>
		/// <param name="encryptData">Encrypt Data</param>
		/// <returns>Decode String</returns>
		public static string DecryptAES256(string encryptData)
		{
			var encryptBytes = HexStringToByteArray(encryptData.ToUpper());
			var rijndael = new RijndaelManaged();

			rijndael.Key = Encoding.UTF8.GetBytes(Constants.NEWEBPAY_PAYMENT_HASHKEY);
			rijndael.IV = Encoding.UTF8.GetBytes(Constants.NEWEBPAY_PAYMENT_HASHIV);
			rijndael.Mode = CipherMode.CBC;
			rijndael.Padding = PaddingMode.None;

			var transform = rijndael.CreateDecryptor();
			return Encoding.UTF8.GetString(RemovePKCS7Padding(transform.TransformFinalBlock(
				encryptBytes, 0, encryptBytes.Length)));
		}

		/// <summary>
		/// Hex String To Byte Array
		/// </summary>
		/// <param name="hexString">Hex String</param>
		/// <returns>List Byte</returns>
		private static byte[] HexStringToByteArray(string hexString)
		{
			var hexStringLength = hexString.Length;
			var byteUnit = new byte[hexStringLength / 2];
			for (var index = 0; index < hexStringLength; index += 2)
			{
				var topChar = (hexString[index] > 0x40 ? hexString[index] - 0x37 : hexString[index] - 0x30) << 4;
				var bottomChar = hexString[index + 1] > 0x40 ? hexString[index + 1] - 0x37 : hexString[index + 1] - 0x30;
				byteUnit[index / 2] = Convert.ToByte(topChar + bottomChar);
			}
			return byteUnit;
		}

		/// <summary>
		/// Remove PKCS7 Padding
		/// </summary>
		/// <param name="data">Data</param>
		/// <returns>List Byte</returns>
		private static byte[] RemovePKCS7Padding(byte[] data)
		{
			var length = data[data.Length - 1];
			var output = new byte[data.Length - length];
			Buffer.BlockCopy(data, 0, output, 0, output.Length);
			return output;
		}

		/// <summary>
		/// Get Time Stamp Of Date Time Now
		/// </summary>
		/// <returns>Time Stamp Of Date Time Now</returns>
		public static long GetTimeStamp()
		{
			var dateNowTimeStamp = (long)(DateTime.Now.ToUniversalTime()
				- new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
			return dateNowTimeStamp;
		}

		/// <summary>
		/// Register As User Credit Card
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークレジットカード</returns>
		public static UserCreditCardModel RegisterAsUserCreditCard(
			string userId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			var userCreditCard = new UserCreditCardModel
			{
				UserId = userId,
				CooperationId = userId,
				CooperationId2 = string.Empty,
				CardDispName = NewebPayConstants.USERCREDITCARD_CARDDISPNAME_NEWEBPAYCUSTOMER,
				LastFourDigit = string.Empty,
				ExpirationMonth = string.Empty,
				ExpirationYear = string.Empty,
				AuthorName = string.Empty,
				DispFlg = Constants.FLG_USERCREDITCARD_DISP_FLG_OFF,
				LastChanged = lastChanged,
				CompanyCode = string.Empty,
				CooperationType = Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_NEWEBPAY,
			};

			new UserCreditCardService().Insert(
				userCreditCard,
				updateHistoryAction,
				accessor);
			return userCreditCard;
		}

		/// <summary>
		/// Get Payment Message Text
		/// </summary>
		/// <param name="externalPaymentType">External Payment Type</param>
		/// <param name="paymentMemo">Payment Memo</param>
		/// <returns>Payment Message Text</returns>
		public static string GetPaymentMessageText(string externalPaymentType, string paymentMemo)
		{
			var pattern = string.Empty;
			switch (externalPaymentType)
			{
				case Constants.FLG_PAYMENT_TYPE_NEWEBPAY_ATM:
					pattern = "(?=銀行番号).*(?=・)";
					break;

				case Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CVS:
					pattern = "(?=支払い番号).*(?=・)";
					break;

				case Constants.FLG_PAYMENT_TYPE_NEWEBPAY_BARCODE:
					pattern = "(?=バーコード).*(?=・)";
					break;

				default:
					return string.Empty;
			}
			var result = Regex.Match(paymentMemo, pattern).Value;
			return result;
		}
	}
}
