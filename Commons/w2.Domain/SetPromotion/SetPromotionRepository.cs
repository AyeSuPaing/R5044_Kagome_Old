/*
=========================================================================================================
  Module      : セットプロモーションリポジトリ (SetPromotionRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.SetPromotion.Helper;

namespace w2.Domain.SetPromotion
{
	/// <summary>
	/// セットプロモーションリポジトリ
	/// </summary>
	public class SetPromotionRepository : RepositoryBase
	{
		/// <summary>キー名</summary>
		private const string XML_KEY_NAME = "SetPromotion";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SetPromotionRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SetPromotionRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(SetPromotionListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", condition.CreateHashtableParams());
			return (int)dv[0][0];
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
			var dv = Get(XML_KEY_NAME, "Search", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new SetPromotionListSearchResult(drv)).ToArray();
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
			var ht = new Hashtable
			{
				{Constants.FIELD_SETPROMOTION_SETPROMOTION_ID, setpromotionId}
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new SetPromotionModel(dv[0]);
		}
		#endregion

		#region +GetUsable 利用可能なもの取得
		/// <summary>
		/// 利用可能なもの取得
		/// </summary>
		/// <returns>モデル列</returns>
		public SetPromotionModel[] GetUsable()
		{
			var dv = Get(XML_KEY_NAME, "GetUsable");
			return dv.Cast<DataRowView>().Select(drv => new SetPromotionModel(drv)).ToArray();
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(SetPromotionModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(SetPromotionModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="setpromotionId">セットプロモーションID</param>
		/// <returns>影響を受けた件数</returns>
		public int Delete(string setpromotionId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SETPROMOTION_SETPROMOTION_ID, setpromotionId},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region +GetItemsAll アイテムすべて取得
		/// <summary>
		/// アイテムすべて取得
		/// </summary>
		/// <param name="ids">ID列</param>
		/// <returns>モデル列</returns>
		public SetPromotionItemModel[] GetItemsAll(params string[] ids)
		{
			if (ids.Length == 0) return new SetPromotionItemModel[0];

			var replace = new KeyValuePair<string, string>(
				"@@ ids @@", string.Join(",", ids.Select(id => string.Format("'{0}'", id)).ToArray()));

			var dv = Get(XML_KEY_NAME, "GetItemsAll", replaces: replace);
			return dv.Cast<DataRowView>().Select(drv => new SetPromotionItemModel(drv)).ToArray();
		}
		#endregion

		#region +DeleteItemsAll アイテムすべて削除
		/// <summary>
		/// アイテムすべて削除
		/// </summary>
		/// <param name="setpromotionId">セットプロモーションID</param>
		/// <returns>影響を受けた件数</returns>
		public int DeleteItemsAll(string setpromotionId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SETPROMOTION_SETPROMOTION_ID, setpromotionId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteItemsAll", ht);
			return result;
		}
		#endregion

		#region +InsertItem アイテム登録
		/// <summary>
		/// アイテム登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertItem(SetPromotionItemModel model)
		{
			Exec(XML_KEY_NAME, "InsertItem", model.DataSource);
		}
		#endregion
	}
}
