/*
=========================================================================================================
  Module      : Amazon 注文コールバック画面(OrderCallback.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.Amazon.Util;
using w2.Common.Web;
using w2.Domain.UpdateHistory.Helper;

/// <summary>
/// Amazon 注文者決定コールバック画面
/// </summary>
public partial class Form_User_Amazon_AmazonPayWidgetCallback : AmazonLoginPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (IsPostBack)
		{
			// 初期処理
			InitPage();

			var state = Request[AmazonConstants.REQUEST_KEY_AMAZON_STATE];
			Response.Redirect(state);
		}
	}
}
