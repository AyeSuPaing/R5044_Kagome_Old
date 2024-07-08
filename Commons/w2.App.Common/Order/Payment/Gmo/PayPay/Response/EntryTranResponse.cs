/*
=========================================================================================================
  Module      : 決済登録結果 (EntryTranResponse.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Paypay.Response
{
	/// <summary>
	/// 決済登録結果
	/// </summary>
	public class EntryTranResponse : PaypayGmoResponse
	{
		/// <summary>取引ID</summary>
		[JsonProperty("accessID")]
		public string AccessId { get; set; }
		/// <summary>取引パスワード</summary>
		[JsonProperty("accessPass")]
		public string AccessPassword { get; set; }
	}
}
