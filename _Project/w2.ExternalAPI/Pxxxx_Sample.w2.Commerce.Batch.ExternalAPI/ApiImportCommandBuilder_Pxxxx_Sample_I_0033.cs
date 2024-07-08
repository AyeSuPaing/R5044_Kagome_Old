using System;
using System.Globalization;
using w2.ExternalAPI.Common.Command.ApiCommand.Stock;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Import;
using w2.ExternalAPI.Common.Entry;

namespace Pxxxx_Sample.w2.Commerce.Batch.ExternalAPI
{

    public class ApiImportCommandBuilder_Pxxxx_Sample_I_0033 : ApiImportCommandBuilder
   {
	   #region #Import インポート処理の実装
	   /// <summary>
	   /// インポート処理の実装
	   /// </summary>
	   /// <param name="apiEntry"></param>
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
			   SetStockQuantityArg setStockQuantityArg = new SetStockQuantityArg
			   {
				   ProductID = (string)apiEntry.Data[0]
				   ,
				   VariationID = (string)apiEntry.Data[1]
				   ,
				   Stock = Convert.ToInt32((string)apiEntry.Data[2])
			   };

			   //コマンド生成
			   SetStockQuantity setStockQuantity = new SetStockQuantity();
			   //コマンド実行
			   SetStockQuantityResult setStockQuantityResult = (SetStockQuantityResult)setStockQuantity.Do(setStockQuantityArg);

			   //コマンド成功可否
			   if (setStockQuantityResult.ResultStatus == EnumResultStatus.Complete)
			   {
				   //コマンド成功
				   Console.WriteLine("SetStockQuantity成功");
			   }
			   else
			   {
				   //コマンド失敗
				   Console.WriteLine("SetStockQuantity成功");
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
	   #endregion
   }
}
