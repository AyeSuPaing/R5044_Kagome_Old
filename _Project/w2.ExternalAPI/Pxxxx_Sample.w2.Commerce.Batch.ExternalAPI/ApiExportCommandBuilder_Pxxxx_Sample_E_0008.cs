using System;
using System.Data;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Command.ApiCommand.Stock;

using w2.ExternalAPI.Common.Export;
using w2.App.Common.Util;
namespace Pxxxx_Sample.w2.Commerce.Batch.ExternalAPI
{
	public class ApiExportCommandBuilder_Pxxxx_Sample_E_0008 : ApiExportCommandBuilder
	{
		public override DataTable GetDataTable()
		{
			//APIコマンド作る
			GetStockQuantitiesFrom cmd = new GetStockQuantitiesFrom();

			GetStockQuantitiesFromArg stockQuantitiesFromArg = new GetStockQuantitiesFromArg();

			// パラメータ設定（開始時間、終了時間を指定）
			stockQuantitiesFromArg.TimeSpan = new PastAbsoluteTimeSpan(-5,
																		DateTime.Parse("2012-02-16 16:17:49.053"),
																		DateTime.Parse("2012-02-16 19:26:52.787"));

			// コマンド実行
			GetStockQuantitiesFromResult stockQuantitiesFromResult = (GetStockQuantitiesFromResult)cmd.Do(stockQuantitiesFromArg);

			return stockQuantitiesFromResult.ResultTable;
		}

		#region #Export 出力処理
		/// <summary>出力処理</summary>
		protected override object[] Export(IDataRecord record)
		{
			return record.ToArray();
		}
		#endregion
	}
}
