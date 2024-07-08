/*
=========================================================================================================
  Module      : 広告コードサービスのインターフェース (IAdvCodeService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;
using w2.Domain.AdvCode.Helper;

namespace w2.Domain.AdvCode
{
	/// <summary>
	/// 広告コードサービスのインターフェース
	/// </summary>
	public interface IAdvCodeService : IService
	{
		/// <summary>
		/// IN条件生成
		/// </summary>
		/// <param name="column">カラム名</param>
		/// <param name="value">対象値</param>
		/// <returns>条件</returns>
		string CreateWhereInCondition(string column, string[] value);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="advcodeNo">広告コードNO</param>
		void DeleteAdvCode(long advcodeNo);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="advcodeMediaTypeId">区分ID</param>
		void DeleteAdvCodeMediaType(string advcodeMediaTypeId);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="advcodeNo">広告コードNO</param>
		/// <returns>モデル</returns>
		AdvCodeModel GetAdvCode(long advcodeNo);

		/// <summary>
		/// Get advertisement code from advertisement code media type id
		/// </summary>
		/// <param name="advCodeMediaType">AdvCode Media Type</param>
		/// <returns>advcode no</returns>
		int GetAdvCodeFromAdvcodeMediaTypeId(string advCodeMediaType);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="advertisementCode">広告コード</param>
		/// <returns>モデル</returns>
		AdvCodeModel GetAdvCodeFromAdvertisementCode(string advertisementCode);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="advcodeMediaTypeId">区分ID</param>
		/// <returns>モデル</returns>
		AdvCodeMediaTypeModel GetAdvCodeMediaType(string advcodeMediaTypeId);

		/// <summary>
		/// Get AdvCode Media Type List All
		/// </summary>
		/// <returns>AdvCode Media Type List</returns>
		AdvCodeMediaTypeModel[] GetAdvCodeMediaTypeListAll();

		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索パラメタ</param>
		/// <returns>件数</returns>
		int GetAdvCodeMediaTypeSearchHitCount(AdvCodeMediaTypeListSearchCondition condition);

		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索パラメタ</param>
		/// <param name="advertisementCode">閲覧可能広告コード</param>
		/// <returns>件数</returns>
		int GetAdvCodeSearchHitCount(AdvCodeListSearchCondition condition, string[] advertisementCode = null);

		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>モデルリスト</returns>
		AdvCodeModel[] GetAll();

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		void InsertAdvCode(AdvCodeModel model);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		void InsertAdvCodeMediaType(AdvCodeMediaTypeModel model);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索パラメタ</param>
		/// /// <param name="advertisementCode">閲覧可能広告コード</param>
		/// <returns>モデル列</returns>
		AdvCodeListSearchResult[] SearchAdvCode(AdvCodeListSearchCondition condition, string[] advertisementCode = null);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索パラメタ</param>
		/// <returns>モデル列</returns>
		AdvCodeMediaTypeListSearchResult[] SearchAdvCodeMediaType(AdvCodeMediaTypeListSearchCondition condition);

		/// <summary>
		/// キーワードから広告媒体区分を検索
		/// </summary>
		/// <param name="searchWord">検索キーワード</param>
		/// <returns>検索結果列</returns>
		AdvCodeMediaTypeModel[] SearchMediaTypeByKeyword(string searchWord);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		int UpdateAdvCode(AdvCodeModel model);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">Accessor</param>
		/// <returns></returns>
		int UpdateAdvCodeMediaType(AdvCodeMediaTypeModel model, SqlAccessor accessor);

		/// <summary>
		/// Search advertisement codes for autosuggest
		/// </summary>
		/// <param name="searchWord">Search word</param>
		/// <returns>Advertisement codes</returns>
		AdvCodeModel[] SearchAdvCodesForAutosuggest(string searchWord);
	}
}