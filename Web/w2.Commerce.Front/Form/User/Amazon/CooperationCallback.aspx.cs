/*
=========================================================================================================
  Module      : Amazon 連携コールバック画面(CooperationCallback.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.Amazon.Util;
using w2.Domain.User;

/// <summary>
/// Amazon 連携コールバック画面
/// </summary>
public partial class Form_User_Amazon_CooperationCallback : AmazonLoginPage
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
			var amazonModel = (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];

			// 既に他のユーザと連携済のAmazonアカウントの場合はエラーとする
			var w2User = AmazonUtil.GetUserByAmazonUserId(amazonModel.UserId);
			if (w2User != null)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_COOPERATED_AMAZON_USER);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			// Amazon連携処理
			var userExtend = new UserService().GetUserExtend(this.LoginUserId);
			AmazonUtil.SetAmazonUserIdForUserExtend(userExtend, this.LoginUserId, amazonModel.UserId);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_SOCIAL_LOGIN_COOPERATION);
		}
	}
}