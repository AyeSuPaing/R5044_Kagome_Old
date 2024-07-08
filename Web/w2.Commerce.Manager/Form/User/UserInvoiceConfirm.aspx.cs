/*
=========================================================================================================
  Module      : User Invoice Confirmation Screen Processing (UserInvoiceConfirm.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Text;
using w2.Common.Web;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserShipping;
using w2.Domain.TwUserInvoice;

public partial class Form_User_UserInvoiceConfirm : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// 表示制御
			if (this.TwUserInvoice.TwInvoiceNo != 0)
			{
				btnRegist.Visible = false;
				btnModify.Visible = true;
				pModifyInfo.Visible = true;
			}
			else
			{
				btnRegist.Visible = true;
				btnModify.Visible = false;
				pRegistInfo.Visible = true;
			}
		}
	}

	/// <summary>
	/// 登録・更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSend_Click(object sender, EventArgs e)
	{
		// ユーザ配送先情報登録・更新
		if (this.TwUserInvoice.TwInvoiceNo == 0)
		{
			new TwUserInvoiceService().Insert(this.TwUserInvoice, this.LoginOperatorName, UpdateHistoryAction.Insert);
		}
		else
		{
			new TwUserInvoiceService().Update(this.TwUserInvoice, this.LoginOperatorName, UpdateHistoryAction.Insert);
		}
		Session[Constants.SESSION_KEY_PARAM_FOR_USER_INVOICE_INPUT] = null;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = null;

		// ユーザ情報詳細画面へ
		var url = new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_USER_ID, Request[Constants.REQUEST_KEY_USER_ID])
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL)
			.AddParam("invoice", "1").CreateUrl();
		string clientScript = string.Format("window.opener.location.href=decodeURIComponent(\"{0}\");window.close();", HttpUtility.UrlEncode(url));
		ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "reloadParent", clientScript, true);
	}

	/// <summary>
	/// アドレス帳登録ページへ遷移
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		var url = new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_INVOICE_INPUT)
			.AddParam(Constants.REQUEST_KEY_INVOICE_NO, this.TwUserInvoice.TwInvoiceNo.ToString())
			.AddParam(Constants.REQUEST_KEY_USER_ID, this.TwUserInvoice.UserId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, this.ActionStatus).CreateUrl();
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = 1;
		Response.Redirect(url);
	}

	/// <summary>ユーザー配送先モデル</summary>
	protected TwUserInvoiceModel TwUserInvoice
	{
		get { return (TwUserInvoiceModel)(Session[Constants.SESSION_KEY_PARAM_FOR_USER_INVOICE_INPUT] ?? new TwUserInvoiceModel()); }
	}
}
