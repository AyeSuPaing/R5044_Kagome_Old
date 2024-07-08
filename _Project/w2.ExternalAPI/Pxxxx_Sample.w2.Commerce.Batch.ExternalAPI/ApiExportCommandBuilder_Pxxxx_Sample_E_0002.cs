using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Command.ApiCommand.Test;
using w2.ExternalAPI.Common.Export;

namespace Pxxxx_Sample.w2.Commerce.Batch.ExternalAPI
{
	public class ApiExportCommandBuilder_Pxxxx_Sample_E_0002 : ApiExportCommandBuilder
	{
		public override DataTable GetDataTable()
		{
			//APIコマンド作る
			TestExportCommand cmd = new TestExportCommand();

			TestExportCommandArg testExportCommandArg = new TestExportCommandArg();

			// コマンド実行
			TestExportApiCommandResult testExportApiCommandResult = (TestExportApiCommandResult)cmd.Do(testExportCommandArg);

			return testExportApiCommandResult.DataTable;
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
