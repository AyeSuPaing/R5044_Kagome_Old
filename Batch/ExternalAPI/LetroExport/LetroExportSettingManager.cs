/*
=========================================================================================================
  Module      : LetroExport 設定ファイル管理クラス(LetroExportSettingManager.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using ExternalAPI.FreeExport;
using System;
using System.IO;
using System.Xml.Serialization;
using w2.Common.Helper;
using w2.Common.Helper.Attribute;

namespace ExternalAPI.LetroExport
{
	/// <summary>
	/// LetroExport 設定ファイル管理クラス
	/// </summary>
	public class LetroExportSettingManager
	{
		/// <summary>シングルトン管理 インスタンス</summary>
		private static readonly LetroExportSettingManager s_instance = new LetroExportSettingManager();
		/// <summary>設定ファイルパス</summary>
		private const string SETTING_FILE_PATH = @"ExternalApi\LetroConnectingSettings.xml";

		/// <summary>
		/// LetroExportSettingファイルの読み込み
		/// </summary>
		public void ReadSettingFile()
		{
			var settingFilePath = Path.Combine(
				Properties.Settings.Default.ConfigFileDirPath,
				SETTING_FILE_PATH);

			if (File.Exists(settingFilePath) == false)
			{
				throw new Exception(string.Format("設定ファイル「{0}」のが見つかりませんでした。", settingFilePath));
			}

			try
			{
				using (var fs = File.OpenRead(settingFilePath))
				{
					this.LetroExportSetting =
						(LetroExportSetting)new XmlSerializer(typeof(LetroExportSetting)).Deserialize(fs);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("設定ファイル「{0}」の読み込みに失敗しました。", settingFilePath), ex);
			}
		}

		/// <summary>
		/// シングルトン管理 インスタンスの取得
		/// </summary>
		/// <returns>インスタンス</returns>
		public static LetroExportSettingManager GetInstance()
		{
			return s_instance;
		}

		/// <summary>LetroExportSetting設定内容</summary>
		public LetroExportSetting LetroExportSetting { get; private set; }
	}

	/// <summary>
	/// LetroExportSetting設定 Rootクラス
	/// </summary>
	[XmlRoot("Root")]
	public class LetroExportSetting
	{
		/// <summary>コマンドキー毎の実行設定</summary>
		[XmlElement("ExportSetting")]
		public ExportSetting[] ExportSettings { get; set; }
	}

	/// <summary>
	/// 実行モードタイプ
	/// </summary>
	[Serializable]
	public enum ExecModeType
	{
		/// <summary>本番：FTP連携がONの場合は外部に出力　更新用のクエリが存在する場合は更新を実行</summary>
		[EnumTextName("本番")]
		[XmlEnum("production")]
		Production,
		/// <summary>デバック：FTP連携がONの場合での外部出力されない　更新用のクエリが存在する場合でも更新が実行されない</summary>
		[EnumTextName("デバック")]
		[XmlEnum("debug")]
		Debug
	}

	/// <summary>
	/// コマンドキー毎の実行設定 ExportSettingクラス
	/// </summary>
	[Serializable]
	public class ExportSetting
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ExportSetting()
		{
			this.FtpSetting = new FtpSetting();
			this.ExecModeType = ExecModeType.Debug;
		}

		/// <summary>コマンドキー</summary>
		[XmlAttribute("CommandKey")]
		public string CommandKey { get; set; }
		/// <summary>実行モードタイプ</summary>
		[XmlAttribute("ExecModeType")]
		public ExecModeType ExecModeType { get; set; }
		/// <summary>連携ファイル設定</summary>
		[XmlElement("FileSetting")]
		public FileSetting FileSetting { get; set; }
		/// <summary>FTP接続設定</summary>
		[XmlElement("FtpSetting")]
		public FtpSetting FtpSetting { get; set; }
		/// <summary>出力内容の設定</summary>
		[XmlElement("SearchSetting")]
		public SearchSetting SearchSetting { get; set; }
		/// <summary>外部にファイルを出力可能か</summary>
		public bool CanExternalExportFile
		{
			get
			{
				switch (this.ExecModeType)
				{
					case ExecModeType.Production:
						return true;

					case ExecModeType.Debug:
						return false;

					default:
						throw new ArgumentOutOfRangeException(
							nameof(this.ExecModeType),
							this.ExecModeType,
							null);
				}
			}
		}
	}

	/// <summary>
	/// 連携ファイル設定
	/// </summary>
	[Serializable]
	public class FileSetting
	{
		/// <summary>コンストラクタ</summary>
		public FileSetting()
		{
			this.UploadType = UploadType.Append;
		}

		/// <summary>サーバ内出力設定</summary>
		[XmlElement("ExportDir")]
		public string ExportDir { get; set; }
		/// <summary>拡張子</summary>
		[XmlElement("Extension")]
		public string Extension { get; set; }
		/// <summary>区切り文字</summary>
		[XmlElement("Separator")]
		public Separator Separator { get; set; }
		/// <summary>ヘッダーの出力有無</summary>
		[XmlElement("HeaderExport")]
		public bool HeaderExport { get; set; }
		/// <summary>引用符</summary>
		[XmlElement("QuotationMark")]
		public string QuotationMark { get; set; }
		/// <summary>文字コード</summary>
		[XmlElement("Encoding")]
		public string EncodingType { get; set; }
		/// <summary>ファイル名 構成配列</summary>
		[XmlArray("FileName")]
		[XmlArrayItem("Value")]
		public FileNameValue[] FileNameValue { get; set; }
		/// <summary>アップロードタイプ</summary>
		[XmlElement("UploadType")]
		public UploadType UploadType { get; set; }
	}

	/// <summary>
	/// 区切り文字タイプ
	/// </summary>
	[Serializable]
	public enum SeparatorType
	{
		/// <summary>入力</summary>
		[EnumTextName("")]
		[XmlEnum("input")]
		Input,
		/// <summary>タブ</summary>
		[EnumTextName("\t")]
		[XmlEnum("tab")]
		Tab
	}

	/// <summary>
	/// アップロードタイプ
	/// </summary>
	[Serializable]
	public enum UploadType
	{
		/// <summary>上書き許可</summary>
		[EnumTextName("上書き許可")]
		[XmlEnum("overWrite")]
		OverWrite,
		/// <summary>上書き禁止</summary>
		[EnumTextName("上書き禁止")]
		[XmlEnum("append")]
		Append,
		/// <summary>アップロード先に同一ファイルが存在する場合にアップロードをスキップ</summary>
		[EnumTextName("同一ファイルが存在する場合にアップロードをスキップ")]
		[XmlEnum("skip")]
		Skip
	}

	/// <summary>
	/// 区切り文字
	/// </summary>
	[Serializable]
	public class Separator
	{
		/// <summary>区切り文字タイプ</summary>
		[XmlAttribute("Type")]
		public SeparatorType Type { get; set; }
		/// <summary>タイプ入力時の区切り文字</summary>
		[XmlText]
		public string Text { get; set; }
		/// <summary>区切り文字</summary>
		public string Value
		{
			get
			{
				string separator;
				switch (this.Type)
				{
					case SeparatorType.Input:
						separator = this.Text;
						break;

					case SeparatorType.Tab:
						separator = SeparatorType.Tab.ToText();
						break;

					default:
						throw new ArgumentOutOfRangeException(
							nameof(this.Type),
							this.Type,
							null);
				}

				return separator;
			}
		}
	}

	/// <summary>
	/// ファイル名 構成内容タイプ
	/// </summary>
	[Serializable]
	public enum FileNameValueType
	{
		/// <summary>文字列</summary>
		[XmlEnum("string")]
		Str,
		/// <summary>日付</summary>
		[XmlEnum("dateString")]
		DateString
	}

	/// <summary>
	/// ファイル名 構成内容
	/// </summary>
	[Serializable]
	public class FileNameValue
	{
		/// <summary>ファイル名 構成内容タイプ</summary>
		[XmlAttribute("Type")]
		public FileNameValueType FileNameValueType { get; set; }
		/// <summary>ファイル名 日付フォーマット</summary>
		[XmlAttribute("Format")]
		public string Format { get; set; }
		/// <summary>ファイル名 内容</summary>
		[XmlText]
		public string Text { get; set; }
	}

	/// <summary>
	/// FTP接続設定
	/// </summary>
	[Serializable]
	public class FtpSetting
	{
		/// <summary>利用有無</summary>
		[XmlAttribute("Use")]
		public bool Use { get; set; }
		/// <summary>アクティブモードか</summary>
		[XmlAttribute("IsActiveMode")]
		public bool IsActiveMode { get; set; }
		/// <summary>SSL利用か</summary>
		[XmlAttribute("EnableSsl")]
		public bool EnableSsl { get; set; }
		/// <summary>接続先ホスト名</summary>
		[XmlElement("Host")]
		public string Host { get; set; }
		/// <summary>ID</summary>
		[XmlElement("Id")]
		public string Id { get; set; }
		/// <summary>パスワード</summary>
		[XmlElement("Password")]
		public string Password { get; set; }
		/// <summary>出力先ディレクトリ</summary>
		[XmlElement("ExportDir")]
		public string ExportDir { get; set; }
	}

	/// <summary>
	/// 抽出設定
	/// </summary>
	[Serializable]
	public class SearchSetting
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SearchSetting()
		{
		}

		/// <summary>対象取得用SQL</summary>
		[XmlElement("ExportSql")]
		public ExecSql ExportSql { get; set; }
		/// <summary>出力フィールド設定配列</summary>
		[XmlArray("Fields")]
		[XmlArrayItem("Field")]
		public Field[] Fields { get; set; }
	}

	/// <summary>
	/// 対象取得用SQ
	/// </summary>
	[Serializable]
	public class ExecSql
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ExecSql()
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
	/// Convert url
	/// </summary>
	[Serializable]
	public enum ConvertUrl
	{
		/// <summary>No conversion</summary>
		None,
		/// <summary>Convert field value to product image url</summary>
		[XmlEnum("ProductImageHead")]
		ProductImageHead,
		/// <summary>Convert field value to product detail url</summary>
		[XmlEnum("ProductDetail")]
		ProductDetail,
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
			this.ExportFlg = true;
			this.StartBytePosition = -1;
			this.ByteLength = -1;
			this.ConvertString = ConvertString.None;
		}

		/// <summary>ヘッダー名</summary>
		[XmlAttribute("HeaderName")]
		public string HeaderName { get; set; }
		/// <summary>連携ファイルに出力するか</summary>
		[XmlAttribute("ExportFlg")]
		public bool ExportFlg { get; set; }
		/// <summary>バイト長切り出しスタート位置</summary>
		[XmlAttribute("StartBytePosition")]
		public int StartBytePosition { get; set; }
		/// <summary>バイト長切り出し長さ</summary>
		[XmlAttribute("ByteLength")]
		public int ByteLength { get; set; }
		/// <summary>文字列変換タイプ</summary>
		[XmlAttribute("ConvertString")]
		public ConvertString ConvertString { get; set; }
		/// <summary>出力SQL</summary>
		[XmlElement("Value")]
		public string Value { get; set; }
		/// <summary>Url conversion type</summary>
		[XmlAttribute("ConvertUrl")]
		public ConvertUrl ConvertUrl { get; set; }
	}
}
