/*
=========================================================================================================
  Module      : ローディングジェスチャー コントローラ(Loading.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Web.Page;

public partial class Form_Common_Loading : CommonUserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	/// <summary>UpdatePanel更新時にローディングを表示するかどうか</summary>
	public bool UpdatePanelReload { get; set; }
}