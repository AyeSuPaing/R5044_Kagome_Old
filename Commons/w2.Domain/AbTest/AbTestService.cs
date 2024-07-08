/*
=========================================================================================================
  Module      : ABテストサービス (AbTestService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.Common.Sql;
using w2.Domain.ContentsLog;

namespace w2.Domain.AbTest
{
	/// <summary>
	/// ABテストサービス
	/// </summary>
	public class AbTestService : ServiceBase,IAbTestService
	{
		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="searchWord">検索条件：タイトル又はファイル名</param>
		/// <returns>検索結果列</returns>
		public AbTestModel[] Search(string searchWord)
		{
			using (var repository = new AbTestRepository())
			{
				var results = repository.Search(searchWord);

				results = results.Select(
					r =>
					{
						r.Items = GetAllItemByAbTestId(r.AbTestId);
						return r;
					}).ToArray();
				return results;
			}
		}
		#endregion

		#region +Get ABテスト取得
		/// <summary>
		/// ABテスト取得
		/// </summary>
		/// <param name="abTestId">ABテストID</param>
		/// <returns>モデル</returns>
		public AbTestModel Get(string abTestId)
		{
			using (var repository = new AbTestRepository())
			{
				var model = repository.Get(abTestId);
				return model;
			}
		}
		#endregion

		#region +GetAllItemByAbTestId ABテストアイテム取得
		/// <summary>
		/// ABテストアイテム取得
		/// </summary>
		/// <param name="abTestId">ABテストID</param>
		/// <returns>ABテストアイテム</returns>
		public AbTestItemModel[] GetAllItemByAbTestId(string abTestId)
		{
			using (var repository = new AbTestRepository())
			{
				var items = repository.GetAllItemByAbTestId(abTestId);

				return items;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(AbTestModel model)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new AbTestRepository(accessor))
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();
				repository.Insert(model);

				if (model.Items != null)
				{
					foreach (var item in model.Items)
					{
						item.AbTestId = model.AbTestId;
						repository.InsertItem(item);
					}
				}
				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(AbTestModel model)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new AbTestRepository(accessor))
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();
				var result = repository.Update(model);
				repository.DeleteAllItemByAbTestId(model.AbTestId);
				if (model.Items != null)
				{
					foreach (var item in model.Items)
					{
						item.AbTestId = model.AbTestId;
						repository.InsertItem(item);
					}
				}
				accessor.CommitTransaction();
				return result;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="abTestId">ABテストID</param>
		/// <returns>影響を受けたABテストの件数</returns>
		public int Delete(string abTestId)
		{
			var contentsLogService = new ContentsLogService();
			var result = 0;

			using (var accessor = new SqlAccessor())
			using (var abTestRepository = new AbTestRepository(accessor))
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var contentsIds = abTestRepository.GetAllItemByAbTestId(abTestId).Select(model => abTestId + "-" + model.PageId).ToArray();

				result += abTestRepository.Delete(abTestId);
				abTestRepository.DeleteAllItemByAbTestId(abTestId);
				contentsLogService.DeleteByContentsIds(Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_ABTEST, contentsIds, accessor);

				accessor.CommitTransaction();
			}

			return result;
		}
		#endregion
	}
}
