/*
=========================================================================================================
  Module      : 再与信基底アクションクラス(BaseReauthAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Global.Region.Currency;
using w2.Common.Sql;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信基底アクションクラス
	/// </summary>
	public abstract class BaseReauthAction<T> : IReauthAction where T : IReauthActionParams
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="actionType">アクションタイプ</param>
		/// <param name="actionName">アクション名</param>
		private BaseReauthAction(ActionTypes actionType, string actionName)
		{
			this.ActionType = actionType;
			this.ActionName = actionName;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="actionType">アクションタイプ</param>
		/// <param name="actionName">アクション名</param>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <param name="accessor">Sql Accessor</param>
		internal BaseReauthAction(ActionTypes actionType, string actionName, T reauthActionParams, SqlAccessor accessor = null)
			: this(actionType, actionName)
		{
			this.ReauthActionParams = reauthActionParams;
			this.Accessor = accessor;
		}
		#endregion

		#region メソッド
		/// <summary>再与信アクション</summary>
		/// <returns>再与信結果</returns>
		public ReauthActionResult Execute()
		{
			OnBeforeExecute(this.ReauthActionParams);
			var result = OnExecute(this.ReauthActionParams);
			OnAfterExecute(this.ReauthActionParams, result);
			return result;
		}

		/// <summary>再与信アクション</summary>
		/// <returns>再与信結果</returns>
		protected abstract ReauthActionResult OnExecute(T reauthActionParams);

		/// <summary>アクション実行前（ログ書いたりとかできる）</summary>
		protected virtual void OnBeforeExecute(T reauthActionParams)
		{
		}

		/// <summary>アクション実行後（ログ書いたりとかできる）</summary>
		protected virtual void OnAfterExecute(T reauthActionParams, ReauthActionResult reauthActionResult)
		{
		}

		/// <summary>
		/// アクションタイプ取得
		/// </summary>
		/// <returns>アクションタイプ</returns>
		public ActionTypes GetActionType()
		{
			return this.ActionType;
		}

		/// <summary>
		/// アクション名取得
		/// </summary>
		/// <returns>アクション名</returns>
		public string GetActionName()
		{
			return this.ActionName;
		}

		/// <summary> 再与信情報(決済注文ID、取引ID)を更新 </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="cardTransId">取引ID</param>
		public virtual void UpdateReauthInfo(string paymentOrderId, string cardTransId)
		{
		}

		/// <summary>
		/// 決済メモ作成
		/// </summary>
		/// <param name="paymentId">決済種別ID</param>
		/// <param name="paymentOrderId">決済注文ID／注文ID</param>
		/// <param name="cardTranId">決済カード取引ID</param>
		/// <param name="lastBilledAmount">最終請求金額（キャンセルの場合はnull）</param>
		/// <returns>決済メモ</returns>
		protected string CreatePaymentMemo(string paymentId, string paymentOrderId, string cardTranId, decimal? lastBilledAmount)
		{
			if (this.ActionType == ActionTypes.NoAction) return string.Empty;

			var paymentMemo = OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
				paymentId,
				paymentOrderId,
				cardTranId,
				this.ActionName,
				lastBilledAmount,
				true);
			return paymentMemo;
		}

		/// <summary>
		/// 送金額
		/// </summary>
		/// <param name="order">注文</param>
		/// <returns>送金額</returns>
		protected decimal GetSendingAmount(OrderModel order)
		{
			return CurrencyManager.GetSendingAmount(order.LastBilledAmount, order.SettlementAmount, order.SettlementCurrency);
		}
		#endregion

		#region プロパティ
		/// <summary>アクションタイプ</summary>
		public ActionTypes ActionType { get; private set; }
		/// <summary>アクション名</summary>
		public string ActionName { get; private set; }
		/// <summary>再与信アクションパラメタ</summary>
		protected T ReauthActionParams { get; set; }
		/// <summary>Sql Accessor</summary>
		protected SqlAccessor Accessor { get; set; }
		#endregion
	}
}