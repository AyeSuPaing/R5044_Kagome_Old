/*
=========================================================================================================
  Module      : メニュータスク件数管理クラス(MenuTaskCountManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Cs.Message;
using w2.Domain.CsIncident;

/// <summary>
/// MenuTaskCountManager の概要の説明です
/// </summary>
public class MenuTaskCountManager
{
	/// <summary>全オペレータの、ステータスごとの、未読/未対応なインシデントの件数を格納します。</summary>
	private static Dictionary<string, Dictionary<TaskStatusRefineMode, int>> m_incidentCountPersonal = new Dictionary<string, Dictionary<TaskStatusRefineMode, int>>();
	/// <summary>全オペレータの、ステータスごとの、未読/未対応なインシデントの件数を格納します。</summary>
	private static Dictionary<string, Dictionary<TaskStatusRefineMode, int>> m_incidentCountGroup = new Dictionary<string, Dictionary<TaskStatusRefineMode, int>>();
	/// <summary>全オペレータの、それぞれの区分の、未読/未対応なメッセージの件数を格納します。</summary>
	private static Dictionary<string, Dictionary<TopPageKbn, int>> m_messageCount = new Dictionary<string, Dictionary<TopPageKbn, int>>();

	/// <summary>
	/// ステータス絞り込みモードを指定して、未対応なインシデントの件数を取得します。
	/// </summary>
	/// <param name="operatorId">オペレータID</param>
	/// <param name="taskStatusType">ステータス絞り込みモード</param>
	/// <param name="target">ターゲットモード</param>
	/// <returns>未対応インシデント件数、指定条件での件数がなければnull</returns>
	public static int? GetIncidentCount(string operatorId, TaskStatusRefineMode taskStatusType, TaskTargetMode target)
	{
		switch (target)
		{
			case TaskTargetMode.Personal:
				lock (m_incidentCountPersonal)
				{
					return (m_incidentCountPersonal.ContainsKey(operatorId) && m_incidentCountPersonal[operatorId].ContainsKey(taskStatusType)) ? (int?)m_incidentCountPersonal[operatorId][taskStatusType] : null;
				}

			case TaskTargetMode.Group:
				lock (m_incidentCountGroup)
				{
					return (m_incidentCountGroup.ContainsKey(operatorId) && m_incidentCountGroup[operatorId].ContainsKey(taskStatusType)) ? (int?)m_incidentCountGroup[operatorId][taskStatusType] : null;
				}

			default:
				throw new Exception("未対応の絞り込みモード：" + target.ToString());
		}
	}

	/// <summary>
	/// トップページ区分を指定して、未対応なメッセージの件数を取得します。
	/// </summary>
	/// <param name="operatorId">オペレータID</param>
	/// <param name="topPageKbn">トップページ区分</param>
	/// <param name="target">ターゲットモード</param>
	/// <returns>未対応メッセージ件数、指定条件での件数がなければnull</returns>
	public static int? GetMessageCount(string operatorId, TopPageKbn topPageKbn, TaskTargetMode target)
	{
		lock (m_messageCount)
		{
			return (m_messageCount.ContainsKey(operatorId) && m_messageCount[operatorId].ContainsKey(topPageKbn)) ? (int?)m_messageCount[operatorId][topPageKbn] : null;
		}
	}

	/// <summary>
	/// 指定したオペレータの、指定したインシデントステータスに対して、未読/未対応件数を設定します。
	/// </summary>
	/// <param name="operatorId">オペレータID</param>
	/// <param name="taskStatusType">ステータス絞り込みモード</param>
	/// <param name="target">ターゲットモード</param>
	/// <param name="count">未対応件数</param>
	private static void SetIncidentCount(string operatorId, TaskStatusRefineMode taskStatusType, TaskTargetMode target, int count)
	{
		switch (target)
		{
			case TaskTargetMode.Personal:
				lock (m_incidentCountPersonal)
				{
					if (m_incidentCountPersonal.ContainsKey(operatorId) == false) m_incidentCountPersonal.Add(operatorId, new Dictionary<TaskStatusRefineMode, int>());
					m_incidentCountPersonal[operatorId][taskStatusType] = count;
				}
				break;

			case TaskTargetMode.Group:
				lock (m_incidentCountGroup)
				{
					if (m_incidentCountGroup.ContainsKey(operatorId) == false) m_incidentCountGroup.Add(operatorId, new Dictionary<TaskStatusRefineMode, int>());
					m_incidentCountGroup[operatorId][taskStatusType] = count;
				}
				break;

			default:
				throw new Exception("未対応の絞り込みモード：" + target.ToString());
		}
	}

	/// <summary>
	/// 指定したオペレータの、指定したトップページ区分に対して、未読/未対応件数を設定します。
	/// </summary>
	/// <param name="operatorId">オペレータID</param>
	/// <param name="topPageKbn">トップページ区分</param>
	/// <param name="count">未対応件数</param>
	private static void SetMessageCount(string operatorId, TopPageKbn topPageKbn, int count)
	{
		lock (m_messageCount)
		{
			if (m_messageCount.ContainsKey(operatorId) == false) m_messageCount.Add(operatorId, new Dictionary<TopPageKbn, int>());
			m_messageCount[operatorId][topPageKbn] = count;
		}
	}

	/// <summary>
	/// 指定したオペレータの、すべての未読/未対応件数を、データベースから取得して非同期で更新します。
	/// </summary>
	/// <param name="deptId">識別ID</param>
	/// <param name="operatorId">オペレータID</param>
	public static void RefreshAllCountAsync(string deptId, string operatorId)
	{
		Action action = () =>
		{
			try
			{
				RefreshAllCount(deptId, operatorId);
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
			}
		};
		action.BeginInvoke(null, null);
		System.Threading.Thread.Sleep(100);
	}

	/// <summary>
	/// 指定したオペレータの、すべての未読/未対応件数を、データベースから取得して更新します。
	/// </summary>
	/// <param name="deptId">識別ID</param>
	/// <param name="operatorId">オペレータID</param>
	public static void RefreshAllCount(string deptId, string operatorId)
	{
		CsIncidentModel[] incidentsPersonal, incidentsGroup = null;
		var includeUnassigned = (Constants.SETTING_CSTOP_GROUP_TASK_DISPLAY_MODE == Constants.GroupTaskDisplayModeType.IncludeUnassigned);
		var countInfo = new CsIncidentService().CountTask(deptId, operatorId, includeUnassigned);
		incidentsPersonal = countInfo.First;
		incidentsGroup = countInfo.Second;

		CsMessageModel[] messages, messagesByRequest, messagesByRequestItem = null;
		var messageService = new CsMessageService(new CsMessageRepository());
		messages = messageService.GetCount(deptId, operatorId);
		messagesByRequest = messageService.GetCountByRequest(deptId, operatorId);
		messagesByRequestItem = messageService.GetCountByRequestItem(deptId, operatorId);

		var opService = new CsOperatorService(new CsOperatorRepository());
		var op = opService.Get(deptId, operatorId);

		// インシデント（個人タスク）
		RefreshIncidentCount(incidentsPersonal, op, TaskTargetMode.Personal);

		// インシデント（グループタスク）
		RefreshIncidentCount(incidentsGroup, op, TaskTargetMode.Group);

		// メッセージ
		RefreshMessageCount(messages, messagesByRequest, messagesByRequestItem, op);
	}

	/// <summary>
	/// 指定したオペレータの、指定したタスクターゲットモードのインシデント件数を集計します。
	/// </summary>
	/// <param name="incidentsPersonal">個人タスクであるインシデントリスト</param>
	/// <param name="op">オペレータモデル</param>
	/// <param name="taskTargetMode">タスクターゲットモード</param>
	private static void RefreshIncidentCount(CsIncidentModel[] incidentsPersonal, CsOperatorModel op, TaskTargetMode taskTargetMode)
	{
		int cntNone = 0, cntActive = 0, cntSuspend = 0, cntUrgent = 0, cntUnassigned = 0;
		foreach (CsIncidentModel inc in incidentsPersonal)
		{
			if (inc.OperatorId == op.OperatorId && inc.Status == Constants.FLG_CSINCIDENT_STATUS_NONE) { cntNone = inc.EX_SearchCount; continue; }
			if (inc.OperatorId == op.OperatorId && inc.Status == Constants.FLG_CSINCIDENT_STATUS_ACTIVE) { cntActive = inc.EX_SearchCount; continue; }
			if (inc.OperatorId == op.OperatorId && inc.Status == Constants.FLG_CSINCIDENT_STATUS_SUSPEND) { cntSuspend = inc.EX_SearchCount; continue; }
			if (inc.OperatorId == op.OperatorId && inc.Status == Constants.FLG_CSINCIDENT_STATUS_URGENT) { cntUrgent = inc.EX_SearchCount; continue; }
			if (inc.OperatorId == "" && (
				(inc.Status == Constants.FLG_CSINCIDENT_STATUS_NONE) ||
				(inc.Status == Constants.FLG_CSINCIDENT_STATUS_ACTIVE) ||
				(inc.Status == Constants.FLG_CSINCIDENT_STATUS_SUSPEND) ||
				(inc.Status == Constants.FLG_CSINCIDENT_STATUS_URGENT))) { cntUnassigned += inc.EX_SearchCount; continue; }
		}
		MenuTaskCountManager.SetIncidentCount(op.OperatorId, TaskStatusRefineMode.None, taskTargetMode, cntNone);
		MenuTaskCountManager.SetIncidentCount(op.OperatorId, TaskStatusRefineMode.Active, taskTargetMode, cntActive);
		MenuTaskCountManager.SetIncidentCount(op.OperatorId, TaskStatusRefineMode.Suspend, taskTargetMode, cntSuspend);
		MenuTaskCountManager.SetIncidentCount(op.OperatorId, TaskStatusRefineMode.Urgent, taskTargetMode, cntUrgent);
	}

	/// <summary>
	/// 指定したオペレータの、メッセージ件数を集計します。
	/// </summary>
	/// <param name="messages">メッセージリスト</param>
	/// <param name="messagesByRequest">メッセージリスト（メッセージ依頼単位）</param>
	/// <param name="messagesByRequestItem">メッセージリスト（メッセージ依頼アイテム単位）</param>
	/// <param name="op">オペレータモデル</param>
	private static void RefreshMessageCount(CsMessageModel[] messages, CsMessageModel[] messagesByRequest, CsMessageModel[] messagesByRequestItem, CsOperatorModel op)
	{
		int cntDraft = 0, cntAppr = 0, cntApprReq = 0, cntApprResult = 0, cntSend = 0, cntSendReq = 0, cntSendResult = 0;
		foreach (CsMessageModel msg in messages)
		{
			if (msg.OperatorId == op.OperatorId && msg.EX_IsDraft) { cntDraft = msg.EX_SearchCount; continue; }
		}
		foreach (CsMessageModel msg in messagesByRequest)
		{
			if (msg.EX_RequestOperatorId == op.OperatorId && IsApprovalRequest(msg.MessageStatus)) { cntApprReq += msg.EX_SearchCount; continue; }
			if (msg.EX_RequestOperatorId == op.OperatorId && IsApprovalResult(msg.MessageStatus)) { cntApprResult += msg.EX_SearchCount; continue; }
			if (msg.EX_RequestOperatorId == op.OperatorId && IsSendRequest(msg.MessageStatus)) { cntSendReq += msg.EX_SearchCount; continue; }
			if (msg.EX_RequestOperatorId == op.OperatorId && IsSendResult(msg.MessageStatus)) { cntSendResult += msg.EX_SearchCount; continue; }
		}
		foreach (CsMessageModel msg in messagesByRequestItem)
		{
			if (msg.EX_ApprOperatorId == op.OperatorId && msg.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_REQ) { cntAppr = msg.EX_SearchCount; continue; }
			if (msg.EX_ApprOperatorId == op.OperatorId && msg.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_REQ) { cntSend = msg.EX_SearchCount; continue; }
		}

		MenuTaskCountManager.SetMessageCount(op.OperatorId, TopPageKbn.Draft, cntDraft);
		MenuTaskCountManager.SetMessageCount(op.OperatorId, TopPageKbn.Approval, cntAppr);
		MenuTaskCountManager.SetMessageCount(op.OperatorId, TopPageKbn.ApprovalRequest, cntApprReq);
		MenuTaskCountManager.SetMessageCount(op.OperatorId, TopPageKbn.ApprovalResult, cntApprResult);
		MenuTaskCountManager.SetMessageCount(op.OperatorId, TopPageKbn.Send, cntSend);
		MenuTaskCountManager.SetMessageCount(op.OperatorId, TopPageKbn.SendRequest, cntSendReq);
		MenuTaskCountManager.SetMessageCount(op.OperatorId, TopPageKbn.SendResult, cntSendResult);
	}

	/// <summary>
	/// 承認依頼中のステータスかを判定します。
	/// </summary>
	/// <param name="status">メッセージステータス</param>
	/// <returns>判定結果</returns>
	private static bool IsApprovalRequest(string status)
	{
		return ((status == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_REQ) || (status == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_CANCEL));
	}

	/// <summary>
	/// 承認結果返却済みのステータスかを判定します。
	/// </summary>
	/// <param name="status">メッセージステータス</param>
	/// <returns>判定結果</returns>
	private static bool IsApprovalResult(string status)
	{
		return ((status == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_OK) || (status == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_NG));
	}

	/// <summary>
	/// 送信依頼中のステータスかを判定します。
	/// </summary>
	/// <param name="status">メッセージステータス</param>
	/// <returns>判定結果</returns>
	private static bool IsSendRequest(string status)
	{
		return ((status == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_REQ) || (status == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_CANCEL));
	}

	/// <summary>
	/// 送信結果返却済みのステータスかを判定します。
	/// </summary>
	/// <param name="status">メッセージステータス</param>
	/// <returns>判定結果</returns>
	private static bool IsSendResult(string status)
	{
		return (status == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_NG);
	}
}
