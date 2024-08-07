/*
=========================================================================================================
  Module      : ノベルティ設定リポジトリ (NoveltyRepository.cs)
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

namespace w2.Domain.Novelty
{
	/// <summary>
	/// ノベルティ設定リポジトリ
	/// </summary>
	public class NoveltyRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "Novelty";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public NoveltyRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public NoveltyRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(Hashtable param)
		{
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", param);
			return (int)dv[0][0];
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
			var dv = Get(XML_KEY_NAME, "Search", param);
			return dv.Cast<DataRowView>().Select(drv => new NoveltyModel(drv)).ToArray();
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
			var ht = new Hashtable
			{
				{Constants.FIELD_NOVELTY_SHOP_ID, shopId},
				{Constants.FIELD_NOVELTY_NOVELTY_ID, noveltyId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new NoveltyModel(dv[0]);
		}
		#endregion

		#region +GetAll 検索
		/// <summary>
		/// 全て取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル列</returns>
		public NoveltyModel[] GetAll(string shopId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_NOVELTY_SHOP_ID, shopId}
			};
			var dv = Get(XML_KEY_NAME, "GetAll", ht);
			return dv.Cast<DataRowView>().Select(drv => new NoveltyModel(drv)).ToArray();
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(NoveltyModel model)
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
		public int Update(NoveltyModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="shopId">店舗ID</param>
		/// <param name="noveltyId">ノベルティID</param>
		public int Delete(string shopId, string noveltyId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_NOVELTY_SHOP_ID, shopId},
				{Constants.FIELD_NOVELTY_NOVELTY_ID, noveltyId},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region +GetTargetItemAll 対象アイテム取得（全て）
		/// <summary>
		/// 対象アイテム取得（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="noveltyId">ノベルティID</param>
		/// <returns>モデル列</returns>
		public NoveltyTargetItemModel[] GetTargetItemAll(string shopId, string noveltyId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_NOVELTYTARGETITEM_SHOP_ID, shopId},
				{Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_ID, noveltyId}
			};
			var dv = Get(XML_KEY_NAME, "GetTargetItemAll", ht);
			return dv.Cast<DataRowView>().Select(drv => new NoveltyTargetItemModel(drv)).ToArray();
		}
		#endregion

		#region +InsertTargetItem 対象アイテム登録
		/// <summary>
		/// 対象アイテム登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertTargetItem(NoveltyTargetItemModel model)
		{
			Exec(XML_KEY_NAME, "InsertTargetItem", model.DataSource);
		}
		#endregion

		#region +DeleteTargetItemAll 対象アイテム削除（全て）
		/// <summary>
		/// 対象アイテム削除（全て）
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="shopId">店舗ID</param>
		/// <param name="noveltyId">ノベルティID</param>
		public int DeleteTargetItemAll(string shopId, string noveltyId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_NOVELTYTARGETITEM_SHOP_ID, shopId},
				{Constants.FIELD_NOVELTYTARGETITEM_NOVELTY_ID, noveltyId}
			};
			var result = Exec(XML_KEY_NAME, "DeleteTargetItemAll", ht);
			return result;
		}
		#endregion

		#region +GetGrantConditionsAll 付与条件取得（全て）
		/// <summary>
		/// 付与条件取得（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="noveltyId">ノベルティID</param>
		/// <returns>モデル列</returns>
		public NoveltyGrantConditionsModel[] GetGrantConditionsAll(string shopId, string noveltyId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_NOVELTYGRANTCONDITIONS_SHOP_ID, shopId},
				{Constants.FIELD_NOVELTYGRANTCONDITIONS_NOVELTY_ID, noveltyId}
			};
			var dv = Get(XML_KEY_NAME, "GetGrantConditionsAll", ht);
			return dv.Cast<DataRowView>().Select(drv => new NoveltyGrantConditionsModel(drv)).ToArray();
		}
		#endregion

		#region +InsertGrantConditions 付与条件登録
		/// <summary>
		/// 付与条件登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertGrantConditions(NoveltyGrantConditionsModel model)
		{
			Exec(XML_KEY_NAME, "InsertGrantConditions", model.DataSource);
		}
		#endregion

		#region +DeleteGrantConditionsAll 付与条件削除（全て）
		/// <summary>
		/// 付与条件削除（全て）
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="shopId">店舗ID</param>
		/// <param name="noveltyId">ノベルティID</param>
		public int DeleteGrantConditionsAll(string shopId, string noveltyId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_NOVELTYGRANTCONDITIONS_SHOP_ID, shopId},
				{Constants.FIELD_NOVELTYGRANTCONDITIONS_NOVELTY_ID, noveltyId}
			};
			var result = Exec(XML_KEY_NAME, "DeleteGrantConditionsAll", ht);
			return result;
		}
		#endregion

		#region +GetGrantItemAll 付与アイテム取得（全て）
		/// <summary>
		/// 付与アイテム取得（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="noveltyId">ノベルティID</param>
		/// <returns>モデル列</returns>
		public NoveltyGrantItemModel[] GetGrantItemAll(string shopId, string noveltyId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_NOVELTYGRANTITEM_SHOP_ID, shopId},
				{Constants.FIELD_NOVELTYGRANTITEM_NOVELTY_ID, noveltyId}
			};
			var dv = Get(XML_KEY_NAME, "GetGrantItemAll", ht);
			return dv.Cast<DataRowView>().Select(drv => new NoveltyGrantItemModel(drv)).ToArray();
		}
		#endregion

		#region +InsertGrantItem 付与アイテム登録
		/// <summary>
		/// 付与アイテム登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertGrantItem(NoveltyGrantItemModel model)
		{
			Exec(XML_KEY_NAME, "InsertGrantItem", model.DataSource);
		}
		#endregion

		#region +DeleteGrantItemAll 付与アイテム削除
		/// <summary>
		/// 付与アイテム削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="shopId">店舗ID</param>
		/// <param name="noveltyId">ノベルティID</param>
		public int DeleteGrantItemAll(string shopId, string noveltyId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_NOVELTYGRANTITEM_SHOP_ID, shopId},
				{Constants.FIELD_NOVELTYGRANTITEM_NOVELTY_ID, noveltyId}
			};
			var result = Exec(XML_KEY_NAME, "DeleteGrantItemAll", ht);
			return result;
		}
		#endregion
	}
}
