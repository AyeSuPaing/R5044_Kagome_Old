/*
=========================================================================================================
  Module      : ソフトバンクペイメント クレジット「顧客情報 参照」API レスポンスデータ(PaymentSBPSCreditCustomerReferResponseData.cs)
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
	/// ソフトバンクペイメント クレジット「顧客情報 参照」API レスポンスデータ
	/// </summary>
	public class PaymentSBPSCreditCustomerReferResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSCreditCustomerReferResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}

		/// <summary>
		/// レスポンスをプロパティへ格納
		/// </summary>
		/// <param name="responseXml">レスポンスXML</param>
		public override void LoadXml(XDocument responseXml)
		{
			// 基底クラスのメソッド呼び出し
			base.LoadXml(responseXml);

			// クレジット固有の値をセット
			foreach (XElement element in responseXml.Root.Elements())
			{
				switch (element.Name.ToString())
				{
					case "res_pay_method_info":
						foreach (XElement paymentElement in element.Elements())
						{
							switch (paymentElement.Name.ToString())
							{
								case "cc_number":
									this.CcNumber = GetDecryptedData(paymentElement.Value);
									break;

								case "cc_expiration":
									this.CcExpiration = GetDecryptedData(paymentElement.Value);
									break;

								case "cardbrand_code":
									this.CardbrandCode = GetDecryptedData(paymentElement.Value);
									break;
							}
						}
						break;
				}
			}
		}

		/// <summary>クレジットカードNO</summary>
		public string CcNumber { get; private set; }
		/// <summary>クレジットカード有効期限</summary>
		public string CcExpiration { get; private set; }
		/// <summary>クレジットカード有効期限(年4桁)</summary>
		public string CcExpirationYear
		{
			get { return this.CcExpiration.Substring(0, 4); }
		}
		/// <summary>クレジットカード有効期限(月)</summary>
		public string CcExpirationMonth
		{
			get { return this.CcExpiration.Substring(4, 2); }
		}
		/// <summary>クレジットカードブランドコード</summary>
		public string CardbrandCode { get; set; }
	}
}
