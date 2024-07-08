/*
=========================================================================================================
  Module      : パスワード変更入力画面処理(PasswordModifyInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.CrossPoint.Point;
using w2.App.Common.CrossPoint.User;
using w2.App.Common.Option.CrossPoint;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.App.Common.User;
using w2.App.Common.Web.WrappedContols;
using w2.Common;
using w2.Common.Util;

public partial class Form_User_PasswordModifyInput : BasePage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス

	#region ラップ済みコントロール宣言
	WrappedTextBox WtbBirth { get { return GetWrappedControl<WrappedTextBox>("tbBirth"); } }
	WrappedCustomValidator WcvBirth { get { return GetWrappedControl<WrappedCustomValidator>("cvBirth"); } }
	WrappedTextBox WtbTel1_1 { get { return GetWrappedControl<WrappedTextBox>("tbTel1_1"); } }
	WrappedTextBox WtbTel1_2 { get { return GetWrappedControl<WrappedTextBox>("tbTel1_2"); } }
	WrappedTextBox WtbTel1_3 { get { return GetWrappedControl<WrappedTextBox>("tbTel1_3"); } }
	WrappedCustomValidator WcvTel1_1 { get { return GetWrappedControl<WrappedCustomValidator>("cvTel1_1"); } }
	WrappedCustomValidator WcvTel1_2 { get { return GetWrappedControl<WrappedCustomValidator>("cvTel1_2"); } }
	WrappedCustomValidator WcvTel1_3 { get { return GetWrappedControl<WrappedCustomValidator>("cvTel1_3"); } }
	WrappedTextBox WtbPassword { get { return GetWrappedControl<WrappedTextBox>("tbPassword"); } }
	WrappedCustomValidator WcvPassword { get { return GetWrappedControl<WrappedCustomValidator>("cvPassword"); } }
	WrappedTextBox WtbPasswordConf { get { return GetWrappedControl<WrappedTextBox>("tbPasswordConf"); } }
	WrappedCustomValidator WcvPasswordConf { get { return GetWrappedControl<WrappedCustomValidator>("cvPasswordConf"); } }
	WrappedTextBox WtbTel1 { get { return GetWrappedControl<WrappedTextBox>("tbTel1"); } }
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

		//------------------------------------------------------
		// 本人認証
		//------------------------------------------------------
		var userService = new UserService();
		this.PasswordReminder = userService.GetPasswordReminder(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_AUTHENTICATION_KEY]));

		// パスワードリマインダー情報が取得出来ない場合は、そもそもパスワード変更をさせないためエラー画面へ遷移
		if (this.PasswordReminder == null)
		{
			ReditectErrorPage(WebMessages.ERRMSG_FRONT_PASSWORDREMINDER_AUTHENTICATION_NO_KEY);
		}

		// 取得したパスワードリマインダー情報が有効期限外の場合、パスワードリマインダー情報を削除し、エラー画面へ遷移
		if (this.PasswordReminder.DateCreated.AddMinutes(Constants.CONST_PASSWORDREMAINDER_VALID_MINUTES) < DateTime.Now)
		{
			userService.DeletePasswordReminder(this.PasswordReminder.UserId);
			ReditectErrorPage(WebMessages.ERRMSG_FRONT_PASSWORDREMINDER_AUTHENTICATION_NO_KEY);
		}

		// 試行可能回数が0以下の場合は、パスワードリマインダー情報を削除し、エラー画面へ遷移する
		if (this.PasswordReminder.ChangeTrialLimitCount <= 0)
		{
			userService.DeletePasswordReminder(this.PasswordReminder.UserId);
			ReditectErrorPage(WebMessages.ERRMSG_FRONT_PASSWORDREMINDER_CHANGE_TRIAL_LIMIT_COUNT_OVER);
		}

		//------------------------------------------------------
		// 初期表示
		//------------------------------------------------------
		if (!IsPostBack)
		{
			var user = userService.Get(this.PasswordReminder.UserId);
			// ログインID設定
			this.LoginId = user.LoginId;
			this.EasyRegisterFlg = user.EasyRegisterFlg;
		}
	}

	/// <summary>
	/// 変更リンクボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbModify_Click(object sender, System.EventArgs e)
	{
		// 入力情報取得
		PasswordModifyInput input = GetInput();
		var userService = new UserService();

		// 入力チェック
		Dictionary<string, string> dicErrorMessages = input.Validate();

		if (dicErrorMessages.Count > 0)
		{
			// カスタムバリデータ取得
			List<CustomValidator> lCustomValidators = new List<CustomValidator>();
			CreateCustomValidators(this, lCustomValidators);

			// エラーをカスタムバリデータへ
			SetControlViewsForError("PasswordModify", dicErrorMessages, lCustomValidators);

			// 変更可能回数制御
			if (dicErrorMessages.ContainsKey(Constants.FIELD_USER_BIRTH)
				|| dicErrorMessages.ContainsKey(Constants.FIELD_USER_TEL1)
				|| dicErrorMessages.ContainsKey(Constants.FIELD_USER_TEL1_1)
				|| dicErrorMessages.ContainsKey(Constants.FIELD_USER_TEL1_2)
				|| dicErrorMessages.ContainsKey(Constants.FIELD_USER_TEL1_3))
			{
				// 変更可能回数を１減らす
				input.ChangeTrialLimitCount -= 1;

				// 変更試行可能回数をDBに設定
				userService.UpdateChangeUserPasswordTrialLimitCount(input.UserId, input.ChangeTrialLimitCount);

				// 変更試行可能回数が0以下の場合、エラーページに遷移
				if (input.ChangeTrialLimitCount <= 0)
				{
					// パスワードリマインダー情報削除
					userService.DeletePasswordReminder(input.UserId);
					ReditectErrorPage(WebMessages.ERRMSG_FRONT_PASSWORDREMINDER_CHANGE_TRIAL_LIMIT_COUNT_OVER);
				}

				this.PasswordReminder.ChangeTrialLimitCount = input.ChangeTrialLimitCount;
			}

			return;
		}

		// ユーザーパスワード変更とパスワードリマインダー情報削除も行う
		userService.UpdateUserAndDeletePasswordReminder(
			input.UserId,
			input.Password,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.DoNotInsert);

		if (Constants.USER_COOPERATION_ENABLED)
		{
			var user = userService.Get(input.UserId);
			var userCooperationPlugin = new UserCooperationPlugin(Constants.FLG_LASTCHANGED_USER);
			// ユーザー情報の編集イベント
			userCooperationPlugin.Update(user);
		}
		if (Constants.CROSS_POINT_OPTION_ENABLED)
		{
			var apiResult = new CrossPointUserApiService().UpdateUserPassword(input.UserId, input.Password);

			if (apiResult.IsSuccess == false)
			{
				var errorMessage = apiResult.ErrorCodeList.Contains(
						Constants.CROSS_POINT_RESULT_DUPLICATE_MEMBER_ID_ERROR_CODE)
					? apiResult.ErrorMessage
					: MessageManager.GetMessages(Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

				throw new w2Exception(errorMessage);
			}
		}

		// 更新履歴登録
		new UpdateHistoryService().InsertForUser(input.UserId, Constants.FLG_LASTCHANGED_USER);

		// ユーザーがロック中のパスワードに変更した場合の対策
		// パスワード変更ユーザーのアカウントロックを解除
		LoginAccountLockManager.GetInstance().CancelAccountLock(
			Request.UserHostAddress, this.LoginId, input.Password);

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_PASSWORD_MODIFY_COMPLETE);
	}

	/// <summary>
	///  入力値取得
	/// </summary>
	/// <returns></returns>
	private PasswordModifyInput GetInput()
	{
		PasswordModifyInput input = new PasswordModifyInput();
		var user = new UserService().Get(this.PasswordReminder.UserId);

		// 生年月日による本人確認の場合
		if (Constants.PASSWORDRIMINDER_AUTHITEM == Constants.PasswordReminderAuthItem.Birth)
		{
			input.Birth = StringUtility.ToHankaku(this.WtbBirth.Text);
			input.RegistedBirth = StringUtility.ToDateString(user.Birth, "yyyyMMdd");
		}
		// 電話番号による本人確認の場合
		else if (Constants.PASSWORDRIMINDER_AUTHITEM == Constants.PasswordReminderAuthItem.Tel)
		{
			// Set value for telephone
			if (this.WtbTel1_1.HasInnerControl == false)
			{
				input.IsTelShortInput = true;
				input.Tel1 = StringUtility.ToHankaku(this.WtbTel1.Text);
				input.RegistedTel1 = StringUtility.ReplaceDelimiter(user.Tel1);
				input.RegistedTel2 = StringUtility.ReplaceDelimiter(user.Tel2);
			}
			else
			{
				input.Tel1_1 = StringUtility.ToHankaku(this.WtbTel1_1.Text);
				input.Tel1_2 = StringUtility.ToHankaku(this.WtbTel1_2.Text);
				input.Tel1_3 = StringUtility.ToHankaku(this.WtbTel1_3.Text);
				input.RegistedTel1_1 = user.Tel1_1;
				input.RegistedTel1_2 = user.Tel1_2;
				input.RegistedTel1_3 = user.Tel1_3;
				input.RegistedTel2_1 = user.Tel2_1;
				input.RegistedTel2_2 = user.Tel2_2;
				input.RegistedTel2_3 = user.Tel2_3;
				input.IsTelShortInput = false;
			}
		}

		input.UserId = user.UserId;
		input.LoginId = user.LoginId;
		input.ChangeTrialLimitCount = this.PasswordReminder.ChangeTrialLimitCount;
		input.Password = StringUtility.ToHankaku(this.WtbPassword.Text);
		input.PasswordConf = StringUtility.ToHankaku(this.WtbPasswordConf.Text);
		input.EasyRegisterFlg = user.EasyRegisterFlg;
		return input;
	}

	/// <summary>
	/// エラーページ遷移
	/// </summary>
	/// <param name="strErrorMessageKey">エラーメッセージキー</param>
	private void ReditectErrorPage(string strErrorMessageKey)
	{
		// エラーメッセージ設定
		Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(strErrorMessageKey);

		// 遷移先URL作成
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_ERROR);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_ERRORPAGE_KBN).Append("=").Append(Constants.KBN_REQUEST_ERRORPAGE_GOTOP);

		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>パスワードリマインダー情報</summary>
	protected PasswordReminderModel PasswordReminder { get; set; }

	/// <summary>ログインID</summary>
	protected string LoginId
	{
		get { return (string)ViewState["LoginId"]; }
		private set { ViewState["LoginId"] = value; }
	}
	/// <summary>かんたん会員フラグ</summary>
	protected string EasyRegisterFlg
	{
		get { return (string)ViewState["EasyRegisterFlg"]; }
		private set { ViewState["EasyRegisterFlg"] = value; }
	}
}
