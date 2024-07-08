/*
=========================================================================================================
  Module      : 定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain
{
	/// <summary>
	/// 定数
	/// </summary>
	public class Constants : w2.Database.Common.Constants
	{
		/// <summary>定期購入ＯＰ：有効無効</summary>
		public static bool FIXEDPURCHASE_OPTION_ENABLED = false;

		/// <summary>カード決済：仮クレジットカード決済種別ID</summary>
		public static string PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID = null;

		/// <summary>注文拡張ステータス最大利用数</summary>
		public static int CONST_ORDER_EXTEND_STATUS_USE_MAX = 50;
		/// <summary>注文拡張ステータスをユーザが最大利用する数</summary>
		public static int CONST_ORDER_EXTEND_STATUS_USER_USE_MAX = 30;
		/// <summary>更新アプリケーション種別</summary>
		public static ApplicationType UPDATEHISTORY_APPLICATIONI_TYPE = ApplicationType.Undefined;

		/// <summary>デフォルト店舗ID</summary>
		public const string CONST_DEFAULT_SHOP_ID = "0";
		/// <summary>デフォルト識別ID</summary>
		public const string CONST_DEFAULT_DEPT_ID = "0";
		/// <summary>DateTimeデフォルト値</summary>
		public const string CONST_DEFAULT_DATETIME_VALUE = "0001/01/01 0:00:00";

		/// <summary>最低限出荷に必要な営業日</summary>
		public static int MINIMUM_SHIPMENT_REQUIRED_BUSINESS_DAYS = 1;

		/// <summary>ユーザーポイント履歴：グループ番号デフォルト値</summary>
		public const int CONST_USERPOINTHISTORY_DEFAULT_GROUP_NO = 0;

		/// <summary>CROSS POINT ユーザー拡張項目(店舗カード番号)</summary>
		public static string CROSS_POINT_USREX_SHOP_CARD_NO = string.Empty;
		/// <summary>CROSS POINT ユーザー拡張項目(登録店舗名)</summary>
		public static string CROSS_POINT_USREX_SHOP_ADD_SHOP_NAME = string.Empty;
		/// <summary>CROSS POINT ユーザー拡張項目(店舗カードPIN)</summary>
		public static string CROSS_POINT_USREX_SHOP_CARD_PIN = string.Empty;
		/// <summary>CROSS POINT ユーザー拡張項目(アプリ会員フラグ)</summary>
		public static string CROSS_POINT_USREX_SHOP_APP_MEMBER_FLAG = string.Empty;
		/// <summary>CROSS POINT ユーザー拡張項目(郵便DM不要フラグ)</summary>
		public static string CROSS_POINT_USREX_DM = string.Empty;
		/// <summary>CROSS POINT ユーザー拡張項目(メールDM不要フラグ)</summary>
		public static string CROSS_POINT_USREX_MAIL_FLG = string.Empty;

		/// <summary>フィールド名：設定文字列</summary>
		public const string MASTEREXPORTSETTING_XML_NAME = "name";
		/// <summary>フィールド名：データタイプ</summary>
		public const string MASTEREXPORTSETTING_XML_TYPE = "type";
		/// <summary>マスタ出力定義セッティング</summary>
		public const string FILE_XML_MASTEREXPORTSETTING_SETTING = "Xml/Setting/MasterExportSetting.xml";

		// CMS日付選択ドロップダウン
		/// <summary>日付選択</summary>
		public enum DateSelectType
		{
			/// <summary>未選択</summary>
			Unselected,
			/// <summary>1日以内</summary>
			Day,
			/// <summary>1週間以内</summary>
			Week,
			/// <summary>1ヶ月以内</summary>
			Month,
			/// <summary>3ヶ月以内</summary>
			ThreeMonth,
			/// <summary>3ヶ月以降</summary>
			AfterThreeMonth,
		}

		/// <summary>注文拡張項目 フィールド一覧</summary>
		public static readonly string[] ORDER_EXTEND_ATTRIBUTE_FIELD_LIST = new string[]
		{
			Constants.FIELD_ORDER_ATTRIBUTE1,
			Constants.FIELD_ORDER_ATTRIBUTE2,
			Constants.FIELD_ORDER_ATTRIBUTE3,
			Constants.FIELD_ORDER_ATTRIBUTE4,
			Constants.FIELD_ORDER_ATTRIBUTE5,
			Constants.FIELD_ORDER_ATTRIBUTE6,
			Constants.FIELD_ORDER_ATTRIBUTE7,
			Constants.FIELD_ORDER_ATTRIBUTE8,
			Constants.FIELD_ORDER_ATTRIBUTE9,
			Constants.FIELD_ORDER_ATTRIBUTE10,
		};

		// Order workflow request key
		/// <summary>ワークフロー区分</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_KBN = "owkkbn";
		/// <summary>ワークフロー枝番</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NO = "owno";
		/// <summary>商品ID</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PRODUCT_ID = "owpdid";
		/// <summary>商品名</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PRODUCT_NAME = "owpdnm";
		/// <summary>注文ID</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_ID = "owodid";
		/// <summary>注文ステータス</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_STATUS = "owos";
		/// <summary>注文年(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_UPDATE_YEAR_FROM = "owuyf";
		/// <summary>注文月(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_UPDATE_MONTH_FROM = "owumf";
		/// <summary>注文日(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_UPDATE_DAY_FROM = "owudf";
		/// <summary>ステータス更新日(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_UPDATE_FROM = "owuf";
		/// <summary>注文年(To)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_UPDATE_YEAR_TO = "owuyt";
		/// <summary>注文月(To)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_UPDATE_MONTH_TO = "owumt";
		/// <summary>注文日(To)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_UPDATE_DAY_TO = "owudt";
		/// <summary>ステータス更新日(To)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_UPDATE_TO = "owut";
		/// <summary>外部決済ステータス</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_STATUS = "owexps";
		/// <summary>決済注文ID</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_PAYMENT_ORDER_ID = "owpoid";
		/// <summary>決済取引ID</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_CARD_TRAN_ID = "ctid";
		/// <summary>拡張ステータス枝番</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_NO = "owesn";
		/// <summary>拡張ステータス</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS = "owes";
		/// <summary>拡張ステータス更新日・拡張ステータス枝番</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_UPDATE_DATE_EXTEND_STATUS_NO = "owduesn";
		/// <summary>更新年(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_YEAR_FROM = "owesuyf";
		/// <summary>更新月(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_MONTH_FROM = "owesumf";
		/// <summary>更新日(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_DAY_FROM = "owesudf";
		/// <summary>拡張ステータス更新日(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_FROM = "owesuf";
		/// <summary>更新年(To)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_YEAR_TO = "owesuyt";
		/// <summary>更新月(To)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_MONTH_TO = "owesumt";
		/// <summary>更新日(To)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_DAY_TO = "owesudt";
		/// <summary>拡張ステータス更新日(To)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_TO = "owesut";
		/// <summary>最終与信年(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_AUTH_DATE_YEAR_FROM = "owexpayf";
		/// <summary>最終与信月(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_AUTH_DATE_MONTH_FROM = "owexpamf";
		/// <summary>最終与信日(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_AUTH_DATE_DAY_FROM = "owexpadf";
		/// <summary>最終与信日時(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_AUTH_DATE_FROM = "owexpaf";
		/// <summary>最終与信日(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_AUTH_DATE_YEAR_TO = "owexpayt";
		/// <summary>最終与信月(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_AUTH_DATE_MONTH_TO = "owexpamt";
		/// <summary>最終与信日(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_AUTH_DATE_DAY_TO = "owexpadt";
		/// <summary>最終与信日時(To)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_AUTH_DATE_TO = "owexpat";
		/// <summary>最終与信日指定無し</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_AUTH_DATE_NONE = "owexpadn";
		/// <summary>配送希望日年(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPINGDATE_YEAR_FROM = "owsyf";
		/// <summary>配送希望日月(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPINGDATE_MONTH_FROM = "owsmf";
		/// <summary>配送希望日日(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPINGDATE_DAY_FROM = "owsdf";
		/// <summary>配送希望日(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPINGDATE_FROM = "owsf";
		/// <summary>配送希望日年(To)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPINGDATE_YEAR_TO = "owsyt";
		/// <summary>配送希望日月(To)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPINGDATE_MONTH_TO = "owsmt";
		/// <summary>配送希望日日(To)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPINGDATE_DAY_TO = "owsdt";
		/// <summary>配送希望日(To)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPINGDATE_TO = "owst";
		/// <summary>指定なし含む</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPINGDATE = "owsd";
		/// <summary>ユーザーID</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_USER_ID = "owuid";
		/// <summary>セットプロモーションID</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SETPROMOTION_ID = "owspid";
		/// <summary>ノベルティID</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NOVELTY_ID = "ownvid";
		/// <summary>レコメンドID</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_RECOMMEND_ID = "owrcdid";
		/// <summary>注文メモ</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_MEMO = "owmm";
		/// <summary>決済連携メモ</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PAYMENT_MEMO = "owpmm";
		/// <summary>管理メモ</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_MANAGEMENT_MEMO = "owmmm";
		/// <summary>配送メモ</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_MEMO = "owsmm";
		/// <summary>外部連携メモ</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_RELATION_MEMO = "owrmm";
		/// <summary>注文メモ</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_MEMO_FLG = "owmmf";
		/// <summary>管理メモ</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_MANAGEMENT_MEMO_FLG = "owmmmf";
		/// <summary>配送メモフラグ</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_MEMO_FLG = "owsmmf";
		/// <summary>決済連携メモ</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PAYMENT_MEMO_FLG = "owpmmf";
		/// <summary>外部連携メモ</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_RELATION_MEMO_FLG = "owrmmf";
		/// <summary>ユーザー特記欄</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_USER_MEMO = "owumm";
		/// <summary>ユーザー特記欄フラグ</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_USER_MEMO_FLG = "owummf";
		/// <summary>商品付帯情報</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PRODUCT_OPTION_TEXTS = "owpot";
		/// <summary>商品付帯情報フラグ</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_PRODUCT_OPTION_FLG = "owpof";
		/// <summary>ワークフロー名</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NAME = "ownm";
		/// <summary>有効フラグ</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_VALID_FLG = "owvf";
		/// <summary>ワークフロー詳細区分</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN = "owdkbn";
		/// <summary>追加検索可否FLG</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ADDITIONAL_SEARCH_FLG = "owasf";
		/// <summary>出荷予定日年(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE_YEAR_FROM = "owssyf";
		/// <summary>出荷予定日月(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE_MONTH_FROM = "owssmf";
		/// <summary>出荷予定日日(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE_DAY_FROM = "owssdf";
		/// <summary>出荷予定日(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE_FROM = "owssf";
		/// <summary>出荷予定日年(To)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE_YEAR_TO = "owssyt";
		/// <summary>出荷予定日月(To)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE_MONTH_TO = "owssmt";
		/// <summary>出荷予定日日(To)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE_DAY_TO = "owssdt";
		/// <summary>出荷予定日(To)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE_TO = "owsst";
		/// <summary>出荷予定なし含む</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE = "owssd";
		/// <summary>請求書同梱フラグ</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_INVOICE_BUNDLE_FLG = "owibf";
		/// <summary>領収書希望フラグ</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_RECEIPT_FLG = "owrf";
		/// <summary>發票ステータス</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_TW_INVOICE_STATUS = "owtwis";
		/// <summary>配送先</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ANOTHER_SHIPPING_FLG = "owansf";
		/// <summary>配送状態</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_STATUS = "owss";
		/// <summary>配送先：都道府県</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_PREFECTURE = "owsp";
		/// <summary>市区町村</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_CITY = "owsc";
		/// <summary>注文拡張項目名</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_NAME = "owooen";
		/// <summary>注文拡張項目ありなしフラグ</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_FLG = "owooef";
		/// <summary>注文拡張項目タイプ</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TYPE = "owooety";
		/// <summary>注文拡張項目の検索テキスト</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TEXT = "owooet";
		/// <summary>完了状態コード</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_STATUS_CODE = "owssc";
		/// <summary>現在の状態</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SHIPPING_CURRENT_STATUS = "owscs";
		/// <summary>頒布会コースID</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SUBSCRIPTION_BOX_COURSE_ID = "owssbci";
		/// <summary>頒布会購入回数(出荷基準)(From)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SUBSCRIPTION_BOX_COUNT_FROM = "owessbcf";
		/// <summary>頒布会購入回数(出荷基準)(To)</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SUBSCRIPTION_BOX_COUNT_TO = "owessbct";
		/// <summary>Store pickup status</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_STORE_PICKUP_STATUS = "owsps";

		// Fixed purchase workflow request key
		/// <summary>定期購入ID</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_ID = "fwfid";
		/// <summary>定期購入商品ID</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_PRODUCT_ID = "fwpid";
		/// <summary>定期購入商品名</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_PRODUCT_NAME = "fwpname";
		/// <summary>ユーザーID</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_USER_ID = "fwuid";
		/// <summary>定期購入ステータス</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_STAUS = "fwfs";
		/// <summary>定期購入決済ステータス</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_FIXEDPURCHASE_PAYMENT_STAUS = "fwfps";
		/// <summary>管理メモフラグ</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_MANAGEMENT_MEMO_FLG = "fwmflg";
		/// <summary>管理メモ</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_MANAGEMENT_MEMO = "fwm";
		/// <summary>配送メモフラグ</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_SHIPPING_MEMO_FLG = "fwsmmf";
		/// <summary>配送メモ</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_SHIPPING_MEMO = "fwsmm";
		/// <summary>購入回数(注文基準)(From)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDERED_COUNT_FROM = "fwocf";
		/// <summary>購入回数(注文基準)(To)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDERED_COUNT_TO = "fwoct";
		/// <summary>購入回数(出荷基準)(From)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_SHIPPED_COUNT_FROM = "fwscf";
		/// <summary>購入回数(出荷基準)(To)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_SHIPPED_COUNT_TO = "fwsct";
		/// <summary>作成日(From)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_CREATED_FROM = "fwdcrf";
		/// <summary>作成日(To)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_CREATED_TO = "fwdcrt";
		/// <summary>更新日(From)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_CHANGED_FROM = "fwdchf";
		/// <summary>更新日(To)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_CHANGED_TO = "fwdcht";
		/// <summary>最終購入日(From)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_LAST_ORDERED_FROM = "fwdlof";
		/// <summary>最終購入日(To)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_LAST_ORDERED_TO = "fwdlot";
		/// <summary>購入開始日(From)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_BGN_FROM = "fwdbgnf";
		/// <summary>購入開始日(To)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_BGN_TO = "fwdbgnt";
		/// <summary>次回配送日(From)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_NEXT_SHIPPING_FROM = "fwdnsf";
		/// <summary>次回配送日(To)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_NEXT_SHIPPING_TO = "fwdnst";
		/// <summary>次々回配送日(From)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_NEXT_NEXT_SHIPPING_FROM = "fwdnnsf";
		/// <summary>次々回配送日(To)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_DATE_NEXT_NEXT_SHIPPING_TO = "fwdnnst";
		/// <summary>拡張ステータス枝番</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_NO = "fwpesn";
		/// <summary>拡張ステータス</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS = "fwpes";
		/// <summary>拡張ステータス更新日・ステータス</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_NO_UPDATE_DATE = "fwesnud";
		/// <summary>拡張ステータス更新日:年(From)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_YEAR_FROM = "fwesuyf";
		/// <summary>拡張ステータス更新日:月(From)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_MONTH_FROM = "fwesumf";
		/// <summary>拡張ステータス更新日:(From)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_DAY_FROM = "fwesudf";
		/// <summary>拡張ステータス更新日(From)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_FROM = "fwesuf";
		/// <summary>拡張ステータス更新日:年(To)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_YEAR_TO = "fwesuyt";
		/// <summary>拡張ステータス更新日:月(To)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_MONTH_TO = "fwesumt";
		/// <summary>拡張ステータス更新日:日(To)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_DAY_TO = "fwesudt";
		/// <summary>拡張ステータス更新日(To)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_EXTEND_STATUS_UPDATE_DATE_TO = "fwesut";
		/// <summary>領収書希望フラグ</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_RECEIPT_FLG = "fwrf";
		/// <summary>注文拡張項目名</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_NAME = "owfoen";
		/// <summary>注文拡張項目ありなしフラグ</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_FLG = "owfoef";
		/// <summary>注文拡張項目タイプ</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TYPE = "owfoety";
		/// <summary>注文拡張項目の検索テキスト</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_WORKFLOW_ORDER_EXTEND_TEXT = "owfoet";
		/// <summary>頒布会コースID</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_SUBSCRIPTION_BOX_COURSE_ID = "fwessbci";
		/// <summary>頒布会購入回数(出荷基準)(From)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_SUBSCRIPTION_BOX_COUNT_FROM = "fwessbcf";
		/// <summary>頒布会購入回数(出荷基準)(To)</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_WORKFLOWSETTING_SUBSCRIPTION_BOX_COUNT_TO = "fwessbct";

		//特集ページ
		///<summary>特集ページページタイトルタグ</summary>
		public const string FEATUREPAGE_PAGE_TITLE = "page-title";
		///<summary>特集ページヘッダーバナータグ</summary>
		public const string FEATUREPAGE_HEADER_BANNER = "header-banner";
		///<summary>商品一覧設定タグ</summary>
		public const string FEATUREPAGE_PRODUCR_LIST = "product-list-";
		///<summary>商品一覧グループアイテム</summary>
		public const string FEATUREPAGE_GROUP_ITEMS = "feature-group-items";
		///<summary>特集ページコンテンツエリア上部タグ</summary>
		public const string FEATUREPAGE_UPPER_CONTENTS_AREA = "upper-contents-area";
		///<summary>特集ページコンテンツエリア下部タグ</summary>
		public const string FEATUREPAGE_LOWER_CONTENTS_AREA = "lower-contents-area";
		///<summary>特集ページ商品リスト追加タグ</summary>
		public const string FEATUREPAGE_ADD_PRODUCT_LIST = "add-product-list";

		/// <summary> 採番マスタキー </summary>
		public const string NUMBER_KEY_CS_EXTERNALLINK_ID = "CsExternalLinkId";
	}
}