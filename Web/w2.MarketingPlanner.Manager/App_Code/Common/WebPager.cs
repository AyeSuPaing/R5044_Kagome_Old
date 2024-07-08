/*
=========================================================================================================
  Module      : ページャモジュール(WebPager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Text;

/// <summary>
/// WebPager の概要の説明です
/// </summary>
public class WebPager : w2.Common.Web.PagerUtility
{
	/// <summary>ページャXMLファイルパス</summary>
	private readonly static string m_pagerXMlFilePath = AppDomain.CurrentDomain.BaseDirectory + "Xml/Pager.xml";

	/// <summary>
	/// デフォルトページャ作成
	/// </summary>
	/// <param name="iTotal"></param>
	/// <param name="iCurrentPage"></param>
	/// <param name="strPageUrl"></param>
	/// <returns></returns>
	public static string CreateDefaultListPager(int iTotal, int iCurrentPage, string strPageUrl)
	{
		return CreatePager(GetPagerSetting(m_pagerXMlFilePath, "DefaultListPagerSetting"), iTotal, iCurrentPage, strPageUrl, Constants.CONST_DISP_CONTENTS_DEFAULT_LIST, 4);
	}

	/// <summary>
	/// ページャ作成
	/// </summary>
	/// <param name="iTotal"></param>
	/// <param name="iCurrentPage"></param>
	/// <param name="strPageUrl"></param>
	/// <param name="iDispLinkPages"></param>
	/// <returns></returns>
	public static string CreateListPager(int iTotal, int iCurrentPage, string strPageUrl, int iDispLinkPages)
	{
		return CreatePager(GetPagerSetting(m_pagerXMlFilePath, "DefaultListPagerSetting"), iTotal, iCurrentPage, strPageUrl, iDispLinkPages, 2);
	}

	/// <summary>
	/// Create report ltv list pager
	/// </summary>
	/// <param name="total">Total</param>
	/// <param name="currentPage">Current page</param>
	/// <param name="pageUrl">Page url</param>
	/// <param name="dispLinkPages">Disp link pages</param>
	/// <returns>Pager html</returns>
	public static string CreateReportLtvListPager(int total, int currentPage, string pageUrl, int dispLinkPages)
	{
		return CreatePager(GetPagerSetting(m_pagerXMlFilePath, "ReportLtvListPagerSetting"), total, currentPage, pageUrl, dispLinkPages, 2);
	}
}
