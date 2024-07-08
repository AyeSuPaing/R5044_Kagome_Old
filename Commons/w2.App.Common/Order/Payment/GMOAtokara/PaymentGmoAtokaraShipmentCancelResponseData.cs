/*
=========================================================================================================
  Module      : GMOアトカラ 出荷報告キャンセル API レスポンスデータ(PaymentGmoAtokaraShipmentCancelResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Linq;

namespace w2.App.Common.Order.Payment.GMOAtokara
{
	/// <summary>
	/// GMOアトカラ 出荷報告キャンセル API レスポンスデータ
	/// </summary>
	public class PaymentGmoAtokaraShipmentCancelResponseData : PaymentGmoAtokaraBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal PaymentGmoAtokaraShipmentCancelResponseData()
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
					case "transactionResult":
						foreach (var paymentElement in element.Elements())
						{
							switch (paymentElement.Name.ToString())
							{
								case "gmoTransactionId":
									this.TransactionResult.GmoTransactionId = paymentElement.Value;
									break;

								case "authorResult":
									this.TransactionResult.AuthorResult = paymentElement.Value;
									break;
							}
						}
						break;
				}
			}
		}

		/// <summary>取得結果</summary>
		public TransactionResultElement TransactionResult { get; private set; }

		public class TransactionResultElement
		{
			/// <summary>GMO取引ID</summary>
			public string GmoTransactionId { get; set; }
			/// <summary>自動審査結果</summary>
			public string AuthorResult { get; set; }
		}
	}
}
