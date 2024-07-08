/*
=========================================================================================================
  Module      : ソフトバンクペイメント クレジット「リアル与信」API レスポンスデータ(PaymentSBPSCreditAuthResponseData.cs)
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
	/// ソフトバンクペイメント クレジット「リアル与信」API レスポンスデータ
	/// </summary>
	public class PaymentSBPSCreditAuthResponseData : PaymentSBPSBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		internal PaymentSBPSCreditAuthResponseData(PaymentSBPSSetting settings)
			: base(settings)
		{
		}

		/// <summary>
		/// レスポンスをプロパティへ格納(クレジット用にオーバーライド）
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
								case "cc_company_code":
									this.CcCompanyCode = GetDecryptedData(paymentElement.Value);
									break;

								case "recognized_no":
									this.RecognizedNo = GetDecryptedData(paymentElement.Value);
									break;
							}
						}
						break;
				}
			}
		}

		/// <summary>クレジット会社コード</summary>
		public string CcCompanyCode { get; private set; }
		/// <summary>承認番号</summary>
		public string RecognizedNo { get; private set; }
		/// <summary>処理SBPS 顧客ID</summary>
		public string ResSpsCustNo { get; private set; }
		/// <summary>処理SBPS 支払方法管理番号</summary>
		public string ResSpsPaymentNo { get; private set; }
	}
}
