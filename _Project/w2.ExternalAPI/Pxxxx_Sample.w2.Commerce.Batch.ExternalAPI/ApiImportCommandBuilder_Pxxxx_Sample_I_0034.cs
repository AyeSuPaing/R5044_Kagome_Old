using System;
using System.Globalization;
using w2.ExternalAPI.Common.Command.ApiCommand.RealStock;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Import;
using w2.ExternalAPI.Common.Entry;

namespace Pxxxx_Sample.w2.Commerce.Batch.ExternalAPI
{
	public class ApiImportCommandBuilder_Pxxxx_Sample_I_0034 : ApiImportCommandBuilder
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

				//コマンド成功可否
				if (setRealStockQuantityResult.ResultStatus == EnumResultStatus.Complete)
				{
					//コマンド成功
					Console.WriteLine("SetRealStockQuantity成功");
				}
				else
				{
					//コマンド失敗
					Console.WriteLine("SetRealStockQuantity成功");
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
