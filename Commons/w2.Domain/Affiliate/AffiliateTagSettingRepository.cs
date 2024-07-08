/*
=========================================================================================================
  Module      : アフィリエイトタグ設定リポジトリ (AffiliateTagSettingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.Affiliate.Helper;

namespace w2.Domain.Affiliate
{
	/// <summary>
	/// アフィリエイトタグ設定リポジトリ
	/// </summary>
	internal class AffiliateTagSettingRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "AffiliateTagSetting";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal AffiliateTagSettingRepository()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal AffiliateTagSettingRepository(SqlAccessor accessor) : base(accessor)
		{
		}
		#endregion

		#region AffiliateTagSetting系
		#region ~GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="replaces">置換値</param>
		/// <returns>件数</returns>
		internal int GetSearchHitCount(AffiliateTagSettingListSearchCondition condition, params KeyValuePair<string, string>[] replaces)
		{
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", condition.CreateHashtableParams(), null, replaces);
			return (int)dv[0][0];
		}
		#endregion

		#region ~Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="replaces">置換値</param>
		/// <returns>検索結果列</returns>
		internal AffiliateTagSettingListSearchResult[] Search(AffiliateTagSettingListSearchCondition condition, params KeyValuePair<string, string>[] replaces)
		{
			var dv = Get(XML_KEY_NAME, "Search", condition.CreateHashtableParams(), null, replaces);
			return dv.Cast<DataRowView>().Select(drv => new AffiliateTagSettingListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~SearchByKeyword キーワード検索
		/// <summary>
		/// キーワード検索
		/// </summary>
		/// <param name="searchWord">キーワード</param>
		/// <returns>検索結果列</returns>
		internal AffiliateTagSettingModel[] SearchByKeyword(string searchWord)
		{
			int affiliateId;
			int.TryParse(searchWord, out affiliateId);
			var ht = new Hashtable
			{
				{ Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_ID, affiliateId },
				{ Constants.FIELD_AFFILIATETAGSETTING_SEARCH_WORD, searchWord },
			};
			var dv = Get(XML_KEY_NAME, "SearchByKeyword", ht);
			return dv.Cast<DataRowView>().Select(drv => new AffiliateTagSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="affiliateId">アフィリエイトID</param>
		/// <returns>モデル</returns>
		internal AffiliateTagSettingModel Get(int affiliateId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_ID, affiliateId },
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new AffiliateTagSettingModel(dv[0]);
		}
		#endregion

		#region ~GetConsiderationCondition 出力条件を考慮して取得
		/// <summary>
		/// 出力条件管理を考慮して取得
		/// </summary>
		/// <param name="affiliateId">アフィリエイトID</param>
		/// <returns>タグ情報</returns>
		internal AffiliateTagSettingModel[] GetConsiderationCondition(int affiliateId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_ID, affiliateId },
			};
			var dv = Get(XML_KEY_NAME, "GetConsiderationCondition", ht);
			return dv.Cast<DataRowView>().Select(item => new AffiliateTagSettingModel(item)).ToArray();
		}
		#endregion

		#region ~GetAll 全取得
		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>モデル</returns>
		internal AffiliateTagSettingModel[] GetAll()
		{
			var dv = Get(XML_KEY_NAME, "GetAll");
			return dv.Cast<DataRowView>().Select(item => new AffiliateTagSettingModel(item)).ToArray();
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal int Insert(AffiliateTagSettingModel model)
		{
			return (int)(decimal)Get(XML_KEY_NAME, "Insert", model.DataSource)[0][Constants
				.FIELD_AFFILIATETAGSETTING_AFFILIATE_ID];
		}
		#endregion

		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(AffiliateTagSettingModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region ~Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="affiliateId">アフィリエイトID</param>
		internal int Delete(int affiliateId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_ID, affiliateId },
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#endregion

		#region AffiliateTagCondition系
		#region ~AffiliateTagConditionGetAllByAffiliateId 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="affiliateId">アフィリエイトID</param>
		/// <returns>モデル</returns>
		internal AffiliateTagConditionModel[] AffiliateTagConditionGetAllByAffiliateId(int affiliateId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_AFFILIATETAGCONDITION_AFFILIATE_ID, affiliateId },
			};
			var dv = Get(XML_KEY_NAME, "AffiliateTagConditionGetAllByAffiliateId", ht);
			return dv.Cast<DataRowView>().Select(item => new AffiliateTagConditionModel(item)).ToArray();
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void AffiliateTagConditionInsert(AffiliateTagConditionModel model)
		{
			Exec(XML_KEY_NAME, "AffiliateTagConditionInsert", model.DataSource);
		}
		#endregion

		#region ~DeleteAll 全削除
		/// <summary>
		/// 全削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="affiliateId">アフィリエイトID</param>
		internal int AffiliateTagConditionDeleteAll(int affiliateId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_AFFILIATETAGCONDITION_AFFILIATE_ID, affiliateId },
			};
			var result = Exec(XML_KEY_NAME, "AffiliateTagConditionDeleteAll", ht);
			return result;
		}
		#endregion
		#endregion

		#region AffiliateProductTagSetting系
		#region ~AffiliateProductTagSettingGetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		internal int AffiliateProductTagSettingGetSearchHitCount(
			AffiliateProductTagSettingListSearchCondition condition)
		{
			var dv = Get(
				XML_KEY_NAME,
				"AffiliateProductTagSettingGetSearchHitCount",
				condition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion

		#region ~AffiliateProductTagSettingSearch 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		internal AffiliateProductTagSettingListSearchResult[] AffiliateProductTagSettingSearch(
			AffiliateProductTagSettingListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "AffiliateProductTagSettingSearch", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new AffiliateProductTagSettingListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~AffiliateProductTagSettingGet 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="affiliateProductTagId">アフィリエイト商品タグID</param>
		/// <returns>モデル</returns>
		internal AffiliateProductTagSettingModel AffiliateProductTagSettingGet(int affiliateProductTagId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_AFFILIATE_PRODUCT_TAG_ID, affiliateProductTagId },
			};
			var dv = Get(XML_KEY_NAME, "AffiliateProductTagSettingGet", ht);
			if (dv.Count == 0) return null;
			return new AffiliateProductTagSettingModel(dv[0]);
		}
		#endregion

		#region ~AffiliateProductTagSettingGetAll 全取得
		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>取得結果</returns>
		internal AffiliateProductTagSettingModel[] AffiliateProductTagSettingGetAll()
		{
			var dv = Get(XML_KEY_NAME, "AffiliateProductTagSettingGetAll");
			return dv.Cast<DataRowView>().Select(drv => new AffiliateProductTagSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~AffiliateProductTagSettingInsert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void AffiliateProductTagSettingInsert(AffiliateProductTagSettingModel model)
		{
			Exec(XML_KEY_NAME, "AffiliateProductTagSettingInsert", model.DataSource);
		}
		#endregion

		#region ~AffiliateProductTagSettingUpdate 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int AffiliateProductTagSettingUpdate(AffiliateProductTagSettingModel model)
		{
			var result = Exec(XML_KEY_NAME, "AffiliateProductTagSettingUpdate", model.DataSource);
			return result;
		}
		#endregion

		#region ~AffiliateProductTagSettingDelete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="affiliateProductTagId">アフィリエイト商品タグID</param>
		internal int AffiliateProductTagSettingDelete(int affiliateProductTagId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_AFFILIATE_PRODUCT_TAG_ID, affiliateProductTagId },
			};
			var result = Exec(XML_KEY_NAME, "AffiliateProductTagSettingDelete", ht);
			return result;
		}
		#endregion
		#endregion
	}
}