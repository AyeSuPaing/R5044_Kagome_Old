/*
=========================================================================================================
  Module      : プログラム本体(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common;
using w2.App.Common.Util;
using w2.Commerce.MallBatch.LiaiseLohacoMall.Properties;
using w2.Common.Logger;

namespace w2.Commerce.MallBatch.LiaiseLohacoMall
{
	/// <summary>
	/// プログラム本体
	/// </summary>
	public class Program
	{
		#region +メイン処理
		/// <summary>
		/// メイン処理
		/// </summary>
		public static void Main(string[] args)
		{
			try
			{
				// 本プログラムインスタンス作成
				var program = new Program();

				FileLogger.WriteInfo("起動");

				// 二重起動禁止
				bool isSuccess = ProcessUtility.ExcecWithProcessMutex(program.Start);
				if (isSuccess == false)
				{
					throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
				}

				FileLogger.WriteInfo("終了");
			}
			catch (Exception ex)
			{
				var errorMessage = ex.ToString();
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
				Constants.APPLICATION_NAME = Settings.Default.Application_Name;

				// アプリケーション共通の設定
				var appSetting
					= new ConfigurationSetting(
						Settings.Default.ConfigFileDirPath,
						ConfigurationSetting.ReadKbn.C000_AppCommon,
						ConfigurationSetting.ReadKbn.C100_BatchCommon,
						ConfigurationSetting.ReadKbn.C200_LiaiseLohacoMall);
				// Lohaco連携設定読み込み
				Constants.LOHACO_GET_ORDER_COUNT = appSetting.GetAppIntSetting("LiaiseLohacoMall_GetOrderCount");
				Constants.LOHACO_DEFAULT_SHIPPING_ID = appSetting.GetAppStringSetting("LiaiseLohacoMall_DefaultShippingId");
				Constants.WRITE_DEBUG_LOG_ENABLED = appSetting.GetAppBoolSetting("LiaiseLohacoMall_WriteDebugLogEnabled");
				Constants.MASK_PERSONAL_INFO_ENABLED = appSetting.GetAppBoolSetting("LiaiseLohacoMall_MaskPersonalInfoEnabled");
				Constants.ORDER_EXTEND_STATUS_NO_LOHACO_RESERVE_ORDER = appSetting.GetAppIntSetting("LiaiseLohacoMall_ReserveOrderExtendStatusNo");
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
			// Lohaco連携処理
			var command = new LiaiseLohacoCommand();
			try
			{
				command.OnStart();
				command.Exec();
				command.OnComplete();
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex.ToString());
				command.OnError();
			}
		}
		#endregion
	}
}