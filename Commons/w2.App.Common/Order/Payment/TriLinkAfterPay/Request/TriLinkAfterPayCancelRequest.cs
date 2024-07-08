/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) キャンセルリクエストクラス(TriLinkAfterPayCancelRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Request
{
	/// <summary>
	/// 後付款(TriLink後払い) キャンセルリクエストクラス
	/// </summary>
	public class TriLinkAfterPayCancelRequest : RequestBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cardTranId">決済取引ID</param>
		public TriLinkAfterPayCancelRequest(string cardTranId)
		{
			this.RequestUrl = Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_API_URL + "orders/" + Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_SITE_CODE + "/" + cardTranId;
		}
		#endregion
	}
}
