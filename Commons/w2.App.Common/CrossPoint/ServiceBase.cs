/*
=========================================================================================================
  Module      : Service Base (ServiceBase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Extensions;
using w2.Common.Logger;

namespace w2.App.Common.CrossPoint
{
	/// <summary>
	/// サービス基底クラス
	/// </summary>
	public class ServiceBase
	{
		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="name">接続API名</param>
		protected static void WriteStartLog(string name)
		{
			if (Constants.CROSS_POINT_API_WRITE_LOG == false) return;

			FileLogger.Write(
				Constants.CROSS_POINT_API_LOG_TYPE,
				string.Format(
					"{0} {1}",
					"[開始]",
					name));
		}

		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="name">接続API名</param>
		/// <param name="param">リクエストパラメータ</param>
		protected static void WriteStartLog(string name, Dictionary<string, string> param)
		{
			if (Constants.CROSS_POINT_API_WRITE_LOG == false) return;

			var requestParameter = param
				.Select(item => string.Format(
					"{0}={1}",
					item.Key,
					item.Value))
				.JoinToString("&");

			var request = string.Format(
				"{0}{1}?{2}",
				Constants.CROSS_POINT_API_URL_ROOT_PATH,
				name,
				requestParameter);

			var message = new StringBuilder()
				.AppendLine()
				.AppendLine("[ログ]CROSSPOINT APIリクエスト前")
				.AppendLine("メッセージ:CROSSPOINT　APIリクエスト開始")
				.AppendFormat("request_type：{0}", name)
				.AppendLine()
				.AppendFormat("request_value：{0}", request)
				.AppendLine()
				.ToString();

			FileLogger.Write(Constants.CROSS_POINT_API_LOG_TYPE, message);
		}

		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="name">接続API名</param>
		/// <param name="status">実行結果ステータス</param>
		/// <param name="xmlResponse">Xml response</param>
		protected static void WriteEndLog(
			string name,
			ResultStatus status,
			string xmlResponse)
		{
			if (Constants.CROSS_POINT_API_WRITE_LOG == false) return;

			var requestParameter = status
				.RequestParameter
				.Select(item => string.Format(
					"{0}={1}",
					item.Key,
					item.Value))
				.JoinToString("&");

			var request = string.Format(
				"{0}{1}?{2}",
				Constants.CROSS_POINT_API_URL_ROOT_PATH,
				name,
				requestParameter);

			var message = new StringBuilder()
				.AppendLine()
				.AppendLine("[ログ]CROSSPOINT APIリクエスト後")
				.AppendLine("メッセージ:CROSSPOINT　APIリクエスト終了")
				.AppendFormat("request_type：{0}", name)
				.AppendLine()
				.AppendFormat("request_value：{0}", request)
				.AppendLine()
				.AppendFormat("result：{0}", xmlResponse)
				.AppendLine()
				.ToString();

			FileLogger.Write(Constants.CROSS_POINT_API_LOG_TYPE, message);
		}
	}
}
