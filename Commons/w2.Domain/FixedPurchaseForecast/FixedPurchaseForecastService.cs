/*
=========================================================================================================
  Module      : 定期出荷予測サービス (FixedPurchaseForecastService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.FixedPurchaseForecast.Helper;
using w2.Domain.Product;
using w2.Domain.ShopShipping;
using w2.Domain.User;

namespace w2.Domain.FixedPurchaseForecast
{
	/// <summary>
	/// 定期出荷予測サービス
	/// </summary>
	public class FixedPurchaseForecastService : ServiceBase
	{
		/// <summary>定期売上予測で何か月集計するか</summary>
		private const int FIXED_PURCHASE_FORECAST_MONTH = 6;

		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 件数取得
		/// </summary>
		/// <param name="accessor">アクセサ</param>
		/// <returns>件数</returns>
		public int GetCount(SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseForecastRepository(accessor))
			{
				var count = repository.GetCount();
				return count;
			}
		}
		#endregion

		#region +SearchTargetProduct 検索(商品単位)
		/// <summary>
		/// 検索(商品単位)
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="frequencyField">頻度</param>
		/// <returns>モデル</returns>
		public FixedPurchaseForecastProductListSearchResult[] SearchTargetProduct(FixedPurchaseForecastListSearchCondition condition, string frequencyField)
		{
			using (var repository = new FixedPurchaseForecastRepository())
			{
				condition.TargetMonth = GetTheMonthClosestToThisMonth();
				var searchResults = repository.SearchTargetProduct(condition, frequencyField);

				// ２か月以降を取得
				foreach (var model in searchResults)
				{
					var targetMonth = model.Item[0].TargetMonth;
					foreach (var i in Enumerable.Range(0, (FIXED_PURCHASE_FORECAST_MONTH - 1)))
					{
						targetMonth = targetMonth.AddMonths(1);
						model.Item.Add(GetTargetProductId(targetMonth, model.ShopId, model.ProductId, frequencyField));
					}
				}
				return searchResults;
			}
		}
		#endregion

		#region +SearchTargetProductVariation 検索(商品バリエーション単位)
		/// <summary>
		/// 検索(バリエーション)
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="frequencyField">頻度</param>
		/// <returns>モデル</returns>
		public FixedPurchaseForecastProductListSearchResult[] SearchTargetProductVariation(FixedPurchaseForecastListSearchCondition condition, string frequencyField)
		{
			using (var repository = new FixedPurchaseForecastRepository())
			{
				condition.TargetMonth = GetTheMonthClosestToThisMonth();
				var searchResults = repository.SearchTargetProductVariation(condition, frequencyField);

				// ２か月以降を取得
				foreach (var model in searchResults)
				{
					var targetMonth = model.Item[0].TargetMonth;
					for (var i = 0; i < (FIXED_PURCHASE_FORECAST_MONTH - 1); i++)
					{
						targetMonth = targetMonth.AddMonths(1);
						model.Item.Add(GetTargetProductVariation(targetMonth, model.ShopId, model.VariationId, frequencyField));
					}
				}
				return searchResults;
			}
		}
		#endregion

		#region +SearchTargetProductForCsv CSV出力用検索(商品単位)
		/// <summary>
		/// CSV出力用検索(商品単位)
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="frequencyField">頻度</param>
		/// <returns>モデル</returns>
		public DataView SearchTargetProductForCsvAtDataView(FixedPurchaseForecastListSearchCondition condition, string frequencyField)
		{
			using (var repository = new FixedPurchaseForecastRepository())
			{
				var serachParameter = condition.CreateHashtableParams();
				// conditionをそのまま渡すとhasvalが適用されないため、ここでパラメータ削除
				serachParameter.Remove("target_month");
				var searchResults = repository.SearchTargetProductForCsvAtDataView(serachParameter, frequencyField);

				return searchResults;
			}
		}
		#endregion

		#region +SearchTargetProductVariationForCsv CSV出力用検索(商品単位)
		/// <summary>
		/// CSV出力用検索(バリエーション単位)
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="frequencyField">頻度</param>
		/// <returns>モデル</returns>
		public DataView SearchTargetProductVariationForCsvAtDataView(FixedPurchaseForecastListSearchCondition condition, string frequencyField)
		{
			using (var repository = new FixedPurchaseForecastRepository())
			{
				var serachParameter = condition.CreateHashtableParams();
				// conditionをそのまま渡すとhasvalが適用されないため、ここでパラメータ削除
				serachParameter.Remove("target_month");
				var searchResults = repository.SearchTargetProductVariationForCsvAtDataView(serachParameter, frequencyField);

				return searchResults;
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public FixedPurchaseForecastModel Get(string shopId, string fixedPurchaseId, string productId, string variationId, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseForecastRepository(accessor))
			{
				var model = repository.Get(shopId, fixedPurchaseId, productId, variationId);
				return model;
			}
		}
		#endregion

		#region +GetMonthly 月単位取得
		/// <summary>
		/// 月単位取得
		/// </summary>
		/// <param name="frequencyField">頻度</param>
		/// <returns>モデル</returns>
		public FixedPurchaseForecastItemSearchResult[] GetMonthly(string frequencyField)
		{
			using (var repository = new FixedPurchaseForecastRepository())
			{
				var model = repository.GetMonthly(frequencyField);
				return model;
			}
		}
		#endregion

		#region +GetMonthly 月単位取得
		/// <summary>
		/// 月単位取得
		/// </summary>
		/// <param name="frequencyField">頻度</param>
		/// <returns>モデル</returns>
		public DataView GetMonthlyForCsvAtDataView(string frequencyField)
		{
			using (var repository = new FixedPurchaseForecastRepository())
			{
				var dv = repository.GetMonthlyForCsvAtDataView(frequencyField);
				return dv;
			}
		}
		#endregion

		#region +GetTargetProductId 商品ID単位で取得
		/// <summary>
		/// 商品ID単位で取得
		/// </summary>
		/// <param name="targetMonth">対象月</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="frequencyField">頻度</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public FixedPurchaseForecastItemSearchResult GetTargetProductId(DateTime targetMonth, string shopId, string productId, string frequencyField, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseForecastRepository(accessor))
			{
				var model = repository.GetTargetProductId(targetMonth, shopId, productId, frequencyField);
				return model;
			}
		}
		#endregion

		#region +GetTargetProductVariation 商品バリエーションID単位で取得
		/// <summary>
		/// 商品バリエーションID単位で取得
		/// </summary>
		/// <param name="targetMonth">対象月</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="variationId">商品バリエーションId</param>
		/// <param name="frequencyField">頻度</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public FixedPurchaseForecastItemSearchResult GetTargetProductVariation(DateTime targetMonth, string shopId, string variationId, string frequencyField, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseForecastRepository(accessor))
			{
				var model = repository.GetTargetProductVariation(targetMonth, shopId, variationId, frequencyField);
				return model;
			}
		}
		#endregion

		#region +GetTheMonthClosestToThisMonth 最も今日に近い日付取得
		/// <summary>
		/// 最も今日に近い日付取得
		/// </summary>
		/// <returns>モデル</returns>
		public DateTime GetTheMonthClosestToThisMonth(SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseForecastRepository(accessor))
			{
				var model = repository.GetTheMonthClosestToThisMonth();
				return model;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Insert(FixedPurchaseForecastModel model, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseForecastRepository(accessor))
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int Update(FixedPurchaseForecastModel model, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseForecastRepository(accessor))
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 対象定期にヒットするデータを削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteTargeFixedPurechaseId(string shopId, string fixedPurchaseId, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseForecastRepository(accessor))
			{
				repository.DeleteTargeFixedPurechaseId(shopId, fixedPurchaseId);
			}
		}
		#endregion

		#region +DeleteAll 削除
		/// <summary>
		/// 全件削除
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteAll(SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseForecastRepository(accessor))
			{
				repository.DeleteAll();
			}
		}
		#endregion



		#region +InsertProductVariation バリエーション商品単位で保存
		/// <summary>
		/// バリエーション商品単位で保存
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="product">商品</param>
		/// <param name="productCount">商品数</param>
		/// <param name="eachMonthDeliveryFrequency">各月の配送回数</param>
		/// <param name="taxExcludedFractionRounding">税処理金額端数処理方法</param>
		/// <param name="managementIncludedTaxFlag">税込み管理フラグ</param>
		/// <param name="currencyLocaleId">通貨ロケールID</param>
		/// <param name="currencyDecimalDigits">通貨の小数点以下の有効桁数</param>
		/// <param name="eachMonthDeliveryFrequencyForShippingDate">各月の出荷回数</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void InsertProductVariation(
			string fixedPurchaseId,
			ProductModel product,
			int productCount,
			Dictionary<string, int> eachMonthDeliveryFrequency,
			string taxExcludedFractionRounding,
			bool managementIncludedTaxFlag,
			string currencyLocaleId,
			int? currencyDecimalDigits,
			Dictionary<string, int> eachMonthDeliveryFrequencyForShippingDate,
			SqlAccessor accessor)
		{
			var sortEachMonthDeliveryFrequency = eachMonthDeliveryFrequency.OrderBy(k => k.Key).ToArray();

			foreach (KeyValuePair<string, int> deliveryFrequency in sortEachMonthDeliveryFrequency)
			{
				var productSaleId = StringUtility.ToEmpty(product.DataSource[Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]);
				var productCalculation = new FixedPurchaseProductCalculator(
					product,
					productSaleId,
					productCount,
					taxExcludedFractionRounding,
					managementIncludedTaxFlag,
					currencyLocaleId,
					currencyDecimalDigits);

				Insert(
					new FixedPurchaseForecastModel
					{
						TargetMonth = DateTime.Parse(deliveryFrequency.Key),
						FixedPurchaseId = fixedPurchaseId,
						ShopId = product.ShopId,
						ProductId = product.ProductId,
						VariationId = product.VariationId,
						ProductPrice = productCalculation.PricePretax,
						ItemQuantity = productCalculation.Count,
						DeliveryFrequency = deliveryFrequency.Value,
						DeliveryFrequencyByScheduledShippingDate = eachMonthDeliveryFrequencyForShippingDate[deliveryFrequency.Key]
					},
					accessor);
			}
		}
		#endregion

		#region +CalculateToSixMonthEachMonthDeliveryFrequency 実行日から半年後までの月ごとの配送回数を計算
		/// <summary>
		/// 実行日から半年後までの月ごとの配送回数を計算
		/// </summary>
		/// <param name="fpKbn">定期購入区分</param>
		/// <param name="fpSetting">定期購入設定</param>
		/// <param name="nextShippingDate">次回配送日</param>
		/// <param name="nextNextShippingDate">次々回配送日</param>
		/// <param name="daysRequired">配送所要日数</param>
		/// <param name="calculationMode">次回配送日計算モード</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <param name="executionDate">実行日</param>
		/// <returns>月ごとの配送回数</returns>
		public Dictionary<string, int> CalculateToSixMonthEachMonthDeliveryFrequency(
			string fpKbn,
			string fpSetting,
			DateTime? nextShippingDate,
			DateTime? nextNextShippingDate,
			int daysRequired,
			int minSpan,
			NextShippingCalculationMode calculationMode,
			DateTime executionDate)
		{
			const string DATE_TIME_FORMAT = "yyyy/MM";

			var tmpDay = executionDate;
			var eachMothDeliveryFrequency = new Dictionary<string, int>();
			for (var i = 0; i < FIXED_PURCHASE_FORECAST_MONTH; i++)
			{
				tmpDay = tmpDay.AddMonths(1);
				eachMothDeliveryFrequency.Add(tmpDay.Date.ToString(DATE_TIME_FORMAT), 0);
			}

			if ((nextShippingDate == null) || (nextNextShippingDate == null)
				|| (Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE != fpKbn
				&& Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY != fpKbn
				&& Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS != fpKbn)) return eachMothDeliveryFrequency;

			var nextShippingDateTmp = (DateTime)nextShippingDate;
			var nextNextShippingDateTmp = (DateTime)nextNextShippingDate;

			// 実行日から半年後の日付取得
			var afterSixMonthDate = DateTime.Today.AddMonths(FIXED_PURCHASE_FORECAST_MONTH);

			// 次回配送日
			if (((nextShippingDateTmp.Date.ToString(DATE_TIME_FORMAT) == afterSixMonthDate.Date.ToString(DATE_TIME_FORMAT)) || (nextShippingDateTmp < afterSixMonthDate))
				&& (executionDate < nextShippingDateTmp) && (executionDate.Month != nextShippingDateTmp.Month))
			{
				var target = nextShippingDateTmp.Date.ToString(DATE_TIME_FORMAT);
				eachMothDeliveryFrequency[target] = eachMothDeliveryFrequency[target] + 1;
			}

			var fixedPurchaseCalculator = new FixedPurchaseShippingDateCalculator();
			var tmpCalculationDate = nextNextShippingDateTmp;
			while ((tmpCalculationDate.Date.ToString(DATE_TIME_FORMAT) == afterSixMonthDate.Date.ToString(DATE_TIME_FORMAT)) || (tmpCalculationDate < afterSixMonthDate))
			{
				var target = tmpCalculationDate.Date.ToString(DATE_TIME_FORMAT);
				if ((executionDate < tmpCalculationDate) && (executionDate.Month != tmpCalculationDate.Month)) eachMothDeliveryFrequency[target] = eachMothDeliveryFrequency[target] + 1;

				tmpCalculationDate = fixedPurchaseCalculator.CalculateNextShippingDate(fpKbn, fpSetting, tmpCalculationDate, daysRequired, minSpan, calculationMode);
			}

			return eachMothDeliveryFrequency;
		}
		#endregion
	}
}
