/*
=========================================================================================================
  Module      : Reauth Aftee Action(ReauthAfteeAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Aftee;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.User;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Reauth Aftee Action
	/// </summary>
	public class ReauthAfteeAction : BaseReauthAction<ReauthAfteeAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ReauthAfteeAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Reauth, "aftee翌月払い与信", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Aftee 与信
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var orderNew = reauthActionParams.OrderNew;
			var result = ExecAuthAftee(orderOld, orderNew);
			return result;
		}

		/// <summary>
		/// Exec Auth Aftee
		/// </summary>
		/// <param name="orderOld">注文情報(変更前)</param>
		/// <param name="orderNew">注文情報(変更後)</param>
		/// <param name="authAmount">与信金額</param>
		/// <returns>与信結果情報</returns>
		private ReauthActionResult ExecAuthAftee(OrderModel orderOld, OrderModel orderNew)
		{
			var requestReauthAftee =
				AfteePaymentApiFacade.CreateDataAfteeAuthoriteForReturnExchange(orderNew, orderOld);
			if (orderNew.IsNotReturnExchangeOrder == false)
			{
				requestReauthAftee.Data.RelatedTransaction = string.Empty;
			}

			var isChangePaymentToAftee = (orderOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					&& (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);

			requestReauthAftee.Data.UpdatedTransactions = (isChangePaymentToAftee
				? orderNew.CardTranId
				: orderOld.CardTranId);

			var user = new UserService().Get(orderNew.UserId);
			if (user != null)
			{
				requestReauthAftee.Data.AuthenticationToken = StringUtility.ToEmpty(
					user.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_AFTEE_TOKEN_ID]);
			}
			var responseReauthAftee = AfteePaymentApiFacade.CreatePayment(requestReauthAftee);

			if (responseReauthAftee.IsSuccess == false)
			{
				var reauthAfteeErrors = responseReauthAftee.Errors.Select(item => item.Code);
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: orderOld.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(
						string.Join(",", reauthAfteeErrors),
						responseReauthAftee.Message));
			}

			orderNew.CardTranId = StringUtility.ToEmpty(responseReauthAftee.TranId);
			orderNew.PaymentOrderId = string.Empty;
			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				CreatePaymentMemo(
					orderOld.OrderPaymentKbn,
					orderOld.OrderId,
					orderNew.CardTranId,
					decimal.Parse(requestReauthAftee.Data.Amount)),
				cardTranId: orderNew.CardTranId,
				cardTranIdForLog: orderOld.CardTranId);
		}
		#endregion

		/// <summary>
		/// 再与信アクションパラメタ
		/// </summary>
		public new class ReauthActionParams : IReauthActionParams
		{
			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="orderOld">注文情報（変更前）</param>
			/// <param name="orderNew">注文情報（変更後）</param>
			public ReauthActionParams(OrderModel orderOld, OrderModel orderNew)
			{
				this.OrderOld = orderOld;
				this.OrderNew = orderNew;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報（変更前）</summary>
			public OrderModel OrderOld { get; private set; }
			/// <summary>注文情報（変更後）</summary>
			public OrderModel OrderNew { get; private set; }
			#endregion
		}
	}
}
