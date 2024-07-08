/*
=========================================================================================================
  Module      : GMOアトカラ 取引変更・キャンセル API レスポンスデータ(PaymentGmoAtokaraCancelResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Linq;

namespace w2.App.Common.Order.Payment.GMOAtokara
{
	/// <summary>
	/// GMOアトカラ 取引変更・キャンセル API レスポンスデータ
	/// </summary>
	public class PaymentGmoAtokaraCancelResponseData : PaymentGmoAtokaraBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal PaymentGmoAtokaraCancelResponseData()
			: base()
		{
			this.TransactionResult = new TransactionResultElement();
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

		/// <summary>取得結果</summary>
		public TransactionResultElement TransactionResult { get; private set; }

		/// <summary>
		/// 取引結果エレメント
		/// </summary>
		public class TransactionResultElement
		{
			/// <summary>GMO取引ID</summary>
			public string GmoTransactionId { get; set; }
		}
	}
}
