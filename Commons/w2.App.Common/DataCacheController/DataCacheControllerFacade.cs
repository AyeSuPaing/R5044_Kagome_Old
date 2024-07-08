/*
=========================================================================================================
  Module      : データキャッシュコントローラファサード(DataCacheControllerFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// データキャッシュコントローラファサード
	/// </summary>
	public class DataCacheControllerFacade
	{
		/// <summary>ロックオブジェクト</summary>
		private static readonly object m_lockObject = new object();
		/// <summary>データキャッシュコントローラリスト</summary>
		private static readonly Dictionary<Type, IDataCacheController> m_dataCacheControllers = new Dictionary<Type, IDataCacheController>();

		/// <summary>
		/// MemberRankCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>MemberRankCacheController</returns>
		public static MemberRankCacheController GetMemberRankCacheController()
		{
			return GetController<MemberRankCacheController>();
		}

		/// <summary>
		/// MemberRankRuleCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>MemberRankRuleCacheController</returns>
		public static MemberRankRuleCacheController GetMemberRankRuleCacheController()
		{
			return GetController<MemberRankRuleCacheController>();
		}

		/// <summary>
		/// NewsCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>NewsCacheController</returns>
		public static NewsCacheController GetNewsCacheController()
		{
			return GetController<NewsCacheController>();
		}

		/// <summary>
		/// SetPromotionCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>SetPromotionCacheController</returns>
		public static SetPromotionCacheController GetSetPromotionCacheController()
		{
			return GetController<SetPromotionCacheController>();
		}

		/// <summary>
		/// NoveltyCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>NoveltyCacheController</returns>
		public static NoveltyCacheController GetNoveltyCacheController()
		{
			return GetController<NoveltyCacheController>();
		}

		/// <summary>
		/// RecommendCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>RecommendCacheController</returns>
		public static RecommendCacheController GetRecommendCacheController()
		{
			return GetController<RecommendCacheController>();
		}

		/// <summary>
		/// SeoMetadatasCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>SeoMetadatasCacheController</returns>
		public static SeoMetadatasCacheController GetSeoMetadatasCacheController()
		{
			return GetController<SeoMetadatasCacheController>();
		}

		/// <summary>
		/// ProductListDispSettingCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>ProductListDispSettingCacheController</returns>
		public static ProductListDispSettingCacheController GetProductListDispSettingCacheController()
		{
			return GetController<ProductListDispSettingCacheController>();
		}

		/// <summary>
		/// ポイントルールのキャッシュコントローラー取得（無ければ生成）
		/// </summary>
		/// <returns>PointRuleCacheController</returns>
		public static PointRuleCacheController GetPointRuleCacheController()
		{
			return GetController<PointRuleCacheController>();
		}

		/// <summary>
		/// UserExtendSettingCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>UserExtendSettingCacheController</returns>
		public static UserExtendSettingCacheController GetUserExtendSettingCacheController()
		{
			return GetController<UserExtendSettingCacheController>();
		}

		/// <summary>
		/// ShortUrlCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>UserExtendSettingCacheController</returns>
		public static ShortUrlCacheController GetShortUrlCacheController()
		{
			return GetController<ShortUrlCacheController>();
		}

		/// <summary>
		/// MallCooperationSettingCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>UserExtendSettingCacheController</returns>
		public static MallCooperationSettingCacheController GetMallCooperationSettingCacheController()
		{
			return GetController<MallCooperationSettingCacheController>();
		}

		/// <summary>
		/// FixedPurchaseCancelReasonCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>UserExtendSettingCacheController</returns>
		public static FixedPurchaseCancelReasonCacheController GetFixedPurchaseCancelReasonCacheController()
		{
			return GetController<FixedPurchaseCancelReasonCacheController>();
		}

		/// <summary>
		/// ProductGroupCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>ProductGroupCacheController</returns>
		public static ProductGroupCacheController GetProductGroupCacheController()
		{
			return GetController<ProductGroupCacheController>();
		}

		/// <summary>
		/// ProductColorCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>ProductColorCacheController</returns>
		public static ProductColorCacheController GetProductColorCacheController()
		{
			return GetController<ProductColorCacheController>();
		}

		/// <summary>
		/// AutoTranslationWordCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>AutoTranslationWordCacheController</returns>
		public static AutoTranslationWordCacheController GetAutoTranslationWordCacheController()
		{
			return GetController<AutoTranslationWordCacheController>();
		}

		/// <summary>
		/// CountryLocationCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>CountryLocationCacheController</returns>
		public static CountryLocationCacheController GetCountryLocationCacheController()
		{
			return GetController<CountryLocationCacheController>();
		}

		/// <summary>
		/// AffiliateTagSettingCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>GetAffiliateTagSettingCacheController</returns>
		public static AffiliateTagSettingCacheController GetAffiliateTagSettingCacheController()
		{
			return GetController<AffiliateTagSettingCacheController>();
		}

		/// <summary>
		/// AdvCodeCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>GetAdvCodeCacheController</returns>
		public static AdvCodeCacheController GetAdvCodeCacheController()
		{
			return GetController<AdvCodeCacheController>();
		}

		/// <summary>
		/// PageDesignCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>PageDesignCacheController</returns>
		public static PageDesignCacheController GetPageDesignCacheController()
		{
			return GetController<PageDesignCacheController>();
		}

		/// <summary>
		/// PartsDesignCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>PartsDesignCacheController</returns>
		public static PartsDesignCacheController GetPartsDesignCacheController()
		{
			return GetController<PartsDesignCacheController>();
		}

		/// <summary>
		/// Feature page design cache controller
		/// </summary>
		/// <returns>Feature page design cache controller</returns>
		public static FeaturePageCacheController GetFeaturePageCacheController()
		{
			return GetController<FeaturePageCacheController>();
		}

		/// <summary>
		/// FeatureAreaBannerCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>FeatureAreaBannerCacheController</returns>
		public static FeatureAreaBannerCacheController GetFeatureAreaBannerCacheController()
		{
			return GetController<FeatureAreaBannerCacheController>();
		}

		/// <summary>
		/// OgpTagSettingCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>OgpTagSettingCacheController</returns>
		public static OgpTagSettingCacheController GetOgpTagSettingCacheController()
		{
			return GetController<OgpTagSettingCacheController>();
		}

		/// <summary>
		/// LandingPageDesignCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>LandingPageDesignCacheController</returns>
		public static LandingPageDesignCacheController GetLandingPageDeCacheController()
		{
			return GetController<LandingPageDesignCacheController>();
		}

		/// <summary>
		/// GetShopShippingDeCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>GetShopShippingDeCacheController</returns>
		public static ShopShippingCacheController GetShopShippingCacheController()
		{
			return GetController<ShopShippingCacheController>();
		}

		/// <summary>
		/// OrderExtendSettingCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>OrderExtendSettingCacheController</returns>
		public static OrderExtendSettingCacheController GetOrderExtendSettingCacheController()
		{
			return GetController<OrderExtendSettingCacheController>();
		}

		/// <summary>
		/// GetPaymentCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>GetPaymentCacheController</returns>
		public static PaymentCacheController GetPaymentCacheController()
		{
			return GetController<PaymentCacheController>();
		}

		/// <summary>
		/// Get real shop area cache controller
		/// </summary>
		/// <returns>Real shop area cache</returns>
		public static RealShopAreaCacheController GetRealShopAreaCacheController()
		{
			return GetController<RealShopAreaCacheController>();
		}

		/// <summary>
		/// SubscriptionBoxCacheController取得（無ければ生成）
		/// </summary>
		/// <returns>SubscriptionBoxCacheController</returns>
		public static SubscriptionBoxCacheController GetSubscriptionBoxCacheController()
		{
			return GetController<SubscriptionBoxCacheController>();
		}

		/// <summary>
		/// Get exchange rate cache controller
		/// </summary>
		/// <returns>Exchange rate cache controller</returns>
		public static ExchangeRateCacheController GetExchangeRateCacheController()
		{
			return GetController<ExchangeRateCacheController>();
		}

		/// <summary>
		/// データキャッシュコントローラ取得（無ければ生成）
		/// </summary>
		/// <returns>データキャッシュプロバイダ</returns>
		private static T GetController<T>()
			where T : IDataCacheController
		{
			lock (m_lockObject)
			{
				var type = typeof(T);
				if (m_dataCacheControllers.ContainsKey(type) == false)
				{
					var controller = (IDataCacheController)Activator.CreateInstance(type, true);
					m_dataCacheControllers.Add(type, controller);
				}
				return (T)m_dataCacheControllers[type];
			}
		}
	}
}