/*
=========================================================================================================
  Module      : スケジュール登録ユーザコントロール処理(ScheduleRegisterForm.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using w2.App.Common.Util;

public partial class Form_Common_ScheduleRegisterForm : System.Web.UI.UserControl
{
	// PageLoadはWEBページ→ユーザーコントロールの順で実行されるため、
	// WEBページからユーザーコントロールへ値をセットする際には
	// 内部プロパティに値をセットした方がよい
	// (ユーザーコントロールでドロップダウンが作成される前に、WEBページの値をセットする処理が走るため)

	// また、このコントロールではプロパティが少々特殊な形で実装される。
	// 例えば日付ドロップダウンの場合、セットする際はローカル変数にセットされるが、
	// ゲットの場合はドロップダウンから直接ゲットされる
	// ゆえに、ポストバック内でのみゲットが有効になることを忘れてはならない。

	private string m_strExecKbn = Constants.FLG_TARGETLIST_EXEC_TIMING_MANUAL;
	private string m_strScheKbn = Constants.FLG_TARGETLIST_SCHEDULE_KBN_WEEK;
	private string m_strScheDayOfWeek = DayOfWeek.Sunday.ToString();
	private int m_iScheYear = DateTime.Now.Year;
	private int m_iScheMonth = DateTime.Now.Month;
	private int m_iScheDay = DateTime.Now.Day;
	private int m_iScheHour = 0;
	private int m_iScheMinute = 0;
	private int m_iScheSecond = 0;

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
			InitializeComponent();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponent()
	{
		//------------------------------------------------------
		// ラジオボタンセット
		//------------------------------------------------------
		rbExecByManual.Text = ValueText.GetValueText(Constants.TABLE_TARGETLIST, Constants.FIELD_TARGETLIST_EXEC_TIMING, Constants.FLG_TARGETLIST_EXEC_TIMING_MANUAL);
		rbExecBySchedule.Text = ValueText.GetValueText(Constants.TABLE_TARGETLIST, Constants.FIELD_TARGETLIST_EXEC_TIMING, Constants.FLG_TARGETLIST_EXEC_TIMING_SCHEDULE);

		rbScheRepeatDay.Text = ValueText.GetValueText(Constants.TABLE_TARGETLIST, Constants.FIELD_TARGETLIST_SCHEDULE_KBN, Constants.FLG_TARGETLIST_SCHEDULE_KBN_DAY);
		rbScheRepeatWeek.Text = ValueText.GetValueText(Constants.TABLE_TARGETLIST, Constants.FIELD_TARGETLIST_SCHEDULE_KBN, Constants.FLG_TARGETLIST_SCHEDULE_KBN_WEEK);
		rbScheRepeatMonth.Text = ValueText.GetValueText(Constants.TABLE_TARGETLIST, Constants.FIELD_TARGETLIST_SCHEDULE_KBN, Constants.FLG_TARGETLIST_SCHEDULE_KBN_MONTH);
		rbScheRepeatOnce.Text = ValueText.GetValueText(Constants.TABLE_TARGETLIST, Constants.FIELD_TARGETLIST_SCHEDULE_KBN, Constants.FLG_TARGETLIST_SCHEDULE_KBN_ONCE);

		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_TARGETLIST, Constants.FIELD_TARGETLIST_SCHEDULE_DAY_OF_WEEK))
		{
			rblScheDayOfWeek.Items.Add(li);
		}
		if (rblScheDayOfWeek.Items.Count > 0)
		{
			rblScheDayOfWeek.Items[0].Selected = true;
		}

		//------------------------------------------------------
		// 保留中の値セット
		//------------------------------------------------------
		rbExecByManual.Checked = (Constants.FLG_TARGETLIST_EXEC_TIMING_MANUAL == m_strExecKbn);
		rbExecBySchedule.Checked = (Constants.FLG_TARGETLIST_EXEC_TIMING_SCHEDULE == m_strExecKbn);
		rbScheRepeatDay.Checked = (Constants.FLG_TARGETLIST_SCHEDULE_KBN_DAY == m_strScheKbn);
		rbScheRepeatWeek.Checked = (Constants.FLG_TARGETLIST_SCHEDULE_KBN_WEEK == m_strScheKbn);
		rbScheRepeatMonth.Checked = (Constants.FLG_TARGETLIST_SCHEDULE_KBN_MONTH == m_strScheKbn);
		rbScheRepeatOnce.Checked = (Constants.FLG_TARGETLIST_SCHEDULE_KBN_ONCE == m_strScheKbn);
		foreach (ListItem li in rblScheDayOfWeek.Items)
		{
			li.Selected = (li.Value == m_strScheDayOfWeek);
		}
		ucScheDateTime.SetDate(
			m_iScheYear,
			m_iScheMonth,
			m_iScheDay,
			m_iScheHour,
			m_iScheMinute,
			m_iScheSecond);
	}

	/// <summary>
	/// 入力チェック
	/// </summary>
	/// <param name="htInput"></param>
	/// <returns>エラーメッセージ</returns>
	public string Validate(Hashtable htInput)
	{
		// 抽出タイミング情報(初期化)
		htInput.Add(Constants.FIELD_TARGETLIST_EXEC_TIMING, "");
		htInput.Add(Constants.FIELD_TARGETLIST_SCHEDULE_KBN, "");

		htInput.Add(Constants.FIELD_TARGETLIST_SCHEDULE_YEAR, null);
		htInput.Add(Constants.FIELD_TARGETLIST_SCHEDULE_MONTH, null);
		htInput.Add(Constants.FIELD_TARGETLIST_SCHEDULE_DAY, null);
		htInput.Add(Constants.FIELD_TARGETLIST_SCHEDULE_DAY_OF_WEEK, "");
		htInput.Add(Constants.FIELD_TARGETLIST_SCHEDULE_HOUR, null);
		htInput.Add(Constants.FIELD_TARGETLIST_SCHEDULE_MINUTE, null);
		htInput.Add(Constants.FIELD_TARGETLIST_SCHEDULE_SECOND, null);

		// 抽出タイミング情報
		switch (this.ExecKbn)
		{
			case Constants.FLG_TARGETLIST_EXEC_TIMING_MANUAL:
				htInput[Constants.FIELD_TARGETLIST_EXEC_TIMING] = this.ExecKbn;
				htInput[Constants.FIELD_TARGETLIST_SCHEDULE_KBN] = "";
				break;

			case Constants.FLG_TARGETLIST_EXEC_TIMING_SCHEDULE:
				htInput[Constants.FIELD_TARGETLIST_EXEC_TIMING] = Constants.FLG_TARGETLIST_EXEC_TIMING_SCHEDULE;
				htInput[Constants.FIELD_TARGETLIST_SCHEDULE_KBN] = this.ScheKbn;

				switch (this.ScheKbn)
				{
					case Constants.FLG_TARGETLIST_SCHEDULE_KBN_DAY:
						break;

					case Constants.FLG_TARGETLIST_SCHEDULE_KBN_WEEK:
						htInput[Constants.FIELD_TARGETLIST_SCHEDULE_DAY_OF_WEEK] = this.ScheDayOfWeek;
						break;

					case Constants.FLG_TARGETLIST_SCHEDULE_KBN_MONTH:
						htInput[Constants.FIELD_TARGETLIST_SCHEDULE_DAY] = this.ScheDay;
						break;

					case Constants.FLG_TARGETLIST_SCHEDULE_KBN_ONCE:
						htInput[Constants.FIELD_TARGETLIST_SCHEDULE_YEAR] = this.ScheYear;
						htInput[Constants.FIELD_TARGETLIST_SCHEDULE_MONTH] = this.ScheMonth;
						htInput[Constants.FIELD_TARGETLIST_SCHEDULE_DAY] = this.ScheDay;

						// 日付チェック用
						htInput["schedule_date"] = ucScheDateTime.DateString;
						break;
				}
				htInput[Constants.FIELD_TARGETLIST_SCHEDULE_HOUR] = this.ScheHour;
				htInput[Constants.FIELD_TARGETLIST_SCHEDULE_MINUTE] = this.ScheMinute;
				htInput[Constants.FIELD_TARGETLIST_SCHEDULE_SECOND] = this.ScheSecond;
				break;
		}

		// 入力チェック
		return Validator.Validate("TargetListSchedule", htInput);
	}

	/// <summary>プロパティ：実行区分</summary>
	public string ExecKbn
	{
		get {
			if (rbExecByManual.Checked) return Constants.FLG_TARGETLIST_EXEC_TIMING_MANUAL;
			return Constants.FLG_TARGETLIST_EXEC_TIMING_SCHEDULE;
		}
		set { m_strExecKbn = value; }
	}

	/// <summary>プロパティ：スケジュール区分</summary>
	public string ScheKbn
	{
		get {
			if (rbScheRepeatDay.Checked) return Constants.FLG_TARGETLIST_SCHEDULE_KBN_DAY;
			if (rbScheRepeatWeek.Checked) return Constants.FLG_TARGETLIST_SCHEDULE_KBN_WEEK;
			if (rbScheRepeatMonth.Checked) return Constants.FLG_TARGETLIST_SCHEDULE_KBN_MONTH;
			//if (rbScheRepeatOnce.Checked) return Constants.FLG_TARGETLIST_SCHEDULE_KBN_ONCE;
			return Constants.FLG_TARGETLIST_SCHEDULE_KBN_ONCE;
		}
		set	{ m_strScheKbn = value;	}
	}
	/// <summary>プロパティ：曜日</summary>
	public string ScheDayOfWeek
	{
		get { return rblScheDayOfWeek.SelectedValue; }
		set { m_strScheDayOfWeek = value; }
	}
	/// <summary>プロパティ：年</summary>
	public int ScheYear
	{
		get { return int.Parse(ucScheDateTime.Year); }
		set { m_iScheYear = value; }
	}
	/// <summary>プロパティ：月</summary>
	public int ScheMonth
	{
		get { return int.Parse(ucScheDateTime.Month); }
		set { m_iScheMonth = value; }
	}
	/// <summary>プロパティ：日</summary>
	public int ScheDay
	{
		get { return int.Parse(ucScheDateTime.Day); }
		set { m_iScheDay = value; }
	}
	/// <summary>プロパティ：時</summary>
	public int ScheHour
	{
		get { return int.Parse(ucScheDateTime.Hour); }
		set { m_iScheHour = value; }
	}
	/// <summary>プロパティ：分</summary>
	public int ScheMinute
	{
		get { return int.Parse(ucScheDateTime.Minute); }
		set { m_iScheMinute = value; }
	}
	/// <summary>プロパティ：秒</summary>
	public int ScheSecond
	{
		get { return int.Parse(ucScheDateTime.Second); }
		set { m_iScheSecond = value; }
	}
}
