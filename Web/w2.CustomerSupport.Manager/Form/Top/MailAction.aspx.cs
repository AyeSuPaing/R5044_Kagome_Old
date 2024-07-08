/*
=========================================================================================================
  Module      : メールアクションページ処理(MailAction.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Cs.IncidentVoc;
using w2.App.Common.Cs.Incident;
using w2.App.Common.Cs.Message;
using w2.App.Common.Cs.SummarySetting;

public partial class Form_Top_MailAction : BasePageCs
{
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
			Initialize();

			// メール、リクエスト取得
			this.Message = GetMessageWithMail();
			this.MessageMail = this.Message.EX_Mail;
			this.MessageRequest = GetMessageRequest();
			this.RequestOperator = GetRequestOperator(this.MessageRequest);

			// 承認作業であれば作業中オペレータ更新
			if ((this.MessageRequest.EX_WorkingOperatorName == "")
				&& (this.MailActionType == MailActionType.ApprovalOK) || (this.MailActionType == MailActionType.ApprovalNG))
			{
				this.MessageRequest.WorkingOperatorId = this.LoginOperatorId;
				this.MessageRequest.EX_WorkingOperatorName = this.LoginOperatorName;
				this.MessageRequest.LastChanged = this.LoginOperatorName;

				var requestService = new CsMessageRequestService(new CsMessageRequestRepository());
				requestService.UpdateWorkingOperator(this.MessageRequest);
			}
			// 承認作業中オペレータがいればアラート表示
			if ((this.MessageRequest.WorkingOperatorId != "") && (this.MessageRequest.WorkingOperatorId != this.LoginOperatorId))
			{
				lWorkingMesaage.Text = WebSanitizer.HtmlEncode(
					WebMessages.GetMessages(WebMessages.MSG_MANAGER_MAIL_ACTION_ALERT_DISPLAY_OPERATOR_DURING_APPROVAL_WORK)
						.Replace("@@ 1 @@", this.MessageRequest.EX_WorkingOperatorName));
			}

			divApprComment.Visible = (this.MailActionType != MailActionType.ApprovalOKSend);

			DisplayIncident();
			DisplayMailAndRequestComment();

			DispButtonAreas();
		}
	}
	#endregion

	#region -Initialize 初期化処理
	/// <summary>
	/// 初期化処理
	/// </summary>
	private void Initialize()
	{
		// 集計区分セット
		rIncidentSummary.DataSource = CsSummarySettingArray();
		rIncidentSummary.DataBind();
	}
	#endregion

	#region -CsSummarySettingArray 集計区分 リピータ用データソース作成
	/// <summary>
	/// 集計区分 リピータ用データソース作成
	/// </summary>
	/// <returns>配列</returns>
	private CsSummarySettingModel[] CsSummarySettingArray()
	{
		var service = new CsSummarySettingService(new CsSummarySettingRepository());
		var list = service.GetValidAllWithValidItems(this.LoginOperatorDeptId);

		return list;
	}
	#endregion

	#region -GetMessageRequest メッセージ依頼情報取得（見つからない場合エラー）
	/// <summary>
	/// メッセージ依頼情報取得（見つからない場合エラー）
	/// </summary>
	/// <returns>メッセージ依頼情報</returns>
	private CsMessageRequestModel GetMessageRequest()
	{
		var service = new CsMessageRequestService(new CsMessageRequestRepository());
		var request = service.GetWithAllItems(this.LoginOperatorDeptId, this.IncidentId, this.MessageNo, this.RequestNo);
		if (request == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERROR_MANAGER_MESSAGE_REQUEST_CANNOT_BE_FOUND);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		return request;
	}
	#endregion

	#region -GetRequestOperator メッセージ依頼オペレータ取得（見つからない場合エラー）
	/// <summary>
	/// メッセージ依頼オペレータ取得（見つからない場合エラー）
	/// </summary>
	/// <param name="request">メッセージリクエスト</param>
	/// <returns>メッセージ依頼オペレータ</returns>
	private CsOperatorModel GetRequestOperator(CsMessageRequestModel request)
	{
		var operatorService = new CsOperatorService(new CsOperatorRepository());
		var ope = operatorService.Get(this.LoginOperatorDeptId, request.RequestOperatorId);
		if (ope == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERROR_MANAGER_OPERATOR_NOT_FOUND)
				.Replace("@@ 1 @@", request.RequestOperatorId);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		return ope;
	}
	#endregion

	#region -GetMessageWithMail メッセージ＆メール取得（見つからない場合エラー）
	/// <summary>
	/// メッセージ＆メール取得（見つからない場合エラー）
	/// </summary>
	/// <returns>メッセージ依頼情報</returns>
	private CsMessageModel GetMessageWithMail()
	{
		var service = new CsMessageService(new CsMessageRepository());
		var message = service.GetWithMail(this.LoginOperatorDeptId, this.IncidentId, this.MessageNo);
		if ((message == null) || (message.EX_Mail == null))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERROR_MANAGER_MESSAGE_MAIL_NOT_FOUND);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		return message;
	}
	#endregion

	#region -DispButtonAreas ボタンエリア表示
	/// <summary>
	/// ボタンエリア表示
	/// </summary>
	private void DispButtonAreas()
	{
		var request = this.MessageRequest;

		bool isMyRequest = (request.RequestOperatorId == this.LoginOperatorId);
		bool isRequestForMe = request.EX_Items.Any(i => (i.RequestNo == this.RequestNo) && (i.ApprOperatorId == this.LoginOperatorId));

		switch (this.MailActionType)
		{
			case MailActionType.ApprovalCancel:
				divApprovalCancelArea.Visible = isMyRequest && (request.RequestStatus == Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_APPROVE_REQ);
				break;

			case MailActionType.ApprovalOK:
				divApprovalOKArea.Visible = isRequestForMe && (request.RequestStatus == Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_APPROVE_REQ);
				break;

			case MailActionType.ApprovalNG:
				divApprovalNGArea.Visible = isRequestForMe && (request.RequestStatus == Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_APPROVE_REQ);
				break;

			case MailActionType.ApprovalOKSend:
				divApprovalOKSendArea.Visible = isMyRequest && (request.RequestStatus == Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_APPROVE_OK);
				break;

			case MailActionType.SendCancel:
				divSendCancelArea.Visible = isMyRequest && (request.RequestStatus == Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_SEND_REQ);
				break;

			case MailActionType.SendNG:
				divSendNGArea.Visible = isRequestForMe && (request.RequestStatus == Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_SEND_REQ);
				break;
		}
	}
	#endregion

	#region -DisplayMailAndRequestComment メール・リクエストコメント表示
	/// <summary>
	/// メール・リクエストコメント表示
	/// </summary>
	private void DisplayMailAndRequestComment()
	{
		var mail = this.MessageMail;
		var request = this.MessageRequest;

		lMailFrom.Text = WebSanitizer.HtmlEncode(mail.MailFrom);
		lMailTo.Text = WebSanitizer.HtmlEncode(mail.MailTo);
		ucErrorPointTo.SetMailAddr("");
		if (mail.MailKbn == Constants.FLG_CSMESSAGEMAIL_MAIL_KBN_SEND) ucErrorPointTo.SetMailAddr(mail.MailTo);
		lMailCc.Text = WebSanitizer.HtmlEncode(mail.MailCc);
		spMailCc.Visible = (string.IsNullOrEmpty(lMailCc.Text) == false);
		lMailBcc.Text = WebSanitizer.HtmlEncode(mail.MailBcc);
		spMailBcc.Visible = (string.IsNullOrEmpty(lMailBcc.Text) == false);
		lMailSubject.Text = WebSanitizer.HtmlEncode(mail.MailSubject);
		lMailBody.Text = StringUtility.ToWordBreakString(WebSanitizer.HtmlEncodeChangeToBr(mail.MailBody), 80);
		rMailAttachmentFiles.DataSource = mail.EX_MailAttachments;
		rMailAttachmentFiles.DataBind();
		rMailAttachmentFiles.Visible = (rMailAttachmentFiles.Items.Count != 0);

		// リクエストコメント
		lRequestComment.Text = WebSanitizer.HtmlEncodeChangeToBr(request.Comment);
	}
	#endregion

	#region -DisplayIncident インシデント表示
	/// <summary>
	/// インシデント表示
	/// </summary>
	private void DisplayIncident()
	{
		// インシデント取得
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		this.Incident = incidentService.GetWithSummaryValues(base.LoginOperatorDeptId, this.IncidentId);
		if (this.Incident == null) return;

		lIncidentId.Text = WebSanitizer.HtmlEncode(this.Incident.IncidentId);
		lUserId.Text = WebSanitizer.HtmlEncode(this.Incident.UserId);
		lIncidentTitle.Text = WebSanitizer.HtmlEncode(this.Incident.IncidentTitle);
		lIncidentCategory.Text = WebSanitizer.HtmlEncode(this.Incident.EX_IncidentCategoryName);
		lIncidentStatus.Text = WebSanitizer.HtmlEncode(this.Incident.EX_StatusText);
		lIncidentImportance.Text = WebSanitizer.HtmlEncode(this.Incident.EX_ImportanceText);

		var vocService = new CsIncidentVocService(new CsIncidentVocRepository());
		var voc = vocService.Get(this.LoginOperatorDeptId, this.Incident.VocId);
		if (voc != null) lIncidentVocText.Text = WebSanitizer.HtmlEncode(voc.VocText);

		lIncidentVocMemo.Text = WebSanitizer.HtmlEncodeChangeToBr(this.Incident.VocMemo);

		var csGroupService = new CsGroupService(new CsGroupRepository());
		var group = csGroupService.Get(this.LoginOperatorDeptId, this.Incident.CsGroupId);
		if (group != null) lIncidentCsGroupName.Text = WebSanitizer.HtmlEncode(this.Incident.EX_CsGroupName);

		var csOperatorService = new CsOperatorService(new CsOperatorRepository());
		var ope = csOperatorService.Get(this.LoginOperatorDeptId, this.Incident.OperatorId);
		if (ope != null) lIncidentCsOperatorName.Text = WebSanitizer.HtmlEncode(this.Incident.EX_CsOperatorName);

		lIncidentComment.Text = WebSanitizer.HtmlEncodeChangeToBr(this.Incident.Comment);

		// 集計区分値
		foreach (RepeaterItem ri in rIncidentSummary.Items)
		{
			var hfSummaryNo = (HiddenField)ri.FindControl("hfSummaryNo");
			var lIncidentSummaryText = (Literal)ri.FindControl("lIncidentSummaryText");

			var value = this.Incident.EX_SummaryValues.FirstOrDefault(v => v.SummaryNo == int.Parse(hfSummaryNo.Value));
			if (value != null) lIncidentSummaryText.Text = WebSanitizer.HtmlEncode(value.EX_Text);
		}

		lIncidentLastChanged.Text = WebSanitizer.HtmlEncode(this.Incident.LastChanged);
	}
	#endregion

	#region #btnApprovalCancel_Click 承認依頼取り下げボタンクリック
	/// <summary>
	/// 承認依頼取り下げボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnApprovalCancel_Click(object sender, EventArgs e)
	{
		// 承認依頼キャンセル
		UpdateMessageStatus(Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_CANCEL);
		UpdateRequestStatus(Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_APPROVE_CANCEL);

		// ロック解除
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		incidentService.UpdateLockStatusForUnlock(
			this.LoginOperatorDeptId,
			this.IncidentId,
			this.LoginOperatorName);

		btnApprovalCancel.Visible = false;
		lApprovalCancelMessage.Visible = true;
		divRefreshScript.Visible = true;
		divFadeoutAndCloseScript.Visible = true;;
	}
	#endregion

	#region #btnApprovalOK_Click 承認ボタンクリック
	/// <summary>
	/// 承認ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnApprovalOK_Click(object sender, EventArgs e)
	{
		// ステータス「OK」へ
		ChangeMessageRequestItemResultStatus(Constants.FLG_CSMESSAGEREQUESTITEM_RESULT_STATUS_OK);

		// 依頼状況によりメッセージステータス更新
		var messageService = new CsMessageService(new CsMessageRepository());
		string messageStatus = messageService.UpdateStatusFromRequestResult(this.LoginOperatorDeptId, this.IncidentId, this.MessageNo, this.RequestNo, this.LoginOperatorName);

		// 通知メール送信
		if (messageStatus != null) SendNotificationMailForRequest(MailActionType.ApprovalOK, tbResultCommnet.Text);

		// 承認作業中オペレータリセット
		ResetMessageRequestWorkingOperator();

		btnApprovalOK.Visible = false;
		lApprovalOKMessage.Visible = true;
		divRefreshScript.Visible = true;
		divFadeoutAndCloseScript.Visible = true;
	}
	#endregion

	#region #btnApprovalNG_Click 承認差し戻しボタンクリック
	/// <summary>
	/// 承認差し戻しボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnApprovalNG_Click(object sender, EventArgs e)
	{
		// ステータス「NG」へ
		ChangeMessageRequestItemResultStatus(Constants.FLG_CSMESSAGEREQUESTITEM_RESULT_STATUS_NG);

		// 依頼状況によりメッセージステータス更新
		var messageService = new CsMessageService(new CsMessageRepository());
		string messageStatus = messageService.UpdateStatusFromRequestResult(this.LoginOperatorDeptId, this.IncidentId, this.MessageNo, this.RequestNo, this.LoginOperatorName);

		// 通知メール送信
		if (messageStatus != null) SendNotificationMailForRequest(MailActionType.ApprovalNG, tbResultCommnet.Text);

		// 承認作業中オペレータリセット
		ResetMessageRequestWorkingOperator();

		btnApprovalNG.Visible = false;
		lApprovalNGMessage.Visible = true;
		divRefreshScript.Visible = true;
		divFadeoutAndCloseScript.Visible = true;
	}
	#endregion

	#region #btnApprovalOKSend_Click 承認OKからの送信ボタンクリック
	/// <summary>
	/// 承認OKからの送信ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnApprovalOKSend_Click(object sender, EventArgs e)
	{
		// メール送信
		if (SendMailForApprovalOK() == false)
		{
			lApprovalOKMessageError.Text = WebMessages.GetMessages(WebMessages.ERROR_MANAGER_FAILED_TO_SEND_THE_EMAIL);
		}
		else
		{
			this.MessageMail.SendDatetime
				= this.Incident.DateChanged
				= this.Incident.DateMessageLastSendReceived
				= DateTime.Now;

			new CsIncidentService(new CsIncidentRepository()).UpdateLastSendDate(this.Incident);
			new CsMessageMailService(new CsMessageMailRepository()).UpdateAll(this.MessageMail);
		}
	}
	#endregion

	#region #btnApprovalOKSendAndCloseIncident_Click 承認OKからの送信＆インシデントクローズボタンクリック
	/// <summary>
	/// 承認OKからの送信＆インシデントクローズボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnApprovalOKSendAndCloseIncident_Click(object sender, EventArgs e)
	{
		// メール送信
		if (SendMailForApprovalOK() == false)
		{
			lApprovalOKMessageError.Text =
				WebMessages.GetMessages(WebMessages.ERROR_MANAGER_FAILED_TO_SEND_THE_EMAIL_INCIDENT_IS_NOT_CLOSED);
			return;
		}
		else
		{
			this.MessageMail.SendDatetime
				= this.Incident.DateChanged
				= this.Incident.DateMessageLastSendReceived
				= DateTime.Now;
			new CsIncidentService(new CsIncidentRepository()).UpdateLastSendDate(this.Incident);
			new CsMessageMailService(new CsMessageMailRepository()).UpdateAll(this.MessageMail);
		}

		// インシデントクローズ
		var service = new CsIncidentService(new CsIncidentRepository());
		var incident = service.Get(this.LoginOperatorDeptId, this.IncidentId);
		incident.Status = Constants.FLG_CSINCIDENT_STATUS_COMPLETE;
		incident.DateCompleted = DateTime.Now;
		service.UpdateStatusAndCompleteDate(incident);
	}
	#endregion

	#region -SendMailForApprovalOK 承認OKからのメール送信
	/// <summary>
	/// 承認OKからのメール送信
	/// </summary>
	private bool SendMailForApprovalOK()
	{
		// メール送信
		if (SendMail(this.MessageMail, this.MessageMail.EX_MailAttachments, this.Incident.UserId, this.Incident.EX_UserMailAddr1) == false)
		{
			return false;
		}
		// メールデータ登録
		using (SqlAccessor accessor = new SqlAccessor())
		{
			accessor.OpenConnection();

			if (this.MessageMail.EX_MailDataModel != null)
			{
				this.MessageMail.EX_MailDataModel.MailId = this.MessageMail.MailId;
				var mailDataService = new CsMessageMailDataService(new CsMessageMailDataRepository());
				mailDataService.Register(this.MessageMail.EX_MailDataModel, accessor);
			}
		}

		// 返信オペレータ、ステータス更新
		var messageService = new CsMessageService(new CsMessageRepository());
		var messageNewest = messageService.Get(this.LoginOperatorDeptId, this.IncidentId, this.MessageNo);
		messageNewest.ReplyOperatorId = this.LoginOperatorId;
		messageNewest.InquiryReplyDate = DateTime.Now;
		messageNewest.MessageStatus = Constants.FLG_CSMESSAGE_MESSAGE_STATUS_DONE;
		messageNewest.LastChanged = this.LoginOperatorName;
		messageService.Update(messageNewest);

		// メール送信したらロック解除
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		incidentService.UpdateLockStatusForLock(
			this.LoginOperatorDeptId,
			this.IncidentId,
			this.LoginOperatorId,
			Constants.FLG_CSINCIDENT_LOCK_STATUS_APPR_REQ,
			Constants.FLG_CSINCIDENT_LOCK_STATUS_NONE,
			this.LoginOperatorName);

		btnApprovalOKSend.Visible = false;
		btnApprovalOKSendAndCloseIncident.Visible = false;

		lApprovalOKSendMessage.Visible = true;
		divRefreshScript.Visible = true;
		divFadeoutAndCloseScript.Visible = true;

		return true;
	}
	#endregion

	#region -SendNotificationMailForRequest 依頼通知メール送信
	/// <summary>
	/// 依頼通知メール送信
	/// </summary>
	/// <param name="mailActionType">メールアクション種別</param>
	/// <param name="comment">依頼文言</param>
	private bool SendNotificationMailForRequest(MailActionType mailActionType, string comment)
	{
		return SendNotificationMailForRequest(
			this.IncidentId,
			this.RequestOperator,
			this.LoginOperatorName,
			mailActionType,
			comment);
	}
	#endregion

	#region #btnSendCancel_Click 送信依頼取り下げボタンクリック
	/// <summary>
	/// 送信依頼取り下げボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSendCancel_Click(object sender, EventArgs e)
	{
		// 送信依頼キャンセル
		UpdateMessageStatus(Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_CANCEL);
		UpdateRequestStatus(Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_SEND_CANCEL);

		// ロック解除
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		incidentService.UpdateLockStatusForUnlock(
			this.LoginOperatorDeptId,
			this.IncidentId,
			this.LoginOperatorName);

		btnSendCancel.Visible = false;
		lSendCancelMessage.Visible = true;
		divRefreshScript.Visible = true;
		divFadeoutAndCloseScript.Visible = true;
	}
	#endregion

	#region #btnSendNG_Click 送信依頼差し戻しボタンクリック
	/// <summary>
	/// 送信依頼差し戻しボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSendNG_Click(object sender, EventArgs e)
	{
		// ステータス「NG」へ
		ChangeMessageRequestItemResultStatus(Constants.FLG_CSMESSAGEREQUESTITEM_RESULT_STATUS_NG);

		// 依頼状況によりメッセージステータス更新
		var messageService = new CsMessageService(new CsMessageRepository());
		string messageStatus = messageService.UpdateStatusFromRequestResult(this.LoginOperatorDeptId, this.IncidentId, this.MessageNo, this.RequestNo, this.LoginOperatorName);

		// 通知メール送信
		if (messageStatus != null) SendNotificationMailForRequest(MailActionType.SendNG, tbResultCommnet.Text);

		btnSendNG.Visible = false;
		lSendNGMessage.Visible = true;
		divRefreshScript.Visible = true;
		divFadeoutAndCloseScript.Visible = true;
	}
	#endregion

	#region -UpdateMessageStatus メッセージステータス更新
	/// <summary>
	/// メッセージステータス更新
	/// </summary>
	/// <param name="messageStatus">メッセージステータス</param>
	private void UpdateMessageStatus(string messageStatus)
	{
		var model = new CsMessageModel();
		model.DeptId = this.LoginOperatorDeptId;
		model.IncidentId = this.IncidentId;
		model.MessageNo = this.MessageNo;
		model.MessageStatus = messageStatus;
		model.LastChanged = this.LoginOperatorName;

		var service = new CsMessageService(new CsMessageRepository());
		service.UpdateMessageStatus(model);
	}
	#endregion

	#region -UpdateRequestStatus 依頼ステータス更新
	/// <summary>
	/// 依頼ステータス更新
	/// </summary>
	/// <param name="requestStatus">依頼ステータス</param>
	private void UpdateRequestStatus(string requestStatus)
	{
		var model = new CsMessageRequestModel();
		model.DeptId = this.LoginOperatorDeptId;
		model.IncidentId = this.IncidentId;
		model.MessageNo = this.MessageNo;
		model.RequestNo = this.RequestNo;
		model.RequestStatus = requestStatus;
		model.LastChanged = this.LoginOperatorName;

		var service = new CsMessageRequestService(new CsMessageRequestRepository());
		service.UpdateRequestStatus(model);
	}
	#endregion

	#region -ChangeMessageRequestItemResultStatus メッセージ依頼アイテム結果ステータス変更
	/// <summary>
	/// メッセージ依頼アイテム結果ステータス変更
	/// </summary>
	/// <param name="resultStatus">結果ステータス</param>
	private void ChangeMessageRequestItemResultStatus(string resultStatus)
	{
		var model = new CsMessageRequestItemModel();
		model.DeptId = this.LoginOperatorDeptId;
		model.IncidentId = this.IncidentId;
		model.MessageNo = this.MessageNo;
		model.RequestNo = this.RequestNo;
		model.BranchNo = this.BranchNo;
		model.ApprOperatorId = this.LoginOperatorId;
		model.ResultStatus = resultStatus;
		model.Comment = tbResultCommnet.Text;
		model.LastChanged = this.LoginOperatorName;

		var service = new CsMessageRequestItemService(new CsMessageRequestItemRepository());
		service.UpdateResult(model);
	}
	#endregion

	#region -ResetMessageRequestWorkingOperator メッセージリクエスト作業オペレータリセット
	/// <summary>
	/// メッセージリクエスト作業オペレータリセット
	/// </summary>
	private void ResetMessageRequestWorkingOperator()
	{
		// 自分が作業中か確認
		var service = new CsMessageRequestService(new CsMessageRequestRepository());
		var request = service.Get(this.LoginOperatorDeptId, this.IncidentId, this.MessageNo, this.RequestNo);
		if (request.WorkingOperatorId != this.LoginOperatorId) return;

		// 作業オペレータリセット
		request.WorkingOperatorId = "";
		request.LastChanged = this.LoginOperatorName;
		service.UpdateWorkingOperator(request);
	}
	#endregion

	#region プロパティ
	/// <summary>インシデントID</summary>
	private string IncidentId
	{
		get { return Request[Constants.REQUEST_KEY_INCIDENT_ID]; }
	}
	/// <summary>インシデント</summary>
	private CsIncidentModel Incident
	{
		get { return (CsIncidentModel)ViewState["Incident"]; }
		set { ViewState["Incident"] = value; }
	}
	/// <summary>メッセージNO</summary>
	private int MessageNo
	{
		get
		{
			int messageNo;
			return int.TryParse(Request[Constants.REQUEST_KEY_MESSAGE_NO], out messageNo) ? messageNo : 0;
		}
	}
	/// <summary>リクエストNO</summary>
	private int RequestNo
	{
		get
		{
			int requestNo;
			return int.TryParse(Request[Constants.REQUEST_KEY_REQUEST_NO], out requestNo) ? requestNo : 0;
		}
	}
	/// <summary>リクエスト枝番</summary>
	private int BranchNo
	{
		get
		{
			int branchNo;
			return int.TryParse(Request[Constants.REQUEST_KEY_BRANCH_NO], out branchNo) ? branchNo : 0;
		}
	}
	/// <summary>メールアクション種別</summary>
	protected MailActionType MailActionType
	{
		get { return GetMailActionTypeFromKbn(Request[Constants.REQUEST_KEY_MAILACTION]); }
	}
	/// <summary>メッセージ</summary>
	private CsMessageModel Message
	{
		get { return (CsMessageModel)ViewState["Message"]; }
		set { ViewState["Message"] = value; }
	}
	/// <summary>メッセージリクエスト</summary>
	private CsMessageRequestModel MessageRequest
	{
		get { return (CsMessageRequestModel)ViewState["MessageRequest"]; }
		set { ViewState["MessageRequest"] = value; }
	}
	/// <summary>メッセージメール</summary>
	private CsMessageMailModel MessageMail
	{
		get { return (CsMessageMailModel)ViewState["MessageMail"]; }
		set { ViewState["MessageMail"] = value; }
	}
	/// <summary>リクエストオペレータ</summary>
	private CsOperatorModel RequestOperator
	{
		get { return (CsOperatorModel)ViewState["RequestOperator"]; }
		set { ViewState["RequestOperator"] = value; }
	}
	#endregion
}
