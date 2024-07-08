using System;
using System.Data;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Export;
using w2.ExternalAPI.Common.Command.ApiCommand.Stock;

namespace SimpleCommandLibrary.w2.Commerce.Batch.ExternalAPI
{

	/// <summary>
	/// 現在の在庫数を取得する汎用コマンドビルダ
	/// </summary>
    public class ApiExportCommandBuilder_SimpleCommandLibrary_E_0001 : ApiExportCommandBuilder
   {
    	public override DataTable GetDataTable()
    	{
			//APIコマンド作る
			GetStockQuantitiesNow cmd = new GetStockQuantitiesNow();

			GetStockQuantitiesNowArg getStockQuantitiesNowArg = new GetStockQuantitiesNowArg();

			// コマンド実行
			GetStockQuantitiesNowResult getStockQuantitiesNowResult 
				= (GetStockQuantitiesNowResult)cmd.Do(getStockQuantitiesNowArg);

			return getStockQuantitiesNowResult.ResultTable;
    	}

    	protected override object[] Export(IDataRecord record)
    	{
			return record.ToArray();
    	}
   }
}
