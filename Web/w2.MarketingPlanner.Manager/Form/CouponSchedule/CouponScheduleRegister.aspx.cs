/*
=========================================================================================================
  Module      : クーポン発行スケジュール登録ページ処理(CouponScheduleRegister.aspx.cs)
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
using w2.Domain.Coupon;
using w2.Domain.TargetList;
using w2.Domain.MailDist;
using w2.Common.Web;
using Input.Coupon;

public partial class Form_CouponSchedule_CouponScheduleRegister : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		ClearBrowserCache();

		if (!IsPostBack)
		{
			// On Load イベント追加
			this.Master.OnLoadEvents += "RefreshSchedule();";

			// コンポーネント初期化
			InitializeComponent();

			// クーポン発行スケジュール取得＆画面セット
			CouponScheduleInput couponSchedule = null;
			// 新規登録
			if (this.ActionStatus == Constants.ACTION_STATUS_INSERT && Session[Constants.SESSIONPARAM_KEY_COUPONSCHEDULE_INFO] != null)
			{
				couponSchedule = (CouponScheduleInput)Session[Constants.SESSIONPARAM_KEY_COUPONSCHEDULE_INFO];
			}
			// 編集・コピー新規
			else if ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
				|| (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT))
			{
				couponSchedule = new CouponScheduleInput(new CouponService().GetCouponSchedule(Request[Constants.REQUEST_KEY_COUPONSCHEDULE_ID]));
			}
			// 画面表示
			SetDisplay(couponSchedule ?? new CouponScheduleInput(new CouponScheduleModel()));
			SetCouponQuantityDisplay();
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

		// クーポン設定ドロップダウンセット
		ddlCoupon.Items.Clear();
		this.PublishCouponList = new CouponService().GetAllPublishCouponsNotPublishDate(Constants.CONST_DEFAULT_DEPT_ID);
		ddlCoupon.Items.Add(new ListItem("", ""));
		ddlCoupon.Items.AddRange(this.PublishCouponList.Select(cm => new ListItem(cm.CouponName, cm.CouponId)).ToArray());

		// メール文章
		var mailTexts = new MailDistService().GetTextAll(this.LoginOperatorDeptId);
		ddlMailTemp.Items.Add(
			new ListItem(
				Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(
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
		rbExecByManual.Text = ValueText.GetValueText(Constants.TABLE_COUPONSCHEDULE, Constants.FIELD_COUPONSCHEDULE_EXEC_TIMING, Constants.FLG_COUPONSCHEDULE_EXEC_TIMING_MANUAL);
		//rbExecByAction.Text = ValueText.GetValueText(Constants.TABLE_COUPONSCHEDULE, Constants.FIELD_COUPONSCHEDULE_EXEC_TIMING, Constants.FLG_COUPONSCHEDULE_EXEC_TIMING_ACTION);
		rbExecBySchedule.Text = ValueText.GetValueText(Constants.TABLE_COUPONSCHEDULE, Constants.FIELD_COUPONSCHEDULE_EXEC_TIMING, Constants.FLG_COUPONSCHEDULE_EXEC_TIMING_SCHEDULE);
		rbExecByManual.Checked = true;

		rbScheRepeatDay.Text = ValueText.GetValueText(Constants.TABLE_COUPONSCHEDULE, Constants.FIELD_COUPONSCHEDULE_SCHEDULE_KBN, Constants.FLG_COUPONSCHEDULE_SCHEDULE_KBN_DAY);
		rbScheRepeatWeek.Text = ValueText.GetValueText(Constants.TABLE_COUPONSCHEDULE, Constants.FIELD_COUPONSCHEDULE_SCHEDULE_KBN, Constants.FLG_COUPONSCHEDULE_SCHEDULE_KBN_WEEK);
		rbScheRepeatMonth.Text = ValueText.GetValueText(Constants.TABLE_COUPONSCHEDULE, Constants.FIELD_COUPONSCHEDULE_SCHEDULE_KBN, Constants.FLG_COUPONSCHEDULE_SCHEDULE_KBN_MONTH);
		rbScheRepeatOnce.Text = ValueText.GetValueText(Constants.TABLE_COUPONSCHEDULE, Constants.FIELD_COUPONSCHEDULE_SCHEDULE_KBN, Constants.FLG_COUPONSCHEDULE_SCHEDULE_KBN_ONCE);
		rbScheRepeatOnce.Checked = true;

		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_COUPONSCHEDULE, Constants.FIELD_COUPONSCHEDULE_SCHEDULE_DAY_OF_WEEK))
		{
			rblScheDayOfWeek.Items.Add(li);
		}
		if (rblScheDayOfWeek.Items.Count > 0)
		{
			rblScheDayOfWeek.Items[0].Selected = true;
		}

		// 年
		ddlScheYear.Items.AddRange(DateTimeUtility.GetBackwardYearListItem());
		// 月
		ddlScheMonth.Items.AddRange(DateTimeUtility.GetMonthListItem());
		// 日
		ddlScheDay.Items.AddRange(DateTimeUtility.GetDayListItem());
		// 時
		ddlScheHour.Items.AddRange(DateTimeUtility.GetHourListItem());
		// 分・秒
		ddlScheMinute.Items.AddRange(DateTimeUtility.GetMinuteListItem());
		ddlScheSecond.Items.AddRange(DateTimeUtility.GetSecondListItem());
	}

	/// <summary>
	/// 画面表示
	/// </summary>
	/// <param name="input">インプット</param>
	private void SetDisplay(CouponScheduleInput input)
	{
		// クーポン発行スケジュールID
		hfCouponScheduleId.Value = input.CouponScheduleId;
		// クーポン発行スケジュール名
		tbCouponScheduleName.Text = input.CouponScheduleName;

		// ターゲットリストID
		foreach (ListItem li in ddlTargetId.Items)
		{
			li.Selected = (li.Value == input.TargetId);
		}
		cbTargetExtract.Checked = (input.TargetExtractFlg == Constants.FLG_COUPONSCHEDULE_TARGET_EXTRACT_FLG_ON);

		// クーポンID
		foreach (ListItem li in ddlCoupon.Items)
		{
			li.Selected = (li.Value == input.CouponId);
		}
		// クーポン発行枚数
		tbCouponQuantity.Text = input.PublishQuantity;
		// メールテンプレートID
		foreach (ListItem li in ddlMailTemp.Items)
		{
			li.Selected = (li.Value == input.MailId);
		}

		// 手動実行
		rbExecByManual.Checked = (input.ExecTiming == Constants.FLG_COUPONSCHEDULE_EXEC_TIMING_MANUAL);
		// スケジュール実行
		rbExecBySchedule.Checked = (input.ExecTiming == Constants.FLG_COUPONSCHEDULE_EXEC_TIMING_SCHEDULE);
		// スケジュール実行
		if (input.ExecTiming == Constants.FLG_COUPONSCHEDULE_EXEC_TIMING_SCHEDULE)
		{
			// スケジュール区分
			// 日単位（毎日HH:mm:ssに実行
			rbScheRepeatDay.Checked = (input.ScheduleKbn == Constants.FLG_COUPONSCHEDULE_SCHEDULE_KBN_DAY);
			// 週単位（毎週ddd曜日HH:mm:ssに実行）
			rbScheRepeatWeek.Checked = (input.ScheduleKbn == Constants.FLG_COUPONSCHEDULE_SCHEDULE_KBN_WEEK);
			// 月単位（毎月dd日HH:mm:ssに実行）
			rbScheRepeatMonth.Checked = (input.ScheduleKbn == Constants.FLG_COUPONSCHEDULE_SCHEDULE_KBN_MONTH);
			// 一回のみ（一回のみyyyy年MM月dd日HH:mm:ssに実行）
			rbScheRepeatOnce.Checked = (input.ScheduleKbn == Constants.FLG_COUPONSCHEDULE_SCHEDULE_KBN_ONCE);

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
			// スケジュール日程(年)
			foreach (ListItem li in ddlScheYear.Items)
			{
				li.Selected = (li.Value == input.ScheduleYear);
			}
			// スケジュール日程(月)
			foreach (ListItem li in ddlScheMonth.Items)
			{
				li.Selected = (li.Value == input.ScheduleMonth);
			}
			// スケジュール日程(日)
			foreach (ListItem li in ddlScheDay.Items)
			{
				li.Selected = (li.Value == input.ScheduleDay);
			}
			// スケジュール日程(時)
			foreach (ListItem li in ddlScheHour.Items)
			{
				li.Selected = (li.Value == input.ScheduleHour);
			}
			// スケジュール日程(分)
			foreach (ListItem li in ddlScheMinute.Items)
			{
				li.Selected = (li.Value == input.ScheduleMinute);
			}
			// スケジュール日程(秒)
			foreach (ListItem li in ddlScheSecond.Items)
			{
				li.Selected = (li.Value == input.ScheduleSecond);
			}
		}
		// 手動実行（スケジュールはデフォルト値を設定しておく）
		else
		{
			rbScheRepeatOnce.Checked = true;
			rbScheRepeatDay.Checked = rbScheRepeatWeek.Checked = rbScheRepeatMonth.Checked = false;
			rblScheDayOfWeek.SelectedIndex = 0;
		}

		ucScheDateTimeOnce.SetStartDate(DateTime.Today);

		if (((this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
				|| (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT))
			&& rbScheRepeatOnce.Checked)
		{
			var scheDateTimeOnce = string.Format("{0}/{1}/{2} {3}:{4}:{5}",
				input.ScheduleYear,
				input.ScheduleMonth,
				input.ScheduleDay,
				input.ScheduleHour,
				input.ScheduleMinute,
				input.ScheduleSecond);

			var scheDateTimeOnceStart = new DateTime();
			if (DateTime.TryParse(scheDateTimeOnce, out scheDateTimeOnceStart))
			{
				ucScheDateTimeOnce.SetStartDate(scheDateTimeOnceStart);
			}
			else
			{
				ucScheDateTimeOnce.SetStartDate(DateTime.Today);
			}
		}

		if (this.IsBackFromConfirm
			&& (Session[Constants.SESSIONPARAM_KEY_COUPONSCHEDULE_INFO] != null))
		{
			var couponScheduleInput =
				(CouponScheduleInput)Session[Constants.SESSIONPARAM_KEY_COUPONSCHEDULE_INFO];
			if (couponScheduleInput != null)
			{
				var scheDateTimeOnce = string.Format("{0}/{1}/{2} {3}:{4}:{5}",
					couponScheduleInput.ScheduleYear,
					couponScheduleInput.ScheduleMonth,
					couponScheduleInput.ScheduleDay,
					couponScheduleInput.ScheduleHour,
					couponScheduleInput.ScheduleMinute,
					couponScheduleInput.ScheduleSecond);

				var scheDateTimeOnceStart = new DateTime();
				if (DateTime.TryParse(scheDateTimeOnce, out scheDateTimeOnceStart))
				{
					ucScheDateTimeOnce.SetStartDate(scheDateTimeOnceStart);
				}
			}

			Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = null;
		}

		// 有効フラグ
		cbValidFlg.Checked = (input.ValidFlg == Constants.FLG_COUPONSCHEDULE_VALID_FLG_VALID);
	}

	/// <summary>
	/// インプット作成
	/// </summary>
	/// <returns>インプット</returns>
	private CouponScheduleInput CreateInput()
	{
		var input = new CouponScheduleInput
		{
			CouponScheduleId = (this.ActionStatus == Constants.ACTION_STATUS_UPDATE) ? hfCouponScheduleId.Value : "",
			CouponScheduleName = tbCouponScheduleName.Text,
			TargetId = ddlTargetId.SelectedValue,
			TargetExtractFlg = ((ddlTargetId.SelectedValue != "") && cbTargetExtract.Checked) ? Constants.FLG_COUPONSCHEDULE_TARGET_EXTRACT_FLG_ON : Constants.FLG_COUPONSCHEDULE_TARGET_EXTRACT_FLG_OFF,
			CouponId = ddlCoupon.SelectedValue,
			PublishQuantity = trCouponQuantity.Visible ? tbCouponQuantity.Text : null,
			MailId = ddlMailTemp.SelectedValue,
			ValidFlg = cbValidFlg.Checked ? Constants.FLG_COUPONSCHEDULE_VALID_FLG_VALID : Constants.FLG_COUPONSCHEDULE_VALID_FLG_INVALID,
			LastChanged = this.LoginOperatorName,
		};
		if (string.IsNullOrEmpty(input.CouponId) == false)
		{
			input.Coupon = this.PublishCouponList.Where(coupon => coupon.CouponId == input.CouponId).First();
		}

		// 実行タイミング情報セット
		if (rbExecByManual.Checked)
		{
			// 手動実行
			input.ExecTiming = Constants.FLG_COUPONSCHEDULE_EXEC_TIMING_MANUAL;
			input.ScheduleKbn = "";
		}
		else if (rbExecBySchedule.Checked)
		{
			// スケジュール実行
			input.ExecTiming = Constants.FLG_COUPONSCHEDULE_EXEC_TIMING_SCHEDULE;

			if (rbScheRepeatDay.Checked)
			{
				// 日単位（毎日HH:mm:ssに実行）
				input.ScheduleKbn = Constants.FLG_COUPONSCHEDULE_SCHEDULE_KBN_DAY;
			}
			else if (rbScheRepeatWeek.Checked)
			{
				// 週単位（毎週ddd曜日HH:mm:ssに実行）
				input.ScheduleKbn = Constants.FLG_COUPONSCHEDULE_SCHEDULE_KBN_WEEK;
				input.ScheduleDayOfWeek = rblScheDayOfWeek.SelectedValue;
			}
			else if (rbScheRepeatMonth.Checked)
			{
				// 月単位（毎月dd日HH:mm:ssに実行）
				input.ScheduleKbn = Constants.FLG_COUPONSCHEDULE_SCHEDULE_KBN_MONTH;
				input.ScheduleDay = ddlScheDay.SelectedValue;
			}
			else if (rbScheRepeatOnce.Checked)
			{
				// 一回のみ（一回のみyyyy年MM月dd日HH:mm:ssに実行）
				input.ScheduleKbn = Constants.FLG_COUPONSCHEDULE_SCHEDULE_KBN_ONCE;
				if (string.IsNullOrEmpty(ucScheDateTimeOnce.HfStartDate.Value) == false)
				{
					var scheduleDate = DateTime.Parse(ucScheDateTimeOnce.HfStartDate.Value);
					input.ScheduleYear = scheduleDate.Year.ToString();
					input.ScheduleMonth = scheduleDate.Month.ToString();
					input.ScheduleDay = scheduleDate.Day.ToString();
				}
			}

			if (rbScheRepeatOnce.Checked)
			{
				if (string.IsNullOrEmpty(ucScheDateTimeOnce.HfStartTime.Value) == false)
				{
					var scheTime = DateTime.Parse(ucScheDateTimeOnce.HfStartTime.Value);
					input.ScheduleHour = scheTime.Hour.ToString();
					input.ScheduleMinute = scheTime.Minute.ToString();
					input.ScheduleSecond = scheTime.Second.ToString();
				}
			}
			else
			{
				input.ScheduleHour = ddlScheHour.SelectedValue;
				input.ScheduleMinute = ddlScheMinute.SelectedValue;
				input.ScheduleSecond = ddlScheSecond.SelectedValue;
			}

		}

		return input;
	}

	#region #発行枚数表示制御
	/// <summary>
	/// クーポン選択変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlCoupon_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetCouponQuantityDisplay();

		rbExecBySchedule.Attributes.Add("OnClick", "RefreshSchedule();");
		rbExecByManual.Attributes.Add("OnClick", "RefreshSchedule();");
		rbScheRepeatDay.Attributes.Add("OnClick", "RefreshSchedule();");
		rbScheRepeatWeek.Attributes.Add("OnClick", "RefreshSchedule();");
		rbScheRepeatMonth.Attributes.Add("OnClick", "RefreshSchedule();");
		rbScheRepeatOnce.Attributes.Add("OnClick", "RefreshSchedule();");
	}

	/// <summary>
	/// クーポン発行枚数表示制御
	/// </summary>
	protected void SetCouponQuantityDisplay()
	{
		var couponId = ddlCoupon.SelectedValue;

		// 選択なしは非表示
		if (string.IsNullOrEmpty(couponId))
		{
			trCouponQuantity.Visible = false;
			return;
		}

		// 選択状態で表示制御
		var selectedCoupon = this.PublishCouponList.Where(coupon => coupon.CouponId == couponId).First();
		switch (selectedCoupon.CouponType)
		{
			case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FOR_REGISTERED_USER:
			case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING_FOR_REGISTERED_USER:
			case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FOR_REGISTERED_USER:
			case Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FREESHIPPING_FOR_REGISTERED_USER:
				trCouponQuantity.Visible = true;
				break;
			default:
				trCouponQuantity.Visible = false;
				break;
		}
	}
	#endregion

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
		Session[Constants.SESSIONPARAM_KEY_COUPONSCHEDULE_INFO] = input;
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
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_COUPONSCHEDULE_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_CONFIRM);
		if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			url.AddParam(Constants.REQUEST_KEY_COUPONSCHEDULE_ID, hfCouponScheduleId.Value);
		}

		Response.Redirect(url.CreateUrl());
	}

	/// <summary>発行可能クーポン一覧</summary>
	protected CouponModel[] PublishCouponList
	{
		get { return (CouponModel[])ViewState["PublishCouponList"]; }
		set { ViewState["PublishCouponList"] = value; }
	}

	/// <summary>確認画面から戻ってきたか</summary>
	protected bool IsBackFromConfirm
	{
		get { return (Session[Constants.SESSION_KEY_PARAM_FOR_BACK] != null); }
	}
}