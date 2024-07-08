/*
=========================================================================================================
  Module      : 商品グループサービス (ProductGroupService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Transactions;
using w2.Domain.ProductGroup.Helper;

namespace w2.Domain.ProductGroup
{
	/// <summary>
	/// 商品グループサービス
	/// </summary>
	public class ProductGroupService : ServiceBase, IProductGroupService
	{
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(ProductGroupListSearchCondition condition)
		{
			using (var repository = new ProductGroupRepository())
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
		public ProductGroupListSearchResult[] Search(ProductGroupListSearchCondition condition)
		{
			using (var repository = new ProductGroupRepository())
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
		/// <param name="productGroupId">商品グループID</param>
		/// <returns>モデル</returns>
		public ProductGroupModel Get(string productGroupId)
		{
			using (var repository = new ProductGroupRepository())
			{
				var model = repository.Get(productGroupId);
				if (model == null) return null;

				var items = repository.GetItemsAll(productGroupId);
				model.Items = items;
				return model;
			}
		}
		#endregion

		#region +GetAllProductGroup 商品グループ全件取得
		/// <summary>
		/// 商品グループ全件取得
		/// </summary>
		/// <returns>モデル</returns>
		public ProductGroupModel[] GetAllProductGroup()
		{
			using (var repository = new ProductGroupRepository())
			{
				var model = repository.GetAll();
				if (model == null) return null;
				return model;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(ProductGroupModel model)
		{
			using (var scope = new TransactionScope())
			using (var repository = new ProductGroupRepository())
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
		public void Update(ProductGroupModel model)
		{
			using (var scope = new TransactionScope())
			using (var repository = new ProductGroupRepository())
			{
				// 更新
				repository.Update(model);

				// アイテムすべて削除
				repository.DeleteItemsAll(model.ProductGroupId);

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
		/// <param name="productGroupId">商品グループID</param>
		public void Delete(string productGroupId)
		{
			using (var repository = new ProductGroupRepository())
			{
				// 削除
				repository.Delete(productGroupId);

				// アイテムすべて削除
				repository.DeleteItemsAll(productGroupId);

			}
		}
		#endregion

		#region +CheckDupulicationProductGroupId 商品グループID重複チェック
		/// <summary>
		/// 商品グループID重複チェック
		/// </summary>
		/// <param name="productGroupId">商品グループID</param>
		/// <returns>チェック結果</returns>
		public bool CheckDupulicationProductGroupId(string productGroupId)
		{
			using (var repository = new ProductGroupRepository())
			{
				// 商品グループID重複チェック
				var model = repository.CheckDupulicationProductGroupId(productGroupId);
				return (model == null);
			}
		}
		#endregion

		#region +GetUsedProductGroupCount 紐づいた件数取得
		/// <summary>
		/// 紐づいた件数取得
		/// </summary>
		/// <param name="productGroupId">商品グループID</param>
		/// <returns>紐づいた件数</returns>
		public int GetUsedProductGroupCount(string productGroupId)
		{
			using (var repository = new ProductGroupRepository())
			{
				// 商品グループID重複チェック
				var count = repository.GetUsedProductGroupCount(productGroupId);
				return count;
			}
		}
		#endregion
	}
}
