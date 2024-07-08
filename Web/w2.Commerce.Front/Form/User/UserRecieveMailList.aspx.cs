/*
=========================================================================================================
  Module      : 受信メール履歴画面処理(UserRecieveMailList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Domain.MailSendLog;
using w2.Common.Web;

public partial class Form_User_UserRecieveMailList : BasePage
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
			BindMailList();
		}
	}

	/// <summary>
	/// 受信メール履歴のリストをバインド
	/// </summary>
	private void BindMailList()
	{
		// メール情報取得
		var mailSendLogs = new MailSendLogService().GetMailSendLogByUserId(this.LoginUserId)
			.OrderByDescending(table => table.DateSendMail).ToArray();

		// 0件の場合エラーメッセージを表示
		if(mailSendLogs.Count() == 0)
		{
			// エラーメッセージ設定
			this.ErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USERRECIEVEMAIL_NO_LIST);
			return;
		}

		// ページャーの設定
		var pageNo = 1;
		if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]) == false)
		{
			int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNo);
		}
		this.PageNumber = pageNo;

		rUserRecieveMailList.DataSource = mailSendLogs
			.Skip((this.PageNumber - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST)
			.Take(Constants.CONST_DISP_CONTENTS_DEFAULT_LIST);
		rUserRecieveMailList.DataBind();

		this.PagerHtml = WebPager.CreateDefaultListPager(
			mailSendLogs.Count(),
			this.PageNumber,
			Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_RECIEVE_MAIL_LIST);
	}

	/// <summary>
	/// 詳細画面へのURL作成
	/// </summary>
	/// <param name="logno"></param>
	/// <returns></returns>
	public string CreateUrlToDetail(long? logno)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_RECIEVE_MAIL_DETAIL)
			.AddParam(Constants.REQUEST_KEY_RECIEVEMAIL_NO, logno.Value.ToString())
			.CreateUrl();
		return url;
	}

	/// <summary>ページ番号</summary>
	protected int PageNumber { get; set; }
	/// <summary>ページャーHTML</summary>
	protected string PagerHtml { get; set; }
	/// <summary>エラーメッセージ</summary>
	protected string ErrorMessage { get; set; }
}