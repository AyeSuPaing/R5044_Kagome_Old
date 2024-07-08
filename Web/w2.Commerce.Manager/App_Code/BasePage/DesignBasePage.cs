/*
=========================================================================================================
  Module      : デザイン基底ページ(DesignBasePage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using w2.Common.Util.Archiver;

/// <summary>
/// DesignManagerBasePage の概要の説明です
/// </summary>
public abstract class DesignBasePage : BasePage
{
	// オプション属性
	private const string OPTION_MENU_ATTRIBUTE = "option";

	//=============================================================================================
	// タグ定義
	//=============================================================================================
	/// <summary>編集可能領域定義（開始タグ）</summary>
	protected string[] m_strEditableTagBgns = { "<%-- ▽編集可能領域：", "▽ --%>" };
	/// <summary>編集可能領域定義（終了タグ）</summary>
	protected string m_strEditableTagEnd = "<%-- △編集可能領域△ --%>";

	/// <summary>レイアウト領域定義（開始タグ）</summary>
	protected string[] m_strLayoutTagBgns = { "<%-- ▽レイアウト領域：", "▽ --%>" };
	/// <summary>レイアウト領域定義（終了タグ）</summary>
	protected string m_strLayoutTagEnd = "<%-- △レイアウト領域△ --%>";

	/// <summary>ユーザーコントロール宣言領域（開始タグ）</summary>
	protected string m_strUserControlDeclarationBgn = "<%-- ▽ユーザーコントロール宣言領域▽ --%>";
	/// <summary>ユーザーコントロール宣言領域（終了タグ）</summary>
	protected string m_strUserControlDeclarationEnd = "<%-- △ユーザーコントロール宣言領域△ --%>";

	/// <summary>パーツタグ</summary>
	public const string TAG_PARTS_BGN = "<!--{$PARTS:";
	public const string TAG_PARTS_END = "}-->";

	/// <summary>コンテンツNGチェックパターン</summary>
	protected const string CONTENTS_NGCHECK_PATTERN = "<%=|<%#|<%|%>|runat=";

	/// <summary>最終更新者タグ</summary>
	protected const string TAG_FILEINFO_LASTCHANGED_BGN = "<%@ FileInfo LastChanged=\"";
	protected const string TAG_FILEINFO_LASTCHANGED_END = "\" %>";

	/// <summary>編集サイト選択済みラジオボタンのクラス名</summary>
	protected const string SELECTED_TARGET_SITE_CSS_CLASS = "selected_target_site";

	/// <summary>編集モード</summary>
	protected enum EditMode
	{
		/// <summary>レイアウト編集</summary>
		Layout,
		/// <summary>コンテンツ編集</summary>
		Contents
	}

	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender">パラメータの説明を記述</param>
	/// <param name="e">パラメータの説明を記述</param>
	protected virtual void Page_Init(object sender, EventArgs e)
	{
		// Chromeの「ERR_BLOCKED_BY_XSS_AUDITOR」回避（HTMLをPOSTする箇所でエラーになる）
		Response.Headers.Set("X-XSS-Protection", "0");

		if ((!IsPostBack)
			|| (m_licStandardPageItemCollection.Count == 0)
			|| (m_licStandardPartsItemCollection.Count == 0)
			|| (m_dicUserControlTags.Count == 0)
			|| (m_dicUserControlDeclarations.Count == 0))
		{
			m_dicUserControlTags.Clear();
			m_dicUserControlDeclarations.Clear();

			//------------------------------------------------------
			// ページ設定読み込み
			//------------------------------------------------------
			{
				m_licStandardPageItemCollection = new ListItemCollection();

				XmlDocument xdDesignPages = new XmlDocument();

				foreach (string strTargetFilePath in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Xml/Design/", "Design_Page*.xml", SearchOption.AllDirectories))
				{
					xdDesignPages.Load(strTargetFilePath);

					foreach (XmlNode xnPageSetting in xdDesignPages.SelectNodes("/Design_Page/PageSetting"))
					{
						if (xnPageSetting.NodeType != XmlNodeType.Comment)
						{
							try
							{
								// 標準ページ設定追加
								m_licStandardPageItemCollection.Add(new ListItem(xnPageSetting.Attributes["name"].Value, xnPageSetting.Attributes["path"].Value));

							}
							catch (Exception ex)
							{
								throw new Exception("ノード「" + xnPageSetting.InnerText + "」の解析に失敗しました。", ex);
							}
						}
					}
				}
			}

			//------------------------------------------------------
			// パーツ設定読み込み
			//------------------------------------------------------
			{
				m_licStandardPartsItemCollection = new ListItemCollection();
				m_licLayoutablePartsItemCollection = new ListItemCollection();

				// 先ずは標準パーツの情報をセット
				XmlDocument xdDesignParts = new XmlDocument();
				foreach (string strTargetFilePath in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Xml/Design/", "Design_Parts*.xml", SearchOption.AllDirectories))
				{
					xdDesignParts.Load(strTargetFilePath);
					foreach (XmlNode xnPartsSetting in xdDesignParts.SelectNodes("/Design_Parts/PartsSetting"))
					{
						if (xnPartsSetting.NodeType != XmlNodeType.Comment)
						{
							try
							{
								// パーツ一覧用設定追加
								m_licStandardPartsItemCollection.Add(
									new ListItem(
										xnPartsSetting.Attributes["name"].Value,
										xnPartsSetting.Attributes["path"].Value));

								// パーツのタグ追加
								foreach (XmlNode xnTag in xnPartsSetting.SelectNodes("LayoutParts"))
								{
									// パーツの宣言追加
									if (xnPartsSetting.SelectSingleNode("Declaration") != null)
									{
										m_dicUserControlDeclarations.Add(
											xnTag.SelectSingleNode("Name").InnerText,
											xnPartsSetting.SelectSingleNode("Declaration").InnerText);
									}

									// パーツの呼び出しタグ登録
									m_dicUserControlTags.Add(
										xnTag.SelectSingleNode("Name").InnerText,
										xnTag.SelectSingleNode("Value").InnerText);

									// バーツボックスへ登録
									m_licLayoutablePartsItemCollection.Add(
										new ListItem(
											xnTag.SelectSingleNode("Name").InnerText,
											xnTag.SelectSingleNode("Value").InnerText));
								}
							}
							catch (Exception ex)
							{
								throw new Exception("ノード「" + xnPartsSetting.InnerText + "」の解析に失敗しました。", ex);
							}
						}
					}
				}

				// カスタムパーツ読み込み
				foreach (string strFilePath in Directory.GetFiles(this.PhysicaldirpathTargetSite + @"Page\Parts\"))
				{
					string strTitle = StringUtility.ToEmpty(GetAspxTitle(strFilePath));

					// 既にキーが存在しているなら数字を付加して識別用のタイトル作成
					if (m_dicUserControlTags.ContainsKey(strTitle))
					{
						int iCount = 1;
						while (true)
						{
							if ((m_dicUserControlTags.ContainsKey(strTitle + "(" + iCount.ToString() + ")")) == false)
							{
								strTitle = strTitle + "(" + iCount.ToString() + ")";
								break;
							}
							iCount++;
						}
					}

					m_dicUserControlTags.Add(strTitle, "<uc:" + Path.GetFileNameWithoutExtension(strFilePath) + " runat=\"server\" />");

					string strFileNameWithoutExtension = Path.GetFileNameWithoutExtension(strFilePath);
					m_dicUserControlDeclarations.Add(strTitle, "<%@ Register TagPrefix=\"uc\" TagName=\"" + WebSanitizer.HtmlEncode(strFileNameWithoutExtension) + "\" Src=\"~/Page/Parts/" + WebSanitizer.HtmlEncode(strFileNameWithoutExtension) + ".ascx\" %>");

					m_licLayoutablePartsItemCollection.Add(new ListItem(
						strTitle,
						strFilePath.Replace(this.PhysicaldirpathTargetSite, "")));
				}
			}
		}
	}

	/// <summary>
	/// ASPXタイトル取得
	/// </summary>
	/// <param name="strFilePath"></param>
	/// <returns></returns>
	public static string GetAspxTitle(string strFilePath)
	{
		string strSrc = GetFileTextAll(strFilePath);
		foreach (Match match in Regex.Matches(strSrc, "<%@ Page " + ".*?" + "%>"))
		{
			if (match.Value.Contains(" Title="))
			{
				int iBgn = match.Value.IndexOf(" Title=\"") + " Title=\"".Length;

				return HttpUtility.HtmlDecode(match.Value.Substring(
					iBgn,
					match.Value.IndexOf("\"", iBgn) - iBgn));
			}
		}

		return "";
	}

	/// <summary>
	/// 編集サイトの選択されているラジオボタンにCSS適用
	/// </summary>
	/// <param name="rbl">編集サイトのラジオボタンリスト</param>
	protected void SetCssTargetSite(RadioButtonList rbl)
	{
		SetClassSelectedRadioButton(rbl, SELECTED_TARGET_SITE_CSS_CLASS);
	}

	/// <summary>
	/// RadioButtonListの選択されているラジオボタンにclass属性を指定
	/// </summary>
	/// <param name="rbl">RadioButtonList</param>
	/// <param name="className">クラス名</param>
	protected void SetClassSelectedRadioButton(RadioButtonList rbl, string className)
	{
		rbl.SelectedItem.Attributes["class"] = className;
	}

	/// <summary>
	/// ファイルテキスト取得
	/// </summary>
	/// <param name="strTargetFilePath"></param>
	/// <returns></returns>
	public static string GetFileTextAll(string strTargetFilePath)
	{
		using (StreamReader srReader = new StreamReader(strTargetFilePath, Encoding.UTF8))
		{
			string fileTextAll = srReader.ReadToEnd();

			// 改行コードはWindown環境(CRLF)ではない場合、CRLFに変換する
			fileTextAll = fileTextAll.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");

			return fileTextAll;
		}
	}

	/// <summary>
	/// カスタムページリスト取得
	/// </summary>
	protected ListItemCollection GetCustomPageList()
	{
		// カスタムページ読み取り
		ListItemCollection licResult = new ListItemCollection();
		foreach (string strFilePath in Directory.GetFiles(this.PhysicaldirpathTargetSite + @"Page\", "*.aspx"))
		{
			licResult.Add(new ListItem(
				StringUtility.ToEmpty(GetAspxTitle(strFilePath)) + " (" + Path.GetFileName(strFilePath) + ")",
				strFilePath.Replace(this.PhysicaldirpathTargetSite, "")));
		}

		return licResult;
	}

	/// <summary>
	/// カスタムパーツリスト取得
	/// </summary>
	protected ListItemCollection GetCustomPartsList()
	{
		// カスタムパーツ読み取り
		ListItemCollection licResult = new ListItemCollection();
		foreach (string strFilePath in Directory.GetFiles(this.PhysicaldirpathTargetSite + @"Page\Parts\", "*.ascx"))
		{
			string strPartsName = "";
			string strPartsInitial = strFilePath.Replace(this.PhysicaldirpathTargetSite + @"Page\Parts\Parts", "").Replace(".ascx", "");
			strPartsInitial = (strPartsInitial.Length >= 3) ? strPartsInitial.Substring(0, strPartsInitial.Length - 3) : "";

			foreach (ListItem li in ValueText.GetValueItemList("CMS", "StandardParts"))
			{
				if (li.Value.ToLower().Contains(strPartsInitial.ToLower()))
				{
					strPartsName = li.Text;
					break;
				}
			}

			if (strPartsName.Length != 0)
			{
				licResult.Add(new ListItem(
					StringUtility.ToEmpty(GetAspxTitle(strFilePath)) + "｜" + strPartsName, strFilePath.Replace(this.PhysicaldirpathTargetSite, "")));
			}
		}

		return licResult;
	}

	/// <summary>
	/// 全ページ・パーツファイルパス取得（標準・カスタム）
	/// </summary>
	protected List<string> GetAllPagePartsList()
	{
		List<string> lResult = new List<string>();

		// ページ・パーツリスト取得（標準・カスタム）
		List<ListItemCollection> lAllPagePartsList = new List<ListItemCollection>();
		lAllPagePartsList.Add(m_licStandardPageItemCollection);
		lAllPagePartsList.Add(GetCustomPageList());
		lAllPagePartsList.Add(m_licStandardPartsItemCollection);
		lAllPagePartsList.Add(GetCustomPartsList());

		// 全ページ・パーツファイルパス取得
		foreach (ListItemCollection lic in lAllPagePartsList)
		{
			foreach (ListItem li in lic)
			{
				// 有効なパーツ？
				if (ValidParts(li.Value) == false) continue;

				string strFilePath = this.PhysicaldirpathTargetSite + li.Value;
				if (File.Exists(strFilePath))
				{
					lResult.Add(strFilePath);
				}
			}
		}

		// Cssファイルパス取得(編集サイト毎のCssPath)
		foreach (string strFilePath in GetFilePathList(this.PhysicaldirpathTargetSite + @"Css\"))
		{
			// .sccファイルとthumbs.dbファイルは追加しない
			if ((Path.GetExtension(strFilePath) == ".scc")
				|| (Path.GetFileName(strFilePath) == "thumbs.db"))
			{
				continue;
			}

			lResult.Add(strFilePath);
		}

		return lResult;
	}

	/// <summary>
	/// ファイルパス一覧取得（再帰）
	/// </summary>
	/// <param name="strDirectoryPath">ディレクトリパス</param>
	/// <returns>ディレクトリ配下のファイルパス一覧</returns>
	protected List<string> GetFilePathList(string strDirectoryPath)
	{
		List<string> lResult = new List<string>();

		// ファイルパス取得
		foreach (string strFilePath in Directory.GetFiles(strDirectoryPath))
		{
			lResult.Add(strFilePath);
		}

		// サブフォルダ内のファイルパス取得
		foreach (string strDirPath in Directory.GetDirectories(strDirectoryPath))
		{
			foreach (string strFilePath in GetFilePathList(strDirPath))
			{
				lResult.Add(strFilePath);
			}
		}

		return lResult;
	}

	/// <summary>
	/// 有効なパーツかどうか？
	/// </summary>
	/// <param name="filePath">ファイルパス</param>
	/// <returns></returns>
	protected bool ValidParts(string filePath)
	{
		// リアル店舗商品在庫一覧パーツ AND リアル店舗OPがOFFの場合
		if ((filePath.ToLower().Contains("BodyRealShopProductStockList.ascx".ToLower()) || (filePath.ToLower().Contains("100RSPSL_".ToLower())))
			&& (Constants.REALSHOP_OPTION_ENABLED == false))
		{
			return false;
		}

		// レコメンド表示パーツ AND レコメンド設定OPがOFFの場合
		if ((filePath.ToLower().Contains("BodyRecommend.ascx".ToLower()) || (filePath.ToLower().Contains("110RCD_".ToLower())))
			&& (Constants.RECOMMEND_OPTION_ENABLED == false))
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// 全ページ・パーツダウロードリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbAllDownload_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 全ページ・パーツファイルパス取得（標準・カスタム）
		//------------------------------------------------------
		List<string> lFilePaths = GetAllPagePartsList();

		//------------------------------------------------------
		// ZIP圧縮＆ファイルダウロード
		//------------------------------------------------------
		Response.ContentType = "application/zip";
		Response.AppendHeader("Content-Disposition", "attachment; filename=Design_" + DateTime.Now.ToString("yyyyMMdd") + ".zip");

		ZipArchiver zaZipArchiver = new ZipArchiver();
		zaZipArchiver.CompressToStream(lFilePaths.ToArray(), this.PhysicaldirpathTargetSite, Response.OutputStream);
	
		Response.End();
	}

	/// <summary>タグ設定</summary>
	protected static Dictionary<string, string> m_dicUserControlTags = new Dictionary<string, string>();
	protected static Dictionary<string, string> m_dicUserControlDeclarations = new Dictionary<string, string>();
	
	/// <summary>標準ページ画面設定用ListItemCollecttion</summary>
	protected static ListItemCollection m_licStandardPageItemCollection = new ListItemCollection();
	/// <summary>標準パーツ画面設定用ListItemCollecttion</summary>
	protected static ListItemCollection m_licStandardPartsItemCollection = new ListItemCollection();

	/// <summary>レイアウトパーツボックス画面設定用ListItemCollecttion</summary>
	protected static ListItemCollection m_licLayoutablePartsItemCollection = new ListItemCollection();

	/// <summary>対象サイトルート</summary>
	protected string TargetSitePathRoot
	{
		get
		{
			if (Request[Constants.REQUEST_KEY_DESIGN_TARGET_SITE] == null)
			{
				return Constants.PATH_ROOT_FRONT_PC;
			}
			return Request[Constants.REQUEST_KEY_DESIGN_TARGET_SITE];
		}
	}
	/// <summary>対象サイト物理パス</summary>
	protected string PhysicaldirpathTargetSite
	{
		get
		{
			return Constants.PHYSICALDIRPATH_FRONT_PC.Substring(0, Constants.PHYSICALDIRPATH_FRONT_PC.Length - Constants.PATH_ROOT_FRONT_PC.Length) + this.TargetSitePathRoot.Replace("/", @"\");
		}
	}
	/// <summary>対象PCサイト物理パス</summary>
	protected string PhysicaldirpathTargetSitePC
	{
		get
		{
			return Constants.PHYSICALDIRPATH_FRONT_PC.Substring(0, Constants.PHYSICALDIRPATH_FRONT_PC.Length - Constants.PATH_ROOT_FRONT_PC.Length) + Constants.PATH_ROOT_FRONT_PC.Replace("/", @"\");
		}
	}

}
