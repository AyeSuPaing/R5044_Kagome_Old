/*
=========================================================================================================
  Module      : YAHOO API Yahooモール注文更新進行クラス (YahooMallOrderUpdateProcedure.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Mall.Yahoo.Interfaces;
using w2.App.Common.Mall.Yahoo.YahooMallOrders;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.DeliveryCompany;
using w2.Domain.Order;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.App.Common.Mall.Yahoo.Procedures
{
	/// <summary>
	/// YAHOO API Yahooモール注文更新進行クラス
	/// </summary>
	public class YahooMallOrderUpdateProcedure : IYahooMallOrderUpdateProcedure
	{
		private readonly IOrderService _orderService;
		private readonly IUpdateHistoryService _updateHistoryService;
		private readonly IUserService _userService;
		private readonly IDeliveryCompanyService _deliveryCompanyService;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public YahooMallOrderUpdateProcedure()
		{
			_orderService = new OrderService();
			_updateHistoryService = new UpdateHistoryService();
			_userService = new UserService();
			_deliveryCompanyService = new DeliveryCompanyService();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public YahooMallOrderUpdateProcedure(
			IUpdateHistoryService updateHistoryService,
			IOrderService orderService,
			IUserService userService,
			IDeliveryCompanyService deliveryComapneyService)
		{
			_orderService = orderService;
			_updateHistoryService = updateHistoryService;
			_userService = userService;
			_deliveryCompanyService = deliveryComapneyService;
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="order">注文</param>
		/// <param name="tempUserId">一時ユーザーのユーザーID</param>
		/// <param name="mallId">モールID</param>
		/// <param name="orderId">ｗ２モール注文ID</param>
		/// <returns>更新結果</returns>
		public bool Update(YahooMallOrder order, string tempUserId, string mallId, string orderId)
		{
			try
			{
				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();

					// 正式なユーザーが存在する場合、仮注文登録時に登録したTempユーザーを削除
					var mallOrderUserId = _userService.GetUserId(
						mallId: mallId,
						mailAddr: order.Owner.OwnerMailAddr,
						accessor);
					var hasMallOrderUser = string.IsNullOrEmpty(mallOrderUserId) == false;
					var userId = hasMallOrderUser ? mallOrderUserId : tempUserId;
					if (hasMallOrderUser)
					{
						_userService.Delete(
							tempUserId,
							Constants.FLG_LASTCHANGED_BATCH,
							UpdateHistoryAction.DoNotInsert,
							accessor);
					}

					// ユーザーを改めて登録
					var user = order.GenerateParamSetToUpdateUser(user: _userService.Get(userId: userId));
					var updateWithUserExtendResult =
						_userService.UpdateWithUserExtend(user, UpdateHistoryAction.Insert, accessor);
					if (updateWithUserExtendResult == false)
					{
						FileLogger.WriteError($"ユーザー登録に失敗しました。order_id={order.OrderId},user_id={userId}");
						return false;
					}
					
					// 注文テーブル (w2_Order) 更新
					var orderUpdateResult = _orderService.AddYahooMallOrderDetail(
						input: order.GenerateParamSetToUpdateOrder(userId, orderId),
						accessor: accessor);
					if (orderUpdateResult == false)
					{
						FileLogger.WriteError($"注文テーブルの更新に失敗しました。order_id={order.OrderId}");
						return false;
					}

					// 注文者テーブル (w2_OrderOwner) 更新
					var orderOwnerUpdateResult = _orderService.AddYahooMallOrderOwnerDetail(
						input: order.GenerateParamSetToUpdateOrderOwner(orderId),
						accessor: accessor);
					if (orderOwnerUpdateResult == false)
					{
						FileLogger.WriteError($"注文者テーブルの更新に失敗しました。order_id={order.OrderId}");
						return false;
					}

					// 配送希望時間設定取得
					// shippingTimeId変数の値が... 空: 指定なし, NULL: 指定した設定がDBに存在しない
					var shippingTimeId = order.Shipping.HasShipRequestTime()
						? _deliveryCompanyService
							.GetByOrderId(order.OrderId, accessor)
							.GetTrimmedShippingTimeList()
							.SingleOrDefault(t => t.Value.TrimStart('0').Replace("-", "～") == order.Shipping.TrimmedShipRequestTime).Key
						: "";
					if (shippingTimeId == null)
					{
						FileLogger.WriteError(
							$"指定された配送希望時間設定が存在しません。'配送サービス設定' > '配送サービス設定編集' > '配送可能時間帯情報'の設定をご確認ください。order_id ={order.OrderId},ShipRequestTime={order.Shipping.TrimmedShipRequestTime}");
						return false;
					}

					// 注文配送テーブル (w2_OrderShipping) 更新
					var orderShippingUpdateResult = _orderService.AddYahooMallOrderShippingDetail(
						input: order.GenerateParamSetToUpdateOrderShipping(shippingTime: shippingTimeId ?? "", orderId),
						accessor);
					if (orderShippingUpdateResult == false)
					{
						FileLogger.WriteError($"注文配送テーブルの更新に失敗しました。order_id={order.OrderId}");
						return false;
					}

					// モール連携ステータスを変更
					_orderService.UpdateMallLinkStatus(
						orderId,
						mallLinkStatus: Constants.FLG_ORDER_MALL_LINK_STATUS_UNSHIP_ODR,
						Constants.FLG_LASTCHANGED_BATCH,
						UpdateHistoryAction.DoNotInsert,
						accessor);

					// 履歴の更新
					_updateHistoryService.InsertForOrder(
						orderId,
						Constants.FLG_LASTCHANGED_BATCH,
						accessor);
					_updateHistoryService.InsertForUser(userId: userId, Constants.FLG_LASTCHANGED_BATCH, accessor);

					accessor.CommitTransaction();
				}
			}
			catch(Exception ex)
			{
				FileLogger.WriteError(ex);
				return false;
			}

			return true;
		}

		/// <summary>
		/// 失敗したことを記録する
		/// </summary>
		/// <param name="orderId">注文ID</param>
		public void RecordFailedResult(string orderId)
		{
			// モール連携ステータスを変更
			_orderService.UpdateMallLinkStatus(
				orderId,
				mallLinkStatus: Constants.FLG_ORDER_MALL_LINK_STATUS_PEND_ODR,
				Constants.FLG_LASTCHANGED_BATCH,
				UpdateHistoryAction.Insert);

			// 外部連携メモを更新
			var msg = "Yahooモール注文取込に失敗しました。";
			_orderService.AppendRelationMemo(orderId, msg, Constants.FLG_LASTCHANGED_BATCH, UpdateHistoryAction.Insert);
		}
	}
}
