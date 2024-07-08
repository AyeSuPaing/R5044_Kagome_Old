/*
=========================================================================================================
  Module      : ログ作成クラス。ログファイルに書き込む文字生成にも利用(LogCreator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
 Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;

namespace w2.App.Common.Order.Payment
{
	/// <summary>
	/// ログ作成クラス
	/// </summary>
	public class LogCreator
	{
		/// <summary>
		/// ログ文字列作成(注文ID、決済注文IDあり。主に成功時に使う。)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="memberId">会員ID</param>
		/// <returns>ログ文字列</returns>
		public static string CreateMessage(string orderId, string paymentOrderId, string memberId = "")
		{
			var idMessageList = new List<string>();

			AddKeyAndValueColonList(Constants.FIELD_FIXEDPURCHASEHISTORY_ORDER_ID, orderId, idMessageList);
			AddKeyAndValueColonList(Constants.FIELD_ORDER_PAYMENT_ORDER_ID, paymentOrderId, idMessageList);
			AddKeyAndValueColonList("member_id", memberId, idMessageList);

			var forLogStr = ConvertToSplitTabStr(idMessageList);
			return forLogStr;
		}

		/// <summary>
		/// ログ用のエラーメッセージを作成
		/// </summary>
		/// <param name="errorCode">エラーコード</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <param name="messageForLog">エラーコードやエラーメッセージの前に入れたいメッセージ。処理名などを入れたりする。</param>
		/// <param name="url">エラーが起こったURL</param>
		/// <returns>ログ用のエラーメッセージ</returns>
		public static string CreateErrorMessage(string errorCode, string errorMessage, string messageForLog = "", string url = "")
		{
			var idMessageList = new List<string>();

			AddKeyAndValueColonList("message", messageForLog, idMessageList);
			AddKeyAndValueColonList("error_code", errorCode, idMessageList);
			AddKeyAndValueColonList("errorMessage", errorMessage, idMessageList);
			AddKeyAndValueColonList("url", url, idMessageList);

			var forLogStr = ConvertToSplitTabStr(idMessageList);
			return forLogStr;
		}

		/// <summary>
		///	ログ文字列作成(注文ID,決済注文ID,支払区分利用)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="paymentId">支払区分</param>
		/// <returns>ログ文字列</returns>
		public static string CreateMessageWithPaymentId(string orderId, string paymentOrderId, string paymentId)
		{
			var idMessageList = new List<string>();

			AddKeyAndValueColonList(Constants.FIELD_FIXEDPURCHASEHISTORY_ORDER_ID, orderId, idMessageList);
			AddKeyAndValueColonList(Constants.FIELD_ORDER_PAYMENT_ORDER_ID, paymentOrderId, idMessageList);
			AddKeyAndValueColonList(Constants.FIELD_FIXEDPURCHASE_ORDER_PAYMENT_KBN, paymentId, idMessageList);

			var forLogStr = ConvertToSplitTabStr(idMessageList);
			return forLogStr;
		}

		/// <summary>
		///	ログ文字列作成(決済取引ID,決済注文ID,最終請求金額,連携種別)
		/// </summary>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="lastBilledAmount">最終請求金額</param>
		/// <param name="cooperationType">連携種別</param>
		/// <returns>ログ文字列</returns>
		public static string CreateMessageWithPaymentId(
			string cardTranId,
			string paymentOrderId,
			string lastBilledAmount,
			string cooperationType)
		{
			var logMessage = string.Format(
				"決済取引ID:{0}・決済注文ID:{1}・{2}円 {3}",
				cardTranId,
				paymentOrderId,
				lastBilledAmount,
				cooperationType);

			return logMessage;
		}

		/// <summary>
		///ログ文字列作成(決済カード取引ID,決済注文ID)
		/// </summary>
		/// <param name="cardTranId">決済カード取引ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <returns>ログ文字列</returns>
		public static string CrateMessageWithCardTranId(string cardTranId, string paymentOrderId)
		{
			var idMessageList = new List<string>();

			AddKeyAndValueColonList(Constants.FIELD_ORDER_CARD_TRAN_ID, cardTranId, idMessageList);
			AddKeyAndValueColonList(Constants.FIELD_ORDER_PAYMENT_ORDER_ID, paymentOrderId, idMessageList);

			var forLogStr = ConvertToSplitTabStr(idMessageList);
			return forLogStr;
		}

		/// <summary>
		/// ログ文字列作成(カード支払い回数コード、ゼウス向けユニークID)
		/// </summary>
		/// <param name="cardInstallmentsCode">"カード支払い回数コード</param>
		/// <param name="zeusSendId">ゼウス向けユニークID</param>
		/// <returns>ログ文字列</returns>
		public static string CreateMessageWithZeus(string cardInstallmentsCode, string zeusSendId)
		{
			var idMessageList = new List<string>();

			AddKeyAndValueColonList(Constants.FIELD_FIXEDPURCHASE_CARD_INSTALLMENTS_CODE, cardInstallmentsCode, idMessageList);
			AddKeyAndValueColonList("for_zeusu_unique_id", zeusSendId, idMessageList);

			var forLogStr = ConvertToSplitTabStr(idMessageList);
			return forLogStr;
		}

		/// <summary>
		///ログ文字列作成(注文ID,処理トラッキングID)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="resTrackingId">処理トラッキングID</param>
		/// <returns>ログ文字列</returns>
		public static string CreateMessageWithTrackingId(string orderId, string resTrackingId)
		{
			var idMessageList = new List<string>();
			AddKeyAndValueColonList(Constants.FIELD_FIXEDPURCHASEHISTORY_ORDER_ID, orderId, idMessageList);
			AddKeyAndValueColonList("res_tracking_id", resTrackingId, idMessageList);

			var forLogStr = ConvertToSplitTabStr(idMessageList);
			return forLogStr;
		}

		/// <summary>
		/// ログ文字列作成(配送伝票番号、決済カード取引ID、支払区分)
		/// </summary>
		/// <param name="shippingCheckNo">配送伝票番号</param>
		/// <param name="cardTranId">決済カード取引ID</param>
		/// <param name="orderPaymentKbn">支払区分</param>
		/// <returns>ログ文字列</returns>
		public static string CreateMessageWithShippingCheckNo(
			string shippingCheckNo,
			string cardTranId,
			string orderPaymentKbn)
		{
			var idMessageList = new List<string>();

			AddKeyAndValueColonList(Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO, shippingCheckNo, idMessageList);
			AddKeyAndValueColonList(Constants.FIELD_ORDER_CARD_TRAN_ID, cardTranId, idMessageList);
			AddKeyAndValueColonList(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, orderPaymentKbn, idMessageList);

			var forLogStr = ConvertToSplitTabStr(idMessageList);
			return forLogStr;
		}

		/// <summary>
		/// ログ文字列作成(決済注文ID,配送伝票番号)
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="shippingCheckNo">配送伝票番号</param>
		/// <returns>ログ文字列</returns>
		public static string CreateMessageWithOrderIdAndShippingCheckNo(string paymentOrderId, string shippingCheckNo)
		{
			var idMessageList = new List<string>();

			AddKeyAndValueColonList(Constants.FIELD_ORDER_PAYMENT_ORDER_ID, paymentOrderId, idMessageList);
			AddKeyAndValueColonList(Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO, shippingCheckNo, idMessageList);

			var forLogStr = ConvertToSplitTabStr(idMessageList);
			return forLogStr;
		}

		/// <summary>
		/// ログ文字列作成(トランザクションID、処理トラッキングID)
		/// </summary>
		/// <param name="transactionId">トランザクションID</param>
		/// <param name="trackingId">トラッキングID</param>
		/// <returns>ログ文字列</returns>
		public static string CreateMessageWithTransactionId(string transactionId, string trackingId)
		{
			var idMessageList = new List<string>();

			AddKeyAndValueColonList("tran_id", transactionId, idMessageList);
			AddKeyAndValueColonList("res_tracking_id", trackingId, idMessageList);

			var forLogStr = ConvertToSplitTabStr(idMessageList);
			return forLogStr;
		}

		/// <summary>
		/// ログ文字列作成(gmoTransactionId)
		/// </summary>
		/// <param name="gmoTransactionId"></param>
		/// <returns></returns>
		public static string CreateMessageWithGmoTransactionId(string gmoTransactionId)
		{
			var idMessageList = new List<string>();
			AddKeyAndValueColonList("gmoTransactionId", gmoTransactionId, idMessageList);

			var forLogStr = ConvertToSplitTabStr(idMessageList);
			return forLogStr;
		}

		/// <summary>
		/// ログ文字列作成(トランザクションID、エラーコード、エラーメッセージ)
		/// </summary>
		/// <param name="transactionId">トランザクションID</param>
		/// /// <param name="errorCode">エラーコード</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns>ログ文字列</returns>
		public static string CreateErrorMessageWithTransactionId(string transactionId, string errorCode, string errorMessage)
		{
			var idMessageList = new List<string>();

			AddKeyAndValueColonList("tran_id", transactionId, idMessageList);
			AddKeyAndValueColonList("error_code", errorCode, idMessageList);
			AddKeyAndValueColonList("error_message", errorMessage, idMessageList);

			var forLogStr = ConvertToSplitTabStr(idMessageList);
			return forLogStr;
		}

		/// <summary>
		/// ログ文字列作成(リクエスト型、リクエスト値、メッセージ、例外メッセージ)
		/// </summary>
		/// <param name="requestType">リクエスト型</param>
		/// /// <param name="requestValue">リクエスト値</param>
		/// <param name="messageForLog">どんなリクエストなのか、なんのリクエストなのかなどのメッセージを入れる</param>
		/// <param name="errorMessage">例外メッセージ</param>
		/// <returns>ログ文字列</returns>
		public static string CreateRequestMessage(string requestType, string requestValue, string messageForLog = "", string errorMessage = "")
		{
			var idMessageList = new List<string>();

			AddKeyAndValueColonList(null, messageForLog, idMessageList);
			AddKeyAndValueColonList("request_type", requestType, idMessageList);
			AddKeyAndValueColonList("request_value", requestValue, idMessageList);
			AddKeyAndValueColonList("exception_message", errorMessage, idMessageList);

			var forLogStr = ConvertToSplitTabStr(idMessageList);
			return forLogStr;
		}

		/// <summary>
		/// ログ文字列作成(リクエスト型、リクエスト値、メッセージ、結果)
		/// </summary>
		/// <param name="requestType">リクエスト型</param>
		/// /// <param name="requestValue">リクエスト値</param>
		/// <param name="messageForLog">メッセージ</param>
		/// <param name="result">結果</param>
		/// <returns>ログ文字列</returns>
		public static string CreateRequestMessageWithResult(string requestType, string requestValue, string messageForLog = "", string result = "")
		{
			var idMessageList = new List<string>();

			AddKeyAndValueColonList(null, messageForLog, idMessageList);
			AddKeyAndValueColonList("request_type", requestType, idMessageList);
			AddKeyAndValueColonList("request_value", requestValue, idMessageList);
			AddKeyAndValueColonList("result", result, idMessageList);

			var forLogStr = ConvertToSplitTabStr(idMessageList);
			return forLogStr;
		}

		/// <summary>
		/// ログ文字列作成(リクエスト型、リクエスト値、メッセージ、結果)
		/// </summary>
		/// <param name="requestType">リクエスト型</param>
		/// /// <param name="requestValue">リクエスト値</param>
		/// <param name="messageForLog">メッセージ</param>
		/// <param name="url">URL</param>
		/// <returns>ログ文字列</returns>
		public static string CreateRequestMessageWithUrl(string requestType, string requestValue, string messageForLog = "", string url = "")
		{
			var idMessageList = new List<string>();

			AddKeyAndValueColonList(null, messageForLog, idMessageList);
			AddKeyAndValueColonList("request_type", requestType, idMessageList);
			AddKeyAndValueColonList("request_value", requestValue, idMessageList);
			AddKeyAndValueColonList("URL", url, idMessageList);

			var forLogStr = ConvertToSplitTabStr(idMessageList);
			return forLogStr;
		}

		/// <summary>
		/// ログ文字列作成(userIdを利用)
		/// </summary>
		/// <param name="userId">userId</param>
		/// <returns>ログ文字列</returns>
		public static string CreateWithUserId(string userId)
		{
			if (string.IsNullOrEmpty(userId)) return "";

			var forLogStr = string.Format("{0}：{1}", Constants.FIELD_ORDER_USER_ID, userId);
			return forLogStr;
		}

		/// <summary>
		/// ログ文字列作成(送信日時、order_id利用)
		/// </summary>
		/// <param name="requestDate">送信日時</param>
		/// <param name="orderId">受注番号</param>
		/// <returns>ログ文字列</returns>
		public static string CreateMessageWithRequestData(string requestDate, string orderId)
		{
			var idMessageList = new List<string>();

			AddKeyAndValueColonList("request_date", requestDate, idMessageList);
			AddKeyAndValueColonList(Constants.FIELD_FIXEDPURCHASEHISTORY_ORDER_ID, orderId, idMessageList);
			var forLogStr = ConvertToSplitTabStr(idMessageList);
			return forLogStr;
		}

		/// <summary>
		/// ログ用のエラーメッセージ作成(送信日時、エラーコード、エラーメッセージ利用)
		/// </summary>
		/// <param name="requestDate">送信日時</param>
		/// <param name="errorCode">エラーコード</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns></returns>
		public static string CreateErrorMessageWithRequestData(string requestDate, string errorCode, string errorMessage)
		{
			var requestDateStr = string.Format("request_date：{0}", requestDate);
			var log = requestDateStr + CreateErrorMessage(errorCode, errorMessage);

			var forLogStr = string.IsNullOrEmpty(log) == false ? log : "";
			return forLogStr;
		}

		/// <summary>
		/// id : valueのような文字列をリストに追加。
		/// </summary>
		/// <param name="idKey">idのkey</param>
		/// <param name="idValue">idのvalue。valueがないときはnullを入れるとidKeだけの文字列が追加される</param>
		/// <param name="keyAndValueColonList">id : keyという文字列を保持するリスト</param>
		private static void AddKeyAndValueColonList(string idKey, string idValue, List<string> keyAndValueColonList)
		{
			if (keyAndValueColonList == null) keyAndValueColonList = new List<string>();

			// idKeyが空またはnull かつ idValueがnullである場合
			if (string.IsNullOrEmpty(idKey) && string.IsNullOrEmpty(idValue)) return;

			// idKeyとidValueどちらも値入ってくるとき
			if ((string.IsNullOrEmpty(idKey) == false) && (string.IsNullOrEmpty(idValue) == false))
			{
				keyAndValueColonList.Add(string.Format("{0}：{1}", idKey, idValue));
				return;
			}

			// idValueだけ値が入っていた場合
			if ((string.IsNullOrEmpty(idKey)) && string.IsNullOrEmpty(idValue) == false)
			{
				keyAndValueColonList.Add(string.Format("{0}", idValue));
			}
		}

		/// <summary>
		/// 配列からタブ(\t)区切りの文字列へ変換
		/// </summary>
		/// <param name="idMessageList">idKey :　idValueのような文字列が入っているリスト</param>
		/// <returns>タブ(\t)区切りの文字列</returns>
		private static string ConvertToSplitTabStr(List<string> idMessageList)
		{
			var idMessageArray = idMessageList.ToArray();
			var result = idMessageList.ToArray().Any() ? string.Join("\t", idMessageArray) : "";

			return result;
		}
	}
}
