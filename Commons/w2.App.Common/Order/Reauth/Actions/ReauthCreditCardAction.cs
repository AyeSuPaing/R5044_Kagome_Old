/*
=========================================================================================================
  Module      : 再与信アクション（クレジットカード与信）クラス(ReauthCreditCardAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Web;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Payment.GMO.Zcom.Direct;
using w2.App.Common.Order.Payment.YamatoKwc;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;
using w2.App.Common.Order.Payment.Zeus;
using w2.App.Common.Order.Payment.EScott;
using w2.App.Common.Order.Payment.Rakuten;
using w2.Common.Sql;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserCreditCard;
using w2.App.Common.Order.Payment.Rakuten.Authorize;
using w2.App.Common.Order.Payment.Paygent;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（クレジットカード与信）クラス
	/// </summary>
	public class ReauthCreditCardAction : BaseReauthAction<ReauthCreditCardAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ReauthCreditCardAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Reauth, "クレジットカード決済与信", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// クレジットカード与信
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			ReauthActionResult result = null;
			var order = reauthActionParams.Order;
			var userCreditCard = new UserCreditCard(new UserCreditCardService().Get(order.UserId, order.CreditBranchNo.Value));

			// クレジット与信が0円で通ってしまうため、
			// 最終請求金額が0円の場合、エラーとする
			if ((GetSendingAmount(order) == 0) && (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten))
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					apiErrorMessage: "最終請求金額が0円のため、与信できません。");
			}

			switch (Constants.PAYMENT_CARD_KBN)
			{
				// カード・ゼウス決済処理
				case Constants.PaymentCard.Zeus:
					result = ExecAuthCardZeus(order, userCreditCard);
					break;

				// カード・SBPSクレジット決済処理
				case Constants.PaymentCard.SBPS:
					result = ExecAuthCardSbps(order, userCreditCard);
					break;

				// カード・ヤマトKWCクレジット決済処理
				case Constants.PaymentCard.YamatoKwc:
					result = ExecAuthCardYamatoKwc(order, userCreditCard);
					break;

				// カード・GMO決済処理
				case Constants.PaymentCard.Gmo:
					result = ExecAuthCardGmo(order, userCreditCard);
					break;

				// Zcom
				case Constants.PaymentCard.Zcom:
					result = ExecAuthCardZcom(order, userCreditCard);
					break;
				
				// e-SCOTT
				case Constants.PaymentCard.EScott:
					result = ExecAuthCardEScott(order, userCreditCard);
					break;

				// ベリトランス
				case Constants.PaymentCard.VeriTrans:
					result = ExecAuthCardVeriTrans(order, userCreditCard, reauthActionParams.OldPaymentKbn);
					break;
					
				// Rakuten
				case Constants.PaymentCard.Rakuten:
					result = ExecAuthCardRakuten(order, userCreditCard, reauthActionParams.IsExecModify);
					break;

				// ペイジェント
				case Constants.PaymentCard.Paygent:
					result = ExecAuthCardPaygent(order, userCreditCard, reauthActionParams.OldPaymentKbn);
					break;

				// その他
				default:
					result = new ReauthActionResult(
						false,
						order.OrderId,
						string.Empty,
						apiErrorMessage: "該当のクレジットカードは再与信をサポートしていません。");
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
		/// ベリトランスクレカ決済
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="userCreditCard">カード情報</param>
		/// <param name="oldPaymentKbn">変更前の決済情報</param>
		/// <returns>与信結果情報</returns>
		private ReauthActionResult ExecAuthCardVeriTrans(OrderModel order, UserCreditCard userCreditCard, string oldPaymentKbn)
		{
			var paymentVeritransCredit = new PaymentVeritransCredit();
			var newPaymentOrderId = OrderCommon.CreatePaymentOrderId(order.ShopId);

			// 変更前の支払い区分がクレジットカード以外場合
			// 登録クレジット：ワンクリック継続課金 新規クレジット：与信処理
			if (oldPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			{
				var cart = CartObject.CreateCartByOrder(order);
				cart.Payment.UserCreditCard = userCreditCard;
				var htOrder = order.DataSource;
				htOrder[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = newPaymentOrderId;

				// 返品後にカートオブジェクトを作成すると金額がずれる為、与信金額は別で計算する
				// 再与信・売上確定処理と同様の計算方法を利用
				var amount = GetSendingAmount(order).ToPriceString();

				if ((cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
					&& Constants.PAYMENT_VERITRANS4G_CREDIT_3DSECURE)
				{
					var paymentVeritrans3ds = new PaymentVeritransCredit3DS();
					var auth3dsResult = paymentVeritrans3ds.Auth(htOrder, cart, amount);
					if (auth3dsResult.Mstatus != VeriTransConst.RESULT_STATUS_OK)
					{
						return new ReauthActionResult(
							false,
							order.OrderId,
							string.Empty,
							cardTranIdForLog: order.CardTranId,
							paymentOrderId: order.PaymentOrderId,
							apiErrorMessage: auth3dsResult.MerrMsg);
					}

					var auth3dsResultPaymentMemo = CreatePaymentMemo(
						order.OrderPaymentKbn,
						newPaymentOrderId,
						auth3dsResult.CustTxn,
						GetSendingAmount(order));

					return new ReauthActionResult(
						true,
						order.OrderId,
						auth3dsResultPaymentMemo,
						cardTranId: auth3dsResult.CustTxn,
						paymentOrderId: newPaymentOrderId,
						cardTranIdForLog: auth3dsResult.CustTxn);
				}
				if (cart.Payment.CreditCardBranchNo != CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
				{
					var cardInfo = paymentVeritransCredit.GetCardInfo(cart.Payment.UserCreditCard.CooperationId);
					if (cardInfo.Mstatus != VeriTransConst.RESULT_STATUS_OK)
					{
						return new ReauthActionResult(
							false,
							order.OrderId,
							string.Empty,
							cardTranIdForLog: order.CardTranId,
							paymentOrderId: order.PaymentOrderId,
							apiErrorMessage: cardInfo.MerrMsg);
					}

					var usePayNowIdResult = paymentVeritransCredit.UsePayNowId(htOrder, cart, amount);
					if (usePayNowIdResult.Mstatus != VeriTransConst.RESULT_STATUS_OK)
					{
						return new ReauthActionResult(
							false,
							order.OrderId,
							string.Empty,
							cardTranIdForLog: order.CardTranId,
							paymentOrderId: order.PaymentOrderId,
							apiErrorMessage: usePayNowIdResult.MerrMsg);
					}

					var usePayNowIdResultPaymentMemo = CreatePaymentMemo(
						order.OrderPaymentKbn,
						newPaymentOrderId,
						usePayNowIdResult.CustTxn,
						GetSendingAmount(order));

					return new ReauthActionResult(
						true,
						order.OrderId,
						usePayNowIdResultPaymentMemo,
						cardTranId: usePayNowIdResult.CustTxn,
						paymentOrderId: newPaymentOrderId,
						cardTranIdForLog: usePayNowIdResult.CustTxn);
				}

				var authResult = paymentVeritransCredit.Auth(htOrder, cart, amount);
				if (authResult.Mstatus != VeriTransConst.RESULT_STATUS_OK)
				{
					return new ReauthActionResult(
						false,
						order.OrderId,
						string.Empty,
						cardTranIdForLog: order.CardTranId,
						paymentOrderId: order.PaymentOrderId,
						apiErrorMessage: authResult.MerrMsg);
				}

				var authResultPaymentMemo = CreatePaymentMemo(
					order.OrderPaymentKbn,
					newPaymentOrderId,
					authResult.CustTxn,
					GetSendingAmount(order));

				return new ReauthActionResult(
					true,
					order.OrderId,
					authResultPaymentMemo,
					cardTranId: authResult.CustTxn,
					paymentOrderId: newPaymentOrderId,
					cardTranIdForLog: authResult.CustTxn);
			}

			// 変更前の支払い区分がクレジットカードの場合、再与信
			var reAuthorizeResult = paymentVeritransCredit.ReAuthorize(order, userCreditCard, newPaymentOrderId);
			if (reAuthorizeResult.Mstatus != VeriTransConst.RESULT_STATUS_OK)
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					paymentOrderId: order.PaymentOrderId,
					apiErrorMessage: reAuthorizeResult.MerrMsg);
			}

			var reAuthorizePaymentMemo = CreatePaymentMemo(
				order.OrderPaymentKbn,
				newPaymentOrderId,
				reAuthorizeResult.CustTxn,
				GetSendingAmount(order));
		
			return new ReauthActionResult(
				true,
				order.OrderId,
				reAuthorizePaymentMemo,
				cardTranId: reAuthorizeResult.CustTxn,
				paymentOrderId: newPaymentOrderId,
				cardTranIdForLog: reAuthorizeResult.CustTxn);
		}

		/// <summary>
		/// ゼウスクレジット与信処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="userCreditCard">クレジットカード情報</param>
		/// <returns>与信結果情報</returns>
		private ReauthActionResult ExecAuthCardZeus(OrderModel order, UserCreditCard userCreditCard)
		{
			var zeusPaymentMethod = (Constants.DIGITAL_CONTENTS_OPTION_ENABLED
				&& (order.DigitalContentsFlg == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON))
					? Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD_FORDIGITALCONTENTS
					: Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD;
			var result = new ZeusSecureLinkApi().Exec(
					null,	// トークン無い場合は既存カード利用
					(zeusPaymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH) ? 0 : GetSendingAmount(order),
					userCreditCard.CooperationInfo.ZeusTelNo,
					(order.Owner.OwnerMailAddr != string.Empty) ? order.Owner.OwnerMailAddr : order.Owner.OwnerMailAddr2,
					userCreditCard.CooperationInfo.ZeusSendId,
					order.CardInstallmentsCode);
			if (result.Success)
			{
				// 与信後決済以外は決済連携メモ作成
				var paymentMemo = (zeusPaymentMethod != Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH)
					? CreatePaymentMemo(order.OrderPaymentKbn, order.OrderId, result.ZeusOrderId, GetSendingAmount(order))
					: "";
				return new ReauthActionResult(
						true,
						order.OrderId,
						paymentMemo,
						cardTranId: (zeusPaymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH) ? string.Empty : result.ZeusOrderId,
						cardTranIdForLog: result.ZeusOrderId);
			}
			return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: result.ErrorMessage.Replace("\n","\t"));
		}

		/// <summary>
		/// GMOクレジット決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="userCreditCard">クレジットカード情報</param>
		/// <returns>与信結果情報</returns>
		private ReauthActionResult ExecAuthCardGmo(OrderModel order, UserCreditCard userCreditCard)
		{
			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(order.ShopId);
			bool result = false;
			var cardTranId = string.Empty;
			var errorMsg = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR);
			var apiErrorMessage = "";

			//------------------------------------------------------
			// 取引登録
			//------------------------------------------------------
			PaymentGmoCredit paymentGMO = new PaymentGmoCredit();

			var entryTranResult = paymentGMO.EntryTran(paymentOrderId, GetSendingAmount(order));
			if (entryTranResult)
			{
				cardTranId = paymentGMO.AccessId + " " + paymentGMO.AccessPass;
				// 登録済みカード検索
				if (paymentGMO.SearchCard(userCreditCard.CooperationInfo.GMOMemberId))
				{
					// 決済実行（登録カード利用）
					result = paymentGMO.ExecTranUseCard(
						paymentOrderId,
						cardTranId,
						userCreditCard.CooperationInfo.GMOMemberId,
						order.CardInstallmentsCode,
						string.Empty);

					if (result == false) apiErrorMessage = paymentGMO.ErrorMessages;
				}
				else
				{
					errorMsg = "登録済みカード情報がありません。";
					apiErrorMessage = paymentGMO.ErrorMessages;
				}
			}
			else
			{
				errorMsg = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_GMO_CARDAUTH_ERROR_ENTRYTRAN);
				apiErrorMessage = paymentGMO.ErrorMessages;
			}

			if (result)
			{
				return new ReauthActionResult(
					true,
					order.OrderId,
					CreatePaymentMemo(order.OrderPaymentKbn, paymentOrderId, cardTranId, GetSendingAmount(order)),
					cardTranId: cardTranId,
					paymentOrderId: paymentOrderId,
					cardTranIdForLog: cardTranId);
			}
			else
			{
				return new ReauthActionResult(
						false,
						order.OrderId,
						string.Empty,
						cardTranIdForLog: order.CardTranId,
						apiErrorMessage: apiErrorMessage);
			}
		}

		/// <summary>
		/// Zcomクレカ決済
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="userCreditCard">カード情報</param>
		/// <returns>与信結果情報</returns>
		private ReauthActionResult ExecAuthCardZcom(OrderModel order, UserCreditCard userCreditCard)
		{
			// 注文IDは再与信を考慮して枝番を付与
			var errorMsg = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR);

			var adp = new ZcomDirectRequestModelAdapter(order, userCreditCard);
			var res = adp.Execute();
			if (res.IsSuccessResult())
			{
				return new ReauthActionResult(
					true,
					order.OrderId,
					CreatePaymentMemo(order.OrderPaymentKbn, adp.PaymentOrderId, res.GetTransCodeValue(), GetSendingAmount(order)),
					cardTranId: res.GetTransCodeValue(),
					paymentOrderId: adp.PaymentOrderId,
					cardTranIdForLog: res.GetTransCodeValue());
			}
			else
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(res.GetErrorCodeValue(), res.GetErrorDetailValue()));
			}
		}

		/// <summary>
		/// SBPSクレジット決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="userCreditCard">クレジットカード情報</param>
		/// <returns>与信結果情報</returns>
		private ReauthActionResult ExecAuthCardSbps(OrderModel order, UserCreditCard userCreditCard)
		{
			bool result = false;
			var paymentOrderId = string.Empty;
			var cardTranId = string.Empty;
			var errorMsg = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR);
			var apiErrorMessage = "";

			// SBPSクレジット決済方法設定を取得
			var paymentMethod = (Constants.DIGITAL_CONTENTS_OPTION_ENABLED && (order.DigitalContentsFlg == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON))
				? Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS
				: Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD;

			// 通常決済？
			if (paymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_WITH_AUTH)
			{
				// 注文IDは再与信を考慮して枝番を付与
				paymentOrderId = OrderCommon.CreatePaymentOrderId(order.ShopId);
				// 登録カード利用
				var api = new PaymentSBPSCreditAuthApi();
				result = api.Exec(
					userCreditCard.CooperationInfo.SBPSCustCode,
					paymentOrderId,
					Constants.PAYMENT_SETTING_SBPS_ITEM_ID,
					Constants.PAYMENT_SETTING_SBPS_ITEM_NAME,
					new List<PaymentSBPSBase.ProductItem>(),
					GetSendingAmount(order),
					PaymentSBPSUtil.GetCreditDivideInfo(order.CardInstallmentsCode));
				cardTranId = api.ResponseData.ResTrackingId;

				if (result == false)
				{
					apiErrorMessage = LogCreator.CreateErrorMessage(
						api.ResponseData.ResErrCode,
						api.ResponseData.ResErrMessages);
				}

				// コミット
				if (result)
				{
					var paymentSbpsCommitApi = new PaymentSBPSCreditCommitApi();
					result = paymentSbpsCommitApi.Exec(cardTranId);
					if (result == false)
					{
						apiErrorMessage += LogCreator.CreateErrorMessage(
							paymentSbpsCommitApi.ResponseData.ResErrCode,
							paymentSbpsCommitApi.ResponseData.ResErrMessages);
					}
				}
			}
			else if (paymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH)
			{
				return new ReauthActionResult(
					true,
					order.OrderId,
					string.Empty,
					cardTranId: string.Empty,
					paymentOrderId: string.Empty,
					cardTranIdForLog: string.Empty);
			}

			if (result)
			{
				return new ReauthActionResult(
					true,
					order.OrderId,
					CreatePaymentMemo(order.OrderPaymentKbn, paymentOrderId, cardTranId, GetSendingAmount(order)),
					cardTranId: cardTranId,
					paymentOrderId: paymentOrderId,
					cardTranIdForLog: cardTranId);
			}
			else
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: apiErrorMessage);
			}
		}

		/// <summary>
		/// e-SCOTTクレカ決済
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="userCreditCard">カード情報</param>
		/// <returns>与信結果情報</returns>
		private ReauthActionResult ExecAuthCardEScott(OrderModel order, UserCreditCard userCreditCard)
		{
			// クレジット決済方法設定を取得
			var paymentMethod = (Constants.DIGITAL_CONTENTS_OPTION_ENABLED && (order.DigitalContentsFlg == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON))
				? Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_PAYMENTMETHOD_FORDIGITALCONTENTS
				: Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_PAYMENTMETHOD;
			if (paymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH)
			{
				return new ReauthActionResult(
					true,
					order.OrderId,
					string.Empty,
					cardTranId: string.Empty,
					paymentOrderId: string.Empty,
					cardTranIdForLog: string.Empty);
			}

			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(order.ShopId);
			var adp = EScottMaster1AuthApi.CreateEScottMaster1AuthApiByTokenForReauth(
				order,
				paymentOrderId,
				userCreditCard);
			var res = adp.ExecRequest();
			if (res.IsSuccess)
			{
				return new ReauthActionResult(
					true,
					order.OrderId,
					CreatePaymentMemo(order.OrderPaymentKbn, order.PaymentOrderId, res.CardTranId, GetSendingAmount(order)),
					cardTranId: res.CardTranId,
					paymentOrderId: paymentOrderId,
					cardTranIdForLog: res.CardTranId);
			}
			else
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(res.ResponseCd, res.ResponseMessage));
			}
		}

		/// <summary>
		/// ヤマトクレジット決済処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="userCreditCard">クレジットカード情報</param>
		/// <returns>与信結果情報</returns>
		private ReauthActionResult ExecAuthCardYamatoKwc(OrderModel order, UserCreditCard userCreditCard)
		{
			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(order.ShopId);
			bool result = false;
			string errorMsg = string.Empty;
			PaymentYamatoKwcCreditAuthResponseData responseData = null;

			// オプションサービスパラメタ取得
			try
			{
				var optionParam = this.GetPaymentYamatoKwcCreditOptionParam(order, userCreditCard);

				// 支払い回数がブランクの場合、デフォルトの支払い回数を設定する
				var cartInstallmentsCode = string.IsNullOrEmpty(order.CardInstallmentsCode)
					? Constants.FIELD_CREDIT_INSTALLMENTS_YAMATOKWC_VALUE
					: order.CardInstallmentsCode;

				// 注文者メールアドレスが空の場合は、ダミーメールアドレスをセット
				var mailAddress
					= (string.IsNullOrEmpty(order.Owner.OwnerMailAddr)
						? Constants.PAYMENT_SETTING_YAMATO_KWC_DUMMY_MAILADDRESS
						: order.Owner.OwnerMailAddr);

				// 与信実行
				responseData =
					new PaymentYamatoKwcCreditAuthApi((optionParam.Token != null)).Exec(
						this.GetYamatoKwcDeviceDiv(order.OrderKbn),
						paymentOrderId,
						Constants.PAYMENT_SETTING_YAMATO_KWC_GOODS_NAME,
						GetSendingAmount(order),
						order.Owner.OwnerName,
						order.Owner.OwnerTel1,
						mailAddress,
						int.Parse(cartInstallmentsCode),
						optionParam);
				result = responseData.Success;
			}
			catch (Exception ex)
			{
				errorMsg = ex.Message;
			}

			if (result)
			{
				return new ReauthActionResult(
					true,
					order.OrderId,
					CreatePaymentMemo(order.OrderPaymentKbn, paymentOrderId, responseData.CrdCResCd, GetSendingAmount(order)),
					cardTranId: responseData.CrdCResCd,
					paymentOrderId: paymentOrderId,
					cardTranIdForLog: responseData.CrdCResCd);
			}
			else
			{
				errorMsg = ((responseData == null) || string.IsNullOrEmpty(responseData.CreditErrorCode))
					? errorMsg
					: string.Format("（{0}:{1}）", responseData.CreditErrorCode, responseData.CreditErrorMessage);
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: errorMsg);
			}
		}

		/// <summary>
		/// Execute Reauthen.Rakuten
		/// </summary>
		/// <param name="order">Order information</param>
		/// <param name="userCreditCard">User Credit card information</param>
		/// <param name="isExecModify">受注編集による実行か</param>
		/// <returns>Result of reauthen</returns>
		private ReauthActionResult ExecAuthCardRakuten(OrderModel order, UserCreditCard userCreditCard, bool isExecModify)
		{
			var paymentOrderId = (string.IsNullOrEmpty(order.OrderIdOrg)
				&& (string.IsNullOrEmpty(order.PaymentOrderId) == false)
				&& (isExecModify))
					? order.PaymentOrderId
					: OrderCommon.CreatePaymentOrderId(order.ShopId);
			var cartInstallmentsCode = string.IsNullOrEmpty(order.CardInstallmentsCode)
				? Constants.FIELD_CREDIT_INSTALLMENTS_RAKUTEN_VALUE
				: order.CardInstallmentsCode;

			var context = HttpContext.Current;
			var ipAddress = (context != null) ? context.Request.ServerVariables["REMOTE_ADDR"] : "";

			// Payment rakuten method
			var rakutenAuthourizeRequest = new RakutenAuthorizeRequest(ipAddress)
			{
				PaymentId = paymentOrderId,
				GrossAmount = GetSendingAmount(order),
				CardToken = new CardTokenBase
				{
					Amount = GetSendingAmount(order).ToString("0"),
					CardToken = userCreditCard.CooperationId,
					WithThreeDSecure = false
				},
			};

			switch (cartInstallmentsCode)
			{
				// 一括
				case RakutenConstants.DealingTypes.ONCE:
					break;

				// リボ払い
				case RakutenConstants.DealingTypes.REVO:
					rakutenAuthourizeRequest.CardToken.WithRevolving = true;
					break;

				// ボーナス一括
				case RakutenConstants.DealingTypes.BONUS1:
					rakutenAuthourizeRequest.CardToken.WithBonus = true;
					break;

				// 支払い回数
				default:
					rakutenAuthourizeRequest.CardToken.Installments = cartInstallmentsCode;
					break;
			}

			// Call api authorize
			var response = RakutenApiFacade.AuthorizeAPI(rakutenAuthourizeRequest);
			var apiErrorMessage = string.Empty;
			var cardTranId = string.Empty;

			// Call api failed
			if ((response.ResultType == RakutenConstants.RESULT_TYPE_FAILURE)
				|| (response.ResultType == RakutenConstants.RESULT_TYPE_PENDING))
			{
				if ((response.ErrorCode == RakutenConstants.ErrorCode.ALREADY_COMPLETED)
					|| (response.ErrorCode == RakutenConstants.ErrorCode.UNACCEPTABLE_REQUEST)
					|| (response.ErrorCode == RakutenConstants.ErrorCode.UNAUTHORIZED_ACCESS))
				{
					// Call api rakuten modify
					var rakutenModifyRequest = new RakutenModifyRequest
					{
						PaymentId = paymentOrderId,
						Amount = GetSendingAmount(order).ToString("0")
					};

					var responseModify = RakutenApiFacade.ModifyPaymentAmount(rakutenModifyRequest);

					// Call api modify failed
					if ((responseModify.ResultType == RakutenConstants.RESULT_TYPE_FAILURE)
						|| (responseModify.ResultType == RakutenConstants.RESULT_TYPE_PENDING))
					{
						apiErrorMessage = LogCreator.CreateErrorMessage(
							responseModify.ErrorCode,
							responseModify.ErrorMessage);
					}

					// When call success modify
					if (responseModify.ResultType == RakutenConstants.RESULT_TYPE_SUCCESS)
					{
						cardTranId = responseModify.AgencyRequestId;
						return new ReauthActionResult(
							true,
							order.OrderId,
							CreatePaymentMemo(order.OrderPaymentKbn, paymentOrderId, cardTranId, GetSendingAmount(order)),
							cardTranId: cardTranId,
							paymentOrderId: paymentOrderId,
							cardTranIdForLog: cardTranId);
					}
					else
					{
						return new ReauthActionResult(
							false,
							order.OrderId,
							string.Empty,
							cardTranIdForLog: order.CardTranId,
							apiErrorMessage: apiErrorMessage);
					}
				}

				// Return failure
				apiErrorMessage = LogCreator.CreateErrorMessage(
					response.ErrorCode,
					response.ErrorMessage);
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: apiErrorMessage);
			}

			// Get tranId and return success
			cardTranId = response.AgencyRequestId;
			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(order.OrderPaymentKbn, paymentOrderId, cardTranId, GetSendingAmount(order)),
				cardTranId: cardTranId,
				paymentOrderId: paymentOrderId,
				cardTranIdForLog: cardTranId);
		}

		/// <summary>
		/// ペイジェント 与信取得
		/// </summary>
		/// <param name="order">受注情報</param>
		/// <param name="userCreditCard">ユーザークレジットカード情報</param>
		/// <param name="oldPaymentKbn">変更前の支払い方法</param>
		/// <returns>結果</returns>
		private ReauthActionResult ExecAuthCardPaygent(OrderModel order, UserCreditCard userCreditCard, string oldPaymentKbn)
		{
			// 新しい金額で与信を取得
			var authParams = new PaygentCreditCardAuthApi();
			authParams.PaymentAmount = Math.Floor(GetSendingAmount(order)).ToString();

			// 支払回数が1回：PaymentClass = 10, SplitCount = ""
			// 支払回数が1回以外：PaymentClass = 61, SplitCount = "paygent_installments"
			// 一括・分割払い以外は対応しない
			if ((string.IsNullOrEmpty(order.CardInstallmentsCode) == false)
				&& (order.CardInstallmentsCode == "1"))
			{
				authParams.PaymentClass = PaygentConstants.PAYGENT_PAYMENT_CLASS_FULL;
				authParams.SplitCount = string.Empty;
			}
			else
			{
				authParams.PaymentClass = PaygentConstants.PAYGENT_PAYMENT_CLASS_INSTALLMENTS;
				authParams.SplitCount = order.CardInstallmentsCode;
			}

			// 3DSecure利用なし
			authParams.Skip3DSecure = "1";
			authParams.UseType = string.Empty;

			// 保存済みカードを利用
			authParams.StockCardMode = "1";
			authParams.CustomerId = userCreditCard.CooperationId;
			authParams.CustomerCardId = userCreditCard.CooperationId2;
			authParams.CardToken = string.Empty;
			var paygentPaymentMethod = Constants.DIGITAL_CONTENTS_OPTION_ENABLED && (order.DigitalContentsFlg == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON)
					? Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS
					: Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD;
			authParams.SalesMode =
				paygentPaymentMethod == Constants.PaygentCreditCardPaymentMethod.Auth
				? "0" 
				: "1";
			authParams.SecurityCodeToken = string.Empty;
			authParams.SecurityCodeUse = string.Empty;
			authParams.AuthId = string.Empty;
			var authResult = PaygentApiFacade.SendRequest(authParams);

			// 新しい注文情報で与信取得できたら注文登録後
			// 旧注文の決済データをキャンセル
			if (((string)authResult[PaygentConstants.RESPONSE_STATUS] == PaygentConstants.PAYGENT_RESPONSE_STATUS_SUCCESS))
			{
				var cardTranId = (string)authResult["payment_id"];
				// 旧支払い方法がクレカだったらキャンセル実施
				if (oldPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				{
					// キャンセル時用に取得
					var oldPaymentId = order.CardTranId;
					using (var accessor = new SqlAccessor())
					{
						accessor.OpenConnection();
						accessor.BeginTransaction();

						var count = new OrderService().UpdateOrderStatus(
							order.OrderId,
							Constants.FLG_ORDER_ORDER_STATUS_ORDERED,
							DateTime.Now,
							Constants.FLG_LASTCHANGED_USER,
							UpdateHistoryAction.Insert,
							accessor);
						count = new OrderService().UpdatePaygentOrder(
							order.OrderId,
							String.Empty,
							cardTranId,
							accessor);

						accessor.CommitTransaction();
					}
					// 受注情報再取得後cardTranIdだけ与信/売上をキャンセルするPayment_idに変更してキャンセル実施
					var paygentCancelOrder = new OrderService().Get(order.OrderId);
					paygentCancelOrder.CardTranId = oldPaymentId;
					var cancelResult = PaygentUtility.DoCancel(paygentCancelOrder);
					if (cancelResult.Item1)
					{
						// クレカ再与信後クレカキャンセル成功時の返却値
						return new ReauthActionResult(
						true,
						order.OrderId,
						CreatePaymentMemo(order.OrderPaymentKbn, string.Empty, cardTranId, GetSendingAmount(order)),
						cardTranId: cardTranId,
						paymentOrderId: string.Empty,
						cardTranIdForLog: cardTranId);
					}
					// 再与信後キャンセル失敗の返却値
					var resultString = (cancelResult.Item2 + ",").Split(',');
					return new ReauthActionResult(
						false,
						order.OrderId,
						string.Empty,
						cardTranIdForLog: order.CardTranId,
						apiErrorMessage: string.Format("再与信後キャンセル失敗　エラーコード：{0} エラー詳細：{1}",
							resultString[0],
							resultString[1]));
				}
				// クレカ再与信時の返却値
				return new ReauthActionResult(
					true,
					order.OrderId,
					CreatePaymentMemo(order.OrderPaymentKbn, string.Empty, cardTranId, GetSendingAmount(order)),
					cardTranId: cardTranId,
					paymentOrderId: string.Empty,
					cardTranIdForLog: cardTranId);

			}
			// 再与信失敗時の返却値
			return new ReauthActionResult(
				false,
				order.OrderId,
				string.Empty,
				cardTranIdForLog: order.CardTranId,
				apiErrorMessage: string.Format("再与信取得失敗　エラーコード：{0} エラー詳細：{1}",
					(string)authResult[PaygentConstants.RESPONSE_CODE],
					(string)authResult[PaygentConstants.RESPONSE_DETAIL]));
		}

		/// <summary>
		/// ヤマトKWC デバイス区分取得
		/// </summary>
		/// <param name="orderKbn">注文区分</param>
		/// <returns>デバイス区分</returns>
		public PaymentYamatoKwcDeviceDiv GetYamatoKwcDeviceDiv(string orderKbn)
		{
			switch (orderKbn)
			{
				case Constants.FLG_ORDER_ORDER_KBN_MOBILE:
					return PaymentYamatoKwcDeviceDiv.Mobile;

				case Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE:
					return PaymentYamatoKwcDeviceDiv.SmartPhone;

				default:
					return PaymentYamatoKwcDeviceDiv.Pc;
			}
		}

		/// <summary>
		/// ヤマトKWCクレジットオプションサービスパラメタ
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="userCreditCard">クレジットカード情報</param>
		/// <returns>オプションサービスパラメタ</returns>
		private PaymentYamatoKwcCreditOptionServiceParamBase GetPaymentYamatoKwcCreditOptionParam(OrderModel order, UserCreditCard userCreditCard)
		{
			// 登録カード利用の時
			var memberId = userCreditCard.CooperationInfo.YamatoKwcMemberId;
			var authenticationKey = userCreditCard.CooperationInfo.YamatoKwcAuthenticationKey;
			var creditInfoResult = new PaymentYamatoKwcCreditInfoGetApi().Exec(memberId, authenticationKey);
			if (creditInfoResult.Success)
			{
				if (creditInfoResult.CardDatas.Count > 0)
				{
					OrderCommon.AppendExternalPaymentCooperationLog(
						true,
						order.OrderId,
						LogCreator.CreateMessage("", "", memberId),
						order.LastChanged,
						UpdateHistoryAction.Insert);

					return new PaymentYamatoKwcCreditOptionServiceParamOptionRep(
						memberId, authenticationKey, creditInfoResult.CardDatas[0], string.Empty);
				}
				throw new Exception(string.Format("カード情報が0件しか取得できませんでした。：{0},{1}", memberId, authenticationKey));
			}
			else
			{
				OrderCommon.AppendExternalPaymentCooperationLog(
					false,
					order.OrderId,
					LogCreator.CreateErrorMessage(creditInfoResult.ErrorCode, creditInfoResult.ErrorMessage),
					order.LastChanged,
					UpdateHistoryAction.Insert);
			}

			throw new Exception(string.Format("カード情報が取得に失敗しました。（{0}）：{1},{2}", creditInfoResult.ErrorInfoForLog, memberId, authenticationKey));
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
			/// <summary>受注編集による実行か</summary>
			public bool IsExecModify { get; set; }
			/// <summary>決済情報（変更前）</summary>
			public string OldPaymentKbn { get; set; }
			#endregion
		}
	}
}
