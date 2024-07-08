/*
=========================================================================================================
  Module      : トラッキングログ送信ページ処理(SendTrackingLog.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Option;
using w2.App.Common.Util;

public partial class Form_Recommend_SendTrackingLog : ProductPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
    {
		//------------------------------------------------------
		// 次の画面に遷移
		//------------------------------------------------------
		Response.Redirect(NextUrlValidation(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NEXT_URL])));
    }
}