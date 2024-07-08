/*
=========================================================================================================
  Module      : Reduce Gmo Post Action (ReduceGmoPostAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.TransactionReduce;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;
using static w2.App.Common.Order.Reauth.Actions.EditGmoPostAction;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Class of reduce claims action (GmoKb)
	/// </summary>
	public class ReduceGmoPostAction : BaseReauthAction<ReduceGmoPostAction.ReduceGmoPostActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reduceGmoPostActionParams">再与信アクションパラメタ</param>
		public ReduceGmoPostAction(ReduceGmoPostActionParams reduceGmoPostActionParams)
			: base(
				ActionTypes.Reduce,
				(reduceGmoPostActionParams.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					? "減額請求 GMO掛け払い（都度払い）与信"
					: "減額請求 GMO掛け払い（枠保証）与信",
				reduceGmoPostActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Reduce Gmokb transaction
		/// </summary>
		/// <param name="reduceGmoPostActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReduceGmoPostActionParams reduceGmoPostActionParams)
		{
			var orderInput = (OrderModel)reduceGmoPostActionParams.Order.Clone();
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(orderInput.ShopId);
			orderInput.PaymentOrderId = paymentOrderId;
			var request = new GmoRequestTransactionReduce(orderInput);
			var registerResult = new GmoTransactionApi().ReduceClaims(request);
			if (registerResult.Result != ResultCode.OK)
			{
				return new ReauthActionResult(
					false,
					orderInput.OrderId,
					string.Empty,
					cardTranIdForLog: orderInput.CardTranId,
					apiErrorMessage: string.Join("\t", registerResult.Errors.Error.Select(x => LogCreator.CreateErrorMessage(x.ErrCode, x.ErrorMessage))));
			}

			var cardTrandId = registerResult.TransactionResult.NewGmoTransactionId;

			return new ReauthActionResult(
				true,
				orderInput.OrderId,
				CreatePaymentMemo(orderInput.OrderPaymentKbn, orderInput.PaymentOrderId, cardTrandId, orderInput.LastBilledAmount),
				cardTrandId,
				orderInput.PaymentOrderId);
		}

		/// <summary>
		/// アクション実行後
		/// </summary>
		/// <param name="reduceGmoPostActionParams">再与信アクションパラメタ</param>
		/// <param name="reauthActionResult">再与信アクション結果</param>
		protected override void OnAfterExecute(ReduceGmoPostActionParams reduceGmoPostActionParams, ReauthActionResult reauthActionResult)
		{
			if (reauthActionResult.Result)
			{
				new OrderService().Modify(
				reduceGmoPostActionParams.Order.OrderIdOrg,
				order =>
				{
					order.CardTranId = reauthActionResult.CardTranId;
					order.PaymentOrderId = reauthActionResult.PaymentOrderId;
					order.DateChanged = DateTime.Now;
					order.LastChanged = reduceGmoPostActionParams.Order.LastChanged;
				},
				UpdateHistoryAction.Insert);
			}
		}
		#endregion

		/// <summary>
		/// 再与信アクションパラメタ
		/// </summary>
		public class ReduceGmoPostActionParams : IReauthActionParams
		{
			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="order">注文情報</param>
			public ReduceGmoPostActionParams(OrderModel order)
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
