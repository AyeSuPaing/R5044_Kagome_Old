/*
=========================================================================================================
  Module      : メール署名一覧ページ処理(MailSignatureList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Cs.MailSignature;

public partial class Form_Message_MailSignatureList : BasePageCs
{
	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			DisplayList();
		}
	}
	#endregion

	#region -DisplayList 一覧表示
	/// <summary>
	/// 一覧表示
	/// </summary>
	private void DisplayList()
	{
		var service = new CsMailSignatureService(new CsMailSignatureRepository());
		var list = service.GetUsableAll(this.LoginOperatorDeptId, this.LoginOperatorId);

		rList.DataSource = list;
		rList.DataBind();
	}
	#endregion
}