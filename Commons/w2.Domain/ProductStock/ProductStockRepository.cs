/*
=========================================================================================================
  Module      : 商品在庫リポジトリ (ProductStockRepository.cs)
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

namespace w2.Domain.ProductStock
{
	/// <summary>
	/// 商品在庫リポジトリ
	/// </summary>
	internal class ProductStockRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "ProductStock";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ProductStockRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ProductStockRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion
		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <returns>モデル</returns>
		internal ProductStockModel Get(string shopId, string productId, string variationId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTSTOCK_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, productId},
				{Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, variationId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new ProductStockModel(dv[0]);
		}
		#endregion

		#region ~GetSum バリエーション含む在庫数取得
		/// <summary>
		/// バリエーション含む在庫数取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <returns>在庫管理する:在庫数 在庫管理しない:半角ハイフン</returns>
		internal string GetSum(string shopId, string productId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCTSTOCK_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, productId }
			};
			var dv = Get(XML_KEY_NAME, "GetSum", ht);

			if (dv[0][0] == DBNull.Value) return "-";
			return ((int)dv[0][0]).ToString();
		}
		#endregion

		#region +IsExist
		/// <summary>
		/// Is exist
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="variationId">Variation ID</param>
		/// <returns>True if product stock is existed, otherwise false</returns>
		internal bool IsExist(
			string shopId,
			string productId,
			string variationId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTVARIATION_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID, productId },
				{ Constants.FIELD_PRODUCTVARIATION_VARIATION_ID, variationId },
			};

			var result = Get(XML_KEY_NAME, "IsExist", input);
			return ((int)result[0][0] > 0);
		}
		#endregion

		#region ~Insert 挿入
		/// <summary>
		/// 挿入
		/// </summary>
		/// <param name="productStock">商品在庫モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Insert(ProductStockModel productStock)
		{
			var result = Exec(XML_KEY_NAME, "Insert", productStock.DataSource);
			return result;
		}
		#endregion

		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(ProductStockModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region ~UpdateProductStockAndGetStock 在庫数更新(更新後在庫数取得)
		/// <summary>
		/// 在庫数更新(更新後在庫数取得)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="orderQuantity">注文数量</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>更新後在庫数</returns>
		internal int UpdateProductStockAndGetStock(string shopId, string productId, string variationId, int orderQuantity, string lastChanged)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCTSTOCK_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, productId },
				{ Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, variationId },
				{ "order_quantity", orderQuantity},
				{ Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED, lastChanged },
			};
			var result = Exec(XML_KEY_NAME, "UpdateProductStock", ht);
			return result;
		}
		#endregion

		#region ~AddProductStock 商品在庫情報の論理在庫・実在庫・引当済実在庫数更新
		/// <summary>
		/// 商品在庫情報の論理在庫・実在庫・引当済実在庫数更新
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="itemQuantity">注文数</param>
		/// <param name="realStock">実A在庫数</param>
		/// <param name="realStockB">実B在庫数</param>
		/// <param name="realStockC">実C在庫数</param>
		/// <param name="realStockReserved">引当済実在庫数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>影響を受けた件数</returns>
		internal int AddProductStock(
			string shopId,
			string productId,
			string variationId,
			int itemQuantity,
			int realStock,
			int realStockB,
			int realStockC,
			int realStockReserved,
			string lastChanged)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCTSTOCK_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, productId },
				{ Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, variationId },
				{ Constants.FIELD_PRODUCTSTOCK_STOCK, itemQuantity },
				{ Constants.FIELD_PRODUCTSTOCK_REALSTOCK, realStock },
				{ Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B, realStockB },
				{ Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C, realStockC },
				{ Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED, realStockReserved },
				{ Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED, lastChanged }
			};
			var result = Exec(XML_KEY_NAME, "AddProductStock", ht);
			return result;
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// マスタをReaderで取得（CSV出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		/// <returns>Reader</returns>
		public SqlStatementDataReader GetMasterWithReader(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			var reader = GetWithReader(XML_KEY_NAME, "GetProductStockMaster", input, replaces);
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
			var dv = Get(XML_KEY_NAME, "GetProductStockMaster", input, replaces: replaces);
			return dv;
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		public void CheckProductStockFieldsForGetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			Get(XML_KEY_NAME, "CheckProductStockFields", input, replaces: replaces);
		}
		#endregion

		#region +DeleteProductStock
		/// <summary>
		/// Delete product stock all
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <param name="ignoredVariationIds">Ignored variation ids</param>
		/// <returns>Number of rows affected</returns>
		internal int DeleteProductStock(
			string shopId,
			string productId,
			string[] ignoredVariationIds)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTSTOCK_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, productId },
			};

			var replacer = new KeyValuePair<string, string>[]
			{
				new KeyValuePair<string, string>(
					"@@ ignored_variation_ids @@",
					string.Format("'{0}'", string.Join("','", ignoredVariationIds)))
			};

			var result = Exec(XML_KEY_NAME, "DeleteProductStock", input, replaces: replacer);
			return result;
		}
		#endregion

		#region +DeleteProductStockAll
		/// <summary>
		/// Delete product stock all
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <returns>Number of rows affected</returns>
		internal int DeleteProductStockAll(string shopId, string productId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTSTOCK_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, productId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteProductStockAll", input);
			return result;
		}
		#endregion
	}
}
