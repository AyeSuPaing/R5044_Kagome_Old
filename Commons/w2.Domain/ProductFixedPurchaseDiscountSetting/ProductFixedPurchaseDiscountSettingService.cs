/*
=========================================================================================================
  Module      : 商品定期購入割引設定サービス (ProductFixedPurchaseDiscountSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.ProductFixedPurchaseDiscountSetting
{
	/// <summary>
	/// 商品定期購入割引設定サービス
	/// </summary>
	public class ProductFixedPurchaseDiscountSettingService : ServiceBase, IProductFixedPurchaseDiscountSettingService
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <returns>モデル</returns>
		public ProductFixedPurchaseDiscountSettingModel[] GetByShopIdAndProductId(string shopId, string productId)
		{
			using (var repository = new ProductFixedPurchaseDiscountSettingRepository())
			{
				var models = repository.GetByShopIdAndProductId(shopId, productId);
				return models;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		public void Insert(ProductFixedPurchaseDiscountSettingModel model, SqlAccessor sqlAccessor = null)
		{
			using (var repository = new ProductFixedPurchaseDiscountSettingRepository(sqlAccessor))
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		public void DeleteByShopIdAndProductId(string shopId, string productId, SqlAccessor sqlAccessor = null)
		{
			using (var repository = new ProductFixedPurchaseDiscountSettingRepository(sqlAccessor))
			{
				var result = repository.DeleteByShopIdAndProductId(shopId, productId);
			}
		}
		#endregion

		#region
		/// <summary>
		/// 適用する定期購入割引設定を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="orderCount">購入回数</param>
		/// <param name="productCount">商品個数</param>
		/// <returns>モデル</returns>
		public ProductFixedPurchaseDiscountSettingModel GetApplyFixedPurchaseDiscountSetting(
			string shopId,
			string productId,
			int orderCount,
			int productCount)
		{
			using (var repository = new ProductFixedPurchaseDiscountSettingRepository())
			{
				var result = repository.GetApplyFixedPurchaseDiscountSetting(shopId, productId, orderCount, productCount);
				return result;
			}
		}
		#endregion

		#region
		/// <summary>
		/// 適用する定期購入ポイント設定を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="orderCount">購入回数</param>
		/// <param name="productCount">商品個数</param>
		/// <returns>モデル</returns>
		public ProductFixedPurchaseDiscountSettingModel GetApplyFixedPurchasePointSetting(
			string shopId,
			string productId,
			int orderCount,
			int productCount)
		{
			using (var repository = new ProductFixedPurchaseDiscountSettingRepository())
			{
				var result = repository.GetApplyFixedPurchasePointSetting(shopId, productId, orderCount, productCount);
				return result;
			}
		}
		#endregion
	}
}
