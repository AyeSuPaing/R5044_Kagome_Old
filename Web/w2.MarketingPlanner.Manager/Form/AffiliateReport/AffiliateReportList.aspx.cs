/*
=========================================================================================================
  Module      : アフィリエイトレポート一覧ページ処理(AffiliateReportList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Util;
using w2.Domain.ShopOperator;

public partial class Form_AffiliateReport_AffiliateReportList : BasePage
{
	#region 定数
	private const string SQLSTATEMENT_WHERE = "@@ where @@";	// 抽出条件文
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		// ユーザーコントロール割り当て
		uMasterDownload.OnCreateSearchInputParams += this.CreateSearchParams;

		if (!IsPostBack)
		{
			Hashtable htParam = new Hashtable();

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents();

			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			htParam = GetParameters(Request);

			//------------------------------------------------------
			// パラメタセット
			//------------------------------------------------------
			// アフィリエイト区分
			foreach (ListItem li in ddlAffiliateKbn.Items)
			{
				li.Selected = (li.Value == (string)htParam[Constants.REQUEST_KEY_AFFILIATET_REPORT_AFFILIATE_KBN]);
			}
			tbMasterId.Text = (string)htParam[Constants.REQUEST_KEY_AFFILIATET_REPORT_MASTER_ID];	// マスタID

			if (string.IsNullOrEmpty(StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_AFFILIATET_REPORT_DATE_FROM])) == false)
			{
				// 抽出(From)
				var affiliatetReportDateStart = DateTime.Parse(string.Format("{0} {1}",
					StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_AFFILIATET_REPORT_DATE_FROM]),
					StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_AFFILIATET_REPORT_TIME_FROM])));
				ucAffiliateReportDatePeriod.SetStartDate(affiliatetReportDateStart);
			}
			if (string.IsNullOrEmpty(StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_AFFILIATET_REPORT_DATE_TO])) == false)
			{
				// 抽出(To)
				var affiliatetReportDateEnd = DateTime.Parse(string.Format("{0} {1}",
					StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_AFFILIATET_REPORT_DATE_TO]),
					StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_AFFILIATET_REPORT_TIME_TO])));
				ucAffiliateReportDatePeriod.SetEndDate(affiliatetReportDateEnd);
			}
			tbTagId.Text = (string)htParam[Constants.REQUEST_KEY_AFFILIATET_REPORT_TAG_ID];	// タグID

			//------------------------------------------------------
			// 検索情報取得
			//------------------------------------------------------
			Hashtable htSearch = GetSearchSqlInfo(htParam);
			int iCurrentPageNumber = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];

			//------------------------------------------------------
			// アフィリエイトレポートの該当件数取得
			//------------------------------------------------------
			var intTag = 0;
			var isIntTagId = ((string.IsNullOrEmpty(tbTagId.Text)) || (int.TryParse(tbTagId.Text, out intTag)));
			int iTotalAffiliateReportCounts = (isIntTagId) ? GetAffiliateReportCount(htSearch) : 0;	// ページング可能総更新履歴数

			//------------------------------------------------------
			// アフィリエイトレポート一覧表示
			//------------------------------------------------------
			// 表示可能上限数を超えている場合
			if (iTotalAffiliateReportCounts > Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST)
			{
				// エラー表示制御
				trListError.Visible = true;
				StringBuilder sbErrorMsg = new StringBuilder();
				sbErrorMsg.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_OVER_HIT_LIST));
				sbErrorMsg.Replace("@@ 1 @@", StringUtility.ToNumeric(iTotalAffiliateReportCounts));
				sbErrorMsg.Replace("@@ 2 @@", StringUtility.ToNumeric(Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST));
				tdErrorMessage.InnerHtml = sbErrorMsg.ToString();
			}
			else
			{
				// アフィリエイトレポート取得
				DataView dvAffiliateReport = (isIntTagId) ? GetAffiliateReportDataView(htSearch, iCurrentPageNumber) : new DataView();

				// アフィリエイトレポートが存在する場合
				if (dvAffiliateReport.Count != 0)
				{
					// エラー非表示制御
					trListError.Visible = false;
				}
				else
				{
					// エラー表示制御
					trListError.Visible = true;
					tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
				}
				// データソースセット
				rList.DataSource = dvAffiliateReport;
				rList.DataBind();
			}

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			string strNextUrl = CreateAffiliateReportListUrl(htParam);
			lbPager1.Text = WebPager.CreateDefaultListPager(iTotalAffiliateReportCounts, iCurrentPageNumber, strNextUrl);
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// アフィリエイト区分
		ddlAffiliateKbn.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_AFFILIATECOOPLOG, Constants.FIELD_AFFILIATECOOPLOG_AFFILIATE_KBN))
		{
			ddlAffiliateKbn.Items.Add(li);
		}
	}

	/// <summary>
	/// アフィリエイトレポート一覧のパラメタ取得
	/// </summary>
	/// <param name="hrRequest">アフィリエイトレポート一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParameters(HttpRequest hrRequest)
	{
		Hashtable htResult = new Hashtable();
		string strSortKbn = String.Empty;

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		htResult.Add(Constants.REQUEST_KEY_AFFILIATET_REPORT_AFFILIATE_KBN, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_AFFILIATET_REPORT_AFFILIATE_KBN]));		// アフィリエイト区分
		htResult.Add(Constants.REQUEST_KEY_AFFILIATET_REPORT_MASTER_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_AFFILIATET_REPORT_MASTER_ID]));				// マスタID
		htResult.Add(Constants.REQUEST_KEY_AFFILIATET_REPORT_DATE_FROM, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_AFFILIATET_REPORT_DATE_FROM]));		// 抽出(From)
		htResult.Add(Constants.REQUEST_KEY_AFFILIATET_REPORT_TIME_FROM, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_AFFILIATET_REPORT_TIME_FROM]));		// 抽出(From)
		htResult.Add(Constants.REQUEST_KEY_AFFILIATET_REPORT_DATE_TO, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_AFFILIATET_REPORT_DATE_TO]));			// 抽出(To)
		htResult.Add(Constants.REQUEST_KEY_AFFILIATET_REPORT_TIME_TO, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_AFFILIATET_REPORT_TIME_TO]));			// 抽出(To)
		htResult.Add(Constants.REQUEST_KEY_AFFILIATET_REPORT_TAG_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_AFFILIATET_REPORT_TAG_ID]));				// タグID

		// ページ番号（ページャ動作時のみもちまわる）
		int iCurrentPageNumber = 1;
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
	/// <param name="htSearch">検索情報</param>.
	/// <param name="isMaster">マスター出力用か</param>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchSqlInfo(Hashtable htSearch, bool isMaster = false)
	{
		Hashtable htResult = new Hashtable();

		//------------------------------------------------------
		// 検索情報取得
		//------------------------------------------------------
		// アフィリエイト区分
		htResult.Add(Constants.FIELD_AFFILIATECOOPLOG_AFFILIATE_KBN,
			StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_AFFILIATET_REPORT_AFFILIATE_KBN]));
		// マスタID
		htResult.Add(Constants.FIELD_AFFILIATECOOPLOG_MASTER_ID + "_like_escaped",
			StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_AFFILIATET_REPORT_MASTER_ID]));
		// アフィリエイトレポート抽出日初期化
		htResult.Add(Constants.FIELD_AFFILIATECOOPLOG_DATE_CREATED + "_from", System.DBNull.Value);
		htResult.Add(Constants.FIELD_AFFILIATECOOPLOG_DATE_CREATED + "_to", System.DBNull.Value);
		// アフィリエイトレポート抽出日(From)
		var strDateFrom = StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_AFFILIATET_REPORT_DATE_FROM]);
		if (Validator.IsDate(strDateFrom))
		{
			htResult[Constants.FIELD_AFFILIATECOOPLOG_DATE_CREATED + "_from"] = string.Format("{0} {1}",
				strDateFrom,
				StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_AFFILIATET_REPORT_TIME_FROM]));
		}
		// アフィリエイトレポート抽出日(To)
		var strDateTo = StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_AFFILIATET_REPORT_DATE_TO]);
		if (Validator.IsDate(strDateTo))
		{
			var dateTo = DateTime.Parse(string.Format("{0} {1}",
				strDateTo,
				StringUtility.ToEmpty(htSearch[Constants.REQUEST_KEY_AFFILIATET_REPORT_TIME_TO])))
				.AddSeconds(1).ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT);
			htResult[Constants.FIELD_AFFILIATECOOPLOG_DATE_CREATED + "_to"] = dateTo;
		}
		// タグID
		htResult.Add(Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA11,
			StringUtility.SqlLikeStringSharpEscape(htSearch[Constants.REQUEST_KEY_AFFILIATET_REPORT_TAG_ID]));

		if (isMaster) htResult.Add(SQLSTATEMENT_WHERE, GetUsableAffiliateTag());

		return htResult;
	}

	/// <summary>
	/// アフィリエイトレポートから検索条件に該当する件数を取得
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <returns>アフィリエイトレポートの検索該当件数</returns>
	private int GetAffiliateReportCount(Hashtable htSearch)
	{
		int iTotalAffiliateReportCounts = 0;

		// ステートメントからアフィリエイト連携ログの該当件数取得
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("AffiliateCoopLog", "GetAffiliateReportCount"))
		{
			sqlStatement.Statement = sqlStatement.Statement.Replace(SQLSTATEMENT_WHERE, GetUsableAffiliateTag());
			DataView dv = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htSearch);
			if (dv.Count != 0)
			{
				iTotalAffiliateReportCounts = (int)dv[0]["row_count"];
			}
		}

		return iTotalAffiliateReportCounts;
	}

	/// <summary>
	/// オペレータの閲覧可能なタグ(抽出条件)取得
	/// </summary>
	/// <returns>オペレータの閲覧可能なタグ(抽出条件)</returns>
	private string GetUsableAffiliateTag()
	{
		var whereCondition = string.Empty;
		var loginOperator = new ShopOperatorService().Get(this.LoginOperatorShopId, this.LoginOperatorId);
		if (loginOperator.UsableAffiliateTagIdsInReport.Length > 0)
		{
			var updateAdCodeArray = StringUtility.SplitCsvLine(loginOperator.UsableAffiliateTagIdsInReport);
			whereCondition = "AND  " + Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA11 + " IN ('" + string.Join(
				"', '",
				updateAdCodeArray.Select(s => StringUtility.SqlLikeStringSharpEscape(s)).ToArray()) + "')";
		}

		return whereCondition;
	}

	/// <summary>
	/// アフィリエイトレポート一覧データビューを表示分だけ取得
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>アフィリエイトレポート一覧データビュー</returns>
	private DataView GetAffiliateReportDataView(Hashtable htSearch, int iPageNumber)
	{
		DataView dvResult = null;

		// ステートメントからアフィリエイト連携ログ一覧データ取得
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("AffiliateCoopLog", "GetAffiliateReportList"))
		{
			htSearch.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iPageNumber - 1) + 1);	// 表示開始記事番号
			htSearch.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iPageNumber);				// 表示開始記事番号

			sqlStatement.Statement = sqlStatement.Statement.Replace(SQLSTATEMENT_WHERE, GetUsableAffiliateTag());
			DataSet ds = sqlStatement.SelectStatementWithOC(sqlAccessor, htSearch);
			dvResult = ds.Tables["Table"].DefaultView;
		}

		return dvResult;
	}

	/// <summary>
	/// アフィリエイトレポート一覧遷移URL作成
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <returns>アフィリエイトレポート一覧遷移URL</returns>
	private string CreateAffiliateReportListUrl(Hashtable htSearch)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT);
		sbResult.Append(Constants.PAGE_MANAGER_AFFILIATET_REPORT_LIST);
		// アフィリエイト区分
		sbResult.Append("?");
		sbResult.Append(Constants.REQUEST_KEY_AFFILIATET_REPORT_AFFILIATE_KBN);
		sbResult.Append("=");
		sbResult.Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_AFFILIATET_REPORT_AFFILIATE_KBN]));
		// マスタID
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_AFFILIATET_REPORT_MASTER_ID);
		sbResult.Append("=");
		sbResult.Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_AFFILIATET_REPORT_MASTER_ID]));
		// 抽出(From)
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_AFFILIATET_REPORT_DATE_FROM);
		sbResult.Append("=");
		sbResult.Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_AFFILIATET_REPORT_DATE_FROM]));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_AFFILIATET_REPORT_TIME_FROM);
		sbResult.Append("=");
		sbResult.Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_AFFILIATET_REPORT_TIME_FROM]));
		// 抽出(To)
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_AFFILIATET_REPORT_DATE_TO);
		sbResult.Append("=");
		sbResult.Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_AFFILIATET_REPORT_DATE_TO]));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_AFFILIATET_REPORT_TIME_TO);
		sbResult.Append("=");
		sbResult.Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_AFFILIATET_REPORT_TIME_TO]));
		// タグID
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_AFFILIATET_REPORT_TAG_ID);
		sbResult.Append("=");
		sbResult.Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_AFFILIATET_REPORT_TAG_ID]));

		return sbResult.ToString();
	}

	/// <summary>
	/// アフィリエイトレポート一覧遷移URL作成
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>アフィリエイトレポート一覧遷移URL</returns>
	private string CreateAffiliateReportListUrl(Hashtable htSearch, int iPageNumber)
	{
		string strResult = CreateAffiliateReportListUrl(htSearch);
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
		Hashtable htSearch = new Hashtable();

		htSearch.Add(Constants.REQUEST_KEY_AFFILIATET_REPORT_AFFILIATE_KBN, ddlAffiliateKbn.SelectedValue);						// アフィリエイト区分
		htSearch.Add(Constants.REQUEST_KEY_AFFILIATET_REPORT_MASTER_ID, tbMasterId.Text.Trim());								// マスタID
		htSearch.Add(Constants.REQUEST_KEY_AFFILIATET_REPORT_DATE_FROM, ucAffiliateReportDatePeriod.HfStartDate.Value);			// 抽出(From)
		htSearch.Add(Constants.REQUEST_KEY_AFFILIATET_REPORT_TIME_FROM, ucAffiliateReportDatePeriod.HfStartTime.Value);			// 抽出(From)
		htSearch.Add(Constants.REQUEST_KEY_AFFILIATET_REPORT_DATE_TO, ucAffiliateReportDatePeriod.HfEndDate.Value);				// 抽出(To)
		htSearch.Add(Constants.REQUEST_KEY_AFFILIATET_REPORT_TIME_TO, ucAffiliateReportDatePeriod.HfEndTime.Value);				// 抽出(To)
		htSearch.Add(Constants.REQUEST_KEY_AFFILIATET_REPORT_TAG_ID, tbTagId.Text.Trim());								//タグID

		return htSearch;
	}

	/// <summary>
	/// 検索実行イベントハンドラ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		// 検索用パラメタ作成し、同じ画面にリダイレクト
		Response.Redirect(CreateAffiliateReportListUrl(GetSearchInfo(), 1));
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

	/// <summary>
	/// アフィリエイト区分の表示用文字列を作成
	/// </summary>
	/// <param name="strAffiliateKbn">アフィリエイト区分ｎ</param>
	/// <returns>表示用文字列</returns>
	protected string CreateDisplayAffiliateKbn(string strAffiliateKbn)
	{
		string strResult = null;
		// アフィリエイト区分が「リンクシェア」、「汎用アフィリエイト(PC)」の場合は、Value値を読込み
		if ((strAffiliateKbn == Constants.FLG_AFFILIATECOOPLOG_AFFILIATE_KBN_LINKSHARE_REP)
			|| (strAffiliateKbn == Constants.FLG_AFFILIATECOOPLOG_AFFILIATE_KBN_PC))
		{
			strResult = ValueText.GetValueText(Constants.TABLE_AFFILIATECOOPLOG, Constants.FIELD_AFFILIATECOOPLOG_AFFILIATE_KBN, strAffiliateKbn);
		}
		// それ以外は、アフィリエイト区分(セッション変数名1)をそのまま返却
		else
		{
			strResult = strAffiliateKbn;
		}

		return strResult;
	}

	/// <summary>
	/// マスタデータ出力用の検索ハッシュテーブル生成
	/// </summary>
	/// <returns>検索ハッシュテーブル</returns>
	/// <remarks>マスタ出力ユーザコントロールのイベントに割り当てて使う</remarks>
	public Hashtable CreateSearchParams()
	{
		return GetSearchSqlInfo(GetParameters(Request), true);
	}
}
