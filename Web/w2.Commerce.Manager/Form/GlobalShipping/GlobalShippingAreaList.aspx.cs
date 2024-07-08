/*
=========================================================================================================
  Module      : 海外配送エリア一覧ページ(GlobalShippingAreaList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.Domain.GlobalShipping;
using w2.Domain.GlobalShipping.Helper;

/// <summary>
/// 海外配送エリア一覧ページ
/// </summary>
public partial class Form_Shipping_ShippingList : BaseGlobalShippingPage
{
	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender">イベント発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// 一覧表示
			ViewList();
		}
	}
	#endregion

	#region #btnInsert_Click 新規登録ボタンクリック
	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender">イベント発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

		// 新規登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_GLOBAL_SHIPPING_AREA_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}
	#endregion

	/// <summary>
	/// 一覧表示
	/// </summary>
	private void ViewList()
	{
		var para = new Hashtable();
		para = GetParameters(Request);

		// 不正パラメータが存在した場合
		if ((bool)para[Constants.ERROR_REQUEST_PRAMETER])
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		int pNo = (int)para[Constants.REQUEST_KEY_PAGE_NO];

		int totalShippingCounts = 0;

		var searchCount = this.GetSearchCount();
		totalShippingCounts = searchCount;

		if (searchCount == 0)
		{
			// エラー表示制御
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			return;
		}

		trListError.Visible = false;

		var data = GetData(pNo);
		rList.DataSource = data;

		string strNextUrl = Constants.PATH_ROOT + Constants.PAGE_MANAGER_GLOBAL_SHIPPING_AREA_LIST;
		lbPager1.Text = WebPager.CreateDefaultListPager(totalShippingCounts, pNo, strNextUrl);

		DataBind();
	}

	/// <summary>
	/// パラメタ取得
	/// </summary>
	/// <param name="hrRequest">HttpRequest</param>
	/// <returns>Hashtable</returns>
	private static Hashtable GetParameters(System.Web.HttpRequest hrRequest)
	{
		// 変数宣言
		Hashtable res = new Hashtable();

		int currentPNo = 1;
		bool blParamError = false;

		// ページ番号（ページャ動作時のみもちまわる）
		try
		{
			if (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_PAGE_NO]) != "")
			{
				currentPNo = int.Parse(hrRequest[Constants.REQUEST_KEY_PAGE_NO]);
			}
		}
		catch
		{
			blParamError = true;
		}

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		res.Add(Constants.REQUEST_KEY_PAGE_NO, currentPNo);
		res.Add(Constants.ERROR_REQUEST_PRAMETER, blParamError);

		return res;
	}

	/// <summary>
	/// 検索結果件数取得
	/// </summary>
	/// <returns>件数</returns>
	private int GetSearchCount()
	{
		var sv = new GlobalShippingService();
		var cond = new GlobalShippingAreaListSearchCondition();
		var rtn = sv.GetSearchGlobalShippingAreaListCount(cond);
		return rtn;
	}

	/// <summary>
	/// 一覧データ取得
	/// </summary>
	/// <param name="pNo">ページ番号</param>
	/// <returns>一覧用の海外配送エリア検索結果モデル</returns>
	private GlobalShippingAreaListSearchResult[] GetData(int pNo)
	{
		int begin = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (pNo - 1) + 1;
		int end = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * pNo;

		var sv = new GlobalShippingService();
		var cond = new GlobalShippingAreaListSearchCondition();
		cond.BeginRowNumber = begin;
		cond.EndRowNumber = end;
		var rtn = sv.SearchGlobalShippingAreaList(cond);
		return rtn;
	}

}
