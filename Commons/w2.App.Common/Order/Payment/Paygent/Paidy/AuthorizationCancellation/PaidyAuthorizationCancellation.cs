/*
=========================================================================================================
  Module      : Paygent API Paidyオーソリキャンセル電文 モジュール(PaidyAuthorizationCancellation.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.Paygent.Foundation;
using w2.App.Common.Order.Payment.Paygent.Paidy.AuthorizationCancellation.Dto;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.AuthorizationCancellation
{
	/// <summary>
	/// Paygent API Paidyオーソリキャンセル電文 モジュール
	/// </summary>
	public class PaidyAuthorizationCancellation
	{
		/// <summary>Paidyオーソリキャンセル電文</summary>
		private const string PAYGENT_API_NAME = "Paidyオーソリキャンセル電文";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paymentId">決済ID</param>
		public PaidyAuthorizationCancellation(string paymentId)
		{
			var postBody = new PaidyAuthorizationCancellationPostBody(paymentId);
			this.PostBody = postBody;
		}

		/// <summary>
		/// Paidyオーソリキャンセル電文 実行
		/// </summary>
		/// <returns>結果</returns>
		public PaidyAuthorizationCancellationResult AuthorizeCancel()
		{
			var response = new PaygentApiService(
					PAYGENT_API_NAME,
					this.PostBody.GenerateKeyValues())
				.GetResult<PaidyAuthorizationCancellationResponseDataset>();
			var result = new PaidyAuthorizationCancellationResult(response);
			return result;
		}

		/// <summary>リクエストボディ</summary>
		public PaidyAuthorizationCancellationPostBody PostBody { get; private set; }
	}
}
