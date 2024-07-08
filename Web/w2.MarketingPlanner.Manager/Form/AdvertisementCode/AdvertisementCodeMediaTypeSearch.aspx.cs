/*
=========================================================================================================
  Module      : 広告媒体区分検索ページ(AdvertisementCodeMediaTypeSearch.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls;
using System.Text;
using w2.Common.Web;
using w2.Domain.AdvCode;
using w2.Domain.AdvCode.Helper;

/// <summary>
/// 広告媒体区分検索ページ
/// </summary>
public partial class Form_Common_AdvertisementCodeMediaTypeSearch : BasePage
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
			// コンポーネント初期化
			InitializeComponents();

			// リクエスト情報取得
			var parameters = GetParameters();

			// 検索値を画面にセット
			SetParameters(parameters);

			// 広告コードデータ取得
			var service = new AdvCodeService();
			var searchCondition = CreateSearchCondition(parameters);
			var dataList = service.SearchAdvCodeMediaType(searchCondition);
			var totalCounts = service.GetAdvCodeMediaTypeSearchHitCount(searchCondition);

			// 一覧非表示・エラー表示制御
			if (dataList.Length != 0)
			{
				trListError.Visible = false;
			}
			else
			{
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}

			// データソースセット
			rAdvCodeList.DataSource = dataList;

			// ページャ作成（一覧処理で総件数を取得）
			var nextUrl = CreateListUrl(parameters);
			lbPager.Text = WebPager.CreateDefaultListPager(totalCounts, this.CurrentPageNumber, nextUrl);

			// データバインド
			DataBind();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 並び順ドロップダウン設定
		ddlSearchSortKbn.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_ADVCODE, Constants.REQUEST_KEY_SORT_KBN));
	}

	/// <summary>
	/// 画面から検索情報取得
	/// </summary>
	/// <returns>検索情報が格納されたHashtable</returns>
	private Hashtable GetSearchInfo()
	{
		var searchParams = new Hashtable
		{
			{ Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID, tbSearchAdvCodeMediaTypeId.Text.Trim() },
			{ Constants.REQUEST_KEY_ADVCODEMEDIATYPE_MEDIA_TYPE_NAME, tbSearchAdvCodeMediaTypeName.Text.Trim() },
			{ Constants.REQUEST_KEY_SORT_KBN, ddlSearchSortKbn.SelectedValue }
		};
		return searchParams;
	}

	/// <summary>
	/// パラメータ取得
	/// </summary>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParameters()
	{
		var parameters = new Hashtable();

		// 区分ID
		parameters.Add(
			Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID,
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID]));
		// 媒体区分名
		parameters.Add(
			Constants.REQUEST_KEY_ADVCODEMEDIATYPE_MEDIA_TYPE_NAME,
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ADVCODEMEDIATYPE_MEDIA_TYPE_NAME]));
		// 並び順
		switch (StringUtility.ToEmpty((string)Request[Constants.REQUEST_KEY_SORT_KBN]))
		{
			case Constants.KBN_SORT_ADVERTISEMENTCODEMEDIATYPE_DATE_CREATED_ASC: // 登録日/昇順
			case Constants.KBN_SORT_ADVERTISEMENTCODEMEDIATYPE_DATE_CREATED_DESC: // 登録日/降順
			case Constants.KBN_SORT_ADVERTISEMENTCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID_ASC: // 区分ID/昇順
			case Constants.KBN_SORT_ADVERTISEMENTCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID_DESC: // 区分ID/降順
				parameters.Add(Constants.REQUEST_KEY_SORT_KBN, Request[Constants.REQUEST_KEY_SORT_KBN]);
				break;

			default:
				parameters.Add(Constants.REQUEST_KEY_SORT_KBN, Constants.KBN_SORT_ADVERTISEMENTCODEMEDIATYPE_DEFAULT);
				break;
		}

		// カレントページ取得
		int currentPageNumber;
		if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out currentPageNumber) == false) currentPageNumber = 1;
		this.CurrentPageNumber = currentPageNumber;
		parameters.Add(Constants.REQUEST_KEY_PAGE_NO, currentPageNumber);

		return parameters;
	}

	/// <summary>
	/// 検索条件作成
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <returns>広告コード検索条件</returns>
	private AdvCodeMediaTypeListSearchCondition CreateSearchCondition(Hashtable parameters)
	{
		// 条件作成
		var condition = new AdvCodeMediaTypeListSearchCondition
		{
			AdvcodeMediaTypeId = (string)parameters[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID],
			AdvcodeMediaTypeName = (string)parameters[Constants.REQUEST_KEY_ADVCODEMEDIATYPE_MEDIA_TYPE_NAME],
			BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNumber - 1) + 1,
			EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNumber,
			SortKbn = (string)parameters[Constants.REQUEST_KEY_SORT_KBN],
		};

		return condition;
	}

	/// <summary>
	/// 画面に検索値をセット
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	private void SetParameters(Hashtable parameters)
	{
		//------------------------------------------------------
		// 画面制御
		//------------------------------------------------------
		// 値セット
		tbSearchAdvCodeMediaTypeId.Text = (string)parameters[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID];
		tbSearchAdvCodeMediaTypeName.Text = (string)parameters[Constants.REQUEST_KEY_ADVCODEMEDIATYPE_MEDIA_TYPE_NAME];
		foreach (ListItem item in ddlSearchSortKbn.Items)
		{
			item.Selected = (item.Value == (string)parameters[Constants.REQUEST_KEY_SORT_KBN]);
		}
	}

	/// <summary>
	/// 広告情報一覧遷移Url作成
	/// </summary>
	/// <param name="searchParams">検索パラメータ</param>
	/// <param name="pageNumber">ページNo</param>
	/// <returns>広告情報一覧遷移Url</returns>
	private string CreateAdvCodeListUrl(Hashtable searchParams, int pageNumber)
	{
		var resultUrl = CreateListUrl(searchParams) + "&" + Constants.REQUEST_KEY_PAGE_NO + "=" + pageNumber.ToString();
		return resultUrl;
	}

	/// <summary>
	/// Create List Url
	/// </summary>
	/// <param name="advCodeMediaTypeId">MediaTypeId</param>
	/// <param name="advCodeMediaTypeName">MediaTypeName</param>
	/// <param name="sortKbn">Sort type</param>
	/// <returns>List Url</returns>
	protected string CreateListUrl(Hashtable searchParams)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISEMENT_CODE_MEDIA_TYPE_SEARCH);
		url.AddParam(
			Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID,
			(string)searchParams[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID]);
		url.AddParam(
			Constants.REQUEST_KEY_ADVCODEMEDIATYPE_MEDIA_TYPE_NAME,
			(string)searchParams[Constants.REQUEST_KEY_ADVCODEMEDIATYPE_MEDIA_TYPE_NAME]);
		url.AddParam(
			Constants.REQUEST_KEY_SORT_KBN,
			(string)searchParams[Constants.REQUEST_KEY_SORT_KBN]);

		return url.CreateUrl();
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		// 広告情報一覧へリダイレクト
		var url = CreateAdvCodeListUrl(GetSearchInfo(), 1);
		Response.Redirect(url);
	}

	/// <summary>
	/// 選択された広告コードを設定パラメータ作成
	/// </summary>
	/// <param name="advCode">広告コード</param>
	/// <returns>広告コードを設定パラメータ</returns>
	protected string CreateJavaScriptSetAdvertisementCode(string advCode)
	{
		var jsParam = "'" + StringUtility.ToEmpty(advCode).Replace("'", "\\'") + "'";
		return jsParam;
	}

	/// <summary>カレントページ用ナンバー</summary>
	protected int CurrentPageNumber
	{
		get { return (int)ViewState["CurrentPageNumber"]; }
		set { ViewState["CurrentPageNumber"] = value; }
	}
}