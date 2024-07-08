/*
=========================================================================================================
  Module      : Front用注文情報更新実行クラス（注文完了ページ向けレコメンドで利用） (OrderUpdaterFront.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.Domain.Coupon;
using w2.Domain.FixedPurchaseRepeatAnalysis;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.Point.Helper;
using w2.Domain.Product;
using w2.Domain.ProductStock;
using w2.Domain.ProductStock.Helper;
using w2.Domain.ProductStockHistory;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.App.Common;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.Reauth;
using w2.App.Common.Order.Register;
using w2.App.Common.Mail;
using w2.App.Common.Order.Payment;
using w2.Domain.FixedPurchase;
using w2.Domain.MailTemplate;
using w2.App.Common.CrossPoint.Point;
using w2.App.Common.Option.CrossPoint;

/// <summary>
/// 注文情報更新実行クラス（注文完了ページ向けレコメンドで利用）
/// </summary>
public class OrderUpdaterFront
{
	#region 定数
	/// <summary>更新実行結果</summary>
	public enum ResultType
	{
		/// <summary>成功</summary>
		Success,
		/// <summary>失敗</summary>
		Fail,
		/// <summary>在庫切れ</summary>
		OutOfStock,
	}
	#endregion

	#region コンストラクタ
	/// <summary>
	/// コンストラクタ（注文キャンセル実行時に利用）
	/// </summary>
	/// <param name="user">ユーザー情報</param>
	/// <param name="order">注文情報</param>
	/// <param name="lastChanged">最終更新者</param>
	public OrderUpdaterFront(
		UserModel user,
		OrderModel order,
		string lastChanged)
	{
		this.User = user;
		this.Order = order;
		this.LastChanged = lastChanged;
		this.IsAuthResultHold = false;
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="user">ユーザー情報</param>
	/// <param name="order">注文情報</param>
	/// <param name="orderOld">更新前注文情報</param>
	/// <param name="userMemo">更新用ユーザメモ</param>
	/// <param name="userManagementLevelId">更新用ユーザ管理レベルID</param>
	/// <param name="lastChanged">最終更新者</param>
	/// <param name="isMyPageModify">マイページからの受注編集か</param>
	public OrderUpdaterFront(
		UserModel user,
		OrderModel order,
		OrderModel orderOld,
		string userMemo,
		string userManagementLevelId,
		string lastChanged,
		bool isMyPageModify = false)
		: this(user, order, lastChanged)
	{
		this.OrderOld = orderOld;
		this.UserMemo = userMemo;
		this.UserManagementLevelId = userManagementLevelId;
		this.IsMyPageModify = isMyPageModify;
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="user">ユーザー情報</param>
	/// <param name="order">注文情報</param>
	/// <param name="orderOld">更新前注文情報</param>
	/// <param name="userMemo">更新用ユーザメモ</param>
	/// <param name="userManagementLevelId">更新用ユーザ管理レベルID</param>
	/// <param name="lastChanged">最終更新者</param>
	/// <param name="userPointList">ユーザポイント情報</param>
	/// <param name="userPointListOld">更新前ユーザポイント情報</param>
	public OrderUpdaterFront(
		UserModel user,
		OrderModel order,
		OrderModel orderOld,
		string userMemo,
		string userManagementLevelId,
		string lastChanged,
		UserPointList userPointList,
		UserPointList userPointListOld)
		: this(user, order, orderOld, userMemo, userManagementLevelId, lastChanged)
	{
		this.UserPoint = userPointList;
		this.UserPointOld = userPointListOld;
	}
	#endregion

	/// <summary>
	/// 変更後の注文情報生成（DB更新は行わない）
	/// </summary>
	/// <param name="doStockCooperation">在庫連動するか</param>
	/// <param name="doAlertOutOfStock">在庫切れ時に警告を出すか</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="cart">カート</param>
	/// <returns>エラーの場合はエラーが発生した時点のトランザクション名</returns>
	public string CreateOrderNew(bool doStockCooperation, bool doAlertOutOfStock, UpdateHistoryAction updateHistoryAction, CartObject cart)
	{
		var transactionName = string.Empty;
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			try
			{
				var executeResult = ExecuteUpdateOrder(doStockCooperation, doAlertOutOfStock, updateHistoryAction, accessor, cart, out transactionName);
				// 失敗または在庫切れの場合
				if ((executeResult == ResultType.Fail) || (executeResult == ResultType.OutOfStock)) return executeResult.ToString();

				this.Order = new OrderService().Get(this.Order.OrderId, accessor);
			}
			catch (Exception ex)
			{
				// ログを記録
				AppLogger.WriteError(ex);
				return transactionName;
			}
			return string.Empty;
		}
	}

	/// <summary>
	/// 外部決済連携実行
	/// </summary>
	/// <param name="oldExecuteType">更新前注文の決済・再与信処理区分</param>
	/// <param name="newExecuteType">更新注文の決済・再与信処理区分</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="errorMessage">エラーメッセージ</param>
	/// <returns>外部決済連携実行したか</returns>
	public bool ExecuteExternalPayment(
		ReauthCreatorFacade.ExecuteTypes oldExecuteType,
		ReauthCreatorFacade.ExecuteTypes newExecuteType,
		UpdateHistoryAction updateHistoryAction,
		out string errorMessage)
	{
		errorMessage = string.Empty;
		if (Constants.PAYMENT_REAUTH_ENABLED == false) return false;

		var reauth = new ReauthCreatorFacade(
			this.OrderOld,
			this.Order,
			oldExecuteType,
			newExecuteType,
			ReauthCreatorFacade.OrderActionTypes.Modify).CreateReauth();
		var reauthResult = reauth.Execute();

		var isResultDetailSuccess = reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success;

		// 外部決済連携実施時のみログを残す
		if (reauth.HasAnyAction)
		{
			// 外部決済連携ログ追加
			OrderCommon.AppendExternalPaymentCooperationLog(
				isResultDetailSuccess,
				this.Order.OrderId,
				isResultDetailSuccess ? LogCreator.CreateMessage(this.OrderOld.OrderId, "") : reauthResult.ApiErrorMessages,
				this.LastChanged,
				UpdateHistoryAction.Insert);
		}

		if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure)
		{
			// Atodeneの場合、与信エラーにする
			if (reauth.AuthLostForError)
			{
				var service = new OrderService();
				service.UpdateExternalPaymentInfoForAuthError(
					this.Order.OrderId,
					reauthResult.ErrorMessages,
					this.Order.LastChanged,
					UpdateHistoryAction.Insert);
			}
			errorMessage = reauthResult.ErrorMessages;
			return false;
		}

		if (this.IsNotLastExchangeOrderModified)
		{
			SetExternalPaymentInfoReauthSplit(reauth, reauthResult);

			this.Order.CardTranId = (string.IsNullOrEmpty(this.RegisterCardTranId) == false)
				? this.RegisterCardTranId
				: this.OrderOld.CardTranId;
			this.Order.PaymentOrderId = (string.IsNullOrEmpty(this.RegisterPaymentOrderId) == false)
				? this.RegisterPaymentOrderId
				: this.OrderOld.PaymentOrderId;
			this.Order.ExternalPaymentAuthDate = this.RegisterExternalPaymentAuthDate;
			this.Order.PaymentMemo = this.Order.PaymentMemo + Environment.NewLine + this.RegisterPaymentMemo;
			this.Order.ExternalPaymentStatus = (string.IsNullOrEmpty(this.RegisterExternalPaymentStatus) == false)
				? this.RegisterExternalPaymentStatus
				: OrderCommon.CanPaymentReauth(this.Order.OrderPaymentKbn)
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
					: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
			errorMessage = this.ReturnOrderReauthErrorMessages;

			return false;
		}
		else
		{
			if (reauthResult.ResultDetail != ReauthResult.ResultDetailTypes.Success)
			{
				errorMessage = reauthResult.ErrorMessages + Environment.NewLine + WebMessages.GetMessages(WebMessages.ERRMSG_EXTERNAL_PAYMENT_CANCEL_FAILED);
			}

			if (string.IsNullOrEmpty(reauthResult.PaymentMemo) == false)
			{
				this.Order.PaymentMemo = string.Join(Environment.NewLine, this.Order.PaymentMemo, reauthResult.PaymentMemo);
				if (reauth.HasAnyAction)
				{
					this.Order.CardTranId = reauthResult.CardTranId;
					this.Order.PaymentOrderId = reauthResult.PaymentOrderId;
				}
				if (reauth.HasReauthOrReduceOrReprint)
				{
					this.Order.ExternalPaymentAuthDate = reauth.GetUpdateReauthDate(
						this.OrderOld.ExternalPaymentAuthDate,
						this.OrderOld.OrderPaymentKbn,
						this.Order.OrderPaymentKbn);
					this.Order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
				}
				else if (reauth.HasOnlyCancel)
				{
					this.Order.ExternalPaymentAuthDate = (DateTime?)null;
					this.Order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
				}
			}
			else if (this.OrderOld.OrderPaymentKbn != this.Order.OrderPaymentKbn)
			{
				if ((this.Order.IsExternalPayment == false)
					|| ((this.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) 
						&& (Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN) == false))
					|| ((this.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
						&& (Constants.REAUTH_COMPLETE_CVSDEF_LIST.Contains(Constants.PAYMENT_CVS_DEF_KBN) == false)))
				{
					this.Order.CardTranId = string.Empty;
					this.Order.PaymentOrderId = string.Empty;
					this.Order.ExternalPaymentAuthDate = (DateTime?)null;
					this.Order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
				}
				else if (newExecuteType == ReauthCreatorFacade.ExecuteTypes.None)
				{
					this.Order.CardTranId = string.Empty;
					this.Order.PaymentOrderId = string.Empty;
					this.Order.ExternalPaymentAuthDate = (DateTime?)null;
					this.Order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
			}

			if (reauthResult.IsAuthResultHold)
			{
				this.Order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST;
			}

			if (reauth.HasSales
				&& ((this.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
					&& (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.VeriTrans)))
			{
				this.Order.OnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
			}

			return reauth.HasAnyAction;
		}
	}

	/// <summary>
	/// 注文情報更新＆更新履歴登録
	/// </summary>
	/// <param name="isRegisterCreditCard">クレジットカード登録したか？</param>
	/// <param name="isUpdateCreditCard">クレジットカード更新したか？</param>
	/// <param name="isExecuteExternalPayment">外部決済連携実行したか？</param>
	/// <param name="doStockCooperation">在庫情報更新を行うか？</param>
	/// <param name="doAlertOutOfStock">在庫切れ時に警告を出すか</param>
	/// <param name="actionType">注文履歴アクション種別</param>
	/// <param name="updateHisoryAction">更新履歴アクション</param>
	/// <param name="cart">カート</param>
	/// <param name="errorMessage">エラーメッセージ</param>
	/// <param name="newOrder">受注情報</param>
	/// <param name="isOrderHisoryBulkUpdate">受注履歴一括更新</param>
	/// <returns>実行結果</returns>
	public ResultType ExecuteUpdateOrderAndRegisterUpdateHistory(
		bool isRegisterCreditCard,
		bool isUpdateCreditCard,
		bool isExecuteExternalPayment,
		bool doStockCooperation,
		bool doAlertOutOfStock,
		OrderHistory.ActionType actionType,
		UpdateHistoryAction updateHisoryAction,
		CartObject cart,
		out string errorMessage,
		OrderModel newOrder = null,
		bool isOrderHisoryBulkUpdate = false)
	{
		var transactionName = string.Empty;
		errorMessage = string.Empty;
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			try
			{
				var orderHistory = new OrderHistory
				{
					OrderId = this.Order.OrderId,
					Action = actionType,
					OpearatorName = this.LastChanged,
					Accessor = accessor
				};
				orderHistory.HistoryBegin();

				if (isExecuteExternalPayment)
				{
					this.Order.LastAuthFlg = Constants.FLG_ORDER_LAST_AUTH_FLG_ON;

					// 他の返品・交換注文を作成した後、既に返金済みの交換注文を編集する場合、編集前の注文情報設定
					if (this.IsNotLastExchangeOrderModified)
					{
						ExecuteUpdateOrderOld(UpdateHistoryAction.DoNotInsert, accessor, out transactionName);
					}
				}

				// 管理メモ
				var productIdList = string.Format("'{0}'", string.Join("','", this.Order.Shippings.SelectMany(x => x.Items.Select(y => y.ProductId))));
				var notFirstTimeOrderIds = this.Order.IsFixedPurchaseOrder
					? new OrderService().GetOrderIdForFixedProductOrderLimitCheck(
						this.Order.Shippings.First(),
						this.Order,
						this.Order.Owner,
						this.Order.ShopId,
						productIdList,
						new string[] { this.Order.OrderId },
						accessor).ToArray()
					: new string[0];
				this.Order.ManagementMemo = OrderCommon.GetNotFirstTimeFixedPurchaseManagementMemo(
					this.Order.ManagementMemo,
					notFirstTimeOrderIds,
					false,
					this.OrderOld.ExtendStatus39,
					this.Order.ExtendStatus39);

				// 更新
				var executeResult = ExecuteUpdateOrder(doStockCooperation, doAlertOutOfStock, updateHisoryAction, accessor, cart, out transactionName, isOrderHisoryBulkUpdate);

				// 在庫切れ
				if (executeResult == ResultType.OutOfStock)
				{
					AppLogger.WriteError(WebMessages.GetMessages(WebMessages.ERRMSG_PRODUCTSTOCK_OUT_OF_STOCK_ERROR));
					return executeResult;
				}
					
				if ((executeResult == ResultType.Success) && isRegisterCreditCard)
				{
					transactionName = "ユーザークレジットカード枝番更新";
					var userCreditCard = new UserCreditCard(new UserCreditCardService().Get(this.Order.UserId, this.Order.CreditBranchNo.Value, accessor));
					if ((string.IsNullOrEmpty(userCreditCard.CardDispName) == false)
						&& (userCreditCard.CardDispName != Constants.CREDITCARD_UNREGIST_DEFAULT_DISPLAY_NAME))
					{
						var isSuccess = new UserCreditCardService().UpdateDispFlg(
							userCreditCard.UserId,
							userCreditCard.BranchNo,
							true,
							userCreditCard.LastChanged,
							UpdateHistoryAction.DoNotInsert,
							accessor);
						if (isSuccess == false)
						{
							AppLogger.WriteError(string.Format(
								WebMessages.GetMessages(WebMessages.ERRMSG_USERCREDITCARD_CANNOT_UPDATE_DISP_FLG_ERROR),
								userCreditCard.UserId,
								userCreditCard.BranchNo));
							executeResult = ResultType.Fail;
						}
					}
				}

				if ((executeResult == ResultType.Success)
					&& (isRegisterCreditCard || isUpdateCreditCard))
				{
					new OrderService().UpdateCreditBranchNo(
						this.Order.OrderId,
						this.Order.CreditBranchNo.Value,
						this.LastChanged,
						UpdateHistoryAction.DoNotInsert,
						accessor);
				}

				// クロスポイント連携
				if ((newOrder != null) && (executeResult == ResultType.Success) && Constants.CROSS_POINT_OPTION_ENABLED)
				{
					// Changes on the Cross Point
					var discount = (newOrder.MemberRankDiscountPrice
						+ newOrder.OrderCouponUse
						+ newOrder.SetpromotionProductDiscountAmount
						+ newOrder.FixedPurchaseDiscountPrice
						+ newOrder.FixedPurchaseMemberDiscountAmount
						- newOrder.OrderPriceRegulation);

					var priceTaxIncluded = TaxCalculationUtility.GetPriceTaxIncluded(
						newOrder.OrderPriceSubtotal,
						newOrder.OrderPriceSubtotalTax);

					var input = new PointApiInput
					{
						MemberId = newOrder.UserId,
						OrderDate = newOrder.OrderDate,
						PosNo = w2.App.Common.Constants.CROSS_POINT_POS_NO,
						OrderId = newOrder.OrderId,
						BaseGrantPoint = newOrder.OrderPointAdd,
						SpecialGrantPoint = 0m,
						PriceTotalInTax = (newOrder.OrderPriceSubtotal - discount),
						PriceTotalNoTax = (priceTaxIncluded - discount),
						UsePoint = newOrder.LastOrderPointUse,
						Items = CartObject.GetOrderDetails(newOrder),
						ReasonId = CrossPointUtility.GetValue(Constants.CROSS_POINT_SETTING_ELEMENT_REASON_ID, w2.App.Common.Constants.CROSS_POINT_REASON_KBN_OPERATOR),
					};
					var result = new CrossPointPointApiService().Modify(input.GetParam(PointApiInput.RequestType.Modify));

					if (result.IsSuccess == false)
					{
						executeResult = ResultType.Fail;
					}
				}

				if (executeResult == ResultType.Success)
				{
					if (updateHisoryAction == UpdateHistoryAction.Insert)
					{
						new UpdateHistoryService().InsertAllForOrder(this.Order.OrderId, this.LastChanged, accessor);
					}

					orderHistory.HistoryComplete();

					accessor.CommitTransaction();
				}
				else
				{
					errorMessage = string.Format("{0}に失敗しました。{1}", transactionName, (isExecuteExternalPayment ? "既に外部連携実行済みです。" : string.Empty));
					AppLogger.WriteError(errorMessage);
				}
				return executeResult;
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
				return ResultType.Fail;
			}
		}
	}

	/// <summary>
	/// 定期台帳登録
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <returns>登録したか</returns>
	public bool RegisterFixedPurchaseOrder(CartObject cart, UpdateHistoryAction updateHistoryAction)
	{
		try
		{
			if (Constants.FIXEDPURCHASE_OPTION_ENABLED && cart.HasFixedPurchase)
			{
				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();

					var fixedPurchaseId = new FixedPurchaseRegister(this.Order.DataSource, cart, this.LastChanged)
						.RegisterAndUpdateFixedPurchaseInfoForOrder(
							Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_TEMP,
							UpdateHistoryAction.Insert,
							accessor);
					if (string.IsNullOrEmpty(fixedPurchaseId) == false)
					{
						this.Order.FixedPurchaseId = fixedPurchaseId;
					}

					// Update Fixed Purchase Status Temp To Normal
					new FixedPurchaseService()
						.UpdateFixedPurchaseStatusTempToNormal(
							this.Order.OrderId,
							fixedPurchaseId,
							this.LastChanged,
							UpdateHistoryAction.Insert,
							accessor);

					accessor.CommitTransaction();
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			AppLogger.WriteError(OrderCommon.CreateOrderSuccessAlertLogMessage("4-1.定期購入注文登録処理", this.Order.DataSource, cart), ex);
			return false;
		}
	}

	/// <summary>
	/// Update Fixed Purchase Order For Recommend At Order Complete
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <returns>更新したか</returns>
	public bool UpdateFixedPurchaseOrderForRecommendAtOrderComplete(CartObject cart, UpdateHistoryAction updateHistoryAction)
	{
		try
		{
			if (Constants.FIXEDPURCHASE_OPTION_ENABLED && cart.HasFixedPurchase)
			{
				new FixedPurchaseRegister(this.Order.DataSource, cart, this.LastChanged)
					.UpdateFixedPurchaseOrder(updateHistoryAction);
				// 更新履歴
				new UpdateHistoryService().InsertForFixedPurchase(this.Order.FixedPurchaseId, this.LastChanged);
				new UpdateHistoryService().InsertForUser(this.User.UserId, this.LastChanged);

				// Process Switch Product Fixed Purchase Next Shipping Second Time
				new FixedPurchaseRegister(
					this.Order.DataSource,
					cart,
					this.LastChanged)
				.ProcessSwitchProductFixedPurchaseNextShippingSecondTime(UpdateHistoryAction.Insert);
			}
			return true;
		}
		catch (Exception ex)
		{
			AppLogger.WriteError(OrderCommon.CreateOrderSuccessAlertLogMessage("4-1`.定期購入注文更新処理", this.Order.DataSource, cart), ex);
			return false;
		}
	}

	/// <summary>
	/// 注文更新メール送信
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="isUser">会員？</param>
	/// <param name="recommendItemNames">レコメンド商品名リスト</param>
	public void SendOrderUpdateMail(CartObject cart, bool isUser, string[] recommendItemNames = null)
	{
		var transactionName = "4-2-1`.注文者宛注文更新メール送信処理";
		try
		{
			// ユーザー向けメール
			SendOrderUpdateMailToUser(cart, isUser, recommendItemNames);
			// 管理者用メール送信
			if (Constants.THANKSMAIL_FOR_OPERATOR_ENABLED)
			{
				transactionName = "4-2-2.メール送信処理(管理者向け)";
				SendOrderUpdateMailToOperator(cart, isUser, recommendItemNames);
			}
		}
		catch (Exception ex)
		{
			AppLogger.WriteWarn(OrderCommon.CreateOrderSuccessAlertLogMessage(transactionName, this.Order.DataSource, cart), ex);
		}
	}

	/// <summary>
	/// 注文キャンセル処理
	/// </summary>
	/// <param name="stockCooperation">在庫連動する？</param>
	/// <param name="actionType">注文履歴アクション区分</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="errorMessage">エラーメッセージ</param>
	/// <param name="disablePaymentCancelOrderId">外部決済のキャンセルをスキップする受注IDかどうか</param>
	/// <returns>成功したか</returns>
	public bool ExecuteCanceOrder(
		bool stockCooperation,
		OrderHistory.ActionType actionType,
		UpdateHistoryAction updateHistoryAction,
		out string errorMessage,
		string disablePaymentCancelOrderId)
	{
		errorMessage = string.Empty;
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			try
			{
				var history = new OrderHistory
				{
					OrderId = this.Order.OrderId,
					Action = actionType,
					OpearatorName = this.LastChanged,
					Accessor = accessor
				};
				history.HistoryBegin();

				this.OrderOld = OrderCommon.GetLastAuthOrder(this.Order.OrderId, accessor);

				// 外部連携キャンセル処理
				var disableCancelApi = (disablePaymentCancelOrderId == this.OrderOld.OrderId);
				if (ExecuteCancelExternalPayment(accessor, out errorMessage, disableCancelApi) == false) return false;

					// ステータス更新実行
				var service = new OrderService();
				var updated = service.UpdateOrderStatus(
					this.Order.OrderId,
					Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED,
					DateTime.Now,
					this.LastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				// 注文情報キャンセル処理
				if (updated > 0)
				{
					OrderCommon.CancelOrderSubProcess(
						this.Order,
						true,
						Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER_CANCEL,
						this.Order.ShopId,
						this.LastChanged,
						stockCooperation,
						updateHistoryAction,
						accessor);
					// ユーザーリアルタイム累計購入回数処理
					var ht = new Hashtable
					{
						{Constants.FIELD_ORDER_USER_ID, this.Order.UserId},
						{Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME, new UserService().Get(this.Order.UserId).OrderCountOrderRealtime}
					};
					OrderCommon.UpdateRealTimeOrderCount(ht, Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_CANCEL, accessor);
				}

				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertAllForOrder(this.Order.OrderId, this.LastChanged, accessor);
				}

				history.HistoryComplete();

				accessor.CommitTransaction();

				return true;
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
				throw;
			}
		}
	}

	/// <summary>
	/// 注文キャンセルメール送信
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="isUser">会員？</param>
	/// <param name="recommendItemNames">レコメンド商品名リスト</param>
	public void SendOrderCancelMail(CartObject cart, bool isUser, string[] recommendItemNames = null)
	{
		var transactionName = "注文者宛注文キャンセルメール送信処理";
		try
		{
			// ユーザー向けメール
			SendOrderCancelMailToUser(cart, isUser, recommendItemNames);
			// 管理者用メール送信
			if (Constants.THANKSMAIL_FOR_OPERATOR_ENABLED)
			{
				transactionName = "キャンセルメール送信処理(管理者向け)";
				SendOrderCancelMailToOperator(cart, isUser, recommendItemNames);
			}
		}
		catch (Exception ex)
		{
			AppLogger.WriteWarn(OrderCommon.CreateOrderSuccessAlertLogMessage(transactionName, this.Order.DataSource, cart), ex);
		}
	}

	/// <summary>
	/// 注文情報更新
	/// </summary>
	/// <param name="doStockCooperation">在庫連動するか</param>
	/// <param name="doAlertOutOfStock">在庫切れの際に警告を出すか</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="cart">カート</param>
	/// <param name="transactionName">トランザクション名</param>
	/// <param name="isOrderHisoryBulkUpdate">受注履歴一括更新か</param>
	/// <returns>成功したか</returns>
	private ResultType ExecuteUpdateOrder(
		bool doStockCooperation,
		bool doAlertOutOfStock,
		UpdateHistoryAction updateHistoryAction,
		SqlAccessor accessor,
		CartObject cart,
		out string transactionName,
		bool isOrderHisoryBulkUpdate = false)
	{
		var orderUpdateHistoryAction = isOrderHisoryBulkUpdate
			? UpdateHistoryAction.DoNotInsert
			: updateHistoryAction;

		// 注文情報
		var isSuccess = UpdateOrderInfo(orderUpdateHistoryAction, accessor, out transactionName)
			? ResultType.Success
			: ResultType.Fail;

		// 在庫情報
		if (doStockCooperation)
		{
			isSuccess = UpdateOrderProductStock(doAlertOutOfStock, updateHistoryAction, accessor);
		}

		// クーポン情報
		if ((isSuccess == ResultType.Success)
			&& Constants.W2MP_COUPON_OPTION_ENABLED
			&& ((this.OrderOld == null)
				|| (this.OrderOld.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_UNKNOWN)))
		{
			isSuccess = UpdateOrderCouponInfo(updateHistoryAction, accessor, out transactionName)
				? ResultType.Success
				: ResultType.Fail;
		}

		// ポイント情報
		if ((isSuccess == ResultType.Success)
			&& Constants.W2MP_POINT_OPTION_ENABLED
			&& (this.OrderOld.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_UNKNOWN)
			&& this.Order.IsOwnSiteOrder
			&& this.User.IsMember)
		{
			// ユーザーポイントの更新
			isSuccess = UpdateOrderPointInfo(updateHistoryAction, accessor, cart, out transactionName)
				? ResultType.Success
				: ResultType.Fail;

			if ((isSuccess == ResultType.Success) && (this.IsMyPageModify == false))
			{
				// 受注のポイント更新
				var userPointHistoriesAfterUserPointUpdate = new PointService().GetUserPointHistories(this.User.UserId, accessor);
				var addedPoint = userPointHistoriesAfterUserPointUpdate
					.Where(history => history.OrderId == this.Order.OrderId)
					.Sum(history => history.PointInc);
				isSuccess = (new OrderService().AdjustAddPoint(this.Order.OrderId, addedPoint, this.LastChanged, updateHistoryAction, accessor) > 0)
					? ResultType.Success
					: ResultType.Fail;
			}
		}

		// ユーザメモ、ユーザ管理レベルID
		if (isSuccess == ResultType.Success)
		{
			isSuccess = UpdateUserMemoAndUserManagementLebelId(updateHistoryAction, accessor, out transactionName)
				? ResultType.Success
				: ResultType.Fail;
		}

		return isSuccess;
	}

	/// <summary>
	/// 注文更新処理
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="transactionName">トランザクション名</param>
	/// <returns>成功したか</returns>
	private bool UpdateOrderInfo(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor, out string transactionName)
	{
		transactionName = "1-2-1.注文情報UPDATE処理";
		if (UpdateOrder(updateHistoryAction, accessor) == false) return false;
			
		transactionName = "1-2-2.注文者情報UPDATE処理";
		if (UpdateOrderOwner(updateHistoryAction, accessor) == false) return false;

		transactionName = "1-2-3.注文配送先情報UPDATE処理";
		if (UpdateOrderShipping(updateHistoryAction, accessor) == false) return false;

		transactionName = "1-2-4.注文商品情報UPDATE処理";
		if (UpdateOrderItem(updateHistoryAction, accessor) == false) return false;
			
		transactionName = "1-2-4-1.定期購入継続分析情報UPDATE処理";
		if (UpdateFixedPurchaseRepeatAnalysis(accessor) == false) return false;

		transactionName = "1-2-5.注文セットプロモーション情報UPDATE処理";
		if (UpdateOrderSetPromotion(updateHistoryAction, accessor) == false) return false;

		transactionName = "1-2-6.税率毎価格情報UPDATE処理";
		return UpdateOrderPriceByTaxRate(accessor);
	}

	/// <summary>
	/// 注文クーポン情報更新処理
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="transactionName">トランザクション名</param>
	/// <returns>成功したか</returns>
	private bool UpdateOrderCouponInfo(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor, out string transactionName)
	{
		transactionName = string.Empty;
		var newCoupon = (this.Order.Coupons.Any()) ? this.Order.Coupons.FirstOrDefault() : null;
		var oldCoupon = (this.OrderOld.Coupons.Any()) ? this.OrderOld.Coupons.FirstOrDefault() : null;

		if ((newCoupon != null)
			&& (string.IsNullOrEmpty(newCoupon.CouponCode) == false)
			&& (oldCoupon == null))
		{
			// 注文クーポン登録時処理
			if (InsertUseNewOrderCouponInfo(newCoupon, updateHistoryAction, accessor, out transactionName) == false) return false;
		}
		else if ((newCoupon != null)
			&& (string.IsNullOrEmpty(newCoupon.CouponCode) == false)
			&& (oldCoupon != null))
		{
			// 注文クーポン変更時処理
			if (UpdateExchangeOrderCouponInfo(newCoupon, oldCoupon, updateHistoryAction, accessor, out transactionName) == false) return false;
		}
		else if ((newCoupon != null)
			&& string.IsNullOrEmpty(newCoupon.CouponCode)
			&& (oldCoupon != null))
		{
			// 注文クーポン削除時処理
			if (DeleteOrderCouponInfo(oldCoupon, updateHistoryAction, accessor, out transactionName) == false) return false;
		}

		return true;
	}

	/// <summary>
	/// 注文ポイント情報更新処理
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="cart">カート</param>
	/// <param name="transactionName">トランザクション名</param>
	/// <returns>成功したか</returns>
	private bool UpdateOrderPointInfo(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor, CartObject cart, out string transactionName)
	{
		var pointService = new PointService();
		this.UserPointOld = new UserPointList(pointService.GetUserPoint(this.User.UserId, null, accessor));
		var pointAddForNewOrder = this.UserPointOld.Items.Select(pt => pt.Clone()).ToList();

		// ①仮ポイントor期間限定ポイントの更新
		// ②元注文に適用されなかったルールでのポイント追加（※注文完了画面からレコメンド押すまでにキャンペーン開始を跨ぐと発生する）
		// ③通常本ポイントの追加
		// ④ユーザーポイント・履歴のDB更新

		// ①仮ポイントor期間限定ポイントの更新
		foreach (var point in pointAddForNewOrder.Where(pt => pt.OrderId == this.OrderOld.OrderId).ToArray())
		{
			if (cart.BuyPoints.Any(bp => (bp.Key == point.PointRuleId)))
			{
				var pointForUpdate = cart.BuyPoints.First(bp => (bp.Key == point.PointRuleId));
				point.Point = pointForUpdate.Value;
			}
			else if (cart.FirstBuyPoints.Any(bp => (bp.Key == point.PointRuleId)))
			{
				var pointForUpdate = cart.FirstBuyPoints.First(bp => (bp.Key == point.PointRuleId));
				point.Point = pointForUpdate.Value;
			}
		}

		// ②元注文に適用されなかったルールでのポイント追加（※注文完了画面からレコメンド押すまでにキャンペーン開始を跨ぐと発生する）
		var unAddedPointList = cart.BuyPoints
			.Where(bp => pointAddForNewOrder
				.Any(pt => (pt.OrderId == this.Order.OrderId) && (pt.PointRuleId == bp.Key)) == false)
			.ToArray();

		var maxPointKbnNo = pointService.IssuePointKbnNoForUser(this.Order.UserId, accessor);
		var unAddedBaseCompPoint = 0m;
		foreach (var unAddedCompPoint in unAddedPointList)
		{
			var pointRule = pointService.GetPointRule(Constants.CONST_DEFAULT_DEPT_ID, unAddedCompPoint.Key);
			if (pointRule.IsBasePoint && (pointRule.UseTempFlg == Constants.FLG_POINTRULE_USE_TEMP_FLG_INVALID))
			{
				// 通常本ポイントであれば合算し、③でまとめて追加する
				unAddedBaseCompPoint += unAddedCompPoint.Value;
				continue;
			}
			// 期間限定または仮ポイント
			pointAddForNewOrder.Add(
				new UserPointModel
				{
					UserId = this.Order.UserId,
					PointKbn = pointRule.PointKbn,
					PointKbnNo = maxPointKbnNo,
					DeptId = pointRule.DeptId,
					PointRuleId = pointRule.PointRuleId,
					PointRuleKbn = pointRule.PointRuleKbn,
					PointType = (pointRule.UseTempFlg == Constants.FLG_POINTRULE_USE_TEMP_FLG_VALID) 
						? Constants.FLG_USERPOINT_POINT_TYPE_TEMP
						: Constants.FLG_USERPOINT_POINT_TYPE_COMP,
					PointIncKbn = pointRule.PointIncKbn,
					Point = unAddedCompPoint.Value,
					PointExp = null,
					Kbn1 = this.Order.OrderId,
					Kbn2 = string.Empty,
					Kbn3 = string.Empty,
					Kbn4 = string.Empty,
					Kbn5 = string.Empty,
					LastChanged = this.LastChanged
				});
			maxPointKbnNo++;
		}

		// ③通常本ポイントの更新
		var userPointHistories = pointService.GetUserPointHistories(this.User.UserId, accessor);

		if (unAddedBaseCompPoint != 0)
		{
			var addedBaseCompPointAtThisOrder = userPointHistories
				.Where(ph => (ph.OrderId == this.OrderOld.OrderId)
					&& ph.IsBasePoint
					&& ph.IsPointTypeComp
					&& (ph.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_BUY))
				.Sum(pf => pf.PointInc);
			var hasCompPoint = this.UserPointOld.Items.Any(ph => ph.IsBasePoint && ph.IsPointTypeComp);
			if (hasCompPoint)
			{
				var compPoint = pointAddForNewOrder.First(ph => ph.IsBasePoint && ph.IsPointTypeComp);
				compPoint.Point =
					this.UserPointOld.Items
						.Where(pt => pt.IsBasePoint && pt.IsPointTypeComp)
						.Sum(up => up.Point)
					- addedBaseCompPointAtThisOrder
					+ unAddedBaseCompPoint;
			}
		}

		// ④DB更新
		this.UserPoint = new UserPointList(pointAddForNewOrder.ToArray());
		var addedBasePointCompOld =
			userPointHistories
				.Where(up => up.IsBasePoint && up.IsPointTypeComp)
				.Sum(up => up.PointInc);

		var isSuccess = true;
		transactionName = string.Empty;
		// ユーザポイントUPDATE処理(付与ポイント)
		foreach (var userPoint in this.UserPoint.Items)
		{
			// 変更前ユーザポイント情報取得
			var userPointOld = this.UserPointOld.GetUserPointByPointKbnNo(userPoint.PointKbnNo);
			
			// 更新対象が仮ポイント AND ユーザポイント(仮ポイント)あり
			if (userPoint.IsPointTypeTemp && (userPointOld != null))
		{
				if (userPoint.Point == userPointOld.Point) continue;

				// ポイントに変更があった場合にユーザポイント更新
			transactionName = "1-5-2-A.ユーザポイント情報(付与ポイント)UPDATE処理";
			isSuccess = UpdateUserPoint(
				userPoint,
					userPoint.Point - userPointOld.Point,
				updateHistoryAction,
				accessor);
		}
			// 更新対象が本ポイント AND 付与ポイントあり
			else if (userPoint.IsPointTypeComp && (userPointOld != null))
			{
				var adjustPoint = userPoint.Point - (userPoint.IsBasePoint ? addedBasePointCompOld : userPointOld.Point);
				if (adjustPoint == 0) continue;

				// ポイントに変更があった場合にユーザポイントを更新
				transactionName = "1-5-2-B.ユーザポイント情報(付与ポイント)UPDATE処理";
				isSuccess = UpdateUserPoint(
					userPoint,
					adjustPoint,
					updateHistoryAction,
					accessor);
			}
			// 元注文に付与ポイントなし
			else if (userPointOld == null)
			{
				// 調整分のユーザポイントを追加
			transactionName = "1-5-2-C.ユーザポイント情報(付与ポイント)INSERT処理";
				isSuccess = InsertUserPoint(
					userPoint,
					userPoint.Point,
					updateHistoryAction,
					accessor);
			}
		}
		return isSuccess;
	}

	/// <summary>
	/// ユーザメモ、ユーザ管理レベルID更新
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="transactionName">トランザクション名</param>
	/// <returns>成功したか</returns>
	private bool UpdateUserMemoAndUserManagementLebelId(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor, out string transactionName)
	{
		transactionName = "2-1.ユーザ情報UPDATE処理";
		return new UserService().UpdateUserMemoAndUserManagementLevelId(
			this.Order.UserId,
			this.UserMemo,
			this.UserManagementLevelId,
			this.Order.LastChanged,
			updateHistoryAction,
			accessor);
	}

	#region 注文情報更新処理
	/// <summary>
	/// 注文情報UPDATE処理
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool UpdateOrder(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var result = (new OrderService().UpdateForModify(this.Order, this.LastChanged, updateHistoryAction, accessor) > 0);
		return result;
	}

	/// <summary>
	/// 注文者情報UPDATE処理
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool UpdateOrderOwner(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var result = (new OrderService().UpdateOwnerForModify(this.Order.Owner, this.LastChanged, updateHistoryAction, accessor) > 0);
		return result;
	}
		
	/// <summary>
	/// 注文配送先情報UPDATE処理
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool UpdateOrderShipping(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var result = (new OrderService().UpdateShippingForModify(this.Order.Shippings, this.LastChanged, updateHistoryAction, accessor) > 0);
		return result;
	}

	/// <summary>
	/// 注文商品情報UPDATE処理
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool UpdateOrderItem(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var result = (new OrderService().UpdateItemForModify(this.Order.Items, this.LastChanged, updateHistoryAction, accessor) > 0);
		return result;
	}

	/// <summary>
	/// 定期購入継続分析情報UPDATE処理
	/// </summary>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool UpdateFixedPurchaseRepeatAnalysis(SqlAccessor accessor)
	{
		if (this.Order.IsFixedPurchaseOrder == false) return true;

		var service = new FixedPurchaseRepeatAnalysisService();
		var analysis = service.GetRepeatAnalysisByOrderId(this.Order.OrderId, accessor);

		analysis.Where(a =>
			this.Order.Items.Any(item => 
				(item.DeleteTarget
					|| (item.ProductId != a.ProductId)
					|| (item.VariationId != a.VariationId)))).ToList()
			.ForEach(a => {
				service.DeleteAnalysisOrder(
					this.Order.UserId,
					a.ProductId,
					a.VariationId,
					this.Order.OrderId,
					this.Order.LastChanged,
					accessor);
			});

		this.Order.Items.Where(item => item.DeleteTarget == false).ToList()
			.ForEach(item => {
				service.ModifyOrderItem(
					this.Order.UserId,
					item.ProductId,
					item.VariationId,
					this.Order.OrderId,
					this.Order.FixedPurchaseId,
					this.Order.LastChanged,
					accessor);
			});
		return true;
	}

	/// <summary>
	/// 注文セットプロモーション情報UPDATE処理
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool UpdateOrderSetPromotion(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		if (this.Order.SetPromotions.Any() == false) return true;
		var result = (new OrderService().UpdateSetPromotionForModify(
			this.Order.SetPromotions,
			this.LastChanged,
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
		var orderPriceByTaxRates = this.Order.OrderPriceByTaxRates;
		if (orderPriceByTaxRates.Length == 0) return true;
		var resultCount = new OrderService().UpdateOrderPriceInfoByTaxRateModify(
			orderPriceByTaxRates,
			accessor);

		return (resultCount > 0);
	}

	/// <summary>
	/// 商品在庫情報UPDATE処理
	/// </summary>
	/// <param name="doAlertOutOfStock">在庫切れ時に警告を出すか</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>実行結果</returns>
	private ResultType UpdateOrderProductStock(bool doAlertOutOfStock, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var stockUpdateListOld = CreateProductStockAdjustList(null, this.OrderOld.Items, true, accessor);
		var stockUpdateList = CreateProductStockAdjustList(stockUpdateListOld, this.Order.Items, false, accessor);

		// 在庫更新
		var stockService = new ProductStockService();
		foreach (var stockUpdate in stockUpdateList.Where(s => s.AdjustmentQuantity != 0))
		{
			if (stockService.Modify(
				stockUpdate.ShopId,
				stockUpdate.ProductId,
				stockUpdate.VariationId,
				(model) => {
					model.Stock = model.Stock - stockUpdate.AdjustmentQuantity;
					model.LastChanged = this.LastChanged;
				},
				accessor) <= 0) return ResultType.Fail;
			var updated = stockService.Get(stockUpdate.ShopId, stockUpdate.ProductId, stockUpdate.VariationId, accessor);
			if ((updated != null)
				&& ((stockUpdate.StockManagementKbn == Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPNG_BUYNG
					|| stockUpdate.StockManagementKbn == Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYNG)
					&& updated.Stock < 0)) return ResultType.OutOfStock;
		}

		// 商品在庫履歴更新
		var stockHistoryService = new ProductStockHistoryService();
		foreach (var stockUpdate in stockUpdateList.Where(s => s.AdjustmentQuantity != 0))
		{
			if (stockHistoryService.Insert(
				new ProductStockHistoryModel
				{
					OrderId = this.Order.OrderId,
					ShopId = stockUpdate.ShopId,
					ProductId = stockUpdate.ProductId,
					VariationId = stockUpdate.VariationId,
					ActionStatus = Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER_MODYIFY,
					AddStock = (stockUpdate.AdjustmentQuantity * -1),
					AddRealstock = 0,
					AddRealstockB = 0,
					AddRealstockC = 0,
					AddRealstockReserved = 0,
					UpdateMemo = string.Empty,
					LastChanged = this.LastChanged
				},
				accessor) <= 0) return ResultType.Fail;
		}

		// 更新履歴登録
		if ((updateHistoryAction == UpdateHistoryAction.Insert) && (stockUpdateList.Any()))
		{
			new UpdateHistoryService().InsertForOrder(
				this.Order.OrderId,
				this.LastChanged,
				accessor);
		}

		return ResultType.Success;
	}

	/// <summary>
	/// 在庫管理対象 商品別数量増減リスト作成
	/// </summary>
	/// <param name="baseProductStockAdjustList">ベースとする在庫管理対象 商品別数量増減リスト</param>
	/// <param name="orderItems">変更前もしくは変更後の商品情報</param>
	/// <param name="isOld">変更前フラグ true：変更前、false:変更後</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>引数の商品情報を追加した在庫管理対象 商品別数量増減リスト</returns>
	private List<ProductStockAdjustCheck> CreateProductStockAdjustList(
		List<ProductStockAdjustCheck> baseProductStockAdjustList,
		OrderItemModel[] orderItems,
		bool isOld,
		SqlAccessor accessor)
	{
		var baseAdjustList = baseProductStockAdjustList ?? new List<ProductStockAdjustCheck>();

		foreach (var orderItem in  orderItems.Where(item => 
			((item.IsReturnItem == false)
					&& ((Constants.REALSTOCK_OPTION_ENABLED && item.IsRealStockReserved) == false)
					&& (item.DeleteTarget == false)
					&& (IsStockUnmanagedProduct(item, accessor) == false))))
		{
			var adjustmentQuantity = orderItem.ItemQuantity * (isOld ? -1 : 1);
			// 入力値の表記ゆれを考慮し全て大文字に変換して比較する
			var adjustItem = baseAdjustList.FirstOrDefault(a =>
				((a.ShopId == orderItem.ShopId)
					&& (a.ProductId.ToUpper() == orderItem.ProductId.ToUpper())
					&& (a.VariationId.ToUpper() == orderItem.VariationId.ToUpper())));
			if (adjustItem != null)
			{
				adjustItem.AdjustmentQuantity += adjustmentQuantity;
			}
			else
			{
				baseAdjustList.Add(new ProductStockAdjustCheck
				{
					ShopId = orderItem.ShopId,
					ProductId = orderItem.ProductId,
					VariationId = orderItem.VariationId,
					ProductName = orderItem.ProductName,
					AdjustmentQuantity = adjustmentQuantity,
					StockManagementKbn = orderItem.StockManagementKbn,
				});
			}
		}
			
		return baseAdjustList;
	}

	/// <summary>
	/// 在庫管理しない注文商品か
	/// </summary>
	/// <param name="orderItem">注文商品情報</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>在庫管理しない？</returns>
	private bool IsStockUnmanagedProduct(OrderItemModel orderItem, SqlAccessor accessor)
	{
		var product = new ProductService().GetProductVariation(orderItem.ShopId, orderItem.ProductId, orderItem.VariationId, string.Empty, accessor);
		var result = ((product == null) || product.IsStockUnmanaged);
		return result;
	}
	#endregion

	#region 注文情報キャンセル処理
	/// <summary>
	/// 外部連携キャンセル処理
	/// </summary>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="errorMessage">エラーメッセージ</param>
	/// <param name="disableCancelApi">キャンセルAPIを利用するかどうか</param>
	/// <returns>成功したか</returns>
	private bool ExecuteCancelExternalPayment(SqlAccessor accessor, out string errorMessage, bool disableCancelApi)
	{
		errorMessage = string.Empty;

		// 自社サイト注文のみ処理
		if (MallOptionUtility.IsOwnSite(this.Order.ShopId, this.Order.MallId) == false) return true;
		// 外部決済のみ処理
		if (OrderCommon.IsExternalPayment(this.Order.OrderPaymentKbn) == false) return true;
		// すでにキャンセル済み
		if (disableCancelApi) return true;

		errorMessage = OrderCommon.CancelExternalCooperationPayment(this.Order, accessor);

		if (string.IsNullOrEmpty(errorMessage) == false) return false;
			
		var service = new OrderService();
		// オンライン決済ステータス更新
		service.UpdateOnlinePaymentStatus(
			this.Order.OrderId,
			Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED,
			this.LastChanged,
			UpdateHistoryAction.DoNotInsert,
			accessor);

		// 外部決済ステータス更新
		service.UpdateExternalPaymentInfo(
			this.Order.OrderId,
			Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL,
			true,
			DateTime.Now,
			string.Empty,
			this.LastChanged,
			UpdateHistoryAction.DoNotInsert,
			accessor);

		// 決済連携メモ更新
		if (this.Order.IsExchangeOrder == false)
		{
			service.AddPaymentMemo(
				this.Order.OrderId,
				OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
					string.IsNullOrEmpty(this.Order.PaymentOrderId)
						? this.Order.OrderId
						: this.Order.PaymentOrderId,
					this.Order.OrderPaymentKbn,
					this.Order.CardTranId,
					"キャンセル",
					this.Order.LastBilledAmount),
				this.LastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);
		}

		return true;
	}
	#endregion

	#region クーポン情報更新処理
	/// <summary>
	/// 注文クーポン登録
	/// </summary>
	/// <param name="newCoupon">更新注文クーポン情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="transactionName">トランザクション名</param>
	/// <returns>成功したか</returns>
	private bool InsertUseNewOrderCouponInfo(
		OrderCouponModel newCoupon,
		UpdateHistoryAction updateHistoryAction,
		SqlAccessor accessor,
		out string transactionName)
	{
		transactionName = "1-4-1-A-1.注文クーポン情報INSERT処理";
		if (InsertOrderCoupon(updateHistoryAction, accessor) == false) return false;

		if (CouponOptionUtility.IsCouponLimit(newCoupon.CouponType))
		{
			transactionName = "1-4-1-A-2-A.ユーザクーポン情報(利用クーポン)UPDATE処理";
			if (UpdateUsedUserCoupon(updateHistoryAction, accessor) == false) return false;
		}
		else if (CouponOptionUtility.IsCouponAllLimit(newCoupon.CouponType))
		{
			transactionName = "1-4-1-A-2-B.クーポン情報(クーポン利用回数)UPDATE処理";
			if (UpdateCouponCountDown(accessor) == false) return false;
		}
		transactionName = "1-4-1-A-3.ユーザクーポン履歴情報(利用クーポン)INSERT処理";
		if (InsertUserCouponApplyHistory(newCoupon, 1, this.Order.OrderCouponUse, accessor) == false) return false;

		if (CouponOptionUtility.IsBlacklistCoupon(newCoupon.CouponType))
		{
			transactionName = "1-4-1-A-4.ブラックリスト型クーポン利用ユーザー情報INSERT処理";
			if (InsertCouponUseUser(accessor) == false) return false;
		}

		return true;
	}

	/// <summary>
	/// 注文クーポン変更
	/// </summary>
	/// <param name="newCoupon">更新注文クーポン情報</param>
	/// <param name="oldCoupon">元注文クーポン情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="transactionName">トランザクション名</param>
	/// <returns>成功したか</returns>
	private bool UpdateExchangeOrderCouponInfo(
		OrderCouponModel newCoupon,
		OrderCouponModel oldCoupon,
		UpdateHistoryAction updateHistoryAction,
		SqlAccessor accessor,
		out string transactionName)
	{
		transactionName = string.Empty;

		// 注文クーポン変更時処理
		if ((newCoupon.CouponCode != oldCoupon.CouponCode)
			|| (newCoupon.CouponDispName != oldCoupon.CouponDispName)
			|| (newCoupon.CouponName != oldCoupon.CouponName))
		{
			transactionName = "1-4-1-B-1.注文クーポン情報UPDATE処理";
			if (UpdateOrderCoupon(updateHistoryAction, accessor) == false) return false;
		}

		if (newCoupon.CouponCode != oldCoupon.CouponCode)
		{
			var isSuccess = true;
			if (CouponOptionUtility.IsCouponLimit(oldCoupon.CouponType))
			{
				transactionName = "1-4-1-B-2-A.ユーザクーポン情報(利用クーポン)UPDATE処理";
				isSuccess = UpdateUnUseUserCoupon(updateHistoryAction, accessor);
			}
			else if (CouponOptionUtility.IsCouponAllLimit(oldCoupon.CouponType))
			{
				transactionName = "1-4-1-B-2-B.クーポン情報(クーポン利用回数)UPDATE処理";
				isSuccess = UpdateCouponCountUp(accessor);
			}
			else if (CouponOptionUtility.IsBlacklistCoupon(oldCoupon.CouponType))
			{
				transactionName = "1-4-1-B-2-C.クーポン情報(ブラックリスト型クーポン利用ユーザー)DELETE処理";
				isSuccess = DeleteCouponUseUser(accessor);
			}

			if (CouponOptionUtility.IsCouponLimit(newCoupon.CouponType))
			{
				transactionName = "1-4-1-B-3-A.ユーザクーポン情報(利用クーポン)UPDATE処理";
				isSuccess = UpdateUsedUserCoupon(updateHistoryAction, accessor);
			}
			else if (CouponOptionUtility.IsCouponAllLimit(newCoupon.CouponType))
			{
				transactionName = "1-4-1-B-3-B.クーポン情報(クーポン利用回数)UPDATE処理";
				isSuccess = UpdateCouponCountDown(accessor);
			}
			else if (CouponOptionUtility.IsBlacklistCoupon(newCoupon.CouponType))
			{
				transactionName = "1-4-1-B-3-C.クーポン情報(ブラックリスト型クーポン利用ユーザー)INSERT処理";
				isSuccess = InsertCouponUseUser(accessor);
			}
			if (isSuccess == false) return false;
		}

		// クーポン履歴
		if ((newCoupon.CouponCode != oldCoupon.CouponCode)
			|| (this.Order.OrderCouponUse != this.OrderOld.OrderCouponUse))
		{
			// 前回履歴の消し込み(マイナス履歴)
			transactionName = "1-4-B-4.ユーザクーポン履歴情報(利用クーポン)INSERT処理";
			if (InsertUserCouponCancelHistory(oldCoupon, 1, this.OrderOld.OrderCouponUse, accessor) == false) return false;

			// 新規適用クーポンの履歴登録
			transactionName = "1-4-B-5.ユーザクーポン履歴情報(利用クーポン)INSERT処理";
			if (InsertUserCouponApplyHistory(newCoupon, 1, this.Order.OrderCouponUse, accessor) == false) return false;
		}

		return true;
	}

	/// <summary>
	/// 注文クーポン削除
	/// </summary>
	/// <param name="oldCoupon">元注文クーポン情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="transactionName">トランザクション名</param>
	/// <returns>成功したか</returns>
	private bool DeleteOrderCouponInfo(OrderCouponModel oldCoupon, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor, out string transactionName)
	{
		transactionName = "1-4-1-C-1.注文クーポン情報DELETE処理";
		if (DeleteOrderCoupon(updateHistoryAction, accessor) == false) return false;

		// クーポン情報戻し
		if (CouponOptionUtility.IsCouponLimit(oldCoupon.CouponType))
		{
			transactionName = "1-4-2-C-2-A.ユーザクーポン情報(利用クーポン)UPDATE処理";
			if (UpdateUnUseUserCoupon(updateHistoryAction, accessor) == false) return false;
		}
		else if (CouponOptionUtility.IsCouponAllLimit(oldCoupon.CouponType))
		{
			transactionName = "1-4-2-C-2-B.クーポン情報(クーポン利用回数)UPDATE処理";
			if (UpdateCouponCountUp(accessor) == false) return false;
		}
		else if (CouponOptionUtility.IsBlacklistCoupon(oldCoupon.CouponType))
		{
			transactionName = "1-4-2-C-2-C.クーポン情報(ブラックリスト型クーポン利用ユーザー)INSERT処理";
			if (DeleteCouponUseUser(accessor) == false) return false;
		}

		// ユーザクーポン履歴追加
		transactionName = "1-4-C-3.ユーザクーポン履歴情報(利用クーポン)INSERT処理";
		return InsertUserCouponCancelHistory(oldCoupon, 1, this.OrderOld.OrderCouponUse, accessor);
	}

	/// <summary>
	/// 注文クーポン登録
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool InsertOrderCoupon(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var result = (new OrderService().InsertCoupon(this.Order.Coupons.First(), updateHistoryAction, accessor) > 0);
		return result;
	}

	/// <summary>
	/// ユーザークーポンの状態を「利用済み」に更新
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool UpdateUsedUserCoupon(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var result = new CouponService().UpdateUserCouponUseFlg(
			this.Order.UserId,
			this.Order.Coupons.First().DeptId,
			this.Order.Coupons.First().CouponId,
			this.Order.Coupons.First().CouponNo,
			true,
			DateTime.Now,
			this.LastChanged,
			updateHistoryAction,
			accessor);
		return result;
	}

	/// <summary>
	/// 回数制限付きクーポン数減算
	/// </summary>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool UpdateCouponCountDown(SqlAccessor accessor)
	{
		var result = new CouponService().UpdateCouponCountDown(
			this.Order.Coupons.First().DeptId,
			this.Order.Coupons.First().CouponId,
			this.Order.Coupons.First().CouponCode,
			this.LastChanged,
			accessor);
		return result;
	}

	/// <summary>
	/// ユーザークーポン利用履歴情報登録
	/// </summary>
	/// <param name="orderCoupon">注文クーポン情報</param>
	/// <param name="useCount">クーポン利用数</param>
	/// <param name="couponPrice">クーポン利用金額</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool InsertUserCouponApplyHistory(OrderCouponModel orderCoupon, int useCount, decimal couponPrice, SqlAccessor accessor)
	{
		return InsertUserCouponHistory(
			orderCoupon,
			Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_USE,
			Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_ORDER,
			(useCount * -1),
			couponPrice,
			accessor);
	}

	/// <summary>
	/// ユーザークーポンキャンセル履歴情報登録
	/// </summary>
	/// <param name="orderCoupon">注文クーポン情報</param>
	/// <param name="cancelCount">クーポンキャンセル数</param>
	/// <param name="couponPrice">クーポンキャンセル金額</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool InsertUserCouponCancelHistory(OrderCouponModel orderCoupon, int cancelCount, decimal couponPrice, SqlAccessor accessor)
	{
		return InsertUserCouponHistory(
			orderCoupon,
			Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_USE_CANCEL,
			Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_ORDER,
			cancelCount,
			(couponPrice * -1),
			accessor);
	}

	/// <summary>
	/// ユーザークーポン履歴情報登録
	/// </summary>
	/// <param name="orderCoupon">注文クーポン情報</param>
	/// <param name="historyKbn">履歴区分</param>
	/// <param name="actionKbn">アクション区分</param>
	/// <param name="addCount">加算数</param>
	/// <param name="couponPrice">クーポン金額</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool InsertUserCouponHistory(OrderCouponModel orderCoupon, string historyKbn, string actionKbn, int addCount, decimal couponPrice, SqlAccessor accessor)
	{
		var result = new CouponService().InsertUserCouponHistory(
			this.Order.UserId,
			orderCoupon.DeptId,
			orderCoupon.CouponId,
			orderCoupon.CouponCode,
			this.Order.OrderId,
			historyKbn,
			actionKbn,
			addCount,
			couponPrice,
			this.LastChanged,
			accessor);
		return result;
	}

	/// <summary>
	/// ブラックリスト型クーポン利用ユーザー登録
	/// </summary>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool InsertCouponUseUser(SqlAccessor accessor)
	{
		var couponUseUser = new CouponUseUserModel
		{
			CouponId = this.Order.Coupons.First().CouponId,
			OrderId = this.Order.OrderId,
			CouponUseUser = (Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE == Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS)
				? this.Order.Owner.OwnerMailAddr
				: this.Order.UserId,
			DateCreated = DateTime.Now,
			LastChanged = this.LastChanged
		};
		var result = new CouponService().InsertCouponUseUser(
			couponUseUser,
			accessor);
		return (result > 0);
	}

	/// <summary>
	/// 注文クーポン情報更新
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool UpdateOrderCoupon(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var result = (new OrderService().UpdateCoupon(this.Order.Coupons.First(), updateHistoryAction, accessor) > 0);
		return result;
	}

	/// <summary>
	/// 元注文ユーザークーポン未使用可
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool UpdateUnUseUserCoupon(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var userCoupon = new UserCouponModel
		{
			UserId = this.OrderOld.UserId,
			DeptId = this.OrderOld.Coupons.First().DeptId,
			CouponId = this.OrderOld.Coupons.First().CouponId,
			CouponNo = this.OrderOld.Coupons.First().CouponNo,
			LastChanged = this.LastChanged
		};

		var result = new CouponService().UpdateUnUseUserCoupon(userCoupon, updateHistoryAction, accessor);
		return (result > 0);
	}

	/// <summary>
	/// 元注文利用クーポンの利用回数を加算
	/// </summary>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool UpdateCouponCountUp(SqlAccessor accessor)
	{
		var result = new CouponService().UpdateCouponCountUp(
			this.OrderOld.Coupons.First().DeptId,
			this.OrderOld.Coupons.First().CouponId,
			this.OrderOld.Coupons.First().CouponCode,
			this.LastChanged,
			accessor);
		return result;
	}

	/// <summary>
	/// 元注文利用ブラックリスト型クーポン利用ユーザー情報削除
	/// </summary>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool DeleteCouponUseUser(SqlAccessor accessor)
	{
		var result = new CouponService().DeleteCouponUseUser(
			this.OrderOld.Coupons.First().CouponId,
			(Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE == Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS)
				? this.OrderOld.Owner.OwnerMailAddr
				: this.OrderOld.UserId,
			accessor);
		return (result > 0);
	}

	/// <summary>
	/// 注文クーポン情報削除
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool DeleteOrderCoupon(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
	{
		var oldCoupon = this.OrderOld.Coupons.First();
		var result = (new OrderService().DeleteCouponByCouponNo(
			oldCoupon.OrderId,
			oldCoupon.OrderCouponNo,
			this.LastChanged,
			updateHistoryAction,
			accessor) > 0);
		return result;
	}
	#endregion

	#region ポイント情報更新処理
	/// <summary>
	/// ユーザポイント情報登録
	/// </summary>
	/// <param name="userPoint">ユーザポイント情報</param>
	/// <param name="adjustPoint">調整ポイント</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	protected bool InsertUserPoint(
		UserPointModel userPoint,
		decimal adjustPoint,
		UpdateHistoryAction updateHistoryAction,
		SqlAccessor accessor)
	{
		var service = new PointService();
		// ポイントマスタ
		var master = service.GetPointMaster().FirstOrDefault(x => x.DeptId == Constants.W2MP_DEPT_ID && x.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE);
		var point = new UserPointModel
		{
			UserId = this.Order.UserId,
			PointKbn = userPoint.PointKbn,
			PointKbnNo = userPoint.PointKbnNo,
			DeptId = userPoint.DeptId,
			PointRuleId = StringUtility.ToEmpty(userPoint.PointRuleId),
			PointRuleKbn = StringUtility.ToEmpty(userPoint.PointRuleKbn),
			PointType = userPoint.PointType,
			PointIncKbn = (userPoint.IsPointTypeTemp)
				? Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_BUY
				: string.Empty,
			Point = adjustPoint,
			PointExp = master.IsValidPointExpKbn
				? (DateTime?)new DateTime(
					this.Order.OrderDate.Value.Year,
					this.Order.OrderDate.Value.Month,
					this.Order.OrderDate.Value.Day,
					23,
					59,
					59,
					997).AddYears(1)
				: null,
			Kbn1 = (userPoint.IsPointTypeTemp)
				? this.Order.OrderId
				: string.Empty,
			Kbn2 = string.Empty,
			Kbn3 = string.Empty,
			Kbn4 = string.Empty,
			Kbn5 = string.Empty,
			LastChanged = this.LastChanged
		};

		var updated = service.RegisterUserPoint(point, updateHistoryAction, accessor);
		if (updated == false) return false;

		// 履歴登録
		var userPoints = service.GetUserPoint(point.UserId, null, accessor);
		var userPointHistoryModel = new UserPointHistoryModel
		{
			UserId = point.UserId,
			DeptId = point.DeptId,
			PointRuleId = point.PointRuleId,
			PointRuleKbn = point.PointRuleKbn,
			PointKbn = point.PointKbn,
			PointType = point.PointType,
			PointIncKbn = point.PointIncKbn,
			PointInc = point.Point,
			PointExpExtend = UserPointHistoryModel.DEFAULT_POINT_EXP_EXTEND_STRING,
			UserPointExp = userPoints.Max(p => p.PointExp),
			Kbn1 = this.Order.OrderId,
			Kbn2 = string.Empty,
			Kbn3 = string.Empty,
			Kbn4 = string.Empty,
			Kbn5 = string.Empty,
			Memo = string.Empty,
			LastChanged = this.LastChanged
		};

		var result = (service.RegisterHistory(userPointHistoryModel, accessor) > 0);
		return result;
	}
	/// <summary>
	/// ユーザポイント情報更新
	/// </summary>
	/// <param name="userPoint">ユーザポイント情報</param>
	/// <param name="point">ポイント変更数</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>成功したか</returns>
	private bool UpdateUserPoint(UserPointModel userPoint, decimal point, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
		var service = new PointService();
		var userPointList = service.GetUserPoint(this.Order.UserId, null, accessor);
		var userUpdatePoint = userPointList.FirstOrDefault(up => up.PointKbnNo == userPoint.PointKbnNo);
		userUpdatePoint.Point += point;
		userUpdatePoint.LastChanged = this.LastChanged;
		if (service.UpdateUserPoint(userUpdatePoint, updateHistoryAction, accessor) == 0) return false;

		// 履歴登録
		var userBasePoint = userPointList.Where(up => up.IsBasePoint);
		var pointHistory = new UserPointHistoryModel
		{
			UserId = userUpdatePoint.UserId,
			DeptId = userUpdatePoint.DeptId,
			PointRuleId = userUpdatePoint.PointRuleId,
			PointRuleKbn = userUpdatePoint.PointRuleKbn,
			PointKbn = userUpdatePoint.PointKbn,
			PointType = userUpdatePoint.PointType,
			PointIncKbn = userUpdatePoint.PointIncKbn,
			PointExpExtend = UserPointHistoryModel.DEFAULT_POINT_EXP_EXTEND_STRING,
			UserPointExp = userBasePoint.Max(ubp => ubp.PointExp),
			Kbn1 = this.Order.OrderId,
			Kbn2 = string.Empty,
			Kbn3 = string.Empty,
			Kbn4 = string.Empty,
			Kbn5 = string.Empty,
			Memo = string.Empty,
			LastChanged = this.LastChanged,
			PointInc = point,
		};

		var result = service.RegisterHistory(pointHistory, accessor);
		return (result > 0);
	}
	#endregion

	#region 外部決済連携情報更新処理
	/// <summary>
	/// 外部決済連携情報セット
	/// </summary>
	/// <param name="reauth">再与信実行インスタンス</param>
	/// <param name="reauthResult">再与信結果</param>
	/// <param name="isReturnAllItems">全返品フラグ</param>
	/// <returns>外部決済連携実行したか</returns>
	private bool SetExternalPaymentInfoReauthSplit(
		ReauthExecuter reauth,
		ReauthResult reauthResult,
		bool isReturnAllItems = false)
	{
		// 外部決済連携なし
		if (reauth.HasAnyAction == false)
		{
			if (OrderCommon.CanPaymentReauth(this.Order.OrderPaymentKbn))
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				this.RegisterCardTranId = this.OrderOld.CardTranId;
				this.RegisterPaymentOrderId = this.OrderOld.PaymentOrderId;
			}
			else
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
			}
			return false;
		}

		// 元注文＆返品・交換注文に決済メモセット
		if (reauth.HasReauthOrReduceOrReprint)
		{
			this.RegisterPaymentMemo = reauthResult.PaymentMemoForReturnExchangeOrder;
			this.UpdatePaymentMemoForOrderOld = reauthResult.PaymentMemoForOrderOld;
		}

		// 再与信・減額・請求書再発行を持つ場合に外部決済情報更新
		if (reauth.HasReauthOrReduceOrReprint)
		{
			this.RegisterCardTranId = reauthResult.CardTranId;
			this.RegisterPaymentOrderId = reauthResult.PaymentOrderId;
			this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
			this.RegisterExternalPaymentAuthDate = reauth.GetUpdateReauthDate(
				this.OrderOld.ExternalPaymentAuthDate,
				this.OrderOld.OrderPaymentKbn,
				this.Order.OrderPaymentKbn);
		}

		// 失敗？
		if (reauthResult.ResultDetail != ReauthResult.ResultDetailTypes.Success)
		{
			this.ReturnOrderReauthErrorMessages =
				reauthResult.ErrorMessages + Environment.NewLine + WebMessages.GetMessages(WebMessages.ERRMSG_EXTERNAL_PAYMENT_CANCEL_FAILED);
		}

		// 返品・交換注文がクレジットカード？
		if ((this.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN))
		{
			// 売上確定失敗？
			if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)
			{
				this.RegisterExternalPaymentStatus = (this.Order.IsReturnOrder && (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.YamatoKwc))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_ERROR
					: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
			}
			// 元注文がヤマト後払いかつ入金済み？
			else if ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (this.OrderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文が後付款(TriLink後払い)？
			else if (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文がNP後払い？
			else if (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			else
			{
				this.RegisterExternalPaymentStatus = (isReturnAllItems && (this.Order.LastBilledAmount == 0))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
					: reauth.HasSales
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
						: this.RegisterExternalPaymentStatus;
			}
		}
		// コンビニ後払い？
		else if ((this.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
			&& Constants.REAUTH_COMPLETE_CVSDEF_LIST.Contains(Constants.PAYMENT_CVS_DEF_KBN))
		{
			// 元注文が後払いかつ入金済み？
			if ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (this.OrderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文が後付款(TriLink後払い)？
			else if (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文がNP後払い？
			else if (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			else
			{
				this.RegisterExternalPaymentStatus = (isReturnAllItems && (this.Order.LastBilledAmount == 0))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
					: this.RegisterExternalPaymentStatus;
			}
		}
		// PayPal決済？
		else if (this.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
		{
			this.RegisterExternalPaymentStatus = (isReturnAllItems && (this.Order.LastBilledAmount == 0))
				? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
				: this.RegisterExternalPaymentStatus;
		}
		// 後付款(TriLink後払い)？
		else if (this.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
		{
			// 元注文がヤマト後払いかつ入金済み？
			if ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (this.OrderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文が後付款(TriLink後払い)？
			else if (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文がNP後払い？
			else if (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			else
			{
				this.RegisterExternalPaymentStatus = (isReturnAllItems && (this.Order.LastBilledAmount == 0))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
					: this.RegisterExternalPaymentStatus;
			}
		}
		// NP After Pay
		else if (this.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
		{
			// 元注文がヤマト後払いかつ入金済み？
			if ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (this.OrderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文が後付款(TriLink後払い)？
			else if (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			// 元注文がNP後払い？
			else if (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
			}
			else
			{
				this.RegisterExternalPaymentStatus = (isReturnAllItems && (this.Order.LastBilledAmount == 0))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
					: this.RegisterExternalPaymentStatus;
			}
		}
		// 代引または決済無し
		else
		{
			// 連携無し
			this.UpdateExternalPaymentStatusForOrderOld = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
		}

		// 元注文情報更新
		// 元注文がクレジットカード？
		if ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& reauth.HasCancel)
		{
			this.UpdateExternalPaymentStatusForOrderOld = 
				((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
					|| ((reauth.HasSales == false) && (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)))
				? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
				: (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
					: this.OrderOld.ExternalPaymentStatus;
		}
		// 元注文がコンビニ後払い？
		else if ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
			&& Constants.REAUTH_COMPLETE_CVSDEF_LIST.Contains(Constants.PAYMENT_CVS_DEF_KBN))
		{
			if (this.OrderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP)
			{
				this.UpdateExternalPaymentStatusForOrderOld = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP;
			}
			else if (reauth.HasCancel)
			{
				this.UpdateExternalPaymentStatusForOrderOld =
					((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
						|| ((reauth.HasSales == false) && (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
					: (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
						: this.OrderOld.ExternalPaymentStatus;
			}
		}
		// 元注文が後付款(TriLink後払い)？
		else if ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			&& reauth.HasCancel)
		{
			this.UpdateExternalPaymentStatusForOrderOld =
				((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
					|| ((reauth.HasSales == false) && (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
					: (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
						: this.OrderOld.ExternalPaymentStatus;
		}
		// 元注文がNP後払い？
		else if ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			&& reauth.HasCancel)
		{
			this.UpdateExternalPaymentStatusForOrderOld =
				((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
					|| ((reauth.HasSales == false) && (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
					: (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
						: this.OrderOld.ExternalPaymentStatus;
		}
		// その他
		else
		{
			// 連携無し
			this.UpdateExternalPaymentStatusForOrderOld = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
		}
		return true;
	}
	#endregion

	#region 元注文情報更新処理
	/// <summary>
	/// 元注文情報更新処理
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="transactionName">トランザクション名</param>
	/// <returns>成功したか</returns>
	private void ExecuteUpdateOrderOld(
		UpdateHistoryAction updateHistoryAction,
		SqlAccessor accessor,
		out string transactionName)
	{
		transactionName = "元注文情報（元注文 or 最後の返品・交換注文）UPDATE処理";

		// 決済メモ
		var service = new OrderService();
		if (string.IsNullOrEmpty(this.UpdatePaymentMemoForOrderOld) == false)
		{
			service.AddPaymentMemo(
				this.OrderOld.OrderId,
				this.UpdatePaymentMemoForOrderOld,
				this.LastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);
		}

		// 外部決済ステータス
		if (string.IsNullOrEmpty(this.UpdateExternalPaymentStatusForOrderOld) == false)
		{
			service.UpdateExternalPaymentInfo(
				this.OrderOld.OrderId,
				this.UpdateExternalPaymentStatusForOrderOld,
				true,
				null,
				string.Empty,
				this.LastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);
		}

		// 最終与信フラグ
		service.UpdateLastAuthFlg(
			this.OrderOld.OrderId,
			Constants.FLG_ORDER_LAST_AUTH_FLG_OFF,
			this.LastChanged,
			updateHistoryAction,
			accessor);
	}
	#endregion

	#region メール送信処理
	/// <summary>
	/// 更新通知メール送信
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="isUser">会員？</param>
	/// <param name="recommendItemNames">レコメンド商品名リスト</param>
	private void SendOrderUpdateMailToUser(CartObject cart, bool isUser, string[] recommendItemNames)
	{
		var input = new MailTemplateDataCreaterByCartAndOrder(true).GetOrderMailDatas(this.Order.DataSource, cart, isUser);
		input[Constants.FIELD_ORDER_MEMO] = ((string)input[Constants.FIELD_ORDER_MEMO]).Trim();
		input["recommend_item_name"] = (recommendItemNames != null)
			? string.Join(",", recommendItemNames)
			: string.Empty;

		using (var mailSender = new MailSendUtility(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.CONST_MAIL_ID_ORDER_UPDATE_FOR_USER,
			this.Order.UserId,
			input,
			true,
			Constants.MailSendMethod.Auto,
			userMailAddress: StringUtility.ToEmpty(cart.Owner.MailAddr)))
		{
			mailSender.AddTo(StringUtility.ToEmpty(cart.Owner.MailAddr));
			if (mailSender.SendMail() == false) throw new Exception("メール送信処理に失敗しました。", mailSender.MailSendException);
		}
	}

	/// <summary>
	/// 更新通知メール送信(管理者向け)
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="isUser">会員？</param>
	/// <param name="recommendItemNames">レコメンド商品名リスト</param>
	private void SendOrderUpdateMailToOperator(CartObject cart, bool isUser, string[] recommendItemNames)
	{
		var input = new MailTemplateDataCreaterByCartAndOrder(false).GetOrderMailDatas(this.Order.DataSource, cart, isUser);
		input["recommend_item_name"] = (recommendItemNames != null)
			? string.Join(",", recommendItemNames)
			: string.Empty;

		using (var mailSender = new MailSendUtility(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.CONST_MAIL_ID_ORDER_UPDATE_FOR_OPERATOR,
			string.Empty,
			input,
			true,
			Constants.MailSendMethod.Auto))
		{
			// Toが設定されている場合にのみメール送信
			if (string.IsNullOrEmpty(mailSender.TmpTo) == false)
			{
				if (mailSender.SendMail() == false) throw new Exception("メール送信処理に失敗しました。", mailSender.MailSendException);
			}
		}
	}

	/// <summary>
	/// 注文キャンセル通知メール送信
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="isUser">会員？</param>
	/// <param name="recommendItemNames">レコメンド商品名リスト</param>
	private void SendOrderCancelMailToUser(CartObject cart, bool isUser, string[] recommendItemNames)
	{
		var input = new MailTemplateDataCreaterByCartAndOrder(true).GetOrderMailDatas(this.Order.DataSource, cart, isUser);
		input["recommend_item_name"] = (recommendItemNames != null)
			? string.Join(",", recommendItemNames)
			: string.Empty;

		using (var mailSender = new MailSendUtility(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.CONST_MAIL_ID_ORDER_CANCEL_BY_RECOMMEND_FOR_USER,
			this.Order.UserId,
			input,
			true,
			Constants.MailSendMethod.Auto,
			userMailAddress: StringUtility.ToEmpty(cart.Owner.MailAddr)))
		{
			mailSender.AddTo(StringUtility.ToEmpty(cart.Owner.MailAddr));
			if (mailSender.SendMail() == false) throw new Exception("メール送信処理に失敗しました。", mailSender.MailSendException);
		}
	}

	/// <summary>
	/// 注文キャンセル通知メール送信(管理者向け)
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="isUser">会員？</param>
	/// <param name="recommendItemNames">レコメンド商品名リスト</param>
	private void SendOrderCancelMailToOperator(CartObject cart, bool isUser, string[] recommendItemNames)
	{
		var input = new MailTemplateDataCreaterByCartAndOrder(true).GetOrderMailDatas(this.Order.DataSource, cart, isUser);
		input["recommend_item_name"] = (recommendItemNames != null)
			? string.Join(",", recommendItemNames)
			: string.Empty;

		using (var mailSender = new MailSendUtility(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.CONST_MAIL_ID_ORDER_CANCEL_BY_RECOMMEND_FOR_OPERATOR,
			this.Order.UserId,
			input,
			true,
			Constants.MailSendMethod.Auto,
			userMailAddress: cart.Owner.MailAddr))
		{
			// Toが設定されている場合にのみメール送信
			if (string.IsNullOrEmpty(mailSender.TmpTo) == false)
			{
				if (mailSender.SendMail() == false) throw new Exception("メール送信処理に失敗しました。", mailSender.MailSendException);
			}
		}
	}
	#endregion

	#region LINE送信処理（リピートライン）
	/// <summary>
	/// 更新通知LINE送信（リピートライン）
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="cart">カート情報</param>
	/// <param name="isUser">会員？</param>
	public void SendOrderUpdateLineMessage(Hashtable order, CartObject cart, bool isUser)
	{
		var transactionName = "4-2-2.LINE連携処理";
		try
		{
			var sendLineMessageFlg = MailSendUtility.GetMailTemplateInfo(
				w2.Domain.Constants.CONST_DEFAULT_SHOP_ID,
				w2.App.Common.Constants.CONST_MAIL_ID_ORDER_COMPLETE).LineUseFlg;

			var updateOrder = new OrderRegisterFront(isUser);

			if (sendLineMessageFlg == MailTemplateModel.LINE_USE_FLG_ON) updateOrder.SendOrderCompleteToLine(order, cart);
		}
		catch (Exception ex)
		{
			AppLogger.WriteWarn(OrderCommon.CreateOrderSuccessAlertLogMessage(transactionName, order, cart), ex);
		}
	}
	#endregion

	#region プロパティ
	/// <summary>会員情報</summary>
	private UserModel User { get; set; }
	/// <summary>注文情報</summary>
	public OrderModel Order { get; set; }
	/// <summary>更新前注文情報</summary>
	private OrderModel OrderOld { get; set; }
	/// <summary>ユーザポイント情報</summary>
	private UserPointList UserPoint { get; set; }
	/// <summary>更新前ユーザポイント情報</summary>
	private UserPointList UserPointOld { get; set; }
	/// <summary>更新用ユーザメモ</summary>
	private string UserMemo { get; set; }
	/// <summary>更新用ユーザ管理レベルID</summary>
	private string UserManagementLevelId { get; set; }
	/// <summary>最終更新者</summary>
	public string LastChanged { get; set; }
	/// <summary>他の返品・交換注文を作成した後、既に返金済みの交換注文を編集するかどうか</summary>
	private bool IsNotLastExchangeOrderModified
	{
		get { return (this.Order.IsExchangeOrder && (this.OrderOld.OrderId != this.Order.OrderId)); }
	}
	/// <summary>マイページからの受注編集か</summary>
	public bool IsMyPageModify { get; set; }
	#region 外部決済連携更新情報
	/// <summary>外部決済連携成功したか</summary>
	private bool IsSuccess { get; set; }
	/// <summary>外部決済ステータス更新値</summary>
	private string RegisterExternalPaymentStatus { get; set; }
	/// <summary>外部決済与信日時更新値</summary>
	private DateTime? RegisterExternalPaymentAuthDate { get; set; }
	/// <summary>カード取引ID更新値</summary>
	private string RegisterCardTranId { get; set; }
	/// <summary>決済注文ID更新値</summary>
	private string RegisterPaymentOrderId { get; set; }
	/// <summary>決済連携メモ更新値</summary>
	private string RegisterPaymentMemo { get; set; }
	/// <summary>元注文情報の決済連携メモ更新値</summary>
	private string UpdatePaymentMemoForOrderOld { get; set; }
	/// <summary>元注文情報の外部決済ステータス更新値</summary>
	private string UpdateExternalPaymentStatusForOrderOld { get; set; }
	/// <summary>返品時再与信エラーメッセージ</summary>
	private string ReturnOrderReauthErrorMessages { get; set; }
	/// <summary>与信結果がHOLDか(現在はコンビニ後払い「DSK」のみ利用)</summary>
	public bool IsAuthResultHold { get; private set; }
	#endregion
	#endregion
}
