/*
=========================================================================================================
  Module      : ポイントルールスケジュール登録ページ処理(PointRuleScheduleRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Util;
using w2.Domain.Point;
using w2.Domain.TargetList;
using w2.Domain.MailDist;
using w2.Common.Web;
using Input.Point;

public partial class Form_PointRuleSchedule_PointRuleScheduleRegister : BasePage
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
			// On Load イベント追加
			this.Master.OnLoadEvents += "RefreshSchedule();";

			// コンポーネント初期化
			InitializeComponent();

			// ポイントルールスケジュール取得＆画面セット
			PointRuleScheduleInput pointRuleSchedule = null;
			// 新規登録
			if (this.ActionStatus == Constants.ACTION_STATUS_INSERT)
			{
				pointRuleSchedule = (PointRuleScheduleInput)Session[Constants.SESSIONPARAM_KEY_POINTRULESCHEDULE_INFO];
			}
			// 編集・コピー新規
			else if ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
				|| (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT))
			{
				pointRuleSchedule = new PointRuleScheduleInput(new PointService().GetPointRuleSchedule(Request[Constants.REQUEST_KEY_POINTRULESCHEDULE_ID]));

			}
			// 画面表示
			SetDisplay(pointRuleSchedule ?? new PointRuleScheduleInput(new PointRuleScheduleModel()));
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponent()
	{
		// ターゲットリストドロップダウンセット
		var targetLists = new TargetListService().GetAll(this.LoginOperatorDeptId);
		var itemIndex = ddlTargetId.Items.Count;
		foreach (var targetList in targetLists.Where(i => i.DelFlg == "0"))
		{
			ddlTargetId.Items.Add(new ListItem(string.Format("{0}({1})", targetList.TargetName, targetList.DataCount), targetList.TargetId));
			ddlTargetId.Items[itemIndex].Attributes["disable_extract"] = Constants.TARGET_LIST_IMPORT_TYPE_LIST.Contains(targetList.TargetType).ToString();
			itemIndex++;
		}

		// ポイントルールドロップダウンセット
		ddlPointRule.Items.Clear();
		ddlPointRule.Items.Add(new ListItem("", ""));
		var pointRules = new PointService().GetAllPointRules();
		foreach (var pointRule in pointRules.Where(i => ((i.ValidFlg == Constants.FLG_POINTRULE_VALID_FLG_VALID) && i.UseScheduleIncKbnPointRule)))
		{
			ddlPointRule.Items.Add(new ListItem(pointRule.PointRuleName, pointRule.PointRuleId));
		}


		// メール文章
		var mailTexts = new MailDistService().GetTextAll(this.LoginOperatorDeptId);
		ddlMailTemp.Items.Add(
			new ListItem(Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(
				"@@DispText.auto_text.unspecified@@",
				Constants.GLOBAL_OPTION_ENABLE
					? Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE
					: ""),
				""));
		foreach (var mailText in mailTexts.Where(i => i.DelFlg == "0"))
		{
			ddlMailTemp.Items.Add(new ListItem(mailText.MailtextName, mailText.MailtextId));
		}

		// スケジュール
		// ラジオボタン系
		rbExecByManual.Text = ValueText.GetValueText(Constants.TABLE_POINTRULESCHEDULE, Constants.FIELD_POINTRULESCHEDULE_EXEC_TIMING, Constants.FLG_POINTRULESCHEDULE_EXEC_TIMING_MANUAL);
		//rbExecByAction.Text = ValueText.GetValueText(Constants.TABLE_POINTRULESCHEDULE, Constants.FIELD_POINTRULESCHEDULE_EXEC_TIMING, Constants.FLG_POINTRULESCHEDULE_EXEC_TIMING_ACTION);
		rbExecBySchedule.Text = ValueText.GetValueText(Constants.TABLE_POINTRULESCHEDULE, Constants.FIELD_POINTRULESCHEDULE_EXEC_TIMING, Constants.FLG_POINTRULESCHEDULE_EXEC_TIMING_SCHEDULE);
		rbExecByManual.Checked = true;

		rbScheRepeatDay.Text = ValueText.GetValueText(Constants.TABLE_POINTRULESCHEDULE, Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_KBN, Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_DAY);
		rbScheRepeatWeek.Text = ValueText.GetValueText(Constants.TABLE_POINTRULESCHEDULE, Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_KBN, Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_WEEK);
		rbScheRepeatMonth.Text = ValueText.GetValueText(Constants.TABLE_POINTRULESCHEDULE, Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_KBN, Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_MONTH);
		rbScheRepeatOnce.Text = ValueText.GetValueText(Constants.TABLE_POINTRULESCHEDULE, Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_KBN, Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_ONCE);
		rbScheRepeatOnce.Checked = true;

		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_POINTRULESCHEDULE, Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_DAY_OF_WEEK))
		{
			rblScheDayOfWeek.Items.Add(li);
		}
		if (rblScheDayOfWeek.Items.Count > 0)
		{
			rblScheDayOfWeek.Items[0].Selected = true;
		}

		// 実行タイミングの日付初期化
		ucScheDateTime.SetDate(DateTime.Now.Date);
	}

	/// <summary>
	/// 画面表示
	/// </summary>
	/// <param name="input">インプット</param>
	private void SetDisplay(PointRuleScheduleInput input)
	{
		// ポイントルールスケジュールID
		hfPointRuleScheduleId.Value = input.PointRuleScheduleId;
		// 最終付与人数・最終実行日をセット
		hfLastCount.Value = input.LastCount;
		hfLastExecDate.Value = input.LastExecDate;
		// ポイントルールスケジュール名
		tbPointRuleScheduleName.Text = input.PointRuleScheduleName;

		// ターゲットリストID
		foreach (ListItem li in ddlTargetId.Items)
		{
			li.Selected = (li.Value == input.TargetId);
		}
		cbTargetExtract.Checked = (input.TargetExtractFlg == Constants.FLG_POINTRULESCHEDULE_TARGET_EXTRACT_FLG_ON);

		// ポイントルールID
		foreach (ListItem li in ddlPointRule.Items)
		{
			li.Selected = (li.Value == input.PointRuleId);
		}
		// メールテンプレートID
		foreach (ListItem li in ddlMailTemp.Items)
		{
			li.Selected = (li.Value == input.MailId);
		}

		// 手動実行
		rbExecByManual.Checked = (input.ExecTiming == Constants.FLG_POINTRULESCHEDULE_EXEC_TIMING_MANUAL);
		// スケジュール実行
		rbExecBySchedule.Checked = (input.ExecTiming == Constants.FLG_POINTRULESCHEDULE_EXEC_TIMING_SCHEDULE);
		// スケジュール実行
		if (input.ExecTiming == Constants.FLG_POINTRULESCHEDULE_EXEC_TIMING_SCHEDULE)
		{
			// スケジュール区分
			// 日単位（毎日HH:mm:ssに実行
			rbScheRepeatDay.Checked = (input.ScheduleKbn == Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_DAY);
			// 週単位（毎週ddd曜日HH:mm:ssに実行）
			rbScheRepeatWeek.Checked = (input.ScheduleKbn == Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_WEEK);
			// 月単位（毎月dd日HH:mm:ssに実行）
			rbScheRepeatMonth.Checked = (input.ScheduleKbn == Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_MONTH);
			// 一回のみ（一回のみyyyy年MM月dd日HH:mm:ssに実行）
			rbScheRepeatOnce.Checked = (input.ScheduleKbn == Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_ONCE);

			if (rbScheRepeatWeek.Checked)
			{
				// スケジュール曜日
				foreach (ListItem li in rblScheDayOfWeek.Items)
				{
					li.Selected = (li.Value == input.ScheduleDayOfWeek);
				}
			}
			else
			{
				rblScheDayOfWeek.SelectedIndex = 0;
			}
			// スケジュール日程
			ucScheDateTime.Year = input.ScheduleYear;
			ucScheDateTime.Month = PadLeftZero2Letter(input.ScheduleMonth);
			ucScheDateTime.Day = PadLeftZero2Letter(input.ScheduleDay);
			ucScheDateTime.Hour = PadLeftZero2Letter(input.ScheduleHour);
			ucScheDateTime.Minute = PadLeftZero2Letter(input.ScheduleMinute);
			ucScheDateTime.Second = PadLeftZero2Letter(input.ScheduleSecond);
		}
		// 手動実行（スケジュールはデフォルト値を設定しておく）
		else
		{
			rbScheRepeatOnce.Checked = true;
			rbScheRepeatDay.Checked = rbScheRepeatWeek.Checked = rbScheRepeatMonth.Checked = false;
			rblScheDayOfWeek.SelectedIndex = 0;
		}

		if (((this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
				|| (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT))
			&& rbScheRepeatOnce.Checked
			&& (this.IsBackFromConfirm == false))
		{
			var scheDateTimeOnce = string.Format("{0}/{1}/{2} {3}:{4}:{5}",
				input.ScheduleYear,
				input.ScheduleMonth,
				input.ScheduleDay,
				input.ScheduleHour,
				input.ScheduleMinute,
				input.ScheduleSecond);

			DateTime scheDateTimeOnces;
			if (DateTime.TryParse(scheDateTimeOnce, out scheDateTimeOnces))
			{
				ucScheDateTimeOnce.SetStartDate(scheDateTimeOnces);
			}
			else
			{
				ucScheDateTimeOnce.SetStartDate(DateTime.Today);
			}
		}
		if (this.ActionStatus == Constants.ACTION_STATUS_INSERT)
		{
			ucScheDateTimeOnce.SetStartDate(DateTime.Today);
		}

		if (this.IsBackFromConfirm
			&& (Session[Constants.SESSIONPARAM_KEY_POINTRULESCHEDULE_INFO] != null))
		{
			var pointRuleScheduleInput =
				(PointRuleScheduleInput)Session[Constants.SESSIONPARAM_KEY_POINTRULESCHEDULE_INFO];

			var scheDateTimeOnce = string.Format("{0}/{1}/{2} {3}:{4}:{5}",
				pointRuleScheduleInput.ScheduleYear,
				pointRuleScheduleInput.ScheduleMonth,
				pointRuleScheduleInput.ScheduleDay,
				pointRuleScheduleInput.ScheduleHour,
				pointRuleScheduleInput.ScheduleMinute,
				pointRuleScheduleInput.ScheduleSecond);

			DateTime scheDateTimeOnces;
			if (DateTime.TryParse(scheDateTimeOnce, out scheDateTimeOnces))
			{
				ucScheDateTimeOnce.SetStartDate(scheDateTimeOnces);
			}

			Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = null;
		}

		// 有効フラグ
		cbValidFlg.Checked = (input.ValidFlg == Constants.FLG_POINTRULESCHEDULE_VALID_FLG_VALID);
	}

	/// <summary>
	/// インプット作成
	/// </summary>
	/// <returns>インプット</returns>
	private PointRuleScheduleInput CreateInput()
	{
		var input = new PointRuleScheduleInput
		{
			PointRuleScheduleId = (this.ActionStatus == Constants.ACTION_STATUS_UPDATE) ? hfPointRuleScheduleId.Value : "",
			PointRuleScheduleName = tbPointRuleScheduleName.Text,
			LastCount = (this.ActionStatus == Constants.ACTION_STATUS_UPDATE) ? hfLastCount.Value : "",
			LastExecDate = (this.ActionStatus == Constants.ACTION_STATUS_UPDATE) ? hfLastExecDate.Value : null,
			TargetId = ddlTargetId.SelectedValue,
			TargetExtractFlg = ((ddlTargetId.SelectedValue != "") && cbTargetExtract.Checked) ? Constants.FLG_POINTRULESCHEDULE_TARGET_EXTRACT_FLG_ON : Constants.FLG_POINTRULESCHEDULE_TARGET_EXTRACT_FLG_OFF,
			PointRuleId = ddlPointRule.SelectedValue,
			MailId = ddlMailTemp.SelectedValue,
			ValidFlg = cbValidFlg.Checked ? Constants.FLG_POINTRULESCHEDULE_VALID_FLG_VALID : Constants.FLG_POINTRULESCHEDULE_VALID_FLG_INVALID,
			LastChanged = this.LoginOperatorName,
		};

		// 実行タイミング情報セット
		if (rbExecByManual.Checked)
		{
			// 手動実行
			input.ExecTiming = Constants.FLG_POINTRULESCHEDULE_EXEC_TIMING_MANUAL;
			input.ScheduleKbn = "";
		}
		else if (rbExecBySchedule.Checked)
		{
			// スケジュール実行
			input.ExecTiming = Constants.FLG_POINTRULESCHEDULE_EXEC_TIMING_SCHEDULE;

			if (rbScheRepeatDay.Checked)
			{
				// 日単位（毎日HH:mm:ssに実行）
				input.ScheduleKbn = Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_DAY;
			}
			else if (rbScheRepeatWeek.Checked)
			{
				// 週単位（毎週ddd曜日HH:mm:ssに実行）
				input.ScheduleKbn = Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_WEEK;
				input.ScheduleDayOfWeek = rblScheDayOfWeek.SelectedValue;
			}
			else if (rbScheRepeatMonth.Checked)
			{
				// 月単位（毎月dd日HH:mm:ssに実行）
				input.ScheduleKbn = Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_MONTH;
				input.ScheduleDay = ucScheDateTime.Day;
			}
			else if (rbScheRepeatOnce.Checked
				&& (string.IsNullOrEmpty(ucScheDateTimeOnce.HfStartDate.Value) == false))
			{
				// 一回のみ（一回のみyyyy年MM月dd日HH:mm:ssに実行）
				input.ScheduleKbn = Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_ONCE;
				var scheDate = DateTime.Parse(ucScheDateTimeOnce.HfStartDate.Value);
				input.ScheduleYear = scheDate.Year.ToString();
				input.ScheduleMonth = scheDate.Month.ToString();
				input.ScheduleDay = scheDate.Day.ToString();
			}

			if (rbScheRepeatOnce.Checked
				&& (string.IsNullOrEmpty(ucScheDateTimeOnce.HfStartDate.Value) == false))
			{
				var scheTime = DateTime.Parse(ucScheDateTimeOnce.HfStartTime.Value);
				input.ScheduleHour = scheTime.Hour.ToString();
				input.ScheduleMinute = scheTime.Minute.ToString();
				input.ScheduleSecond = scheTime.Second.ToString();
			}
			else
			{
				input.ScheduleHour = ucScheDateTime.Hour;
				input.ScheduleMinute = ucScheDateTime.Minute;
				input.ScheduleSecond = ucScheDateTime.Second;
			}
		}

		return input;
	}

	/// <summary>
	/// 確認ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		// 入力値生成
		var input = CreateInput();

		// パラメタをセッションへセット
		Session[Constants.SESSIONPARAM_KEY_POINTRULESCHEDULE_INFO] = input;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = 1;
		// 入力チェック＆重複チェック
		var strErrorMessages = input.Validate();
		if (strErrorMessages != "")
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		// リダイレクト
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULESCHEDULE_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_CONFIRM);
		if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			url.AddParam(Constants.REQUEST_KEY_POINTRULESCHEDULE_ID, hfPointRuleScheduleId.Value);
		}

		Response.Redirect(url.CreateUrl());
	}

	/// <summary>確認画面から戻ってきたか</summary>
	protected bool IsBackFromConfirm
	{
		get { return (Session[Constants.SESSION_KEY_PARAM_FOR_BACK] != null); }
	}
}