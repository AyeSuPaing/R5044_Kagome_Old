/*
=========================================================================================================
  Module      : スケジュール登録ユーザコントロール処理(ScheduleRegisterForm.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Form_Common_ScheduleRegisterForm : UserControl
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
			Initialize();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void Initialize()
	{
		rbExecByManual.Text = ValueText.GetValueText(Constants.VALUETEXT_COMMON_SCHEDULE, Constants.VALUETEXT_COMMON_SCHEDULE_EXEC_TIMING, Constants.VALUETEXT_COMMON_SCHEDULE_EXEC_TIMING_MANUAL);
		rbExecBySchedule.Text = ValueText.GetValueText(Constants.VALUETEXT_COMMON_SCHEDULE, Constants.VALUETEXT_COMMON_SCHEDULE_EXEC_TIMING, Constants.VALUETEXT_COMMON_SCHEDULE_EXEC_TIMING_SCHEDULE);
		rbScheRepeatDay.Text = ValueText.GetValueText(Constants.VALUETEXT_COMMON_SCHEDULE, Constants.VALUETEXT_COMMON_SCHEDULE_KBN, Constants.VALUETEXT_COMMON_SCHEDULE_KBN_DAY);
		rbScheRepeatWeek.Text = ValueText.GetValueText(Constants.VALUETEXT_COMMON_SCHEDULE, Constants.VALUETEXT_COMMON_SCHEDULE_KBN, Constants.VALUETEXT_COMMON_SCHEDULE_KBN_WEEK);
		rbScheRepeatMonth.Text = ValueText.GetValueText(Constants.VALUETEXT_COMMON_SCHEDULE, Constants.VALUETEXT_COMMON_SCHEDULE_KBN, Constants.VALUETEXT_COMMON_SCHEDULE_KBN_MONTH);
		rbScheRepeatOnce.Text = ValueText.GetValueText(Constants.VALUETEXT_COMMON_SCHEDULE, Constants.VALUETEXT_COMMON_SCHEDULE_KBN, Constants.VALUETEXT_COMMON_SCHEDULE_KBN_ONCE);

		foreach (ListItem li in ValueText.GetValueItemList(Constants.VALUETEXT_COMMON_SCHEDULE, Constants.VALUETEXT_COMMON_SCHEDULE_DAY_OF_WEEK))
		{
			rblScheDayOfWeek.Items.Add(li);
		}
		if (rblScheDayOfWeek.Items.Count > 0) rblScheDayOfWeek.Items[0].Selected = true;

		rbExecByManual.Checked = (Constants.VALUETEXT_COMMON_SCHEDULE_EXEC_TIMING_MANUAL == m_strExecKbn);
		rbExecBySchedule.Checked = (Constants.VALUETEXT_COMMON_SCHEDULE_EXEC_TIMING_SCHEDULE == m_strExecKbn);
		rbScheRepeatDay.Checked = (Constants.VALUETEXT_COMMON_SCHEDULE_KBN_DAY == m_strScheKbn);
		rbScheRepeatWeek.Checked = (Constants.VALUETEXT_COMMON_SCHEDULE_KBN_WEEK == m_strScheKbn);
		rbScheRepeatMonth.Checked = (Constants.VALUETEXT_COMMON_SCHEDULE_KBN_MONTH == m_strScheKbn);
		rbScheRepeatOnce.Checked = (Constants.VALUETEXT_COMMON_SCHEDULE_KBN_ONCE == m_strScheKbn);
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
	/// スケジュールの状態を取得
	/// </summary>
	/// <returns></returns>
	public ScheduleConditionResult GetScheduleCondition()
	{
		var scheduleConditionResul = new ScheduleConditionResult(this);
		return scheduleConditionResul;
	}

	/// <summary>プロパティ：実行区分</summary>
	public string ExecKbn
	{
		get {
			if (rbExecByManual.Checked) return Constants.VALUETEXT_COMMON_SCHEDULE_EXEC_TIMING_MANUAL;
			return Constants.VALUETEXT_COMMON_SCHEDULE_EXEC_TIMING_SCHEDULE;
		}
		set { m_strExecKbn = value; }
	}
	private string m_strExecKbn = Constants.VALUETEXT_COMMON_SCHEDULE_EXEC_TIMING_MANUAL;
	/// <summary>プロパティ：スケジュール区分</summary>
	public string ScheKbn
	{
		get {
			if (rbScheRepeatDay.Checked) return Constants.VALUETEXT_COMMON_SCHEDULE_KBN_DAY;
			if (rbScheRepeatWeek.Checked) return Constants.VALUETEXT_COMMON_SCHEDULE_KBN_WEEK;
			if (rbScheRepeatMonth.Checked) return Constants.VALUETEXT_COMMON_SCHEDULE_KBN_MONTH;
			if (rbScheRepeatOnce.Checked) return Constants.VALUETEXT_COMMON_SCHEDULE_KBN_ONCE;
			return Constants.VALUETEXT_COMMON_SCHEDULE_KBN_ONCE;
		}
		set	{ m_strScheKbn = value;	}
	}
	private string m_strScheKbn = Constants.VALUETEXT_COMMON_SCHEDULE_KBN_WEEK;
	/// <summary>プロパティ：曜日</summary>
	public string ScheDayOfWeek
	{
		get { return rblScheDayOfWeek.SelectedValue; }
		set { m_strScheDayOfWeek = value; }
	}
	private string m_strScheDayOfWeek = DayOfWeek.Sunday.ToString();
	/// <summary>プロパティ：年</summary>
	public int ScheYear
	{
		get { return int.Parse(ucScheDateTime.Year); }
		set { m_iScheYear = value; }
	}
	private int m_iScheYear = DateTime.Now.Year;
	/// <summary>プロパティ：月</summary>
	public int ScheMonth
	{
		get { return int.Parse(ucScheDateTime.Month); }
		set { m_iScheMonth = value; }
	}
	private int m_iScheMonth = DateTime.Now.Month;
	/// <summary>プロパティ：日</summary>
	public int ScheDay
	{
		get { return int.Parse(ucScheDateTime.Day); }
		set { m_iScheDay = value; }
	}
	private int m_iScheDay = DateTime.Now.Day;
	/// <summary>プロパティ：時</summary>
	public int ScheHour
	{
		get { return int.Parse(ucScheDateTime.Hour); }
		set { m_iScheHour = value; }
	}
	private int m_iScheHour = 0;
	/// <summary>プロパティ：分</summary>
	public int ScheMinute
	{
		get { return int.Parse(ucScheDateTime.Minute); }
		set { m_iScheMinute = value; }
	}
	private int m_iScheMinute = 0;
	/// <summary>プロパティ：秒</summary>
	public int ScheSecond
	{
		get { return int.Parse(ucScheDateTime.Second); }
		set { m_iScheSecond = value; }
	}
	private int m_iScheSecond = 0;

	/// <summary>
	/// ラジオボタンの選択により見えているパラメーターの値のみを持つ
	/// </summary>
	public class ScheduleConditionResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="schedukeRegisterForm">Form_Common_ScheduleRegisterForm</param>
		public ScheduleConditionResult(Form_Common_ScheduleRegisterForm schedukeRegisterForm)
		{
			SetScheduleCondition(schedukeRegisterForm);
		}

		/// <summary>
		/// スケジュールのコンディションをセット
		/// </summary>
		/// <param name="schedukeRegisterForm"></param>
		private void SetScheduleCondition(Form_Common_ScheduleRegisterForm schedukeRegisterForm)
		{
			InitializePropaty(schedukeRegisterForm);
			if (schedukeRegisterForm.ExecKbn == Constants.VALUETEXT_COMMON_SCHEDULE_EXEC_TIMING_MANUAL) return;
			switch (schedukeRegisterForm.ScheKbn)
			{
				case Constants.VALUETEXT_COMMON_SCHEDULE_KBN_DAY:
					break;
				case Constants.VALUETEXT_COMMON_SCHEDULE_KBN_WEEK:
					this.ScheDayOfWeek = schedukeRegisterForm.ScheDayOfWeek;
					break;
				case Constants.VALUETEXT_COMMON_SCHEDULE_KBN_MONTH:
					this.ScheDay = schedukeRegisterForm.ScheDay.ToString();
					break;
				case Constants.VALUETEXT_COMMON_SCHEDULE_KBN_ONCE:
					this.ScheYear = schedukeRegisterForm.ScheYear.ToString();
					this.ScheMonth = schedukeRegisterForm.ScheMonth.ToString();
					this.ScheDay = schedukeRegisterForm.ScheDay.ToString();
					break;
			}
			this.ScheKbn = schedukeRegisterForm.ScheKbn;
			this.ScheHour = schedukeRegisterForm.ScheHour.ToString();
			this.ScheMinute = schedukeRegisterForm.ScheMinute.ToString();
			this.ScheSecond = schedukeRegisterForm.ScheSecond.ToString();
		}

		/// <summary>
		/// プロパティの初期化
		/// </summary>
		/// <param name="schedukeRegisterForm"></param>
		private void InitializePropaty(Form_Common_ScheduleRegisterForm schedukeRegisterForm)
		{
			this.ExecKbn = schedukeRegisterForm.ExecKbn;
			this.ScheKbn = "";
			this.ScheDayOfWeek = "";
			this.ScheYear = null;
			this.ScheMonth = null;
			this.ScheDay = null;
			this.ScheHour = null;
			this.ScheMinute = null;
			this.ScheSecond = null;
		}

		/// <summary>プロパティ：実行区分</summary>
		public string ExecKbn { get; set; }
		/// <summary>プロパティ：スケジュール区分</summary>
		public string ScheKbn { get; set; }
		/// <summary>プロパティ：曜日</summary>
		public string ScheDayOfWeek { get; set; }
		/// <summary>プロパティ：年</summary>
		public string ScheYear { get; set; }
		/// <summary>プロパティ：月</summary>
		public string ScheMonth { get; set; }
		/// <summary>プロパティ：日</summary>
		public string ScheDay { get; set; }
		/// <summary>プロパティ：時</summary>
		public string ScheHour { get; set; }
		/// <summary>プロパティ：分</summary>
		public string ScheMinute { get; set; }
		/// <summary>プロパティ：秒</summary>
		public string ScheSecond { get; set; }
	}
}
