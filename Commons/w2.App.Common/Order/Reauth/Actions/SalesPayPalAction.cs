/*
=========================================================================================================
  Module      : 再与信アクション（PayPal売上確定）クラス(SalesPayPalAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.PayPal;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（PayPal売上確定）クラス
	/// </summary>
	public class SalesPayPalAction : BaseReauthAction<SalesPayPalAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SalesPayPalAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Sales, "PayPal売上確定", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// PayPal売上確定
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var order = reauthActionParams.Order;

			// 最終請求金額が0円の場合、エラーとする
			if (order.LastBilledAmount == 0)
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					apiErrorMessage: "最終請求金額が0円のため、売上確定できません。");
			}

			// 売上確定
			var result = PayPalUtility.Payment.Sales(order.CardTranId, order.LastBilledAmount);
			if (result.IsSuccess() == false)
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: "PayPal売上確定処理失敗エラー:\t" + string.Join(
						"\t",
						result.Errors.DeepAll().Select(
							error => LogCreator.CreateErrorMessage(error.Code.ToString(), error.Message))));
			}

			// 成功を返す
			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(order.OrderPaymentKbn, order.PaymentOrderId, order.CardTranId, order.LastBilledAmount),
				cardTranId: order.CardTranId,
				cardTranIdForLog: order.CardTranId);
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
			/// <param name="order">注文情報</param>
			public ReauthActionParams(OrderModel order)
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