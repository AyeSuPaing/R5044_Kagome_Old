/*
=========================================================================================================
  Module      : WEBメッセージモジュール(WebMessages.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;

public class WebMessages : w2.App.Common.CommerceMessages
{
	//*****************************************************************************************
	// マネージャーサイト・エラー
	//*****************************************************************************************

	// 店舗IDなし
	public static string ERRMSG_MANAGER_NO_SHOP_ID = "ERRMSG_MANAGER_NO_SHOP_ID";
	// オペレータログイン
	public static string ERRMSG_MANAGER_SHOP_OPERATOR_LOGIN_ERROR = "ERRMSG_MANAGER_SHOP_OPERATOR_LOGIN_ERROR";

	// マスタファイルアップロード
	public static string ERRMSG_MANAGER_MASTERUPLOAD_FILE_UNSELECTED = "ERRMSG_MANAGER_MASTERUPLOAD_FILE_UNSELECTED";
	public static string ERRMSG_MANAGER_MASTERUPLOAD_ALREADY_EXISTS = "ERRMSG_MANAGER_MASTERUPLOAD_ALREADY_EXISTS";
	public static string ERRMSG_MANAGER_MASTERUPLOAD_FILE_NOT_CSV = "ERRMSG_MANAGER_MASTERUPLOAD_FILE_NOT_CSV";
	public static string ERRMSG_MANAGER_MASTERUPLOAD_FILE_UNFIND = "ERRMSG_MANAGER_MASTERUPLOAD_FILE_UNFIND";
	public static string ERRMSG_MANAGER_MASTERUPLOAD_UPLOAD_ERROR = "ERRMSG_MANAGER_MASTERUPLOAD_UPLOAD_ERROR";
	public static string ERRMSG_MANAGER_MASTERUPLOAD_UPLOAD_FIELDS_ERROR = "ERRMSG_MANAGER_MASTERUPLOAD_UPLOAD_FIELDS_ERROR";
	//		// 商品カテゴリー未選択エラー
	//		public static string ERRMSG_MANAGER_PRODUCTCATEGORY_NO_SELECTED_ERROR = "ERRMSG_MANAGER_PRODUCTCATEGORY_NO_SELECTED_ERROR";
	//		// 商品在庫管理更新対象無しエラー
	//		public static string ERRMSG_MANAGER_PRODUCTSTOCK_TARGET_NO_SELECTED_ERROR = "ERRMSG_MANAGER_PRODUCTSTOCK_TARGET_NO_SELECTED_ERROR";
	//		// 店舗配送料データ無しエラー
	//		public static string ERRMSG_MANAGER_PRODUCT_SHOP_SHIPPING_NO_DATA = "ERRMSG_MANAGER_PRODUCT_SHOP_SHIPPING_NO_DATA";
	//		public static string ERRMSG_MANAGER_SHIPPINGZONE_SHOP_SHIPPING_NO_DATA = "ERRMSG_MANAGER_SHIPPINGZONE_SHOP_SHIPPING_NO_DATA";
	//		// 受注管理注文・入力ステータス更新未選択エラー
	//		public static string ERRMSG_MANAGER_ORDER_UPDATE_STATUS_NO_SELECTED_ERROR = "ERRMSG_MANAGER_ORDER_UPDATE_STATUS_NO_SELECTED_ERROR";
	//		// 受注管理更新対象未選択エラー
	//		public static string ERRMSG_MANAGER_ORDER_TARGET_NO_SELECTED_ERROR = "ERRMSG_MANAGER_ORDER_TARGET_NO_SELECTED_ERROR";

	// 店舗管理者マスタパスワード変更
	public static string ERRMSG_MANAGER_SHOP_OPERATOR_NO_OPERATOR_ERROR = "ERRMSG_MANAGER_SHOP_OPERATOR_NO_OPERATOR_ERROR";
	// ログイン失敗制限回数
	public static string ERRMSG_MANAGER_LOGIN_LIMITED_COUNT_ERROR = "ERRMSG_MANAGER_LOGIN_LIMITED_COUNT_ERROR";
	// メニュー表示権限なしエラー
	public static string ERRMSG_MANAGER_LOGIN_UNACCESSABLEUSER_ERROR = "ERRMSG_MANAGER_LOGIN_UNACCESSABLEUSER_ERROR";
	/// <summary>MPオプションOFFエラー</summary>
	public static string ERRMSG_MANAGER_MP_OPTION_DISABLED = "ERRMSG_MANAGER_MP_OPTION_DISABLED";
	// 一覧該当データ無し
	public static string ERRMSG_MANAGER_NO_HIT_LIST = "ERRMSG_MANAGER_NO_HIT_LIST";
	// 一覧該当データオーバー
	public static string ERRMSG_MANAGER_OVER_HIT_LIST = "ERRMSG_MANAGER_OVER_HIT_LIST";
	// 初期表示検索しないメッセージ
	public static string ERRMSG_MANAGER_NOT_SEARCH_DEFAULT = "ERRMSG_MANAGER_NOT_SEARCH_DEFAULT";
	// 詳細該当データ無し
	public static string ERRMSG_MANAGER_NO_HIT_DETAIL = "ERRMSG_MANAGER_NO_HIT_DETAIL";
	// 日付整合性エラー
	public static string ERRMSG_MANAGER_DATE_ADJUSTMENT_ERROR = "ERRMSG_MANAGER_DATE_ADJUSTMENT_ERROR";

	// 不正パラメータエラー
	public static string ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR = "ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR";

	// ポップアップ同時起動時エラー
	public static string ERRMSG_MANAGER_POPUP_IRREGULAR_PARAMETER_ERROR = "ERRMSG_MANAGER_POPUP_IRREGULAR_PARAMETER_ERROR";

	// 会員ランク情報削除不可エラー
	public static string ERRMSG_MANAGER_MEMBER_RANK_DELETE_IMPOSSIBLE_ERROR = "ERRMSG_MANAGER_MEMBER_RANK_DELETE_IMPOSSIBLE_ERROR";
	// 会員ランク登録時、期間、回数や金額で不正パラメータエラー
	public static string ERRMSG_MANAGER_MEMBER_RANK_IRREGULAR_PARAMETER_ERROR = "ERRMSG_MANAGER_MEMBER_RANK_IRREGULAR_PARAMETER_ERROR";

	// ポイント種別マスタデータ無しエラー
	public static string ERRMSG_MANAGER_SHOPPOINT_NO_DATA = "ERRMSG_MANAGER_SHOPPOINT_NO_DATA";

	// ユーザポイント更新対象無しエラー
	public static string ERRMSG_MANAGER_USERPOINT_TARGET_NO_SELECTED_ERROR = "ERRMSG_MANAGER_USERPOINT_TARGET_NO_SELECTED_ERROR";
	// 期間限定ポイントがマイナスになるエラー
	public static string ERRMSG_MANAGER_USERPOINT_NEGATIVE_POINT_UNACCEPTABLE = "ERRMSG_MANAGER_USERPOINT_NEGATIVE_POINT_UNACCEPTABLE";
	// ユーザーポイント有効期間エラー
	public static string ERRMSG_MANAGER_USERPOINT_EXP_DATETIME_NO_TERM = "ERRMSG_MANAGER_USERPOINT_EXP_DATETIME_NO_TERM";
	// ルール有効期間とポイント有効期限の設定から、ポイントが発行されない期間がある
	public static string ERRMSG_MANAGER_POINT_MAY_NOT_BE_ISSUED = "ERRMSG_MANAGER_POINT_MAY_NOT_BE_ISSUED";
	//ポイントスケジュールで使われているエラー
	public static string ERRMSG_MANAGER_USERPOINT_USED = "ERRMSG_MANAGER_USERPOINT_USED";
	// 発行可能クーポン無しエラー
	public static string ERRMSG_MANAGER_USERCOUPON_NO_EXISTS_ERROR = "ERRMSG_MANAGER_USERCOUPON_NO_EXISTS_ERROR";
	// 非会員クーポン発行不可エラー
	public static string ERRMSG_MANAGER_USERCOUPON_NO_PUBLISH_ERROR = "ERRMSG_MANAGER_USERCOUPON_NO_PUBLISH_ERROR";
	// クーポン情報削除不可エラー
	public static string ERRMSG_MANAGER_USERCOUPON_DELETE_IMPOSSIBLE_ERROR = "ERRMSG_MANAGER_USERCOUPON_DELETE_IMPOSSIBLE_ERROR";
	// クーポン情報削除エラー(スケジュール)
	public static string ERRMSG_MANAGER_USERCOUPON_DELETE_IMPOSSIBLE_ERROR_FOR_SCHEDULE_USED = "ERRMSG_MANAGER_USERCOUPON_DELETE_IMPOSSIBLE_ERROR_FOR_SCHEDULE_USED";
	//クーポンスケジュールで使われているエラー
	public static string ERRMSG_MANAGER_USERCOUPON_USED = "ERRMSG_MANAGER_USERCOUPON_USED";
	// ページ管理で使われているエラー
	public static string ERRMSG_MANAGER_PAGEDESIGN_USED = "ERRMSG_MANAGER_PAGEDESIGN_USED";
	/// <summary> 会員ランク変動ルール設定で使われているエラー </summary>
	public static string ERRMSG_MANAGER_MEMBERRANKRULE_USED = "ERRMSG_MANAGER_MEMBERRANKRULE_USED";

	// ブラックリスト型クーポン 利用済みユーザー重複エラー
	public static string ERRMSG_MANAGER_COUPONUSEUSER_BLACKLISTCOUPON_IS_USED = "ERRMSG_MANAGER_COUPONUSEUSER_BLACKLISTCOUPON_IS_USED";
	// ブラックリスト型クーポン 有効な注文IDでないエラー
	public static string ERRMSG_MANAGER_COUPONUSEUSER_IRREGULAR_ORDER_OWNER = "ERRMSG_MANAGER_COUPONUSEUSER_IRREGULAR_ORDER_OWNER";
	// ブラックリスト型クーポン 更新対象クーポン利用ユーザー未選択エラー
	public static string ERRMSG_MANAGER_COUPONUSEUSER_NOT_SELECTED_ERROR = "ERRMSG_MANAGER_COUPONUSEUSER_NOT_SELECTED_ERROR";
	// ブラックリスト型クーポン 利用済ユーザー追加時エラー
	public static string ERRMSG_MANAGER_COUPONUSEUSER_INSERT_NOT_SUCCESS = "ERRMSG_MANAGER_COUPONUSEUSER_INSERT_NOT_SUCCESS";
	// ブラックリスト型クーポン 有効な定期購入IDでないエラー
	public static string ERRMSG_MANAGER_COUPONUSEUSER_IRREGULAR_FIXEDPURCHASE_OWNER = "ERRMSG_MANAGER_COUPONUSEUSER_IRREGULAR_FIXEDPURCHASE_OWNER";

	// クーポン : クーポン割引設定未選択エラー
	public static string ERRMSG_MANAGER_CHECK_DISCOUNT_MONEY_OR_FREE_SHIP = "ERRMSG_MANAGER_CHECK_DISCOUNT_MONEY_OR_FREE_SHIP";

	// キャンペーン有効期間エラー
	public static string ERRMSG_MANAGER_IRREGULAR_CAMPAIGN_EXP_ERROR = "ERRMSG_MANAGER_IRREGULAR_CAMPAIGN_EXP_ERROR";
	public static string ERRMSG_MANAGER_CAMPAIGN_EXP_BGN_END_ERROR = "ERRMSG_MANAGER_CAMPAIGN_EXP_BGN_END_ERROR";
	public static string ERRMSG_MANAGER_CAMPAIGN_EXP_NO_SELECTED_ERROR = "ERRMSG_MANAGER_CAMPAIGN_EXP_NO_SELECTED_ERROR";

	// メール配信文章エラー
	public static string ERRMSG_MANAGER_MAILDISTTEST_NO_TITLE_ERROR = "ERRMSG_MANAGER_MAILDISTTEST_NO_TITLE_ERROR";
	public static string ERRMSG_MANAGER_MAILDISTTEST_NO_BODY_ERROR = "ERRMSG_MANAGER_MAILDISTTEST_NO_BODY_ERROR";
	public static string ERRMSG_MANAGER_MAILDISTTEST_NO_BODY_ERROR_MOBILE = "ERRMSG_MANAGER_MAILDISTTEST_NO_BODY_ERROR_MOBILE";
	public static string ERRMSG_MANAGER_MAILDISTTEXT_USED = "ERRMSG_MANAGER_MAILDISTTEXT_USED";

	// ターゲットリストエラー
	public static string ERRMSG_MANAGER_TARGETLIST_USED = "ERRMSG_MANAGER_TARGETLIST_USED";
	public static string ERRMSG_MANAGER_TARGETLIST_NOT_VALID = "ERRMSG_MANAGER_TARGETLIST_NOT_VALID";

	// コンテンツ管理
	public static string ERRMSG_MANAGER_CONTENTSMANAGER_FILE_UNSELECTED = "ERRMSG_MANAGER_CONTENTSMANAGER_FILE_UNSELECTED";
	public static string ERRMSG_MANAGER_CONTENTSMANAGER_FILE_UNFIND = "ERRMSG_MANAGER_CONTENTSMANAGER_FILE_UNFIND";
	public static string ERRMSG_MANAGER_CONTENTSMANAGER_FILE_ALREADY_EXISTS = "ERRMSG_MANAGER_CONTENTSMANAGER_FILE_ALREADY_EXISTS";
	public static string ERRMSG_MANAGER_CONTENTSMANAGER_FILE_OPERATION_ERROR = "ERRMSG_MANAGER_CONTENTSMANAGER_FILE_OPERATION_ERROR";
	public static string ERRMSG_MANAGER_CONTENTSMANAGER_FILE_ALREADY_EXISTS_ZIPDIR = "ERRMSG_MANAGER_CONTENTSMANAGER_FILE_ALREADY_EXISTS_ZIPDIR";

	// アフィリエイトタグ設定エラー
	public static string ERRMSG_MANAGER_AFFILIATE_TAG_NO_URL_FORMAT_ERROR = "ERRMSG_MANAGER_AFFILIATE_TAG_NO_URL_FORMAT_ERROR";
	public static string ERRMSG_MANAGER_AFFILIATE_TAG_URL_FORMAT_ERROR = "ERRMSG_MANAGER_AFFILIATE_TAG_URL_FORMAT_ERROR";
	public static string ERRMSG_MANAGER_AFFILIATE_TAG_DUPLICATION_ERROR = "ERRMSG_MANAGER_AFFILIATE_TAG_DUPLICATION_ERROR";
	public static string ERRMSG_MANAGER_AFFILIATE_TAG_SELECT_PAGE_ERROR = "ERRMSG_MANAGER_AFFILIATE_TAG_SELECT_PAGE_ERROR";

	// アフィリエイト商品タグタグ設定エラー
	public static string ERRMSG_MANAGER_AFFILIATE_PRODUCT_TAG_SELECTED_ERROR = "ERRMSG_MANAGER_AFFILIATE_PRODUCT_TAG_SELECTED_ERROR";
	public static string ERRMSG_MANAGER_AFFILIATE_PRODUCT_TAG_DELETE_ERROR = "ERRMSG_MANAGER_AFFILIATE_PRODUCT_TAG_DELETE_ERROR";

	// Error message AdvCode Media Type is not edit any item
	public static string ERRMSG_MANAGER_AFFILIATE_MEDIA_TYPE_SELECTED_ERROR = "ERRMSG_MANAGER_AFFILIATE_MEDIA_TYPE_SELECTED_ERROR";
	// Error message AdvCode Media Type is duplicated
	public static string ERRMSG_MANAGER_AFFILIATE_MEDIA_TYPE_DUPLICATION_ERROR = "ERRMSG_MANAGER_AFFILIATE_MEDIA_TYPE_DUPLICATION_ERROR";
	// Error message AdvCode Media Type can not be deleted
	public static string ERRMSG_MANAGER_AFFILIATE_MEDIA_TYPE_DELETE_ERROR = "ERRMSG_MANAGER_AFFILIATE_MEDIA_TYPE_DELETE_ERROR";

	// マスタ出力設定：不正フィールドエラー
	public static string ERRMSG_MANAGER_MASTEREXPORTSETTING_FIELDS_ERROR = "ERRMSG_MANAGER_MASTEREXPORTSETTING_FIELDS_ERROR";
	// マスタ出力設定：CSVフォーマット設定データなしエラー
	public static string ERRMSG_MANAGER_MASTEREXPORTSETTING_NO_DATA = "ERRMSG_MANAGER_MASTEREXPORTSETTING_NO_DATA";
	// EXCEL出力キャパシティオーバーエラー
	public static string ERRMSG_MANAGER_MASTEREXPORT_EXCEL_OVER_CAPACITY = "ERRMSG_MANAGER_MASTEREXPORT_EXCEL_OVER_CAPACITY";

	/// <summary>閲覧可能な広告コード定義不正フィールドエラー</summary>
	public static string ERRMSG_MANAGER_REGISTER_SETTING_ERRORADVCODE = "ERRMSG_MANAGER_REGISTER_SETTING_ERRORADVCODE";

	/// <summary>閲覧可能なタグ定義不正フィールドエラー</summary>
	public static string ERRMSG_MANAGER_REGISTER_SETTING_ERRORTAGID = "ERRMSG_MANAGER_REGISTER_SETTING_ERRORTAGID";
	/// <summary>閲覧可能な広告媒体区分定義不正フィールドエラー</summary>
	public const string ERRMSG_MANAGER_REGISTER_SETTING_ERROR_MEDIATYPE = "ERRMSG_MANAGER_REGISTER_SETTING_ERROR_MEDIATYPE";
	/// <summary>閲覧可能な設置個所定義不正フィールドエラー</summary>
	public const string ERRMSG_MANAGER_REGISTER_SETTING_ERROR_LOCATION = "ERRMSG_MANAGER_REGISTER_SETTING_ERROR_LOCATION";

	// メニュー権限情報エラー
	public static string ERRMSG_MANAGER_MENUAUTHORITY_NO_DEFALT_PAGE = "ERRMSG_MANAGER_MENUAUTHORITY_NO_DEFALT_PAGE";
	// メニュー権限情報削除不可エラー
	public static string ERRMSG_MANAGER_MENUAUTHORITY_DELETE_IMPOSSIBLE_ERROR = "ERRMSG_MANAGER_MENUAUTHORITY_DELETE_IMPOSSIBLE_ERROR";

	/// <summary>定期回数別レポートページ：日付が大きすぎるエラー</summary>
	public static string ERRMSG_MANAGER_ORDER_REPEAT_REPORT_OUT_OF_RANGE = "ERRMSG_MANAGER_ORDER_REPEAT_REPORT_OUT_OF_RANGE";
	/// <summary>定期回数別レポートページ：FromがToより大きいエラー</summary>
	public static string ERRMSG_MANAGER_ORDER_REPEAT_REPORT_INCORRECT_DATE = "ERRMSG_MANAGER_ORDER_REPEAT_REPORT_INCORRECT_DATE";
	/// <summary>定期回数別レポートページ：日付がおかしいエラー</summary>
	public static string ERRMSG_MANAGER_ORDER_REPEAT_REPORT_INVALID_DATE = "ERRMSG_MANAGER_ORDER_REPEAT_REPORT_INVALID_DATE";

	/// <summary>レコメンドレポート：期間指定オーバーエラー</summary>
	public static string ERRMSG_MANAGER_RECOMMEND_REPORT_OUT_OF_RANGE_ERROR = "ERRMSG_MANAGER_RECOMMEND_REPORT_OUT_OF_RANGE_ERROR";
	/// <summary>レコメンドレポート：期間指定FromがToより大きいエラー</summary>
	public static string ERRMSG_MANAGER_RECOMMEND_REPORT_INVALID_DATE_ERROR = "ERRMSG_MANAGER_RECOMMEND_REPORT_INVALID_DATE_ERROR";
	/// <summary>レコメンドレポート：レコメンドID未入力エラー</summary>
	public static string ERRMSG_MANAGER_RECOMMEND_REPORT_NO_RECOMMENDID_ERROR = "ERRMSG_MANAGER_RECOMMEND_REPORT_NO_RECOMMENDID_ERROR";
	/// <summary>レコメンドレポート：該当レコメンド存在しないエラー</summary>
	public static string ERRMSG_MANAGER_RECOMMEND_REPORT_RECOMMENDID_NOT_EXIST_ERROR = "ERRMSG_MANAGER_RECOMMEND_REPORT_RECOMMENDID_NOT_EXIST_ERROR";

	// マスタ出力未選択エラー
	public static string ERRMSG_MASTEREXPORTSETTING_SETTING_ID_NOT_SELECTED = "ERRMSG_MASTEREXPORTSETTING_SETTING_ID_NOT_SELECTED";

	// シルバーエッグレポート取得エラー
	public static string ERRMSG_MANAGER_SILVEREGG_GET_REPORT_ERROR = "ERRMSG_MANAGER_SILVEREGG_GET_REPORT_ERROR";

	// CS系
	// CSオペレータ削除不可エラー
	public static string ERRMSG_MANAGER_CSOPERATOR_DELETE_IMPOSSIBLE_ERROR = "ERRMSG_MANAGER_CSOPERATOR_DELETE_IMPOSSIBLE_ERROR";

	/// <summary>Error Message manager enter a valid date</summary>
	public static string ERRMSG_MANAGER_ENTER_VALID_DATE = "ERRMSG_MANAGER_ENTER_VALID_DATE";
	/// <summary>Error Message manager period designation</summary>
	public static string ERRMSG_MANAGER_PERIOD_DESIGNATION = "ERRMSG_MANAGER_PERIOD_DESIGNATION";
	/// <summary>Error Message manager elapsed time</summary>
	public static string ERRMSG_MANAGER_ELAPSED_TIME = "ERRMSG_MANAGER_ELAPSED_TIME";
	/// <summary>Error Message manager campaign validity period (start)</summary>
	public static string ERRMSG_MANAGER_CAMPAIGN_VALIDITY_PERIOD_START = "ERRMSG_MANAGER_CAMPAIGN_VALIDITY_PERIOD_START";
	/// <summary>Error Message manager campaign validity period (end)</summary>
	public static string ERRMSG_MANAGER_CAMPAIGN_VALIDITY_PERIOD_END = "ERRMSG_MANAGER_CAMPAIGN_VALIDITY_PERIOD_END";
	/// <summary>Error Message manager campaign validity period</summary>
	public static string ERRMSG_MANAGER_CAMPAIGN_VALIDITY_PERIOD = "ERRMSG_MANAGER_CAMPAIGN_VALIDITY_PERIOD";
	/// <summary>Error Message manager campaign validity date</summary>
	public static string ERRMSG_MANAGER_CAMPAIGN_VALIDITY_DATE = "ERRMSG_MANAGER_CAMPAIGN_VALIDITY_DATE";
	/// <summary>Error Message manager CSV is currently being uploaded. Please re-upload after completion</summary>
	public static string ERRMSG_MANAGER_CSV_UPLOADED = "ERRMSG_MANAGER_CSV_UPLOADED";
	/// <summary>Error Message manager target list name</summary>
	public static string ERRMSG_MANAGER_TARGET_LIST_NAME = "ERRMSG_MANAGER_TARGET_LIST_NAME";
	/// <summary>Error Message manager master file import not found</summary>
	public static string ERRMSG_MANAGER_MASTER_FILE_NOT_FOUND = "ERRMSG_MANAGER_MASTER_FILE_NOT_FOUND";
	/// <summary>Error Message manager please select target list 1 information</summary>
	public static string ERRMSG_MANAGER_SELECT_TARGER_LIST1_INFORMATION = "ERRMSG_MANAGER_SELECT_TARGER_LIST1_INFORMATION";
	/// <summary>Error Message manager please select target list 2 information</summary>
	public static string ERRMSG_MANAGER_SELECT_TARGER_LIST2_INFORMATION = "ERRMSG_MANAGER_SELECT_TARGER_LIST2_INFORMATION";
	/// <summary>Error Message manager please select a merge pattern</summary>
	public static string ERRMSG_MANAGER_SELECT_MERGE_PATTERN = "ERRMSG_MANAGER_SELECT_MERGE_PATTERN";
	/// <summary>Error Message manager the selected target list 1 does not exist</summary>
	public static string ERRMSG_MANAGER_SELECTED_TARGET1_NOT_EXIST = "ERRMSG_MANAGER_SELECTED_TARGET1_NOT_EXIST";
	/// <summary>Error Message manager the selected target list 2 does not exist</summary>
	public static string ERRMSG_MANAGER_SELECTED_TARGET2_NOT_EXIST = "ERRMSG_MANAGER_SELECTED_TARGET2_NOT_EXIST";
	/// <summary>Error Message manager could not merge because there are 0 result</summary>
	public static string ERRMSG_MANAGER_COULD_NOT_MERGE = "ERRMSG_MANAGER_COULD_NOT_MERGE";
	/// <summary>Error Message only the text for mobile is enterd</summary>
	public static string ERRMSG_MANAGER_MAIL_DIST_TEXT_NOT_TITLE_TEXT_MOBILE = "ERRMSG_MANAGER_MAIL_DIST_TEXT_NOT_TITLE_TEXT_MOBILE";
	/// <summary>Error Message only the text for pc is enterd</summary>
	public static string ERRMSG_MANAGER_MAIL_DIST_TEXT_NOT_TITLE_TEXT_PC = "ERRMSG_MANAGER_MAIL_DIST_TEXT_NOT_TITLE_TEXT_PC";
	/// <summary>Error Message language code</summary>
	public static string ERRMSG_MANAGER_LANGUAGE_CODE = "ERRMSG_MANAGER_LANGUAGE_CODE";
	/// <summary>Error Message language field column not applicable</summary>
	public static string ERRMSG_MANAGER_FIELD_NOT_APPLICABLE = "ERRMSG_MANAGER_FIELD_NOT_APPLICABLE";

	/// <summary>クーポン設定：対象商品限定を指定(または)の場合、商品IDもしくはキャンペーンアイコンの指定が1つもないエラー</summary>
	public const string ERRMSG_MANAGER_COUPON_PRODUCT_UNTARGET_CHECK = "ERRMSG_MANAGER_COUPON_PRODUCT_UNTARGET_CHECK";
	/// <summary>クーポン設定：対象商品限定を指定(かつ)の場合、ブランドID、カテゴリIDもしくはキャンペーンアイコンの指定が1つもないエラー</summary>
	public const string ERRMSG_MANAGER_COUPON_PRODUCT_UNTARGET_BY_LOGICAL_AND_CHECK_BRAND = "ERRMSG_MANAGER_COUPON_PRODUCT_UNTARGET_BY_LOGICAL_AND_CHECK_BRAND";
	/// <summary>クーポン設定：対象商品限定を指定(かつ)の場合、カテゴリIDもしくはキャンペーンアイコンの指定が1つもないエラー</summary>
	public const string ERRMSG_MANAGER_COUPON_PRODUCT_UNTARGET_BY_LOGICAL_AND_CHECK = "ERRMSG_MANAGER_COUPON_PRODUCT_UNTARGET_BY_LOGICAL_AND_CHECK";
	/// <summary>クーポン設定：カテゴリデータなしエラー</summary>
	public const string ERRMSG_MANAGER_COUPON_PRODUCT_CATEGORY_UNFOUND = "ERRMSG_MANAGER_COUPON_PRODUCT_CATEGORY_UNFOUND";
	/// <summary>クーポン設定：ブランドデータエラーなし</summary>
	public const string ERRMSG_MANAGER_COUPON_PRODUCT_BRAND_UNFOUND = "ERRMSG_MANAGER_COUPON_PRODUCT_BRAND_UNFOUND";
	/// <summary>クーポン設定：商品無効</summary>
	public const string ERRMSG_MANAGER_COUPON_PRODUCT_INVALID = "ERRMSG_MANAGER_COUPON_PRODUCT_INVALID";
	/// <summary>クーポン設定：商品ID未存在</summary>
	public const string ERRMSG_MANAGER_COUPON_PRODUCT_ID_NOT_EXISTS = "ERRMSG_MANAGER_COUPON_PRODUCT_ID_NOT_EXISTS";

	/// <summary>Error message：processing lost data</summary>
	public const string ERRMSG_MANAGER_PROCESSING_LOST_DATA = "ERRMSG_MANAGER_PROCESSING_LOST_DATA";

	/// <summary>ターゲットリスト設定登録メッセージ</summary>
	public const string ERRMSG_USER_TARGET_LIST_NO_TARGET_DATA = "ERRMSG_USER_TARGET_LIST_NO_TARGET_DATA";
}