/*
=========================================================================================================
  Module      : 商品グループサービスのインターフェース (IProductGroupService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Domain.ProductGroup.Helper;

namespace w2.Domain.ProductGroup
{
	/// <summary>
	/// 商品グループサービスのインターフェース
	/// </summary>
	public interface IProductGroupService : IService
	{
		/// <summary>
		/// 商品グループID重複チェック
		/// </summary>
		/// <param name="productGroupId">商品グループID</param>
		/// <returns>チェック結果</returns>
		bool CheckDupulicationProductGroupId(string productGroupId);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="productGroupId">商品グループID</param>
		void Delete(string productGroupId);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="productGroupId">商品グループID</param>
		/// <returns>モデル</returns>
		ProductGroupModel Get(string productGroupId);

		/// <summary>
		/// 商品グループ全件取得
		/// </summary>
		/// <returns>モデル</returns>
		ProductGroupModel[] GetAllProductGroup();

		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		int GetSearchHitCount(ProductGroupListSearchCondition condition);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		void Insert(ProductGroupModel model);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		ProductGroupListSearchResult[] Search(ProductGroupListSearchCondition condition);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		void Update(ProductGroupModel model);
	}
}