/*
=========================================================================================================
  Module      : 注文情報編集確認ページ処理(OrderModifyConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Input.Order;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Atobaraicom;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Payment.Paypay;
using w2.App.Common.Order.Reauth;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.Coupon;
using w2.Domain.DeliveryCompany;
using w2.Domain.DeliveryCompany.Helper;
using w2.Domain.FixedPurchaseRepeatAnalysis;
using w2.Domain.InvoiceDskDeferred;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.Point.Helper;
using w2.Domain.ShopShipping;
using w2.Domain.TwOrderInvoice;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.Common.Web;
using System.Web.UI;
using w2.App.Common.Order.Payment.PayTg;
using w2.Common.Extensions;
using w2.App.Common.Order.Payment.Rakuten;

public partial class Form_Order_OrderModifyConfirm : OrderPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// 処理区分チェック
			CheckActionStatus();

			// 各プロパティセット
			SetProperty();

			// 支払金額合計チェック
			CheckOrderPriceTotal();

			// コンポーネントに値セット
			SetValueToComponents();

			// アラートが必要な配送種別エラーがあるか確認
			IsShippingError();

			// アラートが必要な配送エリアか確認
			var unavailableShippingZip = GetUnavailableShippingZip(this.ShopShipping.ShippingId, this.DeliveryCompany[0].DeliveryCompanyId);
			var shippingZip = this.OrderInput.Shippings[0].HyphenlessShippingZip;
			this.IsUnavailableShippingArea = OrderCommon.CheckUnavailableShippingArea(unavailableShippingZip, shippingZip);
			if (this.IsUnavailableShippingArea)
			{
				trOrderErrorMessagesTitle.Visible = trUnavailableShippingAreaErrorMessages.Visible = true;
				lUnavailableShippingAreaErrorMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_UNAVAILABLE_SHIPPING_AREA_ERROR);
			}

			if (this.TwOrderInvoiceInput != null)
			{
				// Set Visible For Uniform Option
				SetVisibleForUniformOption(this.TwOrderInvoiceInput.TwUniformInvoice);
			}

			// Check can use point for purchase
			CheckCanUsePointForPurchase();
		}
		// 注文商品チェック
		// ※更新ボタンクリックイベント時にもチェックが実行
		var errorMessages = new StringBuilder();
		errorMessages.Append(CheckOrderItem());
		if (((string)Session[Constants.SESSION_KEY_ORDER_REGIST_INPUT_ERROR]).Length != 0)
		{
			errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_REGIST_INPUT_ERROR));
		}
		if (errorMessages.Length != 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		this.ProductOrderLmitOrderIds = new string[0];
		// 商品購入制限チェック（類似配送先を含む）
		if (Constants.PRODUCT_ORDER_LIMIT_ENABLED)
		{
			CheckProductOrderLimit();
		}
	}

	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		if (this.IsUpdateShippingConvenience)
		{
			UpdateOrderShipping(UpdateHistoryAction.DoNotInsert, null);
			SetSessionOrderId(this.OrderInput.OrderId);

			Response.Redirect(
				CreateOrderDetailUrl(
					this.OrderInput.OrderId,
					false,
					((string)Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_RELOAD_PARENT_WINDOW] != Constants.KBN_RELOAD_PARENT_WINDOW_OFF)));
		}

		// 注文済み注文取得（ロールバックする）
		var orderNew = UpdateOrder(UpdateHistoryAction.DoNotInsert);

		// 再与信可能なときのクレジットカード登録処理
		var isCreditCardRegisterd = false;
		var isCreditCardUpdated = false;
		if (Constants.PAYMENT_REAUTH_ENABLED
			&& this.OrderInput.IsPermitReauthOrderSiteKbn
			&& ((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				|| (orderNew.OrderPaymentKbn == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID)))
		{
			int? creditBranchNo = null;
			// 仮クレジットカードへ変更
			var isChangeToProvisionalCreditCard = (this.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus
				&& (orderNew.OrderPaymentKbn == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID)
				&& (this.OrderOld.OrderPaymentKbn != Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID));
			if (isChangeToProvisionalCreditCard)
			{
				var userCreditCard = new ProvisionalCreditCardProcessor().RegisterUnregisterdCreditCard(
					orderNew.UserId,
					this.OrderCreditCard.RegisterCardName,
					Constants.FLG_USERCREDITCARD_REGISTER_ACTION_KBN_ORDER_MODIFY,
					"",
					this.OrderOld.OrderStatus,
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert);
				creditBranchNo = userCreditCard.BranchNo;

				// 注文情報を仮注文にする
				this.OrderInput.OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_TEMP;
			}
			// 通常のクレジットカードで注文変更
			else if (this.OrderCreditCard.CreditBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
			{
				var result = RegisterUserCreditCard(
					orderNew.UserId,
					this.OrderCreditCard,
					this.OrderCreditCard.DoRegister,
					UpdateHistoryAction.DoNotInsert);

				var isSuccess = ((result != null) && result.Success);

				OrderCommon.AppendExternalPaymentCooperationLog(
					isSuccess,
					this.OrderInput.OrderId,
					isSuccess
						? LogCreator.CreateMessage(orderNew.OrderId, orderNew.PaymentOrderId)
						: (result == null)
							? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CREDIT_CARD_PAYMENT_FAILED)
							: result.ErrorMessage,
					this.LoginOperatorName,
					UpdateHistoryAction.Insert);

				if (isSuccess == false)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CREDIT_CARD_PAYMENT_FAILED);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}
				else
				{
					creditBranchNo = result.BranchNo;
				}
			}

			if (creditBranchNo.HasValue) orderNew.CreditBranchNo = creditBranchNo.Value;

			isCreditCardUpdated = (creditBranchNo.HasValue == false) && (this.OrderCreditCard != null)
				&& ((this.OrderOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					|| (this.OrderOld.CreditBranchNo.HasValue == false)
					|| ((this.OrderOld.CreditBranchNo.Value.ToString()) != this.OrderCreditCard.CreditBranchNo));
			if (isCreditCardUpdated) orderNew.CreditBranchNo = int.Parse(this.OrderCreditCard.CreditBranchNo);
			isCreditCardRegisterd = creditBranchNo.HasValue;
		}
		// ペイパル再与信のときはユーザークレジットカード枝番をセットする
		else if (Constants.PAYMENT_REAUTH_ENABLED
			&& this.OrderInput.IsPermitReauthOrderSiteKbn
			&& (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL))
		{
			if (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
			{
				orderNew.CreditBranchNo = this.OrderOld.CreditBranchNo;
			}
			else
			{
				var userCreditCard = PayPalUtility.Payment.RegisterAsUserCreditCard(
					this.User.UserId,
					new PayPalCooperationInfo(this.User.UserExtend),
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert);
				orderNew.CreditBranchNo = userCreditCard.BranchNo;
			}
		}

		// 外部決済連携実行（仮クレジットカードの場合はスキップ）
		var isExecuteExternalPayment = ExecuteExternalPayment(this.OrderOld, orderNew, UpdateHistoryAction.DoNotInsert);

		// Exec gmo cvs payment type
		var errorMessageLog = ExecGmoCvsPaymentType(this.OrderOld, orderNew);
		if (string.IsNullOrEmpty(errorMessageLog) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessageLog.Replace(",", "<br/>");
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 注文情報更新（更新履歴とともに）
		this.ExecuteUpdateOrderAndRegisterUpdateHistory(
			this.OrderOld,
			orderNew,
			isCreditCardRegisterd,
			isCreditCardUpdated,
			isExecuteExternalPayment,
			UpdateHistoryAction.Insert);

		// 出荷情報登録連携
		var deliveryCompany = this.DeliveryCompanyList.First(i => i.DeliveryCompanyId == this.OrderInput.Shippings[0].DeliveryCompanyId);
		foreach (var shipping in this.OrderInput.Shippings.Where(s => s.ExternalShipmentEntry))
		{
			var errorMessage = OrderCommon.ShipmentEntry(
				this.OrderInput.OrderId,
				this.OrderInput.PaymentOrderId,
				this.OrderInput.OrderPaymentKbn,
				shipping.ShippingCheckNo,
				shipping.OldShippingCheckNo,
				this.OrderInput.CardTranId,
				DeliveryCompanyUtil.GetDeliveryCompanyType(deliveryCompany.DeliveryCompanyId, this.OrderInput.OrderPaymentKbn),
				UpdateHistoryAction.DoNotInsert);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				this.Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
				this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}

		// Update External Payment Status To Sales Complete
		if (Constants.PAYMENT_PAYPAYOPTION_ENABLED
			&& (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY))
		{
			var isRefundAfterSalsesFixed = false;
			switch (Constants.PAYMENT_PAYPAY_KBN)
			{
				case Constants.PaymentPayPayKbn.GMO:
					var order = OrderCommon.GetLastAuthOrder(this.OrderInput.OrderId);
					var externalPaymentStatus = new PaypayGmoFacade().GetTransaction(order);
					isRefundAfterSalsesFixed = externalPaymentStatus.Status == Statuses.Captured;
					break;

				case Constants.PaymentPayPayKbn.SBPS:
				case Constants.PaymentPayPayKbn.VeriTrans:
					isRefundAfterSalsesFixed = this.IsSucceedRefundAfterSalsesFixedInPaypay;
					break;

				default:
					break;
			}
			if (isRefundAfterSalsesFixed)
			{
				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();

					var orderService = new OrderService();

					// 外部決済ステータス更新
					orderService.UpdateExternalPaymentInfo(
						orderNew.OrderId,
						Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND,
						true,
						DateTime.Now,
						null,
						this.LoginOperatorName,
						UpdateHistoryAction.DoNotInsert,
						accessor);

					// オンライン決済ステータス更新
					orderService.UpdateOnlinePaymentStatus(
						orderNew.OrderId,
						Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED,
						this.LoginOperatorName,
						UpdateHistoryAction.DoNotInsert,
						accessor);

					accessor.CommitTransaction();
				}
			}
		}

		if ((Constants.GLOBAL_OPTION_ENABLE == false)
			&& (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			&& (Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
			&& (orderNew.LastBilledAmount == 0))
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var orderService = DomainFacade.Instance.OrderService;
				orderService.UpdateOnlinePaymentStatus(
					orderNew.OrderId,
					Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED,
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				orderService.UpdateOrderStatus(
					orderNew.OrderId,
					Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED,
					DateTime.Now,
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				accessor.CommitTransaction();
			}
		}

		//------------------------------------------------------
		// 受注情報詳細へ遷移
		//------------------------------------------------------
		// 注文IDのセッション退避
		SetSessionOrderId(this.OrderInput.OrderId);

		// 受注情報詳細ページへ
		Response.Redirect(
			CreateOrderDetailUrl(
				this.OrderInput.OrderId,
				false,
				((string)Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_RELOAD_PARENT_WINDOW] != Constants.KBN_RELOAD_PARENT_WINDOW_OFF)));
	}

	/// <summary>
	/// 注文済情報取得（ロールバックする）
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <remarks>実際にはDB更新は行わない</remarks>
	/// <returns>注文情報（変更後）</returns>
	private OrderModel UpdateOrder(UpdateHistoryAction updateHistoryAction)
	{
		OrderModel result = null;
		string transactionName = "";

		// 以下の実施
		// DB更新が正しく行えるかチェック
		// 注文情報取得（変更後）
		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();

			try
			{
				this.ExecuteUpdateOrder(accessor, out transactionName, updateHistoryAction);
				result = new OrderService().Get(this.OrderInput.OrderId, accessor);
			}
			catch (Exception ex)
			{
				// ログの記録をしておく
				AppLogger.WriteError(ex);

				// トランザクションロールバック
				accessor.RollbackTransaction();

				// エラー画面へ
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_UPDATE_FAILED).Replace("@@ 1 @@", transactionName);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
			finally
			{
				// トランザクションロールバック
				accessor.RollbackTransaction();
			}
		}

		return result;
	}

	/// <summary>
	/// 外部決済連携実行
	/// </summary>
	/// <param name="orderOld">注文情報（変更前）</param>
	/// <param name="orderNew">注文情報（変更後）</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <returns>外部決済連携実行したか？</returns>
	private bool ExecuteExternalPayment(OrderModel orderOld, OrderModel orderNew, UpdateHistoryAction updateHistoryAction)
	{
		if (Constants.PAYMENT_REAUTH_ENABLED)
		{
			// 外部連携実行
			var reauth =
				new ReauthCreatorFacade(
					orderOld,
					orderNew,
					this.OldExecuteType,
					this.NewExecuteType,
					ReauthCreatorFacade.OrderActionTypes.Modify).CreateReauth();//この時点で何をExecuteするのかが決まっているReauthExecuterが作られる

			if ((orderOld.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP)
				&& (orderOld.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_COMP)
				&& ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (orderOld.OrderPaymentKbn == orderNew.OrderPaymentKbn))
				&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atobaraicom))
			{
				var requestAPIModifyAllItem = new AtobaraicomModifyOrderApi();
				var responseModifyAllItem = requestAPIModifyAllItem.ExecModifyOrderAllItem(orderNew);

				if (requestAPIModifyAllItem.IsAuthorizeHold)
				{
					this.OrderInput.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST;
				}
				else if (requestAPIModifyAllItem.IsAuthorizeNG)
				{
					this.OrderInput.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_ERROR;
				}
				else if (requestAPIModifyAllItem.Status == AtobaraicomConstants.ATOBARAICOM_API_RESULT_STATUS_ERROR)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = StringUtility.ChangeToBrTag(requestAPIModifyAllItem.Messages);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}

				var paymentMemo = OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
					orderNew.PaymentOrderId,
					this.OrderInput.OrderPaymentKbn,
					this.OrderInput.CardTranId,
					AtobaraicomConstants.ATOBARAICOM_PAYMENT_MEMO_TEXT_AUTH_CONFIRMED,
					orderNew.OrderPriceTotal);

				this.OrderInput.PaymentMemo += string.Format("\r\n{0}", paymentMemo);

				return responseModifyAllItem;
			}

			var reauthResult = reauth.Execute();

			var isResultDetailSuccess = reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success;
			string externalApiLog;

			if (isResultDetailSuccess == false)
			{
				externalApiLog = reauthResult.ApiErrorMessages;
			}
			else if ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
				&& (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY))
			{
				string cancelMessage = orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
					? PaygentConstants.PAYGENT_PAIDY_REFUND_LOG_MESSAGE
					: PaygentConstants.PAYGENT_PAIDY_CANCEL_LOG_MESSAGE;

				externalApiLog = LogCreator.CreateMessageWithPaymentId(
					orderOld.CardTranId,
					orderOld.PaymentOrderId,
					orderOld.LastBilledAmount.ToPriceString(),
					cancelMessage);
			}
			else
			{
				externalApiLog = LogCreator.CreateMessage(orderOld.OrderId, string.Empty);
			}

			// 外部決済連携実施時のみログを残す
			if (reauth.HasAnyAction)
			{
				// 外部決済連携エラーログ追加
				OrderCommon.AppendExternalPaymentCooperationLog(
					isResultDetailSuccess,
					orderNew.OrderId,
					externalApiLog,
					this.LoginOperatorName,
					UpdateHistoryAction.Insert);
			}

			// 決済区分がPayPayかつ売り上げ確定後返金処理が成功している場合
			if (Constants.PAYMENT_PAYPAYOPTION_ENABLED
				&& (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				&& (reauth.HasSales
					|| (orderNew.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP))
				&& reauth.HasRefund
				&& isResultDetailSuccess)
			{
				switch (Constants.PAYMENT_PAYPAY_KBN)
				{
					case w2.App.Common.Constants.PaymentPayPayKbn.SBPS:
					case w2.App.Common.Constants.PaymentPayPayKbn.VeriTrans:
						this.IsSucceedRefundAfterSalsesFixedInPaypay = true;
						break;

					default:
						break;
				}
			}

			// 与信のみに失敗している場合エラー画面へ
			if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure)
			{
				// Atodeneの場合、与信エラーにする
				if (reauth.AuthLostForError)
				{
					var service = new OrderService();
					service.UpdateExternalPaymentInfoForAuthError(
						this.OrderInput.OrderId,
						reauthResult.ErrorMessages,
						this.OrderInput.LastChanged,
						UpdateHistoryAction.Insert);

					// パラメータ取得(これがないと戻るボタンで受注詳細画面に戻っても与信ステータスが与信エラーになっていない)
					var parameters = (Hashtable)Session[Constants.SESSIONPARAM_KEY_ORDER_MODIFY_INFO];
					((OrderInput)parameters[Constants.TABLE_ORDER]).ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_ERROR;
					Session[Constants.SESSIONPARAM_KEY_ORDER_MODIFY_INFO] = parameters;
				}

				Session[Constants.SESSION_KEY_ERROR_MSG] = StringUtility.ChangeToBrTag(reauthResult.ApiErrorMessages);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
			// 失敗したが、同額で与信を取り直している場合は元注文情報の「決済カード取引ID」とを更新し、エラー画面へ
			else if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSameAmount)
			{
				using (var accessor = new SqlAccessor())
				{
					// トランザクション開始
					accessor.OpenConnection();
					accessor.BeginTransaction();

					var service = new OrderService();

					SetExternalPaymentSession(reauthResult);
					// 外部決済情報セット
					SetExternalPaymentInfoReauthSplit(reauth, reauthResult, orderNew, orderOld);

					// 決済取引ID更新
					service.UpdateCardTranId(this.OrderOld.OrderId, reauthResult.CardTranId, this.LoginOperatorName, UpdateHistoryAction.DoNotInsert, accessor);

					var paymentMemo = (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSameAmount)
						? StringUtility.ToEmpty(this.UpdatePaymentMemoForOrderOld)
						: (this.RegisterPaymentMemo ?? "");

					// 決済連携メモ追加
					service.AddPaymentMemo(
						this.OrderOld.OrderId,
						paymentMemo,
						this.LoginOperatorName,
						UpdateHistoryAction.DoNotInsert,
						accessor);

					// トランザクションコミット
					accessor.CommitTransaction();
				}

				// 受注情報詳細ページへ
				Response.Redirect(
					CreateOrderDetailUrl(
						this.OrderInput.OrderId,
						false,
						((string)Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_RELOAD_PARENT_WINDOW] != Constants.KBN_RELOAD_PARENT_WINDOW_OFF)));
			}

			// 他の返品／交換注文を作成した後、既に返金済みの交換注文を編集する場合
			if (this.IsNotLastExchangeOrderModified())
			{
				// 最終与信フラグ、決済メモ、外部決済ステータス更新
				SetExternalPaymentInfoReauthSplit(reauth, reauthResult, orderNew, orderOld);

				// 変更後の注文情報設定
				this.OrderInput.CardTranId = this.RegisterCardTranId ?? orderOld.CardTranId;
				this.OrderInput.PaymentOrderId = this.RegisterPaymentOrderId ?? orderOld.PaymentOrderId;
				this.OrderInput.ExternalPaymentAuthDate = this.RegisterExternalPaymentAuthDate ?? null;
				this.OrderInput.PaymentMemo = this.OrderInput.PaymentMemo + "\r\n" + (this.RegisterPaymentMemo ?? "");
				// 与信しない（キャンセルのみ場合も含む）場合、外部決済ステータス判断
				this.OrderInput.ExternalPaymentStatus = this.RegisterExternalPaymentStatus ??
                    (OrderCommon.CheckCanPaymentReauth(this.OrderInput.OrderPaymentKbn)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
						: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE);
			}
			else
			// 通常注文編集、又は、最新の交換注文編集の場合
			{
				// 失敗？
				this.ReturnOrderReauthErrorMessages = null;

				if (reauthResult.ResultDetail != ReauthResult.ResultDetailTypes.Success)
				{
					this.ReturnOrderReauthErrorMessages = reauthResult.ErrorMessages + "\r\n"
						+ WebMessages.GetMessages(WebMessages.ERRMSG_EXTERNAL_PAYMENT_CANCEL_FAILED);

					// 登録完了後にエラーメッセージを表示するため、セッションにセット
					Session[Constants.SESSION_KEY_ERROR_MSG] = StringUtility.ChangeToBrTag(this.ReturnOrderReauthErrorMessages);

					if (PaygentUtility.CheckIsPaidyPaygentPayment(orderOld.OrderPaymentKbn)
						&& (PaygentUtility.CheckIsPaidyPaygentPayment(orderNew.OrderPaymentKbn) == false)
						&& (string.IsNullOrEmpty(reauthResult.ApiErrorMessages) == false))
					{
						SetSessionOrderId(this.OrderInput.OrderId);

						// 受注情報詳細ページへ
						Response.Redirect(
							CreateOrderDetailUrl(
								this.OrderInput.OrderId,
								false,
								((string)Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_RELOAD_PARENT_WINDOW] != Constants.KBN_RELOAD_PARENT_WINDOW_OFF)));
					}
				}

				// 決済メモがある場合は決済情報を更新する（無い場合は更新しない）
				if (string.IsNullOrEmpty(reauthResult.PaymentMemo) == false)
				{
					// 決済連携メモセット
					this.OrderInput.PaymentMemo =
						this.OrderInput.PaymentMemo
						+ (string.IsNullOrEmpty(this.OrderInput.PaymentMemo) ? string.Empty : "\r\n") + reauthResult.PaymentMemo;

					// 何かしらアクションを持っていたら、決済取引ID・決済注文IDセット（キャンセルのみの場合は空が格納される想定）
					if (reauth.HasAnyAction)
					{
						this.OrderInput.CardTranId = reauthResult.CardTranId;
						this.OrderInput.PaymentOrderId = reauthResult.PaymentOrderId;
						if (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF)
						{
							this.OrderInput.PaymentOrderId = (orderNew.OrderId.Length >= 10)
								? orderNew.OrderId.Substring(orderNew.OrderId.Length - 10)
								: orderNew.OrderId.PadRight(10, '0');
						}
					}
					// 再与信・減額・請求書再発行を持っていたら外部決済情報セット
					if (reauth.HasReauthOrReduceOrReprint)
					{
						this.OrderInput.ExternalPaymentAuthDate = reauth.GetUpdateReauthDate(
							orderOld.ExternalPaymentAuthDate,
							orderOld.OrderPaymentKbn,
							orderNew.OrderPaymentKbn).ToString();
						this.OrderInput.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
						switch (this.OrderInput.OrderPaymentKbn)
						{
 							case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
							case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
								var orderModel = this.OrderInput.CreateModel();
								OrderCommon.CreateExternalPaymentStatusForPaymentAtoneOrAftee(orderModel);
								this.OrderInput.ExternalPaymentStatus = orderModel.ExternalPaymentStatus;
								break;

							case Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY:
								// For case order has NP after payment, invoice bundle flag off and order old has shipment
								if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
									&& (orderOld.OrderPaymentKbn == orderNew.OrderPaymentKbn)
									&& NPAfterPayUtility.CheckIfExternalPaymentStatusHasBeenPaid(orderOld.ExternalPaymentStatus)
									&& string.IsNullOrEmpty(reauthResult.ApiErrorMessages))
								{
									this.OrderInput.ExternalPaymentStatus = orderOld.ExternalPaymentStatus;
									this.OrderInput.InvoiceBundleFlg = Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
								}
								break;
						}
					}
					// キャンセルのみであれば外部済情報リセット
					else if (reauth.HasOnlyCancel)
					{
						this.OrderInput.ExternalPaymentAuthDate = null;
						this.OrderInput.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
					}

					if (reauth.HasOnlyCancel
						&& (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
						&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atobaraicom)
						&& (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success))
					{
						this.OrderInput.OnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE;
					}

					if (reauthResult.IsAuthResultHold)
					{
						this.OrderInput.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST;
					}

					if (reauth.HasOnlyCancel
						&& orderOld.IsExternalPayment
						&& (orderNew.OrderPaymentKbn != orderOld.OrderPaymentKbn)
						&& (orderNew.IsExternalPayment == false))
					{
						this.OrderInput.CardTranId = string.Empty;
						this.OrderInput.PaymentOrderId = string.Empty;
					}
				}
				// 外部決済以外に変更された場合は決済取引IDを空、外部決済情報リセット
				else if ((orderOld.OrderPaymentKbn != orderNew.OrderPaymentKbn)
					&& (orderNew.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF))
				{
					if ((orderNew.IsExternalPayment == false)
						|| (((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && (Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN) == false))
							|| (((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF) && (Constants.REAUTH_COMPLETE_CVSDEF_LIST.Contains(Constants.PAYMENT_CVS_DEF_KBN) == false)))))
					{
						this.OrderInput.CardTranId = string.Empty;
						this.OrderInput.PaymentOrderId = string.Empty;
						this.OrderInput.ExternalPaymentAuthDate = null;
						this.OrderInput.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
					}
					else if (this.NewExecuteType == ReauthCreatorFacade.ExecuteTypes.None)
					{
						this.OrderInput.CardTranId = string.Empty;
						this.OrderInput.PaymentOrderId = string.Empty;
						this.OrderInput.ExternalPaymentAuthDate = null;
						this.OrderInput.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
					}
				}

				// 過去にDSK後払いの請求書印字データ取得していた場合はここで削除
				if ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
					&& (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Dsk)
					&& (reauth.HasOnlyCancel == false))
				{
					var invoiceDskDeferredService = new InvoiceDskDeferredService();
					var invoice = invoiceDskDeferredService.Get(orderNew.OrderId);
					if (invoice != null)
					{
						invoiceDskDeferredService.Delete(orderNew.OrderId);
					}
				}
			}
			return reauth.HasAnyAction;
		}
		return false;
	}

	/// <summary>
	/// 注文情報更新＆更新履歴登録
	/// </summary>
	/// <param name="orderOld">注文情報（変更前）</param>
	/// <param name="orderNew">注文情報（変更後）</param>
	/// <param name="isCreditCardRegisted">クレジットカード登録したか？</param>
	/// <param name="isCreditCardUpdated">クレジットカード登録したか？</param>
	/// <param name="isExecuteExternalPayment">外部決済連携実行したか？</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	private void ExecuteUpdateOrderAndRegisterUpdateHistory(
		OrderModel orderOld,
		OrderModel orderNew,
		bool isCreditCardRegisted,
		bool isCreditCardUpdated,
		bool isExecuteExternalPayment,
		UpdateHistoryAction updateHistoryAction)
	{
		string transactionName = string.Empty;
		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();

			try
			{
				// 注文履歴登録（トランザクション開始）
				var orderHistory = new OrderHistory
				{
					OrderId = this.OrderInput.OrderId,
					Action = OrderHistory.ActionType.EcOrderModify,
					OpearatorName = this.LoginOperatorName,
					Accessor = accessor,
				};
				orderHistory.HistoryBegin();

				if (isExecuteExternalPayment)
				{
					// 変更後の注文の最終与信フラグをONにする
					this.OrderInput.LastAuthFlg = Constants.FLG_ORDER_LAST_AUTH_FLG_ON;

					// 他の返品・交換注文を作成した後、既に返金済みの交換注文を編集する場合、編集前の注文情報設定
					if (this.IsNotLastExchangeOrderModified())
					{
						transactionName = "元注文情報（元注文 or 最後の返品・交換注文）UPDATE処理";
						ExecuteUpdateOrderOld(orderOld, UpdateHistoryAction.DoNotInsert, accessor);
					}

					// Update Online Payment Status
					UpdateOnlinePaymentStatus(orderNew, accessor);
				}
				// 管理メモ更新
				this.OrderInput.ManagementMemo = OrderCommon.GetNotFirstTimeFixedPurchaseManagementMemo(
					this.OrderInput.ManagementMemo,
					this.ProductOrderLmitOrderIds,
					false,
					this.BeforeExtendStatus39,
					this.OrderInput.ExtendStatus39);
				// 注文登録
				bool success = ExecuteUpdateOrder(accessor, out transactionName, UpdateHistoryAction.DoNotInsert);

				// ユーザークレジットカードのフラグをON
				if (success && isCreditCardRegisted)
				{
					//「ユーザークレジットカード枝番更新」
					transactionName = ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_ORDER,
						Constants.VALUETEXT_PARAM_TRANSACTION_NAME,
						Constants.VALUETEXT_PARAM_TRANSACTION_NAME_UPDATE_CREDIT_CARD);
					// 表示フラグをONに更新
					var userCreditCard =
						new UserCreditCard(new UserCreditCardService().Get(orderNew.UserId, orderNew.CreditBranchNo.Value, accessor));
					if ((string.IsNullOrEmpty(userCreditCard.CardDispName) == false)
						&& (userCreditCard.CardDispName != Constants.CREDITCARD_UNREGIST_DEFAULT_DISPLAY_NAME))
					{
						success = new UserCreditCardService().UpdateDispFlg(
							userCreditCard.UserId,
							userCreditCard.BranchNo,
							true,
							userCreditCard.LastChanged,
							UpdateHistoryAction.DoNotInsert,
							accessor);
						if (success == false)
						{
							// 失敗
							AppLogger.WriteError("クレジット情報の登録(表示フラグ更新に失敗:User_id=" + userCreditCard.UserId + " No=" + userCreditCard.BranchNo);
						}
					}
				}

				// 注文のクレジットカード支番更新
				if (success && (isCreditCardRegisted || isCreditCardUpdated))
				{
					new OrderService().UpdateCreditBranchNo(
						this.OrderInput.OrderId,
						orderNew.CreditBranchNo.Value,
						this.LoginOperatorName,
						UpdateHistoryAction.DoNotInsert,
						accessor);
				}

				// 関連注文更新処理
				if (success)
				{
					//「関連注文更新処理」
					transactionName = ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_ORDER,
						Constants.VALUETEXT_PARAM_TRANSACTION_NAME,
						Constants.VALUETEXT_PARAM_TRANSACTION_NAME_UPDATE_RELATE_ORDER);
					success = new OrderService().UpdateRelatedOrdersLastAmount(
						(string.IsNullOrEmpty(this.OrderInput.OrderIdOrg))
							? this.OrderInput.OrderId
							: this.OrderInput.OrderIdOrg,
						this.OrderInput.OrderId,
						decimal.Parse(this.OrderInput.LastBilledAmount),
						decimal.Parse(this.OrderInput.LastOrderPointUse),
						decimal.Parse(this.OrderInput.LastOrderPointUseYen),
						this.LoginOperatorName,
						updateHistoryAction,
						accessor);
				}

				// クロスポイントAPI処理
				if (Constants.CROSS_POINT_OPTION_ENABLED
					&& Order.CheckLinkedCrossPoint(orderOld.OrderDate, orderNew.UserId)
					&& (orderOld.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP))
				{
					transactionName = ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_ORDER,
						Constants.VALUETEXT_PARAM_TRANSACTION_NAME,
						Constants.VALUETEXT_PARAM_TRANSACTION_NAME_CROSS_POINT_INTEGRATION);

					string[] errors;
					var isDelete = false;
					success = UpdatePointByApi(orderNew, out errors, out isDelete);
					if (isDelete)
					{
						DomainFacade.Instance.OrderService.UpdateOrderExtendStatus(
							orderNew.OrderId,
							Constants.ORDER_EXTEND_STATUS_NO_CROSSPOINT_GRANTED,
							Constants.FLG_ORDER_EXTEND_STATUS_ON,
							DateTime.Now,
							Constants.FLG_LASTCHANGED_SYSTEM,
							updateHistoryAction,
							accessor);
					}

					// 伝票がないエラーのみの場合、登録完了した上でエラー出力
					if (CheckOnlyNoSlipError(errors))
					{
						var errorMessage = string.Format(
							"{0}{1}",
							WebMessages.GetMessages(
								WebMessages.ERRMSG_MANAGER_CONTENT_UPDATED_SUCCESS_BUT_CROSS_POINT_LINKAGE_FAILED),
							(isExecuteExternalPayment
								? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_LINKAGE_HAS_ALREADY)
								: string.Empty));
						w2.Common.Logger.AppLogger.WriteError(errorMessage);

						accessor.CommitTransaction();

						// 連続投稿防止用
						Session[Constants.SESSION_KEY_ORDER_REGIST_INPUT_ERROR] = string.Format(
							"error : {0}",
							WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_COUNTINUOUS_WIRTE));

						Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					}
				}

				// 成功時処理
				if (success)
				{
					// 更新履歴登録
					if (updateHistoryAction == UpdateHistoryAction.Insert)
					{
						new UpdateHistoryService().InsertAllForOrder(this.OrderInput.OrderId, this.LoginOperatorName, accessor);
					}

					orderHistory.HistoryComplete();

					// トランザクションコミット
					accessor.CommitTransaction();
				}
				// 失敗時処理
				else
				{
					// ログの記録をしておく
					var errorMessages = string.Format(
						"{0}{1}",
						WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_UPDATE_FAILED).Replace("@@ 1 @@", transactionName),
						isExecuteExternalPayment
							? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_LINKAGE_HAS_ALREADY)
							: string.Empty);
					AppLogger.WriteError(errorMessages);

					// トランザクションロールバック
					accessor.RollbackTransaction();

					// エラー画面へ
					Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}

				// 連続投稿防止用
				Session[Constants.SESSION_KEY_ORDER_REGIST_INPUT_ERROR] = string.Format(
					"error : {0}",
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_COUNTINUOUS_WIRTE)); // これが残ってる時はエラー
			}
			catch (Exception ex)
			{
				// ログの記録をしておく
				AppLogger.WriteError(ex);

				// トランザクションロールバック
				accessor.RollbackTransaction();
				throw ex;
			}
		}
	}

	/// <summary>
	/// 注文情報更新
	/// </summary>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="transactionName">トランザクション名</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	private bool ExecuteUpdateOrder(SqlAccessor accessor, out string transactionName, UpdateHistoryAction updateHistoryAction)
	{
		var isSuccess = true;
		transactionName = string.Empty;

		// 注文更新
		if (isSuccess)
		{
			transactionName = "1-2-1.注文情報UPDATE処理";
			isSuccess = UpdateOrder(UpdateHistoryAction.DoNotInsert, accessor);
		}
		if (isSuccess)
		{
			transactionName = "1-2-2.注文者情報UPDATE処理";
			isSuccess = UpdateOrderOwner(UpdateHistoryAction.DoNotInsert, accessor);
		}
		if (isSuccess)
		{
			transactionName = "1-2-3.注文配送先情報UPDATE処理";
			isSuccess = UpdateOrderShipping(UpdateHistoryAction.DoNotInsert, accessor);
		}
		if (isSuccess)
		{
			transactionName = "1-2-4.注文商品情報UPDATE処理";
			isSuccess = UpdateOrderItem(UpdateHistoryAction.DoNotInsert, accessor);
		}
		if (isSuccess)
		{
			transactionName = "1-2-4-1.定期購入継続分析情報UPDATE処理";
			isSuccess = UpdateFixedPurchaseRepeatAnalysis(accessor);
		}
		if (isSuccess)
		{
			transactionName = "1-2-5.注文セットプロモーション情報UPDATE処理";
			isSuccess = UpdateOrderSetPromotion(UpdateHistoryAction.DoNotInsert, accessor);
		}
		if (isSuccess)
		{
			transactionName = "1-2-6.税率毎価格情報UPDATE処理";
			isSuccess = UpdateOrderPriceByTaxRate(accessor);
		}

		// 商品在庫更新処理
		if ((isSuccess) && (Constants.ORDERMANAGEMENT_STOCKCOOPERATION_ENABLED))
		{
			transactionName = "1-3.商品在庫情報UPDATE処理";
			isSuccess = UpdateOrderProductStock(UpdateHistoryAction.DoNotInsert, accessor);
		}

		// クーポンオプション系処理（クーポンは通常注文のみ）
		if ((Constants.W2MP_COUPON_OPTION_ENABLED)
			&& (this.OrderInputOld.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_UNKNOWN))
		{
			if (isSuccess)
			{
				var isExistOldUserCoupon = ((this.OrderInputOld.Coupon != null) && new CouponService().GetAllUserCouponsFromCouponId(
					this.LoginOperatorDeptId,
					this.OrderInput.UserId,
					this.OrderInputOld.Coupon.CouponId,
					int.Parse(this.OrderInputOld.Coupon.CouponNo)).Any());
				// 注文クーポン登録時処理
				if ((this.OrderInput.Coupon.CouponCode != "") && (this.OrderInputOld.Coupon == null))
				{
					// 注文クーポン登録
					if (isSuccess)
					{
						transactionName = "1-4-1-A-1.注文クーポン情報INSERT処理";
						isSuccess = InsertOrderCoupon(UpdateHistoryAction.DoNotInsert, accessor);
					}
					// ユーザクーポン利用
					if (isSuccess)
					{
						// ユーザクーポンを利用済みにする
						if (this.OrderInput.Coupon.IsCouponLimit)
						{
							transactionName = "1-4-1-A-2-A.ユーザクーポン情報(利用クーポン)UPDATE処理";
							isSuccess = UpdateUsedUserCoupon(UpdateHistoryAction.DoNotInsert, accessor);
						}
						// 利用回数制限付きクーポンをマイナス
						else if (this.OrderInput.Coupon.IsCouponAllLimit)
						{
							transactionName = "1-4-1-A-2-B.クーポン情報(クーポン利用回数)UPDATE処理";
							isSuccess = UpdateCouponCountDown(accessor);
						}
						// 会員限定回数制限ありクーポンをマイナス
						else if (this.OrderInput.Coupon.IsCouponLimitedForRegisteredUser)
						{
							transactionName = "1-4-1-A-2-C.会員限定回数制限ありクーポン情報(クーポン利用回数)UPDATE処理";
							isSuccess = CouponOptionUtility.UpdateUserCouponCount(
								this.OrderInput.Coupon.DeptId,
								this.OrderInput.UserId,
								this.OrderInput.Coupon.CouponId,
								int.Parse(this.OrderInput.Coupon.CouponNo),
								accessor,
								false);
						}
					}
					// ユーザクーポン履歴登録
					if (isSuccess)
					{
						transactionName = "1-4-1-A-3.ユーザクーポン履歴情報(利用クーポン)INSERT処理";
						isSuccess = InsertUserCouponHistory(
							this.OrderInput,
							Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_USE,
							Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_ORDER,
							-1,
							decimal.Parse(this.OrderInput.OrderCouponUse),
							accessor);
					}
					// ブラックリスト型クーポン利用ユーザー登録
					if (isSuccess)
					{
						if (this.OrderInput.Coupon.IsBlacklistCoupon)
						{
							transactionName = "1-4-1-A-4.ブラックリスト型クーポン利用ユーザー情報INSERT処理";
							isSuccess = InsertCouponUseUser(accessor);
						}
					}
				}
				// 注文クーポン変更時処理
				else if ((this.OrderInput.Coupon.CouponCode != "") && (this.OrderInput.Coupon != null))
				{
					// 注文クーポン更新
					if (isSuccess)
					{
						if ((this.OrderInput.Coupon.CouponCode != this.OrderInputOld.Coupon.CouponCode)
							|| (this.OrderInput.Coupon.CouponDispName != this.OrderInputOld.Coupon.CouponDispName)
							|| (this.OrderInput.Coupon.CouponName != this.OrderInputOld.Coupon.CouponName))
						{
							transactionName = "1-4-1-B-1.注文クーポン情報UPDATE処理";
							isSuccess = UpdateOrderCoupon(UpdateHistoryAction.DoNotInsert, accessor);
						}
					}

					if (isSuccess)
					{
						// ユーザクーポン変更(クーポンコードに変更があったときのみ）
						if (this.OrderInput.Coupon.CouponCode != this.OrderInputOld.Coupon.CouponCode)
						{
							if (isExistOldUserCoupon)
							{
								// ユーザクーポン(変更前)を未使用にする
								if (this.OrderInputOld.Coupon.IsCouponLimit)
								{
									transactionName = "1-4-1-B-2-A.ユーザクーポン情報(利用クーポン)UPDATE処理";
									isSuccess = UpdateUnUseUserCoupon(UpdateHistoryAction.DoNotInsert, accessor);
								}
								// クーポン利用回数(変更前)をプラス１して数を戻す
								else if (this.OrderInputOld.Coupon.IsCouponAllLimit)
								{
									transactionName = "1-4-1-B-2-B.クーポン情報(クーポン利用回数)UPDATE処理";
									isSuccess = UpdateCouponCountUp(accessor);
								}
								// ブラックリスト型クーポン利用ユーザー情報削除
								else if (this.OrderInputOld.Coupon.IsBlacklistCoupon)
								{
									transactionName = "1-4-1-B-2-C.クーポン情報(ブラックリスト型クーポン利用ユーザー)DELETE処理";
									isSuccess = DeleteCouponUseUser(accessor);
								}
								// 会員限定回数制限ありクーポンをプラス
								else if (this.OrderInputOld.Coupon.IsCouponLimitedForRegisteredUser)
								{
									transactionName = "1-4-1-B-2-D.会員限定回数制限ありクーポン情報(クーポン利用回数)UPDATE処理";
									isSuccess = CouponOptionUtility.UpdateUserCouponCount(
										this.OrderInputOld.Coupon.DeptId,
										this.OrderInputOld.UserId,
										this.OrderInputOld.Coupon.CouponId,
										int.Parse(this.OrderInputOld.Coupon.CouponNo),
										accessor,
										true);
								}
							}

							// ユーザクーポン(変更後)を利用済みにする
							if (this.OrderInput.Coupon.IsCouponLimit)
							{
								transactionName = "1-4-1-B-3-A.ユーザクーポン情報(利用クーポン)UPDATE処理";
								isSuccess = UpdateUsedUserCoupon(UpdateHistoryAction.DoNotInsert, accessor);
							}
							// クーポン利用回数(変更後) マイナス１する
							else if (this.OrderInput.Coupon.IsCouponAllLimit)
							{
								transactionName = "1-4-1-B-3-B.クーポン情報(クーポン利用回数)UPDATE処理";
								isSuccess = UpdateCouponCountDown(accessor);
							}
							else if (this.OrderInput.Coupon.IsBlacklistCoupon)
							{
								transactionName = "1-4-1-B-3-C.クーポン情報(ブラックリスト型クーポン利用ユーザー)INSERT処理";
								isSuccess = InsertCouponUseUser(accessor);
							}
							// 会員限定回数制限ありクーポンをマイナス
							else if (this.OrderInput.Coupon.IsCouponLimitedForRegisteredUser)
							{
								transactionName = "1-4-1-A-3-D.会員限定回数制限ありクーポン情報(クーポン利用回数)UPDATE処理";
								isSuccess = CouponOptionUtility.UpdateUserCouponCount(
									this.OrderInput.Coupon.DeptId,
									this.OrderInput.UserId,
									this.OrderInput.Coupon.CouponId,
									int.Parse(this.OrderInput.Coupon.CouponNo),
									accessor,
									false);
							}
						}
					}

					// ユーザクーポン履歴追加
					if (isSuccess)
					{
						// ※ユーザクーポン履歴の仕様として、
						// 　クーポンコード、クーポン割引額に変更があった場合
						// 　1.以前に適用したクーポンの履歴の消込を行う。(マイナス履歴)
						// 　2.適用するクーポンの履歴を登録を行う。
						if ((this.OrderInput.Coupon.CouponCode != this.OrderInputOld.Coupon.CouponCode)
							|| (decimal.Parse(this.OrderInput.OrderCouponUse) != decimal.Parse(this.OrderInputOld.OrderCouponUse)))
						{
							// 1.以前に適用したクーポンの履歴の消込を行う。(マイナス履歴)
							if (isSuccess)
							{
								transactionName = "1-4-B-4.ユーザクーポン履歴情報(利用クーポン)INSERT処理";
								isSuccess = InsertUserCouponHistory(
									this.OrderInputOld,
									Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_USE_CANCEL,
									Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_ORDER,
									1,
									decimal.Parse(this.OrderInputOld.OrderCouponUse) * -1,
									accessor);
							}
							// 2.適用するクーポンの履歴を登録を行う。
							if (isSuccess)
							{
								transactionName = "1-4-B-5.ユーザクーポン履歴情報(利用クーポン)INSERT処理";
								isSuccess = InsertUserCouponHistory(
									this.OrderInput,
									Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_USE,
									Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_ORDER,
									-1,
									decimal.Parse(this.OrderInput.OrderCouponUse),
									accessor);
							}
						}
					}
				}
				// 注文クーポン削除時処理
				else if ((this.OrderInput.Coupon.CouponCode == "") && (this.OrderInputOld.Coupon != null))
				{
					// 注文クーポン削除
					if (isSuccess)
					{
						transactionName = "1-4-1-C-1.注文クーポン情報DELETE処理";
						isSuccess = DeleteOrderCoupon(updateHistoryAction, accessor);
					}
					// ユーザクーポン情報を戻す
					if (isSuccess && isExistOldUserCoupon)
					{
						// ユーザクーポン(変更前)を未使用にする
						if (this.OrderInputOld.Coupon.IsCouponLimit)
						{
							transactionName = "1-4-2-C-2-A.ユーザクーポン情報(利用クーポン)UPDATE処理";
							isSuccess = UpdateUnUseUserCoupon(updateHistoryAction, accessor);
						}
						// クーポン利用回数(変更前) プラス１して数を戻す
						else if (this.OrderInputOld.Coupon.IsCouponAllLimit)
						{
							transactionName = "1-4-2-C-2-B.クーポン情報(クーポン利用回数)UPDATE処理";
							isSuccess = UpdateCouponCountUp(accessor);
						}
						// ブラックリスト型クーポン利用ユーザー情報削除
						else if (this.OrderInputOld.Coupon.IsBlacklistCoupon)
						{
							transactionName = "1-4-2-C-2-C.クーポン情報(ブラックリスト型クーポン利用ユーザー)INSERT処理";
							isSuccess = DeleteCouponUseUser(accessor);
						}
						// 会員限定回数制限ありクーポンをプラス
						else if (this.OrderInputOld.Coupon.IsCouponLimitedForRegisteredUser)
						{
							transactionName = "1-4-2-C-2-D.会員限定回数制限ありクーポン情報(クーポン利用回数)UPDATE処理";
							isSuccess = CouponOptionUtility.UpdateUserCouponCount(
								this.OrderInputOld.Coupon.DeptId,
								this.OrderInputOld.UserId,
								this.OrderInputOld.Coupon.CouponId,
								int.Parse(this.OrderInputOld.Coupon.CouponNo),
								accessor,
								true);
						}
					}
					// ユーザクーポン履歴追加
					if (isSuccess)
					{
						transactionName = "1-4-C-3.ユーザクーポン履歴情報(利用クーポン)INSERT処理";
						isSuccess = InsertUserCouponHistory(
							this.OrderInputOld,
							Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_USE_CANCEL,
							Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_ORDER,
							1,
							decimal.Parse(this.OrderInputOld.OrderCouponUse) * -1,
							accessor);
					}
				}
			}
		}

		// ポイントオプション系処理（ポイントは通常注文のみ）
		if ((Constants.W2MP_POINT_OPTION_ENABLED)
				&& (this.OrderInputOld.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_UNKNOWN))
		{
			// 自社サイトの会員のみ対象
			if ((this.OrderInput.MallId == Constants.FLG_USER_MALL_ID_OWN_SITE) && this.IsUser)
			{
				// ユーザポイント更新(利用ポイント)
				if (this.OrderInput.OrderPointUse != this.OrderInputOld.OrderPointUse)
				{
					transactionName = "1-4-1.ユーザポイント情報(利用ポイント)再計算処理";
					isSuccess = new PointService().RecalcOrderUsePoint(
						this.OrderOld.UserId,
						// ポイント計算では元注文IDで管理しているので、返品交換注文でも
						// 元注文IDを使用する。
						(string.IsNullOrEmpty(this.OrderOld.OrderIdOrg) == false)
							? this.OrderOld.OrderIdOrg
							: this.OrderOld.OrderId,
						this.OrderInput.OrderId,
						decimal.Parse(this.OrderInput.OrderPointUse),
						this.LoginOperatorName,
						updateHistoryAction,
						accessor) > 0;
				}

				// ユーザポイントUPDATE処理(付与ポイント)
				foreach (var userPoint in this.UserPointList.Items)
				{
					// 変更前ユーザポイント情報取得
					var userPointOld = this.UserPointListOld.GetUserPointByPointKbnNo(userPoint.PointKbnNo);

					// 更新対象が仮ポイント AND ユーザポイント(仮ポイント)が存在する？
					if (userPoint.IsPointTypeTemp && (userPointOld != null))
					{
						// ポイントに変更があった場合にユーザポイント更新
						if (userPoint.Point != userPointOld.Point)
						{
							transactionName = "1-5-2-A.ユーザポイント情報(付与ポイント)UPDATE処理";
							isSuccess = UpdateUserPoint(
								this.OrderInput,
								userPoint,
								userPoint.Point - userPointOld.Point,
								updateHistoryAction,
								accessor);
						}
					}
                    // ユーザポイント(本ポイント)が存在する AND 付与ポイントあり
					else if (userPoint.IsPointTypeComp && (userPointOld != null))
					{
						var adjustPoint = userPoint.Point
							- (userPoint.IsBasePoint ? decimal.Parse(this.AddedBasePointCompOld) : userPointOld.Point);

						// ポイントに変更があった場合
						if (adjustPoint != 0)
						{
							// ユーザポイントを更新
							transactionName = "1-5-2-B.ユーザポイント情報(付与ポイント)UPDATE処理";
							isSuccess = UpdateUserPoint(
							this.OrderInput,
							userPoint,
							adjustPoint,
							updateHistoryAction,
							accessor);
						}
					}
					// ユーザポイント(本ポイント)が存在しない AND 付与ポイント有り
					else if (userPoint.IsPointTypeComp && (userPointOld == null))
					{
						// ポイントに変更があった場合
						if (userPoint.Point != decimal.Parse(this.AddedBasePointCompOld))
						{
							// 変更後付与ポイント - 変更前付与ポイントを取得
							var adjustPoint = userPoint.Point - decimal.Parse(this.AddedBasePointCompOld);

							// 調整分のユーザポイントを追加
							transactionName = "1-5-2-C.ユーザポイント情報(付与ポイント)INSERT処理";
							isSuccess = InsertUserPoint(
								this.OrderInput,
								userPoint,
								adjustPoint,
								updateHistoryAction,
								accessor);
                        }
					}
                }
			}
		}

		// ユーザメモ＆ユーザー管理レベルID更新
		if (isSuccess)
		{
			transactionName = "2-1.ユーザ情報UPDATE処理";

			isSuccess = new UserService().UpdateUserMemoAndUserManagementLevelId(
				this.OrderInput.UserId,
				StringUtility.RemoveUnavailableControlCode(this.OrderInput.Owner.UserMemo),
				this.OrderInput.Owner.UserManagementLevelId,
				this.OrderInput.LastChanged,
				updateHistoryAction,
				accessor);
		}

		// Update order invoice
		if (isSuccess
			&& OrderCommon.DisplayTwInvoiceInfo()
			&& OrderCommon.TwInvoiceStatusCanEnableControl(
				this.TwOrderInvoiceInput.CreateModel(),
				Constants.TWINVOICE_ENABLED))
		{
			transactionName = Constants.TWINVOICE_ENABLED
				? "2-2-A.UPDATE TAIWAN ORDER INVOICE"
				: "2-2-B.UPDATE TAIWAN ECPAY ORDER INVOICE";
			isSuccess = UpdateTwOrderInvoice(UpdateHistoryAction.DoNotInsert, accessor);
		}
		return isSuccess;
	}

	#region メソッド
	/// <summary>
	/// 処理区分チェック
	/// </summary>
	private void CheckActionStatus()
	{
		// 編集でなければエラーページへ
		CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);
		if ((this.IsUpdateShippingConvenience == false)
			&& (this.ActionStatus != Constants.ACTION_STATUS_UPDATE))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// 各プロパティセット
	/// </summary>
	private void SetProperty()
	{
		// 注文情報セット
		SetOrder();

		// ユーザ＆ユーザポイント情報セット
		SetUserAndUserPoint();

		// 督促情報
		if (Constants.DEMAND_OPTION_ENABLE && ShowDisplayDemandStatus(this.OrderInput.OrderPaymentKbn))
		{
			trDemandStatus.Visible = true;
			trDemandDay.Visible = true;
		}

		// 配送種別情報セット
		this.ShopShipping = new ShopShippingService().Get(this.LoginOperatorShopId, this.OrderInput.ShippingId);

		// 配送方法情報セット
		this.ShippingMethod = this.OrderInput.Shippings.Select(i => i.ShippingMethod).ToArray();

		// 配送会社情報セット
		var companyList = new List<DeliveryCompanyModel>();
		foreach (var shipping in this.OrderInput.Shippings)
		{
			var company = this.DeliveryCompanyList.FirstOrDefault(i => i.DeliveryCompanyId == shipping.DeliveryCompanyId);
			companyList.Add((company != null) ? company : new DeliveryCompanyModel());
		}
		this.DeliveryCompany = companyList.ToArray();

		// 項目メモ一覧セット
		this.FieldMemoSettingList = GetFieldMemoSettingList(Constants.TABLE_ORDER);

		// クレジットカード情報セット
		var parameters = (Hashtable)Session[Constants.SESSIONPARAM_KEY_ORDER_MODIFY_INFO];
		this.OrderCreditCard = (OrderCreditCardInput)parameters[Constants.TABLE_ORDER + "_credit_card"];

		// 再与信処理区分セット
		this.OldExecuteType = (ReauthCreatorFacade.ExecuteTypes)Enum.Parse(typeof(ReauthCreatorFacade.ExecuteTypes), (string)parameters["old_execute_type"]);
		this.NewExecuteType = (ReauthCreatorFacade.ExecuteTypes)Enum.Parse(typeof(ReauthCreatorFacade.ExecuteTypes), (string)parameters["new_execute_type"]);

		// Set Taiwan Order Invoice
		SetTwOrderInvoice();
	}

	/// <summary>
	/// 注文情報セット
	/// </summary>
	private void SetOrder()
	{
		// パラメータ取得
		var parameters = (Hashtable)Session[Constants.SESSIONPARAM_KEY_ORDER_MODIFY_INFO];

		// 注文情報セット
		this.OrderInput = (OrderInput)parameters[Constants.TABLE_ORDER];
		this.OrderInputOld = (OrderInput)parameters[Constants.TABLE_ORDER + "_input_old"];
		this.OrderOld = (OrderModel)parameters[Constants.TABLE_ORDER + "_old"];

		//決済金額
		var settlementCurrency = CurrencyManager.GetSettlementCurrency(this.OrderInput.OrderPaymentKbn);
		var settlementRate = CurrencyManager.GetSettlementRate(settlementCurrency);
		this.OrderInput.SettlementCurrency = settlementCurrency;
		this.OrderInput.SettlementRate = settlementRate.ToString();
		this.OrderInput.SettlementAmount = CurrencyManager.GetSettlementAmount(
			this.OrderOld.OrderId,
			this.OrderInput.OrderPaymentKbn,
			decimal.Parse(this.OrderInput.LastBilledAmount),
			settlementRate,
			settlementCurrency).ToString();
	}

	/// <summary>
	/// Set TwOrderInvoice
	/// </summary>
	private void SetTwOrderInvoice()
	{
		// Get Params
		var parametersForTwOrderInvoice = (Hashtable)Session[Constants.SESSIONPARAM_KEY_TWORDERINVOICE_MODIFY_INFO];

		// Set TwOrderInvoice
		this.TwOrderInvoiceInput = (TwOrderInvoiceInput)parametersForTwOrderInvoice[Constants.TABLE_TWORDERINVOICE];
	}

	/// <summary>
	/// ユーザ＆ユーザポイント情報セット
	/// </summary>
	private void SetUserAndUserPoint()
	{
		// 会員ユーザの場合、ユーザポイントセット
		var user = new UserService().Get(this.OrderInput.UserId);
		if (Constants.W2MP_POINT_OPTION_ENABLED
			&& UserService.IsUser(user.UserKbn))
		{
			// パラメータ取得
			var parameters = (Hashtable)Session[Constants.SESSIONPARAM_KEY_ORDER_MODIFY_INFO];

			this.UserPointList = (UserPointList)parameters[Constants.TABLE_USERPOINT];
			this.UserPointListOld = (UserPointList)parameters[Constants.TABLE_USERPOINT + "_old"];
			this.AddedBasePointCompOld = (string)parameters["added_base_point_comp_old"];
		}
		// ユーザ情報セット
		this.User = user;
	}

	/// <summary>
	/// 支払金額合計チェック
	/// </summary>
	private void CheckOrderPriceTotal()
	{
		var orderPriceTotal = decimal.Parse(this.OrderInput.OrderPriceTotal);
		var orderPriceTotalOld = decimal.Parse(this.OrderInputOld.OrderPriceTotal);
		var lastBilledAmount = decimal.Parse(this.OrderInput.LastBilledAmount);
		var orderPointUseYen = decimal.Parse(this.OrderInput.OrderPointUseYen);

		// 注文金額チェック（決済手段の金額範囲に含まれているかどうか）
		if ((this.OrderInput.IsExchangeOrder)
			&& (CheckPaymentPriceEnabled(this.LoginOperatorShopId, this.OrderInput.OrderPaymentKbn, lastBilledAmount, orderPointUseYen) != ""))
		{
			trOrderErrorMessagesTitle.Visible = trOrderErrorMessages.Visible = true;
			lbOrderErrorMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_ORDER_EXCHANGE_EDIT_ERROR).Replace("@@ 1 @@", this.OrderInput.PaymentName);
		}

		// PayPal決済
		if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS)
		{
			if (orderPriceTotal != orderPriceTotalOld)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_PAYPAL_CHANGE_PRICE_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}

		// クレジットカード決済：クレジットカード保機能利用不可の場合、再与信できない
		if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& (OrderCommon.CreditCardRegistable == false))
		{
			if (orderPriceTotal != orderPriceTotalOld)
			{
				this.UpdateAlertMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CREDIT_CARD_CHANGE_PRICE_ERROR);
			}
		}
	}

	/// <summary>
	/// コンポーネントに値セット
	/// </summary>
	private void SetValueToComponents()
	{
		// 拡張ステータス・更新日データソースセット
		rOrderExtendStatusList.DataSource
			= rOrderExtendStatusDates.DataSource
			= GetOrderExtendStatusSettingList(this.LoginOperatorShopId);

		// ポイント情報セット
		if (Constants.W2MP_POINT_OPTION_ENABLED)
		{
			// 表示制御(初期化)
			trOrderPointUse.Visible = false;
			trOrderPointAddComp.Visible = false;
			rOrderPointAddTempOrLimitedTermComp.Visible = false;

			// 会員ユーザの場合
			if (this.IsUser)
			{
				// 利用ポイント戻しのエラーチェック
				if (string.IsNullOrEmpty(this.UserPointExpiredAlertMessage) == false)
				{
					trOrderAlertMessagesTitle.Visible = trOrderAlertMessages.Visible = true;
					lbOrderAlertMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_POINT_RETURN_EXPIRED_ALERT);
				}

				// 利用ポイントセット
				lOrderPointUse.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(this.OrderInput.OrderPointUse));
				trOrderPointUse.Visible = true;

				// 利用ポイントセット
				lLastOrderPointUse.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(this.OrderInput.LastOrderPointUse));
				trLastOrderPointUse.Visible = (this.OrderInput.IsNotReturnExchangeOrder == false);

				// ユーザポイント(仮ポイント)が存在する？
				if (this.UserPointListOld.UserPointTemp.Length > 0)
				{
					rOrderPointAddTempOrLimitedTermComp.DataSource = this.UserPointList.UserPointTemp;
					rOrderPointAddTempOrLimitedTermComp.Visible = true;
				}

				// ユーザポイント(仮ポイント)が存在しない場合 AND 付与ポイント有り？
				if (((this.UserPointListOld.UserPointComp.Length == 0)
                        && (decimal.Parse(this.OrderInputOld.OrderPointAdd) > 0)) 
                    || (this.IsNoPointPublished == Constants.FLG_USERPOINT_POINT_NOT_PUBLISHED)) 
				{
					var addedBasePointComp = (this.IsNoPointPublished == Constants.FLG_USERPOINT_POINT_NOT_PUBLISHED)
						? this.UserPointList.Items.Where(up => up.IsPointTypeComp).Sum(up => up.Point)
						: this.UserPointList.Items.Where(up => (up.IsPointTypeComp && up.IsBasePoint)).Sum(up => up.Point);
					lOrderPointAddComp.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(addedBasePointComp));
					trOrderPointAddComp.Visible = true;
				}

				// Control display gmo cvs type
				trGmoCvsType.Visible = ((Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Gmo)
					&& (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
					&& (this.OrderOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE));
				// Gmo cvs type
				lGmoCvsType.Text = ValueText.GetValueText(Constants.TABLE_PAYMENT, Constants.PAYMENT_GMO_CVS_TYPE, this.GmoCvsType);
			}
		}

		// 決済種別情報セット
		lOrderPaymentKbn.Text = WebSanitizer.HtmlEncode(this.OrderInput.PaymentName);
		tbdyOrderKbnCredit.Visible = (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT
			&& Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN));
		if (tbdyOrderKbnCredit.Visible)
		{
			this.UseNewCreditCard = (this.OrderCreditCard.CreditBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW);
			lbCreditCardCompany.Text =
				WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditCompanyValueTextFieldName, this.OrderCreditCard.CompanyCode));
			var dispCardNo = (this.UseNewCreditCard && this.CreditTokenizedPanUse)
				? this.OrderCreditCard.CardNo
				: string.Format("************{0}", UserCreditCardHelper.CreateCreditCardNoDispString(this.OrderCreditCard.CardNo));
			lbCreditCardNo.Text = WebSanitizer.HtmlEncode(dispCardNo);
			lbValidity.Text = WebSanitizer.HtmlEncode(
				string.Format("{0}/{1}{2}",
					this.OrderCreditCard.ExpireMonth,
					this.OrderCreditCard.ExpireYear,
					lbValidity.Text));
			lbAuthorName.Text = WebSanitizer.HtmlEncode(this.OrderCreditCard.AuthorName);
			trRegistCreditCard.Visible = this.OrderCreditCard.DoRegister;
			lbRegistCreditCard.Text = WebSanitizer.HtmlEncode(
				this.OrderCreditCard.DoRegister
					? ReplaceTag("@@DispText.common_message.register@@")
					: ReplaceTag("@@DispText.common_message.do_not_register@@"));
			var userCreditCardName = this.OrderCreditCard.RegisterCardName;
			if (this.UseNewCreditCard == false)
			{
				var userCreditCard = new UserCreditCardService().Get(
					this.OrderInput.UserId,
					int.Parse(this.OrderCreditCard.CreditBranchNo));
				userCreditCardName = WebSanitizer.HtmlEncode(userCreditCard.CardDispName);
			}
			lbUserCreditCardName.Text = WebSanitizer.HtmlEncode(userCreditCardName);
			trUserCreditCardName.Visible = ((string.IsNullOrEmpty(userCreditCardName) == false)
				&& (userCreditCardName != Constants.CREDITCARD_UNREGIST_DEFAULT_DISPLAY_NAME)
				&& (this.OrderCreditCard.DoRegister || (this.UseNewCreditCard == false)));
		}

		// クーポン情報設定
		if (Constants.W2MP_COUPON_OPTION_ENABLED)
		{
			// クーポン情報が存在する？
			if (this.OrderInput.Coupon != null)
			{
				// クーポンコードに指定がある？
				if (this.OrderInput.Coupon.CouponCode != "")
				{
					lOrderCouponCode.Text = this.OrderInput.Coupon.CouponCode;
					lOrderCouponUse2.Text = WebSanitizer.HtmlEncode(this.OrderInput.OrderCouponUse.ToPriceString(true));
					lOrderCouponDispName.Text = this.OrderInput.Coupon.CouponDispName;
					lOrderCouponName.Text = this.OrderInput.Coupon.CouponName;
				}
				else
				{
					lOrderCouponCode.Text = ReplaceTag("@@DispText.common_message.unspecified@@");
					lOrderCouponUse2.Text = "-";
					lOrderCouponDispName.Text = "-";
					lOrderCouponName.Text = "-";
				}
			}
		}

		// 画面切替
		if (this.OrderInputOld.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_UNKNOWN)
		{
			// ポイント表示、クーポン情報非表示
			divPoint.Visible = true;
			divCoupon.Visible = false;
		}

		// 配送方法アラート表示
		if (this.OrderInput.Shippings
			.Any(shipping => ((shipping.ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)
				&& (OrderCommon.IsAvailableShippingKbnMail(shipping.Items
				.Select(item => item.CreateModel())) == false))))
		{
			trOrderShippingAlertTitle.Visible = true;
			trOrderShippingAlert.Visible = true;
			lbOrderShippingAlert.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_CANNOT_SHIPPING_MAIL);
		}

		// メール便配送サービスアラート表示
		if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED
			&& (this.ShippingMethod[0] == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL))
		{
			var itemQuantity = 0;
			var totalProductSize = OrderCommon.GetTotalProductSizeFactor(this.OrderInput.Shippings[0].Items
				.Select(item => new KeyValuePair<string, Tuple<int, string, string>>(
					item.ProductId,
					new Tuple<int, string, string>(
						(int.TryParse(item.ItemQuantity.ToString(), out itemQuantity)
							? itemQuantity
							: 0),
						item.ShopId,
						item.VariationId))));
			if (totalProductSize > this.DeliveryCompany[0].DeliveryCompanyMailSizeLimit)
			{
				trOrderShippingAlertTitle.Visible = true;
				trOrderShippingAlert.Visible = true;
				lbOrderShippingAlert.Text = WebMessages
					.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_CANNOT_SHIPPING_SERVICE)
					.Replace("@@ 1 @@", this.DeliveryCompany[0].DeliveryCompanyName);
			}
		}

		if (Constants.ORDER_EXTEND_OPTION_ENABLED)
		{
			rOrderExtendInput.DataSource = new OrderExtendInput(this.OrderInput.OrderExtendInput).OrderExtendItems;
		}

		// データバインド
		DataBind();
	}

	/// <summary>
	/// 配送種別が受注情報と商品で異なる場合はエラーを出す
	/// </summary>
	private void IsShippingError()
	{
		var errmsg = (string)Session["shipping_kbn_different_alert"];
		Session["shipping_kbn_different_alert"] = null;

		tbShippingError.Visible = (String.IsNullOrEmpty(errmsg) == false);
		lbShipErrorMessage.Text = errmsg;
	}

	/// <summary>
	/// 配送サービスの配送不可エリア（郵便番号）を取得
	/// </summary>
	/// <param name="shippingId">配送種別ID</param>
	/// <param name="delivaryCompanyId">配送会社ID</param>
	/// <returns>配送不可郵便番号</returns>
	private static string GetUnavailableShippingZip(string shippingId, string delivaryCompanyId)
	{
		var shopShipping = new ShopShippingService();
		var unavailableShippingZip = shopShipping.GetUnavailableShippingZipFromShippingDelivery(shippingId, delivaryCompanyId);
		return unavailableShippingZip;
	}

	/// <summary>
	/// 注文商品チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private string CheckOrderItem()
	{
		var errorMessages = new StringBuilder();

		// 商品全削除チェック
		// ※確認画面では行わない

		// 注文商品入力チェック
		// ※確認画面では行わない

		// 商品存在チェック
		var productVariations = new List<DataView>();
		var orderItems = this.OrderInput.Shippings.SelectMany(os => os.Items).ToArray();
		foreach (var orderItem in orderItems)
		{
			// 商品情報が存在しない？
			var productVariation = GetProductVariation(orderItem.ShopId, orderItem.ProductId, orderItem.VariationId);
			if (productVariation.Count == 0)
			{
				errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_DELETE).Replace("@@ 1 @@", orderItem.ProductName));
			}
			productVariations.Add(productVariation);
		}

		//　販売可能数量チェックはアラートで対応
		// ※確認画面では行わない

		// 商品在庫チェック
		if (Constants.ORDERMANAGEMENT_STOCKCOOPERATION_ENABLED)
		{
			var errorProductNameList = CheckProductStock(this.OrderInputOld, orderItems);
			foreach (var errorProductName in errorProductNameList)
			{
				errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_NO_STOCK).Replace("@@ 1 @@", errorProductName));
			}

			if (errorProductNameList.Count > 0)
			{
				this.IsNeedShowConfirm = false;
			}
		}

		return errorMessages.ToString();
	}

	/// <summary>
	/// 注文情報更新
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>処理結果</returns>
	private bool UpdateOrder(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var order = this.OrderInput.CreateModel();
		order.OrderTaxRoundType = Constants.TAX_EXCLUDED_FRACTION_ROUNDING;
		var result = (new OrderService().UpdateForModify(order, this.LoginOperatorName, updateHistoryAction, accessor) > 0);
		return result;
	}

	/// <summary>
	/// 注文者情報更新
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>処理結果</returns>
	private bool UpdateOrderOwner(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var orderOwner = this.OrderInput.Owner.CreateModel();
		var result = (new OrderService().UpdateOwnerForModify(
			orderOwner,
			this.LoginOperatorName,
			updateHistoryAction,
			accessor) > 0);
		return result;
	}

	/// <summary>
	/// Update Taiwan Order Invoice
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>処理結果</returns>
	private bool UpdateTwOrderInvoice(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var twOrder = this.TwOrderInvoiceInput.CreateModel();
		var result = (new TwOrderInvoiceService().UpdateTwOrderInvoiceForModify(
			twOrder,
			this.LoginOperatorName,
			updateHistoryAction,
			accessor) > 0);

		return result;
	}

	/// <summary>
	/// 配送先情報更新
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>処理結果</returns>
	private bool UpdateOrderShipping(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var orderShippings = this.OrderInput.Shippings.Select(s => s.CreateModel()).ToArray();
		var result = (new OrderService().UpdateShippingForModify(
			orderShippings,
			this.LoginOperatorName,
			updateHistoryAction,
			accessor) > 0);
		return result;
	}

	/// <summary>
	/// 注文商品更新
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>true：成功、false：失敗</returns>
	private bool UpdateOrderItem(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var orderItems = this.OrderInput.Shippings.SelectMany(s => s.Items)
			.Where(i => i.DeleteTarget == false).Select(i => i.CreateModel()).ToArray();
		// 定期商品チェック外れの場合（定期商品フラグがオフ）、定期購入回数（注文基準、出荷基準）をNullに戻す
		foreach (var orderItem in orderItems)
		{
			if (orderItem.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF)
			{
				orderItem.FixedPurchaseItemOrderCount = null;
				orderItem.FixedPurchaseItemShippedCount = null;
			}
		}
		var result = (new OrderService().UpdateItemForModify(
			orderItems,
			this.LoginOperatorName,
			updateHistoryAction,
			accessor) > 0);
		return result;
	}

	/// <summary>
	/// 注文セットプロモーション情報更新
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>true：成功、false：失敗</returns>
	private bool UpdateOrderSetPromotion(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var orderSetPromotions = this.OrderInput.SetPromotions.Select(sp => sp.CreateModel()).ToArray();
		if (orderSetPromotions.Length == 0) return true;

		var result = (new OrderService().UpdateSetPromotionForModify(
			orderSetPromotions,
			this.LoginOperatorName,
			updateHistoryAction,
			accessor) > 0);
		return result;
	}

	/// <summary>
	/// 税率毎価格情報更新
	/// </summary>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>true：成功、false：失敗</returns>
	private bool UpdateOrderPriceByTaxRate(SqlAccessor accessor)
	{
		var orderPriceByTaxRates = this.OrderInput.OrderPriceByTaxRates.Select(orderPriceByTaxRate => orderPriceByTaxRate.CreateModel()).ToArray();
		var resultCount = 0;
		if (orderPriceByTaxRates.Length == 0) return true;
		resultCount += new OrderService().UpdateOrderPriceInfoByTaxRateModify(
			orderPriceByTaxRates,
			accessor);

		return resultCount > 0;
	}

	/// <summary>
	/// 注文クーポン情報追加
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>true：成功、false：失敗</returns>
	private bool InsertOrderCoupon(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var orderCoupon = this.OrderInput.Coupon.CreateModel();
		var result = (new OrderService().InsertCoupon(orderCoupon, updateHistoryAction, accessor) > 0);
		return result;
	}

	/// <summary>
	/// 注文クーポン情報更新
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>true：成功、false：失敗</returns>
	private bool UpdateOrderCoupon(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var orderCoupon = this.OrderInput.Coupon.CreateModel();
		var result = (new OrderService().UpdateCoupon(orderCoupon, updateHistoryAction, accessor) > 0);
		return result;
	}

	/// <summary>
	/// 注文クーポン情報削除
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>true：成功、false：失敗</returns>
	private bool DeleteOrderCoupon(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var orderCoupon = this.OrderInputOld.Coupon.CreateModel();
		var result = (new OrderService().DeleteCouponByCouponNo(
			orderCoupon.OrderId,
			orderCoupon.OrderCouponNo,
			this.LoginOperatorName,
			updateHistoryAction,
			accessor) > 0);
		return result;
	}

	/// <summary>
	/// 在庫更新
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	private bool UpdateOrderProductStock(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		// 変更前後の注文商品取得
		var orderItems = this.OrderInput.Shippings.SelectMany(s => s.Items);
		var orderItemsOld = this.OrderInputOld.Shippings.SelectMany(s => s.Items);

		// 在庫更新用 商品別数量増減リスト作成
		var productStockUpdateListOld = CreateProductStockAdjustList(null, orderItemsOld.ToList(), true, accessor);
		var productStockUpdateList = CreateProductStockAdjustList(productStockUpdateListOld, orderItems.ToList(), false, accessor);

		var inputsList = new List<Hashtable>();
		foreach (var productStockUpdate in productStockUpdateList)
		{
			// 変更前後で増減のない場合は更新対象に追加しない
			if (productStockUpdate.AdjustmentQuantity == 0)
			{
				continue;
			}

			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTSTOCK_SHOP_ID, productStockUpdate.ShopId },
				{ Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, productStockUpdate.ProductId },
				{ Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, productStockUpdate.VariationId },
				{ "delete_count", productStockUpdate.AdjustmentQuantity },
				{ Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED, this.LoginOperatorName }
			};
			inputsList.Add(input);
		}

		// 商品在庫更新（トランザクションの更新順を考慮）
		using (var statement = new SqlStatement("OrderModify", "UpdateProductStock"))
		{
			foreach (var input in inputsList)
			{
				// 商品在庫更新
				if (statement.ExecStatement(accessor, input) <= 0)
				{
					return false;
				}
			}
		}

		// 商品在庫履歴追加（トランザクションの更新順を考慮）
		using (var statementForStockHistory = new SqlStatement("ProductStock", "InsertProductStockHistory"))
		{
			foreach (var input in inputsList)
			{
				var inputForStockHistory = new Hashtable
				{
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID, this.OrderInput.OrderId },
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID, input[Constants.FIELD_PRODUCTSTOCK_SHOP_ID] },
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID, input[Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID] },
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID, input[Constants.FIELD_PRODUCTSTOCK_VARIATION_ID] },
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS, Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER_MODYIFY },
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK, (int)input["delete_count"] * -1 },
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK, 0 },
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_B, 0 },
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_C, 0 },
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_RESERVED, 0 },
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO, string.Empty },
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_LAST_CHANGED, this.LoginOperatorName }
				};

				if (statementForStockHistory.ExecStatement(accessor, inputForStockHistory) <= 0)
				{
					return false;
				}
			}
		}

		// 更新履歴登録
		if ((updateHistoryAction == UpdateHistoryAction.Insert) && (inputsList.Count > 0))
		{
			new UpdateHistoryService().InsertForOrder(
				(string)inputsList[0][Constants.FIELD_ORDER_ORDER_ID],
				this.LoginOperatorName,
				accessor);
		}
		return true;
	}

	/// <summary>
	/// ユーザクーポン情報更新(未使用→使用済み)
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>処理結果</returns>
	private bool UpdateUsedUserCoupon(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var result = new CouponService().UpdateUserCouponUseFlg(
			this.OrderInput.UserId,
			this.OrderInput.Coupon.DeptId,
			this.OrderInput.Coupon.CouponId,
			int.Parse(this.OrderInput.Coupon.CouponNo),
			true,
			DateTime.Now,
			this.LoginOperatorName,
			updateHistoryAction,
			accessor);

		return result;
	}

	/// <summary>
	/// ユーザクーポン情報更新(使用済み→未使用)
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>処理結果</returns>
	private bool UpdateUnUseUserCoupon(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var userCoupon = new UserCouponModel()
	{
		UserId = this.OrderInputOld.UserId,
		DeptId = this.OrderInputOld.Coupon.DeptId,
		CouponId = this.OrderInputOld.Coupon.CouponId,
		CouponNo = int.Parse(this.OrderInputOld.Coupon.CouponNo),
		LastChanged = this.LoginOperatorName,
	};
		var result = new CouponService().UpdateUnUseUserCoupon(userCoupon, updateHistoryAction, accessor);
		return (result > 0);
	}

	/// <summary>
	/// クーポン残り利用回数をマイナス１する
	/// </summary>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>処理結果</returns>
	private bool UpdateCouponCountDown(SqlAccessor accessor)
	{
		var result = new CouponService().UpdateCouponCountDown(
			this.OrderInput.Coupon.DeptId,
			this.OrderInput.Coupon.CouponId,
			this.OrderInput.Coupon.CouponCode,
			this.LoginOperatorName,
			accessor);
		return result;
	}

	/// <summary>
	/// クーポン残り利用回数をプラス１する
	/// </summary>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>処理結果</returns>
	private bool UpdateCouponCountUp(SqlAccessor accessor)
	{
		var result = OrderCommon.UpdateCouponCountUp(
			this.OrderInputOld.Coupon.DeptId,
			this.OrderInputOld.Coupon.CouponId,
			this.OrderInputOld.Coupon.CouponCode,
			this.LoginOperatorName,
			accessor);
		return result;
	}

	/// <summary>
	/// クーポン利用情報ユーザー登録
	/// </summary>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>処理結果</returns>
	private bool InsertCouponUseUser(SqlAccessor accessor)
	{
		var couponUseUser = new CouponUseUserModel
		{
			CouponId = this.OrderInput.Coupon.CouponId,
			CouponUseUser = (Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE == Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS)
				? this.OrderInput.Owner.OwnerMailAddr
				: this.OrderInput.UserId,
			OrderId = this.OrderInput.OrderId,
			FixedPurchaseId = this.OrderInput.FixedPurchaseId,
			LastChanged = this.LoginOperatorName
		};
		var result = (new CouponService().InsertCouponUseUser(couponUseUser, accessor) > 0);
		return result;
	}

	/// <summary>
	/// クーポン利用ユーザー情報削除
	/// </summary>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>処理結果</returns>
	private bool DeleteCouponUseUser(SqlAccessor accessor)
	{
		var result = OrderCommon.DeleteCouponUseUser(
			this.OrderInputOld.Coupon.CouponId,
			this.OrderInputOld.UserId,
			this.OrderInputOld.Owner.OwnerMailAddr,
			accessor);
		return result;
	}

	/// <summary>
	/// ユーザークーポン履歴登録
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="historyKbn">履歴区分</param>
	/// <param name="actionKbn">アクション区分</param>
	/// <param name="couponInc">加算数</param>
	/// <param name="couponPrice">クーポン金額</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>処理結果</returns>
	private bool InsertUserCouponHistory(OrderInput order, string historyKbn, string actionKbn, int couponInc, decimal couponPrice, SqlAccessor accessor)
	{
		var result = OrderCommon.InsertUserCouponHistory(
			order.UserId,
			order.Coupon.DeptId,
			order.Coupon.CouponId,
			order.Coupon.CouponCode,
			order.OrderId,
			historyKbn,
			actionKbn,
			couponInc,
			couponPrice,
			this.LoginOperatorName,
			accessor);
		return result;
	}

	/// <summary>
	/// 定期購入継続分析更新
	/// </summary>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>true：成功、false：失敗</returns>
	private bool UpdateFixedPurchaseRepeatAnalysis(SqlAccessor accessor)
	{
		if (this.OrderInput.FixedPurchaseId == "") return true;

		var service = new FixedPurchaseRepeatAnalysisService();
		var items = service.GetRepeatAnalysisByOrderId(this.OrderInput.OrderId, accessor);

		// 削除
		foreach (var item in items)
		{
			if (this.OrderInput.Shippings.SelectMany(s => s.Items)
				.Where(i => i.DeleteTarget == false)
				.Select(i => i.CreateModel())
				.ToArray().Any(i => ((i.ProductId == item.ProductId) && (i.VariationId == item.VariationId)))) continue;

			service.DeleteAnalysisOrder(
					this.OrderInput.UserId,
					item.ProductId,
					item.VariationId,
					this.OrderInput.OrderId,
					this.OrderInput.LastChanged,
					accessor);
		}

		foreach (var orderItems in this.OrderInput.Shippings.SelectMany(s => s.Items)
			.Where(i => i.DeleteTarget == false).Select(i => i.CreateModel()).ToArray())
		{
			service.ModifyOrderItem(
					this.OrderInput.UserId,
					orderItems.ProductId,
					orderItems.VariationId,
					this.OrderInput.OrderId,
					this.OrderInput.FixedPurchaseId,
					this.OrderInput.LastChanged,
					accessor);
		}
		return true;
	}

	/// <summary>
	/// Exec gmo cvs payment type
	/// </summary>
	/// <param name="orderOld">Order old</param>
	/// <param name="orderNew">Order new</param>
	/// <returns>Error messages</returns>
	private string ExecGmoCvsPaymentType(OrderModel orderOld, OrderModel orderNew)
	{
		if (Constants.PAYMENT_CVS_KBN != Constants.PaymentCvs.Gmo) return string.Empty;

		var paymentGmoCvs = new PaymentGmoCvs();

		// Create payment
		if ((orderOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
			&& (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE))
		{
			// Get order id
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(orderOld.ShopId);
			//「「コンビニ・GMO」」
			var textPaymentName = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ORDER,
				Constants.VALUETEXT_PARAM_ORDER_MODIFY_CONFIRM,
				Constants.VALUETEXT_PARAM_ORDER_MODIFY_PAYMENT_GMO);

			if (paymentGmoCvs.EntryTran(paymentOrderId, orderOld.OrderPriceTotal) == false)
			{
				paymentGmoCvs.CreateLogInfoForPaymmentGmo(textPaymentName, orderOld);
				return paymentGmoCvs.ErrorMessages;
			}

			var success = paymentGmoCvs.ExecTran(
				paymentOrderId,
				this.GmoCvsType,
				orderNew.Owner.OwnerName,
				orderNew.Owner.OwnerNameKana,
				orderNew.Owner.OwnerTel1.Replace("-", string.Empty));

			if (success)
			{
				this.OrderInput.CardTranId = string.Format("{0} {1}",
					paymentGmoCvs.AccessId,
					paymentGmoCvs.AccessPass);
				this.OrderInput.PaymentOrderId = paymentOrderId;
				this.OrderInput.PaymentMemo = string.Format("{0} {1} {2} {3}",
					paymentGmoCvs.ConfNo,
					paymentGmoCvs.ReceiptNo,
					paymentGmoCvs.PaymentTerm,
					paymentGmoCvs.ReceiptUrl);
			}
			else
			{
				paymentGmoCvs.CreateLogInfoForPaymmentGmo(textPaymentName, orderOld);
				return paymentGmoCvs.ErrorMessages;
			}
		}

		return string.Empty;
	}

	/// <summary>
	/// 購入商品を過去に購入したことがあるか（類似配送先を含む）
	/// </summary>
	/// <returns>購入ユーザーに重複情報が含まれるか</returns>
	public OrderErrorcode CheckProductOrderLimit()
	{
		var result = OrderErrorcode.NoError;
		this.BeforeExtendStatus39 = this.OrderInput.ExtendStatus39;
		var productIdList = string.Format("'{0}'", string.Join("','", this.OrderInput.Shippings.SelectMany(shipping => shipping.Items.Where(i => i.DeleteTarget == false)
				.Select(product => product.ProductId))));
		var order = this.OrderInput.CreateModel();

		if (this.OrderInput.IsFixedPurchaseOrder)
		{
			this.ProductOrderLmitOrderIds = this.OrderInput.Shippings.SelectMany(shipping => shipping.Items.Where(i => i.DeleteTarget == false)
				.SelectMany(product => new OrderService().GetOrderIdForFixedProductOrderLimitCheck(
					shipping.CreateModel(),
					order,
					order.Owner,
					this.OrderInput.ShopId,
					productIdList,
					new string[] { this.OrderInput.OrderId }))).ToArray();
			//this.ProductOrderLmitOrderIds = historyOrderIds;
			this.OrderInput.ExtendStatus39 =
				this.HasOrderHistorySimilarShipping
					? Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON
					: Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			tableNotProductOrderLimitErrorMessages.Visible = this.HasOrderHistorySimilarShipping;
		}
		else
		{
			var historyOrderIds = this.OrderInput.Shippings.SelectMany(shipping => shipping.Items.Where(i => i.DeleteTarget == false)
				.SelectMany(product => new OrderService().GetOrderIdForProductOrderLimitCheck(
					shipping.CreateModel(),
					order,
					order.Owner,
					product.ShopId,
					productIdList,
					new string[] { this.OrderInput.OrderId }))).Distinct().ToArray();
				this.ProductOrderLmitOrderIds = historyOrderIds;
				this.OrderInput.ExtendStatus39 =
					this.HasOrderHistorySimilarShipping
						? Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON
						: Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF;
			tableNotProductOrderLimitErrorMessages.Visible = this.HasOrderHistorySimilarShipping;
		}
		return result;
	}

	/// <summary>
	/// Check Uniform
	/// </summary>
	protected void SetVisibleForUniformOption(string uniformType)
	{
		var isPersonal = false;
		var isCompany = false;
		var isDonate = false;

		OrderCommon.CheckUniformType(
			uniformType,
			ref isPersonal,
			ref isDonate,
			ref isCompany);

		this.IsCompany = isCompany;
		this.IsDonate = isDonate;
		this.IsPersonal = isPersonal;
	}

	/// <summary>
	/// Get Taiwan Carry Type Option
	/// </summary>
	/// <returns>string.Empty | 携帯載具コード : TwCarryTypeOption | 自然人証明コード : TwCarryTypeOption</returns>
	public string GetTwCarryTypeOption()
	{
		if (this.TwOrderInvoiceInput == null) return string.Empty;

		if (string.IsNullOrEmpty(this.TwOrderInvoiceInput.TwCarryType))
		{
			return ValueText.GetValueText(
				Constants.TABLE_TWORDERINVOICE,
				Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE, this.TwOrderInvoiceInput.TwCarryType);
		}
		else
		{
			return string.Format("{0} ： {1}",
				ValueText.GetValueText(
					Constants.TABLE_TWORDERINVOICE,
					Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE, this.TwOrderInvoiceInput.TwCarryType),
				this.TwOrderInvoiceInput.TwCarryTypeOption);
		}
	}

	/// <summary>
	/// Update Online Payment Status
	/// </summary>
	/// <param name="orderNew">Order New</param>
	/// <param name="accessor">Accessor</param>
	private void UpdateOnlinePaymentStatus(OrderModel orderNew, SqlAccessor accessor)
	{
		if ((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			&& (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED))
		{
			new OrderService().UpdateOnlinePaymentStatus(
				this.OrderInput.OrderId,
				Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE,
				this.LoginOperatorName,
				UpdateHistoryAction.DoNotInsert,
				accessor);
		}
	}

	/// <summary>
	/// Check can use point for purchase
	/// </summary>
	protected void CheckCanUsePointForPurchase()
	{
		var usePoint = decimal.Parse(this.OrderInput.OrderPointUse);
		if ((Constants.POINT_MINIMUM_PURCHASEPRICE == 0)
			|| (usePoint == 0))
		{
			return;
		}

		var priceTotalPurchase = this.OrderInput.GetPurchasePriceTotal();
		var canUsePointForPurchase = (priceTotalPurchase >= Constants.POINT_MINIMUM_PURCHASEPRICE);
		dvMessagePointMinimum.Visible = (canUsePointForPurchase == false);

		if (canUsePointForPurchase == false)
		{
			lMessagePointMinimum.Text = GetPointMinimumPurchasePriceErrorMessage(this.OrderInput.SettlementCurrency);
		}
	}
	#endregion

	#region プロパティ
	/// <summary>注文入力情報情報</summary>
	protected OrderInput OrderInput
	{
		get { return (OrderInput)ViewState["OrderInput"]; }
		set { ViewState["OrderInput"] = value; }
	}
	/// <summary>注文入力情報情報（変更前）</summary>
	protected OrderInput OrderInputOld
	{
		get { return (OrderInput)ViewState["OrderInputOld"]; }
		set { ViewState["OrderInputOld"] = value; }
	}
	/// <summary>元注文情報（元注文 or 最後の返品注文）</summary>
	protected OrderModel OrderOld
	{
		get { return (OrderModel)ViewState["OrderOld"]; }
		set { ViewState["OrderOld"] = value; }
	}
	/// <summary>ユーザ情報</summary>
	protected new UserModel User
	{
		get { return (UserModel)ViewState["User"]; }
		set { ViewState["User"] = value; }
	}
	/// <summary>配送種別情報</summary>
	protected ShopShippingModel ShopShipping
	{
		get { return (ShopShippingModel)ViewState["ShopShipping"]; }
		set { ViewState["ShopShipping"] = value; }
	}
	/// <summary>配送方法情報</summary>
	protected string[] ShippingMethod
	{
		get { return (string[])ViewState["ShippingMethod"]; }
		set { ViewState["ShippingMethod"] = value; }
	}
	/// <summary>配送種別情報</summary>
	protected DeliveryCompanyModel[] DeliveryCompany
	{
		get { return (DeliveryCompanyModel[])ViewState["DeliveryCompany"]; }
		set { ViewState["DeliveryCompany"] = value; }
	}
	/// <summary>ユーザーポイント情報</summary>
	protected UserPointList UserPointList
	{
		get { return (UserPointList)ViewState["UserPointList"]; }
		set { ViewState["UserPointList"] = value; }
	}
	/// <summary>変更前ユーザーポイント情報</summary>
	protected UserPointList UserPointListOld
	{
		get { return (UserPointList)ViewState["UserPointListOld"]; }
		set { ViewState["UserPointListOld"] = value; }
	}
	/// <summary>注文クレジットカード情報</summary>
	protected OrderCreditCardInput OrderCreditCard
	{
		get { return (OrderCreditCardInput)ViewState["OrderCreditCard"]; }
		set { ViewState["OrderCreditCard"] = value; }
	}
	/// <summary>変更前の決済・再与信処理区分</summary>
	protected ReauthCreatorFacade.ExecuteTypes OldExecuteType
	{
		get { return (ReauthCreatorFacade.ExecuteTypes)ViewState["OldExecuteType"]; }
		set { ViewState["OldExecuteType"] = value; }
	}
	/// <summary>変更後の決済・再与信処理区分</summary>
	protected ReauthCreatorFacade.ExecuteTypes NewExecuteType
	{
		get { return (ReauthCreatorFacade.ExecuteTypes)ViewState["NewExecuteType"]; }
		set { ViewState["NewExecuteType"] = value; }
	}
	/// <summary>ユーザーか会員かどうか</summary>
	protected bool IsUser
	{
		get
		{
			return UserService.IsUser(this.User.UserKbn);
		}
	}
	/// <summary>更新時に表示するアラート</summary>
	protected string UpdateAlertMessage
	{
		get { return (string)ViewState["UpdateAlertMessage"]; }
		set { ViewState["UpdateAlertMessage"] = value; }
	}
	/// <summary>アイテムテーブル用追加カラム数</summary>
	protected int AddColumnCountForItemTable
	{
		get
		{
			return
				(Constants.PRODUCT_SALE_OPTION_ENABLED ? 1 : 0)
				+ ((Constants.NOVELTY_OPTION_ENABLED || Constants.RECOMMEND_OPTION_ENABLED) ? 1 : 0)
				+ (this.OrderInput.HasSetProduct ? 1 : 0)
				+ (Constants.PRODUCTBUNDLE_OPTION_ENABLED ? 1 : 0)
				+ (this.IsFixedPurchaseCountAreaShow ? 2 : 0)
				+ (this.DisplayItemSubscriptionBoxCourseIdArea ? 1 : 0);
		}
	}
	/// <summary>金額変更API実行するか</summary>
	protected bool ExecChangePriceApiOn
	{
		get { return (bool)ViewState["ExecChangePriceApiOn"]; }
		set { ViewState["ExecChangePriceApiOn"] = value; }
	}
	/// <summary> 他の返品／交換注文を作成した後、既に返金済みの交換注文を編集するかどうか </summary>
	protected bool IsNotLastExchangeOrderModified()
	{
		return this.OrderInput.IsExchangeOrder && (this.OrderOld.OrderId != this.OrderInput.OrderId);
	}
	/// <summary>ユーザーポイント戻し時の有効期限切れアラートメッセージ</summary>
	protected string UserPointExpiredAlertMessage
	{
		get { return (string)Session[Constants.SESSION_KEY_USERPOINT_EXPIRED_ALEAT_MESSAGE]; }
	}
	/// <summary>過去に定期購入の履歴があるか（類似配送先含む）</summary>
	protected bool HasOrderHistorySimilarShipping
	{
		get { return (this.ProductOrderLmitOrderIds.Length > 0); }
	}
	/// <summary>過去の購入履歴の注文ID</summary>
	public string[] ProductOrderLmitOrderIds
	{
		get { return (string[])ViewState["ProductOrderLmitOrderIds"]; }
		set { ViewState["ProductOrderLmitOrderIds"] = value; }
	}
	/// <summary>編集前の拡張項目39</summary>
	public string BeforeExtendStatus39
	{
		get { return (string)ViewState["BeforeExtendStatus39"]; }
		set { ViewState["BeforeExtendStatus39"] = value; }
	}
	/// <summary>新規クレジットカード利用するか</summary>
	public bool UseNewCreditCard
	{
		get { return (bool)(ViewState["UseNewCreditCard"] ?? false); }
		set { ViewState["UseNewCreditCard"] = value; }
	}
	/// <summary>Is Update Shipping Convenience</summary>
	protected bool IsUpdateShippingConvenience
	{
		get
		{
			var result = ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE_SHIPPING)
				&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED);
			return result;
		}
	}
	/// <summary>Gmo cvs type</summary>
	protected string GmoCvsType
	{
		get { return (string)ViewState["GmoCvsType"]; }
		set { ViewState["GmoCvsType"] = value; }
	}
	/// <summary>配送不可エリアかどうか</summary>
	protected bool IsUnavailableShippingArea { get; set; }
	/// <summary>定期購入回数エリア表示するかどうか</summary>
	protected bool IsFixedPurchaseCountAreaShow
	{
		get
		{
			var result = (this.OrderInput.Shippings[0].Items.Any(orderItem => orderItem.IsFixedPurchaseItem)
				&& Constants.FIXEDPURCHASE_OPTION_ENABLED);
			return result;
		}
	}
	/// <summary>PayPay(Sbps, Veritrans)決済の場合で売り上げ確定後返金処理が成功しているかどうか</summary>
	private bool IsSucceedRefundAfterSalsesFixedInPaypay { get; set; }
	/// <summary>注文商品の頒布会コースIDエリアを表示するか</summary>
	protected bool DisplayItemSubscriptionBoxCourseIdArea
	{
		get
		{
			return Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && this.OrderInput.IsOrderCombinedWithSubscriptionBoxItem;
		}
	}
	/// <summary>表示用注文商品頒布会コースID（エンコード済み）</summary>
	protected string EncodedSubscriptionBoxCourseIdForDisplay
	{
		get
		{
			if (this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem())
			{
				return HtmlSanitizer.HtmlEncode(this.OrderInput.SubscriptionBoxCourseId);
			}

			var result = HtmlSanitizer.HtmlEncodeChangeToBr(
				string.Join(Environment.NewLine, this.OrderInput.ItemSubscriptionBoxCourseIds));
			return result;
		}
	}
	#endregion
}
