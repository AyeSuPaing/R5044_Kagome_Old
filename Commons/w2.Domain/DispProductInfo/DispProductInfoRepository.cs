/*
=========================================================================================================
  Module      : 商品表示情報リポジトリ (DispProductInfoRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Sql;
using w2.Domain.ProductRanking;

namespace w2.Domain.DispProductInfo
{
	/// <summary>
	/// 商品表示情報リポジトリ
	/// </summary>
	internal class DispProductInfoRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "DispProductInfo";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal DispProductInfoRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal DispProductInfoRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetTotalResultOrder 注文データ集計結果取得
		/// <summary>
		/// 注文データ集計結果取得
		/// </summary>
		/// <param name="productRankingModel">商品ランキング設定</param>
		/// <returns>注文データ集計結果</returns>
		internal DispProductInfoModel[] GetTotalResultOrder(ProductRankingModel productRankingModel)
		{
			var replace = new KeyValuePair<string, string>(
				"@@ where_target_data @@",
				(productRankingModel.CategoryIdList.Length != 0)
				? string.Join(
					" AND ",
					productRankingModel.CategoryIdList.Select(
						id => string.Format(
							"({0}.{1}1 NOT LIKE '{2}%')"
							+ " AND "
							+ "({0}.{1}2 NOT LIKE '{2}%')"
							+ " AND "
							+ "({0}.{1}3 NOT LIKE '{2}%')"
							+ " AND "
							+ "({0}.{1}4 NOT LIKE '{2}%')"
							+ " AND "
							+ "({0}.{1}5 NOT LIKE '{2}%')",
							Constants.TABLE_PRODUCT,
							Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID,
							id.Replace("'", "''"))))
				: "'1' = '1'");

			var dv = Get(XML_KEY_NAME, "CreateProductRanking", productRankingModel.DataSource, replaces: replace);
			return dv.Cast<DataRowView>().Select(drv => new DispProductInfoModel(drv)).ToArray();
		}
		#endregion

		#region ~UpdateStatisticsPrice 統計情報更新 商品金額
		/// <summary>
		/// 統計情報更新 商品金額
		/// </summary>
		internal void UpdateStatisticsProductPrice()
		{
			var dv = Exec(XML_KEY_NAME, "UpdateStatisticsProductPrice");
		}
		#endregion

		#region ~UpdateStatisticsProductCategory 統計情報更新 商品カテゴリ
		/// <summary>
		/// 統計情報更新 商品カテゴリ
		/// </summary>
		internal void UpdateStatisticsProductCategory()
		{
			var dv = Exec(XML_KEY_NAME, "UpdateStatisticsProductCategory");
		}
		#endregion

		#region ~UpdateStatisticsProductStock 統計情報更新 商品在庫
		/// <summary>
		/// 統計情報更新 商品在庫
		/// </summary>
		internal void UpdateStatisticsProductStock()
		{
			var dv = Exec(XML_KEY_NAME, "UpdateStatisticsProductStock");
		}
		#endregion

		#region ~UpdateStatisticsProductProduct 統計情報更新 商品
		/// <summary>
		/// 統計情報更新 商品
		/// </summary>
		internal void UpdateStatisticsProduct()
		{
			var dv = Exec(XML_KEY_NAME, "UpdateStatisticsProduct");
		}
		#endregion

		#region ~UpdateStatisticsDispProductInfo 統計情報更新 商品詳細
		/// <summary>
		/// 統計情報更新 商品詳細
		/// </summary>
		internal void UpdateStatisticsDispProductInfo()
		{
			var dv = Exec(XML_KEY_NAME, "UpdateStatisticsDispProductInfo");
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(DispProductInfoModel model)
		{
			Exec(XML_KEY_NAME, "InsertDispProductInfo", model.DataSource);
		}
		#endregion

		#region ~Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="dataKbn">データ区分</param>
		internal void Delete(string shopId, string dataKbn)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_DISPPRODUCTINFO_SHOP_ID, shopId},
				{Constants.FIELD_DISPPRODUCTINFO_DATA_KBN, dataKbn},
			};
			Exec(XML_KEY_NAME, "DeleteDispProductInfo", ht);
		}
		#endregion
	}
}
