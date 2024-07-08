/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) 注文確定依頼リクエストクラス(TriLinkAfterPayCommitRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Request
{
	/// <summary>
	/// 後付款(TriLink後払い) 注文確定依頼リクエストクラス
	/// </summary>
	[JsonObject]
	public class TriLinkAfterPayCommitRequest : RequestBase
	{
		#region コンストラクタ
		public TriLinkAfterPayCommitRequest(string acceptNumber, string storeOrderCode)
		{
			this.AcceptNumber = acceptNumber;
			this.StoreOrderCode = storeOrderCode;
			this.RequestUrl = Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_API_URL + "orders/commit/" + Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_SITE_CODE;
		}
		#endregion

		#region プロパティ
		/// <summary>承認番号</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ACCEPT_NUMBER)]
		public string AcceptNumber { get; set; }
		/// <summary>加盟店注文番号</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_STORE_ORDER_CODE)]
		public string StoreOrderCode { get; set; }
		#endregion
	}
}
