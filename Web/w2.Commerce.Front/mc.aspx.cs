/*
=========================================================================================================
  Module      : メールクリックページ処理(mc.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;

public partial class mc : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// リクエスト取得
		string mailClickKey = StringUtility.ToEmpty(Request["key"]);
		string userId = StringUtility.ToEmpty(Request["uid"]);

		// メールクリックログ作成
		DataView mailClickList = new DataView();
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MailClick", "CreteMailClickLog"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MAILCLICK_MAILCLICK_KEY, mailClickKey);
			htInput.Add(Constants.FIELD_MAILCLICKLOG_USER_ID, userId);
			mailClickList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		// リダイレクト
		if (mailClickList.Count != 0)
		{
			Response.Redirect((string)mailClickList[0][Constants.FIELD_MAILCLICK_MAILCLICK_URL]);
		}
		else
		{
			Response.Redirect(Constants.PATH_ROOT);
		}
	}
}
