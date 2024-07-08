/*
=========================================================================================================
  Module      : Request Key(RequestKey.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order.OPlux.Requests.Helper
{
	/// <summary>
	/// Request key
	/// </summary>
	public class RequestKey : Attribute
	{
		#region +Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="requestName">Request name</param>
		public RequestKey(string requestName)
		{
			this.RequestName = requestName;
		}
		#endregion

		#region +Properties
		/// <summary>Request name</summary>
		public string RequestName { get; private set; }
		/// <summary>Format value</summary>
		public string FormatValue { get; set; }
		#endregion
	}
}
