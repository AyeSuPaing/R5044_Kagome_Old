/*
=========================================================================================================
  Module      : 1日前から5分前までの在庫の増減数(ApiExportCommandBuilder_Dazzy_fscratch_0005_ExportStocks.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.App.Common.Util;
using w2.ExternalAPI.Common.Command.ApiCommand.Stock;
using w2.ExternalAPI.Common.Export;

namespace Dazzy_fscratch.w2.Commerce.Batch.ExternalAPI
{
	/// <summary>
	/// 1日前から5分前までの在庫の増減数を取得する（1日分を対象）汎用コマンドビルダ
	/// </summary>
	public class ApiExportCommandBuilder_Dazzy_fscratch_0005_ExportStocks : ApiExportCommandBuilder
	{
		#region #Export 出力処理
		/// <summary>
		/// 出力処理
		/// </summary>
		/// <param name="record">出力レコード</param>
		/// <returns>出力内容</returns>
		protected override object[] Export(IDataRecord record)
		{
			var result = new []{
				record["ProductID"],
				record["VariationID"],
				"" , // CooperationID1
				"" , // VariationCooperationID1
				"" , // CooperationID2
				"" , // VariationCooperationID2
				"" , // CooperationID3
				"" , // VariationCooperationID3
				"" , // CooperationID4
				"" , // VariationCooperationID4
				"" , // CooperationID5
				"" , // VariationCooperationID5
				record["Stock"]
			};
			return result;
		}
		#endregion

		#region #GetDataTable テーブル取得
		/// <summary>
		/// テーブル取得
		/// </summary>
		/// <returns>テーブル内容</returns>
		public override DataTable GetDataTable()
		{
			// APIコマンド作る
			var cmd = new GetStockQuantitiesFrom();

			var stockQuantitiesFromArg = new GetStockQuantitiesFromArg()
			{
				// パラメータ設定（開始時間のみ指定）
				TimeSpan = new PastAbsoluteTimeSpan(-5, DateTime.Now.AddDays(-1).AddMinutes(-5))
			};

			// コマンド実行
			var stockQuantitiesFromResult =
				(GetStockQuantitiesFromResult)cmd.Do(stockQuantitiesFromArg);

			return stockQuantitiesFromResult.ResultTable;
		}
		#endregion
	}
}