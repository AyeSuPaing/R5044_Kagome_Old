/*
=========================================================================================================
  Module      : デバッグログプロバイダ(DebugLogProvider.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using w2.Common.Util;
using w2.Common.Logger;

namespace w2.App.Common.Util
{
	/// <summary>
	/// デバッグログプロバイダクラス
	/// </summary>
	public class DebugLogProvider
	{
		/// <summary>サイト</summary>
		public enum Site
		{
			Pc,
			Mobile
		}

		// デバッグログ設定XMLファイルパス
		public const string DIRPATH_DEBUGLOG_SETTING = @"Settings\";
		public const string FILENAME_DEBUGLOG_TARGET_PAGES = @"DebugTargetPages.xml";

		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static DebugLogProvider()
		{
			// 設定更新
			UpdateSetting();
		}

		/// <summary>
		/// 設定更新
		/// </summary>
		public static void UpdateSetting()
		{
			// 設定読み込み
			List<DebugTargetPage> pages = new List<DebugTargetPage>();
			string filePath = Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE + DIRPATH_DEBUGLOG_SETTING + FILENAME_DEBUGLOG_TARGET_PAGES;
			try
			{
				// デバッグ対象ページ設定XMLが存在する？
				if (File.Exists(filePath))
				{
					// デバッグ対象ページリストに追加
					XDocument xd = XDocument.Load(filePath);
					var targets = from target in xd.Descendants("Target")
								  select new
								  {
									  url = target.Attribute("url").Value,
									  site = target.Parent.Attribute("site").Value
								  };
					foreach (var target in targets)
					{
						pages.Add(new DebugTargetPage(target.url, (Site)Enum.Parse(typeof(Site), target.site)));
					}
				}
			}
			catch (Exception ex)
			{
				w2.Common.Logger.AppLogger.WriteError("デバッグログ設定XML（" + filePath + "）の読み込みに失敗しました。", ex);
			}

			// 設定書き換え
			DebugTargetPages = pages;
		}

		/// <summary>
		/// デバッグログ出力
		/// </summary>
		/// <param name="context">HTTPコンテキスト</param>
		/// <param name="sessionKeys">セッションキー</param>
		/// <param name="site">サイト</param>
		public static void Write(HttpContext context, string[] sessionKeys, Site site)
		{
			if (context != null)
			{
				// デバッグ対象ページ？
				HttpRequest request = context.Request;
				if (IsTarget(request.Path, site))
				{
					StringBuilder debug = new StringBuilder();

					// URL
					debug.Append("\"").Append("url=").Append(StringUtility.EscapeCsvColumn(request.Path)).Append("\",");
					// IPアドレス
					debug.Append("\"").Append("ipaddr=").Append(StringUtility.EscapeCsvColumn(request.UserHostAddress)).Append("\",");
					// ユーザエージェント
					debug.Append("\"").Append("useragent=").Append(StringUtility.EscapeCsvColumn(request.UserAgent)).Append("\",");
					// セッションログ
					StringBuilder debugSession = new StringBuilder();
					if (context.Session != null)
					{
						// セッションID
						debugSession.Append("session_id=").Append(context.Session.SessionID);
						// 引数セッションキー
						foreach (string key in sessionKeys) debugSession.Append("\r\n").Append(key).Append("=").Append(context.Session[key]);
					}
					debug.Append("\"").Append(StringUtility.EscapeCsvColumn(debugSession.ToString())).Append("\",");
					// リクエストログ
					StringBuilder debugRequest = new StringBuilder();
					foreach (string key in request.Form.Keys)
					{
						// "__VIEWSTATE"、"__EVENTVALIDATION"以外のキーのみ対象とする
						if (debugRequest.ToString() != "") debugRequest.Append("\r\n");
						if (key != "__VIEWSTATE" && key != "__EVENTVALIDATION") debugRequest.Append(key).Append("=").Append(request[key]);
					}
					debug.Append("\"").Append(StringUtility.EscapeCsvColumn(debugRequest.ToString())).Append("\"");

					// デバッグログ出力
					AppLogger.WriteDebug(debug.ToString());
				}
			}
		}

		/// <summary>
		/// デバッグ対象ページ？
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="site">サイト</param>
		/// <returns>デバッグ対象ページか否か</returns>
		public static bool IsTarget(string url, Site site)
		{
			return DebugTargetPages.Any(target => target.IsTarget(url, site));
		}

		/// <summary>デバッグ対象ページリスト</summary>
		public static List<DebugTargetPage> DebugTargetPages { get; private set; }

		/// <summary>
		/// デバッグ対象ページクラス
		/// </summary>
		public class DebugTargetPage
		{
			/// <summary>
			/// デフォルトコンストラクタ
			/// </summary>
			/// <param name="url">URL</param>
			/// <param name="site">サイト</param>
			public DebugTargetPage(string url, Site site)
			{
				this.Url = url;
				this.Site = site;
			}

			/// <summary>
			/// デバッグ対象ページ？
			/// </summary>
			/// <param name="url">URL</param>
			/// <param name="site">サイト</param>
			/// <returns>デバッグ対象ページか否か</returns>
			public bool IsTarget(string url, Site site)
			{
				return (url.ToLower().StartsWith(Constants.PATH_ROOT.ToLower() + this.Url.ToLower()))
					&& (this.Site == site);
			}

			/// <summary>Url</summary>
			public string Url { get; private set; }
			/// <summary>サイト</summary>
			public Site Site { get; private set; }
		}
	}
}