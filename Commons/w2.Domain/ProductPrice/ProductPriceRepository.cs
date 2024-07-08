/*
=========================================================================================================
  Module      : 商品価格リポジトリ (ProductPriceRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.ProductPrice
{
	/// <summary>
	/// 商品価格リポジトリ
	/// </summary>
	internal class ProductPriceRepository : RepositoryBase
	{
		/// <summary>SQLファイル</summary>
		private const string XML_KEY_NAME = "ProductPrice";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ProductPriceRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		public ProductPriceRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetAll
		/// <summary>
		/// Get all
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <returns>A array of member rank price</returns>
		internal ProductPriceModel[] GetAll(string shopId, string productId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTPRICE_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTPRICE_PRODUCT_ID, productId },
			};

			var result = Get(XML_KEY_NAME, "GetAll", input);
			return result.Cast<DataRowView>()
				.Select(row => new ProductPriceModel(row))
				.ToArray();
		}
		#endregion

		#region +Get
		/// <summary>
		/// Get
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <param name="variationId">Variation Id</param>
		/// <param name="memberRankId">Member rank id</param>
		/// <returns>Model</returns>
		internal ProductPriceModel Get(
			string shopId,
			string productId,
			string variationId,
			string memberRankId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTPRICE_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTPRICE_PRODUCT_ID, productId },
				{ Constants.FIELD_PRODUCTPRICE_VARIATION_ID, variationId },
				{ Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_ID, memberRankId },
			};

			var dv = Get(XML_KEY_NAME, "Get", input);
			return (dv.Count != 0) ? new ProductPriceModel(dv[0]) : null;
		}
		#endregion

		#region +IsExist
		/// <summary>
		/// Is exist
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="variationId">Variation ID</param>
		/// <param name="memberRankId">Member rank ID</param>
		/// <returns>True if product price is existed, otherwise false</returns>
		internal bool IsExist(
			string shopId,
			string productId,
			string variationId,
			string memberRankId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTPRICE_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTPRICE_PRODUCT_ID, productId },
				{ Constants.FIELD_PRODUCTPRICE_VARIATION_ID, variationId },
				{ Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_ID, memberRankId },
			};

			var result = Get(XML_KEY_NAME, "IsExist", input);
			return ((int)result[0][0] > 0);
		}
		#endregion

		#region +Insert
		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Model</param>
		/// <returns>Number of rows affected</returns>
		internal int Insert(ProductPriceModel model)
		{
			var result = Exec(XML_KEY_NAME, "Insert", model.DataSource);
			return result;
		}
		#endregion

		#region +Update
		/// <summary>
		/// Update
		/// </summary>
		/// <param name="model">Model</param>
		/// <returns>Number of rows affected</returns>
		internal int Update(ProductPriceModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +Delete
		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="variationId">Variation ID</param>
		/// <param name="memberRankId">Member rank ID</param>
		/// <returns>Number of rows affected</returns>
		internal int Delete(
			string shopId,
			string productId,
			string variationId,
			string memberRankId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTPRICE_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTPRICE_PRODUCT_ID, productId },
				{ Constants.FIELD_PRODUCTPRICE_VARIATION_ID, variationId },
				{ Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_ID, memberRankId },
			};

			var result = Exec(XML_KEY_NAME, "Delete", input);
			return result;
		}
		#endregion

		#region +DeleteAll
		/// <summary>
		/// Delete all
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <returns>Number of rows affected</returns>
		internal int DeleteAll(
			string shopId,
			string productId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTPRICE_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTPRICE_PRODUCT_ID, productId },
			};

			var result = Exec(XML_KEY_NAME, "DeleteAll", input);
			return result;
		}
		#endregion
	}
}