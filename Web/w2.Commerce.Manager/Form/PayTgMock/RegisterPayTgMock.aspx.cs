/*
=========================================================================================================
  Module      : PayTg：クレジットカード登録モックページ(RegisterPayTgMock.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web.UI;

public partial class Form_PayTgMock_RegisterPayTgMock : BasePage
{
	/// <summary>
	///  ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 特になにもしない
	}

	/// <summary>
	/// 与信OK実行ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSendAuthOk_Click(object sender, EventArgs e)
	{
		ExecRegistrationCard("OK", "success");
	}

	/// <summary>
	/// 与信NG実行ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSendAuthNg_Click(object sender, EventArgs e)
	{
		ExecRegistrationCard("NG", "failure");
	}

	/// <summary>
	/// カード登録実行
	/// </summary>
	/// <param name="resultCase">期待結果</param>
	/// <param name="authResult">与信結果</param>
	protected void ExecRegistrationCard(string resultCase, string authResult)
	{
		bool isSuccess = false;
		switch (resultCase)
		{
			case "OK":
				isSuccess = true;
				break;
			case "NG":
				isSuccess = false;
				break;
		}

		lMessage.Text = string.Format("カード登録が{0}でした！", isSuccess ? "成功" : "失敗");
		var reStr = isSuccess.ToString().ToLower();

		ScriptManager.RegisterStartupScript(
			this,
			GetType(),
			"returnResponseRakuten",
			string.Format(
				"returnResponseRakuten('{0}', '{1}');",
				authResult,
				resultCase),
			true);

	}
}
