/*
=========================================================================================================
  Module      : 商品価格サービスのインターフェース (IProductPriceService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Sql;

namespace w2.Domain.ProductPrice
{
	/// <summary>
	/// 商品価格サービスのインターフェース
	/// </summary>
	public interface IProductPriceService : IService
	{
		/// <summary>
		/// Get all
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <returns>A array of member rank price</returns>
		ProductPriceModel[] GetAll(string shopId, string productId);

		/// <summary>
		/// Get
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <param name="variationId">Variation Id</param>
		/// <param name="memberRankId">Member rank id</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Model</returns>
		ProductPriceModel Get(
			string shopId,
			string productId,
			string variationId,
			string memberRankId,
			SqlAccessor accessor);

		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="variationId">Variation ID</param>
		/// <param name="memberRankId">Member rank ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>True if product price is existed, otherwise false</returns>
		bool IsExist(
			string shopId,
			string productId,
			string variationId,
			string memberRankId,
			SqlAccessor accessor = null);

		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Model</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int Insert(ProductPriceModel model, SqlAccessor accessor = null);

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
		int Modify(
			string shopId,
			string productId,
			string variationId,
			string memberRankId,
			Action<ProductPriceModel> updateAction,
			SqlAccessor accessor,
			Func<ProductPriceModel, bool> execConditionFunc = null);

		/// <summary>
		/// Update for modify
		/// </summary>
		/// <param name="productPrice">Product model</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int UpdateForModify(
			ProductPriceModel productPrice,
			string lastChanged,
			SqlAccessor accessor);

		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="variationId">Variation ID</param>
		/// <param name="memberRankId">Member rank ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int Delete(
			string shopId,
			string productId,
			string variationId,
			string memberRankId,
			SqlAccessor accessor = null);

		/// <summary>
		/// Delete all
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int DeleteAll(
			string shopId,
			string productId,
			SqlAccessor accessor = null);
	}
}
