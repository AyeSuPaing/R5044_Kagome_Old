/*
=========================================================================================================
  Module      : オペレータ情報一覧ページ処理(OperatorList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Operator;
using w2.Common.Web;
using w2.Domain.ShopOperator;

public partial class Form_Operator_OperatorList : BasePage
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
			Initialize();

			var request = new Dictionary<string, string>()
			{
				{ Constants.REQUEST_KEY_OPERATOR_ID, Request[Constants.REQUEST_KEY_OPERATOR_ID] },
				{ Constants.REQUEST_KEY_OPERATOR_NAME, Request[Constants.REQUEST_KEY_OPERATOR_NAME] },
				{ Constants.REQUEST_KEY_OPERATOR_MENUACCESS_LEVEL, Request[Constants.REQUEST_KEY_OPERATOR_MENUACCESS_LEVEL] },
				{ Constants.REQUEST_KEY_OPERATOR_VALID_FLG, Request[Constants.REQUEST_KEY_OPERATOR_VALID_FLG] },
				{ Constants.REQUEST_KEY_SORT_KBN, Request[Constants.REQUEST_KEY_SORT_KBN] },
			};
			var paramData = OperatorUtility.GetParameters(request, this.Request[Constants.REQUEST_KEY_PAGE_NO]);
			SetSearchValues(paramData);
			if (bool.Parse(paramData[Constants.ERROR_REQUEST_PRAMETER]))
			{
				Session[Constants.SESSION_KEY_ERROR_MSG]
					= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			DisplayList(paramData);
		}
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		ddlMenuAccess.Items.Add(new ListItem("", ""));
		var menuAccesses = MenuAuthorityUtility.CreateMenuAuthorityList(
			this.LoginOperatorShopId,
			this.LoginOperatorMenuAccessLevel,
			Constants.ManagerSiteType);
		ddlMenuAccess.Items.AddRange(menuAccesses);

		ddlValid.Items.Add(new ListItem("", ""));
		ddlValid.Items.AddRange(ValueText.GetValueItemArray(
			Constants.TABLE_SHOPOPERATOR, Constants.FIELD_SHOPOPERATOR_VALID_FLG));
		ddlSortKbn.SelectedValue = Constants.FLG_SORT_OPERATOR_LIST_ID_DESC;
	}

	/// <summary>
	/// 検索値設定
	/// </summary>
	/// <param name="request">パラメタが格納されたDictionary</param>
	private void SetSearchValues(Dictionary<string, string> request)
	{
		tbOperatorId.Text = HttpUtility.UrlDecode(request[Constants.REQUEST_KEY_OPERATOR_ID]);
		tbOperatorName.Text = HttpUtility.UrlDecode(request[Constants.REQUEST_KEY_OPERATOR_NAME]);
		ddlMenuAccess.SelectedValue = request[Constants.REQUEST_KEY_OPERATOR_MENUACCESS_LEVEL];
		ddlValid.SelectedValue = request[Constants.REQUEST_KEY_OPERATOR_VALID_FLG];
		ddlSortKbn.SelectedValue = request[Constants.REQUEST_KEY_SORT_KBN];
	}

	/// <summary>
	/// 一覧表示
	/// </summary>
	/// <param name="paramData">リクエストパラメータデータ</param>
	private void DisplayList(Dictionary<string, string> paramData)
	{
		var pageNo = int.Parse(paramData[Constants.REQUEST_KEY_PAGE_NO]);
		var rowNumber = OperatorUtility.GetRowNumber(pageNo);
		var htInputa = OperatorUtility.GetSearchSqlInfo(paramData);
		htInputa.Add(Constants.FIELD_SHOPOPERATOR_SHOP_ID, this.LoginOperatorShopId);
		htInputa.Add(Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG, this.LoginOperatorMenuAccessLevel);
		var operators = new ShopOperatorService().GetMpOperatorList(htInputa, rowNumber);

		if (operators.Length != 0)
		{
			var rowCount = operators[0].RowCount;
			var parameters = CreateParametersList();
			var nextUrl = OperatorUtility.CreateListUrl(parameters);
			lbPager.Text = WebPager.CreateDefaultListPager(rowCount, pageNo, nextUrl);
			rOperatorList.DataSource = operators;
			DataBind();
		}
		else
		{
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnNew_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_OPERATOR_REGISTER)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_INSERT)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		var parameters = CreateParametersList();
		Response.Redirect(OperatorUtility.CreateListUrl(parameters));
	}

	/// <summary>
	/// パラメータリスト作成
	/// </summary>
	/// <returns>検索条件パラメータリスト</returns>
	protected List<string> CreateParametersList()
	{
		var parameters = new List<string>
		{
			tbOperatorId.Text.Trim(),
			tbOperatorName.Text.Trim(),
			ddlSortKbn.SelectedValue,
			ddlMenuAccess.SelectedValue,
			ddlValid.SelectedValue,
		};
		return parameters;
	}

	/// <summary>
	/// MPメニュー権限名称取得
	/// </summary>
	/// <param name="mpOperator">MPオペレータ</param>
	/// <returns>メニュー権限表示文字列</returns>
	protected string GetMpMenuAccessLevelName(ShopOperatorModel mpOperator)
	{
		switch (mpOperator.MpMenuAccessLevel)
		{
			case Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER:
				return Constants.STRING_SUPERUSER_NAME;

			case Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_UNACCESSABLEUSER:
				return Constants.STRING_UNACCESSABLEUSER_NAME;

			default:
				return mpOperator.MenuAuthorityName;
		}
	}
}