/*
=========================================================================================================
  Module      : メール添付アップロードページ処理(MailAttachmentUploader.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Cs.Message;

public partial class Form_Message_MailAttachmentUploader : BasePageCs
{
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

	#region #btnRegisterAttachment_Click 添付登録ボタンクリック
	/// <summary>
	/// 添付登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRegisterAttachment_Click(object sender, EventArgs e)
	{
		if (fuAttachment.HasFile == false) return;

		var model = new CsMessageMailAttachmentModel();
		model.DeptId = this.LoginOperatorDeptId;
		model.MailId = "";	// 仮登録は空
		model.FileData = fuAttachment.FileBytes;
		model.FileName = fuAttachment.FileName;

		var service = new CsMessageMailAttachmentService(new CsMessageMailAttachmentRepository());
		var fileNo = service.Register(model);

		hfAttachmentFileName.Value = model.FileName;
		hfAttachmentFileNo.Value = fileNo.ToString();

		divUpload.Visible = true;
	}
	#endregion
}