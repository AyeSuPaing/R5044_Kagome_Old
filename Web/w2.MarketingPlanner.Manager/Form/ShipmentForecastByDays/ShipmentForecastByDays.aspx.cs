/*
=========================================================================================================
  Module      : 日別出荷予測レポートページ処理(ShipmentForecastByDays.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Services;
using w2.App.Common.Extensions.Currency;
using w2.Common.Helper;
using w2.Common.Util;
using w2.Domain.DailyOrderShipmentForecast;

public partial class Form_ShipmentForecastByDays_ShipmentForecastByDays : BasePage
{
	#region 定数
	protected const string KBN_VIEW_NUMBER = "0";

	protected const string KBN_CHART_BAR = "bar";
	protected const string KBN_CHART_LINE = "line";
	#endregion

	// バッチの実行状態
	private static int s_BatchStatus = -1;

	#region メソッド
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			this.CurrentYear = DateTime.Now.AddDays(-1).Year;
			this.CurrentMonth = DateTime.Now.AddDays(-1).Month;

			// リクエストパラメタ取得 ⇒ 対象年月
			if ((Request[Constants.REQUEST_KEY_CURRENT_YEAR] != null)
				&& (Request[Constants.REQUEST_KEY_CURRENT_MONTH] != null))
			{
				var iYear = 0;
				var iMonth = 0;
				if ((int.TryParse(Request[Constants.REQUEST_KEY_CURRENT_YEAR], out iYear))
					&& (int.TryParse(Request[Constants.REQUEST_KEY_CURRENT_MONTH], out iMonth)))
				{
					this.CurrentYear = iYear;
					this.CurrentMonth = iMonth;
				}
			}
		}

		// 基準カレンダパラメタ設定
		lblCurrentCalendar.Text = CalendarUtility.CreateHtmlYMCalendar(
			this.CurrentYear,
			this.CurrentMonth,
			Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_SHIPMENT_FORECAST,
			string.Empty,
			Constants.REQUEST_KEY_CURRENT_YEAR,
			Constants.REQUEST_KEY_CURRENT_MONTH,
			isShipmentForecastByDaysPage : true,
			totalShippedForEachMonth : GetTotalShippedForEachMonthInYear(this.CurrentYear));
		
		// データ取得＆加工＆表示処理
		GetDataAndDisplay();
	}

	/// <summary>
	/// データの取得、表示処理
	/// </summary>
	protected void GetDataAndDisplay()
	{
		// レポート作成
		CreateReport();

		// グラフ作成
		CreateGraph();

		// 最終更新日設置
		SetLastChangedDate();

		DataBind();
	}

	/// <summary>
	/// レポート作成
	/// </summary>
	private void CreateReport()
	{
		var dailyOrderShipmentForecast = new DailyOrderShipmentForecastService();
		// 現在月のデータの合計を取得する
		var totalCurrentData = dailyOrderShipmentForecast.GetTotalPriceAndShippedForMonth(this.CurrentYear, this.CurrentMonth);
		this.TotalCurrentSalesPrice = (decimal)totalCurrentData[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_PRICE_SUBTOTAL].ToPriceDecimal(DecimalUtility.Format.Round);
		this.TotalCurrentShipped = long.Parse(totalCurrentData[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_COUNT].ToString());
		// 前月のデータを取得する
		var lastMonthYear = this.CurrentMonth == 1 ? (this.CurrentYear - 1) : this.CurrentYear;
		var lastMonth = this.CurrentMonth == 1 ? 12 : (this.CurrentMonth - 1);
		var totalLastMonthData = dailyOrderShipmentForecast.GetTotalPriceAndShippedForMonth(lastMonthYear, lastMonth);
		var totalLastMonthSalesPrice = (decimal)totalLastMonthData[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_PRICE_SUBTOTAL].ToPriceDecimal(DecimalUtility.Format.Round);
		var totalLastMonthShipped = long.Parse(totalLastMonthData[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_COUNT].ToString());

		// 前月との比率
		// 売上金額
		if ((this.TotalCurrentSalesPrice == 0) && (totalLastMonthSalesPrice == 0))
		{
			this.MoMSalesGrowthRate = 0;
		}
		else if ((totalLastMonthSalesPrice == 0) && (this.TotalCurrentSalesPrice != 0))
		{
			this.MoMSalesGrowthRate = 100;
		}
		else
		{
			this.MoMSalesGrowthRate
				= (int)Math.Round((double)(this.TotalCurrentSalesPrice / totalLastMonthSalesPrice * 100));
		}
		// 出荷数量
		if ((this.TotalCurrentShipped == 0) && (totalLastMonthShipped == 0))
		{
			this.MoMShipmentRatio = 0;
		}
		else if ((totalLastMonthShipped == 0) && (this.TotalCurrentShipped != 0))
		{
			this.MoMShipmentRatio = 100;
		}
		else
		{
			this.MoMShipmentRatio
				= (int)Math.Round((double)this.TotalCurrentShipped / totalLastMonthShipped * 100);
		}

		var dailyShipped = new List<long>();
		this.CurrentMonthData = dailyOrderShipmentForecast.GetDailyPriceAndShippmentByYearAndMonth(
			this.CurrentYear,
			this.CurrentMonth,
			DateTimeUtility.GetLastDayOfMonth(
				this.CurrentYear,
				this.CurrentMonth));

		rTableHeader.DataSource = this.CurrentMonthData;
		rTableData.DataSource = this.CurrentMonthData;
	}

	/// <summary>
	/// グラフ作成
	/// </summary>
	private void CreateGraph()
	{
		this.TargetValue = SerializeHelper.SerializeJson(this.CurrentMonthData);
		this.ChartType = (rblChartType.SelectedValue == KBN_CHART_LINE) ? KBN_CHART_LINE : KBN_CHART_BAR;
	}

	/// <summary>
	/// 最終更新日設置
	/// </summary>
	private void SetLastChangedDate()
	{
		this.LastChangedDateTime = new DailyOrderShipmentForecastService().GetLastDataChangedDateForCurrentMonth(
			this.CurrentYear,
			this.CurrentMonth) ?? DateTime.MinValue;
	}

	/// <summary>
	/// 指定年の各月の出荷合計数を取得
	/// </summary>
	/// <param name="year"></param>
	/// <returns>各月の出荷合計数</returns>
	private long[] GetTotalShippedForEachMonthInYear(int year)
	{
		var totalShippedList = new List<long>();

		for (int i = 0; i < 12; i++)
		{
			var totalPriceAndShippedForMonth
				= new DailyOrderShipmentForecastService().GetTotalPriceAndShippedForMonth(year, i + 1);
			var totalShipped = long.Parse(totalPriceAndShippedForMonth[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_COUNT].ToString());
			totalShippedList.Add(totalShipped);
		}

		return totalShippedList.ToArray();
	}

	/// <summary>
	/// テーブル日付CLASS取得
	/// </summary>
	/// <param name="strDateString"></param>
	/// <returns>テーブル日付CLASS</returns>
	protected string CreateTableDayClassName(string strDateString)
	{
		string strResult;
		switch (DateTime.Parse(strDateString).DayOfWeek)
		{
			case DayOfWeek.Saturday:
				strResult = "list_item_bg_wk_sat";
				break;

			case DayOfWeek.Sunday:
				strResult = "list_item_bg_wk_sun";
				break;

			default:
				strResult = "list_item_bg";
				break;
		}

		return strResult;
	}

	/// <summary>
	/// データ更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUpdateData_Click(object sender, EventArgs e)
	{
		// バッチ実行
		ExecuteBatchForDataMigration(Constants.PHYSICALDIRPATH_CREATEREPORT_EXE, "DailyOrderShipmentForecast");
	}

	/// <summary>
	/// バッチを実行する
	/// </summary>
	/// <param name="physicalPath">実行バッチ物理パス</param>
	/// <param name="argument">口論</param>
	/// <returns>実行完了コード</returns>
	public void ExecuteBatchForDataMigration(string physicalPath, string argument)
	{
		var exeProcess = new System.Diagnostics.Process
		{
			StartInfo =
				{
					FileName = physicalPath,
					Arguments = argument,
				}
		};

		exeProcess.Start();
		exeProcess.EnableRaisingEvents = true;
		exeProcess.Exited += new EventHandler(ExeProcess_Exited);
		ResetBatchStatus();
	}

	/// <summary>
	/// バッチ実行完了をキャッチする
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void ExeProcess_Exited(object sender, EventArgs e)
	{
		var exeProcess = (Process)sender;
		if (exeProcess.HasExited
			&& (exeProcess.ExitCode == Constants.BATCH_CREATEREPORT_ERRORCODE_COMPLETED))
		{
			s_BatchStatus = 0;
		}
		else if (exeProcess.HasExited
			&& (exeProcess.ExitCode == Constants.BATCH_CREATEREPORT_ERRORCODE_DUPLICATE_EXECUTION))
		{
			s_BatchStatus = 1;
		}
	}

	/// <summary>
	/// バッチの実行状態を取得
	/// </summary>
	/// <returns>バッチの実行状態</returns>
	[WebMethod]
	public static int GetBatchStatus()
	{
		return s_BatchStatus;
	}

	/// <summary>
	/// バッチの実行状態をリセットする
	/// </summary>
	[WebMethod]
	public static void ResetBatchStatus()
	{
		s_BatchStatus = -1;
	}
	#endregion

	#region プロパティ
	/// <summary>現在の年</summary>
	protected int CurrentYear
	{
		get { return (int)ViewState[Constants.REQUEST_KEY_CURRENT_YEAR]; }
		set { ViewState[Constants.REQUEST_KEY_CURRENT_YEAR] = value; }
	}
	/// <summary>現在の月</summary>
	protected int CurrentMonth
	{
		get { return (int)ViewState[Constants.REQUEST_KEY_CURRENT_MONTH]; }
		set { ViewState[Constants.REQUEST_KEY_CURRENT_MONTH] = value; }
	}
	/// <summary>最終更新時刻</summary>
	protected DateTime LastChangedDateTime { get; private set; }
	/// <summary>現在月のデータ</summary>
	protected List<Hashtable> CurrentMonthData { get; private set; }
	/// <summary>現在月の売上合計</summary>
	protected decimal TotalCurrentSalesPrice { get; private set; }
	/// <summary>現在月の出荷数合計</summary>
	protected long TotalCurrentShipped { get; private set; }
	/// <summary>現在月の売上が前月に対する比</summary>
	protected int MoMSalesGrowthRate { get; private set; }
	/// <summary>現在月の出荷数が前月に対する比</summary>
	protected int MoMShipmentRatio { get; private set; }
	/// <summary>対象のデータ値</summary>
	protected string TargetValue{ get; private set; }
	/// <summary>チャートタイプ</summary>
	protected string ChartType { get; private set; }
	#endregion
}
