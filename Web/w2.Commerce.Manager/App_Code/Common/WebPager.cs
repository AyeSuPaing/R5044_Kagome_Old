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
	/// デフォルトページャ作成
	/// </summary>
	/// <param name="iTotal"></param>
	/// <param name="iCurrentPage"></param>
	/// <param name="strPageUrl"></param>
	/// <param name="iDispContents"></param>
	/// <returns></returns>
	public static string CreateDefaultListPager(int iTotal, int iCurrentPage, string strPageUrl, int iDispContents)
	{
		return CreatePager(GetPagerSetting(m_pagerXMlFilePath, "DefaultListPagerSetting"), iTotal, iCurrentPage, strPageUrl, iDispContents, 4);
	}
	/// <summary>
	/// Display the page when the page parameter is included
	/// </summary>
	/// <param name="total">Total</param>
	/// <param name="currentPage">Current page</param>
	/// <param name="pageUrl">Page url</param>
	/// <param name="dispLinkPages">リンク表示数</param>
	/// <returns>Default List Pager With Limit Number Page</returns>
	public static string CreateDefaultListPagerWithLimitNumberPage(int total, int currentPage, string pageUrl, int dispLinkPages)
	{
		return CreatePager(GetPagerSetting(m_pagerXMlFilePath, "DefaultListPagerSetting"), total, currentPage, pageUrl, Constants.CONST_DISP_CONTENTS_DEFAULT_LIST, dispLinkPages);
	}

	/// <summary>
	/// 商品レビュー用ページャ作成
	/// </summary>
	/// <param name="iTotal"></param>
	/// <param name="iCurrentPage"></param>
	/// <param name="strPageUrl"></param>
	/// <returns></returns>
	public static string CreateProductReviewPager(int iTotal, int iCurrentPage, string strPageUrl)
	{
		return CreatePager(GetPagerSetting(m_pagerXMlFilePath, "DefaultListPagerSetting"), iTotal, iCurrentPage, strPageUrl, Constants.CONST_DISP_CONTENTS_PRODUCTREVIEW_LIST, 4);
	}

	/// <summary>
	/// 決済種別用ページャ作成
	/// </summary>
	/// <param name="iTotal"></param>
	/// <param name="iCurrentPage"></param>
	/// <param name="strPageUrl"></param>
	/// <returns></returns>
	public static string CreatePaymentPager(int iTotal, int iCurrentPage, string strPageUrl)
	{
		return CreatePager(GetPagerSetting(m_pagerXMlFilePath, "DefaultListPagerSetting"), iTotal, iCurrentPage, strPageUrl, Constants.CONST_DISP_CONTENTS_PAYMENT_LIST, 4);
	}

	/// <summary>
	/// 注文ワークフロー設定用ページャ作成
	/// </summary>
	/// <param name="total">トータル</param>
	/// <param name="currentPage">現ページ</param>
	/// <param name="pageUrl">ページURL</param>
	/// <returns>ページャHTML</returns>
	public static string CreateOrderWorkflowSettingListPager(int total, int currentPage, string pageUrl)
	{
		return CreatePager(GetPagerSetting(m_pagerXMlFilePath, "DefaultListPagerSetting"), total, currentPage, pageUrl, Constants.CONST_DISP_CONTENTS_ORDERWORKFLOWSETTING_LIST, 4);
	}
}
