/*
=========================================================================================================
  Module      : Constants setting(Constants_Setting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Xml;
using w2.Common.Logger;

namespace TempoStar.w2.Commerce.ExternalAPI
{
	/// <summary>
	/// Constants class
	/// </summary>
	public static class Constants_Setting
	{
		#region Settings
		// Setting FTP: Ftp host
		public static string SETTING_FTP_HOST = TempoStarSetting.GetSettingFromXml("host");

		// Setting FTP: Ftp user name
		public static string SETTING_FTP_USER_NAME = TempoStarSetting.GetSettingFromXml("user_name");

		// Setting FTP: Ftp password
		public static string SETTING_FTP_USER_PASSWORD = TempoStarSetting.GetSettingFromXml("password");

		// Setting FTP: Ftp user active
		public static bool SETTING_FTP_USER_ACTIVE = Convert.ToBoolean(TempoStarSetting.GetSettingFromXml("user_active"));

		// Setting FTP: Ftp enable ssl
		public static bool SETTING_FTP_ENABLE_SSL = Convert.ToBoolean(TempoStarSetting.GetSettingFromXml("enable_ssl"));

		// Setting FTP: Upload file encode
		public static string SETTING_FTP_FILE_ENCODE = TempoStarSetting.GetSettingFromXml("file_encode");

		// Setting FTP: Shop code directory
		public static string SETTING_FTP_SHOP_CODE_DIRECTORY = TempoStarSetting.GetSettingFromXml("shop_code_directory");

		// Setting FTP: Upload file header
		public static string SETTING_FTP_FILE_HEADER = TempoStarSetting.GetSettingFromXml("ftp_file_header");

		// Setting FTP: Order upload directory
		public static string SETTING_FTP_ORDER_UPLOAD_DIRECTORY = TempoStarSetting.GetSettingFromXml("order_upload_directory");

		// Setting FTP: Order status download directory
		public static string SETTING_FTP_ORDER_STATUS_DOWNLOAD_DIRECTORY = TempoStarSetting.GetSettingFromXml("order_status_download_directory");

		// Setting FTP: Stock download directory
		public static string SETTING_FTP_STOCK_DOWNLOAD_DIRECTORY = TempoStarSetting.GetSettingFromXml("stock_download_directory");

		// Setting FTP: Ftp log time limit
		public static string SETTING_FTP_LOG_TIME_LIMIT = TempoStarSetting.GetSettingFromXml("log_time_limit");

		// Setting FTP: Capacity limit (Megabyte)
		public static string SETTING_FTP_CAPACITY_LIMIT = TempoStarSetting.GetSettingFromXml("capacity_limit");

		// Setting FTP: Mail address length max
		public const int SETTING_FTP_MAIL_ADDRESS_LENGTH_MAX = 255;

		// Setting local: Active directory
		public static string SETTING_LOCAL_ACTIVE_DIRECTORY = "Active";

		// Setting local: Error directory
		public static string SETTING_LOCAL_ERROR_DIRECTORY = "Error";

		// Setting local: Success directory
		public static string SETTING_LOCAL_SUCCESS_DIRECTORY = "Success";

		// Setting local: Mail template
		public static string SETTING_LOCAL_MAIL_TEMPLATE = "10000082";

		// Setting local: Shop id
		public const string SHOP_ID = "0";
		#endregion

		#region Messages
		// Export order fail
		public static string ERRMSG_EXPORT_ORDER_EXPORT_FAIL = "受注情報出力時にエラーが発生したため、ファイルが出力できませんでした。<br />{@@ error @@}<br />システム管理者までご連絡ください。";

		// Export order: File already exists on local
		public static string ERRMSG_EXPORT_ORDER_LOCAL_FILE_ALREADY_EXISTS = "ファイルが既に存在しています。対象ディレクトリを確認してください。";

		// Export order: Full capacity on ftp
		public static string ERRMSG_EXPORT_ORDER_FTP_FULL_CAPACITY = "TEMPOSTARのFTPS領域が不足しはじめています。<br />不要なファイルを削除し、空き領域を増やしてください。";

		// Export order: Connect or upload fail
		public static string ERRMSG_EXPORT_ORDER_CONNECT_OR_UPLOAD_FAIL = "TEMPOSTARへ接続できなかったため、受注情報の送信をスキップしました。<br />[@@ error @@]<br />スキップされた受注情報は次回連携時に合わせて連携されます。<br />このエラーが連続して発生する場合は、システム管理者までご連絡ください。";

		// Export order: File already exists on ftp
		public static string ERRMSG_EXPORT_ORDER_FTP_FILE_ALREADY_EXISTS = "受注情報をTEMPOSTARへ送信しましたが、既に同名のファイルが存在していたため、処理をスキップしました。<br />[@@ error @@]<br />TEMPOSTAR側のファイルを確認してください。<br />このエラーが連続して発生する場合は、システム管理者までご連絡ください。";

		// Import order status: Get or connect fail
		public static string ERRMSG_IMPORT_ORDER_STATUS_GET_OR_CONNECT_FAIL = "TEMPOSTARへ接続できなかったため、注文ステータス連携情報の取得に失敗しました。<br />次回連携時に、今回取得に失敗した注文ステータス連携情報が合わせて取得されます。<br />このエラーが連続して発生する場合は、システム管理者までご連絡ください。";

		// Import order status: File already exists on local
		public static string ERRMSG_IMPORT_ORDER_STATUS_LOCAL_FILE_ALREADY_EXISTS = "受注情報をTEMPOSTARから受信しましたが、既に同名のファイルが存在していたため、処理をスキップしました。<br />[@@ error @@]<br />ファイルを確認してください。<br />このエラーが連続して発生する場合は、システム管理者までご連絡ください。";

		// Import order status: Update fail
		public static string ERRMSG_IMPORT_ORDER_STATUS_UPDATE_FAIL = "注文ステータス連携情報取込処理時にエラーが発生しました。<br />{@@ error @@}<br />システム管理者までご連絡ください。";

		// Import stock: Get or connect fail
		public static string ERRMSG_IMPORT_STOCK_GET_OR_CONNECT_FAIL = "TEMPOSTARへ接続できなかったため、在庫情報の取得に失敗しました。<br />次回連携時に、今回取得に失敗した在庫情報が合わせて取得されます。<br/>このエラーが連続して発生する場合は、システム管理者までご連絡ください。";

		// Import stock: File already exists on local
		public static string ERRMSG_IMPORT_STOCK_LOCAL_FILE_ALREADY_EXISTS = "在庫情報をTEMPOSTARから受信しましたが、<br />既に同名のファイルが存在していたため、処理をスキップしました。<br />[ファイル名を表示]<br />ファイルを確認してください。<br />このエラーが連続して発生する場合は、システム管理者までご連絡ください。";

		// Import stock: Update fail
		public static string ERRMSG_IMPORT_STOCK_UPDATE_FAIL = "在庫情報取込処理時にエラーが発生しました。<br />{@@ error @@}<br />システム管理者までご連絡ください。";
		#endregion
	}
}