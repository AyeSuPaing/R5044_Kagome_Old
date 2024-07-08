/*
=========================================================================================================
  Module      : FreeExport 連携ファイルの移動処理クラス(FreeExportFileMove.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using w2.Common.Logger;
using w2.ExternalAPI.Common.Ftp;

namespace ExternalAPI.FreeExport
{
	/// <summary>
	/// 連携ファイルの移動処理クラス
	/// </summary>
	public class FreeExportFileMove
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="exportSetting">FreeExport 実行設定内容</param>
		/// <param name="successDir">成功ディレクトリ</param>
		/// <param name="errorDir">失敗ディレクトリ</param>
		public FreeExportFileMove(ExportSetting exportSetting, DirectoryInfo successDir, DirectoryInfo errorDir)
		{
			this.ExportSetting = exportSetting;
			this.SuccessDir = successDir;
			this.ErrorDir = errorDir;
		}

		/// <summary>
		/// 連携ファイルの移動
		/// </summary>
		/// <param name="workExportFilePath">作業ディレクトリ 連携ファイルパス</param>
		/// <param name="exportFileNameWithExtension">連携ファイル名(拡張子あり)</param>
		public void FileMove(string workExportFilePath, string exportFileNameWithExtension)
		{
			if (this.ExportSetting.FtpSetting.Use
				&& this.ExportSetting.CanExternalExportFile)
			{
				// FTP連携実行
				var fluentFtpUtility = new FluentFtpUtility(
					this.ExportSetting.FtpSetting.Host,
					this.ExportSetting.FtpSetting.Id,
					this.ExportSetting.FtpSetting.Password,
					this.ExportSetting.FtpSetting.IsActiveMode,
					this.ExportSetting.FtpSetting.EnableSsl);

				var uploadType = FluentFtpUtility.UploadType.Append;

				switch (this.ExportSetting.FileSetting.UploadType)
				{
					case UploadType.OverWrite:
						uploadType = FluentFtpUtility.UploadType.OverWrite;
						break;

					case UploadType.Append:
						uploadType = FluentFtpUtility.UploadType.Append;
						break;

					case UploadType.Skip:
						uploadType = FluentFtpUtility.UploadType.Skip;
						break;
				}

				var result = fluentFtpUtility.Upload(
					workExportFilePath,
					this.ExportSetting.FtpSetting.ExportDir + @"\" + exportFileNameWithExtension,
					uploadType);

				if (result)
				{
					LogFileMove(
						workExportFilePath,
						Path.Combine(this.SuccessDir.FullName, exportFileNameWithExtension));
				}
				else
				{
					throw new Exception("FTP連携に失敗しました。");
				}
			}
			else
			{
				// サーバ内でファイルのコピー
				if (string.IsNullOrEmpty(this.ExportSetting.FileSetting.ExportDir) == false)
				{
					var copyFilePath = Path.Combine(
						new DirectoryInfo(this.ExportSetting.FileSetting.ExportDir).FullName,
						exportFileNameWithExtension);

						switch (this.ExportSetting.FileSetting.UploadType)
						{
							case UploadType.OverWrite:
								File.Copy(workExportFilePath, copyFilePath, true);
								break;

							case UploadType.Append:
								File.Copy(workExportFilePath, copyFilePath, false);
								break;

							case UploadType.Skip:
								if (File.Exists(copyFilePath))
								{
									FileLogger.WriteInfo(
										"「スキップ」アップロード先に同一名ファイルが存在するためスキップしました。 アップロード元:"
										+ workExportFilePath + " アップロード先" + copyFilePath);
								}
								else
								{
									File.Copy(workExportFilePath, copyFilePath, false);
								}
								break;
						}
				}

				LogFileMove(workExportFilePath, Path.Combine(
					this.SuccessDir.FullName,
					exportFileNameWithExtension));
			}
		}

		/// <summary>
		/// ログファイルの移動 移動先のファイル名に日付を付与
		/// </summary>
		/// <param name="sourceFileNae">移動元グファイルパス</param>
		/// <param name="destFileName">移動先ログファイルパス</param>
		public static void LogFileMove(string sourceFileNae, string destFileName)
		{
			File.Move(sourceFileNae, destFileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
		}

		/// <summary>FreeExport 実行設定内容</summary>
		private ExportSetting ExportSetting { get; set; }
		/// <summary>成功ディレクトリ</summary>
		private DirectoryInfo SuccessDir { get; set; }
		/// <summary>失敗ディレクトリ</summary>
		private DirectoryInfo ErrorDir { get; set; }
	}
}