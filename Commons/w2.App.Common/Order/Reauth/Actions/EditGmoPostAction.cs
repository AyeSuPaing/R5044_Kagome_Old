/*
=========================================================================================================
  Module      : Edit Gmo Post Action (EditGmoPostAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Input.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.TransactionModifyCancel;
using w2.App.Common.Order.Payment.GMO.TransactionRegister;
using w2.Common.Util;
using w2.Domain.Order;
using static w2.App.Common.Order.Reauth.Actions.ReauthGmoPostAction;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Class of edit action (GmoKb)
	/// </summary>
	public class EditGmoPostAction : BaseReauthAction<EditGmoPostAction.EditGmoPostActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public EditGmoPostAction(EditGmoPostActionParams editGmoPostActionParams)
			: base(
				ActionTypes.Update,
				(editGmoPostActionParams.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					? "GMO掛け払い（都度払い）編集"
					: "GMO掛け払い（枠保証）編集",
				editGmoPostActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Edit Gmokb transaction
		/// </summary>
		/// <param name="editGmoPostActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(EditGmoPostActionParams editGmoPostActionParams)
		{
			var orderModel = (OrderModel)editGmoPostActionParams.Order.Clone();
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(orderModel.ShopId);
			orderModel.PaymentOrderId = paymentOrderId;
			var orderInput = new OrderInput(orderModel);

			var request = new GmoRequestTransactionModifyCancel(orderModel);
			request.KindInfo.UpdateKind = UpdateKindType.OrderModify;
			var modifyCancelResult = new GmoTransactionApi().TransactionModifyCancel(request);

			if (modifyCancelResult.Result != ResultCode.OK)
			{
				return new ReauthActionResult(
					false,
					orderInput.OrderId,
					string.Empty,
					cardTranIdForLog: orderInput.CardTranId,
					apiErrorMessage: string.Join("\t", modifyCancelResult.Errors.Error.Select(x => LogCreator.CreateErrorMessage(x.ErrCode, x.ErrorMessage))));
			}

			return new ReauthActionResult(
				true,
				orderInput.OrderId,
				CreatePaymentMemo(orderInput.OrderPaymentKbn, paymentOrderId, modifyCancelResult.TransactionResult.GmoTransactionId, orderModel.LastBilledAmount),
				cardTranId: modifyCancelResult.TransactionResult.GmoTransactionId,
				paymentOrderId: paymentOrderId);
		}
		#endregion

		/// <summary>
		/// アクション実行後
		/// </summary>
		/// <param name="editGmoPostActionParams">再与信アクションパラメタ</param>
		/// <param name="reauthActionResult">再与信アクション結果</param>
		protected override void OnAfterExecute(EditGmoPostActionParams editGmoPostActionParams, ReauthActionResult reauthActionResult)
		{
			if (editGmoPostActionParams.IsNeedCallBillingConfirm && reauthActionResult.Result)
			{
				OrderCommon.ExecConfirmBillingGmoPost(editGmoPostActionParams.Order.OrderIdOrg, editGmoPostActionParams.Order.LastChanged, reauthActionResult.CardTranId);
			}
		}

		/// <summary>
		/// 再与信アクションパラメタ
		/// </summary>
		public class EditGmoPostActionParams : IReauthActionParams
		{
			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="order">注文情報</param>
			/// <param name="isNeedCallBillingConfirm">Is Need Call Billing Confirm</param>
			public EditGmoPostActionParams(OrderModel order, bool isNeedCallBillingConfirm = false)
			{
				this.Order = order;
				this.IsNeedCallBillingConfirm = isNeedCallBillingConfirm;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報</summary>
			public OrderModel Order { get; private set; }
			#endregion
			public bool IsNeedCallBillingConfirm { get; private set; }
		}
	}
}