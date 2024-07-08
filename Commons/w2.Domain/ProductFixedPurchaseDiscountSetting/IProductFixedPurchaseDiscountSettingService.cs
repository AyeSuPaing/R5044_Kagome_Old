/*
=========================================================================================================
  Module      : 商品定期購入割引設定サービスのインターフェース (IProductFixedPurchaseDiscountSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.ProductFixedPurchaseDiscountSetting
{
	/// <summary>
	/// 商品定期購入割引設定サービスのインターフェース
	/// </summary>
	public interface IProductFixedPurchaseDiscountSettingService : IService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <returns>モデル</returns>
		ProductFixedPurchaseDiscountSettingModel[] GetByShopIdAndProductId(string shopId, string productId);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		void Insert(ProductFixedPurchaseDiscountSettingModel model, SqlAccessor sqlAccessor = null);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		void DeleteByShopIdAndProductId(string shopId, string productId, SqlAccessor sqlAccessor = null);

		/// <summary>
		/// 適用する定期購入割引設定を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="orderCount">購入回数</param>
		/// <param name="productCount">商品個数</param>
		/// <returns>モデル</returns>
		ProductFixedPurchaseDiscountSettingModel GetApplyFixedPurchaseDiscountSetting(
			string shopId,
			string productId,
			int orderCount,
			int productCount);

		/// <summary>
		/// 適用する定期購入ポイント設定を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="orderCount">購入回数</param>
		/// <param name="productCount">商品個数</param>
		/// <returns>モデル</returns>
		ProductFixedPurchaseDiscountSettingModel GetApplyFixedPurchasePointSetting(
			string shopId,
			string productId,
			int orderCount,
			int productCount);
	}
}
