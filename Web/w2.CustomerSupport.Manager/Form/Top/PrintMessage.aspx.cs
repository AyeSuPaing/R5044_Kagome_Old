/*
=========================================================================================================
  Module      : トップメッセージ印刷ページ処理(PrintMessage.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using w2.App.Common.Cs.Message;

public partial class Form_Top_PrintMessage : BasePageCs
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		int messageNo;
		if (int.TryParse(Request[Constants.REQUEST_KEY_MESSAGE_NO], out messageNo) == false) messageNo = 0;

		// Get message with mail information
		var service = new CsMessageService(new CsMessageRepository());
		this.Message = service.GetWithMail(this.LoginOperatorDeptId, Request[Constants.REQUEST_KEY_INCIDENT_ID], messageNo);
	}

	#region Properties
	/// <summary>Message</summary>
	protected CsMessageModel Message
	{
		get { return (CsMessageModel)(ViewState["Message"]); }
		set { ViewState["Message"] = value; }
	}
	#endregion
}