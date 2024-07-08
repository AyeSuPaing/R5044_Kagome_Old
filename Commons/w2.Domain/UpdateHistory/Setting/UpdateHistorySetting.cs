/*
=========================================================================================================
  Module      : 更新履歴設定クラス(UpdateHistorySetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.Domain.UpdateHistory.Setting
{
	/// <summary>
	/// 更新履歴設定クラス
	/// </summary>
	public class UpdateHistorySetting
	{
		/// <summary>設定ディレクトリ名</summary>
		private static string DIRNAME_SETTINGS = @"Settings";
		/// <summary>ベースディレクトリ名</summary>
		private static string DIRNAME_BASE = @"base";
		/// <summary>更新履歴設定ファイル名</summary>
		private const string FILENAME_UPDATEHISTORY_SETTING = @"UpdateHistorySetting.xml";
		/// <summary>設定ディレクトリパス</summary>
		private static readonly string m_dirSettingDirPath;
		/// <summary>設定ベースディレクトリパス</summary>
		private static readonly string m_baseDirSettingDirPath;
		/// <summary>ベースディレクトリ名</summary>
		private static readonly List<Fields> m_cachedFields = new List<Fields>();
		/// <summary>ロックオブジェクト</summary>
		private static readonly object m_lockObject = new object();

		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static UpdateHistorySetting()
		{
			m_dirSettingDirPath = Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, DIRNAME_SETTINGS);
			m_baseDirSettingDirPath = Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, DIRNAME_SETTINGS, DIRNAME_BASE);

			// 設定更新
			UpdateSetting();

			// 監視セット
			FileUpdateObserver.GetInstance().AddObservation(m_dirSettingDirPath, FILENAME_UPDATEHISTORY_SETTING, UpdateSetting);
			FileUpdateObserver.GetInstance().AddObservation(m_baseDirSettingDirPath, FILENAME_UPDATEHISTORY_SETTING, UpdateSetting);
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
					FieldsList.Clear();

					// 案件の設定が存在しない場合ベースの設定を読込
					var filePath = Path.Combine(m_dirSettingDirPath, FILENAME_UPDATEHISTORY_SETTING);
					if (File.Exists(filePath) == false)
					{
						filePath = Path.Combine(m_baseDirSettingDirPath, FILENAME_UPDATEHISTORY_SETTING);
					}
					var setting = XDocument.Load(filePath).Element("UpdateHistorySetting");
					foreach (var elem in setting.Elements("Fields"))
					{
						FieldsList.Add(new Fields(elem.Attribute("kbn").Value, elem.Elements("Field")));
					}
				}
				catch (Exception ex)
				{
					AppLogger.WriteError("更新履歴設定XMLの読み込みに失敗しました。", ex);
				}
			}
		}

		/// <summary>
		/// 各更新履歴区分に対応した更新履歴出力項目取得
		/// </summary>
		/// <param name="updateHistoryKbn">更新履歴区分</param>
		/// <returns>更新履歴出力項目</returns>
		public static Fields GetFields(string updateHistoryKbn)
		{
			return FieldsList.First(f => f.Kbn == updateHistoryKbn);
		}

		#region +プロパティ
		/// <summary>フィールドリスト</summary>
		public static List<Fields> FieldsList
		{
			get
			{
				lock (m_lockObject)
				{
					return m_cachedFields;
				}
			}
		}
		#endregion
	}

	/// <summary>
	/// 更新履歴出力項目設定群クラス
	/// </summary>
	public class Fields
	{
		#region +コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="kbn">区分</param>
		/// <param name="fields">項目</param>
		public Fields(string kbn, IEnumerable<XElement> fields)
		{
			this.Kbn = kbn;
			this.FieldList = new List<Field>();
			this.FieldList = fields.Select(f => new Field(f)).ToList();
		}
		#endregion

		#region +プロパティ
		/// <summary>区分</summary>
		public string Kbn { get; private set; }
		/// <summary>フィールドリスト</summary>
		public List<Field> FieldList { get; private set; }
		#endregion
	}

	/// <summary>
	/// 更新履歴出力項目設定クラス
	/// </summary>
	public class Field : ICloneable
	{
		#region +コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Field()
		{
			this.JName = "";
			this.Name = "";
			this.Type = "";
			this.Convert = "";
			this.Format = "";
			this.ValueText = new string[1];
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="field">フィールド</param>
		public Field(XElement field)
			: this()
		{
			this.JName = field.Attribute("jname").Value;
			this.Name = field.Attribute("name").Value;
			if (field.Attribute("type") != null) this.Type = field.Attribute("type").Value;
			if (field.Attribute("convert") != null) this.Convert = field.Attribute("convert").Value;
			if (this.Convert == "format")
			{
				if (field.Attribute("format") != null) this.Format = field.Attribute("format").Value;
			}
			if (this.Convert == "valuetext")
			{
				if (field.Attribute("valuetext") != null) this.ValueText = field.Attribute("valuetext").Value.Split(',');
			}
		}
		#endregion

		#region +プロパティ
		/// <summary>項目名（論理名）</summary>
		public string JName { get; set; }
		/// <summary>項目名（物理名）</summary>
		public string Name { get; set; }
		/// <summary>フィールド型</summary>
		public string Type { get; set; }
		/// <summary>変換方法</summary>
		public string Convert { get; set; }
		/// <summary>フォーマット</summary>
		public string Format { get; set; }
		/// <summary>値テキスト（テーブル,フィールド）</summary>
		public string[] ValueText { get; set; }
		#endregion

		/// <summary>
		/// クローン作成
		/// </summary>
		/// <returns>クローン</returns>
		public object Clone()
		{
			return new Field
			{
				JName = this.JName,
				Name = this.Name,
				Type = this.Type,
				Convert = this.Convert,
				Format = this.Format,
				ValueText = this.ValueText,
			};
		}
	}
}