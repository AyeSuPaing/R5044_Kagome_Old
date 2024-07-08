/*
=========================================================================================================
  Module      : ページャユーティリティモジュール(PagerUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using w2.Common.Util;

namespace w2.Common.Web
{
	///**************************************************************************************
	/// <summary>
	/// ページャのHTMLを出力する
	/// </summary>
	///**************************************************************************************
	public class PagerUtility
	{
		/// <summary>テンプレート格納Dictionary</summary>
		private readonly static Dictionary<string, XmlNode> m_dicPagerSettings = new Dictionary<string, XmlNode>();
		/// <summary>更新日付格納Dictionary</summary>
		private readonly static Dictionary<string, DateTime> m_dicPagerSettingUpdates = new Dictionary<string, DateTime>();

		/// <summary>ReaderWriterLockSlimオブジェクト</summary>
		private static System.Threading.ReaderWriterLockSlim m_lock = new System.Threading.ReaderWriterLockSlim();

		/// <summary>
		/// ページャ設定XMLノード取得
		/// </summary>
		/// <param name="pagerXMlFilePath">ページャXMLファイルパス</param>
		/// <param name="settingName">セッティング名</param>
		/// <returns>ページャXMLノード</returns>
		protected static XmlNode GetPagerSetting(string pagerXMlFilePath, string settingName)
		{
			DateTime updateDate = System.IO.File.GetLastWriteTime(pagerXMlFilePath);

			bool doUpdate = false;
			//------------------------------------------------------
			// 更新判定
			//------------------------------------------------------
			// 読み込みロックをかける
			m_lock.EnterReadLock();
			try
			{
				doUpdate = ((m_dicPagerSettingUpdates.ContainsKey(settingName) == false)
					|| (m_dicPagerSettingUpdates[settingName] != updateDate));
			}
			finally
			{
				m_lock.ExitReadLock();
			}

			//------------------------------------------------------
			// 更新処理（書き込みロック）
			//------------------------------------------------------
			if (doUpdate)
			{
				UpdateDatasWithWriteLock(pagerXMlFilePath, settingName, updateDate);
			}

			//------------------------------------------------------
			// 返却
			//------------------------------------------------------
			// 読み込みロックをかける
			m_lock.EnterReadLock();
			try
			{
				return m_dicPagerSettings[settingName];
			}
			finally
			{
				m_lock.ExitReadLock();
			}
		}

		/// <summary>
		/// データ更新（書き込みロック）
		/// </summary>
		/// <param name="pagerXMlFilePath">ページャXMLファイルパス</param>
		/// <param name="settingName">セッティング名</param>
		/// <param name="updateDate">更新日時</param>
		private static void UpdateDatasWithWriteLock(string pagerXMlFilePath, string settingName, DateTime updateDate)
		{
			// 書き込みロックをかける
			m_lock.EnterWriteLock();
			try
			{
				var xd = new XmlDocument();
				xd.Load(pagerXMlFilePath);

				m_dicPagerSettings[settingName] = xd.SelectSingleNode("/Pager/" + settingName);
				m_dicPagerSettingUpdates[settingName] = updateDate;
			}
			finally
			{
				m_lock.ExitWriteLock();
			}

		}

		/// <summary>
		/// ページャ作成共通処理
		/// </summary>
		/// <param name="xnSetting">ページャセッティングXMLNode</param>
		/// <param name="iTotal">総件数</param>
		/// <param name="iCurrentPageNo">現在のページ番号</param>
		/// <param name="strPageUrl">ページURL</param>
		/// <param name="iDispContents">１ページ表示件数</param>
		/// <param name="iDispLinkPages">リンク表示数</param>
		/// <returns>ページャHTML</returns>
		protected static string CreatePager(XmlNode xnSetting, int iTotal, int iCurrentPageNo, string strPageUrl, int iDispContents, int iDispLinkPages)
		{
			return CreatePager(xnSetting, iTotal, iCurrentPageNo, strPageUrl, iDispContents, iDispLinkPages, Constants.REQUEST_KEY_PAGE_NO);
		}
		/// <summary>
		/// ページャ作成共通処理
		/// </summary>
		/// <param name="xnSetting">ページャセッティングXMLNode</param>
		/// <param name="iTotal">総件数</param>
		/// <param name="iCurrentPageNo">現在のページ番号</param>
		/// <param name="strPageUrl">ページURL</param>
		/// <param name="iDispContents">１ページ表示件数</param>
		/// <param name="iDispLinkPages">リンク表示数</param>
		/// <param name="strPageNoReuqestKey">ページ番号リクエストキー</param>
		/// <returns>ページャHTML</returns>
		protected static string CreatePager(XmlNode xnSetting, int iTotal, int iCurrentPageNo, string strPageUrl, int iDispContents, int iDispLinkPages, string strPageNoReuqestKey)
		{
			StringBuilder sbResult = new StringBuilder();

			//------------------------------------------------------
			// ページの記事の範囲を設定
			//------------------------------------------------------
			int iLastPage = ((iTotal - 1) / iDispContents) + 1;
			int iDispBgn = (iCurrentPageNo - 1) * iDispContents + 1;
			int iDispEnd = iCurrentPageNo * iDispContents;

			// 補正
			if (iTotal == 0)
			{
				return "";
			}
			else
			{
				iDispBgn = (iDispBgn < 1) ? 1 : iDispBgn;
				iDispEnd = (iDispEnd > iTotal) ? iTotal : iDispEnd;
			}

			//------------------------------------------------------
			// XMLテンプレート取得
			//------------------------------------------------------
			string strPageCurrentTemplate = xnSetting.SelectSingleNode("PageNumNormal").InnerText;
			string strPageLinkTemplate = xnSetting.SelectSingleNode("PageNumAnchor").InnerText;
			string strPageNumSeperator = xnSetting.SelectSingleNode("PageNumSeperator").InnerText;
			string strPageNumOmitSeperator = xnSetting.SelectSingleNode("PageNumOmitSeperator").InnerText;

			StringBuilder sbPageLinks = new StringBuilder();
			//------------------------------------------------------
			// ページリンク作成
			//------------------------------------------------------
			// １ページ目
			sbPageLinks.Append(CreatePageLink(iCurrentPageNo, 1, strPageUrl, strPageCurrentTemplate, strPageLinkTemplate, strPageNoReuqestKey));
			if (iCurrentPageNo > iDispLinkPages + 2)
			{
				sbPageLinks.Append(strPageNumOmitSeperator);
			}

			if (iLastPage != 1)
			{
				sbPageLinks.Append(strPageNumSeperator);

				// ２ページ目以降
				int iBgnLoop = ((iCurrentPageNo - iDispLinkPages) < 2) ? 2 : iCurrentPageNo - iDispLinkPages;
				int iEndLoop = ((iCurrentPageNo + iDispLinkPages) > iLastPage - 1) ? iLastPage - 1 : iCurrentPageNo + iDispLinkPages;
				for (int iLoop = iBgnLoop; iLoop <= iEndLoop; iLoop++)
				{
					sbPageLinks.Append(CreatePageLink(iCurrentPageNo, iLoop, strPageUrl, strPageCurrentTemplate, strPageLinkTemplate, strPageNoReuqestKey));
					sbPageLinks.Append(strPageNumSeperator);
				}

				// 最終ページ
				if (iCurrentPageNo < iLastPage - iDispLinkPages - 1)
				{
					sbPageLinks.Append(strPageNumOmitSeperator);
				}
				sbPageLinks.Append(CreatePageLink(iCurrentPageNo, iLastPage, strPageUrl, strPageCurrentTemplate, strPageLinkTemplate, strPageNoReuqestKey));
			}

			//------------------------------------------------------
			// 前のページ・次のページリンク作成
			//------------------------------------------------------
			string strPageBack = CreatePageLink(iCurrentPageNo, (iCurrentPageNo == 1) ? iCurrentPageNo : iCurrentPageNo - 1, strPageUrl, xnSetting.SelectSingleNode("PageBackNormal").InnerText, xnSetting.SelectSingleNode("PageBackAnchor").InnerText, strPageNoReuqestKey);
			string strPageNextBack = CreatePageLink(iCurrentPageNo, (iCurrentPageNo == iLastPage) ? iCurrentPageNo : iCurrentPageNo + 1, strPageUrl, xnSetting.SelectSingleNode("PageNextNormal").InnerText, xnSetting.SelectSingleNode("PageNextAnchor").InnerText, strPageNoReuqestKey);

			//------------------------------------------------------
			// パラメタ置換
			//------------------------------------------------------
			sbResult.Append(xnSetting.SelectSingleNode("Template").InnerText);
			sbResult.Replace("<@@PageBackLink@@>", strPageBack);
			sbResult.Replace("<@@PageNextLink@@>", strPageNextBack);
			sbResult.Replace("<@@PageNumLink@@>", sbPageLinks.ToString());
			sbResult.Replace("@@page_num@@", iCurrentPageNo.ToString());
			sbResult.Replace("@@display_max@@", iDispContents.ToString());
			sbResult.Replace("@@page_total@@", StringUtility.ToNumeric(iLastPage));
			sbResult.Replace("@@page_bgn@@", iDispBgn.ToString());
			sbResult.Replace("@@page_end@@", iDispEnd.ToString());
			sbResult.Replace("@@path_root@@", Constants.PATH_ROOT);
			sbResult.Replace("@@total_counts@@", StringUtility.ToNumeric(iTotal));

			return sbResult.ToString();
		}

		/// <summary>
		/// ページ番号リンク作成
		/// </summary>
		/// <param name="iCurrentPageNo">現在のページ番号</param>
		/// <param name="iTargetPageNo">ターゲットページ番号</param>
		/// <param name="strPageUrl">ページURL</param>
		/// <param name="strPageCurrentTemplate">ページがリンクしない場合のテンプレート</param>
		/// <param name="strPageLinkTemplate">ページリンクの場合のテンプレート</param>
		/// <param name="strPageNoRequestKey">ページ番号リクエストキー</param>
		/// <returns>ページャHTML</returns>
		private static string CreatePageLink(int iCurrentPageNo, int iTargetPageNo, string strPageUrl, string strPageCurrentTemplate, string strPageLinkTemplate, string strPageNoRequestKey)
		{
			// カレントページと同じ場合はリンクを貼らない
			if (iCurrentPageNo == iTargetPageNo)
			{
				return strPageCurrentTemplate.Replace("@@page_num@@", iTargetPageNo.ToString());
			}

			// カレントページと異なる場合はリンク作成
			StringBuilder sbPageUrlWithPageNo = new StringBuilder(strPageUrl);
			if (strPageUrl.Contains("?"))
			{
				if (strPageUrl.EndsWith("&") == false)
				{
					sbPageUrlWithPageNo.Append("&");
				}
			}
			else
			{
				sbPageUrlWithPageNo.Append("?");
			}
			sbPageUrlWithPageNo.Append(strPageNoRequestKey).Append("=").Append(iTargetPageNo);

			return strPageLinkTemplate.Replace("@@page_num@@", iTargetPageNo.ToString()).Replace("@@url@@", HtmlSanitizer.UrlAttrHtmlEncode(sbPageUrlWithPageNo.ToString()));
		}

		/// <summary>
		/// ページネーションタグ作成処理
		/// </summary>
		/// <param name="totalCount">総件数</param>
		/// <param name="currentPageNo">現在のページ番号</param>
		/// <param name="pageUrl">ページURL</param>
		/// <param name="displayCount">1ページ当たりの表示件数</param>
		public static string CreatePaginationTag(int totalCount, int currentPageNo, string pageUrl, int displayCount)
		{
			if (totalCount == 0) return string.Empty;

			var lastPageNo = ((totalCount + displayCount - 1) / displayCount);
			var previousTag = (currentPageNo > 1)
				? string.Format(Constants.CONST_PAGINATION_PREVIOUS_TAG,
					new UrlCreator(pageUrl).AddParam(Constants.REQUEST_KEY_PAGE_NO, (currentPageNo - 1).ToString()).CreateUrl()) 
				: string.Empty;
			var nextTag = (currentPageNo < lastPageNo)
				? string.Format(Constants.CONST_PAGINATION_NEXT_TAG,
					new UrlCreator(pageUrl).AddParam(Constants.REQUEST_KEY_PAGE_NO, (currentPageNo + 1).ToString()).CreateUrl())
				: string.Empty;
			return previousTag + Environment.NewLine + nextTag;
		}
	}
}
