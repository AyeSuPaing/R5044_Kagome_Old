/*
=========================================================================================================
  Module      : 注文ページマスタ処理(OrderPage.master.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;

public partial class Form_Common_OrderPage : BaseMasterPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		Response.AddHeader("Cache-Control", "private, no-store, no-cache, must-revalidate");
		Response.AddHeader("Pragma", "no-cache");
	}
}
