/*
=========================================================================================================
  Module      : モール連携監視　コンテンツ出力ページ(MallWatchingLogDisplayContent.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Form_MallWatchingLog_MallWatchingLogDisplayContent : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		Hashtable htParam = new Hashtable();

		//------------------------------------------------------
		// リクエスト情報取得
		//------------------------------------------------------
		htParam = GetParameters(Request);

		//------------------------------------------------------
		// 画面表示処理
		//------------------------------------------------------
		// モール監視ログ情報取得
		DataView dvResult = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallWatchingLog", "GetMallWatchingLogFromLogNo"))
		{
			dvResult = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htParam);
		}
		// モール名取得
		string strMallName = String.Empty;
		if (dvResult[0][Constants.FIELD_MALLWATCHINGLOG_MALL_ID].ToString() != "")
		{
			using (SqlAccessor sqlAccesser = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MallLiaise", "GetMallCooperationSetting"))
			{
				Hashtable ht = new Hashtable();
				ht[Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID] = this.LoginOperatorShopId;
				ht[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID] = (string)dvResult[0][Constants.FIELD_MALLWATCHINGLOG_MALL_ID];

				DataView dv = sqlStatement.SelectSingleStatementWithOC(sqlAccesser, ht);
				if (dv.Count != 0) strMallName = (string)dv[0][Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME];
			}
		}

		lLogNo.Text = dvResult[0][Constants.FIELD_MALLWATCHINGLOG_LOG_NO].ToString();
		lLogDate.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				(string)dvResult[0][Constants.FIELD_MALLWATCHINGLOG_WATCHING_DATE] + " "
				+ (string)dvResult[0][Constants.FIELD_MALLWATCHINGLOG_WATCHING_TIME],
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
		lLogKbn.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLWATCHINGLOG, Constants.FIELD_MALLWATCHINGLOG_LOG_KBN, (string)dvResult[0][Constants.FIELD_MALLWATCHINGLOG_LOG_KBN]));
		lBatchId.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_MALLWATCHINGLOG, Constants.FIELD_MALLWATCHINGLOG_BATCH_ID, (string)dvResult[0][Constants.FIELD_MALLWATCHINGLOG_BATCH_ID]));
		lMallIdAndName.Text = (string.IsNullOrEmpty(strMallName) ? "-（-）" : WebSanitizer.HtmlEncode((string)dvResult[0][Constants.FIELD_MALLWATCHINGLOG_MALL_ID] + "（" + strMallName + "）"));
		lLogMessage.Text = WebSanitizer.HtmlEncode((string)dvResult[0][Constants.FIELD_MALLWATCHINGLOG_LOG_MESSAGE]);
		lLogContent1.Text = WebSanitizer.HtmlEncode((string)dvResult[0][Constants.FIELD_MALLWATCHINGLOG_LOG_CONTENT1]);
		lLogContent2.Text = WebSanitizer.HtmlEncode((string)dvResult[0][Constants.FIELD_MALLWATCHINGLOG_LOG_CONTENT2]);
		lLogContent3.Text = WebSanitizer.HtmlEncode((string)dvResult[0][Constants.FIELD_MALLWATCHINGLOG_LOG_CONTENT3]);
		lLogContent4.Text = WebSanitizer.HtmlEncode((string)dvResult[0][Constants.FIELD_MALLWATCHINGLOG_LOG_CONTENT4]);
		lLogContent5.Text = WebSanitizer.HtmlEncode((string)dvResult[0][Constants.FIELD_MALLWATCHINGLOG_LOG_CONTENT5]);
	}

	/// <summary>
	/// パラメタ取得
	/// </summary>
	/// <param name="hrRequest">パラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	/// <remarks>
	/// </remarks>
	protected Hashtable GetParameters(HttpRequest hrRequest)
	{
		Hashtable htResult = new Hashtable();

		htResult.Add(Constants.FIELD_MALLWATCHINGLOG_LOG_NO, hrRequest[Constants.REQUEST_KEY_MALLWATCHINGLOG_LOG_NO]);

		return htResult;
	}
}
