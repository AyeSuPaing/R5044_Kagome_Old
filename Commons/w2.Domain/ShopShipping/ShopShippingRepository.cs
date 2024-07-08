/*
=========================================================================================================
  Module      : 店舗配送種別リポジトリ (ShopShippingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.ShopShipping.Helper;

namespace w2.Domain.ShopShipping
{
	/// <summary>
	/// 店舗配送種別リポジトリ
	/// </summary>
	public class ShopShippingRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "ShopShipping";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ShopShippingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ShopShippingRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <returns>モデル</returns>
		public ShopShippingModel Get(string shopId, string shippingId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHOPSHIPPING_SHOP_ID, shopId},
				{Constants.FIELD_SHOPSHIPPING_SHIPPING_ID, shippingId}
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new ShopShippingModel(dv[0]);
		}
		#endregion

		#region +GetAll 取得（全て）
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル列</returns>
		public ShopShippingModel[] GetAll(string shopId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHOPSHIPPING_SHOP_ID, shopId},
			};
			var dv = Get(XML_KEY_NAME, "GetAll", ht);
			if (dv.Count == 0) return new ShopShippingModel[0];
			return dv.Cast<DataRowView>().Select(drv => new ShopShippingModel(drv)).ToArray();
		}
		#endregion

		#region +GetShippingInfoByShopId 取得(ドロップダウンリスト用)
		/// <summary>
		/// 取得(ドロップダウンリスト用)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル列</returns>
		public ShopShippingModel[] GetShippingInfoByShopId(string shopId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHOPSHIPPING_SHOP_ID, shopId},
			};
			var dv = Get(XML_KEY_NAME, "GetShippingInfoByShopId", ht);
			if (dv.Count == 0) return new ShopShippingModel[0];
			return dv.Cast<DataRowView>().Select(drv => new ShopShippingModel(drv)).ToArray();
		}
		#endregion

		#region +GetShippingDeliveryPostageAll 配送料マスタ取得（全て）
		/// <summary>
		/// 配送料マスタ取得（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <returns>モデル列</returns>
		public ShippingDeliveryPostageModel[] GetShippingDeliveryPostageAll(string shopId, string shippingId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SHOPSHIPPING_SHOP_ID, shopId },
				{ Constants.FIELD_SHOPSHIPPING_SHIPPING_ID, shippingId },
			};
			var dv = Get(XML_KEY_NAME, "GetShippingDeliveryPostageAll", ht);
			if (dv.Count == 0) return new ShippingDeliveryPostageModel[0];
			return dv.Cast<DataRowView>().Select(drv => new ShippingDeliveryPostageModel(drv)).ToArray();
		}
		#endregion

		#region +GetZoneAll 価格取得（全て）
		/// <summary>
		/// 価格取得（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <returns>モデル列</returns>
		public ShopShippingZoneModel[] GetZoneAll(string shopId, string shippingId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHOPSHIPPING_SHOP_ID, shopId},
				{Constants.FIELD_SHOPSHIPPING_SHIPPING_ID, shippingId}
			};
			var dv = Get(XML_KEY_NAME, "GetZoneAll", ht);
			if (dv.Count == 0) return new ShopShippingZoneModel[0];
			return dv.Cast<DataRowView>().Select(drv => new ShopShippingZoneModel(drv)).ToArray();
		}
		#endregion

		#region +GetCompanyAll 配送会社取得（全て）
		/// <summary>
		/// 配送会社取得（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <returns>モデル列</returns>
		public ShopShippingCompanyModel[] GetCompanyAll(string shopId, string shippingId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHOPSHIPPING_SHOP_ID, shopId},
				{Constants.FIELD_SHOPSHIPPING_SHIPPING_ID, shippingId},
			};
			var dv = Get(XML_KEY_NAME, "GetCompanyAll", ht);
			if (dv.Count == 0) return new ShopShippingCompanyModel[0];
			return dv.Cast<DataRowView>().Select(drv => new ShopShippingCompanyModel(drv)).ToArray();
		}
		#endregion

		#region +GetDefaultCompany 初期配送会社取得
		/// <summary>
		/// 初期配送会社取得
		/// </summary>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="shippingKbn">配送区分</param>
		/// <returns>モデル</returns>
		internal ShopShippingCompanyModel GetDefaultCompany(string shippingId, string shippingKbn)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_ID, shippingId},
				{Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_KBN, shippingKbn}
			};
			var dv = Get(XML_KEY_NAME, "GetDefaultCompany", ht);
			if (dv.Count == 0) return new ShopShippingCompanyModel();
			return new ShopShippingCompanyModel(dv[0]);
		}
		#endregion

		#region ~GetFromZipcode 郵便番号から取得(配送料情報含む)
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
		internal ShopShippingModel GetFromZipcode(
			string shopId,
			string shippingId,
			int shippingZoneNo,
			string zip,
			string deliveryCompanyId,
			int minShippingZoneNo = 47)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SHOPSHIPPING_SHOP_ID, shopId },
				{ Constants.FIELD_SHOPSHIPPING_SHIPPING_ID, shippingId },
				{ Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO, shippingZoneNo },
				{ Constants.FIELD_SHOPSHIPPINGZONE_ZIP, zip },
				{ Constants.FIELD_SHOPSHIPPINGZONE_DELIVERY_COMPANY_ID, deliveryCompanyId },
				{ "min_shipping_zone_no", minShippingZoneNo },
			};
			var dv = Get(XML_KEY_NAME, "GetFromZipcode", ht);
			return dv.Cast<DataRowView>().Select(drv => new ShopShippingHelper().GetShopShipping(drv)).FirstOrDefault();
		}
		#endregion

		#region ~GetUnavailableShippingZipFromShippingDelivery 配送情報から配送不可郵便番号取得
		/// <summary>
		/// 配送情報から配送不可郵便番号取得
		/// </summary>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <returns>配送不可郵便番号</returns>
		internal string GetUnavailableShippingZipFromShippingDelivery(
			string shippingId,
			string deliveryCompanyId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SHOPSHIPPING_SHIPPING_ID, shippingId },
				{ Constants.FIELD_SHOPSHIPPINGZONE_DELIVERY_COMPANY_ID, deliveryCompanyId }
			};
			var dv = Get(XML_KEY_NAME, "GetUnavailableShippingZipFromShippingDelivery", ht);
			return (dv.Count <= 0) ? string.Empty : dv[0].Row[0].ToString();
		}
		#endregion

		#region +DeleteCompany 配送会社削除
		/// <summary>
		/// 配送会社削除
		/// </summary>
		/// <param name="shippingId">配送種別ID</param>
		public void DeleteCompany(string shippingId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHOPSHIPPING_SHIPPING_ID, shippingId},
			};
			Exec(XML_KEY_NAME, "DeleteDeliveryCompany", ht);
		}
		#endregion

		#region +InsertCompany 配送会社登録
		/// <summary>
		/// 配送会社登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertCompany(ShopShippingCompanyModel model)
		{
			Exec(XML_KEY_NAME, "InsertCompany", model.DataSource);
		}
		#endregion

		#region +GetShippingCompanyByDeliveryCompanyId  delivery_company_idに紐づくすべての配送会社情報を取得
		/// <summary>
		/// delivery_company_idに紐づくすべての配送種別配送会社情報を取得
		/// </summary>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <returns>モデル列</returns>
		public ShopShippingCompanyModel[] GetShippingCompanyByDeliveryCompanyId(string deliveryCompanyId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHOPSHIPPINGCOMPANY_DELIVERY_COMPANY_ID, deliveryCompanyId},
			};
			var dv = Get(XML_KEY_NAME, "GetShippingCompanyByDeliveryCompanyId", ht);
			if (dv.Count == 0) return new ShopShippingCompanyModel[0];
			return dv.Cast<DataRowView>().Select(drv => new ShopShippingCompanyModel(drv)).ToArray();
		}
		#endregion

		#region +InsertShopShippingZone 店舗配送料地帯情報を登録
		/// <summary>
		/// 店舗配送料地帯情報を登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertShopShippingZone(ShopShippingZoneModel model)
		{
			Exec(XML_KEY_NAME, "InsertShopShippingZone", model.DataSource);
		}
		#endregion

		#region +DeleteShopShippingZoneByShippingId 配送種別IDに紐づくすべての店舗配送料地帯情報を削除
		/// <summary>
		/// 配送種別IDに紐づくすべての店舗配送料地帯情報を削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		internal void DeleteShopShippingZoneByShippingId(string shopId, string shippingId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SHOPSHIPPINGZONE_SHOP_ID, shopId },
				{ Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ID, shippingId },
			};
			Exec(XML_KEY_NAME, "DeleteShopShippingZoneByShippingId", input);
		}
		#endregion

		#region +DeleteShopShippingZoneByShippingIdAndZone 配送種別IDと地帯に紐づくすべての店舗配送料地帯情報を削除
		/// <summary>
		/// 配送種別IDに紐づくすべての店舗配送料地帯情報を削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="zoneNo">地帯区分</param>
		internal void DeleteShopShippingZoneByShippingIdAndZone(string shopId, string shippingId, string zoneNo)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SHOPSHIPPINGZONE_SHOP_ID, shopId },
				{ Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ID, shippingId },
				{ Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO, zoneNo },
			};
			Exec(XML_KEY_NAME, "DeleteShopShippingZoneByShippingIdAndZone", input);
		}
		#endregion

		#region +GetShippingNamesByShippingIds
		/// <summary>
		/// Get shipping names by shipping ids
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="shippingIds">Shipping ids</param>
		/// <returns>Shipping names</returns>
		internal string[] GetShippingNamesByShippingIds(string shopId, string[] shippingIds)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SHOPSHIPPING_SHOP_ID, shopId },
			};
			var replaceKeyValues = new[]
			{
				new KeyValuePair<string, string>(
					"@@ shipping_ids @@",
					string.Join(",", shippingIds.Select(shippingId => string.Format("'{0}'", shippingId.Replace("'", "''"))))),
			};
			var dv = Get(XML_KEY_NAME, "GetShippingNamesByShippingIds", input, replaces: replaceKeyValues);
			var shippingNames = dv.Cast<DataRowView>()
				.Select(drv => (string)drv[Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME])
				.ToArray();
			return shippingNames;
		}
		#endregion

		#region +UpdateShopShippingDateChangedAndLastChanged
		/// <summary>
		/// 更新日と最終更新者を更新
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="lastChanged">最終更新者</param>
		public void UpdateShopShippingDateChangedAndLastChanged(string shopId, string shippingId, string lastChanged)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SHOPSHIPPING_SHOP_ID, shopId },
				{ Constants.FIELD_SHOPSHIPPING_SHIPPING_ID, shippingId },
				{ Constants.FIELD_SHOPSHIPPING_LAST_CHANGED, lastChanged },
			};
			Exec(XML_KEY_NAME, "UpdateShopShippingDateChangedAndLastChanged", input);
		}
		#endregion
	}
}