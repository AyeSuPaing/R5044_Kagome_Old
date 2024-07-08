/*
=========================================================================================================
  Module      : ソフトバンクペイメント クレジット「コミット」APIクラス(PaymentSBPSCreditCommitApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using w2.Common.Logger;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント クレジット「コミット」APIクラス
	/// </summary>
	public class PaymentSBPSCreditCommitApi : PaymentSBPSBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentSBPSCreditCommitApi()
			: this(PaymentSBPSSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		public PaymentSBPSCreditCommitApi(
			PaymentSBPSSetting settings)
			: base(settings, new PaymentSBPSCreditCommitResponseData(settings))
		{
		}

		/// <summary>
		/// コミット実行
		/// </summary>
		/// <param name="trackingId">トラッキングID</param>
		/// <returns>実行結果</returns>
		public bool Exec(
			string trackingId)
		{
			// XML作成
			XDocument requestXml = CreateCommitXml(
				trackingId);

			// 実行
			return Post(requestXml);
		}

		/// <summary>
		/// コミットXML作成
		/// </summary>
		/// <param name="trackingId">処理対象トラッキングID</param>
		/// <returns>コミットXML</returns>
		private XDocument CreateCommitXml(
			string trackingId)
		{
			// XML作成
			XDocument document = new XDocument(new XDeclaration("1.0", "Shift_JIS", ""));
			document.Add(
				new XElement("sps-api-request", new XAttribute("id", "ST02-00101-101"),
					new XElement("merchant_id", this.HashCalculator.Add(this.Settings.MerchantId)),
					new XElement("service_id", this.HashCalculator.Add(this.Settings.ServiceId)),
					//new XElement("sps_transaction_id", this.HashCalculator.Add(spsTransactionId)),
					new XElement("tracking_id", this.HashCalculator.Add(trackingId)),
					new XElement("processing_datetime", this.HashCalculator.Add(DateTime.Now.ToString("yyyyMMddHHmmss"))),
					new XElement("request_date", this.HashCalculator.Add(DateTime.Now.ToString("yyyyMMddHHmmss"))),
					//new XElement("imit_second", this.HashCalculator.Add(DateTime.Now.ToString("600"))),
					new XElement("sps_hashcode", this.HashCalculator.ComputeHashSHA1AndClearBuffer())	// チェックサム
			));

			return document;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentSBPSCreditCommitResponseData ResponseData { get { return (PaymentSBPSCreditCommitResponseData)this.ResponseDataInner; } }
	}
}
