/*
=========================================================================================================
  Module      : Htmlメールページ処理(HtmlMail.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using w2.App.Common.Mail;
using w2.App.Common.ShopMessage;
using w2.Domain.MailSendLog;
using w2.Domain.User;

public partial class HtmlMail : System.Web.UI.Page
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		var userId = (string)Session[Constants.SESSION_KEY_LOGIN_USER_ID];
		var mailSendLog = new MailSendLogService().GetForDisplay(GetLogNo().Value, userId);
		if (mailSendLog == null) RedirectErrorPage();
		
		var user = new UserService().GetUserForMailSend(mailSendLog.UserId);
		var languageCode = (string)user.DataSource[Constants.FIELD_USER_DISP_LANGUAGE_CODE];
		var languageLocalId = (string)user.DataSource[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID];

		mailSendLog.MailBodyHtml = ShopMessageUtil.ConvertShopMessage(
			mailSendLog.MailBodyHtml,
			languageCode,
			languageLocalId,
			true);

		if (string.IsNullOrEmpty(mailSendLog.MailtextReplaceTags) == false)
		{
			var replaceTags = StringUtility.DecompressString(mailSendLog.MailtextReplaceTags);
			var mailTextReplaces = JsonConvert.DeserializeObject<Dictionary<string, string>>(replaceTags);

			mailSendLog.MailBodyHtml = GetMailTemplateUtility.ConvertMailContentsForDisplay(
				mailTextReplaces,
				mailSendLog.MailBodyHtml);
		}

		lMailBody.Text = mailSendLog.MailBodyHtml;
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
}
