/*
=========================================================================================================
  Module      : 外部ファイル取込処理(ExternalFileImport.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using w2.Common.Logger;
using w2.App.Common;

namespace w2.Commerce.Batch.ExternalFileImport
{
	class ExternalFileImport
	{
		/// <summary>店舗IDリスト</summary>
		private List<string> m_lShopIds = new List<string>();

		/// <summary>ファイルタイプリスト</summary>
		private List<string> m_lFileTypes = new List<string>();

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		/// <param name="args">第一引数：店舗ID、第二引数：ファイルタイプ、第三引数ファイル名</param>
		static void Main(string[] args)
		{
			try
			{
				// 実体作成
				var import = new ExternalFileImport();

				// バッチ起動をイベントログ出力
				AppLogger.WriteInfo("起動");

				if (args.Length == 0)
				{
					import.ImportAll();
				}
				else if (args.Length == 3)
				{
					// アクティヴファイルのフルパスを受け取る
					import.ImportActiveFile(args[0], args[1], args[2]);
				}
				else if (args.Length == 4)
				{
					// Import new yahoo order action
					import.ImportActiveFile(args[0], args[1], args[2], args[3]);
				}

				// バッチ終了をイベントログ出力
				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				// エラーイベントログ出力
				AppLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private ExternalFileImport()
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
					ConfigurationSetting.ReadKbn.C200_ExternalFileImport);

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				// 店舗ID取得
				foreach (string strShopId in csSetting.GetAppStringSettingList("ExternalFileImport_ShopIds"))
				{
					m_lShopIds.Add(strShopId.Trim());
				}
				// ファイルタイプ取得
				foreach (string strFileType in csSetting.GetAppStringSettingList("ExternalFileImport_FileTypes"))
				{
					m_lFileTypes.Add(strFileType.Trim());
				}

				Constants.TAX_ROUNDTYPE = csSetting.GetAppStringSetting("ExternalFileImport_Tax_RoundType");
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}

		/// <summary>
		/// ファイル取込
		/// </summary>
		private void ImportAll()
		{
			//------------------------------------------------------
			// 各ファイルタイプについて繰り返し
			//------------------------------------------------------
			foreach (string strFileType in m_lFileTypes)
			{
				//------------------------------------------------------
				// 各店舗IDについて繰り返し
				//------------------------------------------------------
				foreach (string strShopId in m_lShopIds)
				{
					//------------------------------------------------------
					// ACTIVE各ディレクトリ作成
					//------------------------------------------------------
					string strActiveFileDir = Constants.PHYSICALDIRPATH_EXTERNALFILEUPLOAD + strShopId + @"\" + strFileType + @"\" + Constants.DIRNAME_MASTERIMPORT_ACTIVE + @"\";
					if (!Directory.Exists(strActiveFileDir))
					{
						Directory.CreateDirectory(strActiveFileDir);
					}
					string strUploadFileDir = Constants.PHYSICALDIRPATH_EXTERNALFILEUPLOAD + strShopId + @"\" + strFileType + @"\" + Constants.DIRNAME_MASTERIMPORT_UPLOAD + @"\";
					if (!Directory.Exists(strUploadFileDir))
					{
						Directory.CreateDirectory(strUploadFileDir);
					}

					//------------------------------------------------------
					// 各ファイルについて繰り返し
					//------------------------------------------------------
					// ファイル名抽出
					string[] strFilePaths = Directory.GetFiles(strUploadFileDir);
					if (strFilePaths.Length != 0)
					{
						string[] strFiles = new string[strFilePaths.Length];
						for (int iFileLoop = 0; iFileLoop < strFilePaths.Length; iFileLoop++)
						{
							strFiles[iFileLoop] = strFilePaths[iFileLoop].Substring(strFilePaths[iFileLoop].LastIndexOf(@"\") + 1);
						}

						// ファイルについて繰り返し
						foreach (string strFile in strFilePaths)
						{
							//------------------------------------------------------
							// IMPORTファイル取込共通処理
							//------------------------------------------------------
							// アップロードファイルをアクティヴディレクトリへ移動
							File.Move(strFile, strActiveFileDir + strFile.Substring(strFile.LastIndexOf(@"\") + 1));

							// 取込
							ImportActiveFile(strShopId, strFileType, strFile.Substring(strFile.LastIndexOf(@"\") + 1));
						}
					}
				}
			}
		}

		/// <summary>
		/// アクティヴファイル取込
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="fileType">ファイルタイプ</param>
		/// <param name="fileName">アクティブファイル</param>
		/// <param name="mallId">Mall id</param>
		private void ImportActiveFile(
			string shopId,
			string fileType,
			string fileName,
			string mallId = "")
		{
			DateTime dtBegin = DateTime.Now;
			bool blResult = false;
			string strErrorMessage = null;

			string strActiveFilePath = Constants.PHYSICALDIRPATH_EXTERNALFILEUPLOAD + shopId + @"\" + fileType + @"\" + Constants.DIRNAME_MASTERIMPORT_ACTIVE + @"\" + fileName;

			//------------------------------------------------------
			// COMPLETE各ディレクトリ作成
			//------------------------------------------------------
			string strCompleteFileDir = Constants.PHYSICALDIRPATH_EXTERNALFILEUPLOAD + shopId + @"\" + fileType + @"\" + Constants.DIRNAME_MASTERIMPORT_COMPLETE + @"\";
			if (!Directory.Exists(strCompleteFileDir))
			{
				Directory.CreateDirectory(strCompleteFileDir);
			}

			//------------------------------------------------------
			// 処理振り分け
			//------------------------------------------------------
			Imports.ImportBase import = null;
			switch (fileType)
			{
				case "AddYahooOrder":
					import = new Imports.ImportAddYahooOrder(shopId);
					break;

				case "AddYahooOrderItem":
					import = new Imports.ImportAddYahooOrderItem(shopId);
					break;

				case "AddYahooCustomFields":
					import = new Imports.ImportAddYahooCustomFields(shopId);
					break;

				case Constants.FLG_EXTERNALIMPORT_FILE_TYPE_NEW_YAHOO_ORDER:
					import = new Imports.ImportNewYahooOrder(shopId, mallId);
					break;
			}

			int iImportCount = 0;
			try
			{
				//------------------------------------------------------
				// ファイル取込
				//------------------------------------------------------
				iImportCount = import.Import(strActiveFilePath);

				blResult = true;
			}
			catch (Exception ex)
			{
				strErrorMessage = ex.ToString() + "\r\n";
				strErrorMessage += "該当ファイルの処理でエラー対象のレコードはロールバックされました。\r\n";

				blResult = false;
			}

			if (blResult)
			{
				//------------------------------------------------------
				// 正常終了後、ファイルを完了ディレクトリへ移動（ファイル名に成功区分付加）
				//------------------------------------------------------
				string stSuccessFilePath = strCompleteFileDir + System.DateTime.Now.ToString("yyyyMMddHHmmss").Replace("/", "").Replace(" ", "").Replace(":", "") + Constants.IMPORT_RESULT_KBN_SUCCESS + fileName;
				File.Move(strActiveFilePath, stSuccessFilePath);
			}
			else
			{
				//------------------------------------------------------
				// エラー時処理
				//------------------------------------------------------
				// 異常終了後、ファイルを完了ディレクトリへ移動（ファイル名に失敗区分付加）
				string stFailedFilePath = strCompleteFileDir + System.DateTime.Now.ToString("yyyyMMddHHmmss").Replace("/", "").Replace(" ", "").Replace(":", "") + Constants.IMPORT_RESULT_KBN_FAILED + fileName;
				try
				{
					File.Move(strActiveFilePath, stFailedFilePath);
				}
				catch
				{
					// ファイルが移動できなかったらとりあえずログ出力
					FileLogger.WriteError("ファイルを移動することができませんでした。：" + fileName);
				}

				// ログファイルを同名＋.logの形でエラーを出力
				AppLogger.WriteInfo(stFailedFilePath);
				StreamWriter sw = new StreamWriter(stFailedFilePath + ".log");
				sw.WriteLine(strErrorMessage);
				sw.Close();

				// バッチのエラーログにも出力
				FileLogger.WriteError(strErrorMessage);
			}

			DateTime dtEnd = DateTime.Now;

			//------------------------------------------------------
			// 完了メール送信
			//------------------------------------------------------
			// メールテンプレート取得
			Hashtable htMailContents = new Hashtable();
			htMailContents.Add(Constants.IMPORT_MAILTEMPLATE_KEY_FILE_TYPE, fileType);
			htMailContents.Add(Constants.IMPORT_MAILTEMPLATE_KEY_FILE_NAME, fileName);
			if (blResult)
			{
				// 成功時メッセージセット
				string strMessage = "";
				strMessage += "取込開始時間：" + dtBegin.ToString() + "\r\n";
				strMessage += "取込終了時間：" + dtEnd.ToString() + "\r\n";
				strMessage += "\r\n";
				strMessage += "取込完了  : " + iImportCount + "件\r\n";

				htMailContents.Add(Constants.IMPORT_MAILTEMPLATE_KEY_RESULT, "成功");
				htMailContents.Add(Constants.IMPORT_MAILTEMPLATE_MSG, strMessage + "\r\n");

				// 成功ログの出力
				FileLogger.WriteInfo(fileType + "取込に成功しました。（ファイル：" + fileName + "）\r\n" + strMessage);
			}
			else
			{
				// 失敗時メッセージセット
				htMailContents.Add(Constants.IMPORT_MAILTEMPLATE_KEY_RESULT, "失敗");
				htMailContents.Add(Constants.IMPORT_MAILTEMPLATE_MSG, strErrorMessage);
			}

			// 該当ショップのメールテンプレート読みだし・メール送信
			using (MailSendUtility msMailSend = new MailSendUtility(shopId, Constants.CONST_MAIL_ID_EXTERNAL_IMPORT, "", htMailContents, true, Constants.MailSendMethod.Auto))
			{
				if (msMailSend.SendMail() == false)
				{
					// 送信エラーの場合ログ書き込み
					FileLogger.WriteError(this.GetType().BaseType.ToString() + " : " + msMailSend.MailSendException.ToString());
				}
			}
		}
	}
}
