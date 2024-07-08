/*
=========================================================================================================
  Module      : 再与信アクション（クレジットカードキャンセル）クラス(CancelCreditCardAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.EScott;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.Zcom;
using w2.App.Common.Order.Payment.GMO.Zcom.Cancel;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment.Rakuten;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Payment.YamatoKwc;
using w2.App.Common.Order.Payment.Zeus;
using w2.Common.Sql;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（クレジットカードキャンセル）クラス
	/// </summary>
	public class CancelCreditCardAction : BaseReauthAction<CancelCreditCardAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <param name="accessor">Sql accessor</param>
		public CancelCreditCardAction(ReauthActionParams reauthActionParams, SqlAccessor accessor = null)
			: base(ActionTypes.Cancel, "クレジットカード決済キャンセル", reauthActionParams, accessor)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// クレジットカードキャンセル
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			ReauthActionResult result = null;
			var order = reauthActionParams.Order;

			switch (Constants.PAYMENT_CARD_KBN)
			{
				// カード・ゼウス決済処理
				case Constants.PaymentCard.Zeus:
					result = ExecCancelCardZeus(order);
					break;

				// カード・SBPS決済処理
				case Constants.PaymentCard.SBPS:
					result = ExecCancelCardSbps(order);
					break;

				// カード・YamatoKWCクレジット決済処理
				case Constants.PaymentCard.YamatoKwc:
					result = ExecCancelCardYamatoKwc(order);
					break;

				// カード・GMOクレジット決済処理
				case Constants.PaymentCard.Gmo:
					result = ExecCancelCardGmo(order);
					break;

				// カード・Zcom決済処理
				case Constants.PaymentCard.Zcom:
					result = ExecCancelCardZcom(order);
					break;

				// カード・e-SCOTT決済処理
				case Constants.PaymentCard.EScott:
					result = ExecCancelCardEScott(order);
					break;

				// カード・ベリトランス決済処理
				case Constants.PaymentCard.VeriTrans:
					result = ExecCancelCardVeriTrans(order);
					break;

				// Rakuten
				case Constants.PaymentCard.Rakuten:
					result = ExecCancelCardRakuten(order);
					break;

				case Constants.PaymentCard.Paygent:
					result = ExecCancelCardPaygent(order);
					break;

				default: return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: "該当のクレジットカードは与信の取消し連動をサポートしていません。");
			}
			return result;
		}

		/// <summary>
		/// ゼウスクレジットキャンセル処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelCardZeus(OrderModel order)
		{
			var result = new ZeusSecureBatchCancelApi().Exec(order.CardTranId);
			if (result.Success == false)
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: result.ErrorMessage);
			}

			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(
					order.OrderPaymentKbn,
					order.OrderId,
					order.CardTranId,
					GetSendingAmount(order)),
				cardTranIdForLog: order.CardTranId);
		}

		/// <summary>
		/// SBPSクレジットキャンセル処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelCardSbps(OrderModel order)
		{
			if (string.IsNullOrEmpty(order.CardTranId) == false)
			{
				var api = new PaymentSBPSCreditCancelApi();
				if (api.Exec(order.CardTranId) == false)
				{
					return new ReauthActionResult(
						false,
						order.OrderId,
						string.Empty,
						cardTranIdForLog: order.CardTranId,
						apiErrorMessage: LogCreator.CreateErrorMessage(api.ResponseData.ResErrCode, api.ResponseData.ResErrMessages));
				}
			}
			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(
					order.OrderPaymentKbn,
					order.PaymentOrderId,
					order.CardTranId,
					GetSendingAmount(order)),
				cardTranIdForLog: order.CardTranId);
		}

		/// <summary>
		/// YamatoKWCクレジットキャンセル処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelCardYamatoKwc(OrderModel order)
		{
			var api = new PaymentYamatoKwcCreditCancelApi();
			var response = api.Exec(order.PaymentOrderId);
			if (response.Success == false)
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(response.ErrorCode, response.ErrorMessage));
			}
			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(
					order.OrderPaymentKbn,
					order.PaymentOrderId,
					order.CardTranId,
					GetSendingAmount(order)),
				cardTranIdForLog: order.CardTranId);
		}

		/// <summary>
		/// GMOクレジットキャンセル処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelCardGmo(OrderModel order)
		{
			if (string.IsNullOrEmpty(order.CardTranId) == false)
			{
				var api = new PaymentGmoCredit();
				if (api.Cancel(order.PaymentOrderId, order.CardTranId, order.OrderId) == false)
				{
					return new ReauthActionResult(
						false,
						order.OrderId,
						string.Empty,
						cardTranIdForLog: order.CardTranId,
						apiErrorMessage: api.ErrorMessages);
				}
			}
			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(
					order.OrderPaymentKbn,
					order.PaymentOrderId,
					order.CardTranId,
					GetSendingAmount(order)),
				cardTranIdForLog: order.CardTranId);
		}

		/// <summary>
		/// Zcomキャンセル処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelCardZcom(OrderModel order)
		{
			var adp = new ZcomCancelRequestAdapter(order.PaymentOrderId);
			var result = adp.Execute();

			if (result.IsSuccessResult() == false)
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(result.GetErrorCodeValue(), result.GetErrorDetailValue()));
			}

			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(
					order.OrderPaymentKbn,
					order.OrderId,
					order.CardTranId,
					GetSendingAmount(order)),
				cardTranIdForLog: order.CardTranId);
		}

		/// <summary>
		/// e-SCOTTキャンセル処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelCardEScott(OrderModel order)
		{
			if (string.IsNullOrEmpty(order.CardTranId) == false)
			{
				var adp = EScottProcess1DeleteApi.CreateEScottMaster1DeleteApi(order.CardTranId, order.PaymentOrderId);
				var result = adp.ExecRequest();

				if (result.IsSuccess == false)
				{
					return new ReauthActionResult(
						false,
						order.OrderId,
						string.Empty,
						cardTranIdForLog: order.CardTranId,
						apiErrorMessage: LogCreator.CreateErrorMessage(result.ResponseCd, result.ResponseMessage));
				}
			}

			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(
					order.OrderPaymentKbn,
					order.OrderId,
					order.CardTranId,
					GetSendingAmount(order)),
				cardTranIdForLog: order.CardTranId);
		}

		/// <summary>
		/// ベリトランスキャンセル処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelCardVeriTrans(OrderModel order)
		{
			var result = new PaymentVeritransCredit().Cancel(order.PaymentOrderId);

			if (result.Mstatus != VeriTransConst.RESULT_STATUS_OK)
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(result.VResultCode, result.MerrMsg));
			}

			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(
					order.OrderPaymentKbn,
					order.PaymentOrderId,
					order.CardTranId,
					GetSendingAmount(order)),
				cardTranIdForLog: order.CardTranId);
		}

		/// <summary>
		/// Rakutenキャンセル処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelCardRakuten(OrderModel order)
		{
			if (string.IsNullOrEmpty(order.CardTranId) == false)
			{
				var rakutenAuthourizeRequest = new RakutenCancelOrRefundRequest
				{
					PaymentId = order.PaymentOrderId
				};

				var response = RakutenApiFacade.CancelOrRefund(rakutenAuthourizeRequest);
				if (response.ResultType == RakutenConstants.RESULT_TYPE_FAILURE)
				{
					return new ReauthActionResult(
						false,
						order.OrderId,
						string.Empty,
						cardTranIdForLog: order.CardTranId,
						apiErrorMessage: LogCreator.CreateErrorMessage(response.ErrorCode, response.ErrorMessage));
				}
			}

			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(
					order.OrderPaymentKbn,
					order.PaymentOrderId,
					order.CardTranId,
					GetSendingAmount(order)),
				cardTranIdForLog: order.CardTranId);
		}

		/// <summary>
		/// ペイジェントキャンセル処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelCardPaygent(OrderModel order)
		{
			// キャンセル実施
			var cancelResult = PaygentUtility.DoCancel(order);
			// キャンセル失敗時の返却値
			if (cancelResult.Item1 == false)
			{
				var resultString = (cancelResult.Item2 + ",").Split(',');
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(resultString[0], resultString[1]));
			}
			// キャンセル成功時の返却値
			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(
					order.OrderPaymentKbn,
					order.PaymentOrderId,
					order.CardTranId,
					GetSendingAmount(order)),
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
