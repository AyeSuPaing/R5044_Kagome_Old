/*
=========================================================================================================
  Module      : Zcomキャンセルアダプタ (ZcomCancelRequestAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Util;

namespace w2.App.Common.Order.Payment.GMO.Zcom.Cancel
{
	/// <summary>
	/// Zcomキャンセルアダプタ
	/// </summary>
	public class ZcomCancelRequestAdapter : BaseZcomCancelRequestAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private ZcomCancelRequestAdapter() : base() { }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		public ZcomCancelRequestAdapter(string paymentOrderId)
			: this(paymentOrderId, null)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="apiSetting">Api設定</param>
		public ZcomCancelRequestAdapter(string paymentOrderId, ZcomApiSetting apiSetting)
			: base(apiSetting)
		{
			this.PaymentOrderId = paymentOrderId;
		}

		/// <summary>
		/// オーダー番号取得
		/// </summary>
		/// <returns>オーダー番号</returns>
		public override string GetOrderNumber()
		{
			return StringUtility.ToEmpty(this.PaymentOrderId);
		}

		/// <summary>決済注文ID</summary>
		public string PaymentOrderId { get; set; }
	}
}
