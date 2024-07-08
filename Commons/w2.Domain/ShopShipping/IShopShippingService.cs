/*
=========================================================================================================
  Module      : 店舗配送種別サービスのインタフェース(IShopShippingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.ShopShipping
{
	/// <summary>
	/// 店舗配送種別サービスのインタフェース
	/// </summary>
	public interface IShopShippingService : IService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="accessor">sqlアクセサー</param>
		/// <returns>モデル</returns>
		ShopShippingModel Get(string shopId, string shippingId, SqlAccessor accessor = null);

		/// <summary>
		/// 全件取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		ShopShippingModel[] GetAll(string shopId);

		/// <summary>
		/// 郵便番号から取得(配送料情報含む)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="shippingZoneNo">配送料地帯区分</param>
		/// <param name="zip">郵便番号</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="minShippingZoneNo">Min shipping zone no</param>
		/// <returns>モデル</returns>
		ShopShippingModel GetFromZipcode(
			string shopId,
			string shippingId,
			int shippingZoneNo,
			string zip,
			string deliveryCompanyId,
			int minShippingZoneNo = 47);

		/// <summary>
		/// 初期配送会社取得
		/// </summary>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="shippingKbn">配送区分</param>
		/// <returns>モデル</returns>
		ShopShippingCompanyModel GetDefaultCompany(string shippingId, string shippingKbn);

		/// <summary>
		/// 配送会社登録
		/// </summary>
		/// <param name="accessor">アクセッサ</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="companyList">配送会社リスト</param>
		void InsertCompany(SqlAccessor accessor, string shippingId, ShopShippingCompanyModel[] companyList);

		/// <summary>
		/// 配送会社登録
		/// </summary>
		/// <param name="accessor">アクセッサ</param>
		/// <param name="shippingId">配送種別ID</param>
		void DeleteCompany(SqlAccessor accessor, string shippingId);

		/// <summary>
		/// delivery_company_idに紐づくすべての配送種別配送会社情報を取得
		/// </summary>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>モデル列</returns>
		ShopShippingCompanyModel[] GetShippingCompanyByDeliveryCompanyId(string deliveryCompanyId, SqlAccessor accessor);

		/// <summary>
		/// 店舗配送料地帯情報を登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		void InsertShopShippingZone(ShopShippingZoneModel model, SqlAccessor accessor = null);

		/// <summary>
		/// 配送種別に紐づくすべての店舗配送料地帯情報を登録
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="models">モデル配列</param>
		/// <param name="accessor">SQLアクセサ</param>
		void InsertShopShippingZones(
			string shopId,
			string shippingId,
			ShopShippingZoneModel[] models,
			SqlAccessor accessor = null);

		/// <summary>
		/// 配送種別IDに紐づくすべての店舗配送料地帯情報を削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		void DeleteShopShippingZoneByShippingId(string shopId, string shippingId, SqlAccessor accessor = null);

		/// <summary>
		/// 配送種別IDに紐づくすべての店舗配送料地帯情報を削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="zoneNo">地帯区分</param>
		/// <param name="accessor">SQLアクセサ</param>
		void DeleteShopShippingZoneByShippingIdAndZone(
			string shopId,
			string shippingId,
			string zoneNo,
			SqlAccessor accessor = null);

		/// <summary>
		/// Get shipping names by shipping ids
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="shippingIds">Shipping ids</param>
		/// <returns>Shipping names</returns>
		string[] GetShippingNamesByShippingIds(string shopId, string[] shippingIds);

		/// <summary>
		/// Get all shop shippings
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <returns>Shop shippings</returns>
		ShopShippingModel[] GetAllShopShippings(string shopId);

		/// <summary>
		/// Get shop shipping
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="shippingId">Shipping id</param>
		/// <returns>Shop shipping</returns>
		ShopShippingModel GetShopShipping(string shopId, string shippingId);

		/// <summary>
		/// 配送情報から配送不可エリア郵便番号取得
		/// </summary>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>配送不可郵便番号</returns>
		string GetUnavailableShippingZipFromShippingDelivery(
			string shippingId,
			string deliveryCompanyId,
			SqlAccessor accessor = null);
	}
}
