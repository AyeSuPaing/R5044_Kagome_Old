/*
=========================================================================================================
  Module      : バッチ監視ページ(MallWatchingLogList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using w2.Common.Logger;

public partial class Form_MallWatchingLog_MallWatchingLogList : BasePage
{
	protected const string MALLWATCHINGLOG_WHERE_BATCH_ID = "@@where_batch_id@@";		// 条件式：バッチID
	protected const string MALLWATCHINGLOG_WHERE_LOG_KBN = "@@where_log_kbn@@";			// 条件式：ログ区分

	/// <summary>SQLページ名</summary>
	protected const string SQL_PAGE_NAME_MALLWATCHINGLOG = "MallWatchingLog";
	/// <summary>SQLステートメント名監視ログ情報一覧取得</summary>
	protected const string SQL_STATEMENT_NAME_GET_MALLWATCHINGLOG = "GetMallWatchingLogList";
	/// <summary>SQLステートメント名監視ログ情報一覧件数取得</summary>
	protected const string SQL_STATEMENT_NAME_GET_MALLWATCHINGLOG_COUNT = "GetMallWatchingLogListCount";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			dvLoadingImg.Visible = false;

			// 監視ログ一覧表示
			ViewMallWatchingLogList();
		}
	}

	/// <summary>
	/// 監視ログ情報一覧表示(DataGridにDataView(監視ログ情報)を設定)
	/// </summary>
	private void ViewMallWatchingLogList()
	{
		//------------------------------------------------------
		// 画面制御
		//------------------------------------------------------
		InitializeComponents();

		//------------------------------------------------------
		// リクエスト情報取得
		//------------------------------------------------------
		var htParam = GetParameters(Request);

		//------------------------------------------------------
		// パラメタセット
		//------------------------------------------------------
		tbMallId.Text = (string)htParam[Constants.REQUEST_KEY_MALLWATCHINGLOG_MALL_ID];
		SetSearchCheckBoxValue(cblBatchList, htParam[Constants.REQUEST_KEY_MALLWATCHINGLOG_BATCH_ID].ToString().Split(','));
		SetSearchCheckBoxValue(cblLogKbn, htParam[Constants.REQUEST_KEY_MALLWATCHINGLOG_LOG_KBN].ToString().Split(','));
		var ucMallWatchingLogDateStart = string.Format("{0}{1}",
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_MALLWATCHINGLOG_DATE_FROM]).Replace("/", string.Empty),
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_MALLWATCHINGLOG_TIME_FROM]).Replace(":", string.Empty));

		var ucMallWatchingLogDateEnd = string.Format("{0}{1}",
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_MALLWATCHINGLOG_DATE_TO]).Replace("/", string.Empty),
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_MALLWATCHINGLOG_TIME_TO]).Replace(":", string.Empty));

		ucMallWatchingLogDatePeriod.SetPeriodDate(
			ucMallWatchingLogDateStart,
			ucMallWatchingLogDateEnd);
		tbLogMessage.Text = (string)htParam[Constants.REQUEST_KEY_MALLWATCHINGLOG_LOG_MESSAGE];

		//------------------------------------------------------
		// 検索情報取得
		//------------------------------------------------------
		Hashtable htSearch = GetSearchSqlInfo(htParam);

		//------------------------------------------------------
		// 非同期に監視ログ一覧取得
		//------------------------------------------------------
		this.ProcessInfoCacheKey = Guid.NewGuid().ToString();

		this.ProcessInfo = new ProcessInfoType
		{
			TotalCount = 0,
			CurrentPageNumber = 0,
			MallWatchinLogList = new DataView()
		};
		tProcessTimer.Enabled = true;

		Task.Run(
			() =>
			{
				this.ProcessInfo.StartTime = DateTime.Now;
				try
				{	
					// 総件数取得
					this.ProcessInfo.TotalCount = (int)ActionSqlStatement(SQL_STATEMENT_NAME_GET_MALLWATCHINGLOG_COUNT, htSearch)[0][0];
					if (this.ProcessInfo.TotalCount == 0)
					{
						this.ProcessInfo.IsDone = true;
						return;
					}

					this.ProcessInfo.CurrentPageNumber = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];
					htSearch["bgn_row_num"] = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.ProcessInfo.CurrentPageNumber - 1) + 1;
					htSearch["end_row_num"] = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.ProcessInfo.CurrentPageNumber;
					this.ProcessInfo.MallWatchinLogList = ActionSqlStatement(SQL_STATEMENT_NAME_GET_MALLWATCHINGLOG, htSearch);
					this.ProcessInfo.IsDone = true;
					this.ProcessInfo.Param = htParam;
				}
				catch (Exception ex)
				{
					tProcessTimer.Enabled = false;
					this.ProcessInfo.IsSystemError = true;
					FileLogger.WriteError(ex);
				}
			});

		// 即座に取得出来ていた場合はクライアントのタイマーを回す必要がないので少しまってからタイマー処理を実行する
		Thread.Sleep(50);
		tProcessTimer_Tick(null, null);
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// バッチリスト
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MALLWATCHINGLOG, Constants.FIELD_MALLWATCHINGLOG_BATCH_ID))
		{
			// Remove item of Facebook catalog api cooperation setting when option setting off
			if ((Constants.FACEBOOK_CATALOG_API_COOPERATION_OPTION_ENABLED == false)
				&& (li.Value == Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISE_FACEBOOK_MALL)) continue;

			cblBatchList.Items.Add(li);
		}

		// ログ区分
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MALLWATCHINGLOG, Constants.FIELD_MALLWATCHINGLOG_LOG_KBN))
		{
			cblLogKbn.Items.Add(li);
		}
	}

	/// <summary>
	/// 実行時のタイマー処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void tProcessTimer_Tick(object sender, EventArgs e)
	{
		if (this.ProcessInfo == null)
		{
			tProcessTimer.Enabled = false;
			lProcessMessage.Text = WebSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_PROCESSINFO_ERROR));
			FileLogger.WriteInfo("モール連携監視ログ：商品別販売数取得処理情報は空になっています。");
			dvLoadingImg.Visible = false;
			return;
		}
		if (this.ProcessInfo.IsSystemError)
		{
			tProcessTimer.Enabled = false;
			lProcessMessage.Text = WebSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR));
			FileLogger.WriteInfo("モール連携監視ログ：システムエラーが発生しました。");
			dvLoadingImg.Visible = false;
			return;
		}

		lProcessMessage.Text = WebSanitizer.HtmlEncodeChangeToBr(
			string.Format(
				"処理中です...\r\n経過時間：{0:mm:ss}",
				(DateTime.Parse((DateTime.Now - this.ProcessInfo.StartTime).ToString()))));

		if (this.ProcessInfo.IsDone)
		{
			if (this.ProcessInfo.TotalCount == 0)
			{
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}
			else
			{
				//------------------------------------------------------
				// ページャ作成（一覧処理で総件数を取得）
				//------------------------------------------------------
				lbPager1.Text = WebPager.CreateDefaultListPager(
					this.ProcessInfo.TotalCount,
					this.ProcessInfo.CurrentPageNumber,
					CreateMallWatchingLogListUrl(this.ProcessInfo.Param));

				// データバインド
				rList.DataSource = this.ProcessInfo.MallWatchinLogList;
				rList.DataBind();
			}

			FileLogger.WriteInfo("処理完了");
			lProcessMessage.Text = string.Empty;
			tProcessTimer.Enabled = false;
			dvLoadingImg.Visible = false;
		}
		else
		{
			dvLoadingImg.Visible = true;
		}
	}

	/// <summary>
	/// 商品一覧パラメタ取得
	/// </summary>
	/// <param name="hrRequest">商品一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	/// <remarks>
	/// </remarks>
	protected Hashtable GetParameters(HttpRequest hrRequest)
	{
		// 変数宣言
		Hashtable htResult = new Hashtable();
		int iCurrentPageNumber = 1;
		string strSortKbn = String.Empty;

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		htResult.Add(Constants.REQUEST_KEY_MALLWATCHINGLOG_MALL_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_MALLWATCHINGLOG_MALL_ID]));
		htResult.Add(Constants.REQUEST_KEY_MALLWATCHINGLOG_BATCH_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_MALLWATCHINGLOG_BATCH_ID]));
		htResult.Add(Constants.REQUEST_KEY_MALLWATCHINGLOG_LOG_KBN, (Request.Url.ToString().IndexOf('?') != -1) ? StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_MALLWATCHINGLOG_LOG_KBN]) : Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR + "," + Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING);

		htResult.Add(Constants.REQUEST_KEY_MALLWATCHINGLOG_DATE_FROM, (Request.Url.ToString().IndexOf('?') != -1)
			? StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_MALLWATCHINGLOG_DATE_FROM])
			: DateTime.Now.Date.ToString("yyyy/MM/dd"));
		htResult.Add(Constants.REQUEST_KEY_MALLWATCHINGLOG_TIME_FROM, (Request.Url.ToString().IndexOf('?') != -1)
			? StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_MALLWATCHINGLOG_TIME_FROM])
			: DateTime.Now.Date.ToString("HH:mm:ss"));

		if (string.IsNullOrEmpty(ucMallWatchingLogDatePeriod.StartDateString) == false)
		{
			var ucMallWatchingLogDateFrom = DateTime.Parse(ucMallWatchingLogDatePeriod.StartDateString);
			// 末日補正処理
			if (DateTimeUtility.IsLastDayOfMonth(
				ucMallWatchingLogDateFrom.Year,
				ucMallWatchingLogDateFrom.Month,
				ucMallWatchingLogDateFrom.Day))
			{
				var ucMallWatchingLogLastDayOfMonth = DateTimeUtility.GetLastDayOfMonth(
					ucMallWatchingLogDateFrom.Year,
					ucMallWatchingLogDateFrom.Month);
				htResult[Constants.REQUEST_KEY_MALLWATCHINGLOG_DATE_FROM] =
					ucMallWatchingLogDateFrom.AddDays(ucMallWatchingLogLastDayOfMonth).ToString("yyyy/MM/dd");
				htResult[Constants.REQUEST_KEY_MALLWATCHINGLOG_TIME_FROM] =
					ucMallWatchingLogDateFrom.AddDays(ucMallWatchingLogLastDayOfMonth).ToString("HH:mm:ss");
			}
		}

		htResult.Add(Constants.REQUEST_KEY_MALLWATCHINGLOG_DATE_TO, (Request.Url.ToString().IndexOf('?') != -1)
			? StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_MALLWATCHINGLOG_DATE_TO])
			: DateTime.Now.Date.ToString("yyyy/MM/dd"));
		htResult.Add(Constants.REQUEST_KEY_MALLWATCHINGLOG_TIME_TO, (Request.Url.ToString().IndexOf('?') != -1)
			? StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_MALLWATCHINGLOG_TIME_TO])
			: DateTime.Now.Date.ToString("23:59:59"));

		if (string.IsNullOrEmpty(ucMallWatchingLogDatePeriod.EndDateString) == false)
		{
			var ucMallWatchingLogDateTo = DateTime.Parse((ucMallWatchingLogDatePeriod.EndDateString));
			// 末日補正処理
			if (DateTimeUtility.IsLastDayOfMonth(
				ucMallWatchingLogDateTo.Year,
				ucMallWatchingLogDateTo.Month,
				ucMallWatchingLogDateTo.Day))
			{
				var ucMallWatchingLogLastDayOfMonth = DateTimeUtility.GetLastDayOfMonth(
					ucMallWatchingLogDateTo.Year,
					ucMallWatchingLogDateTo.Month);
				htResult[Constants.REQUEST_KEY_MALLWATCHINGLOG_DATE_TO] =
					ucMallWatchingLogDateTo.AddDays(ucMallWatchingLogLastDayOfMonth).ToString("yyyy/MM/dd");
				htResult[Constants.REQUEST_KEY_MALLWATCHINGLOG_TIME_TO] =
					ucMallWatchingLogDateTo.AddDays(ucMallWatchingLogLastDayOfMonth).ToString("HH:mm:ss");
			}
		}
		htResult.Add(Constants.REQUEST_KEY_MALLWATCHINGLOG_LOG_MESSAGE, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_MALLWATCHINGLOG_LOG_MESSAGE]));

		// ページ番号（ページャ動作時のみもちまわる）
		if (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_PAGE_NO]) != "")
		{
			iCurrentPageNumber = int.Parse(hrRequest[Constants.REQUEST_KEY_PAGE_NO]);
		}

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		htResult.Add(Constants.REQUEST_KEY_PAGE_NO, iCurrentPageNumber);

		return htResult;
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

		//------------------------------------------------------
		// 検索情報取得
		//------------------------------------------------------
		htResult.Add(Constants.FIELD_MALLWATCHINGLOG_MALL_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_MALLWATCHINGLOG_MALL_ID]));
		htResult.Add(Constants.FIELD_MALLWATCHINGLOG_BATCH_ID, StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_MALLWATCHINGLOG_BATCH_ID]));
		htResult.Add(Constants.FIELD_MALLWATCHINGLOG_LOG_KBN, StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_MALLWATCHINGLOG_LOG_KBN]));
		htResult.Add(Constants.FIELD_MALLWATCHINGLOG_LOG_MESSAGE + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_MALLWATCHINGLOG_LOG_MESSAGE]));
		// 監視ログ抽出日初期化
		htResult.Add(Constants.FIELD_MALLWATCHINGLOG_WATCHING_DATE + "_from", DBNull.Value);
		htResult.Add(Constants.FIELD_MALLWATCHINGLOG_WATCHING_DATE + "_to", DBNull.Value);
		// 監視ログ抽出日(From)
		string strDateFrom = (string)htSearch[Constants.REQUEST_KEY_MALLWATCHINGLOG_DATE_FROM];
		if (Validator.IsDate(strDateFrom))
		{
			htResult[Constants.FIELD_MALLWATCHINGLOG_WATCHING_DATE + "_from"] = string.Format("{0} {1}",
				strDateFrom,
				(string)htSearch[Constants.REQUEST_KEY_MALLWATCHINGLOG_TIME_FROM]);
		}
		// 監視ログ抽出日(To)
		string strDateTo = (string)htSearch[Constants.REQUEST_KEY_MALLWATCHINGLOG_DATE_TO];
		if (Validator.IsDate(strDateTo))
		{
			var dateTo = DateTime.Parse(string.Format("{0} {1}",
				strDateTo,
				(string)htSearch[Constants.REQUEST_KEY_MALLWATCHINGLOG_TIME_TO])).AddSeconds(1).ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT);
			htResult[Constants.FIELD_MALLWATCHINGLOG_WATCHING_DATE + "_to"] = dateTo;
		}

		return htResult;

	}

	/// <summary>
	///　SQLステートメントを実行する
	/// </summary>
	/// <param name="statementName">SQLステートメント</param>
	/// <param name="htSearch">検索条件</param>
	/// <returns>実行結果</returns>
	private DataView ActionSqlStatement(string statementName, Hashtable htSearch)
	{
		using (var sqlAccessor = new SqlAccessor())
		using (var sqlStatement = new SqlStatement(SQL_PAGE_NAME_MALLWATCHINGLOG, statementName) { CommandTimeout = 600000 })
		{
			// 条件：バッチID
			bool blSelected = false;
			StringBuilder sbBatchId = new StringBuilder();
			string[] stringBatchId = htSearch[Constants.FIELD_MALLWATCHINGLOG_BATCH_ID].ToString().Split(',');
			foreach (string str in stringBatchId)
			{
				sbBatchId.Append(blSelected ? "," : "").Append("'").Append(str).Append("'");
				blSelected = true;
			}
			// 条件：ログ区分
			blSelected = false;
			StringBuilder sbLogKbn = new StringBuilder();
			string[] stringLogKbn = htSearch[Constants.FIELD_MALLWATCHINGLOG_LOG_KBN].ToString().Split(',');
			foreach (string str in stringLogKbn)
			{
				sbLogKbn.Append(blSelected ? "," : "").Append("'").Append(str).Append("'");
				blSelected = true;
			}
			// 条件作成
			StringBuilder sbWhereBatchId = new StringBuilder();
			StringBuilder sbWhereLogKbn = new StringBuilder();
			sbWhereBatchId.Append(Constants.TABLE_MALLWATCHINGLOG).Append(".").Append(Constants.FIELD_MALLWATCHINGLOG_BATCH_ID).Append(" IN(").Append(sbBatchId.ToString()).Append(")");
			sbWhereLogKbn.Append(Constants.TABLE_MALLWATCHINGLOG).Append(".").Append(Constants.FIELD_MALLWATCHINGLOG_LOG_KBN).Append(" IN(").Append(sbLogKbn.ToString()).Append(")");
			
			// SQL発行
			sqlStatement.Statement = sqlStatement.Statement.Replace(MALLWATCHINGLOG_WHERE_LOG_KBN, sbWhereLogKbn.ToString()).Replace(MALLWATCHINGLOG_WHERE_BATCH_ID, sbWhereBatchId.ToString());
			DataSet ds = sqlStatement.SelectStatementWithOC(sqlAccessor, htSearch);
			
			return ds.Tables["Table"].DefaultView;
		}
	}

	/// <summary>
	/// 商品一覧遷移URL作成
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <returns>商品一覧遷移URL</returns>
	private string CreateMallWatchingLogListUrl(Hashtable htSearch)
	{
		bool blSelected = false;
		System.Text.StringBuilder sbResult = new System.Text.StringBuilder();
		sbResult.Append(Constants.PATH_ROOT);
		sbResult.Append(Constants.PAGE_MANAGER_MALL_WATCHING_LOG_LIST);
		sbResult.Append("?");
		sbResult.Append(Constants.REQUEST_KEY_MALLWATCHINGLOG_MALL_ID);
		sbResult.Append("=");
		sbResult.Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_MALLWATCHINGLOG_MALL_ID]));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_MALLWATCHINGLOG_BATCH_ID + "=");
		for (int iLoop = 0; iLoop < cblBatchList.Items.Count; iLoop++)
		{
			// 選択されている場合
			if (cblBatchList.Items[iLoop].Selected)
			{
				sbResult.Append(blSelected ? "," + cblBatchList.Items[iLoop].Value : cblBatchList.Items[iLoop].Value);
				blSelected = true;
			}
		}
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_MALLWATCHINGLOG_LOG_KBN + "=");
		for (int iLoop = 0; iLoop < cblLogKbn.Items.Count; iLoop++)
		{
			// 選択されている場合
			if (cblLogKbn.Items[iLoop].Selected)
			{
				sbResult.Append(blSelected ? "," + cblLogKbn.Items[iLoop].Value : cblLogKbn.Items[iLoop].Value);
				blSelected = true;
			}
		}
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_MALLWATCHINGLOG_DATE_FROM);
		sbResult.Append("=");
		sbResult.Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_MALLWATCHINGLOG_DATE_FROM].ToString()));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_MALLWATCHINGLOG_DATE_TO);
		sbResult.Append("=");
		sbResult.Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_MALLWATCHINGLOG_DATE_TO].ToString()));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_MALLWATCHINGLOG_TIME_FROM);
		sbResult.Append("=");
		sbResult.Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_MALLWATCHINGLOG_TIME_FROM].ToString()));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_MALLWATCHINGLOG_TIME_TO);
		sbResult.Append("=");
		sbResult.Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_MALLWATCHINGLOG_TIME_TO].ToString()));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_MALLWATCHINGLOG_LOG_MESSAGE);
		sbResult.Append("=");
		sbResult.Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_MALLWATCHINGLOG_LOG_MESSAGE]));

		return sbResult.ToString();
	}

	/// <summary>
	/// 監視ログ一覧遷移URL作成
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>監視ログ一覧遷移URL</returns>
	private string CreateMallWatchingLogListUrl(Hashtable htSearch, int iPageNumber)
	{
		string strResult = CreateMallWatchingLogListUrl(htSearch);
		strResult += "&";
		strResult += Constants.REQUEST_KEY_PAGE_NO + "=" + iPageNumber.ToString();

		return strResult;
	}

	/// <summary>
	/// 各検索コントロールから検索情報取得
	/// </summary>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchInfo()
	{
		// 変数宣言
		Hashtable htSearch = new Hashtable();

		htSearch.Add(Constants.REQUEST_KEY_MALLWATCHINGLOG_MALL_ID, tbMallId.Text.Trim());									// モールID
		htSearch.Add(Constants.REQUEST_KEY_MALLWATCHINGLOG_BATCH_ID, cblBatchList.SelectedValue);							// バッチID
		htSearch.Add(Constants.REQUEST_KEY_MALLWATCHINGLOG_LOG_KBN, cblLogKbn.SelectedValue);								// ログ区分
		htSearch.Add(Constants.REQUEST_KEY_MALLWATCHINGLOG_DATE_FROM, ucMallWatchingLogDatePeriod.HfStartDate.Value);		// Request key mall watching log date from
		htSearch.Add(Constants.REQUEST_KEY_MALLWATCHINGLOG_DATE_TO, ucMallWatchingLogDatePeriod.HfEndDate.Value);			// Request key mall watching log date to
		htSearch.Add(Constants.REQUEST_KEY_MALLWATCHINGLOG_TIME_FROM, ucMallWatchingLogDatePeriod.HfStartTime.Value);		// Request key mall watching log time from
		htSearch.Add(Constants.REQUEST_KEY_MALLWATCHINGLOG_TIME_TO, ucMallWatchingLogDatePeriod.HfEndTime.Value);			// Request key mall watching log time to
		htSearch.Add(Constants.REQUEST_KEY_MALLWATCHINGLOG_LOG_MESSAGE, tbLogMessage.Text.Trim());						// ログメッセージ

		return htSearch;
	}

	/// <summary>
	/// 検索実行イベントハンドラ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void MallWatchingLogSearch_Click(object sender, EventArgs e)
	{
		// 検索用パラメタ作成し、同じ画面にリダイレクト
		Response.Redirect(CreateMallWatchingLogListUrl(GetSearchInfo(), 1));
	}

	/// <summary>
	/// 表示用文字列を作成する
	/// </summary>
	/// <param name="strMessage">ベース文字列</param>
	/// <param name="iByte">ベース文字列の表示バイト数</param>
	/// <returns>編集後文字列</returns>
	protected string CreateDisplayMessage(string strMessage, int iByte)
	{
		return ((Encoding.GetEncoding("Shift_JIS").GetByteCount(strMessage) > (iByte % 2 == 0 ? iByte : iByte + 1)) ? Encoding.GetEncoding("Shift_JIS").GetString(Encoding.GetEncoding("Shift_JIS").GetBytes(strMessage), 0, iByte) + "…" : strMessage);
	}

	/// <summary>処理情報（非同期スレッドでもアクセス可能なようにWEBキャッシュ格納）</summary>
	public ProcessInfoType ProcessInfo
	{
		get
		{
			var val = this.Cache[this.ProcessInfoCacheKey];
			if ((val is ProcessInfoType) == false) val = null;
			return (ProcessInfoType)val;
		}
		set
		{
			if (value != null)
			{
				this.Cache.Insert(
					this.ProcessInfoCacheKey,
					value,
					null,
					System.Web.Caching.Cache.NoAbsoluteExpiration,	// 絶対日付期限切れなし
					TimeSpan.FromMinutes(5));	// 5分キャッシュにアクセスが無かったら破棄する
			}
			else
			{
				this.Cache.Remove(this.ProcessInfoCacheKey);
			}
		}
	}
	/// <summary>処理情報キャッシュキー</summary>
	private string ProcessInfoCacheKey
	{
		get { return (string)ViewState["ProcessInfoCacheKey"]; }
		set { ViewState["ProcessInfoCacheKey"] = value; }
	}
}

/// <summary>
/// 処理情報（非同時処理とのやり取りに利用する）
/// </summary>
[Serializable]
public class ProcessInfoType
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public ProcessInfoType()
	{
		this.IsSystemError = false;
		this.IsDone = false;
		this.Param = new Hashtable();
		this.MallWatchinLogList = new DataView();
		this.TotalCount = 0;
		this.CurrentPageNumber = 0;
		this.StartTime = new DateTime();
	}
	/// <summary>システムエラー発生したか</summary>
	public bool IsSystemError { get; set; }
	/// <summary>処理完了したか</summary>
	public bool IsDone { get; set; }
	/// <summary>検索条件用のパラメタ</summary>
	public Hashtable Param { get; set; }
	/// <summary>モール連携監視ログ一覧</summary>
	public DataView MallWatchinLogList { get; set; }
	/// <summary>実行結果件数</summary>
	public int TotalCount { get; set; }
	/// <summary>ページ番号</summary>
	public int CurrentPageNumber { get; set; }
	/// <summary>処理開始時間</summary>
	public DateTime StartTime { get; set; }
}
