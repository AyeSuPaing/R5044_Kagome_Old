/*
=========================================================================================================
  Module      : ポップアップマスタページ(PopupPage.master)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.App.Common.Manager.Menu;
using w2.App.Common.OperationLog;
using w2.Common.Web;

public partial class Form_Common_PopupPage : MasterBasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		OperationLogWriter.WriteOperationLog(this.LoginOperatorId, Request.UserHostAddress, Session.SessionID, this.RawUrl);

		//------------------------------------------------------
		// ポップアップ画面リダイレクト制御
		//------------------------------------------------------
		CheckPopupPage();

		//------------------------------------------------------
		// OnLoadイベント設定
		//------------------------------------------------------
		if (!IsPostBack)
		{
			//LogデータをJsonファイルに記録
			var url = this.RawUrl.Split(new string[] { "aspx" }, StringSplitOptions.None);
			OperationLogWriter.SendJsonAndWriteOnFailure(
				Constants.PLAN_NAME,
				Constants.PROJECT_NO,
				Constants.ENVIRONMENT_NAME,
				this.LoginOperatorId,
				this.LoginOperatorName,
				Request.UserHostAddress,
				Session.SessionID,
				this.RawUrl,
				this.RawUrl.Split('?')[0],
				this.Request.Url.Query);

			if (Request[Constants.REQUEST_KEY_RELOAD_PARENT_WINDOW] == Constants.KBN_RELOAD_PARENT_WINDOW_ON)
			{
				// 親画面リロード関数セット
				m_strOnLoadEvents += "reload_parent_window();";
			}

			ViewState["OnLoadEvent"] = m_strOnLoadEvents;
		}
		else
		{
			m_strOnLoadEvents = (string)ViewState["OnLoadEvent"];
		}

		//------------------------------------------------------
		// メニューリスト設定＆画面チェック
		//------------------------------------------------------
		// メニューリスト設定
		List<MenuLarge> lOperatorMenuList = (this.LoginOperatorMenu != null) ? this.LoginOperatorMenu : new List<MenuLarge>();

		if (CheckPageMenuAuthority(lOperatorMenuList) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_LOGIN_UNACCESSABLEUSER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}


		//------------------------------------------------------
		// テキストボックスでエンターキーを押した時の動作を制御
		// （検索、または何もしない様にする）
		//------------------------------------------------------
		SetTextBoxEnterKeyDownEventSearchOrNone(form1, btnDummy);
	}

	/// <summary>
	/// メニューが現ページのアドレスを含むか
	/// </summary>
	/// <param name="lMenuLargeList"></param>
	/// <returns></returns>
	private bool CheckPageMenuAuthority(List<MenuLarge> lMenuLargeList)
	{
		if ((Request.Url.AbsolutePath.Contains(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR))
			|| (Request.Url.AbsolutePath.Contains(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_OPERATOR_PASSWORD_CHANGE_COMPLETE))
			|| (Request.Url.AbsolutePath.Contains(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISEMENT_CODE_SEARCH))
			|| (Request.Url.AbsolutePath.Contains(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ACTIONWINDOW_SCHEDULEEXECUTE)))
		{
			return true;
		}

		foreach (MenuLarge ml in lMenuLargeList)
		{
			foreach (MenuSmall ms in ml.SmallMenus)
			{
				if (Request.Url.AbsolutePath.IndexOf(ms.MenuPath) != -1)
				{
					return true;
				}
			}
		}

		return false;
	}
}
