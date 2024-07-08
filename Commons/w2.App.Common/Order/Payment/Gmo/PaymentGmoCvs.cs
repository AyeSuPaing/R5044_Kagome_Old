/*
=========================================================================================================
  Module      : PaymentGmoCvs(PaymentGmoCvs.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.GMO
{
	/// <summary>
	/// GMO convenience
	/// </summary>
	public class PaymentGmoCvs : PaymentGmo
	{
		// セブンイレブン
		const string CONVENIENCE_SEVEN_ELEVENT = "00007";

		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentGmoCvs()
			: this(
				Constants.PAYMENT_SETTING_GMO_CVS_AUTH_SERVER_URL,
				Constants.PAYMENT_SETTING_GMO_CVS_SHOP_ID,
				Constants.PAYMENT_SETTING_GMO_CVS_SHOP_PASS)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="serverUrl">ServerUrl</param>
		/// <param name="shopId">Shop id</param>
		/// <param name="shopPass">Shop pass</param>
		public PaymentGmoCvs(
			string serverUrl,
			string shopId,
			string shopPass)
		{
			this.ServerUrl = serverUrl;
			this.ShopId = shopId;
			this.ShopPass = shopPass;
		}


		/// <summary>
		/// Create entry transaction
		/// </summary>
		/// <param name="gmoOrderId">GMO order id</param>
		/// <param name="priceTotal">Price total</param>
		/// <returns>True:success、False:fail</returns>
		public bool EntryTran(string gmoOrderId, decimal priceTotal)
		{
			// Create POST parameter
			var parameters = new NameValueCollection()
			{
				{PARAM_SHOPID, this.ShopId},
				{PARAM_SHOPPASS, this.ShopPass},
				{PARAM_ORDERID, gmoOrderId},
				{PARAM_AMOUNT, priceTotal.ToPriceString()}
			};

			// Execute
			if (SendParams(Constants.PAYMENT_SETTING_GMO_CVS_ENTRY_TRAN, parameters, gmoOrderId))
			{
				// Success and return access id, access pass
				this.AccessId = (string)this.Result.Parameters[PARAM_ACCESSID];
				this.AccessPass = (string)this.Result.Parameters[PARAM_ACCESSPASS];

				return true;
			}
			else
			{
				// Write log gmo transaction cvs for regist fail
				//WriteLogGmoTranCvsForRegistFail(); //一旦必要ないためコメントアウト

				this.ErrorMessages = this.Result.ErrorMessages;

				// Fail
				return false;
			}
		}

		/// <summary>
		/// Execute transaction convenience
		/// </summary>
		/// <param name="gmoOrderId">GMO order id</param>
		/// <param name="convenienceCode">Convenience code</param>
		/// <param name="customerName">Customer name</param>
		/// <param name="customerNameKana">Customer name Kana</param>
		/// <param name="customerTelNo">Customer tel no</param>
		/// <returns>True:success、False:fail</returns>
		public bool ExecTran(
			string gmoOrderId,
			string convenienceCode,
			string customerName,
			string customerNameKana,
			string customerTelNo)
		{
			// Create POST parameter
			var parameters = new NameValueCollection()
			{
				{PARAM_ACCESSID, this.AccessId},
				{PARAM_ACCESSPASS, this.AccessPass},
				{PARAM_ORDERID, gmoOrderId},
				{PARAM_CONVENIENCE, convenienceCode},
				{PARAM_CUSTOMER_NAME_KANA, customerNameKana},
				{PARAM_CUSTOMER_TEL_NO, customerTelNo},
				{PARAM_CUSTOMER_NAME, (convenienceCode == CONVENIENCE_SEVEN_ELEVENT) ? StringUtility.ToZenkaku(customerName) : customerName},
				{PARAM_PAYMENT_TERM_DAY, Constants.PAYMENT_SETTING_GMO_CVS_PAYMENT_LIMIT_DAY},
				{PARAM_RECEIPTS_DISP_11, Constants.PAYMENT_SETTING_GMO_CVS_RECEIPTS_DISP_11},
				{PARAM_RECEIPTS_DISP_12, Constants.PAYMENT_SETTING_GMO_CVS_RECEIPTS_DISP_12},
				{PARAM_RECEIPTS_DISP_13, Constants.PAYMENT_SETTING_GMO_CVS_RECEIPTS_DISP_13},
			};

			var log = new StringBuilder();
			// Execute
			if (SendParams(Constants.PAYMENT_SETTING_GMO_CVS_EXEC_TRAN, parameters, gmoOrderId))
			{
				// Get data
				this.ConfNo = StringUtility.ToEmpty(this.Result.Parameters[PARAM_CONF_NO]);
				this.ReceiptNo = StringUtility.ToEmpty(this.Result.Parameters[PARAM_RECEIPT_NO]);
				this.PaymentTerm = StringUtility.ToEmpty(this.Result.Parameters[PARAM_PAYMENT_TERM]);
				this.ReceiptUrl = StringUtility.ToEmpty(this.Result.Parameters[PARAM_RECEIPT_URL]);

				// Log output
				log.Append("\t").Append("「成功」");
				log.Append("\t").Append(StringUtility.ToEmpty(this.Result.Parameters[PARAM_ORDERID]));
				log.Append("\t").Append(StringUtility.ToEmpty(this.Result.Parameters[PARAM_CONVENIENCE]));
				log.Append("\t").Append(StringUtility.ToEmpty(this.Result.Parameters[PARAM_CONF_NO]));
				log.Append("\t").Append(StringUtility.ToEmpty(this.Result.Parameters[PARAM_RECEIPT_NO]));
				log.Append("\t").Append(StringUtility.ToEmpty(this.Result.Parameters[PARAM_PAYMENT_TERM]));
				log.Append("\t").Append(StringUtility.ToEmpty(this.Result.Parameters[PARAM_TRAN_DATE]));
				log.Append("\t").Append(StringUtility.ToEmpty(this.Result.Parameters[PARAM_RECEIPT_URL]));
				log.Append("\t").Append(StringUtility.ToEmpty(this.Result.Parameters[PARAM_CHECK_STRING]));

				// Write log Gmo transaction cvs
				WriteLogGmoTranCvs(log.ToString());

				return true;
			}
			else
			{
				// Write log gmo transaction cvs for regist fail
				// WriteLogGmoTranCvsForRegistFail(); //一旦必要ないためコメントアウト

				this.ErrorMessages = this.Result.ErrorMessages;

				// Fail
				return false;
			}
		}

		/// <summary>
		/// 結果メッセージ生成
		/// </summary>
		/// <param name="convenienceCode">支払先コンビニコード</param>
		/// <param name="amount">合計金額</param>
		/// <param name="languageLocaleId">注文者の言語ロケールID</param>
		public Tuple<string, string> CreateResultMessage(string convenienceCode, string amount, string languageLocaleId = "")
		{
			var message = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + Constants.PATH_XML_CVS_GMO)
				.Descendants()
				.First(element => element.Name.ToString() == GetCvsMessageDiv(convenienceCode));
			var result = new Tuple<string, string>(
				message.Elements().First(element => element.Name.ToString() == "Html").Value
					.Replace("@@ ConfNo @@", this.ConfNo)
					.Replace("@@ ReceiptNo @@", this.ReceiptNo)
					.Replace("@@ OnlinePaymentId @@", this.OnlinePaymentId)
					.Replace("@@ Amount @@", amount.ToPriceString())
					.Replace("@@ ReceiptUrl @@", this.ReceiptUrl)
					.Replace("@@ ExpiredDate @@",
						DateTimeUtility.ToString(
							DateTime.ParseExact(this.Result.Parameters[PARAM_PAYMENT_TERM], "yyyyMMddHHmmss", null),
							DateTimeUtility.FormatType.ShortDateHourMinute2Letter,
							languageLocaleId)),
				message.Elements().First(element => element.Name.ToString() == "Text").Value
					.Replace("@@ ConfNo @@", this.ConfNo)
					.Replace("@@ ReceiptNo @@", this.ReceiptNo)
					.Replace("@@ OnlinePaymentId @@", this.OnlinePaymentId)
					.Replace("@@ Amount @@", amount.ToPriceString())
					.Replace("@@ ReceiptUrl @@", this.ReceiptUrl)
					.Replace("@@ ExpiredDate @@",
						DateTimeUtility.ToString(
							DateTime.ParseExact(this.Result.Parameters[PARAM_PAYMENT_TERM], "yyyyMMddHHmmss", null),
							DateTimeUtility.FormatType.ShortDateHourMinute2Letter,
							languageLocaleId))
				);
			return result;
		}

		/// <summary>
		/// コンビニメッセージ区分取得
		/// </summary>
		/// <param name="convenienceCode">支払先コンビニコード</param>
		/// <returns>コンビニメッセージ区分</returns>
		private string GetCvsMessageDiv(string convenienceCode)
		{
			switch (convenienceCode)
			{
				case "00006": // デイリーヤマザキ
					return "DailyYamazaki";

				case "00007": // セブンイレブン
					return "SevenEleven";

				case "00009": // スリーエフ
					return "ThreeF";

				case "10001": // ローソン
					return "Lawson";

				case "10005": // ミニストップ
					return "MiniStop";

				case "10002": // ファミリーマート
					return "FamilyMart";

				case "10003": // サンクス
					return "Thanks";

				case "10004": // サークルK
					return "CircleK";

				case "10008": // セイコーマート
					return "Seicomart";

				default:
					return string.Empty;
			}
		}

		/// <summary>
		/// Cancel transaction convenience
		/// </summary>
		/// <param name="gmoOrderId">GMO order id</param>
		/// <param name="cardTranId">Card tran id</param>
		/// <param name="order">Order</param>
		/// <returns>True:success、False:fail</returns>
		public bool CancelTran(
			string gmoOrderId,
			string cardTranId,
			OrderModel order)
		{
			var cardTran = cardTranId.Split(' ');
			var gmoAccessId = cardTran[0];
			var gmoAccessPass = cardTran.Length > 1 ? cardTranId.Split(' ')[1] : string.Empty;

			// Create POST parameter
			var parameters = new NameValueCollection()
			{
				{PARAM_SHOPID, Constants.PAYMENT_SETTING_GMO_CVS_SHOP_ID},
				{PARAM_SHOPPASS, Constants.PAYMENT_SETTING_GMO_CVS_SHOP_PASS},
				{PARAM_ORDERID, gmoOrderId},
				{PARAM_ACCESSID, gmoAccessId},
				{PARAM_ACCESSPASS, gmoAccessPass}
			};

			var log = new StringBuilder();
			// Execute
			if (SendParams(Constants.PAYMENT_SETTING_GMO_CVS_CANCEL, parameters, gmoOrderId))
			{
				// Success and return status
				this.Status = (string)this.Result.Parameters[PARAM_STATUS];

				// Log output
				log.Append("\t").Append("「支払停止成功」");
				log.Append("\t").Append(this.Result.Parameters[PARAM_ORDERID]);

				// Write log Gmo transaction cvs
				WriteLogGmoTranCvs(log.ToString());

				return true;
			}
			else
			{
				// Log error
				log.Append("\t").Append("「エラー」").Append(this.Result.Parameters[PARAM_ERRCODE]);
				log.Append("\t").Append(this.Result.Parameters[PARAM_ORDERID]);
				log.Append("\t").Append(this.Result.Parameters[PARAM_ERRINFO]);
				log.Append("\t").Append(this.Result.ErrorMessages);

				// Write log Gmo transaction cvs
				WriteLogGmoTranCvs(log.ToString());

				this.ErrorMessages = this.Result.ErrorMessages;

				// Fail
				return false;
			}
		}

		/// <summary>
		/// Send parameter
		/// </summary>
		/// <param name="url">Url</param>
		/// <param name="parameters">Parameters</param>
		/// <param name="gmoOrderId">GMO order id</param>
		/// <remarks>The result is stored in the Result property</remarks>
		public bool SendParams(
			string url,
			NameValueCollection parameters,
			string gmoOrderId = "")
		{
			// Concatenate parameter
			var endoding = Encoding.GetEncoding("Shift_JIS");
			var postParameters = new List<string>();
			foreach (string key in parameters)
			{
				postParameters.Add(string.Concat(key, "=", HttpUtility.UrlEncode(parameters[key], endoding)));
			}

			// Convert
			var data = Encoding.ASCII.GetBytes(string.Join("&", postParameters.ToArray()));

			// Send data
			var request = WebRequest.Create(this.ServerUrl + url);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = data.Length;
			using (var requestStream = request.GetRequestStream())
			{
				requestStream.Write(data, 0, data.Length);
			}

			// Receive response
			string responseString = null;
			using (var response = request.GetResponse())
			using (var responseStream = response.GetResponseStream())
			using (var streamReader = new StreamReader(responseStream, endoding))
			{
				responseString = streamReader.ReadToEnd();
			}

			// Retrieve result from response string
			this.Result = new ResponseResult(responseString);

			return this.Result.IsSuccess;
		}

		/// <summary>
		/// Write log gmo transaction cvs for regist fail
		/// </summary>
		private void WriteLogGmoTranCvsForRegistFail()
		{
			// Log error
			var log = new StringBuilder();
			log.Append("\t").Append("「エラー」").Append(StringUtility.ToEmpty(this.Result.Parameters[PARAM_ERRCODE]));
			log.Append("\t").Append(StringUtility.ToEmpty(this.Result.Parameters[PARAM_ORDERID]));
			log.Append("\t").Append(StringUtility.ToEmpty(this.Result.Parameters[PARAM_ERRINFO]));
			log.Append("\t").Append(StringUtility.ToEmpty(this.Result.ErrorMessages));
			
			// Write log Gmo transaction cvs
			WriteLogGmoTranCvs(log.ToString());
		}

		/// <summary>
		/// Write log Gmo transaction cvs
		/// </summary>
		/// <param name="log">Log</param>
		public void WriteLogGmoTranCvs(string log)
		{
			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				null,
				Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE,
				PaymentFileLogger.PaymentType.Gmo,
				PaymentFileLogger.PaymentProcessingType.Unknown,
				log);
		}

		/// <summary>
		/// Create Log Info For Paymment Gmo
		/// </summary>
		/// <param name="transactionName">Transaction name</param>
		/// <param name="order">Order</param>
		public void CreateLogInfoForPaymmentGmo(string transactionName, OrderModel order)
		{
			var logInfo = new StringBuilder();
			logInfo.Append(transactionName).Append("失敗：[");
			logInfo.Append("user_id=").Append(order.UserId).Append(",");
			logInfo.Append("order_id=").Append(order.OrderId).Append(",");
			logInfo.Append("owner_kbn=").Append((order.Owner == null) ? string.Empty : order.Owner.OwnerKbn).Append(",");
			logInfo.Append("cart_id=").Append(string.Empty).Append(",");
			logInfo.Append("shop_id=").Append(order.ShopId).Append("]");

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				null,
				Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE,
				PaymentFileLogger.PaymentType.Gmo,
				PaymentFileLogger.PaymentProcessingType.Unknown,
				logInfo.ToString(),
				new Dictionary<string, string>
				{
					{ "orderId", order.OrderId },
					{ "cardTranId", order.CardTranId }
				});
		}
	}
}