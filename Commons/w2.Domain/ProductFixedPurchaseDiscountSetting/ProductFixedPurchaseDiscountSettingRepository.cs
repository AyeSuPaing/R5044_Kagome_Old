/*
=========================================================================================================
  Module      : 商品定期購入割引設定リポジトリ (ProductFixedPurchaseDiscountSettingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.ProductFixedPurchaseDiscountSetting
{
	/// <summary>
	/// 商品定期購入割引設定リポジトリ
	/// </summary>
	public class ProductFixedPurchaseDiscountSettingRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "ProductFixedPurchaseDiscountSetting";

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductFixedPurchaseDiscountSettingRepository(SqlAccessor accessor = null)
			: base(accessor)
		{
		}
		#endregion

		#region +GetByShopIdAndProductId 取得
		/// <summary>
		/// 取得(店舗IDおよび商品ID指定)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		public ProductFixedPurchaseDiscountSettingModel[] GetByShopIdAndProductId(string shopId, string productId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_PRODUCT_ID, productId}
			};
			var dv = Get(XML_KEY_NAME, "GetByShopIdAndProductid", ht);
			return (dv.Count == 0)
				? null
				: dv.Cast<DataRowView>().Select(drv => new ProductFixedPurchaseDiscountSettingModel(drv)).ToArray();
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(ProductFixedPurchaseDiscountSettingModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region +DeleteByShopIdAndProductId 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		public int DeleteByShopIdAndProductId(string shopId, string productId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_PRODUCT_ID, productId}
			};
			var result = Exec(XML_KEY_NAME, "DeleteByShopIdAndProductId", ht);
			return result;
		}
		#endregion

		#region +GetApplyFixedPurchaseDiscountSetting 適用する定期購入割引設定を取得
		/// <summary>
		/// 適用する定期購入割引設定を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="orderCount">購入回数</param>
		/// <param name="productCount">商品個数</param>
		public ProductFixedPurchaseDiscountSettingModel GetApplyFixedPurchaseDiscountSetting(
			string shopId,
			string productId,
			int orderCount,
			int productCount)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_PRODUCT_ID, productId},
				{Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT, orderCount},
				{Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_PRODUCT_COUNT, productCount}
			};
			var dv = Get(XML_KEY_NAME, "GetApplyFixedPurchaseDiscountSetting", ht);
			return (dv.Count == 0)
				? null
				: new ProductFixedPurchaseDiscountSettingModel(dv[0]);
		}
		#endregion

		#region +GetApplyFixedPurchaseDiscountSetting 適用する定期購入ポイント設定を取得
		/// <summary>
		/// 適用する定期購入ポイント設定を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="orderCount">購入回数</param>
		/// <param name="productCount">商品個数</param>
		public ProductFixedPurchaseDiscountSettingModel GetApplyFixedPurchasePointSetting(
			string shopId,
			string productId,
			int orderCount,
			int productCount)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_PRODUCT_ID, productId},
				{Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT, orderCount},
				{Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_PRODUCT_COUNT, productCount}
			};
			var dv = Get(XML_KEY_NAME, "GetApplyFixedPurchasePointSetting", ht);
			return (dv.Count == 0)
				? null
				: new ProductFixedPurchaseDiscountSettingModel(dv[0]);
		}
		#endregion
	}
}
