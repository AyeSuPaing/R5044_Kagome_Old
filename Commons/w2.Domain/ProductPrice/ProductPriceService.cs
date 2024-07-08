/*
=========================================================================================================
  Module      : 商品価格サービス (ProductPriceService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Sql;

namespace w2.Domain.ProductPrice
{
	/// <summary>
	/// 商品価格サービス
	/// </summary>
	public class ProductPriceService : ServiceBase, IProductPriceService
	{
		#region +GetAll
		/// <summary>
		/// Get all
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <returns>A array of member rank price</returns>
		public ProductPriceModel[] GetAll(string shopId, string productId)
		{
			using (var repository = new ProductPriceRepository())
			{
				var data = repository.GetAll(shopId, productId);
				return data;
			}
		}
		#endregion

		#region +Get
		/// <summary>
		/// Get
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <param name="variationId">Variation Id</param>
		/// <param name="memberRankId">Member rank id</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Model</returns>
		public ProductPriceModel Get(
			string shopId,
			string productId,
			string variationId,
			string memberRankId,
			SqlAccessor accessor)
		{
			using (var repository = new ProductPriceRepository(accessor))
			{
				var model = repository.Get(
					shopId,
					productId,
					variationId,
					memberRankId);
				return model;
			}
		}
		#endregion

		#region +IsExist
		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="variationId">Variation ID</param>
		/// <param name="memberRankId">Member rank ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>True if product price is existed, otherwise false</returns>
		public bool IsExist(
			string shopId,
			string productId,
			string variationId,
			string memberRankId,
			SqlAccessor accessor = null)
		{
			using (var repository = new ProductPriceRepository(accessor))
			{
				var result = repository.IsExist(shopId, productId, variationId, memberRankId);
				return result;
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
		public int Insert(ProductPriceModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ProductPriceRepository(accessor))
			{
				var result = repository.Insert(model);
				return result;
			}
		}
		#endregion

		#region +Modify
		/// <summary>
		/// Modify
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <param name="variationId">Variation id</param>
		/// <param name="memberRankId">Member rank id</param>
		/// <param name="updateAction">Update action</param>
		/// <param name="accessor">SQL accessor</param>
		/// <param name="execConditionFunc">Execute condition function</param>
		/// <returns>Number of rows affected</returns>
		public int Modify(
			string shopId,
			string productId,
			string variationId,
			string memberRankId,
			Action<ProductPriceModel> updateAction,
			SqlAccessor accessor,
			Func<ProductPriceModel, bool> execConditionFunc = null)
		{
			// 最新データ取得
			var productPrice = Get(
				shopId,
				productId,
				variationId,
				memberRankId,
				accessor);

			// 条件
			var exec = (execConditionFunc == null) || execConditionFunc(productPrice);
			if (exec == false) return 0;

			// モデル内容更新
			updateAction(productPrice);

			// 更新
			int updated;
			using (var repository = new ProductPriceRepository(accessor))
			{
				updated = repository.Update(productPrice);
			}
			return updated;
		}
		#endregion

		#region +UpdateForModify
		/// <summary>
		/// Update for modify
		/// </summary>
		/// <param name="productPrice">Product model</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		public int UpdateForModify(
			ProductPriceModel productPrice,
			string lastChanged,
			SqlAccessor accessor)
		{
			var updated = Modify(
				productPrice.ShopId,
				productPrice.ProductId,
				productPrice.VariationId,
				productPrice.MemberRankId,
				model =>
				{
					model.MemberRankPrice = productPrice.MemberRankPrice;
					model.LastChanged = lastChanged;
				},
				accessor);
			return updated;
		}
		#endregion

		#region +Delete
		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="variationId">Variation ID</param>
		/// <param name="memberRankId">Member rank ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		public int Delete(
			string shopId,
			string productId,
			string variationId,
			string memberRankId,
			SqlAccessor accessor = null)
		{
			using (var repository = new ProductPriceRepository(accessor))
			{
				var result = repository.Delete(
					shopId,
					productId,
					variationId,
					memberRankId);
				return result;
			}
		}
		#endregion

		#region +DeleteAll
		/// <summary>
		/// Delete all
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		public int DeleteAll(
			string shopId,
			string productId,
			SqlAccessor accessor = null)
		{
			using (var repository = new ProductPriceRepository(accessor))
			{
				var result = repository.DeleteAll(shopId, productId);
				return result;
			}
		}
		#endregion
	}
}