/*
=========================================================================================================
  Module      : Name Normalization Utility(NameNormalizationUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Web;

namespace w2.App.Common.Order.OPlux
{
	/// <summary>
	/// Name normalization utility
	/// </summary>
	public class NameNormalizationUtility
	{
		/// <summary>
		/// Create url parameters
		/// </summary>
		/// <param name="name">Name</param>
		/// <returns>Url parameters</returns>
		public static string CreateUrlParameters(string name)
		{
			var fields = string.Format(
				"{0},{1}",
				OPluxConst.CONST_FIELD_LAST_NAME,
				OPluxConst.CONST_FIELD_FIRST_NAME);
			var urlParameters = string.Format(
				"{0}={1}&{2}={3}",
				OPluxConst.PARAM_NAME,
				HttpUtility.UrlEncode(name),
				OPluxConst.PARAM_FIELDS,
				HttpUtility.UrlEncode(fields));

			return urlParameters;
		}
	}
}
