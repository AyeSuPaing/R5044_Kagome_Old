/*
=========================================================================================================
  Module      : Paygent Banknet Order Register Result (BanknetOrderRegisterResult.cs)
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

namespace w2.App.Common.Order.Payment.Paygent.Banknet.Register.Dto
{
	/// <summary>
	/// Paygent Banknet order register result
	/// </summary>
	[Serializable]
	public class BanknetOrderRegisterResult
	{
		/// <summary>Tag replace setting: payment limit date</summary>
		private const string TAG_SETTING_PAYMENT_LIMIT_DATE = "@@ PaymentLimitDate @@";
		/// <summary>Tag replace setting: Asp Url</summary>
		private const string TAG_SETTING_ASP_URL= "@@ AspUrl @@";
		/// <summary>Tag replace setting: amount</summary>
		private const string TAG_SETTING_AMOUNT = "@@ Amount @@";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paygentApiResult">レスポンス結果</param>
		public BanknetOrderRegisterResult(PaygentApiResult paygentApiResult)
		{
			this.Response = (BanknetOrderRegisterResponseDataset)paygentApiResult.Response;
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
				Constants.PHYSICALDIRPATH_FRONT_PC,
				Constants.PATH_XML_PAYGENT_BANKNET_PAYGENT);
			var message = XDocument.Load(filePath)
				.Descendants()
				.First(element => element.Name.ToString() == "Message");

			var htmlMessage = message.Elements().First(element => element.Name.ToString() == "Html").Value
				.Replace(TAG_SETTING_ASP_URL, this.Response.AspUrl)
				.Replace(TAG_SETTING_AMOUNT, StringUtility.ToNumeric(cart.PriceTotal.ToPriceString()))
				.Replace(TAG_SETTING_PAYMENT_LIMIT_DATE,
					DateTimeUtility.ToString(
						DateTime.ParseExact(this.Response.AspPaymentLimitDate, Constants.DATE_FORMAT_LONG, null),
						DateTimeUtility.FormatType.LongDate2Letter,
						cart.Owner.DispLanguageLocaleId));

			var textMessage = message.Elements().First(element => element.Name.ToString() == "Text").Value
				.Replace(TAG_SETTING_ASP_URL, this.Response.AspUrl)
				.Replace(TAG_SETTING_AMOUNT, StringUtility.ToNumeric(cart.PriceTotal.ToPriceString()))
				.Replace(TAG_SETTING_PAYMENT_LIMIT_DATE,
					DateTimeUtility.ToString(
						DateTime.ParseExact(this.Response.AspPaymentLimitDate, Constants.DATE_FORMAT_LONG, null),
						DateTimeUtility.FormatType.ShortDateHourMinute2Letter,
						cart.Owner.DispLanguageLocaleId));

			return (Html: htmlMessage, Text: textMessage);
		}

		/// <summary>成功したか</summary>
		public bool IsSuccess { get; private set; }
		/// <summary>レスポンス</summary>
		public BanknetOrderRegisterResponseDataset Response { get; private set; }
		/// <summary>結果理由</summary>
		public string ReasonPhrase { get; private set; }
	}
}
