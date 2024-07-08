/*
=========================================================================================================
  Module      : ヤマトKWC 3Dセキュア与信送信画面(PostCard3DSecureAuth.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using w2.App.Common.Order.Payment;
using w2.Common.Web;
using w2.Common.Logger;

public partial class Payment_YamatoKwc_PostCard3DSecureAuth : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	protected void Page_Load(object sender, EventArgs e)
	{
		try
		{
			// リクエストから対象の注文IDを取得
			var targetOrderId = this.Request[Constants.REQUEST_KEY_ORDER_ID];

			// 認証結果取得ページで使用するためセッションに格納
			Session[Constants.FIELD_ORDER_ORDER_ID] = targetOrderId;

			var sessionParam = (Hashtable)this.Session[Constants.SESSION_KEY_PARAM];
			var order =
				((List<Hashtable>)sessionParam[Constants.SESSION_KEY_PAYMENT_CREDIT_YAMATOKWC_ORDER_3DSECURE]).Find(
					paymentOrder => ((string)paymentOrder[Constants.FIELD_ORDER_ORDER_ID] == targetOrderId));

			this.ReturnUrl = CreateReturnUrl(order[Constants.FIELD_ORDER_ORDER_ID].ToString());

			this.Card3DSecureAuthHtml = Regex.Match(order[Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_URL].ToString(), @"<!\[CDATA\[(.+)\]\]>").Value;

			card3DSecureAuthHtml.InnerHtml = this.Card3DSecureAuthHtml;

			PaymentFileLogger.WritePaymentLog(
				null,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.Yamatokwc,
				PaymentFileLogger.PaymentProcessingType.CreditPaymentProcessing,
				string.Format("orderId: {0} Card3DSecureAuthHTML: {1}", targetOrderId, this.Card3DSecureAuthHtml));
		}
		catch (Exception ex)
		{
			FileLogger.WriteError(ex);

			this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION);

			// エラーページへ遷移
			var url = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();
			Response.Redirect(url);
		}
	}

	/// <summary>
	/// 認証結果戻し先URL作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>認証結果戻し先URL</returns>
	private string CreateReturnUrl(string orderId)
	{
		var url = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_YAMATO_3DS_RESULT)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderId)
			.CreateUrl();
		return url;
	}

	/// <summary>トランザクション名</summary>
	private string TransactionName { get; set; }
	/// <summary>3Dセキュア認証URL</summary>
	protected string Card3DSecureAuthHtml { get; set; }
	/// <summary>認証結果戻し先URL</summary>
	protected string ReturnUrl { get; set; }
}
