using System;
using System.Data;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Command.ApiCommand.Stock;

using w2.ExternalAPI.Common.Export;

namespace Pxxxx_Sample.w2.Commerce.Batch.ExternalAPI
{
	public class ApiExportCommandBuilder_Pxxxx_Sample_E_0003 : ApiExportCommandBuilder
    {

		#region #Export 出力処理
		/// <summary>出力処理</summary>
		protected override object[] Export(IDataRecord record)
		{
			return record.ToArray();
		}
		#endregion

		public override DataTable GetDataTable()
		{
			//APIコマンド作る
			GetStockQuantitiesNow cmd = new GetStockQuantitiesNow();

			GetStockQuantitiesNowArg getStockQuantitiesNowArg = new GetStockQuantitiesNowArg();

			// コマンド実行
			GetStockQuantitiesNowResult getStockQuantitiesNowResult = (GetStockQuantitiesNowResult)cmd.Do(getStockQuantitiesNowArg);

			return getStockQuantitiesNowResult.ResultTable;
		}
	}
}
