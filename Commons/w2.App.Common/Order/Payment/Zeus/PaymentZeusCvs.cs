/*
=========================================================================================================
  Module      : Payment Zeus CVS (PaymentZeusCvs.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Util;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.App.Common.Order.Payment.Zeus
{
	/// <summary>
	/// Payment Zeus CVS
	/// </summary>
	[Serializable]
	public class PaymentZeusCvs : ZeusApiBase, IPayment
	{
		/// <summary>Payment ZEUS secure CVS API url</summary>
		private const string PAYMENT_ZEUS_SECURE_CVS_API_URL = "https://linkpt.cardservice.co.jp/cgi-bin/cvs.cgi";
		/// <summary>Request payment: Client ip</summary>
		private const string REQUEST_PAYMENT_CLIENT_IP = "clientip";
		/// <summary>Request payment: Act</summary>
		private const string REQUEST_PAYMENT_ACT = "act";
		/// <summary>Request payment: Money</summary>
		private const string REQUEST_PAYMENT_MONEY = "money";
		/// <summary>Request payment: User name</summary>
		private const string REQUEST_PAYMENT_USER_NAME = "username";
		/// <summary>Request payment: Tel no</summary>
		private const string REQUEST_PAYMENT_TEL_NO = "telno";
		/// <summary>Request payment: Email</summary>
		private const string REQUEST_PAYMENT_EMAIL = "email";
		/// <summary>Request payment: Payment cvs</summary>
		private const string REQUEST_PAYMENT_PAYMENT_CVS = "pay_cvs";
		/// <summary>Request payment: Send id</summary>
		public const string REQUEST_PAYMENT_SEND_ID = "sendid";
		/// <summary>Request payment: Test id</summary>
		private const string REQUEST_PAYMENT_TESTID = "testid";
		/// <summary>Request payment: Test type</summary>
		private const string REQUEST_PAYMENT_TEST_TYPE = "test_type";

		/// <summary>Response payment: Order no</summary>
		public const string RESPONSE_PAYMENT_ORDER_NO = "order_no";
		/// <summary>Response payment: Payment no 1</summary>
		private const string RESPONSE_PAYMENT_PAY_NO1 = "pay_no1";
		/// <summary>Response payment: Payment no 2</summary>
		private const string RESPONSE_PAYMENT_PAY_NO2 = "pay_no2";
		/// <summary>Response payment: Payment limit</summary>
		private const string RESPONSE_PAYMENT_PAY_LIMIT = "pay_limit";
		/// <summary>Response payment: Payment url</summary>
		private const string RESPONSE_PAYMENT_PAY_URL = "pay_url";
		/// <summary>Response payment: Error code</summary>
		private const string RESPONSE_PAYMENT_ERROR_CODE = "error_code";
		/// <summary>Response payment: Payment date</summary>
		public const string RESPONSE_PAYMENT_PAY_DATE = "pay_date";
		/// <summary>Response payment: Payment status</summary>
		public const string RESPONSE_PAYMENT_PAY_STATUS = "status";

		/// <summary>Request payment act default: secure order</summary>
		private const string CONST_REQUEST_PAYMENT_ACT_DEFAULT = "secure_order";
		/// <summary>Response payment result success orrder</summary>
		private const string CONST_RESPONSE_PAYMENT_RESULT_SUCCESS_ORDER = "Success_order";

		/// <summary>Tag replace setting: pay no 1</summary>
		private const string TAG_SETTING_PAYNO1 = "@@ PayNo1 @@";
		/// <summary>Tag replace setting: pay no 2</summary>
		private const string TAG_SETTING_PAYNO2 = "@@ PayNo2 @@";
		/// <summary>Tag replace setting: pay limit</summary>
		private const string TAG_SETTING_PAYLIMIT = "@@ PayLimit @@";
		/// <summary>Tag replace setting: pay url</summary>
		private const string TAG_SETTING_PAYURL = "@@ PayUrl @@";
		/// <summary>Tag replace setting: amount</summary>
		private const string TAG_SETTING_AMOUNT = "@@ Amount @@";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="conveniType">支払先</param>
		public PaymentZeusCvs(string conveniType)
			: base(PAYMENT_ZEUS_SECURE_CVS_API_URL, isCvsPayment: true)
		{
			this.ConveniType = conveniType;
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="money">申込金額</param>
		/// <param name="userName">ユーザーの名前</param>
		/// <param name="telNo">ユーザーの電話番号</param>
		/// <param name="email">ユーザーのe-mailアドレス</param>
		/// <param name="sendId">フリーパラメータ(注文ID)</param>
		/// <returns>決済結果</returns>
		public Result Exec(
			decimal money,
			string userName,
			string telNo,
			string email,
			string sendId)
		{
			// パラメタ作成
			var param = CreateParam(money, userName, telNo, email, sendId);

			// 決済実行
			Result result;
			var responseText = "UNKNOWN";
			try
			{
				responseText = PostHttpRequest(param);
				var isSuccess = responseText.Contains(CONST_RESPONSE_PAYMENT_RESULT_SUCCESS_ORDER);

				var responses = responseText
					.Replace("\r\n", "\n")
					.Split('\n')
					.Where(res => res.Contains("="))
					.ToDictionary(
						item => item.Split('=')[0],
						item => item.Split('=')[1]);

				if (isSuccess)
				{
					// 成功時：ゼウス決済種別ID取得
					result = new Result(true)
					{
						ZeusOrderId = responses.First(kvp => (kvp.Key == RESPONSE_PAYMENT_ORDER_NO)).Value,
						PayNo1 = responses.First(kvp => (kvp.Key == RESPONSE_PAYMENT_PAY_NO1)).Value,
						PayNo2 = responses.First(kvp => (kvp.Key == RESPONSE_PAYMENT_PAY_NO2)).Value,
						PayUrl = responses.First(kvp => (kvp.Key == RESPONSE_PAYMENT_PAY_URL)).Value,
						PayLimit = responses.First(kvp => (kvp.Key == RESPONSE_PAYMENT_PAY_LIMIT)).Value,
					};
				}
				else
				{
					// 失敗時
					result = new Result(false)
					{
						ErrorMessage = responseText,
						ErrorCode = responses.FirstOrDefault(kvp => (kvp.Key == RESPONSE_PAYMENT_ERROR_CODE)).Value,
					};
				}
			}
			catch (Exception ex)
			{
				// 失敗時
				result = new Result(false)
				{
					ErrorMessage = ex.Message,
				};
				FileLogger.WriteError("APIエラー", ex);
			}

			// ログ格納処理
			WriteLog(
				PaymentFileLogger.PaymentProcessingType.ApiRequest,
				result.IsSuccess,
				new KeyValuePair<string, string>(REQUEST_PAYMENT_SEND_ID, sendId),
				new KeyValuePair<string, string>(REQUEST_PAYMENT_MONEY, money.ToPriceString()),
				new KeyValuePair<string, string>(REQUEST_PAYMENT_PAYMENT_CVS, this.ConveniType),
				new KeyValuePair<string, string>("response", responseText));
			return result;
		}

		/// <summary>
		/// パラメタ作成
		/// </summary>
		/// <param name="money">申込金額</param>
		/// <param name="userNameKana">ユーザーの名前(カナ)</param>
		/// <param name="telNo">ユーザーの電話番号</param>
		/// <param name="email">ユーザーのe-mailアドレス</param>
		/// <param name="sendId">フリーパラメータ(注文ID)</param>
		/// <returns>パラメタ</returns>
		private Dictionary<string, string> CreateParam(
			decimal money,
			string userNameKana,
			string telNo,
			string email,
			string sendId)
		{

			var param = new Dictionary<string, string>
			{
				{ REQUEST_PAYMENT_CLIENT_IP, this.ClientIP },
				{ REQUEST_PAYMENT_ACT, CONST_REQUEST_PAYMENT_ACT_DEFAULT },
				{ REQUEST_PAYMENT_MONEY, money.ToPriceString() },
				{ REQUEST_PAYMENT_USER_NAME, userNameKana },
				{ REQUEST_PAYMENT_TEL_NO, telNo.Replace("-", string.Empty) },
				{ REQUEST_PAYMENT_EMAIL, email },
				{ REQUEST_PAYMENT_PAYMENT_CVS, this.ConveniType },
				{ REQUEST_PAYMENT_SEND_ID, sendId },
			};

			// For case using test, add param for test
			if (Constants.PAYMENT_CVS_ZUES_PAYMENT_TEST_FLAG)
			{
				param.Add(REQUEST_PAYMENT_TESTID, Constants.PAYMENT_CVS_ZUES_TEST_ID);
				param.Add(REQUEST_PAYMENT_TEST_TYPE, Constants.PAYMENT_CVS_ZUES_TEST_TYPE);
			}

			return param;
		}

		/// <summary>
		/// Zeus Secure Cvs Result
		/// </summary>
		public class Result
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="isSuccess">成功したか</param>
			public Result(bool isSuccess)
			{
				this.IsSuccess = isSuccess;
			}

			/// <summary>
			/// 結果メッセージ生成
			/// </summary>
			/// <param name="convenienceCode">支払先コンビニコード</param>
			/// <param name="amount">合計金額</param>
			/// <param name="languageLocaleId">注文者の言語ロケールID</param>
			/// <returns>Result message (HTML, Text)</returns>
			public (string Html, string Text) CreateResultMessage(
				string convenienceCode,
				decimal amount,
				string languageLocaleId = "")
			{
				var filePath = Path.Combine(
					AppDomain.CurrentDomain.BaseDirectory,
					Constants.PATH_XML_CVS_ZEUS);
				var message = XDocument.Load(filePath)
					.Descendants()
					.First(element => element.Name.ToString() == GetCvsMessageDiv(convenienceCode));

				var htmlMessage = message.Elements().First(element => element.Name.ToString() == "Html").Value
					.Replace(TAG_SETTING_PAYNO1, this.PayNo1)
					.Replace(TAG_SETTING_PAYNO2, this.PayNo2)
					.Replace(TAG_SETTING_PAYURL, this.PayUrl)
					.Replace(TAG_SETTING_AMOUNT, StringUtility.ToNumeric(amount.ToPriceString()))
					.Replace(TAG_SETTING_PAYLIMIT,
						DateTimeUtility.ToString(
							DateTime.ParseExact(this.PayLimit, Constants.DATE_FORMAT_SHORT, null),
							DateTimeUtility.FormatType.ShortDateHourMinute2Letter,
							languageLocaleId));
				var textMessage = message.Elements().First(element => element.Name.ToString() == "Text").Value
					.Replace(TAG_SETTING_PAYNO1, this.PayNo1)
					.Replace(TAG_SETTING_PAYNO2, this.PayNo2)
					.Replace(TAG_SETTING_PAYURL, this.PayUrl)
					.Replace(TAG_SETTING_AMOUNT, StringUtility.ToNumeric(amount.ToPriceString()))
					.Replace(TAG_SETTING_PAYLIMIT,
						DateTimeUtility.ToString(
							DateTime.ParseExact(this.PayLimit, Constants.DATE_FORMAT_SHORT, null),
							DateTimeUtility.FormatType.ShortDateHourMinute2Letter,
							languageLocaleId));
				return (Html: htmlMessage, Text: textMessage);
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
					case "D001":
						// セイコーマート
						return "SevenEleven";

					case "D002":
						// ローソン
						return "Lowson";

					case "D030":
						// ファミリーマート
						return "FamilyMart";

					case "D015":
						return "Seicomart";

					case "D050":
						// ミニストップ
						return "MiniStop";

					case "D060":
						// デイリーヤマザキ
						return "DailyYamazaki";

					default:
						throw new ArgumentOutOfRangeException(nameof(convenienceCode), convenienceCode, null);
				}
			}

			/// <summary>
			/// Get error code message
			/// </summary>
			/// <returns>Error code message</returns>
			public string GetErrorCodeMessage()
			{
				if (string.IsNullOrEmpty(this.ErrorCode)) return string.Empty;

				var document = XDocument.Parse(Properties.Resources.ZeusApiMessages);
				var errorCodeMessage = document
					.Root
					.Elements("ErrorTypes")
					.Elements("Message")
					.Where(e => e.Attribute("code").Value == this.ErrorCode)
					.FirstOrDefault()
					.Value;

				if (string.IsNullOrEmpty(errorCodeMessage))
				{
					PaymentFileLogger.WritePaymentLog(
						false,
						string.Empty,
						PaymentFileLogger.PaymentType.Zeus,
						PaymentFileLogger.PaymentProcessingType.GetErrorMessage,
						string.Format("ZEUSエラーが変換できませんでした。エラーコード{0}", this.ErrorCode));
				}

				return errorCodeMessage;
			}

			/// <summary>成功したか</summary>
			public bool IsSuccess { get; private set; }
			/// <summary>ゼウス注文ID</summary>
			public string ZeusOrderId { get; set; }
			/// <summary>エラーメッセージ</summary>
			public string ErrorMessage { get; set; }
			/// <summary>エラーコード</summary>
			public string ErrorCode { get; set; }
			/// <summary>払込番号1</summary>
			public string PayNo1 { get; set; }
			/// <summary>払込番号2</summary>
			public string PayNo2 { get; set; }
			/// <summary>支払期限</summary>
			public string PayLimit { get; set; }
			/// <summary>リダイレクト先URL</summary>
			public string PayUrl { get; set; }
		}

		/// <summary>支払先</summary>
		public string ConveniType { get; private set; }
	}
}
