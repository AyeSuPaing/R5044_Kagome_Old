/*
=========================================================================================================
  Module      : 商品セールリポジトリ (ProductSaleRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.ProductSale
{
	/// <summary>
	/// 商品セールリポジトリ
	/// </summary>
	internal class ProductSaleRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "ProductSale";

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ProductSaleRepository(SqlAccessor accessor = null)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productsaleId">商品セールID</param>
		/// <returns>モデル</returns>
		internal ProductSaleModel Get(string shopId, string productsaleId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTSALE_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID, productsaleId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			return (dv.Count != 0) ? new ProductSaleModel(dv[0]) : null;
		}
		#endregion

		#region ~GetValidAll 取得（全件）
		/// <summary>
		/// 取得（有効なレコード）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル配列</returns>
		internal ProductSaleModel[] GetValidAll(string shopId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTSALE_SHOP_ID, shopId},
			};
			var dv = Get(XML_KEY_NAME, "GetValidAll", ht);
			return dv.Cast<DataRowView>().Select(drv => new ProductSaleModel(drv)).ToArray();
		}
		#endregion

		#region ~GetValidAllByProductSaleKbn 取得（商品セール区分が条件）
		/// <summary>
		/// 取得（商品セール区分が条件）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productSaleKbn">商品セール区分</param>
		/// <returns>モデル配列</returns>
		internal ProductSaleModel[] GetValidAllByProductSaleKbn(string shopId, string productSaleKbn)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTSALE_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTSALE_PRODUCTSALE_KBN, productSaleKbn},
			};
			var dv = Get(XML_KEY_NAME, "GetValidAllByProductSaleKbn", ht);
			return dv.Cast<DataRowView>().Select(drv => new ProductSaleModel(drv)).ToArray();
		}
		#endregion

		/// <summary>
		/// 期間内に更新された商品セール情報を取得
		/// </summary>
		/// <param name="begin">開始日時</param>
		/// <param name="end">終了日時</param>
		/// <returns>商品セール情報（店舗IDと商品セールIDのペアの配列）</returns>
		public KeyValuePair<string, string>[] GetUpdatedProductSalePriceByDateChanged(DateTime begin, DateTime end)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetUpdatedProductSalePriceByDateChanged",
				new Hashtable
				{
					{ "date_bgn", begin },
					{ "date_end", end },
				});
			return dv.Cast<DataRowView>()
				.Select(drv => new KeyValuePair<string, string>((string)drv[0], (string)drv[1])).ToArray();
		}

		#region ~GetProductSaleCount
		/// <summary>
		/// Get product sale count
		/// </summary>
		/// <param name="productSale">Product sale</param>
		/// <returns>Number</returns>
		public int GetProductSaleCount(Hashtable productSale)
		{
			var dv = Get(XML_KEY_NAME, "GetProductSaleCount", productSale);
			return (int)dv[0][0];
		}
		#endregion

		#region ~GetProductSaleList
		/// <summary>
		/// Get product sale list
		/// </summary>
		/// <param name="productSale">Product sale</param>
		/// <returns>Data product sale</returns>
		public DataView GetProductSaleList(Hashtable productSale)
		{
			var dv = Get(XML_KEY_NAME, "GetProductSaleList", productSale);
			return dv;
		}
		#endregion

		#region ~商品セール価格取得
		/// <summary>
		/// 商品セール価格取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productSaleId">商品セールID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <returns>新注文ID</returns>
		internal decimal? GetProductSalePrice(string shopId, string productSaleId, string productId, string variationId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCTSALE_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID, productSaleId },
				{ Constants.FIELD_PRODUCTSALEPRICE_PRODUCT_ID, productId },
				{ Constants.FIELD_PRODUCTSALEPRICE_VARIATION_ID, variationId }
			};
			var productSalePrice = Get(XML_KEY_NAME, "GetProductSalePrice", ht);
			return (productSalePrice.Count != 0) ? (decimal?)productSalePrice[0][Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE] : null;
		}
		#endregion

		/// <summary>
		/// 商品セール価格テンポラリデータインサート
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productSaleId">商品セールID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>更新件数</returns>
		public int InsertFromProductSalePriceTmp(string shopId, string productSaleId, string lastChanged)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCTSALEPRICETMP_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTSALEPRICETMP_PRODUCTSALE_ID, productSaleId },
				{ Constants.FIELD_PRODUCTSALEPRICETMP_LAST_CHANGED, lastChanged },
			};
			var updated = Exec(XML_KEY_NAME, "InsertFromProductSalePriceTmp", ht);
			return updated;
		}

		/// <summary>
		/// 商品セール価格テンポラリデータ削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productSaleId">商品セールID</param>
		/// <returns>更新件数</returns>
		public int DeleteProductSalePriceTmpBySaleId(string shopId, string productSaleId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCTSALEPRICETMP_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTSALEPRICETMP_PRODUCTSALE_ID, productSaleId },
			};
			var updated = Exec(XML_KEY_NAME, "DeleteProductSalePriceTmpBySaleId", ht);
			return updated;
		}

		#region +マスタ出力
		/// <summary>
		/// マスタをReaderで取得（CSV出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		/// <returns>Reader</returns>
		public SqlStatementDataReader GetMasterWithReader(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			var reader = GetWithReader(XML_KEY_NAME, "GetProductSalePriceMaster", input, replaces);
			return reader;
		}

		/// <summary>
		/// マスタ取得（Excel出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		/// <returns>DataView</returns>
		public DataView GetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			var dv = Get(XML_KEY_NAME, "GetProductSalePriceMaster", input, replaces: replaces);
			return dv;
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		public void CheckProductSalePriceFieldsForGetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			Get(XML_KEY_NAME, "CheckProductSalePriceFields", input, replaces: replaces);
		}
		#endregion
	}
}
