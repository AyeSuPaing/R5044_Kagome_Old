/*
=========================================================================================================
  Module      : マスタ取込バッチ用定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Commerce.Batch.MasterFileImport
{
	/// <summary>
	/// Constants の概要の説明です。
	/// </summary>
	public class Constants : w2.App.Common.Constants
	{
		//*****************************************************************************************
		// マスタ取込バッチ用
		//*****************************************************************************************
		//=========================================================================================
		// フィールドヘッダ
		//=========================================================================================
		public const string IMPORT_HEADER_FIELD								= "TBL";			// テーブル操作

		//=========================================================================================
		// 取込区分
		//=========================================================================================
		public const string IMPORT_KBN										= "_import_kbn_";	// 取込区分
		public const string IMPORT_KBN_INSERT_UPDATE						= "IU";				// Insert/Update
		public const string IMPORT_KBN_DELETE								= "D";				// Delete

		//=========================================================================================
		// 空文字入力が許されないフィールド
		//=========================================================================================
		public const string FIELD_NULLABLE_COLUMNS = ",int,bigint,decimal,datetime,";

		//=========================================================================================
		// 完了ファイル付加コード（例：成功ファイル・・・200512315959_S_00001.csv）
		//=========================================================================================
		public const string IMPORT_RESULT_KBN_SUCCESS						= "_S_";			// 成功
		public const string IMPORT_RESULT_KBN_FAILED						= "_F_";			// 失敗

		//=========================================================================================
		// メールテンプレート
		//=========================================================================================
		public const string IMPORT_MAILTEMPLATE_KEY_MASTER_NAME				= "master_name";	// "商品マスタ" or "商品カテゴリマスタ" or "在庫マスタ"
		public const string IMPORT_MAILTEMPLATE_KEY_FILE_NAME				= "file_name";		// "001.csv"など
		public const string IMPORT_MAILTEMPLATE_KEY_RESULT					= "result";			// "成功" or "失敗"
		public const string IMPORT_MAILTEMPLATE_MSG							= "message";		// メッセージ
		/// <summary>Import mail template key migration name</summary>
		public const string IMPORT_MAILTEMPLATE_KEY_MIGRATION_NAME = "migration_name";

		//=========================================================================================
		// エラーログファイルURLパラメータ
		//=========================================================================================
		public const string REQUEST_KEY_MASTERIMPORTOUTPUTLOG_SHOP_ID = "sid";		// 店舗ID
		public const string REQUEST_KEY_MASTERIMPORTOUTPUTLOG_MASTER = "ms";		// マスタ名
		public const string REQUEST_KEY_MASTERIMPORTOUTPUTLOG_FILE_NAME = "fn";		// ファイル名

		//=========================================================================================
		// 更新者署名
		//=========================================================================================
		public const string IMPORT_LAST_CHANGED								= "batch";
		public const string FIELD_IMPORT_LAST_CHANGED = "last_changed"; // 最終更新者フィールド名
		/// <summary>Field import date created</summary>
		public const string FIELD_IMPORT_DATE_CREATED = "date_created";
		/// <summary>Field import date changed</summary>
		public const string FIELD_IMPORT_DATE_CHANGED = "date_changed";

		//=========================================================================================
		// 各物理ディレクトリ・ファイルパス
		//=========================================================================================
		public static string PHYSICALDIRPATH_MASTERUPLOAD_SETTING_EC = ""; // マスタアップロードセッティングファイルパス（EC用）
		public static string PHYSICALDIRPATH_MASTERUPLOAD_SETTING_MP = ""; // マスタアップロードセッティングファイルパス（MP用）
		public static string PHYSICALDIRPATH_MASTERUPLOAD_SETTING_CMS = ""; // マスタアップロードセッティングファイルパス（CMS用）
		public static string MASTERUPLOAD_ERRORLOGFILE_URL					= "";	// エラーログファイルURL（オペレータ用）
		public static string PHYSICALDIRPATH_PARAMETERSFILE					= "";	// SQLパラメータ設定ファイルパス
		/// <summary>Physical dir path data migration setting file path EC</summary>
		public static string PHYSICALDIRPATH_DATAMIGRATION_SETTINGFILEPATH_EC = string.Empty;
		/// <summary>Physical dir path data migration setting file path CS</summary>
		public static string PHYSICALDIRPATH_DATAMIGRATION_SETTINGFILEPATH_CS = string.Empty;

		//=========================================================================================
		// 商品画像削除用アクション区分
		//=========================================================================================
		public const string ACTION_KBN_DELETE_PRODUCT_IMAGE					= "DeleteProductImage";

		//=========================================================================================
		// メッセージ用
		//=========================================================================================
		// ユーザ系
		public static string INPUTCHECK_NOT_EXIST_MEMBER_RANK_ID = "INPUTCHECK_NOT_EXIST_MEMBER_RANK_ID";

		// ショートURL系
		public static string INPUTCHECK_SHORTURL_IS_EQUAL_TO_LONGURL = "INPUTCHECK_SHORTURL_IS_EQUAL_TO_LONGURL";
		public static string INPUTCHECK_SHORTURL_IS_DIFFERENT_DOMAIN_WITH_LONGURL = "INPUTCHECK_SHORTURL_IS_DIFFERENT_DOMAIN_WITH_LONGURL";
		public static string INPUTCHECK_SHORTURL_IS_SITE_DOMAIN = "INPUTCHECK_SHORTURL_IS_SITE_DOMAIN";

		// クーポン系
		public static string INPUTCHECK_COUPON_DISCOUNT_NECESSARY = "INPUTCHECK_COUPON_DISCOUNT_NECESSARY";
		public static string INPUTCHECK_COUPON_EXPIREDAY_NECESSARY = "INPUTCHECK_COUPON_EXPIREDAY_NECESSARY";
		public static string INPUTCHECK_COUPON_NOT_PUBLISH_COUPON = "INPUTCHECK_COUPON_NOT_PUBLISH_COUPON";
		public static string INPUTCHECK_COUPON_DELETE_IMPOSSIBLE = "INPUTCHECK_COUPON_DELETE_IMPOSSIBLE";
		public static string INPUTCHECK_COUPON_COUPON_ID_NOT_SET = "INPUTCHECK_COUPON_COUPON_ID_NOT_SET";

		// ポイント系
		public const string INPUTCHECK_USERPOINT_POINTRULE_ID_EMPTY = "INPUTCHECK_USERPOINT_POINTRULE_ID_EMPTY";
		public const string INPUTCHECK_USERPOINT_POINTRULE_ID_NOT_EXIST = "INPUTCHECK_USERPOINT_POINTRULE_ID_NOT_EXIST";
		public const string INPUTCHECK_USERPOINT_POINTRULE_BASERULE_NOT_FOR_LIMITED_TERM_POINT = "INPUTCHECK_USERPOINT_POINTRULE_BASERULE_NOT_FOR_LIMITED_TERM_POINT";
		public const string INPUTCHECK_USERPOINT_POINTRULE_CAMPAIGN_NOT_FOR_LIMITED_TERM_POINT = "INPUTCHECK_USERPOINT_POINTRULE_CAMPAIGN_NOT_FOR_LIMITED_TERM_POINT";
		public const string INPUTCHECK_USERPOINT_MISSING_PERIOD = "INPUTCHECK_USERPOINT_MISSING_PERIOD";
		public const string INPUTCHECK_USERPOINT_UNACCEPTABLE_EFFECTIVE_DATE = "INPUTCHECK_USERPOINT_UNACCEPTABLE_EFFECTIVE_DATE";
		public const string INPUTCHECK_USERPOINT_ILLEGAL_PERIOD = "INPUTCHECK_USERPOINT_ILLEGAL_PERIOD";
		//=========================================================================================
		// ユーザマスタ(非入力チェック) ※実際のテーブルは「w2_User」を利用
		//=========================================================================================
		public const string TABLE_USERNOTVALIDATOR = "w2_UserNotValidator";
	}
}
