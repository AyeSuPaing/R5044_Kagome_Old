/*
=========================================================================================================
  Module      : オペレータ操作ログ出力処理(OperationLogWriter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Text;
using w2.Common.Logger;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Web.UI.WebControls;

namespace w2.App.Common.OperationLog
{
	/// <summary>
	/// オペレータログインログ出力処理区分
	/// </summary>
	public enum OperationKbn
	{
		/// <summary>ログイン成功</summary>
		SUCCESS,
		/// <summary>ログイン失敗</summary>
		MISS,
		/// <summary>オペレータロック</summary>
		LOCK
	}

	public class OperationLogWriter
	{
		/// <summary>
		/// オペレータログインログ出力処理
		/// </summary>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="loginId">ログインID</param>
		/// <param name="ipAddress">IPアドレス</param>
		/// <param name="sessionId">セッションID</param>
		/// <param name="operatorName">オペレータ名</param>
		/// <param name="writeMsgFlg">出力メッセージフラグ</param>
		public static void WriteLoginLog(string operatorId, string loginId, string ipAddress, string sessionId, string operatorName, OperationKbn writeMsgFlg)
		{
			if (operatorId == null)
			{
				int OperatorLength = 10;	//オペレータIDの桁数はどこにも持ってないのでここで指定する。
				operatorId = "";
				operatorId = operatorId.PadLeft(OperatorLength);
			}

			StringBuilder infoOthers = new StringBuilder();
			infoOthers.Append("オペレータ: ").Append(loginId);
			switch (writeMsgFlg)
			{
				case OperationKbn.SUCCESS:
					infoOthers.Append("(").Append(operatorName).Append(")").Append(" がログインしました。");
					break;

				case OperationKbn.MISS:
					infoOthers.Append(" がログインに失敗しました。");
					break;

				case OperationKbn.LOCK:
					infoOthers.Append(" がロックされました。");
					break;
			}

			WriteLog(operatorId, ipAddress, sessionId, infoOthers.ToString());
		}

		/// <summary>
		/// オペレータ操作ログ出力処理
		/// </summary>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="ipAddress">IPアドレス</param>
		/// <param name="sessionId">セッションID</param>
		/// <param name="pageUrl">ページURL</param>
		public static void WriteOperationLog(string operatorId, string ipAddress, string sessionId, string pageUrl)
		{
			WriteLog(operatorId, ipAddress, sessionId, pageUrl);
		}

		/// <summary>
		/// オペレータ操作ログ出力処理
		/// Elasticsearch(ES)にもログを出力
		/// </summary>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="operatorName">オペレータ名</param>
		/// <param name="ipAddress">IPアドレス</param>
		/// <param name="sessionId">セッションID</param>
		/// <param name="pageUrlAddParameter">パラメータ付き操作ページURL</param>
		/// <param name="pageUrl">ページURL</param>
		/// <param name="urlParameter">Urlパラメタ</param>
		public static void WriteOperationLogAndEs(
			string operatorId,
			string operatorName,
			string ipAddress,
			string sessionId,
			string pageUrlAddParameter,
			string pageUrl,
			string urlParameter)
		{
			WriteLog(operatorId, ipAddress, sessionId, pageUrl);

			SendJsonAndWriteOnFailure(
				Constants.PLAN_NAME,
				Constants.PROJECT_NO,
				Constants.ENVIRONMENT_NAME,
				operatorId,
				operatorName,
				ipAddress,
				sessionId,
				pageUrlAddParameter,
				pageUrl,
				urlParameter);
		}

		/// <summary>
		/// オペレータログ出力Jsonファイルを送信、送信できない場合エラーをLogファイルに書き込む処理
		/// </summary>
		/// <param name="planName">プラン名</param>
		/// <param name="projectNo">プロジェクトNo</param>
		/// <param name="environmentName">環境名</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="operatorName">オペレータ名</param>
		/// <param name="ipAddress">IPアドレス</param>
		/// <param name="sessionId">セッションID</param>
		/// <param name="pageUrlAddParameter">パラメータ付き操作ページURL</param>
		/// <param name="pageUrl">ページURL</param>
		/// <param name="urlParameter">Urlパラメタ</param>
		/// <remarks>
		/// ポストバックとの兼ね合いで既存ログ出力とは別に
		/// このメソッドを使用する必要がある
		/// </remarks>
		public static void SendJsonAndWriteOnFailure(
			string planName,
			string projectNo,
			string environmentName,
			string operatorId,
			string operatorName,
			string ipAddress,
			string sessionId,
			string pageUrlAddParameter,
			string pageUrl,
			string urlParameter)
		{
			// EP案件では、該当するページがあったらその値を入れる。該当するページがないと空文字を入れる。
			var menuName = GetMenuNameCorrespondingToUrl(pageUrl);
			if (planName != Constants.PLAN_NAME_EP)
			{
				// ページの定義を追加し忘れることを防ぐために例外を送る
				if (string.IsNullOrEmpty(menuName))
				{
					throw new NotSupportedException("ログ出力するページ名が未定義のページが見つかりました. 定義を追加してください");
				}
			}

			// Logstash Api Urlが未設定 もしくは ログイン時のログならメソッドを出る
			// ログイン時のオペレーションログはESでは管理しない
			// ページ定義追加忘れ防止のため、この処理の後に判定する
			if (string.IsNullOrEmpty(Constants.LOGSTASH_API_URL) || menuName.Contains("ログイン"))
			{
				return;
			}

			Dictionary<string, string> output = new Dictionary<string, string>();
			output.Add("PlanName", planName);
			output.Add("ProjectNo", projectNo);
			output.Add("DateTime", DateTime.Now.ToString("yyyyMMddHHmmss+0900")); //kibanaに入っているデータがUTCとして扱われ９時間プラスされていただけなのでformatを "yyyyMMddHHmmssZ"とし入れるデータを "20220826154607+0900" とすることで解決。
			output.Add("EnvironmentName", environmentName);
			output.Add("OperatorId", operatorId);
			output.Add("OperatorName", operatorName);
			output.Add("IpAddress", ipAddress);
			output.Add("SessionId", sessionId);
			output.Add("PageUrlAddParameter", pageUrlAddParameter);
			output.Add("PageUrl", pageUrl);
			output.Add("MenuName", menuName);
			output.Add("UrlParameter", urlParameter);

			var jsonString = JsonConvert.SerializeObject(output, Formatting.None);
			try
			{
				var sendJsonToApi = new SendJsonToApi(Constants.LOGSTASH_API_URL, jsonString);
				sendJsonToApi.PostHttp();
			}
			catch (Exception e)
			{
				if (Directory.Exists(Constants.PHYSICALDIRPATH_OPERATION_NOTSEND_LOGFILE) == false)
				{
					Directory.CreateDirectory(Constants.PHYSICALDIRPATH_OPERATION_NOTSEND_LOGFILE);
				}
				FileLogger.Write(Constants.LOGFILE_NAME_OPERATIONLOG_NOT_SEND, jsonString, Constants.PHYSICALDIRPATH_OPERATION_NOTSEND_LOGFILE, false);
			}
		}

		/// <summary>
		/// オペレータログ出力処理
		/// </summary>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="ipAddress">IPアドレス</param>
		/// <param name="sessionId">セッションID</param>
		/// <param name="infoOthers">その他情報</param>
		private static void WriteLog(string operatorId, string ipAddress, string sessionId, string infoOthers)
		{
			StringBuilder logText = new StringBuilder();
			logText.Append("OperatorId:").Append(operatorId);
			logText.Append(" ipAddress:").Append(ipAddress);
			logText.Append(" SessionId:").Append(sessionId).Append(" ");
			logText.Append(infoOthers);

			if (Directory.Exists(Constants.PHYSICALDIRPATH_OPERATION_LOGFILE) == false)
			{
				Directory.CreateDirectory(Constants.PHYSICALDIRPATH_OPERATION_LOGFILE);
			}

			FileLogger.Write("operation", logText.ToString(), Constants.PHYSICALDIRPATH_OPERATION_LOGFILE, true);
		}

		/// <summary>
		/// URLに対応するメニュー名を取得。存在しない場合空文字。
		/// </summary>
		/// <param name="pageUrl">URL</param>
		/// <returns>Constants_Pagesに記載のメニュー名</returns>
		private static string GetMenuNameCorrespondingToUrl(string pageUrl)
		{
			var result = "";
			if (pageUrl.Contains(Constants.PATH_ROOT_EC))
			{
				result = Constants.LIST_MENU_NAME_CHANGE_FROM_URL_EC.Any(menu => pageUrl.Contains(menu.Key))
					? Constants.LIST_MENU_NAME_CHANGE_FROM_URL_EC.First(menu => pageUrl.Contains(menu.Key)).Value
					: "";
				if (string.IsNullOrEmpty(result) == false)
				{
					return result;
				}
			}
			if (pageUrl.Contains(Constants.PATH_ROOT_MP))
			{
				result = Constants.LIST_MENU_NAME_CHANGE_FROM_URL_MP.Any(menu => pageUrl.Contains(menu.Key))
					? Constants.LIST_MENU_NAME_CHANGE_FROM_URL_MP.First(menu => pageUrl.Contains(menu.Key)).Value
					: "";
				if (string.IsNullOrEmpty(result) == false)
				{
					return result;
				}
			}
			if (pageUrl.Contains(Constants.PATH_ROOT_CS))
			{
				result = Constants.LIST_MENU_NAME_CHANGE_FROM_URL_CS.Any(menu => pageUrl.Contains(menu.Key))
					? Constants.LIST_MENU_NAME_CHANGE_FROM_URL_CS.First(menu => pageUrl.Contains(menu.Key)).Value
					: "";
				if (string.IsNullOrEmpty(result) == false)
				{
					return result;
				}
			}
			if (pageUrl.Contains(Constants.PATH_ROOT_CMS))
			{
				result = Constants.LIST_MENU_NAME_CHANGE_FROM_URL_CMS.Any(menu => pageUrl.Contains(menu.Key))
					? Constants.LIST_MENU_NAME_CHANGE_FROM_URL_CMS.First(menu => pageUrl.Contains(menu.Key)).Value
					: "";
				if (string.IsNullOrEmpty(result) == false)
				{
					return result;
				}
			}
			return result;
		}
	}
}
