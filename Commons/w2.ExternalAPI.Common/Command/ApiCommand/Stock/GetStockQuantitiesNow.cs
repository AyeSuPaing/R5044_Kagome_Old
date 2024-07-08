/*
=========================================================================================================
  Module      : GetStockQuantitiesNowコマンドクラス(GetStockQuantitiesNow.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System.Data;
using w2.App.Common.Stock;

using w2.ExternalAPI.Common.Logging;

namespace w2.ExternalAPI.Common.Command.ApiCommand.Stock
{
	#region Command
	/// <summary>
	/// GetStockQuantitiesNowコマンドクラス
	/// </summary>
	public class GetStockQuantitiesNow :ApiCommandBase
	{
		#region #Execute コマンド実行処理
		/// <summary>
		/// コマンド実行処理
		/// </summary>
		/// <param name="apiCommandArg">コマンド引数クラス</param>
		/// <returns>コマンド実行結果</returns>
		protected override ApiCommandResult Execute(ApiCommandArgBase apiCommandArg)
		{
			//引数情報をログに
			ApiLogger.Write(LogLevel.info, "コマンド引数情報:" + GetType().Name,"引数なし");

			StockCommon stockCommon = new StockCommon();

			DataTable dataTable = stockCommon.GetStockQuantitiesNow();

			return new GetStockQuantitiesNowResult(EnumResultStatus.Complete, dataTable);
		}
		#endregion
	}
	#endregion

	#region Arg
	/// <summary>
	/// GetStockQuantitiesNowコマンド用引数クラス
	/// </summary>
	public class GetStockQuantitiesNowArg : ApiCommandArgBase
	{
		
	}
	#endregion

	#region Result
	/// <summary>
	/// GetStockQuantitiesNowコマンド用実行結果クラス
	/// </summary>
	public class GetStockQuantitiesNowResult : ApiCommandResult
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="enumResultStatus"></param>
		/// <param name="dataTable"></param>
		public GetStockQuantitiesNowResult(EnumResultStatus enumResultStatus, DataTable dataTable) : base(enumResultStatus)
		{
			ResultTable = dataTable;
		}
		#endregion

		#region プロパティ

		public DataTable ResultTable { get; set; }

		#endregion
	}
	#endregion
}
