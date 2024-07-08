/*
=========================================================================================================
  Module      : ページャユーティリティモジュール(PagerUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using w2.Common.Util;
using w2.Common.Web;

namespace w2.Cms.Manager.Codes
{
	/// <summary>
	/// ページャのHTMLを出力する
	/// </summary>
	public class PagerCreator
	{
		/// <summary>ページャXMLファイルパス</summary>
		private readonly string m_pagerXMlFilePath = null;
		/// <summary>テンプレート格納Dictionary</summary>
		private static readonly Dictionary<string, PagerSetting> m_pagerSettings = new Dictionary<string, PagerSetting>();
		/// <summary>更新日付格納Dictionary</summary>
		private static readonly Dictionary<string, DateTime> m_pagerSettingUpdateTimes = new Dictionary<string, DateTime>();
		/// <summary>ReaderWriterLockSlimオブジェクト</summary>
		private static readonly ReaderWriterLockSlim m_lockObj = new ReaderWriterLockSlim();
		/// <summary>ページャXMLファイルパス</summary>
		private readonly string m_pageNoRequestKey = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="pagerXmlFilePath">ページャXMLファイルパス</param>
		/// <param name="pageNoRequestkey">ページ番号リクエストキー名</param>
		public PagerCreator(string pagerXmlFilePath, string pageNoRequestkey = Constants.REQUEST_KEY_PAGE_NO)
		{
			m_pagerXMlFilePath = pagerXmlFilePath;
			m_pageNoRequestKey = pageNoRequestkey;
		}

		/// <summary>
		/// ページャ作成共通処理
		/// </summary>
		/// <param name="settingName">ページャセッティング名</param>
		/// <param name="totalCount">総件数</param>
		/// <param name="currentPageNo">現在のページ番号</param>
		/// <param name="pageUrl">ページURL</param>
		/// <param name="dispContents">１ページ表示件数</param>
		/// <param name="dispLinkPageCount">リンク表示数</param>
		/// <returns>ページャHTML</returns>
		public string CreatePager(
			string settingName,
			int totalCount,
			int currentPageNo,
			string pageUrl,
			int dispContents,
			int dispLinkPageCount)
		{
			var pagerHtml = CreatePager(
				settingName,
				totalCount,
				currentPageNo,
				pageUrl,
				dispContents,
				dispLinkPageCount,
				m_pageNoRequestKey);
			return pagerHtml;
		}

		/// <summary>
		/// ページャ作成共通処理
		/// </summary>
		/// <param name="settingName">ページャセッティング名</param>
		/// <param name="totalCount">総件数</param>
		/// <param name="currentPageNo">現在のページ番号</param>
		/// <param name="pageUrl">ページURL</param>
		/// <param name="dispContents">１ページ表示件数</param>
		/// <param name="dispLinkPageCount">リンク表示数</param>
		/// <param name="pageNoReuqestKey">ページ番号リクエストキー</param>
		/// <returns>ページャHTML</returns>
		public string CreatePager(
			string settingName,
			int totalCount,
			int currentPageNo,
			string pageUrl,
			int dispContents,
			int dispLinkPageCount,
			string pageNoReuqestKey)
		{
			// ページの記事の範囲を設定
			var lastPage = ((totalCount - 1) / dispContents) + 1;
			var dispBgn = (currentPageNo - 1) * dispContents + 1;
			var dispEnd = currentPageNo * dispContents;

			// 補正
			if (totalCount == 0) return "";
			dispBgn = (dispBgn < 1) ? 1 : dispBgn;
			dispEnd = (dispEnd > totalCount) ? totalCount : dispEnd;

			// XMLテンプレート取得
			var setting = GetPagerSetting(m_pagerXMlFilePath, settingName);

			// ページリンク作成
			// １ページ目
			var pageLinks = new StringBuilder();
			pageLinks.Append(
				CreatePageLink(
					currentPageNo,
					1,
					pageUrl,
					setting.PageNumNormal,
					setting.PageNumAnchor,
					pageNoReuqestKey));
			if (currentPageNo > dispLinkPageCount + 2) pageLinks.Append(setting.PageNumOmitSeperator);
			if (lastPage != 1)
			{
				pageLinks.Append(setting.PageNumSeperator);

				// ２ページ目以降
				var begin = ((currentPageNo - dispLinkPageCount) < 2) ? 2 : currentPageNo - dispLinkPageCount;
				var end = ((currentPageNo + dispLinkPageCount) > lastPage - 1)
					? lastPage - 1
					: currentPageNo + dispLinkPageCount;
				for (var i = begin; i <= end; i++)
				{
					pageLinks.Append(
						CreatePageLink(
							currentPageNo,
							i,
							pageUrl,
							setting.PageNumNormal,
							setting.PageNumAnchor,
							pageNoReuqestKey));
					pageLinks.Append(setting.PageNumSeperator);
				}

				// 最終ページ
				if (currentPageNo < lastPage - dispLinkPageCount - 1)
				{
					pageLinks.Append(setting.PageNumOmitSeperator);
				}

				pageLinks.Append(
					CreatePageLink(
						currentPageNo,
						lastPage,
						pageUrl,
						setting.PageNumNormal,
						setting.PageNumAnchor,
						pageNoReuqestKey));
			}

			// 前のページ・次のページリンク作成
			var pageBack = CreatePageLink(
				currentPageNo,
				(currentPageNo == 1) ? currentPageNo : currentPageNo - 1,
				pageUrl,
				setting.PageBackNormal,
				setting.PageBackAnchor,
				pageNoReuqestKey);
			var pageNextBack = CreatePageLink(
				currentPageNo,
				(currentPageNo == lastPage) ? currentPageNo : currentPageNo + 1,
				pageUrl,
				setting.PageNextNormal,
				setting.PageNextAnchor,
				pageNoReuqestKey);

			// パラメタ置換
			var pagerHtml = setting.Template
				.Replace("<@@PageBackLink@@>", pageBack)
				.Replace("<@@PageNextLink@@>", pageNextBack)
				.Replace("<@@PageNumLink@@>", pageLinks.ToString())
				.Replace("@@page_num@@", currentPageNo.ToString())
				.Replace("@@display_max@@", dispContents.ToString())
				.Replace("@@page_total@@", StringUtility.ToNumeric(lastPage))
				.Replace("@@page_bgn@@", dispBgn.ToString())
				.Replace("@@page_end@@", dispEnd.ToString())
				.Replace("@@path_root@@", Constants.PATH_ROOT)
				.Replace("@@total_counts@@", StringUtility.ToNumeric(totalCount));
			return pagerHtml;
		}

		/// <summary>
		/// ページャ設定XMLノード取得
		/// </summary>
		/// <param name="pagerXMlFilePath">ページャXMLファイルパス</param>
		/// <param name="settingName">セッティング名</param>
		/// <returns>ページャ設定</returns>
		private PagerSetting GetPagerSetting(string pagerXMlFilePath, string settingName)
		{
			var updateDate = File.GetLastWriteTime(pagerXMlFilePath);

			// 更新判定
			var doUpdate = false;
			using (new ReadLockGetter(m_lockObj))
			{
				doUpdate = ((m_pagerSettingUpdateTimes.ContainsKey(settingName) == false)
					|| (m_pagerSettingUpdateTimes[settingName] != updateDate));
			}

			// 更新処理（書き込みロック）
			if (doUpdate)
			{
				using (new WriteLockGetter(m_lockObj))
				{
					//var xd = new XmlDocument();
					//xd.Load(pagerXMlFilePath);

					//var serializer = new XmlSerializer(typeof(PagerSetting));
					//var pager = (PagerSetting)serializer.Deserialize(fs);

					var serializer = new XmlSerializer(typeof(PagerSettings));
					using (var sr = new StreamReader(pagerXMlFilePath, new UTF8Encoding(false)))
					{
						var setting = (PagerSettings)serializer.Deserialize(sr);

						m_pagerSettings[settingName] = setting.DefaultListPagerSetting;
						
						m_pagerSettingUpdateTimes[settingName] = updateDate;
					}
				}
			}

			// 返却
			using (new ReadLockGetter(m_lockObj))
			{
				var result = m_pagerSettings[settingName];
				return result;
			}
		}

		/// <summary>
		/// ページ番号リンク作成
		/// </summary>
		/// <param name="currentPageNo">現在のページ番号</param>
		/// <param name="targetPageNo">ターゲットページ番号</param>
		/// <param name="pageUrl">ページURL</param>
		/// <param name="pageCurrentTemplate">ページがリンクしない場合のテンプレート</param>
		/// <param name="pageLinkTemplate">ページリンクの場合のテンプレート</param>
		/// <param name="pageNoRequestKey">ページ番号リクエストキー</param>
		/// <returns>ページャHTML</returns>
		private string CreatePageLink(
			int currentPageNo,
			int targetPageNo,
			string pageUrl,
			string pageCurrentTemplate,
			string pageLinkTemplate,
			string pageNoRequestKey)
		{
			// カレントページと同じ場合はリンクを貼らない
			if (currentPageNo == targetPageNo) return pageCurrentTemplate.Replace("@@page_num@@", targetPageNo.ToString());

			// カレントページと異なる場合はリンク作成
			var pageUrlWithPageNo = new StringBuilder(pageUrl);
			if (pageUrl.Contains("?"))
			{
				if (pageUrl.EndsWith("&") == false) pageUrlWithPageNo.Append("&");
			}
			else
			{
				pageUrlWithPageNo.Append("?");
			}
			pageUrlWithPageNo.Append(pageNoRequestKey).Append("=").Append(targetPageNo);

			var pagerHtml = pageLinkTemplate.Replace("@@page_num@@", targetPageNo.ToString()).Replace(
				"@@url@@",
				HtmlSanitizer.UrlAttrHtmlEncode(pageUrlWithPageNo.ToString()));
			return pagerHtml;
		}

		/// <summary>
		/// ページネーションタグ作成処理
		/// </summary>
		/// <param name="totalCount">総件数</param>
		/// <param name="currentPageNo">現在のページ番号</param>
		/// <param name="pageUrl">ページURL</param>
		/// <param name="displayCount">1ページ当たりの表示件数</param>
		public string CreatePaginationTag(int totalCount, int currentPageNo, string pageUrl, int displayCount)
		{
			if (totalCount == 0) return string.Empty;

			var lastPageNo = ((totalCount + displayCount - 1) / displayCount);
			var previousTag = (currentPageNo > 1)
				? string.Format(
					Constants.CONST_PAGINATION_PREVIOUS_TAG,
					new UrlCreator(pageUrl).AddParam(m_pageNoRequestKey, (currentPageNo - 1).ToString())
						.CreateUrl())
				: string.Empty;
			var nextTag = (currentPageNo < lastPageNo)
				? string.Format(
					Constants.CONST_PAGINATION_NEXT_TAG,
					new UrlCreator(pageUrl).AddParam(m_pageNoRequestKey, (currentPageNo + 1).ToString())
						.CreateUrl())
				: string.Empty;
			return previousTag + Environment.NewLine + nextTag;
		}

		[XmlRoot("Pager")]
		public class PagerSettings
		{
			[XmlElement("DefaultListPagerSetting")]
			public PagerSetting DefaultListPagerSetting { get; set; }
		}

		/// <summary>
		/// ページャーセッティング
		/// </summary>
		public class PagerSetting
		{
			/// <summary>前へリンク（リンクなし）</summary>
			[XmlElement("PageBackNormal")]
			public string PageBackNormal { get; set; }
			/// <summary>前へリンク（リンクあり）</summary>
			[XmlElement("PageBackAnchor")]
			public string PageBackAnchor { get; set; }
			/// <summary>次へリンク（リンクなし）</summary>
			[XmlElement("PageNextNormal")]
			public string PageNextNormal { get; set; }
			/// <summary>次へリンク（リンクあり）</summary>
			[XmlElement("PageNextAnchor")]
			public string PageNextAnchor { get; set; }
			/// <summary>数字リンク（リンクなし）</summary>
			[XmlElement("PageNumNormal")]
			public string PageNumNormal { get; set; }
			/// <summary>数字リンク（リンクあり）</summary>
			[XmlElement("PageNumAnchor")]
			public string PageNumAnchor { get; set; }
			/// <summary>数字リンク（セパレータ）</summary>
			[XmlElement("PageNumSeperator")]
			public string PageNumSeperator { get; set; }
			/// <summary>数字リンク（省略セパレータ）></summary>
			[XmlElement("PageNumOmitSeperator")]
			public string PageNumOmitSeperator { get; set; }
			/// <summary>ページャーデザインテンプレート</summary>
			[XmlElement("Template")]
			public string Template { get; set; }
		}

		/// <summary>
		/// 読み込みロックゲッター★w2.Common移動？
		/// </summary>
		private class ReadLockGetter : IDisposable
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="lockObj">ロックオブジェクト</param>
			public ReadLockGetter(ReaderWriterLockSlim lockObj)
			{
				this.LockObj = lockObj;
				this.LockObj.EnterReadLock();
			}

			/// <summary>
			/// 破棄
			/// </summary>
			public void Dispose()
			{
				this.LockObj.ExitReadLock();
			}

			/// <summary>ロックオブジェクト</summary>
			private ReaderWriterLockSlim LockObj { get; set; }
		}

		/// <summary>
		/// 書き込みロックゲッター★w2.Common移動？
		/// </summary>
		private class WriteLockGetter : IDisposable
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="lockObj">ロックオブジェクト</param>
			public WriteLockGetter(ReaderWriterLockSlim lockObj)
			{
				this.LockObj = lockObj;
				this.LockObj.EnterWriteLock();
			}

			/// <summary>
			/// 破棄
			/// </summary>
			public void Dispose()
			{
				m_lockObj.ExitWriteLock();
			}

			/// <summary>ロックオブジェクト</summary>
			private ReaderWriterLockSlim LockObj { get; set; }
		}
	}
}
