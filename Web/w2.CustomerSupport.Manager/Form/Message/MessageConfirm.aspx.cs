/*
=========================================================================================================
  Module      : メッセージ確認（メールプレビュー）ページ処理(MessageConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Cs.ErrorPoint;
using w2.App.Common.Cs.Incident;
using w2.App.Common.Cs.Message;
using w2.App.Common.Cs.SummarySetting;
using w2.Common.Net.Mail;
using w2.Domain.User;

public partial class Form_Message_MessageConfirm : BasePageCs
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
			GetParams((Hashtable)Session[Constants.SESSION_KEY_PARAM]);

			DisplayUserInfo(this.Incident.UserId);

			DisplayIncidentInfo(this.Incident);

			DisplayMail();

			if (this.MailActionType == MailActionType.ApprovalRequest) DisplayApprovalRequest();
			if (this.MailActionType == MailActionType.MailSendRequest) DisplayMailSendRequest();

			// エラーポイントの警告表示
			CsMailErrorAddrService service = new CsMailErrorAddrService(new CsMailErrorAddrRepository());
			CsMailErrorAddrModel model = service.Get(this.MessageMail.MailTo);
			if ((model != null) && (model.ErrorPoint >= Constants.DISPLAY_ERROR_POINT))
			{
				lErrorPoint.Text = WebSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERROR_MANAGER_POINT_FOR_THIS_EMAIL_ADDRESS)
					.Replace("@@ 1 @@", this.MessageMail.MailTo)
					.Replace("@@ 2 @@", model.ErrorPoint.ToString("#,0")));
				lErrorPoint.ForeColor = System.Drawing.Color.Red;
			}
		}
	}
	#endregion

	#region -GetParams パラメタ取得してプロパティへセット
	/// <summary>
	/// パラメタ取得してプロパティへセット
	/// </summary>
	/// <param name="param">パラメタ</param>
	private void GetParams(Hashtable param)
	{
		switch (Request[Constants.REQUEST_KEY_MAILACTION])
		{
			case Constants.KBN_MAILACTION_SEND:
				this.MailActionType = MailActionType.MailSend;
				break;

			case Constants.KBN_MAILACTION_APPROVE_REQ:
				this.MailActionType = MailActionType.ApprovalRequest;
				break;

			case Constants.KBN_MAILACTION_SEND_REQ:
				this.MailActionType = MailActionType.MailSendRequest;
				break;
		}

		this.Incident = (CsIncidentModel)param["Incident"];
		this.Message = (CsMessageModel)param["Message"];
		this.BeforeUrl = (string)param["BeforeUrl"];
		if (this.Message != null)
		{
			this.MessageMail = this.Message.EX_Mail;
			this.MailAttachemnts = this.Message.EX_Mail.EX_MailAttachments;
		}
		if ((this.Incident == null) || (this.Message == null) || (this.MessageMail == null))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERROR_MANAGER_PARAMETER_CANNOT_BE_FOUND);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}
	#endregion

	#region -DisplayUserInfo 顧客情報表示
	/// <summary>
	/// 顧客情報表示
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	private void DisplayUserInfo(string userId)
	{
		UserModel user = null;
		if (string.IsNullOrEmpty(userId))
		{
			var addressList = (this.MessageMail.MailTo.Split(',')
				.Where(m => (m.Trim() != ""))
				.Select(MailAddress.GetInstance)).ToArray();
			var users = new UserService().GetUsersByMailAddr(addressList[0].AddressRaw);
			if (users.Length == 1)
			{
				user = users[0];
			}
			else if (users.Length > 1)
			{
				lUserId.Text = string.Format(
					"<font color='red'>{0}</font>",
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MAILADDRESS_DUPLICATE_ERROR));
			}
		}
		else
		{
			user = new UserService().Get(userId);
		}

		if (user == null) return;

		lUserName.Text = WebSanitizer.HtmlEncode(user.Name);
		lUserNameKana.Text = WebSanitizer.HtmlEncode(user.NameKana);
		lUserMail.Text = WebSanitizer.HtmlEncodeChangeToBr((user.MailAddr + "\r\n" + user.MailAddr2).Trim());
		lUserTel.Text = WebSanitizer.HtmlEncode(user.Tel1);
		lUserId.Text = WebSanitizer.HtmlEncode(user.UserId);
	}
	#endregion

	#region -DisplayIncidentInfo インシデント情報表示
	/// <summary>
	/// インシデント情報表示
	/// </summary>
	/// <param name="incident">インシデント</param>
	private void DisplayIncidentInfo(CsIncidentModel incident)
	{
		lIncidentId.Text = WebSanitizer.HtmlEncode(string.IsNullOrEmpty(incident.IncidentId)
			? WebMessages.GetMessages(WebMessages.MSG_MANAGER_NEWLY_NUMBERED)
			: incident.IncidentId);
		lIncidentTitle.Text = WebSanitizer.HtmlEncode(incident.IncidentTitle);
		lIncidentCategoryName.Text = WebSanitizer.HtmlEncode(incident.EX_IncidentCategoryName);
		lIncidentStatus.Text = WebSanitizer.HtmlEncode(incident.EX_StatusText);
		lIncidentImportance.Text = WebSanitizer.HtmlEncode(incident.EX_ImportanceText);
		lIncidentVoc.Text = WebSanitizer.HtmlEncode(incident.EX_VocText);
		lIncidentVocMemo.Text = WebSanitizer.HtmlEncode(incident.VocMemo);
		lIncidentOperatorGroup.Text = WebSanitizer.HtmlEncode(incident.EX_CsGroupName);
		lIncidentOperatorName.Text = WebSanitizer.HtmlEncode(incident.EX_CsOperatorName);
		lIncidentComment.Text = WebSanitizer.HtmlEncodeChangeToBr(incident.Comment);

		var summarySettingService = new CsSummarySettingService(new CsSummarySettingRepository());
		rIncidentSummary.DataSource = summarySettingService.GetValidAllWithValidItems(this.LoginOperatorDeptId);
		rIncidentSummary.DataBind();
		foreach (RepeaterItem ri in rIncidentSummary.Items)
		{
			var hfSummaryNo = (HiddenField)ri.FindControl("hfSummaryNo");
			var find = incident.EX_SummaryValues.FirstOrDefault(s => s.SummaryNo.ToString() == hfSummaryNo.Value);
			if (find == null) break;

			var lIncidentSummaryText = (Literal)ri.FindControl("lIncidentSummaryText");
			lIncidentSummaryText.Text = WebSanitizer.HtmlEncode(find.EX_Text);
		}
	}
	#endregion

	#region -DisplayMail メール表示
	/// <summary>
	/// メール表示
	/// </summary>
	private void DisplayMail()
	{
		lPreviewMailTo.Text = WebSanitizer.HtmlEncode(this.MessageMail.MailTo);
		lPreviewMailCc.Text = WebSanitizer.HtmlEncode(this.MessageMail.MailCc);
		trPreviewCc.Visible = (string.IsNullOrEmpty(this.MessageMail.MailCc) == false);
		lPreviewMailBcc.Text = WebSanitizer.HtmlEncode(this.MessageMail.MailBcc);
		trPreviewBcc.Visible = (string.IsNullOrEmpty(this.MessageMail.MailBcc) == false);
		lPreviewMailFrom.Text = WebSanitizer.HtmlEncode(this.MessageMail.MailFrom);
		lPreviewMailSubject.Text = WebSanitizer.HtmlEncode(this.MessageMail.MailSubject);
		lPreviewMailBody.Text = StringUtility.ToWordBreakString(WebSanitizer.HtmlEncodeChangeToBr(this.MessageMail.MailBody), 80);
		trPreviewAttachment.Visible = (this.MailAttachemnts.Length != 0);
		rPreviewAttachmentFiles.DataSource = this.MailAttachemnts;
		rPreviewAttachmentFiles.DataBind();
	}
	#endregion

	#region -DisplayApprovalRequest 承認依頼情報表示
	/// <summary>
	/// 承認依頼情報表示
	/// </summary>
	private void DisplayApprovalRequest()
	{
		rApprovalOperators.DataSource = this.Message.EX_Request.EX_Items;
		rApprovalOperators.DataBind();
		lApprovalUrgencyFlg.Text = WebSanitizer.HtmlEncode(this.Message.EX_Request.UrgencyFlgText);
		lApprovalType.Text = WebSanitizer.HtmlEncode(this.Message.EX_Request.EX_ApprovalTypeName);
		lApprovalRequestComment.Text = WebSanitizer.HtmlEncodeChangeToBr(this.Message.EX_Request.Comment);
	}
	#endregion

	#region -DisplayMailSendRequest メール送信依頼情報表示
	/// <summary>
	/// メール送信依頼情報表示
	/// </summary>
	private void DisplayMailSendRequest()
	{
		rMailSendableOperators.DataSource = this.Message.EX_Request.EX_Items;
		rMailSendableOperators.DataBind();
		lMailSendRequestUrgencyFlg.Text = WebSanitizer.HtmlEncode(this.Message.EX_Request.UrgencyFlgText);
		lMailSendRequestComment.Text = WebSanitizer.HtmlEncodeChangeToBr(this.Message.EX_Request.Comment);
	}
	#endregion

	#region #btnSendMail_Click メール送信ボタンクリック
	/// <summary>
	/// メール送信ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSendMail_Click(object sender, EventArgs e)
	{
		// メール送信
		if (SendMail(this.MessageMail, this.MailAttachemnts, this.Incident.UserId, this.Incident.EX_UserMailAddr1) == false)
		{
			lErrorMessages.Text = WebMessages.GetMessages(WebMessages.ERROR_MANAGER_FAILED_TO_SEND_EMAIL);
			return;
		}

		// 返信オペレータセット
		this.Message.ReplyOperatorId = this.LoginOperatorId;

		// 送信代行の場合は依頼ステータス変更
		UpdateRequestStatusForSendRequest();

		// 問合日時・メール送信日セット
		this.MessageMail.SendDatetime
			= this.Message.InquiryReplyDate
			= DateTime.Now;

		// インシデントの送信日時を更新
		this.Incident.DateMessageLastSendReceived = DateTime.Now;
		new CsIncidentService(new CsIncidentRepository()).UpdateLastSendDate(this.Incident);

		// インシデント・メッセージ・メール登録
		RegisterUpdateIncidentAndMessageAll(
			this.Incident,
			this.Message,
			this.MessageMail,
			this.MailAttachemnts);

		btnSendMail.Visible = false;
		btnBack.Visible = false;
		lErrorPoint.Visible = false;
		Session[Constants.SESSION_KEY_PARAM] = null;

		lCompleteMessages.Text = WebMessages.GetMessages(WebMessages.ERROR_MANAGER_SENT_AN_EMAIL);

		// ロック解除
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		incidentService.UpdateLockStatusForLock(
			this.LoginOperatorDeptId,
			this.Incident.IncidentId,
			this.LoginOperatorId,
			Constants.FLG_CSINCIDENT_LOCK_STATUS_EDIT,
			Constants.FLG_CSINCIDENT_LOCK_STATUS_NONE,
			this.LoginOperatorName);

		// トップページ更新・クローズ
		ScriptManager.RegisterStartupScript(this, this.GetType(), "CloseWindows", "setTimeout('fadeout_and_close()', 500);", true);
	}
	#endregion

	#region -UpdateRequestStatusForSendRequest 送信依頼向け依頼ステータス更新
	/// <summary>
	/// 送信依頼向け依頼ステータス更新
	/// </summary>
	private void UpdateRequestStatusForSendRequest()
	{
		var messageService = new CsMessageService(new CsMessageRepository());
		var message = messageService.Get(this.Message.DeptId, this.Message.IncidentId, this.Message.MessageNo);
		if ((message != null)
			&& (message.MessageStatus == Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_REQ))
		{
			var requestService = new CsMessageRequestService(new CsMessageRequestRepository());
			var requests = requestService.GetAllWithItems(this.Message.DeptId, this.Message.IncidentId, this.Message.MessageNo);
			foreach (var req in requests.Where(r =>
				(r.RequestStatus == Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_SEND_REQ) && (r.EX_Items[0].ApprOperatorId == this.LoginOperatorId)))
			{
				req.RequestStatus = Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_SEND_OK;
				requestService.UpdateRequestStatus(req);

				var item = req.EX_Items[0];
				item.ResultStatus = Constants.FLG_CSMESSAGEREQUESTITEM_RESULT_STATUS_OK;
				var requestItemService = new CsMessageRequestItemService(new CsMessageRequestItemRepository());
				requestItemService.UpdateResult(item);
				break;
			}
		}
	}
	#endregion

	#region #btnApproveRequest_Click 承認依頼ボタンクリック
	/// <summary>
	/// 承認依頼ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnApproveRequest_Click(object sender, EventArgs e)
	{
		RegistRequest();

		btnApproveRequest.Visible = false;
		btnBack.Visible = false;
		Session[Constants.SESSION_KEY_PARAM] = null;
		lCompleteMessages.Text = WebMessages.GetMessages(WebMessages.MSG_MANAGER_I_DID)
			.Replace("@@ 1 @@", GetMailActionString(this.MailActionType));

		// 承認依頼ロック取得
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		incidentService.UpdateLockStatusForLock(
			this.LoginOperatorDeptId,
			this.Incident.IncidentId,
			this.LoginOperatorId,
			Constants.FLG_CSINCIDENT_LOCK_STATUS_EDIT,
			Constants.FLG_CSINCIDENT_LOCK_STATUS_APPR_REQ,
			this.LoginOperatorName);

		// トップページ更新・クローズ
		ScriptManager.RegisterStartupScript(this, this.GetType(), "CloseWindows", "setTimeout('fadeout_and_close()', 500);", true);
	}
	#endregion

	#region #btnSendRequest_Click 送信依頼ボタンクリック
	/// <summary>
	/// 送信依頼ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSendRequest_Click(object sender, EventArgs e)
	{
		RegistRequest();

		btnSendRequest.Visible = false;
		btnBack.Visible = false;
		Session[Constants.SESSION_KEY_PARAM] = null;
		lCompleteMessages.Text = WebMessages.GetMessages(WebMessages.MSG_MANAGER_I_DID)
			.Replace("@@ 1 @@", GetMailActionString(this.MailActionType));

		// 送信依頼ロック取得
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		incidentService.UpdateLockStatusForLock(
			this.LoginOperatorDeptId,
			this.Incident.IncidentId,
			this.LoginOperatorId,
			Constants.FLG_CSINCIDENT_LOCK_STATUS_EDIT,
			Constants.FLG_CSINCIDENT_LOCK_STATUS_SEND_REQ,
			this.LoginOperatorName);

		// トップページ更新
		ScriptManager.RegisterStartupScript(this, this.GetType(), "CloseWindows", "setTimeout('fadeout_and_close()', 500);", true);
	}
	#endregion

	#region -RegistRequest
	/// <summary>
	/// 依頼登録
	/// </summary>
	private void RegistRequest()
	{
		// 依頼では問合せ・回答日はnull
		this.Message.InquiryReplyDate = null;

		// インシデント・メッセージ・メール登録
		RegisterUpdateIncidentAndMessageAll(
			this.Incident,
			this.Message,
			this.MessageMail,
			this.MailAttachemnts);

		// 依頼通知メール送信
		SendNotificationMail(this.Message.EX_Request);
	}
	#endregion

	#region -SendNotificationMail 依頼通知メール送信
	/// <summary>
	/// 依頼通知メール送信
	/// </summary>
	/// <param name="request">メッセージ依頼モデル</param>
	private void SendNotificationMail(CsMessageRequestModel request)
	{
		var csOperatorService = new CsOperatorService(new CsOperatorRepository());
		foreach (var item in request.EX_Items)
		{
			var ope = csOperatorService.Get(this.LoginOperatorDeptId, item.ApprOperatorId);
			if (ope == null) continue;

			SendNotificationMailForRequest(
				request.IncidentId,
				ope,
				this.LoginOperatorName,
				this.MailActionType,
				request.Comment);
		}
	}
	#endregion

	#region #btnBack_Click 戻るボタンクリック
	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = Session[Constants.SESSION_KEY_PARAM];

		Response.Redirect(this.BeforeUrl);
	}
	#endregion

	#region プロパティ
	/// <summary>メールアクション種別</summary>
	protected MailActionType MailActionType
	{
		get { return (MailActionType)ViewState["MailActionType"]; }
		private set { ViewState["MailActionType"] = value; }
	}
	/// <summary>インシデント</summary>
	private CsIncidentModel Incident
	{
		get { return (CsIncidentModel)ViewState["Incident"]; }
		set { ViewState["Incident"] = value; }
	}
	/// <summary>メッセージ</summary>
	private CsMessageModel Message
	{
		get { return (CsMessageModel)ViewState["Message"]; }
		set { ViewState["Message"] = value; }
	}
	/// <summary>前ページURL</summary>
	private string BeforeUrl
	{
		get { return (string)ViewState["BeforeUrl"]; }
		set { ViewState["BeforeUrl"] = value; }
	}
	/// <summary>メッセージメール</summary>
	private CsMessageMailModel MessageMail
	{
		get { return (CsMessageMailModel)ViewState["MessageMail"]; }
		set { ViewState["MessageMail"] = value; }
	}
	/// <summary>メール添付ファイル</summary>
	private CsMessageMailAttachmentModel[] MailAttachemnts
	{
		get { return (CsMessageMailAttachmentModel[])ViewState["MailAttachemnts"]; }
		set { ViewState["MailAttachemnts"] = value; }
	}
	#endregion
}
