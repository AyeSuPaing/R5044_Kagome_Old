/*
=========================================================================================================
  Module      : メイン処理クラス(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using ExternalAPI.FreeExport;
using ExternalAPI.LetroExport;
using ExternalAPI.Mail;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using w2.App.Common.Util;
using w2.Common;
using w2.Common.Logger;
using w2.ExternalAPI.Common;
using w2.ExternalAPI.Common.Ftp;
using w2.ExternalAPI.Common.Logging;

namespace ExternalAPI
{
	/// <summary>
	///	メイン処理クラス
	/// </summary>
	class Program
	{
		#region +Main メイン処理
		/// <summary>
		///	メイン処理
		/// </summary>
		/// <param name="args">
		/// コマンドライン引数
		/// 第一引数：ApiId（連携処理識別のためのID） or -FreeExport or -LetroExport
		/// 第二引数：処理対象ファイルパス or FreeExport コマンドキー or LetroExport コマンドキー
		/// 第三引数：ワーク用ファイルパス
		/// 第四引数：0…インポート処理、1…エクスポート処理
		/// 第五引数：処理対象ファイル形式（csv）
		/// </param>
		/// <remarks>
		/// exeが起動するとこのメソッドがコールされる
		/// </remarks>
		static void Main(string[] args)
		{
			// 初期化
			try
			{
				Console.WriteLine("設定ファイルの読み込み開始");
				AppInit.Init();
				Console.WriteLine("設定ファイルの読み込み完了");
			}
			catch (Exception ex)
			{
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name + string.Format("コマンドライン引数:[{0}]", string.Join(", ", args));
				ApiLogger.Write(LogLevel.fatal, "アプリケーション初期化処理にて致命的な失敗が発生しました。", "", ex);

				throw new w2Exception("アプリケーション初期化処理にて致命的な失敗が発生しました。");
			}
			ApiLogger.Write(LogLevel.info, "アプリケーション初期化完了", "");

			if (args.Any(a => (a == FreeExport.FreeExportProcess.COMMAND_LINE_KEY)))
			{
				FreeExportProcess(args);
			}
			else if (w2.App.Common.Constants.LETRO_OPTION_ENABLED
				&& args.Any(item => item == LetroExport.LetroExportProcess.COMMAND_LINE_KEY))
			{
				LetroExportProcess(args);
			}
			else
			{
				ExternalApiProcess(args);
			}
		}
		#endregion

		#region +FreeExportProcess FreeExportプロセス実行
		/// <summary>
		/// FreeExportプロセス実行
		/// </summary>
		/// <param name="args">コマンドライン引数</param>
		private static void FreeExportProcess(string[] args)
		{
			var process = new FreeExportProcess();
			var success = true;
			var message = "";
			try
			{
				var commandKey = "";
				if (args.Length == 2)
				{
					commandKey = args[1];
					Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name + commandKey;
				}
				else
				{
					message = "コマンドライン引数が不足しています。";
					Console.WriteLine(message);
					AppLogger.WriteInfo(message);
					throw new Exception(message);
				}

				FreeExportSettingManager.GetInstance().ReadSettingFile();
				var setting = FreeExportSettingManager.GetInstance()
					.FreeExportSetting.ExportSettings
					.FirstOrDefault(e => e.CommandKey == commandKey);

				if (setting == null)
				{
					message = string.Format("設定ファイル内にCommandKey[{0}]が存在しません。", commandKey);
					Console.WriteLine(message);
					AppLogger.WriteInfo(message);
					throw new Exception(message);
				}

				process.Setting(setting);

				message = "FreeExport 起動:" + commandKey;
				Console.WriteLine(message);
				AppLogger.WriteInfo(message);

				// 二重起動禁止
				if (ProcessUtility.ExecWithMutex(
						ProcessUtility.GenerateMutexName(
							Assembly.GetEntryAssembly().Location + FreeExport.FreeExportProcess.PROCESS_NAME + commandKey, "Process", true, true), process.Exec) == false)
				{
					message = string.Format("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
					Console.WriteLine(message);
					AppLogger.WriteInfo(message);
					throw new Exception(message);
				}

				message = "FreeExport 終了:" + commandKey;
				Console.WriteLine(message);
				AppLogger.WriteInfo(message);
			}
			catch (Exception ex)
			{
				Console.WriteLine("例外が発生しました。ログファイルを確認してください");
				AppLogger.WriteError("例外が発生しました。" + string.Format("コマンドライン引数:[{0}]", string.Join(", ", args)), ex);
				success = false;
			}
			finally
			{
				message = "結果報告完了メール送信開始";
				Console.WriteLine(message);
				AppLogger.WriteInfo(message);
				FreeExportSendMail.SendMail(process, success);
				message = "結果報告完了メール送信完了";
				Console.WriteLine(message);
				AppLogger.WriteInfo(message);
			}
		}
		#endregion

		#region +LetroExportProcess LetroExportプロセス実行
		/// <summary>
		/// LetroExportプロセス実行
		/// </summary>
		/// <param name="args">コマンドライン引数</param>
		private static void LetroExportProcess(string[] args)
		{
			var process = new LetroExportProcess();
			var success = true;
			var message = "";
			try
			{
				string commandKey;
				if (args.Length == 2)
				{
					commandKey = args[1];
					Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name + commandKey;
				}
				else
				{
					message = "コマンドライン引数が不足しています。";
					LetroExportLog(message);
					throw new Exception(message);
				}

				LetroExportSettingManager.GetInstance().ReadSettingFile();
				var setting = LetroExportSettingManager.GetInstance()
					.LetroExportSetting.ExportSettings
					.FirstOrDefault(item => item.CommandKey == commandKey);

				if (setting == null)
				{
					message = string.Format("設定ファイル内にCommandKey[{0}]が存在しません。", commandKey);
					LetroExportLog(message);
					throw new Exception(message);
				}

				process.Setting(setting);

				message = string.Format("LetroExport 起動: {0}", commandKey);
				LetroExportLog(message);

				if (ProcessUtility.ExecWithMutex(
					ProcessUtility.GenerateMutexName(
						Assembly.GetEntryAssembly().Location + LetroExport.LetroExportProcess.PROCESS_NAME + commandKey,
						"Process",
						true,
						true),
					process.Exec) == false)
				{
					message = "他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。";
					LetroExportLog(message);
					throw new Exception(message);
				}

				message = string.Format("LetroExport 終了: {0}", commandKey);
				LetroExportLog(message);
			}
			catch (Exception ex)
			{
				message = string.Format("例外が発生しました。コマンドライン引数:[{0}]", string.Join(", ", args));
				LetroExportLog(message);
				success = false;
			}
			finally
			{
				message = "結果報告完了メール送信開始";
				LetroExportLog(message);
				new LetroExportSendMail().SendMail(process, success);
				message = "結果報告完了メール送信完了";
				LetroExportLog(message);
			}
		}

		/// <summary>
		/// LetroExportログの出力
		/// </summary>
		/// <param name="message">ログ内容</param>
		private static void LetroExportLog(string message)
		{
			Console.WriteLine(message);
			FileLogger.WriteInfo(message);
		}
		#endregion

		#region +ExternalApiProcess メイン処理（プラグイン利用）
		/// <summary>
		///	メイン処理（プラグイン利用）
		/// </summary>
		/// <param name="args">
		/// コマンドライン引数
		/// 第一引数：ApiId（連携処理識別のためのID）
		/// 第二引数：処理対象ファイルパス
		/// 第三引数：ワーク用ファイルパス
		/// 第四引数：0…インポート処理、1…エクスポート処理
		/// 第五引数：処理対象ファイル形式（csv）
		/// </param>
		private static void ExternalApiProcess(string[] args)
		{
			Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

			ExecuteTarget target = null;
			LogLevel logLevel = LogLevel.fatal;
			string errorMessage = "致命的エラー発生";
			try
			{
				// コマンドライン引数から実行ターゲット取得
				try
				{
					target = CommandlineArguments.GetTargetFromArgs(args);
				}
				catch (Exception ex)
				{
					Console.WriteLine("コマンドライン指定が無効です。");
					Console.WriteLine(CommandlineArguments.GetExplanation());
					throw ex;
				}

				// ログ初期化
				ApiLogger.SetLogTarget(target);
				ApiLogger.Write(LogLevel.info, string.Format("コマンドライン引数取得完了:[{0}]", string.Join(", ", args)), "");

				// 二重起動禁止
				bool isSuccess = ProcessUtility.ExecWithMutex(ProcessUtility.GenerateMutexName(Assembly.GetEntryAssembly().Location + target.APIID, "Process", true, true), () =>
				{
					if (target.UseFtpDownload)
					{
						//結果報告メールのためにFTPダウンロード時のリスト(target.TargetFilePath)を設定して返します。
						target = UseFtpDownloadExecute(target);
					}
					else
					{
						Execute(target);
					}
				});
				if (isSuccess == false)
				{
					logLevel = LogLevel.warn;
					errorMessage = "アプリケーションの起動に失敗しました。";
					throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
				}
			}
			catch (Exception ex)
			{
				try
				{
					ApiLogger.Write(logLevel, errorMessage, "", ex);
				}
				catch (Exception ex2)
				{
					Console.WriteLine(ex2);
				}
			}
			// 結果報告メール送信
			finally
			{
				SendMail sendMail = new SendMail();
				sendMail.Send(target, DateTime.Now);
				ApiLogger.Write(LogLevel.info, "結果報告完了メール送信完了。バッチ終了します。", "");
			}
		}
		#endregion

		#region +Execute 実行処理
		/// <summary>
		/// 実行処理
		/// </summary>
		/// <param name="target">実行内容</param>
		private static void Execute(ExecuteTarget target)
		{
			ApiLogger.Write(LogLevel.info, "処理開始", "");
			var apiExecutor = new APIExecutor();
			apiExecutor.Excute(target);
			ApiLogger.Write(LogLevel.info, "処理完了", "");
		}
		#endregion

		#region +UseFtpDownloadExecute インポート対象ファイル一覧を取得し処理実行
		/// <summary>
		/// インポート対象ファイル一覧を取得し処理実行
		/// </summary>
		/// <param name="target">実行内容</param>
		/// <returns>実行結果</returns>
		private static ExecuteTarget UseFtpDownloadExecute(ExecuteTarget target)
		{
			var fluentFtpUtill = new FluentFtpUtility(
				target.Properties["ftphost"],
				target.Properties["ftpuser"],
				target.Properties["ftppassword"],
				Convert.ToBoolean(target.Properties["useactive"]),
				Convert.ToBoolean(target.Properties["enablessl"]));

			// ダウンロード対象のファイル一覧を全取得
			var downloadFilelist = fluentFtpUtill.FileNameListDownload(target.Properties["downloadsorce"]);

			if (downloadFilelist == null)
			{
				throw new Exception("FTP通信時 ディレクトリ上のファイル一覧取得に失敗しました。");
			}

			// ダウンロード対象にファイル名パターンが設定されていて、一致しない場合は除外
			if (string.IsNullOrEmpty(target.Properties["fileregex"]) == false)
			{
				downloadFilelist.RemoveAll(f => (new Regex(target.Properties["fileregex"], RegexOptions.Compiled).IsMatch(f) == false));
			}

			foreach (string file in downloadFilelist)
			{
				var ftpTarget = new ExecuteTarget(
					target.APIID,
					target.Properties["downloadlocation"] + file,
					target.ApiType,
					target.FileType,
					null,
					null,
					null,
					null,
					target.Properties.ToString(),
					target.WriteLog,
					target.UseFtpDownload);
				Execute(ftpTarget);
			}

			// 結果報告ためTargetFilePathにFTPダウンロードしたファイルのリストを設定
			var resultTarget = new ExecuteTarget(
					target.APIID,
					target.Properties["downloadlocation"] + string.Join(",", downloadFilelist),
					target.ApiType,
					target.FileType,
					null,
					null,
					null,
					null,
					target.Properties.ToString(),
					target.WriteLog,
					target.UseFtpDownload);
			return resultTarget;
		}
		#endregion
	}
}
