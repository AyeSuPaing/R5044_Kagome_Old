/*
=========================================================================================================
  Module      : リピートプラスONE対応 設定クラス(RepeatPlusOneConfigs.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.App.Common.RepeatPlusOne.Config
{
	/// <summary>
	/// リピートプラスONE対応 設定クラス
	/// </summary>
	public class RepeatPlusOneConfigs
	{
		/// <summary>定数：リピートプラスONE設定ディレクトリ名</summary>
		private const string SETTING_FILE_DIR = "Settings";
		/// <summary>定数: プロジェクト用リピートプラスONE設定ファイル名</summary>
		private const string PROJECT_SETTING_FILE_NAME = "ProjectRepeatPlusOneSetting.xml";


		/// <summary>定数: リピートプラスONE設定Baseディレクトリ名</summary>
		private const string SETTING_FILE_BASE_DIR = @"Settings\base";
		/// <summary>定数：リピートプラスONE設定ファイル名</summary>
		private const string SETTING_FILE_NAME = "RepeatPlusOneSetting.xml";

		/// <summary>インスタンス</summary>
		private static RepeatPlusOneConfigs m_singletonInstance;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RepeatPlusOneConfigs()
		{
			Update();
			FileUpdateObserver.GetInstance().AddObservation(Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, SETTING_FILE_BASE_DIR), SETTING_FILE_NAME, Update);
			FileUpdateObserver.GetInstance().AddObservation(Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, SETTING_FILE_DIR), PROJECT_SETTING_FILE_NAME, Update);
		}

		/// <summary>
		/// インスタンス返却
		/// </summary>
		/// <returns>インスタンス</returns>
		public static RepeatPlusOneConfigs GetInstance()
		{
			return m_singletonInstance ?? (m_singletonInstance = new RepeatPlusOneConfigs());
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
					this.RepeatPlusOneSettings =
						(RepeatPlusOneSettings)new XmlSerializer(typeof(RepeatPlusOneSettings))
							.Deserialize(fs);
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError("リピートプラスONE設定「" + (this.SettingFileFullPath) + "」の読み込みに失敗しました。", ex);
			}

			UpdateProjectConf();
		}

		/// <summary>
		/// プロジェクト別の設定更新
		/// </summary>
		private void UpdateProjectConf()
		{
			if (File.Exists(this.SettingProjectFileFullPath) == false) return;

			// 上書きする元がなければ何もしない
			if (this.RepeatPlusOneSettings == null) return;

			try
			{
				using (var fs = File.OpenRead(this.SettingProjectFileFullPath))
				{
					var projectSetting = (RepeatPlusOneSettings)new XmlSerializer(typeof(RepeatPlusOneSettings)).Deserialize(fs);

					// 上書き
					foreach (var prop in projectSetting.GetType().GetProperties()
						.Where(p => Attribute.GetCustomAttribute(p, typeof(XmlArrayItemAttribute)) != null))
					{
						// コレクション要素で要素数が1つ以上あるもののみ、すなわち要素が存在する場合だけを上書き
						if ((prop.GetValue(projectSetting) is ICollection)
							&& (((ICollection)prop.GetValue(projectSetting)).Count > 0))
						{
							this.RepeatPlusOneSettings.GetType().GetProperty(prop.Name).SetValue(this.RepeatPlusOneSettings, prop.GetValue(projectSetting));
						}
					}

					foreach (var prop in projectSetting.GetType().GetProperties()
						.Where(p => Attribute.GetCustomAttribute(p, typeof(XmlElementAttribute)) != null))
					{
						// Null以外、すなわち要素が存在する場合だけ上書き
						if (prop.GetValue(projectSetting) != null)
						{
							this.RepeatPlusOneSettings.GetType().GetProperty(prop.Name).SetValue(this.RepeatPlusOneSettings, prop.GetValue(projectSetting));
						}
					}
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError("リピートプラスONE設定「" + (this.SettingProjectFileFullPath) + "」の読み込みに失敗しました。", ex);
			}
		}

		/// <summary>リピートプラスONE対応 設定</summary>
		public RepeatPlusOneSettings RepeatPlusOneSettings { get; set; }
		/// <summary>リピートプラスONE設定ファイル フルパス</summary>
		private string SettingFileFullPath
		{
			get { return Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, SETTING_FILE_BASE_DIR, SETTING_FILE_NAME); }
		}
		/// <summary>プロジェクト毎のリピートプラスONE設定ファイルのパス </summary>
		private string SettingProjectFileFullPath
		{
			get { return Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, SETTING_FILE_DIR, PROJECT_SETTING_FILE_NAME); }
		}
	}

	/// <summary>
	/// 各ページ設定
	/// </summary>
	[XmlRoot("RepeatPlusOneSettings")]
	public class RepeatPlusOneSettings
	{
		/// <summary>ページ管理設定</summary>
		[XmlArray("PageManager")]
		[XmlArrayItem("HideSetting")]
		public List<HideSetting> PageManager { get; set; }
		/// <summary>パーツ管理設定</summary>
		[XmlArray("PartsManager")]
		[XmlArrayItem("HideSetting")]
		public List<HideSetting> PartsManager { get; set; }
		/// <summary>コンテンツマネージャー設定</summary>
		[XmlArray("ContentsManager")]
		[XmlArrayItem("HideSetting")]
		public List<HideSetting> ContentsManager { get; set; }
	}

	/// <summary>
	/// 非表示設定
	/// </summary>
	[Serializable]
	public class HideSetting
	{
		/// <summary>ファイル名</summary>
		[XmlAttribute("fileName")]
		public string FileName { get; set; }
		/// <summary>ディレクトリ</summary>
		[XmlAttribute("dir")]
		public string Dir { get; set; }
	}
}