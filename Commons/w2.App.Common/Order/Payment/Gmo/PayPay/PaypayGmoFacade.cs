/*
=========================================================================================================
  Module      : Paypayファサード (PaypayGmoFacade.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Web;
using w2.App.Common.Order.Payment.Paypay.Request;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserCreditCard;

namespace w2.App.Common.Order.Payment.Paypay
{
	/// <summary>
	/// API種別
	/// </summary>
	public enum ApiType
	{
		/// <summary>JSON形式</summary>
		Json,
		/// <summary>IdPass形式（GMO独自の形式）</summary>
		IdPass,
	}

	/// <summary>
	/// Paypayファサード
	/// </summary>
	public class PaypayGmoFacade : IPayment
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiInvoker">APIインボーカ</param>
		public PaypayGmoFacade(PaypayGmoApiInvoker apiInvoker = null)
		{
			this.ApiInvoker = apiInvoker ?? new PaypayGmoApiInvoker();
		}

		/// <summary>
		/// 決済実行
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <param name="order">注文</param>
		/// <param name="isExecOrderNow">Is exec order now</param>
		/// <returns>実行結果</returns>
		public PaypayGmoResult ExecPayment(
			CartObject cart,
			OrderModel order,
			bool isExecOrderNow = false)
		{
			// 取引登録
			var isExecFixedPurchase = ((cart != null)
				&& cart.HasFixedPurchase
				&& (isExecOrderNow == false));

			var entryResult = this.ApiInvoker.EntryTran(
				new EntryTranRequest(order, isExecOrderNow),
				isExecFixedPurchase);

			if (entryResult.IsError)
			{
				FileLogger.WriteError(entryResult.GetErrorMessages());
				return new PaypayGmoResult
				{
					Result = Results.Failed | Results.PreOrderRollbackIsRequired,
					ErrorMessage = entryResult.GetErrorMessages(),
				};
			}

			order.CardTranId = entryResult.AccessId;
			order.CardTranPass = entryResult.AccessPassword;

			// 取引実行
			var request = isExecOrderNow
				? new ExecTranRequest(order, cart.FixedPurchase, entryResult.AccessId, entryResult.AccessPassword)
				: new ExecTranRequest(order, entryResult.AccessId, entryResult.AccessPassword);
			var execResult = this.ApiInvoker.ExecTran(
				request,
				isExecFixedPurchase);

			if (execResult.IsError)
			{
				FileLogger.WriteError(execResult.GetErrorMessages());
				return new PaypayGmoResult
				{
					Result = Results.Failed | Results.PreOrderRollbackIsRequired,
					ErrorMessage = execResult.GetErrorMessages(),
				};
			}

			// Update external payment transaction id
			if (isExecFixedPurchase)
			{
				var cooperationId = string.Format(
					"{0} {1}",
					entryResult.AccessId,
					entryResult.AccessPassword);

				var userCreditCard = new UserCreditCardModel
				{
					UserId = order.UserId,
					CooperationId = cooperationId,
					LastChanged = order.LastChanged,
					CooperationType = Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_PAYPAY,
					DispFlg = Constants.FLG_USERCREDITCARD_DISP_FLG_OFF,
				};

				DomainFacade.Instance.UserCreditCardService.Insert(userCreditCard, UpdateHistoryAction.Insert);

				var branchNo = DomainFacade.Instance.UserCreditCardService.GetMaxBranchNo(order.UserId);
				DomainFacade.Instance.FixedPurchaseService.Modify(
					order.FixedPurchaseId,
					Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ORDER_COUNT_UPDATE,
					(model, historyModel) =>
					{
						model.CreditBranchNo = branchNo;
					},
					order.LastChanged,
					UpdateHistoryAction.DoNotInsert);
			}
			else
			{
				// 注文情報更新
				new OrderService().Modify(
					order.OrderId,
					model =>
					{
						model.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_WAIT;
						model.PaymentOrderId = order.PaymentOrderId;
						model.CardTranId = order.CardTranId;
						model.CardTranPass = order.CardTranPass;
					},
					UpdateHistoryAction.Insert);
			}

			if (isExecOrderNow)
			{
				return new PaypayGmoResult
				{
					Result = Results.Success,
					Status = Statuses.Captured,
					PaypayTrackingId = execResult.PaypayTrackingId,
				};
			}

			return new PaypayGmoResult
			{
				Result = Results.Success | Results.RedirectionIsRequired,
				RedirectUrl = new UrlCreator(execResult.StartUrl)
					.AddParam(PaypayConstants.EXTERNAL_REQUEST_KEY_PAYPAY_ACCESS_ID, execResult.AccessId)
					.AddParam(PaypayConstants.EXTERNAL_REQUEST_KEY_PAYPAY_TOKEN, execResult.Token)
					.CreateUrl(),
			};
		}

		/// <summary>
		/// 決済キャンセル
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>実行結果</returns>
		public PaypayGmoResult CancelPayment(OrderModel order)
		{
			var response = this.ApiInvoker.CancelReturnPayment(new CancelReturnPaymentRequest(order));
			if (response.IsError)
			{
				FileLogger.WriteError(response.GetErrorMessages());
				return new PaypayGmoResult
				{
					Status = Statuses.Unprocessed,
					Result = Results.Failed,
					ErrorMessage = response.GetErrorMessages(),
				};
			}

			return new PaypayGmoResult
			{
				Status = Statuses.Canceled,
				Result = Results.Success,
			};
		}

		/// <summary>
		/// 一部返金
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="refundAmount">返金金額</param>
		/// <returns>実行結果</returns>
		public PaypayGmoResult RefundPayment(OrderModel order, decimal refundAmount)
		{
			var response = this.ApiInvoker.CancelReturnPayment(new CancelReturnPaymentRequest(order, refundAmount));
			if (response.IsError)
			{
				FileLogger.WriteError(response.GetErrorMessages());
				return new PaypayGmoResult
				{
					Result = Results.Failed,
					Status = Statuses.Unprocessed,
					ErrorMessage = response.GetErrorMessages(),
				};
			}

			return new PaypayGmoResult
			{
				Status = Statuses.Auth,
				Result = Results.Success,
			};
		}

		/// <summary>
		/// 売上確定
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>実行結果</returns>
		public PaypayGmoResult CapturePayment(OrderModel order)
		{
			var response = this.ApiInvoker.SalesPayment(new SalesPaymentRequest(order));
			if (response.IsError)
			{
				FileLogger.WriteError(response.GetErrorMessages());
				return new PaypayGmoResult
				{
					Result = Results.Failed,
					Status = Statuses.Unprocessed,
					ErrorMessage = response.GetErrorMessages(),
				};
			}

			return new PaypayGmoResult
			{
				Result = Results.Success,
				Status = Statuses.Captured,
			};
		}

		/// <summary>
		/// Accept end payment
		/// </summary>
		/// <param name="fixedPurchaseModel">fixed purchase model</param>
		/// <returns>Paypay Gmo result</returns>
		public PaypayGmoResult AcceptEndPayment(FixedPurchaseModel fixedPurchaseModel)
		{
			var cardTranId = string.Empty;
			var cardTranPass = string.Empty;

			if (fixedPurchaseModel.CreditBranchNo == null) return new PaypayGmoResult();

			var branchNo = (int)fixedPurchaseModel.CreditBranchNo;
			var userCreditCardModel = DomainFacade.Instance.UserCreditCardService.Get(
				fixedPurchaseModel.UserId,
				branchNo);

			if (string.IsNullOrEmpty(userCreditCardModel.CooperationId) == false)
			{
				var externalPaymentTransactionIds = userCreditCardModel.CooperationId.Split();
				cardTranId = externalPaymentTransactionIds[0];
				cardTranPass = (externalPaymentTransactionIds.Length > 1) ? externalPaymentTransactionIds[1] : string.Empty;
			}

			var response = this.ApiInvoker.AcceptEndPayment(
				new AcceptEndPaymentRequest(
					cardTranId,
					cardTranPass,
					fixedPurchaseModel.FixedPurchaseId,
					fixedPurchaseModel.ExternalPaymentAgreementId));

			if (response.IsError)
			{
				FileLogger.WriteError(response.GetErrorMessages());
				return new PaypayGmoResult
				{
					Result = Results.Failed,
					Status = Statuses.Unprocessed,
					ErrorMessage = response.GetErrorMessages(),
				};
			}

			return new PaypayGmoResult
			{
				Result = Results.Success,
				Status = Statuses.Canceled,
			};
		}

		/// <summary>
		/// 外部決済結果受信
		/// </summary>
		/// <param name="httpContext">HTTPコンテキスト</param>
		/// <param name="order">注文情報</param>
		/// <returns>実行結果</returns>
		public PaypayGmoResult ReceiveExternalSitePaymentResult(HttpContext httpContext, OrderModel order)
		{
			var request = new ReceivedPaymentResult(httpContext);
			if (request.Errors.Any()
				|| (request.Status == PaypayConstants.FLG_PAYPAY_STATUS_UNPROCESSED)
				|| (request.Status == PaypayConstants.FLG_PAYPAY_STATUS_PAYFAIL)
				|| (request.Status == PaypayConstants.FLG_PAYPAY_STATUS_EXPIRED))
			{
				var errorMessage = string.Format(
					"{0}{1}",
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PAYPAY_AUTH_ERROR),
					request.GetErrorMessages());

				FileLogger.WriteError(errorMessage);
				return new PaypayGmoResult
				{
					Result = Results.PreOrderRollbackIsRequired | Results.Failed,
					Status = (request.Errors.Any(err => err.IsCanceledByUser())
						? Statuses.Canceled
						: Statuses.Unknown),
					ErrorMessage = errorMessage,
				};
			}

			// 注文情報更新
			order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;

			// Update external payment agreement id
			if (string.IsNullOrEmpty(order.FixedPurchaseId) == false)
			{
				DomainFacade.Instance.FixedPurchaseService.SetExternalPaymentAgreementId(
					order.FixedPurchaseId,
					request.PaypayAcceptCode,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert);
			}

			// 注文確定処理は上位側
			var result = new PaypayGmoResult
			{
				Result = Results.Success,
			};

			switch (request.Status)
			{
				case PaypayConstants.FLG_PAYPAY_STATUS_REQAUTH:
					result.Status = Statuses.Captured;
					break;

				case PaypayConstants.FLG_PAYPAY_STATUS_AUTH:
					result.Status = Statuses.Auth;
					break;

				case PaypayConstants.FLG_PAYPAY_STATUS_REGISTER:
					result.Status = Statuses.Register;
					break;

				default:
					result.Status = Statuses.Unknown;
					break;
			}
			return result;
		}

		/// <summary>
		/// 取引情報取得
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>取引情報</returns>
		public PaypayGmoTransaction GetTransaction(OrderModel order)
		{
			var response = this.ApiInvoker.SearchTrade(
				new SearchTradeRequest(order));

			var result = new PaypayGmoTransaction();
			switch (response.Status)
			{
				case PaypayConstants.FLG_PAYPAY_STATUS_UNPROCESSED:
					result.Status = Statuses.Unprocessed;
					break;

				case PaypayConstants.FLG_PAYPAY_STATUS_REQAUTH:
				case PaypayConstants.FLG_PAYPAY_STATUS_AUTH:
					result.Status = Statuses.Auth;
					break;

				case PaypayConstants.FLG_PAYPAY_STATUS_SALES:
				case PaypayConstants.FLG_PAYPAY_STATUS_CAPTURE:
					result.Status = Statuses.Captured;
					break;

				case PaypayConstants.FLG_PAYPAY_STATUS_RETURN:
					result.Status = Statuses.Return;
					break;

				case PaypayConstants.FLG_PAYPAY_STATUS_CANCEL:
					result.Status = Statuses.Canceled;
					break;

				case PaypayConstants.FLG_PAYPAY_STATUS_EXPIRED:
					result.Status = Statuses.Expired;
					break;

				case PaypayConstants.FLG_PAYPAY_STATUS_PAYFAIL:
					result.Status = Statuses.Unknown;
					break;

				default:
					result.Status = Statuses.Unknown;
					break;
			}

			return result;
		}

		/// <summary>APIインボーカ</summary>
		public PaypayGmoApiInvoker ApiInvoker { get; set; }
	}
}
