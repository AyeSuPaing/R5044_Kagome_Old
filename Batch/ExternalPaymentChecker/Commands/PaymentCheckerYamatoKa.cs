/*
=========================================================================================================
  Module      : 入金チェッカー ヤマト決済（後払い）(PaymentCheckerYamatoKa.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.App.Common.Order;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.Commerce.Batch.ExternalPaymentChecker.Commands
{
	/// <summary>
	/// 入金チェッカー ヤマト決済（後払い）
	/// </summary>
	public class PaymentCheckerYamatoKa : PaymentCheckerBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paymentId">決済ID</param>
		public PaymentCheckerYamatoKa(string paymentId)
		{
			this.PaymentId = paymentId;
		}

		/// <summary>
		/// 実行
		/// </summary>
		public override void Exec()
		{
			// 対象注文取得
			var orders = GetNonPaymentOrders(this.PaymentId, DateTime.Now.AddDays(-1 * Constants.SEARCH_DAYS));

			Console.WriteLine(@"ヤマト後払い向け入金チェッカー　対象：{0}件", StringUtility.ToNumeric(orders.Count));

			var referenceApi = new PaymentYamatoKaReferenceApi();
			foreach (DataRowView drv in orders)
			{
				var orderId = (string)drv[Constants.FIELD_ORDER_ORDER_ID];
				var paymentOrderId = (string)drv[Constants.FIELD_ORDER_PAYMENT_ORDER_ID];

				// 取引情報取得
				var result = referenceApi.Exec(paymentOrderId);
				if (result == false)
				{
					AppLogger.WriteError("ヤマト決済（後払い） 取引情報照会API失敗：" + string.Format("{0}({1})", referenceApi.ResponseData.ErrorMessages, referenceApi.ResponseData.ErrorCode) + "：" + orderId);
					continue;
				}

				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();

					// 外部決済ステータスを「配送完了」に更新
					if (referenceApi.ResponseData.IsSalesFixed)
					{
						new OrderService().UpdateExternalPaymentStatusDeliveryComplete(
							orderId,
							Constants.FLG_LASTCHANGED_BATCH,
							UpdateHistoryAction.DoNotInsert,
							accessor);
						AppLogger.Write("info", "外部決済ステータスを変更しました。：" + orderId, true);
					}
					// 入金処理
					if (referenceApi.ResponseData.IsPaymented)
					{
						// 入金ステータス更新
						new OrderService().UpdatePaymentStatus(
							orderId,
							Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE,
							DateTime.Now.Date,
							Constants.FLG_LASTCHANGED_BATCH,
							UpdateHistoryAction.DoNotInsert,
							accessor);
						// 外部決済ステータスを「入金済み」に更新（更新履歴とともに）
						new OrderService().UpdateExternalPaymentStatusPayComplete(
							orderId,
							Constants.FLG_LASTCHANGED_BATCH,
							UpdateHistoryAction.DoNotInsert,
							accessor);

						// Update FixedPurchaseMemberFlg by settings
						// JugmentCondition payment Status is Complete
						if (Constants.FIXEDPURCHASE_MEMBER_CONDITION_INCLUDES_ORDER_PAYMENT_STATUS_COMPLETE
							&& (string.IsNullOrEmpty((string)drv[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]) == false))
						{
							var resultUpdate = new UserService().UpdateFixedPurchaseMemberFlg((string)drv[Constants.FIELD_ORDER_USER_ID],
								Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON,
								Constants.FLG_LASTCHANGED_BATCH,
								UpdateHistoryAction.DoNotInsert,
								accessor);

							if (resultUpdate == false) AppLogger.Write("error", "Update FixedPurchaseMemberFlg：" + (string)drv[Constants.FIELD_ORDER_USER_ID], true);
						}
					}

					// 更新履歴登録
					new UpdateHistoryService().InsertForOrder(orderId, Constants.FLG_LASTCHANGED_BATCH, accessor);

					AppLogger.Write("info", "入金／外部決済ステータスを変更しました。：" + orderId, true);

					accessor.CommitTransaction();
				}
			}
		}

		/// <summary>決済ID</summary>
		public string PaymentId { get; set; }
	}
}
