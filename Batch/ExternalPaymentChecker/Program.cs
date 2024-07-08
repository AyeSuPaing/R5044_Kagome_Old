/*
=========================================================================================================
  Module      : プログラム本体(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Net;
using w2.App.Common;
using w2.App.Common.Util;
using w2.Commerce.Batch.ExternalPaymentChecker.Commands;
using w2.Common.Logger;

namespace w2.Commerce.Batch.ExternalPaymentChecker
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
				AppLogger.WriteError(ex);
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
					ConfigurationSetting.ReadKbn.C300_ExternalPaymentChecker);

				// 検索日数
				Constants.SEARCH_DAYS = appSetting.GetAppIntSetting("ExternalPaymentChecker_SearchDays");

				// WebRequestのSSLプロトコルバージョン設定
				ServicePointManager.SecurityProtocol
					 = SecurityProtocolType.Tls12
					 | SecurityProtocolType.Tls11
					 | SecurityProtocolType.Tls
					 | SecurityProtocolType.Ssl3;
			}
			catch (Exception ex)
			{
				throw new Exception("設定ファイルの読み込みに失敗しました。\r\n", ex);
			}
		}
		#endregion

		#region -Start プログラム開始
		/// <summary>
		/// プログラム開始
		/// </summary>
		private void Start()
		{
			if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.YamatoKwc)
			{
				new PaymentCheckerYamatoKwc(Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT).Exec();
			}
			if (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.YamatoKwc)
			{
				new PaymentCheckerYamatoKwc(Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE).Exec();
			}
			if (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Paygent)
			{
				new PaymentCheckerPaygent(
						Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE,
						Constants.PAYMENT_PAYGENT_CVS_PAYMENT_LIMIT_DAY)
					.Exec();
			}
			if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.YamatoKa)
			{
				new PaymentCheckerYamatoKa(Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF).Exec();
			}
			if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Dsk)
			{
				new PaymentGetAuthResultDskDeferred().Exec();
			}
			if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atobaraicom)
			{
				new PaymentGetAuthorizeStatusAtobaraicom().Exec();
			}
			if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Score)
			{
				new PaymentGetAuthResultScoreDeferred().Exec();
			}
			if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Veritrans)
			{
				new PaymentGetAuthResultVeritransDeferred().Exec();
			}
			if (Constants.PAYMENT_GMOATOKARA_ENABLED)
			{
				new PaymentGetAuthResultGmoAtokara().Exec();
			}
			if (Constants.PAYMENT_NETBANKING_OPTION_ENABLED
				&& (Constants.GLOBAL_OPTION_ENABLE == false)
				&& (Constants.PAYMENT_NETBANKING_KBN == Constants.PaymentBanknetKbn.Paygent))
			{
				new PaymentCheckerPaygent(
						Constants.FLG_PAYMENT_PAYMENT_ID_BANKNET,
						Constants.SEARCH_DAYS)
					.Exec();
			}
			if (Constants.PAYMENT_ATMOPTION_ENABLED
				&& (Constants.GLOBAL_OPTION_ENABLE == false)
				&& (Constants.PAYMENT_ATM_KBN == Constants.PaymentATMKbn.Paygent))
			{
				new PaymentCheckerPaygent(
						Constants.FLG_PAYMENT_PAYMENT_ID_ATM,
						Constants.SEARCH_DAYS)
					.Exec();
			}
		}
		#endregion
	}
}
