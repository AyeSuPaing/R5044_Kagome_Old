/*
=========================================================================================================
  Module      : メール配信文章タグマニュアルページ処理(MailDistTextTagManual.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

public partial class Form_MailDistText_MailDistTextTagManual : BasePage
{
	protected string XPATH_PAGE = "/MailDistTextTagManual/Node";
	protected const string XPATH_PAGE_NODE_TAG = "Tag";
	protected const string XPATH_PAGE_NODE_DESCRIPTION = "Description";
	protected const string XPATH_PAGE_NODE_SAMPLE = "Sample";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
		// メール配信文章タグマニュアルXML読み込み
		ViewMailDistTextTagManualData();
    }
	
	/// <summary>
	/// メール配信文章タグマニュアルXML読み込み
	/// </summary>
	private void ViewMailDistTextTagManualData()
	{
		// XMLファイル読み込み
		XmlDocument xd = new XmlDocument();
		xd.Load(AppDomain.CurrentDomain.BaseDirectory + Constants.FILE_XML_MAILDISTTEXTTAG_MANUAL);

		// XMLノードリストを格納
		if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false) XPATH_PAGE = XPATH_PAGE + "[" + XPATH_PAGE_NODE_TAG + "!='<@@user:mail_addr2@@>']";
		XmlNodeList xnlNodeList = xd.SelectNodes(XPATH_PAGE);

		rMailDistTextTagManual.DataSource = xnlNodeList;
		rMailDistTextTagManual.DataBind();
	}
}