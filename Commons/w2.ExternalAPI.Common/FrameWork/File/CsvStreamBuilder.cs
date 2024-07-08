/*
=========================================================================================================
  Module      : Csv読み書きクラス用ビルダー(CsvStreamBuilder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System.IO;

namespace w2.ExternalAPI.Common.FrameWork.File
{
	/// <summary>
	/// CSV読み書きクラス用ビルダー
	/// </summary>
	public static class CsvStreamBuilder
	{
		#region +BuildCsvReader CSV読み込みクラス生成
		/// <summary>
		/// CSV読み込みクラス生成
		/// </summary>
		/// <param name="filePath">読み込みファイルパス</param>
		/// <param name="csvSetting">読み込みファイル用CSV情報構造体</param>
		/// <returns>CSV読み込みクラス</returns>
		public static CsvReader BuildCsvReader(string filePath, CsvSetting csvSetting)
		{
			return new CsvReader(new StreamReader(filePath, csvSetting.Encoding), csvSetting);
		}
		#endregion

		#region +BuildCsvWriter CSV書き込みクラス生成
		/// <summary>
		/// CSV書き込みクラス生成
		/// </summary>
		/// <param name="filePath">書き込みファイルパス</param>
		/// <param name="csvSetting">書き込みファイル用CSV情報構造体</param>
		/// <returns>CSV書き込みクラス</returns>
		public static CsvWriter BuildCsvWriter(string filePath,CsvSetting csvSetting)
		{
			return new CsvWriter(new StreamWriter(filePath,false, csvSetting.Encoding),csvSetting);
		}
		#endregion
	}
}
