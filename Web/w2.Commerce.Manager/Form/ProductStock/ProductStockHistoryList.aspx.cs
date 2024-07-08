/*
=========================================================================================================
  Module      : 商品在庫履歴一覧ページ処理(ProductStockHistoryList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class Form_ProductStock_ProductStockHistoryList : ProductPage
{
	//=========================================================================================
	// 表示定数
	//=========================================================================================
	protected const string DISPLAY_PRODUCTSTOCK = "productstock";
	protected const string DISPLAY_PRODUCTREALSTOCK = "productrealstock";
	protected const string DISPLAY_PRODUCTREALSTOCK_B = "productrealstock_b";
	protected const string DISPLAY_PRODUCTREALSTOCK_C = "productrealstock_c";
	protected const string DISPLAY_PRODUCTREALSTOCK_RESERVED = "productrealstock_reserved";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// 商品在庫履歴情報一覧表示
			ViewProductStockHistoryList();
		}
	}

	/// <summary>
	/// 商品在庫履歴情報一覧表示(DataGridにDataView(商品在庫履歴情報)を設定)
	/// </summary>
	private void ViewProductStockHistoryList()
	{
		// 変数宣言
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
		// 検索情報取得
		//------------------------------------------------------
		Hashtable htSearch = GetSearchSqlInfo(htParam);
		int iCurrentPageNumber = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];

		//------------------------------------------------------
		// 商品在庫履歴情報一覧
		//------------------------------------------------------
		int iTotalCounts = 0;	// ページング可能総商品数

		// 商品在庫履歴情報データ取得
		DataView dv = GetProductStockHistoryListDataView(htSearch, iCurrentPageNumber);

		// 商品在庫履歴情報データが存在する場合
		if (dv.Count != 0)
		{
			iTotalCounts = int.Parse(dv[0].Row["row_count"].ToString());
			// エラー非表示制御
			trListError.Visible = false;

			// 商品ID、バリエーションID、商品名表示制御
			lbProductId.Text = (string)htParam[Constants.REQUEST_KEY_PRODUCT_ID];
			lbVariationId.Text = (string)htParam[Constants.REQUEST_KEY_VARIATION_ID];
			lbName.Text = CreateProductAndVariationName(dv[0]);
		}
		else
		{
			iTotalCounts = 0;
			// エラー表示制御
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);

			// 商品ID、バリエーションID表示制御
			trProduct.Visible = false;
		}

		// データソースセット
		rList.DataSource = dv;
		rList.DataBind();

		//------------------------------------------------------
		// ページャ作成（一覧処理で総件数を取得）
		//------------------------------------------------------
		string strNextUrl = CreateProductStockHistoryListUrl(htParam);
		lbPager1.Text = WebPager.CreateDefaultListPager(iTotalCounts, iCurrentPageNumber, strNextUrl);
	}

	/// <summary>
	///　商品在庫履歴一覧パラメタ取得
	/// </summary>
	/// <param name="hrRequest">商品在庫履歴一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	/// <remarks>
	/// </remarks>
	protected Hashtable GetParameters(System.Web.HttpRequest hrRequest)
	{
		// 変数宣言
		Hashtable htResult = new Hashtable();

		int iCurrentPageNumber = 1;
		string strSortKbn = String.Empty;
		bool blParamError = false;

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		// 商品ID
		try
		{
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_PRODUCT_ID]));
		}
		catch
		{
			blParamError = true;
		}
		// バリエーションID
		try
		{
			htResult.Add(Constants.REQUEST_KEY_VARIATION_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_VARIATION_ID]));
		}
		catch
		{
			blParamError = true;
		}
		// ページ番号（ページャ動作時のみもちまわる）
		try
		{
			if (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_PAGE_NO]) != "")
			{
				iCurrentPageNumber = int.Parse(hrRequest[Constants.REQUEST_KEY_PAGE_NO]);
			}
		}
		catch
		{
			blParamError = true;
		}

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		htResult.Add(Constants.REQUEST_KEY_PAGE_NO, iCurrentPageNumber);
		htResult.Add(Constants.ERROR_REQUEST_PRAMETER, blParamError);

		return htResult;
	}

	/// <summary>
	/// 商品在庫履歴情報一覧データビューを表示分だけ取得
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>商品在庫履歴情報一覧データビュー</returns>
	private DataView GetProductStockHistoryListDataView(Hashtable htSearch, int iPageNumber)
	{
		// 変数宣言
		DataView dvResult = null;

		int iBgn = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iPageNumber - 1) + 1;
		int iEnd = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iPageNumber;

		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductStock", "GetProductStockHistoryList"))
		{
			htSearch.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.LoginOperatorShopId);	// 店舗ID
			htSearch.Add("bgn_row_num", iBgn);										// 表示開始記事番号
			htSearch.Add("end_row_num", iEnd);										// 表示開始記事番号

			// SQL発行
			dvResult = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htSearch);
		}

		return dvResult;
	}

	/// <summary>
	/// 検索値取得
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchSqlInfo(Hashtable htSearch)
	{
		// 変数宣言
		Hashtable htResult = new Hashtable();
		string strDate = String.Empty;

		//------------------------------------------------------
		// 検索情報取得
		//------------------------------------------------------
		// 商品ID
		htResult.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID, StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_PRODUCT_ID]));
		// バリエーションID
		htResult.Add(Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID, StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_VARIATION_ID]));

		return htResult;
	}

	/// <summary>
	/// データバインド用商品一覧遷移URL作成
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <returns>注文一覧遷移URL</returns>
	private string CreateProductStockHistoryListUrl(Hashtable htSearch)
	{
		string strResult = "";
		strResult += Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSTOCKHISTORY_LIST;
		strResult += "?";
		strResult += Constants.REQUEST_KEY_PRODUCT_ID + "=" + HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_PRODUCT_ID]);
		strResult += "&";
		strResult += Constants.REQUEST_KEY_VARIATION_ID + "=" + HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_VARIATION_ID]);

		return strResult;
	}

	/// <summary>
	/// 在庫増減、更新表示
	/// </summary>
	/// <param name="drv">商品在庫履歴</param>
	/// <param name="strDisplayStock">表示項目</param>
	/// <returns></returns>
	/// <remarks>
	/// アクションステータスが「マスタ取込」の場合は、更新数、それ以外の場合は増減数を表示
	/// </remarks>
	protected string DisplayProductStock(DataRowView drv, string strDisplayStock)
	{
		string strResult = null;

		string strFieldUpdateStock = null;
		string strFieldAddStock = null;
		switch (strDisplayStock)
		{
			// 論理在庫
			case DISPLAY_PRODUCTSTOCK:
				strFieldUpdateStock = Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_STOCK;
				strFieldAddStock = Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK;
				break;

			// 実在庫
			case DISPLAY_PRODUCTREALSTOCK:
				strFieldUpdateStock = Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK;
				strFieldAddStock = Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK;
				break;

			// 実在庫B
			case DISPLAY_PRODUCTREALSTOCK_B:
				strFieldUpdateStock = Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_B;
				strFieldAddStock = Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_B;
				break;

			// 実在庫C
			case DISPLAY_PRODUCTREALSTOCK_C:
				strFieldUpdateStock = Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_C;
				strFieldAddStock = Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_C;
				break;

			// 引当済み実在庫
			case DISPLAY_PRODUCTREALSTOCK_RESERVED:
				strFieldUpdateStock = Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_RESERVED;
				strFieldAddStock = Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_RESERVED;
				break;
		}

		if (drv[strFieldUpdateStock] != DBNull.Value)
		{
			strResult = string.Format(
				"<span style=\"color:Red\">{0}{1}</span>",
				drv[strFieldUpdateStock],
				//「(更新)」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_PRODUCT_STOCK,
					Constants.VALUETEXT_PARAM_PRODUCT_STOCK_HISTORY_LIST,
					Constants.VALUETEXT_PARAM_PRODUCT_STOCK_UPDATE));
		}
		else
		{
			int iAddStock = (int)drv[strFieldAddStock];
			if (iAddStock == 0)
			{
				strResult = "-";
			}
			else if (iAddStock > 0)
			{
				strResult = "+" + iAddStock;
			}
			else
			{
				strResult = iAddStock.ToString();
			}
		}

		return strResult;
	}
}