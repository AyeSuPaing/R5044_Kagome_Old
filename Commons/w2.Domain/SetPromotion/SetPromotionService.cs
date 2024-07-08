/*
=========================================================================================================
  Module      : セットプロモーションサービス (SetPromotionService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Transactions;
using w2.Domain.SetPromotion.Helper;

namespace w2.Domain.SetPromotion
{
	/// <summary>
	/// セットプロモーションサービス
	/// </summary>
	public class SetPromotionService : ServiceBase, ISetPromotionService
	{
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(SetPromotionListSearchCondition condition)
		{
			using (var repository = new SetPromotionRepository())
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
		public SetPromotionListSearchResult[] Search(SetPromotionListSearchCondition condition)
		{
			using (var repository = new SetPromotionRepository())
			{
				var searchResults = repository.Search(condition);
				return searchResults;
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="setpromotionId">セットプロモーションID</param>
		/// <returns>モデル</returns>
		public SetPromotionModel Get(string setpromotionId)
		{
			using (var repository = new SetPromotionRepository())
			{
				var model = repository.Get(setpromotionId);
				if (model == null) return null;

				var items = repository.GetItemsAll(setpromotionId);
				model.Items = items;
				return model;
			}
		}
		#endregion

		#region +GetUsable 利用可能なものを取得
		/// <summary>
		/// 利用可能なものを取得
		/// </summary>
		/// <returns>モデル配列</returns>
		public SetPromotionModel[] GetUsable()
		{
			using (var repository = new SetPromotionRepository())
			{
				var models = repository.GetUsable();
				var itemsAll = repository.GetItemsAll(models.Select(m => m.SetpromotionId).ToArray());

				foreach (var model in models)
				{
					model.Items = itemsAll.Where(item => item.SetpromotionId == model.SetpromotionId).ToArray();
				}
				return models;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(SetPromotionModel model)
		{
			using (var scope = new TransactionScope())
			using (var repository = new SetPromotionRepository())
			{
				// 登録
				repository.Insert(model);

				// アイテム登録
				foreach (var item in model.Items)
				{
					repository.InsertItem(item);
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
		/// <param name="model">モデル</param>
		public void Update(SetPromotionModel model)
		{
			using (var scope = new TransactionScope())
			using (var repository = new SetPromotionRepository())
			{
				// 更新
				repository.Update(model);

				// アイテムすべて削除
				repository.DeleteItemsAll(model.SetpromotionId);

				// アイテム登録
				foreach (var item in model.Items)
				{
					repository.InsertItem(item);
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
		/// <param name="setpromotionId">セットプロモーションID</param>
		public void Delete(string setpromotionId)
		{
			using (var scope = new TransactionScope())
			using (var repository = new SetPromotionRepository())
			{
				// 削除
				repository.Delete(setpromotionId);

				// アイテムすべて削除
				repository.DeleteItemsAll(setpromotionId);

				// トランザクション完了
				scope.Complete();
			}
		}
		#endregion
	}
}
