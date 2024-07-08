/*
=========================================================================================================
  Module      : ソフトバンクペイメント ソフトバンク・ワイモバイルまとめて支払い「取消要求」APIクラス(PaymentSBPSCareerSoftbankKetaiCancelApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント マルチ決済 ソフトバンク・ワイモバイルまとめて支払い「取消要求」APIクラス
	/// </summary>
	public class PaymentSBPSCareerSoftbankKetaiCancelApi : PaymentSBPSBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentSBPSCareerSoftbankKetaiCancelApi()
			: this(PaymentSBPSSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		public PaymentSBPSCareerSoftbankKetaiCancelApi(
			PaymentSBPSSetting settings)
			: base(settings, new PaymentSBPSCareerSoftbankKetaiCancelResponseData(settings))
		{
		}

		/// <summary>
		/// 取消連携実行
		/// </summary>
		/// <param name="trackingId">トラッキングID</param>
		/// <returns>実行結果</returns>
		public bool Exec(
			string trackingId)
		{
			// XML作成
			XDocument requestXml = CreateSaleXml(
				trackingId);

			// 実行
			return this.Post(requestXml);
		}

		/// <summary>
		/// 取消連携XML作成
		/// </summary>
		/// <param name="trackingId">処理対象トラッキングID</param>
		/// <returns>コミットXML</returns>
		private XDocument CreateSaleXml(
			string trackingId)
		{
			// XML作成
			XDocument document = new XDocument(new XDeclaration("1.0", "Shift_JIS", ""));
			document.Add(
				new XElement("sps-api-request", new XAttribute("id", "ST02-00303-405"),
					new XElement("merchant_id", this.HashCalculator.Add(this.Settings.MerchantId)),
					new XElement("service_id", this.HashCalculator.Add(this.Settings.ServiceId)),
					//new XElement("sps_transaction_id", this.HashCalculator.Add(spsTransactionId)),
					new XElement("tracking_id", this.HashCalculator.Add(trackingId)),
					new XElement("request_date", this.HashCalculator.Add(DateTime.Now.ToString("yyyyMMddHHmmss"))),
					//new XElement("imit_second", this.HashCalculator.Add(DateTime.Now.ToString("600"))),
					new XElement("sps_hashcode", this.HashCalculator.ComputeHashSHA1AndClearBuffer())	// チェックサム
			));

			return document;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentSBPSCareerSoftbankKetaiCancelResponseData ResponseData { get { return (PaymentSBPSCareerSoftbankKetaiCancelResponseData)this.ResponseDataInner; } }
	}
}
