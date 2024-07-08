/*
=========================================================================================================
  Module      : 商品在庫文言情報一覧ページ処理(ProductStockMessageList.aspx.cs)
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

public partial class Form_ProductStockMessage_ProductStockMessageList : BasePage
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
			// 商品在庫文言情報一覧表示
			ViewProductStockMessageList();
		}
	}

	/// <summary>
	/// 商品在庫文言情報一覧表示(DataGridにDataView(商品在庫文言情報)を設定)
	/// </summary>
	private void ViewProductStockMessageList()
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
		int iCurrentPageNumber = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];

		//------------------------------------------------------
		// 商品在庫文言一覧
		//------------------------------------------------------
		int iTotalCounts = 0;	// ページング可能総商品数
		// 商品在庫文言データ取得
		DataView dv = GetProductStockMessageDataView(iCurrentPageNumber);
		if (dv.Count != 0)
		{
			iTotalCounts = int.Parse(dv[0].Row["row_count"].ToString());
			// エラー非表示制御
			trListError.Visible = false;
		}
		else
		{
			// エラー表示制御
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		//------------------------------------------------------
		// ページャ作成（一覧処理で総件数を取得）
		//------------------------------------------------------
		string strNextUrl = Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSTOCKMESSAGE_LIST;
		lbPager1.Text = WebPager.CreateDefaultListPager(iTotalCounts, iCurrentPageNumber, strNextUrl);

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		rList.DataSource = dv;
		DataBind();

	}

	/// <summary>
	/// 商品在庫文言一覧パラメタ取得
	/// </summary>
	/// <param name="hrRequest">商品在庫文言一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	private static Hashtable GetParameters(System.Web.HttpRequest hrRequest)
	{
		// 変数宣言
		Hashtable htResult = new Hashtable();

		int iCurrentPageNumber = 1;
		bool blParamError = false;

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
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
	/// 商品在庫文言情報データビューを表示分だけ取得
	/// </summary>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>商品在庫文言情報データビュー</returns>
	private DataView GetProductStockMessageDataView(int iPageNumber)
	{
		// 変数宣言
		DataView dvResult = null;

		int iBgn = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iPageNumber - 1) + 1;
		int iEnd = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iPageNumber;

		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductStockMessage", "GetProductStockMessageList"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_SHOP_ID, this.LoginOperatorShopId);	// 店舗ID
			htInput.Add("bgn_row_num", iBgn);										// 表示開始記事番号
			htInput.Add("end_row_num", iEnd);										// 表示開始記事番号

			// SQL発行
			dvResult = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		return dvResult;
	}

	/// <summary>
	/// データバインド用商品在庫文言情報詳細URL作成
	/// </summary>
	/// <param name="strStockMessageId">商品在庫文言ID</param>
	/// <returns>商品在庫文言情報詳細URL</returns>
	protected string CreateProductStockMessageDetailUrl(string strStockMessageId)
	{
		string strResult = "";
		strResult += Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSTOCKMESSAGE_CONFIRM;
		strResult += "?";
		strResult += Constants.REQUEST_KEY_STOCK_MESSAGE_ID + "=" + strStockMessageId;
		strResult += "&";
		strResult += Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_DETAIL;

		return strResult;
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertTop_Click(object sender, EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

		// 新規登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSTOCKMESSAGE_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}
}
