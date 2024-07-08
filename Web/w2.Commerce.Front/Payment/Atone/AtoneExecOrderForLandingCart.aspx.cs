/*
=========================================================================================================
  Module      : Atone LPカート用与信取得ページ(AtoneExecOrderForLandingCart.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Web;
using w2.Domain.LandingPage;

public partial class Payment_Atone_AtoneExecOrderForLandingCart : OrderCartPageLanding
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		ucAtonePaymentScript.CurrentUrl = Constants.PATH_ROOT + Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM;
		if (!IsPostBack)
		{
			hfAtoneToken.Value = this.IsLoggedIn
				? this.LoginUser.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_ATONE_TOKEN_ID]
				: string.Empty;
		}
	}

	/// <summary>
	/// LP確認画面に戻るボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReturnToConfirm_Click(object sender, EventArgs e)
	{
		// 確認画面スキップフラグをセッション内に保持
		Session[this.LadingCartConfirmSkipFlgSessionKey] = LandingPageConst.ORDER_CONFIRM_PAGE_SKIP_FLG_ON;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM);
	}

	/// <summary>
	/// LP入力画面に戻るボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReturnToInput_Click(object sender, EventArgs e)
	{
		SessionManager.IsRedirectFromLandingCartConfirm = true;

		// ブラウザバック制御の為、入力画面のURLを画面遷移正当性チェック用セッションへ格納
		Session[this.LadingCartNextPageForCheck] = this.LandingCartInputUrl;

		// ターゲットページ設定(ユーザー拡張項目用)
		Session[Constants.SESSION_KEY_TARGET_PAGE + "_extend"] = Constants.PAGE_FRONT_LANDING_LANDING_CART_INPUT;

		// ランディング入力画面へ戻る
		Response.Redirect(this.LandingCartInputUrl);
	}
}
