/*
=========================================================================================================
  Module      : ポイントルールスケジュール確認ページ処理(PointRuleScheduleConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using w2.Common.Util;
using w2.Domain.Point;
using w2.Domain.TaskSchedule;
using w2.Domain.TaskSchedule.Helper;
using Input.Point;
using w2.Common.Web;
using w2.App.Common.TargetList;
using w2.Domain.MailDist;

/// <summary>
/// ポイントルールスケジュール確認ページ処理
/// </summary>
public partial class Form_PointRuleSchedule_PointRuleScheduleConfirm : BasePage
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
			// ID表示設定
			trId.Visible = (this.ActionStatus == Constants.ACTION_STATUS_DETAIL);

			// コンポーネント初期化
			InitializeComponent(this.ActionStatus);
		}

		// データ取得
		PointRuleScheduleInput input = null;
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_DETAIL:	// 詳細表示
				var pointRuleSchedule = new PointService().GetPointRuleSchedule(this.PointRuleScheduleId);
				input = new PointRuleScheduleInput(pointRuleSchedule);
				break;

			case Constants.ACTION_STATUS_CONFIRM:	// 確認表示
				input = (PointRuleScheduleInput)Session[Constants.SESSIONPARAM_KEY_POINTRULESCHEDULE_INFO];
				break;
		}

		// 画面表示
		SetDisplay(input);
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	/// <param name="actionKbn">アクション区分</param>
	private void InitializeComponent(string actionKbn)
	{
		// 削除ボタン
		btnDelete.Visible = btnDelete2.Visible = (actionKbn == Constants.ACTION_STATUS_DETAIL);

		// 編集ボタン
		btnEdit.Visible = btnEdit2.Visible = (actionKbn == Constants.ACTION_STATUS_DETAIL);

		// コピー新規登録ボタン
		btnCopyInsert.Visible = btnCopyInsert2.Visible = (actionKbn == Constants.ACTION_STATUS_DETAIL);

		// 登録ボタン
		btnInsert.Visible = btnInsert2.Visible = ((actionKbn == Constants.ACTION_STATUS_CONFIRM) && (this.PointRuleScheduleId == null));

		// 更新ボタン
		btnUpdate.Visible = btnUpdate2.Visible = ((actionKbn == Constants.ACTION_STATUS_CONFIRM) && (this.PointRuleScheduleId != null));

		// 会員ランク付与ステータス情報など
		dvMemberRankRuleStatusInfo.Visible = (actionKbn == Constants.ACTION_STATUS_DETAIL);
	}

	/// <summary>
	/// 画面表示
	/// </summary>
	/// <param name="input">インプット</param>
	private void SetDisplay(PointRuleScheduleInput input)
	{
		// ポイントルールスケジュールID
		lPointRuleScheduleId.Text = WebSanitizer.HtmlEncode(input.PointRuleScheduleId);
		// ポイントルールスケジュール名
		lPointRuleScheduleName.Text = WebSanitizer.HtmlEncode(input.PointRuleScheduleName);
		// ポイントルール
		var pointRule = new PointService().GetPointRule(this.LoginOperatorShopId, input.PointRuleId);
		lPointRule.Text = pointRule.PointRuleName;

		// ターゲットリスト
		lTarget.Text = GetTargetName(input.TargetId);

		// メールテンプレート
		var mailTemplate = new MailDistService().GetText(this.LoginOperatorShopId, input.MailId);
		// メールテンプレート名
		lMailTemp.Text = (mailTemplate != null) ? mailTemplate.MailtextName : "-";

		// 実行タイミング
		lScheduleString.Text = GetScheduleString(input);

		// 有効フラグ
		lValidFlg.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_POINTRULESCHEDULE, Constants.FIELD_POINTRULESCHEDULE_VALID_FLG, input.ValidFlg));
		// ステータス
		lStatus.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_POINTRULESCHEDULE, Constants.FIELD_POINTRULESCHEDULE_STATUS, input.Status));
		// 最終付与人数
		lLastCount.Text = WebSanitizer.HtmlEncode(input.LastCount ?? "-");
		// 最終付与日時
		lLastExecDate.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				input.LastExecDate,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
				"-"));
	}

	/// <summary>
	/// ターゲット名取得
	/// </summary>
	/// <param name="targetId">ターゲットID</param>
	/// <returns>ターゲット名取得</returns>
	protected string GetTargetName(string targetId)
	{
		if (string.IsNullOrEmpty(targetId)) return null;

		// ターゲット情報取得
		var targetList = TargetListUtility.GetTargetList(this.LoginOperatorDeptId, targetId);
		if (targetList.Count != 0) return (string)targetList[0][Constants.FIELD_TARGETLIST_TARGET_NAME];

		return null;
	}

	/// <summary>
	/// スケジュール文言作成
	/// </summary>
	/// <param name="input">インプット</param>
	/// <returns>スケジュール文言</returns>
	protected string GetScheduleString(PointRuleScheduleInput input)
	{
		StringBuilder scheduleString = new StringBuilder();
		switch (input.ExecTiming)
		{
			case Constants.FLG_POINTRULESCHEDULE_EXEC_TIMING_MANUAL:		// 手動実行
				scheduleString.Append(ValueText.GetValueText(Constants.TABLE_POINTRULESCHEDULE, Constants.FIELD_POINTRULESCHEDULE_EXEC_TIMING, input.ExecTiming)).Append("\r\n");
				break;

			case Constants.FLG_POINTRULESCHEDULE_EXEC_TIMING_SCHEDULE:		// スケジュール実行
				break;
		}

		if (input.ExecTiming == Constants.FLG_POINTRULESCHEDULE_EXEC_TIMING_SCHEDULE)
		{
			switch (input.ScheduleKbn)
			{
				case Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_DAY:		// 日単位（毎日HH:mm:ssに実行）
					scheduleString.Append(ReplaceTag("@@DispText.schedule_kbn.Day@@"));
					break;

				case Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_WEEK:	// 週単位（毎週ddd曜日HH:mm:ssに実行）
					scheduleString.Append(ReplaceTag("@@DispText.schedule_kbn.Week@@"));
					scheduleString.Append(ValueText.GetValueText(Constants.TABLE_POINTRULESCHEDULE, Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_DAY_OF_WEEK, input.ScheduleDayOfWeek) + ReplaceTag("@@DispText.schedule_unit.DayOfWeek@@"));
					scheduleString.Append(" ");
					break;

				case Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_MONTH:	// 月単位（毎月dd日HH:mm:ssに実行）
					scheduleString.Append(ReplaceTag("@@DispText.schedule_kbn.Month@@"));
					scheduleString.Append(input.ScheduleDay).Append(ReplaceTag("@@DispText.schedule_unit.Day@@"));
					scheduleString.Append(" ");
					break;

				case Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_ONCE:	// 一回のみ（一回のみyyyy年MM月dd日HH:mm:ssに実行）
					scheduleString.Append(ReplaceTag("@@DispText.schedule_kbn.Once@@"));
					scheduleString.Append(DateTimeUtility.ToStringForManager(
						string.Format(
							"{0}/{1}/{2} {3}:{4}:{5}",
							input.ScheduleYear,
							input.ScheduleMonth,
							input.ScheduleDay,
							input.ScheduleHour,
							input.ScheduleMinute,
							input.ScheduleSecond),
						DateTimeUtility.FormatType.LongDateHourMinuteSecond1Letter));
					break;
			}
			if (input.ScheduleKbn != Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_ONCE)
			{
				scheduleString.Append(DateTimeUtility.ToStringForManager(
					string.Format("{0}:{1}:{2}", input.ScheduleHour, input.ScheduleMinute, input.ScheduleSecond),
					DateTimeUtility.FormatType.HourMinuteSecond1Letter));
			}
		}

		return StringUtility.ChangeToBrTag(WebSanitizer.HtmlEncode(scheduleString.ToString()));
	}

	/// <summary>
	/// タスクスケジュール作成判定（過去であれば作成したくない）
	/// </summary>
	/// <param name="input">インプット</param>
	/// <returns>判定結果</returns>
	private bool CanInsertTaskSchedule(PointRuleScheduleInput input)
	{
		if ((input.ExecTiming == Constants.FLG_POINTRULESCHEDULE_EXEC_TIMING_SCHEDULE)
			&& (input.ScheduleKbn == Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_ONCE))
		{
			var schedule = new DateTime(
				int.Parse(input.ScheduleYear),
				int.Parse(input.ScheduleMonth),
				int.Parse(input.ScheduleDay),
				int.Parse(input.ScheduleHour),
				int.Parse(input.ScheduleMinute),
				int.Parse(input.ScheduleSecond));

			return (DateTime.Now.CompareTo(schedule) <= 0);
		}
		return true;
	}

	/// <summary>
	/// 編集
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = null;

		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULESCHEDULE_REGISTER)
			.AddParam(Constants.REQUEST_KEY_POINTRULESCHEDULE_ID, this.PointRuleScheduleId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE);

		// 編集画面へ遷移
		Response.Redirect(url.CreateUrl());
	}

	/// <summary>
	/// コピー新規登録
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULESCHEDULE_REGISTER)
			.AddParam(Constants.REQUEST_KEY_POINTRULESCHEDULE_ID, this.PointRuleScheduleId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_COPY_INSERT);

		// 編集画面へ遷移
		Response.Redirect(url.CreateUrl());
	}

	/// <summary>
	/// 削除
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{

		var pointRuleScheduleId = this.PointRuleScheduleId;

		// スケジュール削除
		new TaskScheduleService().DeleteMasterId(this.LoginOperatorDeptId, Constants.FLG_TASKSCHEDULE_ACTION_KBN_ADD_POINT, pointRuleScheduleId);

		// ポイントルールスケジュール削除
		new PointService().DeletePointRuleSchedule(pointRuleScheduleId);

		// 一覧画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULESCHEDULE_LIST);
	}

	/// <summary>
	/// 登録
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		// ポイントルールスケジュールID作成
		string newPointRuleScheduleId = NumberingUtility.CreateKeyId(this.LoginOperatorShopId, Constants.NUMBER_KEY_MP_POINTRULESCHEDULE_ID, 10);
		var input = (PointRuleScheduleInput)Session[Constants.SESSIONPARAM_KEY_POINTRULESCHEDULE_INFO];
		var model = input.CreateModel();
		model.PointRuleScheduleId = newPointRuleScheduleId;

		try
		{
			// ポイントルールスケジュール登録
			new PointService().InsertPointRuleSchedule(model);

			// スケジュール追加
			if (CanInsertTaskSchedule(input) && (input.ValidFlg == Constants.FLG_POINTRULESCHEDULE_VALID_FLG_VALID))
			{
				new TaskScheduleService().InsertFirstTaskSchedule(
					this.LoginOperatorDeptId,
					Constants.FLG_TASKSCHEDULE_ACTION_KBN_ADD_POINT,
					newPointRuleScheduleId,
					this.LoginOperatorName,
					new TaskScheduleRule(model));
			}
		}
		catch (Exception ex)
		{
			throw ex;
		}

		// 一覧画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULESCHEDULE_LIST);
	}

	/// <summary>
	/// 更新
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		var input = (PointRuleScheduleInput)Session[Constants.SESSIONPARAM_KEY_POINTRULESCHEDULE_INFO];
		var model = input.CreateModel();

		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			try
			{
				// ポイントルールスケジュール更新
				new PointService().UpdatePointRuleSchedule(model, accessor);

				// 未実行スケジュール削除
				new TaskScheduleService().DeleteTaskScheduleUnexecuted(
					this.LoginOperatorDeptId,
					Constants.FLG_TASKSCHEDULE_ACTION_KBN_ADD_POINT,
					model.PointRuleScheduleId,
					accessor);

				// スケジュール追加
				if (CanInsertTaskSchedule(input) && (input.ValidFlg == Constants.FLG_POINTRULESCHEDULE_VALID_FLG_VALID))
				{
					new TaskScheduleService().InsertFirstTaskSchedule(
						this.LoginOperatorDeptId,
						Constants.FLG_TASKSCHEDULE_ACTION_KBN_ADD_POINT,
						model.PointRuleScheduleId,
						this.LoginOperatorName,
						new TaskScheduleRule(model),
						accessor);
				}

				accessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				accessor.RollbackTransaction();
				throw ex;
			}
		}

		// 一覧画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULESCHEDULE_LIST);
	}

	/// <summary>ランク付与ルールID</summary>
	protected string PointRuleScheduleId
	{
		get { return (string)Request[Constants.REQUEST_KEY_POINTRULESCHEDULE_ID]; }
	}
}