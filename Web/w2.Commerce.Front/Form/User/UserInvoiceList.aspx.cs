/*
=========================================================================================================
  Module      : User Invoice List Screen Processing(UserInvoiceList.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.TwUserInvoice;
using w2.Domain.UpdateHistory.Helper;

public partial class Form_User_UserInvoiceList : ProductPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }

	#region ラップ済コントロール宣言
	WrappedRepeater WrUserInvoiceList { get { return GetWrappedControl<WrappedRepeater>("rUserInvoiceList"); } }
	#endregion

	private const string SESSION_KEY_DELTE_MESSAGE_FLG = "delete_message_flg";		// 削除時メッセージ

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// リクエストよりパラメタ取得
			GetParameters();

			// User invoice information acquisition
			var beginRowNum = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.PageNumber - 1) + 1;
			var endRowNum = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.PageNumber;
			var twUserInvoiceService = new TwUserInvoiceService();
			var twUserInvoicesCount = twUserInvoiceService.GetSearchHitCount(this.LoginUserId, beginRowNum, endRowNum);
			var userInvoices = twUserInvoiceService.Search(this.LoginUserId, beginRowNum, endRowNum);

			// User Invoice List Settings
			if (twUserInvoicesCount != 0)
			{
				// If not 0, set pager
				var nextUrl = Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_INVOICE_LIST;
				this.PagerHtml = WebPager.CreateDefaultListPager(twUserInvoicesCount, this.PageNumber, nextUrl);
			}
			else
			{
				// If 0, error message is display
				this.ErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USERINVOICE_NO_INVOICE);
				this.WrUserInvoiceList.Visible = false;
			}

			// Data binding
			this.WrUserInvoiceList.DataSource = userInvoices;
			this.WrUserInvoiceList.DataBind();

			// Delete if there is a message when deleting
			this.IsDelete= (Session[SESSION_KEY_DELTE_MESSAGE_FLG] != null);
			Session[SESSION_KEY_DELTE_MESSAGE_FLG] = null;
		}
	}

	/// <summary>
	/// リピータイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rUserInvoiceList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		var invoiceNo = int.Parse(e.CommandArgument.ToString());

		// Update User Invoice Information
		if (e.CommandName == "Update")
		{
			var nextUrl = new StringBuilder();
			nextUrl.Append(this.SecurePageProtocolAndHost).Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_USER_INVOICE_INPUT)
				.Append("?").Append(Constants.REQUEST_KEY_INVOICE_NO).Append("=").Append(invoiceNo.ToString());

			// To the user invoice input screen
			Response.Redirect(nextUrl.ToString());
		}
		// Delete User Invoice Information
		else if (e.CommandName == "Delete")
		{
			// Delete user invoice information
			new TwUserInvoiceService().Delete(
				this.LoginUserId,
				invoiceNo,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert);

			// Message flag when deleting
			Session[SESSION_KEY_DELTE_MESSAGE_FLG] = true;

			// Return to user invoice list
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_INVOICE_LIST);
		}
	}

	/// <summary>
	/// Click The Add User Invoice Button
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbInsert_Click(object sender, System.EventArgs e)
	{
		// To User Invoice Input Screen
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_INVOICE_INPUT);
	}

	/// <summary>
	/// Get Display Code
	/// </summary>
	/// <param name="userInvoiceModel">User Invoice data item</param>
	/// <returns>String</returns>
	protected string GetDisplayCode(TwUserInvoiceModel userInvoiceModel)
	{
		var result = string.Empty;

		switch (userInvoiceModel.TwUniformInvoice)
		{
			case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
				result = userInvoiceModel.TwCarryTypeOption;
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
				result = string.Format("{0} : {1}<br/>{2} : {3}",
					ReplaceTag("@@TwInvoice.uniform_invoice_company_code_option.name@@"),
					userInvoiceModel.TwUniformInvoiceOption1,
					ReplaceTag("@@TwInvoice.uniform_invoice_company_name_option.name@@"),
					userInvoiceModel.TwUniformInvoiceOption2);
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
				result = string.Format("{0}<br/>{1}",
					ReplaceTag("@@TwInvoice.uniform_invoice_donate_code_option.name@@"),
					userInvoiceModel.TwUniformInvoiceOption1);
				break;
		}

		return result;
	}

	/// <summary>ページャーHTML</summary>
	protected string PagerHtml
	{
		get { return (string)ViewState["PagerHtml"]; }
		private set { ViewState["PagerHtml"] = value; }
	}
	/// <summary>エラーメッセージ</summary>
	protected string ErrorMessage 
	{
		get { return (string)ViewState["ErrorMessage"]; }
		private set { ViewState["ErrorMessage"] = value; }
	}
	/// <summary>デリート？</summary>
	protected bool IsDelete
	{
		get { return (bool)ViewState["DeleteMessage"]; }
		set { ViewState["DeleteMessage"] = value; }
	}
}
