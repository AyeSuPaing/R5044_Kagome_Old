using System;
using System.Globalization;
using w2.ExternalAPI.Common.Command.ApiCommand.Stock;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Import;

namespace Pxxxx_Sample.w2.Commerce.Batch.ExternalAPI
{

    public class ApiImportCommandBuilder_Pxxxx_Sample_I_0028 : ApiImportCommandBuilder
   {
    	protected override void Import(ApiEntry apiEntry)
    	{
			try
			{
				//行データ分割
				foreach (object item in apiEntry.Data.ItemArray)
				{
					Console.WriteLine(item);
				}

				//分割したデータを元にコマンド用引数クラス生成
				AddStockQuantityArg addStockQuantityArg = new AddStockQuantityArg
				{
					ProductID = (string)apiEntry.Data[0]
					,
					VariationID = (string)apiEntry.Data[1]
					,
					Stock = Convert.ToInt32(apiEntry.Data[2])
				};

				//コマンド生成
				AddStockQuantity addStockQuantity = new AddStockQuantity();
				//コマンド実行
				AddStockQuantityResult addtStockQuantityResult = (AddStockQuantityResult)addStockQuantity.Do(addStockQuantityArg);

				//コマンド成功可否
				if (addtStockQuantityResult.ResultStatus == EnumResultStatus.Complete)
				{
					//コマンド成功
					Console.WriteLine("AddStockQuantity成功");
				}
				else
				{
					//コマンド失敗
					Console.WriteLine("AddStockQuantity成功");
				}
			}
			catch (Exception ex)
			{
				//なんか失敗した
				//ログ書く
				Console.WriteLine("エラー：" + ex.Message.ToString(CultureInfo.InvariantCulture));
				Console.WriteLine("スタックトレース：" + ex.StackTrace.ToString(CultureInfo.InvariantCulture));

				throw;
			}
    	}
   }
}
