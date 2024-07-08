/*
=========================================================================================================
  Module      : プログラム本体(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Logger;
using w2.App.Common;
using w2.App.Common.Util;
using w2.Commerce.Batch.UpdateHistoryTransfer.Commands;

namespace w2.Commerce.Batch.UpdateHistoryTransfer
{
	/// <summary>
	/// プログラム本体
	/// </summary>
	class Program
	{
		#region +メイン処理
		/// <summary>
		/// メイン処理
		/// </summary>
		static void Main(string[] args)
		{
			try
			{
				EventLogger.WriteInfo("起動");

				var program = new Program();

				// 実行しない設定であれば実行しない
				if (Constants.UPDATEHISTORY_RETENTION_PERIOD_MONTHS.HasValue == false)
				{
					Console.WriteLine("バッチは実行しない設定になっています。\r\n実行をキャンセルしました。\r\n");
					return;
				}

				// 二重起動禁止
				bool isSuccess = ProcessUtility.ExcecWithProcessMutex(program.Start);
				if (isSuccess == false)
				{
					throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
				}

				EventLogger.WriteInfo("終了");
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
			}
		}
		#endregion

		#region +コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Program()
		{
			// 初期化処理
			Initialize();
		}
		#endregion

		#region -初期化処理
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
					ConfigurationSetting.ReadKbn.C200_UpdateHistoryTransfer);

				// アプリケーション固有の設定
				Constants.MAIL_SUBJECTHEAD = appSetting.GetAppStringSetting("Mail_SubjectHead");
				Constants.MAIL_FROM = appSetting.GetAppMailAddressSetting("Mail_From");
				Constants.MAIL_TO_LIST = appSetting.GetAppMailAddressSettingList("Mail_To");
				Constants.MAIL_CC_LIST = appSetting.GetAppMailAddressSettingList("Mail_Cc");
				Constants.MAIL_BCC_LIST = appSetting.GetAppMailAddressSettingList("Mail_Bcc");
				Constants.UPDATEHISTORY_RETENTION_PERIOD_MONTHS =
					(int?)appSetting.GetAppSetting("UpdateHistory_Retention_Period_Months", typeof(int?));
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}
		#endregion

		#region -プログラム開始
		/// <summary>
		/// プログラム開始
		/// </summary>
		private void Start()
		{
			// 更新履歴データ転送
			new TransferUpdateHistoryCommand().Execute();
		}
		#endregion
	}
}