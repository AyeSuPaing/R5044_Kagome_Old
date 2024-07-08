/*
=========================================================================================================
  Module      : トップページ処理(TopPage.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Cs.Incident;
using w2.App.Common.Cs.IncidentCategory;
using w2.App.Common.Cs.Message;
using w2.App.Common.Cs.Search;
using w2.App.Common.Cs.ShareInfo;

public partial class Form_Top_TopPage : BasePageCs
{
	#region 列挙体・定数
	/// <summary>最大表示件数</summary>
	private static int DISP_LIST_MAX = Constants.CONST_DISP_CONTENTS_CSTOP_LIST;
	#endregion

	#region #Page_Init ページ初期化
	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Init(object sender, EventArgs e)
	{
		ucListPager.PageChanged += PageChanged;
		ucListPager.DispListMax = DISP_LIST_MAX;
	}
	#endregion

	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// 「マイグループ」
			Constants.TEXT_DDLGROUPLIST_MYGROUP = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_TOP_PAGE,
				Constants.VALUETEXT_PARAM_GROUP_LIST,
				Constants.VALUETEXT_PARAM_MY_GROUP);
			// 「未設定」
			Constants.TEXT_DDLGROUPLIST_NOT_SET = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_TOP_PAGE,
				Constants.VALUETEXT_PARAM_GROUP_LIST,
				Constants.VALUETEXT_PARAM_NOT_SET);

			// ページ番号
			int pageNo;
			this.CurrentPageNumber = (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNo)) ? pageNo : 1;

			// ページ区分
			TopPageKbn topPageKbn;
			this.TopPageKbn = (Enum.TryParse(Request[Constants.REQUEST_KEY_CS_TOPPAGE_KBN], out topPageKbn)) ? topPageKbn : TopPageKbn.Top;
			ucIncidentAndMessageParts.TopPageKbn = this.TopPageKbn;

			// カテゴリセット
			var categoryService = new CsIncidentCategoryService(new CsIncidentCategoryRepository());
			var categoryModels = categoryService.GetCategoryTrailList(this.LoginOperatorDeptId, this.IncidentCategoryId);
			rCategoryNavigation.DataSource = categoryModels;
			rCategoryNavigation.DataBind();

			// ドロップダウンリスト項目設定
			var operatorGroupService = new CsOperatorGroupService(new CsOperatorGroupRepository());
			var operatorGroups = operatorGroupService.GetGroups(this.LoginOperatorDeptId, this.LoginOperatorId);
			var enableGroups = new CsGroupService(new CsGroupRepository()).GetValidAll(this.LoginOperatorDeptId);
			var otherGroups = enableGroups.Where(enable => operatorGroups.Any(operatorGroup => operatorGroup.CsGroupId == enable.CsGroupId) == false)
				.Select(other => other);
			var enableOperators = new CsOperatorService(new CsOperatorRepository()).GetValidAll(this.LoginOperatorDeptId);
			ddlGroupList.Items.Add(new ListItem(Constants.TEXT_DDLGROUPLIST_MYGROUP, Constants.TEXT_DDLGROUPLIST_MYGROUP));
			ddlGroupList.Items.AddRange(operatorGroups.Select(group => new ListItem("└" + group.CsGroupName, group.CsGroupId)).ToArray());
			ddlGroupList.Items.AddRange(otherGroups.Select(group => new ListItem(group.CsGroupName, group.CsGroupId)).ToArray());
			ddlGroupList.Items.Add(new ListItem(Constants.TEXT_DDLGROUPLIST_NOT_SET, ""));
			ddlOperatorList.Items.Add(new ListItem(this.LoginOperatorName, this.LoginOperatorId));
			ddlOperatorList.Items.AddRange(enableOperators.Where(enable => enable.OperatorId != this.LoginOperatorId)
				.Select(other => new ListItem(other.EX_ShopOperatorName, other.OperatorId)).ToArray());

			// ドロップダウンリスト選択状態設定
			if (Session[Constants.SESSION_KEY_DDLGROUPLIST_SELECTED] != null) ddlGroupList.SelectedIndex = (int)Session[Constants.SESSION_KEY_DDLGROUPLIST_SELECTED];
			if (Session[Constants.SESSION_KEY_DDLOPERATORLIST_SELECTED] != null) ddlOperatorList.SelectedIndex = (int)Session[Constants.SESSION_KEY_DDLOPERATORLIST_SELECTED];

			// リクエストパラメータで指定があればタスクステータスモードを設定
			TaskStatusRefineMode taskStatusMode;
			if (Enum.TryParse(Request[Constants.REQUEST_KEY_CS_TASKSTATUS_MODE], out taskStatusMode)) this.TaskStatusMode = taskStatusMode;

			// 検索リンククリック時は検索パラメタをクリア
			if (Request["SearchClear"] != null) Session[Constants.SESSION_KEY_CS_SEARCH_PARAMETER] = null;

			// リストリフレッシュ、表示件数更新（非同期処理）
			RefreshList();

			// Set value search
			SetValueSearch();

			MenuTaskCountManager.RefreshAllCountAsync(this.LoginOperatorDeptId, this.LoginOperatorId);
		}
	}
	#endregion

	#region -RefreshList リストリフレッシュ
	/// <summary>
	/// リストリフレッシュ
	/// </summary>
	private void RefreshList()
	{
		this.SearchTopParameter = (Request.QueryString[Constants.REQUEST_KEY_CS_SEARCH] == Constants.FLG_SEARCH_ON) ? this.SearchTopParameter : null;

		// グループドロップダウンリスト表示切替
		ddlGroupList.Enabled = (this.TaskTargetMode == TaskTargetMode.Group);

		// ユーザドロップダウンリスト表示切替
		ddlOperatorList.Enabled = (this.TaskTargetMode == TaskTargetMode.Personal);

		// リスト取得
		CsIncidentModel[] incidentList = null;
		CsMessageModel[] messageList = null;
		switch (this.TopPageKbn)
		{
			case TopPageKbn.Top:
				incidentList = SearchIncident(this.TaskTargetMode, this.TaskStatusMode, this.IncidentCategoryId, Constants.FLG_CSINCIDENT_VALID_FLG_VALID, this.SortIncidentKbn.ToString(), this.SortIncident, this.SearchTopParameter);
				break;

			case TopPageKbn.Draft:
				messageList = SearchMessageByCreateOerator(Constants.FLG_CSMESSAGE_MESSAGE_STATUS_DRAFT, this.SortMessageKbn.ToString(), this.SortMessage, this.SearchTopParameter);
				break;

			case TopPageKbn.Reply:
				messageList = SearchMessageByReplyOperator(this.SortMessageKbn.ToString(), this.SortMessage, this.SearchTopParameter);
				break;

			case TopPageKbn.Approval:
				messageList = SearchMessageRequestByApprovalOerator(Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_REQ, this.SortMessageKbn.ToString(), this.SortMessage, this.SearchTopParameter);
				break;

			case TopPageKbn.ApprovalRequest:
				messageList = SearchMessageRequestByRequestOerator(new[] { Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_REQ, Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_CANCEL }, this.SortMessageKbn.ToString(), this.SortMessage, this.SearchTopParameter);
				break;

			case TopPageKbn.ApprovalResult:
				messageList = SearchMessageRequestByRequestOerator(new[] { Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_OK, Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_NG }, this.SortMessageKbn.ToString(), this.SortMessage, this.SearchTopParameter);
				break;

			case TopPageKbn.Send:
				messageList = SearchMessageRequestByApprovalOerator(Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_REQ, this.SortMessageKbn.ToString(), this.SortMessage, this.SearchTopParameter);
				break;

			case TopPageKbn.SendRequest:
				messageList = SearchMessageRequestByRequestOerator(new[] { Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_REQ, Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_CANCEL }, this.SortMessageKbn.ToString(), this.SortMessage, this.SearchTopParameter);
				break;

			case TopPageKbn.SendResult:
				messageList = SearchMessageRequestByRequestOerator(new[] { Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_NG }, this.SortMessageKbn.ToString(), this.SortMessage, this.SearchTopParameter);
				break;

			case TopPageKbn.SearchIncident:
				ucSearchParts.Visible = true;
				divSearchTop.Visible = false;
				SearchParameter incSearchObject = (SearchParameter)Session[Constants.SESSION_KEY_CS_SEARCH_PARAMETER];
				incidentList = (incSearchObject != null) ? SearchIncidentAdvancedSearchByIncident(incSearchObject, this.SortIncidentKbn.ToString(), this.SortIncident) : null;
				if (incSearchObject == null) lbExportMessage.Visible = false;
				break;

			case TopPageKbn.SearchMessage:
				ucSearchParts.Visible = true;
				divSearchTop.Visible = false;
				SearchParameter msgSearchObject = (SearchParameter)Session[Constants.SESSION_KEY_CS_SEARCH_PARAMETER];
				messageList = (msgSearchObject != null) ? SearchMessageAdvancedSearch(msgSearchObject, this.SortMessageKbn.ToString(), this.SortMessage) : null;
				if (msgSearchObject == null) lbExportMessage.Visible = false;
				break;

			case TopPageKbn.TrashIncident:
				incidentList = SearchIncident(TaskTargetMode.All, TaskStatusRefineMode.All, "", Constants.FLG_CSINCIDENT_VALID_FLG_INVALID, this.SortIncidentKbn.ToString(), this.SortIncident, this.SearchTopParameter);
				break;

			case TopPageKbn.TrashMessage:
				messageList = SearchTrashMessage(this.SortMessageKbn.ToString(), this.SortMessage, this.SearchTopParameter);
				break;

			case TopPageKbn.TrashRequest:
				messageList = SearchTrashRequest(this.SortMessageKbn.ToString(), this.SortMessage, this.SearchTopParameter);
				break;

			case TopPageKbn.TrashDraft:
				messageList = SearchTrashDraft(this.SortMessageKbn.ToString(), this.SortMessage, this.SearchTopParameter);
				break;
		}

		// リスト表示
		if (incidentList != null)
		{
			DisplayIncidentList(incidentList);
		}
		else if (messageList != null)
		{
			DisplayMessageList(messageList);
		}
		else
		{
			taskList.Visible = false;
		}

		// リンク状態も変更
		ChangeTaskView();

		// 共有情報の未読件数を取得
		UpdateShareInfoCount();

		// 先頭選択
		ScriptManager.RegisterStartupScript(up1, up1.GetType(), "listselect_first", "listselect_first();", true);

		// 2ページ目で一覧更新したときに件数が減っていてページャが消えると１ページ目に戻れない件対応
		if ((this.CurrentPageNumber > 1)
			&& (((incidentList != null) && (incidentList.Length == 0))
				|| ((messageList != null) && (messageList.Length == 0))))
		{
			this.CurrentPageNumber = 1;
			RefreshList();
		}
	}
	#endregion

	#region -DisplayIncidentList インシデントリスト表示
	/// <summary>
	/// インシデントリスト表示
	/// </summary>
	/// <param name="incidentList">インシデントリスト表示</param>
	private void DisplayIncidentList(CsIncidentModel[] incidentList)
	{
		int searchCountTotal = (incidentList.Length != 0) ? incidentList[0].EX_SearchCount : 0;

		// Sort Incident List
		rIncidentList.DataSource = incidentList;
		rIncidentList.DataBind();
		lListCount.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(searchCountTotal));
		if (incidentList.Length == 0)
		{
			trIncidentListNoData.Visible = true;
			ucIncidentAndMessageParts.DisplayContents = false;
		}
		else
		{
			trIncidentListNoData.Visible = false;
		}
		divIncidentList.Visible = true;
		divMessageList.Visible = false;

		// Display After Sort Incident List
		DisplayAfterSortIncidentList();

		ucListPager.Visible = (searchCountTotal > DISP_LIST_MAX);
		if (ucListPager.Visible) ucListPager.DispPager(searchCountTotal, this.CurrentPageNumber);	// ページャ作成
	}
	#endregion

	#region -DisplayMessageList メッセージリスト表示
	/// <summary>
	/// メッセージリスト表示
	/// </summary>
	/// <param name="messageList">メッセージリスト表示</param>
	private void DisplayMessageList(CsMessageModel[] messageList)
	{
		int searchCountTotal = (messageList.Length != 0) ? messageList[0].EX_SearchCount : 0;

		// Sort Message List
		rMessageList.DataSource = messageList;
		rMessageList.DataBind();
		lListCount.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(searchCountTotal));
		trMessageListNoData.Visible = (messageList.Length == 0);
		divMessageList.Visible = true;
		divIncidentList.Visible = false;

		// Display After Sort Message List
		DisplayAfterSortMessageList();

		ucListPager.Visible = (searchCountTotal > DISP_LIST_MAX);
		if (ucListPager.Visible) ucListPager.DispPager(searchCountTotal, this.CurrentPageNumber);	// ページャ作成
	}
	#endregion

	#region インシデント最後のメッセージを取得
	/// <summary>
	/// インシデント最後のメッセージを取得
	/// </summary>
	/// <param name="deptId">識別ID</param>
	/// <param name="incidentId">インシデントID</param>
	/// <returns>CsMessageModel</returns>
	public CsMessageModel LastMessageByIncident(string deptId, string incidentId)
	{
		var messageService = new CsMessageService(new CsMessageRepository());
		var messageList = messageService.GetAll(deptId, incidentId);
		return (messageList.Length == 0) ? null : messageList.First();
	}
	#endregion

	#region -StyleListUpdateLockFlg
	/// <summary>
	/// 一括更新ロック判定
	/// </summary>
	/// <param name="lockStatus">ロックステータス</param>
	/// <returns>リスト</returns>
	protected bool StyleListUpdateLockFlg(string lockStatus)
	{
		return (this.LoginOperatorCsInfo.EX_PermitEditFlg
			&& (lockStatus == Constants.FLG_CSINCIDENT_LOCK_STATUS_NONE));
	}
	#endregion

	#region -PageChanged ページ変更イベント
	/// <summary>
	/// ページ変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void PageChanged(object sender, PagerEventArgs e)
	{
		this.CurrentPageNumber = e.PageNo;
		RefreshList();
	}
	#endregion

	#region -SearchMessageByCreateOerator 作成オペレータからメッセージ検索
	/// <summary>
	/// 作成オペレータからメッセージ検索
	/// </summary>
	/// <param name="messageStatus">メッセージステータス</param>
	/// <param name="sortField">Sort field</param>
	/// <param name="sortType">Sort type</param>
	/// <param name="searchParameter">Search parameter</param>
	/// <returns>リスト</returns>
	private CsMessageModel[] SearchMessageByCreateOerator(string messageStatus, string sortField, string sortType, SearchParameter searchParameter)
	{
		var service = new CsMessageService(new CsMessageRepository());
		var models = service.SearchValidByCreateOperatorAndStatus(
			this.LoginOperatorDeptId,
			this.LoginOperatorId,
			messageStatus,
			this.BeginRow,
			this.EndRow,
			sortField,
			sortType,
			searchParameter);
		return models;
	}
	#endregion

	#region -SearchMessageByReplyOperator 回答オペレータからメッセージ検索
	/// <summary>
	/// 回答オペレータでメッセージ検索
	/// </summary>
	/// <param name="sortField">Sort field</param>
	/// <param name="sortType">Sort type</param>
	/// <param name="searchParameter">Search parameter</param>
	/// <returns>リスト</returns>
	private CsMessageModel[] SearchMessageByReplyOperator(string sortField, string sortType, SearchParameter searchParameter)
	{
		var service = new CsMessageService(new CsMessageRepository());
		var models = service.SearchValidByReplyOperator(
			this.LoginOperatorDeptId,
			this.LoginOperatorId,
			this.BeginRow,
			this.EndRow,
			sortField,
			sortType,
			searchParameter);
		return models;
	}
	#endregion

	#region -SearchMessageRequestByApprovalOerator 承認オペレータから依頼メッセージ検索
	/// <summary>
	/// 承認オペレータから依頼メッセージ検索
	/// </summary>
	/// <param name="messageStatus">メッセージステータス</param>
	/// <param name="sortField">Sort field</param>
	/// <param name="sortType">Sort type</param>
	/// <param name="searchParameter">Search parameter</param>
	/// <returns>リスト</returns>
	private CsMessageModel[] SearchMessageRequestByApprovalOerator(string messageStatus, string sortField, string sortType, SearchParameter searchParameter)
	{
		var service = new CsMessageService(new CsMessageRepository());
		var models = service.SearchValidRequestByApprovalOperatorAndStatus(
			this.LoginOperatorDeptId,
			this.LoginOperatorId,
			messageStatus,
			this.BeginRow,
			this.EndRow,
			sortField,
			sortType,
			searchParameter);
		return models;
	}
	#endregion

	#region -SearchMessageRequestByRequestOerator 依頼オペレータから依頼メッセージ検索
	/// <summary>
	/// 依頼オペレータから依頼メッセージ検索
	/// </summary>
	/// <param name="messageStatuses">メッセージステータス</param>
	/// <param name="sortField">Sort field</param>
	/// <param name="sortType">Sort type</param>
	/// <param name="searchParameter">Search parameter</param>
	/// <returns>リスト</returns>
	private CsMessageModel[] SearchMessageRequestByRequestOerator(string[] messageStatuses, string sortField, string sortType, SearchParameter searchParameter)
	{
		var service = new CsMessageService(new CsMessageRepository());
		var models = service.SearchValidRequestByRequestOperator(
			this.LoginOperatorDeptId,
			this.LoginOperatorId,
			messageStatuses,
			this.BeginRow,
			this.EndRow,
			sortField,
			sortType,
			searchParameter);
		return models;
	}
	#endregion

	#region -SearchMessageAdvancedSearch 検索画面からメッセージ詳細検索
	/// <summary>
	/// 検索画面からメッセージ詳細検索
	/// </summary>
	/// <param name="searchObject">検索パラメータ</param>
	/// <param name="sortField">Sort field</param>
	/// <param name="sortType">Sort type</param>
	/// <returns>リスト</returns>
	private CsMessageModel[] SearchMessageAdvancedSearch(SearchParameter searchObject, string sortField, string sortType)
	{
		CsMessageService service = new CsMessageService(new CsMessageRepository());
		return service.SearchAdvanced(this.LoginOperatorDeptId, searchObject, this.BeginRow, this.EndRow, sortField, sortType);
	}
	#endregion

	#region -SearchIncidentAdvancedSearchByIncident 検索画面からメッセージ詳細検索（インシデント単位で取得）
	/// <summary>
	/// 検索画面からメッセージ詳細検索（インシデント単位で取得）
	/// </summary>
	/// <param name="searchObject">検索パラメータ</param>
	/// <param name="sortField">Sort field</param>
	/// <param name="sortType">Sort type</param>
	/// <returns>リスト</returns>
	private CsIncidentModel[] SearchIncidentAdvancedSearchByIncident(SearchParameter searchObject, string sortField, string sortType)
	{
		var service = new CsMessageService(new CsMessageRepository());
		var incidents = service.SearchAdvancedByIncident(
			this.LoginOperatorDeptId,
			searchObject,
			this.BeginRow,
			this.EndRow,
			sortField,
			sortType);

		// インシデント最後のメッセージを取得、メッセージアイコン、コメントの取得に使用
		var incidentIdList = incidents.Select(inc => inc.IncidentId).ToArray();
		var lastMessages = Array.Empty<CsMessageModel>();
		if (incidentIdList.Any())
		{
			var messageService = new CsMessageService(new CsMessageRepository());
			lastMessages = messageService.GetLastMessageByIncidentIds(this.LoginOperatorDeptId, incidentIdList);
		}

		if (lastMessages.Any() == false) return incidents;

		foreach (var incident in incidents)
		{
			incident.LastMessage = lastMessages.FirstOrDefault(msg => msg.IncidentId == incident.IncidentId);
		}

		return incidents;
	}
	#endregion

	#region -SearchTrashMessage ゴミ箱メッセージ検索
	/// <summary>
	/// ゴミ箱メッセージ検索
	/// </summary>
	/// <param name="sortField">Sort field</param>
	/// <param name="sortType">Sort type</param>
	/// <param name="searchParameter">Search parameter</param>
	/// <returns>リスト</returns>
	private CsMessageModel[] SearchTrashMessage(string sortField, string sortType, SearchParameter searchParameter)
	{
		var service = new CsMessageService(new CsMessageRepository());
		var models = service.SearchByCreateOperatorAndStatusAndValidFlg(
			this.LoginOperatorDeptId,
			null,	// ゴミ箱 - メッセージ は全てのオペレータのメッセージを出力する
			new string[] {Constants.FLG_CSMESSAGE_MESSAGE_STATUS_DONE, Constants.FLG_CSMESSAGE_MESSAGE_STATUS_RECEIVED},
			Constants.FLG_CSMESSAGE_VALID_FLG_INVALID,
			this.BeginRow,
			this.EndRow,
			sortField,
			sortType,
			searchParameter);
		return models;
	}
	#endregion

	#region -SearchTrashRequest ゴミ箱メッセージ（承認／送信代行）検索
	/// <summary>
	/// ゴミ箱メッセージ（承認／送信代行）リスト検索
	/// </summary>
	/// <param name="sortField">Sort field</param>
	/// <param name="sortType">Sort type</param>
	/// <param name="searchParameter">Search parameter</param>
	/// <returns>リスト</returns>
	private CsMessageModel[] SearchTrashRequest(string sortField, string sortType, SearchParameter searchParameter)
	{
		var service = new CsMessageService(new CsMessageRepository());
		var models = service.SearchByCreateOperatorAndStatusAndValidFlg(
			this.LoginOperatorDeptId,
			this.LoginOperatorId,
			new string[] {
				Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_REQ,
				Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_OK,
				Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_NG,
				Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_CANCEL,
				Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_REQ,
				Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_NG,
				Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_CANCEL},
			Constants.FLG_CSMESSAGE_VALID_FLG_INVALID,
			this.BeginRow,
			this.EndRow,
			sortField,
			sortType,
			searchParameter);
		return models;
	}
	#endregion

	#region -SearchTrashDraft ゴミ箱メッセージ（下書き）検索
	/// <summary>
	/// ゴミ箱メッセージ（下書き）リスト検索
	/// </summary>
	/// <param name="sortField">Sort field</param>
	/// <param name="sortType">Sort type</param>
	/// <param name="searchParameter">Search parameter</param>
	/// <returns>リスト</returns>
	private CsMessageModel[] SearchTrashDraft(string sortField, string sortType, SearchParameter searchParameter)
	{
		var service = new CsMessageService(new CsMessageRepository());
		var models = service.SearchByCreateOperatorAndStatusAndValidFlg(
			this.LoginOperatorDeptId,
			this.LoginOperatorId,
			new string[] {Constants.FLG_CSMESSAGE_MESSAGE_STATUS_DRAFT},
			Constants.FLG_CSMESSAGE_VALID_FLG_INVALID,
			this.BeginRow,
			this.EndRow,
			sortField,
			sortType,
			searchParameter);
		return models;
	}
	#endregion

	#region #lbRefineTaskStatus_Click タスクステータス絞り込みリンクボタンクリック
	/// <summary>
	/// タスクステータス絞り込みリンクボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRefineTaskStatus_Click(object sender, EventArgs e)
	{
		this.CurrentPageNumber = 1;
		this.TaskStatusMode = (TaskStatusRefineMode)Enum.Parse(typeof(TaskStatusRefineMode), ((LinkButton)sender).CommandArgument);

		// Set link search
		SetSearchParamToSession(this.TaskStatusMode.ToString());
		RedirectLinkSearch(this.TaskStatusMode.ToString());

		RefreshList();

		ucIncidentAndMessageParts.DisplayContents = false;
	}
	#endregion

	#region #lbChangeTaskTarget_Click タスクターゲット変更リンククリック
	/// <summary>
	/// タスクターゲット変更リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbChangeTaskTarget_Click(object sender, EventArgs e)
	{
		this.CurrentPageNumber = 1;

		// ターゲット切替
		this.TaskTargetMode = (TaskTargetMode)Enum.Parse(typeof(TaskTargetMode), ((LinkButton)sender).CommandArgument);

		// タスク表示
		RefreshList();
	}
	#endregion

	#region -ChangeTaskView タスク部分の見た目表示変更
	/// <summary>
	/// タスク部分の見た目表示変更
	/// </summary>
	private void ChangeTaskView()
	{
		ChangeRefineTaskLinkView(lbRefineTaskUncomplete, (this.TaskStatusMode == TaskStatusRefineMode.Uncomplete));
		ChangeRefineTaskLinkView(lbRefineTaskNone, (this.TaskStatusMode == TaskStatusRefineMode.None));
		ChangeRefineTaskLinkView(lbRefineTaskActive, (this.TaskStatusMode == TaskStatusRefineMode.Active));
		ChangeRefineTaskLinkView(lbRefineTaskSuspend, (this.TaskStatusMode == TaskStatusRefineMode.Suspend));
		ChangeRefineTaskLinkView(lbRefineTaskUrgent, (this.TaskStatusMode == TaskStatusRefineMode.Urgent));
		ChangeRefineTaskLinkView(lbRefineTaskComplete, (this.TaskStatusMode == TaskStatusRefineMode.Complete));
		ChangeRefineTaskLinkView(lbRefineTaskAll, (this.TaskStatusMode == TaskStatusRefineMode.All));
		
		ChangeRefineTaskLinkView(lbChangeTaskTargetGroup, (this.TaskTargetMode == TaskTargetMode.Group));
		ChangeRefineTaskLinkView(lbChangeTaskTargetPersonal, (this.TaskTargetMode == TaskTargetMode.Personal));
		ChangeRefineTaskLinkView(lbChangeTaskTargetAll, (this.TaskTargetMode == TaskTargetMode.All));
		ChangeRefineTaskLinkView(lbChangeTaskTargetUnassign, (this.TaskTargetMode == TaskTargetMode.Unassign));
	}
	#endregion

	#region -ChangeRefineTaskLinkView タスク絞り込みリンク見た目表示変更
	/// <summary>
	/// タスク絞り込みリンク見た目表示変更
	/// </summary>
	/// <param name="lb">リンクボタン</param>
	/// <param name="enabled">有効にするか</param>
	private void ChangeRefineTaskLinkView(LinkButton lb, bool enabled)
	{
		lb.Enabled = (enabled == false);
		lb.Font.Bold = enabled;
	}
	#endregion

	#region -SearchIncident インシデント検索
	/// <summary>
	/// インシデント検索
	/// </summary>
	/// <param name="targetMode">タスクターゲットモード</param>
	/// <param name="refineMode">タスク絞り込みモード</param>
	/// <param name="incidentCategoryId">インシデントカテゴリID</param>
	/// <param name="validFlg">有効フラグ</param>
	/// <param name="sortField">Sort field</param>
	/// <param name="sortType">Sort type</param>
	/// <param name="searchParameter">Search parameter</param>
	/// <returns>リスト</returns>
	private CsIncidentModel[] SearchIncident(TaskTargetMode targetMode, TaskStatusRefineMode refineMode, string incidentCategoryId, string validFlg, string sortField, string sortType, SearchParameter searchParameter)
	{
		var operatorId = this.LoginOperatorId;
		if (this.TopPageKbn == TopPageKbn.Top)
		{
			// ddlGroupListが有効かつddlGroupListのSelectedIndexが0(0はマイグループ)でない場合はグループで絞り込みを行う。
			if (ddlGroupList.Enabled && (ddlGroupList.SelectedIndex != 0))
			{
				targetMode = TaskTargetMode.OneGroup;
			}

			if (ddlOperatorList.Enabled)
			{
				operatorId = ddlOperatorList.SelectedValue;
			}
		}

		var service = new CsIncidentService(new CsIncidentRepository());
		var result = service.Search(
			this.LoginOperatorDeptId,
			(targetMode == TaskTargetMode.Unassign) ? string.Empty : operatorId,
			GetStatusDbValueFromTaskStatusRefineModeType(refineMode),
			incidentCategoryId,
			validFlg,
			GetTagrgetOperatorGroupTypeForSearchIncident(targetMode),
			this.BeginRow,
			this.EndRow,
			sortField,
			sortType,
			ddlGroupList.SelectedValue,
			searchParameter);

		// インシデント最後のメッセージを取得、メッセージアイコン、コメントの取得に使用
		var incidentIdList = result.Select(inc => inc.IncidentId).ToArray();

		var lastMessages = Array.Empty<CsMessageModel>();
		if (incidentIdList.Any())
		{
			var messageService = new CsMessageService(new CsMessageRepository());
			lastMessages = messageService.GetLastMessageByIncidentIds(this.LoginOperatorDeptId, incidentIdList);
		}

		var categoryService = new CsIncidentCategoryService(new CsIncidentCategoryRepository());
		var categories = categoryService.GetAll(this.LoginOperatorDeptId);
		foreach (var incident in result)
		{
			// 表示文言の組み立て：カテゴリ欄
			if (incident.IncidentCategoryId != string.Empty)
			{
				var categoryModel = categories.FirstOrDefault(p => p.CategoryId == incident.IncidentCategoryId);
				if ((categoryModel != null) && (categoryModel.EX_IsParentValid == false))
				{
					incident.EX_IncidentCategoryName += GetDispTextInvalid(true);
				}
			}

			// 表示文言の組み立て：担当オペレータ欄
			if (incident.OperatorId != string.Empty)
			{
				if (incident.EX_IsOperatorValid == false)
				{
					incident.EX_CsOperatorName += GetDispTextInvalid(true);
				}
			}

			// 表示文言の組み立て：担当グループ欄
			if (incident.CsGroupId != string.Empty)
			{
				if (incident.EX_IsGroupValid == false)
				{
					incident.EX_CsGroupName += GetDispTextInvalid(true);
				}
			}

			// インシデント最後のメッセージとインシデントを紐づける
			if (lastMessages.Any())
			{
				incident.LastMessage = lastMessages.FirstOrDefault(msg => msg.IncidentId == incident.IncidentId);
			}
		}

		return result.ToArray();
	}
	#endregion

	#region -GetTagrgetOperatorGroupTypeForSearchIncident インシデント検索向けオペレータグループ対象タイプ取得
	/// <summary>
	/// インシデント検索向けオペレータグループ対象タイプ取得
	/// </summary>
	/// <param name="targetMode">タスクターゲットモード</param>
	/// <returns>オペレータグループ対象タイプ文字列</returns>
	private string GetTagrgetOperatorGroupTypeForSearchIncident(TaskTargetMode targetMode)
	{
		string tagrgetOperatorGroupType = null;
		switch (targetMode)
		{
			case TaskTargetMode.Personal:
			case TaskTargetMode.Unassign:
				tagrgetOperatorGroupType = "1";
				break;

			case TaskTargetMode.Group:
				switch (Constants.SETTING_CSTOP_GROUP_TASK_DISPLAY_MODE)
				{
					case Constants.GroupTaskDisplayModeType.OnlyAssigned:
						tagrgetOperatorGroupType = "2-1";
						break;
					case Constants.GroupTaskDisplayModeType.IncludeUnassigned:
						tagrgetOperatorGroupType = "2-2";
						break;
				}
				break;

			case TaskTargetMode.All:
				tagrgetOperatorGroupType = "0";
				break;

			case TaskTargetMode.OneGroup:
				tagrgetOperatorGroupType = "2-3";
				break;
		}
		return tagrgetOperatorGroupType;
	}
	#endregion

	#region -GetDbValueFromTaskStatusRefineModeType 絞り込みモードタイプからDB値へ変換
	/// <summary>
	/// 絞り込みモードタイプからDB値へ変換
	/// </summary>
	/// <param name="statusRefineMode">絞り込みモード</param>
	/// <returns>DB値</returns>
	private string[] GetStatusDbValueFromTaskStatusRefineModeType(TaskStatusRefineMode statusRefineMode)
	{
		switch (statusRefineMode)
		{
			case TaskStatusRefineMode.Uncomplete:
				return new[] { Constants.FLG_CSINCIDENT_STATUS_NONE, Constants.FLG_CSINCIDENT_STATUS_ACTIVE, Constants.FLG_CSINCIDENT_STATUS_SUSPEND, Constants.FLG_CSINCIDENT_STATUS_URGENT };

			case TaskStatusRefineMode.None:
				return new[] { Constants.FLG_CSINCIDENT_STATUS_NONE };

			case TaskStatusRefineMode.Active:
				return new[] { Constants.FLG_CSINCIDENT_STATUS_ACTIVE };

			case TaskStatusRefineMode.Suspend:
				return new[] { Constants.FLG_CSINCIDENT_STATUS_SUSPEND };

			case TaskStatusRefineMode.Urgent:
				return new[] { Constants.FLG_CSINCIDENT_STATUS_URGENT };

			case TaskStatusRefineMode.Complete:
				return new[] { Constants.FLG_CSINCIDENT_STATUS_COMPLETE };

			case TaskStatusRefineMode.All:
				return new[] { Constants.FLG_CSINCIDENT_STATUS_NONE, Constants.FLG_CSINCIDENT_STATUS_ACTIVE, Constants.FLG_CSINCIDENT_STATUS_SUSPEND, Constants.FLG_CSINCIDENT_STATUS_URGENT, Constants.FLG_CSINCIDENT_STATUS_COMPLETE };

			default:
				return new string[0];
		}
	}
	#endregion

	#region #lbSelectList_Click リスト選択リンククリック
	/// <summary>
	/// リスト選択リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSelectList_Click(object sender, EventArgs e)
	{
		switch (this.TopPageKbn)
		{
			case TopPageKbn.Top:
			case TopPageKbn.TrashIncident:
			case TopPageKbn.SearchIncident:
				ucIncidentAndMessageParts.DispIncidentAndLastMessage(hfIncidentId.Value);
				break;

			case TopPageKbn.Approval:
			case TopPageKbn.ApprovalRequest:
			case TopPageKbn.ApprovalResult:
			case TopPageKbn.Send:
			case TopPageKbn.SendRequest:
			case TopPageKbn.SendResult:
			case TopPageKbn.Draft:
			case TopPageKbn.Reply:
			case TopPageKbn.SearchMessage:
			case TopPageKbn.TrashMessage:
			case TopPageKbn.TrashRequest:
			case TopPageKbn.TrashDraft:
				ucIncidentAndMessageParts.DispIncidentAndMessage(hfIncidentId.Value, int.Parse(hfMessageNo.Value));
				break;
		}
		ucIncidentAndMessageParts.DisplayContents = true;

		ScriptManager.RegisterStartupScript(this, this.GetType(), "MyAction", "select_bottom_message_list();", true);
	}
	#endregion

	#region #lbReceiveMail_Click メール取込ボタンクリック
	/// <summary>
	/// メール取込ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReceiveMail_Click(object sender, EventArgs e)
	{
		int mailCount = ExecReceiveMailBatch();

		// リストリフレッシュ、表示件数更新（非同期処理）
		RefreshList();
		MenuTaskCountManager.RefreshAllCountAsync(this.LoginOperatorDeptId, this.LoginOperatorId);

		if (mailCount >= 0)
		{
			receivedCount.Text = WebMessages.GetMessages(WebMessages.ERROR_MANAGER_RECEIVED).Replace("@@ 1 @@", mailCount.ToString());
		}
		else if (mailCount == -2)	// 二重起動エラー
		{
			receivedCount.Text = WebMessages.GetMessages(WebMessages.ERROR_MANAGER_RECEIVING_WHILE);
		}
		else
		{
			receivedCount.Text = WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR);
		}
	}
	#endregion

	#region #lbExport_Click CSVダウンロードリンククリック
	/// <summary>
	/// CSVダウンロードリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbExport_Click(object sender, EventArgs e)
	{
		// パラメタ作成
		SearchSqlParameter searchSqlParam = new SearchSqlParameter((SearchParameter)Session[Constants.SESSION_KEY_CS_SEARCH_PARAMETER]);
		searchSqlParam.InputValues.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, this.LoginOperatorDeptId);
		searchSqlParam.InputValues[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN] = Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_CSMESSAGE;
		searchSqlParam.InputValues["@@ keywords_where @@"] = searchSqlParam.WhereStatement;
		searchSqlParam.InputValues["sqlParamList"] = searchSqlParam.SqlParams;
		Session[Constants.SESSION_KEY_PARAM] = searchSqlParam.InputValues;

		// CSV出力
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MASTEREXPORT);
	}
	#endregion 

	#region #GetFromString 問合せ元文字列取得
	/// <summary>
	/// 問合せ元文字列取得
	/// </summary>
	/// <param name="incident">インシデント</param>
	/// <param name="isTitle">タイトル</param>
	/// <returns>問合せ元文字列</returns>
	protected string GetFromString(CsIncidentModel incident, bool isTitle = false)
	{
		string result;
		
		if (isTitle == false)
		{
			result = string.IsNullOrEmpty(incident.UserId)
				? string.IsNullOrEmpty(incident.UserName)
					? incident.UserContact
					: incident.UserName
				: incident.EX_UserName;
			return result.Trim();
		}
		
		result = string.IsNullOrEmpty(incident.UserId)
			? string.Join(
				"\r\n",
				incident.UserName,
				incident.UserContact)
			: string.Join(
				"\r\n",
				incident.UserId,
				incident.EX_UserKbnText,
				incident.EX_UserName,
				(string.IsNullOrEmpty(incident.EX_UserMailAddr1) ? "" : incident.EX_UserMailAddr1)
				+(string.IsNullOrEmpty(incident.EX_UserMailAddr2) ? "" : incident.EX_UserMailAddr2),
				incident.EX_UserTel1,
				incident.EX_UserTel2);
		return result.Trim();
	}
	#endregion

	#region #GetFromString 問合せ元文字列取得
	/// <summary>
	/// 問合せ元文字列取得
	/// </summary>
	/// <param name="message">メッセージ</param>
	/// <returns>問合せ元文字列</returns>
	protected string GetFromString(CsMessageModel message)
	{
		return (message.EX_Mail != null) ? message.EX_Mail.MailFrom : message.UserName1 + message.UserName2 + "\r\n" + message.UserTel1;
	}
	#endregion

	#region #lbRefresh_Click リフレッシュボタンクリック
	/// <summary>
	/// リフレッシュボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRefresh_Click(object sender, EventArgs e)
	{
		// リストリフレッシュ、表示件数更新（非同期処理）
		RefreshList();
		MenuTaskCountManager.RefreshAllCountAsync(this.LoginOperatorDeptId, this.LoginOperatorId);

		ucIncidentAndMessageParts.DisplayContents = false;
	}
	#endregion

	#region #lbRefreshIncidentAndMessageBottom_Click 下部インシデント・メッセージ更新リンククリック
	/// <summary>
	/// 下部インシデント・メッセージ更新リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRefreshIncidentAndMessageBottom_Click(object sender, EventArgs e)
	{
		ucIncidentAndMessageParts.RefreshIncidentAndMessage();
	}
	#endregion

	#region #lbRefreshShareInfoCount_Click 共有情報未読件数の更新ボタンクリック
	/// <summary>
	/// 共有情報未読件数の更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRefreshShareInfoCount_Click(object sender, EventArgs e)
	{
		UpdateShareInfoCount();
		upShareInfoCount.DataBind();
	}
	#endregion

	#region #lbTrashIncidents_Click 一括ゴミ箱投入ボタンクリック
	/// <summary>
	/// 一括ゴミ箱投入ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbTrashIncidents_Click(object sender, EventArgs e)
	{
		if (divIncidentList.Visible)
		{
			var incidentIds = GetCheckedIncidents();
			MoveToTrashIncidentsWithMessages(incidentIds);
		}
		else if (divMessageList.Visible)
		{
			var messages = GetCheckedMessages();
			MoveToTrashMessages(messages);
		}
		// リストリフレッシュ、表示件数更新（非同期処理）
		RefreshList();
		MenuTaskCountManager.RefreshAllCountAsync(this.LoginOperatorDeptId, this.LoginOperatorId);
	}
	#endregion

	#region -MoveToTrashIncidentsWithMessages インシデントとそれに紐付くメッセージをゴミ箱へ移動する（ロックされていないもののみ）
	/// <summary>
	/// インシデントとそれに紐付くメッセージをゴミ箱へ移動する（ロックされていないもののみ）
	/// </summary>
	/// <param name="incidentIds">インシデントID配列</param>
	private void MoveToTrashIncidentsWithMessages(string[] incidentIds)
	{
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		foreach (var incidentId in incidentIds)
		{
			var model = incidentService.GetWithSummaryValues(this.LoginOperatorDeptId, incidentId);
			if (model.LockStatus != Constants.FLG_CSINCIDENT_LOCK_STATUS_NONE) continue;

			model.ValidFlg = Constants.FLG_CSINCIDENT_VALID_FLG_INVALID;
			model.LastChanged = this.LoginOperatorName;
			incidentService.UpdateValidFlgWithMessage(model);
		}
	}
	#endregion

	#region -MoveToTrashMessages メッセージをゴミ箱へ移動する（ロックされていないもののみ）
	/// <summary>
	/// メッセージをゴミ箱へ移動する（ロックされていないもののみ）
	/// </summary>
	/// <param name="messages">インシデントID＆メッセージNOの配列</param>
	private void MoveToTrashMessages(KeyValuePair<string, int>[] messages)
	{
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		var messageService = new CsMessageService(new CsMessageRepository());
		foreach (var message in messages)
		{
			var incidnet = incidentService.GetWithSummaryValues(this.LoginOperatorDeptId, message.Key);
			if (incidnet.LockStatus != Constants.FLG_CSINCIDENT_LOCK_STATUS_NONE) continue;

			var model = new CsMessageModel();
			model.DeptId = this.LoginOperatorDeptId;
			model.IncidentId = message.Key;
			model.MessageNo = message.Value;
			model.ValidFlg = Constants.FLG_CSMESSAGE_VALID_FLG_INVALID;
			model.LastChanged = this.LoginOperatorName;
			messageService.UpdateValidFlg(model);
		}
	}
	#endregion

	#region #lbDeleteIncidents_Click 一括削除ボタンクリック
	/// <summary>
	/// 一括削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDeleteIncidents_Click(object sender, EventArgs e)
	{
		if (divIncidentList.Visible)
		{
			var incidentIds = GetCheckedIncidents();
			DeleteIncidentsWithAllMessages(incidentIds);
		}
		else if (divMessageList.Visible)
		{
			var messages = GetCheckedMessages();
			DeleteMessages(messages);
		}
		// リストリフレッシュ、表示件数更新（非同期処理）
		RefreshList();
		MenuTaskCountManager.RefreshAllCountAsync(this.LoginOperatorDeptId, this.LoginOperatorId);
	}
	#endregion

	#region -DeleteMessages メッセージを削除する
	/// <summary>
	/// メッセージを削除する
	/// </summary>
	/// <param name="messages">インシデントID、メッセージNO配列</param>
	private void DeleteMessages(KeyValuePair<string, int>[] messages)
	{
		var messageService = new CsMessageService(new CsMessageRepository());
		foreach (var message in messages)
		{
			messageService.DeleteWithRequestAndMails(this.LoginOperatorDeptId, message.Key, message.Value);
		}
	}
	#endregion

	#region -DeleteIncidentsWithAllMessages インシデントとそれに紐付くメッセージを削除する
	/// <summary>
	/// インシデントとそれに紐付くメッセージを削除する
	/// </summary>
	/// <param name="incidentIds">インシデントID配列</param>
	private void DeleteIncidentsWithAllMessages(string[] incidentIds)
	{
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		var messageService = new CsMessageService(new CsMessageRepository());
		foreach (var incidentId in incidentIds)
		{
			incidentService.Delete(this.LoginOperatorDeptId, incidentId);
			messageService.DeleteAllWithMails(this.LoginOperatorDeptId, incidentId);
		}
	}
	#endregion

	#region -GetCheckedIncidents チェックされているインシデント一覧取得
	/// <summary>
	/// チェックされているインシデント一覧取得
	/// </summary>
	/// <returns>インシデントID配列</returns>
	private string[] GetCheckedIncidents()
	{
		var targets = (from RepeaterItem item in rIncidentList.Items
					   where ((CheckBox)item.FindControl("cbCheck")).Checked
					   select ((HiddenField)item.FindControl("hfIncidentId")).Value);
		return targets.ToArray();
	}
	#endregion

	#region -GetCheckedMessages チェックされているメッセージ一覧取得
	/// <summary>
	/// チェックされているメッセージ一覧取得
	/// </summary>
	/// <returns>インシデントID、メッセージNO配列</returns>
	private KeyValuePair<string, int>[] GetCheckedMessages()
	{
		var targets = (from RepeaterItem item in rMessageList.Items
					   where ((CheckBox)item.FindControl("cbCheck")).Checked
					   select new KeyValuePair<string, int>(((HiddenField)item.FindControl("hfIncidentId")).Value, int.Parse(((HiddenField)item.FindControl("hfMessageNo")).Value)));
		return targets.ToArray();
	}
	#endregion

	#region -UpdateShareInfoCount 共有情報未読件数をDBから取得して更新
	/// <summary>
	/// 共有情報未読件数をDBから取得して更新
	/// </summary>
	private void UpdateShareInfoCount()
	{
		var service = new CsShareInfoReadService(new CsShareInfoReadRepository());
		this.ShareInfoCount = service.GetOperatorUnreadCount(this.LoginOperatorDeptId, this.LoginOperatorId);
	}
	#endregion

	#region Sort incident and message list
	/// <summary>
	/// Sort Incident List
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event</param>
	protected void IncidentListSort_Click(object sender, EventArgs e)
	{
		var sortIncidentKbnCurrent = (SortIncidentKbn)Enum.Parse(typeof(SortIncidentKbn), ((LinkButton)sender).CommandArgument);
		this.SortIncident = GetSortFromSortKbn(this.SortIncidentKbn.ToString(), sortIncidentKbnCurrent.ToString(), this.SortIncident);
		this.SortIncidentKbn = sortIncidentKbnCurrent;

		// Display After Sort Incident List
		RefreshList();
		DisplayAfterSortIncidentList();
	}

	/// <summary>
	/// Sort Message List
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event</param>
	protected void MessageListSort_Click(object sender, EventArgs e)
	{
		var sortMessageKbnCurrent = (SortMessageKbn)Enum.Parse(typeof(SortMessageKbn), ((LinkButton)sender).CommandArgument);
		this.SortMessage = GetSortFromSortKbn(this.SortMessageKbn.ToString(), sortMessageKbnCurrent.ToString(), this.SortMessage);
		this.SortMessageKbn = sortMessageKbnCurrent;

		// Display After Sort Message List
		RefreshList();
		DisplayAfterSortMessageList();
	}

	/// <summary>
	/// Display After Sort Incident List
	/// </summary>
	protected void DisplayAfterSortIncidentList()
	{
		ChangeRefineTaskLinkSort(lIncidentIdIconSort, (this.SortIncidentKbn == SortIncidentKbn.IncidentId), this.SortIncident);
		ChangeRefineTaskLinkSort(lIncidentTitleIconSort, (this.SortIncidentKbn == SortIncidentKbn.IncidentTitle), this.SortIncident);
		ChangeRefineTaskLinkSort(lInquirySourceIconSort, (this.SortIncidentKbn == SortIncidentKbn.EX_InquirySource), this.SortIncident);
		ChangeRefineTaskLinkSort(lIncidentCategoryNameIconSort, (this.SortIncidentKbn == SortIncidentKbn.EX_IncidentCategoryName), this.SortIncident);
		ChangeRefineTaskLinkSort(lStatusTextIconSort, (this.SortIncidentKbn == SortIncidentKbn.EX_StatusText), this.SortIncident);
		ChangeRefineTaskLinkSort(lCsOperatorNameIconSort, (this.SortIncidentKbn == SortIncidentKbn.EX_CsOperatorName), this.SortIncident);
		ChangeRefineTaskLinkSort(lCsGroupNameIconSort, (this.SortIncidentKbn == SortIncidentKbn.EX_CsGroupName), this.SortIncident);
		ChangeRefineTaskLinkSort(lDateLastReceivedIconSort, (this.SortIncidentKbn == SortIncidentKbn.DateLastReceived), this.SortIncident);
		ChangeRefineTaskLinkSort(lDateMessageLastSendReceivedIconSort, (this.SortIncidentKbn == SortIncidentKbn.DateMessageLastSendReceived), this.SortIncident);
		ChangeRefineTaskLinkSort(lDateChangedIconSort, (this.SortIncidentKbn == SortIncidentKbn.DateChanged), this.SortIncident);
	}

	/// <summary>
	/// Display After Sort Message List
	/// </summary>
	protected void DisplayAfterSortMessageList()
	{
		ChangeRefineTaskLinkSort(lIncidentIdMessageIconSort, (this.SortMessageKbn == SortMessageKbn.IncidentId), this.SortMessage);
		ChangeRefineTaskLinkSort(lInquiryTitleIconSort, (this.SortMessageKbn == SortMessageKbn.InquiryTitle), this.SortMessage);
		ChangeRefineTaskLinkSort(lSenderIconSort, (this.SortMessageKbn == SortMessageKbn.EX_Sender), this.SortMessage);
		ChangeRefineTaskLinkSort(lReplyOperatorNameIconSort, (this.SortMessageKbn == SortMessageKbn.EX_NameOperator), this.SortMessage);
		ChangeRefineTaskLinkSort(lOperatorNameIconSort, (this.SortMessageKbn == SortMessageKbn.EX_NameOperator), this.SortMessage);
		ChangeRefineTaskLinkSort(lMessageStatusNameIconSort, (this.SortMessageKbn == SortMessageKbn.EX_MessageStatusName), this.SortMessage);
		ChangeRefineTaskLinkSort(lDateChangedMessageIconSort, (this.SortMessageKbn == SortMessageKbn.DateChanged), this.SortMessage);
		ChangeRefineTaskLinkSort(lInquiryReplyDateIconSort, (this.SortMessageKbn == SortMessageKbn.InquiryReplyDate), this.SortMessage);
		ChangeRefineTaskLinkSort(lRequestDateCreatedIconSort, (this.SortMessageKbn == SortMessageKbn.EX_Request), this.SortMessage);
		ChangeRefineTaskLinkSort(lDateCreatedIconSort, (this.SortMessageKbn == SortMessageKbn.DateCreated), this.SortMessage);
	}

	#endregion

	#region btnSearchTop Click
	/// <summary>
	/// Click button search top
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearchTop_Click(object sender, EventArgs e)
	{
		var status = Request.QueryString[Constants.REQUEST_KEY_CS_TASKSTATUS_MODE];

		SetSearchParamToSession(status);
		RedirectLinkSearch(status);
	}
	#endregion

	#region Search incident and message list
	/// <summary>
	/// Redirect link search
	/// </summary>
	/// <param name="topPageKbn">Top page kbn</param>
	private void RedirectLinkSearch(string statusMode)
	{
		var topPageKbn = Request.QueryString[Constants.REQUEST_KEY_CS_TOPPAGE_KBN];

		if (string.IsNullOrEmpty(topPageKbn)) topPageKbn = TopPageKbn.Top.ToString();

		var linkSearch = string.Format("{0}{1}?{2}={3}&{4}={5}&{6}={7}&{8}={9}&{10}={11}",
			Constants.PATH_ROOT,
			Constants.PAGE_W2CS_MANAGER_TOP_PAGE,
			Constants.REQUEST_KEY_CS_TOPPAGE_KBN,
			topPageKbn,
			Constants.REQUEST_KEY_CS_TASKSTATUS_MODE,
			statusMode,
			Constants.REQUEST_KEY_INCIDENT_CATEGORY_ID,
			Request.QueryString[Constants.REQUEST_KEY_INCIDENT_CATEGORY_ID],
			Constants.REQUEST_KEY_CS_SEARCH,
			Constants.FLG_SEARCH_ON,
			Constants.REQUEST_KEY_CS_SEARCH_KEYWORD,
			HttpUtility.UrlEncode(tbKeywordTop.Text));

		Response.Redirect(linkSearch);
	}

	/// <summary>
	/// Set value search
	/// </summary>
	private void SetValueSearch()
	{
		if (this.SearchTopParameter == null) return;

		// Key word
		tbKeywordTop.Text = Request.QueryString[Constants.REQUEST_KEY_CS_SEARCH_KEYWORD];
	}

	/// <summary>
	/// Set search param to session
	/// </summary>
	/// <param name="status">Status</param>
	private void SetSearchParamToSession(string status)
	{
		SearchParameter searchParameter = new SearchParameter();
		searchParameter.Keyword = tbKeywordTop.Text;
		searchParameter.Mode = SearchMode.All;
		searchParameter.Target = SearchTarget.SearchTop;
		searchParameter.StatusFilter = status;
		searchParameter.CategoryFilter = Request.QueryString[Constants.REQUEST_KEY_INCIDENT_CATEGORY_ID];

		searchParameter.SummaryValues = new Dictionary<int, string>();
		searchParameter.SummarySettingTypes = new Dictionary<int, string>();

		// 検索条件をセッションに保存
		this.SearchTopParameter = searchParameter;
	}
	#endregion

	#region +IsLockStatus ロックステータスか判定
	/// <summary>
	/// ロックステータスか判定
	/// </summary>
	/// <param name="model">CSメッセージモデル</param>
	/// <returns>ロックステータスか</returns>
	public bool IsLockStatus(CsMessageModel model)
	{
		var lockStatus = new CsIncidentService(new CsIncidentRepository()).Get(model.DeptId, model.IncidentId).LockStatus;

		return (lockStatus != Constants.FLG_CSINCIDENT_LOCK_STATUS_NONE);
	}
	#endregion

	/// <summary>
	/// インシデント警告アイコンを表示するか判定
	/// </summary>
	/// <param name="incident"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	protected bool IsNeedToDisplayWarningIcon(CsIncidentModel incident, CsMessageModel message)
	{
		if (message == null) return false;

		if ((incident.OperatorId != this.LoginOperatorId)
			&& (string.IsNullOrEmpty(incident.OperatorId) == false))
		{
			return false;
		}

		// 自オペレータが所属していないグループが担当なら表示しない
		if ((this.LoginOperatorCsGroupIds.Contains(incident.CsGroupId) == false)
			&& (string.IsNullOrEmpty(incident.CsGroupId) == false))
		{
			return false;
		}

		if ((message.MessageStatus != Constants.FLG_CSMESSAGE_MESSAGE_STATUS_RECEIVED)
			&& (message.MessageStatus != Constants.FLG_CSMESSAGE_MESSAGE_STATUS_DONE))
		{
			return false;
		}

		return ((message.MediaKbn == Constants.FLG_CSMESSAGE_MEDIA_KBN_TEL)
			|| (message.DirectionKbn == Constants.FLG_CSMESSAGE_DIRECTION_KBN_RECEIVE));
	}

	/// <summary>
	/// グループタスクドロップダウンリスト変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlGroupList_SelectedIndexChanged(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_DDLGROUPLIST_SELECTED] = ddlGroupList.SelectedIndex;
		RefreshList();
	}

	/// <summary>
	/// 個人タスクドロップダウンリスト変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlOperatorList_SelectedIndexChanged(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_DDLOPERATORLIST_SELECTED] = ddlOperatorList.SelectedIndex;
		RefreshList();
	}

	#region プロパティ
	/// <summary>ページ区分</summary>
	protected TopPageKbn TopPageKbn
	{
		get { return (TopPageKbn)ViewState["TopPageKbn"]; }
		set { ViewState["TopPageKbn"] = value; }
	}
	/// <summary>カテゴリID</summary>
	protected string IncidentCategoryId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_INCIDENT_CATEGORY_ID]); }
	}
	/// <summary>タスクターゲットモード</summary>
	protected TaskTargetMode TaskTargetMode
	{
		get { return (Session["TaskTargetMode"] != null) ? (TaskTargetMode)Session["TaskTargetMode"] : TaskTargetMode.Group; }
		set { Session["TaskTargetMode"] = value; }
	}
	/// <summary>タスクステータス</summary>
	protected TaskStatusRefineMode TaskStatusMode
	{
		get { return (Session["TaskStatus"] != null) ? (TaskStatusRefineMode)Session["TaskStatus"] : (TaskStatusRefineMode)(Session["TaskStatus"] = TaskStatusRefineMode.None); }
		set { Session["TaskStatus"] = value; }
	}
	/// <summary>ページNO</summary>
	private int CurrentPageNumber
	{
		get { return (int)ViewState["CurrentPageNumber"]; }
		set { ViewState["CurrentPageNumber"] = value; }
	}
	/// <summary>開始行NO</summary>
	private int BeginRow
	{
		get { return (this.CurrentPageNumber - 1) * DISP_LIST_MAX + 1; }
	}
	/// <summary>終了行NO</summary>
	private int EndRow
	{
		get { return this.CurrentPageNumber * DISP_LIST_MAX; }
	}
	/// <summary>共有情報の未読件数</summary>
	private int ShareInfoCount
	{
		get { return (int)ViewState["ShareInfoCount"]; }
		set { ViewState["ShareInfoCount"] = value; }
	}
	/// <summary>共有情報の未読件数のchar配列</summary>
	protected char[] ShareInfoCountString
	{
		get { return (ShareInfoCount == 0) ? null : (ShareInfoCount).ToString().ToArray(); }
	}
	/// <summary>一覧でゴミ箱投入可能</summary>
	protected bool CanTrashOnList
	{
		get { return ((this.CanDeleteOnList == false)
			&& (this.TopPageKbn != TopPageKbn.Approval)
			&& (this.TopPageKbn != TopPageKbn.Send)); }
	}
	/// <summary>一覧で削除可能</summary>
	protected bool CanDeleteOnList
	{
		get { return ((this.TopPageKbn == TopPageKbn.TrashIncident)
			|| (this.TopPageKbn == TopPageKbn.TrashMessage)
			|| (this.TopPageKbn == TopPageKbn.TrashRequest)
			|| (this.TopPageKbn == TopPageKbn.TrashDraft)); }
	}
	/// <summary>承認/送信代行または下書きページであるかもしくは完全削除権限が付与されているか</summary>
	protected bool IsTrashPageForNotOfficiallySentMessageOrIsAuthorizedOperatorToPermanentlyDelete
	{
		get
		{
			return ((this.TopPageKbn == TopPageKbn.TrashRequest) 
				|| (this.TopPageKbn == TopPageKbn.TrashDraft)
				|| this.LoginOperatorCsInfo.EX_PermitPermanentDeleteFlg);
		}
	}
	/// <summary>Sort Incident</summary>
	protected string SortIncident
	{
		get{ return (string)Session["SortIncident"] ?? Constants.FLG_SORT_KBN_DESC; }
		set { Session["SortIncident"] = value; }
	}
	/// <summary>Sort Message</summary>
	protected string SortMessage
	{
		get { return (string)Session["SortMessage"] ?? Constants.FLG_SORT_KBN_DESC; }
		set { Session["SortMessage"] = value; }
	}
	/// <summary>Sort Incident Kbn</summary>
	protected SortIncidentKbn SortIncidentKbn
	{
		get
		{
			// 他の画面へ切り替えても、ソートを実施した「日時」系の列のソート情報は保持する　（「日時」系の列：　更新日時、受付日時）
			var sortKbn = (SortIncidentKbn?)Session["SortIncidentKbn"] ?? SortIncidentKbn.IncidentId;

			if ((sortKbn == SortIncidentKbn.DateChanged)
				|| (sortKbn == SortIncidentKbn.DateLastReceived)
				|| (sortKbn == SortIncidentKbn.DateMessageLastSendReceived))
			{
				return (this.TopPageKbn == TopPageKbn.TrashIncident) ? SortIncidentKbn.DateChanged : sortKbn;
			}

			return sortKbn;
		}
		set { Session["SortIncidentKbn"] = value; }
	}
	/// <summary>Sort Message Kbn</summary>
	protected SortMessageKbn SortMessageKbn
	{
		get
		{
			// 他の画面へ切り替えても、ソートを実施した「日時」系の列のソート情報は保持する　（「日時」系の列：　更新日時、受付日時、回答日時、依頼日時、作成日）
			var sortKbn = (SortMessageKbn?)Session["SortMessageKbn"] ?? SortMessageKbn.IncidentId;

			if ((sortKbn == SortMessageKbn.DateChanged)
				|| (sortKbn == SortMessageKbn.InquiryReplyDate)
				|| (sortKbn == SortMessageKbn.EX_Request)
				|| (sortKbn == SortMessageKbn.DateCreated))
			{
				switch (this.TopPageKbn)
				{
					case TopPageKbn.Draft:
					case TopPageKbn.TrashMessage:
					case TopPageKbn.TrashRequest:
					case TopPageKbn.TrashDraft:
						sortKbn = SortMessageKbn.DateChanged;
						break;

					case TopPageKbn.Reply:
						sortKbn = SortMessageKbn.InquiryReplyDate;
						break;

					case TopPageKbn.Approval:
					case TopPageKbn.ApprovalRequest:
					case TopPageKbn.ApprovalResult:
					case TopPageKbn.Send:
					case TopPageKbn.SendRequest:
					case TopPageKbn.SendResult:
						sortKbn = SortMessageKbn.EX_Request;
						break;

					default:
						sortKbn = SortMessageKbn.DateCreated;
						break;
				}
			}

			return sortKbn;
		}
		set { Session["SortMessageKbn"] = value; }
	}
	/// <summary>Search top parameter</summary>
	private SearchParameter SearchTopParameter
	{
		get { return (SearchParameter)Session[Constants.SESSION_KEY_CS_SEARCH_TOP_PARAMETER]; }
		set { Session[Constants.SESSION_KEY_CS_SEARCH_TOP_PARAMETER] = value; }
	}
	#endregion
}
