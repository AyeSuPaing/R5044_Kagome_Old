/*
=========================================================================================================
  Module      : 広告コード検索ページ処理(AdvertisementCodeSearch.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls;
using System.Text;
using w2.Domain.AdvCode;
using w2.Domain.AdvCode.Helper;
using System.Linq;

public partial class Form_Common_AdvertisementCodeSearch : BasePage
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
			var searchCondition = CreateSearchCondition(parameters);
			var advService = new AdvCodeService();
			var advCodeData = advService.SearchAdvCode(searchCondition);
			var totalAdvCodeCounts = advService.GetAdvCodeSearchHitCount(searchCondition);
			
			// 一覧非表示・エラー表示制御
			if (totalAdvCodeCounts != 0)
			{
				trListError.Visible = false;
			}
			else
			{
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}

			// データソースセット
			rAdvCodeList.DataSource = advCodeData;

			// ページャ作成（一覧処理で総件数を取得）
			var nextUrl = CreateAdvCodeListUrl(parameters);
			lbPager.Text = WebPager.CreateDefaultListPager(totalAdvCodeCounts, this.CurrentPageNumber, nextUrl);

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
		ddlSearchSortKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ADVCODE, Constants.REQUEST_KEY_SORT_KBN));

		// 広告媒体区分ドロップ設定
		ddlAdvCodeMediaType.Items.Add(new ListItem("", ""));
		ddlAdvCodeMediaType.Items.AddRange(new AdvCodeService().GetAdvCodeMediaTypeListAll().Select(adv =>
			new ListItem(adv.AdvcodeMediaTypeName, adv.AdvcodeMediaTypeId))
			.ToArray());
	}

	/// <summary>
	/// 画面から検索情報取得
	/// </summary>
	/// <returns>検索情報が格納されたHashtable</returns>
	private Hashtable GetSearchInfo()
	{
		var searchParams = new Hashtable
		{
			{Constants.FIELD_ADVCODE_DEPT_ID, this.LoginOperatorDeptId},
			{Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE, tbSearchAdvertisementCode.Text.Trim()},
			{Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME, tbSearchMediaName.Text.Trim()},
			{Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID, ddlAdvCodeMediaType.SelectedValue},
			{Constants.REQUEST_KEY_SORT_KBN, ddlSearchSortKbn.SelectedValue}
		};
		return searchParams;
	}

	/// <summary>
	/// パラメータ取得
	/// </summary>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParameters()
	{
		// パラメータ取得
		var resultData = new Hashtable
		{
			{Constants.FIELD_ADVCODE_DEPT_ID, this.LoginOperatorDeptId},
			{Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE])},
			{Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME])},
			{Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID])}
		};

		// 並び順を取得
		string sortKbn = null;
		switch (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]))
		{
			case Constants.KBN_SORT_ADVCODE_DATE_CREATED_ASC:
			case Constants.KBN_SORT_ADVCODE_DATE_CREATED_DESC:
			case Constants.KBN_SORT_ADVCODE_ADVERTISEMENT_CODE_ASC:
			case Constants.KBN_SORT_ADVCODE_ADVERTISEMENT_CODE_DESC:
			case Constants.KBN_SORT_ADVCODE_ADVCODE_MEDIA_TYPE_ID_ASC:
			case Constants.KBN_SORT_ADVCODE_ADVCODE_MEDIA_TYPE_ID_DESC:
			case Constants.KBN_SORT_ADVCODE_MEDIA_TYPE_DISPLAY_ORDER_ASC:
			case Constants.KBN_SORT_ADVCODE_MEDIA_TYPE_DISPLAY_ORDER_DESC:
			case Constants.KBN_SORT_ADVCODE_DATE_CHANGED_ASC:
			case Constants.KBN_SORT_ADVCODE_DATE_CHANGED_DESC:
				sortKbn = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]);
				break;

			default:
				sortKbn = Constants.KBN_SORT_ADVCODE_DEFAULT;
				break;
		}
		resultData.Add(Constants.REQUEST_KEY_SORT_KBN, sortKbn);

		// カレントページ取得
		int currentPageNumber;
		if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out currentPageNumber) == false) currentPageNumber = 1;
		this.CurrentPageNumber = currentPageNumber;
		resultData.Add(Constants.REQUEST_KEY_PAGE_NO, currentPageNumber);

		return resultData;
	}

	/// <summary>
	/// 検索条件作成
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <returns>広告コード検索条件</returns>
	private AdvCodeListSearchCondition CreateSearchCondition(Hashtable parameters)
	{
		var condition = new AdvCodeListSearchCondition
		{
			DeptId = (string)parameters[Constants.FIELD_ADVCODE_DEPT_ID],
			AdvertisementCode = (string)parameters[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE],
			MediaName = (string)parameters[Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME],
			AdvcodeMediaTypeId = (string)parameters[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID],
			BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNumber - 1) + 1,
			EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNumber,
			SortKbn = (string)parameters[Constants.REQUEST_KEY_SORT_KBN],
			ValidFlg = string.Empty
		};
		return condition;
	}

	/// <summary>
	/// 画面に検索値をセット
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	private void SetParameters(Hashtable parameters)
	{
		tbSearchAdvertisementCode.Text = (string)parameters[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE];
		tbSearchMediaName.Text = (string)parameters[Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME];
		ddlAdvCodeMediaType.Items.Cast<ListItem>().ToList().ForEach(item =>
			item.Selected = (item.Value == (string)parameters[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID]));
		if (StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SORT_KBN]) != string.Empty)
		{
			ddlSearchSortKbn.Items.Cast<ListItem>().ToList().ForEach(item =>
				item.Selected = (item.Value == (string)parameters[Constants.REQUEST_KEY_SORT_KBN]));
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
		var resultUrl = CreateAdvCodeListUrl(searchParams)
			+ "&" + Constants.REQUEST_KEY_PAGE_NO + "=" + pageNumber.ToString();
		return resultUrl;
	}

	/// <summary>
	/// 広告情報一覧遷移Url作成
	/// </summary>
	/// <param name="searchParams">検索パラメータ</param>
	/// <returns>広告情報一覧遷移Url</returns>
	private string CreateAdvCodeListUrl(Hashtable searchParams)
	{
		var resultUrl = Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISEMENT_CODE_SEARCH
			+ "?" + Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE + "=" + HttpUtility.UrlEncode((string)searchParams[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE])
			+ "&" + Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME + "=" + HttpUtility.UrlEncode((string)searchParams[Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME])
			+ "&" + Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID + "=" + HttpUtility.UrlEncode((string)searchParams[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID])
			+ "&" + Constants.REQUEST_KEY_SORT_KBN + "=" + HttpUtility.UrlEncode((string)searchParams[Constants.REQUEST_KEY_SORT_KBN]);
		return resultUrl;
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