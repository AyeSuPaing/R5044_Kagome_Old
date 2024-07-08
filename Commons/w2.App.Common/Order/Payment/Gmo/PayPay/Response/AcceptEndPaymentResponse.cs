/*
=========================================================================================================
  Module      : Accept End Payment Response (AcceptEndPaymentResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Paypay.Response
{
	/// <summary>
	/// Accept end payment response
	/// </summary>
	public class AcceptEndPaymentResponse : PaypayGmoResponse
	{
		/// <summary>オーダーID</summary>
		[JsonProperty("OrderID")]
		public string OrderId { get; set; }
		/// <summary>トークン</summary>
		[JsonProperty("Token")]
		public string Token { get; set; }
		/// <summary>現状態</summary>
		[JsonProperty("Status")]
		public string Status { get; set; }
		/// <summary>エラーコード</summary>
		[JsonProperty("ErrCode")]
		public string ErrorCode { get; set; }
		/// <summary>エラー詳細コード</summary>
		[JsonProperty("ErrInfo")]
		public string ErrorInfo { get; set; }
	}
}
