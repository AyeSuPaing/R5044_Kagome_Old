/*
=========================================================================================================
  Module      : リフレッシュファイルマネージャプロバイダクラス(RefreshFileManagerProvider.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace w2.App.Common.RefreshFileManager
{
	/// <summary>
	/// リフレッシュファイルマネージャプロバイダ
	/// </summary>
	public static class RefreshFileManagerProvider
	{
		/// <summary>キャッシュ</summary>
		private static readonly Dictionary<string, RefreshFileManager> m_cache = new Dictionary<string, RefreshFileManager>();

		/// <summary>キャッシュのロックオブジェクト</summary>
		private static readonly ReaderWriterLockSlim m_cacheLock = new ReaderWriterLockSlim();

		/// <summary>
		/// インスタンス取得
		/// </summary>
		/// <param name="type">リフレッシュファイルタイプ</param>
		/// <returns>インスタンス</returns>
		public static RefreshFileManager GetInstance(RefreshFileType type)
		{
			switch (type)
			{
				case RefreshFileType.DisplayProduct:
					return GetInstance(Constants.FILENAME_DISPLAYPRODUCT_REFRESH);

				case RefreshFileType.MemberRank:
					return GetInstance(Constants.FILENAME_MEMBERRANK_REFRESH);

				case RefreshFileType.News:
					return GetInstance(Constants.FILENAME_NEWS_REFRESH);

				case RefreshFileType.PointRules:
					return GetInstance(Constants.FILENAME_POINTRULES_REFRESH);

				case RefreshFileType.SetPromotion:
					return GetInstance(Constants.FILENAME_SETPROMOTION_REFRESH);

				case RefreshFileType.Novelty:
					return GetInstance(Constants.FILENAME_NOVELTY_REFRESH);

				case RefreshFileType.Recommend:
					return GetInstance(Constants.FILENAME_RECOMMEND_REFRESH);

				case RefreshFileType.SeoMetadatas:
					return GetInstance(Constants.FILENAME_SEO_METADATDAS_REFRESH);

				case RefreshFileType.ProductListDispSetting:
					return GetInstance(Constants.FILENAME_PRODUCT_LIST_DISP_SETTING_REFRESH);

				case RefreshFileType.UserExtendSetting:
					return GetInstance(Constants.FILENAME_USER_EXTEND_SETTING_REFRESH);

				case RefreshFileType.ShortUrl:
					return GetInstance(Constants.FILENAME_SHORTURL_REFRESH);

				case RefreshFileType.FixedPurchaseCancelReason:
					return GetInstance(Constants.FILENAME_FIXEDPURCHASE_CANCEL_REASON_REFRESH);

				case RefreshFileType.ProductGroup:
					return GetInstance(Constants.FILENAME_PRODUCTGROUP_REFRESH);

				case RefreshFileType.ProductColor:
					return GetInstance(Constants.PHYSICALDIRPATH_FRONT_PC_XML, Constants.FILENAME_COLOR_REFRESH);

				case RefreshFileType.AutoTranslationWord:
					return GetInstance(Constants.FILENAME_AUTO_TRANSLATION_WORD_REFRESH);

				case RefreshFileType.CountryLocation:
					return GetInstance(Constants.FILENAME_COUNTRY_LOCATION_REFRESH);

				case RefreshFileType.AffiliateTagSetting:
					return GetInstance(Constants.FILENAME_AFFILIATE_TAG_SETTING_REFRESH);

				case RefreshFileType.AdvCode:
					return GetInstance(Constants.FILENAME_ADVCODE_REFRESH);

				case RefreshFileType.PageDesign:
					return GetInstance(Constants.FILENAME_PAGE_DESIGN_REFRESH);

				case RefreshFileType.PartsDesign:
					return GetInstance(Constants.FILENAME_PARTS_DESIGN_REFRESH);

				case RefreshFileType.FeaturePage:
					return GetInstance(Constants.FILENAME_FEATURE_PAGE_DESIGN_REFRESH);

				case RefreshFileType.FeatureAreaBanner:
					return GetInstance(Constants.FILENAME_FEATURE_AREA_BANNER_REFRESH);

				case RefreshFileType.OgpTagSetting:
					return GetInstance(Constants.FILENAME_OGPTAGSETTING_REFRESH);

				case RefreshFileType.LandingPageDesign:
					return GetInstance(Constants.FILENAME_LANDINGPAGEDESIGN_REFRESH);

				case RefreshFileType.ShopShipping:
					return GetInstance(Constants.FILENAME_SHOPSHIPPING_REFRESH);

				case RefreshFileType.OrderExtendSetting:
					return GetInstance(Constants.FILENAME_ORDEREXTENDSETTING_REFRESH);

				case RefreshFileType.Payment:
					return GetInstance(Constants.FILENAME_PAYMENT_REFRESH);

				case RefreshFileType.RealShopArea:
					return GetInstance(Constants.PHYSICALDIRPATH_FRONT_PC_XML, Constants.FILENAME_REALSHOP_AREA_REFRESH);

				case RefreshFileType.Maintenance:
					return GetInstance(Constants.FILENAME_MAINTENANCE_REFRESH);

				case RefreshFileType.ScoringSale:
					return GetInstance(Constants.FILENAME_SCORINGSALE_REFRESH);

				case RefreshFileType.SubscriptionBox:
					return GetInstance(Constants.FILENAME_SUBSCRIPTIONBOX_REFRESH);

				case RefreshFileType.ExchangeRate:
					return GetInstance(Constants.FILENAME_EXCHANGERATE_REFRESH);

				case RefreshFileType.RealShop:
					return GetInstance(Constants.FILENAME_REALSHOP_REFRESH);

				default:
					throw new Exception("未定義のリフレッシュタイプ：" + type.ToString());
			}
		}

		/// <summary>
		/// インスタンスを取得（キャッシュになければ生成）
		/// </summary>
		/// <param name="fileName">リフレッシュファイル名</param>
		/// <returns>インスタンス</returns>
		private static RefreshFileManager GetInstance(string fileName)
		{
			var instance = GetFromCache(fileName);
			if (instance != null) return instance;

			instance = new RefreshFileManager(fileName);
			SetToCache(fileName, instance);
			return instance;
		}
		/// <summary>
		/// インスタンスを取得（キャッシュになければ生成）（リフレッシュファイルを作成しない）
		/// </summary>
		/// <param name="dirPath">監視対象ディレクトリパス</param>
		/// <param name="fileName">監視対象ファイル名（ワイルドカード可）</param>
		/// <returns>インスタンス</returns>
		private static RefreshFileManager GetInstance(string dirPath, string fileName)
		{
			var instance = GetFromCache(Path.Combine(dirPath, fileName));
			if (instance != null) return instance;

			instance = new RefreshFileManager(dirPath, fileName, false);
			SetToCache(Path.Combine(dirPath, fileName), instance);
			return instance;
		}

		/// <summary>
		/// キャッシュから取得
		/// </summary>
		/// <param name="fileName">リフレッシュファイル名</param>
		/// <returns>インスタンス</returns>
		private static RefreshFileManager GetFromCache(string fileName)
		{
			m_cacheLock.EnterReadLock();
			try
			{
				if (m_cache.ContainsKey(fileName)) return m_cache[fileName];
			}
			finally
			{
				m_cacheLock.ExitReadLock();
			}
			return null;
		}

		/// <summary>
		/// キャッシュへセット
		/// </summary>
		/// <param name="fileName">リフレッシュファイル名</param>
		/// <param name="instance">インスタンス</param>
		private static void SetToCache(string fileName, RefreshFileManager instance)
		{
			m_cacheLock.EnterWriteLock();
			try
			{
				if (m_cache.ContainsKey(fileName)) m_cache[fileName] = instance;
				m_cache.Add(fileName, instance);
			}
			finally
			{
				m_cacheLock.ExitWriteLock();
			}
		}
	}
}
