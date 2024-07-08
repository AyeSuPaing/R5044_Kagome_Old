/*
=========================================================================================================
  Module      : Modify Cancel Billing Gmo Post Action (ModifyCancelBillingGmoPostAction.cs)
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
using w2.App.Common.Order.Payment.GMO.BillingModification;
using w2.Domain.UpdateHistory.Helper;
using static w2.App.Common.Order.Reauth.Actions.EditGmoPostAction;
using w2.Common.Util;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Class of modify or cancel billing action (Gmokb)
	/// </summary>
	public class ModifyCancelBillingGmoPostAction : BaseReauthAction<ModifyCancelBillingGmoPostAction.ModifyCancelBillingGmoPostActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ModifyCancelBillingGmoPostAction(ModifyCancelBillingGmoPostActionParams modifyCancelBillingGmoPostActionParams)
			: base(
				ActionTypes.Billing,
				(modifyCancelBillingGmoPostActionParams.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					? "GMO掛け払い（都度払い）請求確定"
					: "GMO掛け払い（枠保証）請求確定",
				modifyCancelBillingGmoPostActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Modify or cancel Gmokb billing
		/// </summary>
		/// <param name="modifyCancelBillingInvoiceActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ModifyCancelBillingGmoPostActionParams modifyCancelBillingInvoiceActionParams)
		{
			var order = modifyCancelBillingInvoiceActionParams.Order;
			var orderInput = new OrderInput(order);
			var fixRequestDate = DateTime.Now.ToString("yyyy/MM/dd");
			var request = new GmoRequestBillingModification(order.CardTranId, fixRequestDate);
			if (modifyCancelBillingInvoiceActionParams.IsReturnAll)
			{
				request.KindInfo.UpdateKind = UpdateKindType.OrderCancel;
			}

			var modifyCancelResult = new GmoTransactionApi().BillingModifyCancel(request);

			if (modifyCancelResult.Result != ResultCode.OK)
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: string.Join("\t", modifyCancelResult.Errors.Error.Select(x => LogCreator.CreateErrorMessage(x.ErrCode, x.ErrorMessage))));
			}
			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(order.OrderPaymentKbn, order.PaymentOrderId, modifyCancelResult.TransactionInfo.GmoTransactionId, order.LastBilledAmount),
				cardTranIdForLog: modifyCancelResult.TransactionInfo.GmoTransactionId);
		}

		/// <summary>
		/// アクション実行後（ログ書いたりとかできる
		/// </summary>
		/// <param name="modifyCancelBillingGmoPostActionParams">再与信アクションパラメタ</param>
		/// <param name="reauthActionResult">再与信アクション結果</param>
		protected override void OnAfterExecute(ModifyCancelBillingGmoPostActionParams modifyCancelBillingGmoPostActionParams, ReauthActionResult reauthActionResult)
		{
			if (modifyCancelBillingGmoPostActionParams.IsReturnAll && reauthActionResult.Result)
			{
				var orderId = string.IsNullOrEmpty(modifyCancelBillingGmoPostActionParams.Order.OrderIdOrg) ? modifyCancelBillingGmoPostActionParams.Order.OrderId : modifyCancelBillingGmoPostActionParams.Order.OrderIdOrg;
				new OrderService().Modify(
					orderId,
					order =>
					{
						order.OnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED;
						order.DateChanged = DateTime.Now;
						order.LastChanged = modifyCancelBillingGmoPostActionParams.Order.LastChanged;
					},
					UpdateHistoryAction.Insert);
			}
		}
		#endregion

		/// <summary>
		/// 再与信アクションパラメタ
		/// </summary>
		public class ModifyCancelBillingGmoPostActionParams : IReauthActionParams
		{
			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="order">注文情報</param>
			/// <param name="isReturnAll">全て返却</param>
			public ModifyCancelBillingGmoPostActionParams(OrderModel order, bool isReturnAll)
			{
				this.Order = order;
				this.IsReturnAll = isReturnAll;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報</summary>
			public OrderModel Order { get; private set; }
			#endregion

			#region Is Return All
			// <summary>全て返却</summary>
			public bool IsReturnAll{ get; private set; }
			#endregion
		}
	}
}