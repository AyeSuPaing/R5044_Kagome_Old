using System.Collections;
/*
=========================================================================================================
  Module      : Amazon注文 基底ページ(AmazonOrderPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.Amazon.Util;
using w2.App.Common.AmazonCv2;
using w2.Domain.User;

/// <summary>
/// Amazon注文 基底ページ
/// </summary>
public class AmazonOrderPage : OrderCartPage
{
	/// <summary>
	/// 初期処理
	/// </summary>
	protected void InitPage()
	{
		if (!IsPostBack)
		{
			// Amazon Payが利用可能かチェック
			if (CanUseAmazonPayment() == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = "この注文ではAmazon Payはご利用いただけません。";
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
		}
		else
		{
			this.AmazonCheckoutSessionId = Request[AmazonCv2Constants.REQUEST_KEY_AMAZON_CHECKOUT_SESSION_ID];

			if (string.IsNullOrEmpty(this.AmazonCheckoutSessionId) == false)
			{
				var checoutSession = new AmazonCv2ApiFacade().GetCheckoutSession(this.AmazonCheckoutSessionId);
				this.AmazonModel = new AmazonModel(this.AmazonCheckoutSessionId, checoutSession.Buyer);
			}
			else
			{
				// Amazonアカウント認証
				this.AmazonModel = AmazonUtil.AuthenticationAmazon(Request[AmazonConstants.REQUEST_KEY_AMAZON_TOKEN]);
			}

			if (this.AmazonModel == null)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_INVALID_TOKEN_FOR_AMAZON);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			// セッションにAmazonアカウント情報格納
			this.AmazonModel.TransitionCallbackSourcePath = this.AppRelativeVirtualPath;
		}
	}

	/// <summary>
	/// メールアドレスが既にユーザー登録しているが、Amazon未連携している場合、自動Amazon連携をする
	/// </summary>
	/// <returns>ユーザーモデル</returns>
	protected UserModel UpdateCooperation()
	{
		var user = new UserInput
		{
			UserId = this.AmazonModel.UserId,
			MailAddr = this.AmazonModel.Email
		};

		var userService = new UserService();
		var w2User = userService.GetRegisteredUserByMailAddress(user.MailAddr);

		// ユーザー取得失敗した場合はnullを返す
		if (w2User == null) return null;

		w2User.UserExtend = userService.GetUserExtend(w2User.UserId);
		AmazonUtil.SetAmazonUserIdForUserExtend(w2User.UserExtend, w2User.UserId, this.AmazonModel.UserId);
		return w2User;
	}

	#region プロパティ
	/// <summary>Amazonモデル</summary>
	protected AmazonModel AmazonModel
	{
		get { return (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL]; }
		private set { Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL] = value; }
	}
	/// <summary>チェックアウトセッションID</summary>
	protected string AmazonCheckoutSessionId
	{
		get { return (string)Session[AmazonCv2Constants.SESSION_KEY_AMAZON_CHECKOUT_SESSION_ID]; }
		set { Session[AmazonCv2Constants.SESSION_KEY_AMAZON_CHECKOUT_SESSION_ID] = value; }
	}
	#endregion
}
