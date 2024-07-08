/*
=========================================================================================================
  Module      : Paygent API Paidy売上電文 Resultクラス(PaidySettlementResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order.Payment.Paygent.Foundation;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.Settlement.Dto
{
	/// <summary>
	/// Paygent API Paidy売上電文 Resultクラス
	/// </summary>
	[Serializable]
	public class PaidySettlementResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paygentApiResult">レスポンス結果</param>
		public PaidySettlementResult(PaygentApiResult paygentApiResult)
		{
			this.Response = (PaidySettlementResponseDataset)paygentApiResult.Response;
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
		public PaidySettlementResponseDataset Response { get; private set; }
		/// <summary>結果理由</summary>
		public string ReasonPhrase { get; private set; }
	}
}

