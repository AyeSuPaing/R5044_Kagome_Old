/*
=========================================================================================================
  Module      : ソフトバンクペイメント クレジット「API売上連携」クラス(PaymentSBPSCreditSaleApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Xml.Linq;
using w2.App.Common.Extensions.Currency;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント クレジット「API売上連携」クラス
	/// </summary>
	public class PaymentSBPSCreditSaleApi : PaymentSBPSBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentSBPSCreditSaleApi()
			: this(PaymentSBPSSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		public PaymentSBPSCreditSaleApi(
			PaymentSBPSSetting settings)
			: base(settings, new PaymentSBPSCreditSaleResponseData(settings))
		{
		}

		/// <summary>
		/// 売上連携実行
		/// </summary>
		/// <param name="trackingId">トラッキングID</param>
		/// <param name="amount">合計金額</param>
		/// <returns>実行結果</returns>
		public bool Exec(
			string trackingId,
			decimal amount)
		{
			// XML作成
			XDocument requestXml = CreateSaleXml(
				trackingId,
				amount);

			// 実行
			return this.Post(requestXml);
		}

		/// <summary>
		/// API売上連携XML作成
		/// </summary>
		/// <param name="trackingId">処理対象トラッキングID</param>
		/// <param name="amount">合計金額</param>
		/// <returns>コミットXML</returns>
		private XDocument CreateSaleXml(
			string trackingId,
			decimal amount)
		{
			// XML作成
			XDocument document = new XDocument(new XDeclaration("1.0", "Shift_JIS", ""));
			document.Add(
				new XElement("sps-api-request", new XAttribute("id", "ST02-00201-101"),
					new XElement("merchant_id", this.HashCalculator.Add(this.Settings.MerchantId)),
					new XElement("service_id", this.HashCalculator.Add(this.Settings.ServiceId)),
				//new XElement("sps_transaction_id", this.HashCalculator.Add(spsTransactionId)),
					new XElement("tracking_id", this.HashCalculator.Add(trackingId)),
					new XElement("processing_datetime", this.HashCalculator.Add(DateTime.Now.ToString("yyyyMMddHHmmss"))),
					new XElement("pay_option_manage",
						new XElement("amount", this.HashCalculator.Add(amount.ToPriceString()))
					),
					new XElement("request_date", this.HashCalculator.Add(DateTime.Now.ToString("yyyyMMddHHmmss"))),
				//new XElement("imit_second", this.HashCalculator.Add(DateTime.Now.ToString("600"))),
					new XElement("sps_hashcode", this.HashCalculator.ComputeHashSHA1AndClearBuffer())	// チェックサム
			));

			return document;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentSBPSCreditSaleResponseData ResponseData { get { return (PaymentSBPSCreditSaleResponseData)this.ResponseDataInner; } }
	}
}
