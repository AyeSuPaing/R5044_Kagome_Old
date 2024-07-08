/*
=========================================================================================================
  Module      : シングルサインオン画面(SingleSignOn.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Text;
using w2.App.Common.CrossPoint.User;
using w2.Common.Util;
using w2.Domain.LineTemporaryUser;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

public partial class MiniApp_SingleSignOn : LineMiniAppPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	/// <summary>
	/// ログインボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnLogin_OnClick(object sender, EventArgs e)
	{
		// 開発環境ではLINEミニアプリを使用できないため、データのみ再現する
		if (this.IsDevelop)
		{
			CorrectLineUserIdForDebug();
		}

		var lineUserId = hfLineUserId.Value;
		var user = new UserService().GetByExtendColumn(Constants.SOCIAL_PROVIDER_ID_LINE, lineUserId);
		if (user == null)
		{
			var input = new LineTemporaryUserInput(lineUserId);
			var errorMessage = input.Validate();
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				RedirectErrorPage(errorMessage);
			}

			user = RegistTemporaryUser(input);
		}

		this.LineTempUser = new LineTemporaryUserService().Get(lineUserId);
		ExecLogin(user, this.LineTempUser.IsRegularUser);
	}

	/// <summary>
	/// エラーボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnError_OnClick(object sender, EventArgs e)
	{
		RedirectErrorPage();
	}

	/// <summary>
	/// 仮会員登録
	/// </summary>
	/// <param name="input">LINE仮会員入力情報</param>
	/// <returns>仮会員後ユーザー</returns>
	private UserModel RegistTemporaryUser(LineTemporaryUserInput input)
	{
		var temporaryUser = input.CreateModel();
		temporaryUser.LastChanged = Constants.FLG_LASTCHANGED_USER;

		var userInput = new UserInput(new UserModel())
		{
			UserExtendInput = new UserExtendInput(),
		};
		userInput.UserExtendInput.UserExtendColumns.Add(Constants.SOCIAL_PROVIDER_ID_LINE);
		userInput.UserExtendInput.UserExtendDataText.Add(Constants.SOCIAL_PROVIDER_ID_LINE, input.LineUserId);
		userInput.UserExtendInput.UserExtendDataValue.Add(Constants.SOCIAL_PROVIDER_ID_LINE, input.LineUserId);

		var registedUser = userInput.CreateModel();
		registedUser.UserExtend = userInput.UserExtendInput.CreateModel();

		// ユーザーID設定
		var userId = UserService.CreateNewUserId(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.NUMBER_KEY_USER_ID,
			Constants.CONST_USER_ID_HEADER,
			Constants.CONST_USER_ID_LENGTH);
		temporaryUser.TemporaryUserId = userId;
		registedUser.UserId = userId;
		registedUser.UserExtend.UserId = userId;

		if (Constants.CROSS_POINT_OPTION_ENABLED)
		{
			CooperationCrossPointApi(registedUser);
		}

		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			// ユーザー情報登録
			new LineTemporaryUserService().Insert(temporaryUser, accessor);

			var userService = new UserService();
			userService.InsertWithUserExtend(registedUser, Constants.FLG_LASTCHANGED_USER, UpdateHistoryAction.DoNotInsert);

			// 更新履歴登録
			new UpdateHistoryService().InsertForUser(userId, Constants.FLG_LASTCHANGED_USER);

			accessor.CommitTransaction();

			var user = userService.Get(userId, accessor);
			return user;
		}
	}

	/// <summary>
	/// CROSS POINT登録処理
	/// </summary>
	/// <param name="user">ユーザーモデル</param>
	private void CooperationCrossPointApi(UserModel user)
	{
		var apiService = new CrossPointUserApiService();
		var insertResult = apiService.Insert(user);
		if (insertResult.IsSuccess == false)
		{
			var errorMessage = insertResult.ErrorCodeList.Contains(Constants.CROSS_POINT_RESULT_DUPLICATE_MEMBER_ID_ERROR_CODE)
				? insertResult.ErrorMessage
				: MessageManager.GetMessages(Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);
			RedirectErrorPage(errorMessage);
		}

		var apiUser = apiService.Get(user.UserId);
		if (apiUser == null) RedirectErrorPage();

		if (user.UserExtend.UserExtendColumns.Contains(Constants.CROSS_POINT_USREX_SHOP_CARD_NO) == false)
		{
			user.UserExtend.UserExtendColumns.Add(Constants.CROSS_POINT_USREX_SHOP_CARD_NO);
		}
		if (user.UserExtend.UserExtendColumns.Contains(Constants.CROSS_POINT_USREX_SHOP_CARD_PIN) == false)
		{
			user.UserExtend.UserExtendColumns.Add(Constants.CROSS_POINT_USREX_SHOP_CARD_PIN);
		}
		if (user.UserExtend.UserExtendColumns.Contains(Constants.CROSS_POINT_USREX_SHOP_APP_MEMBER_FLAG) == false)
		{
			user.UserExtend.UserExtendColumns.Add(Constants.CROSS_POINT_USREX_SHOP_APP_MEMBER_FLAG);
		}

		user.UserExtend.UserExtendDataValue.CrossPointShopCardNo = apiUser.RealShopCardNo;
		user.UserExtend.UserExtendDataValue.CrossPointShopCardPin = apiUser.PinCode;
		user.UserExtend.UserExtendDataValue.CrossPointShopAppMemberIdFlag = Constants.FLG_USEREXTEND_USREX_SHOP_APP_MEMBER_FLAG_OFF;
	}

	/// <summary>
	/// ログイン処理
	/// </summary>
	/// <param name="user">ユーザー情報</param>
	/// <param name="isRegularUser">本会員登録済みか</param>
	private void ExecLogin(UserModel user, bool isRegularUser)
	{
		var nextUrl = isRegularUser
			? Constants.PATH_ROOT + Constants.PAGE_LINE_MINIAPP_TOP
			: Constants.PATH_ROOT + Constants.PAGE_LINE_MINIAPP_LOGIN_COOPERATION;
		this.Process.ExecLoginSuccessProcessAndGoNextForLogin(
			user,
			nextUrl,
			false,
			LoginType.Line,
			UpdateHistoryAction.Insert);
	}

	#region 開発環境用
	/// <summary>
	/// デバッグ用LINEユーザーID取得
	/// </summary>
	private void CorrectLineUserIdForDebug()
	{
		var physicalDirPathMiniAppDebug = Path.Combine(
			Constants.PHYSICALDIRPATH_FRONT_PC,
			"MiniApp",
			"Debug");
		if (Directory.Exists(physicalDirPathMiniAppDebug) == false)
		{
			Directory.CreateDirectory(physicalDirPathMiniAppDebug);
		}

		var physicalFilePathLineUserId = Path.Combine(
			physicalDirPathMiniAppDebug,
			"LineUserId.tmp");

		string temporaryLineUserId;
		if (File.Exists(physicalFilePathLineUserId))
		{
			using (var reader = new StreamReader(physicalFilePathLineUserId, Encoding.UTF8))
			{
				temporaryLineUserId = reader.ReadLine();
			}
		}
		else
		{
			temporaryLineUserId = Guid.NewGuid().ToString("N");
			using (var writer = new StreamWriter(physicalFilePathLineUserId, false, Encoding.UTF8))
			{
				writer.Write(temporaryLineUserId);
			}
		}

		hfLineUserId.Value = temporaryLineUserId;
	}
	#endregion

	/// <summary>開発環境か</summary>
	/// <remarks>ループバックアドレスの場合、Liff連携が出来ないためデバッグ判定に利用する</remarks>
	protected bool IsDevelop
	{
		get { return Constants.SITE_DOMAIN == "localhost"; }
	}
}