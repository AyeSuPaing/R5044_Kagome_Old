/*
=========================================================================================================
  Module      : パーツ管理 設定ファイル管理(PartsDesignSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
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
	/// パーツ管理 設定ファイル管理
	/// </summary>
	public class PartsDesignSetting
	{
		/// <summary>設定ディレクトリ名</summary>
		private const string SETTING_FILE_DIR = @"Xml\Design\";
		/// <summary>設定ファイル名</summary>
		private const string SETTING_FILE_NAME = "PartsDesign.xml";
		/// <summary>インスタンス</summary>
		private static PartsDesignSetting m_singletonInstance;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PartsDesignSetting()
		{
			Update();
			FileUpdateObserver.GetInstance().AddObservation(
				Path.Combine(Constants.PHYSICALDIRPATH_CMS_MANAGER, SETTING_FILE_DIR),
				SETTING_FILE_NAME,
				Update);
		}

		/// <summary>
		/// インスタンス返却
		/// </summary>
		/// <returns>インスタンス</returns>
		public static PartsDesignSetting GetInstance()
		{
			if (m_singletonInstance == null) m_singletonInstance = new PartsDesignSetting();
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
					this.DesignParts = (DesignParts)new XmlSerializer(typeof(DesignParts)).Deserialize(fs);
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError("設定「" + this.SettingFileFullPath + "」の読み込みに失敗しました。", ex);
			}
		}

		/// <summary>各パーツ設定</summary>
		public DesignParts DesignParts { get; private set; }
		/// <summary>設定ファイル フルパス</summary>
		private string SettingFileFullPath
		{
			get { return Path.Combine(Constants.PHYSICALDIRPATH_CMS_MANAGER, SETTING_FILE_DIR, SETTING_FILE_NAME); }
		}
	}

	/// <summary>
	/// 各パーツ設定
	/// </summary>
	[XmlRoot("PartsDesign")]
	public class DesignParts
	{
		/// <summary>各パーツ設定</summary>
		[XmlArray("PartsSettings")]
		[XmlArrayItem("PartsSetting")]
		public List<PartsSetting> PartsSetting { get; set; }

		/// <summary>利用可能な標準パーツ一覧</summary>
		[XmlArray("UseTemplateStandardParts")]
		[XmlArrayItem("Value")]
		public List<StandardPars> UseTemplateStandardParts { get; set; }
	}

	/// <summary>
	/// 標準パーツ設定
	/// </summary>
	[Serializable]
	public class StandardPars
	{
		/// <summary>パーツ名</summary>
		[XmlAttribute("text")]
		public string Text { get; set; }
		/// <summary>パーツパス</summary>
		[XmlAttribute("value")]
		public string Value { get; set; }
	}

	/// <summary>
	/// パーツ設定
	/// </summary>
	[Serializable]
	public class PartsSetting
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PartsSetting()
		{
			this.LayoutAble = true;
		}

		/// <summary>パーツ名</summary>
		[XmlAttribute("name")]
		public string Name { get; set; }
		/// <summary>パーツパス</summary>
		[XmlAttribute("path")]
		public string Path { get; set; }
		/// <summary>配置レイアウトへの表示フラグ</summary>
		[XmlAttribute("layoutable")]
		public bool LayoutAble { get; set; }
		/// <summary>説明</summary>
		[XmlElement("Declaration")]
		public string Declaration { get; set; }
		/// <summary>レイアウトパーツ</summary>
		[XmlElement("LayoutParts")]
		public List<LayoutParts> LayoutParts { get; set; }
		/// <summary>パーツ内で利用できるタグ</summary>
		[XmlElement("TagSetting")]
		public List<DesignTagSetting> TagSetting { get; set; }
	}

	/// <summary>
	/// レイアウトパーツ
	/// </summary>
	[Serializable]
	public class LayoutParts
	{
		/// <summary>タグ名</summary>
		[XmlElement("Name")]
		public string Name { get; set; }
		/// <summary>タグ内容</summary>
		[XmlElement("Value")]
		public string Value { get; set; }
	}
}