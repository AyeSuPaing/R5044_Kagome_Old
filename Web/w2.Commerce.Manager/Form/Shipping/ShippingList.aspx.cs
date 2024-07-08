/*
=========================================================================================================
  Module      : 配送種別情報一覧ページ(ShippingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;

public partial class Form_Shipping_ShippingList : ShopShippingPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// 配送種別情報一覧表示
			ViewShippingList();
		}
	}

	/// <summary>
	/// 配送種別情報一覧表示(DataGridにDataView(配送種別情報)を設定)
	/// </summary>
	private void ViewShippingList()
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
			Session[Constants.SESSION_KEY_ERROR_MSG] = 
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		int iCurrentPageNumber = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];

		//------------------------------------------------------
		// 配送種別一覧
		//------------------------------------------------------
		int iTotalShippingCounts = 0;	// ページング可能総商品数

		// 配送種別データ取得
		DataView dvShipping = GetShippingDataView(iCurrentPageNumber);
		
		if (dvShipping.Count != 0)
		{
			iTotalShippingCounts = int.Parse(dvShipping[0].Row["row_count"].ToString());
			// エラー非表示制御
			trListError.Visible = false;
		}
		else
		{
			iTotalShippingCounts = 0;

			// エラー表示制御
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		// データソースセット
		rList.DataSource = dvShipping;

		//------------------------------------------------------
		// ページャ作成（一覧処理で総件数を取得）
		//------------------------------------------------------
		string strNextUrl = Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_LIST;
		lbPager1.Text = WebPager.CreateDefaultListPager(iTotalShippingCounts, iCurrentPageNumber, strNextUrl);

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		DataBind();
	}


	/// <summary>
	/// 配送種別一覧パラメタ取得
	/// </summary>
	/// <param name="hrRequest">配送種別一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	/// <remarks>
	/// </remarks>
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
	/// 配送種別一覧データビューを表示分だけ取得
	/// </summary>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>配送種別一覧データビュー</returns>
	private DataView GetShippingDataView(int iPageNumber) 
	{
		// 変数宣言
		DataView dvResult = null;

		int iBgn = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iPageNumber - 1) + 1;
		int iEnd = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iPageNumber;

		// ステートメントからカテゴリデータ取得
		using(SqlAccessor sqlAccessor = new SqlAccessor())
		using(SqlStatement sqlStatement = new SqlStatement("ShopShipping","GetShippingList"))
		{
			Hashtable htInput = new Hashtable();

			htInput.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.LoginOperatorShopId);	// 店舗ID
			htInput.Add("bgn_row_num", iBgn);										// 表示開始記事番号
			htInput.Add("end_row_num", iEnd);										// 表示開始記事番号

			// SQL発行
			DataSet ds = sqlStatement.SelectStatementWithOC(sqlAccessor, htInput);
			dvResult = ds.Tables["Table"].DefaultView;
		}

		return dvResult;
	}

	/// <summary>
	/// データバインド用商品詳細URL作成
	/// </summary>
	/// <param name="strShippingId">配送種別設定ID</param>
	/// <returns>配送種別詳細URL</returns>
	protected string CreateShippingDetailUrl(string strShippingId)
	{
		string strResult = "";
		strResult += Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_CONFIRM;
		strResult += "?";
		strResult += Constants.REQUEST_KEY_SHIPPING_ID + "=" + strShippingId;
		strResult += "&";
		strResult += Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_DETAIL;

		return strResult;
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

		// セッション配送種別情報リセット
		this.ShippingInfoInSession = null;

		// 新規登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_REGISTER + "?" + 
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}
}
