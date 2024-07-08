/*
=========================================================================================================
  Module      : メニュー権限設定一覧ページ処理(MenuAuthorityList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Web;
using w2.Domain.MenuAuthority;

public partial class Form_MenuAuthority_MenuAuthorityList : BasePage
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
			var menuAuthorities = new MenuAuthorityService().GetAllByPkgKbn(
				this.LoginOperatorShopId,
				Constants.ManagerSiteType);
			rMenuAuthorityList.DataSource = menuAuthorities;
			rMenuAuthorityList.DataBind();

			if (menuAuthorities.Length != 0)
			{
				// エラー表示制御
				trListError.Visible = false;
			}
			else
			{
				// エラー表示制御
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}
		}
	}

	/// <summary>
	/// データバインド用詳細画面URL作成
	/// </summary>
	/// <param name="menuAuthorityLevel">メニュー権限レベル</param>
	/// <returns>詳細画面URL</returns>
	protected string CreateDetailUrl(int menuAuthorityLevel)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MENU_AUTHORITY_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_MENUAUTHORITY_LEVEL, menuAuthorityLevel.ToString())
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL)
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnNew_Click(object sender, System.EventArgs e)
	{
		// 新規登録画面へ遷移
		Response.Redirect( Constants.PATH_ROOT + Constants.PAGE_MANAGER_MENU_AUTHORITY_REGISTER + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}
}

