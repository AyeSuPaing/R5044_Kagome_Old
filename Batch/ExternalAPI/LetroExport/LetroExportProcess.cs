/*
=========================================================================================================
  Module      : LetroExport メイン処理クラス(LetroExportProcess.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Linq;
using System.Text;
using w2.App.Common;
using w2.Common.Helper;
using w2.Common.Logger;

namespace ExternalAPI.LetroExport
{
	/// <summary>
	/// LetroExport メイン処理クラス
	/// </summary>
	public class LetroExportProcess
	{
		/// <summary>プロセス名</summary>
		public const string PROCESS_NAME = "LetroExport";
		/// <summary>実行コマンド</summary>
		public const string COMMAND_LINE_KEY = "-LetroExport";
		/// <summary>作業ディレクトリ名</summary>
		private const string WORK_DIR_NAME = "Work";
		/// <summary>成功ディレクトリ名</summary>
		private const string SUCCESS_DIR_NAME = "Success";
		/// <summary>失敗ディレクトリ名</summary>
		private const string ERROR_DIR_NAME = "Error";
		/// <summary>ファイル名 現在時刻指定キー</summary>
		private const string FILE_NAME_DATETIME_NOW = "now";

		/// <summary>File name last execute</summary>
		private const string FILENAME_LASTEXEC = "_LastExec";
		/// <summary>File name last execute</summary>
		private const string FILENAME_LASTEXEC_SEARCH_PATTERN = "_LastExec*";
		/// <summary>File content last execute date format</summary>
		private const string FILECONTENT_LASTEXEC_DATEFORMAT = "yyyyMMddHHmmss";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LetroExportProcess()
		{
			this.StartTime = DateTime.Now;
		}

		/// <summary>
		/// LetroExport 実行設定内容の指定
		/// </summary>
		/// <param name="exportSetting">LetroExport 実行設定内容</param>
		public void Setting(ExportSetting exportSetting)
		{
			this.ExportSetting = exportSetting;
		}

		/// <summary>
		/// 実行
		/// </summary>
		public void Exec()
		{
			ExportLog(string.Format("■実行モード：{0}", this.ExportSetting.ExecModeType.ToText()));

			ExportLog("作業用ディレクトリ準備開始");
			CreateDirectory();
			ExportLog("作業用ディレクトリ準備完了");

			this.ExportFileName = CreateExportFileName();
			var lastExecuteDatetime = GetLastExecuteDatetime();
			var workExportFilePath = Path.Combine(this.WorkDirectory.FullName, this.ExportFileNameWithExtension);
			try
			{
				ExportLog("出力ファイルを作業用ディレクトリに作成開始");
				new LetroExportCreateFile(this.ExportSetting).CreateFile(workExportFilePath, lastExecuteDatetime);
				ExportLog("出力ファイルを作業用ディレクトリに作成完了");

				ExportLog("出力ファイルの移動開始");
				new LetroExportFileMove(this.ExportSetting, this.SuccessDirectory).MoveFile(
					workExportFilePath,
					this.ExportFileNameWithExtension);
				ExportLog("出力ファイルの移動完了");

				UpdateLastExecuteDatetime();
			}
			catch (Exception ex)
			{
				if (File.Exists(workExportFilePath))
				{
					new LetroExportFileMove().MoveLogFile(
						workExportFilePath,
						Path.Combine(
							this.ErrorDirectory.FullName,
							this.ExportFileNameWithExtension));
				}

				var message = "例外が発生したため作業途中のファイルをErrorディレクトリに移動";
				ExportLog(message);
				throw new Exception(message, ex);
			}

			// ログの出力
			void ExportLog(string message)
			{
				Console.WriteLine(message);
				FileLogger.WriteInfo(message);
			}
		}

		/// <summary>
		/// 連携ファイル名の作成
		/// </summary>
		/// <returns>連携ファイル名</returns>
		private string CreateExportFileName()
		{
			var result = new StringBuilder();
			foreach (var value in this.ExportSetting.FileSetting.FileNameValue)
			{
				switch (value.FileNameValueType)
				{
					// 固定文字列を出力
					case FileNameValueType.Str:
						result.Append(value.Text);
						break;

					// 日時値を指定フォーマットで出力
					case FileNameValueType.DateString:
						var dateString = (value.Text == FILE_NAME_DATETIME_NOW)
								&& (string.IsNullOrEmpty(value.Format) == false)
							? this.StartTime.ToString(value.Format)
							: value.Text;
						result.Append(dateString);
						break;

					default:
						throw new ArgumentOutOfRangeException(
							nameof(value.FileNameValueType),
							value.FileNameValueType,
							null);
				}
			}

			return result.ToString();
		}

		/// <summary>
		/// 必要ディレクトリの作成
		/// </summary>
		private void CreateDirectory()
		{
			try
			{
				var exportDir = new DirectoryInfo(this.ExportSetting.FileSetting.ExportDir);

				if (exportDir.Exists == false) exportDir.Create();

				this.RootDirectory = new DirectoryInfo(Constants.SETTING_DIRPATH_LETROEXPORT);

				if (this.RootDirectory.Exists == false) this.RootDirectory.Create();

				this.WorkDirectory = new DirectoryInfo(Path.Combine(this.RootDirectory.FullName, WORK_DIR_NAME));

				if (this.WorkDirectory.Exists == false) this.WorkDirectory.Create();

				this.SuccessDirectory = new DirectoryInfo(Path.Combine(this.RootDirectory.FullName, SUCCESS_DIR_NAME));

				if (this.SuccessDirectory.Exists == false) this.SuccessDirectory.Create();

				this.ErrorDirectory = new DirectoryInfo(Path.Combine(this.RootDirectory.FullName, ERROR_DIR_NAME));

				if (this.ErrorDirectory.Exists == false) this.ErrorDirectory.Create();
			}
			catch (Exception ex)
			{
				throw new ApplicationException("必要ディレクトリの作成に失敗しました。", ex);
			}
		}

		/// <summary>
		/// Get last execute datetime
		/// </summary>
		/// <returns>Last execute datetime</returns>
		private DateTime? GetLastExecuteDatetime()
		{
			try
			{
				var lastExecuteFileInformation = this.RootDirectory.GetFiles(FILENAME_LASTEXEC_SEARCH_PATTERN)
					.OrderBy(item => item.LastWriteTime)
					.FirstOrDefault();
				if (lastExecuteFileInformation == null) return null;
				var fileName = Path.GetFileNameWithoutExtension(lastExecuteFileInformation.Name);
				var dateString = fileName.Substring(FILENAME_LASTEXEC.Length);
				var lastExecuteDateTime = DateTime.ParseExact(
					dateString,
					FILECONTENT_LASTEXEC_DATEFORMAT,
					null);
				return lastExecuteDateTime;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Update last execute datetime
		/// </summary>
		private void UpdateLastExecuteDatetime()
		{
			foreach (var item in this.RootDirectory.GetFiles(FILENAME_LASTEXEC_SEARCH_PATTERN))
			{
				item.Delete();
			}

			var filePath = Path.Combine(
				this.RootDirectory.FullName,
				string.Format("{0}{1}", FILENAME_LASTEXEC, this.StartTime.ToString(FILECONTENT_LASTEXEC_DATEFORMAT)));
			using (var fs = File.CreateText(filePath))
			{
			}
		}

		/// <summary>実行開始時刻</summary>
		public DateTime StartTime { get; private set; }
		/// <summary>LetroExport 実行設定内容</summary>
		public ExportSetting ExportSetting { get; private set; }
		/// <summary>連携ファイル名(拡張子あり)</summary>
		public string ExportFileNameWithExtension
		{
			get
			{
				return string.Format(
					"{0}.{1}",
					this.ExportFileName,
					(this.ExportSetting != null)
						? this.ExportSetting.FileSetting.Extension
						: string.Empty);
			}
		}
		/// <summary>連携ファイル名(拡張子なし)</summary>
		private string ExportFileName { get; set; }
		/// <summary>Root directory</summary>
		private DirectoryInfo RootDirectory { get; set; }
		/// <summary>作業ディレクトリ</summary>
		private DirectoryInfo WorkDirectory { get; set; }
		/// <summary>成功ディレクトリ</summary>
		private DirectoryInfo SuccessDirectory { get; set; }
		/// <summary>失敗ディレクトリ</summary>
		private DirectoryInfo ErrorDirectory { get; set; }
	}
}
