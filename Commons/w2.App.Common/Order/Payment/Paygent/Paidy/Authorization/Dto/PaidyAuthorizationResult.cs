/*
=========================================================================================================
  Module      : Paygent API Paidy決済情報検証電文 Resultクラス(PaidyAuthorizationResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order.Payment.Paygent.Foundation;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.Authorization.Dto
{
	/// <summary>
	/// Paygent API PaidyPaidy決済情報検証電文 Resultクラス
	/// </summary>
	[Serializable]
	public class PaidyAuthorizationResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paygentApiResult">レスポンス結果</param>
		public PaidyAuthorizationResult(PaygentApiResult paygentApiResult)
		{
			this.Response = (PaidyAuthorizationResponseDataset)paygentApiResult.Response;
			this.ReasonPhrase = paygentApiResult.ReasonPhrase;
			this.IsSuccess = paygentApiResult.IsSuccess();
		}

		/// <summary>成功したか</summary>
		public bool IsSuccess { get; private set; }
		/// <summary>レスポンス</summary>
		public PaidyAuthorizationResponseDataset Response { get; private set; }
		/// <summary>結果理由</summary>
		public string ReasonPhrase { get; private set; }
	}
}

