/*
=========================================================================================================
  Module      : メインプログラム(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Logger;
using w2.App.Common;

namespace w2.Commerce.Batch.AffiliateReporter
{
	class Program
	{
		/// <summary>アフィリエイト区分</summary>
		public enum AffiliateKbn { LinkShare }

		/// <summary>
		/// プリケーションのメイン エントリ ポイント
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			try
			{
				// 実体作成
				Program pProgram = new Program();

				//------------------------------------------------------
				// バッチ起動をイベントログ出力
				//------------------------------------------------------
				AppLogger.WriteInfo("起動");

				//------------------------------------------------------
				// 本体実行
				//------------------------------------------------------
				if (args.Length == 0)
				{
					pProgram.ReportAll();
				}
				else
				{
					//objImport.Report(args[0]);
				}

				//------------------------------------------------------
				// バッチ終了をイベントログ出力
				//------------------------------------------------------
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
		public Program()
		{
			try
			{
				//------------------------------------------------------
				// アプリケーション設定読み込み
				//------------------------------------------------------
				// アプリケーション名設定
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

				// アプリケーション共通の設定			
				ConfigurationSetting csSetting = new ConfigurationSetting(
					Properties.Settings.Default.ConfigFileDirPath,
					ConfigurationSetting.ReadKbn.C000_AppCommon,
					ConfigurationSetting.ReadKbn.C100_BatchCommon,
					ConfigurationSetting.ReadKbn.C200_AffiliateReporter);

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				// リンクシェアファイルディレクトリパス
				Constants.PHYSICALDIRPATH_TEMPDIR = AppDomain.CurrentDomain.BaseDirectory + @"Temp\";

				// リンクシェア成果報告EXEファイルパス
				Constants.PHYSICALDIRPATH_LINKSHARE_TRANSFER_EXE = csSetting.GetAppStringSetting("Directory_LinkShareTransferExePath");
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}

		/// <summary>
		/// 全て報告
		/// </summary>
		public void ReportAll()
		{
			foreach (Enum eEnum in Enum.GetValues(typeof(AffiliateKbn)))
			{
				Report((AffiliateKbn)eEnum);
			}
		}

		/// <summary>
		/// 報告
		/// </summary>
		/// <param name="akAffiliateKbn">アフィリエイト区分</param>
		public void Report(AffiliateKbn akAffiliateKbn)
		{
			switch (akAffiliateKbn)
			{
					// リンクシェアアフィリエイト？
				case AffiliateKbn.LinkShare:
					LinkShareReporter lsrLinkShareReporter = new LinkShareReporter();
					lsrLinkShareReporter.Report();
					break;
			}
		}
	}
}
