/*
=========================================================================================================
  Module      : プログラム本体(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Logger;
using w2.App.Common.Util;
using w2.Commerce.Batch.LiaiseAmazonMall.Commands;

namespace w2.Commerce.Batch.LiaiseAmazonMall
{
	/// <summary>
	/// プログラム本体
	/// </summary>
	public class Program
	{
		#region メイン処理
		/// <summary>
		/// メイン処理
		/// </summary>
		/// <param name="args">引数</param>
		static void Main(string[] args)
		{
			try
			{
				var program = new Program();
				EventLogger.WriteInfo("起動");

				if (ProcessUtility.ExcecWithProcessMutex(program.Execute) == false)
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

		#region +Program コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Program()
		{
			Initialize();
		}
		#endregion

		#region -Initialize 設定初期化
		/// <summary>
		/// 設定初期化
		/// </summary>
		private void Initialize()
		{
			try
			{
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

				var appSetting = new w2.App.Common.ConfigurationSetting(
						Properties.Settings.Default.ConfigFileDirPath,
						w2.App.Common.ConfigurationSetting.ReadKbn.C000_AppCommon,
						w2.App.Common.ConfigurationSetting.ReadKbn.C100_BatchCommon,
						w2.App.Common.ConfigurationSetting.ReadKbn.C200_LiaiseAmazonMall
					);

				// Amazon連携設定読み込み
				Constants.SELLERSKU_FOR_NO_VARIATION = appSetting.GetAppStringSetting("SellerSKU_Link_Column_For_NoVariation");
				Constants.SELLERSKU_FOR_USE_VARIATION = appSetting.GetAppStringSetting("SellerSKU_Link_Column_For_UseVariation");
				Constants.LIAISE_AMAZON_MALL_DEFAULT_USER_KBN = appSetting.GetAppStringSetting("LiaiseAmazonMall_DefaultUserKbn");
				Constants.LIAISE_AMAZON_MALL_DEFAULT_SHIPPING_ID = appSetting.GetAppStringSetting("LiaiseAmazonMall_DefaultShippingId");
				w2.App.Common.Constants.AMAZON_MALL_MWS_ENDPOINT = appSetting.GetAppStringSetting("LiaiseAmazonMall_amazon_mws_endpoint");
				AmazonMarketPlaceWebService.Constants.GET_AMAZON_ORDERS_UPDATE_BEFORE_DAYS = appSetting.GetAppIntSetting("Get_Amazon_Orders_Update_Before_Days");
			}
			catch (Exception ex)
			{
				throw new ApplicationException("設定ファイルの読み込みに失敗しました。\r\n" + ex);
			}
		}
		#endregion

		#region -Start 処理実行
		/// <summary>
		/// 処理実行
		/// </summary>
		private void Execute()
		{
			// Amazon連携処理
			var command = new LiaiseAmazonCommand();
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
