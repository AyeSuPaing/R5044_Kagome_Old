/*
=========================================================================================================
  Module      : ベリトランスクレジットカード連携3Dセキュア(PaymentVeritransCredit3DS.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using jp.veritrans.tercerog.mdk.dto;
using jp.veritrans.tercerog.mdk;
using w2.App.Common.Extensions.Currency;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.Veritrans
{
	/// <summary>
	/// ベリトランスクレジットカード連携
	/// </summary>
	public class PaymentVeritransCredit3DS : PaymentVeriTransBase
	{
		/// <summary>ベリトランス決済種別</summary>
		protected override VeriTransConst.VeritransPaymentKbn VeritransPaymentType { get { return VeriTransConst.VeritransPaymentKbn.Credit; } }

		/// <summary>
		/// 3Dセキュア与信
		/// </summary>
		/// <param name="htOrder">注文情報</param>
		/// <param name="coCart">カート情報</param>
		/// <param name="reauthAmount">再与信時与信金額(受注後決済を変更した場合に利用)</param>
		/// <returns>3Dセキュア与信結果DTO</returns>
		public MpiAuthorizeResponseDto Auth(Hashtable htOrder, CartObject coCart, string reauthAmount = null)
		{
			var paymentMethodVeriTrans =
				(Constants.DIGITAL_CONTENTS_OPTION_ENABLED && coCart.IsDigitalContentsOnly)
					? Constants.PAYMENT_SETTING_VERITRANS4G_PAYMENTMETHOD_FORDIGITALCONTENTS
					: Constants.PAYMENT_SETTING_VERITRANS4G_PAYMENTMETHOD;

			var baseUrl = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				string.IsNullOrEmpty(Constants.WEBHOOK_DOMAIN)
					? Constants.SITE_DOMAIN
					: Constants.WEBHOOK_DOMAIN,
				Constants.PATH_ROOT_FRONT_PC);

			var requestData = new MpiAuthorizeRequestDto
			{
				ServiceOptionType = VeriTransConst.SERVICE_OPTION_TYPE_FULL_AUTH,
				OrderId = (string) htOrder[Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
				Amount = reauthAmount ?? coCart.PriceTotal.ToPriceString(), // 与信金額が空の場合、カートの支払金額総合計を入れる
				WithCapture = (paymentMethodVeriTrans == Constants.VeritransCreditCardPaymentMethod.Auth)
					? VeriTransConst.WITH_CAPTURE_ONLY_AUTH
					: VeriTransConst.WITH_CAPTURE_AND_AUTH,
				Token = (coCart.Payment.CreditToken == null)
					? string.Empty
					: coCart.Payment.CreditToken.Token,
				AccountId = coCart.Payment.UserCreditCard.CooperationId,
				HttpUserAgent = (string) htOrder[VeriTransConst.USER_AGENT],
				HttpAccept = (string) htOrder[VeriTransConst.HTTP_ACCEPT],
				RedirectionUri = (string) htOrder[VeriTransConst.REDIRECTION_URI_VERITRANS],
				DeviceChannel = Constants.PAYMENT_CREDIT_VERITRANS_3DSECURE2
					? VeriTransConst.VERITRANS_3DS_DEVICE_CHANNEL
					: string.Empty,
				PushUrl = Constants.PAYMENT_CREDIT_VERITRANS_3DSECURE2
					? new UrlCreator(baseUrl + Constants.PAGE_FRONT_PAYMENT_GMO_CARD_3DSECURE2_GET_RESULT).CreateUrl()
					: string.Empty,
				Jpo = CreateJpo(coCart.Payment.CreditInstallmentsCode),
			};

			var tran = new Transaction();
			var responseData = (MpiAuthorizeResponseDto)tran.Execute(requestData);

			return responseData;
		}
	}
}
