/*
=========================================================================================================
  Module      : カテゴリ検索ページ処理(ProductCategorySearch.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Form_Common_ProductCategorySearch : BasePage
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
			// パラメータ取得
			Hashtable parameters = GetParameters();

			// 画面に検索値セット
			SetParameters(parameters);

			// カテゴリ一覧取得
			DataView categoryList = GetCategoryList(parameters);

			if (categoryList.Count != 0)
			{
				// 該当件数取得
				int totalProductCounts = (int)categoryList[0]["row_count"];

				// カテゴリ一覧データバインド
				rList.DataSource = categoryList;
				rList.DataBind();

				// ページャ作成
				lbPager1.Text = WebPager.CreateDefaultListPager(
					totalProductCounts,
					this.CurrentPageNo,
					CreateCategorySearchListUrl(parameters));

				trListError.Visible = false;
			}
			else
			{
				// 該当件数０件だったらエラーを表示
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
				trListError.Visible = true;
			}
		}
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		Hashtable parameters = new Hashtable();
		parameters.Add(Constants.REQUEST_KEY_PRODUCTCATEGORY_CATEGORY_ID, tbCategoryId.Text.Trim());
		parameters.Add(Constants.REQUEST_KEY_PRODUCTCATEGORY_NAME, tbName.Text.Trim());

		// 検索用パラメータを作成し、同じ画面にリダイレクト
		Response.Redirect(CreateCategorySearchListUrl(parameters, 1));
	}

	/// <summary>
	/// パラメータ取得
	/// </summary>
	/// <returns>パラメータが格納されたHashtable</returns>
	protected Hashtable GetParameters()
	{
		Hashtable parameters = new Hashtable();

		parameters.Add(Constants.REQUEST_KEY_PRODUCTCATEGORY_CATEGORY_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTCATEGORY_CATEGORY_ID]));
		parameters.Add(Constants.REQUEST_KEY_PRODUCTCATEGORY_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTCATEGORY_NAME]));

		// ページ番号
		int currentPageNumber;
		if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out currentPageNumber) == false)
		{
			currentPageNumber = 1;
		}
		this.CurrentPageNo = currentPageNumber;

		return parameters;
	}

	/// <summary>
	/// 検索値を画面にセット
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	private void SetParameters(Hashtable parameters)
	{
		tbCategoryId.Text = (string)parameters[Constants.REQUEST_KEY_PRODUCTCATEGORY_CATEGORY_ID];
		tbName.Text = (string)parameters[Constants.REQUEST_KEY_PRODUCTCATEGORY_NAME];
	}

	/// <summary>
	/// 商品カテゴリ一覧情報取得
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <returns>商品カテゴリ一覧情報</returns>
	private DataView GetCategoryList(Hashtable parameters)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductCategorySearch", "GetCategoryList"))
		{
			return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, GetSqlParams(parameters));
		}
	}

	/// <summary>
	/// SQLパラメータ取得
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <returns>SQLパラメータ</returns>
	private Hashtable GetSqlParams(Hashtable parameters)
	{
		Hashtable sqlParams = new Hashtable();
		sqlParams.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.LoginOperatorShopId);
		sqlParams.Add(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(parameters[Constants.REQUEST_KEY_PRODUCTCATEGORY_CATEGORY_ID]));
		sqlParams.Add(Constants.FIELD_PRODUCTCATEGORY_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(parameters[Constants.REQUEST_KEY_PRODUCTCATEGORY_NAME]));
		sqlParams.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNo - 1) + 1);
		sqlParams.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNo);

		return sqlParams;
	}

	/// <summary>
	/// ページ遷移URL作成
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <returns>ページ遷移URL</returns>
	private string CreateCategorySearchListUrl(Hashtable parameters)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCT_CATEGORY_SEARCH);
		url.Append("?").Append(Constants.REQUEST_KEY_PRODUCTCATEGORY_CATEGORY_ID).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_PRODUCTCATEGORY_CATEGORY_ID]));
		url.Append("&").Append(Constants.REQUEST_KEY_PRODUCTCATEGORY_NAME).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_PRODUCTCATEGORY_NAME]));

		return url.ToString();
	}
	/// <summary>
	/// ページ遷移URL作成
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <param name="pageNumber">ページ番号</param>
	/// <returns>ページ遷移URL</returns>
	private string CreateCategorySearchListUrl(Hashtable parameters, int pageNumber)
	{
		StringBuilder url = new StringBuilder();
		url.Append(CreateCategorySearchListUrl(parameters));
		url.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(pageNumber);

		return url.ToString();
	}

	/// <summary>
	/// JavaScriptのパラメータ作成
	/// </summary>
	/// <param name="category">カテゴリ情報</param>
	/// <returns>JavaScriptのパラメータ</returns>
	/// <remarks>選択した商品を親画面にセットするJavaScriptのパラメータ</remarks>
	protected string CreateJavaScriptSetCategoryInfo(DataRowView category)
	{
		StringBuilder javaScriptParam = new StringBuilder();

		// カテゴリID
		javaScriptParam.Append("'").Append(StringUtility.ToEmpty(category[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID]).Replace("'", "\\'")).Append("',");
		// カテゴリ名
		javaScriptParam.Append("'").Append(StringUtility.ToEmpty(category[Constants.FIELD_PRODUCTCATEGORY_NAME]).Replace("'", "\\'")).Append("'");

		return javaScriptParam.ToString();
	}

	/// <summary>カレントページNO</summary>
	protected int CurrentPageNo
	{
		get { return (int)ViewState["CurrentPageNo"]; }
		set { ViewState["CurrentPageNo"] = value; }
	}
}