/*
=========================================================================================================
  Module      : 楽天IDConnectログ出力クラス(RakutenIDConnectLogger.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using w2.App.Common.Order.Payment;

namespace w2.App.Common.Auth.RakutenIDConnect
{
	/// <summary>
	/// 楽天IDConnectログ出力クラス
	/// </summary>
	public class RakutenIDConnectLogger
	{
		#region メソッド
		/// <summary>
		/// デバッグログ
		/// </summary>
		/// <param name="debug">モック処理種類</param>
		/// <param name="description">デバッグに関する追加情報</param>
		/// <param name="paymentProcessingType">決済処理名</param>
		/// <param name="idForLogDictionary">ログにしたいキーとバリューのディクショナリ</param>
		public static void WriteDebugLog(string debug, string description, PaymentFileLogger.PaymentProcessingType paymentProcessingType, Dictionary<string, string> idForLogDictionary = null)
		{
			if (Constants.RAKUTEN_ID_CONNECT_OUTPUT_DEBUGLOG == false) return;

			var messages = string.Format("debug:{0},description:{1}", debug, description);
			WriteLog(null, messages, paymentProcessingType, idForLogDictionary);
		}

		/// <summary>
		/// エラーログ処理
		/// </summary>
		/// <param name="error">エラーコード</param>
		/// <param name="description">エラーに関する追加情報</param>
		/// <param name="paymentProcessingType">決済処理名</param>
		/// <param name="idForLogDictionary">ログにしたいキーとバリューのディクショナリ</param>
		public static void WriteErrorLog(string error, string description, PaymentFileLogger.PaymentProcessingType paymentProcessingType, Dictionary<string, string> idForLogDictionary = null)
		{
			var messages = LogCreator.CreateErrorMessage(error, description);
			WriteLog(false, messages, paymentProcessingType, idForLogDictionary);
		}

		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="isSuccess">成功時のログかどうか</param>
		/// <param name="messages">メッセージ</param>
		/// <param name="paymentProcessingType">決済処理名</param>
		/// <param name="idForLogDictionary">ログにしたいキーとバリューのディクショナリ</param>
		private static void WriteLog(bool? isSuccess, string messages, PaymentFileLogger.PaymentProcessingType paymentProcessingType, Dictionary<string, string> idForLogDictionary = null)
		{
			PaymentFileLogger.WritePaymentLog(
				isSuccess,
				Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS,
				PaymentFileLogger.PaymentType.RakutenId,
				paymentProcessingType,
				messages,
				idForLogDictionary);
		}

		#endregion
	}
}