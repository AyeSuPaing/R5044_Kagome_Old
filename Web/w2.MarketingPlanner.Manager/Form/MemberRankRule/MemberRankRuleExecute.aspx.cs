/*
=========================================================================================================
  Module      : 会員ランク付与実行ページ処理(MemberRankRuleExecute.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Form_MemberRankRule_MemberRankRuleExecute : BasePage
{
	string m_strActionMasterId = null;	// 会員ランク付与実行用の実行マスタID
	int m_iActionNoMemberRank = -1;		// 会員ランク付与実行用の実行履歴NO

	// 会員ランク付与ターゲット抽出用の、実行マスタIDと実行履歴NOのキーペア
	KeyValuePair<string, int> m_kvpActionNoTarget = new KeyValuePair<string, int>();

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
			// パラメタ取得
			//------------------------------------------------------
			ViewState["ActionMasterId"] = m_strActionMasterId = Request[Constants.REQUEST_KEY_MEMBERRANKRULE_ID];
			ViewState["ActionNoMemberRank"] = m_iActionNoMemberRank = -1;
			ViewState["ActionNoTarget"] = m_kvpActionNoTarget;

			//------------------------------------------------------
			// 会員ランク変動ルール取得
			//------------------------------------------------------
			DataView dvMemberRankRule = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MemberRankRule", "GetMemberRankRule"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID, m_strActionMasterId);

				dvMemberRankRule = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			if (dvMemberRankRule.Count != 0)
			{
				//------------------------------------------------------
				// 画面設定
				//------------------------------------------------------
				lMemberRankRuleId.Text = WebSanitizer.HtmlEncode(dvMemberRankRule[0][Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID]);
				lMemberRankRuleName.Text = WebSanitizer.HtmlEncode(dvMemberRankRule[0][Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_NAME]);
			}
		}
		else
		{
			//------------------------------------------------------
			// ビューステートより値取得
			//------------------------------------------------------
			m_strActionMasterId = (string)ViewState["ActionMasterId"];
			m_iActionNoMemberRank = (int)ViewState["ActionNoMemberRank"];
			m_kvpActionNoTarget = (KeyValuePair<string, int>)ViewState["ActionNoTarget"];

			//------------------------------------------------------
			// スケジュールが取得できれば状態取得
			//------------------------------------------------------
			if ((m_iActionNoMemberRank != -1) || (m_kvpActionNoTarget.Key != null))
			{
				// ステータス表示
				DisplayTaskStatus();
			}
		}
	}

	/// <summary>
	/// タスクステータス表示
	/// </summary>
	protected void DisplayTaskStatus()
	{
		//------------------------------------------------------
		// 会員ランク付与ステータス表示
		//------------------------------------------------------
		DataView dvTaskSchedule = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "GetTaskSchedule"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_CHANGE_MEMBER_RANK);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, m_strActionMasterId);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_NO, m_iActionNoMemberRank);

			dvTaskSchedule = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}
		if (dvTaskSchedule.Count != 0)
		{
			// 準備ステータス
			lbMemberRanklPrepareStatus.Text = ValueText.GetValueText(Constants.TABLE_TASKSCHEDULE,
				                                                     Constants.FIELD_TASKSCHEDULE_PREPARE_STATUS,
																	 (string)dvTaskSchedule[0][Constants.FIELD_TASKSCHEDULE_PREPARE_STATUS]);
			// 実行ステータス
			lbMemberRankExecuteStatus.Text = ValueText.GetValueText(Constants.TABLE_TASKSCHEDULE,
				                                                    Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS,
																	(string)dvTaskSchedule[0][Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS]);
			lbMemberRankProgress.Text = (string)dvTaskSchedule[0][Constants.FIELD_TASKSCHEDULE_PROGRESS];	// 進捗
			
			switch ((string)dvTaskSchedule[0][Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS])
			{
				// 終了 or 停止
				case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE:
				case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_STOP:
					// 付与実行ボタンを有効化
					btnMemberRankChangeStart.Enabled = true;

					// 付与停止ボタンを無効化
					btnMemberRankChangeStop.Enabled = false;
					break;
			}
		}

		//------------------------------------------------------
		// ターゲット抽出ステータス表示
		//------------------------------------------------------
		if (btnTargetStartExtract.Enabled == false)
		{
			DataView dvTaskScheduleTarget = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "GetTaskSchedule"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.LoginOperatorDeptId);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_MEMBER_RANK_USER_EXTRACT);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, m_kvpActionNoTarget.Key);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_NO, m_kvpActionNoTarget.Value);

				dvTaskScheduleTarget = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}
			if (dvTaskScheduleTarget.Count != 0)
			{
				lbTargetExecuteStatus.Text = "";

				string strStatus = ValueText.GetValueText(Constants.TABLE_TASKSCHEDULE, Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS, (string)dvTaskScheduleTarget[0][Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS]);
				string strProgress = (string)dvTaskScheduleTarget[0][Constants.FIELD_TASKSCHEDULE_PROGRESS];

				lbTargetExecuteStatus.Text += (string)dvTaskScheduleTarget[0][Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID] + "：　" + strStatus;

				if (strProgress != string.Empty)
				{
					lbTargetExecuteStatus.Text += "(" + strProgress + ")";
				}

				switch ((string)dvTaskScheduleTarget[0][Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS])
				{
					// 終了 or 停止
					case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE:
					case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_STOP:
						// ターゲット初期化
						ViewState["ActionNoTarget"] = m_kvpActionNoTarget = new KeyValuePair<string, int>();

						// 抽出実行ボタンを有効化
						btnTargetStartExtract.Enabled = true;
						break;
				}
			}
		}
	}

	/// <summary>
	/// 付与実行ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnMemberRankChangeStart_Click(object sender, EventArgs e)
	{
		lbMemberRanklPrepareStatus.Text = "";
		lbMemberRankExecuteStatus.Text = "";

		// タスクスケジュールインサート
		DataView dvTaskSchedule = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "InsertTaskScheduleForExecute"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_CHANGE_MEMBER_RANK);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, m_strActionMasterId);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_LAST_CHANGED, this.LoginOperatorName);

			dvTaskSchedule = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		if (dvTaskSchedule.Count != 0)
		{
			ViewState["ActionNoMemberRank"] = m_iActionNoMemberRank = (int)dvTaskSchedule[0][Constants.FIELD_TASKSCHEDULE_ACTION_NO];
			DisplayTaskStatus();
		}

		// 付与実行ボタンを無効化
		btnMemberRankChangeStart.Enabled = false;

		// 付与停止ボタンを有効化
		btnMemberRankChangeStop.Enabled = true;
	}

	/// <summary>
	/// 付与停止ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnMemberRankChangeStop_Click(object sender, EventArgs e)
	{
		// タスクスケジュール停止
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "SetTaskScheduleStopped"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_CHANGE_MEMBER_RANK);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, m_strActionMasterId);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_LAST_CHANGED, this.LoginOperatorName);

			sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
		}

		// 付与停止ボタンを無効化
		btnMemberRankChangeStop.Enabled = false;
	}

	/// <summary>
	/// 抽出ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnStartExtract_Click(object sender, EventArgs e)
	{
		// 抽出ボタンを無効化
		btnTargetStartExtract.Enabled = false;
		lTargetStartExtract.Visible = false;

		// タスクスケジュール実行
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "InsertTaskScheduleForExecute"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_MEMBER_RANK_USER_EXTRACT);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, m_strActionMasterId);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_LAST_CHANGED, this.LoginOperatorName);

			DataView dvTaskSchedule = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			if (dvTaskSchedule.Count != 0)
			{
				ViewState["ActionNoTarget"] = m_kvpActionNoTarget = new KeyValuePair<string, int>(m_strActionMasterId, (int)dvTaskSchedule[0][Constants.FIELD_TASKSCHEDULE_ACTION_NO]);
				DisplayTaskStatus();
			}
		}
	}
}