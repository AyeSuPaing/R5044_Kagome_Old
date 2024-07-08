using System;
using System.Globalization;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Command.ApiCommand.RealStock;
using w2.ExternalAPI.Common.Import;
using w2.ExternalAPI.Common.Entry;

namespace SimpleCommandLibrary.w2.Commerce.Batch.ExternalAPI
{
	/// <summary>
	/// リアル店舗在庫数を更新する汎用コマンドビルダ
	/// </summary>
	public class ApiImportCommandBuilder_SimpleCommandLibrary_I_0036 : ApiImportCommandBuilder
	{
		#region #Import インポート処理の実装
		/// <summary>
		/// インポート処理の実装
		/// </summary>
		/// <param name="apiEntry"></param>
		protected override void Import(ApiEntry apiEntry)
		{
			//分割したデータを元にコマンド用引数クラス生成
			SetRealStockQuantityArg setRealStockQuantityArg = new SetRealStockQuantityArg
			{
				RealShopID = (string)apiEntry.Data[0],
				ProductID = (string)apiEntry.Data[1],
				VariationID = (string)apiEntry.Data[2],
				RealStock = Convert.ToInt32(apiEntry.Data[3])
			};

			//コマンド生成
			SetRealStockQuantity setRealStockQuantity = new SetRealStockQuantity();

			//コマンド実行
			SetRealStockQuantityResult setRealStockQuantityResult = (SetRealStockQuantityResult)setRealStockQuantity.Do(setRealStockQuantityArg);
		}
		#endregion
	}
}