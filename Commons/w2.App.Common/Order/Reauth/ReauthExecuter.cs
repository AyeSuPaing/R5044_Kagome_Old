/*
=========================================================================================================
  Module      : 再与信実行クラス(ReauthExecuter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Reauth.Actions;
using w2.App.Common.SendMail;
using w2.Common.Logger;

namespace w2.App.Common.Order.Reauth
{
	/// <summary>再与信実行クラス</summary>
	public class ReauthExecuter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthAction">与信アクション（クレジットカード or コンビニ後払い or AmazonPay）</param>
		/// <param name="cancelAction">キャンセルアクション（クレジットカード or コンビニ後払い or Amazon Pay）</param>
		/// <param name="reduceAction">請求金額減額アクション（コンビニ後払い）</param>
		/// <param name="updateAction">注文情報更新アクション(後付款(TriLink後払い))</param>
		/// <param name="reprintAction">請求書再発行アクション（コンビニ後払い）</param>
		/// <param name="salesAction">売上確定アクション（クレジットカード or Amazon Pay）</param>
		/// <param name="refundAction">返金アクション（Amazon Pay）</param>
		/// <param name="billingAction">減額アクション（Gmo掛け払い）</param>
		/// <param name="preCancelFlg">与信前キャンセルフラグ(Amazon Pay)</param>
		public ReauthExecuter(
			IReauthAction reauthAction,
			IReauthAction cancelAction,
			IReauthAction reduceAction,
			IReauthAction updateAction,
			IReauthAction reprintAction,
			IReauthAction salesAction,
			IReauthAction refundAction,
			IReauthAction billingAction,
			bool preCancelFlg = false)
		{
			this.ReauthAction = reauthAction;
			this.CancelAction = cancelAction;
			this.ReduceAction = reduceAction;
			this.UpdateAction = updateAction;
			this.ReprintAction = reprintAction;
			this.SalesAction = salesAction;
			this.RefundAction = refundAction;
			this.BillingAction = billingAction;
			this.PreCancelFlg = preCancelFlg;
			this.AuthLostForError = false;
		}

		/// <summary>再与信</summary>
		/// <returns>再与信結果</returns>
		public ReauthResult Execute()
		{
			var resultDetail = ReauthResult.ResultDetailTypes.Success;
			var orderId = string.Empty;
			var cardTranId = string.Empty;
			var paymentOrderId = string.Empty;
			var apiErrorMessages = string.Empty;
			var sendErrorMail = false;
			var paymentMemoList = new Dictionary<ActionTypes, string>();
			var isAuthResultHold = false;
			var results = new Dictionary<ActionTypes, ReauthActionResult>();

			// Billing Action for GMO
			var actionType = this.BillingAction.GetActionType();
			if ((resultDetail == ReauthResult.ResultDetailTypes.Success)
				&& (actionType == ActionTypes.Billing))
			{
				var billingAction = this.BillingAction.Execute();
				if (billingAction.Result)
				{
					results.Add(actionType, billingAction);
					paymentMemoList.Add(actionType, billingAction.PaymentMemo);
				}
				else
				{
					resultDetail = ReauthResult.ResultDetailTypes.Failure;
					apiErrorMessages = billingAction.ApiErrorMessage;
				}
			}

			// 与信前キャンセル(AmazonPay)
			actionType = this.CancelAction.GetActionType();
			if ((this.PreCancelFlg)
				&& (resultDetail == ReauthResult.ResultDetailTypes.Success)
				&& (actionType == ActionTypes.Cancel))
			{
				var preCancelResult = this.CancelAction.Execute();
				if (preCancelResult.Result)
				{
					results.Add(actionType, preCancelResult);
					paymentMemoList.Add(actionType, preCancelResult.PaymentMemo);
				}
				else
				{
					resultDetail = ReauthResult.ResultDetailTypes.Failure;
					apiErrorMessages = preCancelResult.ApiErrorMessage;
				}
			}

			// 与信（クレジットカード or コンビニ後払い or AmazonPay or 後付款(TriLink後払い)）
			actionType = this.ReauthAction.GetActionType();
			if ((resultDetail == ReauthResult.ResultDetailTypes.Success)
				&& (actionType == ActionTypes.Reauth))
			{
				var reauthResult = this.ReauthAction.Execute();
				if (reauthResult.Result)
				{
					cardTranId = reauthResult.CardTranId;
					paymentOrderId = reauthResult.PaymentOrderId;
					// 再与信してから、取引ID/決済注文ID更新
					this.SalesAction.UpdateReauthInfo(paymentOrderId, cardTranId);
				}
				else
				{
					resultDetail = (this.PreCancelFlg) ? ReauthResult.ResultDetailTypes.FailureButAuthSameAmount : ReauthResult.ResultDetailTypes.Failure;
					if (resultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSameAmount)
					{
						cardTranId = reauthResult.CardTranId;
						paymentOrderId = reauthResult.PaymentOrderId;
					}
					if (reauthResult.AuthLostForError)
					{
						sendErrorMail = true; // Atodeneで失敗したら与信が消えるためエラーメッセージを出す
						AuthLostForError = true;
					}
					apiErrorMessages = reauthResult.ApiErrorMessage;
				}
				orderId = reauthResult.OrderId;
				results.Add(actionType, reauthResult);
				paymentMemoList.Add(actionType, reauthResult.PaymentMemo);
				isAuthResultHold = reauthResult.IsAuthResultHold;
			}

			// 売上確定（クレジットカード・AmazonPay）
			actionType = this.SalesAction.GetActionType();
			if ((resultDetail == ReauthResult.ResultDetailTypes.Success)
				&& (actionType == ActionTypes.Sales))
			{
				var salesResult = this.SalesAction.Execute();
				if (salesResult.Result)
				{
					cardTranId = salesResult.CardTranId;
					paymentOrderId = string.IsNullOrEmpty(salesResult.PaymentOrderId) ? paymentOrderId : salesResult.PaymentOrderId;
				}
				else
				{
					resultDetail = ReauthResult.ResultDetailTypes.FailureButAuthSuccess;
					sendErrorMail = true;

					apiErrorMessages = salesResult.ApiErrorMessage;
				}
				orderId = salesResult.OrderId;
				results.Add(actionType, salesResult);
				paymentMemoList.Add(actionType, salesResult.PaymentMemo);
			}

			// キャンセル（クレジットカード or コンビニ後払い or Amazon Pay or 後付款(TriLink後払い)）
			actionType = this.CancelAction.GetActionType();
			if ((this.PreCancelFlg == false)
				&& (resultDetail == ReauthResult.ResultDetailTypes.Success)
				&& (actionType == ActionTypes.Cancel))
			{
				var cancelResult = this.CancelAction.Execute();
				if (cancelResult.Result == false)
				{
					resultDetail = (this.SalesAction.GetActionType() == ActionTypes.Sales)
						? ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess
						: ReauthResult.ResultDetailTypes.FailureButAuthSuccess;
					sendErrorMail = true;
					apiErrorMessages = cancelResult.ApiErrorMessage;
				}
				else if ((this.CancelAction is CancelPaidyPayAction)
					&& (Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
					&& string.IsNullOrEmpty(cardTranId))
				{
					cardTranId = cancelResult.CardTranId;
					paymentOrderId = cancelResult.PaymentOrderId;
				}
				orderId = cancelResult.OrderId;
				results.Add(actionType, cancelResult);
				paymentMemoList.Add(actionType, cancelResult.PaymentMemo);
			}

			// 請求金額減額（コンビニ後払い or 後付款(TriLink後払い)）
			actionType = this.ReduceAction.GetActionType();
			if ((resultDetail == ReauthResult.ResultDetailTypes.Success)
				&& (actionType == ActionTypes.Reduce))
			{
				var reduceResult = this.ReduceAction.Execute();
				if (reduceResult.Result)
				{
					cardTranId = reduceResult.CardTranId;
					paymentOrderId = string.IsNullOrEmpty(reduceResult.PaymentOrderId) ? paymentOrderId : reduceResult.PaymentOrderId;
				}
				else
				{
					resultDetail = ReauthResult.ResultDetailTypes.Failure;
					apiErrorMessages = reduceResult.ApiErrorMessage;
				}
				orderId = reduceResult.OrderId;
				results.Add(actionType, reduceResult);
				paymentMemoList.Add(actionType, reduceResult.PaymentMemo);
			}

			// 注文情報更新（後付款(TriLink後払い)）
			actionType = this.UpdateAction.GetActionType();
			if ((resultDetail == ReauthResult.ResultDetailTypes.Success)
				&& (actionType == ActionTypes.Update))
			{
				var reduceResult = this.UpdateAction.Execute();
				if (reduceResult.Result)
				{
					cardTranId = reduceResult.CardTranId;
					paymentOrderId = string.IsNullOrEmpty(reduceResult.PaymentOrderId) ? paymentOrderId : reduceResult.PaymentOrderId;
				}
				else
				{
					resultDetail = ReauthResult.ResultDetailTypes.Failure;
					apiErrorMessages = reduceResult.ApiErrorMessage;
				}
				orderId = reduceResult.OrderId;
				results.Add(actionType, reduceResult);
				paymentMemoList.Add(actionType, reduceResult.PaymentMemo);
			}

			// 請求書再発行（コンビニ後払い）
			actionType = this.ReprintAction.GetActionType();
			if ((resultDetail == ReauthResult.ResultDetailTypes.Success)
				&& (actionType == ActionTypes.Reprint))
			{
				var reprintResult = this.ReprintAction.Execute();
				if (reprintResult.Result)
				{
					cardTranId = reprintResult.CardTranId;
				}
				else
				{
					resultDetail = ReauthResult.ResultDetailTypes.Failure;
					apiErrorMessages = reprintResult.ApiErrorMessage;
				}
				orderId = reprintResult.OrderId;
				results.Add(actionType, reprintResult);
				paymentMemoList.Add(actionType, reprintResult.PaymentMemo);
			}

			// 返金（AmazonPay）
			actionType = this.RefundAction.GetActionType();
			if ((resultDetail == ReauthResult.ResultDetailTypes.Success)
				&& (actionType == ActionTypes.Refund))
			{
				var refundResult = this.RefundAction.Execute();
				if (refundResult != null)
				{
					if (refundResult.Result == false)
					{
						resultDetail = ReauthResult.ResultDetailTypes.Failure;
							apiErrorMessages = refundResult.ApiErrorMessage;
					}
					orderId = refundResult.OrderId;
					// 空文字の場合cardTranIdは上書きしない
					if (string.IsNullOrEmpty(refundResult.CardTranId) == false) cardTranId = refundResult.CardTranId;
					paymentOrderId = refundResult.PaymentOrderId;
					results.Add(actionType, refundResult);
					paymentMemoList.Add(actionType, refundResult.PaymentMemo);
				}
			}

			// エラー？
			if (resultDetail != ReauthResult.ResultDetailTypes.Success)
			{
				var errorMessages = string.Empty;
				var errorResult = results.FirstOrDefault(r => r.Value.Result == false);
				// ログ出力
				errorMessages = CreateErrorMessages(errorResult.Key, errorResult.Value);
				var errorMessagesForErrorMail = CreateErrorMessagesForErrorMail(results);
				FileLogger.WriteError(errorMessagesForErrorMail);

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					"再与信時",
					PaymentFileLogger.PaymentType.Unknown,
					PaymentFileLogger.PaymentProcessingType.ReAuthExec,
					errorMessagesForErrorMail,
					new Dictionary<string, string>
					{
						{ "orderId", orderId },
						{ "cardTranId", cardTranId },
						{ "paymentOrderId", paymentOrderId }
					});

				// エラーメール送信
				// 複数の外部連携を実行し途中でエラーが発生した場合のみエラーメールを送信する
				if (sendErrorMail)
				{
					SendMailCommon.SendReauthErrorMail(orderId, errorMessagesForErrorMail);
				}
				return new ReauthResult(resultDetail, paymentMemoList, cardTranId, paymentOrderId, errorMessages, apiErrorMessages);
			}
			return new ReauthResult(ReauthResult.ResultDetailTypes.Success, paymentMemoList, cardTranId, paymentOrderId, isAuthResultHold: isAuthResultHold);
		}

		/// <summary>
		/// エラーメッセージ作成（エラーメール用）
		/// </summary>
		/// <param name="results">結果リスト</param>
		/// <returns>エラーメッセージ</returns>
		public string CreateErrorMessagesForErrorMail(Dictionary<ActionTypes, ReauthActionResult> results)
		{
			var errorMessages = string.Empty;
			var errorResult = results.FirstOrDefault(r => r.Value.Result == false);
			errorMessages = CreateErrorMessages(errorResult.Key, errorResult.Value);

			var resultFormat = "　→再与信アクション{0}=>結果:{1},決済取引ID:{2},APIエラー:{3}\r\n";
			var actionNo = 0;
			foreach (var result in results.Select(r => r.Value))
			{
				actionNo++;
				errorMessages += 
					string.Format(
					resultFormat,
					actionNo,
					(result.Result ? "成功" : "失敗"),
					result.CardTranIdForLog,
					result.ApiErrorMessage);
			}
			return errorMessages;
		}

		/// <summary>
		/// エラーメッセージ作成
		/// </summary>
		/// <param name="errorActionType">エラーアクションタイプ</param>
		/// <param name="errorResult">エラー結果</param>
		/// <returns>エラーメッセージ</returns>
		public string CreateErrorMessages(ActionTypes errorActionType, ReauthActionResult errorResult)
		{
			var actionNo = 0;
			var actionContents = string.Empty;
			var errorActionNo = 0;
			var errorMessagesContents = string.Empty;
			foreach (var action in this.AnyActions)
			{
				actionNo++;
				// 処理内容メッセージ作成
				if (actionContents.Length != 0) actionContents += " > ";
				actionContents += string.Format("({0}){1}", actionNo, action.GetActionName());

				// エラー処理、エラー内容メッセージ作成
				if (action.GetActionType() == errorActionType)
				{
					errorActionNo = actionNo;
					errorMessagesContents = errorResult.ApiErrorMessage;
				}
			}

			string errorMessagesFormat =
				"再与信処理でエラーが発生しました。\r\n\r\n"
				+ "処理内容：{0}\r\n"
				+ "→({1})でエラーが発生しました。\r\n"
				+ "　(エラー内容：{2})\r\n";
			return string.Format(errorMessagesFormat,
				actionContents,
				errorActionNo,
				errorMessagesContents);
		}

		/// <summary>
		/// 与信日時取得
		/// </summary>
		/// <param name="oldOrderAuthDate">古い外部決済与信日</param>
		/// <param name="orderOldPaymentKbn">古い与信の決済種別</param>
		/// <param name="orderNewPaymentKbn">新しい与信の決済種別</param>
		/// <returns>与信日時</returns>
		public DateTime? GetUpdateReauthDate(DateTime? oldOrderAuthDate, string orderOldPaymentKbn, string orderNewPaymentKbn)
		{
			var result = ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atodene)
				&& (this.ReauthAction.GetActionType() == ActionTypes.Reauth)
				&& (orderOldPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (orderNewPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
					? oldOrderAuthDate
					: DateTime.Now;
			return result;
		}

		/// <summary>与信アクション（クレジットカード or コンビニ後払い）</summary>
		private IReauthAction ReauthAction { get; }
		/// <summary>キャンセルアクション（クレジットカード or コンビニ後払い）</summary>
		private IReauthAction CancelAction { get; }
		/// <summary>請求金額減額アクション（コンビニ後払い）</summary>
		private IReauthAction ReduceAction { get; }
		/// <summary>請求金額減額アクション（コンビニ後払い）</summary>
		private IReauthAction UpdateAction { get; }
		/// <summary>請求書再発行アクション（コンビニ後払い）</summary>
		private IReauthAction ReprintAction { get; }
		/// <summary>売上確定アクション（クレジットカード）</summary>
		private IReauthAction SalesAction { get; }
		/// <summary>返金アクション（AmazonPay）</summary>
		private IReauthAction RefundAction { get; }
		/// <summary>Billing Action</summary>
		private IReauthAction BillingAction { get; }
		/// <summary>何もしない以外のアクション</summary>
		private IReauthAction[] AnyActions
		{
			get
			{
				return new[]
				{
					this.ReauthAction,
					this.SalesAction,
					this.CancelAction,
					this.ReduceAction,
					this.ReprintAction,
					this.RefundAction,
					this.UpdateAction,
					this.ReprintAction,
					this.BillingAction
				}
				.Where(action => action.GetActionType() != ActionTypes.NoAction).ToArray();
			}
		}
		/// <summary>何もしない以外をもつ</summary>
		public bool HasAnyAction
		{
			get
			{
				return ((this.ReauthAction.GetActionType() != ActionTypes.NoAction)
					|| (this.CancelAction.GetActionType() != ActionTypes.NoAction)
					|| (this.ReduceAction.GetActionType() != ActionTypes.NoAction)
					|| (this.UpdateAction.GetActionType() != ActionTypes.NoAction)
					|| (this.ReprintAction.GetActionType() != ActionTypes.NoAction)
					|| (this.SalesAction.GetActionType() != ActionTypes.NoAction)
					|| (this.RefundAction.GetActionType() != ActionTypes.NoAction)
					|| (this.BillingAction.GetActionType() != ActionTypes.NoAction));
			}
		}
		/// <summary>再与信・減額・請求書再発行をもつ</summary>
		public bool HasReauthOrReduceOrReprint
		{
			get
			{
				return ((this.ReauthAction.GetActionType() == ActionTypes.Reauth)
					|| (this.ReduceAction.GetActionType() == ActionTypes.Reduce)
					|| (this.ReprintAction.GetActionType() == ActionTypes.Reprint));
			}
		}
		/// <summary>売上確定をもつ</summary>
		public bool HasSales
		{
			get
			{
				return (this.SalesAction.GetActionType() == ActionTypes.Sales);
			}
		}
		/// <summary>キャンセルをもつ</summary>
		public bool HasCancel
		{
			get
			{
				return (this.CancelAction.GetActionType() == ActionTypes.Cancel);
			}
		}
		/// <summary>キャンセルのみもつ</summary>
		public bool HasOnlyCancel
		{
			get
			{
				return ((this.ReauthAction.GetActionType() == ActionTypes.NoAction)
					&& (this.CancelAction.GetActionType() == ActionTypes.Cancel)
					&& (this.ReduceAction.GetActionType() == ActionTypes.NoAction)
					&& (this.UpdateAction.GetActionType() == ActionTypes.NoAction)
					&& (this.ReprintAction.GetActionType() == ActionTypes.NoAction)
					&& (this.SalesAction.GetActionType() == ActionTypes.NoAction)
					&& (this.RefundAction.GetActionType() == ActionTypes.NoAction)
					&& (this.BillingAction.GetActionType() == ActionTypes.NoAction));
			}
		}
		/// <summary>与信前キャンセルフラグ</summary>
		public bool PreCancelFlg { get; set; }
		/// <summary> 与信エラー時に与信が消えるフラグ </summary>
		public bool AuthLostForError { get; private set; }
		/// <summary>Has refund</summary>
		public bool HasRefund
		{
			get
			{
				return (this.RefundAction.GetActionType() == ActionTypes.Refund);
			}
		}
	}
}
