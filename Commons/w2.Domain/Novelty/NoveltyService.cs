/*
=========================================================================================================
  Module      : ノベルティ設定サービス (NoveltyService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Transactions;

namespace w2.Domain.Novelty
{
	/// <summary>
	/// ノベルティ設定サービス
	/// </summary>
	public class NoveltyService : ServiceBase, INoveltyService
	{
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(Hashtable param)
		{
			using (var repository = new NoveltyRepository())
			{
				var count = repository.GetSearchHitCount(param);
				return count;
			}
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <returns>モデル列</returns>
		public NoveltyModel[] Search(Hashtable param)
		{
			using (var repository = new NoveltyRepository())
			{
				var models = repository.Search(param);
				return models;
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="noveltyId">ノベルティID</param>
		/// <returns>モデル</returns>
		public NoveltyModel Get(string shopId, string noveltyId)
		{
			using (var repository = new NoveltyRepository())
			{
				// 取得
				var model = repository.Get(shopId, noveltyId);
				if (model == null) return null;

				// 対象アイテム
				model.TargetItemList = repository.GetTargetItemAll(shopId, noveltyId);
				// 付与条件
				model.GrantConditionsList = repository.GetGrantConditionsAll(shopId, noveltyId);
				// 付与アイテム
				var grantItemList = repository.GetGrantItemAll(shopId, noveltyId);
				foreach (var grantConditions in model.GrantConditionsList)
				{
					grantConditions.GrantItemList =
						grantItemList.Where(grantItem => grantItem.ConditionNo == grantConditions.ConditionNo).ToArray();
				}

				return model;
			}
		}
		#endregion

		#region +GetAll 全て取得
		/// <summary>
		/// 全て取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル列</returns>
		public NoveltyModel[] GetAll(string shopId)
		{
			using (var repository = new NoveltyRepository())
			{
				var models = repository.GetAll(shopId);
				foreach (var model in models)
				{
					// 対象アイテム
					model.TargetItemList = repository.GetTargetItemAll(model.ShopId, model.NoveltyId);
					// 付与条件
					model.GrantConditionsList = repository.GetGrantConditionsAll(model.ShopId, model.NoveltyId);
					// 付与アイテム
					var grantItemList = repository.GetGrantItemAll(model.ShopId, model.NoveltyId);
					foreach (var grantConditions in model.GrantConditionsList)
					{
						grantConditions.GrantItemList =
							grantItemList.Where(grantItem => grantItem.ConditionNo == grantConditions.ConditionNo).ToArray();
					}
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
		public void Insert(NoveltyModel model)
		{
			using (var scope = new TransactionScope())
			using (var repository = new NoveltyRepository())
			{
				// 登録
				repository.Insert(model);

				// 対象アイテム登録
				foreach (var targetItem in model.TargetItemList)
				{
					repository.InsertTargetItem(targetItem);
				}

				// 付与条件登録
				foreach (var grantConditions in model.GrantConditionsList)
				{
					repository.InsertGrantConditions(grantConditions);

					// 付与アイテム登録
					foreach (var grantItem in grantConditions.GrantItemList)
					{
						repository.InsertGrantItem(grantItem);
					}
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
		public void Update(NoveltyModel model)
		{
			using (var scope = new TransactionScope())
			using (var repository = new NoveltyRepository())
			{
				// 更新
				repository.Update(model);

				// 対象アイテム更新（DELETE → INSERT）
				repository.DeleteTargetItemAll(model.ShopId, model.NoveltyId);
				foreach (var targetItem in model.TargetItemList)
				{
					repository.InsertTargetItem(targetItem);
				}

				// 付与条件・付与アイテム更新（DELETE → INSERT）
				repository.DeleteGrantConditionsAll(model.ShopId, model.NoveltyId);
				repository.DeleteGrantItemAll(model.ShopId, model.NoveltyId);
				foreach (var grantConditions in model.GrantConditionsList)
				{
					// 付与条件
					repository.InsertGrantConditions(grantConditions);
					// 付与アイテム
					foreach (var grantItem in grantConditions.GrantItemList)
					{
						repository.InsertGrantItem(grantItem);
					}
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
		/// <param name="shopId">店舗ID</param>
		/// <param name="noveltyId">ノベルティID</param>
		public void Delete(string shopId, string noveltyId)
		{
			using (var scope = new TransactionScope())
			using (var repository = new NoveltyRepository())
			{
				// 削除
				repository.Delete(shopId, noveltyId);
				// 対象アイテム
				repository.DeleteTargetItemAll(shopId, noveltyId);
				// 付与条件
				repository.DeleteGrantConditionsAll(shopId, noveltyId);
				// 付与アイテム
				repository.DeleteGrantItemAll(shopId, noveltyId);

				// トランザクション完了
				scope.Complete();
			}
		}
		#endregion
	}
}
