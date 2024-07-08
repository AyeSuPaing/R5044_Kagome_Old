/*
=========================================================================================================
  Module      : Paygent API レスポンスインターフェース(IPaygentResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Paygent.Foundation
{
	/// <summary>
	/// Paygent API レスポンスインターフェース
	/// </summary>
	public interface IPaygentResponse
	{
		/// <summary>処理結果</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_RESULT)]
		string Result { get; set; }
	}
}
