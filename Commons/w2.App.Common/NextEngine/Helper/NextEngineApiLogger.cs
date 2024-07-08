/*
=========================================================================================================
  Module      : ネクストエンジンAPIロガークラス (NextEngineApiLogger.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text.RegularExpressions;
using w2.Common.Logger;

namespace w2.App.Common.NextEngine.Helper
{
	/// <summary>
	/// ネクストエンジンAPIロガークラス
	/// </summary>
	public class NextEngineApiLogger
	{
		/// <summary>
		/// Apiログ出力
		/// </summary>
		/// <param name="request">リクエスト</param>
		/// <param name="response">レスポンス</param>
		public static void WriteApiLog(
			NextEngineRequest request,
			string response)
		{
			var result = response.Contains(NextEngineConstants.FLG_RESULT_SUCCESS)
				? "OK"
				: "NG";
			var logMessage = string.Format(
				"【{0}】\t{1}?{2}\tresponse:{3}",
				result,
				request.EndpointUri,
				request.CreatePostStringForWriteApiLog(),
				Regex.Unescape(response.Replace("\"", "")));
			FileLogger.Write("NextEngineApi", logMessage);
		}

		/// <summary>
		/// Apiログ出力(例外発生時)
		/// </summary>
		/// <param name="ex">例外</param>
		public static void WriteApiLog(Exception ex)
		{
			FileLogger.Write("NextEngineApi", ex);
		}
	}
}