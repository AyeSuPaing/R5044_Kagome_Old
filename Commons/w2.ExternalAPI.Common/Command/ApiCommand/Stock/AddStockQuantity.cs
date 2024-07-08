/*
=========================================================================================================
  Module      : AddStockQuantityコマンドクラス(AddStockQuantity.cs)
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
	/// AddStockQuantityコマンドクラス
	/// </summary>
	public class AddStockQuantity : ApiCommandBase
	{
		#region #ApiCommandResult コマンド実行処理
		/// <summary>
		/// コマンド実行処理
		/// </summary>
		/// <param name="apiCommandArg">コマンド引数クラス</param>
		/// <returns>コマンド実行結果</returns>
		protected override ApiCommandResult Execute(ApiCommandArgBase apiCommandArg)
		{
		
			//Arg取得
			AddStockQuantityArg addStockQuantityArg = (AddStockQuantityArg)apiCommandArg;

			//引数情報をログに
			ApiLogger.Write(LogLevel.info, "コマンド引数情報:" + GetType().Name,
				string.Format("ProductID:'{0}',VariationID:'{1}',Stock:'{2}'",
				addStockQuantityArg.ProductID ?? "Null",
				addStockQuantityArg.VariationID ?? "Null",
				addStockQuantityArg.Stock.ToString()
				));

			//Result
		
			StockCommon stockCommon = new StockCommon();
			stockCommon.AddStockQuantity(
					addStockQuantityArg.ProductID
					, addStockQuantityArg.VariationID
					, addStockQuantityArg.Stock
					);

			//正常終了列挙体セット
			AddStockQuantityResult addStockQuantityResult = new AddStockQuantityResult(EnumResultStatus.Complete);
		
			return addStockQuantityResult;
		}
		#endregion
	}
	#endregion

	#region Arg
	/// <summary>
	/// AddStockQuantityコマンド用引数クラス
	/// </summary>
	public class AddStockQuantityArg : ApiCommandArgBase
	{
		/// <summary>
		/// 商品ID
		/// </summary>
		public string ProductID { get; set; }

		/// <summary>
		/// 商品バリエーションID
		/// </summary>
		public string VariationID { get; set; }

		/// <summary>
		/// 在庫数
		/// </summary>
		public int Stock { get; set; }
	}
	#endregion

	#region Result
	/// <summary>
	/// AddStockQuantityコマンド用実行結果クラス
	/// </summary>
	public class AddStockQuantityResult : ApiCommandResult
	{
		public AddStockQuantityResult(EnumResultStatus enumResultStatus) : base(enumResultStatus)
		{
		}
	}
	#endregion
}
