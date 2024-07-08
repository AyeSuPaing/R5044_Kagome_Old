/*
=========================================================================================================
  Module      : 外部連携注文決済登録クラス(OrderPaymentRegisterExternal.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order.Register
{
	/// <summary>
	/// 外部連携注文決済登録クラス
	/// </summary>
	/// <remarks></remarks>
	public class OrderPaymentRegisterExternal
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="lastChanged">最終更新者</param>
		public OrderPaymentRegisterExternal(OrderModel order, CartObject cart, string lastChanged)
		{
			this.Order = order;
			this.Cart = cart;
			this.PaymentRegister = new OrderPaymentRegister(
				new OrderRegisterProperties(
					OrderRegisterBase.ExecTypes.External,
					true,
					lastChanged));
		}

		/// <summary>
		/// 決済処理
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string ExecPayment()
		{
			try
			{
				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();

					var paymentStatusCompleteFlg =
						(this.Cart.HasDigitalContents && Constants.DIGITAL_CONTENTS_OPTION_ENABLED)
							? Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE_FORDIGITALCONTENTS
							: Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE;
					var successPaymentStatus = paymentStatusCompleteFlg
						? Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE
						: Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM;

					bool execPaymentResult = true;
					switch (this.Cart.Payment.PaymentId)
					{
						// コンビニ決済処理(後払い)
						case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF:
							execPaymentResult = this.PaymentRegister.ExecPaymentCvsDef(
								this.Order.DataSource,
								this.Cart);
							break;

						// NP後払い
						case Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY:
							execPaymentResult = this.PaymentRegister.ExecPaymentNpAfterPay(
								this.Order.DataSource,
								this.Cart);
							if (execPaymentResult)
							{
								this.Order.OrderPaymentStatus = successPaymentStatus;
							}
							break;

						default:
							break;
					}

					this.Order.PaymentMemo = OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
						this.Order.OrderId,
						this.Cart.Payment.PaymentId,
						this.Order.CardTranId,
						"与信",
						this.Cart.PriceTotal);

					var service = new OrderService();
					// 決済取引情報更新
					service.UpdateCardTransactionAndPaymentMemo(
						this.Order.OrderId,
						this.Order.PaymentOrderId,
						this.Order.CardTranId,
						this.Order.PaymentMemo,
						Constants.FLG_LASTCHANGED_BATCH,
						UpdateHistoryAction.DoNotInsert,
						accessor);

					if (execPaymentResult)
					{
						// 注文ステータスを注文済みに更新
						service.UpdateOrderStatus(
							this.Order.OrderId,
							Constants.FLG_ORDER_ORDER_STATUS_ORDERED,
							DateTime.Now,
							Constants.FLG_LASTCHANGED_BATCH,
							UpdateHistoryAction.DoNotInsert,
							accessor);

						// 定期注文の場合定期購入ステータスを通常に更新
						if (this.Order.IsFixedPurchaseOrder)
						{
							new FixedPurchaseService()
								.UpdateFixedPurchaseStatusTempToNormal(
									this.Order.OrderId,
									this.Order.FixedPurchaseId,
									Constants.FLG_LASTCHANGED_BATCH,
									UpdateHistoryAction.DoNotInsert,
									accessor);
						}

						// 外部決済ステータス更新
						service.UpdateExternalPaymentInfoForAuthSuccess(
							this.Order.OrderId,
							Constants.FLG_LASTCHANGED_BATCH,
							UpdateHistoryAction.DoNotInsert,
							accessor);

						// 更新履歴登録
						new UpdateHistoryService().InsertAllForOrder(this.Order.OrderId, Constants.FLG_LASTCHANGED_BATCH, accessor);
					}
					else
					{
						// 外部決済ステータスエラー情報更新
						var errorMessage = string.Format("外部注文取り込み時与信でエラーが発生しました。" + Environment.NewLine
							+ " （エラー内容：{0}）" + Environment.NewLine,
							this.PaymentRegister.ApiErrorMessage);

						// 外部決済ステータス更新（更新履歴とともに）
						service.UpdateExternalPaymentInfoForAuthError(
							this.Order.OrderId,
							errorMessage,
							Constants.FLG_LASTCHANGED_BATCH,
							UpdateHistoryAction.Insert,
							accessor);
						accessor.CommitTransaction();

						if (this.Order.IsFixedPurchaseOrder
							&& (((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atodene)
									&& (this.Cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
								|| (this.Cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)))
						{
							new FixedPurchaseService().UpdateForFailedPayment(
								this.Order.FixedPurchaseId,
								Constants.FLG_LASTCHANGED_BATCH,
								UpdateHistoryAction.Insert);
						}

						throw new Exception(errorMessage);
					}

					accessor.CommitTransaction();
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return ex.Message;
			}
			return string.Empty;
		}

		#region プロパティ
		/// <summary>注文情報</summary>
		private OrderModel Order { get; set; }
		/// <summary>カート</summary>
		private CartObject Cart { get; set; }
		/// <summary>注文決済登録クラス</summary>
		private OrderPaymentRegister PaymentRegister { get; set; }
		#endregion
	}
}
