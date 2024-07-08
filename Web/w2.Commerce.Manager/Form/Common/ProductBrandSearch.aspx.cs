/*
=========================================================================================================
  Module      : ブランド検索ページ処理(ProductBrandSearch.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using w2.Domain.ProductBrand;

public partial class Form_Common_ProductBrandSearch : BasePage
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
			// ブランド一覧表示
			DisplayBrandList();
		}
	}

	/// <summary>
	/// ノベルティ設定一覧表示
	/// </summary>
	private void DisplayBrandList()
	{
		// 検索フォームにパラメータをセット
		tbBrandId.Text = this.RequestBrandId;
		tbBrandName.Text = this.RequestBrandName;

		// ページ番号取得
		int bgnRow = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.RequestPageNum - 1) + 1;
		int endRow = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.RequestPageNum;

		// パラメータセット
		var param = new Hashtable
		{
			{Constants.FIELD_PRODUCTBRAND_BRAND_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(this.RequestBrandId)},
			{Constants.FIELD_PRODUCTBRAND_BRAND_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(this.RequestBrandName)},
			{"bgn_row_num", bgnRow},
			{"end_row_num", endRow}
		};

		// ブランド一覧取得
		var service = new ProductBrandService();
		int totalCount = service.GetSearchHitCount(param);
		var models = service.Search(param);
		rList.DataSource = models;
		rList.DataBind();

		// 件数取得、エラー表示制御
		if (totalCount != 0)
		{
			trListError.Visible = false;
		}
		else
		{
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		// ページャ作成
		string nextUrl = CreateBrandListUrl(this.RequestBrandId, this.RequestBrandName);
		lbPager1.Text = WebPager.CreateDefaultListPager(totalCount, this.RequestPageNum, nextUrl);
	}

	/// <summary>
	/// ブランド一覧URL作成
	/// </summary>
	/// <param name="brandId">ブランドID</param>
	/// <param name="string">ブランド名</param>
	/// <returns>ブランド一覧URL</returns>
	private string CreateBrandListUrl(string brandId, string brandName)
	{
		var url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCT_BRAND_SEARCH);
		url.Append("?").Append(Constants.REQUEST_KEY_PRODUCTBRAND_BRAND_ID).Append("=").Append(HttpUtility.UrlEncode(brandId));
		url.Append("&").Append(Constants.REQUEST_KEY_PRODUCTBRAND_BRAND_NAME).Append("=").Append(HttpUtility.UrlEncode(brandName));

		return url.ToString();
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		var url = CreateBrandListUrl(tbBrandId.Text, tbBrandName.Text);
		Response.Redirect(url);
	}

	#region プロパティ
	/// <summary>リクエスト：ブランドID</summary>
	private string RequestBrandId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTBRAND_BRAND_ID]); }
	}
	/// <summary>リクエスト：ブランド名</summary>
	private string RequestBrandName
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTBRAND_BRAND_NAME]); }
	}
	/// <summary>リクエスト：ページ番号</summary>
	private int RequestPageNum
	{
		get
		{
			int pageNum;
			return int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNum) ? pageNum : 1;
		}
	}
	#endregion
}