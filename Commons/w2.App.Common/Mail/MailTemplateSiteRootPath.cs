/*
=========================================================================================================
  Module      : メールテンプレートで利用するルートパス情報管理クラス(MailTemplateSiteRootPath.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Text.RegularExpressions;
using w2.Common.Web;

namespace w2.App.Common.Mail
{
	public class MailTemplateSiteRootPath
	{
		#region 定数
		/// <summary>メールテンプレートタグ：PCサイトのルートパス置換タグ</summary>
		private const string TAG_SITE_ROOT_PATH_FRONT_PC_REPLACE_TEXT = "@@ site_root_path_PCFront @@";
		/// <summary>メールテンプレートタグ：PCサイトのルートパス置換タグ表示文言</summary>
		private const string TAG_SITE_ROOT_PATH_FRONT_PC_DISPLAY_TEXT = "PCサイトのルートパス";
		/// <summary>メールテンプレートタグ：EC管理サイトのルートパス置換タグ</summary>
		private const string TAG_SITE_ROOT_PATH_EC_REPLACE_TEXT = "@@ site_root_path_w2cManager @@";
		/// <summary>メールテンプレートタグ：EC管理サイトのルートパス置換タグ表示文言</summary>
		private const string TAG_SITE_ROOT_PATH_EC_DISPLAY_TEXT = "EC管理サイトのルートパス";
		/// <summary>メールテンプレートタグ：MP管理サイトのルートパス置換タグ</summary>
		private const string TAG_SITE_ROOT_PATH_MP_REPLACE_TEXT = "@@ site_root_path_w2mpManager @@";
		/// <summary>メールテンプレートタグ：MP管理サイトのルートパス置換タグ表示文言</summary>
		private const string TAG_SITE_ROOT_PATH_MP_DISPLAY_TEXT = "MP管理サイトのルートパス";
		/// <summary>メールテンプレートタグ：CS管理サイトのルートパス置換タグ</summary>
		private const string TAG_SITE_ROOT_PATH_CS_REPLACE_TEXT = "@@ site_root_path_w2csManager @@";
		/// <summary>メールテンプレートタグ：CS管理サイトのルートパス置換タグ表示文言</summary>
		private const string TAG_SITE_ROOT_PATH_CS_DISPLAY_TEXT = "CS管理サイトのルートパス";
		/// <summary>メールテンプレートタグ：CMS管理サイトのルートパス置換タグ</summary>
		private const string TAG_SITE_ROOT_PATH_CMS_REPLACE_TEXT = "@@ site_root_path_w2cmsManager @@";
		/// <summary>メールテンプレートタグ：CMS管理サイトのルートパス置換タグ表示文言</summary>
		private const string TAG_SITE_ROOT_PATH_CMS_DISPLAY_TEXT = "CMS管理サイトのルートパス";
		#endregion

		/// <summary>
		/// コンストラクター
		/// </summary>
		public MailTemplateSiteRootPath()
		{
			SetSiteRootPath();
		}

		/// <summary>
		/// メールテンプレートタグ用ルートパスを設定
		/// </summary>
		private void SetSiteRootPath()
		{
			var protocolHttps = (string.IsNullOrEmpty(Constants.PROTOCOL_HTTPS) == false)
				? Constants.PROTOCOL_HTTPS
				: "https://";
			this.SiteRootPathPcFront = protocolHttps + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC;
			this.SiteRootPathEc = protocolHttps + Constants.SITE_DOMAIN + Constants.PATH_ROOT_EC;
			this.SiteRootPathMp = protocolHttps + Constants.SITE_DOMAIN + Constants.PATH_ROOT_MP;
			this.SiteRootPathCs = protocolHttps + Constants.SITE_DOMAIN + Constants.PATH_ROOT_CS;
			this.SiteRootPathCms = protocolHttps + Constants.SITE_DOMAIN + Constants.PATH_ROOT_CMS;
		}

		/// <summary>
		/// サイトのルートパスタグ置換
		/// </summary>
		/// <param name="replaceText">置換前文字列</param>
		/// <param name="doHtmlEncodeChangeToBr">改行をbrにするか</param>
		/// <returns>置換後文字列</returns>
		public string ConvertSiteRootPath(string replaceText, bool doHtmlEncodeChangeToBr)
		{
			var replaceTags = new Dictionary<string, string>
			{
				{ TAG_SITE_ROOT_PATH_FRONT_PC_REPLACE_TEXT, this.SiteRootPathPcFront },
				{ TAG_SITE_ROOT_PATH_EC_REPLACE_TEXT, this.SiteRootPathEc },
				{ TAG_SITE_ROOT_PATH_MP_REPLACE_TEXT, this.SiteRootPathMp },
				{ TAG_SITE_ROOT_PATH_CS_REPLACE_TEXT, this.SiteRootPathCs },
				{ TAG_SITE_ROOT_PATH_CMS_REPLACE_TEXT, this.SiteRootPathCms },
			};

			foreach (var replaceTag in replaceTags)
			{
				foreach (Match siteRootPathTag in Regex.Matches(replaceText, replaceTag.Key))
				{
					replaceText = replaceText.Replace(
						siteRootPathTag.Value,
						doHtmlEncodeChangeToBr
							? HtmlSanitizer.HtmlEncodeChangeToBr(replaceTag.Value)
							: replaceTag.Value);
				}
			}

			return replaceText;
		}

		/// <summary>
		/// サイトのルートパスの表示文言とメールタグを一緒に取得
		/// </summary>
		/// <returns>表示文言とメールタグを含む配列</returns>
		public KeyValuePair<string, string>[] GetSiteRootPathMailTagArray()
		{
			var result = new[]
			{
				new KeyValuePair<string, string>(
					TAG_SITE_ROOT_PATH_FRONT_PC_REPLACE_TEXT,
					TAG_SITE_ROOT_PATH_FRONT_PC_DISPLAY_TEXT),
				new KeyValuePair<string, string>(
					TAG_SITE_ROOT_PATH_EC_REPLACE_TEXT,
					TAG_SITE_ROOT_PATH_EC_DISPLAY_TEXT),
				new KeyValuePair<string, string>(
					TAG_SITE_ROOT_PATH_MP_REPLACE_TEXT,
					TAG_SITE_ROOT_PATH_MP_DISPLAY_TEXT),
				new KeyValuePair<string, string>(
					TAG_SITE_ROOT_PATH_CS_REPLACE_TEXT,
					TAG_SITE_ROOT_PATH_CS_DISPLAY_TEXT),
				new KeyValuePair<string, string>(
					TAG_SITE_ROOT_PATH_CMS_REPLACE_TEXT,
					TAG_SITE_ROOT_PATH_CMS_DISPLAY_TEXT),
			};
			return result;
		}

		/// <summary>サイトルートパス：PCサイト</summary>
		private string SiteRootPathPcFront { set; get; }
		/// <summary>サイトルートパス：EC管理画面</summary>
		private string SiteRootPathEc { set; get; }
		/// <summary>サイトルートパス：MP管理画面</summary>
		private string SiteRootPathMp { set; get; }
		/// <summary>サイトルートパス：CS管理画面</summary>
		private string SiteRootPathCs { set; get; }
		/// <summary>サイトルートパス：CMS管理画面</summary>
		private string SiteRootPathCms { set; get; }
		/// <summary>サイトのルートパスのメールタグをカンマ区切り</summary>
		public string SiteRootPathMailTag
		{
			get
			{
				return string.Format(
					"{0},{1},{2},{3},{4}",
					TAG_SITE_ROOT_PATH_FRONT_PC_REPLACE_TEXT,
					TAG_SITE_ROOT_PATH_EC_REPLACE_TEXT,
					TAG_SITE_ROOT_PATH_MP_REPLACE_TEXT,
					TAG_SITE_ROOT_PATH_CS_REPLACE_TEXT,
					TAG_SITE_ROOT_PATH_CMS_REPLACE_TEXT);
			}
		}
	}
}
