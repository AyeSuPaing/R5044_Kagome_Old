/*
=========================================================================================================
  Module      : LINEログイン ログインコールバック画面(LoginCallback.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.Line.Util;
using w2.App.Common.User;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.App.Common.CrossPoint.User;
using w2.App.Common.User.SocialLogin.Helper;
using w2.Common.Util;
using w2.Domain.UpdateHistory;
using w2.App.Common.Global.Region;
using w2.App.Common.Util;
using w2.Domain.User.Helper;
using w2.App.Common.Option;

/// <summary>
/// ソーシャルログイン ログインコールバック画面
/// </summary>
public partial class Form_User_Line_LoginCallback : LineLoginPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			InitPage();

			var nextPath = this.NextUrl;

			if (string.IsNullOrEmpty(Constants.SOCIAL_PROVIDER_ID_LINE)) Response.Redirect(nextPath);

			var w2user = LineUtil.GetUserByLineUserId(this.LineUserId, Constants.SOCIAL_PROVIDER_ID_LINE);

			if (string.IsNullOrEmpty(this.LoginUserId) == false)
			{
				if (w2user != null)
				{
					ReturnSocialLoginCooperationPageWithError(
					"failed",
					"This social media has already been associated with another user.");
				}
				var service = new UserService();
				var userExtend = service.GetUserExtend(this.LoginUserId);
				LineUtil.SetLineUserIdForUserExtend(userExtend, this.LoginUserId, LineUserId);

				var user = service.Get(this.LoginUserId);
				SetLoginUserData(user, UpdateHistoryAction.DoNotInsert);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_SOCIAL_LOGIN_COOPERATION);
			}

			if (w2user != null)
			{
				if (nextPath == (Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_REGULATION)) nextPath = Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_PAYMENT;
			}
			else
			{
				if (w2.App.Common.Line.Constants.LINE_DIRECT_AUTO_LOGIN_OPTION == false)
				{
					SessionManager.LineProviderUserId = this.LineUserId;
					Response.Redirect(nextPath);
				}

				// 自動ログインOPがONの場合はユーザIDとライン連携IDのみを持つユーザを作成する
				var user = new UserModel();

				var userId = UserService.CreateNewUserId(
					Constants.CONST_DEFAULT_SHOP_ID,
					Constants.NUMBER_KEY_USER_ID,
					Constants.CONST_USER_ID_HEADER,
					Constants.CONST_USER_ID_LENGTH);
				user.UserId = userId;
				user.LastChanged = Constants.FLG_LASTCHANGED_USER;
				user.UserExtend = new UserExtendModel(new UserExtendSettingList());
				user.UserExtend.UserExtendColumns.Add(Constants.SOCIAL_PROVIDER_ID_LINE);
				user.UserExtend.UserExtendDataValue[Constants.SOCIAL_PROVIDER_ID_LINE] = this.LineUserId;
				user.MemberRankId = MemberRankOptionUtility.GetDefaultMemberRank();

				// クロスポイント連携
				if (Constants.CROSS_POINT_OPTION_ENABLED)
				{
					user.UserExtend.UserExtendColumns.Add(Constants.CROSS_POINT_USREX_SHOP_APP_MEMBER_FLAG);
					if (string.IsNullOrEmpty(SessionManager.AppKeyForCrossPoint) == false)
					{
						user.UserExtend.UserExtendDataValue[Constants.CROSS_POINT_USREX_SHOP_APP_MEMBER_FLAG]
							= (StringUtility.ToEmpty(SessionManager.AppKeyForCrossPoint) == Constants.CROSS_POINT_APP_REQUEST_APP_KEY)
								? Constants.FLG_USEREXTEND_USREX_SHOP_APP_MEMBER_FLAG_ON
								: Constants.FLG_USEREXTEND_USREX_SHOP_APP_MEMBER_FLAG_OFF;
						SessionManager.AppKeyForCrossPoint = null;
						SessionManager.MemberIdForCrossPoint = null;
						SessionManager.PinCodeForCrossPoint = null;
					}
					else
					{
						user.UserExtend.UserExtendDataValue[Constants.CROSS_POINT_USREX_SHOP_APP_MEMBER_FLAG]
							= Constants.FLG_USEREXTEND_USREX_SHOP_APP_MEMBER_FLAG_OFF;
					}

					CooperationCrossPointApi(user);
				}

				var userService = new UserService();
				userService.InsertWithUserExtend(
					user,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert);

				// Userのグローバル情報を更新
				RegionManager.GetInstance().UpdateUserRegion(new RegionModel(), userId);

				// DBから最新情報を取得
				var registedUser = userService.Get(userId);

				// ユーザー登録イベント
				new UserRegister(
					false,
					(List<w2.App.Common.Order.CartObject>)Session[Constants.SESSION_KEY_USER_REGIST_AFTER_ORDER_CART_LIST],
					Constants.SOCIAL_LOGIN_ENABLED
						? (SocialLoginModel)Session[Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL]
						: null,
					Constants.AMAZON_LOGIN_OPTION_ENABLED
						? (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL]
						: null,
					this.IsRakutenIdConnectUserRegister)
						.ExecProcessOnUserRegisted(
						registedUser,
						UpdateHistoryAction.DoNotInsert);

				// セッション情報のクリア
				Session.Remove(Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL);
				Session.Remove(Constants.SESSION_KEY_REFERRAL_CODE);

				// 更新履歴登録
				new UpdateHistoryService().InsertForUser(userId, Constants.FLG_LASTCHANGED_USER);

				// IPとアカウント、IPとパスワードでそれぞれアカウントロックがかかっている可能性がある。
				// そのため、登録時にアカウントロックキャンセルを行う
				LoginAccountLockManager.GetInstance().CancelAccountLock(
					this.Page.Request.UserHostAddress,
					user.LoginId,
					user.PasswordDecrypted);

				// ログインIDをCookieから削除 ※クッキーのログインIDが他者の可能性があるため
				UserCookieManager.CreateCookieForLoginId(string.Empty, consent: false);
				// ログイン成功アクション実行
				var loginUser = userService.Get(userId);
				SetLoginUserData(loginUser, UpdateHistoryAction.DoNotInsert);
				ExecLoginSuccessActionAndGoNextInner(loginUser, nextPath, UpdateHistoryAction.Insert);
			}

			SetLoginUserData(w2user, UpdateHistoryAction.DoNotInsert);
			ExecLoginSuccessActionAndGoNextInner(w2user, nextPath, UpdateHistoryAction.Insert);
		}
	}

	/// <summary>
	/// クロスポイントユーザ登録連携
	/// </summary>
	/// <param name="user">User model</param>
	private void CooperationCrossPointApi(UserModel user)
	{
		var apiResult = new CrossPointUserApiService().Insert(user);

		if (apiResult.IsSuccess == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG]
				= apiResult.ErrorCodeList.Contains(
					w2.App.Common.Constants.CROSS_POINT_RESULT_DUPLICATE_MEMBER_ID_ERROR_CODE)
						? apiResult.ErrorMessage
						: MessageManager.GetMessages(w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		var userApiResult = new CrossPointUserApiService().Get(user.UserId);
		if (userApiResult == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		if (user.UserExtend.UserExtendColumns.Contains(Constants.CROSS_POINT_USREX_SHOP_CARD_NO) == false)
		{
			user.UserExtend.UserExtendColumns.Add(Constants.CROSS_POINT_USREX_SHOP_CARD_NO);
		}
		if (user.UserExtend.UserExtendColumns.Contains(Constants.CROSS_POINT_USREX_SHOP_CARD_PIN) == false)
		{
			user.UserExtend.UserExtendColumns.Add(Constants.CROSS_POINT_USREX_SHOP_CARD_PIN);
		}

		user.UserExtend.UserExtendDataValue.CrossPointShopCardNo = userApiResult.RealShopCardNo;
		user.UserExtend.UserExtendDataValue.CrossPointShopCardPin = userApiResult.PinCode;
	}
}
