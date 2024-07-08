/*
=========================================================================================================
  Module      : 更新履歴表示フォーマット設定クラス(UpdateHistoryDisplayFormatSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using w2.Common.Util;

/// <summary>
/// 更新履歴表示フォーマット設定クラス
/// </summary>
public class UpdateHistoryDisplayFormatSetting
{
	/// <summary>設定ディレクトリ名</summary>
	private static string DIRNAME_SETTINGS = @"Settings";
	/// <summary>ベースディレクトリ名</summary>
	private static string DIRNAME_BASE = @"base";
	/// <summary>更新履歴表示フォーマット設定ファイル名</summary>
	private const string FILENAME_UPDATE_HISTORY_DISPLAY_FORMAT_SETTING = @"UpdateHistoryDisplayFormatSetting.xml";
	/// <summary>設定ディレクトリパス</summary>
	private static readonly string m_dirSettingDirPath;
	/// <summary>設定ベースディレクトリパス</summary>
	private static readonly string m_baseDirSettingDirPath;
	/// <summary>フォーマット設定格納ディクショナリ</summary>
	private static readonly Dictionary<string, string> m_displayFormat = new Dictionary<string, string>();
	/// <summary>ロックオブジェクト</summary>
	private static readonly object m_lockObject = new object();

	/// <summary>
	/// スタティックコンストラクタ
	/// </summary>
	static UpdateHistoryDisplayFormatSetting()
	{
		m_dirSettingDirPath = Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, DIRNAME_SETTINGS);
		m_baseDirSettingDirPath = Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, DIRNAME_SETTINGS, DIRNAME_BASE);

		// 設定更新
		UpdateSetting();

		// 監視セット
		FileUpdateObserver
			.GetInstance()
			.AddObservation(m_dirSettingDirPath, FILENAME_UPDATE_HISTORY_DISPLAY_FORMAT_SETTING, UpdateSetting);
		FileUpdateObserver
			.GetInstance()
			.AddObservation(m_baseDirSettingDirPath, FILENAME_UPDATE_HISTORY_DISPLAY_FORMAT_SETTING, UpdateSetting);
	}

	/// <summary>
	/// 設定更新
	/// </summary>
	public static void UpdateSetting()
	{
		lock (m_lockObject)
		{
			try
			{
				// 案件の設定が存在しない場合ベースの設定を読込
				var filePath = Path.Combine(m_dirSettingDirPath, FILENAME_UPDATE_HISTORY_DISPLAY_FORMAT_SETTING);
				if (File.Exists(filePath) == false)
				{
					filePath = Path.Combine(m_baseDirSettingDirPath, FILENAME_UPDATE_HISTORY_DISPLAY_FORMAT_SETTING);
				}

				var displayFormatXmlDocument = new XmlDocument();
				displayFormatXmlDocument.Load(filePath);
				foreach (var displayFormat in displayFormatXmlDocument.SelectSingleNode("DisplayFormat").ChildNodes.Cast<XmlNode>())
				{
					if (displayFormat.NodeType == XmlNodeType.Comment) continue;

					m_displayFormat[displayFormat.Name] = Regex.Unescape(displayFormat.InnerText);
				}
			}
			catch (Exception ex)
			{
				AppLogger.WriteError("更新履歴表示フォーマット設定XMLの読み込みに失敗しました。", ex);
			}
		}
	}

	/// <summary>
	/// 名称を指定してフォーマット文字列を取得
	/// </summary>
	/// <param name="formatName">フォーマット名</param>
	/// <returns>表示用フォーマット文字列</returns>
	public static string GetFormat(string formatName)
	{
		return m_displayFormat[formatName];
	}

	#region +プロパティ
	/// <summary>フォーマットリスト</summary>
	public static Dictionary<string, string> FormatList
	{
		get
		{
			lock (m_lockObject)
			{
				return m_displayFormat;
			}
		}
	}
	#endregion
}