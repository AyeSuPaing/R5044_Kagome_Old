/*
=========================================================================================================
  Module      : 商品タグサービス (ProductTagService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
using w2.Common.Sql;

namespace w2.Domain.ProductTag
{
	/// <summary>
	/// 商品タグサービス
	/// </summary>
	public class ProductTagService : ServiceBase, IProductTagService
	{
		#region +GetProductTag 商品タグ情報取得
		/// <summary>
		/// 商品タグ情報取得
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <returns>商品タグ情報</returns>
		public ProductTagModel GetProductTag(string productId)
		{
			using (var repository = new ProductTagRepository())
			{
				var model = repository.GetProductTag(productId);
				return model;
			}
		}
		#endregion

		#region +GetProductTagSetting 有効な商品タグ設定取得
		/// <summary>
		/// 有効な商品タグ設定取得
		/// </summary>
		/// <returns>商品タグ設定</returns>
		public ProductTagSettingModel[] GetProductTagSetting()
		{
			using (var repository = new ProductTagRepository())
			{
				var models = repository.GetProductTagSetting();
				return models;
			}
		}
		#endregion

		#region +GetTagSettingList 商品タグ設定一覧取得
		/// <summary>
		/// 商品タグ設定一覧取得
		/// </summary>
		/// <returns>商品タグ設定一覧</returns>
		public DataView GetTagSettingList()
		{
			using (var repository = new ProductTagRepository())
			{
				var models = repository.GetTagSettingList();
				return models;
			}
		}
		#endregion

		#region +Insert
		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Model</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		public int Insert(ProductTagModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ProductTagRepository(accessor))
			{
				var result = repository.Insert(model);
				return result;
			}
		}
		#endregion

		#region +Delete
		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="productId">Product ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		public int Delete(string productId, SqlAccessor accessor = null)
		{
			using (var repository = new ProductTagRepository(accessor))
			{
				var result = repository.Delete(productId);
				return result;
			}
		}
		#endregion
	}
}
