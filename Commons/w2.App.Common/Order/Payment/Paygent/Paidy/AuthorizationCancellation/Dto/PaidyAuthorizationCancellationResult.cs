/*
=========================================================================================================
  Module      : Paygent API Paidyオーソリキャンセル電文 Resultクラス(PaidyAuthorizationCancellationResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order.Payment.Paygent.Foundation;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.AuthorizationCancellation.Dto
{
	/// <summary>
	/// Paygent API Paidyオーソリキャンセル電文 Resultクラス
	/// </summary>
	[Serializable]
	public class PaidyAuthorizationCancellationResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paygentApiResult">レスポンス結果</param>
		public PaidyAuthorizationCancellationResult(PaygentApiResult paygentApiResult)
		{
			this.Response = (PaidyAuthorizationCancellationResponseDataset)paygentApiResult.Response;
			this.ReasonPhrase = paygentApiResult.ReasonPhrase;
			this.IsSuccess = paygentApiResult.IsSuccess();
		}

		/// <summary>
		/// Get error message
		/// </summary>
		/// <returns>Error message</returns>
		public string GetErrorMessage()
		{
			var errorMessage = (this.Response != null)
				? this.Response.ResponseDetail
				: this.ReasonPhrase ?? string.Empty;
			return errorMessage;
		}

		/// <summary>成功したか</summary>
		public bool IsSuccess { get; private set; }
		/// <summary>レスポンス</summary>
		public PaidyAuthorizationCancellationResponseDataset Response { get; private set; }
		/// <summary>結果理由</summary>
		public string ReasonPhrase { get; private set; }
	}
}

