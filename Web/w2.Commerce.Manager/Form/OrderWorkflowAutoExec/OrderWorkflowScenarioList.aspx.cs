/*
=========================================================================================================
  Module      : 受注ワークフローシナリオ一覧ページ(OrderWorkflowScenarioList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Common.Web;
using w2.Domain.OrderWorkflowScenarioSetting;

namespace Form.OrderWorkflowAutoExec
{
	/// <summary>
	/// 受注ワークフローシナリオ一覧ページ
	/// </summary>
	public partial class OrderWorkflowScenarioList : OrderWorkflowPage
	{
		/// <summary>
		/// ページロード
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				Initialize();
			}
		}

		/// <summary>
		/// 初期化
		/// </summary>
		private void Initialize()
		{
			var scenarioSettings = new OrderWorkflowScenarioSettingService().GetAll();
			if (scenarioSettings.Length == 0)
			{
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}

			rScenario.DataSource = scenarioSettings;
			rScenario.DataBind();
		}

		/// <summary>
		/// 新規登録ボタンクリック
		/// </summary>
		protected void btnInsert_Click(object sender, EventArgs e)
		{
			// Session initialization
			Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_WORKFLOW_SCENARIO_SETTING] = null;

			Response.Redirect(
				new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_RESISTER)
					.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_INSERT)
					.CreateUrl());
		}

		/// <summary>
		/// シナリオ一のアップデート用のURLを作成
		/// </summary>
		/// <param name="scenarioId">シナリオID</param>
		/// <returns>URL</returns>
		protected string CreateScenarioUpdateUrl(string scenarioId)
		{
			var url = WebSanitizer.UrlAttrHtmlEncode(new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_CONFIRM)
				.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL)
				.AddParam(Constants.REQUEST_KEY_ORDERWORKFLOWSCENARIOSETTING_ORDERWORKFLOW_SCENARIOSETTING_ID, scenarioId)
				.CreateUrl());
			return url;
		}

		/// <summary>
		/// 有効フラグを〇か-に変換
		/// </summary>
		/// <param name="validFlg">有効フラグ</param>
		/// <returns>文字列の有効フラグ</returns>
		protected string ConversionValidFlgString(string validFlg)
		{
			var validFlgString = ValueText.GetValueText(
				Constants.VALUETEXT_COMMON_SCHEDULE,
				Constants.VALUETEXT_COMMON_SCHEDULE_VALID_FLG,
				validFlg);
			return validFlgString;
		}

		/// <summary>
		/// スケジュールの文字列を作成
		/// </summary>
		/// <param name="scenarioSetting">受注ワークフローシナリオ設定モデル</param>
		/// <returns>スケジュールの文字列</returns>
		protected string CreateExecTimingForDisplay(OrderWorkflowScenarioSettingModel scenarioSetting)
		{
			var execTiming = "";
			switch (scenarioSetting.ExecTiming)
			{
				case Constants.VALUETEXT_COMMON_SCHEDULE_EXEC_TIMING_MANUAL:		// 手動実行
					execTiming = ValueText.GetValueText(
						Constants.VALUETEXT_COMMON_SCHEDULE,
						Constants.VALUETEXT_COMMON_SCHEDULE_EXEC_TIMING,
						scenarioSetting.ExecTiming);
					break;

				case Constants.VALUETEXT_COMMON_SCHEDULE_EXEC_TIMING_SCHEDULE:		// スケジュール実行
					switch (scenarioSetting.ScheduleKbn)
					{
						case Constants.VALUETEXT_COMMON_SCHEDULE_KBN_DAY:		// 日単位（毎日HH:mm:ssに実行）
							execTiming = string.Format(
								ValueText.GetValueText(
									Constants.VALUETEXT_COMMON_SCHEDULE,
									Constants.VALUETEXT_COMMON_SCHEDULE_DISPLAY_FORMAT,
									Constants.VALUETEXT_COMMON_DISPLAY_FORMAT_DAY),
								CreateStringTimeFormat(scenarioSetting));
							break;

						case Constants.VALUETEXT_COMMON_SCHEDULE_KBN_WEEK:	// 週単位（毎週ddd曜日HH:mm:ssに実行）
							execTiming = string.Format(
								ValueText.GetValueText(
									Constants.VALUETEXT_COMMON_SCHEDULE,
									Constants.VALUETEXT_COMMON_SCHEDULE_DISPLAY_FORMAT,
									Constants.VALUETEXT_COMMON_DISPLAY_FORMAT_WEEK),
								ValueText.GetValueText(
								Constants.VALUETEXT_COMMON_SCHEDULE,
								Constants.VALUETEXT_COMMON_SCHEDULE_DAY_OF_WEEK,
								scenarioSetting.ScheduleDayOfWeek),
								CreateStringTimeFormat(scenarioSetting));
							break;

						case Constants.VALUETEXT_COMMON_SCHEDULE_KBN_MONTH:	// 月単位（毎月dd日HH:mm:ssに実行）
							execTiming = string.Format(
								ValueText.GetValueText(
									Constants.VALUETEXT_COMMON_SCHEDULE,
									Constants.VALUETEXT_COMMON_SCHEDULE_DISPLAY_FORMAT,
									Constants.VALUETEXT_COMMON_DISPLAY_FORMAT_MONTH),
								scenarioSetting.ScheduleDay,
								CreateStringTimeFormat(scenarioSetting));
							break;

						case Constants.VALUETEXT_COMMON_SCHEDULE_KBN_ONCE:	// 一回のみ（一回のみyyyy年MM月dd日HH:mm:ssに実行）
							execTiming = string.Format(
								ValueText.GetValueText(
									Constants.VALUETEXT_COMMON_SCHEDULE,
									Constants.VALUETEXT_COMMON_SCHEDULE_DISPLAY_FORMAT,
									Constants.VALUETEXT_COMMON_DISPLAY_FORMAT_ONCE),
								CreateStringDateFormat(scenarioSetting));
							break;
					}
					break;
			}

			return WebSanitizer.HtmlEncode(execTiming);
		}

		/// <summary>
		/// 文字列の時間を作成
		/// </summary>
		/// <param name="scenarioSetting">受注ワークフローシナリオ設定モデル</param>
		/// <returns>文字列の時間</returns>
		private string CreateStringTimeFormat(OrderWorkflowScenarioSettingModel scenarioSetting)
		{
			var stringTime = string.Format(
				"{0}:{1}:{2}",
				scenarioSetting.ScheduleHour,
				scenarioSetting.ScheduleMinute,
				scenarioSetting.ScheduleSecond);

			var stringTimeFormat = DateTimeUtility.ToStringForManager(stringTime, DateTimeUtility.FormatType.HourMinuteSecond1Letter);

			return HtmlSanitizer.HtmlEncode(stringTimeFormat);
		}

		/// <summary>
		/// 文字列の日時を作成
		/// </summary>
		/// <param name="scenarioSetting">受注ワークフローシナリオ設定モデル</param>
		/// <returns>文字列の日時</returns>
		private string CreateStringDateFormat(OrderWorkflowScenarioSettingModel scenarioSetting)
		{
			var stringDate = string.Format(
				"{0}/{1}/{2} {3}:{4}:{5}",
				scenarioSetting.ScheduleYear,
				scenarioSetting.ScheduleMonth,
				scenarioSetting.ScheduleDay,
				scenarioSetting.ScheduleHour,
				scenarioSetting.ScheduleMinute,
				scenarioSetting.ScheduleSecond);

			var stringDateFormat = DateTimeUtility.ToStringForManager(stringDate, DateTimeUtility.FormatType.ShortDateHourMinute2Letter);

			return HtmlSanitizer.HtmlEncode(stringDateFormat);
		}
	}
}