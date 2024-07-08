/*
=========================================================================================================
  Module      : 新着情報キャッシュコントローラ(NewsCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.News;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// 新着情報キャッシュコントローラ
	/// </summary>
	public class NewsCacheController : DataCacheControllerBase<Dictionary<string, NewsModel[]>>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal NewsCacheController()
			: base(RefreshFileType.News)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			// Dictionary更新方法がオブジェクトの入れ替えのため、現状スレッドセーフである
			var topNewsList = DomainFacade.Instance.NewsService.GetTopNewsList();

			var newsData = topNewsList.Select(topNews => topNews.ShopId).Distinct()
				.ToDictionary(key => key, key => topNewsList.Where(topNews => topNews.ShopId == key).ToArray());

			this.CacheData = newsData;
		}

		/// <summary>
		/// Get Apply New List
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="brandId">Brand id</param>
		/// <param name="mobileDispFlg">Mobile disp flg</param>
		/// <returns>Applicable News List</returns>
		public NewsModel[] GetApplyNewsList(string shopId, string brandId, string mobileDispFlg)
		{
			if (this.CacheData.ContainsKey(shopId) == false) return null;

			return this.CacheData[shopId]
				.Where(item => ((string.IsNullOrEmpty(brandId) || (brandId == item.BrandId))
					&& (item.DisplayDateFrom <= DateTime.Now)
					&& ((item.DisplayDateTo == null) || (item.DisplayDateTo >= DateTime.Now))
					&& ((item.MobileDispFlg == Constants.FLG_NEWS_MOBILE_DISP_FLG_ALL) || (item.MobileDispFlg == mobileDispFlg)))).ToArray();
		}
	}
}
