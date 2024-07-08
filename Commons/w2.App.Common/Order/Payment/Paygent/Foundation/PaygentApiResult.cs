/*
=========================================================================================================
  Module      : Paygent API Resultクラス(PaygentApiResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order.Payment.Paygent.Foundation
{
	/// <summary>
	/// Paygent API Resultクラス
	/// </summary>
	[Serializable]
	public class PaygentApiResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reasonPhrase">理由</param>
		public PaygentApiResult(string reasonPhrase)
		{
			this.ReasonPhrase = reasonPhrase;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="response">レスポンス</param>
		/// <param name="reasonPhrase">理由</param>
		public PaygentApiResult(IPaygentResponse response, string reasonPhrase)
		{
			this.Response = response;
			this.ReasonPhrase = reasonPhrase;
		}

		/// <summary>
		/// 成功したか
		/// </summary>
		/// <returns>成功：true、失敗：false</returns>
		public bool IsSuccess()
		{
			return (this.Response != null)
				&& (this.Response.Result == PaygentConstants.FLG_PAYGENT_API_RESPONSE_RESULT_NORMAL);
		}

		/// <summary>レスポンス</summary>
		public IPaygentResponse Response { get; private set; }
		/// <summary>理由</summary>
		public string ReasonPhrase { get; private set; }
	}
}
