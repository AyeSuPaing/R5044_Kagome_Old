/*
=========================================================================================================
  Module      : モール連携 商品アップロード商品情報一覧ページ処理(MallProductUploadProductList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Form_MallProductUpload_MallProductUploadProductList : ProductPage
{
	private int m_iCurrentPageNumber = 1;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// セッション情報が存在チェック
			//------------------------------------------------------
			// モール商品アップロード画面での検索情報が存在しない？
			if (Session[Constants.SESSIONPARAM_KEY_MALLPRODUCTUPLOAD_SEARCH_INFO] == null)
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR + "?" + Constants.REQUEST_KEY_WINDOW_KBN + "=" + HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP));
			}

			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			// 商品ID
			tbProductId.Text = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_ID]).Trim();
			// 商品名
			tbName.Text = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_NAME]).Trim();
			// 商品一覧表示区分
			switch(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MALLPRODUCTUPLOAD_DIPLAY_KBN]))
			{
				case Constants.KBN_MALLPRODUCTUPLOAD_DIPLAY_KBN_INSERT:
				case Constants.KBN_MALLPRODUCTUPLOAD_DIPLAY_KBN_UPDATE:
					hfDisplayKbn.Value = Request[Constants.REQUEST_KEY_MALLPRODUCTUPLOAD_DIPLAY_KBN];
					break;
				default:
					hfDisplayKbn.Value = Constants.KBN_MALLPRODUCTUPLOAD_DIPLAY_KBN_INSERT;
					break;
			}
			// ページNo
			if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out m_iCurrentPageNumber) == false)
			{
				m_iCurrentPageNumber = 1;
			}

			//------------------------------------------------------
			// 商品一覧情報取得
			//------------------------------------------------------
			DataView dvProductList = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MallCooperationUpdateLog", "MallProductUploadProductList"))
			{
				Hashtable htInput = (Hashtable)((Hashtable)Session[Constants.SESSIONPARAM_KEY_MALLPRODUCTUPLOAD_SEARCH_INFO]).Clone();
				htInput.Add(Constants.FIELD_PRODUCT_PRODUCT_ID + "_like_escaped2", StringUtility.SqlLikeStringSharpEscape(tbProductId.Text));
				htInput.Add(Constants.FIELD_PRODUCT_NAME + "_like_escaped2", StringUtility.SqlLikeStringSharpEscape(tbName.Text));
				htInput.Add("disp_kbn", hfDisplayKbn.Value);
				htInput.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (m_iCurrentPageNumber - 1) + 1);
				htInput.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * m_iCurrentPageNumber);

				dvProductList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			//------------------------------------------------------
			// 画面表示
			//------------------------------------------------------
			// データバインド
			rList.DataSource = dvProductList;
			rList.DataBind();

			// 総件数取得・エラー表示制御
			int iTotalCounts = 0;
			if (dvProductList.Count != 0)
			{
				iTotalCounts = (int)dvProductList[0]["row_count"];

				trListError.Visible = false;
			}
			else
			{
				iTotalCounts = 0;

				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}

			// ページャ作成
			string strNextUrl = CreateListUrl();
			lbPager1.Text = WebPager.CreateDefaultListPager(iTotalCounts, m_iCurrentPageNumber, strNextUrl);
		}
	}

	/// <summary>
	/// 商品一覧遷移URL作成
	/// </summary>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>商品一覧遷移URL</returns>
	protected string CreateListUrl(int iPageNumber)
	{
		return CreateListUrl() + "&" + Constants.REQUEST_KEY_PAGE_NO + "=" + iPageNumber.ToString();
	}
	/// <summary>
	/// 商品一覧遷移URL作成
	/// </summary>
	/// <returns>商品一覧遷移URL</returns>
	protected string CreateListUrl()
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_MALL_PRODUCT_UPLOAD_PRODUCT_LIST);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode(tbProductId.Text));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_NAME).Append("=").Append(HttpUtility.UrlEncode(tbName.Text));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_MALLPRODUCTUPLOAD_DIPLAY_KBN).Append("=").Append(HttpUtility.UrlEncode(hfDisplayKbn.Value));

		return sbResult.ToString();
	}

	/// <summary>
	/// 検索実行イベントハンドラ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateListUrl(1));
	}
}
