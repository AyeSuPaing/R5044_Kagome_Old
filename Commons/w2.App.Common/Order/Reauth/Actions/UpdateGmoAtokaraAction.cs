/*
=========================================================================================================
  Module      : 再与信アクション（GMOアトカラ）クラス(UpdateGmoAtokaraAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.GMOAtokara;
using w2.App.Common.Order.Payment.GMOAtokara.Utils;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（GMOアトカラ）クラス
	/// </summary>
	public class UpdateGmoAtokaraAction : BaseReauthAction<UpdateGmoAtokaraAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		public UpdateGmoAtokaraAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Update, "GMOアトカラ注文情報更新", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// GMOアトカラ注文情報更新
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderNew = reauthActionParams.OrderNew;
			var orderOld = reauthActionParams.OrderOld;

			return ExecutePayment(orderOld, orderNew);
		}

		/// <summary>
		/// Execute Payment
		/// </summary>
		/// <param name="orderOld">注文情報（変更前）</param>
		/// <param name="orderNew">注文情報（変更後）</param>
		/// <returns>再与信アクション結果</returns>
		private ReauthActionResult ExecutePayment(OrderModel orderOld, OrderModel orderNew)
		{
			var cancelApi = new PaymentGmoAtokaraCancelApi();
			var apiResult = cancelApi.Exec(PaymentGmoAtokaraTypes.UpdateKind.Modify, orderNew);

			if (apiResult == false)
			{
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					cancelApi.GetErrorMessage(),
					cardTranIdForLog: orderOld.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(
						cancelApi.ResponseData.Errors.ErrorCode,
						cancelApi.ResponseData.Errors.ErrorMessage));
			}

			// 決済取引ID、決済注文IDは更新する必要がないので元注文のものをそのまま入れる
			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				CreatePaymentMemo(
					orderOld.OrderPaymentKbn,
					orderOld.PaymentOrderId,
					orderOld.CardTranId,
					orderOld.LastBilledAmount),
				paymentOrderId: orderOld.PaymentOrderId,
				cardTranId: orderOld.CardTranId,
				cardTranIdForLog: string.Empty);
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
