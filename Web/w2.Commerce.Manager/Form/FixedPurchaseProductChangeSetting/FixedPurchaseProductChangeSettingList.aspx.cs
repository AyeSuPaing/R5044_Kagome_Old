/*
=========================================================================================================
  Module      : 定期商品変更設定一覧画面処理 (FixedPurchaseProductChangeSettingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Web.UI.WebControls;
using w2.Common.Web;
using w2.Domain.FixedPurchaseProductChangeSetting;
using w2.Domain.FixedPurchaseProductChangeSetting.Helper;

public partial class Form_FixedPurchaseProductChangeSetting_FixedPurchaseProductChangeSettingList : BasePage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// コンポーネント初期化
			InitializeComponents();

			// 検索パラメータ取得、セッションに格納
			var parameters = GetParameters();
			Session[Constants.SESSIONPARAM_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_INFO] = parameters;

			// 検索値を画面にセット
			SetParameters(parameters);

			// 一覧＆件数取得
			var service = new FixedPurchaseProductChangeSettingService();
			var searchCondition = CreateSearchCondition(parameters);
			var totalCount = service.GetSearchHitCount(searchCondition);
			var searchResults = service.Search(searchCondition);

			if (searchResults.Length != 0)
			{
				// データバインドsearchResults
				rList.DataSource = searchResults;
				rList.DataBind();

				// ページャセット
				lbPager1.Text = WebPager.CreateDefaultListPager(totalCount, this.CurrentPageNo, GetUrlByParameters(parameters));

				trListError.Visible = false;
			}
			else
			{
				CheckRedirectToLastPage(
					searchResults.Length,
					totalCount,
					GetUrlForSearch(parameters));

				// 0件だったらエラーメッセージ表示
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
				trListError.Visible = true;
			}
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 並び順
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_FIXEDPURCHASEPRODUCTCHANGESETTING, Constants.REQUEST_KEY_SORT_KBN))
		{
			ddlSortKbn.Items.Add(li);
			if (li.Value == Constants.KBN_SORT_FIXEDPURCHASEPRODUCTCHANGESETTING_LIST_DEFAULT) li.Selected = true;
		}

		// 有効フラグ
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_FIXEDPURCHASEPRODUCTCHANGESETTING, Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG))
		{
			ddlValidFlg.Items.Add(li);
		}
	}

	/// <summary>
	/// 検索パラメータ取得
	/// </summary>
	/// <returns>検索パラメータ</returns>
	private Hashtable GetParameters()
	{
		var parameters = new Hashtable
		{
			// 定期商品変更ID
			{ Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID]) },
			// 定期商品変更名
			{ Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_NAME]) },
			// 変更元商品ID
			{ Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_BEFORE_CHANGE_PRODUCT_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_BEFORE_CHANGE_PRODUCT_ID]) },
			// 変更後商品ID
			{ Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_AFTER_CHANGE_PRODUCT_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_AFTER_CHANGE_PRODUCT_ID]) },
			// 有効フラグ
			{
				Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG, string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG]) == false
					? Request[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG]
					: Constants.FLG_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID
			},
		};

		// 並び順
		switch (StringUtility.ToEmpty((string)Request[Constants.REQUEST_KEY_SORT_KBN]))
		{
			case Constants.KBN_SORT_FIXEDPURCHASEPRODUCTCHANGESETTING_LIST_FIXED_PURCHASE_PRODUCT_CHANGE_ID_ASC:
			case Constants.KBN_SORT_FIXEDPURCHASEPRODUCTCHANGESETTING_LIST_FIXED_PURCHASE_PRODUCT_CHANGE_ID_DESC:
			case Constants.KBN_SORT_FIXEDPURCHASEPRODUCTCHANGESETTING_LIST_PRIORITY_DESC:
			case Constants.KBN_SORT_FIXEDPURCHASEPRODUCTCHANGESETTING_LIST_DATE_CREATED_ASC:
			case Constants.KBN_SORT_FIXEDPURCHASEPRODUCTCHANGESETTING_LIST_DATE_CREATED_DESC:
			case Constants.KBN_SORT_FIXEDPURCHASEPRODUCTCHANGESETTING_LIST_DATE_CHANGED_ASC:
			case Constants.KBN_SORT_FIXEDPURCHASEPRODUCTCHANGESETTING_LIST_DATE_CHANGED_DESC:
				parameters.Add(Constants.REQUEST_KEY_SORT_KBN, Request[Constants.REQUEST_KEY_SORT_KBN]);
				break;

			default:
				parameters.Add(Constants.REQUEST_KEY_SORT_KBN, Constants.KBN_SORT_FIXEDPURCHASEPRODUCTCHANGESETTING_LIST_DEFAULT);
				break;
		}

		// ページ番号
		int pageNo;
		if (int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]), out pageNo) == false)
		{
			pageNo = 1;
		}
		parameters.Add(Constants.REQUEST_KEY_PAGE_NO, pageNo);
		this.CurrentPageNo = pageNo;

		return parameters;
	}

	/// <summary>
	/// 画面に検索値をセット
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	private void SetParameters(Hashtable parameters)
	{
		// 定期商品変更ID
		tbFixedPurchaseProductChangeId.Text = (string)parameters[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID];
		// 定期商品変更名
		tbFixedPurchaseProductChangeName.Text = (string)parameters[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_NAME];
		// 変更元商品ID
		tbBeforeChangeProductId.Text = (string)parameters[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_BEFORE_CHANGE_PRODUCT_ID];
		// 変更後商品ID
		tbAfterChangeProductId.Text = (string)parameters[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_AFTER_CHANGE_PRODUCT_ID];
		// 有効フラグ
		ddlValidFlg.SelectedValue = (string)parameters[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG];
		// 並び順
		ddlSortKbn.SelectedValue = (string)parameters[Constants.REQUEST_KEY_SORT_KBN];
	}

	/// <summary>
	/// 検索ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		var htSearch = GetSearchInfoFromControl();

		var errorMessages = Validator.Validate("FixedPurchaseProductChangeSettingSearch", htSearch);
		if (string.IsNullOrEmpty(errorMessages) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		Response.Redirect(GetUrlForSearch(htSearch));
	}

	/// <summary>
	/// 新規登録ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_REGISTER)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_INSERT)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 検索条件作成
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <returns>セットプロモーション検索条件</returns>
	private FixedPurchaseProductChangeSettingListSearchCondition CreateSearchCondition(Hashtable parameters)
	{
		var condition = new FixedPurchaseProductChangeSettingListSearchCondition
		{
			FixedPurchaseProductChangeId = StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID]),
			FixedPurchaseProductChangeName = StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_NAME]),
			BeforeChangeProductId = StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_BEFORE_CHANGE_PRODUCT_ID]),
			AfterChangeProductId = StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_AFTER_CHANGE_PRODUCT_ID]),
			SortKbn = StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SORT_KBN]),
			ValidFlg = StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG]),
			BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNo - 1) + 1,
			EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNo,
		};
		return condition;
	}

	/// <summary>
	/// パラメータから検索用URLを作成
	/// </summary>
	/// <param name="parameters">パラメータ</param>
	/// <returns>検索用URL</returns>
	private string GetUrlByParameters(Hashtable parameters)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_LIST)
			.AddParam(
				Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID,
				StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID]))
			.AddParam(
				Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_NAME,
				StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_NAME]))
			.AddParam(
				Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_BEFORE_CHANGE_PRODUCT_ID,
				StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_BEFORE_CHANGE_PRODUCT_ID]))
			.AddParam(
				Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_AFTER_CHANGE_PRODUCT_ID,
				StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_AFTER_CHANGE_PRODUCT_ID]))
			.AddParam(
				Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG,
				StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG]))
			.AddParam(
				Constants.REQUEST_KEY_SORT_KBN,
				StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SORT_KBN]))
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// 入力値から検索用URL取得
	/// </summary>
	/// <returns>検索用URL</returns>
	private string GetUrlForSearch(Hashtable htSerach)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_LIST)
			.AddParam(
				Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID,
				StringUtility.ToEmpty(htSerach[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID]))
			.AddParam(
				Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_NAME,
				StringUtility.ToEmpty(htSerach[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_NAME]))
			.AddParam(
				Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_BEFORE_CHANGE_PRODUCT_ID,
				StringUtility.ToEmpty(htSerach[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_BEFORE_CHANGE_PRODUCT_ID]))
			.AddParam(
				Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_AFTER_CHANGE_PRODUCT_ID,
				StringUtility.ToEmpty(htSerach[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_AFTER_CHANGE_PRODUCT_ID]))
			.AddParam(
				Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG,
				StringUtility.ToEmpty(htSerach[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG]))
			.AddParam(
				Constants.REQUEST_KEY_SORT_KBN,
				StringUtility.ToEmpty(htSerach[Constants.REQUEST_KEY_SORT_KBN]))
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// 各検索コントロールから検索情報取得
	/// </summary>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchInfoFromControl()
	{
		var htSerch = new Hashtable
		{
			{ Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID, tbFixedPurchaseProductChangeId.Text.Trim() },
			{ Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_NAME, tbFixedPurchaseProductChangeName.Text.Trim() },
			{ Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_BEFORE_CHANGE_PRODUCT_ID, tbBeforeChangeProductId.Text.Trim() },
			{ Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_AFTER_CHANGE_PRODUCT_ID, tbAfterChangeProductId.Text.Trim() },
			{ Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG, ddlValidFlg.SelectedValue },
			{ Constants.REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue },
		};
		return htSerch;
	}

	/// <summary>
	/// 詳細ページURL作成
	/// </summary>
	/// <param name="fixedPurchaseProductChangeSettingId">定期商品変更設定ID</param>
	/// <returns>詳細ページURL</returns>
	protected string CreateDetailUrl(string fixedPurchaseProductChangeSettingId)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL)
			.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID, fixedPurchaseProductChangeSettingId)
			.CreateUrl();
		return WebSanitizer.UrlAttrHtmlEncode(url);
	}

	/// <summary>カレントページ番号</summary>
	private int CurrentPageNo { get; set; }
}
