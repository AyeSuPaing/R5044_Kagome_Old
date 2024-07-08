/*
=========================================================================================================
  Module      : ソフトバンクペイメント クレジット「情報登録要求」APIクラス(PaymentSBPSCreditCustomerRegistApi.cs)
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
	/// ソフトバンクペイメント クレジット「情報登録要求」APIクラス
	/// </summary>
	public class PaymentSBPSCreditCustomerRegistWithTokenApi : PaymentSBPSBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentSBPSCreditCustomerRegistWithTokenApi()
			: this(PaymentSBPSSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		public PaymentSBPSCreditCustomerRegistWithTokenApi(
			PaymentSBPSSetting settings)
			: base(settings, new PaymentSBPSCreditCustomerRegistResponseData(settings))
		{
		}

		/// <summary>
		/// 顧客情報 新規登録 実行
		/// </summary>
		/// <param name="custCode">顧客ID（決済情報保管時の紐付けキー）</param>
		/// <param name="creditTokenInfo">トークン情報</param>
		/// <returns>実行結果</returns>
		public bool Exec(
			string custCode,
			CartPayment.CreditTokenInfoBase creditTokenInfo)
		{
			// XML生成
			var requestXml = CreateRegistXml(
				custCode,
				(CartPayment.CreditTokenInfoSbps)creditTokenInfo);
			// 実行
			return Post(requestXml);
		}

		/// <summary>
		/// 顧客情報 新規登録 XML作成
		/// </summary>
		/// <param name="custCode">顧客ID（決済情報保管時の紐付けキー）</param>
		/// <param name="creditTokenInfo">トークン情報</param>
		/// <returns>顧客情報新規登録XML</returns>
		private XDocument CreateRegistXml(
			string custCode,
			CartPayment.CreditTokenInfoSbps creditTokenInfo)
		{
			XDocument document = new XDocument(new XDeclaration("1.0", "Shift_JIS", ""));
			document.Add(
				new XElement("sps-api-request", new XAttribute("id", "MG02-00131-101"),
					new XElement("merchant_id", this.HashCalculator.Add(this.Settings.MerchantId)),
					new XElement("service_id", this.HashCalculator.Add(this.Settings.ServiceId)),
					new XElement("cust_code", this.HashCalculator.Add(custCode)),
					//new XElement("ps_cust_info_return_flg", this.HashCalculator.Add("0")),
					CreatePayOptionManage(creditTokenInfo.Token, creditTokenInfo.TokenKey),
					new XElement("encrypted_flg", this.HashCalculator.Add("1")),
					new XElement("request_date", this.HashCalculator.Add(DateTime.Now.ToString("yyyyMMddHHmmss"))),
					//new XElement("limit_second", this.HashCalculator.Add("")),	// リクエスト時の許容時間（省略の場合は事前設定値を適用）
					new XElement("sps_hashcode", this.HashCalculator.ComputeHashSHA1AndClearBuffer())	// チェックサム
			));

			return document;
		}

		/// <summary>
		/// リアル与信API「pay_method_info」エレメント作成
		/// </summary>
		/// <param name="token">トークン</param>
		/// <param name="tokenKey">トークンキー</param>
		/// <returns>pay_method_info</returns>
		private XElement CreatePayOptionManage(
			string token,
			string tokenKey)
		{
			return new XElement("pay_option_manage",
				new XElement("token", this.HashCalculator.Add(token)),
				new XElement("token_key", this.HashCalculator.Add(tokenKey)));
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentSBPSCreditCustomerRegistResponseData ResponseData { get { return (PaymentSBPSCreditCustomerRegistResponseData)this.ResponseDataInner; } }
	}
}
