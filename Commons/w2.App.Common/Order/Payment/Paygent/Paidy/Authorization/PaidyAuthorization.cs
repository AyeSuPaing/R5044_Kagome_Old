/*
=========================================================================================================
  Module      : Paygent API Paidy決済情報検証電文モジュール(PaidyAuthorization.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.Paygent.Foundation;
using w2.App.Common.Order.Payment.Paygent.Paidy.Authorization.Dto;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.Authorization
{
	/// <summary>
	/// Paygent API Paidy決済情報検証 モジュール
	/// </summary>
	public class PaidyAuthorization
	{
		/// <summary>Paidy決済情報検証電文</summary>
		private const string PAYGENT_API_NAME = "Paidy決済情報検証電文";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paidyPaymentId">Paidy決済ID</param>
		public PaidyAuthorization(string paidyPaymentId)
		{
			var postBody = new PaidyAuthorizationPostBody(paidyPaymentId);
			this.PostBody = postBody;
		}

		/// <summary>
		/// Paidy決済情報検証電文 実行
		/// </summary>
		/// <returns>結果</returns>
		public PaidyAuthorizationResult Authorize()
		{
			var response = new PaygentApiService(
					PAYGENT_API_NAME,
					this.PostBody.GenerateKeyValues())
				.GetResult<PaidyAuthorizationResponseDataset>();
			var result = new PaidyAuthorizationResult(response);
			return result;
		}

		/// <summary>リクエストボディ</summary>
		public PaidyAuthorizationPostBody PostBody { get; private set; }
	}
}
