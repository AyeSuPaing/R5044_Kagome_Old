/*
=========================================================================================================
  Module      : ソフトバンクペイメント クレジット「顧客情報 参照」APIクラス(PaymentSBPSCreditCustomerReferApi.cs)
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
	/// ソフトバンクペイメント クレジット「顧客情報 参照」APIクラス
	/// </summary>
	public class PaymentSBPSCreditCustomerReferApi : PaymentSBPSBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentSBPSCreditCustomerReferApi()
			: this(PaymentSBPSSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		public PaymentSBPSCreditCustomerReferApi(
			PaymentSBPSSetting settings)
			: base(settings, new PaymentSBPSCreditCustomerReferResponseData(settings))
		{
		}

		/// <summary>
		/// 顧客情報 参照 実行
		/// </summary>
		/// <param name="custCode">顧客ID</param>
		/// <returns>実行結果</returns>
		public bool Exec(string custCode)
		{
			// XML作成
			XDocument requestXml = CreateReferXml(custCode);

			// 実行
			return Post(requestXml);
		}

		/// <summary>
		/// 顧客情報 参照 XML作成
		/// </summary>
		/// <param name="custCode">顧客ID</param>
		/// <returns>顧客情報参照XML</returns>
		private XDocument CreateReferXml(string custCode)
		{
			// XML作成
			XDocument document = new XDocument(new XDeclaration("1.0", "Shift_JIS", ""));
			document.Add(
				new XElement("sps-api-request", new XAttribute("id", "MG02-00104-101"),
					new XElement("merchant_id", this.HashCalculator.Add(this.Settings.MerchantId)),
					new XElement("service_id", this.HashCalculator.Add(this.Settings.ServiceId)),
					new XElement("cust_code", this.HashCalculator.Add(custCode)),
					new XElement("sps_cust_info_return_flg", this.HashCalculator.Add("1")),
					new XElement("response_info_type", this.HashCalculator.Add("2")),
					new XElement("pay_option_manage",
						new XElement("cardbrand_return_flg", this.HashCalculator.Add("1"))),
					new XElement("encrypted_flg", this.HashCalculator.Add("1")),
					new XElement("request_date", this.HashCalculator.Add(DateTime.Now.ToString("yyyyMMddHHmmss"))),
					//new XElement("limit_second", this.HashCalculator.Add("")),	// リクエスト時の許容時間（省略の場合は事前設定値を適用）
					new XElement("sps_hashcode", this.HashCalculator.ComputeHashSHA1AndClearBuffer())	// チェックサム
			));

			return document;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentSBPSCreditCustomerReferResponseData ResponseData { get { return (PaymentSBPSCreditCustomerReferResponseData)this.ResponseDataInner; } }
	}
}
