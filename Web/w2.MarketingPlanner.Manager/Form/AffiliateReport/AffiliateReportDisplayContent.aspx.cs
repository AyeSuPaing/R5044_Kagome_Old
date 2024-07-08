/*
=========================================================================================================
  Module      : アフィリエイトレポート一覧 コンテンツ出力ページ処理(AffiliateReportDisplayContent.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
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
using System.Xml;

public partial class Form_AffiliateReport_AffiliateReportDisplayContent : BasePage
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
		// アフィリエイト連携ログ情報取得
		DataView dvResult = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("AffiliateCoopLog", "GetAffiliateReportFromLogNo"))
		{
			dvResult = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htParam);
		}

		// ログNo
		lLogNo.Text = dvResult[0][Constants.FIELD_AFFILIATECOOPLOG_LOG_NO].ToString();
		// ログ日時
		lLogDate.Text = DateTimeUtility.ToStringForManager(
			dvResult[0][Constants.FIELD_AFFILIATECOOPLOG_DATE_CREATED],
			DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);

		// アフィリエイト区分
		string strAffiliateKbn = null;
		// アフィリエイト区分が「リンクシェア」、「汎用アフィリエイト(PC)」の場合は、Value値を読込み
		if (((string)dvResult[0][Constants.FIELD_AFFILIATECOOPLOG_AFFILIATE_KBN] == Constants.FLG_AFFILIATECOOPLOG_AFFILIATE_KBN_LINKSHARE_REP)
			|| ((string)dvResult[0][Constants.FIELD_AFFILIATECOOPLOG_AFFILIATE_KBN] == Constants.FLG_AFFILIATECOOPLOG_AFFILIATE_KBN_PC))
		{
			strAffiliateKbn = ValueText.GetValueText(
										Constants.TABLE_AFFILIATECOOPLOG,
										Constants.FIELD_AFFILIATECOOPLOG_AFFILIATE_KBN,
										(string)dvResult[0][Constants.FIELD_AFFILIATECOOPLOG_AFFILIATE_KBN]);
		}
		// それ以外は、アフィリエイト区分(セッション変数名1)をそのまま出力
		else
		{
			strAffiliateKbn = (string)dvResult[0][Constants.FIELD_AFFILIATECOOPLOG_AFFILIATE_KBN];
		}
		lAffiliateKbn.Text = WebSanitizer.HtmlEncode(strAffiliateKbn);

		lMasterId.Text = WebSanitizer.HtmlEncode((string)dvResult[0][Constants.FIELD_AFFILIATECOOPLOG_MASTER_ID]);		// マスタID
		lTagId.Text = ((dvResult[0][Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA11]) == DBNull.Value) ? string.Empty : WebSanitizer.HtmlEncode(dvResult[0][Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA11].ToString());		// タグID
		lCoopData1.Text = WebSanitizer.HtmlEncode((string)dvResult[0][Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA1]);	// 連携データ1
		lCoopData2.Text = WebSanitizer.HtmlEncode((string)dvResult[0][Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA2]);	// 連携データ2
		lCoopData4.Text = WebSanitizer.HtmlEncode((string)dvResult[0][Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA4]);	// 連携データ4
		lCoopData5.Text = WebSanitizer.HtmlEncode((string)dvResult[0][Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA5]);	// 連携データ5
	}

	/// <summary>
	/// パラメタ取得
	/// </summary>
	/// <param name="hrRequest">パラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParameters(HttpRequest hrRequest)
	{
		Hashtable htResult = new Hashtable();

		htResult.Add(Constants.FIELD_AFFILIATECOOPLOG_LOG_NO, hrRequest[Constants.REQUEST_KEY_AFFILIATET_REPORT_LOG_NO]);

		return htResult;
	}
}
