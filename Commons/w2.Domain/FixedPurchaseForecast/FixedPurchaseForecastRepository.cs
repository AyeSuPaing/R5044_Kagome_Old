/*
=========================================================================================================
  Module      : 定期出荷予測リポジトリ (FixedPurchaseForecastRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.FixedPurchaseForecast.Helper;

namespace w2.Domain.FixedPurchaseForecast
{
	/// <summary>
	/// 定期出荷予測リポジトリ
	/// </summary>
	internal class FixedPurchaseForecastRepository : RepositoryBase
	{
		/// <returns>XMLページ名</returns>
		private const string XML_KEY_NAME = "FixedPurchaseForecast";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal FixedPurchaseForecastRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal FixedPurchaseForecastRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 件数取得
		/// </summary>
		/// <returns>件数</returns>
		internal int GetCount()
		{
			var dv = Get(XML_KEY_NAME, "GetCount");
			return (int)dv[0][0];
		}
		#endregion

		#region ~SearchTargetProduct 検索(商品単位)
		/// <summary>
		/// 検索(商品単位)
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="frequencyField">頻度</param>
		/// <returns>検索結果列</returns>
		internal FixedPurchaseForecastProductListSearchResult[] SearchTargetProduct(FixedPurchaseForecastListSearchCondition condition, string frequencyField)
		{
			var replace = new KeyValuePair<string, string>("@@ frequency_field @@", frequencyField);
			var dv = Get(XML_KEY_NAME, "SearchTargetProduct", condition.CreateHashtableParams(), replaces: replace);
			return dv.Cast<DataRowView>().Select(drv => new FixedPurchaseForecastProductListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~SearchTargetProductVariation 検索(商品バリエーション単位)
		/// <summary>
		/// 検索(商品バリエーション単位)
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="frequencyField">頻度</param>
		/// <returns>検索結果列</returns>
		internal FixedPurchaseForecastProductListSearchResult[] SearchTargetProductVariation(FixedPurchaseForecastListSearchCondition condition, string frequencyField)
		{
			var replace = new KeyValuePair<string, string>("@@ frequency_field @@", frequencyField);
			var dv = Get(XML_KEY_NAME, "SearchTargetProductVariation", condition.CreateHashtableParams(), replaces: replace);
			return dv.Cast<DataRowView>().Select(drv => new FixedPurchaseForecastProductListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~SearchTargetProductForCsv CSV出力用検索(商品単位)
		/// <summary>
		/// 検索(商品単位)
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="frequencyField">頻度</param>
		/// <returns>検索結果列</returns>
		internal DataView SearchTargetProductForCsvAtDataView(Hashtable condition, string frequencyField)
		{
			var replace = new KeyValuePair<string, string>("@@ frequency_field @@", frequencyField);
			var dv = Get(XML_KEY_NAME, "SearchTargetProduct", condition, replaces: replace);
			return dv;
		}
		#endregion

		#region ~SearchTargetProductVariation CSV出力用検索(商品バリエーション単位)
		/// <summary>
		/// 検索(商品バリエーション単位)
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="frequencyField">頻度</param>
		/// <returns>検索結果列</returns>
		internal DataView SearchTargetProductVariationForCsvAtDataView(Hashtable condition, string frequencyField)
		{
			var replace = new KeyValuePair<string, string>("@@ frequency_field @@", frequencyField);
			var dv = Get(XML_KEY_NAME, "SearchTargetProductVariation", condition, replaces: replace);
			return dv;
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <returns>モデル</returns>
		internal FixedPurchaseForecastModel Get(string shopId, string fixedPurchaseId, string productId, string variationId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASEFORECAST_SHOP_ID, shopId},
				{Constants.FIELD_FIXEDPURCHASEFORECAST_FIXED_PURCHASE_ID, fixedPurchaseId},
				{Constants.FIELD_FIXEDPURCHASEFORECAST_PRODUCT_ID, productId},
				{Constants.FIELD_FIXEDPURCHASEFORECAST_VARIATION_ID, variationId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new FixedPurchaseForecastModel(dv[0]);
		}
		#endregion

		#region ~GetMonthly 月単位取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="frequencyField">頻度</param>
		/// <returns>モデル</returns>
		internal FixedPurchaseForecastItemSearchResult[] GetMonthly(string frequencyField)
		{
			var replace = new KeyValuePair<string, string>("@@ frequency_field @@", frequencyField);
			var dv = Get(XML_KEY_NAME, "GetMonthly", replaces: replace);
			return dv.Cast<DataRowView>().Select(drv => new FixedPurchaseForecastItemSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~GetMonthlyForDataView 月単位取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="frequencyField">頻度</param>
		/// <returns>モデル</returns>
		internal DataView GetMonthlyForCsvAtDataView(string frequencyField)
		{
			var replace = new KeyValuePair<string, string>("@@ frequency_field @@", frequencyField);
			var dv = Get(XML_KEY_NAME, "GetMonthly", replaces: replace);
			return dv;
		}
		#endregion

		#region ~GetTargetProductId 商品ID単位で取得
		/// <summary>
		/// 商品ID単位で取得
		/// </summary>
		/// <param name="target_month">対象月</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="frequencyField">頻度</param>
		/// <returns>モデル</returns>
		internal FixedPurchaseForecastItemSearchResult GetTargetProductId(DateTime target_month, string shopId, string productId, string frequencyField)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASEFORECAST_TARGET_MONTH, target_month },
				{ Constants.FIELD_FIXEDPURCHASEFORECAST_SHOP_ID, shopId },
				{ Constants.FIELD_FIXEDPURCHASEFORECAST_PRODUCT_ID, productId },
			};
			var replace = new KeyValuePair<string, string>("@@ frequency_field @@", frequencyField);
			var dv = Get(XML_KEY_NAME, "GetTargetProductId", ht, replaces: replace);
			if (dv.Count == 0) return null;
			return new FixedPurchaseForecastItemSearchResult(dv[0]);
		}
		#endregion

		#region ~GetTargetProductVariation 商品バリエーションID単位で取得
		/// <summary>
		/// 商品バリエーションID単位で取得
		/// </summary>
		/// <param name="target_month">対象月</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="variationId">商品バリエーションId</param>
		/// <param name="frequencyField">頻度</param>
		/// <returns>モデル</returns>
		internal FixedPurchaseForecastItemSearchResult GetTargetProductVariation(DateTime target_month, string shopId, string variationId, string frequencyField)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASEFORECAST_TARGET_MONTH, target_month },
				{ Constants.FIELD_FIXEDPURCHASEFORECAST_SHOP_ID, shopId },
				{ Constants.FIELD_FIXEDPURCHASEFORECAST_VARIATION_ID, variationId },
			};
			var replace = new KeyValuePair<string, string>("@@ frequency_field @@", frequencyField);
			var dv = Get(XML_KEY_NAME, "GetTargetProductVariationId", ht, replaces: replace);
			if (dv.Count == 0) return null;
			return new FixedPurchaseForecastItemSearchResult(dv[0]);
		}
		#endregion

		#region ~GetTheMonthClosestToThisMonth 最も今日に近い日付取得
		/// <summary>
		/// 最も今日に近い日付取得
		/// </summary>
		/// <returns>日付</returns>
		internal DateTime GetTheMonthClosestToThisMonth()
		{
			var dv = Get(XML_KEY_NAME, "GetTheMonthClosestToThisMonth");
			return (DateTime)dv[0][0];
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(FixedPurchaseForecastModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(FixedPurchaseForecastModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region ~Delete 削除
		/// <summary>
		/// 対象定期にヒットするデータを削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="shopId">店舗ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		internal int DeleteTargeFixedPurechaseId(string shopId, string fixedPurchaseId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASEFORECAST_FIXED_PURCHASE_ID, fixedPurchaseId},
				{Constants.FIELD_FIXEDPURCHASEFORECAST_SHOP_ID, shopId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteTargeFixedPurechaseId", ht);
			return result;
		}
		#endregion

		#region ~DeleteAll 全件削除
		/// <summary>
		/// 全件削除
		/// </summary>
		internal void DeleteAll()
		{
			Exec(XML_KEY_NAME, "DeleteAll");
		}
		#endregion
	}
}
