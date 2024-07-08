/*
=========================================================================================================
  Module      : ショートURLキャッシュコントローラ(ShortUrlCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.ShortUrl;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// ショートURLキャッシュコントローラ
	/// </summary>
	public class ShortUrlCacheController : DataCacheControllerBase<ShortUrlModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ShortUrlCacheController()
			: base(RefreshFileType.ShortUrl)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.ShortUrlService.GetAll(Constants.CONST_DEFAULT_SHOP_ID).ToArray();
		}

		/// <summary>
		/// ロングURL取得
		/// </summary>
		/// <param name="shortUrl">ショートURL</param>
		/// <returns>ロングURL</returns>
		public string GetLongUrl(string shortUrl)
		{
			var shorUrl = this.CacheData.Where(s => s.ShortUrl.ToLower() == shortUrl.ToLower());
			if (shorUrl.Any()) return shorUrl.First().LongUrl;

			return null;
		}
	}
}