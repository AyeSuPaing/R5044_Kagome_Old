/*
=========================================================================================================
  Module      : 受注ワークフロー実行履歴ページ(OrderWorkflowExecHistory.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Web.UI.WebControls;
using w2.Common.Web;
using w2.Domain.OrderWorkflowExecHistory;
using w2.Domain.OrderWorkflowExecHistory.Helper;
using w2.Domain.OrderWorkflowScenarioSetting;
using w2.Domain.OrderWorkflowSetting;

namespace Form.OrderWorkflowExecHistory
{
	/// <summary>
	/// 受注ワークフロー実行履歴一覧ページ
	/// </summary>
	public partial class OrderWorkflowExecHistoryList : OrderWorkflowPage
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
				// 定期台帳オプションがFALSEなら検索条件にワークフロー種別を表示しない
				if (Constants.FIXEDPURCHASE_OPTION_ENABLED == false)
				{
					tdWorkdlowTypeCheckBoxListTitle.Visible = false;
					tdWorkdlowTypeCheckBoxList.Visible = false;
				}
				InitSearchComponents();
				var searchCondition = GetSearchCondition();
				DisplaySearchCondition(searchCondition);
				DisplayHistoryList(searchCondition);
				CreatePagination(searchCondition);
			}
		}

		/// <summary>
		/// 検索コンポーネントの初期化
		/// </summary>
		private void InitSearchComponents()
		{
			InitDoropDownListForSearchBox();
			InitCheckBoxForSearchBox();
		}

		/// <summary>
		/// 検索に使うドロップダウンリストを初期化
		/// </summary>
		private void InitDoropDownListForSearchBox()
		{
			var orderWorkflowSettings = new OrderWorkflowSettingService().GetAll();
			ddlWorkflowList.Items.Add("");
			ddlWorkflowList.Items.AddRange(
				orderWorkflowSettings.Select(
					orderWorkflowSetting =>
					{
						var listItem = new ListItem(
							orderWorkflowSetting.WorkflowName,
							string.Format("{0}:{1}", orderWorkflowSetting.WorkflowKbn, orderWorkflowSetting.WorkflowNo));
						return listItem;
					}).ToArray());

			var scenarioSettings = new OrderWorkflowScenarioSettingService().GetAll();
			ddlScenarioList.Items.Add("");
			ddlScenarioList.Items.AddRange(
				scenarioSettings.Select(
					scenarioSetting =>
					{
						var listItem = new ListItem(scenarioSetting.ScenarioName, scenarioSetting.ScenarioSettingId);
						return listItem;
					}).ToArray());
		}

		/// <summary>
		/// 検索に使うチェックボックスを作成
		/// </summary>
		private void InitCheckBoxForSearchBox()
		{
			cblExecStatus.Items.AddRange(
				ValueText.GetValueItemArray(
					Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
					Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS));

			if (Constants.ORDERWORKFLOW_AUTOEXEC_OPTION_ENABLE == false)
			{
				cblExecStatus.Items.Remove(cblExecStatus.Items.FindByText(ReplaceTag("@@DispText.common_message.execstatus_stopped@@")));
				cblExecStatus.Items.Remove(cblExecStatus.Items.FindByText(ReplaceTag("@@DispText.common_message.execstatus_pending@@")));
			}

			cblExecPlace.Items.AddRange(
				ValueText.GetValueItemArray(
					Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
					Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE));

			cblExecTiming.Items.AddRange(
				ValueText.GetValueItemArray(
					Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
					Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING));

			cblWorkflowType.Items.AddRange(
				ValueText.GetValueItemArray(
					Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
					Constants.FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE));
		}

		/// <summary>
		/// 検索の条件を表示
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		private void DisplaySearchCondition(OrderWorkflowExecHistoryListSearchCondition searchCondition)
		{
			// ドロップボックスの選択をする
			foreach (ListItem item in ddlWorkflowList.Items)
			{
				if (string.IsNullOrEmpty(item.Value)) continue;
				var listItemValueSplit = item.Value.Split(':');
				var workflowKbn = listItemValueSplit[0];
				var workflowNo = listItemValueSplit[1];
				if ((searchCondition.WorkflowKbn == workflowKbn) && (searchCondition.WorkflowNo.ToString() == workflowNo))
				{
					item.Selected = true;
				}
			}
			foreach (ListItem item in ddlScenarioList.Items)
			{
				if (searchCondition.ScenarioSettingId == item.Value) item.Selected = true;
			}

			// チェックボックスの選択をする
			foreach (ListItem item in cblExecStatus.Items)
			{
				if (searchCondition.ExecStatusSuccess == item.Value) item.Selected = true;
				if (searchCondition.ExecStatusError == item.Value) item.Selected = true;
				if (searchCondition.ExecStatusStopped == item.Value) item.Selected = true;
				if (searchCondition.ExecStatusRunning == item.Value) item.Selected = true;
				if (searchCondition.ExecStatusHold == item.Value) item.Selected = true;
			}
			foreach (ListItem item in cblExecPlace.Items)
			{
				if (searchCondition.ExecPlaceWorkflow == item.Value) item.Selected = true;
				if (searchCondition.ExecPlaceScenario == item.Value) item.Selected = true;
			}
			foreach (ListItem item in cblExecTiming.Items)
			{
				if (searchCondition.ExecTimingManual == item.Value) item.Selected = true;
				if (searchCondition.ExecTimingScedule == item.Value) item.Selected = true;
			}
			foreach (ListItem item in cblWorkflowType.Items)
			{
				if (searchCondition.WorkflowTypeOrderWorkflow == item.Value) item.Selected = true;
				if (searchCondition.WorkflowTypeFixedPurchaseWorkflow == item.Value) item.Selected = true;
			}

			DisplayDateInputForSearchBox();
		}

		/// <summary>
		/// 実行日時の初期化
		/// </summary>
		private void DisplayDateInputForSearchBox()
		{
			if (HasSearchConditionRequestParams() == false)
			{
				ucSearchDatePeriod.SetPeriodDateToday();
			}
			else
			{
				var searchDatePeriodFrom = string.Format("{0}{1}",
					StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_DATE_FROM])
						.Replace("/", string.Empty),
					StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_TIME_FROM])
						.Replace(":", string.Empty));

				var searchDatePeriodTo = string.Format("{0}{1}",
					StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_DATE_TO])
						.Replace("/", string.Empty),
					StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_TIME_TO])
						.Replace(":", string.Empty));

				ucSearchDatePeriod.SetPeriodDate(searchDatePeriodFrom, searchDatePeriodTo);
			}
		}

		/// <summary>
		/// 履歴のリストを表示
		/// </summary>
		/// <param name="searchCondition">検索条件</param>
		private void DisplayHistoryList(OrderWorkflowExecHistoryListSearchCondition searchCondition)
		{
			var orderWorkflowExecHistories = new OrderWorkflowExecHistoryService().Search(searchCondition);

			if (orderWorkflowExecHistories.Length == 0)
			{
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}
			rHistoryList.DataSource = orderWorkflowExecHistories;
			rHistoryList.DataBind();
		}

		/// <summary>
		/// 検索時に必要な検索条件を取得
		/// </summary>
		/// <returns>検索条件</returns>
		private OrderWorkflowExecHistoryListSearchCondition GetSearchCondition()
		{
			if (HasSearchConditionRequestParams())
			{
				return new OrderWorkflowExecHistoryListSearchCondition
				{
					ShopId = StringUtility.ToEmpty(this.RequestSearchShopId),
					WorkflowKbn = StringUtility.ToEmpty(this.RequestSearchWorkflowKbn),
					WorkflowNo =
						string.IsNullOrEmpty(this.RequestSearchWorkflowNo) == false
							? int.Parse(StringUtility.ToEmpty(this.RequestSearchWorkflowNo))
							: (int?)null,
					ScenarioSettingId = StringUtility.ToEmpty(this.RequestSearchScenarioSettingId),
					ExecStatus = StringUtility.ToEmpty(this.RequestSearchExecStatus),
					ExecStatusSuccess = StringUtility.ToEmpty(this.RequestSearchExecStatus)
						.Contains(Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_OK)
							? Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_OK
							: "",
					ExecStatusError = StringUtility.ToEmpty(this.RequestSearchExecStatus)
						.Contains(Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_NG)
							? Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_NG
							: "",
					ExecStatusStopped = StringUtility.ToEmpty(this.RequestSearchExecStatus)
						.Contains(Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_STOPPED)
							? Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_STOPPED
							: "",
					ExecStatusRunning = StringUtility.ToEmpty(this.RequestSearchExecStatus)
						.Contains(Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_RUNNING)
							? Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_RUNNING
							: "",
					ExecStatusHold = StringUtility.ToEmpty(this.RequestSearchExecStatus)
						.Contains(Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_HOLD)
							? Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_HOLD
							: "",
					ExecPlace = StringUtility.ToEmpty(this.RequestSearchExecPlace),
					ExecPlaceWorkflow = StringUtility.ToEmpty(this.RequestSearchExecPlace)
						.Contains(Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE_WORKFLOW)
							? Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE_WORKFLOW
							: "",
					ExecPlaceScenario = StringUtility.ToEmpty(this.RequestSearchExecPlace)
						.Contains(Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE_SCENARIO)
							? Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE_SCENARIO
							: "",
					ExecTiming = StringUtility.ToEmpty(this.RequestSearchExecTiming),
					ExecTimingManual = StringUtility.ToEmpty(this.RequestSearchExecTiming)
						.Contains(Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING_MANUAL)
							? Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING_MANUAL
							: "",
					ExecTimingScedule = StringUtility.ToEmpty(this.RequestSearchExecTiming)
						.Contains(Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING_SCHEDULE)
							? Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING_SCHEDULE
							: "",
					WorkflowType = StringUtility.ToEmpty(this.RequestSearchWorkflowType),
					WorkflowTypeOrderWorkflow = StringUtility.ToEmpty(this.RequestSearchWorkflowType)
						.Contains(Constants.FLG_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE_ORDER_WORKFLOW)
							? Constants.FLG_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE_ORDER_WORKFLOW
							: "",
					WorkflowTypeFixedPurchaseWorkflow = StringUtility.ToEmpty(this.RequestSearchWorkflowType)
						.Contains(Constants.FLG_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE_FIXED_PURCHASE_WORKFLOW)
							? Constants.FLG_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE_FIXED_PURCHASE_WORKFLOW
							: "",
					DateFrom = (string.IsNullOrEmpty(this.RequestSearchDateFrom) && string.IsNullOrEmpty(this.RequestSearchTimeFrom))
						? (DateTime?)null
						: DateTime.Parse(string.Format("{0} {1}",
							this.RequestSearchDateFrom,
							this.RequestSearchTimeFrom)),
					DateTo = (string.IsNullOrEmpty(this.RequestSearchDateTo) && string.IsNullOrEmpty(this.RequestSearchTimeTo))
						? (DateTime?)null
						: DateTime.Parse(string.Format("{0} {1}",
							this.RequestSearchDateTo,
							this.RequestSearchTimeTo)).AddSeconds(1),
					BgnRowNum = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNo - 1) + 1,
					EndRowNum = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNo,
				};
			}
			return new OrderWorkflowExecHistoryListSearchCondition();
		}

		/// <summary>
		/// 検索コンディションのパラメーターがあるか
		/// </summary>
		/// <returns>検索コンディションのパラメーターがあればTrue</returns>
		private bool HasSearchConditionRequestParams()
		{
			return (this.RequestSearchShopId != null)
				|| (this.RequestSearchWorkflowKbn != null)
				|| (this.RequestSearchWorkflowNo != null)
				|| (this.RequestSearchScenarioSettingId != null)
				|| (this.RequestSearchExecStatus != null)
				|| (this.RequestSearchExecPlace != null)
				|| (this.RequestSearchExecTiming != null)
				|| (this.RequestSearchWorkflowType != null)
				|| (this.RequestSearchDateFrom != null)
				|| (this.RequestSearchTimeFrom != null)
				|| (this.RequestSearchDateTo != null)
				|| (this.RequestSearchTimeTo != null);
		}

		/// <summary>
		/// 文字列の年月日からDateTimeを作成
		/// </summary>
		/// <param name="year">年</param>
		/// <param name="month">月</param>
		/// <param name="day">日</param>
		/// <returns>DateTime</returns>
		private DateTime? CreateDateTime(string year, string month, string day)
		{
			if (string.IsNullOrEmpty(year)
				|| string.IsNullOrEmpty(month)
				|| string.IsNullOrEmpty(day))
			{
				return null;
			}

			// 末日補正処理
			day = DateTimeUtility.CorrectLastDayOfMonth(year, month, day);

			DateTime date;
			if (DateTime.TryParse((string.Format("{0}/{1}/{2}", year, month, day)), out date))
			{
				return date;
			}

			return null;
		}

		/// <summary>
		/// ページネーションを作成
		/// </summary>
		/// <param name="condition">検索条件</param>
		private void CreatePagination(OrderWorkflowExecHistoryListSearchCondition condition)
		{
			var count = new OrderWorkflowExecHistoryService().GetSearchHitCount(condition);
			var url = CreateSearchConditionUrl();
			lbPager1.Text = WebPager.CreateDefaultListPager(count, this.CurrentPageNo, url);
		}

		/// <summary>
		/// list_item_bg_?のCSSクラスを取得
		/// </summary>
		/// <param name="execStatus">実行ステータス</param>
		/// <returns>CSS Clas</returns>
		protected string GetCssClassToListItemBackGround(string execStatus)
		{
			var cssClass = "";
			switch (execStatus)
			{
				case Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_OK:
					cssClass = "list_item_bg_success";
					break;

				case Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_STOPPED:
				case Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_NG:
					cssClass = "list_item_bg_error";
					break;
			}
			return WebSanitizer.HtmlEncode(cssClass);
		}

		/// <summary>
		/// ExecStatusの表示用Textに変換
		/// </summary>
		/// <param name="execStatus">実行ステータス</param>
		/// <returns>ExecStatus表示用Text</returns>
		protected string ConvertExecStatusForDisplay(string execStatus)
		{
			var valueExecStatus = WebSanitizer.HtmlEncode(ValueText.GetValueText(
				Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
				Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS,
				execStatus));
			return valueExecStatus;
		}

		/// <summary>
		/// ExecPlaceの表示用Textに変換
		/// </summary>
		/// <param name="execPlace">実行ステータス</param>
		/// <returns>ExecPlace表示用Text</returns>
		protected string ConvertExecPlaceForDisplay(string execPlace)
		{
			var valueExecStatus = WebSanitizer.HtmlEncode(ValueText.GetValueText(
				Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
				Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE,
				execPlace));
			return valueExecStatus;
		}

		/// <summary>
		/// ExecTimingの表示用Textに変換
		/// </summary>
		/// <param name="execTiming">実行ステータス</param>
		/// <returns>ExecTiming表示用Text</returns>
		protected string ConvertExecTimingForDisplay(string execTiming)
		{
			var valueExecStatus = WebSanitizer.HtmlEncode(ValueText.GetValueText(
				Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
				Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING,
				execTiming));
			return valueExecStatus;
		}

		/// <summary>
		/// 詳細画面へ遷移するためのURLを作成
		/// </summary>
		/// <param name="execHistoryId">OrderWorkflowExecHistoryId</param>
		/// <returns>詳細画面URL</returns>
		protected string CreateExecHistoryDetailsUrl(long execHistoryId)
		{
			SessionManager.OrderworkflowDetailsUrlOfPreviousPage = Request.Url.ToString();
			var url = new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDERWORKFLOW_EXEC_HISTORY_DETAILS)
				.AddParam(Constants.FIELD_ORDERWORKFLOWEXECHISTORY_ORDER_WORKFLOW_EXEC_HISTORY_ID, execHistoryId.ToString())
				.CreateUrl();
			return WebSanitizer.UrlAttrHtmlEncode(url);
		}

		/// <summary>
		/// 表示用のシナリオ名に変換
		/// </summary>
		/// <param name="scenarioName">シナリオ名</param>
		/// <returns>シナリオ名表示用Text</returns>
		protected string ConvertScenarioNameForDisplay(string scenarioName)
		{
			return WebSanitizer.HtmlEncode(!string.IsNullOrEmpty(scenarioName) ? "(" + scenarioName + ")" : "");
		}

		/// <summary>
		/// WorkflowTypeの表示用Textに変換
		/// </summary>
		/// <param name="workflowType">ワークフロータイプ</param>
		/// <returns>ExecPlace表示用Text</returns>
		protected string ConvertWorkflowTypeForDisplay(string workflowType)
		{
			var valueExecStatus = WebSanitizer.HtmlEncode(ValueText.GetValueText(
				Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
				Constants.FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE,
				workflowType));
			return valueExecStatus;
		}

		/// <summary>
		/// 検索ボタンクリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnSearch_Click(object sender, EventArgs e)
		{
			Response.Redirect(CreateSearchConditionUrl());
		}

		/// <summary>
		/// 検索条件のパラメーター付URLを作成
		/// </summary>
		/// <returns> 検索条件のパラメーター付URL</returns>
		private string CreateSearchConditionUrl()
		{
			var workflowKbn = "";
			var workflowNo = "";
			if (!string.IsNullOrEmpty(ddlWorkflowList.SelectedValue))
			{
				var listItemValueSplit = ddlWorkflowList.SelectedValue.Split(':');
				workflowKbn = listItemValueSplit[0];
				workflowNo = listItemValueSplit[1];
			}

			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_EXEC_HISTORY_LIST)
				.AddParam(Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_SHOP_ID, this.LoginOperatorShopId)
				.AddParam(Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_WORKFLOW_KBN, workflowKbn)
				.AddParam(Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_WORKFLOW_NO, workflowNo)
				.AddParam(
					Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_SCENARIO_SETTING_ID,
					ddlScenarioList.SelectedValue)
				.AddParam(
					Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS,
					CreateCheckBoxListParameterValue(cblExecStatus))
				.AddParam(
					Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE,
					CreateCheckBoxListParameterValue(cblExecPlace))
				.AddParam(
					Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING,
					CreateCheckBoxListParameterValue(cblExecTiming))
				.AddParam(
					Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE,
					CreateCheckBoxListParameterValue(cblWorkflowType))
				.AddParam(
					Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_DATE_FROM,
					ucSearchDatePeriod.HfStartDate.Value)
				.AddParam(
					Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_TIME_FROM,
					ucSearchDatePeriod.HfStartTime.Value)
				.AddParam(
					Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_DATE_TO,
					ucSearchDatePeriod.HfEndDate.Value)
				.AddParam(
					Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_TIME_TO,
					ucSearchDatePeriod.HfEndTime.Value)
				.CreateUrl();

			return url;
		}

		/// <summary>
		/// チェックボックスリストのチェックされているアイテムを「,」区切りの文字列で取得
		/// </summary>
		/// <param name="checkBoxList">チェックボックスリスト</param>
		/// <returns>チェックされているアイテムの値</returns>
		private string CreateCheckBoxListParameterValue(CheckBoxList checkBoxList)
		{
			var param = string.Join(
				",",
				checkBoxList.Items.Cast<ListItem>()
					.Where(item => item.Selected)
					.Select(item => item.Value).ToArray());
			return param;
		}

		/// <summary>現在のページ番号</summary>
		private int CurrentPageNo
		{
			get
			{
				int pageNo;
				if (!int.TryParse(Request.Params[Constants.REQUEST_KEY_PAGE_NO], out pageNo)) return 1;
				var parsedCurrentPageNo = (pageNo < 1) ? 1 : pageNo;
				return parsedCurrentPageNo;
			}
		}
		/// <summary>検索用リクエスト: ショップID</summary>
		private string RequestSearchShopId
		{
			get { return Request.Params[Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_SHOP_ID]; }
		}
		/// <summary>検索用リクエスト: ワークフロー区分</summary>
		private string RequestSearchWorkflowKbn
		{
			get { return Request.Params[Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_WORKFLOW_KBN]; }
		}
		/// <summary>検索用リクエスト: ワークフローNo</summary>
		private string RequestSearchWorkflowNo
		{
			get { return Request.Params[Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_WORKFLOW_NO]; }
		}
		/// <summary>検索用リクエスト: シナリオ設定ID</summary>
		private string RequestSearchScenarioSettingId
		{
			get { return Request.Params[Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_SCENARIO_SETTING_ID]; }
		}
		/// <summary>検索用リクエスト: 実行ステータス</summary>
		private string RequestSearchExecStatus
		{
			get { return Request.Params[Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS]; }
		}
		/// <summary>検索用リクエスト: 実行起点</summary>
		private string RequestSearchExecPlace
		{
			get { return Request.Params[Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE]; }
		}
		/// <summary>検索用リクエスト: 実行タイミング</summary>
		private string RequestSearchExecTiming
		{
			get { return Request.Params[Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING]; }
		}
		/// <summary>検索用リクエスト: ワークフロータイプ</summary>
		private string RequestSearchWorkflowType
		{
			get { return Request.Params[Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE]; }
		}
		/// <summary>Search request: Month of end date and time</summary>
		private string RequestSearchDateFrom
		{
			get { return Request.Params[Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_DATE_FROM]; }
		}
		/// <summary>Search request: Month of end date and time</summary>
		private string RequestSearchTimeFrom
		{
			get { return Request.Params[Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_TIME_FROM]; }
		}
		/// <summary>Search request: Month of end date and time</summary>
		private string RequestSearchDateTo
		{
			get { return Request.Params[Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_DATE_TO]; }
		}
		/// <summary>Search request: Month of end date and time</summary>
		private string RequestSearchTimeTo
		{
			get { return Request.Params[Constants.REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_TIME_TO]; }
		}
	}
}