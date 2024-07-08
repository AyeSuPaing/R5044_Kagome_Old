/*
=========================================================================================================
  Module      : プログラム本体(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using w2.Common.Logger;
using w2.App.Common;
using w2.App.Common.Util;
using w2.Commerce.Batch.ExternalOrderImport.Commands;

namespace w2.Commerce.Batch.ExternalOrderImport
{
	/// <summary>
	/// プログラム本体
	/// </summary>
	public class Program
	{
		/// <summary>実行するコマンド</summary>
		private readonly List<ICommand> m_commands = new List<ICommand>();

		#region メイン処理
		/// <summary>
		/// メイン処理
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			try
			{
				EventLogger.WriteInfo("起動");

				var program = new Program();
				if (ProcessUtility.ExcecWithProcessMutex(program.Start) == false)
				{
					throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
				}
				EventLogger.WriteInfo("終了");
			}
			catch (Exception ex)
			{
				ConsoleLogger.WriteError(ex);
			}
		}
		#endregion

		#region +Program コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Program()
		{
			Initialize();
		}
		#endregion

		#region -Initialize 初期化
		/// <summary>
		/// 初期化処理
		/// </summary>
		private void Initialize()
		{
			try
			{
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

				var appSetting = new ConfigurationSetting(
					Properties.Settings.Default.ConfigFileDirPath,
					ConfigurationSetting.ReadKbn.C000_AppCommon,
					ConfigurationSetting.ReadKbn.C100_BatchCommon,
					ConfigurationSetting.ReadKbn.C200_ExternalOrderImport);

				// つくーるAPI連携設定読み込み
				Constants.URERU_AD_IMPORT_API_URL = appSetting.GetAppStringSetting("UreruAdImport_ApiUrl");
				Constants.URERU_AD_IMPORT_ACCOUNT = appSetting.GetAppStringSetting("UreruAdImport_Account");
				Constants.URERU_AD_IMPORT_PASS = appSetting.GetAppStringSetting("UreruAdImport_Pass");
				Constants.URERU_AD_IMPORT_KEY = appSetting.GetAppStringSetting("UreruAdImport_Key");
				Constants.URERU_AD_IMPORT_DEFAULT_USER_KBN = appSetting.GetAppStringSetting("UreruAdImport_DefaultUserKbn");

				// 領収書OP用 プロトコル取得
				Constants.PROTOCOL_HTTP = appSetting.GetAppStringSetting("Site_ProtocolHttp");
				Constants.PROTOCOL_HTTPS = appSetting.GetAppStringSetting("Site_ProtocolHttps");
				
				// 実行コマンドの登録
				m_commands.Add(new UreruAdImportCommand());
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}
		#endregion

		#region -Start 処理開始
		/// <summary>
		/// 処理開始
		/// </summary>
		private void Start()
		{
			foreach (var command in m_commands)
			{
				ConsoleLogger.WriteInfo(@"{0} 開始", command.ToString());
				try
				{
					command.OnStart();
					command.Exec();
					command.OnComplete();
					ConsoleLogger.WriteInfo(
						@"{0} 終了 {1}/{2}件(スキップ {3}件)",
						command.ToString(),
						command.ExecuteResult.SuccessCount,
						command.ExecuteResult.ExecuteCount,
						command.ExecuteResult.SkipCount);
				}
				catch (Exception ex)
				{
					command.ExecuteResult.ErrorLog.Add(ex.Message);
					command.OnError();
					ConsoleLogger.WriteError(ex);
				}
				SendMail(command.ExecuteResult);
			}
		}
		#endregion

		#region -SendMail 結果通知メール送信
		/// <summary>
		/// 結果通知メール送信
		/// </summary>
		/// <param name="commandResult">処理結果</param>
		private void SendMail(CommandResultBase commandResult)
		{
			var message = new StringBuilder();
			commandResult.ErrorLog.ForEach(result => message.AppendLine(result));

			var mailSender = new MailSendUtil(commandResult.MailTemplateId)
			{
				BeginTime = commandResult.BeginTime,
				EndTime = commandResult.EndTime,
				ExecuteCount = commandResult.ExecuteCount,
				SuccessCount = commandResult.SuccessCount,
				FailureCount = commandResult.FailureCount,
				SkipCount = commandResult.SkipCount,
				AppName = commandResult.AppName,
				Message = message.Replace("<br />", Environment.NewLine)
			};
			mailSender.SendMailForOperator();
		}
		#endregion
	}
}
