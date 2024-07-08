/*
=========================================================================================================
  Module      : 商品一覧表示設定サービスのインターフェース (IProductListDispSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
namespace w2.Domain.ProductListDispSetting
{
	/// <summary>
	/// 商品一覧表示設定サービスのインターフェース
	/// </summary>
	public interface IProductListDispSettingService : IService
	{
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="settingId">設定ID</param>
		/// <param name="settingKbn">設定区分</param>
		/// <returns>影響を受けた数</returns>
		int Delete(string settingId, string settingKbn);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="settingKbn">設定区分</param>
		/// <returns>影響を受けた数</returns>
		int DeleteBySettingKbn(string settingKbn);

		/// <summary>
		/// すべて取得
		/// </summary>
		/// <returns>モデル列</returns>
		ProductListDispSettingModel[] GetAll();

		/// <summary>
		/// 利用可能なもの取得
		/// </summary>
		/// <returns>モデル列</returns>
		ProductListDispSettingModel[] GetUsable();

		/// <summary>
		/// 利用可能なもの取得（設定区分指定）
		/// </summary>
		/// <param name="settingKbn">設定区分</param>
		/// <returns>モデル配列</returns>
		ProductListDispSettingModel[] GetUsable(string settingKbn);

		/// <summary>
		/// 件数登録
		/// </summary>
		/// <param name="models">モデル配列</param>
		int InsertCountSetting(ProductListDispSettingModel[] models);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		int Update(ProductListDispSettingModel model);
	}
}