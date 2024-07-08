/*
=========================================================================================================
  Module      : Base OPlux Response(BaseOPluxResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Helper;
using w2.Common.Web;

namespace w2.App.Common.Order.OPlux
{
	/// <summary>
	/// Base O-PLUX response
	/// </summary>
	public abstract class BaseOPluxResponse : IHttpApiResponseData
	{
		#region +Constructor
		/// <summary>
		/// Create response string
		/// </summary>
		/// <returns>Response string</returns>
		public string CreateResponseString()
		{
			return SerializeHelper.Serialize(this);
		}
		#endregion
	}
}
