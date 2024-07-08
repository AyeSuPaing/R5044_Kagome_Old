/*
=========================================================================================================
  Module      : 店舗配送種別サービス (ShopShippingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Transactions;
using w2.Common.Sql; // TODO: 本来は呼ばないほうがいい。

namespace w2.Domain.ShopShipping
{
	/// <summary>
	/// 店舗配送種別サービス
	/// </summary>
	public class ShopShippingService : ServiceBase, IShopShippingService
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="accessor">sqlアクセサー</param>
		/// <returns>モデル</returns>
		public ShopShippingModel Get(string shopId, string shippingId, SqlAccessor accessor = null)
		{
			using (var repository = (accessor == null) ? new ShopShippingRepository() : new ShopShippingRepository(accessor))
			{
				// 取得
				var model = repository.Get(shopId, shippingId);
				if (model == null) return null;

				// 配送会社ごとの配送料マスタ
				model.CompanyPostageSettings = repository.GetShippingDeliveryPostageAll(shopId, shippingId);

				// 地域
				model.ZoneList = repository.GetZoneAll(shopId, shippingId);

				// 配送会社
				model.CompanyList = repository.GetCompanyAll(shopId, shippingId);

				return model;
			}
		}
		#endregion

		#region +GetAsStatic 取得(static版)
		/// <summary>
		/// 取得(static版)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <returns>店舗配送種別情報</returns>
		public static ShopShippingModel GetAsStatic(string shopId, string shippingId)
		{
			using (var repository = new ShopShippingRepository())
			{
				// 取得
				var model = repository.Get(shopId, shippingId);
				if (model == null) return null;

				// 配送会社ごとの配送料マスタ
				model.CompanyPostageSettings = repository.GetShippingDeliveryPostageAll(shopId, shippingId);

				// 地域
				model.ZoneList = repository.GetZoneAll(shopId, shippingId);

				// 配送会社
				model.CompanyList = repository.GetCompanyAll(shopId, shippingId);

				return model;
			}
		}
		#endregion

		#region +GetOnlyModel 取得
		/// <summary>
		/// 取得(配送料、地域、配送会社は取得しない)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="accessor">sqlアクセサー</param>
		/// <returns>モデル</returns>
		public ShopShippingModel GetOnlyModel(string shopId, string shippingId, SqlAccessor accessor = null)
		{
			using (var repository = new ShopShippingRepository(accessor))
			{
				var model = repository.Get(shopId, shippingId);
				return model;
			}
		}
		#endregion

		#region +GetAll 全件取得
		/// <summary>
		/// 全件取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		public ShopShippingModel[] GetAll(string shopId)
		{
			using (var repository = new ShopShippingRepository())
			{
				var models = repository.GetAll(shopId);

				// 配送会社ごとの配送料マスタ、地域、配送会社
				foreach (var model in models)
				{
					// 配送会社ごとの配送料マスタ、地域、配送会社
					model.CompanyPostageSettings =
						repository.GetShippingDeliveryPostageAll(model.ShopId, model.ShippingId);
					// 地域
					model.ZoneList = repository.GetZoneAll(model.ShopId, model.ShippingId);
					// 配送会社
					model.CompanyList = repository.GetCompanyAll(model.ShopId, model.ShippingId);
				}
				return models;
			}
		}
		#endregion

		#region +GetShippingInfoByShopId 取得(ドロップダウンリスト用)
		/// <summary>
		/// 取得(ドロップダウンリスト用)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		public ShopShippingModel[] GetShippingInfoByShopId(string shopId)
		{
			using (var repository = new ShopShippingRepository())
			{
				var models = repository.GetShippingInfoByShopId(shopId);
				return models;
			}
		}
		#endregion

		#region +GetFromZipcode 郵便番号から取得(配送料情報含む)
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
		public ShopShippingModel GetFromZipcode(
			string shopId,
			string shippingId,
			int shippingZoneNo,
			string zip,
			string deliveryCompanyId,
			int minShippingZoneNo = 47)
		{
			using (var repository = new ShopShippingRepository())
			{
				var model = repository.GetFromZipcode(
					shopId,
					shippingId,
					shippingZoneNo,
					zip,
					deliveryCompanyId,
					minShippingZoneNo);
				return model;
			}
		}
		#endregion

		#region +GetDefaultCompany 初期配送会社取得
		/// <summary>
		/// 初期配送会社取得
		/// </summary>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="shippingKbn">配送区分</param>
		/// <returns>モデル</returns>
		public ShopShippingCompanyModel GetDefaultCompany(string shippingId, string shippingKbn)
		{
			using (var repository = new ShopShippingRepository())
			{
				var model = repository.GetDefaultCompany(shippingId, shippingKbn);
				return model;
			}
		}
		#endregion

		#region +GetUnavailableShippingZipFromShippingDelivery 配送情報から配送不可エリア郵便番号取得
		/// <summary>
		/// 配送情報から配送不可エリア郵便番号取得
		/// </summary>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>配送不可郵便番号</returns>
		public string GetUnavailableShippingZipFromShippingDelivery(string shippingId, string deliveryCompanyId, SqlAccessor accessor = null)
		{
			using (var repository = new ShopShippingRepository(accessor))
			{
				var unavailableShippingZip = repository.GetUnavailableShippingZipFromShippingDelivery(shippingId, deliveryCompanyId);
				return unavailableShippingZip;
			}
		}
		#endregion

		# region +InsertCompany 配送会社登録
		/// <summary>
		/// 配送会社登録
		/// </summary>
		/// <param name="accessor">アクセッサ</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="companyList">配送会社リスト</param>
		public void InsertCompany(SqlAccessor accessor, string shippingId, ShopShippingCompanyModel[] companyList)
		{
			using (var repostory = new ShopShippingRepository(accessor))
			{
				// 一旦全部消す
				repostory.DeleteCompany(shippingId);
				foreach (var company in companyList)
				{
					repostory.InsertCompany(company);
				}
			}
		}
		#endregion

		# region +DeleteCompany 配送会社削除
		/// <summary>
		/// 配送会社登録
		/// </summary>
		/// <param name="accessor">アクセッサ</param>
		/// <param name="shippingId">配送種別ID</param>
		public void DeleteCompany(SqlAccessor accessor, string shippingId)
		{
			using (var repostory = new ShopShippingRepository(accessor))
			{
				repostory.DeleteCompany(shippingId);
			}
		}
		#endregion

		#region +GetShippingCompanyByDeliveryCompanyId  delivery_company_idに紐づくすべての配送会社情報を取得
		/// <summary>
		/// delivery_company_idに紐づくすべての配送種別配送会社情報を取得
		/// </summary>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>モデル列</returns>
		public ShopShippingCompanyModel[] GetShippingCompanyByDeliveryCompanyId(string deliveryCompanyId, SqlAccessor accessor)
		{
			using (var repository = new ShopShippingRepository(accessor))
			{
				// 取得
				var models = repository.GetShippingCompanyByDeliveryCompanyId(deliveryCompanyId);
				return models;
			}
		}
		#endregion

		#region +InsertShopShippingZone 店舗配送料地帯情報を登録
		/// <summary>
		/// 店舗配送料地帯情報を登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void InsertShopShippingZone(ShopShippingZoneModel model, SqlAccessor accessor = null)
		{
			using (var repostory = new ShopShippingRepository(accessor))
			{
				repostory.InsertShopShippingZone(model);
			}
		}
		#endregion

		#region +InsertShopShippingZones 配送種別に紐づくすべての店舗配送料地帯情報を登録
		/// <summary>
		/// 配送種別に紐づくすべての店舗配送料地帯情報を登録
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="models">モデル配列</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void InsertShopShippingZones(
			string shopId,
			string shippingId,
			ShopShippingZoneModel[] models,
			SqlAccessor accessor = null)
		{
			using (var repostory = new ShopShippingRepository(accessor))
			{
				// 一旦全部消す
				repostory.DeleteShopShippingZoneByShippingId(shopId, shippingId);
				// 登録
				foreach(var model in models)
				{
					repostory.InsertShopShippingZone(model);
				}
			}
		}
		#endregion

		#region +DeleteShopShippingZoneByShippingId 配送種別IDに紐づくすべての店舗配送料地帯情報を削除
		/// <summary>
		/// 配送種別IDに紐づくすべての店舗配送料地帯情報を削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteShopShippingZoneByShippingId(string shopId, string shippingId, SqlAccessor accessor = null)
		{
			using (var repostory = new ShopShippingRepository(accessor))
			{
				repostory.DeleteShopShippingZoneByShippingId(shopId, shippingId);
			}
		}
		#endregion

		#region +DeleteShopShippingZoneByShippingIdAndZone 配送種別IDと地帯に紐づくすべての店舗配送料地帯情報を削除
		/// <summary>
		/// 配送種別IDに紐づくすべての店舗配送料地帯情報を削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="zoneNo">地帯区分</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteShopShippingZoneByShippingIdAndZone(
			string shopId,
			string shippingId,
			string zoneNo,
			SqlAccessor accessor = null)
		{
			using (var repostory = new ShopShippingRepository(accessor))
			{
				repostory.DeleteShopShippingZoneByShippingIdAndZone(shopId, shippingId, zoneNo);
			}
		}
		#endregion

		#region +GetShippingNamesByShippingIds
		/// <summary>
		/// Get shipping names by shipping ids
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="shippingIds">Shipping ids</param>
		/// <returns>Shipping names</returns>
		public string[] GetShippingNamesByShippingIds(string shopId, string[] shippingIds)
		{
			using (var repository = new ShopShippingRepository())
			{
				var shippingNames = repository.GetShippingNamesByShippingIds(shopId, shippingIds);
				return shippingNames;
			}
		}
		#endregion

		#region +GetAllShopShippings
		/// <summary>
		/// Get all shop shippings
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <returns>Shop shippings</returns>
		public ShopShippingModel[] GetAllShopShippings(string shopId)
		{
			using (var repository = new ShopShippingRepository())
			{
				var shippings = repository.GetAll(shopId);
				return shippings;
			}
		}
		#endregion

		#region +GetShopShipping
		/// <summary>
		/// Get shop shipping
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="shippingId">Shipping id</param>
		/// <returns>Shop shipping</returns>
		public ShopShippingModel GetShopShipping(string shopId, string shippingId)
		{
			using (var repository = new ShopShippingRepository())
			{
				var model = repository.Get(shopId, shippingId);
				return model;
			}
		}
		#endregion

		#region +UpdateShopShippingDateChangedAndLastChanged
		/// <summary>
		/// 更新日と最終更新者を更新
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateShopShippingDateChangedAndLastChanged(string shopId, string shippingId, string lastChanged, SqlAccessor accessor = null)
		{
			using (var repostory = new ShopShippingRepository(accessor))
			{
				repostory.UpdateShopShippingDateChangedAndLastChanged(shopId, shippingId, lastChanged);
			}
		}
		#endregion
	}
}
