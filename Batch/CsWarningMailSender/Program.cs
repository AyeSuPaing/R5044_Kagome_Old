/*
=========================================================================================================
  Module      : プログラム本体(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common;
using w2.App.Common.Util;
using w2.Common.Logger;

namespace w2.CustomerSupport.Batch.CsWarningMailSender
{
	/// <summary>
	/// プログラム本体
	/// </summary>
	class Program
	{
		#region メイン処理
		/// <summary>
		/// メイン処理
		/// </summary>
		static void Main()
		{
			try
			{
				Program program = new Program();

				// バッチ起動をイベントログ出力
				AppLogger.WriteInfo("起動");

				// 二重起動禁止
				bool isSuccess = ProcessUtility.ExcecWithProcessMutex(() =>
				{
					program.Start();
				});
				if (isSuccess == false)
				{
					throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
				}

				// バッチ終了をイベントログ出力
				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
				Console.WriteLine(ex);
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
		private static void Initialize()
		{
			try
			{
				// アプリケーション設定読み込み
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

				// アプリケーション共通の設定
				var appSetting = new ConfigurationSetting(
					Properties.Settings.Default.ConfigFileDirPath,
					ConfigurationSetting.ReadKbn.C000_AppCommon,
					ConfigurationSetting.ReadKbn.C100_BatchCommon,
					ConfigurationSetting.ReadKbn.C200_CsWarningMailSender);

				// 警告メール送信バッチ設定
				Constants.MAIL_SEND_TIME = appSetting.GetAppIntSetting("MailSendTime");
				Constants.WARNING_NO_ACTION = appSetting.GetAppBoolSetting("WarningNoAction");
				Constants.WARNING_NO_ACTION_LIMIT_DAYS = appSetting.GetAppIntSetting("WarningNoActionLimitDays");
				Constants.WARNING_NO_ASSIGN = appSetting.GetAppBoolSetting("WarningNoAssign");
				Constants.WARNING_NO_ASSIGN_LIMIT_DAYS = appSetting.GetAppIntSetting("WarningNoAssignLimitDays");
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
			var sender = new WarningMailSender();
			sender.Send();
		}
		#endregion
	}
}
