/*
=========================================================================================================
  Module      : リアル店舗情報サービスのインタフェース (IRealShopService.cs)
  ････････････････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.RealShop
{
	/// <summary>
	/// リアル店舗情報サービスのインタフェース
	/// </summary>
	public interface IRealShopService : IService
	{
		/// <summary>
		/// Get real shops
		/// </summary>
		/// <param name="areaId">Area id</param>
		/// <param name="brandId">Brand id</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Array of real shop model</returns>
		RealShopModel[] GetRealShops(string areaId, string brandId, SqlAccessor accessor = null);

		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>モデル</returns>
		RealShopModel[] GetAll();

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="realShopId">リアル店舗ID</param>
		/// <returns>モデル</returns>
		RealShopModel Get(string realShopId);
	}
}
