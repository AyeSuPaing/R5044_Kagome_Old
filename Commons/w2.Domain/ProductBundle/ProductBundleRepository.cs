/*
=========================================================================================================
  Module      : 商品同梱テーブルリポジトリ (ProductBundleRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.ProductBundle.Helper;

namespace w2.Domain.ProductBundle
{
	/// <summary>
	/// 商品同梱テーブルリポジトリ
	/// </summary>
	public class ProductBundleRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "ProductBundle";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductBundleRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductBundleRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(ProductBundleListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", condition.CreateHashtableParams(), replaces: GetExcludeTargetOrderType());
			return (int)dv[0][0];
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		public ProductBundleListSearchResult[] Search(ProductBundleListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "Search", condition.CreateHashtableParams(), replaces: GetExcludeTargetOrderType());
			return dv.Cast<DataRowView>().Select(drv => new ProductBundleListSearchResult(drv)).ToArray();
		}
		#endregion

		/// <summary>
		/// 除外する対象注文種別取得
		/// </summary>
		/// <returns>除外する対象注文種別</returns>
		private KeyValuePair<string, string> GetExcludeTargetOrderType()
		{
			var excludeTargetOrderType = (Constants.FIXEDPURCHASE_OPTION_ENABLED
				? string.Empty
				: Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_FIXED_PURCHASE);
			var replaceKeyValues = new KeyValuePair<string, string>(
				"@@ excludeTargetOrderType @@",
				("'" + excludeTargetOrderType + "'"));
			return replaceKeyValues;
		}

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="productBundleId">商品同梱ID</param>
		/// <returns>モデル</returns>
		public ProductBundleModel Get(string productBundleId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID, productBundleId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new ProductBundleModel(dv[0]);
		}
		#endregion

		#region +GetProductBundleValidForCart 条件に合致する同梱設定を取得
		/// <summary>
		/// 条件に合致する同梱設定を取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="excludeOrderIds">利用回数にカウントしない注文ID</param>
		/// <param name="advCodeFirst">初回広告コード</param>
		/// <param name="advCodeNew">最新広告コード</param>
		/// <param name="orderPriceSubtotal">商品代金合計</param>
		/// <param name="targetOrderType">注文種別</param>
		/// <param name="targetCouponCode">クーポンコード</param>
		/// <returns>モデル</returns>
		public ProductBundleModel[] GetProductBundleValidForCart(
			string userId,
			IEnumerable<string> excludeOrderIds,
			string advCodeFirst,
			string advCodeNew,
			decimal orderPriceSubtotal,
			string targetOrderType,
			string targetCouponCode)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_ADVCODE_FIRST, advCodeFirst },
				{ Constants.FIELD_ORDER_ADVCODE_NEW, advCodeNew },
				{ Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL, orderPriceSubtotal },
				{ Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_TYPE, targetOrderType },
				{ Constants.FIELD_PRODUCTBUNDLE_TARGET_COUPON_CODES, targetCouponCode }
			};

			var result = Get(XML_KEY_NAME, "GetProductBundleForCart", ht)
				.Cast<DataRowView>()
				.Select(drv => new ProductBundleModel(drv))
				.ToArray();
			return result;
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(ProductBundleModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(ProductBundleModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="productBundleId">商品同梱ID</param>
		public int Delete(string productBundleId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID, productBundleId},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region +GetItemsAll アイテムすべて取得
		/// <summary>
		/// アイテムすべて取得
		/// </summary>
		/// <param name="productBundleId">商品同梱ID</param>
		/// <returns>モデル列</returns>
		public ProductBundleItemModel[] GetItemsAll(string productBundleId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCTBUNDLEITEM_PRODUCT_BUNDLE_ID, productBundleId }
			};

			var dv = Get(XML_KEY_NAME, "GetItemsAll", ht);
			return dv.Cast<DataRowView>().Select(drv => new ProductBundleItemModel(drv)).ToArray();
		}
		#endregion

		#region +GetProductBundleItems 同梱商品取得
		/// <summary>
		/// 同梱商品取得
		/// </summary>
		/// <param name="productBundleId">商品同梱ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="excludeOrderIds">利用回数にカウントしない注文ID</param>
		/// <returns>モデル列</returns>
		public ProductBundleItemModel[] GetProductBundleItems(
			string productBundleId,
			string userId,
			IEnumerable<string> excludeOrderIds)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCTBUNDLEITEM_PRODUCT_BUNDLE_ID, productBundleId },
				{ Constants.FIELD_ORDER_USER_ID, userId }
			};

			var orderIds = (excludeOrderIds != null)
				? string.Join(",", excludeOrderIds.Select(id => string.Format("'{0}'", id)))
				: "''";
			var replaceKeyValues = new[]
			{
				new KeyValuePair<string, string>("@@ excludeOrderIds @@", orderIds),
			};

			var dv = Get(XML_KEY_NAME, "GetProductBundleItems", ht, replaces: replaceKeyValues);
			return (dv.Count > 0)
				? dv.Cast<DataRowView>().Select(drv => new ProductBundleItemModel(drv)).ToArray()
				: new ProductBundleItemModel[0];
		}
		#endregion

		#region +DeleteItemsAll アイテムすべて削除
		/// <summary>
		/// アイテムすべて削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="productBundleId">商品同梱ID</param>
		public int DeleteItemsAll(string productBundleId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID, productBundleId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteItemsAll", ht);
			return result;
		}
		#endregion

		#region +InsertItem アイテム登録
		/// <summary>
		/// アイテム登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertItem(ProductBundleItemModel model)
		{
			Exec(XML_KEY_NAME, "InsertItem", model.DataSource);
		}
		#endregion
	}
}
