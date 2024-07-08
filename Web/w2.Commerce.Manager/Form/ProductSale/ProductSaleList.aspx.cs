/*
=========================================================================================================
  Module      : 商品セール情報一覧ページ処理(ProductSaleList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using w2.Domain;

public partial class Form_ProductSale_ProductSaleList : BasePage
{
	private string m_strSortKbn = null;
	private int m_iCurrentPageNumber = 1;

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
			string strProductSaleKbn = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_KBN]);
			string strProductSaleInfo = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_INFO]);
			string strProductSaleOpend = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_OPENED]);
			switch (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]))
			{
				case Constants.KBN_SORT_PRODUCTSALE_DATE_BGN_ASC:
				case Constants.KBN_SORT_PRODUCTSALE_DATE_BGN_DESC:
				case Constants.KBN_SORT_PRODUCTSALE_ID_ASC:
				case Constants.KBN_SORT_PRODUCTSALE_ID_DESC:
					m_strSortKbn = Request[Constants.REQUEST_KEY_SORT_KBN];
					break;
				default:
					m_strSortKbn = Constants.KBN_SORT_PRODUCTSALE_DEFAULT;
					break;
			}
			if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out m_iCurrentPageNumber) == false)
			{
				m_iCurrentPageNumber = 1;
			}

			//------------------------------------------------------
			// コンポーネント設定
			//------------------------------------------------------
			foreach (ListItem li in rblProductSaleKbn.Items)
			{
				li.Selected = (li.Value == strProductSaleKbn);
			}
			if (rblProductSaleKbn.SelectedIndex == -1)
			{
				rblProductSaleKbn.SelectedIndex = 0;
			}

			tbProductSaleInfo.Text = strProductSaleInfo;

			foreach (ListItem li in ddlProductSaleOpend.Items)
			{
				li.Selected = (li.Value == strProductSaleOpend);
			}
			foreach (ListItem li in ddlSortKbn.Items)
			{
				li.Selected = (li.Value == m_strSortKbn);
			}

			//------------------------------------------------------
			// 検索情報保持(編集で利用)
			//------------------------------------------------------
			Hashtable htRequestParam = new Hashtable();
			htRequestParam.Add(Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_KBN, strProductSaleKbn);
			htRequestParam.Add(Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_INFO, strProductSaleInfo);
			htRequestParam.Add(Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_OPENED, strProductSaleOpend);
			htRequestParam.Add(Constants.REQUEST_KEY_SORT_KBN, m_strSortKbn);
			htRequestParam.Add(Constants.REQUEST_KEY_PAGE_NO, m_iCurrentPageNumber);
			Session[Constants.SESSIONPARAM_KEY_PRODUCTSALE_INFO] = htRequestParam;

			//------------------------------------------------------
			// 商品セール情報取得
			//------------------------------------------------------
			// ステートメントからデータ総数を取得
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTSALE_SHOP_ID, this.LoginOperatorShopId },
				{ Constants.FIELD_PRODUCTSALE_PRODUCTSALE_KBN, strProductSaleKbn },
				{ "sale_info_like_escaped", StringUtility.SqlLikeStringSharpEscape(strProductSaleInfo) },
				{ "sale_opened", strProductSaleOpend },
				{ "bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (m_iCurrentPageNumber - 1) + 1 },
				{ "end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * m_iCurrentPageNumber },
				{ "sort_kbn", m_strSortKbn },
			};

			var dvProductSaleList = DomainFacade.Instance.ProductSaleService.GetProductSaleList(input);
			var totalCount = DomainFacade.Instance.ProductSaleService.GetProductSaleCount(input);

			// Redirect to last page when current page no don't have any data
			CheckRedirectToLastPage(
				dvProductSaleList.Count,
				totalCount,
				CreateListUrl(htRequestParam));

			//------------------------------------------------------
			// 画面表示
			//------------------------------------------------------
			// データバインド
			rList.DataSource = dvProductSaleList;
			rList.DataBind();

			// 総件数取得・エラー表示制御
			int iTotalCounts = 0;
			if (dvProductSaleList.Count != 0)
			{
				iTotalCounts = (int)dvProductSaleList[0]["row_count"];

				trListError.Visible = false;
			}
			else
			{
				iTotalCounts = 0;

				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}

			// ページャ作成
			string strNextUrl = CreateListUrl((Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCTSALE_INFO]);
			lbPager1.Text = WebPager.CreateDefaultListPager(iTotalCounts, m_iCurrentPageNumber, strNextUrl);
		}
	}

	/// <summary>
	/// 詳細URL作成
	/// </summary>
	/// <param name="strProductSaleKbn"></param>
	/// <param name="strProductSaleId"></param>
	/// <returns></returns>
	protected string CreateDetailUrl(string strProductSaleKbn, string strProductSaleId)
	{
		StringBuilder sbResultUrl = new StringBuilder();
		sbResultUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTSALE_REGISTER);
		sbResultUrl.Append("?").Append(Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_ID).Append("=").Append(HttpUtility.UrlEncode(strProductSaleId));
		sbResultUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_UPDATE);

		return sbResultUrl.ToString();
	}

	/// <summary>
	/// 一覧遷移URL作成
	/// </summary>
	/// <param name="htParam">検索情報</param>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>URL</returns>
	protected string CreateListUrl(Hashtable htParam, int iPageNumber)
	{
		return CreateListUrl(htParam) + "&" + Constants.REQUEST_KEY_PAGE_NO + "=" + iPageNumber.ToString();
	}
	/// <summary>
	/// 一覧遷移URL作成
	/// </summary>
	/// <param name="htParam">検索情報</param>
	/// <returns>注文一覧遷移URL</returns>
	protected string CreateListUrl(Hashtable htParam)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTSALE_LIST);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_KBN).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_KBN]));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_INFO).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_INFO]));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_OPENED).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_OPENED]));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_SORT_KBN]));

		return sbResult.ToString();
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		Hashtable htParam = new Hashtable();
		htParam.Add(Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_KBN, rblProductSaleKbn.SelectedValue);
		htParam.Add(Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_INFO, tbProductSaleInfo.Text);
		htParam.Add(Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_OPENED, ddlProductSaleOpend.SelectedValue);
		htParam.Add(Constants.REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue);

		Response.Redirect(CreateListUrl(htParam, 1));
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		StringBuilder sbResultUrl = new StringBuilder();
		sbResultUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTSALE_REGISTER);
		sbResultUrl.Append("?").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_INSERT);

		Response.Redirect(sbResultUrl.ToString());
	}
}
