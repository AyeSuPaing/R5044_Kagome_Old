/*
=========================================================================================================
  Module      : AbTestサービスのインターフェース (IAbTestService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.AbTest
{
	/// <summary>
	/// Abテストサービスのインターフェース
	/// </summary>
	public interface IAbTestService : IService
	{
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="searchWord">検索条件：タイトル又はファイル名</param>
		/// <returns>検索結果列</returns>
		AbTestModel[] Search(string searchWord);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="abTestId">ABテストID</param>
		AbTestModel Get(string abTestId);

		/// <summary>
		/// ABテストアイテム取得
		/// </summary>
		/// <param name="abTestId">ABテストID</param>
		/// <returns>ABテストアイテム</returns>
		AbTestItemModel[] GetAllItemByAbTestId(string abTestId);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		void Insert(AbTestModel model);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		int Update(AbTestModel model);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="abTestId">ABテストID</param>
		/// <returns>影響を受けたABテストの件数</returns>
		int Delete(string abTestId);
	}
}
