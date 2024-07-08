/*
=========================================================================================================
  Module      : 会員登録確認画面処理(UserRegistConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.App.Common.CrossPoint;
using w2.App.Common.CrossPoint.User;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.Global;
using w2.App.Common.Global.Region;
using w2.App.Common.Line.Util;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.FrameGuarantee;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.User;
using w2.App.Common.User.SocialLogin.Helper;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.LineTemporaryUser;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using CrossPointConstants = w2.App.Common.Constants;
using w2.Domain.UserBusinessOwner;

public partial class Form_User_UserRegistConfirm : UserPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// HTTPS通信チェック（HTTPのとき、規約画面へ）
		//------------------------------------------------------
		CheckHttps(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_REGULATION);

		//------------------------------------------------------
		// ログインチェック（ログイン済みの場合、トップ画面へ）
		//------------------------------------------------------
		if (this.IsLoggedIn && (SessionManager.HasTemporaryUserId == false))
		{
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT);
		}

		//------------------------------------------------------
		// URLセッションチェック
		//------------------------------------------------------
		CheckUrlSessionForUserRegistModify();

		// ターゲットページ設定
		this.SessionParamTargetPage = Constants.PAGE_FRONT_USER_REGIST_INPUT;

		this.IsVisible_UserPassword = true;

		if (Constants.SOCIAL_LOGIN_ENABLED)
		{
			var socialLogin = (SocialLoginModel)Session[Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL];
			if (socialLogin != null)
			{
				socialLogin.TransitionSourcePath = this.AppRelativeVirtualPath;
				this.IsVisible_UserPassword = false;
			}
		}
		if ((Constants.SOCIAL_LOGIN_ENABLED == false)
			&& w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED
			&& (string.IsNullOrEmpty(SessionManager.LineProviderUserId) == false))
		{
			var userExtend = new UserExtendInput();
			userExtend.UserExtendDataValue[Constants.SOCIAL_PROVIDER_ID_LINE] = SessionManager.LineProviderUserId;
			this.UserInput.UserExtendInput = userExtend;
		}

		if (Constants.AMAZON_LOGIN_OPTION_ENABLED && this.IsAmazonLoggedIn)
		{
			this.IsVisible_UserPassword = false;
		}
		if (SessionManager.PayPalLoginResult != null)
		{
			this.IsVisible_UserPassword = false;
		}
		if (this.IsRakutenIdConnectUserRegister)
		{
			this.IsVisible_UserPassword = false;
		}
	}

	/// <summary>
	/// 送信リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSend_Click(object sender, EventArgs e)
	{
		var user = this.UserInput.CreateModel();

		// 広告コードより補正
		UserUtility.CorrectUserByAdvCode(user);

		// Addr3,Addr4がnullの可能性があるのでnull→空文字
		if (this.IsAmazonLoggedIn)
		{
			user.Addr3 = StringUtility.ToEmpty(user.Addr3);
			user.Addr4 = StringUtility.ToEmpty(user.Addr4);
		}
		user.UserExtend = this.UserInput.UserExtendInput.CreateModel();

		if (Constants.CROSS_POINT_OPTION_ENABLED)
		{
			user.UserExtend.UserExtendColumns.Add(Constants.CROSS_POINT_USREX_SHOP_APP_MEMBER_FLAG);
			if (string.IsNullOrEmpty(SessionManager.AppKeyForCrossPoint) == false)
			{
				user.UserExtend.UserExtendDataValue[Constants.CROSS_POINT_USREX_SHOP_APP_MEMBER_FLAG] =
					(StringUtility.ToEmpty(SessionManager.AppKeyForCrossPoint) == CrossPointConstants.CROSS_POINT_APP_REQUEST_APP_KEY)
						? Constants.FLG_USEREXTEND_USREX_SHOP_APP_MEMBER_FLAG_ON
						: Constants.FLG_USEREXTEND_USREX_SHOP_APP_MEMBER_FLAG_OFF;
				SessionManager.AppKeyForCrossPoint = null;
				SessionManager.MemberIdForCrossPoint = null;
				SessionManager.PinCodeForCrossPoint = null;
			}
			else
			{
				user.UserExtend.UserExtendDataValue[Constants.CROSS_POINT_USREX_SHOP_APP_MEMBER_FLAG] = Constants.FLG_USEREXTEND_USREX_SHOP_APP_MEMBER_FLAG_OFF;
			}
		}

		if　(w2.App.Common.Line.Constants.LINE_MINIAPP_OPTION_ENABLED)
		{
			user.UserExtend.UserExtendDataValue[Constants.SOCIAL_PROVIDER_ID_LINE] = SessionManager.LineProviderUserId;
		}

		string userId;
		var userService = new UserService();

		// 仮ユーザー本登録
		// HACK: ゲスト購入後の会員登録 または LINEミニアプリからの本登録
		if (IsToRegularAccountFlow())
		{
			userId = SessionManager.HasTemporaryUserId == false
				? (string)Session[Constants.SESSION_KEY_USER_REGIST_AFTER_ORDER_USER_ID]
				: SessionManager.TemporaryUserId;
			var updateUser = userService.Get(userId);

			updateUser.UserKbn = user.UserKbn;
			updateUser.Name = user.Name;
			updateUser.Name1 = user.Name1;
			updateUser.Name2 = user.Name2;
			updateUser.NameKana = user.NameKana;
			updateUser.NameKana1 = user.NameKana1;
			updateUser.NameKana2 = user.NameKana2;
			updateUser.NickName = user.NickName;
			updateUser.Birth = user.Birth;
			updateUser.BirthYear = user.BirthYear;
			updateUser.BirthMonth = user.BirthMonth;
			updateUser.BirthDay = user.BirthDay;
			updateUser.Sex = user.Sex;
			updateUser.MailAddr = user.MailAddr;
			updateUser.MailAddr2 = user.MailAddr2;
			updateUser.Zip = user.Zip;
			updateUser.Zip1 = user.Zip1;
			updateUser.Zip2 = user.Zip2;
			updateUser.Addr = user.Addr;
			updateUser.Addr1 = user.Addr1;
			updateUser.Addr2 = user.Addr2;
			updateUser.Addr3 = user.Addr3;
			updateUser.Addr4 = user.Addr4;
			updateUser.CompanyName = user.CompanyName;
			updateUser.CompanyPostName = user.CompanyPostName;
			updateUser.Tel1 = user.Tel1;
			updateUser.Tel1_1 = user.Tel1_1;
			updateUser.Tel1_2 = user.Tel1_2;
			updateUser.Tel1_3 = user.Tel1_3;
			updateUser.Tel2 = user.Tel2;
			updateUser.Tel2_1 = user.Tel2_1;
			updateUser.Tel2_2 = user.Tel2_2;
			updateUser.Tel2_3 = user.Tel2_3;
			updateUser.MailFlg = user.MailFlg;
			updateUser.RemoteAddr = user.RemoteAddr;
			updateUser.LoginId = user.LoginId;
			updateUser.Password = user.Password;
			updateUser.MemberRankId = user.MemberRankId;
			updateUser.UserManagementLevelId = user.UserManagementLevelId;
			updateUser.RecommendUid = user.RecommendUid;
			updateUser.LastChanged = Constants.FLG_LASTCHANGED_USER;
			updateUser.UserExtend = user.UserExtend;

			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				user.UserId = userId;
				CooperationCrossPointApi(user);
			}

			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var lineTemporaryService = new LineTemporaryUserService();
				var lineTemporaryUser = lineTemporaryService.GetByUserId(userId);
				if (lineTemporaryUser != null)
				{
					lineTemporaryService.UpdateToRegularAccount(lineTemporaryUser, Constants.FLG_LASTCHANGED_USER, accessor);
				}

				userService.UpdateWithUserExtend(updateUser, UpdateHistoryAction.DoNotInsert, accessor);

				accessor.CommitTransaction();
			}

			SetSocialLoginInfo(updateUser, UpdateHistoryAction.DoNotInsert);
		}
		else
		{
			// ログインIDの重複チェックを行い、重複する場合は入力画面に遷移
			if (userService.CheckDuplicationLoginId(user.LoginId, Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? "1" : "0") == false)
			{
				var url = this.IsRakutenIdConnectUserRegister
					? new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_INPUT)
						.AddParam(Constants.REQUEST_KEY_RAKUTEN_REGIST, Constants.FLG_RAKUTEN_USER_REGIST)
						.CreateUrl()
					: Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_INPUT;
				Response.Redirect(url);
			}

			// 登録
			userId = UserService.CreateNewUserId(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.NUMBER_KEY_USER_ID,
				Constants.CONST_USER_ID_HEADER,
				Constants.CONST_USER_ID_LENGTH);
			user.UserId = userId;
			user.LastChanged = Constants.FLG_LASTCHANGED_USER;

			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				CooperationCrossPointApi(user);
			}

			userService.InsertWithUserExtend(
				user,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert);

			//register GMO
			if ((Constants.PAYMENT_GMO_POST_ENABLED) && (this.IsUserAddrJp))
			{
				var userBusinessOwnerService = new UserBusinessOwnerService();
				if (this.UserInput.BusinessOwner != null)
				{
					var userBusinessOwnerInput = this.UserInput.BusinessOwner.CreateModel();
					userBusinessOwnerInput.ShopCustomerId = string.Format("{0}-F", user.UserId);
					if (userBusinessOwnerInput.RequestBudget > 0)
					{
						userBusinessOwnerInput.CreditStatus = Constants.FLG_BUSINESS_OWNER_CREDIT_STATUS_UNDER_REVIEW_EN;
					}
					userBusinessOwnerInput.UserId = user.UserId;
					var request = new GmoRequestFrameGuaranteeRegister(user, userBusinessOwnerInput);
					var gmo = new GmoTransactionApi();
					var result = gmo.FrameGuaranteeRegister(request);
					if (result.IsResultOk)
					{
						userBusinessOwnerService.Insert(userBusinessOwnerInput);
					}
				}
			}
			SetSocialLoginInfo(user, UpdateHistoryAction.DoNotInsert);
		}

		// Userのグローバル情報を更新
		RegionManager.GetInstance().UpdateUserRegion(new RegionModel(), userId);

		// DBから最新情報を取得
		var registedUser = userService.Get(userId);

		var userCooperationPlugin = new UserCooperationPlugin(Constants.FLG_LASTCHANGED_USER);
		if (Constants.USER_COOPERATION_ENABLED)
		{
			if (IsAfterGuestOrder())
			{
				userCooperationPlugin.Update(registedUser);
			}
			else
			{
				userCooperationPlugin.Regist(registedUser);
			}
		}

		// ユーザー登録イベント
		new UserRegister(
			IsAfterGuestOrder(),
			(List<w2.App.Common.Order.CartObject>)Session[Constants.SESSION_KEY_USER_REGIST_AFTER_ORDER_CART_LIST],
			Constants.SOCIAL_LOGIN_ENABLED ? (SocialLoginModel)Session[Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL] : null,
			Constants.AMAZON_LOGIN_OPTION_ENABLED ? (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL] : null,
			this.IsRakutenIdConnectUserRegister)
			.ExecProcessOnUserRegisted(registedUser, UpdateHistoryAction.DoNotInsert);

		// セッション情報のクリア
		Session.Remove(Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL);
		Session.Remove(Constants.SESSION_KEY_LOGIN_USER_LINE_ID);
		// Remove session user introduce code
		Session.Remove(Constants.SESSION_KEY_REFERRAL_CODE);
		// ターゲットページ設定を削除
		this.SessionParamTargetPage = null;

		// 更新履歴登録
		new UpdateHistoryService().InsertForUser(userId, Constants.FLG_LASTCHANGED_USER);

		// IPとアカウント、IPとパスワードでそれぞれアカウントロックがかかっている可能性がある。
		// そのため、登録時にアカウントロックキャンセルを行う
		LoginAccountLockManager.GetInstance().CancelAccountLock(
			this.Page.Request.UserHostAddress,
			user.LoginId,
			user.PasswordDecrypted);

		var loggedUser = this.IsVisible_UserPassword
			? userService.TryLogin(user.LoginId, user.PasswordDecrypted, Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED)
			: userService.Get(userId);
		if (loggedUser == null)
		{
			// ログイン失敗時はエラー画面に遷移
			Session[Constants.SESSION_KEY_ERROR_MSG] = GetLoginDeniedErrorMessage(userId, user.PasswordDecrypted);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// ログイン成功処理＆次の画面へ遷移
		ExecLoginSuccessProcessAndGoNextForUserRegister(
			loggedUser,
			UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// ソーシャルログイン情報情報セット
	/// </summary>
	/// <param name="user">ユーザー</param>
	/// <param name="updateHistoryAction">更新し歴アクション</param>
	private void SetSocialLoginInfo(UserModel user, UpdateHistoryAction updateHistoryAction)
	{
		if ((string.IsNullOrEmpty(Constants.PAYPAL_USEREXTEND_COLUMNNAME_CUSTOMER_ID) == false)
			&& (SessionManager.PayPalLoginResult != null))
		{
			PayPalUtility.Account.UpdateUserExtendForPayPal(
				user.UserId,
				SessionManager.PayPalLoginResult,
				updateHistoryAction);
		}

		if ((string.IsNullOrEmpty(Constants.SOCIAL_PROVIDER_ID_LINE) == false)
			&& (Constants.SOCIAL_LOGIN_ENABLED == false) 
			&& (string.IsNullOrEmpty(SessionManager.LineProviderUserId) == false))
		{
			LineUtil.UpdateUserExtendForLineUserId(user.UserId, 
				SessionManager.LineProviderUserId,
				UpdateHistoryAction.DoNotInsert);
		}
	}

	/// <summary>
	/// 戻るリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBack_Click(object sender, EventArgs e)
	{
		var user = this.UserInput.CreateModel();
		user.UserExtend = this.UserInput.UserExtendInput.CreateModel();

		if ((user.UserExtend != null)
			&& (user.UserExtend.UserExtendDataValue.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_NO))
			&& (user.UserExtend.UserExtendDataValue.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_PIN)
			&& (string.IsNullOrEmpty(user.UserExtend.UserExtendDataValue[Constants.CROSS_POINT_USREX_SHOP_CARD_NO]) == false)
			&& (string.IsNullOrEmpty(user.UserExtend.UserExtendDataValue[Constants.CROSS_POINT_USREX_SHOP_CARD_PIN]) == false)))
		{
			SessionManager.IsBackForCrossPoint = true;
		}

		//------------------------------------------------------
		// 会員登録ページへ遷移
		//------------------------------------------------------
		var url = this.IsRakutenIdConnectUserRegister
			? new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_INPUT)
				.AddParam(Constants.REQUEST_KEY_RAKUTEN_REGIST, Constants.FLG_RAKUTEN_USER_REGIST)
				.CreateUrl()
			: Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_INPUT;
		Response.Redirect(url);
	}

	/// <summary>
	/// Cooperation Cross Point api
	/// </summary>
	/// <param name="user">User model</param>
	private void CooperationCrossPointApi(UserModel user)
	{
		var apiResult = IsToRegularAccountFlow()
			? new CrossPointUserApiService().Update(user)
			: new CrossPointUserApiService().Insert(user);

		if (apiResult.IsSuccess == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = apiResult.ErrorCodeList.Contains(
					w2.App.Common.Constants.CROSS_POINT_RESULT_DUPLICATE_MEMBER_ID_ERROR_CODE)
				? apiResult.ErrorMessage
				: MessageManager.GetMessages(w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		if (new CrossPointUtility().IsCrossPointUser(this.UserInput.CreateModel()))
		{
			apiResult = new CrossPointUserApiService().Merge(
				user.UserId,
				user.UserExtend.UserExtendDataValue.CrossPointShopCardNo,
				user.UserExtend.UserExtendDataValue.CrossPointShopCardPin,
				user.UserExtend.UserExtendDataValue.CrossPointShopAppMemberIdFlag);
			if (apiResult.IsSuccess == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = MessageManager.GetMessages(
					w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);
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
		else
		{
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

	/// <summary>
	/// 本会員への更新フロー判定
	/// </summary>
	/// <returns>本会員への更新フローか</returns>
	private bool IsToRegularAccountFlow()
	{
		var isAfterGuestOrder = IsAfterGuestOrder();
		var hasTemporaryUserId = SessionManager.HasTemporaryUserId;

		return isAfterGuestOrder || hasTemporaryUserId;
	}

	/// <summary>
	/// ゲスト購入後の会員登録判定
	/// </summary>
	/// <returns>ゲスト購入後の会員登録か</returns>
	private bool IsAfterGuestOrder()
	{
		var afterOrderUserId = StringUtility.ToEmpty((string)Session[Constants.SESSION_KEY_USER_REGIST_AFTER_ORDER_USER_ID]);
		return string.IsNullOrEmpty(afterOrderUserId) == false;
	}

	/// <summary>ユーザーバリューオブジェクト</summary>
	protected UserInput UserInput { get { return (UserInput)Session[Constants.SESSION_KEY_PARAM]; } }
	/// <summary>パスワード表示フラグ</summary>
	protected bool IsVisible_UserPassword
	{
		get { return (bool)ViewState["UserPassword"]; }
		set { ViewState["UserPassword"] = value; }
	}
	/// <summary>ユーザーの住所が日本か</summary>
	public bool IsUserAddrJp
	{
		get { return GlobalAddressUtil.IsCountryJp(this.UserAddrCountryIsoCode); }
	}
	/// <summary>ユーザーの住所国ISOコード</summary>
	public string UserAddrCountryIsoCode
	{
		get { return this.UserInput.AddrCountryIsoCode; }
	}
}
