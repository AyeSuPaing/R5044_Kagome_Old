/*
=========================================================================================================
  Module      : サイト情報ユーティリティクラス(SiteInformationUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using w2.Common.Util;

public static class SiteInformationUtility
{
	/// <summary>ShopMessage.xml の XDocument</summary>
	private static XDocument m_shopMessageXml;

	/// <summary>
	/// サイト情報種別
	/// </summary>
	public enum SiteInformationType
	{
		/// <summary>サイト名</summary>
		ShopName,
		/// <summary>会社名</summary>
		CompanyName,
		/// <summary>問い合わせメールアドレス</summary>
		ContactMail,
		/// <summary>問い合わせ窓口情報</summary>
		ContactCenterInfo,
		/// <summary>メールアドレスドメイン</summary>
		ShopMailDomain,
		/// <summary>返品特約ページパス</summary>
		ReturnSpecialContractPage,
	}

	/// <summary>
	/// サイト設定情報モデル
	/// </summary>
	public class SiteInformationModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="element">要素</param>
		public SiteInformationModel(XElement element)
		{
			this.NodeName = (SiteInformationType)Enum.Parse(typeof(SiteInformationType), element.Name.LocalName);
			this.TagName = element.XPathSelectElement("Name").Value;
			this.TextBoxRows = int.Parse(element.XPathSelectElement("TextBoxRows").Value);
			this.Text = element.XPathSelectElement("Text").Value;
			this.Description = element.XPathSelectElement("Description").Value;
		}

		/// <summary>ノード名</summary>
		public SiteInformationType NodeName { get; private set; }
		/// <summary>タグ名</summary>
		public string TagName { get; private set; }
		/// <summary>行数</summary>
		public int TextBoxRows { get; private set; }
		/// <summary>設定値</summary>
		public string Text { get; private set; }
		/// <summary>説明文</summary>
		public string Description { get; private set; }

	}

	/// <summary>定数: 設定ディレクトリ名</summary>
	private const string SETTING_FILE_DIR = "Contents";
	/// <summary>定数: 設定ファイル名</summary>
	private const string SETTING_FILE_NAME = "ShopMessage.xml";
	/// <summary>定数: 設定フルパス</summary>
	private static readonly string SETTING_FULL_PATH = Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, SETTING_FILE_DIR, SETTING_FILE_NAME);

	/// <summary>
	/// 静的コンストラクタ
	/// </summary>
	static SiteInformationUtility()
	{
		FileUpdateObserver.GetInstance().AddObservation(Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, SETTING_FILE_DIR), SETTING_FILE_NAME, Update);
		Update();
	}

	/// <summary>
	/// 最新の設定データに更新
	/// </summary>
	private static void Update()
	{
		if (File.Exists(SETTING_FULL_PATH) == false) return;
		m_shopMessageXml = XDocument.Load(SETTING_FULL_PATH);
	}

	/// <summary>
	/// サイト情報取得
	/// </summary>
	public static IEnumerable<SiteInformationModel> SiteInformation
	{
		get
		{
			return m_shopMessageXml.XPathSelectElements("ShopMessage/*").Select(x => new SiteInformationModel(x));
		}
	}
}
