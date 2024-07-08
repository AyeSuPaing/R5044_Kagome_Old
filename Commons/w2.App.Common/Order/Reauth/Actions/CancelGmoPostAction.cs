/*
=========================================================================================================
  Module      : Cancel Gmo Post Action(CancelGmoPostAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Option;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Extensions.Currency;
using w2.Domain.Order;
using w2.App.Common.Order.Payment.GMO.TransactionModifyCancel;
using w2.App.Common.Input.Order;
using static w2.App.Common.Order.Reauth.Actions.EditGmoPostAction;
using w2.Common.Util;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Class of cancel action (GmoKb)
	/// </summary>
	public class CancelGmoPostAction : BaseReauthAction<CancelGmoPostAction.CancelGmoPostActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CancelGmoPostAction(CancelGmoPostActionParams cancelGmoPostActionParams)
			: base(
				ActionTypes.Cancel,
				(cancelGmoPostActionParams.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					? "GMO掛け払い（都度払い）キャンセル"
					: "GMO掛け払い（枠保証）キャンセル",
				cancelGmoPostActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Cancel Gmokb transaction
		/// </summary>
		/// <param name="cancelInvoiceActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(CancelGmoPostActionParams cancelInvoiceActionParams)
		{
			var order = cancelInvoiceActionParams.Order;
			var orderInput = new OrderInput(order);
			var request = new GmoRequestTransactionModifyCancel(orderInput);
			request.KindInfo.UpdateKind = UpdateKindType.OrderCancel;

			var cancelResult = new GmoTransactionApi().TransactionModifyCancel(request);

			if (cancelResult.Result != ResultCode.OK)
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: string.Join("\t", cancelResult.Errors.Error.Select(x => LogCreator.CreateErrorMessage(x.ErrCode, x.ErrorMessage))));
			}
			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(order.OrderPaymentKbn, order.PaymentOrderId, order.CardTranId, order.LastBilledAmount),
				cardTranIdForLog: order.CardTranId);
		}

		#endregion

		/// <summary>
		/// 再与信アクションパラメタ
		/// </summary>
		public class CancelGmoPostActionParams : IReauthActionParams
		{
			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="order">注文情報</param>
			public CancelGmoPostActionParams(OrderModel order)
			{
				this.Order = order;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報</summary>
			public OrderModel Order { get; private set; }
			#endregion
		}
	}
}