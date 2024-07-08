﻿/*
=========================================================================================================
  Module      : SBPS ドコモケータイ払い「継続課金（定期・従量）解約要求処理」APIクラス(PaymentSBPSCareerDocomoKetaiContinuousCancelApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Xml.Linq;

namespace w2.App.Common.Order
{
	/// <summary>
	/// SBPS ドコモケータイ払い「継続課金（定期・従量）解約要求処理」APIクラス
	/// </summary>
	public class PaymentSBPSCareerDocomoKetaiContinuousCancelApi : PaymentSBPSBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentSBPSCareerDocomoKetaiContinuousCancelApi()
			: this(PaymentSBPSSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		public PaymentSBPSCareerDocomoKetaiContinuousCancelApi(PaymentSBPSSetting settings)
			: base(settings, new PaymentSBPSCareerDocomoKetaiContinuousCancelResponseData(settings))
		{
		}

		/// <summary>
		/// 継続課金（定期・従量）解約連携実行
		/// </summary>
		/// <param name="trackingId">トラッキングID</param>
		/// <returns>実行結果</returns>
		public bool Exec(string trackingId)
		{
			// XML作成
			var requestXml = CreateCancelXml(trackingId);

			// 実行
			return Post(requestXml);
		}

		/// <summary>
		/// 継続課金（定期・従量）解約連携XML作成
		/// </summary>
		/// <param name="trackingId">処理対象トラッキングID</param>
		/// <returns>コミットXML</returns>
		private XDocument CreateCancelXml(string trackingId)
		{
			// XML作成
			var document = new XDocument(new XDeclaration("1.0", "Shift_JIS", ""));
			document.Add(
				new XElement("sps-api-request", new XAttribute("id", "ST02-00309-401"),
					new XElement("merchant_id", this.HashCalculator.Add(this.Settings.MerchantId)),
					new XElement("service_id", this.HashCalculator.Add(this.Settings.ServiceId)),
					new XElement("tracking_id", this.HashCalculator.Add(trackingId)),
					new XElement("request_date", this.HashCalculator.Add(DateTime.Now.ToString("yyyyMMddHHmmss"))),
					new XElement("sps_hashcode", this.HashCalculator.ComputeHashSHA1AndClearBuffer())	// チェックサム
			));

			return document;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentSBPSCareerDocomoKetaiContinuousCancelResponseData ResponseData
		{
			get { return (PaymentSBPSCareerDocomoKetaiContinuousCancelResponseData)this.ResponseDataInner; }
		}
	}
}