/*
=========================================================================================================
  Module      : FreeExport メイン処理クラス(FreeExportProcess.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Text;
using w2.App.Common;
using w2.Common.Helper;
using w2.Common.Logger;

namespace ExternalAPI.FreeExport
{
	/// <summary>
	/// FreeExport メイン処理クラス
	/// </summary>
	public class FreeExportProcess
	{
		/// <summary>プロセス名</summary>
		public const string PROCESS_NAME = "FreeExport";
		/// <summary>実行コマンド</summary>
		public const string COMMAND_LINE_KEY = "-FreeExport";
		/// <summary>バッチ内作業ディレクトリRootパス</summary>
		private const string ROOT_DIR_PATH = @".\FreeExport";
		/// <summary>作業ディレクトリ名</summary>
		private const string WORK_DIR_NAME = "Work";
		/// <summary>成功ディレクトリ名</summary>
		private const string SUCCESS_DIR_NAME = "Success";
		/// <summary>失敗ディレクトリ名</summary>
		private const string ERROR_DIR_NAME = "Error";
		/// <summary>ファイル名 現在時刻指定キー</summary>
		private const string FILE_NAME_DATETIME_NOW = "now";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FreeExportProcess()
		{
			this.StartTime = DateTime.Now;
		}

		/// <summary>
		/// FreeExport 実行設定内容の指定
		/// </summary>
		/// <param name="exportSetting">FreeExport 実行設定内容</param>
		public void Setting(ExportSetting exportSetting)
		{
			this.ExportSetting = exportSetting;
		}

		/// <summary>
		/// 実行
		/// </summary>
		public void Exec()
		{
			ExecLogOutPut("■実行モード：" + this.ExportSetting.ExecModeType.ToText());

			ExecLogOutPut("作業用ディレクトリ準備開始");
			CreateDir();
			ExecLogOutPut("作業用ディレクトリ準備完了");

			this.ExportFileName = CreateExportFileName();
			var workExportFilePath = Path.Combine(this.WorkDir.FullName, this.ExportFileNameWithExtension);
			var workUpdateDateFilePath = Path.Combine(this.WorkDir.FullName, this.UpdateDateFileNameWithExtension);

			try
			{
				ExecLogOutPut("出力ファイルを作業用ディレクトリに作成開始");
				new FreeExportCreateFile(this.ExportSetting).CreateFile(workExportFilePath, workUpdateDateFilePath);
				ExecLogOutPut("出力ファイルを作業用ディレクトリに作成完了");

				ExecLogOutPut("出力ファイルの移動開始");
				new FreeExportFileMove(this.ExportSetting, this.SuccessDir, this.ErrorDir).FileMove(
					workExportFilePath,
					this.ExportFileNameWithExtension);
				ExecLogOutPut("出力ファイルの移動完了");

				ExecLogOutPut("DB更新開始");
				new FreeExportDbUpdate(this.ExportSetting).Update(workUpdateDateFilePath);
				ExecLogOutPut("DB更新完了");
			}
			catch (Exception e)
			{
				if (File.Exists(workExportFilePath))
				{
					FreeExportFileMove.LogFileMove(workExportFilePath, Path.Combine(
						this.ErrorDir.FullName,
						this.ExportFileNameWithExtension));
				}

				if (File.Exists(workUpdateDateFilePath))
				{
					FreeExportFileMove.LogFileMove(workUpdateDateFilePath, Path.Combine(
						this.ErrorDir.FullName,
						this.UpdateDateFileNameWithExtension));
				}

				var message = "例外が発生したため作業途中のファイルをErrorディレクトリに移動";
				ExecLogOutPut(message);
				throw new Exception(message, e);
			}
		}

		/// <summary>
		/// ログの出力
		/// </summary>
		/// <param name="message">ログ内容</param>
		private void ExecLogOutPut(string message)
		{
			Console.WriteLine(message);
			AppLogger.WriteInfo(message);
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
						var temp = ((value.Text == FILE_NAME_DATETIME_NOW)
							&& (string.IsNullOrEmpty(value.Format) == false))
							? this.StartTime.ToString(value.Format)
							: value.Text;
						result.Append(temp);
						break;
				}
			}

			return result.ToString();
		}

		/// <summary>
		/// 必要ディレクトリの作成
		/// </summary>
		private void CreateDir()
		{
			try
			{
				var exportDir = new DirectoryInfo(this.ExportSetting.FileSetting.ExportDir);

				if (exportDir.Exists == false) exportDir.Create();

				var rootDir = new DirectoryInfo(Constants.SETTING_DIRPATH_FREEEXPORT);

				if (rootDir.Exists == false) rootDir.Create();

				this.WorkDir = new DirectoryInfo(Path.Combine(rootDir.FullName, WORK_DIR_NAME));

				if (this.WorkDir.Exists == false) this.WorkDir.Create();

				this.SuccessDir = new DirectoryInfo(Path.Combine(rootDir.FullName, SUCCESS_DIR_NAME));

				if (this.SuccessDir.Exists == false) this.SuccessDir.Create();

				this.ErrorDir = new DirectoryInfo(Path.Combine(rootDir.FullName, ERROR_DIR_NAME));

				if (this.ErrorDir.Exists == false) this.ErrorDir.Create();
			}
			catch (Exception ex)
			{
				throw new ApplicationException("必要ディレクトリの作成に失敗しました。", ex);
			}
		}

		/// <summary>実行開始時刻</summary>
		public DateTime StartTime { get; private set; }
		/// <summary>FreeExport 実行設定内容</summary>
		public ExportSetting ExportSetting { get; private set; }
		/// <summary>連携ファイル名(拡張子あり)</summary>
		public string ExportFileNameWithExtension
		{
			get
			{
				return
					this.ExportFileName
					+ "."
					+ ((this.ExportSetting != null) ? this.ExportSetting.FileSetting.Extension : "");
			}
		}
		/// <summary>連携ファイル名(拡張子なし)</summary>
		private string ExportFileName { get; set; }
		/// <summary>更新データファイル名(拡張子あり)</summary>
		private string UpdateDateFileNameWithExtension
		{
			get { return string.Format("update_sql_{0}.csv", this.ExportFileName); }
		}
		/// <summary>作業ディレクトリ</summary>
		private DirectoryInfo WorkDir { get; set; }
		/// <summary>成功ディレクトリ</summary>
		private DirectoryInfo SuccessDir { get; set; }
		/// <summary>失敗ディレクトリ</summary>
		private DirectoryInfo ErrorDir { get; set; }
	}
}