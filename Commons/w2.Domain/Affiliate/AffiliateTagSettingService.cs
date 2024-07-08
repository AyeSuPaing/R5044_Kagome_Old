/*
=========================================================================================================
  Module      : アフィリエイトタグ設定サービス (AffiliateTagSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using w2.Common.Extensions;
using w2.Common.Util;
using w2.Domain.Affiliate.Helper;

namespace w2.Domain.Affiliate
{
	/// <summary>
	/// アフィリエイトタグ設定サービス
	/// </summary>
	public class AffiliateTagSettingService : ServiceBase, IAffiliateTagSettingService
	{
		/// <summary>アフィリエイトID抽出置換値</summary>
		private const string WHERE_AFFILIATE_ID = "@@ where_affiliate_id @@";
		/// <summary>広告媒体区分抽出置換値</summary>
		private const string WHERE_MEDIA_TYPE_ID = "@@ where_media_type_id @@";
		/// <summary>設置個所抽出置換値</summary>
		private const string WHERE_LOCATION = "@@ where_location @@";

		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="tagIds">閲覧可能なタグID(配列)</param>
		/// <param name="mediaTypeIds">閲覧可能な広告媒体区分(配列)</param>
		/// <param name="locations">閲覧可能な設置個所(配列)</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(
			AffiliateTagSettingListSearchCondition condition,
			string[] tagIds,
			string[] mediaTypeIds,
			string[] locations)
		{
			var replaces = CreateWhereConditionReplaces(tagIds, mediaTypeIds, locations);

			using (var repository = new AffiliateTagSettingRepository())
			{
				var count = repository.GetSearchHitCount(condition, replaces);
				return count;
			}
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="tagIds">閲覧可能なタグID(配列)</param>
		/// <param name="mediaTypeIds">閲覧可能な広告媒体区分(配列)</param>
		/// <param name="locations">閲覧可能な設置個所(配列)</param>
		/// <returns>検索結果列</returns>
		public AffiliateTagSettingListSearchResult[] Search(
			AffiliateTagSettingListSearchCondition condition,
			string[] tagIds,
			string[] mediaTypeIds,
			string[] locations)
		{
			var replaces = CreateWhereConditionReplaces(tagIds, mediaTypeIds, locations);

			using (var repository = new AffiliateTagSettingRepository())
			{
				var results = repository.Search(condition, replaces);
				return results;
			}
		}
		#endregion

		#region +SearchByKeyword キーワード検索
		/// <summary>
		/// キーワード検索
		/// </summary>
		/// <param name="searchWord">検索キーワード</param>
		/// <returns>検索結果列</returns>
		public AffiliateTagSettingModel[] SearchByKeyword(string searchWord)
		{
			using (var repository = new AffiliateTagSettingRepository())
			{
				var results = repository.SearchByKeyword(searchWord);
				return results;
			}
		}
		#endregion

		/// <summary>
		/// 閲覧制限抽出条件置換式配列生成
		/// </summary>
		/// <param name="tagIds">閲覧可能なタグID(配列)</param>
		/// <param name="mediaTypeIds">閲覧可能な広告媒体区分(配列)</param>
		/// <param name="locations">閲覧可能な設置個所(配列)</param>
		/// <returns>抽出条件置換式配列</returns>
		private KeyValuePair<string, string>[] CreateWhereConditionReplaces(string[] tagIds, string[] mediaTypeIds, string[] locations)
		{
			var affiliateWhereCondition = CreateWhereCondition(Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_ID, tagIds);
			var mediaTypeWhereCondition = CreateWhereCondition(Constants.FIELD_AFFILIATETAGCONDITION_CONDITION_VALUE, mediaTypeIds);
			var locationWhereConditioin = CreateWhereCondition(Constants.FIELD_AFFILIATETAGCONDITION_CONDITION_VALUE, locations);

			var replaces = new[]
			{
				new KeyValuePair<string, string>(WHERE_AFFILIATE_ID, affiliateWhereCondition),
				new KeyValuePair<string, string>(WHERE_MEDIA_TYPE_ID, mediaTypeWhereCondition),
				new KeyValuePair<string, string>(WHERE_LOCATION, locationWhereConditioin),
			};

			return replaces;
		}

		/// <summary>
		/// 閲覧制限抽出条件置換式生成
		/// </summary>
		/// <param name="field">カラム名</param>
		/// <param name="values">閲覧可能な対象</param>
		/// <returns>抽出条件置換式</returns>
		private static string CreateWhereCondition(string field, string[] values)
		{
			if (values.Any() == false) return string.Empty;

			var whereCondition = string.Format(
				"AND  {0} IN ('',{1})",
				field,
				values.Select(value => string.Format("'{0}'", StringUtility.SqlLikeStringSharpEscape(value))).JoinToString(", "));

			return whereCondition;
		}

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="affiliateId">アフィリエイトID</param>
		/// <returns>モデル</returns>
		public AffiliateTagSettingModel Get(int affiliateId)
		{
			using (var repository = new AffiliateTagSettingRepository())
			{
				var model = repository.Get(affiliateId);
				return model;
			}
		}
		#endregion

		#region +GetConsiderationCondition 出力条件管理を考慮して取得
		/// <summary>
		/// 出力条件管理を考慮して取得
		/// </summary>
		/// <param name="affiliateId">アフィリエイトID</param>
		/// <returns>タグ情報</returns>
		public AffiliateTagSettingModel[] GetConsiderationCondition(int affiliateId)
		{
			using (var repository = new AffiliateTagSettingRepository())
			{
				var model = repository.GetConsiderationCondition(affiliateId);
				return model;
			}
		}
		#endregion

		#region +GetAll 全取得
		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>モデル</returns>
		public AffiliateTagSettingModel[] GetAllIncludeConditionModels()
		{
			using (var repository = new AffiliateTagSettingRepository())
			{
				var model = repository.GetAll();

				foreach (var affiliateTagSettingModel in model)
				{
					if (affiliateTagSettingModel.AffiliateProductTagId != null)
					{
						affiliateTagSettingModel.AffiliateProductTagSettingModel =
							repository.AffiliateProductTagSettingGet(
								affiliateTagSettingModel.AffiliateProductTagId.Value);
					}

					affiliateTagSettingModel.AffiliateTagConditionList =
						repository.AffiliateTagConditionGetAllByAffiliateId(affiliateTagSettingModel.AffiliateId);
				}

				return model;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public int Insert(AffiliateTagSettingModel model)
		{
			using (var repository = new AffiliateTagSettingRepository())
			{
				var id = repository.Insert(model);
				return id;
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int Update(AffiliateTagSettingModel model)
		{
			using (var repository = new AffiliateTagSettingRepository())
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="affiliateId">アフィリエイトID</param>
		public int Delete(int affiliateId)
		{
			using (var repository = new AffiliateTagSettingRepository())
			{
				var result = repository.Delete(affiliateId);
				return result;
			}
		}
		#endregion

		#region AffiliateTagCondition系
		#region +AffiliateTagConditionGetAllByAffiliateId 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="affiliateId">アフィリエイトID</param>
		/// <returns>モデル</returns>
		public AffiliateTagConditionModel[] AffiliateTagConditionGetAllByAffiliateId(int affiliateId)
		{
			using (var repository = new AffiliateTagSettingRepository())
			{
				var models = repository.AffiliateTagConditionGetAllByAffiliateId(affiliateId);
				return models;
			}
		}
		#endregion

		#region +InsertAll 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="models">モデル配列</param>
		public void AffiliateTagConditionInsertAll(AffiliateTagConditionModel[] models)
		{
			using (var repository = new AffiliateTagSettingRepository())
			{
				repository.AffiliateTagConditionDeleteAll(models.FirstOrDefault().AffiliateId);
				foreach (var model in models)
				{
					repository.AffiliateTagConditionInsert(model);
				}
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="affiliateId">アフィリエイトID</param>
		/// <param name="models">モデル配列</param>
		public void AffiliateTagConditionUpdate(int affiliateId, AffiliateTagConditionModel[] models)
		{
			AffiliateTagConditionDeleteAll(affiliateId);
			AffiliateTagConditionInsertAll(models);
		}
		#endregion

		#region +DeleteAll 対象アフィリエイトタグIDに紐づく条件の全削除
		/// <summary>
		/// 対象アフィリエイトタグIDに紐づく条件の全削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="affiliateId">アフィリエイトID</param>
		public int AffiliateTagConditionDeleteAll(int affiliateId)
		{
			using (var repository = new AffiliateTagSettingRepository())
			{
				var result = repository.AffiliateTagConditionDeleteAll(affiliateId);
				return result;
			}
		}
		#endregion
		#endregion

		#region AffiliateProductTagSetting系
		#region +AffiliateProductTagSettingGetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int AffiliateProductTagSettingGetSearchHitCount(AffiliateProductTagSettingListSearchCondition condition)
		{
			using (var repository = new AffiliateTagSettingRepository())
			{
				var count = repository.AffiliateProductTagSettingGetSearchHitCount(condition);
				return count;
			}
		}
		#endregion

		#region +AffiliateProductTagSettingSearch 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		public AffiliateProductTagSettingListSearchResult[] AffiliateProductTagSettingSearch(
			AffiliateProductTagSettingListSearchCondition condition)
		{
			using (var repository = new AffiliateTagSettingRepository())
			{
				var results = repository.AffiliateProductTagSettingSearch(condition);
				return results;
			}
		}
		#endregion

		#region +AffiliateProductTagSettingGet 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="affiliateProductTagId">アフィリエイト商品タグID</param>
		/// <returns>モデル</returns>
		public AffiliateProductTagSettingModel AffiliateProductTagSettingGet(int affiliateProductTagId)
		{
			using (var repository = new AffiliateTagSettingRepository())
			{
				var model = repository.AffiliateProductTagSettingGet(affiliateProductTagId);
				return model;
			}
		}
		#endregion

		#region +AffiliateProductTagSettingGetAll 全取得
		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>検索結果列</returns>
		public AffiliateProductTagSettingModel[] AffiliateProductTagSettingGetAll()
		{
			using (var repository = new AffiliateTagSettingRepository())
			{
				var results = repository.AffiliateProductTagSettingGetAll();
				return results;
			}
		}
		#endregion

		#region +AffiliateProductTagSettingInsert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void AffiliateProductTagSettingInsert(AffiliateProductTagSettingModel model)
		{
			using (var repository = new AffiliateTagSettingRepository())
			{
				repository.AffiliateProductTagSettingInsert(model);
			}
		}
		#endregion

		#region +AffiliateProductTagSettingUpdate 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int AffiliateProductTagSettingUpdate(AffiliateProductTagSettingModel model)
		{
			using (var repository = new AffiliateTagSettingRepository())
			{
				var result = repository.AffiliateProductTagSettingUpdate(model);
				return result;
			}
		}
		#endregion

		#region +AffiliateProductTagSettingDelete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="affiliateProductTagId">アフィリエイト商品タグID</param>
		public int AffiliateProductTagSettingDelete(int affiliateProductTagId)
		{
			using (var repository = new AffiliateTagSettingRepository())
			{
				var result = repository.AffiliateProductTagSettingDelete(affiliateProductTagId);
				return result;
			}
		}
		#endregion
		#endregion
	}
}