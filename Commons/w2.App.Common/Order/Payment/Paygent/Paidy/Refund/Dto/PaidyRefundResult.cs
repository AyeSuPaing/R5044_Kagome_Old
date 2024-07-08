/*
=========================================================================================================
  Module      : Paygent API Paidy返金電文 結果クラス(PaidyRefundResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order.Payment.Paygent.Foundation;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.Refund.Dto
{
	/// <summary>
	/// Paygent API Paidy返金電文 結果クラス
	/// </summary>
	[Serializable]
	public class PaidyRefundResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paygentApiResult">API実行結果</param>
		public PaidyRefundResult(PaygentApiResult paygentApiResult)
		{
			this.Response = (PaidyRefundResponseDataset)paygentApiResult.Response;
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
		/// <summary>レスポンスデータセット</summary>
		public PaidyRefundResponseDataset Response { get; private set; }
		/// <summary>結果理由</summary>
		public string ReasonPhrase { get; private set; }
	}
}

