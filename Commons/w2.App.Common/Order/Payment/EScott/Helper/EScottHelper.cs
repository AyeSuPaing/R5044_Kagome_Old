/*
=========================================================================================================
  Module      : e-SCOTTヘルパークラス(EScottHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;

namespace w2.App.Common.Order.Payment.EScott.Helper
{
	/// <summary>
	/// e-SCOTTヘルパークラス
	/// </summary>
	internal static class EScottHelper
	{
		/// <summary>
		/// 値もしくは空を取得
		/// </summary>
		/// <param name="dic">辞書</param>
		/// <param name="key">キー</param>
		/// <returns>結果</returns>
		public static string GetValueOrEmpty(this Dictionary<string, string> dic, string key)
		{
			var result = dic.ContainsKey(key) ? dic[key] : string.Empty;
			return result;
		}

		/// <summary>
		/// プロセスID取得
		/// </summary>
		/// <param name="transactionId">トランザクションID</param>
		/// <returns>プロセスID</returns>
		public static string GetProcessId(string transactionId)
		{
			var transactionIdArray = transactionId.Split(',');
			var result = transactionIdArray.Length >= 2 ? transactionId.Split(',')[1] : string.Empty;
			return result;
		}

		/// <summary>
		/// プロセスパスワード取得
		/// </summary>
		/// <param name="transactionId">トランザクションID</param>
		/// <returns>プロセスパスワード</returns>
		public static string GetProcessPass(string transactionId)
		{
			var transactionIdArray = transactionId.Split(',');
			var result = transactionIdArray.Length >= 3 ? transactionId.Split(',')[2] : string.Empty;
			return result;
		}

		/// <summary>
		/// 会員ID取得
		/// </summary>
		/// <param name="transactionId">トランザクションID</param>
		/// <returns>会員ID</returns>
		public static string GetKaiinId(string transactionId)
		{
			var transactionIdArray = transactionId.Split(',');
			var result = transactionIdArray.Length >= 1 ? transactionId.Split(',')[0] : string.Empty;
			return result;
		}

		/// <summary>
		/// transactionIdで会員パスワード取得
		/// </summary>
		/// <param name="transactionId">transactionId</param>
		/// <returns>会員パスワード</returns>
		public static string GetKaiinPassFromTransactionId(string transactionId)
		{
			var transactionIdArray = transactionId.Split(',');
			var result = transactionIdArray.Length >= 4
				? transactionId.Split(',')[3]
				: Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MEMBER_PASSWORD;
			return result;
		}

		/// <summary>
		/// cooperationIdで会員パスワード取得
		/// </summary>
		/// <param name="cooperationId">cooperationId</param>
		/// <returns>会員パスワード</returns>
		public static string GetKaiinPassFromCooperationId(string cooperationId)
		{
			var cooperationIdArray = cooperationId.Split(',');
			var result = cooperationIdArray.Length >= 2
				? cooperationId.Split(',')[1]
				: Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MEMBER_PASSWORD;
			return result;
		}

		/// <summary>
		/// トランザクション日取得
		/// </summary>
		/// <returns>トランザクション日</returns>
		public static string GetTransactionDate()
		{
			var result = ToEScottDate(DateTime.Now);
			return result;
		}

		/// <summary>
		/// e-SCOTT用日付生成
		/// </summary>
		/// <param name="time">時間</param>
		/// <returns>e-SCOTT用日付</returns>
		public static string ToEScottDate(DateTime time)
		{
			var result = time.ToString("yyyyMMdd");
			return result;
		}

		/// <summary>
		/// 支払回数に変換
		/// </summary>
		/// <param name="creditInstallmentsCode">支払回数コード</param>
		/// <returns>支払回数</returns>
		public static string ToPayType(string creditInstallmentsCode)
		{
			if (string.IsNullOrEmpty(creditInstallmentsCode)) return "01";

			var result = creditInstallmentsCode.PadLeft(2, '0');
			return result;
		}

		/// <summary>
		/// e-SCOTT用の金額に変更
		/// </summary>
		/// <param name="price">金額</param>
		/// <returns>金額</returns>
		public static string ToAmount(decimal price)
		{
			var result = decimal.ToInt32(price).ToString();
			return result;
		}

		/// <summary>
		/// レスポンスの分割
		/// </summary>
		/// <param name="responseText">レスポンス文</param>
		/// <returns>レスポンスの辞書</returns>
		public static Dictionary<string, string> SplitResponse(string responseText)
		{
			var responseParameterConnectedByEqual = responseText.Split('&');

			var responseParamterList = new Dictionary<string, string>();
			foreach (var text in responseParameterConnectedByEqual)
			{
				var keyAndValue = text.Split('=');
				responseParamterList.Add(keyAndValue[0], keyAndValue[1]);
			}
			return responseParamterList;
		}
	}
}
