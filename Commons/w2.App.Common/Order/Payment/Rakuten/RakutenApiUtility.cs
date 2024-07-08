/*
=========================================================================================================
  Module      : Rakuten Api Utility (RakutenApiUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order.Payment.PayTg;
using w2.App.Common.Order.Payment.Rakuten.Authorize;
using w2.App.Common.Util;
using w2.App.Common.Web;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.Rakuten
{
	/// <summary>
	/// Rakuten api utility
	/// </summary>
	public static class RakutenApiUtility
	{
		/// <summary>
		/// Create rakuten cvs authorize request
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="rakutenCvsType">Rakuten cvs type</param>
		/// <returns>Rakuten authorize request</returns>
		public static RakutenAuthorizeRequest CreateRakutenCvsAuthorizeRequest(OrderModel order, string rakutenCvsType = "")
		{
			var ipAddress = (HttpContext.Current != null)
				? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
				: string.Empty;

			var customer = new Customer
			{
				FirstName = StringUtility.ToZenkaku(order.Owner.OwnerName1),
				LastName = StringUtility.ToZenkaku(order.Owner.OwnerName2),
				Phone = order.Owner.OwnerTel1,
			};

			var amount = GetSendingAmount(order);
			var requestItems = new Items[]
			{
				new Items
				{
					Name = Constants.PAYMENT_RAKUTEN_CVS_RECEIPT_DISPLAY_NAME,
					Id = RakutenConstants.DUMMY_VALUE_OF_PRODUCT_ID,
					Price = amount,
					Quantity = RakutenConstants.DUMMY_VALUE_OF_PRODUCT_QUANTITY,
				},
			};

			var expirationDate = CreateExpirationDateAuth();
			var cvsPayment = new CvsPayment
			{
				Version = RakutenConstants.KEY_VERSION,
				Amount = amount,
				OrderDate = int.Parse(((DateTime)order.OrderDate).ToString("yyyyMMdd")),
				ExpirationDate = expirationDate,
				AddHandlingFee = false,
			};

			var notificationUrl = string.Format(
				"{0}{1}{2}{3}",
				Constants.PROTOCOL_HTTP,
				Constants.SITE_DOMAIN,
				Constants.PATH_ROOT_FRONT_PC,
				Constants.PAGE_FRONT_PAYMENT_RAKUTEN_CVS_PAYMENT_RECEIVE);

			var subServiceId = string.Empty;
			switch (rakutenCvsType)
			{
				case RakutenConstants.PAYMENT_RAKUTENCVSDEF_TYPE_SUBSERVICEID_SEVEN:
					subServiceId = Constants.PAYMENT_RAKUTENCVSDEF_SUBSERVICEID_SEVEN;
					break;

				case RakutenConstants.PAYMENT_RAKUTENCVSDEF_TYPE_SUBSERVICEID_ECON:
					subServiceId = Constants.PAYMENT_RAKUTENCVSDEF_SUBSERVICEID_ECON;
					break;

				case RakutenConstants.PAYMENT_RAKUTENCVSDEF_TYPE_SUBSERVICEID_WELLNET:
					subServiceId = Constants.PAYMENT_RAKUTENCVSDEF_SUBSERVICEID_WELLNET;
					break;
			}

			var result = new RakutenAuthorizeRequest
			{
				SubServiceId = subServiceId,
				ServiceReferenceId = order.OrderId,
				GrossAmount = amount,
				NotificationUrl = notificationUrl,
				CvsPayment = cvsPayment,
				Order = new Authorize.Order(ipAddress, true)
				{
					Customer = customer,
					Items = requestItems,
				},
			};

			return result;
		}

		/// <summary>
		/// Get sending amount
		/// </summary>
		/// <param name="order">Order</param>
		/// <returns>Amount</returns>
		private static decimal GetSendingAmount(OrderModel order)
		{
			var result = CurrencyManager.GetSendingAmount(
				order.LastBilledAmount,
				order.SettlementAmount,
				order.SettlementCurrency);
			return result;
		}

		/// <summary>
		/// Write log rakuten request
		/// </summary>
		/// <param name="request">Request</param>
		public static void WriteLogRakutenRequest(object request)
		{
			var jsonRequest = JsonConvert.SerializeObject(
				request,
				new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore,
					Converters = new List<JsonConverter> { new DecimalJsonConverter() },
				});

			FileLogger.Write("RakutenCvs", jsonRequest);
		}

		/// <summary>
		/// Create expiration date auth
		/// </summary>
		/// <returns>Expiration date</returns>
		private static int CreateExpirationDateAuth()
		{
			var expirationDate = (Constants.PAYMENT_RAKUTEN_CVSDEF_AUTHLIMITDAY < RakutenConstants.DATE_LIMIT_ALLOW_AUTH)
				? DateTime.Now.AddDays(Constants.PAYMENT_RAKUTEN_CVSDEF_AUTHLIMITDAY)
				: DateTime.Now.AddDays(RakutenConstants.DATE_LIMIT_ALLOW_AUTH);
			var result = int.Parse(expirationDate.ToString(Constants.DATE_FORMAT_SHORT));
			return result;
		}

		/// <summary>
		/// Create result message
		/// </summary>
		/// <param name="convenienceCode">Convenience code</param>
		/// <param name="reference">Reference</param>
		/// <param name="amount">Amount</param>
		/// <param name="expiredDate">Expired date</param>
		/// <param name="languageLocaleId">Language locale id</param>
		/// <returns>Result message</returns>
		public static Tuple<string, string> CreateResultMessage(
			string convenienceCode,
			string reference,
			string amount,
			int expiredDate,
			string languageLocaleId = "")
		{
			var messageTemplate = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + Constants.PATH_XML_CVS_RAKUTEN)
				.Descendants()
				.First(element => (element.Name.ToString() == GetCvsMessageDiv(convenienceCode)));

			var messageHtml = messageTemplate
				.Elements()
				.First(element => (element.Name.ToString() == "Html"))
				.Value
				.ReplaceMessage(
					reference,
					amount,
					expiredDate,
					languageLocaleId);

			var messageText = messageTemplate
				.Elements()
				.First(element => (element.Name.ToString() == "Text"))
				.Value
				.ReplaceMessage(
					reference,
					amount,
					expiredDate,
					languageLocaleId);

			var result = new Tuple<string, string>(messageHtml, messageText);
			return result;
		}

		/// <summary>
		/// Replace message
		/// </summary>
		/// <param name="message">Message</param>
		/// <param name="reference">Reference</param>
		/// <param name="amount">Amount</param>
		/// <param name="expiredDate">Expired date</param>
		/// <param name="languageLocaleId">Language locale id</param>
		/// <returns>Result message</returns>
		private static string ReplaceMessage(
			this string message,
			string reference,
			string amount,
			int expiredDate,
			string languageLocaleId = "")
		{
			var result = message
				.Replace("@@ Reference @@", reference)
				.Replace("@@ Amount @@", amount)
				.Replace("@@ ExpiredDate @@",
					DateTimeUtility.ToString(
						DateTime.ParseExact(
							string.Format("{0}2359", expiredDate),
							"yyyyMMddHHmm",
							null),
						DateTimeUtility.FormatType.ShortDateHourMinute2Letter,
						languageLocaleId));

			return result;
		}

		/// <summary>
		/// コンビニメッセージ区分取得
		/// </summary>
		/// <param name="convenienceCode">支払先コンビニコード</param>
		/// <returns>コンビニメッセージ区分</returns>
		private static string GetCvsMessageDiv(string convenienceCode)
		{
			switch (convenienceCode)
			{
				// セブンイレブン
				case "SevenEleven":
					return "SevenEleven";

				// ローソン
				// ミニストップ
				// ファミリーマート
				// セイコーマート
				case "Econ":
					return "Econ";

				// デイリーヤマザキ
				case "Wellnet":
					return "Wellnet";

				default:
					// Default is convenience seven eleven
					return RakutenConstants.CVS_TYPE_DEFAULT;
			}
		}

		/// <summary>
		/// Decimal json converter
		/// </summary>
		public class DecimalJsonConverter : JsonConverter<decimal>
		{
			/// <summary>
			/// Read json
			/// </summary>
			/// <param name="reader">Reader</param>
			/// <param name="objectType">Object type</param>
			/// <param name="existingValue">Existing value</param>
			/// <param name="hasExistingValue">Has existing value</param>
			/// <param name="serializer">Serializer</param>
			/// <returns>Json</returns>
			public override decimal ReadJson(
				JsonReader reader,
				Type objectType,
				decimal existingValue,
				bool hasExistingValue,
				JsonSerializer serializer)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// Write json
			/// </summary>
			/// <param name="writer">Writer</param>
			/// <param name="value">Value</param>
			/// <param name="serializer">Serializer</param>
			public override void WriteJson(
				JsonWriter writer,
				decimal value,
				JsonSerializer serializer)
			{
				// Customise how you want the decimal value to be output in here
				// for example, you may want to consider culture
				writer.WriteRawValue(string.Format("{0:0.#}", value));
			}
		}
	}
}
