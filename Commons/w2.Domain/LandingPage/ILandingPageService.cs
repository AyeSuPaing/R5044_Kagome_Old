/*
=========================================================================================================
  Module      : Lpページサービスのインターフェース (ILandingPageService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Domain.LandingPage.Helper;

namespace w2.Domain.LandingPage
{
	/// <summary>
	/// Lpページサービスのインターフェース
	/// </summary>
	public interface ILandingPageService : IService
	{
		/// <summary>
		/// ページ削除
		/// </summary>
		/// <param name="pageId">ページID</param>
		void DeletePageData(string pageId);

		/// <summary>
		/// ページ情報を取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>モデル</returns>
		LandingPageDesignModel Get(string pageId);

		/// <summary>
		/// 全ページ取得
		/// </summary>
		/// <returns></returns>
		LandingPageDesignModel[] GetAllPage();

		/// <summary>
		/// LP件数取得
		/// </summary>
		/// <returns>LP件数</returns>
		int GetCount();

		/// <summary>
		/// ファイル名からページを取得
		/// </summary>
		/// <param name="pageFileName">ファイル名</param>
		/// <returns>ページ情報</returns>
		LandingPageDesignModel[] GetPageByFileName(string pageFileName);

		/// <summary>
		/// デザイン情報とともにページ情報を取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>モデル</returns>
		LandingPageDesignModel GetPageDataWithDesign(string pageId);
		/// <summary>
		/// デザイン情報とともにページ情報を取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="designType">デザインタイプ</param>
		/// <returns>モデル</returns>
		LandingPageDesignModel GetPageDataWithDesign(string pageId, string designType);

		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>商品情報</returns>
		LandingPageProductModel[] GetPageProducts(string pageId);

		/// <summary>
		/// 商品セット情報取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>商品セット情報</returns>
		LandingPageProductSetModel[] GetPageProductSets(string pageId);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		void Insert(LandingPageDesignModel model);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="publicDateKbn">公開日条件区分</param>
		/// <param name="searchWord">検索条件：タイトル又はファイル名</param>
		/// <param name="publicStatus">公開状態</param>
		/// <param name="designMode">デザインモード</param>
		/// <returns>検索結果列</returns>
		LandingPageDesignModel[] Search(string publicDateKbn, string searchWord, string publicStatus, string designMode);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		int UpdatePage(LandingPageDesignModel model);

		/// <summary>
		/// ページデザイン更新
		/// </summary>
		/// <param name="model"></param>
		void UpdatePageDesign(LandingPageDesignModel model);

		/// <summary>
		/// パラメタ検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		int GetCountOfSearchByParamModel(LandingPageSearchParamModel condition);

		/// <summary>
		/// パラメタ検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>Lpページ</returns>
		LandingPageDesignModel[] SearchByParamModel(LandingPageSearchParamModel condition);

		/// <summary>
		/// ABテストアイテム件数取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>件数</returns>
		int GetCountInAbTestItemByPageId(string pageId);
	}
}