/*
=========================================================================================================
  Module      : 注文キャンセルコマンドクラス(OrderCancelCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Order;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.OrderCancelBatch
{
	/// <summary>
	/// 注文キャンセルコマンド
	/// </summary>
	public class OrderCancelCommand
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="orderCalcelIntervalMinutes">注文キャンセルインターバル(分)</param>
		public OrderCancelCommand(int orderCalcelIntervalMinutes)
		{
			this.OrderCalcelIntervalMinutes = orderCalcelIntervalMinutes;
		}

		/// <summary>
		/// キャンセル実行
		/// </summary>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>実行件数</returns>
		public int Exec(UpdateHistoryAction updateHistoryAction)
		{
			// 注文情報取得
			var orders = GetTargetOrders();

			// ループして注文キャンセル
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				orders.ForEach(
					order =>
					{
						var orderId = (string)order[Constants.FIELD_ORDER_ORDER_ID];
						var fixedPurchaseId = (string)order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID];

						// キャンセル可能か確認
						if (CheckCanCancelOrder(order, accessor) == false) return;

						// 注文キャンセル付随処理（先にこちらから）
						CancelOrderSubProcess(order, UpdateHistoryAction.DoNotInsert, accessor);

						// 注文ステータスをキャンセルに（更新履歴とともに）
						new OrderService().Modify(
							orderId,
							(orderModel) =>
							{
								orderModel.OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED;
								orderModel.LastChanged = Constants.FLG_LASTCHANGED_BATCH;
							},
							UpdateHistoryAction.Insert,
							accessor);

						// 更新履歴登録
						if (updateHistoryAction == UpdateHistoryAction.Insert)
						{
							new UpdateHistoryService().InsertForFixedPurchase((string)order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID], Constants.FLG_LASTCHANGED_BATCH, accessor);
							new UpdateHistoryService().InsertForOrder(orderId, Constants.FLG_LASTCHANGED_BATCH, accessor);
							new UpdateHistoryService().InsertForUser((string)order[Constants.FIELD_ORDER_USER_ID], Constants.FLG_LASTCHANGED_BATCH, accessor);
						}

						//仮登録キャンセルする（更新履歴とともに）
						new FixedPurchaseService().CancelTemporaryRegistrationFixedPurchase(
							fixedPurchaseId,
							Constants.FLG_LASTCHANGED_BATCH,
							UpdateHistoryAction.Insert,
							accessor,
							orderId);

						// トランザクションコミット
						accessor.CommitTransaction();

						// ログを落とす
						w2.Common.Logger.FileLogger.WriteInfo((string)order[Constants.FIELD_ORDER_ORDER_ID] + " を仮注文キャンセルしました。");
					});
			}

			return orders.Count;
		}

		/// <summary>
		/// ターゲット注文取得
		/// </summary>
		/// <returns></returns>
		private List<DataRowView> GetTargetOrders()
		{
			// ターゲット注文ID取得
			var orderIds = GetTargetOrderIds();

			// ターゲット注文取得
			List<DataRowView> orders = new List<DataRowView>();
			foreach (var orderId in orderIds)
			{
				DataView order = OrderCommon.GetOrder(orderId);
				if (order.Count != 0)
				{
					orders.Add(order[0]);
				}
			}
			return orders;
		}

		/// <summary>
		/// ターゲット注文ID取得
		/// </summary>
		/// <returns>注文IDリスト</returns>
		private string[] GetTargetOrderIds()
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement("OrderCancel", "GetCancelTargetOrderIds"))
			{
				Hashtable param = new Hashtable();
				param.Add("order_calcel_interval_minutes", this.OrderCalcelIntervalMinutes);
				statement.SetDymamicParameters(param, Constants.ORDER_CANCEL_DISALLOW_PAYMENT_KBNS, SqlDbType.NVarChar);

				var dv = statement.SelectSingleStatementWithOC(accessor, param);
				return dv.Cast<DataRowView>().Select(drv => (string)drv[0]).ToArray();
			}
		}

		/// <summary>
		/// 注文キャンセル付随処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void CancelOrderSubProcess(DataRowView order, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			var isNeedRollbackStock =
				((StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN])
					== Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY));
			// 注文キャンセル付随処理
			OrderCommon.CancelOrderSubProcess(
				order,
				isNeedRollbackStock,
				// ここでは実在庫は関係ないとしてるので実在庫系のステートメントも実装していません
				"注文キャンセルバッチ",
				Constants.W2MP_DEPT_ID,
				"batch",
				true,
				updateHistoryAction,
				accessor);
		}

		/// <summary>
		/// キャンセル可能か
		/// </summary>
		/// <param name="order">注文</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>キャンセル可能か</returns>
		private bool CheckCanCancelOrder(DataRowView order, SqlAccessor accessor)
		{
			var paymentId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]);
			var orderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);
			var paymentOrderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
			var cardTranId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_CARD_TRAN_ID]);

			// 仮登録ステータスか確認
			if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
				&& (new OrderService().CheckTemporaryRegistration(orderId, accessor) == false)) return false;

			// Paidy決済では、決済取引IDと決済注文IDのどちらも空の場合以外では仮注文をキャンセルしない
			if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
				&& (Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
				&& ((string.IsNullOrEmpty(paymentOrderId) == false)
					|| (string.IsNullOrEmpty(cardTranId) == false))) return false;

			return true;
		}

		/// <summary>注文キャンセルインターバル</summary>
		public int OrderCalcelIntervalMinutes { get; private set; }
	}
}
