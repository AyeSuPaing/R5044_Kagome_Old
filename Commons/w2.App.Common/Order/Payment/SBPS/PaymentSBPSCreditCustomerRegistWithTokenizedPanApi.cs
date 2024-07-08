/*
=========================================================================================================
  Module      : ソフトバンクペイメント クレジット「情報登録要求（永久トークン利用）」APIクラス(PaymentSBPSCreditCustomerRegistWithTokenizedPanApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
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
	/// ソフトバンクペイメント クレジット「情報登録要求（永久トークン利用）」APIクラス
	/// </summary>
	public class PaymentSBPSCreditCustomerRegistWithTokenizedPanApi : PaymentSBPSBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentSBPSCreditCustomerRegistWithTokenizedPanApi()
			: this(PaymentSBPSSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		public PaymentSBPSCreditCustomerRegistWithTokenizedPanApi(
			PaymentSBPSSetting settings)
			: base(settings, new PaymentSBPSCreditCustomerRegistResponseData(settings))
		{
		}

		/// <summary>
		/// 顧客情報 新規登録 実行
		/// </summary>
		/// <param name="custCode">顧客ID（決済情報保管時の紐付けキー）</param>
		/// <param name="tokenizedPan">永久トークン</param>
		/// <param name="cardExpYear">カード有効期限(年2or4桁)</param>
		/// <param name="cardExpMonth">カード有効期限(月)</param>
		/// <returns>実行結果</returns>
		public bool Exec(
			string custCode,
			string tokenizedPan,
			string cardExpYear,
			string cardExpMonth)
		{
			var requestXml = CreateRegistXml(
				custCode,
				tokenizedPan,
				cardExpYear,
				cardExpMonth);
			return Post(requestXml);
		}

		/// <summary>
		/// 顧客情報 新規登録 XML作成
		/// </summary>
		/// <param name="custCode">顧客ID（決済情報保管時の紐付けキー）</param>
		/// <param name="tokenizedPan">永久トークン</param>
		/// <param name="cardExpYear">カード有効期限(年2or4桁)</param>
		/// <param name="cardExpMonth">カード有効期限(月)</param>
		/// <returns>顧客情報新規登録XML</returns>
		private XDocument CreateRegistXml(
			string custCode,
			string tokenizedPan,
			string cardExpYear,
			string cardExpMonth)
		{
			string year = ((cardExpYear.Length == 2) ? "20" : "") + cardExpYear;
			string month = cardExpMonth.PadLeft(2, '0');

			// XML作成
			var document = new XDocument(new XDeclaration("1.0", "Shift_JIS", ""));
			document.Add(
				new XElement("sps-api-request", new XAttribute("id", "MG12-00107-101"),
					new XElement("merchant_id", this.HashCalculator.Add(this.Settings.MerchantId)),
					new XElement("service_id", this.HashCalculator.Add(this.Settings.ServiceId)),
					new XElement("cust_code", this.HashCalculator.Add(custCode)),
					//new XElement("ps_cust_info_return_flg", this.HashCalculator.Add("0")),
					new XElement("pay_method_info",
						new XElement("tokenized_pan", GetEncryptedData(this.HashCalculator.Add(tokenizedPan))),
						new XElement("cc_expiration", GetEncryptedData(this.HashCalculator.Add(year + month)))),
						//new XElement("security_code", GetEncryptedData(this.HashCalculator.Add(cardSecurityCode)))),
					new XElement("encrypted_flg", this.HashCalculator.Add("1")),
					new XElement("request_date", this.HashCalculator.Add(DateTime.Now.ToString("yyyyMMddHHmmss"))),
					//new XElement("limit_second", this.HashCalculator.Add("")),	// リクエスト時の許容時間（省略の場合は事前設定値を適用）
					new XElement("sps_hashcode", this.HashCalculator.ComputeHashSHA1AndClearBuffer())	// チェックサム
			));

			return document;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentSBPSCreditCustomerRegistResponseData ResponseData { get { return (PaymentSBPSCreditCustomerRegistResponseData)this.ResponseDataInner; } }
	}
}
