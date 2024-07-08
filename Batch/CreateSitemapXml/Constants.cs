/*
=========================================================================================================
  Module      : サイトマップXML作成バッチ用定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.Batch.CreateSitemapXml
{
	/// <summary>
	/// サイトマップXML作成バッチ用定数定義
	/// </summary>
	public class Constants : w2.App.Common.Constants
	{
		public const int SITEMAPXML_MAX_URL_COUNT = 50000;					// sitemap.xml内のurl数上限
		public const int SITEMAPXML_MAX_STREAM_LENGTH = 10000000;			// ストリーム長の上限(バイト単位)

		// 属性
		public const string SITEMAPXML_ELE_URLSET = "urlset";				// ルート属性
		public const string SITEMAPXML_ELE_URL = "url";						// 情報セット
		public const string SITEMAPXML_ELE_LOCATION = "loc";				// ページURL
		public const string SITEMAPXML_ELE_LAST_MOD = "lastmod";			// 最終更新日
		public const string SITEMAPXML_ELE_CHANGE_FREQ = "changefreq";		// 更新頻度
		public const string SITEMAPXML_ELE_PRIORITY = "priority";			// 優先度

		public const string SITEMAPINDEXXML_ELE_SITEMAPINDEX = "sitemapindex";	// ルート属性
		public const string SITEMAPINDEXXML_ELE_SITEMAP = "sitemap";			// 情報セット
		public const string SITEMAPINDEXXML_ELE_LOCATION = "loc";				// ファイルの場所
		public const string SITEMAPINDEXXML_ELE_LAST_MOD = "lastmod";			// 最終更新日
	}
}
