/*
=========================================================================================================
  Module      : 注文同梱設定クラス(OrderCombineSettings.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.Domain.Order.Helper;

namespace w2.App.Common.Order.OrderCombine
{
	/// <summary>
	/// 注文同梱設定クラス
	/// </summary>
	class OrderCombineSettings : IOrderCombineSettings
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="isFront">フロントか(TRUEの場合はフロント、FALSEの場合管理画面</param>
		public OrderCombineSettings(bool isFront)
		{
			this.AllowPaymentKbn = isFront
				? Constants.ORDERCOMBINE_ALLOW_PAYMENT_KBN_FRONT
				: Constants.ORDERCOMBINE_ALLOW_PAYMENT_KBN_MANAGER
					.Where(paymentId => (Constants.PAYMENT_PAIDY_KBN != Constants.PaymentPaidyKbn.Paygent)
						|| (paymentId != Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY))
					.ToArray();

			if ((isFront == false)
				&& (this.AllowPaymentKbn.Length == 0))
			{
				this.AllowPaymentKbn = new string[] { string.Empty };
			}

			this.AllowOrderStatus = isFront
				? Constants.ORDERCOMBINE_ALLOW_ORDER_STATUS_FRONT
				: Constants.ORDERCOMBINE_ALLOW_ORDER_STATUS_MANAGER;

			this.AllowOrderPaymentStatus = isFront
				? Constants.ORDERCOMBINE_ALLOW_ORDER_PAYMENT_STATUS_FRONT
				: Constants.ORDERCOMBINE_ALLOW_ORDER_PAYMENT_STATUS_MANAGER;

			this.AllowOrderDayPassed = isFront
				? Constants.ORDERCOMBINE_ALLOW_ORDER_DAY_PASSED_FRONT
				: Constants.ORDERCOMBINE_ALLOW_ORDER_DAY_PASSED_MANAGER;

			this.AllowShippingDayBefore = isFront
				? Constants.ORDERCOMBINE_ALLOW_SHIPPING_DAY_BEFORE_FRONT
				: Constants.ORDERCOMBINE_ALLOW_SHIPPING_DAY_BEFORE_MANAGER;

			this.DenyShippingIds = isFront
				? Constants.ORDERCOMBINE_DENY_SHIPPING_ID_FRONT
				: Constants.ORDERCOMBINE_DENY_SHIPPING_ID_MANAGER;

			this.DenyShippingMethods = isFront
				?Constants.ORDERCOMBINE_DENY_SHIPPING_METHOD_FRONT
				: Constants.ORDERCOMBINE_DENY_SHIPPING_METHOD_MANAGER;
		}

		/// <summary>注文同梱許可 決済種別</summary>
		public string[] AllowPaymentKbn { get; private set; }
		/// <summary>注文同梱許可 注文ステータス</summary>
		public string[] AllowOrderStatus { get; private set; }
		/// <summary>注文同梱許可 注文決済ステータス</summary>
		public string[] AllowOrderPaymentStatus { get; private set; }
		/// <summary>注文同梱許可 注文日からの経過日数</summary>
		public int AllowOrderDayPassed { get; private set; }
		/// <summary>注文同梱許可 配送希望日までの日数</summary>
		public int AllowShippingDayBefore { get; private set; }
		/// <summary>注文同梱許可しない 配送種別</summary>
		public string[] DenyShippingIds { get; private set; }
		/// <summary>注文同梱許可しない 配送方法</summary>
		public string[] DenyShippingMethods { get; private set; }
	}
}
