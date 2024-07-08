/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) アクセストークンリクエストクラス(TriLinkAfterPayAccessTokenRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Request
{
	/// <summary>
	/// 後付款(TriLink後払い) アクセストークンリクエストクラス
	/// </summary>
	[JsonObject]
	public class TriLinkAfterPayAccessTokenRequest : RequestBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TriLinkAfterPayAccessTokenRequest()
		{
			this.RequestUrl = Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_API_URL + "token/" + Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_SITE_CODE;
			this.SecretKey = Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_SECRET_KEY;
		}
		#endregion

		#region プロパティ
		/// <summary>シークレットキー</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_SECRET_KEY)]
		public string SecretKey { get; set; }
		#endregion
	}
}
