/*
=========================================================================================================
  Module      : 広告コード一覧ページ処理(AdvertisementCodeList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Text;
using w2.App.Common.Option;
using w2.Common.Web;
using w2.Domain.AdvCode;
using w2.Domain.AdvCode.Helper;
using w2.Domain.ShopOperator;
using w2.Domain.UserManagementLevel;

public partial class Form_AdvertisementCode_AdvertisementCodeList : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// ユーザーコントロール割り当て
		uMasterDownload.OnCreateSearchInputParams += this.CreateSearchParams;

		if (!IsPostBack)
		{
			ViewAdvCodeList();
		}
	}

	/// <summary>
	/// View Advertisement Code
	/// </summary>
	private void ViewAdvCodeList()
	{
		// 画面制御
		InitializeComponents();

		// リクエスト情報取得
		var parameters = GetParameters(Request);
		Session[Constants.SESSIONPARAM_KEY_ADVCODE_SEARCH_INFO] = parameters;

		// 検索値を画面にセット
		SetParameters(parameters);

		// Get AdvCode data
		var loginOperator = new ShopOperatorService().Get(this.LoginOperatorShopId, this.LoginOperatorId);
		var updateAdvCodeArray = StringUtility.SplitCsvLine(loginOperator.UsableAdvcodeNosInReport).ToArray();

		var searchCondition = CreateSearchCondition(parameters);

		var advService = new AdvCodeService();
		var advCodeData = (loginOperator.UsableAdvcodeNosInReport.Length == 0)
			? advService.SearchAdvCode(searchCondition)
			: advService.SearchAdvCode(searchCondition, updateAdvCodeArray);

		var totalAdvCodeCounts = (loginOperator.UsableAdvcodeNosInReport.Length == 0)
		? advService.GetAdvCodeSearchHitCount(searchCondition)
		: advService.GetAdvCodeSearchHitCount(searchCondition, updateAdvCodeArray);

		if (totalAdvCodeCounts != 0)
		{
			trListError.Visible = false;
		}
		else
		{
			// 一覧非表示・エラー表示制御
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		// データソースセット
		rAdvCodeList.DataSource = advCodeData;

		// ページャ作成（一覧処理で総件数を取得）
		string nextUrl = CreateAdvCodeListUrl(parameters);
		lbPager.Text = WebPager.CreateDefaultListPager(totalAdvCodeCounts, this.CurrentPageNumber, nextUrl);

		// データバインド
		DataBind();
	}

	/// <summary>
	/// Initialize Components
	/// </summary>
	private void InitializeComponents()
	{
		// Sort
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ADVCODE, Constants.REQUEST_KEY_SORT_KBN))
		{
			ddlSearchSortKbn.Items.Add(li);
		}

		// AdvCode media type
		var advCodeMediaTypeList = new AdvCodeService().GetAdvCodeMediaTypeListAll();
		ddlAdvCodeMediaType.Items.Add(new ListItem(string.Empty, string.Empty));
		foreach (var advCodeMediaType in advCodeMediaTypeList)
		{
			ddlAdvCodeMediaType.Items.Add(new ListItem(advCodeMediaType.AdvcodeMediaTypeName, advCodeMediaType.AdvcodeMediaTypeId));
		}

		// 会員ランクドロップダウン作成
		var memberRank = MemberRankOptionUtility.GetMemberRankList();
		ddlMemberRank.Items.Add(new ListItem("", ""));
		ddlMemberRank.Items.AddRange(
			memberRank.Select(m => new ListItem(m.MemberRankName, m.MemberRankId)).ToArray());

		// ユーザー管理レベルドロップダウン作成
		var userManagemetLevel = new UserManagementLevelService().GetAllList();
		ddlUserManagementLevel.Items.Add(new ListItem("", ""));
		ddlUserManagementLevel.Items.AddRange(
			userManagemetLevel.Select(m => new ListItem(m.UserManagementLevelName, m.UserManagementLevelId)).ToArray());

	}

	/// <summary>
	/// Get Search Info
	/// </summary>
	/// <returns>Search info</returns>
	private Hashtable GetSearchInfo()
	{
		// Search params
		var searchParams = new Hashtable
		{
			{ Constants.FIELD_ADVCODE_DEPT_ID, this.LoginOperatorDeptId },
			{ Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE, tbSearchAdvertisementCode.Text.Trim() },
			{ Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME, tbSearchMediaName.Text.Trim() },
			{ Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID, ddlAdvCodeMediaType.SelectedValue },
			{ Constants.REQUEST_KEY_SORT_KBN, ddlSearchSortKbn.SelectedValue },
			{ Constants.REQUEST_KEY_ADVCODE_MEMBERRANK_ID, ddlMemberRank.SelectedValue },
			{ Constants.REQUEST_KEY_ADVCODE_USER_MANAGEMENT_LEVEL_ID, ddlUserManagementLevel.SelectedValue },
		};

		return searchParams;
	}

	/// <summary>
	/// Get Parameters
	/// </summary>
	/// <param name="requestParams">Request Params</param>
	/// <returns>Request params</returns>
	protected Hashtable GetParameters(HttpRequest requestParams)
	{
		var resultData = new Hashtable();
		int currentPageNumber;

		// Get parameters
		resultData.Add(Constants.FIELD_ADVCODE_DEPT_ID, this.LoginOperatorDeptId);
		resultData.Add(Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE, StringUtility.ToEmpty(requestParams[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE]));
		resultData.Add(Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME, StringUtility.ToEmpty(requestParams[Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME]));
		resultData.Add(Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID, StringUtility.ToEmpty(requestParams[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID]));
		resultData.Add(Constants.REQUEST_KEY_ADVCODE_MEMBERRANK_ID, StringUtility.ToEmpty(requestParams[Constants.REQUEST_KEY_ADVCODE_MEMBERRANK_ID]));
		resultData.Add(Constants.REQUEST_KEY_ADVCODE_USER_MANAGEMENT_LEVEL_ID, StringUtility.ToEmpty(requestParams[Constants.REQUEST_KEY_ADVCODE_USER_MANAGEMENT_LEVEL_ID]));

		// ソート
		var sortKbn = Constants.KBN_SORT_ADVCODE_DEFAULT;
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
				sortKbn = StringUtility.ToEmpty(requestParams[Constants.REQUEST_KEY_SORT_KBN]);
				break;
		}
		resultData.Add(Constants.REQUEST_KEY_SORT_KBN, sortKbn);

		// Set current page number
		if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out currentPageNumber) == false)
		{
			currentPageNumber = 1;
		}
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
		// 条件作成
		var condition = new AdvCodeListSearchCondition
		{
			DeptId = (string)parameters[Constants.FIELD_ADVCODE_DEPT_ID],
			AdvertisementCode = (string)parameters[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE],
			MediaName = (string)parameters[Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME],
			AdvcodeMediaTypeId = (string)parameters[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID],
			BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNumber - 1) + 1,
			EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNumber,
			SortKbn = (string)parameters[Constants.REQUEST_KEY_SORT_KBN],
			ValidFlg = string.Empty,
			MemberRankId = (string)parameters[Constants.REQUEST_KEY_ADVCODE_MEMBERRANK_ID],
			UserManagementLevelId = (string)parameters[Constants.REQUEST_KEY_ADVCODE_USER_MANAGEMENT_LEVEL_ID],
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
		// パラメタセット
		//------------------------------------------------------
		tbSearchAdvertisementCode.Text = (string)parameters[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE];		// Advertisement code
		tbSearchMediaName.Text = (string)parameters[Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME];						// Media name
		if (StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_SORT_KBN]) != string.Empty)
		{
			foreach (ListItem item in ddlSearchSortKbn.Items)
			{
				item.Selected = (item.Value == (string)parameters[Constants.REQUEST_KEY_SORT_KBN]);
			}
		}
		// Advertisement code media type
		foreach (ListItem item in ddlAdvCodeMediaType.Items)
		{
			item.Selected = (item.Value == (string)parameters[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID]);
		}
		foreach (ListItem item in ddlMemberRank.Items)
		{
			item.Selected = (item.Value == (string)parameters[Constants.REQUEST_KEY_ADVCODE_MEMBERRANK_ID]);
		}
		foreach (ListItem item in ddlUserManagementLevel.Items)
		{
			item.Selected = (item.Value == (string)parameters[Constants.REQUEST_KEY_ADVCODE_USER_MANAGEMENT_LEVEL_ID]);
		}
	}

	/// <summary>
	/// Create AdvCode List Url
	/// </summary>
	/// <param name="searchParams">Search Params</param>
	/// <param name="pageNumber">page Number</param>
	/// <returns>AdvCode list url</returns>
	private string CreateAdvCodeListUrl(Hashtable searchParams, int pageNumber)
	{
		string resultUrl = CreateAdvCodeListUrl(searchParams);
		resultUrl += "&";
		resultUrl += Constants.REQUEST_KEY_PAGE_NO + "=" + pageNumber.ToString();

		return resultUrl;
	}

	/// <summary>
	/// Create AdvCode List Url
	/// </summary>
	/// <param name="searchParams">Search Params</param>
	/// <returns>AdvCode list url</returns>
	private string CreateAdvCodeListUrl(Hashtable searchParams)
	{
		var resultUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_LIST)
			.AddParam(Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE, (string)searchParams[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE])
			.AddParam(Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME, (string)searchParams[Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME])
			.AddParam(Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID, (string)searchParams[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID])
			.AddParam(Constants.REQUEST_KEY_SORT_KBN, (string)searchParams[Constants.REQUEST_KEY_SORT_KBN])
			.AddParam(Constants.REQUEST_KEY_ADVCODE_MEMBERRANK_ID, (string)searchParams[Constants.REQUEST_KEY_ADVCODE_MEMBERRANK_ID])
			.AddParam(Constants.REQUEST_KEY_ADVCODE_USER_MANAGEMENT_LEVEL_ID, (string)searchParams[Constants.REQUEST_KEY_ADVCODE_USER_MANAGEMENT_LEVEL_ID])
			.CreateUrl();

		return resultUrl;
	}

	/// <summary>
	/// Create AdvCode Detail Url
	/// </summary>
	/// <param name="advCode">AdvCode</param>
	/// <returns>AdvCode Detail Url</returns>
	protected string CreateAdvCodeDetailUrl(string advCode)
	{
		StringBuilder resultUrl = new StringBuilder();

		resultUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_REGISTER);
		resultUrl.Append("?").Append(Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE).Append("=").Append(HttpUtility.UrlEncode(advCode));
		resultUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_UPDATE);
		resultUrl.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(this.CurrentPageNumber);

		return resultUrl.ToString();
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		// To AdvCode list
		Response.Redirect(CreateAdvCodeListUrl(GetSearchInfo(), 1));
	}

	/// <summary>
	/// 新規ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = null;
		// 新規登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_REGISTER + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}

	/// <summary>
	/// マスタデータ出力用の検索ハッシュテーブル生成
	/// </summary>
	/// <returns>検索ハッシュテーブル</returns>
	/// <remarks>マスタ出力ユーザコントロールのイベントに割り当てて使う</remarks>
	public Hashtable CreateSearchParams()
	{
		return CreateSearchCondition(GetParameters(Request)).CreateHashtableParams();
	}

	/// <summary>カレントページ用ナンバー</summary>
	protected int CurrentPageNumber
	{
		get { return (int)ViewState["CurrentPageNumber"]; }
		set { ViewState["CurrentPageNumber"] = value; }
	}
}
