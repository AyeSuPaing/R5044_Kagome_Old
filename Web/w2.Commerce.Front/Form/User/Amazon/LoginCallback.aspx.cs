/*
=========================================================================================================
  Module      : Amazon ログインコールバック画面(LoginCallback.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.Amazon.Util;
using w2.Domain.UpdateHistory.Helper;

/// <summary>
/// Amazon ログインコールバック画面
/// </summary>
public partial class Form_User_Amazon_LoginCallback : AmazonLoginPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (IsPostBack)
		{
			// 初期処理
			InitPage();

			// AmazonユーザーIDからユーザー情報取得
			var w2User = AmazonUtil.GetUserByAmazonUserId(this.AmazonModel.UserId);

			// 遷移元取得
			var state = Request[AmazonConstants.REQUEST_KEY_AMAZON_STATE];
			var nextPath = Request[Constants.REQUEST_KEY_NEXT_URL];
			if (string.IsNullOrEmpty(nextPath)) nextPath = Constants.PATH_ROOT;

			// Amazon未連携ユーザーの場合
			if (w2User == null)
			{
				//　会員登録画面からの遷移場合は、会員登録ページに遷移
				if　(state == (Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_INPUT))
				{
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_INPUT);
				}
				// かんたん会員登録
				else if (state == (Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_EASY_REGIST_INPUT))
				{
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_EASY_REGIST_INPUT);
				}

				// 三分岐画面からの遷移でない場合、会員登録規約画面に遷移
				if (state != (Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_OWNER_DECISION))
				{
					this.AmazonModel.TransitionCallbackSourcePath = this.AppRelativeVirtualPath;
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_REGULATION);
				}
				else
				{
					nextPath = Constants.PAGE_FRONT_ORDER_SHIPPING;
					Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = nextPath;
					Response.Redirect(Constants.PATH_ROOT + nextPath);
				}
			}

			// ログイン処理をし、遷移
			SetLoginUserData(w2User, UpdateHistoryAction.DoNotInsert);
			ExecLoginSuccessActionAndGoNextInner(w2User, nextPath, UpdateHistoryAction.Insert);
		}
	}
}
