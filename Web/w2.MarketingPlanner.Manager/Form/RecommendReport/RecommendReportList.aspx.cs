/*
=========================================================================================================
  Module      : レコメンドレポート レポート詳細対象選択ページ処理(RecommendReportList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using w2.Common.Helper;
using w2.Common.Web;
using w2.Domain.Recommend;

public partial class Form_RecommendReport_RecommendReportList : BasePage
{
	#region 定数
	/// <summary>集計データ</summary>
	protected ArrayList _allTableData = new ArrayList();
	///<summary>日付</summary>
	private string _horizonalLabel;
	///<summary>値</summary>
	private string _verticalLabel;
	///<summary>単位</summary>
	private string _verticalUnit;
	///<summary>統計項目パラメータ</summary>
	private const string REQUEST_KEY_DATA_KBN = "dk";
	///<summary>統計項目：PV数</summary>
	private const string KBN_DISP_PV_NUM = "0";
	///<summary>統計項目：CV数</summary>
	private const string KBN_DISP_CV_NUM = "1";
	///<summary>統計項目：CV率</summary>
	private const string KBN_DISP_CV_PER = "2";
	///<summary>グラフ形式パラメータ名</summary>
	private const string REQUEST_KEY_CHART_TYPE = "ct";
	///<summary>グラフ形式：棒</summary>
	private const string KBN_CHART_COLUMN = "0";
	///<summary>グラフ形式：折り線</summary>
	private const string KBN_CHART_LINE = "1";
	///<summary>グラフ形式：数値あり、なし</summary>
	private const string KBN_VIEW_NUMBER = "0";
	///<summary>集計データ：日付</summary>
	protected const string TABLE_HEADER_DAY = "day";
	///<summary>集計データ：日付の値</summary>
	protected const string TABLE_DATA_VALUE = "value";
	///<summary>集計データ：値</summary>
	protected const string DATA_DATE_VALUE = "counts";
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
			// リクエストパラメタ取得
			// レコメンドID
			if (string.IsNullOrEmpty(this.RequestRecommendId) == false)
			{
				tbRecommendId.Text = HtmlSanitizer.HtmlEncode(this.RequestRecommendId);
			}

			// グラフタイプ
			if (string.IsNullOrEmpty(this.Request[REQUEST_KEY_CHART_TYPE]) == false)
			{
				var graphChartType = this.Request[REQUEST_KEY_CHART_TYPE];
				foreach (ListItem li in rblChartType.Items)
				{
					li.Selected = li.Value == graphChartType;
				}
			}

			// データタイプ
			if (string.IsNullOrEmpty(this.Request[REQUEST_KEY_DATA_KBN]) == false)
			{
				recommendPv.Checked = recommendCv.Checked = recommendCvp.Checked = false;

				switch (this.Request[REQUEST_KEY_DATA_KBN])
				{
					case KBN_DISP_PV_NUM:
						recommendPv.Checked = true;
						break;

					case KBN_DISP_CV_NUM:
						recommendCv.Checked = true;
						break;

					// デフォルトはPV数
					default:
						recommendCvp.Checked = true;
						break;
				}
			}

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

			var recommendIdErrorMessage = CheckRecommendId();
			if (string.IsNullOrEmpty(recommendIdErrorMessage) == false)
			{
				lRecommendReportRecommendIdErrorMessages.Text = HtmlSanitizer.HtmlEncode(recommendIdErrorMessage);
			}
		}
		else
		{
			var recommendIdErrormessage = CheckRecommendId();
			if (string.IsNullOrEmpty(recommendIdErrormessage) == false)
			{
				lRecommendReportRecommendIdErrorMessages.Text = HtmlSanitizer.HtmlEncode(recommendIdErrormessage);
				return;
			}
		}

		lRecommendReportDateErrorMessages.Text = string.Empty;
		var dateErrorMessage = CheckTerm();
		if (string.IsNullOrEmpty(dateErrorMessage) == false)
		{
			lRecommendReportDateErrorMessages.Text = HtmlSanitizer.HtmlEncode(dateErrorMessage);
			return;
		}

		// ラジオボタン制御
		if (recommendPv.Checked)
		{
			this.GraphDispKbn = KBN_DISP_PV_NUM;

			// グラフx軸「日付」
			_horizonalLabel = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_DATE);

			// グラフy軸「PV数」
			_verticalLabel = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_NUMBER_PAGE_VIEW);

			// 値の単位「回」
			_verticalUnit = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_TIME);
		}
		else if (recommendCv.Checked)
		{
			this.GraphDispKbn = KBN_DISP_CV_NUM;

			// グラフx軸「日付」
			_horizonalLabel = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_DATE);

			// グラフy軸「CV数」
			_verticalLabel = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_NUMBER_CONVERSION);

			// 値の単位「回」
			_verticalUnit = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_TIME);
		}
		else if (recommendCvp.Checked)
		{
			this.GraphDispKbn = KBN_DISP_CV_PER;

			// グラフx軸「日付」
			_horizonalLabel = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_DATE);

			// グラフy軸「CV率」
			_verticalLabel = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_CONVERSION_RATE);

			// 値の単位「%」
			_verticalUnit = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_PERCENT);
		}

		// 集計データ生成
		var recommendData = new RecommendService();
		if (recommendCvp.Checked)
		{
			var pvData = recommendData.GetRecommendReportGraphPv(
				this.DateBgn,
				this.DateEnd,
				this.LoginOperatorShopId,
				tbRecommendId.Text.Trim());

			var cvData = recommendData.GetRecommendReportGraphCv(
				this.DateBgn,
				this.DateEnd,
				this.LoginOperatorShopId,
				tbRecommendId.Text.Trim());

			// CV数データ、循環用
			var dataCount = 0;
			// 開始時間から終了時間まで、日付が一日増える
			for (var bgn = this.DateBgn; bgn <= this.DateEnd; bgn = bgn.AddDays(1))
			{
				var graphValue = string.Empty;

				if ((dataCount < cvData.Length)
					&& (bgn.Year == int.Parse(cvData[dataCount].DateYear))
					&& (bgn.Month == int.Parse(cvData[dataCount].DateMonth))
					&& (bgn.Day == int.Parse(cvData[dataCount].DateDay)))
				{
					var cvNum = cvData[dataCount].DateValue;
					var pvNum = 0;

					// PV数データ、循環用。CV数データと同じ日付のPV数を取得する
					for (var pvCount = 0; pvCount < pvData.Length; pvCount++)
					{
						if ((bgn.Year == int.Parse(pvData[pvCount].DateYear))
							&& (bgn.Month == int.Parse(pvData[pvCount].DateMonth))
							&& (bgn.Day == int.Parse(pvData[pvCount].DateDay)))
						{
							pvNum = pvData[pvCount].DateValue;
						}
					}
					var cvPer = Math.Floor((double)cvNum * 100 / pvNum);
					graphValue = cvPer.ToString();
					dataCount++;
				}
				else
				{
					// CV数がないと、値を0に代入する
					graphValue = "0";
				}

				// グラフ用データ作成
				// "day":"2021/12/31"（例）
				var headerDay = string.Format("{0}/{1}/{2}", bgn.Year, bgn.Month, bgn.Day);
				var tableData = new Hashtable
				{
					{ TABLE_HEADER_DAY, headerDay },
					// "value":"0"（"2021/12/31"の値）
					{ TABLE_DATA_VALUE, graphValue },
				};
				// 集計データ作成
				_allTableData.Add(tableData);
			}
		}
		else
		{
			var dataCount = 0;

			var newData = recommendPv.Checked
				? recommendData.GetRecommendReportGraphPv(
					this.DateBgn,
					this.DateEnd,
					this.LoginOperatorShopId,
					tbRecommendId.Text.Trim())
				: recommendData.GetRecommendReportGraphCv(
					this.DateBgn,
					this.DateEnd,
					this.LoginOperatorShopId,
					tbRecommendId.Text.Trim());

			for (var bgn = this.DateBgn; bgn <= this.DateEnd; bgn = bgn.AddDays(1))
			{
				var graphValue = string.Empty;

				if ((dataCount < newData.Length)
					&& (bgn.Year == int.Parse(newData[dataCount].DateYear))
					&& (bgn.Month == int.Parse(newData[dataCount].DateMonth))
					&& (bgn.Day == int.Parse(newData[dataCount].DateDay)))
				{
					graphValue = newData[dataCount].DateValue.ToString();
					dataCount++;
				}
				else
				{
					graphValue = "0";
				}

				var headerDay = string.Format("{0}/{1}/{2}", bgn.Year, bgn.Month, bgn.Day);
				var tableData = new Hashtable
				{
					{ TABLE_HEADER_DAY, headerDay },
					{ TABLE_DATA_VALUE, graphValue },
				};
				_allTableData.Add(tableData);
			}
		}

		// アクセスレポートグラフ作成
		// 数値表示するかチェック
		var isViewNumber = (rblCheckNumber.SelectedValue == KBN_VIEW_NUMBER);
		// パーセント表示対応か（Y軸最大値：100）
		var isPercent = recommendCvp.Checked;
		var jsonValues = SerializeHelper.SerializeJson(_allTableData);

		var chartType = string.Empty;
		switch (rblChartType.SelectedValue)
		{
			case KBN_CHART_COLUMN:
				chartType = "bar";
				break;

			case KBN_CHART_LINE:
				chartType = "line";
				break;
		}

		// グラフ表示
		CreateFunction(jsonValues, chartType, _horizonalLabel, _verticalLabel, _verticalUnit, isViewNumber, isPercent);

		// レポート表示のデータ設定
		// 日付の表示
		rTableHeader.DataSource = _allTableData;
		rTableHeader.DataBind();
		// 値の表示
		rTableData.DataSource = _allTableData;
		rTableData.DataBind();

		// レコメンドタイトル表示
		lRecommendReportTitle.Text = HtmlSanitizer.HtmlEncode(
			string.Format(
				"{0}　{1}　（{2}：{3}～{4}）",
				this.RecommendName,
				CreateDisplayKbnString(),
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_PERIOD),
				DateTimeUtility.ToStringForManager(this.DateBgn, DateTimeUtility.FormatType.LongDate1Letter),
				DateTimeUtility.ToStringForManager(this.DateEnd, DateTimeUtility.FormatType.LongDate1Letter)));
	}

	/// <summary>
	/// テーブル日付CLASS取得
	/// </summary>
	/// <param name="date">日付</param>
	/// <returns>テーブル日付CLASS</returns>
	protected string CreateTableDayClassName(string date)
	{
		var result = string.Empty;
		switch (DateTime.Parse(date).DayOfWeek)
		{
			case DayOfWeek.Saturday:
				result = "list_item_bg_wk_sat";
				break;

			case DayOfWeek.Sunday:
				result = "list_item_bg_wk_sun";
				break;

			default:
				result = "list_item_bg";
				break;
		}
		return result;
	}

	/// <summary>
	/// 表示区分文字列取得
	/// </summary>
	/// <returns>表示区分文字列</returns>
	private string CreateDisplayKbnString()
	{
		var result = recommendPv.Checked
			? recommendPv.Text.Trim()
			: recommendCv.Checked
				? recommendCv.Text.Trim()
				: recommendCvp.Checked
					? recommendCvp.Text.Trim()
					: string.Empty;

		result = string.Format("{0}({1})", result, _verticalUnit);
		return result;
	}

	/// <summary>
	/// CSVダウンロードリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReportExport_Click(object sender, EventArgs e)
	{
		lRecommendReportDateErrorMessages.Text = string.Empty;
		var dateErrorMessage = CheckTerm();
		var recommendIdErrorMessage = CheckRecommendId();
		if ((string.IsNullOrEmpty(dateErrorMessage) == false) ||
			(string.IsNullOrEmpty(recommendIdErrorMessage) == false))
		{
			lRecommendReportDateErrorMessages.Text = HtmlSanitizer.HtmlEncode(dateErrorMessage);
			lRecommendReportRecommendIdErrorMessages.Text = HtmlSanitizer.HtmlEncode(recommendIdErrorMessage);
			return;
		}

		var titleParams = new List<string>
		{
			// 「レコメンドレポート」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST,
				Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_TITLE),
			string.Format(
				"{0}　{1}　（{2}：{3}～{4}）",
				this.RecommendName,
				CreateDisplayKbnString(),
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST,
					Constants.VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_PERIOD),
				DateTimeUtility.ToStringForManager(this.DateBgn, DateTimeUtility.FormatType.LongDate1Letter),
				DateTimeUtility.ToStringForManager(this.DateEnd, DateTimeUtility.FormatType.LongDate1Letter))
		};

		var records = new StringBuilder();
		records.Append(CreateRecordCsv(titleParams));
		// 一行空き
		titleParams.Clear();
		records.Append(CreateRecordCsv(titleParams));

		// ヘッダ作成
		var headerParams = new List<string>();
		foreach (RepeaterItem ri in rTableHeader.Items)
		{
			headerParams.Add(((Literal)ri.FindControl("lHeader")).Text.Trim());
		}
		records.Append(CreateRecordCsv(headerParams));

		// データ作成
		var dataParams = new List<string>();
		if (rTableData.Items.Count > 0)
		{
			foreach (RepeaterItem ri in rTableData.Items)
			{
				dataParams.Add(((Literal)ri.FindControl("lData")).Text.Trim());
			}
		}
		records.Append(CreateRecordCsv(dataParams));

		var fileName = string.Format(
			"RecommendReportList_{0:yyyyMMdd}-{1:yyyyMMdd}",
			this.DateBgn,
			this.DateEnd);

		// ファイル出力
		OutPutFileCsv(fileName, records.ToString());
	}

	/// <summary>
	/// グラフ作成のFunction呼び出し
	/// </summary>
	/// <param name="values">データ値</param>
	/// <param name="chartType">グラフの種類</param>
	/// <param name="xAxisTitle">x軸のラベルタイトル</param>
	/// <param name="label">x軸の各目盛りのラベル</param>
	/// <param name="valueUnit">データ値の単位</param>
	/// <param name="isViewNumber">数値の表示の可否</param>
	/// <param name="isPercent">パーセント表示対応か</param>
	private void CreateFunction(
		string values,
		string chartType,
		string xAxisTitle,
		string label,
		string valueUnit,
		bool isViewNumber,
		bool isPercent)
	{
		lCreateScript.Text = string.Format(
			"<script>CreateRecommendReportCharts({0}, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', 'report')</script>",
			values,
			chartType,
			xAxisTitle,
			label,
			valueUnit,
			isViewNumber,
			isPercent);
	}

	/// <summary>
	/// 表示クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ShowRecommendReport_Click(object sender, EventArgs e)
	{
		lRecommendReportDateErrorMessages.Text = string.Empty;
		var dateErrorMessage = CheckTerm();
		var recommendIdErrormessage = CheckRecommendId();
		if ((string.IsNullOrEmpty(dateErrorMessage) == false) ||
			(string.IsNullOrEmpty(recommendIdErrormessage) == false))
		{
			lRecommendReportDateErrorMessages.Text = HtmlSanitizer.HtmlEncode(dateErrorMessage);
			lRecommendReportRecommendIdErrorMessages.Text = HtmlSanitizer.HtmlEncode(recommendIdErrormessage);
			return;
		}

		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_RECOMMEND_REPORT)
			.AddParam(Constants.REQUEST_KEY_RECOMMEND_ID, tbRecommendId.Text.Trim())
			.AddParam(Constants.REQUEST_KEY_CURRENT_YEAR, this.DateBgn.Year.ToString())
			.AddParam(Constants.REQUEST_KEY_CURRENT_MONTH, this.DateBgn.Month.ToString().PadLeft(2, '0'))
			.AddParam(Constants.REQUEST_KEY_CURRENT_DAY, this.DateBgn.Day.ToString().PadLeft(2, '0'))
			.AddParam(Constants.REQUEST_KEY_TARGET_YEAR, this.DateEnd.Year.ToString())
			.AddParam(Constants.REQUEST_KEY_TARGET_MONTH, this.DateEnd.Month.ToString().PadLeft(2, '0'))
			.AddParam(Constants.REQUEST_KEY_TARGET_DAY, this.DateEnd.Day.ToString().PadLeft(2, '0'))
			.AddParam(REQUEST_KEY_CHART_TYPE, rblChartType.SelectedValue)
			.AddParam(REQUEST_KEY_DATA_KBN, this.GraphDispKbn);

		var url = urlCreator.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 期間指定エラーチェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private string CheckTerm()
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
	/// レコメンドID入力エラーチェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private string CheckRecommendId()
	{
		lRecommendReportRecommendIdErrorMessages.Text = string.Empty;
		this.RecommendName = string.Empty;
		var result = string.Empty;

		if (string.IsNullOrEmpty(tbRecommendId.Text.Trim()))
		{
			result = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RECOMMEND_REPORT_NO_RECOMMENDID_ERROR);
		}
		else
		{
			var resultRecommendModel = new RecommendService().Get(this.LoginOperatorShopId, tbRecommendId.Text.Trim());

			if (resultRecommendModel == null)
			{
				result = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RECOMMEND_REPORT_RECOMMENDID_NOT_EXIST_ERROR);
				this.RecommendName = string.Empty;
			}
			else
			{
				this.RecommendName = string.Format("【{0}】", resultRecommendModel.RecommendName);
			}
		}

		return result;
	}

	/// <summary>
	/// レコメンドレポート、詳細レポート対象選択URL
	/// </summary>
	/// <returns>URL</returns>
	protected string CreateRecommendReportTableListUrl()
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_RECOMMEND_TABLE_REPORT)
			.CreateUrl();
		return url;
	}

	#region プロパティ
	/// <summary>リクエスト：レコメンドID</summary>
	private string RequestRecommendId
	{
		get { return StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_RECOMMEND_ID]).Trim(); }
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
	/// <summary>レコメンド名(管理用)</summary>
	private string RecommendName { get; set; }
	/// <summary>時間指定、開発時間</summary>
	private DateTime DateBgn { get; set; }
	/// <summary>時間指定、終了時間</summary>
	private DateTime DateEnd { get; set; }
	/// <summary>統計項目フラグ</summary>
	private string GraphDispKbn { get; set; }
	#endregion
}