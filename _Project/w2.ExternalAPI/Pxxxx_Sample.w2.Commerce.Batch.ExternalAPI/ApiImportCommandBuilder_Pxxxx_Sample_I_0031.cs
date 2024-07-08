using System;
using System.Globalization;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Import;

namespace Pxxxx_Sample.w2.Commerce.Batch.ExternalAPI
{

    public class ApiImportCommandBuilder_Pxxxx_Sample_I_0031 : ApiImportCommandBuilder
   {
	   #region #Import インポート処理の実装
	   /// <summary>
	   /// インポート処理の実装
	   /// </summary>
	   /// <param name="apiEntry">処理対象の情報を持つApiEntry</param>
	   protected override void Import(ApiEntry apiEntry)
	   {
		   try
		   {
			   foreach (object item in apiEntry.Data.ItemArray)
			   {
				   Console.WriteLine(item);
			   }

			   //分割したデータを元にコマンド用引数クラス生成
			   ShipOrderArg shipOrderArg = new ShipOrderArg
			   {
				   OrderId = (string)apiEntry.Data[0],
				   ShippingNo = ((string)apiEntry.Data[1] == "") ? 1 : Convert.ToInt32(apiEntry.Data[1]),
				   ShippingCheckNo = (string)apiEntry.Data[2],
				   DoesMail = (string)apiEntry.Data[3] != "" && Convert.ToBoolean(apiEntry.Data[3]),
				   ApiMemo = (string)apiEntry.Data[4],
				   IsOverwriteMemo = (string)apiEntry.Data[5] != "" && Convert.ToBoolean(apiEntry.Data[5])
			   };
			   //コマンド生成
			   ShipOrder shipOrder = new ShipOrder();
			   //コマンド実行
			   ShipOrderResult shipOrderResult = (ShipOrderResult)shipOrder.Do(shipOrderArg);

			   //コマンド成功可否
			   if (shipOrderResult.ResultStatus == EnumResultStatus.Complete)
			   {
				   //コマンド成功
				   Console.WriteLine("ShipOrder成功");
			   }
			   else
			   {
				   //コマンド失敗
				   Console.WriteLine("ShipOrder成功");
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
