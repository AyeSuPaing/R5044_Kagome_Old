/*
=========================================================================================================
  Module      : メッセージページ処理(MessageInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.AnswerTemplate;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Cs.Incident;
using w2.App.Common.Cs.Message;
using w2.App.Common.Cs.UserHistory;
using w2.Domain.User;

public partial class Form_Message_MessageInput : BasePageCs
{
	#region 定数
	/// <summary>顧客一覧表示件数</summary>
	private const int USER_LIST_COUNT_MAX = 50;
	/// <summary>Tag use for replacing</summary>
	private const string REPLACE_TAG = "@@ {0} @@";
	/// <summary>Operator name</summary>
	private const string OPERATOR_NAME = "operator_name";
	/// <summary>Mail action</summary>
	private const string MAIL_ACTION = "MailAction";
	/// <summary>メールアクションフラグ 承認依頼</summary>
	private const string FLG_MAIL_ACTION_APPROVAL_REQUEST = "ApprovalRequest";
	/// <summary>メールアクション 送信依頼</summary>
	private const string FLG_MAIL_ACTION_MAILSEND_REQUEST = "MailSendRequest";
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
			Initialize();

			// レイアウト上、マスタページの閉じるボタンを表示しない
			((Form_Common_PopupPage)Master).HideCloseButton = true;

			UpdateIncidentForEditLock(this.RequestIncidentId);

			InitializeRequestForm();	// 依頼フォーム初期化

			Display();

			RestoreForHistoryBack();

			if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_MESSAGE_NO]) == false)
			{
				ScriptManager.RegisterStartupScript(this, this.GetType(), "MyAction", "setTimeout('select_message_list(" + Request[Constants.REQUEST_KEY_MESSAGE_NO] + ")', 100);", true);
			}

			RefreshActionButtonAndockMessage();
		}
	}
	#endregion

	#region -Initialize 初期化
	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		this.MailAddressList = new List<string>();

		// 回答例カテゴリセット
		var answerTemplateService = new CsAnswerTemplateCategoryService(new CsAnswerTemplateCategoryRepository());
		var answerTemplateModels = answerTemplateService.GetValidAll(this.LoginOperatorDeptId);
		ddlAnswerTemplateCategories.Items.Add(new ListItem("", ""));
		ddlAnswerTemplateCategories.Items.AddRange(
			(from m in answerTemplateModels select new ListItem(m.EX_RankedCategoryName, m.CategoryId)).ToArray());

		// 承認依頼の承認方法セット
		rblApprovalType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_CSMESSAGEREQUEST, Constants.FIELD_CSMESSAGEREQUEST_APPROVAL_TYPE));
		rblApprovalType.SelectedValue = Constants.MESSAGEREQUEST_APPROVAL_TYPE_DEFAULT;

		// アクション権限制御（メール送信直接権限がない場合は送信NG）
		var mailActionList = rblMailAction.Items.Cast<ListItem>().ToArray();
		if (this.LoginOperatorCsInfo.EX_PermitMailSendFlg == false)
		{
			mailActionList.ToList().ForEach(li => li.Enabled = (li.Value != "MailSend"));
		}
		mailActionList.First(li => li.Enabled).Selected = true;

		// 顧客検索一覧列数セット
		tdUserSearchResultAlert.ColSpan = Constants.DISPLAY_CORPORATION_ENABLED ? 5 : 3;
	}
	#endregion

	#region -RestoreForHistoryBack 確認画面からの戻り向け画面セット
	/// <summary>
	/// 確認画面からの戻り向け画面セット
	/// </summary>
	private void RestoreForHistoryBack()
	{
		if (Session[Constants.SESSION_KEY_PARAM_FOR_BACK] == null) return;

		var param = (Hashtable)Session[Constants.SESSION_KEY_PARAM_FOR_BACK];
		var incident = (CsIncidentModel)param["Incident"];
		var message = (CsMessageModel)param["Message"];
		var mailAction = StringUtility.ToEmpty(param[MAIL_ACTION]);

		ucMessageRightIncident.SetIncident(incident);
		ucMessageRightMail.SetMessageInfoForEdit(message);
		// Get mail action
		rblMailAction.SelectedValue = mailAction;

		// 依頼フォーム初期化
		InitializeRequestForm();

		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = null;
		if (message.EX_Request == null) return;

		// 画面へセット
		var operatorIds = message.EX_Request.EX_Items.Select(item => item.ApprOperatorId).ToArray();
		switch (mailAction)
		{
			case FLG_MAIL_ACTION_APPROVAL_REQUEST:
				ChangeApprovalOperators(operatorIds);
				cbApprovalUrgencyFlg.Checked = (message.EX_Request.UrgencyFlg == Constants.FLG_CSMESSAGEREQUEST_URGENCY_URGENT);
				rblApprovalType.Items.Cast<ListItem>().ToList().ForEach(li => li.Selected = (li.Value == message.EX_Request.ApprovalType));
				tbApprovalRequestComment.Text = message.EX_Request.Comment;
				break;

			case FLG_MAIL_ACTION_MAILSEND_REQUEST:
				for (int noLoop = 0; noLoop < rMailSendableOperators.Items.Count; noLoop++)
				{
					foreach (ListItem item in ((DropDownList)rMailSendableOperators.Items[noLoop].FindControl("ddlOperator")).Items)
					{
						item.Selected = (item.Value == operatorIds[noLoop]);
					}
				}
				cbMailSendRequestUrgencyFlg.Checked = (message.EX_Request.UrgencyFlg == Constants.FLG_CSMESSAGEREQUEST_URGENCY_URGENT);
				tbMailSendRequestComment.Text = message.EX_Request.Comment;
				break;
		}
	}
	#endregion

	#region -UpdateIncidentForEditLock 編集ロック取得用にインシデント更新
	/// <summary>
	/// 編集ロック取得用にインシデント更新
	/// </summary>
	/// <param name="incidentId">インシデントID</param>
	private void UpdateIncidentForEditLock(string incidentId)
	{
		if (string.IsNullOrEmpty(incidentId)) return;

		var service = new CsIncidentService(new CsIncidentRepository());
		var model = service.GetWithSummaryValues(this.LoginOperatorDeptId, incidentId);
		if (model == null) return;
		if ((model.LockStatus != Constants.FLG_CSINCIDENT_LOCK_STATUS_NONE)
			&& (model.LockOperatorId == this.LoginOperatorId)) return;

		service.UpdateLockStatusForLock(
			this.LoginOperatorDeptId,
			incidentId,
			this.LoginOperatorId,
			Constants.FLG_CSINCIDENT_LOCK_STATUS_NONE,
			Constants.FLG_CSINCIDENT_LOCK_STATUS_EDIT,
			this.LoginOperatorName);
	}
	#endregion

	#region -Display 画面表示
	/// <summary>
	/// 画面表示
	/// </summary>
	private void Display()
	{
		// ユーザーIDありの場合は顧客選択状態とする
		if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_USER_ID]) == false)
		{
			DispUserInfo(Request[Constants.REQUEST_KEY_USER_ID]);

			ChangeLeftDispMode(DispLeftMode.UserInfo);

			ReflectRightUserFromUser(hfSelectedUserId.Value);
		}
		// インシデントID有りの場合はインシデント選択状態とする。
		// メッセージが特定出来たら、ステータスによって編集か引用かを切り替える
		else if (string.IsNullOrEmpty(this.RequestIncidentId) == false)
		{
			var incident = DispIncidentLeft(this.RequestIncidentId);
			if (incident != null) DispUserInfo(incident.UserId);

			CsMessageModel message = null;
			if (this.RequestMessageNo.HasValue)
			{
				var messageService = new CsMessageService(new CsMessageRepository());
				message = messageService.GetWithMail(this.LoginOperatorDeptId, this.RequestIncidentId, this.RequestMessageNo.Value);
			}
			if (this.EditMode == EditMode.Reply) DispMessageDetailLeft(message);
			SetRightQuotation(message);

			// 各種情報差し込み
			ReflectIncident();
			if (string.IsNullOrEmpty(incident.UserId) == false && this.EditMode != EditMode.Reply)
			{
				ReflectRightUserFromUser(incident.UserId);
			}
			else
			{
				ReflectRightUserFromMessage();
			}
			switch (this.EditMode)
			{
				case EditMode.EditDraft:
				case EditMode.EditForSend:
				case EditMode.EditDone:
					if (message.MediaKbn == Constants.FLG_CSMESSAGE_MEDIA_KBN_MAIL)
					{
						// メール情報セット
						ucMessageRightMail.SetMessageInfoForEdit(message);

						// メール依頼情報セット
						DisplayMessageRequestInfoForEditDraft(message);
					}
					else
					{
						// 電話情報セット
						ucMessageRightTel.SetMessageInfoForEdit(message);
					}
					break;

				case EditMode.Reply:
					ucMessageRightMail.InsertSubjectRe();
					ucMessageRightMail.InsertQuotationBody();
					break;
			}

			ChangeLeftDispMode(DispLeftMode.Incident);
		}
		// パラメタ無しの場合は顧客検索表示
		else
		{
			ChangeLeftDispMode(DispLeftMode.UserSearch);
		}

		// 右画面表示制御
		ucMessageRightTel.Visible = (this.DispRightMode == DispRightMode.TelOthers);
		ucMessageRightMail.Visible = (this.DispRightMode == DispRightMode.Mail);
	}
	#endregion

	/// <summary>
	/// 下書き編集向けメッセージ依頼情報表示
	/// </summary>
	/// <param name="message"></param>
	private void DisplayMessageRequestInfoForEditDraft(CsMessageModel message)
	{
		if ((message.MessageStatus != Constants.FLG_CSMESSAGE_MESSAGE_STATUS_DRAFT)
			|| (Session[Constants.SESSION_KEY_PARAM_FOR_BACK] != null))
		{
			return;
		}

		// 依頼取得
		var service = new CsMessageRequestService(new CsMessageRequestRepository());
		var request = service.GetDraftWithAllItems(message.DeptId, message.IncidentId, message.MessageNo);
		if (request == null) return;

		// 依頼フォーム切替・再度初期化
		if (request.RequestType == Constants.FLG_CSMESSAGEREQUEST_REQUEST_TYPE_APPROVE)
		{
			rblMailAction.SelectedValue = FLG_MAIL_ACTION_APPROVAL_REQUEST;
		}
		else if (request.RequestType == Constants.FLG_CSMESSAGEREQUEST_REQUEST_TYPE_MAILSEND)
		{
			rblMailAction.SelectedValue = FLG_MAIL_ACTION_MAILSEND_REQUEST;
		}
		InitializeRequestForm();

		// 画面へセット
		var operatorIds = (from item in request.EX_Items select item.ApprOperatorId).ToArray();
		switch (request.RequestType)
		{
			case Constants.FLG_CSMESSAGEREQUEST_REQUEST_TYPE_APPROVE:
				ChangeApprovalOperators(operatorIds);
				cbApprovalUrgencyFlg.Checked = (request.UrgencyFlg == Constants.FLG_CSMESSAGEREQUEST_URGENCY_URGENT);
				rblApprovalType.Items.Cast<ListItem>().ToList().ForEach(li => li.Selected = (li.Value == request.ApprovalType));
				tbApprovalRequestComment.Text = request.Comment;
				break;

			case Constants.FLG_CSMESSAGEREQUEST_REQUEST_TYPE_MAILSEND:
				for (int i = 0; i < rMailSendableOperators.Items.Count; i++)
				{
					foreach (ListItem li in ((DropDownList)rMailSendableOperators.Items[i].FindControl("ddlOperator")).Items)
					{
						li.Selected = (li.Value == operatorIds[i]);
					}
				}
				cbMailSendRequestUrgencyFlg.Checked = (request.UrgencyFlg == Constants.FLG_CSMESSAGEREQUEST_URGENCY_URGENT);
				tbMailSendRequestComment.Text = request.Comment;
				break;
		}
	}

	#region #lbChangeDispMode_Click モードチェンジリンククリック
	/// <summary>
	/// モードチェンジリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbChangeDispMode_Click(object sender, EventArgs e)
	{
		ChangeLeftDispMode((DispLeftMode)Enum.Parse(typeof(DispLeftMode), ((LinkButton)sender).CommandArgument));
	}
	#endregion

	#region #btnUserSearch_Click 顧客検索ボタンクリック
	/// <summary>
	/// 顧客検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUserSearch_Click(object sender, EventArgs e)
	{
		Hashtable input = new Hashtable();
		input.Add(Constants.FIELD_USER_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(tbSearchUserName.Text.Trim()));
		input.Add(Constants.FIELD_USER_TEL1 + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(tbSearchTel.Text.Trim()));
		input.Add(Constants.FIELD_USER_MAIL_ADDR + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(tbSearchMailAddr.Text.Trim()));
		input.Add(Constants.FIELD_USER_USER_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(tbSearchUserId.Text.Trim()));
		input.Add(Constants.FIELD_ORDER_ORDER_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(tbSearchOrderId.Text.Trim()));

		// ユーザー件数取得・表示
		int searchCount = 0;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("CsUserSearch", "SearchUserCount"))
		{
			searchCount = (int)sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input)[0][0];
		}
		lUserSearchCount.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(searchCount) + "件");
		if (searchCount > USER_LIST_COUNT_MAX)
		{
			lUserSearchCount.Text += WebMessages.GetMessages(WebMessages.MSG_MANAGER_USER_LIST_COUNT_MAX_DISPLAYED)
				.Replace("@@ 1 @@", USER_LIST_COUNT_MAX.ToString());
		}

		// ユーザー一覧表示
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("CsUserSearch", "SearchUser"))
		{
			sqlStatement.Statement = sqlStatement.Statement.Replace("@@ count_max @@", USER_LIST_COUNT_MAX.ToString());

			var users = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
			this.rUserSearchResult.DataSource = users;

			// User search list
			this.UserSearchMessageInput = users.Table;
		}
		rUserSearchResult.DataBind();
		trUserSearchResultAlert.Visible = (rUserSearchResult.Items.Count == 0);

		divUserList.Visible = true;

		// Set default sort
		SetDefaultSort();
	}
	#endregion

	#region #lbSelectUser_Click ユーザー選択リンククリック
	/// <summary>
	/// ユーザー選択リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSelectUser_Click(object sender, EventArgs e)
	{
		DispUserInfo(hfSelectedUserId.Value);

		ChangeLeftDispMode(DispLeftMode.UserInfo);
	}
	#endregion

	#region #lbSelectIncident_Click インシデント選択リンククリック
	/// <summary>
	/// インシデント選択リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSelectIncident_Click(object sender, EventArgs e)
	{
		// インシデント表示
		DispIncidentLeft(hfSelectedIncidentId.Value);

		// 表示切り替え
		ChangeLeftDispMode(DispLeftMode.Incident);

		divMessageTel.Visible = false;
		divMessageMail.Visible = false;

		ScriptManager.RegisterStartupScript(this, this.GetType(), "MyAction", "select_message_list(" + hfSelectedMessageNo.Value + ");", true);
	}
	#endregion

	#region -DispIncidentLeft 左インシデント情報表示
	/// <summary>
	/// 左インシデント情報表示
	/// </summary>
	/// <param name="incidentId">インシデントID</param>
	/// <returns>インシデント</returns>
	private CsIncidentModel DispIncidentLeft(string incidentId)
	{
		// インシデント取得
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		var incident = incidentService.GetWithSummaryValues(base.LoginOperatorDeptId, incidentId);
		if (incident == null) return null;

		hfSelectedIncidentId.Value = incidentId;
		this.IsIncidentLocked = (incident.LockStatus != "");

		imgLockTop.Visible = this.IsIncidentLocked;
		ScriptManager.RegisterStartupScript(up3, up3.GetType(), "refresh_incident_info_opener", "refresh_incident_info_opener('" + incident.IncidentId + "', " + this.IsIncidentLocked.ToString().ToLower() + ");", true);

		lIncidentId.Text = WebSanitizer.HtmlEncode(incident.IncidentId);
		lIncidentTitle.Text = WebSanitizer.HtmlEncode(incident.IncidentTitle);
		lIncidentComment.Text = WebSanitizer.HtmlEncode(incident.Comment);

		// メッセージ履歴取得
		var messageService = new CsMessageService(new CsMessageRepository());
		var messageList = 
			messageService.GetValidAll(base.LoginOperatorDeptId, incidentId)
			.OrderByDescending(m => m.EX_InquiryReplyChangedDate).ToArray();
		rIncidentRequires.DataSource = messageList;
		rIncidentRequires.DataBind();

		// 最終対応者をインシデントの担当者とする。
		lIncidentOperatorName.Text = WebSanitizer.HtmlEncode((messageList.Length != 0) ? messageList[0].EX_OperatorName : "");

		return incident;
	}
	#endregion

	#region -RefreshActionButtonAndockMessage アクションボタン＆メッセージ表示制御
	/// <summary>
	/// アクションボタン＆メッセージ表示制御
	/// </summary>
	/// <returns>インシデント</returns>
	private void RefreshActionButtonAndockMessage()
	{
		// インシデント取得
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		var incident = incidentService.GetWithSummaryValues(base.LoginOperatorDeptId, ucMessageRightIncident.IncidentId);
		if (incident == null)
		{
			divActionButtonAera.Visible = true;
			lLockMessage.Visible = false;
			return;
		}

		// ロック状態から表示制御
		switch (incident.LockStatus)
		{
			// ロック無し
			case Constants.FLG_CSINCIDENT_LOCK_STATUS_NONE:
				divActionButtonAera.Visible = true;
				lLockMessage.Visible = false;
				break;

			// 自分のロックであれば編集できる場合
			case Constants.FLG_CSINCIDENT_LOCK_STATUS_EDIT:
			case Constants.FLG_CSINCIDENT_LOCK_STATUS_DRAFT:
				divActionButtonAera.Visible = (incident.LockOperatorId == this.LoginOperatorId);
				lLockMessage.Visible = (incident.LockOperatorId != this.LoginOperatorId);
				break;

			// 編集できない場合
			default:
				divActionButtonAera.Visible = false;
				lLockMessage.Visible = true;
				break;
		}

		if (incident.LockStatus != Constants.FLG_CSINCIDENT_LOCK_STATUS_NONE)
		{
			lLockMessage.Text = WebSanitizer.HtmlEncode(string.Format(
				// 「{0} さんにより {1} 中です。」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_MESSAGE_INPUT,
					Constants.VALUETEXT_PARAM_USER_LIST_COUNT_MAX,
					Constants.VALUETEXT_PARAM_BY_INSIDE),
				incident.EX_LockOperatorName,
				incident.EX_LockStatusName));
		}
	}
	#endregion

	#region #lbSelectMessage_Click メッセージ選択リンククリック
	/// <summary>
	/// メッセージ選択リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSelectMessage_Click(object sender, EventArgs e)
	{
		DispMessage(hfSelectedIncidentId.Value, int.Parse(hfSelectedMessageNo.Value));
	}
	#endregion

	#region -DispMessage メッセージ情報表示
	/// <summary>
	/// メッセージ情報表示
	/// </summary>
	/// <param name="incidentId">インシデントID</param>
	/// <param name="messageNo">メッセージNO</param>
	private void DispMessage(string incidentId, int messageNo)
	{
		var service = new CsMessageService(new CsMessageRepository());
		var message = service.GetWithMail(this.LoginOperatorDeptId, incidentId, messageNo);
		if (message == null) return;

		DispMessageDetailLeft(message);

		SetRightQuotation(message);
	}
	#endregion

	#region -DispMessageDetailLeft メッセージ情報表示（左）
	/// <summary>
	/// メッセージ情報表示（左）
	/// </summary>
	/// <param name="message">メッセージ</param>
	private void DispMessageDetailLeft(CsMessageModel message)
	{
		if (message == null) return;

		hfSelectedIncidentId.Value = message.IncidentId;
		hfSelectedMessageNo.Value = message.MessageNo.ToString();

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

			trMailMessageStatus.Visible = (message.EX_IsDraft || message.EX_IsRequest);
			lMailMessageStatus.Text = WebSanitizer.HtmlEncode(message.EX_MessageStatusName);
		}
		else
		{
			lMessageInquiryDateTime.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					message.InquiryReplyDate,
					DateTimeUtility.FormatType.ShortDateHourMinute2Letter));
			lMessageReplyOperatorName.Text = WebSanitizer.HtmlEncode(message.EX_ReplyOperatorName);
			lMessageUserName.Text = WebSanitizer.HtmlEncode(message.UserName1 + message.UserName2);
			lMessageUserNameKana.Text = WebSanitizer.HtmlEncode(message.UserNameKana1 + message.UserNameKana2);
			lMessageUserTel.Text = WebSanitizer.HtmlEncode(message.UserTel1);
			lMessageUserMail.Text = WebSanitizer.HtmlEncode(message.UserMailAddr);
			ucErrorPointMessage.SetMailAddr(message.UserMailAddr);
			lMessageInquiryTitle.Text = WebSanitizer.HtmlEncode(message.InquiryTitle);
			lMessageInquiryText.Text = WebSanitizer.HtmlEncodeChangeToBr(message.InquiryText);
			lMessageReplyText.Text = WebSanitizer.HtmlEncodeChangeToBr(message.ReplyText);

			trTelMessageStatus.Visible = message.EX_IsDraft;
			lTelMessageStatus.Text = WebSanitizer.HtmlEncode(message.EX_MessageStatusName);
		}
		divMessageTel.Visible = (message.MediaKbn != Constants.FLG_CSMESSAGE_MEDIA_KBN_MAIL);
		divMessageMail.Visible = (message.MediaKbn == Constants.FLG_CSMESSAGE_MEDIA_KBN_MAIL);
	}
	#endregion

	#region -SetRightQuotation 右側に引用文をセット
	/// <summary>
	/// 右側に引用文をセット
	/// </summary>
	/// <param name="message">メッセージ</param>
	private void SetRightQuotation(CsMessageModel message)
	{
		if (message == null) return;
		if (message.MediaKbn == Constants.FLG_CSMESSAGE_MEDIA_KBN_MAIL)
		{
			ucMessageRightMail.SetQuotation(message.EX_Mail.MailSubject, message.EX_Mail.MailBody);
		}
		else
		{
			ucMessageRightMail.SetQuotation("", "");
		}
	}
	#endregion

	#region #btnReflectUserFromUser_Click 顧客情報差込ボタンクリック（ユーザー情報より）
	/// <summary>
	/// 顧客情報差込ボタンクリック（ユーザー情報より）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnReflectUserFromUser_Click(object sender, EventArgs e)
	{
		ReflectRightUserFromUser(hfSelectedUserId.Value);
	}
	#endregion

	/// <summary>
	/// 送信先差込ボタンクリック（ユーザー情報より）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSendMailAddress_OnClick(object sender, EventArgs e)
	{
		SendMailAddressForRight(hfSelectedUserId.Value);
	}

	#region #btnReflectUserFromMessage_Click 顧客情報差込ボタンクリック（メッセージ情報より）
	/// <summary>
	/// 顧客情報差込ボタンクリック（メッセージ情報より）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnReflectUserFromMessage_Click(object sender, EventArgs e)
	{
		ReflectRightUserFromMessage();
	}
	#endregion

	#region -ReflectRightUserFromUser 右領域に顧客情報差し込み
	/// <summary>
	/// 右領域に顧客情報差し込み
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	private void ReflectRightUserFromUser(string userId)
	{
		// ユーザー情報から差し込み
		var user = new UserService().Get(userId);
		if (user == null) return;

		if (this.DispRightMode == DispRightMode.TelOthers)
		{
			ucMessageRightTel.SetUser(user);
		}
		else if (this.DispRightMode == DispRightMode.Mail)
		{
			ucMessageRightMail.SetUser(user);
		}

		// インシデントにユーザー情報セット
		ucMessageRightIncident.SetUser(userId);
	}
	#endregion

	#region -SendMailAddressForRight 右領域に宛先差し込み
	/// <summary>
	/// 右領域に顧客情報差し込み
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	private void SendMailAddressForRight(string userId)
	{
		// ユーザー情報から差し込み
		var user = new UserService().Get(userId);
		if (user == null) return;

		if (this.DispRightMode == DispRightMode.TelOthers)
		{
			ucMessageRightTel.SetUser(user);
		}
		else if (this.DispRightMode == DispRightMode.Mail)
		{
			ucMessageRightMail.SetMailAddress(user, this.MailAddressList);
		}
		// インシデントにユーザー情報セット
		ucMessageRightIncident.SetUser(userId);
	}
	#endregion

	#region -ReflectRightUserFromMessage メッセージから顧客情報差し込み
	/// <summary>
	/// 右領域にメッセージから顧客情報差し込み
	/// </summary>
	private void ReflectRightUserFromMessage()
	{
		// メッセージから差し込み
		CsMessageModel message = null;
		if (string.IsNullOrEmpty(hfSelectedMessageNo.Value) == false)
		{
			var service = new CsMessageService(new CsMessageRepository());
			message = service.GetWithMail(this.LoginOperatorDeptId, hfSelectedIncidentId.Value, int.Parse(hfSelectedMessageNo.Value));
		}
		if (message == null)
		{
			ReflectRightUserFromUser(hfSelectedUserId.Value);
			return;
		}

		if (this.DispRightMode == DispRightMode.TelOthers)
		{
			ucMessageRightTel.SetUser(message);
		}
		else if (this.DispRightMode == DispRightMode.Mail)
		{
			ucMessageRightMail.SetUser(message);
		}

		// インシデントにユーザー情報セット
		ucMessageRightIncident.SetUser(hfSelectedUserId.Value);
	}
	#endregion

	#region #btnReflectIncident_Click インシデント情報差込ボタンクリック
	/// <summary>
	/// インシデント情報差込ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnReflectIncident_Click(object sender, EventArgs e)
	{
		// 編集ロック
		UpdateIncidentForEditLock(hfSelectedIncidentId.Value);

		// インシデント情報差込
		ReflectIncident();

		ScriptManager.RegisterStartupScript(this, this.GetType(), "refresh_action_button", "refresh_action_button();", true);
	}
	#endregion

	#region #ReflectIncident インシデント差し込み
	/// <summary>
	/// インシデント差し込み
	/// </summary>
	private void ReflectIncident()
	{
		// インシデント差し込み
		ucMessageRightIncident.SetIncident(hfSelectedIncidentId.Value);
	}
	#endregion

	#region -ChangeLeftDispMode 左表示モード変更
	/// <summary>
	/// 左表示モード変更
	/// </summary>
	/// <param name="dispMode">表示モード</param>
	private void ChangeLeftDispMode(DispLeftMode dispMode)
	{
		this.DispLeftMode = dispMode;

		divUserSearch.Visible = (this.DispLeftMode == DispLeftMode.UserSearch);
		divUserInfo.Visible = (this.DispLeftMode == DispLeftMode.UserInfo);
		divIncident.Visible = (string.IsNullOrEmpty(hfSelectedIncidentId.Value) == false) && (this.DispLeftMode == DispLeftMode.Incident);
		divNoIncidentMessage.Visible = (string.IsNullOrEmpty(hfSelectedIncidentId.Value)) && (this.DispLeftMode == DispLeftMode.Incident);
		divAnswerTemplates.Visible = (this.DispLeftMode == DispLeftMode.AnswerTemplate);

		imgLockTop.Visible = this.IsIncidentLocked;
		ScriptManager.RegisterStartupScript(up3, up3.GetType(), "refresh_incident_info_opener", "refresh_incident_info_opener('" + hfSelectedIncidentId.Value + "', " + this.IsIncidentLocked.ToString().ToLower() + ");", true);
	}
	#endregion

	#region -DispUserInfo 顧客情報表示
	/// <summary>
	/// 顧客情報表示
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	private void DispUserInfo(string userId)
	{
		var user = new UserService().Get(userId);
		if (user == null) return;

		hfSelectedUserId.Value = userId;

		// ユーザー情報セット
		lUserName.Text = WebSanitizer.HtmlEncode(user.Name);
		lUserNameKana.Text = WebSanitizer.HtmlEncode(user.NameKana);
		lUserTel1.Text = WebSanitizer.HtmlEncode(user.Tel1);
		lUserTel2.Text = WebSanitizer.HtmlEncode(user.Tel2);
		lUserMail.Text = WebSanitizer.HtmlEncode(user.MailAddr);
		ucErrorPoint.SetMailAddr(user.MailAddr);
		lUserMail2.Text = WebSanitizer.HtmlEncode(user.MailAddr2);
		ucErrorPoint2.SetMailAddr(user.MailAddr2);
		this.CompanyName = user.CompanyName;
		this.CompanyPostName = user.CompanyPostName;

		// インシデント履歴セット
		var historyService = new UserHistoryIncidentService(new UserHistoryIncidentRepository());
		rUserIncidentHisory.DataSource = historyService.GetList(userId);
		rUserIncidentHisory.DataBind();
		trUserIncidentHisoryAlert.Visible = (rUserIncidentHisory.Items.Count == 0);
	}
	#endregion

	#region #btnRegisterTelMessage_Click 電話メッセージ登録ボタンクリック
	/// <summary>
	/// 電話メッセージ登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRegisterTelMessage_Click(object sender, EventArgs e)
	{
		// 登録
		var result = RegisterIncidentAndMessageTel(false);
		if (result == false) return;

		lCompleteMessages.Text = WebMessages.GetMessages(WebMessages.MSG_MANAGER_REGISTER_MESSAGE);

		// 開いていたら左画面更新
		if (divIncident.Visible) DispIncidentLeft(hfSelectedIncidentId.Value);
		else if (divUserInfo.Visible) DispUserInfo(hfSelectedUserId.Value);
	}
	#endregion

	#region #btnRegisterTelMessageWithCloseIncident_Click 電話メッセージ登録（インシデントクローズ）ボタンクリック
	/// <summary>
	/// 電話メッセージ登録（インシデントクローズ）ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRegisterTelMessageWithCloseIncident_Click(object sender, EventArgs e)
	{
		// クローズ指定
		ucMessageRightIncident.SetIncidentStatus(Constants.FLG_CSINCIDENT_STATUS_COMPLETE);

		// 登録
		var result = RegisterIncidentAndMessageTel(false);
		if (result == false) return;

		divActionButtonAera.Visible = false;
		lCompleteMessages.Text = WebMessages.GetMessages(WebMessages.MSG_MANAGER_REGISTER_MESSAGE_INCIDENT_CLOSED);

		// 開いていたら左画面更新
		if (divIncident.Visible)
		{
			DispIncidentLeft(hfSelectedIncidentId.Value);
		}
		else if (divUserInfo.Visible)
		{
			DispUserInfo(hfSelectedUserId.Value);
		}

		// トップページ更新・クローズ
		lCompleteMessages.Text = WebMessages.GetMessages(WebMessages.MSG_MANAGER_UPDATE_MESSAGE);
	}
	#endregion

	#region #btnSaveToTelDraft_Click 電話メッセージ下書き保存ボタンクリック
	/// <summary>
	/// 電話メッセージ下書き保存ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSaveToTelDraft_Click(object sender, EventArgs e)
	{
		// 登録
		var result = RegisterIncidentAndMessageTel(true);
		if (result == false) return;

		// ウィンドウを閉じる＆トップページ更新
		ScriptManager.RegisterStartupScript(this, this.GetType(), "CloseWindows", "setTimeout('fadeout_and_close()', 500);", true);
	}
	#endregion

	#region #btnUpdateTel_Click 電話メッセージ更新ボタンクリック
	/// <summary>
	/// 電話メッセージ更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateTel_Click(object sender, EventArgs e)
	{
		// 登録
		var result = RegisterIncidentAndMessageTel(false);
		if (result == false) return;

		lCompleteMessages.Text = "メッセージを更新しました。";

		// 開いていたら左画面更新
		if (divIncident.Visible) DispIncidentLeft(hfSelectedIncidentId.Value);
		else if (divUserInfo.Visible) DispUserInfo(hfSelectedUserId.Value);
	}
	#endregion

	#region -RegisterIncidentAndMessageTel インシデント・メッセージ登録（電話）
	/// <summary>
	/// インシデント・メッセージ登録（電話）
	/// </summary>
	/// <param name="saveAsDraft">下書き保存するか</param>
	/// <returns>結果</returns>
	private bool RegisterIncidentAndMessageTel(bool saveAsDraft)
	{
		var incidentService = new CsIncidentService(new CsIncidentRepository());

		// 各入力チェック
		var incident = ucMessageRightIncident.CheckInput(saveAsDraft);
		var message = ucMessageRightTel.CheckMessageInput(saveAsDraft ? Constants.FLG_CSMESSAGE_MESSAGE_STATUS_DRAFT : Constants.FLG_CSMESSAGE_MESSAGE_STATUS_DONE, saveAsDraft);
		if ((incident == null) || (message == null)) return false;

		// 取得したユーザー情報のメッセージ内容にタグを切り替える
		var userInfo = GetUserInfo(message);
		var errorMessage = GetReplaceEmptyTags(userInfo, message.ReplyText);
		message.ReplyText = ReplaceMessageBodyByUserInfo(userInfo, message.ReplyText);
		if (saveAsDraft == false)
		{
			SetTextMessageBody(ucMessageRightTel, message.ReplyText, errorMessage);
			if (string.IsNullOrEmpty(errorMessage) == false) return false;
		}

		// インシデント・メッセージすべて登録（上書きの場合はインシデントID、メッセージNOセット）
		if (this.RequestIncidentId != null) incident.IncidentId = this.RequestIncidentId;
		if ((this.EditMode == EditMode.New) && (this.NewRegisteredIncidentId != "")) incident.IncidentId = this.NewRegisteredIncidentId;	// 新規登録直後の更新は、上書きとする。
		if ((this.EditMode == EditMode.Reply) && (this.NewRegisteredIncidentId != "")) incident.IncidentId = this.NewRegisteredIncidentId;	// 返信登録直後の更新は、上書きとする。
		if ((this.RequestMessageNo.HasValue)
			&& ((this.EditMode == EditMode.EditDraft) || (this.EditMode == EditMode.EditDone))) message.MessageNo = this.RequestMessageNo.Value;
		if ((this.EditMode == EditMode.New) && (this.NewRegisteredIncidentId != "")) message.MessageNo = this.NewRegisteredMessageNo;		// 新規登録直後の更新は、上書きとする。
		if ((this.EditMode == EditMode.Reply) && (this.NewRegisteredIncidentId != "")) message.MessageNo = this.NewRegisteredMessageNo;		// 返信登録直後の更新は、上書きとする。
		var isIncidentNew = string.IsNullOrEmpty(incident.IncidentId);
		RegisterUpdateIncidentAndMessageAll(incident, message);
		this.NewRegisteredIncidentId = incident.IncidentId;
		this.NewRegisteredMessageNo = message.MessageNo;

		// インシデント差し込み
		ucMessageRightIncident.SetIncident(incident.IncidentId);

		// 受信なら最終受信日時更新
		if ((saveAsDraft == false) && (message.DirectionKbn == Constants.FLG_CSMESSAGE_DIRECTION_KBN_RECEIVE))
		{
			incident.DateLastReceived = DateTime.Parse((string)message.DataSource[Constants.FIELD_CSMESSAGE_INQUIRY_REPLY_DATE]);
			incidentService.UpdateLastReceivedDate(incident);
		}

		// 送信なら最終送受信日時更新
		if ((saveAsDraft == false) && (message.DirectionKbn == Constants.FLG_CSMESSAGE_DIRECTION_KBN_SEND))
		{
			incident.DateMessageLastSendReceived = DateTime.Now;
			incidentService.UpdateLastSendDate(incident);
		}

		// 下書き保存であればそれ用にロック
		if (saveAsDraft)
		{
			incidentService.UpdateLockStatusForLock(
				this.LoginOperatorDeptId,
				incident.IncidentId,
				this.LoginOperatorId,
				isIncidentNew ? Constants.FLG_CSINCIDENT_LOCK_STATUS_NONE : Constants.FLG_CSINCIDENT_LOCK_STATUS_EDIT,
				Constants.FLG_CSINCIDENT_LOCK_STATUS_DRAFT,
				this.LoginOperatorName);
		}

		return true;
	}
	#endregion

	#region #btnPreviewMail_Click 送信プレビュークリック
	/// <summary>
	/// 送信プレビュークリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnPreviewMail_Click(object sender, EventArgs e)
	{
		string messageStatus = null;
		string mailActionRequestKbn = null;
		switch (this.MailAction)
		{
			case MailActionType.MailSend:
				messageStatus = Constants.FLG_CSMESSAGE_MESSAGE_STATUS_DONE;
				mailActionRequestKbn = Constants.KBN_MAILACTION_SEND;
				break;

			case MailActionType.ApprovalRequest:
				messageStatus = Constants.FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_REQ;
				mailActionRequestKbn = Constants.KBN_MAILACTION_APPROVE_REQ;
				break;

			case MailActionType.MailSendRequest:
				messageStatus = Constants.FLG_CSMESSAGE_MESSAGE_STATUS_SEND_REQ;
				mailActionRequestKbn = Constants.KBN_MAILACTION_SEND_REQ;
				break;
		}

		var message = ucMessageRightMail.CheckMessageInput(messageStatus, false);
		if (message == null) return;

		var messageMail = ucMessageRightMail.CheckMessageMailInput(hfSelectedIncidentId.Value, false);
		var attachments = ucMessageRightMail.MailAttachemnts;
		if (messageMail == null) return;

		message.EX_Mail = messageMail;
		message.EX_Mail.EX_MailAttachments = attachments.ToArray();

		var incident = ucMessageRightIncident.CheckInput(false);
		if (incident == null) return;

		if ((this.MailAction == MailActionType.ApprovalRequest)
			|| (this.MailAction == MailActionType.MailSendRequest))
		{
			var messageRequest = CheckAndCreateRequest(this.MailAction, false);
			if (messageRequest == null) return;
			message.EX_Request = messageRequest;
		}

		// Replace tag in message body with user information
		Hashtable userInfo = GetUserInfo(incident.UserId);
		string errorMessage = GetReplaceEmptyTags(userInfo, message.EX_Mail.MailBody);
		message.EX_Mail.MailBody = ReplaceMessageBodyByUserInfo(userInfo, message.EX_Mail.MailBody);
		SetTextMessageBody(ucMessageRightMail, message.EX_Mail.MailBody, errorMessage);
		if (string.IsNullOrEmpty(errorMessage) == false) return;

		Hashtable param = new Hashtable();
		param.Add("Incident", incident);
		param.Add("Message", message);
		param.Add("BeforeUrl", Request.Url.PathAndQuery);
		param.Add(MAIL_ACTION, rblMailAction.SelectedValue);
		Session[Constants.SESSION_KEY_PARAM] = param;

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MESSAGE_MESSAGE_CONFIRM
			+ "?" + Constants.REQUEST_KEY_MAILACTION + "=" + HttpUtility.UrlEncode(mailActionRequestKbn));
	}
	#endregion

	#region #btnSaveToMailDraft_Click メール下書き保存ボタンクリック
	/// <summary>
	/// メール下書き保存ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSaveToMailDraft_Click(object sender, EventArgs e)
	{
		// 下書き登録
		var result = RegisterIncidentAndMessageMailAsDraft();
		if (result == false) return;

		// ウィンドウを閉じる＆トップページ更新
		ScriptManager.RegisterStartupScript(this, this.GetType(), "CloseWindows", "setTimeout('fadeout_and_close()', 500);", true);
	}
	#endregion 

	#region -RegisterIncidentAndMessageMailAsDraft インシデント・メッセージ下書き登録（メール）
	/// <summary>
	/// インシデント・メッセージ下書き登録（メール）
	/// </summary>
	/// <returns>成功か</returns>
	private bool RegisterIncidentAndMessageMailAsDraft()
	{
		// インシデント・メッセージ入力チェック＆取得
		var incident = ucMessageRightIncident.CheckInput(true);
		var message = ucMessageRightMail.CheckMessageInput(Constants.FLG_CSMESSAGE_MESSAGE_STATUS_DRAFT, true);
		if ((incident == null) || (message == null)) return false;

		message.InquiryReplyDate = null;	// 下書きであればnull

		// メール情報入力チェック＆取得
		var messageMail = ucMessageRightMail.CheckMessageMailInput(hfSelectedIncidentId.Value, true);
		var attachments = ucMessageRightMail.MailAttachemnts;
		if (messageMail == null) return false;

		// 依頼情報入力チェック＆取得
		if ((this.MailAction == MailActionType.ApprovalRequest)
			|| (this.MailAction == MailActionType.MailSendRequest))
		{
			var messageRequest = CheckAndCreateRequest(this.MailAction, true);
			if (messageRequest == null) return false;
			message.EX_Request = messageRequest;
		}

		// Replace tag in message body with user information
		messageMail.MailBody = ReplaceMessageBodyByUserInfo(GetUserInfo(incident.UserId), messageMail.MailBody);

		// インシデント・メッセージ・メール登録
		var isIncidentNew = string.IsNullOrEmpty(incident.IncidentId);
		RegisterUpdateIncidentAndMessageAll(
			incident,
			message,
			messageMail,
			attachments.ToArray());

		// 下書きロック
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		incidentService.UpdateLockStatusForLock(
			this.LoginOperatorDeptId,
			incident.IncidentId,
			this.LoginOperatorId,
			isIncidentNew ? Constants.FLG_CSINCIDENT_LOCK_STATUS_NONE : Constants.FLG_CSINCIDENT_LOCK_STATUS_EDIT,
			Constants.FLG_CSINCIDENT_LOCK_STATUS_DRAFT,
			this.LoginOperatorName);

		return true;
	}
	#endregion

	#region #bthUnlockAndClose_Click ロック解除して閉じるボタンクリック
	/// <summary>
	/// ロック解除して閉じるボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void bthUnlockAndClose_Click(object sender, EventArgs e)
	{
		// 登録/更新されたインシデントIDを取得
		string incidentId = 
			(this.NewRegisteredIncidentId != "")
				? this.NewRegisteredIncidentId
				: (this.RequestIncidentId ?? hfSelectedIncidentId.Value);

		// 編集ロック解除
		var incidentService = new CsIncidentService(new CsIncidentRepository());
		incidentService.UpdateLockStatusForLock(
			this.LoginOperatorDeptId,
			incidentId,
			this.LoginOperatorId,
			Constants.FLG_CSINCIDENT_LOCK_STATUS_EDIT,
			Constants.FLG_CSINCIDENT_LOCK_STATUS_NONE,
			this.LoginOperatorName);

		// ウィンドウを閉じる＆トップページ更新
		ScriptManager.RegisterStartupScript(this, this.GetType(), "CloseWindows", "setTimeout('fadeout_and_close()', 500);", true);
	}
	#endregion

	#region #btnAnswerTemplateSearch_Click 回答例検索ボタンクリック
	/// <summary>
	/// 回答例検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAnswerTemplateSearch_Click(object sender, EventArgs e)
	{
		var service = new CsAnswerTemplateService(new CsAnswerTemplateRepository());
		var models = service.SearchValid(this.LoginOperatorDeptId, ddlAnswerTemplateCategories.SelectedValue, tbAnswerTemplateSearchKeyword.Text);

		rAnswerTemplateList.DataSource = models;
		rAnswerTemplateList.DataBind();

		trAnswerTempalteResultAlert.Visible = (models.Length == 0);

		divAnswerTempalteList.Visible = true;
	}
	#endregion

	#region #lbSelectAnswerTemplate_Click 回答例クリック（差込）
	/// <summary>
	/// 回答例クリック（差込）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSelectAnswerTemplate_Click(object sender, EventArgs e)
	{
		var service = new CsAnswerTemplateService(new CsAnswerTemplateRepository());
		var model = service.Get(this.LoginOperatorDeptId, hfSelectedAnswerId.Value);
		var userInfo = new Hashtable();
		var tbMessageBody = new TextBox();
		var tbMailSubject = new TextBox();

		if (this.DispRightMode == DispRightMode.Mail)
		{
			ucMessageRightMail.SetAnswerTemplate(model.AnswerText, model.AnswerMailTitle);

			// Replace tag in message body by user information from DB
			userInfo = GetUserInfo(((TextBox)ucMessageRightIncident.FindControl("tbIncidentUserId")).Text);
			tbMessageBody = ((TextBox)ucMessageRightMail.FindControl("tbMailBody"));
			tbMailSubject = ((TextBox)ucMessageRightMail.FindControl("tbMailSubject"));
		}
		else if (this.DispRightMode == DispRightMode.TelOthers)
		{
			ucMessageRightTel.SetAnswerTemplate(model.AnswerText, model.AnswerMailTitle);

			// Replace tag in message body by user information from input data
			userInfo = GetUserInfo(new CsMessageModel(ucMessageRightTel.GetTelInput("")));
			tbMessageBody = ((TextBox)ucMessageRightTel.FindControl("tbReplyText"));
			tbMailSubject = ((TextBox)ucMessageRightTel.FindControl("tbInquiryTitle"));
		}

		tbMessageBody.Text = ReplaceMessageBodyByUserInfo(userInfo, tbMessageBody.Text);
		tbMailSubject.Text = ReplaceMessageBodyByUserInfo(userInfo, tbMailSubject.Text);
	}
	#endregion

	#region #rblMailAction_SelectedIndexChanged メールアクションラジオボタンリスト変更
	/// <summary>
	/// メールアクションラジオボタンリスト変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblMailAction_SelectedIndexChanged(object sender, EventArgs e)
	{
		InitializeRequestForm();

		if (divApprovalForm.Visible) tbApprovalRequestComment.Focus();
		if (divSendRequest.Visible) tbMailSendRequestComment.Focus();
	}
	#endregion

	#region -InitializeRequestForm 依頼フォーム初期化
	/// <summary>
	/// 依頼フォーム初期化
	/// </summary>
	private void InitializeRequestForm()
	{
		if (this.DispRightMode == DispRightMode.TelOthers) return;

		divApprovalForm.Visible = (this.MailAction == MailActionType.ApprovalRequest);
		divSendRequest.Visible = (this.MailAction == MailActionType.MailSendRequest);

		// 承認依頼？
		if (divApprovalForm.Visible)
		{
			if (this.ApprovalOperators == null)	// リストがなければ作成
			{
				var service = new CsOperatorService(new CsOperatorRepository());
				var operators = service.GetApprovalValidAll(this.LoginOperatorDeptId);

				var list = new List<KeyValuePair<string, string>>();
				list.Add(new KeyValuePair<string, string>());
				list.AddRange(operators
					.Where(op => op.OperatorId != this.LoginOperatorId)
					.Select(op => new KeyValuePair<string, string>(op.OperatorId, op.EX_ShopOperatorName)).ToArray());
				this.ApprovalOperators = list.ToArray();

				rApprovalOperators.DataSource = new string[1];	// リピータに１行表示のため要素数１の配列をセット
				rApprovalOperators.DataBind();
			}
		}
		// 送信依頼？
		else if (divSendRequest.Visible)
		{
			if (this.MailSendableOperators == null)	// リストがなければ作成
			{
				var service = new CsOperatorService(new CsOperatorRepository());
				var operators = service.GetMailSendableValidAll(this.LoginOperatorDeptId);

				var list = new List<KeyValuePair<string, string>>();
				list.Add(new KeyValuePair<string, string>());
				list.AddRange(operators
									.Where(op => op.OperatorId != this.LoginOperatorId)
									.Select(op => new KeyValuePair<string, string>(op.OperatorId, op.EX_ShopOperatorName)).ToArray());
				this.MailSendableOperators = list.ToArray();

				rMailSendableOperators.DataSource = new string[1];	// リピータに１行表示のため要素数１の配列をセット
				rMailSendableOperators.DataBind();
			}
		}

		// 送信ボタンの文言制御
		if (divApprovalForm.Visible)
		{
			// 「承認依頼（プレビュー）」
			btnPreviewMail.Text = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_MESSAGE_INPUT,
				Constants.VALUETEXT_PARAM_USER_LIST_COUNT_MAX,
				Constants.VALUETEXT_PARAM_APPROVAL_REQUEST);
		}
		else if (divSendRequest.Visible)
		{
			// 「送信依頼（プレビュー）」
			btnPreviewMail.Text = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_MESSAGE_INPUT,
				Constants.VALUETEXT_PARAM_USER_LIST_COUNT_MAX,
				Constants.VALUETEXT_PARAM_SEND_REQUEST);
		}
		else
		{
			// 「送信（プレビュー）」
			btnPreviewMail.Text = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_MESSAGE_INPUT,
				Constants.VALUETEXT_PARAM_USER_LIST_COUNT_MAX,
				Constants.VALUETEXT_PARAM_SEND_PREVIEW);
		}

	}
	#endregion

	#region #btnAddOperator_Click オペレータ追加ボタンクリック
	/// <summary>
	/// オペレータ追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddOperator_Click(object sender, EventArgs e)
	{
		AddApprovalOperator();
	}
	#endregion

	#region #btnDeleteOperator_Click オペレータ削除ボタンクリック
	/// <summary>
	/// オペレータ削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDeleteOperator_Click(object sender, EventArgs e)
	{
		DeleteApprovalOperator(int.Parse(((Button)sender).CommandArgument));
	}
	#endregion

	#region btnAddOperatorAll_Click 全オペレータ追加ボタンクリック
	/// <summary>
	/// 全オペレータ追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddOperatorAll_Click(object sender, EventArgs e)
	{
		ChangeApprovalOperators(this.ApprovalOperators.Skip(1).Select(p => p.Key).ToArray());
	}
	#endregion

	#region btnAddOperatorGroup_Click 所属グループ内オペレータ追加ボタンクリック
	/// <summary>
	/// 所属グループ内オペレータ追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddOperatorGroup_Click(object sender, EventArgs e)
	{
		CsOperatorGroupService service = new CsOperatorGroupService(new CsOperatorGroupRepository());
		List<string> operatorIdList = service.GetValidGroupOperatorIdsByOperatorId(this.LoginOperatorDeptId, this.LoginOperatorId);
		ChangeApprovalOperators(this.ApprovalOperators.Skip(1).Where(p => operatorIdList.Contains(p.Key)).Select(p => p.Key).ToArray());
	}
	#endregion

	#region -AddApprovalOperator 依頼オペレータ追加
	/// <summary>
	/// 依頼オペレータ追加
	/// </summary>
	private void AddApprovalOperator()
	{
		// 現在選択中のオペレータIDリスト取得
		var selectedOperatorIds
			= (from RepeaterItem ri in rApprovalOperators.Items
			   select ((DropDownList)ri.FindControl("ddlOperator")).SelectedValue).ToList();

		// 追加
		selectedOperatorIds.Add("");

		// 再構築
		CreateApprovalOperatorSelector(selectedOperatorIds);
	}
	#endregion

	#region -DeleteApprovalOperator 依頼オペレータ削除
	/// <summary>
	/// 依頼オペレータ削除
	/// </summary>
	/// <param name="removeIndex">削除インデックス</param>
	private void DeleteApprovalOperator(int removeIndex)
	{
		// 現在選択中のオペレータIDリスト取得
		var selectedOperatorIds
			= (from RepeaterItem ri in rApprovalOperators.Items
			   select ((DropDownList)ri.FindControl("ddlOperator")).SelectedValue).ToList();

		// 削除
		selectedOperatorIds.RemoveAt(removeIndex);

		// 再構築
		CreateApprovalOperatorSelector(selectedOperatorIds);
	}
	#endregion

	#region -ChangeApprovalOperators 依頼オペレータ変更
	/// <summary>
	/// 依頼オペレータ変更
	/// </summary>
	/// <param name="addOperatorIds">セットするオペレータIDリスト（nullの場合は終端に追加）</param>
	private void ChangeApprovalOperators(string[] addOperatorIds)
	{
		// セットするオペレータリスト作成
		var selectedOperatorIds = new List<string>((addOperatorIds.Length != 0) ? addOperatorIds : new string[] { "" });

		// 再構築
		CreateApprovalOperatorSelector(selectedOperatorIds);
	}
	#endregion

	#region -CreateApprovalOperatorSelector 承認オペレータ選択部分作成
	/// <summary>
	/// 承認オペレータ選択部分作成
	/// </summary>
	/// <param name="selectedOperatorIds">選択済みオペレータID</param>
	private void CreateApprovalOperatorSelector(List<string> selectedOperatorIds)
	{
		rApprovalOperators.DataSource = selectedOperatorIds.ToArray();
		rApprovalOperators.DataBind();
		for (int i = 0; i < rApprovalOperators.Items.Count; i++)
		{
			((DropDownList)rApprovalOperators.Items[i].FindControl("ddlOperator")).SelectedValue = selectedOperatorIds[i];
		}
	}
	#endregion

	#region -CheckAndCreateRequest 依頼チェック＆作成
	/// <summary>
	/// 依頼チェック＆作成
	/// </summary>
	/// <param name="mailActionType">メールアクション種別</param>
	/// <param name="isDraft">下書きか</param>
	/// <returns>メッセージ依頼モデル</returns>
	private CsMessageRequestModel CheckAndCreateRequest(MailActionType mailActionType, bool isDraft)
	{
		// 場合分け／値取得
		string requestStatus = "";
		string requestType = "";
		string approvalType = "";
		CheckBox cbUrgencyFlg = null;
		string comment = "";
		Repeater rOperators = null;
		switch (mailActionType)
		{
			case MailActionType.ApprovalRequest:
				requestStatus = isDraft ? Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_APPROVE_DRAFT : Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_APPROVE_REQ;
				requestType = Constants.FLG_CSMESSAGEREQUEST_REQUEST_TYPE_APPROVE;
				approvalType = rblApprovalType.SelectedValue;
				cbUrgencyFlg = cbApprovalUrgencyFlg;
				comment = tbApprovalRequestComment.Text;
				rOperators = rApprovalOperators;
				break;

			case MailActionType.MailSendRequest:
				requestStatus = isDraft ? Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_SEND_DRAFT : Constants.FLG_CSMESSAGEREQUEST_REQUEST_STATUS_SEND_REQ;
				requestType = Constants.FLG_CSMESSAGEREQUEST_REQUEST_TYPE_MAILSEND;
				approvalType = Constants.FLG_CSMESSAGEREQUEST_APPROVAL_TYPE_CONSULTATION;
				cbUrgencyFlg = cbMailSendRequestUrgencyFlg;
				comment = tbMailSendRequestComment.Text;
				rOperators = rMailSendableOperators;
				break;
		}

		// 承認依頼作成
		var messageRequest = new CsMessageRequestModel();
		messageRequest.DeptId = this.LoginOperatorDeptId;
		messageRequest.RequestOperatorId = this.LoginOperatorId;
		messageRequest.RequestStatus = requestStatus;
		messageRequest.RequestType = requestType;
		messageRequest.UrgencyFlg = cbUrgencyFlg.Checked ? Constants.FLG_CSMESSAGEREQUEST_URGENCY_URGENT : Constants.FLG_CSMESSAGEREQUEST_URGENCY_NORMAL;
		messageRequest.ApprovalType = approvalType;
		messageRequest.Comment = comment;
		messageRequest.LastChanged = this.LoginOperatorName;

		// 承認依頼アイテム作成
		messageRequest.EX_Items = new CsMessageRequestItemModel[rOperators.Items.Count];
		foreach (RepeaterItem ri in rOperators.Items)
		{
			var item = new CsMessageRequestItemModel();
			item.DeptId = this.LoginOperatorDeptId;
			item.ApprOperatorId = ((DropDownList)ri.FindControl("ddlOperator")).SelectedValue;
			item.EX_ApprOperatorName = ((DropDownList)ri.FindControl("ddlOperator")).SelectedItem.Text;
			item.LastChanged = this.LoginOperatorName;
			messageRequest.EX_Items[ri.ItemIndex] = item;
		}
		
		// チェック
		var operatorString = (mailActionType == MailActionType.ApprovalRequest)
			// 「承認者」
			? ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_MESSAGE_INPUT,
				Constants.VALUETEXT_PARAM_USER_LIST_COUNT_MAX,
				Constants.VALUETEXT_PARAM_RECOGNIZER)
			// 「送信者」
			: ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_MESSAGE_INPUT,
				Constants.VALUETEXT_PARAM_USER_LIST_COUNT_MAX,
				Constants.VALUETEXT_PARAM_SENDER);
		Label lErrorMessages = (mailActionType == MailActionType.ApprovalRequest) ? lApprRequestErrorMessages : lSendRequestErrorMessages;

		bool error = false;
		lErrorMessages.Text = "";
		if ((isDraft == false) && messageRequest.EX_Items.Any(i => i.ApprOperatorId == ""))
		{
			lErrorMessages.Text += WebSanitizer.HtmlEncodeChangeToBr(
				WebMessages.GetMessages(WebMessages.ERROR_MANAGER_IS_REQUIRED)
					.Replace("@@ 1 @@", operatorString)
					.Replace("@@ 2 @@", Environment.NewLine));
			error = true;
		}
		if (messageRequest.EX_Items.Any(i => messageRequest.EX_Items.Any(i2 => (i != i2) && (i.ApprOperatorId == i2.ApprOperatorId))))
		{
			lErrorMessages.Text += WebSanitizer.HtmlEncodeChangeToBr(
				WebMessages.GetMessages(WebMessages.ERROR_MANAGER_ARE_DUPLICATED)
					.Replace("@@ 1 @@", operatorString)
					.Replace("@@ 2 @@", Environment.NewLine));
			error = true;
		}

		if (error)
		{
			aRequestBottom.Focus();
			return null;
		}

		return messageRequest;
	}
	#endregion

	#region #lbRefreshActionButton_Click アクションボタン＆メッセージ表示リフレッシュリンクボタンクリック
	/// <summary>
	/// アクションボタン＆メッセージ表示リフレッシュリンクボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRefreshActionButton_Click(object sender, EventArgs e)
	{
		RefreshActionButtonAndockMessage();
	}
	#endregion

	#region GetUserInfo
	/// <summary>
	/// Get user info by user id
	/// </summary>
	/// <param name="userId">User ID</param>
	/// <returns>User info</returns>
	public Hashtable GetUserInfo(string userId)
	{
		var user = new UserService().Get(userId);

		if (user == null) user = new UserModel();

		Hashtable result = new Hashtable();

		result.Add(Constants.FIELD_CSMESSAGE_USER_NAME1, user.Name1);
		result.Add(Constants.FIELD_CSMESSAGE_USER_NAME2, user.Name2);
		result.Add(Constants.FIELD_CSMESSAGE_USER_NAME_KANA1, user.NameKana1);
		result.Add(Constants.FIELD_CSMESSAGE_USER_NAME_KANA2, user.NameKana2);
		result.Add(Constants.FIELD_CSMESSAGE_USER_MAIL_ADDR, user.MailAddr);
		result.Add(Constants.FIELD_CSMESSAGE_USER_TEL1, user.Tel1);
		result.Add(Constants.FIELD_USER_TEL2, user.Tel2);
		result.Add(OPERATOR_NAME, this.LoginOperatorName);
		result.Add(Constants.FIELD_USER_COMPANY_NAME, user.CompanyName);
		result.Add(Constants.FIELD_USER_COMPANY_POST_NAME, user.CompanyPostName);

		return result;
	}

	/// <summary>
	/// Get user info by message model object
	/// </summary>
	/// <param name="message">Message model object</param>
	/// <returns>User info</returns>
	public Hashtable GetUserInfo(CsMessageModel message)
	{
		if (message == null) message = new CsMessageModel();

		Hashtable result = new Hashtable();

		result.Add(Constants.FIELD_CSMESSAGE_USER_NAME1, message.UserName1);
		result.Add(Constants.FIELD_CSMESSAGE_USER_NAME2, message.UserName2);
		result.Add(Constants.FIELD_CSMESSAGE_USER_NAME_KANA1, message.UserNameKana1);
		result.Add(Constants.FIELD_CSMESSAGE_USER_NAME_KANA2, message.UserNameKana2);
		result.Add(Constants.FIELD_CSMESSAGE_USER_MAIL_ADDR, message.UserMailAddr);
		result.Add(Constants.FIELD_CSMESSAGE_USER_TEL1, message.UserTel1);
		result.Add(OPERATOR_NAME, this.LoginOperatorName);
		result.Add(Constants.FIELD_USER_COMPANY_NAME, this.CompanyName);
		result.Add(Constants.FIELD_USER_COMPANY_POST_NAME, this.CompanyPostName);

		return result;
	}
	#endregion

	#region ReplaceMessageBodyByUserInfo
	/// <summary>
	/// Replace user info in message body
	/// </summary>
	/// <param name="userInfos">user info</param>
	/// <param name="messageBody">mail body contents</param>
	/// <returns>replaced mail body</returns>
	public string ReplaceMessageBodyByUserInfo(Hashtable userInfos, string messageBody)
	{
		if (string.IsNullOrEmpty(messageBody)) return messageBody;

		foreach (DictionaryEntry item in userInfos)
		{
			// 法人OPがOFFであるにもかかわらず、企業名置換タグが利用されている場合は、空白(長さが0の文字列)を出力させてください
			if ((Constants.DISPLAY_CORPORATION_ENABLED == false)
				&& ((item.Key.ToString() == Constants.FIELD_USER_COMPANY_NAME) || (item.Key.ToString() == Constants.FIELD_USER_COMPANY_POST_NAME)))
			{
				messageBody = messageBody.Replace(string.Format(REPLACE_TAG, item.Key), string.Empty);
				continue;
			}

			if (string.IsNullOrEmpty((string)item.Value)) continue;

			messageBody = messageBody.Replace(string.Format(REPLACE_TAG, item.Key), item.Value.ToString());
		}

		return messageBody;
	}

	/// <summary>
	/// Get list of user info tags not replaced in message body
	/// </summary>
	/// <param name="userInfos">User info</param>
	/// <param name="messageBody">Message body</param>
	/// <returns>List of not replaced tags</returns>
	public string GetReplaceEmptyTags(Hashtable userInfos, string messageBody)
	{
		if (string.IsNullOrEmpty(messageBody)) return string.Empty;

		StringBuilder result = new StringBuilder();
		foreach (DictionaryEntry item in userInfos)
		{
			if ((string.IsNullOrEmpty((string)item.Value) == false)
				|| (messageBody.Contains(string.Format(REPLACE_TAG, item.Key)) == false)) continue;
			
			result.Append("<br />" + string.Format(REPLACE_TAG, item.Key));
		}

		return result.ToString();
	}
	#endregion

	#region SetTextMessageBody
	/// <summary>
	/// Set text to message body and error message
	/// </summary>
	/// <param name="userControl">User Control</param>
	/// <param name="mailBody">Message body</param>
	/// <param name="errorMessage">Error message</param>
	public void SetTextMessageBody(object userControl, string mailBody, string errorMessage)
	{
		TextBox tbMessageBody = new TextBox();
		Label lErrorMessages = new Label();
		HtmlAnchor aTitle = new HtmlAnchor();

		if (this.DispRightMode == DispRightMode.Mail)
		{
			tbMessageBody = ((TextBox) ucMessageRightMail.FindControl("tbMailBody"));
			lErrorMessages = ((Label) ucMessageRightMail.FindControl("lErrorMessages"));
			aTitle = ((HtmlAnchor) ucMessageRightMail.FindControl("aMailTitle"));
		}
		else if (this.DispRightMode == DispRightMode.TelOthers)
		{
			tbMessageBody = ((TextBox) ucMessageRightTel.FindControl("tbReplyText"));
			lErrorMessages = ((Label)ucMessageRightTel.FindControl("lErrorMessages"));
			aTitle = ((HtmlAnchor) ucMessageRightTel.FindControl("aTelTitle"));
		}

		if (errorMessage != "")
		{
			string error = string.Format("{0}\n{1}", WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MESSAGE_BODY_USER_INFO_REPLACE_NULL_ERROR), errorMessage);
			lErrorMessages.Text = string.IsNullOrEmpty(lErrorMessages.Text) ? error : string.Format("{0}\n{1}", lErrorMessages.Text, error);
			aTitle.Focus();
		}

		tbMessageBody.Text = mailBody;
	}
	#endregion

	#region プロパティ
	/// <summary>リクエストインシデントID</summary>
	private string RequestIncidentId { get { return (string)Request[Constants.REQUEST_KEY_INCIDENT_ID]; } }
	/// <summary>リクエストメッセージNO</summary>
	private int? RequestMessageNo
	{
		get
		{
			int messageNo;
			return (int.TryParse(Request[Constants.REQUEST_KEY_MESSAGE_NO], out messageNo)) ? messageNo : (int?)null;
		}
	}
	/// <summary>
	/// 登録更新モード
	/// </summary>
	protected EditMode EditMode
	{
		get
		{
			switch (Request[Constants.REQUEST_KEY_MESSAGE_EDIT_MODE])
			{
				case Constants.KBN_MESSAGE_EDIT_MODE_NEW:
					return EditMode.New;

				case Constants.KBN_MESSAGE_EDIT_MODE_REPLY:
					return EditMode.Reply;

				case Constants.KBN_MESSAGE_EDIT_MODE_EDIT_DRAFT:
					return EditMode.EditDraft;

				case Constants.KBN_MESSAGE_EDIT_MODE_EDIT_FOR_SEND:
					return EditMode.EditForSend;

				case Constants.KBN_MESSAGE_EDIT_MODE_EDIT_DONE:
					return EditMode.EditDone;
			}
			throw new Exception("無効な編集区分：" + Request[Constants.REQUEST_KEY_MESSAGE_EDIT_MODE]);
		}
	}
	/// <summary>左表示モード</summary>
	protected DispLeftMode DispLeftMode
	{
		get { return (DispLeftMode)(ViewState["DispLeftMode"] ?? DispLeftMode.UserSearch); }
		set { ViewState["DispLeftMode"] = value; }
	}
	/// <summary>右表示モード</summary>
	protected DispRightMode DispRightMode
	{
		get { return (Request[Constants.REQUEST_KEY_MESSAGE_MEDIA_MODE] == Constants.KBN_MESSAGE_MEDIA_MODE_TEL) ? DispRightMode.TelOthers : DispRightMode.Mail; }
	}
	/// <summary>承認可能オペレータリスト</summary>
	protected KeyValuePair<string, string>[] ApprovalOperators
	{
		get { return (KeyValuePair<string, string>[])ViewState["ApprovalOperators"]; }
		set { ViewState["ApprovalOperators"] = value; }
	}
	/// <summary>メール送信可能オペレータリスト</summary>
	protected KeyValuePair<string, string>[] MailSendableOperators
	{
		get { return (KeyValuePair<string, string>[])ViewState["MailSendableOperators"]; }
		set { ViewState["MailSendableOperators"] = value; }
	}
	/// <summary>インシデントロック状態か</summary>
	protected bool IsIncidentLocked
	{
		get { return (bool)(ViewState["IncidentLocked"] ?? false); }
		set { ViewState["IncidentLocked"] = value; }
	}
	/// <summary>新規登録されたインシデントのID</summary>
	protected string NewRegisteredIncidentId
	{
		get { return StringUtility.ToEmpty(ViewState["NewRegisteredIncidentId"]); }
		set { ViewState["NewRegisteredIncidentId"] = value; }
	}
	/// <summary>新規登録されたメッセージの番号</summary>
	private int NewRegisteredMessageNo
	{
		get { return (int)(ViewState["NewRegisteredMessageNo"] ?? false); }
		set { ViewState["NewRegisteredMessageNo"] = value; }
	}
	/// <summary>メールアクション</summary>
	private MailActionType MailAction
	{
		get { return (MailActionType)Enum.Parse(typeof(MailActionType), rblMailAction.SelectedValue); }
	}
	/// <summary>Company name</summary>
	private string CompanyName
	{
		get { return (string)ViewState["CompanyName"]; }
		set { ViewState["CompanyName"] = value; }
	}
	/// <summary>Company post name</summary>
	private string CompanyPostName
	{
		get { return (string)ViewState["CompanyPostName"]; }
		set { ViewState["CompanyPostName"] = value; }
	}
	/// <summary>User search message input</summary>
	private DataTable UserSearchMessageInput
	{
		get { return (DataTable)ViewState["UserSearchMessageInput"]; }
		set { ViewState["UserSearchMessageInput"] = value; }
	}
	/// <summary>Sort user message input kbn</summary>
	protected SortUserMessageInputKbn SortKbnCurrent
	{
		get { return (SortUserMessageInputKbn)ViewState["sortKbnCurrent"]; }
		set { ViewState["sortKbnCurrent"] = value; }
	}
	/// <summary>Sort order status</summary>
	protected string SortType
	{
		get { return (string)ViewState["SortType"]; }
		set { ViewState["SortType"] = value; }
	}
	/// <summary>
	/// メールアドレスリスト
	/// </summary>
	protected List<string> MailAddressList
	{
		get { return (List<string>)ViewState["MailAddressList"]; }
		set { ViewState["MailAddressList"] = value; }
	}
	#endregion

	#region SortUserInMessageInput
	/// <summary>
	/// Sort user search action
	/// </summary>
	/// <param name="sender">sender</param>
	/// <param name="e">event</param>
	protected void lbMessageInputSort_Click(object sender, EventArgs e)
	{
		var sortKbn = (SortUserMessageInputKbn)Enum.Parse(typeof(SortUserMessageInputKbn), ((LinkButton)sender).CommandArgument);
		this.SortType = ((this.SortKbnCurrent == sortKbn) && (this.SortType == Constants.FLG_SORT_KBN_ASC)) ? Constants.FLG_SORT_KBN_DESC : Constants.FLG_SORT_KBN_ASC;
		this.SortKbnCurrent = sortKbn;
		var userSearch = new DataView(this.UserSearchMessageInput);
		RefreshCurrentSortSymbols();

		switch (this.SortKbnCurrent)
		{
			case SortUserMessageInputKbn.UserName:
				SortAction(userSearch, this.SortType, Constants.FIELD_USER_NAME_KANA, this.lUserNameIconSort, this.rUserSearchResult);
				break;

			case SortUserMessageInputKbn.CompanyName:
				SortAction(userSearch, this.SortType, Constants.FIELD_USER_COMPANY_NAME, this.lCompanyNameIconSort, this.rUserSearchResult);
				break;

			case SortUserMessageInputKbn.CompanyPostName:
				SortAction(userSearch, this.SortType, Constants.FIELD_USER_COMPANY_POST_NAME, this.lCompanyPostNameIconSort, this.rUserSearchResult);
				break;

			case SortUserMessageInputKbn.MailAddress:
				SortAction(userSearch, this.SortType, string.Format("{0}_{1}", Constants.FIELD_USER_MAIL_ADDR, Constants.FIELD_USER_MAIL_ADDR2), this.lMailAddressIconSort, this.rUserSearchResult);
				break;

			default:
				SortAction(userSearch, this.SortType, Constants.FIELD_USER_USER_ID, this.lUserIdIconSort, this.rUserSearchResult);
				break;
		}
	}

	/// <summary>
	/// Refresh current sort symbols
	/// </summary>
	private void RefreshCurrentSortSymbols()
	{
		this.lUserIdIconSort.Text
			= this.lMailAddressIconSort.Text
			= this.lUserNameIconSort.Text
			= this.lCompanyNameIconSort.Text
			= this.lCompanyPostNameIconSort.Text = string.Empty;
	}

	/// <summary>
	/// Set default sort
	/// </summary>
	private void SetDefaultSort()
	{
		RefreshCurrentSortSymbols();
		this.SortKbnCurrent = SortUserMessageInputKbn.UserId;
		this.SortType = Constants.FLG_SORT_KBN_ASC;
		this.lUserIdIconSort.Text = Constants.FLG_SORT_SYMBOL_ASC;
	}
	#endregion
}

#region 列挙体
/// <summary>編集モード</summary>
public enum EditMode
{
	/// <summary>新規作成</summary>
	New,
	/// <summary>メール返信</summary>
	Reply,
	/// <summary>下書き編集</summary>
	EditDraft,
	/// <summary>代理送信編集</summary>
	EditForSend,
	/// <summary>完了メッセージ編集</summary>
	EditDone
}
/// <summary>左表示モード</summary>
public enum DispLeftMode
{
	/// <summary>顧客検索</summary>
	UserSearch,
	/// <summary>顧客情報</summary>
	UserInfo,
	/// <summary>インシデント</summary>
	Incident,
	/// <summary>回答例</summary>
	AnswerTemplate
}
/// <summary>右表示モード</summary>
public enum DispRightMode
{
	/// <summary>電話・その他</summary>
	TelOthers,
	/// <summary>メール</summary>
	Mail
}
#endregion
