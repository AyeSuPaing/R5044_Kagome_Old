/*
=========================================================================================================
  Module      : レコメンドレポート レポート対象選択ページ処理(RecommendReportTableList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.Manager;
using w2.Common.Web;
using w2.Domain.MenuAuthority.Helper;
using w2.Domain.Recommend;
using w2.Domain.Recommend.Helper;

public partial class Form_RecommendReport_RecommendReportTableList : BasePage
{
	#region 定数
	/// <summary>並び順パラメータ</summary>
	private const string FIELD_RECOMMEND_SORTKBN = "report_sort";
	/// <summary>統計項目パラメータ</summary>
	private const string FIELD_KEY_DATA_KBN = "dk";
	///<summary>最大指定期間日数</summary>
	protected const string CONST_GET_RECOMMENDREPORT_CONDITION_DAYS = "100";
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// 表示コンポーネント初期化
			InitializeComponents();

			// 期間指定(デフォルト当月)
			var dt = DateTime.Parse(string.Format("{0}/{1}/01", DateTime.Now.Year, DateTime.Now.Month));
			var termDateFrom = DateTimeUtility.ToStringForManager(
				dt,
				DateTimeUtility.FormatType.ShortDate2LetterNoneServerTime);
			var termDateTo = DateTimeUtility.ToStringForManager(
				dt.AddMonths(1).AddDays(-1),
				DateTimeUtility.FormatType.ShortDate2LetterNoneServerTime);

			if ((string.IsNullOrEmpty(this.RequestCurrentYear) == false)
				&& (string.IsNullOrEmpty(this.RequestCurrentMonth) == false)
				&& (string.IsNullOrEmpty(this.RequestCurrentDay) == false)
				&& (string.IsNullOrEmpty(this.RequestTargetYear) == false)
				&& (string.IsNullOrEmpty(this.RequestTargetMonth) == false)
				&& (string.IsNullOrEmpty(this.RequestTargetDay) == false))
			{
				termDateFrom = string.Format(
					"{0}/{1}/{2}",
					this.RequestCurrentYear,
					this.RequestCurrentMonth.PadLeft(2, '0'),
					this.RequestCurrentDay.PadLeft(2, '0'));

				termDateTo = string.Format(
					"{0}/{1}/{2}",
					this.RequestTargetYear, 
					this.RequestTargetMonth.PadLeft(2, '0'),
					this.RequestTargetDay.PadLeft(2, '0'));
			}

			try
			{
				ucTermPeriod.SetPeriodDate(
					DateTime.Parse(termDateFrom),
					DateTime.Parse(string.Format("{0} 23:59:59", termDateTo)));
			}
			catch
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			this.DateBgn = DateTime.Parse(ucTermPeriod.StartDateTimeString);
			this.DateEnd = DateTime.Parse(ucTermPeriod.EndDateTimeString);

			lRecommendReportDateErrorMessages.Text = string.Empty;
			var errorMessage = GetTermErrorMessage();
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				lRecommendReportDateErrorMessages.Text = HtmlSanitizer.HtmlEncode(errorMessage);
				return;
			}

			// レコメンド一覧表示
			DisplayRecommendReportTableList();
		}
	}

	/// <summary>
	/// CSVダウンロードリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReportExport_Click(object sender, System.EventArgs e)
	{
		lRecommendReportDateErrorMessages.Text = string.Empty;
		var errorMessage = GetTermErrorMessage();
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			lRecommendReportDateErrorMessages.Text = HtmlSanitizer.HtmlEncode(errorMessage);
			return;
		}

		var titleParams = new List<string>
		{
			// 「レコメンドレポート」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_TITLE),
			string.Format(
				"{0}　（{1}：{2}～{3}）",
				ddlSortKbn.SelectedItem,
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_PERIOD),
				DateTimeUtility.ToStringForManager(this.DateBgn, DateTimeUtility.FormatType.LongDate1Letter),
				DateTimeUtility.ToStringForManager(this.DateEnd, DateTimeUtility.FormatType.LongDate1Letter))
		};

		var records = new StringBuilder();
		records.Append(CreateRecordCsv(titleParams));
		// 一行空き
		titleParams.Clear();
		records.Append(CreateRecordCsv(titleParams));

		// ヘッダ作成
		var headerParams = new List<string>
		{
			// 「レコメンドID」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_RECOMMEND_ID)
		};

		var service = new RecommendService();
		var searchCondition = CreateSearchCondition();
		searchCondition.BeginRowNumber = 1;
		searchCondition.EndRowNumber = service.GetRecommendReportHitCount(searchCondition);
		this.SearchResult = service.GetRecommendReport(searchCondition);
		foreach (var ri in this.SearchResult)
		{
			headerParams.Add(ri.RecommendId);
		}

		// 「合計」
		headerParams.Add(
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_TOTAL));
		records.Append(CreateRecordCsv(headerParams));

		// データ作成
		var dataNameParams = new List<string>();
		var dataPvNumParams = new List<string>();
		var dataCvNumParams = new List<string>();
		var dataCvpAverageParams = new List<string>();
		var dataDisplayPageParams = new List<string>();
		var dataKbnParams = new List<string>();
		var dataStatusParams = new List<string>();
		var dataValidFlgParams = new List<string>();

		if (rList.Items.Count > 0)
		{
			// 「レコメンド名(管理用)」
			dataNameParams.Add(
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_RECOMMEND_NAME));

			// 「PV数」
			dataPvNumParams.Add(
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_NUMBER_PAGE_VIEW));

			// 「CV数」
			dataCvNumParams.Add(
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_NUMBER_CONVERSION));

			// 「CV率(%)」
			dataCvpAverageParams.Add(
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_CONVERSION_RATE));

			// 「表示ページ」
			dataDisplayPageParams.Add(
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_RECOMMEND_DISPLAYNAME));

			// 「レコメンド区分」
			dataKbnParams.Add(
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_RECOMMEND_KBN));

			// 「開催状態」
			dataStatusParams.Add(
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_RECOMMEND_STATUS));

			// 「有効フラグ」
			dataValidFlgParams.Add(
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_RECOMMEND_VALIDFLG));

			foreach (var ri in this.SearchResult)
			{
				dataNameParams.Add(ri.RecommendName.ToString());
				dataPvNumParams.Add(ri.PvNumber.ToString());
				dataCvNumParams.Add(ri.CvNumber.ToString());
				dataCvpAverageParams.Add(ri.CvPercent.ToString());
				dataDisplayPageParams.Add(ri.RecommendDisplayPageText.ToString());
				dataKbnParams.Add(ri.RecommendKbnText.ToString());
				dataStatusParams.Add(
					ValueText.GetValueText(
						Constants.TABLE_RECOMMEND,
						Constants.REQUEST_KEY_RECOMMEND_STATUS,
						ri.Status));
				dataValidFlgParams.Add(ri.ValidFlgText.ToString());
			}
			dataNameParams.Add("－");
			dataPvNumParams.Add(lDataPvNumTotal.Text.Trim());
			dataCvNumParams.Add(lDataCvNumTotal.Text.Trim());
			dataCvpAverageParams.Add(lDataCvpAverager.Text.Trim());
			dataDisplayPageParams.Add("－");
			dataKbnParams.Add("－");
			dataStatusParams.Add("－");
			dataValidFlgParams.Add("－");
		}
		records.Append(CreateRecordCsv(dataNameParams));
		records.Append(CreateRecordCsv(dataPvNumParams));
		records.Append(CreateRecordCsv(dataCvNumParams));
		records.Append(CreateRecordCsv(dataCvpAverageParams));
		records.Append(CreateRecordCsv(dataDisplayPageParams));
		records.Append(CreateRecordCsv(dataKbnParams));
		records.Append(CreateRecordCsv(dataStatusParams));
		records.Append(CreateRecordCsv(dataValidFlgParams));

		var fileName = string.Format(
			"RecommendReportTableList_{0:yyyyMMdd}-{1:yyyyMMdd}",
			this.DateBgn,
			this.DateEnd);

		// ファイル出力
		OutPutFileCsv(fileName, records.ToString());
	}

	/// <summary>
	/// 表示クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ShowRecommendReport_Click(object sender, EventArgs e)
	{
		lRecommendReportDateErrorMessages.Text = string.Empty;
		var errorMessage = GetTermErrorMessage();
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			lRecommendReportDateErrorMessages.Text = HtmlSanitizer.HtmlEncode(errorMessage);
			return;
		}

		var searchValues = new SearchValues(
			tbRecommendId.Text.Trim(),
			tbRecommendName.Text.Trim(),
			ddlRecommendKbn.SelectedValue,
			ddlStatus.SelectedValue,
			ddlValidFlg.SelectedValue,
			ddlSortKbn.SelectedValue,
			1);
		Response.Redirect(searchValues.CreateRecommendListUrl(this.DateBgn, this.DateEnd));
	}

	/// <summary>
	/// 期間指定のエラーメッセージを取得
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private string GetTermErrorMessage()
	{
		var result = string.Empty;

		this.DateBgn = DateTime.Parse(ucTermPeriod.StartDateTimeString);
		this.DateEnd = DateTime.Parse(ucTermPeriod.EndDateTimeString);

		var days = new TimeSpan(this.DateEnd.Ticks - this.DateBgn.Ticks).Days;

		if (days > int.Parse(CONST_GET_RECOMMENDREPORT_CONDITION_DAYS))
		{
			result = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RECOMMEND_REPORT_OUT_OF_RANGE_ERROR)
				.Replace("@@ 1 @@", CONST_GET_RECOMMENDREPORT_CONDITION_DAYS);
		}
		else if (days < 0)
		{
			result = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RECOMMEND_REPORT_INVALID_DATE_ERROR);
		}

		return result;
	}

	/// <summary>
	/// レコメンド一覧表示
	/// </summary>
	private void DisplayRecommendReportTableList()
	{
		// 検索フォームにパラメータをセット
		tbRecommendId.Text = HtmlSanitizer.HtmlEncode(this.RequestRecommendId);
		tbRecommendName.Text = HtmlSanitizer.HtmlEncode(this.RequestRecommendName);
		ddlRecommendKbn.SelectedValue = this.RequestRecommendKbn;
		ddlStatus.SelectedValue = this.RequestStatus;
		ddlSortKbn.SelectedValue = this.RequestSortKbn;
		ddlValidFlg.SelectedValue = this.RequestValidFlg;

		var service = new RecommendService();
		var searchCondition = CreateSearchCondition();
		var totalCount = service.GetRecommendReportHitCount(searchCondition);
		this.SearchResult = service.GetRecommendReport(searchCondition);

		rList.DataSource = this.SearchResult;
		rList.DataBind();

		// 検索結果件数が0件の場合、エラー表示制御、「合計」表示制御
		if (totalCount > 0)
		{
			trListError.Visible = false;
			lbReportExport.Visible = true;
			lDataPvNumTotal.Text = HtmlSanitizer.HtmlEncode(
				service.GetRecommendReportPvNumberTotal(searchCondition));
			lDataCvNumTotal.Text = HtmlSanitizer.HtmlEncode(
				service.GetRecommendReportCvNumberTotal(searchCondition));
			var cvpAverager = int.Parse(lDataPvNumTotal.Text.Trim()) == 0
				? 0
				: int.Parse(lDataCvNumTotal.Text.Trim()) * 100 / int.Parse(lDataPvNumTotal.Text.Trim());
			lDataCvpAverager.Text = HtmlSanitizer.HtmlEncode(
				string.Format(
					"{0}{1}",
					cvpAverager,
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
						Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST,
						Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_AVERAGE)));
		}
		else
		{
			trListError.Visible = true;
			lbReportExport.Visible = false;
			lDataTotal.Visible = false;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		// レコメンド一覧検索情報を格納
		this.SearchInfo = new SearchValues(
			this.RequestRecommendId,
			this.RequestRecommendName,
			this.RequestRecommendKbn,
			this.RequestStatus,
			this.RequestValidFlg,
			this.RequestSortKbn,
			this.RequestPageNum);

		// ページャ作成
		var nextUrl = this.SearchInfo.CreateRecommendListUrl(this.DateBgn, this.DateEnd, false);
		lbPager.Text = WebPager.CreateDefaultListPager(totalCount, this.RequestPageNum, nextUrl);

		// レコメンドタイトル表示
		lRecommendReportTitle.Text = HtmlSanitizer.HtmlEncode(
			string.Format(
				"{0}　（{1}：{2}～{3}）",
				ddlSortKbn.SelectedItem,
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_PERIOD),
				DateTimeUtility.ToStringForManager(this.DateBgn, DateTimeUtility.FormatType.LongDate1Letter),
				DateTimeUtility.ToStringForManager(this.DateEnd, DateTimeUtility.FormatType.LongDate1Letter)));
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// レコメンド区分
		ddlRecommendKbn.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlRecommendKbn.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_RECOMMEND, Constants.FIELD_RECOMMEND_RECOMMEND_KBN));
		// 開催状態
		ddlStatus.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_RECOMMEND, Constants.REQUEST_KEY_RECOMMEND_STATUS));
		// 有効フラグ
		ddlValidFlg.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlValidFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_RECOMMEND, Constants.FIELD_RECOMMEND_VALID_FLG));
		// 並び順
		ddlSortKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_RECOMMEND, FIELD_RECOMMEND_SORTKBN));
	}

	/// <summary>
	/// 検索条件作成
	/// </summary>
	/// <returns>検索条件</returns>
	private RecommendListSearchCondition CreateSearchCondition()
	{
		return new RecommendListSearchCondition
		{
			ShopId = this.LoginOperatorShopId,
			RecommendId = tbRecommendId.Text.Trim(),
			RecommendName = tbRecommendName.Text.Trim(),
			RecommendKbn = ddlRecommendKbn.SelectedValue,
			Status = ddlStatus.SelectedValue,
			ReportSortKbn = ddlSortKbn.SelectedValue,
			BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.RequestPageNum - 1) + 1,
			EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.RequestPageNum,
			ValidFlg = ddlValidFlg.SelectedValue,
			BeginDate = this.DateBgn,
			EndDate = this.DateEnd,
		};
	}

	/// <summary>
	/// レコメンドレポート、レポート対象選択URL
	/// </summary>
	/// <param name="recommendId">レコメンドID</param>
	/// <param name="dataKbn">統計項目（PV数、CV数、CP率）</param>
	/// <returns>URL</returns>
	protected string CreateRecommendReportListUrl(string recommendId = null, string dataKbn = null)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_RECOMMEND_REPORT);

		if ((string.IsNullOrEmpty(recommendId) == false) && (string.IsNullOrEmpty(dataKbn) == false))
		{
			urlCreator.AddParam(Constants.REQUEST_KEY_CURRENT_YEAR, this.DateBgn.Year.ToString())
			.AddParam(Constants.REQUEST_KEY_CURRENT_MONTH, this.DateBgn.Month.ToString())
			.AddParam(Constants.REQUEST_KEY_CURRENT_DAY, this.DateBgn.Day.ToString())
			.AddParam(Constants.REQUEST_KEY_TARGET_YEAR, this.DateEnd.Year.ToString())
			.AddParam(Constants.REQUEST_KEY_TARGET_MONTH, this.DateEnd.Month.ToString())
			.AddParam(Constants.REQUEST_KEY_TARGET_DAY, this.DateEnd.Day.ToString())
			.AddParam(Constants.REQUEST_KEY_RECOMMEND_ID, recommendId)
			.AddParam(FIELD_KEY_DATA_KBN, dataKbn);
		}

		var url = urlCreator.CreateUrl();
		return url;
	}

	/// <summary>
	/// レコメンド設定URL
	/// </summary>
	/// <param name="recommendId">レコメンドID</param>
	/// <returns>URL</returns>
	protected string CreateRecommendSettingUrl(string recommendId)
	{
		var url = SingleSignOnUrlCreator.CreateForWebForms(
			MenuAuthorityHelper.ManagerSiteType.Ec,
				new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_RECOMMEND_REGISTER)
				.AddParam(Constants.REQUEST_KEY_RECOMMEND_ID, recommendId)
				.AddParam("action_status", Constants.ACTION_STATUS_UPDATE)
				.CreateUrl());

		return url;
	}

	#region プロパティ
	/// <summary>リクエスト：レコメンドID</summary>
	private string RequestRecommendId
	{
		get { return StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_RECOMMEND_ID]).Trim(); }
	}
	/// <summary>リクエスト：レコメンド名（管理用）</summary>
	private string RequestRecommendName
	{
		get { return StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_RECOMMEND_NAME]).Trim(); }
	}
	/// <summary>リクエスト：レコメンド区分</summary>
	private string RequestRecommendKbn
	{
		get { return StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_RECOMMEND_KBN]).Trim(); }
	}
	/// <summary>リクエスト：開催状態</summary>
	private string RequestStatus
	{
		get { return StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_RECOMMEND_STATUS]).Trim(); }
	}
	/// <summary>リクエスト：有効フラグ</summary>
	private string RequestValidFlg
	{
		get { return StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_RECOMMEND_VALID_FLG]).Trim(); }
	}
	/// <summary>リクエスト：開始時間：年</summary>
	private string RequestCurrentYear
	{
		get { return StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_CURRENT_YEAR]).Trim(); }
	}
	/// <summary>リクエスト：開始時間：月</summary>
	private string RequestCurrentMonth
	{
		get { return StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_CURRENT_MONTH]).Trim(); }
	}
	/// <summary>リクエスト：開始時間：日</summary>
	private string RequestCurrentDay
	{
		get { return StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_CURRENT_DAY]).Trim(); }
	}
	/// <summary>リクエスト：終了時間：年</summary>
	private string RequestTargetYear
	{
		get { return StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_TARGET_YEAR]).Trim(); }
	}
	/// <summary>リクエスト：終了時間：月</summary>
	private string RequestTargetMonth
	{
		get { return StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_TARGET_MONTH]).Trim(); }
	}
	/// <summary>リクエスト：終了時間：日</summary>
	private string RequestTargetDay
	{
		get { return StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_TARGET_DAY]).Trim(); }
	}
	/// <summary>リクエスト：並び順</summary>
	private string RequestSortKbn
	{
		get
		{
			var sortKbn = Constants.KBN_REPORT_SORT_RECOMMEND_LIST_STATUS;
			switch (this.Request[FIELD_RECOMMEND_SORTKBN])
			{
				case Constants.KBN_REPORT_SORT_RECOMMEND_LIST_STATUS:
				case Constants.KBN_REPORT_SORT_RECOMMEND_LIST_RECOMMEND_NAME_ASC:
				case Constants.KBN_REPORT_SORT_RECOMMEND_LIST_RECOMMEND_NAME_DESC:
				case Constants.KBN_REPORT_SORT_RECOMMEND_LIST_PAGE_VIEW_ASC:
				case Constants.KBN_REPORT_SORT_RECOMMEND_LIST_PAGE_VIEW_DESC:
				case Constants.KBN_REPORT_SORT_RECOMMEND_LIST_CONVERSION_ASC:
				case Constants.KBN_REPORT_SORT_RECOMMEND_LIST_CONVERSION_DESC:
				case Constants.KBN_REPORT_SORT_RECOMMEND_LIST_CONVERSION_RATE_ASC:
				case Constants.KBN_REPORT_SORT_RECOMMEND_LIST_CONVERSION_RATE_DESC:
					sortKbn = this.Request[FIELD_RECOMMEND_SORTKBN];
					break;
			}
			return sortKbn;
		}
	}
	/// <summary>リクエスト：ページ番号</summary>
	private int RequestPageNum
	{
		get
		{
			int pageNum;
			return int.TryParse(this.Request[Constants.REQUEST_KEY_PAGE_NO], out pageNum) ? pageNum : 1;
		}
	}
	/// <summary>レコメンド設定一覧検索情報</summary>
	protected SearchValues SearchInfo
	{
		get
		{
			return Session[Constants.SESSIONPARAM_KEY_RECOMMEND_SEARCH_INFO] != null
				? (SearchValues)Session[Constants.SESSIONPARAM_KEY_RECOMMEND_SEARCH_INFO]
				: new SearchValues(
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					1);
		}
		private set { Session[Constants.SESSIONPARAM_KEY_RECOMMEND_SEARCH_INFO] = value; }
	}
	/// <summary>レコメンド設定一覧検索結果</summary>
	private RecommendListSearchResult[] SearchResult
	{
		get { return (RecommendListSearchResult[])ViewState["SearchResult"]; }
		set { ViewState["SearchResult"] = value; }
	}
	/// <summary>期間指定 開始時間</summary>
	private DateTime DateBgn { get; set; }
	/// <summary>期間指定 終了時間</summary>
	private DateTime DateEnd { get; set; }
	#endregion

	#region 検索結果格納クラス
	/// <summary>検索値格納クラス</summary>
	[Serializable]
	protected class SearchValues
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="recommendName">レコメンド名（管理用）</param>
		/// <param name="recommendKbn">レコメンド区分</param>
		/// <param name="status">開催状態</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <param name="sortKbn">並び順</param>
		/// <param name="pageNum">ページ番号</param>
		public SearchValues(
			string recommendId,
			string recommendName,
			string recommendKbn,
			string status,
			string validFlg,
			string sortKbn,
			int pageNum)
		{
			this.RecommendId = recommendId;
			this.RecommendName = recommendName;
			this.RecommendKbn = recommendKbn;
			this.Status = status;
			this.ValidFlg = validFlg;
			this.SortKbn = sortKbn;
			this.PageNum = pageNum;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// レコメンド設定一覧URL作成
		/// </summary>
		/// <param name="addPageNo">ページを付けるかどうかフラグ（ページャ作成の際はfalse）</param>
		/// <param name="dateBgn">開始時間</param>
		/// <param name="dateEnd">終了時間</param>
		/// <returns>レコメンド設定一覧URL</returns>
		public string CreateRecommendListUrl(DateTime dateBgn, DateTime dateEnd, bool addPageNo = true)
		{
			var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_RECOMMEND_TABLE_REPORT);
			urlCreator.AddParam(Constants.REQUEST_KEY_CURRENT_YEAR, dateBgn.Year.ToString())
				.AddParam(Constants.REQUEST_KEY_CURRENT_MONTH, dateBgn.Month.ToString())
				.AddParam(Constants.REQUEST_KEY_CURRENT_DAY, dateBgn.Day.ToString())
				.AddParam(Constants.REQUEST_KEY_TARGET_YEAR, dateEnd.Year.ToString())
				.AddParam(Constants.REQUEST_KEY_TARGET_MONTH, dateEnd.Month.ToString())
				.AddParam(Constants.REQUEST_KEY_TARGET_DAY, dateEnd.Day.ToString())
				.AddParam(Constants.REQUEST_KEY_RECOMMEND_ID, this.RecommendId)
				.AddParam(Constants.REQUEST_KEY_RECOMMEND_NAME, this.RecommendName)
				.AddParam(Constants.REQUEST_KEY_RECOMMEND_KBN, this.RecommendKbn)
				.AddParam(Constants.REQUEST_KEY_RECOMMEND_VALID_FLG, this.ValidFlg)
				.AddParam(Constants.REQUEST_KEY_RECOMMEND_STATUS, this.Status)
				.AddParam(FIELD_RECOMMEND_SORTKBN, this.SortKbn);
			if (addPageNo)
			{
				urlCreator.AddParam(Constants.REQUEST_KEY_PAGE_NO, this.PageNum.ToString());
			}

			var url = urlCreator.CreateUrl();
			return url;
		}
		#endregion

		#region プロパティ
		/// <summary>レコメンドID</summary>
		public string RecommendId { get; set; }
		/// <summary>レコメンド名（表示用）</summary>
		public string RecommendName { get; set; }
		/// <summary>レコメンド区分</summary>
		public string RecommendKbn { get; set; }
		/// <summary>開催状態</summary>
		public string Status { get; set; }
		/// <summary>有効フラグ</summary>
		public string ValidFlg { get; set; }
		/// <summary>並び順</summary>
		public string SortKbn { get; set; }
		/// <summary>ページ番号</summary>
		public int PageNum { get; set; }
		#endregion
	}
	#endregion
}