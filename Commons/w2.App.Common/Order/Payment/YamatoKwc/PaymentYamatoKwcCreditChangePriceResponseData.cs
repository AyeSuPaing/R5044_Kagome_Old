/*
=========================================================================================================
  Module      : ヤマトKWC クレジット金額変更APIレスポンスデータ(PaymentYamatoKwcCreditChangePriceResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC クレジット金額変更APIレスポンスデータ
	/// </summary>
	public class PaymentYamatoKwcCreditChangePriceResponseData : PaymentYamatoKwcResponseDataBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		public PaymentYamatoKwcCreditChangePriceResponseData(string responseString)
			: base(responseString)
		{
		}

		/// <summary>
		/// レスポンスをプロパティへ格納
		/// </summary>
		/// <param name="responseXml">レスポンスXML</param>
		public override void SetPropertyFromXml(XDocument responseXml)
		{
			// 基底クラスのメソッド呼び出し
			base.SetPropertyFromXml(responseXml);

			// 固有の値をセット
			foreach (var element in responseXml.Root.Elements())
			{
				switch (element.Name.ToString())
				{
					case "creditErrorCode":
						this.CreditErrorCode = element.Value;
						break;
				}
			}
		}

		/// <summary>与信結果エラーコード</summary>
		public string CreditErrorCode { get; private set; }
		/// <summary>与信結果エラーメッセージ</summary>
		public string CreditErrorMessage
		{
			get { return m_creditErrorMessage ?? (m_creditErrorMessage = GetCgMessage(this.CreditErrorCode)); }
		}
		private string m_creditErrorMessage = null;
	}
}
