/*
=========================================================================================================
  Module      : 配送種別キャッシュコントローラ(ShopShippingCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.ShopShipping;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// 配送種別キャッシュコントローラ
	/// </summary>
	public class ShopShippingCacheController : DataCacheControllerBase<ShopShippingModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ShopShippingCacheController()
			: base(RefreshFileType.ShopShipping)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.ShopShippingService.GetAll(Constants.CONST_DEFAULT_SHOP_ID);
		}

		/// <summary>
		/// 配送種別モデル取得
		/// </summary>
		/// <param name="shippingId">配送種別ID</param>
		/// <returns>配送種別モデル</returns>
		public ShopShippingModel Get(string shippingId)
		{
			var result = this.CacheData.FirstOrDefault(m => m.ShippingId == shippingId);
			return result;
		}
	}
}
