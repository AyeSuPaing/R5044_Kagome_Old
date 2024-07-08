/*
=========================================================================================================
  Module      : Amazon Pay詳細ウィジェット画面処理(AmazonPayDetailWidget.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;

public partial class Form_Common_AmazonPayDetailWidget : AmazonPayHistoryPage
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