/*
=========================================================================================================
  Module      : 商品在庫文言リポジトリ (ProductStockMessageRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.ProductStockMessage
{
	/// <summary>
	/// 商品在庫文言リポジトリ
	/// </summary>
	internal class ProductStockMessageRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "ProductStockMessage";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ProductStockMessageRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ProductStockMessageRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~InsertGlobalSetting グローバル設定登録
		/// <summary>
		/// グローバル設定登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertGlobalSetting(ProductStockMessageGlobalModel model)
		{
			Exec(XML_KEY_NAME, "InsertGlobalSetting", model.DataSource);
		}
		#endregion

		#region ~UpdateGlobalSetting グローバル設定更新
		/// <summary>
		/// グローバル設定更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateGlobalSetting(ProductStockMessageGlobalModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateGlobalSetting", model.DataSource);
			return result;
		}
		#endregion

		#region ~DeleteGlobalSetting グローバル設定削除
		/// <summary>
		/// グローバル設定削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="shopId">店舗ID</param>
		/// <param name="stockMessageId">商品在庫文言ID</param>
		internal int DeleteGlobalSetting(string shopId, string stockMessageId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTSTOCKMESSAGE_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID, stockMessageId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteGlobalSetting", ht);
			return result;
		}
		#endregion

		#region ~GetProductStockMessageTranslationData 商品在庫文言翻訳情報取得
		/// <summary>
		/// 商品在庫文言翻訳情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="stockMessageId">商品在庫文言ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>モデル</returns>
		internal ProductStockMessageModel GetProductStockMessageTranslationSetting(string shopId, string stockMessageId, string languageCode, string languageLocaleId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE_ID, stockMessageId},
				{Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LANGUAGE_CODE, languageCode},
				{Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LANGUAGE_LOCALE_ID, languageLocaleId},
			};
			var dv = Get(XML_KEY_NAME, "GetProductStockMessageTranslationSetting", ht);
			if (dv.Count == 0) return null;
			return new ProductStockMessageModel(dv[0]);
		}
		#endregion

		#region ~GetProductStockMessageTranslationSettingDataView 商品在庫文言翻訳情報取得(DataView)
		/// <summary>
		/// 商品在庫文言翻訳情報取得(DataView)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="stockMessageId">商品在庫文言ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>データビュー</returns>
		internal DataView GetProductStockMessageTranslationSettingDataView(string shopId, string stockMessageId, string languageCode, string languageLocaleId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE_ID, stockMessageId},
				{Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LANGUAGE_CODE, languageCode},
				{Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LANGUAGE_LOCALE_ID, languageLocaleId},
			};
			var dv = Get(XML_KEY_NAME, "GetProductStockMessageTranslationSetting", ht);
			return dv;
		}
		#endregion

		#region ~GetProductStockMessageDataView 言語コードで商品在庫文言情報取得(DataView)
		/// <summary>
		/// 言語コードで商品在庫文言情報取得(DataView)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="stockMessageId">商品在庫文言ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId"></param>
		/// <returns></returns>
		internal DataView GetProductStockMessageDataView(string shopId, string stockMessageId, string languageCode, string languageLocaleId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE_ID, stockMessageId},
				{Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LANGUAGE_CODE, languageCode},
				{Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LANGUAGE_LOCALE_ID, languageLocaleId},
			};
			var dv = Get(XML_KEY_NAME, "GetProductStockMessageByLanguageCode", ht);
			return dv;
		}
		#endregion

		#region ~GetProductStockMessageLanguageCodeSettings 在庫文言設定情報に設定されている言語コード情報を取得
		/// <summary>
		/// 在庫文言設定情報に設定されている言語コード情報を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="stockMessageId">商品在庫文言ID</param>
		/// <returns>モデル</returns>
		internal ProductStockMessageModel[] GetProductStockMessageLanguageCodeSettings(string shopId, string stockMessageId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE_ID, stockMessageId},
			};
			var dv = Get(XML_KEY_NAME, "GetProductStockMessageLanguageCodeSettings", ht);
			if (dv.Count == 0) return new ProductStockMessageModel[0];
			return dv.Cast<DataRowView>().Select(drv => new ProductStockMessageModel(drv)).ToArray();
		}
		#endregion

		#region ~GetProductStockMessages
		/// <summary>
		/// Get product stock messages
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <returns>A collection of product stock messages</returns>
		internal ProductStockMessageModel[] GetProductStockMessages(string shopId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_SHOP_ID, shopId }
			};

			var result = Get(XML_KEY_NAME, "GetProductStockMessages", input);
			return result.Cast<DataRowView>()
				.Select(row => new ProductStockMessageModel(row))
				.ToArray();
		}
		#endregion

		#region ~Get (Product stock message)
		/// <summary>
		/// Get product stock message
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="stockMessageId">Stock message Id</param>
		/// <returns>A product stock message</returns>
		internal ProductStockMessageModel Get(string shopId, string stockMessageId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE_ID, stockMessageId }
			};

			var dv = Get(XML_KEY_NAME, "Get", input);
			return (dv.Count != 0) ? new ProductStockMessageModel(dv[0]) : null;
		}
		#endregion

		#region ~CheckLanguageCodeDupulication 言語コード重複登録チェック
		/// <summary>
		/// 言語コード重複登録チェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="stockMessageId">商品在庫文言ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>メールテンプレート情報</returns>
		internal ProductStockMessageModel[] CheckLanguageCodeDupulication(string shopId, string stockMessageId, string languageCode, string languageLocaleId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTSTOCKMESSAGE_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID, stockMessageId},
				{Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_CODE, languageCode},
				{Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_LOCALE_ID, languageLocaleId},
			};
			var dv = Get(XML_KEY_NAME, "GetProductStockMessageLanguageCodeSettings", ht);
			return dv.Cast<DataRowView>().Select(drv => new ProductStockMessageModel(drv)).ToArray();
		}
		#endregion
	}
}
