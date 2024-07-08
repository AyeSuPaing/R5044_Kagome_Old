/*
=========================================================================================================
  Module      : 受注ワークフローシナリオ設定詳細ページ(OrderWorkflowScenarioConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text.RegularExpressions;
using w2.Common.Web;
using w2.Domain.OrderWorkflowScenarioSetting;
using w2.Domain.TaskSchedule;
using w2.Domain.TaskSchedule.Helper;

namespace Form.OrderWorkflowAutoExec
{
	/// <summary>
	/// オーダーワークフローシナリオ確認ページ
	/// </summary>
	public partial class OrderWorkflowScenarioConfirm : OrderWorkflowPage
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
			else
			{
				// スケジュールが取得できれば状態取得
				if (this.ActionNo != -1)
				{
					// ステータス表示
					DisplayTaskStatus();
				}
			}
		}

		/// <summary>
		/// 初期化
		/// </summary>
		private void Initialize()
		{
			switch (this.ActionStatus)
			{
				case Constants.ACTION_STATUS_INSERT:
					ControleComponenstDisplayForInsert();
					break;

				case Constants.ACTION_STATUS_COPY_INSERT:
					ControleComponenstDisplayForInsert();
					break;

				case Constants.ACTION_STATUS_UPDATE:
					ControleComponenstDisplayForUpdate();
					break;

				case Constants.ACTION_STATUS_DETAIL:
					ControleComponenstDisplayForDetail();
					this.ScenarioSetting = new OrderWorkflowScenarioSettingService()
						.GetForDatails(this.RequestScenarioSettingId);
					if (this.ScenarioSetting == null)
					{
						Response.Redirect(
							new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_LIST)
								.CreateUrl());
					}
					this.ScenarioSettingId = this.RequestScenarioSettingId;
					break;
			}
			btnStop.Enabled = false;
			this.ActionNo = -1;
			DisplayScenarioSetting();
		}

		/// <summary>
		/// インサート用のコンポーネント表示操作
		/// </summary>
		private void ControleComponenstDisplayForInsert()
		{
			btnInsertTop.Visible = true;
			btnInsertBottom.Visible = true;
			tblManualExec.Visible = false;
		}

		/// <summary>
		/// アップデート用のコンポーネント表示操作
		/// </summary>
		private void ControleComponenstDisplayForUpdate()
		{
			btnUpdateTop.Visible = true;
			btnUpdateBottom.Visible = true;
			tblManualExec.Visible = false;
		}

		/// <summary>
		/// 詳細表示用のコンポーネント表示操作
		/// </summary>
		private void ControleComponenstDisplayForDetail()
		{
			btnCopyInsertTop.Visible = true;
			btnCopyInsertBottom.Visible = true;
			btnEditTop.Visible = true;
			btnEditBottom.Visible = true;
			btnDeleteTop.Visible = true;
			btnDeleteBottom.Visible = true;
		}

		/// <summary>
		/// シナリオを表示する
		/// </summary>
		private void DisplayScenarioSetting()
		{
			lScenarioName.Text = this.ScenarioSetting.ScenarioName;
			lExecTiming.Text = CreateExecTimingForDisplay(this.ScenarioSetting);
			lValidFlgt.Text = ValueText.GetValueText(
				Constants.VALUETEXT_COMMON_SCHEDULE,
				Constants.VALUETEXT_COMMON_SCHEDULE_VALID_FLG,
				this.ScenarioSetting.ValidFlg);
			rScenario.DataSource = this.ScenarioSetting.Items;
			rScenario.DataBind();
		}

		/// <summary>
		/// スケジュールの文字列を作成
		/// </summary>
		/// <param name="scenarioSetting">受注ワークフローシナリオ設定モデル</param>
		/// <returns>スケジュールの文字列</returns>
		private string CreateExecTimingForDisplay(OrderWorkflowScenarioSettingModel scenarioSetting)
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

			var stringTimeFormat
				= DateTimeUtility.ToStringForManager(stringTime, DateTimeUtility.FormatType.HourMinuteSecond1Letter);
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

			var stringDateFormat
				= DateTimeUtility.ToStringForManager(stringDate, DateTimeUtility.FormatType.ShortDateHourMinute2Letter);
			return HtmlSanitizer.HtmlEncode(stringDateFormat);
		}

		/// <summary>
		/// バックボタンクリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnBack_Click(object sender, EventArgs e)
		{
			switch (this.ActionStatus)
			{
				case Constants.ACTION_STATUS_INSERT:
					Response.Redirect(
						new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_RESISTER)
							.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_INSERT)
							.CreateUrl());
					break;

				case Constants.ACTION_STATUS_COPY_INSERT:
					Response.Redirect(
						new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_RESISTER)
							.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_COPY_INSERT)
							.AddParam(
								Constants.REQUEST_KEY_ORDERWORKFLOWSCENARIOSETTING_ORDERWORKFLOW_SCENARIOSETTING_ID,
								this.ScenarioSetting.ScenarioSettingId)
							.CreateUrl());
					break;

				case Constants.ACTION_STATUS_UPDATE:
					Response.Redirect(
						new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_RESISTER)
							.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE)
							.AddParam(
								Constants.REQUEST_KEY_ORDERWORKFLOWSCENARIOSETTING_ORDERWORKFLOW_SCENARIOSETTING_ID,
								this.ScenarioSetting.ScenarioSettingId)
							.CreateUrl());
					break;

				case Constants.ACTION_STATUS_DETAIL:
					this.ScenarioSetting = null;
					Response.Redirect(
						new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_LIST)
							.CreateUrl());
					break;
			}
		}

		/// <summary>
		/// 登録ボタンクリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnInsert_Click(object sender, EventArgs e)
		{
			new OrderWorkflowScenarioSettingService().Insert(this.ScenarioSetting, this.LoginOperatorShopId);
			if ((this.ScenarioSetting.ValidFlg == Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_VALID_FLG_VALID)
				&& (this.ScenarioSetting.ExecTiming == Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING_SCHEDULE))
			{
				new TaskScheduleService().InsertFirstTaskSchedule(
					this.LoginOperatorDeptId,
					Constants.FLG_TASKSCHEDULE_ACTION_KBN_ORDERWORKFLOW_EXEC_SCENARIO_TO_SCHEDULE,
					this.ScenarioSetting.ScenarioSettingId,
					this.LoginOperatorName,
					new TaskScheduleRule(this.ScenarioSetting));
			}
			this.ScenarioSetting = null;
			var scenarioListUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_LIST)
				.CreateUrl();
			Response.Redirect(scenarioListUrl);
		}

		/// <summary>
		/// コピー登録ボタンクリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnCopyInsert_Click(object sender, EventArgs e)
		{
			var copyInsertUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_RESISTER)
				.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_COPY_INSERT)
				.AddParam(
					Constants.REQUEST_KEY_ORDERWORKFLOWSCENARIOSETTING_ORDERWORKFLOW_SCENARIOSETTING_ID,
					this.RequestScenarioSettingId).CreateUrl();
			Response.Redirect(copyInsertUrl);
		}

		/// <summary>
		/// 更新ボタンクリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <exception cref="Exception"></exception>
		protected void btnUpdate_Click(object sender, EventArgs e)
		{
			new OrderWorkflowScenarioSettingService().Update(
				this.ScenarioSetting,
				this.LoginOperatorDeptId,
				this.LoginOperatorName);

			var scenarioList = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_LIST)
				.CreateUrl();
			Response.Redirect(scenarioList);
		}

		/// <summary>
		/// 編集ボタンクリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnEdit_Click(object sender, EventArgs e)
		{
			var scenarioEditUrl =
				new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_RESISTER)
					.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE)
					.AddParam(
						Constants.REQUEST_KEY_ORDERWORKFLOWSCENARIOSETTING_ORDERWORKFLOW_SCENARIOSETTING_ID,
						this.RequestScenarioSettingId).CreateUrl();
			Response.Redirect(scenarioEditUrl);
		}

		/// <summary>
		/// 削除ボタンクリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnDelete_Click(object sender, EventArgs e)
		{
			new OrderWorkflowScenarioSettingService().Delete(this.ScenarioSetting.ScenarioSettingId);

			// スケジュール削除
			new TaskScheduleService().DeleteMasterId(
				this.LoginOperatorDeptId,
				Constants.FLG_TASKSCHEDULE_ACTION_KBN_ORDERWORKFLOW_EXEC_SCENARIO_TO_SCHEDULE,
				this.ScenarioSetting.ScenarioSettingId);

			this.ScenarioSetting = null;
			var scenarioListUrl =
				new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_LIST)
					.CreateUrl();
			Response.Redirect(scenarioListUrl);
		}

		/// <summary>
		/// 今すぐ実行ボタンクリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnExec_Click(object sender, EventArgs e)
		{
			btnExec.Enabled = false;
			btnStop.Enabled = true;

			lbPrepareStatus.Text = "";
			lbExecuteStatus.Text = "";

			var actionNo = InsertTaskSchedule(
				Constants.FLG_TASKSCHEDULE_ACTION_KBN_ORDERWORKFLOW_EXEC_SCENARIO_TO_MANUAL,
				this.ScenarioSetting.ScenarioSettingId);

			if (actionNo > 0)
			{
				this.ActionNo = actionNo;
				DisplayTaskStatus();
			}
		}

		/// <summary>
		/// タスクスケジュール登録
		/// </summary>
		/// <param name="actionKbn">アクション区分</param>
		/// <param name="actionMasterId">アクションマスターID</param>
		/// <returns>アクションNO</returns>
		private int InsertTaskSchedule(string actionKbn, string actionMasterId)
		{
			var model = new TaskScheduleService()
				.InsertTaskScheduleForExecute(this.LoginOperatorDeptId, actionKbn, actionMasterId, this.LoginOperatorName);

			return (model != null) ? model.ActionNo : -1;
		}

		/// <summary>
		/// タスクステータス表示
		/// </summary>
		protected void DisplayTaskStatus()
		{
			// ステータス表示
			var taskSchedule = new TaskScheduleService().Get(
				this.LoginOperatorShopId,
				Constants.FLG_TASKSCHEDULE_ACTION_KBN_ORDERWORKFLOW_EXEC_SCENARIO_TO_MANUAL,
				this.ScenarioSetting.ScenarioSettingId,
				this.ActionNo);
			if (taskSchedule != null)
			{
				// 準備ステータス
				lbPrepareStatus.Text = HtmlSanitizer.HtmlEncode(ValueText.GetValueText(
					Constants.TABLE_TASKSCHEDULE,
					Constants.FIELD_TASKSCHEDULE_PREPARE_STATUS,
					taskSchedule.PrepareStatus));

				// 実行ステータス
				lbExecuteStatus.Text = HtmlSanitizer.HtmlEncode(ValueText.GetValueText(
					Constants.TABLE_TASKSCHEDULE,
					Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS,
					taskSchedule.ExecuteStatus));

				// 進捗(進捗を見て、進捗の説明文の制御もする)
				var progress = taskSchedule.Progress;
				DisplayControlOfProgressDescription(progress);

				switch (taskSchedule.ExecuteStatus)
				{
					// 終了 or 停止
					case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE:
					case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_STOP:
						// 実行ボタンを有効化
						btnExec.Enabled = true;
						// 停止ボタンを無効化
						btnStop.Enabled = false;
						break;
				}
			}
		}

		/// <summary>
		/// 進捗説明の制御
		/// </summary>
		/// <param name="progress">進捗</param>
		private void DisplayControlOfProgressDescription(string progress)
		{
			if (new Regex(@"^\d+\/\d+\r\n\d+\/\d+").IsMatch(progress))
			{
				progressDescriptionForWorkflowCount.Visible = true;
				progressDescriptionForActionCount.Visible = true;
				lbProgress.Text = HtmlSanitizer.HtmlEncodeChangeToBr(progress);
			}
			else if (new Regex(@"^\d+\/\d+").IsMatch(progress))
			{
				progressDescriptionForWorkflowCount.Visible = true;
				progressDescriptionForActionCount.Visible = false;
				lbProgress.Text = HtmlSanitizer.HtmlEncodeChangeToBr(progress);
			}
			else
			{
				progressDescriptionForWorkflowCount.Visible = false;
				progressDescriptionForActionCount.Visible = false;
				lbProgress.Text = HtmlSanitizer.HtmlEncode(
					ValueText.GetValueText(
						Constants.TABLE_TASKSCHEDULE,
						Constants.FIELD_TASKSCHEDULE_PROGRESS,
						progress));
			}
		}

		/// <summary>
		/// 停止ボタンクリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnStop_Click(object sender, EventArgs e)
		{
			// 停止ボタンを無効化
			btnStop.Enabled = false;

			// タスクスケジュール停止
			new TaskScheduleService().SetTaskScheduleStoppedByPrimarykey(
				this.LoginOperatorShopId,
				Constants.FLG_TASKSCHEDULE_ACTION_KBN_ORDERWORKFLOW_EXEC_SCENARIO_TO_MANUAL,
				this.ScenarioSettingId,
				this.ActionNo,
				this.LoginOperatorName);

			// 実行ボタンを有効化
			btnExec.Enabled = true;
		}

		/// <summary>リクエストのシナリオ設定ID</summary>
		private string RequestScenarioSettingId
		{
			get { return Request[Constants.REQUEST_KEY_ORDERWORKFLOWSCENARIOSETTING_ORDERWORKFLOW_SCENARIOSETTING_ID]; }
		}
		/// <summary>セッションに格納されるシナリオ設定</summary>
		private OrderWorkflowScenarioSettingModel ScenarioSetting
		{
			get { return (OrderWorkflowScenarioSettingModel)Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_WORKFLOW_SCENARIO_SETTING]; }
			set
			{
				Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_WORKFLOW_SCENARIO_SETTING] = value;
			}
		}
		/// <summary>シナリオID</summary>
		private string ScenarioSettingId
		{
			get { return (string)ViewState["ScenarioSettingId"]; }
			set { ViewState["ScenarioSettingId"] = value; }
		}
		// <summary>アクションNO</summary>
		private int ActionNo
		{
			get { return (int)ViewState["ActionNo"]; }
			set { ViewState["ActionNo"] = value; }
		}
	}
}