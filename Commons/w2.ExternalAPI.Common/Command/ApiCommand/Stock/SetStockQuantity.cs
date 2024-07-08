/*
=========================================================================================================
  Module      : SetStockQuantityコマンドクラス(SetStockQuantity.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Stock;
using w2.ExternalAPI.Common.Logging;

namespace w2.ExternalAPI.Common.Command.ApiCommand.Stock
{
	#region Command
	/// <summary>
	/// SetStockQuantityコマンドクラス
	/// </summary>
	public class SetStockQuantity : ApiCommandBase
	{
		#region #Execute コマンド実行処理
		/// <summary>
		/// コマンド実行処理
		/// </summary>
		/// <param name="apiCommandArg">コマンド引数クラス</param>
		/// <returns>コマンド実行結果</returns>
		protected override ApiCommandResult Execute(ApiCommandArgBase apiCommandArg)
		{
			//Arg取得
			SetStockQuantityArg setStockQuantityArg = (SetStockQuantityArg) apiCommandArg;

			//引数情報をログに
			ApiLogger.Write(LogLevel.info, "コマンド引数情報:" + GetType().Name,
				string.Format("ProductID:'{0}',VariationID:'{1}',Stock:'{2}'",
				setStockQuantityArg.ProductID ?? "Null",
				setStockQuantityArg.VariationID ?? "Null",
				setStockQuantityArg.Stock.ToString()
				));

			//API呼び出し
			StockCommon stockCommon = new StockCommon();
			stockCommon.SetStockQuantity(
					setStockQuantityArg.ProductID
					, setStockQuantityArg.VariationID
					, setStockQuantityArg.Stock
					);

			//正常終了列挙体セット
			SetStockQuantityResult setStockQuantityResult = new SetStockQuantityResult(EnumResultStatus.Complete);
			
			return setStockQuantityResult;

		}
		#endregion

	}
	#endregion

	#region Arg
	/// <summary>
	/// SetStockQuantityコマンド引数クラス
	/// </summary>
	public class SetStockQuantityArg : ApiCommandArgBase
	{
		/// <summary>商品ID</summary>
		public string ProductID { get; set; }
		/// <summary>商品バリエーションID</summary>
		public string VariationID { get; set; }
		/// <summary>在庫数</summary>
		public int Stock { get; set; }
	}

	#endregion

	#region Result
	/// <summary>
	/// SetStockQuantityコマンド実行結果クラス
	/// </summary>
	public class SetStockQuantityResult : ApiCommandResult
	{
		public SetStockQuantityResult(EnumResultStatus enumResultStatus) : base(enumResultStatus)
		{
		}

		public int MailCount { get; set; }
	}

	#endregion
}
