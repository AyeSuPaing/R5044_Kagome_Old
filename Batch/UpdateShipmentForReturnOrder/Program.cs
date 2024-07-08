/*
=========================================================================================================
  Module      : プログラム本体(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common;
using w2.App.Common.Util;
using w2.Commerce.Batch.UpdateShipmentForReturnOrder.Commands;
using w2.Common.Logger;

namespace w2.Commerce.Batch.UpdateShipmentForReturnOrder
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
				var program = new Program();

				AppLogger.WriteInfo("起動");

				// 22:00-8:00のメンテナンス時間帯は実行しない
				if (DateTime.Now.Hour >= 22) return;
				if (DateTime.Now.Hour < 8) return;

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
				var appSetting
					= new w2.App.Common.ConfigurationSetting(
						Properties.Settings.Default.ConfigFileDirPath,
						w2.App.Common.ConfigurationSetting.ReadKbn.C000_AppCommon,
						w2.App.Common.ConfigurationSetting.ReadKbn.C100_BatchCommon,
						w2.App.Common.ConfigurationSetting.ReadKbn.C200_UpdateShipmentForReturnOrder);

				// アプリケーション固有の設定
				Constants.MAIL_SUBJECTHEAD = appSetting.GetAppStringSetting("Mail_SubjectHead");
				Constants.MAIL_FROM = appSetting.GetAppMailAddressSetting("Mail_From");
				Constants.MAIL_TO_LIST = appSetting.GetAppMailAddressSettingList("Mail_To");
				Constants.MAIL_CC_LIST = appSetting.GetAppMailAddressSettingList("Mail_Cc");
				Constants.MAIL_BCC_LIST = appSetting.GetAppMailAddressSettingList("Mail_Bcc");
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
			if (Constants.PAYMENT_REAUTH_ENABLED)
			{
				// コンビニ後払い出荷報告依頼（返品注文用）
				new UpdateCvsDefShipmentCompleteForReturnOrderCommand().Execute();

				if (Constants.PAYMENT_SETTING_CREDIT_RETURN_AUTOSALES_ENABLED)
				{
					// ヤマトクレジットカード出荷報告完了更新（返品注文用）
					new UpdateYamatoKwcShipmentCompleteForReturnOrderCommand().Execute();
				}
			}
		}
		#endregion
	}
}