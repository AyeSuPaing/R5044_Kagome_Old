/*
=========================================================================================================
  Module      : 定期商品変更設定サービス (FixedPurchaseProductChangeSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Transactions;
using w2.Common.Sql;
using w2.Domain.FixedPurchaseProductChangeSetting.Helper;

namespace w2.Domain.FixedPurchaseProductChangeSetting
{
	/// <summary>
	/// 定期商品変更設定サービス
	/// </summary>
	public class FixedPurchaseProductChangeSettingService : ServiceBase, IFixedPurchaseProductChangeSettingService
	{
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(FixedPurchaseProductChangeSettingListSearchCondition condition)
		{
			using (var repository = new FixedPurchaseProductChangeSettingRepository())
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
		public FixedPurchaseProductChangeSettingListSearchResult[] Search(FixedPurchaseProductChangeSettingListSearchCondition condition)
		{
			using (var repository = new FixedPurchaseProductChangeSettingRepository())
			{
				var searchResults = repository.Search(condition);
				return searchResults;
			}
		}
		#endregion

		#region +Get 取得：定期商品変更ID
		/// <summary>
		/// 取得：定期商品変更ID
		/// </summary>
		/// <param name="fixedPurchaseProductChangeId">定期商品変更ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>定期商品変更設定モデル</returns>
		public FixedPurchaseProductChangeSettingModel Get(string fixedPurchaseProductChangeId, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseProductChangeSettingRepository(accessor))
			{
				var model = repository.GetByFixedPurchaseProductChangeId(fixedPurchaseProductChangeId);
				if (model == null) return null;
				model.BeforeChangeItems = repository.GetBeforeChangeItems(fixedPurchaseProductChangeId);
				model.AfterChangeItems = repository.GetAfterChangeItems(fixedPurchaseProductChangeId);
				return model;
			}
		}
		#endregion

		#region +GetByProductId 取得：商品ID
		/// <summary>
		/// 取得：商品ID
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>定期商品変更設定モデル</returns>
		public FixedPurchaseProductChangeSettingModel GetByProductId(string productId, string variationId, string shopId, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseProductChangeSettingRepository(accessor))
			{
				var beforeChangeItems = repository.GetBeforeChangeItemsByProductId(productId, variationId, shopId);
				if (beforeChangeItems == null) return null;

				var fixedPurchaseProductChangeIds = beforeChangeItems.Select(
					item => item.FixedPurchaseProductChangeId).Distinct().ToArray();
				var model = repository.GetByFixedPurchaseProductChangeIds(fixedPurchaseProductChangeIds);
				model.BeforeChangeItems = repository.GetBeforeChangeItems(model.FixedPurchaseProductChangeId);
				model.AfterChangeItems = repository.GetAfterChangeItems(model.FixedPurchaseProductChangeId);
				return model;
			}
		}
		#endregion

		#region +GetAfterChangeItems 変更後商品取得
		/// <summary>
		/// 変更後商品取得
		/// </summary>
		/// <param name="fixedPurchaseProductChangeId">定期商品変更設定ID</param>
		/// <returns></returns>
		public FixedPurchaseAfterChangeItemModel[] GetAfterChangeItems(string fixedPurchaseProductChangeId)
		{
			using (var repository = new FixedPurchaseProductChangeSettingRepository())
			{
				var model = repository.GetAfterChangeItems(fixedPurchaseProductChangeId);
				return model;
			}
		}
		#endregion

		#region +GetContainer 表示用定期商品変更設定取得
		/// <summary>
		/// 表示用定期商品変更設定取得
		/// </summary>
		/// <param name="fixedPurchaseProductChangeId">定期商品変更ID</param>
		/// <returns>定期商品変更設定モデル</returns>
		public FixedPurchaseProductChangeSettingContainer GetContainer(string fixedPurchaseProductChangeId)
		{
			using (var repository = new FixedPurchaseProductChangeSettingRepository())
			{
				var model = repository.GetContainer(fixedPurchaseProductChangeId);
				if (model == null) return null;
				model.BeforeChangeItemContainers = repository.GetBeforeChangeItemContainers(fixedPurchaseProductChangeId);
				model.AfterChangeItemContainers = repository.GetAfterChangeItemContainers(fixedPurchaseProductChangeId);
				return model;
			}
		}
		#endregion

		#region Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">定期商品変更設定モデル</param>
		public void Insert(FixedPurchaseProductChangeSettingModel model)
		{
			using (var scope = new TransactionScope())
			using (var repository = new FixedPurchaseProductChangeSettingRepository())
			{
				// 登録
				repository.Insert(model);

				// 定期変更元商品登録
				foreach (var beforeChangeItem in model.BeforeChangeItems)
				{
					repository.InsertBeforeChangeItem(beforeChangeItem);
				}

				// 定期変更後商品登録
				foreach (var afterChangeItem in model.AfterChangeItems)
				{
					repository.InsertAfterChangeItem(afterChangeItem);
				}

				// トランザクション完了
				scope.Complete();
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">定期商品変更設定モデル</param>
		public void Update(FixedPurchaseProductChangeSettingModel model)
		{
			using (var scope = new TransactionScope())
			using (var repository = new FixedPurchaseProductChangeSettingRepository())
			{
				// 更新
				repository.Update(model);

				// 定期変更商品削除
				repository.DeleteBeforeChangeItem(model.FixedPurchaseProductChangeId);
				repository.DeleteAfterChangeItem(model.FixedPurchaseProductChangeId);

				// 定期変更後商品登録
				foreach (var beforeChangeItem in model.BeforeChangeItems)
				{
					repository.InsertBeforeChangeItem(beforeChangeItem);
				}
				foreach (var afterChangeItem in model.AfterChangeItems)
				{
					repository.InsertAfterChangeItem(afterChangeItem);
				}

				// トランザクション完了
				scope.Complete();
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="fixedPurchaseProductChangeSettingId">定期商品変更設定ID</param>
		public void Delete(string fixedPurchaseProductChangeSettingId)
		{
			using (var scope = new TransactionScope())
			using (var repository = new FixedPurchaseProductChangeSettingRepository())
			{
				// 削除
				repository.Delete(fixedPurchaseProductChangeSettingId);

				// 定期変更商品削除
				repository.DeleteBeforeChangeItem(fixedPurchaseProductChangeSettingId);
				repository.DeleteAfterChangeItem(fixedPurchaseProductChangeSettingId);

				// トランザクション完了
				scope.Complete();
			}
		}
		#endregion
	}
}
