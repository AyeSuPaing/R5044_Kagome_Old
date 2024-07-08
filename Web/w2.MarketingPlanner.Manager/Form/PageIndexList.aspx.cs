/*
=========================================================================================================
  Module      : 機能一覧画面(PageIndexList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Manager.Menu;

/// <summary>
/// 機能一覧画面
/// </summary>
public partial class Form_PageIndexList : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			if (string.IsNullOrEmpty(this.Key))
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			Display();
		}
	}

	/// <summary>
	/// 表示
	/// </summary>
	private void Display()
	{
		var pageIndexList = PageIndexListUtility.GetPageIndexList(
			Constants.PHYSICALDIRPATH_PAGE_INDEX_LIST_XML,
			this.Key,
			ManagerMenuCache.Instance.MenuList);
		lMenuLargeName.Text = pageIndexList.Name;
		rSmallMenuCategories.DataSource = pageIndexList.PageIndexSmallMenuCategories;
		rSmallMenuCategories.DataBind();
	}

	/// <summary>
	/// 利用可能な機能の抽出
	/// </summary>
	/// <param name="pageIndexList">機能情報リスト</param>
	/// <returns>利用可能な機能</returns>
	protected PageIndexListUtility.PageIndexSmallMenu[] GetAuthorizedPage(
		PageIndexListUtility.PageIndexSmallMenu[] pageIndexList)
	{
		var loginMenuSmalls = this.LoginOperatorMenu.First(loginMenuLarge => loginMenuLarge.Key == this.Key).SmallMenus;
		var authorizedPages = pageIndexList.Where(
			pil => loginMenuSmalls.Any(loginMenuSmall => loginMenuSmall.Name == pil.Name)).ToArray();
		return authorizedPages;
	}

	#region プロパティ
	/// <summary>機能キー名</summary>
	protected string Key
	{
		get { return Request[Constants.REQUEST_KEY_PAGE_INDEX_LIST_KEY] ?? string.Empty; }
	}
	#endregion
}