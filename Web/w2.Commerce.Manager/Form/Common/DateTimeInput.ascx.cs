/*
=========================================================================================================
  Module      : 日付入力コントロール処理(DateTimeInput.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Web.WebCustomControl;

public partial class Form_Common_DateTimeInput : DateTimeInputControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void Page_Load(object sender, EventArgs e)
	{
		base.Page_Load(sender, e);
	}

	/// <summary>日付第1部分クライアントID</summary>
	public string DdlDatePart1ClientId
	{
		get { return ddlDatePart1.ClientID; }
	}
	/// <summary>日付第2部分クライアントID</summary>
	public string DdlDatePart2ClientId
	{
		get { return ddlDatePart2.ClientID; }
	}
	/// <summary>日付第3部分クライアントID</summary>
	public string DdlDatePart3ClientId
	{
		get { return ddlDatePart3.ClientID; }
	}
}
