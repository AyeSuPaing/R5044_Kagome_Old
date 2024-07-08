/*
=========================================================================================================
  Module      : 決済実行結果 (ExecTranResponse.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Paypay.Response
{
	/// <summary>
	/// 決済実行結果
	/// </summary>
	public class ExecTranResponse : PaypayGmoResponse
	{
		/// <summary>取引ID</summary>
		[JsonProperty("AccessID")]
		public string AccessId { get; set; }
		/// <summary>トークン</summary>
		[JsonProperty("Token")]
		public string Token { get; set; }
		/// <summary>支払い手続き開始URL</summary>
		[JsonProperty("StartURL")]
		public string StartUrl { get; set; }
		/// <summary>Start Limit Date</summary>
		[JsonProperty("StartLimitDate")]
		public string StartLimitDate { get; set; }
		/// <summary>PayPayトラッキングID</summary>
		[JsonProperty("PaypayTrackingID")]
		public string PaypayTrackingId { get; set; }
		/// <summary>現状態</summary>
		[JsonProperty("Status")]
		public string Status { get; set; }
	}
}
