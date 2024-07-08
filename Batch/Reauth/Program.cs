/*
=========================================================================================================
  Module      : プログラム本体(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common;
using w2.App.Common.Util;
using w2.Commerce.Reauth.Commands;
using w2.Common.Logger;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Reauth
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
		/// <param name="args">実行対象日</param>
		static void Main(string[] args)
		{
			try
			{
				EventLogger.WriteInfo("起動");

				// 引数
				var argument = new Argument(args);

				var program = new Program(argument);

				ConsoleLogger.WriteInfo(@"引数:{0}", argument.ArgumentString);

				// 引数がおかしい
				if (string.IsNullOrEmpty(argument.ErrorMessage) == false)
				{
					ConsoleLogger.WriteInfo(argument.ErrorMessage);
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
				ConsoleLogger.WriteError(ex);
			}
		}
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ：初期化処理
		/// </summary>
		/// <param name="argument">引数</param>
		Program(Argument argument)
		{
			Initialize(argument);
		}
		#endregion

		#region -Initialize 初期化処理
		/// <summary>
		/// 初期化処理
		/// </summary>
		/// <param name="argument">引数</param>
		private void Initialize(Argument argument)
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
					ConfigurationSetting.ReadKbn.C200_Reauth);

				Constants.CREDIT_ZEUS_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_Credit_Zeus_AuthLimitDay");
				Constants.CREDIT_SBPS_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_Credit_Sbps_AuthLimitDay");
				Constants.CREDIT_GMO_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_Credit_Gmo_AuthLimitDay");
				Constants.CREDIT_YAMATOKWC_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_Credit_YamatoKwc_AuthLimitDay");
				Constants.CREDIT_VERITRANS_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_Credit_VeriTrans_AuthLimitDay");
				Constants.CVSDEF_YAMATOKA_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_CvsDef_YamatoKa_AuthLimitDay");
				Constants.CVSDEF_GMO_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_CvsDef_Gmo_AuthLimitDay");
				Constants.CVSDEF_ATODENE_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_CvsDef_Atodene_AuthLimitDay");
				Constants.CVSDEF_DSK_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_CvsDef_Dsk_AuthLimitDay");
				Constants.AMAZONPAY_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_AmazonPay_AuthLimitDay");
				Constants.CREDIT_ZCOM__AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_Zcom_AuthLimitDay");
				Constants.PAIDYPAY_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_Paidy_AuthLimitDay");
				Constants.LINEPAY_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_LinePay_AuthLimitDay");
				Constants.NPAFTERPAY_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_NPAfterPay_AuthLimitDay");
				Constants.NOT_TARGET_REAUTH_PAYMENT_IDS = appSetting.GetAppStringSettingList("Payment_Not_Target_Reauth_Payment_Ids");
				Constants.CREDIT_ESCOTT_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_EScott_AuthLimitDay");
				Constants.RAKUTEN_PAY_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_Credit_RakutenCredit_AuthLimitDay");
				Constants.CREDIT_PAYGENT_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_Credit_Paygent_AuthLimitDay");
				Constants.CVSDEF_SCORE_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_CvsDef_Score_AuthLimitDay");
				Constants.CVSDEF_VERITRANS_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_CvsDef_Veritrans_AuthLimitDay");
				Constants.GMOPOST_AUTH_EXPIRE_DAYS = appSetting.GetAppIntSetting("Payment_GmoPost_AuthLimitDay");

				// コマンド追加
				m_commands.Add(new ReauthAuthExpiredOrder(argument));
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
			foreach (var item in m_commands.Select((command, index) => new { Command = command, Index = index }))
			{
				ConsoleLogger.WriteInfo(@"{0} 開始", item.Command.ToString());
				var resultCount = item.Command.Exec(UpdateHistoryAction.Insert);
				ConsoleLogger.WriteInfo(@"{0} 終了 {1}/{2}件", item.Command.ToString(), resultCount.Item1, resultCount.Item2);
			}
		}
		#endregion
	}
}
