/*
=========================================================================================================
  Module      : 連携exe初期化クラス(AppInit.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common;

namespace ExternalAPI
{
	/// <summary>
	/// 連携exe初期化クラス
	/// </summary>
	public static class AppInit
	{
		#region +Init 初期化
		/// <summary>
		/// 初期化
		/// </summary>
		public static void Init()
		{
			try
			{
				//------------------------------------------------------
				// アプリケーション設定読み込み
				//------------------------------------------------------
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

				// アプリケーション共通の設定			
				ConfigurationSetting csSetting = new ConfigurationSetting(
					Properties.Settings.Default.ConfigFileDirPath,
					ConfigurationSetting.ReadKbn.C000_AppCommon,
					ConfigurationSetting.ReadKbn.C100_BatchCommon,
					ConfigurationSetting.ReadKbn.C200_ExternalApi);

				//Constants.PATH_ROOT = csSetting.GetAppStringSetting("Site_RootPath_w2cManager");
				//Constants.PHYSICALDIRPATH_FRONT_PC = Constants.PHYSICALDIRPATH_CONTENTS_ROOT;
				//Constants.CONST_DISP_CONTENTS_PRODUCTREVIEW_LIST = csSetting.GetAppIntSetting("Const_DispListContentsCount_ProductReviewList");
				//Constants.KBN_SORT_PRODUCT_LIST_DEFAULT = csSetting.GetAppStringSetting("SortKbn_ProductList_Default");
				//Constants.PAYMENT_SETTING_DOCOMO_SERVER_URL_DECISION = csSetting.GetAppStringSetting("Payment_Docomo_RealSales_ServerUrl");
				//Constants.PAYMENT_SETTING_DOCOMO_PAYMENT = csSetting.GetAppBoolSetting("Payment_Docomo_RealSales_Enabled");
				//Constants.PAYMENT_SETTING_SOFTBANK_SERVER_URL_DECISION = csSetting.GetAppStringSetting("Payment_Softbank_ServerUrl_Decision");
				//Constants.PAYMENT_SETTING_SOFTBANK_PAYMENT = csSetting.GetAppBoolSetting("Payment_Softbank_Payment");
				//Constants.INVOICECSV_ENABLED = csSetting.GetAppBoolSetting("OrderInvoiceCSV_Enabled");
				//Constants.PDF_OUTPUT_ORDERSTATEMENT_ENABLED = csSetting.GetAppBoolSetting("PdfOutputOrderstatement_Enabled");
				//Constants.TASKSCHEDULER_EXECFILE = csSetting.GetAppStringSetting("Program_CreateAdvertiseFile");

				Constants.PHYSICALDIRPATH_EXTERNALAPI_STORAGE_LOCATION = csSetting.GetAppStringSetting("Setting_DirPath_ExternalApi_Storage_Location");
				Constants.GRANT_ORDER_POINT_AUTOMATICALLY = csSetting.GetAppBoolSetting("Point_GrantOrderPointWithShipment"); // 注文本ポイント自動付与（本ポイント）
				Constants.PHYSICALDIRPATH_ARRIVALMAILSEND_EXE = csSetting.GetAppStringSetting("Program_ArrivalMailSend"); // 入荷通知メール送信EXE
				Constants.PHYSICALDIRPATH_SQL_STATEMENT = csSetting.GetAppStringSetting("Directory_w2cManagerSqlStatementXml"); // SQLステートメントファイルパス
				Constants.PHYSICALFILEPATH_VALUETEXT = csSetting.GetAppStringSetting("File_w2cManagerValueTextXml"); //ValueTextファイルパス
				Constants.PHYSICALDIRPATH_EXTERNALAPI_STORAGE_SETTING_LOCATION = csSetting.GetAppStringSetting("Setting_DirPath_ExternalApi_Storage_Setting_Location");
				Constants.SETTING_DIRPATH_FREEEXPORT = csSetting.GetAppStringSetting("Setting_DirPath_FreeExport");

				Constants.MAIL_SUBJECTHEAD = csSetting.GetAppStringSetting("Mail_SubjectHead");
				Constants.MAIL_FROM = csSetting.GetAppMailAddressSetting("Mail_From");
				Constants.MAIL_TO_LIST = csSetting.GetAppMailAddressSettingList("Mail_To");
				Constants.MAIL_CC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Cc");
				Constants.MAIL_BCC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Bcc");

				// 領収書OP用 プロトコル取得
				Constants.PROTOCOL_HTTP = csSetting.GetAppStringSetting("Site_ProtocolHttp");
				Constants.PROTOCOL_HTTPS = csSetting.GetAppStringSetting("Site_ProtocolHttps");
				Constants.SITE_DOMAIN = csSetting.GetAppStringSetting("Site_Domain");
				Constants.PATH_ROOT_FRONT_PC = csSetting.GetAppStringSetting("Site_RootPath_PCFront");
				Constants.URL_FRONT_PC_SECURE = Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC;

				Constants.SETTING_DIRPATH_LETROEXPORT = csSetting.GetAppStringSetting("Setting_DirPath_LetroExport");
				Constants.LETRO_OPTION_ENABLED = csSetting.GetAppBoolSetting("Letro_Option_Enabled");
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}

		}
		#endregion
	}
}
