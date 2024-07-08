/*
=========================================================================================================
  Module      : ヤマトKWC セブンイレブン決済APIレスポンスデータ(PaymentYamatoKwcCvs1AuthResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Xml.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Util;
using w2.Common.Util;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC セブンイレブン決済APIレスポンスデータ
	/// </summary>
	public class PaymentYamatoKwcCvs1AuthResponseData : PaymentYamatoKwcResponseDataBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		public PaymentYamatoKwcCvs1AuthResponseData(string responseString)
			: base(responseString)
		{
			this.CvsMessageDiv = "B01";
		}

		/// <summary>
		/// レスポンスをプロパティへ格納
		/// </summary>
		/// <param name="responseXml">レスポンスXML</param>
		public override void SetPropertyFromXml(XDocument responseXml)
		{
			// 基底クラスのメソッド呼び出し
			base.SetPropertyFromXml(responseXml);

			// 固有の値をセット
			foreach (var element in responseXml.Root.Elements())
			{
				switch (element.Name.ToString())
				{
					case "billingNo":
						this.BillingNo = element.Value;
						break;

					case "billingUrl":
						this.BillingUrl = element.Value;
						break;

					case "expiredDate":
						this.ExpiredDate = DateTime.ParseExact(element.Value, "yyyyMMdd", null);
						break;
				}
			}
		}

		/// <summary>
		/// コンビニメッセージ取得
		/// </summary>
		/// <param name="cvsMessage">コンビニメッセージ</param>
		/// <param name="amount">支払金額</param>
		/// <param name="languageLocaleId">注文者の言語ロケールID</param>
		/// <returns>Html、Textの順に格納</returns>
		public Tuple<string, string> GetCvsMessage(XDocument cvsMessage, decimal amount, string languageLocaleId = "")
		{
			var elem = cvsMessage.Root.Elements().First(e => e.Name == this.CvsMessageDiv);

			// 日本通貨の場合は３桁カンマ区切り
			var amountPrice = (CurrencyManager.IsJapanKeyCurrencyCode) ? StringUtility.ToNumeric(amount.ToPriceString()) : amount.ToPriceString();

			var result = new Tuple<string, string>(
				elem.Elements().First(e => e.Name == "Html").Value
					.Replace("@@ BillingNo @@", this.BillingNo)
					.Replace("@@ Amount @@", amountPrice)
					.Replace("@@ ExpiredDate @@",
						DateTimeUtility.ToString(
							this.ExpiredDate,
							DateTimeUtility.FormatType.ShortDate2Letter,
							languageLocaleId))
					.Replace("@@ BillingUrl @@", this.BillingUrl),
				elem.Elements().First(e => e.Name == "Text").Value
					.Replace("@@ BillingNo @@", this.BillingNo)
					.Replace("@@ Amount @@", amountPrice)
					.Replace("@@ ExpiredDate @@",
						DateTimeUtility.ToString(
							this.ExpiredDate,
							DateTimeUtility.FormatType.ShortDate2Letter,
							languageLocaleId))
					.Replace("@@ BillingUrl @@", this.BillingUrl)
				);
			return result;
		}

		/// <summary>払込票番号</summary>
		public string BillingNo { get; private set; }
		/// <summary>払込票URL</summary>
		public string BillingUrl { get; private set; }
		/// <summary>支払期限日</summary>
		public DateTime ExpiredDate { get; private set; }
		/// <summary>コンビニメッセージ区分</summary>
		public string CvsMessageDiv { get; private set; }

	}
}
