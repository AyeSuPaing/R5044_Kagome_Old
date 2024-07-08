/*
=========================================================================================================
  Module      : メッセージページメールフォーム出力コントローラ処理(MessageRightMail.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Cs.MailFrom;
using w2.App.Common.Cs.MailSignature;
using w2.App.Common.Cs.Message;
using w2.Common.Net.Mail;
using w2.Domain.User;

public partial class Form_Message_MessageRightMail : BaseUserControl
{
	#region #Page_Init ページ初期化
	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Init(object sender, EventArgs e)
	{
		// PageのPage_Loadでドロップダウンの初期値をセットしたいため
		// Page_Initで初期化する。
		if (!IsPostBack)
		{
			Initialize();
		}
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
	}
	#endregion

	#region -Initialize 初期化
	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		// 送信元プルダウン
		CsMailFromService service = new CsMailFromService(new CsMailFromRepository());
		CsMailFromModel[] models = service.GetValidAll(this.LoginOperatorDeptId);
		if (models.Length == 0)
		{
			ddlMailFrom.Items.Add(new ListItem(
				// 「※利用可能な送信元が存在しません」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_MESSAGE_RIGHT_MAIL,
					Constants.VALUETEXT_PARAM_MAIL_FROM,
					Constants.VALUETEXT_PARAM_SOURCE_UNAVAILABLE),
				string.Empty));
		}
		else
		{
			ddlMailFrom.Items.Add(new ListItem("", ""));
			ddlMailFrom.Items.AddRange(models.Select(model => new ListItem(model.EX_DisplayAddress, model.MailFromId)).ToArray());
			CsOperatorService operatorService = new CsOperatorService(new CsOperatorRepository());
			string defaultId = operatorService.Get(this.LoginOperatorDeptId, this.LoginOperatorId).MailFromId;
			if (ddlMailFrom.Items.FindByValue(defaultId) != null) { ddlMailFrom.SelectedValue = defaultId; };
		}
	}
	#endregion

	#region +SetQuotation 引用文セット
	/// <summary>
	/// 引用文セット
	/// </summary>
	/// <param name="subject">件名</param>
	/// <param name="body">本文</param>
	public void SetQuotation(string subject, string body)
	{
		hfReplySubject.Value = subject;
		hfReplyBody.Value = body;

		btnSetQuotation.Enabled = true;
	}
	#endregion

	#region #brn_Click 引用ボタンクリック
	/// <summary>
	/// 引用ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSetQuotation_Click(object sender, EventArgs e)
	{
		InsertSubjectRe();

		InsertQuotationBody();
	}
	#endregion

	#region +InsertQuotationBody 引用文(本文）挿入
	/// <summary>
	/// 引用文(本文）挿入
	/// </summary>
	public void InsertQuotationBody()
	{
		if (hfReplyBody.Value == "") return;

		string[] lines = hfReplyBody.Value.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(
			line => "> " + line).ToArray();

		tbMailBody.Text =
			tbMailBody.Text
			+ "\r\n\r\n\r\n"
			+ "-- Original Message --" + "\r\n"
			+ string.Join("\r\n", lines) + "\r\n";
	}
	#endregion

	#region +InsertSubjectRe Re件名挿入
	/// <summary>
	/// Re件名挿入
	/// </summary>
	public void InsertSubjectRe()
	{
		if (hfReplySubject.Value == "") return;
		if (tbMailSubject.Text == "")
		{
			if ((hfReplySubject.Value.Trim().ToLower().StartsWith("re:") == false)) tbMailSubject.Text = "Re:";

			tbMailSubject.Text += hfReplySubject.Value;
		}
	}
	#endregion

	#region +SetMessageInfoForEdit 編集向けメッセージ情報セット
	/// <summary>
	/// 編集向けメッセージ情報セット
	/// </summary>
	/// <param name="message">メッセージ</param>
	public void SetMessageInfoForEdit(CsMessageModel message)
	{
		if (message.EX_Mail == null) return;

		this.IncidentIdForEdit = message.IncidentId;
		this.MessageNoForEdit = message.MessageNo;
		this.OperatorIdForEdit = message.OperatorId;
		this.MailIdForEdit = message.EX_Mail.MailId;
		tbMailTo.Text = message.EX_Mail.MailTo;
		tbMailCc.Text = message.EX_Mail.MailCc;
		tbMailBcc.Text = message.EX_Mail.MailBcc;
		foreach (ListItem li in ddlMailFrom.Items)
		{
			li.Selected = (li.Text == message.EX_Mail.MailFrom);
		}
		tbMailSubject.Text = message.EX_Mail.MailSubject;
		tbMailBody.Text = message.EX_Mail.MailBody;

		this.MailAttachemnts = message.EX_Mail.EX_MailAttachments.ToList();
		rAttachmentFiles.DataSource = this.MailAttachemnts;
		rAttachmentFiles.DataBind();
	}
	#endregion

	#region 追加ボタンクリック時
	/// <summary>
	/// 追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddUserMailAddress_OnClick(object sender, EventArgs e)
	{
		this.UserMailAddressList = CreateMailAddressInput();
		this.UserMailAddressList.Add("");
		rMailAddress.DataSource = this.UserMailAddressList;
		rMailAddress.DataBind();
	}
	#endregion

	#region #btnClear_Click 顧客情報差し込み（メッセージ情報より）
	/// <summary>
	/// クリアボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnClear_Click(object sender, EventArgs e)
	{
		ResetMailBodyInput();
	}
	#endregion

	#region -ResetMailBodyInput メール本文入力情報リセット
	/// <summary>
	/// メール本文入力情報リセット
	/// </summary>
	private void ResetMailBodyInput()
	{
		tbMailBody.Text = "";

		DeleteAttachmentTempFileAll();
	}
	#endregion

	#region +SetUser 顧客情報差し込み（ユーザー情報より）
	/// <summary>
	/// 顧客情報差し込み
	/// </summary>
	/// <param name="user">ユーザー</param>
	public void SetUser(UserModel user)
	{
		tbMailTo.Text = (user.MailAddr + " " + user.MailAddr2).Trim().Split(' ')[0];
	}
	#endregion

	#region +SetUser 顧客情報差し込み（ユーザー情報より）
	/// <summary>
	/// 送信先差込
	/// </summary>
	/// <param name="user">ユーザー</param>
	/// <param name="userList">リスト</param>
	public void SetMailAddress(UserModel user, List<string> userList)
	{
		this.UserMailAddressList = CreateMailAddressInput();
		lAlreadyUser.Text = "";
		if (this.UserMailAddressList.Any(mailAddress => (mailAddress == user.MailAddr)))
		{
			lAlreadyUser.Text = "既に差し込み済です";
			return;
		}
		if (string.IsNullOrEmpty(user.MailAddr))
		{
			lAlreadyUser.Text = "メールアドレスが空です";
			return;
		}
		this.UserMailAddressList.Add(user.MailAddr);
		rMailAddress.DataSource = this.UserMailAddressList;
		rMailAddress.DataBind();
	}
	#endregion

	#region MyRegion
	/// <summary>
	/// 顧客情報差し込み（メッセージ情報より）
	/// </summary>
	/// <param name="message">メッセージ</param>
	public void SetUser(CsMessageModel message)
	{
		var oldMailFromString = (message.EX_Mail != null)
			? MailAddress.GetInstance(message.EX_Mail.MailFrom).AddressRaw
			: string.Empty;
		var oldMailToStrings = (message.EX_Mail != null)
			? message.EX_Mail.MailTo.Split(',').Select(m => m.Trim()).Where(m => string.IsNullOrEmpty(m) == false)
				.Select(m => MailAddress.GetInstance(m).AddressRaw).ToArray()
			: new string[0];
		var oldMailCCStrings = (message.EX_Mail != null)
			? message.EX_Mail.MailCc.Split(',').Select(m => m.Trim()).Where(m => string.IsNullOrEmpty(m) == false)
				.Select(m => MailAddress.GetInstance(m).AddressRaw).ToArray()
			: new string[0];

		// 送信メールに対しての返信は、TO、CCは元メールのものを指定
		if (string.IsNullOrEmpty(message.SendMailId) == false)
		{
			tbMailTo.Text = string.Join(", ", oldMailToStrings);
			tbMailCc.Text = string.Join(", ", oldMailCCStrings);
		}
		// 受信メールに対しての返信は、以下のルールに従う
		else if (string.IsNullOrEmpty(message.ReceiveMailId) == false)
		{
			// オペレータのデフォルトメールアドレスIDの取得
			var operatorService = new CsOperatorService(new CsOperatorRepository());
			var defaultMailId = operatorService.Get(this.LoginOperatorDeptId, this.LoginOperatorId).MailFromId;

			// デフォルトメールアドレスの取得
			var csMailFromModel = new CsMailFromService(new CsMailFromRepository()).Get(this.LoginOperatorDeptId, defaultMailId);
			var defaultMailAddress = (csMailFromModel == null) ? null : csMailFromModel.MailAddress;

			// 元メールのFromアドレスがデフォルトメールアドレスと一致していたら、元メールのTOをそのままセット
			// 一致していない場合はFromアドレスに返信
			var newMailTos = ((oldMailFromString == defaultMailAddress) && (defaultMailAddress != null))
				? oldMailToStrings.ToArray()
				: new[] { oldMailFromString };

			// 含まれていない送信先があればCCに追加
			// 送信元と同じメールアドレスはCCに追加しない
			var newMailCCs = new List<string>();
			var service = new CsMailFromService(new CsMailFromRepository());
			var csMailForm = service.Get(this.LoginOperatorDeptId, ddlMailFrom.Text);
			newMailCCs.AddRange(oldMailToStrings.Where(mto => ((newMailTos.Contains(mto) == false)
					&& (newMailCCs.Contains(mto) == false))
				&& ((csMailForm == null) || (mto.Contains(csMailForm.MailAddress) == false))));
			newMailCCs.AddRange(oldMailCCStrings.Where(mcc => ((newMailTos.Contains(mcc) == false)
					&& (newMailCCs.Contains(mcc) == false))
				&& ((csMailForm == null) || (mcc.Contains(csMailForm.MailAddress) == false))));

			tbMailTo.Text = string.Join(", ", newMailTos);
			tbMailCc.Text = string.Join(", ", newMailCCs);
		}
		// 送信でも受信でもない場合はユーザーのメールアドレス宛に送信
		else if (string.IsNullOrEmpty(message.UserMailAddr) == false)
		{
			tbMailTo.Text = message.UserMailAddr;
		}
	}
	#endregion

	#region +CheckMessageMailInput メッセージメール情報入力チェック
	/// <summary>
	/// メッセージメール情報入力チェック
	/// </summary>
	/// <param name="incidentId">インシデントID</param>
	/// <param name="saveAsDraft">下書きか（下書きの場合は必須チェックを実施しない）</param>
	/// <returns>メッセージメールモデル（添付ファイル以外）</returns>
	public CsMessageMailModel CheckMessageMailInput(string incidentId, bool saveAsDraft)
	{
		if (trBcc.Visible == false)
		{
			tbMailBcc.Text = "";
		}
		Hashtable input = GetMailInput();

		var inputForCheck = (Hashtable)input.Clone();
		if (saveAsDraft) inputForCheck.Keys.Cast<string>().Where(key => StringUtility.ToEmpty(inputForCheck[key]) == "").ToList().ForEach(key => inputForCheck[key] = null);

		var error = Validator.Validate("CsMessageMail", inputForCheck);
		error += BasePageCs.CheckMailAddrInputs(tbMailTo.Text);
		error += BasePageCs.CheckMailAddrInputs(tbMailCc.Text);
		error += BasePageCs.CheckMailAddrInputs(tbMailBcc.Text);

		if (saveAsDraft == false) error += BasePageCs.CheckMailTransmissionDisabledString(tbMailBody.Text);

		foreach (RepeaterItem mailAddress in rMailAddress.Items)
		{
			error += BasePageCs.CheckMailAddrInputs(((TextBox)mailAddress.FindControl("tbMailAddress")).Text);
		}
		error = error.Trim();
		lErrorMessages.Text = error;

		if (string.IsNullOrEmpty(error) == false)
		{
			aMailTitle.Focus();
			return null;
		}

		var mail = GetSendMailWithoutAttachment(incidentId);

		return mail;
	}
	#endregion

	#region +GetSendMailModel 送信メールモデル取得
	/// <summary>
	/// 送信メールモデル取得
	/// </summary>
	/// <param name="incidentId">インシデントID（最後のメールからIn-Reply-To作成用）</param>
	/// <returns>メールモデル</returns>
	public CsMessageMailModel GetSendMailWithoutAttachment(string incidentId)
	{
		// 最終メール取得（メッセージID取得用）
		var messageService = new CsMessageService(new CsMessageRepository());
		var lastMail = messageService.GetLastMail(this.LoginOperatorDeptId, incidentId);

		// メールモデル作成
		var mailService = new CsMessageMailService(new CsMessageMailRepository());
		var sendMail = new CsMessageMailModel(GetMailInput());
		sendMail.MessageId = MessageIdGenerator.Generate();
		if (lastMail != null)
		{
			sendMail.InReplyTo = lastMail.MessageId;
			sendMail.EX_References = (string.IsNullOrEmpty(lastMail.InReplyTo))
				? new string[] { lastMail.MessageId } : new string[] { lastMail.InReplyTo, lastMail.MessageId };
		}
		return sendMail;
	}
	#endregion

	#region +CheckMessageInput メッセージ情報入力チェック
	/// <summary>
	/// メッセージ情報入力チェック
	/// </summary>
	/// <param name="messageStatus">メッセージステータス</param>
	/// <param name="saveAsDraft">下書きか（下書きの場合は必須チェックを実施しない）</param>
	/// <returns>メッセージモデル</returns>
	public CsMessageModel CheckMessageInput(string messageStatus, bool saveAsDraft)
	{
		Hashtable ht = new Hashtable();
		ht.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, this.LoginOperatorDeptId);
		ht.Add(Constants.FIELD_CSMESSAGE_INCIDENT_ID, this.IncidentIdForEdit);
		ht.Add(Constants.FIELD_CSMESSAGE_MESSAGE_NO, this.MessageNoForEdit);
		ht.Add(Constants.FIELD_CSMESSAGE_MEDIA_KBN, Constants.FLG_CSMESSAGE_MEDIA_KBN_MAIL);
		ht.Add(Constants.FIELD_CSMESSAGE_DIRECTION_KBN, Constants.FLG_CSMESSAGE_DIRECTION_KBN_SEND);
		ht.Add(Constants.FIELD_CSMESSAGE_OPERATOR_ID, this.OperatorIdForEdit ?? this.LoginOperatorId);
		ht.Add(Constants.FIELD_CSMESSAGE_MESSAGE_STATUS, messageStatus);
		ht.Add(Constants.FIELD_CSMESSAGE_INQUIRY_REPLY_DATE, null);
		ht.Add(Constants.FIELD_CSMESSAGE_USER_NAME1, "");
		ht.Add(Constants.FIELD_CSMESSAGE_USER_NAME2, "");
		ht.Add(Constants.FIELD_CSMESSAGE_USER_NAME_KANA1, "");
		ht.Add(Constants.FIELD_CSMESSAGE_USER_NAME_KANA2, "");
		ht.Add(Constants.FIELD_CSMESSAGE_USER_TEL1, "");
		ht.Add(Constants.FIELD_CSMESSAGE_USER_MAIL_ADDR, "");
		ht.Add(Constants.FIELD_CSMESSAGE_INQUIRY_TITLE, tbMailSubject.Text);
		ht.Add(Constants.FIELD_CSMESSAGE_REPLY_OPERATOR_ID, this.LoginOperatorId);
		ht.Add(Constants.FIELD_CSMESSAGE_RECEIVE_MAIL_ID, "");
		ht.Add(Constants.FIELD_CSMESSAGE_VALID_FLG, Constants.FLG_CSMESSAGE_VALID_FLG_VALID);
		ht.Add(Constants.FIELD_CSMESSAGE_LAST_CHANGED, this.LoginOperatorName);

		var htForCheck = (Hashtable)ht.Clone();
		if (saveAsDraft) htForCheck.Keys.Cast<string>().Where(key => StringUtility.ToEmpty(htForCheck[key]) == "").ToList().ForEach(key => htForCheck[key] = null);

		var error = Validator.Validate("CsMessage", htForCheck);
		lErrorMessages.Text = error;
		if (string.IsNullOrEmpty(error) == false)
		{
			aMailTitle.Focus();
			return null;
		}

		var message = new CsMessageModel(ht);
		message.InquiryText = "";
		message.ReplyText = "";

		return message;
	}
	#endregion

	#region -GetMailInput メール入力情報取得
	/// <summary>
	/// メール入力情報取得
	/// </summary>
	/// <returns>メール入力情報</returns>
	private Hashtable GetMailInput()
	{
		var ht = new Hashtable();
		var toUser = "";
		var ccUser = "";
		var bccUser = "";
		toUser += ((TextBox)tbMailTo.FindControl("tbMailTo")).Text;
		ccUser += ((TextBox)tbMailTo.FindControl("tbMailCc")).Text;
		bccUser += ((TextBox)tbMailTo.FindControl("tbMailBcc")).Text;
		foreach (RepeaterItem destination in rMailAddress.Items)
		{
			var addrUser = ((DropDownList)destination.FindControl("Addr")).SelectedValue;
			var mailAddresses = ((TextBox)destination.FindControl("tbMailAddress")).Text;
			if (string.IsNullOrEmpty(mailAddresses)) continue;

			if ("To" == addrUser)
			{
				if (string.IsNullOrEmpty(toUser) == false) toUser += ",";
				toUser += mailAddresses;
			}
			if ("Cc" == addrUser)
			{
				if (string.IsNullOrEmpty(ccUser) == false) ccUser += ",";
				ccUser += mailAddresses;
			}
			if ("Bcc" == addrUser)
			{
				if (string.IsNullOrEmpty(bccUser) == false) bccUser += ",";
				bccUser += mailAddresses;
			}
		}
		ht.Add(Constants.FIELD_CSMESSAGEMAIL_MAIL_TO, toUser);
		ht.Add(Constants.FIELD_CSMESSAGEMAIL_MAIL_CC, ccUser);
		ht.Add(Constants.FIELD_CSMESSAGEMAIL_MAIL_BCC, bccUser);
		ht.Add(Constants.FIELD_CSMESSAGEMAIL_DEPT_ID, this.LoginOperatorDeptId);
		ht.Add(Constants.FIELD_CSMESSAGEMAIL_MAIL_ID, this.MailIdForEdit);
		ht.Add(Constants.FIELD_CSMESSAGEMAIL_MAIL_KBN, Constants.FLG_CSMESSAGEMAIL_MAIL_KBN_SEND);
		ht.Add(Constants.FIELD_CSMESSAGEMAIL_MAIL_FROM, ddlMailFrom.SelectedValue == "" ? "" : ddlMailFrom.SelectedItem.Text);
		ht.Add(Constants.FIELD_CSMESSAGEMAIL_MAIL_SUBJECT, tbMailSubject.Text);
		ht.Add(Constants.FIELD_CSMESSAGEMAIL_MAIL_BODY, tbMailBody.Text);
		ht.Add(Constants.FIELD_CSMESSAGEMAIL_SEND_OPERATOR_ID, this.LoginOperatorId);
		ht.Add(Constants.FIELD_CSMESSAGEMAIL_SEND_DATETIME, null);
		ht.Add(Constants.FIELD_CSMESSAGEMAIL_MESSAGE_ID, "");	// ダミー。後で上書きされる。
		ht.Add(Constants.FIELD_CSMESSAGEMAIL_IN_REPLY_TO, "");	// ダミー。後で上書きされる。
		ht.Add(Constants.FIELD_CSMESSAGEMAIL_LAST_CHANGED, this.LoginOperatorName);

		return ht;
	}
	#endregion

	#region +SetAnswerTemplate 回答例セット
	/// <summary>
	/// 回答例セット
	/// </summary>
	/// <param name="text">文章</param>
	/// <param name="title">タイトル</param>
	public void SetAnswerTemplate(string text, string title)
	{
		tbMailBody.Text = text + "\r\n\r\n" + tbMailBody.Text;
		tbMailSubject.Text = (string.IsNullOrEmpty(title) == false)
			? title
			: tbMailSubject.Text;
	}
	#endregion

	#region #lbSetAttachmentFile_Click 添付ファイル登録リンクボタンクリック（Javascirptで呼ばれる）
	/// <summary>
	/// 添付ファイル登録リンクボタンクリック（Javascirptで呼ばれる）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSetAttachmentFile_Click(object sender, EventArgs e)
	{
		var attachment = new CsMessageMailAttachmentModel();
		attachment.FileName = hfAttachmentFileNameForSet.Value;
		attachment.FileNo = int.Parse(hfAttachmentFileNoForSet.Value);

		this.MailAttachemnts.Add(attachment);

		rAttachmentFiles.DataSource = this.MailAttachemnts;
		rAttachmentFiles.DataBind();
	}
	#endregion

	#region #btnDeleteAttachment_Click 添付ファイル削除ボタンクリック
	/// <summary>
	/// 添付ファイル削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDeleteAttachment_Click(object sender, EventArgs e)
	{
		int index = int.Parse(((Button)sender).CommandArgument);

		DeleteAttachmentTempFile(index);

		rAttachmentFiles.DataSource = this.MailAttachemnts;
		rAttachmentFiles.DataBind();
	}
	#endregion

	#region -DeleteAttachmentTempFileAll 添付ファイルテンポラリ全て削除
	/// <summary>
	/// 添付ファイルテンポラリ全て削除
	/// </summary>
	private void DeleteAttachmentTempFileAll()
	{
		for (int i = this.MailAttachemnts.Count - 1; i >= 0; i--)
		{
			DeleteAttachmentTempFile(i);
		}
	}
	#endregion
	#region -DeleteAttachmentTempFile 添付ファイルテンポラリ削除
	/// <summary>
	/// 添付ファイルテンポラリ削除
	/// </summary>
	/// <param name="index">削除するファイルのインデックス</param>
	private void DeleteAttachmentTempFile(int index)
	{
		var service = new CsMessageMailAttachmentService(new CsMessageMailAttachmentRepository());
		service.Delete(this.LoginOperatorDeptId, this.MailAttachemnts[index].MailId, this.MailAttachemnts[index].FileNo);

		this.MailAttachemnts.RemoveAt(index);
	}
	#endregion

	#region #lSetMailSignature_Click メール署名セット
	/// <summary>
	/// メール署名セット
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lSetMailSignature_Click(object sender, EventArgs e)
	{
		var service = new CsMailSignatureService(new CsMailSignatureRepository());
		var model = service.Get(this.LoginOperatorDeptId, hfMailSignatureId.Value, this.LoginOperatorId);
		if (model == null) return;

		switch (Constants.SETTING_MAILINPUT_MAIL_SIGNATURE_INSERT_MODE)
		{
			case Constants.MailSignatureInsertModeType.Top:
				tbMailBody.Text = model.SignatureText + "\r\n\r\n" + tbMailBody.Text;
				break;

			case Constants.MailSignatureInsertModeType.Bottom:
				tbMailBody.Text = tbMailBody.Text + "\r\n\r\n" + model.SignatureText;
				break;

			default:
				throw new Exception("未対応のメール署名挿入モード：" + Constants.SETTING_MAILINPUT_MAIL_SIGNATURE_INSERT_MODE.ToString());
		}
	}
	#endregion

	#region メールアドレスを追加
	/// <summary>
	/// メールアドレスを追加
	/// </summary>
	/// <returns></returns>
	private List<string> CreateMailAddressInput()
	{
		var items = rMailAddress.Items.Cast<RepeaterItem>().Select(addr => ((TextBox)addr.FindControl("tbMailAddress")).Text).ToList();
		return items;
	}
	#endregion

	#region 追加アドレス削除
	/// <summary>
	/// 追加アドレス削除
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rUserMailAddress_OnItemCommand(object source, RepeaterCommandEventArgs e)
	{
		this.UserMailAddressList = CreateMailAddressInput();
		var mailAddress = int.Parse(e.CommandArgument.ToString());
		this.UserMailAddressList.RemoveAt(mailAddress);
		rMailAddress.DataSource = this.UserMailAddressList;
		rMailAddress.DataBind();
	}
	#endregion

	#region プロパティ
	/// <summary>インシデントID（編集用）</summary>
	private string IncidentIdForEdit
	{
		get { return (string)ViewState["IncidentIdForEdit"]; }
		set { ViewState["IncidentIdForEdit"] = value; }
	}
	/// <summary>メッセージNO（編集用）</summary>
	private int MessageNoForEdit
	{
		get { return (int)(ViewState["MessageNoForEdit"] ?? 0); }
		set { ViewState["MessageNoForEdit"] = value; }
	}
	/// <summary>担当オペレータID（編集用）</summary>
	private string OperatorIdForEdit
	{
		get { return (string)ViewState["OperatorIdForEdit"]; }
		set { ViewState["OperatorIdForEdit"] = value; }
	}
	/// <summary>メールID（編集用）</summary>
	private string MailIdForEdit
	{
		get { return (string)ViewState["MailIdForEdit"]; }
		set { ViewState["MailIdForEdit"] = value; }
	}
	/// <summary>メールアドレスの追加</summary>
	protected List<string> UserMailAddressList
	{
		get { return (List<string>)ViewState["UserMailAddressList"]; }
		set { ViewState["UserMailAddressList"] = value; }
	}
	/// <summary>添付ファイル（ファイル名、添付ファイルパス）</summary>
	public List<CsMessageMailAttachmentModel> MailAttachemnts
	{
		get
		{
			if (ViewState["AttachmentFiles"] == null) ViewState["AttachmentFiles"] = new List<CsMessageMailAttachmentModel>();
			return (List<CsMessageMailAttachmentModel>)ViewState["AttachmentFiles"];
		}
		private set { ViewState["AttachmentFiles"] = value; }
	}
	#endregion
}
