/*
=========================================================================================================
  Module      : DB定数定義Commerce部分(Constants_Commerce.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
namespace w2.Database.Common
{
	///*********************************************************************************************
	/// <summary>
	/// DB定数定義Commerce部分
	/// </summary>
	///*********************************************************************************************
	[System.Runtime.InteropServices.GuidAttribute("0FBD1D15-7D3D-495B-AAAA-523D7D8F36B0")]
	public partial class Constants : w2.Common.Constants
	{
		#region テーブル・フィールド定数

		// 店舗マスタ
		public const string TABLE_SHOP = "w2_Shop";                                                 // 店舗マスタ
		public const string FIELD_SHOP_SHOP_ID = "shop_id";                                         // 店舗ID
		public const string FIELD_SHOP_CORPORATION_ID = "corporation_id";                           // 企業ID
		public const string FIELD_SHOP_NAME = "name";                                               // 店舗名
		public const string FIELD_SHOP_NAME_KANA = "name_kana";                                     // 店舗名かな
		public const string FIELD_SHOP_DESC_SHORT = "desc_short";                                   // 説明(小)
		public const string FIELD_SHOP_DESC_MEDIUM = "desc_medium";                                 // 説明(中)
		public const string FIELD_SHOP_DESC_LONG = "desc_long";                                     // 説明(大)
		public const string FIELD_SHOP_ZIP = "zip";                                                 // 郵便番号
		public const string FIELD_SHOP_ZIP1 = "zip1";                                               // 郵便番号1
		public const string FIELD_SHOP_ZIP2 = "zip2";                                               // 郵便番号2
		public const string FIELD_SHOP_ADDR = "addr";                                               // 住所
		public const string FIELD_SHOP_ADDR1 = "addr1";                                             // 住所1
		public const string FIELD_SHOP_ADDR2 = "addr2";                                             // 住所2
		public const string FIELD_SHOP_ADDR3 = "addr3";                                             // 住所3
		public const string FIELD_SHOP_ADDR4 = "addr4";                                             // 住所4
		public const string FIELD_SHOP_TEL = "tel";                                                 // 電話番号
		public const string FIELD_SHOP_TEL_1 = "tel_1";                                             // 電話番号1
		public const string FIELD_SHOP_TEL_2 = "tel_2";                                             // 電話番号2
		public const string FIELD_SHOP_TEL_3 = "tel_3";                                             // 電話番号3
		public const string FIELD_SHOP_FAX = "fax";                                                 // ＦＡＸ
		public const string FIELD_SHOP_FAX_1 = "fax_1";                                             // ＦＡＸ1
		public const string FIELD_SHOP_FAX_2 = "fax_2";                                             // ＦＡＸ2
		public const string FIELD_SHOP_FAX_3 = "fax_3";                                             // ＦＡＸ3
		public const string FIELD_SHOP_URL = "url";                                                 // URL
		public const string FIELD_SHOP_MAIL_ADDR = "mail_addr";                                     // 管理者メールアドレス
		public const string FIELD_SHOP_MAIL_ADDR_BATCH = "mail_addr_batch";                         // バッチ実行結果送信先メールアドレス
		public const string FIELD_SHOP_DEL_FLG = "del_flg";                                         // 削除フラグ
		public const string FIELD_SHOP_DATE_CREATED = "date_created";                               // 作成日
		public const string FIELD_SHOP_DATE_CHANGED = "date_changed";                               // 更新日

		// メニュー権限管理マスタ
		public const string TABLE_MENUAUTHORITY = "w2_MenuAuthority";                               // メニュー権限管理マスタ
		public const string FIELD_MENUAUTHORITY_SHOP_ID = "shop_id";                                // 店舗ID
		public const string FIELD_MENUAUTHORITY_PKG_KBN = "pkg_kbn";                                // パッケージ区分
		public const string FIELD_MENUAUTHORITY_MENU_AUTHORITY_LEVEL = "menu_authority_level";      // 表示レベル
		public const string FIELD_MENUAUTHORITY_MENU_AUTHORITY_NAME = "menu_authority_name";        // メニュー権限名
		public const string FIELD_MENUAUTHORITY_MENU_PATH = "menu_path";                            // メニューパス
		public const string FIELD_MENUAUTHORITY_FUNCTION_LEVEL = "function_level";                  // 機能レベル
		public const string FIELD_MENUAUTHORITY_DEFAULT_DISP_FLG = "default_disp_flg";              // デフォルト表示フラグ
		public const string FIELD_MENUAUTHORITY_DEL_FLG = "del_flg";                                // 削除フラグ
		public const string FIELD_MENUAUTHORITY_DATE_CREATED = "date_created";                      // 作成日
		public const string FIELD_MENUAUTHORITY_DATE_CHANGED = "date_changed";                      // 更新日
		public const string FIELD_MENUAUTHORITY_LAST_CHANGED = "last_changed";                      // 最終更新者

		// 店舗管理者マスタ
		public const string TABLE_SHOPOPERATOR = "w2_ShopOperator";                                 // 店舗管理者マスタ
		public const string FIELD_SHOPOPERATOR_SHOP_ID = "shop_id";                                 // 店舗ID
		public const string FIELD_SHOPOPERATOR_OPERATOR_ID = "operator_id";                         // オペレータID
		public const string FIELD_SHOPOPERATOR_NAME = "name";                                       // オペレータ名
		public const string FIELD_SHOPOPERATOR_MAIL_ADDR = "mail_addr";                             // メールアドレス
		public const string FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL1 = "menu_access_level1";           // メニューアクセスレベル1
		public const string FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL2 = "menu_access_level2";           // メニューアクセスレベル2
		public const string FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL3 = "menu_access_level3";           // メニューアクセスレベル3
		public const string FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL4 = "menu_access_level4";           // メニューアクセスレベル4
		public const string FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL5 = "menu_access_level5";           // メニューアクセスレベル5
		public const string FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL6 = "menu_access_level6";           // メニューアクセスレベル6
		public const string FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL7 = "menu_access_level7";           // メニューアクセスレベル7
		public const string FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL8 = "menu_access_level8";           // メニューアクセスレベル8
		public const string FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL9 = "menu_access_level9";           // メニューアクセスレベル9
		public const string FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL10 = "menu_access_level10";         // メニューアクセスレベル10
		public const string FIELD_SHOPOPERATOR_LOGIN_ID = "login_id";                               // ログインID
		public const string FIELD_SHOPOPERATOR_PASSWORD = "password";                               // パスワード
		public const string FIELD_SHOPOPERATOR_ODBC_USER_NAME = "odbc_user_name";                   // ODBCユーザ名
		public const string FIELD_SHOPOPERATOR_ODBC_PASSWORD = "odbc_password";                     // ODBCパスワード
		public const string FIELD_SHOPOPERATOR_VALID_FLG = "valid_flg";                             // 有効フラグ
		public const string FIELD_SHOPOPERATOR_DEL_FLG = "del_flg";                                 // 削除フラグ
		public const string FIELD_SHOPOPERATOR_DATE_CREATED = "date_created";                       // 作成日
		public const string FIELD_SHOPOPERATOR_DATE_CHANGED = "date_changed";                       // 更新日
		public const string FIELD_SHOPOPERATOR_LAST_CHANGED = "last_changed";                       // 最終更新者
		/// <summary>閲覧可能な広告コード</summary>
		public const string FIELD_SHOPOPERATOR_USABLE_ADVCODE_NOS_IN_REPORT = "usable_advcode_nos_in_report";   // 閲覧可能な広告コード
		/// <summary>閲覧可能なタグID</summary>
		public const string FIELD_SHOPOPERATOR_USABLE_AFFILIATE_TAG_IDS_IN_REPORT = "usable_affiliate_tag_ids_in_report";   // 閲覧可能なタグID
		/// <summary>閲覧可能な広告媒体区分ID</summary>
		public const string FIELD_SHOPOPERATOR_USABLE_ADVCODE_MEDIA_TYPE_IDS = "usable_advcode_media_type_ids";
		/// <summary>閲覧可能な出力箇所</summary>
		public const string FIELD_SHOPOPERATOR_USABLE_OUTPUT_LOCATIONS = "usable_output_locations";
		/// <summary>最終ログイン日時</summary>
		public const string FIELD_SHOPOPERATOR_DATE_LAST_LOGGEDIN = "date_last_loggedin";
		/// <summary>リモートIPアドレス</summary>
		public const string FIELD_SHOPOPERATOR_REMOTE_ADDR = "remote_addr";
		/// <summary>認証コード</summary>
		public const string FIELD_SHOPOPERATOR_AUTHENTICATION_CODE = "authentication_code";
		/// <summary>認証コード送信日時</summary>
		public const string FIELD_SHOPOPERATOR_DATE_CODE_SEND = "date_code_send";

		// メールテンプレートマスタ
		public const string TABLE_MAILTEMPLATE = "w2_MailTemplate";                                 // メールテンプレートマスタ
		public const string FIELD_MAILTEMPLATE_SHOP_ID = "shop_id";                                 // 店舗ID
		public const string FIELD_MAILTEMPLATE_MAIL_ID = "mail_id";                                 // メールテンプレートID
		public const string FIELD_MAILTEMPLATE_MAIL_NAME = "mail_name";                             // メールテンプレート名
		public const string FIELD_MAILTEMPLATE_MAIL_FROM = "mail_from";                             // メールFrom
		public const string FIELD_MAILTEMPLATE_MAIL_TO = "mail_to";                                 // メールTo
		public const string FIELD_MAILTEMPLATE_MAIL_CC = "mail_cc";                                 // メールCC
		public const string FIELD_MAILTEMPLATE_MAIL_BCC = "mail_bcc";                               // メールBCC
		public const string FIELD_MAILTEMPLATE_MAIL_SUBJECT = "mail_subject";                       // メールタイトル
		public const string FIELD_MAILTEMPLATE_MAIL_BODY = "mail_body";                             // メール本文
		public const string FIELD_MAILTEMPLATE_MAIL_SUBJECT_MOBILE = "mail_subject_mobile";         // メールタイトル（モバイル用）
		public const string FIELD_MAILTEMPLATE_MAIL_BODY_MOBILE = "mail_body_mobile";               // メール本文（モバイル用）
		public const string FIELD_MAILTEMPLATE_MAIL_ATTACHMENTFILE_PATH = "mail_attachmentfile_path";// メール添付ファイルパス
		public const string FIELD_MAILTEMPLATE_DEL_FLG = "del_flg";                                 // 削除フラグ
		public const string FIELD_MAILTEMPLATE_DATE_CREATED = "date_created";                       // 作成日
		public const string FIELD_MAILTEMPLATE_DATE_CHANGED = "date_changed";                       // 更新日
		public const string FIELD_MAILTEMPLATE_LAST_CHANGED = "last_changed";                       // 最終更新者
		public const string FIELD_MAILTEMPLATE_MAIL_FROM_NAME = "mail_from_name";                   // メールFROM名
		public const string FIELD_MAILTEMPLATE_MAIL_CATEGORY = "mail_category";                     // メールカテゴリ
		public const string FIELD_MAILTEMPLATE_AUTO_SEND_FLG = "auto_send_flg";                     // 自動送信フラグ
		public const string FIELD_MAILTEMPLATE_LANGUAGE_CODE = "language_code";                     // 言語コード
		public const string FIELD_MAILTEMPLATE_LANGUAGE_LOCALE_ID = "language_locale_id";           // 言語ロケールID
		public const string FIELD_MAILTEMPLATE_SMS_USE_FLG = "sms_use_flg";                         // SMS利用フラグ
		public const string FIELD_MAILTEMPLATE_LINE_USE_FLG = "line_use_flg";                       // LINE利用フラグ
		/// <summary>メール文章HTML</summary>
		public const string FIELD_MAILTEMPLATE_MAIL_TEXT_HTML = "mailtext_html";
		/// <summary>HTMLメール送信フラグ</summary>
		public const string FIELD_MAILTEMPLATE_SEND_HTML_FLG = "sendhtml_flg";

		// 決済種別マスタ
		public const string TABLE_PAYMENT = "w2_Payment";                                           // 決済種別マスタ
		public const string FIELD_PAYMENT_SHOP_ID = "shop_id";                                      // 店舗ID
		public const string FIELD_PAYMENT_PAYMENT_GROUP_ID = "payment_group_id";                    // 決済種別グループID
		public const string FIELD_PAYMENT_PAYMENT_ID = "payment_id";                                // 決済種別ID
		public const string FIELD_PAYMENT_PAYMENT_ALT_ID = "payment_alt_id";                        // 連携用決済ID
		public const string FIELD_PAYMENT_PAYMENT_NAME = "payment_name";                            // 決済種別名
		public const string FIELD_PAYMENT_PAYMENT_NAME_MOBILE = "payment_name_mobile";              // モバイル用決済種別名
		public const string FIELD_PAYMENT_PAYMENT_PRICE_KBN = "payment_price_kbn";                  // 決済手数料区分
		public const string FIELD_PAYMENT_PAYMENT_PRICE = "payment_price";                          // 決済手数料
		public const string FIELD_PAYMENT_USABLE_PRICE_MIN = "usable_price_min";                    // 利用可能金額(下限)
		public const string FIELD_PAYMENT_USABLE_PRICE_MAX = "usable_price_max";                    // 利用可能金額(上限)
		public const string FIELD_PAYMENT_MOBILE_DISP_FLG = "mobile_disp_flg";                      // モバイル表示フラグ
		public const string FIELD_PAYMENT_DISPLAY_ORDER = "display_order";                          // 表示順
		public const string FIELD_PAYMENT_VALID_FLG = "valid_flg";                                  // 有効フラグ
		public const string FIELD_PAYMENT_DEL_FLG = "del_flg";                                      // 削除フラグ
		public const string FIELD_PAYMENT_DATE_CREATED = "date_created";                            // 作成日
		public const string FIELD_PAYMENT_DATE_CHANGED = "date_changed";                            // 更新日
		public const string FIELD_PAYMENT_LAST_CHANGED = "last_changed";                            // 最終更新者
		public const string FIELD_PAYMENT_USER_MANAGEMENT_LEVEL_NOT_USE = "user_management_level_not_use";	// List of user management level not use payment
		public const string FIELD_PAYMENT_ORDER_OWNER_KBN_NOT_USE = "order_owner_kbn_not_use";      // 利用不可な注文者区分リスト

		// 決済手数料マスタ
		public const string TABLE_PAYMENTPRICE = "w2_PaymentPrice";                                 // 決済手数料マスタ
		public const string FIELD_PAYMENTPRICE_SHOP_ID = "shop_id";                                 // 店舗ID
		public const string FIELD_PAYMENTPRICE_PAYMENT_GROUP_ID = "payment_group_id";               // 決済種別グループID
		public const string FIELD_PAYMENTPRICE_PAYMENT_ID = "payment_id";                           // 決済種別ID
		public const string FIELD_PAYMENTPRICE_PAYMENT_PRICE_NO = "payment_price_no";               // 枝番
		public const string FIELD_PAYMENTPRICE_TGT_PRICE_BGN = "tgt_price_bgn";                     // 対象購入金額（以上）
		public const string FIELD_PAYMENTPRICE_TGT_PRICE_END = "tgt_price_end";                     // 対象購入金額（以下）
		public const string FIELD_PAYMENTPRICE_PAYMENT_PRICE = "payment_price";                     // 決済手数料
		public const string FIELD_PAYMENTPRICE_DEL_FLG = "del_flg";                                 // 削除フラグ
		public const string FIELD_PAYMENTPRICE_DATE_CREATED = "date_created";                       // 作成日
		public const string FIELD_PAYMENTPRICE_DATE_CHANGED = "date_changed";                       // 更新日
		public const string FIELD_PAYMENTPRICE_LAST_CHANGED = "last_changed";                       // 最終更新者

		// 店舗配送種別マスタ
		public const string TABLE_SHOPSHIPPING = "w2_ShopShipping";                                 // 店舗配送種別マスタ
		public const string FIELD_SHOPSHIPPING_SHOP_ID = "shop_id";                                 // 店舗ID
		public const string FIELD_SHOPSHIPPING_SHIPPING_ID = "shipping_id";                         // 配送種別ID
		/// <summary>配送拠点ID</summary>
		public const string FIELD_SHOPSHIPPING_SHIPPING_BASE_ID = "shipping_base_id";
		public const string FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME = "shop_shipping_name";           // 配送種別名
		public const string FIELD_SHOPSHIPPING_PAYMENT_RELATION_ID = "payment_relation_id";         // 決済連携ID
		public const string FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG = "shipping_date_set_flg";     // 配送日設定可能フラグ
		public const string FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_BEGIN = "shipping_date_set_begin"; // 配送日設定可能範囲(開始)
		public const string FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_END = "shipping_date_set_end";     // 配送日設定可能範囲（終了）
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_SET_FLG = "shipping_time_set_flg";     // 配送希望時間帯設定可能フラグ
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_ID1 = "shipping_time_id1";             // 配送希望時間帯ID1
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_MESSAGE1 = "shipping_time_message1";   // 配送希望時間帯文言1
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_ID2 = "shipping_time_id2";             // 配送希望時間帯ID2
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_MESSAGE2 = "shipping_time_message2";   // 配送希望時間帯文言2
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_ID3 = "shipping_time_id3";             // 配送希望時間帯ID3
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_MESSAGE3 = "shipping_time_message3";   // 配送希望時間帯文言3
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_ID4 = "shipping_time_id4";             // 配送希望時間帯ID4
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_MESSAGE4 = "shipping_time_message4";   // 配送希望時間帯文言4
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_ID5 = "shipping_time_id5";             // 配送希望時間帯ID5
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_MESSAGE5 = "shipping_time_message5";   // 配送希望時間帯文言5
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_ID6 = "shipping_time_id6";             // 配送希望時間帯ID6
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_MESSAGE6 = "shipping_time_message6";   // 配送希望時間帯文言6
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_ID7 = "shipping_time_id7";             // 配送希望時間帯ID7
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_MESSAGE7 = "shipping_time_message7";   // 配送希望時間帯文言7
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_ID8 = "shipping_time_id8";             // 配送希望時間帯ID8
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_MESSAGE8 = "shipping_time_message8";   // 配送希望時間帯文言8
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_ID9 = "shipping_time_id9";             // 配送希望時間帯ID9
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_MESSAGE9 = "shipping_time_message9";   // 配送希望時間帯文言9
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_ID10 = "shipping_time_id10";           // 配送希望時間帯ID10
		//public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_MESSAGE10 = "shipping_time_message10"; // 配送希望時間帯文言10
		public const string FIELD_SHOPSHIPPING_DEL_FLG = "del_flg";                                 // 削除フラグ
		public const string FIELD_SHOPSHIPPING_DATE_CREATED = "date_created";                       // 作成日
		public const string FIELD_SHOPSHIPPING_DATE_CHANGED = "date_changed";                       // 更新日
		public const string FIELD_SHOPSHIPPING_LAST_CHANGED = "last_changed";                       // 最終更新者
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG = "fixed_purchase_flg";                   // 定期購入可能フラグ
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG = "fixed_purchase_kbn1_flg";         // 定期購入区分1設定フラグ
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING = "fixed_purchase_kbn1_setting"; // 定期購入区分1設定値
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG = "fixed_purchase_kbn2_flg";         // 定期購入区分2設定フラグ
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN2_SETTING = "fixed_purchase_kbn2_setting"; // 定期購入区分2設定値
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_FLG = "fixed_purchase_kbn3_flg";         // 定期購入区分3設定フラグ
		/// <summary>定期購入区分4設定フラグ</summary>
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG = "fixed_purchase_kbn4_flg";
		/// <summary>定期購入区分4設定値1</summary>
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING1 = "fixed_purchase_kbn4_setting1";
		/// <summary>定期購入区分4設定値2</summary>
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING2 = "fixed_purchase_kbn4_setting2";
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_NOTDISPLAY_FLG = "fixed_purchase_shipping_notdisplay_flg";	// フロント側の配送パターン非表示フラグ
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_SETTING = "fixed_purchase_kbn3_setting"; // 定期購入区分3設定値
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_ORDER_ENTRY_TIMING = "fixed_purchase_order_entry_timing";// 定期購入受注タイミング
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_CANCEL_DEADLINE = "fixed_purchase_cancel_deadline";// 定期購入キャンセル期限
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_DAYS_REQUIRED = "fixed_purchase_shipping_days_required";	// 定期購入配送所要日数
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_MINIMUM_SHIPPING_SPAN = "fixed_purchase_minimum_shipping_span";	// 定期購入最低配送間隔
		public const string FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG = "wrapping_paper_flg";         // のし利用フラグ
		public const string FIELD_SHOPSHIPPING_WRAPPING_PAPER_TYPES = "wrapping_paper_types";     // のし種類
		public const string FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG = "wrapping_bag_flg";               // 包装利用フラグ
		public const string FIELD_SHOPSHIPPING_WRAPPING_BAG_TYPES = "wrapping_bag_types";           // 包装種類
		public const string FIELD_SHOPSHIPPING_PAYMENT_SELECTION_FLG = "payment_selection_flg"; // 決済選択の任意利用フラグ
		public const string FIELD_SHOPSHIPPING_PERMITTED_PAYMENT_IDS = "permitted_payment_ids"; // 決済選択の可能リスト
		public const string FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG = "shipping_price_separate_estimates_flg";// 配送料の別途見積もりの利用フラグ
		public const string FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_MESSAGE = "shipping_price_separate_estimates_message";// 配送料の別途見積もりの文言
		public const string FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_MESSAGE_MOBILE = "shipping_price_separate_estimates_message_mobile";// 配送料の別途見積もりの文言モバイル
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG = "fixed_purchase_free_shipping_flg";// 定期配送料無料フラグ
		public const string FIELD_SHOPSHIPPING_BUSINESS_DAYS_FOR_SHIPPING = "business_days_for_shipping"; // 出荷所要営業日数
		public const string FIELD_SHOPSHIPPING_NEXT_SHIPPING_MAX_CHANGE_DAYS = "next_shipping_max_change_days"; // 次回配送日変更可能日数
		/// <summary>定期購入区分1設定値2</summary>///
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING2 = "fixed_purchase_kbn1_setting2";
		/// <summary>定期購入初回配送翌月フラグ</summary>
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_FIRST_SHIPPING_NEXT_MONTH_FLG = "fixed_purchase_first_shipping_next_month_flg";

		// 配送料マスタ
		public const string TABLE_SHIPPINGDELIVERYPOSTAGE = "w2_ShippingDeliveryPostage";           // 配送料マスタ
		public const string FIELD_SHIPPINGDELIVERYPOSTAGE_SHOP_ID = "shop_id";                      // 店舗ID
		public const string FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_ID = "shipping_id";              // 配送種別ID
		public const string FIELD_SHIPPINGDELIVERYPOSTAGE_DELIVERY_COMPANY_ID = "delivery_company_id";// 配送会社ID
		public const string FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_PRICE_KBN = "shipping_price_kbn";// 配送料設定区分
		public const string FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_FREE_PRICE_FLG = "shipping_free_price_flg";// 配送料無料購入金額設定フラグ
		public const string FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_FREE_PRICE = "shipping_free_price";// 配送料無料購入金額設定
		public const string FIELD_SHIPPINGDELIVERYPOSTAGE_ANNOUNCE_FREE_SHIPPING_FLG = "announce_free_shipping_flg";// 配送料無料案内表示フラグ
		public const string FIELD_SHIPPINGDELIVERYPOSTAGE_CALCULATION_PLURAL_KBN = "calculation_plural_kbn";// 複数商品計算区分
		public const string FIELD_SHIPPINGDELIVERYPOSTAGE_PLURAL_SHIPPING_PRICE = "plural_shipping_price";// 複数商品配送料
		public const string FIELD_SHIPPINGDELIVERYPOSTAGE_DATE_CREATED = "date_created";            // 作成日
		public const string FIELD_SHIPPINGDELIVERYPOSTAGE_DATE_CHANGED = "date_changed";            // 更新日
		public const string FIELD_SHIPPINGDELIVERYPOSTAGE_LAST_CHANGED = "last_changed";            // 最終更新者
		/// <summary>店舗受取時配送料無料フラグ</summary>
		public const string FIELD_SHIPPINGDELIVERYPOSTAGE_STOREPICKUP_FREE_PRICE_FLG = "storepickup_free_price_flg";
		/// <summary>配送料無料時の請求料金</summary>
		public const string FIELD_SHIPPINGDELIVERYPOSTAGE_FREE_SHIPPING_FEE = "free_shipping_fee";

		// 店舗配送料地帯マスタ
		public const string TABLE_SHOPSHIPPINGZONE = "w2_ShopShippingZone";                         // 店舗配送料地帯マスタ
		public const string FIELD_SHOPSHIPPINGZONE_SHOP_ID = "shop_id";                             // 店舗ID
		public const string FIELD_SHOPSHIPPINGZONE_SHIPPING_ID = "shipping_id";                     // 配送料設定ID
		public const string FIELD_SHOPSHIPPINGZONE_DELIVERY_COMPANY_ID = "delivery_company_id";     // 配送会社ID
		public const string FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO = "shipping_zone_no";           // 配送料地帯区分
		public const string FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NAME = "shipping_zone_name";       // 地帯名
		public const string FIELD_SHOPSHIPPINGZONE_ZIP = "zip";                                     // 郵便番号
		public const string FIELD_SHOPSHIPPINGZONE_SIZE_XXS_SHIPPING_PRICE = "size_xxs_shipping_price";// XXSサイズ商品配送料
		public const string FIELD_SHOPSHIPPINGZONE_SIZE_XS_SHIPPING_PRICE = "size_xs_shipping_price";// XSサイズ商品配送料
		public const string FIELD_SHOPSHIPPINGZONE_SIZE_S_SHIPPING_PRICE = "size_s_shipping_price"; // Sサイズ商品配送料
		public const string FIELD_SHOPSHIPPINGZONE_SIZE_M_SHIPPING_PRICE = "size_m_shipping_price"; // Mサイズ商品配送料
		public const string FIELD_SHOPSHIPPINGZONE_SIZE_L_SHIPPING_PRICE = "size_l_shipping_price"; // Lサイズ商品配送料
		public const string FIELD_SHOPSHIPPINGZONE_SIZE_XL_SHIPPING_PRICE = "size_xl_shipping_price";// XLサイズ商品配送料
		public const string FIELD_SHOPSHIPPINGZONE_SIZE_XXL_SHIPPING_PRICE = "size_xxl_shipping_price";// XXLサイズ商品配送料
		public const string FIELD_SHOPSHIPPINGZONE_DEL_FLG = "del_flg";                             // 削除フラグ
		public const string FIELD_SHOPSHIPPINGZONE_DATE_CREATED = "date_created";                   // 作成日
		public const string FIELD_SHOPSHIPPINGZONE_DATE_CHANGED = "date_changed";                   // 更新日
		public const string FIELD_SHOPSHIPPINGZONE_LAST_CHANGED = "last_changed";                   // 最終更新者
		public const string FIELD_SHOPSHIPPINGZONE_SIZE_MAIL_SHIPPING_PRICE = "size_mail_shipping_price";// メールサイズ商品配送料
		/// <summary>条件付き配送料閾値</summary>
		public const string FIELD_SHOPSHIPPINGZONE_CONDITIONAL_SHIPPING_PRICE_THRESHOLD = "conditional_shipping_price_threshold";
		/// <summary>条件付き配送料</summary>
		public const string FIELD_SHOPSHIPPINGZONE_CONDITIONAL_SHIPPING_PRICE = "conditional_shipping_price";
		/// <summary>配送不可エリアフラグ</summary>
		public const string FIELD_SHOPSHIPPINGZONE_UNAVAILABLE_SHIPPING_AREA_FLG = "unavailable_shipping_area_flg";

		// 配送会社マスタ
		public const string TABLE_DELIVERYCOMPANY = "w2_DeliveryCompany";                           // 配送会社マスタ
		public const string FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_ID = "delivery_company_id";      // 配送会社ID
		public const string FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_NAME = "delivery_company_name";  // 配送会社名
		public const string FIELD_DELIVERYCOMPANY_DELIVERY_SIZE_LIMIT = "delivery_company_mail_size_limit";                      // 配送サイズ上限
		/// <summary>配送希望時間帯文言</summary>
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE = "shipping_time_message";
		/// <summary>出荷連携配送会社(クレジットカード)</summary>
		public const string FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_CREDITCARD = "delivery_company_type_creditcard";
		/// <summary>出荷連携配送会社(Gooddeal)</summary>
		public const string FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_GOODDEAL = "delivery_company_type_gooddeal";
		/// <summary>出荷連携配送会社(後払い)</summary>
		public const string FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_POST_PAYMENT = "delivery_company_type_post_payment";
		public const string FIELD_DELIVERYCOMPANY_DISPLAY_ORDER = "display_order";                  // 表示順
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG = "shipping_time_set_flg";  // 配送希望時間帯設定可能フラグ
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID1 = "shipping_time_id1";          // 配送希望時間帯ID1
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE1 = "shipping_time_message1";// 配送希望時間帯文言1
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID2 = "shipping_time_id2";          // 配送希望時間帯ID2
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE2 = "shipping_time_message2";// 配送希望時間帯文言2
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID3 = "shipping_time_id3";          // 配送希望時間帯ID3
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE3 = "shipping_time_message3";// 配送希望時間帯文言3
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID4 = "shipping_time_id4";          // 配送希望時間帯ID4
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE4 = "shipping_time_message4";// 配送希望時間帯文言4
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID5 = "shipping_time_id5";          // 配送希望時間帯ID5
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE5 = "shipping_time_message5";// 配送希望時間帯文言5
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID6 = "shipping_time_id6";          // 配送希望時間帯ID6
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE6 = "shipping_time_message6";// 配送希望時間帯文言6
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID7 = "shipping_time_id7";          // 配送希望時間帯ID7
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE7 = "shipping_time_message7";// 配送希望時間帯文言7
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID8 = "shipping_time_id8";          // 配送希望時間帯ID8
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE8 = "shipping_time_message8";// 配送希望時間帯文言8
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID9 = "shipping_time_id9";          // 配送希望時間帯ID9
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE9 = "shipping_time_message9";// 配送希望時間帯文言9
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID10 = "shipping_time_id10";        // 配送希望時間帯ID10
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE10 = "shipping_time_message10";// 配送希望時間帯文言10
		public const string FIELD_DELIVERYCOMPANY_DATE_CREATED = "date_created";                    // 作成日
		public const string FIELD_DELIVERYCOMPANY_DATE_CHANGED = "date_changed";                    // 更新日
		public const string FIELD_DELIVERYCOMPANY_LAST_CHANGED = "last_changed";                    // 最終更新者
		public const string FIELD_DELIVERYCOMPANY_DELIVERY_LEAD_TIME_SET_FLG = "delivery_lead_time_set_flg";	// リードタイム設定可能フラグ
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_LEAD_TIME_DEFAULT = "shipping_lead_time_default";	// 基本配送リードタイム
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING1 = "shipping_time_matching1";// 配送希望時間帯マッチング文字列1
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING2 = "shipping_time_matching2";// 配送希望時間帯マッチング文字列2
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING3 = "shipping_time_matching3";// 配送希望時間帯マッチング文字列3
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING4 = "shipping_time_matching4";// 配送希望時間帯マッチング文字列4
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING5 = "shipping_time_matching5";// 配送希望時間帯マッチング文字列5
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING6 = "shipping_time_matching6";// 配送希望時間帯マッチング文字列6
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING7 = "shipping_time_matching7";// 配送希望時間帯マッチング文字列7
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING8 = "shipping_time_matching8";// 配送希望時間帯マッチング文字列8
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING9 = "shipping_time_matching9";// 配送希望時間帯マッチング文字列9
		public const string FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING10 = "shipping_time_matching10";// 配送希望時間帯マッチング文字列10
		public const string FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_POST_NP_PAYMENT = "delivery_company_type_post_np_payment";	// 出荷連携配送会社(NP後払い)
		/// <summary>当日出荷締め時間</summary>
		public const string FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_DEADLINE_TIME = "deadline_time";
		/// <summary>出荷連携配送会社(GMOアトカラ)</summary>
		public const string FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_GMO_ATOKARA_PAYMENT = "delivery_company_type_gmo_atokara_payment";

		// 配送種別配送会社マスタ
		public const string TABLE_SHOPSHIPPINGCOMPANY = "w2_ShopShippingCompany";                   // 配送種別配送会社マスタ
		public const string FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_ID = "shipping_id";                  // 配送種別ID
		public const string FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_KBN = "shipping_kbn";                // 配送区分
		public const string FIELD_SHOPSHIPPINGCOMPANY_DELIVERY_COMPANY_ID = "delivery_company_id";  // 配送会社ID
		public const string FIELD_SHOPSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY = "default_delivery_company";// 初期配送会社
		public const string FIELD_SHOPSHIPPINGCOMPANY_DATE_CREATED = "date_created";                // 作成日
		public const string FIELD_SHOPSHIPPINGCOMPANY_DATE_CHANGED = "date_changed";                // 更新日
		public const string FIELD_SHOPSHIPPINGCOMPANY_LAST_CHANGED = "last_changed";                // 最終更新者

		// テンポラリデータテーブル
		public const string TABLE_TEMPDATAS = "w2_TempDatas";                                       // テンポラリデータテーブル
		public const string FIELD_TEMPDATAS_TEMP_TYPE = "temp_type";                                // テンポラリタイプ
		public const string FIELD_TEMPDATAS_TEMP_KEY = "temp_key";                                  // テンポラリキー
		public const string FIELD_TEMPDATAS_TEMP_DATA = "temp_data";                                // テンポラリデータ
		public const string FIELD_TEMPDATAS_DATE_CREATED = "date_created";                          // 作成日

		// ユーザマスタ
		public const string TABLE_USER = "w2_User";                                                 // ユーザマスタ
		public const string FIELD_USER_USER_ID = "user_id";                                         // ユーザID
		public const string FIELD_USER_USER_KBN = "user_kbn";                                       // 顧客区分
		public const string FIELD_USER_MALL_ID = "mall_id";                                         // モールID
		public const string FIELD_USER_NAME = "name";                                               // 氏名
		public const string FIELD_USER_NAME1 = "name1";                                             // 氏名1
		public const string FIELD_USER_NAME2 = "name2";                                             // 氏名2
		public const string FIELD_USER_NAME_KANA = "name_kana";                                     // 氏名かな
		public const string FIELD_USER_NAME_KANA1 = "name_kana1";                                   // 氏名かな1
		public const string FIELD_USER_NAME_KANA2 = "name_kana2";                                   // 氏名かな2
		public const string FIELD_USER_NICK_NAME = "nick_name";                                     // ニックネーム
		public const string FIELD_USER_MAIL_ADDR = "mail_addr";                                     // メールアドレス
		public const string FIELD_USER_MAIL_ADDR2 = "mail_addr2";                                   // メールアドレス2
		public const string FIELD_USER_ZIP = "zip";                                                 // 郵便番号
		public const string FIELD_USER_ZIP1 = "zip1";                                               // 郵便番号1
		public const string FIELD_USER_ZIP2 = "zip2";                                               // 郵便番号2
		public const string FIELD_USER_ADDR = "addr";                                               // 住所
		public const string FIELD_USER_ADDR1 = "addr1";                                             // 住所1
		public const string FIELD_USER_ADDR2 = "addr2";                                             // 住所2
		public const string FIELD_USER_ADDR3 = "addr3";                                             // 住所3
		public const string FIELD_USER_ADDR4 = "addr4";                                             // 住所4
		public const string FIELD_USER_TEL1 = "tel1";                                               // 電話番号1
		public const string FIELD_USER_TEL1_1 = "tel1_1";                                           // 電話番号1-1
		public const string FIELD_USER_TEL1_2 = "tel1_2";                                           // 電話番号1-2
		public const string FIELD_USER_TEL1_3 = "tel1_3";                                           // 電話番号1-3
		public const string FIELD_USER_TEL2 = "tel2";                                               // 電話番号2
		public const string FIELD_USER_TEL2_1 = "tel2_1";                                           // 電話番号2-1
		public const string FIELD_USER_TEL2_2 = "tel2_2";                                           // 電話番号2-2
		public const string FIELD_USER_TEL2_3 = "tel2_3";                                           // 電話番号2-3
		public const string FIELD_USER_TEL3 = "tel3";                                               // 電話番号3
		public const string FIELD_USER_TEL3_1 = "tel3_1";                                           // 電話番号3-1
		public const string FIELD_USER_TEL3_2 = "tel3_2";                                           // 電話番号3-2
		public const string FIELD_USER_TEL3_3 = "tel3_3";                                           // 電話番号3-3
		public const string FIELD_USER_FAX = "fax";                                                 // ＦＡＸ
		public const string FIELD_USER_FAX_1 = "fax_1";                                             // ＦＡＸ1
		public const string FIELD_USER_FAX_2 = "fax_2";                                             // ＦＡＸ2
		public const string FIELD_USER_FAX_3 = "fax_3";                                             // ＦＡＸ3
		public const string FIELD_USER_SEX = "sex";                                                 // 性別
		public const string FIELD_USER_BIRTH = "birth";                                             // 生年月日
		public const string FIELD_USER_BIRTH_YEAR = "birth_year";                                   // 生年月日（年）
		public const string FIELD_USER_BIRTH_MONTH = "birth_month";                                 // 生年月日（月）
		public const string FIELD_USER_BIRTH_DAY = "birth_day";                                     // 生年月日（日）
		public const string FIELD_USER_COMPANY_NAME = "company_name";                               // 企業名
		public const string FIELD_USER_COMPANY_POST_NAME = "company_post_name";                     // 部署名
		public const string FIELD_USER_COMPANY_EXECTIVE_NAME = "company_exective_name";             // 役職名
		public const string FIELD_USER_ADVCODE_FIRST = "advcode_first";                             // 初回広告コード
		public const string FIELD_USER_ATTRIBUTE1 = "attribute1";                                   // 属性1
		public const string FIELD_USER_ATTRIBUTE2 = "attribute2";                                   // 属性2
		public const string FIELD_USER_ATTRIBUTE3 = "attribute3";                                   // 属性3
		public const string FIELD_USER_ATTRIBUTE4 = "attribute4";                                   // 属性4
		public const string FIELD_USER_ATTRIBUTE5 = "attribute5";                                   // 属性5
		public const string FIELD_USER_ATTRIBUTE6 = "attribute6";                                   // 属性6
		public const string FIELD_USER_ATTRIBUTE7 = "attribute7";                                   // 属性7
		public const string FIELD_USER_ATTRIBUTE8 = "attribute8";                                   // 属性8
		public const string FIELD_USER_ATTRIBUTE9 = "attribute9";                                   // 属性9
		public const string FIELD_USER_ATTRIBUTE10 = "attribute10";                                 // 属性10
		public const string FIELD_USER_LOGIN_ID = "login_id";                                       // ログインＩＤ
		public const string FIELD_USER_PASSWORD = "password";                                       // パスワード
		public const string FIELD_USER_QUESTION = "question";                                       // 質問
		public const string FIELD_USER_ANSWER = "answer";                                           // 回答
		public const string FIELD_USER_CAREER_ID = "career_id";										// キャリアID
		public const string FIELD_USER_MOBILE_UID = "mobile_uid";                                   // モバイルユーザID
		public const string FIELD_USER_REMOTE_ADDR = "remote_addr";									// リモートIPアドレス
		public const string FIELD_USER_MAIL_FLG = "mail_flg";                                       // メール配信フラグ
		public const string FIELD_USER_USER_MEMO = "user_memo";                                     // ユーザメモ
		public const string FIELD_USER_DEL_FLG = "del_flg";                                         // 削除フラグ
		public const string FIELD_USER_DATE_CREATED = "date_created";                               // 作成日
		public const string FIELD_USER_DATE_CHANGED = "date_changed";                               // 更新日
		public const string FIELD_USER_LAST_CHANGED = "last_changed";                               // 最終更新者
		public const string FIELD_USER_MEMBER_RANK_ID = "member_rank_id";                           // 会員ランクID
		public const string FIELD_USER_RECOMMEND_UID = "recommend_uid";								// 外部レコメンド連携用ユーザID
		public const string FIELD_USER_DATE_LAST_LOGGEDIN = "date_last_loggedin";                   // 最終ログイン日時
		public const string FIELD_USER_USER_MANAGEMENT_LEVEL_ID = "user_management_level_id";       // ユーザー管理レベルID
		public const string FIELD_USER_INTEGRATED_FLG = "integrated_flg";                           // ユーザー統合フラグ
		public const string FIELD_USER_FIXED_PURCHASE_MEMBER_FLG = "fixed_purchase_member_flg";		// 定期会員フラグ
		public const string FIELD_USER_EASY_REGISTER_FLG = "easy_register_flg";						// かんたん会員フラグ
		public const string FIELD_USER_ACCESS_COUNTRY_ISO_CODE = "access_country_iso_code";         // アクセス国ISOコード
		public const string FIELD_USER_DISP_LANGUAGE_CODE = "disp_language_code";               // 表示言語コード
		public const string FIELD_USER_DISP_LANGUAGE_LOCALE_ID = "disp_language_locale_id";     // 表示言語ロケールID
		public const string FIELD_USER_DISP_CURRENCY_CODE = "disp_currency_code";               // 表示通貨コード
		public const string FIELD_USER_DISP_CURRENCY_LOCALE_ID = "disp_currency_locale_id";     // 表示通貨ロケールID
		public const string FIELD_USER_LAST_BIRTHDAY_POINT_ADD_YEAR = "last_birthday_point_add_year";// 最終誕生日ポイント付与年
		public const string FIELD_USER_ADDR_COUNTRY_ISO_CODE = "addr_country_iso_code";             // 住所国ISOコード
		public const string FIELD_USER_ADDR_COUNTRY_NAME = "addr_country_name";                     // 住所国名
		public const string FIELD_USER_ADDR5 = "addr5";                                             // 住所5
		public const string FIELD_USER_LAST_BIRTHDAY_COUPON_PUBLISH_YEAR = "last_birthday_coupon_publish_year";// 最終誕生日クーポン付与年

		//User Business Owner 
		public const string TABLE_USER_BUSINESS_OWNER = "w2_UserBusinessOwnerGMO";                   // User Business Owner table
		public const string FIELD_USER_BUSINESS_OWNER_USER_ID = "user_id";                           // user Id
		public const string FIELD_USER_BUSINESS_OWNER_SHOP_CUSTOMER_ID = "gmo_shop_customer_id";     // Shop customer id
		public const string FIELD_USER_BUSINESS_OWNER_NAME1 = "owner_name1";                         // Owner name 1
		public const string FIELD_USER_BUSINESS_OWNER_NAME2 = "owner_name2";                         // Owner name 2
		public const string FIELD_USER_BUSINESS_OWNER_NAME_KANA1 = "owner_name_kana1";               // Owner name kana 1
		public const string FIELD_USER_BUSINESS_OWNER_NAME_KANA2 = "owner_name_kana2";               // Owner name kana 2
		public const string FIELD_USER_BUSINESS_OWNER_BIRTH_YEAR = "owner_birth_year";               // birthday - year
		public const string FIELD_USER_BUSINESS_OWNER_BIRTH_MONTH = "owner_birth_month";             // birthday - month
		public const string FIELD_USER_BUSINESS_OWNER_BIRTH_DAY = "owner_birth_day";                 // birthday - day
		public const string FIELD_USER_BUSINESS_OWNER_BIRTH = "owner_birth";                         // birthday - date
		public const string FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET = "request_budget";             // request budget
		public const string FIELD_USER_BUSINESS_OWNER_CREDIT_STATUS = "credit_status";               // credit status
		public const string FIELD_USER_BUSINESS_OWNER_DATE_CREATED = "date_created";                 // date created
		public const string FIELD_USER_BUSINESS_OWNER_DATE_CHANGED = "date_changed";                 // date changed
		public const string FLG_BUSINESS_OWNER_CREDIT_STATUS_UNDER_REVIEW = "審査中";                // GMO response: in review JP
		public const string FLG_BUSINESS_OWNER_CREDIT_STATUS_UNDER_REVIEW_EN = "INREVIEW";                // GMO response: in review EN

		/// <summary>リアルタイム購入回数（注文基準）</summary>
		public const string FIELD_USER_ORDER_COUNT_ORDER_REALTIME = "order_count_order_realtime";
		/// <summary>過去累計購入回数</summary>
		public const string FIELD_USER_ORDER_COUNT_OLD = "order_count_old";
		/// <summary>紹介コード</summary>
		public const string FIELD_USER_REFERRAL_CODE = "referral_code";
		/// <summary>紹介元ユーザーID</summary>
		public const string FIELD_USER_REFERRED_USER_ID = "referred_user_id";

		// ユーザー属性マスタ
		public const string TABLE_USERATTRIBUTE = "w2_UserAttribute";                               // ユーザー属性マスタ
		public const string FIELD_USERATTRIBUTE_USER_ID = "user_id";                                // ユーザーID
		public const string FIELD_USERATTRIBUTE_FIRST_ORDER_DATE = "first_order_date";              // 初回購入日
		public const string FIELD_USERATTRIBUTE_SECOND_ORDER_DATE = "second_order_date";            // ２回目購入日
		public const string FIELD_USERATTRIBUTE_LAST_ORDER_DATE = "last_order_date";                // 最終購入日
		public const string FIELD_USERATTRIBUTE_ENROLLMENT_DAYS = "enrollment_days";                // 在籍期間(日)
		public const string FIELD_USERATTRIBUTE_AWAY_DAYS = "away_days";                            // 離脱期間(日)
		public const string FIELD_USERATTRIBUTE_ORDER_AMOUNT_ORDER_ALL = "order_amount_order_all";  // 累計購入金額（注文基準・全体）
		public const string FIELD_USERATTRIBUTE_ORDER_AMOUNT_ORDER_FP = "order_amount_order_fp";    // 累計購入金額（注文基準・定期のみ）
		public const string FIELD_USERATTRIBUTE_ORDER_COUNT_ORDER_ALL = "order_count_order_all";    // 累計購入回数（注文基準・全体）
		public const string FIELD_USERATTRIBUTE_ORDER_COUNT_ORDER_FP = "order_count_order_fp";      // 累計購入回数（注文基準・定期のみ）
		public const string FIELD_USERATTRIBUTE_ORDER_AMOUNT_SHIP_ALL = "order_amount_ship_all";    // 累計購入金額（出荷基準・全体）
		public const string FIELD_USERATTRIBUTE_ORDER_AMOUNT_SHIP_FP = "order_amount_ship_fp";      // 累計購入金額（出荷基準・定期のみ）
		public const string FIELD_USERATTRIBUTE_ORDER_COUNT_SHIP_ALL = "order_count_ship_all";      // 累計購入回数（出荷基準・全体）
		public const string FIELD_USERATTRIBUTE_ORDER_COUNT_SHIP_FP = "order_count_ship_fp";        // 累計購入回数（出荷基準・定期のみ）
		public const string FIELD_USERATTRIBUTE_DATE_CREATED = "date_created";                      // 作成日
		public const string FIELD_USERATTRIBUTE_DATE_CHANGED = "date_changed";                      // 更新日
		public const string FIELD_USERATTRIBUTE_LAST_CHANGED = "last_changed";                      // 最終更新者
		public const string FIELD_USERATTRIBUTE_CPM_CLUSTER_ATTRIBUTE1 = "cpm_cluster_attribute1";  // CPMクラスタ属性1
		public const string FIELD_USERATTRIBUTE_CPM_CLUSTER_ATTRIBUTE2 = "cpm_cluster_attribute2";  // CPMクラスタ属性2
		public const string FIELD_USERATTRIBUTE_CPM_CLUSTER_ID = "cpm_cluster_id";                  // CPMクラスタID
		public const string FIELD_USERATTRIBUTE_CPM_CLUSTER_ATTRIBUTE1_BEFORE = "cpm_cluster_attribute1_before";// 以前のCPMクラスタ属性1
		public const string FIELD_USERATTRIBUTE_CPM_CLUSTER_ATTRIBUTE2_BEFORE = "cpm_cluster_attribute2_before";// 以前のCPMクラスタ属性2
		public const string FIELD_USERATTRIBUTE_CPM_CLUSTER_ID_BEFORE = "cpm_cluster_id_before";    // 以前のCPMクラスタID
		public const string FIELD_USERATTRIBUTE_CPM_CLUSTER_CHANGED_DATE = "cpm_cluster_changed_date";// CPMクラスタ変更日

		// ユーザ配送先情報
		public const string TABLE_USERSHIPPING = "w2_UserShipping";                                 // ユーザ配送先情報
		public const string FIELD_USERSHIPPING_USER_ID = "user_id";                                 // ユーザID
		public const string FIELD_USERSHIPPING_SHIPPING_NO = "shipping_no";                         // 配送先枝番
		public const string FIELD_USERSHIPPING_NAME = "name";                                       // 配送先名
		public const string FIELD_USERSHIPPING_SHIPPING_NAME = "shipping_name";                     // 配送先氏名
		public const string FIELD_USERSHIPPING_SHIPPING_NAME_KANA = "shipping_name_kana";           // 配送先氏名かな
		public const string FIELD_USERSHIPPING_SHIPPING_ZIP = "shipping_zip";                       // 郵便番号
		public const string FIELD_USERSHIPPING_SHIPPING_ADDR1 = "shipping_addr1";                   // 住所1
		public const string FIELD_USERSHIPPING_SHIPPING_ADDR2 = "shipping_addr2";                   // 住所2
		public const string FIELD_USERSHIPPING_SHIPPING_ADDR3 = "shipping_addr3";                   // 住所3
		public const string FIELD_USERSHIPPING_SHIPPING_ADDR4 = "shipping_addr4";                   // 住所4
		public const string FIELD_USERSHIPPING_SHIPPING_TEL1 = "shipping_tel1";                     // 電話番号1
		public const string FIELD_USERSHIPPING_SHIPPING_TEL2 = "shipping_tel2";                     // 電話番号2
		public const string FIELD_USERSHIPPING_SHIPPING_TEL3 = "shipping_tel3";                     // 電話番号3
		public const string FIELD_USERSHIPPING_SHIPPING_FAX = "shipping_fax";                       // ＦＡＸ
		public const string FIELD_USERSHIPPING_SHIPPING_COMPANY = "shipping_company";               // 配送業者
		public const string FIELD_USERSHIPPING_DEL_FLG = "del_flg";                                 // 削除フラグ
		public const string FIELD_USERSHIPPING_DATE_CREATED = "date_created";                       // 作成日
		public const string FIELD_USERSHIPPING_DATE_CHANGED = "date_changed";                       // 更新日
		public const string FIELD_USERSHIPPING_SHIPPING_NAME1 = "shipping_name1";                   // 配送先氏名1
		public const string FIELD_USERSHIPPING_SHIPPING_NAME2 = "shipping_name2";                   // 配送先氏名2
		public const string FIELD_USERSHIPPING_SHIPPING_NAME_KANA1 = "shipping_name_kana1";         // 配送先氏名かな1
		public const string FIELD_USERSHIPPING_SHIPPING_NAME_KANA2 = "shipping_name_kana2";         // 配送先氏名かな2
		public const string FIELD_USERSHIPPING_SHIPPING_COMPANY_NAME = "shipping_company_name";     // 企業名
		public const string FIELD_USERSHIPPING_SHIPPING_COMPANY_POST_NAME = "shipping_company_post_name";// 部署名
		public const string FIELD_USERSHIPPING_SHIPPING_COUNTRY_ISO_CODE = "shipping_country_iso_code";// 住所国ISOコード
		public const string FIELD_USERSHIPPING_SHIPPING_COUNTRY_NAME = "shipping_country_name";     // 住所国名
		public const string FIELD_USERSHIPPING_SHIPPING_ADDR5 = "shipping_addr5";                   // 住所5
		public const string FIELD_USERSHIPPING_SHIPPING_RECEIVING_STORE_FLG = "shipping_receiving_store_flg";// 店舗受取フラグ
		public const string FIELD_USERSHIPPING_SHIPPING_RECEIVING_STORE_ID = "shipping_receiving_store_id";// 店舗受取店舗ID
		/// <summary>コンビニ受取：受取方法</summary>
		public const string FIELD_USERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE = "shipping_receiving_store_type";

		// ユーザー統合情報
		public const string TABLE_USERINTEGRATION = "w2_UserIntegration";                           // ユーザー統合情報
		public const string FIELD_USERINTEGRATION_USER_INTEGRATION_NO = "user_integration_no";      // ユーザー統合No
		public const string FIELD_USERINTEGRATION_STATUS = "status";                                // ステータス
		public const string FIELD_USERINTEGRATION_DATE_CREATED = "date_created";                    // 作成日
		public const string FIELD_USERINTEGRATION_DATE_CHANGED = "date_changed";                    // 更新日
		public const string FIELD_USERINTEGRATION_LAST_CHANGED = "last_changed";                    // 最終更新者

		// ユーザー統合ユーザー情報
		public const string TABLE_USERINTEGRATIONUSER = "w2_UserIntegrationUser";                   // ユーザー統合ユーザ情報
		public const string FIELD_USERINTEGRATIONUSER_USER_INTEGRATION_NO = "user_integration_no";  // ユーザー統合No
		public const string FIELD_USERINTEGRATIONUSER_USER_ID = "user_id";                          // ユーザーID
		public const string FIELD_USERINTEGRATIONUSER_REPRESENTATIVE_FLG = "representative_flg";    // 代表フラグ
		public const string FIELD_USERINTEGRATIONUSER_DATE_CREATED = "date_created";                // 作成日
		public const string FIELD_USERINTEGRATIONUSER_DATE_CHANGED = "date_changed";                // 更新日
		public const string FIELD_USERINTEGRATIONUSER_LAST_CHANGED = "last_changed";                // 最終更新者

		// ユーザー統合履歴情報
		public const string TABLE_USERINTEGRATIONHISTORY = "w2_UserIntegrationHistory";             // ユーザー統合履歴情報
		public const string FIELD_USERINTEGRATIONHISTORY_USER_INTEGRATION_NO = "user_integration_no";// ユーザー統合No
		public const string FIELD_USERINTEGRATIONHISTORY_USER_ID = "user_id";                       // ユーザーID
		public const string FIELD_USERINTEGRATIONHISTORY_BRANCH_NO = "branch_no";                   // 枝番
		public const string FIELD_USERINTEGRATIONHISTORY_TABLE_NAME = "table_name";                 // テーブル名
		public const string FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY1 = "primary_key1";             // 主キー1
		public const string FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY2 = "primary_key2";             // 主キー2
		public const string FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY3 = "primary_key3";             // 主キー3
		public const string FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY4 = "primary_key4";             // 主キー4
		public const string FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY5 = "primary_key5";             // 主キー5
		public const string FIELD_USERINTEGRATIONHISTORY_DATE_CREATED = "date_created";             // 作成日
		public const string FIELD_USERINTEGRATIONHISTORY_DATE_CHANGED = "date_changed";             // 更新日
		public const string FIELD_USERINTEGRATIONHISTORY_LAST_CHANGED = "last_changed";             // 最終更新者

		// ユーザーデフォルト注文方法設定
		public const string TABLE_USERDEFAULTORDERSETTING = "w2_UserDefaultOrderSetting";			// ユーザーデフォルト注文方法設定
		public const string FIELD_USERDEFAULTORDERSETTING_USER_ID = "user_id";						// ユーザーID
		public const string FIELD_USERDEFAULTORDERSETTING_PAYMENT_ID = "payment_id";				// 決済種別ID
		public const string FIELD_USERDEFAULTORDERSETTING_CREDIT_BRANCH_NO = "credit_branch_no";	// カード枝番
		public const string FIELD_USERDEFAULTORDERSETTING_USER_SHIPPING_NO = "user_shipping_no";	// 配送先枝番
		public const string FIELD_USERDEFAULTORDERSETTING_DATE_CREATED = "date_created";			// 作成日
		public const string FIELD_USERDEFAULTORDERSETTING_DATE_CHANGED = "date_changed";			// 更新日
		public const string FIELD_USERDEFAULTORDERSETTING_LAST_CHANGED = "last_changed";			// 最終更新者
		public const string FIELD_USERDEFAULTORDERSETTING_USER_INVOICE_NO = "user_invoice_no";		// User Invoice No
		/// <summary>楽天コンビニ前払い支払いコンビニ</summary>
		public const string FIELD_USERDEFAULTORDERSETTING_RAKUTEN_CVS_TYPE = "rakuten_cvs_type";
		public const string FIELD_USERDEFAULTORDERSETTING_USER_ZEUS_CVS_TYPE = "zeus_cvs_type";		// Zeusコンビニ前払い支払いコンビニ
		/// <summary>Paygent convenience store type</summary>
		public const string FIELD_USERDEFAULTORDERSETTING_PAYGENT_CVS_TYPE = "paygent_cvs_type";

		// ユーザーアクティビティ
		public const string TABLE_USERACTIVITY = "w2_UserActivity";                                 // ユーザーアクティビティ
		public const string FIELD_USERACTIVITY_USER_ID = "user_id";                                 // ユーザーID
		public const string FIELD_USERACTIVITY_MASTER_KBN = "master_kbn";                           // マスター区分
		public const string FIELD_USERACTIVITY_MASTER_ID = "master_id";                             // マスターID
		public const string FIELD_USERACTIVITY_DATE = "date";                                       // 日付
		public const string FIELD_USERACTIVITY_TGT_YEAR = "tgt_year";                               // 対象年
		public const string FIELD_USERACTIVITY_TGT_MONTH = "tgt_month";                             // 対象月
		public const string FIELD_USERACTIVITY_TGT_DAY = "tgt_day";                                 // 対象日

		// 商品カテゴリマスタ
		public const string TABLE_PRODUCTCATEGORY = "w2_ProductCategory";                           // 商品カテゴリマスタ
		public const string FIELD_PRODUCTCATEGORY_SHOP_ID = "shop_id";                              // 店舗ID
		public const string FIELD_PRODUCTCATEGORY_CATEGORY_ID = "category_id";                      // カテゴリID
		public const string FIELD_PRODUCTCATEGORY_PARENT_CATEGORY_ID = "parent_category_id";        // 親カテゴリID
		public const string FIELD_PRODUCTCATEGORY_NAME = "name";                                    // カテゴリ名
		public const string FIELD_PRODUCTCATEGORY_NAME_KANA = "name_kana";                              // カテゴリ名 (フリガナ)
		public const string FIELD_PRODUCTCATEGORY_DISPLAY_ORDER = "display_order";                      // 表示順
		public const string FIELD_PRODUCTCATEGORY_CHILD_CATEGORY_SORT_KBN = "child_category_sort_kbn";  // 子カテゴリの並び順
		public const string FIELD_PRODUCTCATEGORY_NAME_MOBILE = "name_mobile";                      // モバイル用カテゴリ名
		public const string FIELD_PRODUCTCATEGORY_SEO_KEYWORDS = "seo_keywords";                    // SEOキーワード
		public const string FIELD_PRODUCTCATEGORY_CANONICAL_TEXT = "canonical_text";                // カノニカルタグ用テキスト
		public const string FIELD_PRODUCTCATEGORY_URL = "url";                                      // カテゴリページURL
		public const string FIELD_PRODUCTCATEGORY_MOBILE_DISP_FLG = "mobile_disp_flg";              // モバイル表示フラグ
		public const string FIELD_PRODUCTCATEGORY_VALID_FLG = "valid_flg";                          // 有効フラグ
		public const string FIELD_PRODUCTCATEGORY_DEL_FLG = "del_flg";                              // 削除フラグ
		public const string FIELD_PRODUCTCATEGORY_DATE_CREATED = "date_created";                    // 作成日
		public const string FIELD_PRODUCTCATEGORY_DATE_CHANGED = "date_changed";                    // 更新日
		public const string FIELD_PRODUCTCATEGORY_LAST_CHANGED = "last_changed";                    // 最終更新者
		public const string FIELD_PRODUCTCATEGORY_PERMITTED_BRAND_IDS = "permitted_brand_ids";      // 許可ブランドID
		public const string FIELD_PRODUCTCATEGORY_USE_RECOMMEND_FLG = "use_recommend_flg";          // 外部レコメンド利用フラグ
		public const string FIELD_PRODUCTCATEGORY_MEMBER_RANK_ID = "member_rank_id";                // 閲覧可能会員ランクID
		public const string FIELD_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG = "lower_member_can_display_tree_flg";// 会員ランク表示制御：カテゴリツリー表示フラグ
		public const string FIELD_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG = "only_fixed_purchase_member_flg";	// 定期会員限定フラグ 

		// 商品マスタ
		public const string TABLE_PRODUCT = "w2_Product";                                           // 商品マスタ
		public const string FIELD_PRODUCT_SHOP_ID = "shop_id";                                      // 店舗ID
		public const string FIELD_PRODUCT_SUPPLIER_ID = "supplier_id";                              // サプライヤID
		public const string FIELD_PRODUCT_PRODUCT_ID = "product_id";                                // 商品ID
		public const string FIELD_PRODUCT_VARIATION_ID = "variation_id";                                // 商品ID
		public const string FIELD_PRODUCT_COOPERATION_ID1 = "cooperation_id1";                      // 商品連携ID1
		public const string FIELD_PRODUCT_COOPERATION_ID2 = "cooperation_id2";                      // 商品連携ID2
		public const string FIELD_PRODUCT_COOPERATION_ID3 = "cooperation_id3";                      // 商品連携ID3
		public const string FIELD_PRODUCT_COOPERATION_ID4 = "cooperation_id4";                      // 商品連携ID4
		public const string FIELD_PRODUCT_COOPERATION_ID5 = "cooperation_id5";                      // 商品連携ID5
		public const string FIELD_PRODUCT_MALL_EX_PRODUCT_ID = "mall_ex_product_id";              // モール拡張商品ID
		public const string FIELD_PRODUCT_MAKER_ID = "maker_id";                                    // メーカーID
		public const string FIELD_PRODUCT_MAKER_NAME = "maker_name";                                // メーカー名
		public const string FIELD_PRODUCT_NAME = "name";                                            // 商品名
		public const string FIELD_PRODUCT_NAME_KANA = "name_kana";                                  // 商品名かな
		public const string FIELD_PRODUCT_NAME2 = "name2";                                          // 商品名2
		public const string FIELD_PRODUCT_NAME2_KANA = "name2_kana";                                // 商品名2かな
		public const string FIELD_PRODUCT_SEO_KEYWORDS = "seo_keywords";                            // SEOキーワード
		public const string FIELD_PRODUCT_CATCHCOPY = "catchcopy";                                  // キャッチコピー
		public const string FIELD_PRODUCT_CATCHCOPY_MOBILE = "catchcopy_mobile";                    // モバイルキャッチコピー
		public const string FIELD_PRODUCT_SEARCH_KEYWORD = "search_keyword";                        // 検索キーワード
		public const string FIELD_PRODUCT_OUTLINE_KBN = "outline_kbn";                              // 商品概要HTML区分
		public const string FIELD_PRODUCT_OUTLINE = "outline";                                      // 商品概要
		public const string FIELD_PRODUCT_OUTLINE_KBN_MOBILE = "outline_kbn_mobile";                // モバイル商品概要HTML区分
		public const string FIELD_PRODUCT_OUTLINE_MOBILE = "outline_mobile";                        // モバイル商品概要
		public const string FIELD_PRODUCT_DESC_DETAIL_KBN1 = "desc_detail_kbn1";                    // 商品詳細１HTML区分
		public const string FIELD_PRODUCT_DESC_DETAIL1 = "desc_detail1";                            // 商品詳細１
		public const string FIELD_PRODUCT_DESC_DETAIL_KBN2 = "desc_detail_kbn2";                    // 商品詳細2HTML区分
		public const string FIELD_PRODUCT_DESC_DETAIL2 = "desc_detail2";                            // 商品詳細2
		public const string FIELD_PRODUCT_DESC_DETAIL_KBN3 = "desc_detail_kbn3";                    // 商品詳細3HTML区分
		public const string FIELD_PRODUCT_DESC_DETAIL3 = "desc_detail3";                            // 商品詳細3
		public const string FIELD_PRODUCT_DESC_DETAIL_KBN4 = "desc_detail_kbn4";                    // 商品詳細4HTML区分
		public const string FIELD_PRODUCT_DESC_DETAIL4 = "desc_detail4";                            // 商品詳細4
		public const string FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE = "return_exchange_message";      // 返品交換文言
		public const string FIELD_PRODUCT_DESC_DETAIL_KBN1_MOBILE = "desc_detail_kbn1_mobile";      // モバイル商品詳細１HTML区分
		public const string FIELD_PRODUCT_DESC_DETAIL1_MOBILE = "desc_detail1_mobile";              // モバイル商品詳細１
		public const string FIELD_PRODUCT_DESC_DETAIL_KBN2_MOBILE = "desc_detail_kbn2_mobile";      // モバイル商品詳細2HTML区分
		public const string FIELD_PRODUCT_DESC_DETAIL2_MOBILE = "desc_detail2_mobile";              // モバイル商品詳細2
		public const string FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE_MOBILE = "return_exchange_message_mobile";// モバイル返品交換文言
		public const string FIELD_PRODUCT_NOTE = "note";                                            // 備考
		public const string FIELD_PRODUCT_DISPLAY_PRICE = "display_price";                          // 商品表示価格
		public const string FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE = "display_special_price";          // 商品表示特別価格
		public const string FIELD_PRODUCT_TAX_INCLUDED_FLG = "tax_included_flg";                    // 税込フラグ
		public const string FIELD_PRODUCT_TAX_RATE = "tax_rate";                                    // 税率
		public const string FIELD_PRODUCT_TAX_ROUND_TYPE = "tax_round_type";                        // 税計算方法
		public const string FIELD_PRODUCT_PRICE_PRETAX = "price_pretax";                            // 税込販売価格
		public const string FIELD_PRODUCT_PRICE_SHIPPING = "price_shipping";                        // 配送料
		public const string FIELD_PRODUCT_SHIPPING_TYPE = "shipping_type";                          // 配送料種別
		public const string FIELD_PRODUCT_SHIPPING_ID = "shipping_id";                              // 配送種別ID
		public const string FIELD_PRODUCT_SHIPPING_SIZE_KBN = "shipping_size_kbn";                  // 配送サイズ区分
		public const string FIELD_PRODUCT_PRICE_COST = "price_cost";                                // 商品原価
		public const string FIELD_PRODUCT_POINT_KBN1 = "point_kbn1";                                // 付与ポイント区分１
		public const string FIELD_PRODUCT_POINT1 = "point1";                                        // 付与ポイント１
		public const string FIELD_PRODUCT_POINT_KBN2 = "point_kbn2";                                // 定期購入付与ポイント区分
		public const string FIELD_PRODUCT_POINT2 = "point2";                                        // 定期購入付与ポイント
		public const string FIELD_PRODUCT_POINT_KBN3 = "point_kbn3";                                // 付与ポイント区分３
		public const string FIELD_PRODUCT_POINT3 = "point3";                                        // 付与ポイント３
		public const string FIELD_PRODUCT_CAMPAIGN_FROM = "campaign_from";                          // キャンペーン期間(FROM)
		public const string FIELD_PRODUCT_CAMPAIGN_TO = "campaign_to";                              // キャンペーン期間(TO)
		public const string FIELD_PRODUCT_CAMPAIGN_POINT_KBN = "campaign_point_kbn";                // キャンペーン付与ポイント区分
		public const string FIELD_PRODUCT_CAMPAIGN_POINT = "campaign_point";                        // キャンペーン付与ポイント
		public const string FIELD_PRODUCT_DISPLAY_FROM = "display_from";                            // 表示期間(FROM)
		public const string FIELD_PRODUCT_DISPLAY_TO = "display_to";                                // 表示期間(TO)
		public const string FIELD_PRODUCT_SELL_FROM = "sell_from";                                  // 販売期間(FROM)
		public const string FIELD_PRODUCT_SELL_TO = "sell_to";                                      // 販売期間(TO)
		public const string FIELD_PRODUCT_BEFORE_SALE_DISPLAY_KBN = "before_sale_display_kbn";      // 販売期間前表示フラグ
		public const string FIELD_PRODUCT_AFTER_SALE_DISPLAY_KBN = "after_sale_display_kbn";        // 販売期間後表示フラグ
		public const string FIELD_PRODUCT_MAX_SELL_QUANTITY = "max_sell_quantity";                  // 販売可能数
		public const string FIELD_PRODUCT_STOCK_MANAGEMENT_KBN = "stock_management_kbn";            // 在庫管理方法
		public const string FIELD_PRODUCT_STOCK_DISP_KBN = "stock_disp_kbn";                        // 在庫表示区分
		public const string FIELD_PRODUCT_STOCK_MESSAGE_ID = "stock_message_id";                    // 商品在庫文言ID
		public const string FIELD_PRODUCT_URL = "url";                                              // 紹介URL
		public const string FIELD_PRODUCT_INQUIRE_EMAIL = "inquire_email";                          // 問い合わせ用メールアドレス
		public const string FIELD_PRODUCT_INQUIRE_TEL = "inquire_tel";                              // 問い合わせ用電話番号
		public const string FIELD_PRODUCT_DISPLAY_KBN = "display_kbn";                              // 検索時表示区分
		public const string FIELD_PRODUCT_CATEGORY_ID1 = "category_id1";                            // カテゴリID1
		public const string FIELD_PRODUCT_CATEGORY_ID2 = "category_id2";                            // カテゴリID2
		public const string FIELD_PRODUCT_CATEGORY_ID3 = "category_id3";                            // カテゴリID3
		public const string FIELD_PRODUCT_CATEGORY_ID4 = "category_id4";                            // カテゴリID4
		public const string FIELD_PRODUCT_CATEGORY_ID5 = "category_id5";                            // カテゴリID5
		public const string FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1 = "related_product_id_cs1";        // 関連商品ID1（クロスセル）
		public const string FIELD_PRODUCT_RELATED_PRODUCT_ID_CS2 = "related_product_id_cs2";        // 関連商品ID2（クロスセル）
		public const string FIELD_PRODUCT_RELATED_PRODUCT_ID_CS3 = "related_product_id_cs3";        // 関連商品ID3（クロスセル）
		public const string FIELD_PRODUCT_RELATED_PRODUCT_ID_CS4 = "related_product_id_cs4";        // 関連商品ID4（クロスセル）
		public const string FIELD_PRODUCT_RELATED_PRODUCT_ID_CS5 = "related_product_id_cs5";        // 関連商品ID5（クロスセル）
		public const string FIELD_PRODUCT_RELATED_PRODUCT_ID_US1 = "related_product_id_us1";        // 関連商品ID1（アップセル）
		public const string FIELD_PRODUCT_RELATED_PRODUCT_ID_US2 = "related_product_id_us2";        // 関連商品ID2（アップセル）
		public const string FIELD_PRODUCT_RELATED_PRODUCT_ID_US3 = "related_product_id_us3";        // 関連商品ID3（アップセル）
		public const string FIELD_PRODUCT_RELATED_PRODUCT_ID_US4 = "related_product_id_us4";        // 関連商品ID4（アップセル）
		public const string FIELD_PRODUCT_RELATED_PRODUCT_ID_US5 = "related_product_id_us5";        // 関連商品ID5（アップセル）
		public const string FIELD_PRODUCT_IMAGE_HEAD = "image_head";                                // 商品画像名ヘッダ
		public const string FIELD_PRODUCT_IMAGE_MOBILE = "image_mobile";                            // モバイル商品画像名
		public const string FIELD_PRODUCT_ICON_FLG1 = "icon_flg1";                                  // アイコンフラグ1
		public const string FIELD_PRODUCT_ICON_TERM_END1 = "icon_term_end1";                        // アイコン有効期限1
		public const string FIELD_PRODUCT_ICON_FLG2 = "icon_flg2";                                  // アイコンフラグ2
		public const string FIELD_PRODUCT_ICON_TERM_END2 = "icon_term_end2";                        // アイコン有効期限2
		public const string FIELD_PRODUCT_ICON_FLG3 = "icon_flg3";                                  // アイコンフラグ3
		public const string FIELD_PRODUCT_ICON_TERM_END3 = "icon_term_end3";                        // アイコン有効期限3
		public const string FIELD_PRODUCT_ICON_FLG4 = "icon_flg4";                                  // アイコンフラグ4
		public const string FIELD_PRODUCT_ICON_TERM_END4 = "icon_term_end4";                        // アイコン有効期限4
		public const string FIELD_PRODUCT_ICON_FLG5 = "icon_flg5";                                  // アイコンフラグ5
		public const string FIELD_PRODUCT_ICON_TERM_END5 = "icon_term_end5";                        // アイコン有効期限5
		public const string FIELD_PRODUCT_ICON_FLG6 = "icon_flg6";                                  // アイコンフラグ6
		public const string FIELD_PRODUCT_ICON_TERM_END6 = "icon_term_end6";                        // アイコン有効期限6
		public const string FIELD_PRODUCT_ICON_FLG7 = "icon_flg7";                                  // アイコンフラグ7
		public const string FIELD_PRODUCT_ICON_TERM_END7 = "icon_term_end7";                        // アイコン有効期限7
		public const string FIELD_PRODUCT_ICON_FLG8 = "icon_flg8";                                  // アイコンフラグ8
		public const string FIELD_PRODUCT_ICON_TERM_END8 = "icon_term_end8";                        // アイコン有効期限8
		public const string FIELD_PRODUCT_ICON_FLG9 = "icon_flg9";                                  // アイコンフラグ9
		public const string FIELD_PRODUCT_ICON_TERM_END9 = "icon_term_end9";                        // アイコン有効期限9
		public const string FIELD_PRODUCT_ICON_FLG10 = "icon_flg10";                                // アイコンフラグ10
		public const string FIELD_PRODUCT_ICON_TERM_END10 = "icon_term_end10";                      // アイコン有効期限10
		public const string FIELD_PRODUCT_MOBILE_DISP_FLG = "mobile_disp_flg";                      // モバイル表示フラグ
		public const string FIELD_PRODUCT_USE_VARIATION_FLG = "use_variation_flg";                  // 複数バリエーション使用フラグ
		public const string FIELD_PRODUCT_RESERVATION_FLG = "reservation_flg";                      // 予約商品フラグ
		public const string FIELD_PRODUCT_FIXED_PURCHASE_FLG = "fixed_purchase_flg";                // 定期購入フラグ
		public const string FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG = "fixed_purchase_product_order_limit_flg";// 定期商品購入制限チェックフラグ
		public const string FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG = "product_order_limit_flg_fp";// 通常商品購入制限チェックフラグ
		public const string FIELD_PRODUCT_MALL_COOPERATED_FLG = "mall_cooperated_flg";              // モール連携済フラグ
		public const string FIELD_PRODUCT_VALID_FLG = "valid_flg";                                  // 有効フラグ
		public const string FIELD_PRODUCT_DEL_FLG = "del_flg";                                      // 削除フラグ
		public const string FIELD_PRODUCT_DATE_CREATED = "date_created";                            // 作成日
		public const string FIELD_PRODUCT_DATE_CHANGED = "date_changed";                            // 更新日
		public const string FIELD_PRODUCT_LAST_CHANGED = "last_changed";                            // 最終更新者
		public const string FIELD_PRODUCT_GOOGLE_SHOPPING_FLG = "google_shopping_flg";				// Googleショッピング連携フラグ
		public const string FIELD_PRODUCT_PRODUCT_OPTION_SETTINGS = "product_option_settings";          // 商品付帯情報
		public const string FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG = "member_rank_discount_flg";    // 会員ランク割引対象フラグ
		public const string FIELD_PRODUCT_DISPLAY_MEMBER_RANK = "display_member_rank";              // 閲覧可能会員ランク
		public const string FIELD_PRODUCT_BUYABLE_MEMBER_RANK = "buyable_member_rank";              // 購入可能会員ランク
		public const string FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG = "arrival_mail_valid_flg";        // 再入荷通知メール有効フラグ
		public const string FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG = "release_mail_valid_flg";        // 販売開始通知メール有効フラグ
		public const string FIELD_PRODUCT_RESALE_MAIL_VALID_FLG = "resale_mail_valid_flg";			// 再販売通知メール有効フラグ
		public const string FIELD_PRODUCT_SELECT_VARIATION_KBN = "select_variation_kbn";            // PC用商品バリエーション選択方法
		public const string FIELD_PRODUCT_SELECT_VARIATION_KBN_MOBILE = "select_variation_kbn_mobile";// モバイル用商品バリエーション選択方法
		public const string FIELD_PRODUCT_BRAND_ID1 = "brand_id1";                                    // ブランドID
		public const string FIELD_PRODUCT_BRAND_ID2 = "brand_id2";                                  // ブランドID2
		public const string FIELD_PRODUCT_BRAND_ID3 = "brand_id3";                                  // ブランドID3
		public const string FIELD_PRODUCT_BRAND_ID4 = "brand_id4";                                  // ブランドID4
		public const string FIELD_PRODUCT_BRAND_ID5 = "brand_id5";                                  // ブランドID5
		public const string FIELD_PRODUCT_USE_RECOMMEND_FLG = "use_recommend_flg";                  // 外部レコメンド利用フラグ
		public const string FIELD_PRODUCT_AGE_LIMIT_FLG = "age_limit_flg";                          // 年齢制限フラグ
		public const string FIELD_PRODUCT_GIFT_FLG = "gift_flg";                                    // ギフト購入フラグ
		public const string FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG = "plural_shipping_price_free_flg";// 配送料金複数個無料フラグ
		public const string FIELD_PRODUCT_DIGITAL_CONTENTS_FLG = "digital_contents_flg";			// デジタルコンテンツ商品フラグ
		public const string FIELD_PRODUCT_DOWNLOAD_URL = "download_url";							// ダウンロードURL
		public const string FIELD_PRODUCT_DISPLAY_SELL_FLG = "display_sell_flg";                                      // 販売期間表示フラグ
		public const string FIELD_PRODUCT_DISPLAY_PRIORITY = "display_priority";                                      // 表示優先順
		public const string FIELD_PRODUCT_LIMITED_PAYMENT_IDS = "limited_payment_ids";				// Field product limited payment ids
		public const string FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE = "fixed_purchase_firsttime_price";	// 定期初回購入価格
		public const string FIELD_PRODUCT_FIXED_PURCHASE_PRICE = "fixed_purchase_price";			// 定期購入価格
		public const string FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE = "bundle_item_display_type";            // 同梱商品明細表示フラグ
		public const string FIELD_PRODUCT_PRODUCT_TYPE = "product_type";                          // 商品区分
		public const string FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING = "limited_fixed_purchase_kbn1_setting";  // 利用不可配送周期月
		/// <summary>利用不可配送周期週</summary>
		public const string FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING = "limited_fixed_purchase_kbn4_setting";
		public const string FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING = "limited_fixed_purchase_kbn3_setting";  // 利用不可配送周期日
		public const string FIELD_PRODUCT_RECOMMEND_PRODUCT_ID = "recommend_product_id";  // レコメンド用商品ID
		public const string FIELD_PRODUCT_COOPERATION_ID6 = "cooperation_id6";                      // 商品連携ID6
		public const string FIELD_PRODUCT_COOPERATION_ID7 = "cooperation_id7";                      // 商品連携ID7
		public const string FIELD_PRODUCT_COOPERATION_ID8 = "cooperation_id8";                      // 商品連携ID8
		public const string FIELD_PRODUCT_COOPERATION_ID9 = "cooperation_id9";                      // 商品連携ID9
		public const string FIELD_PRODUCT_COOPERATION_ID10 = "cooperation_id10";                    // 商品連携ID10
		public const string FIELD_PRODUCT_ANDMALL_RESERVATION_FLG = "andmall_reservation_flg";      // ＆mallの予約商品フラグ
		public const string FIELD_PRODUCT_PRODUCT_COLOR_ID = "product_color_id";                    // 商品カラーID
		public const string FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG = "display_only_fixed_purchase_member_flg";	// 定期会員限定フラグ（閲覧）
		public const string FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG = "sell_only_fixed_purchase_member_flg";	// 定期会員限定フラグ（購入）
		public const string FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM = "product_weight_gram";              // 商品重量(g）
		public const string FIELD_PRODUCT_TAX_CATEGORY_ID = "tax_category_id";                    // 商品税率カテゴリID
		public const string FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT = "fixed_purchase_cancelable_count";// 解約可能回数
		public const string FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS = "fixed_purchase_limited_user_level_ids";	// 定期購入制限ユーザレベル
		public const string FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID = "fixed_purchase_next_shipping_product_id";			// 定期購入2回目以降配送商品ID
		public const string FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID = "fixed_purchase_next_shipping_variation_id";		// 定期購入2回目以降配送商品バリエーションID
		public const string FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY = "fixed_purchase_next_shipping_item_quantity";	// 定期購入2回目以降配送商品注文個数
		public const string FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT = "fixed_purchase_limited_skipped_count";				// 定期購入スキップ制限回数
		public const string FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_KBN = "next_shipping_item_fixed_purchase_kbn";				// 定期購入2回目以降配送商品 定期購入区分
		public const string FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING = "next_shipping_item_fixed_purchase_setting";		// 定期購入2回目以降配送商品 定期購入設定
		public const string FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG = "add_cart_url_limit_flg";											// カート投入URL制限フラグ
		public const string FIELD_PRODUCT_PRODUCT_SIZE_FACTOR = "product_size_factor";													// Product size factor
		public const string FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG = "subscription_box_flg";												// Subscription box flg
		public const string FIELD_PRODUCT_MEMBER_RANK_POINT_EXCLUDE_FLG = "member_rank_point_exclude_flg";								// 会員ランクポイント付与率除外フラグ
		/// <summary>店舗受取可能フラグ</summary>
		public const string FIELD_PRODUCT_STOREPICKUP_FLG = "storepickup_flg";
		/// <summary>配送料無料時の請求料金利用フラグ</summary>
		public const string FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG = "exclude_free_shipping_flg";

		// 商品同梱テーブル
		public const string TABLE_PRODUCTBUNDLE = "w2_ProductBundle";                               // 商品同梱テーブル
		public const string FIELD_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID = "product_bundle_id";            // 商品同梱ID
		public const string FIELD_PRODUCTBUNDLE_PRODUCT_BUNDLE_NAME = "product_bundle_name";        // 商品同梱名
		public const string FIELD_PRODUCTBUNDLE_TARGET_ORDER_TYPE = "target_order_type";            // 対象注文種別
		public const string FIELD_PRODUCTBUNDLE_DESCRIPTION = "description";                        // 説明文
		public const string FIELD_PRODUCTBUNDLE_START_DATETIME = "start_datetime";                  // 開始日時
		public const string FIELD_PRODUCTBUNDLE_END_DATETIME = "end_datetime";                      // 終了日時
		public const string FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_KBN = "target_product_kbn";          // 対象商品指定方法
		public const string FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_IDS = "target_product_ids";          // 対象商品ID
		public const string FIELD_PRODUCTBUNDLE_TARGET_ORDER_FIXED_PURCHASE_COUNT_FROM = "target_order_fixed_purchase_count_from";// 対象定期注文回数_FROM
		public const string FIELD_PRODUCTBUNDLE_TARGET_ORDER_FIXED_PURCHASE_COUNT_TO = "target_order_fixed_purchase_count_to";// 対象定期注文回数_TO
		public const string FIELD_PRODUCTBUNDLE_USABLE_TIMES_KBN = "usable_times_kbn";              // ユーザ利用可能回数
		public const string FIELD_PRODUCTBUNDLE_APPLY_TYPE = "apply_type";                          // 商品同梱設定適用種別
		public const string FIELD_PRODUCTBUNDLE_VALID_FLG = "valid_flg";                            // 有効フラグ
		public const string FIELD_PRODUCTBUNDLE_MULTIPLE_APPLY_FLG = "multiple_apply_flg";          // 重複適用フラグ
		public const string FIELD_PRODUCTBUNDLE_APPLY_ORDER = "apply_order";                        // 適用優先順
		public const string FIELD_PRODUCTBUNDLE_DATE_CREATED = "date_created";                      // 作成日
		public const string FIELD_PRODUCTBUNDLE_DATE_CHANGED = "date_changed";                      // 更新日
		public const string FIELD_PRODUCTBUNDLE_LAST_CHANGED = "last_changed";                      // 最終更新者
		public const string FIELD_PRODUCTBUNDLE_USABLE_TIMES = "usable_times";	// ユーザ利用可能指定回数
		public const string FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_CATEGORY_IDS = "target_product_category_ids";	// 対象商品カテゴリID
		public const string FIELD_PRODUCTBUNDLE_EXCEPT_PRODUCT_IDS = "except_product_ids";	// 対象外商品ID
		public const string FIELD_PRODUCTBUNDLE_EXCEPT_PRODUCT_CATEGORY_IDS = "except_product_category_ids";	// 対象外商品カテゴリID
		public const string FIELD_PRODUCTBUNDLE_TARGET_ID = "target_id";	// ターゲットリストID
		public const string FIELD_PRODUCTBUNDLE_TARGET_ID_EXCEPT_FLG = "target_id_except_flg";	// ターゲットリストID除外フラグ
		public const string FIELD_PRODUCTBUNDLE_TARGET_ORDER_PRICE_SUBTOTAL_MIN = "target_order_price_subtotal_min";	// 対象注文の商品合計金額適用下限
		public const string FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_COUNT_MIN = "target_product_count_min";	// 対象商品個数適用下限
		public const string FIELD_PRODUCTBUNDLE_TARGET_ADVCODES_FIRST = "target_advcodes_first";	// 初回広告コード
		public const string FIELD_PRODUCTBUNDLE_TARGET_ADVCODES_NEW = "target_advcodes_new";	// 最新広告コード
		public const string FIELD_PRODUCTBUNDLE_TARGET_PAYMENT_IDS = "target_payment_ids";	// 決済種別
		/// <summary>対象注文種別(頒布会オプション)</summary>
		public const string FIELD_PRODUCTBUNDLE_TARGET_ORDER_TYPE_SUBSCRIPTION_BOX = "target_order_type_subscription_box";
		/// <summary>クーポンコード</summary>
		public const string FIELD_PRODUCTBUNDLE_TARGET_COUPON_CODES = "target_coupon_codes";

		// 商品同梱 同梱商品テーブル
		public const string TABLE_PRODUCTBUNDLEITEM = "w2_ProductBundleItem";                       // 商品同梱 同梱商品テーブル
		public const string FIELD_PRODUCTBUNDLEITEM_PRODUCT_BUNDLE_ID = "product_bundle_id";        // 商品同梱ID
		public const string FIELD_PRODUCTBUNDLEITEM_PRODUCT_BUNDLE_ITEM_NO = "product_bundle_item_no"; // 同梱商品枝番
		public const string FIELD_PRODUCTBUNDLEITEM_GRANT_PRODUCT_ID = "grant_product_id";          // 同梱商品ID
		public const string FIELD_PRODUCTBUNDLEITEM_GRANT_PRODUCT_VARIATION_ID = "grant_product_variation_id";  // 同梱商品バリエーションID
		public const string FIELD_PRODUCTBUNDLEITEM_GRANT_PRODUCT_COUNT = "grant_product_count";    // 同梱個数
		public const string FIELD_PRODUCTBUNDLEITEM_DATE_CREATED = "date_created";                  // 作成日
		public const string FIELD_PRODUCTBUNDLEITEM_LAST_CHANGED = "last_changed";                  // 最終更新者
		public const string FIELD_PRODUCTBUNDLEITEM_ORDERED_PRODUCT_EXCEPT_FLG = "ordered_product_except_flg"; // 初回のみ同梱フラグ

		// 商品バリエーションマスタ
		public const string TABLE_PRODUCTVARIATION = "w2_ProductVariation";                         // 商品バリエーションマスタ
		public const string FIELD_PRODUCTVARIATION_SHOP_ID = "shop_id";                             // 店舗ID
		public const string FIELD_PRODUCTVARIATION_PRODUCT_ID = "product_id";                       // 商品ID
		public const string FIELD_PRODUCTVARIATION_VARIATION_ID = "variation_id";                   // 商品バリエーションID
		public const string FIELD_PRODUCTVARIATION_FAVORITE_COUNT = "favorite_count";               // お気に入り登録数
		public const string FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1 = "variation_cooperation_id1";// 商品バリエーション連携ID1
		public const string FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2 = "variation_cooperation_id2";// 商品バリエーション連携ID2
		public const string FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3 = "variation_cooperation_id3";// 商品バリエーション連携ID3
		public const string FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4 = "variation_cooperation_id4";// 商品バリエーション連携ID4
		public const string FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5 = "variation_cooperation_id5";// 商品バリエーション連携ID5
		public const string FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1 = "mall_variation_id1";     // モールバリエーションID1
		public const string FIELD_PRODUCTVARIATION_MALL_VARIATION_ID2 = "mall_variation_id2";     // モールバリエーションID2
		public const string FIELD_PRODUCTVARIATION_MALL_VARIATION_TYPE = "mall_variation_type";   // モールバリエーション種別
		public const string FIELD_PRODUCTVARIATION_VARIATION_NAME1 = "variation_name1";             // バリエーション名1
		public const string FIELD_PRODUCTVARIATION_VARIATION_NAME2 = "variation_name2";             // バリエーション名2
		public const string FIELD_PRODUCTVARIATION_VARIATION_NAME3 = "variation_name3";             // バリエーション名3
		public const string FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID = "variation_color_id";       // 商品カラーID
		public const string FIELD_PRODUCTVARIATION_PRICE = "price";                                 // 販売価格
		public const string FIELD_PRODUCTVARIATION_SPECIAL_PRICE = "special_price";                 // 特別価格
		public const string FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD = "variation_image_head";   // バリエーション画像名ヘッダ
		public const string FIELD_PRODUCTVARIATION_VARIATION_IMAGE_MOBILE = "variation_image_mobile";// モバイルバリエーション画像名
		public const string FIELD_PRODUCTVARIATION_SHIPPING_TYPE = "shipping_type";                 // 配送料種別
		public const string FIELD_PRODUCTVARIATION_SHIPPING_SIZE_KBN = "shipping_size_kbn";         // 配送サイズ区分
		public const string FIELD_PRODUCTVARIATION_DISPLAY_ORDER = "display_order";                 // 表示順
		public const string FIELD_PRODUCTVARIATION_VARIATION_MALL_COOPERATED_FLG = "variation_mall_cooperated_flg";// バリエーションモール連携済フラグ
		public const string FIELD_PRODUCTVARIATION_VALID_FLG = "valid_flg";                         // 有効フラグ
		public const string FIELD_PRODUCTVARIATION_DEL_FLG = "del_flg";                             // 削除フラグ
		public const string FIELD_PRODUCTVARIATION_DATE_CREATED = "date_created";                   // 作成日
		public const string FIELD_PRODUCTVARIATION_DATE_CHANGED = "date_changed";                   // 更新日
		public const string FIELD_PRODUCTVARIATION_LAST_CHANGED = "last_changed";                   // 最終更新者
		public const string FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL = "variation_download_url";// ダウンロードURL
		public const string FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE = "variation_fixed_purchase_firsttime_price";	// 定期初回購入価格
		public const string FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE = "variation_fixed_purchase_price";	// 定期購入価格
		public const string FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6 = "variation_cooperation_id6";// 商品バリエーション連携ID6
		public const string FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7 = "variation_cooperation_id7";// 商品バリエーション連携ID7
		public const string FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8 = "variation_cooperation_id8";// 商品バリエーション連携ID8
		public const string FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9 = "variation_cooperation_id9";// 商品バリエーション連携ID9
		public const string FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10 = "variation_cooperation_id10";// 商品バリエーション連携ID10
		public const string FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG = "variation_andmall_reservation_flg"; // ＆mallの予約商品フラグ
		public const string FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM = "variation_weight_gram"; // 商品バリエーション重量（g）
		public const string FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG = "variation_add_cart_url_limit_flg"; // カート投入URL制限フラグ

		// 商品セットマスタ
		public const string TABLE_PRODUCTSET = "w2_ProductSet";                                     // 商品セットマスタ
		public const string FIELD_PRODUCTSET_SHOP_ID = "shop_id";                                   // 店舗ID
		public const string FIELD_PRODUCTSET_PRODUCT_SET_ID = "product_set_id";                     // 商品セットID
		public const string FIELD_PRODUCTSET_PRODUCT_SET_NAME = "product_set_name";                 // 商品セット名
		public const string FIELD_PRODUCTSET_PARENT_MIN = "parent_min";                             // 親商品数(下限)
		public const string FIELD_PRODUCTSET_PARENT_MAX = "parent_max";                             // 親商品数(上限)
		public const string FIELD_PRODUCTSET_CHILD_MIN = "child_min";                               // 子商品数(下限)
		public const string FIELD_PRODUCTSET_CHILD_MAX = "child_max";                               // 子商品数(上限)
		public const string FIELD_PRODUCTSET_PRIORITY = "priority";                                 // 適用優先順位
		public const string FIELD_PRODUCTSET_DESCRIPTION_KBN = "description_kbn";                   // 表示用文言HTML区分
		public const string FIELD_PRODUCTSET_DESCRIPTION = "description";                           // 表示用文言
		public const string FIELD_PRODUCTSET_DESCRIPTION_KBN_MOBILE = "description_kbn_mobile";     // モバイル表示用文言HTML区分
		public const string FIELD_PRODUCTSET_DESCRIPTION_MOBILE = "description_mobile";             // モバイル表示用文言
		public const string FIELD_PRODUCTSET_MAX_SELL_QUANTITY = "max_sell_quantity";               // 販売可能数
		public const string FIELD_PRODUCTSET_SHIPPING_SIZE_KBN = "shipping_size_kbn";               // 配送サイズ区分
		public const string FIELD_PRODUCTSET_EDITABLE_FLG = "editable_flg";                         // 編集可能フラグ
		public const string FIELD_PRODUCTSET_VALID_FLG = "valid_flg";                               // 有効フラグ
		public const string FIELD_PRODUCTSET_DATE_CREATED = "date_created";                         // 作成日
		public const string FIELD_PRODUCTSET_DATE_CHANGED = "date_changed";                         // 更新日
		public const string FIELD_PRODUCTSET_LAST_CHANGED = "last_changed";                         // 最終更新者

		// 商品セットアイテムマスタ
		public const string TABLE_PRODUCTSETITEM = "w2_ProductSetItem";                             // 商品セットアイテムマスタ
		public const string FIELD_PRODUCTSETITEM_SHOP_ID = "shop_id";                               // 店舗ID
		public const string FIELD_PRODUCTSETITEM_PRODUCT_SET_ID = "product_set_id";                 // 商品セットID
		public const string FIELD_PRODUCTSETITEM_PRODUCT_ID = "product_id";                         // 商品ID
		public const string FIELD_PRODUCTSETITEM_VARIATION_ID = "variation_id";                     // 商品バリエーションID
		public const string FIELD_PRODUCTSETITEM_SETITEM_PRICE = "setitem_price";                   // セット価格
		public const string FIELD_PRODUCTSETITEM_COUNT_MIN = "count_min";                           // 個数（下限）
		public const string FIELD_PRODUCTSETITEM_COUNT_MAX = "count_max";                           // 個数（上限）
		public const string FIELD_PRODUCTSETITEM_FAMILY_FLG = "family_flg";                         // 親子フラグ
		public const string FIELD_PRODUCTSETITEM_DISPLAY_ORDER = "display_order";                   // 表示順
		public const string FIELD_PRODUCTSETITEM_DATE_CREATED = "date_created";                     // 作成日
		public const string FIELD_PRODUCTSETITEM_LAST_CHANGED = "last_changed";                     // 最終更新者

		// セットプロモーションマスタ
		public const string TABLE_SETPROMOTION = "w2_SetPromotion";									// セットプロモーションマスタ
		public const string FIELD_SETPROMOTION_SETPROMOTION_ID = "setpromotion_id";                 // セットプロモーションID
		public const string FIELD_SETPROMOTION_SETPROMOTION_NAME = "setpromotion_name";             // 管理用セットプロモーション名
		public const string FIELD_SETPROMOTION_SETPROMOTION_DISP_NAME = "setpromotion_disp_name";   // 表示用セットプロモーション名
		public const string FIELD_SETPROMOTION_SETPROMOTION_DISP_NAME_MOBILE = "setpromotion_disp_name_mobile";	// モバイル表示用セットプロモーション名
		public const string FIELD_SETPROMOTION_PRODUCT_DISCOUNT_FLG = "product_discount_flg";			// 商品金額割引フラグ
		public const string FIELD_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG = "shipping_charge_free_flg";	// 配送料無料フラグ
		public const string FIELD_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG = "payment_charge_free_flg";		// 決済手数料無料フラグ
		public const string FIELD_SETPROMOTION_PRODUCT_DISCOUNT_KBN = "product_discount_kbn";			// 商品割引区分
		public const string FIELD_SETPROMOTION_PRODUCT_DISCOUNT_SETTING = "product_discount_setting";   // 商品割引設定
		public const string FIELD_SETPROMOTION_DESCRIPTION_KBN = "description_kbn";                 // 表示用文言HTML区分
		public const string FIELD_SETPROMOTION_DESCRIPTION = "description";                         // 表示用文言
		public const string FIELD_SETPROMOTION_DESCRIPTION_KBN_MOBILE = "description_kbn_mobile";   // モバイル表示用文言HTML区分
		public const string FIELD_SETPROMOTION_DESCRIPTION_MOBILE = "description_mobile";           // モバイル表示用文言
		public const string FIELD_SETPROMOTION_BEGIN_DATE = "begin_date";                           // 開始日時
		public const string FIELD_SETPROMOTION_END_DATE = "end_date";                               // 終了日時
		public const string FIELD_SETPROMOTION_TARGET_MEMBER_RANK = "target_member_rank";           // 適用会員ランク
		public const string FIELD_SETPROMOTION_TARGET_ORDER_KBN = "target_order_kbn";               // 適用注文区分
		public const string FIELD_SETPROMOTION_URL = "url";											// URL
		public const string FIELD_SETPROMOTION_URL_MOBILE = "url_mobile";                           // モバイルURL
		public const string FIELD_SETPROMOTION_DISPLAY_ORDER = "display_order";                     // 表示優先順
		public const string FIELD_SETPROMOTION_VALID_FLG = "valid_flg";                             // 有効フラグ
		public const string FIELD_SETPROMOTION_DATE_CREATED = "date_created";                       // 作成日
		public const string FIELD_SETPROMOTION_DATE_CHANGED = "date_changed";                       // 更新日
		public const string FIELD_SETPROMOTION_LAST_CHANGED = "last_changed";                       // 最終更新者
		public const string FIELD_SETPROMOTION_TARGET_TARGET_IDS = "target_target_ids";             // ターゲットID
		public const string FIELD_SETPROMOTION_APPLY_ORDER = "apply_order";                    //適用優先順

		// セットプロモーションアイテムマスタ
		public const string TABLE_SETPROMOTIONITEM = "w2_SetPromotionItem";							// セットプロモーションアイテムマスタ
		public const string FIELD_SETPROMOTIONITEM_SETPROMOTION_ID = "setpromotion_id";             // セットプロモーションID
		public const string FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_NO = "setpromotion_item_no";   // セットプロモーションアイテム枝番
		public const string FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN = "setpromotion_item_kbn";	// セットプロモーションアイテム区分
		public const string FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEMS = "setpromotion_items";       // 対象商品
		public const string FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_QUANTITY = "setpromotion_item_quantity";	// 数量
		/// <summary>数量以上フラグ</summary>
		public const string FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_QUANTITY_MORE_FLG = "setpromotion_item_quantity_more_flg";

		// 商品在庫マスタ
		public const string TABLE_PRODUCTSTOCK = "w2_ProductStock";                                 // 商品在庫マスタ
		public const string FIELD_PRODUCTSTOCK_SHOP_ID = "shop_id";                                 // 店舗ID
		public const string FIELD_PRODUCTSTOCK_PRODUCT_ID = "product_id";                           // 商品ID
		public const string FIELD_PRODUCTSTOCK_VARIATION_ID = "variation_id";                       // 商品バリエーションID
		public const string FIELD_PRODUCTSTOCK_STOCK = "stock";                                     // 商品在庫数
		public const string FIELD_PRODUCTSTOCK_STOCK_ALERT = "stock_alert";                         // 商品在庫安全基準
		public const string FIELD_PRODUCTSTOCK_REALSTOCK = "realstock";                             // 実在庫数
		public const string FIELD_PRODUCTSTOCK_REALSTOCK_B = "realstock_b";                         // 実在庫数B
		public const string FIELD_PRODUCTSTOCK_REALSTOCK_C = "realstock_c";                         // 実在庫数C
		public const string FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED = "realstock_reserved";           // 引当済実在庫数
		public const string FIELD_PRODUCTSTOCK_DATE_CREATED = "date_created";                       // 作成日
		public const string FIELD_PRODUCTSTOCK_DATE_CHANGED = "date_changed";                       // 更新日
		public const string FIELD_PRODUCTSTOCK_LAST_CHANGED = "last_changed";                       // 最終更新者

		// 商品在庫履歴マスタ
		public const string TABLE_PRODUCTSTOCKHISTORY = "w2_ProductStockHistory";                   // 商品在庫履歴マスタ
		public const string FIELD_PRODUCTSTOCKHISTORY_HISTORY_NO = "history_no";                    // 履歴NO
		public const string FIELD_PRODUCTSTOCKHISTORY_ORDER_ID = "order_id";                        // 注文ID
		public const string FIELD_PRODUCTSTOCKHISTORY_SHOP_ID = "shop_id";                          // 店舗ID
		public const string FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID = "product_id";                    // 商品ID
		public const string FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID = "variation_id";                // 商品バリエーションID
		public const string FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS = "action_status";              // アクションステータス
		public const string FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK = "add_stock";                      // 商品在庫増減
		public const string FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK = "add_realstock";              // 実在庫数増減
		public const string FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_B = "add_realstock_b";          // 実在庫数B増減
		public const string FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_C = "add_realstock_c";          // 実在庫数C増減
		public const string FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_RESERVED = "add_realstock_reserved";// 引当済実在庫数増減
		public const string FIELD_PRODUCTSTOCKHISTORY_UPDATE_STOCK = "update_stock";                // 商品在庫更新
		public const string FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK = "update_realstock";        // 実在庫数更新
		public const string FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_B = "update_realstock_b";    // 実在庫数B更新
		public const string FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_C = "update_realstock_c";    // 実在庫数C更新
		public const string FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_RESERVED = "update_realstock_reserved";// 引当済実在庫数更新
		public const string FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO = "update_memo";                  // 在庫更新メモ
		public const string FIELD_PRODUCTSTOCKHISTORY_DATE_CREATED = "date_created";                // 作成日
		public const string FIELD_PRODUCTSTOCKHISTORY_LAST_CHANGED = "last_changed";                // 最終更新者
		public const string FIELD_PRODUCTSTOCKHISTORY_SYNC_FLG = "sync_flg";						// 同期フラグ

		// 商品在庫文言マスタ
		public const string TABLE_PRODUCTSTOCKMESSAGE = "w2_ProductStockMessage";                   // 商品在庫文言マスタ
		public const string FIELD_PRODUCTSTOCKMESSAGE_SHOP_ID = "shop_id";                          // 店舗ID
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID = "stock_message_id";        // 商品在庫文言ID
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_NAME = "stock_message_name";    // 在庫文言名
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF = "stock_message_def";      // 標準在庫文言
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM1 = "stock_datum1";                // 在庫基準1
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE1 = "stock_message1";            // 在庫文言1
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM2 = "stock_datum2";                // 在庫基準2
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE2 = "stock_message2";            // 在庫文言2
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM3 = "stock_datum3";                // 在庫基準3
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE3 = "stock_message3";            // 在庫文言3
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM4 = "stock_datum4";                // 在庫基準4
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE4 = "stock_message4";            // 在庫文言4
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM5 = "stock_datum5";                // 在庫基準5
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE5 = "stock_message5";            // 在庫文言5
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM6 = "stock_datum6";                // 在庫基準6
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE6 = "stock_message6";            // 在庫文言6
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM7 = "stock_datum7";                // 在庫基準7
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE7 = "stock_message7";            // 在庫文言7
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM8 = "stock_datum8";                // 在庫基準8
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE8 = "stock_message8";            // 在庫文言8
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM9 = "stock_datum9";                // 在庫基準9
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE9 = "stock_message9";            // 在庫文言9
		public const string FIELD_PRODUCTSTOCKMESSAGE_DEL_FLG = "del_flg";                          // 削除フラグ
		public const string FIELD_PRODUCTSTOCKMESSAGE_DATE_CREATED = "date_created";                // 作成日
		public const string FIELD_PRODUCTSTOCKMESSAGE_DATE_CHANGED = "date_changed";                // 更新日
		public const string FIELD_PRODUCTSTOCKMESSAGE_LAST_CHANGED = "last_changed";                // 最終更新者
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF_MOBILE = "stock_message_def_mobile";// モバイル用標準在庫文言
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE1 = "stock_message_mobile1";// モバイル用在庫文言1
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE2 = "stock_message_mobile2";// モバイル用在庫文言2
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE3 = "stock_message_mobile3";// モバイル用在庫文言3
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE4 = "stock_message_mobile4";// モバイル用在庫文言4
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE5 = "stock_message_mobile5";// モバイル用在庫文言5
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE6 = "stock_message_mobile6";// モバイル用在庫文言6
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE7 = "stock_message_mobile7";// モバイル用在庫文言7
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE8 = "stock_message_mobile8";// モバイル用在庫文言8
		public const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE9 = "stock_message_mobile9";// モバイル用在庫文言9
		public const string FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_CODE = "language_code";              // 言語コード
		public const string FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_LOCALE_ID = "language_locale_id";    // 言語ロケールID

		// 商品セールマスタ
		public const string TABLE_PRODUCTSALE = "w2_ProductSale";                                   // 商品セールマスタ
		public const string FIELD_PRODUCTSALE_SHOP_ID = "shop_id";                                  // 店舗ID
		public const string FIELD_PRODUCTSALE_PRODUCTSALE_ID = "productsale_id";                    // 商品セールID
		public const string FIELD_PRODUCTSALE_PRODUCTSALE_KBN = "productsale_kbn";                  // 商品セール区分
		public const string FIELD_PRODUCTSALE_PRODUCTSALE_NAME = "productsale_name";                // 商品セール名
		public const string FIELD_PRODUCTSALE_CLOSEDMARKET_PASSWORD = "closedmarket_password";      // 闇市パスワード
		public const string FIELD_PRODUCTSALE_DATE_BGN = "date_bgn";                                // 開始日時
		public const string FIELD_PRODUCTSALE_DATE_END = "date_end";                                // 終了日時
		public const string FIELD_PRODUCTSALE_HOLDING_PERIOD = "holding_period";                    // 開催期間
		public const string FIELD_PRODUCTSALE_VALID_FLG = "valid_flg";                              // 有効フラグ
		public const string FIELD_PRODUCTSALE_DEL_FLG = "del_flg";                                  // 削除フラグ
		public const string FIELD_PRODUCTSALE_DATE_CREATED = "date_created";                        // 作成日
		public const string FIELD_PRODUCTSALE_DATE_CHANGED = "date_changed";                        // 更新日
		public const string FIELD_PRODUCTSALE_LAST_CHANGED = "last_changed";                        // 最終更新者

		// 商品セール価格マスタ
		public const string TABLE_PRODUCTSALEPRICE = "w2_ProductSalePrice";                         // 商品セール価格マスタ
		public const string FIELD_PRODUCTSALEPRICE_SHOP_ID = "shop_id";                             // 店舗ID
		public const string FIELD_PRODUCTSALEPRICE_PRODUCTSALE_ID = "productsale_id";               // 商品セールID
		public const string FIELD_PRODUCTSALEPRICE_PRODUCT_ID = "product_id";                       // 商品ID
		public const string FIELD_PRODUCTSALEPRICE_VARIATION_ID = "variation_id";                   // 商品バリエーションID
		public const string FIELD_PRODUCTSALEPRICE_SALE_PRICE = "sale_price";                       // 商品セール価格
		public const string FIELD_PRODUCTSALEPRICE_DISPLAY_ORDER = "display_order";                 // 表示順
		public const string FIELD_PRODUCTSALEPRICE_DEL_FLG = "del_flg";                             // 削除フラグ
		public const string FIELD_PRODUCTSALEPRICE_DATE_CREATED = "date_created";                   // 作成日
		public const string FIELD_PRODUCTSALEPRICE_DATE_CHANGED = "date_changed";                   // 更新日
		public const string FIELD_PRODUCTSALEPRICE_LAST_CHANGED = "last_changed";                   // 最終更新者

		// 商品セール価格テンポラリテーブル
		public const string TABLE_PRODUCTSALEPRICETMP = "w2_ProductSalePriceTmp";                   // 商品セール価格テンポラリテーブル
		public const string FIELD_PRODUCTSALEPRICETMP_SHOP_ID = "shop_id";                          // 店舗ID
		public const string FIELD_PRODUCTSALEPRICETMP_PRODUCTSALE_ID = "productsale_id";            // 商品セールID
		public const string FIELD_PRODUCTSALEPRICETMP_PRODUCT_ID = "product_id";                    // 商品ID
		public const string FIELD_PRODUCTSALEPRICETMP_SALE_PRICE = "sale_price";                    // 商品セール価格
		public const string FIELD_PRODUCTSALEPRICETMP_DISPLAY_ORDER = "display_order";              // 表示順
		public const string FIELD_PRODUCTSALEPRICETMP_DATE_CREATED = "date_created";                // 作成日
		public const string FIELD_PRODUCTSALEPRICETMP_LAST_CHANGED = "last_changed";                // 最終更新者

		// 商品拡張項目設定
		public const string TABLE_PRODUCTEXTENDSETTING = "w2_ProductExtendSetting";                 // 商品拡張項目設定
		public const string FIELD_PRODUCTEXTENDSETTING_SHOP_ID = "shop_id";                         // 店舗ID
		public const string FIELD_PRODUCTEXTENDSETTING_EXTEND_NO = "extend_no";                     // 拡張項目番号
		public const string FIELD_PRODUCTEXTENDSETTING_EXTEND_NAME = "extend_name";                 // 拡張項目名称
		public const string FIELD_PRODUCTEXTENDSETTING_EXTEND_DISCRIPTION = "extend_discription";   // 拡張項目説明
		public const string FIELD_PRODUCTEXTENDSETTING_DEL_FLG = "del_flg";                         // 削除フラグ
		public const string FIELD_PRODUCTEXTENDSETTING_DATE_CREATED = "date_created";               // 作成日
		public const string FIELD_PRODUCTEXTENDSETTING_DATE_CHANGED = "date_changed";               // 更新日
		public const string FIELD_PRODUCTEXTENDSETTING_LAST_CHANGED = "last_changed";               // 最終更新者

		// 商品拡張拡張項目マスタ
		public const string TABLE_PRODUCTEXTEND = "w2_ProductExtend";                               // 商品拡張拡張項目マスタ
		public const string FIELD_PRODUCTEXTEND_SHOP_ID = "shop_id";                                // 店舗ID
		public const string FIELD_PRODUCTEXTEND_PRODUCT_ID = "product_id";                          // 商品ID
		public const string FIELD_PRODUCTEXTEND_EXTEND1 = "extend1";                                // 拡張項目1
		public const string FIELD_PRODUCTEXTEND_EXTEND2 = "extend2";                                // 拡張項目2
		public const string FIELD_PRODUCTEXTEND_EXTEND3 = "extend3";                                // 拡張項目3
		public const string FIELD_PRODUCTEXTEND_EXTEND4 = "extend4";                                // 拡張項目4
		public const string FIELD_PRODUCTEXTEND_EXTEND5 = "extend5";                                // 拡張項目5
		public const string FIELD_PRODUCTEXTEND_EXTEND6 = "extend6";                                // 拡張項目6
		public const string FIELD_PRODUCTEXTEND_EXTEND7 = "extend7";                                // 拡張項目7
		public const string FIELD_PRODUCTEXTEND_EXTEND8 = "extend8";                                // 拡張項目8
		public const string FIELD_PRODUCTEXTEND_EXTEND9 = "extend9";                                // 拡張項目9
		public const string FIELD_PRODUCTEXTEND_EXTEND10 = "extend10";                              // 拡張項目10
		public const string FIELD_PRODUCTEXTEND_EXTEND11 = "extend11";                              // 拡張項目11
		public const string FIELD_PRODUCTEXTEND_EXTEND12 = "extend12";                              // 拡張項目12
		public const string FIELD_PRODUCTEXTEND_EXTEND13 = "extend13";                              // 拡張項目13
		public const string FIELD_PRODUCTEXTEND_EXTEND14 = "extend14";                              // 拡張項目14
		public const string FIELD_PRODUCTEXTEND_EXTEND15 = "extend15";                              // 拡張項目15
		public const string FIELD_PRODUCTEXTEND_EXTEND16 = "extend16";                              // 拡張項目16
		public const string FIELD_PRODUCTEXTEND_EXTEND17 = "extend17";                              // 拡張項目17
		public const string FIELD_PRODUCTEXTEND_EXTEND18 = "extend18";                              // 拡張項目18
		public const string FIELD_PRODUCTEXTEND_EXTEND19 = "extend19";                              // 拡張項目19
		public const string FIELD_PRODUCTEXTEND_EXTEND20 = "extend20";                              // 拡張項目20
		public const string FIELD_PRODUCTEXTEND_EXTEND21 = "extend21";                              // 拡張項目21
		public const string FIELD_PRODUCTEXTEND_EXTEND22 = "extend22";                              // 拡張項目22
		public const string FIELD_PRODUCTEXTEND_EXTEND23 = "extend23";                              // 拡張項目23
		public const string FIELD_PRODUCTEXTEND_EXTEND24 = "extend24";                              // 拡張項目24
		public const string FIELD_PRODUCTEXTEND_EXTEND25 = "extend25";                              // 拡張項目25
		public const string FIELD_PRODUCTEXTEND_EXTEND26 = "extend26";                              // 拡張項目26
		public const string FIELD_PRODUCTEXTEND_EXTEND27 = "extend27";                              // 拡張項目27
		public const string FIELD_PRODUCTEXTEND_EXTEND28 = "extend28";                              // 拡張項目28
		public const string FIELD_PRODUCTEXTEND_EXTEND29 = "extend29";                              // 拡張項目29
		public const string FIELD_PRODUCTEXTEND_EXTEND30 = "extend30";                              // 拡張項目30
		public const string FIELD_PRODUCTEXTEND_EXTEND31 = "extend31";                              // 拡張項目31
		public const string FIELD_PRODUCTEXTEND_EXTEND32 = "extend32";                              // 拡張項目32
		public const string FIELD_PRODUCTEXTEND_EXTEND33 = "extend33";                              // 拡張項目33
		public const string FIELD_PRODUCTEXTEND_EXTEND34 = "extend34";                              // 拡張項目34
		public const string FIELD_PRODUCTEXTEND_EXTEND35 = "extend35";                              // 拡張項目35
		public const string FIELD_PRODUCTEXTEND_EXTEND36 = "extend36";                              // 拡張項目36
		public const string FIELD_PRODUCTEXTEND_EXTEND37 = "extend37";                              // 拡張項目37
		public const string FIELD_PRODUCTEXTEND_EXTEND38 = "extend38";                              // 拡張項目38
		public const string FIELD_PRODUCTEXTEND_EXTEND39 = "extend39";                              // 拡張項目39
		public const string FIELD_PRODUCTEXTEND_EXTEND40 = "extend40";                              // 拡張項目40
		public const string FIELD_PRODUCTEXTEND_EXTEND41 = "extend41";                              // 拡張項目41
		public const string FIELD_PRODUCTEXTEND_EXTEND42 = "extend42";                              // 拡張項目42
		public const string FIELD_PRODUCTEXTEND_EXTEND43 = "extend43";                              // 拡張項目43
		public const string FIELD_PRODUCTEXTEND_EXTEND44 = "extend44";                              // 拡張項目44
		public const string FIELD_PRODUCTEXTEND_EXTEND45 = "extend45";                              // 拡張項目45
		public const string FIELD_PRODUCTEXTEND_EXTEND46 = "extend46";                              // 拡張項目46
		public const string FIELD_PRODUCTEXTEND_EXTEND47 = "extend47";                              // 拡張項目47
		public const string FIELD_PRODUCTEXTEND_EXTEND48 = "extend48";                              // 拡張項目48
		public const string FIELD_PRODUCTEXTEND_EXTEND49 = "extend49";                              // 拡張項目49
		public const string FIELD_PRODUCTEXTEND_EXTEND50 = "extend50";                              // 拡張項目50
		public const string FIELD_PRODUCTEXTEND_EXTEND51 = "extend51";                              // 拡張項目51
		public const string FIELD_PRODUCTEXTEND_EXTEND52 = "extend52";                              // 拡張項目52
		public const string FIELD_PRODUCTEXTEND_EXTEND53 = "extend53";                              // 拡張項目53
		public const string FIELD_PRODUCTEXTEND_EXTEND54 = "extend54";                              // 拡張項目54
		public const string FIELD_PRODUCTEXTEND_EXTEND55 = "extend55";                              // 拡張項目55
		public const string FIELD_PRODUCTEXTEND_EXTEND56 = "extend56";                              // 拡張項目56
		public const string FIELD_PRODUCTEXTEND_EXTEND57 = "extend57";                              // 拡張項目57
		public const string FIELD_PRODUCTEXTEND_EXTEND58 = "extend58";                              // 拡張項目58
		public const string FIELD_PRODUCTEXTEND_EXTEND59 = "extend59";                              // 拡張項目59
		public const string FIELD_PRODUCTEXTEND_EXTEND60 = "extend60";                              // 拡張項目60
		public const string FIELD_PRODUCTEXTEND_EXTEND61 = "extend61";                              // 拡張項目61
		public const string FIELD_PRODUCTEXTEND_EXTEND62 = "extend62";                              // 拡張項目62
		public const string FIELD_PRODUCTEXTEND_EXTEND63 = "extend63";                              // 拡張項目63
		public const string FIELD_PRODUCTEXTEND_EXTEND64 = "extend64";                              // 拡張項目64
		public const string FIELD_PRODUCTEXTEND_EXTEND65 = "extend65";                              // 拡張項目65
		public const string FIELD_PRODUCTEXTEND_EXTEND66 = "extend66";                              // 拡張項目66
		public const string FIELD_PRODUCTEXTEND_EXTEND67 = "extend67";                              // 拡張項目67
		public const string FIELD_PRODUCTEXTEND_EXTEND68 = "extend68";                              // 拡張項目68
		public const string FIELD_PRODUCTEXTEND_EXTEND69 = "extend69";                              // 拡張項目69
		public const string FIELD_PRODUCTEXTEND_EXTEND70 = "extend70";                              // 拡張項目70
		public const string FIELD_PRODUCTEXTEND_EXTEND71 = "extend71";                              // 拡張項目71
		public const string FIELD_PRODUCTEXTEND_EXTEND72 = "extend72";                              // 拡張項目72
		public const string FIELD_PRODUCTEXTEND_EXTEND73 = "extend73";                              // 拡張項目73
		public const string FIELD_PRODUCTEXTEND_EXTEND74 = "extend74";                              // 拡張項目74
		public const string FIELD_PRODUCTEXTEND_EXTEND75 = "extend75";                              // 拡張項目75
		public const string FIELD_PRODUCTEXTEND_EXTEND76 = "extend76";                              // 拡張項目76
		public const string FIELD_PRODUCTEXTEND_EXTEND77 = "extend77";                              // 拡張項目77
		public const string FIELD_PRODUCTEXTEND_EXTEND78 = "extend78";                              // 拡張項目78
		public const string FIELD_PRODUCTEXTEND_EXTEND79 = "extend79";                              // 拡張項目79
		public const string FIELD_PRODUCTEXTEND_EXTEND80 = "extend80";                              // 拡張項目80
		public const string FIELD_PRODUCTEXTEND_EXTEND81 = "extend81";                              // 拡張項目81
		public const string FIELD_PRODUCTEXTEND_EXTEND82 = "extend82";                              // 拡張項目82
		public const string FIELD_PRODUCTEXTEND_EXTEND83 = "extend83";                              // 拡張項目83
		public const string FIELD_PRODUCTEXTEND_EXTEND84 = "extend84";                              // 拡張項目84
		public const string FIELD_PRODUCTEXTEND_EXTEND85 = "extend85";                              // 拡張項目85
		public const string FIELD_PRODUCTEXTEND_EXTEND86 = "extend86";                              // 拡張項目86
		public const string FIELD_PRODUCTEXTEND_EXTEND87 = "extend87";                              // 拡張項目87
		public const string FIELD_PRODUCTEXTEND_EXTEND88 = "extend88";                              // 拡張項目88
		public const string FIELD_PRODUCTEXTEND_EXTEND89 = "extend89";                              // 拡張項目89
		public const string FIELD_PRODUCTEXTEND_EXTEND90 = "extend90";                              // 拡張項目90
		public const string FIELD_PRODUCTEXTEND_EXTEND91 = "extend91";                              // 拡張項目91
		public const string FIELD_PRODUCTEXTEND_EXTEND92 = "extend92";                              // 拡張項目92
		public const string FIELD_PRODUCTEXTEND_EXTEND93 = "extend93";                              // 拡張項目93
		public const string FIELD_PRODUCTEXTEND_EXTEND94 = "extend94";                              // 拡張項目94
		public const string FIELD_PRODUCTEXTEND_EXTEND95 = "extend95";                              // 拡張項目95
		public const string FIELD_PRODUCTEXTEND_EXTEND96 = "extend96";                              // 拡張項目96
		public const string FIELD_PRODUCTEXTEND_EXTEND97 = "extend97";                              // 拡張項目97
		public const string FIELD_PRODUCTEXTEND_EXTEND98 = "extend98";                              // 拡張項目98
		public const string FIELD_PRODUCTEXTEND_EXTEND99 = "extend99";                              // 拡張項目99
		public const string FIELD_PRODUCTEXTEND_EXTEND100 = "extend100";                            // 拡張項目100
		public const string FIELD_PRODUCTEXTEND_EXTEND101 = "extend101";                            // 拡張項目101
		public const string FIELD_PRODUCTEXTEND_EXTEND102 = "extend102";                            // 拡張項目102
		public const string FIELD_PRODUCTEXTEND_EXTEND103 = "extend103";                            // 拡張項目103
		public const string FIELD_PRODUCTEXTEND_EXTEND104 = "extend104";                            // 拡張項目104
		public const string FIELD_PRODUCTEXTEND_EXTEND105 = "extend105";                            // 拡張項目105
		public const string FIELD_PRODUCTEXTEND_EXTEND106 = "extend106";                            // 拡張項目106
		public const string FIELD_PRODUCTEXTEND_EXTEND107 = "extend107";                            // 拡張項目107
		public const string FIELD_PRODUCTEXTEND_EXTEND108 = "extend108";                            // 拡張項目108
		public const string FIELD_PRODUCTEXTEND_EXTEND109 = "extend109";                            // 拡張項目109
		public const string FIELD_PRODUCTEXTEND_EXTEND110 = "extend110";                            // 拡張項目110
		public const string FIELD_PRODUCTEXTEND_EXTEND111 = "extend111";                            // 拡張項目111
		public const string FIELD_PRODUCTEXTEND_EXTEND112 = "extend112";                            // 拡張項目112
		public const string FIELD_PRODUCTEXTEND_EXTEND113 = "extend113";                            // 拡張項目113
		public const string FIELD_PRODUCTEXTEND_EXTEND114 = "extend114";                            // 拡張項目114
		public const string FIELD_PRODUCTEXTEND_EXTEND115 = "extend115";                            // 拡張項目115
		public const string FIELD_PRODUCTEXTEND_EXTEND116 = "extend116";                            // 拡張項目116
		public const string FIELD_PRODUCTEXTEND_EXTEND117 = "extend117";                            // 拡張項目117
		public const string FIELD_PRODUCTEXTEND_EXTEND118 = "extend118";                            // 拡張項目118
		public const string FIELD_PRODUCTEXTEND_EXTEND119 = "extend119";                            // 拡張項目119
		public const string FIELD_PRODUCTEXTEND_EXTEND120 = "extend120";                            // 拡張項目120
		public const string FIELD_PRODUCTEXTEND_EXTEND121 = "extend121";                            // 拡張項目121
		public const string FIELD_PRODUCTEXTEND_EXTEND122 = "extend122";                            // 拡張項目122
		public const string FIELD_PRODUCTEXTEND_EXTEND123 = "extend123";                            // 拡張項目123
		public const string FIELD_PRODUCTEXTEND_EXTEND124 = "extend124";                            // 拡張項目124
		public const string FIELD_PRODUCTEXTEND_EXTEND125 = "extend125";                            // 拡張項目125
		public const string FIELD_PRODUCTEXTEND_EXTEND126 = "extend126";                            // 拡張項目126
		public const string FIELD_PRODUCTEXTEND_EXTEND127 = "extend127";                            // 拡張項目127
		public const string FIELD_PRODUCTEXTEND_EXTEND128 = "extend128";                            // 拡張項目128
		public const string FIELD_PRODUCTEXTEND_EXTEND129 = "extend129";                            // 拡張項目129
		public const string FIELD_PRODUCTEXTEND_EXTEND130 = "extend130";                            // 拡張項目130
		public const string FIELD_PRODUCTEXTEND_EXTEND131 = "extend131";                            // 拡張項目131
		public const string FIELD_PRODUCTEXTEND_EXTEND132 = "extend132";                            // 拡張項目132
		public const string FIELD_PRODUCTEXTEND_EXTEND133 = "extend133";                            // 拡張項目133
		public const string FIELD_PRODUCTEXTEND_EXTEND134 = "extend134";                            // 拡張項目134
		public const string FIELD_PRODUCTEXTEND_EXTEND135 = "extend135";                            // 拡張項目135
		public const string FIELD_PRODUCTEXTEND_EXTEND136 = "extend136";                            // 拡張項目136
		public const string FIELD_PRODUCTEXTEND_EXTEND137 = "extend137";                            // 拡張項目137
		public const string FIELD_PRODUCTEXTEND_EXTEND138 = "extend138";                            // 拡張項目138
		public const string FIELD_PRODUCTEXTEND_EXTEND139 = "extend139";                            // 拡張項目139
		public const string FIELD_PRODUCTEXTEND_EXTEND140 = "extend140";                            // 拡張項目140
		public const string FIELD_PRODUCTEXTEND_DEL_FLG = "del_flg";                                // 削除フラグ
		public const string FIELD_PRODUCTEXTEND_DATE_CREATED = "date_created";                      // 作成日
		public const string FIELD_PRODUCTEXTEND_DATE_CHANGED = "date_changed";                      // 更新日
		public const string FIELD_PRODUCTEXTEND_LAST_CHANGED = "last_changed";                      // 最終更新者

		// 発注情報
		public const string TABLE_STOCKORDER = "w2_StockOrder";                                     // 発注情報
		public const string FIELD_STOCKORDER_SHOP_ID = "shop_id";                                   // 店舗ID
		public const string FIELD_STOCKORDER_STOCK_ORDER_ID = "stock_order_id";                     // 発注ID
		public const string FIELD_STOCKORDER_RELATION_ID = "relation_id";                           // 発注連携ID
		public const string FIELD_STOCKORDER_ORDER_ITEM_COUNT = "order_item_count";                 // 発注商品点数
		public const string FIELD_STOCKORDER_DELIVERY_ITEM_COUNT = "delivery_item_count";           // 入庫済商品点数
		public const string FIELD_STOCKORDER_ORDER_STATUS = "order_status";                         // 発注ステータス
		public const string FIELD_STOCKORDER_DELIVERY_STATUS = "delivery_status";                   // 入庫ステータス
		public const string FIELD_STOCKORDER_ORDER_DATE = "order_date";                             // 発注日
		public const string FIELD_STOCKORDER_ORDER_INPUT_DATE = "order_input_date";                 // 発注手配日
		public const string FIELD_STOCKORDER_DELIVERY_REPLY_DATE = "delivery_reply_date";           // 納期回答日
		public const string FIELD_STOCKORDER_DELIVERY_SCHE_DATE = "delivery_sche_date";             // 入庫予定日
		public const string FIELD_STOCKORDER_DELIVERY_DATE = "delivery_date";                       // 入庫日
		public const string FIELD_STOCKORDER_DELIVERY_INPUT_DATE = "delivery_input_date";           // 入庫登録日
		public const string FIELD_STOCKORDER_DATE_CREATED = "date_created";                         // 作成日
		public const string FIELD_STOCKORDER_DATE_CHANGED = "date_changed";                         // 更新日
		public const string FIELD_STOCKORDER_LAST_CHANGED = "last_changed";                         // 最終更新者

		// 発注商品情報
		public const string TABLE_STOCKORDERITEM = "w2_StockOrderItem";                             // 発注商品情報
		public const string FIELD_STOCKORDERITEM_SHOP_ID = "shop_id";                               // 店舗ID
		public const string FIELD_STOCKORDERITEM_STOCK_ORDER_ID = "stock_order_id";                 // 発注ID
		public const string FIELD_STOCKORDERITEM_PRODUCT_ID = "product_id";                         // 商品ID
		public const string FIELD_STOCKORDERITEM_VARIATION_ID = "variation_id";                     // 商品バリエーションID
		public const string FIELD_STOCKORDERITEM_ORDER_COUNT = "order_count";                       // 発注数
		public const string FIELD_STOCKORDERITEM_DELIVERY_COUNT = "delivery_count";                 // 入庫数
		public const string FIELD_STOCKORDERITEM_ORDER_STATUS = "order_status";                     // 発注ステータス
		public const string FIELD_STOCKORDERITEM_DELIVERY_STATUS = "delivery_status";               // 入庫ステータス
		public const string FIELD_STOCKORDERITEM_ORDER_DATE = "order_date";                         // 発注日
		public const string FIELD_STOCKORDERITEM_ORDER_INPUT_DATE = "order_input_date";             // 発注手配日
		public const string FIELD_STOCKORDERITEM_DELIVERY_REPLY_DATE = "delivery_reply_date";       // 納期回答日
		public const string FIELD_STOCKORDERITEM_DELIVERY_SCHE_DATE = "delivery_sche_date";         // 入庫予定日
		public const string FIELD_STOCKORDERITEM_DELIVERY_DATE = "delivery_date";                   // 入庫日
		public const string FIELD_STOCKORDERITEM_DELIVERY_INPUT_DATE = "delivery_input_date";       // 入庫登録日
		public const string FIELD_STOCKORDERITEM_DATE_CREATED = "date_created";                     // 作成日
		public const string FIELD_STOCKORDERITEM_DATE_CHANGED = "date_changed";                     // 更新日
		public const string FIELD_STOCKORDERITEM_LAST_CHANGED = "last_changed";                     // 最終更新者

		// 商品レビュー情報
		public const string TABLE_PRODUCTREVIEW = "w2_ProductReview";                               // 商品レビュー情報
		public const string FIELD_PRODUCTREVIEW_SHOP_ID = "shop_id";                                // 店舗ID
		public const string FIELD_PRODUCTREVIEW_PRODUCT_ID = "product_id";                          // 商品ID
		public const string FIELD_PRODUCTREVIEW_REVIEW_NO = "review_no";                            // レビュー番号
		public const string FIELD_PRODUCTREVIEW_USER_ID = "user_id";                                // ユーザID
		public const string FIELD_PRODUCTREVIEW_NICK_NAME = "nick_name";                            // ニックネーム
		public const string FIELD_PRODUCTREVIEW_REVIEW_RATING = "review_rating";                    // 評価
		public const string FIELD_PRODUCTREVIEW_REVIEW_TITLE = "review_title";                      // タイトル
		public const string FIELD_PRODUCTREVIEW_REVIEW_COMMENT = "review_comment";                  // コメント
		public const string FIELD_PRODUCTREVIEW_OPEN_FLG = "open_flg";                              // 公開フラグ
		public const string FIELD_PRODUCTREVIEW_CHECKED_FLG = "checked_flg";                        // 管理者チェックフラグ
		public const string FIELD_PRODUCTREVIEW_DATE_OPENED = "date_opened";                        // 公開日
		public const string FIELD_PRODUCTREVIEW_DATE_CHECKED = "date_checked";                      // 管理者チェック日
		public const string FIELD_PRODUCTREVIEW_DEL_FLG = "del_flg";                                // 削除フラグ
		public const string FIELD_PRODUCTREVIEW_DATE_CREATED = "date_created";                      // 作成日
		public const string FIELD_PRODUCTREVIEW_DATE_CHANGED = "date_changed";                      // 更新日
		public const string FIELD_PRODUCTREVIEW_LAST_CHANGED = "last_changed";                      // 最終更新者
		/// <summary>レビュー投稿ポイント付与フラグ</summary>
		public const string FIELD_PRODUCTREVIEW_REVIEW_REWARD_POINT_FLG = "review_reward_point_flg";

		// お気に入り商品マスタ
		public const string TABLE_FAVORITE = "w2_Favorite";                                         // お気に入り商品マスタ
		public const string FIELD_FAVORITE_USER_ID = "user_id";                                     // ユーザID
		public const string FIELD_FAVORITE_SHOP_ID = "shop_id";                                     // 店舗ID
		public const string FIELD_FAVORITE_PRODUCT_ID = "product_id";                               // 商品ID
		public const string FIELD_FAVORITE_DEL_FLG = "del_flg";                                     // 削除フラグ
		public const string FIELD_FAVORITE_DATE_CREATED = "date_created";                           // 作成日
		public const string FIELD_FAVORITE_DATE_CHANGED = "date_changed";                           // 更新日
		public const string FIELD_FAVORITE_VARIATION_ID = "variation_id";                           // バリエーションID
		public const string FIELD_FAVORITE_PRODUCT_IMAGE = "variation_image_head";                  // 商品画像URL
		public const string FIELD_FAVORITE_DELETE_FLG = "0";                                        // 削除フラグ
		public const string FIELD_FAVORITE_ALERTMAIL_SEND_FLG = "stock_alert_mail_send_flg";        // アラートフラグ

		// 買い物かごマスタ
		public const string TABLE_CART = "w2_Cart";                                                 // 買い物かごマスタ
		public const string FIELD_CART_CART_ID = "cart_id";                                         // カートID
		public const string FIELD_CART_CART_ITEM_NO = "cart_item_no";                               // 枝番
		public const string FIELD_CART_USER_ID = "user_id";                                         // ユーザID
		public const string FIELD_CART_SHOP_ID = "shop_id";                                         // 店舗ID
		public const string FIELD_CART_SUPPLIER_ID = "supplier_id";                                 // サプライヤID
		public const string FIELD_CART_PRODUCT_ID = "product_id";                                   // 商品ID
		public const string FIELD_CART_VARIATION_ID = "variation_id";                               // 商品バリエーションID
		public const string FIELD_CART_PRODUCT_COUNT = "product_count";                             // 商品注文数
		public const string FIELD_CART_PRODUCT_SET_ID = "product_set_id";                           // 商品セットID
		public const string FIELD_CART_PRODUCT_SET_NO = "product_set_no";                           // 商品セット枝番
		public const string FIELD_CART_PRODUCT_SET_COUNT = "product_set_count";                     // 商品セット注文数
		public const string FIELD_CART_CART_DIV_TYPE1 = "cart_div_type1";                           // カート分割基準1
		public const string FIELD_CART_CART_DIV_TYPE2 = "cart_div_type2";                           // カート分割基準2
		public const string FIELD_CART_CART_DIV_TYPE3 = "cart_div_type3";                           // カート分割基準3
		public const string FIELD_CART_KBN1 = "kbn1";                                               // 区分１
		public const string FIELD_CART_KBN2 = "kbn2";                                               // 区分２
		public const string FIELD_CART_KBN3 = "kbn3";                                               // 区分３
		public const string FIELD_CART_PRODUCTSALE_ID = "productsale_id";                           // 商品セールID
		public const string FIELD_CART_FIXED_PURCHASE_FLG = "fixed_purchase_flg";                   // 定期購入フラグ
		public const string FIELD_CART_MOBILE_KBN = "mobile_kbn";                                   // モバイル区分
		public const string FIELD_CART_DEL_FLG = "del_flg";                                         // 削除フラグ
		public const string FIELD_CART_DATE_CREATED = "date_created";                               // 作成日
		public const string FIELD_CART_DATE_CHANGED = "date_changed";                               // 更新日
		public const string FIELD_CART_PRODUCT_OPTION_TEXTS = "product_option_texts";               // 商品付帯情報選択値
		public const string FIELD_CART_GIFT_ORDER_FLG = "gift_order_flg";                           // ギフト購入フラグ
		public const string FIELD_CART_NOVELTY_ID = "novelty_id";	                                // ノベルティID
		public const string FIELD_CART_RECOMMEND_ID = "recommend_id";                               // レコメンドID

		// 注文情報
		public const string TABLE_ORDER = "w2_Order";                                               // 注文情報
		public const string FIELD_ORDER_ORDER_ID = "order_id";                                      // 注文ID
		public const string FIELD_ORDER_ORDER_ID_ORG = "order_id_org";                              // 元注文ID
		public const string FIELD_ORDER_ORDER_GROUP_ID = "order_group_id";                          // 注文グループID
		public const string FIELD_ORDER_ORDER_NO = "order_no";                                      // 注文番号
		public const string FIELD_ORDER_BUNDLE_CHILD_ORDER_IDS = "bundle_child_order_ids";          // 同梱子注文ID列
		public const string FIELD_ORDER_BUNDLE_PARENT_ORDER_ID = "bundle_parent_order_id";          // 同梱親注文ID
		public const string FIELD_ORDER_BUNDLE_ORDER_BAK = "bundle_order_bak";                      // 同梱済注文XML
		public const string FIELD_ORDER_USER_ID = "user_id";                                        // ユーザID
		public const string FIELD_ORDER_SHOP_ID = "shop_id";                                        // 店舗ID
		public const string FIELD_ORDER_SUPPLIER_ID = "supplier_id";                                // サプライヤID
		public const string FIELD_ORDER_ORDER_KBN = "order_kbn";                                    // 注文区分
		public const string FIELD_ORDER_MALL_ID = "mall_id";                                        // モールID
		public const string FIELD_ORDER_ORDER_PAYMENT_KBN = "order_payment_kbn";                    // 支払区分
		public const string FIELD_ORDER_ORDER_STATUS = "order_status";                              // 注文ステータス
		public const string FIELD_ORDER_ORDER_DATE = "order_date";                                  // 注文日時
		public const string FIELD_ORDER_ORDER_RECOGNITION_DATE = "order_recognition_date";          // 受注承認日時
		public const string FIELD_ORDER_ORDER_STOCKRESERVED_STATUS = "order_stockreserved_status";  // 在庫引当ステータス
		public const string FIELD_ORDER_ORDER_STOCKRESERVED_DATE = "order_stockreserved_date";      // 在庫引当日時
		public const string FIELD_ORDER_ORDER_SHIPPING_DATE = "order_shipping_date";                // 出荷手配日時
		public const string FIELD_ORDER_ORDER_SHIPPED_STATUS = "order_shipped_status";              // 出荷ステータス
		public const string FIELD_ORDER_ORDER_SHIPPED_DATE = "order_shipped_date";                  // 出荷完了日時
		public const string FIELD_ORDER_ORDER_DELIVERING_DATE = "order_delivering_date";            // 配送完了日時
		public const string FIELD_ORDER_ORDER_CANCEL_DATE = "order_cancel_date";                    // キャンセル日時
		public const string FIELD_ORDER_ORDER_RETURN_DATE = "order_return_date";                    // 返品日時
		public const string FIELD_ORDER_ORDER_PAYMENT_STATUS = "order_payment_status";              // 入金ステータス
		public const string FIELD_ORDER_ORDER_PAYMENT_DATE = "order_payment_date";                  // 入金確認日時
		public const string FIELD_ORDER_DEMAND_STATUS = "demand_status";                            // 督促ステータス
		public const string FIELD_ORDER_DEMAND_DATE = "demand_date";                                // 督促日時
		public const string FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS = "order_return_exchange_status";// 返品交換ステータス
		public const string FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE = "order_return_exchange_receipt_date";// 返品交換受付日時
		public const string FIELD_ORDER_ORDER_RETURN_EXCHANGE_ARRIVAL_DATE = "order_return_exchange_arrival_date";// 返品交換商品到着日時
		public const string FIELD_ORDER_ORDER_RETURN_EXCHANGE_COMPLETE_DATE = "order_return_exchange_complete_date";// 返品交換完了日時
		public const string FIELD_ORDER_ORDER_REPAYMENT_STATUS = "order_repayment_status";          // 返金ステータス
		public const string FIELD_ORDER_ORDER_REPAYMENT_DATE = "order_repayment_date";              // 返金日時
		public const string FIELD_ORDER_ORDER_ITEM_COUNT = "order_item_count";                      // 注文アイテム数
		public const string FIELD_ORDER_ORDER_PRODUCT_COUNT = "order_product_count";                // 注文商品数
		public const string FIELD_ORDER_ORDER_PRICE_SUBTOTAL = "order_price_subtotal";              // 小計
		public const string FIELD_ORDER_ORDER_PRICE_PACK = "order_price_pack";                      // 荷造金額
		public const string FIELD_ORDER_ORDER_PRICE_TAX = "order_price_tax";                        // 税金合計
		public const string FIELD_ORDER_ORDER_PRICE_SHIPPING = "order_price_shipping";              // 配送料
		public const string FIELD_ORDER_ORDER_PRICE_EXCHANGE = "order_price_exchange";              // 代引手数料
		public const string FIELD_ORDER_ORDER_PRICE_REGULATION = "order_price_regulation";          // 調整金額
		public const string FIELD_ORDER_ORDER_PRICE_REPAYMENT = "order_price_repayment";            // 返金金額
		public const string FIELD_ORDER_ORDER_PRICE_TOTAL = "order_price_total";                    // 支払金額合計
		public const string FIELD_ORDER_ORDER_DISCOUNT_SET_PRICE = "order_discount_set_price";      // セット値引金額
		public const string FIELD_ORDER_ORDER_POINT_USE = "order_point_use";                        // 利用ポイント数
		public const string FIELD_ORDER_ORDER_POINT_USE_YEN = "order_point_use_yen";                // ポイント利用額
		public const string FIELD_ORDER_ORDER_POINT_ADD = "order_point_add";                        // 付与ポイント
		public const string FIELD_ORDER_ORDER_POINT_RATE = "order_point_rate";                      // ポイント調整率
		public const string FIELD_ORDER_ORDER_COUPON_USE = "order_coupon_use";                      // クーポン割引額
		public const string FIELD_ORDER_CARD_KBN = "card_kbn";                                      // 決済カード区分
		public const string FIELD_ORDER_CARD_INSTRUMENTS = "card_instruments";                      // 決済カード支払回数文言
		public const string FIELD_ORDER_CARD_TRAN_ID = "card_tran_id";                              // 決済カード取引ID
		public const string FIELD_ORDER_SHIPPING_ID = "shipping_id";                                // 配送種別ID
		public const string FIELD_ORDER_FIXED_PURCHASE_ID = "fixed_purchase_id";                    // 定期購入ID
		public const string FIELD_ORDER_ADVCODE_FIRST = "advcode_first";                            // 初回広告コード
		public const string FIELD_ORDER_ADVCODE_NEW = "advcode_new";                                // 最新広告コード
		public const string FIELD_ORDER_SHIPPED_CHANGED_KBN = "shipped_changed_kbn";                // 出荷後変更区分
		public const string FIELD_ORDER_RETURN_EXCHANGE_KBN = "return_exchange_kbn";                // 返品交換区分
		public const string FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN = "return_exchange_reason_kbn";  // 返品交換都合区分
		public const string FIELD_ORDER_ATTRIBUTE1 = "attribute1";                                  // 注文拡張項目1
		public const string FIELD_ORDER_ATTRIBUTE2 = "attribute2";                                  // 注文拡張項目2
		public const string FIELD_ORDER_ATTRIBUTE3 = "attribute3";                                  // 注文拡張項目3
		public const string FIELD_ORDER_ATTRIBUTE4 = "attribute4";                                  // 注文拡張項目4
		public const string FIELD_ORDER_ATTRIBUTE5 = "attribute5";                                  // 注文拡張項目5
		public const string FIELD_ORDER_ATTRIBUTE6 = "attribute6";                                  // 注文拡張項目6
		public const string FIELD_ORDER_ATTRIBUTE7 = "attribute7";                                  // 注文拡張項目7
		public const string FIELD_ORDER_ATTRIBUTE8 = "attribute8";                                  // 注文拡張項目8
		public const string FIELD_ORDER_ATTRIBUTE9 = "attribute9";                                  // 注文拡張項目9
		public const string FIELD_ORDER_ATTRIBUTE10 = "attribute10";                                // 注文拡張項目10
		public const string FIELD_ORDER_EXTEND_STATUS1 = "extend_status1";                          // 拡張ステータス１
		public const string FIELD_ORDER_EXTEND_STATUS_DATE1 = "extend_status_date1";                // 拡張ステータス更新日時１
		public const string FIELD_ORDER_EXTEND_STATUS2 = "extend_status2";                          // 拡張ステータス２
		public const string FIELD_ORDER_EXTEND_STATUS_DATE2 = "extend_status_date2";                // 拡張ステータス更新日時２
		public const string FIELD_ORDER_EXTEND_STATUS3 = "extend_status3";                          // 拡張ステータス３
		public const string FIELD_ORDER_EXTEND_STATUS_DATE3 = "extend_status_date3";                // 拡張ステータス更新日時３
		public const string FIELD_ORDER_EXTEND_STATUS4 = "extend_status4";                          // 拡張ステータス４
		public const string FIELD_ORDER_EXTEND_STATUS_DATE4 = "extend_status_date4";                // 拡張ステータス更新日時４
		public const string FIELD_ORDER_EXTEND_STATUS5 = "extend_status5";                          // 拡張ステータス５
		public const string FIELD_ORDER_EXTEND_STATUS_DATE5 = "extend_status_date5";                // 拡張ステータス更新日時５
		public const string FIELD_ORDER_EXTEND_STATUS6 = "extend_status6";                          // 拡張ステータス６
		public const string FIELD_ORDER_EXTEND_STATUS_DATE6 = "extend_status_date6";                // 拡張ステータス更新日時６
		public const string FIELD_ORDER_EXTEND_STATUS7 = "extend_status7";                          // 拡張ステータス７
		public const string FIELD_ORDER_EXTEND_STATUS_DATE7 = "extend_status_date7";                // 拡張ステータス更新日時７
		public const string FIELD_ORDER_EXTEND_STATUS8 = "extend_status8";                          // 拡張ステータス８
		public const string FIELD_ORDER_EXTEND_STATUS_DATE8 = "extend_status_date8";                // 拡張ステータス更新日時８
		public const string FIELD_ORDER_EXTEND_STATUS9 = "extend_status9";                          // 拡張ステータス９
		public const string FIELD_ORDER_EXTEND_STATUS_DATE9 = "extend_status_date9";                // 拡張ステータス更新日時９
		public const string FIELD_ORDER_EXTEND_STATUS10 = "extend_status10";                        // 拡張ステータス１０
		public const string FIELD_ORDER_EXTEND_STATUS_DATE10 = "extend_status_date10";              // 拡張ステータス更新日時１０
		public const string FIELD_ORDER_CAREER_ID = "career_id";                                    // キャリアID
		public const string FIELD_ORDER_MOBILE_UID = "mobile_uid";                                  // モバイルUID
		public const string FIELD_ORDER_REMOTE_ADDR = "remote_addr";                                // リモートIPアドレス
		public const string FIELD_ORDER_MEMO = "memo";                                              // メモ
		public const string FIELD_ORDER_WRAPPING_MEMO = "wrapping_memo";                            // 熨斗メモ
		public const string FIELD_ORDER_PAYMENT_MEMO = "payment_memo";                              // 決済連携メモ
		public const string FIELD_ORDER_MANAGEMENT_MEMO = "management_memo";                        // 管理メモ
		public const string FIELD_ORDER_RELATION_MEMO = "relation_memo";                            // 外部連携メモ
		public const string FIELD_ORDER_RETURN_EXCHANGE_REASON_MEMO = "return_exchange_reason_memo";// 返品交換理由メモ
		public const string FIELD_ORDER_REGULATION_MEMO = "regulation_memo";                        // 調整金額メモ
		public const string FIELD_ORDER_REPAYMENT_MEMO = "repayment_memo";                          // 返金メモ
		public const string FIELD_ORDER_DEL_FLG = "del_flg";                                        // 削除フラグ
		public const string FIELD_ORDER_DATE_CREATED = "date_created";                              // 作成日
		public const string FIELD_ORDER_DATE_CHANGED = "date_changed";                              // 更新日
		public const string FIELD_ORDER_LAST_CHANGED = "last_changed";                              // 最終更新者
		public const string FIELD_ORDER_MEMBER_RANK_DISCOUNT_PRICE = "member_rank_discount_price";  // 会員ランク割引額
		public const string FIELD_ORDER_MEMBER_RANK_ID = "member_rank_id";                          // 注文時会員ランク
		public const string FIELD_ORDER_CREDIT_BRANCH_NO = "credit_branch_no";                      // クレジットカード枝番
		public const string FIELD_ORDER_AFFILIATE_SESSION_NAME1 = "affiliate_session_name1";        // アフィリエイトセッション変数名1
		public const string FIELD_ORDER_AFFILIATE_SESSION_VALUE1 = "affiliate_session_value1";      // アフィリエイトセッション値1
		public const string FIELD_ORDER_AFFILIATE_SESSION_NAME2 = "affiliate_session_name2";        // アフィリエイトセッション変数名2
		public const string FIELD_ORDER_AFFILIATE_SESSION_VALUE2 = "affiliate_session_value2";      // アフィリエイトセッション値2
		public const string FIELD_ORDER_USER_AGENT = "user_agent";                                  // ユーザーエージェント
		public const string FIELD_ORDER_GIFT_FLG = "gift_flg";                                      // ギフト購入フラグ
		public const string FIELD_ORDER_DIGITAL_CONTENTS_FLG = "digital_contents_flg";              // デジタルコンテンツ購入フラグ
		public const string FIELD_ORDER_CARD_3DSECURE_TRAN_ID = "card_3dsecure_tran_id";            // 3DセキュアトランザクションID
		public const string FIELD_ORDER_CARD_3DSECURE_AUTH_URL = "card_3dsecure_auth_url";          // 3Dセキュア認証URL
		public const string FIELD_ORDER_CARD_3DSECURE_AUTH_KEY = "card_3dsecure_auth_key";          // 3Dセキュア認証キー
		public const string FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG = "shipping_price_separate_estimates_flg";// 配送料の別見積もりの利用フラグ
		public const string FIELD_ORDER_ORDER_TAX_INCLUDED_FLG = "order_tax_included_flg";          // 税込フラグ(使用しない)
		public const string FIELD_ORDER_ORDER_TAX_RATE = "order_tax_rate";                          // 税率(使用しない)
		public const string FIELD_ORDER_ORDER_TAX_ROUND_TYPE = "order_tax_round_type";              // 税計算方法(使用しない)
		public const string FIELD_ORDER_EXTEND_STATUS11 = "extend_status11";                        // 拡張ステータス11
		public const string FIELD_ORDER_EXTEND_STATUS_DATE11 = "extend_status_date11";              // 拡張ステータス更新日時11
		public const string FIELD_ORDER_EXTEND_STATUS12 = "extend_status12";                        // 拡張ステータス12
		public const string FIELD_ORDER_EXTEND_STATUS_DATE12 = "extend_status_date12";              // 拡張ステータス更新日時12
		public const string FIELD_ORDER_EXTEND_STATUS13 = "extend_status13";                        // 拡張ステータス13
		public const string FIELD_ORDER_EXTEND_STATUS_DATE13 = "extend_status_date13";              // 拡張ステータス更新日時13
		public const string FIELD_ORDER_EXTEND_STATUS14 = "extend_status14";                        // 拡張ステータス14
		public const string FIELD_ORDER_EXTEND_STATUS_DATE14 = "extend_status_date14";              // 拡張ステータス更新日時14
		public const string FIELD_ORDER_EXTEND_STATUS15 = "extend_status15";                        // 拡張ステータス15
		public const string FIELD_ORDER_EXTEND_STATUS_DATE15 = "extend_status_date15";              // 拡張ステータス更新日時15
		public const string FIELD_ORDER_EXTEND_STATUS16 = "extend_status16";                        // 拡張ステータス16
		public const string FIELD_ORDER_EXTEND_STATUS_DATE16 = "extend_status_date16";              // 拡張ステータス更新日時16
		public const string FIELD_ORDER_EXTEND_STATUS17 = "extend_status17";                        // 拡張ステータス17
		public const string FIELD_ORDER_EXTEND_STATUS_DATE17 = "extend_status_date17";              // 拡張ステータス更新日時17
		public const string FIELD_ORDER_EXTEND_STATUS18 = "extend_status18";                        // 拡張ステータス18
		public const string FIELD_ORDER_EXTEND_STATUS_DATE18 = "extend_status_date18";              // 拡張ステータス更新日時18
		public const string FIELD_ORDER_EXTEND_STATUS19 = "extend_status19";                        // 拡張ステータス19
		public const string FIELD_ORDER_EXTEND_STATUS_DATE19 = "extend_status_date19";              // 拡張ステータス更新日時19
		public const string FIELD_ORDER_EXTEND_STATUS20 = "extend_status20";                        // 拡張ステータス20
		public const string FIELD_ORDER_EXTEND_STATUS_DATE20 = "extend_status_date20";              // 拡張ステータス更新日時20
		public const string FIELD_ORDER_EXTEND_STATUS21 = "extend_status21";                        // 拡張ステータス21
		public const string FIELD_ORDER_EXTEND_STATUS_DATE21 = "extend_status_date21";              // 拡張ステータス更新日時21
		public const string FIELD_ORDER_EXTEND_STATUS22 = "extend_status22";                        // 拡張ステータス22
		public const string FIELD_ORDER_EXTEND_STATUS_DATE22 = "extend_status_date22";              // 拡張ステータス更新日時22
		public const string FIELD_ORDER_EXTEND_STATUS23 = "extend_status23";                        // 拡張ステータス23
		public const string FIELD_ORDER_EXTEND_STATUS_DATE23 = "extend_status_date23";              // 拡張ステータス更新日時23
		public const string FIELD_ORDER_EXTEND_STATUS24 = "extend_status24";                        // 拡張ステータス24
		public const string FIELD_ORDER_EXTEND_STATUS_DATE24 = "extend_status_date24";              // 拡張ステータス更新日時24
		public const string FIELD_ORDER_EXTEND_STATUS25 = "extend_status25";                        // 拡張ステータス25
		public const string FIELD_ORDER_EXTEND_STATUS_DATE25 = "extend_status_date25";              // 拡張ステータス更新日時25
		public const string FIELD_ORDER_EXTEND_STATUS26 = "extend_status26";                        // 拡張ステータス26
		public const string FIELD_ORDER_EXTEND_STATUS_DATE26 = "extend_status_date26";              // 拡張ステータス更新日時26
		public const string FIELD_ORDER_EXTEND_STATUS27 = "extend_status27";                        // 拡張ステータス27
		public const string FIELD_ORDER_EXTEND_STATUS_DATE27 = "extend_status_date27";              // 拡張ステータス更新日時27
		public const string FIELD_ORDER_EXTEND_STATUS28 = "extend_status28";                        // 拡張ステータス28
		public const string FIELD_ORDER_EXTEND_STATUS_DATE28 = "extend_status_date28";              // 拡張ステータス更新日時28
		public const string FIELD_ORDER_EXTEND_STATUS29 = "extend_status29";                        // 拡張ステータス29
		public const string FIELD_ORDER_EXTEND_STATUS_DATE29 = "extend_status_date29";              // 拡張ステータス更新日時29
		public const string FIELD_ORDER_EXTEND_STATUS30 = "extend_status30";                        // 拡張ステータス30
		public const string FIELD_ORDER_EXTEND_STATUS_DATE30 = "extend_status_date30";              // 拡張ステータス更新日時30
		public const string FIELD_ORDER_CARD_INSTALLMENTS_CODE = "card_installments_code";          // カード支払い回数コード
		public const string FIELD_ORDER_SETPROMOTION_PRODUCT_DISCOUNT_AMOUNT = "setpromotion_product_discount_amount";// セットプロモーション商品割引額
		public const string FIELD_ORDER_SETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT = "setpromotion_shipping_charge_discount_amount";// セットプロモーション配送料割引額
		public const string FIELD_ORDER_SETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT = "setpromotion_payment_charge_discount_amount";// セットプロモーション決済手数料割引額
		public const string FIELD_ORDER_ONLINE_PAYMENT_STATUS = "online_payment_status";            // オンライン決済ステータス
		public const string FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT = "fixed_purchase_order_count";  // 定期購入回数(注文時点)
		public const string FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT = "fixed_purchase_shipped_count";// 定期購入回数(出荷時点)
		public const string FIELD_ORDER_FIXED_PURCHASE_DISCOUNT_PRICE = "fixed_purchase_discount_price";// 定期購入割引額
		public const string FIELD_ORDER_PAYMENT_ORDER_ID = "payment_order_id";                      // 決済注文ID
		public const string FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT = "fixed_purchase_member_discount_amount";// 定期会員割引額
		public const string FIELD_ORDER_LAST_BILLED_AMOUNT = "last_billed_amount";                  // 最終請求金額
		public const string FIELD_ORDER_EXTERNAL_PAYMENT_STATUS = "external_payment_status";        // 外部決済ステータス
		public const string FIELD_ORDER_EXTERNAL_PAYMENT_ERROR_MESSAGE = "external_payment_error_message";// 外部決済エラーメッセージ
		public const string FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE = "external_payment_auth_date";  // 外部決済与信日時
		public const string FIELD_ORDER_EXTEND_STATUS31 = "extend_status31";                        // 拡張ステータス31
		public const string FIELD_ORDER_EXTEND_STATUS32 = "extend_status32";                        // 拡張ステータス32
		public const string FIELD_ORDER_EXTEND_STATUS33 = "extend_status33";                        // 拡張ステータス33
		public const string FIELD_ORDER_EXTEND_STATUS34 = "extend_status34";                        // 拡張ステータス34
		public const string FIELD_ORDER_EXTEND_STATUS35 = "extend_status35";                        // 拡張ステータス35
		public const string FIELD_ORDER_EXTEND_STATUS36 = "extend_status36";                        // 拡張ステータス36
		public const string FIELD_ORDER_EXTEND_STATUS37 = "extend_status37";                        // 拡張ステータス37
		public const string FIELD_ORDER_EXTEND_STATUS38 = "extend_status38";                        // 拡張ステータス38
		public const string FIELD_ORDER_EXTEND_STATUS39 = "extend_status39";                        // 拡張ステータス39
		public const string FIELD_ORDER_EXTEND_STATUS40 = "extend_status40";                        // 拡張ステータス40
		public const string FIELD_ORDER_EXTEND_STATUS_DATE31 = "extend_status_date31";              // 拡張ステータス更新日時31
		public const string FIELD_ORDER_EXTEND_STATUS_DATE32 = "extend_status_date32";              // 拡張ステータス更新日時32
		public const string FIELD_ORDER_EXTEND_STATUS_DATE33 = "extend_status_date33";              // 拡張ステータス更新日時33
		public const string FIELD_ORDER_EXTEND_STATUS_DATE34 = "extend_status_date34";              // 拡張ステータス更新日時34
		public const string FIELD_ORDER_EXTEND_STATUS_DATE35 = "extend_status_date35";              // 拡張ステータス更新日時35
		public const string FIELD_ORDER_EXTEND_STATUS_DATE36 = "extend_status_date36";              // 拡張ステータス更新日時36
		public const string FIELD_ORDER_EXTEND_STATUS_DATE37 = "extend_status_date37";              // 拡張ステータス更新日時37
		public const string FIELD_ORDER_EXTEND_STATUS_DATE38 = "extend_status_date38";              // 拡張ステータス更新日時38
		public const string FIELD_ORDER_EXTEND_STATUS_DATE39 = "extend_status_date39";              // 拡張ステータス更新日時39
		public const string FIELD_ORDER_EXTEND_STATUS_DATE40 = "extend_status_date40";              // 拡張ステータス更新日時40
		public const string FIELD_ORDER_LAST_ORDER_POINT_USE = "last_order_point_use";              // 最終利用ポイント数
		public const string FIELD_ORDER_LAST_ORDER_POINT_USE_YEN = "last_order_point_use_yen";      // 最終ポイント利用額
		public const string FIELD_ORDER_EXTERNAL_ORDER_ID = "external_order_id";                    // 外部連携受注ID
		public const string FIELD_ORDER_EXTERNAL_IMPORT_STATUS = "external_import_status";          // 外部連携取込ステータス
		public const string FIELD_ORDER_COMBINED_ORG_ORDER_IDS = "combined_org_order_ids";          // 注文同梱元注文ID
		public const string FIELD_ORDER_LAST_AUTH_FLG = "last_auth_flg";                            // 最終与信フラグ
		public const string FIELD_ORDER_MALL_LINK_STATUS = "mall_link_status";                      // モール連携ステータス
		public const string FIELD_ORDER_ORDER_PRICE_SUBTOTAL_TAX = "order_price_subtotal_tax";      // 商品小計税額
		public const string FIELD_ORDER_ORDER_PRICE_SHIPPING_TAX = "order_price_shipping_tax";      // 配送料税額
		public const string FIELD_ORDER_ORDER_PRICE_EXCHANGE_TAX = "order_price_exchange_tax";      // 決済手数料税額
		public const string FIELD_ORDER_SETTLEMENT_CURRENCY = "settlement_currency";                // 決済通貨
		public const string FIELD_ORDER_SETTLEMENT_RATE = "settlement_rate";                        // 決済レート
		public const string FIELD_ORDER_SETTLEMENT_AMOUNT = "settlement_amount";                    // 決済金額
		public const string FIELD_ORDER_SHIPPING_MEMO = "shipping_memo";                            // 配送メモ
		public const string FIELD_ORDER_EXTERNAL_PAYMENT_COOPERATION_LOG = "external_payment_cooperation_log";// 外部連携決済ログ
		public const string FIELD_ORDER_SHIPPING_TAX_RATE = "shipping_tax_rate";                    // 配送料税率
		public const string FIELD_ORDER_PAYMENT_TAX_RATE = "payment_tax_rate";                      // 決済手数料税率
		public const string FIELD_ORDER_ORDER_COUNT_ORDER = "order_count_order";                    // 購入回数（注文基準）
		public const string FIELD_ORDER_INVOICE_BUNDLE_FLG = "invoice_bundle_flg";                  // 請求書同梱フラグ
		public const string FIELD_ORDER_INFLOW_CONTENTS_ID = "inflow_contents_id";                  // 流入コンテンツID
		public const string FIELD_ORDER_INFLOW_CONTENTS_TYPE = "inflow_contents_type";              // 流入コンテンツタイプ
		public const string FIELD_ORDER_RECEIPT_FLG = "receipt_flg";                                // 領収書希望フラグ
		public const string FIELD_ORDER_RECEIPT_OUTPUT_FLG = "receipt_output_flg";                  // 領収書出力フラグ
		public const string FIELD_ORDER_RECEIPT_ADDRESS = "receipt_address";                        // 宛名
		public const string FIELD_ORDER_RECEIPT_PROVISO = "receipt_proviso";                        // 但し書き
		public const string FIELD_ORDER_DELIVERY_TRAN_ID = "delivery_tran_id";                      // 物流取引ID
		public const string FIELD_ORDER_ONLINE_DELIVERY_STATUS = "online_delivery_status";          // オンライン物流ステータス
		public const string FIELD_ORDER_EXTERNAL_PAYMENT_TYPE = "external_payment_type";            // 外部決済タイプ
		public const string FIELD_ORDER_LOGI_COOPERATION_STATUS = "logi_cooperation_status";        // 物流連携ステータス
		public const string FIELD_ORDER_EXTEND_STATUS41 = "extend_status41";                        // 拡張ステータス41
		public const string FIELD_ORDER_EXTEND_STATUS42 = "extend_status42";                        // 拡張ステータス42
		public const string FIELD_ORDER_EXTEND_STATUS43 = "extend_status43";                        // 拡張ステータス43
		public const string FIELD_ORDER_EXTEND_STATUS44 = "extend_status44";                        // 拡張ステータス44
		public const string FIELD_ORDER_EXTEND_STATUS45 = "extend_status45";                        // 拡張ステータス45
		public const string FIELD_ORDER_EXTEND_STATUS46 = "extend_status46";                        // 拡張ステータス46
		public const string FIELD_ORDER_EXTEND_STATUS47 = "extend_status47";                        // 拡張ステータス47
		public const string FIELD_ORDER_EXTEND_STATUS48 = "extend_status48";                        // 拡張ステータス48
		public const string FIELD_ORDER_EXTEND_STATUS49 = "extend_status49";                        // 拡張ステータス49
		public const string FIELD_ORDER_EXTEND_STATUS50 = "extend_status50";                        // 拡張ステータス50
		public const string FIELD_ORDER_EXTEND_STATUS_DATE41 = "extend_status_date41";              // 拡張ステータス更新日時41
		public const string FIELD_ORDER_EXTEND_STATUS_DATE42 = "extend_status_date42";              // 拡張ステータス更新日時42
		public const string FIELD_ORDER_EXTEND_STATUS_DATE43 = "extend_status_date43";              // 拡張ステータス更新日時43
		public const string FIELD_ORDER_EXTEND_STATUS_DATE44 = "extend_status_date44";              // 拡張ステータス更新日時44
		public const string FIELD_ORDER_EXTEND_STATUS_DATE45 = "extend_status_date45";              // 拡張ステータス更新日時45
		public const string FIELD_ORDER_EXTEND_STATUS_DATE46 = "extend_status_date46";              // 拡張ステータス更新日時46
		public const string FIELD_ORDER_EXTEND_STATUS_DATE47 = "extend_status_date47";              // 拡張ステータス更新日時47
		public const string FIELD_ORDER_EXTEND_STATUS_DATE48 = "extend_status_date48";              // 拡張ステータス更新日時48
		public const string FIELD_ORDER_EXTEND_STATUS_DATE49 = "extend_status_date49";              // 拡張ステータス更新日時49
		public const string FIELD_ORDER_EXTEND_STATUS_DATE50 = "extend_status_date50";              // 拡張ステータス更新日時50
		public const string FIELD_ORDER_CARD_TRAN_PASS = "card_tran_pass";                          // 決済カード取引PASS
		public const string FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID = "subscription_box_course_id";  // 頒布会コースID
		public const string FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT = "subscription_box_fixed_amount";// 定額価格
		public const string FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT = "order_subscription_box_order_count";// 頒布会注文回数
		public const string FIELD_ORDER_ADVCODE_WORKFLOW = "advcode_workflow";						// AdvCode Workflow

		// 注文テーブルに付随して作成した定数
		public const string FIELD_ORDER_EXTEND_STATUS_BASENAME = "extend_status";                   // 拡張ステータスベース名
		public const string FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME = "extend_status_date";         // 拡張ステータス更新日時ベース名
		public const string FIELD_ORDER_EXTEND_STATUS_VALUE = "extend_status_value";                // 拡張ステータス値
		public const string FIELD_ORDER_AMAZON_PAY_CV2_CHECKOUTSESSION_ID = "amazonpay_cv2_checkoutsession_id"; // AmazonPayCv2チェックアウトセッションID
		/// <summary>店舗受取ステータス</summary>
		public const string FIELD_ORDER_STOREPICKUP_STATUS = "storepickup_status";
		/// <summary>店舗到着日時</summary>
		public const string FIELD_ORDER_STOREPICKUP_STORE_ARRIVED_DATE = "storepickup_store_arrived_date";
		/// <summary>引渡し日時</summary>
		public const string FIELD_ORDER_STOREPICKUP_DELIVERED_COMPLETE_DATE = "storepickup_delivered_complete_date";
		/// <summary>返送日時</summary>
		public const string FIELD_ORDER_STOREPICKUP_RETURN_DATE = "storepickup_return_date";

		// 税率毎の注文金額情報
		public const string TABLE_ORDERPRICEBYTAXRATE = "w2_OrderPriceByTaxRate";                   // 税率毎の注文金額情報
		public const string FIELD_ORDERPRICEBYTAXRATE_ORDER_ID = "order_id";                        // 注文ID
		public const string FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE = "key_tax_rate";                        // 税率
		public const string FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE = "price_subtotal_by_rate";            // 税込商品合計額
		public const string FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE = "price_shipping_by_rate";            // 税込配送料合計額
		public const string FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE = "price_payment_by_rate";              // 税込決済手数料合計額
		public const string FIELD_ORDERPRICEBYTAXRATE_PRICE_TOTAL_BY_RATE = "price_total_by_rate";                  // 税込合計額
		public const string FIELD_ORDERPRICEBYTAXRATE_TAX_PRICE_BY_RATE = "tax_price_by_rate";                      // 税額
		public const string FIELD_ORDERPRICEBYTAXRATE_DATE_CREATED = "date_created";                // 作成日
		public const string FIELD_ORDERPRICEBYTAXRATE_DATE_CHANGED = "date_changed";                // 更新日
		public const string FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE = "return_price_correction_by_rate";	// 返品用金額補正

		// 注文者情報
		public const string TABLE_ORDEROWNER = "w2_OrderOwner";                                     // 注文者情報
		public const string FIELD_ORDEROWNER_ORDER_ID = "order_id";                                 // 注文ID
		public const string FIELD_ORDEROWNER_OWNER_KBN = "owner_kbn";                               // 注文者区分
		public const string FIELD_ORDEROWNER_OWNER_NAME = "owner_name";                             // 注文者氏名
		public const string FIELD_ORDEROWNER_OWNER_NAME_KANA = "owner_name_kana";                   // 注文者氏名かな
		public const string FIELD_ORDEROWNER_OWNER_MAIL_ADDR = "owner_mail_addr";                   // メールアドレス
		public const string FIELD_ORDEROWNER_OWNER_MAIL_ADDR2 = "owner_mail_addr2";                 // モバイルメールアドレス
		public const string FIELD_ORDEROWNER_OWNER_ZIP = "owner_zip";                               // 郵便番号
		public const string FIELD_ORDEROWNER_OWNER_ADDR = "owner_addr";                             // 住所
		public const string FIELD_ORDEROWNER_OWNER_ADDR1 = "owner_addr1";                           // 住所1
		public const string FIELD_ORDEROWNER_OWNER_ADDR2 = "owner_addr2";                           // 住所2
		public const string FIELD_ORDEROWNER_OWNER_ADDR3 = "owner_addr3";                           // 住所3
		public const string FIELD_ORDEROWNER_OWNER_ADDR4 = "owner_addr4";                           // 住所４
		public const string FIELD_ORDEROWNER_OWNER_TEL1 = "owner_tel1";                             // 電話番号1
		public const string FIELD_ORDEROWNER_OWNER_TEL2 = "owner_tel2";                             // 電話番号2
		public const string FIELD_ORDEROWNER_OWNER_TEL3 = "owner_tel3";                             // 電話番号3
		public const string FIELD_ORDEROWNER_OWNER_FAX = "owner_fax";                               // ＦＡＸ
		public const string FIELD_ORDEROWNER_OWNER_SEX = "owner_sex";                               // 性別
		public const string FIELD_ORDEROWNER_OWNER_BIRTH = "owner_birth";                           // 生年月日
		public const string FIELD_ORDEROWNER_OWNER_COMPANY_NAME = "owner_company_name";             // 企業名
		public const string FIELD_ORDEROWNER_OWNER_COMPANY_POST_NAME = "owner_company_post_name";   // 部署名
		public const string FIELD_ORDEROWNER_OWNER_COMPANY_EXECTIVE_NAME = "owner_company_exective_name";// 役職名
		public const string FIELD_ORDEROWNER_DEL_FLG = "del_flg";                                   // 削除フラグ
		public const string FIELD_ORDEROWNER_DATE_CREATED = "date_created";                         // 作成日
		public const string FIELD_ORDEROWNER_DATE_CHANGED = "date_changed";                         // 更新日
		public const string FIELD_ORDEROWNER_OWNER_NAME1 = "owner_name1";                           // 注文者氏名1
		public const string FIELD_ORDEROWNER_OWNER_NAME2 = "owner_name2";                           // 注文者氏名2
		public const string FIELD_ORDEROWNER_OWNER_NAME_KANA1 = "owner_name_kana1";                 // 注文者氏名かな1
		public const string FIELD_ORDEROWNER_OWNER_NAME_KANA2 = "owner_name_kana2";                 // 注文者氏名かな2
		public const string FIELD_ORDEROWNER_ACCESS_COUNTRY_ISO_CODE = "access_country_iso_code";   // アクセス国ISOコード
		public const string FIELD_ORDEROWNER_DISP_LANGUAGE_CODE = "disp_language_code";         // 表示言語コード
		public const string FIELD_ORDEROWNER_DISP_LANGUAGE_LOCALE_ID = "disp_language_locale_id";// 表示言語ロケールID
		public const string FIELD_ORDEROWNER_DISP_CURRENCY_CODE = "disp_currency_code";         // 表示通貨コード
		public const string FIELD_ORDEROWNER_DISP_CURRENCY_LOCALE_ID = "disp_currency_locale_id";// 表示通貨ロケールID
		public const string FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE = "owner_addr_country_iso_code";// 住所国ISOコード
		public const string FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_NAME = "owner_addr_country_name";   // 住所国名
		public const string FIELD_ORDEROWNER_OWNER_ADDR5 = "owner_addr5";                           // 住所5

		// 注文商品情報
		public const string TABLE_ORDERITEM = "w2_OrderItem";                                       // 注文商品情報
		public const string FIELD_ORDERITEM_ORDER_ID = "order_id";                                  // 注文ID
		public const string FIELD_ORDERITEM_ORDER_ITEM_NO = "order_item_no";                        // 注文商品枝番
		public const string FIELD_ORDERITEM_ORDER_SHIPPING_NO = "order_shipping_no";                // 配送先枝番
		public const string FIELD_ORDERITEM_SHOP_ID = "shop_id";                                    // 店舗ID
		public const string FIELD_ORDERITEM_PRODUCT_ID = "product_id";                              // 商品ID
		public const string FIELD_ORDERITEM_VARIATION_ID = "variation_id";                          // 商品バリエーションID
		public const string FIELD_ORDERITEM_SUPPLIER_ID = "supplier_id";                            // サプライヤID
		public const string FIELD_ORDERITEM_PRODUCT_NAME = "product_name";                          // 商品名
		public const string FIELD_ORDERITEM_PRODUCT_NAME_KANA = "product_name_kana";                // 商品名かな
		public const string FIELD_ORDERITEM_PRODUCT_PRICE = "product_price";                        // 商品単価
		public const string FIELD_ORDERITEM_PRODUCT_PRICE_ORG = "product_price_org";                // 商品単価（値引き前）
		public const string FIELD_ORDERITEM_PRODUCT_POINT = "product_point";                        // 商品ポイント
		public const string FIELD_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG = "product_tax_included_flg";  // 税込フラグ
		public const string FIELD_ORDERITEM_PRODUCT_TAX_RATE = "product_tax_rate";                  // 税率
		public const string FIELD_ORDERITEM_PRODUCT_TAX_ROUND_TYPE = "product_tax_round_type";      // 税計算方法
		public const string FIELD_ORDERITEM_PRODUCT_PRICE_PRETAX = "product_price_pretax";          // 税込販売価格
		public const string FIELD_ORDERITEM_PRODUCT_PRICE_SHIP = "product_price_ship";              // 配送料
		public const string FIELD_ORDERITEM_PRODUCT_PRICE_COST = "product_price_cost";              // 商品原価
		public const string FIELD_ORDERITEM_PRODUCT_POINT_KBN = "product_point_kbn";                // 付与ポイント区分
		public const string FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED = "item_realstock_reserved";    // 実在庫引当済み商品数
		public const string FIELD_ORDERITEM_ITEM_REALSTOCK_SHIPPED = "item_realstock_shipped";      // 実在庫出荷済み商品数
		public const string FIELD_ORDERITEM_ITEM_QUANTITY = "item_quantity";                        // 注文数
		public const string FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE = "item_quantity_single";          // 注文数（セット未考慮）
		public const string FIELD_ORDERITEM_ITEM_PRICE = "item_price";                              // 明細金額（小計）
		public const string FIELD_ORDERITEM_ITEM_PRICE_SINGLE = "item_price_single";                // 明細金額（小計・セット未考慮）
		public const string FIELD_ORDERITEM_ITEM_PRICE_TAX = "item_price_tax";                      // 税額
		public const string FIELD_ORDERITEM_PRODUCT_SET_ID = "product_set_id";                      // 商品セットID
		public const string FIELD_ORDERITEM_PRODUCT_SET_NO = "product_set_no";                      // 商品セット枝番
		public const string FIELD_ORDERITEM_PRODUCT_SET_COUNT = "product_set_count";                // 商品セット注文数
		public const string FIELD_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN = "item_return_exchange_kbn";  // 返品交換区分
		public const string FIELD_ORDERITEM_ITEM_MEMO = "item_memo";  // 明細備考
		public const string FIELD_ORDERITEM_ITEM_POINT = "item_point";                              // 明細ポイント
		public const string FIELD_ORDERITEM_ITEM_CANCEL_FLG = "item_cancel_flg";                    // キャンセルフラグ
		public const string FIELD_ORDERITEM_ITEM_CANCEL_DATE = "item_cancel_date";                  // キャンセル日時
		public const string FIELD_ORDERITEM_ITEM_RETURN_FLG = "item_return_flg";                    // 返品フラグ
		public const string FIELD_ORDERITEM_ITEM_RETURN_DATE = "item_return_date";                  // 返品日時
		public const string FIELD_ORDERITEM_DEL_FLG = "del_flg";                                    // 削除フラグ
		public const string FIELD_ORDERITEM_DATE_CREATED = "date_created";                          // 作成日
		public const string FIELD_ORDERITEM_DATE_CHANGED = "date_changed";                          // 更新日
		public const string FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS = "product_option_texts";          // 商品付帯情報選択値
		public const string FIELD_ORDERITEM_BRAND_ID = "brand_id";                                  // ブランドID
		public const string FIELD_ORDERITEM_DOWNLOAD_URL = "download_url";                          // ダウンロードURL
		public const string FIELD_ORDERITEM_PRODUCTSALE_ID = "productsale_id";                      // 商品セールID
		public const string FIELD_ORDERITEM_ORDER_SETPROMOTION_NO = "order_setpromotion_no";        // セットプロモーション枝番
		public const string FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO = "order_setpromotion_item_no";    // セットプロモーション商品枝番
		public const string FIELD_ORDERITEM_COOPERATION_ID1 = "cooperation_id1";                      // 商品連携ID1
		public const string FIELD_ORDERITEM_COOPERATION_ID2 = "cooperation_id2";                      // 商品連携ID2
		public const string FIELD_ORDERITEM_COOPERATION_ID3 = "cooperation_id3";                      // 商品連携ID3
		public const string FIELD_ORDERITEM_COOPERATION_ID4 = "cooperation_id4";                      // 商品連携ID4
		public const string FIELD_ORDERITEM_COOPERATION_ID5 = "cooperation_id5";                      // 商品連携ID5
		public const string FIELD_ORDERITEM_STOCK_RETURNED_FLG = "stock_returned_flg";                // 在庫戻し済みフラグ
		public const string FIELD_ORDERITEM_NOVELTY_ID = "novelty_id";                                // ノベルティID
		public const string FIELD_ORDERITEM_RECOMMEND_ID = "recommend_id";                          // レコメンドID
		public const string FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG = "fixed_purchase_product_flg";// 定期商品フラグ
		public const string FIELD_ORDERITEM_PRODUCT_BUNDLE_ID = "product_bundle_id";                // 商品同梱ID
		public const string FIELD_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE = "bundle_item_display_type";          // 同梱商品明細表示フラグ
		public const string FIELD_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE = "order_history_display_type";  // 明細表示フラグ
		public const string FIELD_ORDERITEM_PLURAL_SHIPPING_PRICE_FREE_FLG = "plural_shipping_price_free_flg";// 配送料金複数個無料フラグ
		public const string FIELD_ORDERITEM_SHIPPING_SIZE_KBN = "shipping_size_kbn";                // 配送サイズ区分
		public const string FIELD_ORDERITEM_COLUMN_FOR_MALL_ORDER = "column_for_mall_order";                // モール用項目
		public const string FIELD_ORDERITEM_COOPERATION_ID6 = "cooperation_id6";                    // 商品連携ID6
		public const string FIELD_ORDERITEM_COOPERATION_ID7 = "cooperation_id7";                    // 商品連携ID7
		public const string FIELD_ORDERITEM_COOPERATION_ID8 = "cooperation_id8";                    // 商品連携ID8
		public const string FIELD_ORDERITEM_COOPERATION_ID9 = "cooperation_id9";                    // 商品連携ID9
		public const string FIELD_ORDERITEM_COOPERATION_ID10 = "cooperation_id10";                  // 商品連携ID10
		public const string FIELD_ORDERITEM_GIFT_WRAPPING_ID = "gift_wrapping_id";                  // ギフト包装ID
		public const string FIELD_ORDERITEM_GIFT_MESSAGE = "gift_message";                          // ギフトメッセージ
		public const string FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_VALUE = "fixed_purchase_discount_value";  // 定期購入値引
		public const string FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_TYPE = "fixed_purchase_discount_type";    // 定期購入値引タイプ
		public const string FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_ORDER_COUNT = "fixed_purchase_item_order_count";		// 定期商品購入回数(注文時点)
		public const string FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_SHIPPED_COUNT = "fixed_purchase_item_shipped_count";	// 定期商品購入回数(出荷時点)
		public const string FIELD_ORDERITEM_LIMITED_PAYMENT_IDS = "limited_payment_ids";            // 決済利用不可
		/// <summary>明細金額（割引後価格）</summary>
		public const string FIELD_ORDERITEM_DISCOUNTED_PRICE = "item_discounted_price";
		/// <summary>頒布会コースID</summary>
		public const string FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID = "subscription_box_course_id";
		/// <summary>頒布会定額価格</summary>
		public const string FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT = "subscription_box_fixed_amount";

		/// <summary>頒布会コースID（接頭辞付き）</summary>
		public const string FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID_WITH_PREFIX = "item_subscription_box_course_id";
		/// <summary>頒布会定額価格（接頭辞付き）</summary>
		public const string FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX = "item_subscription_box_fixed_amount";

		// 注文配送先情報
		public const string TABLE_ORDERSHIPPING = "w2_OrderShipping";                               // 注文配送先情報
		public const string FIELD_ORDERSHIPPING_ORDER_ID = "order_id";                              // 注文ID
		public const string FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO = "order_shipping_no";            // 配送先枝番
		public const string FIELD_ORDERSHIPPING_SHIPPING_NAME = "shipping_name";                    // 配送先氏名
		public const string FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA = "shipping_name_kana";          // 配送先氏名かな
		public const string FIELD_ORDERSHIPPING_SHIPPING_ZIP = "shipping_zip";                      // 郵便番号
		public const string FIELD_ORDERSHIPPING_SHIPPING_ADDR1 = "shipping_addr1";                  // 住所1
		public const string FIELD_ORDERSHIPPING_SHIPPING_ADDR2 = "shipping_addr2";                  // 住所2
		public const string FIELD_ORDERSHIPPING_SHIPPING_ADDR3 = "shipping_addr3";                  // 住所3
		public const string FIELD_ORDERSHIPPING_SHIPPING_ADDR4 = "shipping_addr4";                  // 住所４
		public const string FIELD_ORDERSHIPPING_SHIPPING_TEL1 = "shipping_tel1";                    // 電話番号1
		public const string FIELD_ORDERSHIPPING_SHIPPING_TEL2 = "shipping_tel2";                    // 電話番号2
		public const string FIELD_ORDERSHIPPING_SHIPPING_TEL3 = "shipping_tel3";                    // 電話番号3
		public const string FIELD_ORDERSHIPPING_SHIPPING_FAX = "shipping_fax";                      // ＦＡＸ
		public const string FIELD_ORDERSHIPPING_SHIPPING_COMPANY = "shipping_company";              // 配送業者
		public const string FIELD_ORDERSHIPPING_SHIPPING_DATE = "shipping_date";                    // 配送希望日
		public const string FIELD_ORDERSHIPPING_SHIPPING_TIME = "shipping_time";                    // 配送希望時間帯
		public const string FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO = "shipping_check_no";            // 配送伝票番号
		public const string FIELD_ORDERSHIPPING_DEL_FLG = "del_flg";                                // 削除フラグ
		public const string FIELD_ORDERSHIPPING_DATE_CREATED = "date_created";                      // 作成日
		public const string FIELD_ORDERSHIPPING_DATE_CHANGED = "date_changed";                      // 更新日
		public const string FIELD_ORDERSHIPPING_SHIPPING_NAME1 = "shipping_name1";                  // 配送先氏名1
		public const string FIELD_ORDERSHIPPING_SHIPPING_NAME2 = "shipping_name2";                  // 配送先氏名2
		public const string FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1 = "shipping_name_kana1";        // 配送先氏名かな1
		public const string FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2 = "shipping_name_kana2";        // 配送先氏名かな2
		public const string FIELD_ORDERSHIPPING_SENDER_NAME = "sender_name";                        // 送り主氏名
		public const string FIELD_ORDERSHIPPING_SENDER_NAME1 = "sender_name1";                      // 送り主氏名1
		public const string FIELD_ORDERSHIPPING_SENDER_NAME2 = "sender_name2";                      // 送り主氏名2
		public const string FIELD_ORDERSHIPPING_SENDER_NAME_KANA = "sender_name_kana";              // 送り主氏名かな
		public const string FIELD_ORDERSHIPPING_SENDER_NAME_KANA1 = "sender_name_kana1";            // 送り主氏名かな1
		public const string FIELD_ORDERSHIPPING_SENDER_NAME_KANA2 = "sender_name_kana2";            // 送り主氏名かな2
		public const string FIELD_ORDERSHIPPING_SENDER_ZIP = "sender_zip";                          // 送り主郵便番号
		public const string FIELD_ORDERSHIPPING_SENDER_ADDR1 = "sender_addr1";                      // 送り主住所1
		public const string FIELD_ORDERSHIPPING_SENDER_ADDR2 = "sender_addr2";                      // 送り主住所2
		public const string FIELD_ORDERSHIPPING_SENDER_ADDR3 = "sender_addr3";                      // 送り主住所3
		public const string FIELD_ORDERSHIPPING_SENDER_ADDR4 = "sender_addr4";                      // 送り主住所４
		public const string FIELD_ORDERSHIPPING_SENDER_TEL1 = "sender_tel1";                        // 送り主電話番号1
		public const string FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE = "wrapping_paper_type";        // のし種類
		public const string FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME = "wrapping_paper_name";        // のし差出人
		public const string FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE = "wrapping_bag_type";            // 包装種類
		public const string FIELD_ORDERSHIPPING_SHIPPING_MEMO = "shipping_memo";                    // 配送メモ
		public const string FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME = "shipping_company_name";    // 配送先企業名
		public const string FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME = "shipping_company_post_name";// 配送先部署名
		public const string FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME = "sender_company_name";        // 送り主企業名
		public const string FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME = "sender_company_post_name";// 送り主部署名
		public const string FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG = "another_shipping_flg";        // 別出荷フラグ
		public const string FIELD_ORDERSHIPPING_SHIPPING_METHOD = "shipping_method";				// 配送方法
		public const string FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID = "delivery_company_id";        // 配送会社ID
		public const string FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE = "shipping_country_iso_code";// 配送先住所国ISOコード
		public const string FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME = "shipping_country_name";    // 配送先住所国名
		public const string FIELD_ORDERSHIPPING_SHIPPING_ADDR5 = "shipping_addr5";                  // 住所5
		public const string FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE = "sender_country_iso_code";// 送り主住所国ISOコード
		public const string FIELD_ORDERSHIPPING_SENDER_COUNTRY_NAME = "sender_country_name";        // 送り主住所国名
		public const string FIELD_ORDERSHIPPING_SENDER_ADDR5 = "sender_addr5";                      // 送り主住所5
		public const string FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE = "scheduled_shipping_date";// 出荷予定日
		public const string FIELD_ORDERSHIPPING_EXTERNAL_SHIPPING_COOPERATION_ID = "external_shipping_cooperation_id";// 外部連携用ID
		public const string FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG = "shipping_receiving_store_flg";	// 店舗受取フラグ
		public const string FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID = "shipping_receiving_store_id";	// 店舗受取店舗ID
		public const string FIELD_ORDERSHIPPING_SHIPPING_EXTERNAL_DELIVERTY_STATUS = "shipping_external_deliverty_status";	// 配送のペリカン管理番号
		public const string FIELD_ORDERSHIPPING_SHIPPING_STATUS = "shipping_status";							// 配送状態
		public const string FIELD_ORDERSHIPPING_SHIPPING_STATUS_UPDATE_DATE = "shipping_status_update_date";	// 配送状態更新日
		public const string FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_MAIL_DATE = "shipping_receiving_mail_date";	// 店舗受取メール送信日
		/// <summary>コンビニ受取：受取方法</summary>
		public const string FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE = "shipping_receiving_store_type";
		public const string FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE = "shipping_status_code";      // 完了状態コード
		public const string FIELD_ORDERSHIPPING_SHIPPING_OFFICE_NAME = "shipping_office_name";      // 営業所略称
		public const string FIELD_ORDERSHIPPING_SHIPPING_HANDY_TIME = "shipping_handy_time";        // Handy操作時間
		public const string FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS = "shipping_current_status";// 現在の状態
		public const string FIELD_ORDERSHIPPING_SHIPPING_STATUS_DETAIL = "shipping_status_detail";  // 状態説明
		/// <summary>Order shipping storepickup real shop id</summary>
		public const string FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID = "storepickup_real_shop_id";

		// 注文セットプロモーション情報
		public const string TABLE_ORDERSETPROMOTION = "w2_OrderSetPromotion";								// 注文セットプロモーション情報
		public const string FIELD_ORDERSETPROMOTION_ORDER_ID = "order_id";									// 注文ID
		public const string FIELD_ORDERSETPROMOTION_ORDER_SETPROMOTION_NO = "order_setpromotion_no";		// 注文セットプロモーション枝番
		public const string FIELD_ORDERSETPROMOTION_SETPROMOTION_ID = "setpromotion_id";					// セットプロモーションID
		public const string FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME = "setpromotion_name";				// 管理用セットプロモーション名
		public const string FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME = "setpromotion_disp_name";		// 表示用セットプロモーション名
		public const string FIELD_ORDERSETPROMOTION_UNDISCOUNTED_PRODUCT_SUBTOTAL = "undiscounted_product_subtotal";	// 割引前商品金額
		public const string FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG = "product_discount_flg";			// 商品金額割引フラグ
		public const string FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT = "product_discount_amount";	// 商品割引額
		public const string FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG = "shipping_charge_free_flg";				// 配送料無料フラグ
		public const string FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT = "shipping_charge_discount_amount";// 配送料値引き額
		public const string FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG = "payment_charge_free_flg";				// 決済手数料無料フラグ
		public const string FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT = "payment_charge_discount_amount";	// 決済手数料値引き額

		// 注文クーポンテーブル
		public const string TABLE_ORDERCOUPON = "w2_OrderCoupon";                                   // 注文クーポンテーブル
		public const string FIELD_ORDERCOUPON_ORDER_ID = "order_id";                                // 注文ID
		public const string FIELD_ORDERCOUPON_ORDER_COUPON_NO = "order_coupon_no";                  // 注文クーポン枝番
		public const string FIELD_ORDERCOUPON_DEPT_ID = "dept_id";                                  // 識別ID
		public const string FIELD_ORDERCOUPON_COUPON_ID = "coupon_id";                              // クーポンID
		public const string FIELD_ORDERCOUPON_COUPON_NO = "coupon_no";                              // 枝番
		public const string FIELD_ORDERCOUPON_COUPON_CODE = "coupon_code";                          // クーポンコード
		public const string FIELD_ORDERCOUPON_COUPON_NAME = "coupon_name";                          // 管理用クーポン名
		public const string FIELD_ORDERCOUPON_COUPON_DISP_NAME = "coupon_disp_name";                // 表示用クーポン名
		public const string FIELD_ORDERCOUPON_COUPON_TYPE = "coupon_type";                          // クーポン種別
		public const string FIELD_ORDERCOUPON_COUPON_DISCOUNT_PRICE = "coupon_discount_price";      // クーポン割引額
		public const string FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE = "coupon_discount_rate";        // クーポン割引率
		public const string FIELD_ORDERCOUPON_DATE_CREATED = "date_created";                        // 作成日
		public const string FIELD_ORDERCOUPON_DATE_CHANGED = "date_changed";                        // 更新日
		public const string FIELD_ORDERCOUPON_LAST_CHANGED = "last_changed";                        // 最終更新者

		// 注文ワークフロー設定
		public const string TABLE_ORDERWORKFLOWSETTING = "w2_OrderWorkflowSetting";                 // 注文ワークフロー設定
		public const string FIELD_ORDERWORKFLOWSETTING_SHOP_ID = "shop_id";                         // 店舗ID
		public const string FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN = "workflow_kbn";               // ワークフロー区分
		public const string FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO = "workflow_no";                 // 枝番
		public const string FIELD_ORDERWORKFLOWSETTING_WORKFLOW_REF_NO = "workflow_ref_no";         // 参照枝番
		public const string FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NAME = "workflow_name";             // ワークフロー名
		public const string FIELD_ORDERWORKFLOWSETTING_DESC1 = "desc1";                             // 説明1
		public const string FIELD_ORDERWORKFLOWSETTING_DESC2 = "desc2";                             // 説明2
		public const string FIELD_ORDERWORKFLOWSETTING_DESC3 = "desc3";                             // 説明3
		public const string FIELD_ORDERWORKFLOWSETTING_DISPLAY_ORDER = "display_order";             // 表示順
		public const string FIELD_ORDERWORKFLOWSETTING_DISPLAY_COUNT = "display_count";             // 表示件数
		public const string FIELD_ORDERWORKFLOWSETTING_VALID_FLG = "valid_flg";                     // 有効フラグ
		public const string FIELD_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN = "workflow_detail_kbn"; // ワークフロー詳細区分
		public const string FIELD_ORDERWORKFLOWSETTING_DISPLAY_KBN = "display_kbn";                 // 表示区分
		public const string FIELD_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG = "additional_search_flg";// 追加検索可否FLG
		public const string FIELD_ORDERWORKFLOWSETTING_SEARCH_SETTING = "search_setting";           // 抽出検索条件
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_STATUS_CHANGE = "order_status_change"; // 注文ステータス変更区分
		public const string FIELD_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE = "product_realstock_change";// 商品実在庫変更区分
		public const string FIELD_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE = "payment_status_change";// 入金ステータス変更区分
		public const string FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION = "external_payment_action";// 外部決済連携処理区分
		public const string FIELD_ORDERWORKFLOWSETTING_DEMAND_STATUS_CHANGE = "demand_status_change";// 督促ステータス変更区分
		public const string FIELD_ORDERWORKFLOWSETTING_RETURN_EXCHANGE_STATUS_CHANGE = "return_exchange_status_change";// 返品交換ステータス変更区分
		public const string FIELD_ORDERWORKFLOWSETTING_REPAYMENT_STATUS_CHANGE = "repayment_status_change";// 返金ステータス変更区分
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE1 = "order_extend_status_change1";// 注文拡張ステータス変更区分1
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE2 = "order_extend_status_change2";// 注文拡張ステータス変更区分2
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE3 = "order_extend_status_change3";// 注文拡張ステータス変更区分3
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE4 = "order_extend_status_change4";// 注文拡張ステータス変更区分4
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE5 = "order_extend_status_change5";// 注文拡張ステータス変更区分5
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE6 = "order_extend_status_change6";// 注文拡張ステータス変更区分6
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE7 = "order_extend_status_change7";// 注文拡張ステータス変更区分7
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE8 = "order_extend_status_change8";// 注文拡張ステータス変更区分8
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE9 = "order_extend_status_change9";// 注文拡張ステータス変更区分9
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE10 = "order_extend_status_change10";// 注文拡張ステータス変更区分10
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE11 = "order_extend_status_change11";// 注文拡張ステータス変更区分11
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE12 = "order_extend_status_change12";// 注文拡張ステータス変更区分12
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE13 = "order_extend_status_change13";// 注文拡張ステータス変更区分13
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE14 = "order_extend_status_change14";// 注文拡張ステータス変更区分14
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE15 = "order_extend_status_change15";// 注文拡張ステータス変更区分15
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE16 = "order_extend_status_change16";// 注文拡張ステータス変更区分16
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE17 = "order_extend_status_change17";// 注文拡張ステータス変更区分17
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE18 = "order_extend_status_change18";// 注文拡張ステータス変更区分18
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE19 = "order_extend_status_change19";// 注文拡張ステータス変更区分19
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE20 = "order_extend_status_change20";// 注文拡張ステータス変更区分20
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE21 = "order_extend_status_change21";// 注文拡張ステータス変更区分21
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE22 = "order_extend_status_change22";// 注文拡張ステータス変更区分22
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE23 = "order_extend_status_change23";// 注文拡張ステータス変更区分23
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE24 = "order_extend_status_change24";// 注文拡張ステータス変更区分24
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE25 = "order_extend_status_change25";// 注文拡張ステータス変更区分25
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE26 = "order_extend_status_change26";// 注文拡張ステータス変更区分26
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE27 = "order_extend_status_change27";// 注文拡張ステータス変更区分27
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE28 = "order_extend_status_change28";// 注文拡張ステータス変更区分28
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE29 = "order_extend_status_change29";// 注文拡張ステータス変更区分29
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE30 = "order_extend_status_change30";// 注文拡張ステータス変更区分30
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE31 = "order_extend_status_change31";// 注文拡張ステータス変更区分31
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE32 = "order_extend_status_change32";// 注文拡張ステータス変更区分32
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE33 = "order_extend_status_change33";// 注文拡張ステータス変更区分33
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE34 = "order_extend_status_change34";// 注文拡張ステータス変更区分34
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE35 = "order_extend_status_change35";// 注文拡張ステータス変更区分35
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE36 = "order_extend_status_change36";// 注文拡張ステータス変更区分36
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE37 = "order_extend_status_change37";// 注文拡張ステータス変更区分37
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE38 = "order_extend_status_change38";// 注文拡張ステータス変更区分38
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE39 = "order_extend_status_change39";// 注文拡張ステータス変更区分39
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE40 = "order_extend_status_change40";// 注文拡張ステータス変更区分40
		/// <summary>注文拡張ステータス変更区分41</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE41 = "order_extend_status_change41";
		/// <summary>注文拡張ステータス変更区分42</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE42 = "order_extend_status_change42";
		/// <summary>注文拡張ステータス変更区分43</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE43 = "order_extend_status_change43";
		/// <summary>注文拡張ステータス変更区分44</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE44 = "order_extend_status_change44";
		/// <summary>注文拡張ステータス変更区分45</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE45 = "order_extend_status_change45";
		/// <summary>注文拡張ステータス変更区分46</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE46 = "order_extend_status_change46";
		/// <summary>注文拡張ステータス変更区分47</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE47 = "order_extend_status_change47";
		/// <summary>注文拡張ステータス変更区分48</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE48 = "order_extend_status_change48";
		/// <summary>注文拡張ステータス変更区分49</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE49 = "order_extend_status_change49";
		/// <summary>注文拡張ステータス変更区分50</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE50 = "order_extend_status_change50";
		public const string FIELD_ORDERWORKFLOWSETTING_MAIL_ID = "mail_id";                         // 送信メールテンプレートID
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEFAULT_SELECT = "cassette_default_select";// カセット表示用初期選択値
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE = "cassette_no_update";   // カセット表示用未実行フラグ
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STATUS_CHANGE = "cassette_order_status_change";// カセット表示用注文ステータス変更区分
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_PRODUCT_REALSTOCK_CHANGE = "cassette_product_realstock_change";// カセット表示用商品実在庫変更区分
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_PAYMENT_STATUS_CHANGE = "cassette_payment_status_change";// カセット表示用入金ステータス変更区分
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_PAYMENT_ACTION = "cassette_external_payment_action";// カセット表示用外部決済連携処理区分
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE = "cassette_fixed_purchase_stop_unavailable_shipping_area_change";// カセット表示用配送不可エリア停止変更区分
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEMAND_STATUS_CHANGE = "cassette_demand_status_change";// カセット表示用督促ステータス変更区分
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_EXCHANGE_STATUS_CHANGE = "cassette_return_exchange_status_change";// カセット表示用返品交換ステータス変更区分
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_REPAYMENT_STATUS_CHANGE = "cassette_repayment_status_change";// カセット表示用返金ステータス変更区分
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE1 = "cassette_order_extend_status_change1";// カセット表示用注文拡張ステータス変更区分1
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE2 = "cassette_order_extend_status_change2";// カセット表示用注文拡張ステータス変更区分2
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE3 = "cassette_order_extend_status_change3";// カセット表示用注文拡張ステータス変更区分3
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE4 = "cassette_order_extend_status_change4";// カセット表示用注文拡張ステータス変更区分4
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE5 = "cassette_order_extend_status_change5";// カセット表示用注文拡張ステータス変更区分5
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE6 = "cassette_order_extend_status_change6";// カセット表示用注文拡張ステータス変更区分6
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE7 = "cassette_order_extend_status_change7";// カセット表示用注文拡張ステータス変更区分7
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE8 = "cassette_order_extend_status_change8";// カセット表示用注文拡張ステータス変更区分8
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE9 = "cassette_order_extend_status_change9";// カセット表示用注文拡張ステータス変更区分9
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE10 = "cassette_order_extend_status_change10";// カセット表示用注文拡張ステータス変更区分10
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE11 = "cassette_order_extend_status_change11";// カセット表示用注文拡張ステータス変更区分11
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE12 = "cassette_order_extend_status_change12";// カセット表示用注文拡張ステータス変更区分12
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE13 = "cassette_order_extend_status_change13";// カセット表示用注文拡張ステータス変更区分13
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE14 = "cassette_order_extend_status_change14";// カセット表示用注文拡張ステータス変更区分14
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE15 = "cassette_order_extend_status_change15";// カセット表示用注文拡張ステータス変更区分15
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE16 = "cassette_order_extend_status_change16";// カセット表示用注文拡張ステータス変更区分16
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE17 = "cassette_order_extend_status_change17";// カセット表示用注文拡張ステータス変更区分17
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE18 = "cassette_order_extend_status_change18";// カセット表示用注文拡張ステータス変更区分18
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE19 = "cassette_order_extend_status_change19";// カセット表示用注文拡張ステータス変更区分19
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE20 = "cassette_order_extend_status_change20";// カセット表示用注文拡張ステータス変更区分20
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE21 = "cassette_order_extend_status_change21";// カセット表示用注文拡張ステータス変更区分21
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE22 = "cassette_order_extend_status_change22";// カセット表示用注文拡張ステータス変更区分22
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE23 = "cassette_order_extend_status_change23";// カセット表示用注文拡張ステータス変更区分23
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE24 = "cassette_order_extend_status_change24";// カセット表示用注文拡張ステータス変更区分24
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE25 = "cassette_order_extend_status_change25";// カセット表示用注文拡張ステータス変更区分25
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE26 = "cassette_order_extend_status_change26";// カセット表示用注文拡張ステータス変更区分26
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE27 = "cassette_order_extend_status_change27";// カセット表示用注文拡張ステータス変更区分27
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE28 = "cassette_order_extend_status_change28";// カセット表示用注文拡張ステータス変更区分28
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE29 = "cassette_order_extend_status_change29";// カセット表示用注文拡張ステータス変更区分29
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE30 = "cassette_order_extend_status_change30";// カセット表示用注文拡張ステータス変更区分30
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE31 = "cassette_order_extend_status_change31";// カセット表示用注文拡張ステータス変更区分31
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE32 = "cassette_order_extend_status_change32";// カセット表示用注文拡張ステータス変更区分32
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE33 = "cassette_order_extend_status_change33";// カセット表示用注文拡張ステータス変更区分33
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE34 = "cassette_order_extend_status_change34";// カセット表示用注文拡張ステータス変更区分34
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE35 = "cassette_order_extend_status_change35";// カセット表示用注文拡張ステータス変更区分35
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE36 = "cassette_order_extend_status_change36";// カセット表示用注文拡張ステータス変更区分36
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE37 = "cassette_order_extend_status_change37";// カセット表示用注文拡張ステータス変更区分37
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE38 = "cassette_order_extend_status_change38";// カセット表示用注文拡張ステータス変更区分38
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE39 = "cassette_order_extend_status_change39";// カセット表示用注文拡張ステータス変更区分39
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE40 = "cassette_order_extend_status_change40";// カセット表示用注文拡張ステータス変更区分40
		/// <summary>カセット表示用注文拡張ステータス変更区分41</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE41 = "cassette_order_extend_status_change41";
		/// <summary>カセット表示用注文拡張ステータス変更区分42</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE42 = "cassette_order_extend_status_change42";
		/// <summary>カセット表示用注文拡張ステータス変更区分43</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE43 = "cassette_order_extend_status_change43";
		/// <summary>カセット表示用注文拡張ステータス変更区分44</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE44 = "cassette_order_extend_status_change44";
		/// <summary>カセット表示用注文拡張ステータス変更区分45</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE45 = "cassette_order_extend_status_change45";
		/// <summary>カセット表示用注文拡張ステータス変更区分46</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE46 = "cassette_order_extend_status_change46";
		/// <summary>カセット表示用注文拡張ステータス変更区分47</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE47 = "cassette_order_extend_status_change47";
		/// <summary>カセット表示用注文拡張ステータス変更区分48</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE48 = "cassette_order_extend_status_change48";
		/// <summary>カセット表示用注文拡張ステータス変更区分49</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE49 = "cassette_order_extend_status_change49";
		/// <summary>カセット表示用注文拡張ステータス変更区分50</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE50 = "cassette_order_extend_status_change50";
		public const string FIELD_ORDERWORKFLOWSETTING_DEL_FLG = "del_flg";                         // 削除フラグ
		public const string FIELD_ORDERWORKFLOWSETTING_DATE_CREATED = "date_created";               // 作成日
		public const string FIELD_ORDERWORKFLOWSETTING_DATE_CHANGED = "date_changed";               // 更新日
		public const string FIELD_ORDERWORKFLOWSETTING_LAST_CHANGED = "last_changed";               // 最終更新者
		public const string FIELD_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION = "scheduled_shipping_date_action";	// 出荷予定日アクション
		public const string FIELD_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION = "shipping_date_action";	// 配送希望日アクション
		public const string FIELD_ORDERWORKFLOWSETTING_TARGET_TYPE = "workflow_target_type";	// ワークフロー対象種別
		public const string FIELD_ORDERWORKFLOWSETTING_RETURN_ACTION = "return_action";	// Return action
		public const string FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_KBN = "return_reason_kbn";	// Return reason kbn
		public const string FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_MEMO = "return_reason_memo";	// Return reason memo
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_ACTION = "cassette_return_action";	// Cassette return action
		public const string FIELD_ORDERWORKFLOWSETTING_RECEIPT_OUTPUT_FLG_CHANGE = "receipt_output_flg_change";// 領収書出力フラグ
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_RECEIPT_OUTPUT_FLG_CHANGE = "cassette_receipt_output_flg_change";// カセット表示用領収書出力フラグ
		public const string FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_API = "tw_invoice_status_api";	// Invoice status Api
		public const string FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_CHANGE = "tw_invoice_status_change";	// Invoice status Change
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_CHANGE = "tw_cassette_invoice_status_change";	// Cassette Invoice Status Change
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_API = "tw_cassette_invoice_status_api";	// Cassette Invoice Status Api
		/// <summary>External Order Information Action</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION = "tw_external_order_info_action";
		/// <summary>Cassete External Order Information Action</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_ORDER_INFO_ACTION = "tw_cassette_external_order_info_action";
		/// <summary>配送先 都道府県</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_SHIPPING_PREFECTURES = "workflow_prefectures";
		/// <summary>市区町村</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_SHIPPING_CITY = "workflow_city";
		/// <summary>店舗受取ステータス</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_ORDER_STOREPICKUP_STATUS_CHANGE = "storepickup_status_change";
		/// <summary>カセット店舗受取ステータス</summary>
		public const string FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STOREPICKUP_STATUS_CHANGE = "cassette_storepickup_status_change";

		// 定期ワークフロー設定
		public const string TABLE_FIXEDPURCHASEWORKFLOWSETTING = "w2_FixedPurchaseWorkflowSetting"; // 定期ワークフロー設定
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_SHOP_ID = "shop_id";                 // 店舗ID
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_KBN = "workflow_kbn";       // ワークフロー区分
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_NO = "workflow_no";         // 枝番
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_NAME = "workflow_name";     // ワークフロー名
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_DESC1 = "desc1";                     // 説明1
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_DESC2 = "desc2";                     // 説明2
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_DESC3 = "desc3";                     // 説明3
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_ORDER = "display_order";     // 表示順
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_COUNT = "display_count";     // 表示件数
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_VALID_FLG = "valid_flg";             // 有効フラグ
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_DETAIL_KBN = "workflow_detail_kbn";// ワークフロー詳細区分
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_KBN = "display_kbn";         // 表示区分
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG = "additional_search_flg";// 追加検索可否FLG
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_SEARCH_SETTING = "search_setting";   // 抽出検索条件
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE = "fixed_purchase_is_alive_change";// 定期状態変更
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE = "fixed_purchase_payment_status_change";// 決済ステータス更新
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_SHIPPING_DATE_CHANGE = "next_shipping_date_change";// 次回配送日更新
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_NEXT_SHIPPING_DATE_CHANGE = "next_next_shipping_date_change";// 次々回配送日更新
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE = "fixed_purchase_stop_unavailable_shipping_area_change";// 配送不可エリア停止変更
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE1 = "fixed_purchase_extend_status_change1";// 注文拡張ステータス変更区分1
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE2 = "fixed_purchase_extend_status_change2";// 注文拡張ステータス変更区分2
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE3 = "fixed_purchase_extend_status_change3";// 注文拡張ステータス変更区分3
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE4 = "fixed_purchase_extend_status_change4";// 注文拡張ステータス変更区分4
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE5 = "fixed_purchase_extend_status_change5";// 注文拡張ステータス変更区分5
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE6 = "fixed_purchase_extend_status_change6";// 注文拡張ステータス変更区分6
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE7 = "fixed_purchase_extend_status_change7";// 注文拡張ステータス変更区分7
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE8 = "fixed_purchase_extend_status_change8";// 注文拡張ステータス変更区分8
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE9 = "fixed_purchase_extend_status_change9";// 注文拡張ステータス変更区分9
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE10 = "fixed_purchase_extend_status_change10";// 注文拡張ステータス変更区分10
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_DEFAULT_SELECT = "cassette_default_select";// カセット表示用初期選択値
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NO_UPDATE = "cassette_no_update";// カセット表示用未実行フラグ
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_IS_ALIVE_CHANGE = "cassette_fixed_purchase_is_alive_change";// カセット表示用定期状態変更
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE = "cassette_fixed_purchase_payment_status_change";// カセット表示用決済ステータス更新
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE = "cassette_fixed_purchase_stop_unavailable_shipping_area_change";// カセット表示用配送不可エリア停止更新
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NEXT_SHIPPING_DATE_CHANGE = "cassette_next_shipping_date_change";// カセット表示用次回配送日更新
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NEXT_NEXT_SHIPPING_DATE_CHANGE = "cassette_next_next_shipping_date_change";// カセット表示用次々回配送日更新
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE1 = "cassette_fixed_purchase_extend_status_change1";// カセット表示用注文拡張ステータス変更区分1
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE2 = "cassette_fixed_purchase_extend_status_change2";// カセット表示用注文拡張ステータス変更区分2
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE3 = "cassette_fixed_purchase_extend_status_change3";// カセット表示用注文拡張ステータス変更区分3
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE4 = "cassette_fixed_purchase_extend_status_change4";// カセット表示用注文拡張ステータス変更区分4
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE5 = "cassette_fixed_purchase_extend_status_change5";// カセット表示用注文拡張ステータス変更区分5
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE6 = "cassette_fixed_purchase_extend_status_change6";// カセット表示用注文拡張ステータス変更区分6
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE7 = "cassette_fixed_purchase_extend_status_change7";// カセット表示用注文拡張ステータス変更区分7
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE8 = "cassette_fixed_purchase_extend_status_change8";// カセット表示用注文拡張ステータス変更区分8
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE9 = "cassette_fixed_purchase_extend_status_change9";// カセット表示用注文拡張ステータス変更区分9
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE10 = "cassette_fixed_purchase_extend_status_change10";// カセット表示用注文拡張ステータス変更区分10
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_DATE_CREATED = "date_created";       // 作成日
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_DATE_CHANGED = "date_changed";       // 更新日
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_LAST_CHANGED = "last_changed";       // 最終更新者
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE11 = "fixed_purchase_extend_status_change11";// 注文拡張ステータス変更区分11
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE12 = "fixed_purchase_extend_status_change12";// 注文拡張ステータス変更区分12
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE13 = "fixed_purchase_extend_status_change13";// 注文拡張ステータス変更区分13
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE14 = "fixed_purchase_extend_status_change14";// 注文拡張ステータス変更区分14
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE15 = "fixed_purchase_extend_status_change15";// 注文拡張ステータス変更区分15
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE16 = "fixed_purchase_extend_status_change16";// 注文拡張ステータス変更区分16
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE17 = "fixed_purchase_extend_status_change17";// 注文拡張ステータス変更区分17
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE18 = "fixed_purchase_extend_status_change18";// 注文拡張ステータス変更区分18
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE19 = "fixed_purchase_extend_status_change19";// 注文拡張ステータス変更区分19
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE20 = "fixed_purchase_extend_status_change20";// 注文拡張ステータス変更区分20
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE21 = "fixed_purchase_extend_status_change21";// 注文拡張ステータス変更区分21
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE22 = "fixed_purchase_extend_status_change22";// 注文拡張ステータス変更区分22
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE23 = "fixed_purchase_extend_status_change23";// 注文拡張ステータス変更区分23
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE24 = "fixed_purchase_extend_status_change24";// 注文拡張ステータス変更区分24
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE25 = "fixed_purchase_extend_status_change25";// 注文拡張ステータス変更区分25
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE26 = "fixed_purchase_extend_status_change26";// 注文拡張ステータス変更区分26
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE27 = "fixed_purchase_extend_status_change27";// 注文拡張ステータス変更区分27
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE28 = "fixed_purchase_extend_status_change28";// 注文拡張ステータス変更区分28
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE29 = "fixed_purchase_extend_status_change29";// 注文拡張ステータス変更区分29
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE30 = "fixed_purchase_extend_status_change30";// 注文拡張ステータス変更区分30
		/// <summary>注文拡張ステータス変更区分41</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE41 = "fixed_purchase_extend_status_change41";
		/// <summary>注文拡張ステータス変更区分42</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE42 = "fixed_purchase_extend_status_change42";
		/// <summary>注文拡張ステータス変更区分43</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE43 = "fixed_purchase_extend_status_change43";
		/// <summary>注文拡張ステータス変更区分44</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE44 = "fixed_purchase_extend_status_change44";
		/// <summary>注文拡張ステータス変更区分45</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE45 = "fixed_purchase_extend_status_change45";
		/// <summary>注文拡張ステータス変更区分46</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE46 = "fixed_purchase_extend_status_change46";
		/// <summary>注文拡張ステータス変更区分47</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE47 = "fixed_purchase_extend_status_change47";
		/// <summary>注文拡張ステータス変更区分48</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE48 = "fixed_purchase_extend_status_change48";
		/// <summary>注文拡張ステータス変更区分49</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE49 = "fixed_purchase_extend_status_change49";
		/// <summary>注文拡張ステータス変更区分50</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE50 = "fixed_purchase_extend_status_change50";
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE11 = "cassette_fixed_purchase_extend_status_change11";// カセット表示用注文拡張ステータス変更区分11
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE12 = "cassette_fixed_purchase_extend_status_change12";// カセット表示用注文拡張ステータス変更区分12
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE13 = "cassette_fixed_purchase_extend_status_change13";// カセット表示用注文拡張ステータス変更区分13
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE14 = "cassette_fixed_purchase_extend_status_change14";// カセット表示用注文拡張ステータス変更区分14
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE15 = "cassette_fixed_purchase_extend_status_change15";// カセット表示用注文拡張ステータス変更区分15
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE16 = "cassette_fixed_purchase_extend_status_change16";// カセット表示用注文拡張ステータス変更区分16
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE17 = "cassette_fixed_purchase_extend_status_change17";// カセット表示用注文拡張ステータス変更区分17
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE18 = "cassette_fixed_purchase_extend_status_change18";// カセット表示用注文拡張ステータス変更区分18
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE19 = "cassette_fixed_purchase_extend_status_change19";// カセット表示用注文拡張ステータス変更区分19
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE20 = "cassette_fixed_purchase_extend_status_change20";// カセット表示用注文拡張ステータス変更区分20
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE21 = "cassette_fixed_purchase_extend_status_change21";// カセット表示用注文拡張ステータス変更区分21
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE22 = "cassette_fixed_purchase_extend_status_change22";// カセット表示用注文拡張ステータス変更区分22
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE23 = "cassette_fixed_purchase_extend_status_change23";// カセット表示用注文拡張ステータス変更区分23
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE24 = "cassette_fixed_purchase_extend_status_change24";// カセット表示用注文拡張ステータス変更区分24
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE25 = "cassette_fixed_purchase_extend_status_change25";// カセット表示用注文拡張ステータス変更区分25
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE26 = "cassette_fixed_purchase_extend_status_change26";// カセット表示用注文拡張ステータス変更区分26
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE27 = "cassette_fixed_purchase_extend_status_change27";// カセット表示用注文拡張ステータス変更区分27
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE28 = "cassette_fixed_purchase_extend_status_change28";// カセット表示用注文拡張ステータス変更区分28
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE29 = "cassette_fixed_purchase_extend_status_change29";// カセット表示用注文拡張ステータス変更区分29
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE30 = "cassette_fixed_purchase_extend_status_change30";// カセット表示用注文拡張ステータス変更区分30
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE31 = "fixed_purchase_extend_status_change31";// 注文拡張ステータス変更区分31
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE32 = "fixed_purchase_extend_status_change32";// 注文拡張ステータス変更区分32
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE33 = "fixed_purchase_extend_status_change33";// 注文拡張ステータス変更区分33
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE34 = "fixed_purchase_extend_status_change34";// 注文拡張ステータス変更区分34
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE35 = "fixed_purchase_extend_status_change35";// 注文拡張ステータス変更区分35
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE36 = "fixed_purchase_extend_status_change36";// 注文拡張ステータス変更区分36
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE37 = "fixed_purchase_extend_status_change37";// 注文拡張ステータス変更区分37
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE38 = "fixed_purchase_extend_status_change38";// 注文拡張ステータス変更区分38
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE39 = "fixed_purchase_extend_status_change39";// 注文拡張ステータス変更区分39
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE40 = "fixed_purchase_extend_status_change40";// 注文拡張ステータス変更区分40
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE31 = "cassette_fixed_purchase_extend_status_change31";// カセット表示用注文拡張ステータス変更区分31
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE32 = "cassette_fixed_purchase_extend_status_change32";// カセット表示用注文拡張ステータス変更区分32
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE33 = "cassette_fixed_purchase_extend_status_change33";// カセット表示用注文拡張ステータス変更区分33
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE34 = "cassette_fixed_purchase_extend_status_change34";// カセット表示用注文拡張ステータス変更区分34
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE35 = "cassette_fixed_purchase_extend_status_change35";// カセット表示用注文拡張ステータス変更区分35
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE36 = "cassette_fixed_purchase_extend_status_change36";// カセット表示用注文拡張ステータス変更区分36
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE37 = "cassette_fixed_purchase_extend_status_change37";// カセット表示用注文拡張ステータス変更区分37
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE38 = "cassette_fixed_purchase_extend_status_change38";// カセット表示用注文拡張ステータス変更区分38
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE39 = "cassette_fixed_purchase_extend_status_change39";// カセット表示用注文拡張ステータス変更区分39
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE40 = "cassette_fixed_purchase_extend_status_change40";// カセット表示用注文拡張ステータス変更区分40
		/// <summary>カセット表示用注文拡張ステータス変更区分41</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE41 = "cassette_fixed_purchase_extend_status_change41";
		/// <summary>カセット表示用注文拡張ステータス変更区分42</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE42 = "cassette_fixed_purchase_extend_status_change42";
		/// <summary>カセット表示用注文拡張ステータス変更区分43</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE43 = "cassette_fixed_purchase_extend_status_change43";
		/// <summary>カセット表示用注文拡張ステータス変更区分44</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE44 = "cassette_fixed_purchase_extend_status_change44";
		/// <summary>カセット表示用注文拡張ステータス変更区分45</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE45 = "cassette_fixed_purchase_extend_status_change45";
		/// <summary>カセット表示用注文拡張ステータス変更区分46</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE46 = "cassette_fixed_purchase_extend_status_change46";
		/// <summary>カセット表示用注文拡張ステータス変更区分47</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE47 = "cassette_fixed_purchase_extend_status_change47";
		/// <summary>カセット表示用注文拡張ステータス変更区分48</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE48 = "cassette_fixed_purchase_extend_status_change48";
		/// <summary>カセット表示用注文拡張ステータス変更区分49</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE49 = "cassette_fixed_purchase_extend_status_change49";
		/// <summary>カセット表示用注文拡張ステータス変更区分50</summary>
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE50 = "cassette_fixed_purchase_extend_status_change50";
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_REASON_ID = "cancel_reason_id";	// FixedPurchase cancel reason
		public const string FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_MEMO = "cancel_memo";	// FixedPurchase cancel memo

		// 注文拡張項目設定マスタ
		public const string TABLE_ORDEREXTENDSETTING = "w2_OrderExtendSetting";                     // 注文拡張項目設定マスタ
		public const string FIELD_ORDEREXTENDSETTING_SETTING_ID = "setting_id";                     // ユーザ拡張項目ID
		public const string FIELD_ORDEREXTENDSETTING_SETTING_NAME = "setting_name";                 // 名称
		public const string FIELD_ORDEREXTENDSETTING_OUTLINE_KBN = "outline_kbn";                   // 概要表示区分
		public const string FIELD_ORDEREXTENDSETTING_OUTLINE = "outline";                           // 概要
		public const string FIELD_ORDEREXTENDSETTING_SORT_ORDER = "sort_order";                     // 並び順
		public const string FIELD_ORDEREXTENDSETTING_INPUT_TYPE = "input_type";                     // 入力種別
		public const string FIELD_ORDEREXTENDSETTING_INPUT_DEFAULT = "input_default";               // 初期値
		public const string FIELD_ORDEREXTENDSETTING_INIT_ONLY_FLG = "init_only_flg";               // 登録時のみフラグ
		public const string FIELD_ORDEREXTENDSETTING_VALIDATOR = "validator";                       // バリデータテキスト
		public const string FIELD_ORDEREXTENDSETTING_DISPLAY_KBN = "display_kbn";                   // 表示区分
		public const string FIELD_ORDEREXTENDSETTING_DATE_CREATED = "date_created";                 // 作成日
		public const string FIELD_ORDEREXTENDSETTING_DATE_CHANGED = "date_changed";                 // 更新日
		public const string FIELD_ORDEREXTENDSETTING_LAST_CHANGED = "last_changed";                 // 最終更新者

		// 注文拡張ステータス設定マスタ
		public const string TABLE_ORDEREXTENDSTATUSSETTING = "w2_OrderExtendStatusSetting";         // 注文拡張ステータス設定マスタ
		public const string FIELD_ORDEREXTENDSTATUSSETTING_SHOP_ID = "shop_id";                     // 店舗ID
		public const string FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO = "extend_status_no";   // 拡張ステータス番号
		public const string FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME = "extend_status_name";// 拡張ステータス名称
		public const string FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_DISCRIPTION = "extend_status_discription";// 拡張ステータス説明
		public const string FIELD_ORDEREXTENDSTATUSSETTING_DATE_CREATED = "date_created";           // 作成日
		public const string FIELD_ORDEREXTENDSTATUSSETTING_DATE_CHANGED = "date_changed";           // 更新日
		public const string FIELD_ORDEREXTENDSTATUSSETTING_LAST_CHANGED = "last_changed";           // 最終更新者

		// マスタ出力定義
		public const string TABLE_MASTEREXPORTSETTING = "w2_MasterExportSetting";                   // マスタ出力定義
		public const string FIELD_MASTEREXPORTSETTING_SHOP_ID = "shop_id";                          // 店舗ID
		public const string FIELD_MASTEREXPORTSETTING_MASTER_KBN = "master_kbn";                    // マスタ区分
		public const string FIELD_MASTEREXPORTSETTING_SETTING_ID = "setting_id";                    // 設定ID
		public const string FIELD_MASTEREXPORTSETTING_SETTING_NAME = "setting_name";                // 設定名
		public const string FIELD_MASTEREXPORTSETTING_FIELDS = "fields";                            // フィールド列
		public const string FIELD_MASTEREXPORTSETTING_DEL_FLG = "del_flg";                          // 削除フラグ
		public const string FIELD_MASTEREXPORTSETTING_DATE_CREATED = "date_created";                // 作成日
		public const string FIELD_MASTEREXPORTSETTING_DATE_CHANGED = "date_changed";                // 更新日
		public const string FIELD_MASTEREXPORTSETTING_LAST_CHANGED = "last_changed";                // 最終更新者
		public const string FIELD_MASTEREXPORTSETTING_EXPORT_FILE_TYPE = "export_file_type";        // マスタ出力定義

		// SEOメタデータマスタ
		public const string TABLE_SEOMETADATAS = "w2_SeoMetadatas";                                 // SEOメタデータマスタ
		public const string FIELD_SEOMETADATAS_SHOP_ID = "shop_id";                                 // 店舗ID
		public const string FIELD_SEOMETADATAS_DATA_KBN = "data_kbn";                               // データ区分
		public const string FIELD_SEOMETADATAS_HTML_TITLE = "html_title";                           // タイトル
		public const string FIELD_SEOMETADATAS_METADATA_KEYWORDS = "metadata_keywords";             // キーワード
		public const string FIELD_SEOMETADATAS_METADATA_DESC = "metadata_desc";                     // ディスクリプション
		public const string FIELD_SEOMETADATAS_COMMENT = "comment";                                 // コメント
		public const string FIELD_SEOMETADATAS_DEL_FLG = "del_flg";                                 // 削除フラグ
		public const string FIELD_SEOMETADATAS_DATE_CREATED = "date_created";                       // 作成日
		public const string FIELD_SEOMETADATAS_DATE_CHANGED = "date_changed";                       // 更新日
		public const string FIELD_SEOMETADATAS_LAST_CHANGED = "last_changed";                       // 最終更新者
		public const string FIELD_SEOMETADATAS_SEO_TEXT = "seo_text";                              // SEO文言
		public const string FIELD_SEOMETADATAS_DEFAULT_TEXT = "default_text";                       // デフォルト文言

		// 採番マスタ
		public const string TABLE_NUMBER = "w2_Number";                                             // 採番マスタ
		public const string FIELD_NUMBER_DEPT_ID = "dept_id";                                       // 識別ID
		public const string FIELD_NUMBER_NUMBER_CODE = "number_code";                               // 採番コード
		public const string FIELD_NUMBER_NUMBER = "number";                                         // 保持値
		public const string FIELD_NUMBER_DATE_CREATED = "date_created";                             // 作成日
		public const string FIELD_NUMBER_DATE_CHANGED = "date_changed";                             // 更新日

		// 郵便番号マスタ
		public const string TABLE_ZIPCODE = "w2_Zipcode";                                           // 郵便番号マスタ
		public const string FIELD_ZIPCODE_LOCAL_PUB_CODE = "local_pub_code";                        // 全国地方公共団体コード
		public const string FIELD_ZIPCODE_ZIPCODE_OLD = "zipcode_old";                              // (旧)郵便番号
		public const string FIELD_ZIPCODE_ZIPCODE = "zipcode";                                      // 郵便番号
		public const string FIELD_ZIPCODE_PREFECTURE_KANA = "prefecture_kana";                      // 都道府県名かな
		public const string FIELD_ZIPCODE_CITY_KANA = "city_kana";                                  // 市区町村名かな
		public const string FIELD_ZIPCODE_TOWN_KANA = "town_kana";                                  // 町域名かな
		public const string FIELD_ZIPCODE_PREFECTURE = "prefecture";                                // 都道府県名
		public const string FIELD_ZIPCODE_CITY = "city";                                            // 市区町村名
		public const string FIELD_ZIPCODE_TOWN = "town";                                            // 町域名
		public const string FIELD_ZIPCODE_FLG1 = "flg1";                                            // フラグ1
		public const string FIELD_ZIPCODE_FLG2 = "flg2";                                            // フラグ2
		public const string FIELD_ZIPCODE_FLG3 = "flg3";                                            // フラグ3
		public const string FIELD_ZIPCODE_FLG4 = "flg4";                                            // フラグ4
		public const string FIELD_ZIPCODE_FLG5 = "flg5";                                            // フラグ5
		public const string FIELD_ZIPCODE_FLG6 = "flg6";                                            // フラグ6

		// 商品表示情報マスタ
		public const string TABLE_DISPPRODUCTINFO = "w2_DispProductInfo";                           // 商品表示情報マスタ
		public const string FIELD_DISPPRODUCTINFO_SHOP_ID = "shop_id";                              // 店舗ID
		public const string FIELD_DISPPRODUCTINFO_DATA_KBN = "data_kbn";                            // データ区分
		public const string FIELD_DISPPRODUCTINFO_DISPLAY_ORDER = "display_order";                  // 表示順
		public const string FIELD_DISPPRODUCTINFO_PRODUCT_ID = "product_id";                        // 商品ID
		public const string FIELD_DISPPRODUCTINFO_KBN1 = "kbn1";                                    // 区分1
		public const string FIELD_DISPPRODUCTINFO_KBN2 = "kbn2";                                    // 区分2
		public const string FIELD_DISPPRODUCTINFO_KBN3 = "kbn3";                                    // 区分3
		public const string FIELD_DISPPRODUCTINFO_KBN4 = "kbn4";                                    // 区分4
		public const string FIELD_DISPPRODUCTINFO_KBN5 = "kbn5";                                    // 区分5
		public const string FIELD_DISPPRODUCTINFO_DATE_CREATED = "date_created";                    // 作成日
		public const string FIELD_DISPPRODUCTINFO_LAST_CHANGED = "last_changed";                    // 最終更新者

		// 機種グループマスタ
		public const string TABLE_MOBILEGROUP = "w2_MobileGroup";                                   // 機種グループマスタ
		public const string FIELD_MOBILEGROUP_DEPT_ID = "dept_id";                                  // 識別ID
		public const string FIELD_MOBILEGROUP_CAREER_ID = "career_id";                              // キャリアID
		public const string FIELD_MOBILEGROUP_MOBILE_GROUP_ID = "mobile_group_id";                  // 機種グループID
		public const string FIELD_MOBILEGROUP_MOBILE_GROUP_NAME = "mobile_group_name";              // 機種グループ名
		public const string FIELD_MOBILEGROUP_MODEL_NO = "model_no";                                // 枝番
		public const string FIELD_MOBILEGROUP_MODEL_NAME = "model_name";                            // 機種名
		public const string FIELD_MOBILEGROUP_DEL_FLG = "del_flg";                                  // 削除フラグ
		public const string FIELD_MOBILEGROUP_DATE_CREATED = "date_created";                        // 作成日
		public const string FIELD_MOBILEGROUP_DATE_CHANGED = "date_changed";                        // 更新日
		public const string FIELD_MOBILEGROUP_LAST_CHANGED = "last_changed";                        // 最終更新者

		// モバイルページマスタ
		public const string TABLE_MOBILEPAGE = "w2_MobilePage";                                     // モバイルページマスタ
		public const string FIELD_MOBILEPAGE_DEPT_ID = "dept_id";                                   // 識別ID
		public const string FIELD_MOBILEPAGE_PAGE_ID = "page_id";                                   // ページID
		public const string FIELD_MOBILEPAGE_PAGE_NAME = "page_name";                               // ページ名
		public const string FIELD_MOBILEPAGE_CAREER_ID = "career_id";                               // キャリアID
		public const string FIELD_MOBILEPAGE_MOBILE_GROUP_ID = "mobile_group_id";                   // 機種グループID
		public const string FIELD_MOBILEPAGE_TITLE = "title";                                       // タイトル
		public const string FIELD_MOBILEPAGE_HTML = "html";                                         // HTML
		public const string FIELD_MOBILEPAGE_DEL_FLG = "del_flg";                                   // 削除フラグ
		public const string FIELD_MOBILEPAGE_DATE_CREATED = "date_created";                         // 作成日
		public const string FIELD_MOBILEPAGE_DATE_CHANGED = "date_changed";                         // 更新日
		public const string FIELD_MOBILEPAGE_LAST_CHANGED = "last_changed";                         // 最終更新者
		public const string FIELD_MOBILEPAGE_BRAND_ID = "brand_id";                                 // ブランドID

		// モバイルオリジナルタグマスタ
		public const string TABLE_MOBILEORIGINALTAG = "w2_MobileOriginalTag";                       // モバイルオリジナルタグマスタ
		public const string FIELD_MOBILEORIGINALTAG_DEPT_ID = "dept_id";                            // 識別ID
		public const string FIELD_MOBILEORIGINALTAG_ORGTAG_ID = "orgtag_id";                        // オリジナルタグID
		public const string FIELD_MOBILEORIGINALTAG_ORGTAG_NAME = "orgtag_name";                    // 表示名
		public const string FIELD_MOBILEORIGINALTAG_ORGTAG_HTML = "orgtag_html";                    // HTML
		public const string FIELD_MOBILEORIGINALTAG_DEL_FLG = "del_flg";                            // 削除フラグ
		public const string FIELD_MOBILEORIGINALTAG_DATE_CREATED = "date_created";                  // 作成日
		public const string FIELD_MOBILEORIGINALTAG_DATE_CHANGED = "date_changed";                  // 更新日
		public const string FIELD_MOBILEORIGINALTAG_LAST_CHANGED = "last_changed";                  // 最終更新者

		// モバイル絵文字マスタ
		public const string TABLE_MOBILEPICTORIALSYMBOL = "w2_MobilePictorialSymbol";               // モバイル絵文字マスタ
		public const string FIELD_MOBILEPICTORIALSYMBOL_DEPT_ID = "dept_id";                        // 識別ID
		public const string FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_ID = "symbol_id";                    // 絵文字ID
		public const string FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_TAG = "symbol_tag";                  // 絵文字識別名
		public const string FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_NAME = "symbol_name";                // 絵文字名
		public const string FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_CODE1 = "symbol_code1";              // 絵文字コード1
		public const string FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_CODE2 = "symbol_code2";              // 絵文字コード2
		public const string FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_CODE3 = "symbol_code3";              // 絵文字コード3
		public const string FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_CODE4 = "symbol_code4";              // 絵文字コード4
		public const string FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_CODE5 = "symbol_code5";              // 絵文字コード5
		public const string FIELD_MOBILEPICTORIALSYMBOL_SYMBOL_CODE6 = "symbol_code6";              // 絵文字コード6
		public const string FIELD_MOBILEPICTORIALSYMBOL_DEL_FLG = "del_flg";                        // 削除フラグ
		public const string FIELD_MOBILEPICTORIALSYMBOL_DATE_CREATED = "date_created";              // 作成日
		public const string FIELD_MOBILEPICTORIALSYMBOL_DATE_CHANGED = "date_changed";              // 更新日
		public const string FIELD_MOBILEPICTORIALSYMBOL_LAST_CHANGED = "last_changed";              // 最終更新者

		// モバイル機種情報マスタ
		public const string TABLE_MOBILEMODELINFO = "w2_MobileModelInfo";                           // モバイル機種情報マスタ
		public const string FIELD_MOBILEMODELINFO_CAREER = "career";                                // キャリア
		public const string FIELD_MOBILEMODELINFO_MODEL_NAME = "model_name";                        // 機種名
		public const string FIELD_MOBILEMODELINFO_MAKER = "maker";                                  // メーカー
		public const string FIELD_MOBILEMODELINFO_NICKNAME = "nickname";                            // ニックネーム
		public const string FIELD_MOBILEMODELINFO_SALE_DATE = "sale_date";                          // 発売日
		public const string FIELD_MOBILEMODELINFO_SERIES = "series";                                // シリーズ
		public const string FIELD_MOBILEMODELINFO_MARKUP_LANGUAGE = "markup_language";              // マークアップ言語
		public const string FIELD_MOBILEMODELINFO_BROWSER_VER = "browser_ver";                      // ブラウザバージョン
		public const string FIELD_MOBILEMODELINFO_TRANSMISSION_SPEED = "transmission_speed";        // 通信速度
		public const string FIELD_MOBILEMODELINFO_IMG_GRADATION = "img_gradation";                  // 画像階調
		public const string FIELD_MOBILEMODELINFO_DISP_GIF_FLG = "disp_gif_flg";                    // GIF
		public const string FIELD_MOBILEMODELINFO_DISP_JPEG_FLG = "disp_jpeg_flg";                  // JPEG
		public const string FIELD_MOBILEMODELINFO_DISP_PNG_FLG = "disp_png_flg";                    // PNG
		public const string FIELD_MOBILEMODELINFO_DISP_BMP2_FLG = "disp_bmp2_flg";                  // BMP2
		public const string FIELD_MOBILEMODELINFO_DISP_BMP4_FLG = "disp_bmp4_flg";                  // BMP4
		public const string FIELD_MOBILEMODELINFO_DISP_MNG_FLG = "disp_mng_flg";                    // MNG
		public const string FIELD_MOBILEMODELINFO_DISP_GIF_ANIME_FLG = "disp_gif_anime_flg";        // アニメーションGIF
		public const string FIELD_MOBILEMODELINFO_DISP_GIF_TRANS_FLG = "disp_gif_trans_flg";        // 透過GIF
		public const string FIELD_MOBILEMODELINFO_COMMUNICATION_METHOD = "communication_method";    // 通信方式
		public const string FIELD_MOBILEMODELINFO_SSL_FLG = "ssl_flg";                              // SSL対応
		public const string FIELD_MOBILEMODELINFO_CAMERA_PIXEL = "camera_pixel";                    // カメラの画素数
		public const string FIELD_MOBILEMODELINFO_CAMERA_PIXEL2 = "camera_pixel2";                  // 第二カメラの画素数
		public const string FIELD_MOBILEMODELINFO_APP_FLG = "app_flg";                              // アプリケーション対応
		public const string FIELD_MOBILEMODELINFO_APP_TYPE = "app_type";                            // アプリケーション種類
		public const string FIELD_MOBILEMODELINFO_APP_VER = "app_ver";                              // アプリケーションバージョン
		public const string FIELD_MOBILEMODELINFO_APP_SIZE = "app_size";                            // アプリケーション最大容量
		public const string FIELD_MOBILEMODELINFO_MEMORY_TYPE = "memory_type";                      // 外部メモリースロットとメディアの種類
		public const string FIELD_MOBILEMODELINFO_CHORDS_NUM = "chords_num";                        // 和音数
		public const string FIELD_MOBILEMODELINFO_CHAKUUTA_FLG = "chakuuta_flg";                    // 着うた対応
		public const string FIELD_MOBILEMODELINFO_CHAKUUTA_FULL_FLG = "chakuuta_full_flg";          // 着うたフル対応
		public const string FIELD_MOBILEMODELINFO_CHAKUMOVIE_FLG = "chakumovie_flg";                // 着ムービー対応
		public const string FIELD_MOBILEMODELINFO_QR_FLG = "qr_flg";                                // QR対応
		public const string FIELD_MOBILEMODELINFO_FELICA_FLG = "felica_flg";                        // Felica対応
		public const string FIELD_MOBILEMODELINFO_BLUETOOTH_FLG = "bluetooth_flg";                  // Bluetooth対応
		public const string FIELD_MOBILEMODELINFO_FLASH_FLG = "flash_flg";                          // FLASH対応
		public const string FIELD_MOBILEMODELINFO_ROOTCA_VERISIGN_FLG = "rootca_verisign_flg";      // ルートCA証明書VeriSign
		public const string FIELD_MOBILEMODELINFO_ROOTCA_ENTRUST_FLG = "rootca_entrust_flg";        // ルートCA証明書Entrust
		public const string FIELD_MOBILEMODELINFO_ROOTCA_CYBERTRUST_FLG = "rootca_cybertrust_flg";  // ルートCA証明書Cyber Trust
		public const string FIELD_MOBILEMODELINFO_ROOTCA_GEOTRUST_FLG = "rootca_geotrust_flg";      // ルートCA証明書Geotrust
		public const string FIELD_MOBILEMODELINFO_ROOTCA_RSASECURITY_FLG = "rootca_rsasecurity_flg";// ルートCA証明書RSA Security
		public const string FIELD_MOBILEMODELINFO_QVGA_FLG = "qvga_flg";                            // QVGA画面
		public const string FIELD_MOBILEMODELINFO_FULLBROWSER_FLG = "fullbrowser_flg";              // フルブラウザ対応
		public const string FIELD_MOBILEMODELINFO_INFRARED_RAYS_FLG = "infrared_rays_flg";          // 赤外線対応
		public const string FIELD_MOBILEMODELINFO_FILE_VIEWER_FLG = "file_viewer_flg";              // 添付ファイルビューアー対応
		public const string FIELD_MOBILEMODELINFO_GPS_FLG = "gps_flg";                              // GPS対応
		public const string FIELD_MOBILEMODELINFO_MOVIE_TYPE = "movie_type";                        // MOVIE形式
		public const string FIELD_MOBILEMODELINFO_FULLBROWSER_VER = "fullbrowser_ver";              // フルブラウザバージョン
		public const string FIELD_MOBILEMODELINFO_FULLBROWSER_UAGENT = "fullbrowser_uagent";        // フルブラウザUSER-AGENT
		public const string FIELD_MOBILEMODELINFO_MAIL_RCV_SIZE = "mail_rcv_size";                  // 添付受信サイズ
		public const string FIELD_MOBILEMODELINFO_FLASH_VER = "flash_ver";                          // Flashバージョン
		public const string FIELD_MOBILEMODELINFO_CACHE_SIZE = "cache_size";                        // キャッシュ容量
		public const string FIELD_MOBILEMODELINFO_ONESEG_FLG = "oneseg_flg";                        // ワンセグ対応
		public const string FIELD_MOBILEMODELINFO_TV_PHONE_FLG = "tv_phone_flg";                    // テレビ電話対応
		public const string FIELD_MOBILEMODELINFO_OFFICEDOC_DL_FLG = "officedoc_dl_flg";            // office文書WebDL
		public const string FIELD_MOBILEMODELINFO_OFFICEDOC_DISP_SIZE = "officedoc_disp_size";      // officeファイル表示可能サイズ
		public const string FIELD_MOBILEMODELINFO_COOLIE_FLG = "coolie_flg";                        // Cookie
		public const string FIELD_MOBILEMODELINFO_DEICEDOCE_SEND_FLG = "deicedoce_send_flg";        // 機体番号送信
		public const string FIELD_MOBILEMODELINFO_MOBILE_SUICA_FLG = "mobile_suica_flg";            // モバイルSuica
		public const string FIELD_MOBILEMODELINFO_MAIL_URL_SIZE = "mail_url_size";                  // メールURL長
		public const string FIELD_MOBILEMODELINFO_BOOKMARK_URL_SIZE = "bookmark_url_size";          // BookMarkURL長
		public const string FIELD_MOBILEMODELINFO_BROWSER_URL_SIZE = "browser_url_size";            // ブラウザURL長
		public const string FIELD_MOBILEMODELINFO_PUSHINFO_SEND_FLG = "pushinfo_send_flg";          // プッシュ型情報配信
		public const string FIELD_MOBILEMODELINFO_MAIL_SUBJECT_SIZE = "mail_subject_size";          // メール件名文字数
		public const string FIELD_MOBILEMODELINFO_MAIL_SEND_SIZE = "mail_send_size";                // 添付送信サイズ
		public const string FIELD_MOBILEMODELINFO_HTML_MAIL_FLG = "html_mail_flg";                  // HTMLメール
		public const string FIELD_MOBILEMODELINFO_KISEKAE_FLG = "kisekae_flg";                      // きせかえツール
		public const string FIELD_MOBILEMODELINFO_BROWSER_IMG_SIZE = "browser_img_size";            // ブラウザ画像サイズ
		public const string FIELD_MOBILEMODELINFO_INFO_UPDATE_DATE = "info_update_date";            // 最終更新日
		public const string FIELD_MOBILEMODELINFO_DATE_CREATED = "date_created";                    // 作成日
		public const string FIELD_MOBILEMODELINFO_DATE_CHANGED = "date_changed";                    // 更新日

		// モバイルユーザーエージェントマスタ
		public const string TABLE_MOBILEMODELUSERAGENT = "w2_MobileModelUserAgent";                 // モバイルユーザーエージェントマスタ
		public const string FIELD_MOBILEMODELUSERAGENT_CAREER = "career";                           // キャリア
		public const string FIELD_MOBILEMODELUSERAGENT_MODEL_NAME = "model_name";                   // 機種名
		public const string FIELD_MOBILEMODELUSERAGENT_DEVICE_NAME = "device_name";                 // デバイス名
		public const string FIELD_MOBILEMODELUSERAGENT_USER_AGENT = "user_agent";                   // USER_AGENT
		public const string FIELD_MOBILEMODELUSERAGENT_USER_AGENT_SCRAP = "user_agent_scrap";       // USER_AGENT_SCRAP
		public const string FIELD_MOBILEMODELUSERAGENT_INFO_UPDATE_DATE = "info_update_date";       // 最終更新日
		public const string FIELD_MOBILEMODELUSERAGENT_DATE_CREATED = "date_created";               // 作成日
		public const string FIELD_MOBILEMODELUSERAGENT_DATE_CHANGED = "date_changed";               // 更新日

		// モバイルIP設定マスタ
		public const string TABLE_MOBILEIPSETTING = "w2_MobileIPSetting";                           // モバイルIP設定マスタ
		public const string FIELD_MOBILEIPSETTING_IP_SETTING = "ip_setting";                        // IPアドレス
		public const string FIELD_MOBILEIPSETTING_DATE_CHANGED = "date_changed";                    // 更新日

		// ショートURLマスタ
		public const string TABLE_SHORTURL = "w2_ShortUrl";                                         // ショートURLマスタ
		public const string FIELD_SHORTURL_SURL_NO = "surl_no";                                     // ショートURL NO
		public const string FIELD_SHORTURL_SHOP_ID = "shop_id";                                     // 店舗ID
		public const string FIELD_SHORTURL_SHORT_URL = "short_url";                                 // ショートURL
		public const string FIELD_SHORTURL_LONG_URL = "long_url";                                   // ロングURL
		public const string FIELD_SHORTURL_DATE_CREATED = "date_created";                           // 作成日
		public const string FIELD_SHORTURL_DATE_CHANGED = "date_changed";                           // 更新日
		public const string FIELD_SHORTURL_LAST_CHANGED = "last_changed";                           // 最終更新者

		// ユーザモバイル登録状況
		public const string TABLE_USERMOBILEREGINFO = "w2_UserMobileRegInfo";                       // ユーザモバイル登録状況
		public const string FIELD_USERMOBILEREGINFO_CAREER_ID = "career_id";          // モバイルキャリアID
		public const string FIELD_USERMOBILEREGINFO_MOBILE_UID = "mobile_uid";                      // モバイルユーザID
		public const string FIELD_USERMOBILEREGINFO_SITE_ID = "site_id";                            // サイトID
		public const string FIELD_USERMOBILEREGINFO_REG_FLG = "reg_flg";                            // 登録フラグ
		public const string FIELD_USERMOBILEREGINFO_ATTRIBUTE1 = "attribute1";                      // 属性1
		public const string FIELD_USERMOBILEREGINFO_ATTRIBUTE2 = "attribute2";                      // 属性2
		public const string FIELD_USERMOBILEREGINFO_ATTRIBUTE3 = "attribute3";                      // 属性3
		public const string FIELD_USERMOBILEREGINFO_ATTRIBUTE4 = "attribute4";                      // 属性4
		public const string FIELD_USERMOBILEREGINFO_ATTRIBUTE5 = "attribute5";                      // 属性5
		public const string FIELD_USERMOBILEREGINFO_DATE_REG = "date_reg";                          // 登録日
		public const string FIELD_USERMOBILEREGINFO_DATE_UNREG = "date_unreg";                      // 登録解除日
		public const string FIELD_USERMOBILEREGINFO_LAST_CHANGED = "last_changed";                  // 最終更新者

		// ユーザモバイル登録状況ログ
		public const string TABLE_USERMOBILEREGINFOLOG = "w2_UserMobileRegInfoLog";                 // ユーザモバイル登録状況ログ
		public const string FIELD_USERMOBILEREGINFOLOG_LOG_NO = "log_no";                           // ログNO
		public const string FIELD_USERMOBILEREGINFOLOG_REG_DATE = "reg_date";                       // 日付
		public const string FIELD_USERMOBILEREGINFOLOG_REG_TIME = "reg_time";                       // 時間
		public const string FIELD_USERMOBILEREGINFOLOG_CAREER_ID = "career_id";       // モバイルキャリアID
		public const string FIELD_USERMOBILEREGINFOLOG_MOBILE_UID = "mobile_uid";                   // モバイルユーザID
		public const string FIELD_USERMOBILEREGINFOLOG_SITE_ID = "site_id";                         // サイトID
		public const string FIELD_USERMOBILEREGINFOLOG_REG_FLG = "reg_flg";                         // 登録フラグ
		public const string FIELD_USERMOBILEREGINFOLOG_ATTRIBUTE1 = "attribute1";                   // 属性1
		public const string FIELD_USERMOBILEREGINFOLOG_ATTRIBUTE2 = "attribute2";                   // 属性2
		public const string FIELD_USERMOBILEREGINFOLOG_ATTRIBUTE3 = "attribute3";                   // 属性3
		public const string FIELD_USERMOBILEREGINFOLOG_ATTRIBUTE4 = "attribute4";                   // 属性4
		public const string FIELD_USERMOBILEREGINFOLOG_ATTRIBUTE5 = "attribute5";                   // 属性5
		public const string FIELD_USERMOBILEREGINFOLOG_LAST_CHANGED = "last_changed";               // 最終更新者

		// 定期購入情報
		public const string TABLE_FIXEDPURCHASE = "w2_FixedPurchase";                               // 定期購入情報
		public const string FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID = "fixed_purchase_id";            // 定期購入ID
		public const string FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN = "fixed_purchase_kbn";          // 定期購入区分
		public const string FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1 = "fixed_purchase_setting1";// 定期購入設定１
		public const string FIELD_FIXEDPURCHASE_BASE_TEL_NO = "base_tel_no";                        // 元電話番号
		public const string FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS = "fixed_purchase_status";    // 定期購入ステータス
		public const string FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_KBN = "fixed_purchase_status_kbn";    // 定期購入ステータス区分
		public const string FIELD_FIXEDPURCHASE_PAYMENT_STATUS = "payment_status";                  // 決済ステータス
		public const string FIELD_FIXEDPURCHASE_LAST_ORDER_DATE = "last_order_date";                // 最終購入日
		public const string FIELD_FIXEDPURCHASE_ORDER_COUNT = "order_count";                        // 購入回数(注文基準)
		public const string FIELD_FIXEDPURCHASE_USER_ID = "user_id";                                // ユーザID
		public const string FIELD_FIXEDPURCHASE_SHOP_ID = "shop_id";                                // 店舗ID
		public const string FIELD_FIXEDPURCHASE_SUPPLIER_ID = "supplier_id";                        // サプライヤID
		public const string FIELD_FIXEDPURCHASE_ORDER_KBN = "order_kbn";                            // 注文区分
		public const string FIELD_FIXEDPURCHASE_ORDER_PAYMENT_KBN = "order_payment_kbn";            // 支払区分
		public const string FIELD_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN = "fixed_purchase_date_bgn";// 定期購入開始日時
		public const string FIELD_FIXEDPURCHASE_CARD_KBN = "card_kbn";                              // 決済カード区分
		public const string FIELD_FIXEDPURCHASE_VALID_FLG = "valid_flg";                            // 有効フラグ
		public const string FIELD_FIXEDPURCHASE_DEL_FLG = "del_flg";                                // 削除フラグ
		public const string FIELD_FIXEDPURCHASE_DATE_CREATED = "date_created";                      // 作成日
		public const string FIELD_FIXEDPURCHASE_DATE_CHANGED = "date_changed";                      // 更新日
		public const string FIELD_FIXEDPURCHASE_LAST_CHANGED = "last_changed";                      // 最終更新者
		public const string FIELD_FIXEDPURCHASE_CREDIT_BRANCH_NO = "credit_branch_no";              // クレジットカード枝番
		public const string FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE = "next_shipping_date";          // 次回配送日
		public const string FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE = "next_next_shipping_date";// 次々回配送日
		public const string FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO = "fixed_purchase_management_memo";// 定期購入管理メモ
		public const string FIELD_FIXEDPURCHASE_CARD_INSTALLMENTS_CODE = "card_installments_code";  // カード支払い回数コード
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS1 = "extend_status1";                  // 拡張ステータス1
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS2 = "extend_status2";                  // 拡張ステータス2
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS3 = "extend_status3";                  // 拡張ステータス3
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS4 = "extend_status4";                  // 拡張ステータス4
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS5 = "extend_status5";                  // 拡張ステータス5
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS6 = "extend_status6";                  // 拡張ステータス6
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS7 = "extend_status7";                  // 拡張ステータス7
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS8 = "extend_status8";                  // 拡張ステータス8
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS9 = "extend_status9";                  // 拡張ステータス9
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS10 = "extend_status10";                // 拡張ステータス10
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS11 = "extend_status11";                // 拡張ステータス11
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS12 = "extend_status12";                // 拡張ステータス12
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS13 = "extend_status13";                // 拡張ステータス13
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS14 = "extend_status14";                // 拡張ステータス14
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS15 = "extend_status15";                // 拡張ステータス15
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS16 = "extend_status16";                // 拡張ステータス16
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS17 = "extend_status17";                // 拡張ステータス17
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS18 = "extend_status18";                // 拡張ステータス18
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS19 = "extend_status19";                // 拡張ステータス19
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS20 = "extend_status20";                // 拡張ステータス20
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS21 = "extend_status21";                // 拡張ステータス21
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS22 = "extend_status22";                // 拡張ステータス22
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS23 = "extend_status23";                // 拡張ステータス23
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS24 = "extend_status24";                // 拡張ステータス24
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS25 = "extend_status25";                // 拡張ステータス25
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS26 = "extend_status26";                // 拡張ステータス26
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS27 = "extend_status27";                // 拡張ステータス27
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS28 = "extend_status28";                // 拡張ステータス28
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS29 = "extend_status29";                // 拡張ステータス29
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS30 = "extend_status30";                // 拡張ステータス30
		public const string FIELD_FIXEDPURCHASE_CANCEL_REASON_ID = "cancel_reason_id";              // 解約理由区分ID
		public const string FIELD_FIXEDPURCHASE_CANCEL_MEMO = "cancel_memo";                        // 解約メモ
		public const string FIELD_FIXEDPURCHASE_SHIPPED_COUNT = "shipped_count";                    // 購入回数(出荷基準)
		public const string FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT = "next_shipping_use_point";// 次回購入の利用ポイント数
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS31 = "extend_status31";                // 拡張ステータス31
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS32 = "extend_status32";                // 拡張ステータス32
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS33 = "extend_status33";                // 拡張ステータス33
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS34 = "extend_status34";                // 拡張ステータス34
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS35 = "extend_status35";                // 拡張ステータス35
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS36 = "extend_status36";                // 拡張ステータス36
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS37 = "extend_status37";                // 拡張ステータス37
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS38 = "extend_status38";                // 拡張ステータス38
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS39 = "extend_status39";                // 拡張ステータス39
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS40 = "extend_status40";                // 拡張ステータス40
		public const string FIELD_FIXEDPURCHASE_COMBINED_ORG_FIXEDPURCHASE_IDS = "combined_org_fixedpurchase_ids";// 定期購入同梱元定期購入ID
		public const string FIELD_FIXEDPURCHASE_ACCESS_COUNTRY_ISO_CODE = "access_country_iso_code";// アクセス国ISOコード
		public const string FIELD_FIXEDPURCHASE_DISP_LANGUAGE_CODE = "disp_language_code";          // 表示言語コード
		public const string FIELD_FIXEDPURCHASE_DISP_LANGUAGE_LOCALE_ID = "disp_language_locale_id";// 表示言語ロケールID
		public const string FIELD_FIXEDPURCHASE_DISP_CURRENCY_CODE = "disp_currency_code";          // 表示通貨コード
		public const string FIELD_FIXEDPURCHASE_DISP_CURRENCY_LOCALE_ID = "disp_currency_locale_id";// 表示通貨ロケールID
		public const string FIELD_FIXEDPURCHASE_EXTERNAL_PAYMENT_AGREEMENT_ID = "external_payment_agreement_id";// 外部支払契約ID
		public const string FIELD_FIXEDPURCHASE_RESUME_DATE = "resume_date";                        // 定期再開予定日
		public const string FIELD_FIXEDPURCHASE_SUSPEND_REASON = "suspend_reason";                  // 休止理由
		public const string FIELD_FIXEDPURCHASE_SHIPPING_MEMO = "shipping_memo";                    // 配送メモ
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE1 = "extend_status_date1";        // 拡張ステータス更新日1
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE2 = "extend_status_date2";        // 拡張ステータス更新日2
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE3 = "extend_status_date3";        // 拡張ステータス更新日3
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE4 = "extend_status_date4";        // 拡張ステータス更新日4
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE5 = "extend_status_date5";        // 拡張ステータス更新日5
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE6 = "extend_status_date6";        // 拡張ステータス更新日6
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE7 = "extend_status_date7";        // 拡張ステータス更新日7
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE8 = "extend_status_date8";        // 拡張ステータス更新日8
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE9 = "extend_status_date9";        // 拡張ステータス更新日9
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE10 = "extend_status_date10";      // 拡張ステータス更新日10
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE11 = "extend_status_date11";      // 拡張ステータス更新日11
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE12 = "extend_status_date12";      // 拡張ステータス更新日12
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE13 = "extend_status_date13";      // 拡張ステータス更新日13
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE14 = "extend_status_date14";      // 拡張ステータス更新日14
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE15 = "extend_status_date15";      // 拡張ステータス更新日15
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE16 = "extend_status_date16";      // 拡張ステータス更新日16
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE17 = "extend_status_date17";      // 拡張ステータス更新日17
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE18 = "extend_status_date18";      // 拡張ステータス更新日18
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE19 = "extend_status_date19";      // 拡張ステータス更新日19
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE20 = "extend_status_date20";      // 拡張ステータス更新日20
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE21 = "extend_status_date21";      // 拡張ステータス更新日21
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE22 = "extend_status_date22";      // 拡張ステータス更新日22
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE23 = "extend_status_date23";      // 拡張ステータス更新日23
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE24 = "extend_status_date24";      // 拡張ステータス更新日24
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE25 = "extend_status_date25";      // 拡張ステータス更新日25
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE26 = "extend_status_date26";      // 拡張ステータス更新日26
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE27 = "extend_status_date27";      // 拡張ステータス更新日27
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE28 = "extend_status_date28";      // 拡張ステータス更新日28
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE29 = "extend_status_date29";      // 拡張ステータス更新日29
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE30 = "extend_status_date30";      // 拡張ステータス更新日30
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE31 = "extend_status_date31";      // 拡張ステータス更新日31
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE32 = "extend_status_date32";      // 拡張ステータス更新日32
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE33 = "extend_status_date33";      // 拡張ステータス更新日33
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE34 = "extend_status_date34";      // 拡張ステータス更新日34
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE35 = "extend_status_date35";      // 拡張ステータス更新日35
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE36 = "extend_status_date36";      // 拡張ステータス更新日36
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE37 = "extend_status_date37";      // 拡張ステータス更新日37
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE38 = "extend_status_date38";      // 拡張ステータス更新日38
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE39 = "extend_status_date39";      // 拡張ステータス更新日39
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE40 = "extend_status_date40";      // 拡張ステータス更新日40
		public const string FIELD_FIXEDPURCHASE_CANCEL_DATE = "cancel_date";                        // 解約日
		public const string FIELD_FIXEDPURCHASE_RESTART_DATE = "restart_date";                      // 再開日
		public const string FIELD_FIXEDPURCHASE_MEMO = "memo";                                      // メモ
		public const string FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_COUPON_ID = "next_shipping_use_coupon_id";// 次回購入の利用クーポンID
		public const string FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_COUPON_NO = "next_shipping_use_coupon_no";// 次回購入の利用クーポン枝番
		public const string FIELD_FIXEDPURCHASE_RECEIPT_FLG = "receipt_flg";                        // 領収書希望フラグ
		public const string FIELD_FIXEDPURCHASE_RECEIPT_ADDRESS = "receipt_address";                // 宛名
		public const string FIELD_FIXEDPURCHASE_RECEIPT_PROVISO = "receipt_proviso";                // 但し書き
		public const string FIELD_FIXEDPURCHASE_SKIPPED_COUNT = "skipped_count";                    // スキップ回数
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS41 = "extend_status41";                // 拡張ステータス41
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS42 = "extend_status42";                // 拡張ステータス42
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS43 = "extend_status43";                // 拡張ステータス43
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS44 = "extend_status44";                // 拡張ステータス44
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS45 = "extend_status45";                // 拡張ステータス45
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS46 = "extend_status46";                // 拡張ステータス46
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS47 = "extend_status47";                // 拡張ステータス47
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS48 = "extend_status48";                // 拡張ステータス48
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS49 = "extend_status49";                // 拡張ステータス49
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS50 = "extend_status50";                // 拡張ステータス50
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE41 = "extend_status_date41";      // 拡張ステータス更新日時41
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE42 = "extend_status_date42";      // 拡張ステータス更新日時42
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE43 = "extend_status_date43";      // 拡張ステータス更新日時43
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE44 = "extend_status_date44";      // 拡張ステータス更新日時44
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE45 = "extend_status_date45";      // 拡張ステータス更新日時45
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE46 = "extend_status_date46";      // 拡張ステータス更新日時46
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE47 = "extend_status_date47";      // 拡張ステータス更新日時47
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE48 = "extend_status_date48";      // 拡張ステータス更新日時48
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE49 = "extend_status_date49";      // 拡張ステータス更新日時49
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE50 = "extend_status_date50";      // 拡張ステータス更新日時50
		public const string FIELD_FIXEDPURCHASE_ATTRIBUTE1 = "attribute1";                          // 注文拡張項目1
		public const string FIELD_FIXEDPURCHASE_ATTRIBUTE2 = "attribute2";                          // 注文拡張項目2
		public const string FIELD_FIXEDPURCHASE_ATTRIBUTE3 = "attribute3";                          // 注文拡張項目3
		public const string FIELD_FIXEDPURCHASE_ATTRIBUTE4 = "attribute4";                          // 注文拡張項目4
		public const string FIELD_FIXEDPURCHASE_ATTRIBUTE5 = "attribute5";                          // 注文拡張項目5
		public const string FIELD_FIXEDPURCHASE_ATTRIBUTE6 = "attribute6";                          // 注文拡張項目6
		public const string FIELD_FIXEDPURCHASE_ATTRIBUTE7 = "attribute7";                          // 注文拡張項目7
		public const string FIELD_FIXEDPURCHASE_ATTRIBUTE8 = "attribute8";                          // 注文拡張項目8
		public const string FIELD_FIXEDPURCHASE_ATTRIBUTE9 = "attribute9";                          // 注文拡張項目9
		public const string FIELD_FIXEDPURCHASE_ATTRIBUTE10 = "attribute10";                        // 注文拡張項目10
		public const string FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_COURSE_ID = "subscription_box_course_id";// 頒布会コースID
		public const string FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_ORDER_COUNT = "subscription_box_order_count";// 頒布会コース注文回数
		public const string FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_FIXED_AMOUNT = "subscription_box_fixed_amount"; // 頒布会定額価格
		/// <summary> 次回購入の利用ポイントの全適用フラグ </summary>
		public const string FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG = "use_all_point_flg";

		// 定期購入テーブル付随して作成した定数
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_BASENAME = "extend_status";           // 拡張ステータスベース名
		public const string FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE_BASENAME = "extend_status_date";       // 拡張ステータス更新日ベース名

		//データ結合マスタ出力変換用定数
		public const string FIELD_USER_EXTEND_CONVERTING_NAME = "userextend_";
		public const string FIELD_USER_ATTRIBUTE_CONVERTING_NAME = "userattribute_";
		public const string FIELD_PRODUCT_TAG_CONVERTING_NAME = "producttag_";
		public const string FIELD_PRODUCT_EXTEND_CONVERTING_NAME = "productextend_";
		public const string FIELD_FIXEDPURCHASE_CONVERTING_NAME = "fixedpurchase_";
		public const string FIELD_ORDER_CONVERTING_NAME = "order_";
		public const string FIELD_ORDER_SETTING_CONVERTING_NAME = "ordersetting_";
		public const string FIELD_ORDER_EXTEND_SETTING_CONVERTING_NAME = "orderextendsetting_";
		public const string FIELD_ORDER_SETPROMOTION_CONVERTING_NAME = "ordersetpromotion_";
		public const string FIELD_PRODUCT_STOCK_CONVERTING_NAME = "productstock_";
		//オプション別項目チェック用定数
		public const string FIELD_ORDER_SETPROMOTION_OPTION_CHECK_NAME = "order_setpromotion_";
		public const string FIELD_FIXEDPURCHASE_OPTION_CHECK_NAME = "fixedpurchase_";
		public const string FIELD_FIXED_PURCHASE_OPTION_CHECK_NAME = "fixed_purchase_";
		public const string FIELD_FIXEDPURCHASE_ITEM_OPTION_CHECK_NAME = "fixedpurchaseitem_";

		// 定期購入商品情報
		public const string TABLE_FIXEDPURCHASEITEM = "w2_FixedPurchaseItem";                       // 定期購入商品情報
		public const string FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ID = "fixed_purchase_id";        // 定期購入ID
		public const string FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ITEM_NO = "fixed_purchase_item_no";// 定期購入注文商品枝番
		public const string FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_SHIPPING_NO = "fixed_purchase_shipping_no";// 定期購入配送先枝番
		public const string FIELD_FIXEDPURCHASEITEM_SHOP_ID = "shop_id";                            // 店舗ID
		public const string FIELD_FIXEDPURCHASEITEM_PRODUCT_ID = "product_id";                      // 商品ID
		public const string FIELD_FIXEDPURCHASEITEM_VARIATION_ID = "variation_id";                  // 商品バリエーションID
		public const string FIELD_FIXEDPURCHASEITEM_SUPPLIER_ID = "supplier_id";                    // サプライヤID
		public const string FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY = "item_quantity";                // 注文数
		public const string FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY_SINGLE = "item_quantity_single";  // 注文数（セット未考慮）
		public const string FIELD_FIXEDPURCHASEITEM_PRODUCT_SET_ID = "product_set_id";              // 商品セットID
		public const string FIELD_FIXEDPURCHASEITEM_PRODUCT_SET_NO = "product_set_no";              // 商品セット枝番
		public const string FIELD_FIXEDPURCHASEITEM_PRODUCT_SET_COUNT = "product_set_count";        // 商品セット注文数
		public const string FIELD_FIXEDPURCHASEITEM_DATE_CREATED = "date_created";                  // 作成日
		public const string FIELD_FIXEDPURCHASEITEM_DATE_CHANGED = "date_changed";                  // 更新日
		public const string FIELD_FIXEDPURCHASEITEM_PRODUCT_OPTION_TEXTS = "product_option_texts";  // 商品付帯情報選択値
		public const string FIELD_FIXEDPURCHASEITEM_ITEM_ORDER_COUNT = "item_order_count";			// 商品購入回数(注文基準)
		public const string FIELD_FIXEDPURCHASEITEM_ITEM_SHIPPED_COUNT = "item_shipped_count";		// 商品購入回数(出荷基準)

		// 定期購入配送先情報
		public const string TABLE_FIXEDPURCHASESHIPPING = "w2_FixedPurchaseShipping";               // 定期購入配送先情報
		public const string FIELD_FIXEDPURCHASESHIPPING_FIXED_PURCHASE_ID = "fixed_purchase_id";    // 定期購入ID
		public const string FIELD_FIXEDPURCHASESHIPPING_FIXED_PURCHASE_SHIPPING_NO = "fixed_purchase_shipping_no";// 定期購入配送先枝番
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME = "shipping_name";            // 配送先氏名
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME_KANA = "shipping_name_kana";  // 配送先氏名かな
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ZIP = "shipping_zip";              // 郵便番号
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR1 = "shipping_addr1";          // 住所1
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR2 = "shipping_addr2";          // 住所2
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR3 = "shipping_addr3";          // 住所3
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR4 = "shipping_addr4";          // 住所４
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TEL1 = "shipping_tel1";            // 電話番号1
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TEL2 = "shipping_tel2";            // 電話番号2
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TEL3 = "shipping_tel3";            // 電話番号3
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_FAX = "shipping_fax";              // ＦＡＸ
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_COMPANY = "shipping_company";      // 配送業者
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_DATE = "shipping_date";            // 配送希望日
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TIME = "shipping_time";            // 配送希望時間帯
		public const string FIELD_FIXEDPURCHASESHIPPING_DEL_FLG = "del_flg";                        // 削除フラグ
		public const string FIELD_FIXEDPURCHASESHIPPING_DATE_CREATED = "date_created";              // 作成日
		public const string FIELD_FIXEDPURCHASESHIPPING_DATE_CHANGED = "date_changed";              // 更新日
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME1 = "shipping_name1";          // 配送先氏名1
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME2 = "shipping_name2";          // 配送先氏名2
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME_KANA1 = "shipping_name_kana1";// 配送先氏名かな1
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_NAME_KANA2 = "shipping_name_kana2";// 配送先氏名かな2
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_COMPANY_NAME = "shipping_company_name";// 配送先企業名
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_COMPANY_POST_NAME = "shipping_company_post_name";// 配送先部署名
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_METHOD = "shipping_method";        // 配送方法
		public const string FIELD_FIXEDPURCHASESHIPPING_DELIVERY_COMPANY_ID = "delivery_company_id";// 配送会社ID
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_COUNTRY_ISO_CODE = "shipping_country_iso_code";// 住所国ISOコード
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_COUNTRY_NAME = "shipping_country_name";// 住所国名
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR5 = "shipping_addr5";          // 住所5
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_RECEIVING_STORE_FLG = "shipping_receiving_store_flg";// 店舗受取フラグ
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_RECEIVING_STORE_ID = "shipping_receiving_store_id";// 店舗受取店舗ID
		/// <summary>コンビニ受取：受取方法</summary>
		public const string FIELD_FIXEDPURCHASESHIPPING_SHIPPING_RECEIVING_STORE_TYPE = "shipping_receiving_store_type";

		// 定期購入履歴情報
		public const string TABLE_FIXEDPURCHASEHISTORY = "w2_FixedPurchaseHistory";                 // 定期購入履歴情報
		public const string FIELD_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_ID = "fixed_purchase_id";     // 定期購入ID
		public const string FIELD_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_NO = "fixed_purchase_history_no";// 定期購入注文履歴NO
		public const string FIELD_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN = "fixed_purchase_history_kbn";// 定期購入履歴区分
		public const string FIELD_FIXEDPURCHASEHISTORY_BASE_TEL_NO = "base_tel_no";                 // 元電話番号
		public const string FIELD_FIXEDPURCHASEHISTORY_USER_ID = "user_id";                         // ユーザID
		public const string FIELD_FIXEDPURCHASEHISTORY_ORDER_ID = "order_id";                       // 注文ID
		public const string FIELD_FIXEDPURCHASEHISTORY_DATE_CREATED = "date_created";               // 作成日
		public const string FIELD_FIXEDPURCHASEHISTORY_LAST_CHANGED = "last_changed";               // 最終更新者
		public const string FIELD_FIXEDPURCHASEHISTORY_UPDATE_ORDER_COUNT = "update_order_count";   // 購入回数(注文基準)更新
		public const string FIELD_FIXEDPURCHASEHISTORY_UPDATE_SHIPPED_COUNT = "update_shipped_count";// 購入回数(出荷基準)更新
		public const string FIELD_FIXEDPURCHASEHISTORY_UPDATE_ORDER_COUNT_RESULT = "update_order_count_result";// 購入回数(注文基準)更新結果
		public const string FIELD_FIXEDPURCHASEHISTORY_UPDATE_SHIPPED_COUNT_RESULT = "update_shipped_count_result";// 購入回数(出荷基準)更新結果
		public const string FIELD_FIXEDPURCHASEHISTORY_EXTERNAL_PAYMENT_COOPERATION_LOG = "external_payment_cooperation_log"; //外部決済連携ログ

		// 定期購入バッチメールの一時ログ
		public const string TABLE_FIXEDPURCHASEBATCHMAILTMPLOG = "w2_FixedPurchaseBatchMailTmpLog"; // 定期購入バッチメールの一時ログ
		public const string FIELD_FIXEDPURCHASEBATCHMAILTMPLOG_TMP_LOG_ID = "tmp_log_id";           // 一時ログID
		public const string FIELD_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_TYPE = "master_type";         // マスタ種別
		public const string FIELD_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_ID = "master_id";             // マスタID
		public const string FIELD_FIXEDPURCHASEBATCHMAILTMPLOG_ACTION_MASTER_ID = "action_master_id";// 実行マスタID

		// 定期解約理由区分設定
		public const string TABLE_FIXEDPURCHASECANCELREASON = "w2_FixedPurchaseCancelReason";       // 定期解約理由区分設定
		public const string FIELD_FIXEDPURCHASECANCELREASON_CANCEL_REASON_ID = "cancel_reason_id";  // 解約理由区分ID
		public const string FIELD_FIXEDPURCHASECANCELREASON_CANCEL_REASON_NAME = "cancel_reason_name";// 解約理由区分名
		public const string FIELD_FIXEDPURCHASECANCELREASON_DISPLAY_ORDER = "display_order";        // 表示順
		public const string FIELD_FIXEDPURCHASECANCELREASON_DISPLAY_KBN = "display_kbn";                // 表示区分
		public const string FIELD_FIXEDPURCHASECANCELREASON_DATE_CREATED = "date_created";          // 作成日
		public const string FIELD_FIXEDPURCHASECANCELREASON_DATE_CHANGED = "date_changed";          // 更新日
		public const string FIELD_FIXEDPURCHASECANCELREASON_LAST_CHANGED = "last_changed";          // 更新者

		// Regular status : All regular , Normal regular , subscription box regular
		public const string FIELD_FIXEDPURCHASE_REGULAR_STATUS = "regular_type";    // Regular status

		/// <summary>定期商品変更設定</summary>
		public const string TABLE_FIXEDPURCHASEPRODUCTCHANGESETTING = "w2_FixedPurchaseProductChangeSetting";
		/// <summary>定期商品変更ID</summary>
		public const string FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID = "fixed_purchase_product_change_id";
		/// <summary>定期商品変更名</summary>
		public const string FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_NAME = "fixed_purchase_product_change_name";
		/// <summary>適用優先順</summary>
		public const string FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_PRIORITY = "priority";
		/// <summary>有効フラグ</summary>
		public const string FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG = "valid_flg";
		/// <summary>作成日</summary>
		public const string FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_DATE_CREATED = "date_created";
		/// <summary>更新日</summary>
		public const string FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_DATE_CHANGED = "date_changed";
		/// <summary>最終更新者</summary>
		public const string FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_LAST_CHANGED = "last_changed";

		/// <summary>定期変更元商品</summary>
		public const string TABLE_FIXEDPURCHASEBEFORECHANGEITEM = "w2_FixedPurchaseBeforeChangeItem";
		/// <summary>定期商品変更ID</summary>
		public const string FIELD_FIXEDPURCHASEBEFORECHANGEITEM_FIXED_PURCHASE_PRODUCT_CHANGE_ID = "fixed_purchase_product_change_id";
		/// <summary>商品単位種別</summary>
		public const string FIELD_FIXEDPURCHASEBEFORECHANGEITEM_ITEM_UNIT_TYPE = "item_unit_type";
		/// <summary>店舗ID</summary>
		public const string FIELD_FIXEDPURCHASEBEFORECHANGEITEM_SHOP_ID = "shop_id";
		/// <summary>商品ID</summary>
		public const string FIELD_FIXEDPURCHASEBEFORECHANGEITEM_PRODUCT_ID = "product_id";
		/// <summary>バリエーションID</summary>
		public const string FIELD_FIXEDPURCHASEBEFORECHANGEITEM_VARIATION_ID = "variation_id";
		/// <summary>作成日</summary>
		public const string FIELD_FIXEDPURCHASEBEFORECHANGEITEM_DATE_CREATED = "date_created";
		/// <summary>更新日</summary>
		public const string FIELD_FIXEDPURCHASEBEFORECHANGEITEM_DATE_CHANGED = "date_changed";
		/// <summary>最終更新者</summary>
		public const string FIELD_FIXEDPURCHASEBEFORECHANGEITEM_LAST_CHANGED = "last_changed";

		/// <summary>定期変更後商品</summary>
		public const string TABLE_FIXEDPURCHASEAFTERCHANGEITEM = "w2_FixedPurchaseAfterChangeItem";
		/// <summary>定期商品変更ID</summary>
		public const string FIELD_FIXEDPURCHASEAFTERCHANGEITEM_FIXED_PURCHASE_PRODUCT_CHANGE_ID = "fixed_purchase_product_change_id";
		/// <summary>商品単位種別</summary>
		public const string FIELD_FIXEDPURCHASEAFTERCHANGEITEM_ITEM_UNIT_TYPE = "item_unit_type";
		/// <summary>店舗ID</summary>
		public const string FIELD_FIXEDPURCHASEAFTERCHANGEITEM_SHOP_ID = "shop_id";
		/// <summary>商品ID</summary>
		public const string FIELD_FIXEDPURCHASEAFTERCHANGEITEM_PRODUCT_ID = "product_id";
		/// <summary>バリエーションID</summary>
		public const string FIELD_FIXEDPURCHASEAFTERCHANGEITEM_VARIATION_ID = "variation_id";
		/// <summary>作成日</summary>
		public const string FIELD_FIXEDPURCHASEAFTERCHANGEITEM_DATE_CREATED = "date_created";
		/// <summary>更新日</summary>
		public const string FIELD_FIXEDPURCHASEAFTERCHANGEITEM_DATE_CHANGED = "date_changed";
		/// <summary>最終更新者</summary>
		public const string FIELD_FIXEDPURCHASEAFTERCHANGEITEM_LAST_CHANGED = "last_changed";

		// 入荷通知メール情報
		public const string TABLE_USERPRODUCTARRIVALMAIL = "w2_UserProductArrivalMail";             // 入荷通知メール情報
		public const string FIELD_USERPRODUCTARRIVALMAIL_USER_ID = "user_id";						// ユーザID
		public const string FIELD_USERPRODUCTARRIVALMAIL_MAIL_NO = "mail_no";						// 枝番
		public const string FIELD_USERPRODUCTARRIVALMAIL_SHOP_ID = "shop_id";						// 店舗ID
		public const string FIELD_USERPRODUCTARRIVALMAIL_PRODUCT_ID = "product_id";					// 商品ID
		public const string FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID = "variation_id";			    // バリエーションID
		public const string FIELD_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN = "pcmobile_kbn";			    // PCモバイル区分
		public const string FIELD_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN = "arrival_mail_kbn";     // 入荷通知メール区分
		public const string FIELD_USERPRODUCTARRIVALMAIL_MAIL_SEND_STATUS = "mail_send_status";	    // 送信ステータス
		public const string FIELD_USERPRODUCTARRIVALMAIL_DATE_EXPIRED = "date_expired";             // 入荷通知メール期限
		public const string FIELD_USERPRODUCTARRIVALMAIL_GUEST_MAIL_ADDR = "guest_mail_addr";		// メールアドレス
		public const string FIELD_USERPRODUCTARRIVALMAIL_DATE_CREATED = "date_created";			    // 作成日
		public const string FIELD_USERPRODUCTARRIVALMAIL_DATE_CHANGED = "date_changed";			    // 更新日
		public const string FIELD_USERPRODUCTARRIVALMAIL_LAST_CHANGED = "last_changed";				// 最終更新者

		public const string FIELD_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_COUNT = "arrival_mail_count";	// 再入荷通知メール件数
		public const string FIELD_USERPRODUCTARRIVALMAIL_RELEASE_MAIL_COUNT = "release_mail_count";	// 販売開始通知メール件数
		public const string FIELD_USERPRODUCTARRIVALMAIL_RESALE_MAIL_COUNT = "resale_mail_count";	// 再販売通知メール件数

		// モール連携設定
		public const string TABLE_MALLCOOPERATIONSETTING = "w2_MallCooperationSetting";             // モール連携設定
		public const string FIELD_MALLCOOPERATIONSETTING_SHOP_ID = "shop_id";                       // 店舗ID
		public const string FIELD_MALLCOOPERATIONSETTING_MALL_ID = "mall_id";                       // モールID
		public const string FIELD_MALLCOOPERATIONSETTING_MALL_KBN = "mall_kbn";                     // モール区分
		public const string FIELD_MALLCOOPERATIONSETTING_MALL_NAME = "mall_name";                   // モール名
		public const string FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG = "mall_exhibits_config";// モール出品設定
		public const string FIELD_MALLCOOPERATIONSETTING_TGT_MAIL_ADDR = "tgt_mail_addr";           // 対象メールアドレス
		public const string FIELD_MALLCOOPERATIONSETTING_POP_SERVER = "pop_server";                 // 受信POPサーバ
		public const string FIELD_MALLCOOPERATIONSETTING_POP_PORT = "pop_port";                     // 受信POPポート
		public const string FIELD_MALLCOOPERATIONSETTING_POP_USER_NAME = "pop_user_name";           // 受信POPユーザ名
		public const string FIELD_MALLCOOPERATIONSETTING_POP_PASSWORD = "pop_password";             // 受信POPパスワード
		public const string FIELD_MALLCOOPERATIONSETTING_POP_APOP_FLG = "pop_apop_flg";             // 受信APOPフラグ
		public const string FIELD_MALLCOOPERATIONSETTING_FTP_HOST = "ftp_host";                     // FTPホスト名（アドレス）
		public const string FIELD_MALLCOOPERATIONSETTING_FTP_USER_NAME = "ftp_user_name";           // FTPユーザー名
		public const string FIELD_MALLCOOPERATIONSETTING_FTP_PASSWORD = "ftp_password";             // FTPパスワード
		public const string FIELD_MALLCOOPERATIONSETTING_FTP_UPLOAD_DIR = "ftp_upload_dir";         // FTPアップロード先ディレクトリ
		public const string FIELD_MALLCOOPERATIONSETTING_SFTP_HOST = "sftp_host";									// SFTPホスト名（アドレス）
		public const string FIELD_MALLCOOPERATIONSETTING_SFTP_USER_NAME = "sftp_user_name";							// SFTPユーザー名
		public const string FIELD_MALLCOOPERATIONSETTING_SFTP_PASS_PHRASE = "sftp_pass_phrase";						// SFTPパスフレーズ
		public const string FIELD_MALLCOOPERATIONSETTING_SFTP_PORT = "sftp_port";									// SFTPポート番号
		public const string FIELD_MALLCOOPERATIONSETTING_SFTP_PRIVATE_KEY_FILE_PATH = "sftp_private_key_file_path";	// SFTP秘密鍵ファイルパス
		public const string FIELD_MALLCOOPERATIONSETTING_ANDMALL_COOPERATION = "andmall_cooperation";// ＆mallの商品連携カラム
		public const string FIELD_MALLCOOPERATIONSETTING_ANDMALL_VARIATION_COOPERATION = "andmall_variation_cooperation";// ＆mallの商品バリエーション連携カラム
		public const string FIELD_MALLCOOPERATIONSETTING_ORDER_IMPORT_SETTING = "order_import_setting";// 受注情報取込設定
		public const string FIELD_MALLCOOPERATIONSETTING_OTHER_SETTING = "other_setting";			// その他情報設定
		public const string FIELD_MALLCOOPERATIONSETTING_RTN_MAIL_ADDR = "rtn_mail_addr";			// 店舗連絡先メールアドレス（楽天あんしんメルアドサービス）
		public const string FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_INS_ITEMCSV = "cnvid_rtn_n_ins_itemcsv";// 商品コンバータID：楽天1
		public const string FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_UPD_ITEMCSV = "cnvid_rtn_n_upd_itemcsv";// 商品コンバータID：楽天2
		public const string FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_STK_ITEMCSV = "cnvid_rtn_n_stk_itemcsv";// 商品コンバータID：楽天3
		public const string FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_INS_ITEMCSV = "cnvid_rtn_v_ins_itemcsv";// 商品コンバータID：楽天4
		public const string FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_INS_SELECTCSV = "cnvid_rtn_v_ins_selectcsv";// 商品コンバータID：楽天5
		public const string FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_UPD_ITEMCSV = "cnvid_rtn_v_upd_itemcsv";// 商品コンバータID：楽天6
		public const string FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_STK_SELECTCSV = "cnvid_rtn_v_stk_selectcsv";// 商品コンバータID：楽天8
		public const string FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_ITEMCATCSV = "cnvid_rtn_itemcatcsv";// 商品コンバータID：楽天9
		public const string FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_INS_DATACSV = "cnvid_yho_n_ins_datacsv";// 商品コンバータID：Yahoo!1
		public const string FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_INS_STOCKCSV = "cnvid_yho_n_ins_stockcsv";// 商品コンバータID：Yahoo!2
		public const string FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_UPD_DATACSV = "cnvid_yho_n_upd_datacsv";// 商品コンバータID：Yahoo!3
		public const string FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_STK_DATACSV = "cnvid_yho_n_stk_datacsv";// 商品コンバータID：Yahoo!4
		public const string FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_INS_DATACSV = "cnvid_yho_v_ins_datacsv";// 商品コンバータID：Yahoo!5
		public const string FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_INS_STOCKCSV = "cnvid_yho_v_ins_stockcsv";// 商品コンバータID：Yahoo!6
		public const string FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_UPD_DATACSV = "cnvid_yho_v_upd_datacsv";// 商品コンバータID：Yahoo!7
		public const string FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_STK_DATACSV = "cnvid_yho_v_stk_datacsv";// 商品コンバータID：Yahoo!8
		public const string FIELD_MALLCOOPERATIONSETTING_MAINTENANCE_DATE_FROM = "maintenance_date_from";// メンテナンス開始日
		public const string FIELD_MALLCOOPERATIONSETTING_MAINTENANCE_DATE_TO = "maintenance_date_to";// メンテナンス終了日
		public const string FIELD_MALLCOOPERATIONSETTING_VALID_FLG = "valid_flg";                   // 有効フラグ
		public const string FIELD_MALLCOOPERATIONSETTING_DEL_FLG = "del_flg";                       // 削除フラグ
		public const string FIELD_MALLCOOPERATIONSETTING_LAST_PRODUCT_LOG_NO = "last_product_log_no";// 最終商品マスタログNO
		public const string FIELD_MALLCOOPERATIONSETTING_LAST_PRODUCTVARIATION_LOG_NO = "last_productvariation_log_no";// 最終商品バリエーションマスタログNO
		public const string FIELD_MALLCOOPERATIONSETTING_LAST_PRODUCTSTOCK_LOG_NO = "last_productstock_log_no";// 最終商品在庫マスタログNO
		public const string FIELD_MALLCOOPERATIONSETTING_DATE_CREATED = "date_created";             // 作成日
		public const string FIELD_MALLCOOPERATIONSETTING_DATE_CHANGED = "date_changed";             // 更新日
		public const string FIELD_MALLCOOPERATIONSETTING_LAST_CHANGED = "last_changed";             // 最終更新者
		public const string FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_USER_NAME = "rakuten_api_user_name";			// 楽天APIのユーザ名
		public const string FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_SHOP_URL = "rakuten_api_shop_url";				// 楽天APIの店舗URL
		public const string FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_SERVICE_SECRET = "rakuten_api_service_secret";	// 楽天APIのサービスシークレット
		public const string FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_LICENSE_KEY = "rakuten_api_license_key";		// 楽天APIのライセンスキー
		public const string FIELD_MALLCOOPERATIONSETTING_AMAZON_MERCHANT_ID = "amazon_merchant_id";		// Amazon出品者ID
		public const string FIELD_MALLCOOPERATIONSETTING_AMAZON_MARKETPLACE_ID = "amazon_marketplace_id";		// AmazonマーケットプレイスID
		public const string FIELD_MALLCOOPERATIONSETTING_AMAZON_AWS_ACCESSKEY_ID = "amazon_aws_accesskey_id";		// AmazonAWSアクセスキーID
		public const string FIELD_MALLCOOPERATIONSETTING_AMAZON_SECRET_KEY = "amazon_secret_key";		// Amazon秘密キー
		public const string FIELD_MALLCOOPERATIONSETTING_AMAZON_MWS_AUTHTOKEN = "amazon_mws_authtoken";		// AmazonMWS認証トークン
		public const string FIELD_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG = "stock_update_use_flg";// 在庫連携利用フラグ
		public const string FIELD_MALLCOOPERATIONSETTING_ANDMALL_TENANT_CODE = "andmall_tenant_code";			// ＆mallのテナントコード
		public const string FIELD_MALLCOOPERATIONSETTING_ANDMALL_BASE_STORE_CODE = "andmall_base_store_code";	// ＆mallのショップID
		public const string FIELD_MALLCOOPERATIONSETTING_ANDMALL_SHOP_NO = "andmall_shop_no";					// ＆mallの店番
		public const string FIELD_MALLCOOPERATIONSETTING_ANDMALL_SITE_CODE = "andmall_site_code";				// ＆mallのサイトコード
		public const string FIELD_MALLCOOPERATIONSETTING_ANDMALL_SIGNATURE_KEY = "andmall_signature_key";		// ＆mallのサイト認証キー
		public const string FIELD_MALLCOOPERATIONSETTING_LOHACO_API_PRIVATE_KEY = "lohaco_private_key";				// ロハコ秘密鍵
		public const string FIELD_MALLCOOPERATIONSETTING_NEXT_ENGINE_STOCK_STORE_ACCOUNT = "next_engine_stock_store_account";// ネクストエンジン_在庫連携_ストアアカウント
		public const string FIELD_MALLCOOPERATIONSETTING_NEXT_ENGINE_STOCK_AUTH_KEY = "next_engine_stock_auth_key";// ネクストエンジン_在庫連携_認証キー
		/// <summary>Facebook catalog id</summary>
		public const string FIELD_MALLCOOPERATIONSETTING_FACEBOOK_CATALOG_ID = "facebook_catalog_id";
		/// <summary>Facebook access token</summary>
		public const string FIELD_MALLCOOPERATIONSETTING_FACEBOOK_ACCESS_TOKEN = "facebook_access_token";
		/// <summary>Yahoo API Client ID (アプリケーションID)</summary>
		public const string FIELD_MALLCOOPERATIONSETTING_YAHOO_API_CLIENT_ID = "yahoo_api_client_id";
		/// <summary>Yahoo API Client シークレット</summary>
		public const string FIELD_MALLCOOPERATIONSETTING_YAHOO_API_CLIENT_SECRET = "yahoo_api_client_secret";
		/// <summary>Yahoo API Access Token</summary>
		public const string FIELD_MALLCOOPERATIONSETTING_YAHOO_API_ACCESS_TOKEN = "yahoo_api_access_token";
		/// <summary>Yahoo API Access Token有効期限</summary>
		public const string FIELD_MALLCOOPERATIONSETTING_YAHOO_API_ACCESS_TOKEN_EXPIRATION_DATETIME = "yahoo_api_access_token_expiration_datetime";
		/// <summary>Yahoo API Refresh Token</summary>
		public const string FIELD_MALLCOOPERATIONSETTING_YAHOO_API_REFRESH_TOKEN = "yahoo_api_refresh_token";
		/// <summary>Yahoo API Refresh Token 有効期限</summary>
		public const string FIELD_MALLCOOPERATIONSETTING_YAHOO_API_REFRESH_TOKEN_EXPIRATION_DATETIME = "yahoo_api_refresh_token_expiration_datetime";
		/// <summary>Yahoo API セラーID</summary>
		public const string FIELD_MALLCOOPERATIONSETTING_YAHOO_API_SELLER_ID = "yahoo_api_seller_id";
		/// <summary>Yahoo API 公開鍵</summary>
		public const string FIELD_MALLCOOPERATIONSETTING_YAHOO_API_PUBLIC_KEY = "yahoo_api_public_key";
		/// <summary>公開鍵のバージョン</summary>
		public const string FIELD_MALLCOOPERATIONSETTING_YAHOO_API_PUBLIC_KEY_VERSION = "yahoo_api_public_key_version";
		/// <summary>最終認証日時</summary>
		public const string FIELD_MALLCOOPERATIONSETTING_YAHOO_API_LAST_PUBLIC_KEY_AUTHORIZED_AT = "yahoo_api_last_public_key_authorized_at";
		/// <summary>楽天SKU管理「SKU管理ID」列上書き用フォーマット（バリエーションなし用）</summary>
		public const string FIELD_MALLCOOPERATIONSETTING_RAKUTEN_SKU_MANAGEMENT_ID_OUTPUT_FORMAT_FOR_NORMAL = "rakuten_sku_management_id_output_format_for_normal";
		/// <summary>楽天SKU管理「SKU管理ID」列上書き用フォーマット（バリエーションあり用）</summary>
		public const string FIELD_MALLCOOPERATIONSETTING_RAKUTEN_SKU_MANAGEMENT_ID_OUTPUT_FORMAT_FOR_VARIATION = "rakuten_sku_management_id_output_format_for_variation";


		// モール連携更新ログ
		public const string TABLE_MALLCOOPERATIONUPDATELOG = "w2_MallCooperationUpdateLog";         // モール連携更新ログ
		public const string FIELD_MALLCOOPERATIONUPDATELOG_LOG_NO = "log_no";                       // ログNO
		public const string FIELD_MALLCOOPERATIONUPDATELOG_SHOP_ID = "shop_id";                     // 店舗ID
		public const string FIELD_MALLCOOPERATIONUPDATELOG_MALL_ID = "mall_id";                     // モールID
		public const string FIELD_MALLCOOPERATIONUPDATELOG_PRODUCT_ID = "product_id";               // 商品ID
		public const string FIELD_MALLCOOPERATIONUPDATELOG_VARIATION_ID = "variation_id";           // 商品バリエーションID
		public const string FIELD_MALLCOOPERATIONUPDATELOG_MASTER_KBN = "master_kbn";               // マスタ区分
		public const string FIELD_MALLCOOPERATIONUPDATELOG_ACTION_KBN = "action_kbn";               // 処理区分
		public const string FIELD_MALLCOOPERATIONUPDATELOG_ACTION_STATUS = "action_status";         // 処理ステータス
		public const string FIELD_MALLCOOPERATIONUPDATELOG_DATE_CREATED = "date_created";           // 作成日
		public const string FIELD_MALLCOOPERATIONUPDATELOG_DATE_CHANGED = "date_changed";           // 更新日

		// モール出品設定マスタ
		public const string TABLE_MALLEXHIBITSCONFIG = "w2_MallExhibitsConfig";                     // モール出品設定マスタ
		public const string FIELD_MALLEXHIBITSCONFIG_SHOP_ID = "shop_id";                           // 店舗ID
		public const string FIELD_MALLEXHIBITSCONFIG_PRODUCT_ID = "product_id";                     // 商品ID
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG1 = "exhibits_flg1";               // 出品FLG1
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG2 = "exhibits_flg2";               // 出品FLG2
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG3 = "exhibits_flg3";               // 出品FLG3
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG4 = "exhibits_flg4";               // 出品FLG4
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG5 = "exhibits_flg5";               // 出品FLG5
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG6 = "exhibits_flg6";               // 出品FLG6
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG7 = "exhibits_flg7";               // 出品FLG7
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG8 = "exhibits_flg8";               // 出品FLG8
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG9 = "exhibits_flg9";               // 出品FLG9
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG10 = "exhibits_flg10";             // 出品FLG10
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG11 = "exhibits_flg11";             // 出品FLG11
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG12 = "exhibits_flg12";             // 出品FLG12
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG13 = "exhibits_flg13";             // 出品FLG13
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG14 = "exhibits_flg14";             // 出品FLG14
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG15 = "exhibits_flg15";             // 出品FLG15
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG16 = "exhibits_flg16";             // 出品FLG16
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG17 = "exhibits_flg17";             // 出品FLG17
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG18 = "exhibits_flg18";             // 出品FLG18
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG19 = "exhibits_flg19";             // 出品FLG19
		public const string FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG20 = "exhibits_flg20";             // 出品FLG20
		public const string FIELD_MALLEXHIBITSCONFIG_DATE_CREATED = "date_created";                 // 作成日
		public const string FIELD_MALLEXHIBITSCONFIG_DATE_CHANGED = "date_changed";                 // 更新日
		public const string FIELD_MALLEXHIBITSCONFIG_LAST_CHANGED = "last_changed";                 // 最終更新者

		// モール監視ログマスタ
		public const string TABLE_MALLWATCHINGLOG = "w2_MallWatchingLog";                           // モール監視ログマスタ
		public const string FIELD_MALLWATCHINGLOG_LOG_NO = "log_no";                                // ログno
		public const string FIELD_MALLWATCHINGLOG_WATCHING_DATE = "watching_date";                  // 日付
		public const string FIELD_MALLWATCHINGLOG_WATCHING_TIME = "watching_time";                  // 時間
		public const string FIELD_MALLWATCHINGLOG_BATCH_ID = "batch_id";                            // バッチID
		public const string FIELD_MALLWATCHINGLOG_MALL_ID = "mall_id";                              // モールID
		public const string FIELD_MALLWATCHINGLOG_LOG_KBN = "log_kbn";                              // ログ区分
		public const string FIELD_MALLWATCHINGLOG_LOG_MESSAGE = "log_message";                      // ログメッセージ
		public const string FIELD_MALLWATCHINGLOG_LOG_CONTENT1 = "log_content1";                    // ログ予約1
		public const string FIELD_MALLWATCHINGLOG_LOG_CONTENT2 = "log_content2";                    // ログ予約2
		public const string FIELD_MALLWATCHINGLOG_LOG_CONTENT3 = "log_content3";                    // ログ予約3
		public const string FIELD_MALLWATCHINGLOG_LOG_CONTENT4 = "log_content4";                    // ログ予約4
		public const string FIELD_MALLWATCHINGLOG_LOG_CONTENT5 = "log_content5";                    // ログ予約5

		// モール商品コンバータマスタ
		public const string TABLE_MALLPRDCNV = "w2_MallPrdcnv";                                     // モール商品コンバータマスタ
		public const string FIELD_MALLPRDCNV_ADTO_ID = "adto_id";                                   // 広告先ID
		public const string FIELD_MALLPRDCNV_SHOP_ID = "shop_id";                                   // 店舗ID
		public const string FIELD_MALLPRDCNV_ADTO_NAME = "adto_name";                               // 広告先名
		public const string FIELD_MALLPRDCNV_SEPARATER = "separater";                               // 区切り記号
		public const string FIELD_MALLPRDCNV_CHARACTERCODETYPE = "characterCodeType";               // 文字コード
		public const string FIELD_MALLPRDCNV_NEWLINETYPE = "newLineType";                           // 改行コード
		public const string FIELD_MALLPRDCNV_QUOTE = "quote";                                       // 囲い記号
		public const string FIELD_MALLPRDCNV_ISQUOTE = "isQuote";                                   // 囲い記号フラグ
		public const string FIELD_MALLPRDCNV_ISHEADER = "isHeader";                                 // ヘッダ出力フラグ
		public const string FIELD_MALLPRDCNV_TASKID = "taskId";                                     // タスクID
		public const string FIELD_MALLPRDCNV_LASTCREATED = "lastCreated";                           // 最終作成日
		public const string FIELD_MALLPRDCNV_CREATEDRECORDCOUNT = "createdRecordCount";             // 作成レコード数
		public const string FIELD_MALLPRDCNV_FILENAME = "filename";                                 // ファイル名
		public const string FIELD_MALLPRDCNV_PATH = "path";                                         // パス
		public const string FIELD_MALLPRDCNV_SOURCETABLENAME = "sourceTableName";                   // テーブル名
		public const string FIELD_MALLPRDCNV_EXTRACTIONPATTERN = "extractionPattern";               // 抽出条件

		// モール商品コンバータカラム
		public const string TABLE_MALLPRDCNVCOLUMNS = "w2_MallPrdcnvColumns";                       // モール商品コンバータカラム
		public const string FIELD_MALLPRDCNVCOLUMNS_ADCOLUMN_ID = "adcolumn_id";                    // 広告先カラムID
		public const string FIELD_MALLPRDCNVCOLUMNS_ADTO_ID = "adto_id";                            // 広告先ID
		public const string FIELD_MALLPRDCNVCOLUMNS_COLUMN_NO = "column_no";                        // カラムNo
		public const string FIELD_MALLPRDCNVCOLUMNS_COLUMN_NAME = "column_name";                    // カラム名
		public const string FIELD_MALLPRDCNVCOLUMNS_OUTPUT_FORMAT = "output_format";                // 出力フォーマット

		// モール商品コンバータ文字変換ルール
		public const string TABLE_MALLPRDCNVRULE = "w2_MallPrdcnvRule";                             // モール商品コンバータ文字変換ルール
		public const string FIELD_MALLPRDCNVRULE_ADCONV_ID = "adconv_id";                           // 広告先変換ID
		public const string FIELD_MALLPRDCNVRULE_ADTO_ID = "adto_id";                               // 広告先ID
		public const string FIELD_MALLPRDCNVRULE_CONVERTFROM = "convertFrom";                       // 変換前文字
		public const string FIELD_MALLPRDCNVRULE_CONVERTTO = "convertTo";                           // 変換後文字
		public const string FIELD_MALLPRDCNVRULE_TARGET = "target";                                 // ターゲット

		// モール商品コンバータ作成ファイル
		public const string TABLE_MALLPRDCNVFILES = "w2_MallPrdcnvFiles";                           // モール商品コンバータ作成ファイル
		public const string FIELD_MALLPRDCNVFILES_ADFILES_ID = "adfiles_id";                        // 広告先ファイルID
		public const string FIELD_MALLPRDCNVFILES_ADTO_ID = "adto_id";                              // 広告先ID
		public const string FIELD_MALLPRDCNVFILES_PATH = "path";                                    // パス

		// モール注文メールマスタ
		public const string TABLE_MALLORDERMAIL = "w2_MallOrderMail";                               // モール注文メールマスタ
		public const string FIELD_MALLORDERMAIL_ORDER_MAIL_ID = "order_mail_id";                    // 注文メールID
		public const string FIELD_MALLORDERMAIL_MAIL_TO = "mail_to";                                // メール宛先
		public const string FIELD_MALLORDERMAIL_MAIL_MESSAGE = "mail_message";                      // メール本文
		public const string FIELD_MALLORDERMAIL_RECIVED_DATETIME = "recived_datetime";              // 受信日時
		public const string FIELD_MALLORDERMAIL_ORDER_ENTRY_FLG = "order_entry_flg";                // 本文取得FLG

		// 商品サブ画像設定マスタ
		public const string TABLE_PRODUCTSUBIMAGESETTING = "w2_ProductSubImageSetting";             // 商品サブ画像設定マスタ
		public const string FIELD_PRODUCTSUBIMAGESETTING_SHOP_ID = "shop_id";                       // 店舗ID
		public const string FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_ID = "product_sub_image_id";// 商品サブ画像ID
		public const string FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO = "product_sub_image_no";// 商品サブ画像番号
		public const string FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NAME = "product_sub_image_name";// 商品サブ画像名称
		public const string FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_DISCRIPTION = "product_sub_image_discription";// 商品サブ画像説明
		public const string FIELD_PRODUCTSUBIMAGESETTING_DATE_CREATED = "date_created";             // 作成日
		public const string FIELD_PRODUCTSUBIMAGESETTING_DATE_CHANGED = "date_changed";             // 更新日
		public const string FIELD_PRODUCTSUBIMAGESETTING_LAST_CHANGED = "last_changed";             // 最終更新者

		// 商品初期設定マスタ
		public const string TABLE_PRODUCTDEFAULTSETTING = "w2_ProductDefaultSetting";                // 商品初期設定マスタ
		public const string FIELD_PRODUCTDEFAULTSETTING_SHOP_ID = "shop_id";                         // 店舗ID
		public const string FIELD_PRODUCTDEFAULTSETTING_INIT_DATA = "init_data";                     // 初期設定
		public const string FIELD_PRODUCTDEFAULTSETTING_DATE_CREATED = "date_created";               // 作成日
		public const string FIELD_PRODUCTDEFAULTSETTING_DATE_CHANGED = "date_changed";               // 更新日
		public const string FIELD_PRODUCTDEFAULTSETTING_LAST_CHANGED = "last_changed";               // 最終更新者

		// プレビューマスタ
		public const string TABLE_PREVIEW = "w2_Preview";                                           // プレビューマスタ
		public const string FIELD_PREVIEW_PREVIEW_KBN = "preview_kbn";                              // プレビュー区分
		public const string FIELD_PREVIEW_PREVIEW_ID1 = "preview_id1";                              // プレビューID1
		public const string FIELD_PREVIEW_PREVIEW_ID2 = "preview_id2";                              // プレビューID2
		public const string FIELD_PREVIEW_PREVIEW_ID3 = "preview_id3";                              // プレビューID3
		public const string FIELD_PREVIEW_PREVIEW_ID4 = "preview_id4";                              // プレビューID4
		public const string FIELD_PREVIEW_PREVIEW_ID5 = "preview_id5";                              // プレビューID5
		public const string FIELD_PREVIEW_PREVIEW_DATA = "preview_data";                            // プレビューデータ
		public const string FIELD_PREVIEW_DATE_CREATED = "date_created";                            // 作成日

		// 新着情報マスタ
		public const string TABLE_NEWS = "w2_News";                                                 // 新着情報マスタ
		public const string FIELD_NEWS_SHOP_ID = "shop_id";                                         // 店舗ID
		public const string FIELD_NEWS_NEWS_ID = "news_id";                                         // 新着ID
		public const string FIELD_NEWS_DISPLAY_DATE = "display_date";                               // 表示日付 ※旧カラム
		public const string FIELD_NEWS_DISPLAY_DATE_FROM = "display_date_from";                     // 表示日付（From）
		public const string FIELD_NEWS_DISPLAY_DATE_TO = "display_date_to";                         // 表示日付（To）
		public const string FIELD_NEWS_NEWS_TEXT_KBN = "news_text_kbn";                             // 本文区分
		public const string FIELD_NEWS_NEWS_TEXT = "news_text";                                     // 本文
		public const string FIELD_NEWS_NEWS_TEXT_KBN_MOBILE = "news_text_kbn_mobile";               // モバイル本文区分
		public const string FIELD_NEWS_NEWS_TEXT_MOBILE = "news_text_mobile";                       // モバイル本文
		public const string FIELD_NEWS_DISP_FLG = "disp_flg";                                       // 表示フラグ
		public const string FIELD_NEWS_MOBILE_DISP_FLG = "mobile_disp_flg";                         // モバイル表示フラグ
		public const string FIELD_NEWS_DISPLAY_ORDER = "display_order";                             // 表示順
		public const string FIELD_NEWS_VALID_FLG = "valid_flg";                                     // 有効フラグ
		public const string FIELD_NEWS_DEL_FLG = "del_flg";                                         // 削除フラグ
		public const string FIELD_NEWS_DATE_CREATED = "date_created";                               // 作成日
		public const string FIELD_NEWS_DATE_CHANGED = "date_changed";                               // 更新日
		public const string FIELD_NEWS_LAST_CHANGED = "last_changed";                               // 最終更新者
		public const string FIELD_NEWS_BRAND_ID = "brand_id";                                       // ブランドID

		// パスワードリマインダー
		public const string TABLE_PASSWORDREMINDER = "w2_PasswordReminder";                         // パスワードリマインダー
		public const string FIELD_PASSWORDREMINDER_USER_ID = "user_id";                             // ユーザID
		public const string FIELD_PASSWORDREMINDER_AUTHENTICATION_KEY = "authentication_key";       // 認証キー
		public const string FIELD_PASSWORDREMINDER_CHANGE_TRIAL_LIMIT_COUNT = "change_trial_limit_count";// 変更試行回数制限
		public const string FIELD_PASSWORDREMINDER_DATE_CREATED = "date_created";                   // 作成日

		// 商品一覧表示設定
		/// <summary>商品一覧表示設定</summary>
		public const string TABLE_PRODUCTLISTDISPSETTING = "w2_ProductListDispSetting";
		/// <summary>商品一覧表示設定：設定ID</summary>
		public const string FIELD_PRODUCTLISTDISPSETTING_SETTING_ID = "setting_id";
		/// <summary>商品一覧表示設定：表示名</summary>
		public const string FIELD_PRODUCTLISTDISPSETTING_SETTING_NAME = "setting_name";
		/// <summary>商品一覧表示設定：表示／非表示</summary>
		public const string FIELD_PRODUCTLISTDISPSETTING_DISP_ENABLE = "disp_enable";
		/// <summary>商品一覧表示設定：表示順</summary>
		public const string FIELD_PRODUCTLISTDISPSETTING_DISP_NO = "disp_no";
		/// <summary>商品一覧表示設定：デフォルト表示フラグ</summary>
		public const string FIELD_PRODUCTLISTDISPSETTING_DEFAULT_DISP_FLG = "default_disp_flg";
		/// <summary>商品一覧表示設定：説明</summary>
		public const string FIELD_PRODUCTLISTDISPSETTING_DESCRIPTION = "description";
		/// <summary>商品一覧表示設定：更新日</summary>
		public const string FIELD_PRODUCTLISTDISPSETTING_DATE_CHANGED = "date_changed";
		/// <summary>商品一覧表示設定：最終更新者</summary>
		public const string FIELD_PRODUCTLISTDISPSETTING_LAST_CHANGED = "last_changed";
		/// <summary>商品一覧表示設定：最終更新者</summary>
		public const string FIELD_PRODUCTLISTDISPSETTING_SETTING_KBN = "setting_kbn";
		/// <summary>商品一覧表示設定：最終更新者</summary>
		public const string FIELD_PRODUCTLISTDISPSETTING_DISP_COUNT = "disp_count";

		// 商品ランキング設定マスタ
		/// <summary>商品ランキング設定マスタ</summary>
		public const string TABLE_PRODUCTRANKING = "w2_ProductRanking";
		/// <summary>店舗ID</summary>
		public const string FIELD_PRODUCTRANKING_SHOP_ID = "shop_id";
		/// <summary>商品ランキングID</summary>
		public const string FIELD_PRODUCTRANKING_RANKING_ID = "ranking_id";
		/// <summary>集計タイプ</summary>
		public const string FIELD_PRODUCTRANKING_TOTAL_TYPE = "total_type";
		/// <summary>集計期間区分</summary>
		public const string FIELD_PRODUCTRANKING_TOTAL_KBN = "total_kbn";
		/// <summary>集計期間(FROM)</summary>
		public const string FIELD_PRODUCTRANKING_TOTAL_FROM = "total_from";
		/// <summary>集計期間(TO)</summary>
		public const string FIELD_PRODUCTRANKING_TOTAL_TO = "total_to";
		/// <summary>集計日数</summary>
		public const string FIELD_PRODUCTRANKING_TOTAL_DAYS = "total_days";
		/// <summary>カテゴリ指定</summary>
		public const string FIELD_PRODUCTRANKING_CATEGORY_KBN = "category_kbn";
		/// <summary>カテゴリID</summary>
		public const string FIELD_PRODUCTRANKING_CATEGORY_ID = "category_id";
		/// <summary>説明</summary>
		public const string FIELD_PRODUCTRANKING_DESC1 = "desc1";
		/// <summary>在庫切れ商品</summary>
		public const string FIELD_PRODUCTRANKING_STOCK_KBN = "stock_kbn";
		/// <summary>有効フラグ</summary>
		public const string FIELD_PRODUCTRANKING_VALID_FLG = "valid_flg";
		/// <summary>作成日</summary>
		public const string FIELD_PRODUCTRANKING_DATE_CREATED = "date_created";
		/// <summary>更新日</summary>
		public const string FIELD_PRODUCTRANKING_DATE_CHANGED = "date_changed";
		/// <summary>最終更新者</summary>
		public const string FIELD_PRODUCTRANKING_LAST_CHANGED = "last_changed";
		/// <summary>ブランド指定</summary>
		public const string FIELD_PRODUCTRANKING_BRAND_KBN = "brand_kbn";
		/// <summary>ブランドID</summary>
		public const string FIELD_PRODUCTRANKING_BRAND_ID = "brand_id";
		/// <summary>カテゴリ除外ID</summary>
		public const string FIELD_PRODUCTRANKING_EXCLUDE_CATEGORY_IDS = "exclude_category_ids";

		// 商品ランキングアイテム設定マスタ
		public const string TABLE_PRODUCTRANKINGITEM = "w2_ProductRankingItem";                     // 商品ランキングアイテム設定マスタ
		public const string FIELD_PRODUCTRANKINGITEM_SHOP_ID = "shop_id";                           // 店舗ID
		public const string FIELD_PRODUCTRANKINGITEM_RANKING_ID = "ranking_id";                     // 商品ランキングID
		public const string FIELD_PRODUCTRANKINGITEM_RANK = "rank";                                 // ランキング
		public const string FIELD_PRODUCTRANKINGITEM_PRODUCT_ID = "product_id";                     // 商品ID
		public const string FIELD_PRODUCTRANKINGITEM_FIXATION_FLG = "fixation_flg";                 // 固定フラグ
		public const string FIELD_PRODUCTRANKINGITEM_DATE_CREATED = "date_created";                 // 作成日
		public const string FIELD_PRODUCTRANKINGITEM_DATE_CHANGED = "date_changed";                 // 更新日
		public const string FIELD_PRODUCTRANKINGITEM_LAST_CHANGED = "last_changed";                 // 最終更新者

		// 商品ブランドマスタ
		public const string TABLE_PRODUCTBRAND = "w2_ProductBrand";                                 // 商品ブランドマスタ
		public const string FIELD_PRODUCTBRAND_BRAND_ID = "brand_id";                               // ブランドID
		public const string FIELD_PRODUCTBRAND_BRAND_NAME = "brand_name";                           // ブランド名
		public const string FIELD_PRODUCTBRAND_BRAND_TITLE = "brand_title";                         // ブランドタイトル
		public const string FIELD_PRODUCTBRAND_ADDITIONAL_DESIGN_TAG = "additional_design_tag";     // 追加デザインタグ
		public const string FIELD_PRODUCTBRAND_SEO_KEYWORD = "seo_keyword";                         // SEOキーワード
		public const string FIELD_PRODUCTBRAND_DEFAULT_FLG = "default_flg";							// デフォルト設定フラグ
		public const string FIELD_PRODUCTBRAND_VALID_FLG = "valid_flg";                             // 有効フラグ
		public const string FIELD_PRODUCTBRAND_DATE_CREATED = "date_created";                       // 作成日
		public const string FIELD_PRODUCTBRAND_DATE_CHANGED = "date_changed";                       // 更新日
		public const string FIELD_PRODUCTBRAND_LAST_CHANGED = "last_changed";                       // 最終更新者

		// 商品タグ設定マスタ
		public const string TABLE_PRODUCTTAGSETTING = "w2_ProductTagSetting";                       // 商品タグ設定マスタ
		public const string FIELD_PRODUCTTAGSETTING_TAG_NO = "tag_no";                              // タグNo
		public const string FIELD_PRODUCTTAGSETTING_TAG_ID = "tag_id";                              // タグID
		public const string FIELD_PRODUCTTAGSETTING_TAG_NAME = "tag_name";                          // タイトル
		public const string FIELD_PRODUCTTAGSETTING_TAG_DISCRIPTION = "tag_discription";            // 説明
		public const string FIELD_PRODUCTTAGSETTING_TAG_VALID_FLG = "tag_valid_flg";                // 有効フラグ
		public const string FIELD_PRODUCTTAGSETTING_DATE_CREATED = "date_created";                  // 作成日
		public const string FIELD_PRODUCTTAGSETTING_DATE_CHANGED = "date_changed";                  // 更新日
		public const string FIELD_PRODUCTTAGSETTING_LAST_CHANGED = "last_changed";                  // 最終更新者

		// 商品タグマスタ
		public const string TABLE_PRODUCTTAG = "w2_ProductTag";                                     // 商品タグマスタ
		public const string FIELD_PRODUCTTAG_PRODUCT_ID = "product_id";                             // 商品ID
		public const string FIELD_PRODUCTTAG_DATE_CREATED = "date_created";                         // 作成日
		public const string FIELD_PRODUCTTAG_DATE_CHANGED = "date_changed";                         // 更新日
		public const string FIELD_PRODUCTTAG_LAST_CHANGED = "last_changed";                         // 最終更新者

		// 外部レコメンド連携更新ログ
		public const string TABLE_RECOMMENDCOOPUPDATELOG = "w2_RecommendCoopUpdateLog";             // 外部レコメンド連携更新ログ
		public const string FIELD_RECOMMENDCOOPUPDATELOG_LOG_NO = "log_no";                         // ログNO
		public const string FIELD_RECOMMENDCOOPUPDATELOG_SHOP_ID = "shop_id";                       // 店舗ID
		public const string FIELD_RECOMMENDCOOPUPDATELOG_MASTER_KBN = "master_kbn";                 // マスタ区分
		public const string FIELD_RECOMMENDCOOPUPDATELOG_MASTER_ID = "master_id";                   // マスタID
		public const string FIELD_RECOMMENDCOOPUPDATELOG_ACTION_STATUS = "action_status";           // 処理ステータス
		public const string FIELD_RECOMMENDCOOPUPDATELOG_DATE_CREATED = "date_created";             // 作成日
		public const string FIELD_RECOMMENDCOOPUPDATELOG_DATE_CHANGED = "date_changed";             // 更新日
		public const string FIELD_RECOMMENDCOOPUPDATELOG_LAST_CHANGED = "last_changed";             // 最終更新者

		// シリアルキー情報
		public const string TABLE_SERIALKEY = "w2_SerialKey";                                       // シリアルキー情報
		public const string FIELD_SERIALKEY_SERIAL_KEY = "serial_key";                              // シリアルキー 
		public const string FIELD_SERIALKEY_PRODUCT_ID = "product_id";                              // 商品ID
		public const string FIELD_SERIALKEY_VARIATION_ID = "variation_id";                          // 商品バリエーションID
		public const string FIELD_SERIALKEY_ORDER_ID = "order_id";                                  // 注文ID
		public const string FIELD_SERIALKEY_ORDER_ITEM_NO = "order_item_no";                        // 注文商品枝番
		public const string FIELD_SERIALKEY_USER_ID = "user_id";                                    // ユーザID
		public const string FIELD_SERIALKEY_STATUS = "status";                                      // 状態
		public const string FIELD_SERIALKEY_VALID_FLG = "valid_flg";                                // 有効フラグ
		public const string FIELD_SERIALKEY_DATE_DELIVERED = "date_delivered";                      // 発行日
		public const string FIELD_SERIALKEY_DOWNLOAD_COUNT = "download_count";                      // 回数
		public const string FIELD_SERIALKEY_DATE_CREATED = "date_created";                          // 作成日
		public const string FIELD_SERIALKEY_DATE_CHANGED = "date_changed";                          // 更新日
		public const string FIELD_SERIALKEY_LAST_CHANGED = "last_changed";                          // 最終更新者

		// ノベルティ設定
		public const string TABLE_NOVELTY = "w2_Novelty";                                           // ノベルティ設定
		public const string FIELD_NOVELTY_SHOP_ID = "shop_id";                                      // 店舗ID
		public const string FIELD_NOVELTY_NOVELTY_ID = "novelty_id";                                // ノベルティID
		public const string FIELD_NOVELTY_NOVELTY_DISP_NAME = "novelty_disp_name";                  // ノベルティ名（表示用）
		public const string FIELD_NOVELTY_NOVELTY_NAME = "novelty_name";                            // ノベルティ名（管理用）
		public const string FIELD_NOVELTY_DISCRIPTION = "discription";                              // 説明（管理用）
		public const string FIELD_NOVELTY_DATE_BEGIN = "date_begin";                                // 開始日時
		public const string FIELD_NOVELTY_DATE_END = "date_end";                                    // 終了日時
		public const string FIELD_NOVELTY_VALID_FLG = "valid_flg";                                  // 有効フラグ
		public const string FIELD_NOVELTY_DATE_CREATED = "date_created";                            // 作成日
		public const string FIELD_NOVELTY_DATE_CHANGED = "date_changed";                            // 更新日
		public const string FIELD_NOVELTY_LAST_CHANGED = "last_changed";                            // 最終更新者
		public const string FIELD_NOVELTY_AUTO_ADDITIONAL_FLG = "auto_additional_flg";				// 自動付与フラグ

		// ノベルティ対象アイテム
		public const string TABLE_NOVELTYTARGETITEM = "w2_NoveltyTargetItem";                       // ノベルティ対象アイテム
		public const string FIELD_NOVELTYTARGETITEM_SHOP_ID = "shop_id";                            // 店舗ID
		public const string FIELD_NOVELTYTARGETITEM_NOVELTY_ID = "novelty_id";                      // ノベルティID
		public const string FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_NO = "novelty_target_item_no";// ノベルティ対象アイテム枝番
		public const string FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_TYPE = "novelty_target_item_type";// ノベルティ対象アイテム種別
		public const string FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_VALUE = "novelty_target_item_value";// ノベルティ対象アイテム値
		public const string FIELD_NOVELTYTARGETITEM_NOVELTY_TARGET_ITEM_TYPE_SORT_NO = "novelty_target_item_type_sort_no";// ノベルティ対象アイテム並び順
		public const string FIELD_NOVELTYTARGETITEM_DATE_CREATED = "date_created";                  // 作成日
		public const string FIELD_NOVELTYTARGETITEM_DATE_CHANGED = "date_changed";                  // 更新日
		public const string FIELD_NOVELTYTARGETITEM_LAST_CHANGED = "last_changed";                  // 最終更新者

		// ノベルティ付与条件
		public const string TABLE_NOVELTYGRANTCONDITIONS = "w2_NoveltyGrantConditions";             // ノベルティ付与条件
		public const string FIELD_NOVELTYGRANTCONDITIONS_SHOP_ID = "shop_id";                       // 店舗ID
		public const string FIELD_NOVELTYGRANTCONDITIONS_NOVELTY_ID = "novelty_id";                 // ノベルティID
		public const string FIELD_NOVELTYGRANTCONDITIONS_CONDITION_NO = "condition_no";             // ノベルティ付与条件枝番
		public const string FIELD_NOVELTYGRANTCONDITIONS_USER_RANK_ID = "user_rank_id";             // 対象会員
		public const string FIELD_NOVELTYGRANTCONDITIONS_AMOUNT_BEGIN = "amount_begin";             // 対象金額（以上）
		public const string FIELD_NOVELTYGRANTCONDITIONS_AMOUNT_END = "amount_end";                 // 対象金額（以下）
		public const string FIELD_NOVELTYGRANTCONDITIONS_DATE_CREATED = "date_created";             // 作成日
		public const string FIELD_NOVELTYGRANTCONDITIONS_DATE_CHANGED = "date_changed";             // 更新日
		public const string FIELD_NOVELTYGRANTCONDITIONS_LAST_CHANGED = "last_changed";             // 最終更新者

		// ノベルティ付与アイテム
		public const string TABLE_NOVELTYGRANTITEM = "w2_NoveltyGrantItem";                         // ノベルティ付与アイテム
		public const string FIELD_NOVELTYGRANTITEM_SHOP_ID = "shop_id";                             // 店舗ID
		public const string FIELD_NOVELTYGRANTITEM_NOVELTY_ID = "novelty_id";                       // ノベルティID
		public const string FIELD_NOVELTYGRANTITEM_CONDITION_NO = "condition_no";                   // ノベルティ付与条件枝番
		public const string FIELD_NOVELTYGRANTITEM_PRODUCT_ID = "product_id";                       // 商品ID
		public const string FIELD_NOVELTYGRANTITEM_SORT_NO = "sort_no";                             // 並び順
		public const string FIELD_NOVELTYGRANTITEM_DATE_CREATED = "date_created";                   // 作成日
		public const string FIELD_NOVELTYGRANTITEM_DATE_CHANGED = "date_changed";                   // 更新日
		public const string FIELD_NOVELTYGRANTITEM_LAST_CHANGED = "last_changed";                   // 最終更新者

		// レコメンド設定
		public const string TABLE_RECOMMEND = "w2_Recommend";                                       // レコメンド設定
		public const string FIELD_RECOMMEND_SHOP_ID = "shop_id";                                    // 店舗ID
		public const string FIELD_RECOMMEND_RECOMMEND_ID = "recommend_id";                          // レコメンドID
		public const string FIELD_RECOMMEND_RECOMMEND_NAME = "recommend_name";                      // レコメンド名（管理用）
		public const string FIELD_RECOMMEND_DISCRIPTION = "discription";                            // 説明（管理用）
		public const string FIELD_RECOMMEND_RECOMMEND_DISPLAY_PAGE = "recommend_display_page";      // レコメンド表示ページ
		public const string FIELD_RECOMMEND_RECOMMEND_KBN = "recommend_kbn";                        // レコメンド区分
		public const string FIELD_RECOMMEND_DATE_BEGIN = "date_begin";                              // 開始日時
		public const string FIELD_RECOMMEND_DATE_END = "date_end";                                  // 終了日時
		public const string FIELD_RECOMMEND_PRIORITY = "priority";                                  // 適用優先順
		public const string FIELD_RECOMMEND_VALID_FLG = "valid_flg";                                // 有効フラグ
		public const string FIELD_RECOMMEND_RECOMMEND_DISPLAY_KBN_PC = "recommend_display_kbn_pc";  // レコメンド表示区分PC
		public const string FIELD_RECOMMEND_RECOMMEND_DISPLAY_PC = "recommend_display_pc";          // レコメンド表示PC
		public const string FIELD_RECOMMEND_RECOMMEND_DISPLAY_KBN_SP = "recommend_display_kbn_sp";  // レコメンド表示区分SP
		public const string FIELD_RECOMMEND_RECOMMEND_DISPLAY_SP = "recommend_display_sp";          // レコメンド表示SP
		public const string FIELD_RECOMMEND_DATE_CREATED = "date_created";                          // 作成日
		public const string FIELD_RECOMMEND_DATE_CHANGED = "date_changed";                          // 更新日
		public const string FIELD_RECOMMEND_LAST_CHANGED = "last_changed";                          // 最終更新者
		public const string FIELD_RECOMMEND_ONETIME_FLG = "onetime_flg";                            // ワンタイム表示フラグ

		// レコメンドレポート
		/// <summary>PV数</summary>
		public const string FIELD_RECOMMEND_PV_NUMBER = "pv_number";
		/// <summary>CV数</summary>
		public const string FIELD_RECOMMEND_CV_NUMBER = "cv_number";
		/// <summary>合計PV数</summary>
		public const string FIELD_RECOMMEND_PV_NUMBER_TOTAL = "pv_number_total";
		/// <summary>合計CV数</summary>
		public const string FIELD_RECOMMEND_CV_NUMBER_TOTAL = "cv_number_total";
		/// <summary>開始時間</summary>
		public const string FIELD_RECOMMEND_REPORT_GRAPH_DATE_FROM = "date_bgn";
		/// <summary>終了時間</summary>
		public const string FIELD_RECOMMEND_REPORT_GRAPH_DATE_TO = "date_end";
		/// <summary>年</summary>
		public const string FIELD_RECOMMEND_REPORT_GRAPH_DATE_YEAR = "tgt_year";
		/// <summary>月</summary>
		public const string FIELD_RECOMMEND_REPORT_GRAPH_DATE_MONTH = "tgt_month";
		/// <summary>日</summary>
		public const string FIELD_RECOMMEND_REPORT_GRAPH_DATE_DAY = "tgt_day";
		/// <summary>値</summary>
		public const string FIELD_RECOMMEND_REPORT_GRAPH_DATE_VALUE = "counts";

		/// <summary>Chatbot use flg</summary>
		public const string FIELD_RECOMMEND_CHATBOT_USE_FLG = "chatbot_use_flg";
		/// <summary>商品タイプ</summary>
		public const string FIELD_RECOMMEND_RECOMMEND_PRODUCT_TYPE = "recommend_product_type";
		/// <summary>所品タイプ:頒布会オプションON</summary>
		public const string FIELD_RECOMMEND_RECOMMEND_PRODUCT_TYPE_VALID_SUBSCRIPTION_BOX = "recommend_product_type_valid_subscription_box";

		// レコメンド適用条件アイテム
		public const string TABLE_RECOMMENDAPPLYCONDITIONITEM = "w2_RecommendApplyConditionItem";   // レコメンド適用条件アイテム
		public const string FIELD_RECOMMENDAPPLYCONDITIONITEM_SHOP_ID = "shop_id";                  // 店舗ID
		public const string FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_ID = "recommend_id";        // レコメンドID
		public const string FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE = "recommend_apply_condition_type";// レコメンド適用条件種別
		public const string FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE = "recommend_apply_condition_item_type";// レコメンド適用条件アイテム種別
		public const string FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_UNIT_TYPE = "recommend_apply_condition_item_unit_type";// レコメンド適用条件アイテム単位種別
		public const string FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_PRODUCT_ID = "recommend_apply_condition_item_product_id";// レコメンド適用条件アイテム商品ID
		public const string FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_VARIATION_ID = "recommend_apply_condition_item_variation_id";// レコメンドアイテム商品バリエーションID
		public const string FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_SORT_NO = "recommend_apply_condition_item_sort_no";// レコメンド適用条件アイテム並び順
		public const string FIELD_RECOMMENDAPPLYCONDITIONITEM_DATE_CREATED = "date_created";        // 作成日
		public const string FIELD_RECOMMENDAPPLYCONDITIONITEM_DATE_CHANGED = "date_changed";        // 更新日
		public const string FIELD_RECOMMENDAPPLYCONDITIONITEM_LAST_CHANGED = "last_changed";        // 最終更新者

		// レコメンドアップセル対象アイテム
		public const string TABLE_RECOMMENDUPSELLTARGETITEM = "w2_RecommendUpsellTargetItem";       // レコメンドアップセル対象アイテム
		public const string FIELD_RECOMMENDUPSELLTARGETITEM_SHOP_ID = "shop_id";                    // 店舗ID
		public const string FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_ID = "recommend_id";          // レコメンドID
		public const string FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE = "recommend_upsell_target_item_type";// レコメンドアップセル対象アイテム種別
		public const string FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_PRODUCT_ID = "recommend_upsell_target_item_product_id";// レコメンドアップセル対象アイテム商品ID
		public const string FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_VARIATION_ID = "recommend_upsell_target_item_variation_id";// レコメンドアップセル対象アイテム商品バリエーションID
		public const string FIELD_RECOMMENDUPSELLTARGETITEM_DATE_CREATED = "date_created";          // 作成日
		public const string FIELD_RECOMMENDUPSELLTARGETITEM_DATE_CHANGED = "date_changed";          // 更新日
		public const string FIELD_RECOMMENDUPSELLTARGETITEM_LAST_CHANGED = "last_changed";          // 最終更新者

		// レコメンドアイテム
		public const string TABLE_RECOMMENDITEM = "w2_RecommendItem";                               // レコメンドアイテム
		public const string FIELD_RECOMMENDITEM_SHOP_ID = "shop_id";                                // 店舗ID
		public const string FIELD_RECOMMENDITEM_RECOMMEND_ID = "recommend_id";                      // レコメンドID
		public const string FIELD_RECOMMENDITEM_RECOMMEND_ITEM_TYPE = "recommend_item_type";        // レコメンドアイテム種別
		public const string FIELD_RECOMMENDITEM_RECOMMEND_ITEM_PRODUCT_ID = "recommend_item_product_id";// レコメンドアイテム商品ID
		public const string FIELD_RECOMMENDITEM_RECOMMEND_ITEM_VARIATION_ID = "recommend_item_variation_id";// レコメンドアイテム商品バリエーションID
		public const string FIELD_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY_TYPE = "recommend_item_add_quantity_type";// レコメンドアイテム投入数種別
		public const string FIELD_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY = "recommend_item_add_quantity";// レコメンドアイテム投入数
		public const string FIELD_RECOMMENDITEM_RECOMMEND_ITEM_SORT_NO = "recommend_item_sort_no";  // レコメンドアイテム並び順
		public const string FIELD_RECOMMENDITEM_DATE_CREATED = "date_created";                      // 作成日
		public const string FIELD_RECOMMENDITEM_DATE_CHANGED = "date_changed";                      // 更新日
		public const string FIELD_RECOMMENDITEM_LAST_CHANGED = "last_changed";                      // 最終更新者
		public const string FIELD_RECOMMENDITEM_FIXED_PURCHASE_KBN = "fixed_purchase_kbn";              // 定期購入区分
		public const string FIELD_RECOMMENDITEM_FIXED_PURCHASE_SETTING1 = "fixed_purchase_setting1";    // 定期購入設定1

		// レコメンド表示履歴
		public const string TABLE_RECOMMENDHISTORY = "w2_RecommendHistroy";                        // レコメンド表示履歴
		public const string FIELD_RECOMMENDHISTORY_SHOP_ID = "shop_id";                            // 店舗ID
		public const string FIELD_RECOMMENDHISTORY_RECOMMEND_ID = "recommend_id";                  // レコメンドID
		public const string FIELD_RECOMMENDHISTORY_USER_ID = "user_id";                            // ユーザーID
		public const string FIELD_RECOMMENDHISTORY_RECOMMEND_HISTORY_NO = "recommend_history_no";  // 履歴枝番
		public const string FIELD_RECOMMENDHISTROY_TARGET_ORDER_ID = "target_order_id";            // レコメンド表示対象注文ID
		public const string FIELD_RECOMMENDHISTORY_DISPLAY_KBN = "display_kbn";                    // 表示区分
		public const string FIELD_RECOMMENDHISTORY_ORDERED_FLG = "ordered_flg";                    // 購入フラグ
		public const string FIELD_RECOMMENDHISTROY_DATE_CREATED = "date_created";                  // 作成日
		public const string FIELD_RECOMMENDHISTORY_DATE_CHANGED = "date_changed";                  // 更新日
		public const string FIELD_RECOMMENDHISTORY_LAST_CHANGED = "last_changed";                  // 最終更新者

		// リアル店舗情報
		public const string TABLE_REALSHOP = "w2_RealShop";                                         // リアル店舗情報
		public const string FIELD_REALSHOP_REAL_SHOP_ID = "real_shop_id";                           // リアル店舗ID
		public const string FIELD_REALSHOP_NAME = "name";                                           // 店舗名
		public const string FIELD_REALSHOP_NAME_KANA = "name_kana";                                 // 店舗名かな
		public const string FIELD_REALSHOP_DESC1_KBN_PC = "desc1_kbn_pc";                           // PC説明1HTML区分
		public const string FIELD_REALSHOP_DESC1_PC = "desc1_pc";                                   // PC説明1
		public const string FIELD_REALSHOP_DESC2_KBN_PC = "desc2_kbn_pc";                           // PC説明2HTML区分
		public const string FIELD_REALSHOP_DESC2_PC = "desc2_pc";                                   // PC説明2
		public const string FIELD_REALSHOP_DESC1_KBN_SP = "desc1_kbn_sp";                           // SP説明1HTML区分
		public const string FIELD_REALSHOP_DESC1_SP = "desc1_sp";                                   // SP説明1
		public const string FIELD_REALSHOP_DESC2_KBN_SP = "desc2_kbn_sp";                           // SP説明2HTML区分
		public const string FIELD_REALSHOP_DESC2_SP = "desc2_sp";                                   // SP説明2
		public const string FIELD_REALSHOP_DESC1_KBN_MB = "desc1_kbn_mb";                           // MB説明1HTML区分
		public const string FIELD_REALSHOP_DESC1_MB = "desc1_mb";                                   // MB説明1
		public const string FIELD_REALSHOP_DESC2_KBN_MB = "desc2_kbn_mb";                           // MB説明2HTML区分
		public const string FIELD_REALSHOP_DESC2_MB = "desc2_mb";                                   // MB説明2
		public const string FIELD_REALSHOP_ZIP = "zip";                                             // 郵便番号
		public const string FIELD_REALSHOP_ZIP1 = "zip1";                                           // 郵便番号1
		public const string FIELD_REALSHOP_ZIP2 = "zip2";                                           // 郵便番号2
		public const string FIELD_REALSHOP_ADDR = "addr";                                           // 住所
		public const string FIELD_REALSHOP_ADDR1 = "addr1";                                         // 住所1
		public const string FIELD_REALSHOP_ADDR2 = "addr2";                                         // 住所2
		public const string FIELD_REALSHOP_ADDR3 = "addr3";                                         // 住所3
		public const string FIELD_REALSHOP_ADDR4 = "addr4";                                         // 住所4
		public const string FIELD_REALSHOP_TEL = "tel";                                             // 電話番号
		public const string FIELD_REALSHOP_TEL_1 = "tel_1";                                         // 電話番号1
		public const string FIELD_REALSHOP_TEL_2 = "tel_2";                                         // 電話番号2
		public const string FIELD_REALSHOP_TEL_3 = "tel_3";                                         // 電話番号3
		public const string FIELD_REALSHOP_FAX = "fax";                                             // FAX
		public const string FIELD_REALSHOP_FAX_1 = "fax_1";                                         // FAX1
		public const string FIELD_REALSHOP_FAX_2 = "fax_2";                                         // FAX2
		public const string FIELD_REALSHOP_FAX_3 = "fax_3";                                         // FAX3
		public const string FIELD_REALSHOP_URL = "url";                                             // URL
		public const string FIELD_REALSHOP_MAIL_ADDR = "mail_addr";                                 // メールアドレス
		public const string FIELD_REALSHOP_OPENING_HOURS = "opening_hours";                         // 営業時間
		public const string FIELD_REALSHOP_DISPLAY_ORDER = "display_order";                         // 表示順
		public const string FIELD_REALSHOP_VALID_FLG = "valid_flg";                                 // 有効フラグ
		public const string FIELD_REALSHOP_DEL_FLG = "del_flg";                                     // 削除フラグ
		public const string FIELD_REALSHOP_DATE_CREATED = "date_created";                           // 作成日
		public const string FIELD_REALSHOP_DATE_CHANGED = "date_changed";                           // 更新日
		public const string FIELD_REALSHOP_LAST_CHANGED = "last_changed";                           // 最終更新者
		public const string FIELD_REALSHOP_COUNTRY_ISO_CODE = "country_iso_code";                   // 国ISOコード
		public const string FIELD_REALSHOP_COUNTRY_NAME = "country_name";                           // 国名
		public const string FIELD_REALSHOP_ADDR5 = "addr5";                                         // 住所5
		/// <summary>Area id</summary>
		public const string FIELD_REALSHOP_AREA_ID = "area_id";
		/// <summary>Brand id</summary>
		public const string FIELD_REALSHOP_BRAND_ID = "brand_id";
		/// <summary>Longitude</summary>
		public const string FIELD_REALSHOP_LONGITUDE = "longitude";
		/// <summary>Latitude</summary>
		public const string FIELD_REALSHOP_LATITUDE = "latitude";

		// リアル店舗商品在庫情報
		public const string TABLE_REALSHOPPRODUCTSTOCK = "w2_RealShopProductStock";                 // リアル店舗商品在庫情報
		public const string FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID = "real_shop_id";               // リアル店舗ID
		public const string FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID = "product_id";                   // 商品ID
		public const string FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID = "variation_id";               // 商品バリエーションID
		public const string FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK = "real_shop_stock";         // リアル店舗商品在庫数
		public const string FIELD_REALSHOPPRODUCTSTOCK_DATE_CREATED = "date_created";               // 作成日
		public const string FIELD_REALSHOPPRODUCTSTOCK_DATE_CHANGED = "date_changed";               // 更新日
		public const string FIELD_REALSHOPPRODUCTSTOCK_LAST_CHANGED = "last_changed";               // 最終更新者

		// 各ワークテーブル（※テーブル名のみ）
		public const string TABLE_WORKPRODUCT = "w2_WorkProduct";									// 商品マスタ用ワークテーブル
		public const string TABLE_WORKPRODUCTVARIATION = "w2_WorkProductVariation";					// 商品バリエーションマスタ用ワークテーブル
		public const string TABLE_WORKPRODUCTSTOCK = "w2_WorkProductStock";							// 商品在庫マスタ用ワークテーブル
		public const string TABLE_WORKPRODUCTEXTEND = "w2_WorkProductExtend";						// 商品拡張拡張項目マスタ用ワークテーブル
		public const string TABLE_WORKPRODUCTCATEGORY = "w2_WorkProductCategory";					// 商品カテゴリマスタ用ワークテーブル
		public const string TABLE_WORKUSER = "w2_WorkUser";											// ユーザマスタ用ワークテーブル
		public const string TABLE_WORKUSERPOINT = "w2_WorkUserPoint";								// ユーザポイントマスタ用ワークテーブル
		public const string TABLE_WORKUSERSHIPPING = "w2_WorkUserShipping";							// ユーザ配送先情報用ワークテーブル
		public const string TABLE_WORKSERIALKEY = "w2_WorkSerialKey";								// シリアルキーマスタ用ワークテーブル
		public const string TABLE_WORKPRODUCTTAG = "w2_WorkProductTag";                             // 商品タグマスタ用ワーク
		public const string TABLE_WORKTARGETLISTDATA = "w2_WorkTargetListData";                     // ターゲットリストデータ用ワークテーブル
		public const string TABLE_WORKREALSHOP = "w2_WorkRealShop";                                 // リアル店舗情報用ワークテーブル
		public const string TABLE_WORKREALSHOPPRODUCTSTOCK = "w2_WorkRealShopProductStock";         // リアル店舗商品在庫情報用ワークテーブル
		/// <summary>店舗管理者マスタワークテーブル</summary>
		public const string TABLE_WORKSHOPOPERATOR = "w2_WorkShopOperator";

		// ターゲットユーザリストの一時保持テーブル
		public const string TABLE_TEMPTARGETUSER = "w2_TempTargetUser";                             // ターゲットユーザリストの一時保持テーブル
		public const string FIELD_TEMPTARGETUSER_USER_ID = "user_id";                               // ユーザID

		// 決済カード連携マスタ
		public const string TABLE_USERCREDITCARD = "w2_UserCreditCard";                             // 決済カード連携マスタ
		public const string FIELD_USERCREDITCARD_USER_ID = "user_id";                               // ユーザID
		public const string FIELD_USERCREDITCARD_BRANCH_NO = "branch_no";                           // カード枝番
		public const string FIELD_USERCREDITCARD_COOPERATION_ID = "cooperation_id";                 // 連携ID
		public const string FIELD_USERCREDITCARD_CARD_DISP_NAME = "card_disp_name";                 // カード表示名
		public const string FIELD_USERCREDITCARD_LAST_FOUR_DIGIT = "last_four_digit";               // カード番号下４桁
		public const string FIELD_USERCREDITCARD_EXPIRATION_MONTH = "expiration_month";             // 有効期限（月）
		public const string FIELD_USERCREDITCARD_EXPIRATION_YEAR = "expiration_year";               // 有効期限（年）
		public const string FIELD_USERCREDITCARD_AUTHOR_NAME = "author_name";                       // カード名義人
		public const string FIELD_USERCREDITCARD_DISP_FLG = "disp_flg";                             // 表示フラグ
		public const string FIELD_USERCREDITCARD_DATE_CREATED = "date_created";                     // 作成日
		public const string FIELD_USERCREDITCARD_DATE_CHANGED = "date_changed";                     // 更新日
		public const string FIELD_USERCREDITCARD_LAST_CHANGED = "last_changed";                     // 最終更新者
		public const string FIELD_USERCREDITCARD_COMPANY_CODE = "company_code";                     // カード会社コード
		public const string FIELD_USERCREDITCARD_COOPERATION_ID2 = "cooperation_id2";               // 連携ID2
		public const string FIELD_USERCREDITCARD_REGISTER_ACTION_KBN = "register_action_kbn";       // 登録アクション区分
		public const string FIELD_USERCREDITCARD_REGISTER_STATUS = "register_status";               // 登録ステータス
		public const string FIELD_USERCREDITCARD_REGISTER_TARGET_ID = "register_target_id";         // 登録対象ID
		public const string FIELD_USERCREDITCARD_BEFORE_ORDER_STATUS = "before_order_status";       // 更新前ステータス
		public const string FIELD_USERCREDITCARD_COOPERATION_TYPE = "cooperation_type";             // 連携種別

		// 注文メモ設定テーブル
		public const string TABLE_ORDERMEMOSETTING = "w2_OrderMemoSetting";                         // 注文メモ設定テーブル
		public const string FIELD_ORDERMEMOSETTING_ORDER_MEMO_ID = "order_memo_id";                 // 注文メモID
		public const string FIELD_ORDERMEMOSETTING_ORDER_MEMO_NAME = "order_memo_name";             // 注文メモ名称
		public const string FIELD_ORDERMEMOSETTING_HEIGHT = "height";                               // 入力欄縦幅
		public const string FIELD_ORDERMEMOSETTING_WIDTH = "width";                                 // 入力欄横幅
		public const string FIELD_ORDERMEMOSETTING_CSS_CLASS = "css_class";                         // CSS Class
		public const string FIELD_ORDERMEMOSETTING_MAX_LENGTH = "max_length";                       // 入力可能最大文字数
		public const string FIELD_ORDERMEMOSETTING_DEFAULT_TEXT = "default_text";                   // デフォルト文字列
		public const string FIELD_ORDERMEMOSETTING_DISPLAY_KBN = "display_kbn";                     // 表示区分
		public const string FIELD_ORDERMEMOSETTING_DISPLAY_ORDER = "display_order";                 // 表示順
		public const string FIELD_ORDERMEMOSETTING_TERM_BGN = "term_bgn";                           // 有効期間（開始）
		public const string FIELD_ORDERMEMOSETTING_TERM_END = "term_end";                           // 有効期間（終了）
		public const string FIELD_ORDERMEMOSETTING_VALID_FLG = "valid_flg";                         // 有効フラグ
		public const string FIELD_ORDERMEMOSETTING_DATE_CREATED = "date_created";                   // 作成日
		public const string FIELD_ORDERMEMOSETTING_DATE_CHANGED = "date_changed";                   // 更新日
		public const string FIELD_ORDERMEMOSETTING_LAST_CHANGED = "last_changed";                   // 最終更新者


		// 商品レビュー情報用ワークテーブル
		public const string TABLE_WORKPRODUCTREVIEW = "w2_WorkProductReview";                       // 商品レビュー情報用ワークテーブル
		public const string FIELD_WORKPRODUCTREVIEW_SHOP_ID = "shop_id";                            // 店舗ID
		public const string FIELD_WORKPRODUCTREVIEW_PRODUCT_ID = "product_id";                      // 商品ID
		public const string FIELD_WORKPRODUCTREVIEW_REVIEW_NO = "review_no";                        // レビュー番号
		public const string FIELD_WORKPRODUCTREVIEW_USER_ID = "user_id";                            // ユーザID
		public const string FIELD_WORKPRODUCTREVIEW_NICK_NAME = "nick_name";                        // ニックネーム
		public const string FIELD_WORKPRODUCTREVIEW_REVIEW_RATING = "review_rating";                // 評価
		public const string FIELD_WORKPRODUCTREVIEW_REVIEW_TITLE = "review_title";                  // タイトル
		public const string FIELD_WORKPRODUCTREVIEW_REVIEW_COMMENT = "review_comment";              // コメント
		public const string FIELD_WORKPRODUCTREVIEW_OPEN_FLG = "open_flg";                          // 公開フラグ
		public const string FIELD_WORKPRODUCTREVIEW_CHECKED_FLG = "checked_flg";                    // 管理者チェックフラグ
		public const string FIELD_WORKPRODUCTREVIEW_DATE_OPENED = "date_opened";                    // 公開日
		public const string FIELD_WORKPRODUCTREVIEW_DATE_CHECKED = "date_checked";                  // 管理者チェック日
		public const string FIELD_WORKPRODUCTREVIEW_DEL_FLG = "del_flg";                            // 削除フラグ
		public const string FIELD_WORKPRODUCTREVIEW_DATE_CREATED = "date_created";                  // 作成日
		public const string FIELD_WORKPRODUCTREVIEW_DATE_CHANGED = "date_changed";                  // 更新日
		public const string FIELD_WORKPRODUCTREVIEW_LAST_CHANGED = "last_changed";                  // 最終更新者

		// 商品セール価格マスタ用ワークテーブル
		public const string TABLE_WORKPRODUCTSALEPRICE = "w2_WorkProductSalePrice";                 // 商品セール価格マスタ用ワークテーブル
		public const string FIELD_WORKPRODUCTSALEPRICE_SHOP_ID = "shop_id";                         // 店舗ID
		public const string FIELD_WORKPRODUCTSALEPRICE_PRODUCTSALE_ID = "productsale_id";           // 商品セールID
		public const string FIELD_WORKPRODUCTSALEPRICE_PRODUCT_ID = "product_id";                   // 商品ID
		public const string FIELD_WORKPRODUCTSALEPRICE_VARIATION_ID = "variation_id";               // 商品バリエーションID
		public const string FIELD_WORKPRODUCTSALEPRICE_SALE_PRICE = "sale_price";                   // 商品セール価格
		public const string FIELD_WORKPRODUCTSALEPRICE_DISPLAY_ORDER = "display_order";             // 表示順
		public const string FIELD_WORKPRODUCTSALEPRICE_DEL_FLG = "del_flg";                         // 削除フラグ
		public const string FIELD_WORKPRODUCTSALEPRICE_DATE_CREATED = "date_created";               // 作成日
		public const string FIELD_WORKPRODUCTSALEPRICE_DATE_CHANGED = "date_changed";               // 更新日
		public const string FIELD_WORKPRODUCTSALEPRICE_LAST_CHANGED = "last_changed";               // 最終更新者

		// ショートURLマスタ用ワークテーブル
		public const string TABLE_WROKSHORTURL = "w2_WorkShortUrl";                                 // ショートURLマスタ用ワークテーブル
		public const string FIELD_WROKSHORTURL_SURL_NO = "surl_no";                                 // ショートURL NO
		public const string FIELD_WROKSHORTURL_SHOP_ID = "shop_id";                                 // 店舗ID
		public const string FIELD_WROKSHORTURL_SHORT_URL = "short_url";                             // ショートURL
		public const string FIELD_WROKSHORTURL_LONG_URL = "long_url";                               // ロングURL
		public const string FIELD_WROKSHORTURL_DATE_CREATED = "date_created";                       // 作成日
		public const string FIELD_WROKSHORTURL_DATE_CHANGED = "date_changed";                       // 更新日
		public const string FIELD_WROKSHORTURL_LAST_CHANGED = "last_changed";                       // 最終更新者

		// 商品価格マスタ
		public const string TABLE_PRODUCTPRICE = "w2_ProductPrice";                                 // 商品価格マスタ
		public const string FIELD_PRODUCTPRICE_SHOP_ID = "shop_id";                                 // 店舗ID
		public const string FIELD_PRODUCTPRICE_PRODUCT_ID = "product_id";                           // 商品ID
		public const string FIELD_PRODUCTPRICE_VARIATION_ID = "variation_id";                       // 商品バリエーションID
		public const string FIELD_PRODUCTPRICE_MEMBER_RANK_ID = "member_rank_id";                   // 会員ランクID
		public const string FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE = "member_rank_price";             // 会員価格
		public const string FIELD_PRODUCTPRICE_DATE_CREATED = "date_created";                       // 作成日
		public const string FIELD_PRODUCTPRICE_DATE_CHANGED = "date_changed";                       // 更新日
		public const string FIELD_PRODUCTPRICE_LAST_CHANGED = "last_changed";                       // 最終更新者

		// 商品税率カテゴリマスタ
		public const string TABLE_PRODUCTTAXCATEGORY = "w2_PRODUCTTAXCATEGORY";                     // 商品税率カテゴリマスタ
		public const string FIELD_PRODUCTTAXCATEGORY_TAX_CATEGORY_ID = "tax_category_id";           // 税率カテゴリID
		public const string FIELD_PRODUCTTAXCATEGORY_TAX_CATEGORY_NAME = "tax_category_name";       // 税率カテゴリ名
		public const string FIELD_PRODUCTTAXCATEGORY_TAX_RATE = "tax_rate";                         // 税率
		public const string FIELD_PRODUCTTAXCATEGORY_DISPLAY_ORDER = "display_order";                         // 表示順
		public const string FIELD_PRODUCTTAXCATEGORY_DATE_CREATED = "date_created";                 // 作成日
		public const string FIELD_PRODUCTTAXCATEGORY_DATE_CHANGED = "date_changed";                 // 更新日
		public const string FIELD_PRODUCTTAXCATEGORY_LAST_CHANGED = "last_changed";                 // 最終更新者

		// 商品価格マスタ用ワークテーブル
		public const string TABLE_WORKPRODUCTPRICE = "w2_WorkProductPrice";                         // 商品価格マスタ
		public const string FIELD_WORKPRODUCTPRICE_SHOP_ID = "shop_id";                             // 店舗ID
		public const string FIELD_WORKPRODUCTPRICE_PRODUCT_ID = "product_id";                       // 商品ID
		public const string FIELD_WORKPRODUCTPRICE_VARIATION_ID = "variation_id";                   // 商品バリエーションID
		public const string FIELD_WORKPRODUCTPRICE_MEMBER_RANK_ID = "member_rank_id";               // 会員ランクID
		public const string FIELD_WORKPRODUCTPRICE_MEMBER_RANK_PRICE = "member_rank_price";         // 会員価格
		public const string FIELD_WORKPRODUCTPRICE_DATE_CREATED = "date_created";                   // 作成日
		public const string FIELD_WORKPRODUCTPRICE_DATE_CHANGED = "date_changed";                   // 更新日
		public const string FIELD_WORKPRODUCTPRICE_LAST_CHANGED = "last_changed";                   // 最終更新者

		// ユーザー管理レベルマスタ
		public const string TABLE_USERMANAGEMENTLEVEL = "w2_UserManagementLevel";                   // ユーザー管理レベルマスタ
		public const string FIELD_USERMANAGEMENTLEVEL_SEQ_NO = "seq_no";                            // シーケンス番号
		public const string FIELD_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_ID = "user_management_level_id";// ユーザー管理レベルID
		public const string FIELD_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_NAME = "user_management_level_name";// ユーザー管理レベル名
		public const string FIELD_USERMANAGEMENTLEVEL_DISPLAY_ORDER = "display_order";              // 表示順
		public const string FIELD_USERMANAGEMENTLEVEL_DATE_CREATED = "date_created";                // 作成日
		public const string FIELD_USERMANAGEMENTLEVEL_DATE_CHANGED = "date_changed";                // 更新日
		public const string FIELD_USERMANAGEMENTLEVEL_LAST_CHANGED = "last_changed";                // 最終更新者

		// 外部連携ＡＰＩログ
		public const string TABLE_EXTERNALAPILOG = "w2_ExternalApiLog";                             // 外部連携ＡＰＩログ
		public const string FIELD_EXTERNALAPILOG_LOG_ID = "log_id";                                 // ログID
		public const string FIELD_EXTERNALAPILOG_DATE_EXCUTED = "date_excuted";                     // API実行時間
		public const string FIELD_EXTERNALAPILOG_LOG_LEVEL = "log_level";                           // ログレベル
		public const string FIELD_EXTERNALAPILOG_SOURCE = "source";                                 // ソース
		public const string FIELD_EXTERNALAPILOG_STACKTRACE = "stacktrace";                         // スタックトレース
		public const string FIELD_EXTERNALAPILOG_MESSAGE = "message";                               // メッセージ
		public const string FIELD_EXTERNALAPILOG_DATA = "data";                                     // 実行時データ
		public const string FIELD_EXTERNALAPILOG_EXCEPTION = "exception";                           // 例外情報
		public const string FIELD_EXTERNALAPILOG_DATE_CREATED = "date_created";                     // 作成日

		// ユーザ拡張項目設定マスタ
		public const string TABLE_USEREXTENDSETTING = "w2_UserExtendSetting";                       // ユーザ拡張項目設定マスタ
		public const string FIELD_USEREXTENDSETTING_SETTING_ID = "setting_id";                      // ユーザ拡張項目ID
		public const string FIELD_USEREXTENDSETTING_SETTING_NAME = "setting_name";                  // 名称
		public const string FIELD_USEREXTENDSETTING_OUTLINE_KBN = "outline_kbn";                    // ユーザ拡張項目概要表示区分
		public const string FIELD_USEREXTENDSETTING_OUTLINE = "outline";                            // ユーザ拡張項目概要
		public const string FIELD_USEREXTENDSETTING_SORT_ORDER = "sort_order";                      // 並び順
		public const string FIELD_USEREXTENDSETTING_INPUT_TYPE = "input_type";                      // 入力種別
		public const string FIELD_USEREXTENDSETTING_INPUT_DEFAULT = "input_default";                // 初期値
		public const string FIELD_USEREXTENDSETTING_INIT_ONLY_FLG = "init_only_flg";                // 登録時のみフラグ
		public const string FIELD_USEREXTENDSETTING_DISPLAY_KBN = "display_kbn";                    // 表示区分
		public const string FIELD_USEREXTENDSETTING_DATE_CREATED = "date_created";                  // 作成日
		public const string FIELD_USEREXTENDSETTING_DATE_CHANGED = "date_changed";                  // 更新日
		public const string FIELD_USEREXTENDSETTING_LAST_CHANGED = "last_changed";                  // 最終更新者

		// ユーザ拡張項目マスタ
		public const string TABLE_USEREXTEND = "w2_UserExtend";                                     // ユーザ拡張項目マスタ
		public const string FIELD_USEREXTEND_USER_ID = "user_id";                                   // ユーザID
		public const string FIELD_USEREXTEND_DATE_CREATED = "date_created";                         // 作成日
		public const string FIELD_USEREXTEND_DATE_CHANGED = "date_changed";                         // 更新日
		public const string FIELD_USEREXTEND_LAST_CHANGED = "last_changed";                         // 最終更新者

		// ユーザ拡張項目マスタワークテーブル
		public const string TABLE_WORKUSEREXTEND = "w2_WorkUserExtend";                             // ユーザ拡張項目マスタワークテーブル
		public const string FIELD_WORKUSEREXTEND_USER_ID = "user_id";                               // ユーザID
		public const string FIELD_WORKUSEREXTEND_DATE_CREATED = "date_created";                     // 作成日
		public const string FIELD_WORKUSEREXTEND_DATE_CHANGED = "date_changed";                     // 更新日
		public const string FIELD_WORKUSEREXTEND_LAST_CHANGED = "last_changed";                     // 最終更新者

		// 項目メモ設定マスタ
		public const string TABLE_FIELDMEMOSETTING = "w2_FieldMemoSetting";                         // 項目メモ設定マスタ
		public const string FIELD_FIELDMEMOSETTING_TABLE_NAME = "table_name";                       // テーブル名
		public const string FIELD_FIELDMEMOSETTING_FIELD_NAME = "field_name";                       // フィールド名
		public const string FIELD_FIELDMEMOSETTING_MEMO = "memo";                                   // メモ
		public const string FIELD_FIELDMEMOSETTING_DATE_CREATED = "date_created";                   // 作成日
		public const string FIELD_FIELDMEMOSETTING_DATE_CHANGED = "date_changed";                   // 更新日
		public const string FIELD_FIELDMEMOSETTING_LAST_CHANGED = "last_changed";                   // 最終更新者

		// かんたん会員登録設定マスタ
		public const string TABLE_USEREASYREGISTERSETTING = "w2_UserEasyRegisterSetting";           // かんたん会員登録設定マスタ
		public const string FIELD_USEREASYREGISTERSETTING_ITEM_ID = "item_id";                      // 項目ID
		public const string FIELD_USEREASYREGISTERSETTING_DISPLAY_FLG = "display_flg";              // 表示フラグ
		public const string FIELD_USEREASYREGISTERSETTING_DATE_CREATED = "date_created";            // 作成日
		public const string FIELD_USEREASYREGISTERSETTING_DATE_CHANGED = "date_changed";            // 更新日
		public const string FIELD_USEREASYREGISTERSETTING_LAST_CHANGED = "last_changed";            // 最終更新者

		// 商品定期購入割引設定
		public const string TABLE_PRODUCTFIXEDPURCHASEDISCOUNTSETTING = "w2_ProductFixedPurchaseDiscountSetting";	// 商品定期購入割引設定テーブル
		public const string FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_SHOP_ID = "shop_id";					// 店舗ID
		public const string FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_PRODUCT_ID = "product_id";			// 商品ID
		public const string FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT = "order_count";			// 購入回数
		public const string FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT_MORE_THAN_FLG = "order_count_more_than_flg";// 購入回数以降フラグ
		public const string FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_PRODUCT_COUNT = "product_count";		// 商品個数
		public const string FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_VALUE = "discount_value";	// 値引き
		public const string FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE = "discount_type";		// 値引きタイプ
		public const string FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_VALUE = "point_value";			// 付与ポイント
		public const string FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_TYPE = "point_type";			// 付与ポイントタイプ
		public const string FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DATE_CREATED = "date_created";		// 作成日
		public const string FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DATE_CHANGED = "date_changed";		// 更新日
		public const string FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_LAST_CHANGED = "last_changed";		// 最終更新者

		// 商品グループマスタ
		public const string TABLE_PRODUCTGROUP = "w2_ProductGroup";                                 // 商品グループマスタ
		public const string FIELD_PRODUCTGROUP_PRODUCT_GROUP_ID = "product_group_id";               // 商品グループID
		public const string FIELD_PRODUCTGROUP_PRODUCT_GROUP_NAME = "product_group_name";           // 商品グループ名
		public const string FIELD_PRODUCTGROUP_BEGIN_DATE = "begin_date";                           // 開始日時
		public const string FIELD_PRODUCTGROUP_END_DATE = "end_date";                               // 終了日時
		public const string FIELD_PRODUCTGROUP_VALID_FLG = "valid_flg";                             // 有効フラグ
		public const string FIELD_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS_KBN = "product_group_page_contents_kbn";// 商品グループページ表示内容HTML区分
		public const string FIELD_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS = "product_group_page_contents"; // 商品グループページ表示内容
		public const string FIELD_PRODUCTGROUP_DATE_CREATED = "date_created";                       // 作成日
		public const string FIELD_PRODUCTGROUP_DATE_CHANGED = "date_changed";                       // 更新日
		public const string FIELD_PRODUCTGROUP_LAST_CHANGED = "last_changed";                       // 最終更新者

		// 商品グループアイテムマスタ
		public const string TABLE_PRODUCTGROUPITEM = "w2_ProductGroupItem";                         // 商品グループアイテムマスタ
		public const string FIELD_PRODUCTGROUPITEM_PRODUCT_GROUP_ID = "product_group_id";           // 商品グループID
		public const string FIELD_PRODUCTGROUPITEM_ITEM_NO = "item_no";                             // 商品グループアイテム枝番
		public const string FIELD_PRODUCTGROUPITEM_ITEM_TYPE = "item_type";                         // アイテムタイプ
		public const string FIELD_PRODUCTGROUPITEM_SHOP_ID = "shop_id";                             // 店舗ID
		public const string FIELD_PRODUCTGROUPITEM_MASTER_ID = "master_id";                         // マスタID
		public const string FIELD_PRODUCTGROUPITEM_DATE_CREATED = "date_created";                   // 作成日
		public const string FIELD_PRODUCTGROUPITEM_LAST_CHANGED = "last_changed";                   // 最終更新者

		// 休日マスタ
		public const string TABLE_HOLIDAY = "w2_Holidays";					// 休日マスタ
		public const string FIELD_HOLIDAY_YEAR_MONTH = "year_month";		// 年月
		public const string FIELD_HOLIDAY_DAYS = "days";					// 当月の休日（カンマ区切り）
		public const string FIELD_HOLIDAY_DATE_CREATED = "date_created";	// 作成日
		public const string FIELD_HOLIDAY_DATE_CHANGED = "date_changed";	// 更新日
		public const string FIELD_HOLIDAY_LAST_CHANGED = "last_changed";	// 最終更新者


		// 定期購入継続分析テーブル
		public const string TABLE_FIXEDPURCHASEREPEATANALYSIS = "w2_FixedPurchaseRepeatAnalysis";   // 定期購入継続分析テーブル
		public const string FIELD_FIXEDPURCHASEREPEATANALYSIS_DATA_NO = "data_no";                  // データ番号
		public const string FIELD_FIXEDPURCHASEREPEATANALYSIS_USER_ID = "user_id";                  // ユーザーID
		public const string FIELD_FIXEDPURCHASEREPEATANALYSIS_PRODUCT_ID = "product_id";            // 商品ID
		public const string FIELD_FIXEDPURCHASEREPEATANALYSIS_VARIATION_ID = "variation_id";        // バリエーションID
		public const string FIELD_FIXEDPURCHASEREPEATANALYSIS_COUNT = "count";                      // 回数
		public const string FIELD_FIXEDPURCHASEREPEATANALYSIS_ORDER_ID = "order_id";                // 注文ID
		public const string FIELD_FIXEDPURCHASEREPEATANALYSIS_FIXED_PURCHASE_ID = "fixed_purchase_id";// 定期購入ID
		public const string FIELD_FIXEDPURCHASEREPEATANALYSIS_STATUS = "status";                    // ステータス
		public const string FIELD_FIXEDPURCHASEREPEATANALYSIS_DATE_CREATED = "date_created";        // 作成日
		public const string FIELD_FIXEDPURCHASEREPEATANALYSIS_DATE_CHANGED = "date_changed";        // 更新日
		public const string FIELD_FIXEDPURCHASEREPEATANALYSIS_LAST_CHANGED = "last_changed";        // 最終更新者

		// 更新履歴情報
		public const string TABLE_UPDATEHISTORY = "w2_UpdateHistory";                               // 更新履歴情報
		public const string FIELD_UPDATEHISTORY_UPDATE_HISTORY_NO = "update_history_no";            // 更新履歴No
		public const string FIELD_UPDATEHISTORY_UPDATE_HISTORY_KBN = "update_history_kbn";          // 更新履歴区分
		public const string FIELD_UPDATEHISTORY_USER_ID = "user_id";                                // ユーザーID
		public const string FIELD_UPDATEHISTORY_MASTER_ID = "master_id";                            // マスタID
		public const string FIELD_UPDATEHISTORY_UPDATE_HISTORY_ACTION = "update_history_action";    // 更新履歴アクション
		public const string FIELD_UPDATEHISTORY_UPDATE_DATA = "update_data";                        // 更新データ
		public const string FIELD_UPDATEHISTORY_UPDATE_DATA_HASH = "update_data_hash";              // 更新データハッシュ
		public const string FIELD_UPDATEHISTORY_DATE_CREATED = "date_created";                      // 作成日
		public const string FIELD_UPDATEHISTORY_LAST_CHANGED = "last_changed";                      // 最終更新者

		// ＆mall在庫引当
		public const string TABLE_ANDMALLINVENTORYRESERVE = "w2_AndmallInventoryReserve";           // ＆mall在庫引当
		public const string FIELD_ANDMALLINVENTORYRESERVE_IDENTIFICATION_CODE = "identification_code";// 識別コード
		public const string FIELD_ANDMALLINVENTORYRESERVE_SKU_ID = "sku_id";                        // SKUコード
		public const string FIELD_ANDMALLINVENTORYRESERVE_ANDMALL_BASE_STORE_CODE = "andmall_base_store_code";// ショップコード
		public const string FIELD_ANDMALLINVENTORYRESERVE_PRODUCT_ID = "product_id";                // 商品ID
		public const string FIELD_ANDMALLINVENTORYRESERVE_VARIATION_ID = "variation_id";            // 商品バリエーションID
		public const string FIELD_ANDMALLINVENTORYRESERVE_QUANTITY = "quantity";                    // 数量
		public const string FIELD_ANDMALLINVENTORYRESERVE_STATUS = "status";                        // ステータス
		public const string FIELD_ANDMALLINVENTORYRESERVE_DATE_CREATED = "date_created";            // 作成日
		public const string FIELD_ANDMALLINVENTORYRESERVE_DATE_CHANGED = "date_changed";            // 更新日
		public const string FIELD_ANDMALLINVENTORYRESERVE_CANCEL_DATE = "cancel_date";

		// リージョン判定IP範囲テーブル
		public const string TABLE_COUNTRYIPV4 = "w2_CountryIpv4";                                   // リージョン判定IP範囲テーブル
		public const string FIELD_COUNTRYIPV4_IP_NUMERIC = "ip_numeric";                            // ネットワークアドレス_数値
		public const string FIELD_COUNTRYIPV4_IP_BROADCAST_NUMERIC = "ip_broadcast_numeric";        // ブロードキャスト_数値
		public const string FIELD_COUNTRYIPV4_IP = "ip";                                            // ネットワークアドレス
		public const string FIELD_COUNTRYIPV4_IP_BROADCAST = "ip_broadcast";                        // ブロードキャスト
		public const string FIELD_COUNTRYIPV4_GEONAME_ID = "geoname_id";                            // リージョンID
		public const string FIELD_COUNTRYIPV4_DATE_CHANGED = "date_changed";                        // 最終更新日時

		// 国ISOコードのマスタテーブル
		public const string TABLE_COUNTRYLOCATION = "w2_CountryLocation";                           // 国ISOコードのマスタテーブル
		public const string FIELD_COUNTRYLOCATION_GEONAME_ID = "geoname_id";                        // リージョンID
		public const string FIELD_COUNTRYLOCATION_COUNTRY_ISO_CODE = "country_iso_code";            // 国ISOコード
		public const string FIELD_COUNTRYLOCATION_COUNTRY_NAME = "country_name";                    // 国名
		public const string FIELD_COUNTRYLOCATION_DATE_CHANGED = "date_changed";                    // 最終更新日時

		// 名称翻訳設定マスタ
		public const string TABLE_NAMETRANSLATIONSETTING = "w2_NameTranslationSetting";             // 名称翻訳設定マスタ
		public const string FIELD_NAMETRANSLATIONSETTING_DATA_KBN = "data_kbn";                     // 対象データ区分
		public const string FIELD_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN = "translation_target_column";// 翻訳対象項目
		public const string FIELD_NAMETRANSLATIONSETTING_MASTER_ID1 = "master_id1";                 // マスタID1
		public const string FIELD_NAMETRANSLATIONSETTING_MASTER_ID2 = "master_id2";                 // マスタID2
		public const string FIELD_NAMETRANSLATIONSETTING_MASTER_ID3 = "master_id3";                 // マスタID3
		public const string FIELD_NAMETRANSLATIONSETTING_LANGUAGE_CODE = "language_code";           // 言語コード
		public const string FIELD_NAMETRANSLATIONSETTING_LANGUAGE_LOCALE_ID = "language_locale_id"; // 言語ロケールID
		public const string FIELD_NAMETRANSLATIONSETTING_AFTER_TRANSLATIONAL_NAME = "after_translational_name";// 翻訳後名称
		public const string FIELD_NAMETRANSLATIONSETTING_DISPLAY_KBN = "display_kbn";               // 表示HTML区分
		public const string FIELD_NAMETRANSLATIONSETTING_DATE_CREATED = "date_created";             // 作成日
		public const string FIELD_NAMETRANSLATIONSETTING_DATE_CHANGED = "date_changed";             // 更新日

		// 名称翻訳設定マスタ用ワーク
		public const string TABLE_WORKNAMETRANSLATIONSETTING = "w2_WorkNameTranslationSetting";     // 名称翻訳設定マスタ用ワーク
		public const string FIELD_WORKNAMETRANSLATIONSETTING_DATA_KBN = "data_kbn";                 // 対象データ区分
		public const string FIELD_WORKNAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN = "translation_target_column";// 翻訳対象項目
		public const string FIELD_WORKNAMETRANSLATIONSETTING_MASTER_ID1 = "master_id1";             // マスタID1
		public const string FIELD_WORKNAMETRANSLATIONSETTING_MASTER_ID2 = "master_id2";             // マスタID2
		public const string FIELD_WORKNAMETRANSLATIONSETTING_MASTER_ID3 = "master_id3";             // マスタID3
		public const string FIELD_WORKNAMETRANSLATIONSETTING_LANGUAGE_CODE = "language_code";       // 言語コード
		public const string FIELD_WORKNAMETRANSLATIONSETTING_LANGUAGE_LOCALE_ID = "language_locale_id";// 言語ロケールID
		public const string FIELD_WORKNAMETRANSLATIONSETTING_AFTER_TRANSLATIONAL_NAME = "after_translational_name";// 翻訳後名称
		public const string FIELD_WORKNAMETRANSLATIONSETTING_DISPLAY_KBN = "display_kbn";           // 表示HTML区分
		public const string FIELD_WORKNAMETRANSLATIONSETTING_DATE_CREATED = "date_created";         // 作成日
		public const string FIELD_WORKNAMETRANSLATIONSETTING_DATE_CHANGED = "date_changed";         // 更新日

		// メールテンプレートグローバルマスタ
		public const string TABLE_MAILTEMPLATEGLOBAL = "w2_MailTemplateGlobal";                     // メールテンプレートグローバルマスタ
		public const string FIELD_MAILTEMPLATEGLOBAL_SHOP_ID = "shop_id";                           // 店舗ID
		public const string FIELD_MAILTEMPLATEGLOBAL_MAIL_ID = "mail_id";                           // メールテンプレートID
		public const string FIELD_MAILTEMPLATEGLOBAL_LANGUAGE_CODE = "language_code";               // 言語コード
		public const string FIELD_MAILTEMPLATEGLOBAL_LANGUAGE_LOCALE_ID = "language_locale_id";     // 言語ロケールID
		public const string FIELD_MAILTEMPLATEGLOBAL_MAIL_NAME = "mail_name";                       // メールテンプレート名
		public const string FIELD_MAILTEMPLATEGLOBAL_MAIL_FROM = "mail_from";                       // メールFrom
		public const string FIELD_MAILTEMPLATEGLOBAL_MAIL_TO = "mail_to";                           // メールTo
		public const string FIELD_MAILTEMPLATEGLOBAL_MAIL_CC = "mail_cc";                           // メールCC
		public const string FIELD_MAILTEMPLATEGLOBAL_MAIL_BCC = "mail_bcc";                         // メールBCC
		public const string FIELD_MAILTEMPLATEGLOBAL_MAIL_SUBJECT = "mail_subject";                 // メールタイトル
		public const string FIELD_MAILTEMPLATEGLOBAL_MAIL_BODY = "mail_body";                       // メール本文
		public const string FIELD_MAILTEMPLATEGLOBAL_MAIL_ATTACHMENTFILE_PATH = "mail_attachmentfile_path";// メール添付ファイルパス
		public const string FIELD_MAILTEMPLATEGLOBAL_DATE_CREATED = "date_created";                 // 作成日
		public const string FIELD_MAILTEMPLATEGLOBAL_DATE_CHANGED = "date_changed";                 // 更新日
		public const string FIELD_MAILTEMPLATEGLOBAL_LAST_CHANGED = "last_changed";                 // 最終更新者
		public const string FIELD_MAILTEMPLATEGLOBAL_MAIL_FROM_NAME = "mail_from_name";             // 送信元名
		public const string FIELD_MAILTEMPLATEGLOBAL_MAIL_CATEGORY = "mail_category";               // メールカテゴリ
		public const string FIELD_MAILTEMPLATEGLOBAL_AUTO_SEND_FLG = "auto_send_flg";               // 自動送信フラグ

		// 商品在庫文言グローバルマスタ
		public const string TABLE_PRODUCTSTOCKMESSAGEGLOBAL = "w2_ProductStockMessageGlobal";       // 商品在庫文言グローバルマスタ
		public const string FIELD_PRODUCTSTOCKMESSAGEGLOBAL_SHOP_ID = "shop_id";                    // 店舗ID
		public const string FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE_ID = "stock_message_id";  // 商品在庫文言ID
		public const string FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LANGUAGE_CODE = "language_code";        // 言語コード
		public const string FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LANGUAGE_LOCALE_ID = "language_locale_id";// 言語ロケールID
		public const string FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE_NAME = "stock_message_name";// 在庫文言名
		public const string FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE_DEF = "stock_message_def";// 標準在庫文言
		public const string FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE1 = "stock_message1";      // 在庫文言1
		public const string FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE2 = "stock_message2";      // 在庫文言2
		public const string FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE3 = "stock_message3";      // 在庫文言3
		public const string FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE4 = "stock_message4";      // 在庫文言4
		public const string FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE5 = "stock_message5";      // 在庫文言5
		public const string FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE6 = "stock_message6";      // 在庫文言6
		public const string FIELD_PRODUCTSTOCKMESSAGEGLOBAL_STOCK_MESSAGE7 = "stock_message7";      // 在庫文言7
		public const string FIELD_PRODUCTSTOCKMESSAGEGLOBAL_DATE_CREATED = "date_created";          // 作成日
		public const string FIELD_PRODUCTSTOCKMESSAGEGLOBAL_DATE_CHANGED = "date_changed";          // 更新日
		public const string FIELD_PRODUCTSTOCKMESSAGEGLOBAL_LAST_CHANGED = "last_changed";          // 最終更新者

		// 海外配送エリア
		public const string TABLE_GLOBALSHIPPINGAREA = "w2_GlobalShippingArea";                     // 海外配送エリア
		public const string FIELD_GLOBALSHIPPINGAREA_GLOBAL_SHIPPING_AREA_ID = "global_shipping_area_id";// 海外配送エリアID
		public const string FIELD_GLOBALSHIPPINGAREA_GLOBAL_SHIPPING_AREA_NAME = "global_shipping_area_name";// 海外配送エリア名
		public const string FIELD_GLOBALSHIPPINGAREA_SORT_NO = "sort_no";                           // 並び順
		public const string FIELD_GLOBALSHIPPINGAREA_VALID_FLG = "valid_flg";                       // 有効フラグ
		public const string FIELD_GLOBALSHIPPINGAREA_DATE_CREATED = "date_created";                 // 作成日
		public const string FIELD_GLOBALSHIPPINGAREA_DATE_CHANGED = "date_changed";                 // 更新日
		public const string FIELD_GLOBALSHIPPINGAREA_LAST_CHANGED = "last_changed";                 // 最終更新者

		// 海外配送エリア構成
		public const string TABLE_GLOBALSHIPPINGAREACOMPONENT = "w2_GlobalShippingAreaComponent";   // 海外配送エリア構成
		public const string FIELD_GLOBALSHIPPINGAREACOMPONENT_SEQ = "seq";                          // シーケンス
		public const string FIELD_GLOBALSHIPPINGAREACOMPONENT_GLOBAL_SHIPPING_AREA_ID = "global_shipping_area_id";// 地域ID
		public const string FIELD_GLOBALSHIPPINGAREACOMPONENT_COUNTRY_ISO_CODE = "country_iso_code";// ISO国コード
		public const string FIELD_GLOBALSHIPPINGAREACOMPONENT_CONDITION_ADDR5 = "condition_addr5";  // 住所5条件
		public const string FIELD_GLOBALSHIPPINGAREACOMPONENT_CONDITION_ADDR4 = "condition_addr4";  // 住所4条件
		public const string FIELD_GLOBALSHIPPINGAREACOMPONENT_CONDITION_ADDR3 = "condition_addr3";  // 住所3条件
		public const string FIELD_GLOBALSHIPPINGAREACOMPONENT_CONDITION_ADDR2 = "condition_addr2";  // 住所2条件
		public const string FIELD_GLOBALSHIPPINGAREACOMPONENT_CONDITION_ZIP = "condition_zip";      // 郵便番号条件
		public const string FIELD_GLOBALSHIPPINGAREACOMPONENT_DATE_CREATED = "date_created";        // 作成日
		public const string FIELD_GLOBALSHIPPINGAREACOMPONENT_DATE_CHANGED = "date_changed";        // 更新日
		public const string FIELD_GLOBALSHIPPINGAREACOMPONENT_LAST_CHANGED = "last_changed";        // 最終更新者

		// 海外配送料金表
		public const string TABLE_GLOBALSHIPPINGPOSTAGE = "w2_GlobalShippingPostage";               // 海外配送料金表
		public const string FIELD_GLOBALSHIPPINGPOSTAGE_SEQ = "seq";                                // シーケンス
		public const string FIELD_GLOBALSHIPPINGPOSTAGE_SHOP_ID = "shop_id";                        // 店舗ID
		public const string FIELD_GLOBALSHIPPINGPOSTAGE_SHIPPING_ID = "shipping_id";                // 配送種別ID
		public const string FIELD_GLOBALSHIPPINGPOSTAGE_DELIVERY_COMPANY_ID = "delivery_company_id";// 配送会社ID
		public const string FIELD_GLOBALSHIPPINGPOSTAGE_GLOBAL_SHIPPING_AREA_ID = "global_shipping_area_id";// 地域ID
		public const string FIELD_GLOBALSHIPPINGPOSTAGE_WEIGHT_GRAM_GREATER_THAN_OR_EQUAL_TO = "weight_gram_greater_than_or_equal_to";// 重量（～g以上）
		public const string FIELD_GLOBALSHIPPINGPOSTAGE_WEIGHT_GRAM_LESS_THAN = "weight_gram_less_than";// 重量（～g未満）
		public const string FIELD_GLOBALSHIPPINGPOSTAGE_GLOBAL_SHIPPING_POSTAGE = "global_shipping_postage";// 送料
		public const string FIELD_GLOBALSHIPPINGPOSTAGE_DATE_CREATED = "date_created";              // 作成日
		public const string FIELD_GLOBALSHIPPINGPOSTAGE_DATE_CHANGED = "date_changed";              // 更新日
		public const string FIELD_GLOBALSHIPPINGPOSTAGE_LAST_CHANGED = "last_changed";              // 最終更新者

		// SMSエラーポイント
		public const string TABLE_SMSERRORPOINTGLOBALTELNO = "w2_SMSErrorPointGlobalTelNo";         // SMSエラーポイント
		public const string FIELD_SMSERRORPOINTGLOBALTELNO_GLOBAL_TEL_NO = "global_tel_no";         // グローバル電話番号
		public const string FIELD_SMSERRORPOINTGLOBALTELNO_ERROR_POINT = "error_point";             // エラーポイント
		public const string FIELD_SMSERRORPOINTGLOBALTELNO_DATE_CREATED = "date_created";           // 作成日
		public const string FIELD_SMSERRORPOINTGLOBALTELNO_DATE_CHANGED = "date_changed";           // 更新日
		public const string FIELD_SMSERRORPOINTGLOBALTELNO_LAST_CHANGED = "last_changed";           // 最終更新者

		// SMSステータス
		public const string TABLE_GLOBALSMSSTATUS = "w2_GlobalSMSStatus";                           // SMSステータス
		public const string FIELD_GLOBALSMSSTATUS_MESSAGE_ID = "message_id";                        // メッセージID
		public const string FIELD_GLOBALSMSSTATUS_GLOBAL_TEL_NO = "global_tel_no";                  // グローバル電話番号
		public const string FIELD_GLOBALSMSSTATUS_SMS_STATUS = "sms_status";                        // SMSステータス
		public const string FIELD_GLOBALSMSSTATUS_DATE_CREATED = "date_created";                    // 作成日
		public const string FIELD_GLOBALSMSSTATUS_DATE_CHANGED = "date_changed";                    // 更新日
		public const string FIELD_GLOBALSMSSTATUS_LAST_CHANGED = "last_changed";                    // 最終更新者

		// SMSテンプレート
		public const string TABLE_GLOBALSMSTEMPLATE = "w2_GlobalSMSTemplate";                       // SMSテンプレート
		public const string FIELD_GLOBALSMSTEMPLATE_SHOP_ID = "shop_id";                            // 店舗ID
		public const string FIELD_GLOBALSMSTEMPLATE_MAIL_ID = "mail_id";                            // メールテンプレートID
		public const string FIELD_GLOBALSMSTEMPLATE_PHONE_CARRIER = "phone_carrier";                // キャリア
		public const string FIELD_GLOBALSMSTEMPLATE_SMS_TEXT = "sms_text";                          // SMS本文
		public const string FIELD_GLOBALSMSTEMPLATE_DATE_CREATED = "date_created";                  // 作成日
		public const string FIELD_GLOBALSMSTEMPLATE_DATE_CHANGED = "date_changed";                  // 更新日
		public const string FIELD_GLOBALSMSTEMPLATE_LAST_CHANGED = "last_changed";                  // 最終更新者

		// メッセージアプリ向けコンテンツ
		public const string TABLE_MESSAGINGAPPCONTENTS = "w2_MessagingAppContents";                 // メッセージ送信内容
		public const string FIELD_MESSAGINGAPPCONTENTS_DEPT_ID = "dept_id";                         // 識別ID
		public const string FIELD_MESSAGINGAPPCONTENTS_MASTER_KBN = "master_kbn";                   // マスタ区分
		public const string FIELD_MESSAGINGAPPCONTENTS_MASTER_ID = "master_id";                     // マスタID
		public const string FIELD_MESSAGINGAPPCONTENTS_BRANCH_NO = "branch_no";                     // 枝番
		public const string FIELD_MESSAGINGAPPCONTENTS_MESSAGING_APP_KBN = "messaging_app_kbn";    // メッセージアプリ区分
		public const string FIELD_MESSAGINGAPPCONTENTS_MEDIA_TYPE = "media_type";                   // メディアタイプ
		public const string FIELD_MESSAGINGAPPCONTENTS_CONTENTS = "contents";                       // コンテンツ
		public const string FIELD_MESSAGINGAPPCONTENTS_DATE_CREATED = "date_created";               // 作成日
		public const string FIELD_MESSAGINGAPPCONTENTS_DATE_CHANGED = "date_changed";               // 更新日
		public const string FIELD_MESSAGINGAPPCONTENTS_LAST_CHANGED = "last_changed";               // 最終更新者

		// 自動翻訳ワード管理
		public const string TABLE_AUTOTRANSLATIONWORD = "w2_AutoTranslationWord";                   // 自動翻訳ワード管理
		public const string FIELD_AUTOTRANSLATIONWORD_WORD_HASH_KEY = "word_hash_key";              // 翻訳前ワード ハッシュキー
		public const string FIELD_AUTOTRANSLATIONWORD_LANGUAGE_CODE = "language_code";              // 言語コード
		public const string FIELD_AUTOTRANSLATIONWORD_WORD_BEFORE = "word_before";                  // 翻訳前ワード
		public const string FIELD_AUTOTRANSLATIONWORD_WORD_AFTER = "word_after";                    // 翻訳後ワード
		public const string FIELD_AUTOTRANSLATIONWORD_CLEAR_FLG = "clear_flg";                      // 削除対象フラグ
		public const string FIELD_AUTOTRANSLATIONWORD_DATE_USED = "date_used";                      // 最終利用日時
		public const string FIELD_AUTOTRANSLATIONWORD_DATE_CREATED = "date_created";                // 作成日
		public const string FIELD_AUTOTRANSLATIONWORD_DATE_CHANGED = "date_changed";                // 更新日
		public const string FIELD_AUTOTRANSLATIONWORD_LAST_CHANGED = "last_changed";                // 最終更新者

		// 配送リードタイムマスタ
		public const string TABLE_DELIVERYLEADTIME = "w2_DeliveryLeadTime";							// 配送リードタイムマスタ
		public const string FIELD_DELIVERYLEADTIME_SHOP_ID = "shop_id";								// 店舗ID
		public const string FIELD_DELIVERYLEADTIME_DELIVERY_COMPANY_ID = "delivery_company_id";		// 配送会社ID
		public const string FIELD_DELIVERYLEADTIME_LEAD_TIME_ZONE_NO = "lead_time_zone_no";			// リードタイム地帯区分
		public const string FIELD_DELIVERYLEADTIME_LEAD_TIME_ZONE_NAME = "lead_time_zone_name";		// 地帯名
		public const string FIELD_DELIVERYLEADTIME_ZIP = "zip";										// 郵便番号
		public const string FIELD_DELIVERYLEADTIME_SHIPPING_LEAD_TIME = "shipping_lead_time";		// 追加配送リードタイム
		public const string FIELD_DELIVERYLEADTIME_DATE_CREATED = "date_created";					// 作成日
		public const string FIELD_DELIVERYLEADTIME_DATE_CHANGED = "date_changed";					// 更新日
		public const string FIELD_DELIVERYLEADTIME_LAST_CHANGED = "last_changed";					// 最終更新者

		// Lpページデザイン
		public const string TABLE_LANDINGPAGEDESIGN = "w2_LandingPageDesign";                       // Lpページデザイン
		public const string FIELD_LANDINGPAGEDESIGN_PAGE_ID = "page_id";                            // ページID
		public const string FIELD_LANDINGPAGEDESIGN_PAGE_TITLE = "page_title";                      // ページタイトル
		public const string FIELD_LANDINGPAGEDESIGN_PAGE_FILE_NAME = "page_file_name";              // ページファイル名
		public const string FIELD_LANDINGPAGEDESIGN_PUBLIC_STATUS = "public_status";                // 公開状態
		public const string FIELD_LANDINGPAGEDESIGN_PUBLIC_START_DATETIME = "public_start_datetime";// 公開開始日時
		public const string FIELD_LANDINGPAGEDESIGN_PUBLIC_END_DATETIME = "public_end_datetime";    // 公開終了日時
		public const string FIELD_LANDINGPAGEDESIGN_PRODUCT_CHOOSE_TYPE = "product_choose_type";    // 商品選択タイプ
		public const string FIELD_LANDINGPAGEDESIGN_USER_REGISTRATION_TYPE = "user_registration_type";// 会員登録タイプ
		public const string FIELD_LANDINGPAGEDESIGN_LOGIN_FORM_TYPE = "login_form_type";            // ログインフォームタイプ
		public const string FIELD_LANDINGPAGEDESIGN_DATE_CREATED = "date_created";                  // 作成日
		public const string FIELD_LANDINGPAGEDESIGN_DATE_CHANGED = "date_changed";                  // 更新日
		public const string FIELD_LANDINGPAGEDESIGN_LAST_CHANGED = "last_changed";                  // 最終更新者
		public const string FIELD_LANDINGPAGEDESIGN_METADATA_DESC = "metadata_desc";                // SEOディスクリプション
		public const string FIELD_LANDINGPAGEDESIGN_MANAGEMENT_TITLE = "management_title";          // 管理用タイトル
		public const string FIELD_LANDINGPAGEDESIGN_SOCIAL_LOGIN_USE_TYPE = "social_login_use_type";// 利用するソーシャルログインタイプ
		public const string FIELD_LANDINGPAGEDESIGN_SOCIAL_LOGIN_LIST = "social_login_list";        // ソーシャルログインリスト
		public const string FIELD_LANDINGPAGEDESIGN_EFO_CUBE_USE_FLG = "efo_cube_use_flg";          // EFO CUBE利用
		public const string FIELD_LANDINGPAGEDESIGN_ORDER_CONFIRM_PAGE_SKIP_FLG = "order_confirm_page_skip_flg";// 確認画面スキップ
		public const string FIELD_LANDINGPAGEDESIGN_MAIL_ADDRESS_CONFIRM_FORM_USE_FLG = "mail_address_confirm_form_use_flg";// メールアドレス確認フォーム利用フラグ
		public const string FIELD_LANDINGPAGEDESIGN_UNPERMITTED_PAYMENT_IDS = "unpermitted_payment_ids";// 除外する決済種別IDリスト
		public const string FIELD_LANDINGPAGEDESIGN_TAG_SETTING_LIST = "tag_setting_list";        // タグ設定リスト
		public const string FIELD_LANDINGPAGEDESIGN_PAYMENT_CHOOSE_TYPE = "payment_choose_type";// LP決済種別選択タイプ
		/// <summary>デフォルト決済種別</summary>
		public const string FIELD_LANDINGPAGEDESIGN_DEFAULT_PAYMENT_ID = "default_payment_id";      // デフォルト決済種別
		/// <summary>ノベルティ利用フラグ</summary>
		public const string FIELD_LANDINGPAGEDESIGN_NOVELTY_USE_FLG = "novelty_use_flg";
		/// <summary>デザインモード</summary>
		public const string FIELD_LANDINGPAGEDESIGN_DESIGN_MODE = "design_mode";                    // デザインモード
		/// <summary>Personal authentication use flag</summary>
		public const string FIELD_LANDINGPAGEDESIGN_PERSONAL_AUTHENTICATION_USE_FLG = "personal_authentication_use_flg";

		// Lpページブロックデザイン
		public const string TABLE_LANDINGPAGEDESIGNBLOCK = "w2_LandingPageDesignBlock";             // Lpページブロックデザイン
		public const string FIELD_LANDINGPAGEDESIGNBLOCK_PAGE_ID = "page_id";                       // ページID
		public const string FIELD_LANDINGPAGEDESIGNBLOCK_DESIGN_TYPE = "design_type";               // デザインタイプ
		public const string FIELD_LANDINGPAGEDESIGNBLOCK_BLOCK_INDEX = "block_index";               // ブロックインデックス
		public const string FIELD_LANDINGPAGEDESIGNBLOCK_BLOCK_CLASS_NAME = "block_class_name";     // ブロッククラス名
		public const string FIELD_LANDINGPAGEDESIGNBLOCK_DATE_CREATED = "date_created";             // 作成日
		public const string FIELD_LANDINGPAGEDESIGNBLOCK_DATE_CHANGED = "date_changed";             // 更新日
		public const string FIELD_LANDINGPAGEDESIGNBLOCK_LAST_CHANGED = "last_changed";             // 最終更新者

		// Lpページ要素デザイン
		public const string TABLE_LANDINGPAGEDESIGNELEMENT = "w2_LandingPageDesignElement";         // Lpページ要素デザイン
		public const string FIELD_LANDINGPAGEDESIGNELEMENT_PAGE_ID = "page_id";                     // ページID
		public const string FIELD_LANDINGPAGEDESIGNELEMENT_DESIGN_TYPE = "design_type";             // デザインタイプ
		public const string FIELD_LANDINGPAGEDESIGNELEMENT_BLOCK_INDEX = "block_index";             // ブロックインデックス
		public const string FIELD_LANDINGPAGEDESIGNELEMENT_ELEMENT_INDEX = "element_index";         // 要素インデックス
		public const string FIELD_LANDINGPAGEDESIGNELEMENT_ELEMENT_PLACE_HOLDER_NAME = "element_place_holder_name";// 要素プレースホルダ―名
		public const string FIELD_LANDINGPAGEDESIGNELEMENT_DATE_CREATED = "date_created";           // 作成日
		public const string FIELD_LANDINGPAGEDESIGNELEMENT_DATE_CHANGED = "date_changed";           // 更新日
		public const string FIELD_LANDINGPAGEDESIGNELEMENT_LAST_CHANGED = "last_changed";           // 最終更新者

		// Lpページ属性デザイン
		public const string TABLE_LANDINGPAGEDESIGNATTRIBUTE = "w2_LandingPageDesignAttribute";     // Lpページ属性デザイン
		public const string FIELD_LANDINGPAGEDESIGNATTRIBUTE_PAGE_ID = "page_id";                   // ページID
		public const string FIELD_LANDINGPAGEDESIGNATTRIBUTE_DESIGN_TYPE = "design_type";           // デザインタイプ
		public const string FIELD_LANDINGPAGEDESIGNATTRIBUTE_BLOCK_INDEX = "block_index";           // ブロックインデックス
		public const string FIELD_LANDINGPAGEDESIGNATTRIBUTE_ELEMENT_INDEX = "element_index";       // 要素インデックス
		public const string FIELD_LANDINGPAGEDESIGNATTRIBUTE_ATTRIBUTE = "attribute";               // 属性
		public const string FIELD_LANDINGPAGEDESIGNATTRIBUTE_VALUE = "value";                       // 値
		public const string FIELD_LANDINGPAGEDESIGNATTRIBUTE_DATE_CREATED = "date_created";         // 作成日
		public const string FIELD_LANDINGPAGEDESIGNATTRIBUTE_DATE_CHANGED = "date_changed";         // 更新日
		public const string FIELD_LANDINGPAGEDESIGNATTRIBUTE_LAST_CHANGED = "last_changed";         // 最終更新者

		// Lpページ商品セット
		public const string TABLE_LANDINGPAGEPRODUCTSET = "w2_LandingPageProductSet";               // Lpページ商品セット
		public const string FIELD_LANDINGPAGEPRODUCTSET_PAGE_ID = "page_id";                        // ページID
		public const string FIELD_LANDINGPAGEPRODUCTSET_BRANCH_NO = "branch_no";                    // 枝番
		public const string FIELD_LANDINGPAGEPRODUCTSET_SET_NAME = "set_name";                      // セット名
		public const string FIELD_LANDINGPAGEPRODUCTSET_BUY_TYPE = "buy_type";                      // 購入タイプ
		public const string FIELD_LANDINGPAGEPRODUCTSET_VALID_FLG = "valid_flg";                    // 有効フラグ
		public const string FIELD_LANDINGPAGEPRODUCTSET_DATE_CREATED = "date_created";              // 作成日
		public const string FIELD_LANDINGPAGEPRODUCTSET_DATE_CHANGED = "date_changed";              // 更新日
		public const string FIELD_LANDINGPAGEPRODUCTSET_LAST_CHANGED = "last_changed";              // 最終更新者
		public const string FIELD_LANDINGPAGEPRODUCTSET_SUBSCRIPTION_BOX_COURSE_ID = "subscription_box_course_id";// 頒布会コースID

		// Lpページ商品
		public const string TABLE_LANDINGPAGEPRODUCT = "w2_LandingPageProduct";                     // Lpページ商品
		public const string FIELD_LANDINGPAGEPRODUCT_PAGE_ID = "page_id";                           // ページID
		public const string FIELD_LANDINGPAGEPRODUCT_SHOP_ID = "shop_id";                           // 店舗ID
		public const string FIELD_LANDINGPAGEPRODUCT_PRODUCT_ID = "product_id";                     // 商品ID
		public const string FIELD_LANDINGPAGEPRODUCT_VARIATION_ID = "variation_id";                 // 商品バリエーションID
		public const string FIELD_LANDINGPAGEPRODUCT_VARIATION_SORT_NUMBER = "variation_sort_number";// 商品バリエーション順序
		public const string FIELD_LANDINGPAGEPRODUCT_QUANTITY = "quantity";                         // 個数
		public const string FIELD_LANDINGPAGEPRODUCT_DATE_CREATED = "date_created";                 // 作成日
		public const string FIELD_LANDINGPAGEPRODUCT_DATE_CHANGED = "date_changed";                 // 更新日
		public const string FIELD_LANDINGPAGEPRODUCT_LAST_CHANGED = "last_changed";                 // 最終更新者
		public const string FIELD_LANDINGPAGEPRODUCT_BRANCH_NO = "branch_no";                       // 枝番
		public const string FIELD_LANDINGPAGEPRODUCT_SHIPPING_ID = "shipping_id";                   // 配送種別ID
		/// <summary>購入タイプ</summary>
		public const string FIELD_LANDINGPAGEPRODUCT_BUY_TYPE = "buy_type";

		// 表示設定管理
		public const string TABLE_MANAGERLISTDISPSETTING = "w2_ManagerListDispSetting";				// 表示設定管理
		public const string FIELD_MANAGERLISTDISPSETTING_SHOP_ID = "shop_id";						// 店舗ID
		public const string FIELD_MANAGERLISTDISPSETTING_DISP_SETTING_KBN = "disp_setting_kbn";		// 表示設定先区分
		public const string FIELD_MANAGERLISTDISPSETTING_DISP_COLUMN_NAME = "disp_colmun_name";		// 表示項目名
		public const string FIELD_MANAGERLISTDISPSETTING_DISP_FLAG = "disp_flag";					// 表示フラグ
		public const string FIELD_MANAGERLISTDISPSETTING_DISP_ORDER = "disp_order";					// 項目の表示順
		public const string FIELD_MANAGERLISTDISPSETTING_COLMUN_WIDTH = "colmun_width";				// カラム幅
		public const string FIELD_MANAGERLISTDISPSETTING_COLMUN_ALIGN = "colmun_align";				// 表示位置
		public const string FIELD_MANAGERLISTDISPSETTING_DATE_CHANGED = "date_changed";				// 最終更新日
		public const string FIELD_MANAGERLISTDISPSETTING_LAST_CHANGED = "last_changed";				// 最終更新者

		// Atodene後払い請求書
		public const string TABLE_INVOICEATODENE = "w2_InvoiceAtodene";                             // Atodene後払い請求書
		public const string FIELD_INVOICEATODENE_ORDER_ID = "order_id";                             // 注文ID
		public const string FIELD_INVOICEATODENE_ZIP = "zip";                                       // 郵便番号
		public const string FIELD_INVOICEATODENE_ADDRESS1 = "address1";                             // 住所1
		public const string FIELD_INVOICEATODENE_ADDRESS2 = "address2";                             // 住所2
		public const string FIELD_INVOICEATODENE_COMPANYNAME = "companyName";                       // 会社名
		public const string FIELD_INVOICEATODENE_SECTIONNAME = "sectionName";                       // 部署名
		public const string FIELD_INVOICEATODENE_NAME = "name";                                     // 氏名
		public const string FIELD_INVOICEATODENE_SITENAMETITLE = "siteNameTitle";                   // 加盟店名タイトル
		public const string FIELD_INVOICEATODENE_SITENAME = "siteName";                             // 請求書記載店舗名
		public const string FIELD_INVOICEATODENE_SHOPORDERIDTITLE = "shopOrderIdTitle";             // 加盟店取引IDタイトル
		public const string FIELD_INVOICEATODENE_SHOPORDERID = "shopOrderId";                       // ご購入店受注番号
		public const string FIELD_INVOICEATODENE_DESCRIPTIONTEXT1 = "descriptionText1";             // 請求書記載事項1
		public const string FIELD_INVOICEATODENE_DESCRIPTIONTEXT2 = "descriptionText2";             // 請求書記載事項2
		public const string FIELD_INVOICEATODENE_DESCRIPTIONTEXT3 = "descriptionText3";             // 請求書記載事項3
		public const string FIELD_INVOICEATODENE_DESCRIPTIONTEXT4 = "descriptionText4";             // 請求書記載事項4
		public const string FIELD_INVOICEATODENE_DESCRIPTIONTEXT5 = "descriptionText5";             // 請求書記載事項5
		public const string FIELD_INVOICEATODENE_BILLSERVICENAME = "billServiceName";               // 請求書発行元企業名
		public const string FIELD_INVOICEATODENE_BILLSERVICEINFO1 = "billServiceInfo1";             // 請求書発行元情報1
		public const string FIELD_INVOICEATODENE_BILLSERVICEINFO2 = "billServiceInfo2";             // 請求書発行元情報2
		public const string FIELD_INVOICEATODENE_BILLSERVICEINFO3 = "billServiceInfo3";             // 請求書発行元情報3
		public const string FIELD_INVOICEATODENE_BILLSERVICEINFO4 = "billServiceInfo4";             // 請求書発行元情報4
		public const string FIELD_INVOICEATODENE_BILLSTATE1 = "billState1";                         // 請求書ステータス
		public const string FIELD_INVOICEATODENE_BILLFIRSTGREET1 = "billFirstGreet1";               // 宛名欄挨拶文欄1
		public const string FIELD_INVOICEATODENE_BILLFIRSTGREET2 = "billFirstGreet2";               // 宛名欄挨拶文欄2
		public const string FIELD_INVOICEATODENE_BILLFIRSTGREET3 = "billFirstGreet3";               // 宛名欄挨拶文欄3
		public const string FIELD_INVOICEATODENE_BILLFIRSTGREET4 = "billFirstGreet4";               // 宛名欄挨拶文欄4
		public const string FIELD_INVOICEATODENE_EXPAND1 = "expand1";                               // 予備項目1
		public const string FIELD_INVOICEATODENE_EXPAND2 = "expand2";                               // 予備項目2
		public const string FIELD_INVOICEATODENE_EXPAND3 = "expand3";                               // 予備項目3
		public const string FIELD_INVOICEATODENE_EXPAND4 = "expand4";                               // 予備項目4
		public const string FIELD_INVOICEATODENE_EXPAND5 = "expand5";                               // 予備項目5
		public const string FIELD_INVOICEATODENE_EXPAND6 = "expand6";                               // 予備項目6
		public const string FIELD_INVOICEATODENE_EXPAND7 = "expand7";                               // 予備項目7
		public const string FIELD_INVOICEATODENE_EXPAND8 = "expand8";                               // 予備項目8
		public const string FIELD_INVOICEATODENE_EXPAND9 = "expand9";                               // 予備項目9
		public const string FIELD_INVOICEATODENE_EXPAND10 = "expand10";                             // 予備項目10
		public const string FIELD_INVOICEATODENE_BILLEDAMOUNTTITLE = "billedAmountTitle";           // 請求金額タイトル
		public const string FIELD_INVOICEATODENE_BILLEDAMOUNT = "billedAmount";                     // 請求金額
		public const string FIELD_INVOICEATODENE_BILLEDFEETAX = "billedFeeTax";                     // 請求金額消費税
		public const string FIELD_INVOICEATODENE_BILLORDERDAYTITLE = "billOrderdayTitle";           // 注文日タイトル
		public const string FIELD_INVOICEATODENE_SHOPORDERDATE = "shopOrderDate";                   // 注文日
		public const string FIELD_INVOICEATODENE_BILLSENDDATETITLE = "billSendDateTitle";           // 請求書発行日タイトル
		public const string FIELD_INVOICEATODENE_BILLSENDDATE = "billSendDate";                     // 請求書発行日
		public const string FIELD_INVOICEATODENE_BILLDEADLINEDATETITLE = "billDeadlineDateTitle";   // お支払期限日タイトル
		public const string FIELD_INVOICEATODENE_BILLDEADLINEDATE = "billDeadlineDate";             // お支払期限日
		public const string FIELD_INVOICEATODENE_TRANSACTIONIDTITLE = "transactionIdTitle";         // お問合せ番号タイトル
		public const string FIELD_INVOICEATODENE_TRANSACTIONID = "transactionId";                   // お問合せ番号
		public const string FIELD_INVOICEATODENE_BILLBANKINFOMATION = "billBankInfomation";         // 銀行振込注意文言
		public const string FIELD_INVOICEATODENE_BANKNAMETITLE = "bankNameTitle";                   // 銀行名タイトル
		public const string FIELD_INVOICEATODENE_BANKNAME = "bankName";                             // 銀行名漢字
		public const string FIELD_INVOICEATODENE_BANKCODE = "bankCode";                             // 銀行コード
		public const string FIELD_INVOICEATODENE_BRANCHNAMETITLE = "branchNameTitle";               // 支店名タイトル
		public const string FIELD_INVOICEATODENE_BRANCHNAME = "branchName";                         // 支店名漢字
		public const string FIELD_INVOICEATODENE_BRANCHCODE = "branchCode";                         // 支店コード
		public const string FIELD_INVOICEATODENE_BANKACCOUNTNUMBERTITLE = "bankAccountNumberTitle"; // 口座番号タイトル
		public const string FIELD_INVOICEATODENE_BANKACCOUNTKIND = "bankAccountKind";               // 預金種別
		public const string FIELD_INVOICEATODENE_BANKACCOUNTNUMBER = "bankAccountNumber";           // 口座番号
		public const string FIELD_INVOICEATODENE_BANKACCOUNTNAMETITLE = "bankAccountNameTitle";     // 口座名義タイトル
		public const string FIELD_INVOICEATODENE_BANKACCOUNTNAME = "bankAccountName";               // 銀行口座名義
		public const string FIELD_INVOICEATODENE_RECEIPTBILLDEADLINEDATE = "receiptBillDeadlineDate";// 払込取扱用支払期限日
		public const string FIELD_INVOICEATODENE_RECEIPTNAME = "receiptName";                       // 払込取扱用購入者氏名
		public const string FIELD_INVOICEATODENE_INVOICEBARCODE = "invoiceBarcode";                 // バーコード情報
		public const string FIELD_INVOICEATODENE_RECEIPTCOMPANYTITLE = "receiptCompanyTitle";       // 収納代行会社名タイトル
		public const string FIELD_INVOICEATODENE_RECEIPTCOMPANY = "receiptCompany";                 // 収納代行会社名
		public const string FIELD_INVOICEATODENE_DOCKETBILLEDAMOUNT = "docketbilledAmount";         // 請求金額
		public const string FIELD_INVOICEATODENE_DOCKETCOMPANYNAME = "docketCompanyName";           // 受領証用購入者会社名
		public const string FIELD_INVOICEATODENE_DOCKETSECTIONNAME = "docketSectionName";           // 受領証用購入者部署名
		public const string FIELD_INVOICEATODENE_DOCKETNAME = "docketName";                         // 受領証用購入者氏名
		public const string FIELD_INVOICEATODENE_DOCKETTRANSACTIONIDTITLE = "docketTransactionIdTitle";// お問い合せ番号タイトル
		public const string FIELD_INVOICEATODENE_DOCKETTRANSACTIONID = "docketTransactionId";       // お問い合せ番号
		public const string FIELD_INVOICEATODENE_VOUCHERCOMPANYNAME = "voucherCompanyName";         // 払込受領書用購入者会社名
		public const string FIELD_INVOICEATODENE_VOUCHERSECTIONNAME = "voucherSectionName";         // 払込受領書用購入者部署名
		public const string FIELD_INVOICEATODENE_VOUCHERCUSTOMERFULLNAME = "voucherCustomerFullName";// 払込受領書用購入者氏名
		public const string FIELD_INVOICEATODENE_VOUCHERTRANSACTIONIDTITLE = "voucherTransactionIdTitle";// 払込受領書用お問い合せ番号タイトル
		public const string FIELD_INVOICEATODENE_VOUCHERTRANSACTIONID = "voucherTransactionId";     // 払込受領書用お問い合せ番号
		public const string FIELD_INVOICEATODENE_VOUCHERBILLEDAMOUNT = "voucherBilledAmount";       // 払込受領書用請求金額
		public const string FIELD_INVOICEATODENE_VOUCHERBILLEDFEETAX = "voucherBilledFeeTax";       // 払込受領書用消費税金額
		public const string FIELD_INVOICEATODENE_REVENUESTAMPREQUIRED = "revenueStampRequired";     // 収入印紙文言
		public const string FIELD_INVOICEATODENE_GOODSTITLE = "goodsTitle";                         // 明細内容タイトル
		public const string FIELD_INVOICEATODENE_GOODSAMOUNTTITLE = "goodsAmountTitle";             // 注文数タイトル
		public const string FIELD_INVOICEATODENE_GOODSPRICETITLE = "goodsPriceTitle";               // 単価タイトル
		public const string FIELD_INVOICEATODENE_GOODSSUBTOTALTITLE = "goodsSubtotalTitle";         // 金額タイトル
		public const string FIELD_INVOICEATODENE_DETAILINFOMATION = "detailInfomation";             // 明細注意事項
		public const string FIELD_INVOICEATODENE_EXPAND11 = "expand11";                             // ゆうちょ口座番号
		public const string FIELD_INVOICEATODENE_EXPAND12 = "expand12";                             // ゆうちょ加入者名
		public const string FIELD_INVOICEATODENE_EXPAND13 = "expand13";                             // OCR-Bフォント印字項目上段情報
		public const string FIELD_INVOICEATODENE_EXPAND14 = "expand14";                             // OCR-Bフォント印字項目下段情報
		public const string FIELD_INVOICEATODENE_EXPAND15 = "expand15";                             // 払込取扱用購入者住所
		public const string FIELD_INVOICEATODENE_EXPAND16 = "expand16";                             // 印字ズレチェックマーク
		public const string FIELD_INVOICEATODENE_EXPAND17 = "expand17";                             // 予備項目17
		public const string FIELD_INVOICEATODENE_EXPAND18 = "expand18";                             // 予備項目18
		public const string FIELD_INVOICEATODENE_EXPAND19 = "expand19";                             // 予備項目19
		public const string FIELD_INVOICEATODENE_EXPAND20 = "expand20";                             // 予備項目20
		public const string FIELD_INVOICEATODENE_DATE_CREATED = "date_created";                     // 作成日

		// Atodene後払い請求書明細
		public const string TABLE_INVOICEATODENEDETAIL = "w2_InvoiceAtodeneDetail";                 // Atodene後払い請求書明細
		public const string FIELD_INVOICEATODENEDETAIL_ORDER_ID = "order_id";                       // 注文ID
		public const string FIELD_INVOICEATODENEDETAIL_DETAIL_NO = "detail_no";                     // 明細番号
		public const string FIELD_INVOICEATODENEDETAIL_GOODS = "goods";                             // 明細内容
		public const string FIELD_INVOICEATODENEDETAIL_GOODSAMOUNT = "goodsAmount";                 // 注文数
		public const string FIELD_INVOICEATODENEDETAIL_GOODSPRICE = "goodsPrice";                   // 単価
		public const string FIELD_INVOICEATODENEDETAIL_GOODSSUBTOTAL = "goodsSubtotal";             // 金額
		public const string FIELD_INVOICEATODENEDETAIL_GOODSEXPAND = "goodsExpand";                 // 明細予備項目

		// 定期購入電子発票情報
		public const string TABLE_TWFIXEDPURCHASEINVOICE = "w2_TwFixedPurchaseInvoice";								// 定期購入電子発票情報
		public const string FIELD_TWFIXEDPURCHASEINVOICE_FIXED_PURCHASE_ID = "fixed_purchase_id";					// 定期購入ID
		public const string FIELD_TWFIXEDPURCHASEINVOICE_FIXED_PURCHASE_SHIPPING_NO = "fixed_purchase_shipping_no";	// 定期購入電子発票枝番
		public const string FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE = "tw_uniform_invoice";					// 電子発票種別
		public const string FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE_OPTION1 = "tw_uniform_invoice_option1";	// 電子発票項目1
		public const string FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE_OPTION2 = "tw_uniform_invoice_option2";	// 電子発票項目2
		public const string FIELD_TWFIXEDPURCHASEINVOICE_TW_CARRY_TYPE = "tw_carry_type";							// 載具種別
		public const string FIELD_TWFIXEDPURCHASEINVOICE_TW_CARRY_TYPE_OPTION = "tw_carry_type_option";				// 載具項目
		public const string FIELD_TWFIXEDPURCHASEINVOICE_DATE_CREATED = "date_created";								// 作成日
		public const string FIELD_TWFIXEDPURCHASEINVOICE_DATE_CHANGED = "date_changed";								// 更新日

		// 注文電子発票情報
		public const string TABLE_TWORDERINVOICE = "w2_TwOrderInvoice";										// 注文電子発票情報
		public const string FIELD_TWORDERINVOICE_ORDER_ID = "order_id";										// 注文ID
		public const string FIELD_TWORDERINVOICE_ORDER_SHIPPING_NO = "order_shipping_no";					// 電子発票枝番
		public const string FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE = "tw_uniform_invoice";					// 電子発票種別
		public const string FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION1 = "tw_uniform_invoice_option1";	// 電子発票項目1
		public const string FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION2 = "tw_uniform_invoice_option2";	// 電子発票項目2
		public const string FIELD_TWORDERINVOICE_TW_CARRY_TYPE = "tw_carry_type";							// 載具種別
		public const string FIELD_TWORDERINVOICE_TW_CARRY_TYPE_OPTION = "tw_carry_type_option";				// 載具項目
		public const string FIELD_TWORDERINVOICE_TW_INVOICE_NO = "tw_invoice_no";							// 発票番号
		public const string FIELD_TWORDERINVOICE_TW_INVOICE_DATE = "tw_invoice_date";						// 発票日時
		public const string FIELD_TWORDERINVOICE_TW_INVOICE_STATUS = "tw_invoice_status";					// 発票ステータス
		public const string FIELD_TWORDERINVOICE_DATE_CREATED = "date_created";								// 作成日
		public const string FIELD_TWORDERINVOICE_DATE_CHANGED = "date_changed";								// 更新日

		// 電子発票情報
		public const string TABLE_TWINVOICE = "w2_TwInvoice";									// 電子発票情報
		public const string FIELD_TWINVOICE_TW_INVOICE_ID = "tw_invoice_id";					// 電子発票管理Id
		public const string FIELD_TWINVOICE_TW_INVOICE_DATE_START = "tw_invoice_date_start";	// 発番開始日
		public const string FIELD_TWINVOICE_TW_INVOICE_DATE_END = "tw_invoice_date_end";		// 発番終了日
		public const string FIELD_TWINVOICE_TW_INVOICE_CODE = "tw_invoice_code";				// 発票種別コード
		public const string FIELD_TWINVOICE_TW_INVOICE_TYPE_NAME = "tw_invoice_type_name";		// 発票種別名
		public const string FIELD_TWINVOICE_TW_INVOICE_CODE_NAME = "tw_invoice_code_name";		// 発票種別コード名
		public const string FIELD_TWINVOICE_TW_INVOICE_NO_START = "tw_invoice_no_start";		// 発票開始番号
		public const string FIELD_TWINVOICE_TW_INVOICE_NO_END = "tw_invoice_no_end";			// 発票終了番号
		public const string FIELD_TWINVOICE_TW_INVOICE_NO = "tw_invoice_no";					// 最終発票番号
		public const string FIELD_TWINVOICE_TW_INVOICE_ALERT_COUNT = "tw_invoice_alert_count";	// アラート値
		public const string FIELD_TWINVOICE_DATE_CREATED = "date_created";						// 作成日
		public const string FIELD_TWINVOICE_DATE_CHANGED = "date_changed";						// 更新日

		// ユーザ電子発票管理情報
		public const string TABLE_TWUSERINVOICE = "w2_TwUserInvoice";										// ユーザ電子発票管理情報
		public const string FIELD_TWUSERINVOICE_USER_ID = "user_id";										// ユーザID
		public const string FIELD_TWUSERINVOICE_TW_INVOICE_NO = "tw_invoice_no";							// 電子発票管理枝番
		public const string FIELD_TWUSERINVOICE_TW_INVOICE_NAME = "tw_invoice_name";						// 電子発票情報名
		public const string FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE = "tw_uniform_invoice";					// 電子発票種別
		public const string FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE_OPTION1 = "tw_uniform_invoice_option1";	// 電子発票項目1
		public const string FIELD_TWUSERINVOICE_TW_UNIFORM_INVOICE_OPTION2 = "tw_uniform_invoice_option2";	// 電子発票項目2
		public const string FIELD_TWUSERINVOICE_TW_CARRY_TYPE = "tw_carry_type";							// 載具種別
		public const string FIELD_TWUSERINVOICE_TW_CARRY_TYPE_OPTION = "tw_carry_type_option";				// 載具項目
		public const string FIELD_TWUSERINVOICE_DATE_CREATED = "date_created";								// 作成日
		public const string FIELD_TWUSERINVOICE_DATE_CHANGED = "date_changed";								// 更新日

		// ユーザ電子発票管理情報
		public const string TABLE_TWWORKUSERINVOICE = "w2_TwWorkUserInvoice";									// ユーザ電子発票管理情報
		public const string FIELD_TWWORKUSERINVOICE_USER_ID = "user_id";										// ユーザID
		public const string FIELD_TWWORKUSERINVOICE_TW_INVOICE_NO = "tw_invoice_no";							// 電子発票管理枝番
		public const string FIELD_TWWORKUSERINVOICE_TW_INVOICE_NAME = "tw_invoice_name";						// 電子発票情報名
		public const string FIELD_TWWORKUSERINVOICE_TW_UNIFORM_INVOICE = "tw_uniform_invoice";					// 電子発票種別
		public const string FIELD_TWWORKUSERINVOICE_TW_UNIFORM_INVOICE_OPTION1 = "tw_uniform_invoice_option1";	// 電子発票項目1
		public const string FIELD_TWWORKUSERINVOICE_TW_UNIFORM_INVOICE_OPTION2 = "tw_uniform_invoice_option2";	// 電子発票項目2
		public const string FIELD_TWWORKUSERINVOICE_TW_CARRY_TYPE = "tw_carry_type";							// 載具種別
		public const string FIELD_TWWORKUSERINVOICE_TW_CARRY_TYPE_OPTION = "tw_carry_type_option";				// 載具項目
		public const string FIELD_TWWORKUSERINVOICE_DATE_CREATED = "date_created";								// 作成日
		public const string FIELD_TWWORKUSERINVOICE_DATE_CHANGED = "date_changed";								// 更新日

		// 頒布会コース
		public const string TABLE_SUBSCRIPTIONBOX = "w2_SubscriptionBox";                           // 頒布会コース
		public const string FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID = "subscription_box_course_id";// 頒布会コースID
		public const string FIELD_SUBSCRIPTIONBOX_MANAGEMENT_NAME = "management_name";              // 頒布会管理名
		public const string FIELD_SUBSCRIPTIONBOX_DISPLAY_NAME = "display_name";                    // 頒布会表示名
		public const string FIELD_SUBSCRIPTIONBOX_AUTO_RENEWAL = "auto_renewal";                    // 自動繰り返し設定
		public const string FIELD_SUBSCRIPTIONBOX_ITEMS_CHANGEABLE_BY_USER = "items_changeable_by_user";// フロント商品変更可否
		public const string FIELD_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE = "order_item_determination_type";// 商品決定方法
		public const string FIELD_SUBSCRIPTIONBOX_MINIMUM_PURCHASE_QUANTITY = "minimum_purchase_quantity";// 最低購入数量
		public const string FIELD_SUBSCRIPTIONBOX_MAXIMUM_PURCHASE_QUANTITY = "maximum_purchase_quantity";// 最大購入数量
		public const string FIELD_SUBSCRIPTIONBOX_MINIMUM_NUMBER_OF_PRODUCTS = "minimum_number_of_products";// 最低購入商品種類数
		public const string FIELD_SUBSCRIPTIONBOX_MAXIMUM_NUMBER_OF_PRODUCTS = "maximum_number_of_products";// 最大購入商品種類数
		public const string FIELD_SUBSCRIPTIONBOX_VALID_FLG = "valid_flg";                          // 有効フラグ
		public const string FIELD_SUBSCRIPTIONBOX_DATE_CREATED = "date_created";                    // 作成日
		public const string FIELD_SUBSCRIPTIONBOX_DATE_CHANGED = "date_changed";                    // 更新日
		public const string FIELD_SUBSCRIPTIONBOX_LAST_CHANGED = "last_changed";                    // 最終更新者
		public const string FIELD_SUBSCRIPTIONBOX_FIXED_AMOUNT_FLG = "fixed_amount_flg";            // 定額フラグ
		public const string FIELD_SUBSCRIPTIONBOX_FIXED_AMOUNT = "fixed_amount";                    // 定額価格
		public const string FIELD_SUBSCRIPTIONBOX_TAX_CATEGORY_ID = "tax_category_id";              // 	税率カテゴリID
		public const string FIELD_SUBSCRIPTIONBOX_DISPLAY_PRIORITY = "display_priority";            // 頒布会表示順
		public const string FIELD_SUBSCRIPTIONBOX_MINIMUM_AMOUNT = "minimum_amount";            // 商品合計金額下限（税込）
		public const string FIELD_SUBSCRIPTIONBOX_MAXIMUM_AMOUNT = "maximum_amount";            // 商品合計金額上限（税込）
		/// <summary>頒布会初回選択画面フラグ</summary>
		public const string FIELD_SUBSCRIPTIONBOX_FIRST_SELECTABLE_FLG = "first_selectable_flg";            // 商品合計金額上限（税込）
		/// <summary>無期限設定フラグ</summary>
		public const string FIELD_SUBSCRIPTIONBOX_INDEFINITE_PERIOD_FLG = "indefinite_period_flg";

		// 頒布会選択可能商品
		public const string TABLE_SUBSCRIPTIONBOXITEM = "w2_SubscriptionBoxItem";                   // 頒布会選択可能商品
		public const string FIELD_SUBSCRIPTIONBOXITEM_SUBSCRIPTION_BOX_COURSE_ID = "subscription_box_course_id";// 頒布会コースID
		public const string FIELD_SUBSCRIPTIONBOXITEM_BRANCH_NO = "branch_no";                      // 枝番
		public const string FIELD_SUBSCRIPTIONBOXITEM_SHOP_ID = "shop_id";                          // 店舗ID
		public const string FIELD_SUBSCRIPTIONBOXITEM_PRODUCT_ID = "product_id";                    // 商品ID
		public const string FIELD_SUBSCRIPTIONBOXITEM_VARIATION_ID = "variation_id";                // バリエーションID
		public const string FIELD_SUBSCRIPTIONBOXITEM_SELECTABLE_SINCE = "selectable_since";        // 選択可能期間開始
		public const string FIELD_SUBSCRIPTIONBOXITEM_SELECTABLE_UNTIL = "selectable_until";        // 選択可能期間終了
		public const string FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_SINCE = "campaign_since";            // 選択可能期間終了
		public const string FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_UNTIL = "campaign_until";            // 選択可能期間終了
		public const string FIELD_SUBSCRIPTIONBOXITEM_CAMPAIGN_PRICE = "campaign_price";            // 選択可能期間終了

		// 頒布会デフォルト注文商品
		public const string TABLE_SUBSCRIPTIONBOXDEFAULTITEM = "w2_SubscriptionBoxDefaultItem";     // 頒布会デフォルト注文商品
		public const string FIELD_SUBSCRIPTIONBOXDEFAULTITEM_SUBSCRIPTION_BOX_COURSE_ID = "subscription_box_course_id";// 頒布会コースID
		public const string FIELD_SUBSCRIPTIONBOXDEFAULTITEM_BRANCH_NO = "branch_no";               // 枝番
		public const string FIELD_SUBSCRIPTIONBOXDEFAULTITEM_COUNT = "count";                       // 回数
		public const string FIELD_SUBSCRIPTIONBOXDEFAULTITEM_TERM_SINCE = "term_since";             // 期間開始
		public const string FIELD_SUBSCRIPTIONBOXDEFAULTITEM_TERM_UNTIL = "term_until";             // 期間終了
		public const string FIELD_SUBSCRIPTIONBOXDEFAULTITEM_SHOP_ID = "shop_id";                   // 店舗ID
		public const string FIELD_SUBSCRIPTIONBOXDEFAULTITEM_PRODUCT_ID = "product_id";             // 商品ID
		public const string FIELD_SUBSCRIPTIONBOXDEFAULTITEM_VARIATION_ID = "variation_id";         // バリエーションID
		public const string FIELD_SUBSCRIPTIONBOXDEFAULTITEM_ITEM_QUANTITY = "item_quantity";       // 数量
		public const string FIELD_SUBSCRIPTIONBOXDEFAULTITEM_NECESSARY_PRODUCT_FLG = "necessary_product_flg";// 必須商品フラグ

		/// <summary>Sale Goal</summary>
		public const string TABLE_SALEGOAL = "w2_SaleGoal";
		/// <summary>Sale Goal: Year</summary>
		public const string FIELD_SALEGOAL_YEAR = "year";
		/// <summary>Sale Goal: Annual Goal</summary>
		public const string FIELD_SALEGOAL_ANNUAL_GOAL = "annual_goal";
		/// <summary>Sale Goal: Applicable Month</summary>
		public const string FIELD_SALEGOAL_APPLICABLE_MONTH = "applicable_month";
		/// <summary>Sale Goal: Monthly Goal 1</summary>
		public const string FIELD_SALEGOAL_MONTHLY_GOAL_1 = "monthly_goal_1";
		/// <summary>Sale Goal: Monthly Goal 2</summary>
		public const string FIELD_SALEGOAL_MONTHLY_GOAL_2 = "monthly_goal_2";
		/// <summary>Sale Goal: Monthly Goal 3</summary>
		public const string FIELD_SALEGOAL_MONTHLY_GOAL_3 = "monthly_goal_3";
		/// <summary>Sale Goal: Monthly Goal 4</summary>
		public const string FIELD_SALEGOAL_MONTHLY_GOAL_4 = "monthly_goal_4";
		/// <summary>Sale Goal: Monthly Goal 5</summary>
		public const string FIELD_SALEGOAL_MONTHLY_GOAL_5 = "monthly_goal_5";
		/// <summary>Sale Goal: Monthly Goal 6</summary>
		public const string FIELD_SALEGOAL_MONTHLY_GOAL_6 = "monthly_goal_6";
		/// <summary>Sale Goal: Monthly Goal 7</summary>
		public const string FIELD_SALEGOAL_MONTHLY_GOAL_7 = "monthly_goal_7";
		/// <summary>Sale Goal: Monthly Goal 8</summary>
		public const string FIELD_SALEGOAL_MONTHLY_GOAL_8 = "monthly_goal_8";
		/// <summary>Sale Goal: Monthly Goal 9</summary>
		public const string FIELD_SALEGOAL_MONTHLY_GOAL_9 = "monthly_goal_9";
		/// <summary>Sale Goal: Monthly Goal 10</summary>
		public const string FIELD_SALEGOAL_MONTHLY_GOAL_10 = "monthly_goal_10";
		/// <summary>Sale Goal: Monthly Goal 11</summary>
		public const string FIELD_SALEGOAL_MONTHLY_GOAL_11 = "monthly_goal_11";
		/// <summary>Sale Goal: Monthly Goal 12</summary>
		public const string FIELD_SALEGOAL_MONTHLY_GOAL_12 = "monthly_goal_12";
		/// <summary>Sale Goal: Date Created</summary>
		public const string FIELD_SALEGOAL_DATE_CREATED = "date_created";
		/// <summary>Sale Goal: Date Changed</summary>
		public const string FIELD_SALEGOAL_DATE_CHANGED = "date_changed";
		/// <summary>Sale Goal: Last Changed</summary>
		public const string FIELD_SALEGOAL_LAST_CHANGED = "last_changed";

		/// <summary>Summary report</summary>
		public const string TABLE_SUMMARYREPORT = "w2_SummaryReport";
		/// <summary>Summary report: Period kbn</summary>
		public const string FIELD_SUMMARYREPORT_PERIOD_KBN = "period_kbn";
		/// <summary>Summary report: Data kbn</summary>
		public const string FIELD_SUMMARYREPORT_DATA_KBN = "data_kbn";
		/// <summary>Summary report: Report date</summary>
		public const string FIELD_SUMMARYREPORT_REPORT_DATE = "report_date";
		/// <summary>Summary report: Data</summary>
		public const string FIELD_SUMMARYREPORT_DATA = "data";
		/// <summary>Summary report: Date created</summary>
		public const string FIELD_SUMMARYREPORT_DATE_CREATED = "date_created";

		/// <summary>Default setting</summary>
		public const string TABLE_DEFAULTSETTING = "w2_DefaultSetting";
		/// <summary>Shop id</summary>
		public const string FIELD_DEFAULTSETTING_SHOP_ID = "shop_id";
		/// <summary>Classification</summary>
		public const string FIELD_DEFAULTSETTING_CLASSIFICATION = "classification";
		/// <summary>Init data</summary>
		public const string FIELD_DEFAULTSETTING_INIT_DATA = "init_data";
		/// <summary>Date created</summary>
		public const string FIELD_DEFAULTSETTING_DATE_CREATED = "date_created";
		/// <summary>Date changed</summary>
		public const string FIELD_DEFAULTSETTING_DATE_CHANGED = "date_changed";
		/// <summary>Last created</summary>
		public const string FIELD_DEFAULTSETTING_LAST_CHANGED = "last_changed";
		public const string FIELD_DEFAULTSETTING_LAST_CREATED = "last_created";

		// DSK後払い請求書
		public const string TABLE_INVOICEDSKDEFERRED = "w2_InvoiceDskDeferred";                     // DSK後払い請求書
		public const string FIELD_INVOICEDSKDEFERRED_ORDER_ID = "order_id";                         // 注文ID
		public const string FIELD_INVOICEDSKDEFERRED_INVOICE_BAR_CODE = "invoice_bar_code";         // 請求書バーコード
		public const string FIELD_INVOICEDSKDEFERRED_INVOICE_CODE = "invoice_code";                 // 請求書コード
		public const string FIELD_INVOICEDSKDEFERRED_INVOICE_KBN = "invoice_kbn";                   // 発行区分
		public const string FIELD_INVOICEDSKDEFERRED_HISTORY_SEQ = "history_seq";                   // 履歴番号
		public const string FIELD_INVOICEDSKDEFERRED_REMINDED_KBN = "reminded_kbn";                 // 督促区分
		public const string FIELD_INVOICEDSKDEFERRED_COMPANY_NAME = "company_name";                 // 会社名
		public const string FIELD_INVOICEDSKDEFERRED_DEPARTMENT = "department";                     // 部署名
		public const string FIELD_INVOICEDSKDEFERRED_CUSTOMER_NAME = "customer_name";               // 購入者氏名
		public const string FIELD_INVOICEDSKDEFERRED_CUSTOMER_ZIP = "customer_zip";                 // 購入者郵便番号
		public const string FIELD_INVOICEDSKDEFERRED_CUSTOMER_ADDRESS1 = "customer_address1";       // 購入者住所都道府県
		public const string FIELD_INVOICEDSKDEFERRED_CUSTOMER_ADDRESS2 = "customer_address2";       // 購入者住所市区町村
		public const string FIELD_INVOICEDSKDEFERRED_CUSTOMER_ADDRESS3 = "customer_address3";       // 購入者住所それ以降の住所
		public const string FIELD_INVOICEDSKDEFERRED_SHOP_ZIP = "shop_zip";                         // 加盟店郵便番号
		public const string FIELD_INVOICEDSKDEFERRED_SHOP_ADDRESS1 = "shop_address1";               // 加盟店住所都道府県
		public const string FIELD_INVOICEDSKDEFERRED_SHOP_ADDRESS2 = "shop_address2";               // 購入者住所市区町村
		public const string FIELD_INVOICEDSKDEFERRED_SHOP_ADDRESS3 = "shop_address3";               // 加盟店住所それ以降の住所
		public const string FIELD_INVOICEDSKDEFERRED_SHOP_TEL = "shop_tel";                         // 加盟店電話
		public const string FIELD_INVOICEDSKDEFERRED_SHOP_FAX = "shop_fax";                         // 加盟店FAX番号
		public const string FIELD_INVOICEDSKDEFERRED_BILLED_AMOUNT = "billed_amount";               // 顧客請求金額
		public const string FIELD_INVOICEDSKDEFERRED_TAX = "tax";                                   // 消費税
		public const string FIELD_INVOICEDSKDEFERRED_TIME_OF_RECEIPTS = "time_of_receipts";         // 購入者払込期限日
		public const string FIELD_INVOICEDSKDEFERRED_INVOICE_START_DATE = "invoice_start_date";     // 請求書発行日付
		public const string FIELD_INVOICEDSKDEFERRED_INVOICE_TITLE = "invoice_title";               // 帳票タイトル
		public const string FIELD_INVOICEDSKDEFERRED_MESSAGE1 = "message1";                         // 通信欄1
		public const string FIELD_INVOICEDSKDEFERRED_MESSAGE2 = "message2";                         // 通信欄2
		public const string FIELD_INVOICEDSKDEFERRED_MESSAGE3 = "message3";                         // 通信欄3
		public const string FIELD_INVOICEDSKDEFERRED_MESSAGE4 = "message4";                         // 通信欄4
		public const string FIELD_INVOICEDSKDEFERRED_INVOICE_SHOPSITE_NAME = "invoice_shopsite_name";// 加盟店サイト名称
		public const string FIELD_INVOICEDSKDEFERRED_SHOP_EMAIL = "shop_email";                     // 加盟店メールアドレス
		public const string FIELD_INVOICEDSKDEFERRED_NAME = "name";                                 // 固定文言1
		public const string FIELD_INVOICEDSKDEFERRED_QA_URL = "qa_url";                             // 固定文言2
		public const string FIELD_INVOICEDSKDEFERRED_SHOP_ORDER_DATE = "shop_order_date";           // 加盟店注文日
		public const string FIELD_INVOICEDSKDEFERRED_SHOP_CODE = "shop_code";                       // 加盟店ID
		public const string FIELD_INVOICEDSKDEFERRED_TRANSACTION_ID = "transaction_id";             // 注文ID
		public const string FIELD_INVOICEDSKDEFERRED_SHOP_TRANSACTION_ID1 = "shop_transaction_id1"; // 加盟店注文ID1
		public const string FIELD_INVOICEDSKDEFERRED_SHOP_TRANSACTION_ID2 = "shop_transaction_id2"; // 加盟店注文ID2
		public const string FIELD_INVOICEDSKDEFERRED_SHOP_MESSAGE1 = "shop_message1";               // 加盟店通信欄1
		public const string FIELD_INVOICEDSKDEFERRED_SHOP_MESSAGE2 = "shop_message2";               // 加盟店通信欄2
		public const string FIELD_INVOICEDSKDEFERRED_SHOP_MESSAGE3 = "shop_message3";               // 加盟店通信欄3
		public const string FIELD_INVOICEDSKDEFERRED_SHOP_MESSAGE4 = "shop_message4";               // 加盟店通信欄4
		public const string FIELD_INVOICEDSKDEFERRED_SHOP_MESSAGE5 = "shop_message5";               // 加盟店通信欄5
		public const string FIELD_INVOICEDSKDEFERRED_YOBI1 = "yobi1";                               // 請求書形式
		public const string FIELD_INVOICEDSKDEFERRED_YOBI2 = "yobi2";                               // 郵便口座番号
		public const string FIELD_INVOICEDSKDEFERRED_YOBI3 = "yobi3";                               // 郵便口座名義
		public const string FIELD_INVOICEDSKDEFERRED_YOBI4 = "yobi4";                               // 郵便OCR-Bフォント上段
		public const string FIELD_INVOICEDSKDEFERRED_YOBI5 = "yobi5";                               // 郵便OCR-Bフォント下段
		public const string FIELD_INVOICEDSKDEFERRED_YOBI6 = "yobi6";                               // 払込取扱用購入者住所
		public const string FIELD_INVOICEDSKDEFERRED_YOBI7 = "yobi7";                               // X印
		public const string FIELD_INVOICEDSKDEFERRED_DATE_CREATED = "date_created";                 // 作成日

		// DSK後払い後払い請求書明細
		public const string TABLE_INVOICEDSKDEFERREDDETAIL = "w2_InvoiceDskDeferredDetail";         // DSK後払い後払い請求書明細
		public const string FIELD_INVOICEDSKDEFERREDDETAIL_ORDER_ID = "order_id";                   // 注文ID
		public const string FIELD_INVOICEDSKDEFERREDDETAIL_DETAIL_NO = "detail_no";                 // 明細番号
		public const string FIELD_INVOICEDSKDEFERREDDETAIL_GOODS_NAME = "goods_name";               // 明細名
		public const string FIELD_INVOICEDSKDEFERREDDETAIL_GOODS_PRICE = "goods_price";             // 単価
		public const string FIELD_INVOICEDSKDEFERREDDETAIL_GOODS_NUM = "goods_num";                 // 数量
		public const string FIELD_INVOICEDSKDEFERREDDETAIL_DATE_CREATED = "date_created";           // 作成日

		// スコア後払い請求書
		public const string TABLE_INVOICE_SCORE = "w2_InvoiceScore";                     // スコア後払い請求書
		public const string FIELD_INVOICE_SCORE_ORDER_ID = "order_id";                         // 注文ID
		public const string FIELD_INVOICE_SCORE_INVOICE_BAR_CODE = "invoice_bar_code";         // 請求書バーコード
		public const string FIELD_INVOICE_SCORE_INVOICE_CODE = "invoice_code";                 // 請求書コード
		public const string FIELD_INVOICE_SCORE_INVOICE_KBN = "invoice_kbn";                   // 発行区分
		public const string FIELD_INVOICE_SCORE_HISTORY_SEQ = "history_seq";                   // 履歴番号
		public const string FIELD_INVOICE_SCORE_REMINDED_KBN = "reminded_kbn";                 // 督促区分
		public const string FIELD_INVOICE_SCORE_COMPANY_NAME = "company_name";                 // 会社名
		public const string FIELD_INVOICE_SCORE_DEPARTMENT = "department";                     // 部署名
		public const string FIELD_INVOICE_SCORE_CUSTOMER_NAME = "customer_name";               // 購入者氏名
		public const string FIELD_INVOICE_SCORE_CUSTOMER_ZIP = "customer_zip";                 // 購入者郵便番号
		public const string FIELD_INVOICE_SCORE_CUSTOMER_ADDRESS1 = "customer_address1";       // 購入者住所都道府県
		public const string FIELD_INVOICE_SCORE_CUSTOMER_ADDRESS2 = "customer_address2";       // 購入者住所市区町村
		public const string FIELD_INVOICE_SCORE_CUSTOMER_ADDRESS3 = "customer_address3";       // 購入者住所それ以降の住所
		public const string FIELD_INVOICE_SCORE_SHOP_ZIP = "shop_zip";                         // 加盟店郵便番号
		public const string FIELD_INVOICE_SCORE_SHOP_ADDRESS1 = "shop_address1";               // 加盟店住所都道府県
		public const string FIELD_INVOICE_SCORE_SHOP_ADDRESS2 = "shop_address2";               // 購入者住所市区町村
		public const string FIELD_INVOICE_SCORE_SHOP_ADDRESS3 = "shop_address3";               // 加盟店住所それ以降の住所
		public const string FIELD_INVOICE_SCORE_SHOP_TEL = "shop_tel";                         // 加盟店電話
		public const string FIELD_INVOICE_SCORE_SHOP_FAX = "shop_fax";                         // 加盟店FAX番号
		public const string FIELD_INVOICE_SCORE_BILLED_AMOUNT = "billed_amount";               // 顧客請求金額
		public const string FIELD_INVOICE_SCORE_TAX = "tax";                                   // 消費税
		public const string FIELD_INVOICE_SCORE_TIME_OF_RECEIPTS = "time_of_receipts";         // 購入者払込期限日
		public const string FIELD_INVOICE_SCORE_INVOICE_START_DATE = "invoice_start_date";     // 請求書発行日付
		public const string FIELD_INVOICE_SCORE_INVOICE_TITLE = "invoice_title";               // 帳票タイトル
		public const string FIELD_INVOICE_SCORE_MESSAGE1 = "nissen_message1";                         // 通信欄1
		public const string FIELD_INVOICE_SCORE_MESSAGE2 = "nissen_message2";                         // 通信欄2
		public const string FIELD_INVOICE_SCORE_MESSAGE3 = "nissen_message3";                         // 通信欄3
		public const string FIELD_INVOICE_SCORE_MESSAGE4 = "nissen_message4";                         // 通信欄4
		public const string FIELD_INVOICE_SCORE_INVOICE_SHOPSITE_NAME = "invoice_shopsite_name";// 加盟店サイト名称
		public const string FIELD_INVOICE_SCORE_SHOP_EMAIL = "shop_email";                     // 加盟店メールアドレス
		public const string FIELD_INVOICE_SCORE_NAME = "nissen_name";                                 // 固定文言1
		public const string FIELD_INVOICE_SCORE_QA_URL = "nissen_qa_url";                             // 固定文言2
		public const string FIELD_INVOICE_SCORE_SHOP_ORDER_DATE = "shop_order_date";           // 加盟店注文日
		public const string FIELD_INVOICE_SCORE_SHOP_CODE = "shop_code";                       // 加盟店ID
		public const string FIELD_INVOICE_SCORE_TRANSACTION_ID = "nissen_transaction_id";             // 注文ID
		public const string FIELD_INVOICE_SCORE_SHOP_TRANSACTION_ID1 = "shop_transaction_id1"; // 加盟店注文ID1
		public const string FIELD_INVOICE_SCORE_SHOP_TRANSACTION_ID2 = "shop_transaction_id2"; // 加盟店注文ID2
		public const string FIELD_INVOICE_SCORE_SHOP_MESSAGE1 = "shop_message1";               // 加盟店通信欄1
		public const string FIELD_INVOICE_SCORE_SHOP_MESSAGE2 = "shop_message2";               // 加盟店通信欄2
		public const string FIELD_INVOICE_SCORE_SHOP_MESSAGE3 = "shop_message3";               // 加盟店通信欄3
		public const string FIELD_INVOICE_SCORE_SHOP_MESSAGE4 = "shop_message4";               // 加盟店通信欄4
		public const string FIELD_INVOICE_SCORE_SHOP_MESSAGE5 = "shop_message5";               // 加盟店通信欄5
		public const string FIELD_INVOICE_SCORE_INVOICE_FORM = "invoice_form";                        // 請求書形式
		public const string FIELD_INVOICE_SCORE_POSTAL_ACCOUNT_NUMBER = "postal_account_number";               // 郵便口座番号
		public const string FIELD_INVOICE_SCORE_POSTAL_ACCOUNT_HOLDER_NAME = "postal_account_holder_name";          // 郵便口座名義
		public const string FIELD_INVOICE_SCORE_POSTAL_FONT_TOP_ROW = "postal_font_top_row";                 // 郵便OCR-Bフォント上段
		public const string FIELD_INVOICE_SCORE_POSTAL_FONT_BOTTOM_ROW = "postal_font_bottom_row";              // 郵便OCR-Bフォント下段
		public const string FIELD_INVOICE_SCORE_REMITTANCE_ADDRESS = "remittance_address";                 // 払込取扱用購入者住所
		public const string FIELD_INVOICE_SCORE_X_SYMBOL = "x_symbol";                            //X印
		public const string FIELD_INVOICE_SCORE_RESERVE1 = "reserve1";                            //予備項目1
		public const string FIELD_INVOICE_SCORE_RESERVE2 = "reserve2";                            //予備項目2
		public const string FIELD_INVOICE_SCORE_RESERVE3 = "reserve3";                            //予備項目3
		public const string FIELD_INVOICE_SCORE_DATE_CREATED = "date_created";                    // 作成日

		// スコア後払い後払い請求書明細
		public const string TABLE_INVOICE_SCORE_DETAIL = "w2_InvoiceScoreDetail";         // スコア後払い後払い請求書明細
		public const string FIELD_INVOICE_SCORE_DETAIL_ORDER_ID = "order_id";                   // 注文ID
		public const string FIELD_INVOICE_SCORE_DETAIL_DETAIL_NO = "detail_no";                 // 明細番号
		public const string FIELD_INVOICE_SCORE_DETAIL_GOODS_NAME = "goods_name";               // 明細名
		public const string FIELD_INVOICE_SCORE_DETAIL_GOODS_PRICE = "goods_price";             // 単価
		public const string FIELD_INVOICE_SCORE_DETAIL_GOODS_NUM = "goods_num";                 // 数量
		public const string FIELD_INVOICE_SCORE_DETAIL_DATE_CREATED = "date_created";           // 作成日

		/// <summary>Global zipcode table</summary>
		public const string TABLE_GLOBALZIPCODE = "w2_GlobalZipcode";
		/// <summary>Country ISO code</summary>
		public const string FIELD_GLOBALZIPCODE_COUNTRY_ISO_CODE = "country_iso_code";
		/// <summary>Zipcode</summary>
		public const string FIELD_GLOBALZIPCODE_ZIPCODE = "zipcode";
		/// <summary>Country</summary>
		public const string FIELD_GLOBALZIPCODE_COUNTRY = "country";
		/// <summary>Province</summary>
		public const string FIELD_GLOBALZIPCODE_PROVINCE = "province";
		/// <summary>City</summary>
		public const string FIELD_GLOBALZIPCODE_CITY = "city";
		/// <summary>Address</summary>
		public const string FIELD_GLOBALZIPCODE_ADDRESS = "address";

		// ABテスト
		public const string TABLE_ABTEST = "w2_AbTest";                                             // ABテスト
		public const string FIELD_ABTEST_AB_TEST_ID = "ab_test_id";                                 // ABテストID
		public const string FIELD_ABTEST_AB_TEST_TITLE = "ab_test_title";                           // ABテストタイトル
		public const string FIELD_ABTEST_PUBLIC_STATUS = "public_status";                           // 公開状態
		public const string FIELD_ABTEST_PUBLIC_START_DATETIME = "public_start_datetime";           // 公開開始日時
		public const string FIELD_ABTEST_PUBLIC_END_DATETIME = "public_end_datetime";               // 公開終了日時
		public const string FIELD_ABTEST_DATE_CREATED = "date_created";                             // 作成日
		public const string FIELD_ABTEST_DATE_CHANGED = "date_changed";                             // 更新日
		public const string FIELD_ABTEST_LAST_CHANGED = "last_changed";                             // 最終更新者

		// ABテストアイテム
		public const string TABLE_ABTESTITEM = "w2_AbTestItem";                                     // ABテストアイテム
		public const string FIELD_ABTESTITEM_AB_TEST_ID = "ab_test_id";                             // ABテストID
		public const string FIELD_ABTESTITEM_ITEM_NO = "item_no";                                   // アイテムNO
		public const string FIELD_ABTESTITEM_PAGE_ID = "page_id";                                   // ランディングページID
		public const string FIELD_ABTESTITEM_DISTRIBUTION_RATE = "distribution_rate";               // 振り分け比率

		/// <summary>User credit card work table</summary>
		public const string TABLE_WORKUSERCREDITCARD = "w2_WorkUserCreditCard";
		/// <summary>Fixed purchase work table</summary>
		public const string TABLE_WORKFIXEDPURCHASE = "w2_WorkFixedPurchase";
		/// <summary>Fixed purchase item work table</summary>
		public const string TABLE_WORKFIXEDPURCHASEITEM = "w2_WorkFixedPurchaseItem";
		/// <summary>Fixed purchase shipping work table</summary>
		public const string TABLE_WORKFIXEDPURCHASESHIPPING = "w2_WorkFixedPurchaseShipping";
		/// <summary>Fixed purchase history work table</summary>
		public const string TABLE_WORKFIXEDPURCHASEHISTORY = "w2_WorkFixedPurchaseHistory";

		/// <summary>後払い.com請求書テーブル</summary>
		public const string TABLE_INVOICEATOBARAICOM = "w2_InvoiceAtobaraicom";
		/// <summary>注文ID</summary>
		public const string FIELD_INVOICEATOBARAICOM_ORDER_ID = "order_id";
		/// <summary>請求金額</summary>
		public const string FIELD_INVOICEATOBARAICOM_USE_AMOUNT = "use_amount";
		/// <summary>うち消費税額</summary>
		public const string FIELD_INVOICEATOBARAICOM_TAX_AMOUNT = "tax_amount";
		/// <summary>支払期限日</summary>
		public const string FIELD_INVOICEATOBARAICOM_LIMIT_DATE = "limit_date";
		/// <summary>顧客氏名</summary>
		public const string FIELD_INVOICEATOBARAICOM_NAME_KJ = "name_kj";
		/// <summary>バーコードデータ</summary>
		public const string FIELD_INVOICEATOBARAICOM_CV_BARCODE_DATA = "cv_barcode_data";
		/// <summary>バーコード文字列1</summary>
		public const string FIELD_INVOICEATOBARAICOM_CV_BARCODE_STRING1 = "cv_barcode_string1";
		/// <summary>バーコード文字列2</summary>
		public const string FIELD_INVOICEATOBARAICOM_CV_BARCODE_STRING2 = "cv_barcode_string2";
		/// <summary>ゆうちょ口座 - MT用OCRコード1</summary>
		public const string FIELD_INVOICEATOBARAICOM_YU_MT_OCR_CODE1 = "yu_mt_ocr_code1";
		/// <summary>ゆうちょ口座 - MT用OCRコード2</summary>
		public const string FIELD_INVOICEATOBARAICOM_YU_MT_OCR_CODE2 = "yu_mt_ocr_code2";
		/// <summary>ゆうちょ口座 - 加入者名</summary>
		public const string FIELD_INVOICEATOBARAICOM_YU_ACCOUNT_NAME = "yu_account_name";
		/// <summary>ゆうちょ口座 - 口座番号</summary>
		public const string FIELD_INVOICEATOBARAICOM_YU_ACCOUNT_NO = "yu_account_no";
		/// <summary>ゆうちょ口座 - 払込負担区分</summary>
		public const string FIELD_INVOICEATOBARAICOM_YU_LOAD_KIND = "yu_load_kind";
		/// <summary>CVS収納代行会社名</summary>
		public const string FIELD_INVOICEATOBARAICOM_CVS_COMPANY_NAME = "cvs_company_name";
		/// <summary>CVS収納代行加入者名</summary>
		public const string FIELD_INVOICEATOBARAICOM_CVS_USER_NAME = "cvs_user_name";
		/// <summary>銀行口座 - 銀行コード</summary>
		public const string FIELD_INVOICEATOBARAICOM_BK_CODE = "bk_code";
		/// <summary>銀行口座 - 支店コード</summary>
		public const string FIELD_INVOICEATOBARAICOM_BK_BRANCH_CODE = "bk_branch_code";
		/// <summary>銀行口座 - 銀行名</summary>
		public const string FIELD_INVOICEATOBARAICOM_BK_NAME = "bk_name";
		/// <summary>銀行口座 - 支店名</summary>
		public const string FIELD_INVOICEATOBARAICOM_BK_BRANCH_NAME = "bk_branch_name";
		/// <summary>銀行口座 - 口座種別</summary>
		public const string FIELD_INVOICEATOBARAICOM_BK_ACCOUNT_KIND = "bk_account_kind";
		/// <summary>銀行口座 - 口座番号</summary>
		public const string FIELD_INVOICEATOBARAICOM_BK_ACCOUNT_NO = "bk_account_no";
		/// <summary>銀行口座 - 口座名義</summary>
		public const string FIELD_INVOICEATOBARAICOM_BK_ACCOUNT_NAME = "bk_account_name";
		/// <summary>銀行口座 - 口座名義カナ</summary>
		public const string FIELD_INVOICEATOBARAICOM_BK_ACCOUNT_KANA = "bk_account_kana";
		/// <summary>マイページパスワード</summary>
		public const string FIELD_INVOICEATOBARAICOM_MYPAGE_PWD = "mypage_pwd";
		/// <summary>マイページURL</summary>
		public const string FIELD_INVOICEATOBARAICOM_MYPAGE_URL = "mypage_url";
		/// <summary>クレジット利用期限日</summary>
		public const string FIELD_INVOICEATOBARAICOM_CREDIT_DEADLINE = "credit_deadline";
		/// <summary>作成日</summary>
		public const string FIELD_INVOICEATOBARAICOM_DATE_CREATED = "date_created";

		/// <summary>ベリトランス請求書テーブル</summary>
		public const string TABLE_INVOICEVERITRANS = "w2_InvoiceVeritrans";
		/// <summary>注文ID</summary>
		public const string FIELD_INVOICEVERITRANS_ORDER_ID = "order_id";
		/// <summary>決済サービスタイプ</summary>
		public const string FIELD_INVOICEVERITRANS_SERVICE_TYPE = "service_type";
		/// <summary>処理結果コード</summary>
		public const string FIELD_INVOICEVERITRANS_M_STATUS = "m_status";
		/// <summary>詳細結果コード</summary>
		public const string FIELD_INVOICEVERITRANS_V_RESULT_CODE = "v_result_code";
		/// <summary>エラーメッセージ</summary>
		public const string FIELD_INVOICEVERITRANS_M_ERR_MSG = "m_err_msg";
		/// <summary>電文 ID</summary>
		public const string FIELD_INVOICEVERITRANS_M_ARCH_TXN = "m_arch_txn";
		/// <summary>取引 ID</summary>
		public const string FIELD_INVOICEVERITRANS_PAYMENT_ORDER_ID = "payment_order_id";
		/// <summary>取引毎に付く ID</summary>
		public const string FIELD_INVOICEVERITRANS_CUST_TXN = "cust_txn";
		/// <summary>MDK バージョン</summary>
		public const string FIELD_INVOICEVERITRANS_TXN_VERSION = "txn_version";
		/// <summary>請求書バーコード</summary>
		public const string FIELD_INVOICEVERITRANS_INVOICE_BAR_CODE = "invoice_bar_code";
		/// <summary>請求書コード</summary>
		public const string FIELD_INVOICEVERITRANS_INVOICE_CODE = "invoice_code";
		/// <summary>発行区分</summary>
		public const string FIELD_INVOICEVERITRANS_INVOICE_KBN = "invoice_kbn";
		/// <summary>履歴番号</summary>
		public const string FIELD_INVOICEVERITRANS_HISTORY_SEQ = "history_seq";
		/// <summary>督促区分</summary>
		public const string FIELD_INVOICEVERITRANS_REMINDED_KBN= "reminded_kbn";
		/// <summary>会社名</summary>
		public const string FIELD_INVOICEVERITRANS_COMPANY_NAME = "company_name";
		/// <summary>部署名</summary>
		public const string FIELD_INVOICEVERITRANS_DEPARTMENT = "department";
		/// <summary>購入者氏名</summary>
		public const string FIELD_INVOICEVERITRANS_CUSTOMER_NAME = "customer_name";
		/// <summary>購入者郵便番号</summary>
		public const string FIELD_INVOICEVERITRANS_CUSTOMER_ZIP = "customer_zip";
		/// <summary>購入者住所：都道府県</summary>
		public const string FIELD_INVOICEVERITRANS_CUSTOMER_ADDRESS1 = "customer_address1";
		/// <summary>購入者住所：市区町村</summary>
		public const string FIELD_INVOICEVERITRANS_CUSTOMER_ADDRESS2 = "customer_address2";
		/// <summary>購入者住所：それ以降の住所</summary>
		public const string FIELD_INVOICEVERITRANS_CUSTOMER_ADDRESS3 = "customer_address3";
		/// <summary>加盟店郵便番号</summary>
		public const string FIELD_INVOICEVERITRANS_SHOP_ZIP = "shop_zip";
		/// <summary>加盟店住所：都道府県</summary>
		public const string FIELD_INVOICEVERITRANS_SHOP_ADDRESS1 = "shop_address1";
		/// <summary>加盟店住所：市区町村</summary>
		public const string FIELD_INVOICEVERITRANS_SHOP_ADDRESS2 = "shop_address2";
		/// <summary>加盟店住所：それ以降の住所</summary>
		public const string FIELD_INVOICEVERITRANS_SHOP_ADDRESS3 = "shop_address3";
		/// <summary>加盟店電話</summary>
		public const string FIELD_INVOICEVERITRANS_SHOP_TEL = "shop_tel";
		/// <summary>加盟店FAX番号</summary>
		public const string FIELD_INVOICEVERITRANS_SHOP_FAX = "shop_fax";
		/// <summary>顧客請求金額</summary>
		public const string FIELD_INVOICEVERITRANS_BILLED_AMOUNT = "billed_amount";
		/// <summary>消費税</summary>
		public const string FIELD_INVOICEVERITRANS_TAX = "tax";
		/// <summary>購入者払込期限日</summary>
		public const string FIELD_INVOICEVERITRANS_TIME_OF_RECEIPTS = "time_of_receipts";
		/// <summary>請求書発行日付</summary>
		public const string FIELD_INVOICEVERITRANS_INVOICE_START_DATE = "invoice_start_date";
		/// <summary>帳票タイトル</summary>
		public const string FIELD_INVOICEVERITRANS_INVOICE_TITLE = "invoice_title";
		/// <summary>スコア通信欄1</summary>
		public const string FIELD_INVOICEVERITRANS_NISSEN_MESSAGE1 = "nissen_message1";
		/// <summary>スコア通信欄2</summary>
		public const string FIELD_INVOICEVERITRANS_NISSEN_MESSAGE2 = "nissen_message2";
		/// <summary>スコア通信欄3</summary>
		public const string FIELD_INVOICEVERITRANS_NISSEN_MESSAGE3 = "nissen_message3";
		/// <summary>スコア通信欄4</summary>
		public const string FIELD_INVOICEVERITRANS_NISSEN_MESSAGE4 = "nissen_message4";
		/// <summary>加盟店サイト名称</summary>
		public const string FIELD_INVOICEVERITRANS_INVOICE_SHOPSITE_NAME = "invoice_shopsite_name";
		/// <summary>加盟店メールアドレス</summary>
		public const string FIELD_INVOICEVERITRANS_SHOP_EMAIL = "shop_email";
		/// <summary>スコア社名</summary>
		public const string FIELD_INVOICEVERITRANS_NISSEN_NAME = "nissen_name";
		/// <summary>スコア連絡先URL</summary>
		public const string FIELD_INVOICEVERITRANS_NISSEN_QA_URL = "nissen_qa_url";
		/// <summary>加盟店注文日</summary>
		public const string FIELD_INVOICEVERITRANS_SHOP_ORDER_DATE = "shop_order_date";
		/// <summary>加盟店コード</summary>
		public const string FIELD_INVOICEVERITRANS_SHOP_CODE = "shop_code";
		/// <summary>スコア注文ID</summary>
		public const string FIELD_INVOICEVERITRANS_NISSEN_TRANSACTION_ID = "nissen_transaction_id";
		/// <summary>加盟店注文ID1</summary>
		public const string FIELD_INVOICEVERITRANS_SHOP_TRANSACTION_ID1 = "shop_transaction_id1";
		/// <summary>加盟店注文ID2</summary>
		public const string FIELD_INVOICEVERITRANS_SHOP_TRANSACTION_ID2 = "shop_transaction_id2";
		/// <summary>加盟店通信欄1</summary>
		public const string FIELD_INVOICEVERITRANS_SHOP_MESSAGE1 = "shop_message1";
		/// <summary>加盟店通信欄2</summary>
		public const string FIELD_INVOICEVERITRANS_SHOP_MESSAGE2 = "shop_message2";
		/// <summary>加盟店通信欄3</summary>
		public const string FIELD_INVOICEVERITRANS_SHOP_MESSAGE3 = "shop_message3";
		/// <summary>加盟店通信欄4</summary>
		public const string FIELD_INVOICEVERITRANS_SHOP_MESSAGE4 = "shop_message4";
		/// <summary>加盟店通信欄5</summary>
		public const string FIELD_INVOICEVERITRANS_SHOP_MESSAGE5 = "shop_message5";
		/// <summary>請求書形式</summary>
		public const string FIELD_INVOICEVERITRANS_INVOICE_FORM = "invoice_form";
		/// <summary>郵便口座番号</summary>
		public const string FIELD_INVOICEVERITRANS_POSTAL_ACCOUNT_NUMBER = "postal_account_number";
		/// <summary>郵便口座名義</summary>
		public const string FIELD_INVOICEVERITRANS_POSTAL_ACCOUNT_HOLDER_NAME = "postal_account_holder_name";
		/// <summary>郵便 OCR-B フォント：上段</summary>
		public const string FIELD_INVOICEVERITRANS_POSTAL_FONT_TOP_ROW = "postal_font_top_row";
		/// <summary>郵便 OCR-B フォント：下段</summary>
		public const string FIELD_INVOICEVERITRANS_POSTAL_FONT_BOTTOM_ROW = "postal_font_bottom_row";
		/// <summary>払込取扱用 購入者住所</summary>
		public const string FIELD_INVOICEVERITRANS_REMITTANCE_ADDRESS = "remittance_address";
		/// <summary>X 印 </summary>
		public const string FIELD_INVOICEVERITRANS_X_SYMBOL = "x_symbol";
		/// <summary>予備項目1</summary>
		public const string FIELD_INVOICEVERITRANS_RESERVE1 = "reserve1";
		/// <summary>予備項目2</summary>
		public const string FIELD_INVOICEVERITRANS_RESERVE2 = "reserve2";
		/// <summary>予備項目3</summary>
		public const string FIELD_INVOICEVERITRANS_RESERVE3 = "reserve3";
		/// <summary>作成日</summary>
		public const string FIELD_INVOICEVERITRANS_DATE_CREATED = "date_created";
		/// <summary>更新日</summary>
		public const string FIELD_INVOICEVERITRANS_DATE_CHANGED = "date_changed";
		/// <summary>最終更新者</summary>
		public const string FIELD_INVOICEVERITRANS_LAST_CHANGED = "last_changed";

		/// <summary>ベリトランス請求書詳細テーブル</summary>
		public const string TABLE_INVOICEVERITRANSDETAIL = "w2_InvoiceVeritransDetail";
		/// <summary>注文ID</summary>
		public const string FIELD_INVOICEVERITRANSDETAIL_ORDER_ID = "order_id";
		/// <summary>明細番号</summary>
		public const string FIELD_INVOICEVERITRANSDETAIL_DETAIL_NO = "detail_no";
		/// <summary>明細名</summary>
		public const string FIELD_INVOICEVERITRANSDETAIL_GOODS_NAME = "goods_name";
		/// <summary>単価</summary>
		public const string FIELD_INVOICEVERITRANSDETAIL_GOODS_PRICE = "goods_price";
		/// <summary>数量</summary>
		public const string FIELD_INVOICEVERITRANSDETAIL_GOODS_NUM = "goods_num";
		/// <summary>作成日</summary>
		public const string FIELD_INVOICEVERITRANSDETAIL_DATE_CREATED = "date_created";

		/// <summary>Product field extend: Product price</summary>
		public const string FIELD_PRODUCT_PRODUCTPRICE_EXTEND = "EX_ProductPrice";
		/// <summary>Product field extend: Product variation</summary>
		public const string FIELD_PRODUCT_PRODUCTVARIATION_EXTEND = "EX_ProductVariation";
		/// <summary>Product field extend: Product tag</summary>
		public const string FIELD_PRODUCT_PRODUCTTAG_EXTEND = "EX_ProductTag";
		/// <summary>Product field extend: Product extend</summary>
		public const string FIELD_PRODUCT_PRODUCTEXTEND_EXTEND = "EX_ProductExtend";
		/// <summary>Product field extend: Mall exhibits config</summary>
		public const string FIELD_PRODUCT_MALLEXHIBITSCONFIG_EXTEND = "EX_MallExhibitsConfig";
		/// <summary>Product field extend: Product stock</summary>
		public const string FIELD_PRODUCT_PRODUCTSTOCK_EXTEND = "EX_ProductStock";
		/// <summary>Product field extend: Product fixed purchase discount setting</summary>
		public const string FIELD_PRODUCT_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_EXTEND = "EX_ProductFixedPurchaseDiscountSetting";

		/// <summary>Product variation field extend: Product price</summary>
		public const string FIELD_PRODUCTVARIATION_PRODUCTPRICE_EXTEND = "EX_ProductVariationPrice";

		/// <summary>Scoring sale</summary>
		public const string TABLE_SCORINGSALE = "w2_ScoringSale";
		/// <summary>スコアリング販売ID</summary>
		public const string FIELD_SCORINGSALE_SCORING_SALE_ID = "scoring_sale_id";
		/// <summary>スコアリング販売タイトル</summary>
		public const string FIELD_SCORINGSALE_SCORING_SALE_TITLE = "scoring_sale_title";
		/// <summary>公開状態</summary>
		public const string FIELD_SCORINGSALE_PUBLISH_STATUS = "publish_status";
		/// <summary>公開開始日時</summary>
		public const string FIELD_SCORINGSALE_PUBLIC_START_DATETIME = "public_start_datetime";
		/// <summary>公開終了日時</summary>
		public const string FIELD_SCORINGSALE_PUBLIC_END_DATETIME = "public_end_datetime";
		/// <summary>スコア軸ID</summary>
		public const string FIELD_SCORINGSALE_SCORE_AXIS_ID = "score_axis_id";
		/// <summary>テーマカラー</summary>
		public const string FIELD_SCORINGSALE_THEME_COLOR = "theme_color";
		/// <summary>トップページ利用</summary>
		public const string FIELD_SCORINGSALE_TOP_PAGE_USE_FLG = "top_page_use_flg";
		/// <summary>トップページタイトル</summary>
		public const string FIELD_SCORINGSALE_TOP_PAGE_TITLE = "top_page_title";
		/// <summary>トップページサブタイトル</summary>
		public const string FIELD_SCORINGSALE_TOP_PAGE_SUB_TITLE = "top_page_sub_title";
		/// <summary>トップページ本文</summary>
		public const string FIELD_SCORINGSALE_TOP_PAGE_BODY = "top_page_body";
		/// <summary>トップページ画像</summary>
		public const string FIELD_SCORINGSALE_TOP_PAGE_IMG_PATH = "top_page_img_path";
		/// <summary>トップページボタン文言</summary>
		public const string FIELD_SCORINGSALE_TOP_PAGE_BTN_CAPTION = "top_page_btn_caption";
		/// <summary>結果ページタイトル</summary>
		public const string FIELD_SCORINGSALE_RESULT_PAGE_TITLE = "result_page_title";
		/// <summary>結果ページ本文HTML(上)</summary>
		public const string FIELD_SCORINGSALE_RESULT_PAGE_BODY_ABOVE = "result_page_body_above";
		/// <summary>結果ページ本文HTML(下)</summary>
		public const string FIELD_SCORINGSALE_RESULT_PAGE_BODY_BELOW = "result_page_body_below";
		/// <summary>レーダーチャート利用</summary>
		public const string FIELD_SCORINGSALE_RADAR_CHART_USE_FLG = "radar_chart_use_flg";
		/// <summary>レーダーチャートタイトル</summary>
		public const string FIELD_SCORINGSALE_RADAR_CHART_TITLE = "radar_chart_title";
		/// <summary>結果ページボタン文言</summary>
		public const string FIELD_SCORINGSALE_RESULT_PAGE_BTN_CAPTION = "result_page_btn_caption";
		/// <summary>作成日</summary>
		public const string FIELD_SCORINGSALE_DATE_CREATED = "date_created";
		/// <summary>更新日</summary>
		public const string FIELD_SCORINGSALE_DATE_CHANGED = "date_changed";
		/// <summary>最終更新者</summary>
		public const string FIELD_SCORINGSALE_LAST_CHANGED = "last_changed";

		/// <summary>Scoring sale question page</summary>
		public const string TABLE_SCORINGSALEQUESTIONPAGE = " w2_ScoringSaleQuestionPage";
		/// <summary>スコアリング販売ID</summary>
		public const string FIELD_SCORINGSALEQUESTIONPAGE_SCORING_SALE_ID = "scoring_sale_id";
		/// <summary>ページNo</summary>
		public const string FIELD_SCORINGSALEQUESTIONPAGE_PAGE_NO = "page_no";
		/// <summary>前ページボタン文言</summary>
		public const string FIELD_SCORINGSALEQUESTIONPAGE_PREVIOUS_PAGE_BTN_CAPTION = "previous_page_btn_caption";
		/// <summary>次ページボタン文言</summary>
		public const string FIELD_SCORINGSALEQUESTIONPAGE_NEXT_PAGE_BTN_CAPTION = "next_page_btn_caption";
		/// <summary>作成日</summary>
		public const string FIELD_SCORINGSALEQUESTIONPAGE_DATE_CREATED = "date_created";
		/// <summary>更新日</summary>
		public const string FIELD_SCORINGSALEQUESTIONPAGE_DATE_CHANGED = "date_changed";
		/// <summary>最終更新者</summary>
		public const string FIELD_SCORINGSALEQUESTIONPAGE_LAST_CHANGED = "last_changed";

		/// <summary>Scoring sale question page item</summary>
		public const string TABLE_SCORINGSALEQUESTIONPAGEITEM = "w2_ScoringSaleQuestionPageItem";
		/// <summary>スコアリング販売ID</summary>
		public const string FIELD_SCORINGSALEQUESTIONPAGEITEM_SCORING_SALE_ID = "scoring_sale_id";
		/// <summary>質問ID</summary>
		public const string FIELD_SCORINGSALEQUESTIONPAGEITEM_QUESTION_ID = "question_id";
		/// <summary>ページNo</summary>
		public const string FIELD_SCORINGSALEQUESTIONPAGEITEM_PAGE_NO = "page_no";
		/// <summary>枝番</summary>
		public const string FIELD_SCORINGSALEQUESTIONPAGEITEM_BRANCH_NO = "branch_no";
		/// <summary>Question</summary>
		public const string FIELD_SCORINGSALEQUESTIONPAGEITEM_QUESTION = "EX_Question";

		/// <summary>Scoring sale axis setting</summary>
		public const string TABLE_SCORINGSALE_AXIS_SETTING = "w2_ScoringSaleAxisSetting";
		/// <summary>スコア軸ID</summary>
		public const string FIELD_SCOREAXIS_SCORE_AXIS_ID = "score_axis_id";
		/// <summary>スコア軸タイトル</summary>
		public const string FIELD_SCOREAXIS_SCORE_AXIS_TITLE = "score_axis_title";
		/// <summary>軸名１</summary>
		public const string FIELD_SCOREAXIS_AXIS_NAME1 = "axis_name1";
		/// <summary>軸名２</summary>
		public const string FIELD_SCOREAXIS_AXIS_NAME2 = "axis_name2";
		/// <summary>軸名３</summary>
		public const string FIELD_SCOREAXIS_AXIS_NAME3 = "axis_name3";
		/// <summary>軸名４</summary>
		public const string FIELD_SCOREAXIS_AXIS_NAME4 = "axis_name4";
		/// <summary>軸名５</summary>
		public const string FIELD_SCOREAXIS_AXIS_NAME5 = "axis_name5";
		/// <summary>軸名６</summary>
		public const string FIELD_SCOREAXIS_AXIS_NAME6 = "axis_name6";
		/// <summary>軸名７</summary>
		public const string FIELD_SCOREAXIS_AXIS_NAME7 = "axis_name7";
		/// <summary>軸名８</summary>
		public const string FIELD_SCOREAXIS_AXIS_NAME8 = "axis_name8";
		/// <summary>軸名９</summary>
		public const string FIELD_SCOREAXIS_AXIS_NAME9 = "axis_name9";
		/// <summary>軸名１０</summary>
		public const string FIELD_SCOREAXIS_AXIS_NAME10 = "axis_name10";
		/// <summary>軸名１１</summary>
		public const string FIELD_SCOREAXIS_AXIS_NAME11 = "axis_name11";
		/// <summary>軸名１２</summary>
		public const string FIELD_SCOREAXIS_AXIS_NAME12 = "axis_name12";
		/// <summary>軸名１３</summary>
		public const string FIELD_SCOREAXIS_AXIS_NAME13 = "axis_name13";
		/// <summary>軸名１４</summary>
		public const string FIELD_SCOREAXIS_AXIS_NAME14 = "axis_name14";
		/// <summary>軸名１５</summary>
		public const string FIELD_SCOREAXIS_AXIS_NAME15 = "axis_name15";
		/// <summary>作成日</summary>
		public const string FIELD_SCOREAXIS_DATE_CREATED = "date_created";
		/// <summary>更新日</summary>
		public const string FIELD_SCOREAXIS_DATE_CHANGED = "date_changed";
		/// <summary>最終更新者</summary>
		public const string FIELD_SCOREAXIS_LAST_CHANGED = "last_changed";

		/// <summary>Scoring sale question</summary>
		public const string TABLE_SCORINGSALEQUESTION = "w2_ScoringSaleQuestion";
		/// <summary>質問ID</summary>
		public const string FIELD_SCORINGSALEQUESTION_QUESTION_ID = "question_id";
		/// <summary>質問タイトル</summary>
		public const string FIELD_SCORINGSALEQUESTION_QUESTION_TITLE = "question_title";
		/// <summary>スコア軸ID</summary>
		public const string FIELD_SCORINGSALEQUESTION_SCORE_AXIS_ID = "score_axis_id";
		/// <summary>回答タイプ</summary>
		public const string FIELD_SCORINGSALEQUESTION_ANSWER_TYPE = "answer_type";
		/// <summary>質問文</summary>
		public const string FIELD_SCORINGSALEQUESTION_QUESTION_STATEMENT = "question_statement";
		/// <summary>作成日</summary>
		public const string FIELD_SCORINGSALEQUESTION_DATE_CREATED = "date_created";
		/// <summary>更新日</summary>
		public const string FIELD_SCORINGSALEQUESTION_DATE_CHANGED = "date_changed";
		/// <summary>最終更新者</summary>
		public const string FIELD_SCORINGSALEQUESTION_LAST_CHANGED = "last_changed";
		/// <summary>Question choice</summary>
		public const string FIELD_SCORINGSALEQUESTION_QUESTION_CHOICE = "EX_QuestionChoice";

		/// <summary>Scoring sale question choice</summary>
		public const string TABLE_SCORINGSALEQUESTIONCHOICE = "w2_ScoringSaleQuestionChoice";
		/// <summary>質問ID</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_ID = "question_id";
		/// <summary>枝番</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_BRANCH_NO = "branch_no";
		/// <summary>選択肢文</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_CHOICE_STATEMENT = "question_choice_statement";
		/// <summary>選択肢画像</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_CHOICE_STATEMENT_IMG_PATH = "question_choice_statement_img_path";
		/// <summary>軸加算値１</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL1 = "axis_additional1";
		/// <summary>軸加算値２</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL2 = "axis_additional2";
		/// <summary>軸加算値３</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL3 = "axis_additional3";
		/// <summary>軸加算値４</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL4 = "axis_additional4";
		/// <summary>軸加算値５</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL5 = "axis_additional5";
		/// <summary>軸加算値６</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL6 = "axis_additional6";
		/// <summary>軸加算値７</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL7 = "axis_additional7";
		/// <summary>軸加算値８</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL8 = "axis_additional8";
		/// <summary>軸加算値９</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL9 = "axis_additional9";
		/// <summary>軸加算値１０</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL10 = "axis_additional10";
		/// <summary>軸加算値１１</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL11 = "axis_additional11";
		/// <summary>軸加算値１２</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL12 = "axis_additional12";
		/// <summary>軸加算値１３</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL13 = "axis_additional13";
		/// <summary>軸加算値１４</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL14 = "axis_additional14";
		/// <summary>軸加算値１５</summary>
		public const string FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL15 = "axis_additional15";

		/// <summary>Scoring sale product</summary>
		public const string TABLE_SCORINGSALEPRODUCT = "w2_ScoringSaleProduct";
		/// <summary>スコアリング販売ID</summary>
		public const string FIELD_SCORINGSALEPRODUCT_SCORING_SALE_ID = "scoring_sale_id";
		/// <summary>枝番</summary>
		public const string FIELD_SCORINGSALEPRODUCT_BRANCH_NO = "branch_no";
		/// <summary>店舗ID</summary>
		public const string FIELD_SCORINGSALEPRODUCT_SHOP_ID = "shop_id";
		/// <summary>商品ID</summary>
		public const string FIELD_SCORINGSALEPRODUCT_PRODUCT_ID = "product_id";
		/// <summary>商品バリエーションID</summary>
		public const string FIELD_SCORINGSALEPRODUCT_VARIATION_ID = "variation_id";
		/// <summary>個数</summary>
		public const string FIELD_SCORINGSALEPRODUCT_QUANTITY = "quantity";

		/// <summary>Scoring sale result condition</summary>
		public const string TABLE_SCORINGSALERESULTCONDITION = "w2_ScoringSaleResultCondition";
		/// <summary>スコアリング販売ID</summary>
		public const string FIELD_SCORINGSALERESULTCONDITION_SCORING_SALE_ID = "scoring_sale_id";
		/// <summary>枝番</summary>
		public const string FIELD_SCORINGSALERESULTCONDITION_BRANCH_NO = "branch_no";
		/// <summary>条件枝番</summary>
		public const string FIELD_SCORINGSALERESULTCONDITION_CONDITION_BRANCH_NO = "condition_branch_no";
		/// <summary>スコア軸番号</summary>
		public const string FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_NO = "score_axis_axis_no";
		/// <summary>スコア軸値(From)</summary>
		public const string FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_VALUE_FROM = "score_axis_axis_value_from";
		/// <summary>スコア軸値(To)</summary>
		public const string FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_VALUE_TO = "score_axis_axis_value_to";
		/// <summary>条件</summary>
		public const string FIELD_SCORINGSALERESULTCONDITION_CONDITION = "condition";
		/// <summary>グループ番号</summary>
		public const string FIELD_SCORINGSALERESULTCONDITION_GROUP_NO = "group_no";

		/// <summary>Scoring Sale Product Name</summary>
		public const string FIELD_SCORINGSALE_PRODUCT_NAME = "product_name";
		/// <summary>Scoring Sale Product Price</summary>
		public const string FIELD_SCORINGSALE_PRODUCT_PRICE = "price";
		/// <summary>Score axis name</summary>
		public const string FIELD_SCORINGSALEQUESTION_SCORE_AXIS_NAME = "axis_name";
		/// <summary>Score axis</summary>
		public const string FIELD_SCORE_AXIS = "score_axis";

		// LINE仮ユーザー
		/// <summary>LINEユーザーID</summary>
		public const string FIELD_LINETEMPORARYUSER_LINE_USER_ID = "line_user_id";
		/// <summary>仮採番ユーザーID</summary>
		public const string FIELD_LINETEMPORARYUSER_TEMPORARY_USER_ID = "temporary_user_id";
		/// <summary>仮会員登録日時</summary>
		public const string FIELD_LINETEMPORARYUSER_TEMPORARY_USER_REGISTRATION_DATE = "temporary_user_registration_date";
		/// <summary>本会員登録フラグ</summary>
		public const string FIELD_LINETEMPORARYUSER_REGULAR_USER_REGISTRATION_FLAG = "regular_user_registration_flag";
		/// <summary>本会員登録日時</summary>
		public const string FIELD_LINETEMPORARYUSER_REGULAR_USER_REGISTRATION_DATE = "regular_user_registration_date";
		/// <summary>作成日</summary>
		public const string FIELD_LINETEMPORARYUSER_DATE_CREATED = "date_created";
		/// <summary>更新日</summary>
		public const string FIELD_LINETEMPORARYUSER_DATE_CHANGED = "date_changed";
		/// <summary>最終更新者</summary>
		public const string FIELD_LINETEMPORARYUSER_LAST_CHANGED = "last_changed";

		// 商品カラー記事ブロックテーブル
		public const string TABLE_OPERATORAUTHORITY = "w2_OperatorAuthority";
		/// <summary>店舗ID</summary>
		public const string FIELD_OPERATORAUTHORITY_SHOP_ID = "shop_id";
		/// <summary>オペレータID</summary>
		public const string FIELD_OPERATORAUTHORITY_OPERATOR_ID = "operator_id";
		/// <summary>権限設定</summary>
		public const string FIELD_OPERATORAUTHORITY_CONDITION_TYPE = "condition_type";
		/// <summary>許可区分</summary>
		public const string FIELD_OPERATORAUTHORITY_PERMISSION = "permission";
		/// <summary>設定値</summary>
		public const string FIELD_OPERATORAUTHORITY_CONDITION_VALUE = "condition_value";

		/// <summary>Shipping delivery postage minimum shipping fee</summary>
		public const string FIELD_SHIPPINGDELIVERYPOSTAGE_MINIIUM_SHIPPING_FEE = "minimum_shipping_fee";

		/// <summary>出荷集計テーブル</summary>
		public const string TABLE_SHIPMENTQUANTITY = "w2_DailyOrderShipmentForecast";
		/// <summary>出荷日</summary>
		public const string FIELD_SHIPMENTQUANTITY_SHIPMENT_DATE = "shipment_date";
		/// <summary>出荷数</summary>
		public const string FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_COUNT = "shipment_order_count";
		/// <summary>出荷商品金額合計</summary>
		public const string FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_PRICE_SUBTOTAL = "total_order_price_subtotal";
		/// <summary>作成日</summary>
		public const string FIELD_SHIPMENTQUANTITY_DATE_CREATED = "date_created";
		/// <summary>更新日</summary>
		public const string FIELD_SHIPMENTQUANTITY_DATE_CHANGED = "date_changed";
		/// <summary>最終更新者</summary>
		public const string FIELD_SHIPMENTQUANTITY_LAST_CHANGED = "last_changed";
		/// <summary>出荷注文検索期間最小値</summary>
		public const string MIN_DATE_SHIPPING_ORDER_SEARCH = "min_time";
		/// <summary>出荷注文検索期間最大値</summary>
		public const string MAX_DATE_SHIPPING_ORDER_SEARCH = "max_time";
		#endregion

		#region フィールドフラグ定数

		// ユーザ：注文元サイト（モールID）
		public const string FLG_USER_MALL_ID_OWN_SITE = "OWN_SITE";	// 自社サイト
		public const string FLG_USER_MALL_ID_URERU_AD = "URERU_AD"; // 売れるネット広告つくーる
		/// <summary>外部連携注文サイト</summary>
		public static string[] FLG_USER_MALL_ID_EXTERNAL_ORDER_SITES = new[]
		{
			FLG_USER_MALL_ID_URERU_AD,
		};

		// 初期配送パターン定数
		public static string FIXED_PURCHASE_FISRT_PATTERN_STRINGS = "FixedPurchaseFirstPatternStrings"; // カート別配送パターン

		// ユーザ：顧客区分
		public const string FLG_USER_USER_KBN_ALL_USER = "USER";			// 全ての会員(会員として集計したい場合は定義値に"_USER"をつけること)
		public const string FLG_USER_USER_KBN_ALL_GUEST = "GEST";			// 全てのゲスト(ゲストとして集計したい場合は定義値に"_GEST"をつけること)
		public const string FLG_USER_USER_KBN_PC_USER = "PC_USER";			// PC会員
		public const string FLG_USER_USER_KBN_PC_GUEST = "PC_GEST";			// PCゲスト
		public const string FLG_USER_USER_KBN_MOBILE_USER = "MB_USER";		// モバイル会員
		public const string FLG_USER_USER_KBN_MOBILE_GUEST = "MB_GEST";		// モバイルゲスト
		public const string FLG_USER_USER_KBN_SMARTPHONE_USER = "SP_USER";	// スマフォ会員
		public const string FLG_USER_USER_KBN_SMARTPHONE_GUEST = "SP_GEST";	// スマフォゲスト
		public const string FLG_USER_USER_KBN_OFFLINE_USER = "OFF_USER";	// オフライン会員
		public const string FLG_USER_USER_KBN_OFFLINE_GUEST = "OFF_GEST";	// オフラインゲスト
		public const string FLG_USER_USER_KBN_MAILMAGAZINE = "MAIL";		// メルマガ会員
		public const string FLG_USER_USER_KBN_CS = "CS";					// CS

		// ユーザ：性別
		public const string FLG_USER_SEX_MALE = "MALE";		// 男
		public const string FLG_USER_SEX_FEMALE = "FEMALE";	// 女
		public const string FLG_USER_SEX_UNKNOWN = "UNKNOWN";		// 不明

		/// <summary>CROSS POINT 性別：女性</summary>
		public const string CROSS_POINT_FLG_USER_SEX_FEMALE = "1";
		/// <summary>CROSS POINT 性別：男性</summary>
		public const string CROSS_POINT_FLG_USER_SEX_MALE = "2";
		/// <summary>CROSS POINT 性別：不明</summary>
		public const string CROSS_POINT_FLG_USER_SEX_UNKNOWN = "3";
		/// <summary>CROSS POINT（Batch用） 性別：女性</summary>
		public const string CROSS_POINT_BATCH_USER_SEX_FEMALE = "女性";
		/// <summary>CROSS POINT（Batch用） 性別：男性</summary>
		public const string CROSS_POINT_BATCH_USER_SEX_MALE = "男性";

		// ユーザ：メール配信
		public const string FLG_USER_MAILFLG_OK = "ON";		// 配信する
		public const string FLG_USER_MAILFLG_NG = "OFF";	// 配信しない
		public const string FLG_USER_MAILFLG_UNKNOWN = "UNKNOWN";	// 不明

		// ユーザ：削除フラグ
		public const string FLG_USER_DELFLG_UNDELETED = "0";	// 通常
		public const string FLG_USER_DELFLG_DELETED = "1";	// 削除済み

		// ユーザ：定期会員フラグ
		public const string FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF = "0";	// 通常会員
		public const string FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON = "1";	// 定期会員

		// 選択したユーザー管理レベルを除いて検索するか、しないか
		public const string FLG_USER_USER_MANAGEMENT_LEVEL_EXCLUDE_FLG_OFF = "0"; // 無効
		public const string FLG_USER_USER_MANAGEMENT_LEVEL_EXCLUDE_FLG_ON = "1"; // 有効

		// Amazonログイン連携済みユーザーのみを検索するか、しないか
		public const string FLG_USER_AMAZON_COOPERATED_FLG_OFF = "0"; // 無効
		public const string FLG_USER_AMAZON_COOPERATED_FLG_ON = "1"; // 有効

		// ユーザー管理レベルID
		/// <summary>通常</summary>
		public const string FLG_USER_USER_MANAGEMENT_LEVEL_NORMAL = "normal";

		// ユーザー：かんたん会員フラグ
		public const string FLG_USER_EASY_REGISTER_FLG_NORMAL = "0"; // 通常会員
		public const string FLG_USER_EASY_REGISTER_FLG_EASY = "1"; // かんたん会員

		// ユーザー：ユーザー統合フラグ
		public const string FLG_USER_INTEGRATED_FLG_NONE = "0";	// 通常
		public const string FLG_USER_INTEGRATED_FLG_DONE = "1";	// 統合済み（統合されたユーザ）

		// ユーザー統合：ステータス
		public const string FLG_USERINTEGRATION_STATUS_NONE = "";	// 統合未確定
		public const string FLG_USERINTEGRATION_STATUS_SUSPEND = "SUSPEND";	// 統合保留
		public const string FLG_USERINTEGRATION_STATUS_DONE = "DONE";	// 統合確定
		public const string FLG_USERINTEGRATION_STATUS_EXCLUDED = "EXCLUDED";	// 除外

		// ユーザー統合ユーザ情報：代表フラグ
		public const string FLG_USERINTEGRATIONUSER_REPRESENTATIVE_FLG_OFF = "0";    // 代表ではない
		public const string FLG_USERINTEGRATIONUSER_REPRESENTATIVE_FLG_ON = "1";    // 代表である

		// ユーザ配送先情報：配送先枝番
		public const string FLG_USERSHIPPING_OWNER_SHIPPING_NO = "0";	// 注文者情報の住所

		// かんたん会員登録：項目ID
		// <summary>項目ID：都道府県</summary>
		public const string FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR1 = "UserAddr1";
		// <summary>項目ID：市区町村</summary>
		public const string FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR2 = "UserAddr2";
		// <summary>項目ID：番地</summary>
		public const string FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR3 = "UserAddr3";
		// <summary>項目ID：ビル・マンション名</summary>
		public const string FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR4 = "UserAddr4";
		// <summary>項目ID：州</summary>
		public const string FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ADDR5 = "UserAddr5";
		// <summary>項目ID：生年月日</summary>
		public const string FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_BIRTH = "UserBirth";
		// <summary>項目ID：企業名</summary>
		public const string FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_COMPANY_NAME = "UserCompanyName";
		// <summary>項目ID：部署名</summary>
		public const string FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_COMPANY_POST_NAME = "UserCompanyPostName";
		// <summary>項目ID：モバイルメールアドレス</summary>
		public const string FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_MAIL_ADDR2 = "UserMailAddr2";
		// <summary>項目ID：お知らせメールの配信希望</summary>
		public const string FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_MAIL_FLG = "UserMailFlg";
		// <summary>項目ID：氏名</summary>
		public const string FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_NAME = "UserName";
		// <summary>氏名(かな項目ID：)</summary>
		public const string FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_NAME_KANA = "UserNameKana";
		// <summary>項目ID：ニックネーム</summary>
		public const string FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_NICK_NAME = "UserNickName";
		// <summary>項目ID：性別</summary>
		public const string FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_SEX = "UserSex";
		// <summary>項目ID：電話番号</summary>
		public const string FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_TEL1 = "UserTel1";
		// <summary>項目ID：電話番号（予備）</summary>
		public const string FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_TEL2 = "UserTel2";
		// <summary>項目ID：郵便番号</summary>
		public const string FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_ZIP = "UserZip";
		// <summary>項目ID：国</summary>
		public const string FLG_USER_EASY_REGISTER_SETTING_ITEM_ID_USER_COUNTRY = "UserCountry";

		// ユーザー：楽天IDConnect会員登録ユーザー
		public const string FLG_USER_RAKUTEN_ID_CONNECT_REGISTER_USER_OFF = "0";// 通常会員
		public const string FLG_USER_RAKUTEN_ID_CONNECT_REGISTER_USER_ON = "1";	// 楽天IDConnect会員登録ユーザー

		// ユーザー：ユーザーアクティビティ 区分
		public const string FLG_USERACTIVITY_MASTER_KBN_COORDINATE_LIKE = "COORDINATE_LIKE";		// いいね
		public const string FLG_USERACTIVITY_MASTER_KBN_COORDINATE_FOLLOW = "COORDINATE_FOLLOW";	// フォロー

		// 楽天IDConnectで新規会員登録かどうか
		public const string FLG_RAKUTEN_USER_REGIST = "1";

		// 商品：Googleショッピング連携フラグ
		public const string FLG_PRODUCT_GOOGLE_SHOPPING_FLG_VALID = "1";		// 有効
		public const string FLG_PRODUCT_GOOGLE_SHOPPING_FLG_INVALID = "0";		// 無効

		// 商品：＆mallの予約商品フラグ
		public const string FLG_PRODUCT_ANDMALL_RESERVATION_FLG_COMMON = "001";			// 通常商品
		public const string FLG_PRODUCT_ANDMALL_RESERVATION_FLG_RESERVATION = "002";	// 予約商品

		// 商品バリエーション：＆mallの予約商品フラグ
		public const string FLG_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG_COMMON = "001";	// 通常商品
		public const string FLG_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG_RESERVATION = "002";	// 予約商品

		/// <summary>商品バリエーション：有効フラグ：無効</summary>
		public const string FLG_PRODUCTVARIATION_VALID_FLG_INVALID = "0";
		/// <summary>商品バリエーション：有効フラグ：有効</summary>
		public const string FLG_PRODUCTVARIATION_VALID_FLG_VALID = "1";

		/// <summary>商品バリエーション：削除フラグ：通常</summary>
		public const string FLG_PRODUCTVARIATION_DEL_FLG_UNDELETED = "0";
		/// <summary>商品バリエーション：削除フラグ：削除済み</summary>
		public const string FLG_PRODUCTVARIATION_DEL_FLG_DELETED = "1";

		/// <summary>商品拡張拡張項目：削除フラグ：通常</summary>
		public const string FLG_PRODUCTEXTEND_DEL_FLG_UNDELETED = "0";
		/// <summary>商品拡張拡張項目：削除フラグ：削除済み</summary>
		public const string FLG_PRODUCTEXTEND_DEL_FLG_DELETED = "1";

		// 商品：税区分
		public const string FLG_PRODUCT_TAX_EXCLUDE_PRETAX = "0";	// 税抜き
		public const string FLG_PRODUCT_TAX_INCLUDED_PRETAX = "1";	// 税込み

		// 商品：商品詳細説明区分（概要も同様）
		public const string FLG_PRODUCT_DESC_DETAIL_TEXT = "0";	// TEXT
		public const string FLG_PRODUCT_DESC_DETAIL_HTML = "1";	// HTML

		// 商品；配送サイズ区分
		public const string FLG_PRODUCT_SHIPPING_SIZE_KBN_XXS = "XXS";
		public const string FLG_PRODUCT_SHIPPING_SIZE_KBN_XS = "XS";
		public const string FLG_PRODUCT_SHIPPING_SIZE_KBN_S = "S";
		public const string FLG_PRODUCT_SHIPPING_SIZE_KBN_M = "M";
		public const string FLG_PRODUCT_SHIPPING_SIZE_KBN_L = "L";
		public const string FLG_PRODUCT_SHIPPING_SIZE_KBN_XL = "XL";
		public const string FLG_PRODUCT_SHIPPING_SIZE_KBN_XXL = "XXL";
		public const string FLG_PRODUCT_SHIPPING_SIZE_KBN_MAIL = "MAIL";

		// 商品：付与ポイント1
		public const string FLG_PRODUCT_POINT1_DEFAULT = "0";	// デフォルトポイント

		// 商品：付与ポイント2
		public const string FLG_PRODUCT_POINT2_DEFAULT = "0";	// デフォルトポイント

		// 商品：付与ポイント3
		public const string FLG_PRODUCT_POINT3_DEFAULT = "0";	// デフォルトポイント

		// 商品：付与ポイント区分1
		public const string FLG_PRODUCT_POINT_KBN1_INVALID = "0";	// 設定無し
		public const string FLG_PRODUCT_POINT_KBN1_NUM = "1";	// 値
		public const string FLG_PRODUCT_POINT_KBN1_RATE = "2";	// 率（%)

		// 商品：付与ポイント区分2
		public const string FLG_PRODUCT_POINT_KBN2_INVALID = "0";	// 設定無し
		public const string FLG_PRODUCT_POINT_KBN2_NUM = "1";	// 値
		public const string FLG_PRODUCT_POINT_KBN2_RATE = "2";	// 率（%)

		// 商品：付与ポイント区分3
		public const string FLG_PRODUCT_POINT_KBN3_INVALID = "0";	// 設定無し
		public const string FLG_PRODUCT_POINT_KBN3_NUM = "1";	// 値
		public const string FLG_PRODUCT_POINT_KBN3_RATE = "2";	// 率（%)

		// 商品：キャンペーン付与ポイント区分
		public const string FLG_PRODUCT_CAMPAIGN_POINT_KBN_INVALID = "0";	// 設定なし
		public const string FLG_PRODUCT_CAMPAIGN_POINT_KBN_NUM = "1";	// 値
		public const string FLG_PRODUCT_CAMPAIGN_POINT_KBN_RATE = "2";	// 率（%）

		// 商品：販売期間前表示フラグ
		public const string FLG_PRODUCT_BEFORE_SALE_DISPLAY_KBN_INVALID = "0";	// 販売期間前は表示しない
		public const string FLG_PRODUCT_BEFORE_SALE_DISPLAY_KBN_VALID = "1";	// 販売期間前でも表示する

		// 商品：販売期間後表示フラグ
		public const string FLG_PRODUCT_AFTER_SALE_DISPLAY_KBN_INVALID = "0";	// 販売期間後は表示しない
		public const string FLG_PRODUCT_AFTER_SALE_DISPLAY_KBN_VALID = "1";	// 販売期間後でも表示する

		// 商品：在庫管理方法
		public const string FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED = "0";	// 在庫管理をしない
		public const string FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYOK = "1";	// 在庫０以下の場合でも表示する。購入可能
		public const string FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYNG = "2";	// 在庫０以下の場合でも表示する。購入不可
		public const string FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPNG_BUYNG = "3";	// 在庫０以下の場合は表示しない。購入不可

		// 商品：在庫表示区分
		public const string FLG_PRODUCT_STOCK_DISP_KBN_NUM = "0";	// 在庫を数字で表示
		public const string FLG_PRODUCT_STOCK_DISP_KBN_TEXT = "1";	// 在庫を文言で表示

		// 商品：検索時表示フラグ
		public const string FLG_PRODUCT_DISPLAY_DISP_ALL = "0";	// 商品一覧○、商品詳細○
		public const string FLG_PRODUCT_DISPLAY_DISP_ONLY_DETAIL = "1";	// 商品一覧×、商品詳細○
		public const string FLG_PRODUCT_DISPLAY_UNDISP_ALL = "2";	// 商品一覧×、商品詳細×

		// 商品：アイコン表示フラグ
		public const string FLG_PRODUCT_ICON_1 = "1";	// キャンペーンアイコン１
		public const string FLG_PRODUCT_ICON_2 = "2";	// キャンペーンアイコン２
		public const string FLG_PRODUCT_ICON_3 = "3";	// キャンペーンアイコン３
		public const string FLG_PRODUCT_ICON_4 = "4";	// キャンペーンアイコン４
		public const string FLG_PRODUCT_ICON_5 = "5";	// キャンペーンアイコン５
		public const string FLG_PRODUCT_ICON_6 = "6";	// キャンペーンアイコン６
		public const string FLG_PRODUCT_ICON_7 = "7";	// キャンペーンアイコン７
		public const string FLG_PRODUCT_ICON_8 = "8";	// キャンペーンアイコン８
		public const string FLG_PRODUCT_ICON_9 = "9";	// キャンペーンアイコン９
		public const string FLG_PRODUCT_ICON_10 = "10";	// キャンペーンアイコン１０

		// 商品：アイコン表示フラグ1～5
		public const string FLG_PRODUCT_ICON_OFF = "0";	// 無効
		public const string FLG_PRODUCT_ICON_ON = "1";	// 有効

		// 商品：予約商品フラグ
		public const string FLG_PRODUCT_RESERVATION_FLG_INVALID = "0";	// 予約不可
		public const string FLG_PRODUCT_RESERVATION_FLG_VALID = "1";	// 品切れの場合予約可

		// 商品：定期購入フラグ
		public const string FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID = "0";	// 無効
		public const string FLG_PRODUCT_FIXED_PURCHASE_FLG_VALID = "1";	// 有効
		public const string FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY = "2";	// 定期購入のみ

		// 商品：モバイル表示フラグ
		public const string FLG_PRODUCT_MOBILE_DISP_FLG_ALL = "0";	// ＰＣ・モバイル
		public const string FLG_PRODUCT_MOBILE_DISP_FLG_PC = "1";	// ＰＣのみ
		public const string FLG_PRODUCT_MOBILE_DISP_FLG_MOBLE = "2";    // モバイルのみ
		public const string FLG_PRODUCT_MOBILE_DISP_FLG_EC = "3";   //管理画面のみ

        // 商品：削除フラグ
        public const string FLG_PRODUCT_DELFLG_UNDELETED = "0";	// 通常
		public const string FLG_PRODUCT_DELFLG_DELETED = "1";	// 削除済み

		// 商品：複数バリエーション使用フラグ
		public const string FLG_PRODUCT_USE_VARIATION_FLG_USE_UNUSE = "0";  // 複数バリエーション使用しない
		public const string FLG_PRODUCT_USE_VARIATION_FLG_USE_USE = "1";  // 複数バリエーション使用する

		// 商品：モール連携済フラグ
		public const string FLG_PRODUCT_MALL_COOPERATED_FLG_INVALID = "0";	// モール未連携
		public const string FLG_PRODUCT_MALL_COOPERATED_FLG_VALID = "1";	// モール連携済み

		// 商品：再入荷通知メール有効フラグ
		public const string FLG_PRODUCT_ARRIVAL_MAIL_VALID_FLG_INVALID = "0";	// 無効
		public const string FLG_PRODUCT_ARRIVAL_MAIL_VALID_FLG_VALID = "1";		// 有効

		// 商品：販売開始通知メール有効フラグ
		public const string FLG_PRODUCT_RELEASE_MAIL_VALID_FLG_INVALID = "0";	// 無効
		public const string FLG_PRODUCT_RELEASE_MAIL_VALID_FLG_VALID = "1";		// 有効

		// 商品：再販売通知メール有効フラグ
		public const string FLG_PRODUCT_RESALE_MAIL_VALID_FLG_INVALID = "0";	// 無効
		public const string FLG_PRODUCT_RESALE_MAIL_VALID_FLG_VALID = "1";		// 有効

		// 商品：商品詳細バリエーション選択方法
		public const string FLG_PRODUCT_SELECT_VARIATION_KBN_STANDARD = "STANDARD";	// 従来通り
		public const string FLG_PRODUCT_SELECT_VARIATION_KBN_DROPDOWNLIST = "DROPDOWNLIST";	// STANDARD＋在庫文言表示
		public const string FLG_PRODUCT_SELECT_VARIATION_KBN_DOUBLEDROPDOWNLIST = "DOUBLEDROPDOWNLIST";	// 2つDDLを表示
		public const string FLG_PRODUCT_SELECT_VARIATION_KBN_MATRIX = "MATRIX";	// 表を出力しセルにラジオボタンで選択
		public const string FLG_PRODUCT_SELECT_VARIATION_KBN_MATRIXANDMESSAGE = "MATRIXANDMESSAGE";	// MATRIX＋在庫文言表示

		// 商品：カート投入URL制限フラグ
		public const string FLG_PRODUCT_ADD_CART_URL_LIMIT_FLG_INVALID = "0";		// 無効
		public const string FLG_PRODUCT_ADD_CART_URL_LIMIT_FLG_VALID = "1";		// 有効

		// 商品バリエーション：カート投入URL制限フラグ
		public const string FLG_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG_INVALID = "0";		// 無効
		public const string FLG_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG_VALID = "1";		// 有効

		// 商品：有効フラグ
		public const string FLG_PRODUCT_VALID_FLG_INVALID = "0";		// 無効
		public const string FLG_PRODUCT_VALID_FLG_VALID = "1";		// 有効

		// 商品：会員ランク割引対象フラグ
		public const string FLG_PRODUCT_MEMBER_RANK_DISCOUNT_FLG_INVALID = "0";		// 無効
		public const string FLG_PRODUCT_MEMBER_RANK_DISCOUNT_FLG_VALID = "1";		// 有効

		// 商品：同梱商品明細表示フラグ
		public const string FLG_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE_VALID = "VALID";	// 注文明細や納品書に表示
		public const string FLG_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE_INVALID = "INVALID";	// 注文明細や納品書に非表示

		// 商品：商品区分
		public const string FLG_PRODUCT_PRODUCT_TYPE_PRODUCT = "PRODUCT";	// 通常商品
		public const string FLG_PRODUCT_PRODUCT_TYPE_FLYER = "FLYER";	// チラシ

		// 商品：モールバリエーション種別
		public const string FLG_PRODUCTVARIATION_MALL_VARIATION_TYPE_UNKNOWN = "";		// なし
		public const string FLG_PRODUCTVARIATION_MALL_VARIATION_TYPE_MATRIX = "i";		// 縦軸横軸のマトリクス
		public const string FLG_PRODUCTVARIATION_MALL_VARIATION_TYPE_DROPDOWN = "s";	// ドロップダウン
		public const string FLG_PRODUCTVARIATION_MALL_VARIATION_TYPE_CHECKBOX = "c";	// チェックボックス

		// 商品：外部レコメンド利用フラグ
		public const string FLG_PRODUCT_USE_RECOMMEND_FLG_INVALID = "0";		// 無効
		public const string FLG_PRODUCT_USE_RECOMMEND_FLG_VALID = "1";			// 有効

		// 商品：配送料複数個無料フラグ
		public const string FLG_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG_INVALID = "0"; // 無効
		public const string FLG_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG_VALID = "1"; // 有効

		// 商品：年齢制限フラグ
		public const string FLG_PRODUCT_AGE_LIMIT_FLG_INVALID = "0"; // 無効
		public const string FLG_PRODUCT_AGE_LIMIT_FLG_VALID = "1"; // 有効

		// 商品：ギフト購入フラグ
		public const string FLG_PRODUCT_GIFT_FLG_INVALID = "0";	// 無効
		public const string FLG_PRODUCT_GIFT_FLG_VALID = "1";	// 有効
		public const string FLG_PRODUCT_GIFT_FLG_ONLY = "2";	// ギフトのみ

		// 商品：デジタルコンテンツ商品フラグ
		public const string FLG_PRODUCT_DIGITAL_CONTENTS_FLG_INVALID = "0"; // 無効
		public const string FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID = "1"; // 有効

		// 商品：販売期間表示フラグ
		public const string FLG_PRODUCT_DISPLAY_SELL_FLG_UNDISP = "0";	// 非表示
		public const string FLG_PRODUCT_DISPLAY_SELL_FLG_DISP = "1";	// 表示

		// 定期会員限定フラグ（閲覧）
		public const string FLG_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG_ON = "1";	// 定期会員のみ閲覧可能
		public const string FLG_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG_OFF = "0";	// 閲覧制限なし

		// 定期会員限定フラグ（販売）
		public const string FLG_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG_ON = "1";	// 定期会員のみ購入可能
		public const string FLG_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG_OFF = "0";	// 購入制限なし

		// 商品セット：表示文言HTML区分
		public const string FLG_PRODUCTSET_DESCRIPTION_KBN_TEXT = "0";	// TEXT
		public const string FLG_PRODUCTSET_DESCRIPTION_KBN_HTML = "1";	// HTML

		// 商品：通常商品購入制限チェックフラグ
		public const string FLG_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG_INVALID = "0";	// 無効
		public const string FLG_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG_VALID = "1"; 	// 有効

		// 商品：定期商品購入制限チェックフラグ
		public const string FLG_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG_INVALID = "0";	// 無効
		public const string FLG_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG_VALID = "1"; 	// 有効

		// 商品：会員ランクポイント付与率除外フラグ
		public const string FLG_PRODUCT_CHECK_MEMBER_RANK_POINT_EXCLUDE_FLG_INVALID = "0";	// 無効
		public const string FLG_PRODUCT_CHECK_MEMBER_RANK_POINT_EXCLUDE_FLG_VALID = "1";	// 有効

		// 商品セット：モバイル表示文言HTML区分
		public const string FLG_PRODUCTSET_DESCRIPTION_KBN_MOBILE_TEXT = "0";	// TEXT
		public const string FLG_PRODUCTSET_DESCRIPTION_KBN_MOBILE_HTML = "1";	// HTML

		// 商品セット：有効フラグ
		public const string FLG_PRODUCTSET_VALID_FLG_INVALID = "0";		// 無効
		public const string FLG_PRODUCTSET_VALID_FLG_VALID = "1";		// 有効

		// 商品セットアイテム：親子フラグ
		public const string FLG_PRODUCTSETITEM_FAMILY_FLG_PARENT = "1";	// TEXT
		public const string FLG_PRODUCTSETITEM_FAMILY_FLG_CHILD = "2";	// HTML

		// セットプロモーション：商品金額割引フラグ
		public const string FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_OFF = "0";	// 無効
		public const string FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON = "1";		// 有効

		// セットプロモーション：配送料無料フラグ
		public const string FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_OFF = "0";	// 無効
		public const string FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON = "1";		// 有効

		// セットプロモーション：決済手数料無料フラグ
		public const string FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_OFF = "0";	// 無効
		public const string FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON = "1";	// 有効

		// セットプロモーション：セット価格区分
		public const string FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNTED_PRICE = "0";	// 割引後金額指定
		public const string FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_PRICE = "1";		// 割引金額指定
		public const string FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_RATE = "2";		// 割引率指定

		// セットプロモーション：適用優先順デフォルト値
		public const int FLG_SETPROMOTION_APPLY_ORDER_DEFAULT = 1;

		// セットプロモーション：表示文言HTML区分
		public const string FLG_SETPROMOTION_DESCRIPTION_KBN_TEXT = "0";	// TEXT
		public const string FLG_SETPROMOTION_DESCRIPTION_KBN_HTML = "1";	// HTML

		// セットプロモーション：モバイル表示文言HTML区分
		public const string FLG_SETPROMOTION_DESCRIPTION_KBN_MOBILE_TEXT = "0";	// TEXT
		public const string FLG_SETPROMOTION_DESCRIPTION_KBN_MOBILE_HTML = "1";	// HTML

		// セットプロモーション：有効フラグ
		public const string FLG_SETPROMOTION_VALID_FLG_INVALID = "0";	// 無効
		public const string FLG_SETPROMOTION_VALID_FLG_VALID = "1";		// 有効

		// セットプロモーションアイテム：セットプロモーションアイテム区分
		public const string FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_PRODUCT = "PRODUCT";		// 商品ID指定
		public const string FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_VARIATION = "VARIATION";	// バリエーションID指定
		public const string FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_CATEGORY = "CATEGORY";	// カテゴリID指定
		public const string FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_ALL = "ALL";				// 全商品
		/// <summary>Set promotion item quantity more flag invalid</summary>
		public const string FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_QUANTITY_MORE_FLG_INVALID = "0";
		/// <summary>Set promotion item quantity more flag valid</summary>
		public const string FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_QUANTITY_MORE_FLG_VALID = "1";

		// 商品カテゴリ：削除フラグ
		//public const string FLG_PRODUCT_CATEGORY_DELFLG_UNDELETED = "0";	// 通常
		//public const string FLG_PRODUCT_CATEGORY_DELFLG_DELETED = "1";	// 削除済み

		// 商品カテゴリ：外部レコメンド利用フラグ
		public const string FLG_PRODUCTCATEGORY_USE_RECOMMEND_FLG_INVALID = "0";		// 無効
		public const string FLG_PRODUCTCATEGORY_USE_RECOMMEND_FLG_VALID = "1";			// 有効

		// 商品カテゴリ：有効フラグ
		public const string FLG_PRODUCTCATEGORY_VALID_FLG_INVALID = "0";	// 無効
		public const string FLG_PRODUCTCATEGORY_VALID_FLG_VALID = "1";	// 有効

		// 商品カテゴリ：会員ランク表示制御：カテゴリツリー表示フラグ
		public const string FLG_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG_INVALID = "0";	// 無効
		public const string FLG_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG_VALID = "1";		// 有効

		// 商品カテゴリ：モバイル表示フラグ
		public const string FLG_PRODUCTCATEGORY_MOBILEDISPFLAG_INVALID = "0";	// 無効
		public const string FLG_PRODUCTCATEGORY_MOBILEDISPFLAG_VALID = "1";	// 有効

		// ルートカテゴリ用ソート区分（0:カテゴリーID順, 1:名前順, 2:フリガナ順, 3:表示順)
		public const string FLG_CATEGORY_SORT_BY_CATEGORY_ID = "0";						// カテゴリIDー順 （デフォルト）
		public const string FLG_CATEGORY_SORT_BY_NAME = "1";							// 名前順
		public const string FLG_CATEGORY_SORT_BY_NAME_KANA = "2";						// フリガナ順
		public const string FLG_CATEGORY_SORT_BY_DISPLAY_ORDER = "3";					// 表示順

		// 商品カテゴリ：定期会員限定フラグ
		public const string FLG_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG_INVALID = "0";	// 無効
		public const string FLG_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG_VALID = "1";		// 有効

		// 商品レビュー：公開フラグ
		public const string FLG_PRODUCTREVIEW_OPEN_FLG_INVALID = "0";	// 非公開
		public const string FLG_PRODUCTREVIEW_OPEN_FLG_VALID = "1";		// 公開

		// 商品レビュー：チェックフラグ
		public const string FLG_PRODUCTREVIEW_CHECK_FLG_INVALID = "0";	// 未チェック
		public const string FLG_PRODUCTREVIEW_CHECK_FLG_VALID = "1";    // チェック済

		/// <summary>レビュー投稿ポイント付与フラグ：未付与</summary>
		public const string FLG_PRODUCTREVIEW_REVIEW_REWARD_POINT_FLG_INVALID = "0";
		/// <summary>レビュー投稿ポイント付与フラグ：付与済み</summary>
		public const string FLG_PRODUCTREVIEW_REVIEW_REWARD_POINT_FLG_VALID = "1";

		// 商品在庫履歴：区分
		public const string FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_MASTER_IMPORT = "01";		// マスタ取込
		public const string FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_OPERATION = "02";	// 在庫管理操作
		public const string FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER_MODYIFY = "03";		// 注文変更
		public const string FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER = "11";				// 注文
		public const string FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_RESERVED = "12";	// 注文在庫引当
		public const string FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_FORWARD = "13";		// 注文商品出荷
		public const string FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER_CANCEL = "16";		// 注文キャンセル
		public const string FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_FORWARD_CANCEL = "17";// 注文在庫引当キャンセル
		public const string FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER_RETURN = "18";		// 注文返品
		public const string FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_RETURNED = "19";	// 注文在庫戻し
		public const string FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_ORDER = "21";		// 在庫発注
		public const string FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_DELIVERED = "22";	// 在庫入庫
		public const string FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_PRODUCTVAIRATION_DELETE = "30"; // 商品バリエーション情報を削除した際
		public const string FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER_FOR_COMBINE = "41";		// 注文同梱
		public const string FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER_CANCEL_FOR_COMBINE = "42";	// 注文同梱に伴うキャンセル
		public const string FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_EXTERNAL_API = "40";	// 在庫外部連携

		// 注文：領収書希望フラグ
		public const string FLG_ORDER_RECEIPT_FLG_ON = "1"; // 領収書希望あり
		public const string FLG_ORDER_RECEIPT_FLG_OFF = "0"; // 領収書希望なし
		public const string FLG_ORDER_RECEIPT_FLG_SAME_CART1 = ""; // カート1と同じ

		// 注文：領収書出力フラグ
		public const string FLG_ORDER_RECEIPT_OUTPUT_FLG_ON = "1"; // 領収書出力済み
		public const string FLG_ORDER_RECEIPT_OUTPUT_FLG_OFF = "0"; // 領収書未出力

		// 商品在庫履歴：同期フラグ
		public const int FLG_PRODUCTSTOCKHISTORY_SYNC_FLG_UNSYNCED = 0;	// 未同期
		public const int FLG_PRODUCTSTOCKHISTORY_SYNC_FLG_SYNCED = 1;	// 同期済

		// 商品セールマスタ：セール区分
		public const string FLG_PRODUCTSALE_PRODUCTSALE_KBN_TIMESALES = "TS";
		public const string FLG_PRODUCTSALE_PRODUCTSALE_KBN_CLOSEDMARKET = "CM";

		// 商品セールマスタ：有効フラグ
		public const string FLG_PRODUCTSALE_VALID_FLG_VALID = "1";
		public const string FLG_PRODUCTSALE_VALID_FLG_INVALID = "0";

		// 商品セールマスタ：有効フラグ（有効期間考慮）
		public const string FLG_PRODUCTSALE_VALIDITY_VALID = "1";	// 有効
		public const string FLG_PRODUCTSALE_VALIDITY_INVALID = "0";	// 無効

		// 商品セールマスタ：削除フラグ
		public const string FLG_PRODUCTSALE_DELFLG_UNDELETED = "0";	// 通常
		public const string FLG_PRODUCTSALE_DELFLG_DELETED = "1";	// 削除済み

		// 商品在庫仕入マスタ：発注ステータス
		public const string FLG_STOCKORDER_ORDER_STATUS_UNORDERED = "";			// 未発注
		public const string FLG_STOCKORDER_ORDER_STATUS_ORDERED = "01";			// 発注済み

		// 商品在庫仕入マスタ：入庫ステータス
		public const string FLG_STOCKORDER_DELIVERY_STATUS_UNDELIVERED = "";		// 未入庫
		public const string FLG_STOCKORDER_DELIVERY_STATUS_DELIVERED_INPART = "31";		// 一部入庫済み
		public const string FLG_STOCKORDER_DELIVERY_STATUS_DELIVERED = "41";		// 入庫済み

		// 商品在庫仕入商品マスタ：発注ステータス
		public const string FLG_STOCKORDERITEM_ORDER_STATUS_UNORDERED = "";			// 未発注
		public const string FLG_STOCKORDERITEM_ORDER_STATUS_ORDERED = "01";			// 発注済み

		// 商品在庫仕入商品マスタ：入庫ステータス
		public const string FLG_STOCKORDERITEM_DELIVERY_STATUS_UNDELIVERED = "";		// 未入庫
		public const string FLG_STOCKORDERITEM_DELIVERY_STATUS_DELIVERED_INPART = "31";		// 一部入庫済み
		public const string FLG_STOCKORDERITEM_DELIVERY_STATUS_DELIVERED = "41";		// 入庫済み

		// 商品ランキング：集計タイプ
		public const string FLG_PRODUCTRANKING_TOTAL_TYPE_AUTO = "AUTO";	// 自動
		public const string FLG_PRODUCTRANKING_TOTAL_TYPE_MANUAL = "MANUAL";// 手動

		// 商品ランキング：集計期間区分
		public const string FLG_PRODUCTRANKING_TOTAL_KBN_PERIOD = "PERIOD";	// 期間指定
		public const string FLG_PRODUCTRANKING_TOTAL_KBN_DAYS = "DAYS";		// 日数指定

		// 商品ランキング：ブランド指定
		public const string FLG_PRODUCTRANKING_BRAND_KBN_VALID = "1";		// 指定する
		public const string FLG_PRODUCTRANKING_BRAND_KBN_INVALID = "0";		// 指定しない

		// 商品ランキング：在庫切れ商品
		public const string FLG_PRODUCTRANKING_STOCK_KBN_VALID = "1";		// 含める
		public const string FLG_PRODUCTRANKING_STOCK_KBN_INVALID = "0";		// 含めない

		// 商品ランキング：有効フラグ
		public const string FLG_PRODUCTRANKING_VALID_FLG_VALID = "1";		// 有効
		public const string FLG_PRODUCTRANKING_VALID_FLG_INVALID = "0";		// 無効

		// 商品ランキングアイテム：固定フラグ
		public const string FLG_PRODUCTRANKINGITEM_FIXATION_FLG_ON = "1";	// 固定
		public const string FLG_PRODUCTRANKINGITEM_FIXATION_FLG_OFF = "0";	// 不定

		// 商品タグ：有効フラグ
		public const string FLG_PRODUCTTAGSETTING_VALID_FLG_VALID = "1"; // 有効
		public const string FLG_PRODUCTTAGSETTING_VALID_FLG_INVALID = "0"; // 無効

		// ブランド情報：デフォルト設定フラグ
		public const string FLG_PRODUCTBRAND_DEFAULT_FLG_OFF = "OFF";		// デフォルトブランドではない
		public const string FLG_PRODUCTBRAND_DEFAULT_FLG_ON = "ON";		// デフォルトブランド

		// ブランド情報：有効フラグ
		public const string FLG_PRODUCTBRAND_VALID_FLG_VALID = "1";			// 有効
		public const string FLG_PRODUCTBRAND_VALID_FLG_INVALID = "0";		// 無効

		// カート：定期購入フラグ
		public const string FLG_CART_FIXED_PURCHASE_FLG_ON = "1";		// 定期購入
		public const string FLG_CART_FIXED_PURCHASE_FLG_OFF = "0";		// 通常購入

		// カート：ギフト購入フラグ
		public const string FLG_CART_GIFT_ORDER_FLG_FLG_ON = "1";	// ギフト購入
		public const string FLG_CART_GIFT_ORDER_FLG_FLG_OFF = "0";	// 通常購入

		// カート：デジタルコンテンツフラグ
		public const string FLG_CART_DIGITAL_CONTENTS_FLG_ON = "1";		// デジタルコンテンツ
		public const string FLG_CART_DIGITAL_CONTENTS_FLG_OFF = "0";	// 通常コンテンツ

		#region カート：カートリストLPフラグ
		/// <summary>カートリストLPフラグ：有効</summary>
		public const string FLG_CART_LIST_LP_ON = "1";
		/// <summary>カートリストLPフラグ：無効</summary>
		public const string FLG_CART_LIST_LP_OFF = "0";
		#endregion

		// 注文：モールID
		public const string FLG_ORDER_MALL_ID_OWN_SITE = "OWN_SITE";		// 自社サイト

		// 注文：注文区分
		public const string FLG_ORDER_ORDER_KBN_PC = "PC";		// PC注文
		public const string FLG_ORDER_ORDER_KBN_MOBILE = "MB";	// モバイル注文
		public const string FLG_ORDER_ORDER_KBN_SMARTPHONE = "SP";	// スマートフォン注文
		public const string FLG_ORDER_ORDER_KBN_TEL = "TEL";	// 電話注文
		public const string FLG_ORDER_ORDER_KBN_FAX = "FAX";	// FAX注文

		// 注文：注文ステータス
		public const string FLG_ORDER_ORDER_STATUS_TEMP = "TMP";					// 仮注文
		public const string FLG_ORDER_ORDER_STATUS_ORDERED = "ODR";					// 注文済み
		public const string FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED = "ODR_RECG";	// 受注確認
		public const string FLG_ORDER_ORDER_STATUS_STOCK_RESERVED = "STK_RSVD";		// 在庫引当
		public const string FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED = "SHP_ARGD";		// 出荷手配済み
		public const string FLG_ORDER_ORDER_STATUS_SHIP_COMP = "SHP_COMP";			// 出荷完了
		public const string FLG_ORDER_ORDER_STATUS_DELIVERY_COMP = "DLV_COMP";		// 配送完了
		public const string FLG_ORDER_ORDER_STATUS_ORDER_CANCELED = "ODR_CNSL";		// キャンセル
		public const string FLG_ORDER_ORDER_STATUS_TEMP_CANCELED = "TMP_CNSL";		// 仮注文キャンセル
		public const string FLG_ORDER_ORDER_STATUS_UNKNOWN = "";					// 指定無し

		// 注文：在庫引当ステータス
		public const string FLG_ORDER_ORDER_STOCKRESERVED_STATUS_UNKNOWN = "00";		// 不明
		public const string FLG_ORDER_ORDER_STOCKRESERVED_STATUS_UNRESERVED = "01";		// 在庫未引当
		public const string FLG_ORDER_ORDER_STOCKRESERVED_STATUS_PARTRESERVED = "05";	// 在庫一部引当済
		public const string FLG_ORDER_ORDER_STOCKRESERVED_STATUS_RESERVED = "10";		// 在庫引当済

		// 注文：出荷ステータス
		public const string FLG_ORDER_ORDER_SHIPPED_STATUS_UNKNOWN = "00";		// 不明
		public const string FLG_ORDER_ORDER_SHIPPED_STATUS_UNSHIPPED = "01";	// 未出荷
		//public const string FLG_ORDER_ORDER_SHIPPED_STATUS_PARTSHIPPED = "05";	// 一部出荷済
		public const string FLG_ORDER_ORDER_SHIPPED_STATUS_SHIPPED = "10";		// 出荷済

		// 注文：入金ステータス
		public const string FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM = "0";	// 入金確認待ち
		public const string FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE = "1";	// 入金済
		public const string FLG_ORDER_ORDER_PAYMENT_STATUS_SHORTAGE = "2";	// 一部入金済
		public const string FLG_ORDER_ORDER_PAYMENT_STATUS_UNKNOWN = "9";	// 指定無し

		// 注文：督促ステータス
		public const string FLG_ORDER_DEMAND_STATUS_LEVEL0 = "0";	// 督促なし
		public const string FLG_ORDER_DEMAND_STATUS_LEVEL1 = "1";	// 督促レベル１
		public const string FLG_ORDER_DEMAND_STATUS_LEVEL2 = "2";	// 督促レベル２
		public const string FLG_ORDER_DEMAND_STATUS_LEVEL3 = "3";	// 督促レベル３
		public const string FLG_ORDER_DEMAND_STATUS_UNKNOWN = "9";	// 指定無し

		// 注文：返品交換ステータス
		public const string FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_UNKNOWN = "00";	// 指定無し
		public const string FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_RECEIPT = "01";	// 返品交換受付
		public const string FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_ARRIVAL = "02";	// 返品交換商品到着
		public const string FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_COMPLETE = "03";	// 返品交換完了

		// 注文：返金ステータス
		public const string FLG_ORDER_ORDER_REPAYMENT_STATUS_UNKNOWN = "00";		// 指定無し
		public const string FLG_ORDER_ORDER_REPAYMENT_STATUS_NOREPAYMENT = "01";	// 返金無し
		public const string FLG_ORDER_ORDER_REPAYMENT_STATUS_CONFIRM = "02";		// 未返金
		public const string FLG_ORDER_ORDER_REPAYMENT_STATUS_COMPLETE = "03";		// 返金済み

		// 注文：拡張ステータス
		public const string FLG_ORDER_ORDER_EXTEND_STATUS_ON = "1";		// 拡張情報あり
		public const string FLG_ORDER_ORDER_EXTEND_STATUS_OFF = "0";	// 拡張情報なし
		public const string FLG_ORDER_ORDER_EXTEND_UNKNOWN = "";		// 指定無し

		// 注文：出荷後変更区分
		public const string FLG_ORDER_SHIPPED_CHANGED_KBN_UNKNOWN = "00";			// 指定無し
		public const string FLG_ORDER_SHIPPED_CHANGED_KBN_NOCHANAGED = "01";		// 出荷後変更無し
		public const string FLG_ORDER_SHIPPED_CHANGED_KBN_CHANAGED = "02";			// 出荷後変更有り

		// 注文：返品交換区分
		public const string FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN = "00";	// 指定無し
		public const string FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN = "01";	// 返品
		public const string FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE = "10";	// 交換

		// 注文：ギフト購入フラグ
		public const string FLG_ORDER_GIFT_FLG_ON = "1";	// ON
		public const string FLG_ORDER_GIFT_FLG_OFF = "0";	// OFF

		// 注文：デジタルコンテンツ商品フラグ
		public const string FLG_ORDER_DIGITAL_CONTENTS_FLG_ON = "1"; // ON
		public const string FLG_ORDER_DIGITAL_CONTENTS_FLG_OFF = "0"; // OFF

		// 注文：配送料の別途見積もり利用フラグ
		public const string FLG_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_VALID = "1"; // 有効(見積中)
		public const string FLG_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_INVALID = "0"; // 無効(見積済)

		// 注文：税区分
		public const string FLG_ORDER_ORDER_TAX_EXCLUDE_PRETAX = "0";	// 税抜き
		public const string FLG_ORDER_ORDER_TAX_INCLUDED_PRETAX = "1";	// 税込み

		// 注文：税計算方法
		public const string FLG_ORDER_ORDER_TAX_EXCLUDED_FRACTION_ROUNDING_ROUND_OFF = "ROUNDOFF";	// 四捨五入
		public const string FLG_ORDER_ORDER_TAX_EXCLUDED_FRACTION_ROUNDING_ROUND_DOWN = "ROUNDDOWN";	// 切り捨て
		public const string FLG_ORDER_ORDER_TAX_EXCLUDED_FRACTION_ROUNDING_ROUND_UP = "ROUNDUP";		// 切り上げ

		// 注文：決済取引ID
		public const string FLG_REALSALES_TEMP_TRAN_ID = "TEMP_CARD_TRAN_ID";	// 実売上連携時の仮取引ID

		// 注文：返品交換都合区分
		public const string FLG_ORDER_RETURN_EXCHANGE_REASON_KBN_UNKNOWN = "00";	// 指定無し
		public const string FLG_ORDER_RETURN_EXCHANGE_REASON_KBN_USER = "01";		// 顧客都合
		public const string FLG_ORDER_RETURN_EXCHANGE_REASON_KBN_SALES = "02";		// 販売元都合
		public const string FLG_ORDER_RETURN_EXCHANGE_REASON_KBN_EITHER = "03";		// どちらともいえない

		// 注文：拡張ステータス
		public const string FLG_ORDER_EXTEND_STATUS_OFF = "0";
		public const string FLG_ORDER_EXTEND_STATUS_ON = "1";

		// 注文：オンライン決済ステータス
		public const string FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE = ""; // 未連携
		public const string FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED = "01"; // 売上確定済
		public const string FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED = "09"; // キャンセル済

		// 注文：再注文フラグ
		public const string FLG_REORDER_FLG_OFF = "0";	// 再注文不可
		public const string FLG_REORDER_FLG_ON = "1";	// 再注文可能

		// 注文：外部決済ステータス
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE = "NONE";				// 連携なし
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED = "UNCONFIRMED";	// 未決済
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_WAIT = "AUTH_WAIT";		// 与信待ち
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST = "AUTH_MIDST";	// 与信中
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP = "AUTH_COMP";		// 与信済み
		/// <summary>外部決済スタータス：保留中</summary>
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_PEND = "AUTH_PEND";
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_INV_MIDST = "INV_MIDST";		// 請求書印字情報取得中
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_INV_COMP = "INV_COMP";		// 請求書印字情報取得済み
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_WAIT = "SHIP_WAIT";		// 出荷報告中
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_COMP = "SHIP_COMP";		// 出荷報告済み
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP = "SALES_COMP";	// 売上確定済み
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_DELI_COMP = "DELI_COMP";		// 配送完了
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP = "PAY_COMP";		// 入金済み
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL = "CANCEL";			// キャンセル
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND = "REFUND";			// 返金済み
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_ERROR = "AUTH_ERROR";	// 与信エラー
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_ERROR = "SALES_ERROR";	// 売上確定エラー
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR = "CANCEL_ERROR";// 与信取消しエラー
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_INV_ERROR = "INV_ERROR";		// 請求書印字情報取得エラー
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_ERROR = "SHIP_ERROR";	// 出荷報告エラー
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_ERROR = "ERROR";				// その他エラー
		/// <summary>外部決済スタータス：: 与信期限切れ</summary>
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_EXPIRED = "AUTH_EXPIRED";
		/// <summary>外部決済スタータス：: 売上取消期限切れ</summary>
		public const string FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_CANCEL_EXPIRED = "SALES_CANCEL_EXPIRED";

		// 注文：外部連携取込ステータス
		public const string FLG_ORDER_EXTERNAL_IMPORT_STATUS_SUCCESS = "COMP";	// 正常
		public const string FLG_ORDER_EXTERNAL_IMPORT_STATUS_ERROR = "ERR";	// 異常
		public const string FLG_ORDER_EXTERNAL_IMPORT_STATUS_WARNING = "WARN";	// 要確認

		// 注文：最終与信フラグ
		public const string FLG_ORDER_LAST_AUTH_FLG_OFF = "0";		// OFF：最終与信の注文ではない
		public const string FLG_ORDER_LAST_AUTH_FLG_ON = "1";		// ON：最終与信の注文ではない

		// 注文：モール連携ステータス
		public const string FLG_ORDER_MALL_LINK_STATUS_PEND_ODR = "PEND_ODR";		// 保留注文
		public const string FLG_ORDER_MALL_LINK_STATUS_UNSHIP_ODR = "UNSHIP_ODR";		// 未出荷注文
		public const string FLG_ORDER_MALL_LINK_STATUS_SHIPNOTE_PROGRESS = "SHIPNOTE_PROGRESS";		// 出荷通知処理中
		public const string FLG_ORDER_MALL_LINK_STATUS_SHIPNOTE_COMP = "SHIPNOTE_COMP";		// 出荷通知済み
		public const string FLG_ORDER_MALL_LINK_STATUS_CANCEL = "CANCEL";	// キャンセル
		public const string FLG_ORDER_MALL_LINK_STATUS_NONE = "";	// モール注文以外

		/// <summary>注文請求書同梱フラグ：OFF</summary>
		public const string FLG_ORDER_INVOICE_BUNDLE_FLG_OFF = "0";
		/// <summary>注文請求書同梱フラグ：ON</summary>
		public const string FLG_ORDER_INVOICE_BUNDLE_FLG_ON = "1";

		/// <summary>Flag order store pickup status：default</summary>
		public const string FLG_ORDER_ORDER_STORE_PICKUP_STATUS_DEFAULT = "";
		/// <summary>Flag order store pickup status：pending</summary>
		public const string FLG_ORDER_ORDER_STORE_PICKUP_STATUS_PENDING = "PENDING";
		/// <summary>Flag order store pickup status：arrived</summary>
		public const string FLG_ORDER_ORDER_STORE_PICKUP_STATUS_ARRIVED = "ARRIVED";
		/// <summary>Flag order store pickup status：delivered</summary>
		public const string FLG_ORDER_ORDER_STORE_PICKUP_STATUS_DELIVERED = "DELIVERED";
		/// <summary>Flag order store pickup status：returned</summary>
		public const string FLG_ORDER_ORDER_STORE_PICKUP_STATUS_RETURNED = "RETURNED";

		// 注文者：注文者区分
		public const string FLG_ORDEROWNER_OWNER_KBN_PC_USER = "PC_USER";		// PC会員
		public const string FLG_ORDEROWNER_OWNER_KBN_PC_GUEST = "PC_GEST";		// PCゲスト
		public const string FLG_ORDEROWNER_OWNER_KBN_MOBILE_USER = "MB_USER";	// モバイル会員一般
		public const string FLG_ORDEROWNER_OWNER_KBN_MOBILE_GUEST = "MB_GEST";	// モバイルゲスト
		public const string FLG_ORDEROWNER_OWNER_KBN_SMARTPHONE_USER = "SP_USER";	// スマートフォン会員一般
		public const string FLG_ORDEROWNER_OWNER_KBN_SMARTPHONE_GUEST = "SP_GEST";	// スマートフォンゲスト
		public const string FLG_ORDEROWNER_OWNER_KBN_OFFLINE_USER = "OFF_USER";		// オフライン会員
		public const string FLG_ORDEROWNER_OWNER_KBN_OFFLINE_GUEST = "OFF_GEST";	// オフラインゲスト

		// 注文者：性別
		public const string FLG_ORDEROWNER_OWNER_SEX_MALE = "MALE";		// 男
		public const string FLG_ORDEROWNER_OWNER_SEX_FEMALE = "FEMALE";	// 女
		public const string FLG_ORDEROWNER_OWNER_SEX_UNKNOWN = "UNKNOWN";		// 不明

		// 注文商品：税込フラグ
		public const string FLG_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG_OFF = "0"; // 税抜き
		public const string FLG_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG_ON = "1"; // 税込み

		// 注文商品：税計算方法
		public const string FLG_ORDERITEM_PRODUCT_TAX_ROUND_TYPE_ROUNDOFF = "ROUNDOFF"; // 四捨五入
		public const string FLG_ORDERITEM_PRODUCT_TAX_ROUND_TYPE_ROUNDDOWN = "ROUNDDOWN"; // 切り捨て
		public const string FLG_ORDERITEM_PRODUCT_TAX_ROUND_TYPE_ROUNDUP = "ROUNDUP"; // 切り上げ

		// 注文商品：返品交換区分
		/// <summary> 返品交換区分：指定無し </summary>
		public const string FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_UNKNOWN = "00";
		/// <summary> 返品交換区分：返品 </summary>
		public const string FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN = "01";
		/// <summary> 返品交換区分：交換 </summary>
		public const string FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_EXCHANGE = "10";

		// 注文商品：在庫戻し済みフラグ
		public const string FLG_ORDERITEM_STOCK_RETURNED_FLG_NORETURN = "0";	// 在庫戻しなし
		public const string FLG_ORDERITEM_STOCK_RETURNED_FLG_RETURNED = "1";	// 在庫戻し済み

		// 注文商品：定期商品フラグ
		public const string FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON = "1";	// ON
		public const string FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF = "0";	// OFF

		// 注文商品：同梱商品明細表示フラグ
		public const string FLG_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE_VALID = "VALID";	// 注文明細や納品書に同梱商品を表示
		public const string FLG_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE_INVALID = "INVALID";	// 注文明細や納品書に同梱商品を非表示

		// 注文商品：明細表示フラグ
		public const string FLG_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE_VALID = "VALID";	// 注文明細や納品書に表示
		public const string FLG_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE_INVALID = "INVALID";	// 注文明細や納品書に非表示

		// 注文セットプロモーション：商品金額割引フラグ
		public const string FLG_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG_OFF = "0";	// 無効
		public const string FLG_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG_ON = "1";		// 有効

		// 注文セットプロモーション：配送料無料フラグ
		public const string FLG_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG_OFF = "0";	// 無効
		public const string FLG_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON = "1";		// 有効

		// 注文セットプロモーション：決済手数料無料フラグ
		public const string FLG_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG_OFF = "0";	// 無効
		public const string FLG_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON = "1";	// 有効

		// 注文ワークフロー設定：ワークフロー区分
		public const string FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_DAY = "001";	// 日次ワーク
		public const string FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_MONTH = "002";	// 月次ワーク
		public const string FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_YEAR = "003";	// 年次ワーク
		public const string FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_OTHER = "009";	// その他
		public const string FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_RETURN_EXCHANGE = "100";	// 返品交換ワーク
		public const string FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_SYSTEM = "999";	// システム利用

		// 注文ワークフロー設定：有効フラグ
		public const string FLG_ORDERWORKFLOWSETTING_VALID_FLG_VALID = "1";	// 有効
		public const string FLG_ORDERWORKFLOWSETTING_VALID_FLG_INVALID = "0";	// 無効

		// 注文ワークフロー設定：ワークフロー詳細区分
		public const string FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_NORMAL = "NORMAL";	// 通常のワークフロー
		public const string FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_ODR_IMP = "ODR_IMP";	// 注文関連ファイル取込ポップアップ
		public const string FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_RETURN = "RETURN";	// Return WorkFlow

		// 注文ワークフロー設定：表示区分
		public const string FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE = "LINE";	// １行表示
		public const string FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_CASSETTE = "CASSETTE";	// カセット表示

		// 注文ワークフロー設定：追加検索可否フラグ
		public const string FLG_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG_OFF = "0";	// 通常検索
		public const string FLG_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG_ON = "1";	// 追加検索（受注情報）
		public const string FLG_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FIXEDPURCHASE_FLG_ON = "2";	// 追加検索（定期台帳）

		// 注文ワークフロー設定：注文ステータス変更区分
		public const string FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_ORDERED = "ODR";	// 注文済みへ変更
		public const string FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_ORDER_RECOGNIZED = "ODR_RECG";	// 受注確認へ変更
		public const string FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_STCOK_RESERVED = "STK_RSVD";	// 在庫引当へ変更
		public const string FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_SHIP_ARRANGED = "SHP_ARGD";		// 出荷手配済みへ変更
		public const string FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_SHIP_COMP = "SHP_COMP";	// 出荷完了
		public const string FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_DELIVERY_COMP = "DLV_COMP";	// 配送完了へ変更
		public const string FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_ORDER_CANCELED = "ODR_CNSL";	// キャンセルへ変更
		public const string FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_TEMP_CANCELED = "TMP_CNSL";	// 仮注文キャンセルへ変更

		// 注文ワークフロー設定：商品在庫変更区分
		public const string FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_RESERVED_STCOK = "12";		// 実在庫引当（在庫引当）
		public const string FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_FORWARD_STCOK = "13";			// 実在庫出荷（商品出荷）
		public const string FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_CANCEL_REALSTCOK = "17";		// 実在庫引当戻し

		// 注文ワークフロー設定：入金ステータス変更区分
		public const string FLG_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE_CONFIRM = "0";	// 入金待ちへ変更変更
		public const string FLG_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE_COMPLETE = "1";	// 入金済へ変更
		public const string FLG_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE_SHORTAGE = "2";	// 一部入金済へ変更

		// 注文ワークフロー設定：外部決済連携処理区分
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ZEUS_CREDITCARD_PAYMENT = "0";	// Zeus決済連携
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_DOCOMO_PAYMENT = "1";	// ドコモケータイ払い決済
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SOFTBANK_PAYMENT = "2";	// S!まとめて支払い決済
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_CREDITCARD_SALES = "6";	// SBPSクレジット売上請求要求
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_SOFTBANKKETAI_PAYMENT = "7";	// SBPS ソフトバンク・ワイモバイルまとめて支払い売上要求
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_DOCOMOKETAI_PAYMENT = "8";	// SBPS ドコモケータイ払い売上要求
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_AUKANTAN_PAYMENT = "9";		// SBPS auかんたん決済売上要求
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CREDITCARD_SALES = "10";		// GMOクレジット売上請求確定
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_RECRUIT_PAYMENT = "11";　　　　// SBPS リクルートかんたん支払い売上要求
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_RAKUTEN_ID_PAYMENT = "12";　　　　// SBPS 楽天ペイ売上要求
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：Gmo後払い出荷報告</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CVS_DEF_SHIP = "14";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：Amazon Pay</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_AMAZON_PAYMENT = "15";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：PayPal</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAYPAL_PAYMENT = "16";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：後付款(TriLink後払い)出荷報告</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_TRILINK_AFTERPAY_SHIP = "17";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：Atodene後払い出荷報告</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ATODENE_CVS_DEF_SHIP = "18";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：Zcomクレジット売上請求要求</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ZCOM_CREDITCARD_SALES = "19";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：Paidy Pay</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAIDY_PAYMENT = "20";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：LINE Pay翌月払い</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_LINE_PAYMENT = "21";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：atone翌月払い</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ATONE_PAYMENT = "34";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：aftee翌月払い</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_AFTEE_PAYMENT = "35";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：NP後払い出荷報告  </summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_NP_AFTERPAY_SHIP = "22";
		/// <summary>注文ワークフロー設定：ECPay受注情報処理</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_ECPAY = "23";
		/// <summary>注文ワークフロー設定：ネクストエンジン受注情報アップロード</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_NEXTENGINE = "24";
		/// <summary>注文ワークフロー設定：ネクストエンジン受注ステータス取込</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_NEXTENGINE_IMPORT = "25";
		/// <summary>注文ワークフロー設定：クロスモール受注情報ステータス更新</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_CROSSMALL_UPDATE_STATUS = "27";
		/// <summary>注文ワークフロー設定：Recustomer受注連携</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_RECUSTOMER = "28";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：Ec Pay</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_EC_PAYMENT = "37";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：Gmo Deffered Payment Invoice Reissue</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CVS_DEF_INVOICE_REISSUE = "38";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：NewebPay</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_NEWEB_PAYMENT = "39";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：EScott</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ESCOTT_CREDITCARD_SALES = "26";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：ベリトランスクレジット売上請求要求</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_VERITRANS_CREDITCARD_SALES = "40";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：DSK後払い出荷報告</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_DSK_CVS_DEF_SHIP = "41";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：PayPay</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAYPAY_PAYMENT = "42";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：Rakutenクレジット売上請求要求</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_RAKUTEN_CREDITCARD_PAYMENT = "43";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：Atobaraicom後払い出荷報告</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ATOBARAICOM_CVS_DEF_SHIP = "44";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：キャリア決済（Boku)</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_CARRIERBILLING_BOKU = "45";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：GMO掛け払い　※請求確定処理</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_INVOICE = "46";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：スコア後払い出荷報告</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SCORE_CVS_DEF_SHIP = "47";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：ベリトランス後払い出荷報告</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_VERITRANS_CVS_DEF_SHIP = "48";
		/// <summary>注文ワークフロー設定：外部決済連携処理区分：ペイジェントクレジット売上請求要求</summary>
		public const string FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAYGENT_CREDITCARD_SALES = "49";

		// 注文ワークフロー設定：督促ステータス変更区分
		public const string FLG_ORDERWORKFLOWSETTING_DEMAND_STATUS_CHANGE_LEVEL1 = "1";	// 督促レベル１
		public const string FLG_ORDERWORKFLOWSETTING_DEMAND_STATUS_CHANGE_LEVEL2 = "2";	// 督促レベル２
		public const string FLG_ORDERWORKFLOWSETTING_DEMAND_STATUS_CHANGE_LEVEL3 = "3";	// 督促レベル３

		// 注文ワークフロー設定：返品交換ステータス変更区分
		public const string FLG_ORDERWORKFLOWSETTING_RETURN_EXCHANGE_STATUS_CHANGE_RECEIPT = "01";	// 返品交換受付
		public const string FLG_ORDERWORKFLOWSETTING_RETURN_EXCHANGE_STATUS_CHANGE_ARRIVAL = "02";	// 返品交換商品到着
		public const string FLG_ORDERWORKFLOWSETTING_RETURN_EXCHANGE_STATUS_CHANGE_COMPLETE = "03";	// 返品交換完了

		// 注文ワークフロー設定：返金ステータス変更区分
		public const string FLG_ORDERWORKFLOWSETTING_REPAYMENT_STATUS_CHANGE_NOREPAYMENT = "01";	// 返金無し
		public const string FLG_ORDERWORKFLOWSETTING_REPAYMENT_STATUS_CHANGE_CONFIRM = "02";		// 未返金
		public const string FLG_ORDERWORKFLOWSETTING_REPAYMENT_STATUS_CHANGE_COMPLETE = "03";		// 返金済み

		// 注文ワークフロー設定：複数設定未実行フラグFIELD_
		public const string FLG_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE_OFF = "0";	// チェック無
		public const string FLG_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE_ON = "1";	// チェック有

		// 受注ワークフロー設定：ワークフロー対象区分
		public const string FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_ORDER = "order"; // 通常ワークフロー
		public const string FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_FIXED_PURCHASE = "fixed_purchase"; // 定期ワークフロー

		// 注文ワークフロー設定：出荷予定日アクション
		public const string FLG_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION_ON = "1";	// チェック有
		public const string FLG_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION_OFF = "0";	// チェック無

		// Return action
		public const string FLG_ORDERWORKFLOWSETTING_RETURN_ACTION_NOT_CHANGE = "0";	// Not change
		public const string FLG_ORDERWORKFLOWSETTING_RETURN_ACTION_ACCEPT_RETURN = "1";	// Accept return

		// Return reason Kbn
		public const string FLG_ORDERWORKFLOWSETTING_RETURN_REASON_KBN_CUSTOMER_CONVENIENCE = "01";	// Customer Convenience
		public const string FLG_ORDERWORKFLOWSETTING_RETURN_REASON_KBN_SALES_AGENCY_CONVENIENCE = "02";	// Sales Agency Convenience
		public const string FLG_ORDERWORKFLOWSETTING_RETURN_REASON_KBN_OTHER = "03";	// Neither Agree Nor Disagree

		// 注文ワークフロー設定：配送希望日アクション
		public const string FLG_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION_OFF = "0";	// 変更なし
		public const string FLG_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION_ON_CALCULATE_SCHEDULED_SHIPPING_DATE = "1";	// 変更する(出荷予定日を自動計算する)
		public const string FLG_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION_ON_NONCALCULATE_SCHEDULED_SHIPPING_DATE = "2";	// 変更する(出荷予定日を自動計算しない)

		// 注文ワークフロー設定：注文種別
		public const string FLG_ORDERWORKFLOWSETTING_TARGET_ORDER_TYPE_FIXED_PURCHASE = "FIXED_PURCHASE";	// 定期注文
		public const string FLG_ORDERWORKFLOWSETTING_TARGET_ORDER_TYPE_NORMAL = "NORMAL";	// 通常注文

		// 注文ワークフロー設定：削除フラグ
		public const string FLG_ORDERWORKFLOWSETTING_DEL_FLG_NOMAL = "0";	// 定期注文
		public const string FLG_ORDERWORKFLOWSETTING_DEL_FLG_DELETED = "1";	// 通常注文

		// 注文ワークフロー設定：キャンセル可能な注文を抽出しない
		public const string FLG_ORDERWORKFLOWSETTING_DONOT_EXTRACT_CANCELABLE_ORDERS_ON = "1";	// チェック有
		public const string FLG_ORDERWORKFLOWSETTING_DONOT_EXTRACT_CANCELABLE_ORDERS_OFF = "0"; // チェック無

		// 注文ワークフロー設定：店舗受取ステータス
		public const string FLG_ORDERWORKFLOWSETTING_ORDER_STORE_PICKUP_STATUS_DEFAULT = "";
		public const string FLG_ORDERWORKFLOWSETTING_ORDER_STORE_PICKUP_STATUS_CHANGE_PENDING = "PENDING";
		public const string FLG_ORDERWORKFLOWSETTING_ORDER_STORE_PICKUP_STATUS_CHANGE_ARRIVED = "ARRIVED";
		public const string FLG_ORDERWORKFLOWSETTING_ORDER_STORE_PICKUP_STATUS_CHANGE_DELIVERED = "DELIVERED";
		public const string FLG_ORDERWORKFLOWSETTING_ORDER_STORE_PICKUP_STATUS_CHANGE_RETURNED = "RETURNED";

		// 定期ワークフロー設定：定期購入状況変更アクション
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_NONE = "";	// 変更なし
		/// <summary> 定期ワークフロー設定：完了 </summary>
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_COMPLETE = "complete";
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_CANCEL = "cancel";	// 解約
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_RESTART = "restart";	// 購入再開
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_SUSPEND = "suspend";	// 休止

		// 定期ワークフロー設定：定期決済ステータス変更アクション
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE_ACTION_NONE = "";	// 変更なし
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE_ACTION_NOMAL = "1";	// 通常
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE_ACTION_FAILED = "2";	// 失敗

		// 定期ワークフロー設定：次回配送日変更アクション
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_NEXT_SHIPPING_DATE_CHANGE_ACTION_OFF = "";	// 変更なし
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_NEXT_SHIPPING_DATE_CHANGE_ACTION_ON_WITH_CALCULATE_NEXT_NEXT_SHIPPINGDATE = "1";	// 変更する(次々回配送日も自動計算する)
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_NEXT_SHIPPING_DATE_CHANGE_ACTION_ON = "2";	// 変更する(次々回配送日の自動計算をしない)

		// 定期ワークフロー設定：次々回配送日変更アクション
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_NEXT_NEXT_SHIPPING_DATE_CHANGE_ACTION_OFF = "";	// 変更なし
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_NEXT_NEXT_SHIPPING_DATE_CHANGE_ACTION_ON = "1";	// 変更する

		// 定期ワークフロー設定：配送不可エリア停止変更アクション
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE_ACTION_NONE = "";	// 変更なし
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE_ACTION_ON = "1";	// 配送不可エリア停止にする
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE_ACTION_OFF = "0";	// 配送不可エリア停止状態を解除する
		 
		// 定期台帳ワークフロー: 有効フラグ
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_VALIDFLG_FALSE = "0";	// 無効
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_VALIDFLG_TRUE = "1";	// 有効

		// 定期台帳ワークフロー: ワークフロー詳細区分
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_NORMAL = "NORMAL";	// 通常のワークフロー
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_ODR_IMP = "ODR_IMP";	// 受注系ファイル取込ポップアップ
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_BUNDLE = "BUNDLE";	// 同梱処理ポップアップ

		// 定期台帳ワークフロー: 表示区分
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_KBN_LINE = "LINE";	// 一行表示
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_KBN_CASSETTE = "CASSETTE";	// カセット表示

		// 定期台帳ワークフロー: 追加検索可否FLG
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG_NOMAL = "0";	// 通常検索
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG_ADD = "2";	// 追加検索

		// 定期ワークフロー設定：複数設定未実行フラグFIELD_
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NO_UPDATE_OFF = "0";	// チェック無
		public const string FLG_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NO_UPDATE_ON = "1";	// チェック有

		// 受注ワークフローシナリオ設定
        public const string TABLE_ORDERWORKFLOWSCENARIOSETTING = "w2_OrderWorkflowScenarioSetting"; // 受注ワークフローシナリオ設定
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTING_SCENARIO_SETTING_ID = "scenario_setting_id";// シナリオ設定ID
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTING_SCENARIO_NAME = "scenario_name";     // シナリオ名
		public const string FIELD_ORDERWORKFLOWSCENARIOSETTING_EXEC_TIMING = "exec_timing";         // 実行タイミング
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_KBN = "schedule_kbn";       // スケジュール区分
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_DAY_OF_WEEK = "schedule_day_of_week";// スケジュール曜日
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_YEAR = "schedule_year";     // スケジュール日程(年)
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_MONTH = "schedule_month";   // スケジュール日程(月)
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_DAY = "schedule_day";       // スケジュール日程(日)
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_HOUR = "schedule_hour";     // スケジュール日程(時)
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_MINUTE = "schedule_minute"; // スケジュール日程(分)
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_SECOND = "schedule_second"; // スケジュール日程(秒)
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTING_VALID_FLG = "valid_flg";             // 有効フラグ
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTING_DATE_CREATED = "date_created";       // 作成日
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTING_DATE_CHANGED = "date_changed";       // 更新日
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTING_LAST_CHANGED = "last_changed";       // 最終更新者
		public const string NUMBER_KEY_SCENARIO_SETTING_ID = "ScenarioSettingId";

        // 受注ワークフローシナリオ設定アイテム
        public const string TABLE_ORDERWORKFLOWSCENARIOSETTINGITEM = "w2_OrderWorkflowScenarioSettingItem";// シナリオ設定アイテム
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_SCENARIO_SETTING_ID = "scenario_setting_id";// シナリオ設定ID
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_SCENARIO_NO = "scenario_no";     // シナリオ枝番
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_SHOP_ID = "shop_id";             // 店舗ID
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_WORKFLOW_KBN = "workflow_kbn";   // ワークフロー区分
        public const string FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_WORKFLOW_NO = "workflow_no";     // ワークフロー枝番
		public const string FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_TARGET_WORKFLOW_KBN = "target_workflow_kbn";// 実行対象ワークフロー区分

		// 受注ワークフロー実行履歴
		public const string TABLE_ORDERWORKFLOWEXECHISTORY = "w2_OrderWorkflowExecHistory";         // 受注ワークフロー実行履歴
        public const string FIELD_ORDERWORKFLOWEXECHISTORY_ORDER_WORKFLOW_EXEC_HISTORY_ID = "order_workflow_exec_history_id";// 受注ワークフロー実行履歴ID
        public const string FIELD_ORDERWORKFLOWEXECHISTORY_SHOP_ID = "shop_id";                     // 店舗ID
        public const string FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_KBN = "workflow_kbn";           // ワークフロー区分
        public const string FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_NO = "workflow_no";             // ワークフロー枝番
        public const string FIELD_ORDERWORKFLOWEXECHISTORY_SCENARIO_SETTING_ID = "scenario_setting_id";// シナリオ設定ID
        public const string FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_NAME = "workflow_name";         // ワークフロー名
        public const string FIELD_ORDERWORKFLOWEXECHISTORY_SCENARIO_NAME = "scenario_name";         // シナリオ名
        public const string FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS = "exec_status";             // 実行ステータス
        public const string FIELD_ORDERWORKFLOWEXECHISTORY_SUCCESS_RATE = "success_rate";           // 成功件率
        public const string FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE = "workflow_type";         // ワークフロー種別
        public const string FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE = "exec_place";               // 実行起点
        public const string FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING = "exec_timing";             // 実行タイミング
        public const string FIELD_ORDERWORKFLOWEXECHISTORY_MESSAGE = "message";                     // メッセージ
        public const string FIELD_ORDERWORKFLOWEXECHISTORY_DATE_BEGIN = "date_begin";               // 開始日時
        public const string FIELD_ORDERWORKFLOWEXECHISTORY_DATE_END = "date_end";                   // 終了日時
        public const string FIELD_ORDERWORKFLOWEXECHISTORY_DATE_CREATED = "date_created";           // 作成日
        public const string FIELD_ORDERWORKFLOWEXECHISTORY_LAST_CHANGED = "last_changed";           // 最終更新者

		// 受注ワークフローシナリオ：有効フラグ
		public const string FLG_ORDERWORKFLOWSCENARIOSETTING_VALID_FLG_VALID = "ON";	// ON
		public const string FLG_ORDERWORKFLOWSCENARIOSETTING_VALID_FLG_INVALID = "OFF";	// OFF

		// 受注ワークフローシナリオ：スケジュール区分
		public const string FLG_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_KBN_DAY = "01";	// 日単位
		public const string FLG_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_KBN_WEEK = "02";	// 週単位
		public const string FLG_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_KBN_MONTH = "03";	// 月単位
		public const string FLG_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_KBN_ONCE = "04";  // 一回のみ

		// 受注ワークフローシナリオ：実行対象ワークフロー区分
		public const string FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_NORMAL = "NORMAL";   // 受注
		public const string FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_FIXEDPURCHASE = "FIXED_PURCHASE";	// 定期

		// 受注ワークフロー実行履歴：実行ステータス
		public const string FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_OK = "OK";	// 成功
		public const string FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_NG = "NG";	// 失敗あり
		public const string FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_STOPPED = "Stopped";	// 停止済み
		public const string FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_RUNNING = "Running";	// 実行中
		public const string FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_HOLD = "Hold";	// 保留中

		// 受注ワークフロー実行履歴：実行場所
		public const string FLG_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE_WORKFLOW = "01";	// ワークフロー
		public const string FLG_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE_SCENARIO = "02";	// シナリオ

		// 受注ワークフロー実行履歴：実行タイプ
		public const string FLG_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE_ORDER_WORKFLOW = "01";	//　受注ワークフロー
		public const string FLG_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE_FIXED_PURCHASE_WORKFLOW = "02";	//　定期台帳ワークフロー

		// 受注ワークフロー実行履歴：実行タイミング
		public const string FLG_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING_MANUAL = "01";	// 手動実行
		public const string FLG_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING_SCHEDULE = "02";	// スケジュール実行

		// 受注ワークフロー実行履歴：メッセージ
		public const string FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_SUCCESS = "WorkflowSuccess";	// ワークフロー成功メッセージ
		public const string FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_FAILURE = "WorkflowFailure";	// ワークフロー失敗ありメッセージ
		public const string FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_STOP = "WorkflowStop";	// ワークフロー停止メッセージ
		public const string FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_FIXEDPURCHASE_SUCCESS = "FixedpurchaseSuccess";	// 定期台帳ワークフロー成功メッセージ
		public const string FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_FIXEDPURCHASE_FAILURE = "FixedpurchaseFailure";	// 定期台帳ワークフロー失敗ありメッセージ
		public const string FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_FIXEDPURCHASE_STOP = "FixedpurchaseStop";	// 定期台帳ワークフロー停止済みメッセージ
		public const string FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_RUNNING = "WorkflowRunning";	// ワークフロー実行中メッセージ
		public const string FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_HOLD = "WorkflowHold";	// ワークフロー保留中メッセージ
		public const string FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_CHANGE_FROM_RUNNING_TO_NG = "ChangeFromRunningToNg";	// 実行中から失敗ありになった場合のメッセージ
		// ネクストエンジン受注情報アップロード中、アクセストークンが利用できなかった場合。
		public const string FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_NEXTENGINE_INVALID_ACCESS_TOKEN = "workflowNextEngineInvalidAccessToken";

		// タスクスケジュール：アクション区分
		public const string FLG_TASKSCHEDULE_ACTION_KBN_ORDERWORKFLOW_EXEC_SCENARIO_TO_SCHEDULE = "ExecScenarioToSchedule"; // タスクスケジュール - アクション区分: 受注ワークフローシナリオ(スケジュール実行)
		public const string FLG_TASKSCHEDULE_ACTION_KBN_ORDERWORKFLOW_EXEC_SCENARIO_TO_MANUAL = "ExecScenarioToManual";	// タスクスケジュール - アクション区分: 受注ワークフローシナリオ(手動実行)

		// タスクスケジュール：プログレス
		public const string FLG_TASK_SCHEDULE_WORKFLOW_EXEC_PROGRESS_WAITING_FOR_EXEC = "WorkflowExecAtProgressWaiting";	// ワークフロー実行待ち

		//------------------------------------------------------
		// マスタ出力区分
		//------------------------------------------------------
		/// <summary>マスタ出力区分：商品マスタ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCT = "Product";
		/// <summary>マスタ出力区分：商品バリエーションマスタ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTVARIATION = "ProductVariation";
		/// <summary>マスタ出力区分：商品タグ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTTAG = "ProductTag";
		/// <summary>マスタ出力区分：商品拡張項目</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTEXTEND = "ProductExtend";
		/// <summary>マスタ出力区分：商品在庫</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTSTOCK = "ProductStock";
		/// <summary>マスタ出力区分：商品カテゴリー</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTCATEGORY = "ProductCategory";
		/// <summary>マスタ出力区分：商品価格</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTPRICE = "ProductPrice";
		/// <summary>マスタ出力区分：注文</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER = "Order";
		/// <summary>マスタ出力区分：注文商品</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM = "OrderItem";
		/// <summary>マスタ出力区分：税率毎注文価格情報マスタ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER_PRICE_BY_TAX_RATE = "OrderPriceByTaxRate";
		/// <summary>マスタ出力区分：注文セットプロモーション</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION = "OrderSetPromotion";
		/// <summary>マスタ出力区分：注文ワークフロー</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER_WORKFLOW = "OrderWrokflow";
		/// <summary>マスタ出力区分：注文商品ワークフロー</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM_WORKFLOW = "OrderitemWorkflow";
		/// <summary>マスタ出力区分：定期ワークフロー</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE_WORKFLOW = "fixedPurchaseWorkflow";
		/// <summary>マスタ出力区分：注文セットプロモーションワークフロー</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION_WORKFLOW = "OrderSetPromotionWorkflow";
		/// <summary>マスタ出力区分：データ結合マスタワークフロー</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING_WORKFLOW = "DataBindingWorkflow";
		/// <summary>マスタ出力区分：ユーザー</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_USER = "User";
		/// <summary>マスタ出力区分：ユーザー属性（ユーザーマスタに合算）</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_USERATTRIBUTE = "UserAttribute";
		/// <summary>マスタ出力区分：発注情報</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_STOCKORDER = "StockOrder";
		/// <summary>マスタ出力区分：発注商品情報</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_STOCKORDERITEM = "StockOrderItem";
		/// <summary>マスタ出力区分：商品+バリエーション</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTVIEW = "ProductView";
		/// <summary>マスタ出力区分：モール出品設定情報</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_MALLEXHIBITSCONFIG = "MallExhibitsConfig";
		/// <summary>マスタ出力区分：入荷通知メール情報（サマリー）</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPRODUCTARRIVALMAIL = "UserProductArrivalMail";
		/// <summary>マスタ出力区分：入荷通知メール情報（明細）</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPRODUCTARRIVALMAIL_DETAIL = "UserProductArrivalMailDetail";
		/// <summary>マスタ出力区分：商品レビュー</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTREVIEW = "ProductReview";
		/// <summary>マスタ出力区分：商品セール価格</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTSALEPRICE = "ProductSalePrice";
		/// <summary>マスタ出力区分：ショートURL</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_SHORTURL = "ShortUrl";
		/// <summary>マスタ出力区分：定期購入マスタ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE = "FixedPurchase";
		/// <summary>マスタ出力区分：定期購入商品マスタ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM = "FixedPurchaseItem";
		/// <summary>マスタ出力区分：定期ワークフロー</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM_WORKFLOW = "FixedPurchaseItemWorkflow";
		/// <summary>マスタ出力区分：シリアルキーマスタ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_SERIALKEY = "SerialKey";
		/// <summary>マスタ出力区分：メッセージマスタ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_CSMESSAGE = "CsMessage";
		/// <summary>マスタ出力区分：リアル店舗マスタ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_REALSHOP = "RealShop";
		/// <summary>マスタ出力区分：リアル店舗商品在庫マスタ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_REALSHOPPRODUCTSTOCK = "RealShopProductStock";
		/// <summary>マスタ出力区分：ユーザ配送先マスタ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_USERSHIPPING = "UserShipping";
		/// <summary>マスタ出力区分：広告コードマスタ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE = "AdvCode";
		/// <summary>マスタ出力区分：広告媒体区分マスタ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODEMEDIATYPE = "AdvCodeMediaType";
		/// <summary>マスタ出力区分：ユーザーポイント情報</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPOINT = "UserPoint";
		/// <summary>マスタ出力区分：ターゲットリスト</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_TARGETLISTDATA = "TargetListData";
		/// <summary>マスタ出力区分：クーポンマスタ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON = "Coupon";
		/// <summary>マスタ出力区分：ユーザー電子発票管理マスタ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_USERINVOICE = "UserInvoice";
		/// <summary>マスタ出力区分：データ結合マスタ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING = "DataBinding";
		/// <summary>マスタ出力区分：コーディネート</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE = "Coordinate";
		/// <summary>マスタ出力区分：コーディネートアイテム</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE_ITEM = "CoordinateItem";
		/// <summary>マスタ出力区分：コーディネートカテゴリ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE_CATEGORY = "CoordinateCategory";
		/// <summary>マスタ出力区分：スタッフ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_STAFF = "Staff";
		/// <summary>マスタ出力区分：アフィリエイト連携ログ表示</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_AFFILIATECOOPLOG = "AffiliateCoopLog";
		/// <summary>マスタ出力区分：広告媒体区分マスタ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE_MEDIA_TYPE = "AdvCodeMediaType";
		/// <summary>マスタ出力区分：ポイント情報(詳細)</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPOINT_DETAIL = "UserPointDetail";
		/// <summary>マスタ出力区分：ユーザクーポンマスタ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_USERCOUPON = "UserCoupon";
		/// <summary>マスタ出力区分：クーポン利用ユーザー情報</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON_USE_USER = "CouponUseUser";
		/// <summary>Extend Title</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_EXTEND_TITLE = "ExtendTitle";
		/// <summary>マスタ出力区分：オペレータマスタ</summary>
		public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_OPERATOR = "Operator";

		//------------------------------------------------------
		// マスタ出力ファイル形式区分
		//------------------------------------------------------
		/// <summary>マスタ出力ファイル形式区分：CSV形式</summary>
		public const string FLG_MASTEREXPORTSETTING_EXPORT_FILE_TYPE_CSV = "CSV";
		/// <summary>マスタ出力ファイル形式区分：エクセル形式</summary>
		public const string FLG_MASTEREXPORTSETTING_EXPORT_FILE_TYPE_XLS = "XLS";

		// 店舗管理者：有効フラグ
		public const string FLG_SHOPOPERATOR_VALID_FLG_VALID = "1";	// 有効
		public const string FLG_SHOPOPERATOR_VALID_FLG_INVALID = "0";	// 無効

		// 店舗メニュー権限管理マスタ：デフォルト表示フラグ
		public const string FLG_MENUAUTHORITY_DEFAULT_DISP_FLG_ON = "1";
		public const string FLG_MENUAUTHORITY_DEFAULT_DISP_FLG_OFF = "0";

		// 店舗メニュー権限管理マスタ：パッケージ区分
		public const string FLG_MENUAUTHORITY_PKG_KBN_EC = "1";	// w3Commerce
		public const string FLG_MENUAUTHORITY_PKG_KBN_MP = "2";	// w2MarketingPlanner
		public const string FLG_MENUAUTHORITY_PKG_KBN_CS = "3";	// w2MarketingPlanner
		public const string FLG_MENUAUTHORITY_PKG_KBN_CMS = "4";	// w2Cms

		// 店舗メニュー権限管理マスタ：表示レベル
		public const string FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER = "0";			// スーパーユーザー
		public const string FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_UNACCESSABLEUSER = "";	// 権限なしユーザー

		// 店舗配送料：配送日設定可能フラグ
		public const string FLG_SHIPPING_PRICE_KBN_NONE = "0";	// 配送料なし
		public const string FLG_SHIPPING_PRICE_KBN_SAME = "1";	// 全国一律
		public const string FLG_SHIPPING_PRICE_KBN_AREA = "2";	// 地域別設定

		// 店舗配送種別：配送日設定可能フラグ
		public const string FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_VALID = "1";	// 有効
		public const string FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_INVALID = "0";	// 無効

		// 店舗配送種別：定期購入設定可能フラグ
		public const string FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_VALID = "1"; // 有効
		public const string FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_INVALID = "0"; // 無効

		// 店舗配送種別：定期購入区分1設定可能フラグ
		public const string FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG_VALID = "1"; // 有効
		public const string FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG_INVALID = "0"; // 無効

		// 店舗配送種別：定期購入区分2設定可能フラグ
		public const string FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG_VALID = "1"; // 有効
		public const string FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG_INVALID = "0"; // 無効

		// 店舗配送種別：定期購入区分3設定可能フラグ
		public const string FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN3_FLG_VALID = "1"; // 有効
		public const string FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN3_FLG_INVALID = "0"; // 無効

		/// <summary>店舗配送種別：定期購入区分4設定可能フラグ:有効</summary>
		public const string FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG_VALID = "1";
		/// <summary>店舗配送種別：定期購入区分4設定可能フラグ:無効</summary>
		public const string FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG_INVALID = "0";

		// 店舗配送種別：フロント側配送パターン非表示フラグ
		public const string FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN3_DEFAULT_SETTING_FLG_VALID = "1"; // 非表示
		public const string FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN3_DEFAULT_SETTING_FLG_INVALID = "0"; // 表示

		// 店舗配送種別：次回配送日変更可能日数のデフォルト値
		public const int FLG_SHOPSHIPPING_NEXT_SHIPPING_MAX_CHANGE_DAYS_DEFAULT = 15;

		// 店舗配送種別：配送料無料購入金額設定フラグ
		public const string FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_VALID = "1";	// 有効
		public const string FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_INVALID = "0";	// 無効

		// 店舗配送料：送料無料案内表示フラグ
		public const string FLG_SHOPSHIPPING_ANNOUNCE_FREE_SHIPPING_FLG_VALID = "1";   // 有効
		public const string FLG_SHOPSHIPPING_ANNOUNCE_FREE_SHIPPING_FLG_INVALID = "0"; // 無効

		// 店舗配送種別：計算方法設定フラグ
		public const string FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_SUM_OF_PRODUCT_SHIPPING_PRICE = "1"; // 商品１つ毎に送料を設定
		public const string FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_HIGHEST_SHIPPING_PRICE_PLUS_PLURAL_PRICE = "2";	// 最も高い送料１点＋x円／個の送料を設定

		// 店舗配送種別：削除フラグ
		public const string FLG_SHOPSHIPPING_DELFLG_UNDELETED = "0";	// 通常
		public const string FLG_SHOPSHIPPING_DELFLG_DELETED = "1";	// 削除済み

		// 店舗配送種別：のし利用フラグ
		public const string FLG_SHOPSHIPPING_WRAPPING_PAPER_FLG_VALID = "1";	// 有効
		public const string FLG_SHOPSHIPPING_WRAPPING_PAPER_FLG_INVALID = "0";	// 無効

		// 店舗配送種別：包装利用フラグ
		public const string FLG_SHOPSHIPPING_WRAPPING_BAG_FLG_VALID = "1";		// 有効
		public const string FLG_SHOPSHIPPING_WRAPPING_BAG_FLG_INVALID = "0";	// 無効

		// 店舗配送種別：決済選択の任意利用フラグ
		public const string FLG_SHOPSHIPPING_PAYMENT_SELECTION_FLG_VALID = "1"; // 有効
		public const string FLG_SHOPSHIPPING_PAYMENT_SELECTION_FLG_INVALID = "0"; // 無効

		// 店舗配送種別：配送料の別途見積もり利用フラグ
		public const string FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_VALID = "1"; // 有効
		public const string FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_INVALID = "0"; // 無効

		// 店舗配送種別：定期配送料無料フラグ
		public const string FLG_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG_VALID = "1";	// 送料無料
		public const string FLG_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG_INVALID = "0";	// 送料あり

		// 別出荷フラグ
		public const string FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_VALID = "1";		// 有効
		public const string FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_INVALID = "0";	// 無効
		public const string FLG_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG = "2";	// Flag shipping receiving store
		/// <summary>Flag shipping delivery to delivery address</summary>
		public const string FLG_ORDERSHIPPING_SHIPPING_TO_DELIVERY_ADDRESS = "3";
		/// <summary>Flag shipping store pick up</summary>
		public const string FLG_ORDERSHIPPING_SHIPPING_STORE_PICKUP_FLG = "4";

		//配送種別
		/// <summary>デフォルト配送拠点ID（01：小金井）</summary>
		public const string FLG_SHOPSHIPPING_SHIPPING_BASE_ID_DEFAULT = "01";

		// 配送種別配送会社：配送区分
		public const string FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS = "EXPRESS";	// 宅配便
		public const string FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL = "MAIL";	// メール便

		// 配送種別配送会社：初期配送会社
		public const string FLG_SHOPSHIPPINGSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY_VALID = "1";	// 有効
		public const string FLG_SHOPSHIPPINGSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY_INVALID = "0";	// 無効

		// 店舗配送料地帯：配送不可エリアフラグ
		public const string FLG_SHOPSHIPPINGZONE_UNAVAILABLE_SHIPPING_AREA_VALID = "1";	//有効
		public const string FLG_SHOPSHIPPINGZONE_UNAVAILABLE_SHIPPING_AREA_INVALID = "0";	//無効

		// 配送会社：出荷連携配送会社
		/// <summary>ヤマト運輸・クロネコメール便</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_YAMATO = "YAMATO";
		/// <summary>佐川急便</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_SAGAWA = "SAGAWA";
		/// <summary>西濃運輸</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_SEINO = "SEINO";
		/// <summary>郵便書留・特定記録郵便</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_YUSEI = "YUSEI";
		/// <summary>ゆうパック・エクスパック・ポスパケット</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_YUPAKE = "YUPAKE";
		/// <summary>福山通運</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_FUKUYAMA = "FUKUYAMA";
		/// <summary>エコ配</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_ECO = "ECO";
		/// <summary>翌朝10時便・レターパック・新特急郵便</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_TOKYU = "YOKUASA";
		/// <summary>その他</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_OTHER = "OTHER";
		/// <summary>日本通運</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_NITTSU = "NITTSU";
		/// <summary>新潟運輸</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_NIIGATA = "NIIGATA";
		/// <summary>名鉄運輸</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_MEITETSU = "MEITETSU";
		/// <summary>信州名鉄運輸</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_SHINMEI = "SHINMEI";
		/// <summary>トールエクスプレス</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_TOLLEX = "TOLLEX";
		/// <summary>トナミ運輸</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_TONAMI = "TONAMI";
		/// <summary>セイノースーパーエクスプレス</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_SEINOEX = "SEINOEX";
		/// <summary>大川配送サービス</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_OKAWA = "OKAWA";
		/// <summary>プラスサービス</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_PLUS = "PLUS";
		/// <summary>エスラインギフ</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_ESURAIN = "SLINE";
		/// <summary>ハートランス</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_HARTRANSU = "HARTRANS";
		/// <summary>Delivery company type: post np payment default</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_POST_NP_PAYMENT_DEFAULT = "50000";

		// 配送会社：配送希望時間帯設定可能フラグ
		public const string FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_VALID = "1";	// 有効
		public const string FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_INVALID = "0";	// 無効
		public const string FLG_DELIVERYCOMPANY_LEAD_TIME_SET_FLG_VALID = "1";	// 有効
		public const string FLG_DELIVERYCOMPANY_LEAD_TIME_SET_FLG_INVALID = "0";	// 無効
		public const int FLG_SHIPPING_LEAD_TIME_DEFAULT = 2;	// 基本配送リードタイム

		// 決済種別：決済種別ID
		public const string FLG_PAYMENT_PAYMENT_ID_CREDIT = "K10";		// クレジット
		public const string FLG_PAYMENT_PAYMENT_ID_COLLECT = "K20";		// 代金引換
		//public const string FLG_PAYMENT_PAYMENT_ID_CVS = "K30";		// コンビニ
		public const string FLG_PAYMENT_PAYMENT_ID_CVS_PRE = "K31";		// コンビニ（前払い）
		public const string FLG_PAYMENT_PAYMENT_ID_CVS_DEF = "K32";		// コンビニ（後払い）
		public const string FLG_PAYMENT_PAYMENT_ID_PAIDY = "K33";		// Paidy翌月払い(コンビニ/銀行)
		public const string FLG_PAYMENT_PAYMENT_ID_SMS_DEF = "K37";		// ヤマト後払いSMS認証連携
		public const string FLG_PAYMENT_PAYMENT_ID_GMOATOKARA = "K40";	// GMOアトカラ
		public const string FLG_PAYMENT_PAYMENT_ID_BANK_PRE = "K41";	// 銀行振込（前払い）
		public const string FLG_PAYMENT_PAYMENT_ID_BANK_DEF = "K42";	// 銀行振込（後払い）
		/// <summary>Banknet</summary>
		public const string FLG_PAYMENT_PAYMENT_ID_BANKNET = "K43";
		/// <summary>ATM決済</summary>
		public const string FLG_PAYMENT_PAYMENT_ID_ATM = "K44";
		public const string FLG_PAYMENT_PAYMENT_ID_POST_PRE = "K51";	// 郵便振込（前払い）
		public const string FLG_PAYMENT_PAYMENT_ID_POST_DEF = "K52";	// 郵便振込（後払い）
		public const string FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_ORG = "K60";	// ドコモケータイ払い
		public const string FLG_PAYMENT_PAYMENT_ID_SMATOMETE_ORG = "K61";	// S!まとめて支払い
		public const string FLG_PAYMENT_PAYMENT_ID_AUMATOMETE_ORG = "K62";	// まとめてau支払い
		public const string FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS = "K63";	// ソフトバンク・ワイモバイルまとめて支払い(SBPS)
		public const string FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS = "K64";		// auかんたん決済(SBPS)
		public const string FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS = "K65";	// ドコモケータイ払い(SBPS)
		public const string FLG_PAYMENT_PAYMENT_ID_SMATOMETE_SBPS = "K66";	// S!まとめて支払い(SBPS)
		public const string FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS = "K67";	// リクルートかんたん支払い(SBPS)
		public const string FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS = "K68";	// PayPal(SBPS)
		public const string FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS = "K69";	// 楽天ペイ(SBPS)
		/// <summary>キャリア決済（Boku)</summary>
		public const string FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU = "K70";
		public const string FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT = "K80";	// Amazon Pay
		public const string FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2 = "K81";	// Amazon Pay Cv2
		public const string FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY = "G31";	// 後付款(TriLink後払い)
		public const string FLG_PAYMENT_PAYMENT_ID_NOPAYMENT = "K90";	// 決済なし
		public const string FLG_PAYMENT_PAYMENT_ID_PAYPAL = "G15";	// PayPal
		public const string FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE = "G33";	// コンビニエンス決済
		public const string FLG_PAYMENT_PAYMENT_ID_LINEPAY = "G16";	// LINE Pay翌月払い
		public const string FLG_PAYMENT_PAYMENT_ID_ATONE = "K34";	// atone翌月払い
		public const string FLG_PAYMENT_PAYMENT_ID_AFTEE = "K35";	// aftee翌月払い
		public const string FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY = "K36";	// NP後払い
		public const string FLG_PAYMENT_PAYMENT_ID_ECPAY = "G34";	// Ec Pay
		public const string FLG_PAYMENT_PAYMENT_ID_DSK_DEF = "K91";	// DSKコンビニ（後払い）
		public const string FLG_PAYMENT_PAYMENT_ID_NEWEBPAY = "G35";	// Neweb Pay
		public const string FLG_PAYMENT_PAYMENT_ID_PAYPAY = "K82";	// PayPay
		public const string FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO = "K38";	// GMO: Pay as you go
		public const string FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE = "K39";	// GMO: Frame Guarantee

		// 決済手数料区分
		public const string FLG_PAYMENT_PAYMENT_PRICE_KBN_SINGULAR = "0";	// 金額によって決済手数料を分けない
		public const string FLG_PAYMENT_PAYMENT_PRICE_KBN_PLURAL = "1";	// 金額によって決済手数料を分ける

		// モバイル表示フラグ
		/// <summary>PC、モバイルどちらも表示</summary>
		public const string FLG_PAYMENT_MOBILE_DISP_FLG_BOTH_PC_AND_MOBILE = "0";
		/// <summary>PCのみ表示</summary>
		public const string FLG_PAYMENT_MOBILE_DISP_FLG_PC = "1";
		/// <summary>モバイルのみ表示</summary>
		public const string FLG_PAYMENT_MOBILE_DISP_FLG_MOBILE = "2";

		// 決済種別：有効フラグ
		public const string FLG_PAYMENT_VALID_FLG_VALID = "1";	// 有効
		public const string FLG_PAYMENT_INVALID_FLG_VALID = "0";	// 無効

		// モバイル機種マスタ：キャリアID
		public const string FLG_MOBILEMODEL_CAREER_ID_DOCOMO = "01dc";
		public const string FLG_MOBILEMODEL_CAREER_ID_AU = "02au";
		public const string FLG_MOBILEMODEL_CAREER_ID_SOFTBANK = "03vd";
		public const string FLG_MOBILEMODEL_CAREER_ID_WILLCOM = "04wc";
		public const string FLG_MOBILEMODEL_CAREER_ID_DEFAULT = "";

		// モバイル機種マスタ：機種コード
		public const string FLG_MOBILEMODEL_MODEL_CODE_DEFAULT = "";

		// モバイル機種マスタ：各種画像表示フラグ
		public const string FLG_MOBILEMODEL_DISP_JPEG_FLG_ON = "1";
		public const string FLG_MOBILEMODEL_DISP_JPEG_FLG_OFF = "0";
		public const string FLG_MOBILEMODEL_DISP_GIF_FLG_ON = "1";
		public const string FLG_MOBILEMODEL_DISP_GIF_FLG_OFF = "0";
		public const string FLG_MOBILEMODEL_DISP_PNG_FLG_ON = "1";
		public const string FLG_MOBILEMODEL_DISP_PNG_FLG_OFF = "0";
		public const string FLG_MOBILEMODEL_DISP_BMP2_FLG_ON = "1";
		public const string FLG_MOBILEMODEL_DISP_BMP2_FLG_OFF = "0";
		public const string FLG_MOBILEMODEL_DISP_BMP4_FLG_ON = "1";
		public const string FLG_MOBILEMODEL_DISP_BMP4_FLG_OFF = "0";
		public const string FLG_MOBILEMODEL_DISP_MNG_FLG_ON = "1";
		public const string FLG_MOBILEMODEL_DISP_MNG_FLG_OFF = "0";

		// ユーザモバイル登録情報：登録フラグ
		public const string FLG_USERMOBILEREGINFO_REG_FLG_ON = "1";					// 登録済
		public const string FLG_USERMOBILEREGINFO_REG_FLG_OFF = "0";				// 未登録

		// 定期購入情報：定期購入区分
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE = "01";
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY = "02";
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS = "03";
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY = "04";

		// 定期購入情報：定期購入設定（最終日）
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1_DAY_LASTDAY = "-1";

		//定期購入パラメータ
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PARAMETER_NORMAL = "NORMAL";
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PARAMETER_FAILED = "FAILED";
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PARAMETER_NOSTOCK = "NOSTOCK";
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PARAMETER_SUSPEND = "SUSPEND";
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PARAMETER_COMPLETE = "COMPLETE";
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PARAMETER_CANCEL = "CANCEL";

		// 定期購入情報：定期購入ステータス
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_TEMP = "01";
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL = "10";
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED = "11";
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NOSTOCK = "12";
		/// <summary> 定期購入ステータス(完了) </summary>
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_COMPLETE= "14";
		/// <summary>配送不可エリア停止</summary>
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_UNAVAILABLE_SHIPPING_AREA = "15";
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_FAILED = "19";
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_SUSPEND = "20";	// 休止
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL = "30";
		public const string FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL_TEMP = "31";

		// 定期購入情報：決済ステータス
		public const string FLG_FIXEDPURCHASE_PAYMENT_STATUS_NORMAL = "10";	// 通常
		public const string FLG_FIXEDPURCHASE_PAYMENT_STATUS_ERROR = "11";	// エラー

		// 定期購入情報：注文区分
		public const string FLG_FIXEDPURCHASE_ORDER_KBN_PC = "PC";		// PC注文
		public const string FLG_FIXEDPURCHASE_ORDER_KBN_MOBILE = "MB";	// モバイル注文
		public const string FLG_FIXEDPURCHASE_ORDER_KBN_SMARTPHONE = "SP";	// スマートフォン注文
		public const string FLG_FIXEDPURCHASE_ORDER_KBN_TEL = "TEL";	// 電話注文
		public const string FLG_FIXEDPURCHASE_ORDER_KBN_FAX = "FAX";	// FAX注文

		// 定期購入情報：有効フラグ
		public const string FLG_FIXEDPURCHASE_VALID_FLG_VALID = "1";	// 有効
		public const string FLG_FIXEDPURCHASE_VALID_FLG_INVALID = "0";	// 無効

		// 定期購入情報：拡張ステータス1～20
		public const string FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_ON = "1";	// ON
		public const string FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF = "0";	// OFF

		// 定期購入情報：次回購入の利用ポイントの全適用フラグ
		public const string FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_ON = "1"; // ON
		public const string FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_OFF = "0"; // OFF

		// 定期購入履歴：定期購入
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_TEMP = "01";	// 仮登録
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_REGISTER_SUCCESS = "02";	// 本登録
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS = "10";	// 購入成功
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_PAYMENT = "11";	// 購入失敗（決済エラー）
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_NOSTOCK = "12";	// 購入失敗（在庫切れ）
		/// <summary> 定期購入履歴：完了 </summary>
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_COMPLETE = "14";
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED_UNAVAILABLE_SHIPPING_AREA = "15";	// 購入失敗（配送不可エリア）
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_FAILED = "19";	// 購入失敗（その他エラー）
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_AUTH_SUCCESS = "20";	// 与信成功・クレジットカード情報登録
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_AUTH_FAILED = "21";	// 与信失敗
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ORDER_CANCEL = "22";	// 注文キャンセル
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ORDER_SHIPPED = "23";	// 注文出荷完了
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ORDER_RETURN = "24";	// 注文全返品
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_CANCEL = "30";	// 購入キャンセル
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_RESUME = "31";	// 定期再開
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_SUSPEND = "32";	// 購入休止
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_SHIPPINGUPDATE = "80";	// 配送先の更新
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_PATTERNUPDATE = "81";	// 配送パターンの更新
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_SKIPSHIPPINGDATE = "82";	// 次回配送スキップ
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_EXTENDSTATUS_UPDATE = "83";	// 拡張ステータスの更新
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ORDER_COUNT_UPDATE = "84";	// 購入回数(注文基準)の更新
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_SHIPPED_COUNT_UPDATE = "85";	// 購入回数(出荷基準)の更新
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ITEMUPDATE = "86";	// 商品の変更
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_CANCELREASON_UPDATE = "87";	// 解約理由の更新
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ORDERPAYMENT_UPDATE = "88";	// 支払方法の更新
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_NEXTSHIPINGUSEPOINT_UPDATE = "89";	// 次回定期購入の利用ポイント数の更新
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_BUY_SUCCESS_FOR_COMBINE = "91";	// 注文登録(注文同梱)
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ORDER_CANCEL_FOR_COMBINE = "92";	// 注文キャンセル(注文同梱)
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ITEMUPDATE_FOR_COMBINE = "93";	// 商品の変更(注文同梱)
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_CANCEL_FOR_COMBINE = "94";	// 定期キャンセル(注文同梱)
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_CANCEL_TEMP = "95";	// 仮登録キャンセル
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_SHIPING_DATE_UPDATE = "96";	// 次回配送日/次々回配送日更新
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_SUSPEND_REASON_UPDATE = "97";	// 休止理由の更新
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_CREDIT_REGISTER_FAIL = "100";	//クレジット登録失敗
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_NEXTSHIPPINGUSECOUPON_UPDATE = "101";	// 次回定期購入の利用クーポンの更新
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_RECEIPT_UPDATE = "102";	// 領収書情報の更新
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_CONTINUOUS_ORDER_REGISTER = "103";	// 継続課金（定期・従量）申込
		public const string FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_CONTINUOUS_ORDER_CANCEL = "104";	// 継続課金（定期・従量）解約

		// 定期購入バッチメールの一時ログ:メールタイプ
		public const string FLG_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_TYPE_ORDER = "order";         //注文完了メール
		public const string FLG_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_TYPE_ERROR = "error";         //注文エラーメール
		public const string FLG_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_TYPE_DEADLINE = "deadline";   //変更期限メール

		// 定期継続分析：ステータス
		public const string FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_EXISTS = "00";	// 定期あり
		public const string FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_ORDER = "10";	// 注文
		public const string FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_DELIVERED = "20";	// 配達
		public const string FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_FALLOUT = "90";  // 離脱

		/// <summary>定期商品変更設定：有効</summary>
		public const string FLG_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID = "1";
		/// <summary>定期商品変更設定：無効</summary>
		public const string FLG_FIXEDPURCHASEPRODUCTCHANGESETTING_INVALID = "0";

		/// <summary>定期変更元商品単位種別:商品単位</summary>
		public const string FLG_FIXEDPURCHASEBEFOREPRODUCTCHANGEITEM_ITEM_UNIT_TYPE_PRODUCT = "PRODUCT";
		/// <summary>定期変更元商品単位種別:バリエーション単位</summary>
		public const string FLG_FIXEDPURCHASEBEFOREPRODUCTCHANGEITEM_ITEM_UNIT_TYPE_VARIATION = "VARIATION";

		/// <summary>定期変更後商品単位種別:商品単位</summary>
		public const string FLG_FIXEDPURCHASEAFTERPRODUCTCHANGEITEM_ITEM_UNIT_TYPE_PRODUCT = "PRODUCT";
		/// <summary>定期変更後商品単位種別:バリエーション単位</summary>
		public const string FLG_FIXEDPURCHASEAFTERPRODUCTCHANGEITEM_ITEM_UNIT_TYPE_VARIATION = "VARIATION";

		// 入荷通知メール情報：PCモバイル区分
		public const string FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_PC = "PC";		// PC
		public const string FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_MOBILE = "MB";	// モバイル

		// 入荷通知メール情報：入荷通知メール区分
		public const string FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_ARRIVAL = "ARRIVAL";	// 再入荷通知
		public const string FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RELEASE = "RELEASE";	// 販売開始通知
		public const string FLG_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN_RESALE = "RESALE";		// 再販売通知

		// 入荷通知メール情報：送信ステータス
		public const string FLG_USERPRODUCTARRIVALMAIL_MAIL_SEND_STATUS_UNSENT = "0";	// 未送信
		public const string FLG_USERPRODUCTARRIVALMAIL_MAIL_SEND_STATUS_SENDING = "1";	// 処理中
		public const string FLG_USERPRODUCTARRIVALMAIL_MAIL_SEND_STATUS_SENT = "2";		// 送信済み
		public const string FLG_USERPRODUCTARRIVALMAIL_MAIL_SEND_STATUS_NOTSEND = "9";	// 送信なし処理済み

		//在庫アラートメール送信フラグ
		public const string FLG_FAVORITE_STOCK_ALERT_MAIL_SEND_FLG_SENT = "1";	// 送信済み
		public const string FLG_FAVORITE_STOCK_ALERT_MAIL_SEND_FLG_UNSENT = "0";	// 未送信

		// 入荷通知メール情報：ゲスト入荷通知登録のユーザーID
		public const string FLG_USERPRODUCTARRIVALMAIL_USER_ID_GUEST = "GUEST";

		// モール連携設定：受信POPポート
		public const int FLG_MALLCOOPERATIONSETTING_POP_PORT_DEFAULT = 110;	// デフォルト

		// モール連携設定：受信APOPフラグ
		public const string FLG_MALLCOOPERATIONSETTING_POP_APOP_FLG_APOP = "0";	// APOP
		public const string FLG_MALLCOOPERATIONSETTING_POP_APOP_FLG_POP = "1";	// POP

		// モール連携設定：モール区分
		public const string FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN = "R";	// 楽天
		public const string FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO = "Y";	// ヤフー
		public const string FLG_MALLCOOPERATIONSETTING_MALL_KBN_AMAZON = "A";	// Amazon
		public const string FLG_MALLCOOPERATIONSETTING_MALL_KBN_ANDMALL = "M";	// &mall
		public const string FLG_MALLCOOPERATIONSETTING_MALL_KBN_LOHACO = "L";	// Lohaco
		/// <summary>モール連携設定：モール区分 ネクストエンジン</summary>
		public const string FLG_MALLCOOPERATIONSETTING_MALL_KBN_NEXT_ENGINE = "NE";
		/// <summary>モール連携設定：Facebook</summary>
		public const string FLG_MALLCOOPERATIONSETTING_MALL_KBN_FACEBOOK = "FB";
		// モール連携設定：有効フラグ
		public const string FLG_MALLCOOPERATIONSETTING_VALID_FLG_VALID = "1";	// 有効
		public const string FLG_MALLCOOPERATIONSETTING_VALID_FLG_INVALID = "0";	// 無効

		// モール連携設定：在庫連携利用フラグ
		public const string FLG_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG_VALID = "1";	// 有効
		public const string FLG_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG_INVALID = "0";	// 無効

		// モール連携設定：出品設定
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_1 = "EXH1";	// 出品設定1
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_2 = "EXH2";	// 出品設定2
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_3 = "EXH3";	// 出品設定3
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_4 = "EXH4";	// 出品設定4
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_5 = "EXH5";	// 出品設定5
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_6 = "EXH6";	// 出品設定6
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_7 = "EXH7";	// 出品設定7
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_8 = "EXH8";	// 出品設定8
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_9 = "EXH9";	// 出品設定9
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_10 = "EXH10";	// 出品設定10
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_11 = "EXH11";	// 出品設定11
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_12 = "EXH12";	// 出品設定12
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_13 = "EXH13";	// 出品設定13
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_14 = "EXH14";	// 出品設定14
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_15 = "EXH15";	// 出品設定15
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_16 = "EXH16";	// 出品設定16
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_17 = "EXH17";	// 出品設定17
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_18 = "EXH18";	// 出品設定18
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_19 = "EXH19";	// 出品設定19
		public const string FLG_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG_20 = "EXH20";	// 出品設定20

		// モール連携更新ログ：マスタ区分
		public const string FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCT = "Product";
		public const string FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCTVARIATION = "ProductVariation";
		public const string FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCTSTOCK = "ProductStock";

		// モール連携更新ログ：アクション区分
		public const string FLG_MALLCOOPERATIONUPDATELOG_ACTION_KBN_INSERT = "I";
		public const string FLG_MALLCOOPERATIONUPDATELOG_ACTION_KBN_UPDATE = "U";
		public const string FLG_MALLCOOPERATIONUPDATELOG_ACTION_KBN_DELETE = "D";

		// モール連携更新ログ：アクションステータス
		public const string FLG_MALLCOOPERATIONUPDATELOG_ACTION_STATUS_INITIAL = "00";
		public const string FLG_MALLCOOPERATIONUPDATELOG_ACTION_STATUS_ACTIVE = "10";
		public const string FLG_MALLCOOPERATIONUPDATELOG_ACTION_STATUS_COMPLETE = "20";
		public const string FLG_MALLCOOPERATIONUPDATELOG_ACTION_STATUS_EXCEPT = "99";

		// モール出品設定：連携FLG
		public const string FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF = "0"; // 連携しない
		public const string FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON = "1"; // 連携する

		/// <summary>最大出品モール数</summary>
		public const int CONST_MALLEXHIBITS_COUNT = 20;
		/// <summary>出品フラグ</summary>
		public const string CONST_MALLEXHIBITSCONFIG_EXHIBITS_FLG = "exhibits_flg";

		// モール監視ログ：バッチID
		public const string FLG_MALLWATCHINGLOG_BATCH_ID_MAILORDERGETTER = "MAILORDERGETTER"; // 注文メール取込
		public const string FLG_MALLWATCHINGLOG_BATCH_ID_RAKUTEN_API_ORDER_COOP = "RAKUTEN_API_ORDER_COOP"; // 楽天API受注連携
		public const string FLG_MALLWATCHINGLOG_BATCH_ID_MKADV = "MKADV"; // 商品コンバータ
		public const string FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE = "STOCKUPDATE"; // 商品・在庫アップロード
		public const string FLG_MALLWATCHINGLOG_BATCH_ID_MALLORDERIMPORTER = "MALLORDERIMPORTER"; // モール注文取込
		public const string FLG_MALLWATCHINGLOG_BATCH_ID_EXTERNALFILEIMPORT = "EXTERNALFILEIMPORT"; // 外部ファイル取込
		public const string FLG_MALLWATCHINGLOG_BATCH_ID_LIAISEAMAZONMALL = "LIAISEAMAZONMALL";	// Amazonモール連携
		public const string FLG_MALLWATCHINGLOG_BATCH_ID_ANDMALL_STOCK_API = "ANDMALL_STOCK_API";	// ＆mall在庫引当API
		public const string FLG_MALLWATCHINGLOG_BATCH_ID_LIAISE_LOHACO_MALL = "LIAISE_LOHACO_MALL"; // Lohacoモール連携
		/// <summary>ネクストエンジン連携</summary>
		public const string FLG_MALLWATCHINGLOG_BATCH_ID_NEXT_ENGINE_API = "NEXT_ENGINE";
		/// <summary>Facebookモール連携</summary>
		public const string FLG_MALLWATCHINGLOG_BATCH_ID_LIAISE_FACEBOOK_MALL = "LIAISE_FACEBOOK_MALL";

		// モール監視ログ：ログ区分
		public const string FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS = "SUCCESS"; // 成功
		public const string FLG_MALLWATCHINGLOG_LOG_KBN_ERROR = "ERROR"; // 異常
		public const string FLG_MALLWATCHINGLOG_LOG_KBN_WARNING = "WARNING"; // 警告

		// 広告先マスタ：セパレートタイプ
		public const int FLG_MALLPRDCNVSEPARATERTYPE_COMMA = 0;
		public const int FLG_MALLPRDCNVSEPARATERTYPE_TAB = 1;

		// 広告先マスタ：文字コードタイプ
		public const string FLG_MALLPRDCNVCHARACTERCODETYPE_SHIFT = "Shift_JIS";
		public const string FLG_MALLPRDCNVCHARACTERCODETYPE_UTF = "UTF-8";

		// 広告先マスタ：文字コードタイプ
		public const string FLG_MALLPRDCNV_NEWLINETYPE_CR = "CR";
		public const string FLG_MALLPRDCNV_NEWLINETYPE_LF = "LF";
		public const string FLG_MALLPRDCNV_NEWLINETYPE_CRLF = "CRLF";

		// プレビューマスタ：プレビュー区分
		public const string FLG_PREVIEW_PREVIEW_KBN_PRODUCT_DETAIL = "PRODUCT_DETAIL";	// 商品詳細プレビュー
		public const string FLG_PREVIEW_PREVIEW_KBN_COORDINATE_DETAIL = "COORDINATE_DETAIL";	// コーディネート詳細プレビュー
		public const string FLG_PREVIEW_PREVIEW_KBN_FEATURE_AREA = "FEATURE_AREA";	// 特集エリアプレビュー
		public const string FLG_PREVIEW_PREVIEW_KBN_FEATURE_AREA_BANNER = "FEATURE_AREA_BANNER";	// 特集エリアバナープレビュー

		// 新着情報：本文区分
		public const string FLG_NEWS_NEWS_TEXT_KBN_TEXT = "0";	// TEXT
		public const string FLG_NEWS_NEWS_TEXT_KBN_HTML = "1";	// HTML

		// 新着情報：モバイル本文区分
		public const string FLG_NEWS_NEWS_TEXT_KBN_MOBILE_TEXT = "0";	// TEXT
		public const string FLG_NEWS_NEWS_TEXT_KBN_MOBILE_HTML = "1";	// HTML

		// 新着情報：表示フラグ
		public const string FLG_NEWS_DISP_FLG_ALL = "0";	// 新着一覧、TOPページどちらも表示
		public const string FLG_NEWS_DISP_FLG_LIST = "1";	// 新着一覧のみ表示

		// 新着情報：モバイル表示フラグ
		public const string FLG_NEWS_MOBILE_DISP_FLG_ALL = "0";	// ＰＣ・モバイル
		public const string FLG_NEWS_MOBILE_DISP_FLG_PC = "1";	// ＰＣのみ
		public const string FLG_NEWS_MOBILE_DISP_FLG_MOBLE = "2";	// モバイルのみ

		// 新着情報：有効フラグ
		public const string FLG_NEWS_VALID_FLG_VALID = "1";	// 有効
		public const string FLG_NEWS_VALID_FLG_INVALID = "0";	// 無効

		// News：Del flag
		public const string FLG_NEWS_DEL_FLG_OFF = "0";			// OFF

		// News：Display order
		public const int FLG_NEWS_DISPLAY_ORDER_DEFAULT = 1;	// Default

		// 商品一覧表示設定：表示／非表示設定
		/// <summary>商品一覧表示設定：表示／非表示設定：表示</summary>
		public const string FLG_PRODUCTLISTDISPSETTING_DISP_ENABLE_ON = "ON";
		/// <summary>商品一覧表示設定：表示／非表示設定：非表示</summary>
		public const string FLG_PRODUCTLISTDISPSETTING_DISP_ENABLE_OFF = "OFF";

		// 商品一覧表示設定：デフォルト表示設定
		/// <summary>商品一覧表示設定：デフォルト表示設定：デフォルト</summary>
		public const string FLG_PRODUCTLISTDISPSETTING_DEFAULT_DISP_FLG_ON = "ON";
		/// <summary>商品一覧表示設定：デフォルト表示設定：非デフォルト</summary>
		public const string FLG_PRODUCTLISTDISPSETTING_DEFAULT_DISP_FLG_OFF = "OFF";

		/// <summary>商品一覧表示設定：設定区分：ソート形式</summary>
		public const string FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_SORT = "SORT";
		/// <summary>商品一覧表示設定：設定区分：件数</summary>
		public const string FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_COUNT = "COUNT";
		/// <summary>商品一覧表示設定：設定区分：表示形式</summary>
		public const string FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_IMG = "IMG";
		/// <summary>商品一覧表示設定：設定区分：在庫区分</summary>
		public const string FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_STOCK = "STOCK";

		// ユーザークレジットカードマスタ：表示フラグ
		public const string FLG_USERCREDITCARD_DISP_FLG_ON = "1";    // 表示
		public const string FLG_USERCREDITCARD_DISP_FLG_OFF = "0";   // 非表示

		// ユーザークレジットカードマスタ：登録アクション区分
		public const string FLG_USERCREDITCARD_REGISTER_ACTION_KBN_NORMAL = "";    // 通常
		public const string FLG_USERCREDITCARD_REGISTER_ACTION_KBN_ORDER_REGISTER = "ODR_REG";    // 注文登録
		public const string FLG_USERCREDITCARD_REGISTER_ACTION_KBN_ORDER_MODIFY = "ODR_MDF";    // 注文変更
		public const string FLG_USERCREDITCARD_REGISTER_ACTION_KBN_ORDER_RETURN_EXCHANGE = "ODR_REX";    // 注文返品交換
		public const string FLG_USERCREDITCARD_REGISTER_ACTION_KBN_FIXEDPURCHASE_MODIFY = "FXP_MDF";    // 定期変更
		public const string FLG_USERCREDITCARD_REGISTER_ACTION_KBN_USERCREDITCARD_REGISTER = "UCC_REG";    // ユーザークレカ登録

		// ユーザークレジットカードマスタ：登録ステータス
		public const string FLG_USERCREDITCARD_REGISTER_STATUS_NORMAL = "";    // 通常
		public const string FLG_USERCREDITCARD_REGISTER_STATUS_UNREGISTERED = "UNRGST";   // 未登録（登録待ち）
		public const string FLG_USERCREDITCARD_REGISTER_STATUS_UNAUTHED = "UNAUTH";   // 未与信（与信待ち）
		public const string FLG_USERCREDITCARD_REGISTER_STATUS_AUTHERROR = "AUTHERR";   // 与信エラー

		// ユーザークレジットカードマスタ：連携種別
		public const string FLG_USERCREDITCARD_COOPERATION_TYPE_CREDITCARD = "CREDIT";    // クレジットカード
		public const string FLG_USERCREDITCARD_COOPERATION_TYPE_PAYPAL = "PAYPAL";    // PayPal
		public const string FLG_USERCREDITCARD_COOPERATION_TYPE_PAIDY = "PAIDY";	// Paidy
		public const string FLG_USERCREDITCARD_COOPERATION_TYPE_ECPAY = "ECPAY";	// EcPay
		public const string FLG_USERCREDITCARD_COOPERATION_TYPE_NEWEBPAY = "NEWEBPAY";	// NewebPay
		public const string FLG_USERCREDITCARD_COOPERATION_TYPE_YAMATOKASMS = "YAMATOSMS";  // ヤマト後払いSMS認証決済
		/// <summary>連携種別：PAYPAY</summary>
		public const string FLG_USERCREDITCARD_COOPERATION_TYPE_PAYPAY = "PAYPAY";

		// ユーザークレジットカードマスタ：クレジットカード会社コード
		/// <summary>クレジットカード会社コード：VISA</summary>
		public const string FLG_USERCREDITCARD_VISA = "9";
		/// <summary>クレジットカード会社コード：MASTER</summary>
		public const string FLG_USERCREDITCARD_MASTER = "10";
		/// <summary>クレジットカード会社コード：JCB</summary>
		public const string FLG_USERCREDITCARD_JCB = "3";
		/// <summary>クレジットカード会社コード：AMEX</summary>
		public const string FLG_USERCREDITCARD_AMEX = "12";
		/// <summary>クレジットカード会社コード：DINERS</summary>
		public const string FLG_USERCREDITCARD_DINERS = "2";

		// 注文メモ情報：表示区分
		public const string FLG_ORDER_MEMO_SETTING_DISP_FLG_PC = "PC";	// ＰＣ
		public const string FLG_ORDER_MEMO_SETTING_DISP_FLG_MOBILE = "MB";	// モバイル

		// 注文メモ情報：有効フラグ
		public const string FLG_ORDER_MEMO_SETTING_VALID_FLG_VALID = "1";	// 有効
		public const string FLG_ORDER_MEMO_SETTING_VALID_FLG_INVALID = "0";	// 無効

		// 外部レコメンド連携更新ログ：マスタ区分
		public const string FLG_RECOMMENDCOOPUPDATELOG_MASTER_KBN_PRODUCT = "Product";					// 商品マスタ
		public const string FLG_RECOMMENDCOOPUPDATELOG_MASTER_KBN_PRODUCTCATEGORY = "ProductCategory";	// 商品カテゴリマスタ

		// 外部レコメンド連携更新ログ：処理ステータス
		public const string FLG_RECOMMENDCOOPUPDATELOG_ACTION_STATUS_READY = "READY";		// 連携準備中
		public const string FLG_RECOMMENDCOOPUPDATELOG_ACTION_STATUS_ACTIVE = "ACTIVE";		// 連携実行中
		public const string FLG_RECOMMENDCOOPUPDATELOG_ACTION_STATUS_DONE = "DONE";			// 連携済
		public const string FLG_RECOMMENDCOOPUPDATELOG_ACTION_STATUS_ERROR = "ERROR";		// 連携エラー

		// シリアルキー：状態
		public const string FLG_SERIALKEY_STATUS_NOT_RESERVED = "NOTRESERVED"; // 未引当
		public const string FLG_SERIALKEY_STATUS_RESERVED = "RESERVED"; // 予約済（引当済）
		public const string FLG_SERIALKEY_STATUS_DELIVERED = "DELIVERED"; // 引渡済
		public const string FLG_SERIALKEY_STATUS_CANCELLED = "CANCELLED"; //キャンセル済

		// シリアルキー：有効フラグ
		public const string FLG_SERIALKEY_VALID_FLG_INVALID = "0"; // 無効
		public const string FLG_SERIALKEY_VALID_FLG_VALID = "1"; // 有効

		// ノベルティ：有効フラグ
		public const string FLG_NOVELTY_VALID_FLG_INVALID = "0";				// 無効
		public const string FLG_NOVELTY_VALID_FLG_VALID = "1";					// 有効

		// ノベルティ：ノベルティ対象アイテム種別		
		public const string FLG_NOVELTY_TARGET_ITEM_TYPE_ALL = "ALL";				// 全商品
		public const string FLG_NOVELTY_TARGET_ITEM_TYPE_BRAND = "BRAND";			// ブランドID指定
		public const string FLG_NOVELTY_TARGET_ITEM_TYPE_CATEGORY = "CATEGORY";		// カテゴリーID指定
		public const string FLG_NOVELTY_TARGET_ITEM_TYPE_PRODUCT = "PRODUCT";		// 商品ID指定
		public const string FLG_NOVELTY_TARGET_ITEM_TYPE_VARIATION = "VARIATION";	// 商品バリエーションID指定

		// ノベルティ：対象会員
		public const string FLG_NOVELTYGRANTCONDITIONS_USER_RANK_ID_MEMBERRANK_ALL = "";				// 全ユーザ
		public const string FLG_NOVELTYGRANTCONDITIONS_USER_RANK_ID_MEMBER_ONLY = "MEMBER_ONLY";		// 会員のみ

		// ノベルティ：自動付与フラグ
		public const string FLG_NOVELTY_AUTO_ADDITIONAL_FLG_INVALID = "0";	// 無効
		public const string FLG_NOVELTY_AUTO_ADDITIONAL_FLG_VALID = "1";	// 有効

		// レコメンド設定：レコメンド表示ページ
		public const string FLG_RECOMMEND_RECOMMEND_DISPLAY_PAGE_ORDER_CONFIRM = "ORDER_CONFIRM";	// 注文確認
		public const string FLG_RECOMMEND_RECOMMEND_DISPLAY_PAGE_ORDER_COMPLETE = "ORDER_COMPLETE";	// 注文完了

		// レコメンド設定：レコメンド区分
		public const string FLG_RECOMMEND_RECOMMEND_KBN_UP_SELL = "UP_SELL";			// アップセル
		public const string FLG_RECOMMEND_RECOMMEND_KBN_CROSS_SELL = "CROSS_SELL";		// クロスセル
		public const string FLG_RECOMMEND_RECOMMEND_KBN_RECOMMEND_HTML = "RECOMMEND_HTML";	// レコメンドHTML

		// レコメンド設定：有効フラグ
		public const string FLG_RECOMMEND_VALID_FLG_INVALID = "0";				// 無効
		public const string FLG_RECOMMEND_VALID_FLG_VALID = "1";				// 有効

		// レコメンド設定：レコメンド表示区分PC
		public const string FLG_RECOMMEND_RECOMMEND_DISPLAY_KBN_PC_TEXT = "0";	// TEXT
		public const string FLG_RECOMMEND_RECOMMEND_DISPLAY_KBN_PC_HTML = "1";	// HTML

		// レコメンド設定：レコメンド表示区分SP
		public const string FLG_RECOMMEND_RECOMMEND_DISPLAY_KBN_SP_TEXT = "0";	// TEXT
		public const string FLG_RECOMMEND_RECOMMEND_DISPLAY_KBN_SP_HTML = "1";	// HTML

		// レコメンド設定：ワンタイム表示フラグ
		public const string FLG_RECOMMEND_ONETIME_FLG_VALID = "VALID";	// 有効
		public const string FLG_RECOMMEND_ONETIME_FLG_INVALID = "INVALID";	// 無効

		// レコメンド適用条件アイテム：レコメンド適用条件種別
		public const string FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_BUY = "BUY";			// 過去注文もしくはカートで購入している
		public const string FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_NOT_BUY = "NOT_BUY";	// 過去注文もしくはカートで購入していない

		// レコメンドアップセル対象アイテム：レコメンド適用条件アイテム種別
		public const string FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE_NORMAL = "NORMAL";					// 通常商品
		public const string FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE_FIXED_PURCHASE = "FIXED_PURCHASE";	// 定期商品
		/// <summary>頒布会商品</summary>
		public const string FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE_SUBSCRIPTION_BOX = "SUBSCRIPTION_BOX";

		// レコメンド適用条件アイテム：レコメンド適用条件アイテム単位種別
		public const string FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_UNIT_TYPE_PRODUCT = "PRODUCT";		// 商品指定
		public const string FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_UNIT_TYPE_VARIATION = "VARIATION";	// 商品バリエーション指定

		// レコメンドアップセル対象アイテム：レコメンドアップセル対象アイテム種別
		public const string FLG_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE_NORMAL = "NORMAL";					// 通常商品
		public const string FLG_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE_FIXED_PURCHASE = "FIXED_PURCHASE";	// 定期商品
		/// <summary>頒布会商品</summary>
		public const string FLG_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE_SUBSCRIPTION_BOX = "SUBSCRIPTION_BOX";

		// レコメンドアイテム：レコメンドアイテム種別
		public const string FLG_RECOMMENDITEM_RECOMMEND_ITEM_TYPE_NORMAL = "NORMAL";					// 通常商品
		public const string FLG_RECOMMENDITEM_RECOMMEND_ITEM_TYPE_FIXED_PURCHASE = "FIXED_PURCHASE";	// 定期商品
		/// <summary>頒布会商品</summary>
		public const string FLG_RECOMMENDITEM_RECOMMEND_ITEM_TYPE_SUBSCRIPTION_BOX = "SUBSCRIPTION_BOX";

		// レコメンドアイテム：レコメンドアイテム投入数種別
		public const string FLG_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY_TYPE_SPECIFY_QUANTITY = "SPECIFY_QUANTITY";	// 指定した数
		public const string FLG_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY_TYPE_SAME_QUANTITY = "SAME_QUANTITY";			// 更新対象と同じ数

		// レコメンドアイテム：定期購入区分
		public const string FLG_RECOMMENDITEM_FIXED_PURCHASE_KBN_MONTHLY_DATE = "01";
		public const string FLG_RECOMMENDITEM_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY = "02";
		public const string FLG_RECOMMENDITEM_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS = "03";

		// レコメンドアイテム：定期購入設定（最終日）
		public const string FLG_RECOMMENDITEM_FIXED_PURCHASE_SETTING1_DAY_LASTDAY = "-1";

		// レコメンド表示履歴：表示区分
		public const string FLG_RECOMMENDHISTORY_DISPLAY_KBN_FRONT = "FRONT";	// Front
		public const string FLG_RECOMMENDHISTORY_DISPLAY_KBN_EC = "EC";	// EC管理画面

		// レコメンド表示履歴：購入フラグ
		public const string FLG_RECOMMENDHISTORY_ORDERED_FLG_BUY = "BUY";	// 購入済み
		public const string FLG_RECOMMENDHISTORY_ORDERED_FLG_DISP = "DISP";	// 未購入（表示のみ）

		// リアル店舗：説明
		public const string FLG_REALSHOP_DESC_TEXT = "0";	// TEXT
		public const string FLG_REALSHOP_DESC_HTML = "1";	// HTML

		// リアル店舗：有効フラグ
		public const string FLG_REALSHOP_VALID_FLG_INVALID = "0";		// 無効
		public const string FLG_REALSHOP_VALID_FLG_VALID = "1";		// 有効

		/// レコメンド系商品の在庫切れ表示可否
		public const string FLG_SHOW_OUT_OF_STOCK_ITEMS_INVALID = "0"; // 無効
		public const string FLG_SHOW_OUT_OF_STOCK_ITEMS_VALID = "1"; // 有効

		// テンポラリデータテーブル：テンポラリタイプ
		public const string FLG_TEMPDATAS_TEMP_TYPE_CHANGE_SESSION_ID = "ChangeSessionId";	// セッションID変更
		public const string FLG_TEMPDATAS_TEMP_TYPE_GO_TO_OTHER_SITE = "GoToOtherSite";		// 別サイト遷移
		public const string FLG_TEMPDATAS_TEMP_TYPE_LOGIN_ERROR_INFO_LOGIN_ID = "LoginErrorHistoryLoginId";		// ログイン失敗情報(IP+ログインID)
		public const string FLG_TEMPDATAS_TEMP_TYPE_LOGIN_ERROR_INFO_PASSWORD = "LoginErrorHistoryPassword";	// ログイン失敗情報(IP+パスワード)
		public const string FLG_TEMPDATAS_TEMP_TYPE_CREDIT_AUTH_ERROR_INFO_USER_ID = "CreditAuthErrorInfoUserId";	// クレジット認証失敗情報(ユーザーID)
		public const string FLG_TEMPDATAS_TEMP_TYPE_LINE_AUTH_KEY_ERROR_INFO_IP = "LineAuthKeyErrorInfoIp";	// LINE連携API_Authキー認証失敗情報(Ipアドレス)
		public const string FLG_TEMPDATAS_TEMP_TYPE_NE_TOKEN = "NextEngineToken";
		public const string FLG_TEMPDATAS_TEMP_TYPE_CREDIT_AUTH_ERROR_INFO_IP_ADDRESS = "CreditAuthErrorInfoIpAddress";		// Credit auth error Information IP address
		/// <summary>Letro auth key error info ip</summary>
		public const string FLG_TEMPDATAS_TEMP_TYPE_LETRO_AUTH_KEY_ERROR_INFO_IP = "LetroAuthKeyErrorInfoIp";
		/// <summary>Auth code</summary>
		public const string FLG_TEMPDATAS_TEMP_TYPE_AUTHCODE = "AuthCode";
		/// <summary>Auth code send ip address</summary>
		public const string FLG_TEMPDATAS_TEMP_TYPE_AUTHCODE_SEND_IPADDRESS = "AuthCodeSendIpAddress";
		/// <summary>Auth code try ip address</summary>
		public const string FLG_TEMPDATAS_TEMP_TYPE_AUTHCODE_TRY_IPADDRESS = "AuthCodeTryIpAddress";
		/// <summary>Auth code try send tel</summary>
		public const string FLG_TEMPDATAS_TEMP_TYPE_AUTHCODE_TRY_SENDTEL = "AuthCodeTrySendTel";

		// ユーザ拡張項目設定：入力方法
		public const string FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT = "TB"; // テキストボックス
		public const string FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN = "DDL"; // ドロップダウンリスト
		public const string FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX = "CB"; // チェックボックス
		public const string FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO = "RB"; // ラジオボタン
		// ユーザ拡張項目設定：表示区分
		public const string FLG_USEREXTENDSETTING_DISPLAY_PC = "PC"; // PC表示
		public const string FLG_USEREXTENDSETTING_DISPLAY_MB = "MB"; // MB表示
		//public const string FLG_USEREXTENDSETTING_DISPLAY_SP = "SP"; // SP表示
		public const string FLG_USEREXTENDSETTING_DISPLAY_EC = "EC"; // EC表示
		//public const string FLG_USEREXTENDSETTING_DISPLAY_MP = "MP"; // MP表示
		public const string FLG_USEREXTENDSETTING_DISPLAY_CS = "CS"; // CS表示
		// ユーザ拡張項目設定：概要の表示区分
		public const string FLG_USEREXTENDSETTING_OUTLINE_TEXT = "TEXT"; // TEXT
		public const string FLG_USEREXTENDSETTING_OUTLINE_HTML = "HTML"; // HTML
		// ユーザ拡張項目設定：プリフィックスキー
		public const string FLG_USEREXTENDSETTING_PREFIX_KEY = "usrex_"; // プリフィックスキー
		// ユーザ拡張項目設定：登録時のみフラグ
		public const string FLG_USEREXTENDSETTING_INITONLY = "INITONLY"; // 会員登録のみ
		public const string FLG_USEREXTENDSETTING_UPDATABLE = "UPDATABLE"; // 会員登録・編集両方

		// 商品検索ワード：アクセス区分
		public const string FLG_PRODUCTSEARCHWORDHISTORY_ACCESS_KBN_PC = "0";			// PC
		public const string FLG_PRODUCTSEARCHWORDHISTORY_ACCESS_KBN_MOBILE = "1";		// モバイル
		public const string FLG_PRODUCTSEARCHWORDHISTORY_ACCESS_KBN_SMARTPHONE = "2";	// スマートフォン

		// メールテンプレート：メールカテゴリ
		public const string FLG_MAILTEMPLATE_MAIL_CATEGORY_DEFAULT = "";					// デフォルトカテゴリ
		public const string FLG_MAILTEMPLATE_MAIL_CATEGORY_ORDER = "Order";					// 受注情報出力用メールテンプレート
		public const string FLG_MAILTEMPLATE_MAIL_CATEGORY_FIXEDPURCHASE = "FixedPurchase";	// 定期購入情報出力用メールテンプレート
		public const string FLG_MAILTEMPLATE_MAIL_CATEGORY_PRODUCTARRIVAL = "ProductArrival";	// 入荷通知メール情報出力用メールテンプレート
		public const string FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM = "Custom";					// デフォルトカスタムメールテンプレート
		public const string FLG_MAILTEMPLATE_MAIL_CATEGORY_STOCKALERT = "STOCKALERT";			// お気に入り商品在庫リマインドメールテンプレート
		/// <summary>受注情報出力用カスタムメールテンプレート</summary>
		public const string FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_ORDER = "Custom_Order";
		/// <summary>定期購入情報出力用カスタムメールテンプレート</summary>
		public const string FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_FIXEDPURCHASE = "Custom_FixedPurchase";
		/// <summary>ユーザー情報出力用カスタムメールテンプレート</summary>
		public const string FLG_MAILTEMPLATE_MAIL_CATEGORY_CUSTOM_USER = "Custom_User";
		/// <summary>Mailtemplate mail category to real shop</summary>
		public const string FLG_MAILTEMPLATE_MAIL_CATEGORY_TOREALSHOP = "ToRealShop";
		/// <summary>Mailtemplate mail category from real shop</summary>
		public const string FLG_MAILTEMPLATE_MAIL_CATEGORY_FROMREALSHOP = "FromRealShop";

		// メールテンプレート：自動送信フラグ
		public const string FLG_MAILTEMPLATE_AUTOSENDFLG_NOTSEND = "0";	// 自動送信フラグOFF
		public const string FLG_MAILTEMPLATE_AUTOSENDFLG_SEND = "1";		// 自動送信フラグON

		// メールテンプレート：削除フラグ
		public const string FLG_MAILTEMPLATE_DELFLG_UNDELETED = "0";	// 通常
		public const string FLG_MAILTEMPLATE_DELFLG_DELETED = "1";		// 削除済み

		/// <summary>HTMLメール送信しない</summary>
		public const string FLG_MAILTEMPLATE_SEND_HTML_FLG_NOTSEND = "0";
		/// <summary>HTMLメール送信する</summary>
		public const string FLG_MAILTEMPLATE_SEND_HTML_FLG_SEND = "1";

		// 商品定期購入割引設定：購入回数以降フラグ
		public const string FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT_MORE_THAN_FLG_VALID = "1";		// 有効
		public const string FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT_MORE_THAN_FLG_INVALID = "0";	// 無効

		// 商品定期購入割引設定：値引きタイプ
		public const string FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN = "YEN";			// 円
		public const string FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_PERCENT = "PERCENT";	// パーセント

		// 商品定期購入割引設定：値引きタイプ
		public const string FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_TYPE_POINT = "POINT";		// ポイント
		public const string FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_TYPE_PERCENT = "PERCENT";	// パーセント

		// 商品グループマスタ：有効フラグ
		public const string FLG_PRODUCTGROUP_VALID_FLG_INVALID = "0";		// 無効
		public const string FLG_PRODUCTGROUP_VALID_FLG_VALID = "1";		// 有効

		// 商品グループマスタ：商品グループページ表示内容HTML区分
		public const string FLG_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS_KBN_TEXT = "0";	// TEXT
		public const string FLG_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS_KBN_HTML = "1";	// HTML

		// 商品グループアイテムマスタ：アイテムタイプ
		public const string FLG_PRODUCTGROUPITEM_ITEM_TYPE_PRODUCT = "PRODUCT";	// 商品ID指定

		// 商品同梱設定：対象注文種別
		public const string FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_ALL = "ALL";	// すべての注文
		public const string FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_FIXED_PURCHASE = "FIXED_PURCHASE"; // 定期注文
		public const string FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_NORMAL = "NORMAL";	// 通常注文

		// 商品同梱設定：対象商品指定方法
		public const string FLG_PRODUCTBUNDLE_TARGET_PRODUCT_KBN_SELECT = "SELECT";	// 条件指定
		public const string FLG_PRODUCTBUNDLE_TARGET_PRODUCT_KBN_ALL = "ALL";	// 全商品指定

		// 商品同梱設定：対象定期注文回数初期値
		public const int FLG_PRODUCTBUNDLE_TARGET_ORDER_FIXED_PURCHASE_COUNT_FROM_DEFAULT = 0;	//対象定期注文回数(FROM)
		public const int FLG_PRODUCTBUNDLE_TARGET_ORDER_FIXED_PURCHASE_COUNT_TO_DEFAULT = 0;	//対象定期注文回数(TO)

		// 商品同梱設定：ユーザ利用可能回数設定
		public const string FLG_PRODUCTBUNDLE_USABLE_TIMES_KBN_NOLIMIT = "NO_LIMIT";	// 無制限
		public const string FLG_PRODUCTBUNDLE_USABLE_TIMES_KBN_ONCETIME = "ONCE_TIME";	// 1回のみ
		public const string FLG_PRODUCTBUNDLE_USABLE_TIMES_KBN_NUMSPECIFY = "NUM_SPECIFY";	// 回数指定

		// 商品同梱設定：商品同梱設定適用種別
		public const string FLG_PRODUCTBUNDLE_APPLY_TYPE_FOR_ORDER = "FOR_ORDER";	// 注文単位
		public const string FLG_PRODUCTBUNDLE_APPLY_TYPE_FOR_PRODUCT = "FOR_PRODUCT";	// 購入商品単位

		// 商品同梱設定：有効フラグ
		public const string FLG_PRODUCTBUNDLE_VALID_FLG_INVALID = "INVALID";	// 無効
		public const string FLG_PRODUCTBUNDLE_VALID_FLG_VALID = "VALID";	// 有効

		// 商品同梱設定：重複適用フラグ
		public const string FLG_PRODUCTBUNDLE_MULTIPLE_APPLY_FLG_INVALID = "INVALID";	// 無効
		public const string FLG_PRODUCTBUNDLE_MULTIPLE_APPLY_FLG_VALID = "VALID";	// 有効

		// 商品同梱設定：適用優先順
		public const int FLG_PRODUCTBUNDLE_APPLY_ORDER_DEFAULT = 100;	// 既定値

		// 商品同梱 同梱商品：同梱個数
		public const int FLG_PRODUCTBUNDLEITEM_GRANT_PRODUCT_COUNT_DEFAULT = 0;	// 初期値

		// 商品同梱設定：ターゲットリストID除外フラグ
		public const string FLG_PRODUCTBUNDLE_TARGET_ID_EXCEPT_FLG_TARGET = "0";	// ターゲットリストは対象となる
		public const string FLG_PRODUCTBUNDLE_TARGET_ID_EXCEPT_FLG_EXCEPT = "1";	// ターゲットリストは除外する

		// 商品同梱 同梱商品：初回のみ同梱フラグ
		public const string FLG_PRODUCTBUNDLEITEM_ORDERED_PRODUCT_EXCEPT_FLG_BUNDLED_TARGET = "0";	// 同梱対象
		public const string FLG_PRODUCTBUNDLEITEM_ORDERED_PRODUCT_EXCEPT_FLG_BUNDLED_EXCEPT = "1";	// 同梱除外

		// 更新履歴情報：更新履歴区分
		public const string FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_USER = "User";						// ユーザー
		public const string FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_ORDER = "Order";					// 注文
		public const string FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_FIXEDPURCHASE = "FixedPurchase";	// 定期

		// ＆mall在庫引当：ステータス
		public const string FLG_ANDMALLINVENTORYRESERVE_STATUS_ALLOCATION = "RESERVE";				//在庫引当済み
		public const string FLG_ANDMALLINVENTORYRESERVE_STATUS_CANCEL = "CANCEL";					//在庫引当キャンセル

		// 名称翻訳設定マスタ：対象データ区分
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT = "Product";	// 商品情報
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION = "ProductVariation";	// 商品バリエーション情報
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY = "ProductCategory";	// 商品カテゴリ情報
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION = "SetPromotion";	// セットプロモーション情報
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET = "ProductSet";	// 商品セット情報
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_COUPON = "Coupon";	// クーポン情報
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_MEMBERRANK = "MemberRank";	// 会員ランク情報
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT = "Payment";	// 決済種別情報
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_USEREXTENDSETTING = "UserExtendSetting";	// ユーザー拡張項目情報
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTBRAND = "ProductBrand";	// 商品ブランド情報
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_NOVELTY = "Novelty";	// ノベルティ情報
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS = "News";	// 新着情報
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING = "OrderMemoSetting";	// 注文メモ設定情報
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON = "FixedPurchaseCancelReason";	// 定期解約理由区分設定情報
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTLISTDISPSETTING = "ProductListDispSetting";	// 商品一覧表示設定情報
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_SITEINFORMATION = "SiteInformation";	// サイト基本情報
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTTAGSETTING = "ProductTagSetting"; // 商品タグ情報
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATE = "Coordinate";	// コーディネート
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_COORDINATECATEGORY = "CoordinateCategory";	// コーディネートカテゴリ
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_STAFF = "Staff";	// スタッフ
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_REALSHOP = "RealShop";	// リアル店舗
		public const string FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDEREXTENDSETTING = "OrderExtendSetting";    // 注文拡張項目設定

		// 名称翻訳設定マスタ：翻訳対象項目
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_NAME = "name";	// 商品名
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_OUTLINE = "outline";	// 商品概要
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL1 = "desc_detail1";	// 商品詳細1
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL2 = "desc_detail2";	// 商品詳細2
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL3 = "desc_detail3";	// 商品詳細3
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL4 = "desc_detail4";	// 商品詳細4
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_RETURN_EXCHANGE_MESSAGE = "return_exchange_message";	// 商品返品交換文言
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_CATCHCOPY = "catchcopy";	// キャッチコピー
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTVARIATION_VARIATION_NAME1 = "variation_name1";	// バリエーション名1
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTVARIATION_VARIATION_NAME2 = "variation_name2";	// バリエーション名2
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTVARIATION_VARIATION_NAME3 = "variation_name3";	// バリエーション名3
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTCATEGORY_NAME = "name";	// カテゴリ名
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SETPROMOTION_SETPROMOTION_DISP_NAME = "setpromotion_disp_name";	// 表示用セットプロモーション名
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SETPROMOTION_DESCRIPTION = "description";	// 表示用文言(セットプロモーション)
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTSET_PRODUCT_SET_NAME = "product_set_name";	// 商品セット名
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTSET_DESCRIPTION = "description";	// 表示用文言(商品セット)
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_COUPON_COUPON_DISP_NAME = "coupon_disp_name";	// 表示用クーポン名
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_COUPON_COUPON_DISP_DISCRIPTION = "coupon_disp_discription";	// フロント表示用クーポン説明文
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_MEMBERRANK_MENBER_RANK_NAME = "member_rank_name";	// ランク名(会員ランク)
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PAYMENT_PAYMENT_NAME = "payment_name";	// 決済種別名
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_USEREXTENDSETTING_SETTING_NAME = "setting_name";	// 名称(ユーザー拡張項目)
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_USEREXTENDSETTING_OUTLINE = "outline";	// ユーザ拡張項目概要
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTBRAND_BRAND_NAME = "brand_name";	// ブランド名
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_NOVELTY_NOVELTY_DISP_NAME = "novelty_disp_name";	// ノベルティ名（表示用）
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_NEWS_NEWS_TEXT = "news_text";	// 本文(新着情報)
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_ORDERMEMOSETTING_ORDER_MEMO_NAME = "order_memo_name";	// 注文メモ名称
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_ORDERMEMOSETTING_DEFAULT_TEXT = "default_text";	// デフォルト文字列
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_FIXEDPURCHASECANCELREASON_CANCEL_REASON_NAME = "cancel_reason_name";	// 解約理由区分名
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTLISTDISPSETTING_SETTING_NAME = "setting_name";	// 設定名(商品一覧表示設定)
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_SHOP_NAME = "ShopName";	// サイト基本情報「サイト名」
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_COMPANY_NAME = "CompanyName";	// サイト基本情報「企業名」
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SITEINFORMATION_CONTACT_CENTER_INFO = "ContactCenterInfo";	// サイト基本情報「問い合わせ窓口情報」
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTTAGSETTING_TAG_NAME = "tag_name"; // タグ名称
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_COORDINATE_TITLE = "coordinate_title"; // タイトル
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_COORDINATE_SUMMARY = "coordinate_summary"; // 概要
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_COORDINATECATEGORY_COORDINATE_CATEGORY_NAME = "coordinate_category_name"; // カテゴリ名
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_STAFF_STAFF_NAME = "staff_name"; // スタッフ名
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_STAFF_STAFF_PROFILE = "staff_profile"; // スタッフプロフィール
		public const string FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_REALSHOP_NAME = "name"; // リアル店舗名

		// Invoice Status
		public const string FLG_ORDER_INVOICE_STATUS_NOT_ISSUED = "01";				// 未発行
		public const string FLG_ORDER_INVOICE_STATUS_ISSUED = "02";					// 発行済
		public const string FLG_ORDER_INVOICE_STATUS_ISSUED_LINKED = "03";			// 発行済（連携済）
		public const string FLG_ORDER_INVOICE_STATUS_REFUND = "04";					// 払い戻し
		public const string FLG_ORDER_INVOICE_STATUS_REFUND_COMPLETED = "05";		// 払い戻し済（連携済）
		public const string FLG_ORDER_INVOICE_STATUS_CANCEL = "06";					// キャンセル

		// 商品税率カテゴリマスタ：商品税率カテゴリID
		public const string DEFAULT_PRODUCT_TAXCATEGORY_ID = "default";			// デフォルト
		// 商品税率カテゴリマスタ：新規登録区分
		public const string FLG_PRODUCT_TAXCATEGORY_KBN_NOT_REGISTERED = "notRegistered";			// 未登録
		// 商品税率カテゴリマスタ：新規登録済み区分
		public const string FLG_PRODUCT_TAXCATEGORY_KBN_REGISTERED = "Registered";			// 登録済み

		// 商品在庫文言マスタ：削除フラグ
		public const string FLG_PRODUCTSTOCKMESSAGE_DEL_FLG_ON = "1";	// 削除済み
		public const string FLG_PRODUCTSTOCKMESSAGE_DEL_FLG_OFF = "0";	// 通常

		// 自動翻訳ワード管理
		/// <summary>自動翻訳ワード管理:削除対象フラグ:ON</summary>
		public const string FLG_AUTOTRANSLATIONWORD_CLEAR_FLG_ON = "1";
		/// <summary>自動翻訳ワード管理:削除対象フラグ:OFF</summary>
		public const string FLG_AUTOTRANSLATIONWORD_CLEAR_FLG_OFF = "0";

		/// <summary>リードタイム設定可能フラグ:ON</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_LEAD_TIME_SET_FLG_VALID = "1";	// 有効
		/// <summary>リードタイム設定可能フラグ:OFF</summary>
		public const string FLG_DELIVERYCOMPANY_DELIVERY_LEAD_TIME_SET_FLG_INVALID = "0";	// 無効

		/// <summary>受注情報表示一覧設定：表示フラグ</summary>
		public const string FLG_MANAGERLISTDISPSETTING_DISP_FLAG_ON = "1"; //有効
		/// <summary>受注情報表示一覧設定：表示フラグ</summary>
		public const string FLG_MANAGERLISTDISPSETTING_DISP_FLAG_OFF = "0"; //無効
		/// <summary>受注情報表示一覧設定：設定区分として"受注情報"を指定する</summary>
		public const string FLG_MANAGERLISTDISPSETTING_DISPSETTINGKBN_ORDERLIST = "orderlist";
		/// <summary>受注情報表示一覧設定：設定区分として"受注ワークフロー"を指定する</summary>
		public const string FLG_MANAGERLISTDISPSETTING_DISPSETTINGKBN_ORDERWORKFLOW = "orderworkflow";
		/// <summary>受注情報表示一覧設定：設定区分として"店舗受取注文情報"を指定する</summary>
		public const string FLG_MANAGERLISTDISPSETTING_DISPSETTINGKBN_ORDERSTOREPICKUP = "orderstorepickup";
		/// <summary>受注情報または受注ワークフロー：拡張項目リスト</summary>
		public const string FLG_ORDER_EXTEND_STATUS_LIST = "extend_status_list";
		/// <summary>受注情報または受注ワークフロー：在庫引当ステータス(一覧用)</summary>
		public const string FLG_ORDER_ORDER_STOCKRESERVED_STATUS_LIST = "order_stockreserved_status_list";
		/// <summary>注文拡張ステータス設定マスタ：拡張ステータスのMAXサイズのフィールド名</summary>
		public const string FLG_ORDER_EXTEND_STATUS_MAX = "extend_status_max";
		/// <summary>受注情報と受注ワークフローに表示する拡張ステータス一覧のフィールド名</summary>
		public const string FLG_DISPLAY_EXTEND_STATUS_LIST_VIEW = "DisplayExtendStatusList";
		/// <summary>受注情報と受注ワークフローの受注情報一覧表示の表示設定のフィールド名</summary>
		public const string FLG_COLUMNDISPLAY_SETTINGS_VIEW = "ColumnDisplaySettings";

		/// <summary>リアルタイム累計購入回数用アクション名：注文</summary>
		public const string FLG_REAL_TIME_ORDER_COUNT_ACTION_ORDER = "order";
		/// <summary>リアルタイム累計購入回数用アクション名：キャンセル</summary>
		public const string FLG_REAL_TIME_ORDER_COUNT_ACTION_CANCEL = "cancel";
		/// <summary>リアルタイム累計購入回数用アクション名：注文同梱</summary>
		public const string FLG_REAL_TIME_ORDER_COUNT_ACTION_COMBINE = "combine";
		/// <summary>リアルタイム累計購入回数用アクション名：返品交換</summary>
		public const string FLG_REAL_TIME_ORDER_COUNT_ACTION_RETURN_EXCHANGE = "returnExchange";
		/// <summary>リアルタイム累計購入回数用アクション名：ロールバック</summary>
		public const string FLG_REAL_TIME_ORDER_COUNT_ACTION_ROLLBACK = "rollback";

		/// <summary>発票種類:個人発票</summary>
		public const string FLG_TW_UNIFORM_INVOICE_PERSONAL = "PERSONAL";
		/// <summary>発票種類:会社発票</summary>
		public const string FLG_TW_UNIFORM_INVOICE_COMPANY = "COMPANY";
		/// <summary>発票種類:寄付発票</summary>
		public const string FLG_TW_UNIFORM_INVOICE_DONATE = "DONATE";

		/// <summary>載具種別:MOBILE</summary>
		public const string FLG_ORDER_TW_CARRY_TYPE_MOBILE = "MOBILE";
		/// <summary>載具種別:CERTIFICATE</summary>
		public const string FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE = "CERTIFICATE";
		/// <summary>載具種別:NONE</summary>
		public const string FLG_ORDER_TW_CARRY_TYPE_NONE = "";

		/// <summary>Setting file taiwan carry kbn new</summary>
		public const string FLG_TW_CARRY_KBN_NEW = "NEW";

		/// <summary>Payment Paidy: API status: CLOSED</summary>
		public const string FLG_PAYMENT_PAIDY_API_STATUS_CLOSED = "closed";
		/// <summary>Payment Paidy: API status: REJECTED</summary>
		public const string FLG_PAYMENT_PAIDY_API_STATUS_REJECTED = "rejected";
		/// <summary>Payment Paidy: API status: AUTHORIZED</summary>
		public const string FLG_PAYMENT_PAIDY_API_STATUS_AUTHORIZED = "authorized";

		/// <summary>Use convenience store</summary>
		public const string FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON = "1";
		/// <summary>Not use convenience store</summary>
		public const string FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF = "0";

		/// <summary>User shipping kbn</summary>
		public const string FLG_FIXEDPURCHASE_USER_SHIPPING_KBN = "user_shipping_kbn";

		/// <summary>Usrex Atone Token Id</summary>
		public const string FLG_USEREXTEND_USREX_ATONE_TOKEN_ID = "usrex_atone_token_id";
		/// <summary>Usrex Aftee Token Id</summary>
		public const string FLG_USEREXTEND_USREX_AFTEE_TOKEN_ID = "usrex_aftee_token_id";

		/// <summary>LINE Pay Return Code Default</summary>
		public const string FLG_PAYMENT_LINEPAY_CODE_DEFAULT = "0000";

		/// <summary>Receiving store tw ecpay shipping service: Family mart</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_FAMILY_MART = "FM";
		/// <summary>Receiving store tw ecpay shipping service: Family mart payment</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_FAMILY_MART_PAYMENT = "FMR";
		/// <summary>Receiving store tw ecpay shipping service: 7-ELEVEN</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_7_ELEVENT = "SE";
		// <summary>Receiving store tw ecpay shipping service: 7-ELEVEN payment</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_7_ELEVENT_PAYMENT = "SER";
		/// <summary>Receiving store tw ecpay shipping service: Hi-Life</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_HI_LIFE = "HL";
		/// <summary>Receiving store tw ecpay shipping service: Hi-Life payment</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_HI_LIFE_PAYMENT = "HLR";

		/// <summary>Receiving store tw ecpay logistics type: CVS</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_TYPE_CVS = "CVS";
		/// <summary>Receiving store tw ecpay logistics type: Home</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_TYPE_HOME = "Home";

		/// <summary>Receiving store tw ecpay logistics sub type: Family mart</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_FAMILY_MART = "FAMI";
		/// <summary>Receiving store tw ecpay logistics sub type: 7-ELEVEN</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_7_ELEVEN = "UNIMART";
		/// <summary>Receiving store tw ecpay logistics sub type: Hi-Life</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_HI_LIFE = "HILIFE";
		/// <summary>Receiving store tw ecpay logistics sub type: Family mart C2C</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_FAMILY_MART_C2C = "FAMIC2C";
		/// <summary>Receiving store tw ecpay logistics sub type: 7-ELEVEN C2C</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_7_ELEVEN_C2C = "UNIMARTC2C";
		/// <summary>Receiving store tw ecpay logistics sub type: Hi-Life C2C</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_HI_LIFE_C2C = "HILIFEC2C";
		/// <summary>Receiving store tw ecpay logistics sub type: Yamato home delivery</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_YAMATO_HOME_DELIVERY = "TCAT";
		/// <summary>Receiving store tw ecpay logistics sub type: Home delivery</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_HOME_DELIVERY = "ECAN";

		/// <summary>Receiving store tw ecpay logistics collection: ON</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_ON = "Y";
		/// <summary>Receiving store tw ecpay logistics collection: OFF</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_OFF = "N";

		/// <summary>Receiving store tw ecpay logistics device: PC</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_DEVICE_PC = "0";
		/// <summary>Receiving store tw ecpay logistics device: SP</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_DEVICE_SP = "1";

		/// <summary>Receiving store tw ecpay shipping status: Abnormal</summary>
		public const string FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_STATUS_ABNORMAL = "23";

		/// <summary>Receiving store tw ecpay logistics device: Shipping Status Convert</summary>
		public const string FIELD_ORDERSHIPPING_SHIPPING_STATUS_CONVERT = "shipping_status_convert";

		/// <summary>Online delivery status settled</summary>
		public const string FLG_ORDER_ONLINE_DELIVERY_STATUS_SETTLED = "01";

		/// <summary>Credit Installments Code Once</summary>
		public const string FLG_ORDER_CARD_INSTALLMENTS_CODE_ONCE = "01";

		/// <summary>Shipping size kbn: XXS</summary>
		public const int FLG_SHIPPING_SIZE_KBN_XXS = 10;
		/// <summary>Shipping size kbn: XS</summary>
		public const int FLG_SHIPPING_SIZE_KBN_XS = 20;
		/// <summary>Shipping size kbn: S</summary>
		public const int FLG_SHIPPING_SIZE_KBN_S = 40;
		/// <summary>Shipping size kbn: ML</summary>
		public const int FLG_SHIPPING_SIZE_KBN_ML = 60;
		/// <summary>Shipping size kbn: XL</summary>
		public const int FLG_SHIPPING_SIZE_KBN_XL = 90;
		/// <summary>Shipping size kbn: XXL</summary>
		public const int FLG_SHIPPING_SIZE_KBN_XXL = 150;

		/// <summary>Max lenght owner name</summary>
		public const int FLG_MAX_LENGHT_OWNER_NAME_ECPAY = 10;

		/// <summary>Payment Type EcPay Credit Card</summary>
		public const string FLG_PAYMENT_TYPE_ECPAY_CREDIT = "Credit";
		/// <summary>Payment Type EcPay Web ATM</summary>
		public const string FLG_PAYMENT_TYPE_ECPAY_WEBATM = "WebATM";
		/// <summary>Payment Type EcPay ATM</summary>
		public const string FLG_PAYMENT_TYPE_ECPAY_ATM = "ATM";
		/// <summary>Payment Type EcPay CVS</summary>
		public const string FLG_PAYMENT_TYPE_ECPAY_CVS = "CVS";
		/// <summary>Payment Type EcPay Barcode</summary>
		public const string FLG_PAYMENT_TYPE_ECPAY_BARCODE = "BARCODE";

		/// <summary>Cart list landing page name</summary>
		public static string CART_LIST_LP_PAGE_NAME = "CARTLISTLP";
		/// <summary>Add cart kbn: Normal</summary>
		public const string FLG_ADD_CART_KBN_NORMAL = "1";
		/// <summary>Add cart kbn: FixedPurchase</summary>
		public const string FLG_ADD_CART_KBN_FIXEDPURCHASE = "2";

		/// <summary>Payment Type Neweb Pay: Credit Card</summary>
		public const string FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT = "CREDIT";
		/// <summary>Payment Type Neweb Pay: LINE Pay</summary>
		public const string FLG_PAYMENT_TYPE_NEWEBPAY_LINEPAY = "LINEPAY";
		/// <summary>Payment Type Neweb Pay: Web ATM</summary>
		public const string FLG_PAYMENT_TYPE_NEWEBPAY_WEBATM = "WEBATM";
		/// <summary>Payment Type Neweb Pay: ATM</summary>
		public const string FLG_PAYMENT_TYPE_NEWEBPAY_ATM = "VACC";
		/// <summary>Payment Type Neweb Pay: CVS</summary>
		public const string FLG_PAYMENT_TYPE_NEWEBPAY_CVS = "CVS";
		/// <summary>Payment Type Neweb Pay: BARCODE</summary>
		public const string FLG_PAYMENT_TYPE_NEWEBPAY_BARCODE = "BARCODE";
		/// <summary>Payment Type Neweb Pay: CVSCOM</summary>
		public const string FLG_PAYMENT_TYPE_NEWEBPAY_CVSCOM = "CVSCOM";

		/// <summary>お支払い方法がクレジットカード一括払いの場合</summary>
		public const int FLG_NEWEB_PAYMENT_CREDIT_FLAG_ON = 1;
		/// <summary>お支払い方法がクレジットカード一括払い以外の場合</summary>
		public const int FLG_NEWEB_PAYMENT_CREDIT_FLAG_OFF = 0;
		/// <summary>お支払い方法がLINE Payの場合</summary>
		public const int FLG_NEWEB_PAYMENT_LINEPAY_FLAG_ON = 1;
		/// <summary>お支払い方法がLINE Pay以外の場合</summary>
		public const int FLG_NEWEB_PAYMENT_LINEPAY_FLAG_OFF = 0;
		/// <summary>お支払い方法がWEBATMの場合</summary>
		public const int FLG_NEWEB_PAYMENT_WEBATM_FLAG_ON = 1;
		/// <summary>お支払い方法がWEBATM以外の場合</summary>
		public const int FLG_NEWEB_PAYMENT_WEBATM_FLAG_OFF = 0;
		/// <summary>お支払い方法がATMの場合</summary>
		public const int FLG_NEWEB_PAYMENT_ATM_FLAG_ON = 1;
		/// <summary>お支払い方法がATM以外の場合</summary>
		public const int FLG_NEWEB_PAYMENT_ATM_FLAG_OFF = 0;
		/// <summary>お支払い方法がコンビニ番号の場合</summary>
		public const int FLG_NEWEB_PAYMENT_CVS_FLAG_ON = 1;
		/// <summary>お支払い方法がコンビニ番号以外の場合</summary>
		public const int FLG_NEWEB_PAYMENT_CVS_FLAG_OFF = 0;
		/// <summary>お支払い方法がコンビニバーコードの場合</summary>
		public const int FLG_NEWEB_PAYMENT_BARCODE_FLAG_ON = 1;
		/// <summary>お支払い方法がコンビニバーコード以外の場合</summary>
		public const int FLG_NEWEB_PAYMENT_BARCODE_FLAG_OFF = 0;
		/// <summary>お支払い方法がコンビニ受取払いの場合</summary>
		public const int FLG_NEWEB_PAYMENT_CVSCOM_FLAG_ON = 1;
		/// <summary>お支払い方法がコンビニ受取払い以外の場合</summary>
		public const int FLG_NEWEB_PAYMENT_CVSCOM_FLAG_OFF = 0;

		/// <summary>Payment Type FLAPS Credit Card</summary>
		public const string FLG_FLAPS_PAYMENT_TYPE_CASH = "01";
		/// <summary>Payment Type FLAPS Web ATM</summary>
		public const string FLG_FLAPS_PAYMENT_TYPE_CREDIT_CARD = "02";
		/// <summary>Payment Type FLAPS その他</summary>
		public const string FLG_FLAPS_PAYMENT_TYPE_OTHERS = "05";

		/// <summary>ランディングページ：w2_LandingPageDesignAttribute Attribute src</summary>
		public const string FLG_LANDINGPAGEDESIGNATTRIBUTE_ATTRIBUTE_SRC = "src";
		/// <summary>ランディングページ：w2_LandingPageDesignAttribute Attribute background-image</summary>
		public const string FLG_LANDINGPAGEDESIGNATTRIBUTE_ATTRIBUTE_BACKGROUND_IMAGE = "background-image";
		/// <summary>ランディングページ：w2デフォルトデザイン</summary>
		public const string FLG_LANDINGPAGEDESIGN_DESIGN_MODE_DEFAULT = "DEFAULT";
		/// <summary>ランディングページ：独自デザイン</summary>
		public const string FLG_LANDINGPAGEDESIGN_DESIGN_MODE_CUSTOM = "CUSTOM";

		/// <summary>Recommend chatbot use flag invalid</summary>
		public const string FLG_RECOMMEND_CHATBOT_USE_FLG_INVALID = "0";
		/// <summary>Recommend chatbot use flag valid</summary>
		public const string FLG_RECOMMEND_CHATBOT_USE_FLG_VALID = "1";

		/// <summary>Product stock prefix</summary>
		public const string FLG_PRODUCTSTOCK_PREFIX = "productstock_";

		/// <summary>物流連携ステータス：正常</summary>
		public const string FLG_ORDER_LOGI_COOPERATION_STATUS_COMPLETE = "COMP";
		/// <summary>物流連携ステータス：異常</summary>
		public const string FLG_ORDER_LOGI_COOPERATION_STATUS_ERROR = "ERR";

		// 注文拡張項目設定：入力方法
		public const string FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT = "TB"; // テキストボックス
		public const string FLG_ORDEREXTENDSETTING_INPUT_TYPE_DROPDOWN = "DDL"; // ドロップダウンリスト
		public const string FLG_ORDEREXTENDSETTING_INPUT_TYPE_CHECKBOX = "CB"; // チェックボックス
		public const string FLG_ORDEREXTENDSETTING_INPUT_TYPE_RADIO = "RB"; // ラジオボタン
		// 注文拡張項目設定：表示区分
		public const string FLG_ORDEREXTENDSETTING_DISPLAY_PC = "PC"; // PC/SP表示
		public const string FLG_ORDEREXTENDSETTING_DISPLAY_EC = "EC"; // EC表示
		// 注文拡張項目設定：概要の表示区分
		public const string FLG_ORDEREXTENDSETTING_OUTLINE_TEXT = "TEXT"; // TEXT
		public const string FLG_ORDEREXTENDSETTING_OUTLINE_HTML = "HTML"; // HTML
		// 注文拡張項目設定：登録時のみフラグ
		public const string FLG_ORDEREXTENDSETTING_INITONLY = "INITONLY"; // 会員登録のみ
		public const string FLG_ORDEREXTENDSETTING_UPDATABLE = "UPDATABLE"; // 会員登録・編集両方

		// 注文拡張項目 一覧検索用フィールド
		/// <summary>一覧検索 注文拡張項目 項目名</summary>
		public const string SEARCH_FIELD_ORDER_EXTEND_NAME = "order_extend_name";
		/// <summary>一覧検索 注文拡張項目 項目有無</summary>
		public const string SEARCH_FIELD_ORDER_EXTEND_FLG = "order_extend_flg";
		/// <summary>一覧検索 注文拡張項目 選択方法</summary>
		public const string SEARCH_FIELD_ORDER_EXTEND_TYPE = "order_extend_type";
		/// <summary>一覧検索 注文拡張項目 内容</summary>
		public const string SEARCH_FIELD_ORDER_EXTEND_LIKE_ESCAPED = "order_extend_like_escaped";

		/// <summary>Summary report period type: Last 7 days</summary>
		public const string FLG_SUMMARYREPORT_PERIOD_KBN_LAST_SEVEN_DAYS = "L7";
		/// <summary>Summary report period type: This month</summary>
		public const string FLG_SUMMARYREPORT_PERIOD_KBN_THIS_MONTH = "TM";
		/// <summary>Summary report period type: Last month</summary>
		public const string FLG_SUMMARYREPORT_PERIOD_KBN_LAST_MONTH = "LM";
		/// <summary>Summary report period type: This year</summary>
		public const string FLG_SUMMARYREPORT_PERIOD_KBN_THIS_YEAR = "TY";
		/// <summary>Summary report period type: Yesterday</summary>
		public const string FLG_SUMMARYREPORT_PERIOD_KBN_YESTERDAY = "YD";
		/// <summary>Summary report period type: Today</summary>
		public const string FLG_SUMMARYREPORT_PERIOD_KBN_TODAY = "TD";

		/// <summary>Summary report data type: User access</summary>
		public const string FLG_SUMMARYREPORT_DATA_KBN_USER_ACCESS = "USR_VST";
		/// <summary>Summary report data type: Order count</summary>
		public const string FLG_SUMMARYREPORT_DATA_KBN_ORDER_COUNT = "ODR_CNT";
		/// <summary>Summary report data type: Order amount</summary>
		public const string FLG_SUMMARYREPORT_DATA_KBN_ORDER_AMOUNT = "ODR_AMT";
		/// <summary>Summary report data type: Conversion</summary>
		public const string FLG_SUMMARYREPORT_DATA_KBN_CONVERSION = "CNV_REP";
		/// <summary>Summary report data type: Ltv</summary>
		public const string FLG_SUMMARYREPORT_DATA_KBN_LTV = "LTV_REP";
		/// <summary>Summary report data type: Fixed purchase register</summary>
		public const string FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_REGISTER = "FXP_NEW";
		/// <summary>Summary report data type: Fixed purchase cancel</summary>
		public const string FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_CANCEL = "FXP_WDW";
		/// <summary>Summary report data type: User register</summary>
		public const string FLG_SUMMARYREPORT_DATA_KBN_USER_REGISTER = "MBR_NEW";
		/// <summary>Summary report data type: User withdrawal</summary>
		public const string FLG_SUMMARYREPORT_DATA_KBN_USER_WITHDRAWAL = "MBR_WDW";
		/// <summary>Summary report data type: Membership count</summary>
		public const string FLG_SUMMARYREPORT_DATA_KBN_MEMBERSHIP_COUNT = "MBR_CNT";
		/// <summary>Summary report data type: Fixed purchase count</summary>
		public const string FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_COUNT = "FXP_CNT";
		/// <summary> メール配信数 </summary>
		public const string FLG_SUMMARYREPORT_DATA_KBN_SENT_MAIL_COUNT = "SNT_MIL";
		/// <summary> メールクリック数 </summary>
		public const string FLG_SUMMARYREPORT_DATA_KBN_MAIL_CLICK_COUNT = "CLK_MIL";

		/// <summary>Disp summary analysis summary kbn order workflow count </summary>
		public const string FLG_DISPSUMMARYANALYSYS_SUMMARY_KBN_ORDER_WORKFLOW_COUNT = "orderwfcnt";
		/// <summary>Disp summary analysis summary kbn fixed purchase workflow count </summary>
		public const string FLG_DISPSUMMARYANALYSYS_SUMMARY_KBN_FIXED_PURCHASE_WORKFLOW_COUNT = "fpwfcnt";

		/// <summary>Credit only: Take a new credit</summary>
		public const string FLG_CREDIT_MODE_KBN_AUTH_ONLY = "AuthOnly";
		/// <summary>Re-credit: Cancel existing credit and take new credit</summary>
		public const string FLG_CREDIT_MODE_KBN_REAUTH = "Reauth";

		/// <summary>Data migration comman name: Import</summary>
		public const string FLG_DATA_MIGRATION_COMMAND_NAME_IMPORT = "import";
		/// <summary>Data migration comman name: Delete</summary>
		public const string FLG_DATA_MIGRATION_COMMAND_NAME_DELETE = "delete";

		/// <summary>商品決定方法：期間</summary>
		public const string FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_PERIOD = "TERM";
		/// <summary>商品決定方法：回数</summary>
		public const string FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_NUMBER_TIME = "COUNT";

		/// <summary>有効フラグ：TRUE</summary>
		public const string FLG_SUBSCRIPTIONBOX_VALID_TRUE = "1";
		/// <summary>有効フラグ：FALSE</summary>
		public const string FLG_SUBSCRIPTIONBOX_VALID_FALSE = "0";

		/// <summary>自動繰り返し設定：TRUE</summary>
		public const string FLG_SUBSCRIPTIONBOX_AUTO_RENEWAL_TRUE = "1";
		/// <summary>自動繰り返し設定：FALSE</summary>
		public const string FLG_SUBSCRIPTIONBOX_AUTO_RENEWAL_FALSE = "0";

		/// <summary>無期限設定フラグ：TRUE</summary>
		public const string FLG_SUBSCRIPTIONBOX_INDEFINITE_PERIOD_TRUE = "1";
		/// <summary>無期限設定フラグ：FALSE</summary>
		public const string FLG_SUBSCRIPTIONBOX_INDEFINITE_PERIOD_FALSE = "0";

		/// <summary>繰り返し設定：設定なし</summary>
		public const string FLG_SUBSCRIPTIONBOX_RENEWAL_TYPE_NONE = "NONE";
		/// <summary>繰り返し設定：自動繰り返し</summary>
		public const string FLG_SUBSCRIPTIONBOX_RENEWAL_TYPE_AUTO = "AUTO";
		/// <summary>繰り返し設定：最終商品繰り返し</summary>
		public const string FLG_SUBSCRIPTIONBOX_RENEWAL_TYPE_INFINITE = "INFINITE";

		/// <summary>マイページでの商品変更可否：TRUE</summary>
		public const string FLG_SUBSCRIPTIONBOX_ITEMS_CHANGEABLE_BY_USER_TRUE = "1";
		/// <summary>マイページでの商品変更可否：FALSE</summary>
		public const string FLG_SUBSCRIPTIONBOX_ITEMS_CHANGEABLE_BY_USER_FALSE = "0";

		/// <summary>定額設定：TRUE</summary>
		public const string FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_TRUE = "1";
		/// <summary>定額設定：FALSE</summary>
		public const string FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_FALSE = "0";

		/// <summary>頒布会初回選択画面：TRUE</summary>
		public const string FLG_SUBSCRIPTIONBOX_FIRST_SELECTABLE_PAGE_TRUE = "1";
		/// <summary>頒布会初回選択画面：FALSE</summary>
		public const string FLG_SUBSCRIPTIONBOX_FIRST_SELECTABLE_PAGE_FALSE = "0";

		// 商品：頒布会フラグ
		public const string FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID = "0";	// 無効
		public const string FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_VALID = "1";	// 有効
		public const string FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_ONLY = "2";	// 定期購入のみ

		/// <summary>頒布会検索フラグ：有効</summary>
		public const string FLG_PRODUCT_SUBSCRIPTION_BOX_SEARCH_FLG_VALID = "1";
		/// <summary>頒布会検索フラグ：無効</summary>
		public const string FLG_PRODUCT_SUBSCRIPTION_BOX_SEARCH_FLG_INVALID = "0";

		public const string FIELD_HISTORY_ORDER_TYPE = "type";

		public const string FLG_ORDER_FIXEDPURCHASE_WEEK = "fixed_purchase_week";		//fixed_purchase_week
		public const string FLG_ORDER_FIXEDPURCHASE_DAY = "fixed_purchase_day";			//fixed_purchase_day
		public const string FLG_ORDER_FIXEDPURCHASE_MONTH = "fixed_purchase_month";			//fixed_purchase_month
		public const string FLG_ORDER_FIXEDPURCHASE_DISPLAY_COUNT = "fixed_purchase_display_count";		//fixed_purchase_display_count
		public const string FLG_ORDER_FIXEDPURCHASE_DATETIME_COMPARE = "fixed_purchase_datetime_compare";		//fixed_purchase_datetime_compare

		/// <summary>差分取得日時の初期値(この日時よりも後にFLAPS側で更新された商品を取得する)</summary>
		public const string FLG_FLAPS_DEFAULT_CHECKPOINT = "2001-01-01 00:00:00.000";
		/// <summary>連携対象のデータの種類: 商品</summary>
		public const string FLG_FLAPS_REPLICATION_DATA_PRODUCT = "Product";
		/// <summary>連携対象のデータの種類: 在庫</summary>
		public const string FLG_FLAPS_REPLICATION_DATA_PRODUCTSTOCK = "ProductStock";
		/// <summary>FLAPS側に購入された商品が登録されていないことによる注文失敗メッセージ</summary>
		public const string FLG_FLAPS_ORDER_ERROR_MESSAGE_NO_PRODUCT_FOUND = "商品代號不正確";

		/// <summary>Short date time 2 letter format</summary>
		public const string CONST_SHORTDATETIME_2LETTER_FORMAT = "yyyy/MM/dd HH:mm:ss";

		/// <summary>User authentication code const key error</summary>
		public const string CONST_KEY_USER_AUTHENTICATION_CODE = "authentication_code";
		/// <summary>Authentication result flag: Stop process</summary>
		public const string FLG_AUTHENTICATION_RESULT_STOP_PROCESS = "stop_process";
		/// <summary>Authentication result flag: Verification code is incorrect</summary>
		public const string FLG_AUTHENTICATION_RESULT_VERIFICATION_CODE_IS_INCORRECT = "verification_code_is_incorrect";
		/// <summary>Authentication result flag: Verification code has expired</summary>
		public const string FLG_AUTHENTICATION_RESULT_VERIFICATION_CODE_HAS_EXPIRED = "verification_code_has_expired";
		/// <summary>Authentication result flag: Succsess</summary>
		public const string FLG_AUTHENTICATION_RESULT_SUCCSESS = "succsess";

		// ネクストエンジン備考欄に追加内容
		/// <summary>注文メモ</summary>
		public const string NEXTENGINE_REMARKS_ADD_ORDER_MEMO = "order_memo";
		/// <summary>注文拡張項目</summary>
		public const string NEXTENGINE_REMARKS_ADD_ORDER_EXTEND = "order_extend";
		/// <summary>決済注文ID</summary>
		public const string NEXTENGINE_REMARKS_ADD_PAYMENT_ORDER_ID = "payment_order_id";
		/// <summary>決済取引ID</summary>
		public const string NEXTENGINE_REMARKS_ADD_CARD_TRAN_ID = "card_tran_id";
		/// <summary>注文区分</summary>
		public const string NEXTENGINE_REMARKS_ADD_ORDER_KBN = "order_kbn";
		/// <summary>ブランドID</summary>
		public const string NEXTENGINE_REMARKS_ADD_BRAND_ID = "brand_id";

		// 商品拡張項目設定:削除フラグ
		/// <summary>商品拡張項目設定:削除フラグ:ON</summary>
		public const string FLG_PRODUCTEXTENDSETTING_DEL_FLG_ON = "1";
		/// <summary>商品拡張項目設定:削除フラグ:ON</summary>
		public const string FLG_PRODUCTEXTENDSETTING_DEL_FLG_OFF = "0";

		/// <summary>Maximum number of scoring sale questions page is displayed</summary>
		public const int FLG_SCORINGSALE_QUESTION_PAGE_MAX_NUMBER_QUESTIONS_DISPLAY = 10;
		/// <summary>Maximum number of scoring sale questions page item is displayed</summary>
		public const int FLG_SCORINGSALE_QUESTION_PAGE_ITEM_MAX_NUMBER_QUESTIONS_DISPLAY = 5;
		/// <summary>Const scoring sale question question type chart choice amount max</summary>
		public const int CONST_SCORING_SALE_QUESTION_ANSWER_TYPE_CHART_CHOICE_AMOUNT_MAX = 5;
		/// <summary>Const scoring sale question question type chart choice amount max</summary>
		public const int CONST_SCORING_SALE_QUESTION_ANSWER_TYPE_OTHER_CHOICE_AMOUNT_MAX = 50;
		/// <summary>Const score axis column name amount max</summary>
		public const int CONST_SCORE_AXIS_COLUMN_NAME_AMOUNT_MAX = 15;
		/// <summary>Scoring sale page no default</summary>
		public const string CONST_SCORINGSALE_PAGE_NO_DEFAULT = "1";
		/// <summary>Scoring sale branch no default</summary>
		public const string CONST_SCORINGSALE_BRANCH_NO_DEFAULT = "0";
		/// <summary>Scoring sale condition branch no</summary>
		public const string CONST_SCORINGSALE_CONDITION_BRANCH_NO = "0";
		/// <summary>Scoring sale group no default</summary>
		public const string CONST_SCORINGSALE_GROUP_NO_DEFAULT = "0";
		/// <summary>Scoring sale quantity default</summary>
		public const string CONST_SCORINGSALE_QUANTITY_DEFAULT = "0";
		/// <summary>Scoring sale buy type normal</summary>
		public const string CONST_SCORINGSALE_BUY_TYPE_NORMAL = "NORMAL";
		/// <summary>Scoring sale public status: Published</summary>
		public const string FLG_SCORINGSALE_PUBLISH_STATUS_PUBLISHED = "PUBLISHED";
		/// <summary>Scoring sale public status: Unpublished</summary>
		public const string FLG_SCORINGSALE_PUBLISH_STATUS_UNPUBLISHED = "UNPUBLISHED";
		/// <summary>Scoring sale top page use flg: ON</summary>
		public const string FLG_SCORINGSALE_TOP_PAGE_USE_FLG_ON = "ON";
		/// <summary>Scoring sale top page use flg: OFF</summary>
		public const string FLG_SCORINGSALE_TOP_PAGE_USE_FLG_OFF = "OFF";
		/// <summary>Scoring sale radar chart use flg: ON</summary>
		public const string FLG_SCORINGSALE_RADAR_CHART_USE_FLG_ON = "ON";
		/// <summary>Scoring sale radar chart use flg: OFF</summary>
		public const string FLG_SCORINGSALE_RADAR_CHART_USE_FLG_OFF = "OFF";
		/// <summary>Flg scoring sale product recommend buy type normal</summary>
		public const string FLG_SCORINGSALEPRODUCT_BUY_TYPE_NORMAL = "NORMAL";
		/// <summary>Flg scoring sale product recommend buy type fixedpurchase</summary>
		public const string FLG_SCORINGSALEPRODUCT_BUY_TYPE_FIXEDPURCHASE = "FIXEDPURCHASE";
		/// <summary>Scoring sale question type text single</summary>
		public const string FLG_SCORINGSALE_QUESTION_TYPE_TEXT_SINGLE = "TEXT_SINGLE";
		/// <summary>Scoring sale question type text multiple</summary>
		public const string FLG_SCORINGSALE_QUESTION_TYPE_TEXT_MULTIPLE = "TEXT_MULTIPLE";
		/// <summary>Scoring sale question type image text single</summary>
		public const string FLG_SCORINGSALE_QUESTION_TYPE_IMAGE_TEXT_SINGLE = "IMAGE_TEXT_SINGLE";
		/// <summary>Scoring sale question type image text multiple</summary>
		public const string FLG_SCORINGSALE_QUESTION_TYPE_IMAGE_TEXT_MULTIPLE = "IMAGE_TEXT_MULTIPLE";
		/// <summary>Scoring sale question type chart</summary>
		public const string FLG_SCORINGSALE_QUESTION_TYPE_CHART = "CHART";
		/// <summary>Scoring sale question type pulldown</summary>
		public const string FLG_SCORINGSALE_QUESTION_TYPE_PULLDOWN = "PULLDOWN";
		/// <summary>Scoring sale top page use flag on</summary>
		public const string FLG_SCORINGSALE_TOP_PAGE_USE_FLAG_ON = "ON";
		/// <summary>Scoring sale top page use flag off</summary>
		public const string FLG_SCORINGSALE_TOP_PAGE_USE_FLAG_OFF = "OFF";
		/// <summary>Scoring sale result condition or</summary>
		public const string FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_OR = "OR";
		/// <summary>Scoring sale result condition and</summary>
		public const string FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_AND = "AND";
		/// <summary>Scoring sale result condition value</summary>
		public const string FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_VALUE = "VALUE";
		/// <summary>Scoring sale result condition value</summary>
		public const string FLG_SCORINGSALE_RESULT_CONDITION_CONDITION_VALUE_MAX = "VALUE_MAX";
		/// <summary>Scoring sale result condition value min</summary>
		public const string SCORINGSALE_RESULT_CONDITION_CONDITION_VALUE_MIN = "-999";
		/// <summary>Scoring sale result condition value max</summary>
		public const string SCORINGSALE_RESULT_CONDITION_CONDITION_VALUE_MAX = "999";
		/// <summary>Scoring sale design type pc</summary>
		public const string SCORINGSALE_DESIGN_TYPE_PC = "PC";
		/// <summary>Scoring sale design type sp</summary>
		public const string SCORINGSALE_DESIGN_TYPE_SP = "SP";
		/// <summary>Scoring sale top scoring sale_id</summary>
		public const string SCORING_SALE_ID = "@@scoring_sale_id@@";
		/// <summary>Scoring sale top scoring sale_id</summary>
		public const string SCORING_SALE_PAGE_NO = "@@page_no@@";
		/// <summary>Value text param public date kbn</summary>
		public const string VALUETEXT_PARAM_PUBLIC_DATE_KBN = "public_date_kbn";
		/// <summary>Value text scoring sale product recommend condition type</summary>
		public const string VALUETEXT_SCORINGSALE_PRODUCT_RECOMMEND_CONDITION_TYPE = "condition_type";
		/// <summary>Value text scoring sale replacement tag</summary>
		public const string VALUETEXT_SCORINGSALE_REPLACEMENT_TAG = "ReplacementTag";
		/// <summary>Value text scoring sale product recommend condition value</summary>
		public const string VALUETEXT_SCORINGSALE_PRODUCT_RECOMMEND_CONDITION_VALUE = "value_type";

		/// <summary>Const response key result</summary>
		public const string CONST_RESPONSE_KEY_RESULT = "result";
		/// <summary>Const response key result</summary>
		public const string CONST_RESPONSE_KEY_RESULT_OK = "ok";
		/// <summary>Const response key result</summary>
		public const string CONST_RESPONSE_KEY_RESULT_NG = "ng";
		/// <summary>Const response key message</summary>
		public const string CONST_RESPONSE_KEY_MESSAGE = "msg";
		/// <summary>Const response key id</summary>
		public const string CONST_RESPONSE_KEY_ID = "id";
		/// <summary>Request key preview key</summary>
		public const string REQUEST_KEY_PREVIEW_KEY = "previewKey";
		/// <summary>Request key reset</summary>
		public const string REQUEST_KEY_RESET = "reset";

		/// <summary>GMO承認結果: 審査中</summary>
		public const string CONST_RESPONSE_AUTHOR_RESULT_INREVIEW = "審査中";
		/// <summary>GMO承認結果: 入金待機</summary>
		public const string CONST_RESPONSE_AUTHOR_RESULT_DEPOSIT_WAITING = "入金待機";
		/// <summary>GMO承認結果：Alert</summary>
		public const string CONST_RESPONSE_AUTHOR_RESULT_ALERT = "ALERT";
		/// <summary>GMO承認結果：入金待機</summary>
		public const string FLG_ORDER_CREDIT_STATUS_DEPOSIT_WAITING = "DEPOSITWAITING";
		/// <summary>GMO承認結果: 審査中</summary>
		public const string FLG_ORDER_CREDIT_STATUS_INREVIEW = "INREVIEW";

		/// <summary>GMOリクエスト: 8%</summary>
		public const string FLG_GMO_REQUEST_TAX_TYPE_8 = "1";
		/// <summary>GMOリクエスト: 10%</summary>
		public const string FLG_GMO_REQUEST_TAX_TYPE_10 = "2";

		/// <summary>頒布会必須商品設定：TRUE</summary>
		public const string FLG_SUBSCRIPTIONBOX_NECESSARY_VALID = "1";
		/// <summary>頒布会必須商品設定：FALSE</summary>
		public const string FLG_SUBSCRIPTIONBOX_NECESSARY_INVALID = "0";
		/// <summary>店舗（サイト）：全店舗権限フラグ</summary>
		public const string FLG_SHOPSITE_SHOP_SITE_ID_SUPERVISOR = "00";
		/// <summary>店舗（サイト）：有効フラグON</summary>
		public const string FLG_SHOPSITE_VALID_FLG_ON = "1";
		/// <summary>店舗（サイト）：デフォルト値</summary>
		public const string FLG_SHOPSITE_SHOP_SITE_ID_DEFAULT = "01";
		/// <summary>Flag userextend usrex shop app member on</summary>
		public const string FLG_USEREXTEND_USREX_SHOP_APP_MEMBER_FLAG_ON = "1";
		/// <summary>Flag userextend usrex shop app member off</summary>
		public const string FLG_USEREXTEND_USREX_SHOP_APP_MEMBER_FLAG_OFF = "0";

		/// <summary>External import file type: new yahoo order</summary>
		public const string FLG_EXTERNALIMPORT_FILE_TYPE_NEW_YAHOO_ORDER = "ImportNewYahooOrder";

		/// <summary>本会員登録フラグ：未登録</summary>
		public const string FLG_LINETEMPORARYUSER_REGISTRATION_FLAG_INVALID = "0";
		/// <summary>本会員登録フラグ：登録済み</summary>
		public const string FLG_LINETEMPORARYUSER_REGISTRATION_FLAG_VALID = "1";

		/// <summary>定期購入初回配送翌月フラグ: 有効</summary>
		public const string FLG_SHOPSHIPPING_FIXED_PURCHASE_FIRST_SHIPPING_NEXT_MONTH_FLG_VALID = "1";
		/// <summary>定期購入初回配送翌月フラグ: 無効</summary>
		public const string FLG_SHOPSHIPPING_FIXED_PURCHASE_FIRST_SHIPPING_NEXT_MONTH_FLG_INVALID = "0";

		/// <summary>店舗受取可能フラグ:無効</summary>
		public const string FLG_PRODUCT_STOREPICKUP_FLG_INVALID = "0";
		/// <summary>店舗受取可能フラグ:有効</summary>
		public const string FLG_PRODUCT_STOREPICKUP_FLG_VALID = "1";
		/// <summary>店舗受取ステータス：店舗未到着</summary>
		public const string FLG_STOREPICKUP_STATUS_PENDING = "PENDING";
		/// <summary>店舗受取ステータス：店舗到着</summary>
		public const string FLG_STOREPICKUP_STATUS_ARRIVED = "ARRIVED";
		/// <summary>店舗受取ステータス：引き渡し済み</summary>
		public const string FLG_STOREPICKUP_STATUS_DELIVERED = "DELIVERED";
		/// <summary>店舗受取ステータス：返送</summary>
		public const string FLG_STOREPICKUP_STATUS_RETURNED = "RETURNED";

		/// <summary>配送料無料時の請求料金利用フラグ: 有効</summary>
		public const string FLG_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG_VALID = "1";
		/// <summary>配送料無料時の請求料金利用フラグ: 無効</summary>
		public const string FLG_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG_INVALID = "0";

		/// <summary>おすすめタグ・商品表示パーツ表示区分：全て</summary>
		public const string FLG_AWOO_PRODUCT_RECOMMEND_KBN_ALL = "ALL";
		/// <summary>おすすめタグ・商品表示パーツ表示区分：商品</summary>
		public const string FLG_AWOO_PRODUCT_RECOMMEND_KBN_PRODUCT = "PRODUCT";
		/// <summary>おすすめタグ・商品表示パーツ表示区分：タグ</summary>
		public const string FLG_AWOO_PRODUCT_RECOMMEND_KBN_TAG = "TAG";

		/// <summary>クレジットカードトークン</summary>
		public const string CREDIT_CARD_TOKEN = "credit_card_token";
		#endregion
	}
}
