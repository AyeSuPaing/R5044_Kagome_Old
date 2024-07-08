/*
=========================================================================================================
  Module      : Error Helper (ErrorHelper.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Util;

namespace w2.App.Common.CrossPoint.Helper
{
	/// <summary>
	/// クロスポイントエラーヘルパー
	/// </summary>
	public class ErrorHelper
	{
		/// <summary>
		/// Create cross point error
		/// </summary>
		/// <param name="errorMessage">Error message</param>
		/// <param name="errorCode">Error code</param>
		/// <returns>Message</returns>
		public static string CreateCrossPointApiError(string errorMessage, string errorCode)
		{
			var message = string.Format(
				"[CrossPoint]{0}:{1}",
				errorCode,
				errorMessage);
			return message;
		}

		/// <summary>
		/// Create cross point linkage error
		/// </summary>
		/// <param name="errorCode">Error code</param>
		/// <returns>Message</returns>
		public static string CreateCrossPointLinkageError(string errorCode)
		{
			var message = string.Format(
				"[CrossPoint]:{0}{1}",
				errorCode,
				MessageManager.GetMessages(Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR));
			return message;
		}
	}
}
