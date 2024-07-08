/*
=========================================================================================================
  Module      : ソフトバンクペイメント リクルートかんたん支払い「売上要求処理」APIクラス(PaymentSBPSRecruitSaleApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Xml.Linq;
using w2.App.Common.Extensions.Currency;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント マルチ決済 リクルートかんたん支払い「売上要求処理」APIクラス
	/// </summary>
	public class PaymentSBPSRecruitSaleApi : PaymentSBPSBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentSBPSRecruitSaleApi()
			: this(PaymentSBPSSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		public PaymentSBPSRecruitSaleApi(
			PaymentSBPSSetting settings)
			: base(settings, new PaymentSBPSRecruitSaleResponseData(settings))
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
				trackingId, amount);

			// 実行
			return this.Post(requestXml);
		}

		/// <summary>
		/// 売上連携XML作成
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
				new XElement("sps-api-request", new XAttribute("id", "ST02-00202-309"),
					new XElement("merchant_id", this.HashCalculator.Add(this.Settings.MerchantId)),
					new XElement("service_id", this.HashCalculator.Add(this.Settings.ServiceId)),
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
		public PaymentSBPSRecruitSaleResponseData ResponseData { get { return (PaymentSBPSRecruitSaleResponseData)this.ResponseDataInner; } }
	}
}
