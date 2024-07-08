/*
=========================================================================================================
  Module      : ユーザー注文履歴サービスクラス(UserHistoryOrderService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace w2.App.Common.Cs.UserHistory
{
	/// <summary>
	///ユーザー注文履歴サービスクラス
	/// </summary>
	public class UserHistoryOrderService
	{
		/// <summary>レポジトリ</summary>
		private UserHistoryOrderRepository Repository;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">レポジトリ</param>
		public UserHistoryOrderService(UserHistoryOrderRepository repository)
		{
			this.Repository = repository;
		}

		/// <summary>
		/// ユーザー履歴（注文）取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>履歴一覧</returns>
		public UserHistoryBase[] GetList(string userId)
		{
			var orderHistories = new List<UserHistoryBase>();

			string tempId = null;
			var tempShippingNo = 0;
			UserHistoryOrder temp = null;
			foreach (DataRowView drv in this.Repository.GetUserHistoryOrders(userId))
			{
				if (tempId != (string)drv[Constants.FIELD_ORDER_ORDER_ID])
				{
					temp = new UserHistoryOrder(drv);
					tempId = temp.OrderId;
					tempShippingNo = 0;
					orderHistories.Add(temp);
				}
				if (tempShippingNo != (int)drv[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO])
				{
					temp.Shippings.Add(new UserHistoryOrderShipping(drv));
					tempShippingNo = (int)drv[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO];
				}
				if (drv[Constants.FIELD_ORDERITEM_PRODUCT_ID] != DBNull.Value) temp.Items.Add(new UserHistoryOrderItem(drv));
			}
			return orderHistories.ToArray();
		}
	}
}