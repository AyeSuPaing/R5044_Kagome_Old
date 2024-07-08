/*
=========================================================================================================
  Module      : 特集ページサービス (IFeaturePageService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.FeaturePage
{
	/// <summary>
	/// 特集ページサービス
	/// </summary>
	public interface IFeaturePageService : IService
	{
		/// <summary>
		/// 全てのページ取得
		/// </summary>
		/// <returns>特集ページモデル配列</returns>
		FeaturePageModel[] GetAllPage();

		/// <summary>
		/// 全てのページ取得
		/// </summary>
		/// <returns>特集ページモデル配列</returns>
		FeaturePageModel[] GetAllPageWithContents();

		/// <summary>
		/// 特集ページ取得
		/// </summary>
		/// <param name="pageId">特集ページID</param>
		/// <returns>モデル</returns>
		FeaturePageModel Get(long pageId);

		/// <summary>
		/// 取得件数を指定して特集ページ取得
		/// </summary>
		/// <param name="num">取得件数</param>
		/// <returns>モデル</returns>
		FeaturePageModel GetBySpecifyingNumber(int num);

		/// <summary>
		/// ファイル名で取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>モデル</returns>
		FeaturePageModel GetByFileName(string fileName);

		/// <summary>
		/// 特集ページIDでコンテンツをすべて取得
		/// </summary>
		/// <param name="pageId">特集ページID</param>
		/// <returns>特集ページコンテンツモデル配列</returns>
		FeaturePageContentsModel[] GetContents(long pageId);

		/// <summary>
		/// 特集ページ登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>新規特集ページID</returns>
		int Insert(FeaturePageModel model);

		/// <summary>
		/// 特集ページ更新
		/// </summary>
		/// <param name="model">モデル</param>
		void Update(FeaturePageModel model);

		/// <summary>
		/// 特集ページの表示順更新
		/// </summary>
		/// <param name="models">特集ページモデル配列</param>
		void UpdatePageSort(FeaturePageModel[] models);

		/// <summary>
		///  管理用タイトル更新
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="title">管理用タイトル</param>
		void UpdateManagementTitle(long pageId, string title);

		/// <summary>
		/// 特集ページ削除
		/// </summary>
		/// <param name="pageId">特集ページID</param>
		/// <returns>影響を受けた件数</returns>
		int Delete(long pageId);

		/// <summary>
		/// 最上位カテゴリ取得
		/// </summary>
		/// <returns>最上位カテゴリ情報</returns>
		FeaturePageModel[] GetRootCategoryItem();

		/// <summary>
		/// 子カテゴリ取得
		/// </summary>
		/// <returns>子カテゴリ情報</returns>
		FeaturePageModel[] GetChildCategoryItem(string parentCategoryId);

		/// <summary>
		/// ブランド情報取得
		/// </summary>
		/// <returns>ブランド情報</returns>
		FeaturePageModel[] GetBrand();
	}
}
