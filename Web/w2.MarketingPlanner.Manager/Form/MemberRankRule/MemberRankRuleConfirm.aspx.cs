/*
=========================================================================================================
  Module      : 会員ランク変動ルール確認ページ処理(MemberRankRuleConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.TargetList;
using w2.Common.Util;
using w2.Domain.MemberRankRule;
using w2.Domain.TaskSchedule.Helper;

public partial class Form_MemberRankRule_MemberRankRuleConfirm : BasePage
{
	private string m_strMemberRankRuleId = null;
	public string m_strActionStatus = null;
	private Hashtable m_htMemberRankRuleSet = null;

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
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			// アクションステータス
			m_strActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, m_strActionStatus);
			// ランク付与ルールID
			m_strMemberRankRuleId = (string)Request[Constants.REQUEST_KEY_MEMBERRANKRULE_ID];
			ViewState.Add(Constants.REQUEST_KEY_MEMBERRANKRULE_ID, m_strMemberRankRuleId);

			// ID表示設定
			trId.Visible = (m_strActionStatus == Constants.ACTION_STATUS_DETAIL);

			// コンポーネント初期化
			InitializeComponent(m_strActionStatus);
		}
		else
		{
			m_strActionStatus = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];
			m_strMemberRankRuleId = (string)ViewState[Constants.REQUEST_KEY_MEMBERRANKRULE_ID];
		}

		//------------------------------------------------------
		// データ取得
		//------------------------------------------------------
		switch (m_strActionStatus)
		{
			case Constants.ACTION_STATUS_DETAIL:	// 詳細表示
				m_htMemberRankRuleSet = GetMemberRankRule(m_strMemberRankRuleId);
				break;

			case Constants.ACTION_STATUS_CONFIRM:	// 確認表示
				m_htMemberRankRuleSet = (Hashtable)Session[Constants.SESSIONPARAM_KEY_MEMBERRANKRULE_INFO];
				break;
		}

		// メールテンプレート取得
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MailTemplate", "GetMailTemplate"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MAILTEMPLATE_SHOP_ID, this.LoginOperatorShopId);
			htInput.Add(Constants.FIELD_MAILTEMPLATE_MAIL_ID, m_htMemberRankRuleSet[Constants.FIELD_MAILTEMPLATE_MAIL_ID]);

			this.MailTemplate = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		var targets = new List<Hashtable>();
		for (var i = 1; i <= 5; i++)
		{
			var fieldNum = (i != 1) ? i.ToString() : "";
			if (((string)m_htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_TARGET_ID + fieldNum]).Length != 0)
			{
				var input = new Hashtable
				{
					{
						"target_name",
						TargetListUtility.GetTargetName(
							Constants.CONST_DEFAULT_DEPT_ID,
							(string)m_htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_TARGET_ID + fieldNum])
					},
					{
						"target_extract_flg",
						(string)m_htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_FLG + fieldNum]
					},
				};
				targets.Add(input);
			}
		}
		rTargetLists.DataSource = targets;
		rTargetLists.DataBind();
	}

	/// <summary>
	/// 表示ための金額変換
	/// </summary>
	/// <param name="price">金額</param>
	/// <returns>金額</returns>
	protected string ChangePriceForDisplay(string price)
	{
		var localId = Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId;
		var result = CurrencyManager.IsJapanKeyCurrencyCode
			? StringUtility.ToPrice(price, localId, "{0:#,##0}")
			: price.ToPriceString(true);
		return result;
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	/// <param name="strActionKbn"></param>
	private void InitializeComponent(string strActionKbn)
	{
		// 削除ボタン
		btnDelete.Visible = btnDelete2.Visible = (strActionKbn == Constants.ACTION_STATUS_DETAIL);

		// 編集ボタン
		btnEdit.Visible = btnEdit2.Visible = (strActionKbn == Constants.ACTION_STATUS_DETAIL);

		// コピー新規登録ボタン
		btnCopyInsert.Visible = btnCopyInsert2.Visible = (strActionKbn == Constants.ACTION_STATUS_DETAIL);

		// 登録ボタン
		btnInsert.Visible = btnInsert2.Visible = ((strActionKbn == Constants.ACTION_STATUS_CONFIRM) && (m_strMemberRankRuleId == null));

		// 更新ボタン
		btnUpdate.Visible = btnUpdate2.Visible = ((strActionKbn == Constants.ACTION_STATUS_CONFIRM) && (m_strMemberRankRuleId != null));

		// 会員ランク付与ステータス情報など
		dvMemberRankRuleStatusInfo.Visible = (strActionKbn == Constants.ACTION_STATUS_DETAIL);
	}

	/// <summary>
	/// 会員ランク変動ルール取得
	/// </summary>
	/// <param name="strMemberRankRuleId">ランク付与ルールID</param>
	/// <returns>会員ランク変動ルール</returns>
	private Hashtable GetMemberRankRule(string strMemberRankRuleId)
	{
		Hashtable htResult = new Hashtable();

		//------------------------------------------------------
		// 会員ランク変動ルール取得
		//------------------------------------------------------
		DataView dvMemberRankRule = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MemberRankRule", "GetMemberRankRule"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID, strMemberRankRuleId);

			dvMemberRankRule = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}
		foreach (DataColumn dc in dvMemberRankRule.Table.Columns)
		{
			htResult.Add(dc.ColumnName, dvMemberRankRule[0][dc.ColumnName]);
		}

		//------------------------------------------------------
		// 最終付与人数取得
		//------------------------------------------------------
		DataView dvTaskSchedule = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "GetTaskScheduleLastProgress"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_CHANGE_MEMBER_RANK);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, strMemberRankRuleId);

			dvTaskSchedule = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}
		htResult.Add(Constants.FIELD_TASKSCHEDULE_PROGRESS, (dvTaskSchedule.Count > 0) ? dvTaskSchedule[0][Constants.FIELD_TASKSCHEDULE_PROGRESS] : DBNull.Value);

		return htResult;
	}

	/// <summary>
	/// スケジュール文言作成
	/// </summary>
	/// <param name="htMemberRankRuleSet"></param>
	/// <returns></returns>
	protected string GetScheduleString(Hashtable htMemberRankRuleSet)
	{
		return GetScheduleString(
			(string)htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_EXEC_TIMING],
			(string)htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_KBN],
			(string)htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY_OF_WEEK],
			htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_YEAR],
			htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MONTH],
			htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY],
			htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_HOUR],
			htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MINUTE],
			htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_SECOND]);
	}
	/// <summary>
	/// スケジュール文言作成
	/// </summary>
	/// <param name="strExecTiming"></param>
	/// <param name="strScheKbn"></param>
	/// <param name="strWeek"></param>
	/// <param name="objYear"></param>
	/// <param name="objMonth"></param>
	/// <param name="objDay"></param>
	/// <param name="objHour"></param>
	/// <param name="objMinute"></param>
	/// <param name="objSecond"></param>
	/// <returns></returns>
	protected string GetScheduleString(
		string strExecTiming,
		string strScheKbn,
		string strWeek,
		object objYear,
		object objMonth,
		object objDay,
		object objHour,
		object objMinute,
		object objSecond)
	{
		StringBuilder sbSchedule = new StringBuilder();
		switch (strExecTiming)
		{
			case Constants.FLG_MEMBERRANKRULE_EXEC_TIMING_MANUAL:		// 手動実行
				sbSchedule.Append(ValueText.GetValueText(Constants.TABLE_MEMBERRANKRULE, Constants.FIELD_MEMBERRANKRULE_EXEC_TIMING, strExecTiming));
				break;

			case Constants.FLG_MEMBERRANKRULE_EXEC_TIMING_SCHEDULE:		// スケジュール実行
				break;
		}

		if (strExecTiming == Constants.FLG_MEMBERRANKRULE_EXEC_TIMING_SCHEDULE)
		{
			switch (strScheKbn)
			{
				case Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN_DAY:		// 日単位（毎日HH:mm:ssに実行）
					sbSchedule.Append(ReplaceTag("@@DispText.schedule_kbn.Day@@"));
					break;

				case Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN_WEEK:	// 週単位（毎週ddd曜日HH:mm:ssに実行）
					sbSchedule.Append(ReplaceTag("@@DispText.schedule_kbn.Week@@"));
					sbSchedule.Append(ValueText.GetValueText(Constants.TABLE_MEMBERRANKRULE, Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY_OF_WEEK, strWeek) + ReplaceTag("@@DispText.schedule_unit.DayOfWeek@@"));
					sbSchedule.Append(" ");
					break;

				case Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN_MONTH:	// 月単位（毎月dd日HH:mm:ssに実行）
					sbSchedule.Append(ReplaceTag("@@DispText.schedule_kbn.Month@@"));
					sbSchedule.Append(objDay).Append(ReplaceTag("@@DispText.schedule_unit.Day@@"));
					sbSchedule.Append(" ");
					break;

				case Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN__ONCE:	// 一回のみ（一回のみyyyy年MM月dd日HH:mm:ssに実行）
					sbSchedule.Append(ReplaceTag("@@DispText.schedule_kbn.Once@@"));
					sbSchedule.Append(
						DateTimeUtility.ToStringForManager(
							string.Format("{0}/{1}/{2} {3}:{4}:{5}", objYear, objMonth, objDay, objHour, objMinute, objSecond),
							DateTimeUtility.FormatType.LongDateHourMinuteSecond1Letter));
					break;
			}
			if (strScheKbn != Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN__ONCE)
			{
				sbSchedule.Append(
					DateTimeUtility.ToStringForManager(
						string.Format("{0}:{1}:{2}", objHour, objMinute, objSecond),
						DateTimeUtility.FormatType.HourMinuteSecond1Letter));
			}
		}

		return StringUtility.ChangeToBrTag(WebSanitizer.HtmlEncode(sbSchedule.ToString()));
	}

	/// <summary>
	/// タスクスケジュール作成判定（過去であれば作成したくない）
	/// </summary>
	/// <param name="htMemberRankRuleSet"></param>
	/// <returns></returns>
	private bool CanInsertTaskSchedule(Hashtable htMemberRankRuleSet)
	{
		if ((string)htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_EXEC_TIMING] == Constants.FLG_MEMBERRANKRULE_EXEC_TIMING_SCHEDULE)
		{
			if ((string)htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_KBN] == Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN__ONCE)
			{
				DateTime dtSchedule = new DateTime(
					int.Parse(htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_YEAR].ToString()),
					int.Parse(htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MONTH].ToString()),
					int.Parse(htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY].ToString()),
					int.Parse(htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_HOUR].ToString()),
					int.Parse(htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MINUTE].ToString()),
					int.Parse(htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_SECOND].ToString()));

				if (DateTime.Now.CompareTo(dtSchedule) > 0)
				{
					return false;
				}
			}
		}

		return true;
	}

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = null;

		// 編集画面へ遷移
		StringBuilder sbUrl = new StringBuilder(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_RULE_REGISTER);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_MEMBERRANKRULE_ID).Append("=").Append(HttpUtility.UrlEncode(m_strMemberRankRuleId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(Constants.ACTION_STATUS_UPDATE));

		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	/// コピー新規登録クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		// 編集画面へ遷移
		StringBuilder sbUrl = new StringBuilder(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_RULE_REGISTER);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_MEMBERRANKRULE_ID).Append("=").Append(HttpUtility.UrlEncode(m_strMemberRankRuleId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(Constants.ACTION_STATUS_COPY_INSERT));

		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			//------------------------------------------------------
			// スケジュール削除
			//------------------------------------------------------
			using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "DeleteTaskSchedule"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.LoginOperatorDeptId);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_CHANGE_MEMBER_RANK);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, m_htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID]);

				sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}

			//------------------------------------------------------
			// 会員ランク変動ルール削除
			//------------------------------------------------------
			using (SqlStatement sqlStatement = new SqlStatement("MemberRankRule", "DeleteMemberRankRule"))
			{
				sqlStatement.ExecStatementWithOC(sqlAccessor, m_htMemberRankRuleSet);
			}
		}

		//------------------------------------------------------
		// 一覧画面へ遷移
		//------------------------------------------------------
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_RULE_LIST);
	}

	/// <summary>
	/// 登録する
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		// Create new member rank rule ID
		string strMemberRankRuleId = NumberingUtility.CreateKeyId(this.LoginOperatorShopId, Constants.NUMBER_KEY_MP_MEMBERRANKRULE_ID, 10);

		var memberRankRuleInput = new MemberRankRuleInput(new MemberRankRuleModel());
		memberRankRuleInput.SetSchedule(m_htMemberRankRuleSet);
		var taskScheduleRule = new TaskScheduleRule(memberRankRuleInput.CreateModel());
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			try
			{
				//------------------------------------------------------
				// 会員ランク変動ルール登録
				//------------------------------------------------------
				m_htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID] = strMemberRankRuleId;

				using (SqlStatement sqlStatement = new SqlStatement("MemberRankRule", "InsertMemberRankRule"))
				{
					sqlStatement.ExecStatement(sqlAccessor, m_htMemberRankRuleSet);
				}

				//------------------------------------------------------
				// スケジュール追加（スケジュールが未来or繰り返し かつ 有効）
				//------------------------------------------------------
				if (CanInsertTaskSchedule(m_htMemberRankRuleSet)
					&& ((string)m_htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_VALID_FLG] == Constants.FLG_MEMBERRANKRULE_VALID_FLG_VALID)
					&& taskScheduleRule.CheckCanCreateFirstTaskScheduleForScheduleMonth())
				{
					using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "InsertFirstTaskSchedule"))
					{
						m_htMemberRankRuleSet[Constants.FIELD_TASKSCHEDULE_DEPT_ID] = this.LoginOperatorDeptId;
						m_htMemberRankRuleSet[Constants.FIELD_TASKSCHEDULE_ACTION_KBN] = Constants.FLG_TASKSCHEDULE_ACTION_KBN_CHANGE_MEMBER_RANK;
						m_htMemberRankRuleSet[Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID] = strMemberRankRuleId;
						m_htMemberRankRuleSet[Constants.FIELD_TASKSCHEDULE_LAST_CHANGED] = this.LoginOperatorName;

						int iUpdate = sqlStatement.ExecStatement(sqlAccessor, m_htMemberRankRuleSet);
					}
				}

				sqlAccessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				sqlAccessor.RollbackTransaction();
				throw ex;
			}
		}

		//------------------------------------------------------
		// 一覧画面へ遷移
		//------------------------------------------------------
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_RULE_LIST);
	}

	/// <summary>
	/// 更新する
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		var memberRankRuleInput = new MemberRankRuleInput(new MemberRankRuleModel());
		memberRankRuleInput.SetSchedule(m_htMemberRankRuleSet);
		var taskScheduleRule = new TaskScheduleRule(memberRankRuleInput.CreateModel());
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			try
			{
				//------------------------------------------------------
				// 未実行スケジュール削除
				//------------------------------------------------------
				using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "DeleteTaskScheduleUnexecuted"))
				{
					Hashtable htParam = new Hashtable();
					htParam[Constants.FIELD_TASKSCHEDULE_DEPT_ID] = this.LoginOperatorDeptId;
					htParam[Constants.FIELD_TASKSCHEDULE_ACTION_KBN] = Constants.FLG_TASKSCHEDULE_ACTION_KBN_CHANGE_MEMBER_RANK;
					htParam[Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID] = m_htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID];

					sqlStatement.ExecStatement(sqlAccessor, htParam);
				}

				//------------------------------------------------------
				// 会員ランク変動ルール更新
				//------------------------------------------------------
				using (SqlStatement sqlStatement = new SqlStatement("MemberRankRule", "UpdateMemberRankRule"))
				{
					sqlStatement.ExecStatement(sqlAccessor, m_htMemberRankRuleSet);
				}

				//------------------------------------------------------
				// スケジュール追加（スケジュールが未来or繰り返し かつ 有効）
				//------------------------------------------------------
				if (CanInsertTaskSchedule(m_htMemberRankRuleSet)
					&& ((string)m_htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_VALID_FLG] == Constants.FLG_MEMBERRANKRULE_VALID_FLG_VALID)
					&& taskScheduleRule.CheckCanCreateFirstTaskScheduleForScheduleMonth())
				{
					using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "InsertFirstTaskSchedule"))
					{
						m_htMemberRankRuleSet[Constants.FIELD_TASKSCHEDULE_DEPT_ID] = this.LoginOperatorDeptId;
						m_htMemberRankRuleSet[Constants.FIELD_TASKSCHEDULE_ACTION_KBN] = Constants.FLG_TASKSCHEDULE_ACTION_KBN_CHANGE_MEMBER_RANK;
						m_htMemberRankRuleSet[Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID] = m_htMemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID];
						m_htMemberRankRuleSet[Constants.FIELD_TASKSCHEDULE_LAST_CHANGED] = this.LoginOperatorName;

						int iUpdate = sqlStatement.ExecStatement(sqlAccessor, m_htMemberRankRuleSet);
					}
				}

				// トランザクションコミット
				sqlAccessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				sqlAccessor.RollbackTransaction();
				throw ex;
			}
		}

		//------------------------------------------------------
		// 一覧画面へ遷移
		//------------------------------------------------------
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_RULE_LIST);
	}

	/// <summary>ランク付与ルールID</summary>
	protected string MemberRankRuleId
	{
		get { return m_strMemberRankRuleId; }
	}
	/// <summary>会員ランク変動ルール</summary>
	protected Hashtable MemberRankRuleSet
	{
		get { return m_htMemberRankRuleSet; }
	}
	/// <summary>集計期間内の合計購入金額（～円以上）</summary>
	protected string MemberRankRuleTargetExtractTotalPriceFrom
	{
		get
		{
			return StringUtility.ToEmpty(
				this.MemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_FROM]);
		}
	}
	/// <summary>集計期間内の合計購入金額（～円以下）</summary>
	protected string MemberRankRuleTargetExtractTotalPriceTo
	{
		get
		{
			return StringUtility.ToEmpty(
				this.MemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_TO]);
		}
	}
	/// <summary>集計期間内の合計購入回数（～回以上）</summary>
	protected string MemberRankRuleTargetExtractTotalCountFrom
	{
		get
		{
			return StringUtility.ToEmpty(
				this.MemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_FROM]);
		}
	}
	/// <summary>集計期間内の合計購入回数（～回以下）</summary>
	protected string MemberRankRuleTargetExtractTotalCountTo
	{
		get
		{
			return StringUtility.ToEmpty(
				this.MemberRankRuleSet[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_TO]);
		}
	}
	/// <summary>メールテンプレート</summary>
	protected DataView MailTemplate { get; set; }
}