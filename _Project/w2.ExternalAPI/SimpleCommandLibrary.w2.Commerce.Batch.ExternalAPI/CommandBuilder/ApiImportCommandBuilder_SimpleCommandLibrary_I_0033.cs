using System;
using System.Globalization;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Import;
using w2.ExternalAPI.Common.Entry;

namespace SimpleCommandLibrary.w2.Commerce.Batch.ExternalAPI
{
	/// <summary>
	/// 注文キャンセルを行う汎用コマンドビルダ
	/// </summary>
    public class ApiImportCommandBuilder_SimpleCommandLibrary_I_0033 : ApiImportCommandBuilder
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
			CancelOrderArg cancelOrderArg = new CancelOrderArg
			{
				OrderId = (string)apiEntry.Data[0],
				DoesMail = (string)apiEntry.Data[1] != "" && Convert.ToBoolean(apiEntry.Data[1]),
				ApiMemo = (string)apiEntry.Data[2],
				IsOverwriteMemo = (string)apiEntry.Data[3] != "" && Convert.ToBoolean(apiEntry.Data[3])
			};
			//コマンド生成
			CancelOrder cancelOrder = new CancelOrder();
			//コマンド実行
			CancelOrderResult cancelOrderResult = (CancelOrderResult)cancelOrder.Do(cancelOrderArg);
			
	   }
	   #endregion
   }
}
