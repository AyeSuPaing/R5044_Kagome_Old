/*
=========================================================================================================
  Module      : Webページャ(WebPager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.Cms.Manager.Codes.Common
{
	/// <summary>
	/// WebPager
	/// </summary>
	public class WebPager
	{
		/// <summary>ページャクリエータ</summary>
		private static readonly PagerCreator m_pagerCreator =
			new PagerCreator(AppDomain.CurrentDomain.BaseDirectory + "Xml/Pager.xml");

		/// <summary>
		/// デフォルトページャ作成
		/// </summary>
		/// <param name="total"></param>
		/// <param name="currentPage"></param>
		/// <param name="pageUrl"></param>
		/// <returns></returns>
		public static string CreateDefaultListPager(int total, int currentPage, string pageUrl)
		{
			return m_pagerCreator.CreatePager(
				"DefaultListPagerSetting",
				total,
				currentPage,
				pageUrl,
				Constants.CONST_DISP_CONTENTS_DEFAULT_LIST,
				4);
		}
		/// <summary>
		/// デフォルトページャ作成
		/// </summary>
		/// <param name="total"></param>
		/// <param name="currentPage"></param>
		/// <param name="pageUrl"></param>
		/// <param name="dispContents"></param>
		/// <returns></returns>
		public static string CreateDefaultListPager(int total, int currentPage, string pageUrl, int dispContents)
		{
			return m_pagerCreator.CreatePager("DefaultListPagerSetting", total, currentPage, pageUrl, dispContents, 4);
		}
	}
}
