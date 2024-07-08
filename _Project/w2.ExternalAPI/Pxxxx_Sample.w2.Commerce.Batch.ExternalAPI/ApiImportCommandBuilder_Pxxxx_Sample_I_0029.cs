using System;
using System.Globalization;
using w2.ExternalAPI.Common.Command.ApiCommand.Stock;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Import;


namespace Pxxxx_Sample.w2.Commerce.Batch.ExternalAPI
{

    public class ApiImportCommandBuilder_Pxxxx_Sample_I_0029 : ApiImportCommandBuilder
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
			   SendArrivalMailArg sendArrivalMailArg = new SendArrivalMailArg
			   {
				   ProductID = (string)apiEntry.Data[0]
				   ,
				   VariationID = (string)apiEntry.Data[1]
				   ,
				   Stock = ((string)apiEntry.Data[2] == "") ? (int?)null : Convert.ToInt32(apiEntry.Data[2])
				   ,
				   UseSafetyCriteria = Convert.ToBoolean(apiEntry.Data[3])
			   };

			   //コマンド生成
			   SendArrivalMail sendArrivalMail = new SendArrivalMail();
			   //コマンド実行
			   SendArrivalMailResult sendArrivalMailResult = (SendArrivalMailResult)sendArrivalMail.Do(sendArrivalMailArg);

			   //コマンド成功可否
			   if (sendArrivalMailResult.ResultStatus == EnumResultStatus.Complete)
			   {
				   //コマンド成功
				   Console.WriteLine("SendArrivalMail成功");
			   }
			   else
			   {
				   //コマンド失敗
				   Console.WriteLine("SendArrivalMail成功");
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
