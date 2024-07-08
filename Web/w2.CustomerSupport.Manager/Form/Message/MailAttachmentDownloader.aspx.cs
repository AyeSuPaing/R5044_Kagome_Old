/*
=========================================================================================================
  Module      : 添付ファイルダウンローダーページ処理(MailAttachmentDownloader.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web;
using w2.App.Common.Cs.Message;

public partial class Form_Message_MailAttachmentDownloader : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		var service = new CsMessageMailAttachmentService(new CsMessageMailAttachmentRepository());
		var attachment = service.Get(this.LoginOperatorDeptId, Request[Constants.REQUEST_KEY_MAIL_ID], int.Parse(Request[Constants.REQUEST_KEY_FILE_NO]));
		if (attachment == null) return;

		Response.ContentType = "application/octet-stream";
		Response.AppendHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(attachment.FileName));
		Response.BinaryWrite(attachment.FileData);
		Response.End();
	}
}