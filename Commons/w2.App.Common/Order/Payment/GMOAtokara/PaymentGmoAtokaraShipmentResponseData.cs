/*
=========================================================================================================
  Module      : GMOアトカラ 出荷報告 API レスポンスデータ(PaymentGmoAtokaraShipmentResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Linq;

namespace w2.App.Common.Order.Payment.GMOAtokara
{
	/// <summary>
	/// GMOアトカラ 出荷報告 API レスポンスデータ
	/// </summary>
	public class PaymentGmoAtokaraShipmentResponseData : PaymentGmoAtokaraBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal PaymentGmoAtokaraShipmentResponseData()
			: base()
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
			foreach (var element in responseXml.Root.Elements())
			{
				switch (element.Name.ToString())
				{
					case "transactionInfo":
						foreach (var paymentElement in element.Elements())
						{
							switch (paymentElement.Name.ToString())
							{
								case "gmoTransactionId":
									this.GmoTransactionId = paymentElement.Value;
									break;
							}
						}
						break;

					case "errors":
						foreach (var childElement in element.Elements())
						{
							switch (childElement.Name.ToString())
							{
								case "error":
									this.Errors = LoadXmlErrorsElement(childElement);
									break;
							}
						}
						break;
				}
			}
		}

		/// <summary>GMO取引ID</summary>
		public string GmoTransactionId { get; private set; }
	}
}
