/*
=========================================================================================================
  Module      : Amazonログイン 基底ページ(AmazonLoginPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.Amazon.Util;
using w2.App.Common.AmazonCv2;

/// <summary>
/// Amazonログイン 基底ページ
/// </summary>
public class AmazonLoginPage : BasePage
{
	/// <summary>
	/// 初期処理
	/// </summary>
	protected void InitPage()
	{
		if (IsPostBack)
		{
			// トークン取得
			var key = Constants.AMAZON_PAYMENT_CV2_ENABLED
				? AmazonCv2Constants.REQUEST_KEY_AMAZON_BUYER_TOKEN
				: AmazonConstants.REQUEST_KEY_AMAZON_TOKEN;
			var token = Request[key];

			// Amazonアカウント認証
			this.AmazonModel = AmazonUtil.AuthenticationAmazon(token);
			if (this.AmazonModel == null)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_INVALID_TOKEN_FOR_AMAZON);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			// セッションにAmazonアカウント情報格納
			this.AmazonModel.TransitionCallbackSourcePath = this.AppRelativeVirtualPath;
			Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL] = this.AmazonModel;
		}
	}

	#region プロパティ
	/// <summary>Amazonモデル</summary>
	protected AmazonModel AmazonModel { get; set; }
	#endregion
}