/*
=========================================================================================================
  Module      : 楽天3Dセキュア認証データ送信ページ(PostCard3DSecureAuth.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Rakuten;
using w2.App.Common.Order.Payment.Rakuten.AuthorizeHtml;
using w2.Common.Web;
using w2.Domain.UserCreditCard;

public partial class Payment_Rakuten_Post3DSAuth : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		var orderId = Request[Constants.REQUEST_KEY_ORDER_ID];

		PaymentFileLogger.WritePaymentLog(
			null,
			Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
			PaymentFileLogger.PaymentType.Rakuten,
			PaymentFileLogger.PaymentProcessingType.Rakuten3DSecureAuthResultSend,
			LogCreator.CreateMessage(orderId, ""));

		// セッション情報をファイルに保存
		SessionSecurityManager.SaveSesstionContetnsToDatabaseForGoToOtherSite(Session, orderId);

		var context = HttpContext.Current;
		var ipAddress = (context != null) ? context.Request.ServerVariables["REMOTE_ADDR"] : "";

		// 仮注文情報取得
		var tempOrderData = OrderCommon.GetOrder(orderId);
		if (tempOrderData.Count == 0) return;

		var tempOrder = tempOrderData[0];
		var paymentOrderId = OrderCommon.CreatePaymentOrderId((string)tempOrder[Constants.FIELD_ORDER_SHOP_ID]);
		var installmentsCode = (string)tempOrder[Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE];
		var sendingAmount = CurrencyManager.GetSendingAmount(
			(decimal)tempOrder[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT],
			(decimal)tempOrder[Constants.FIELD_ORDER_SETTLEMENT_AMOUNT],
			(string)tempOrder[Constants.FIELD_ORDER_SETTLEMENT_CURRENCY]);

		// セッションに格納された楽天連携用註文情報を取得
		var param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
		var rakuten3DSecurePaymentOrder = (List<Hashtable>)param["rakuten_order_3dsecure"];
		var order = rakuten3DSecurePaymentOrder.Find(rakutenOrder => (string)rakutenOrder[Constants.FIELD_ORDER_ORDER_ID] == orderId);
		var userCreditCard = new UserCreditCard(
			new UserCreditCardService().Get(
				(string)order[Constants.FIELD_ORDER_USER_ID],
				(int)order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO]));

		// 註文情報からリクエストパラメータを作成
		var rakutenAuthourizeRequest = new RakutenAuthorizeHtmlRequest(ipAddress)
		{
			PaymentId = paymentOrderId,
			GrossAmount = sendingAmount.ToString("0"),
			CardToken = new CardTokenBase
			{
				Amount = sendingAmount.ToString("0"),
				CardToken = userCreditCard.CooperationId,
				CvvToken = StringUtility.ToNull(order[CartPayment.FIELD_CREDIT_RAKUTEN_CVV_TOKEN]),
			},
			CallbackUrl = CreateReturnUrl(orderId),
		};
		rakutenAuthourizeRequest.CardToken.ThreeDSecure = new ThreeDSecure
		{
			MessageCategoryType = RakutenConstants.MESSAGE_CATEGORY_TYPE_PA,
			PurchaseDate = ((DateTime)order[Constants.FIELD_ORDER_ORDER_DATE]).ToString("yyyyMMddHHmmss")
		};
		switch (installmentsCode)
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

			case "":
				break;

			// 支払い回数
			default:
				rakutenAuthourizeRequest.CardToken.Installments = installmentsCode;
				break;
		}
		SetMerchantId(rakutenAuthourizeRequest, userCreditCard.CompanyCode);

		var jsonData = JsonConvert.SerializeObject(
			rakutenAuthourizeRequest,
			Formatting.None,
			new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			});

		var targetApiUrl = "";
		switch (Constants.PAYMENT_RAKUTEN_CREDIT_PAYMENT_METHOD)
		{
			case Constants.RakutenPaymentType.AUTH:
				targetApiUrl = RakutenConstants.URL_PAYMENT_AUTHORIZE_3D_ON;
				break;

			case Constants.RakutenPaymentType.CAPTURE:
				targetApiUrl = RakutenConstants.URL_PAYMENT_AUTHORIZE_AND_CAPTURE_3D_ON;
				break;
		}
		this.Card3DSecureAuthUrl = string.Format(
			targetApiUrl,
			Constants.PAYMENT_RAKUTEN_API_URL);
		this.PaymentInfo = RakutenApiFacade.EncodeToBase64Url(Encoding.UTF8.GetBytes(jsonData));
		this.Signature = RakutenApiFacade.CreateSignature(jsonData);
		this.ReturnUrl = CreateReturnUrl(orderId);
    }

	/// <summary>
	/// 認証結果受け取り先URL作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>認証結果受け取り先URL</returns>
	private string CreateReturnUrl(string orderId)
	{
		var url = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_RAKUTEN_3DS_RESULT)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderId)
			.CreateUrl();

		return url;
	}

	/// <summary>
	/// リクエストパラメータにマーチャントIDを設定する
	/// </summary>
	/// <param name="request">注文ID</param>
	/// <param name="companyCode">クレジットカード会社コード</param>
	private void SetMerchantId(RakutenAuthorizeHtmlRequest request, string companyCode)
	{
		switch (companyCode)
		{
			case Constants.FLG_USERCREDITCARD_VISA:
				request.CardToken.ThreeDSecure.MerchantId = Constants.PAYMENT_SETTING_RAKUTEN_3DSECURE_MERCHANT_ID_VISA;
				break;

			case Constants.FLG_USERCREDITCARD_MASTER:
				request.CardToken.ThreeDSecure.MerchantId = Constants.PAYMENT_SETTING_RAKUTEN_3DSECURE_MERCHANT_ID_MASTERCARD;
				break;
		}
	}

	/// <summary>3DセキュアAPI URL</summary>
	protected string Card3DSecureAuthUrl { get; set; }
	/// <summary>3Dセキュア 決済情報</summary>
	protected string PaymentInfo { get; set; }
	/// <summary>3Dセキュア シグネチャ</summary>
	protected string Signature { get; set; }
	/// <summary>3Dセキュア認証キー</summary>
	protected string KeyVersion { get { return RakutenConstants.KEY_VERSION; } }
	/// <summary>認証結果戻し先URL</summary>
	protected string ReturnUrl { get; set; }
}