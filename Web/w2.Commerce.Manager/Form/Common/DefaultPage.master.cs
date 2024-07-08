/*
=========================================================================================================
  Module      : デフォルトマスタページ処理(DefaultPage.master.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Manager.Menu;
using w2.App.Common.OperationLog;
using w2.Domain.MenuAuthority.Helper;

public partial class Form_Common_DefaultPage : MasterBasePage
{
	/// <summary>クーポンメニューフォルダパス</summary>
	private const string FOLDER_PATH_COUPON_MENU = "Form/Coupon/";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		OperationLogWriter.WriteOperationLog(this.LoginOperatorId, Request.UserHostAddress, Session.SessionID, this.RawUrl);

		//------------------------------------------------------
		// OnLoadイベント設定
		//------------------------------------------------------
		if (!IsPostBack)
		{
			ViewState["OnLoadEvent"] = m_strOnLoadEvents;

			//LogデータをJsonファイルに記録
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
		}
		else
		{
			m_strOnLoadEvents = (string)ViewState["OnLoadEvent"];
		}

		//------------------------------------------------------
		// メニューリスト設定＆画面チェック
		//------------------------------------------------------
		// メニューリスト設定
		var lOperatorMenuList = this.LoginOperatorMenu ?? new List<MenuLarge>();

		if (CheckPageMenuAuthority(lOperatorMenuList) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_LOGIN_UNACCESSABLEUSER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// メニュー一覧データバインド
		//------------------------------------------------------
		rLargeMenu.DataSource = lOperatorMenuList;
		rLargeMenu.DataBind();

		//------------------------------------------------------
		// テキストボックスでエンターキーを押した時の動作を制御
		// （検索、または何もしない様にする）
		//------------------------------------------------------
		SetTextBoxEnterKeyDownEventSearchOrNone(form1, btnDummy);
	}

	/// <summary>
	/// メニューが現ページのアドレスを含むか
	/// </summary>
	/// <param name="lMenuLargeList">メニュー情報</param>
	/// <returns>含んでいる？</returns>
	private bool CheckPageMenuAuthority(List<MenuLarge> lMenuLargeList)
	{
		if (Request.Url.AbsolutePath.Contains(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR)
			|| Request.Url.AbsolutePath.Contains(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_PASSWORD_CHANGE_COMPLETE)
			|| Request.Url.AbsolutePath.Contains(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PAGE_INDEX_LIST))
		{
			return true;
		}

		var targetManagerType = MenuAuthorityHelper.ManagerSiteType.Ec;
		var targetMenuPath = Request.Url.AbsolutePath;
		if (Request.Url.AbsolutePath.Contains(Constants.PATH_ROOT + Constants.PAGE_MANAGER_COUPON_LIST_POPUP))
		{
			targetManagerType = MenuAuthorityHelper.ManagerSiteType.Mp;
			targetMenuPath = FOLDER_PATH_COUPON_MENU;
		}

		var result = new OperatorMenuManager().CheckPageMenuAuthorityWithManagetType(
			this.LoginShopOperator.GetMenuAccessLevel(targetManagerType),
			this.LoginOperatorShopId,
			targetMenuPath,
			targetManagerType);

		return result;
	}
}
