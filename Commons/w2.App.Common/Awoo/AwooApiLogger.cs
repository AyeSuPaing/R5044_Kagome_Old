/*
=========================================================================================================
  Module      : AwooApi Logger(AwooApiLogger.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Util;
using w2.Common.Helper;
using w2.Common.Helper.Attribute;
using w2.Common.Wrapper;

namespace w2.App.Common.Awoo
{
	/// <summary>
	/// AwooApiLogger
	/// </summary>
	public class AwooApiLogger : ApiLoggerBase
	{
		private const string CONST_AWOO_API_NAME = "AwooApiLog";

		/// <summary>
		/// AwooApi processing type
		/// </summary>
		public enum AwooApiProcessingType
		{
			/// <summary>API request begin</summary>
			[EnumTextName("API Request Begin")]
			ApiRequestBegin,
			/// <summary>API request end</summary>
			[EnumTextName("API Request End")]
			ApiRequestEnd,
			/// <summary>API request error</summary>
			[EnumTextName("API Request Error")]
			ApiRequestError,
		}

		/// <summary>
		/// Write AwooApi log
		/// </summary>
		/// <param name="processingContent">Processing content</param>
		/// <param name="message">External log</param>
		public static void WriteAwooApiLog(AwooApiProcessingType processingContent, string message)
		{
			if (Constants.AWOO_LOG_OUTPUT_FLAG == false) return;

			var messageLine = string.Format(
				"[{0}] {1:yyyy年M月d日HH:mm:ss} {2}: {3}",
				CONST_AWOO_API_NAME,
				DateTimeWrapper.Instance.Now,
				processingContent.ToText(),
				(string.IsNullOrEmpty(message) ? string.Empty : message.Replace("\r\n", "\t")));
			Write(
				messageLine,
				Constants.PHYSICALDIRPATH_LOGFILE + CONST_AWOO_API_NAME + "/",
				string.Format("{0}_{1}", CONST_AWOO_API_NAME, DateTimeWrapper.Instance.Now.ToString(Constants.DATE_FORMAT_SHORT)),
				Constants.AWOO_API_LOGFILE_THRESHOLD);
		}
	}
}
