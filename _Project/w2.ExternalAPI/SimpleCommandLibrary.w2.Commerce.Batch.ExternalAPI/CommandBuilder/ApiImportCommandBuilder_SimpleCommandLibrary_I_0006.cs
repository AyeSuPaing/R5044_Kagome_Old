using System;
using System.Globalization;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Command.ApiCommand.Stock;
using w2.ExternalAPI.Common.Import;
using w2.ExternalAPI.Common.Entry;

namespace SimpleCommandLibrary.w2.Commerce.Batch.ExternalAPI
{
	/// <summary>
	/// 在庫数を絶対値で更新する汎用コマンドビルダ
	/// </summary>
    public class ApiImportCommandBuilder_SimpleCommandLibrary_I_0006 : ApiImportCommandBuilder
   {
	   #region #Import インポート処理の実装
	   /// <summary>
	   /// インポート処理の実装
	   /// </summary>
	   /// <param name="apiEntry"></param>
	   protected override void Import(ApiEntry apiEntry)
	   {
		  
			//分割したデータを元にコマンド用引数クラス生成
			SetStockQuantityArg setStockQuantityArg = new SetStockQuantityArg
			{
				ProductID = (string)apiEntry.Data[0]
				,
				VariationID = (string)apiEntry.Data[1]
				,
				Stock = Convert.ToInt32(apiEntry.Data[2])
			};

			//コマンド生成
			SetStockQuantity setStockQuantity = new SetStockQuantity();
			
		   //コマンド実行
		   SetStockQuantityResult setStockQuantityResult = (SetStockQuantityResult)setStockQuantity.Do(setStockQuantityArg);

	   }
	   #endregion
   }
}
