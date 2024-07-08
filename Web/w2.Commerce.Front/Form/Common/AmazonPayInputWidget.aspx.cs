/*
=========================================================================================================
  Module      : Amazon Pay入力ウィジェット画面処理(AmazonPayInputWidget.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;

public partial class Form_Common_AmazonPayInputWidget : AmazonPayHistoryPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// ログインチェック
		CheckLoggedIn();

		// HTTPS通信チェック（HTTPのとき、トップ画面へ）
		CheckHttps();

		if (!IsPostBack)
		{
			Initialize();
		}
	}
}