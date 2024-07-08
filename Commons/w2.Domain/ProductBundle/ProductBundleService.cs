/*
=========================================================================================================
  Module      : 商品同梱テーブルサービス (ProductBundleService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.Order;
using w2.Domain.ProductBundle.Helper;

namespace w2.Domain.ProductBundle
{
	/// <summary>
	/// 商品同梱テーブルサービス
	/// </summary>
	public class ProductBundleService : ServiceBase
	{
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(ProductBundleListSearchCondition condition)
		{
			using (var repository = new ProductBundleRepository())
			{
				var count = repository.GetSearchHitCount(condition);
				return count;
			}
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		public ProductBundleListSearchResult[] Search(ProductBundleListSearchCondition condition)
		{
			using (var repository = new ProductBundleRepository())
			{
				var results = repository.Search(condition);
				return results;
			}
		}
		#endregion
		
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="productBundleId">商品同梱ID</param>
		/// <returns>モデル</returns>
		public ProductBundleModel Get(string productBundleId)
		{
			using (var repository = new ProductBundleRepository())
			{
				var model = repository.Get(productBundleId);
				if (model == null) return null;

				var items = repository.GetItemsAll(productBundleId);
				model.Items = items;
				return model;
			}
		}
		#endregion

		#region +GetProductBundleValidForCart 条件に合致する同梱設定を取得
		/// <summary>
		/// 条件に合致する同梱設定を取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="excludeOrderIds">利用回数にカウントしない注文ID</param>
		/// <param name="advCodeFirst">初回広告コード</param>
		/// <param name="advCodeNew">最新広告コード</param>
		/// <param name="orderPriceSubtotal">商品代金合計</param>
		/// <param name="targetOrderType">注文種別</param>
		/// <param name="targetCouponCode">クーポンコード</param>
		/// <returns>モデル</returns>
		public ProductBundleModel[] GetProductBundleValidForCart(
			string userId,
			string[] excludeOrderIds,
			string advCodeFirst,
			string advCodeNew,
			decimal orderPriceSubtotal,
			string targetOrderType,
			string targetCouponCode)
		{
			using (var repository = new ProductBundleRepository())
			{
				var bundles = repository.GetProductBundleValidForCart(
					userId,
					excludeOrderIds,
					advCodeFirst,
					advCodeNew,
					orderPriceSubtotal,
					targetOrderType,
					targetCouponCode);
				// ユーザー利用可能回数チェック
				bundles = CheckUsableTimes(userId, bundles, excludeOrderIds);

				return bundles;
			}
		}
		#endregion

		#region +SetProductBundleItems 同梱設定の同梱商品をセットする
		/// <summary>
		/// 同梱設定の同梱商品をセットする
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="excludeOrderIds">利用回数にカウントしない注文ID</param>
		/// <param name="bundles">同梱設定</param>
		public void SetProductBundleItems(
			string userId,
			string[] excludeOrderIds,
			List<ProductBundleModel> bundles)
		{
			using (var repository = new ProductBundleRepository())
			{
				foreach (var bundle in bundles)
				{
					bundle.Items = repository.GetProductBundleItems(
						bundle.ProductBundleId,
						userId,
						excludeOrderIds);
				}
			}
		}
		#endregion

		/// <summary>
		/// ユーザー利用可能回数チェック
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="productBundles">商品同梱情報</param>
		/// <param name="excludeOrderIds">利用回数にカウントしない注文ID</param>
		/// <returns>適用可能か</returns>
		private ProductBundleModel[] CheckUsableTimes(
			string userId,
			ProductBundleModel[] productBundles,
			string[] excludeOrderIds)
		{
			var bundleIdsForCheck = productBundles
				.Where(bundle =>
					((bundle.UsableTimesKbn == Constants.FLG_PRODUCTBUNDLE_USABLE_TIMES_KBN_ONCETIME)
						|| (bundle.UsableTimesKbn == Constants.FLG_PRODUCTBUNDLE_USABLE_TIMES_KBN_NUMSPECIFY)))
				.Select(bundle => bundle.ProductBundleId).ToList();
			// ユーザー利用可能回数が無制限の場合はチェックを行わない
			if (bundleIdsForCheck.Count == 0) return productBundles;

			// ユーザー利用可能回数が一回のみか回数指定の場合はチェックする
			var orderedCountInfo = new OrderService().GetOrderedCountForProductBundle(userId, bundleIdsForCheck, excludeOrderIds);
			var applyBundles = new List<ProductBundleModel>();
			foreach (var bundle in productBundles)
			{
				switch (bundle.UsableTimesKbn)
				{
					case Constants.FLG_PRODUCTBUNDLE_USABLE_TIMES_KBN_NOLIMIT:
						applyBundles.Add(bundle);
						break;

					case Constants.FLG_PRODUCTBUNDLE_USABLE_TIMES_KBN_ONCETIME:
						if (orderedCountInfo.ContainsKey(bundle.ProductBundleId) == false)
						{
							applyBundles.Add(bundle);
						}
						break;

					case Constants.FLG_PRODUCTBUNDLE_USABLE_TIMES_KBN_NUMSPECIFY:
						if ((orderedCountInfo.ContainsKey(bundle.ProductBundleId) == false)
							|| (orderedCountInfo[bundle.ProductBundleId] < bundle.UsableTimes))
						{
							if (orderedCountInfo.ContainsKey(bundle.ProductBundleId))
							{
								bundle.OrderedCount = orderedCountInfo[bundle.ProductBundleId];
							}
							applyBundles.Add(bundle);
						}
						break;
				}
			}
			return applyBundles.ToArray();
		}

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(ProductBundleModel model)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new ProductBundleRepository(accessor))
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();
				
				// 登録
				repository.Insert(model);

				// アイテム登録
				foreach (var item in model.Items)
				{
					repository.InsertItem(item);
				}

				// コミット
				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int Update(ProductBundleModel model)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new ProductBundleRepository(accessor))
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 更新
				var count = repository.Update(model);

				// アイテムすべて削除
				repository.DeleteItemsAll(model.ProductBundleId);

				// アイテム登録
				foreach (var item in model.Items)
				{
					repository.InsertItem(item);
				}

				// コミット
				accessor.CommitTransaction();

				return count;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="productBundleId">商品同梱ID</param>
		public void Delete(string productBundleId)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new ProductBundleRepository())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 削除
				repository.Delete(productBundleId);

				// アイテムすべて削除
				repository.DeleteItemsAll(productBundleId);

				// コミット
				accessor.CommitTransaction();
			}
		}
		#endregion
	}
}
