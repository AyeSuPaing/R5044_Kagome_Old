/*
=========================================================================================================
 Module      : 商品同梱設定一覧ページ(ProductBundleList.aspx.cs)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
 Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Domain.ProductBundle;
using w2.Domain.ProductBundle.Helper;

/// <summary>
/// 商品同梱設定一覧ページ
/// </summary>
public partial class Form_ProductBundle_ProductBundleList : ProductBundlePage
{
	#region イベントハンドラ
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// コンポーネント初期化
			InitializeComponents();

			// パラメタ取得
			var parameters = GetParameters();
			Session[Constants.SESSIONPARAM_KEY_PRODUCTBUNDLE_SEARCH_INFO] = parameters;

			// コントロールへの値セット
			SetParameters(parameters);

			ViewProductBundleList(parameters);
		}
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateProductBundleRegister(Constants.ACTION_STATUS_INSERT, string.Empty));
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateProductBundleListUrl(GetSearchInfoFromControl(), true, 1));
	}
	#endregion

	/// <summary>
	/// 各種コントロール初期化
	/// </summary>
	private void InitializeComponents()
	{
		InitializeTargetOrderType();
	}

	/// <summary>
	/// 対象注文種別初期化
	/// </summary>
	private void InitializeTargetOrderType()
	{
		ddlTargetOrderType.Items.Add(new ListItem(string.Empty, string.Empty));
		if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
		{
			ddlTargetOrderType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_PRODUCTBUNDLE, Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_TYPE));
		}
		else
		{
			ddlTargetOrderType.Items.Add(
				new ListItem(
					//「通常注文」
					ValueText.GetValueText(
						Constants.TABLE_PRODUCTBUNDLE,
						Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_TYPE,
						Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_NORMAL),
					Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_NORMAL));
		}
	}

	/// <summary>
	/// パラメタ取得
	/// </summary>
	private Hashtable GetParameters()
	{
		var parameters = new Hashtable();

		parameters.Add(Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID]));
		parameters.Add(Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_NAME]));
		parameters.Add(Constants.REQUEST_KEY_PRODUCTBUNDLE_TARGET_ORDER_TYPE, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTBUNDLE_TARGET_ORDER_TYPE]));

		var sortType = new[] 
		{
			Constants.KBN_SORT_PRODUCTBUNDLE_LIST_PRODUCT_BUNDLE_ID_ASC,
			Constants.KBN_SORT_PRODUCTBUNDLE_LIST_PRODUCT_BUNDLE_ID_DESC,
			Constants.KBN_SORT_PRODUCTBUNDLE_LIST_PRODUCT_BUNDLE_NAME_ASC,
			Constants.KBN_SORT_PRODUCTBUNDLE_LIST_PRODUCT_BUNDLE_NAME_DESC,
			Constants.KBN_SORT_PRODUCTBUNDLE_LIST_START_DATE_TIME_ASC,
			Constants.KBN_SORT_PRODUCTBUNDLE_LIST_START_DATE_TIME_DESC,
			Constants.KBN_SORT_PRODUCTBUNDLE_LIST_END_DATE_TIME_ASC,
			Constants.KBN_SORT_PRODUCTBUNDLE_LIST_END_DATE_TIME_DESC,
			Constants.KBN_SORT_PRODUCTBUNDLE_LIST_APPLY_ORDER_ASC,
			Constants.KBN_SORT_PRODUCTBUNDLE_LIST_APPLY_ORDER_DESC
		};
		parameters.Add(Constants.REQUEST_KEY_SORT_KBN, (sortType.Any(type => type == StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN])))
			? Request[Constants.REQUEST_KEY_SORT_KBN]
			: Constants.KBN_SORT_PRODUCTBUNDLE_LIST_DEFAULT);
		parameters.Add(Constants.REQUEST_KEY_PRODUCTBUNDLE_TARGET_PRODUCT_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTBUNDLE_TARGET_PRODUCT_ID]));
		parameters.Add(Constants.REQUEST_KEY_PRODUCTBUNDLE_BUNDLE_PRODUCT_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTBUNDLE_BUNDLE_PRODUCT_ID]));
		parameters.Add(Constants.REQUEST_KEY_PAGE_NO, this.CurrentPageNo);

		return parameters;
	}

	/// <summary>
	/// 検索条件を画面にセット
	/// </summary>
	/// <param name="parameters">検索パラメタ</param>
	private void SetParameters(Hashtable parameters)
	{
		tbBundleId.Text = (string)parameters[Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID];
		tbBundleName.Text = (string)parameters[Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_NAME];
		ddlTargetOrderType.SelectedValue = (string)parameters[Constants.REQUEST_KEY_PRODUCTBUNDLE_TARGET_ORDER_TYPE];
		ddlSortKbn.SelectedValue = (string)parameters[Constants.REQUEST_KEY_SORT_KBN];
		tbTargetProductId.Text = (string)parameters[Constants.REQUEST_KEY_PRODUCTBUNDLE_TARGET_PRODUCT_ID];
		tbBundleProductId.Text = (string)parameters[Constants.REQUEST_KEY_PRODUCTBUNDLE_BUNDLE_PRODUCT_ID];
	}

	/// <summary>
	/// 検索情報を画面から取得
	/// </summary>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchInfoFromControl()
	{
		var searchInfo = new Hashtable
		{
			{ Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID, tbBundleId.Text.Trim() },
			{ Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_NAME, tbBundleName.Text.Trim() },
			{ Constants.REQUEST_KEY_PRODUCTBUNDLE_TARGET_ORDER_TYPE, ddlTargetOrderType.SelectedValue },
			{ Constants.REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue },
			{ Constants.REQUEST_KEY_PRODUCTBUNDLE_TARGET_PRODUCT_ID, tbTargetProductId.Text.Trim()},
			{ Constants.REQUEST_KEY_PRODUCTBUNDLE_BUNDLE_PRODUCT_ID, tbBundleProductId.Text.Trim()}
		};
		return searchInfo;
	}

	/// <summary>
	/// 商品同梱設定一覧の表示
	/// </summary>
	/// <param name="parameters">検索パラメタ</param>
	private void ViewProductBundleList(Hashtable parameters)
	{
		var service = new ProductBundleService();
		var searchCondition = CreateRequestParameter(parameters);
		var totalCount = service.GetSearchHitCount(searchCondition);

		if (totalCount > 0)
		{
			var productBundleList = service.Search(searchCondition);

			// Redirect to last page when current page no don't have any data
			CheckRedirectToLastPage(
				productBundleList.Length,
				totalCount,
				CreateProductBundleListUrl(parameters, false));

			rProductBundleList.DataSource = productBundleList;
			rProductBundleList.DataBind();

			// ページャ
			lbPager.Text = WebPager.CreateDefaultListPager(
				totalCount,
				this.CurrentPageNo,
				CreateProductBundleListUrl(parameters, false));

			trListError.Visible = false;
		}
		else
		{
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}
	}

	/// <summary>
	/// DB問い合わせ用パラメタ生成
	/// </summary>
	/// <returns>DB問い合わせ用パラメタ</returns>
	private ProductBundleListSearchCondition CreateRequestParameter(Hashtable parameters)
	{
		var condition = new ProductBundleListSearchCondition
		{
			ProductBundleId = (string)parameters[Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID],
			ProductBundleName = (string)parameters[Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_NAME],
			TargetOrderType = (string)parameters[Constants.REQUEST_KEY_PRODUCTBUNDLE_TARGET_ORDER_TYPE],
			SortKbn = (string)parameters[Constants.REQUEST_KEY_SORT_KBN],
			TargetProductId = (string)parameters[Constants.REQUEST_KEY_PRODUCTBUNDLE_TARGET_PRODUCT_ID],
			BundleProductId = (string)parameters[Constants.REQUEST_KEY_PRODUCTBUNDLE_BUNDLE_PRODUCT_ID],
			BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNo - 1) + 1,
			EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNo
		};
		return condition;
	}

	#region プロパティ
	/// <summary>現在のページ番号</summary>
	private int CurrentPageNo 
	{ 
		get 
		{ 
			int pageNo;
			return int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNo) ? pageNo : DEFAULT_PAGE_NO; 
		}
	}
	#endregion
}