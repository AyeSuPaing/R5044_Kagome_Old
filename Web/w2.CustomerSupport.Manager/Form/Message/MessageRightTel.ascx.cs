/*
=========================================================================================================
  Module      : メッセージページ電話フォーム出力コントローラ処理(MessageRightTel.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.Message;
using w2.Domain.User;

public partial class Form_Message_MessageRightTel : IncidentUserControl
{
	#region #Page_Init ページ初期化
	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Init(object sender, EventArgs e)
	{
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
		// 媒体ラジオボタン作成
		new string[] { Constants.FLG_CSMESSAGE_MEDIA_KBN_TEL, Constants.FLG_CSMESSAGE_MEDIA_KBN_OTHERS}.ToList().ForEach(kbn =>
			rblMediaKbn.Items.Add(new ListItem(ValueText.GetValueText(Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_MEDIA_KBN, kbn),  kbn)));
		rblMediaKbn.Items[0].Selected = true;

		// 受信/発信ラジオボタン作成
		ValueText.GetValueItemArray(Constants.TABLE_CSMESSAGE, Constants.FIELD_CSMESSAGE_DIRECTION_KBN).ToList().ForEach(item =>
			rblDirectionKbn.Items.Add(item));
		rblDirectionKbn.Items[0].Selected = true;

		// 現在日時セット
		SetMessageInquiryDateTime();

		// 回答者セット
		ddlReplyOperators.Items.Add("");
		ddlReplyOperators.Items.AddRange(CreateCsOperatorItems(""));
		foreach (ListItem li in ddlReplyOperators.Items)
		{
			li.Selected = (li.Value == this.LoginOperatorId);
		}
		// 作成者
		lUpdateOperator.Text = WebSanitizer.HtmlEncode(this.LoginOperatorName);
	}
	#endregion

	#region +SetUser ユーザー情報差込（ユーザーより）
	/// <summary>
	/// ユーザー情報差込（ユーザーより）
	/// </summary>
	/// <param name="user">ユーザー</param>
	public void SetUser(UserModel user)
	{
		tbUserName1.Text = user.Name1;
		tbUserName2.Text = user.Name2;
		tbUserNameKana1.Text = user.NameKana1;
		tbUserNameKana2.Text = user.NameKana2;
		tbUserTel1.Text = user.Tel1;
		tbUserMail.Text = (user.MailAddr + " " + user.MailAddr2).Trim().Split(' ')[0];
	}
	#endregion
	#region +SetUser 顧客情報差し込み（メッセージ情報より）
	/// <summary>
	/// 顧客情報差し込み（メッセージ情報より）
	/// </summary>
	/// <param name="message">メッセージ</param>
	public void SetUser(CsMessageModel message)
	{
		if (message.MediaKbn != Constants.FLG_CSMESSAGE_MEDIA_KBN_MAIL)
		{
			tbUserName1.Text = message.UserName1;
			tbUserName2.Text = message.UserName2;
			tbUserNameKana1.Text = message.UserNameKana1;
			tbUserNameKana2.Text = message.UserNameKana2;
			tbUserTel1.Text =message.UserTel1;
			tbUserMail.Text = message.UserMailAddr;
		}
		else
		{
			var user = new UserService().Get(message.EX_UserId);
			if (user == null) return;

			tbUserName1.Text = user.Name1;
			tbUserName2.Text = user.Name2;
			tbUserNameKana1.Text = user.NameKana1;
			tbUserNameKana2.Text = user.NameKana2;
			tbUserTel1.Text = user.Tel1;
			tbUserMail.Text = user.MailAddr;
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
		this.IncidentIdForEdit = message.IncidentId;
		this.MessageNoForEdit = message.MessageNo;
		foreach (ListItem li in rblMediaKbn.Items)
		{
			li.Selected = (li.Value == message.MediaKbn);
		}
		foreach (ListItem li in rblDirectionKbn.Items)
		{
			li.Selected = (li.Value == message.DirectionKbn);
		}

		tbInquiryDateTime.Text = DateTimeUtility.ToStringForManager(
			message.InquiryReplyDate,
			DateTimeUtility.FormatType.ShortDateHourMinuteNoneServerTime);
		tbUserName1.Text = message.UserName1;
		tbUserName2.Text = message.UserName2;
		tbUserNameKana1.Text = message.UserNameKana1;
		tbUserNameKana2.Text = message.UserNameKana2;
		tbUserTel1.Text = message.UserTel1;
		tbUserMail.Text = message.UserMailAddr;
		tbInquiryTitle.Text = message.InquiryTitle;
		tbInquiryText.Text = message.InquiryText;
		tbReplyText.Text = message.ReplyText;
		foreach (ListItem li in ddlReplyOperators.Items)
		{
			li.Selected = (li.Value == message.ReplyOperatorId);
		}
	}
	#endregion

	#region #btnSetMessageInquiryDateTime_Click メッセージ問合せ日時セットボタンクリック
	/// <summary>
	/// メッセージ問合せ日時セット
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSetMessageInquiryDateTime_Click(object sender, EventArgs e)
	{
		SetMessageInquiryDateTime();
	}
	#endregion

	#region -SetMessageInquiryDateTime メッセージ問合せ日時セット
	/// <summary>
	/// メッセージ問合せ日時セット
	/// </summary>
	private void SetMessageInquiryDateTime()
	{
		tbInquiryDateTime.Text = DateTimeUtility.ToStringForManager(
			DateTime.Now,
			DateTimeUtility.FormatType.ShortDateHourMinuteNoneServerTime);
	}
	#endregion

	#region +CheckMessageInput メッセージ入力チェック
	/// <summary>
	/// メッセージ入力チェック
	/// </summary>
	/// <param name="messageStatus">メッセージステータス</param>
	/// <param name="saveAsDraft">下書きか（下書きの場合は必須チェックを実施しない）</param>
	/// <returns>メッセージモデル</returns>
	public CsMessageModel CheckMessageInput(string messageStatus, bool saveAsDraft)
	{
		Hashtable input = GetTelInput(messageStatus);

		var inputForCheck = (Hashtable)input.Clone();
		if (saveAsDraft) inputForCheck.Keys.Cast<string>().Where(key => StringUtility.ToEmpty(inputForCheck[key]) == "").ToList().ForEach(key => inputForCheck[key] = null);
		// 日付入力チェックのため、一時的にカルチャーを変更する
		var currentCulture = Thread.CurrentThread.CurrentCulture;
		if (Constants.GLOBAL_OPTION_ENABLE && (string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) == false))
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE);
		}
		var error = Validator.Validate("CsMessage", inputForCheck);
		// カルチャーを復元
		Thread.CurrentThread.CurrentCulture = currentCulture;
		lErrorMessages.Text = error;
		if (string.IsNullOrEmpty(error) == false)
		{
			aTelTitle.Focus();
			return null;
		}

		var message = new CsMessageModel(input);

		return message;
	}
	#endregion

	#region -GetTelInput 電話問合せ入力情報取得
	/// <summary>
	/// 電話問合せ入力情報取得
	/// </summary>
	/// <param name="messageStatus">メッセージステータス</param>
	/// <returns>電話問合せ入力情報</returns>
	public Hashtable GetTelInput(string messageStatus)
	{
		Hashtable input = new Hashtable();
		input.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, this.LoginOperatorDeptId);
		input.Add(Constants.FIELD_CSMESSAGE_INCIDENT_ID, this.IncidentIdForEdit);
		input.Add(Constants.FIELD_CSMESSAGE_MESSAGE_NO, this.MessageNoForEdit ?? 0);
		input.Add(Constants.FIELD_CSMESSAGE_MEDIA_KBN, rblMediaKbn.SelectedValue);
		input.Add(Constants.FIELD_CSMESSAGE_DIRECTION_KBN, rblDirectionKbn.SelectedValue);
		input.Add(Constants.FIELD_CSMESSAGE_OPERATOR_ID, this.LoginOperatorId);
		input.Add(Constants.FIELD_CSMESSAGE_MESSAGE_STATUS, messageStatus);
		input.Add(Constants.FIELD_CSMESSAGE_INQUIRY_REPLY_DATE, tbInquiryDateTime.Text);
		input.Add(Constants.FIELD_CSMESSAGE_USER_NAME1, StringUtility.ToZenkaku(tbUserName1.Text));
		input.Add(Constants.FIELD_CSMESSAGE_USER_NAME2, StringUtility.ToZenkaku(tbUserName2.Text));
		input.Add(Constants.FIELD_CSMESSAGE_USER_NAME_KANA1, StringUtility.ToZenkaku(tbUserNameKana1.Text));
		input.Add(Constants.FIELD_CSMESSAGE_USER_NAME_KANA2, StringUtility.ToZenkaku(tbUserNameKana2.Text));
		input.Add(Constants.FIELD_CSMESSAGE_USER_TEL1, StringUtility.ToHankaku(tbUserTel1.Text));
		input.Add(Constants.FIELD_CSMESSAGE_USER_MAIL_ADDR, StringUtility.ToHankaku(tbUserMail.Text));
		input.Add(Constants.FIELD_CSMESSAGE_INQUIRY_TITLE, tbInquiryTitle.Text);
		input.Add(Constants.FIELD_CSMESSAGE_INQUIRY_TEXT, tbInquiryText.Text);
		input.Add(Constants.FIELD_CSMESSAGE_REPLY_TEXT, tbReplyText.Text);
		input.Add(Constants.FIELD_CSMESSAGE_REPLY_OPERATOR_ID, ddlReplyOperators.SelectedValue);
		input.Add(Constants.FIELD_CSMESSAGE_RECEIVE_MAIL_ID, "");
		input.Add(Constants.FIELD_CSMESSAGE_SEND_MAIL_ID, "");
		input.Add(Constants.FIELD_CSMESSAGE_VALID_FLG, Constants.FLG_CSMESSAGE_VALID_FLG_VALID);
		input.Add(Constants.FIELD_CSMESSAGE_LAST_CHANGED, this.LoginOperatorName);
		return input;
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
		tbReplyText.Text = text + "\r\n\r\n" + tbReplyText.Text;
		tbInquiryTitle.Text = (string.IsNullOrEmpty(title) == false)
			? title
			: tbInquiryTitle.Text;
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
	private int? MessageNoForEdit
	{
		get { return (int?)ViewState["MessageNoForEdit"]; }
		set { ViewState["MessageNoForEdit"] = value; }
	}
	#endregion
}

