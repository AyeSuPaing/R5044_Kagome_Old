/*
=========================================================================================================
  Module      : メール配信設定確認ページ処理(MailDistSettingConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using w2.App.Common.TargetList;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.MailDistSentUser;
using w2.Domain.MailDistSetting;
using w2.Domain.TaskSchedule.Helper;

public partial class Form_MailDistSetting_MailDistSettingConfirm : BasePage
{
	private string m_strMailDistId = null;
	public string m_strActionStatus = null;
	private Hashtable m_htMailDistSet = null;
	private int m_dispAddressCount = 10;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			m_strActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, m_strActionStatus);

			// ID表示設定
			trId.Visible = (m_strActionStatus == Constants.ACTION_STATUS_DETAIL);
			m_strMailDistId = (string)Request[Constants.REQUEST_KEY_MAILDIST_ID];
			ViewState.Add(Constants.REQUEST_KEY_MAILDIST_ID, m_strMailDistId);

			// コンポーネント初期化
			InitializeComponent(m_strActionStatus);
		}
		else
		{
			m_strActionStatus = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];
			m_strMailDistId = (string)ViewState[Constants.REQUEST_KEY_MAILDIST_ID];
		}

		//------------------------------------------------------
		// データ取得
		//------------------------------------------------------
		switch (m_strActionStatus)
		{
			case Constants.ACTION_STATUS_DETAIL:
				m_htMailDistSet = GetMailDistSetting(m_strMailDistId);
				break;

			case Constants.ACTION_STATUS_CONFIRM:
				m_htMailDistSet = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
				break;
		}

		// Check data MailDistSetting
		if (m_htMailDistSet == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// データ表示
		//------------------------------------------------------
		lMailDistId.Text = WebSanitizer.HtmlEncode(m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID]);
		lMailDistName.Text = WebSanitizer.HtmlEncode(m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_MAILDIST_NAME]);
		lMailText.Text = WebSanitizer.HtmlEncode(m_htMailDistSet[Constants.FIELD_MAILDISTTEXT_MAILTEXT_NAME]);

		List<Hashtable> lTargets = new List<Hashtable>();
		for (int iLoop = 1; iLoop <= 5; iLoop++)
		{
			string strFieldNum = (iLoop != 1) ? iLoop.ToString() : "";
			if (((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_TARGET_ID + strFieldNum]).Length != 0)
			{
				Hashtable htInput = new Hashtable();
				htInput.Add("target_name", GetTargetName((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_TARGET_ID + strFieldNum]));
				htInput.Add("target_extract_flg", GetTargetName((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG + strFieldNum]));

				lTargets.Add(htInput);
			}
		}
		rTargetLists.DataSource = lTargets;
		rTargetLists.DataBind();

		this.ExceptLists = ((string)m_htMailDistSet["except_list"]).Split(',');

		if (m_strActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			lStatus.Text = WebSanitizer.HtmlEncode(
				ValueText.GetValueText(
					Constants.TABLE_MAILDISTSETTING,
					Constants.FIELD_MAILDISTSETTING_STATUS,
					(string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_STATUS]));
			lLastCount.Text = WebSanitizer.HtmlEncode(
				(m_htMailDistSet[Constants.FIELD_TASKSCHEDULE_PROGRESS] != DBNull.Value)
					? m_htMailDistSet[Constants.FIELD_TASKSCHEDULE_PROGRESS]
					: "-");
			lLastErrorExceptCount.Text = WebSanitizer.HtmlEncode(
				(m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_LAST_DIST_DATE] != DBNull.Value)
					? m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_LAST_ERROREXCEPT_COUNT]
					: "-");
			lLastDuplicateExceptCount.Text = WebSanitizer.HtmlEncode(
				(m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_LAST_DUPLICATE_EXCEPT_COUNT] != DBNull.Value)
					? m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_LAST_DUPLICATE_EXCEPT_COUNT]
					: "-");
			lLastDistDate.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_LAST_DIST_DATE],
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
					"-"));
		}
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
		btnInsert.Visible = btnInsert2.Visible = ((strActionKbn == Constants.ACTION_STATUS_CONFIRM) && (m_strMailDistId == null));

		// 更新ボタン
		btnUpdate.Visible = btnUpdate2.Visible = ((strActionKbn == Constants.ACTION_STATUS_CONFIRM) && (m_strMailDistId != null));

		// 配信ステータス情報など
		dvMailDistStatusInfo.Visible = (strActionKbn == Constants.ACTION_STATUS_DETAIL);
	}

	/// <summary>
	/// メール配信設定取得
	/// </summary>
	/// <param name="strMailDistId">メール配信設定ID</param>
	/// <returns>メール配信設定</returns>
	private Hashtable GetMailDistSetting(string strMailDistId)
	{
		Hashtable htResult = new Hashtable();

		//------------------------------------------------------
		// メール配信設定情報取得
		//------------------------------------------------------
		DataView dvMailDistSetting = null;
		DataView dvMailDistExceptList = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MailDistSetting", "GetMailDistSetting"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MAILDISTSETTING_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_MAILDISTSETTING_MAILDIST_ID, Request[Constants.REQUEST_KEY_MAILDIST_ID]);

			DataSet dsMailDistSetting = sqlStatement.SelectStatementWithOC(sqlAccessor, htInput);
			dvMailDistSetting = dsMailDistSetting.Tables[0].DefaultView;
			dvMailDistExceptList = dsMailDistSetting.Tables[1].DefaultView;
		}

		if (dvMailDistSetting.Count == 0) return null;

		foreach (DataColumn dc in dvMailDistSetting.Table.Columns)
		{
			htResult.Add(dc.ColumnName, dvMailDistSetting[0][dc.ColumnName]);
		}

		StringBuilder sbExceptList = new StringBuilder();
		foreach (DataRowView drv in dvMailDistExceptList)
		{
			sbExceptList.Append((sbExceptList.Length != 0) ? "," : "");
			sbExceptList.Append(drv[Constants.FIELD_MAILDISTEXCEPTLIST_MAIL_ADDR]);
		}
		htResult.Add("except_list", sbExceptList.ToString());

		//------------------------------------------------------
		// 最終配信件数取得
		//------------------------------------------------------
		DataView dvTaskSchedule = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "GetTaskScheduleLastProgress"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MAILDISTSETTING_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_MAILDISTSETTING_MAILDIST_ID, Request[Constants.REQUEST_KEY_MAILDIST_ID]);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_MAIL_DIST);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, m_strMailDistId);

			dvTaskSchedule = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}
		htResult.Add(Constants.FIELD_TASKSCHEDULE_PROGRESS, (dvTaskSchedule.Count > 0) ? dvTaskSchedule[0][Constants.FIELD_TASKSCHEDULE_PROGRESS] : DBNull.Value);


		return htResult;
	}

	/// <summary>
	/// スケジュール文言作成
	/// </summary>
	/// <param name="htMailDistSet"></param>
	/// <returns></returns>
	protected string GetScheduleString(Hashtable htMailDistSet)
	{
		return GetScheduleString(
			(string)htMailDistSet[Constants.FIELD_MAILDISTSETTING_EXEC_TIMING],
			(string)htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN],
			(string)htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY_OF_WEEK],
			htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_YEAR],
			htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MONTH],
			htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY],
			htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_HOUR],
			htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MINUTE],
			htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_SECOND]);
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
			case Constants.FLG_TARGETLIST_EXEC_TIMING_MANUAL:
				sbSchedule.Append(ValueText.GetValueText(Constants.TABLE_TARGETLIST, Constants.FIELD_TARGETLIST_EXEC_TIMING, strExecTiming));
				break;

			case Constants.FLG_TARGETLIST_EXEC_TIMING_SCHEDULE:
				break;
		}

		if (strExecTiming == Constants.FLG_TARGETLIST_EXEC_TIMING_SCHEDULE)
		{
			switch (strScheKbn)
			{
				case Constants.FLG_TARGETLIST_SCHEDULE_KBN_DAY:
					sbSchedule.Append(ReplaceTag("@@DispText.schedule_kbn.Day@@"));
					break;

				case Constants.FLG_TARGETLIST_SCHEDULE_KBN_WEEK:
					sbSchedule.Append(ReplaceTag("@@DispText.schedule_kbn.Week@@"));
					sbSchedule.Append(ValueText.GetValueText(Constants.TABLE_TARGETLIST, Constants.FIELD_TARGETLIST_SCHEDULE_DAY_OF_WEEK, strWeek) + ReplaceTag("@@DispText.schedule_unit.DayOfWeek@@"));
					sbSchedule.Append(" ");
					break;

				case Constants.FLG_TARGETLIST_SCHEDULE_KBN_MONTH:
					sbSchedule.Append(ReplaceTag("@@DispText.schedule_kbn.Month@@"));
					sbSchedule.Append(objDay).Append(ReplaceTag("@@DispText.schedule_unit.Day@@"));
					sbSchedule.Append(" ");
					break;

				case Constants.FLG_TARGETLIST_SCHEDULE_KBN_ONCE:
					sbSchedule.Append(ReplaceTag("@@DispText.schedule_kbn.Once@@"));
					sbSchedule.Append(DateTimeUtility.ToStringForManager(
						string.Format("{0}/{1}/{2} {3}:{4}:{5}", objYear, objMonth, objDay, objHour, objMinute, objSecond),
						DateTimeUtility.FormatType.LongDateHourMinuteSecond1Letter));
					break;
			}

			if (strScheKbn != Constants.FLG_TARGETLIST_SCHEDULE_KBN_ONCE)
			{
				sbSchedule.Append(DateTimeUtility.ToStringForManager(
					string.Format("{0}:{1}:{2}", objHour, objMinute, objSecond),
					DateTimeUtility.FormatType.HourMinuteSecond1Letter));
			}
		}

		return StringUtility.ChangeToBrTag(WebSanitizer.HtmlEncode(sbSchedule.ToString()));
	}

	/// <summary>
	/// タスクスケジュール作成判定（過去であれば作成したくない）
	/// </summary>
	/// <param name="htMailDistSet"></param>
	/// <returns></returns>
	private bool CanInsertTaskSchedule(Hashtable htMailDistSet)
	{
		if ((string)htMailDistSet[Constants.FIELD_MAILDISTSETTING_EXEC_TIMING] == Constants.FLG_TARGETLIST_EXEC_TIMING_SCHEDULE)
		{
			if ((string)htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN] == Constants.FLG_TARGETLIST_SCHEDULE_KBN_ONCE)
			{
				DateTime dtSchedule = new DateTime(
					int.Parse(htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_YEAR].ToString()),
					int.Parse(htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MONTH].ToString()),
					int.Parse(htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY].ToString()),
					int.Parse(htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_HOUR].ToString()),
					int.Parse(htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MINUTE].ToString()),
					int.Parse(htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_SECOND].ToString()));

				if (DateTime.Now.CompareTo(dtSchedule) > 0)
				{
					return false;
				}
			}
		}

		return true;
	}

	/// <summary>
	/// ターゲット名取得
	/// </summary>
	/// <param name="strTargetId">ターゲットID</param>
	/// <returns>ターゲット名取得</returns>
	protected string GetTargetName(string strTargetId)
	{
		string strResult = null;

		if (strTargetId != "")
		{
			// ターゲット情報取得
			DataView dvTargetList = TargetListUtility.GetTargetList(this.LoginOperatorDeptId, strTargetId);
			if (dvTargetList.Count != 0) strResult = (string)dvTargetList[0][Constants.FIELD_TARGETLIST_TARGET_NAME];
		}

		return strResult;
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
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILDIST_SETTING_REGISTER)
			.AddParam(Constants.REQUEST_KEY_MAILDIST_ID, m_strMailDistId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE)
			.CreateUrl();

		Response.Redirect(url);
	}

	/// <summary>
	/// コピー新規登録クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		// 編集画面へ遷移
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILDIST_SETTING_REGISTER)
			.AddParam(Constants.REQUEST_KEY_MAILDIST_ID, m_strMailDistId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_COPY_INSERT)
			.CreateUrl();

		Response.Redirect(url);
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
				htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_MAIL_DIST);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID]);

				sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}

			//------------------------------------------------------
			// メール配信設定削除フラグ更新
			//------------------------------------------------------
			using (SqlStatement sqlStatement = new SqlStatement("MailDistSetting", "UpdateMailDistSettingDelFlg"))
			{
				sqlStatement.ExecStatementWithOC(sqlAccessor, m_htMailDistSet);
			}

			//------------------------------------------------------
			// メール配信除外リスト削除
			//------------------------------------------------------
			using (SqlStatement sqlStatement = new SqlStatement("MailDistSetting", "DeleteMailDistExceptListAll"))
			{
				sqlStatement.ExecStatementWithOC(sqlAccessor, m_htMailDistSet);
			}

			// 配信済ユーザを削除
			new MailDistSentUserService().DeleteByMaildistId(m_strMailDistId);
		}

		//------------------------------------------------------
		// 一覧画面へ遷移
		//------------------------------------------------------
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILDIST_SETTING_LIST);
	}

	/// <summary>
	/// 登録する
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		// Create new mail dist setting ID
		string strMailDistId = NumberingUtility.CreateKeyId(this.LoginOperatorShopId, Constants.NUMBER_KEY_MP_MAILDISTSETTING_ID, 10);

		var mailDistSettingInput = new MailDistSettingInput(new MailDistSettingModel());
		mailDistSettingInput.SetSchedule(m_htMailDistSet);
		var taskScheduleRule = new TaskScheduleRule(mailDistSettingInput.CreateModel());
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			try
			{
				//------------------------------------------------------
				// メール配信設定インサート
				//------------------------------------------------------
				m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID] = strMailDistId;

				using (SqlStatement sqlStatement = new SqlStatement("MailDistSetting", "InsertMailDistSetting"))
				{
					sqlStatement.ExecStatement(sqlAccessor, m_htMailDistSet);
				}

				//------------------------------------------------------
				// メール配信除外リストインサート
				//------------------------------------------------------
				string[] strExceptLists = ((string)m_htMailDistSet["except_list"]).Split(',');
				using (SqlStatement sqlStatement = new SqlStatement("MailDistSetting", "InsertMailDistExceptList"))
				{
					Hashtable htInput = new Hashtable();
					htInput[Constants.FIELD_MAILDISTEXCEPTLIST_DEPT_ID] = this.LoginOperatorDeptId;
					htInput[Constants.FIELD_MAILDISTEXCEPTLIST_MAILDIST_ID] = strMailDistId;

					Hashtable htTmp = new Hashtable();
					foreach (string strExcept in strExceptLists)
					{
						if ((strExcept.Length != 0) && (htTmp.ContainsKey(strExcept) == false))
						{
							htInput[Constants.FIELD_MAILDISTEXCEPTLIST_MAIL_ADDR] = strExcept;
							sqlStatement.ExecStatement(sqlAccessor, htInput);

							htTmp[strExcept] = null;
						}
					}
				}

				//------------------------------------------------------
				// スケジュール追加（スケジュールが未来or繰り返し かつ 有効）
				//------------------------------------------------------
				if (CanInsertTaskSchedule(m_htMailDistSet)
					&& ((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_VALID_FLG] == Constants.FLG_MAILDISTSETTING_VALID_FLG_VALID)
					&& (taskScheduleRule.CheckCanCreateFirstTaskScheduleForScheduleMonth()))
				{
					using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "InsertFirstTaskSchedule"))
					{
						m_htMailDistSet[Constants.FIELD_TASKSCHEDULE_ACTION_KBN] = Constants.FLG_TASKSCHEDULE_ACTION_KBN_MAIL_DIST;
						m_htMailDistSet[Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID] = strMailDistId;
						m_htMailDistSet[Constants.FIELD_TASKSCHEDULE_LAST_CHANGED] = this.LoginOperatorName;

						int iUpdate = sqlStatement.ExecStatement(sqlAccessor, m_htMailDistSet);
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
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILDIST_SETTING_LIST);
	}

	/// <summary>
	/// 更新する
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		var mailDistSettingInput = new MailDistSettingInput(new MailDistSettingModel());
		mailDistSettingInput.SetSchedule(m_htMailDistSet);
		var taskScheduleRule = new TaskScheduleRule(mailDistSettingInput.CreateModel());
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
					htParam[Constants.FIELD_TASKSCHEDULE_ACTION_KBN] = Constants.FLG_TASKSCHEDULE_ACTION_KBN_MAIL_DIST;
					htParam[Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID] = m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID];

					sqlStatement.ExecStatement(sqlAccessor, htParam);
				}

				//------------------------------------------------------
				// メール配信設定更新
				//------------------------------------------------------
				using (SqlStatement sqlStatement = new SqlStatement("MailDistSetting", "UpdateMailDistSetting"))
				{
					sqlStatement.ExecStatement(sqlAccessor, m_htMailDistSet);
				}

				//------------------------------------------------------
				// メール配信除外リスト全削除
				//------------------------------------------------------
				Hashtable htInput = new Hashtable();
				htInput[Constants.FIELD_MAILDISTEXCEPTLIST_DEPT_ID] = this.LoginOperatorDeptId;
				htInput[Constants.FIELD_MAILDISTEXCEPTLIST_MAILDIST_ID] = m_htMailDistSet[Constants.FIELD_MAILDISTEXCEPTLIST_MAILDIST_ID];
				using (SqlStatement sqlStatement = new SqlStatement("MailDistSetting", "DeleteMailDistExceptListAll"))
				{
					sqlStatement.ExecStatement(sqlAccessor, htInput);
				}

				//------------------------------------------------------
				// メール配信除外リストインサート
				//------------------------------------------------------
				string[] strExceptLists = ((string)m_htMailDistSet["except_list"]).Split(',');
				using (SqlStatement sqlStatement = new SqlStatement("MailDistSetting", "InsertMailDistExceptList"))
				{
					foreach (string strExcept in strExceptLists)
					{
						htInput[Constants.FIELD_MAILDISTEXCEPTLIST_MAIL_ADDR] = strExcept;
						sqlStatement.ExecStatement(sqlAccessor, htInput);
					}
				}

				//------------------------------------------------------
				// スケジュール追加（スケジュールが未来or繰り返し かつ 有効）
				//------------------------------------------------------
				if (CanInsertTaskSchedule(m_htMailDistSet)
					&& ((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_VALID_FLG] == Constants.FLG_MAILDISTSETTING_VALID_FLG_VALID)
					&& taskScheduleRule.CheckCanCreateFirstTaskScheduleForScheduleMonth())
				{
					using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "InsertFirstTaskSchedule"))
					{
						m_htMailDistSet[Constants.FIELD_TASKSCHEDULE_DEPT_ID] = this.LoginOperatorDeptId;
						m_htMailDistSet[Constants.FIELD_TASKSCHEDULE_ACTION_KBN] = Constants.FLG_TASKSCHEDULE_ACTION_KBN_MAIL_DIST;
						m_htMailDistSet[Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID] = m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID];
						m_htMailDistSet[Constants.FIELD_TASKSCHEDULE_LAST_CHANGED] = this.LoginOperatorName;

						int iUpdate = sqlStatement.ExecStatement(sqlAccessor, m_htMailDistSet);
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
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILDIST_SETTING_LIST);
	}

	/// <summary>プロパティ：メール配信ID</summary>
	protected string MailDistId
	{
		get { return m_strMailDistId; }
	}
	///  <summary>メールセット</summary>
	protected Hashtable MailDistSet
	{
		get { return m_htMailDistSet; }
	}
	/// <summary>アドレス数</summary>
	protected int DispAddressCount
	{
		get { return m_dispAddressCount; }
	}
	/// <summary>排除メールアドレスリスト</summary>
	protected string[] ExceptLists { get; set; }
}
