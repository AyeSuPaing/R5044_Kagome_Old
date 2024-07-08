/*
=========================================================================================================
  Module      : メニュー権限設定登録ページ処理(MenuAuthorityRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.Common.Web;
using w2.Domain.MenuAuthority;
using w2.Domain.MenuAuthority.Helper;

public partial class Form_MenuAuthority_MenuAuthorityRegister : BasePage
{
	// メニュー権限状況（編集画面用)
	protected Dictionary<string, MenuAuthorityModel> m_menuAuthorityInfoList = new Dictionary<string, MenuAuthorityModel>();

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			DisplayForInit();
		}
	}

	/// <summary>
	/// 初期化用表示
	/// </summary>
	private void DisplayForInit()
	{
		if (this.ActionStatus == Constants.ACTION_STATUS_INSERT)
		{
			tbMenuAuthorityName.Text = "";
		}
		else if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			var menuAuthorities = new MenuAuthorityService().Get(
				this.LoginOperatorShopId,
				Constants.ManagerSiteType,
				this.MenuAuthorityLevel);

			tbMenuAuthorityName.Text = menuAuthorities[0].MenuAuthorityName;
			m_menuAuthorityInfoList = menuAuthorities.ToDictionary(
				ma => ma.MenuPath,
				ma => new MenuAuthorityModel
				{
					MenuPath = ma.MenuPath,
					FunctionLevel = ma.FunctionLevel,
					IsDefaultDispOn = ma.IsDefaultDispOn,
				});
		}

		// データバインド
		rLargeMenu.DataSource = this.LoginOperatorMenu;
		rLargeMenu.DataBind();
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, System.EventArgs e)
	{
		// 入力値読み取り
		var menuAuthorityListInfo = new Dictionary<string, MenuAuthorityModel>();
		foreach (RepeaterItem riLargeMenu in rLargeMenu.Items)
		{
			var smallMenu = (Repeater)riLargeMenu.FindControl("rSmallMenu");
			foreach (RepeaterItem riSmallMenu in smallMenu.Items)
			{
				if (((CheckBox)riSmallMenu.FindControl("cbMenuCheck")).Checked == false) continue;

				var functionFlg = 0;
				var rFuntion = (Repeater)riSmallMenu.FindControl("rFunction");
				foreach (RepeaterItem riFunction in rFuntion.Items)
				{
					if (((CheckBox)riFunction.FindControl("cbCheck")).Checked)
					{
						functionFlg += int.Parse(((HiddenField)riFunction.FindControl("hfCheck")).Value);
					}
				}

				menuAuthorityListInfo.Add(
					((HiddenField)riSmallMenu.FindControl("hfMenuPath")).Value,
					new MenuAuthorityModel
					{
						MenuPath = ((HiddenField)riSmallMenu.FindControl("hfMenuPath")).Value,
						FunctionLevel = functionFlg,
						IsDefaultDispOn = (((HiddenField)riSmallMenu.FindControl("hfMenuPath")).Value == Request["rbDefaultPage"]),
					});
			}
		}

		// 許可メニューリスト取得
		var authorityMenuList = ManagerMenuCache.Instance.GetAuthorityMenuList(menuAuthorityListInfo);

		// 入力チェック
		var errorMessages = "";
		var param = new Hashtable
		{
			{ Constants.FIELD_MENUAUTHORITY_SHOP_ID, this.LoginOperatorShopId },
			{ Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_NAME, tbMenuAuthorityName.Text },
			{ "menu_authority_count", menuAuthorityListInfo.Count.ToString() }
		};
		if (this.ActionStatus == Constants.ACTION_STATUS_INSERT)
		{
			errorMessages = Validator.Validate("MenuAuthorityRegist", param);
		}
		else if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			errorMessages = Validator.Validate("MenuAuthorityModity", param);
		}
		// デフォルトページ設定チェック
		if (menuAuthorityListInfo.Values.Any(mai => mai.IsDefaultDispOn) == false)
		{
			errorMessages = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MENUAUTHORITY_NO_DEFALT_PAGE);
		}
		if (string.IsNullOrEmpty(errorMessages) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 確認画面へ
		var authorities = authorityMenuList.SelectMany(ml => ml.SmallMenus).Select(
			ms => new MenuAuthorityModel
			{
				ShopId = this.LoginOperatorShopId,
				PkgKbn = MenuAuthorityHelper.GetPkgKbn(Constants.ManagerSiteType),
				MenuAuthorityLevel =
					(this.ActionStatus == Constants.ACTION_STATUS_UPDATE) ? this.MenuAuthorityLevel : 0,
				MenuAuthorityName = tbMenuAuthorityName.Text,
				MenuPath = ms.MenuPath,
				FunctionLevel = ms.Functions.Sum(mf => mf.Level),
				DefaultDispFlg = ms.IsAuthorityDefaultDispPage
					? Constants.FLG_MENUAUTHORITY_DEFAULT_DISP_FLG_ON
					: Constants.FLG_MENUAUTHORITY_DEFAULT_DISP_FLG_OFF,
				LastChanged = this.LoginOperatorName,
			}).ToArray();
		Session[Constants.SESSION_KEY_MENUAUTHORITY_LMENUS] = authorityMenuList;
		Session[Constants.SESSION_KEY_MENUAUTHORITY_INFO] = authorities;

		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MENU_AUTHORITY_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, this.ActionStatus)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>メニュー権限レベル</summary>
	public int MenuAuthorityLevel
	{
		get { return int.Parse(Request[Constants.REQUEST_KEY_MENUAUTHORITY_LEVEL]); }
	}
}

