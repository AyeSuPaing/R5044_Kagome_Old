/*
=========================================================================================================
  Module      : ページデザイン ページ管理リポジトリ (PageDesignRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.PageDesign.Helper;

namespace w2.Domain.PageDesign
{
	/// <summary>
	/// ページデザイン ページ管理リポジトリ
	/// </summary>
	internal class PageDesignRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "PageDesign";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal PageDesignRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal PageDesignRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		internal int GetSearchHitCount(PageDesignListSearch condition)
		{
			var input = condition.CreateHashtableParams();
			var dv = Get(
				XML_KEY_NAME,
				"GetSearchHitCount",
				input,
				replaces: condition.ReplaceList());

			return (int)dv[0][0];
		}
		#endregion

		#region ~Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		internal PageDesignListSearchResult[] Search(PageDesignListSearch condition)
		{
			var input = condition.CreateHashtableParams();
			var dv = Get(
				XML_KEY_NAME,
				"Search",
				input,
				replaces: condition.ReplaceList());

			return dv.Cast<DataRowView>().Select(drv => new PageDesignListSearchResult(drv)).ToArray();
		}
		#endregion

		#region グループ系
		#region +GetGroup グループ取得
		/// <summary>
		/// グループ取得
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <returns>モデル</returns>
		internal PageDesignGroupModel GetGroup(long groupId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PAGEDESIGNGROUP_GROUP_ID, groupId},
			};
			var dv = Get(XML_KEY_NAME, "GetGroup", ht);
			if (dv.Count == 0) return null;
			return new PageDesignGroupModel(dv[0]);
		}
		#endregion

		#region +GetAllGroup グループ全取得
		/// <summary>
		/// グループ全取得
		/// </summary>
		/// <returns>モデル配列</returns>
		internal PageDesignGroupModel[] GetAllGroup()
		{
			var dv = Get(XML_KEY_NAME, "GetAllGroup", new Hashtable());
			return dv.Cast<DataRowView>().Select(item => new PageDesignGroupModel(item)).ToArray();
		}
		#endregion

		#region +InsertGroup グループ登録
		/// <summary>
		/// グループ登録
		/// </summary>
		/// <param name="model">グループモデル</param>
		/// <returns>新規グループID</returns>
		internal int InsertGroup(PageDesignGroupModel model)
		{
			var result = Get(XML_KEY_NAME, "InsertGroup", model.DataSource);
			return int.Parse(result[0][0].ToString());
		}
		#endregion


		#region +UpdateGroup グループ更新
		/// <summary>
		/// グループ更新
		/// </summary>
		/// <param name="model">グループモデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateGroup(PageDesignGroupModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateGroup", model.DataSource);
			return result;
		}
		#endregion

		#region +Delete グループ削除
		/// <summary>
		/// グループ削除
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteGroup(long groupId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PAGEDESIGNGROUP_GROUP_ID, groupId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteGroup", ht);
			return result;
		}
		#endregion
		#endregion

		#region ページ系
		#region +GetPage ページ取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>モデル</returns>
		internal PageDesignModel GetPage(long pageId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PAGEDESIGN_PAGE_ID, pageId},
			};
			var dv = Get(XML_KEY_NAME, "GetPage", ht);
			if (dv.Count == 0) return null;
			return new PageDesignModel(dv[0]);
		}
		#endregion

		#region +GetPageByFileName ページ取得
		/// <summary>
		/// ファイル名で取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>モデル</returns>
		internal PageDesignModel GetPageByFileName(string fileName)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PAGEDESIGN_FILE_NAME, fileName},
			};
			var dv = Get(XML_KEY_NAME, "GetPageByFileName", ht);
			if (dv.Count == 0) return null;
			return new PageDesignModel(dv[0]);
		}
		#endregion

		#region +GetAllPage ページ全取得
		/// <summary>
		/// ページ 全取得
		/// </summary>
		/// <returns>モデル配列</returns>
		internal PageDesignModel[] GetAllPage()
		{
			var dv = Get(XML_KEY_NAME, "GetAllPage");
			return dv.Cast<DataRowView>().Select(item => new PageDesignModel(item)).ToArray();
		}
		#endregion

		#region +InsertPage ページ登録
		/// <summary>
		/// ページ登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>新規ページID</returns>
		internal int InsertPage(PageDesignModel model)
		{
			var result = Get(XML_KEY_NAME, "InsertPage", model.DataSource);
			return int.Parse(result[0][0].ToString());
		}
		#endregion

		#region +UpdateCustomPage カスタムページ更新
		/// <summary>
		/// カスタムページ更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateCustomPage(PageDesignModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateCustomPage", model.DataSource);
			return result;
		}
		#endregion

		#region +UpdateNormalPage 標準ページ更新
		/// <summary>
		/// 標準ページ更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateNormalPage(PageDesignModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateNormalPage", model.DataSource);
			return result;
		}
		#endregion

		#region +UpdatePageSort ページ順序 更新
		/// <summary>
		/// ページ順序 更新
		/// </summary>
		/// <param name="model">モデル配列</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdatePageGroupSort(PageDesignModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdatePageGroupSort", model.DataSource);
			return result;
		}
		#endregion

		#region +DeletePage ページ削除
		/// <summary>
		/// ページ削除
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeletePage(long pageId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PAGEDESIGN_PAGE_ID, pageId},
			};
			var result = Exec(XML_KEY_NAME, "DeletePage", ht);
			return result;
		}
		#endregion

		#region +UpdatePageMoveOtherGroup ページをその他グループに移動
		/// <summary>
		/// ページをその他グループに移動
		/// </summary>
		/// <param name="groupId">変更対象のグループID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdatePageMoveOtherGroup(long groupId, string lastChanged)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PAGEDESIGN_GROUP_ID, groupId},
				{Constants.FIELD_PAGEDESIGN_LAST_CHANGED, lastChanged},
			};
			var result = Exec(XML_KEY_NAME, "UpdatePageMoveOtherGroup", ht);
			return result;
		}
		#endregion

		#region +NotExistGroupIds 存在しないグループと紐づいているページのグループIDを抽出
		/// <summary>
		/// 存在しないグループと紐づいているページのグループIDを抽出
		/// </summary>
		/// <returns>モデル</returns>
		internal long[] NotExistGroupIds()
		{
			var dv = Get(XML_KEY_NAME, "NotExistGroupIds");
			if (dv.Count == 0) return new long[] { };
			return dv.Cast<DataRowView>().Select(drv => (long)drv[0]).ToArray();
		}
		#endregion

		#endregion

		#region サイトマップ用
		/// <summary>
		/// CMS管理のページをすべて取得
		/// </summary>
		/// <returns>XMLサイトマップ用ページモデル</returns>
		internal SitemapPageModel[] GetAllManagedPagesForSitemap()
		{
			var dv = Get(XML_KEY_NAME, "GetAllManagedPagesForSitemap");
			return dv.Cast<DataRowView>().Select(drv => new SitemapPageModel(drv)).ToArray();
		}
		#endregion

		#region +CheckTargetListUsed ページ管理で使われているか
		/// <summary>
		/// ページ管理で使われているか
		/// </summary>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>ページ管理モデル</returns>
		public PageDesignModel[] CheckTargetListUsed(string targetId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_TARGETLIST_TARGET_ID, targetId },
			};
			var dv = Get(XML_KEY_NAME, "CheckTargetListUsed", ht);
			return dv.Cast<DataRowView>().Select(drv => new PageDesignModel(drv)).ToArray();
		}
		#endregion
	}
}
