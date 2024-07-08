using System;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Command.ApiCommand.Test;

using w2.ExternalAPI.Common.Import;
using w2.ExternalAPI.Common.Entry;

namespace Pxxxx_Sample.w2.Commerce.Batch.ExternalAPI
{
	public class ApiImportCommandBuilder_Pxxxx_Sample_I_0001 : ApiImportCommandBuilder 
	{
		
		/// <summary>
		/// インポート実行処理の実装
		/// </summary>
		/// <param name="apiEntry"></param>
		protected override void Import(ApiEntry apiEntry)
		{
			//分割したデータを元にコマンド用引数クラス生成
			TestApiCommandArg testApiCommandArg = new TestApiCommandArg();
			testApiCommandArg.Para1 = (string)apiEntry.Data[0];
			testApiCommandArg.Para2 = (string)apiEntry.Data[1];
			testApiCommandArg.Para3 = (string)apiEntry.Data[2];
			
			//コマンド生成
			TestApiCommand testApiCommand = new TestApiCommand();
			//コマンド実行
			ApiCommandResult apiCommandResult = testApiCommand.Do(testApiCommandArg);

			//コマンド成功可否
			if(apiCommandResult.ResultStatus == EnumResultStatus.Complete)
			{
				Console.WriteLine("コマンド成功");
			}
			else
			{
				Console.WriteLine("コマンド失敗、戻り値:" + apiCommandResult.ResultStatus.ToString());
			}
			
			//終了
			testApiCommandArg = null;
			testApiCommand = null;
			apiCommandResult = null;

		}

	}
}
