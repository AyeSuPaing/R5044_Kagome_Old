/*
=========================================================================================================
  Module      : ステータス更新後処理(ProcessAfterUpdateOrderStatus.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using w2.App.Common.CrossPoint.Helper;
using w2.App.Common.CrossPoint.Point;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Flaps;
using w2.App.Common.Input.Order;
using w2.App.Common.User;
using w2.Common.Logger;
using w2.Domain.FixedPurchase;
using w2.Domain.Point;
using w2.Domain.SerialKey;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain;
using w2.Common.Sql;
using w2.App.Common.Order.Payment.Atobaraicom.Invoice;
using w2.App.Common.Order.Payment.DSKDeferred.GetInvoice;
using w2.App.Common.Order.Payment.JACCS.ATODENE.GetInvoice;
using w2.App.Common.Order.Payment.JACCS.ATODENE;
using w2.App.Common.Order.Payment;
using w2.Common.Util;
using w2.Domain.InvoiceAtobaraicom;
using w2.Domain.InvoiceDskDeferred;
using w2.App.Common.Order.Import;
using w2.App.Common.Order.Payment.Score;
using w2.App.Common.Order.Payment.Score.GetInvoice;
using w2.Common.Helper;
using w2.Domain.Order;
using w2.Domain.Score;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Payment.Veritrans.ObjectElement;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ステータス更新後処理
	/// </summary>
	public class ProcessAfterUpdateOrderStatus
	{
		/// <summary>
		/// ステータス更新
		/// </summary>
		/// <param name="statement">ステートメント</param>
		/// <param name="accessor">SQLアクセッサー</param>
		/// <param name="input">入力値</param>
		/// <returns>件数</returns>
		public int ModifyOrderStatus(string statement, SqlAccessor accessor, Hashtable input)
		{
			// ステータス更新実行
			using (SqlStatement SqlStatement = new SqlStatement("Order", statement))
			{
				var orderStatusUpdated = SqlStatement.ExecStatement(accessor, input);
				return orderStatusUpdated;
			}
		}

		/// <summary>
		/// 注文ステータス更新による請求書印字処理
		/// </summary>
		/// <param name="orderInput">受注情報</param>
		/// <param name="updateStatus">ステータス種別</param>
		/// <param name="orderStatus">ステータス値</param>
		/// <param name="accessor">SQLアクセッサー</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns></returns>
		public string UpdatedInvoiceByOrderStatus(
			OrderInput orderInput,
			Constants.StatusType updateStatus,
			string orderStatus,
			SqlAccessor accessor,
			string lastChanged = "")
		{
			// 請求書印字データを取得
			// NP後払いは請求書印字データを取得しない
			// 印字データが取得できた場合にのみステータス更新するようにする
			var orderPaymentKbn = orderInput.OrderPaymentKbn;
			if ((updateStatus == Constants.StatusType.Order)
				&& (orderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED)
				&& OrderCommon.IsInvoiceBundleServiceUsable(orderPaymentKbn)
				&& (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
			{
				switch (Constants.PAYMENT_CVS_DEF_KBN)
				{
					case Constants.PaymentCvsDef.Atodene:
						if (orderInput.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF) break;

						// Atodeneの場合の印字データ取得処理
						var responseAtodene = new AtodeneGetInvoiceModelAdapter(orderInput.CreateModel()).Execute();
						if (responseAtodene.Result != AtodeneConst.RESULT_OK)
						{
							var errorMessage = ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_GET_INVOICE_ERROR)
								.Replace(
									"@@ 1 @@",
									string.Join(
										"\r\n",
										responseAtodene.Errors.Error
										.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage))
									.ToArray()));
							return errorMessage;

						}
						// 印字データの登録
						responseAtodene.InsertInvoice(orderInput.OrderId, accessor);
						break;

					case Constants.PaymentCvsDef.Dsk:
						if ((orderInput.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF)
							|| (new InvoiceDskDeferredService().Get(orderInput.OrderId, accessor) != null))
						{
							break;
						}

						var dskAdapter = new DskDeferredGetInvoiceAdapter(orderInput.CreateModel());
						var responseDsk = dskAdapter.Execute();
						if (responseDsk.IsResultOk == false)
						{
							var errorMessage = ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_GET_INVOICE_ERROR)
								.Replace(
									"@@ 1 @@",
									string.Join(
										"\r\n",
										responseDsk.Errors.Error
										.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage))
									.ToArray()));
							return errorMessage;
						}
						dskAdapter.InsertInvoice(orderInput.OrderId, responseDsk);
						break;

					case Constants.PaymentCvsDef.Atobaraicom:
						if (Constants.PAYMENT_SETTING_ATOBARAICOM_USE_INVOICE_SYSTEM_SERVICE == false) break;

						// 印刷キューリクエスト
						var request = new InvoiceRequest.TransferPrintQueueRequest(orderInput.PaymentOrderId, orderInput.InvoiceBundleFlg);
						var responseApi = InvoiceApiFacade.CallApi<InvoiceResponse.TransferPrintQueueResponse>(
							Constants.PAYMENT_ATOBARAICOM_TRANSFER_PRINT_QUEUE_APIURL,
							request.BillingObject);

						if (responseApi.ResultsObject.ResultObject.ExecResult != InvoiceConstants.API_RESULT_OK)
						{
							var errorMessage = (responseApi.ResultsObject.ResultObject.ExecResult == InvoiceConstants.API_RESULT_ERROR)
								? string.Format(
									"{0}: {1}",
									responseApi.ResultsObject.ResultObject.ErrorObject.ErrorCode,
									responseApi.ResultsObject.ResultObject.ErrorObject.ErrorBody)
								: string.Empty;
							var error = ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_GET_INVOICE_ERROR)
								.Replace("@@ 1 @@", string.Join("\r\n", errorMessage));
							return errorMessage;
						}

						var orderInvoiceModel = new InvoiceAtobaraicomModel
						{
							OrderId = orderInput.OrderId,
						};

						// 印刷キュー取得
						var requestQue = new InvoiceRequest.GetListTargetInvoiceRequest();
						var responseQue = InvoiceApiFacade.CallApi<InvoiceResponse.GetListTargetInvoiceResponse>(
							Constants.PAYMENT_ATOBARAICOM_GET_LIST_TARGET_INVOICE_APIURL,
							requestQue.BillingObject);

						if ((responseQue.Status == InvoiceConstants.API_RESULT_STATUS_OK)
							&& (responseQue.ResultsObject.ResultObject != null))
						{
							// Perform insert/update invoice before export
							foreach (var item in responseQue.ResultsObject.ResultObject)
							{
								var model = new InvoiceAtobaraicomModel
								{
									OrderId = item.Ent_OrderId,
									UseAmount = int.Parse(item.UseAmount),
									TaxAmount = item.TaxAmount,
									LimitDate = DateTime.Parse(item.LimitDate),
									NameKj = item.NameKj,
									CvBarcodeData = item.Cv_BarcodeData,
									CvBarcodeString1 = item.Cv_BarcodeString1,
									CvBarcodeString2 = item.Cv_BarcodeString2,
									YuMtOcrCode1 = item.Yu_MtOcrCode1,
									YuMtOcrCode2 = item.Yu_MtOcrCode2,
									YuAccountName = item.Yu_SubscriberName,
									YuAccountNo = item.Yu_AccountNumber,
									YuLoadKind = item.Yu_ChargeClass,
									CvsCompanyName = item.Cv_ReceiptAgentName,
									CvsUserName = item.Cv_SubscriberName,
									BkCode = item.Bk_BankCode,
									BkBranchCode = item.Bk_BranchCode,
									BkName = item.Bk_BankName,
									BkBranchName = item.Bk_BranchName,
									BkAccountKind = item.Bk_DepositClass,
									BkAccountNo = item.Bk_AccountNumber,
									BkAccountName = item.Bk_AccountHolder,
									BkAccountKana = item.Bk_AccountHolderKn,
									MypagePwd = item.MypagePassword,
									MypageUrl = item.MypageUrl,
									CreditDeadline = StringUtility.ToEmpty(item.CreditLimitDate),
								};

								DomainFacade.Instance.InvoiceAtobaraicomService.InsertUpdateInvoiceAtobaraicom(model, accessor);
							}
						}

						// 請求書発行処理実行
						var requestInv = new InvoiceRequest.InvoiceProcessExecuteRequest(orderInput.PaymentOrderId);
						var responseInv = InvoiceApiFacade.CallApi<InvoiceResponse.InvoiceProcessExecuteResponse>(
							Constants.PAYMENT_ATOBARAICOM_INVOICE_PROCESS_EXECUTE_APIURL,
							requestInv.BillingObject);

						if (responseInv.ResultsObject.ResultObject.ExecResult != InvoiceConstants.API_RESULT_OK)
						{
							var errorMessage = (responseInv.ResultsObject.ResultObject.ExecResult == InvoiceConstants.API_RESULT_ERROR)
								? string.Format(
									"{0}: {1}",
									responseInv.ResultsObject.ResultObject.ErrorObject.ErrorCode,
									responseInv.ResultsObject.ResultObject.ErrorObject.ErrorBody)
								: string.Empty;
							var error = ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_GET_INVOICE_ERROR)
								.Replace("@@ 1 @@", string.Join("\r\n", errorMessage));
							return errorMessage;
						}
						break;

					case Constants.PaymentCvsDef.Score:
						var scoreErrorMessage = string.Empty;
						var invoiceScoreService = new InvoiceScoreService();
						var orderService = new OrderService();
						var externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_INV_COMP;

						if ((Constants.PAYMENT_SETTING_SCORE_DEFERRED_USE_INVOICE_BUNDLE == false)
							|| (orderInput.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF))
						{
							break;
						}

						try
						{
							var getInvoiceRequest = new ScoreGetInvoiceRequest(orderInput);
							var getInvoiceResponse = new ScoreApiFacade().GetInvoiceResult(getInvoiceRequest);

							if (getInvoiceResponse.Result != ScoreResult.Ok.ToText())
							{
								orderStatus = orderInput.OrderStatus;
								externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_INV_ERROR;
								scoreErrorMessage = ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_GET_INVOICE_ERROR)
									.Replace(
										"@@ 1 @@",
										string.Join(
											"\r\n",
											getInvoiceResponse.Errors.ErrorList
											.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage))
										.ToArray()));
							}
							else
							{
								// 請求書印字データを登録
								invoiceScoreService.Insert(model: getInvoiceResponse.CreateModel(orderInput.OrderId), accessor);
							}
						}
						catch (Exception e)
						{
							// HTTP例外・エラーになった際は請求書印字情報取得エラーに更新し、例外をハンドリングし、エラーメッセージを返す
							orderStatus = orderInput.OrderStatus;
							externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_INV_ERROR;
							scoreErrorMessage = ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_GET_INVOICE_ERROR)
								.Replace("@@ 1 @@", e.Message);
						}
						finally
						{
							// 請求書印字情報取得ステータス更新
							orderService.UpdateOrderStatusAndExternalPaymentStatus(
								orderId: orderInput.OrderId,
								orderStatus: orderStatus,
								externalPaymentStatus: externalPaymentStatus,
								externalPaymentAuthDate: DateTime.Parse(orderInput.ExternalPaymentAuthDate),
								updateDate: DateTime.Now,
								lastChanged: lastChanged,
								updateHistoryAction: UpdateHistoryAction.DoNotInsert,
								accessor: accessor
							);

							// 決済連携メモ追記
							orderService.AddPaymentMemo(
								orderId: orderInput.OrderId,
								addMemoString: OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
									paymentOrderId: string.IsNullOrEmpty(orderInput.PaymentOrderId) ? orderInput.OrderId : orderInput.PaymentOrderId,
									paymentId: orderInput.OrderPaymentKbn,
									cardTranId: orderInput.CardTranId,
									actionNameWithoutPaymentName: Constants.ACTION_NAME_INVOICE_REPORT,
									lastBilledAmount: orderInput.LastBilledAmount.ToPriceDecimal()),
								lastChanged: lastChanged,
								updateHistoryAction: UpdateHistoryAction.Insert,
								accessor: accessor);

							accessor?.CommitTransaction();
						}
						return scoreErrorMessage;

					case Constants.PaymentCvsDef.Veritrans:
						var response = new PaymentVeritransCvsDef().GetInvoiceData(orderInput.PaymentOrderId);

						if (response.Mstatus != VeriTransConst.RESULT_STATUS_OK)
						{
							var errorMessage = string.Format("{0},{1}", response.VResultCode, response.MerrMsg);
							return errorMessage;
						}

						var invoiceVeritransModel = new InvoiceElement(response).CreateModel(orderInput.OrderId, orderInput.LastChanged);
						DomainFacade.Instance.InvoiceVeritransService.InsertUpdate(invoiceVeritransModel);
						break;
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// 受注ステータス更新による定期台帳更新処理
		/// </summary>
		/// <param name="orderStatusUpdated">更新件数</param>
		/// <param name="updateStatus">ステータス更新種別</param>
		/// <param name="orderStatus">ステータス値</param>
		/// <param name="orderInput">受注情報</param>
		/// <param name="loginOperatorName">オペレーター名</param>
		/// <param name="accessor">SQLアクセッサー</param>
		public void UpdateFixedPurchaseByOrderStatus(
			int orderStatusUpdated,
			Constants.StatusType updateStatus,
			string orderStatus,
			OrderInput orderInput,
			string loginOperatorName,
			SqlAccessor accessor)
		{
			// ステータス更新有りかつ注文ステータス更新かつ定期購入OP有効
			if ((orderStatusUpdated > 0)
				&& (updateStatus == Constants.StatusType.Order)
				&& (Constants.FIXEDPURCHASE_OPTION_ENABLED))
			{
				// 注文済みに更新の場合
				if (orderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDERED)
				{
					// 仮登録を通常に更新（更新履歴とともに）
					new FixedPurchaseService().UpdateFixedPurchaseStatusTempToNormal(
						orderInput.OrderId,
						orderInput.FixedPurchaseId,
						loginOperatorName,
						UpdateHistoryAction.Insert,
						accessor);
				}

				// 仮注文キャンセルに更新の場合
				if (orderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED)
				{
					// 仮登録を仮登録キャンセルに更新（更新履歴とともに）
					new FixedPurchaseService().CancelTemporaryRegistrationFixedPurchase(
						orderInput.FixedPurchaseId,
						loginOperatorName,
						UpdateHistoryAction.Insert,
						accessor,
						orderInput.OrderId);
				}
			}

			// 注文情報出荷完了（or 配送完了）処理
			if ((orderStatusUpdated > 0)
				&& (updateStatus == Constants.StatusType.Order)
				&& ((orderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
					|| (orderStatus == Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP)))
			{
				// 定期購入情報更新
				var order = orderInput.CreateModel();
				OrderCommon.UpdateFixedPurchaseShippedCount(order, loginOperatorName, UpdateHistoryAction.DoNotInsert, accessor);

				// FLAPS連携済みフラグをオンに更新
				if (Constants.FLAPS_OPTION_ENABLE 
					&& (order.ExtendStatus43 == Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF))
				{
					new FlapsIntegrationFacade().TurnOnErpIntegrationFlg(
						order.OrderId,
						loginOperatorName,
						accessor);
				}
			}
		}

		/// <summary>
		/// 受注情報ステータス更新による仮ポイントから本ポイント更新処理
		/// </summary>
		/// <param name="orderInput">受注情報</param>
		/// <param name="updateStatus">ステータス更新種別</param>
		/// <param name="orderStatus">ステータス値</param>
		/// <param name="accessor">SQLアクセッサー</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="loginOperatorName">オペレーター名</param>
		public void UpdateTempPointToRealPointByOrderStatus(
			OrderInput orderInput,
			Constants.StatusType updateStatus,
			string orderStatus,
			SqlAccessor accessor,
			string userId,
			string orderId,
			string loginOperatorName)
		{
			// 仮ポイント→本ポイント更新処理
			// ポイントオプションが有効 かつ 注文本ポイント自動付与？
			if (Constants.W2MP_POINT_OPTION_ENABLED
				&& Constants.GRANT_ORDER_POINT_AUTOMATICALLY)
			{
				if ((updateStatus == Constants.StatusType.Order)
					&& (orderStatus == Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_SHIP_COMP))
				{
					// 仮ポイントあり？
					if (OrderCommon.CheckPointTypeTemp(userId, orderId))
					{
						if (Constants.CROSS_POINT_OPTION_ENABLED)
						{
							var pointApiInput = new PointApiInput()
							{
								MemberId = orderInput.UserId,
								OrderId = orderInput.OrderId,
								UserCode = Constants.FLG_LASTCHANGED_BATCH
							};

							var point = new CrossPointPointApiService()
								.Grant(pointApiInput.GetParam(PointApiInput.RequestType.Grant));
							if (point.IsSuccess == false)
							{
								var error = ErrorHelper.CreateCrossPointApiError(point.ErrorMessage, orderInput.UserId);
								FileLogger.WriteError(error);
							}
							else
							{
								// Convert temporary points into real points
								new PointService().TempToRealPoint(
									orderInput.UserId,
									orderInput.OrderId,
									loginOperatorName,
									UpdateHistoryAction.DoNotInsert,
									accessor);

								var user = DomainFacade.Instance.UserService.Get(orderInput.UserId, accessor);
								UserUtility.AdjustPointAndMemberRankByCrossPointApi(user, accessor);
							}
						}
						else
						{
							// Convert temporary points into real points
							new PointService().TempToRealPoint(
							orderInput.UserId,
							orderInput.OrderId,
							loginOperatorName,
							UpdateHistoryAction.DoNotInsert,
							accessor);
						}
					}
				}
			}
		}

		/// <summary>
		///ステータス更新によるシリアルキー割り当て
		/// </summary>
		/// <param name="isUpdatedOrderStatusOrPaymentStatus">注文ステータスまたは入金ステータス更新か</param>
		/// <param name="updatedOrderCount">更新件数</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="digitalContentsFlg">デジタルコンテンツフラグ</param>
		/// <param name="orderStatusAfterUpdate">更新後の注文ステータス</param>
		/// <param name="paymentStatusAfterUpdate">更新後の入金ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセッサ</param>
		public void DeliverSerialKeyByUpdateStatus(
			bool isUpdatedOrderStatusOrPaymentStatus,
			int updatedOrderCount,
			string orderId,
			string digitalContentsFlg,
			string orderStatusAfterUpdate,
			string paymentStatusAfterUpdate,
			string lastChanged,
			SqlAccessor accessor)
		{
			// 商品がデジタルコンテンツでない、または、注文更新がない、または注文ステータス・入金ステータス更新でない場合、何もしない
			if ((digitalContentsFlg != Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON)
				|| (updatedOrderCount == 0)
				|| (isUpdatedOrderStatusOrPaymentStatus == false))
			{
				return;
			}

			// 注文ステータス「出荷完了」「配送完了」かつ入金ステータス「入金済み」であればキーを引き渡す
			if ((paymentStatusAfterUpdate == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE)
				&& ((orderStatusAfterUpdate == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
					|| (orderStatusAfterUpdate == Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP)))
			{
				new SerialKeyService().DeliverSerialKey(orderId, lastChanged, accessor);
			}
		}

		/// <summary>
		/// 入金ステータス更新による定期購入会員判定条件支払い
		/// </summary>
		/// <param name="updateStatus">ステータス更新種別</param>
		/// <param name="orderStatus">ステータス値</param>
		/// <param name="orderInput">受注情報</param>
		/// <param name="loginOperatorName">オペレータ名</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセッサー</param>
		public void UpdateFixPurChaseMemberFlgByPaymentStatus(
			Constants.StatusType updateStatus,
			string orderStatus,
			OrderInput orderInput,
			string loginOperatorName,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// Update FixPurChaseMemberFlg by settings in project.config (FixedPurchaseMember_JudgmentCondition_PaymentStatusComplete)
			if (Constants.FIXEDPURCHASE_MEMBER_CONDITION_INCLUDES_ORDER_PAYMENT_STATUS_COMPLETE
				&& (updateStatus == Constants.StatusType.Payment)
				&& (orderStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE)
				&& orderInput.IsFixedPurchaseOrder)
			{
				new UserService().UpdateFixedPurchaseMemberFlg(
					orderInput.UserId,
					Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON,
					loginOperatorName,
					updateHistoryAction,
					accessor);
			}
		}
	}
}
