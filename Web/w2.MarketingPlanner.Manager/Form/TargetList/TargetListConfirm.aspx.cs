/*
=========================================================================================================
  Module      : ターゲットリスト設定確認ページ処理(TargetListConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using w2.App.Common.TargetList;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.Coupon;
using w2.Domain.MemberRankRule;
using w2.Domain.PageDesign;
using w2.Domain.Point;
using w2.Domain.TargetList;
using w2.Domain.TaskSchedule.Helper;
using w2.Domain.User.Helper;

public partial class Form_TargetList_TargetListConfirm : BasePage
{
	string m_strTargetId = null;
	public string m_strActionKbn = null;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// ユーザーコントロール割り当て
		uMasterDownload.OnCreateSearchInputParams += this.CreateSearchParams;

		// パラメタ取得
		m_strActionKbn = IsPostBack
			? (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS]
			: (string)Request[Constants.REQUEST_KEY_ACTION_STATUS];
		m_strTargetId = IsPostBack
			? (string)ViewState[Constants.REQUEST_KEY_TARGET_ID]
			: (string)Request[Constants.REQUEST_KEY_TARGET_ID];
		
		ViewState[Constants.REQUEST_KEY_ACTION_STATUS] = m_strActionKbn;
		ViewState[Constants.REQUEST_KEY_TARGET_ID] = m_strTargetId;

		// ターゲットリストデータ取得
		if (string.IsNullOrEmpty(m_strTargetId) == false)
		{
			this.TargetList = TargetListUtility.GetTargetList(this.LoginOperatorDeptId, this.m_strTargetId)[0];
		}

		if (!IsPostBack)
		{

			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComornent(m_strActionKbn);

			//------------------------------------------------------
			// ターゲット名、条件、スケジュール表示
			//------------------------------------------------------
			TargetListConditionList lTargetListCondition = null;	// switch内で宣言がかぶるのでここで宣言
			switch (m_strActionKbn)
			{
				case Constants.ACTION_STATUS_DETAIL:

					lbDataCounts.Text =
						WebSanitizer.HtmlEncode(
							(this.TargetList[Constants.FIELD_TARGETLIST_DATA_COUNT] != DBNull.Value) 
								? string.Format(
									ReplaceTag("@@DispText.target_list.ExtractionCount@@"),
									this.TargetList[Constants.FIELD_TARGETLIST_DATA_COUNT],
									DateTimeUtility.ToStringForManager(
										this.TargetList[Constants.FIELD_TARGETLIST_DATA_COUNT_DATE],
										DateTimeUtility.FormatType.ShortDateHourMinute2Letter))
								: "-");

					// ステータスが「通常」の場合、「抽出実行」ボタンをenableにする
					btnExtractTarget.Enabled = (StringUtility.ToEmpty(this.TargetList[Constants.FIELD_TARGETLIST_STATUS]) == Constants.FLG_TARGETLIST_STATUS_NORMAL);

					// 空の条件を除外します。
					if (Constants.TARGET_LIST_IMPORT_TYPE_LIST.Contains((string)this.TargetList[Constants.FIELD_TARGETLIST_TARGET_TYPE]))
					{
						btnCopyInsertBottom.Visible = btnCopyInsertTop.Visible = false;
						btnEditBottom.Visible = btnEditTop.Visible = false;
						trSchedule.Visible = tblTargetExtract.Visible = false;
						lbImportTypeTargetCondition.Visible = true;
						lbImportTypeTargetCondition.Text = WebSanitizer.HtmlEncode(
							ValueText.GetValueText(
								Constants.TABLE_TARGETLIST,
								Constants.FIELD_TARGETLIST_TARGET_TYPE,
								(string)this.TargetList[Constants.FIELD_TARGETLIST_TARGET_TYPE]));
						return;
					}
					lTargetListCondition = TargetListConditionRelationXml.CreateTargetListConditionList((string)this.TargetList[Constants.FIELD_TARGETLIST_TARGET_CONDITION]);
					break;

				case Constants.ACTION_STATUS_INSERT:
				case Constants.ACTION_STATUS_UPDATE:
				case Constants.ACTION_STATUS_COPY_INSERT:
					this.TargetListHashTable = (Hashtable)Session[Constants.SESSION_KEY_PARAM];

					// 条件（文字列生成はswitchのあとで行う）
					lTargetListCondition = (TargetListConditionList)this.TargetListHashTable["lTargetListCondition"];

					// スケジュール
					tblTargetExtract.Visible = false;
					tblMasterOutput.Visible = false;
					break;
			}

			// 条件文字列生成
			rConditions.DataSource = lTargetListCondition.TargetConditionList;
			rConditions.DataBind();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	/// <param name="strActionKbn">アクション区分</param>
	private void InitializeComornent(string strActionKbn)
	{
		//------------------------------------------------------
		// ボタン表示制御
		//------------------------------------------------------
		btnEditTop.Visible = btnEditBottom.Visible = (strActionKbn == Constants.ACTION_STATUS_DETAIL);
		btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = (strActionKbn == Constants.ACTION_STATUS_DETAIL);
		btnDeleteTop.Visible = btnDeleteBottom.Visible = (strActionKbn == Constants.ACTION_STATUS_DETAIL);
		btnUpdateTop.Visible = btnUpdateBottom.Visible = (strActionKbn == Constants.ACTION_STATUS_UPDATE);
		btnInsertTop.Visible = btnInsertBottom.Visible = (strActionKbn == Constants.ACTION_STATUS_INSERT) || (m_strActionKbn == Constants.ACTION_STATUS_COPY_INSERT);

		//------------------------------------------------------
		// その他表示制御
		//------------------------------------------------------
		trCounts.Visible = (strActionKbn == Constants.ACTION_STATUS_DETAIL);
		trTargetId.Visible = (strActionKbn == Constants.ACTION_STATUS_DETAIL) || (strActionKbn == Constants.ACTION_STATUS_UPDATE);
	}

	/// <summary>
	/// スケジュール文言作成
	/// </summary>
	/// <param name="targetList">ターゲットリスト</param>
	/// <returns>作成した文言</returns>
	protected string GetScheduleString(DataRowView targetList)
	{
		return GetScheduleString(
			(string) targetList[Constants.FIELD_TARGETLIST_EXEC_TIMING],
			(string) targetList[Constants.FIELD_TARGETLIST_SCHEDULE_KBN],
			(string) targetList[Constants.FIELD_TARGETLIST_SCHEDULE_DAY_OF_WEEK],
			targetList[Constants.FIELD_TARGETLIST_SCHEDULE_YEAR],
			targetList[Constants.FIELD_TARGETLIST_SCHEDULE_MONTH],
			targetList[Constants.FIELD_TARGETLIST_SCHEDULE_DAY],
			targetList[Constants.FIELD_TARGETLIST_SCHEDULE_HOUR],
			targetList[Constants.FIELD_TARGETLIST_SCHEDULE_MINUTE],
			targetList[Constants.FIELD_TARGETLIST_SCHEDULE_SECOND]);
	}

	/// <summary>
	/// スケジュール文言作成
	/// </summary>
	/// <param name="targetList">ターゲットリスト</param>
	/// <returns>作成した文言</returns>
	protected string GetScheduleStringByHashTable(Hashtable targetList)
	{
		return GetScheduleString(
			(string)targetList[Constants.FIELD_TARGETLIST_EXEC_TIMING],
			(string)targetList[Constants.FIELD_TARGETLIST_SCHEDULE_KBN],
			(string)targetList[Constants.FIELD_TARGETLIST_SCHEDULE_DAY_OF_WEEK],
			targetList[Constants.FIELD_TARGETLIST_SCHEDULE_YEAR],
			targetList[Constants.FIELD_TARGETLIST_SCHEDULE_MONTH],
			targetList[Constants.FIELD_TARGETLIST_SCHEDULE_DAY],
			targetList[Constants.FIELD_TARGETLIST_SCHEDULE_HOUR],
			targetList[Constants.FIELD_TARGETLIST_SCHEDULE_MINUTE],
			targetList[Constants.FIELD_TARGETLIST_SCHEDULE_SECOND]);
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
	/// <returns>作成した文言</returns>
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
					sbSchedule.Append(objDay.ToString()).Append(ReplaceTag("@@DispText.schedule_unit.Day@@"));
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
	/// <param name="htTargetList">ターゲットリストパラメータ</param>
	/// <returns>作成すればfalse、そうでないならtrue</returns>
	private bool CanInsertTaskSchedule(Hashtable htTargetList)
	{
		if ((string)htTargetList[Constants.FIELD_MAILDISTSETTING_EXEC_TIMING] == Constants.FLG_TARGETLIST_EXEC_TIMING_SCHEDULE)
		{
			if ((string)htTargetList[Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN] == Constants.FLG_TARGETLIST_SCHEDULE_KBN_ONCE)
			{
				DateTime dtSchedule = new DateTime(
					int.Parse(htTargetList[Constants.FIELD_MAILDISTSETTING_SCHEDULE_YEAR].ToString()),
					int.Parse(htTargetList[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MONTH].ToString()),
					int.Parse(htTargetList[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY].ToString()),
					int.Parse(htTargetList[Constants.FIELD_MAILDISTSETTING_SCHEDULE_HOUR].ToString()),
					int.Parse(htTargetList[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MINUTE].ToString()),
					int.Parse(htTargetList[Constants.FIELD_MAILDISTSETTING_SCHEDULE_SECOND].ToString()));

				if (DateTime.Now.CompareTo(dtSchedule) > 0)
				{
					return false;
				}
			}
		}

		return true;
	}

	/// <summary>
	/// Back button click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		var actionStatus = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ACTION_STATUS]);
		switch (actionStatus)
		{
			case Constants.ACTION_STATUS_DETAIL:
				Response.Redirect(CreateTargetListUrl((Hashtable)Session[Constants.SESSIONPARAM_KEY_TARGETLIST_SEARCH_INFO]));
				break;

			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
			case Constants.ACTION_STATUS_UPDATE:
				Response.Redirect(string.Format("{0}{1}?{2}={3}&{4}={5}",
					Constants.PATH_ROOT,
					Constants.PAGE_W2MP_MANAGER_TARGETLIST_REGISTER,
					Constants.REQUEST_KEY_TARGET_ID,
					this.m_strTargetId,
					Constants.REQUEST_KEY_ACTION_STATUS,
					actionStatus));
				break;
		}
	}

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, EventArgs e)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_TARGETLIST_REGISTER);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_TARGET_ID).Append("=").Append(HttpUtility.UrlEncode(m_strTargetId));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_UPDATE);

		Response.Redirect(sbResult.ToString());
	}

	/// <summary>
	/// コピー新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_TARGETLIST_REGISTER);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_TARGET_ID).Append("=").Append(HttpUtility.UrlEncode(m_strTargetId));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_COPY_INSERT);

		Response.Redirect(sbResult.ToString());
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// メール配信設定に設定されていないかチェック
		//------------------------------------------------------
		DataView dvMailDistSettings = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MailDistSetting", "CheckTargetListUsed"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_TARGETLIST_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_TARGETLIST_TARGET_ID, m_strTargetId);

			dvMailDistSettings = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
		}

		// ページ管理に設定されていないか
		var pageDesignByTargetListId = new PageDesignService().CheckTargetListUsed(m_strTargetId);
		// ポイントルールスケジュールに設定されていないか
		var pointRuleschedules = new PointService().CheckTargetListUsed(m_strTargetId);
		// クーポン発行スケジュールに設定されていないか
		var couponSchedules = new CouponService().CheckTargetListUsed(m_strTargetId);
		// 会員ランク変動ルールに設定されていないかチェック
		var memberRankRuleLists = new MemberRankRuleService().GetMemberRankRuleFromTargetList(m_strTargetId);

		//------------------------------------------------------
		// 設定されていたら削除させない、エラーページへ
		//------------------------------------------------------
		if ((dvMailDistSettings.Count != 0)
				|| (pointRuleschedules.Length != 0)
				|| (couponSchedules.Length != 0)
				|| (pageDesignByTargetListId.Length != 0)
				|| (memberRankRuleLists.Length != 0))
		{
			// エラーメッセージに、設定されている設定を表示させる
			StringBuilder sbErrMsg = new StringBuilder();
			if (dvMailDistSettings.Count != 0) sbErrMsg.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MAILDISTTEXT_USED));
			foreach (DataRowView drvMailDistSetting in dvMailDistSettings)
			{
				sbErrMsg.Append((string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID]).Append(" : ");
				sbErrMsg.Append((string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_MAILDIST_NAME]).Append("<br />");
			}
			if (pointRuleschedules.Length != 0) sbErrMsg.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERPOINT_USED));
			foreach (PointRuleScheduleModel pointRuleSchedule in pointRuleschedules)
			{
				sbErrMsg.Append(pointRuleSchedule.PointRuleScheduleId).Append(" : ");
				sbErrMsg.Append(pointRuleSchedule.PointRuleScheduleName).Append("<br />");
			}
			if (couponSchedules.Length != 0) sbErrMsg.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERCOUPON_USED));
			foreach (CouponScheduleModel couponRuleSchedule in couponSchedules)
			{
				sbErrMsg.Append(couponRuleSchedule.CouponScheduleId).Append(" : ");
				sbErrMsg.Append(couponRuleSchedule.CouponScheduleName).Append("<br />");
			}
			if (pageDesignByTargetListId.Length != 0) sbErrMsg.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAGEDESIGN_USED));
			foreach (var pageDesignSchedule in pageDesignByTargetListId)
			{
				sbErrMsg.Append(pageDesignSchedule.PageId).Append(" : ");
				sbErrMsg.Append(pageDesignSchedule.ManagementTitle).Append("<br />");
			}
			if (memberRankRuleLists.Length != 0) sbErrMsg.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MEMBERRANKRULE_USED));
			foreach (var memberRankRuleList in memberRankRuleLists)
			{
				sbErrMsg.Append(memberRankRuleList.MemberRankRuleId).Append(" : ");
				sbErrMsg.Append(memberRankRuleList.MemberRankRuleName).Append("<br />");
			}
			Session[Constants.SESSION_KEY_ERROR_MSG] = sbErrMsg.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// 設定されていなければ削除
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			try
			{
				//------------------------------------------------------
				// スケジュール削除
				//------------------------------------------------------
				using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "DeleteTaskSchedule"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.LoginOperatorDeptId);
					htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_CREATE_TARGETLIST);
					htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, m_strTargetId);

					sqlStatement.ExecStatement(sqlAccessor, htInput);
				}

				//------------------------------------------------------
				// ターゲットリスト削除フラグ更新
				//------------------------------------------------------
				using (SqlStatement sqlStatement = new SqlStatement("TargetList", "UpdateTargetListDelFlg"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_TARGETLIST_DEPT_ID, this.LoginOperatorDeptId);
					htInput.Add(Constants.FIELD_TARGETLIST_TARGET_ID, m_strTargetId);
					sqlStatement.ExecStatement(sqlAccessor, htInput);
				}

				// ターゲットリストに紐づいたターゲットリストデータを削除
				TargetListUtility.DeleteTargetListData(this.LoginOperatorDeptId, m_strTargetId, sqlAccessor);

				sqlAccessor.CommitTransaction();
			}
			catch(Exception ex)
			{
				FileLogger.WriteError(ex);
				throw;
			}
		}

		//------------------------------------------------------
		// 一覧ページへリダイレクト
		//------------------------------------------------------
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_TARGETLIST_LIST);
	}

	/// <summary>
	/// 登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		Hashtable htParam = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
		htParam[Constants.FIELD_TARGETLIST_TARGET_ID] = NumberingUtility.CreateKeyId(this.LoginOperatorShopId, Constants.NUMBER_KEY_TARGET_LIST_ID, 10);

		CheckUserExtendSettingExist(htParam);

		var targetInput = new TargetListInput(new TargetListModel());
		targetInput.SetSchedule(htParam);
		var taskScheduleRule = new TaskScheduleRule(targetInput.CreateModel());
		//------------------------------------------------------
		// 追加・更新
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			// トランザクション開始
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			try
			{
				//------------------------------------------------------
				// ターゲットリスト追加（ターゲットID取得のためSELECT）
				//------------------------------------------------------
				DataView dvResult = null;
				using (SqlStatement sqlStatement = new SqlStatement("TargetList", "InsertTargetList"))
				{
					htParam.Add(Constants.FIELD_TARGETLIST_DEL_FLG, "0");
					dvResult = sqlStatement.SelectSingleStatement(sqlAccessor, htParam);
				}

				// 初回スケジュール追加
				if (taskScheduleRule.CheckCanCreateFirstTaskScheduleForScheduleMonth())
				{
					using (var statement = new SqlStatement("TaskSchedule", "InsertFirstTaskSchedule"))
					{
						htParam[Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID] = dvResult[0][Constants.FIELD_TARGETLIST_TARGET_ID];
						htParam[Constants.FIELD_TASKSCHEDULE_ACTION_KBN] = Constants.FLG_TASKSCHEDULE_ACTION_KBN_CREATE_TARGETLIST;

						statement.ExecStatement(sqlAccessor, htParam);
					}
				}
				// コミット
				sqlAccessor.CommitTransaction();
			}
			catch
			{
				// ロールバック
				sqlAccessor.RollbackTransaction();
				throw;
			}
		}

		//------------------------------------------------------
		// 一覧ページへリダイレクト
		//------------------------------------------------------
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_TARGETLIST_LIST);

	}

	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		Hashtable htParam = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
		CheckUserExtendSettingExist(htParam);

		var targetInput = new TargetListInput(new TargetListModel());
		targetInput.SetSchedule(htParam);
		var taskScheduleRule = new TaskScheduleRule(targetInput.CreateModel());
		//------------------------------------------------------
		// 更新
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			// トランザクション開始
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			try
			{
				//------------------------------------------------------
				// 未実行スケジュール削除
				//------------------------------------------------------
				using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "DeleteTaskScheduleUnexecuted"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.LoginOperatorDeptId);
					htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_CREATE_TARGETLIST);
					htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, m_strTargetId);

					sqlStatement.ExecStatement(sqlAccessor, htInput);
				}

				//------------------------------------------------------
				// ターゲットリスト更新
				//------------------------------------------------------
				using (SqlStatement sqlStatement = new SqlStatement("TargetList", "UpdateTargetList"))
				{
					sqlStatement.ExecStatement(sqlAccessor, htParam);
				}

				// 初回スケジュール追加
				if (CanInsertTaskSchedule(htParam) && taskScheduleRule.CheckCanCreateFirstTaskScheduleForScheduleMonth())
				{
					using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "InsertFirstTaskSchedule"))
					{
						htParam.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_CREATE_TARGETLIST);
						htParam.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, m_strTargetId);

						int iUpdate = sqlStatement.ExecStatement(sqlAccessor, htParam);
					}
				}

				// コミット
				sqlAccessor.CommitTransaction();
			}
			catch
			{
				// ロールバック
				sqlAccessor.RollbackTransaction();
				throw;
			}
		}

		//------------------------------------------------------
		// 一覧ページへリダイレクト
		//------------------------------------------------------
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_TARGETLIST_LIST);
	}

	/// <summary>
	/// ユーザー拡張項目に項目が存在しているか否かを確認
	/// </summary>
	/// <param name="htParam">パラメータ</param>
	private void CheckUserExtendSettingExist(Hashtable htParam)
	{
		var lTargetListCondition = TargetListConditionRelationXml.CreateTargetListConditionList((string)htParam[Constants.FIELD_TARGETLIST_TARGET_CONDITION]);
		var uesList = new UserExtendSettingList(this.LoginOperatorId);

		var notExistUserExtendSetting = new Dictionary<string, string>();

		foreach (var condition in lTargetListCondition.TargetConditionList)
		{
			condition.IsExtendSettingExist(condition, uesList, notExistUserExtendSetting);
		}

		if (notExistUserExtendSetting.Count > 0)
		{
			// エラーメッセージに、設定されているメール配信設定を表示させる
			StringBuilder errMessage = new StringBuilder();

			errMessage.Append(WebMessages.ERRMSG_MANAGER_TARGETLIST_NOT_VALID);
			foreach (string key in notExistUserExtendSetting.Keys)
			{
				errMessage.Append(ReplaceTag("@@DispText.target_list.ExtendSettingId@@")).Append(key).Append(" / ");
				errMessage.Append(ReplaceTag("@@DispText.target_list.ExtendSettingName@@")).Append(notExistUserExtendSetting[key]).Append(Environment.NewLine);
			}

			Session[Constants.SESSION_KEY_ERROR_MSG] = errMessage.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// 抽出実行
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnExtractTarget_Click(object sender, EventArgs e)
	{
		try
		{
			// 非同期に監視ログ一覧取得
			this.ProcessInfoCacheKey = Guid.NewGuid().ToString();
			this.ProcessInfo = new ProcessInfoType();
			tProcessTimer.Enabled = true;

			Task.Run(
				() =>
				{
					try
					{
						lblExtractStatus.Text = "処理中";
						btnExtractTarget.Enabled = false;
						imgLoading.Visible = true;
						TargetListUtility.ExecuteExtractTargetListData(this.LoginOperatorDeptId, this.m_strTargetId);
						this.ProcessInfo.IsDone = true;
					}
					catch (Exception ex)
					{
						tProcessTimer.Enabled = false;
						this.ProcessInfo.IsSystemError = true;
						FileLogger.WriteError(ex);
					}
				});

			Thread.Sleep(50);
			tProcessTimer_Tick(null, null);
		}
		catch (Exception ex)
		{
			FileLogger.WriteError("ターゲットリスト抽出失敗", ex);
			lblExtractStatus.Text = ReplaceTag("@@DispText.target_list.ExtractionError@@");
		}
	}

	/// <summary>
	/// 実行時のタイマー処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void tProcessTimer_Tick(object sender, EventArgs e)
	{
		if ((this.ProcessInfo == null)
			|| this.ProcessInfo.IsSystemError)
		{
			tProcessTimer.Enabled = false;
			FileLogger.WriteError("ターゲットリスト抽出失敗");
			return;
		}

		if (this.ProcessInfo.IsDone)
		{
			tProcessTimer.Enabled = false;
			imgLoading.Visible = false;
			btnExtractTarget.Enabled = true;
			var targetList = TargetListUtility.GetTargetList(this.LoginOperatorDeptId, this.m_strTargetId)[0];
			lbDataCounts.Text = (targetList[Constants.FIELD_TARGETLIST_DATA_COUNT] != DBNull.Value)
				? WebSanitizer.HtmlEncode(
					string.Format(
						ReplaceTag("@@DispText.target_list.ExtractionCount@@"),
						targetList[Constants.FIELD_TARGETLIST_DATA_COUNT],
						DateTimeUtility.ToStringForManager(
							targetList[Constants.FIELD_TARGETLIST_DATA_COUNT_DATE],
							DateTimeUtility.FormatType.ShortDateHourMinute2Letter)))
				: "-";
			lblExtractStatus.Text = ReplaceTag("@@DispText.target_list.ExtractionComplete@@");
		}
	}

	/// <summary>
	/// マスタデータ出力用の検索ハッシュテーブル生成
	/// </summary>
	/// <returns>検索ハッシュテーブル</returns>
	/// <remarks>マスタ出力ユーザコントロールのイベントに割り当てて使う</remarks>
	public Hashtable CreateSearchParams()
	{
		var sqlParams = new Hashtable()
		{
			{Constants.FIELD_TARGETLISTDATA_DEPT_ID, this.LoginOperatorDeptId},
			{Constants.FIELD_TARGETLISTDATA_MASTER_ID, this.m_strTargetId},
			{Constants.FIELD_TARGETLISTDATA_TARGET_KBN, Constants.FLG_TARGETLISTDATA_TARGET_KBN_TARGETLIST}
		};

		return sqlParams;
	}

	/// <summary>
	/// 配列文字場所指定
	/// </summary>
	/// <param name="fixedPurchasePattern">カンマ区切り文字列</param>
	/// <param name="number">配列指定</param>
	/// <returns>定期配送月日</returns>
	protected string FixedPurchasePattern(string fixedPurchasePattern, int number = 0)
	{
		var textSplit = TextSplit(fixedPurchasePattern);
		var result = (textSplit.Length > number)
			? textSplit[number]
			: "";
		return result;
	}

	/// <summary>
	/// 数字から曜日に変換
	/// </summary>
	/// <param name="fixedPurchasePattern">カンマ区切り文字列</param>
	/// <param name="number">配列指定</param>
	/// <returns>曜日</returns>
	protected string NumberToDayOfWeek(string fixedPurchasePattern, int number = 0)
	{
		var textSplit = TextSplit(fixedPurchasePattern);
		var dayOfWeek = (textSplit.Length > number)
			? ValueText.GetValueText(
				Constants.TABLE_FIXEDPURCHASE,
				Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_DAY_OF_WEEK,
			textSplit[number])
			: "";
		return dayOfWeek;
	}

	/// <summary>
	/// 文字分割
	/// </summary>
	/// <param name="fixedPurchasePattern">カンマ区切り文字列</param>
	/// <returns>分割文字</returns>
	private string[] TextSplit(string fixedPurchasePattern)
	{
		var valueName = fixedPurchasePattern.Split(',');
		return valueName;
	}

	/// <summary>
	/// 定期配送パターンに変換
	/// </summary>
	/// <param name="fixedPurchaseKbn">定期配送パターン区分</param>
	/// <returns>定期配送パターン</returns>
	protected string FixedPurchaseText(string fixedPurchaseKbn)
	{
		var fixedPurchasePatternText = ValueText.GetValueText(
				Constants.TABLE_FIXEDPURCHASE,
				Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1,
			fixedPurchaseKbn);
		return fixedPurchasePatternText;
	}

	/// <summary>ターゲットリスト</summary>
	protected DataRowView TargetList { get; set; }
	/// <summary>ターゲットリスト（ハッシュテーブル）</summary>
	protected Hashtable TargetListHashTable { get; set; }
	/// <summary>ターゲットID</summary>
	protected string TargetId
	{
		get { return m_strTargetId; }
	}
	/// <summary>アクションステータスがDetailか</summary>
	protected bool IsActionStatusDetail
	{
		get { return (m_strActionKbn == Constants.ACTION_STATUS_DETAIL); }
	}
	/// <summary>処理情報（非同期スレッドでもアクセス可能なようにWEBキャッシュ格納）</summary>
	public ProcessInfoType ProcessInfo
	{
		get
		{
			var value = this.Cache[this.ProcessInfoCacheKey];

			return (value is ProcessInfoType)
				? (ProcessInfoType)value
				: null;
		}
		set
		{
			if (value != null)
			{
				this.Cache.Insert(
					this.ProcessInfoCacheKey,
					value,
					null,
					System.Web.Caching.Cache.NoAbsoluteExpiration,
					TimeSpan.FromMinutes(5));
			}
			else
			{
				this.Cache.Remove(this.ProcessInfoCacheKey);
			}
		}
	}
	/// <summary>処理情報キャッシュキー</summary>
	private string ProcessInfoCacheKey
	{
		get { return (string)ViewState["ProcessInfoCacheKey"]; }
		set { ViewState["ProcessInfoCacheKey"] = value; }
	}
}

/// <summary>
/// 処理情報（非同時処理とのやり取りに利用する）
/// </summary>
[Serializable]
public class ProcessInfoType
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public ProcessInfoType()
	{
		this.IsSystemError = false;
		this.IsDone = false;
	}

	/// <summary>システムエラー発生したか</summary>
	public bool IsSystemError { get; set; }
	/// <summary>処理完了したか</summary>
	public bool IsDone { get; set; }
}
