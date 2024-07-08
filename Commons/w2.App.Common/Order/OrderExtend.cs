/*
=========================================================================================================
  Module      : 注文エクステンドクラス(OrderExtend.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.App.Common.Order.Reauth;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 注文エクステンドクラス
	/// </summary>
	public class OrderExtend
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="registerCardTranId">値登録向け・カード取引ID（登録する場合に値を格納する）</param>
		/// <param name="registerPaymentOrderId">値登録向け・決済注文ID（登録する場合に値を格納する）</param>
		/// <param name="registerExternalPaymentStatus">値登録向け・外部決済与信日時（登録する場合に値を格納する）</param>
		/// <param name="registerExternalPaymentAuthDate">値登録向け・外部決済ステータス（登録する場合に値を格納する）</param>
		/// <param name="registerOnlinePaymentStatus">値登録向け・オンライン決済ステータス（登録する場合に値を格納する）</param>
		/// <param name="registerPaymentMemo">値登録向け・決済連携メモ（登録する場合に値を格納する）</param>
		public OrderExtend(
			string registerCardTranId,
			string registerPaymentOrderId,
			string registerExternalPaymentStatus,
			string registerExternalPaymentAuthDate,
			string registerOnlinePaymentStatus,
			string registerPaymentMemo)
		{
			this.RegisterCardTranId = registerCardTranId;
			this.RegisterPaymentOrderId = registerPaymentOrderId;
			this.RegisterExternalPaymentStatus = registerExternalPaymentStatus;
			this.RegisterExternalPaymentAuthDate = registerExternalPaymentAuthDate;
			this.RegisterOnlinePaymentStatus = registerOnlinePaymentStatus;
			this.RegisterPaymentMemo = registerPaymentMemo;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderExtend()
		{
		}

		/// <summary>
		/// Process Execute External Payment
		/// </summary>
		/// <param name="orderOld">Order old</param>
		/// <param name="orderNew">Order new</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="accessor">Sql Accessor</param>
		public void ProcessExecuteExternalPayment(OrderModel orderOld, Hashtable orderNew, string loginOperatorName, SqlAccessor accessor)
		{
			// 外部決済連携実行（仮クレジットカードの場合はスキップ）
			var isExecuteExternalPayment = ExecuteExternalPayment(orderOld, orderNew, loginOperatorName, accessor);

			if (isExecuteExternalPayment)
			{
				// 変更後の注文の最終与信フラグをONにする
				orderNew[Constants.FIELD_ORDER_LAST_AUTH_FLG] = Constants.FLG_ORDER_LAST_AUTH_FLG_ON;

				// 元注文情報更新
				ExecuteUpdateOrderOld(orderOld, UpdateHistoryAction.DoNotInsert, loginOperatorName, accessor);
			}
		}

		/// <summary>
		/// 外部決済連携実行
		/// </summary>
		/// <param name="orderOld">Order Old</param>
		/// <param name="orderNew">Order new</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>>
		/// <param name="accessor">Accessor</param>
		/// <returns>外部決済連携実行したか？</returns>
		private bool ExecuteExternalPayment(
			OrderModel orderOld,
			Hashtable orderNew,
			string loginOperatorName,
			SqlAccessor accessor)
		{
			if (Constants.PAYMENT_REAUTH_ENABLED)
			{
				this.RegisterCardTranId = null;
				this.RegisterExternalPaymentAuthDate = null;
				this.RegisterExternalPaymentStatus = null;
				this.RegisterPaymentMemo = null;
				this.UpdatePaymentMemoForOrderOld = null;
				this.UpdateExternalPaymentStatusForOrderOld = null;

				// Set Order Owner, Shippings, Items For Order
				var orderOwner = new OrderOwnerModel(orderOld.DataSource);
				var shippings = new List<OrderShippingModel>()
				{
					new OrderShippingModel(orderOld.DataSource)
				};
				var items = new List<OrderItemModel>()
				{
					new OrderItemModel(orderOld.DataSource)
				};
				orderOld.Items = items.ToArray();
				orderOld.Shippings = shippings.ToArray();
				orderOld.Owner = orderOwner;
				orderOld.IsReturnExchangeProcessAtWorkflow = true;

				var orderModelNew = new OrderModel(orderNew);
				orderModelNew.Items = items.ToArray();
				orderModelNew.Shippings = shippings.ToArray();
				orderModelNew.Owner = orderOwner;

				// Set Order Date For Order New
				orderModelNew.OrderDate = orderOld.OrderDate;

				// Don't Allow To Create Return Order When Np After Pay Has Paid
				if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& NPAfterPayUtility.IsNPAfterPayHasPaid(orderOld.CardTranId))
				{
					throw new Exception(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_NPAFTERPAY_CANNOT_LINK_PAYMENT));
				}

				// 外部決済連携実行
				var reauth = new ReauthCreatorFacade(orderOld, orderModelNew, ReauthCreatorFacade.ExecuteTypes.System, ReauthCreatorFacade.OrderActionTypes.Return, accessor)
					.CreateReauth();
				var reauthResult = reauth.Execute();

				var isReauthResultSuccess = (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success);

				// 外部決済連携実行ログ処理
				OrderCommon.AppendExternalPaymentCooperationLog(
					isReauthResultSuccess,
					orderModelNew.OrderIdOrg,
					isReauthResultSuccess ? LogCreator.CreateMessage(orderModelNew.OrderIdOrg, "") : reauthResult.ApiErrorMessages,
					loginOperatorName,
					UpdateHistoryAction.Insert,
					accessor);

				// 与信のみに失敗している場合エラー画面へ
				var isEcPayOrNewebPayAndFailureButAuthSuccess = (((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
						|| (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
					&& (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess));
				if (isEcPayOrNewebPayAndFailureButAuthSuccess
					|| (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure))
				{
					// Atodeneの場合、与信エラーにする
					if (reauth.AuthLostForError)
					{
						var service = new OrderService();
						service.UpdateExternalPaymentInfoForAuthError(
							orderOld.OrderId,
							reauthResult.ErrorMessages,
							orderOld.LastChanged,
							UpdateHistoryAction.Insert,
							accessor);
					}
					throw new Exception(reauthResult.ErrorMessages);
				}
				// 失敗したが、同額で与信を取り直している場合は元注文情報を更新し、エラー画面へ
				else if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSameAmount)
				{
					// 外部決済情報セット
					SetExternalPaymentInfoReauthSplit(reauth, reauthResult, orderModelNew, orderOld, true);

					// 元注文情報更新
					ExecuteUpdateOrderOld(orderOld, UpdateHistoryAction.DoNotInsert, loginOperatorName, accessor);

					// 決済取引ID更新
					new OrderService().UpdateCardTranId(orderOld.OrderId, reauthResult.CardTranId, loginOperatorName, UpdateHistoryAction.DoNotInsert, accessor);

					// 更新履歴登録
					new UpdateHistoryService().InsertForOrder(orderOld.OrderId, loginOperatorName, accessor);
				}

				var isNewebPayPayAndSuccess = ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
					&& (reauth.HasSales == false));
				if (isNewebPayPayAndSuccess)
				{
					// 外部決済情報セット
					SetExternalPaymentInfoReauthSplit(reauth, reauthResult, orderModelNew, orderOld, true);

					// 元注文情報更新
					this.UpdatePaymentMemoForOrderOld = string.Empty;
					ExecuteUpdateOrderOld(orderOld, UpdateHistoryAction.DoNotInsert, loginOperatorName, accessor);
				}

				// 外部決済連携情報セット
				return SetExternalPaymentInfoReauthSplit(reauth, reauthResult, orderModelNew, orderOld, true);
			}

			return false;
		}

		/// <summary>
		/// 外部決済連携情報セット（注文分割：返品／交換登録、又は、途中交換注文編集の場合等）
		/// </summary>
		/// <param name="reauth">再与信実行インスタンス</param>
		/// <param name="reauthResult">再与信結果</param>
		/// <param name="orderNew">（返品・交換・編集後の）注文情報</param>
		/// <param name="orderOld">（元注文・直前の）注文情報</param>
		/// <param name="isReturnAllItems">全返品フラグ</param>
		/// <returns>外部決済連携実行したか？</returns>
		public bool SetExternalPaymentInfoReauthSplit(
			ReauthExecuter reauth,
			ReauthResult reauthResult,
			OrderModel orderNew,
			OrderModel orderOld,
			bool isReturnAllItems = false)
		{
			// 外部決済連携をしていない？
			if (reauth.HasAnyAction == false)
			{
				// 返品・交換注文
				// クレジットカード OR ヤマト後払い：未決済
				// 上記以外：連携なし
				if (OrderCommon.CheckCanPaymentReauth(orderNew.OrderPaymentKbn))
				{
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
					this.RegisterCardTranId = orderOld.CardTranId;
					this.RegisterPaymentOrderId = orderOld.PaymentOrderId;
				}
				else
				{
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
				}
				return false;
			}

			// 元注文＆返品・交換注文に決済メモセット
			if (string.IsNullOrEmpty(reauthResult.PaymentMemo) == false)
			{
				this.RegisterPaymentMemo = reauthResult.PaymentMemoForReturnExchangeOrder;
				this.UpdatePaymentMemoForOrderOld = reauthResult.PaymentMemoForOrderOld;
			}

			// 再与信・減額・請求書再発行もつ場合に外部決済情報更新
			if (reauth.HasReauthOrReduceOrReprint)
			{
				// 返品・交換注文
				// 決済取引ID、決済注文ID、外部決済ステータス、外部決済与信日時をセット
				this.RegisterCardTranId = reauthResult.CardTranId;
				this.RegisterPaymentOrderId = reauthResult.PaymentOrderId;
				this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
				this.RegisterExternalPaymentAuthDate = reauth.GetUpdateReauthDate(
							orderOld.ExternalPaymentAuthDate,
							orderOld.OrderPaymentKbn,
							orderNew.OrderPaymentKbn).ToString();
			}

			// 失敗し、同額で与信を取り直している場合
			if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSameAmount)
			{
				this.UpdatePaymentMemoForOrderOld = reauthResult.PaymentMemo;
				return false;
			}

			// ▽ 返品注文の外部決済ステータスをセット ▽
			// 返品・交換注文がクレジットカード？
			if ((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				&& (Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN)))
			{
				// 売上確定に失敗？
				if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)
				{
					// 全返品、一部返品：売上確定エラー
					// 交換：与信済み
					this.RegisterExternalPaymentStatus = (orderNew.IsReturnOrder && Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.YamatoKwc)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_ERROR
						: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
				}
				// 元注文がヤマト後払い AND 入金済み？
				else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
				{
					// 返品注文
					// 全返品、一部返品：未決済
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				// 元注文が台湾後払い？
				else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
				{
					// 返品注文
					// 全返品、一部返品：未決済
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				// 元注文がNP後払い？
				else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
				{
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				else
				{
					// 返品注文 全返品：未決済
					this.RegisterExternalPaymentStatus = (isReturnAllItems && (orderNew.LastBilledAmount == 0)) ? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED : this.RegisterExternalPaymentStatus;
					// 返品注文 一部返品：売上確定済み
					this.RegisterExternalPaymentStatus = reauth.HasSales ? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP : this.RegisterExternalPaymentStatus;
				}
			}
			// 返品・交換注文がヤマト後払い？
			else if ((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& Constants.REAUTH_COMPLETE_CVSDEF_LIST.Contains(Constants.PAYMENT_CVS_DEF_KBN))
			{
				// 元注文がヤマト後払い AND 入金済み？
				if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
				{
					// 返品注文
					// 全返品、一部返品：未決済
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				// 元注文が台湾後払い？
				else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				{
					// 返品注文
					// 全返品、一部返品：未決済
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				else
				{
					// 返品注文
					// 全返品：未決済、一部返品：与信済み
					this.RegisterExternalPaymentStatus =
						isReturnAllItems && (orderNew.LastBilledAmount == 0)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
						: this.RegisterExternalPaymentStatus;
				}
			}
			// 返品・交換注文が台湾後払い
			else if (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				// 元注文がヤマト後払い AND 入金済み？
				if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
				{
					// 返品注文
					// 全返品、一部返品：未決済
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				// 元注文が台湾後払い？
				else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
				{
					// 返品注文
					// 全返品、一部返品：未決済
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				// 元注文がNP後払い？
				else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
				{
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				else
				{
					// 返品注文
					// 全返品：未決済、一部返品：与信済み
					this.RegisterExternalPaymentStatus =
						isReturnAllItems && (orderNew.LastBilledAmount == 0)
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
							: this.RegisterExternalPaymentStatus;
				}
			}
			// 返品・交換注文がAmazonPay？
			else if (OrderCommon.IsAmazonPayment(orderNew.OrderPaymentKbn))
			{
				// 決済取引IDをセット
				this.RegisterCardTranId = reauthResult.CardTranId;

				// 売上確定に失敗？
				if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)
				{
					// 全返品、一部返品：売上確定エラー
					// 交換：与信済み
					this.RegisterExternalPaymentStatus = (orderNew.IsReturnOrder && Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.YamatoKwc)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_ERROR
						: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
				}
				// 元注文がヤマト後払い AND 入金済み？
				else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
				{
					// 返品注文
					// 全返品、一部返品：未決済
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				// 元注文が台湾後払い？
				else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
				{
					// 返品注文
					// 全返品、一部返品：未決済
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				// 元注文がNP後払い？
				else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
				{
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				else
				{
					// 返品注文 全返品：未決済
					this.RegisterExternalPaymentStatus = (isReturnAllItems && (orderNew.LastBilledAmount == 0)) ? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED : this.RegisterExternalPaymentStatus;
					// 返品注文 一部返品：売上確定済み
					if (reauth.HasSales)
					{
						this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
						this.RegisterOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
					}
				}
			}
			// For return exchange order of paidy pay
			else if (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			{
				// 決済取引IDをセット
				this.RegisterCardTranId = reauthResult.CardTranId;

				// 売上確定に失敗？
				if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)
				{
					// 全返品、一部返品：売上確定エラー
					// 交換：与信済み
					this.RegisterExternalPaymentStatus = (orderNew.IsReturnOrder && (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.YamatoKwc))
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_ERROR
						: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
				}
				// 元注文がヤマト後払い AND 入金済み？
				else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
				{
					// 返品注文
					// 全返品、一部返品：未決済
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				// 元注文が台湾後払い？
				else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
				{
					// 返品注文
					// 全返品、一部返品：未決済
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				// 元注文がNP後払い？
				else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
				{
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				else
				{
					// 返品注文 全返品：未決済
					this.RegisterExternalPaymentStatus = (isReturnAllItems && (orderNew.LastBilledAmount == 0))
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
						: this.RegisterExternalPaymentStatus;

					// 返品注文 一部返品：売上確定済み
					if (reauth.HasSales)
					{
						this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
						this.RegisterOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
						this.UpdateOnlinePaymentStatusForOrderOld = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
					}
				}
			}
			// 返品・交換注文がLINE Pay？
			else if (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
			{
				// 決済取引IDをセット
				this.RegisterCardTranId = reauthResult.CardTranId;

				// 売上確定に失敗？
				if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)
				{
					// 全返品、一部返品：売上確定エラー
					// 交換：与信済み
					this.RegisterExternalPaymentStatus =
						(orderNew.IsReturnOrder
						&& (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.YamatoKwc))
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_ERROR
							: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
				}
				// 元注文がヤマト後払い AND 入金済み？
				else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
				{
					// 返品注文
					// 全返品、一部返品：未決済
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				// 元注文が台湾後払い？
				else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
				{
					// 返品注文
					// 全返品、一部返品：未決済
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				// 元注文がNP後払い？
				else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
				{
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				else
				{
					// 返品注文 全返品：未決済
					this.RegisterExternalPaymentStatus =
						(isReturnAllItems && (orderNew.LastBilledAmount == 0))
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
							: this.RegisterExternalPaymentStatus;
					// 返品注文 一部返品：売上確定済み
					if (reauth.HasSales
						&& orderNew.IsExchangeOrder
						&& (orderNew.LastBilledAmount < orderOld.LastBilledAmount))
					{
						this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
						this.RegisterOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
					}
				}
			}
			// NP After Pay
			else if (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				// 元注文がヤマト後払い AND 入金済み？
				if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
				{
					// 返品注文
					// 全返品、一部返品：未決済
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				// 元注文が台湾後払い？
				else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
				{
					// 返品注文
					// 全返品、一部返品：未決済
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				// 元注文がNP後払い？
				else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
				{
					this.RegisterExternalPaymentStatus =
						(NPAfterPayUtility.CheckIfExternalPaymentStatusHasBeenPaid(orderOld.ExternalPaymentStatus)
							&& string.IsNullOrEmpty(reauthResult.ApiErrorMessages))
						? orderOld.ExternalPaymentStatus
						: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				else
				{
					// 返品注文
					// 全返品：未決済、一部返品：与信済み
					this.RegisterExternalPaymentStatus =
						(isReturnAllItems && (orderNew.LastBilledAmount == 0))
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
							: this.RegisterExternalPaymentStatus;
				}
			}
			// For return exchange order of Ec pay
			else if (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
			{
				// 決済取引IDをセット
				this.RegisterCardTranId = reauthResult.CardTranId;

				// 売上確定に失敗？
				if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)
				{
					// 全返品、一部返品：売上確定エラー
					// 交換：与信済み
					this.RegisterExternalPaymentStatus = orderNew.IsReturnOrder
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_ERROR
						: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
				}
				// 元注文がヤマト後払い AND 入金済み？
				else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
				{
					// 返品注文
					// 全返品、一部返品：未決済
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				// 元注文が台湾後払い？
				else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
				{
					// 返品注文
					// 全返品、一部返品：未決済
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				else
				{
					// 返品注文 全返品：未決済
					this.RegisterExternalPaymentStatus = (isReturnAllItems && (orderNew.LastBilledAmount == 0))
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
						: this.RegisterExternalPaymentStatus;

					// 返品注文 一部返品：売上確定済み
					if (reauth.HasSales)
					{
						this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
						this.RegisterOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
						this.UpdateOnlinePaymentStatusForOrderOld = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
					}
				}
			}
			// For return exchange order of Newebpay
			else if (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
			{
				// 決済取引IDをセット
				this.RegisterCardTranId = reauthResult.CardTranId;

				// 売上確定に失敗？
				if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)
				{
					// 全返品、一部返品：売上確定エラー
					// 交換：与信済み
					this.RegisterExternalPaymentStatus = orderNew.IsReturnOrder
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_ERROR
						: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
				}
				else
				{
					// 返品注文 全返品：未決済
					this.RegisterExternalPaymentStatus = (isReturnAllItems && (orderNew.LastBilledAmount == 0))
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
						: this.RegisterExternalPaymentStatus;

					// 返品注文 一部返品：売上確定済み
					if (reauth.HasSales)
					{
						this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
						this.RegisterOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
						this.UpdateOnlinePaymentStatusForOrderOld = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
					}
					else if (reauth.HasCancel)
					{
						this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL;
						this.RegisterOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED;
						this.UpdateOnlinePaymentStatusForOrderOld = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED;
					}
				}
			}
			// For return exchange order of Paypay
			else if ((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				&& ((Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.GMO)
					|| (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.VeriTrans)))
			{
				// 決済取引IDをセット
				this.RegisterCardTranId = reauthResult.CardTranId;

				// 売上確定に失敗？
				if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)
				{
					// 全返品、一部返品：売上確定エラー
					// 交換：与信済み
					this.RegisterExternalPaymentStatus = (orderNew.IsReturnOrder)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_ERROR
						: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
				}
				// 元注文がヤマト後払い AND 入金済み？
				else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP))
				{
					// 返品注文
					// 全返品、一部返品：未決済
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				// 元注文が台湾後払い？
				else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
				{
					// 返品注文
					// 全返品、一部返品：未決済
					this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
				}
				else
				{
					// 返品注文 全返品：未決済
					this.RegisterExternalPaymentStatus = (isReturnAllItems && (orderNew.LastBilledAmount == 0))
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED
						: this.RegisterExternalPaymentStatus;

					// 返品注文 一部返品：売上確定済み
					if (reauth.HasSales)
					{
						this.RegisterExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
						this.RegisterOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
					}
				}
			}
			// 返品・交換注文が代引き OR 決済無し
			else
			{
				// 返品・交換注文：連携なし
				this.UpdateExternalPaymentStatusForOrderOld =
					Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
			}

			// ▽ 元注文の外部決済ステータスをセット ▽
			//　元注文がクレジットカード？
			if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				&& reauth.HasCancel)
			{
				// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
				this.UpdateExternalPaymentStatusForOrderOld =
					((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
						|| ((reauth.HasSales == false) && (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
					: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
						: orderOld.ExternalPaymentStatus);
			}
			// 元注文がヤマト後払い？
			else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& Constants.REAUTH_COMPLETE_CVSDEF_LIST.Contains(Constants.PAYMENT_CVS_DEF_KBN))
			{
				// 元注文がヤマト後払い AND 入金済み？
				if (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP)
				{
					// 元注文：入金済み
					this.UpdateExternalPaymentStatusForOrderOld =
						Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP;
				}
				else if (reauth.HasCancel)
				{
					// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
					this.UpdateExternalPaymentStatusForOrderOld =
					((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
						|| ((reauth.HasSales == false) && (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
					: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
						: orderOld.ExternalPaymentStatus);
				}
			}
			// 元注文が台湾後払い
			else if (OrderCommon.IsAmazonPayment(orderNew.OrderPaymentKbn) && reauth.HasCancel)
			{
				if (reauth.HasCancel)
				{
					// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
					this.UpdateExternalPaymentStatusForOrderOld =
					((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
						|| ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)))
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
					: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
						: orderOld.ExternalPaymentStatus);
				}
			}
			// 元注文がAmazonPay？
			else if (OrderCommon.IsAmazonPayment(orderNew.OrderPaymentKbn))
			{
				// キャンセルエラーになるのは全返品が失敗したときのみ　→　新注文の最終請求金額が0以下 || 失敗している
				// 返金済みになるのは減額かつSuccessのときのみ
				// 更新しないのはそれ以外
				// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
				this.UpdateExternalPaymentStatusForOrderOld =
				((orderNew.LastBilledAmount <= 0) && (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure))
				? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
				: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
					: orderOld.ExternalPaymentStatus);
			}
			// 元注文がPaidy Pay
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			{
				if (reauth.HasCancel)
				{
					// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
					this.UpdateExternalPaymentStatusForOrderOld =
						((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
							|| ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)))
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
						: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
							: orderOld.ExternalPaymentStatus);
				}
				else
				{
					// キャンセルエラーになるのは全返品が失敗したときのみ　→　新注文の最終請求金額が0以下 || 失敗している
					// 返金済みになるのは減額かつSuccessのときのみ
					// 更新しないのはそれ以外
					// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
					this.UpdateExternalPaymentStatusForOrderOld =
						((orderNew.LastBilledAmount <= 0)
							&& (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure))
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
						: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
							: orderOld.ExternalPaymentStatus);
				}

				this.UpdateOnlinePaymentStatusForOrderOld = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
			}
			// 元注文がLINE Pay？
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
			{
				if (reauth.HasCancel)
				{
					// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
					this.UpdateExternalPaymentStatusForOrderOld =
						((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
							|| ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)))
								? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
								: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
									? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
									: orderOld.ExternalPaymentStatus);
				}
				else
				{
					// キャンセルエラーになるのは全返品が失敗したときのみ　→　新注文の最終請求金額が0以下 || 失敗している
					// 返金済みになるのは減額かつSuccessのときのみ
					// 更新しないのはそれ以外
					// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
					this.UpdateExternalPaymentStatusForOrderOld =
						((orderNew.LastBilledAmount <= 0)
							&& (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure))
								? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
								: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
									? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
									: orderOld.ExternalPaymentStatus);
				}

				if (reauth.HasSales)
				{
					this.UpdateOnlinePaymentStatusForOrderOld = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
					this.UpdatePaymentMemoForOrderOld +=
						string.IsNullOrEmpty(reauthResult.PaymentMemoSales)
							? string.Empty
							: "\r\n" + reauthResult.PaymentMemoSales;
				}
			}
			// 元注文がNP後払い？
			else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
				&& reauth.HasCancel)
			{
				this.UpdateExternalPaymentStatusForOrderOld =
					((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
						|| ((reauth.HasSales == false) && (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)))
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
						: (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
							: orderOld.ExternalPaymentStatus;
			}
			// 元注文がEcPay
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
			{
				if (reauth.HasCancel)
				{
					// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
					this.UpdateExternalPaymentStatusForOrderOld =
						((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
							|| (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess))
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
						: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
							: orderOld.ExternalPaymentStatus);
				}
				else
				{
					// キャンセルエラーになるのは全返品が失敗したときのみ　→　新注文の最終請求金額が0以下 || 失敗している
					// 返金済みになるのは減額かつSuccessのときのみ
					// 更新しないのはそれ以外
					// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
					this.UpdateExternalPaymentStatusForOrderOld =
						((orderNew.LastBilledAmount <= 0)
							&& (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure))
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
						: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
							: orderOld.ExternalPaymentStatus);
				}
			}
			// 元注文がNewebPay
			else if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
			{
				if (reauth.HasCancel)
				{
					// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
					this.UpdateExternalPaymentStatusForOrderOld = ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess)
						|| (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess))
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
							: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
								? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL
								: orderOld.ExternalPaymentStatus);
				}
				else
				{
					// キャンセルエラーになるのは全返品が失敗したときのみ　→　新注文の最終請求金額が0以下 || 失敗している
					// 返金済みになるのは減額かつSuccessのときのみ
					// 更新しないのはそれ以外
					// 元注文：キャンセル成功→返金済み、キャンセル失敗→キャンセルエラー、キャンセルしない→更新しない
					this.UpdateExternalPaymentStatusForOrderOld = ((orderNew.LastBilledAmount <= 0)
						&& (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure))
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR
							: ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
								? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
								: orderOld.ExternalPaymentStatus);
				}
			}
			// その他（代引き OR 決済無し等）
			else
			{
				// 元注文：連携なし
				this.UpdateExternalPaymentStatusForOrderOld =
					Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
			}
			return true;
		}

		/// <summary>
		/// 元注文情報（元注文 or 最後の返品注文）更新
		/// </summary>
		/// <param name="orderOld">元注文</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="accessor">SQLアクセサ</param>
		protected void ExecuteUpdateOrderOld(
			OrderModel orderOld,
			UpdateHistoryAction updateHistoryAction,
			string loginOperatorName,
			SqlAccessor accessor)
		{
			// 決済メモ更新
			var orderService = new OrderService();
			if (string.IsNullOrEmpty(this.UpdatePaymentMemoForOrderOld) == false)
			{
				orderService.AddPaymentMemo(
					orderOld.OrderId,
					this.UpdatePaymentMemoForOrderOld,
					loginOperatorName,
					UpdateHistoryAction.DoNotInsert,
					accessor);
			}

			// 外部決済ステータス更新
			if (string.IsNullOrEmpty(this.UpdateExternalPaymentStatusForOrderOld) == false)
			{
				orderService.UpdateExternalPaymentInfo(
					orderOld.OrderId,
					this.UpdateExternalPaymentStatusForOrderOld,
					true,
					null,
					"",
					loginOperatorName,
					UpdateHistoryAction.DoNotInsert,
					accessor);
			}

			// For case has update online payment status for order old
			if (string.IsNullOrEmpty(this.UpdateOnlinePaymentStatusForOrderOld) == false)
			{
				orderService.UpdateOnlinePaymentStatus(
					orderOld.OrderId,
					this.UpdateOnlinePaymentStatusForOrderOld,
					loginOperatorName,
					UpdateHistoryAction.DoNotInsert,
					accessor);
			}

			// 最終与信フラグ更新（更新履歴とともに？）
			orderService.UpdateLastAuthFlg(
				orderOld.OrderId,
				Constants.FLG_ORDER_LAST_AUTH_FLG_OFF,
				loginOperatorName,
				updateHistoryAction,
				accessor);
		}

		/// <summary>
		/// Get Order Input For Return Exchange
		/// </summary>
		/// <param name="order">Order input</param>
		/// <param name="returnOrder">Return Order</param>
		/// <param name="orderOld">Order Old</param>
		/// <param name="isReturnWorkFlow">Is Return Work Flow</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>Order input</returns>
		public Hashtable GetOrderInputForReturnExchange(
			Hashtable order,
			Order returnOrder,
			OrderModel orderOld,
			bool isReturnWorkFlow,
			string loginOperatorName,
			SqlAccessor accessor)
		{
			order[Constants.FIELD_ORDER_ORDER_ID_ORG] = returnOrder.OrderId;
			order[Constants.FIELD_ORDER_SHOP_ID] = returnOrder.ShopId;
			order[Constants.FIELD_ORDER_USER_ID] = returnOrder.UserId;
			order[Constants.FIELD_ORDER_ORDER_KBN] = returnOrder.OrderKbn;
			order[Constants.FIELD_ORDER_MALL_ID] = returnOrder.MallId;
			order[Constants.FIELD_ORDER_SHIPPING_ID] = returnOrder.ShippingId;
			order[Constants.FIELD_ORDER_SHIPPING_ID] = returnOrder.ShippingId;
			order[Constants.FIELD_ORDER_LAST_CHANGED] = loginOperatorName;
			order[Constants.FIELD_ORDER_GIFT_FLG] = returnOrder.GiftFlg;
			order[Constants.FIELD_ORDER_ORDER_TAX_INCLUDED_FLG] = Constants.FLG_PRODUCT_TAX_INCLUDED_PRETAX;
			order[Constants.FIELD_ORDER_ORDER_TAX_RATE] = 0m;
			order[Constants.FIELD_ORDER_ORDER_TAX_ROUND_TYPE] = Constants.TAX_EXCLUDED_FRACTION_ROUNDING;
			order[Constants.FIELD_ORDER_MEMBER_RANK_ID] = returnOrder.MemberRankId;
			order[Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT] = returnOrder.FixedPurchaseOrderCount;
			order[Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT] = returnOrder.FixedPurchaseShippedCount;
			order[Constants.FIELD_ORDER_CARD_TRAN_ID] = this.RegisterCardTranId ?? orderOld.CardTranId;
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = this.RegisterPaymentOrderId ?? orderOld.PaymentOrderId;
			order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS] = this.RegisterExternalPaymentStatus ?? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
			order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE] = this.RegisterExternalPaymentAuthDate ?? null;
			order[Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS] = this.RegisterOnlinePaymentStatus ?? Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE;
			order[Constants.FIELD_ORDER_PAYMENT_MEMO] = this.RegisterPaymentMemo ?? string.Empty;
			order[Constants.FIELD_ORDER_SHIPPING_TAX_RATE] = returnOrder.ShippingTaxRate;
			order[Constants.FIELD_ORDER_PAYMENT_TAX_RATE] = returnOrder.PaymentTaxRate;
			order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE] = orderOld.ExternalPaymentType;

			var paymentKbn = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]);
			var isNeedUpdateInvoiceBundleFlgOff = (Constants.PAYMENT_NP_AFTERPAY_OPTION_ENABLED
				&& Constants.PAYMENT_REAUTH_ENABLED
				&& (paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
				&& NPAfterPayUtility.CheckIfExternalPaymentStatusHasBeenPaid(orderOld.ExternalPaymentStatus));
			order[Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG] = isNeedUpdateInvoiceBundleFlgOff
				? Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF
				: returnOrder.InvoiceBundleFlg;

			if (isReturnWorkFlow)
			{
				order[Constants.FIELD_ORDER_REPAYMENT_MEMO] = string.Empty;
				order[Constants.FIELD_ORDER_ORDER_ITEM_COUNT] = 0;
				order[Constants.FIELD_ORDER_ORDER_PRODUCT_COUNT] = 0;
				order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT] = 0m;
				order[Constants.FIELD_ORDER_ORDER_POINT_USE] = (decimal.Parse(returnOrder.OrderPointUse) * -1);
				order[Constants.FIELD_ORDER_ORDER_POINT_USE_YEN] = (decimal.Parse(returnOrder.OrderPointUseYen) * -1);
				order[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE] = 0;
				order[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE_YEN] = 0;

				// 決済金額
				var settlementCurrency = CurrencyManager.GetSettlementCurrency(paymentKbn);
				var settlementRate = CurrencyManager.GetSettlementRate(settlementCurrency);
				var settlementAmount = CurrencyManager.GetSettlementAmount(returnOrder.OrderId, paymentKbn, 0m, settlementRate, settlementCurrency, accessor);
				order[Constants.FIELD_ORDER_SETTLEMENT_CURRENCY] = settlementCurrency;
				order[Constants.FIELD_ORDER_SETTLEMENT_RATE] = settlementRate;
				order[Constants.FIELD_ORDER_SETTLEMENT_AMOUNT] = settlementAmount;
			}

			return order;
		}

		/// <summary>値登録向け・カード取引ID（登録する場合は値が格納される）</summary>
		protected string RegisterCardTranId { get; set; }
		/// <summary>値登録向け・決済注文ID（登録する場合は値が格納される）</summary>
		protected string RegisterPaymentOrderId { get; set; }
		/// <summary>値登録向け・外部決済与信日時（登録する場合は値が格納される）</summary>
		protected string RegisterExternalPaymentAuthDate { get; set; }
		/// <summary>値登録向け・外部決済ステータス（登録する場合は値が格納される）</summary>
		protected string RegisterExternalPaymentStatus { get; set; }
		/// <summary>値登録向け・オンライン決済ステータス（登録する場合は値が格納される）</summary>
		protected string RegisterOnlinePaymentStatus { get; set; }
		/// <summary>値登録向け・決済連携メモ（登録する場合は値が格納される）</summary>
		protected string RegisterPaymentMemo { get; set; }
		/// <summary>元注文情報値更新向け・外部決済ステータス（更新する場合は値が格納される）</summary>
		protected string UpdateExternalPaymentStatusForOrderOld { get; set; }
		/// <summary>元注文情報値更新向け・決済連携メモ（更新する場合は値が格納される</summary>
		protected string UpdatePaymentMemoForOrderOld { get; set; }
		/// <summary>Update Online Payment Status For Order Old</summary>
		protected string UpdateOnlinePaymentStatusForOrderOld { get; set; }
	}
}
