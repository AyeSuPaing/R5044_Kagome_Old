/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) レスポンスエラーデータ(PaymentTriLinkAfterPayResponseErrorData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Response
{
	/// <summary>
	/// 後付款(TriLink後払い) レスポンスエラーデータ
	/// </summary>
	[JsonObject]
	public class PaymentTriLinkAfterPayResponseErrorData
	{
		/// <summary>問合せ番号</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ORDER_CODE)]
		public string OrderCode { get; set; }
		/// <summary>理由コード</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_REASON)]
		public string Reason { get; set; }
		/// <summary>対象フィールド</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_FIELDS)]
		public string Fields { get; set; }
		/// <summary>エラーコード</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ERROR_CODE)]
		public string ErrorCode { get; set; }
		/// <summary>メッセージ</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_MESSAGE)]
		public string Message { get; set; }
	}
}
