/*
=========================================================================================================
  Module      : File Defines(FileDefines.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Xml.Linq;
using System.Linq;
using System.IO;

namespace w2.Commerce.Batch.CustomerRingsExport
{
	/// <summary>
	/// 出力ファイルの定義一覧クラス
	/// </summary>
	public class FileDefines
	{

		public enum Export
		{
			All,
			Differential,
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settingFilePath">設定ファイルパス</param>
		public FileDefines(string settingFilePath)
		{
			LoadBySetting(settingFilePath);
		}

		/// <summary>
		/// 設定ファイル読込
		/// </summary>
		/// <param name="settingFilePath">設定ファイルパス</param>
		public void LoadBySetting(string settingFilePath)
		{
			if (File.Exists(settingFilePath) == false) return;

			// 設定ファイルの初期化
			var doc = XDocument.Load(settingFilePath);
			if (doc.Root == null) return;

			this.Defines = doc.Root.Elements()
				.Select(file => new FileDefine()
				{
					Name = file.Attribute("name").Value,
					FieldSeparator = file.Attribute("fieldseparator").Value,
					Enclosed = file.Attribute("enclosed").Value,
					Encoding = file.Attribute("encoding").Value,
					ExportHeader = bool.Parse(file.Attribute("exportheader").Value),
					IsUtf8WithBOM = bool.Parse(file.Attribute("utf8withbom").Value),
					IsEscapeCsv = bool.Parse(file.Attribute("escapecsv").Value),
					NewLineReplaceString = file.Attribute("newlinereplacestring").Value,
					//Export = Enum.Parse(typeof(FileDefines.Export), file.Attribute("export").Value)
				}).ToArray();
		}

		/// <summary>
		/// ファイル定義一覧
		/// </summary>
		public FileDefine[] Defines { get; private set; }

		/// <summary>
		/// 出力ファイルの定義クラス
		/// </summary>
		public class FileDefine
		{
			/// <summary>出力ファイル名接頭辞（出力ファイル種類）</summary>
			public string Name { get; set; }
			/// <summary>区切り文字</summary>
			public string FieldSeparator { get; set; }
			/// <summary>列の引用符</summary>
			public string Enclosed { get; set; }
			/// <summary>エンコーディング</summary>
			public string Encoding { get; set; }
			/// <summary>BOM付きUTF-8かどうか</summary>
			public bool IsUtf8WithBOM { get; set; }
			/// <summary>ヘッダ行出力有無</summary>
			public bool ExportHeader { get; set; }
			/// <summary>Csvエスケープ対応かどうか</summary>
			public bool IsEscapeCsv { get; set; }
			/// <summary>改行文字から変換文字列</summary>
			public string NewLineReplaceString { get; set; }
			public FileDefines.Export Export { get; set; }
		}
	}
}
