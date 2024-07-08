/*
 =========================================================================================================
  Module      : 休日情報登録ページ処理(HolidayRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Domain.Holiday;
using Calendar = System.Web.UI.WebControls.Calendar;

public partial class Form_HolidayManagement_HolidayRegister : BasePage
{
	/// <summary> 選択された日付の定義 </summary>
	protected const string SELECTED_DATE = "selected_date";
	/// <summary> カレンダーリピータデータバインド用 </summary>
	protected ArrayList m_calendarList = new ArrayList();

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 指定した言語ロケールIDにより、カルチャーを変換する
		if(Constants.GLOBAL_OPTION_ENABLE && (string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) == false))
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE);
		}

		if (!IsPostBack)
		{
			// 表示コンポーネント初期化
			InitializeComponents();

			// 更新の場合、DBから情報取得
			if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				// 休日情報を画面に表示
				DisplayHolidayFromDB();
			}

			// カレンダー描画
			CreateHolidayCalendar();
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 表示制御
		switch (this.ActionStatus)
		{
			// 新規登録
			case Constants.ACTION_STATUS_INSERT:
				trRegister.Visible = true;
				btnInsertTop.Visible = btnInsertBottom.Visible = true;
				btnDeleteTop.Visible = btnDeleteBottom.Visible = false;
				btnUpdateTop.Visible = btnUpdateBottom.Visible = false;
				ddlYear.Enabled = true;
				break;

			// 更新
			case Constants.ACTION_STATUS_UPDATE:
				trEdit.Visible = true;
				btnInsertTop.Visible = btnInsertBottom.Visible = false;
				btnDeleteTop.Visible = btnDeleteBottom.Visible = true;
				btnUpdateTop.Visible = btnUpdateBottom.Visible = true;
				ddlYear.Enabled = false;
				break;

			default:
				DispIrregularParameterMessage();
				break;
		}

		// 休日設定対象のプルダウン初期化（当年から10年間後まで）
		var thisYear = DateTime.Now.Year;
		ddlYear.Items.AddRange(DateTimeUtility.GetYearListItem(thisYear, thisYear + 10));
		ddlYear.SelectedValue = thisYear.ToString();

		// 毎週の曜日CheckboxListに項目を追加
		cblDayOfWeek.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_HOLIDAY, "day_list"));
	}

	/// <summary>
	/// カレンダー描画
	/// </summary>
	/// <remarks> カレンダーリピータデータソースバイド用を作成 </remarks>
	private void CreateHolidayCalendar()
	{
		// 休日設定対処（年月）を取得
		var dateBegin = DateTime.Parse(ddlYear.SelectedValue + "/01/01");
		var dateEnd = dateBegin.AddMonths(11);

		// カレンダーリピータのデータソース作成
		while (dateBegin <= dateEnd)
		{
			// カレンダー情報を格納
			m_calendarList.Add(dateBegin);

			// 1月単位で追加
			dateBegin = dateBegin.AddMonths(1);
		}

		// データバインド
		rHoliday.DataBind();
	}

	/// <summary>
	/// カレンダー日付が選択されたときに呼び出されるイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void cldHoliday_SelectionChanged(object sender, System.EventArgs e)
	{
		// カレンダー選択情報取得
		var holidays = this.SelectedDates;

		// カレンダー取得
		var calendar = (Calendar)sender;

		// 選択されていない場合、選択追加
		if (holidays.Contains(calendar.SelectedDate) == false)
		{
			holidays.Add(calendar.SelectedDate);
		}
		else
		{
			// 既に選択されている場合、選択解除
			this.SelectedDates.Remove(calendar.SelectedDate);
		}

		// そしてカレンダオブジェクトの選択を解除
		calendar.SelectedDates.Clear();

		// カレンダー選択情報をビューステートに保存
		this.SelectedDates = holidays;
	}

	/// <summary>
	/// カレンダー日付描画毎に呼び出されるイベント 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void cldHoliday_DayRender(object sender, DayRenderEventArgs e)
	{
		// カレンダー取得
		var calendar = (Calendar)sender;

		// 同じ月の場合
		e.Cell.BackColor = Color.FromArgb(0xee, 0xee, 0xee);
		if (calendar.VisibleDate.Month == e.Day.Date.Month)
		{
			// カレンダー選択情報に対象セルの日付が含まれていない場合、戻る
			if (this.SelectedDates.Contains(e.Day.Date) == false) return;

			// 選択されたフォーマットに変更
			e.Cell.BackColor = Color.FromArgb(0xF5, 0xA9, 0xD0);
			e.Cell.Font.Bold = true;
			e.Cell.ForeColor = Color.FromArgb(0x08, 0x64, 0xAA);
			
		}
		// 別の月の場合
		else
		{
			// リンク削除
			var day = ((LiteralControl)e.Cell.Controls[0]).Text;
			e.Cell.Controls.Clear();
			e.Cell.Controls.Add(new LiteralControl(day));
		}
	}

	/// <summary>
	/// 年のプルダウンの選択肢が変更されるイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
	{
		// 登録/更新メッセージ非表示
		divComp.Visible = false;

		// カレンダー再描画
		CreateHolidayCalendar();
	}

	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnToList_Click(object sender, EventArgs e)
	{
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_HOLIDAY_MANAGEMENT_LIST);
	}

	/// <summary>
	/// 登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		// 入力情報検証
		var errorMessages = Validate();
		if (string.IsNullOrEmpty(errorMessages) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 休日登録
		var holidayService = new HolidayService();
		CreateDataInput().ForEach(model => holidayService.Insert(model));

		// 登録メッセージ表示
		divComp.Visible = true;
	}

	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		// 入力情報検証
		var errorMessages = Validate();
		if (string.IsNullOrEmpty(errorMessages) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// DBから削除
		var holidayService = new HolidayService();
		holidayService.Delete(Request[Constants.REQUEST_KEY_HOLIDAY_YEAR]);

		// 休日登録
		CreateDataInput().ForEach(model => holidayService.Insert(model));

		// 登録/更新メッセージ表示
		divComp.Visible = true;
	}

	/// <summary>
	/// DBから削除
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		// DBから削除
		new HolidayService().Delete(Request[Constants.REQUEST_KEY_HOLIDAY_YEAR]);

		// 一覧ページへ遷移
		btnToList_Click(sender, e);
	}

	/// <summary>
	/// カレンダーに反映ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnReflect_Click(object sender, EventArgs e)
	{
		// 選択した曜日に応じて、休日情報に該当曜日を追加
		cblDayOfWeek.Items.Cast<ListItem>().Where(item => item.Selected).ToList()
			.ForEach(item => SetHolidayCalendar((DayOfWeek)int.Parse(item.Value)));

		// 選択しない曜日に応じて、休日情報から該当曜日を削除
		cblDayOfWeek.Items.Cast<ListItem>().Where(item => item.Selected == false).ToList()
			.ForEach(item => RemoveHolidayCalendar((DayOfWeek)int.Parse(item.Value)));
	}

	/// <summary>
	/// 毎週の指定曜日を休日として設定
	/// </summary>
	/// <param name="dayOfWeek">曜日</param>
	private void SetHolidayCalendar(DayOfWeek dayOfWeek)
	{
		// カレンダー選択情報取得
		var holidays = this.SelectedDates;

		// 休日設定期間設定
		var dateBegin = DateTime.Parse(ddlYear.SelectedValue + "/01/01");
		var dateEnd = dateBegin.AddMonths(12).AddDays(-1);

		while (dateBegin <= dateEnd)
		{
			if (dateBegin.DayOfWeek == dayOfWeek)
			{
				if (holidays.Contains(dateBegin) == false) holidays.Add(dateBegin);
				// 1週単位で追加
				dateBegin = dateBegin.AddDays(7);
			}
			else
			{
				// 1日単位で追加
				dateBegin = dateBegin.AddDays(1);
			}
		}

		// カレンダー選択情報をビューステートに保存
		this.SelectedDates = holidays;
	}

	/// <summary>
	/// 毎週の指定曜日を休日リストを削除
	/// </summary>
	/// <param name="dayOfWeek">曜日</param>
	private void RemoveHolidayCalendar(DayOfWeek dayOfWeek)
	{
		// カレンダー選択情報取得
		var holidays = this.SelectedDates;

		// 休日設定期間設定
		var dateBegin = DateTime.Parse(ddlYear.SelectedValue + "/01/01");
		var dateEnd = dateBegin.AddMonths(12).AddDays(-1);

		while (dateBegin <= dateEnd)
		{
			if (dateBegin.DayOfWeek == dayOfWeek)
			{
				if (holidays.Contains(dateBegin)) holidays.Remove(dateBegin);
				// 1週単位で追加
				dateBegin = dateBegin.AddDays(7);
			}
			else
			{
				// 1日単位で追加
				dateBegin = dateBegin.AddDays(1);
			}
		}

		// カレンダー選択情報をビューステートに保存
		this.SelectedDates = holidays;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private new string Validate()
	{
		var year = ddlYear.SelectedValue;
		// カレンダーで休日を選択されるかのチェック
		if (this.SelectedDates.Cast<DateTime>().Any(d => year == d.Year.ToString()) == false)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_HOLIDAY_NOT_SET);
		}
		else if (this.ActionStatus == Constants.ACTION_STATUS_INSERT)
		{
			// DBに当年の休日を登録されるかのチェック
			var holidayModels = new HolidayService().GetByYear(year);
			return (holidayModels.Length > 0)
				? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_HOLIDAY_DUPLICATION).Replace("@@ 1 @@", year)
				: string.Empty;
		}

		return string.Empty;
	}

	/// <summary>
	/// 入力データ作成
	/// </summary>
	/// <returns>休日モデルリスト</returns>
	private List<HolidayModel> CreateDataInput()
	{
		var result = new List<HolidayModel>();
		var year = ddlYear.SelectedValue;

		// カレンダーで選択日付リストを年月単位で休日モデル作成
		for (int monthIndex = 1; monthIndex <= 12; monthIndex++)
		{
			var days = string.Empty;
			if (this.SelectedDates.Cast<DateTime>().Any(d => (year == d.Year.ToString()) && (monthIndex == d.Month)))
			{
				days = string.Join(",",
					this.SelectedDates.Cast<DateTime>()
					.OrderBy(d => d)
					.Where(d => (year == d.Year.ToString()) && (monthIndex == d.Month))
					.Select(d => d.Day.ToString()));
			}

			var input = new HolidayInput
			{
				YearMonth = year + monthIndex.ToString("00"),
				Days = days,
				LastChanged = this.LoginOperatorName,
			};

			result.Add(input.CreateModel());
		}
	
		return result;
	}

	/// <summary>
	/// 不正なパラメータエラーメッセージ表示
	/// </summary>
	private void DispIrregularParameterMessage()
	{
		Session[Constants.SESSION_KEY_ERROR_MSG] =
			WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
	}

	/// <summary>
	/// DBから取得した休日情報を画面に表示
	/// </summary>
	private void DisplayHolidayFromDB()
	{
		// パラメータ年の取得
		var yearParam = Request[Constants.REQUEST_KEY_HOLIDAY_YEAR];
		// パラメータ年の検証
		if (yearParam.Length != 4 ) DispIrregularParameterMessage();

		// 休日情報を取得
		var models = new HolidayService().GetByYear(yearParam);

		// 休日情報がない場合、エラーページへ遷移
		if (models.Length < 1)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = 
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_HOLIDAY_NOT_EXISTS).Replace("@@ 1 @@", yearParam);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 年のプルダウンを設定
		if (ddlYear.Items.FindByValue(yearParam) == null) ddlYear.Items.Add(new ListItem(yearParam, yearParam));
		ddlYear.SelectedValue = yearParam;

		// カレンダーに休日情報を反映のため、データ作成・ビューステートに保存
		var holidays = new ArrayList();
		models.Cast<HolidayModel>().ToList().ForEach(model =>
			holidays.AddRange(model.Holidays
				.Select(d => DateTime.Parse(string.Format("{0}/{1}/{2}", model.Year, model.Month, d)))
				.ToArray()));
		this.SelectedDates = holidays;
	}

	/// <summary> カレンダーで選択された日付リスト </summary>
	protected ArrayList SelectedDates
	{
		get { return (ArrayList)ViewState[SELECTED_DATE] ?? new ArrayList(); }
		set { ViewState[SELECTED_DATE] = value; }
	}
}