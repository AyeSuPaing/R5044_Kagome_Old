/*
=========================================================================================================
  Module      : 商品コンバータ 設定一覧ページ処理(ProductConverterList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using System.Text;

public partial class Form_ProductConverter_ProductConverterList : ProductConverterPage
{
	string m_strActionStatus = null;
	string m_strShopId = null;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// リクエスト取得＆ビューステート格納
		//------------------------------------------------------
		m_strActionStatus = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ACTION_STATUS]);
		ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, m_strActionStatus);

		//------------------------------------------------------
		// ログインしているオペレータのshop_idを取得
		//------------------------------------------------------
		m_strShopId = this.LoginOperatorShopId;

		if (!IsPostBack)
		{
			// 商品コンバータ一覧表示
			ViewProductConverterList();

			// データバインド
			DataBind();
		}
	}

	/// <summary>
	/// 商品コンバータ情報一覧表示
	/// </summary>
	private void ViewProductConverterList()
	{
		Hashtable htParam = new Hashtable();

		//------------------------------------------------------
		// リクエスト情報取得
		//------------------------------------------------------
		htParam = GetParameters(Request);

		// 不正パラメータが存在した場合
		if ((bool)htParam[Constants.ERROR_REQUEST_PRAMETER])
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// ページ番号取得
		//------------------------------------------------------
		int iCurrentPageNumber = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];

		//------------------------------------------------------
		// 商品コンバータ一覧
		//------------------------------------------------------
		int iTotalCount = 0;	// ページング可能総数

		// 商品コンバータデータ取得
		DataView dvProductConverter = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallProductConverter", "GetProductConverter"))
		{
			Hashtable htSearch = new Hashtable();
			htSearch.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iCurrentPageNumber - 1) + 1);										// 表示開始記事番号
			htSearch.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iCurrentPageNumber);										// 表示開始記事番号
			htSearch.Add(Constants.FIELD_MALLPRDCNV_SHOP_ID, m_strShopId);               // 店舗ID

			dvProductConverter = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htSearch);
		}
		if (dvProductConverter.Count != 0)
		{
			iTotalCount = int.Parse(dvProductConverter[0].Row["row_count"].ToString());

			// エラー非表示制御
			trProductConverterListError.Visible = false;
		}
		else
		{
			iTotalCount = 0;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);

			// 一覧非表示・エラー表示制御
			trProductConverterListError.Visible = true;
		}

		// データソースセット
		rProductConverterList.DataSource = dvProductConverter;

		//------------------------------------------------------
		// ページャ作成（一覧処理で総件数を取得）
		//------------------------------------------------------
		string strNextUrl = CreateProductConverterListUrl();
		lbPager1.Text = WebPager.CreateDefaultListPager(iTotalCount, iCurrentPageNumber, strNextUrl);
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTCONVERTER_DETAIL);
		sbUrl.Append("?");
		sbUrl.Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_INSERT);
		
		// 編集画面へ
		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	/// 商品コンバータ一覧遷移URL作成
	/// </summary>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>商品コンバータ一覧遷移URL</returns>
	private string CreateProductConverterListUrl(int iPageNumber)
	{
		return CreateProductConverterListUrl() + iPageNumber;
	}
	/// <summary>
	/// 商品コンバータ一覧遷移URL作成
	/// </summary>
	/// <returns>商品コンバータ一覧遷移URL</returns>
	private string CreateProductConverterListUrl()
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTCONVERTER_LIST);

		return sbResult.ToString();
	}

	/// <summary>
	/// データバインド用商品コンバータ詳細URL作成
	/// </summary>
	/// <param name="strAdtoId">商品コンバータID</param>
	/// <returns>商品コンバータ詳細URL</returns>
	protected string CreateProductConverterDetailUrl(string strAdtoId)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTCONVERTER_DETAIL);
		sbResult.Append("?");
		sbResult.Append(Constants.REQUEST_KEY_ADTO_ID).Append("=").Append(HttpUtility.UrlEncode(strAdtoId));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);

		return sbResult.ToString();
	}

	/// <summary>
	/// データバインド用商品コンバータ詳細URL作成
	/// </summary>
	/// <param name="strAdtoId">商品コンバータID</param>
	/// <returns>商品コンバータ詳細URL</returns>
	protected string CreateProductConverterFilesUrl(string strAdtoId)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTCONVERTER_FILES);
		sbResult.Append("?");
		sbResult.Append(Constants.REQUEST_KEY_ADTO_ID).Append("=").Append(HttpUtility.UrlEncode(strAdtoId));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);

		return sbResult.ToString();
	}
}
