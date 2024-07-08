/*
=========================================================================================================
  Module      : Facebook External Setting Manager (FacebookExternalSettingManager.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using w2.App.Common.Facebook;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.Commerce.Batch.LiaiseFacebookMall.Settings
{
	/// <summary>
	/// Facebook External Setting Manager
	/// </summary>
	public class FacebookExternalSettingManager
	{
		/// <summary>Setting directory name</summary>
		private const string SETTING_FILE_DIR = "Settings";
		/// <summary>Setting Facebook external file name</summary>
		private const string SETTING_FILE_NAME = "FacebookExternalSetting.xml";

		/// <summary>シングルトン管理 インスタンス</summary>
		private static FacebookExternalSettingManager s_instance;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FacebookExternalSettingManager()
		{
			ReadSettingFile();
		}

		/// <summary>
		/// シングルトン管理 インスタンスの取得
		/// </summary>
		/// <returns>インスタンス</returns>
		public static FacebookExternalSettingManager GetInstance()
		{
			return s_instance ?? (s_instance = new FacebookExternalSettingManager());
		}

		/// <summary>
		/// FacebookExternalSettingファイルの読み込み
		/// </summary>
		private void ReadSettingFile()
		{
			if (File.Exists(this.SettingFileFullPath) == false)
			{
				throw new Exception(
					string.Format(
						"Facebook連携設定ファイル「{0}」のが見つかりませんでした。",
					this.SettingFileFullPath));
			}

			try
			{
				using (var fs = File.OpenRead(this.SettingFileFullPath))
				{
					this.Setting =
						(FacebookExternalSetting)new XmlSerializer(typeof(FacebookExternalSetting))
							.Deserialize(fs);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(
					string.Format(
						"Facebook連携設定ファイル「{0}」の読み込みに失敗しました。",
						this.SettingFileFullPath),
					ex);
			}
		}

		/// <summary>
		/// Get API requests
		/// </summary>
		/// <param name="exhibitsFlgName">Exhibits flag name</param>
		/// <returns>A list of Facebook catalog request field api</returns>
		public FacebookCatalogRequestFieldApi[] GetApiRequests(string exhibitsFlgName)
		{
			var requests = new List<FacebookCatalogRequestFieldApi>();

			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement(this.Setting.ExportSql.Sql))
			{
				// 連携ファイル作成用クエリの作成
				statement.CommandTimeout = this.Setting.ExportSql.ExecTimeOut;
				var whereClause = string.Format(
					"{0}.{1} = '{2}'",
					Constants.TABLE_MALLEXHIBITSCONFIG,
					exhibitsFlgName,
					Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON);
				statement.ReplaceStatement("@@ exhibits_flg_where @@", whereClause);
				statement.ReplaceStatement(
					"@@ fields @@",
					string.Join(
						",\r\n",
						this.Setting.Fields.Select(field => string.Format("({0})", field.Value))));

				accessor.OpenConnection();
				using (var reader = new SqlStatementDataReader(accessor, statement))
				{
					while (reader.Read())
					{
						var dataLine = GetData(reader);
						var dataRequest = new FacebookExternalData(dataLine);
						requests.Add(dataRequest.GetDataRequestFields());
					}
				}
			}

			return requests.ToArray();
		}

		/// <summary>
		/// Get data
		/// </summary>
		/// <param name="dataReader">Data reader</param>
		/// <returns>Data</returns>
		private Hashtable GetData(SqlStatementDataReader dataReader)
		{
			var result = new Hashtable();
			for (var i = 0; i < dataReader.FieldCount; i++)
			{
				result.Add(
					this.Setting.Fields[i].ApiRequestKey,
					StringUtility.ToEmpty(dataReader[i]));
			}
			return result;
		}

		/// <summary>Facebook external setting</summary>
		private FacebookExternalSetting Setting { get; set; }
		/// <summary>Setting file full path</summary>
		private string SettingFileFullPath
		{
			get
			{
				return Path.Combine(
					Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE,
					SETTING_FILE_DIR,
					SETTING_FILE_NAME);
			}
		}
	}

	/// <summary>
	/// Facebook External Setting
	/// </summary>
	[XmlRoot("FacebookExternalSetting")]
	public class FacebookExternalSetting
	{
		/// <summary>対象取得用SQL</summary>
		[XmlElement("ExportSql")]
		public ExportSql ExportSql { get; set; }
		/// <summary>出力フィールド設定配列</summary>
		[XmlArray("Fields")]
		[XmlArrayItem("Field")]
		public Field[] Fields { get; set; }
	}

	/// <summary>
	/// 対象取得用SQL
	/// </summary>
	[Serializable]
	public class ExportSql
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ExportSql()
		{
			this.ExecTimeOut = 60;
		}

		/// <summary>SQL実行タイムアウト時間</summary>
		[XmlAttribute("ExecTimeOut")]
		public int ExecTimeOut { get; set; }
		/// <summary>実行SQL</summary>
		[XmlElement("Sql")]
		public string Sql { get; set; }
	}

	/// <summary>
	/// 出力フィールド設定
	/// </summary>
	[Serializable]
	public class Field
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Field()
		{
			this.ApiRequestKey = string.Empty;
			this.ExportFlg = true;
		}

		/// <summary>Api request key</summary>
		[XmlAttribute("ApiRequestKey")]
		public string ApiRequestKey { get; set; }
		/// <summary>Export flag</summary>
		[XmlAttribute("ExportFlg")]
		public bool ExportFlg { get; set; }
		/// <summary>出力SQL</summary>
		[XmlElement("Value")]
		public string Value { get; set; }
	}
}