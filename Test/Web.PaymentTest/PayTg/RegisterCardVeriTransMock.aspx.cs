/*
=========================================================================================================
  Module      : ベリトランスPayTgモック(RegisterCardVeriTransMock.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web.UI;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Payment.PayTg;

public partial class Form_PayTg_RegisterCardVeriTransMock : Page
{
	/// <summary>
	///  ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			hfToken.Value = string.Empty;
			var accountId = Request[PayTgConstants.PARAM_CUSTOMERID];
			lbAccountId.Text = accountId;

			var dtNow = DateTime.Now.AddYears(5);
			var iYear = dtNow.Year;
			var iMonth = dtNow.Month;
			tbExp.Text = string.Format("{0}/{1}", iMonth.ToString().PadLeft(2,'0'), iYear.ToString().Substring(2)); 

		}
	}

	/// <summary>
	/// 失敗カードの実行ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void btnCreateTokenId_Click(object sender, EventArgs e)
	{
		var carInfo = string.Format("{0}#{1}#{2}", cardNumber.Text, tbExp.Text, csc.Text);

		ScriptManager.RegisterStartupScript(
		this, GetType(),
		"getTokenVeriTrans",
		"getTokenVeriTrans('" + carInfo + "')",
		true);
	}

	/// <summary>
	/// カード登録処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void RegisterCard_Click(object sender, EventArgs e)
	{
		var isSuccess = false;
		if (string.IsNullOrEmpty(hfToken.Value) == false)
		{
			var result = new AccountManager().AddCardByUser(lbAccountId.Text, hfToken.Value);
			isSuccess = (result.Mstatus == VeriTransConst.RESULT_STATUS_OK);
		}

		var reStr = isSuccess.ToString().ToLower();

		lMessage.Text = string.Format("カード登録が{0}でした！", isSuccess ? "成功" : "失敗");
		ScriptManager.RegisterStartupScript(
			this,
			GetType(),
			"returnResponse",
			"returnResponse(" + reStr + ");",
			true);
	}

}