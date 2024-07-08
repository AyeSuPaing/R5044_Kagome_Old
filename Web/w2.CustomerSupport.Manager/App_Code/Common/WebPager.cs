/*
=========================================================================================================
  Module      : ページャモジュール(WebPager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
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
}
