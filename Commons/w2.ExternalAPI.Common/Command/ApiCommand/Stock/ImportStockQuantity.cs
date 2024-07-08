/*
=========================================================================================================
  Module      : Tempostar import stock(ImportStockQuantity.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common;
using w2.App.Common.Stock;

namespace w2.ExternalAPI.Common.Command.ApiCommand.Stock
{
	/// <summary>
	/// Import stock quantity
	/// </summary>
	public class ImportStockQuantity : ApiCommandBase
	{
		#region Execute
		/// <summary>
		/// Execute
		/// </summary>
		/// <param name="apiCommandArg">Api Command arg base</param>
		/// <returns>Excute result</returns>
		protected override ApiCommandResult Execute(ApiCommandArgBase apiCommandArg)
		{
			var setStockQuantityArg = (ImportStockQuantityArg)apiCommandArg;

			var stockCommon = new StockCommon();
			stockCommon.ExternalSetStockQuantity(setStockQuantityArg.ProductID, setStockQuantityArg.VariationID, setStockQuantityArg.Stock);

			return new ImportStockQuantityResult(EnumResultStatus.Complete);
		}
		#endregion
	}

	#region Argument
	/// <summary>
	/// Import Stock Quantity Argument
	/// </summary>
	public class ImportStockQuantityArg : ApiCommandArgBase
	{
		/// <summary>Product Id</summary>
		public string ProductID { get; set; }

		/// <summary>Variation Id</summary>
		public string VariationID { get; set; }

		/// <summary>Stock</summary>
		public int Stock { get; set; }
	}

	#endregion

	#region Result
	/// <summary>
	/// Import stock quantity result
	/// </summary>
	public class ImportStockQuantityResult : ApiCommandResult
	{
		public ImportStockQuantityResult(EnumResultStatus enumResultStatus)
			: base(enumResultStatus)
		{
		}
	}
	#endregion
}