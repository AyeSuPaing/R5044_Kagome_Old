using System;
using System.Globalization;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Command.ApiCommand.Stock;
using w2.ExternalAPI.Common.Import;
using w2.ExternalAPI.Common.Entry;

namespace SimpleCommandLibrary.w2.Commerce.Batch.ExternalAPI
{
	/// <summary>
	/// 再入荷通知メールを送信する汎用コマンドビルダ
	/// </summary>
    public class ApiImportCommandBuilder_SimpleCommandLibrary_I_0008 : ApiImportCommandBuilder
   {
	   #region #Import インポート処理の実装
	   /// <summary>
	   /// インポート処理の実装
	   /// </summary>
	   /// <param name="apiEntry"></param>
	   protected override void Import(ApiEntry apiEntry)
	   {
			
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
	   }
	   #endregion
   }
}
