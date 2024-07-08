/*
=========================================================================================================
  Module      : ヤマトKWCロガー(PaymentYamatoKwcLogger.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;

namespace w2.App.Common.Order.Payment.YamatoKwc.Helper
{
	/// <summary>
	/// ヤマトKWCロガー
	/// </summary>
	internal class PaymentYamatoKwcLogger
	{
		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="kbn">区分（決済の種類）</param>
		/// <param name="processingContent">処理内容</param>
		/// <param name="result">結果（なければnull）</param>
		/// <param name="infos">情報</param>
		internal static void WriteLog(string kbn, PaymentFileLogger.PaymentProcessingType processingContent, bool? result, params KeyValuePair<string, string>[] infos)
		{
			PaymentFileLogger.WritePaymentLog(
				result,
				kbn,
				PaymentFileLogger.PaymentType.Yamatokwc,
				processingContent,
				string.Join("\t", infos.Select(info => string.Format("{0}：{1}", info.Key, info.Value))));
		}
	}
}
