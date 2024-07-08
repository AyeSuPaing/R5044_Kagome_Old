/*
=========================================================================================================
  Module      : Zcom実売上アダプタ (ZcomSalesRequestAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Util;

namespace w2.App.Common.Order.Payment.GMO.Zcom.Sales
{
	/// <summary>
	/// Zcom実売上アダプタ
	/// </summary>
	public class ZcomSalesRequestAdapter : BaseZcomSalesRequestAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private ZcomSalesRequestAdapter() : base() { }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		public ZcomSalesRequestAdapter(string paymentOrderId)
			: this(paymentOrderId, null)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		public ZcomSalesRequestAdapter(string paymentOrderId, ZcomApiSetting apisetting)
			: base(apisetting)
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
