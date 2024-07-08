/*
=========================================================================================================
  Module      : FLAPS受注結果モデル (OrderResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;

namespace w2.App.Common.Flaps.Order
{
	/// <summary>
	/// FLAPS受注結果モデル
	/// </summary>
	[JsonObject(Constants.FLAPS_API_RESULT_ORDER)]
	public class OrderResult : ResultBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderResult()
		{
			this.Message = string.Empty;
			this.PosNoSerNo = string.Empty;
			this.PisNo = string.Empty;
			this.InvoiceNo = string.Empty;
			this.MemberSerNo = string.Empty;
			this.MemberCode = string.Empty;
		}
		
		/// <summary>メッセージ</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_ORDER_MESSAGE)]
		public string Message { get; set; }
		/// <summary>ショップカウンター業績唯一番号</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_ORDER_POST_NO_SER_NO)]
		public string PosNoSerNo { get; set; }
		/// <summary>販売コード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_ORDER_PIS_NO)]
		public string PisNo { get; set; }
		/// <summary>電子発票番号</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_ORDER_INVOICE_NO)]
		public string InvoiceNo { get; set; }
		/// <summary>会員唯一番号</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_ORDER_MEMBER_SER_NO)]
		public string MemberSerNo { get; set; }
		/// <summary>会員コード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_ORDER_MEMBER_CODE)]
		public string MemberCode { get; set; }
	}
}