/*
=========================================================================================================
  Module      : ヤマトクレジットカード出荷報告完了更新（返品注文用）コマンドクラス(UpdateYamatoKwcShipmentCompleteForReturnOrderCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.YamatoKwc;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.DeliveryCompany.Helper;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.UpdateShipmentForReturnOrder.Commands
{
	/// <summary>
	/// ヤマトクレジットカード出荷報告完了更新（返品注文用）コマンドクラス
	/// </summary>
	public class UpdateYamatoKwcShipmentCompleteForReturnOrderCommand : CommandBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateYamatoKwcShipmentCompleteForReturnOrderCommand()
			: base()
		{
			this.Action = "ヤマトクレジットカード出荷報告更新（返品注文用）";
		}
		#endregion

		#region メソッド
		/// <summary>
		/// ヤマトクレジットカード出荷報告完了更新（返品注文用）実行
		/// </summary>
		protected override void Exec()
		{
			// ヤマトクレジットカード以外の場合何もしない
			if (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.YamatoKwc) return;

			// 更新対象の注文情報取得
			var orders = new OrderService().GetCreditUpdateShipmentCompleteTargeReturnOrder();

			// 対象日時取得（外部決済与信日時）から10分経っている
			orders = orders
				.Where(o =>
					(o.ExternalPaymentAuthDate.HasValue)
					&& ((DateTime.Now - o.ExternalPaymentAuthDate.Value).TotalMinutes >= 10)).ToArray();

			// 外部決済ステータスを「出荷報告済み」更新
			UpdateExternalPaymentStatusShipmentComplete(orders);
		}

		/// <summary>
		/// 外部決済ステータスを「出荷報告済み」更新
		/// </summary>
		/// <param name="orders">注文情報</param>
		private void UpdateExternalPaymentStatusShipmentComplete(OrderModel[] orders)
		{
			// 外部決済ステータスを「出荷報告済み」に更新
			var successCount = 0;
			var errorCount = 0;
			foreach (var order in orders)
			{
				var externalPaymentErrorMessage = string.Empty;
				var result = false;
				try
				{
					var resultInfo = new PaymentYamatoKwcTradeInfoApi().Exec(order.PaymentOrderId);

					// 外部決済連携ログ格納処理
					OrderCommon.AppendExternalPaymentCooperationLog(
						resultInfo.Success,
						order.OrderId,
						resultInfo.Success
							? LogCreator.CreateMessage(order.OrderId, order.PaymentOrderId)
							: LogCreator.CreateErrorMessage(resultInfo.ErrorCode, resultInfo.ErrorMessage),
						Constants.FLG_LASTCHANGED_BATCH,
						UpdateHistoryAction.Insert);

					// 取引情報照会
					if (resultInfo.Success == false)
					{
						externalPaymentErrorMessage = " 取引情報照会に失敗しました。" + resultInfo.ErrorInfoForLog;
					}

					if (resultInfo.ResultDatas.Count == 0)
					{
						externalPaymentErrorMessage = " 取引情報が取得できませんでした。";
					}
					// 配送伝票番号存在するかどうかチェック
					if (string.IsNullOrEmpty(externalPaymentErrorMessage) && (string.IsNullOrEmpty(resultInfo.ResultDatas[0].SlipNo) == false))
					{
						var resultCancel = new PaymentYamatoKwcShipmentCancelApi().Exec(order.PaymentOrderId, resultInfo.ResultDatas[0].SlipNo);

						// 外部決済連携ログ処理
						OrderCommon.AppendExternalPaymentCooperationLog(
							resultCancel.Success,
							order.OrderId,
							resultCancel.Success
								? LogCreator.CreateMessage(order.OrderId, order.PaymentOrderId)
								: LogCreator.CreateErrorMessage(resultCancel.ErrorCode, resultCancel.ErrorMessage),
							Constants.FLG_LASTCHANGED_BATCH,
							UpdateHistoryAction.Insert);

						if (resultCancel.Success == false)
						{
							externalPaymentErrorMessage = " 出荷情報取消に失敗しました。" + resultCancel.ErrorInfoForLog;
						}
					}
					// 出荷情報登録→売上確定
					if (string.IsNullOrEmpty(externalPaymentErrorMessage))
					{
						var resultEntry = new PaymentYamatoKwcShipmentEntryApi().Exec(order.PaymentOrderId,
							order.Shippings[0].ShippingCheckNo,
							(DeliveryCompanyUtil.GetDeliveryCompanyType(order.Shippings[0].DeliveryCompanyId, order.OrderPaymentKbn) == Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_YAMATO)
								? PaymentYamatoKwcDeliveryServiceCode.YAMATO_KWC_DLIVERY_SERVICE_CODE_YAMATO
								: PaymentYamatoKwcDeliveryServiceCode.YAMATO_KWC_DLIVERY_SERVICE_CODE_OTHER);

						// 外部決済連携ログ格納処理
						OrderCommon.AppendExternalPaymentCooperationLog(
							resultEntry.Success,
							order.OrderId,
							resultEntry.Success
								? LogCreator.CreateMessage(order.OrderId, order.PaymentOrderId)
								: LogCreator.CreateErrorMessage(resultEntry.ErrorCode, resultEntry.ErrorMessage),
							Constants.FLG_LASTCHANGED_BATCH,
							UpdateHistoryAction.Insert);

						if (resultEntry.Success)
						{
							result = true;
						}
						else
						{
							externalPaymentErrorMessage = " 出荷情報登録に失敗しました。" + resultEntry.ErrorInfoForLog;
						}
					}
				}
				catch (Exception ex)
				{
					// エラーメッセージ格納
					externalPaymentErrorMessage = ex.Message;

					// エラーログ出力
					AppLogger.WriteError(ex);
				}
				this.ErrorMessages.Add(CreateErrorMessage(order.OrderId, externalPaymentErrorMessage));

				using (SqlAccessor accessor = new SqlAccessor())
				{
					// トランザクション開始
					accessor.OpenConnection();
					accessor.BeginTransaction();

					try
					{
						// 成功？
						var service = new OrderService();
						if (result)
						{
							service.UpdateExternalPaymentStatusShipmentComplete(
								order.OrderId,
								Constants.FLG_LASTCHANGED_BATCH,
								UpdateHistoryAction.Insert,
								accessor);
						}
						// 失敗？
						else
						{
							service.UpdateExternalPaymentStatusShipmentError(
								order.OrderId,
								externalPaymentErrorMessage,
								Constants.FLG_LASTCHANGED_BATCH,
								UpdateHistoryAction.Insert,
								accessor);
						}
						// トランザクションコミット
						accessor.CommitTransaction();

						if (result) successCount++;
						if (result == false) errorCount++;
					}
					catch (Exception ex)
					{
						errorCount++;

						// エラーメッセージ格納
						this.ErrorMessages.Add(CreateErrorMessage(order.OrderId, ex.Message));
						// エラーログ出力
						AppLogger.WriteError(ex);
						break;
					}
				}
			}
			// 完了プロパティセット
			SetFinishProperties(successCount, errorCount);
		}
		#endregion
	}
}