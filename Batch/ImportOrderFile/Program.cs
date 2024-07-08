/*
=========================================================================================================
  Module      : 注文ファイル取り込みバッチ(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using w2.App.Common;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.Commerce.Batch.ImportOrderFile
{
	/// <summary>
	/// 注文ファイル取り込みバッチ
	/// </summary>
	class Program
	{
		/// <summary>
		/// エントリポイント
		/// </summary>
		/// <param name="args">
		/// バッチ引数
		/// ・第一引数：取り込み対象のファイルのフルパス
		/// ・第二引数：取り込みファイルの種別
		/// ★指定可能なファイルタイプ
		/// 　ECAT2000LINK           ：e-cat2000紐づけデータ
		/// 　SHIPPING_NO_LINK       ：w2Commerce標準配送伝票紐付けデータ
		/// 　B2_RAKUTEN_INCL_LINK   ：B2配送伝票紐付けデータ
		/// 　PAYMENT_DATA           ：入金データ
		/// 　IMPORT_ORDER_STATUS    ：受注情報ステータス更新
		/// ・第三引数：出荷情報登録連携するかしないか
		/// 　1      ：連携する
		/// 　1以外  ：連携しない
		/// ・第四引数：実行オペレータ名
		/// ・第五引数：注文ファイル設定値（
		/// ・第六引数：メールテンプレートID（省略可、紐付けデータ用）
		/// </param>
		static void Main(string[] args)
		{
			// Debug
			// args = new string[] { @"C:\inetpub\wwwroot\R5044_Kagome.Develop\Batch\ImportOrderFile\csv\upload\test1.csv", "SHIPPING_NO_LINK", "1", "hogeuser", "SHIPPING_NO_LINK", "" };

			BatchInit();
			AppLogger.WriteInfo("起動");
			var batchArgs = new BatchArgs(args);

			try
			{
				var proc = new BatchProc();
				var result = proc.Execute(batchArgs);
				SendBatchEndMail(result, batchArgs);
				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				SendBatchErrorMail(batchArgs, ex);
				AppLogger.WriteInfo("異常終了");
			}
		}

		#region -BatchInit バッチ初期化
		/// <summary>
		/// バッチ初期化
		/// </summary>
		/// <remarks>XML周りはw2cManagerのものを見るように</remarks>
		static void BatchInit()
		{
			// アプリケーション名設定
			w2.Common.Constants.APPLICATION_NAME = ImportOrderFile.Properties.Settings.Default.Application_Name;

			// アプリケーション共通の設定
			var csSetting = new ConfigurationSetting(
				Properties.Settings.Default.ConfigFileDirPath,
				new[]
				{
					ConfigurationSetting.ReadKbn.C000_AppCommon,
					ConfigurationSetting.ReadKbn.C100_BatchCommon,
					ConfigurationSetting.ReadKbn.C200_ImportOrder,
					ConfigurationSetting.ReadKbn.C200_CommonManager
				});

			// ファイル出力先
			Constants.DIRECTORY_TEMP_PATH = csSetting.GetAppStringSetting("Directory_Temp_Path");
			Constants.DIRECTORY_COMP_PATH = csSetting.GetAppStringSetting("Directory_Comp_Path");

			// SqlXml(管理画面のものを利用)
			w2.Common.Constants.PHYSICALDIRPATH_SQL_STATEMENT = csSetting.GetAppStringSetting("Directory_w2cManager") + w2.Common.Constants.DIRPATH_XML_STATEMENTS;
			// エラーメッセージファイルパス(管理画面のものを利用)
			var msgSetting = new List<string>();
			msgSetting.Add(csSetting.GetAppStringSetting("Directory_w2cManager") + @"Xml/Message/ErrorMessages.xml");
			w2.Common.Constants.PHYSICALFILEPATH_ERROR_MESSAGE_XMLS = msgSetting;
			// ValueTextのパス
			w2.Common.Constants.PHYSICALFILEPATH_VALUETEXT = csSetting.GetAppStringSetting("Directory_w2cManager") + Constants.FILEPATH_XML_VALUE_TEXT.Replace("/", @"\");
		}
		#endregion

		#region -SendBatchEndMail バッチ終了メール送信
		/// <summary>
		/// バッチ終了メール送信
		/// </summary>
		/// <param name="result"></param>
		/// <param name="args">バッチ引数</param>
		private static void SendBatchEndMail(BatchProcResult result, BatchArgs args)
		{
			var ht = new Hashtable();
			if (args.ImportFileType == Constants.KBN_ORDERFILE_IMPORT_ORDER
				&& result.Result == false
				&& result.ResultSuccessCount != 0)
			{
				ht.Add("result", "一部失敗");
			}
			else
			{
				ht.Add("result", (result.Result ? "成功" : "失敗"));
			}
			ht.Add("filetype", "注文関連ファイル取込：" + args.GetFileTypeName());
			ht.Add("file_name", Path.GetFileName(args.ImportFilePath));

			ht.Add("message", result.ResultMessage);

			using (var mail = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, Constants.CONST_MAIL_ID_EXTERNAL_IMPORT, string.Empty, ht))
			{
				if (mail.SendMail() == false)
				{
					FileLogger.WriteError(mail.MailSendException);
				}
			}
		}
		#endregion

		#region -SendBatchErrorMail バッチ異常終了メール送信
		/// <summary>
		/// バッチ異常終了メール送信
		/// </summary>
		/// <param name="args">バッチ引数</param>
		/// <param name="ex">例外</param>
		private static void SendBatchErrorMail(BatchArgs args, Exception ex)
		{
			var ht = new Hashtable();
			ht.Add("result", "失敗");
			ht.Add("filetype", "注文関連ファイル取込：" + args.GetFileTypeName());
			ht.Add("file_name", Path.GetFileName(args.ImportFilePath));
			ht.Add("message", "予期せぬエラーにより異常終了しました。\r\n\r\n" + ex.ToString());

			using (var mail = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, Constants.CONST_MAIL_ID_EXTERNAL_IMPORT, string.Empty, ht))
			{
				if (mail.SendMail() == false)
				{
					FileLogger.WriteError(mail.MailSendException);
				}
			}
		}
		#endregion
	}
}
