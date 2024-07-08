/*
=========================================================================================================
  Module      : ポップアップページマスタ処理(PopupPage.master.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;

public partial class Form_Common_PopupPage : BaseMasterPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// フレンドリーURL対応のためリアルURLをセット
		form1.Action = Request.Url.PathAndQuery;
	}
}
