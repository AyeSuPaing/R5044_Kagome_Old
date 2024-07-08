/*
=========================================================================================================
  Module      : 構成管理共通モジュール(ConfigurationSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Linq;
using System.Net.Mime;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using w2.App.Common.Global.Config;
using Braintree;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;
using w2.App.Common.RepeatPlusOne.Config;
using w2.App.Common.SubscriptionBox;
using w2.Common.Net.Mail;
using w2.Common.Util;
using w2.Common.Util.TagReplacer;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User.Helper;

namespace w2.App.Common
{

	///*********************************************************************************************
	/// <summary>
	/// アプリケーション設定の設定値を取得する
	/// </summary>
	///*********************************************************************************************
	public class ConfigurationSetting
	{
		///**************************************************************************************
		/// <summary>読取区分</summary>
		///**************************************************************************************
		public enum ReadKbn
		{
			C000_AppCommon,
			C100_SiteCommon,
			C200_CommonFront,
			C300_Pc,
			C300_Mobile,
			C200_CommonManager,
			C300_ComerceManager,
			C300_MarketingPlanner,
			C300_CustomerSupport,
			C300_Cms,
			C100_BatchCommon,
			C200_AccessLogImporter,
			C200_AffiliateReporter,
			C200_ArrivalMailSend,
			C200_CreateDispProduct,
			C200_CreateGoogleMerchantDataFeed,
			C200_CreateReport,
			C200_CreateSitemapXml,
			C200_ExternalApi,
			C200_ExternalFileImport,
			C200_FixedPurchaseBatch,
			C200_ImportErrorMails,
			C200_MasterFileImport,
			C200_MyMenuCheckerDoCoMo,
			C200_OrderPdfCreater,
			C200_OrderPointBatch,
			C200_SendRecommendItem,
			C200_WebRequestSender,
			C200_ScheduleManager,
			C200_MallBatch,
			C200_OrderCancelBatch,
			C200_CsMailReceiver,
			C200_CsWarningMailSender,
			C300_MailOrder,
			C300_MailOrderGetter,
			C300_PopGet,
			C300_MallOrderImporter,
			C300_Mkadv,
			C300_StockUpdate,
			C300_UserIntegrationCreator,
			C300_ExternalPaymentChecker,
			C200_DeleteData,
			C200_CustomerRingsExport,
			C200_Reauth,
			C200_UpdateShipmentForReturnOrder,
			C200_UpdateYamatoKaInvoice,
			C200_ExternalOrderImport,
			C200_UpdateHistoryTransfer,
			C200_ImportOrder,
			C200_LiaiseAmazonMall,
			C200_LiaiseLohacoMall,
			C200_GetExchangeRate,
			C200_TokenDeleteBatch,
			C200_ShippingReceivingStoreStatusImportBatch,
			C200_GooddealGetShippingCheckNo,
			C200_PageDesignConsistency,
			C200_ShippingReceivingStoreSettingImportBatch,
			C200_SilvereggAigentCsvFileUploader,
			C200_TagManagerMigration,
			C200_TwInvoice,
			C200_UpdateSMSState,
			C200_UserAttributeCalculator,
			C200_WmsShippingBatch,
			C200_GooddealShipping,
			C200_AwooProductSync,
		}
		/// <summary> 設定項目定義：キー </summary>
		public const string CONST_KEY = "key";
		/// <summary> 設定項目定義：値 </summary>
		public const string CONST_VALUE = "value";
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ConfigurationSetting()
		{
			//　プロパティ初期化
			this.ReadKbnList = new List<ReadKbn>();
			this.ConfigFilePathList = new List<string>();

			Constants.CONFIGURATION_SETTING = this;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strConfigFileDirPath">コンフィグファイルディレクトリパス</param>
		/// <param name="rkKeys">読取区分</param>
		/// <remarks>
		/// Configの読み込みは後勝ちのため、複数の区分で同名のオプションを利用している場合
		/// 最後に読み込まれた区分の設定値が優先される
		/// </remarks>
		public ConfigurationSetting(
			string strConfigFileDirPath,
			params ReadKbn[] rkKeys)
			: this()
		{
			// アプリケーション初期化時にWebRequestのSSLプロトコルバージョン設定
			System.Net.ServicePointManager.SecurityProtocol
				= System.Net.SecurityProtocolType.Tls12
				| System.Net.SecurityProtocolType.Tls11
				| System.Net.SecurityProtocolType.Tls
				| System.Net.SecurityProtocolType.Ssl3;

			// プロパティセット
			this.ConfigDirPath = strConfigFileDirPath;
			this.ReadKbnList = CreateReadKbnList(rkKeys);
			this.ConfigFilePathList = CreateConfigFilePathList(strConfigFileDirPath);
			this.EnvTagList = CreateEnvTagList(strConfigFileDirPath);

			// 設定値を作成してプロパティにセット
			this.ConfigurationSettingInfo = CreateConfigurationSettingInfo();

			// 全体設定情報取得
			SetConfigSettingForDisplay();

			// 共通で使う定数はここでセットしてしまう
			// アプリケーション共通、サイト共通、バッチ共通など。
			// 残りはそれぞれで行う。（PCFront用の設定であればPCFrontのGlobal.asax）
			Initialize();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="configFileDirPath">コンフィグファイルディレクトリパス</param>
		/// <remarks>
		/// Configの読み込みは後勝ちのため、複数の区分で同名のオプションを利用している場合
		/// 最後に読み込まれた区分の設定値が優先される
		/// </remarks>
		public ConfigurationSetting(string configFileDirPath)
			: this()
		{
			// プロパティセット
			this.ConfigDirPath = configFileDirPath;
			this.ConfigFilePathList = CreateConfigFilePathList(configFileDirPath);

			// 全体設定情報取得
			SetConfigSettingForDisplay();
		}

		/// <summary>
		/// アプリケーション指定で構成
		/// </summary>
		/// <param name="strConfigFileDirPath">コンフィグファイルディレクトリパス</param>
		/// <param name="rkKey">読取区分</param>
		public static ConfigurationSetting CreateInstanceByReadKbn(string strConfigFileDirPath, ReadKbn rkKey)
		{
			var rkKeys = new List<ReadKbn> { rkKey };

			// アプリケーション別の読み取り区分設定
			switch (rkKey)
			{
				// ComerceManager
				case ReadKbn.C300_ComerceManager:
					rkKeys.AddRange(new List<ReadKbn>{
						ReadKbn.C000_AppCommon,
						ReadKbn.C100_SiteCommon,
						ReadKbn.C200_CommonManager,
						ReadKbn.C300_Cms });
					break;

				default:
					throw new ArgumentOutOfRangeException(string.Format("無効な読み取り区分です：{0}", rkKey));
			}

			var instance = new ConfigurationSetting(strConfigFileDirPath, rkKeys.ToArray());
			return instance;
		}

		/// <summary>
		/// 読み込み区分作成
		/// </summary>
		/// <param name="rkKeys">読み込み区分</param>
		/// <returns>読み込み区分リスト</returns>
		private List<ReadKbn> CreateReadKbnList(params ReadKbn[] rkKeys)
		{
			// 読み込み優先度設定のためにSortする
			List<ReadKbn> lReadKbn = new List<ReadKbn>(rkKeys);
			lReadKbn.Sort();
			return lReadKbn;
		}

		/// <summary>
		/// 設定ファイルパスリスト作成
		/// </summary>
		/// <param name="configFileDirPath">CustomerResouceへのパス</param>
		/// <returns>設定ファイルリスト</returns>
		private List<string> CreateConfigFilePathList(string configFileDirPath)
		{
			var paths = new List<string>();
			Console.WriteLine(configFileDirPath);
			var appAllConfigPath = Path.Combine(configFileDirPath, "AppConfig", "base", "AppAll.Config");

			// AppConfig\base\AppAll.Configファイルを追加
			paths.Add(appAllConfigPath);

			// ファイルシステムがNTFSの場合、GetFilesの結果がファイル名順に取ってこれます。
			// AppConfig\base\直下のみで、AppAll.Config以外のConfigファイルを追加
			paths.AddRange(
				Directory.GetFiles(
					Path.Combine(configFileDirPath, "AppConfig", "base"), "*.Config")
						.Where(configFile => (appAllConfigPath != configFile)));

			// AppConfig直下のみのConfigファイルを追加
			paths.AddRange(
				Directory.GetFiles(
					Path.Combine(configFileDirPath, "AppConfig"), "*.Config"));

			return paths;
		}

		/// <summary>
		/// 環境タグリスト作成
		/// </summary>
		/// <param name="strConfigFileDirPath">CustomerResouceへのパス</param>
		/// <returns>環境タグリスト(key:タグ,value:値)</returns>
		private Dictionary<string, string> CreateEnvTagList(string strConfigFileDirPath)
		{
			Dictionary<string, string> dicTagList = new Dictionary<string, string>();

			// SetEnv読み込み(EnvTag要素のtag属性とvalue属性の値を取得）
			XDocument xEnv = XDocument.Load(strConfigFileDirPath + @"AppConfig\_EnvSetting.xml");
			var vEnvTagList = from xeEnv in xEnv.Descendants("EnvTag")
							  select new
							  {
								  Tag = xeEnv.Attribute("tag").Value,
								  Value = xeEnv.Attribute("value").Value
							  };

			foreach (var taginfo in vEnvTagList)
			{
				// タグが複数存在した場合はあと勝ち
				dicTagList[taginfo.Tag] = taginfo.Value;
			}

			return dicTagList;
		}

		/// <summary>
		/// 設定値格納Hashを作成
		/// </summary>
		/// <returns>設定値がすべて格納されたHashtable</returns>
		public Hashtable CreateConfigurationSettingInfo()
		{
			Hashtable htResults = new Hashtable();

			// 区分でソート済みのため、順番に取り出すだけでよい
			foreach (var vappconfig in CreateAppConfigList())
			{
				foreach (var vSetting in vappconfig.SettingList)
				{
					htResults[vSetting.Key] = ReplaceEnvTag(vSetting.Value);
				}
			}

			return htResults;
		}

		/// <summary>
		/// 設定値格納オブジェクトを作成
		/// </summary>
		/// <returns>設定値オブジェクトリスト</returns>
		private List<AppConfigs> CreateAppConfigList()
		{
			// 設定値を格納するオブジェクトを作成
			List<AppConfigs> lAppConfigs = new List<AppConfigs>();
			foreach (var rkKbn in this.ReadKbnList)
			{
				lAppConfigs.Add(new AppConfigs(rkKbn));
			}

			// 設定値オブジェクトに設定値をセットしていく
			foreach (var configPath in this.ConfigFilePathList)
			{
				// 設定値を読み込み区分と合わせて取得
				var vSettingList = GetSettingNodes(configPath);

				foreach (var rkKbn in this.ReadKbnList)
				{
					// 読み込んだ設定値の追加先を決定する（区分で一意になるはずなのでFirst）
					Dictionary<string, string> dicTarget =
						lAppConfigs.Where(config => (config.ReadKbn == rkKbn)).Select(config => config.SettingList).First();

					foreach (var vSetting in vSettingList.Where(s => (s.ReadKbn == rkKbn.ToString("F"))))
					{
						// 既に存在している場合は後勝ちとする
						dicTarget[vSetting.Key] = vSetting.Value;
					}
				}
			}

			return lAppConfigs;
		}

		/// <summary>
		/// 設定値を読み込み区分と合わせて取得
		/// </summary>
		/// <param name="filePath">設定ファイルパス</param>
		/// <returns>「SettingNode」モデルのリスト</returns>
		private List<SettingNode> GetSettingNodes(string filePath)
		{
			var settingNodes = XDocument.Load(filePath).XPathSelectElements("//Setting")
				.Where(e => ((e.Attribute(CONST_KEY) != null) && Enum.IsDefined(typeof(ReadKbn), e.Parent.Name.ToString())))
				.Select(e => new SettingNode
				{
					ReadKbn = e.Parent.Name.ToString(),
					Key = e.Attribute(CONST_KEY).Value,
					Value = e.Attribute(CONST_VALUE) != null ? e.Attribute(CONST_VALUE).Value : string.Empty,
					Comment = ((e.PreviousNode != null) && (e.PreviousNode.NodeType == XmlNodeType.Comment))
						? e.PreviousNode.ToString().Replace("<!--", string.Empty).Replace("-->", string.Empty).Trim()
						: string.Empty,
					AppAllValue = string.Empty,
					DataType = typeof(string),
					BeforeValue = e.Attribute(CONST_VALUE) != null ? e.Attribute(CONST_VALUE).Value : string.Empty,
					PrimaryConfig = filePath
				})
				.ToList();

			// AppAll.Configファイルでの設定値をセット
			if (filePath.ToLower().Contains(Constants.FILENAME_APPALL_CONFIG.ToLower()))
			{
				settingNodes.ForEach(s => s.AppAllValue = s.Value);
			}

			return settingNodes;
		}

		/// <summary>
		/// 全体設定情報取得（EC管理の設定画面に表示用）
		/// </summary>
		/// <returns>設定情報リスト</returns>
		private void SetConfigSettingForDisplay()
		{
			var allConfig = new List<SettingNode>();
			// 各設定ファイルの内容を取得し、結果の値は後勝ちとなる
			foreach (var configPath in this.ConfigFilePathList)
			{
				var config = GetSettingNodes(configPath);
				// 設定値を結合
				CombinateValues(allConfig, config);
			}

			this.SettingNodeList = allConfig.OrderBy(c => c.ReadKbn).ToList();
		}

		/// <summary>
		/// 設定情報結合
		/// </summary>
		/// <param name="target">ターゲットの設定情報</param>
		/// <param name="source">元の設定情報</param>
		/// <returns>ターゲットに元の設定情報を結合されるリスト</returns>
		private void CombinateValues(List<SettingNode> target, List<SettingNode> source)
		{
			// ターゲットの項目がない場合、元の全項目を追加
			if (target.Count == 0)
			{
				target.AddRange(source);
				return;
			}

			// ターゲットに存在しない項目を追加したり、項目値を書き換えたりする
			foreach (var node in source)
			{
				var findItem = target.Find(item => ((item.ReadKbn == node.ReadKbn) && (item.Key == node.Key)));
				if (findItem == null)
				{
					node.AppAllValue = "(キーなし)";
					target.Add(node);
				}
				else
				{
					findItem.PrimaryConfig = node.PrimaryConfig;
					findItem.Value = node.Value;
					findItem.BeforeValue = node.BeforeValue;
				}
			}
		}

		/// <summary>
		/// 環境タグ置換
		/// </summary>
		/// <param name="strValue">置換対象文字列</param>
		/// <returns>置換後文字列</returns>
		private string ReplaceEnvTag(string strValue)
		{
			string strTarget = strValue;

			foreach (var envtag in this.EnvTagList)
			{
				strTarget = strTarget.Replace(envtag.Key, envtag.Value);
			}

			return strTarget;
		}

		/// <summary>
		/// 読取区分からそれぞれ共通のアプリケーション設定情報を初期化
		/// </summary>
		public void Initialize()
		{
			foreach (ReadKbn rkKey in this.ReadKbnList)
			{
				switch (rkKey)
				{
					// アプリケーション共通
					case ReadKbn.C000_AppCommon:
						InitializeAppCommon();
						break;

					// サイト共通
					case ReadKbn.C100_SiteCommon:
						InitializeSiteCommon();
						break;

					// フロントサイト共通
					case ReadKbn.C200_CommonFront:
						w2.Domain.Constants.UPDATEHISTORY_APPLICATIONI_TYPE = ApplicationType.Front;
						InitializeFrontCommon();
						break;

					// 管理サイト共通
					case ReadKbn.C200_CommonManager:
						w2.Domain.Constants.UPDATEHISTORY_APPLICATIONI_TYPE = ApplicationType.Manager;
						InitializeManagerCommon();
						break;

					// バッチ共通
					case ReadKbn.C100_BatchCommon:
						w2.Domain.Constants.UPDATEHISTORY_APPLICATIONI_TYPE = ApplicationType.Batch;
						InitializeBatchCommon();
						break;

					// バッチ共通
					case ReadKbn.C200_CsMailReceiver:
						InitializeCsMailReceiver();
						break;
				}
			}
		}

		/// <summary>
		/// アプリケーション設定情報を初期化(全体)
		/// </summary>
		public void InitializeAppCommon()
		{
			// SQL接続文字列設定
			Constants.STRING_SQL_CONNECTION = GetAppStringSetting("Connection_String");
			// 転送先SQL接続文字列設定
			Constants.STRING_SQL_CONNECTION_TRANSFER = GetAppStringSetting("Connection_String_Transfer");

			// 本番完了設定
			Constants.SETTING_PRODUCTION_ENVIRONMENT = GetAppBoolSetting("Setting_Production_Environment");
			// SqlStatementXml格納ディレクトリ物理パス
			Constants.PHYSICALDIRPATH_SQL_STATEMENT = AppDomain.CurrentDomain.BaseDirectory + Constants.DIRPATH_XML_STATEMENTS;
			// ヴァリデータパス
			Constants.PHYSICALDIRPATH_VALIDATOR = AppDomain.CurrentDomain.BaseDirectory + Constants.DIRPATH_XML_VALIDATORS;
			// エラーメッセージファイルパス
			Constants.PHYSICALFILEPATH_ERROR_MESSAGE_XMLS.Add(AppDomain.CurrentDomain.BaseDirectory + @"Xml/Message/ErrorMessages.xml");
			// 案件ごとのプラン名
			Constants.PLAN_NAME = GetAppStringSetting("Plan_Name");
			// プロジェクト名
			Constants.PROJECT_NO = GetAppStringSetting("Project_No");
			// 環境名
			Constants.ENVIRONMENT_NAME = GetAppStringSetting("Environment_Name");
			// プロジェクトNO＋アプリケーション名
			Constants.APPLICATION_NAME_WITH_PROJECT_NO = string.Join(".", Constants.PROJECT_NO, Constants.APPLICATION_NAME);

			// 物理パス設定
			Constants.PHYSICALDIRPATH_EXTERNALFILEUPLOAD = GetAppStringSetting("Directory_ExternalFileImport_ExternalFile");
			Constants.PHYSICALDIRPATH_LOGFILE = GetAppStringSetting("Directory_LogFilePath") + Constants.APPLICATION_NAME + @"\";
			Constants.PHYSICALDIRPATH_OPERATION_LOGFILE = GetAppStringSetting("Directory_OperationLogFilePath");
			Constants.PHYSICALDIRPATH_OPERATION_NOTSEND_LOGFILE = GetAppStringSetting("Directory_OperationLogNotSentFilePath");
			Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR = GetAppStringSetting("Directory_MasterUploadFilePath");
			Constants.PHYSICALDIRPATH_FRONT_PC = GetAppStringSetting("Directory_PCFront");
			Constants.PHYSICALDIRPATH_FRONT_MOBILE = GetAppStringSetting("Directory_MobileFront");
			Constants.PHYSICALDIRPATH_CONTENTS_ROOT = (string.IsNullOrEmpty(Constants.PHYSICALDIRPATH_FRONT_PC) == false)
				? Constants.PHYSICALDIRPATH_FRONT_PC
				: Constants.PHYSICALDIRPATH_FRONT_MOBILE;
			Constants.PHYSICALDIRPATH_FRONT_PC_XML = Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, "Xml");
			Constants.PHYSICALDIRPATH_EXTERNAL_EXE = GetAppStringSetting("Program_ExternalFileimport");
			Constants.PHYSICALDIRPATH_SENDRECOMMENDITEM_EXE = GetAppStringSetting("Program_SendRecommendItem");
			Constants.PHYSICALDIRPATH_COMMERCE_MANAGER = GetAppStringSetting("Directory_w2cManager");
			Constants.PHYSICALDIRPATH_CMS_MANAGER = GetAppStringSetting("Directory_w2cmsManager");
			Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE = GetAppStringSetting("Directory_CustomerResource");
			Constants.PHYSICALDIRPATH_OPERATION_UPDATEHISTORY_LOGFILE = GetAppStringSetting("Directory_OperationUpdateHistoryLogFilePath");
			Constants.PHYSICALDIRPATH_FLAPS_API_LOGFILE = GetAppStringSetting("Directory_FLAPS_Api_LogFilePath");

			// 受注管理の在庫連動可否
			Constants.ORDERMANAGEMENT_STOCKCOOPERATION_ENABLED = GetAppBoolSetting("OrderManagementStockCooperation_Enabled");
			// WFでの出荷報告時の外部決済ST
			Constants.EXTERNAL_PAYMENT_STATUS_SHIPCOMP_ORDERWORKFLOW_EXTERNALSHIPMENTACTION
				= GetAppBoolSetting("External_Payment_Status_ShipComp_OrderWorkflow_ExternalShipmentAction");

			// 管理側の言語ロケールコード
			Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE = GetAppStringSetting("Operational_Language_Locale_Code");
			//------------------------------------------------------
			// DataSchema設定
			//------------------------------------------------------
			InitializeDataSchema();
			// DataSchemaSetting監視起動（DataSchema置換設定初期化処理セット）
			FileUpdateObserver.GetInstance().AddObservation(
				Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE + @"DataSchema\",
				"ReplaceTagSetting.xml",
				InitializeDataSchema);

			// サイトドメイン
			Constants.SITE_DOMAIN = GetAppStringSetting("Site_Domain");
			Constants.PATH_ROOT_FRONT_PC = GetAppStringSetting("Site_RootPath_PCFront");
			Constants.PATH_ROOT_EC = GetAppStringSetting("Site_RootPath_w2cManager");
			Constants.PATH_ROOT_MP = GetAppStringSetting("Site_RootPath_w2mpManager");
			Constants.PATH_ROOT_CS = GetAppStringSetting("Site_RootPath_w2csManager");
			Constants.PATH_ROOT_CMS = GetAppStringSetting("Site_RootPath_w2cmsManager");

			// 決済設定
			Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE = GetAppBoolSetting("Payment_Credit_PaymentStatusComplete");
			Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE_FORDIGITALCONTENTS = GetAppBoolSetting("Payment_Credit_PaymentStatusComplete_ForDigitalContents");
			Constants.PAYMENT_CHOOSE_TYPE = string.IsNullOrEmpty(GetAppStringSetting("Payment_Choose_Type"))
				? Constants.PAYMENT_CHOOSE_TYPE_RB
				: GetAppStringSetting("Payment_Choose_Type");
			Constants.PAYMENT_CHOOSE_TYPE_LP_OPTION = GetAppBoolSetting("Payment_Choose_Type_LP_Option");

			// SMTPサーバ設定
			// 配列順内容：Server,Port,AuthType,PopServer,PopPort,PopType,UserName,Password
			string[] strSmtpSettings = GetAppStringSetting("Server_Smtp_Settings").Split(',');
			Constants.SERVER_SMTP = strSmtpSettings[0];
			Constants.SERVER_SMTP_PORT = int.Parse(strSmtpSettings[1]);
			foreach (Enum e in Enum.GetValues(typeof(w2.Common.SmtpAuthType)))
			{
				if (e.ToString().ToUpper() == strSmtpSettings[2].ToUpper())
				{
					Constants.SERVER_SMTP_AUTH_TYPE = (w2.Common.SmtpAuthType)e;
					break;
				}
			}
			if (Constants.SERVER_SMTP_AUTH_TYPE == w2.Common.SmtpAuthType.PopBeforeSmtp)
			{
				Constants.SERVER_SMTP_AUTH_POP_SERVER = strSmtpSettings[3];
				Constants.SERVER_SMTP_AUTH_POP_PORT = strSmtpSettings[4];
				foreach (Enum e in Enum.GetValues(typeof(w2.Common.PopType)))
				{
					if (e.ToString().ToUpper() == strSmtpSettings[5].ToUpper())
					{
						Constants.SERVER_SMTP_AUTH_POP_TYPE = (w2.Common.PopType)e;
						break;
					}
				}
				Constants.SERVER_SMTP_AUTH_USER_NAME = strSmtpSettings[6];
				Constants.SERVER_SMTP_AUTH_PASSOWRD = strSmtpSettings[7];
			}
			else if (Constants.SERVER_SMTP_AUTH_TYPE == w2.Common.SmtpAuthType.SmtpAuth)
			{
				Constants.SERVER_SMTP_AUTH_USER_NAME = strSmtpSettings[6];
				Constants.SERVER_SMTP_AUTH_PASSOWRD = strSmtpSettings[7];
			}

			// 管理者向け注文完了メール送信オプション
			Constants.SEND_ORDER_COMPLETE_EMAIL_FOR_OPERATOR_ENABLED_LIST = GetAppStringSettingList("SendOrderCompleteEmail_ForOperator_Enabled_List");

			// メール受信エラー通知メールアドレス
			Constants.MAIL_RECV_ERROR_MAILADDR_FROM = GetAppStringSetting("MailRecvError_MailAddrFrom");
			Constants.MAIL_RECV_ERROR_MAILADDR_TO = GetAppStringSetting("MailRecvError_MailAddrTo").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			// エラーメール送信先
			Constants.ERROR_MAILADDRESS = GetAppStringSetting("MailDistribute_ErrorMailAddress");

			// ショートURL拒否拡張子
			Constants.SHORTURL_DENY_EXTENSIONS = new List<string>(GetAppStringSetting("ShortUrl_Deny_Extensions").Split(','));

			// PCメール送信のデフォルトエンコーディング表示名
			Constants.PC_MAIL_DEFAULT_ENCODING_STRING = GetAppStringSetting("Pc_Mail_Default_Encoding");
			// PCメール送信のデフォルトエンコーディング
			Constants.PC_MAIL_DEFAULT_ENCODING = System.Text.Encoding.GetEncoding(Constants.PC_MAIL_DEFAULT_ENCODING_STRING);

			// PCメール送信のデフォルトエンコーディング(Content-Transfer-Encoding用)
			// ※「ISO-2022-JP」の場合、7bitを、そうでない場合はbase64を指定
			Constants.PC_MAIL_DEFAULT_TRANSFER_ENCODING = (Constants.PC_MAIL_DEFAULT_ENCODING == Constants.MOBILE_MAIL_ENCODING)
				? TransferEncoding.SevenBit
				: TransferEncoding.Base64;

			// メールに表示する金額のフォーマット
			Constants.SETTING_MAIL_PRICE_FORMAT = GetAppStringSetting("Setting_Mail_Price_Format");
			// メールテンプレート:商品価格 税込み・税抜き表記 表記:true 非表示false
			Constants.SETTING_MAIL_PRODUCT_PRICE_TAX_TEXT_DISPLAY = GetAppBoolSetting("Setting_Mail_Product_Price_Tax_Text_Display");

			// ユーザーパスワード暗号化設定
			// 注:KEYとIVの区切り文字は半角空白
			string[] strSettingUserEncryptionPasswords = GetAppStringSetting("Setting_UserEncryptionKey").Split(' ');
			Constants.ENCRYPTION_USER_PASSWORD_KEY = Convert.FromBase64String(strSettingUserEncryptionPasswords[0]);
			Constants.ENCRYPTION_USER_PASSWORD_IV = Convert.FromBase64String(strSettingUserEncryptionPasswords[1]);
			Constants.BASIC_AUTHENTICATION_USER_ACCOUNT = GetAppStringSetting("Site_BasicAuthentication_UserAccount");

			// 各種オプション利用有無系
			Constants.MOBILEOPTION_ENABLED = GetAppBoolSetting("MobileOption_Enabled");
			Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED = GetAppBoolSetting("DisplayMobileDatasOption_Enabled");
			Constants.SMARTPHONE_OPTION_ENABLED = GetAppBoolSetting("SmartPhoneOption_Enabled");
			Constants.W2MP_AFFILIATE_OPTION_ENABLED = GetAppBoolSetting("AffiliateOption_Enabled");//マーケティング：MARKETINGPLANNER_AFFILIATE_OPTION_ENABLE
			Constants.W2MP_COUPON_OPTION_ENABLED = GetAppBoolSetting("CouponOption_Enabled");//マーケティング：MARKETINGPLANNER_COUPON_OPTION_ENABLE
			Constants.W2MP_POINT_OPTION_ENABLED = GetAppBoolSetting("PointOption_Enabled");//マーケティング：MARKETINGPLANNER_POINT_OPTION_ENABLE
			Constants.REVIEW_REWARD_POINT_ENABLED = GetAppBoolSetting("ReviewRewardPoint_Enabled");
			Constants.REVIEW_REWARD_POINT_GRANT_LIMIT = GetAppStringSetting("ReviewRewardPoint_GrantLimit");
			Constants.W2MP_MULTIPURPOSE_AFFILIATE_OPTION_ENABLED = GetAppBoolSetting("MultiPurpose_AffiliateOption_Enabled");//マーケティング：MARKETINGPLANNER_MULTIPURPOSE_AFFILIATE_OPTION_ENABLE
			Constants.EXTERNAL_IMPORT_OPTION_ENABLED = GetAppBoolSetting("ExternalImportOption_Enabled");
			Constants.MALLCOOPERATION_OPTION_ENABLED = GetAppBoolSetting("MallCooperationOption_Enabled");
			Constants.FIXEDPURCHASE_OPTION_ENABLED = GetAppBoolSetting("FixedPurchaseOption_Enabled");
			Constants.FIXEDPURCHASE_OPTION_CART_SEPARATION = GetAppBoolSetting("FixedPurchaseOption_CartSeparation");
			Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE = (NextShippingCalculationMode)GetAppSetting("FixedPurchaseOption_NextShippingCalculationMode", typeof(NextShippingCalculationMode));
			Constants.GIFTORDER_OPTION_ENABLED = GetAppBoolSetting("GiftOrderOption_Enabled");
			Constants.CARTCOPY_OPTION_ENABLED = GetAppBoolSetting("CartCopy_Enabled");
			Constants.CART_SHIPPINGMETHOD_UNSELECTED_PRIORITY = GetAppStringSetting("Cart_ShippingMethod_UnSelected_Priority");
			Constants.NOVELTY_OPTION_ENABLED = GetAppBoolSetting("NoveltyOption_Enabled");
			Constants.RECOMMEND_OPTION_ENABLED = GetAppBoolSetting("RecommendOption_Enabled");
			Constants.DIGITAL_CONTENTS_OPTION_ENABLED = GetAppBoolSetting("DigitalContentsOption_Enabled");
			Constants.MARKETINGPLANNER_USE_TEMPORARY_POINT = GetAppBoolSetting("PointOption_UseTemporaryPoint");
			Constants.POINT_OPTION_USE_TAX_EXCLUDED_POINT = GetAppBoolSetting("PointOption_UseTaxExcludedPoint");
			Constants.POINT_OPTION_TAXEXCLUDED_FRACTION_ROUNDING = GetAppStringSetting("PointOption_TaxExcludedFractionRounding");
			Constants.POINTRULE_OPTION_CLICKPOINT_ENABLED = GetAppBoolSetting("PointOption_UseClickPointRule"); //クリックポイント発行
			Constants.PRODUCT_ORDER_LIMIT_ENABLED = GetAppBoolSetting("ProductOrderLimit_Enabled"); //商品購入制限利用可否
			Constants.PRODUCT_ORDER_LIMIT_KBN_CAN_BUY = (Constants.ProductOrderLimitKbn?)GetAppSetting("ProductOrderLimit_Kbn", typeof(Constants.ProductOrderLimitKbn)); //商品購入制限利用区分
			Constants.TAX_EXCLUDED_FRACTION_ROUNDING = GetAppStringSetting("TaxExcludedFractionRounding");
			Constants.MULTIPURPOSE_AFFILIATE_OPTION_TAXEXCLUDED_FRACTION_ROUNDING = GetAppStringSetting("MultiPurpose_AffiliateOption_TaxExcludedFractionRounding");
			Constants.PERCENTOFF_FRACTION_ROUNDING = GetAppStringSetting("PercentOffFractionRounding");
			Constants.ORDER_ITEM_DISCOUNTED_PRICE_ENABLE = GetAppBoolSetting("Order_Item_Discounted_Price_Display");
			Constants.DISCOUNTED_PRICE_FRACTION_ROUNDING = GetAppStringSetting("DiscountedPriceFractionRounding");
			Constants.MEMBER_RANK_OPTION_ENABLED = GetAppBoolSetting("MemberRankOption_Enabled");
			Constants.USERINTEGRATION_OPTION_ENABLED = GetAppBoolSetting("UserIntegrationOption_Enabled");
			Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED = GetAppBoolSetting("LoginIdUseMailaddress_Enabled");
			Constants.PRODUCT_BRAND_ENABLED = GetAppBoolSetting("ProductBrand_Enabled");
			Constants.PRODUCT_SALE_OPTION_ENABLED = GetAppBoolSetting("ProductSaleOption_Enabled");
			Constants.PRODUCT_SET_OPTION_ENABLED = GetAppBoolSetting("ProductSetOption_Enabled");
			Constants.SETPROMOTION_OPTION_ENABLED = GetAppBoolSetting("SetPromotionOption_Enabled");
			Constants.REALSHOP_OPTION_ENABLED = GetAppBoolSetting("RealShop_Enabled");
			Constants.PRODUCTGROUP_OPTION_ENABLED = GetAppBoolSetting("ProductGroupOption_Enabled");
			Constants.PRODUCTBUNDLE_OPTION_ENABLED = GetAppBoolSetting("ProductBundleOption_Enabled"); // 商品同梱機能
			Constants.ORDER_COMBINE_OPTION_ENABLED = GetAppBoolSetting("OrderCombineOption_Enabled");
			Constants.FIXED_PURCHASE_COMBINE_OPTION_ENABLED = GetAppBoolSetting("FixedPurchaseCombineOption_Enabled");
			Constants.SEND_FIXED_PURCHASE_DEADLINE_NOTIFICATION_MAIL_TO_USER = GetAppBoolSetting("SendFixedPurchaseDeadlineNotificationMailToUser");

			Constants.DM_SHIPPING_HISTORY_OPTION_ENABLED = GetAppBoolSetting("DmShippingHistoryOption_Enabled");
			Constants.SITEMAP_OPTION_ENABLED = GetAppBoolSetting("SitemapOption_Enabled");
			Constants.FRIENDLY_URL_ENABLED = GetAppBoolSetting("FriendlyUrlEnabled");
			Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG = GetAppBoolSetting("FixedPurchase_UseShippingIntervalDaysDefault_Flg");

			Constants.MANAGEMENT_INCLUDED_TAX_FLAG = GetAppBoolSetting("ManagementIncludedTaxFlag"); // 税込み管理フラグ
			Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG = GetAppBoolSetting("GlobalTransactionIncludedTaxFlag"); // 海外配送時税込み請求フラグ

			Constants.FRONT_PRODUCTURL_OMIT_EMPTY_QUERYPARAMETER = GetAppBoolSetting("Front_ProductUrl_Omit_Empty_QueryParameter"); // 商品URLの空パラメーター出力フラグ

			Constants.VARIATION_FAVORITE_CORRESPONDENCE = GetAppBoolSetting("Variation_Favorite_Correspondence");// SKU単位でのお気に入り対応

			Constants.FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT_ALL_OPTION_ENABLE = GetAppBoolSetting("FixedPurchase_Next_Shipping_Use_Point_All"); // 定期の全ポイント継続利用機能

			// 定期商品変更オプション
			Constants.FIXEDPURCHASE_PRODUCTCHANGE_ENABLED = GetAppBoolSetting("FixedPurchase_ProductChange_Enabled");

			// 日別出荷予測機能を利用するかどうか
			Constants.SHIPTMENT_FORECAST_BY_DAYS_ENABLED = GetAppBoolSetting("ShipmentForecastByDays_Enabled");

			// 外部レコメンド連携設定系
			Constants.RECOMMEND_ENGINE_KBN = (Constants.RecommendEngine?)GetAppSetting("Recommend_Engine_Kbn", typeof(Constants.RecommendEngine));
			Constants.RECOMMEND_SILVEREGG_MERCHANT_ID = GetAppStringSetting("Recommend_Silveregg_Merchant_Id");
			Constants.RECOMMEND_SILVEREGG_API_DOMAIN = GetAppStringSetting("Recommend_Silveregg_Api_Domain");
			Constants.RECOMMEND_SILVEREGG_REPORT_API_URL = GetAppStringSetting("Recommend_Silveregg_report_Api_Url");
			Constants.RECOMMEND_SILVEREGG_REPORT_API_TOKEN = GetAppStringSetting("Recommend_Silveregg_report_Api_Token");
			Constants.RECOMMEND_SILVEREGG_FTP_HOST = GetAppStringSetting("Recommend_Silveregg_Ftp_Host");
			Constants.RECOMMEND_SILVEREGG_FTP_ID = GetAppStringSetting("Recommend_Silveregg_Ftp_Id");
			Constants.RECOMMEND_SILVEREGG_FTP_PW = GetAppStringSetting("Recommend_Silveregg_Ftp_Pw");

			// その他
			Constants.W2MP_DEPT_ID = GetAppStringSetting("Const_DeptId");
			Constants.FORMAT_ORDER_ID = GetAppStringSetting("Format_OrderId");
			Constants.FORMAT_PAYMENT_ORDER_ID = GetAppStringSetting("Format_PaymentOrderId");
			Constants.FORMAT_PAYMENT_YAMATOKWC_MEMBER_ID = GetAppStringSetting("Format_YamatoKwc_MemberId");
			Constants.FORMAT_FIXEDPURCHASE_ID = GetAppStringSetting("Format_FixedPurchaseId");
			Constants.ORDER_NUMBERING_TYPE = (Constants.OrderNumberingType?)GetAppSetting("Order_Numbering_Type", typeof(Constants.OrderNumberingType));
			Constants.CONST_ORDER_ID_LENGTH = GetAppIntSetting("Const_OrderId_Length");
			Constants.CONST_USER_ID_HEADER = GetAppStringSetting("Const_UserId_Header");
			Constants.CONST_USER_ID_LENGTH = GetAppIntSetting("Const_UserId_Length");
			Constants.MARKETINGPLANNER_DECOME_MOBILEHTMLMAIL_DIRPATH = GetAppStringSetting("Mobile_DecomeImgDirPath");
			Constants.MARKETINGPLANNER_DECOME_MOBILEHTMLMAIL_URL = GetAppStringSetting("Mobile_DecomeImgUrl");
			Constants.PRODUCTSUBIMAGE_FOOTER = GetAppStringSetting("Setting_ProductSubImage_Footer");
			Constants.PRODUCTSUBIMAGE_NUMBERFORMAT = GetAppStringSetting("Setting_ProductSubImage_NumberFormat");
			Constants.SHIPPINGPRIORITY_SETTING = (Constants.ShippingPriority?)GetAppSetting("Setting_ShippingPriority", typeof(Constants.ShippingPriority));
			Constants.THANKSMAIL_FOR_OPERATOR_ENABLED = GetAppBoolSetting("ThanksMail_For_Operator_Enabled");
			Constants.PHYSICALDIRPATH_TEMP = Path.Combine(Path.GetTempPath(), Constants.SITE_DOMAIN, Constants.APPLICATION_NAME_WITH_PROJECT_NO);
			Constants.CONST_SHIPPING_TAXRATE = decimal.Parse(GetAppStringSetting("ShippingTaxRate"));
			Constants.CONST_PAYMENT_TAXRATE = decimal.Parse(GetAppStringSetting("PaymentTaxRate"));
			Constants.SEOSETTING_CHILD_CATEGORY_TOP_COUNT = GetAppIntSetting("ChildCategoryTopCount");
			Constants.ROOT_CATEGORY_SORT_KBN = GetAppStringSetting("Root_Category_SortKbn");
			Constants.SETPROMOTION_MAXIMUM_NUMBER_OF_COMBINATIONS = GetAppIntSetting("SetPromotion_MaximumNumberOfCombinations");
			Constants.SETPROMOTION_MAXIMUM_NUMBER_OF_TARGET_SKUS = GetAppIntSetting("SetPromotion_MaximumNumberOfTargetSKUs");
			Constants.SETPROMOTION_APPLY_ORDER_OPTION_ENABLED = GetAppBoolSetting("SetPromotion_ApplyOrderOption");
			Constants.MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED = GetAppBoolSetting("MailSendBothPcAndMobile_Enabled");
			Constants.EITHER_ENTER_MAIL_ADDRESS_ENABLED = GetAppBoolSetting("EitherEnterMailAddress_Enabled");
			Constants.ORDERMEMO_REGISTERMODE = (Constants.OrderMemoRegisterMode?)GetAppSetting("OrderMemo_RegisterMode", typeof(Constants.OrderMemoRegisterMode));
			Constants.CAN_FIXEDPURCHASE_PAYMENTIDS = GetAppStringSetting("Setting_CanFixpurchasePaymentIds").Split(',').Where(paymentId => paymentId != "").ToList();
			Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE = GetAppStringSetting("BlacklistCoupon_UsedUserJudgeType");
			Constants.MYPAGE_ORDER_MODIFY_BY_DATE = (int?)GetAppSetting("MypageOrderModifyByDate", typeof(int?));
			Constants.PRODUCTSUBIMAGE_MAXCOUNT = GetAppIntSetting("ProductSubImage_MaxCount");
			Constants.PRODUCT_FIXED_PURCHASE_STRING = GetAppStringSetting("FixedPurchase_ItemName_PreFix");
			Constants.PRODUCT_SUBSCRIPTION_BOX_STRING = GetAppStringSetting("SubscriptionBox_ItemName_PreFix");
			Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED = HttpUtility.UrlEncode(GetAppStringSetting("QueryString_For_UpdateExternalFile"));
			Constants.OPERATIONAL_BASE_ISO_CODE = GetAppStringSetting("Operational_Base_Iso_Code");
			Constants.OPERATIONAL_BASE_PROVINCE = GetAppStringSetting("Operational_Base_Province");
			Constants.CURRENT_VERSION = GetAppStringSetting("Current_Version");
			Constants.ORDERWORKFLOW_AUTOEXEC_OPTION_ENABLE = GetAppBoolSetting("OrderWorkflowAutoExecOption_Enable");
			Constants.AB_TEST_OPTION_ENABLED = GetAppBoolSetting("AB_Test_Option_Enabled");
			Constants.ORDER_HISTORY_DETAIL_ORDER_CANCEL_TIME = GetAppIntSetting("OrderHistoryDetail_OrderCancelTime");
			Constants.PRODUCTOPTIONVALUES_MAX_COUNT = GetAppIntSetting("ProductOptionValues_Max_Count");
			Constants.CORRESPONDENCE_SPECIFIEDCOMMERCIALTRANSACTIONS_ENABLE = GetAppBoolSetting("Correspondence_SpecifiedCommercialTransactions_Enable");

			// 決済設定
			Constants.PAYMENT_CARD_KBN = (Constants.PaymentCard?)GetAppSetting("Payment_CreditKbn", typeof(Constants.PaymentCard));
			Constants.PAYMENT_CVS_KBN = Constants.PAYMENT_CVS_KBN = (Constants.PaymentCvs?)GetAppSetting("Payment_Cvs_Type", typeof(Constants.PaymentCvs));
			Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID = GetAppStringSetting("Payment_Credit_Provisional_CreditCard_PaymentId");
			Constants.CALCULATE_PAYMENT_PRICE_ONLY_SUBTOTAL = GetAppBoolSetting("Calculate_Payment_Price_Only_Subtotal");
			Constants.CREDITCARD_ERROR_DETAILS_DISPLAY_ENABLED = GetAppBoolSetting("CreditCard_Error_Details_Display_Enabled");
			Constants.PAYMENT_CVS_DEF_INVOICE_ALL_ITEM_DISPLAYED = GetAppBoolSetting("Payment_Cvs_Def_Invoice_All_Item_Displayed");
			Constants.PAYMENT_SETTING_DOCOMOKETAI_SHOP_CODE = GetAppStringSetting("Payment_Docomo_ShopCode");//モバイル：PAYMENT_DOCOMO_SHOP_CODE
			Constants.PAYMENT_SETTING_DOCOMOKETAI_SHOP_PASSWORD = GetAppStringSetting("Payment_Docomo_ShopPassword");//モバイル：PAYMENT_DOCOMO_SHOP_PASSWORD
			Constants.PAYMENT_SETTING_SMATOMETE_CONNECT_PASSWORD = GetAppStringSetting("Payment_Softbank_AuthPassword");//モバイル：PAYMENT_SOFTBANK_AUTH_PASSWORD
			Constants.PAYMENT_SETTING_SMATOMETE_CONNECT_USER_ID = GetAppStringSetting("Payment_Softbank_AuthId");//モバイル：PAYMENT_SOFTBANK_AUTH_ID
			Constants.PAYMENT_SETTING_SMATOMETE_SHOP_ID = GetAppStringSetting("Payment_Softbank_ShopCode");//モバイル：PAYMENT_SOFTBANK_SHOP_CODE
			Constants.PAYMENT_SETTING_SBPS_API_URL = GetAppStringSetting("Payment_SBPS_ApiUrl");
			Constants.PAYMENT_SETTING_SBPS_CREDIT_GETTOKEN_JS_URL = GetAppStringSetting("Payment_SBPS_Credit_GetTokenJsUrl");
			Constants.PAYMENT_SETTING_SBPS_ORDER_LINK_URL = GetAppStringSetting("Payment_SBPS_OrderLinkUrl");
			Constants.PAYMENT_SETTING_SBPS_CARD_REGSITER_ORDER_LINK_URL = GetAppStringSetting("Payment_SBPS_CardRegister_OrderLinkUrl");
			Constants.PAYMENT_SETTING_SBPS_ORDER_LINK_URL_LOCALTEST = "";
			Constants.PAYMENT_SETTING_SBPS_CARD_REGSITER_ORDER_LINK_URL_LOCALTEST = "";
			if (this.ConfigurationSettingInfo["Payment_SBPS_OrderLinkUrl_Local"] != null)
			{
				Constants.PAYMENT_SETTING_SBPS_ORDER_LINK_URL_LOCALTEST = GetAppStringSetting("Payment_SBPS_OrderLinkUrl_Local");
				Constants.PAYMENT_SETTING_SBPS_CARD_REGSITER_ORDER_LINK_URL_LOCALTEST = GetAppStringSetting("Payment_SBPS_CardRegister_OrderLinkUrl_Local");
			}
			Constants.PAYMENT_SETTING_SBPS_MERCHANT_ID = GetAppStringSetting("Payment_SBPS_MerchantId");
			Constants.PAYMENT_SETTING_SBPS_SERVICE_ID = GetAppStringSetting("Payment_SBPS_ServiceId");
			Constants.PAYMENT_SETTING_SBPS_HASHKEY = GetAppStringSetting("Payment_SBPS_HashKey");
			Constants.PAYMENT_SETTING_SBPS_3DES_KEY = GetAppStringSetting("Payment_SBPS_3DES_Key");
			Constants.PAYMENT_SETTING_SBPS_3DES_IV = GetAppStringSetting("Payment_SBPS_3DES_IV");
			Constants.PAYMENT_SETTING_SBPS_BASIC_AUTHENTICATION_ID = GetAppStringSetting("Payment_SBPS_BasicAuthenticationId");
			Constants.PAYMENT_SETTING_SBPS_BASIC_AUTHENTICATION_PASSWORD = GetAppStringSetting("Payment_SBPS_BasicAuthenticationPassword");
			Constants.PAYMENT_SETTING_SBPS_ITEM_ID = GetAppStringSetting("Payment_SBPS_ItemId");
			Constants.PAYMENT_SETTING_SBPS_ITEM_NAME = GetAppStringSetting("Payment_SBPS_ItemName");
			Constants.PAYMENT_SETTING_SBPS_PAYMENT_STATUS_COMPLETE = GetAppBoolSetting("Payment_SBPS_PaymentStatusComplete");
			Constants.PAYMENT_SETTING_SBPS_CREDIT_SECURITYCODE = GetAppBoolSetting("Payment_SBPS_Credit_SecurityCode");
			Constants.PAYMENT_SETTING_SBPS_CREDIT_DIVIDE = GetAppBoolSetting("Payment_SBPS_Credit_Divide");
			Constants.PAYMENT_SETTING_RAKUTEN_CREDIT_DIVIDE = GetAppBoolSetting("Payment_Rakuten_Credit_Divide");
			Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD = (Constants.PaymentCreditCardPaymentMethod?)GetAppSetting("Payment_SBPS_Credit_PaymentMethod", typeof(Constants.PaymentCreditCardPaymentMethod));
			Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS = (Constants.PaymentCreditCardPaymentMethod?)GetAppSetting("Payment_SBPS_Credit_PaymentMethod_ForDigitalContents", typeof(Constants.PaymentCreditCardPaymentMethod));
			Constants.PAYMENT_SETTING_SBPS_CREDIT_3DSECURE = GetAppBoolSetting("Payment_SBPS_Credit_3DSecure");
			Constants.PAYMENT_SETTING_SBPS_CVS_PAYMENT_LIMIT_DAY = (int?)GetAppSetting("Payment_SBPS_Cvs_PaymentLimitDay", typeof(int));
			Constants.PAYMENT_SETTING_SBPS_RAKUTENIDV2_ENABLED = GetAppBoolSetting("Payment_SBPS_RakutenIDv2_Enabled");
			Constants.PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE = GetAppStringSetting("Payment_YamatoKwc_TraderCode");
			Constants.PAYMENT_SETTING_YAMATO_KWC_ACCESS_KEY = GetAppStringSetting("Payment_YamatoKwc_AccessKey");
			Constants.PAYMENT_SETTING_YAMATO_KWC_CREDIT_GETTOKEN_JS_URL = GetAppStringSetting("Payment_YamatoKwc_Credit_GetTokenJsUrl");
			Constants.PAYMENT_SETTING_YAMATO_KWC_GOODS_NAME = GetAppStringSetting("Payment_YamatoKwc_GoodsName");
			Constants.PAYMENT_SETTING_YAMATO_KWC_API_URL_TYPE = (PaymentYamatoKwcApiUrlSetting.UrlType)GetAppSetting("Payment_YamatoKwc_ApiUrlType", typeof(PaymentYamatoKwcApiUrlSetting.UrlType));
			Constants.PAYMENT_SETTING_YAMATO_KWC_CREDIT_DIVIDE = GetAppBoolSetting("Payment_YamatoKwc_Credit_Divide");
			Constants.PAYMENT_SETTING_YAMATO_KWC_CREDIT_CANCEL_ENABLED = GetAppBoolSetting("Payment_YamatoKwc_Credit_Cancel_Enabled");
			Constants.PAYMENT_SETTING_YAMATO_KWC_DUMMY_MAILADDRESS = GetAppStringSetting("Payment_YamatoKwc_DummyMailAddress");
			Constants.PAYMENT_SETTING_YAMATO_KWC_CREDIT_SECURITYCODE = GetAppBoolSetting("Payment_YamatoKwc_Credit_SecurityCode");
			Constants.PAYMENT_SETTING_YAMATO_KWC_3DSECURE = GetAppBoolSetting("Payment_YamatoKwc_Credit_3DSecure");
			Constants.PAYMENT_SETTING_ZEUS_SECURE_LINK_SERVER_URL = GetAppStringSetting("Payment_Credit_Zeus_AuthServerUrl");
			Constants.PAYMENT_SETTING_ZEUS_SECURE_API_AUTH_SERVER_URL = GetAppStringSetting("Payment_Credit_Zeus_SecureApiAuthServerUrl");
			Constants.PAYMENT_SETTING_ZEUS_USE_LINKPOINT_ENABLED = GetAppBoolSetting("Payment_Credit_Use_LinkPoint_Enabled");
			Constants.PAYMENT_SETTING_ZEUS_LINKPOINT_SERVER_URL = GetAppStringSetting("Payment_Credit_Zeus_LinkPointServerUrl");
			if (this.ConfigurationSettingInfo["Payment_Credit_Zeus_LinkPointServerUrl_Local"] != null)
			{
				Constants.PAYMENT_SETTING_ZEUS_LINKPOINT_SERVER_URL_LOCALTEST = GetAppStringSetting("Payment_Credit_Zeus_LinkPointServerUrl_Local");
			}
			Constants.PAYMENT_SETTING_ZEUS_CLIENT_IP = GetAppStringSetting("Payment_Credit_Zeus_ClientIP");
			Constants.PAYMENT_SETTING_ZEUS_CLIENT_IP_OFFLINE = GetAppStringSetting("Payment_Credit_Zeus_ClientIP_Offline");
			Constants.PAYMENT_SETTING_ZEUS_TABLET_USERAGENT_PATTERN = GetAppStringSetting("Payment_Credit_Zeus_TabletUserAgentsPattern");
			Constants.PAYMENT_SETTING_ZEUS_SECURE_API_AUTH_KEY = GetAppStringSetting("Payment_Credit_Zeus_SecureApiAuthKey");
			Constants.PAYMENT_SETTING_ZEUS_DIVIDE = GetAppBoolSetting("Payment_Credit_Zeus_Divide");
			Constants.PAYMENT_SETTING_ZEUS_SECURITYCODE = GetAppBoolSetting("Payment_Credit_Zeus_SecurityCode");
			Constants.PAYMENT_SETTING_ZEUS_3DSECURE = GetAppBoolSetting("Payment_Credit_Zeus_3DSecure");
			Constants.PAYMENT_SETTING_ZEUS_3DSECURE2 = GetAppBoolSetting("Payment_Credit_Zeus_3DSecure2");
			Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD = (Constants.PaymentCreditCardPaymentMethod?)GetAppSetting("Payment_Zeus_PaymentMethod", typeof(Constants.PaymentCreditCardPaymentMethod));
			Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD_FORDIGITALCONTENTS = (Constants.PaymentCreditCardPaymentMethod?)GetAppSetting("Payment_Zeus_PaymentMethod_ForDigitalContents", typeof(Constants.PaymentCreditCardPaymentMethod));
			Constants.PAYMENT_SETTING_GMO_AUTH_SERVER_URL = GetAppStringSetting("Payment_Credit_Gmo_AuthServerUrl");
			Constants.PAYMENT_SETTING_GMO_SITE_ID = GetAppStringSetting("Payment_Credit_Gmo_SiteId");
			Constants.PAYMENT_SETTING_GMO_SITE_PASS = GetAppStringSetting("Payment_Credit_Gmo_SitePass");
			Constants.PAYMENT_SETTING_GMO_SHOP_ID = GetAppStringSetting("Payment_Credit_Gmo_ShopId");
			Constants.PAYMENT_SETTING_GMO_SHOP_PASS = GetAppStringSetting("Payment_Credit_Gmo_ShopPass");
			Constants.PAYMENT_SETTING_GMO_PAYMENTMETHOD = (Constants.GmoCreditCardPaymentMethod?)GetAppSetting("Payment_Credit_Gmo_PaymentMethod", typeof(Constants.GmoCreditCardPaymentMethod));
			Constants.PAYMENT_SETTING_GMO_SECURITYCODE = GetAppBoolSetting("Payment_Credit_Gmo_SecurityCode");
			Constants.PAYMENT_CREDIT_GMO_GETTOKEN_JS = GetAppStringSetting("Payment_Credit_Gmo_GetToken_Js");
			Constants.PAYMENT_SETTING_GMO_CVS_AUTH_SERVER_URL = GetAppStringSetting("Payment_Cvs_Gmo_AuthServerUrl");
			Constants.PAYMENT_SETTING_GMO_CVS_SHOP_ID = GetAppStringSetting("Payment_Cvs_Gmo_ShopId");
			Constants.PAYMENT_SETTING_GMO_CVS_SHOP_PASS = GetAppStringSetting("Payment_Cvs_Gmo_ShopPass");
			Constants.PAYMENT_SETTING_GMO_CVS_PAYMENT_LIMIT_DAY = GetAppStringSetting("Payment_Cvs_Gmo_PaymentLimitDay");
			Constants.PAYMENT_SETTING_GMO_CVS_RECEIPTS_DISP_11 = GetAppStringSetting("Payment_Cvs_Gmo_ReceiptsDisp11");
			Constants.PAYMENT_SETTING_GMO_CVS_RECEIPTS_DISP_12 = GetAppStringSetting("Payment_Cvs_Gmo_ReceiptsDisp12");
			Constants.PAYMENT_SETTING_GMO_CVS_RECEIPTS_DISP_13 = GetAppStringSetting("Payment_Cvs_Gmo_ReceiptsDisp13");
			Constants.PAYMENT_SETTING_GMO_CVS_ENTRY_TRAN = GetAppStringSetting("Payment_Cvs_Gmo_EntryTran_Url");
			Constants.PAYMENT_SETTING_GMO_CVS_EXEC_TRAN = GetAppStringSetting("Payment_Cvs_Gmo_ExecTran_Url");
			Constants.PAYMENT_SETTING_GMO_CVS_CANCEL = GetAppStringSetting("Payment_Cvs_Gmo_Cancel_Url");
			Constants.MAX_NUM_REGIST_CREDITCARD = GetAppIntSetting("Setting_Max_Num_Regist_CreditCard");
			Constants.DISPLAY_ORDERSETTLEMENTPAGE_IN_SINGLE_CART_CASE = GetAppBoolSetting("Setting_DisplayOrderSettlementPageInSingleCartCase");
			Constants.PAYMENT_CVS_DEF_KBN = (Constants.PaymentCvsDef?)GetAppSetting("Payment_Cvs_Def_Type", typeof(Constants.PaymentCvsDef));
			Constants.PAYMENT_SMS_DEF_KBN = (Constants.PaymentSmsDef?)GetAppSetting("Payment_Sms_Def_Type", typeof(Constants.PaymentSmsDef));

			Constants.PAYMENT_SETTING_YAMATO_KA_API_URL = GetAppStringSetting("Payment_YamatoKa_ApiUrl");
			Constants.PAYMENT_SETTING_YAMATO_KA_TRADER_CODE = GetAppStringSetting("Payment_YamatoKa_TraderCode");
			Constants.PAYMENT_SETTING_YAMATO_KA_TRADER_PASSWORD = GetAppStringSetting("Payment_YamatoKa_TraderPass");
			Constants.PAYMENT_SETTING_YAMATO_KA_CART_CODE = GetAppStringSetting("Payment_YamatoKa_CartCode");
			Constants.PAYMENT_SETTING_YAMATO_KA_SHIPPING_TERM_PLAN = GetAppIntSetting("Payment_YamatoKa_ShippingTermPlan");
			Constants.PAYMENT_SETTING_YAMATO_KA_ITEM_NAME = GetAppStringSetting("Payment_YamatoKa_ItemName");
			Constants.PAYMENT_SETTING_YAMATO_KA_INVOICE_BUNDLE = GetAppBoolSetting("Payment_YamatoKa_InvoiceBundle");
			Constants.PAYMENT_SETTING_YAMATO_KA_DEVELOP = GetAppBoolSetting("Payment_YamatoKa_Develop");
			Constants.PAYMENT_SETTING_YAMATO_KA_SMS_USE_FIXEDPURCHASE = GetAppBoolSetting("Payment_YamatoKaSms_UseFixedPurchase");
			Constants.PAYMENT_SETTING_GMO_DEFERRED_AUTHENTICATIONID = GetAppStringSetting("Payment_Deferred_Gmo_AuthenticationId");
			Constants.PAYMENT_SETTING_GMO_DEFERRED_SHOPCODE = GetAppStringSetting("Payment_Gmo_Deferred_ShopCode");
			Constants.PAYMENT_SETTING_GMO_DEFERRED_CONNECTPASSWORD = GetAppStringSetting("Payment_Gmo_Deferred_ConnectPassword");
			Constants.PAYMENT_SETTING_GMO_DEFERRED_BASICUSERID = GetAppStringSetting("Payment_Gmo_Deferred_BasicUserId");
			Constants.PAYMENT_SETTING_GMO_DEFERRED_BASICPASSWORD = GetAppStringSetting("Payment_Gmo_Deferred_BasicPassword");
			Constants.PAYMENT_SETTING_GMO_DEFERRED_URL_ORDERREGISTER = GetAppStringSetting("Payment_Gmo_Deferred_Url_OrderRegister");
			Constants.PAYMENT_SETTING_GMO_DEFERRED_URL_GETAUTHRESULT = GetAppStringSetting("Payment_Gmo_deferred_Url_GetAuthresult");
			Constants.PAYMENT_SETTING_GMO_DEFERRED_URL_GETINVOICEPRINTDATA = GetAppStringSetting("Payment_Gmo_deferred_Url_GetInvoicePrintData");
			Constants.PAYMENT_SETTING_GMO_DEFERRED_URL_ORDERMODIFYCANCEL = GetAppStringSetting("Payment_Gmo_deferred_Url_OrderModifyCancel");
			Constants.PAYMENT_SETTING_GMO_DEFERRED_URL_GETDEFPAYMENTSTATUS = GetAppStringSetting("Payment_Gmo_deferred_Url_GetDefPaymentStatus");
			Constants.PAYMENT_SETTING_GMO_DEFERRED_URL_REDUCESALES = GetAppStringSetting("Payment_Gmo_deferred_Url_ReduceSales");
			Constants.PAYMENT_SETTING_GMO_DEFERRED_URL_SHIPMENT = GetAppStringSetting("Payment_Gmo_deferred_Url_Shipment");
			Constants.PAYMENT_SETTING_GMO_DEFERRED_URL_SHIPMENTMODIFYCANCEL = GetAppStringSetting("Payment_Gmo_deferred_Url_ShipmentModifyCancel");
			Constants.PAYMENT_SETTING_GMO_DEFERRED_PDCOMPANYCODE = GetAppStringSetting("Payment_Gmo_deferred_PdCompanyCode");
			Constants.PAYMENT_SETTING_GMO_DEFERRED_DISPLAYRETURNAPI = GetAppBoolSetting("Payment_Gmo_deferred_DisplayReturnApi");
			Constants.PAYMENT_SETTING_GMO_DEFERRED_INVOICEBUNDLE = GetAppBoolSetting("Payment_Gmo_deferred_InvoiceBundle");
			Constants.PAYMENT_SETTING_GMO_DEFERRED_ENABLE_HTTPHEADERS_POST = GetAppBoolSetting("Payment_Gmo_deferred_Enable_HttpHeaders_Post");
			Constants.PAYMENT_SETTING_CREDIT_RETURN_AUTOSALES_ENABLED = GetAppBoolSetting("Payment_Credit_Return_AutoSales_Enabled");
			Constants.FIXEDPURCHASE_MEMBER_CONDITION_INCLUDES_ORDER_PAYMENT_STATUS_COMPLETE = GetAppBoolSetting("FixedPurchaseMemberCondition_IncludesOrderPaymentStatusComplete");
			Constants.SHIPPINGPRICE_SIZE_FOR_EXPRESS = GetAppStringSetting("ShippingPrice_Size_For_Express");
			Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_API_URL = GetAppStringSetting("Payment_TriLink_AfterPay_Api_Url");
			Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_SITE_CODE = GetAppStringSetting("Payment_TriLink_AfterPay_Site_Code");
			Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_API_KEY = GetAppStringSetting("Payment_TriLink_AfterPay_Api_Key");
			Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_SECRET_KEY = GetAppStringSetting("Payment_TriLink_AfterPay_Secret_Key");
			Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_DELIVERY_COMPANY_CODE = GetAppStringSetting("Payment_TriLink_Delivery_Company_Code");
			Constants.PAYMENT_SETTING_GMO_GETDEFPAYMENTSTATUS_OPTION = GetAppBoolSetting("Payment_Gmo_GetDefPaymentStatus_Option");
			Constants.PAYMENT_SETTING_GMO_DEFERRED_URL_REISSUE = GetAppStringSetting("Payment_Gmo_deferred_Url_Reissue");
			Constants.PAYMENT_SETTING_GMO_3DSECURE = GetAppBoolSetting("Payment_Credit_Gmo_3DSecure");
			Constants.PAYMENT_SETTING_GMO_SHOP_NAME = GetAppStringSetting("Payment_Credit_Gmo_ShopName");
			Constants.PAYMENT_SETTING_GMO_TDS2_TYPE = GetAppStringSetting("Payment_Credit_Gmo_Tds2Type");

			// ベリトランス連携
			Constants.PAYMENT_CREDIT_VERITRANS4G_GETTOKEN = GetAppStringSetting("Payment_Credit_Veritrans4G_GetToken");
			Constants.PAYMENT_CREDIT_VERITRANS4G_TOKEN_API_KEY = GetAppStringSetting("Payment_Credit_Veritrans4G_Token_Api_Key");
			Constants.PAYMENT_VERITRANS4G_CREDIT_3DSECURE = GetAppBoolSetting("Payment_Veritrans4G_Credit_3DSecure");
			Constants.PAYMENT_CREDIT_VERITRANS_3DSECURE2 = GetAppBoolSetting("Payment_Credit_Veritrans_3DSecure2");
			Constants.PAYMENT_SETTING_PAYTG_MOCK_URL_FOR_VERITRANS4G = GetAppStringSetting("Payment_payTg_Mock_Url_For_Veritrans4G");
			Constants.PAYMENT_VERITRANS4G_LOG4G_CONFIG_PATH = GetAppStringSetting("Payment_Veritrans4G_Log4g_Config_Path");
			Constants.PAYMENT_VERITRANS4G_MDK_CONFIG_PATH = GetAppStringSetting("Payment_Veritrans4G_Mdk_Config_Path");
			Constants.PAYMENT_SETTING_PAYTG_DEFAULT_CARD_NUMBER = GetAppStringSetting("Payment_PayTg_Default_Card_Number");
			Constants.PAYMENT_SETTING_PAYTG_DEFAULT_EXPIRATION_MONTH = GetAppStringSetting("Payment_PayTg_Default_Expiration_Date").Split('/')[0];
			Constants.PAYMENT_SETTING_PAYTG_DEFAULT_EXPIRATION_YEAR = GetAppStringSetting("Payment_payTg_Mock_Url_For_Veritrans4G").Split('/')[1];
			Constants.PAYMENT_SETTING_CREDIT_VERITRANS4G_SECURITYCODE = GetAppBoolSetting("Payment_Setting_Credit_VeriTrans_Securitycode");
			Constants.PAYMENT_SETTING_VERITRANS4G_PAYMENTMETHOD = (Constants.VeritransCreditCardPaymentMethod?)GetAppSetting("Payment_Veritrans4G_PaymentMethod", typeof(Constants.VeritransCreditCardPaymentMethod));
			Constants.PAYMENT_SETTING_VERITRANS4G_PAYMENTMETHOD_FORDIGITALCONTENTS = (Constants.VeritransCreditCardPaymentMethod?)GetAppSetting("Payment_Veritrans4G_PaymentMethod_ForDigitalContents", typeof(Constants.VeritransCreditCardPaymentMethod));
			Constants.PAYMENT_SETTING_VERITRANS_USE_INVOICE_BUNDLE = GetAppBoolSetting("Payment_Setting_Veritrans_Use_Invoice_Bundle");
			Constants.PAYMENT_PAYPAY_VERITRANS4G_ITEMNAME = GetAppStringSetting("Payment_Paypay_Veritrans4G_ItemName");
			Constants.PAYMENT_PAYPAY_VERITRANS4G_ITEMID = GetAppStringSetting("Payment_Paypay_Veritrans4G_ItemId");

			// 後払い
			Constants.PAYMENT_SETTING_ATOBARAICOM_REGISTATION_URL = GetAppStringSetting("Payment_Atobaraicom_Regist_Url");
			Constants.PAYMENT_SETTING_ATOBARAICOM_CANCELATION_URL = GetAppStringSetting("Payment_Atobaraicom_Cancel_Url");
			Constants.PAYMENT_SETTING_ATOBARAICOM_MODIFICATION_URL = GetAppStringSetting("Payment_Atobaraicom_Modify_Url");
			Constants.PAYMENT_SETTING_ATOBARAICOM_ALL_MODIFICATION_URL = GetAppStringSetting("Payment_Atobaraicom_Modify_All_Url");
			Constants.PAYMENT_SETTING_ATOBARAICOM_ENTERPRISED = GetAppStringSetting("Payment_Atobaraicom_Enterprised");
			Constants.PAYMENT_SETTING_ATOBARAICOM_SITE = GetAppStringSetting("Payment_Atobaraicom_Site_Id");
			Constants.PAYMENT_SETTING_ATOBARAICOM_API_USER_ID = GetAppStringSetting("Payment_Atobaraicom_Api_User_Id");
			// Atobaraicom register invoice api url
			Constants.PAYMENT_ATOBARAICOM_SHIPPING_APIURL = GetAppStringSetting("Payment_Atobaraicom_Shipping_ApiUrl");
			// Atobaraicom transfer print queue Api Url
			Constants.PAYMENT_ATOBARAICOM_TRANSFER_PRINT_QUEUE_APIURL = GetAppStringSetting("Payment_Atobaraicom_Transfer_Print_Queue_ApiUrl");
			// Atobaraicom get list target invoice api url
			Constants.PAYMENT_ATOBARAICOM_GET_LIST_TARGET_INVOICE_APIURL = GetAppStringSetting("Payment_Atobaraicom_Get_List_Target_Invoice_ApiUrl");
			// Atobaraicom invoice process execute api url
			Constants.PAYMENT_ATOBARAICOM_INVOICE_PROCESS_EXECUTE_APIURL = GetAppStringSetting("Payment_Atobaraicom_Invoice_Process_Execute_ApiUrl");
			// Atobaraicom use invoice bundle service
			Constants.PAYMENT_SETTING_ATOBARAICOM_USE_INVOICE_BUNDLE_SERVICE = GetAppBoolSetting("Payment_Setting_Atobaraicom_Use_Invoice_Bundle_Service");
			// Atobaraicom get order status api url
			Constants.PAYMENT_ATOBARAICOM_GET_ORDER_STATUS_APIURL = GetAppStringSetting("Payment_Atobaraicom_Get_Order_Status_ApiUrl");
			// Atobaraicom get authorize status api url
			Constants.PAYMENT_ATOBARAICOM_GET_AUTHORIZE_STATUS_APIURL = GetAppStringSetting("Payment_Atobaraicom_Get_Authorize_Status_ApiUrl");
			// Atobaraicom max request get authorize status
			Constants.PAYMENT_ATOBARAICOM_MAX_REQUEST_GET_AUTHORIZE_STATUS = GetAppIntSetting("Payment_Atobaraicom_Max_Request_Get_Authorize_Status");
			// Atobaraicom invoice System use flag
			Constants.PAYMENT_SETTING_ATOBARAICOM_USE_INVOICE_SYSTEM_SERVICE = GetAppBoolSetting("Payment_Setting_Atobaraicom_Use_Invoice_System_Service");
			// Atobaraicom web request time out second
			Constants.PAYMENT_ATOBARAICOM_WEB_REQUEST_TIME_OUT_SECOND = GetAppIntSetting("Payment_Setting_Atobaraicom_Web_Request_Time_Out_Second");

			// Amazonペイメント周り
			Constants.AMAZON_LOGIN_OPTION_ENABLED = GetAppBoolSetting("AmazonLoginOption_Enabled");
			Constants.AMAZON_PAYMENT_OPTION_ENABLED = GetAppBoolSetting("AmazonPaymentOption_Enabled");
			Constants.AMAZON_PAYMENT_CV2_ENABLED = GetAppBoolSetting("AmazonPaymentCv2_Enabled");
			Constants.AMAZON_PAYMENT_CV2_USE_SHIPPING_AS_OWNER_ENABLED = GetAppBoolSetting("AmazonPaymentCV2_UseShippingAsOwner_Enabled");
			Constants.PAYMENT_AMAZON_SELLERID = GetAppStringSetting("Payment_Amazon_SellerId");
			Constants.PAYMENT_AMAZON_AWSACCESSKEY = GetAppStringSetting("Payment_Amazon_AwsAccessKey");
			Constants.PAYMENT_AMAZON_AWSSECRET = GetAppStringSetting("Payment_Amazon_AwsSecret");
			Constants.PAYMENT_AMAZON_CLIENTID = GetAppStringSetting("Payment_Amazon_ClientId");
			Constants.PAYMENT_AMAZON_PUBLIC_KEY_ID = GetAppStringSetting("Payment_Amazon_PublicKeyid");
			Constants.PAYMENT_AMAZON_PRIVATE_KEY = GetAppStringSetting("Payment_Amazon_PrivateKey");
			Constants.PAYMENT_AMAZON_ISSANDBOX = GetAppBoolSetting("Payment_Amazon_IsSandbox");
			Constants.PAYMENT_AMAZON_WIDGETSSCRIPT = GetAppStringSetting("Payment_Amazon_WidgetsScript");
			Constants.PAYMENT_AMAZON_PAYMENTCAPTURENOW = GetAppBoolSetting("Payment_Amazon_PaymentCaptureNow");
			Constants.PAYMENT_AMAZON_PAYMENT_RETURN_AUTOSALES_ENABLED = GetAppBoolSetting("Payment_Amazon_Payment_Return_Autosales_Enabled");
			Constants.PAYMENT_AMAZON_AUTO_PAY_USABLE_PRICE_MAX = decimal.Parse(GetAppStringSetting("Payment_Amazon_Auto_Pay_Usable_Price_Max"));
			Constants.PAYMENT_AMAZON_PAYMENTSTATUSCOMPLETE = GetAppBoolSetting("Payment_Amazon_PaymentStatusComplete");
			Constants.PAYMENT_AMAZON_STORENAME = GetAppStringSetting("Payment_Amazon_StoreName");
			Constants.PAYMENT_AMAZON_NAMEKANA1 = GetAppStringSetting("Payment_Amazon_NameKana1");
			Constants.PAYMENT_AMAZON_NAMEKANA2 = GetAppStringSetting("Payment_Amazon_NameKana2");
			Constants.PAYMENT_AMAZON_PLATFORMID = GetAppStringSetting("Payment_Amazon_PlatformId");
			Constants.PAYMENT_AMAZON_WRITE_API_RESPONSE_LOG = GetAppBoolSetting("Payment_Amazon_Write_Api_Response_Log");
			Constants.PAYMENT_AMAZON_CV2_WRITE_API_RESPONSE_LOG = GetAppBoolSetting("Payment_Amazon_CV2_Write_Api_Response_Log");
			// 請求先住所取得オプション
			Constants.AMAZONPAYMENTCV2_USEBILLINGASOWNER_ENABLED = GetAppBoolSetting("AmazonPaymentCV2_UseBillingAsOwner_Enabled");

			//Zcom
			Constants.PAYMENT_CREDIT_ZCOM_BASIC_USER_ID = GetAppStringSetting("Payment_Credit_Zcom_Basic_User_Id");
			Constants.PAYMENT_CREDIT_ZCOM_BASIC_PASSWORD = GetAppStringSetting("Payment_Credit_Zcom_Basic_Password");
			Constants.PAYMENT_CREDIT_ZCOM_APIURL_DIRECTPAYMENT = GetAppStringSetting("Payment_Credit_Zcom_ApiUrl_DirectPayment");
			Constants.PAYMENT_CREDIT_ZCOM_APIURL_CANCELPAYMENT = GetAppStringSetting("Payment_Credit_Zcom_ApiUrl_CancelPayment");
			Constants.PAYMENT_CREDIT_ZCOM_APIURL_SALESPAYMENT = GetAppStringSetting("Payment_Credit_Zcom_ApiUrl_SalesPayment");
			Constants.PAYMENT_CREDIT_ZCOM_APICONTACTCODE = GetAppStringSetting("Payment_Credit_Zcom_ApiContactCode");
			Constants.PAYMENT_CREDIT_ZCOM_APILANGID = GetAppStringSetting("Payment_Credit_Zcom_ApiLangId");
			Constants.PAYMENT_CREDIT_ZCOM_APIMISSIONCODE = GetAppStringSetting("Payment_Credit_Zcom_ApiMissionCode");
			Constants.PAYMENT_CREDIT_ZCOM_APIADDINFO1 = GetAppStringSetting("Payment_Credit_Zcom_ApiAddInfo1");
			Constants.PAYMENT_CREDIT_ZCOM_APIADDINFO1_FORDIGITALCONTENTS = GetAppStringSetting("Payment_Credit_Zcom_ApiAddInfo1_ForDigitalContents");
			Constants.PAYMENT_SETTING_CREDIT_ZCOM_SECURITYCODE = GetAppBoolSetting("Payment_Setting_Credit_Zcom_Securitycode");
			Constants.PAYMENT_SETTING_CREDIT_ZCOM_CURRENCYCODE = GetAppStringSetting("Payment_Setting_Credit_Zcom_CurrencyCode");
			Constants.PAYMENT_SETTING_CREDIT_ZCOM_CURRENCYFORMAT = GetAppStringSetting("Payment_Setting_Credit_Zcom_CurrencyFormat");
			Constants.PAYMENT_SETTING_CREDIT_ZCOM_STCODE = (Constants.ZcomStCode?)GetAppSetting("Payment_Setting_Credit_Zcom_StCode", typeof(Constants.ZcomStCode));
			Constants.PAYMENT_CREDIT_ZCOM_APIURL_CHECKAUTH = GetAppStringSetting("Payment_Credit_Zcom_ApiUrl_CheckAuth");
			Constants.PAYMENT_CREDIT_USE_ZCOM3DS_MOCK = GetAppBoolSetting("Payment_Credit_Use_Zcom3ds_Mock");
			Constants.PAYMENT_CREDIT_ZCOM_APIURL_CHECKAUTH_MOCK = GetAppStringSetting("Payment_Credit_Zcom_ApiUrl_CheckAuth_Mock");
			Constants.PAYMENT_CREDIT_ZCOM_APIURL_ACCESSAUTH_MOCK = GetAppStringSetting("Payment_Credit_Zcom_ApiUrl_AccessAuth_Mock");

			// ソニーペイメントe-SCOTT
			Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_GETTOKEN_JS_URL = GetAppStringSetting("Payment_EScott_Get_Token_Js_Url");
			Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_TENANTID = GetAppStringSetting("Payment_EScott_Tenant_Id");
			Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MASTERANDPROCESSANDRECOVER_URL = GetAppStringSetting("Payment_Credit_EScott_Master_And_Process_And_Recover_Url");
			Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_TOKENPAYMENTPROCESS_URL = GetAppStringSetting("Payment_Credit_EScott_Token_Payment_Process_Url");
			Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_TOKENPAYMENTAUTHCODE = GetAppStringSetting("Payment_Credit_EScott_Token_Payment_Auth_Code");
			Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MEMBERREGISTER_URL = GetAppStringSetting("Payment_Credit_EScott_Member_Rgister_Url");
			Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MERCHANTID = GetAppStringSetting("Payment_Credit_EScott_Merchant_Id");
			Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MERCHANTPASSWORD = GetAppStringSetting("Payment_Credit_EScott_Merchant_Password");
			Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_DIVID = bool.Parse(GetAppStringSetting("Payment_Credit_EScott_Divid"));
			Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_PAYMENTMETHOD = (Constants.PaymentCreditCardPaymentMethod?)GetAppSetting("Payment_ESCOCT_PaymentMethod", typeof(Constants.PaymentCreditCardPaymentMethod));
			Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_PAYMENTMETHOD_FORDIGITALCONTENTS = (Constants.PaymentCreditCardPaymentMethod?)GetAppSetting("Payment_ESCOCT_PaymentMethod_ForDigitalContents", typeof(Constants.PaymentCreditCardPaymentMethod));
			Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MEMBER_PASSWORD = GetAppStringSetting("Payment_Credit_EScott_Member_Password");
			Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MEMBER_DELETED_DAY_FROM = int.Parse(GetAppStringSetting("Payment_Credit_EScott_Member_Deleted_Day_From"));
			Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MEMBER_DELETED_DAY_TO = int.Parse(GetAppStringSetting("Payment_Credit_EScott_Member_Deleted_Day_To"));

			// PayTg
			Constants.PAYMENT_SETTING_PAYTG_ENABLED = GetAppBoolSetting("Payment_Setting_PayTg_Enabled");
			Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED = GetAppBoolSetting("Payment_Setting_PayTg_Mock_Enabled");
			Constants.PAYMENT_SETTING_PAYTG_BASEURL = GetAppStringSetting("Payment_PayTg_WebApi_Url");
			Constants.PAYMENT_SETTING_PAYTG_REGISTCREDITURL = GetAppStringSetting("Payment_Setting_PayTg_RegistCreditUrl");
			Constants.PAYMENT_SETTING_PAYTG_DEVICE_STATUS_CHECK_URL = GetAppStringSetting("Payment_Setting_PayTg_Device_Status_Check");

			// PayPalログイン＆決済
			Constants.PAYPAL_LOGINPAYMENT_ENABLED = GetAppBoolSetting("Payment_PaypalOption_Enabled");
			if (Constants.PAYPAL_LOGINPAYMENT_ENABLED)
			{
				var paypalEnvironmentStr = GetAppStringSetting("Payment_Paypal_Environment").ToLower();
				var braintreeEnvironment = Braintree.Environment.ParseEnvironment(paypalEnvironmentStr);
				var braintreeAccessToekn = GetAppStringSetting("Payment_Paypal_AccessToken");
				Constants.PAYMENT_PAYPAL_GATEWAY = new BraintreeGateway
				{
					Environment = braintreeEnvironment,
					AccessToken = braintreeAccessToekn,
				};
				Constants.PAYMENT_PAYPAL_GATEWAY_FOR_GET_CLIENTTOKEN = new BraintreeGateway
				{
					Environment = braintreeEnvironment,
					AccessToken = braintreeAccessToekn,
					Configuration =
					{
						Timeout = 10 * 1000, // トークン取得用は10sにタイムアウトを設定する
					},
				};
			}
			Constants.PAYPAL_PAYMENT_METHOD = (Constants.PayPalPaymentMethod)GetAppSetting("Paypal_PaymentMethod", typeof(Constants.PayPalPaymentMethod));
			Constants.PAYPAL_PAYMENT_BNCODE = GetAppStringSetting("Payment_Paypal_BnCode");

			Constants.PAYPAL_USEREXTEND_COLUMNNAME_CUSTOMER_ID = GetAppStringSetting("PayPal_UserExrtendColumnName_CustomerId");
			Constants.PAYPAL_USEREXTEND_COLUMNNAME_COOPERATION_INFOS = GetAppStringSetting("PayPal_UserExrtendColumnName_CooperationInfos");

			// 再与信有効設定
			Constants.PAYMENT_REAUTH_ENABLED = GetAppBoolSetting("Payment_Reauth_Enabled");
			Constants.PAYMENT_REAUTH_ORDER_SITE_KBN = GetAppStringSettingList("Payment_Reauth_Order_Site_Kbn");

			// スコア@払い
			Constants.PAYMENT_SCORE_AFTER_PAY_SHOP_CODE = GetAppStringSetting("Payment_ScoreAfterPay_ShopCode");
			Constants.PAYMENT_SCORE_AFTER_PAY_SHOP_PASSWORD = GetAppStringSetting("Payment_ScoreAfterPay_ShopPassword");
			Constants.PAYMENT_SCORE_AFTER_PAY_TERMINAL_ID = GetAppStringSetting("Payment_ScoreAfterPay_TerminalId");
			Constants.PAYMENT_SCORE_AFTER_PAY_URL_TRANSACTION = GetAppStringSetting("Payment_ScoreAfterPay_Url_Transaction");
			Constants.PAYMENT_SCORE_AFTER_PAY_URL_MODIFY_TRANSACTION = GetAppStringSetting("Payment_ScoreAfterPay_Url_ModifyTransaction");
			Constants.PAYMENT_SCORE_AFTER_PAY_URL_PD_REQUEST = GetAppStringSetting("Payment_ScoreAfterPay_Url_PdRequest");
			Constants.PAYMENT_SCORE_AFTER_PAY_URL_CANCEL = GetAppStringSetting("Payment_ScoreAfterPay_Url_Cancel");
			Constants.PAYMENT_SCORE_AFTER_PAY_URL_GET_AUTHOR = GetAppStringSetting("Payment_ScoreAfterPay_Url_GetAuthor");
			Constants.PAYMENT_SCORE_AFTER_PAY_URL_GET_INVOICE = GetAppStringSetting("Payment_ScoreAfterPay_Url_GetInvoice");
			Constants.PAYMENT_SETTING_SCORE_DEFERRED_USE_INVOICE_BUNDLE = GetAppBoolSetting("Payment_ScoreAfterPay_Invoice_Included");

			// ペイジェント決済連携
			Constants.PAYMENT_PAYGENT_MERCHANTID = GetAppStringSetting("Payment_Paygent_MerchantId");
			Constants.PAYMENT_PAYGENT_CONNECTID = GetAppStringSetting("Payment_Paygent_ConnectId");
			Constants.PAYMENT_PAYGENT_CONNECTIDPASSWORD = GetAppStringSetting("Payment_Paygent_ConnectIdPassword");
			Constants.PAYMENT_PAYGENT_API_VERSION = GetAppStringSetting("Payment_Paygent_Api_Version");
			Constants.PAYMENT_PAYGENT_CREDIT_GENERATE_TOKEN_KEY = GetAppStringSetting("Payment_Paygent_Credit_Generate_Token_Key");
			Constants.PAYMENT_PAYGENT_CREDIT_RECEIVE_TOKEN_HASHKEY = GetAppStringSetting("Payment_Paygent_Credit_Receive_Token_HashKey");
			Constants.PAYMENT_PAYGENT_CREDIT_RECEIVE_3DSECURERESULT_HASHKEY = GetAppStringSetting("Payment_Paygent_Credit_Receive_3DSecureResult_HashKey");
			Constants.PAYMENT_PAYGENT_CREDIT_3DSECURE = GetAppBoolSetting("Payment_Paygent_Credit_3DSecure");
			Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD = (Constants.PaygentCreditCardPaymentMethod?)GetAppSetting("Payment_Paygent_Credit_PaymentMethod", typeof(Constants.PaygentCreditCardPaymentMethod));
			Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS = (Constants.PaygentCreditCardPaymentMethod?)GetAppSetting("Payment_Paygent_Credit_PaymentMethod_ForDigitalContents", typeof(Constants.PaygentCreditCardPaymentMethod));
			Constants.PAYMENT_PAYGENT_CREDIT_AUTHORIZATION_CONTROLKBN = GetAppStringSetting("Payment_Paygent_Credit_Authorization_ControlKbn");
			Constants.PAYMENT_PAYGENT_CREDIT_GETTOKENJSURL = GetAppStringSetting("Payment_Paygent_Credit_GetTokenJsUrl");
			Constants.PAYMENT_PAYGENT_MERCHANTNAME = GetAppStringSetting("Payment_Paygent_MerchantName");
			// ペイジェント銀行ネットネットバンキング決済支払期限(日)
			Constants.PAYMENT_PAYGENT_BANKNET_PAYMENT_LIMIT_DAY = GetAppIntSetting("Payment_Paygent_Banknet_PaymentLimitDay");
			// ペイジェント銀行ネット注文内容
			Constants.PAYMENT_PAYGENT_BANKNET_ORDERDETAIL_NAME = GetAppStringSetting("Payment_Paygent_Banknet_OrderDetail_Name");
			// ペイジェント銀行ネット注文内容（カナ）
			Constants.PAYMENT_PAYGENT_BANKNET_ORDERDETAIL_NAME_KANA = GetAppStringSetting("Payment_Paygent_Banknet_OrderDetail_Name_Kana");
			// ペイジェントクレジット 差分通知ハッシュ値生成キー
			Constants.PAYMENT_PAYGENT_NOTICE_HASHKEY = GetAppStringSetting("Payment_Paygent_Notice_HashKey");
			// ペイジェントATM決済 ATM決済支払期限(日)
			Constants.PAYMENT_PAYGENT_ATM_PAYMENT_LIMIT_DAY = GetAppIntSetting("Payment_Paygent_Atm_PaymentLimitDay");
			// ペイジェントATM決済 注文内容
			Constants.PAYMENT_PAYGENT_ATM_ORDERDETAIL_NAME = GetAppStringSetting("Payment_Paygent_Atm_OrderDetail_Name");
			// ペイジェントATM決済 注文内容（カナ）
			Constants.PAYMENT_PAYGENT_ATM_ORDERDETAIL_NAME_KANA = GetAppStringSetting("Payment_Paygent_Atm_OrderDetail_Name_Kana");

			// GMOアトカラ
			Constants.PAYMENT_GMOATOKARA_ENABLED = GetAppBoolSetting("Payment_GmoAtokara_Enabled");
			Constants.PAYMENT_GMOATOKARA_DEFERRED_AUTHENTICATIONID = GetAppStringSetting("Payment_Deferred_GmoAtokara_AuthenticationId");
			Constants.PAYMENT_GMOATOKARA_DEFERRED_SHOPCODE = GetAppStringSetting("Payment_GmoAtokara_Deferred_ShopCode");
			Constants.PAYMENT_GMOATOKARA_DEFERRED_CONNECTPASSWORD = GetAppStringSetting("Payment_GmoAtokara_Deferred_ConnectPassword");
			Constants.PAYMENT_GMOATOKARA_DEFERRED_SMSAUTHENTICATIONPASSWORD = GetAppStringSetting("Payment_GmoAtokara_Deferred_SMSAuthenticationPassword");
			Constants.PAYMENT_GMOATOKARA_DEFERRED_URL_ORDERREGISTER = GetAppStringSetting("Payment_GmoAtokara_Deferred_Url_OrderRegister");
			Constants.PAYMENT_GMOATOKARA_DEFERRED_URL_GETAUTHRESULT = GetAppStringSetting("Payment_GmoAtokara_deferred_Url_GetAuthresult");
			Constants.PAYMENT_GMOATOKARA_DEFERRED_URL_ORDERMODIFYCANCEL = GetAppStringSetting("Payment_GmoAtokara_deferred_Url_OrderModifyCancel");
			Constants.PAYMENT_GMOATOKARA_DEFERRED_URL_SHIPMENT = GetAppStringSetting("Payment_GmoAtokara_deferred_Url_Shipment");
			Constants.PAYMENT_GMOATOKARA_DEFERRED_URL_SHIPMENTMODIFYCANCEL = GetAppStringSetting("Payment_GmoAtokara_deferred_Url_ShipmentModifyCancel");

			// パフォーマンス計測用
			Constants.LOGGING_PERFORMANCE_SQL_ENABLED = GetAppBoolSetting("LoggingPerformanceSql_Enabled");
			Constants.LOGGING_PERFORMANCE_REQUEST_ENABLED = GetAppBoolSetting("LoggingPerformanceRequest_Enabled");

			// ログファイル
			Constants.LOGFILE_ENCODING = GetAppStringSetting("LogFile_Encoding");

			// Option Enabled For EC, MP, CS
			Constants.EC_OPTION_ENABLED = GetAppBoolSetting("EC_Option_Enabled");
			Constants.MP_OPTION_ENABLED = GetAppBoolSetting("MP_Option_Enabled");
			Constants.CS_OPTION_ENABLED = GetAppBoolSetting("CS_Option_Enabled");
			Constants.CMS_OPTION_ENABLED = GetAppBoolSetting("Cms_Option_Enabled");

			// リターゲティング系の設定
			Constants.CRITEO_OPTION_ENABLED = GetAppBoolSetting("CriteoOption_Enabled");
			Constants.CRITEO_ACCOUNT_ID = GetAppStringSetting("Criteo_AccountId");
			Constants.CRITEO_CROSS_DEVICE_ENABLED = GetAppBoolSetting("Criteo_Cross_Device_Enabled");

			// CPM（顧客ポートフォリオ・マネジメント）設定
			Constants.CPM_OPTION_ENABLED = GetAppBoolSetting("Cpm_Option_Enabled");
			if (Constants.CPM_OPTION_ENABLED) Constants.CPM_CLUSTER_SETTINGS = new CpmClusterSettings(Path.Combine(this.ConfigDirPath, "Settings", "CpmClusterSettings.xml"));

			// グローバル対応オプション
			Constants.GLOBAL_OPTION_ENABLE = GetAppBoolSetting("Global_Option_Enabled");
			Constants.GLOBAL_CONFIGS = GlobalConfigs.GetInstance();

			// クーポンBOX設定
			Constants.COUPONBOX_DISPLAY_PASSED_DAYS_FROM_EXPIREDATE = GetAppIntSetting("CouponBox_Display_Passed_Days_From_ExpireDate");

			// ユーザー情報設定
			Constants.USER_NAME_COMPLETION_TEXT = GetAppStringSetting("User_Name_Completion_Text");

			// メール便エスカレーション個数
			// 1を設定した場合、2以上で宅配便にエスカレーションされる
			Constants.MAIL_ESCALATION_COUNT = GetAppIntSetting("Mail_Escalation_Count");

			//メール便の出荷予定日にリードタイムを考慮
			Constants.MAIL_SHIPPINGDATE_INCLUDE_LEADTIME = GetAppBoolSetting("Mail_ShippingDate_Include_Leadtime");

			// 配送方法自動判定(配送サイズ係数)：定期注文配送方法切り替わりフラグの注文拡張ステータス番号
			Constants.MAIL_ESCALATION_TO_EXPRESS_ORDEREXTENDNO = GetAppStringSetting("Mail_Escalation_To_Express_OrderExtendNo");

			//メール便配送サービスエスカレーション機能
			Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED = GetAppBoolSetting("DeliveryCompany_Mail_Escalation_Enabled");
			// メール便配送サービスエスカレーション機能：定期注文配送サービス切り替わりフラグの注文拡張ステータス番号
			Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ORDEREXTENDNO = GetAppStringSetting("DeliveryCompany_Mail_Escalation_OrderExtendNo");

			// ソーシャルプラス系の設定
			Constants.SOCIAL_LOGIN_ENABLED = GetAppBoolSetting("Social_Login_Enabled");
			Constants.SOCIAL_LOGIN_FQDN = GetAppStringSetting("Social_Login_FQDN");
			Constants.SOCIAL_LOGIN_FQDN_APPLE = GetAppStringSetting("Social_Login_FQDN_Apple");
			Constants.SOCIAL_LOGIN_API_KEY = GetAppStringSetting("Social_Login_APIKey");
			Constants.SOCIAL_LOGIN_ACCOUNT_ID = GetAppStringSetting("Social_Login_AccountID");
			Constants.SOCIAL_LOGIN_SITE_ID = GetAppStringSetting("Social_Login_SiteID");
			Constants.SOCIAL_LOGIN_WEBAPI_TIMEOUT = GetAppIntSetting("Social_Login_WebAPI_Timeout");

			// ソーシャルプラスのプロバイダID連携設定
			Constants.SOCIAL_PROVIDER_ID_FACEBOOK = GetAppStringSetting("Social_Provider_ID_Facebook");
			Constants.SOCIAL_PROVIDER_ID_LINE = GetAppStringSetting("Social_Provider_ID_Line");
			Constants.SOCIAL_PROVIDER_ID_TWITTER = GetAppStringSetting("Socail_Provider_ID_Twitter");
			Constants.SOCIAL_PROVIDER_ID_YAHOO = GetAppStringSetting("Social_Provider_ID_Yahoo");
			Constants.SOCIAL_PROVIDER_ID_GOOGLE = GetAppStringSetting("Social_Provider_ID_Google");
			Constants.SOCIAL_PROVIDER_ID_APPLE = GetAppStringSetting("Social_Provider_ID_Apple");
			Constants.SOCIAL_PROVIDER_ID_USEREXTEND_COLUMN_NAMES = new[]
			{
				Constants.SOCIAL_PROVIDER_ID_FACEBOOK,
				Constants.SOCIAL_PROVIDER_ID_LINE,
				Constants.SOCIAL_PROVIDER_ID_TWITTER,
				Constants.SOCIAL_PROVIDER_ID_YAHOO,
				Constants.SOCIAL_PROVIDER_ID_GOOGLE,
				Constants.SOCIAL_PROVIDER_ID_APPLE,
			};

			// Amazonログイン連携
			Constants.AMAZON_USER_ID_USEREXTEND_COLUMN_NAME = GetAppStringSetting("Amazon_User_Id_UserExtend_Column_Name");

			// 楽天IDConnect連携
			Constants.RAKUTEN_LOGIN_ENABLED = GetAppBoolSetting("Rakuten_Login_Enabled");
			Constants.RAKUTEN_ID_CONNECT_CLIENT_ID = GetAppStringSetting("RakutenIDConnect_ClientId");
			Constants.RAKUTEN_ID_CONNECT_CLIENT_SECRET = GetAppStringSetting("RakutenIDConnect_ClientSecret");
			Constants.RAKUTEN_ID_CONNECT_MOCK_URL = GetAppStringSetting("RakutenIDConnect_MockUrl");
			Constants.RAKUTEN_ID_CONNECT_OUTPUT_DEBUGLOG = GetAppBoolSetting("RakutenIDConnect_OutputDebugLog");
			Constants.RAKUTEN_ID_CONNECT_OPEN_ID = GetAppStringSetting("RakutenIDConnect_Open_ID");
			Constants.RAKUTEN_ID_CONNECT_REGISTER_USER = GetAppStringSetting("RakutenIDConnect_Register_User");

			// LINE直接連携
			Line.Constants.LINE_DIRECT_OPTION_ENABLED = GetAppBoolSetting("LINE_Direct_Option_Enabled");
			Line.Constants.LINE_DIRECT_CONNECT_CLIENT_ID = GetAppStringSetting("Line_Direct_Connect_Client_Id");
			Line.Constants.LINE_DIRECT_CONNECT_CLIENT_SECRET = GetAppStringSetting("Line_Direct_Connect_Client_Secret");
			Line.Constants.LINE_DIRECT_CHANNEL_ACCESS_TOKEN = GetAppStringSetting("LINE_Direct_Channel_Access_Token");
			Line.Constants.LINE_DIRECT_MAX_MESSAGE_COUNT = GetAppIntSetting("LINE_Direct_Max_Message_Count");
			Line.Constants.LINE_DIRECT_CONNECT_REQUEST_VALUE_REDIRECT_URI = GetAppStringSetting("LINE_Direct_Connect_Request_Value_Redirect_Uri");
			Line.Constants.LINE_DIRECT_AUTO_LOGIN_OPTION = GetAppBoolSetting("Line_Direct_Auto_Login_Option");
			Line.Constants.LINE_DIRECT_CONNECT_REQUEST_VALUE_BOT_PROMPT = GetAppStringSetting("Line_Direct_Connect_Request_Value_Bot_Prompt");
			Line.Constants.LINE_DIRECT_AUTO_LOGIN_USER_NAME = GetAppStringSetting("Line_Direct_Auto_Login_User_Name");

			// LINEミニアプリ
			Line.Constants.LINE_MINIAPP_OPTION_ENABLED = GetAppBoolSetting("LINE_MiniApp_Option_Enabled");
			Line.Constants.LINE_MINIAPP_LIFF_ID = GetAppStringSetting("Line_LiffId");

			// ソーシャルログイン共通
			Constants.COMMON_SOCIAL_LOGIN_ENABLED =
				(Constants.SOCIAL_LOGIN_ENABLED
				|| Constants.RAKUTEN_LOGIN_ENABLED
				|| Constants.AMAZON_LOGIN_OPTION_ENABLED
				|| Constants.PAYPAL_LOGINPAYMENT_ENABLED
				|| Line.Constants.LINE_DIRECT_OPTION_ENABLED);

			// 外部連携注文取込設定
			Constants.URERU_AD_IMPORT_ENABLED = GetAppBoolSetting("UreruAdImport_Enabled"); // つくーる
			// ウケトル連携有効無効
			Constants.UKETORU_TRACKERS_API_ENABLED = GetAppBoolSetting("UketoruTrackersApi_Enabled");
			// ウケトル連携リクエストURL
			Constants.UKETORU_TRACKERS_API_URL = GetAppStringSetting("UketoruTrackersApi_Url");
			// ウケトル連携ショップトークン
			Constants.UKETORU_TRACKERS_API_SHOP_TOKEN = GetAppStringSetting("UketoruTrackersApi_Shop_Token");

			// レコメンド設定
			Constants.RECOMMENDOPTION_APPLICABLE_PAYMENTIDS_FOR_ORDER_COMPLETE = GetAppStringSettingList("RecommendOption_ApplicablePaymentIdsForOrderComplete").ToArray();
			Constants.RECOMMENDOPTION_IS_FORCED_FIXEDPURCHASE_SETTING_BY_RECOMMEND = GetAppBoolSetting("RecommendOption_IsForcedFixedPurchaseSettingByRecommend");
			// 注文完了ページでのレコメンド商品購入時に確認画面表示するか
			Constants.ENABLES_ORDERCONFIRM_MODAL_WINDOW_ON_RECOMMENDATION_AT_ORDERCOMPLETE = GetAppBoolSetting("Enables_OrderConfirm_Modal_Window_On_Recommendation_At_OrderComplete");

			// 配送希望日時：配送希望日・のデフォルト文字列表示オプション
			Constants.DISPLAY_DEFAULTSHIPPINGDATE_ENABLED = GetAppBoolSetting("Display_DefaultShippingDate_Enabled");

			// モール連携：SFTP秘密鍵ファイルディレクトリ
			Constants.PHYSICALDIRPATH_MALLCOOPERATION_SFTPPRIVATEKEY_FILEPATH = GetAppStringSetting("Directory_MallCooperation_SftpPrivateKey_FilePath");
			// モール連携：SFTP秘密鍵ファイル名
			Constants.MALLCOOPERATION_SFTPPRIVATEKEY_FILENAME = GetAppStringSetting("MallCooperation_SftpPrivateKey_FileName");

			// 半角文字入力可否
			Constants.HALFWIDTH_CHARACTER_INPUTABLE = GetAppBoolSetting("Halfwidth_Character_Inputable");

			// 基軸通貨コード
			Constants.CONST_KEY_CURRENCY_CODE = GetAppStringSetting("Key_Currency_Code");

			// 運用拠点ISOコード
			Constants.OPERATIONAL_BASE_ISO_CODE = GetAppStringSetting("Operational_Base_Iso_Code");
			// 納品書のデフォルト言語コード
			Constants.DEFAULT_INVOICE_LANGUAGE_CODE = GetAppStringSetting("Default_Invoice_Language_Code");

			// GlobalSMS周り
			Constants.GLOBAL_SMS_OPTION_ENABLED = GetAppBoolSetting("Global_SMS_Option_Enabled");
			Constants.GLOBAL_SMS_TYPE = GetAppStringSetting("Global_SMS_Type");
			Constants.GLOBAL_SMS_STATUS_TIME_OVER_HOURS = GetAppIntSetting("Global_SMS_Status_Time_Over_Hours");
			Constants.GLOBAL_SMS_STOP_ERROR_POINT = GetAppIntSetting("Global_SMS_Stop_Error_Point");
			Constants.GLOBAL_SMS_STATUS_CLEAING_DAYS = GetAppIntSetting("Global_SMS_Status_Cleaing_Days");
			Constants.MACROKIOSK_API_URL = GetAppStringSetting("MacroKiosk_Api_Url");
			Constants.MACROKIOSK_API_USER = GetAppStringSetting("MacroKiosk_Api_User");
			Constants.MACROKIOSK_API_PASS = GetAppStringSetting("MacroKiosk_Api_Pass");
			Constants.MACROKIOSK_API_SERVID = GetAppStringSetting("MacroKiosk_Api_ServId");
			Constants.MACROKIOSK_DN_OUTPUT_DIR_PATH = GetAppStringSetting("MacroKiosk_DN_Output_Dir_Path");
			Constants.GLOBAL_SMS_FROM = GetAppStringSetting("Global_SMS_From");

			// Atodene設定
			Constants.PAYMENT_SETTING_ATODENE_SHOP_CODE = GetAppStringSetting("Payment_Setting_Atodene_Shop_Code");
			Constants.PAYMENT_SETTING_ATODENE_LINK_ID = GetAppStringSetting("Payment_Setting_Atodene_Link_Id");
			Constants.PAYMENT_SETTING_ATODENE_LINK_PASSWORD = GetAppStringSetting("Payment_Setting_Atodene_Link_Password");
			Constants.PAYMENT_SETTING_ATODENE_BASIC_USER_ID = GetAppStringSetting("Payment_Setting_Atodene_Basic_User_Id");
			Constants.PAYMENT_SETTING_ATODENE_BASIC_PASSWORD = GetAppStringSetting("Payment_Setting_Atodene_Basic_Password");
			Constants.PAYMENT_SETTING_ATODENE_URL_ORDERREGISTER = GetAppStringSetting("Payment_Setting_Atodene_Url_OrderRegister");
			Constants.PAYMENT_SETTING_ATODENE_URL_GETAUTHRESULT = GetAppStringSetting("Payment_Setting_Atodene_Url_GetAuthResult");
			Constants.PAYMENT_SETTING_ATODENE_URL_GETINVOICEPRINTDATA = GetAppStringSetting("Payment_Setting_Atodene_Url_GetInvoicePrintData");
			Constants.PAYMENT_SETTING_ATODENE_URL_ORDERMODIFYCANCEL = GetAppStringSetting("Payment_Setting_Atodene_Url_OrderModifyCancel");
			Constants.PAYMENT_SETTING_ATODENE_URL_SHIPMENT = GetAppStringSetting("Payment_Setting_Atodene_Url_Shipment");
			Constants.PAYMENT_SETTING_ATODENE_DELIVERY_COMPANY_CODE = GetAppStringSetting("Payment_Setting_Atodene_Delivery_Company_Code");
			Constants.PAYMENT_SETTING_ATODENE_DETAIL_NAME_SUBTOTAL = GetAppStringSetting("Payment_Setting_Atodene_Detail_Name_Subtotal");
			Constants.PAYMENT_SETTING_ATODENE_DETAIL_NAME_SHIPPING = GetAppStringSetting("Payment_Setting_Atodene_Detail_Name_Shipping");
			Constants.PAYMENT_SETTING_ATODENE_DETAIL_NAME_PAYMENT = GetAppStringSetting("Payment_Setting_Atodene_Detail_Name_Payment");
			Constants.PAYMENT_SETTING_ATODENE_DETAIL_NAME_DISCOUNT_ETC = GetAppStringSetting("Payment_Setting_Atodene_Detail_Name_Discount_Etc");
			Constants.PAYMENT_SETTING_ATODENE_DETAIL_NAME_PURCHASED_ITEM_ETC = GetAppStringSetting("Payment_Setting_Atodene_Detail_Name_Purchased_Item_Etc");
			Constants.PAYMENT_SETTING_ATODENE_USE_INVOICE_BUNDLE_SERVICE = GetAppBoolSetting("Payment_Setting_Atodene_Use_Invoice_Bundle_Service");
			Constants.PAYMENT_SETTING_ATODENE_ORDER_DETAIL_COOPERATION = GetAppBoolSetting("Payment_Setting_Atodene_Order_Detail_Cooperation");
			Constants.PAYMENT_SETTING_ATODENE_ORDER_DETAIL_COOPERATION_PRODUCTNAME = GetAppStringSetting("Payment_Setting_Atodene_Order_Detail_Cooperation_ProductName");
			Constants.PAYMENT_SETTING_ATODENE_ORDER_DETAIL_COOPERATION_PRODUCTID = GetAppStringSetting("Payment_Setting_Atodene_Order_Detail_Cooperation_ProductId");

			// 出荷予定日オプション
			Constants.SCHEDULED_SHIPPING_DATE_OPTION_ENABLE = GetAppBoolSetting("Scheduled_Shipping_Date_Option_Enable");
			Constants.SCHEDULED_SHIPPING_DATE_VISIBLE = GetAppBoolSetting("Scheduled_Shipping_Date_Visible");

			// 当日出荷締め時間設定
			Constants.TODAY_SHIPPABLE_DEADLINE_TIME = GetAppBoolSetting("Today_Shipping_Enable");

			// タグ機能
			Constants.TAG_OUTPUT_PRODUCT_ENV_ONLY = GetAppBoolSetting("Tag_Output_Product_Env_Only");
			Constants.TAG_TARGETPAGECHECKBOX_OPTION = GetAppBoolSetting("Tag_TargetPage_Option");

			// Webキャプチャー
			Constants.WEB_BROWSER_CAPTURE_API_URL = GetAppStringSetting("Web_browser_capture_api_url");

			// CMSのLP関連
			Constants.CMS_LANDING_PAGE_DIR_PATH_PC = GetAppStringSetting("CMS_Landing_Page_Dir_Path_PC");
			Constants.CMS_LANDING_PAGE_DIR_PATH_SP = GetAppStringSetting("CMS_Landing_Page_Dir_Path_SP");
			Constants.CMS_LANDING_PAGE_TEMPLATE_FILE_PATH_PC = GetAppStringSetting("CMS_Landing_Page_Template_File_Path_PC");
			Constants.CMS_LANDING_PAGE_TEMPLATE_FILE_PATH_SP = GetAppStringSetting("CMS_Landing_Page_Template_File_Path_SP");
			Constants.CMS_LANDING_PAGE_CUSTOM_TEMPLATE_FILE_PATH_PC = GetAppStringSetting("CMS_Landing_Page_Custom_Template_File_Path_PC");
			Constants.CMS_LANDING_PAGE_CUSTOM_TEMPLATE_FILE_PATH_SP = GetAppStringSetting("CMS_Landing_Page_Custom_Template_File_Path_SP");
			Constants.CMS_LANDING_PAGE_DIR_URL_PC = GetAppStringSetting("CMS_Landing_Page_Dir_Url_PC");
			Constants.CMS_LANDING_PAGE_DIR_URL_SP = GetAppStringSetting("CMS_Landing_Page_Dir_Url_SP");
			Constants.CMS_LANDING_PAGE_DEFAULT_BLOCK_DESIGN_FILE_PATH = GetAppStringSetting("CMS_Landing_Page_Default_Block_Design_File_Path");
			Constants.CMS_LANDING_PAGE_RECREATE_DESIGN_FILE_ON_UPDATE = GetAppBoolSetting("CMS_Landing_Page_ReCreate_Design_File_On_Update");
			Constants.CMS_LANDING_PAGE_ENABLE_MAINTENANCE_TOOL = GetAppBoolSetting("CMS_Landing_Page_Enable_Maintenance_Tool");
			Constants.CMS_LANDING_PAGE_USE_CUSTOM_DESIGN_MODE = GetAppBoolSetting("CMS_Landing_Page_Use_Custom_Design_Mode");

			// プレビュー公開範囲機能
			Constants.PREVIEW_RELEASE_RANGE_ENABLE = GetAppBoolSetting("Preview_ReleaseRange_Enabled");
			// 無効の場合は、プレビューマスタをフロントと同じマスタを使う
			Constants.PAGE_FRONT_DEFAULT_PREVIEW = Constants.PREVIEW_RELEASE_RANGE_ENABLE
				? Constants.PAGE_FRONT_DEFAULT_PREVIEW_MASTER
				: Constants.PAGE_FRONT_DEFAULT_MASTER;
			Constants.PAGE_FRONT_SIMPLE_DEFAULT_PREVIEW = Constants.PREVIEW_RELEASE_RANGE_ENABLE
				? Constants.PAGE_FRONT_SIMPLE_DEFAULT_PREVIEW_MASTER
				: Constants.PAGE_FRONT_SIMPLE_DEFAULT_MASTER;

			// 「法人（企業名、部署名）」利用オプション
			Constants.DISPLAY_CORPORATION_ENABLED = GetAppBoolSetting("Setting_Corporation_Enabled");

			// カート投入URLでの定期購入配送パターンエリア非表示設定
			Constants.HIDE_FIXEDPURCHASE_SHIPPING_PATTERN_AREA_IN_ADD_CART_URL_ENABLED = GetAppBoolSetting("Hide_FixedPurchase_Shipping_Pattern_Area_In_Add_Cart_Url_Enabled");
			// 商品詳細：SEO設定・OGP設定
			Constants.SEOTAG_AND_OGPTAG_IN_PRODUCTDETAIL_ENABLED = GetAppBoolSetting("Seotag_And_Ogptag_In_ProductDetail_Enabled");
			// 商品一覧：SEO設定
			Constants.SEOTAG_IN_PRODUCTLIST_ENABLED = GetAppBoolSetting("Seotag_In_ProductList_Enabled");
			//EFOオプション
			Constants.EFO_OPTION_PROJECT_NO = GetAppStringSetting("EFO_Option_Project_No");

			// 領収書発行オプション
			Constants.RECEIPT_OPTION_ENABLED = GetAppBoolSetting("ReceiptOption_Enabled");
			Constants.NOT_OUTPUT_RECEIPT_PAYMENT_KBN = GetAppStringSettingList("Not_Output_Receipt_Payment_Kbn");

			// 電子発票連携オプション
			Constants.TWINVOICE_ENABLED = GetAppBoolSetting("TwInvoice_Enabled");
			// OPTION：台湾電子発票API連携用統一編号
			Constants.TWINVOICE_UNIFORM_ID = GetAppStringSetting("TwInvoice_Uniform_Id");
			// OPTION：台湾電子発票API連携用アカウント
			Constants.TWINVOICE_UNIFORM_ACCOUNT = GetAppStringSetting("TwInvoice_Uniform_Account");
			// OPTION：台湾電子発票API連携用パスワード
			Constants.TWINVOICE_UNIFORM_PASSWORD = GetAppStringSetting("TwInvoice_Uniform_Password");
			// OPTION：台湾電子発票番号取得日
			Constants.TWINVOICE_GET_DATE = GetAppIntSetting("TwInvoice_Get_Date");
			// OPTION：台湾電子発票番号取得枚数
			Constants.TWINVOICE_GET_COUNT = GetAppIntSetting("TwInvoice_Get_Count");
			// Invoice Url
			Constants.TWINVOICE_URL = GetAppStringSetting("TwInvoice_Url");
			// Invoice order invoicing
			Constants.TWINVOICE_ORDER_INVOICING = GetAppBoolSetting("TwInvoice_Order_Invoicing");
			// Invoice pdf format
			Constants.TWINVOICE_PDF_FORMAT = GetAppIntSetting("TwInvoice_PDF_Format");

			// Paidy決済設定
			Constants.PAYMENT_PAIDY_OPTION_ENABLED = GetAppBoolSetting("Payment_PaidyOption_Enabled");
			Constants.PAYMENT_PAIDY_KBN = (Constants.PaymentPaidyKbn?)GetAppSetting("Payment_PaidyKbn", typeof(Constants.PaymentPaidyKbn));
			Constants.PAYMENT_PAIDY_API_KEY = GetAppStringSetting("Payment_Paidy_ApiKey");
			Constants.PAYMENT_PAIDY_SECRET_KEY = GetAppStringSetting("Payment_Paidy_SecretKey");
			Constants.PAYMENT_PAIDY_API_URL = GetAppStringSetting("Payment_Paidy_ApiUrl");
			Constants.PAYMENT_PAIDY_API_VERSION = GetAppStringSetting("Payment_Paidy_ApiVersion");
			Constants.PAYMENT_PAIDY_CHECKOUT_REQUEST_ITEMS_ID = GetAppStringSetting("Payment_Paidy_Checkout_Request_Items_Id");
			Constants.PAYMENT_PAIDY_CHECKOUT_REQUEST_ITEMS_TITLE = GetAppStringSetting("Payment_Paidy_Checkout_Request_Items_Title");
			Constants.PAYMENT_PAYGENT_PAIDY_API_KEY = GetAppStringSetting("Payment_Paygent_Paidy_ApiKey");
			Constants.PAYMENT_PAYGENT_PAIDY_SECRET_KEY = GetAppStringSetting("Payment_Paygent_Paidy_SecretKey");

			// Convenience store map: PC URL
			Constants.CONVENIENCESTOREMAP_PC_URL = GetAppStringSetting("ReceivingStore_TwPelican_CvsMap");
			// Convenience store map: SMART PHONE URL
			Constants.CONVENIENCESTOREMAP_SMARTPHONE_URL = GetAppStringSetting("ReceivingStore_TwPelican_SpCvsMap");
			// Convenience store map: cvstemp
			Constants.CONVENIENCESTOREMAP_CVSTEMP = GetAppStringSetting("ReceivingStore_TwPelican_CvsLink");
			// Convenience store map: cvsid
			Constants.RECEIVINGSTORE_TWPELICAN_CVSID = GetAppStringSetting("ReceivingStore_TwPelican_CvsId");
			// Convenience store map: cvsname
			Constants.RECEIVINGSTORE_TWPELICAN_CVSNAME = GetAppStringSetting("ReceivingStore_TwPelican_CvsName");
			// Convenience Store Limit Price
			Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITPRICE = GetAppDecimalSetting("ReceivingStore_TwPelican_CvsLimitPrice");
			// Convenience Store Limit Kg
			Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITKG = GetAppStringSetting("ReceivingStore_TwPelican_CvsLimitKg").Split(',');
			// Convenience store: Shipping Date
			Constants.RECEIVING_STORE_TWPELICAN_CVS_SHIPPING_DATE = GetAppStringSetting("ReceivingStore_TwPelican_CvsShippingDate").Split(',');
			// Service Shipping Convenience Store ID
			Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID = GetAppStringSetting("TwPelicanExpress_Convenience_Store_Id");
			// OPTION：台湾コンビニ受取
			Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED = GetAppBoolSetting("ReceivingStore_TwPelican_CvsOption_Enabled");

			// ペリカンペリカンクライアント(顧客番号,会社名)
			Constants.TWPELICANEXPRESS_CLIENT = GetAppStringSetting("TwPelicanExpress_Client");
			// 宅配通：連携用FTPホスト
			Constants.TWPELICANEXPRESS_FTP_HOST = GetAppStringSetting("TwPelicanExpress_Ftp_Host");
			// 宅配通：連携用FTPアカウント
			Constants.TWPELICANEXPRESS_FTP_ID = GetAppStringSetting("TwPelicanExpress_Ftp_Id");
			// 宅配通：連携用FTPパスワード
			Constants.TWPELICANEXPRESS_FTP_PW = GetAppStringSetting("TwPelicanExpress_Ftp_Pw");
			// 宅配通：連携用FTPポート番号
			Constants.TWPELICANEXPRESS_FTP_PORT = GetAppIntSetting("TwPelicanExpress_Ftp_Port");
			// 宅配通：Setting enable SSL
			Constants.TWPELICANEXPRESS_FTP_ENABLE_SSL = GetAppBoolSetting("TwPelicanExpress_Enable_SSL");
			// Receiving store mailsend method
			Constants.RECEIVINGSTORE_TWPELICAN_MAILSENDMETHOD = GetAppStringSetting("ReceivingStore_TwPelican_MailSendMethod");
			// Receiving store mail order extend no
			Constants.RECEIVINGSTORE_TWPELICAN_MAILORDEREXTENDNO = GetAppStringSetting("ReceivingStore_TwPelican_MailOrderExtendNo");
			// OPTION：宅配通連携拡張オプション
			Constants.TWPELICAN_COOPERATION_EXTEND_ENABLED = GetAppBoolSetting("TwPelican_Cooperation_Extend_Enabled");

			// atoneオプション（ログイン＆決済）
			Constants.PAYMENT_ATONEOPTION_ENABLED = GetAppBoolSetting("Payment_AtoneOption_Enabled");
			// Atone Apikey
			Constants.PAYMENT_ATONE_APIKEY = GetAppStringSetting("Payment_Atone_ApiKey");
			// Atone SecretKey
			Constants.PAYMENT_ATONE_SECRET_KEY = GetAppStringSetting("Payment_Atone_SecretKey");
			// Atone Api Url
			Constants.PAYMENT_ATONE_API_URL = GetAppStringSetting("Payment_Atone_ApiUrl");
			// Atone Script Url
			Constants.PAYMENT_ATONE_SCRIPT_URL = GetAppStringSetting("Payment_Atone_ScriptUrl");
			// Atone User Extend ColumnName
			Constants.ATONE_USEREXTENDCOLUMNNAME_TOKENID = GetAppStringSetting("Atone_UserExrtendColumnName_TokenId");
			// Payment Atone Terminal Id
			Constants.PAYMENT_ATONE_TERMINALID = GetAppStringSetting("Payment_Atone_TerminalId");
			// Payment Atone Temporary Registration Enabled
			Constants.PAYMENT_ATONE_TEMPORARYREGISTRATION_ENABLED = GetAppBoolSetting("Payment_Atone_TemporaryRegistration_Enabled");
			// Payment Atone Update Amount Enabled
			Constants.PAYMENT_ATONE_UPDATEAMOUNT_ENABLED = GetAppBoolSetting("Payment_Atone_UpdateAmount_Enabled");

			// afteeオプション（ログイン＆決済）
			Constants.PAYMENT_AFTEEOPTION_ENABLED = GetAppBoolSetting("Payment_AfteeOption_Enabled");
			// Aftee Apikey
			Constants.PAYMENT_AFTEE_APIKEY = GetAppStringSetting("Payment_Aftee_ApiKey");
			// Aftee SecretKey
			Constants.PAYMENT_AFTEE_SECRET_KEY = GetAppStringSetting("Payment_Aftee_SecretKey");
			// Aftee Api Url
			Constants.PAYMENT_AFTEE_API_URL = GetAppStringSetting("Payment_Aftee_ApiUrl");
			// Aftee Script Url
			Constants.PAYMENT_AFTEE_SCRIPT_URL = GetAppStringSetting("Payment_Aftee_ScriptUrl");
			// Aftee User Extend ColumnName
			Constants.AFTEE_USEREXTENDCOLUMNNAME_TOKENID = GetAppStringSetting("Aftee_UserExrtendColumnName_TokenId");
			// Payment Aftee Terminal Id
			Constants.PAYMENT_AFTEE_TERMINALID = GetAppStringSetting("Payment_Aftee_TerminalId");
			// Payment Aftee Temporary Registration Enabled
			Constants.PAYMENT_AFTEE_TEMPORARYREGISTRATION_ENABLED = GetAppBoolSetting("Payment_Aftee_TemporaryRegistration_Enabled");
			// Payment Aftee Update Amount Enabled
			Constants.PAYMENT_AFTEE_UPDATEAMOUNT_ENABLED = GetAppBoolSetting("Payment_Aftee_UpdateAmount_Enabled");
			// Card Tran Id Option Enabled
			Constants.CARDTRANIDOPTION_ENABLED = GetAppBoolSetting("CardTranIdOption_Enabled");

			// LINE Payオプション（決済）
			Constants.PAYMENT_LINEPAY_OPTION_ENABLED = GetAppBoolSetting("Payment_LinePay_Option_Enabled");
			// 決済：LINE Pay：LINE Payと通信を行う際のチャンネルID
			Constants.PAYMENT_LINEPAY_CHANNEL_ID = GetAppStringSetting("Payment_LinePay_Channel_ID");
			// 決済：LINE Pay：LINE Payと通信を行う際のシークレットキー
			Constants.PAYMENT_LINEPAY_SECRET_KEY = GetAppStringSetting("Payment_LinePay_Secret_Key");
			// 決済：LINE Pay：API接続URL
			Constants.PAYMENT_LINEPAY_API_URL = GetAppStringSetting("Payment_LinePay_Api_Url");
			// 決済：LINE Pay：加盟店の国家（JP/TW/TH）
			Constants.PAYMENT_LINEPAY_MERCHANT_COUNTRY = GetAppStringSetting("Payment_LinePay_Merchant_Country");
			// LINE Pay 自動決済キー格納用ユーザー拡張項目名(例：usrex_linepay_reg_key)
			Constants.LINEPAY_USEREXRTEND_COLUMNNAME_REGKEY = GetAppStringSetting("LinePay_UserExrtend_ColumnName_RegKey");
			// 決済：LINE Pay：即時売上かどうか TRUE:即時売上 FALSE:仮売上げ ※LINE Pay加盟店の国家（JP/TW/TH）がJP以外の場合はTRUEに設定
			Constants.PAYMENT_LINEPAY_PAYMENTCAPTURENOW = GetAppBoolSetting("Payment_LinePay_PaymentCaptureNow");
			// 決済：LINE Pay：決済連携後に入金済みにするかどうか
			Constants.PAYMENT_LINEPAY_PAYMENTSTATUSCOMPLETE = GetAppBoolSetting("Payment_LinePay_PaymentStatusComplete");

			// 決済：Logstash連携：API接続URL
			Constants.LOGSTASH_API_URL = GetAppStringSetting("Logstash_Api_Url");

			// NP後払いオプション（決済）
			Constants.PAYMENT_NP_AFTERPAY_OPTION_ENABLED = GetAppBoolSetting("Payment_NpAfterPayOption_Enabled");
			// NP後払い：加盟店コード
			Constants.PAYMENT_NP_AFTERPAY_MERCHANTCODE = GetAppStringSetting("Payment_NpAfterPay_MerchantCode");
			// NP後払い：SPコード
			Constants.PAYMENT_NP_AFTERPAY_SPCODE = GetAppStringSetting("Payment_NpAfterPay_SpCode");
			// NP後払い：ターミナルID
			Constants.PAYMENT_NP_AFTERPAY_TERMINALID = GetAppStringSetting("Payment_NpAfterPay_TerminalId");
			// NP後払い：API接続URL
			Constants.PAYMENT_NP_AFTERPAY_APIURL = GetAppStringSetting("Payment_NpAfterPay_ApiUrl");
			// 決済：NP後払い：請求書同梱
			Constants.PAYMENT_NP_AFTERPAY_INVOICEBUNDLE = GetAppBoolSetting("Payment_NpAfterPay_InvoiceBundle");
			// 決済：NP後払い：マニュアルサイト
			Constants.PAYMENT_NP_AFTERPAY_MANUALSITE = GetAppStringSetting("Payment_NpAfterPay_ManualSite");

			// 定期２回目以降商品設定機能のオプション
			Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED = GetAppBoolSetting("FixedPurchase_NextShipping_Option_Enabled");

			// JAFログイン連携
			Constants.JAF_SERVICE_ID = GetAppStringSetting("Jaf_Service_Id");
			Constants.JAF_RANK_ID = GetAppStringSetting("Jaf_Rank_Id");
			Constants.JAF_LOGIN_API_URL = GetAppStringSetting("Jaf_Login_Api_Url");
			Constants.JAF_REGISTER_API_URL = GetAppStringSetting("Jaf_Register_Api_Url");
			Constants.JAF_MEMBER_API_URL = GetAppStringSetting("Jaf_Member_Api_Url");
			Constants.JAF_RETURN_URL_FROM_SSO = GetAppStringSetting("Jaf_Return_Url_From_Sso");
			Constants.JAF_ENCRYPTKNNO_USEREXTEND_COLUMN_NAME = GetAppStringSetting("Jaf_EncryptKnNo_UserExtend_Column_Name");
			Constants.JAF_STATUS_USEREXTEND_COLUMN_NAME = GetAppStringSetting("Jaf_Status_UserExtend_Column_Name");
			Constants.JAF_ERROR_USEREXTEND_COLUMN_NAME = GetAppStringSetting("Jaf_Error_UserExtend_Column_Name");

			// ECPayペイメント：共通：ECPayと通信を行う際の加盟店コード
			Constants.PAYMENT_ECPAY_MERCHANTID = GetAppStringSetting("Payment_EcPay_MerchantID");
			// ECPayペイメント：共通：特約加盟店の場合
			Constants.PAYMENT_ECPAY_SPECIAL_MERCHANTFLAG = GetAppBoolSetting("Payment_EcPay_Special_MerchantFlag");
			// ECPayペイメント：共通：暗号化ハッシュキー
			Constants.PAYMENT_ECPAY_HASHKEY = GetAppStringSetting("Payment_EcPay_HashKey").Split(',');
			// ECPayペイメントオプション（コンビニ受取）
			Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED = GetAppBoolSetting("ReceivingStore_TwEcPay_CvsOption_Enabled");
			// ECPayペイメント：コンビニ受取：商品発送元名前(会社名ではない)
			Constants.RECEIVINGSTORE_TWECPAY_SENDNAME = GetAppStringSetting("ReceivingStore_TwEcPay_SendName").Split(',');
			// API接続URL：ECPayコンビニ受取本番環境
			Constants.RECEIVINGSTORE_TWECPAY_APIURL = GetAppStringSetting("ReceivingStore_TwEcPay_ApiUrl");
			// テスト用ドメイン、初期値：（ブランク、使用しない）
			Constants.WEBHOOK_DOMAIN = GetAppStringSetting("Webhook_Domain");
			// Shipping service yamato for EcPay
			Constants.SHIPPING_SERVICE_YAMATO_FOR_ECPAY = GetAppStringSetting("Shipping_Service_Yamato_For_EcPay").Split(',');
			// Shipping service home delivery for EcPay
			Constants.SHIPPING_SERVICE_HOME_DELIVERY_FOR_ECPAY = GetAppStringSetting("Shipping_Service_Home_Delivery_For_EcPay");
			// ECPayペイメント：コンビニ受取：コンビニ物流方法
			Constants.RECEIVINGSTORE_TWECPAY_CVS_LOGISTIC_METHOD = GetAppStringSetting("ReceivingStore_TwEcPay_CvsLogisticMethod");
			// Ec Pay
			Constants.ECPAY_PAYMENT_OPTION_ENABLED = GetAppBoolSetting("Payment_EcPay_Enabled");
			Constants.ECPAY_PAYMENT_API_URL = GetAppStringSetting("Payment_EcPay_ApiUrl");
			Constants.ECPAY_PAYMENT_CREDIT_INSTALLMENT = GetAppStringSetting("Payment_EcPay_CreditInstallment");

			// Secret Key API Line
			Constants.SECRET_KEY_API_LINE = GetAppStringSetting("SecretKey_API_Line");
			// LINE APIの認証キー試行回数
			Constants.POSSIBLE_LINE_AUTH_KEY_ERROR_COUNT = GetAppIntSetting("Possible_Line_Auth_Key_Error_Count");
			// LINE APIの認証キーロックの有効時間(分単位) 
			Constants.POSSIBLE_LINE_AUTH_KEY_LOCK_MINUTES = GetAppIntSetting("Possible_Line_Auth_Key_Lock_Minutes");

			// LINE連携URL
			w2.App.Common.Line.Constants.LINE_API_URL_ROOT_PATH = GetAppStringSetting("LINE_Api_Url_Root_Path");
			// LINE認証キー
			w2.App.Common.Line.Constants.LINE_API_AUTHENTICATION_KEY = GetAppStringSetting("LINE_Api_Authentication_Key");
			// LINEログレベル
			w2.App.Common.Line.Constants.LINE_API_LOG_LEVEL = GetAppStringSetting("LINE_Api_log_level");
			// ログファイル出力先フォルダパス
			w2.App.Common.Line.Constants.DIRECTORY_LINE_API_LOGFILEPATH = GetAppStringSetting("Directory_LINE_Api_LogFilePath");
			// LINE連携：テンプレートID
			Line.Constants.LINE_API_TEMPLATE_ID = GetAppIntSetting("LINE_Api_Template_Id");

			// LINE連携
			Constants.LINE_COOPERATION_OPTION_ENABLED = GetAppBoolSetting("LINE_Cooperation_Option_Enabled");
			// リピートライン
			Constants.REPEATLINE_OPTION_ENABLED = GetAppRepeatLineOptionSetting();

			// リピートプラスONE設定
			Constants.REPEATPLUSONE_OPTION_ENABLED = GetAppBoolSetting("RepeatPlusOne_Option_Enabled");
			Constants.REPEATPLUSONE_CONFIGS = RepeatPlusOneConfigs.GetInstance();
			Constants.LPBUILDER_MAXCOUNT = GetAppIntOrNullSetting("LpBuilder_MaxCount");
			Constants.REPEATPLUSONE_REDIRECT_PAGE = GetAppStringSetting("RepeatPlusOne_Redirect_Page");

			// その他オプション設定
			Constants.USERPRODUCTARRIVALMAIL_OPTION_ENABLED = GetAppBoolSetting("UserProductArrivalMailOption_Enabled");
			Constants.COORDINATE_WITH_STAFF_OPTION_ENABLED = GetAppBoolSetting("CoordinateWithStaffOption_Enabled");
			Constants.PRODUCTRANKING_OPTION_ENABLED = GetAppBoolSetting("ProductRankingOption_Enabled");
			Constants.PRODUCTLISTDISPSETTING_OPTION_ENABLED = GetAppBoolSetting("ProductListDispSettingOption_Enabled");
			Constants.FEATUREAREASETTING_OPTION_ENABLED = GetAppBoolSetting("FeatureAreaSettingOption_Enabled");
			Constants.FEATUREPAGESETTING_OPTION_ENABLED = GetAppBoolSetting("FeaturePageSettingOption_Enabled");
			Constants.NEWSLISTDISPSETTING_OPTION_ENABLED = GetAppBoolSetting("NewsListDispSettingOption_Enabled");
			Constants.SEOTAGDISPSETTING_OPTION_ENABLED = GetAppBoolSetting("SeoTagDispSettingOption_Enabled");
			Constants.USERMANAGEMENTLEVELSETTING_OPTION_ENABLED = GetAppBoolSetting("UserManagementLevelSettingOption_Enabled");
			Constants.USEREAZYREGISTERSETTING_OPTION_ENABLED = GetAppBoolSetting("UserEazyRegisterSettingOption_Enabled");
			Constants.ORDEREXTENDSTATUSSETTING_OPTION_ENABLED = GetAppBoolSetting("OrderExtendStatusSettingOption_Enabled");
			Constants.ORDERWORKFLOW_OPTION_ENABLED = GetAppBoolSetting("OrderWorkflowOption_Enabled");
			Constants.USEREXTENDSETTING_OPTION_ENABLED = GetAppBoolSetting("UserExtendSettingOption_Enabled");
			Constants.MARKETINGPLANNER_TARGETLIST_OPTION_ENABLE = GetAppBoolSetting("TargetListOption_Enabled");
			Constants.PRODUCT_STOCK_OPTION_ENABLE = GetAppBoolSetting("ProductStockOption_Enabled");
			Constants.SHORTURL_OPTION_ENABLE = GetAppBoolSetting("ShorturlOption_Enabled");
			Constants.PRODUCT_CTEGORY_OPTION_ENABLE = GetAppBoolSetting("ProductCtegoryOption_Enabled");
			Constants.PRODUCT_TAG_OPTION_ENABLE = GetAppBoolSetting("ProductTagOption_Enabled");
			Constants.PRODUCT_SUBIMAGE_SETTING_OPTION_ENABLE = GetAppBoolSetting("ProductSubImageSettingOption_Enabled");
			Constants.TWOCLICK_OPTION_ENABLE = GetAppBoolSetting("TwoclickOption_Enabled");
			Constants.DEMAND_OPTION_ENABLE = GetAppBoolSetting("DemandOption_Enabled");
			Constants.FIXED_PURCHASE_DISCOUNT_PRICE_OPTION_ENABLE = GetAppBoolSetting("FixedPurchaseDiscountOption_Enabled");
			Constants.USER_ATTRIBUTE_OPTION_ENABLE = GetAppBoolSetting("UserAttributeOption_Enabled");
			Constants.STOCKALERTMAIL_OPTION_ENABLED = GetAppBoolSetting("StockAlertMailOption_Enabled");
			Constants.STOCK_ALERT_MAIL_THRESHOLD = GetAppIntSetting("StockAlertMailOption_Threshold");
			Constants.FAVORITE_PRODUCT_DECREASE_MAILSENDFLG_USEREXRTEND_COLUMNNAME = GetAppStringSetting("Favorite_Product_Decrease_Mail_Send_Flg_UserExtend_Column_Name");

			// Googleショッピング利用有無
			Constants.GOOGLESHOPPING_COOPERATION_OPTION_ENABLED = GetAppBoolSetting("GoogleShoppingOption_Enabled");

			// Cart list landing page option
			Constants.CART_LIST_LP_OPTION = GetAppBoolSetting("CartList_Lp_Option");

			// Taiwan country shipping enable
			Constants.TW_COUNTRY_SHIPPING_ENABLE = (Constants.GLOBAL_OPTION_ENABLE
				&& (Constants.OPERATIONAL_BASE_ISO_CODE == Constants.COUNTRY_ISO_CODE_TW));

			// Botchan option
			Constants.BOTCHAN_OPTION = GetAppBoolSetting("Botchan_Option");
			// Secret key api Botchan
			Constants.SECRET_KEY_API_BOTCHAN = GetAppStringSetting("SecretKey_API_Botchan");
			// Allowed ip address for BotChan
			Constants.ALLOWED_IP_ADDRESS_FOR_BOTCHAN = GetAppStringSetting("Allowed_IP_Address_For_BotChan");
			// Possible Botchan Login Error Count
			Constants.POSSIBLE_BOTCHAN_LOGIN_ERROR_COUNT = GetAppIntSetting("Possible_Botchan_Login_Error_Count");
			// Possible Botchan Login Lock Minutes
			Constants.POSSIBLE_BOTCHAN_LOGIN_LOCK_MINUTES = GetAppIntSetting("Possible_Botchan_Login_Lock_Minutes");
			// BOTCHAN ログファイル出力先フォルダパス
			Constants.DIRECTORY_BOTCHAN_API_LOGFILEPATH = GetAppStringSetting("Directory_Botchan_Api_LogFilePath");
			// BOTCHAN APIログファイル サイズ閾値（MB）
			Constants.BOTCHAN_API_LOGFILE_THRESHOLD = GetAppIntSetting("Botchan_Api_logfile_threshold");
			// BOTCHAN連携クレカ登録名最長文字数
			Constants.BOTCHAN_API_CREDIT_NAME_MAX_LENGTH = GetAppIntSetting("Botchan_Api_Credit_Name_Max_Length");
			// BOTCHAN RequestParamとResponseをログに出力するかどうか
			Constants.BOTCHAN_OUTPUT_REQUEST_PARAM_AMD_RESPONSE_TO_THE_LOG_ENABLED = GetAppBoolSetting("Botchan_OutputRequestParamAndResponseToTheLog_Enabled");

			// Gooddeal連携オプション
			Constants.TWSHIPPING_GOODDEAL_OPTION_ENABLED = GetAppBoolSetting("TwShipping_Gooddeal_Enabled");
			// Gooddeal連携EshopId
			Constants.TWSHIPPING_GOODDEAL_ESHOP_ID = GetAppStringSetting("TwShipping_Gooddeal_Eshop_Id");
			// Gooddeal連携CorporationId
			Constants.TWSHIPPING_GOODDEAL_CORPORATION_ID = GetAppStringSetting("TwShipping_Gooddeal_Corporation_Id");
			// Gooddeal連携越境決済区分（カンマ区切り）
			Constants.TwShipping_Gooddeal_Post_PayType = GetAppStringSetting("TwShipping_Gooddeal_Post_PayType").Split(',');
			// Gooddeal連携ApiKey
			Constants.TWSHIPPING_GOODDEAL_APIKEY = GetAppStringSetting("TwShipping_Gooddeal_ApiKey");
			// Gooddeal連携HashKey
			Constants.TWSHIPPING_GOODDEAL_HASHKEY = GetAppStringSetting("TwShipping_Gooddeal_HashKey");
			// Gooddeal受注出荷連携API
			Constants.TWSHIPPING_GOODDEAL_DELIVER_APIURL = GetAppStringSetting("TwShipping_Gooddeal_Deliver_ApiUrl");
			// Gooddeal受注キャンセル連携API
			Constants.TWSHIPPING_GOODDEAL_CANCEL_APIURL = GetAppStringSetting("TwShipping_Gooddeal_Cancel_ApiUrl");
			// Gooddeal配送伝票番号取得API
			Constants.TWSHIPPING_GOODDEAL_GET_SHIPPINGNO_APIURL = GetAppStringSetting("TwShipping_Gooddeal_Get_ShippingNo_ApiUrl");

			// ECPayオプション（電子発票)
			Constants.TWINVOICE_ECPAY_ENABLED = GetAppBoolSetting("TwInvoice_EcPay_Enabled");
			// ハッシュキー：ECPay
			Constants.TWINVOICE_ECPAY_HASH_KEY = GetAppStringSetting("TwInvoice_EcPay_HashKey");
			// ハッシュIV：ECPay
			Constants.TWINVOICE_ECPAY_HASH_IV = GetAppStringSetting("TwInvoice_EcPay_HashIV");
			// ECPay電子発票：API接続URL
			Constants.TWINVOICE_ECPAY_API_URL = GetAppStringSetting("TwInvoice_EcPay_ApiUrl");
			// 電子発票： ECPay連携バージョン
			Constants.TWINVOICE_ECPAY_VISION = GetAppStringSetting("TwInvoice_EcPay_Vision");
			// ECPay電子発票字軌種別
			Constants.TWINVOICE_ECPAY_INV_TYPE = GetAppStringSetting("TwInvoice_EcPay_InvType");
			// ECPay電子発票税別
			Constants.TWINVOICE_ECPAY_TAX_TYPE = GetAppIntSetting("TwInvoice_EcPay_TaxType");
			// ECPay電子発票通関方法
			Constants.TWINVOICE_ECPAY_CLEARANCE_MARK = GetAppStringSetting("TwInvoice_EcPay_ClearanceMark");
			// ECPay電子発票商品単位
			Constants.TWINVOICE_ECPAY_ITEM_WORD = GetAppStringSetting("TwInvoice_EcPay_ItemWord");
			// ECPay電子発票払い戻し時消費者に通知方法
			Constants.TWINVOICE_ECPAY_ALLOWANCE_NOTIFY = GetAppStringSetting("TwInvoice_EcPay_AllowanceNotify");

			// 定期：マイページから変更可能な決済種別IDの追加設定（カンマ区切り。空白で非利用となる。）
			Constants.SETTING_PARTICULAR_USABLE_FIXEDPURCHASE_PAYMENT_IDS_WHEN_CHANGE_ADDITIONAL_SETTING
				= GetAppStringSetting("Setting_Particular_UsableFixedPurchasePaymentIdsWhenChangeAdditionalSetting").Split(',');
			// 通常：マイページから変更可能な決済種別IDの追加設定（カンマ区切り。空白で非利用となる。）
			Constants.SETTING_PARTICULAR_USABLE_ORDER_PAYMENT_IDS_WHEN_CHANGE_ADDITIONAL_SETTING
				= GetAppStringSetting("Setting_Particular_UsableOrderPaymentIdsWhenChangeAdditionalSetting").Split(',');
			// 定期：マイページから変更可能な決済種別IDを絞り込む（カンマ区切り。Setting_CanFixpurchasePaymentIds内から選択する。空白で全て対象となる。）
			Constants.SETTING_CAN_CHANGE_FIXEDPURCHASE_PAYMENT_IDS =
				GetAppStringSetting("Setting_CanChangeFixedPurchasePaymentIds").Split(',').Intersect(Constants.CAN_FIXEDPURCHASE_PAYMENTIDS).ToArray();
			// 定期：マイページから変更可能な決済種別に優先度を付ける（TRUEの場合、変更前の決済種別より優先度の高い決済種別が表示される）
			Constants.FIXEDPURCHASE_PAYMENT_IDS_PRIORITY_OPTION_ENABLED = GetAppBoolSetting("FixedPurchasePaymentIdsPriorityOption_Enabled");
			// 通常：マイページから変更可能な決済種別IDを絞り込む（カンマ区切り。空白で全て対象となる）
			Constants.SETTING_CAN_CHANGE_ORDER_PAYMENT_IDS = GetAppStringSetting("Setting_CanChangeOrderPaymentIds").Split(',');
			// 通常：マイページから変更可能な決済種別に優先度を付ける（TRUEの場合、変更前の決済種別より優先度の高い決済種別が表示される）
			Constants.ORDER_PAYMENT_IDS_PRIORITY_OPTION_ENABLED = GetAppBoolSetting("OrderPaymentIdsPriorityOption_Enabled");
			// 定期注文：定期注文の初回購入価格および回数割引の適用方法（FIXEDPURCHASE_PRODUCT_COUNT：定期商品購入回数、FIXEDPURCHASE_COUNT：定期購入回数）
			Constants.FIXEDPURCHASE_ORDER_DISCOUNT_METHOD = GetAppStringSetting("FixedPurchase_Order_Discount_Method");

			// 決済：藍新Payペイメント：藍新Payオプション
			Constants.NEWEBPAY_PAYMENT_OPTION_ENABLED = GetAppBoolSetting("Payment_NewebPay_Enabled");
			// 決済：藍新Payペイメント：藍新Payと通信を行う際の加盟店コード
			Constants.NEWEBPAY_PAYMENT_MERCHANTID = GetAppStringSetting("Payment_NewebPay_MerchantID");
			// 決済：藍新Payペイメント：暗号化ハッシュキー
			Constants.NEWEBPAY_PAYMENT_HASHKEY = GetAppStringSetting("Payment_NewebPay_HashKey");
			// 決済：藍新Payペイメント：暗号化ハッシュIV
			Constants.NEWEBPAY_PAYMENT_HASHIV = GetAppStringSetting("Payment_NewebPay_HashIV");
			// API接続URL：藍新Payテスト環境
			Constants.NEWEBPAY_PAYMENT_APIURL = GetAppStringSetting("Payment_NewebPay_ApiUrl");
			// 決済：藍新Payペイメント：藍新Payにログイン必須フラグ(1:必須、0:必要ない)初期値：0
			Constants.NEWEBPAY_PAYMENT_LOGINTYPE = GetAppIntSetting("Payment_NewebPay_LoginType");
			// 決済：藍新pay：商品名(MAX 50文字)
			Constants.NEWEBPAY_PAYMENT_ITEMNAME = GetAppStringSetting("Payment_NewebPay_ItemName");

			// NextEngineオプション
			Constants.NE_OPTION_ENABLED = GetAppBoolSetting("NextEngine_Enabled");
			// NextEngineクライアントID
			Constants.NE_CLIENT_ID = GetAppStringSetting("NextEngine_Client_Id");
			// NextEngineクライアントシークレット
			Constants.NE_CLIENT_SECRET = GetAppStringSetting("NextEngine_Client_Secret");
			// NextEngineショップID
			Constants.NE_SHOP_ID = GetAppStringSetting("NextEngine_Shop_Id");
			// NextEngine備考欄連携項目
			Constants.NE_REMARKS_ENABLED_ITEM = GetAppStringSetting("NextEngine_Remarks_Enabled_Item");
			// NextEngineログにuploadOrderDataを出力するか
			Constants.NE_WRITE_UPLOAD_ORDER_DATA_TO_LOG = GetAppBoolSetting("NextEngine_Write_UploadOrderData_To_Log");
			// 仮注文作成・キャンセル時にネクストエンジン連携するかどうか 
			Constants.NE_REALATION_TEMP_ORDER = GetAppBoolSetting("NextEngine_Relation_Temp_Order");
			// ネクストエンジンキャンセル連携オプション
			Constants.NE_COOPERATION_CANCEL_ENABLED = GetAppBoolSetting("NextEngine_Cooperation_Cancel_Enabled");
			// ネクストエンジン注文同梱時連携オプション
			Constants.NE_COOPERATION_ORDERCOMBINE_ENABLED = GetAppBoolSetting("NextEngine_Cooperation_OrderCombine_Enabled");

			// イーロジット連携オプション
			Constants.ELOGIT_WMS_ENABLED = GetAppBoolSetting("ELogit_WMS_Enabled");
			// イーロジットアップロード連携ApiKey
			Constants.ELOGIT_WMS_UPLOAD_APIKEY = GetAppStringSetting("ELogit_WMS_Upload_ApiKey");
			// イーロジットダウンロード連携ApiKey
			Constants.ELOGIT_WMS_DOWNLOAD_APIKEY = GetAppStringSetting("ELogit_WMS_Download_ApiKey");
			// イーロジット荷主コード
			Constants.ELOGIT_WMS_CODE = GetAppStringSetting("ELogit_WMS_Code");
			// イーロジットユーザーID
			Constants.ELOGIT_WMS_USER_ID = GetAppStringSetting("ELogit_WMS_User_Id");
			// イーロジットパスワード
			Constants.ELOGIT_WMS_PASSWORD = GetAppStringSetting("ELogit_WMS_Password");
			// Execution EXE: Wms shipping batch
			Constants.PHYSICALDIRPATH_WMSSHIPPING_EXE = GetAppStringSetting("Program_WmsShipping");
			/// <summary>Add advc to request parameter option enabled</summary>
			Constants.ADD_ADVC_TO_REQUEST_PARAMETER_OPTION_ENABLED = GetAppBoolSetting("Add_Advc_To_RequestParameter_Option_Enabled");

			// 定期売上予測集計オプション
			Constants.FIXED_PURCHASE_FORECAST_REPORT_OPTION = GetAppBoolSetting("FixedPurchaseForecastReportOption_Enabled");

			// 注文拡張項目:注文拡張項目オプション
			Constants.ORDER_EXTEND_OPTION_ENABLED = GetAppBoolSetting("Order_Extend_Option_Enabled");
			// 注文拡張項目:メールテンプレート チェックボックス形式 区切り文字
			Constants.ORDER_EXTEND_MAIL_CHECKBOX_SEPARATOR = GetAppStringSetting("Order_Extend_Mail_Checkbox_Separator");

			// 頒布会オプション
			Constants.SUBSCRIPTION_BOX_OPTION_ENABLED = GetAppBoolSetting("Subscription_Box_Option_Enabled");

			// Rakuten Credit Environment
			var env = GetAppStringSetting("Payment_RakutenCredit_Environment");
			// GetToken Js Url 
			Constants.PAYMENT_RAKUTEN_CREDIT_GET_TOKEN_JS_URL = GetAppStringSetting("Payment_RakutenCredit_GetTokenJsUrl_" + env);
			// Rakuten API URL
			Constants.PAYMENT_RAKUTEN_API_URL = GetAppStringSetting("Payment_Rakuten_ApiHost_" + env);
			// Rakuten service id
			Constants.PAYMENT_RAKUTEN_CREDIT_SERVICE_ID = GetAppStringSetting("Payment_RakutenCredit_ServiceId");
			// Rakuten security code flag
			Constants.PAYMENT_SETTING_CREDIT_RAKUTEN_SECURITYCODE = GetAppBoolSetting("Payment_Setting_Credit_Rakuten_Securitycode");
			// Rakuten signature key
			Constants.PAYMENT_RAKUTEN_CREDIT_SIGNATURE_KEY = GetAppStringSetting("Payment_RakutenCredit_SignatureKey");
			// Rakuten payment method
			Constants.PAYMENT_RAKUTEN_CREDIT_PAYMENT_METHOD = (Constants.RakutenPaymentType)GetAppSetting("Payment_RakutenCredit_PaymentMethod", typeof(Constants.RakutenPaymentType));
			// Rakuten 3Dセキュア利用フラグ
			Constants.PAYMENT_SETTING_RAKUTEN_3DSECURE = GetAppBoolSetting("Payment_RakutenCredit_3DSecure");
			// Rakuten 3Dセキュア利用フラグ
			Constants.PAYMENT_SETTING_RAKUTEN_3DSECURE_MERCHANT_ID_VISA = GetAppStringSetting("Payment_RakutenCredit_3DSecure_Merchant_id_Visa");
			// Rakuten 3Dセキュア利用フラグ
			Constants.PAYMENT_SETTING_RAKUTEN_3DSECURE_MERCHANT_ID_MASTERCARD = GetAppStringSetting("Payment_RakutenCredit_3DSecure_Merchant_id_MasterCard");
			// Rakuten 3Dセキュア利用フラグ
			Constants.PAYMENT_SETTING_RAKUTEN_3DSECURE_MERCHANT_NAME = GetAppStringSetting("Payment_RakutenCredit_3DSecure_Merchant_Name");

			// DSK後払い：加盟店ID
			Constants.PAYMENT_SETTING_DSK_DEFERRED_SHOPCODE = GetAppStringSetting("Payment_Dsk_Deferred_ParticipatingStoreId");
			// DSK後払い：接続元ID
			Constants.PAYMENT_SETTING_DSK_DEFERRED_TERMINAI_ID = GetAppStringSetting("Payment_Dsk_Deferred_ConnectionSourceId");
			// DSK後払い：ダイレクトパスワード
			Constants.PAYMENT_SETTING_DSK_DEFERRED_SHOP_PASSWORD = GetAppStringSetting("Payment_Dsk_Deferred_DirectPassword");
			// DSK後払い：注文情報登録URL
			Constants.PAYMENT_SETTING_DSK_DEFERRED_URL_ORDERREGISTER = GetAppStringSetting("Payment_Dsk_Deferred_Url_OrderRegister");
			// DSK後払い：注文情報修正URL
			Constants.PAYMENT_SETTING_DSK_DEFERRED_URL_ORDERMODIFY = GetAppStringSetting("Payment_Dsk_Deferred_Url_OrderModify");
			// DSK後払い：与信結果取得URL
			Constants.PAYMENT_SETTING_DSK_DEFERRED_URL_GETAUTHRESULT = GetAppStringSetting("Payment_Dsk_Deferred_Url_GetAuthResult");
			// DSK後払い：注文キャンセルURL
			Constants.PAYMENT_SETTING_DSK_DEFERRED_URL_ORDERCANCEL = GetAppStringSetting("Payment_Dsk_Deferred_Url_OrderCancel");
			// DSK後払い：請求書印字データ取得URL
			Constants.PAYMENT_SETTING_DSK_DEFERRED_URL_GETINVOICEPRINTDATA = GetAppStringSetting("Payment_Dsk_Deferred_Url_GetInvoicePrintData");
			// DSK後払い：発送情報登録URL
			Constants.PAYMENT_SETTING_DSK_DEFERRED_URL_SHIPMENT = GetAppStringSetting("Payment_Dsk_Deferred_Url_Shipment");
			// DSK後払い：受注明細連携
			Constants.PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_COOPERATION = GetAppBoolSetting("Payment_Setting_Dsk_Deferred_Order_Detail_Cooperation");
			// DSK後払い：受注明細：小計
			Constants.PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_NAME_SUBTOTAL = GetAppStringSetting("Payment_Setting_Dsk_Deferred_Detail_Name_Subtotal");
			// DSK後払い：受注明細：送料
			Constants.PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_NAME_SHIPPING = GetAppStringSetting("Payment_Setting_Dsk_Deferred_Detail_Name_Shipping");
			// DSK後払い：受注明細：決済手数料
			Constants.PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_NAME_PAYMENT = GetAppStringSetting("Payment_Setting_Dsk_Deferred_Detail_Name_Payment");
			// DSK後払い：受注明細：割引等
			Constants.PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_NAME_DISCOUNT_ETC = GetAppStringSetting("Payment_Setting_Dsk_Deferred_Detail_Name_Discount_Etc");
			// DSK後払い：受注明細：その他ご購入商品
			Constants.PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_NAME_ITEM_ETC = GetAppStringSetting("Payment_Setting_Dsk_Deferred_Detail_Name_Discount_Etc");
			// DSK後払い：請求書同梱フラグ
			Constants.PAYMENT_SETTING_DSK_DEFERRED_USE_INVOICE_BUNDLE = GetAppBoolSetting("Payment_Dsk_Deferred_InvoiceBundle");

			// 台湾FLAPS連携オプション
			Constants.FLAPS_OPTION_ENABLE = GetAppBoolSetting("Flaps_Option_Enable");
			// 台湾FLAPS連携：API接続URL
			Constants.FLAPS_API_URL = GetAppStringSetting("Flaps_Api_Url");
			// 台湾FLAPS連携トークン
			Constants.FLAPS_API_TOKEN = GetAppStringSetting("Flaps_Api_Token");
			// 台湾FLAPS連携IS指令集
			Constants.FLAPS_OPTION_IS = GetAppStringSetting("Flaps_Api_Is");
			// 台湾FLAPS連携API連携設定
			Constants.FLAPS_API_LOGFILE_OUTPUT = GetAppBoolSetting("FLAPS_Api_LogFile_Output");
			Constants.FLAPS_API_LOGFILE_NAME_PREFIX = GetAppStringSetting("FLAPS_Api_LogFileNamePrefix");
			Constants.PHYSICALDIRPATH_FLAPS_API_LOGFILE = GetAppStringSetting("Directory_FLAPS_Api_LogFilePath");
			Constants.FLAPS_API_LOGFILE_THRESHOLD = GetAppIntSetting("FLAPS_Api_LogFileThreshold");

			// レジコード(注文連携の際に必要な値)
			Constants.FLAPS_ORDER_CASH_REGISTER_CODE = GetAppStringSetting("Flaps_Order_Cash_Register_Code");
			// ショップカウンターコード(注文連携の際に必要な値)
			Constants.FLAPS_ORDER_SALE_POINT_CODE = GetAppStringSetting("Flaps_Order_Sale_Point_Code");
			// 販売者コード(注文連携の際に必要な値)
			Constants.FLAPS_ORDER_USER_ID_CODE = GetAppStringSetting("Flaps_Order_User_Id_Code");
			// 会員唯一番号(注文連携の際に必要な値)
			Constants.FLAPS_ORDER_MEMBER_SER_NO = GetAppStringSetting("Flaps_Order_Member_Ser_No");
			// 会員カード発行者コード(注文連携の際に必要な値)
			Constants.FLAPS_ORDER_EMPLOYEE_CODE = GetAppStringSetting("Flaps_Order_Employee_Code");
			// ショップカウンター業績コード用の注文拡張項目の番号。1と10の間を指定する。 (注文キャンセル処理時必要)
			Constants.FLAPS_ORDEREXTENDSETTING_ATTRNO_FOR_POSNOSERNO =
				"attribute" + GetAppStringSetting("Flaps_OrderExtendSetting_AttrNo_For_PosNoSerNo");
			// シリアルナンバーとバーコード用の商品連携ID(cooperation_id)の番号(カンマ区切りで1と10の間の別の数字を入力)
			// "シリアルナンバー用の番号, バーコード用の番号" 例: "1,2"
			Constants.FLAPS_PRODUCT_COOPIDS_FOR_SERNO_AND_BARCODE =
				GetAppStringSettingList("Flaps_Product_CoopIds_For_SerNo_And_Barcode")
					.Select(id => "cooperation_id" + id).ToList();
			// シリアルナンバーとバーコード用の商品バリエーション連携ID(cooperation_id)の番号(カンマ区切りで1と10の間の別の数字を入力)
			// "シリアルナンバー用の番号, バーコード用の番号" 例: "1,2"
			Constants.FLAPS_PRODUCTVARIATION_COOPIDS_FOR_SERNO_AND_BARCODE =
				GetAppStringSettingList("Flaps_ProductVariation_CoopIds_For_SerNo_And_Barcode")
					.Select(id => "variation_cooperation_id" + id).ToList();
			// 一度に取得する商品データの数
			Constants.FLAPS_THE_NUMBER_OF_RECORDS_TO_CAPTURE_AT_ONCE =
				GetAppStringSetting("Flaps_The_Number_Of_Records_To_Capture_Per_Request").Trim().Replace(",", "");

			/// <summary>O-PLUX連携オプション</summary>
			Constants.OPLUX_ENABLED = GetAppBoolSetting("OPlux_Enabled");
			/// <summary>加盟店ID（企業ID）</summary>
			Constants.OPLUX_REQUEST_SHOP_ID = GetAppStringSetting("OPlux_Request_Shop_Id");
			/// <summary>接続元ID</summary>
			Constants.OPLUX_CONNECTION_ID = GetAppStringSetting("OPlux_Connection_Id");
			/// <summary>シークレットアクセスキー</summary>
			Constants.OPLUX_SECRET_ACCESS_KEY = GetAppStringSetting("OPlux_Secret_Access_Key");
			/// <summary>審査モデルID</summary>
			Constants.OPLUX_REQUEST_EVENT_MODEL_ID = GetAppStringSetting("OPlux_Request_Event_Model_id");
			/// <summary>接続先URL：イベント登録API（POST）</summary>
			Constants.OPLUX_REQUEST_EVENT_URL = GetAppStringSetting("OPlux_Request_Event_Url");
			/// <summary>接続先URL：氏名正規化API（GET ）</summary>
			Constants.OPLUX_NORMALIZE_NAME_URL = GetAppStringSetting("OPlux_Normalize_Name_Url");
			/// <summary>決済種別　クレジットカード</summary>
			Constants.OPLUX_PAYMENT_KBN = GetAppStringSetting("OPlux_Payment_Kbn");
			/// <summary>O-PLUX拡張ステータス</summary>
			Constants.OPLUX_REVIEW_EXTEND_NO = GetAppStringSetting("OPlux_Review_ExtendNo");
			/// <summary>Directory O-PLUX api log file path</summary>
			Constants.DIRECTORY_OPLUX_API_LOGFILEPATH = GetAppStringSetting("Directory_OPlux_Api_LogFilePath");

			// O-MOTION連携オプション
			Constants.OMOTION_ENABLED = GetAppBoolSetting("OMotion_Enabled");
			// O-MOTION：加盟店コード
			Constants.OMOTION_MERCHANTID = GetAppStringSetting("OMotion_MerchantId");
			// O-MOTION：署名コード
			Constants.OMOTION_SIGNATURE = GetAppStringSetting("OMotion_Signature");
			// O-MOTION：ハッシュ用ソルト値
			Constants.OMOTION_SALT = GetAppStringSetting("OMotion_Salt");
			// O-MOTION：JS path
			Constants.OMOTION_JS_PATH = GetAppStringSetting("OMotion_JsPath");
			// O-MOTION：Authori url
			Constants.OMOTION_REQUEST_AUTHORI_URL = GetAppStringSetting("OMotion_Request_Authori_Url");
			// O-MOTION：Authori login success url
			Constants.OMOTION_REQUEST_AUTHORI_LOGIN_SUCCESS_URL = GetAppStringSetting("OMotion_Request_Authori_Login_Success_Url");
			// O-MOTION：Authori feedback url
			Constants.OMOTION_REQUEST_AUTHORI_FEEDBACK_URL = GetAppStringSetting("OMotion_Request_Authori_Feedback_Url");
			// O-MOTION：テストログインID
			Constants.OMOTION_TEST_LOGINID = GetAppStringSetting("OMotion_Test_LoginID");

			// PayPayオプション（決済）
			Constants.PAYMENT_PAYPAYOPTION_ENABLED = GetAppBoolSetting("Payment_PayPayOption_Enabled");
			// 決済区分：PayPay (SBPS、GMO)
			Constants.PAYMENT_PAYPAY_KBN = (Constants.PaymentPayPayKbn?)GetAppSetting("Payment_PayPayKbn", typeof(Constants.PaymentPayPayKbn));
			// PayPay　処理区分(AUTH:仮売上, CAPTURE:即時売上)
			Constants.PAYMENT_PAYPAY_JOB_CODE = GetAppStringSetting("Payment_PayPay_JobCode");
			// PayPayショップID
			Constants.PAYMENT_PAYPAY_SHOP_ID = GetAppStringSetting("Payment_PayPay_ShopID");
			// PayPayショップパスワード
			Constants.PAYMENT_PAYPAY_SHOP_PASSWORD = GetAppStringSetting("Payment_PayPay_ShopPassword");
			// PayPay　商品カテゴリID
			Constants.PAYMENT_PAYPAY_CATEGORY_ID = GetAppStringSetting("Payment_PayPay_CategoryId");
			// PayPay　ペイペイ接続先API 取引登録API
			Constants.PAYMENT_PAYPAY_ENTRY_TRAN_API = GetAppStringSetting("Payment_PayPay_Entry_tran_Api");
			// PayPay　ペイペイ接続先API 決済実行API
			Constants.PAYMENT_PAYPAY_EXEC_API = GetAppStringSetting("Payment_PayPay_Exec_Api");
			// PayPay　ペイペイ接続先API 実売上API
			Constants.PAYMENT_PAYPAY_SALES_API = GetAppStringSetting("Payment_PayPay_Sales_Api");
			// PayPay　ペイペイ接続先API キャンセルAPI
			Constants.PAYMENT_PAYPAY_CANCEL_RETURN_API = GetAppStringSetting("Payment_PayPay_Cancel_Return_Api");
			// PayPay接続先：取引照会API
			Constants.PAYMENT_PAYPAY_SEARCH_TRADE_MULTI_API = GetAppStringSetting("Payment_PayPay_Search_Trade_Multi_Api");
			// PayPay　ペイペイ接続先API 定期取引登録API
			Constants.PAYMENT_PAYPAY_ENTRY_TRAN_ACCEPT_API = GetAppStringSetting("Payment_PayPay_Entry_tran_Accept_Api");
			// PayPay　ペイペイ接続先API 定期決済実行API
			Constants.PAYMENT_PAYPAY_EXEC_ACCEPT_API = GetAppStringSetting("Payment_PayPay_Exec_Accept_Api");
			// PayPay　ペイペイ接続先API 利用承諾終了API
			Constants.PAYMENT_PAYPAY_EXEC_ACCEPT_END_API = GetAppStringSetting("Payment_PayPay_Exec_Accept_End_Api");

			// データ移行オプション
			Constants.DATAMIGRATION_OPTION_ENABLED = GetAppBoolSetting("DataMigration_OptionEnabled");
			// データ移行利用終了日時
			Constants.DATAMIGRATION_END_DATETIME = GetAppDateTimeSetting("DataMigration_EndDateTime");
			// データ移行ログ出力先物理パス
			Constants.DIRECTORY_DATAMIGRATION_LOG_FILEPATH = GetAppStringSetting("Directory_DataMigration_LogFilePath");

			/// <summary>Introduction coupon option enabled</summary>
			Constants.INTRODUCTION_COUPON_OPTION_ENABLED = GetAppBoolSetting("IntroductionCouponOption_Enabled");

			/// <summary>Personal authentication of user registration option enabled</summary>
			Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED = GetAppBoolSetting("PersonalAuthenticationOfUserRegistration_Option_Enabled");
			/// <summary>Personal authentication of user registration auth method</summary>
			Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_AUTH_METHOD = GetAppStringSetting("PersonalAuthenticationOfUserRegistration_AuthMethod");
			/// <summary>Personal authentication of user registration authcode digits</summary>
			Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_AUTH_CODE_DIGITS = GetAppIntSetting("PersonalAuthenticationOfUserRegistration_AuthCode_Digits");
			/// <summary>Personal authentication of user registration authcode expiration time</summary>
			Constants.PERSONAL_AUTHENTICATION_OF_USERR_EGISTRATION_AUTH_CODE_EXPIRATION_TIME = GetAppIntSetting("PersonalAuthenticationOfUserRegistration_AuthCode_ExpirationTime");
			/// <summary>Personal authentication possible trial auth code send count</summary>
			Constants.PERSONAL_AUTHENTICATION_POSSIBLE_TRIAL_AUTH_CODE_SEND_COUNT = GetAppIntSetting("PersonalAuthentication_Possible_Trial_AuthCodeSend_Count");
			/// <summary>Personal authentication auth code send lock minutes</summary>
			Constants.PERSONAL_AUTHENTICATION_AUTH_CODE_SEND_LOCK_MINUTES = GetAppIntSetting("PersonalAuthentication_AuthCodeSend_Lock_Minutes");
			/// <summary>Personal authentication possible trial auth code try count</summary>
			Constants.PERSONAL_AUTHENTICATION_POSSIBLE_TRIAL_AUTH_CODE_TRY_COUNT = GetAppIntSetting("PersonalAuthentication_Possible_Trial_AuthCodeTry_Count");
			/// <summary>Personal authentication  auth code try lock minutes</summary>
			Constants.PERSONAL_AUTHENTICATION_AUTH_CODE_TRY_LOCK_MINUTES = GetAppIntSetting("PersonalAuthentication_AuthCodeTry_Lock_Minutes");

			/// <summary>FacebookコンバージョンAPI連携オプション</summary>
			Constants.MARKETING_FACEBOOK_CAPI_OPTION_ENABLED = GetAppBoolSetting("Marketing_Facebook_CAPI_OptionEnabled");
			/// <summary>FacebookコンバージョンAPI連携：API接続先URL</summary>
			Constants.MARKETING_FACEBOOK_CAPI_API_URL = GetAppStringSetting("Marketing_Facebook_CAPI_ApiUrl");
			/// <summary>FacebookコンバージョンAPI連携：API接続先バージョン</summary>
			Constants.MARKETING_FACEBOOK_CAPI_API_VERSION = GetAppStringSetting("Marketing_Facebook_CAPI_ApiVersion");
			/// <summary>FacebookコンバージョンAPI連携：ピクセルID</summary>
			Constants.MARKETING_FACEBOOK_CAPI_PIXEL_ID = GetAppStringSetting("Marketing_Facebook_CAPI_PixelId");
			/// <summary>FacebookコンバージョンAPI連携：アクセストークン</summary>
			Constants.MARKETING_FACEBOOK_CAPI_ACCESS_TOKEN = GetAppStringSetting("Marketing_Facebook_CAPI_AccessToken");
			/// <summary>FacebookコンバージョンAPI連携：テストイベントコード（イベントテストツール用）</summary>
			Constants.MARKETING_FACEBOOK_CAPI_TEST_EVENT_CODE = GetAppStringSetting("Marketing_Facebook_CAPI_TestEventCode");

			// Facebook catalog api 連携のON/OFFの設定
			Constants.FACEBOOK_CATALOG_API_COOPERATION_OPTION_ENABLED = GetAppBoolSetting("Facebook_Catalog_Api_Cooperation_Option_Enabled");
			// Facebook catalog api url
			Constants.FACEBOOK_CATALOG_API_URL = GetAppStringSetting("Facebook_Catalog_Api_Url");
			// Facebook catalog api version
			Constants.FACEBOOK_CATALOG_API_VERSION = GetAppStringSetting("Facebook_Catalog_ApiVersion");

			/// <summary>スコアリング販売オプション</summary>
			Constants.SCORINGSALE_OPTION = GetAppBoolSetting("ScoringSale_Option");
			// CrossPoint連携
			Constants.CROSS_POINT_API_URL_ROOT_PATH = GetAppStringSetting("CrossPoint_ApiUrl_RootPath");
			Constants.CROSS_POINT_AUTH_TENANT_CODE = GetAppStringSetting("CrossPoint_Auth_TenantCode");
			Constants.CROSS_POINT_AUTH_SHOP_CODE = GetAppStringSetting("CrossPoint_Auth_ShopCode");
			Constants.CROSS_POINT_POS_NO = GetAppStringSetting("CrossPoint_PosNo");
			Constants.CROSS_POINT_AUTH_AUTHENTICATION_KEY = GetAppStringSetting("CrossPoint_Auth_AuthenticationKey");
			Constants.CROSS_POINT_APP_REQUEST_APP_KEY = GetAppStringSetting("CrossPoint_AppRequest_AppKey");
			Constants.CROSS_POINT_API_WRITE_LOG = GetAppBoolSetting("CrossPoint_ApiWriteLog");
			Constants.CROSS_POINT_LINK_START_DATETIME = GetAppDateTimeSetting("CrossPoint_LinkStartDateTime");
			Constants.CROSS_POINT_OPTION_ENABLED = GetAppBoolSetting("CrossPoint_Option_Enabled");
			Constants.CROSS_POINT_LOGIN_POINT_ENABLED = GetAppBoolSetting("CrossPoint_LoginPoint_Enabled");
			Constants.CROSS_POINT_FTP_HOST = GetAppStringSetting("CrossPoint_Ftp_Host");
			Constants.CROSS_POINT_FTP_ID = GetAppStringSetting("CrossPoint_Ftp_Id");
			Constants.CROSS_POINT_FTP_PW = GetAppStringSetting("CrossPoint_Ftp_Pw");
			Constants.CROSS_POINT_FTP_PORT = GetAppIntSetting("CrossPoint_Ftp_Port");
			Constants.CROSS_POINT_FTP_FILE_PATH = GetAppStringSetting("CrossPoint_Ftp_FilePath");
			Constants.CROSS_POINT_EC_SHOP_NAME = GetAppStringSetting("CrossPoint_EC_ShopName");
			Constants.CROSS_POINT_USREX_SHOP_CARD_NO = GetAppStringSetting("CrossPoint_Usrex_Shop_Card_No");
			Constants.CROSS_POINT_USREX_SHOP_ADD_SHOP_NAME = GetAppStringSetting("CrossPoint_Usrex_Shop_Add_Shop_Name");
			Constants.CROSS_POINT_USREX_SHOP_CARD_PIN = GetAppStringSetting("CrossPoint_Usrex_Shop_Card_Pin");
			Constants.CROSS_POINT_USREX_SHOP_APP_MEMBER_FLAG = GetAppStringSetting("CrossPoint_Usrex_Shop_App_Member_Flag");
			Constants.CROSS_POINT_USREX_DM = GetAppStringSetting("CrossPoint_Usrex_Dm");
			Constants.CROSS_POINT_USREX_MAIL_FLG = GetAppStringSetting("CrossPoint_Usrex_Mail_Flg");
			Constants.CROSS_POINT_JANCODE_PRODUCT_COOPERATION_ID_NO = GetAppIntOrNullSetting("CrossPoint_JanCode_Product_CooperationId_No") ?? 0;
			Constants.CROSS_POINT_DUMMY_JANCODE = GetAppStringSetting("CrossPoint_Dummy_JanCode");
			Constants.ORDER_POINT_BATCH_POINT_TEMP_TO_COMP_DAYS = GetAppIntSetting("OrderPointBatch_PointTempToCompDays");

			/// <summary>ポイント：ポイント利用時の最低購入金額</summary>
			decimal.TryParse(GetAppStringSetting("Point_Minimum_PurchasePrice"), out Constants.POINT_MINIMUM_PURCHASEPRICE);

			/// <summary>Boku option enabled</summary>
			Constants.PAYMENT_BOKU_OPTION_ENABLED = GetAppBoolSetting("Payment_Boku_Option_Enabled");
			/// <summary>Boku optin url</summary>
			Constants.PAYMENT_BOKU_OPTIN_URL = GetAppStringSetting("Payment_Boku_Optin_Url");
			/// <summary>Boku validate optin url</summary>
			Constants.PAYMENT_BOKU_VALIDATE_OPTIN_URL = GetAppStringSetting("Payment_Boku_Validate_Optin_Url");
			/// <summary>Boku confirm optin url</summary>
			Constants.PAYMENT_BOKU_CONFIRM_OPTIN_URL = GetAppStringSetting("Payment_Boku_Confirm_Optin_Url");
			/// <summary>Boku charge url</summary>
			Constants.PAYMENT_BOKU_CHARGE_URL = GetAppStringSetting("Payment_Boku_Charge_Url");
			/// <summary>Boku query charge url</summary>
			Constants.PAYMENT_BOKU_QUERY_CHARGE_URL = GetAppStringSetting("Payment_Boku_Query_Charge_Url");
			/// <summary>Boku reverse charge url</summary>
			Constants.PAYMENT_BOKU_REVERSE_CHARGE_URL = GetAppStringSetting("Payment_Boku_Reverse_Charge_Url");
			/// <summary>Boku refund charge url</summary>
			Constants.PAYMENT_BOKU_REFUND_CHARGE_URL = GetAppStringSetting("Payment_Boku_Refund_Charge_Url");
			/// <summary>Boku query refund url</summary>
			Constants.PAYMENT_BOKU_QUERY_REFUND_URL = GetAppStringSetting("Payment_Boku_Query_Refund_Url");
			/// <summary>Boku check eligibility url</summary>
			Constants.PAYMENT_BOKU_CANCEL_OPTIN_URL = GetAppStringSetting("Payment_Boku_Cancel_Optin_Url");
			/// <summary>Boku merchant id</summary>
			Constants.PAYMENT_BOKU_MERCHANT_ID = GetAppStringSetting("Payment_Boku_Merchant_Id");
			/// <summary>Boku merchant request id</summary>
			Constants.PAYMENT_BOKU_MERCHANT_REQUEST_ID = GetAppStringSetting("Payment_Boku_Merchant_Request_Id");
			/// <summary>Boku time out request</summary>
			Constants.PAYMENT_BOKU_TIMEOUT_REQUEST = GetAppIntSetting("Payment_Boku_TimeOut_Request");
			/// <summary>Boku skip retry flag</summary>
			Constants.PAYMENT_BOKU_SKIP_RETRY_FLG = GetAppBoolSetting("Payment_Boku_Skip_Retry_Flg");
			/// <summary>Boku send notification flag</summary>
			Constants.PAYMENT_BOKU_SEND_NOTIFICATION_FLG = GetAppBoolSetting("Payment_Boku_Send_Notification_Flg");
			/// <summary>Boku key id</summary>
			Constants.PAYMENT_BOKU_KEY_ID = GetAppStringSetting("Payment_Boku_Key_Id");
			/// <summary>Boku api key</summary>
			Constants.PAYMENT_BOKU_API_KEY = GetAppStringSetting("Payment_Boku_API_Key");
			/// <summary>Boku merchant consumer id</summary>
			Constants.PAYMENT_BOKU_MERCHANT_CONSUMER_ID = GetAppStringSetting("Payment_Boku_Merchant_Consumer_Id");
			/// <summary>Boku payment query limit time</summary>
			Constants.BOKU_PAYMENT_QUERY_LIMIT_TIME = GetAppIntSetting("Boku_Payment_Query_Limit_Time");

			//GMO Payment setting
			Constants.PAYMENT_GMO_POST_ENABLED = GetAppBoolSetting("Payment_GMO_Post");
			Constants.SETTING_GMO_PAYMENT_AUTHENTICATIONID = GetAppStringSetting("Gmo_Payment_AuthencationId");
			Constants.SETTING_GMO_PAYMENT_SHOPCODE = GetAppStringSetting("Gmo_Payment_ShopCode");
			Constants.SETTING_GMO_PAYMENT_CONNECTPASSWORD = GetAppStringSetting("Gmo_Payment_ConnectPassword");
			Constants.SETTING_GMO_PAYMENT_URL_TRANSACTION_REGISTER = GetAppStringSetting("Gmo_Payment_Url_Transaction_Register");
			Constants.SETTING_GMO_PAYMENT_URL_MODIFY_CANCEL_TRANSACTION = GetAppStringSetting("Gmo_Payment_Url_Modify_Cancel_Transaction");
			Constants.SETTING_GMO_PAYMENT_URL_BILLING_CONFIRM = GetAppStringSetting("Gmo_Payment_Url_Billing_Confirm");
			Constants.SETTING_GMO_PAYMENT_URL_BILLING_MODIFY_CANCEL = GetAppStringSetting("Gmo_Payment_Url_Billing_Modify_Cancel");
			Constants.SETTING_GMO_PAYMENT_URL_GET_CREDIT_RESULT = GetAppStringSetting("Gmo_Payment_Url_Get_Credit_Result");
			Constants.SETTING_GMO_PAYMENT_URL_REDUCED_CLAIMS = GetAppStringSetting("Gmo_Payment_Url_Reduced_Claims");
			Constants.SETTING_GMO_PAYMENT_URL_FRAME_GUARANTEE_GET_STATUS = GetAppStringSetting("Gmo_Payment_Url_Frame_Gurantee_Get_Status");
			Constants.SETTING_GMO_PAYMENT_URL_FRAME_GUARANTEE_REGISTER = GetAppStringSetting("Gmo_Payment_Url_Frame_Gurantee_Register");
			Constants.SETTING_GMO_PAYMENT_URL_FRAME_GUARANTEE_UPDATE = GetAppStringSetting("Gmo_Payment_Url_Frame_Gurantee_Update");

			// Order workflow limit update order status enabled
			Constants.ORDERWORKFLOW_LIMIT_UPDATEORDERSTATUS_ENABLED = GetAppBoolSetting("OrderWorkflow_Limit_UpdateOrderStatus_Enabled");

			/// <summary>決済：楽天コンビニ前払い：サブサービスID(セブンイレブン)</summary>
			Constants.PAYMENT_RAKUTENCVSDEF_SUBSERVICEID_SEVEN = GetAppStringSetting("Payment_RakutenCvsDef_SubServiceId_seven");
			/// <summary>決済：楽天コンビニ前払い：サブサービスID(イーコン)</summary>
			Constants.PAYMENT_RAKUTENCVSDEF_SUBSERVICEID_ECON = GetAppStringSetting("Payment_RakutenCvsDef_SubServiceId_Econ");
			/// <summary>決済：楽天コンビニ前払い：サブサービスID(ウェルネット)</summary>
			Constants.PAYMENT_RAKUTENCVSDEF_SUBSERVICEID_WELLNET = GetAppStringSetting("Payment_RakutenCvsDef_SubServiceId_Wellnet");
			/// <summary>決済：楽天コンビニ前払い：支払期限日数</summary>
			Constants.PAYMENT_RAKUTEN_CVS_RECEIPT_DISPLAY_NAME = GetAppStringSetting("Payment_Rakuten_Cvs_Receipt_display_name");
			/// <summary>Rakutenコンビニ前払い与信期限切れ(X日後有効期限切れ)  ※最終与信日を含まない</summary>
			Constants.PAYMENT_RAKUTEN_CVSDEF_AUTHLIMITDAY = GetAppIntSetting("Payment_Rakuten_CvsDef_AuthLimitDay");
			/// <summary>Payment rakuten cvs mock option enabled</summary>
			Constants.PAYMENT_RAKUTEN_CVS_MOCK_OPTION_ENABLED = GetAppBoolSetting("Payment_Rakuten_Cvs_Mock_Option_Enabled");
			/// <summary>Payment rakuten cvs api url authentication mock</summary>
			Constants.PAYMENT_RAKUTEN_CVS_APIURL_AUTH_MOCK = GetAppStringSetting("Payment_Rakuten_Cvs_ApiUrl_Auth_Mock");

			/// <summary>商品付帯情報の選択肢に価格情報を持たせるオプション</summary>
			Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED = GetAppBoolSetting("Product_Option_Settings_Price_Grant_Enabled");

			// 商品画像ヘッダ使用
			Constants.PRODUCT_IMAGE_HEAD_ENABLED = GetAppBoolSetting("Product_Image_Head_Enabled");

			// ギフト遷移短縮機能
			Constants.SHORTENING_GIFT_OPTION_ENABLED = GetAppBoolSetting("ShorteningGiftOption_Enabled");
			// Gift order option with shortening gift option enabled
			Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED = Constants.GIFTORDER_OPTION_ENABLED
				&& Constants.SHORTENING_GIFT_OPTION_ENABLED;

			// Yahoo API - アクセストークンを取得するためにアクセスするエンドポイント(Tokenエンドポイント)
			Constants.YAHOO_API_TOKEN_API_URL = GetAppStringSetting("Yahoo_Api_Token_Api_Url");

			// 決済：ZEUSコンビニ決済：加盟店IPコード
			Constants.PAYMENT_CVS_ZUES_CLIENT_IP = GetAppStringSetting("Payment_Cvs_Zeus_Client_Ip");
			// 決済：ZEUSコンビニ決済：テスト決済フラグ
			Constants.PAYMENT_CVS_ZUES_PAYMENT_TEST_FLAG = GetAppBoolSetting("Payment_Cvs_Zeus_Payment_Test_Flag");
			// 決済：ZEUSコンビニ決済：テストID
			Constants.PAYMENT_CVS_ZUES_TEST_ID = GetAppStringSetting("Payment_Cvs_Zeus_Test_Id");
			// 決済：ZEUSコンビニ決済：テスト決済処理区分
			Constants.PAYMENT_CVS_ZUES_TEST_TYPE = GetAppStringSetting("Payment_Cvs_Zeus_Test_Type");

			/// <summary>楽天モール連携SKU移行状態</summary>
			Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION = GetAppBoolSetting("MallCooperation_Rakuten_SKUMigration");
			/// <summary>楽天モール連携 ファイル転送時のSFTP利用</summary>
			Constants.MALLCOOPERATION_RAKUTEN_USE_SFTP = GetAppBoolSetting("MallCooperation_Rakuten_Use_SFTP");

			// 店舗受取オプション
			Constants.STORE_PICKUP_OPTION_ENABLED = Constants.REALSHOP_OPTION_ENABLED && GetAppBoolSetting("StorePickupOption_Enabled");
			// 店舗受取時選択可能決済種別ID(カンマ区切り)
			Constants.SETTING_CAN_STORE_PICKUP_OPTION_PAYMENT_IDS = GetAppStringSettingList("StorePickUpOption_CanStorePickUpOptionPaymentIds");

			/// <summary>CROSSMALL連携オプション</summary>
			Constants.CROSS_MALL_OPTION_ENABLED = GetAppBoolSetting("CrossMall_Enabled");
			/// <summary>CROSSMALL連携 ： アカウント(会社コード)</summary>
			Constants.CROSS_MALL_ACCOUNT = GetAppStringSetting("CrossMall_Account");
			/// <summary>CROSSMALL連携 ： 認証鍵</summary>
			Constants.CROSS_MALL_AUTHENTICATION_KEY = GetAppStringSetting("CrossMall_Authentication_Key");
			/// <summary>CROSSMALL連携 ： 注文伝票情報取得API</summary>
			Constants.CROSS_MALL_GET_ORDER_API_URL = GetAppStringSetting("CrossMall_GetOrder_ApiUrl");
			/// <summary>CROSSMALL連携 ： API連携時のログ排出設定</summary>
			Constants.CROSS_MALL_INTEGRATION_PHASE_NAME_DELIMITER = GetAppStringSetting("CrossMall_Integration_Phase_Name_Delimiter");
			/// <summary>CROSSMALL連携 ： 出荷完了連携処理フェーズ名(区切り文字利用により複数設定可)</summary>
			Constants.CROSS_MALL_INTEGRATION_PHASE_NAME = GetAppStringSetting("CrossMall_Integration_Phase_Name");
			/// <summary>CROSSMALL連携 ： API連携時のログ排出設定</summary>
			Constants.CROSS_MALL_INTEGRATION_ENABLE_LOGGING = GetAppBoolSetting("CrossMall_Integration_Enable_Logging");

			// 配送料無料適用外機能を利用するかどうか
			Constants.FREE_SHIPPING_FEE_OPTION_ENABLED = GetAppBoolSetting("Free_Shipping_Fee_Option_Enabled");

			// オプション：awoo連携
			Constants.AWOO_OPTION_ENABLED = GetAppBoolSetting("Awoo_Option_Enabled");
			// awoo連携ID
			Constants.AWOO_NUNUNIID = GetAppStringSetting("Awoo_NununiId");
			// awoo連携Baererトークン
			Constants.AWOO_AUTHENTICATION_BAERER_TOKEN = GetAppStringSetting("Awoo_Authentication_Baerer_Token");
			// awooAPIサーバーURL
			Constants.AWOO_API_SERVER = GetAppStringSetting("Awoo_Api_ServerUrl");
			// PAGE API
			Constants.AWOO_API_PAGE_ACTION = GetAppStringSetting("Awoo_Api_PageAction");
			// TAGS API
			Constants.AWOO_API_TAGS_ACTION = GetAppStringSetting("Awoo_Api_TagsAction");
			// CLASSIFYPRODUCTTYPE API
			Constants.AWOO_API_CLASSIFYPRODUCTTYPE_ACTION = GetAppStringSetting("Awoo_Api_ClassifyProductTypeAction");
			// awoo商品一覧ページに表示する商品数
			Constants.AWOO_PAGE_PRODUCT_LIMIT = GetAppIntSetting("Awoo_Page_Product_Limit");
			// 商品詳細画面のAWOOレコメンド商品の設定
			Constants.AWOO_RECOMMEND_DIRECTION = GetAppStringSettingList("Awoo_Recommend_Direction").ToArray();
			// 商品詳細画面のおすすめの商品表示数
			Constants.AWOO_PRODUCT_DETAIL_PRODUCT_LIMIT = GetAppIntSetting("Awoo_Product_Detail_Product_Limit");
			// Awooとの連携ログを出力するか
			Constants.AWOO_LOG_OUTPUT_FLAG = GetAppBoolSetting("Awoo_Log_Output_Flag");

			// Letro連携オプション
			Constants.LETRO_OPTION_ENABLED = GetAppBoolSetting("Letro_Option_Enabled");
			// Letro用の連携認証キー
			Constants.LETRO_API_AUTHENTICATION_KEY = GetAppStringSetting("Letro_Api_Authentication_Key");
			// Letro用の許可IPアドレス
			Constants.LETRO_ALLOWED_IP_ADDRESS = GetAppStringSettingList("Letro_Allowed_IP_Address", StringSplitOptions.RemoveEmptyEntries);
			// Letro APIの認証キー試行回数
			Constants.POSSIBLE_LETRO_AUTH_KEY_ERROR_COUNT = GetAppIntSetting("Possible_Letro_Auth_Key_Error_Count");
			// Letro APIの認証キーロックの有効時間(分単位)
			Constants.POSSIBLE_LETRO_AUTH_KEY_LOCK_MINUTES = GetAppIntSetting("Possible_Letro_Auth_Key_Lock_Minutes");

			// メール：ListUnsubscribeオプション
			Constants.MAIL_LISTUNSUBSCRIBE_OPTION_ENABLED = GetAppBoolSetting("Mail_ListUnsubscribe_Option_Enabled");
			// メール：ListUnsubscribe メール送信先
			Constants.MAIL_LISTUNSUBSCRIBE_MAILTO = GetAppStringSetting("Mail_ListUnsubscribe_MailTo");

			// メール：DKIMオプション
			Constants.MAIL_DKIM_OPTION_ENABLED = GetAppBoolSetting("Mail_Dkim_Option_Enabled");
			// メール：DKIM セレクタ
			Constants.MAIL_DKIM_SELECTOR = GetAppStringSetting("Mail_Dkim_Selector");

			// Recustomer連携オプション
			Constants.RECUSTOMER_API_OPTION_ENABLED = GetAppBoolSetting("Recustomer_Api_Option_Enabled");
			// RecustomerストアKey
			Constants.RECUSTOMER_API_STOER_KEY = GetAppStringSetting("Recustomer_Api_Store_Key");
			// Recustomer認証用トークン
			Constants.RECUSTOMER_API_TOKEN = GetAppStringSetting("Recustomer_Api_Token");
			// Recustomer連携OrderImporterURL
			Constants.RECUSTOMER_API_ORDER_IMPORTER_URL = GetAppStringSetting("Recustomer_Api_Order_Importer_URL");
			// Recustomerリクエストログ設定
			Constants.RECUSTOMER_API_REQUEST_LOG_EXPORT_ENABLED = GetAppBoolSetting("Recustomer_Api_Request_Log_Export_Enabled");
			// Recustomer_Api_Prefix
			Constants.RECUSTOMER_API_PREFIX = GetAppStringSetting("Recustomer_Api_Prefix");

			// ペイジェントクレジット 差分通知ハッシュ値生成キー
			Constants.PAYMENT_PAYGENT_NOTICE_HASHKEY = GetAppStringSetting("Payment_Paygent_Notice_HashKey");
			// ペイジェントクレジット コンビニ支払期限(日)
			Constants.PAYMENT_PAYGENT_CVS_PAYMENT_LIMIT_DAY = GetAppIntSetting("Payment_Paygent_Cvs_PaymentLimitDay");

			// ネットバンキング決済オプション
			Constants.PAYMENT_NETBANKING_OPTION_ENABLED = GetAppBoolSetting("Payment_NetBankingOption_Enabled");
			// 決済区分
			Constants.PAYMENT_NETBANKING_KBN = (Constants.PaymentBanknetKbn?)GetAppSetting("Payment_NetBankingKbn", typeof(Constants.PaymentBanknetKbn));

			// ATM決済オプション
			Constants.PAYMENT_ATMOPTION_ENABLED = GetAppBoolSetting("Payment_AtmOption_Enabled");
			// 決済区分：ATM (Paygent)
			Constants.PAYMENT_ATM_KBN = (Constants.PaymentATMKbn?)GetAppSetting("Payment_AtmKbn", typeof(Constants.PaymentATMKbn));
		}

		/// <summary>
		/// DataSchemaSetting設定初期化
		/// </summary>
		public void InitializeDataSchema()
		{
			Constants.TAG_REPLACER_DATA_SCHEMA = TagReplacer.GetInstanceDataSchemaSetting();
		}

		/// <summary>
		/// DataSchemaSetting設定初期化(フロント用)
		/// </summary>
		public void InitializeFrontDataSchema()
		{
			Constants.TAG_REPLACER_DATA_SCHEMA = TagReplacer.GetInstanceDataSchemaSetting();
		}

		/// <summary>
		/// アプリケーション設定情報を初期化(サイト系)
		/// </summary>
		public void InitializeSiteCommon()
		{
			// 各種オプション利用有無系
			Constants.PRODUCTREVIEW_AUTOOPEN_ENABLED = GetAppBoolSetting("ProductReview_AutoOpen_Enabled");
			Constants.PRODUCTREVIEW_ENABLED = GetAppBoolSetting("ProductReview_Enabled");
			Constants.W2MP_PRODUCT_SEARCHWORD_RANKING_ENABLED = GetAppBoolSetting("ProductSearchWordRanking_Enabled");
			Constants.SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED = GetAppBoolSetting("ShippingPrice_Separate_Estimate_Enabled");

			// コントロール表示設定系
			Constants.DISPLAY_ADDR4_ENABLED = GetAppBoolSetting("Setting_Addr4_Enabled");
			// ドロップダウン年の並び順
			Constants.YEAR_LIST_ITEM_ORDER = (Constants.YearListItemOrder)GetAppSetting("Year_List_Item_Order", typeof(Constants.YearListItemOrder));

			// デジタルコンテンツ対応
			Constants.DIGITAL_CONTENTS_SERIAL_KEY_FORMAT = GetAppStringSetting("Digital_Contents_Serial_Key_Format");
			Constants.DIGITAL_CONTENTS_SERIAL_KEY_VALID_DAYS = GetAppIntSetting("Digital_Contents_Serial_Key_Valid_Days");

			// プラグインメール送信設定
			Constants.PHYSICALDIRPATH_PLUGINS_STORAGE_LOCATION = GetAppStringSetting("Setting_DirPath_Plugin_Storage_Location");
			Constants.MAILADDRESS_FROM_FOR_PLUGIN = GetAppStringSetting("Setting_Plugin_MailAddress_From");
			Constants.MAILADDRESS_TO_LIST_FOR_PLUGIN = GetAppStringSettingList("Setting_Plugin_MailAddress_To_List", StringSplitOptions.RemoveEmptyEntries);
			Constants.MAILADDRESS_CC_LIST_FOR_PLUGIN = GetAppStringSettingList("Setting_Plugin_MailAddress_Cc_List", StringSplitOptions.RemoveEmptyEntries);
			Constants.MAILADDRESS_BCC_LIST_FOR_PLUGIN = GetAppStringSettingList("Setting_Plugin_MailAddress_Bcc_List", StringSplitOptions.RemoveEmptyEntries);

			// その他
			Constants.CONST_PASSWORDREMAINDER_VALID_MINUTES = GetAppIntSetting("User_PasswordRemainder_Activation_Minutes");
			Constants.FLG_PASSWORDREMINDER_CHANGE_TRIAL_LIMIT_COUNT_DEFAULT = GetAppIntSetting("User_PasswordReminder_Change_Limit_Count_Default");

			Constants.CONST_POSSIBLE_TRIAL_LOGIN_COUNT = GetAppIntSetting("User_Possible_Trial_Login_Count");
			Constants.CONST_ACCOUNT_LOCK_VALID_MINUTES = GetAppIntSetting("User_Account_Lock_Minutes");

			Constants.CONST_POSSIBLE_TRIAL_CREDIT_AUTH_ATTACK_COUNT = (int?)GetAppSetting("Possible_Trial_Credit_AUth_Atack_Count", typeof(int?));
			Constants.CONST_CREDIT_AUTH_ATTACK_LOCK_VALID_MINUTES = GetAppIntSetting("Credit_AUth_Atack_Lock_Minutes");

			Constants.PATH_ROOT_FRONT_MOBILE = GetAppStringSetting("Site_RootPath_MobileFront");
			Constants.GOOGLEANALYTICS_ENABLED = GetAppBoolSetting("GoogleAnalytics_Enabled");
			Constants.GOOGLEANALYTICS_PROFILE_ID = GetAppStringSetting("GoogleAnalytics_ProfileId");
			Constants.GOOGLEANALYTICS_MEASUREMENT_ID = GetAppStringSetting("GoogleAnalytics_MeasurementId");
			Constants.DEFAULTUPDATE_TOUSER_FROMORDEROWNER = GetAppBoolSetting("DefaultUpdate_ToUser_FromOrderOwner");
			Constants.DISPLAY_NOT_SEARCH_DEFAULT = GetAppBoolSetting("Display_Not_Search_Default");
			Constants.FIXEDPURCHASEORDERNOW_NEXT_SHIPPING_DATE_UPDATE_DEFAULT = GetAppBoolSetting("FixedPurchaseOrderNow_NextShippingDate_Update_Default");
			Constants.FIXEDPURCHASEORDERNOW_NEXT_NEXT_SHIPPING_DATE_UPDATE_DEFAULT =
				GetAppBoolSetting("FixedPurchaseOrderNow_NextNextShippingDate_Update_Default");
			Constants.EXCHANGERATE_EXPIRE_MINUTES = GetAppIntSetting("ExchangeRate_ExpireMinutes");
			Constants.CONST_CREDIT_AUTH_ATTACK_BLOCK_METHOD =
				(Constants.CreditAuthAtackBlockMethod?)GetAppSetting("Credit_AUth_Atack_Block_Method", typeof(Constants.CreditAuthAtackBlockMethod));
		}

		/// <summary>
		/// アプリケーション設定情報を初期化(Front系)
		/// </summary>
		public void InitializeFrontCommon()
		{
			// エラーメッセージファイルパス
			Constants.PHYSICALFILEPATH_ERROR_MESSAGE_XMLS.Add(AppDomain.CurrentDomain.BaseDirectory + "Xml/Message/ErrorMessages_Common.xml");
			Constants.PHYSICALFILEPATH_ERROR_MESSAGE_XMLS.Add(AppDomain.CurrentDomain.BaseDirectory + "Xml/Message/ErrorMessages_Commerce.xml");

			// 一覧表示系の設定
			Constants.KBN_REQUEST_DISP_IMG_KBN_DEFAULT = GetAppStringSetting("Setting_ProductListImgKbn_Default");
			Constants.UNDISPLAY_NOSTOCK_PRODUCT_ENABLED = GetAppBoolSetting("Undisplay_Nostock_Product_Enabled");
			Constants.DISPLAY_NOSTOCK_PRODUCT_BOTTOM = GetAppBoolSetting("Display_NoStock_Product_Bottom");

			// 頒布会コース一覧：表示件数
			Constants.DISP_LIST_CONTENTS_COUNT_SUBSCRIPTION_BOX_LIST = GetAppIntSetting("DISP_LIST_CONTENTS_COUNT_SUBSCRIPTION_BOX_LIST");
			// 頒布会コース詳細：商品表示件数
			Constants.DISP_LIST_CONTENTS_COUNT_SUBSCRIPTION_BOX_PRODUCT_LIST = GetAppIntSetting("DISP_LIST_CONTENTS_COUNT_SUBSCRIPTION_BOX_PRODUCT_LIST");

			// ProductView取得項目(商品バリエーション一覧用)カラム名の設定
			Constants.PRODUCT_VARIATIONLIST_SELECTCOLUMNS = GetAppStringSetting("ProductVariationList_SelectColumns");

			// 商品一覧のデフォルト設定
			if (Constants.UNDISPLAY_NOSTOCK_PRODUCT_ENABLED)
			{
				// デフォルト：在庫ありのみ
				Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_DEFAULT = Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_UNDISPLAY_NOSTOCK;
			}
			else
			{
				// デフォルト：在庫無しも含む
				if (Constants.DISPLAY_NOSTOCK_PRODUCT_BOTTOM)
				{
					// デフォルト：在庫あり優先
					Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_DEFAULT = Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_DISPLAY_NOSTOCK_BOTTOM;
				}
				else
				{
					// デフォルト：在庫有無無関係
					Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_DEFAULT = Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_DISPLAY_NOSTOCK;
				}
			}

			// パスワードリマインダー
			Constants.PASSWORDRIMINDER_AUTHITEM = (Constants.PasswordReminderAuthItem?)GetAppSetting("PasswordReminder_AuthItem", typeof(Constants.PasswordReminderAuthItem));

			// アクセスログ設定
			Constants.W2MP_ACCESSLOG_BEGIN_DATETIME = GetAppDateTimeSetting("AccessLog_BeginDateTime");
			Constants.W2MP_ACCESSLOG_ENABLED = GetAppBoolSetting("AccessLog_Enabled");

			// SingleSignOn設定
			Constants.ALLOW_SINGLE_SIGN_ON_URL_REFERRER = GetAppStringSettingList("Setting_AllowSingleSignOnReferrer", StringSplitOptions.RemoveEmptyEntries)
															.Select(url => new Uri(url)).ToList();
			Constants.ENCRYPTION_SINGLE_SIGN_ON_KEY = GetAppStringSetting("Setting_EncryptionSingleSignOnKey");
			Constants.ENCRYPTION_SINGLE_SIGN_ON_IV = GetAppStringSetting("Setting_EncryptionSingleSignOnIV");

			// その他
			Constants.PROTOCOL_HTTP = GetAppStringSetting("Site_ProtocolHttp");
			Constants.PROTOCOL_HTTPS = GetAppStringSetting("Site_ProtocolHttps");
			Constants.RECOMMEND_DISP_SETTING_FILE_PATH = GetAppStringSetting("RecommendDispSettingFilePath");
			Constants.RECOMMEND_PATTERN_SETTING_FILE_PATH = GetAppStringSetting("RecommendPatternSettingFilePath");
			Constants.DATE_EXPIRED_ADD_MONTH_ARRIVAL = GetAppIntSetting("DateExpired_Add_Month_Arrival");
			Constants.DATE_EXPIRED_ADD_MONTH_RELEASE = GetAppIntSetting("DateExpired_Add_Month_Release");
			Constants.DATE_EXPIRED_ADD_MONTH_RESALE = GetAppIntSetting("DateExpired_Add_Month_Resale");
			Constants.LOGIN_ID_COOKIE_LIMIT_DAYS = GetAppIntSetting("LoginIdCookie_LimitDays");
			Constants.CART_ID_COOKIE_LIMIT_DAYS = GetAppIntSetting("CartIdCookie_LimitDays");
			Constants.SHOW_OUT_OF_STOCK_ITEMS = GetAppBoolSetting("Show_Out_Of_Stock_Items");
			// 定期：定期解約時アラート表示ステータス（カンマ区切り。空白で非利用となる。）
			Constants.AlertOrderStatusForCancelFixedPurchase_ENABLED
				= GetAppStringSetting("AlertOrderStatusForCancelFixedPurchase").Split(',');

			// 注文同梱
			Constants.ORDERCOMBINE_ALLOW_PAYMENT_KBN_FRONT = GetAppStringSetting("OrderCombine_AllowPaymentKBN_Front").Split(',');
			Constants.ORDERCOMBINE_ALLOW_ORDER_STATUS_FRONT = GetAppStringSetting("OrderCombine_AllowOrderStatus_Front").Split(',');
			Constants.ORDERCOMBINE_ALLOW_ORDER_DAY_PASSED_FRONT = GetAppIntSetting("OrderCombine_AllowOrderDayPassed_Front");
			Constants.ORDERCOMBINE_ALLOW_SHIPPING_DAY_BEFORE_FRONT = GetAppIntSetting("OrderCombine_AllowShippingDayBefore_Front");
			Constants.ORDERCOMBINE_ALLOW_ORDER_PAYMENT_STATUS_FRONT = GetAppStringSetting("OrderCombine_AllowOrderPaymentStatus_Front").Split(',');
			Constants.ORDERCOMBINE_DENY_SHIPPING_METHOD_FRONT = GetAppStringSetting("OrderCombine_DenyShippingMethod_Front").Split(',');
			Constants.ORDERCOMBINE_DENY_SHIPPING_ID_FRONT = GetAppStringSetting("OrderCombine_DenyShippingId_Front").Split(',');

			// キャプチャ認証系
			Constants.RECAPTCHA_SITE_KEY = GetAppStringSetting("Recaptcha_Site_Key");
			Constants.RECAPTCHA_SECRET_KEY = GetAppStringSetting("Recaptcha_Secret_Key");

			// DataSchemaの再設定(フロントでは管理画面の言語ロケールIDを使用しないため)
			InitializeFrontDataSchema();
			// DataSchemaSetting監視起動（DataSchema置換設定初期化処理セット）
			FileUpdateObserver.GetInstance().AddObservation(
				Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, @"DataSchema\"),
				"ReplaceTagSetting.xml",
				InitializeFrontDataSchema);

			Constants.DISALLOW_SAMESITE_NONE_USERAGENTSPATTERN = GetAppStringSetting("DisAllow_SameSite_None_UserAgentsPattern");

			Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE = GetAppBoolSetting("LandingCart_UserRegisterWhenOrderComplete");

			Constants.BRAND_SESSION_ENABLED = GetAppBoolSetting("Brand_Session_Enabled");

			Constants.REDIRECT_TO_DEFAULT_BRAND_TOP = GetAppBoolSetting("Redirect_To_Default_Brand_Top");

			// 商品一覧：商品バリエーション検索区分画像区分
			Constants.SETTING_PRODUCT_LIST_SEARCH_KBN = GetAppBoolSetting("Setting_ProductListSearchKbn");

			// 頒布会：商品選択方法
			Constants.SUBSCRIPTION_BOX_PRODUCT_CHANGE_METHOD
				= (SubscriptionBoxProductChangeMethod)GetAppEnumSetting("Subscription_Box_Product_Change_Method", typeof(SubscriptionBoxProductChangeMethod));

			// 商品一覧：モーダル画面を利用するか
			Constants.USE_MODAL_PRODUCT_LIST = GetAppBoolSetting("Use_Modal_Product_List");
			// 商品一覧：無限ロードを利用するか
			Constants.USE_INFINITE_LOAD_PRODUCT_LIST = GetAppBoolSetting("Use_Infinite_Load_Product_List");
			// 商品一覧：無限ロード利用時、画面内に表示する商品数
			Constants.DISPLAY_PRODUCT_COUNT_FOR_INFINITE_LOAD = GetAppIntSetting("Display_Product_Count_For_Infinite_Load");

		}

		/// <summary>
		/// アプリケーション設定情報を初期化(Manager系)
		/// </summary>
		public void InitializeManagerCommon()
		{
			// メニューXMLファイル
			Constants.PHYSICALDIRPATH_MANAGER_MENU_XML = AppDomain.CurrentDomain.BaseDirectory + Constants.FILE_XML_MANAGER_MENU_SETTING;
			// 機能一覧XMLファイル
			Constants.PHYSICALDIRPATH_MANAGER_PAGE_INDEX_LIST_XML = AppDomain.CurrentDomain.BaseDirectory + Constants.FILE_XML_MANAGER_PAGE_INDEX_LIST;

			// エラーメッセージファイルパス
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Xml/Message/ErrorMessages_Common.xml"))
			{
				Constants.PHYSICALFILEPATH_ERROR_MESSAGE_XMLS.Add(AppDomain.CurrentDomain.BaseDirectory + "Xml/Message/ErrorMessages_Common.xml");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Xml/Message/ErrorMessages_Commerce.xml"))
			{
				Constants.PHYSICALFILEPATH_ERROR_MESSAGE_XMLS.Add(AppDomain.CurrentDomain.BaseDirectory + "Xml/Message/ErrorMessages_Commerce.xml");
			}

			// 一覧表示系の設定
			Constants.CONST_DISP_CONTENTS_DEFAULT_LIST = GetAppIntSetting("Const_DispListContentsCount_Default");
			Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST = GetAppIntSetting("Const_SearchOverHitCount_Default");

			// その他
			Constants.PROTOCOL_HTTP = GetAppStringSetting("Site_ProtocolHttp");
			Constants.PROTOCOL_HTTPS = GetAppStringSetting("Site_ProtocolHttps");
			Constants.USERSYMBOL_HAS_NOTE = GetAppStringSetting("UserSymbol_HasNote");
			Constants.USERSYMBOL_REPEATER = GetAppStringSetting("UserSymbol_Repeater");

			// 各種オプション利用有無系
			Constants.GOOGLESHOPPING_COOPERATION_OPTION_ENABLED = GetAppBoolSetting("GoogleShoppingOption_Enabled");
			Constants.GRANT_ORDER_POINT_AUTOMATICALLY = GetAppBoolSetting("Point_GrantOrderPointWithShipment");

			// 管理画面デザイン設定
			var managerDesignSetting = GetAppStringSetting("Manager_DesignSetting");
			Constants.MANAGER_DESIGN_SETTING = string.IsNullOrEmpty(managerDesignSetting) ? "w2" : managerDesignSetting;
			Constants.MANAGER_DESIGN_DECORATE_ICON_FILENAME = GetAppStringSetting("Manager_Design_Decorate_Icon_FileName");
			Constants.MANAGER_DESIGN_DECORATE_MESSAGE = GetAppStringSetting("Manager_Dsign_Decorate_Message");

			// サポートサイト連携
			Constants.SUPPORT_SITE_URL = GetAppStringSetting("SupportSiteUrl");
			Constants.COOPERATION_SUPPORT_SITE = GetAppBoolSetting("CooperationSupportSite");
			Constants.W2UNIFIED_TERMS_OF_SERVICE_URL = GetAppStringSetting("W2Unified_TermsOfServiceUrl");
			Constants.W2REPEAT_TERMS_OF_SERVICE_URL = GetAppStringSetting("W2Repeat_TermsOfServiceUrl");

			// マスタ出力設定のエクセル形式（.xls：Excel2000/2002/2003、.xlsx：Excel2007以降）
			Constants.MASTEREXPORT_EXCELFORMAT = GetAppStringSetting("MasterExport_ExcelFormat");
			// CSVマスタ出力時の文字コード
			Constants.MASTEREXPORT_CSV_ENCODE = GetAppStringSetting("MasterExport_Csv_Encode");

			// 注文同梱
			Constants.ORDERCOMBINE_ALLOW_PAYMENT_KBN_MANAGER = GetAppStringSetting("OrderCombine_AllowPaymentKBN_Manager").Split(',');
			Constants.ORDERCOMBINE_ALLOW_ORDER_STATUS_MANAGER = GetAppStringSetting("OrderCombine_AllowOrderStatus_Manager").Split(',');
			Constants.ORDERCOMBINE_ALLOW_ORDER_DAY_PASSED_MANAGER = GetAppIntSetting("OrderCombine_AllowOrderDayPassed_Manager");
			Constants.ORDERCOMBINE_ALLOW_SHIPPING_DAY_BEFORE_MANAGER = GetAppIntSetting("OrderCombine_AllowShippingDayBefore_Manager");
			Constants.ORDERCOMBINE_ALLOW_ORDER_PAYMENT_STATUS_MANAGER = GetAppStringSetting("OrderCombine_AllowOrderPaymentStatus_Manager").Split(',');
			Constants.ORDERCOMBINE_DENY_SHIPPING_METHOD_MANAGER = GetAppStringSetting("OrderCombine_DenyShippingMethod_Manager").Split(',');
			Constants.ORDERCOMBINE_DENY_SHIPPING_ID_MANAGER = GetAppStringSetting("OrderCombine_DenyShippingId_Manager").Split(',');
			Constants.FIXEDPURCHASECOMBINE_ALLOW_FIXEDPURCHASE_STATUS = GetAppStringSetting("FixedPurchaseCombine_AllowFixedPurchaseStatus_Manager").Split(',');
			Constants.FIXEDPURCHASECOMBINE_ALLOW_PAYMENT_STATUS = GetAppStringSetting("FixedPurchaseCombine_AllowPaymentStatus_Manager").Split(',');

			// Order search max length url
			Constants.ORDER_SEARCH_MAX_LENGTH_URL = GetAppIntSetting("Order_Search_Max_Length_Url");

			// CSトップページグループタスク表示モード
			Constants.SETTING_CSTOP_GROUP_TASK_DISPLAY_MODE = (Constants.GroupTaskDisplayModeType)GetAppSetting("CsTop_GroupTaskDisplayMode", typeof(Constants.GroupTaskDisplayModeType));

			// Execution EXE: Reauth
			Constants.PHYSICALDIRPATH_REAUTH_EXE = GetAppStringSetting("Program_Reauth");

			// マスタファイル取込実行ＥＸＥ
			Constants.PHYSICALDIRPATH_MASTERUPLOAD_EXE = GetAppStringSetting("Program_MasterFileImport");

			// 日別出荷予測レポート実行ＥＸＥ
			Constants.PHYSICALDIRPATH_CREATEREPORT_EXE = GetAppStringSetting("CreateReport");

			// 2段階認証オプション（利用：TRUE 非利用：FALSE)
			Constants.TWO_STEP_AUTHENTICATION_OPTION_ENABLED = GetAppBoolSetting("2-Step_Authentication");
			// 2段階認証を行わないIPアドレス(カンマ区切り)
			Constants.TWO_STEP_AUTHENTICATION_EXCLUSION_IPADDRESS = GetAppStringSetting("2_Step_Authentication_Exclusion_Ipaddress");
			// 2段階認証有効期限(日)
			Constants.TWO_STEP_AUTHENTICATION_EXPIRATION_DATE = (int?)GetAppSetting("2-Step_Authentication_Expiration_Date", typeof(int?)) ?? 0;
			// 2段階認証コード有効期限(分)
			Constants.TWO_STEP_AUTHENTICATION_DEADLINE = (int?)GetAppSetting("2-Step_Authentication_Deadline", typeof(int?)) ?? 20;

			// 通貨コード（元）
			Constants.EXCHANGERATE_DSTCURRENCYCODES = GetAppStringSetting("ExchangeRate_DstCurrencyCodes")
				.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			// 通貨コード（先）
			Constants.EXCHANGERATE_SRCCURRENCYCODES = GetAppStringSetting("ExchangeRate_SrcCurrencyCodes")
				.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			//WebAPI接続先URL
			Constants.EXCHANGERATE_BASEURL = GetAppStringSetting("ExchangeRate_BaseUrl");
			//アクセスキ
			Constants.EXCHANGERATE_ACCESSKEY = GetAppStringSetting("ExchangeRate_AccessKey");

			// Yahoo API - 認可コードを取得するためにアクセスするエンドポイント (Authorizationエンドポイント)
			Constants.YAHOO_API_AUTH_API_URL = GetAppStringSetting("Yahoo_Api_Auth_Api_Url");

			// マスタアップロード/ダウンロード時区切り文字
			var delimiter = GetAppStringSetting("MasterFileImport_Delimiter");
			Constants.MASTERFILEIMPORT_DELIMITER = StringUtility.ToValueIfNullOrEmpty(delimiter, " ");

			// データ結合マスタ
			Constants.MASTEREXPORT_DATABINDING_OPTION_ENABLE = GetAppBoolSetting("MasterExport_DataBindingOption_Enabled");
			Constants.MASTEREXPORT_DATABINDING_OUTPUT_LINES_LIMIT = GetAppIntSetting("MasterExport_DataBinding_OutputLinesLimit");

			// 統合解除対象ユーザーの受注件数が何件以上の場合非同期実行するか
			Constants.ORDER_QUANTITY_TO_EXECUTE_ASYNC_CANCEL_USER_INTEGRATE = GetAppIntSetting("Order_Quantity_To_Execute_Async_Cancel_User_Integrate");
		}

		/// <summary>
		/// アプリケーション設定情報を初期化(Batch系)
		/// </summary>
		public void InitializeBatchCommon()
		{
			// 現状C100_BatchCommonで設定する項目なし //
		}

		/// <summary>
		/// Initialize CsMail Receiver
		/// </summary>
		public void InitializeCsMailReceiver()
		{
			// Exchange Web Services URL
			Constants.EXCHANGE_WEB_SERVICES_URL = GetAppStringSetting("Exchange_Web_Services_URL");
			// Exchange Web Services scopes
			Constants.EXCHANGE_WEB_SERVICES_SCOPES = GetAppStringSetting("Exchange_Web_Services_Scopes").Split(',');
		}

		/// <summary>
		/// アプリケーション設定取得（文字列）
		/// </summary>
		/// <param name="strKey">設定キー</param>
		/// <returns>文字列を変換した結果</returns>
		public string GetAppStringSetting(string strKey)
		{
			return (string)GetAppSetting(strKey, typeof(string));
		}
		/// <summary>
		/// アプリケーション設定取得（数値）
		/// </summary>
		/// <param name="strKey">設定キー</param>
		/// <returns>文字列を変換した結果</returns>
		public int GetAppIntSetting(string strKey)
		{
			return (int)GetAppSetting(strKey, typeof(int));
		}
		/// <summary>
		/// アプリケーション設定取得（数値Null許容）
		/// </summary>
		/// <param name="strKey">設定キー</param>
		/// <returns>文字列を変換した結果</returns>
		public int? GetAppIntOrNullSetting(string strKey)
		{
			return (int?)GetAppSetting(strKey, typeof(int?));
		}
		/// <summary>
		/// アプリケーション設定取得（bool）
		/// </summary>
		/// <param name="strKey">設定キー</param>
		/// <returns>文字列を変換した結果</returns>
		public bool GetAppBoolSetting(string strKey)
		{
			return (bool)GetAppSetting(strKey, typeof(bool));
		}
		/// <summary>
		/// アプリケーション設定取得（日付）
		/// </summary>
		/// <param name="strKey">設定キー</param>
		/// <returns>文字列を変換した結果</returns>
		public DateTime GetAppDateTimeSetting(string strKey)
		{
			return (DateTime)GetAppSetting(strKey, typeof(DateTime));
		}
		/// <summary>
		/// アプリケーション設定取得（メールアドレス）
		/// </summary>
		/// <param name="settingKey">設定キー</param>
		/// <returns>文字列を変換した結果</returns>
		public MailAddress GetAppMailAddressSetting(string settingKey)
		{
			return (MailAddress)GetAppSetting(settingKey, typeof(MailAddress));
		}

		/// <summary>
		/// アプリケーション設定取得（ENUM）
		/// </summary>
		/// <typeparam name="T">型</typeparam>
		/// <param name="settingKey">設定キー</param>
		/// <param name="enumType">列挙体タイプ</param>
		/// <returns>文字列を変換した結果</returns>
		public object GetAppEnumSetting(string settingKey, Type enumType)
		{
			var str = GetAppStringSetting(settingKey);
			var result = Enum.Parse(enumType, str, ignoreCase: true);
			return result;
		}

		/// <summary>
		/// アプリケーション設定取得
		/// </summary>
		/// <param name="strKey">設定キー</param>
		/// <param name="type">型</param>
		/// <returns>文字列を変換した結果</returns>
		public object GetAppSetting(string strKey, Type type)
		{
			//------------------------------------------------------
			// 設定取得
			//------------------------------------------------------
			string strValue = (string)this.ConfigurationSettingInfo[strKey];
			if (strValue == null)
			{
				throw new NullReferenceException("アプリケーション設定\"" + strKey + "\"が見つかりませんでした。");
			}

			// 設定情報リストに型代入
			if (this.SettingNodeList != null)
			{
				var targetSetting = this.SettingNodeList.FirstOrDefault(s => s.Key == strKey);

				if (targetSetting != null)
				{
					targetSetting.DataType = type;
				}
			}

			//------------------------------------------------------
			// 型変換
			//------------------------------------------------------
			if (type == typeof(int))
			{
				int iResult = 0;
				if (int.TryParse(strValue, out iResult) == false)
				{
					throw new ApplicationException("アプリケーション設定\"" + strKey + "\"がint値に変換できませんでした。");
				}
				return iResult;
			}
			else if (type == typeof(int?))
			{
				if (strValue == "") return null;
				int result;
				if (int.TryParse(strValue, out result) == false)
				{
					throw new ApplicationException("アプリケーション設定\"" + strKey + "\"がint?値に変換できませんでした。");
				}
				return result;
			}
			else if (type == typeof(bool))
			{
				bool blResult = false;
				if (bool.TryParse(strValue, out blResult) == false)
				{
					throw new ApplicationException("アプリケーション設定\"" + strKey + "\"がbool値に変換できませんでした。");
				}
				return blResult;
			}
			else if (type == typeof(DateTime))
			{
				DateTime dtResult = new DateTime(0);
				if (DateTime.TryParse(strValue, out dtResult) == false)
				{
					throw new ApplicationException("アプリケーション設定\"" + strKey + "\"がDateTime値に変換できませんでした。");
				}
				return dtResult;
			}
			else if (type == typeof(MailAddress))
			{
				MailAddress result = new MailAddress(strValue);
				// HACK: MailAddressの変換チェックもしたい
				return result;
			}
			else if (type.IsEnum)
			{
				if (strValue == "")
				{
					return null;
				}
				foreach (Enum e in Enum.GetValues(type))
				{
					if (e.ToString().ToUpper() == strValue.ToUpper())
					{
						return e;
					}
				}
				throw new ApplicationException("アプリケーション設定\"" + strKey + "\"が" + type.ToString() + "値に変換できませんでした。");
			}
			else if (type == typeof(decimal))
			{
				var result = 0m;
				if (decimal.TryParse(strValue, out result) == false)
				{
					throw new ApplicationException("アプリケーション設定\"" + strKey + "\"がdecimal値に変換できませんでした。");
				}

				return result;
			}

			return strValue;
		}
		/// <summary>
		/// 文字列型のリストを返却する
		/// </summary>
		/// <param name="strKey">設定キー</param>
		/// <param name="splitOption">分割オプション</param>
		/// <returns>文字列リスト</returns>
		/// <remarks>リストの登録順は、Keyの連番順と一致する。</remarks>
		public List<string> GetAppStringSettingList(string strKey, StringSplitOptions splitOption = StringSplitOptions.None)
		{
			List<string> lSetting = new List<string>();

			if (this.ConfigurationSettingInfo.Contains(strKey) == false)
			{
				throw new NullReferenceException("アプリケーション設定\"" + strKey + "\"が見つかりませんでした。");
			}

			// Keyの連番順にリストへ格納していく
			string[] separator = { "," };
			foreach (string strValue in this.ConfigurationSettingInfo[strKey].ToString().Split(separator, splitOption))
			{
				lSetting.Add(strValue);
			}

			return lSetting;
		}
		/// <summary>
		/// 数値型のリストを返却する
		/// </summary>
		/// <param name="strKey">設定キー</param>
		/// <returns>数値列リスト</returns>
		/// <remarks>リストの登録順は、Keyの連番順と一致する。</remarks>
		public List<int> GetAppIntSettingList(string strKey)
		{
			List<int> lSetting = new List<int>();

			if (this.ConfigurationSettingInfo.Contains(strKey) == false)
			{
				throw new NullReferenceException("アプリケーション設定\"" + strKey + "\"が見つかりませんでした。");
			}

			if (this.ConfigurationSettingInfo[strKey].ToString() == "")
			{
				return lSetting;
			}

			// Keyの連番順にリストへ格納していく
			foreach (string strValue in this.ConfigurationSettingInfo[strKey].ToString().Split(','))
			{
				int iValue;
				if (int.TryParse(strValue, out iValue) == false)
				{
					throw new NullReferenceException("アプリケーション設定\"" + strKey + "\"の文字列が数値列に変換できませんでした。");
				}

				lSetting.Add(iValue);
			}

			return lSetting;
		}

		/// <summary>
		/// メールアドレスのリストを返却する
		/// </summary>
		/// <param name="settingKey">設定キー</param>
		/// <returns>メールアドレスリスト</returns>
		public List<MailAddress> GetAppMailAddressSettingList(string settingKey)
		{
			List<MailAddress> mailAddressList = new List<MailAddress>();
			GetAppStringSettingList(settingKey).ForEach(mail => mailAddressList.Add(new MailAddress(mail)));

			return mailAddressList;
		}

		/// <summary>
		/// Get application settings (decimal)
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns>Result of converting the string</returns>
		public decimal GetAppDecimalSetting(string key)
		{
			return (decimal)GetAppSetting(key, typeof(decimal));
		}

		/// <summary>
		/// リピートラインオプション列挙値を返却する
		/// </summary>
		/// <returns>リピートラインオプション列挙値</returns>
		public Constants.RepeatLineOption GetAppRepeatLineOptionSetting()
		{
			// ユーザー連携とLINE送信両方使用
			if (Constants.LINE_COOPERATION_OPTION_ENABLED && GetAppBoolSetting("LINE_Messaging_Option_Enabled"))
			{
				return Constants.RepeatLineOption.CooperationAndMessaging;
			}
			// ユーザー連携のみ使用
			else if (Constants.LINE_COOPERATION_OPTION_ENABLED && (GetAppBoolSetting("LINE_Messaging_Option_Enabled") == false))
			{
				return Constants.RepeatLineOption.CooperationOnly;
			}
			// 不使用
			else
			{
				return Constants.RepeatLineOption.Off;
			}
		}

		/// <summary>コンフィグディレクトリパス</summary>
		public string ConfigDirPath { get; set; }
		/// <summary>コンフィグファイルパスリスト</summary>
		public List<string> ConfigFilePathList { get; set; }
		/// <summary>アプリケーション設定値情報</summary>
		private Hashtable ConfigurationSettingInfo { get; set; }
		/// <summary>読み込み区分リスト(区分でソート済み）</summary>
		public List<ReadKbn> ReadKbnList { get; set; }
		/// <summary>環境タグリスト(key:タグ value:値)</summary>
		public Dictionary<string, string> EnvTagList { get; set; }
		/// <summary>設定情報リスト</summary>
		public List<SettingNode> SettingNodeList { get; private set; }
	}

	/// <summary>
	/// XMLから読み込んだ設定値を一旦保存しておくクラス
	/// </summary>
	public class AppConfigs
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AppConfigs()
		{
			this.SettingList = new Dictionary<string, string>();
			this.ReadKbn = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="rkKbn">読み込み区分</param>
		public AppConfigs(ConfigurationSetting.ReadKbn rkKbn)
			: this()
		{
			this.ReadKbn = rkKbn;
		}

		/// <summary>読み込み区分</summary>
		public ConfigurationSetting.ReadKbn? ReadKbn { get; set; }
		/// <summary>設定値</summary>
		public Dictionary<string, string> SettingList { get; set; }
	}

	/// <summary>
	/// 設定情報クラス
	/// </summary>
	[Serializable]
	public class SettingNode
	{
		/// <summary>読取区分</summary>
		public string ReadKbn { get; set; }
		/// <summary>キー</summary>
		public string Key { get; set; }
		/// <summary>コメント</summary>
		public string Comment { get; set; }
		/// <summary>値</summary>
		public string Value { get; set; }
		/// <summary>AppAll.Configの値</summary>
		public string AppAllValue { get; set; }
		/// <summary>データ型</summary>
		public Type DataType { get; set; }
		/// <summary>変更前の値</summary>
		public string BeforeValue { get; set; }
		/// <summary>優先コンフィグ</summary>
		public string PrimaryConfig { get; set; }
	}
}
