/*
=========================================================================================================
  Module      : ヤマトKWC ファミリーマート決済APIレスポンスデータ(PaymentYamatoKwcCvs2AuthResponseData.cs)
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
	/// ヤマトKWC ファミリーマート決済APIレスポンスデータ
	/// </summary>
	public class PaymentYamatoKwcCvs2AuthResponseData : PaymentYamatoKwcResponseDataBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		public PaymentYamatoKwcCvs2AuthResponseData(string responseString)
			: base(responseString)
		{
			this.CvsMessageDiv = "B02";
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
					case "companyCode":
						this.CompanyCode = element.Value;
						break;

					case "orderNoF":
						this.OrderNoF = element.Value;
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
					.Replace("@@ CompanyCode @@", this.CompanyCode)
					.Replace("@@ OrderNoF @@", this.OrderNoF)
					.Replace("@@ Amount @@", amountPrice)
					.Replace("@@ ExpiredDate @@",
						DateTimeUtility.ToString(
							this.ExpiredDate,
							DateTimeUtility.FormatType.ShortDate2Letter,
							languageLocaleId)),
				elem.Elements().First(e => e.Name == "Text").Value
					.Replace("@@ CompanyCode @@", this.CompanyCode)
					.Replace("@@ OrderNoF @@", this.OrderNoF)
					.Replace("@@ Amount @@", amountPrice)
					.Replace("@@ ExpiredDate @@",
						DateTimeUtility.ToString(
							this.ExpiredDate,
							DateTimeUtility.FormatType.ShortDate2Letter,
							languageLocaleId))
				);
			return result;
		}

		/// <summary>企業コード</summary>
		public string CompanyCode { get; private set; }
		/// <summary>注文番号</summary>
		public string OrderNoF { get; private set; }
		/// <summary>支払期限日</summary>
		public DateTime ExpiredDate { get; private set; }
		/// <summary>コンビニメッセージ区分</summary>
		public string CvsMessageDiv { get; private set; }

	}
}
