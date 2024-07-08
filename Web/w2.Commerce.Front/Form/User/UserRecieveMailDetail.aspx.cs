/*
=========================================================================================================
  Module      : 受信メール詳細画面処理(UserRecieveMailDetail.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using w2.App.Common.Mail;
using w2.App.Common.ShopMessage;
using w2.Common.Web;
using w2.Domain.MailSendLog;
using w2.Domain.User;

public partial class Form_User_UserRecieveMailDetail : BasePage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// ログインユーザー宛てのメールか確認
			CorrectRecipientConfirm();

			// 情報の表示
			DisplayRecieveMail();

			// 既読へ更新
			if (this.IsRead == false) UpdateStatusRead();
		}
	}

	/// <summary>
	/// 正しい受信者か確認
	/// </summary>
	private void CorrectRecipientConfirm()
	{
		var receiveMailNo = long.Parse(Request[Constants.REQUEST_KEY_RECIEVEMAIL_NO]);
		var mailSendLog = new MailSendLogService().GetForDisplay(receiveMailNo, this.LoginUserId);
		if (mailSendLog == null) RedirectErrorPage();
	}

	/// <summary>
	/// エラーページへリダイレクト
	/// </summary>
	private void RedirectErrorPage()
	{
		Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USERRECIEVEMAIL_NO_ITEM);
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
	}

	/// <summary>
	/// 受信メール表示
	/// </summary>
	private void DisplayRecieveMail()
	{
		var existMailSendLog = false;

		var logNo = GetLogNo();
		if (logNo != null)
		{
			var mailSendLog = new MailSendLogService().GetForDisplay(logNo.Value, this.LoginUserId);
			if (mailSendLog != null)
			{
				var user = new UserService().GetUserForMailSend(mailSendLog.UserId);
				var languageCode = (string)user.DataSource[Constants.FIELD_USER_DISP_LANGUAGE_CODE];
				var languageLocalId = (string)user.DataSource[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID];

				// メール送信ログ情報表示
				lDateSendMail.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringFromRegion(mailSendLog.DateSendMail,
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
				lMailFrom.Text = WebSanitizer.HtmlEncode(
					mailSendLog.MailFrom
						+ ((string.IsNullOrEmpty(mailSendLog.MailFromName) == false) ? " ( " + mailSendLog.MailFromName + " )" : ""));
				lMailTo.Text = WebSanitizer.HtmlEncode(mailSendLog.MailTo);
				mailSendLog.MailSubject = ShopMessageUtil.ConvertShopMessage(
					mailSendLog.MailSubject,
					languageCode,
					languageLocalId,
					false);
				mailSendLog.MailBody = ShopMessageUtil.ConvertShopMessage(
					mailSendLog.MailBody,
					languageCode,
					languageLocalId,
					false);

				// メール文章のタグを置換
				if (string.IsNullOrEmpty(mailSendLog.MailtextReplaceTags) == false)
				{
					var replaceTags = StringUtility.DecompressString(mailSendLog.MailtextReplaceTags);
					var mailTextReplaces = JsonConvert.DeserializeObject<Dictionary<string, string>>(replaceTags);

					mailSendLog.MailSubject = GetMailTemplateUtility.ConvertMailContentsForDisplay(
						mailTextReplaces,
						mailSendLog.MailSubject);

					mailSendLog.MailBody = GetMailTemplateUtility.ConvertMailContentsForDisplay(
						mailTextReplaces,
						mailSendLog.MailBody);
				}

				lMailSubject.Text = WebSanitizer.HtmlEncode(mailSendLog.MailSubject);
				if (string.IsNullOrEmpty(mailSendLog.MailBodyHtml) == false)
				{
					this.IsHtml = true;
					lMailBody.Visible = false;
				}
				else
				{
					lMailBody.Text = StringUtility.ChangeToBrTag(WebSanitizer.HtmlEncode(mailSendLog.MailBody));
				}
				existMailSendLog = true;
				this.IsRead = (mailSendLog.ReadFlg == Constants.FLG_MAILSENDLOG_READ_FLG_READ);
			}
		}

		if (existMailSendLog == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USERRECIEVEMAIL_NO_ITEM);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}
	}

	/// <summary>
	/// ステータスの更新
	/// </summary>
	private void UpdateStatusRead()
	{
		// 未読から既読へ変更
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			new MailSendLogService().UpdateReadFlg(GetLogNo().Value, true, accessor);

			accessor.CommitTransaction();
		}
		this.IsRead = true;
	}

	/// <summary>
	/// ログNo取得
	/// </summary>
	/// <returns></returns>
	private long? GetLogNo()
	{
		long logNo = 0;
		if (long.TryParse(Request[Constants.REQUEST_KEY_RECIEVEMAIL_NO], out logNo))
		{
			return logNo;
		}
		return null;
	}

	/// <summary>
	/// HtmlメールポップアップUrl取得
	/// </summary>
	/// <returns></returns>
	protected string GetHtmlMailPopupUrl()
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_MAIL).AddParam(
			Constants.REQUEST_KEY_RECIEVEMAIL_NO,
			GetLogNo().Value.ToString()).CreateUrl();
		return url;
	}

	/// <summary>既読か</summary>
	private bool IsRead { get; set; }
	/// <summary>Htmlか</summary>
	protected bool IsHtml { get; set; }
}