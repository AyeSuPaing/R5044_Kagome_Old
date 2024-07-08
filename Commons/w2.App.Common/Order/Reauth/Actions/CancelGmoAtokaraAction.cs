/*
=========================================================================================================
  Module      : GMOアトカラ キャンセルアクション(CancelGmoAtokaraAction.cs)
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
	/// GMOアトカラ キャンセルアクション
	/// </summary>
	public class CancelGmoAtokaraAction : BaseReauthAction<CancelGmoAtokaraAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">Reauth action params</param>
		public CancelGmoAtokaraAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Cancel, "GMOアトカラキャンセル", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// GMOアトカラ キャンセル
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var orderNew = reauthActionParams.OrderNew;
			var result = ExecCancelGmoAtokara(orderOld, orderNew);

			return result;
		}

		/// <summary>
		/// GMOアトカラ キャンセル 実行
		/// </summary>
		/// <param name="orderOld">注文情報(変更前)</param>
		/// <param name="orderNew">注文情報(変更後)</param>
		/// <returns>結果情報</returns>
		private ReauthActionResult ExecCancelGmoAtokara(OrderModel orderOld, OrderModel orderNew)
		{
			var cancelApi = new PaymentGmoAtokaraCancelApi();
			var apiResult = cancelApi.Exec(PaymentGmoAtokaraTypes.UpdateKind.Cancel, orderOld);

			if (apiResult == false)
			{
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
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
			/// <param name="orderOld">注文情報(変更前)</param>
			/// <param name="orderNew">注文情報(変更後)</param>
			public ReauthActionParams(OrderModel orderOld, OrderModel orderNew)
			{
				this.OrderOld = orderOld;
				this.OrderNew = orderNew;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報(変更前)</summary>
			public OrderModel OrderOld { get; private set; }
			/// <summary>注文情報(変更後)</summary>
			public OrderModel OrderNew { get; private set; }
			#endregion
		}
	}
}
