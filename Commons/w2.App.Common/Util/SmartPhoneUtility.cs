/*
=========================================================================================================
  Module      : スマートフォンユーティリティ(SmartPhoneUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using w2.Common.Util;

namespace w2.App.Common.Util
{
	//*********************************************************************************************
	/// <summary>
	/// スマートフォンユーティリティ
	/// </summary>
	//*********************************************************************************************
	public class SmartPhoneUtility
	{
		public static string DIRPATH_SMARTPHONE_SITE_SETTING = @"Settings\";
		public const string FILENAME_SMARTPHONE_SITE_SETTING = @"SmartPhoneSiteSetting.xml";

		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static SmartPhoneUtility()
		{
			// 設定更新
			UpdateSetting();
		}

		/// <summary>
		/// 設定更新
		/// </summary>
		public static void UpdateSetting()
		{
			List<SmartPhoneSiteSetting> lSmartPhoneSiteSettings = new List<SmartPhoneSiteSetting>();

			// 設定読み込み
			try
			{
				XmlDocument xdSmartPhoneSetting = new XmlDocument();
				xdSmartPhoneSetting.Load(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE + DIRPATH_SMARTPHONE_SITE_SETTING + FILENAME_SMARTPHONE_SITE_SETTING);
				foreach (XmlNode xnSetting in xdSmartPhoneSetting.SelectNodes("/SmartPhoneSiteSetting/Setting"))
				{
					lSmartPhoneSiteSettings.Add(
						new SmartPhoneSiteSetting(
							xnSetting.Attributes["name"].Value,
							xnSetting.Attributes["pattern"].Value,
							xnSetting.SelectSingleNode("RootPath").InnerText));
				}
			}
			catch (Exception ex)
			{
				w2.Common.Logger.AppLogger.WriteError("スマートフォンサイト設定XMLの読み込みに失敗しました。", ex);
			}

			// 設定張り替え
			m_lSmartPhoneSiteSettings = lSmartPhoneSiteSettings;
		}

		/// <summary>
		/// スマートフォンURL取得
		/// </summary>
		/// <param name="strUrlOrg">オリジナルURL</param>
		/// <param name="strUserAgent">ユーザーエージェント</param>
		/// <param name="hcCurrent">HTTPコンテキスト</param>
		/// <returns>スマートフォンURL（マッチングしなかったらnull）</returns>
		public static string GetSmartPhoneUrl(string strUrlOrg, string strUserAgent, HttpContext hcCurrent)
		{
			var targetSetting =
				m_lSmartPhoneSiteSettings.FirstOrDefault(
					setting => Regex.IsMatch(StringUtility.ToEmpty(strUserAgent), setting.UserAgentPattern));

			// SPプレビューの場合はユーザーエージェントに関わらず設定を取得する
			if (HttpContext.Current.Request.Path.Contains("/SmartPhone/Page/Preview/"))
			{
				targetSetting = m_lSmartPhoneSiteSettings[0];
			}
			// 設定が取得できなかったらnull
			if (targetSetting == null) return null;
			// スマートフォンサイトが無効であればnull
			if (targetSetting.SmartPhonePageEnabled == false) return null;

			string strQueryLessUrl = targetSetting.RootPath + strUrlOrg.Split('?')[0].Substring(2);
			var ngChars = new[] { "<", ">", "*", "\\", "\"", "%", "|" };
			var enable = (ngChars.Any(item => strQueryLessUrl.Contains(item)) == false);
			if (enable && (System.IO.File.Exists(hcCurrent.Server.MapPath(strQueryLessUrl))))
			{
				return strQueryLessUrl + (strUrlOrg.Contains('?') ? strUrlOrg.Substring(strUrlOrg.IndexOf('?')) : "");
			}

			// 見つからなければnull
			return null;
		}

		/// <summary>
		/// スマートフォンパス取得
		/// </summary>
		/// <param name="urlOrg">オリジナルパス</param>
		/// <param name="userAgent">ユーザーエージェント</param>
		/// <param name="pathOrg"></param>
		/// <returns>スマートフォンパス</returns>
		public static string GetSmartPhonePath(string urlOrg, string userAgent, string pathOrg)
		{
			// 存在するURLを検索して返す
			foreach (var spss in m_lSmartPhoneSiteSettings
				.Where(spss => spss.SmartPhonePageEnabled))
			{
				if (Regex.IsMatch(StringUtility.ToEmpty(userAgent), spss.UserAgentPattern))
				{
					string path = HttpContext.Current.Server.MapPath(Constants.PATH_ROOT + spss.RootPath.TrimStart('~') + pathOrg);
					if (System.IO.File.Exists(path))
					{
						return path;
					}
				}
			}

			// 見つからなければnull
			return null;
		}

		/// <summary>
		/// スマートフォンコンテンツURL取得
		/// </summary>
		/// <param name="contensUrl">スマートフォンコンテンツURL（ルートパス含めない）</param>
		/// <returns>スマートフォンコンテンツURL</returns>
		/// <remarks>スマートフォンコンテンツURLがなければPCコンテンツURLを表示</remarks>
		public static string GetSmartPhoneContentsUrl(string contensUrl)
		{
			// スマートフォンOP有効 AND スマートフォンサイト?
			if ((Constants.SMARTPHONE_OPTION_ENABLED)
				&& (CheckSmartPhoneSite(HttpContext.Current.Request.Path)))
			{
				var smartphoneUrl = GetSmartPhoneUrl("~/" + contensUrl, HttpContext.Current.Request.UserAgent, HttpContext.Current);
				if (smartphoneUrl != null) return VirtualPathUtility.ToAbsolute(smartphoneUrl);
			}

			return Constants.PATH_ROOT + contensUrl;
		}

		/// <summary>
		/// スマートフォン判定
		/// </summary>
		/// <param name="strUserAgent">ユーザーエージェント</param>
		/// <returns>スマートフォンか否か</returns>
		public static bool CheckSmartPhone(string strUserAgent)
		{
			foreach (SmartPhoneSiteSetting spss in m_lSmartPhoneSiteSettings)
			{
				if (Regex.IsMatch(StringUtility.ToEmpty(strUserAgent), spss.UserAgentPattern))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// スマートフォンサイト判定
		/// </summary>
		/// <param name="strApplicationPathFromRoot">ルートからのアプリケーションパス</param>
		/// <returns>スマートフォンサイトか否か</returns>
		public static bool CheckSmartPhoneSite(string strApplicationPathFromRoot)
		{
			foreach (var spss in m_lSmartPhoneSiteSettings
				.Where(spss => spss.SmartPhonePageEnabled))
			{
				if (spss.IsSmartPhoneSite(strApplicationPathFromRoot))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>スマートフォンサイト設定</summary>
		public static List<SmartPhoneSiteSetting> SmartPhoneSiteSettings
		{
			get { return m_lSmartPhoneSiteSettings; }
		}
		static List<SmartPhoneSiteSetting> m_lSmartPhoneSiteSettings = new List<SmartPhoneSiteSetting>();

		//*********************************************************************************************
		/// <summary>
		/// スマートフォンサイトクラス
		/// </summary>
		//*********************************************************************************************
		public class SmartPhoneSiteSetting
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="strName">設定名</param>
			/// <param name="strUserAgentPattern">UserAgentパターン(正規表現)</param>
			/// <param name="strRootPath">ルートパス（「Form/」に置き換わるもの）</param>
			public SmartPhoneSiteSetting(string strName, string strUserAgentPattern, string strRootPath)
			{
				this.Name = strName;
				this.UserAgentPattern = strUserAgentPattern;
				this.RootPath = strRootPath;
			}

			/// <summary>
			/// スマートフォンサイト判定
			/// </summary>
			/// <param name="strApplicationPathFromRoot">ルートからのアプリケーションパス</param>
			/// <returns>スマートフォンサイトか否か</returns>
			public bool IsSmartPhoneSite(string strApplicationPathFromRoot)
			{
				if (this.SmartPhonePageEnabled == false) return false;

				string strPathTmp = "~/" + strApplicationPathFromRoot.Substring(Constants.PATH_ROOT.Length);

				return strPathTmp.StartsWith(this.RootPath, false, null);
			}

			/// <summary>設定名</summary>
			public string Name { get; private set; }
			/// <summary>パターン</summary>
			public string UserAgentPattern { get; private set; }
			/// <summary>ルートパス</summary>
			public string RootPath { get; private set; }
			/// <summary>スマートフォンページ利用が有効か（レスポンシブの場合はfalse）</summary>
			public bool SmartPhonePageEnabled
			{
				get { return (string.IsNullOrEmpty(this.RootPath) == false); }
			}
		}
	}
}
