/*
=========================================================================================================
  Module      : Apiログ出力クラス (CrossMallApiLogger.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using w2.Common.Logger;

namespace w2.App.Common.CrossMall
{
	/// <summary>
	/// Apiログ出力クラス
	/// </summary>
	public static class CrossMallApiLogger
	{
		/// <summary> ログ区分名 </summary>
		private const string LOG_KBN_NAME = "CrossMallApi";
		
		/// <summary>
		/// インフォログ出力
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="status">Getステータス</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <param name="totalResultCount">取得した件数</param>
		public static void WriteShortInfo(string orderId, string status, string errorMessage, int totalResultCount)
		{
			if (Constants.CROSS_MALL_INTEGRATION_ENABLE_LOGGING) return;
			
			var resultCountStr  = string.Empty;
			// 成功した場合、件数を表示する(取得できたが件数0の場合あり)
			if(status == CrossMallConstants.FLS_RESULT_STATUS_SUCCESS_VALUE) resultCountStr = string.Format("\r\n[成功]: {0}件", totalResultCount);
			var logStr = string.Format(
				"{0}\r\nRequest:Order_Id = {1}\r\nResponse:Get_Status = {2}{3}\r\n",
				resultCountStr,
				orderId,
				status,
				errorMessage);
			FileLogger.Write(LOG_KBN_NAME, logStr);
		}

		/// <summary>
		/// インフォログ出力(リクエストデータ含む)
		/// </summary>
		/// <param name="request">リクエスト文字列</param>
		/// <param name="response">レスポンス文字列</param>
		public static void WriteDetailInfo(string request, string response)
		{
			if (Constants.CROSS_MALL_INTEGRATION_ENABLE_LOGGING == false) return;

			var logStr = string.Format("\r\nRequest:{0}\r\nResponse:{1}\r\n", request,response);
			FileLogger.Write(LOG_KBN_NAME, logStr);
		}

		/// <summary>
		/// エラーログ出力
		/// </summary>
		/// <param name="ex">例外エラー</param>
		public static void WriteError(Exception ex)
		{
			FileLogger.Write(LOG_KBN_NAME, ex);
		}
	}
}
