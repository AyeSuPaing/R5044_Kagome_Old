/*
=========================================================================================================
  Module      : 定数クラス(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Commerce.Batch.SilvereggAigentCsvFileUploader
{
	/// <summary>
	/// 定数
	/// </summary>
	public class Constants : w2.App.Common.Constants
	{
		/// <summary>CSVファイル格納ディレクトリ</summary>
		public static string PHYSICALDIRPATH_CSV_FILES = null;

		/// <summary>CSVファイル文字エンコード</summary>
		public static string CSV_ENCODE = null;

		/// <summary>商品マスタCSVファイル名</summary>
		public static string CSV_FILENAME_PRODUCT = null;

		/// <summary>カテゴリマスタCSVファイル名</summary>
		public static string CSV_FILENAME_CATEGORY = null;

		/// <summary>商品マスタCSVファイル名</summary>
		public static string CSV_FILENAME_PRODUCT_CAT = null;

		/// <summary>FTP送信先ファイルパス</summary>
		public static string DESTINATION_FILE_PATH = null;

		/// <summary>改行コード</summary>
		public const string CR_LF_CODE = "\r\n";

		/// <summary>デフォルト画像名</summary>
		public const string PRODUCTIMAGE_NOIMAGE_HEADER = "NowPrinting";
	}
}
