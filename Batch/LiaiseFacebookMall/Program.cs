/*
=========================================================================================================
  Module      : プログラム本体(Program.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Util;
using w2.Commerce.Batch.LiaiseFacebookMall.Commands;
using w2.Common.Logger;

namespace w2.Commerce.Batch.LiaiseFacebookMall
{
	/// <summary>
	/// プログラム本体
	/// </summary>
	public class Program
	{
		/// <summary>
		/// メイン処理
		/// </summary>
		/// <param name="args">引数</param>
		static void Main(string[] args)
		{
			try
			{
				var program = new Program();
				FileLogger.WriteInfo("起動");
	
				if (ProcessUtility.ExcecWithProcessMutex(program.Execute) == false)
				{
					throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
				}
				FileLogger.WriteInfo("終了");
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
			}
		}
	
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Program()
		{
			Initialize();
		}

		/// <summary>
		/// 設定初期化
		/// </summary>
		private void Initialize()
		{
			try
			{
				Constants.APPLICATION_NAME = Properties.Settings.Default.ApplicationName;
	
				new w2.App.Common.ConfigurationSetting(
					Properties.Settings.Default.ConfigFileDirPath,
					w2.App.Common.ConfigurationSetting.ReadKbn.C000_AppCommon,
					w2.App.Common.ConfigurationSetting.ReadKbn.C100_BatchCommon,
					w2.App.Common.ConfigurationSetting.ReadKbn.C200_CommonFront);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("設定ファイルの読み込みに失敗しました。\r\n" + ex);
			}
		}
	
		/// <summary>
		/// 処理実行
		/// </summary>
		private void Execute()
		{
			// Facebook連携処理
			var command = new LiaiseFacebookCommand();
			try
			{
				command.OnStart();
				command.Exec();
				command.OnComplete();
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				command.OnError();
			}
		}
	}
}