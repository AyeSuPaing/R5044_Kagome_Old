/*
=========================================================================================================
  Module      : Program (Program.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using w2.App.Common;
using w2.Common.Logger;

namespace w2.Commerce.Batch.FlapsIntegration
{
	/// <summary>
	/// Program
	/// </summary>
	public class Program
	{
		/// <summary>
		/// 
		/// </summary>
		private static void Main(string[] args)
		{
#if DEBUG
			if (args.Any() == false)
			{
				args = new string[1];
				args[0] = Constants.FLG_FLAPS_REPLICATION_DATA_PRODUCT;
			}
#endif
			try
			{
				// 初期設定
				if (args.Length != 1)
				{
					var msg = "引数の数が適切ではありません。";
					FileLogger.WriteError(msg);
					Console.WriteLine(msg);
					return;
				}

				Initializer.ReadCommonConfig();
				
				var startMsg = "起動";
				Console.WriteLine(startMsg);
				FileLogger.WriteInfo(startMsg);

				if (Constants.FLAPS_OPTION_ENABLE == false)
				{
					var msg = "FLAPS連携オプション(Flaps_Option_Enable)をオンにして下さい。";
					FileLogger.WriteError(msg);
					Console.WriteLine(msg);
					return;
				}

				// 処理実行 (多重起動を許可)
				Execute(args[0]);

				var completedMsg = "正常終了";
				Console.WriteLine(completedMsg);
				FileLogger.WriteInfo(completedMsg);
			}
			catch (Exception ex)
			{
				NotifyException(args[0]);
				FileLogger.WriteError("異常終了", ex);
			}
		}

		/// <summary>
		/// FLAPS同期処理実行
		/// </summary>
		/// <param name="replicatedData">同期対象のデータ</param>
		private static void Execute(string replicatedData)
		{
			var action = new FlapsIntegrationAction();
			switch (replicatedData)
			{
				case Constants.FLG_FLAPS_REPLICATION_DATA_PRODUCT:
					action.CaptureChangedProducts();
					break;

				case Constants.FLG_FLAPS_REPLICATION_DATA_PRODUCTSTOCK:
					action.CaptureChangedProductStocks();
					break;

				default:
					var msg = string.Format(
						"引数の値は '{0}' または '{1}'を指定してください。",
						Constants.FLG_FLAPS_REPLICATION_DATA_PRODUCT,
						Constants.FLG_FLAPS_REPLICATION_DATA_PRODUCTSTOCK);
					FileLogger.WriteError(msg);
					Console.WriteLine(msg);
					break;
			}
		}

		/// <summary>
		/// エラーを通知
		/// </summary>
		/// <param name="replicationType">同期するデータの種類</param>
		private static void NotifyException(string replicationType)
		{
			var msg = string.Format("批处理程序失败。({0})", replicationType);
			using (var mail = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_FLAPS_INTEGRATION_ERROR,
				string.Empty,
				new Hashtable { { "message", msg } }))
			{
				if (mail.SendMail() == false)
				{
					FileLogger.WriteError(mail.MailSendException);
				}
			}
		}
	}
}