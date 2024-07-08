/*
=========================================================================================================
  Module      : 会員ランク変動ルール登録ページ処理(MemberRankRuleRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
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
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Util;
using w2.App.Common.Mail;
using w2.Domain.TargetList;

public partial class Form_MemberRankRule_MemberRankRuleRegister : BasePage
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
			//------------------------------------------------------
			// On Load イベント追加
			//------------------------------------------------------
			this.Master.OnLoadEvents += "RefleshMemberRankRuleSchedule();";

			//------------------------------------------------------
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			// アクションステータス
			string strActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, strActionStatus);

			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComponent();

			//------------------------------------------------------
			// 会員ランク変動ルール取得＆画面セット
			//------------------------------------------------------
			// 新規登録
			if (strActionStatus == Constants.ACTION_STATUS_INSERT)
			{
				// 初期値指定
				rbTargetExtractTypeDuring.Checked = true;		// 抽出条件集計期間指定：期間指定
				// ランク付与方法は「ランクアップ用」
				foreach (ListItem li in rblRankChangeType.Items)
				{
					li.Selected = (li.Value == Constants.FLG_MEMBERRANKRULE_RANK_CHANGE_TYPE_UP);
				}
			}
			// 編集・コピー新規
			else if ((strActionStatus == Constants.ACTION_STATUS_UPDATE)
				|| (strActionStatus == Constants.ACTION_STATUS_COPY_INSERT))
			{
				// 会員ランク変動ルール取得
				DataView dvMemberRankRule = null;
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("MemberRankRule", "GetMemberRankRule"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID, Request[Constants.REQUEST_KEY_MEMBERRANKRULE_ID]);

					dvMemberRankRule = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
				}

				// 画面セット
				if (dvMemberRankRule.Count != 0)
				{
					DataRowView drvMemberRankRule = dvMemberRankRule[0];

					hdnMemberRankRuleId.Value = (string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID];		// ランク付与ルールID(編集時に使用)
					tbMemberRankRuleName.Text = (string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_NAME];	// ランク付与ルール名
					// 抽出条件集計期間指定
					switch ((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TYPE])
					{
						case Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_TYPE_DURING:	// 期間指定
							rbTargetExtractTypeDuring.Checked = true;
							if (this.IsBackFromConfirm == false)
							{
								// 期間開始日
								if (drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_START] != DBNull.Value)
								{
									ucTargetExtractDatePeriod
										.SetStartDate(((DateTime)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_START]));
								}
								// 期間終了日
								if (drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_END] != DBNull.Value)
								{
									ucTargetExtractDatePeriod
										.SetEndDate(((DateTime)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_END]));
								}
							}
							break;

						case Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_TYPE_DAYSAGO:	// 前日指定
							rbTargetExtractTypeDaysAgo.Checked = true;
							tbTargetExtractDaysAgo.Text = (drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_DAYS_AGO]).ToString();
							break;
					}
					tbTargetExtractTotalPriceFrom.Text = StringUtility.ToEmpty(drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_FROM]).ToPriceString();	// 抽出条件合計購入金額範囲(From)
					tbTargetExtractTotalPriceTo.Text = StringUtility.ToEmpty(drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_TO]).ToPriceString();		// 抽出条件合計購入金額範囲(To)
					tbTargetExtractTotalCountFrom.Text = StringUtility.ToEmpty(drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_FROM]);	// 抽出条件合計購入回数範囲(From)
					tbTargetExtractTotalCountTo.Text = StringUtility.ToEmpty(drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_TO]);		// 抽出条件合計購入回数範囲(To)
					// 抽出時の旧ランク情報抽出判定
					cbTargetExtractOldRankFlg.Checked =
						((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG] == Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG_ON);
					// ランク付与方法
					foreach (ListItem li in rblRankChangeType.Items)
					{
						li.Selected = (li.Value == (string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_TYPE]);
					}
					// 指定付与ランクID
					foreach (ListItem li in ddlRankChangeRank.Items)
					{
						li.Selected = (li.Value == (string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_RANK_ID]);
					}
					// メールテンプレートID
					foreach (ListItem li in ddlMailTemp.Items)
					{
						li.Selected = (li.Value == (string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_MAIL_ID]);
					}

					rbExecByManual.Checked = ((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_EXEC_TIMING] == Constants.FLG_MEMBERRANKRULE_EXEC_TIMING_MANUAL);		// 手動実行
					rbExecBySchedule.Checked = ((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_EXEC_TIMING] == Constants.FLG_MEMBERRANKRULE_EXEC_TIMING_SCHEDULE);	// スケジュール実行
					// スケジュール実行
					if ((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_EXEC_TIMING] == Constants.FLG_MEMBERRANKRULE_EXEC_TIMING_SCHEDULE)
					{
						// スケジュール区分
						rbScheRepeatDay.Checked = ((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_KBN] == Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN_DAY);	// 日単位（毎日HH:mm:ssに実行
						rbScheRepeatWeek.Checked = ((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_KBN] == Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN_WEEK);	// 週単位（毎週ddd曜日HH:mm:ssに実行）
						rbScheRepeatMonth.Checked = ((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_KBN] == Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN_MONTH);// 月単位（毎月dd日HH:mm:ssに実行）
						rbScheRepeatOnce.Checked = ((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_KBN] == Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN__ONCE);	// 一回のみ（一回のみyyyy年MM月dd日HH:mm:ssに実行）

						if (rbScheRepeatWeek.Checked)
						{
							// スケジュール曜日
							foreach (ListItem li in rblScheDayOfWeek.Items)
							{
								li.Selected = (li.Value == ((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY_OF_WEEK]));
							}
						}
						else
						{
							rblScheDayOfWeek.SelectedIndex = 0;
						}

						if (rbScheRepeatOnce.Checked && (this.IsBackFromConfirm == false))
						{
							var scheDateTimeOnce = string.Format("{0}/{1}/{2} {3}:{4}:{5}",
								StringUtility.ToEmpty(drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_YEAR]),
								StringUtility.ToEmpty(drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MONTH]),
								StringUtility.ToEmpty(drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY]),
								StringUtility.ToEmpty(drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_HOUR]),
								StringUtility.ToEmpty(drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MINUTE]),
								StringUtility.ToEmpty(drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_SECOND]));

							var scheDateTimeOnceStart = new DateTime();
							if (DateTime.TryParse(scheDateTimeOnce, out scheDateTimeOnceStart))
							{
								ucScheDateTimeOnce.SetStartDate(scheDateTimeOnceStart);
							}
						}
						else
						{
							// スケジュール日程
							ucScheDateTime.Year =
								drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_YEAR].ToString();
							ucScheDateTime.Month = PadLeftZero2Letter(
								drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MONTH].ToString());
							ucScheDateTime.Day = PadLeftZero2Letter(
								drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY].ToString());
							ucScheDateTime.Hour = PadLeftZero2Letter(
								drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_HOUR].ToString());
							ucScheDateTime.Minute = PadLeftZero2Letter(
								drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MINUTE].ToString());
							ucScheDateTime.Second = PadLeftZero2Letter(
								drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_SECOND].ToString());
						}
					}
					// 手動実行（スケジュールはデフォルト値を設定しておく）
					else
					{
						rbScheRepeatOnce.Checked = true;
						rbScheRepeatDay.Checked = rbScheRepeatWeek.Checked = rbScheRepeatMonth.Checked = false;
						rblScheDayOfWeek.SelectedIndex = 0;
					}

					cbValidFlg.Checked = ((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_VALID_FLG] == Constants.FLG_MEMBERRANKRULE_VALID_FLG_VALID);	// 有効フラグ

					foreach (ListItem li in ddlTargetId.Items)
					{
						li.Selected = (li.Value == (string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_ID]);
					}
					foreach (ListItem li in ddlTargetId2.Items)
					{
						li.Selected = (li.Value == (string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_ID2]);
					}
					foreach (ListItem li in ddlTargetId3.Items)
					{
						li.Selected = (li.Value == (string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_ID3]);
					}
					foreach (ListItem li in ddlTargetId4.Items)
					{
						li.Selected = (li.Value == (string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_ID4]);
					}
					foreach (ListItem li in ddlTargetId5.Items)
					{
						li.Selected = (li.Value == (string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_ID5]);
					}

					cbTargetExtract.Checked =
						((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_FLG]
							== Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_FLG_ON);
					cbTargetExtract2.Checked =
						((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_FLG2]
							== Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_FLG_ON);
					cbTargetExtract3.Checked =
						((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_FLG3]
							== Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_FLG_ON);
					cbTargetExtract4.Checked =
						((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_FLG4]
							== Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_FLG_ON);
					cbTargetExtract5.Checked =
						((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_FLG5]
							== Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_FLG_ON);
				}
			}

			Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = null;
		}
		else
		{
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, (string)Request[Constants.REQUEST_KEY_ACTION_STATUS]);
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponent()
	{
		ucScheDateTimeOnce.SetStartDate(DateTime.Today);
		if (this.IsBackFromConfirm
			&& (Session[Constants.SESSIONPARAM_KEY_MEMBERRANKRULE_INFO] != null))
		{
			Hashtable htParam = (Hashtable)Session[Constants.SESSIONPARAM_KEY_MEMBERRANKRULE_INFO];

			var startTargetExtract = new DateTime();
			var endTargetExtract = new DateTime();

			if (DateTime.TryParse(StringUtility.ToEmpty(htParam[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_START]), out startTargetExtract)
				&& DateTime.TryParse(StringUtility.ToEmpty(htParam[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_END]), out endTargetExtract))
			{
				ucTargetExtractDatePeriod.SetPeriodDate(startTargetExtract, endTargetExtract);
			}

			var scheDateTimeOnce = string.Format("{0}/{1}/{2} {3}:{4}:{5}",
				StringUtility.ToEmpty(htParam[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_YEAR]),
				StringUtility.ToEmpty(htParam[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MONTH]),
				StringUtility.ToEmpty(htParam[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY]),
				StringUtility.ToEmpty(htParam[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_HOUR]),
				StringUtility.ToEmpty(htParam[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MINUTE]),
				StringUtility.ToEmpty(htParam[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_SECOND]));

			var scheDateTimeOnceStart = new DateTime();
			if (DateTime.TryParse(scheDateTimeOnce, out scheDateTimeOnceStart))
			{
				ucScheDateTimeOnce.SetStartDate(scheDateTimeOnceStart);
			}
		}
		//------------------------------------------------------
		// ランク更新方法ラジオボタンセット
		//------------------------------------------------------
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MEMBERRANKRULE, Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_TYPE))
		{
			rblRankChangeType.Items.Add(li);
		}

		//------------------------------------------------------
		// 指定付与ランクドロップダウンセット
		//------------------------------------------------------
		DataView dvMemberRanks = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatetment = new SqlStatement("MemberRank", "GetMemberRankListAll"))
		{
			dvMemberRanks = sqlStatetment.SelectSingleStatementWithOC(sqlAccessor);
		}
		foreach (DataRowView drv in dvMemberRanks)
		{
			ddlRankChangeRank.Items.Add(new ListItem((string)drv[Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME], (string)drv[Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID]));
		}

		//------------------------------------------------------
		// メールテンプレート
		//------------------------------------------------------
		ddlMailTemp.Items.Add(
			new ListItem(
				Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(
					"@@DispText.mail_template.not_send@@",
					Constants.GLOBAL_OPTION_ENABLE
						? Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE
						: ""),
				""));
		ddlMailTemp.Items.AddRange(GetMailTemplateUtility.GetMailTemplateForCustom(this.LoginOperatorShopId).Select(mail => new ListItem(mail.MailName, mail.MailId)).ToArray());
		
		//------------------------------------------------------
		// スケジュール
		//------------------------------------------------------
		// ラジオボタン系
		rbExecByManual.Text = ValueText.GetValueText(Constants.TABLE_MAILDISTSETTING, Constants.FIELD_MAILDISTSETTING_EXEC_TIMING, Constants.FLG_MAILDISTSETTING_EXEC_TIMING_MANUAL);
		//rbExecByAction.Text = ValueText.GetValueText(Constants.TABLE_MAILDISTSETTING, Constants.FIELD_MAILDISTSETTING_EXEC_TIMING, Constants.FLG_MAILDISTSETTING_EXEC_TIMING_ACTION);
		rbExecBySchedule.Text = ValueText.GetValueText(Constants.TABLE_MAILDISTSETTING, Constants.FIELD_MAILDISTSETTING_EXEC_TIMING, Constants.FLG_MAILDISTSETTING_EXEC_TIMING_SCHEDULE);
		rbExecByManual.Checked = true;

		rbScheRepeatDay.Text = ValueText.GetValueText(Constants.TABLE_MAILDISTSETTING, Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN, Constants.FLG_MAILDISTSETTING_SCHEDULE_KBN_DAY);
		rbScheRepeatWeek.Text = ValueText.GetValueText(Constants.TABLE_MAILDISTSETTING, Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN, Constants.FLG_MAILDISTSETTING_SCHEDULE_KBN_WEEK);
		rbScheRepeatMonth.Text = ValueText.GetValueText(Constants.TABLE_MAILDISTSETTING, Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN, Constants.FLG_MAILDISTSETTING_SCHEDULE_KBN_MONTH);
		rbScheRepeatOnce.Text = ValueText.GetValueText(Constants.TABLE_MAILDISTSETTING, Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN, Constants.FLG_MAILDISTSETTING_SCHEDULE_KBN_ONCE);
		rbScheRepeatOnce.Checked = true;

		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MAILDISTSETTING, Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY_OF_WEEK))
		{
			rblScheDayOfWeek.Items.Add(li);
		}
		if (rblScheDayOfWeek.Items.Count > 0)
		{
			rblScheDayOfWeek.Items[0].Selected = true;
		}

		// 実行タイミングの日付初期化
		ucScheDateTime.SetDate(DateTime.Now.Date);

		// ターゲットリストドロップダウンセット
		var targetLists = new TargetListService().GetAllValidTargetList(Constants.CONST_DEFAULT_DEPT_ID);
		foreach (var targetList in targetLists.Select((item, itemIndex) => new {item, itemIndex}))
		{
			(new List<DropDownList> { ddlTargetId, ddlTargetId2, ddlTargetId3, ddlTargetId4, ddlTargetId5 }).ForEach(ddl =>
			{
				ddl.Items.Add(new ListItem(string.Format("{0}({1})", targetList.item.TargetName, targetList.item.DataCount), targetList.item.TargetId));
				ddl.Items[targetList.itemIndex].Attributes["disable_extract"] = Constants.TARGET_LIST_IMPORT_TYPE_LIST.Contains(targetList.item.TargetType).ToString();
			});
		}
	}

	/// <summary>
	/// 確認ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 入力値格納
		//------------------------------------------------------
		// 基本情報セット
		Hashtable htInput = new Hashtable();
		if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE)
		{
			htInput.Add(Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID, hdnMemberRankRuleId.Value);				// ランク付与ルールID
		}
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_NAME, tbMemberRankRuleName.Text);				// ランク付与ルール名

		// 抽出条件集計期間指定(初期化)
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_START, null);
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_END, null);
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_DAYS_AGO, null);

		// 抽出条件集計期間指定
		if (rbTargetExtractTypeDuring.Checked)
		{
			// 期間指定
			htInput.Add(Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TYPE, Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_TYPE_DURING);
			// 開始日（年・月・日が空指定の場合は、設定しない）
			if (string.IsNullOrEmpty(ucTargetExtractDatePeriod.StartDateTimeString) == false)
			{
				htInput[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_START] = ucTargetExtractDatePeriod.StartDateTimeString;
			}
			// 終了日（年・月・日が空指定の場合は、設定しない）
			if (string.IsNullOrEmpty(ucTargetExtractDatePeriod.EndDateTimeString) == false)
			{
				htInput[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_END] = ucTargetExtractDatePeriod.EndDateTimeString;
			}
		}
		else if (rbTargetExtractTypeDaysAgo.Checked)
		{
			// 前日指定
			htInput.Add(Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TYPE, Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_TYPE_DAYSAGO);
			htInput[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_DAYS_AGO] = tbTargetExtractDaysAgo.Text;
		}

		// 抽出条件合計購入金額範囲(From)
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_FROM,
			(tbTargetExtractTotalPriceFrom.Text != "") ? tbTargetExtractTotalPriceFrom.Text : null);
		// 抽出条件合計購入金額範囲(To)
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_TO,
			(tbTargetExtractTotalPriceTo.Text != "") ? tbTargetExtractTotalPriceTo.Text : null);
		// 抽出条件合計購入回数範囲(From)
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_FROM,
			(tbTargetExtractTotalCountFrom.Text != "") ? tbTargetExtractTotalCountFrom.Text : null);
		// 抽出条件合計購入回数範囲(To)
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_TO,
			(tbTargetExtractTotalCountTo.Text != "") ? tbTargetExtractTotalCountTo.Text : null);
		// 入力チェック用(合計購入金額・回数は、少なくとも1つは入力されている必要がある)
		htInput.Add("target_extract_total_price_count_check",
			tbTargetExtractTotalPriceFrom.Text + tbTargetExtractTotalPriceTo.Text + tbTargetExtractTotalCountFrom.Text + tbTargetExtractTotalCountTo.Text);

		// 抽出時の旧ランク情報抽出判定
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG,
			cbTargetExtractOldRankFlg.Checked ? Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG_ON : Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG_OFF);
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_TYPE, rblRankChangeType.SelectedValue);				// ランク付与方法
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_RANK_ID, ddlRankChangeRank.SelectedValue);			// 指定付与ランクID
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_MAIL_ID, ddlMailTemp.SelectedValue);								// メールテンプレートID
		// 有効フラグ
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_VALID_FLG,
			cbValidFlg.Checked ? Constants.FLG_MEMBERRANKRULE_VALID_FLG_VALID : Constants.FLG_MEMBERRANKRULE_VALID_FLG_INVALID);
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_LAST_CHANGED, this.LoginOperatorName);							// 最終更新者

		// 実行タイミング情報セット(初期化)
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_EXEC_TIMING, "");
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_SCHEDULE_KBN, "");

		htInput.Add(Constants.FIELD_MEMBERRANKRULE_SCHEDULE_YEAR, null);
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MONTH, null);
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY, null);
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY_OF_WEEK, "");
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_SCHEDULE_HOUR, null);
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MINUTE, null);
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_SCHEDULE_SECOND, null);
		
		// 実行タイミング情報セット
		if (rbExecByManual.Checked)
		{
			// 手動実行
			htInput[Constants.FIELD_MEMBERRANKRULE_EXEC_TIMING] = Constants.FLG_MEMBERRANKRULE_EXEC_TIMING_MANUAL;
			htInput[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_KBN] = "";
		}
		else if (rbExecBySchedule.Checked)
		{
			// スケジュール実行
			htInput[Constants.FIELD_MEMBERRANKRULE_EXEC_TIMING] = Constants.FLG_MEMBERRANKRULE_EXEC_TIMING_SCHEDULE;

			if (rbScheRepeatDay.Checked)
			{
				// 日単位（毎日HH:mm:ssに実行）
				htInput[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_KBN] = Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN_DAY;
			}
			else if (rbScheRepeatWeek.Checked)
			{
				// 週単位（毎週ddd曜日HH:mm:ssに実行）
				htInput[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_KBN] = Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN_WEEK;
				htInput[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY_OF_WEEK] = rblScheDayOfWeek.SelectedValue;
			}
			else if (rbScheRepeatMonth.Checked)
			{
				// 月単位（毎月dd日HH:mm:ssに実行）
				htInput[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_KBN] = Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN_MONTH;
				htInput[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY] = ucScheDateTime.Day;
			}
			else if (rbScheRepeatOnce.Checked)
			{
				// 一回のみ（一回のみyyyy年MM月dd日HH:mm:ssに実行）
				htInput[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_KBN] = Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN__ONCE;
				htInput[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_YEAR] =
					DateTime.Parse(ucScheDateTimeOnce.HfStartDate.Value).Year.ToString();
				htInput[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MONTH] =
					DateTime.Parse(ucScheDateTimeOnce.HfStartDate.Value).Month.ToString();
				htInput[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY] =
					DateTime.Parse(ucScheDateTimeOnce.HfStartDate.Value).Day.ToString();

				// 日付チェック用
				htInput["schedule_date"] = ucScheDateTimeOnce.EndDateTimeString;
			}

			if (rbScheRepeatOnce.Checked)
			{
				htInput[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_HOUR] =
					DateTime.Parse(ucScheDateTimeOnce.HfStartTime.Value).Hour.ToString();
				htInput[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MINUTE] =
					DateTime.Parse(ucScheDateTimeOnce.HfStartTime.Value).Minute.ToString();
				htInput[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_SECOND] =
					DateTime.Parse(ucScheDateTimeOnce.HfStartTime.Value).Second.ToString();
			}
			else
			{
				htInput[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_HOUR] = ucScheDateTime.Hour;
				htInput[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MINUTE] = ucScheDateTime.Minute;
				htInput[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_SECOND] = ucScheDateTime.Second;
			}
		}

		htInput.Add(Constants.FIELD_MEMBERRANKRULE_TARGET_ID, ddlTargetId.SelectedValue);
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_TARGET_ID2, ddlTargetId2.SelectedValue);
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_TARGET_ID3, ddlTargetId3.SelectedValue);
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_TARGET_ID4, ddlTargetId4.SelectedValue);
		htInput.Add(Constants.FIELD_MEMBERRANKRULE_TARGET_ID5, ddlTargetId5.SelectedValue);
		htInput.Add(
			Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_FLG,
			((ddlTargetId.SelectedValue != "") && cbTargetExtract.Checked)
				? Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_FLG_ON
				: Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_FLG_OFF);
		htInput.Add(
			Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_FLG2,
			((ddlTargetId2.SelectedValue != "") && cbTargetExtract2.Checked)
				? Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_FLG_ON
				: Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_FLG_OFF);
		htInput.Add(
			Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_FLG3,
			((ddlTargetId3.SelectedValue != "") && cbTargetExtract3.Checked)
				? Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_FLG_ON
				: Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_FLG_OFF);
		htInput.Add(
			Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_FLG4,
			((ddlTargetId4.SelectedValue != "") && cbTargetExtract4.Checked)
				? Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_FLG_ON
				: Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_FLG_OFF);
		htInput.Add(
			Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_FLG5,
			((ddlTargetId5.SelectedValue != "") && cbTargetExtract5.Checked)
				? Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_FLG_ON
				: Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_FLG_OFF);

		// 入力チェック用
		htInput.Add("target_ids", ddlTargetId.SelectedValue + ddlTargetId2.SelectedValue + ddlTargetId3.SelectedValue + ddlTargetId4.SelectedValue + ddlTargetId5.SelectedValue);

		// パラメタをセッションへセット
		Session[Constants.SESSIONPARAM_KEY_MEMBERRANKRULE_INFO] = htInput;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = 1;

		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		var errorMessage = Validator.Validate("MemberRankRuleRegist", htInput);
		if ((htInput[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_START] != null)
			&& (htInput[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_END] != null)
			&& (Validator.CheckDateRange(htInput[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_START], htInput[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_END]) == false))
		{
			var message = ReplaceTagByLocaleId("@@DispText.member_rank_rule_extract.aggregation_period@@", Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE);
			errorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MEMBER_RANK_IRREGULAR_PARAMETER_ERROR).Replace("@@ 1 @@", message);
		}

		if ((htInput[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_FROM] != null)
			&& (htInput[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_TO] != null)
			&& (Validator.CheckNumberRange(htInput[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_FROM], htInput[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_TO]) == false))
		{
			var message = ReplaceTagByLocaleId("@@DispText.member_rank_rule_extract.total_price@@", Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE);
			errorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MEMBER_RANK_IRREGULAR_PARAMETER_ERROR).Replace("@@ 1 @@", message);
		}

		if ((htInput[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_FROM] != null)
			&& (htInput[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_TO] != null)
			&& Validator.CheckNumberRange(htInput[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_FROM], htInput[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_TO]) == false)
		{
			var message = ReplaceTagByLocaleId("@@DispText.member_rank_rule_extract.total_count@@", Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE);
			errorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MEMBER_RANK_IRREGULAR_PARAMETER_ERROR).Replace("@@ 1 @@", message);
		}
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// リダイレクト
		//------------------------------------------------------
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_RULE_CONFIRM);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_CONFIRM);
		if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE)
		{
			sbUrl.Append("&").Append(Constants.REQUEST_KEY_MEMBERRANKRULE_ID).Append("=").Append(HttpUtility.UrlEncode(hdnMemberRankRuleId.Value));
		}

		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>確認画面から戻ってきたか</summary>
	protected bool IsBackFromConfirm
	{
		get { return (Session[Constants.SESSION_KEY_PARAM_FOR_BACK] != null); }
	}
}