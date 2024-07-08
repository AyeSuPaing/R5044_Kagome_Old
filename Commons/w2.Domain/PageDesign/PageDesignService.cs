/*
=========================================================================================================
  Module      : ページデザイン ページ管理サービス (PageDesignService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.PageDesign.Helper;

namespace w2.Domain.PageDesign
{
	/// <summary>
	/// ページデザイン ページ管理サービス
	/// </summary>
	public class PageDesignService : ServiceBase, IPageDesignService
	{
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(PageDesignListSearch condition)
		{
			using (var repository = new PageDesignRepository())
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
		public PageDesignListSearchResult[] Search(PageDesignListSearch condition)
		{
			using (var repository = new PageDesignRepository())
			{
				var results = repository.Search(condition);
				return results;
			}
		}

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="otherPageGroupModel">その他 グループモデル</param>
		/// <returns>検索結果列</returns>
		public PageDesignListSearchGroupResult[] SearchGroup(
			PageDesignListSearch condition,
			PageDesignGroupModel otherPageGroupModel)
		{
			var searchResult = Search(condition);

			var groupModelList = GetAllGroup();
			var result = groupModelList.Where(g => (condition.GroupId == null) || (g.GroupId == condition.GroupId.Value))
				.Select(
					g => new PageDesignListSearchGroupResult(g.DataSource)
					{
						PageList = searchResult.Where(s => s.GroupId == g.GroupId).ToArray()
					}).ToArray();
			if ((condition.GroupId == null)
				|| (Constants.FLG_PAGEDESIGNGROUP_GROUP_ID_OTHER_ID == condition.GroupId.Value))
			{
				var otherGroupModel = new PageDesignListSearchGroupResult(otherPageGroupModel.DataSource)
				{
					PageList = searchResult
						.Where(s => (s.GroupId == Constants.FLG_PAGEDESIGNGROUP_GROUP_ID_OTHER_ID))
						.ToArray()
				};
				Array.Resize(ref result, result.Length + 1);
				result[result.Length - 1] = otherGroupModel;
			}
			return result;
		}
		#endregion

		#region グループ系
		#region +GetGroup グループ取得
		/// <summary>
		/// グループ取得
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public PageDesignGroupModel GetGroup(long groupId, SqlAccessor accessor = null)
		{
			using (var repository = new PageDesignRepository(accessor))
			{
				var model = repository.GetGroup(groupId);
				return model;
			}
		}
		#endregion

		#region +GetAllGroup グループ全取得
		/// <summary>
		/// グループ全取得
		/// </summary>
		/// <returns>モデル配列</returns>
		public PageDesignGroupModel[] GetAllGroup()
		{
			using (var repository = new PageDesignRepository())
			{
				var model = repository.GetAllGroup();
				return model;
			}
		}
		#endregion

		#region +InsertGroup グループ登録
		/// <summary>
		/// グループ登録
		/// </summary>
		/// <param name="model">グループモデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>新規グループID</returns>
		public int InsertGroup(PageDesignGroupModel model, SqlAccessor accessor = null)
		{
			using (var repository = new PageDesignRepository(accessor))
			{
				var id = repository.InsertGroup(model);
				return id;
			}
		}
		#endregion

		#region +UpdateGroup グループ更新
		/// <summary>
		/// グループ更新
		/// </summary>
		/// <param name="model">グループモデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateGroup(PageDesignGroupModel model, SqlAccessor accessor = null)
		{
			using (var repository = new PageDesignRepository(accessor))
			{
				var result = repository.UpdateGroup(model);
				return result;
			}
		}
		#endregion

		#region +UpdateGroupSort グループ順序 更新
		/// <summary>
		/// グループ順序 更新
		/// </summary>
		/// <param name="groupIds">グループ順序</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public void UpdateGroupSort(long[] groupIds, SqlAccessor accessor = null)
		{
			using (var repository = new PageDesignRepository(accessor))
			{
				for (var i = 0; i < groupIds.Length; i++)
				{
					var model = repository.GetGroup(groupIds[i]);
					if (model == null) { continue; }

					model.GroupSortNumber = i + 1;
					repository.UpdateGroup(model);
				}
			}
		}
		#endregion

		#region +Delete グループ削除
		/// <summary>
		/// グループ削除
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int DeleteGroup(long groupId, string lastChanged, SqlAccessor accessor = null)
		{
			using (var repository = new PageDesignRepository(accessor))
			{
				var result = repository.DeleteGroup(groupId);
				repository.UpdatePageMoveOtherGroup(groupId, lastChanged);
				return result;
			}
		}
		#endregion
		#endregion

		#region ページ系
		#region +GetPage ページ取得
		/// <summary>
		/// ページ取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public PageDesignModel GetPage(long pageId, SqlAccessor accessor = null)
		{
			using (var repository = new PageDesignRepository(accessor))
			{
				var model = repository.GetPage(pageId);
				return model;
			}
		}
		#endregion

		#region +GetPage ページ取得
		/// <summary>
		/// ファイル名でページ取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public PageDesignModel GetPageByFileName(string fileName, SqlAccessor accessor = null)
		{
			using (var repository = new PageDesignRepository(accessor))
			{
				var model = repository.GetPageByFileName(fileName);
				return model;
			}
		}
		#endregion

		#region +GetAllPage ページ 全取得
		/// <summary>
		/// ページ 全取得
		/// </summary>
		/// <returns>モデル配列</returns>
		public PageDesignModel[] GetAllPage()
		{
			using (var repository = new PageDesignRepository())
			{
				var model = repository.GetAllPage();
				return model;
			}
		}
		#endregion

		#region +InsertPage ページ登録
		/// <summary>
		/// ページ登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>新規ページID</returns>
		public int InsertPage(PageDesignModel model, SqlAccessor accessor = null)
		{
			using (var repository = new PageDesignRepository(accessor))
			{
				var id = repository.InsertPage(model);
				return id;
			}
		}
		#endregion

		#region +UpdatePage ページ更新
		/// <summary>
		/// ページ更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdatePage(PageDesignModel model, SqlAccessor accessor = null)
		{
			using (var repository = new PageDesignRepository(accessor))
			{
				var result = ((model.PageType == Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM)
						|| (model.PageType == Constants.FLG_PAGEDESIGN_PAGE_TYPE_HTML))
					? repository.UpdateCustomPage(model)
					: repository.UpdateNormalPage(model);
				return result;
			}
		}
		#endregion

		#region +UpdateManagementTitle 管理用タイトル更新
		/// <summary>
		///  管理用タイトル更新
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="title">管理用タイトル</param>
		public void UpdateManagementTitle(long pageId, string title)
		{
			var model = GetPage(pageId);
			if (model == null) return;
			model.ManagementTitle = title;
			UpdatePage(model);
		}
		#endregion

		#region +UpdatePageSort ページ順序 更新
		/// <summary>
		/// ページ順序 更新
		/// </summary>
		/// <param name="models">モデル配列</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdatePageSort(PageDesignModel[] models, SqlAccessor accessor = null)
		{
			using (var repository = new PageDesignRepository(accessor))
			{
				foreach (var model in models)
				{
					repository.UpdatePageGroupSort(model);
				}
			}
		}
		#endregion

		#region +DeletePage ページ削除
		/// <summary>
		/// ページ削除
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int DeletePage(long pageId, SqlAccessor accessor = null)
		{
			using (var repository = new PageDesignRepository(accessor))
			{
				var result = repository.DeletePage(pageId);
				return result;
			}
		}
		#endregion

		#region +UpdatePageMoveOtherGroup ページをその他グループに移動
		/// <summary>
		/// ページをその他グループに移動
		/// </summary>
		/// <param name="groupId">変更対象のグループID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdatePageMoveOtherGroup(long groupId, string lastChanged, SqlAccessor accessor = null)
		{
			using (var repository = new PageDesignRepository(accessor))
			{
				repository.UpdatePageMoveOtherGroup(groupId, lastChanged);
			}
		}
		#endregion

		#region +NotExistGroupIds 存在しないグループと紐づいているページのグループIDを抽出
		/// <summary>
		/// 存在しないグループと紐づいているページのグループIDを抽出
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public long[] NotExistGroupIds(SqlAccessor accessor = null)
		{
			using (var repository = new PageDesignRepository(accessor))
			{
				var result = repository.NotExistGroupIds();
				return result;
			}
		}
		#endregion
		#endregion

		#region サイトマップ用
		/// <summary>
		/// CMS管理のページをすべて取得
		/// </summary>
		/// <param name="accessor">アクセサ</param>
		/// <returns>XMLサイトマップ用ページモデル</returns>
		public SitemapPageModel[] GetAllManagedPagesForXmlSitemap(SqlAccessor accessor = null)
		{
			using (var repository = new PageDesignRepository(accessor))
			{
				var result = repository.GetAllManagedPagesForSitemap();
				return result;
			}
		}
		#endregion

		#region +CheckTargetListUsed ページ管理で使われているか
		/// <summary>
		/// ターゲットリストで使われているか
		/// </summary>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>ページ管理モデル</returns>
		public PageDesignModel[] CheckTargetListUsed(string targetId)
		{
			using (var repository = new PageDesignRepository())
			{
				var result = repository.CheckTargetListUsed(targetId);
				return result;
			}
		}
		#endregion
	}
}