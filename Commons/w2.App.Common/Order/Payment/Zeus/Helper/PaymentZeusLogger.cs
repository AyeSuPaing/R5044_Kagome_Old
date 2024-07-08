/*
=========================================================================================================
  Module      : ゼウス決済ロガー(PaymentZeusLogger.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;

namespace w2.App.Common.Order.Payment.Zeus.Helper
{
	/// <summary>
	/// ゼウス決済ロガー
	/// </summary>
	internal class PaymentZeusLogger
	{
		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="kbn">区分（決済種別IDなど）</param>
		/// <param name="processingContent">処理内容</param>
		/// <param name="result">結果（なければnull）</param>
		/// <param name="infos">情報</param>
		internal static void WriteLog(string kbn, PaymentFileLogger.PaymentProcessingType processingContent, bool? result, params KeyValuePair<string, string>[] infos)
		{
			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				result,
				kbn,
				PaymentFileLogger.PaymentType.Zeus,
				processingContent,
				string.Join("\t", infos.Select(info => string.Format("{0}:{1}", info.Key, info.Value))));
		}
	}
}
