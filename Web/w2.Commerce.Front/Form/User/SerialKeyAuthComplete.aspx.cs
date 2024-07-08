/*
=========================================================================================================
  Module      : シリアルキー認証完了画面処理(SerialKeyAuthComplete.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.Common.Logger;
using w2.App.Common.Product;

public partial class Form_User_SerialKeyAuthComplete : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		// 何もしない //
	}

	/// <summary>
	/// 「ダウンロード」リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDownload_Click(object sender, EventArgs e)
	{
		// ダウンロードログ出力
		String format = "シリアルキーがダウンロードされました。 ([order_id]={0}, [serial_key]={1}, [download_url]={2}, [user_id]={3})";
		Object[] args = new Object[] { OrderId, SerialKey, DownloadUrl, UserId };
		FileLogger.Write(BaseLogger.LOGKBN_INFO, String.Format(format, args), true);

		// ダウンロード先にリダイレクト
		Response.Redirect(this.DownloadUrl);
	}

	/// <summary>
	/// 「トップページへ」リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbTopPage_Click(object sender, EventArgs e)
	{
		// トップページへ
		Response.Redirect(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT);
	}

	/// <summary>表示文字列</summary>
	protected string OrderId { get { return ((Dictionary<string, string>)Session[Constants.SESSION_KEY_PARAM])["order_id"]; } }
	protected string ProductName { get { return ((Dictionary<string, string>)Session[Constants.SESSION_KEY_PARAM])["product_name"]; } }
	protected string DownloadUrl { get { return ((Dictionary<string, string>)Session[Constants.SESSION_KEY_PARAM])["download_url"]; } }
	protected string SerialKey { get { return SerialKeyUtility.DecryptSerialKey(((Dictionary<string, string>)Session[Constants.SESSION_KEY_PARAM])["serial_key"]); } }
	protected string SerialKeyFormatted { get { return SerialKeyUtility.GetFormattedKeyString(this.SerialKey); } }
	protected string UserId { get { return ((Dictionary<string, string>)Session[Constants.SESSION_KEY_PARAM])["user_id"]; } }

}
