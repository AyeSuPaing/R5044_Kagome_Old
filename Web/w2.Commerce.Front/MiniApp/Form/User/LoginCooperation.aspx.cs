/*
=========================================================================================================
  Module      : LINEミニアプリログイン連携画面(LoginCooperation.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.User;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.LineTemporaryUser;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

public partial class MiniApp_User_LoginCooperation : LineMiniAppPage
{
	#region ラップ済みコントロール
	private WrappedTextBox WtbLoginId { get { return GetWrappedControl<WrappedTextBox>("tbLoginId"); } }
	private WrappedTextBox WtbPassword { get { return GetWrappedControl<WrappedTextBox>("tbPassword"); } }
	private WrappedHtmlGenericControl WdvCooperateError { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvCooperateError"); } }
	private WrappedLiteral WlCooperateError { get { return GetWrappedControl<WrappedLiteral>("lCooperateError"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	/// <summary>
	/// アカウント連携ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCooperate_OnClick(object sender, EventArgs e)
	{
		var loginId = this.WtbLoginId.Text.Trim();
		var password = this.WtbPassword.Text.Trim();

		if (LoginAccountLockManager.GetInstance().IsAccountLocked(Request.UserHostAddress, loginId, password))
		{
			DisplayCooperateError(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_W2_EXISTING_USER_COOPERATION_ACCOUNT_LOCK));
			return;
		}

		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			var userService = new UserService();
			var user = userService.TryLogin(loginId, password, Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED);
			if (user == null)
			{
				var errorMessage = this.Process.GetLoginDeniedErrorMessage(loginId, password, true);
				DisplayCooperateError(errorMessage);
				return;
			}

			var existsUsers = userService.GetUsersByExtendColumn(
				Constants.SOCIAL_PROVIDER_ID_LINE,
				this.LoginLineUserId,
				accessor);
			if (existsUsers.Any(existsUser => existsUser.UserId != this.LoginUser.UserId))
			{
				DisplayCooperateError(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_COOPERATE_WITH_SOMEONE));
				return;
			}

			var userExtend = userService.GetUserExtend(user.UserId, accessor);
			userExtend.UserExtendDataValue[Constants.SOCIAL_PROVIDER_ID_LINE] = this.LoginLineUserId;
			userService.UpdateUserExtend(
				userExtend,
				user.UserId,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert,
				accessor);

			var lineTemporaryUserService = new LineTemporaryUserService();
			lineTemporaryUserService.UpdateToRegularAccount(this.LineTempUser, Constants.FLG_LASTCHANGED_USER, accessor);

			this.LoginUser = userService.Get(user.UserId, accessor);
			this.LineTempUser = lineTemporaryUserService.Get(this.LineTempUser.LineUserId, accessor);

			accessor.CommitTransaction();
		}

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_LINE_MINIAPP_TOP);
	}

	/// <summary>
	/// アカウント連携エラー表示
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	private void DisplayCooperateError(string errorMessage)
	{
		this.WlCooperateError.Text = HtmlSanitizer.HtmlEncodeChangeToBr(errorMessage);
		this.WdvCooperateError.Visible = true;
	}

	/// <summary>
	/// 新規会員登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRegist_OnClick(object sender, EventArgs e)
	{
		SessionManager.LineProviderUserId = this.LoginLineUserId;
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_REGULATION)
			.AddParam(Constants.REQUEST_KEY_NEXT_URL, Constants.PATH_ROOT + Constants.PAGE_LINE_MINIAPP_TOP)
			.AddParam(Constants.REQUEST_KEY_TEMPORARY_USER_ID, this.LineTempUser.TemporaryUserId);

		var url = urlCreator.CreateUrl();
		Response.Redirect(url);
	}
}
