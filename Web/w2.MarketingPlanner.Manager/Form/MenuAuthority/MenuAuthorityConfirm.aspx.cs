/*
=========================================================================================================
  Module      : メニュー権限設定確認ページ処理(MenuAuthorityConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.App.Common.Manager.Menu;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.MenuAuthority;

public partial class Form_MenuAuthority_MenuAuthorityConfirm : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			InitializeComponents();
			Display();
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		btnEditTop.Visible = btnEditBottom.Visible = (this.ActionStatus == Constants.ACTION_STATUS_DETAIL);
		btnDeleteTop.Visible = btnDeleteBottom.Visible = (this.ActionStatus == Constants.ACTION_STATUS_DETAIL);
		btnInsertTop.Visible = btnInsertBottom.Visible = (this.ActionStatus == Constants.ACTION_STATUS_INSERT);
		btnUpdateTop.Visible = btnUpdateBottom.Visible = (this.ActionStatus == Constants.ACTION_STATUS_UPDATE);
	}

	/// <summary>
	/// 表示
	/// </summary>
	private void Display()
	{
		string bindName = null;
		MenuLarge[] bindLargeMenus = null;

		// 画面設定処理
		if ((this.ActionStatus == Constants.ACTION_STATUS_INSERT)
			|| (this.ActionStatus == Constants.ACTION_STATUS_UPDATE))
		{
			// 処理区分チェック・入力データ取得
			var authorityMenuList = (MenuAuthorityModel[])Session[Constants.SESSION_KEY_MENUAUTHORITY_INFO];
			bindName = (authorityMenuList.Length != 0) ? authorityMenuList[0].MenuAuthorityName : "";
			bindLargeMenus = (MenuLarge[])Session[Constants.SESSION_KEY_MENUAUTHORITY_LMENUS];
		}
		else if (this.ActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			var menuAuthorityList = new MenuAuthorityService().Get(
				this.LoginOperatorShopId,
				Constants.ManagerSiteType,
				this.MenuAuthorityLevel);
			if (menuAuthorityList.Length == 0)
			{
				this.Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
				this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			var menuAuthorityInfoList = menuAuthorityList.ToDictionary(ma => ma.MenuPath, ma => ma);
			bindName = menuAuthorityList[0].MenuAuthorityName;
			bindLargeMenus = ManagerMenuCache.Instance.GetAuthorityMenuList(menuAuthorityInfoList);

			// ログインオペレータが所持している権限の中に、対象の権限が一部でも含まれていない場合編集不可
			if (ManagerMenuCache.Instance.HasOperatorMenuAuthority(bindLargeMenus, this.LoginOperatorMenu) == false)
			{
				btnEditTop.Visible = btnEditBottom.Visible = false;
				btnDeleteTop.Visible = btnDeleteBottom.Visible = false;
			}
		}

		// データバインド
		lMenuAuthorityName.Text = WebSanitizer.HtmlEncode(bindName);
		rLargeMenu.DataSource = bindLargeMenus;
		rLargeMenu.DataBind();
	}

	/// <summary>
	/// 登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// インサート
		var authorities = (MenuAuthorityModel[])Session[Constants.SESSION_KEY_MENUAUTHORITY_INFO];
		new MenuAuthorityService().Insert(
			authorities,
			(int)NumberingUtility.CreateNewNumber(this.LoginOperatorDeptId, Constants.NUMBER_KEY_MENU_AUTHORITY_LEVEL));

		// 一覧画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MENU_AUTHORITY_LIST);
	}


	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		// アップデート（デリートインサートを行う）
		var authorities = (MenuAuthorityModel[])Session[Constants.SESSION_KEY_MENUAUTHORITY_INFO];
		new MenuAuthorityService().Update(authorities);

		// 一覧画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MENU_AUTHORITY_LIST);
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		var isUsed = new MenuAuthorityService().CheckMenuAuthorityUsed(
			this.LoginOperatorShopId,
			Constants.ManagerSiteType,
			this.MenuAuthorityLevel);
		if (isUsed)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MENUAUTHORITY_DELETE_IMPOSSIBLE_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		new MenuAuthorityService().Delete(this.LoginOperatorShopId, Constants.ManagerSiteType, this.MenuAuthorityLevel);

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MENU_AUTHORITY_LIST);
	}

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MENU_AUTHORITY_REGISTER)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE)
			.AddParam(Constants.REQUEST_KEY_MENUAUTHORITY_LEVEL, this.MenuAuthorityLevel.ToString())
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>メニュー権限レベル</summary>
	public int MenuAuthorityLevel
	{
		get { return int.Parse(Request[Constants.REQUEST_KEY_MENUAUTHORITY_LEVEL]); }
	}
}
