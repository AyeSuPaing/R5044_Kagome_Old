/*
=========================================================================================================
  Module      : 注文取り込み(初期移行)クラス(ImportActionMigration.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Sql;
using w2.Common.Logger;
using w2.Domain.Order;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.App.Common.Order.Import.OrderImport.Entity;
using w2.Common.Util;

namespace w2.App.Common.Order.Import.OrderImport.ImportAction
{
	/// <summary>
	/// 注文取り込み(初期移行)
	/// </summary>
	public class ImportActionMigration : ImportActionBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="orderData">取り込み注文データ</param>
		public ImportActionMigration(OrderData orderData)
			: base(orderData)
		{
		}

		/// <summary>
		/// 取り込み処理
		/// </summary>
		public override void Import()
		{
			SetDefaultValue();

			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				try
				{
					// ユーザー登録
					if (this.ImportData.UserOrg == null)
					{
						new UserService().Insert(this.ImportData.User, UpdateHistoryAction.DoNotInsert, accessor);
					}

					// 注文登録
					var service = new OrderService();
					service.InsertOrder(this.ImportData.Order, UpdateHistoryAction.DoNotInsert, accessor);

					// クーポン登録
					if ((Constants.W2MP_COUPON_OPTION_ENABLED)
						&& (string.IsNullOrEmpty(this.ImportData.Coupon.CouponCode) == false))
					{
						var order = new OrderService();
						order.InsertCoupon(this.ImportData.Coupon, UpdateHistoryAction.DoNotInsert, accessor);
					}

					// 履歴
					new UpdateHistoryService().InsertAllForOrder(this.ImportData.Order.OrderId, this.ImportData.Order.LastChanged, accessor);

					// コミット
					accessor.CommitTransaction();

					this.OrderItemImportCount += this.ImportData.Order.OrderItemCount;
				}
				catch (Exception ex)
				{
					AppLogger.WriteError(ex);
					this.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_UNEXPECTED_ERROR));
					accessor.RollbackTransaction();
				}
			}
		}

		/// <summary>
		/// 初期値設定
		/// </summary>
		protected override void SetDefaultValue()
		{
			this.ImportData.Order.OrderItemCount = this.ImportData.Order.Items.Count();
			this.ImportData.Order.OrderProductCount = this.ImportData.Order.Items.Sum(item => item.ItemQuantity);
		}

	}
}
