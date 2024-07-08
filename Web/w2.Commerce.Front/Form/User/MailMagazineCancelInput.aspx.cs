/*
=========================================================================================================
  Module      : メールマガジン解除入力画面処理(MailMagazineCancelInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.App.Common;
using w2.App.Common.Web.WrappedContols;
using w2.App.Common.CrossPoint.User;
using w2.Common.Util;

public partial class Form_User_MailMagazineCancelInput : BasePage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス

	#region ラップ済コントロール宣言
	WrappedTextBox WtbMailAddr { get { return GetWrappedControl<WrappedTextBox>("tbMailAddr"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// HTTPS通信チェック（HTTPの場合、HTTPSで再読込）
		//------------------------------------------------------
		CheckHttps(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MAILMAGAZINE_CANCEL_INPUT);

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 初期設定
			//------------------------------------------------------
			// ログインメールアドレス設定
			this.WtbMailAddr.Text = this.LoginUserMail;
		}
	}

	/// <summary>
	/// 解除するリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCancelClick(object sender, EventArgs e)
	{
		// 入力情報取得
		UserInput userInfo = CreateInputData();
		// 入力チェック
		Dictionary<string, string> errorMessages = userInfo.Validate(UserInput.EnumUserInputValidationKbn.MailMagazineCancel);
		if (errorMessages.Count != 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = string.Join("<br/>", errorMessages.Select(error => error.Value));
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		//------------------------------------------------------
		// メールマガジン解除対象のメールアドレスチェック
		//------------------------------------------------------
		var userService = new UserService();
		var user = userService.GetUserByMailAddrForMailMagazineCancel(userInfo.MailAddr);
		// ユーザー情報が存在しない?
		if (user == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_MAILMAGAZINE_CANCEL_NO_MAILADDR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		//------------------------------------------------------
		// メールマガジン解除
		//------------------------------------------------------
		// 更新は入力値のメールアドレスを元に更新する
		bool updateResult = userService.MailMagazineCancel(
			user.UserId,
			userInfo.MailAddr,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);

		// メールマガジン解除エラーの場合
		if (updateResult == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_MAILMAGAZINE_CANCEL_NO_USER);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		if (Constants.CROSS_POINT_OPTION_ENABLED)
		{
			var result = new CrossPointUserApiService().Update(userService.GetUserByMailAddrForMailMagazineCancel(userInfo.MailAddr));
			if (result.IsSuccess == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = MessageManager.GetMessages(
				w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
		}

		//------------------------------------------------------
		// メールマガジン解除メール送信
		//------------------------------------------------------
		Hashtable input = new Hashtable
		{
			{Constants.FIELD_USER_MAIL_ADDR, userInfo.MailAddr},
			{Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID, user.UserManagementLevelId},
		};
		using (MailSendUtility msMailSend = new MailSendUtility(
			Constants.CONST_DEFAULT_SHOP_ID, 
			Constants.CONST_MAIL_ID_MAILMAGAZINE_CANCEL,
			user.UserId,
			input,
			true,
			Constants.MailSendMethod.Auto,
			user.DispLanguageCode,
			user.DispLanguageLocaleId,
			this.WtbMailAddr.Text))
		{
			msMailSend.AddTo(this.WtbMailAddr.Text);
			if (msMailSend.SendMail() == false)
			{
				// エラーログ出力
				AppLogger.WriteError(this.GetType().BaseType.ToString() + " : " + msMailSend.MailSendException.ToString());
			}
		}

		//------------------------------------------------------
		// メールマガジン解除完了ページへ
		//------------------------------------------------------
		// メールアドレスをセッションへ格納（完了ページ表示用）
		Session[Constants.SESSION_KEY_PARAM] = this.WtbMailAddr.Text;

		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_MAILMAGAZINE_CANCEL_COMPLETE);
		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	/// 入力値を取得してValueObjectに格納
	/// </summary>
	private UserInput CreateInputData()
	{
		return new UserInput
		{
			MailAddr = StringUtility.ToHankaku(this.WtbMailAddr.Text),
		};
	}
}
