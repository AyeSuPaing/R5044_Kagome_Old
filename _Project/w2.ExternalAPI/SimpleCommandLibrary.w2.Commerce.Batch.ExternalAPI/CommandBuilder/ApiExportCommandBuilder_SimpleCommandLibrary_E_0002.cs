using System;
using System.Data;
using w2.App.Common.Util;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Command.ApiCommand.Stock;
using w2.ExternalAPI.Common.Export;

namespace SimpleCommandLibrary.w2.Commerce.Batch.ExternalAPI
{
	/// <summary>
	/// 10分前から5前の間の在庫の増減を取得する(5分間を対象）汎用コマンドビルダ
	/// </summary>
	public class ApiExportCommandBuilder_SimpleCommandLibrary_E_0002 : ApiExportCommandBuilder
   {
    	public override DataTable GetDataTable()
    	{
			// APIコマンド作る
			GetStockQuantitiesFrom cmd = new GetStockQuantitiesFrom();

			GetStockQuantitiesFromArg stockQuantitiesFromArg = new GetStockQuantitiesFromArg();

			// パラメータ設定（開始時間のみ指定）
			stockQuantitiesFromArg.TimeSpan = new PastAbsoluteTimeSpan(-5, DateTime.Now.AddMinutes(-10));

			// コマンド実行
			GetStockQuantitiesFromResult stockQuantitiesFromResult = (GetStockQuantitiesFromResult)cmd.Do(stockQuantitiesFromArg);

			return stockQuantitiesFromResult.ResultTable;
		}

    	protected override object[] Export(IDataRecord record)
    	{
			return record.ToArray();
    	}
   }
}
