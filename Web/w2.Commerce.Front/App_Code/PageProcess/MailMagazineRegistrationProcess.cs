/*
=========================================================================================================
  Module      : Mail Magazine Registration Process (MailMagazineRegistrationProcess.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Web;
using System.Web.UI;
using w2.App.Common;
using w2.App.Common.CrossPoint.User;
using w2.App.Common.Global.Region;
using w2.Common.Util;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

/// <summary>
/// Mail Magazine Registration Process
/// </summary>
public partial class MailMagazineRegistrationProcess : BasePageProcess
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="caller">呼び出し元</param>
	/// <param name="viewState">ビューステート</param>
	/// <param name="context">コンテキスト</param>
	public MailMagazineRegistrationProcess(object caller, StateBag viewState, HttpContext context)
		: base(caller, viewState, context)
	{
	}

	/// <summary>
	/// Execute regist user mail magazine process
	/// </summary>
	/// <param name="userInput">User input</param>
	public void ExecRegistUserMailMagazine(UserInput userInput)
	{
		var newUser = userInput.CreateModel();
		var userId = string.Empty;
		var managementLevel = Constants.FLG_USER_USER_MANAGEMENT_LEVEL_NORMAL;
		var userService = new UserService();

		// メールアドレスでユーザー情報を取得する
		var oldUser = userService.GetUserByMailAddrForMailMagazineRegister(newUser.MailAddr);

		// 登録済みユーザの場合、更新する
		if (oldUser != null)
		{
			userService.UpdateMailFlg(
				oldUser.UserId,
				this.Request.ServerVariables["REMOTE_ADDR"],
				newUser.MailAddr,
				Constants.FLG_USER_MAILFLG_OK,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert);
			managementLevel = oldUser.UserManagementLevelId;
			userId = oldUser.UserId;
		}
		// 登録済みでない場合、メルマガ会員を新規登録する
		else
		{
			newUser.UserId = UserService.CreateNewUserId(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.NUMBER_KEY_USER_ID,
				Constants.CONST_USER_ID_HEADER,
				Constants.CONST_USER_ID_LENGTH);
			newUser.UserKbn = Constants.FLG_USER_USER_KBN_MAILMAGAZINE;
			newUser.MallId = Constants.FLG_USER_MALL_ID_OWN_SITE;
			newUser.MailFlg = Constants.FLG_USER_MAILFLG_OK;
			newUser.LastChanged = Constants.FLG_LASTCHANGED_USER;

			userService.InsertWithUserExtend(
				newUser,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert);
			userId = newUser.UserId;
		}

		// CROSSPOINTに連携して会員登録、更新を行う
		if (Constants.CROSS_POINT_OPTION_ENABLED)
		{
			if (oldUser != null)
			{
				oldUser.MailFlg = Constants.FLG_USER_MAILFLG_OK;
				oldUser.LastChanged = Constants.FLG_LASTCHANGED_USER;
				oldUser.MailAddr = newUser.MailAddr;
			}
			var result = oldUser == null
				? new CrossPointUserApiService().Insert(newUser)
				: new CrossPointUserApiService().Update(oldUser);
			if (result.IsSuccess == false)
			{
				SessionManager.MessageForErrorPage
					= MessageManager.GetMessages(Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
		}

		// メール送信用のデータ
		var mailInput = (Hashtable)userInput.DataSource.Clone();
		mailInput[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID] = managementLevel;

		// メール送信
		var mailSend = new MailSendUtility(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.CONST_MAIL_ID_MAILMAGAZINE_REGIST,
			userId,
			mailInput,
			true,
			Constants.MailSendMethod.Auto,
			RegionManager.GetInstance().Region.LanguageCode,
			RegionManager.GetInstance().Region.LanguageLocaleId,
			newUser.MailAddr);
		using (mailSend)
		{
			mailSend.AddTo(newUser.MailAddr);

			// 送信エラーの場合、ログに書き込む
			if (mailSend.SendMail() == false)
			{
				AppLogger.WriteError(GetType().BaseType + " : " + mailSend.MailSendException);
			}
		}
	}
}
