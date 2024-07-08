/*
=========================================================================================================
  Module      : 商品レビューリポジトリ (ProductReviewRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.ProductReview
{
	/// <summary>
	/// 商品レビューリポジトリ
	/// </summary>
	class ProductReviewRepository : RepositoryBase
	{
		/// <returns>ユーザーSQLファイル</returns>
		private const string XML_KEY_NAME = "ProductReview";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductReviewRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductReviewRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 商品レビューの取得
		/// <summary>
		/// 商品レビューの取得
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <param name="reviewNo">レビュー番号</param>
		/// <returns>商品レビュー取得</returns>
		internal ProductReviewModel Get(string shopId, string productId, string reviewNo)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCTREVIEW_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID, productId },
				{ Constants.FIELD_PRODUCTREVIEW_REVIEW_NO, reviewNo }
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			return (dv.Count == 0) ? null : new ProductReviewModel(dv[0]);
		}

		#endregion

		#region~GetNewReviewNo 生成した新しいレビュー番号の取得
		/// <summary>
		/// 生成した新しいレビュー番号の取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <returns>レビュー番号</returns>
		internal int GetNewReviewNo(string shopId, string productId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCTREVIEW_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTREVIEW_PRODUCT_ID, productId },
			};
			var dv = Get(XML_KEY_NAME, "GetNewReviewNo", ht);
			return (int)dv[0][0];
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
			var reader = GetWithReader(XML_KEY_NAME, "GetProductReviewMaster", input, replaces);
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
			var dv = Get(XML_KEY_NAME, "GetProductReviewMaster", input, replaces: replaces);
			return dv;
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		public void CheckProductReviewFieldsForGetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			Get(XML_KEY_NAME, "CheckProductReviewFields", input, replaces: replaces);
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>登録件数</returns>
		internal int Insert(ProductReviewModel model)
		{
			return Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>更新した件数</returns>
		internal int Update(ProductReviewModel model)
		{
			return Exec(XML_KEY_NAME, "Update", model.DataSource);
		}
		#endregion
	}
}