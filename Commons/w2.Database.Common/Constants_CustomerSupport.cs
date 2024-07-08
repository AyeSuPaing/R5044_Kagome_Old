/*
=========================================================================================================
  Module      : DB定数定義CustomerSupport部分(Constants_CustomerSupport.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
namespace w2.Database.Common
{
	///*********************************************************************************************
	/// <summary>
	/// DB定数定義CustomerSupport部分
	/// </summary>
	///*********************************************************************************************
	public partial class Constants : w2.Common.Constants
	{
		#region テーブル・フィールド定数

		// インシデントカテゴリマスタ
		public const string TABLE_CSINCIDENTCATEGORY = "w2_CsIncidentCategory";                     // インシデントカテゴリマスタ
		public const string FIELD_CSINCIDENTCATEGORY_DEPT_ID = "dept_id";                           // 識別ID
		public const string FIELD_CSINCIDENTCATEGORY_CATEGORY_ID = "category_id";                   // インシデントカテゴリID
		public const string FIELD_CSINCIDENTCATEGORY_PARENT_CATEGORY_ID = "parent_category_id";     // インシデント親カテゴリID
		public const string FIELD_CSINCIDENTCATEGORY_CATEGORY_NAME = "category_name";               // インシデントカテゴリ名
		public const string FIELD_CSINCIDENTCATEGORY_DISPLAY_ORDER = "display_order";               // 表示順
		public const string FIELD_CSINCIDENTCATEGORY_VALID_FLG = "valid_flg";                       // 有効フラグ
		public const string FIELD_CSINCIDENTCATEGORY_DEL_FLG = "del_flg";                           // 削除フラグ
		public const string FIELD_CSINCIDENTCATEGORY_DATE_CREATED = "date_created";                 // 作成日
		public const string FIELD_CSINCIDENTCATEGORY_DATE_CHANGED = "date_changed";                 // 更新日
		public const string FIELD_CSINCIDENTCATEGORY_LAST_CHANGED = "last_changed";                 // 最終更新者

		// インシデントVOCマスタ
		public const string TABLE_CSINCIDENTVOC = "w2_CsIncidentVoc";                               // インシデントVOCマスタ
		public const string FIELD_CSINCIDENTVOC_DEPT_ID = "dept_id";                                // 識別ID
		public const string FIELD_CSINCIDENTVOC_VOC_ID = "voc_id";                                  // VOC ID
		public const string FIELD_CSINCIDENTVOC_VOC_TEXT = "voc_text";                              // 表示文言
		public const string FIELD_CSINCIDENTVOC_DISPLAY_ORDER = "display_order";                    // 表示順
		public const string FIELD_CSINCIDENTVOC_VALID_FLG = "valid_flg";                            // 有効フラグ
		public const string FIELD_CSINCIDENTVOC_DEL_FLG = "del_flg";                                // 削除フラグ
		public const string FIELD_CSINCIDENTVOC_DATE_CREATED = "date_created";                      // 作成日
		public const string FIELD_CSINCIDENTVOC_DATE_CHANGED = "date_changed";                      // 更新日
		public const string FIELD_CSINCIDENTVOC_LAST_CHANGED = "last_changed";                      // 最終更新者

		// 集計区分マスタ
		public const string TABLE_CSSUMMARYSETTING = "w2_CsSummarySetting";                         // 集計区分マスタ
		public const string FIELD_CSSUMMARYSETTING_DEPT_ID = "dept_id";                             // 識別ID
		public const string FIELD_CSSUMMARYSETTING_SUMMARY_SETTING_NO = "summary_setting_no";       // 集計区分番号
		public const string FIELD_CSSUMMARYSETTING_SUMMARY_SETTING_TITLE = "summary_setting_title"; // 集計区分名
		public const string FIELD_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE = "summary_setting_type";   // 集計区分入力種別
		public const string FIELD_CSSUMMARYSETTING_DISPLAY_ORDER = "display_order";                 // 表示順
		public const string FIELD_CSSUMMARYSETTING_VALID_FLG = "valid_flg";                         // 有効フラグ
		public const string FIELD_CSSUMMARYSETTING_DEL_FLG = "del_flg";                             // 削除フラグ
		public const string FIELD_CSSUMMARYSETTING_DATE_CREATED = "date_created";                   // 作成日
		public const string FIELD_CSSUMMARYSETTING_DATE_CHANGED = "date_changed";                   // 更新日
		public const string FIELD_CSSUMMARYSETTING_LAST_CHANGED = "last_changed";                   // 最終更新者

		// 集計区分アイテムマスタ
		public const string TABLE_CSSUMMARYSETTINGITEM = "w2_CsSummarySettingItem";                 // 集計区分アイテムマスタ
		public const string FIELD_CSSUMMARYSETTINGITEM_DEPT_ID = "dept_id";                         // 識別ID
		public const string FIELD_CSSUMMARYSETTINGITEM_SUMMARY_SETTING_NO = "summary_setting_no";   // 集計区分番号
		public const string FIELD_CSSUMMARYSETTINGITEM_SUMMARY_SETTING_ITEM_ID = "summary_setting_item_id";// 集計区分アイテムID
		public const string FIELD_CSSUMMARYSETTINGITEM_SUMMARY_SETTING_ITEM_TEXT = "summary_setting_item_text";// 集計区分アイテム文言
		public const string FIELD_CSSUMMARYSETTINGITEM_DISPLAY_ORDER = "display_order";             // 表示順
		public const string FIELD_CSSUMMARYSETTINGITEM_VALID_FLG = "valid_flg";                     // 有効フラグ
		public const string FIELD_CSSUMMARYSETTINGITEM_DEL_FLG = "del_flg";                         // 削除フラグ
		public const string FIELD_CSSUMMARYSETTINGITEM_DATE_CREATED = "date_created";               // 作成日
		public const string FIELD_CSSUMMARYSETTINGITEM_DATE_CHANGED = "date_changed";               // 更新日
		public const string FIELD_CSSUMMARYSETTINGITEM_LAST_CHANGED = "last_changed";               // 最終更新者

		// 回答例マスタ
		public const string TABLE_CSANSWERTEMPLATE = "w2_CsAnswerTemplate";                         // 回答例マスタ
		public const string FIELD_CSANSWERTEMPLATE_DEPT_ID = "dept_id";                             // 識別ID
		public const string FIELD_CSANSWERTEMPLATE_ANSWER_ID = "answer_id";                         // 回答例ID
		public const string FIELD_CSANSWERTEMPLATE_ANSWER_CATEGORY_ID = "answer_category_id";       // 回答例カテゴリID
		public const string FIELD_CSANSWERTEMPLATE_ANSWER_TITLE = "answer_title";                   // 回答例タイトル
		public const string FIELD_CSANSWERTEMPLATE_ANSWER_TEXT = "answer_text";                     // 回答例本文
		public const string FIELD_CSANSWERTEMPLATE_DISPLAY_ORDER = "display_order";                 // 表示順
		public const string FIELD_CSANSWERTEMPLATE_VALID_FLG = "valid_flg";                         // 有効フラグ
		public const string FIELD_CSANSWERTEMPLATE_DEL_FLG = "del_flg";                             // 削除フラグ
		public const string FIELD_CSANSWERTEMPLATE_DATE_CREATED = "date_created";                   // 作成日
		public const string FIELD_CSANSWERTEMPLATE_DATE_CHANGED = "date_changed";                   // 更新日
		public const string FIELD_CSANSWERTEMPLATE_LAST_CHANGED = "last_changed";                   // 最終更新者
		/// <summary>回答例件名</summary>
		public const string FIELD_CSANSWERTEMPLATE_ANSWER_MAIL_TITLE = "answer_mail_title";

		// 外部リンク設定マスタ
		public const string TABLE_CSEXTERNALLINK = "w2_CsExternalLink";                             // 外部リンク設定マスタ
		public const string FIELD_CSEXTERNALLINK_DEPT_ID = "dept_id";                               // 識別ID
		public const string FIELD_CSEXTERNALLINK_EXTERNAL_LINK_ID = "link_id";                      // リンクID
		public const string FIELD_CSEXTERNALLINK_EXTERNAL_LINK_TITLE = "link_title";                // 外部リンク設定タイトル
		public const string FIELD_CSEXTERNALLINK_EXTERNAL_LINK_URL = "link_url";                    // リンクURL
		public const string FIELD_CSEXTERNALLINK_EXTERNAL_LINK_MEMO = "link_memo";                  // リンクメモー
		public const string FIELD_CSEXTERNALLINK_VALID_FLG = "valid_flg";                           // 有効フラグ
		public const string FIELD_CSEXTERNALLINK_DISPLAY_ORDER = "display_order";                   // 表示順
		public const string FIELD_CSEXTERNALLINK_LAST_CHANGED = "last_changed";                     // 最終更新者
		public const string FIELD_CSEXTERNALLINK_DATE_CREATED = "date_created";                     // 作成日
		public const string FIELD_CSEXTERNALLINK_DATE_CHANGED = "date_changed";                     // 更新日

		// 回答例カテゴリマスタ
		public const string TABLE_CSANSWERTEMPLATECATEGORY = "w2_CsAnswerTemplateCategory";         // 回答例カテゴリマスタ
		public const string FIELD_CSANSWERTEMPLATECATEGORY_DEPT_ID = "dept_id";                     // 識別ID
		public const string FIELD_CSANSWERTEMPLATECATEGORY_CATEGORY_ID = "category_id";             // 回答例カテゴリID
		public const string FIELD_CSANSWERTEMPLATECATEGORY_PARENT_CATEGORY_ID = "parent_category_id";// 回答例親カテゴリID
		public const string FIELD_CSANSWERTEMPLATECATEGORY_CATEGORY_NAME = "category_name";         // 回答例カテゴリ名
		public const string FIELD_CSANSWERTEMPLATECATEGORY_DISPLAY_ORDER = "display_order";         // 表示順
		public const string FIELD_CSANSWERTEMPLATECATEGORY_VALID_FLG = "valid_flg";                 // 有効フラグ
		public const string FIELD_CSANSWERTEMPLATECATEGORY_DEL_FLG = "del_flg";                     // 削除フラグ
		public const string FIELD_CSANSWERTEMPLATECATEGORY_DATE_CREATED = "date_created";           // 作成日
		public const string FIELD_CSANSWERTEMPLATECATEGORY_DATE_CHANGED = "date_changed";           // 更新日
		public const string FIELD_CSANSWERTEMPLATECATEGORY_LAST_CHANGED = "last_changed";           // 最終更新者

		// メール署名マスタ
		public const string TABLE_CSMAILSIGNATURE = "w2_CsMailSignature";                           // メール署名マスタ
		public const string FIELD_CSMAILSIGNATURE_DEPT_ID = "dept_id";                              // 識別ID
		public const string FIELD_CSMAILSIGNATURE_MAIL_SIGNATURE_ID = "mail_signature_id";          // メール署名ID
		public const string FIELD_CSMAILSIGNATURE_SIGNATURE_TITLE = "signature_title";              // 署名タイトル
		public const string FIELD_CSMAILSIGNATURE_SIGNATURE_TEXT = "signature_text";                // 署名本文
		public const string FIELD_CSMAILSIGNATURE_OWNER_ID = "owner_id";                            // 所有オペレータID
		public const string FIELD_CSMAILSIGNATURE_DISPLAY_ORDER = "display_order";                  // 表示順
		public const string FIELD_CSMAILSIGNATURE_VALID_FLG = "valid_flg";                          // 有効フラグ
		public const string FIELD_CSMAILSIGNATURE_DEL_FLG = "del_flg";                              // 削除フラグ
		public const string FIELD_CSMAILSIGNATURE_DATE_CREATED = "date_created";                    // 作成日
		public const string FIELD_CSMAILSIGNATURE_DATE_CHANGED = "date_changed";                    // 更新日
		public const string FIELD_CSMAILSIGNATURE_LAST_CHANGED = "last_changed";                    // 最終更新者

		// メール送信元マスタ
		public const string TABLE_CSMAILFROM = "w2_CsMailFrom";                                     // メール送信元マスタ
		public const string FIELD_CSMAILFROM_DEPT_ID = "dept_id";                                   // 識別ID
		public const string FIELD_CSMAILFROM_MAIL_FROM_ID = "mail_from_id";                         // 送信元ID
		public const string FIELD_CSMAILFROM_MAIL_ADDRESS = "mail_address";                         // 送信元アドレス
		public const string FIELD_CSMAILFROM_DISPLAY_NAME = "display_name";                         // 送信元表示名
		public const string FIELD_CSMAILFROM_DISPLAY_ORDER = "display_order";                       // 表示順
		public const string FIELD_CSMAILFROM_VALID_FLG = "valid_flg";                               // 有効フラグ
		public const string FIELD_CSMAILFROM_DEL_FLG = "del_flg";                                   // 削除フラグ
		public const string FIELD_CSMAILFROM_DATE_CREATED = "date_created";                         // 作成日
		public const string FIELD_CSMAILFROM_DATE_CHANGED = "date_changed";                         // 更新日
		public const string FIELD_CSMAILFROM_LAST_CHANGED = "last_changed";                         // 最終更新者

		// インシデントマスタ
		public const string TABLE_CSINCIDENT = "w2_CsIncident";                                     // インシデントマスタ
		public const string FIELD_CSINCIDENT_DEPT_ID = "dept_id";                                   // 識別ID
		public const string FIELD_CSINCIDENT_INCIDENT_ID = "incident_id";                           // インシデントID
		public const string FIELD_CSINCIDENT_USER_ID = "user_id";                                   // ユーザーID
		public const string FIELD_CSINCIDENT_INCIDENT_CATEGORY_ID = "incident_category_id";         // インシデントカテゴリID
		public const string FIELD_CSINCIDENT_INCIDENT_TITLE = "incident_title";                     // インシデント件名
		public const string FIELD_CSINCIDENT_STATUS = "status";                                     // ステータス
		public const string FIELD_CSINCIDENT_VOC_ID = "voc_id";                                     // VOCID
		public const string FIELD_CSINCIDENT_VOC_MEMO = "voc_memo";                                 // VOCメモ
		public const string FIELD_CSINCIDENT_COMMENT = "comment";                                   // コメント
		public const string FIELD_CSINCIDENT_IMPORTANCE = "importance";                             // 重要度
		public const string FIELD_CSINCIDENT_USER_NAME = "user_name";                               // 問合せ元名称
		public const string FIELD_CSINCIDENT_USER_CONTACT = "user_contact";                         // 問合せ元連絡先
		public const string FIELD_CSINCIDENT_CS_GROUP_ID = "cs_group_id";                           // 担当グループ
		public const string FIELD_CSINCIDENT_OPERATOR_ID = "operator_id";                           // 担当オペレータ
		public const string FIELD_CSINCIDENT_LOCK_STATUS = "lock_status";                           // ロック状態
		public const string FIELD_CSINCIDENT_LOCK_OPERATOR_ID = "lock_operator_id";                 // ロック保持者
		public const string FIELD_CSINCIDENT_VALID_FLG = "valid_flg";                               // 有効フラグ
		public const string FIELD_CSINCIDENT_DATE_LAST_RECEIVED = "date_last_received";             // 最終受付日時
		/// <summary>最終送受信日時</summary>
		public const string FIELD_CSINCIDENT_DATE_MESSAGE_LAST_SEND_RECEIVED = "date_message_last_send_received";
		public const string FIELD_CSINCIDENT_DATE_COMPLETED = "date_completed";                     // 対応完了日
		public const string FIELD_CSINCIDENT_DATE_CREATED = "date_created";                         // 作成日
		public const string FIELD_CSINCIDENT_DATE_CHANGED = "date_changed";                         // 更新日
		public const string FIELD_CSINCIDENT_LAST_CHANGED = "last_changed";                         // 最終更新者

		// インシデント集計区分値
		public const string TABLE_CSINCIDENTSUMMARYVALUE = "w2_CsIncidentSummaryValue";             // インシデント集計区分値
		public const string FIELD_CSINCIDENTSUMMARYVALUE_DEPT_ID = "dept_id";                       // 識別ID
		public const string FIELD_CSINCIDENTSUMMARYVALUE_INCIDENT_ID = "incident_id";               // インシデントID
		public const string FIELD_CSINCIDENTSUMMARYVALUE_SUMMARY_NO = "summary_no";                 // 集計区分番号
		public const string FIELD_CSINCIDENTSUMMARYVALUE_VALUE = "value";                           // 集計区分値
		public const string FIELD_CSINCIDENTSUMMARYVALUE_DATE_CREATED = "date_created";             // 作成日
		public const string FIELD_CSINCIDENTSUMMARYVALUE_DATE_CHANGED = "date_changed";             // 更新日
		public const string FIELD_CSINCIDENTSUMMARYVALUE_LAST_CHANGED = "last_changed";             // 最終更新者

		/// <summary>CSインシデント警告アイコン</summary>
		public const string TABLE_CSINCIDENTWARNINGICON = "w2_CsIncidentWarningIcon";
		/// <summary>CSインシデント警告アイコン：識別ID</summary>
		public const string FIELD_CSINCIDENTWARNINGICON_DEPT_ID = "dept_id";
		/// <summary>CSインシデント警告アイコン：オペレータID</summary>
		public const string FIELD_CSINCIDENTWARNINGICON_OPERATOR_ID = "operator_id";
		/// <summary>CSインシデント警告アイコン：インシデントステータス</summary>
		public const string FIELD_CSINCIDENTWARNINGICON_INCIDENT_STATUS = "incident_status";
		/// <summary>CSインシデント警告アイコン：警告レベル</summary>
		public const string FIELD_CSINCIDENTWARNINGICON_WARNING_LEVEL = "warning_level";
		/// <summary>CSインシデント警告アイコン：アイコン切替時間</summary>
		public const string FIELD_CSINCIDENTWARNINGICON_TERM = "term";
		/// <summary>CSインシデント警告アイコン：作成日</summary>
		public const string FIELD_CSINCIDENTWARNINGICON_DATE_CREATED = "date_created";
		/// <summary>CSインシデント警告アイコン：最終更新者</summary>
		public const string FIELD_CSINCIDENTWARNINGICON_LAST_CHANGED = "last_changed";

		// メッセージマスタ
		public const string TABLE_CSMESSAGE = "w2_CsMessage";                                       // メッセージマスタ
		public const string FIELD_CSMESSAGE_DEPT_ID = "dept_id";                                    // 識別ID
		public const string FIELD_CSMESSAGE_INCIDENT_ID = "incident_id";                            // インシデントID
		public const string FIELD_CSMESSAGE_MESSAGE_NO = "message_no";                              // メッセージ番号
		public const string FIELD_CSMESSAGE_MEDIA_KBN = "media_kbn";                                // 問合せ媒体
		public const string FIELD_CSMESSAGE_DIRECTION_KBN = "direction_kbn";                        // 受発信区分
		public const string FIELD_CSMESSAGE_OPERATOR_ID = "operator_id";                            // 作成オペレータID
		public const string FIELD_CSMESSAGE_INQUIRY_REPLY_DATE = "inquiry_reply_date";              // 問合せ・回答日時
		public const string FIELD_CSMESSAGE_MESSAGE_STATUS = "message_status";                      // メッセージステータス
		public const string FIELD_CSMESSAGE_USER_NAME1 = "user_name1";                              // 顧客氏名1
		public const string FIELD_CSMESSAGE_USER_NAME2 = "user_name2";                              // 顧客氏名2
		public const string FIELD_CSMESSAGE_USER_NAME_KANA1 = "user_name_kana1";                    // 顧客氏名かな1
		public const string FIELD_CSMESSAGE_USER_NAME_KANA2 = "user_name_kana2";                    // 顧客氏名かな2
		public const string FIELD_CSMESSAGE_USER_MAIL_ADDR = "user_mail_addr";                      // 顧客メールアドレス
		public const string FIELD_CSMESSAGE_USER_TEL1 = "user_tel1";                                // 顧客電話番号1
		public const string FIELD_CSMESSAGE_INQUIRY_TITLE = "inquiry_title";                        // 問合せ件名
		public const string FIELD_CSMESSAGE_INQUIRY_TEXT = "inquiry_text";                          // 問合せ内容
		public const string FIELD_CSMESSAGE_REPLY_TEXT = "reply_text";                              // 回答内容
		public const string FIELD_CSMESSAGE_REPLY_OPERATOR_ID = "reply_operator_id";                // 回答オペレータID
		public const string FIELD_CSMESSAGE_RECEIVE_MAIL_ID = "receive_mail_id";                    // 受信メール識別ID
		public const string FIELD_CSMESSAGE_SEND_MAIL_ID = "send_mail_id";                          // 送信メール識別ID
		public const string FIELD_CSMESSAGE_VALID_FLG = "valid_flg";                                // 有効フラグ
		public const string FIELD_CSMESSAGE_DATE_CREATED = "date_created";                          // 作成日
		public const string FIELD_CSMESSAGE_DATE_CHANGED = "date_changed";                          // 更新日
		public const string FIELD_CSMESSAGE_LAST_CHANGED = "last_changed";                          // 最終更新者

		// メッセージメール
		public const string TABLE_CSMESSAGEMAIL = "w2_CsMessageMail";                               // メッセージメール
		public const string FIELD_CSMESSAGEMAIL_DEPT_ID = "dept_id";                                // 識別ID
		public const string FIELD_CSMESSAGEMAIL_MAIL_ID = "mail_id";                                // メールID
		public const string FIELD_CSMESSAGEMAIL_MAIL_KBN = "mail_kbn";                              // メール区分
		public const string FIELD_CSMESSAGEMAIL_MAIL_FROM = "mail_from";                            // メールFROM
		public const string FIELD_CSMESSAGEMAIL_MAIL_TO = "mail_to";                                // メールTO
		public const string FIELD_CSMESSAGEMAIL_MAIL_CC = "mail_cc";                                // メールCC
		public const string FIELD_CSMESSAGEMAIL_MAIL_BCC = "mail_bcc";                              // メールBCC
		public const string FIELD_CSMESSAGEMAIL_MAIL_SUBJECT = "mail_subject";                      // メール件名
		public const string FIELD_CSMESSAGEMAIL_MAIL_BODY = "mail_body";                            // メール本文
		public const string FIELD_CSMESSAGEMAIL_SEND_OPERATOR_ID = "send_operator_id";              // 送信オペレータID
		public const string FIELD_CSMESSAGEMAIL_SEND_DATETIME = "send_datetime";                    // 送信日時
		public const string FIELD_CSMESSAGEMAIL_RECEIVE_DATETIME = "receive_datetime";              // 受信日時
		public const string FIELD_CSMESSAGEMAIL_MESSAGE_ID = "message_id";                          // Message-Id
		public const string FIELD_CSMESSAGEMAIL_IN_REPLY_TO = "in_reply_to";                        // In-Reply-To
		public const string FIELD_CSMESSAGEMAIL_DEL_FLG = "del_flg";                                // 削除フラグ
		public const string FIELD_CSMESSAGEMAIL_DATE_CREATED = "date_created";                      // 作成日
		public const string FIELD_CSMESSAGEMAIL_DATE_CHANGED = "date_changed";                      // 更新日
		public const string FIELD_CSMESSAGEMAIL_LAST_CHANGED = "last_changed";                      // 最終更新者

		// メッセージメールデータ
		public const string TABLE_CSMESSAGEMAILDATA = "w2_CsMessageMailData";                       // メッセージメールデータ
		public const string FIELD_CSMESSAGEMAILDATA_DEPT_ID = "dept_id";                            // 識別ID
		public const string FIELD_CSMESSAGEMAILDATA_MAIL_ID = "mail_id";                            // メールID
		public const string FIELD_CSMESSAGEMAILDATA_MAIL_DATA = "mail_data";                        // メールデータ

		// メッセージメール添付ファイル
		public const string TABLE_CSMESSAGEMAILATTACHMENT = "w2_CsMessageMailAttachment";           // メッセージメール添付ファイル
		public const string FIELD_CSMESSAGEMAILATTACHMENT_DEPT_ID = "dept_id";                      // 識別ID
		public const string FIELD_CSMESSAGEMAILATTACHMENT_MAIL_ID = "mail_id";                      // メールID
		public const string FIELD_CSMESSAGEMAILATTACHMENT_FILE_NO = "file_no";                      // ファイル枝番
		public const string FIELD_CSMESSAGEMAILATTACHMENT_FILE_NAME = "file_name";                  // ファイル名
		public const string FIELD_CSMESSAGEMAILATTACHMENT_FILE_DATA = "file_data";                  // ファイルデータ
		public const string FIELD_CSMESSAGEMAILATTACHMENT_DATE_CREATED = "date_created";            // 作成日

		// メッセージ依頼マスタ
		public const string TABLE_CSMESSAGEREQUEST = "w2_CsMessageRequest";                         // メッセージ依頼マスタ
		public const string FIELD_CSMESSAGEREQUEST_DEPT_ID = "dept_id";                             // 識別ID
		public const string FIELD_CSMESSAGEREQUEST_INCIDENT_ID = "incident_id";                     // インシデントID
		public const string FIELD_CSMESSAGEREQUEST_MESSAGE_NO = "message_no";                       // メッセージ番号
		public const string FIELD_CSMESSAGEREQUEST_REQUEST_NO = "request_no";                       // 依頼番号
		public const string FIELD_CSMESSAGEREQUEST_REQUEST_OPERATOR_ID = "request_operator_id";     // 依頼者オペレータID
		public const string FIELD_CSMESSAGEREQUEST_REQUEST_STATUS = "request_status";               // 依頼ステータス
		public const string FIELD_CSMESSAGEREQUEST_REQUEST_TYPE = "request_type";                   // 依頼種別
		public const string FIELD_CSMESSAGEREQUEST_URGENCY_FLG = "urgency_flg";                     // 緊急度
		public const string FIELD_CSMESSAGEREQUEST_APPROVAL_TYPE = "approval_type";                 // 承認方法
		public const string FIELD_CSMESSAGEREQUEST_COMMENT = "comment";                             // 承認依頼コメント
		public const string FIELD_CSMESSAGEREQUEST_WORKING_OPERATOR_ID = "working_operator_id";     // 作業中オペレータID
		public const string FIELD_CSMESSAGEREQUEST_DATE_CREATED = "date_created";                   // 作成日
		public const string FIELD_CSMESSAGEREQUEST_DATE_CHANGED = "date_changed";                   // 更新日
		public const string FIELD_CSMESSAGEREQUEST_LAST_CHANGED = "last_changed";                   // 最終更新者

		// メッセージ依頼アイテムマスタ
		public const string TABLE_CSMESSAGEREQUESTITEM = "w2_CsMessageRequestItem";                 // メッセージ依頼アイテムマスタ
		public const string FIELD_CSMESSAGEREQUESTITEM_DEPT_ID = "dept_id";                         // 識別ID
		public const string FIELD_CSMESSAGEREQUESTITEM_INCIDENT_ID = "incident_id";                 // インシデントID
		public const string FIELD_CSMESSAGEREQUESTITEM_MESSAGE_NO = "message_no";                   // メッセージ番号
		public const string FIELD_CSMESSAGEREQUESTITEM_REQUEST_NO = "request_no";                   // 依頼番号
		public const string FIELD_CSMESSAGEREQUESTITEM_BRANCH_NO = "branch_no";                     // 枝番
		public const string FIELD_CSMESSAGEREQUESTITEM_APPR_OPERATOR_ID = "appr_operator_id";       // 承認者オペレータID
		public const string FIELD_CSMESSAGEREQUESTITEM_RESULT_STATUS = "result_status";             // 結果ステータス
		public const string FIELD_CSMESSAGEREQUESTITEM_COMMENT = "comment";                         // 結果理由
		public const string FIELD_CSMESSAGEREQUESTITEM_DATE_STATUS_CHANGED = "date_status_changed"; // ステータス変更日
		public const string FIELD_CSMESSAGEREQUESTITEM_DATE_CREATED = "date_created";               // 作成日
		public const string FIELD_CSMESSAGEREQUESTITEM_DATE_CHANGED = "date_changed";               // 更新日
		public const string FIELD_CSMESSAGEREQUESTITEM_LAST_CHANGED = "last_changed";               // 最終更新者

		// 共有情報マスタ
		public const string TABLE_CSSHAREINFO = "w2_CsShareInfo";                                   // 共有情報マスタ
		public const string FIELD_CSSHAREINFO_DEPT_ID = "dept_id";                                  // 識別ID
		public const string FIELD_CSSHAREINFO_INFO_NO = "info_no";                                  // 共有情報NO
		public const string FIELD_CSSHAREINFO_INFO_TEXT_KBN = "info_text_kbn";                      // 共有情報テキスト区分
		public const string FIELD_CSSHAREINFO_INFO_TEXT = "info_text";                              // 共有情報テキスト
		public const string FIELD_CSSHAREINFO_INFO_KBN = "info_kbn";                                // 共有情報区分
		public const string FIELD_CSSHAREINFO_INFO_IMPORTANCE = "info_importance";                  // 共有情報重要度
		public const string FIELD_CSSHAREINFO_SENDER = "sender";                                    // 送信元オペレータID
		public const string FIELD_CSSHAREINFO_DEL_FLG = "del_flg";                                  // 削除フラグ
		public const string FIELD_CSSHAREINFO_DATE_CREATED = "date_created";                        // 作成日
		public const string FIELD_CSSHAREINFO_DATE_CHANGED = "date_changed";                        // 更新日
		public const string FIELD_CSSHAREINFO_LAST_CHANGED = "last_changed";                        // 最終更新者

		// 共有情報既読管理マスタ
		public const string TABLE_CSSHAREINFOREAD = "w2_CsShareInfoRead";                           // 共有情報既読管理マスタ
		public const string FIELD_CSSHAREINFOREAD_DEPT_ID = "dept_id";                              // 識別ID
		public const string FIELD_CSSHAREINFOREAD_INFO_NO = "info_no";                              // 共有情報NO
		public const string FIELD_CSSHAREINFOREAD_OPERATOR_ID = "operator_id";                      // オペレータID
		public const string FIELD_CSSHAREINFOREAD_READ_FLG = "read_flg";                            // 既読フラグ
		public const string FIELD_CSSHAREINFOREAD_PINNED_FLG = "pinned_flg";                        // ピン止めフラグ
		public const string FIELD_CSSHAREINFOREAD_DEL_FLG = "del_flg";                              // 削除フラグ
		public const string FIELD_CSSHAREINFOREAD_DATE_CREATED = "date_created";                    // 作成日
		public const string FIELD_CSSHAREINFOREAD_DATE_CHANGED = "date_changed";                    // 更新日
		public const string FIELD_CSSHAREINFOREAD_LAST_CHANGED = "last_changed";                    // 最終更新者

		// 受信メール振分設定
		public const string TABLE_CSMAILASSIGNSETTING = "w2_CsMailAssignSetting";                   // 受信メール振分設定
		public const string FIELD_CSMAILASSIGNSETTING_DEPT_ID = "dept_id";                          // 識別ID
		public const string FIELD_CSMAILASSIGNSETTING_MAIL_ASSIGN_ID = "mail_assign_id";            // メール振分設定ID
		public const string FIELD_CSMAILASSIGNSETTING_ASSIGN_PRIORITY = "assign_priority";          // 振分優先順
		public const string FIELD_CSMAILASSIGNSETTING_LOGICAL_OPERATOR = "logical_operator";        // 論理演算子
		public const string FIELD_CSMAILASSIGNSETTING_STOP_FILTERING = "stop_filtering";            // 振分け停止
		public const string FIELD_CSMAILASSIGNSETTING_ASSIGN_INCIDENT_CATEGORY_ID = "assign_incident_category_id";// 振分先インシデントカテゴリID
		public const string FIELD_CSMAILASSIGNSETTING_ASSIGN_OPERATOR_ID = "assign_operator_id";    // 振分後担当オペレータID
		public const string FIELD_CSMAILASSIGNSETTING_ASSIGN_CS_GROUP_ID = "assign_cs_group_id";    // 振分後担当CSグループID
		public const string FIELD_CSMAILASSIGNSETTING_ASSIGN_IMPORTANCE = "assign_importance";      // 振分後重要度
		public const string FIELD_CSMAILASSIGNSETTING_ASSIGN_STATUS = "assign_status";              // 振分後ステータス
		public const string FIELD_CSMAILASSIGNSETTING_TRASH = "trash";                              // ごみ箱
		public const string FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE = "auto_response";              // オートレスポンス
		public const string FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_FROM = "auto_response_from";    // オートレスポンス送信元アドレス
		public const string FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_CC = "auto_response_cc";        // オートレスポンスCC
		public const string FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_BCC = "auto_response_bcc";      // オートレスポンスBCC
		public const string FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_SUBJECT = "auto_response_subject";// オートレスポンス件名
		public const string FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_BODY = "auto_response_body";    // オートレスポンス本文
		public const string FIELD_CSMAILASSIGNSETTING_VALID_FLG = "valid_flg";                      // 有効フラグ
		public const string FIELD_CSMAILASSIGNSETTING_DATE_CREATED = "date_created";                // 作成日
		public const string FIELD_CSMAILASSIGNSETTING_DATE_CHANGED = "date_changed";                // 更新日
		public const string FIELD_CSMAILASSIGNSETTING_LAST_CHANGED = "last_changed";                // 最終更新者
		public const string FIELD_CSMAILASSIGNSETTING_MAIL_ASSIGN_NAME = "mail_assign_name";		// 振分設定名

		// 受信メール振分設定アイテム
		public const string TABLE_CSMAILASSIGNSETTINGITEM = "w2_CsMailAssignSettingItem";           // 受信メール振分設定アイテム
		public const string FIELD_CSMAILASSIGNSETTINGITEM_DEPT_ID = "dept_id";                      // 識別ID
		public const string FIELD_CSMAILASSIGNSETTINGITEM_MAIL_ASSIGN_ID = "mail_assign_id";        // メール振分設定ID
		public const string FIELD_CSMAILASSIGNSETTINGITEM_ITEM_NO = "item_no";                      // アイテム番号
		public const string FIELD_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET = "matching_target";      // マッチング対象
		public const string FIELD_CSMAILASSIGNSETTINGITEM_MATCHING_VALUE = "matching_value";        // マッチング値
		public const string FIELD_CSMAILASSIGNSETTINGITEM_MATCHING_TYPE = "matching_type";          // マッチング種別
		public const string FIELD_CSMAILASSIGNSETTINGITEM_IGNORE_CASE = "ignore_case";              // 大文字小文字非区別
		public const string FIELD_CSMAILASSIGNSETTINGITEM_DATE_CREATED = "date_created";            // 作成日
		public const string FIELD_CSMAILASSIGNSETTINGITEM_DATE_CHANGED = "date_changed";            // 更新日
		public const string FIELD_CSMAILASSIGNSETTINGITEM_LAST_CHANGED = "last_changed";            // 最終更新者

		// CSオペレータマスタ
		public const string TABLE_CSOPERATOR = "w2_CsOperator";                                     // CSオペレータマスタ
		public const string FIELD_CSOPERATOR_DEPT_ID = "dept_id";                                   // 識別ID
		public const string FIELD_CSOPERATOR_OPERATOR_ID = "operator_id";                           // オペレータID
		public const string FIELD_CSOPERATOR_OPERATOR_AUTHORITY_ID = "operator_authority_id";       // オペレータ権限ID
		public const string FIELD_CSOPERATOR_MAIL_FROM_ID = "mail_from_id";                         // メール送信元ID
		public const string FIELD_CSOPERATOR_NOTIFY_INFO_FLG = "notify_info_flg";                   // 情報メール通知フラグ
		public const string FIELD_CSOPERATOR_NOTIFY_WARN_FLG = "notify_warn_flg";                   // 警告メール通知フラグ
		public const string FIELD_CSOPERATOR_MAIL_ADDR = "mail_addr";                               // メールアドレス
		public const string FIELD_CSOPERATOR_DISPLAY_ORDER = "display_order";                       // 表示順
		public const string FIELD_CSOPERATOR_DATE_CREATED = "date_created";                         // 作成日
		public const string FIELD_CSOPERATOR_DATE_CHANGED = "date_changed";                         // 更新日
		public const string FIELD_CSOPERATOR_LAST_CHANGED = "last_changed";                         // 最終更新者

		// CSオペレータ権限マスタ
		public const string TABLE_CSOPERATORAUTHORITY = "w2_CsOperatorAuthority";                   // CSオペレータ権限マスタ
		public const string FIELD_CSOPERATORAUTHORITY_DEPT_ID = "dept_id";                          // 識別ID
		public const string FIELD_CSOPERATORAUTHORITY_OPERATOR_AUTHORITY_ID = "operator_authority_id";// オペレータ権限ID
		public const string FIELD_CSOPERATORAUTHORITY_OPERATOR_AUTHORITY_NAME = "operator_authority_name";// オペレータ権限名
		public const string FIELD_CSOPERATORAUTHORITY_PERMIT_EDIT_FLG = "permit_edit_flg";          // 編集許可フラグ
		public const string FIELD_CSOPERATORAUTHORITY_PERMIT_MAIL_SEND_FLG = "permit_mail_send_flg";// メール直接送信許可フラグ
		public const string FIELD_CSOPERATORAUTHORITY_PERMIT_APPROVAL_FLG = "permit_approval_flg";  // 承認受付許可フラグ
		public const string FIELD_CSOPERATORAUTHORITY_PERMIT_UNLOCK_FLG = "permit_unlock_flg";      // ロック解除許可フラグ
		public const string FIELD_CSOPERATORAUTHORITY_PERMIT_EDIT_SIGNATURE_FLG = "permit_edit_signature_flg";// 共通署名編集許可フラグ
		public const string FIELD_CSOPERATORAUTHORITY_RECEIVE_NO_ASSIGN_WARNING_FLG = "receive_no_assign_warning_flg";// 担当未設定警告メール受け取りフラグ
		public const string FIELD_CSOPERATORAUTHORITY_DATE_CREATED = "date_created";                // 作成日
		public const string FIELD_CSOPERATORAUTHORITY_DATE_CHANGED = "date_changed";                // 更新日
		public const string FIELD_CSOPERATORAUTHORITY_LAST_CHANGED = "last_changed";                // 最終更新者
		public const string FIELD_CSOPERATORAUTHORITY_PERMIT_PERMANENT_DELETE_FLG = "permit_permanent_delete_flg";// 完全削除許可フラグ

		// CSグループマスタ
		public const string TABLE_CSGROUP = "w2_CsGroup";                                           // CSグループマスタ
		public const string FIELD_CSGROUP_DEPT_ID = "dept_id";                                      // 識別ID
		public const string FIELD_CSGROUP_CS_GROUP_ID = "cs_group_id";                              // CSグループID
		public const string FIELD_CSGROUP_CS_GROUP_NAME = "cs_group_name";                          // CSグループ名
		public const string FIELD_CSGROUP_DISPLAY_ORDER = "display_order";                          // 表示順
		public const string FIELD_CSGROUP_VALID_FLG = "valid_flg";                                  // 有効フラグ
		public const string FIELD_CSGROUP_DATE_CREATED = "date_created";                            // 作成日
		public const string FIELD_CSGROUP_DATE_CHANGED = "date_changed";                            // 更新日
		public const string FIELD_CSGROUP_LAST_CHANGED = "last_changed";                            // 最終更新者

		// CSオペレータ所属グループ
		public const string TABLE_CSOPERATORGROUP = "w2_CsOperatorGroup";                           // CSオペレータ所属グループ
		public const string FIELD_CSOPERATORGROUP_DEPT_ID = "dept_id";                              // 識別ID
		public const string FIELD_CSOPERATORGROUP_CS_GROUP_ID = "cs_group_id";                      // CSグループID
		public const string FIELD_CSOPERATORGROUP_OPERATOR_ID = "operator_id";                      // オペレータID
		public const string FIELD_CSOPERATORGROUP_DATE_CREATED = "date_created";                    // 作成日
		public const string FIELD_CSOPERATORGROUP_LAST_CHANGED = "last_changed";                    // 最終更新者

		// メール送信ログ
		public const string TABLE_MAILSENDLOG = "w2_MailSendLog";                                   // メール送信ログ
		public const string FIELD_MAILSENDLOG_LOG_NO = "log_no";                                    // ログNO
		public const string FIELD_MAILSENDLOG_USER_ID = "user_id";                                  // ユーザーID
		public const string FIELD_MAILSENDLOG_DEPT_ID = "dept_id";                                  // 識別ID
		public const string FIELD_MAILSENDLOG_MAILTEXT_ID = "mailtext_id";                          // メール文章ID
		public const string FIELD_MAILSENDLOG_MAILTEXT_NAME = "mailtext_name";                      // メール文章名
		public const string FIELD_MAILSENDLOG_MAILDIST_ID = "maildist_id";                          // メール配信設定ID
		public const string FIELD_MAILSENDLOG_MAILDIST_NAME = "maildist_name";                      // メール配信設定名
		public const string FIELD_MAILSENDLOG_SHOP_ID = "shop_id";                                  // 店舗ID
		public const string FIELD_MAILSENDLOG_MAIL_ID = "mail_id";                                  // メールテンプレートID
		public const string FIELD_MAILSENDLOG_MAIL_NAME = "mail_name";                              // メールテンプレート名
		public const string FIELD_MAILSENDLOG_MAIL_FROM_NAME = "mail_from_name";                    // 送信元名
		public const string FIELD_MAILSENDLOG_MAIL_FROM = "mail_from";                              // メールFrom
		public const string FIELD_MAILSENDLOG_MAIL_TO = "mail_to";                                  // メールTo
		public const string FIELD_MAILSENDLOG_MAIL_CC = "mail_cc";                                  // メールCC
		public const string FIELD_MAILSENDLOG_MAIL_BCC = "mail_bcc";                                // メールBCC
		public const string FIELD_MAILSENDLOG_MAIL_SUBJECT = "mail_subject";                        // メールタイトル
		public const string FIELD_MAILSENDLOG_MAIL_BODY = "mail_body";                              // メール本文
		public const string FIELD_MAILSENDLOG_MAIL_BODY_HTML = "mail_body_html";                    // メール本文HTML
		public const string FIELD_MAILSENDLOG_ERROR_MESSAGE = "error_message";                      // エラーメッセージ
		public const string FIELD_MAILSENDLOG_DATE_SEND_MAIL = "date_send_mail";                    // 送信日時
		public const string FIELD_MAILSENDLOG_DATE_CREATED = "date_created";                        // 作成日
		public const string FIELD_MAILSENDLOG_READ_FLG = "read_flg";                                // 既読フラグ
		public const string FIELD_MAILSENDLOG_DATE_READ_MAIL = "date_read_mail";                    // 既読日
		public const string FIELD_MAILSENDLOG_TEXT_HISTORY_NO = "text_history_no";                  // メール配信時文章履歴NO
		public const string FIELD_MAILSENDLOG_MAIL_ADDR_KBN = "mail_addr_kbn";                      // メールアドレス区分

		// メール配信時文章履歴
		public const string TABLE_MAILSENDTEXTHISTORY = "w2_MailSendTextHistory";                   // メール配信時文章履歴
		public const string FIELD_MAILSENDTEXTHISTORY_TEXT_HISTORY_NO = "text_history_no";          // メール配信時文章履歴NO
		public const string FIELD_MAILSENDTEXTHISTORY_DEPT_ID = "dept_id";                          // 識別ID
		public const string FIELD_MAILSENDTEXTHISTORY_MAILTEXT_ID = "mailtext_id";                  // メール文章ID
		public const string FIELD_MAILSENDTEXTHISTORY_MAILDIST_ID = "maildist_id";                  // メール配信設定ID
		public const string FIELD_MAILSENDTEXTHISTORY_MAILDIST_NAME = "maildist_name";              // メール配信設定名
		public const string FIELD_MAILSENDTEXTHISTORY_MAIL_BODY = "mail_body";                      // メール本文
		public const string FIELD_MAILSENDTEXTHISTORY_MAIL_BODY_HTML = "mail_body_html";            // メール本文HTML
		public const string FIELD_MAILSENDTEXTHISTORY_MAILTEXT_BODY_MOBILE = "mailtext_body_mobile";// メール文章モバイル
		public const string FIELD_MAILSENDTEXTHISTORY_MAILTEXT_DECOME = "mailtext_decome";          // メール文章デコメ
		public const string FIELD_MAILSENDTEXTHISTORY_DATE_CREATED = "date_created";                // 作成日
		public const string FIELD_MAILSENDTEXTHISTORY_MAILTEXT_REPLACE_TAGS = "mailtext_replace_tags"; //メール文章タグ置換情報

		// DM発送履歴マスタ
		/// <summary>DM発送履歴マスタ</summary>
		public const string TABLE_DMSHIPPINGHISTORY = "w2_DmShippingHistory";
		/// <summary>ユーザーID</summary>
		public const string FIELD_DMSHIPPINGHISTORY_USER_ID = "user_id";
		/// <summary>DMコード</summary>
		public const string FIELD_DMSHIPPINGHISTORY_DM_CODE = "dm_code";
		/// <summary>DM発送日</summary>
		public const string FIELD_DMSHIPPINGHISTORY_DM_SHIPPING_DATE = "dm_shipping_date";
		/// <summary>DM名</summary>
		public const string FIELD_DMSHIPPINGHISTORY_DM_NAME = "dm_name";
		/// <summary>有効期間(From)</summary>
		public const string FIELD_DMSHIPPINGHISTORY_VALID_DATE_FROM = "valid_date_from";
		/// <summary>有効期間(To)</summary>
		public const string FIELD_DMSHIPPINGHISTORY_VALID_DATE_TO = "valid_date_to";
		/// <summary>作成日</summary>
		public const string FIELD_DMSHIPPINGHISTORY_DATE_CREATED = "date_created";
		/// <summary>更新日</summary>
		public const string FIELD_DMSHIPPINGHISTORY_DATE_CHANGED = "date_changed";
		/// <summary>最終更新者</summary>
		public const string FIELD_DMSHIPPINGHISTORY_LAST_CHANGED = "last_changed";

		/// <summary>Cs incident work table</summary>
		public const string TABLE_WORKCSINCIDENT = "w2_WorkCsIncident";
		/// <summary>Cs message work table</summary>
		public const string TABLE_WORKCSMESSAGE = "w2_WorkCsMessage";

		#endregion

		#region フィールドフラグ定数

		// インシデントカテゴリマスタ：有効フラグ
		public const string FLG_CSINCIDENTCATEGORY_VALID_FLG_VALID = "1";	// 有効
		public const string FLG_CSINCIDENTCATEGORY_VALID_FLG_INVALID = "0";	// 無効

		// インシデントVOCマスタ：有効フラグ
		public const string FLG_CSINCIDENTVOC_VALID_FLG_VALID = "1";		// 有効
		public const string FLG_CSINCIDENTVOC_VALID_FLG_INVALID = "0";		// 無効

		// 集計区分マスタ：集計区分入力種別
		public const string FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_RADIO = "RB";		// ラジオボタン選択
		public const string FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_DROPDOWN = "DDL";	// ドロップダウン選択
		public const string FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_TEXT = "TB";		// テキスト入力

		// 集計区分マスタ：有効フラグ
		public const string FLG_CSSUMMARYSETTING_VALID_FLG_VALID = "1";		// 有効
		public const string FLG_CSSUMMARYSETTING_VALID_FLG_INVALID = "0";	// 無効

		// 集計区分アイテムマスタ：有効フラグ
		public const string FLG_CSSUMMARYSETTINGITEM_VALID_FLG_VALID = "1";		// 有効
		public const string FLG_CSSUMMARYSETTINGITEM_VALID_FLG_INVALID = "0";	// 無効

		// 回答例マスタ：有効フラグ
		public const string FLG_CSANSWERTEMPLATE_VALID_FLG_VALID = "1";		// 有効
		public const string FLG_CSANSWERTEMPLATE_VALID_FLG_INVALID = "0";	// 無効

		// 外部リンク設定マスタ：有効フラグ
		public const string FLG_CSEXTERNALLINK_VALID_FLG_VALID = "1";		// 有効
		public const string FLG_CSEXTERNALLINK_VALID_FLG_INVALID = "0";	// 無効

		// 回答例カテゴリマスタ：有効フラグ
		public const string FLG_CSANSWERTEMPLATECATEGORY_VALID_FLG_VALID = "1";		// 有効
		public const string FLG_CSANSWERTEMPLATECATEGORY_VALID_FLG_INVALID = "0";	// 無効

		// メール署名マスタ：有効フラグ
		public const string FLG_CSMAILSIGNATURE_VALID_FLG_VALID = "1";		// 有効
		public const string FLG_CSMAILSIGNATURE_VALID_FLG_INVALID = "0";	// 無効

		// メール送信元マスタ：有効フラグ
		public const string FLG_CSMAILFROM_VALID_FLG_VALID = "1";	// 有効
		public const string FLG_CSMAILFROM_VALID_FLG_INVALID = "0";	// 無効

		// インシデントマスタ：ステータス
		public const string FLG_CSINCIDENT_STATUS_NONE = "NONE";			// 未対応
		public const string FLG_CSINCIDENT_STATUS_ACTIVE = "ACTIVE";		// 対応中
		public const string FLG_CSINCIDENT_STATUS_SUSPEND = "SUSPEND";		// 保留
		public const string FLG_CSINCIDENT_STATUS_URGENT = "URGENT";		// 至急
		public const string FLG_CSINCIDENT_STATUS_COMPLETE = "COMPLETE";	// 完了

		// インシデントマスタ：重要度
		public const string FLG_CSINCIDENT_IMPORTANCE_HIGH = "5";			// 高
		public const string FLG_CSINCIDENT_IMPORTANCE_UPPERMIDDLE = "4";	// やや高
		public const string FLG_CSINCIDENT_IMPORTANCE_MIDDLE = "3";			// 普通
		public const string FLG_CSINCIDENT_IMPORTANCE_LOWERMIDDLE = "2";	// やや低
		public const string FLG_CSINCIDENT_IMPORTANCE_LOW = "1";			// 低

		// インシデントマスタ：ロック状態
		public const string FLG_CSINCIDENT_LOCK_STATUS_NONE = "";				// ロック無し
		public const string FLG_CSINCIDENT_LOCK_STATUS_EDIT = "EDIT";			// 編集中ロック
		public const string FLG_CSINCIDENT_LOCK_STATUS_DRAFT = "DRAFT";			// 下書きロック
		public const string FLG_CSINCIDENT_LOCK_STATUS_APPR_REQ = "APPR_REQ";	// 承認依頼ロック
		public const string FLG_CSINCIDENT_LOCK_STATUS_SEND_REQ = "SEND_REQ";	// 送信依頼ロック

		// インシデントマスタ：有効フラグ
		public const string FLG_CSINCIDENT_VALID_FLG_VALID = "1";			// 有効
		public const string FLG_CSINCIDENT_VALID_FLG_INVALID = "0";			// 無効（ゴミ箱）

		// インシデントマスタ : Sort Kbn
		public const string FLG_SORT_KBN_ASC = "ASC";	// Ascending
		public const string FLG_SORT_KBN_DESC = "DESC";	// Descending

		// インシデントマスタ : Sort Symbol
		public const string FLG_SORT_SYMBOL_ASC = "▲";	// Ascending symbol
		public const string FLG_SORT_SYMBOL_DESC = "▼";	// Descending symbol

		/// <summary>インシデント警告アイコンテーブル警告レベル：橙色</summary>
		public const string FLG_CSINCIDENT_WARNING_LEVEL_ORANGE = "0";
		/// <summary>インシデント警告アイコンテーブル警告レベル：赤色</summary>
		public const string FLG_CSINCIDENT_WARNING_LEVEL_RED = "1";

		// メッセージマスタ：問合せ媒体
		public const string FLG_CSMESSAGE_MEDIA_KBN_TEL = "TEL";		// TEL
		public const string FLG_CSMESSAGE_MEDIA_KBN_MAIL = "MAIL";		// MAIL
		public const string FLG_CSMESSAGE_MEDIA_KBN_OTHERS = "OTHERS";	// その他

		// メッセージマスタ：受発信区分
		public const string FLG_CSMESSAGE_DIRECTION_KBN_RECEIVE = "REC";	// 受信
		public const string FLG_CSMESSAGE_DIRECTION_KBN_SEND = "SEND";		// 発信

		// メッセージマスタ：メッセージステータス
		public const string FLG_CSMESSAGE_MESSAGE_STATUS_RECEIVED = "RECEIVED";		// 受信済み
		public const string FLG_CSMESSAGE_MESSAGE_STATUS_DRAFT = "DRAFT";			// 下書き
		public const string FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_REQ = "APPR_RQ";	// 承認依頼済み
		public const string FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_NG = "APPR_NG";	// 承認差し戻し
		public const string FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_OK = "APPR_OK";	// 承認済み
		public const string FLG_CSMESSAGE_MESSAGE_STATUS_APPROVE_CANCEL = "APPR_CNCL";	// 承認依頼取り下げ
		public const string FLG_CSMESSAGE_MESSAGE_STATUS_SEND_REQ = "SEND_RQ";		// 送信依頼済み
		public const string FLG_CSMESSAGE_MESSAGE_STATUS_SEND_NG = "SEND_NG";		// 送信差し戻し
		public const string FLG_CSMESSAGE_MESSAGE_STATUS_SEND_CANCEL = "SEND_CNCL";	// 送信依頼取り下げ
		public const string FLG_CSMESSAGE_MESSAGE_STATUS_DONE = "DONE";				// 完了

		// メッセージマスタ：有効フラグ
		public const string FLG_CSMESSAGE_VALID_FLG_VALID = "1";					// 有効
		public const string FLG_CSMESSAGE_VALID_FLG_INVALID = "0";					// 無効（ゴミ箱）

		// メッセージメール：メール区分
		public const string FLG_CSMESSAGEMAIL_MAIL_KBN_SEND = "SEND";        // 送信
		public const string FLG_CSMESSAGEMAIL_MAIL_KBN_RECEIVE = "RECV";     // 受信

		// メッセージ依頼マスタ：依頼ステータス
		public const string FLG_CSMESSAGEREQUEST_REQUEST_STATUS_APPROVE_DRAFT = "APPR_DRAFT";	// 承認依頼下書き
		public const string FLG_CSMESSAGEREQUEST_REQUEST_STATUS_APPROVE_REQ = "APPR_RQ";	// 承認依頼済み
		public const string FLG_CSMESSAGEREQUEST_REQUEST_STATUS_APPROVE_NG = "APPR_NG";		// 承認差し戻し
		public const string FLG_CSMESSAGEREQUEST_REQUEST_STATUS_APPROVE_OK = "APPR_OK";		// 承認済み
		public const string FLG_CSMESSAGEREQUEST_REQUEST_STATUS_APPROVE_CANCEL = "APPR_CNCL";	// 送信依頼取り下げ
		public const string FLG_CSMESSAGEREQUEST_REQUEST_STATUS_SEND_DRAFT = "SEND_DRAFT";	// 承認依頼下書き
		public const string FLG_CSMESSAGEREQUEST_REQUEST_STATUS_SEND_REQ = "SEND_RQ";		// 送信依頼済み
		public const string FLG_CSMESSAGEREQUEST_REQUEST_STATUS_SEND_NG = "SEND_NG";		// 送信差し戻し
		public const string FLG_CSMESSAGEREQUEST_REQUEST_STATUS_SEND_OK = "SEND_OK";		// 送信OK
		public const string FLG_CSMESSAGEREQUEST_REQUEST_STATUS_SEND_CANCEL = "SEND_CNCL";	// 送信依頼取り下げ

		// メッセージ依頼マスタ：依頼種別
		public const string FLG_CSMESSAGEREQUEST_REQUEST_TYPE_APPROVE = "APPR";		// 承認依頼
		public const string FLG_CSMESSAGEREQUEST_REQUEST_TYPE_MAILSEND = "SEND";	// 送信依頼

		// メッセージ依頼マスタ：緊急度
		public const string FLG_CSMESSAGEREQUEST_URGENCY_NORMAL = "0";		// 通常
		public const string FLG_CSMESSAGEREQUEST_URGENCY_URGENT = "1";		// 緊急

		// メッセージ依頼マスタ：承認方法
		public const string FLG_CSMESSAGEREQUEST_APPROVAL_TYPE_CONSULTATION = "CONS";	// 合議
		public const string FLG_CSMESSAGEREQUEST_APPROVAL_TYPE_PARALLEL = "PARA";		// 並行

		// メッセージ依頼アイテムマスタ：結果ステータス
		public const string FLG_CSMESSAGEREQUESTITEM_RESULT_STATUS_NONE = "";	// 依頼中
		public const string FLG_CSMESSAGEREQUESTITEM_RESULT_STATUS_OK = "OK";	// 承認・送信完了
		public const string FLG_CSMESSAGEREQUESTITEM_RESULT_STATUS_NG = "NG";	// 差し戻し

		// 共有情報マスタ：共有情報テキスト区分
		public const string FLG_CSSHAREINFO_INFO_TEXT_KBN_TEXT = "0";	// TEXT
		public const string FLG_CSSHAREINFO_INFO_TEXT_KBN_HTML = "1";	// HTML

		// 共有情報既読管理マスタ：既読フラグ
		public const string FLG_CSSHAREINFOREAD_READ_FLG_READ = "1";	// 既読（チェック済み）
		public const string FLG_CSSHAREINFOREAD_READ_FLG_UNREAD = "0";	// 未読（未確認）

		// 共有情報既読管理マスタ：ピン止めフラグ
		public const string FLG_CSSHAREINFOREAD_PINNED_FLG_PINNED = "1";	// ピン止め中
		public const string FLG_CSSHAREINFOREAD_PINNED_FLG_NOPIN = "0";		// 解除

		// 受信メール振分設定：論理演算子
		public const string FLG_CSMAILASSIGNSETTING_LOGICAL_OPERATOR_AND = "AND";	// かつ
		public const string FLG_CSMAILASSIGNSETTING_LOGICAL_OPERATOR_OR = "OR";		// または

		// 受信メール振分設定：振分け停止
		public const string FLG_CSMAILASSIGNSETTING_STOP_FILTERING_VALID = "1";		// 有効
		public const string FLG_CSMAILASSIGNSETTING_STOP_FILTERING_INVALID = "0";	// 無効

		// 受信メール振分設定：担当振分けクリア
		public const string FLG_CSMAILASSIGNSETTING_ASSIGN_OPERATOR_ID_CLEAR = "-1";	// 担当オペレータ振分けクリア
		public const string FLG_CSMAILASSIGNSETTING_ASSIGN_GROUP_ID_CLEAR = "-1";		// 担当CSグループ振分けクリア
		public const string FLG_CSMAILASSIGNSETTING_ASSIGN_CLEAR_TEXT = "(クリア)";		// 担当振分けクリア文字列

		// 受信メール振分設定：振分後重要度
		public const string FLG_CSMAILASSIGNSETTING_ASSIGN_IMPORTANCE_HIGH = "1";	// 重要
		public const string FLG_CSMAILASSIGNSETTING_ASSIGN_IMPORTANCE_NORMAL = "0";	// 普通

		// 受信メール振分設定：振分後ステータス
		public const string FLG_CSMAILASSIGNSETTING_ASSIGN_STATUS_DONE = "1";		// 完了済
		public const string FLG_CSMAILASSIGNSETTING_ASSIGN_STATUS_UNDONE = "0";		// 未完了

		// 受信メール振分設定：ごみ箱
		public const string FLG_CSMAILASSIGNSETTING_TRASH_VALID = "1";				// 有効
		public const string FLG_CSMAILASSIGNSETTING_TRASH_INVALID = "0";			// 無効

		// 受信メール振分設定：オートレスポンス
		public const string FLG_CSMAILASSIGNSETTING_AUTO_RESPONSE_VALID = "1";		// 送信する
		public const string FLG_CSMAILASSIGNSETTING_AUTO_RESPONSE_INVALID = "0";	// 送信しない

		// 受信メール振分設定：有効フラグ
		public const string FLG_CSMAILASSIGNSETTING_VALID_FLG_VALID = "1";			// 有効
		public const string FLG_CSMAILASSIGNSETTING_VALID_FLG_INVALID = "0";		// 無効

		// 受信メール振分設定アイテム：マッチング対象
		public const string FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_SUBJECT = "1";	// 件名
		public const string FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_BODY = "2";		// 本文
		public const string FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_HEADER = "3";	// ヘッダー
		public const string FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_FROM = "4";		// 送信元
		/// <summary>宛先(To,Cc)</summary>
		public const string FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_TOCC = "5";
		/// <summary>宛先(To)</summary>
		public const string FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_TO = "6";
		/// <summary>宛先(Cc)</summary>
		public const string FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET_CC = "7";

		// 受信メール振分設定アイテム：マッチング種別
		public const string FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TYPE_INCLUDE = "1";	// に含まれる
		public const string FLG_CSMAILASSIGNSETTINGITEM_MATCHING_TYPE_NO_INCLUDE = "0";	// に含まれない

		// 受信メール振分設定アイテム：大文字小文字非区別
		public const string FLG_CSMAILASSIGNSETTINGITEM_IGNORE_CASE_VALID = "1";	// 大文字小文字を区別しない
		public const string FLG_CSMAILASSIGNSETTINGITEM_IGNORE_CASE_INVALID = "0";	// 大文字小文字を区別する

		// CSオペレータマスタ：情報メール通知フラグ
		public const string FLG_CSOPERATOR_NOTIFY_INFO_FLG_VALID = "1";			// 有効
		public const string FLG_CSOPERATOR_NOTIFY_INFO_FLG_INVALID = "0";		// 無効

		// CSオペレータマスタ：警告メール通知フラグ
		public const string FLG_CSOPERATOR_NOTIFY_WARN_FLG_VALID = "1";			// 有効
		public const string FLG_CSOPERATOR_NOTIFY_WARN_FLG_INVALID = "0";		// 無効

		// CSオペレータ権限マスタ：編集許可フラグ
		public const string FLG_CSOPERATORAUTHORITY_PERMIT_EDIT_FLG_VALID = "1";				// 有効
		public const string FLG_CSOPERATORAUTHORITY_PERMIT_EDIT_FLG_INVALID = "0";				// 無効

		// CSオペレータ権限マスタ：メール直接送信許可フラグ
		public const string FLG_CSOPERATORAUTHORITY_PERMIT_MAIL_SEND_FLG_VALID = "1";			// 有効
		public const string FLG_CSOPERATORAUTHORITY_PERMIT_MAIL_SEND_FLG_INVALID = "0";			// 無効

		// CSオペレータ権限マスタ：承認受付許可フラグ
		public const string FLG_CSOPERATORAUTHORITY_PERMIT_APPROVAL_FLG_VALID = "1";			// 有効
		public const string FLG_CSOPERATORAUTHORITY_PERMIT_APPROVAL_FLG_INVALID = "0";			// 無効

		// CSオペレータ権限マスタ：ロック解除許可フラグ
		public const string FLG_CSOPERATORAUTHORITY_PERMIT_UNLOCK_FLG_VALID = "1";				// 有効
		public const string FLG_CSOPERATORAUTHORITY_PERMIT_UNLOCK_FLG_INVALID = "0";			// 無効

		// CSオペレータ権限マスタ：ロック解除許可フラグ
		public const string FLG_CSOPERATORAUTHORITY_PERMIT_PERMANENT_DELETE_FLG_VALID = "1";				// 有効
		public const string FLG_CSOPERATORAUTHORITY_PERMIT_PERMANENT_DELETE_FLG_INVALID = "0";			// 無効

		// CSオペレータ権限マスタ：共通署名編集許可フラグ
		public const string FLG_CSOPERATORAUTHORITY_PERMIT_EDIT_SIGNATURE_FLG_VALID = "1";		// 有効
		public const string FLG_CSOPERATORAUTHORITY_PERMIT_EDIT_SIGNATURE_FLG_INVALID = "0";	// 無効

		// CSオペレータ権限マスタ：担当未設定警告メール受け取りフラグ
		public const string FLG_CSOPERATORAUTHORITY_RECEIVE_NO_ASSIGN_WARNING_FLG_VALID = "1";	// 有効
		public const string FLG_CSOPERATORAUTHORITY_RECEIVE_NO_ASSIGN_WARNING_FLG_INVALID = "0";// 無効

		// CSグループマスタ：有効フラグ
		public const string FLG_CSGROUP_VALID_FLG_VALID = "1";		// 有効
		public const string FLG_CSGROUP_VALID_FLG_INVALID = "0";	// 無効

		// メール送信ログ：既読フラグ
		public const string FLG_MAILSENDLOG_READ_FLG_READ = "1";	// 既読
		public const string FLG_MAILSENDLOG_READ_FLG_UNREAD = "0";	// 未読

		// メール送信ログ：メールアドレス区分
		public const string FLG_MAILSENDLOG_MAIL_ADDR_KBN_PC = "PC";
		public const string FLG_MAILSENDLOG_MAIL_ADDR_KBN_MOBILE = "MB";

		// Top page search flag
		public const string FLG_SEARCH_ON = "1";	// 有効
		#endregion
	}
}
