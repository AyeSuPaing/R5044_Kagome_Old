/*
=========================================================================================================
  Module      : 商品一覧ページ処理(ProductList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
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
using w2.App.Common.Order;

public partial class Form_OrderConditionReport_ProductList : BasePage
{
	/// <summary>商品IDを含まないバリエーションID</summary>
	public const string FIELD_PRODUCTVARIATION_V_ID = "v_id";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			Hashtable htParam = GetParameters();

			if (this.IsNotSearchDefault) return;

			//------------------------------------------------------
			// SQL検索パラメータ取得
			//------------------------------------------------------
			Hashtable htSqlParam = new Hashtable();
			htSqlParam.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.LoginOperatorShopId);
			htSqlParam.Add(Constants.FIELD_PRODUCT_PRODUCT_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htParam[Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID]));
			htSqlParam.Add(Constants.FIELD_PRODUCT_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htParam[Constants.REQUEST_KEY_PRODUCT_NAME]));
			htSqlParam.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNo - 1) + 1);
			htSqlParam.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNo);

			//------------------------------------------------------
			// 商品該当件数取得・エラー制御
			//------------------------------------------------------
			int iTotalProductCounts = 0;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("OrderConditionReport", "GetProductCount"))
			{
				DataView dvProductCount = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htSqlParam);
				if (dvProductCount.Count != 0)
				{
					iTotalProductCounts = (int)dvProductCount[0]["row_count"];
				}
			}

			// 上限件数より多い場合、エラー表示
			bool blDisplayPager = true;
			StringBuilder sbErrorMessage = new StringBuilder();
			if (iTotalProductCounts > Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST)
			{
				sbErrorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_OVER_HIT_LIST));
				sbErrorMessage.Replace("@@ 1 @@", StringUtility.ToNumeric(iTotalProductCounts));
				sbErrorMessage.Replace("@@ 2 @@", StringUtility.ToNumeric(Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST));

				blDisplayPager = false;
			}
			//  該当件数なしの場合、エラー表示
			else if (iTotalProductCounts == 0)
			{
				sbErrorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST));
			}
			tdErrorMessage.InnerHtml = sbErrorMessage.ToString();
			trListError.Visible = (sbErrorMessage.ToString().Length != 0);

			//------------------------------------------------------
			// 商品一覧表示
			//------------------------------------------------------
			if (trListError.Visible == false)
			{
				// 商品情報取得・データバインド
				DataView dvProduct = null;
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("OrderConditionReport", "GetProductList"))
				{
					dvProduct = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htSqlParam);
				}
				rList.DataSource = dvProduct;
				rList.DataBind();
			}

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			if (blDisplayPager)
			{
				lbPager1.Text = WebPager.CreateDefaultListPager(
					iTotalProductCounts,
					this.CurrentPageNo,
					CreateProductSearchListUrl(htParam));
			}
		}
	}

	/// <summary>
	/// 商品一覧パラメタ取得
	/// </summary>
	/// <param name="hrRequest">商品一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParameters()
	{
		Hashtable htParams = new Hashtable();

		// 商品ID
		htParams.Add(Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID]));
		tbProductId.Text = Request[Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID];
		// 商品名
		htParams.Add(Constants.REQUEST_KEY_PRODUCT_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_NAME]));
		tbName.Text = Request[Constants.REQUEST_KEY_PRODUCT_NAME];
		// ページ番号（ページャ動作時のみもちまわる）
		int iCurrentPageNumber;
		if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out iCurrentPageNumber) == false)
		{
			iCurrentPageNumber = 1;
		}
		htParams.Add(Constants.REQUEST_KEY_PAGE_NO, iCurrentPageNumber);
		this.CurrentPageNo = iCurrentPageNumber;

		return htParams;
	}

	/// <summary>
	/// 商品一覧遷移URL作成
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>商品一覧遷移URL</returns>
	private string CreateProductSearchListUrl(Hashtable htParam, int iPageNumber)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(CreateProductSearchListUrl(htParam));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(iPageNumber.ToString());

		return sbUrl.ToString();
	}
	/// <summary>
	/// 商品一覧遷移URL作成
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <returns>商品一覧遷移URL</returns>
	private string CreateProductSearchListUrl(Hashtable htParam)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_PRODUCTLIST);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID]));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_NAME).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_PRODUCT_NAME]));

		return sbUrl.ToString();
	}

	/// <summary>
	/// 選択された商品情報を設定パラメータ作成
	/// </summary>
	/// <param name="drv">商品情報</param>
	/// <returns>商品情報を設定パラメータ</returns>
	protected string CreateJavaScriptSetProductInfo(DataRowView drv)
	{
		StringBuilder sbJavaScriptParam = new StringBuilder();
		sbJavaScriptParam.Append("'").Append(StringUtility.ToEmpty(drv[Constants.FIELD_PRODUCT_PRODUCT_ID]).Replace("'", "\\'")).Append("'");

		return sbJavaScriptParam.ToString();
	}

	/// <summary>
	/// 検索実行イベントハンドラ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ProductSearch(object sender, EventArgs e)
	{
		Hashtable htParam = new Hashtable();
		htParam.Add(Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID, tbProductId.Text.Trim());
		htParam.Add(Constants.REQUEST_KEY_PRODUCT_NAME, tbName.Text.Trim());

		// 検索用パラメタ作成し、同じ画面にリダイレクト
		Response.Redirect(CreateProductSearchListUrl(htParam, 1));
	}

	//=========================================================================================
	/// <summary>
	/// 現在有効な価格を取得
	/// </summary>
	/// <param name="drvProduct">商品情報</param>
	/// <returns> 現在有効な価格</returns>
	/// <remarks>
	/// 価格の優先度は、セール価格>特別価格>通常価格(バリエーション)
	/// </remarks>
	//=========================================================================================
	protected decimal GetProductValidityPrice(DataRowView drvProduct)
	{
		// 特別価格が有効？
		if (drvProduct[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE] != DBNull.Value)
		{
			return (decimal)drvProduct[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE];
		}

		// 通常価格が有効
		return (decimal)drvProduct[Constants.FIELD_PRODUCT_DISPLAY_PRICE];
	}
	
	/// <summary>カレントページNO</summary>
	protected int CurrentPageNo
	{
		get { return (int)ViewState["CurrentPangeNo"]; }
		set { ViewState["CurrentPangeNo"] = value; }
	}
}
