/*
=========================================================================================================
  Module      : リアル店舗キャッシュコントローラ(RealShopCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.RealShop;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// リアル店舗キャッシュコントローラ
	/// </summary>
	public class RealShopCacheController : DataCacheControllerBase<RealShopModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RealShopCacheController() : base(RefreshFileType.RealShop)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.RealShopService.GetAll();
		}

		/// <summary>
		/// リアル店舗 有効なもの全取得
		/// </summary>
		/// <returns>リアル店舗モデルリスト</returns>
		public RealShopModel[] GetRealShopModels()
		{
			return this.CacheData.Where(realShop => realShop.IsValid).ToArray();
		}

		/// <summary>
		/// リアル店舗取得
		/// </summary>
		/// <param name="realShopId">リアル店舗ID</param>
		/// <returns>リアル店舗</returns>
		public RealShopModel GetRealShopModel(string realShopId)
		{
			return this.CacheData.FirstOrDefault(realShop => realShop.IsValid && (realShop.RealShopId == realShopId));
		}
	}
}
