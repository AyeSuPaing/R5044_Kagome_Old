/*
=========================================================================================================
  Module      : Reauth Atone Action(ReauthAtoneAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Atone;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.User;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Reauth Atone Action
	/// </summary>
	public class ReauthAtoneAction : BaseReauthAction<ReauthAtoneAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ReauthAtoneAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Reauth, "atone翌月払い与信", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Atone 与信
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var orderNew = reauthActionParams.OrderNew;
			var result = ExecAuthAtone(orderOld, orderNew);
			return result;
		}

		/// <summary>
		/// Exec Auth Atone
		/// </summary>
		/// <param name="orderOld">注文情報(変更前)</param>
		/// <param name="orderNew">注文情報(変更後)</param>
		/// <returns>与信結果情報</returns>
		private ReauthActionResult ExecAuthAtone(OrderModel orderOld, OrderModel orderNew)
		{
			var requestReauthAtone =
				AtonePaymentApiFacade.CreateDataAtoneAuthoriteForReturnExchange(orderNew, orderOld);
			if (orderNew.IsNotReturnExchangeOrder == false)
				requestReauthAtone.Data.RelatedTransaction = orderOld.CardTranId;

			var isChangePaymentToAtone = (orderOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
				&& (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE);

			requestReauthAtone.Data.UpdatedTransactions = (isChangePaymentToAtone
				? new[] { orderNew.CardTranId }
				: null);
			var user = new UserService().Get(orderNew.UserId);
			if (user != null)
			{
				requestReauthAtone.Data.AuthenticationToken = StringUtility.ToEmpty(
					user.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_ATONE_TOKEN_ID]);
			}
			
			var responseReauthAtone = AtonePaymentApiFacade.CreatePayment(requestReauthAtone);

			if (responseReauthAtone.IsSuccess == false)
			{
				var reauthAtoneErrors = responseReauthAtone.Errors.Select(item => item.Code);
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: orderOld.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(
						string.Join(",", reauthAtoneErrors),
						responseReauthAtone.Message));
			}
			if (responseReauthAtone.IsAuthorizationSuccess == false)
			{
				var result = new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: orderOld.CardTranId,
					apiErrorMessage: responseReauthAtone.AuthorizationResultNgReasonMessage);
				return result;
			}

			orderNew.CardTranId = StringUtility.ToEmpty(responseReauthAtone.TranId);
			orderNew.PaymentOrderId = string.Empty;
			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				CreatePaymentMemo(
					orderOld.OrderPaymentKbn,
					orderOld.OrderId,
					orderNew.CardTranId,
					decimal.Parse(requestReauthAtone.Data.Amount)),
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
