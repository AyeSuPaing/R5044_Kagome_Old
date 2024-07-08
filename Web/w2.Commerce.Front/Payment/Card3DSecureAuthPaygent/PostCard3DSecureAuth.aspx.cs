/*
=========================================================================================================
  Module      : ペイジェント3Dセキュア送信画面 (PostCard3DSecureAuth.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.App.Common.Order.Payment.Paygent;
using w2.Common.Web;
using w2.Domain.Order;

/// <summary>
/// ペイジェント3Dセキュア送信画面
/// </summary>
public partial class PostCard3DSecureAuth : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 3Dセキュア認証で送るパラメータ作成
		// アカウント保持期間
		var accountIndicator = "01";
		if (IsLoggedIn)
		{
			var dateSpan = (DateTime.Now - LoginUser.DateCreated).TotalDays;
			// w2_User.date_createdが当日(含め)から30日未満過ぎている場合
			if (dateSpan < 30) accountIndicator = "03";
			// w2_User.date_createdが当日(含め)から30日-60日過ぎている場合
			if ((dateSpan >= 30) && (dateSpan < 60)) accountIndicator = "04";
			// w2_User.date_createdが当日(含め)から60日以上過ぎている場合
			if (dateSpan >= 60) accountIndicator = "05";
		}

		// 3Dセキュア認証電文用パラメータセット
		var paygentOrder = (Hashtable)Session[PaygentConstants.PAYGENT_SESSION_ORDER];
		var order = new OrderModel(paygentOrder);
		order.PaygentCreditToken = (string)paygentOrder[Constants.CREDIT_CARD_TOKEN];
		var paygent3dsRequest = new PaygentCreditCard3DSecureAuthApi();

		paygent3dsRequest.TradingId = order.OrderId;
		paygent3dsRequest.TermUrl = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_PAYGENT_POST_3DS_RESULT)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, order.OrderId)
			.CreateUrl();
		paygent3dsRequest.MerchantName = Constants.PAYMENT_PAYGENT_MERCHANTNAME;
		paygent3dsRequest.AuthenticationType = "01";
		var customerCardId = (string)paygentOrder[Constants.FIELD_USERCREDITCARD_COOPERATION_ID2];
		// 顧客カードIDない場合はカード情報の指定方法を"token"に、存在する場合は"customer"にする。
		if (string.IsNullOrEmpty(customerCardId))
		{
			paygent3dsRequest.CardSetMethod = "token";
			paygent3dsRequest.CardToken = order.PaygentCreditToken;
			paygent3dsRequest.CustomerId = string.Empty;
			paygent3dsRequest.CustomerCardId = string.Empty;
		}
		else
		{
			paygent3dsRequest.CardSetMethod = "customer";
			paygent3dsRequest.CardToken = string.Empty;
			paygent3dsRequest.CustomerId = (string)paygentOrder[Constants.FIELD_USERCREDITCARD_COOPERATION_ID];
			paygent3dsRequest.CustomerCardId = customerCardId;
		}
		paygent3dsRequest.PaymentAmount = Math.Floor(order.OrderPriceTotal).ToString();
		paygent3dsRequest.TransactionType = "01";
		paygent3dsRequest.LoginType = IsLoggedIn ?
			PaygentConstants.PAYGENT_PAYMENT_LOGIN_TYPE_LOGGED_IN
			: PaygentConstants.PAYGENT_PAYMENT_LOGIN_TYPE_NOT_LOGGED_IN;

		// ログイン中ユーザーの最終ログイン日、アカウント作成日が取得できれば入れる
		if (IsLoggedIn && LoginUser.DateLastLoggedin != null)
		{
			paygent3dsRequest.LoginDate = TimeZoneInfo.ConvertTimeToUtc((DateTime)LoginUser.DateLastLoggedin).ToString("yyyyMMddHHmm");
			paygent3dsRequest.AccountCreateDate = TimeZoneInfo.ConvertTimeToUtc(LoginUser.DateCreated).ToString("yyyyMMdd");
		}
		paygent3dsRequest.AccountIndicator = accountIndicator;
		var result = PaygentApiFacade.SendRequest(paygent3dsRequest);
		Session["Paygent3DSResult"] = result;
		var htmlStr = (string)result["out_acs_html"];
		this.Response.Write(htmlStr);
	}
}
