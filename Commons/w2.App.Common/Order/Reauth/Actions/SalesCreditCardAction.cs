/*
=========================================================================================================
  Module      : 再与信アクション（クレジットカード売上確定）クラス(SalesCreditCardAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Net;
using jp.veritrans.tercerog.mdk.dto;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.EScott;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.Zcom.Sales;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment.Rakuten;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Payment.Zeus;
using w2.Common.Helper;
using w2.Common.Logger;
using w2.Domain.Order;
using w2.Domain.UserCreditCard;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（クレジットカード売上確定）クラス
	/// </summary>
	public class SalesCreditCardAction : BaseReauthAction<SalesCreditCardAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SalesCreditCardAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Sales, "クレジットカード売上確定", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// クレジットカード売上確定
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			ReauthActionResult result = null;
			var order = reauthActionParams.Order;
			var userCreditCard = new UserCreditCard(new UserCreditCardService().Get(order.UserId, order.CreditBranchNo.Value));

			// 最終請求金額が0円の場合、エラーとする
			if (order.LastBilledAmount == 0)
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					apiErrorMessage: "最終請求金額が0円のため、売上確定できません。");
			}

			switch (Constants.PAYMENT_CARD_KBN)
			{
				// カード・ゼウス決済処理
				case Constants.PaymentCard.Zeus:
					result = ExecSalesCardZeus(order, userCreditCard);
					break;

				// カード・GMO決済処理
				case Constants.PaymentCard.Gmo:
					result = ExecSalesCardGmo(order, userCreditCard);
					break;

				// カード・SBPSクレジット決済処理
				case Constants.PaymentCard.SBPS:
					result = ExecSalesCardSbps(order, userCreditCard);
					break;

				// カード・Zcom決済処理
				case Constants.PaymentCard.Zcom:
					result = ExecSalesCardZcom(order, userCreditCard);
					break;
					
				// カード・e-SCOTT決済処理
				case Constants.PaymentCard.EScott:
					result = ExecSalesCardEScott(order, userCreditCard);
					break;

				// カード・ベリトランス決済処理
				case Constants.PaymentCard.VeriTrans:
					result = ExecSalesCardVeriTrans(order, userCreditCard);
					break;

				// Sale confirm rakuten
				case Constants.PaymentCard.Rakuten:
					result = ExecSalesCardRakuten(order);
					break;

				case Constants.PaymentCard.Paygent:
					result = ExecSalesCardPaygent(order);
					break;

				// その他
				default:
					result = new ReauthActionResult(
						false,
						order.OrderId,
						string.Empty,
						apiErrorMessage: "該当のクレジットカードは再与信売上確定未対応です。");
					break;
			}

			// 決済カード取引IDをセット
			if (result.Result)
			{
				order.CardTranId = result.CardTranId;
				order.PaymentOrderId = result.PaymentOrderId;
			}

			return result;
		}

		/// <summary>
		/// ゼウスクレジット売上確定処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="userCreditCard">クレジットカード情報</param>
		/// <returns>売上確定結果情報</returns>
		private ReauthActionResult ExecSalesCardZeus(OrderModel order, UserCreditCard userCreditCard)
		{
			var zeusPaymentMethod = (Constants.DIGITAL_CONTENTS_OPTION_ENABLED && (order.DigitalContentsFlg == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON)) ? Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD_FORDIGITALCONTENTS : Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD;
			string errMessage = null;
			if (zeusPaymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_WITH_AUTH)
			{
				var result = new ZeusCoolingOffBatchApi().Exec(
					GetSendingAmount(order),
					DateTime.Now.ToString("yyyyMMdd"),
					order.CardTranId);

				if (result.Success)
				{
					return new ReauthActionResult(
						true,
						order.OrderId,
						CreatePaymentMemo(order.OrderPaymentKbn, order.OrderId, order.CardTranId, GetSendingAmount(order)),
						cardTranId: order.CardTranId,
						cardTranIdForLog: order.CardTranId);
				}
				errMessage = result.ErrorMessage;
			}
			else if (string.IsNullOrEmpty(order.CardTranId) && zeusPaymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH)
			{
				try
				{
					var result = new ZeusSecureLinkBatchApi().Exec(
						GetSendingAmount(order),
						userCreditCard.CooperationInfo.ZeusTelNo,
						(order.Owner.OwnerMailAddr != string.Empty) ? order.Owner.OwnerMailAddr : order.Owner.OwnerMailAddr2,
						userCreditCard.CooperationInfo.ZeusSendId,
						order.CardInstallmentsCode);

					if (result.Success)
					{
						return new ReauthActionResult(
							true,
							order.OrderId,
							CreatePaymentMemo(order.OrderPaymentKbn, order.OrderId, result.ZeusOrderId, GetSendingAmount(order)),
							cardTranId: result.ZeusOrderId,
							cardTranIdForLog: result.ZeusOrderId);
					}
					errMessage = result.ErrorMessage;
				}
				catch (WebException webEx)
				{
					AppLogger.WriteError("カード実売上処理失敗エラー" + "[受注ID : " + order.OrderId + "]", webEx);
					// ログ格納処理
					PaymentFileLogger.WritePaymentLog(
						false,
						Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
						PaymentFileLogger.PaymentType.Zeus,
						PaymentFileLogger.PaymentProcessingType.SalesConfirmation,
						errMessage,
						new Dictionary<string, string>
						{
							{Constants.FIELD_ORDER_ORDER_ID, order.OrderId},
							{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, order.PaymentOrderId},
							{Constants.FIELD_ORDER_CARD_TRAN_ID, order.CardTranId}
						});
				}
			}

			// エラー結果返す
			return new ReauthActionResult(
				false,
				order.OrderId,
				string.Empty,
				cardTranIdForLog: order.CardTranId,
				apiErrorMessage: string.IsNullOrEmpty(errMessage)
					? PaymentFileLogger.PaymentProcessingType.CardRealSalesProcessingFailureError.ToText()
					: errMessage);
		}

		/// <summary>
		/// GMOクレジット売上確定処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="userCreditCard">クレジットカード情報</param>
		/// <returns>売上確定結果情報</returns>
		private ReauthActionResult ExecSalesCardGmo(OrderModel order, UserCreditCard userCreditCard)
		{
			var apiErrorMessage = "";
			// 仮売り⇒本売りの場合
			if (Constants.PAYMENT_SETTING_GMO_PAYMENTMETHOD == Constants.GmoCreditCardPaymentMethod.Auth)
			{
				try
				{
					var paymentGMO = new PaymentGmoCredit();
					// 仮売上⇒実売上
					if (paymentGMO.Sales(
						order.PaymentOrderId,
						order.CardTranId,
						GetSendingAmount(order)))
					{
						return new ReauthActionResult(
							true,
							order.OrderId,
							CreatePaymentMemo(order.OrderPaymentKbn, order.PaymentOrderId, order.CardTranId, GetSendingAmount(order)),
							cardTranId: order.CardTranId,
							cardTranIdForLog: order.CardTranId);
					}

					apiErrorMessage = paymentGMO.ErrorMessages;
				}
				catch (WebException webEx)
				{
					AppLogger.WriteError("カード実売上処理失敗エラー" + "[受注ID : " + order.OrderId + "]", webEx);
					// ログ格納処理
					PaymentFileLogger.WritePaymentLog(
						false,
						Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
						PaymentFileLogger.PaymentType.Gmo,
						PaymentFileLogger.PaymentProcessingType.CreditSalesSettlementProcessing,
						BaseLogger.CreateExceptionMessage(webEx),
						new Dictionary<string, string>
						{
							{ Constants.FIELD_ORDER_ORDER_ID, order.OrderId },
							{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, order.PaymentOrderId },
							{ Constants.FIELD_ORDER_CARD_TRAN_ID, order.CardTranId }
						});
				}
			}

			// エラー結果返す
			return new ReauthActionResult(
				false,
				order.OrderId,
				string.Empty,
				cardTranIdForLog: order.CardTranId,
				apiErrorMessage: string.IsNullOrEmpty(apiErrorMessage)
					? PaymentFileLogger.PaymentProcessingType.CardRealSalesProcessingFailureError.ToString()
					: apiErrorMessage);
		}

		/// <summary>
		/// SBPSクレジット売上確定処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="userCreditCard">クレジットカード情報</param>
		/// <returns>売上確定結果情報</returns>
		private ReauthActionResult ExecSalesCardSbps(OrderModel order, UserCreditCard userCreditCard)
		{
			// 「指定日売」のとき
			var sbpsPaymentMethod = (Constants.DIGITAL_CONTENTS_OPTION_ENABLED && (order.DigitalContentsFlg == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON)) ? Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS : Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD;
			string errMessage = null;
			try
			{
				if (sbpsPaymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_WITH_AUTH)
				{
					var saleApi = new PaymentSBPSCreditSaleApi();
					if (saleApi.Exec(order.CardTranId, GetSendingAmount(order)))
					{
						return new ReauthActionResult(
							true,
							order.OrderId,
							CreatePaymentMemo(order.OrderPaymentKbn, order.PaymentOrderId, order.CardTranId, GetSendingAmount(order)),
							cardTranId: order.CardTranId,
							cardTranIdForLog: order.CardTranId);
					}
					errMessage = LogCreator.CreateErrorMessage(saleApi.ResponseData.ResErrCode, saleApi.ResponseData.ResErrMessages);
				}
				// 与信後決済の場合(SBSP「顧客参照」＆「リアル与信」で即売)
				else if (sbpsPaymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH)
				{
					// 決済取引IDが空かどうかをthis.OrderMasterは参照せず改めて取得して確認
					if (order.CardTranId == string.Empty)
					{
						// 決済取引IDに仮IDを格納
						order.CardTranId = Constants.FLG_REALSALES_TEMP_TRAN_ID;

						// 仮決済取引IDが入っているかをthis.OrderMasterは参照せず改めて取得して確認
						// 決済注文ID生成
						var paymentOrderId = OrderCommon.CreatePaymentOrderId(order.ShopId);

						// SBPSクレジット「リアル与信」実行
						var authApi = new PaymentSBPSCreditAuthApi();
						var authResult = authApi.Exec(
							userCreditCard.CooperationInfo.SBPSCustCode,
							paymentOrderId,
							Constants.PAYMENT_SETTING_SBPS_ITEM_ID,
							Constants.PAYMENT_SETTING_SBPS_ITEM_NAME,
							new List<PaymentSBPSBase.ProductItem>(),
							GetSendingAmount(order),
							PaymentSBPSUtil.GetCreditDivideInfo(order.CardInstallmentsCode));

						if (authResult)
						{
							// SBPSクレジット「コミット」処理
							var commitApi = new PaymentSBPSCreditCommitApi();
							if (commitApi.Exec(authApi.ResponseData.ResTrackingId))
							{
								return new ReauthActionResult(
									true,
									order.OrderId,
									CreatePaymentMemo(order.OrderPaymentKbn, order.PaymentOrderId, authApi.ResponseData.ResTrackingId, GetSendingAmount(order)),
									paymentOrderId: paymentOrderId,
									cardTranId: authApi.ResponseData.ResTrackingId,
									cardTranIdForLog: authApi.ResponseData.ResTrackingId);
							}
							errMessage = LogCreator.CreateErrorMessage(commitApi.ResponseData.ResErrCode, commitApi.ResponseData.ResErrMessages);
						}
					}
					else
					{
						errMessage = "売上確定済みの受注データです。ステータスは更新されていません。";
					}
				}
			}
			catch (WebException webEx)
			{
				AppLogger.WriteError("カード実売上処理失敗エラー" + "[受注ID : " + order.OrderId + "]", webEx);

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentFileLogger.PaymentType.Sbps,
					PaymentFileLogger.PaymentProcessingType.CreditSalesSettlementProcessing,
					BaseLogger.CreateExceptionMessage(webEx),
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_ORDER_ID, order.OrderId},
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, order.PaymentOrderId},
						{Constants.FIELD_ORDER_CARD_TRAN_ID, order.CardTranId}
					});
			}

			// エラー結果返す
			return new ReauthActionResult(
				false,
				order.OrderId,
				string.Empty,
				cardTranIdForLog: order.CardTranId,
				apiErrorMessage: string.IsNullOrEmpty(errMessage)
					? PaymentFileLogger.PaymentProcessingType.CardRealSalesProcessingFailureError.ToText()
					: errMessage);
		}

		/// <summary>
		/// Zcom実売上処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="userCreditCard">クレジットカード情報</param>
		/// <returns>売上確定結果情報</returns>
		private ReauthActionResult ExecSalesCardZcom(OrderModel order, UserCreditCard userCreditCard)
		{
			var errMessage = "";
			var adp = new ZcomSalesRequestAdapter(order.PaymentOrderId);
			try
			{
				var result = adp.Execute();
				if (result.IsSuccessResult())
				{
					return new ReauthActionResult(
						true,
						order.OrderId,
						CreatePaymentMemo(
							order.OrderPaymentKbn,
							order.PaymentOrderId,
							order.CardTranId,
							GetSendingAmount(order)),
						cardTranId: order.CardTranId,
						cardTranIdForLog: order.CardTranId);
				}

				errMessage = LogCreator.CreateErrorMessage(result.GetErrorCodeValue(), result.GetErrorDetailValue());
			}
			catch (WebException webEx)
			{
				AppLogger.WriteError("カード実売上処理失敗エラー" + "[受注ID : " + order.OrderId + "]", webEx);
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentFileLogger.PaymentType.Zcom,
					PaymentFileLogger.PaymentProcessingType.RealSalesProcessing,
					BaseLogger.CreateExceptionMessage(webEx),
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_ORDER_ID,order.OrderId},
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID,order.PaymentOrderId},
						{Constants.FIELD_ORDER_CARD_TRAN_ID,order.CardTranId}
					});
			}

			// エラー結果返す
			return new ReauthActionResult(
				false,
				order.OrderId,
				string.Empty,
				cardTranIdForLog: order.CardTranId,
				apiErrorMessage: PaymentFileLogger.PaymentProcessingType.CardRealSalesProcessingFailureError
				+ ":\t" + errMessage);
		}
		
		/// <summary>
		/// e-SCOTT実売上処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="userCreditCard">クレジットカード情報</param>
		/// <returns>売上確定結果情報</returns>
		private ReauthActionResult ExecSalesCardEScott(OrderModel order, UserCreditCard userCreditCard)
		{
			var errMessage = string.Empty;
			var escottPaymentMethod =
				(Constants.DIGITAL_CONTENTS_OPTION_ENABLED &&
					(order.DigitalContentsFlg == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON))
					? Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_PAYMENTMETHOD_FORDIGITALCONTENTS
					: Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_PAYMENTMETHOD;

			try
			{
				if (escottPaymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_WITH_AUTH)
				{
					var adp = EScottProcess1CaptureApi.CreateEScottProcess1CaptureApi(
						order.CardTranId,
						order.PaymentOrderId,
						DateTime.Now);
					var result = adp.ExecRequest();
					if (result.IsSuccess)
					{
						return new ReauthActionResult(
							true,
							order.OrderId,
							CreatePaymentMemo(
								order.OrderPaymentKbn,
								order.PaymentOrderId,
								order.CardTranId,
								GetSendingAmount(order)),
							cardTranId: order.CardTranId,
							cardTranIdForLog: order.CardTranId);
					}

					errMessage = LogCreator.CreateErrorMessage(result.ResponseCd, result.ResponseMessage);
				}
				// 与信後決済の場合
				else if (escottPaymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH)
				{
					// 決済取引IDが空かどうかをthis.OrderMasterは参照せず改めて取得して確認
					if (order.CardTranId == string.Empty)
					{
						var adp = EScottMaster1GatheringApi.CreateEScottMaster1GatheringApi(
							order.PaymentOrderId,
							order.LastBilledAmount,
							order.CardInstallmentsCode,
							userCreditCard,
							DateTime.Now);
						var result = adp.ExecRequest();
						if (result.IsSuccess)
						{
							return new ReauthActionResult(
								true,
								order.OrderId,
								CreatePaymentMemo(
									order.OrderPaymentKbn,
									order.PaymentOrderId,
									result.CardTranId,
									GetSendingAmount(order)),
								cardTranId: order.CardTranId,
								cardTranIdForLog: order.CardTranId);
						}

						errMessage = LogCreator.CreateErrorMessage(result.ResponseCd, result.ResponseMessage);
					}
					else
					{
						errMessage = "売上確定済みの受注データです。ステータスは更新されていません。";
					}
				}
			}
			catch (WebException webEx)
			{
				AppLogger.WriteError("カード実売上処理失敗エラー" + "[受注ID : " + order.OrderId + "]", webEx);
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentFileLogger.PaymentType.EScott,
					PaymentFileLogger.PaymentProcessingType.RealSalesProcessing,
					BaseLogger.CreateExceptionMessage(webEx),
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, order.OrderId },
						{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, order.PaymentOrderId },
						{ Constants.FIELD_ORDER_CARD_TRAN_ID, order.CardTranId }
					});
			}

			// エラー結果返す
			return new ReauthActionResult(
				false,
				order.OrderId,
				string.Empty,
				cardTranIdForLog: order.CardTranId,
				apiErrorMessage: PaymentFileLogger.PaymentProcessingType.CardRealSalesProcessingFailureError
				+ ":\t" + errMessage);
		}

		/// <summary>
		/// ベリトランス実売上処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="userCreditCard">クレジットカード情報</param>
		/// <returns>売上確定結果情報</returns>
		private ReauthActionResult ExecSalesCardVeriTrans(OrderModel order, UserCreditCard userCreditCard)
		{
			var paymentMethodVeriTrans =
				(Constants.DIGITAL_CONTENTS_OPTION_ENABLED &&
				 (order.DigitalContentsFlg == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON))
					? Constants.PAYMENT_SETTING_VERITRANS4G_PAYMENTMETHOD_FORDIGITALCONTENTS
					: Constants.PAYMENT_SETTING_VERITRANS4G_PAYMENTMETHOD;

			string errMessage;
			if (paymentMethodVeriTrans == Constants.VeritransCreditCardPaymentMethod.Auth)
			{
				var responseVeritrans = new PaymentVeritransCredit().Capture(
					order.PaymentOrderId,
					GetSendingAmount(order).ToPriceString());

				if (responseVeritrans.Mstatus == VeriTransConst.RESULT_STATUS_OK)
				{
					return new ReauthActionResult(
						true,
						order.OrderId,
						CreatePaymentMemo(
							order.OrderPaymentKbn,
							order.PaymentOrderId,
							order.CardTranId,
							GetSendingAmount(order)),
						cardTranId: order.CardTranId,
						cardTranIdForLog: order.CardTranId);
				}

				errMessage = PaymentFileLogger.PaymentProcessingType.CardRealSalesProcessingFailureError
					+ ":\t" + responseVeritrans.MerrMsg;
			}
			else
			{
				errMessage = "売上確定済みの受注データです。ステータスは更新されていません。";
			}

			return new ReauthActionResult(
				false,
				order.OrderId,
				string.Empty,
				cardTranIdForLog: order.CardTranId,
				apiErrorMessage: errMessage);
		}

		/// <summary>
		/// Exec sale confirm rakuten card
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>売上確定結果情報</returns>
		private ReauthActionResult ExecSalesCardRakuten(OrderModel order)
		{
			var errMessage = string.Empty;
			if (Constants.PAYMENT_RAKUTEN_CREDIT_PAYMENT_METHOD == Constants.RakutenPaymentType.AUTH)
			{
				var rakutenApiExecResult = RakutenApiFacade.Capture(new RakutenCaptureRequest
				{
					PaymentId = order.PaymentOrderId
				});

				if (rakutenApiExecResult.ResultType == RakutenConstants.RESULT_TYPE_SUCCESS)
				{
					return new ReauthActionResult(
						true,
						order.OrderId,
						CreatePaymentMemo(
							order.OrderPaymentKbn,
							order.PaymentOrderId,
							order.CardTranId,
							GetSendingAmount(order)),
						cardTranId: order.CardTranId,
						cardTranIdForLog: order.CardTranId);
				}

				errMessage = PaymentFileLogger.PaymentProcessingType.CardRealSalesProcessingFailureError
					+ ":\t" + rakutenApiExecResult.ErrorMessage;
			}
			else
			{
				errMessage = "売上確定済みの受注データです。ステータスは更新されていません。";
			}

			return new ReauthActionResult(
				false,
				order.OrderId,
				string.Empty,
				cardTranIdForLog: order.CardTranId,
				apiErrorMessage: errMessage);
		}

		/// <summary>
		/// ペイジェント売上確定
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>売上確定結果情報</returns>
		private ReauthActionResult ExecSalesCardPaygent(OrderModel order)
		{
			var realSaleParams = new PaygentApiHeader(PaygentConstants.PAYGENT_APITYPE_CARD_REALSALE);
			realSaleParams.PaymentId = order.CardTranId;
			var result = PaygentApiFacade.SendRequest(realSaleParams);
			if ((string)result[PaygentConstants.RESPONSE_STATUS] == PaygentConstants.PAYGENT_RESPONSE_STATUS_SUCCESS)
			{
				return new ReauthActionResult(
					true,
					order.OrderId,
					CreatePaymentMemo(
						order.OrderPaymentKbn,
						order.PaymentOrderId,
						order.CardTranId,
						GetSendingAmount(order)),
					cardTranId: order.CardTranId,
					cardTranIdForLog: order.CardTranId);
			}
			else
			{
				return new ReauthActionResult(
				false,
				order.OrderId,
				string.Empty,
				cardTranIdForLog: order.CardTranId,
				apiErrorMessage: "ペイジェントクレカ売確失敗");
			}
		}


		/// <summary>
		/// 決済注文ID、取引ID更新
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="cardTranId">取引ID</param>
		public override void UpdateReauthInfo(string paymentOrderId, string cardTranId)
		{
			((ReauthActionParams)base.ReauthActionParams).Order.PaymentOrderId = paymentOrderId;
			((ReauthActionParams)base.ReauthActionParams).Order.CardTranId = cardTranId;
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
