/*
=========================================================================================================
  Module      : リマインダー入力画面処理(PasswordReminderInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.Common.Web;
using w2.App.Common;
using w2.App.Common.Global.Region;
using w2.App.Common.User;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.User;

public partial class Form_User_PasswordReminderInput : BasePage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス

	# region ラップ済みコントロール宣言
	WrappedTextBox WtbLoginId { get { return GetWrappedControl<WrappedTextBox>(Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? "tbMailAddr" : "tbLoginId"); } }
	WrappedTextBox WtbMailAddr { get { return GetWrappedControl<WrappedTextBox>("tbMailAddr"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// HTTPS通信チェック（HTTPのとき、トップ画面へ）
		//------------------------------------------------------
		CheckHttps();

		//------------------------------------------------------
		// ログインチェック（ログイン済みの場合、トップ画面へ）
		//------------------------------------------------------
		if (this.IsLoggedIn)
		{
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT);
		}

		if (!IsPostBack)
		{
#if DEBUG		
			//------------------------------------------------------
			// デバッグの場合初期値設定
			//------------------------------------------------------
			this.WtbLoginId.Text = "ochiai";
			this.WtbMailAddr.Text = string.Format("bh+{0}@w2s.xyz", DateTime.Now.ToString("yyMMddHHmmss"));
#endif
		}
	}

	/// <summary>
	/// 送信リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSend_Click(object sender, EventArgs e)
	{
		// ユーザー情報取得
		var userInfo = CreateInputData();
		// ユーザー情報検証
		var errorMessages = userInfo.Validate(UserInput.EnumUserInputValidationKbn.PasswordReminderInput);
		// エラーが無い場合
		if (errorMessages.Count != 0)
		{
			// カスタムバリデータにメッセージを表示させる
			GetErrMsgAndFocusToCV(errorMessages);
			return;
		}

		// 問題なければ次画面に遷移する
		GoNext(userInfo);
	}

	/// <summary>
	/// 次画面へ遷移
	/// </summary>
	private void GoNext(UserInput userInfo)
	{
		var userService = new UserService();
		var user = userService.GetUserForReminder(userInfo.LoginId, userInfo.MailAddr, Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED);

		// ユーザが見つかった場合はメール送信
		if (user != null)
		{
			// 楽天IDConnectで会員登録したユーザーの場合は対象としない
			// ※VFからユーザー取得した時に本情報を取得できないため、W2側から再取得
			if (Constants.RAKUTEN_LOGIN_ENABLED)
			{
				var userExtend = userService.GetUserExtend(user.UserId);
				if ((userExtend.UserExtendDataValue[Constants.RAKUTEN_ID_CONNECT_REGISTER_USER] == Constants.FLG_USER_RAKUTEN_ID_CONNECT_REGISTER_USER_ON))
				{
					this.ErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PASSWORD_REMINDER_NO_USER);
					return;
				}
			}

			// ユーザーIDと現在時刻を繋げたものを暗号化し、認証キーとする
			w2.Common.Util.Security.RijndaelCrypto rcAuthenticationKey
				= new w2.Common.Util.Security.RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
			string authenticationKey = rcAuthenticationKey.Encrypt(user.UserId + DateTime.Now.ToString("yyyyMMdd"));

			// パスワードリマインダー情報挿入
			userService.DeleteInsertPasswordReminder(user.UserId, authenticationKey, Constants.FLG_PASSWORDREMINDER_CHANGE_TRIAL_LIMIT_COUNT_DEFAULT, DateTime.Now);

			var userName = user.ComplementUserName;

			// メール送信
			SendRemindMail(
				authenticationKey,
				user.UserId,
				user.MailAddr,
				userName,
				user.UserManagementLevelId,
				RegionManager.GetInstance().Region.LanguageCode,
				RegionManager.GetInstance().Region.LanguageLocaleId);
		}

		//------------------------------------------------------
		// 画面遷移
		//------------------------------------------------------
		// メールアドレスをセッションへ格納（完了ページ表示用）
		Session[Constants.SESSION_KEY_PARAM] = this.WtbMailAddr.Text;

		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_PASSWORD_REMINDER_COMPLETE);
		if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_NEXT_URL]) == false) url.AddParam(Constants.REQUEST_KEY_NEXT_URL, Request[Constants.REQUEST_KEY_NEXT_URL]);
		// パスワードリマインダ完了ページへ
		Response.Redirect(url.CreateUrl());
	}

	/// <summary>
	/// リマインダーメール送信
	/// </summary>
	/// <param name="strAuthenticationKey">暗号化キー</param>
	/// <param name="userId">ユーザーID</param>
	/// <param name="mailAddr">メールアドレス</param>
	/// <param name="userName">ユーザー名</param>
	/// <param name="managementLevel">ユーザー管理レベルID</param>
	/// <param name="languageCode">言語コード</param>
	/// <param name="languageLocaleId">言語ロケールID</param>
	private void SendRemindMail(string strAuthenticationKey, string userId, string mailAddr, string userName, string managementLevel, string languageCode, string languageLocaleId)
	{
		// メール送信先決定（PC or Mobile）
		bool blIsPC = (StringUtility.ToHankaku(this.WtbMailAddr.Text).ToLower().Trim() == mailAddr.ToLower());

		// パスワード変更画面URL作成
		StringBuilder sbPasswordRemainderUrl = new StringBuilder();
		sbPasswordRemainderUrl.Append(this.SecurePageProtocolAndHost).Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_PASSWORD_MODIFY_INPUT);
		sbPasswordRemainderUrl.Append("?").Append(Constants.REQUEST_KEY_AUTHENTICATION_KEY).Append("=").Append(HttpUtility.UrlEncode(strAuthenticationKey));

		Hashtable htMailTempInput = new Hashtable();
		htMailTempInput.Add(Constants.FIELD_USER_NAME, userName);
		htMailTempInput.Add("url", sbPasswordRemainderUrl.ToString());
		htMailTempInput.Add("validtime",
			DateTimeUtility.ToStringFromRegion(DateTime.Now.AddMinutes(Constants.CONST_PASSWORDREMAINDER_VALID_MINUTES),
				DateTimeUtility.FormatType.LongDateHourMinute1Letter));
		htMailTempInput.Add(Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID, managementLevel);

		// メール送信
		using (MailSendUtility msMailSend = new MailSendUtility(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.CONST_MAIL_ID_PASSWORD_REMINDER,
			userId,
			htMailTempInput,
			blIsPC,
			Constants.MailSendMethod.Auto,
			languageCode,
			languageLocaleId,
			this.WtbMailAddr.Text))
		{
			msMailSend.AddTo(this.WtbMailAddr.Text);

			if (msMailSend.SendMail() == false)
			{
				AppLogger.WriteError(this.GetType().BaseType.ToString() + " : " + msMailSend.MailSendException.ToString());
			}
		}
	}

	/// <summary>
	/// 入力値を取得してUserInputに格納
	/// </summary>
	private UserInput CreateInputData()
	{
		var userInput = new UserInput(new UserModel());
		userInput.MailAddr = StringUtility.ToHankaku(this.WtbMailAddr.Text);
		userInput.LoginId = (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? userInput.MailAddr : StringUtility.ToHankaku(this.WtbLoginId.Text));

		return userInput;
	}

	/// <summary>
	///  エラーメッセージをカスタムバリデータにセットしてフォーカス
	/// </summary>
	/// <param name="errorMessages">エラーメッセージ一覧</param>
	private void GetErrMsgAndFocusToCV(Dictionary<string, string> errorMessages)
	{
		// カスタムバリデータ取得
		var customValidators = new List<CustomValidator>();
		CreateCustomValidators(this, customValidators);

		// エラーをカスタムバリデータへ
		SetControlViewsForError("PasswordReminderInput", errorMessages, customValidators);

		// エラーフォーカス
		// HACK:JSでおｋ（その方がお客さんも幸せでは？）
		foreach (var customValidator in customValidators)
		{
			var webControl = (WebControl)customValidator.Parent.FindControl(customValidator.ControlToValidate);
			if (webControl != null)
			{
				if (customValidator.IsValid == false)
				{
					webControl.Focus();
					break;
				}
				else
				{
					customValidator.ErrorMessage = "";
					webControl.CssClass = webControl.CssClass.Replace(Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING, "");
				}
			}
		}
	}

	/// <summary>エラーメッセージ</summary>
	protected string ErrorMessage
	{
		get { return (string)ViewState["ErrorMessage"]; }
		private set { ViewState["ErrorMessage"] = value; }
	}
}
