/*
=========================================================================================================
  Module      : Paygent API Paidy返金電文 モジュール(PaidyRefund.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.Paygent.Foundation;
using w2.App.Common.Order.Payment.Paygent.Paidy.Refund.Dto;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.Refund
{
	/// <summary>
	/// Paygent API Paidy返金電文 モジュール
	/// </summary>
	public class PaidyRefund
	{
		/// <summary>Paidyオーソリキャンセル電文</summary>
		private const string PAYGENT_API_NAME = "Paidy返金電文";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paymentId">決済ID</param>
		/// <param name="amount">返金額</param>
		public PaidyRefund(
			string paymentId,
			decimal amount)
		{
			var postBody = new PaidyRefundPostBody(paymentId, amount);
			this.PostBody = postBody;
		}

		/// <summary>
		/// Paidy返金電文 実行
		/// </summary>
		/// <returns>結果</returns>
		public PaidyRefundResult Refund()
		{
			var response = new PaygentApiService(
					PAYGENT_API_NAME,
					this.PostBody.GenerateKeyValues())
				.GetResult<PaidyRefundResponseDataset>();
			var result = new PaidyRefundResult(response);
			return result;
		}

		/// <summary>リクエストボディ</summary>
		public PaidyRefundPostBody PostBody { get; private set; }
	}
}
