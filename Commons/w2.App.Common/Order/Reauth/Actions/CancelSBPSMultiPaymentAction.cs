/*
=========================================================================================================
  Module      : 再与信アクション（SBPSのキャリア決済・リクルートかんたん・PayPal・楽天ペイ）キャンセルクラス(CancelSBPSMultiPaymentAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Order.Payment;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（SBPSのキャリア決済・リクルートかんたん・PayPal・楽天ペイ キャンセル）クラス
	/// </summary>
	public class CancelSBPSMultiPaymentAction : BaseReauthAction<CancelSBPSMultiPaymentAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CancelSBPSMultiPaymentAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Cancel, "キャリア決済キャンセル", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// SBPSのキャリア決済・リクルートかんたん・PayPal・楽天ペイキャンセル
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var order = reauthActionParams.Order;
			bool result = false;
			string errMessage = string.Empty;
			switch (order.OrderPaymentKbn)
			{
				// ソフトバンク・ワイモバイルまとめて支払い(SBPS)
				case Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS:
					var sbCancelApi = new PaymentSBPSCareerSoftbankKetaiCancelApi();
					result = sbCancelApi.Exec(order.CardTranId);
					errMessage = LogCreator.CreateErrorMessage(sbCancelApi.ResponseData.ResErrCode, sbCancelApi.ResponseData.ResErrMessages);
					break;

				// ドコモケータイ払い(SBPS)
				case Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS:
					var docomoCancelApi = new PaymentSBPSCareerDocomoKetaiCancelApi();
					result = docomoCancelApi.Exec(order.CardTranId);
					errMessage = LogCreator.CreateErrorMessage(docomoCancelApi.ResponseData.ResErrCode, docomoCancelApi.ResponseData.ResErrMessages);
					break;

				// auかんたん決済(SBPS)
				case Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS:
					var auCancelApi = new PaymentSBPSCareerAuKantanCancelApi();
					result = auCancelApi.Exec(order.CardTranId);
					errMessage = LogCreator.CreateErrorMessage(auCancelApi.ResponseData.ResErrCode, auCancelApi.ResponseData.ResErrMessages);
					break;

				// リクルートかんたん支払い(SBPS)
				case Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS:
					var recruitCancelApi = new PaymentSBPSRecruitCancelApi();
					result = recruitCancelApi.Exec(order.CardTranId);
					errMessage = LogCreator.CreateErrorMessage(recruitCancelApi.ResponseData.ResErrCode, recruitCancelApi.ResponseData.ResErrMessages);
					break;

				// PayPal(SBPS)
				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS:
					var paypalCancelApi = new PaymentSBPSPaypalCancelApi();
					result = paypalCancelApi.Exec(order.CardTranId);
					errMessage = LogCreator.CreateErrorMessage(paypalCancelApi.ResponseData.ResErrCode, paypalCancelApi.ResponseData.ResErrMessages);
					break;

				// 楽天ペイ(SBPS)
				case Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS:
					var rakutenIdApi = new PaymentSBPSRakutenIdCancelApi();
					result = rakutenIdApi.Exec(order.CardTranId);
					errMessage = LogCreator.CreateErrorMessage(rakutenIdApi.ResponseData.ResErrCode, rakutenIdApi.ResponseData.ResErrMessages);
					break;

				// PayPay(SBPS)
				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY:
					var paypayCancelApi = new PaymentSBPSPaypayCancelApi();
					result = paypayCancelApi.Exec(order.CardTranId, order.OrderPriceTotal);
					errMessage = LogCreator.CreateErrorMessage(paypayCancelApi.ResponseData.ResErrCode, paypayCancelApi.ResponseData.ResErrMessages);
					break;

				// その他
				default:
					errMessage = "該当の決済は与信の取消し連動をサポートしていません。";
					break;
			}

			if (result == false)
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: errMessage);
			}

			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(order.OrderPaymentKbn, order.OrderId, order.CardTranId, null),
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