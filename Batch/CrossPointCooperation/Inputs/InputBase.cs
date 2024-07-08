/*
=========================================================================================================
  Module      : Input Base (InputBase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;

namespace w2.Commerce.Batch.CrossPointCooperation.Inputs
{
	/// <summary>
	/// Input base
	/// </summary>
	public class InputBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		protected InputBase()
		{
			this.DataSource = new Hashtable();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source">Source</param>
		protected InputBase(Hashtable source)
		{
			this.DataSource = source;
		}

		public Hashtable DataSource { get; set; }
	}
}
