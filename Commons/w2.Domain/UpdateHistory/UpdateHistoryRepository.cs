/*
=========================================================================================================
  Module      : 更新履歴情報リポジトリ (UpdateHistoryRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain.UpdateHistory
{
	/// <summary>
	/// 更新履歴情報リポジトリ
	/// </summary>
	public class UpdateHistoryRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "UpdateHistory";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UpdateHistoryRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateHistoryRepository(SqlAccessor accessor)
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
		internal int GetSearchHitCount(UpdateHistoryListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", condition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion

		#region ~Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		internal UpdateHistoryListSearchResult[] Search(UpdateHistoryListSearchCondition condition)
		{
			var dv = GetUseLiteralSql(XML_KEY_NAME, "Search", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new UpdateHistoryListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~BeforeAfterSearch 検索（変更前後）
		/// <summary>
		/// 検索（変更前後）
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		internal UpdateHistoryBeforeAndAfterSearchResult BeforeAfterSearch(UpdateHistoryBeforeAndAfterSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "BeforeAfterSearch", condition.CreateHashtableParams());
			return new UpdateHistoryBeforeAndAfterSearchResult(dv);
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="updateHistoryNo">更新履歴No</param>
		/// <returns>モデル</returns>
		internal UpdateHistoryModel Get(long updateHistoryNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_UPDATEHISTORY_UPDATE_HISTORY_NO, updateHistoryNo},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new UpdateHistoryModel(dv[0]);
		}
		#endregion

		#region ~Register  登録（同じものが登録されていなければ）
		/// <summary>
		///  登録（同じものが登録されていなければ）
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		internal decimal Register(UpdateHistoryModel model)
		{
			var historyNo = (decimal)Get(XML_KEY_NAME, "Register", model.DataSource)[0][0];
			return historyNo;
		}
		#endregion
	}
}