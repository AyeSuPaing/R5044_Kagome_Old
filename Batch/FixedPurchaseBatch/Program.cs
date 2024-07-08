/*
=========================================================================================================
  Module      : 定期購入バッチ処理(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Text;
using w2.App.Common;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.Order.FixedPurchase;
using w2.App.Common.Order.Register;
using w2.App.Common.Util;
using w2.Common.Logger;

namespace w2.Commerce.Batch.FixedPurchaseBatch
{
	class Program
	{
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			try
			{
				// 実体作成
				var program = new Program();

				// バッチ起動をイベントログ出力
				AppLogger.WriteInfo("起動");

				var success = ProcessUtility.ExcecWithProcessMutex(
					() =>
					{
						try
						{
							if (App.Common.Constants.SEND_FIXED_PURCHASE_DEADLINE_NOTIFICATION_MAIL_TO_USER)
							{
								// 定期購入変更期限案内メール送信
								program.ExecSendChangeDeadlineMail();
							}


							// 定期購入実行
							program.ExecFixedPurchase();
						}
						catch (Exception ex)
						{
							// 管理者向け：定期購入報告メール送信
							var body = new StringBuilder();
							body.Append("\n");
							body.Append("\n");
							body.Append("定期購入注文登録でエラーが発生しました。お手数ですが、管理者へご連絡下さい。");

							var mailSender = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, Constants.CONST_MAIL_ID_FIXEDPURCHASE_FOR_OPERATOR, "", new Hashtable(), true, Constants.MailSendMethod.Auto);
							mailSender.SetBody(body.ToString());
							mailSender.SetSubject(string.Format("{0}[{1}]", mailSender.Subject, DateTime.Now.ToString("MM/dd")));
							if (mailSender.SendMail() == false)
							{
								FileLogger.WriteError(body.ToString(), mailSender.MailSendException);
							}

							AppLogger.WriteError("定期購入注文登録でエラーが発生しました。", ex);
						}

						if (Constants.FIXED_PURCHASE_FORECAST_REPORT_OPTION)
						{
							try
							{
								AppLogger.WriteInfo("定期売上予測集計開始");
								new FixedPurchaseForecastReport.FixedPurchaseForecastReport().Aggregate();
								AppLogger.WriteInfo("定期売上予測集計正常終了");
							}
							catch (Exception ex)
							{
								AppLogger.WriteError("定期売上予測集計でエラーが発生しました", ex);
								AppLogger.WriteInfo("定期売上予測集計異常終了");
							}
						}
					});
				if (success == false) throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");

				// バッチ終了をイベントログ出力
				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
			}
			finally
			{
				// 最終実行日設定
				LastExecDate.UpdateLastExecDate();
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Program()
		{
			// 初期化
			Iniitialize();
		}

		/// <summary>
		/// 設定初期化
		/// </summary>
		private void Iniitialize()
		{
			try
			{
				// アプリケーション名設定
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

				// アプリケーション共通の設定
				w2.App.Common.ConfigurationSetting csSetting
					= new w2.App.Common.ConfigurationSetting(
						Properties.Settings.Default.ConfigFileDirPath,
						w2.App.Common.ConfigurationSetting.ReadKbn.C000_AppCommon,
						w2.App.Common.ConfigurationSetting.ReadKbn.C100_BatchCommon,
						w2.App.Common.ConfigurationSetting.ReadKbn.C200_FixedPurchaseBatch);

				// ユーザ向けメール送信可否
				Constants.SEND_USER_MAIL_FLG = csSetting.GetAppBoolSetting("SendUserMailFlg");

				// 領収書OP用 プロトコル取得
				Constants.PROTOCOL_HTTP = csSetting.GetAppStringSetting("Site_ProtocolHttp");
				Constants.PROTOCOL_HTTPS = csSetting.GetAppStringSetting("Site_ProtocolHttps");
				Constants.SITE_DOMAIN = csSetting.GetAppStringSetting("Site_Domain");
				Constants.PATH_ROOT_FRONT_PC = csSetting.GetAppStringSetting("Site_RootPath_PCFront");
				Constants.URL_FRONT_PC_SECURE = Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC;

				// 定期購入メールの送信タイミング管理にNG期間を設定
				Constants.FIXED_PURCHASE_MAIL_SEND_TIMING = csSetting.GetAppStringSetting("SendNgTimeZone");

				// 定期便変更案内メール送信日（配送キャンセル期限の何日前に送信するか）を設定する。
				Constants.NEXT_FIXED_PURCHASE_CHANGE_DEADLINE_SEND_DATE
					= csSetting.GetAppIntSetting("FixedPurchaseOption_NextFixedPurchaseChangeDeadlineSendDate");
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}

		#region 定期購入処理
		/// <summary>
		/// 定期購入処理
		/// </summary>
		private void ExecFixedPurchase()
		{
			// 定期購入注文登録
			var orderRegisterFixedPurchase = new OrderRegisterFixedPurchase(
				Constants.FLG_LASTCHANGED_BATCH,
				Constants.SEND_USER_MAIL_FLG,
				true,
				new FixedPurchaseMailSendTiming(Constants.FIXED_PURCHASE_MAIL_SEND_TIMING));
			CrossPointUtility.SetFixedPurchaseOrderForCrossPoint();
			orderRegisterFixedPurchase.ExecByBatch();
		}
		#endregion

		#region 定期購入変更期限案内メール送信処理
		/// <summary>
		/// 定期購入変更期限案内メール送信処理
		/// </summary>
		private void ExecSendChangeDeadlineMail()
		{
			new ChangeDeadlineMailSendCommand(
				new FixedPurchaseMailSendTiming(Constants.FIXED_PURCHASE_MAIL_SEND_TIMING)).Exec();
		}
		#endregion
	}
}
