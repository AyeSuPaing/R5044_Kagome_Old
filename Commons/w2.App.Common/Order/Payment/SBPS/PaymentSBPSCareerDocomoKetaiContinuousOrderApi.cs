﻿/*
=========================================================================================================
  Module      : SBPS ドコモケータイ払い「継続課金（定期・従量）購入要求処理」APIクラス(PaymentSBPSCareerDocomoKetaiContinuousOrderApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Xml.Linq;
using w2.App.Common.Extensions.Currency;

namespace w2.App.Common.Order
{
	/// <summary>
	/// SBPS ドコモケータイ払い「継続課金（定期・従量）購入要求処理」APIクラス
	/// </summary>
	public class PaymentSBPSCareerDocomoKetaiContinuousOrderApi : PaymentSBPSBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentSBPSCareerDocomoKetaiContinuousOrderApi()
			: this(PaymentSBPSSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		public PaymentSBPSCareerDocomoKetaiContinuousOrderApi(PaymentSBPSSetting settings)
			: base(settings, new PaymentSBPSCareerDocomoKetaiContinuousOrderResponseData(settings))
		{
		}

		/// <summary>
		/// 継続課金（定期・従量）購入要求実行
		/// </summary>
		/// <param name="trackingId">トラッキングID</param>
		/// <param name="custCode">顧客ID（決済情報保管時の紐付けキー）</param>
		/// <param name="sbpsOrderId">注文ID</param>
		/// <param name="itemId">商品検索用ID</param>
		/// <param name="itemName">商品表示名</param>
		/// <param name="amount">合計金額</param>
		/// <returns>実行結果</returns>
		public bool Exec(
			string trackingId,
			string custCode,
			string sbpsOrderId,
			string itemId,
			string itemName,
			decimal amount)
		{
			// XML作成
			var requestXml = CreateContinuousOrderXml(
				trackingId,
				custCode,
				sbpsOrderId,
				itemId,
				itemName,
				amount);

			// 実行
			return this.Post(requestXml);
		}

		/// <summary>
		/// 継続課金（定期・従量）購入要求XML作成
		/// </summary>
		/// <param name="trackingId">処理対象トラッキングID</param>
		/// <param name="custCode">顧客ID（決済情報保管時の紐付けキー）</param>
		/// <param name="sbpsOrderId">注文ID</param>
		/// <param name="itemId">商品検索用ID</param>
		/// <param name="itemName">商品表示名</param>
		/// <param name="amount">合計金額</param>
		/// <returns>コミットXML</returns>
		private XDocument CreateContinuousOrderXml(
			string trackingId,
			string custCode,
			string sbpsOrderId,
			string itemId,
			string itemName,
			decimal amount)
		{
			// XML作成
			XDocument document = new XDocument(new XDeclaration("1.0", "Shift_JIS", ""));
			document.Add(
				new XElement("sps-api-request", new XAttribute("id", "ST01-00104-401"),
					new XElement("merchant_id", this.HashCalculator.Add(this.Settings.MerchantId)),
					new XElement("service_id", this.HashCalculator.Add(this.Settings.ServiceId)),
					new XElement("tracking_id", this.HashCalculator.Add(trackingId)),
					new XElement("cust_code", this.HashCalculator.Add(custCode)),
					new XElement("order_id", this.HashCalculator.Add(sbpsOrderId)),
					new XElement("item_id", this.HashCalculator.Add(itemId)),
					new XElement("item_name", EncodeMultibyteToBase64(this.HashCalculator.Add(itemName))),
					new XElement("amount", this.HashCalculator.Add(amount.ToPriceString())),
					new XElement("request_date", this.HashCalculator.Add(DateTime.Now.ToString("yyyyMMddHHmmss"))),
					new XElement("sps_hashcode", this.HashCalculator.ComputeHashSHA1AndClearBuffer())	// チェックサム
				));

			return document;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentSBPSCareerDocomoKetaiContinuousOrderResponseData ResponseData
		{
			get { return (PaymentSBPSCareerDocomoKetaiContinuousOrderResponseData)this.ResponseDataInner; }
		}
	}
}