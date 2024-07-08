/*
=========================================================================================================
  Module      : ターゲットリストマージ完了画面処理 (TargetListMergeComplete.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;

public partial class Form_TargetListMerge_TargetListMergeComplete : BasePage
{

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		var result = (Hashtable)Session[Constants.SESSION_KEY_PARAM];

		// ターゲットリストマージ結果を表示します。
		lbTargetList1.Text = WebSanitizer.HtmlEncode(result[Constants.FIELD_TARGETLIST_TARGET_NAME + "1"].ToString());
		lbTargetList2.Text = WebSanitizer.HtmlEncode(result[Constants.FIELD_TARGETLIST_TARGET_NAME + "2"].ToString());
		lbDataCount1.Text = WebSanitizer.HtmlEncode(result[Constants.FIELD_TARGETLIST_DATA_COUNT + "1"].ToString());
		lbDataCount2.Text = WebSanitizer.HtmlEncode(result[Constants.FIELD_TARGETLIST_DATA_COUNT + "2"].ToString());
		lbTargetList.Text = WebSanitizer.HtmlEncode(result[Constants.FIELD_TARGETLIST_TARGET_NAME].ToString());
		lbDataCount.Text = WebSanitizer.HtmlEncode(result[Constants.FIELD_TARGETLIST_DATA_COUNT].ToString());
		lbMergeKbn.Text = WebSanitizer.HtmlEncode(result[Constants.FIELD_TARGETLIST_TARGET_TYPE].ToString());
	}

	/// <summary>
	/// ターゲットリストマージページへ遷移
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_TARGETLIST_MERGE);
	}
}