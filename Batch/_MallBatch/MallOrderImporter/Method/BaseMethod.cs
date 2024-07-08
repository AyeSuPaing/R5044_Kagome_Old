/*
=========================================================================================================
  Module      : ベースメソッド(BaseMethod.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using w2.App.Common.MallCooperation;
using w2.Common.Logger;
using w2.Common.Net.Mail;
using w2.Commerce.MallBatch.MallOrderImporter.HtmlPerser;
using w2.Commerce.MallBatch.MallOrderImporter.HtmlObjects;
using System.Threading;
using w2.Common.Util;

namespace w2.Commerce.MallBatch.MallOrderImporter.Method
{
	internal class BaseMethod
	{
		// ユーザエージェント
		private const string REQUEST_USERAGENT =
			"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; GTB6.6; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; InfoPath.2; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; OfficeLiveConnector.1.3; OfficeLivePatch.0.0; .NET4.0C; .NET4.0E)";
		// スレッドスリープ
		private const int REQUEST_THREAD_SLEEP = 3000;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected BaseMethod()
		{
			this.Cookies = new CookieContainer();
		}

		/// <summary>
		/// HTML書き込み（デバッグ用）★
		/// </summary>
		/// <param name="strHtml"></param>
		/// <param name="strFileName"></param>
		protected void WriteHtml(string strHtml, string strFileName)
		{
			using (
				TextWriter twTextWriter = new StreamWriter(@"C:\Documents and Settings\ochiai\デスクトップ\" + strFileName, false,
					Encoding.GetEncoding("EUC-JP")))
			{
				twTextWriter.WriteLine(strHtml);
			}
		}

		/// <summary>
		/// フォーム情報取得
		/// </summary>
		/// <param name="strHtml"></param>
		/// <param name="strFormName"></param>
		/// <returns></returns>
		protected FormInfo GetFormInfo(string strHtml, string strFormName)
		{
			// \ . ^ $ [ ] * + ? | ( ) をエスケープ
			return GetFormInfo(strHtml,
				new Regex(
					strFormName.Replace(@"\", @"\\")
						.Replace(@".", @"\.")
						.Replace(@"^", @"\^")
						.Replace(@"$", @"\$")
						.Replace(@"[", @"\[")
						.Replace(@"]", @"\]")
						.Replace(@"*", @"\*")
						.Replace(@"+", @"\+")
						.Replace(@"?", @"\?")
						.Replace(@"|", @"\|")
						.Replace(@"(", @"\(")
						.Replace(@")", @"\)")));
		}

		/// <summary>
		/// フォーム情報取得
		/// </summary>
		/// <param name="strHtml"></param>
		/// <param name="rPattern"></param>
		/// <returns></returns>
		protected FormInfo GetFormInfo(string strHtml, Regex rFormName)
		{
			FormInfo fiTargetFormInfo = null;

			HtmlParser hpParser = new HtmlParser();
			hpParser.Source = strHtml;
			while (!hpParser.Eof())
			{
				var ch = hpParser.Parse();
				if (ch == 0)
				{
					HtmlTagInfo alTagTmp = hpParser.GetTag();

					// formタグが見つからない？
					if ((fiTargetFormInfo == null) && (alTagTmp.Name.ToLower() == "/html"))
					{
						WriteWarnAndThrow(this.TransactionName + "：formタグを取得できませんでした。", strHtml);
					}

					// formタグ？
					if ((fiTargetFormInfo == null) && (alTagTmp.Name.ToLower() == "form")
					    && ((alTagTmp["name"] != null) && (rFormName.IsMatch(alTagTmp["name"].Value))))
					{
						fiTargetFormInfo = new FormInfo(alTagTmp["action"].Value, alTagTmp["method"].Value);
					}
						// inputタグ？
					else if (alTagTmp.Name.ToLower() == "input")
					{
						if ((alTagTmp["name"] != null) && (alTagTmp["value"] != null)
						    && (fiTargetFormInfo.Params.ContainsKey(alTagTmp["name"].Value) == false))
						{
							fiTargetFormInfo.Params.Add(alTagTmp["name"].Value, alTagTmp["value"].Value);
						}
						else if ((alTagTmp["id"] != null) && (alTagTmp["value"] != null)
						         && (fiTargetFormInfo.Params2.ContainsKey(alTagTmp["id"].Value) == false))
						{
							fiTargetFormInfo.Params2.Add(alTagTmp["id"].Value, alTagTmp["value"].Value);
						}
					}
						// formタグ終了？
					else if ((fiTargetFormInfo != null) && (alTagTmp.Name.ToLower() == "/form"))
					{
						break;
					}
				}
			}

			return fiTargetFormInfo;
		}


		/// <summary>
		/// POSTリクエスト（ファイルダウンロード）
		/// </summary>
		/// <param name="formActionUrl">FormアクションURL</param>
		/// <param name="formParams">パラメタ</param>
		/// <param name="encoding">エンコーディング</param>
		/// <param name="saveDirPath">ファイル保存先</param>
		/// <returns>作成ファイル名</returns>
		protected string ExecPostRequestFileDownload(string formActionUrl, Dictionary<string, string> formParams,
			Encoding encoding, string saveDirPath)
		{
			// POST実行
			WebHeaderCollection responseHeaders;
			var responseString = ExecPostRequestInner(formActionUrl, formParams, encoding, out responseHeaders);

			// ファイルパス取得
			var contentDisposition = StringUtility.ToEmpty(responseHeaders["Content-Disposition"]);
			var match = Regex.Match(contentDisposition, @""".*""");
			if (match.Success == false)
			{
				FileLogger.WriteError("ファイル名が取得できませんでした：contentDisposition = " + contentDisposition);
				return null;
			}
			var fileName = match.Value.Replace(@"""", "");
			var filePath = Path.Combine(saveDirPath, fileName);
			if (Directory.Exists(saveDirPath) == false) Directory.CreateDirectory(saveDirPath);
			if (File.Exists(filePath)) File.Delete(filePath);

			// ファイル書き込み
			using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
			using (var sw = new StreamWriter(fs, encoding))
			{
				sw.Write(responseString);
			}
			return fileName;
		}

		/// <summary>
		/// GETリクエスト
		/// </summary>
		/// <param name="currentUrl">カレントURL</param>
		/// <param name="formActionUrl">FormAction先URL</param>
		/// <param name="encoding">エンコード</param>
		/// <returns>取得ページ</returns>
		protected ResponseObject ExecGetRequest(string currentUrl, string formActionUrl, Encoding encoding)
		{
			return ExecGetRequest(GenarateNextUrl(currentUrl, formActionUrl), encoding);
		}

		/// <summary>
		/// GETリクエスト
		/// </summary>
		/// <param name="url">GET先URL</param>
		/// <param name="encoding">エンコード</param>
		/// <returns>取得ページ</returns>
		protected ResponseObject ExecGetRequest(string url, Encoding encoding)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.CookieContainer = this.Cookies;
			request.UserAgent = REQUEST_USERAGENT;

			string html = null;
			using (var responseStream = request.GetResponse().GetResponseStream())
			using (var sr = new StreamReader(responseStream, encoding))
			{
				html = sr.ReadToEnd();
			}
			// スレッドスリープ
			Thread.Sleep(REQUEST_THREAD_SLEEP);

			return new ResponseObject(request.Address.AbsoluteUri, html);
		}

		/// <summary>
		/// POSTリクエスト
		/// </summary>
		/// <param name="strCurrentUrl">カレントURL</param>
		/// <param name="strFormActionUrl">FormAction先URL</param>
		/// <param name="dParams">パラメタ</param>
		/// <param name="encoding">エンコード</param>
		/// <returns>取得ページ</returns>
		protected ResponseObject ExecPostRequest(string strCurrentUrl, string strFormActionUrl,
			Dictionary<string, string> dParams, Encoding encoding)
		{
			return ExecPostRequest(GenarateNextUrl(strCurrentUrl, strFormActionUrl), dParams, encoding);
		}

		/// <summary>
		/// POSTリクエスト
		/// </summary>
		/// <param name="formActionUrl">POST先URL</param>
		/// <param name="formParams">パラメタ</param>
		/// <param name="encoding">エンコード</param>
		/// <returns>取得ページ</returns>
		protected ResponseObject ExecPostRequest(string formActionUrl, Dictionary<string, string> formParams,
			Encoding encoding)
		{
			WebHeaderCollection responseHeaders;
			var responseString = ExecPostRequestInner(formActionUrl, formParams, encoding, out responseHeaders);

			return new ResponseObject(formActionUrl, responseString);
		}

		/// <summary>
		/// POSTリクエスト
		/// </summary>
		/// <param name="formActionUrl">POST先URL</param>
		/// <param name="formParams">パラメタ</param>
		/// <param name="encoding">エンコード</param>
		/// <param name="responseHeaders">レスポンスヘッダ</param>
		/// <returns></returns>
		private string ExecPostRequestInner(string formActionUrl, Dictionary<string, string> formParams, Encoding encoding,
			out WebHeaderCollection responseHeaders)
		{
			// パラメタ作成
			var paramString = string.Join("&",
				formParams.Select(p => string.Format("{0}={1}", p.Key, HttpUtility.UrlEncode(p.Value, encoding))).ToArray());

			var request = (HttpWebRequest)WebRequest.Create(formActionUrl);
			request.Method = "POST";
			request.Timeout = 60000; // 60秒でタイムアウトさせる。
			//webReq.Proxy = System.Net.WebProxy.GetDefaultProxy();	// IE のProxy 設定を使用する。
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = Encoding.ASCII.GetBytes(paramString).Length;
			request.CookieContainer = this.Cookies;
			request.UserAgent = REQUEST_USERAGENT;
			// Ascii なので、エンコーディングは指定する必要なし。
			using (var reqStream = request.GetRequestStream())
			using (var sw = new StreamWriter(reqStream))
			{
				sw.Write(paramString);
			}

			// 結果を受け取る
			string responseString;
			using (var webRes = request.GetResponse())
			using (var stream = webRes.GetResponseStream())
			using (var sr = new StreamReader(stream, encoding))
			{
				responseString = sr.ReadToEnd();
				responseHeaders = webRes.Headers;
			}
			// スレッドスリープ
			Thread.Sleep(REQUEST_THREAD_SLEEP);

			return responseString;
		}

		/// <summary>
		/// Form遷移先URL作成
		/// </summary>
		/// <param name="currentUrl"></param>
		/// <param name="formActionUrl"></param>
		/// <returns></returns>
		protected string GenarateNextUrl(string currentUrl, string formActionUrl)
		{
			var targetUrl = new StringBuilder();
			if ((currentUrl != null) && (formActionUrl != null))
			{
				if (formActionUrl.StartsWith(Uri.UriSchemeHttp) == false)
				{
					var current = new Uri(currentUrl);
					targetUrl.Append(current.Scheme).Append(Uri.SchemeDelimiter).Append(current.Host);
					if (formActionUrl.StartsWith("/"))
					{
						targetUrl.Append(formActionUrl);
					}
					else
					{
						if (currentUrl.EndsWith("/"))
						{
							targetUrl.Append(current.AbsolutePath).Append(formActionUrl);
						}
						else
						{
							for (int iIndex = 0; iIndex < current.Segments.Length - 1; iIndex++)
							{
								targetUrl.Append(current.Segments[iIndex]);
							}
							targetUrl.Append(formActionUrl);
						}
					}
				}
				else
				{
					targetUrl.Append(formActionUrl);
				}
			}
			return (targetUrl.Length != 0) ? targetUrl.ToString() : null;
		}

		/// <summary>
		/// アンカーURL取得
		/// </summary>
		/// <param name="html">対象HTML</param>
		/// <param name="linkInnerString">リンク内文字列</param>
		/// <returns></returns>
		protected string GetAnchorUrl(string html, string linkInnerString)
		{
			var parser = new HtmlParser()
			{
				Source = html
			};

			HtmlTagInfo beforeAnchorTag = null;
			var sbTemp = new StringBuilder();
			while (parser.Eof() == false)
			{
				char next = parser.Parse();
				if (next == 0)
				{
					sbTemp = new StringBuilder();
					var halTagTmp = parser.GetTag();
					if (halTagTmp.Name.ToLower() == "a")
					{
						beforeAnchorTag = halTagTmp;
					}
					else if (halTagTmp.Name.ToLower() == "/a")
					{
						beforeAnchorTag = null;
					}
				}
				else
				{
					sbTemp.Append(next);
					// リンク先URL取得
					if ((sbTemp.Length == linkInnerString.Length) && (sbTemp.ToString() == linkInnerString))
					{
						if (beforeAnchorTag != null)
						{
							foreach (var haTarget in beforeAnchorTag.List)
							{
								if (haTarget.Name.ToLower() == "href") return haTarget.Value;
							}
						}
						break;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// エラーメールを送信する
		/// </summary>
		/// <param name="strErrorMassage">エラーメッセージ</param>
		public static void ErrorMailSender(string strErrorMassage)
		{
			using (var smsMailSend = new SmtpMailSender(Constants.SERVER_SMTP))
			{
				smsMailSend.SetSubject(Constants.MAIL_SUBJECTHEAD);
				smsMailSend.SetFrom(Constants.MAIL_FROM.Address);
				Constants.MAIL_TO_LIST.ForEach(mail => smsMailSend.AddTo(mail.Address));
				Constants.MAIL_CC_LIST.ForEach(mail => smsMailSend.AddCC(mail.Address));
				Constants.MAIL_BCC_LIST.ForEach(mail => smsMailSend.AddBcc(mail.Address));
				smsMailSend.SetBody("[取り込みエラー]" + DateTime.Now + "\r\n" + strErrorMassage);
				if (smsMailSend.SendMail() == false)
				{
					// 送信エラーの場合ログ書き込み
					FileLogger.WriteError(smsMailSend.MailSendException.ToString());
				}
			}
		}

		/// <summary>
		/// モール監視ログ挿入
		/// </summary>
		/// <param name="logKbn">モールID</param>
		/// <param name="logKbn">ログ区分</param>
		/// <param name="message">メッセージ</param>
		protected void InsertMallWatchingLog(string mallId, string logKbn, string message)
		{
			MallWatchingLogManager.Insert(
				Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MALLORDERIMPORTER,
				mallId,
				Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
				message);

		}

		/// <summary>
		/// 警告ログ出力
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="detail">詳細メッセージ（HTMLなど）</param>
		protected void WriteWarnAndThrow(string message, string detail = null)
		{
			if (string.IsNullOrEmpty(this.MallId) == false) InsertMallWatchingLog(this.MallId, Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR, message);
			Console.WriteLine(message);
			FileLogger.WriteWarn(message + ((string.IsNullOrEmpty(detail) == false) ? ("\r\n" + detail) : ""));
			this.LastMessage = message;

			throw new Exception(message);
		}

		/// <summary>モールID</summary>
		protected string MallId { get; set; }
		/// <summary>トランザクション名</summary>
		protected string TransactionName { get; set; }
		/// <summary>クッキー</summary>
		private CookieContainer Cookies { get; set; }
		/// <summary>
		/// モール監視ログマネージャ
		/// </summary>
		public static MallWatchingLogManager MallWatchingLogManager
		{
			get { return m_mallWatchingLogManager; }
		}
		public static MallWatchingLogManager m_mallWatchingLogManager = new MallWatchingLogManager();
		/// <summary>最終警告メッセージ</summary>
		public string LastMessage { get; private set; }
	}
}
