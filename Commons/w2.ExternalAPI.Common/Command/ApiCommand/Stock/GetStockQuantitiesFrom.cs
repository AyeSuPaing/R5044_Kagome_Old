/*
=========================================================================================================
  Module      : GetStockQuantitiesFromコマンドクラス(GetStockQuantitiesFrom.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Data;
using w2.App.Common.Util;
using w2.App.Common.Stock;
using w2.ExternalAPI.Common.Logging;

namespace w2.ExternalAPI.Common.Command.ApiCommand.Stock
{
	#region Command
	/// <summary>
	/// GetStockQuantitiesFromコマンドクラス
	/// </summary>
	public class GetStockQuantitiesFrom : ApiCommandBase
	{
		#region #Execute コマンド実行処理
		/// <summary>
		/// コマンド実行処理
		/// </summary>
		/// <param name="apiCommandArg">コマンド引数クラス</param>
		/// <returns>コマンド実行結果</returns>
		protected override ApiCommandResult Execute(ApiCommandArgBase apiCommandArg)
		{
			GetStockQuantitiesFromArg arg = (GetStockQuantitiesFromArg) apiCommandArg;

			//引数情報をログに
			ApiLogger.Write(LogLevel.info, "コマンド引数情報:" + GetType().Name,
				string.Format("TimeSpan:'{0}'",
				(arg.TimeSpan == null) ? "Null" : arg.TimeSpan.ToString()
				));

			// 作成日チェック
			arg.Validate(arg.TimeSpan);

			StockCommon stockCommon = new StockCommon();

			// 在庫数増減を取得
			arg.TimeSpan.MaxEndTime = DateTime.Now.AddMinutes(-5);	// 5分前であることを保証
			DataTable dataTable = stockCommon.GetStockQuantitiesFrom(arg.TimeSpan);

			return new GetStockQuantitiesFromResult(EnumResultStatus.Complete, dataTable);
		}
		#endregion
	}
	#endregion

	#region Arg
	/// <summary>
	/// GetStockQuantitiesFromコマンド用引数クラス
	/// </summary>
	public class GetStockQuantitiesFromArg : ApiCommandArgBase
	{
		#region プロパティ

		/// <summary>過去における厳密期間</summary>
		public PastAbsoluteTimeSpan TimeSpan;

		#endregion
	}

	#endregion

	#region Result
	/// <summary>
	/// GetStockQuantitiesFromコマンド用実行結果クラス
	/// </summary>
	public class GetStockQuantitiesFromResult : ApiCommandResult
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="enumResultStatus"></param>
		/// <param name="dataTable"></param>
		public GetStockQuantitiesFromResult(EnumResultStatus enumResultStatus, DataTable dataTable)　: base(enumResultStatus)
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
