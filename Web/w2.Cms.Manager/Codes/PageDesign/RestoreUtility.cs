/*
=========================================================================================================
  Module      : 復元ユーティリティ(RestoreUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.IO;
using System.Linq;
using w2.App.Common.Design;

namespace w2.Cms.Manager.Codes.PageDesign
{
	/// <summary>復元タイプ</summary>
	public enum RestoreType
	{
		/// <summary>ページ</summary>
		Page,
		/// <summary>パーツ</summary>
		Parts
	}

	/// <summary>
	/// 復元ユーティリティ
	/// </summary>
	public class RestoreUtility
	{
		/// <summary>フロントデザインバックアップディレクトリ</summary>
		public static readonly string PHYSICALDIRPATH_FRONT_PC_BK = Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, Constants.PATH_AUTO_BACK_UP);

		/// <summary>
		/// バックアップファイルの作成
		/// </summary>
		/// <param name="targetFilePath">ターゲットファイルパス</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <param name="bkFileName">バックアップファイル名</param>
		/// <param name="restoreType">復元タイプ</param>
		public static void CreateBuckUpFile(
			string targetFilePath,
			string bkFileName,
			DesignCommon.DeviceType deviceType,
			RestoreType restoreType)
		{
			var bkDirPath = GetBuckUpDirectoryPath(
					targetFilePath,
					deviceType,
					restoreType);
			CreateIfThereIsNoDir(bkDirPath);

			var bkFilePath =Path.Combine(bkDirPath, bkFileName);
			while (new DirectoryInfo(bkDirPath).GetFiles().Length >= Constants.NUMBER_OF_GENERATIONS_HOLODING_FRONT_BACKUP)
			{
				var file = new DirectoryInfo(bkDirPath).GetFiles().OrderBy(f => f.CreationTime).First();
				if (File.Exists(file.FullName)) File.Delete(file.FullName);
			}

			if (File.Exists(targetFilePath) && (File.Exists(bkFilePath) == false)) File.Copy(targetFilePath, bkFilePath, true);
		}

		/// <summary>
		/// ディレクトリが存在しなければ新しく作成する
		/// </summary>
		/// <param name="dirPath">ディレクトリパス</param>
		private static void CreateIfThereIsNoDir(string dirPath)
		{
			if (Directory.Exists(dirPath) == false) Directory.CreateDirectory(dirPath);
		}

		/// <summary>
		/// バックアップディレクトリパスを取得
		/// </summary>
		/// <param name="targetFilePath">ターゲットファイルパス</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <param name="restoreType">復元タイプ</param>
		/// <returns>バックアップディレクトリパス</returns>
		public static string GetBuckUpDirectoryPath(
			string targetFilePath, 
			DesignCommon.DeviceType deviceType, 
			RestoreType restoreType)
		{
			var originalFileName = Path.GetFileName(targetFilePath);
			var pageType = Path.GetFileName(Path.GetDirectoryName(targetFilePath));

			string bkDirPath;
			switch (pageType)
			{
				case "Page":
					bkDirPath = Path.Combine(PHYSICALDIRPATH_FRONT_PC_BK, pageType, originalFileName, deviceType.ToString()) + @"\\";
					break;

				case "Product":
				case "Order":
				case "Coordinate":
					if (restoreType == RestoreType.Page)
					{
						bkDirPath = Path.Combine(PHYSICALDIRPATH_FRONT_PC_BK, "Form", pageType, originalFileName, deviceType.ToString()) + @"\\";
					}
					else
					{
						bkDirPath = Path.Combine(PHYSICALDIRPATH_FRONT_PC_BK, "Form", "Common", pageType, originalFileName, deviceType.ToString()) + @"\\";
					}
					break;

				case "Parts":
					bkDirPath = Path.Combine(PHYSICALDIRPATH_FRONT_PC_BK, "Page", pageType, originalFileName, deviceType.ToString()) + @"\\";
					break;

				default:
					bkDirPath = Path.Combine(PHYSICALDIRPATH_FRONT_PC_BK, originalFileName, deviceType.ToString()) + @"\\";
					break;
			}

			return bkDirPath;
		}
	}
}