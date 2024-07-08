/*
=========================================================================================================
  Module      : Paygent CVS Order Register Result(CvsOrderRegisterResult.cs)
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
using w2.Domain.Payment;

namespace w2.App.Common.Order.Payment.Paygent.Cvs.Register.Dto
{
	/// <summary>
	/// Paygent Cvs order register result
	/// </summary>
	[Serializable]
	public class OrderRegisterResult
	{
		/// <summary>Tag replace setting: 決済ベンダ受付番号</summary>
		private const string TAG_SETTING_RECEIPT_NUMBER = "@@ ReceiptNumber @@";
		/// <summary>Tag replace setting: trading id</summary>
		private const string TAG_SETTING_TRADING_ID = "@@ TradingId @@";
		/// <summary>Tag replace setting: payment limit date</summary>
		private const string TAG_SETTING_PAYMENT_LIMIT_DATE = "@@ PaymentLimitDate @@";
		/// <summary>Tag replace setting: receipt print url</summary>
		private const string TAG_SETTING_RECEIPT_PRINT_URL = "@@ ReceiptPrintUrl @@";
		/// <summary>Tag replace setting: amount</summary>
		private const string TAG_SETTING_AMOUNT = "@@ Amount @@";
		/// <summary>Tag replace setting: payment service</summary>
		private const string TAG_SETTING_PAYMENTSERVICE_WAY_TO_PAY = "@@ PaymentService @@";
		/// <summary>Tag replace setting: 注文者電話番号</summary>
		private const string TAG_SETTING_OWNER_TEL = "@@ OwnerTel @@";
		/// <summary>PAYGENTリンク</summary>
		private const string PAYGENT_PAYMENT_SERVICE_URL = "https://www.paygent.co.jp/payment_service/way_to_pay/cvs/";
		/// <summary>コンビニエンスコード：デイリーヤマザキ</summary>
		private const string CONVENIENCE_CODE_DAILYYAMAZAKI = "00C014";
		/// <summary>コンビニエンスコード：セイコーマート</summary>
		private const string CONVENIENCE_CODE_SEICOMART = "P0C016";
		/// <summary>コンビニエンスコード：セブンイレブン</summary>
		private const string CONVENIENCE_CODE_SEVENELEVEN = "00C001";
		/// <summary>コンビニエンスコード：ファミリーマート</summary>
		private const string CONVENIENCE_CODE_FAMILYMART = "00C005";
		/// <summary>コンビニエンスコード：ローソン・ミニストップ</summary>
		private const string CONVENIENCE_CODE_LAWSONMINISTOP = "P0C002";
		/// <summary>コンビニエンス名：デイリーヤマザキ</summary>
		private const string CONVENIENCE_NAME_DAILYYAMAZAKI = "DailyYamazaki";
		/// <summary>コンビニエンス名：セイコーマート</summary>
		private const string CONVENIENCE_NAME_SEICOMART = "Seicomart";
		/// <summary>コンビニエンス名；セブンイレブン</summary>
		private const string CONVENIENCE_NAME_SEVENELEVEN = "SevenEleven";
		/// <summary>コンビニエンス名：ファミリーマート</summary>
		private const string CONVENIENCE_NAME_FAMILYMART = "FamilyMart";
		/// <summary>コンビニエンス名：ローソン・ミニストップ</summary>
		private const string CONVENIENCE_NAME_LAWSONMINISTOP = "LawsonMinistop";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paygentApiResult">レスポンス結果</param>
		public OrderRegisterResult(PaygentApiResult paygentApiResult)
		{
			this.Response = (OrderRegisterResponseDataset)paygentApiResult.Response;
			this.ReasonPhrase = paygentApiResult.ReasonPhrase;
			this.IsSuccess = paygentApiResult.IsSuccess();
		}

		/// <summary>
		/// Get error message
		/// </summary>
		/// <returns>Error message</returns>
		public string GetErrorMessage()
		{
			var errorMessage = this.Response != null
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
				Constants.PATH_XML_CVS_PAYGENT);
			var message = XDocument.Load(filePath)
				.Descendants()
				.First(element => element.Name.ToString() == GetCvsMessage(cart.Payment.GetPaygentCvsType()));

			var htmlMessage = message.Elements().First(element => element.Name.ToString() == "Html").Value
				.Replace(TAG_SETTING_RECEIPT_NUMBER, this.Response.ReceiptNumber)
				.Replace(TAG_SETTING_TRADING_ID, this.Response.TradingId)
				.Replace(TAG_SETTING_RECEIPT_PRINT_URL, this.Response.ReceiptPrintUrl)
				.Replace(TAG_SETTING_PAYMENTSERVICE_WAY_TO_PAY, PAYGENT_PAYMENT_SERVICE_URL)
				.Replace(TAG_SETTING_AMOUNT, StringUtility.ToNumeric(cart.SettlementAmount.ToPriceString()))
				.Replace(TAG_SETTING_OWNER_TEL, cart.Owner.Tel1)
				.Replace(TAG_SETTING_PAYMENT_LIMIT_DATE,
					DateTimeUtility.ToString(
						DateTime.ParseExact(this.Response.PaymentLimitDate, Constants.DATE_FORMAT_SHORT, null),
						DateTimeUtility.FormatType.ShortDate2Letter,
						cart.Owner.DispLanguageLocaleId));

			var textMessage = message.Elements().First(element => element.Name.ToString() == "Text").Value
				.Replace(TAG_SETTING_RECEIPT_NUMBER, this.Response.ReceiptNumber)
				.Replace(TAG_SETTING_TRADING_ID, this.Response.TradingId)
				.Replace(TAG_SETTING_RECEIPT_PRINT_URL, this.Response.ReceiptPrintUrl)
				.Replace(TAG_SETTING_PAYMENTSERVICE_WAY_TO_PAY, PAYGENT_PAYMENT_SERVICE_URL)
				.Replace(TAG_SETTING_AMOUNT, StringUtility.ToNumeric(cart.SettlementAmount.ToPriceString()))
				.Replace(TAG_SETTING_OWNER_TEL, cart.Owner.Tel1)
				.Replace(TAG_SETTING_PAYMENT_LIMIT_DATE,
					DateTimeUtility.ToString(
						DateTime.ParseExact(this.Response.PaymentLimitDate, Constants.DATE_FORMAT_SHORT, null),
						DateTimeUtility.FormatType.ShortDate2Letter,
						cart.Owner.DispLanguageLocaleId));

			return (Html: htmlMessage, Text: textMessage);
		}

		/// <summary>
		/// コンビニメッセージ区分取得
		/// </summary>
		/// <param name="convenienceCode">支払先コンビニコード</param>
		/// <returns>コンビニメッセージ区分</returns>
		public string GetCvsMessage(string convenienceCode)
		{
			switch (convenienceCode)
			{
				case CONVENIENCE_CODE_DAILYYAMAZAKI:
					// デイリーヤマザキ
					return CONVENIENCE_NAME_DAILYYAMAZAKI;

				case CONVENIENCE_CODE_SEICOMART:
					// セイコーマート
					return CONVENIENCE_NAME_SEICOMART;

				case CONVENIENCE_CODE_SEVENELEVEN:
					// セブンイレブン
					return CONVENIENCE_NAME_SEVENELEVEN;

				case CONVENIENCE_CODE_FAMILYMART:
					// ファミリーマート
					return CONVENIENCE_NAME_FAMILYMART;

				case CONVENIENCE_CODE_LAWSONMINISTOP:
					// ローソン・ミニストップ
					return CONVENIENCE_NAME_LAWSONMINISTOP;

				default:
					throw new ArgumentOutOfRangeException(nameof(convenienceCode), convenienceCode, null);
			}
		}

		/// <summary>成功したか</summary>
		public bool IsSuccess { get; private set; }
		/// <summary>レスポンス</summary>
		public OrderRegisterResponseDataset Response { get; private set; }
		/// <summary>結果理由</summary>
		public string ReasonPhrase { get; private set; }
	}
}
