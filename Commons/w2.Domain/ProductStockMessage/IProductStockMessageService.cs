/*
=========================================================================================================
  Module      : 商品在庫文言サービスのインターフェース (IProductStockMessageService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Data;

namespace w2.Domain.ProductStockMessage
{
	/// <summary>
	/// 商品在庫文言サービスのインターフェース
	/// </summary>
	public interface IProductStockMessageService : IService
	{
		/// <summary>
		/// グローバル設定登録
		/// </summary>
		/// <param name="model">モデル</param>
		void InsertGlobalSetting(ProductStockMessageGlobalModel model);

		/// <summary>
		/// グローバル設定更新
		/// </summary>
		/// <param name="model">モデル</param>
		int UpdateGlobalSetting(ProductStockMessageGlobalModel model);

		/// <summary>
		/// グローバル設定削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="stockMessageId">商品在庫文言ID</param>
		void DeleteGlobalSetting(string shopId, string stockMessageId);

		/// <summary>
		/// 商品在庫文言翻訳情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="stockMessageId">商品在庫文言ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>モデル</returns>
		ProductStockMessageModel GetProductStockMessageTranslationSetting(
			string shopId,
			string stockMessageId,
			string languageCode,
			string languageLocaleId);

		/// <summary>
		/// 商品在庫文言翻訳情報取得(DataView)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="stockMessageId">商品在庫文言ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>データビュー</returns>
		DataView GetProductStockMessageTranslationSettingDataView(
			string shopId,
			string stockMessageId,
			string languageCode,
			string languageLocaleId);

		/// <summary>
		/// 言語コードで商品在庫文言情報取得(DataView)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="stockMessageId">商品在庫文言ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>モデル</returns>
		DataView GetProductStockMessageDataView(
			string shopId,
			string stockMessageId,
			string languageCode,
			string languageLocaleId);

		/// <summary>
		/// 在庫文言設定情報に設定されている言語コード設定情報を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="stockMessageId">商品在庫文言ID</param>
		/// <returns>言語コード設定情報</returns>
		ProductStockMessageModel[] GetProductStockMessageLanguageCodeSettings(
			string shopId,
			string stockMessageId);

		/// <summary>
		/// Get product stock messages
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <returns>A collection of product stock messages</returns>
		ProductStockMessageModel[] GetProductStockMessages(string shopId);

		/// <summary>
		/// Get product stock message
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="stockMessageId">Stock message Id</param>
		/// <returns>A product stock message</returns>
		ProductStockMessageModel Get(string shopId, string stockMessageId);

		/// <summary>
		/// 言語コード重複登録チェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="stockMessageId">商品在庫文言ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>true:重複登録なし、false:重複登録あり</returns>
		bool CheckLanguageCodeDupulication(
			string shopId,
			string stockMessageId,
			string languageCode,
			string languageLocaleId);
	}
}
