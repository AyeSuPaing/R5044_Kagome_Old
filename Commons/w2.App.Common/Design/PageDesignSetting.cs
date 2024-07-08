/*
=========================================================================================================
  Module      : ページ管理 設定ファイル管理(PageDesignSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.App.Common.Design
{
	/// <summary>
	/// ページ管理 設定ファイル管理
	/// </summary>
	public class PageDesignSetting
	{
		/// <summary>設定ディレクトリ名</summary>
		private const string SETTING_FILE_DIR = @"Xml\Design\";
		/// <summary>設定ファイル名</summary>
		private const string SETTING_FILE_NAME = "PageDesign.xml";
		/// <summary>インスタンス</summary>
		private static PageDesignSetting m_singletonInstance;

		/// <summary>ページオプション</summary>
		public enum Option
		{
			/// <summary>特集ページオプション</summary>
			FeaturePage,
			/// <summary>Amazonオプション</summary>
			Amazon,
			/// <summary>同梱オプション</summary>
			Combine,
			/// <summary>ギフトオプション</summary>
			Gift,
			/// <summary>Awoo連携オプション</summary>
			Awoo,
			/// <summary>共通</summary>
			Common
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PageDesignSetting()
		{
			Update();
			FileUpdateObserver.GetInstance().AddObservation(Path.Combine(Constants.PHYSICALDIRPATH_CMS_MANAGER, SETTING_FILE_DIR), SETTING_FILE_NAME, Update);
		}

		/// <summary>
		/// インスタンス返却
		/// </summary>
		/// <returns>インスタンス</returns>
		public static PageDesignSetting GetInstance()
		{
			if (m_singletonInstance == null) m_singletonInstance = new PageDesignSetting();
			return m_singletonInstance;
		}

		/// <summary>
		/// 最新の設定データに更新
		/// </summary>
		private void Update()
		{
			if (File.Exists(this.SettingFileFullPath) == false) return;

			try
			{
				using (var fs = File.OpenRead(this.SettingFileFullPath))
				{
					this.DesignPage = (DesignPage)new XmlSerializer(typeof(DesignPage)).Deserialize(fs);
					if (Constants.REPEATPLUSONE_OPTION_ENABLED)
					{
						this.DesignPage.PageSetting = this.DesignPage.PageSetting
							.Where(
								p => (Constants.REPEATPLUSONE_CONFIGS
									.RepeatPlusOneSettings
									.PageManager.Any(rc => p.Path.Contains(rc.FileName)) == false))
							.ToList();
					}
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError("設定「" + this.SettingFileFullPath + "」の読み込みに失敗しました。", ex);
			}
		}

		/// <summary>各ページ設定</summary>
		public List<PageSetting> PageSettingList
		{
			get { return this.DesignPage.PageSetting.Where(p => p.Option != Option.Common.ToString()).ToList(); }
		}
		/// <summary>設定</summary>
		public DesignPage DesignPage { get; set; }
		/// <summary>設定ファイル フルパス</summary>
		private string SettingFileFullPath
		{
			get { return Path.Combine(Constants.PHYSICALDIRPATH_CMS_MANAGER, SETTING_FILE_DIR, SETTING_FILE_NAME); }
		}
	}

	/// <summary>
	/// 各ページ設定
	/// </summary>
	[XmlRoot("PageDesign")]
	public class DesignPage
	{
		/// <summary>各ページ設定</summary>
		[XmlArray("PageSettings")]
		[XmlArrayItem("PageSetting")]
		public List<PageSetting> PageSetting { get; set; }
	}

	/// <summary>
	/// ページ設定
	/// </summary>
	[Serializable]
	public class PageSetting
	{
		/// <summary>ページ名</summary>
		[XmlAttribute("name")]
		public string Name { get; set; }
		/// <summary>ページパス</summary>
		[XmlAttribute("path")]
		public string Path { get; set; }
		/// <summary>ページオプション</summary>
		[XmlAttribute("option")]
		public string Option { get; set; }
		/// <summary>ページ内で利用できるタグ一覧</summary>
		[XmlElement("TagSetting")]
		public List<DesignTagSetting> TagSetting { get; set; }
	}
}
