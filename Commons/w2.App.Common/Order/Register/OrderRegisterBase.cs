/*
=========================================================================================================
  Module      : 注文登録基底クラス(OrderRegisterBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web;
using w2.App.Common.Flaps;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Line.SendLineMessage;
using w2.App.Common.Mail;
using w2.App.Common.NextEngine.Helper;
using w2.App.Common.Order.OPlux;
using w2.App.Common.Order.OPlux.NameNormalization;
using w2.App.Common.Order.OPlux.RegisterEvent;
using w2.App.Common.Order.Reauth;
using w2.App.Common.Order.Payment;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.ContentsLog;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.Order;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.Domain.SubscriptionBox;
using w2.Domain.Order.Helper;
using static w2.App.Common.Constants;
using w2.Common;
using w2.Domain;

namespace w2.App.Common.Order.Register
{
	/// <summary>
	/// 注文登録基底クラス
	/// </summary>
	public abstract class OrderRegisterBase
	{
		/// <summary>Request Key Order Id</summary>
		private const string REQUEST_KEY_ORDER_ID = "odid";

		/// <summary>注文実行種別</summary>
		public enum ExecTypes
		{
			/// <summary>PCフロント</summary>
			Pc,
			/// <summary>モバイルフロント</summary>
			Mobile,
			/// <summary>管理注文登録</summary>
			CommerceManager,
			/// <summary>定期購入バッチ（今すぐ注文含む）</summary>
			FixedPurchaseBatch,
			/// <summary>外部連携注文</summary>
			External
		}

		/// <summary>注文結果</summary>
		public enum ResultTypes
		{
			/// <summary>成功</summary>
			Success,
			/// <summary>失敗</summary>
			Fail,
			/// <summary>スキップ（外部決済など）</summary>
			Skip
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="orderExecType">注文実行種別</param>
		/// <param name="isUser">ユーザーか（ポイント付与判断など）</param>
		/// <param name="lastChanged">DB最終更新者</param>
		/// <param name="senderType">管理者向け注文完了メール送信者種別</param>
		protected OrderRegisterBase(
			ExecTypes orderExecType,
			bool isUser,
			string lastChanged,
			Constants.EnabledOrderCompleteEmailSenderType senderType = Constants.EnabledOrderCompleteEmailSenderType.Batch)
		{
			this.Properties = new OrderRegisterProperties(orderExecType, isUser, lastChanged);
			this.ApiErrorMessage = string.Empty;
			this.Properties.OrderCompleteEmailSenderType= senderType;
		}

		/// <summary>
		/// 再与信を実行しつつ注文実行をする
		/// </summary>
		/// <param name="orderOld">古い受注</param>
		/// <param name="order">受注情報</param>
		/// <param name="cart">カート</param>
		/// <param name="registGuestUser">ゲストを作るか</param>
		/// <param name="isFirstCart">最初のカートか</param>
		/// <param name="isOrderCombine">同梱かどうか</param>
		/// <param name="isFront">フロントかどうか</param>
		/// <param name="disablePaymentCancelOrderId">外部決済キャンセル対象外の受注ID</param>
		/// <param name="inputOrderNew">新しい受注</param>
		/// <returns></returns>
		public ResultTypes ExecReauthAndNewOrder(
			OrderModel orderOld,
			Hashtable order,
			CartObject cart,
			bool registGuestUser,
			bool isFirstCart,
			bool isOrderCombine,
			bool isFront,
			out string disablePaymentCancelOrderId,
			OrderModel inputOrderNew = null)
		{
			var newOrder = inputOrderNew ?? cart.CreateNewOrder((string)order[Constants.FIELD_ORDER_ORDER_ID]);

			newOrder.CardTranId = orderOld.CardTranId;

			disablePaymentCancelOrderId = orderOld.OrderId;
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = string.Empty;

			var apiErrorMessage = string.Empty;
			var reauthResult = ExecReauthByNewOrder(orderOld, newOrder, order, isOrderCombine, isFront, out apiErrorMessage);
			if (reauthResult == ResultTypes.Success)
			{
				Exec(order, cart, registGuestUser, isFirstCart, isOrderCombine, isExternalPaymentApiSkip: true);
			}

			// 外部決済連携ログ格納処理
			var hasError = (reauthResult != ResultTypes.Success);
			OrderCommon.AppendExternalPaymentCooperationLog(
				hasError == false,
				hasError ? orderOld.OrderId : newOrder.OrderId,
				hasError ? apiErrorMessage : LogCreator.CreateMessage(newOrder.OrderId, ""),
				this.LastChanged,
				UpdateHistoryAction.Insert);

			return reauthResult;
		}

		/// <summary>
		/// 注文実行（注文ID、ユーザーIDは確保してあること）
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="registGuestUser">ゲストを作るか</param>
		/// <param name="isFirstCart">最初のカートか</param>
		/// <returns>注文結果区分</returns>
		public ResultTypes Exec(Hashtable order, CartObject cart, bool registGuestUser, bool isFirstCart)
		{
			return Exec(order, cart, registGuestUser, isFirstCart, false);
		}

		/// <summary>
		/// 注文実行（注文ID、ユーザーIDは確保してあること）
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="registGuestUser">ゲストを作るか</param>
		/// <param name="isFirstCart">最初のカートか</param>
		/// <param name="isOrderCombine">注文同梱されているか</param>
		/// <param name="isRegisterUserWhenOrderComplete">会員登録を注文処理と同時に行うか</param>
		/// <param name="isExternalPaymentApiSkip">外部決済連携をスキップするかどうか</param>
		/// <param name="contentsExecuteLog">Abテスト用コンテンツログモデル(注文実行時)</param>
		/// <returns>注文結果区分</returns>
		public ResultTypes Exec(
			Hashtable order,
			CartObject cart,
			bool registGuestUser,
			bool isFirstCart,
			bool isOrderCombine,
			bool isRegisterUserWhenOrderComplete = false,
			bool isExternalPaymentApiSkip = false,
			ContentsLogModel contentsExecuteLog = null)
		{
			var nextEngineTempOrderSync = new NextEngineTempOrderSync();

			try
			{
				var result = Exec(
					order,
					cart,
					registGuestUser,
					isFirstCart,
					isOrderCombine,
					isRegisterUserWhenOrderComplete,
					isExternalPaymentApiSkip,
					contentsExecuteLog,
					nextEngineTempOrderSync);
				if (result != ResultTypes.Success)
				{
					nextEngineTempOrderSync.SyncTempOrder(this, order, cart);
				}

				return result;
			}
			catch (Exception)
			{
				nextEngineTempOrderSync.SyncTempOrder(this, order, cart);
				throw;
			}
		}

		/// <summary>
		/// 注文実行（注文ID、ユーザーIDは確保してあること）
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="registGuestUser">ゲストを作るか</param>
		/// <param name="isFirstCart">最初のカートか</param>
		/// <param name="isOrderCombine">注文同梱されているか</param>
		/// <param name="isRegisterUserWhenOrderComplete">会員登録を注文処理と同時に行うか</param>
		/// <param name="isExternalPaymentApiSkip">外部決済連携をスキップするかどうか</param>
		/// <param name="contentsExecuteLog">Abテスト用コンテンツログモデル(注文実行時)</param>
		/// <param name="nextEngineTempOrderSync">ネクストエンジン仮注文連携オブジェクト</param>
		/// <returns>注文結果区分</returns>
		/// <remarks>
		/// ネクストエンジンオプションONの場合、
		/// 仮注文によるネクストエンジンとの在庫のズレを対応するため、
		/// 注文実行後、仮注文生成された場合ネクストエンジン側と連携するようにメソッドを分割
		/// </remarks>
		private ResultTypes Exec(
			Hashtable order,
			CartObject cart,
			bool registGuestUser,
			bool isFirstCart,
			bool isOrderCombine,
			bool isRegisterUserWhenOrderComplete,
			bool isExternalPaymentApiSkip,
			ContentsLogModel contentsExecuteLog,
			NextEngineTempOrderSync nextEngineTempOrderSync)
		{
			int shippingNo = 0;

			//-----------------------------------------------------
			// １．仮注文処理（トランザクション）
			//		・ここで登録される情報は、注文戻し処理と整合性が取れている必要があります。
			//------------------------------------------------------
			bool success = true;
			var isOpluxPayment = false;
			try
			{
				NextEngineTempOrderSync.SetNextEngineTempOrderSync(order, nextEngineTempOrderSync);

				if (isExternalPaymentApiSkip == false)
				{
					// 外部決済で決済注文IDの発行が必要の場合、決済注文IDを生成
					order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = (IsPaymentOrderIdIssue(cart.Payment.PaymentId)
						|| ((this.OrderExecType == ExecTypes.Mobile)
							&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
							&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.SBPS)))
						? OrderCommon.CreatePaymentOrderId(cart.ShopId)
						: string.Empty;

					if (cart.Payment.IsPaymentYamatoKaSms)
					{
						order[Constants.FIELD_ORDER_CARD_TRAN_ID] = order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID];
					}
					else if (cart.Payment.IsPaymentPaygentPaidy)
					{
						order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = StringUtility.ToEmpty(cart.Payment.CardTranId);
					}
				}

				cart.Payment.PaymentId = OrderCommon.ConvertAmazonPaymentId(cart.Payment.PaymentId);
				isOpluxPayment = (Constants.OPLUX_PAYMENT_KBN == OPluxConst.OPLUX_PAYMENT_KBN_OTHER)
					|| ((Constants.OPLUX_PAYMENT_KBN == OPluxConst.OPLUX_PAYMENT_KBN_CREDIT)
						&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT));

				// O-PLUX
				if (Constants.OPLUX_ENABLED
					&& (this.OrderExecType != ExecTypes.FixedPurchaseBatch)
					&& isOpluxPayment)
				{
					var nameNormalizationResponseOfOwner = GetNameNormalizationResponse(cart.Owner.Name);
					var nameNormalizationResponseOfShipping = GetNameNormalizationResponse(cart.GetShipping().Name);

					if (CheckAndSetErrorNormalizationResponse(nameNormalizationResponseOfOwner)
						&& CheckAndSetErrorNormalizationResponse(nameNormalizationResponseOfShipping))
					{
						var advCode = order.ContainsKey(Constants.FIELD_ORDER_ADVCODE_NEW)
							? StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ADVCODE_NEW])
							: string.Empty;

						var registerEventResponse = GetRegisterEventResponse(
							cart,
							nameNormalizationResponseOfOwner,
							nameNormalizationResponseOfShipping,
							advCode,
							this.OrderExecType);

						if (registerEventResponse != null)
						{
							if (registerEventResponse.Errors.Error != null)
							{
								foreach (var error in registerEventResponse.Errors.Error)
								{
									this.ErrorMessages.Add(error.Message);
								}
								success = false;
							}
							else
							{
								var orderPaymentMemoMessage = CreateOrderPaymentMemoMessage(registerEventResponse);
								order[Constants.FIELD_ORDER_PAYMENT_MEMO] = OrderExternalPaymentUtility.SetExternalPaymentMemo(
									StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_MEMO]),
									orderPaymentMemoMessage);
								order[Constants.OPLUX_REGISTER_EVENT_API_RESULT] = registerEventResponse.Event.Result;
							}
						}
					}
					else
					{
						success = false;
					}
				}

				if (new OrderPreorderRegister().RegistPreOrder(
					order,
					cart,
					(registGuestUser && isFirstCart),
					this.IsUser,
					isFirstCart,
					out shippingNo,
					this.LastChanged,
					this.OrderExecType,
					UpdateHistoryAction.Insert) == false)
				{
					success = false;
					this.ErrorMessages.Add(
						CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ORDER_INSERT_ERROR));
				}

				order.Add(Constants.FIELD_USERSHIPPING_SHIPPING_NO, shippingNo);
			}
			// 在庫エラー？
			catch (ProductStockException ex)
			{
				success = false;
				if(ex.ProductNames != null)
				{
					var names = string.Join("\r\n", ex.ProductNames.Select(name => "「"+name+"」"));
					this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_MULTIPLE_PRODUCT_NO_STOCK)
						.Replace("@@ 1 @@", names));
				}
				else
				{
					this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PRODUCT_NO_STOCK)
						.Replace("@@ 1 @@", ((ProductStockException)ex).ProductName));
				}
			}
			// シリアルキー引当エラー？
			catch (ReserveSerialKeyException ex)
			{
				success = false;
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PRODUCT_NO_NOSERIAL_KEY)
					.Replace("@@ 1 @@", ((ReserveSerialKeyException)ex).ErrorCartProduct.ProductJointName));
			}
			// カートエラー？
			catch (CartException ex)
			{
				success = false;
				this.ErrorMessages.Add(ex.Message);
			}

			// 注文実行種別が定期購入バッチ（今すぐ注文含む）か
			var isOrderCreatedByFixedPurchase = (this.OrderExecType == ExecTypes.FixedPurchaseBatch);

			if (Constants.OPLUX_ENABLED
				&& (StringUtility.ToEmpty(order[Constants.OPLUX_REGISTER_EVENT_API_RESULT]) == OPluxConst.API_REGISTER_EVENT_RESULT_NG)
				&& isOpluxPayment)
			{
				// Delete the registered member at the time of ordering when there is no order other than the rollback target
				var userExistOrder = new OrderService().GetOrdersByUserId(cart.OrderUserId)
					.Count(item => (item.OrderId != cart.OrderId));
				var isUserDelete = ((registGuestUser || isRegisterUserWhenOrderComplete)
					&& (this.SuccessOrders.Count == 0)
					&& (userExistOrder == 0));

				OrderCommon.RollbackPreOrder(
					order,
					cart,
					isUserDelete,
					shippingNo,
					this.IsUser,
					UpdateHistoryAction.Insert,
					isOrderCreatedByFixedPurchase);

				this.ErrorMessages.Add(CommerceMessages.GetMessages(
					CommerceMessages.ERRMSG_FRONT_OPLUX_REGISTER_ORDER_DID_NOT_PASS_EXAMINATION));

				return ResultTypes.Fail;
			}

			if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE && (success == false))
			{
				var isUserDelete = (isRegisterUserWhenOrderComplete
					&& (this.SuccessOrders.Count == 0)
					&& (new OrderService().GetOrdersByUserId(cart.OrderUserId)
						.Count(item => (item.OrderId != cart.OrderId)) == 0));
				if (isUserDelete)
				{
					using (var accessor = new SqlAccessor())
					{
						accessor.OpenConnection();
						accessor.BeginTransaction();

						new UserService().Delete(
							(string)order[Constants.FIELD_ORDER_USER_ID],
							(string)order[Constants.FIELD_ORDER_LAST_CHANGED],
							UpdateHistoryAction.DoNotInsert,
							accessor);

						accessor.CommitTransaction();
					}
				}
			}

			if (success == false) return ResultTypes.Fail;

			// 外部決済で与信がHOLD時に利用(現在はコンビニ後払い(DSK)のみ利用)
			var isExternalPaymentAuthResultHold = false;

			bool isExternalPayment = isExternalPaymentApiSkip;
			if (isExternalPaymentApiSkip == false)
			{
				//------------------------------------------------------
				// 画面遷移決済のとき
				//	- 注文情報を格納して、ここで処理終了
				//	- 次のループへ進ませる
				//------------------------------------------------------
				if (CheckExternalPayment(order, cart))
				{
					// 注文完了スキップ時の処理
					SkipOrderCompleteProcesses(order, cart);

					return ResultTypes.Skip;
				}

				//------------------------------------------------------
				// ２．外部連携決済処理
				//------------------------------------------------------
				bool needsRollback;

				var paymentRegister = new OrderPaymentRegister(this.Properties);
				try
				{
					// 決済実行
					success = paymentRegister.ExecOrderPayment(
						order,
						cart,
						out needsRollback,
						out isExternalPayment,
						UpdateHistoryAction.DoNotInsert);

					if (success == false)
					{
						if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
						{
							this.LinePayFailedCarts.Add(cart);
						}

						this.ApiErrorMessage = paymentRegister.ApiErrorMessage;

						if (string.IsNullOrEmpty(this.ApiErrorMessage)) this.ApiErrorMessage = "例外が発生しました。";
					}
					else
					{
						if (this.GetType() == typeof(OrderRegisterFixedPurchaseInner))
						{
							this.ApiErrorMessage = "";
						}

						this.GmoTransactionResult = paymentRegister.GmoTransactionResult;
					}
					isExternalPaymentAuthResultHold = paymentRegister.IsAuthResultHold;
				}
				// 仮注文で残るパターンはここに来る
				catch (Exception)
				{
					if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					{
						this.LinePayFailedCarts.Add(cart);
					}

					this.ApiErrorMessage = paymentRegister.ApiErrorMessage;

					// 更新履歴挿入
					new UpdateHistoryService().InsertForOrder((string)order[Constants.FIELD_ORDER_ORDER_ID], this.LastChanged);
					throw;
				}

				// 仮注文情報削除処理(ゲスト削除、ポイント戻しも行う)
				if (needsRollback)
				{
					this.ApiErrorMessage = paymentRegister.ApiErrorMessage;

					this.TransactionName = "2-X.注文ロールバック処理";

					// ロールバック対象以外に注文がない注文時会員登録ユーザーを削除
					var isUserDelete = ((registGuestUser || isRegisterUserWhenOrderComplete)
						&& (this.SuccessOrders.Count == 0)
						&& (new OrderService().GetOrdersByUserId(cart.OrderUserId)
							.Count(item => (item.OrderId != cart.OrderId)) == 0));

					OrderCommon.RollbackPreOrder(
						order,
						cart,
						isUserDelete,
						shippingNo,
						this.IsUser,
						UpdateHistoryAction.Insert,
						isOrderCreatedByFixedPurchase);

					if (Constants.FLAPS_OPTION_ENABLE)
					{
						try
						{
							new FlapsIntegrationFacade().CancelOrder(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								StringUtility.ToEmpty(order[Constants.FLAPS_ORDEREXTENDSETTING_ATTRNO_FOR_POSNOSERNO]));
						}
						catch (Exception exception)
						{
							// FLAPS注文キャンセル処理に失敗しても他の処理を進める
							FileLogger.WriteError(
								string.Format(
									"FLAPS注文キャンセル処理に失敗しました。FLAPS管理画面にて在庫数を変更してください。order_id: {0}",
									(string)order[Constants.FIELD_ORDER_ORDER_ID]),
								exception);
						}
					}
				}
				// 3Dセキュアの場合はスキップ（PCフロントのみ）
				if (success
					&& (this.OrderExecType == ExecTypes.Pc)
					&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
						&& ((this.ZeusCard3DSecurePaymentOrders.Count != 0)
							|| (this.GmoCard3DSecurePaymentOrders.Count != 0)
							|| (this.ZcomCard3DSecurePaymentOrders.Contains(order))
							|| (this.RakutenCard3DSecurePaymentOrders.Contains(order))
							|| (this.VeriTrans3DSecurePaymentOrders.Contains(order))
							|| (this.YamatoCard3DSecurePaymentOrders.Contains(order))
							|| (this.PaygentCard3DSecurePaymentOrders.Contains(order))))
				{
					// GMO3DセキュアまたはヤマトKWCクレジットセキュア2.0またはベリトランスクレジットセキュアの場合
					if (((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo) && Constants.PAYMENT_SETTING_GMO_3DSECURE)
						|| (cart.Payment.IsYamatoKwcCredit3dSecure2)
						|| ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans) && Constants.PAYMENT_VERITRANS4G_CREDIT_3DSECURE))
					{
						new OrderService().Modify(
							cart.OrderId,
							orderTmp =>
							{
								orderTmp.CardTranId = (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID];
								orderTmp.PaymentOrderId = (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID];
								orderTmp.CreditBranchNo = order.Contains(Constants.FIELD_ORDER_CREDIT_BRANCH_NO)
									? (int?)order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO]
									: null;
								orderTmp.PaymentMemo = (string)order[Constants.FIELD_ORDER_PAYMENT_MEMO];
							},
							UpdateHistoryAction.DoNotInsert);
					}

					// 注文完了スキップ時の処理
					SkipOrderCompleteProcesses(order, cart);
					return ResultTypes.Skip;
				}

				if (success == false)
				{
					// 仮注文で注文が残る場合
					if (needsRollback == false)
					{
						new OrderService().UpdateOrderCountForPreOrder(new OrderModel(order), this.LastChanged);
					}

					// 決済失敗時処理
					PaymentFailedProcess();

					// 更新履歴登録
					new UpdateHistoryService().InsertEmptyForOrder(
						(string)order[Constants.FIELD_ORDER_ORDER_ID],
						(string)order[Constants.FIELD_USER_USER_ID],
						this.LastChanged);

					return ResultTypes.Fail;
				}
			}

			// ABテストコンテンツログ(注文実行時)更新
			if (Constants.AB_TEST_OPTION_ENABLED && contentsExecuteLog != null)
			{
				var contentsLog = new ContentsLogModel
				{
					LogNo = contentsExecuteLog.LogNo,
					ReportType = contentsExecuteLog.ReportType,
					AccessKbn = contentsExecuteLog.AccessKbn,
					ContentsId = contentsExecuteLog.ContentsId,
					ContentsType = contentsExecuteLog.ContentsType,
					Price = cart.PriceCartTotalWithoutPaymentPrice,
					OrderId = cart.OrderId
				};
				new ContentsLogService().Update(contentsLog);
			}

			// Get invoice if non invoice left => return error
			if (Constants.TWINVOICE_ENABLED
				&& Constants.TWINVOICE_ORDER_INVOICING
				&& success
				&& cart.Shippings.Any(shipping => shipping.IsShippingAddrTw)
				&& (OrderCommon.InvoiceReleased(order) == false))
			{
				FileLogger.WriteError("Invoice Released Error!");

				return ResultTypes.Fail;
			}

			//------------------------------------------------------
			// ３．注文確定処理
			//	・ここを正常通過すれば何があっても注文完了。
			//------------------------------------------------------
			if (success)
			{
				success = UpdateForOrderComplete(order, cart, isExternalPayment, UpdateHistoryAction.DoNotInsert, isExternalPaymentAuthResultHold);
				if (success == false) return ResultTypes.Fail;

				if ((Constants.PAYMENT_ATM_KBN == PaymentATMKbn.Paygent)
					&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATM))
				{
					DomainFacade.Instance.OrderService.AppendExternalPaymentCooperationLog(
						(string)order[Constants.FIELD_ORDER_ORDER_ID],
						StringUtility.ToEmpty(order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_COOPERATION_LOG]),
						this.LastChanged,
						UpdateHistoryAction.DoNotInsert);
				}

				// 仮注文でECPayと藍新Payが電子発票を発行しないこと
				if (Constants.TWINVOICE_ECPAY_ENABLED
					&& (cart.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& (cart.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
				{
					for (var index = 0; index < cart.Shippings.Count; index++)
					{
						var errorMessage = OrderCommon.EcPayInvoiceReleased(
							StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
							cart.Shippings[index],
							index + 1,
							StringUtility.ToEmpty(order[Constants.FIELD_ORDER_LAST_CHANGED]));
						if (string.IsNullOrEmpty(errorMessage) == false)
						{
							success = false;
							order[OrderCommon.ECPAY_INVOICE_API_MESSAGE] = errorMessage;
						}
					}
				}

				// Update Skipped Count FixedPurchase
				new FixedPurchaseService().ClearSkippedCount((string)order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]);
			}

			//------------------------------------------------------
			// ４．後処理
			//------------------------------------------------------
			// 注文完了時の処理
			string alertMessage = OrderCompleteProcesses(order, cart, UpdateHistoryAction.DoNotInsert);
			if (alertMessage != "") this.AlertMessages.Add(alertMessage);

			// 注文完了後の処理（更新履歴とともに）
			AfterOrderCompleteProcesses(order, cart, UpdateHistoryAction.Insert);

			if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
			{
				var landingCartSessionKey = StringUtility.ToEmpty(order["w2cFront_landing_cart_session_key"]);
				cart.IsLandingUseNewebPay = (string.IsNullOrEmpty(landingCartSessionKey) == false);
			}

			// ABテストコンテンツログ(注文完了時ログ)作成
			if (Constants.AB_TEST_OPTION_ENABLED && contentsExecuteLog != null)
			{
				var contentsLog = new ContentsLogModel
				{
					OrderId = cart.OrderId,
					Price = cart.PriceCartTotalWithoutPaymentPrice,
					ReportType = Constants.FLG_CONTENTSLOG_REPORT_TYPE_ODRCOMPCV,
					ContentsType = contentsExecuteLog.ContentsType,
					ContentsId = contentsExecuteLog.ContentsId,
					AccessKbn = contentsExecuteLog.AccessKbn,
					Date = DateTime.Now
				};
				new ContentsLogService().Insert(contentsLog);
			}

			this.SuccessOrders.Add(order);
			this.SuccessCarts.Add(cart);    // 成功した注文のカートはすぐ削除したいが、ループの中なのであとで削除する

			cart.IsOrderDone = true;    // 注文完了フラグを立てる

			// Update FixedPurchaseMemberFlg by settings
			// JugmentCondition payment Status is Complete
			if (success && Constants.FIXEDPURCHASE_MEMBER_CONDITION_INCLUDES_ORDER_PAYMENT_STATUS_COMPLETE)
			{
				success = UpdateFixedPurchaseMemberFlgBySetting(order, cart, this.LastChanged, UpdateHistoryAction.DoNotInsert);
				if (success == false) return ResultTypes.Fail;
			}

			// Set order extend for O-PLUX
			if (success
				&& Constants.OPLUX_ENABLED
				&& (string.IsNullOrEmpty(Constants.OPLUX_REVIEW_EXTEND_NO) == false)
				&& (StringUtility.ToEmpty(order[Constants.OPLUX_REGISTER_EVENT_API_RESULT]) == OPluxConst.API_REGISTER_EVENT_RESULT_REVIEW)
				&& isOpluxPayment)
			{
				new OrderService().UpdateOrderExtendStatus(
					StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
					int.Parse(Constants.OPLUX_REVIEW_EXTEND_NO),
					Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON,
					DateTime.Now,
					StringUtility.ToEmpty(order[Constants.FIELD_ORDER_LAST_CHANGED]),
					UpdateHistoryAction.DoNotInsert);

				if (Constants.FIXEDPURCHASE_OPTION_ENABLED
					&& cart.HasFixedPurchase)
				{
					new FixedPurchaseService().UpdateExtendStatus(
						StringUtility.ToEmpty(order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]),
						int.Parse(Constants.OPLUX_REVIEW_EXTEND_NO),
						Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON,
						DateTime.Now,
						StringUtility.ToEmpty(order[Constants.FIELD_ORDER_LAST_CHANGED]),
						UpdateHistoryAction.DoNotInsert);
				}
			}

			// 更新履歴挿入
			new UpdateHistoryService().InsertAllForOrder((string)order[Constants.FIELD_ORDER_ORDER_ID], this.LastChanged);

			return ResultTypes.Success;
		}

		/// <summary>
		/// 注文完了メール(ネクストエンジン連携)
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		public void SendOrderMailByNextEngine(Hashtable order, CartObject cart)
		{
			if (Constants.NE_OPTION_ENABLED && Constants.NE_REALATION_TEMP_ORDER)
			{
				// 注文完了メールを送信
				SendOrderMailToOperator(order, cart, this.IsUser);
			}
		}

		/// <summary>
		/// 新しい受注から再与信をする
		/// </summary>
		/// <param name="orderOld">古い受注</param>
		/// <param name="orderNew">新しい受注</param>
		/// <param name="order">受注情報</param>
		/// <param name="isOrderCombine">同梱かどうか</param>
		/// <param name="isFront">フロントかどうか</param>
		/// <param name="apiErrorMessage">APIエラーメッセージ</param>
		/// <returns></returns>
		protected ResultTypes ExecReauthByNewOrder(
			OrderModel orderOld,
			OrderModel orderNew,
			Hashtable order,
			bool isOrderCombine,
			bool isFront,
			out string apiErrorMessage)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				// 外部決済連携実行
				var reauth = new ReauthCreatorFacade(orderOld, orderNew, ReauthCreatorFacade.ExecuteTypes.System, ReauthCreatorFacade.OrderActionTypes.Modify, sqlAccessor)
					.CreateReauth();
				var reauthResult = reauth.Execute();

				var service = new OrderService();
				apiErrorMessage = reauthResult.ApiErrorMessages;

				// 与信のみに失敗している場合エラー画面へ
				if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure)
				{
					if (reauth.AuthLostForError)
					{
						service.UpdateExternalPaymentInfoForAuthError(
							orderOld.OrderId,
							reauthResult.ErrorMessages,
							orderOld.LastChanged,
							UpdateHistoryAction.Insert,
							sqlAccessor);
					}
				}
				else
				{
					order[Constants.FIELD_ORDER_PAYMENT_MEMO] = OrderExternalPaymentUtility.SetExternalPaymentMemo(
						StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_MEMO]),
							reauthResult.PaymentMemo,
							isOrderCombine);
					order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_ERROR_MESSAGE] = isOrderCombine
						? reauthResult.ErrorMessages
						: string.Join(Environment.NewLine, orderOld.ExternalPaymentErrorMessage, reauthResult.ErrorMessages);
					order[Constants.FIELD_ORDER_CARD_TRAN_ID] = reauthResult.CardTranId;
					order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = reauthResult.PaymentOrderId;

					order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE] = reauth.GetUpdateReauthDate(
						orderOld.ExternalPaymentAuthDate,
						orderOld.OrderPaymentKbn,
						orderNew.OrderPaymentKbn);
					order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS] = reauth.HasSales
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_COMP
						: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
					order[Constants.FLG_ORDER_PAYMENT_API_SKIP] = true;

					service.UpdateExternalPaymentStatusAndMemoForReauthByNewOrder(
						orderOld,
						this.LastChanged,
						(string)order[Constants.FIELD_ORDER_ORDER_ID],
						UpdateHistoryAction.DoNotInsert,
						isOrderCombine,
						sqlAccessor);
				}

				sqlAccessor.CommitTransaction();

				if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure)
				{
					var displayErrorMessage = isFront
						? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_REAUTH_ALERT)
						: reauthResult.ErrorMessages;
					this.ErrorMessages.Add(displayErrorMessage);
					return ResultTypes.Fail;
				}
			}

			return ResultTypes.Success;
		}

		/// <summary>
		/// 外部決済かどうかチェック
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>外部決済か</returns>
		protected abstract bool CheckExternalPayment(Hashtable order, CartObject cart);

		/// <summary>
		/// 外部決済で決済注文ID発行が必要かどうか
		/// </summary>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>true: 発行必要、false: 発行不要</returns>
		protected virtual bool IsPaymentOrderIdIssue(string paymentId)
		{
			return Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans;
		}

		/// <summary>
		/// 注文完了時の処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <returns>アラート文言</returns>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public abstract string OrderCompleteProcesses(Hashtable order, CartObject cart, UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 注文完了後の処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public abstract void AfterOrderCompleteProcesses(Hashtable order, CartObject cart, UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 注文完了スキップ時の処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		public abstract void SkipOrderCompleteProcesses(Hashtable order, CartObject cart);

		/// <summary>
		/// 決済失敗時処理
		/// </summary>
		protected virtual void PaymentFailedProcess()
		{
		}

		/// <summary>
		/// 支払回数更新処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>結果</returns>
		public bool UpdateOrderInstallmentsCode(Hashtable order, UpdateHistoryAction updateHistoryAction)
		{
			this.TransactionName = "支払回数更新";

			try
			{
				if (OrderCommon.UpdateOrderInstallmentsCode(
					(string)order[Constants.FIELD_ORDER_ORDER_ID],
					(string)order[Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE],
					(string)order[Constants.FIELD_ORDER_CARD_INSTRUMENTS], updateHistoryAction) <= 0)
				{
					throw new Exception("支払回数更新処理に失敗しました");
				}
			}
			catch (Exception ex)
			{
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ORDER_STATUS_UPUDATE_ERROR));
				AppLogger.WriteError(OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, null, ""), ex);
				return false;
			}

			return true;
		}

		/// <summary>
		/// 注文ステータス更新
		/// </summary>
		/// <param name="htOrder">注文情報</param>
		/// <param name="coCart">カート情報</param>
		/// <param name="isExternalPayment">外部決済あり？</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="isExternalPaymentAuthResultHold">外部決済で与信結果がHOLDなのか</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		public bool UpdateForOrderComplete(
			Hashtable htOrder,
			CartObject coCart,
			bool isExternalPayment,
			UpdateHistoryAction updateHistoryAction,
			bool isExternalPaymentAuthResultHold = false,
			SqlAccessor accessor = null)
		{
			this.TransactionName = "3-1注文確定処理";

			try
			{
				if (OrderCommon.UpdateForOrderComplete(htOrder, coCart, isExternalPayment, updateHistoryAction, isExternalPaymentAuthResultHold, accessor) <= 0)
				{
					throw new Exception("注文確定処理に失敗しました");
				}
			}
			catch (Exception ex)
			{
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ORDER_STATUS_UPUDATE_ERROR));
				AppLogger.WriteError(OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, htOrder, coCart, ""), ex);
				return false;
			}

			return true;
		}

		/// <summary>
		/// 注文メール送信処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="isUser">会員か</param>
		/// <param name="isRecommend">レコメンド機能による注文登録処理のメール送信か</param>
		/// <param name="isSendFixedPurchaseMailToUser">定期注文登録時にユーザーに注文メール＆定期購入エラーメール送信するか</param>
		public void SendOrderMails(Hashtable order, CartObject cart, bool isUser, bool isRecommend = false, bool isSendFixedPurchaseMailToUser = true)
		{
			try
			{
				this.TransactionName = "4-2-1.注文者宛メール送信処理";
				if (isSendFixedPurchaseMailToUser)
				{
					if (Constants.MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED)
					{
						if (StringUtility.ToEmpty(cart.Owner.MailAddr) != "") SendOrderMailToUser(order, cart, isUser, true);
						if (StringUtility.ToEmpty(cart.Owner.MailAddr2) != "") SendOrderMailToUser(order, cart, isUser, false);
					}
					// 注文実行種別によって判断
					else
					{
						switch (this.OrderExecType)
						{
							case ExecTypes.Pc:
								SendOrderMailToUser(order, cart, isUser, true);
								break;

							case ExecTypes.Mobile:
								SendOrderMailToUser(order, cart, isUser, false);
								break;

							case ExecTypes.FixedPurchaseBatch:
							case ExecTypes.CommerceManager:	// ※管理画面は現状メールを送らない
								// モバイル以外はPC優先
								if ((string)order[Constants.FIELD_ORDER_ORDER_KBN] != Constants.FLG_ORDER_ORDER_KBN_MOBILE)
								{
									if (cart.Owner.MailAddr != "") SendOrderMailToUser(order, cart, isUser, true);
									else if (cart.Owner.MailAddr2 != "") SendOrderMailToUser(order, cart, isUser, false);
								}
								else
								{
									if (cart.Owner.MailAddr2 != "") SendOrderMailToUser(order, cart, isUser, false);
									else if (cart.Owner.MailAddr != "") SendOrderMailToUser(order, cart, isUser, true);
								}
								break;
						}
					}
				}

				// 管理者用メール送信
				if (IsSendOrderMailToOperator(order, this.OrderExecType, this.Properties.OrderCompleteEmailSenderType))
				{
					this.TransactionName = "4-2-2.メール送信処理(管理者向け)";

					SendOrderMailToOperator(order, cart, isUser);
				}
			}
			catch (Exception ex)
			{
				AppLogger.WriteWarn(OrderCommon.CreateOrderSuccessAlertLogMessage(this.TransactionName, order, cart), ex);
			}
		}

		/// <summary>
		/// 管理者メール送信判定
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="execType">実行種別</param>
		/// <param name="senderType">送信者種別</param>
		/// <returns>判定結果</returns>
		private static bool IsSendOrderMailToOperator(Hashtable order, ExecTypes execType, Constants.EnabledOrderCompleteEmailSenderType senderType)
		{
			if (NextEngineTempOrderSync.IsTempOrderSyncOptionEnabled == false)
			{
				return IsExecTypeInEnabledListForOrderCompleteEmail(execType, senderType);
			};

			// NOTE:ネクストエンジンとの仮注文連携は管理者メール送信を利用しているので、仮注文連携済みの場合は二重送信しないようにする
			var isSynchronizedNextEngineTempOrder = NextEngineTempOrderSync.GetNextEngineTempOrderSync(order).IsSynchronized;
			var result = (isSynchronizedNextEngineTempOrder == false) && Constants.THANKSMAIL_FOR_OPERATOR_ENABLED;
			return result;
		}

		/// <summary>
		/// 管理者向け注文完了メール送信オプションで設定された実行元かどうか
		/// </summary>
		/// <param name="execType">実行種別</param>
		/// <param name="senderType">送信者種別</param>
		/// <returns></returns>
		private static bool IsExecTypeInEnabledListForOrderCompleteEmail(ExecTypes execType, Constants.EnabledOrderCompleteEmailSenderType senderType)
		{
			switch (execType)
			{
				// 定期バッチ（フロント/ 管理画面の今すぐ注文含む）
				case ExecTypes.FixedPurchaseBatch:
					if (senderType == Constants.EnabledOrderCompleteEmailSenderType.Batch)
					{
						return Constants.SEND_ORDER_COMPLETE_EMAIL_FOR_OPERATOR_ENABLED_LIST.Contains(EnabledOrderCompleteEmailSenderType.Batch.ToString());
					}
					else if (senderType == Constants.EnabledOrderCompleteEmailSenderType.Front)
					{
						goto case ExecTypes.Pc;
					}
					else if (senderType == Constants.EnabledOrderCompleteEmailSenderType.Manager)
					{
						goto case ExecTypes.CommerceManager;
					}
					throw new w2Exception("管理者向け注文完了メール送信者区分「" + senderType + "」は定義されていません。");

				// フロントからの注文（定期今すぐ注文は除く）
				case ExecTypes.Pc :
				case ExecTypes.Mobile :
				case ExecTypes.External:
					return Constants.SEND_ORDER_COMPLETE_EMAIL_FOR_OPERATOR_ENABLED_LIST.Contains(EnabledOrderCompleteEmailSenderType.Front.ToString());

				// 管理画面からの注文（定期今すぐ注文は除く）
				case ExecTypes.CommerceManager:
					return Constants.SEND_ORDER_COMPLETE_EMAIL_FOR_OPERATOR_ENABLED_LIST.Contains(EnabledOrderCompleteEmailSenderType.Manager.ToString());

				default:
					throw new w2Exception("実行区分「" + execType + "」は定義されていません。");
			}
		}

		/// <summary>
		/// 注文メール送信処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="isUser">会員か</param>
		/// <param name="targetPcMail">PCメール向けか</param>
		/// <param name="isRecommend">レコメンド機能による注文登録処理のメール送信か</param>
		private void SendOrderMailToUser(Hashtable order, CartObject cart, bool isUser, bool targetPcMail, bool isRecommend = false)
		{
			var mailAddr = targetPcMail ? cart.Owner.MailAddr : cart.Owner.MailAddr2;
			var mailTemplate = isRecommend
				? Constants.CONST_MAIL_ID_ORDER_UPDATE_FOR_USER
				: Constants.CONST_MAIL_ID_ORDER_COMPLETE;

			var input = new MailTemplateDataCreaterByCartAndOrder(targetPcMail).GetOrderMailDatas(order, cart, isUser);
			input[Constants.FIELD_ORDER_MEMO] = ((string)input[Constants.FIELD_ORDER_MEMO]).Trim();

			using (MailSendUtility mailSender = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				mailTemplate,
				(string)order[Constants.FIELD_ORDER_USER_ID],
				input,
				targetPcMail,
				Constants.MailSendMethod.Auto,
				cart.Owner.DispLanguageCode,
				cart.Owner.DispLanguageLocaleId,
				StringUtility.ToEmpty(mailAddr)))
			{
				mailSender.AddTo(StringUtility.ToEmpty(mailAddr));
				if (mailSender.SendMail() == false) throw new Exception("メール送信処理に失敗しました。", mailSender.MailSendException);
			}
		}

		/// <summary>
		/// 注文完了情報をLINEに連携
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		public void SendOrderCompleteToLine(Hashtable order, CartObject cart)
		{
			if ((Constants.REPEATLINE_OPTION_ENABLED != Constants.RepeatLineOption.CooperationAndMessaging)
				&& (CheckLineColumnExists((string)order[Constants.FIELD_ORDER_USER_ID]) == false)) return;

			var uri = string.Empty;

			switch (this.OrderExecType)
			{
				case ExecTypes.Pc:
					uri = HttpContext.Current.Request.Url.Scheme
						+ Uri.SchemeDelimiter
						+ Constants.SITE_DOMAIN
						+ new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL)
							.AddParam(REQUEST_KEY_ORDER_ID, cart.OrderId)
							.CreateUrl();
					break;

				case ExecTypes.CommerceManager:
				case ExecTypes.FixedPurchaseBatch:
					uri = Constants.PROTOCOL_HTTPS
						+ Constants.SITE_DOMAIN
						+ new UrlCreator(Constants.PATH_ROOT_FRONT_PC + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL)
							.AddParam(REQUEST_KEY_ORDER_ID, cart.OrderId)
							.CreateUrl();
					break;
			}

			var input = new LineMessagePost
			{
				TemplateId = Line.Constants.LINE_API_TEMPLATE_ID,
				TelNo = string.Format(
					"{0}{1}{2}{3}",
					"+81",
					cart.Shippings[0].Tel1_1.TrimStart('0'),
					cart.Shippings[0].Tel1_2.Trim(),
					cart.Shippings[0].Tel1_3.Trim()),
				Value = new OrderParam
				{
					OrderNumber = cart.OrderId,
					OrderedDate = DateTime.Now.ToString("yyyy/MM/dd"),
					ShippingDate = string.IsNullOrEmpty(string.Format("{0:yyyy/MM/dd}", cart.Shippings[0].ShippingDate))
						? null
						: string.Format("{0:yyyy/MM/dd}", cart.Shippings[0].ShippingDate),
					TotalAmount = CurrencyManager.ToPrice(cart.PriceSubtotal),
					Uri = uri,
					Item = cart.Items.Where(item => item.OrderHistoryDisplayType == Constants.FLG_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE_VALID).Select(
						item => new OrderItemParam
						{
							ItemName = item.ProductJointName,
							Quantity = item.CountSingle.ToString(),
							UnitPrice = CurrencyManager.ToPrice(item.Price),
						}).Take(10).ToArray(),
				},
			};

			SendLineMessageApiServer.SendOrderComplete(input);
		}

		/// <summary>
		/// ユーザ拡張項目IDの列が存在するか
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>true:存在する</returns>
		private bool CheckLineColumnExists(string userId)
		{
			var extender = new UserService().GetUserExtend(userId);
			var result = ((extender != null)
				&& extender.UserExtendDataValue.ContainsKey(Constants.SOCIAL_PROVIDER_ID_LINE)
				&& (string.IsNullOrEmpty((string)extender.UserExtendDataValue[Constants.SOCIAL_PROVIDER_ID_LINE]) == false));
			return result;
		}

		/// <summary>
		/// 注文完了時会員登録メール送信
		/// </summary>
		/// <param name="mail">メール情報</param>
		protected void SendUserRegisterMail(Hashtable mail)
		{
			using (var mailSend = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_USER_REGIST,
				(string)mail[Constants.FIELD_USER_USER_ID],
				mail,
				true,
				Constants.MailSendMethod.Auto,
				(string)mail[Constants.FIELD_USER_DISP_LANGUAGE_CODE],
				(string)mail[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID],
				mail[Constants.FIELD_USER_MAIL_ADDR].ToString()))
			{
				mailSend.AddTo(mail[Constants.FIELD_USER_MAIL_ADDR].ToString());

				if (mailSend.SendMail() == false)
				{
					AppLogger.WriteError(this.GetType().BaseType + " : " + mailSend.MailSendException);
				}
			}
		}

		/// <summary>
		/// 管理者向けメール送信処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="isUser">会員か</param>
		/// <param name="isRecommend">レコメンド機能による注文登録処理のメール送信か</param>
		public void SendOrderMailToOperator(Hashtable order, CartObject cart, bool isUser, bool isRecommend = false)
		{
			var targetPcMail = (this.OrderExecType != ExecTypes.Mobile);
			var mailTemplate = isRecommend
				? Constants.CONST_MAIL_ID_ORDER_UPDATE_FOR_OPERATOR
				: Constants.CONST_MAIL_ID_ORDER_COMPLETE_FOR_OPERATOR;

			if (Constants.GIFTORDER_OPTION_ENABLED && Constants.NE_OPTION_ENABLED && cart.IsGift)
			{
				var shippingNo = 0;
				foreach (var cartShipping in cart.Shippings)
				{
					var input = new MailTemplateDataCreaterByCartAndOrder(false).GetOrderMailDatas(order, cart, isUser, shippingNo, isToNextEngine:true);
					using (MailSendUtility mailSender = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, mailTemplate, "", input, targetPcMail, Constants.MailSendMethod.Auto))
					{
						// Toが設定されている場合にのみメール送信
						if (mailSender.TmpTo != "")
						{
							if (mailSender.SendMail() == false) throw new Exception("メール送信処理に失敗しました。", mailSender.MailSendException);
						}
					}

					shippingNo++;
				}
			}
			else
			{
				var input = new MailTemplateDataCreaterByCartAndOrder(false).GetOrderMailDatas(order, cart, isUser);
				using (MailSendUtility mailSender = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, mailTemplate, "", input, targetPcMail, Constants.MailSendMethod.Auto))
				{
					// Toが設定されている場合にのみメール送信
					if (mailSender.TmpTo != "")
					{
						if (mailSender.SendMail() == false) throw new Exception("メール送信処理に失敗しました。", mailSender.MailSendException);
					}
				}
			}
		}

		/// <summary>
		/// クレジット登録確定
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="isUser">会員か</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>アラートメッセージ</returns>
		public string UpdateUserCreditCard(
			Hashtable order,
			CartObject cart,
			bool isUser,
			UpdateHistoryAction updateHistoryAction)
		{
			StringBuilder dispAlertMessages = new StringBuilder();

			if (isUser && OrderCommon.CreditCardRegistable && cart.Payment.UserCreditCardRegistFlg)
			{
				var userId = (string)order[Constants.FIELD_ORDER_USER_ID];
				try
				{
					// カート情報が登録可能ならユーザークレジットカードテーブル更新
					if (OrderCommon.GetCreditCardRegistable(isUser, userId))
					{
						if (new UserCreditCardService().UpdateDispFlg(
							userId,
							cart.Payment.UserCreditCard.BranchNo,
							true,
							Constants.FLG_LASTCHANGED_USER,
							updateHistoryAction) == false)
						{
							throw new Exception(this.TransactionName + "に失敗しました");
						}
					}
				}
				catch (Exception ex)
				{
					dispAlertMessages.Append(
						CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CREDITCARD_REGIST_DETERMINE_ERROR));

					PaymentFileLogger.WritePaymentLog(
						false,
						cart.Payment.PaymentId,
						PaymentFileLogger.PaymentType.Unknown,
						PaymentFileLogger.PaymentProcessingType.CreditRegistrationConfirmationProcessing,
						BaseLogger.CreateExceptionMessage(OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart), ex),
						new Dictionary<string, string>
						{
							{Constants.FIELD_ORDER_ORDER_ID, cart.OrderId},
							{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
							{Constants.FIELD_USER_USER_ID, (string)order[Constants.FIELD_ORDER_USER_ID]}
						});
				}
			}

			return dispAlertMessages.ToString();
		}

		/// <summary>
		/// カート削除
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <returns>アラートメッセージ</returns>
		protected string DeleteCart(Hashtable order, CartObject cart)
		{
			if (OrderCommon.DeleteCart(cart.CartId) <= 0)
			{
				AppLogger.WriteError(OrderCommon.CreateOrderSuccessAlertLogMessage(this.TransactionName, order, cart));
				return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ORDER_DELETE_CART_DB_ALERT);
			}
			return "";
		}

		/// <summary>
		/// 仮登録の定期台帳を通常に更新
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateFixedPurchaseStatusTempToNormal(Hashtable order, UpdateHistoryAction updateHistoryAction)
		{
			new FixedPurchaseService()
				.UpdateFixedPurchaseStatusTempToNormal(
					(string)order[Constants.FIELD_ORDER_ORDER_ID],
					(string)order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID],
					this.LastChanged,
					updateHistoryAction);
		}

		/// <summary>
		/// Update FixedPurchaseMemberFlg By Settings
		/// </summary>
		/// <param name="order">Infor Order</param>
		/// <param name="cart">Info Cart</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <param name="updateHistoryAction">Action</param>
		/// <param name="sqlAccessor">SqlAccessor</param>
		/// <returns>True: update is sucess, False: update is failed</returns>
		public static bool UpdateFixedPurchaseMemberFlgBySetting(
			Hashtable order,
			CartObject cart,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			// Check fixedPurchaseMember and order has FixedPurchase
			if ((cart.IsFixedPurchaseMember == false) && cart.HasFixedPurchase)
			{
				// Update FixedPurchaseMemberFlg By Settings
				if ((string)order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE)
				{
					return new UserService().UpdateFixedPurchaseMemberFlg(
						(string)order[Constants.FIELD_ORDER_USER_ID],
						Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON,
						lastChanged,
						updateHistoryAction);
				}
			}
			return true;
		}

		/// <summary>
		/// Get name normalization response
		/// </summary>
		/// <param name="name">Name</param>
		/// <returns>Name normalization response</returns>
		public static NameNormalizationResponse GetNameNormalizationResponse(string name)
		{
			var urlParameters = NameNormalizationUtility.CreateUrlParameters(name);
			for (int index = 0; index < 3; index++)
			{
				var nameNormalizationResponse = NameNormalizationApiFacade.CallApi(urlParameters);
				if (nameNormalizationResponse != null) return nameNormalizationResponse;
			}

			return null;
		}

		/// <summary>
		/// Check and set error normalization response
		/// </summary>
		/// <param name="nameNormalizationResponse">Name normalization response</param>
		/// <returns>True if not error response : false</returns>
		private bool CheckAndSetErrorNormalizationResponse(NameNormalizationResponse nameNormalizationResponse)
		{
			if (nameNormalizationResponse == null)
			{
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ORDER_INSERT_ERROR));
				return false;
			}

			if (nameNormalizationResponse.ResponseObject.Result != OPluxConst.API_NAME_NORMALIZATION_RESULT_OK)
			{
				this.ErrorMessages.Add(nameNormalizationResponse.ResponseObject.Errors.Error.Message);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Get register event response
		/// </summary>
		/// <param name="cart">Cart</param>
		/// <param name="nameNormalizationResponseOfOwner">Name normalization response of owner</param>
		/// <param name="nameNormalizationResponseOfShipping">Name normalization response of shipping</param>
		/// <param name="advCode">Adv code</param>
		/// <param name="orderExecType">実行タイプ</param>
		/// <returns>Register event response</returns>
		public static RegisterEventResponse GetRegisterEventResponse(
			CartObject cart,
			NameNormalizationResponse nameNormalizationResponseOfOwner,
			NameNormalizationResponse nameNormalizationResponseOfShipping,
			string advCode,
			ExecTypes orderExecType)
		{
			var registerEventRequest = OPluxUtility.CreateRegisterEventRequest(
				cart,
				nameNormalizationResponseOfOwner.ResponseObject,
				nameNormalizationResponseOfShipping.ResponseObject,
				advCode,
				orderExecType);
			var registerEventRequestAdapter = new RegisterEventRequestAdapter(registerEventRequest);

			for (int index = 0; index < 3; index++)
			{
				var registerEventResponse = registerEventRequestAdapter.Execute();
				if (registerEventResponse != null) return registerEventResponse;
			}

			return null;
		}

		/// <summary>
		/// Create order payment memo message
		/// </summary>
		/// <param name="response">Register event response</param>
		/// <returns>Order payment memo message</returns>
		public static string CreateOrderPaymentMemoMessage(RegisterEventResponse response)
		{
			var memoMessage = new StringBuilder()
				.Append("審査結果：")
				.Append(response.Event.Result)
				.Append(", イベントID：")
				.Append(response.Event.Id)
				.Append(", 発動ルール：[PAID_MOBMAIL] 支払い済みの過去イベントを、携帯電話メールアドレスをキーとして検索し、一致件数が閾値以上の場合に発動。");

			if ((response.Event.Rules.Rule != null)
				&& (response.Event.Rules.Rule.Length > 1))
			{
				memoMessage.Append("(他あり)");
			}

			return StringUtility.ToEmpty(memoMessage);
		}

		/// <summary>
		/// 注文ステータス更新
		/// </summary>
		/// <param name="htOrder">注文情報</param>
		/// <param name="coCart">カート情報</param>
		/// <param name="isExternalPayment">外部決済あり？</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか</returns>
		public bool UpdateOrderStatus(
			Hashtable htOrder,
			CartObject coCart,
			bool isExternalPayment,
			UpdateHistoryAction updateHistoryAction)
		{
			this.TransactionName = "3-1注文確定処理";

			try
			{
				if (OrderCommon.UpdateOrderStatus(
					htOrder,
					coCart,
					isExternalPayment,
					updateHistoryAction) <= 0)
				{
					throw new Exception("注文確定処理に失敗しました");
				}
			}
			catch (Exception ex)
			{
				this.ErrorMessages.Add(CommerceMessages
					.GetMessages(CommerceMessages.ERRMSG_FRONT_ORDER_STATUS_UPUDATE_ERROR));
				AppLogger.WriteError(
					OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						htOrder,
						coCart),
					ex);
				return false;
			}
			return true;
		}

		/// <summary>
		/// 頒布会の次回配送商品を変更する
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <param name="order">注文情報</param>
		protected void UpdateNextSubscriptionBoxProduct(CartObject cart, Hashtable order)
		{
			// 頒布会が含まれない、もしくは注文同梱時は新規頒布会がない場合は処理を抜ける
			var fixedPurchaseId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]);
			if ((cart.HasSubscriptionBox == false)
				|| (cart.IsOrderCombined
					&& (cart.IsShouldRegistSubscriptionForCombine == false))
				|| string.IsNullOrEmpty(fixedPurchaseId)) return;

			// 次回配送商品の更新を行う
			var nextDate = cart.Shippings.Select(d => d.NextShippingDate).First();
			var fixedPurchaseContainer = new FixedPurchaseService().GetContainer(fixedPurchaseId);
			var getNextProductsResult = new SubscriptionBoxService().GetFixedPurchaseNextProduct(
				fixedPurchaseContainer.SubscriptionBoxCourseId,
				fixedPurchaseContainer.FixedPurchaseId,
				fixedPurchaseContainer.MemberRankId,
				nextDate,
				subscriptionBoxOrderCount: fixedPurchaseContainer.SubscriptionBoxOrderCount + 1,
				shipping: fixedPurchaseContainer.Shippings[0]);

			new FixedPurchaseService().UpdateNextDeliveryForSubscriptionBox(
				fixedPurchaseId,
				this.LastChanged,
				Constants.W2MP_POINT_OPTION_ENABLED,
				getNextProductsResult,
				UpdateHistoryAction.DoNotInsert);

			// 次回商品が1つも取得できなかった場合はログに記録する。
			if (getNextProductsResult.Result == SubscriptionBoxGetNextProductsResult.ResultTypes.Fail)
			{
				var errorMessage = CommerceMessages
					.GetMessages(CommerceMessages.ERRMSG_FRONT_SUBSCRIPTION_BOX_NO_NEXT_PRODUCT)
					.Replace("@@ 1 @@", fixedPurchaseContainer.FixedPurchaseId).Replace(
						"@@ 2 @@",
						fixedPurchaseContainer.SubscriptionBoxCourseId);
				FileLogger.WriteError(errorMessage);
			}

			// 条件を満たす定期台帳の定期購入ステータスを完了にする。
			var isComplete = false;
			var subscriptionBox =
				new SubscriptionBoxService().GetByCourseId(fixedPurchaseContainer.SubscriptionBoxCourseId);
			switch (subscriptionBox.IsNumberTime)
			{
				// 回数指定
				case true:
					// 自動繰り返し設定または無期限設定フラグがONの場合は処理を抜ける
					if (subscriptionBox.IsAutoRenewal || subscriptionBox.IsIndefinitePeriod) break;

					// 注文回数が満期になったものは定期購入ステータスを完了にする
					var maxCount = subscriptionBox.DefaultOrderProducts.Max(defaultItem => defaultItem.Count).Value;
					if (fixedPurchaseContainer.SubscriptionBoxOrderCount >= maxCount) isComplete = true;
					break;

				// 期間指定
				case false:
					// 次回配送日がデフォルト商品配送期間の最終日より後の時に定期購入ステータスを完了にする
					var lastDate = subscriptionBox.DefaultOrderProducts.Max(dp => dp.TermUntil);
					if ((lastDate != null) && (lastDate < fixedPurchaseContainer.NextShippingDate))
						isComplete = true;
					break;
			}

			if (isComplete == false) return;

			new FixedPurchaseService().Complete(
				fixedPurchaseContainer.FixedPurchaseId,
				this.LastChanged,
				fixedPurchaseContainer.NextShippingDate,
				fixedPurchaseContainer.NextNextShippingDate,
				UpdateHistoryAction.Insert,
				accessor: null);
		}

		/// <summary>
		/// ペイジェント3Dセキュア注文を抽出
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>ペイジェント3Dセキュア注文</returns>
		public Hashtable GetPaygent3DSecurOrderByOrderId(string orderId)
		{
			var result = this.PaygentCard3DSecurePaymentOrders.FirstOrDefault(
				order => (string)order[Constants.FIELD_ORDER_ORDER_ID] == orderId);
			return result ?? new Hashtable();
		}

		/// <summary>トランザクション名</summary>
		protected string TransactionName { get; set; }
		/// <summary>注文登録プロパティ</summary>
		private OrderRegisterProperties Properties { get; set; }
		/// <summary>注文実行種別</summary>
		private ExecTypes OrderExecType { get { return this.Properties.OrderExecType; } }
		/// <summary>ユーザーか</summary>
		protected bool IsUser { get { return this.Properties.IsUser; } }
		/// <summary>エラーメッセージ</summary>
		public List<string> ErrorMessages { get { return this.Properties.ErrorMessages; } }
		/// <summary>アラートメッセージ</summary>
		public List<string> AlertMessages { get { return this.Properties.AlertMessages; } }
		/// <summary>ゼウス3Dセキュア注文</summary>
		public List<Hashtable> ZeusCard3DSecurePaymentOrders
		{
			get { return this.Properties.ZeusCard3DSecurePaymentOrders; }
			set { this.Properties.ZeusCard3DSecurePaymentOrders = value; }
		}
		/// <summary>楽天3Dセキュア注文</summary>
		public List<Hashtable> RakutenCard3DSecurePaymentOrders
		{
			get { return this.Properties.RakutenCard3DSecurePaymentOrders; }
			set { this.Properties.RakutenCard3DSecurePaymentOrders = value; }
		}
		/// <summary>成功注文</summary>
		public List<Hashtable> SuccessOrders
		{
			get { return this.Properties.SuccessOrders; }
			set { this.Properties.SuccessOrders = value; }
		}
		/// <summary>成功カート</summary>
		public List<CartObject> SuccessCarts { get { return this.Properties.SuccessCarts; } }
		/// <summary>DB更新時の最終更新者</summary>
		public string LastChanged { get { return this.Properties.LastChanged; } }
		/// <summary>apiのエラーメッセージ保持用</summary>
		public string ApiErrorMessage { get; private set; }
		/// <summary>LINE Pay failed carts</summary>
		public List<CartObject> LinePayFailedCarts
		{
			get { return this.Properties.LinePayFailedCarts; }
		}
		/// <summary>Zcom card 3DSecure payment orders</summary>
		public List<Hashtable> ZcomCard3DSecurePaymentOrders
		{
			get { return this.Properties.ZcomCard3DSecurePaymentOrders; }
			set { this.Properties.ZcomCard3DSecurePaymentOrders = value; }
		}
		/// <summary>ベリトランス3Dセキュア注文情報</summary>
		public List<Hashtable> VeriTrans3DSecurePaymentOrders
		{
			get { return this.Properties.VeriTrans3DSecurePaymentOrders; }
			set { this.Properties.VeriTrans3DSecurePaymentOrders = value; }
		}
		/// <summary>GMO3Dセキュア注文</summary>
		public List<Hashtable> GmoCard3DSecurePaymentOrders
		{
			get { return this.Properties.GmoCard3DSecurePaymentOrders; }
			set { this.Properties.GmoCard3DSecurePaymentOrders = value; }
		}
		/// <summary>GMO承認結果</summary>
		public string GmoTransactionResult { get; private set; }
		/// <summary>ヤマトKWC3Dセキュア注文</summary>
		public List<Hashtable> YamatoCard3DSecurePaymentOrders
		{
			get { return this.Properties.YamatoCard3DSecurePaymentOrders; }
			set { this.Properties.YamatoCard3DSecurePaymentOrders = value; }
		}
		/// <summary>YamatoKwcクレジットが最後の注文から1年以上たち期限切れになっているか</summary>
		public bool IsExpiredYamatoKwcCredit 
		{
			get { return this.Properties.IsExpiredYamatoKwcCredit; }
			set { this.Properties.IsExpiredYamatoKwcCredit = value; }
		}
		/// <summary> ペイジェントクレジット3Dセキュア注文</summary>
		public List<Hashtable> PaygentCard3DSecurePaymentOrders
		{
			get { return this.Properties.PaygentCard3DSecurePaymentOrders; }
			set { this.Properties.PaygentCard3DSecurePaymentOrders = value; }
		}
	}
}
