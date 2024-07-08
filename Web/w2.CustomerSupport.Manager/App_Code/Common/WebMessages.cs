/*
=========================================================================================================
  Module      : メッセージモジュール(WebMessages.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
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
	// オペレータパスワード変更
	public static string ERRMSG_MANAGER_SHOP_OPERATOR_NO_OPERATOR_ERROR = "ERRMSG_MANAGER_SHOP_OPERATOR_NO_OPERATOR_ERROR";
	// ログイン失敗制限回数超過エラー
	public static string ERRMSG_MANAGER_LOGIN_LIMITED_COUNT_ERROR = "ERRMSG_MANAGER_LOGIN_LIMITED_COUNT_ERROR";
	// メニュー表示権限なしエラー
	public static string ERRMSG_MANAGER_LOGIN_UNACCESSABLEUSER_ERROR = "ERRMSG_MANAGER_LOGIN_UNACCESSABLEUSER_ERROR";
	/// <summary>CSオプションOFFエラー</summary>
	public static string ERRMSG_MANAGER_CS_OPTION_DISABLED = "ERRMSG_MANAGER_CS_OPTION_DISABLED";
	// 一覧該当データ無し
	public static string ERRMSG_MANAGER_NO_HIT_LIST = "ERRMSG_MANAGER_NO_HIT_LIST";
	// 一覧該当データオーバー
	public static string ERRMSG_MANAGER_OVER_HIT_LIST = "ERRMSG_MANAGER_OVER_HIT_LIST";
	// 詳細該当データ無し
	public static string ERRMSG_MANAGER_NO_HIT_DETAIL = "ERRMSG_MANAGER_NO_HIT_DETAIL";
	// メニュー権限情報エラー
	public static string ERRMSG_MANAGER_MENUAUTHORITY_NO_DEFALT_PAGE = "ERRMSG_MANAGER_MENUAUTHORITY_NO_DEFALT_PAGE";
	// メニュー権限情報削除不可エラー
	public static string ERRMSG_MANAGER_MENUAUTHORITY_DELETE_IMPOSSIBLE_ERROR = "ERRMSG_MANAGER_MENUAUTHORITY_DELETE_IMPOSSIBLE_ERROR";
	// CSV出力フィールドエラー
	public static string ERRMSG_MANAGER_CSV_OUTPUT_FIELD_ERROR = "ERRMSG_MANAGER_CSV_OUTPUT_FIELD_ERROR";
	// CSVフォーマット設定データなしエラー
	public static string ERRMSG_MANAGER_MASTEREXPORTSETTING_NO_DATA = "ERRMSG_MANAGER_MASTEREXPORTSETTING_NO_DATA";
	// EXCEL出力キャパシティオーバーエラー
	public static string ERRMSG_MANAGER_MASTEREXPORT_EXCEL_OVER_CAPACITY = "ERRMSG_MANAGER_MASTEREXPORT_EXCEL_OVER_CAPACITY";

	// 不正パラメータエラー
	public static string ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR = "ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR";

	// ポップアップ同時起動時エラー
	public static string ERRMSG_MANAGER_POPUP_IRREGULAR_PARAMETER_ERROR = "ERRMSG_MANAGER_POPUP_IRREGULAR_PARAMETER_ERROR";

	//------------------------------------------------------
	// CS系
	//------------------------------------------------------
	// VOC削除エラー
	public static string ERRMSG_MANAGER_INCIDENTVOC_NO_DELETE = "ERRMSG_MANAGER_INCIDENTVOC_NO_DELETE";
	// 集計区分登録エラー
	public static string ERRMSG_MANAGER_SUMMARYSETTING_REGISTER_ERROR = "ERRMSG_MANAGER_SUMMARYSETTING_REGISTER_ERROR";
	// インシデントカテゴリ未選択エラー
	public static string ERRMSG_MANAGER_INCIDENT_CATEGORY_NO_SELECTED_ERROR = "ERRMSG_MANAGER_INCIDENT_CATEGORY_NO_SELECTED_ERROR";
	// インシデントカテゴリ更新エラー
	public static string ERRMSG_MANAGER_INCIDENT_CATEGORY_NO_UPDATE = "ERRMSG_MANAGER_INCIDENT_CATEGORY_NO_UPDATE";
	// インシデントカテゴリ削除エラー
	public static string ERRMSG_MANAGER_INCIDENT_CATEGORY_NO_DELETE_BY_INCIDENT = "ERRMSG_MANAGER_INCIDENT_CATEGORY_NO_DELETE_BY_INCIDENT";
	public static string ERRMSG_MANAGER_INCIDENT_CATEGORY_NO_DELETE_BY_CHILD = "ERRMSG_MANAGER_INCIDENT_CATEGORY_NO_DELETE_BY_CHILD";
	// 回答例カテゴリ未選択エラー
	public static string ERRMSG_MANAGER_ANSWER_TEMPLATE_CATEGORY_NO_SELECTED_ERROR = "ERRMSG_MANAGER_ANSWER_TEMPLATE_CATEGORY_NO_SELECTED_ERROR";
	// 回答例カテゴリ更新エラー
	public static string ERRMSG_MANAGER_ANSWER_TEMPLATE_CATEGORY_NO_UPDATE = "ERRMSG_MANAGER_ANSWER_TEMPLATE_CATEGORY_NO_UPDATE";
	// 回答例カテゴリ削除エラー
	public static string ERRMSG_MANAGER_ANSWER_TEMPLATE_CATEGORY_NO_DELETE = "ERRMSG_MANAGER_ANSWER_TEMPLATE_CATEGORY_NO_DELETE";
	// 共有情報オペレータ未選択エラー
	public static string ERRMSG_MANAGER_SHARE_INFO_OPERATOR_NO_SELECTED_ERROR = "ERRMSG_MANAGER_SHARE_INFO_OPERATOR_NO_SELECTED_ERROR";
	// CSグループ未選択エラー
	public static string ERRMSG_MANAGER_CSGROUP_NO_SELECTED_ERROR = "ERRMSG_MANAGER_CSGROUP_NO_SELECTED_ERROR";
	// 画面を閉じる場合の確認メッセージ
	public static string CFMMSG_MANAGER_MESSAGE_POPUP_CLOSE_CONFIRM = "CFMMSG_MANAGER_MESSAGE_POPUP_CLOSE_CONFIRM";
	// ユーザー登録画面を閉じる場合の確認メッセージ
	public static string CFMMSG_USERREGISTER_POPUP_CLOSE_CONFIRM = "CFMMSG_USERREGISTER_POPUP_CLOSE_CONFIRM";
	// 注文登録画面を閉じる場合の確認メッセージ
	public static string CFMMSG_ORDERREGISTINPUT_POPUP_CLOSE_CONFIRM = "CFMMSG_ORDERREGISTINPUT_POPUP_CLOSE_CONFIRM";
	// Error replace user info in message body
	public static string ERRMSG_MANAGER_MESSAGE_BODY_USER_INFO_REPLACE_NULL_ERROR = "ERRMSG_MANAGER_MESSAGE_BODY_USER_INFO_REPLACE_NULL_ERROR";
	// CSオペレータ権限情報削除不可エラー
	public static string ERRMSG_MANAGER_CSOPERATORAUTHORITY_DELETE_IMPOSSIBLE_ERROR = "ERRMSG_MANAGER_CSOPERATORAUTHORITY_DELETE_IMPOSSIBLE_ERROR";
	// CSオペレータ削除不可エラー
	public static string ERRMSG_MANAGER_CSOPERATOR_DELETE_IMPOSSIBLE_ERROR = "ERRMSG_MANAGER_CSOPERATOR_DELETE_IMPOSSIBLE_ERROR";
	// メールアドレス重複ユーザー存在エラー
	public static string ERRMSG_MANAGER_MAILADDRESS_DUPLICATE_ERROR = "ERRMSG_MANAGER_MAILADDRESS_DUPLICATE_ERROR";
	/// <summary>Error manager point for this email address</summary>
	public static string ERROR_MANAGER_POINT_FOR_THIS_EMAIL_ADDRESS = "ERROR_MANAGER_POINT_FOR_THIS_EMAIL_ADDRESS";
	/// <summary>Error manager parameter cannot be found</summary>
	public static string ERROR_MANAGER_PARAMETER_CANNOT_BE_FOUND = "ERROR_MANAGER_PARAMETER_CANNOT_BE_FOUND";
	/// <summary>Error manager failed to send email</summary>
	public static string ERROR_MANAGER_FAILED_TO_SEND_EMAIL = "ERROR_MANAGER_FAILED_TO_SEND_EMAIL";
	/// <summary>Error manager sent an email</summary>
	public static string ERROR_MANAGER_SENT_AN_EMAIL = "ERROR_MANAGER_SENT_AN_EMAIL";
	/// <summary>Error manager is required</summary>
	public static string ERROR_MANAGER_IS_REQUIRED = "ERROR_MANAGER_IS_REQUIRED";
	/// <summary>Error manager are duplicated</summary>
	public static string ERROR_MANAGER_ARE_DUPLICATED = "ERROR_MANAGER_ARE_DUPLICATED";
	/// <summary>Error manager when setting menu authority</summary>
	public static string ERROR_MANAGER_WHEN_SETTING_MENU_AUTHORITY = "ERROR_MANAGER_WHEN_SETTING_MENU_AUTHORITY";
	/// <summary>Error manager start date correct date</summary>
	public static string ERROR_MANAGER_START_DATE_CORRECT_DATE = "ERROR_MANAGER_START_DATE_CORRECT_DATE";
	/// <summary>Error manager end date correct date</summary>
	public static string ERROR_MANAGER_END_DATE_CORRECT_DATE = "ERROR_MANAGER_END_DATE_CORRECT_DATE";
	/// <summary>Error manager start date before end date</summary>
	public static string ERROR_MANAGER_START_DATE_BEFORE_END_DATE = "ERROR_MANAGER_START_DATE_BEFORE_END_DATE";
	/// <summary>Error manager period within 100 days</summary>
	public static string ERROR_MANAGER_PERIOD_WITHIN_100_DAYS = "ERROR_MANAGER_PERIOD_WITHIN_100_DAYS";
	/// <summary>Error manager register one or more valid aggregate item information</summary>
	public static string ERROR_MANAGER_REGISTER_ONE_OR_MORE_VALID_AGGREGATE_ITEM_INFORMATION = "ERROR_MANAGER_REGISTER_ONE_OR_MORE_VALID_AGGREGATE_ITEM_INFORMATION";
	/// <summary>Error manager message request cannot be found</summary>
	public static string ERROR_MANAGER_MESSAGE_REQUEST_CANNOT_BE_FOUND = "ERROR_MANAGER_MESSAGE_REQUEST_CANNOT_BE_FOUND";
	/// <summary>Error manager operator not found</summary>
	public static string ERROR_MANAGER_OPERATOR_NOT_FOUND = "ERROR_MANAGER_OPERATOR_NOT_FOUND";
	/// <summary>Error manager message mail not found</summary>
	public static string ERROR_MANAGER_MESSAGE_MAIL_NOT_FOUND = "ERROR_MANAGER_MESSAGE_MAIL_NOT_FOUND";
	/// <summary>Error manager failed to send the email</summary>
	public static string ERROR_MANAGER_FAILED_TO_SEND_THE_EMAIL = "ERROR_MANAGER_FAILED_TO_SEND_THE_EMAIL";
	/// <summary>Error manager failed to send the email incident is not closed</summary>
	public static string ERROR_MANAGER_FAILED_TO_SEND_THE_EMAIL_INCIDENT_IS_NOT_CLOSED = "ERROR_MANAGER_FAILED_TO_SEND_THE_EMAIL_INCIDENT_IS_NOT_CLOSED";
	/// <summary>Error manager received</summary>
	public static string ERROR_MANAGER_RECEIVED = "ERROR_MANAGER_RECEIVED";
	/// <summary>Error manager receiving while</summary>
	public static string ERROR_MANAGER_RECEIVING_WHILE = "ERROR_MANAGER_RECEIVING_WHILE";
	/// <summary>Error manager incident has been updated</summary>
	public static string ERROR_MANAGER_INCIDENT_HAS_BEEN_UPDATED = "ERROR_MANAGER_INCIDENT_HAS_BEEN_UPDATED";
	/// <summary>Error manager updated internal notes</summary>
	public static string ERROR_MANAGER_UPDATED_INTERNAL_NOTES = "ERROR_MANAGER_UPDATED_INTERNAL_NOTES";
	/// <summary>Error manager incident has been closed</summary>
	public static string ERROR_MANAGER_INCIDENT_HAS_BEEN_CLOSED = "ERROR_MANAGER_INCIDENT_HAS_BEEN_CLOSED";
	/// <summary>Error manager incident id has been updated</summary>
	public static string ERROR_MANAGER_INCIDENT_ID_HAS_BEEN_UPDATED = "ERROR_MANAGER_INCIDENT_ID_HAS_BEEN_UPDATED";
	/// <summary>Error manager created new incident</summary>
	public static string ERROR_MANAGER_CREATED_NEW_INCIDENT = "ERROR_MANAGER_CREATED_NEW_INCIDENT";
	/// <summary>Error manager same id as before the update</summary>
	public static string ERROR_MANAGER_SAME_ID_AS_BEFORE_THE_UPDATE = "ERROR_MANAGER_SAME_ID_AS_BEFORE_THE_UPDATE";
	/// <summary>Error manager corresponding id did not exist</summary>
	public static string ERROR_MANAGER_CORRESPONDING_ID_DID_NOT_EXIST = "ERROR_MANAGER_CORRESPONDING_ID_DID_NOT_EXIST";
	/// <summary>Error manager incident is locked</summary>
	public static string ERROR_MANAGER_INCIDENT_IS_LOCKED = "ERROR_MANAGER_INCIDENT_IS_LOCKED";
	/// <summary>Message manager register message (incident closed)</summary>
	public static string MSG_MANAGER_REGISTER_MESSAGE_INCIDENT_CLOSED = "MSG_MANAGER_REGISTER_MESSAGE_INCIDENT_CLOSED";
	/// <summary>Message manager register message</summary>
	public static string MSG_MANAGER_REGISTER_MESSAGE = "MSG_MANAGER_REGISTER_MESSAGE";
	/// <summary>Message manager update message</summary>
	public static string MSG_MANAGER_UPDATE_MESSAGE = "MSG_MANAGER_UPDATE_MESSAGE";
	/// <summary>Message manager newly numbered</summary>
	public static string MSG_MANAGER_NEWLY_NUMBERED = "MSG_MANAGER_NEWLY_NUMBERED";
	/// <summary>Message manager i did</summary>
	public static string MSG_MANAGER_I_DID = "MSG_MANAGER_I_DID";
	/// <summary>Message manager user list count max displayed</summary>
	public static string MSG_MANAGER_USER_LIST_COUNT_MAX_DISPLAYED = "MSG_MANAGER_USER_LIST_COUNT_MAX_DISPLAYED";
	/// <summary>Message manager message right mail already plugged in</summary>
	public static string MSG_MANAGER_MESSAGE_RIGHT_MAIL_ALREADY_PLUGGED_IN = "MSG_MANAGER_MESSAGE_RIGHT_MAIL_ALREADY_PLUGGED_IN";
	/// <summary>Message manager message right mail address is empty</summary>
	public static string MSG_MANAGER_MESSAGE_RIGHT_MAIL_ADDRESS_IS_EMPTY = "MSG_MANAGER_MESSAGE_RIGHT_MAIL_ADDRESS_IS_EMPTY";
	/// <summary>Message manager mail action alert display operator during approval work</summary>
	public static string MSG_MANAGER_MAIL_ACTION_ALERT_DISPLAY_OPERATOR_DURING_APPROVAL_WORK = "MSG_MANAGER_MAIL_ACTION_ALERT_DISPLAY_OPERATOR_DURING_APPROVAL_WORK";
	/// <summary>Message manager incident and message parts main area by inside</summary>
	public static string MSG_MANAGER_INCIDENT_AND_MESSAGE_PARTS_MAIN_AREA_BY_INSIDE = "MSG_MANAGER_INCIDENT_AND_MESSAGE_PARTS_MAIN_AREA_BY_INSIDE";
	/// <summary>Message manager search parts incorrect start date for period</summary>
	public static string MSG_MANAGER_SEARCH_PARTS_INCORRECT_START_DATE_FOR_PERIOD = "MSG_MANAGER_SEARCH_PARTS_INCORRECT_START_DATE_FOR_PERIOD";
	/// <summary>Message manager search parts incorrect end date of period</summary>
	public static string MSG_MANAGER_SEARCH_PARTS_INCORRECT_END_DATE_FOR_PERIOD = "MSG_MANAGER_SEARCH_PARTS_INCORRECT_END_DATE_FOR_PERIOD";
	/// <summary>受信時振分けのときメールがないエラーメッセージ</summary>
	public const string MSG_MANAGER_MAIL_ASSIGN_NO_MAIL = "MSG_MANAGER_MAIL_ASSIGN_NO_MAIL";
}