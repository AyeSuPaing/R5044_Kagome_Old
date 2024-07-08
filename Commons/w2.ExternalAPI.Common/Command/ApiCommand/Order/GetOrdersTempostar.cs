/*
=========================================================================================================
  Module      : Get orders tempostar(GetOrdersTempostar.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Data;
using w2.App.Common.Order;

namespace w2.ExternalAPI.Common.Command.ApiCommand.Order
{
	/// <summary>
	/// Get orders tempostar
	/// </summary>
	public class GetOrdersTempostar : ApiCommandBase
	{
		#region #Execute
		/// <summary>
		/// Excute
		/// </summary>
		/// <param name="apiCommandArg">Api command argument</param>
		/// <returns>Result</returns>
		protected override ApiCommandResult Execute(ApiCommandArgBase apiCommandArg = null)
		{
			var orderCommon = new OrderCommon();

			// Order information
			var result = orderCommon.GetOrdersByExtendStatus();

			return new GetOrdersTempostarResult(EnumResultStatus.Complete, result);
		}
		#endregion
	}

	#region Result
	/// <summary>
	/// Get orders Tempostar result
	/// </summary>
	public class GetOrdersTempostarResult : ApiCommandResult
	{
		#region Constructor
		/// <summary>
		/// Get orders Tempostar result
		/// </summary>
		/// <param name="enumResultStatus">Enum result status</param>
		/// <param name="dataTable">Data table</param>
		public GetOrdersTempostarResult(EnumResultStatus enumResultStatus, DataTable dataTable)
			: base(enumResultStatus)
		{
			ResultTable = dataTable;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Result table
		/// </summary>
		public DataTable ResultTable { get; set; }
		#endregion
	}
	#endregion
}