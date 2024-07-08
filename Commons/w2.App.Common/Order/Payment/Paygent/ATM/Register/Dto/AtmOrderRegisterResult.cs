/*
=========================================================================================================
  Module      : Atm Order Register Result(AtmOrderRegisterResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order.Payment.Paygent.Foundation;
using w2.App.Common.Util;
using w2.Common.Util;

namespace w2.App.Common.Order.Payment.Paygent.ATM.Register.Dto
{
	/// <summary>
	/// Atm order register result
	/// </summary>
	[Serializable]
	public class AtmOrderRegisterResult
	{
		/// <summary>Tag replace setting: Pay center number</summary>
		private const string TAG_SETTING_PAY_CENTER_NUMBER = "@@ PayCenterNumber @@";
		/// <summary>Tag replace setting: Customer number</summary>
		private const string TAG_SETTING_CUSTOMER_NUMBER = "@@ CustomerNumber @@";
		/// <summary>Tag replace setting: Conf number</summary>
		private const string TAG_SETTING_CONF_NUMBER = "@@ ConfNumber @@";
		/// <summary>Tag replace setting: Payment limit date</summary>
		private const string TAG_SETTING_PAYMENT_LIMIT_DATE = "@@ PaymentLimitDate @@";
		/// <summary>Tag replace setting: Amount</summary>
		private const string TAG_SETTING_AMOUNT = "@@ Amount @@";
		/// <summary>Tag replace setting: Payment date</summary>
		private const string TAG_SETTING_PAYMENTSERVICE_PAYMENT_DATE = "@@ PaymentDate @@";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paygentApiResult">レスポンス結果</param>
		public AtmOrderRegisterResult(PaygentApiResult paygentApiResult)
		{
			this.Response = (AtmOrderRegisterResponse)paygentApiResult.Response;
			this.ReasonPhrase = paygentApiResult.ReasonPhrase;
			this.IsSuccess = paygentApiResult.IsSuccess();
		}

		/// <summary>
		/// Get error message
		/// </summary>
		/// <returns>Error message</returns>
		public string GetErrorMessage()
		{
			var errorMessage = (this.Response != null)
				? LogCreator.CreateErrorMessage(
					this.Response.ResponseCode,
					this.Response.ResponseDetail)
				: this.ReasonPhrase ?? string.Empty;
			return errorMessage;
		}

		/// <summar>
		/// Create result message
		/// </summary>
		/// <param name="cart">Cart</param>
		/// <returns>Result message</returns>
		public (string Html, string Text) CreateResultMessage(CartObject cart)
		{
			var filePath = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				Constants.PATH_XML_ATM_PAYGENT);
			var message = XDocument.Load(filePath).Descendants("PaygentMessage").FirstOrDefault();
			var htmlMessage = message.Elements().First(element => element.Name.ToString() == "Html").Value
				.Replace(TAG_SETTING_PAY_CENTER_NUMBER, this.Response.PayCenterNumber)
				.Replace(TAG_SETTING_CUSTOMER_NUMBER, this.Response.CustomerNumber)
				.Replace(TAG_SETTING_CONF_NUMBER, this.Response.ConfNumber)
				.Replace(TAG_SETTING_AMOUNT, StringUtility.ToNumeric(cart.PriceTotal.ToPriceString()))
				.Replace(TAG_SETTING_PAYMENT_LIMIT_DATE,
					DateTimeUtility.ToString(
						DateTime.ParseExact(this.Response.PaymentLimitDate, Constants.DATE_FORMAT_SHORT, null),
						DateTimeUtility.FormatType.ShortDateHourMinute2Letter,
						cart.Owner.DispLanguageLocaleId));

			var textMessage = message.Elements().First(element => element.Name.ToString() == "Text").Value
				.Replace(TAG_SETTING_PAYMENTSERVICE_PAYMENT_DATE,
					DateTimeUtility.ToString(
						DateTime.Now,
						DateTimeUtility.FormatType.ShortDateHourMinute2Letter,
						cart.Owner.DispLanguageLocaleId))
				.Replace(TAG_SETTING_PAY_CENTER_NUMBER, this.Response.PayCenterNumber)
				.Replace(TAG_SETTING_CUSTOMER_NUMBER, this.Response.CustomerNumber)
				.Replace(TAG_SETTING_CONF_NUMBER, this.Response.ConfNumber)
				.Replace(TAG_SETTING_AMOUNT, StringUtility.ToNumeric(cart.PriceTotal.ToPriceString()))
				.Replace(TAG_SETTING_PAYMENT_LIMIT_DATE,
					DateTimeUtility.ToString(
						DateTime.ParseExact(this.Response.PaymentLimitDate, Constants.DATE_FORMAT_SHORT, null),
						DateTimeUtility.FormatType.ShortDateHourMinute2Letter,
						cart.Owner.DispLanguageLocaleId));

			return (Html: htmlMessage, Text: textMessage);
		}

		/// <summary>成功したか</summary>
		public bool IsSuccess { get; private set; }
		/// <summary>レスポンス</summary>
		public AtmOrderRegisterResponse Response { get; set; }
		/// <summary>結果理由</summary>
		public string ReasonPhrase { get; private set; }
	}
}
