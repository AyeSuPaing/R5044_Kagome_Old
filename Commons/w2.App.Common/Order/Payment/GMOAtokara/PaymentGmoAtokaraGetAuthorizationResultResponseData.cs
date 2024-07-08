/*
=========================================================================================================
  Module      : GMOアトカラ 与信審査結果取得 API レスポンスデータ(PaymentGmoAtokaraGetAuthorizationResultResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Linq;

namespace w2.App.Common.Order.Payment.GMOAtokara
{
	/// <summary>
	/// GMOアトカラ 与信審査結果取得 API レスポンスデータ
	/// </summary>
	public class PaymentGmoAtokaraGetAuthorizationResultResponseData : PaymentGmoAtokaraBaseResponseData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal PaymentGmoAtokaraGetAuthorizationResultResponseData()
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

								case "autoAuthorResult":
									this.TransactionResult.AuthAuthorResult = paymentElement.Value;
									break;

								case "maulAuthorResult":
									this.TransactionResult.MaulAuthorResult = paymentElement.Value;
									break;

								case "reasons":
									foreach (var reasonElement in paymentElement.Elements())
									{
										switch (reasonElement.Name.ToString())
										{
											case "reason":
												this.TransactionResult.Reason = reasonElement.Value;
												break;
										}
									}
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
			/// <summary>自動審査結果</summary>
			public string AuthAuthorResult { get; set; }
			/// <summary>目視審査結果</summary>
			public string MaulAuthorResult { get; set; }
			/// <summary>目視審査結果理由</summary>
			public string Reason { get; set; }
		}
	}
}
