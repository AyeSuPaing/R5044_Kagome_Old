/*
=========================================================================================================
  Module      : RecustomerApi Logger(RecustomerApiLogger.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Recustomer.OrderImporter;
using w2.App.Common.Util;
using w2.Common.Sql;
using w2.Common.Wrapper;
using w2.Domain;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Recustomer
{
	/// <summary>
	/// RecustomerApiLogger
	/// </summary>
	public class RecustomerApiLogger : ApiLoggerBase
	{
		/// <summary>連携名：Recustomer</summary>
		private const string CONST_RECUSTOMER_API_NAME = "Recustomer";

		/// <summary>
		/// エラー区分
		/// </summary>
		public enum ErrorKbn
		{
			/// <summary>ギフト注文</summary>
			Gift,
			/// <summary>デジタルコンテンツ注文</summary>
			DigitalContents,
			/// <summary>未出荷注文</summary>
			UnShipped,
		}

		/// <summary>
		/// Recustomerリクエストログ出力
		/// </summary>
		/// <param name="request">リクエスト</param>
		public static void WriteRecustomerRequestApiLog(string request)
		{
			if (Constants.RECUSTOMER_API_REQUEST_LOG_EXPORT_ENABLED == false) return;

			var messageLine = string.Format(
				"[{0}] {1:yyyy年MM月dd日HH:mm:ss} {2}",
				CONST_RECUSTOMER_API_NAME,
				DateTimeWrapper.Instance.Now,
				(string.IsNullOrEmpty(request) ? string.Empty : request.Replace("\r\n", "\t")));
			Write(
				messageLine,
				Constants.PHYSICALDIRPATH_LOGFILE,
				string.Format("{0}_{1}", CONST_RECUSTOMER_API_NAME, DateTimeWrapper.Instance.Now.ToString(Constants.DATE_FORMAT_SHORT)),
				Constants.RECUSTOMER_API_LOGFILE_THRESHOLD);
		}

		/// <summary>
		/// Recustomerレスポンスログ出力
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="result">連携結果</param>
		/// <param name="response">レスポンス</param>
		/// <param name="accessor">アクセサ</param>
		public static string WriteRecustomerResponseApiLog(OrderModel order, string lastChanged, bool result, OrderImporterResponse response, SqlAccessor accessor)
		{
			var messageLine = string.Format(
				"[{0}] {1:yyyy年MM月dd日HH:mm:ss} {2}: {3}",
				CONST_RECUSTOMER_API_NAME,
				DateTimeWrapper.Instance.Now,
				result ? "成功" : "失敗",
				string.Format("{{ \"{0}\": \"{1}\" }}",result ? "results" : "message", result ? response.Results : response.Message));
			Write(
				messageLine,
				Constants.PHYSICALDIRPATH_LOGFILE,
				string.Format("{0}_{1}", CONST_RECUSTOMER_API_NAME, DateTimeWrapper.Instance.Now.ToString(Constants.DATE_FORMAT_SHORT)),
				Constants.RECUSTOMER_API_LOGFILE_THRESHOLD);

			DomainFacade.Instance.OrderService.AppendRelationMemo(
				order.OrderId,
				((string.IsNullOrEmpty(order.RelationMemo) ? string.Empty : "\r\n") + messageLine),
				lastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			return messageLine;
		}

		/// <summary>
		/// エラー用外部連携メモ作成
		/// </summary>
		/// <param name="errorKbn">エラー区分</param>
		/// <returns>外部連携メモ</returns>
		public static string CreateRelationMemoForError(ErrorKbn errorKbn)
		{
			var errorMessage = string.Empty;

			switch (errorKbn)
			{
				case ErrorKbn.Gift:
				case ErrorKbn.DigitalContents:
					errorMessage = string.Format("[{0}] {1:yyyy年MM月dd日HH:mm:ss} {2}注文のため、注文情報の更新に失敗しました。",
						CONST_RECUSTOMER_API_NAME,
						DateTimeWrapper.Instance.Now,
						(errorKbn == ErrorKbn.Gift) ? "ギフト" : "デジタルコンテンツ");
				break;

				case ErrorKbn.UnShipped:
					errorMessage = string.Format("[{0}] {1:yyyy年MM月dd日HH:mm:ss} 注文ステータスが出荷完了ではないため、注文情報の更新に失敗しました。",
						CONST_RECUSTOMER_API_NAME,
						DateTimeWrapper.Instance.Now);
				break;
			}

			return errorMessage;
		}
	}
}
