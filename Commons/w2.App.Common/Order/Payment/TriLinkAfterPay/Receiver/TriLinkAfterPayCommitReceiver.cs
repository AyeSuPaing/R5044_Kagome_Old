/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) 注文確定通知受信クラス(TriLinkAfterPayCommitReceiver.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Receiver
{
	/// <summary>
	/// 後付款(TriLink後払い) 注文確定通知受信クラス
	/// </summary>
	[JsonObject]
	public class TriLinkAfterPayCommitReceiver
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="jsonString">JSON文字列</param>
		public TriLinkAfterPayCommitReceiver(string jsonString)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>加盟店注文番号</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_STORE_ORDER_CODE)]
		public string StoreOrderCode { get; private set; }
		/// <summary>問合せ番号</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ORDER_CODE)]
		public string OrderCode { get; private set; }
		#endregion
	}
}
