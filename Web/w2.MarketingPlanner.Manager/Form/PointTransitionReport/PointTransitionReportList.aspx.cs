/*
=========================================================================================================
  Module      : ポイント推移レポート一覧ページ処理(PointTransitionReportList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Web.UI.WebControls;
using w2.Domain.Point;
using w2.Domain.Point.Helper;

public partial class Form_PointTransitionReport_PointTransitionReportList : BasePage
{
	protected int m_iCurrentYear = DateTime.Now.Year;
	protected int m_iCurrentMonth = DateTime.Now.Month;

	const string REQUEST_KEY_DATE_TYPE				= "dtype";

	protected const string KBN_DISP_MONTHLY_REPORT	= "1";
	protected const string KBN_DISP_DAILY_REPORT	= "0";

	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		// 年月取得
		try
		{
			if (ViewState[Constants.REQUEST_KEY_CURRENT_YEAR] != null)
			{
				m_iCurrentYear = (int)ViewState[Constants.REQUEST_KEY_CURRENT_YEAR];
				m_iCurrentMonth = (int)ViewState[Constants.REQUEST_KEY_CURRENT_MONTH];
			}
			else
			{
				m_iCurrentYear = int.Parse(Request[Constants.REQUEST_KEY_CURRENT_YEAR]);
				m_iCurrentMonth = int.Parse(Request[Constants.REQUEST_KEY_CURRENT_MONTH]);
			}
		}
		catch
		{
			m_iCurrentYear = DateTime.Now.Year;
			m_iCurrentMonth = DateTime.Now.Month;
		}
		// 年月はビューステートへ格納
		ViewState[Constants.REQUEST_KEY_CURRENT_YEAR] = m_iCurrentYear;
		ViewState[Constants.REQUEST_KEY_CURRENT_MONTH] = m_iCurrentMonth;

		if (!IsPostBack)
		{
			// ラジオボタン選択状態取得・設定
			try
			{
				int iChecked = int.Parse(Request[REQUEST_KEY_DATE_TYPE]);
				foreach (ListItem li in rblReportType.Items)
				{
					li.Selected = (li.Value == iChecked.ToString());
				}
			}
			catch
			{
			}
		}
		else
		{
			divDaily.Visible = false;
			divMonthly.Visible = false;
		}

		//------------------------------------------------------
		// コンポーネント初期化
		//------------------------------------------------------
		Initialize();

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		rDataList.DataSource = CreateReportData();
		rDataList.DataBind();
	}
	#endregion

	#region -Initialize コンポーネント初期化
	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void Initialize()
	{
		//------------------------------------------------------
		// カレンダ設定
		//------------------------------------------------------
		string strParamForCurrent = REQUEST_KEY_DATE_TYPE + "=" + rblReportType.SelectedValue;
		// 基準カレンダ設定
		lblCurrentCalendar.Text = CalendarUtility.CreateHtmlYMCalendar(m_iCurrentYear, m_iCurrentMonth, Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINT_TRANSITION_REPORT_LIST, strParamForCurrent, Constants.REQUEST_KEY_CURRENT_YEAR, Constants.REQUEST_KEY_CURRENT_MONTH);

		// 表示情報設定
		if (rblReportType.SelectedValue == KBN_DISP_DAILY_REPORT)
		{
			divDaily.Visible = true;

		}
		else if (rblReportType.SelectedValue == KBN_DISP_MONTHLY_REPORT)
		{
			divMonthly.Visible = true;
		}
	}
	#endregion

	#region -CreateReportData レポートデータ生成
	/// <summary>
	/// レポートデータ生成
	/// </summary>
	/// <returns>レポートデータ</returns>
	private WrappedReportResult[] CreateReportData()
	{
		var sv = new PointService();
		// レポート検索条件
		var cond = new PointTransitionReportCondition();
		cond.DeptId = this.LoginOperatorDeptId;
		cond.Year = m_iCurrentYear.ToString();
		cond.Month = m_iCurrentMonth.ToString();
		cond.ReportType = rblReportType.SelectedValue == KBN_DISP_DAILY_REPORT
			? ReportType.Day
			: ReportType.Month;

		// レポートデータを取得して表示様のクラスでラップ
		var res = sv.PointTransitionReportSearch(cond).Select(i => new WrappedReportResult(i)).ToArray();

		return res;
	}
	#endregion

	#region プロパティ
	/// <summary>選択された月</summary>
	protected string CurrentMonth
	{
		get
		{
			return DateTimeUtility.ToStringForManager(
				new DateTime(m_iCurrentYear, m_iCurrentMonth, 1),
				DateTimeUtility.FormatType.LongYearMonth);
		}
	}
	/// <summary>選択された年</summary>
	protected string CurrentYear
	{
		get { return m_iCurrentYear.ToString(); }
	}
	#endregion

	#region レポートのラッパークラス
	/// <summary>
	/// レポートのラッパークラス
	/// </summary>
	[Serializable]
	protected class WrappedReportResult : PointTransitionReportResult 
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="result">レポート結果</param>
		public WrappedReportResult(PointTransitionReportResult result)
			: base(result.DataSource,result.ReportType)
		{
		}

		// 表示用にプロパティ加工
		
		/// <summary>表示用日付</summary>
		public string ViewDate
		{
			get
			{
				return (this.ReportType == ReportType.Day)
					? StringUtility.ToEmpty(this.TargetDay) + (string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE)
						? ReplaceTag("@@DispText.schedule_unit.Day@@")
						: string.Empty)
					: StringUtility.ToEmpty(this.TargetMonth) + (string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE)
						? ReplaceTag("@@DispText.schedule_unit.Month@@")
						: string.Empty);
			}
		}

		/// <summary>表示用発行ポイント数</summary>
		public string ViewAddPoint
		{
			get { return DisplayPoint(AddPoint); }
		}

		/// <summary>表示用発行ポイント人数</summary>
		public string ViewAddPointCount
		{
			get { return DisplayPeople(AddPointCount); }
		}

		/// <summary>表示用利用ポイント数</summary>
		public string ViewUsePoint
		{
			get { return DisplayPoint(UsePoint); }
		}

		/// <summary>表示用利用ポイント人数</summary>
		public string ViewUsePointCount
		{
			get { return DisplayPeople(UsePointCount); }
		}

		/// <summary>表示用消滅ポイント数</summary>
		public string ViewExpPoint
		{
			get { return DisplayPoint(ExpPoint); }
		}

		/// <summary>表示用消滅ポイント人数</summary>
		public string ViewExpPointCount
		{
			get { return DisplayPeople(ExpPointCount); }
		}

		/// <summary>表示用ポイント小計</summary>
		public string ViewSubtotalPoint
		{
			get { return DisplayPoint(SubtotalPoint); }
		}

		/// <summary>表示用未回収ポイント</summary>
		public string ViewUnusedPoint
		{
			get { return DisplayPoint(UnusedPoint); }
		}
		
		/// <summary>
		/// ポイント数表示の処理
		/// </summary>
		/// <param name="val">元ネタ</param>
		/// <returns>表示するポイント数</returns>
		private string DisplayPoint(decimal? val)
		{
			if (val.HasValue == false){return "－";}

			if (val == 0){return string.Format("(±){0}pt", StringUtility.ToNumeric(val));}

			if (val < 0){return string.Format("(-){0}pt", StringUtility.ToNumeric(val * -1));}

			if (val > 0){return string.Format("(+){0}pt", StringUtility.ToNumeric(val));}

			return "－";
		}

		/// <summary>
		/// 件数表示の処理
		/// </summary>
		/// <param name="val">元ネタ</param>
		/// <returns>表示する件数</returns>
		private string DisplayPeople(decimal? val)
		{
			if (val.HasValue == false) { return "－"; }

			var textSymbolPeople = ReplaceTag("@@DispText.common_message.unit_of_people@@");
			if (val == 0)
			{
				return string.Format(
					"(±)" + textSymbolPeople,
					StringUtility.ToNumeric(val));
			}

			if (val < 0)
			{
				return string.Format(
					"(-)" + textSymbolPeople,
					StringUtility.ToNumeric(val * -1));
			}

			if (val > 0)
			{
				return string.Format(
					"(+)" + textSymbolPeople,
					StringUtility.ToNumeric(val));
			}

			return "－";
		}
	}
	#endregion
}
