/*
=========================================================================================================
  Module      : 与信期限切れ注文の再与信コマンド(ReauthAuthExpiredOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Reauth;
using w2.App.Common.Order.Reauth.Actions;
using w2.App.Common.SendMail;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.Order;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.InvoiceDskDeferred;

namespace w2.Commerce.Reauth.Commands
{
	/// <summary>
	/// 与信期限切れ注文の再与信コマンド
	/// </summary>
	public class ReauthAuthExpiredOrder : CommandBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="argument">引数</param>
		public ReauthAuthExpiredOrder(Argument argument)
		{
			this.Argument = argument;
			this.TargetDate = argument.TargetDate;
			this.IsAuthOnly = argument.IsAuthOnly;
			this.ExtendStatusNumber = argument.ExtendStatusNumber;
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功件数/全体件数</returns>
		public override Tuple<int, int> Exec(UpdateHistoryAction updateHistoryAction)
		{
			if (Constants.PAYMENT_REAUTH_ENABLED)
			{
				int creditAuthExpire = 0;
				int cvsDefAuthExpire = 0;

				// クレジットカード
				switch (Constants.PAYMENT_CARD_KBN)
				{
					case Constants.PaymentCard.Zeus:
						creditAuthExpire = Constants.CREDIT_ZEUS_AUTH_EXPIRE_DAYS;
						break;

					case Constants.PaymentCard.SBPS:
						creditAuthExpire = Constants.CREDIT_SBPS_AUTH_EXPIRE_DAYS;
						break;

					case Constants.PaymentCard.Gmo:
						creditAuthExpire = Constants.CREDIT_GMO_AUTH_EXPIRE_DAYS;
						break;

					case Constants.PaymentCard.YamatoKwc:
						creditAuthExpire = Constants.CREDIT_YAMATOKWC_AUTH_EXPIRE_DAYS;
						break;

					case Constants.PaymentCard.Zcom:
						creditAuthExpire = Constants.CREDIT_ZCOM__AUTH_EXPIRE_DAYS;
						break;

					case Constants.PaymentCard.EScott:
						creditAuthExpire = Constants.CREDIT_ESCOTT_AUTH_EXPIRE_DAYS;
						break;

					case Constants.PaymentCard.VeriTrans:
						creditAuthExpire = Constants.CREDIT_VERITRANS_AUTH_EXPIRE_DAYS;
						break;

					case Constants.PaymentCard.Rakuten:
						creditAuthExpire = Constants.RAKUTEN_PAY_AUTH_EXPIRE_DAYS;
						break;

					case Constants.PaymentCard.Paygent:
						creditAuthExpire = Constants.CREDIT_PAYGENT_AUTH_EXPIRE_DAYS;
						break;
				}

				// コンビニ後払い
				switch (Constants.PAYMENT_CVS_DEF_KBN)
				{
					case Constants.PaymentCvsDef.YamatoKa:
						cvsDefAuthExpire = Constants.CVSDEF_YAMATOKA_AUTH_EXPIRE_DAYS;
						break;

					case Constants.PaymentCvsDef.Gmo:
						cvsDefAuthExpire = Constants.CVSDEF_GMO_AUTH_EXPIRE_DAYS;
						break;

					case Constants.PaymentCvsDef.Atodene:
						cvsDefAuthExpire = Constants.CVSDEF_ATODENE_AUTH_EXPIRE_DAYS;
						break;

					case Constants.PaymentCvsDef.Dsk:
						cvsDefAuthExpire = Constants.CVSDEF_DSK_AUTH_EXPIRE_DAYS;
						break;

					case Constants.PaymentCvsDef.Score:
						cvsDefAuthExpire = Constants.CVSDEF_SCORE_AUTH_EXPIRE_DAYS;
						break;

					case Constants.PaymentCvsDef.Veritrans:
						cvsDefAuthExpire = Constants.CVSDEF_VERITRANS_AUTH_EXPIRE_DAYS;
						break;
				}

				var successCount = 0;
				var failure = 0;
				var date = this.Argument.TargetDate.Date;
				var extend = this.Argument.ExtendStatusNumber;
				var exec_mode = (this.Argument.IsAuthOnly) ? "与信のみ" : "再与信";
				OrderModel[] orders;
				try
				{
					// 注文取得
					orders = (this.IsAuthOnly)
						? new OrderService().GetOrderForAuth(this.ExtendStatusNumber)
						: new OrderService().GetAuthExpired(
							this.TargetDate,
							creditAuthExpire,
							cvsDefAuthExpire,
							Constants.AMAZONPAY_AUTH_EXPIRE_DAYS,
							Constants.PAIDYPAY_AUTH_EXPIRE_DAYS,
							Constants.LINEPAY_AUTH_EXPIRE_DAYS,
							Constants.NPAFTERPAY_AUTH_EXPIRE_DAYS,
							Constants.RAKUTEN_PAY_AUTH_EXPIRE_DAYS,
							Constants.GMOPOST_AUTH_EXPIRE_DAYS);
				}
				catch (Exception ex)
				{
					SendMailCommon.SendReauthSuccessMail(
						exec_mode,
						date.ToString(),
						extend,
						successCount,
						failure,
						false);

					FileLogger.WriteError(ex);
					return new Tuple<int, int>(0, 0);
				}

				var exceptionMessages = new StringBuilder();
				foreach (var order in orders)
				{
					try
					{
						var result = AuthAndCancel(order, Constants.FLG_LASTCHANGED_BATCH, updateHistoryAction);
						if (result) successCount++;
						else failure++;
					}
					catch (Exception ex)
					{
						failure++;

						var errorMessage = string.Format(
							"再与信処理で予期しない例外が発生しました。注文ID：{0} ユーザーID：{1} 決済種別：{2}",
							order.OrderId,
							order.UserId,
							order.OrderPaymentKbn);
						exceptionMessages.Append(
							string.Format(
								"{0}   {1}",
								Environment.NewLine,
								FileLogger.CreateExceptionMessage(errorMessage, ex)));
					}
				}

				// 例外エラーログ書き込み
				if (exceptionMessages.Length > 0) FileLogger.WriteError(exceptionMessages.ToString());

				// Send mail
				SendMailCommon.SendReauthSuccessMail(
					exec_mode,
					date.ToString(),
					extend,
					successCount,
					failure);

				return new Tuple<int, int>(successCount, orders.Count());
			}
			return new Tuple<int, int>(0, 0);
		}

		/// <summary>
		/// 与信＆キャンセル
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>再与信結果</returns>
		private bool AuthAndCancel(OrderModel order, string lastChanged, UpdateHistoryAction updateHistoryAction)
		{
			// 決済カード取引IDなどプロパティの更新が行われるため別インスタンスを作成
			var authOrder = (OrderModel)order.Clone();
			var cancelOrder = (OrderModel)order.Clone();

			// 再与信対象外の決済種別の場合はFALSEを返す
			if (Constants.NOT_TARGET_REAUTH_PAYMENT_IDS.Contains(order.OrderPaymentKbn)) return false;

			// Returns false if the payment type is convenience store deferred payment of Atobaraicom
			if ((order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atobaraicom)) return false;

			switch (order.OrderPaymentKbn)
			{
				case Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT:
					// 再与信未対応カードの場合FALSE返す
					if (Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN) == false) return false;
					// 再与信
					var creditReauthResult = this.ExecAction(
						new ReauthCreditCardAction(new ReauthCreditCardAction.ReauthActionParams(authOrder)),
						new CancelCreditCardAction(new CancelCreditCardAction.ReauthActionParams(cancelOrder)),
						order,
						lastChanged,
						updateHistoryAction);
					ConsoleLogger.WriteInfo("{0}：{1}：{2}", order.OrderId, order.OrderPaymentKbn, GetResultString(creditReauthResult));
					return creditReauthResult;

				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO:
				case Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE:
					// 再与信
					var invoiceReauthResult = this.ExecAction(
						new ReauthGmoPostAction(new ReauthGmoPostAction.ReauthGmoPostActionParams(authOrder)),
						new CancelGmoPostAction(new CancelGmoPostAction.CancelGmoPostActionParams(cancelOrder)),
						order,
						lastChanged,
						updateHistoryAction);
					ConsoleLogger.WriteInfo("{0}：{1}：{2}", order.OrderId, order.OrderPaymentKbn, GetResultString(invoiceReauthResult));
					return invoiceReauthResult;

				case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF:
					// 再与信未対応コンビニ後払いの場合FALSE返す
					if (Constants.REAUTH_COMPLETE_CVSDEF_LIST.Contains(Constants.PAYMENT_CVS_DEF_KBN) == false) return false;
					// 再与信
					var cvsDefReauthResult = this.ExecAction(
						new ReauthCvsDefAction(new ReauthCvsDefAction.ReauthActionParams(authOrder)),
						new CancelCvsDefAction(new CancelCvsDefAction.ReauthActionParams(cancelOrder)),
						order,
						lastChanged,
						updateHistoryAction);

					// 過去にDSK後払いの請求書印字データ取得していた場合はここで削除
					if ((cvsDefReauthResult)
						&& (authOrder.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
						&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Dsk))
					{
						var invoiceDskDeferredService = new InvoiceDskDeferredService();
						var invoice = invoiceDskDeferredService.Get(authOrder.OrderId);
						if (invoice != null)
						{
							invoiceDskDeferredService.Delete(authOrder.OrderId);
						}
					}

					ConsoleLogger.WriteInfo("{0}：{1}：{2}", order.OrderId, order.OrderPaymentKbn, GetResultString(cvsDefReauthResult));
					return cvsDefReauthResult;

				case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT:
					// 再与信
					var amazonPayReauthResult = this.ExecAction(
						new ReauthAmazonPayAction(new ReauthAmazonPayAction.ReauthActionParams(cancelOrder, authOrder)),
						new CancelAmazonPayAction(new CancelAmazonPayAction.ReauthActionParams(cancelOrder)),
						order,
						lastChanged,
						updateHistoryAction);
					ConsoleLogger.WriteInfo("{0}：{1}：{2}", order.OrderId, order.OrderPaymentKbn, GetResultString(amazonPayReauthResult));
					return amazonPayReauthResult;

				case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2:
					// 再与信
					var amazonPayCv2ReauthResult = this.ExecAction(
						new ReauthAmazonPayCv2Action(new ReauthAmazonPayCv2Action.ReauthActionParams(cancelOrder, authOrder)),
						new DoNothingAction(new DoNothingAction.ReauthActionParams()),
						order,
						lastChanged,
						updateHistoryAction);
					ConsoleLogger.WriteInfo("{0}：{1}：{2}", order.OrderId, order.OrderPaymentKbn, GetResultString(amazonPayCv2ReauthResult));
					return amazonPayCv2ReauthResult;

				case Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY:
					// 再与信
					var paidyPayReauthResult = this.ExecAction(
						new ReauthPaidyPayAction(new ReauthPaidyPayAction.ReauthActionParams(cancelOrder, authOrder)),
						new CancelPaidyPayAction(new CancelPaidyPayAction.ReauthActionParams(
							cancelOrder,
							authOrder,
							isReturnExchange: false)),
						order,
						lastChanged,
						updateHistoryAction);

					ConsoleLogger.WriteInfo("{0}：{1}：{2}", order.OrderId, order.OrderPaymentKbn, GetResultString(paidyPayReauthResult));
					return paidyPayReauthResult;

				case Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY:
					var linePayReauthResult = this.ExecAction(
						new ReauthLinePayAction(new ReauthLinePayAction.ReauthActionParams(cancelOrder, authOrder)),
						new CancelLinePayAction(new CancelLinePayAction.ReauthActionParams(cancelOrder, authOrder)),
						order,
						lastChanged,
						updateHistoryAction);
					ConsoleLogger.WriteInfo("{0}：{1}：{2}", order.OrderId, order.OrderPaymentKbn, GetResultString(linePayReauthResult));
					return linePayReauthResult;

				case Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY:
					// 再与信
					var npAfterPayReauthResult = this.ExecAction(
						new ReauthNPAfterPayAction(new ReauthNPAfterPayAction.ReauthActionParams(cancelOrder, authOrder)),
						new CancelNPAfterPayAction(new CancelNPAfterPayAction.ReauthActionParams(cancelOrder)),
						order,
						lastChanged,
						updateHistoryAction);
					ConsoleLogger.WriteInfo("{0}：{1}：{2}", order.OrderId, order.OrderPaymentKbn, GetResultString(npAfterPayReauthResult));
					return npAfterPayReauthResult;

				default:
					throw new Exception("対応しない与信：" + order.OrderPaymentKbn);
			}
		}

		/// <summary>
		/// アクション実行
		/// </summary>
		/// <param name="reauthAction">アクション1（与信）</param>
		/// <param name="cancelAction">アクション2（キャンセル）</param>
		/// <param name="order">注文情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>実行結果</returns>
		private bool ExecAction(
			IReauthAction reauthAction,
			IReauthAction cancelAction,
			OrderModel order,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			// 再与信実行クラス作成（再与信とキャンセルのみ）
			var reauthExecuter = new ReauthExecuter(
				reauthAction,
				cancelAction,
				new DoNothingAction(new DoNothingAction.ReauthActionParams()),
				new DoNothingAction(new DoNothingAction.ReauthActionParams()),
				new DoNothingAction(new DoNothingAction.ReauthActionParams()),
				new DoNothingAction(new DoNothingAction.ReauthActionParams()),
				new DoNothingAction(new DoNothingAction.ReauthActionParams()),
				new DoNothingAction(new DoNothingAction.ReauthActionParams()));

			var results = new Dictionary<ActionTypes, ReauthActionResult>();

			// 「キャンセル」実行
			// AmazonPayは与信前に実行する
			var isAmazonPayment = (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT);
			if (isAmazonPayment && (this.IsAuthOnly == false))
			{

				var execResult = ExecCancelAction(
					cancelAction,
					order,
					lastChanged,
					updateHistoryAction,
					results,
					reauthExecuter
					);

				if (execResult == false) return false;
			}

			// 「再与信」実行
			var reauthResult = reauthAction.Execute();
			results.Add(ActionTypes.Reauth, reauthResult);

			// 外部決済連携ログ格納処理
			OrderCommon.AppendExternalPaymentCooperationLog(
				reauthResult.Result,
				order.OrderId,
				reauthResult.Result
					? LogCreator.CreateMessage(order.OrderId, order.PaymentOrderId)
					: reauthResult.ApiErrorMessage,
				lastChanged,
				UpdateHistoryAction.Insert);

			if (reauthResult.Result == false)
			{
				new OrderService().UpdateExternalPaymentInfoForAuthError(
					order.OrderId,
					reauthExecuter.CreateErrorMessages(reauthAction.GetActionType(), reauthResult),
					lastChanged,
					updateHistoryAction);

				var errorMessages = reauthExecuter.CreateErrorMessagesForErrorMail(results);
				AppLogger.WriteError(errorMessages);
				SendMailCommon.SendReauthErrorMail(reauthResult.OrderId, errorMessages);
				return false;
			}

			// 注文情報更新
			UpdateOrderForReauthSuccess(
				order,
				reauthResult.PaymentOrderId,
				reauthResult.CardTranId,
				reauthResult.PaymentMemo,
				lastChanged,
				UpdateHistoryAction.Insert);

			// 「キャンセル」実行
			if ((isAmazonPayment == false) && (this.IsAuthOnly == false))
			{
				var execResult = ExecCancelAction(
					cancelAction,
					order,
					lastChanged,
					updateHistoryAction,
					results,
					reauthExecuter
					);

				if (execResult == false) return false;
			}
			return true;
		}

		/// <summary>
		/// 再与信キャンセルアクション実行
		/// </summary>
		/// <param name="action2">アクション2（キャンセル）</param>
		/// <param name="order">注文情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="results">実行結果</param>
		/// <param name="reauthExecuter">再与信実行インスタンス</param>
		/// <param name="apiErrorMessage">apiエラーメッセージ</param>
		/// <returns>成功：true、失敗：false</returns>
		private bool ExecCancelAction(
			IReauthAction action2,
			OrderModel order,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			Dictionary<ActionTypes, ReauthActionResult> results,
			ReauthExecuter reauthExecuter)
		{
			var cancelResult = action2.Execute();
			results.Add(ActionTypes.Cancel, cancelResult);

			OrderCommon.AppendExternalPaymentCooperationLog(
				cancelResult.Result,
				order.OrderId,
				cancelResult.Result
					? LogCreator.CreateMessage(order.OrderId, order.PaymentOrderId)
					: cancelResult.ApiErrorMessage,
				lastChanged,
				UpdateHistoryAction.Insert);

			if (cancelResult.Result == false)
			{
				// 再与信時キャンセル失敗フラグON
				new OrderService().UpdateOrderExtendStatus(
					order.OrderId,
					w2.App.Common.Constants.ORDER_EXTEND_STATUS_NO_AUTHEXPIRED_REAUTH_CANCEL_FAILED,
					Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON,
					DateTime.Now.Date,
					lastChanged,
					updateHistoryAction);

				var errorMessages = reauthExecuter.CreateErrorMessagesForErrorMail(results);

				AppLogger.WriteError(errorMessages);

				SendMailCommon.SendReauthErrorMail(cancelResult.OrderId, errorMessages);
				return false;
			}
			// 注文情報更新
			this.UpdateOrderForCancelSuccess(order, cancelResult.PaymentMemo, lastChanged, updateHistoryAction);

			return true;
		}

		/// <summary>
		/// 再与信成功向け注文情報更新
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="paymentMemo">決済メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		private void UpdateOrderForReauthSuccess(
			OrderModel order,
			string paymentOrderId,
			string cardTranId,
			string paymentMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			var orderService = new OrderService();

			var invoiceBundleFlg = OrderCommon.IsInvoiceBundleServiceUsable(order.OrderPaymentKbn)
				? OrderCommon.JudgmentInvoiceBundleFlg(order)
				: Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;

			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// カード取引ID更新
				orderService.UpdateCardTransaction(order.OrderId, paymentOrderId, cardTranId, lastChanged, UpdateHistoryAction.DoNotInsert, accessor);
				// 外部決済情報更新
				orderService.UpdateExternalPaymentInfoForAuthSuccess(order.OrderId, lastChanged, UpdateHistoryAction.DoNotInsert, accessor);
				// 決済連携メモ追加
				orderService.AddPaymentMemo(order.OrderId, paymentMemo, lastChanged, UpdateHistoryAction.DoNotInsert, accessor);
				// 請求書同梱フラグ更新
				orderService.UpdateInvoiceBundleFlg(order.OrderId, invoiceBundleFlg, lastChanged, UpdateHistoryAction.DoNotInsert, accessor);

				// For case order has payment paidy
				if (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
				{
					var orderIdOrg = string.IsNullOrEmpty(order.OrderIdOrg)
						? order.OrderId
						: order.OrderIdOrg;

					// Update Related Payment Order Id
					orderService.UpdateRelatedPaymentOrderId(
						orderIdOrg,
						order.OrderId,
						paymentOrderId,
						lastChanged,
						UpdateHistoryAction.Insert,
						accessor);
				}

				// 与信のみの場合は拡張ステータスをオフにする
				if (this.IsAuthOnly)
				{
					orderService.UpdateOrderExtendStatus(
						order.OrderId,
						int.Parse(this.ExtendStatusNumber),
						Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF,
						DateTime.Today,
						lastChanged,
						UpdateHistoryAction.DoNotInsert,
						accessor);
				}

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForOrder(order.OrderId, lastChanged, accessor);
				}
				accessor.CommitTransaction();
			}
		}

		/// <summary>
		/// キャンセル成功向け注文情報更新
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="paymentMemo">決済メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		private void UpdateOrderForCancelSuccess(OrderModel order, string paymentMemo, string lastChanged, UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 決済連携メモ追加
				new OrderService().AddPaymentMemo(order.OrderId, paymentMemo, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
			}
		}

		/// <summary>
		/// 結果文字列
		/// </summary>
		/// <param name="result">結果</param>
		/// <returns>結果文字列</returns>
		private string GetResultString(bool result)
		{
			return (result ? "OK" : "NG");
		}

		/// <summary>引数</summary>
		public Argument Argument { get; private set; }
		/// <summary>対象日</summary>
		public DateTime TargetDate { get; private set; }
		/// <summary>拡張ステータス番号</summary>
		protected string ExtendStatusNumber { get; private set; }
		/// <summary>APIエラーメッセージ</summary>
		private string ApiErrorMessage { get; set; }
		/// <summary>与信のみか？</summary>
		protected bool IsAuthOnly { get; private set; }
	}
}
