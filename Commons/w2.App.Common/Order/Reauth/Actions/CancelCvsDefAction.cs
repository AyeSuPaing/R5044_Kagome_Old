/*
=========================================================================================================
  Module      : 再与信アクション（コンビニ後払いキャンセル）クラス(CancelCvsDefAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.DSKDeferred.OrderCancel;
using w2.Domain.Order;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.OrderModifyCancel;
using w2.App.Common.Order.Payment.JACCS.ATODENE;
using w2.App.Common.Order.Payment.JACCS.ATODENE.ModifyTransaction;
using w2.App.Common.Order.Payment.Score;
using w2.App.Common.Order.Payment.Score.Cancel;
using w2.App.Common.Order.Payment.Veritrans;
using w2.Common.Helper;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（コンビニ後払いキャンセル）クラス
	/// </summary>
	public class CancelCvsDefAction : BaseReauthAction<CancelCvsDefAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CancelCvsDefAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Cancel, "コンビニ後払いキャンセル", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// コンビニ後払いキャンセル
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			ReauthActionResult result = null;
			var order = reauthActionParams.Order;

			switch (Constants.PAYMENT_CVS_DEF_KBN)
			{
				case Constants.PaymentCvsDef.YamatoKa:
					result = ExecCancelYamatoKa(order);
					break;

				case Constants.PaymentCvsDef.Gmo:
					result = ExecCancelGmo(order);
					break;

				case Constants.PaymentCvsDef.Atodene:
					result = ExecCancelAtodene(order);
					break;

				case Constants.PaymentCvsDef.Dsk:
					result = ExecCancelDSKDeferred(order);
					break;

				case Constants.PaymentCvsDef.Atobaraicom:
					result = ExecCancelAtobaraicom(order);
					break;

				case Constants.PaymentCvsDef.Score:
					result = ExecCancelScore(order);
					break;

				case Constants.PaymentCvsDef.Veritrans:
					result = ExecCancelVeritrans(order);
					break;

				// その他
				default:
					result = new ReauthActionResult(
						false,
						order.OrderId,
						string.Empty,
						apiErrorMessage: "該当のコンビニ後払いは再与信をサポートしていません。");
					break;
			}
			return result;
		}

		/// <summary>
		/// ヤマトコンビニ後払いキャンセル処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelYamatoKa(OrderModel order)
		{
			var api = new PaymentYamatoKaCancelApi();
			if (api.Exec(order.CardTranId) == false)
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(api.ResponseData.ErrorCode, api.ResponseData.ErrorMessages));
			}
			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(order.OrderPaymentKbn, order.PaymentOrderId, order.CardTranId, order.LastBilledAmount),
				cardTranIdForLog: order.CardTranId);
		}

		/// <summary>
		/// GMOコンビニ後払いキャンセル処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelGmo(OrderModel order)
		{
			var request = new GmoRequestOrderModifyCancel();
			request.KindInfo = new KindInfoElement();
			request.KindInfo.UpdateKind = UpdateKindType.OrderCancel;
			request.Buyer = new BuyerElement();
			request.Buyer.GmoTransactionId = order.CardTranId;
			request.Buyer.ShopTransactionId = order.PaymentOrderId;

			var result = new GmoDeferredApiFacade().OrderModifyCancel(request);
			if (result.Result != ResultCode.OK)
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: string.Join("\t", result.Errors.Error.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage))));
			}
			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(order.OrderPaymentKbn, order.PaymentOrderId, order.CardTranId, order.LastBilledAmount),
				cardTranIdForLog: order.CardTranId);
		}

		/// <summary>
		/// Atodeneキャンセル処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelAtodene(OrderModel order)
		{
			// アダプタ生成してAPIたたく
			var adp = new AtodeneTransactionMpdofyModelAdapter(order);
			var res = adp.ExecuteCancel();

			if (res.Result != AtodeneConst.RESULT_OK)
			{
				var errorMsg = "";

				if ((res.Errors != null) && (res.Errors.Error != null))
				{
					errorMsg = string.Join("\t", res.Errors.Error.Select(e => LogCreator.CreateErrorMessage(e.ErrorCode, e.ErrorMessage)));
				}

				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: errorMsg);
			}

			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(order.OrderPaymentKbn, order.PaymentOrderId, order.CardTranId, order.LastBilledAmount),
				cardTranIdForLog: order.CardTranId);
		}

		/// <summary>
		/// DSK後払いキャンセル処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelDSKDeferred(OrderModel order)
		{
			var adapter = new DskDeferredOrderCancelAdapter(order.CardTranId, order.PaymentOrderId, order.LastBilledAmount.ToPriceString());
			var response = adapter.Execute();

			if (response.IsResultOk == false)
			{
				var errorMessage = "";

				if ((response.Errors != null) && (response.Errors.Error != null))
				{
					errorMessage = string.Join("\t", response.Errors.Error.Select(e => LogCreator.CreateErrorMessage(e.ErrorCode, e.ErrorMessage)));
				}

				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: errorMessage);
			}

			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(order.OrderPaymentKbn, order.PaymentOrderId, order.CardTranId, order.LastBilledAmount),
				cardTranIdForLog: order.CardTranId);
		}

		/// <summary>
		/// Atobaraicom後払いキャンセル処理
		/// </summary>
		/// <param name="order">Order model</param>
		/// <returns>後払いキャンセルの実行</returns>
		private ReauthActionResult ExecCancelAtobaraicom(OrderModel order)
		{
			var resutl = new AtobaraicomCancelationApi();
			var res = resutl.ExecCancel(order.PaymentOrderId);

			if (res == false)
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: string.Join("\t", resutl.ResponseMessage));
			}

			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(order.OrderPaymentKbn, order.PaymentOrderId, order.CardTranId, order.LastBilledAmount),
				cardTranIdForLog: order.CardTranId);
		}

		/// <summary>
		/// スコア後払いキャンセル処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelScore(OrderModel order)
		{
			var request = new ScoreCancelRequest(order);
			var result = new ScoreApiFacade().OrderCancel(request);

			if (result.Result != ScoreResult.Ok.ToText())
			{
				var isPaidError = result.Errors.ErrorList.All(error => error.ErrorCode == ScoreConstants.SCORE_CVSDEF_PAID_ERROR_CODE);
				var errorMessage = string.Join("\t", result.Errors.ErrorList.Select(error => LogCreator.CreateErrorMessage(error.ErrorCode, error.ErrorMessage)));

				// 入金済みでキャンセル失敗以外の場合は失敗扱いとする
				if (isPaidError == false)
				{
					return new ReauthActionResult(
						result: false,
						orderId: order.OrderId,
						paymentMemo: string.Empty,
						paymentOrderId: order.PaymentOrderId,
						cardTranId: order.CardTranId,
						apiErrorMessage: errorMessage);
				}

				OrderCommon.AppendExternalPaymentCooperationLog(
					isSuccess: false,
					orderId: order.OrderId,
					externalPaymentLog: errorMessage,
					lastChanged: Constants.FLG_LASTCHANGED_SYSTEM,
					updateHistoryAction: UpdateHistoryAction.Insert);
			}

			return new ReauthActionResult(
				result: true,
				orderId: order.OrderId,
				paymentMemo: CreatePaymentMemo(order.OrderPaymentKbn, order.PaymentOrderId, order.CardTranId, order.LastBilledAmount),
				cardTranId: order.CardTranId,
				paymentOrderId: order.PaymentOrderId,
				cardTranIdForLog: order.CardTranId);
		}

		/// <summary>
		/// ベリトランス後払いキャンセル処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelVeritrans(OrderModel order)
		{
			var result = new PaymentVeritransCvsDef().OrderCancel(order.PaymentOrderId);
			if (result.Mstatus != VeriTransConst.RESULT_STATUS_OK)
			{
				return new ReauthActionResult(
					result: false,
					orderId: order.OrderId,
					paymentMemo: string.Empty,
					cardTranId: order.CardTranId,
					paymentOrderId: order.PaymentOrderId,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: string.Join(
						"\t",
						result.Errors?
							.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage))
							.ToArray() ?? Array.Empty<string>()));
			}
			return new ReauthActionResult(
				result: true,
				orderId: order.OrderId,
				paymentMemo: CreatePaymentMemo(
					order.OrderPaymentKbn,
					order.PaymentOrderId,
					order.CardTranId,
					order.LastBilledAmount),
				cardTranId: order.CardTranId,
				paymentOrderId: order.PaymentOrderId,
				cardTranIdForLog: order.CardTranId);
		}
		#endregion

		/// <summary>
		/// 再与信アクションパラメタ
		/// </summary>
		public new class ReauthActionParams : IReauthActionParams
		{
			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="order">注文情報</param>
			public ReauthActionParams(OrderModel order)
			{
				this.Order = order;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報</summary>
			public OrderModel Order { get; private set; }
			#endregion
		}
	}
}
