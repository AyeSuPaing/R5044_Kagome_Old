/*
=========================================================================================================
  Module      : メインプログラム(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common;
using w2.Common.Logger;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.OrderCancelBatch
{
	/// <summary>
	/// メインプログラム
	/// </summary>
	class Program
	{
		/// <summary>
		/// プログラムのエントリポイント
		/// </summary>
		static void Main()
		{
			try
			{
				Program program = new Program();

				// バッチ起動をイベントログ出力
				AppLogger.WriteInfo("起動");

				program.Exec();

				// バッチ終了をイベントログ出力
				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		Program()
		{
			Initialize();
		}

		/// <summary>
		/// 設定初期化
		/// </summary>
		private void Initialize()
		{
			// アプリケーション名設定
			Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

			// アプリケーション共通の設定			
			ConfigurationSetting csSetting = new ConfigurationSetting(
				Properties.Settings.Default.ConfigFileDirPath,
				ConfigurationSetting.ReadKbn.C000_AppCommon,
				ConfigurationSetting.ReadKbn.C100_BatchCommon,
				ConfigurationSetting.ReadKbn.C200_OrderCancelBatch);

			Constants.ORDER_CANCEL_INTERVAL_MINUTES = csSetting.GetAppIntSetting("OrderCalcelIntervalMinutes");
			Constants.ORDER_CANCEL_DISALLOW_PAYMENT_KBNS = csSetting.GetAppStringSetting("OrderCancelDisallowPaymentKbns").Split(',');
		}

		/// <summary>
		/// 実行
		/// </summary>
		private void Exec()
		{
			var command = new OrderCancelCommand(Constants.ORDER_CANCEL_INTERVAL_MINUTES);
			int result = command.Exec(UpdateHistoryAction.Insert);
		}
	}
}
