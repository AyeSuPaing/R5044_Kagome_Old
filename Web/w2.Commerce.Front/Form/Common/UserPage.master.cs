/*
=========================================================================================================
  Module      : マイページマスタ処理(UserPage.master.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;

public partial class Form_Common_UserPage : BaseMasterPage
{
	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected override void Page_Init(object sender, EventArgs e)
	{
		// 基底クラスのメソッドをロード
		base.Page_Init(sender, e);

		// マイページメニュー表示制御
		BodyMyPageMenu.Visible = this.DispMyPageMenu;
	}

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// なにもしない
	}
}
