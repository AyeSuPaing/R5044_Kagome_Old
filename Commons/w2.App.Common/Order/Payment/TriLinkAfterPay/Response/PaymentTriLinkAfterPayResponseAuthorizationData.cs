/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) レスポンス審査データ(PaymentTriLinkAfterPayResponseAuthorizationData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Response
{
	/// <summary>
	/// 後付款(TriLink後払い) レスポンス審査データ
	/// </summary>
	[JsonObject]
	public class PaymentTriLinkAfterPayResponseAuthorizationData
	{
		#region プロパティ
		/// <summary>審査結果</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_RESULT)]
		public string Result { get; set; }
		#endregion
	}
}
