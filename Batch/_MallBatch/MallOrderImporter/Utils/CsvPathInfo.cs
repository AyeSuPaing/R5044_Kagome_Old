/*
=========================================================================================================
  Module      : CSVパス情報(CsvPathInfo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace w2.Commerce.MallBatch.MallOrderImporter.Utils
{
	/// <summary>
	/// CSVパス情報
	/// </summary>
	public class CsvPathInfo
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="fileType">ファイルタイプ</param>
		public CsvPathInfo(string fileType)
		{
			this.FileType = fileType;
			this.FileName = "";
		}

		/// <summary>ファイルタイプ</summary>
		public string FileType { get; private set; }
		/// <summary>ファイル名</summary>
		public string FileName { get; set; }
		/// <summary>アップロードディレクトリパス</summary>
		public string UploadDirectoryPath
		{
			get { return Path.Combine(Constants.PHYSICALDIRPATH_EXTERNALFILEUPLOAD, Constants.CONST_DEFAULT_SHOP_ID, this.FileType, Constants.DIRNAME_MASTERIMPORT_UPLOAD); }
		}
		/// <summary>アクティブディレクトリパス</summary>
		public string ActiveDirectoryPath
		{
			get { return Path.Combine(Constants.PHYSICALDIRPATH_EXTERNALFILEUPLOAD, Constants.CONST_DEFAULT_SHOP_ID, this.FileType, Constants.DIRNAME_MASTERIMPORT_ACTIVE); }
		}
		/// <summary>アップロードファイルパス</summary>
		public string UploadFilePath
		{
			get { return Path.Combine(this.UploadDirectoryPath, this.FileName); }
		}
		/// <summary>アクティブファイルパス</summary>
		public string ActiveFilePath
		{
			get { return Path.Combine(this.ActiveDirectoryPath, this.FileName); }
		}

	}
}
