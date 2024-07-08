/*
=========================================================================================================
  Module      : つくーるAPI連携：ユーザーポイント情報登録 (UserPointImport.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Sql;
using w2.Domain.Order;
using w2.App.Common;
using w2.App.Common.Order;
using w2.Commerce.Batch.ExternalOrderImport.Entity;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.ExternalOrderImport.Import.UreruAd
{
	/// <summary>
	/// つくーるAPI連携：ユーザーポイント情報登録
	/// </summary>
	public class UserPointImport : UreruAdImportBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseData">レスポンスデータ</param>
		/// <param name="accessor">SQLアクセサ</param>
		public UserPointImport(UreruAdResponseDataItem responseData, SqlAccessor accessor)
			: base(responseData, accessor)
		{
		}

		/// <summary>
		/// 登録
		/// </summary>
		public override void Import()
		{
			var orders = new OrderService().GetOrdersByUserId(this.ResponseData.User.UserId, this.Accessor);
			var isFirstOrder =
				(orders.Any(
					order =>
						((order.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP)
							&& (order.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED)
							&& (order.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
							&& (string.IsNullOrEmpty(order.OrderStatus) == false) && (order.OrderId != this.ResponseData.OrderId))) == false);

			var orderInfo = OrderCommon.CreateOrderInfo(
				this.ResponseData.Cart,
				this.ResponseData.OrderId,
				this.ResponseData.User.UserId,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				Constants.FLG_LASTCHANGED_BATCH);

			PublishPointForImport(orderInfo, isFirstOrder, UpdateHistoryAction.Insert);
		}

		/// <summary>
		/// ポイント発行
		/// </summary>
		/// <param name="orderInfo">注文情報</param>
		/// <param name="isFirstOrder">初回注文？</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		private void PublishPointForImport(Hashtable orderInfo, bool isFirstOrder, UpdateHistoryAction updateHistoryAction)
		{
			var transactionName = "通常購入ポイント";
			var result = (OrderCommon.AddUserPoint(
				orderInfo,
				this.ResponseData.Cart,
				Constants.FLG_POINTRULE_POINT_INC_KBN_BUY,
				UpdateHistoryAction.DoNotInsert,
				this.Accessor) >= 0);
			if (result == false)
			{
				this.ResponseData.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_FAILED_TO_ADD_USER_POINT)
					.Replace("@@ 1 @@", this.ResponseData.Id)
					.Replace("@@ 2 @@", transactionName));
				return;
			}

			if ((this.ResponseData.Cart.FirstBuyPoint != 0)
				&& (this.ResponseData.Cart.FirstBuyPointKbn == Constants.FLG_POINTRULE_INC_TYPE_RATE)
				&& isFirstOrder)
			{
				transactionName = "初回購入ポイント";
				result = (OrderCommon.AddUserPoint(
					orderInfo,
					this.ResponseData.Cart,
					Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY,
					UpdateHistoryAction.DoNotInsert,
					this.Accessor) >= 0);
				if (result == false)
				{
					this.ResponseData.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_FAILED_TO_ADD_USER_POINT)
						.Replace("@@ 1 @@", this.ResponseData.Id)
						.Replace("@@ 2 @@", transactionName));
				}
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(
					this.ResponseData.Cart.OrderUserId,
					Constants.FLG_LASTCHANGED_BATCH,
					this.Accessor);
			}
		}
	}
}
