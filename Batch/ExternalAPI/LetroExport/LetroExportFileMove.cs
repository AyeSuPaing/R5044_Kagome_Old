/*
=========================================================================================================
  Module      : LetroExport 連携ファイルの移動処理クラス(LetroExportFileMove.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using w2.Common.Logger;
using w2.ExternalAPI.Common.Ftp;

namespace ExternalAPI.LetroExport
{
	/// <summary>
	/// 連携ファイルの移動処理クラス
	/// </summary>
	public class LetroExportFileMove
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public LetroExportFileMove()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="exportSetting">LetroExport 実行設定内容</param>
		/// <param name="successDir">成功ディレクトリ</param>
		public LetroExportFileMove(
			ExportSetting exportSetting,
			DirectoryInfo successDir)
		{
			this.ExportSetting = exportSetting;
			this.SuccessDirectory = successDir;
		}

		/// <summary>
		/// 連携ファイルの移動
		/// </summary>
		/// <param name="workExportFilePath">作業ディレクトリ 連携ファイルパス</param>
		/// <param name="exportFileNameWithExtension">連携ファイル名(拡張子あり)</param>
		public void MoveFile(string workExportFilePath, string exportFileNameWithExtension)
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
				FluentFtpUtility.UploadType uploadType;
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

					default:
						throw new ArgumentOutOfRangeException(
							nameof(this.ExportSetting.FileSetting.UploadType),
							this.ExportSetting.FileSetting.UploadType,
							null);
				}

				var result = fluentFtpUtility.Upload(
					workExportFilePath,
					Path.Combine(this.ExportSetting.FtpSetting.ExportDir, exportFileNameWithExtension),
					uploadType);
				if (result)
				{
					throw new Exception("FTP連携に失敗しました。");
					MoveLogFile(
						workExportFilePath,
						Path.Combine(this.SuccessDirectory.FullName, exportFileNameWithExtension));
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
									string.Format(
										"「スキップ」アップロード先に同一名ファイルが存在するためスキップしました。 アップロード元:{0} アップロード先{1}",
										workExportFilePath,
										copyFilePath));
							}
							else
							{
								File.Copy(workExportFilePath, copyFilePath, false);
							}
							break;

						default:
							throw new ArgumentOutOfRangeException(
								nameof(this.ExportSetting.FileSetting.UploadType),
								this.ExportSetting.FileSetting.UploadType,
								null);
					}
				}

				MoveLogFile(
					workExportFilePath,
					Path.Combine(this.SuccessDirectory.FullName, exportFileNameWithExtension));
			}
		}

		/// <summary>
		/// ログファイルの移動 移動先のファイル名に日付を付与
		/// </summary>
		/// <param name="sourceFileName">移動元グファイルパス</param>
		/// <param name="destFileName">移動先ログファイルパス</param>
		public void MoveLogFile(string sourceFileName, string destFileName)
		{
			File.Move(
				sourceFileName,
				string.Format(
					"{0}_{1:yyyyMMddHHmmss}",
					destFileName,
					DateTime.Now));
		}

		/// <summary>LetroExport 実行設定内容</summary>
		private ExportSetting ExportSetting { get; set; }
		/// <summary>成功ディレクトリ</summary>
		private DirectoryInfo SuccessDirectory { get; set; }
	}
}
