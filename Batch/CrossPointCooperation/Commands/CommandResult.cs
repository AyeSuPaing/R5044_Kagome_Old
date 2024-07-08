/*
=========================================================================================================
  Module      : Command Result (CommandResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Text;

namespace w2.Commerce.Batch.CrossPointCooperation.Commands
{
	/// <summary>
	/// Command result
	/// </summary>
	public class CommandResult
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CommandResult()
		{
			this.ErrorMessages = new StringBuilder();
		}

		/// <summary>Error messages</summary>
		public StringBuilder ErrorMessages { get; set; }
	}
}
