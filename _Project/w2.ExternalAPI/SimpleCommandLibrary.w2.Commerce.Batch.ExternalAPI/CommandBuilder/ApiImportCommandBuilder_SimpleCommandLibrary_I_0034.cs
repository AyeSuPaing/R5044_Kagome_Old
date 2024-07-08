using System;
using System.Globalization;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Import;
using w2.ExternalAPI.Common.Entry;

namespace SimpleCommandLibrary.w2.Commerce.Batch.ExternalAPI
{
	/// <summary>
	/// 注文を出荷完了にする汎用コマンドビルダ
	/// </summary>
    public class ApiImportCommandBuilder_SimpleCommandLibrary_I_0034 : ApiImportCommandBuilder
   {
	   #region #Import インポート処理の実装
	   /// <summary>
	   /// インポート処理の実装
	   /// </summary>
	   /// <param name="apiEntry">処理対象の情報を持つApiEntry</param>
	   protected override void Import(ApiEntry apiEntry)
	   {
		  
			//行データ分割
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
				IsOverwriteMemo = (string)apiEntry.Data[5] != "" && Convert.ToBoolean(apiEntry.Data[5]),
				ShippedDate = apiEntry.GetData<DateTime>(6)
			};
			//コマンド生成
			ShipOrder shipOrder = new ShipOrder();
			//コマンド実行
			ShipOrderResult shipOrderResult = (ShipOrderResult)shipOrder.Do(shipOrderArg);

	   }
	   #endregion
   }
}
