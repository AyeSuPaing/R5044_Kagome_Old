/*
=========================================================================================================
  Module      : プログラム本体(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using w2.App.Common.Util;
using w2.Commerce.Batch.UserAttributeCalculator.Commands;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.DispSummaryAnalysis;

namespace w2.Commerce.Batch.UserAttributeCalculator
{
	/// <summary>
	/// プログラム本体
	/// </summary>
	class Program
	{
		/// <summary>コマンド一覧</summary>
		private readonly List<ICommand> m_commands = new List<ICommand>();

		#region メイン処理
		/// <summary>
		/// メイン処理
		/// </summary>
		static void Main()
		{
			try
			{
				var program = new Program();

				AppLogger.WriteInfo("起動");

				// 二重起動禁止
				bool isSuccess = ProcessUtility.ExcecWithProcessMutex(program.Start);
				if (isSuccess == false)
				{
					throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
				}

				AppLogger.WriteInfo("終了");
			}
			catch (Exception ex)
			{
				ConsoleLogger.WriteError(ex);
			}
		}
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ：初期化処理
		/// </summary>
		Program()
		{
			Initialize();
		}
		#endregion

		#region -Initialize 初期化処理
		/// <summary>
		/// 初期化処理
		/// </summary>
		private void Initialize()
		{
			try
			{
				// アプリケーション設定読み込み
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

				// アプリケーション共通の設定
				var appSetting
					= new w2.App.Common.ConfigurationSetting(
						Properties.Settings.Default.ConfigFileDirPath,
						w2.App.Common.ConfigurationSetting.ReadKbn.C000_AppCommon,
						w2.App.Common.ConfigurationSetting.ReadKbn.C100_BatchCommon,
						w2.App.Common.ConfigurationSetting.ReadKbn.C200_UserAttributeCalculator);

				// コマンド追加
				m_commands.Add(new CalculateOrderInfoCommand());
				m_commands.Add(new CreateEmptyUserAttributeCommand());
				if (Constants.CPM_OPTION_ENABLED)
				{
					m_commands.Add(new CalculateCpmClusterCommand());
					m_commands.Add(new CreateCpmReportCommand());
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}
		#endregion

		#region -Start プログラム開始
		/// <summary>
		/// プログラム開始
		/// </summary>
		private void Start()
		{
			var targetStart = GetLastExec().AddDays(1);
			var targetEnd = DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd 23:59:59.998"));
			ConsoleLogger.WriteInfo("");	// ログを見やすくするため空行挿入
			ConsoleLogger.WriteInfo(@"対象日時(開始):{0}", targetStart.ToString("yyyy/MM/dd HH:mm:ss"));
			ConsoleLogger.WriteInfo(@"対象日時(終了):{0}", targetEnd.ToString("yyyy/MM/dd HH:mm:ss"));

			var calculateAllCommands = GetCalculateAllCommands();

			foreach (var item in m_commands.Select((command, index) => new { Command = command, Index = index }))
			{
				var calculateAll = calculateAllCommands.Contains(item.Command);
				var commandTargetStart = calculateAll ? DateTime.MinValue : targetStart;
				var commandTargetEnd = targetEnd;

				Console.WriteLine(@"{0}:{1} 開始{2}", item.Index, item.Command.ToString(), calculateAll ? "(全体)" : "");
				var resultCount = item.Command.Exec(commandTargetStart, commandTargetEnd);
				Console.WriteLine(@"{0}:{1} 終了{2}", item.Index, item.Command.ToString(), calculateAll ? "(全体)" : "");
			}

			// 正常終了したら進捗ファイル更新
			UpdateLastExecFile(targetEnd);
			// コマンドファイルも更新
			UpdateCalculateAllCommandsFile(targetEnd);
		}
		#endregion

		/// <summary>
		/// 最終実行日取得
		/// </summary>
		/// <returns></returns>
		private DateTime GetLastExec()
		{
			try
			{
				var lastExec = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, Constants.FILENAME_LASTEXEC_PREFIX + "*");
				if (lastExec.Length > 0)
				{
					var datetime = Path.GetFileName(lastExec[0]).Substring(Constants.FILENAME_LASTEXEC_PREFIX.Length);
					return DateTime.ParseExact(datetime, Constants.FILENAME_LASTEXEC_SUFFIX_DATEFORMAT, null);
				}
			}
			catch (Exception)
			{
			}
			return DateTime.MinValue;
		}

		/// <summary>
		/// 全計算コマンド取得
		/// </summary>
		/// <returns>全計算コマンド</returns>
		private List<ICommand> GetCalculateAllCommands()
		{
			var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.FILENAME_CALCULATE_ALL_COMMANDS);
			if (File.Exists(fileName) == false) return new List<ICommand>();

			var commands = File.ReadAllLines(fileName, Encoding.GetEncoding("Shift_JIS"))
				.Where(line => line.StartsWith("//") == false).ToArray();
			if (commands.Any(command => string.Equals(command, "All", StringComparison.OrdinalIgnoreCase))) return m_commands;

			return m_commands.Where(command =>
				commands.Any(calculateAllCommand =>
					string.Equals(calculateAllCommand, command.ToString(), StringComparison.OrdinalIgnoreCase))).ToList();
		}

		/// <summary>
		/// 進捗ファイル更新
		/// </summary>
		/// <param name="targetEnd">対象日時（終了）</param>
		private void UpdateLastExecFile(DateTime targetEnd)
		{
			Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, Constants.FILENAME_LASTEXEC_PREFIX + "*")
				.ToList().ForEach(File.Delete);
			File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
				Constants.FILENAME_LASTEXEC_PREFIX + targetEnd.ToString(Constants.FILENAME_LASTEXEC_SUFFIX_DATEFORMAT)));
		}

		/// <summary>
		/// コマンドファイル更新
		/// </summary>
		/// <param name="targetEnd">対象日時（終了）</param>
		private void UpdateCalculateAllCommandsFile(DateTime targetEnd)
		{
			var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.FILENAME_CALCULATE_ALL_COMMANDS);
			if (File.Exists(filePath))
			{
				File.Move(filePath, filePath + "." + targetEnd.ToString("yyyyMMddHHmmss") + ".txt");
			}
		}
	}
}
