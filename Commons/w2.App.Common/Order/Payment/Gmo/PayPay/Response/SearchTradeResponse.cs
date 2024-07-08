/*
=========================================================================================================
  Module      : 決済照会結果 (SearchTradeResponse.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.Paypay.Response
{
	/// <summary>
	/// 決済照会結果
	/// </summary>
	public class SearchTradeResponse : PaypayGmoResponse
	{
		/// <summary>ステータス</summary>
		[IdPassProperty("Status")]
		public string Status { get; set; }
		/// <summary>処理区分</summary>
		[IdPassProperty("JobCd")]
		public string JobCode { get; set; }
		/// <summary>取引ID</summary>
		[IdPassProperty("AccessID")]
		public string AccessID { get; set; }
		/// <summary>取引パスワード</summary>
		[IdPassProperty("AccessPass")]
		public string AccessPass { get; set; }
		/// <summary>Amount</summary>
		[IdPassProperty("Amount")]
		public string Amount { get; set; }
	}
}
