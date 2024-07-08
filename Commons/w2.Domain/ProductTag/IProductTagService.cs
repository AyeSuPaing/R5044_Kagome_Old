/*
=========================================================================================================
  Module      : 商品タグサービスのインタフェース (IProductTagService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
using w2.Common.Sql;

namespace w2.Domain.ProductTag
{
	/// <summary>
	/// 商品タグサービスのインタフェース
	/// </summary>
	public interface IProductTagService : IService
	{
		/// <summary>
		/// 商品タグ情報取得
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <returns>商品タグ情報</returns>
		ProductTagModel GetProductTag(string productId);

		/// <summary>
		/// 商品タグ設定一覧取得
		/// </summary>
		/// <returns>商品タグ設定一覧</returns>
		DataView GetTagSettingList();

		/// <summary>
		/// 有効な商品タグ設定取得
		/// </summary>
		/// <returns>商品タグ設定</returns>
		ProductTagSettingModel[] GetProductTagSetting();

		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Model</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int Insert(ProductTagModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="productId">Product ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int Delete(string productId, SqlAccessor accessor = null);
	}
}