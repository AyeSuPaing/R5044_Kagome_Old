/*
=========================================================================================================
  Module      : Reauth Gmo Post Action (ReauthGmoPostAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.TransactionRegister;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;
using static w2.App.Common.Order.Reauth.Actions.EditGmoPostAction;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Class of reatuth action (GmoKb)
	/// </summary>
	public class ReauthGmoPostAction : BaseReauthAction<ReauthGmoPostAction.ReauthGmoPostActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ReauthGmoPostAction(ReauthGmoPostActionParams reauthGmoPostActionParams)
			: base(
				ActionTypes.Reauth,
				(reauthGmoPostActionParams.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					? "GMO掛け払い（都度払い）再与信"
					: "GMO掛け払い（枠保証）再与信",
				reauthGmoPostActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Register GmoKb transaction
		/// </summary>
		/// <param name="reauthGmoPostActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthGmoPostActionParams reauthGmoPostActionParams)
		{
			var orderInput = (OrderModel)reauthGmoPostActionParams.Order.Clone();
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(orderInput.ShopId);
			orderInput.PaymentOrderId = paymentOrderId;
			orderInput.DeviceInfo = reauthGmoPostActionParams.Order.DeviceInfo;
			var request = new GmoRequestTransactionRegister(orderInput);
			var registerResult = new GmoTransactionApi().TransactionRegister(request);

			if (registerResult.IsResultNg)
			{
				return new ReauthActionResult(
					false,
					orderInput.OrderId,
					string.Empty,
					cardTranIdForLog: orderInput.CardTranId,
					apiErrorMessage: string.Join("\t", registerResult.Errors.Error.Select(x => LogCreator.CreateErrorMessage(x.ErrCode, x.ErrorMessage))));
			}

			if (registerResult.IsNG || registerResult.IsAlert)
			{
				return new ReauthActionResult(
					false,
					orderInput.OrderId,
					string.Empty,
					cardTranIdForLog: orderInput.CardTranId,
					apiErrorMessage: CommerceMessages.GetMessages(CommerceMessages.ERRMSG_GMO_KB_PAYMENT_ALERT));
			}

			return new ReauthActionResult(
				true,
				orderInput.OrderId,
				CreatePaymentMemo(orderInput.OrderPaymentKbn, paymentOrderId, registerResult.TransactionResult.GmoTransactionId, orderInput.LastBilledAmount),
				cardTranId: registerResult.TransactionResult.GmoTransactionId,
				paymentOrderId: paymentOrderId, cardTranIdForLog: string.Empty, apiErrorMessage: string.Empty, authLostForError: false, transactionResult: registerResult.TransactionResult);
		}
		#endregion

		/// <summary>
		/// アクション実行後
		/// </summary>
		/// <param name="reauthGmoPostActionParams">再与信アクションパラメタ</param>
		/// <param name="reauthActionResult">再与信アクション結果</param>
		protected override void OnAfterExecute(ReauthGmoPostActionParams reauthGmoPostActionParams, ReauthActionResult reauthActionResult)
		{
			var transactionResult = reauthActionResult.TransactionResult;
			if (transactionResult != null)
			{
				if (reauthActionResult.Result)
				{
					new OrderService().Modify(
						reauthGmoPostActionParams.Order.OrderId,
						order =>
						{
							order.OrderPaymentStatus = Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE;
							order.OrderPaymentDate = DateTime.Now;
							order.ExternalPaymentStatus = StringUtility.ToEmpty(transactionResult.AuthorResult);
						},
						UpdateHistoryAction.Insert);
				}
				else
				{
					new OrderService().Modify(
						reauthGmoPostActionParams.Order.OrderId,
						order =>
						{
							order.ExternalPaymentStatus = StringUtility.ToEmpty(transactionResult.AuthorResult);
						},
						UpdateHistoryAction.Insert);
				}
			}
		}

		/// <summary>
		/// 再与信アクションパラメタ
		/// </summary>
		public class ReauthGmoPostActionParams : IReauthActionParams
		{
			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="order">注文情報</param>
			public ReauthGmoPostActionParams(OrderModel order)
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
