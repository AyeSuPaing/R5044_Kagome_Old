/*
=========================================================================================================
  Module      : 商品在庫文言サービス (ProductStockMessageService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
using System.Linq;

namespace w2.Domain.ProductStockMessage
{
	/// <summary>
	/// 商品在庫文言サービス
	/// </summary>
	public class ProductStockMessageService : ServiceBase, IProductStockMessageService
	{
		#region +InsertGlobalSetting グローバル設定登録
		/// <summary>
		/// グローバル設定登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertGlobalSetting(ProductStockMessageGlobalModel model)
		{
			using (var repository = new ProductStockMessageRepository())
			{
				repository.InsertGlobalSetting(model);
			}
		}
		#endregion

		#region +UpdateGlobalSetting グローバル設定更新
		/// <summary>
		/// グローバル設定更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int UpdateGlobalSetting(ProductStockMessageGlobalModel model)
		{
			using (var repository = new ProductStockMessageRepository())
			{
				var result = repository.UpdateGlobalSetting(model);
				return result;
			}
		}
		#endregion

		#region +DeleteGlobalSetting グローバル設定削除
		/// <summary>
		/// グローバル設定削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="stockMessageId">商品在庫文言ID</param>
		public void DeleteGlobalSetting(string shopId, string stockMessageId)
		{
			using (var repository = new ProductStockMessageRepository())
			{
				var result = repository.DeleteGlobalSetting(shopId, stockMessageId);
			}
		}
		#endregion

		#region +GetProductStockMessageTranslationData 商品在庫文言翻訳情報取得
		/// <summary>
		/// 商品在庫文言翻訳情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="stockMessageId">商品在庫文言ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>モデル</returns>
		public ProductStockMessageModel GetProductStockMessageTranslationSetting(string shopId, string stockMessageId, string languageCode, string languageLocaleId)
		{
			using (var repository = new ProductStockMessageRepository())
			{
				var model = repository.GetProductStockMessageTranslationSetting(shopId, stockMessageId, languageCode, languageLocaleId);
				return model;
			}
		}
		#endregion

		#region +GetProductStockMessageTranslationSettingDataView 商品在庫文言翻訳情報取得(DataView)
		/// <summary>
		/// 商品在庫文言翻訳情報取得(DataView)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="stockMessageId">商品在庫文言ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>データビュー</returns>
		public DataView GetProductStockMessageTranslationSettingDataView(string shopId, string stockMessageId, string languageCode, string languageLocaleId)
		{
			using (var repository = new ProductStockMessageRepository())
			{
				var dv = repository.GetProductStockMessageTranslationSettingDataView(shopId, stockMessageId, languageCode, languageLocaleId);
				return dv;
			}
		}
		#endregion

		#region +GetProductStockMessageDataView 言語コードで商品在庫文言情報取得(DataView)
		/// <summary>
		/// 言語コードで商品在庫文言情報取得(DataView)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="stockMessageId">商品在庫文言ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>モデル</returns>
		public DataView GetProductStockMessageDataView(string shopId, string stockMessageId, string languageCode, string languageLocaleId)
		{
			using (var repository = new ProductStockMessageRepository())
			{
				var dv = repository.GetProductStockMessageDataView(shopId, stockMessageId, languageCode, languageLocaleId);
				return dv;
			}
		}
		#endregion

		#region +GetProductStockMessageLanguageCodeSettings 在庫文言設定情報に設定されている言語コード設定情報を取得
		/// <summary>
		/// 在庫文言設定情報に設定されている言語コード設定情報を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="stockMessageId">商品在庫文言ID</param>
		/// <returns>言語コード設定情報</returns>
		public ProductStockMessageModel[] GetProductStockMessageLanguageCodeSettings(string shopId, string stockMessageId)
		{
			using (var repository = new ProductStockMessageRepository())
			{
				var settings = repository.GetProductStockMessageLanguageCodeSettings(shopId, stockMessageId);
				return settings;
			}
		}
		#endregion

		#region +GetProductStockMessages
		/// <summary>
		/// Get product stock messages
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <returns>A collection of product stock messages</returns>
		public ProductStockMessageModel[] GetProductStockMessages(string shopId)
		{
			using (var repository = new ProductStockMessageRepository())
			{
				var results = repository.GetProductStockMessages(shopId);
				return results;
			}
		}
		#endregion

		#region +Get (Product stock message)
		/// <summary>
		/// Get product stock message
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="stockMessageId">Stock message Id</param>
		/// <returns>A product stock message</returns>
		public ProductStockMessageModel Get(string shopId, string stockMessageId)
		{
			using (var repository = new ProductStockMessageRepository())
			{
				var model = repository.Get(shopId, stockMessageId);
				return model;
			}
		}
		#endregion

		#region +CheckLanguageCodeDupulication 言語コード重複登録チェック
		/// <summary>
		/// 言語コード重複登録チェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="stockMessageId">商品在庫文言ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>true:重複登録なし、false:重複登録あり</returns>
		public bool CheckLanguageCodeDupulication(string shopId, string stockMessageId, string languageCode, string languageLocaleId)
		{
			using (var repository = new ProductStockMessageRepository())
			{
				var models = repository.CheckLanguageCodeDupulication(shopId, stockMessageId, languageCode, languageLocaleId);
				return (models.Any() == false);
			}
		}
		#endregion
	}
}
