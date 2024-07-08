/*
=========================================================================================================
  Module      : 商品表示情報サービス (DispProductInfoService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Transactions;
using w2.Common.Sql;
using w2.Domain.ProductRanking;

namespace w2.Domain.DispProductInfo
{
	/// <summary>
	/// 商品表示情報サービス
	/// </summary>
	public class DispProductInfoService : ServiceBase
	{
		#region 定数
		/// <summary>タイムアウト(ミリ秒)</summary>
		private const int UPDATE_STATISTICS_TIME_OUT = 600;
		#endregion

		#region +GetTotalResultOrder 注文データ集計結果取得
		/// <summary>
		/// 注文データ集計結果取得
		/// </summary>
		/// <param name="productRankingModel">商品ランキング設定マスタモデル</param>
		/// <returns>注文データ集計結果</returns>
		public DispProductInfoModel[] GetTotalResultOrder(ProductRankingModel productRankingModel)
		{
			using (var repository = new DispProductInfoRepository { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				var dispProductInfoModel = repository.GetTotalResultOrder(productRankingModel);
				return dispProductInfoModel;
			}
		}
		#endregion

		#region ~UpdateStatistics 統計情報更新
		/// <summary>
		/// 統計情報更新
		/// </summary>
		public void UpdateStatistics()
		{
			using (var repository = new DispProductInfoRepository() { CommandTimeout = UPDATE_STATISTICS_TIME_OUT })
			{
				repository.UpdateStatisticsProductPrice();
				repository.UpdateStatisticsProductCategory();
				repository.UpdateStatisticsProductStock();
				repository.UpdateStatisticsProduct();
				repository.UpdateStatisticsDispProductInfo();
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Insert(DispProductInfoModel model, SqlAccessor accessor)
		{
			using (var repository = new DispProductInfoRepository(accessor))
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="dataKbn">データ区分</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(string shopId, string dataKbn, SqlAccessor accessor)
		{
			using (var repository = new DispProductInfoRepository(accessor))
			{
				repository.Delete(shopId, dataKbn);
			}
		}
		#endregion
	}
}
