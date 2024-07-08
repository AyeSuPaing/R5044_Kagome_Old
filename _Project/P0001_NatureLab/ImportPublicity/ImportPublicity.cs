/*
=========================================================================================================
  Module      : P0001_NatureLab：新着情報XML取込み処理(ImportPublicity.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Application : w2.Commerce.Batch.ImportPublicity
  BaseVersion : V5.0
  Author      : M.Yoshimoto
  email       : product@w2solution.co.jp
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  URL         : http://www.w2solution.co.jp/
=========================================================================================================
PKG-V5.0[PF0118] 2011/04/25 M.Yoshimoto     P0001_NatureLab：パブリシティ取込機能対応
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Linq;
using System.Xml.Linq;
using w2.Common.Extensions;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Logger;
using w2.App.Common;
using w2.App.Common.Order;
using w2.Commerce.Batch.ImportPublicity;

namespace w2.Commerce.Batch.ImportPublicity
{
	public class ImportPublicity
	{
		// 置換タグ
		private const string TAG_FILEINFO_UPDATED_DATE_BGN = "<%@ FileInfo UpdatedDate=\"";
		private const string TAG_FILEINFO_UPDATED_DATE_END = "\" %>";
		private const string TAG_KEY_ENTRY_BEGIN = "<@@entries@@>";
		private const string TAG_KEY_ENTRY_END = "</@@entries@@>";
		private const string TAG_KEY_IMAGES_BEGIN = "<@@images@@>";
		private const string TAG_KEY_IMAGES_END = "</@@images@@>";
		private const string TAG_KEY_LINKS_BEGIN = "<@@links@@>";
		private const string TAG_KEY_LINKS_END = "</@@links@@>";
		private const string TAG_KEY_HAS_LINK_BEGIN = "<@@has_link@@>";
		private const string TAG_KEY_HAS_LINK_END = "</@@has_link@@>";
		private const string TAG_MOBILE_PUBLICITY_TOP_DISPLAY_AREA_BGN = "<!-- @@PublicityTopBgn@@ -->";
		private const string TAG_MOBILE_PUBLICITY_TOP_DISPLAY_AREA_END = "<!-- @@PublicityTopEnd@@ -->";

		private const int CONST_TOP_DISPLAY_COUNT_MAX = 10;					// 新着情報最大表示数（TOPページ）
		private const int CONST_TOP_DISPLAY_COUNT_MAX_SMARTPHONE = 3;		// 新着情報最大表示数（TOPページ）
		private const int CONST_TOP_DISPLAY_COUNT_MAX_MOBILE = 3;			// 新着情報最大表示数（TOPページ）
		private const int CONST_VIEW_DISPLAY_COUNT_MAX = 20;	// 新着情報最大表示数（一覧ページ）

		private IEnumerable<XElement> m_xeDetailElementsForView = null;		// 新着情報XML（一覧ページ用）
		private IEnumerable<XElement> m_xeDetailElementsForTop = null;		// 新着情報XML（TOPページ用）
		private Dictionary<string, int> m_dCountPerPublishMonth = new Dictionary<string, int>();	// 月毎の新着情報数集計

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		/// <param name="args">コマンドライン引数</param>
		static void Main(string[] args)
		{
			try
			{
				// 実体作成
				ImportPublicity program = new ImportPublicity();

				// バッチ起動をイベントログ出力
				AppLogger.WriteInfo("起動");

				// テスト用：第一コマンドライン引数が"update"の場合、強制的にパブリシティ系ページの更新を行う
				if ((args.Length >= 1) && (args[0].ToLower() == "update"))
				{
					program.Import(true);
				}
				else
				{
					program.Import(false);
				}

				// バッチ終了をイベントログ出力
				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImportPublicity()
		{
			// 初期化
			Iniitialize();
		}

		/// <summary>
		/// 設定初期化
		/// </summary>
		private void Iniitialize()
		{
			try
			{
				//------------------------------------------------------
				// SQL接続文字列設定
				//------------------------------------------------------
				Constants.STRING_SQL_CONNECTION = Properties.Settings.Default.SqlConnection;

				//------------------------------------------------------
				// アプリケーション設定読み込み
				//------------------------------------------------------
				// アプリケーション名設定
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;
				Constants.PHYSICALDIRPATH_LOGFILE = Properties.Settings.Default.Directory_LogFilePath;

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				// ディレクトリ・パス
				Constants.PHYSICALDIRPATH_SQL_STATEMENT = AppDomain.CurrentDomain.BaseDirectory + Constants.DIRPATH_XML_STATEMENTS;
				Constants.PC_PHYSICALDIRPATH_IMAGE_FILE = Properties.Settings.Default.PC_ImageFileDirPath;
				Constants.PC_PHYSICALDIRPATH_TEMPLATE_FILE = Properties.Settings.Default.PC_TemplateFileDirPath;
				Constants.PC_PHYSICALDIRPATH_PUBLICITY_FILE = Properties.Settings.Default.PC_PublicityFileDirPath;
				Constants.SMARTPHONE_PHYSICALDIRPATH_IMAGE_FILE = Properties.Settings.Default.SmartPhone_ImageFileDirPath;
				Constants.SMARTPHONE_PHYSICALDIRPATH_TEMPLATE_FILE = Properties.Settings.Default.SmartPhone_TemplateFileDirPath;
				Constants.SMARTPHONE_PHYSICALDIRPATH_PUBLICITY_FILE = Properties.Settings.Default.SmartPhone_PublicityFileDirPath;
				Constants.MOBILE_PHYSICALDIRPATH_IMAGE_FILE = Properties.Settings.Default.Mobile_ImageFileDirPath;
				Constants.PUBLICITY_SETTING_SITE_ID = Properties.Settings.Default.SiteId;
				Constants.PUBLICITY_SETTING_ACCESS_UPDATED_DATE_URL = Properties.Settings.Default.Publicity_ManagementServer_Access_UpdatedDateUrl;
				Constants.PUBLICITY_SETTING_ACCESS_PUBLICITY_XML_URL = Properties.Settings.Default.Publicity_ManagementServer_Access_PublicityXmlUrl;

				// モバイルページID
				Constants.MOBILE_PUBLICITY_VIEW_TEMPLATE_PAGE_ID = Properties.Settings.Default.Mobile_PublicityViewTemplate_PageID;
				Constants.MOBILE_PUBLICITY_ARCHIVE_TEMPLATE_PAGE_ID = Properties.Settings.Default.Mobile_PublicityArchiveTemplate_PageID;
				Constants.MOBILE_PUBLICITY_VIEW_PAGE_ID = Properties.Settings.Default.Mobile_PublicityView_PageID;
				Constants.MOBILE_PUBLICITY_ARCHIVE_PAGE_ID = Properties.Settings.Default.Mobile_PublicityArchive_PageID;
				Constants.MOBILE_PUBLICITY_BACKNUMBER_TEMPLATE_PAGE_ID = Properties.Settings.Default.Mobile_PublicityBackNumberTemplate_PageID;
				Constants.MOBILE_SITETOP_PAGE_ID = Properties.Settings.Default.Mobile_SiteTop_PageID;
				Constants.MOBILE_PUBLICITY_TOP_TEMPLATE_PAGE_ID = Properties.Settings.Default.Mobile_PublicityTopTemplate_PageID;
			}
			catch (Exception ex)
			{
				throw new ApplicationException("設定値の読み込みに失敗しました。\r\n" + ex.ToString());
			}
		}

		/// <summary>
		/// 新着情報XML取込み
		/// </summary>
		/// <param name="blForceUpdate">強制更新フラグ</param>
		private void Import(bool blForceUpdate)
		{
			//------------------------------------------------------
			// 管理サイト側、最終更新日取得
			//------------------------------------------------------
			StringBuilder sbRequestUrl = new StringBuilder();
			DateTime dtOriginalUpdatedDate = new DateTime();

			// 管理サイト側の新着情報"最終更新日"の取得URL作成
			sbRequestUrl.Append(Constants.PUBLICITY_SETTING_ACCESS_UPDATED_DATE_URL);
			sbRequestUrl.Append("?").Append("id_site").Append("=").Append(Constants.PUBLICITY_SETTING_SITE_ID);

			HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(sbRequestUrl.ToString());

			// "最終更新日"取得
			using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
			using (Stream responseStream = (webResponse.GetResponseStream()))
			using (StreamReader sr = new StreamReader(responseStream, Encoding.UTF8))
			{
				dtOriginalUpdatedDate = DateTime.Parse(sr.ReadToEnd());
			}

			//------------------------------------------------------
			// ｗ２サイト側、新着情報コンテンツの最終更新日取得
			//------------------------------------------------------
			string strTargetFilePath = Constants.PC_PHYSICALDIRPATH_PUBLICITY_FILE + "index.aspx";
			string strPublicityViewContents = null;
			DateTime dtContentsUpdatedDate = new DateTime(1900,1,1);	// 初期値は1900年1月1日に設定

			if (File.Exists(strTargetFilePath))
			{
				// 新着情報一覧ページ読込
				using (StreamReader sr = new StreamReader(strTargetFilePath, Encoding.UTF8))
				{
					strPublicityViewContents = sr.ReadToEnd();
				}

				// ファイル最終更新日時取得（※ファイルの更新TIMESTAMPでは判断しない）
				dtContentsUpdatedDate = DateTime.Parse(Regex.Match(strPublicityViewContents, TAG_FILEINFO_UPDATED_DATE_BGN + ".*?" + TAG_FILEINFO_UPDATED_DATE_END).Value.Replace(TAG_FILEINFO_UPDATED_DATE_BGN, "").Replace(TAG_FILEINFO_UPDATED_DATE_END, ""));
			}

			// ｗ２側新着情報ページの更新日時より、管理サイトの新着情報の更新日時が新しい場合に新着情報ページを再生成
			if (blForceUpdate || (dtOriginalUpdatedDate > dtContentsUpdatedDate))
			{
				// PC画像ディレクトリが存在しない場合は、新たに作成
				if (Directory.Exists(Constants.PC_PHYSICALDIRPATH_IMAGE_FILE) == false)
				{
					Directory.CreateDirectory(Constants.PC_PHYSICALDIRPATH_IMAGE_FILE);
				}
				// PCパブリシティ出力ディレクトリが存在しない場合は、新たに作成
				if (Directory.Exists(Constants.PC_PHYSICALDIRPATH_PUBLICITY_FILE) == false)
				{
					Directory.CreateDirectory(Constants.PC_PHYSICALDIRPATH_PUBLICITY_FILE);
				}
				// SmartPhone画像ディレクトリが存在しない場合は、新たに作成
				if (Directory.Exists(Constants.SMARTPHONE_PHYSICALDIRPATH_IMAGE_FILE) == false)
				{
					Directory.CreateDirectory(Constants.SMARTPHONE_PHYSICALDIRPATH_IMAGE_FILE);
				}
				// PCパブリシティ出力ディレクトリが存在しない場合は、新たに作成
				if (Directory.Exists(Constants.SMARTPHONE_PHYSICALDIRPATH_PUBLICITY_FILE) == false)
				{
					Directory.CreateDirectory(Constants.SMARTPHONE_PHYSICALDIRPATH_PUBLICITY_FILE);
				}
				// モバイル画像ディレクトリが存在しない場合は、新たに作成
				if (Directory.Exists(Constants.MOBILE_PHYSICALDIRPATH_IMAGE_FILE) == false)
				{
					Directory.CreateDirectory(Constants.MOBILE_PHYSICALDIRPATH_IMAGE_FILE);
				}

				//------------------------------------------------------
				// 管理サイトから新着情報XML取得
				//------------------------------------------------------
				sbRequestUrl = new StringBuilder();
				XDocument xDocument = null;

				// 管理サイト側の新着情報XMLの取得URL作成
				sbRequestUrl.Append(Constants.PUBLICITY_SETTING_ACCESS_PUBLICITY_XML_URL);
				sbRequestUrl.Append("?").Append("id_site").Append("=").Append(Constants.PUBLICITY_SETTING_SITE_ID);

				webRequest = (HttpWebRequest)WebRequest.Create(sbRequestUrl.ToString());

				// 新着情報XML取得
				using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
				using (Stream responseStream = (webResponse.GetResponseStream()))
				using (StreamReader sr = new StreamReader(responseStream, Encoding.UTF8))
				{
					xDocument = XDocument.Load(sr);

					// 新着情報取得（一覧、アーカイブページ）
					m_xeDetailElementsForView = (
						from detailElement in xDocument.Descendants("detail")
						where detailElement.Element("del_flg").Value != Constants.KBN_PUBLICITY_DELETE_FLG_DELETED
						orderby detailElement.Element("pub_date").Value descending
						select detailElement);

					// 新着情報取得（TOPページ）
					m_xeDetailElementsForTop = (
						from detaiElement in xDocument.Descendants("detail")
						where (detaiElement.Element("del_flg").Value != Constants.KBN_PUBLICITY_DELETE_FLG_DELETED
							&& detaiElement.Element("fixity_flg").Value == Constants.KBN_PUBLICITY_FIXITY_FLG_FIXED)
						orderby detaiElement.Element("pub_date").Value descending
						select detaiElement).Union<XElement>(
						from detaiElement in xDocument.Descendants("detail")
						where (detaiElement.Element("del_flg").Value != Constants.KBN_PUBLICITY_DELETE_FLG_DELETED
							&& detaiElement.Element("fixity_flg").Value == Constants.KBN_PUBLICITY_FIXITY_FLG_UNFIXED)
						orderby detaiElement.Element("pub_date").Value descending
						select detaiElement);
				}

				//------------------------------------------------------
				// 発行月毎の新着情報数集計
				//------------------------------------------------------
				string strPublishMonth = null;

				foreach (XElement xeDetailElement in m_xeDetailElementsForView)
				{
					strPublishMonth = DateTime.ParseExact(xeDetailElement.Element("pub_date").Value, "yyyy-MM-dd", null).ToString("yyyyMM");

					if (m_dCountPerPublishMonth.ContainsKey(strPublishMonth))
					{
						m_dCountPerPublishMonth[strPublishMonth]++;
					}
					else
					{
						m_dCountPerPublishMonth.Add(strPublishMonth, 1);
					}
				}

				//------------------------------------------------------
				// 画像ファイルの生成（画像ファイルが存在しない場合、URLから画像データ取得⇒ファイル生成）
				//------------------------------------------------------
				foreach (XElement xeDetailElement in m_xeDetailElementsForView)
				{
					if((string.IsNullOrEmpty(xeDetailElement.Element("id_attachment1").Value) == false)
						&& (string.IsNullOrEmpty(xeDetailElement.Element("file_name1").Value) == false))
					{
						// PC
						if (File.Exists(Constants.PC_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name1").Value) == false)
						{
							CreateImageFileFromStream(xeDetailElement.Element("id_attachment1").Value, Constants.PC_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name1").Value);
						}
						// SmartPhone
						if (File.Exists(Constants.SMARTPHONE_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name1").Value) == false)
						{
							CreateImageFileFromStream(xeDetailElement.Element("id_attachment1").Value, Constants.SMARTPHONE_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name1").Value);
						}
						// モバイル
						if (File.Exists(Constants.MOBILE_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name1").Value) == false)
						{
							CreateImageFileFromStream(xeDetailElement.Element("id_attachment1").Value, Constants.MOBILE_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name1").Value);
						}
					}
					if((string.IsNullOrEmpty(xeDetailElement.Element("thumbnail1").Value) == false)
						&& (string.IsNullOrEmpty(xeDetailElement.Element("file_name_thum1").Value) == false))
					{
						// PC
						if (File.Exists(Constants.PC_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name_thum1").Value) == false)
						{
							CreateImageFileFromStream(xeDetailElement.Element("thumbnail1").Value, Constants.PC_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name_thum1").Value);
						}
						// SmartPhone
						if (File.Exists(Constants.SMARTPHONE_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name_thum1").Value) == false)
						{
							CreateImageFileFromStream(xeDetailElement.Element("thumbnail1").Value, Constants.SMARTPHONE_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name_thum1").Value);
						}
						// モバイル
						if (File.Exists(Constants.MOBILE_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name_thum1").Value) == false)
						{
							CreateImageFileFromStream(xeDetailElement.Element("thumbnail1").Value, Constants.MOBILE_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name_thum1").Value);
						}
					}
					if((string.IsNullOrEmpty(xeDetailElement.Element("id_attachment2").Value) == false)
						&& (string.IsNullOrEmpty(xeDetailElement.Element("file_name2").Value) == false))
					{
						// PC
						if (File.Exists(Constants.PC_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name2").Value) == false)
						{
							CreateImageFileFromStream(xeDetailElement.Element("id_attachment2").Value, Constants.PC_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name2").Value);
						}
						// SmartPhone
						if (File.Exists(Constants.SMARTPHONE_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name2").Value) == false)
						{
							CreateImageFileFromStream(xeDetailElement.Element("id_attachment2").Value, Constants.SMARTPHONE_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name2").Value);
						}
						// モバイル
						if (File.Exists(Constants.MOBILE_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name2").Value) == false)
						{
							CreateImageFileFromStream(xeDetailElement.Element("id_attachment2").Value, Constants.MOBILE_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name2").Value);
						}
					}
					if((string.IsNullOrEmpty(xeDetailElement.Element("thumbnail2").Value) == false)
						&& (string.IsNullOrEmpty(xeDetailElement.Element("file_name_thum2").Value) == false))
					{
						// PC
						if (File.Exists(Constants.PC_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name_thum2").Value) == false)
						{
							CreateImageFileFromStream(xeDetailElement.Element("thumbnail2").Value, Constants.PC_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name_thum2").Value);
						}
						// SAMRTPHONE
						if (File.Exists(Constants.SMARTPHONE_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name_thum2").Value) == false)
						{
							CreateImageFileFromStream(xeDetailElement.Element("thumbnail2").Value, Constants.SMARTPHONE_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name_thum2").Value);
						}
						// モバイル
						if (File.Exists(Constants.MOBILE_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name_thum2").Value) == false)
						{
							CreateImageFileFromStream(xeDetailElement.Element("thumbnail2").Value, Constants.MOBILE_PHYSICALDIRPATH_IMAGE_FILE + xeDetailElement.Element("file_name_thum2").Value);
						}
					}
				}

				// PC新着情報ページ生成
				CreatePublicityContentsInPC(dtOriginalUpdatedDate);

				// SmartPhone新着情報ページ生成
				CreatePublicityContentsInSmartPhone(dtOriginalUpdatedDate);

				// Mobile新着情報ページ生成
				CreatePublicityContentsInMobile();

				//------------------------------------------------------
				// 取込み新着情報XML保存
				//------------------------------------------------------
				File.WriteAllText(Constants.PHYSICALDIRPATH_LOGFILE + "ImportedPublicity_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".xml", xDocument.ToString(), Encoding.UTF8);
			}
			else
			{
				FileLogger.WriteInfo("パブリシティの更新がないため、処理終了");
			}
		}

		/// <summary>
		/// PC：新着情報ページ作成
		/// </summary>
		/// <param name="dtOriginalUpdatedDate">最終更新日時（管理サイト）</param>
		private void CreatePublicityContentsInPC(DateTime dtOriginalUpdatedDate)
		{
			//------------------------------------------------------
			// 新着情報テンプレートファイル取得
			//------------------------------------------------------
			// テンプレートファイルが存在しない場合は、処理終了
			if ((File.Exists(Constants.PC_PHYSICALDIRPATH_TEMPLATE_FILE + "template_index.aspx") == false)
				|| (File.Exists(Constants.PC_PHYSICALDIRPATH_TEMPLATE_FILE + "template_backnumber.ascx") == false)
				|| (File.Exists(Constants.PC_PHYSICALDIRPATH_TEMPLATE_FILE + "template_yyyymm.aspx") == false))
			{
				throw new Exception("テンプレートファイルが指定フォルダに存在しないため、処理終了。");
			}

			// template_top.ascxのコンテンツ取得
			string strTopContents = File.ReadAllText(Constants.PC_PHYSICALDIRPATH_TEMPLATE_FILE + "template_top.ascx");
			// template_index.aspxのコンテンツ取得
			string strIndexContents = File.ReadAllText(Constants.PC_PHYSICALDIRPATH_TEMPLATE_FILE + "template_index.aspx");
			// template_backnumber.ascxのコンテンツ取得
			string strBacknumberContents = File.ReadAllText(Constants.PC_PHYSICALDIRPATH_TEMPLATE_FILE + "template_backnumber.ascx");
			// template_yyyymm.aspxのコンテンツ取得
			string strYyyymmContents = File.ReadAllText(Constants.PC_PHYSICALDIRPATH_TEMPLATE_FILE + "template_yyyymm.aspx");

			//------------------------------------------------------
			// TOPページのタグ置換（top.aspx）
			//------------------------------------------------------
			StringBuilder sbEntries = new StringBuilder();
			int iPublicityCountForTOP = 0;

			if (Regex.Match(strTopContents, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value.Length > 0)
			{
				foreach (XElement xeDetailElementForTop in m_xeDetailElementsForTop)
				{
					// 記事数がTOP画面の最大表示件数に達した場合、ループを抜ける
					if (iPublicityCountForTOP >= CONST_TOP_DISPLAY_COUNT_MAX)
					{
						break;
					}

					sbEntries.Append(CreatePublicityEntryForTop(GetTagInnerContents(strTopContents, TAG_KEY_ENTRY_BEGIN, TAG_KEY_ENTRY_END), xeDetailElementForTop));

					iPublicityCountForTOP++;
				}

				strTopContents = strTopContents.Replace(Regex.Match(strTopContents, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value, sbEntries.ToString());
			}

			//------------------------------------------------------
			// 新着情報一覧ページのタグ置換（index.aspx）
			//------------------------------------------------------
			sbEntries = new StringBuilder();
			int iPublicityCountForView = 0;

			if (Regex.Match(strIndexContents, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value.Length > 0)
			{
				foreach (XElement xeDetailElement in m_xeDetailElementsForView)
				{
					// 記事数が一覧画面の最大表示件数に達した場合、ループを抜ける
					if (iPublicityCountForView >= CONST_VIEW_DISPLAY_COUNT_MAX)
					{
						break;
					}

					sbEntries.Append(CreatePublicityEntryForView(GetTagInnerContents(strIndexContents, TAG_KEY_ENTRY_BEGIN, TAG_KEY_ENTRY_END), xeDetailElement));
					iPublicityCountForView++;
				}

				strIndexContents = strIndexContents.Replace(Regex.Match(strIndexContents, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value, sbEntries.ToString());
			}

			// 最終更新日時記録
			strIndexContents = strIndexContents.Replace(
				Regex.Match(strIndexContents, TAG_FILEINFO_UPDATED_DATE_BGN + ".*?" + TAG_FILEINFO_UPDATED_DATE_END).Value,
				TAG_FILEINFO_UPDATED_DATE_BGN + dtOriginalUpdatedDate.ToString() + TAG_FILEINFO_UPDATED_DATE_END);

			//------------------------------------------------------
			// バックナンバーページのタグ置換（backnumber.ascx）
			//------------------------------------------------------
			sbEntries = new StringBuilder();

			if (Regex.Match(strBacknumberContents, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value.Length > 0)
			{
				foreach (string strKey in m_dCountPerPublishMonth.Keys)
				{
					sbEntries.Append(CreatePublicityEntryForBackNumber(GetTagInnerContents(strBacknumberContents, TAG_KEY_ENTRY_BEGIN, TAG_KEY_ENTRY_END), strKey, m_dCountPerPublishMonth[strKey]));
				}

				strBacknumberContents = strBacknumberContents.Replace(
				Regex.Match(strBacknumberContents, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value, sbEntries.ToString());
			}

			//------------------------------------------------------
			// 新着情報アーカイブページのタグ置換（yyyymm.html）
			//------------------------------------------------------
			// 発行月毎にループし、アーカイブファイルを作成
			foreach (string strKey in m_dCountPerPublishMonth.Keys)
			{
				// 初期化
				sbEntries = new StringBuilder();
				strYyyymmContents = File.ReadAllText(Constants.PC_PHYSICALDIRPATH_TEMPLATE_FILE + "template_yyyymm.aspx");

				if (Regex.Match(strYyyymmContents, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value.Length > 0)
				{
					foreach (XElement xeDetailElement in m_xeDetailElementsForView)
					{
						if (strKey == DateTime.ParseExact(xeDetailElement.Element("pub_date").Value, "yyyy-MM-dd", null).ToString("yyyyMM"))
						{
							sbEntries.Append(CreatePublicityEntryForArchive(GetTagInnerContents(strYyyymmContents, TAG_KEY_ENTRY_BEGIN, TAG_KEY_ENTRY_END), xeDetailElement));
						}
					}

					strYyyymmContents = strYyyymmContents.Replace(Regex.Match(strYyyymmContents, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value, sbEntries.ToString());
				}

				// month_jタグ置換
				strYyyymmContents = strYyyymmContents.Replace("@@month_j@@", strKey.Substring(0, 4) + "年" + strKey.Substring(4, 2) + "月");

				// ファイル更新
				File.WriteAllText(Constants.PC_PHYSICALDIRPATH_PUBLICITY_FILE + strKey + ".aspx", strYyyymmContents, Encoding.UTF8);
			}

			//------------------------------------------------------
			// ファイル作成/更新
			//------------------------------------------------------
			File.WriteAllText(Constants.PC_PHYSICALDIRPATH_PUBLICITY_FILE + "top.ascx", strTopContents, Encoding.UTF8);
			File.WriteAllText(Constants.PC_PHYSICALDIRPATH_PUBLICITY_FILE + "index.aspx", strIndexContents, Encoding.UTF8);
			File.WriteAllText(Constants.PC_PHYSICALDIRPATH_PUBLICITY_FILE + "backnumber.ascx", strBacknumberContents, Encoding.UTF8);
		}

		/// <summary>
		/// SmartPhone：新着情報ページ作成
		/// </summary>
		/// <param name="dtOriginalUpdatedDate">最終更新日時（管理サイト）</param>
		private void CreatePublicityContentsInSmartPhone(DateTime dtOriginalUpdatedDate)
		{
			//------------------------------------------------------
			// 新着情報テンプレートファイル取得
			//------------------------------------------------------
			// テンプレートファイルが存在しない場合は、処理終了
			if ((File.Exists(Constants.SMARTPHONE_PHYSICALDIRPATH_TEMPLATE_FILE + "template_index.aspx") == false)
				|| (File.Exists(Constants.SMARTPHONE_PHYSICALDIRPATH_TEMPLATE_FILE + "template_backnumber.ascx") == false)
				|| (File.Exists(Constants.SMARTPHONE_PHYSICALDIRPATH_TEMPLATE_FILE + "template_yyyymm.aspx") == false))
			{
				throw new Exception("テンプレートファイルが指定フォルダに存在しないため、処理終了。");
			}

			// template_top.ascxのコンテンツ取得
			string strTopContents = File.ReadAllText(Constants.SMARTPHONE_PHYSICALDIRPATH_TEMPLATE_FILE + "template_top.ascx");
			// template_index.aspxのコンテンツ取得
			string strIndexContents = File.ReadAllText(Constants.SMARTPHONE_PHYSICALDIRPATH_TEMPLATE_FILE + "template_index.aspx");
			// template_backnumber.ascxのコンテンツ取得
			string strBacknumberContents = File.ReadAllText(Constants.SMARTPHONE_PHYSICALDIRPATH_TEMPLATE_FILE + "template_backnumber.ascx");
			// template_yyyymm.aspxのコンテンツ取得
			string strYyyymmContents = File.ReadAllText(Constants.SMARTPHONE_PHYSICALDIRPATH_TEMPLATE_FILE + "template_yyyymm.aspx");

			//------------------------------------------------------
			// TOPページのタグ置換（top.aspx）
			//------------------------------------------------------
			StringBuilder sbEntries = new StringBuilder();
			int iPublicityCountForTOP = 0;

			if (Regex.Match(strTopContents, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value.Length > 0)
			{
				foreach (XElement xeDetailElementForTop in m_xeDetailElementsForTop)
				{
					// 記事数がTOP画面の最大表示件数に達した場合、ループを抜ける
					if (iPublicityCountForTOP >= CONST_TOP_DISPLAY_COUNT_MAX_SMARTPHONE)
					{
						break;
					}

					sbEntries.Append(CreatePublicityEntryForTop(GetTagInnerContents(strTopContents, TAG_KEY_ENTRY_BEGIN, TAG_KEY_ENTRY_END), xeDetailElementForTop));

					iPublicityCountForTOP++;
				}

				strTopContents = strTopContents.Replace(Regex.Match(strTopContents, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value, sbEntries.ToString());
			}

			//------------------------------------------------------
			// 新着情報一覧ページのタグ置換（index.aspx）
			//------------------------------------------------------
			sbEntries = new StringBuilder();
			int iPublicityCountForView = 0;

			if (Regex.Match(strIndexContents, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value.Length > 0)
			{
				foreach (XElement xeDetailElement in m_xeDetailElementsForView)
				{
					// 記事数が一覧画面の最大表示件数に達した場合、ループを抜ける
					if (iPublicityCountForView >= CONST_VIEW_DISPLAY_COUNT_MAX)
					{
						break;
					}

					sbEntries.Append(CreatePublicityEntryForView(GetTagInnerContents(strIndexContents, TAG_KEY_ENTRY_BEGIN, TAG_KEY_ENTRY_END), xeDetailElement));
					iPublicityCountForView++;
				}

				strIndexContents = strIndexContents.Replace(Regex.Match(strIndexContents, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value, sbEntries.ToString());
			}

			// 最終更新日時記録
			strIndexContents = strIndexContents.Replace(
				Regex.Match(strIndexContents, TAG_FILEINFO_UPDATED_DATE_BGN + ".*?" + TAG_FILEINFO_UPDATED_DATE_END).Value,
				TAG_FILEINFO_UPDATED_DATE_BGN + dtOriginalUpdatedDate.ToString() + TAG_FILEINFO_UPDATED_DATE_END);

			//------------------------------------------------------
			// バックナンバーページのタグ置換（backnumber.ascx）
			//------------------------------------------------------
			sbEntries = new StringBuilder();

			if (Regex.Match(strBacknumberContents, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value.Length > 0)
			{
				foreach (string strKey in m_dCountPerPublishMonth.Keys)
				{
					sbEntries.Append(CreatePublicityEntryForBackNumber(GetTagInnerContents(strBacknumberContents, TAG_KEY_ENTRY_BEGIN, TAG_KEY_ENTRY_END), strKey, m_dCountPerPublishMonth[strKey]));
				}

				strBacknumberContents = strBacknumberContents.Replace(
				Regex.Match(strBacknumberContents, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value, sbEntries.ToString());
			}

			//------------------------------------------------------
			// 新着情報アーカイブページのタグ置換（yyyymm.html）
			//------------------------------------------------------
			// 発行月毎にループし、アーカイブファイルを作成
			foreach (string strKey in m_dCountPerPublishMonth.Keys)
			{
				// 初期化
				sbEntries = new StringBuilder();
				strYyyymmContents = File.ReadAllText(Constants.SMARTPHONE_PHYSICALDIRPATH_TEMPLATE_FILE + "template_yyyymm.aspx");

				if (Regex.Match(strYyyymmContents, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value.Length > 0)
				{
					foreach (XElement xeDetailElement in m_xeDetailElementsForView)
					{
						if (strKey == DateTime.ParseExact(xeDetailElement.Element("pub_date").Value, "yyyy-MM-dd", null).ToString("yyyyMM"))
						{
							sbEntries.Append(CreatePublicityEntryForArchive(GetTagInnerContents(strYyyymmContents, TAG_KEY_ENTRY_BEGIN, TAG_KEY_ENTRY_END), xeDetailElement));
						}
					}

					strYyyymmContents = strYyyymmContents.Replace(Regex.Match(strYyyymmContents, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value, sbEntries.ToString());
				}

				// month_jタグ置換
				strYyyymmContents = strYyyymmContents.Replace("@@month_j@@", strKey.Substring(0, 4) + "年" + strKey.Substring(4, 2) + "月");

				// ファイル更新
				File.WriteAllText(Constants.SMARTPHONE_PHYSICALDIRPATH_PUBLICITY_FILE + strKey + ".aspx", strYyyymmContents, Encoding.UTF8);
			}

			//------------------------------------------------------
			// ファイル作成/更新
			//------------------------------------------------------
			File.WriteAllText(Constants.SMARTPHONE_PHYSICALDIRPATH_PUBLICITY_FILE + "top.ascx", strTopContents, Encoding.UTF8);
			File.WriteAllText(Constants.SMARTPHONE_PHYSICALDIRPATH_PUBLICITY_FILE + "index.aspx", strIndexContents, Encoding.UTF8);
			File.WriteAllText(Constants.SMARTPHONE_PHYSICALDIRPATH_PUBLICITY_FILE + "backnumber.ascx", strBacknumberContents, Encoding.UTF8);
		}


		/// <summary>
		/// Mobile：新着情報ページ作成
		/// </summary>
		private void CreatePublicityContentsInMobile()
		{
			//------------------------------------------------------
			// 新着情報テンプレートページのデータ取得
			//------------------------------------------------------
			// テンプレートファイルが存在しない場合は、処理終了
			if ((GetMobilePageData(Constants.MOBILE_PUBLICITY_TOP_TEMPLATE_PAGE_ID).Count <= 0)
				|| (GetMobilePageData(Constants.MOBILE_PUBLICITY_VIEW_TEMPLATE_PAGE_ID).Count <= 0)
				|| (GetMobilePageData(Constants.MOBILE_PUBLICITY_ARCHIVE_TEMPLATE_PAGE_ID).Count <= 0)
				|| (GetMobilePageData(Constants.MOBILE_PUBLICITY_BACKNUMBER_TEMPLATE_PAGE_ID).Count <= 0))
			{
				throw new Exception("テンプレートモバイルページが指定フォルダに存在しないため、処理終了。");
			}

			// TOP新着情報テンプレート
			string strMobileTOPTemplate = GetMobilePageData(Constants.MOBILE_PUBLICITY_TOP_TEMPLATE_PAGE_ID)[0][Constants.FIELD_MOBILEPAGE_HTML].ToString();
			// 新着情報一覧テンプレート
			string strMobileViewTemplate = GetMobilePageData(Constants.MOBILE_PUBLICITY_VIEW_TEMPLATE_PAGE_ID)[0][Constants.FIELD_MOBILEPAGE_HTML].ToString();
			// 新着情報アーカイブテンプレート
			string strMobileArchiveTemplate = GetMobilePageData(Constants.MOBILE_PUBLICITY_ARCHIVE_TEMPLATE_PAGE_ID)[0][Constants.FIELD_MOBILEPAGE_HTML].ToString();
			// バックナンバーテンプレート
			string strMobileBackNumberTemplate = GetMobilePageData(Constants.MOBILE_PUBLICITY_BACKNUMBER_TEMPLATE_PAGE_ID)[0][Constants.FIELD_MOBILEPAGE_HTML].ToString();

			//------------------------------------------------------
			// TOPページのタグ置換
			//------------------------------------------------------
			StringBuilder sbEntries = new StringBuilder();
			int iPublicityCountForTOP = 0;

			if (Regex.Match(strMobileTOPTemplate, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value.Length > 0)
			{
				foreach (XElement xeDetailElementForTop in m_xeDetailElementsForTop)
				{
					// 記事数がTOP画面の最大表示件数に達した場合、ループを抜ける
					if (iPublicityCountForTOP >= CONST_TOP_DISPLAY_COUNT_MAX)
					{
						break;
					}

					sbEntries.Append(CreatePublicityEntryForTop(GetTagInnerContents(strMobileTOPTemplate, TAG_KEY_ENTRY_BEGIN, TAG_KEY_ENTRY_END), xeDetailElementForTop));
					iPublicityCountForTOP++;
				}

				strMobileTOPTemplate = strMobileTOPTemplate.Replace(Regex.Match(strMobileTOPTemplate, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value, sbEntries.ToString());
			}

			string strTOPContents = GetMobilePageData(Constants.MOBILE_SITETOP_PAGE_ID)[0][Constants.FIELD_MOBILEPAGE_HTML].ToString();

			strTOPContents = (Regex.Match(strTOPContents, TAG_MOBILE_PUBLICITY_TOP_DISPLAY_AREA_BGN + "(.|\n)*?" + TAG_MOBILE_PUBLICITY_TOP_DISPLAY_AREA_END).Value.Length > 0)
				? strTOPContents.Replace(
					Regex.Match(strTOPContents, TAG_MOBILE_PUBLICITY_TOP_DISPLAY_AREA_BGN + "(.|\n)*?" + TAG_MOBILE_PUBLICITY_TOP_DISPLAY_AREA_END).Value,
					TAG_MOBILE_PUBLICITY_TOP_DISPLAY_AREA_BGN + strMobileTOPTemplate + TAG_MOBILE_PUBLICITY_TOP_DISPLAY_AREA_END)
				: strTOPContents;

			//------------------------------------------------------
			// バックナンバーページのタグ置換
			//------------------------------------------------------
			sbEntries = new StringBuilder();

			if (Regex.Match(strMobileBackNumberTemplate, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value.Length > 0)
			{
				foreach (string strKey in m_dCountPerPublishMonth.Keys)
				{
					sbEntries.Append(CreatePublicityEntryForBackNumber(GetTagInnerContents(strMobileBackNumberTemplate, TAG_KEY_ENTRY_BEGIN, TAG_KEY_ENTRY_END), strKey, m_dCountPerPublishMonth[strKey]));
				}

				strMobileBackNumberTemplate = strMobileBackNumberTemplate.Replace(Regex.Match(strMobileBackNumberTemplate, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value, sbEntries.ToString());
			}

			//------------------------------------------------------
			// 新着情報一覧ページのタグ置換
			//------------------------------------------------------
			sbEntries = new StringBuilder();
			int iPublicityCountForView = 0;

			if (Regex.Match(strMobileViewTemplate, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value.Length > 0)
			{
				foreach (XElement xeDetailElement in m_xeDetailElementsForView)
				{
					// 記事数が一覧画面の最大表示件数に達した場合、ループを抜ける
					if (iPublicityCountForView >= CONST_VIEW_DISPLAY_COUNT_MAX)
					{
						break;
					}

					sbEntries.Append(CreatePublicityEntryForView(GetTagInnerContents(strMobileViewTemplate, TAG_KEY_ENTRY_BEGIN, TAG_KEY_ENTRY_END), xeDetailElement));
					iPublicityCountForView++;
				}

				strMobileViewTemplate = strMobileViewTemplate.Replace(Regex.Match(strMobileViewTemplate, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value, sbEntries.ToString());
			}

			// バックナンバータグ置換
			strMobileViewTemplate = strMobileViewTemplate.Replace("<@@BackNumber@@>", strMobileBackNumberTemplate);

			//------------------------------------------------------
			// 新着情報アーカイブページのタグ置換
			//------------------------------------------------------
			sbEntries = new StringBuilder();
			string strArchivePageName = null;

			// 発行月毎にループし、アーカイブファイルを作成
			foreach (string strKey in m_dCountPerPublishMonth.Keys)
			{
				// 初期化
				sbEntries = new StringBuilder();
				strMobileArchiveTemplate = GetMobilePageData(Constants.MOBILE_PUBLICITY_ARCHIVE_TEMPLATE_PAGE_ID)[0][Constants.FIELD_MOBILEPAGE_HTML].ToString();

				if (Regex.Match(strMobileArchiveTemplate, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value.Length > 0)
				{
					foreach (XElement xeDetailElement in m_xeDetailElementsForView)
					{
						if (strKey == DateTime.ParseExact(xeDetailElement.Element("pub_date").Value, "yyyy-MM-dd", null).ToString("yyyyMM"))
						{
							sbEntries.Append(CreatePublicityEntryForArchive(GetTagInnerContents(strMobileArchiveTemplate, TAG_KEY_ENTRY_BEGIN, TAG_KEY_ENTRY_END), xeDetailElement));
						}
					}

					strMobileArchiveTemplate = strMobileArchiveTemplate.Replace(Regex.Match(strMobileArchiveTemplate, TAG_KEY_ENTRY_BEGIN + "(.|\n)*?" + TAG_KEY_ENTRY_END).Value, sbEntries.ToString());
				}

				// month_jタグ置換
				strMobileArchiveTemplate = strMobileArchiveTemplate.Replace("@@month_j@@", strKey.Substring(0, 4) + "年" + strKey.Substring(4, 2) + "月");

				// バックナンバータグ置換
				strMobileArchiveTemplate = strMobileArchiveTemplate.Replace("<@@BackNumber@@>", strMobileBackNumberTemplate);

				// モバイルページID置換
				strArchivePageName = Constants.MOBILE_PUBLICITY_ARCHIVE_PAGE_ID.Replace("@@yyyyMM@@", strKey);

				// 新着情報ページ生成
				InsertUpdateMobilePageData(strArchivePageName, strMobileArchiveTemplate, "アーカイブページ（" + strKey.Substring(0, 4) + "年" + strKey.Substring(4, 2) + "月" + "）");
			}

			//------------------------------------------------------
			// 新着情報ページ生成
			//------------------------------------------------------
			InsertUpdateMobilePageData(Constants.MOBILE_SITETOP_PAGE_ID, strTOPContents, "");
			InsertUpdateMobilePageData(Constants.MOBILE_PUBLICITY_VIEW_PAGE_ID, strMobileViewTemplate, "パブリシティ一覧ページ");
		}

		/// <summary>
		/// ストリームから画像ファイル作成
		/// </summary>
		/// <param name="strRequestURL">リクエストURL</param>
		/// <param name="strImageFilePath">画像保存先パス</param>
		private void CreateImageFileFromStream(string strRequestURL, string strImageFilePath)
		{
			HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(strRequestURL.Replace("https", "http"));

			// 画像データ取得
			using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
			using (Stream responseStream = (webResponse.GetResponseStream()))
			using (Image iImage = Image.FromStream(responseStream))
			{
				// 画像ファイル保存
				iImage.Save(strImageFilePath);
			}
		}

		/// <summary>
		/// 新着情報エントリー作成（TOPページ）
		/// </summary>
		/// <param name="strEntry">新着情報エントリー</param>
		/// <param name="xeDetailElement">新着情報XML要素</param>
		private string CreatePublicityEntryForTop(string strEntry, XElement xeDetailElement)
		{
			// pub_dateタグ置換
			strEntry = strEntry.Replace("@@pub_date@@", DateTime.ParseExact(xeDetailElement.Element("pub_date").Value, "yyyy-MM-dd", null).ToString("yy.MM.dd"));

			// divisionタグ置換
			string strIconName = "";
			switch (xeDetailElement.Element("division").Value)
			{
				case Constants.KBN_PUBLICITY_DIVISION_NEW_ITEM:
					strIconName = "ico_new.gif";		// 新商品
					break;
				case Constants.KBN_PUBLICITY_DIVISION_MEDIA:
					strIconName = "ico_media.gif";		// メディア
					break;
				case Constants.KBN_PUBLICITY_DIVISION_CAMPAIGN:
					strIconName = "ico_campaign.gif";	// キャンペーン
					break;
				case Constants.KBN_PUBLICITY_DIVISION_ANNOUNCEMENT:
					strIconName = "ico_info.gif";		// お知らせ
					break;
				case Constants.KBN_PUBLICITY_DIVISION_OTHER:
					strIconName = "ico_other.gif";		// その他
					break;
				default:
					strIconName = "ico_other.gif";		// その他
					break;
			}
			strEntry = strEntry.Replace("@@division@@", strIconName);

			// monタグ置換
			strEntry = strEntry.Replace("@@mon@@", DateTime.ParseExact(xeDetailElement.Element("pub_date").Value, "yyyy-MM-dd", null).ToString("yyyyMM"));

			// id_publicityタグ置換
			strEntry = strEntry.Replace("@@id_publicity@@", xeDetailElement.Element("id_publicity").Value);

			// titleタグ置換
			strEntry = strEntry.Replace("@@title@@", xeDetailElement.Element("title").Value);

			return strEntry;
		}

		/// <summary>
		/// 新着情報エントリー作成（一覧ページ）
		/// </summary>
		/// <param name="strEntry">新着情報エントリー</param>
		/// <param name="xeDetailElement">新着情報XML要素</param>
		private string CreatePublicityEntryForView(string strEntry, XElement xeDetailElement)
		{
			// id_publicityタグ置換
			strEntry = strEntry.Replace("@@id_publicity@@", xeDetailElement.Element("id_publicity").Value);
			// pub_dateタグ置換
			strEntry = strEntry.Replace("@@pub_date@@", DateTime.ParseExact(xeDetailElement.Element("pub_date").Value, "yyyy-MM-dd", null).ToString("yy.MM.dd"));
			// titleタグ置換
			strEntry = strEntry.Replace("@@title@@", xeDetailElement.Element("title").Value);
			// divisionタグ置換
			string strIconName = "";
			switch (xeDetailElement.Element("division").Value)
			{
				case Constants.KBN_PUBLICITY_DIVISION_NEW_ITEM:
					strIconName = "ico_new.gif";		// 新商品
					break;
				case Constants.KBN_PUBLICITY_DIVISION_MEDIA:
					strIconName = "ico_media.gif";		// メディア
					break;
				case Constants.KBN_PUBLICITY_DIVISION_CAMPAIGN:
					strIconName = "ico_campaign.gif";	// キャンペーン
					break;
				case Constants.KBN_PUBLICITY_DIVISION_ANNOUNCEMENT:
					strIconName = "ico_info.gif";		// お知らせ
					break;
				case Constants.KBN_PUBLICITY_DIVISION_OTHER:
					strIconName = "ico_other.gif";		// その他
					break;
				default:
					strIconName = "ico_other.gif";		// その他
					break;
			}
			strEntry = strEntry.Replace("@@division@@", strIconName);

			// Imageタグ置換
			if (Regex.Match(strEntry, TAG_KEY_IMAGES_BEGIN + "(.|\n)*?" + TAG_KEY_IMAGES_END).Value.Length > 0)
			{
				StringBuilder sbImages = new StringBuilder();

				string strImageTag = GetTagInnerContents(strEntry, TAG_KEY_IMAGES_BEGIN, TAG_KEY_IMAGES_END);

				if ((string.IsNullOrEmpty(xeDetailElement.Element("file_name1").Value) == false)
					&& string.IsNullOrEmpty(xeDetailElement.Element("file_name_thum1").Value) == false)
				{
					sbImages.Append(strImageTag);
					sbImages = sbImages.Replace("@@attachment@@", xeDetailElement.Element("file_name1").Value);
					sbImages = sbImages.Replace("@@thumb@@", xeDetailElement.Element("file_name_thum1").Value);
				}
				if ((string.IsNullOrEmpty(xeDetailElement.Element("file_name2").Value) == false)
					&& string.IsNullOrEmpty(xeDetailElement.Element("file_name_thum2").Value) == false)
				{
					sbImages.Append(strImageTag);
					sbImages = sbImages.Replace("@@attachment@@", xeDetailElement.Element("file_name2").Value);
					sbImages = sbImages.Replace("@@thumb@@", xeDetailElement.Element("file_name_thum2").Value);
				}

				strEntry = strEntry.Replace(Regex.Match(strEntry, TAG_KEY_IMAGES_BEGIN + "(.|\n)*?" + TAG_KEY_IMAGES_END).Value, sbImages.ToString());
			}

			// bodyタグ置換
			strEntry = strEntry.Replace("@@body@@", xeDetailElement.Element("body").Value.ReplaceCrLf("<br />"));

			// linkタグ置換
			int iLinkCount = 0;

			if (Regex.Match(strEntry, TAG_KEY_LINKS_BEGIN + "(.|\n)*?" + TAG_KEY_LINKS_END).Value.Length > 0)
			{
				StringBuilder sbLinks = new StringBuilder();
				
				string strLinkTag = GetTagInnerContents(strEntry, TAG_KEY_LINKS_BEGIN, TAG_KEY_LINKS_END);

				if ((string.IsNullOrEmpty(xeDetailElement.Element("anchor_title1").Value) == false)
					&& string.IsNullOrEmpty(xeDetailElement.Element("anchor_link1").Value) == false)
				{
					sbLinks.Append(strLinkTag);
					sbLinks = sbLinks.Replace("@@link_url@@", xeDetailElement.Element("anchor_link1").Value);
					sbLinks = sbLinks.Replace("@@link_title@@", xeDetailElement.Element("anchor_title1").Value);
					iLinkCount++;
				}
				if ((string.IsNullOrEmpty(xeDetailElement.Element("anchor_title2").Value) == false)
					&& string.IsNullOrEmpty(xeDetailElement.Element("anchor_link2").Value) == false)
				{
					sbLinks.Append(strLinkTag);
					sbLinks = sbLinks.Replace("@@link_url@@", xeDetailElement.Element("anchor_link2").Value);
					sbLinks = sbLinks.Replace("@@link_title@@", xeDetailElement.Element("anchor_title2").Value);
					iLinkCount++;
				}
				if ((string.IsNullOrEmpty(xeDetailElement.Element("anchor_title3").Value) == false)
					&& string.IsNullOrEmpty(xeDetailElement.Element("anchor_link3").Value) == false)
				{
					sbLinks.Append(strLinkTag);
					sbLinks = sbLinks.Replace("@@link_url@@", xeDetailElement.Element("anchor_link3").Value);
					sbLinks = sbLinks.Replace("@@link_title@@", xeDetailElement.Element("anchor_title3").Value);
					iLinkCount++;
				}
				if ((string.IsNullOrEmpty(xeDetailElement.Element("anchor_title4").Value) == false)
					&& string.IsNullOrEmpty(xeDetailElement.Element("anchor_link4").Value) == false)
				{
					sbLinks.Append(strLinkTag);
					sbLinks = sbLinks.Replace("@@link_url@@", xeDetailElement.Element("anchor_link4").Value);
					sbLinks = sbLinks.Replace("@@link_title@@", xeDetailElement.Element("anchor_title4").Value);
					iLinkCount++;
				}

				strEntry = strEntry.Replace(Regex.Match(strEntry, TAG_KEY_LINKS_BEGIN + "(.|\n)*?" + TAG_KEY_LINKS_END).Value, sbLinks.ToString());
			}

			// has_linkタグ置換
			if (iLinkCount == 0)
			{
				strEntry = strEntry.Replace(Regex.Match(strEntry, TAG_KEY_HAS_LINK_BEGIN + "(.|\n)*?" + TAG_KEY_HAS_LINK_END).Value, "");
			}
			else
			{
				strEntry = strEntry.Replace(TAG_KEY_HAS_LINK_BEGIN, "").Replace(TAG_KEY_HAS_LINK_END, "");
			}

			return strEntry;
		}

		/// <summary>
		/// 新着情報エントリー作成（アーカイブ）
		/// </summary>
		/// <param name="strEntry">新着情報エントリー</param>
		/// <param name="xeDetailElement">新着情報XML要素</param>
		private string CreatePublicityEntryForArchive(string strEntry, XElement xeDetailElement)
		{
			// id_publicityタグ置換
			strEntry = strEntry.Replace("@@id_publicity@@", xeDetailElement.Element("id_publicity").Value);
			// pub_dateタグ置換
			strEntry = strEntry.Replace("@@pub_date@@", DateTime.ParseExact(xeDetailElement.Element("pub_date").Value, "yyyy-MM-dd", null).ToString("yy.MM.dd"));
			// titleタグ置換
			strEntry = strEntry.Replace("@@title@@", xeDetailElement.Element("title").Value);
			// divisionタグ置換
			string strIconName = "";
			switch (xeDetailElement.Element("division").Value)
			{
				case Constants.KBN_PUBLICITY_DIVISION_NEW_ITEM:
					strIconName = "ico_new.gif";		// 新商品
					break;
				case Constants.KBN_PUBLICITY_DIVISION_MEDIA:
					strIconName = "ico_media.gif";		// メディア
					break;
				case Constants.KBN_PUBLICITY_DIVISION_CAMPAIGN:
					strIconName = "ico_campaign.gif";	// キャンペーン
					break;
				case Constants.KBN_PUBLICITY_DIVISION_ANNOUNCEMENT:
					strIconName = "ico_info.gif";		// お知らせ
					break;
				case Constants.KBN_PUBLICITY_DIVISION_OTHER:
					strIconName = "ico_other.gif";		// その他
					break;
				default:
					strIconName = "ico_other.gif";		// その他
					break;
			}
			strEntry = strEntry.Replace("@@division@@", strIconName);

			// Imageタグ置換
			if (Regex.Match(strEntry, TAG_KEY_IMAGES_BEGIN + "(.|\n)*?" + TAG_KEY_IMAGES_END).Value.Length > 0)
			{
				StringBuilder sbImages = new StringBuilder();

				string strImageTag = GetTagInnerContents(strEntry, TAG_KEY_IMAGES_BEGIN, TAG_KEY_IMAGES_END);

				if ((string.IsNullOrEmpty(xeDetailElement.Element("file_name1").Value) == false)
					&& string.IsNullOrEmpty(xeDetailElement.Element("file_name_thum1").Value) == false)
				{
					sbImages.Append(strImageTag);
					sbImages = sbImages.Replace("@@attachment@@", xeDetailElement.Element("file_name1").Value);
					sbImages = sbImages.Replace("@@thumb@@", xeDetailElement.Element("file_name_thum1").Value);
				}
				if ((string.IsNullOrEmpty(xeDetailElement.Element("file_name2").Value) == false)
					&& string.IsNullOrEmpty(xeDetailElement.Element("file_name_thum2").Value) == false)
				{
					sbImages.Append(strImageTag);
					sbImages = sbImages.Replace("@@attachment@@", xeDetailElement.Element("file_name2").Value);
					sbImages = sbImages.Replace("@@thumb@@", xeDetailElement.Element("file_name_thum2").Value);
				}

				strEntry = strEntry.Replace(Regex.Match(strEntry, TAG_KEY_IMAGES_BEGIN + "(.|\n)*?" + TAG_KEY_IMAGES_END).Value, sbImages.ToString());
			}

			// bodyタグ置換
			strEntry = strEntry.Replace("@@body@@", xeDetailElement.Element("body").Value.ReplaceCrLf("<br />"));

			// linkタグ置換
			int iLinkCount = 0;
			if (Regex.Match(strEntry, TAG_KEY_LINKS_BEGIN + "(.|\n)*?" + TAG_KEY_LINKS_END).Value.Length > 0)
			{
				StringBuilder sbLinks = new StringBuilder();

				string strLinkTag = GetTagInnerContents(strEntry, TAG_KEY_LINKS_BEGIN, TAG_KEY_LINKS_END);

				if ((string.IsNullOrEmpty(xeDetailElement.Element("anchor_title1").Value) == false)
					&& string.IsNullOrEmpty(xeDetailElement.Element("anchor_link1").Value) == false)
				{
					sbLinks.Append(strLinkTag);
					sbLinks = sbLinks.Replace("@@link_url@@", xeDetailElement.Element("anchor_link1").Value);
					sbLinks = sbLinks.Replace("@@link_title@@", xeDetailElement.Element("anchor_title1").Value);
					iLinkCount++;
				}
				if ((string.IsNullOrEmpty(xeDetailElement.Element("anchor_title2").Value) == false)
					&& string.IsNullOrEmpty(xeDetailElement.Element("anchor_link2").Value) == false)
				{
					sbLinks.Append(strLinkTag);
					sbLinks = sbLinks.Replace("@@link_url@@", xeDetailElement.Element("anchor_link2").Value);
					sbLinks = sbLinks.Replace("@@link_title@@", xeDetailElement.Element("anchor_title2").Value);
					iLinkCount++;
				}
				if ((string.IsNullOrEmpty(xeDetailElement.Element("anchor_title3").Value) == false)
					&& string.IsNullOrEmpty(xeDetailElement.Element("anchor_link3").Value) == false)
				{
					sbLinks.Append(strLinkTag);
					sbLinks = sbLinks.Replace("@@link_url@@", xeDetailElement.Element("anchor_link3").Value);
					sbLinks = sbLinks.Replace("@@link_title@@", xeDetailElement.Element("anchor_title3").Value);
					iLinkCount++;
				}
				if ((string.IsNullOrEmpty(xeDetailElement.Element("anchor_title4").Value) == false)
					&& string.IsNullOrEmpty(xeDetailElement.Element("anchor_link4").Value) == false)
				{
					sbLinks.Append(strLinkTag);
					sbLinks = sbLinks.Replace("@@link_url@@", xeDetailElement.Element("anchor_link4").Value);
					sbLinks = sbLinks.Replace("@@link_title@@", xeDetailElement.Element("anchor_title4").Value);
					iLinkCount++;
				}

				strEntry = strEntry.Replace(Regex.Match(strEntry, TAG_KEY_LINKS_BEGIN + "(.|\n)*?" + TAG_KEY_LINKS_END).Value, sbLinks.ToString());
			}

			// has_linkタグ置換
			if (iLinkCount == 0)
			{
				strEntry = strEntry.Replace(Regex.Match(strEntry, TAG_KEY_HAS_LINK_BEGIN + "(.|\n)*?" + TAG_KEY_HAS_LINK_END).Value, "");
			}
			else
			{
				strEntry = strEntry.Replace(TAG_KEY_HAS_LINK_BEGIN, "").Replace(TAG_KEY_HAS_LINK_END, "");
			}

			return strEntry;
		}

		/// <summary>
		/// 新着情報エントリー作成（バックナンバー）
		/// </summary>
		/// <param name="strEntry">新着情報エントリー</param>
		/// <param name="strYearMonth">年月</param>
		/// <param name="iEntryCount">該当月の新着情報数</param>
		private string CreatePublicityEntryForBackNumber(string strEntry, string strYearMonth, int iEntryCount)
		{
			// monthタグ置換
			strEntry = strEntry.Replace("@@month@@", strYearMonth);
			// month_jタグ置換
			strEntry = strEntry.Replace("@@month_j@@", strYearMonth.Substring(0, 4) + "年" + strYearMonth.Substring(4, 2) + "月");
			// cntタグ置換
			strEntry = strEntry.Replace("@@cnt@@", iEntryCount.ToString());

			return strEntry;
		}

		/// <summary>
		/// 特定タグ内コンテンツ取得
		/// </summary>
		/// <param name="strContents">検索コンテンツ</param>
		/// <param name="strTagBgn">開始タグ</param>
		/// <param name="strTagEnd">終了タグ</param>
		private string GetTagInnerContents(string strContents, string strTagBgn, string strTagEnd)
		{
			return Regex.Match(strContents, strTagBgn + "(.|\n)*?" + strTagEnd).Value.Replace(strTagBgn, "").Replace(strTagEnd, "");
		}

		/// <summary>
		/// モバイルページデータ取得
		/// </summary>
		/// <param name="strMobilePageID">モバイルページID</param>
		private DataView GetMobilePageData(string strMobilePageID)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MobilePage", "GetMobilePage"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MOBILEPAGE_DEPT_ID, Constants.CONST_DEFAULT_DEPT_ID);
				htInput.Add(Constants.FIELD_MOBILEPAGE_PAGE_ID, strMobilePageID);
				htInput.Add(Constants.FIELD_MOBILEPAGE_BRAND_ID, "");

				return sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// モバイルページデータ挿入/更新
		/// </summary>
		/// <param name="strMobilePageID">モバイルページID</param>
		/// <param name="strHTML">モバイルHTML</param>
		private int InsertUpdateMobilePageData(string strMobilePageID, string strHTML, string strMobilePageName)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MobilePage", "InsertUpdateMobilePage"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MOBILEPAGE_DEPT_ID, Constants.CONST_DEFAULT_DEPT_ID);
				htInput.Add(Constants.FIELD_MOBILEPAGE_PAGE_ID, strMobilePageID);
				htInput.Add(Constants.FIELD_MOBILEPAGE_HTML, strHTML);
				htInput.Add(Constants.FIELD_MOBILEPAGE_PAGE_NAME, strMobilePageName);
				htInput.Add(Constants.FIELD_MOBILEPAGE_LAST_CHANGED, "batch");
				htInput.Add(Constants.FIELD_MOBILEPAGE_BRAND_ID, "");

				return sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}
		}
	}
}
