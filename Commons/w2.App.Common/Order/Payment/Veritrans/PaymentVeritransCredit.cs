/*
=========================================================================================================
  Module      : ベリトランスクレジットカード連携(PaymentVeritransCredit.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using jp.veritrans.tercerog.mdk.dto;
using jp.veritrans.tercerog.mdk;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region.Currency;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.Veritrans 
{
	/// <summary>
	/// ベリトランスクレジットカード連携
	/// </summary>
	public class PaymentVeritransCredit : PaymentVeriTransBase
	{
		/// <summary>ベリトランス決済種別</summary>
		protected override VeriTransConst.VeritransPaymentKbn VeritransPaymentType { get { return VeriTransConst.VeritransPaymentKbn.Credit; } }

		/// <summary>
		/// 与信処理
		/// </summary>
		/// <param name="htOrder">注文情報</param>
		/// <param name="coCart">カート情報</param>
		/// <param name="reauthAmount">再与信時与信金額(受注後決済を変更した場合に利用)</param>
		/// <returns>カート与信レスポンス</returns>
		public CardAuthorizeResponseDto Auth(Hashtable htOrder, CartObject coCart, string reauthAmount = null)
		{
			var paymentMethodVeriTrans =
				(Constants.DIGITAL_CONTENTS_OPTION_ENABLED && coCart.IsDigitalContentsOnly)
					? Constants.PAYMENT_SETTING_VERITRANS4G_PAYMENTMETHOD_FORDIGITALCONTENTS
					: Constants.PAYMENT_SETTING_VERITRANS4G_PAYMENTMETHOD;

			var requestData = new CardAuthorizeRequestDto
			{
				OrderId = (string) htOrder[Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
				Amount = reauthAmount ?? coCart.PriceTotal.ToPriceString(), // 与信金額が空の場合、カートの支払金額総合計を入れる
				WithCapture = (paymentMethodVeriTrans == Constants.VeritransCreditCardPaymentMethod.Auth)
					? VeriTransConst.WITH_CAPTURE_ONLY_AUTH
					: VeriTransConst.WITH_CAPTURE_AND_AUTH,
				Token = coCart.Payment.CreditToken.Token,
				AccountId = coCart.Payment.UserCreditCard.CooperationId,
				Jpo = CreateJpo(coCart.Payment.CreditInstallmentsCode),
			};

			var tran = new Transaction();
			var responseData = (CardAuthorizeResponseDto)tran.Execute(requestData);

			return responseData;
		}

		/// <summary>
		/// 再与信処理
		/// </summary>
		/// <param name="orderModel">注文モデル</param>
		/// <param name="userCreditCard">ユーザークレカモデル</param>
		/// <returns>カート再与信レスポンス</returns>
		public CardReAuthorizeResponseDto ReAuthorize(OrderModel orderModel, UserCreditCard userCreditCard, string newPaymentOrderId)
		{
			var paymentMethodVeriTrans =
				(Constants.DIGITAL_CONTENTS_OPTION_ENABLED &&
				 (orderModel.DigitalContentsFlg == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON))
					? Constants.PAYMENT_SETTING_VERITRANS4G_PAYMENTMETHOD_FORDIGITALCONTENTS
					: Constants.PAYMENT_SETTING_VERITRANS4G_PAYMENTMETHOD;

			var requestData = new CardReAuthorizeRequestDto
			{
				OrderId = newPaymentOrderId,
				OriginalOrderId = orderModel.PaymentOrderId,
				Amount = CurrencyManager.GetSendingAmount(
					orderModel.LastBilledAmount,
					orderModel.SettlementAmount,
					orderModel.SettlementCurrency).ToPriceString(),
				WithCapture = (paymentMethodVeriTrans == Constants.VeritransCreditCardPaymentMethod.Auth)
					? VeriTransConst.WITH_CAPTURE_ONLY_AUTH
					: VeriTransConst.WITH_CAPTURE_AND_AUTH,
				Jpo = CreateJpo(orderModel.CardInstallmentsCode),
			};

			var tran = new Transaction();
			var responseData = (CardReAuthorizeResponseDto)tran.Execute(requestData);

			return responseData;
		}

		/// <summary>
		/// 売上確定処理
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="amount">売上確定金額</param>
		/// <returns>売上確定レスポンス</returns>
		public CardCaptureResponseDto Capture(string paymentOrderId, string amount)
		{
			var requestData = new CardCaptureRequestDto
			{
				OrderId = paymentOrderId,
				Amount = amount
			};

			var tran = new Transaction();
			var responseData = (CardCaptureResponseDto)tran.Execute(requestData);

			return responseData;
		}

		/// <summary>
		/// キャンセル処理
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <returns>カードキャンセルレスポンス</returns>
		public CardCancelResponseDto Cancel(string paymentOrderId)
		{
			var requestData = new CardCancelRequestDto { OrderId = paymentOrderId };
			var tran = new Transaction();
			var responseData = (CardCancelResponseDto)tran.Execute(requestData);

			return responseData;
		}

		/// <summary>
		/// カード情報取得処理
		/// </summary>
		/// <param name="accountId">会員ID</param>
		/// <returns>カード情報取得レスポンス</returns>
		public CardInfoGetResponseDto GetCardInfo(string accountId)
		{
			var reqDto = new CardInfoGetRequestDto { AccountId = accountId };
			var tran = new Transaction();
			var responseData = (CardInfoGetResponseDto)tran.Execute(reqDto);

			return responseData;
		}

		/// <summary>
		/// ワンクリック継続課金処理
		/// </summary>
		/// <param name="htOrder">注文情報</param>
		/// <param name="coCart">カート情報</param>
		/// <param name="reauthAmount">再与信時与信金額(受注後決済を変更した場合に利用)</param>
		/// <returns>ワンクリック継続課金レスポンス</returns>
		public CardAuthorizeResponseDto UsePayNowId(Hashtable htOrder, CartObject coCart, string reauthAmount = null)
		{
			var paymentMethodVeriTrans =
				(Constants.DIGITAL_CONTENTS_OPTION_ENABLED && coCart.IsDigitalContentsOnly)
					? Constants.PAYMENT_SETTING_VERITRANS4G_PAYMENTMETHOD_FORDIGITALCONTENTS
					: Constants.PAYMENT_SETTING_VERITRANS4G_PAYMENTMETHOD;

			var requestData = new CardAuthorizeRequestDto
			{
				OrderId = (string)htOrder[Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
				Amount = reauthAmount ?? coCart.PriceTotal.ToPriceString(), // 与信金額が空の場合、カートの支払金額総合計を入れる
				AccountId = coCart.Payment.UserCreditCard.CooperationId,
				WithCapture = (paymentMethodVeriTrans == Constants.VeritransCreditCardPaymentMethod.Auth)
					? VeriTransConst.WITH_CAPTURE_ONLY_AUTH
					: VeriTransConst.WITH_CAPTURE_AND_AUTH,
				Jpo = CreateJpo(coCart.Payment.CreditInstallmentsCode),
			};

			var tran = new Transaction();
			var responseData = (CardAuthorizeResponseDto)tran.Execute(requestData);

			return responseData;
		}
	}
}
