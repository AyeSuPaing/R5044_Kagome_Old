/*
=========================================================================================================
  Module      : 楽天受注連携APIログクラス (OrderApiService_Logging.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Logger;

namespace w2.App.Common.Mall.Rakuten
{
	/// <summary>
	/// SOAPクラスを継承したログするSOAPクラス
	/// </summary>
	public partial class OrderApiService
	{
		/// <summary>
		/// 楽天APIを呼び出す時、ログ出力
		/// </summary>
		/// <param name="methodName">実行する関数名</param>
		/// <param name="parameters">引数一覧</param>
		/// <returns>楽天APIのレスポンス</returns>
		protected new object[] Invoke(string methodName, object[] parameters)
		{
			string id = WriteBeginLog(methodName, parameters);

			// 実行
			object[] results = base.Invoke(methodName, parameters);

			WriteEndLog(id, results);

			return results;
		}

		/// <summary>
		/// 実行前ログ書き出し
		/// </summary>
		/// <param name="methodName">実行する関数名</param>
		/// <param name="parameters">引数一覧</param>
		/// <returns>ログID</returns>
		protected string WriteBeginLog(string methodName, object[] parameters)
		{
			string id = Guid.NewGuid().ToString("N");

			FileLogger.WriteDebug(string.Format(
				"OrderApiService: {{at:'{0}', id:'{1}', call:'begin', method:'{2}', param:{3}, stacktrace:{4}}}",
				DateTime.Now.ToString(""),
				id,
				methodName,
				Model2JsonString.GenerateJson(parameters),
				GetStackTraceJsonString()));

			return id;
		}

		/// <summary>
		/// 実行後ログ書き出し
		/// </summary>
		/// <param name="id">ログID</param>
		/// <param name="results">楽天APIのレスポンス</param>
		protected void WriteEndLog(string id, object[] results)
		{
			FileLogger.WriteDebug(string.Format(
				"OrderApiService: {{at:'{0}', id:'{1}', call:'end', result:{2}}}",
				DateTime.Now.ToString(),
				id,
				Model2JsonString.GenerateJson(results)));
		}

		/// <summary>
		/// スタックトレースをJson文字列に変換
		/// </summary>
		/// <returns>スタックトレースから変換されたJson文字列</returns>
		protected string GetStackTraceJsonString()
		{
			var stackFrames = new System.Diagnostics.StackTrace(3, true).GetFrames();

			const int MAX_STACKFRAME_COUNT = 2;
			int max = (MAX_STACKFRAME_COUNT < stackFrames.Length) ? MAX_STACKFRAME_COUNT : stackFrames.Length;
			string[] jsonStrings = new string[max];
			for (int index = 0; index < max; index++)
			{
				jsonStrings[index] = string.Format(
					"{{file:'{0}', method:'{1}', line:{2}}}",
					stackFrames[index].GetFileName().Replace("\\", "/"),
					stackFrames[index].GetMethod(),
					stackFrames[index].GetFileLineNumber());
			}

			return string.Format("[{0}]", string.Join(",", jsonStrings));
		}
	}
}