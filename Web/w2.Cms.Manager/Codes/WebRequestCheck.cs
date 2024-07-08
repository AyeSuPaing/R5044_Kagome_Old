/*
=========================================================================================================
  Module      : Webリクエストチェック(WebRequestCheck.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using w2.Cms.Manager.Codes.Common;
using w2.Common.Logger;

namespace w2.Cms.Manager.Codes
{
	/// <summary>
	/// Webリクエストチェック
	/// </summary>
	public static class WebRequestCheck
	{
		/// <summary>読み取り終了文字</summary>
		private const string ERROR_CAPTION_DETAIL_COMPILER_OUTPUT = "詳しいコンパイラ出力を表示:";
		/// <summary>エラー情報読み取り開始タグ</summary>
		private const string ERROR_CAPTION_TAG = "<h2> <i>";
		/// <summary>コード開始タグ</summary>
		private const string ERROR_CODE_START_TAG = "<code>";
		/// <summary>コード終了タグ</summary>
		private const string ERROR_CODE_END_TAG = "</code>";
		/// <summary>整形済みテキスト開始タグ</summary>
		private const string ERROR_PREFORMATTED_TEXT_START_TAG = "<pre>";
		/// <summary>整形済みテキスト終了タグ</summary>
		private const string ERROR_PREFORMATTED_TEXT_END_TAG = "</pre>";

		/// <summary>
		/// Webリクエスト送信
		/// </summary>
		/// <param name="webUrl">リクエストURL</param>
		/// <returns>送信結果</returns>
		public static string Send(string webUrl)
		{
			return Send(
				webUrl,
				"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; "
				+ ".NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; MDDS; InfoPath.2; OfficeLiveConnector.1.5; OfficeLivePatch.1.3; .NET4.0C; .NET4.0E)");
		}
		/// <summary>
		/// Webリクエスト送信
		/// </summary>
		/// <param name="webUrl">リクエストURL</param>
		/// <param name="userAgent">ユーザエージェント</param>
		/// <returns>送信結果</returns>
		private static string Send(string webUrl, string userAgent)
		{
			var webRequest = (HttpWebRequest)WebRequest.Create(webUrl);

			// 文字エンコードの指定
			var authorizationEncoding = Encoding.GetEncoding("UTF-8");
			// 要求の事前認証を行うよう設定
			webRequest.PreAuthenticate = true;
			// ユーザーエージェントの設定
			webRequest.UserAgent = userAgent;
			var basicUserAccount = (string.IsNullOrEmpty(Constants.PREVIEW_BASIC_AUTHENTICATION_USER_ACCOUNT))
				? Constants.BASIC_AUTHENTICATION_USER_ACCOUNT
				: Constants.PREVIEW_BASIC_AUTHENTICATION_USER_ACCOUNT;
			// basic認証情報をBASE64でエンコードして設定
			webRequest.Headers.Add("Authorization: Basic " + Convert.ToBase64String(authorizationEncoding.GetBytes(basicUserAccount)));
			// 自己証明許可
			webRequest.ServerCertificateValidationCallback = OnRemoteCertificateValidationCallback;
			// 要求がリダイレクト応答に従わないように設定
			webRequest.AllowAutoRedirect = false;

			var errorMessage = string.Empty;

			try
			{
				using ((HttpWebResponse)webRequest.GetResponse()) { }
			}
			// リクエストで例外発生時は、エラー画面に遷移しレスポンスの内容を出力する
			catch (WebException we)
			{
				if (we.Response != null)
				{
					errorMessage = GetAccessErrorMessage(we);

					if (string.IsNullOrEmpty(errorMessage) || errorMessage.Contains("[ConfigurationErrorsException]"))
					{
						FileLogger.WriteError(string.Format("URL（{0}）へのアクセスでエラーが発生しました。(ConfigurationErrorsException)", webUrl), we);
					}

					// 認証エラーの場合はエラー画面に遷移し、認証エラーである旨のメッセージを表示
					if (((HttpWebResponse)we.Response).StatusCode == HttpStatusCode.Unauthorized)
					{
						return WebMessages.FrontSiteUnAuthorizedError;
					}

					// SQLエラーの場合
					if (errorMessage.Contains("SqlException"))
					{
						errorMessage = WebMessages.DataBaseError;
						FileLogger.WriteError(errorMessage);
					}
				}
				else
				{
					errorMessage = WebMessages.FrontSiteAccessError;

					FileLogger.WriteError(string.Format("URL（{0}）へのアクセスでエラーが発生しました。", webUrl), we);

					return errorMessage;
				}
			}

			return errorMessage;
		}

		/// <summary>
		/// アクセスエラーメッセージ取得
		/// </summary>
		/// <param name="we">WebException</param>
		/// <returns>エラーメッセージ</returns>
		private static string GetAccessErrorMessage(WebException we)
		{
			var errorMessagesTmp = new StringBuilder();

			using (var responseStream = we.Response.GetResponseStream())
			using (var sr = new StreamReader(responseStream, Encoding.UTF8))
			{
				var lineFeedFlg = false;
				while (sr.EndOfStream == false)
				{
					var readedLine = sr.ReadLine();

					// 「<h2> <i>」タグから読み取り開始
					if ((readedLine.Contains(ERROR_CAPTION_TAG) == false) && (errorMessagesTmp.Length == 0))
					{
						continue;
					}
					// 「詳しいコンパイラ出力を表示：～」があれば読み取り終了
					if (readedLine.Contains(ERROR_CAPTION_DETAIL_COMPILER_OUTPUT)) break;

					errorMessagesTmp.Append(readedLine);

					// </code>タグがあれば改行フラグをオフにする
					if (readedLine.Contains(ERROR_CODE_END_TAG))
					{
						lineFeedFlg = false;

						// 読み取り行が</pre>,</code>タグのみ含むかどうかで改行を調整
						if ((readedLine.StartsWith(ERROR_PREFORMATTED_TEXT_END_TAG)) || (readedLine.StartsWith(ERROR_CODE_END_TAG)))
						{
							errorMessagesTmp.Replace(ERROR_CODE_END_TAG, ERROR_CODE_END_TAG + "<br />");
						}
						else
						{
							errorMessagesTmp.Replace(ERROR_CODE_END_TAG, "<br />" + ERROR_CODE_END_TAG + "<br />");
						}
					}

					// <code>～</code>タグ間は改行タグを付与する
					if (lineFeedFlg) errorMessagesTmp.Append("<br />");

					// <code>タグがあれば改行フラグをオンにする
					if (readedLine.Contains(ERROR_CODE_START_TAG)) lineFeedFlg = true;
				}
			}

			// IEでテーブル内で文字列が折り返されないため<pre>～</pre>タグを削除
			errorMessagesTmp.Replace(ERROR_PREFORMATTED_TEXT_START_TAG, "");
			errorMessagesTmp.Replace(ERROR_PREFORMATTED_TEXT_END_TAG, "");

			// エラー情報のルート物理パスを削除
			var errorMessages = errorMessagesTmp.ToString();
			errorMessages = Regex.Replace(errorMessages, Constants.PHYSICALDIRPATH_FRONT_PC.Replace(@"\", @"\\"), "", RegexOptions.IgnoreCase);
			errorMessages = Regex.Replace(errorMessages, Constants.PATH_ROOT_FRONT_PC, "", RegexOptions.IgnoreCase);

			return errorMessages;
		}

		/// <summary>
		/// 自己証明許可のためのコールバック用
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="certificate"></param>
		/// <param name="chain"></param>
		/// <param name="sslPolicyErrors"></param>
		/// <returns></returns>
		private static bool OnRemoteCertificateValidationCallback(
			Object sender,
			X509Certificate certificate,
			X509Chain chain,
			SslPolicyErrors sslPolicyErrors)
		{
			// 信用したことにする
			return true;
		}
	}
}
