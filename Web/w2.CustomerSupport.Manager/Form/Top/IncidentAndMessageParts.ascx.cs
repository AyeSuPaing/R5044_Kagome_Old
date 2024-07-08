/*
=========================================================================================================
  Module      : インシデント＆メッセージパーツユーザーコントロール処理(IncidentAndMessageParts.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.User;
using w2.App.Common.Cs.Incident;
using w2.App.Common.Cs.Message;

public partial class Form_Top_IncidentAndMessageParts : IncidentUserControl
{
	#region 列挙体
	/// <summary>左表示モード種別</summary>
	protected enum LeftDispModeType
	{
		/// <summary>メッセージ</summary>
		MessageMain,
		/// <summary>メッセージプロパティ</summary>
		MessageProperty
	}
	/// <summary>右表示モード種別</summary>
	protected enum RightDispModeType
	{
		/// <summary>インシデント</summary>
		IncidentMain,
		/// <summary>インシデントプロパティ</summary>
		IncidentProperty
	}
	/// <summary>ページ区分種別</summary>
	private enum PageKbnType
	{
		/// <summary>TOP</summary>
		Top,
		/// <summary>承認依頼</summary>
		ApprovalRequest
	}
	#endregion

	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			InitializeComponent();

			ChangeLeftDispMode(LeftDispModeType.MessageMain);
			ChangeRightDispMode(RightDispModeType.IncidentMain);
		}
	}
	#endregion

	#region -InitializeComponent 初期化処理
	/// <summary>
	/// 初期化処理
	/// </summary>
	private void InitializeComponent()
	{
		// インシデントカテゴリ作成
		ddlIncidentCategory.Items.Add("");
		ddlIncidentCategory.Items.AddRange(CreateIncidentCategoryItems());
		// ステータスセット
		ddlIncidentStatus.Items.AddRange(CreateIncidentStatusItems());
		// 重要度セット
		ddlImportance.Items.AddRange(CreateIncidentImportanceItems());
		ddlImportance.SelectedValue = Constants.FLG_CSINCIDENT_IMPORTANCE_MIDDLE;
		// VOCセット
		ddlVoc.Items.Add("");
		ddlVoc.Items.AddRange(CreateIncidentVocItems());
		// グループ
		ddlCsGroups.Items.Add("");
		ddlCsGroups.Items.AddRange(CreateCsGroupItems());
		// オペレータ
		ddlCsOperators.Items.Add("");
		ddlCsOperators.Items.AddRange(CreateCsOperatorItems(ddlCsGroups.SelectedValue));
		// 集計区分セット
		rIncidentSummary.DataSource = CsSummarySettingArray();
		rIncidentSummary.DataBind();
	}
	#endregion

	#region +DispIncidentAndLastMessage インシデント情報・最終メッセージ表示（メッセージ一覧も）
	/// <summary>
	/// インシデント情報・最終メッセージ表示（メッセージ一覧も）
	/// </summary>
	/// <param name="incidentId">インシデントID</param>
	public void DispIncidentAndLastMessage(string incidentId)
	{
		DisplayAll(incidentId);
	}
	#endregion

	#region +DispIncidentAndMessage インシデント情報・メッセージ表示（メッセージ一覧も）
	/// <summary>
	/// インシデント情報・メッセージ表示（メッセージ一覧も）
	/// </summary>
	/// <param name="incidentId">インシデントID</param>
	/// <param name="messageNo">問合せNO</param>
	public void DispIncidentAndMessage(string incidentId, int messageNo)
	{
		DisplayAll(incidentId, messageNo);
	}
	#endregion

	#region -DisplayAll すべて表示
	/// <summary>
	/// すべて表示
	/// </summary>
	/// <param name="incidentId">インシデントID</param>
	/// <param name="messageNo">メッセージNO（nullの場合は先頭を表示）</param>
	private void DisplayAll(string incidentId, int? messageNo = null)
	{
		SetIncidentToProperty(incidentId);
		DispIncident();

		var incidentMessages = GetIncidentMessageHistoryList(incidentId);
		DispIncidentMessageHistoryList(incidentMessages);

		if (messageNo.HasValue)
		{
			SetMessageToProperty(incidentId, messageNo.Value);
		}
		else if (incidentMessages.Length > 0)
		{
			SetMessageToProperty(incidentId, incidentMessages[0].MessageNo);
		}
		else
		{
			SetMessageToProperty(incidentId, null);
		}
		DispMessage();

		ChangeLeftDispMode(this.LeftDispMode);
		ChangeRightDispMode(this.RightDispMode);

		// Display sort
		DisplayAfterSortMessageRightList();
	}
	#endregion

	#region +RefreshIncidentAndMessage インシデントとメッセージリフレッシュ
	/// <summary>
	/// インシデントとメッセージリフレッシュ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>ポップアップ編集のロック状態などをポップアップ先から反映する際に利用する</remarks>
	public void RefreshIncidentAndMessage()
	{
		SetIncidentToProperty(hfIncidentId.Value);
		DispIncident();

		if (hfMessageNo.Value != "") SetMessageToProperty(hfIncidentId.Value, int.Parse(hfMessageNo.Value));
		DispMessage();
	}
	#endregion

	#region -DispIncident インシデント情報表示
	/// <summary>
	/// インシデント情報表示
	/// </summary>
	private void DispIncident()
	{
		if (this.Incident == null) return;
		var incident = this.Incident;

		this.IsIncidentLocked = (incident.LockStatus != "");

		// 右側／メインエリア
		ScriptManager.RegisterStartupScript(up3, up3.GetType(), "refresh_incident_lock_icon", "refresh_incident_lock_icon('" + incident.IncidentId + "', " + this.IsIncidentLocked.ToString().ToLower() + ");", true);
		lLockMessage.Visible = this.IsIncidentLocked;
		lLockMessage.Text = WebMessages.GetMessages(WebMessages.MSG_MANAGER_INCIDENT_AND_MESSAGE_PARTS_MAIN_AREA_BY_INSIDE)
			.Replace("@@ 1 @@", incident.EX_LockOperatorName)
			.Replace("@@ 2 @@", incident.EX_LockStatusName);

		btnUnlock.Visible = GetLockUnlockable();

		lIncidentDeletedString1.Visible = (incident.ValidFlg == Constants.FLG_CSINCIDENT_VALID_FLG_INVALID);
		lIncidentId1.Text = WebSanitizer.HtmlEncode(incident.IncidentId);
		lIncidentTitle.Text = WebSanitizer.HtmlEncode(incident.IncidentTitle);
		lIncidentCategory.Text = WebSanitizer.HtmlEncode(incident.EX_IncidentCategoryName);
		lIncidentStatus.Text = WebSanitizer.HtmlEncode(incident.EX_StatusText);
		lIncidentImportance.Text = WebSanitizer.HtmlEncode(incident.EX_ImportanceText);
		lIncidentOperatorName.Text = WebSanitizer.HtmlEncode(incident.EX_CsOperatorName);
		lIncidentGroupName.Text = WebSanitizer.HtmlEncode(incident.EX_CsGroupName);
		lUserId.Text = incident.UserId;

		// 右側・プロパティ
		imgLock2.Visible = this.IsIncidentLocked;
		lIncidentDeletedString2.Visible = (incident.ValidFlg == Constants.FLG_CSINCIDENT_VALID_FLG_INVALID);
		lIncidentId2.Text = WebSanitizer.HtmlEncode(incident.IncidentId);
		hfIncidentId.Value = incident.IncidentId;
		tbUserId.Text = incident.UserId;
		tbIncidentTitle.Text = incident.IncidentTitle;
		foreach (ListItem li in ddlIncidentCategory.Items) li.Selected = (li.Value == incident.IncidentCategoryId);
		foreach (ListItem li in ddlIncidentStatus.Items) li.Selected = (li.Value == incident.Status);
		foreach (ListItem li in ddlImportance.Items) li.Selected = (li.Value == incident.Importance);
		foreach (ListItem li in ddlVoc.Items) li.Selected = (li.Value == incident.VocId);
		tbVocMemo.Text = incident.VocMemo;
		foreach (ListItem li in ddlCsGroups.Items) li.Selected = (li.Value == incident.CsGroupId);
		RecreateCsOperatorDropDown(ddlCsOperators, ddlCsGroups.SelectedValue);
		foreach (ListItem li in ddlCsOperators.Items) li.Selected = (li.Value == incident.OperatorId);
		hfCsOperatorBefore.Value = incident.OperatorId;
		tbIncidentComment.Text = incident.Comment;
		// 集計区分値
		foreach (RepeaterItem ri in rIncidentSummary.Items)
		{
			var hfSummaryNo = (HiddenField)ri.FindControl("hfSummaryNo");
			var hfSummarySettingType = (HiddenField)ri.FindControl("hfSummarySettingType");
			var rblSummaryValue = (RadioButtonList)ri.FindControl("rblSummaryValue");
			var ddlSummaryValue = (DropDownList)ri.FindControl("ddlSummaryValue");
			var tbSummaryValue = (TextBox)ri.FindControl("tbSummaryValue");

			var values = incident.EX_SummaryValues.Where(v => v.SummaryNo == int.Parse(hfSummaryNo.Value)).ToArray();
			var value = (values.Length != 0) ? values[0].Value : null;

			switch (hfSummarySettingType.Value)
			{
				case Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_RADIO:
					rblSummaryValue.Items[0].Selected = true;	// デフォルト選択
					foreach (ListItem li in rblSummaryValue.Items) li.Selected = (li.Value == value);
					break;

				case Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_DROPDOWN:
					foreach (ListItem li in ddlSummaryValue.Items) li.Selected = (li.Value == value);
					break;

				case Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_TEXT:
					tbSummaryValue.Text = value;
					break;
			}
		}
		lIncidentLastChanged.Text = WebSanitizer.HtmlEncode(incident.LastChanged);

		spIncidentButtonAreaUndeleted.Visible = (incident.ValidFlg == Constants.FLG_CSINCIDENT_VALID_FLG_VALID);
		spIncidentButtonAreaDeleted.Visible = (incident.ValidFlg == Constants.FLG_CSINCIDENT_VALID_FLG_INVALID);
	}
	#endregion

	#region -SetIncidentToProperty インシデントをプロパティへセット
	/// <summary>
	/// インシデントをプロパティへセット
	/// </summary>
	/// <param name="incidentId">インシデントID</param>
	private void SetIncidentToProperty(string incidentId)
	{
		hfIncidentId.Value = incidentId;
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		this.Incident = incidentService.GetWithSummaryValues(this.LoginOperatorDeptId, incidentId);
	}
	#endregion

	#region -SetMessageToProperty メッセージをプロパティへセット
	/// <summary>
	/// メッセージをプロパティへセット
	/// </summary>
	/// <param name="incidentId">インシデントID</param>
	/// <param name="messageNo">メッセージNO（nullの場合はメッセージ無し）</param>
	private void SetMessageToProperty(string incidentId, int? messageNo = null)
	{
		hfIncidentId.Value = incidentId;

		if (messageNo.HasValue)
		{
			hfMessageNo.Value = messageNo.ToString();

			var messageService = new CsMessageService(new CsMessageRepository());
			this.Message = messageService.GetWithMail(this.LoginOperatorDeptId, incidentId, messageNo.Value);
		}
		else
		{
			hfMessageNo.Value = "";
			this.Message = null;
		}
	}
	#endregion

	#region -GetLockUnlockable ロック解除可能か取得
	/// <summary>
	/// ロック解除可能か取得
	/// </summary>
	/// <param name="incident">インシデント</param>
	/// <returns>ロック解除可能か</returns>
	private bool GetLockUnlockable()
	{
		if (this.Incident == null) return false;
		if (this.IsIncidentLocked == false) return false;

		// 管理者or本人の場合OK
		if (this.LoginOperatorCsInfo.EX_PermitUnlockFlg || (this.LoginOperatorId == this.Incident.LockOperatorId)) return true;

		// 送信依頼先であればOK
		if (this.Incident.LockStatus == Constants.FLG_CSINCIDENT_LOCK_STATUS_SEND_REQ)
		{
			var messageService = new CsMessageService(new CsMessageRepository());
			var messages = messageService.GetValidAll(this.Incident.DeptId, this.Incident.IncidentId, this.SortMessageRightKbn.ToString(), this.SortMessageRight);
			var messageRequestService = new CsMessageRequestService(new CsMessageRequestRepository());
			foreach (var m in messages)
			{
				var req = messageRequestService.GetAllWithItems(m.DeptId, m.IncidentId, m.MessageNo);
				if (req.Any(r => (r.RequestStatus == Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_SEND_REQ)
					&& r.EX_Items.Any(i => i.ApprOperatorId == this.LoginOperatorId))) return true;
			}
		}
		return false;
	}
	#endregion

	#region -GetIncidentMessageHistoryList インシデントメッセージ履歴一覧取得
	/// <summary>
	/// インシデントメッセージ履歴一覧取得
	/// </summary>
	/// <param name="incidentId">インシデントID</param>
	/// <returns>インシデントメッセージ履歴</returns>
	private CsMessageModel[] GetIncidentMessageHistoryList(string incidentId)
	{
		CsMessageModel[] messages;
		var messageService = new CsMessageService(new CsMessageRepository());
		if (this.TopPageKbn == TopPageKbn.TrashIncident)	// ゴミ箱：インシデントの場合は一覧に削除済み問合せも出したい
		{
			messages = messageService.GetAll(this.LoginOperatorDeptId, incidentId, this.SortMessageRightKbn.ToString(), this.SortMessageRight);
		}
		else
		{
			messages = messageService.GetValidAll(this.LoginOperatorDeptId, incidentId, this.SortMessageRightKbn.ToString(), this.SortMessageRight);
		}
		return messages;
	}
	#endregion

	#region -DispIncidentMessageHistoryList インシデントメッセージ履歴一覧表示
	/// <summary>
	/// インシデントメッセージ履歴一覧表示
	/// </summary>
	/// <param name="messages">メッセージリスト</param>
	/// <returns>問合せ履歴</returns>
	private CsMessageModel[] DispIncidentMessageHistoryList(CsMessageModel[] messages)
	{
		rIncidentMessages.DataSource = messages;
		rIncidentMessages.DataBind();
		return messages;
	}
	#endregion

	#region -DispMessage メッセージ情報表示
	/// <summary>
	/// メッセージ情報表示
	/// </summary>
	private void DispMessage()
	{
		if (this.Message != null)
		{
			hfMessageNo.Value = this.Message.MessageNo.ToString();
			hfMessageMode.Value = (this.Message.MediaKbn == Constants.FLG_CSMESSAGE_MEDIA_KBN_MAIL) ? Constants.KBN_MESSAGE_MEDIA_MODE_MAIL : Constants.KBN_MESSAGE_MEDIA_MODE_TEL;

			DispMessageMain();

			var requests = GetReuqestsWithItems();
			DispMessageProperty(requests);

			var requestLast = (requests.Length != 0) ? requests[0] : null;
			DispMessageActionButtons(requestLast);
		}
		else
		{
			hfMessageNo.Value = "";

			divMessageTel.Visible = false;
			divMessageMail.Visible = false;

			divMessageProperty.Visible = false;
		}
	}
	#endregion

	#region -DispMessageActionButtons メッセージアクションボタン表示
	/// <summary>
	/// アクションボタン表示
	/// </summary>
	/// <param name="request">リクエスト</param>
	private void DispMessageActionButtons(CsMessageRequestModel request)
	{
		if (this.Incident == null) return;
		if (this.Message == null) return;
		var incident = this.Incident;
		var message = this.Message;

		// 元に戻すボタン表示
		btnUntrashMessage.Visible = (message.ValidFlg == Constants.FLG_CSINCIDENT_VALID_FLG_INVALID);
		btnUntrashMessage.Enabled = (incident.ValidFlg == Constants.FLG_CSINCIDENT_VALID_FLG_VALID);
		lUntrashMessageErrorMessage.Visible = (message.ValidFlg == Constants.FLG_CSINCIDENT_VALID_FLG_INVALID) && (incident.ValidFlg != Constants.FLG_CSINCIDENT_VALID_FLG_VALID);

		// 各種リクエスト系ボタン初期化
		spApprovalCancelButtonArea.Visible
			= spSendCancelButtonArea.Visible
			= spApprovalOkSendArea.Visible
			= spApprovalOkNGButtonArea.Visible
			= spSendButtonArea.Visible = false;
		hfRequestNo.Value = hfBranchNo.Value = "0";
		if (request == null) return;

		hfRequestNo.Value = request.RequestNo.ToString();

		// 取り下げリンク表示
		spApprovalCancelButtonArea.Visible
			= ((message.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_REQ)
				&& (message.OperatorId == this.LoginOperatorId));
		spSendCancelButtonArea.Visible
			= ((message.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_REQ)
				&& (message.OperatorId == this.LoginOperatorId));

		var itemsRequestForMe = request.EX_Items.Where(i =>
			(i.ApprOperatorId == this.LoginOperatorId)
			&& (i.ResultStatus == Constants.FLG_CSMESSAGEREQUESTITEM_RESULT_STATUS_NONE)).ToArray();

		// 承認後送信リンク表示
		spApprovalOkSendArea.Visible = ((message.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_OK)
				&& (message.OperatorId == this.LoginOperatorId));

		// 承認／差戻ボタン表示
		spApprovalOkNGButtonArea.Visible = ((itemsRequestForMe.Length > 0)
			&& (message.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_REQ)
			&& this.LoginOperatorCsInfo.EX_PermitApprovalFlg);

		// 送信ボタン表示
		spSendButtonArea.Visible = (itemsRequestForMe.Length > 0)
			&& (message.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_REQ);

		if (itemsRequestForMe.Length != 0) hfBranchNo.Value = itemsRequestForMe[0].BranchNo.ToString();
	}
	#endregion

	#region -DispMessageMain メッセージ情報表示（メイン領域）
	/// <summary>
	/// メッセージ情報表示（メイン領域）
	/// </summary>
	/// <param name="message"></param>
	private void DispMessageMain()
	{
		if (this.Message == null) return;
		var message = this.Message;

		if (message.MediaKbn == Constants.FLG_CSMESSAGE_MEDIA_KBN_MAIL)
		{
			var mail = message.EX_Mail;

			lMailFrom.Text = WebSanitizer.HtmlEncode(mail.MailFrom);
			ucErrorPointFrom.SetMailAddr("");
			if (mail.MailKbn == Constants.FLG_CSMESSAGEMAIL_MAIL_KBN_RECEIVE) ucErrorPointFrom.SetMailAddr(mail.MailFrom);
			lMailTo.Text = WebSanitizer.HtmlEncode(mail.MailTo);
			ucErrorPointTo.SetMailAddr("");
			if (mail.MailKbn == Constants.FLG_CSMESSAGEMAIL_MAIL_KBN_SEND) ucErrorPointTo.SetMailAddr(mail.MailTo);
			lMailCc.Text = WebSanitizer.HtmlEncode(mail.MailCc);
			spMailCc.Visible = (string.IsNullOrEmpty(lMailCc.Text) == false);
			lMailBcc.Text = WebSanitizer.HtmlEncode(mail.MailBcc);
			spMailBcc.Visible = (string.IsNullOrEmpty(lMailBcc.Text) == false);
			if (mail.SendDatetime.HasValue)
			{
				lMailDate.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringForManager(
						mail.SendDatetime,
						DateTimeUtility.FormatType.ShortDateHourMinute2Letter));
			}
			else if (mail.ReceiveDatetime.HasValue)
			{
				lMailDate.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringForManager(
						mail.ReceiveDatetime,
						DateTimeUtility.FormatType.ShortDateHourMinute2Letter));
			}
			else
			{
				lMailDate.Text = "";
			}
			lMailSubject.Text = WebSanitizer.HtmlEncode(mail.MailSubject);
			lMailBody.Text = StringUtility.ToWordBreakString(WebSanitizer.HtmlEncodeChangeToBr(mail.MailBody), 80);
			rMailAttachmentFiles.DataSource = mail.EX_MailAttachments;
			rMailAttachmentFiles.DataBind();
			rMailAttachmentFiles.Visible = (rMailAttachmentFiles.Items.Count != 0);

			trMailMessageStatus.Visible = (this.IsDraftMessage || this.IsRequestMessage);
			lMailMessageStatus.Text = WebSanitizer.HtmlEncode(message.EX_MessageStatusName);
		}
		else
		{
			lMessageInquiryDateTime.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					message.EX_InquiryReplyChangedDate,
					DateTimeUtility.FormatType.ShortDateHourMinute2Letter));
			lMessageReplyOperatorName.Text = WebSanitizer.HtmlEncode(message.EX_ReplyOperatorName);
			lMessageUserName.Text = WebSanitizer.HtmlEncode(message.UserName1 + message.UserName2);
			lMessageUserNameKana.Text = WebSanitizer.HtmlEncode(message.UserNameKana1 + message.UserNameKana2);
			lMessageUserTel.Text = WebSanitizer.HtmlEncode(message.UserTel1);
			lMessageUserMail.Text = WebSanitizer.HtmlEncode(message.UserMailAddr);
			ucErrorPoint.SetMailAddr(message.UserMailAddr);
			lMessageInquiryTitle.Text = WebSanitizer.HtmlEncode(message.InquiryTitle);
			lMessageInquiryText.Text = WebSanitizer.HtmlEncodeChangeToBr(message.InquiryText);
			lMessageReplyText.Text = WebSanitizer.HtmlEncodeChangeToBr(message.ReplyText);

			trTelMessageStatus.Visible = this.IsDraftMessage;
			lTelMessageStatus.Text = WebSanitizer.HtmlEncode(message.EX_MessageStatusName);
		}
		divMessageTel.Visible = (message.MediaKbn != Constants.FLG_CSMESSAGE_MEDIA_KBN_MAIL);
		divMessageMail.Visible = (message.MediaKbn == Constants.FLG_CSMESSAGE_MEDIA_KBN_MAIL);
	}
	#endregion

	#region -DispMessageProperty メッセージプロパティ表示
	/// <summary>
	/// メッセージプロパティ表示
	/// </summary>
	/// <param name="requests">リクエスト配列</param>
	private void DispMessageProperty(CsMessageRequestModel[] requests)
	{
		if (this.Message == null) return;

		tbMessageIncidentId.Text = this.Message.IncidentId;
		foreach (var req in requests)
		{
			foreach (var item in req.EX_Items)
			{
				item.EX_ResultStatusString = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_CSMESSAGEREQUESTITEM, Constants.FIELD_CSMESSAGEREQUESTITEM_RESULT_STATUS, item.ResultStatus));
			}
		}
		rMessageRequestHistoryList.DataSource = requests;
		rMessageRequestHistoryList.DataBind();

		divMessagePropertyRequest.Visible = (requests.Length != 0);
	}
	#endregion

	#region -GetReuqestsWithItems メッセージのリクエストをアイテムとともに取得
	/// <summary>
	/// メッセージのリクエストをアイテムとともに取得
	/// </summary>
	/// <returns>メッセージリクエストリスト</returns>
	private CsMessageRequestModel[] GetReuqestsWithItems()
	{
		if (this.Message == null) return new CsMessageRequestModel[0];

		var service = new CsMessageRequestService(new CsMessageRequestRepository());
		var requests = service.GetAllWithItems(this.Message.DeptId, this.Message.IncidentId, this.Message.MessageNo);
		return requests;
	}
	#endregion

	#region #btnUnlock_Click ロック解除ボタンクリック
	/// <summary>
	/// ロック解除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUnlock_Click(object sender, EventArgs e)
	{
		// ロック解除
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		incidentService.UpdateLockStatusForUnlock(
			this.LoginOperatorDeptId,
			hfIncidentId.Value,
			this.LoginOperatorName);

		// 表示更新
		SetIncidentToProperty(hfIncidentId.Value);
		DispIncident();
	}
	#endregion

	#region #lbMessageMainTab_Click メッセージメインタブクリック
	/// <summary>
	/// メッセージメインタブクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbMessageMainTab_Click(object sender, EventArgs e)
	{
		ChangeLeftDispMode(LeftDispModeType.MessageMain);
	}
	#endregion

	#region #lbMessagePropertyTab_Click メッセージプロパティタブクリック
	/// <summary>
	/// メッセージプロパティタブクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbMessagePropertyTab_Click(object sender, EventArgs e)
	{
		ChangeLeftDispMode(LeftDispModeType.MessageProperty);
	}
	#endregion

	#region #lbIncidentMainTab_Click インシデントメインタブクリック
	/// <summary>
	/// インシデントメインタブクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbIncidentMainTab_Click(object sender, EventArgs e)
	{
		ChangeRightDispMode(RightDispModeType.IncidentMain);

		ScriptManager.RegisterStartupScript(up3, up3.GetType(), "refresh_incident_lock_icon", "refresh_incident_lock_icon('" +　hfIncidentId.Value + "', " + this.IsIncidentLocked.ToString().ToLower() + ");", true);
		ScriptManager.RegisterStartupScript(up3, up3.GetType(), "select_bottom_message_list", "select_bottom_message_list();", true);
	}
	#endregion

	#region #lbIncidentPropertyTab_Click インシデントプロパティタブクリック
	/// <summary>
	/// インシデントプロパティタブクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbIncidentPropertyTab_Click(object sender, EventArgs e)
	{
		ChangeRightDispMode(RightDispModeType.IncidentProperty);
	}
	#endregion

	#region -ChangeLeftDispMode 左表示モード変更
	/// <summary>
	/// 左表示モード変更
	/// </summary>
	/// <param name="dispMode">表示モード</param>
	private void ChangeLeftDispMode(LeftDispModeType dispMode)
	{
		this.LeftDispMode = dispMode;

		divMessageMain.Visible = (hfMessageNo.Value != "") && (this.LeftDispMode == LeftDispModeType.MessageMain);
		divMessageProperty.Visible = (hfMessageNo.Value != "") && (this.LeftDispMode == LeftDispModeType.MessageProperty);
		divMessageNone.Visible = (hfMessageNo.Value == "");
	}
	#endregion

	#region -ChangeRightDispMode 右表示モード変更
	/// <summary>
	/// 右表示モード変更
	/// </summary>
	/// <param name="dispMode">表示モード</param>
	private void ChangeRightDispMode(RightDispModeType dispMode)
	{
		this.RightDispMode = dispMode;

		divIncidentMain.Visible = (this.RightDispMode == RightDispModeType.IncidentMain);
		divIncidentProperty.Visible = (this.RightDispMode == RightDispModeType.IncidentProperty);
	}
	#endregion

	#region #ddlCsGroup_SelectedIndexChanged オペレータグループ変更イベント
	/// <summary>
	/// オペレータグループ変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlCsGroup_SelectedIndexChanged(object sender, EventArgs e)
	{
		RecreateCsOperatorDropDown(ddlCsOperators, ddlCsGroups.SelectedValue);
	}
	#endregion

	#region #lbSelectMessage_Click メッセージリンククリック
	/// <summary>
	/// メッセージリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSelectMessage_Click(object sender, EventArgs e)
	{
		SetMessageToProperty(hfIncidentId.Value, int.Parse(hfMessageNo.Value));
		DispMessage();
	}
	#endregion

	#region -CreateMailActionUrl メールアクションURL作成
	/// <summary>
	/// メールアクションURL作成
	/// </summary>
	/// <param name="mailActionType">メールアクション種別</param>
	/// <returns>メールアクションURL</returns>
	protected string CreateMailActionUrl(MailActionType mailActionType)
	{
		var url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2CS_MANAGER_TOP_MAILACTION);
		url.Append("?").Append(Constants.REQUEST_KEY_INCIDENT_ID).Append("=").Append(HttpUtility.UrlEncode(hfIncidentId.Value));
		url.Append("&").Append(Constants.REQUEST_KEY_MESSAGE_NO).Append("=").Append(HttpUtility.UrlEncode(hfMessageNo.Value));
		if (hfRequestNo.Value != "0")
		{
			url.Append("&").Append(Constants.REQUEST_KEY_REQUEST_NO).Append("=").Append(HttpUtility.UrlEncode(hfRequestNo.Value));
			url.Append("&").Append(Constants.REQUEST_KEY_BRANCH_NO).Append("=").Append(HttpUtility.UrlEncode(hfBranchNo.Value));
		}
		url.Append("&").Append(Constants.REQUEST_KEY_MAILACTION).Append("=").Append(HttpUtility.UrlEncode(BasePageCs.GetMailActionKbnFromType(mailActionType)));

		return url.ToString();
	}
	#endregion

	#region #btnUpdateIncident_Click インシデント更新ボタンクリック
	/// <summary>
	/// インシデント更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateIncident_Click(object sender, EventArgs e)
	{
		var incident = CheckInput();
		if (incident == null) return;

		RegisterUpdateIncidentAndSummaryValues(incident);

		// 担当変更メール送信
		if (incident.EX_OperatorIdBefore != incident.OperatorId)
		{
			var mailSender = new NotificationMailSender(this.LoginOperatorShopId);
			mailSender.SendNotificationMailForIncidentOperatorChanged(
				this.LoginOperatorDeptId,
				incident.IncidentId,
				ddlCsOperators.SelectedValue);

			hfCsOperatorBefore.Value = ddlCsOperators.SelectedValue;
		}
		lCompleteMessages.Text = WebMessages.GetMessages(WebMessages.ERROR_MANAGER_INCIDENT_HAS_BEEN_UPDATED);
		lUpdateInternalNoteMessages.Text = WebMessages.GetMessages(WebMessages.ERROR_MANAGER_UPDATED_INTERNAL_NOTES);

		// 最新情報表示
		DispIncident();

		ScriptManager.RegisterStartupScript(up3, up3.GetType(), "refreshlist", "refresh();", true);
	}
	#endregion

	#region -RegisterUpdateIncidentAndSummaryValues インシデント情報・集計区分値登録更新
	/// <summary>
	/// インシデント情報・集計区分値登録更新
	/// </summary>
	/// <param name="incident">インシデント</param>
	public void RegisterUpdateIncidentAndSummaryValues(CsIncidentModel incident)
	{
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		string incidentId = incidentService.RegisterUpdateWithSummaryValues(incident);

		SetIncidentToProperty(incidentId);
	}
	#endregion

	#region -CheckInput インシデントの入力チェック
	/// <summary>
	/// インシデントの入力チェック
	/// </summary>
	/// <param name="incident"></param>
	/// <returns>インシデントモデル</returns>
	/// <remarks>InquiryRightIncident.ascxに同様のメソッドあり</remarks>
	private CsIncidentModel CheckInput()
	{
		// 入力情報取得
		var input = GetInput();

		// インシデントチェック
		string errorMessage = "";
		var incident = CheckAndCreateIncident(input, rIncidentSummary, false, out errorMessage);
		lErrorMessages.Text = errorMessage;
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			return null;
		}

		// イその他インシデント情報セット
		incident.EX_IncidentCategoryName = ddlIncidentCategory.SelectedItem.Text.Trim();
		incident.EX_VocText = ddlVoc.SelectedItem.Text;
		incident.EX_CsGroupName = ddlCsGroups.SelectedItem.Text;
		incident.EX_CsOperatorName = ddlCsOperators.SelectedItem.Text;

		return incident;
	}
	#endregion

	#region -GetInput インシデント入力情報取得（入力チェック用）
	/// <summary>
	/// インシデント入力情報取得（入力チェック用）
	/// </summary>
	/// <returns>インシデント入力情報</returns>
	/// <remarks>InquiryRightIncident.ascxに同様のメソッドあり</remarks>
	private Hashtable GetInput()
	{
		Hashtable input = new Hashtable();
		input.Add(Constants.FIELD_CSINCIDENT_DEPT_ID, this.LoginOperatorDeptId);
		input.Add(Constants.FIELD_CSINCIDENT_INCIDENT_ID, hfIncidentId.Value);
		input.Add(Constants.FIELD_CSINCIDENT_USER_ID, tbUserId.Text);
		input.Add(Constants.FIELD_CSINCIDENT_INCIDENT_CATEGORY_ID, ddlIncidentCategory.SelectedValue);
		input.Add(Constants.FIELD_CSINCIDENT_INCIDENT_TITLE, tbIncidentTitle.Text);
		input.Add(Constants.FIELD_CSINCIDENT_STATUS, ddlIncidentStatus.SelectedValue);
		input.Add(Constants.FIELD_CSINCIDENT_VOC_ID, ddlVoc.SelectedValue);
		input.Add(Constants.FIELD_CSINCIDENT_VOC_MEMO, tbVocMemo.Text);
		input.Add(Constants.FIELD_CSINCIDENT_COMMENT, tbIncidentComment.Text);
		input.Add(Constants.FIELD_CSINCIDENT_IMPORTANCE, ddlImportance.SelectedValue);
		input.Add(Constants.FIELD_CSINCIDENT_USER_NAME, null);		// あとでセット
		input.Add(Constants.FIELD_CSINCIDENT_USER_CONTACT, null);	// あとでセット
		input.Add(Constants.FIELD_CSINCIDENT_CS_GROUP_ID, ddlCsGroups.SelectedValue);
		input.Add(Constants.FIELD_CSINCIDENT_OPERATOR_ID, ddlCsOperators.SelectedValue);
		input.Add(Constants.FIELD_CSINCIDENT_OPERATOR_ID + "_before", hfCsOperatorBefore.Value);
		input.Add(Constants.FIELD_CSINCIDENT_LAST_CHANGED, this.LoginOperatorName);

		return input;
	}
	#endregion

	#region #lbTrashMessage_Click メッセージごみ箱投入ボタンクリック
	/// <summary>
	/// メッセージごみ箱投入ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbTrashMessage_Click(object sender, EventArgs e)
	{
		// 有効フラグ更新
		UpdateMessageValidFlg(this.LoginOperatorDeptId, hfIncidentId.Value, int.Parse(hfMessageNo.Value), Constants.FLG_CSMESSAGE_VALID_FLG_INVALID);

		// リロード
		Response.Redirect(Request.Url.PathAndQuery);
	}
	#endregion

	#region #btnUntrashMessage_Click　メッセージをごみ箱から元に戻すボタンクリック
	/// <summary>
	/// メッセージをごみ箱から元に戻すボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUntrashMessage_Click(object sender, EventArgs e)
	{
		// インシデント削除チェック
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		var incident = incidentService.GetWithSummaryValues(this.LoginOperatorDeptId, hfIncidentId.Value);
		if (incident.ValidFlg == Constants.FLG_CSINCIDENT_VALID_FLG_INVALID)
		{
			lUntrashMessageErrorMessage.Visible = true;
			return;
		}

		// 有効フラグ更新
		UpdateMessageValidFlg(this.LoginOperatorDeptId, hfIncidentId.Value, int.Parse(hfMessageNo.Value), Constants.FLG_CSMESSAGE_VALID_FLG_VALID);

		// リロード
		Response.Redirect(Request.Url.PathAndQuery);
	}
	#endregion

	#region #btnUntrashIncident_Click インシデントをごみ箱から元に戻すボタンクリック
	/// <summary>
	/// インシデントをごみ箱から元に戻すボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUntrashIncident_Click(object sender, EventArgs e)
	{
		// 更新
		UpdateIncidentValidFlg(this.LoginOperatorDeptId, hfIncidentId.Value, Constants.FLG_CSINCIDENT_VALID_FLG_VALID);

		// リロード
		Response.Redirect(Request.Url.PathAndQuery);
	}
	#endregion

	#region #lbDeleteMessage_Click メッセージ削除ボタンクリック
	/// <summary>
	/// メッセージ削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDeleteMessage_Click(object sender, EventArgs e)
	{
		var service = new CsMessageService(new CsMessageRepository());
		service.DeleteWithRequestAndMails(this.LoginOperatorDeptId, hfIncidentId.Value, int.Parse(hfMessageNo.Value));

		// リロード
		ScriptManager.RegisterStartupScript(up3, up3.GetType(), "refreshlist", "refresh();", true);
		up3.Update();
	}
	#endregion

	#region #btnTrashIncident_Click インシデントごみ箱投入ボタンクリック
	/// <summary>
	/// インシデントごみ箱投入ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnTrashIncident_Click(object sender, EventArgs e)
	{
		// インシデント取得
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		var incident = incidentService.GetWithSummaryValues(this.LoginOperatorDeptId, hfIncidentId.Value);

		// 有効フラグ変更
		incident.ValidFlg = Constants.FLG_CSINCIDENT_VALID_FLG_INVALID;
		incident.LastChanged = this.LoginOperatorName;
		incidentService.UpdateValidFlgWithMessage(incident);

		// リロード
		Response.Redirect(Request.Url.PathAndQuery);
	}
	#endregion

	#region #btnDeleteIncident_Click インシデント削除ボタンクリック
	/// <summary>
	/// インシデント削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDeleteIncident_Click(object sender, EventArgs e)
	{
		// インシデント削除
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		incidentService.Delete(this.LoginOperatorDeptId, hfIncidentId.Value);

		// メッセージ削除
		var messageService = new CsMessageService(new CsMessageRepository());
		messageService.DeleteAllWithMails(this.LoginOperatorDeptId, hfIncidentId.Value);

		// リロード
		ScriptManager.RegisterStartupScript(up3, up3.GetType(), "refreshlist", "refresh();", true);
	}
	#endregion

	#region #UpdateIncidentValidFlg インシデント有効フラグ更新
	/// <summary>
	/// インシデント有効フラグ更新
	/// </summary>
	/// <param name="deptId">識別ID</param>
	/// <param name="incidentId">インシデントID</param>
	/// <param name="validFlg">有効フラグ</param>
	private void UpdateIncidentValidFlg(string deptId, string incidentId, string validFlg)
	{
		var incident = new CsIncidentModel();
		incident.DeptId = deptId;
		incident.IncidentId = incidentId;
		incident.ValidFlg = validFlg;
		incident.LastChanged = this.LoginOperatorName;

		var service = new CsIncidentService(new CsIncidentRepository());
		service.UpdateValidFlg(incident);
	}
	#endregion

	#region -UpdateMessageValidFlg メッセージ有効フラグ更新
	/// <summary>
	/// メッセージ有効フラグ更新
	/// </summary>
	/// <param name="deptId">識別ID</param>
	/// <param name="incidentId">インシデントID</param>
	/// <param name="messageNo">メッセージNO</param>
	/// <param name="validFlg">有効フラグ</param>
	private void UpdateMessageValidFlg(string deptId, string incidentId, int messageNo, string validFlg)
	{
		// 取得
		var service = new CsMessageService(new CsMessageRepository());
		var message = service.Get(deptId, incidentId, messageNo);

		// 更新
		message.ValidFlg = validFlg;
		message.LastChanged = this.LoginOperatorName;
		service.Update(message);
	}
	#endregion

	#region #lbRefreshIncident_Click インシデント情報更新（メッセージ一覧は更新しない）
	/// <summary>
	/// インシデント情報更新（メッセージ一覧は更新しない）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRefreshIncident_Click(object sender, EventArgs e)
	{
		SetIncidentToProperty(hfIncidentId.Value);
		DispIncident();

		ScriptManager.RegisterStartupScript(up3, up3.GetType(), "select_bottom_message_list", "select_bottom_message_list();", true);
	}
	#endregion

	#region #btnCloseIncident_Click インシデントクローズボタンクリック
	/// <summary>
	/// インシデントクローズボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCloseIncident_Click(object sender, EventArgs e)
	{
		// インシデントクローズ
		CloseIncident(this.LoginOperatorDeptId, hfIncidentId.Value);

		// 表示更新
		SetIncidentToProperty(hfIncidentId.Value);
		DispIncident();

		lCompleteMessages.Text = WebMessages.GetMessages(WebMessages.ERROR_MANAGER_INCIDENT_HAS_BEEN_CLOSED);
		ScriptManager.RegisterStartupScript(up3, up3.GetType(), "refreshlist", "refresh();", true);
	}
	#endregion

	#region -CloseIncident インシデントクローズ
	/// <summary>
	/// インシデントクローズ
	/// </summary>
	/// <param name="deptId">識別ID</param>
	/// <param name="incidentId">インシデントID</param>
	private void CloseIncident(string deptId, string incidentId)
	{
		var incident = new CsIncidentModel();
		incident.DeptId = deptId;
		incident.IncidentId = incidentId;
		incident.Status = Constants.FLG_CSINCIDENT_STATUS_COMPLETE;
		incident.LastChanged = this.LoginOperatorName;

		var service = new CsIncidentService(new CsIncidentRepository());
		service.UpdateStatusAndCompleteDate(incident);
	}
	#endregion

	#region #btnUpdateMessageIncidentId_Click メッセージインシデントID更新ボタンクリック
	/// <summary>
	/// メッセージインシデントID更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateMessageIncidentId_Click(object sender, EventArgs e)
	{
		// エラーチェック
		string errorMessage = CheckForUpdateMessageIncidentId(hfIncidentId.Value, tbMessageIncidentId.Text);
		if (errorMessage != "")
		{
			lUpdateMessageIncidentIdMessage.CssClass = "notice";
			lUpdateMessageIncidentIdMessage.Text = errorMessage;
			return;
		}

		string afterIncidentId;
		int afterMessageNo;
		// インシデントID更新
		if (tbMessageIncidentId.Text != "")
		{
			afterMessageNo = UpdateMessageIncidentId(hfIncidentId.Value, int.Parse(hfMessageNo.Value), tbMessageIncidentId.Text);
			lUpdateMessageIncidentIdMessage.Text = WebMessages.GetMessages(WebMessages.ERROR_MANAGER_INCIDENT_ID_HAS_BEEN_UPDATED);
			afterIncidentId = tbMessageIncidentId.Text;
		}
		// インシデントを新規に作成して紐付け
		else
		{
			afterIncidentId = CreateEmptyIncident(hfIncidentId.Value);
			afterMessageNo = UpdateMessageIncidentId(hfIncidentId.Value, int.Parse(hfMessageNo.Value), afterIncidentId);
			lUpdateMessageIncidentIdMessage.Text = WebMessages.GetMessages(WebMessages.ERROR_MANAGER_CREATED_NEW_INCIDENT);
		}

		// 表示更新・インシデントプロパティを開く
		DispIncidentAndLastMessage(afterIncidentId);
		hfMessageNo.Value = afterMessageNo.ToString();

		ScriptManager.RegisterStartupScript(up3, up3.GetType(), "refreshlist", "refresh('" + afterIncidentId + "', '" + afterMessageNo + "');", true);
	}
	#endregion

	#region -CheckForUpdateMessageIncidentId メッセージインシデントID更新用チェック
	/// <summary>
	/// メッセージインシデントID更新用チェック
	/// </summary>
	/// <param name="beforeIncidentId">変更前インシデントID</param>
	/// <param name="afterIncidentId">変更後インシデントID</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckForUpdateMessageIncidentId(string beforeIncidentId, string afterIncidentId)
	{
		// 空であれば新規作成するのでreturn
		if (string.IsNullOrEmpty(beforeIncidentId) || string.IsNullOrEmpty(afterIncidentId)) return string.Empty;

		// 同一IDチェック
		if (afterIncidentId == beforeIncidentId) return WebMessages.GetMessages(WebMessages.ERROR_MANAGER_SAME_ID_AS_BEFORE_THE_UPDATE);

		// ID存在チェック
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		var incidnet = incidentService.GetWithSummaryValues(this.LoginOperatorDeptId, afterIncidentId);
		if (incidnet == null) return WebMessages.GetMessages(WebMessages.ERROR_MANAGER_CORRESPONDING_ID_DID_NOT_EXIST);

		// インシデントロックチェック
		if (incidnet.LockStatus != Constants.FLG_CSINCIDENT_LOCK_STATUS_NONE) return WebMessages.GetMessages(WebMessages.ERROR_MANAGER_INCIDENT_IS_LOCKED);

		return "";
	}
	#endregion

	#region -CreateEmptyIncident 空のインシデント作成
	/// <summary>
	/// 空のインシデント作成
	/// </summary>
	/// <param name="beforeIncidentId">元のインシデントID</param>
	/// <returns>作成したインシデントID</returns>
	private string CreateEmptyIncident(string beforeIncidentId)
	{
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		var before = incidentService.Get(this.LoginOperatorDeptId, beforeIncidentId);
		var afterIncidentId = incidentService.RegisterEmpty(this.LoginOperatorDeptId, this.LoginOperatorName, before);
		return afterIncidentId;
	}
	#endregion

	#region -UpdateMessageIncidentId  メッセージのインシデントID更新
	/// <summary>
	/// メッセージのインシデントID更新
	/// </summary>
	/// <param name="beforeIncidentId">変更前インシデントID</param>
	/// <param name="beforeMessageNo">変更前メッセージNO</param>
	/// <param name="afterIncidentId">変更後インシデントID</param>
	/// <returns>変更後問合せNO</returns>
	private int UpdateMessageIncidentId(string beforeIncidentId, int beforeMessageNo, string afterIncidentId)
	{
		var messageService = new CsMessageService(new CsMessageRepository());
		int afterMessageNo = messageService.UpdateIncidentIdWithRequests(this.LoginOperatorDeptId, beforeIncidentId, beforeMessageNo, afterIncidentId, this.LoginOperatorName);
		return afterMessageNo;
	}
	#endregion

	#region #btnSetOperatorAndGroup_Click CSオペレータ・グループセット
	/// <summary>
	/// CSオペレータ・グループセット
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSetOperatorAndGroup_Click(object sender, EventArgs e)
	{
		SetOperatorAndGroup(this.LoginOperatorId, ddlCsGroups, ddlCsOperators);
	}
	#endregion

	#region #CreateMessageInputUrl メッセージ入力画面URL作成
	/// <summary>
	/// メッセージ入力画面URL作成
	/// </summary>
	/// <param name="mediaModeKbn">媒体モード区分</param>
	/// <param name="editModeKbn">編集モード区分</param>
	/// <returns>問合せ入力画面URL</returns>
	protected string CreateMessageInputUrl(string mediaModeKbn, string editModeKbn)
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MESSAGE_MESSAGE_INPUT
			+ "?" + Constants.REQUEST_KEY_MESSAGE_MEDIA_MODE + "=" + HttpUtility.UrlEncode(mediaModeKbn)
			+ "&" + Constants.REQUEST_KEY_MESSAGE_EDIT_MODE + "=" + HttpUtility.UrlEncode(editModeKbn)
			+ "&" + Constants.REQUEST_KEY_INCIDENT_ID + "=" + HttpUtility.UrlEncode(hfIncidentId.Value)
			+ "&" + Constants.REQUEST_KEY_MESSAGE_NO + "=" + hfMessageNo.Value;
	}
	#endregion

	#region #CreatePrintUrl 印刷画面URL作成
	/// <summary>
	/// 印刷画面URL作成
	/// </summary>
	/// <returns>印刷画面URL</returns>
	protected string CreatePrintUrl()
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_TOP_PRINT_MESSAGE
			+ "?" + Constants.REQUEST_KEY_INCIDENT_ID + "=" + HttpUtility.UrlEncode(hfIncidentId.Value)
			+ "&" + Constants.REQUEST_KEY_MESSAGE_NO + "=" + hfMessageNo.Value;
	}
	#endregion

	#region #Sort message & message mail
	/// <summary>
	/// Sort message & message mail list
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event</param>
	protected void MessageRightListSort_Click(object sender, EventArgs e)
	{
		var commandArgument = ((LinkButton)sender).CommandArgument;
		var sortMessageRightKbnCurrent = (SortMessageRightKbn)Enum.Parse(typeof(SortMessageRightKbn), commandArgument);
		this.SortMessageRight = BasePageCs.GetSortFromSortKbn(this.SortMessageRightKbn.ToString(), sortMessageRightKbnCurrent.ToString(), this.SortMessageRight);
		this.SortMessageRightKbn = sortMessageRightKbnCurrent;

		// Display After Sort
		DisplayAll(hfIncidentId.Value);
	}

	/// <summary>
	/// Display After Sort Message List
	/// </summary>
	protected void DisplayAfterSortMessageRightList()
	{
		BasePageCs.ChangeRefineTaskLinkSort(lDateSendOrReceiveIconSort, (this.SortMessageRightKbn == SortMessageRightKbn.DateSendOrReceive), this.SortMessageRight);
		BasePageCs.ChangeRefineTaskLinkSort(lReplyChangedDateIconSort, (this.SortMessageRightKbn == SortMessageRightKbn.ReplyChangedDate), this.SortMessageRight);
		BasePageCs.ChangeRefineTaskLinkSort(lInquiryTitleIconSort, (this.SortMessageRightKbn == SortMessageRightKbn.InquiryTitle), this.SortMessageRight);
		BasePageCs.ChangeRefineTaskLinkSort(lOperatorNameIconSort, (this.SortMessageRightKbn == SortMessageRightKbn.OperatorName), this.SortMessageRight);
	}
	#endregion

	#region #GetDateStatusChange
	/// <summary>
	/// Get Date Status Change
	/// </summary>
	/// <returns>Date Status Change</returns>
	protected string GetDateStatusChange(DateTime? dateStatusChange)
	{
		var dateStatus = (this.Message.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_CANCEL)
			? this.Message.DateChanged
			: dateStatusChange;

		var result = DateTimeUtility.ToStringForManager(
			dateStatus,
			DateTimeUtility.FormatType.ShortDateHourMinute2Letter);

		return result;
	}
	#endregion

	#region プロパティ
	/// <summary>ページ区分</summary>
	public TopPageKbn TopPageKbn
	{
		get { return (TopPageKbn)ViewState["TopPageKbn"]; }
		set { ViewState["TopPageKbn"] = value; }
	}
	/// <summary>コンテンツ表示するか</summary>
	public bool DisplayContents
	{
		get { return divContents.Visible; }
		set { divContents.Visible = value; }
	}
	/// <summary>左表示モード</summary>
	protected LeftDispModeType LeftDispMode
	{
		get { return (LeftDispModeType)ViewState["DispLeftMode"]; }
		set { ViewState["DispLeftMode"] = value; }
	}
	/// <summary>右表示モード</summary>
	protected RightDispModeType RightDispMode
	{
		get { return (RightDispModeType)ViewState["DispRightMode"]; }
		set { ViewState["DispRightMode"] = value; }
	}
	/// <summary>インシデント情報</summary>
	protected CsIncidentModel Incident
	{
		get { return (CsIncidentModel)ViewState["Incident"]; }
		set { ViewState["Incident"] = value; }
	}
	/// <summary>メッセージ情報</summary>
	protected CsMessageModel Message
	{
		get { return (CsMessageModel)ViewState["Message"]; }
		set { ViewState["Message"] = value; }
	}
	/// <summary>インシデントロック状態か</summary>
	protected bool IsIncidentLocked
	{
		get { return (bool)(ViewState["IncidentLocked"] ?? false); }
		set { ViewState["IncidentLocked"] = value; }
	}
	/// <summary>読み取り専用でない＆インシデントロックされていない</summary>
	protected bool IsNotReadonlyOperatorAndIncidentUnlocked
	{
		get { return this.LoginOperatorCsInfo.EX_PermitEditFlg && (this.IsIncidentLocked == false); }
	}
	/// <summary>削除不可能な他オペレータのメッセージ（他オペレータのメッセージは勝手に削除できない）</summary>
	protected bool IsUndeletedOtherOperatorMessage
	{
		get
		{
			return ((this.IsRequestMessage || IsDraftMessage)
				&& (this.Message.OperatorId != this.LoginOperatorId));
		}
	}
	/// <summary>メッセージが下書きか</summary>
	protected bool IsDraftMessage
	{
		get { return ((this.Message != null) && this.Message.EX_IsDraft); }
	}
	/// <summary>メッセージがリクエストか</summary>
	protected bool IsRequestMessage
	{
		get { return ((this.Message != null) && this.Message.EX_IsRequest); }
	}
	/// <summary>ゴミ箱に入れられるメッセージか（ロック状態は見ない）</summary>
	protected bool IsTrashableMessage
	{
		get
		{
			return ((this.Message != null) && (this.Message.EX_IsValid));
		}
	}
	/// <summary>削除可能メッセージか（ロック状態は見ない）</summary>
	protected bool IsDeletableMessage
	{
		get
		{
			return ((this.Message != null)
				&& (this.Message.ValidFlg == Constants.FLG_CSMESSAGE_VALID_FLG_INVALID)
				&& (this.TopPageKbn != TopPageKbn.TrashIncident)
				&& (this.TopPageKbn != TopPageKbn.Top)
				&& (this.IsRequestMessage
					|| this.IsDraftMessage 
					|| this.LoginOperatorCsInfo.EX_PermitPermanentDeleteFlg));
		}
	}
	/// <summary>編集可能な未完了メッセージか（ロック状態は見ない）</summary>
	protected bool IsEditableUncompleteMessage
	{
		get
		{
			return ((this.Message != null)
				&& this.Message.EX_IsValid
				&& (this.Message.EX_IsDraft
					|| (this.Message.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_NG)
					|| (this.Message.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_OK)
					|| (this.Message.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_CANCEL)
					|| (this.Message.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_NG)
					|| (this.Message.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_CANCEL))
				&& (this.Message.OperatorId == this.LoginOperatorId));
		}
	}
	/// <summary>編集可能な完了メッセージか（ロック状態は見ない）</summary>
	protected bool IsEditableDoneMessage
	{
		get
		{
			return ((this.Message != null)
				&& this.Message.EX_IsValid
				&& ((this.Message.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_DONE)
					&& ((this.Message.MediaKbn == Constants.FLG_CSMESSAGE_MEDIA_KBN_TEL) || (this.Message.MediaKbn == Constants.FLG_CSMESSAGE_MEDIA_KBN_OTHERS)))
				&& (this.Message.OperatorId == this.LoginOperatorId));
		}
	}
	/// <summary>編集可能な送信依頼メッセージか（ロック状態は見ない）</summary>
	protected bool IsEditableSendRequestedMessage
	{
		get
		{
			return ((this.Message != null)
					&& this.Message.EX_IsValid
					&& (this.Message.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_REQ));
		}
	}
	/// <summary>返信可能なメッセージか（ロック状態は見ない）</summary>
	protected bool IsReplyableMessage
	{
		get
		{
			return ((this.Message != null)
					&& this.Message.EX_IsValid
					&& ((this.Message.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_RECEIVED)
						|| (this.Message.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_DONE)));
		}
	}
	/// <summary>Sort Message Right</summary>
	protected string SortMessageRight
	{
		get { return (string)Session["SortMessageRight"] ?? Constants.FLG_SORT_KBN_DESC; }
		set { Session["SortMessageRight"] = value; }
	}
	/// <summary>Sort Message Right Kbn</summary>
	protected SortMessageRightKbn SortMessageRightKbn
	{
		get { return (SortMessageRightKbn?)Session["SortMessageRightKbn"] ?? SortMessageRightKbn.DateSendOrReceive; }
		set { Session["SortMessageRightKbn"] = value; }
	}
	#endregion
}
