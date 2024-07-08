/*
=========================================================================================================
  Module      : コンビニ後払い出荷報告完了更新（返品注文用）コマンドクラス(UpdateCvsDefShipmentCompleteForReturnOrderCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.Helper;
using w2.App.Common.Order.Payment.GMO.Shipment;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.DeliveryCompany.Helper;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.UpdateShipmentForReturnOrder.Commands
{
	/// <summary>
	/// ヤマト後払い出荷報告完了更新（返品注文用）コマンドクラス
	/// </summary>
	public class UpdateCvsDefShipmentCompleteForReturnOrderCommand : CommandBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateCvsDefShipmentCompleteForReturnOrderCommand()
			: base()
		{
			this.Action = "コンビニ後払い出荷報告更新（返品注文用）";
		}
		#endregion

		#region メソッド
		/// <summary>
		/// ヤマト後払い出荷報告完了更新（返品注文用）実行
		/// </summary>
		protected override void Exec()
		{
			// 更新対象の注文情報取得
			var orders = new OrderService().GetCvsDefUpdateShipmentCompleteTargeReturnOrder();

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

				// 出荷報告依頼
				bool result = true;
				try
				{
					string errorMessage = null;
					switch (Constants.PAYMENT_CVS_DEF_KBN)
					{
						case Constants.PaymentCvsDef.YamatoKa:
							errorMessage = ExecuteYamatoKaShipmentEntry(order);
							break;

						case Constants.PaymentCvsDef.Gmo:
							errorMessage = ExecuteGmoShipmentEntry(order);
							break;

						default:
							errorMessage = "該当のコンビニ後払いは再与信をサポートしていません。";
							break;
					}

					// 外部決済連携ログ格納
					OrderCommon.AppendExternalPaymentCooperationLog(
						string.IsNullOrEmpty(errorMessage),
						order.OrderId,
						string.IsNullOrEmpty(errorMessage)
							? LogCreator.CreateMessage(order.OrderId, order.PaymentOrderId)
							: errorMessage ?? "",
						Constants.FLG_LASTCHANGED_BATCH,
						UpdateHistoryAction.Insert);

					// 失敗？
					if (string.IsNullOrEmpty(errorMessage) == false)
					{
						throw new Exception(errorMessage);
					}
				}
				catch (Exception ex)
				{
					// エラーメッセージ格納
					externalPaymentErrorMessage = ex.Message;
					this.ErrorMessages.Add(CreateErrorMessage(order.OrderId, externalPaymentErrorMessage));

					// エラーログ出力
					AppLogger.WriteError(ex);

					result = false;
				}

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

		/// <summary>
		/// ヤマトコンビニ後払い出荷報告依頼
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>結果</returns>
		private string ExecuteYamatoKaShipmentEntry(OrderModel order)
		{
			string errMessage = null;
			// 出荷報告依頼
			var api = new PaymentYamatoKaShipmentEntryApi();
			var result = api.Exec(
				order.PaymentOrderId,
				order.Shippings[0].ShippingCheckNo,
				(DeliveryCompanyUtil.GetDeliveryCompanyType(order.Shippings[0].DeliveryCompanyId, order.OrderPaymentKbn) == Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_YAMATO)
					? PaymentYamatoKaProcessDiv.Entry
					: PaymentYamatoKaProcessDiv.InsertUpdate,
				null,
				string.Empty);

			// 失敗？
			if (result == false)
			{
				errMessage = LogCreator.CreateErrorMessage(api.ResponseData.ErrorCode, api.ResponseData.ErrorMessages);
			}
			return errMessage;
		}

		/// <summary>
		/// GMOコンビニ後払い出荷報告依頼
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>結果</returns>
		private string ExecuteGmoShipmentEntry(OrderModel order)
		{
			string errMessage = null;
			var request = new GmoRequestShipment();
			request.Transaction = new TransactionElement();
			request.Transaction.GmoTransactionId = order.CardTranId;
			request.Transaction.Pdcompanycode = PaymentGmoDeliveryServiceCode.GetDeliveryServiceCode(
				DeliveryCompanyUtil.GetDeliveryCompanyType(order.Shippings[0].DeliveryCompanyId, order.OrderPaymentKbn));
			request.Transaction.Slipno = order.Shippings[0].ShippingCheckNo;
			// 出荷報告依頼
			var result = new GmoDeferredApiFacade().Shipment(request);
			// 失敗？
			if (result.Result != ResultCode.OK)
			{
				errMessage = string.Join(
					"\t",
					result.Errors.Error.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage)));
			}
			return errMessage;
		}
		#endregion
	}
}