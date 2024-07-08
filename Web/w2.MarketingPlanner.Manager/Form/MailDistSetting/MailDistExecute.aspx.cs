/*
=========================================================================================================
  Module      : メール配信実行ページ処理(MailDistExecute.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.TargetList;
using w2.Domain.MessagingAppContents;
using w2.Domain.TaskScheduleLog;

public partial class Form_MailDistSetting_MailDistExecute : BasePage
{
	string m_strActionMasterId = null;
	int m_iActionNoMail = -1;

	Dictionary<string, string> m_dicTargetName = new Dictionary<string, string>();
	List<KeyValuePair<string,int>> m_lActionNoTargets = new List<KeyValuePair<string,int>>();

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
			ViewState["ActionMasterId"] = m_strActionMasterId = Request[Constants.REQUEST_KEY_MAILDIST_ID];
			ViewState["ActionNoMail"] = m_iActionNoMail = -1;
			ViewState["TargetName"] = m_dicTargetName;
			ViewState["ActionNoTargets"] = m_lActionNoTargets;

			//------------------------------------------------------
			// メール配信設定取得
			//------------------------------------------------------
			DataView dvMailDistSetting = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MailDistSetting", "GetMailDistSetting"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MAILDISTSETTING_DEPT_ID, this.LoginOperatorDeptId);
				htInput.Add(Constants.FIELD_MAILDISTSETTING_MAILDIST_ID, m_strActionMasterId);

				dvMailDistSetting = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			if (dvMailDistSetting.Count != 0)
			{
				//------------------------------------------------------
				// 画面設定
				//------------------------------------------------------
				lbMailDistId.Text = WebSanitizer.HtmlEncode(dvMailDistSetting[0][Constants.FIELD_MAILDISTSETTING_MAILDIST_ID]);
				lbMailDistName.Text = WebSanitizer.HtmlEncode(dvMailDistSetting[0][Constants.FIELD_MAILDISTSETTING_MAILDIST_NAME]);

				//------------------------------------------------------
				// データ設定
				//------------------------------------------------------
				var enabledFlags = false;
				string strTargetIdTmp = null;
				strTargetIdTmp = (string)dvMailDistSetting[0][Constants.FIELD_MAILDISTSETTING_TARGET_ID];
				if (strTargetIdTmp != string.Empty)
				{
					m_dicTargetName[strTargetIdTmp] = (string)dvMailDistSetting[0][Constants.FIELD_TARGETLIST_TARGET_NAME + "1"];
					enabledFlags = enabledFlags || CreatedByOrderOrFixedPurchase(strTargetIdTmp);
				}
				strTargetIdTmp = (string)dvMailDistSetting[0][Constants.FIELD_MAILDISTSETTING_TARGET_ID2];
				if (strTargetIdTmp != string.Empty)
				{
					m_dicTargetName[strTargetIdTmp] = (string)dvMailDistSetting[0][Constants.FIELD_TARGETLIST_TARGET_NAME + "2"];
					enabledFlags = enabledFlags || CreatedByOrderOrFixedPurchase(strTargetIdTmp);
				}
				strTargetIdTmp = (string)dvMailDistSetting[0][Constants.FIELD_MAILDISTSETTING_TARGET_ID3];
				if (strTargetIdTmp != string.Empty)
				{
					m_dicTargetName[strTargetIdTmp] = (string)dvMailDistSetting[0][Constants.FIELD_TARGETLIST_TARGET_NAME + "3"];
					enabledFlags = enabledFlags || CreatedByOrderOrFixedPurchase(strTargetIdTmp);
				}
				strTargetIdTmp = (string)dvMailDistSetting[0][Constants.FIELD_MAILDISTSETTING_TARGET_ID4];
				if (strTargetIdTmp != string.Empty)
				{
					m_dicTargetName[strTargetIdTmp] = (string)dvMailDistSetting[0][Constants.FIELD_TARGETLIST_TARGET_NAME + "4"];
					enabledFlags = enabledFlags || CreatedByOrderOrFixedPurchase(strTargetIdTmp);
				}
				strTargetIdTmp = (string)dvMailDistSetting[0][Constants.FIELD_MAILDISTSETTING_TARGET_ID5];
				if (strTargetIdTmp != string.Empty)
				{
					m_dicTargetName[strTargetIdTmp] = (string)dvMailDistSetting[0][Constants.FIELD_TARGETLIST_TARGET_NAME + "5"];
					enabledFlags = enabledFlags || CreatedByOrderOrFixedPurchase(strTargetIdTmp);
				}

				// 抽出実行ボタン無効化判定
				if (enabledFlags == false)
				{
					btnTargetStartExtract.Enabled = false;
				}
			}
		}
		else
		{
			//------------------------------------------------------
			// ビューステートより値取得
			//------------------------------------------------------
			m_strActionMasterId = (string)ViewState["ActionMasterId"];
			m_iActionNoMail = (int)ViewState["ActionNoMail"];
			m_dicTargetName = (Dictionary<string, string>)ViewState["TargetName"];
			m_lActionNoTargets = (List<KeyValuePair<string, int>>)ViewState["ActionNoTargets"];

			//------------------------------------------------------
			// スケジュールが取得できれば状態取得
			//------------------------------------------------------
			if ((m_iActionNoMail != -1) || (m_lActionNoTargets.Count != 0))
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
		// メール配信ステータス表示
		//------------------------------------------------------
		DataView dvTaskSchedule = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "GetTaskSchedule"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_MAIL_DIST);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, m_strActionMasterId);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_NO, m_iActionNoMail);

			dvTaskSchedule = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}
		if (dvTaskSchedule.Count != 0)
		{
			var messageSendResult = GetMessageSendResult(MessagingAppContentsModel.MESSAGING_APP_KBN_LINE);
			lbMailPrepareStatus.Text = ValueText.GetValueText(Constants.TABLE_TASKSCHEDULE, Constants.FIELD_TASKSCHEDULE_PREPARE_STATUS, (string)dvTaskSchedule[0][Constants.FIELD_TASKSCHEDULE_PREPARE_STATUS]);
			lbMailExecuteStatus.Text = ValueText.GetValueText(Constants.TABLE_TASKSCHEDULE, Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS, (string)dvTaskSchedule[0][Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS]) + messageSendResult;
			lbMailProgress.Text = (string)dvTaskSchedule[0][Constants.FIELD_TASKSCHEDULE_PROGRESS];
			if (lbMailProgress.Text != "")
			{
				lbErrorPointExceptCount.Text = StringUtility.ToNumeric(dvTaskSchedule[0][Constants.FIELD_MAILDISTSETTING_LAST_ERROREXCEPT_COUNT]);
				lbMobileMailExceptCount.Text = StringUtility.ToNumeric(dvTaskSchedule[0][Constants.FIELD_MAILDISTSETTING_LAST_MOBILEMAILEXCEPT_COUNT]);
				lDuplicateExceptCount.Text = StringUtility.ToNumeric(dvTaskSchedule[0][Constants.FIELD_MAILDISTSETTING_LAST_DUPLICATE_EXCEPT_COUNT]);
			}

			switch ((string)dvTaskSchedule[0][Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS])
			{
				// 終了
				case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE:
					// 配信実行ボタンを無効化
					btnMailStartDelivery.Enabled = false;

					// 配信停止ボタンを無効化
					btnMailStopDelivery.Enabled = false;

					break;

				// 停止
				case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_STOP:
					// 配信実行ボタンを有効化
					btnMailStartDelivery.Enabled = true;

					// 配信停止ボタンを無効化
					btnMailStopDelivery.Enabled = false;
					break;
			}
		}

		//------------------------------------------------------
		// ターゲット抽出ステータス表示
		//------------------------------------------------------
		if (btnTargetStartExtract.Enabled == false)
		{
			List<DataView> lTaskScheduleTarget = new List<DataView>();
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "GetTaskSchedule"))
			{
				foreach (KeyValuePair<string,int> kvpTargetList in m_lActionNoTargets)
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.LoginOperatorDeptId);
					htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_CREATE_TARGETLIST);
					htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, kvpTargetList.Key);
					htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_NO, kvpTargetList.Value);

					lTaskScheduleTarget.Add( sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput) );
				}
			}
			if (lTaskScheduleTarget.Count != 0)
			{
				lbTargetExecuteStatus.Text = "";
				foreach (DataView dvTaskScheduleTarget in lTaskScheduleTarget)
				{
					if (dvTaskScheduleTarget.Count != 0)
					{
						string strStatus = ValueText.GetValueText(Constants.TABLE_TASKSCHEDULE, Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS, (string)dvTaskScheduleTarget[0][Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS]);
						string strProgress = (string)dvTaskScheduleTarget[0][Constants.FIELD_TASKSCHEDULE_PROGRESS];

						if (lTaskScheduleTarget.Count > 1)
						{
							lbTargetExecuteStatus.Text += lTaskScheduleTarget.IndexOf(dvTaskScheduleTarget) + 1 + ".";
						}

						lbTargetExecuteStatus.Text += m_dicTargetName[(string)dvTaskScheduleTarget[0][Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID]] + "：　" + strStatus;

						if (strProgress != string.Empty)
						{
							lbTargetExecuteStatus.Text += "(" + strProgress + ")";
						}

						lbTargetExecuteStatus.Text += "<br />";
					}
				}

				switch ((string)lTaskScheduleTarget[0][0][Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS])
				{
					// 終了 or 停止
					case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE:
					case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_STOP:
					case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_SKIP:

						// 抽出実行ボタンを有効化
						m_lActionNoTargets.Clear();
						btnTargetStartExtract.Enabled = true;
						// 配信実行ボタンを有効化
						btnMailStartDelivery.Enabled = true;

						break;
				}
			}
		}
	}

	/// <summary>
	/// 受注情報または定期台帳から作成したターゲットリストか
	/// </summary>
	/// <param name="targetId">ターゲットID</param>
	/// <returns>真偽</returns>
	private bool CreatedByOrderOrFixedPurchase(string targetId)
	{
		var targetList = TargetListUtility.GetTargetList(this.LoginOperatorDeptId, targetId)[0];
		var enabledFlag = Constants.TARGET_LIST_IMPORT_TYPE_LIST.Contains((string)targetList[Constants.FIELD_TARGETLIST_TARGET_TYPE]) == false;
		return enabledFlag;
	}

	/// <summary>
	/// 配信実行ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnStartDelivery_Click(object sender, EventArgs e)
	{
		lbMailPrepareStatus.Text = string.Empty;
		lbMailExecuteStatus.Text = string.Empty;
		lbErrorPointExceptCount.Text = string.Empty;
		lbMobileMailExceptCount.Text = string.Empty;
		lDuplicateExceptCount.Text = string.Empty;

		// タスクスケジュールインサート
		DataView dvTaskSchedule = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "InsertTaskScheduleForExecute"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_MAIL_DIST);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, m_strActionMasterId);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_LAST_CHANGED, this.LoginOperatorName);

			dvTaskSchedule = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		if (dvTaskSchedule.Count != 0)
		{
			ViewState["ActionNoMail"] = m_iActionNoMail = (int)dvTaskSchedule[0][Constants.FIELD_TASKSCHEDULE_ACTION_NO];
			DisplayTaskStatus();
		}

		// 配信実行ボタンを無効化
		btnMailStartDelivery.Enabled = false;

		// 配信停止ボタンを有効化
		btnMailStopDelivery.Enabled = true;
	}

	/// <summary>
	/// 配信停止ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnStopDelivery_Click(object sender, EventArgs e)
	{
		// タスクスケジュール停止
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "SetTaskScheduleStopped"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_MAIL_DIST);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, m_strActionMasterId);
			htInput.Add(Constants.FIELD_TASKSCHEDULE_LAST_CHANGED, this.LoginOperatorName);

			sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
		}

		// 配信停止ボタンを無効化
		btnMailStopDelivery.Enabled = false;
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
		// 配信実行ボタンを無効化
		btnMailStartDelivery.Enabled = false;

		// タスクスケジュール停止
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "InsertTaskScheduleForExecute"))
		{
			foreach (string strTargetListId in m_dicTargetName.Keys)
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.LoginOperatorDeptId);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, Constants.FLG_TASKSCHEDULE_ACTION_KBN_CREATE_TARGETLIST);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, strTargetListId);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_LAST_CHANGED, this.LoginOperatorName);

				DataView dvTaskSchedule = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
				if (dvTaskSchedule.Count != 0)
				{
					m_lActionNoTargets.Add(new KeyValuePair<string,int>(strTargetListId, (int)dvTaskSchedule[0][Constants.FIELD_TASKSCHEDULE_ACTION_NO]));
				}
			}
		}

		if (m_lActionNoTargets.Count != 0)
		{
			DisplayTaskStatus();
		}
	}

	/// <summary>
	/// メッセージアプリ区分を指定して配信結果取得
	/// </summary>
	/// <param name="messagingAppKbn">メッセージアプリ区分</param>
	/// <returns>配信結果</returns>
	private string GetMessageSendResult(string messagingAppKbn)
	{
		var taskScheduleLogModel = new TaskScheduleLogService().GetLogEachMessagingAppKbn(
			this.LoginOperatorDeptId,
			Constants.FLG_TASKSCHEDULE_ACTION_KBN_MAIL_DIST,
			m_strActionMasterId,
			m_iActionNoMail,
			messagingAppKbn);

		if (taskScheduleLogModel.Length == 0) return string.Empty;

		var lineSendResult = WebSanitizer.HtmlEncodeChangeToBr(((string.IsNullOrEmpty(taskScheduleLogModel[0].Result) == false)
			? string.Format("{0}{0}{1}配信結果{0}{2}", Environment.NewLine, messagingAppKbn, taskScheduleLogModel[0].Result) : string.Empty));
		return lineSendResult;
	}

	public class TargetListStatus
	{
		string m_strName = null;
		string m_strTargetId = null;
		int m_iActionNo = -1;

		public TargetListStatus(string strName, string strTargetId, int iActionNo)
		{
			m_strName = strName;
			m_strTargetId = strTargetId;
			m_iActionNo = iActionNo;
		}

		public string Name
		{
			get { return m_strName; }
		}

		public string TargetId
		{
			get { return m_strTargetId; }
		}
		public int ActionNo
		{
			get { return m_iActionNo; }
		}

	}
}