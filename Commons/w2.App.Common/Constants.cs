/*
=========================================================================================================
  Module      : w2Commerce共通定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.App.Common.Global.Config;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;
using w2.App.Common.RepeatPlusOne.Config;
using w2.Common.Net.Mail;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.User.Helper;
using Braintree;
using w2.App.Common.SubscriptionBox;
using w2.App.Common.Awoo;

namespace w2.App.Common
{
	///*********************************************************************************************
	/// <summary>
	/// 定数/設定値を管理する
	/// </summary>
	///*********************************************************************************************
	public partial class Constants : w2.Domain.Constants
	{
		//========================================================================
		// 各種設定（設定ファイルより読み出しを行い、アプリケーション毎に設定）
		//========================================================================
		/// <summary>コンフィグ設定</summary>
		public static ConfigurationSetting CONFIGURATION_SETTING = null;

		//------------------------------------------------------
		// 現在のバージョン
		//------------------------------------------------------
		public static string CURRENT_VERSION = "";

		//------------------------------------------------------
		// 各種オプション利用有無系
		//------------------------------------------------------
		/// <summary>w2mp識別ID</summary>
		public static string W2MP_DEPT_ID = "";
		/// <summary>w2mpポイントオプション利用有無</summary>
		public static bool W2MP_POINT_OPTION_ENABLED = false;
		/// <summary>w2mpクーポンオプション利用可否</summary>
		public static bool W2MP_COUPON_OPTION_ENABLED = false;
		/// <summary>w2mpアフィリエイトオプション利用可否</summary>
		public static bool W2MP_AFFILIATE_OPTION_ENABLED = false;
		/// <summary>w2mp汎用アフィリエイトオプション利用可否</summary>
		public static bool W2MP_MULTIPURPOSE_AFFILIATE_OPTION_ENABLED = false;
		/// <summary>注文登録（電話注文）ＯＰ：有効無効</summary>
		public static bool ORDERREGIST_OPTION_ENABLED = false;
		/// <summary>定期購入カート同梱設定</summary>
		public static bool FIXEDPURCHASE_OPTION_CART_SEPARATION = false;
		/// <summary>定期購入：次回配送日計算モード</summary>
		public static NextShippingCalculationMode FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE = NextShippingCalculationMode.Monthly;
		/// <summary>ギフト購入ＯＰ：有効無効</summary>
		public static bool GIFTORDER_OPTION_ENABLED = false;
		/// <summary>カート複製ＯＰ：有効無効</summary>
		public static bool CARTCOPY_OPTION_ENABLED = false;
		/// <summary>カート 配送方法が未確定時(ユーザが未選択)の優先する配送方法 対応配送先{ EXPRESS, MAIL }</summary>
		public static string CART_SHIPPINGMETHOD_UNSELECTED_PRIORITY = "EXPRESS";
		/// <summary>ノベルティＯＰ：有効無効</summary>
		public static bool NOVELTY_OPTION_ENABLED = false;
		/// <summary>レコメンド設定ＯＰ：有効無効</summary>
		public static bool RECOMMEND_OPTION_ENABLED = false;
		/// <summary>商品グループＯＰ：有効無効</summary>
		public static bool PRODUCTGROUP_OPTION_ENABLED = false;
		/// <summary>モール連携ＯＰ：有効無効</summary>
		public static bool MALLCOOPERATION_OPTION_ENABLED = false;
		/// <summary>商品同梱ＯＰ：有効無効</summary>
		public static bool PRODUCTBUNDLE_OPTION_ENABLED = false;
		/// <summary>商品レビュー機能利用有無</summary>
		public static bool PRODUCTREVIEW_ENABLED = false;
		/// <summary>商品レビュー即時反映</summary>
		public static bool PRODUCTREVIEW_AUTOOPEN_ENABLED = false;
		/// <summary>商品レビュー投稿ポイント機能利用有無</summary>
		public static bool REVIEW_REWARD_POINT_ENABLED = false;
		/// <summary>レビュー投稿ポイント付与制限</summary>
		public static string REVIEW_REWARD_POINT_GRANT_LIMIT = "";
		/// <summary>商品セール利用有無</summary>
		public static bool PRODUCT_SALE_OPTION_ENABLED = false;
		/// <summary>商品セット設定有無</summary>
		public static bool PRODUCT_SET_OPTION_ENABLED = false;
		/// <summary>セットプロモーションＯＰ：有効無効</summary>
		public static bool SETPROMOTION_OPTION_ENABLED = false;
		/// <summary>会員ランクＯＰ：有効無効</summary>
		public static bool MEMBER_RANK_OPTION_ENABLED = false;
		/// <summary>ユーザー統合ＯＰ：有効無効</summary>
		public static bool USERINTEGRATION_OPTION_ENABLED = false;
		/// <summary>商品ブランド：有効無効</summary>
		public static bool PRODUCT_BRAND_ENABLED = false;
		/// <summary>デジタルコンテンツＯＰ：有効無効</summary>
		public static bool DIGITAL_CONTENTS_OPTION_ENABLED = false;
		/// <summary>リアル店舗OP：有効無効</summary>
		public static bool REALSHOP_OPTION_ENABLED = false;
		/// <summary>update point error mail OP</summary>
		public static bool UPDATE_POINT_ERROR_MAIL_OPTION_ENABLED = false;
		/// <summary>実在庫利用設定有無</summary>
		public static bool REALSTOCK_OPTION_ENABLED = false;
		/// <summary>注文同梱ＯＰ：有効無効</summary>
		public static bool ORDER_COMBINE_OPTION_ENABLED = false;
		/// <summary>定期台帳注文同梱ＯＰ：有効無効</summary>
		public static bool FIXED_PURCHASE_COMBINE_OPTION_ENABLED = false;
		/// <summary>領収書発行ＯＰ：有効無効</summary>
		public static bool RECEIPT_OPTION_ENABLED = false;
		/// <summary>領収書発行しない決済区分一覧</summary>
		public static List<string> NOT_OUTPUT_RECEIPT_PAYMENT_KBN = new List<string>();
		/// <summary>DM発送履歴OP：有効無効</summary>
		public static bool DM_SHIPPING_HISTORY_OPTION_ENABLED = false;
		/// <summary>XMLサイトマップ管理画面:有効無効</summary>
		public static bool SITEMAP_OPTION_ENABLED = true;
		/// <summary>受注管理の在庫連動可否</summary>
		public static bool ORDERMANAGEMENT_STOCKCOOPERATION_ENABLED = false;
		/// <summary> メール配信オプション：メール配信オプション利用有無 </summary>
		public static bool MARKETINGPLANNER_MAIL_OPTION_ENABLE = false;

		/// <summary>EC Option Enable</summary>
		public static bool EC_OPTION_ENABLED = false;
		/// <summary>MP Option Enable</summary>
		public static bool MP_OPTION_ENABLED = false;
		/// <summary>CSオプション利用可否</summary>
		public static bool CS_OPTION_ENABLED = false;
		/// <summary>CMSオプション利用可否</summary>
		public static bool CMS_OPTION_ENABLED = false;

		/// <summary>送り状発行CSV利用有無</summary>
		public static bool INVOICECSV_ENABLED = false;
		///<summary>
		/// 管理画面デザイン設定
		///		W2Unified の場合「」（空文字）
		///		W2Repeat の場合「Repeat」
		///		W2RepeatFood の場合「RepeatFood」
		///		その他 の場合「OEM」
		/// </summary>
		public static string MANAGER_DESIGN_SETTING = "";
		/// <summary>管理画面デザイン 装飾アイコンファイル名（""：表記なし（本番など）、"icon_site_test.svg"：テストサイト表記</summary>
		public static string MANAGER_DESIGN_DECORATE_ICON_FILENAME = "";
		/// <summary>管理画面デザイン 装飾文字列（""：表記なし（本番など）、"テスト環境"：テストサイト表記）</summary>
		public static string MANAGER_DESIGN_DECORATE_MESSAGE = "";

		/// <summary>ピッキングリスト出力利用有無</summary>
		public static bool PDF_OUTPUT_PICKINGLIST_ENABLED = false;
		/// <summary>受注明細書出力利用有無</summary>
		public static bool PDF_OUTPUT_ORDERSTATEMENT_ENABLED = false;

		/// <summary>ポイントオプション：仮ポイント有効無効</summary>
		public static bool MARKETINGPLANNER_USE_TEMPORARY_POINT = false;
		/// <summary>ポイントオプション：本ポイント自動付与</summary>
		public static bool GRANT_ORDER_POINT_AUTOMATICALLY = false;
		/// <summary>ポイントオプション：税抜ポイント利用可否</summary>
		public static bool POINT_OPTION_USE_TAX_EXCLUDED_POINT = false;
		/// <summary>ポイントオプション：税抜金額端数処理方法</summary>
		public static string POINT_OPTION_TAXEXCLUDED_FRACTION_ROUNDING = "";
		/// <summary>ポイントルール：クリックポイント有効無効</summary>
		public static bool POINTRULE_OPTION_CLICKPOINT_ENABLED = false;
		/// <summary>汎用アフィリエイトオプション：税抜金額端数処理方法</summary>
		public static string MULTIPURPOSE_AFFILIATE_OPTION_TAXEXCLUDED_FRACTION_ROUNDING = "";
		/// <summary>税処理金額端数処理方法</summary>
		public static string TAX_EXCLUDED_FRACTION_ROUNDING = "";

		/// <summary>税込み管理フラグ</summary>
		public static bool MANAGEMENT_INCLUDED_TAX_FLAG = true;
		/// <summary>海外配送税込み請求フラグ</summary>
		public static bool GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG = true;

		/// <summary>商品URLの空パラメーター出力フラグ</summary>
		public static bool FRONT_PRODUCTURL_OMIT_EMPTY_QUERYPARAMETER = false;

		/// <summary>楽天ファイル取り込み設定有無</summary>
		public static bool EXTERNAL_IMPORT_OPTION_ENABLED = false;
		/// <summary>商品検索ワードランキング：有効無効</summary>
		public static bool W2MP_PRODUCT_SEARCHWORD_RANKING_ENABLED = false;
		/// <summary>モバイルサイト有無</summary>
		public static bool MOBILEOPTION_ENABLED = false;
		/// <summary>モバイルデータの表示と非表示</summary>
		public static bool DISPLAYMOBILEDATAS_OPTION_ENABLED = false;
		/// <summary>GoogleAnalytics：利用可否</summary>
		public static bool GOOGLEANALYTICS_ENABLED = false;
		/// <summary>GoogleShopping連携：利用可否</summary>
		public static bool GOOGLESHOPPING_COOPERATION_OPTION_ENABLED = false;
		/// <summary>スマートフォンオプション：利用可否</summary>
		public static bool SMARTPHONE_OPTION_ENABLED = false;
		/// <summary>メールアドレスのログインID利用有無</summary>
		public static bool LOGIN_ID_USE_MAILADDRESS_ENABLED = true;
		/// <summary>Frontサイト管理者向け注文完了メール通知：利用可否</summary>
		public static bool THANKSMAIL_FOR_OPERATOR_ENABLED = false;
		/// <summary>商品一覧：在庫無し非表示検索デフォルト設定</summary>
		public static bool UNDISPLAY_NOSTOCK_PRODUCT_ENABLED = false;
		/// <summary>商品一覧：在庫無し商品をうしろに回す設定</summary>
		public static bool DISPLAY_NOSTOCK_PRODUCT_BOTTOM = false;
		/// <summary>商品バリエーション画像レイヤー表示可否</summary>
		public static bool LAYER_DISPLAY_VARIATION_IMAGES_ENABLED = false;
		/// <summary>商品バリエーション画像レイヤー表示範囲設定</summary>
		public static bool LAYER_DISPLAY_NOVARIATION_UNDISPLAY_ENABLED = false;
		/// <summary>商品バリエーション画像レイヤー表示：グループ指定</summary>
		public static string LAYER_DISPLAY_VARIATION_GROUP_NAME = "";
		/// <summary>配送料の別途見積もり表示利用有無</summary>
		public static bool SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED = false;
		/// <summary>バリエーションが1つの場合商品詳細画面でバリエーション選択済みにするかどうか</summary>
		public static bool PRODUCTDETAIL_VARIATION_SINGLE_SELECTED = false;
		/// <summary>商品一覧に商品バリエーションを表示するかどうか</summary>
		public static bool PRODUCTLIST_VARIATION_DISPLAY_ENABLED = false;
		/// <summary>おすすめ商品に商品バリエーションを表示するか</summary>
		public static bool PRODUCT_RECOMMEND_VARIATION_DISPLAY_ENABLED = false;

		/// <summary>入荷通知メール：入荷通知メールの通知期限デフォルト設定</summary>
		public static int DATE_EXPIRED_ADD_MONTH_ARRIVAL = 0;
		/// <summary>入荷通知メール：販売開始通知メールの通知期限デフォルト設定</summary>
		public static int DATE_EXPIRED_ADD_MONTH_RELEASE = 0;
		/// <summary>入荷通知メール：再販売通知メールの通知期限デフォルト設定</summary>
		public static int DATE_EXPIRED_ADD_MONTH_RESALE = 0;
		/// <summary>在庫減少アラートメール：通知メール送信の最低閾値デフォルト設定</summary>
		public static int STOCK_ALERT_MAIL_THRESHOLD = 0;
		/// <summary>メール送信承諾：ユーザー拡張項目名リスト</summary>
		public static string FAVORITE_PRODUCT_DECREASE_MAILSENDFLG_USEREXRTEND_COLUMNNAME = "";

		/// <summary>定期会員になる条件に、定期注文が入金済みになったかの条件を含めるか</summary>
		public static bool FIXEDPURCHASE_MEMBER_CONDITION_INCLUDES_ORDER_PAYMENT_STATUS_COMPLETE = false;

		/// <summary>商品購入制限利用可否</summary>
		public static bool PRODUCT_ORDER_LIMIT_ENABLED = false;
		/// <summary>商品購入制限利用区分</summary>
		public static ProductOrderLimitKbn? PRODUCT_ORDER_LIMIT_KBN_CAN_BUY = null;

		/// <summary>EFOオプション：EFO案件番号</summary>
		public static string EFO_OPTION_PROJECT_NO = "";

		/// <summary>ドロップダウン年の並び順(ASC：昇順、DESC：降順　※生年月日の年は降順固定となります)</summary>
		public static YearListItemOrder YEAR_LIST_ITEM_ORDER = YearListItemOrder.ASC;
		/// <summary>ドロップダウン年の並び順の種類</summary>
		public enum YearListItemOrder
		{
			/// <summary>昇順</summary>
			ASC,
			/// <summary>降順</summary>
			DESC
		}
		/// <summary>受注ワークフロー自動実行オプション</summary>
		public static bool ORDERWORKFLOW_AUTOEXEC_OPTION_ENABLE = false;

		/// <summary> SKU単位でのお気に入りに対応するか </summary>
		public static bool VARIATION_FAVORITE_CORRESPONDENCE = false;

		/// <summary> 定期の全ポイント継続利用オプション </summary>
		public static bool FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT_ALL_OPTION_ENABLE = false;

		/// <summary>定期商品変更オプション</summary>
		public static bool FIXEDPURCHASE_PRODUCTCHANGE_ENABLED = false;

		// 日別出荷予測機能を利用するかどうか
		public static bool SHIPTMENT_FORECAST_BY_DAYS_ENABLED = false;

		/// <summary>商品一覧：モーダル画面を利用するか</summary>
		public static bool USE_MODAL_PRODUCT_LIST = false;
		/// <summary>商品一覧：無限ロードを利用するか</summary>
		public static bool USE_INFINITE_LOAD_PRODUCT_LIST = false;
		/// <summary>商品一覧：無限ロード利用時、画面内に表示する商品数</summary>
		public static int DISPLAY_PRODUCT_COUNT_FOR_INFINITE_LOAD = 0;

		//------------------------------------------------------
		// W2MP設定
		//------------------------------------------------------
		/// <summary>アクセスログ：有効無効</summary>
		public static bool W2MP_ACCESSLOG_ENABLED = false;
		/// <summary>アクセスログ：ログ取得トラッカーパス</summary>
		public static string W2MP_ACCESSLOG_TRACKER_PATH = "";
		/// <summary>アクセスログ：getlogパス</summary>
		public static string W2MP_ACCESSLOG_GETLOG_PATH = "";
		/// <summary>アクセスログ：利用開始日時</summary>
		public static DateTime W2MP_ACCESSLOG_BEGIN_DATETIME = DateTime.Parse("2100/01/01");
		/// <summary>アクセスログ：アカウントID</summary>
		public static string W2MP_ACCESSLOG_ACCOUNT_ID = "";
		/// <summary>アクセスログ：対象ドメイン</summary>
		public static string W2MP_ACCESSLOG_TARGET_DOMAIN = "";

		//------------------------------------------------------
		// 決済設定系
		//------------------------------------------------------
		/// <summary>カード決済区分(SBPS、ゼウス、)</summary>
		public static PaymentCard? PAYMENT_CARD_KBN = null;
		/// <summary>カードオーソリ後入金ステータスを「入金済」にするか</summary>
		public static bool PAYMENT_CARD_PATMENT_STAUS_COMPLETE = true;
		/// <summary>カードオーソリ後入金ステータスを「入金済」にするか(デジタルコンテンツ用)</summary>
		public static bool PAYMENT_CARD_PATMENT_STAUS_COMPLETE_FORDIGITALCONTENTS = false;
		/// <summary>カード決済：返品時クレジットカードの自動売上確定</summary>
		public static bool PAYMENT_SETTING_CREDIT_RETURN_AUTOSALES_ENABLED = false;
		/// <summary>決済：決済手数料の計算に商品小計のみを使用する</summary>
		public static bool CALCULATE_PAYMENT_PRICE_ONLY_SUBTOTAL = true;
		/// <summary>クレジットカード決済エラー詳細を表示する</summary>
		public static bool CREDITCARD_ERROR_DETAILS_DISPLAY_ENABLED = false;
		/// <summary>コンビニ後払い請求書全商品表示</summary>
		public static bool PAYMENT_CVS_DEF_INVOICE_ALL_ITEM_DISPLAYED = false;

		/// <summary>ソフトバンクペイメント：API接続先URL</summary>
		public static string PAYMENT_SETTING_SBPS_API_URL = "";
		/// <summary>ソフトバンクペイメント：リンク決済URL</summary>
		public static string PAYMENT_SETTING_SBPS_ORDER_LINK_URL = "";
		/// <summary>ソフトバンクペイメント：リンク決済URL（ローカルテスト用）</summary>
		public static string PAYMENT_SETTING_SBPS_ORDER_LINK_URL_LOCALTEST = "";
		/// <summary>ソフトバンクペイメント：リンク型カード登録URL</summary>
		public static string PAYMENT_SETTING_SBPS_CARD_REGSITER_ORDER_LINK_URL = "";
		/// <summary>ソフトバンクペイメント：リンク型カード登録URL（ローカルテスト用）</summary>
		public static string PAYMENT_SETTING_SBPS_CARD_REGSITER_ORDER_LINK_URL_LOCALTEST = "";
		/// <summary>ソフトバンクペイメント：マーチャントID</summary>
		public static string PAYMENT_SETTING_SBPS_MERCHANT_ID = "";
		/// <summary>ソフトバンクペイメント：サービスID</summary>
		public static string PAYMENT_SETTING_SBPS_SERVICE_ID = "";
		/// <summary>ソフトバンクペイメント：ハッシュキー</summary>
		public static string PAYMENT_SETTING_SBPS_HASHKEY = "";
		/// <summary>ソフトバンクペイメント：3DES暗号化キー</summary>
		public static string PAYMENT_SETTING_SBPS_3DES_KEY = "";
		/// <summary>ソフトバンクペイメント：3DES初期化キー</summary>
		public static string PAYMENT_SETTING_SBPS_3DES_IV = "";
		/// <summary>ソフトバンクペイメント：BASIC認証ID</summary>
		public static string PAYMENT_SETTING_SBPS_BASIC_AUTHENTICATION_ID = "";
		/// <summary>ソフトバンクペイメント：BASIC認証PASSWORD</summary>
		public static string PAYMENT_SETTING_SBPS_BASIC_AUTHENTICATION_PASSWORD = "";
		/// <summary>ソフトバンクペイメント：注文商品ID（必須）</summary>
		public static string PAYMENT_SETTING_SBPS_ITEM_ID = "";
		/// <summary>ソフトバンクペイメント：注文商品名</summary>
		public static string PAYMENT_SETTING_SBPS_ITEM_NAME = "";
		/// <summary>ソフトバンクペイメント：決済後入金ステータスを「入金済」にするか（カード以外）</summary>
		public static bool PAYMENT_SETTING_SBPS_PAYMENT_STATUS_COMPLETE = false;
		/// <summary>ソフトバンクペイメント：クレジットトークン取得JS URL</summary>
		public static string PAYMENT_SETTING_SBPS_CREDIT_GETTOKEN_JS_URL = "";
		/// <summary>ソフトバンクペイメント：クレジットセキュリティコード対応</summary>
		public static bool PAYMENT_SETTING_SBPS_CREDIT_SECURITYCODE = false;
		/// <summary>ソフトバンクペイメント：クレジット分割支払い対応</summary>
		public static bool PAYMENT_SETTING_SBPS_CREDIT_DIVIDE = false;
		/// <summary>Rakuten Payment: Credit installment payment support</summary>
		public static bool PAYMENT_SETTING_RAKUTEN_CREDIT_DIVIDE = false;
		/// <summary>ソフトバンクペイメント：クレジット決済方法 与信後決済：PAYMENT_AFTER_AUTH、自動売上／手動売上：PAYMENT_WITH_AUTH</summary>
		public static PaymentCreditCardPaymentMethod? PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD = null;
		/// <summary>ソフトバンクペイメント：クレジット決済方法（デジタルコンテンツ用） 与信後決済：PAYMENT_AFTER_AUTH、自動売上／手動売上：PAYMENT_WITH_AUTH</summary>
		public static PaymentCreditCardPaymentMethod? PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS = null;
		/// <summary>ソフトバンクペイメント：クレジット3Dセキュア利用</summary>
		public static bool PAYMENT_SETTING_SBPS_CREDIT_3DSECURE = false;
		/// <summary>フトバンクペイメント：WEBコンビニ支払期限(日)</summary>
		public static int? PAYMENT_SETTING_SBPS_CVS_PAYMENT_LIMIT_DAY = null;
		/// <summary>楽天ペイ（オンライン決済）V2を利用するかどうか</summary>
		public static bool PAYMENT_SETTING_SBPS_RAKUTENIDV2_ENABLED = false;
		/// <summary>
		/// WFでの出荷報告時の外部決済ST
		/// TRUE：外部決済STを出荷報告済みにする
		/// FALSE：外部決済STを売り上げ確定済みにする（V5.12でのデフォルト動作）
		/// </summary>
		public static bool EXTERNAL_PAYMENT_STATUS_SHIPCOMP_ORDERWORKFLOW_EXTERNALSHIPMENTACTION = true;

		/// <summary>ヤマトKWC：加盟店コード</summary>
		public static string PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE = "";
		/// <summary>ヤマトKWC：アクセスキー</summary>
		public static string PAYMENT_SETTING_YAMATO_KWC_ACCESS_KEY = "";
		/// <summary>ヤマトKWC：トークン取得JS URL</summary>
		public static string PAYMENT_SETTING_YAMATO_KWC_CREDIT_GETTOKEN_JS_URL = "";

		/// <summary>ヤマトKWC：URLタイプ</summary>
		public static PaymentYamatoKwcApiUrlSetting.UrlType PAYMENT_SETTING_YAMATO_KWC_API_URL_TYPE = PaymentYamatoKwcApiUrlSetting.UrlType.Product;
		/// <summary>ヤマトKWC：商品名</summary>
		public static string PAYMENT_SETTING_YAMATO_KWC_GOODS_NAME = "";
		/// <summary>ヤマトKWC：クレジット分割支払い対応</summary>
		public static bool PAYMENT_SETTING_YAMATO_KWC_CREDIT_DIVIDE = false;
		/// <summary>ヤマトKWC：クレジットキャンセル対応</summary>
		public static bool PAYMENT_SETTING_YAMATO_KWC_CREDIT_CANCEL_ENABLED = false;
		/// <summary>ヤマトKWC：オフライン用ダミーメールアドレス</summary>
		public static string PAYMENT_SETTING_YAMATO_KWC_DUMMY_MAILADDRESS = "";
		/// <summary>ヤマトKWC：クレジットセキュリティコード対応</summary>
		public static bool PAYMENT_SETTING_YAMATO_KWC_CREDIT_SECURITYCODE = false;
		///<summary>ヤマトKWC：3Dセキュア2.0対応</summary>
		public static bool PAYMENT_SETTING_YAMATO_KWC_3DSECURE = false;

		/// <summary>Zeus：決済サーバURL(SecureLink用)</summary>
		public static string PAYMENT_SETTING_ZEUS_SECURE_LINK_SERVER_URL = "";
		/// <summary>Zeus：決済サーバURL(SecureAPI用)</summary>
		public static string PAYMENT_SETTING_ZEUS_SECURE_API_AUTH_SERVER_URL = "";
		/// <summary>Zeus：LINKPOINT利用</summary>
		public static bool PAYMENT_SETTING_ZEUS_USE_LINKPOINT_ENABLED = false;
		/// <summary>Zeus：決済サーバURL(LinkPointテスト用)</summary>
		public static string PAYMENT_SETTING_ZEUS_LINKPOINT_SERVER_URL_LOCALTEST = "";
		/// <summary>Zeus：決済サーバURL(LinkPoint用)</summary>
		public static string PAYMENT_SETTING_ZEUS_LINKPOINT_SERVER_URL = "";
		/// <summary>Zeus：決済加盟店IPコード</summary>
		public static string PAYMENT_SETTING_ZEUS_CLIENT_IP = "";
		/// <summary>Zeus：決済加盟店IPコード（オフライン注文）</summary>
		public static string PAYMENT_SETTING_ZEUS_CLIENT_IP_OFFLINE = "";
		/// <summary>Zeus：タブレットUserAgentパターンリスト</summary>
		public static string PAYMENT_SETTING_ZEUS_TABLET_USERAGENT_PATTERN = "";
		/// <summary>Zeus：決済加盟店認証キー</summary>
		public static string PAYMENT_SETTING_ZEUS_SECURE_API_AUTH_KEY = "";
		/// <summary>Zeus：分割支払い対応</summary>
		public static bool PAYMENT_SETTING_ZEUS_DIVIDE = false;
		/// <summary>Zeus：決済方法 与信後決済/仮売り・本売り(PAYMENT_AFTER_AUTH,PAYMENT_WITH_AUTH)</summary>
		public static PaymentCreditCardPaymentMethod? PAYMENT_SETTING_ZEUS_PAYMENTMETHOD = null;
		/// <summary>Zeus：決済方法（デジタルコンテンツ用）与信後決済/仮売り・本売り(PAYMENT_AFTER_AUTH,PAYMENT_WITH_AUTH)</summary>
		public static PaymentCreditCardPaymentMethod? PAYMENT_SETTING_ZEUS_PAYMENTMETHOD_FORDIGITALCONTENTS = null;
		/// <summary>Zeus：セキュリティコード対応</summary>
		public static bool PAYMENT_SETTING_ZEUS_SECURITYCODE = false;
		/// <summary>Zeus：3Dセキュア対応</summary>
		public static bool PAYMENT_SETTING_ZEUS_3DSECURE = false;
		/// <summary>Zeus：3Dセキュア対応2.0</summary>
		public static bool PAYMENT_SETTING_ZEUS_3DSECURE2 = false;

		/// <summary>GMO：決済サーバURL</summary>
		public static string PAYMENT_SETTING_GMO_AUTH_SERVER_URL = "";
		/// <summary>GMO：サイトID</summary>
		public static string PAYMENT_SETTING_GMO_SITE_ID = "";
		/// <summary>GMO：サイトパスワード</summary>
		public static string PAYMENT_SETTING_GMO_SITE_PASS = "";
		/// <summary>GMO：ショップID</summary>
		public static string PAYMENT_SETTING_GMO_SHOP_ID = "";
		/// <summary>GMO：ショップパスワード</summary>
		public static string PAYMENT_SETTING_GMO_SHOP_PASS = "";
		/// <summary>GMO：決済区分</summary>
		public static GmoCreditCardPaymentMethod? PAYMENT_SETTING_GMO_PAYMENTMETHOD = null;
		/// <summary>GMO：セキュリティコード対応</summary>
		public static bool PAYMENT_SETTING_GMO_SECURITYCODE = false;
		/// <summary>GMO：Token取得JS</summary>
		public static string PAYMENT_CREDIT_GMO_GETTOKEN_JS = "";
		/// <summary>GMO：決済サーバURL</summary>
		public static string PAYMENT_SETTING_GMO_CVS_AUTH_SERVER_URL = "";
		/// <summary>GMO：ショップID</summary>
		public static string PAYMENT_SETTING_GMO_CVS_SHOP_ID = "";
		/// <summary>GMO：ショップパスワード</summary>
		public static string PAYMENT_SETTING_GMO_CVS_SHOP_PASS = "";
		/// <summary>GMO:コンビニ支払期限</summary>
		public static string PAYMENT_SETTING_GMO_CVS_PAYMENT_LIMIT_DAY = "";
		/// <summary>GMO:お問い合わせ先</summary>
		public static string PAYMENT_SETTING_GMO_CVS_RECEIPTS_DISP_11 = "";
		/// <summary>GMO:お問い合わせ先電話番号</summary>
		public static string PAYMENT_SETTING_GMO_CVS_RECEIPTS_DISP_12 = "";
		/// <summary>GMO:お問合せ先受付時間</summary>
		public static string PAYMENT_SETTING_GMO_CVS_RECEIPTS_DISP_13 = "";
		/// <summary>GMO:Execute access transaction</summary>
		public static string PAYMENT_SETTING_GMO_CVS_ENTRY_TRAN = "";
		/// <summary>GMO:Execute transaction convenience</summary>
		public static string PAYMENT_SETTING_GMO_CVS_EXEC_TRAN = "";
		/// <summary>GMO:Execute cancel convenience</summary>
		public static string PAYMENT_SETTING_GMO_CVS_CANCEL = "";
		/// <summary>Gmo convenience type</summary>
		public static string PAYMENT_GMO_CVS_TYPE = "gmo_cvs_type";
		/// <summary>Gmo：3Dセキュア対応</summary>
		public static bool PAYMENT_SETTING_GMO_3DSECURE = false;
		/// <summary>Gmo：ショップ名</summary>
		public static string PAYMENT_SETTING_GMO_SHOP_NAME = "";
		/// <summary>GMO：3DS2.0未対応時取り扱い</summary>
		public static string PAYMENT_SETTING_GMO_TDS2_TYPE = "";

		/// <summary>ベリトランス：Token取得</summary>
		public static string PAYMENT_CREDIT_VERITRANS4G_GETTOKEN = "";
		/// <summary>ベリトランス：セキュリティーコード対応</summary>
		public static bool PAYMENT_SETTING_CREDIT_VERITRANS4G_SECURITYCODE = true;
		/// <summary>ベリトランス：トークンAPIキー</summary>
		public static string PAYMENT_CREDIT_VERITRANS4G_TOKEN_API_KEY = "";
		/// <summary>ベリトランス：3Dセキュア利用</summary>
		public static bool PAYMENT_VERITRANS4G_CREDIT_3DSECURE = false;
		/// <summary>ベリトランス：3Dセキュア2.0利用</summary>
		public static bool PAYMENT_CREDIT_VERITRANS_3DSECURE2 = false;
		/// <summary>ベリトランス：log4g設定ファイルパス</summary>
		public static string PAYMENT_VERITRANS4G_LOG4G_CONFIG_PATH = "";
		/// <summary>ベリトランス：ベリトランス設定ファイルパス</summary>
		public static string PAYMENT_VERITRANS4G_MDK_CONFIG_PATH = "";
		/// <summary>ベリトランス：決済方法 即時売上：CAPTURE、仮売上/実売上：AUTH</summary>
		public static VeritransCreditCardPaymentMethod? PAYMENT_SETTING_VERITRANS4G_PAYMENTMETHOD = null;
		/// <summary>ベリトランス：決済方法（デジタルコンテンツ用）即時売上：CAPTURE、仮売上/実売上：AUTH</summary>
		public static VeritransCreditCardPaymentMethod? PAYMENT_SETTING_VERITRANS4G_PAYMENTMETHOD_FORDIGITALCONTENTS = null;
		/// <summary>ベリトランス後払い：請求書同梱フラグ</summary>
		public static bool PAYMENT_SETTING_VERITRANS_USE_INVOICE_BUNDLE = false;
		/// <summary>ベリトランスPaypay：注文商品名</summary>
		public static string PAYMENT_PAYPAY_VERITRANS4G_ITEMNAME = string.Empty;
		/// <summary>ベリトランスPaypay：注文商品ID（必須）</summary>
		public static string PAYMENT_PAYPAY_VERITRANS4G_ITEMID = string.Empty;

		/// <summary>PayTg：PayTgを利用するか</summary>
		public static bool PAYMENT_SETTING_PAYTG_ENABLED = false;
		/// <summary>PayTg：モックか</summary>
		public static bool PAYMENT_SETTING_PAYTG_MOCK_ENABLED = false;
		/// <summary>PayTg：ベースURL</summary>
		public static string PAYMENT_SETTING_PAYTG_BASEURL = "";
		/// <summary>PayTg：クレジット登録用URL（ベリトランス）</summary>
		public static string PAYMENT_SETTING_PAYTG_REGISTCREDITURL = "";
		/// <summary>PayTg：クレジット端末確認用URL</summary>
		public static string PAYMENT_SETTING_PAYTG_DEVICE_STATUS_CHECK_URL = "";
		/// <summary>PayTg: クレジットカード番号デフォルト値</summary>
		public static string PAYMENT_SETTING_PAYTG_DEFAULT_CARD_NUMBER = "";
		/// <summary>PayTg: クレジットカード名義人デフォルト値</summary>
		public static string PAYMENT_SETTING_PAYTG_DEFAULT_AUTHOR_NAME = "";
		/// <summary>PayTg: クレジットカード有効期限デフォルト値：月</summary>
		public static string PAYMENT_SETTING_PAYTG_DEFAULT_EXPIRATION_MONTH = "";
		/// <summary>PayTg: クレジットカード有効期限デフォルト値：年</summary>
		public static string PAYMENT_SETTING_PAYTG_DEFAULT_EXPIRATION_YEAR = "";
		/// <summary>PayTg: ベリトランス⇔payTG連携のMock</summary>
		public static string PAYMENT_SETTING_PAYTG_MOCK_URL_FOR_VERITRANS4G = "";

		/// <summary>コンビニ決済区分（DSK、SBPS、、、）</summary>
		public static PaymentCvs? PAYMENT_CVS_KBN = null;

		/// <summary>電算システムコンビニ決済：決済URL</summary>
		public static string PAYMENT_SETTING_DSK_SERVER_URL = "";
		/// <summary>電算システムコンビニ決済：テナントID</summary>
		public static string PAYMENT_SETTING_DSK_TENANT_ID = "";
		/// <summary>電算システムコンビニ決済：パスワード</summary>
		public static string PAYMENT_SETTING_DSK_PASSWORD = "";
		/// <summary>電算システムコンビニ決済：支払期限</summary>
		public static int PAYMENT_SETTING_DSK_CVS_PAYMENT_LIMIT = 0;

		/// <summary>銀行振込決済区分（互換性のために残す）</summary>
		public static PaymentEbank? PAYMENT_EBANK_KBN = null;

		/// <summary>S!まとめて支払い：決済確定要求先URL</summary>
		public static string PAYMENT_SETTING_SMATOMETE_SERVER_URL_DECISION = "";
		/// <summary>S!まとめて支払い：加盟店コード</summary>
		public static string PAYMENT_SETTING_SMATOMETE_SHOP_ID = "";
		/// <summary>S!まとめて支払い：接続時ベーシック認証 ID</summary>
		public static string PAYMENT_SETTING_SMATOMETE_CONNECT_USER_ID = "";
		/// <summary>S!まとめて支払い：接続時ベーシック認証 パスワード</summary>
		public static string PAYMENT_SETTING_SMATOMETE_CONNECT_PASSWORD = "";
		/// <summary>S!まとめて支払い連動</summary>
		public static bool PAYMENT_SETTING_SMATOMETE_REALSALES_ENABLED = false;

		/// <summary>ドコモケータイ払い：確定要求サーバURL</summary>
		public static string PAYMENT_SETTING_DOCOMOKETAI_SERVER_URL_DECISION = "";
		/// <summary>ドコモケータイ払い：加盟店コード</summary>
		public static string PAYMENT_SETTING_DOCOMOKETAI_SHOP_CODE = "";
		/// <summary>ドコモケータイ払い：加盟店パスワード</summary>
		public static string PAYMENT_SETTING_DOCOMOKETAI_SHOP_PASSWORD = "";
		/// <summary>ドコモケータイ払い連動</summary>
		public static bool PAYMENT_SETTING_DOCOMOKETAI_REALSALES_ENABLED = false;

		/// <summary>SBPS ソフトバンク・ワイモバイルまとめて支払い：売上要求</summary>
		public static bool PAYMENT_SETTING_SBPS_SOFTBANKKETAI_REALSALES_ENABLED = false;
		/// <summary>SBPS ドコモケータイタイ払い：売上要求</summary>
		public static bool PAYMENT_SETTING_SBPS_DOCOMOKETAI_REALSALES_ENABLED = false;
		/// <summary>SBPS ａｕかんたん決済：売上要求</summary>
		public static bool PAYMENT_SETTING_SBPS_AUKANTAN_REALSALES_ENABLED = false;
		/// <summary>SBPS リクルートかんたん支払い：売上要求</summary>
		public static bool PAYMENT_SETTING_SBPS_RECRUIT_REALSALES_ENABLED = false;
		/// <summary>SBPS 楽天ペイ：売上要求</summary>
		public static bool PAYMENT_SETTING_SBPS_RAKUTEN_ID_REALSALES_ENABLED = false;

		/// <summary>SBPS ソフトバンク・ワイモバイルまとめて支払い：キャンセル要求</summary>
		public static bool PAYMENT_SETTING_SBPS_SOFTBANKKETAI_CANCEL_ENABLED = false;
		/// <summary>SBPS ドコモケータイタイ払い：キャンセル要求</summary>
		public static bool PAYMENT_SETTING_SBPS_DOCOMOKETAI_CANCEL_ENABLED = false;
		/// <summary>SBPS ａｕかんたん決済：キャンセル要求</summary>
		public static bool PAYMENT_SETTING_SBPS_AUKANTAN_CANCEL_ENABLED = false;
		/// <summary>SBPS Paypal：キャンセル要求</summary>
		public static bool PAYMENT_SETTING_SBPS_PAYPAL_CANCEL_ENABLED = false;
		/// <summary>SBPS リクルートかんたん支払い：キャンセル要求</summary>
		public static bool PAYMENT_SETTING_SBPS_RECRUIT_CANCEL_ENABLED = false;
		/// <summary>SBPS 楽天ペイ：キャンセル要求</summary>
		public static bool PAYMENT_SETTING_SBPS_RAKUTEN_ID_CANCEL_ENABLED = false;

		/// <summary>後払い区分（Yamato、、、）</summary>
		public static PaymentCvsDef? PAYMENT_CVS_DEF_KBN = null;

		/// <summary>ヤマト後払い：API URL</summary>
		public static string PAYMENT_SETTING_YAMATO_KA_API_URL = "";
		/// <summary>ヤマト後払い：加盟店コード</summary>
		public static string PAYMENT_SETTING_YAMATO_KA_TRADER_CODE = "";
		/// <summary>ヤマト後払い：加盟店パスワード</summary>
		public static string PAYMENT_SETTING_YAMATO_KA_TRADER_PASSWORD = "";
		/// <summary>ヤマト後払い：パートナー社識別コード</summary>
		public static string PAYMENT_SETTING_YAMATO_KA_CART_CODE = "";
		/// <summary>ヤマト後払い：出荷予定日期間</summary>
		public static int PAYMENT_SETTING_YAMATO_KA_SHIPPING_TERM_PLAN = 1;
		/// <summary>ヤマト後払い：商品名</summary>
		public static string PAYMENT_SETTING_YAMATO_KA_ITEM_NAME = "";
		/// <summary>ヤマト後払い：請求書同梱</summary>
		public static bool PAYMENT_SETTING_YAMATO_KA_INVOICE_BUNDLE = false;
		/// <summary>ヤマト後払い：開発環境？</summary>
		public static bool PAYMENT_SETTING_YAMATO_KA_DEVELOP = false;

		/// <summary>ヤマト後払いSMS認証連携</summary>
		public static PaymentSmsDef? PAYMENT_SMS_DEF_KBN = null;
		/// <summary>ヤマト後払いSMS認証連携：：決済タイプSMSを定期購入で利用するか</summary>
		public static bool PAYMENT_SETTING_YAMATO_KA_SMS_USE_FIXEDPURCHASE = false;

		/// <summary>Amazonログイン連携オプション</summary>
		public static bool AMAZON_LOGIN_OPTION_ENABLED = false;
		/// <summary>Amazonペイメントオプション</summary>
		public static bool AMAZON_PAYMENT_OPTION_ENABLED = false;
		/// <summary>AmazonペイメントCv2(Checkout Version2)</summary>
		public static bool AMAZON_PAYMENT_CV2_ENABLED = false;
		/// <summary>AmazonペイメントCV2の配送先を注文者として登録するかどうか （TRUE/FALSE）</summary>
		public static bool AMAZON_PAYMENT_CV2_USE_SHIPPING_AS_OWNER_ENABLED = false;
		/// <summary>Amazonペイメント：セラーID</summary>
		public static string PAYMENT_AMAZON_SELLERID = "";
		/// <summary>Amazonペイメント：AWSアクセスキー</summary>
		public static string PAYMENT_AMAZON_AWSACCESSKEY = "";
		/// <summary>Amazonペイメント：AWSシークレット</summary>
		public static string PAYMENT_AMAZON_AWSSECRET = "";
		/// <summary>Amazonペイメント：クライアントID</summary>
		public static string PAYMENT_AMAZON_CLIENTID = "";
		/// <summary>Amazonペイメント：公開鍵ID</summary>
		public static string PAYMENT_AMAZON_PUBLIC_KEY_ID = "";
		/// <summary>Amazonペイメント：秘密鍵</summary>
		public static string PAYMENT_AMAZON_PRIVATE_KEY = "";
		/// <summary>Amazonペイメント：サンドボックス</summary>
		public static bool PAYMENT_AMAZON_ISSANDBOX = false;
		/// <summary>Amazonペイメント：ウィジェットスクリプト</summary>
		public static string PAYMENT_AMAZON_WIDGETSSCRIPT = "";
		/// <summary>Amazonペイメント：即時売上かどうか TRUE:即時売上 FALSE:仮売上げ</summary>
		public static bool PAYMENT_AMAZON_PAYMENTCAPTURENOW = false;
		/// <summary>Amazonペイメント：返品時Amazon Payの自動売上確定</summary>
		public static bool PAYMENT_AMAZON_PAYMENT_RETURN_AUTOSALES_ENABLED = true;
		/// <summary>Amazonペイメント：決済連携後に入金済みにするかどうか TRUE:入金済みにする FALSE:入金済みにしない</summary>
		public static bool PAYMENT_AMAZON_PAYMENTSTATUSCOMPLETE = false;
		/// <summary>Amazonペイメント：Auto Pay上限金額(1定期契約/月)</summary>
		public static decimal PAYMENT_AMAZON_AUTO_PAY_USABLE_PRICE_MAX = 50000;
		/// <summary>Amazonペイメント：取引情報に記載するサイト（店舗）名</summary>
		public static string PAYMENT_AMAZON_STORENAME = "";
		/// <summary>Amazonペイメント：かな補完名（姓）</summary>
		public static string PAYMENT_AMAZON_NAMEKANA1 = "";
		/// <summary>Amazonペイメント：かな補完名（名）</summary>
		public static string PAYMENT_AMAZON_NAMEKANA2 = "";
		/// <summary>Amazonペイメント：事業者様ごとの出品者ID</summary>
		public static string PAYMENT_AMAZON_PLATFORMID = "";
		/// <summary>Amazonペイメント：APIレスポンスログの出力 TRUE:ログ出力する FALSE:ログ出力しない</summary>
		public static bool PAYMENT_AMAZON_WRITE_API_RESPONSE_LOG = false;
		/// <summary>AmazonペイメントCV2：APIレスポンスログの出力 TRUE:ログ出力する FALSE:ログ出力しない</summary>
		public static bool PAYMENT_AMAZON_CV2_WRITE_API_RESPONSE_LOG = false;
		/// <summary>請求先住所取得オプション</summary>
		public static bool AMAZONPAYMENTCV2_USEBILLINGASOWNER_ENABLED = false;

		/// <summary>GMO後払い：認証ID</summary>
		public static string PAYMENT_SETTING_GMO_DEFERRED_AUTHENTICATIONID = "";
		/// <summary>GMO後払い：加盟店コード</summary>
		public static string PAYMENT_SETTING_GMO_DEFERRED_SHOPCODE = "";
		/// <summary>GMO後払い：接続パスワード</summary>
		public static string PAYMENT_SETTING_GMO_DEFERRED_CONNECTPASSWORD = "";
		/// <summary>GMO後払い：基本認証ユーザーID</summary>
		public static string PAYMENT_SETTING_GMO_DEFERRED_BASICUSERID = "";
		/// <summary>GMO後払い：基本認証パスワード</summary>
		public static string PAYMENT_SETTING_GMO_DEFERRED_BASICPASSWORD = "";
		/// <summary>GMO後払い：取引登録URL</summary>
		public static string PAYMENT_SETTING_GMO_DEFERRED_URL_ORDERREGISTER = "";
		/// <summary>GMO後払い：与信審査結果取得URL</summary>
		public static string PAYMENT_SETTING_GMO_DEFERRED_URL_GETAUTHRESULT = "";
		/// <summary>GMO後払い：請求書印字データ取得URL</summary>
		public static string PAYMENT_SETTING_GMO_DEFERRED_URL_GETINVOICEPRINTDATA = "";
		/// <summary>GMO後払い：取引修正・キャンセルURL</summary>
		public static string PAYMENT_SETTING_GMO_DEFERRED_URL_ORDERMODIFYCANCEL = "";
		/// <summary>GMO後払い：請求減額URL</summary>
		public static string PAYMENT_SETTING_GMO_DEFERRED_URL_REDUCESALES = "";
		/// <summary>GMO後払い：出荷報告URL</summary>
		public static string PAYMENT_SETTING_GMO_DEFERRED_URL_SHIPMENT = "";
		/// <summary>GMO後払い：出荷報告修正・キャンセルURL</summary>
		public static string PAYMENT_SETTING_GMO_DEFERRED_URL_SHIPMENTMODIFYCANCEL = "";
		/// <summary>GMO後払い：出荷報告時の運送会社</summary>
		public static string PAYMENT_SETTING_GMO_DEFERRED_PDCOMPANYCODE = "";
		/// <summary>GMO後払い：入金状況確認URL</summary>
		public static string PAYMENT_SETTING_GMO_DEFERRED_URL_GETDEFPAYMENTSTATUS = string.Empty;
		/// <summary>GMO後払い：返品時のAPIインタフェース表示</summary>
		public static bool PAYMENT_SETTING_GMO_DEFERRED_DISPLAYRETURNAPI = false;
		/// <summary>GMO後払い：請求書同梱</summary>
		public static bool PAYMENT_SETTING_GMO_DEFERRED_INVOICEBUNDLE = false;
		/// <summary>GMO後払い：HTTPヘッダ情報のPOST</summary>
		public static bool PAYMENT_SETTING_GMO_DEFERRED_ENABLE_HTTPHEADERS_POST = false;
		/// <summary>GMO後払い：請求書同梱</summary>
		public static bool PAYMENT_SETTING_GMO_GETDEFPAYMENTSTATUS_OPTION = false;
		/// <summary>GMO deferred payment: invoice reissue URL</summary>
		public static string PAYMENT_SETTING_GMO_DEFERRED_URL_REISSUE = string.Empty;

		/// <summary>PayPalログイン＆決済：有効か</summary>
		public static bool PAYPAL_LOGINPAYMENT_ENABLED = false;
		/// <summary>PayPal：ゲートウェイ（クライアントトークン取得以外用）</summary>
		public static BraintreeGateway PAYMENT_PAYPAL_GATEWAY = null;
		/// <summary>PayPal：ゲートウェイ（クライアントトークン取得用）</summary>
		public static BraintreeGateway PAYMENT_PAYPAL_GATEWAY_FOR_GET_CLIENTTOKEN = null;
		/// <summary>PayPal決済区分</summary>
		public static PayPalPaymentMethod PAYPAL_PAYMENT_METHOD = PayPalPaymentMethod.AUTH;
		/// <summary>PayPalパートナーボタンソースコード</summary>
		public static string PAYPAL_PAYMENT_BNCODE = "";
		/// <summary>ソーシャルプロバイダID連携：PayPal CustomerId格納用ユーザー拡張項目名</summary>
		public static string PAYPAL_USEREXTEND_COLUMNNAME_CUSTOMER_ID = "";
		/// <summary>ソーシャルプロバイダID連携：PayPal 連携情報（メールアドレス）格納用ユーザー拡張項目名</summary>
		public static string PAYPAL_USEREXTEND_COLUMNNAME_COOPERATION_INFOS = "";

		/// <summary>後付款(TriLink後払い)：API接続先URL</summary>
		public static string PAYMENT_SETTING_TRILINK_AFTERPAY_API_URL = "";
		/// <summary>後付款(TriLink後払い)：サイトコード</summary>
		public static string PAYMENT_SETTING_TRILINK_AFTERPAY_SITE_CODE = "";
		/// <summary>後付款(TriLink後払い)：APIキー</summary>
		public static string PAYMENT_SETTING_TRILINK_AFTERPAY_API_KEY = "";
		/// <summary>後付款(TriLink後払い)：シークレットキー</summary>
		public static string PAYMENT_SETTING_TRILINK_AFTERPAY_SECRET_KEY = "";
		/// <summary>後付款(TriLink後払い)：出荷報告時の運送会社</summary>
		public static string PAYMENT_SETTING_TRILINK_AFTERPAY_DELIVERY_COMPANY_CODE = "";

		/// <summary>アトディーネ：ショップコード</summary>
		public static string PAYMENT_SETTING_ATODENE_SHOP_CODE = "";
		/// <summary>アトディーネ：リンクID</summary>
		public static string PAYMENT_SETTING_ATODENE_LINK_ID = "";
		/// <summary>アトディーネ：パスワード</summary>
		public static string PAYMENT_SETTING_ATODENE_LINK_PASSWORD = "";
		/// <summary>アトディーネ：運送会社コード</summary>
		public static string PAYMENT_SETTING_ATODENE_DELIVERY_COMPANY_CODE = "";
		/// <summary>アトディーネ：基本認証ユーザーID</summary>
		public static string PAYMENT_SETTING_ATODENE_BASIC_USER_ID = "";
		/// <summary>アトディーネ：基本認証パスワード</summary>
		public static string PAYMENT_SETTING_ATODENE_BASIC_PASSWORD = "";
		/// <summary>アトディーネ：取引登録URL</summary>
		public static string PAYMENT_SETTING_ATODENE_URL_ORDERREGISTER = "";
		/// <summary>アトディーネ：与信結果取得URL</summary>
		public static string PAYMENT_SETTING_ATODENE_URL_GETAUTHRESULT = "";
		/// <summary>アトディーネ：請求書印字データ取得URL</summary>
		public static string PAYMENT_SETTING_ATODENE_URL_GETINVOICEPRINTDATA = "";
		/// <summary>アトディーネ：取引変更取消URL</summary>
		public static string PAYMENT_SETTING_ATODENE_URL_ORDERMODIFYCANCEL = "";
		/// <summary>アトディーネ：出荷報告URL</summary>
		public static string PAYMENT_SETTING_ATODENE_URL_SHIPMENT = "";
		/// <summary>アトディーネ：明細名：小計</summary>
		public static string PAYMENT_SETTING_ATODENE_DETAIL_NAME_SUBTOTAL = "";
		/// <summary>アトディーネ：明細名：送料</summary>
		public static string PAYMENT_SETTING_ATODENE_DETAIL_NAME_SHIPPING = "";
		/// <summary>アトディーネ：明細名：決済手数料</summary>
		public static string PAYMENT_SETTING_ATODENE_DETAIL_NAME_PAYMENT = "";
		/// <summary>アトディーネ：明細名：割引等</summary>
		public static string PAYMENT_SETTING_ATODENE_DETAIL_NAME_DISCOUNT_ETC = "";
		/// <summary>アトディーネ：明細名：その他ご購入商品</summary>
		public static string PAYMENT_SETTING_ATODENE_DETAIL_NAME_PURCHASED_ITEM_ETC = "";
		/// <summary>アトディーネ：請求書同梱サービスフラグ</summary>
		public static bool PAYMENT_SETTING_ATODENE_USE_INVOICE_BUNDLE_SERVICE = false;
		/// <summary>アトディーネ：受注明細連携 </summary>
		public static bool PAYMENT_SETTING_ATODENE_ORDER_DETAIL_COOPERATION = false;
		/// <summary>アトディーネ：受注明細連携：商品名 </summary>
		public static string PAYMENT_SETTING_ATODENE_ORDER_DETAIL_COOPERATION_PRODUCTNAME = "";
		/// <summary>アトディーネ：受注明細連携：商品ID </summary>
		public static string PAYMENT_SETTING_ATODENE_ORDER_DETAIL_COOPERATION_PRODUCTID = "";

		/// <summary>Zcom：基本認証ユーザーID</summary>
		public static string PAYMENT_CREDIT_ZCOM_BASIC_USER_ID = "";
		/// <summary>Zcom：基本認証パスワード</summary>
		public static string PAYMENT_CREDIT_ZCOM_BASIC_PASSWORD = "";
		/// <summary>Zcom：決済API</summary>
		public static string PAYMENT_CREDIT_ZCOM_APIURL_DIRECTPAYMENT = "";
		/// <summary>Zcom：決済取消しAPI</summary>
		public static string PAYMENT_CREDIT_ZCOM_APIURL_CANCELPAYMENT = "";
		/// <summary>Zcom：実売り上げAPI</summary>
		public static string PAYMENT_CREDIT_ZCOM_APIURL_SALESPAYMENT = "";
		/// <summary>Zcom：契約コード</summary>
		public static string PAYMENT_CREDIT_ZCOM_APICONTACTCODE = "";
		/// <summary>Zcom：API言語ID</summary>
		public static string PAYMENT_CREDIT_ZCOM_APILANGID = "";
		/// <summary>Zcom：API課金区分 現在1（都度課金）のみ</summary>
		public static string PAYMENT_CREDIT_ZCOM_APIMISSIONCODE = "";
		/// <summary>Zcom：API仮売り即時判断フラグ 1：仮売り 1以外：即売上</summary>
		public static string PAYMENT_CREDIT_ZCOM_APIADDINFO1 = "";
		/// <summary>Zcom：API仮売り即時判断フラグ(デジタルコンテンツ用) 1：仮売り 1以外：即売上</summary>
		public static string PAYMENT_CREDIT_ZCOM_APIADDINFO1_FORDIGITALCONTENTS = "";
		/// <summary>Zcom：セキュリティーコード対応</summary>
		public static bool PAYMENT_SETTING_CREDIT_ZCOM_SECURITYCODE = false;
		/// <summary>Zcom：通貨コード</summary>
		public static string PAYMENT_SETTING_CREDIT_ZCOM_CURRENCYCODE = "";
		/// <summary>Zcom：通貨フォーマット</summary>
		public static string PAYMENT_SETTING_CREDIT_ZCOM_CURRENCYFORMAT = "";
		/// <summary>Zcom：決済区分「st_code」</summary>
		public static ZcomStCode? PAYMENT_SETTING_CREDIT_ZCOM_STCODE = null;

		/// <summary>ソニーペイメントe-SCOTT：token取得JSのURL</summary>
		public static string PAYMENT_SETTING_SONYPAYMENT_ESCOTT_GETTOKEN_JS_URL = string.Empty;
		/// <summary>ソニーペイメントe-SCOTT：店舗コード</summary>
		public static string PAYMENT_SETTING_SONYPAYMENT_ESCOTT_TENANTID = string.Empty;
		/// <summary>ソニーペイメントe-SCOTT：Master and Process and 電文取り消し用URL</summary>
		public static string PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MASTERANDPROCESSANDRECOVER_URL = string.Empty;
		/// <summary>ソニーペイメントe-SCOTT：トークンProcessURL</summary>
		public static string PAYMENT_SETTING_SONYPAYMENT_ESCOTT_TOKENPAYMENTPROCESS_URL = string.Empty;
		/// <summary>ソニーペイメントe-SCOTT：会員登録URL</summary>
		public static string PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MEMBERREGISTER_URL = string.Empty;
		/// <summary>ソニーペイメントe-SCOTT：トークン認証コード</summary>
		public static string PAYMENT_SETTING_SONYPAYMENT_ESCOTT_TOKENPAYMENTAUTHCODE = string.Empty;
		/// <summary>ソニーペイメントe-SCOTT：マーチャントID</summary>
		public static string PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MERCHANTID = string.Empty;
		/// <summary>ソニーペイメントe-SCOTT：マーチャントパスワード</summary>
		public static string PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MERCHANTPASSWORD = string.Empty;
		/// <summary>ソニーペイメントe-SCOTT：分割支払い対応</summary>
		public static bool PAYMENT_SETTING_SONYPAYMENT_ESCOTT_DIVID = false;
		/// <summary>ソニーペイメントe-SCOTT：仮売り</summary>
		public static PaymentCreditCardPaymentMethod? PAYMENT_SETTING_SONYPAYMENT_ESCOTT_PAYMENTMETHOD = null;
		/// <summary>ソニーペイメントe-SCOTT：仮売り</summary>
		public static PaymentCreditCardPaymentMethod? PAYMENT_SETTING_SONYPAYMENT_ESCOTT_PAYMENTMETHOD_FORDIGITALCONTENTS = null;
		/// <summary>ソニーペイメントe-SCOTT：会員パスワード</summary>
		public static string PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MEMBER_PASSWORD = string.Empty;
		/// <summary>ソニーペイメントe-SCOTT：会員削除を消す期限(FROM)</summary>
		public static int PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MEMBER_DELETED_DAY_FROM = 0;
		/// <summary>ソニーペイメントe-SCOTT：会員削除を消す期限(TO)</summary>
		public static int PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MEMBER_DELETED_DAY_TO = 0;

		/// <summary>DSK後払い：加盟店ID</summary>
		public static string PAYMENT_SETTING_DSK_DEFERRED_SHOPCODE = "";
		/// <summary>DSK後払い：接続元ID</summary>
		public static string PAYMENT_SETTING_DSK_DEFERRED_TERMINAI_ID = "";
		/// <summary>DSK後払い：ダイレクトパスワード</summary>
		public static string PAYMENT_SETTING_DSK_DEFERRED_SHOP_PASSWORD = "";
		/// <summary>DSK後払い：注文情報登録URL</summary>
		public static string PAYMENT_SETTING_DSK_DEFERRED_URL_ORDERREGISTER = "";
		/// <summary>DSK後払い：注文情報修正URL</summary>
		public static string PAYMENT_SETTING_DSK_DEFERRED_URL_ORDERMODIFY = "";
		/// <summary>DSK後払い：与信結果取得URL</summary>
		public static string PAYMENT_SETTING_DSK_DEFERRED_URL_GETAUTHRESULT = "";
		/// <summary>DSK後払い：注文キャンセルURL</summary>
		public static string PAYMENT_SETTING_DSK_DEFERRED_URL_ORDERCANCEL = "";
		/// <summary>DSK後払い：請求書印字データ取得URL</summary>
		public static string PAYMENT_SETTING_DSK_DEFERRED_URL_GETINVOICEPRINTDATA = "";
		/// <summary>DSK後払い：発送情報登録URL</summary>
		public static string PAYMENT_SETTING_DSK_DEFERRED_URL_SHIPMENT = "";
		/// <summary>DSK後払い：受注明細連携 </summary>
		public static bool PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_COOPERATION = false;
		/// <summary>DSK後払い：受注明細：小計</summary>
		public static string PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_NAME_SUBTOTAL = "";
		/// <summary>DSK後払い：受注明細：送料</summary>
		public static string PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_NAME_SHIPPING = "";
		/// <summary>DSK後払い：受注明細：決済手数料</summary>
		public static string PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_NAME_PAYMENT = "";
		/// <summary>DSK後払い：受注明細：割引等</summary>
		public static string PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_NAME_DISCOUNT_ETC = "";
		/// <summary>DSK後払い：受注明細：その他ご購入商品</summary>
		public static string PAYMENT_SETTING_DSK_DEFERRED_ORDER_DETAIL_NAME_ITEM_ETC = "";
		/// <summary>DSK後払い：請求書同梱フラグ</summary>
		public static bool PAYMENT_SETTING_DSK_DEFERRED_USE_INVOICE_BUNDLE = false;

		/// <summary>再与信有効設定</summary>
		public static bool PAYMENT_REAUTH_ENABLED = false;
		/// <summary>再与信可能の注文サイト区分一覧</summary>
		public static List<string> PAYMENT_REAUTH_ORDER_SITE_KBN = new List<string>();

		/// <summary> クレジットカードの未登録カード名（デフォルト） </summary>
		public static string CREDITCARD_UNREGIST_DEFAULT_DISPLAY_NAME = "未登録の決済カード";
		/// <summary> クレジットカードの未登録カード名（定期の場合） </summary>
		public static string CREDITCARD_UNREGIST_FIXEDPURCHASE_DISPLAY_NAME = "FixedPurchase";
		/// <summary>配送料の計算処理で、配送方法が宅配便の場合に参照するサイズ重量区分</summary>
		public static string SHIPPINGPRICE_SIZE_FOR_EXPRESS = "";
		/// <summary>決済種別選択タイプ（RB ：ラジオボタン DDL：ドロップダウン）</summary>
		public static string PAYMENT_CHOOSE_TYPE = "RB";
		/// <summary>LP決済種別選択タイプ（RB ：ラジオボタン DDL：ドロップダウン）</summary>
		public static string PAYMENT_CHOOSE_TYPE_LP = "RB";
		/// <summary>LP決済種別選択タイプ表示制御オプション</summary>
		public static bool PAYMENT_CHOOSE_TYPE_LP_OPTION = false;

		/// <summary>Payment Setting Atobaraicom Registation Url</summary>
		public static string PAYMENT_SETTING_ATOBARAICOM_REGISTATION_URL = string.Empty;
		/// <summary>Payment Setting Atobaraicom Cancelation Url</summary>
		public static string PAYMENT_SETTING_ATOBARAICOM_CANCELATION_URL = string.Empty;
		/// <summary>Payment Setting Atobaraicom Modification Url</summary>
		public static string PAYMENT_SETTING_ATOBARAICOM_MODIFICATION_URL = string.Empty;
		/// <summary>Payment Setting Atobaraicom All Modification Url</summary>
		public static string PAYMENT_SETTING_ATOBARAICOM_ALL_MODIFICATION_URL = string.Empty;
		/// <summary>Payment Setting Atobaraicom Enterprised</summary>
		public static string PAYMENT_SETTING_ATOBARAICOM_ENTERPRISED = string.Empty;
		/// <summary>Payment Setting Atobaraicom Site</summary>
		public static string PAYMENT_SETTING_ATOBARAICOM_SITE = string.Empty;
		/// <summary>Payment Setting Atobaraicom Api User Id</summary>
		public static string PAYMENT_SETTING_ATOBARAICOM_API_USER_ID = string.Empty;
		/// <summary>Atobaraicom register invoice api url</summary>
		public static string PAYMENT_ATOBARAICOM_SHIPPING_APIURL = string.Empty;
		/// <summary>Atobaraicom transfer print queue api url</summary>
		public static string PAYMENT_ATOBARAICOM_TRANSFER_PRINT_QUEUE_APIURL = string.Empty;
		/// <summary>Atobaraicom get list target invoice api url</summary>
		public static string PAYMENT_ATOBARAICOM_GET_LIST_TARGET_INVOICE_APIURL = string.Empty;
		/// <summary>Atobaraicom invoice process execute api url</summary>
		public static string PAYMENT_ATOBARAICOM_INVOICE_PROCESS_EXECUTE_APIURL = string.Empty;
		/// <summary>Atobaraicom use invoice bundle service</summary>
		public static bool PAYMENT_SETTING_ATOBARAICOM_USE_INVOICE_BUNDLE_SERVICE = false;
		/// <summary>Atobaraicom get order status api url</summary>
		public static string PAYMENT_ATOBARAICOM_GET_ORDER_STATUS_APIURL = string.Empty;
		/// <summary>Atobaraicom get authorize status api url</summary>
		public static string PAYMENT_ATOBARAICOM_GET_AUTHORIZE_STATUS_APIURL = string.Empty;
		/// <summary>Atobaraicom max request get authorize status</summary>
		public static int PAYMENT_ATOBARAICOM_MAX_REQUEST_GET_AUTHORIZE_STATUS = 1000;
		/// <summary>Atobaraicom invoice System use flag</summary>
		public static bool PAYMENT_SETTING_ATOBARAICOM_USE_INVOICE_SYSTEM_SERVICE = false;
		/// <summary>Atobaraicom default web request time out second</summary>
		public static int PAYMENT_ATOBARAICOM_WEB_REQUEST_TIME_OUT_SECOND = 0;

		/// <summary>加盟店ID</summary>
		public static string PAYMENT_SCORE_AFTER_PAY_SHOP_CODE = string.Empty;
		/// <summary>ダイレクトパスワード</summary>
		public static string PAYMENT_SCORE_AFTER_PAY_SHOP_PASSWORD = string.Empty;
		/// <summary>接続元ID</summary>
		public static string PAYMENT_SCORE_AFTER_PAY_TERMINAL_ID = string.Empty;
		/// <summary>注文情報登録URL</summary>
		public static string PAYMENT_SCORE_AFTER_PAY_URL_TRANSACTION = string.Empty;
		/// <summary>注文情報修正URL</summary>
		public static string PAYMENT_SCORE_AFTER_PAY_URL_MODIFY_TRANSACTION = string.Empty;
		/// <summary>発送情報登録URL</summary>
		public static string PAYMENT_SCORE_AFTER_PAY_URL_PD_REQUEST = string.Empty;
		/// <summary>注文キャンセルURL</summary>
		public static string PAYMENT_SCORE_AFTER_PAY_URL_CANCEL = string.Empty;
		/// <summary>与信結果取得URL</summary>
		public static string PAYMENT_SCORE_AFTER_PAY_URL_GET_AUTHOR = string.Empty;
		/// <summary>払込票印字データ取得URL</summary>
		public static string PAYMENT_SCORE_AFTER_PAY_URL_GET_INVOICE = string.Empty;
		/// <summary>スコア後払い：請求書同梱フラグ</summary>
		public static bool PAYMENT_SETTING_SCORE_DEFERRED_USE_INVOICE_BUNDLE = false;

		/// <summary>ペイジェント：マーチャントID </summary>
		public static string PAYMENT_PAYGENT_MERCHANTID = "";
		/// <summary>ペイジェント：接続ID </summary>
		public static string PAYMENT_PAYGENT_CONNECTID = "";
		/// <summary>ペイジェント：接続IDパスワード </summary>
		public static string PAYMENT_PAYGENT_CONNECTIDPASSWORD = "";
		/// <summary>ペイジェント：電文バージョン </summary>
		public static string PAYMENT_PAYGENT_API_VERSION = "";
		/// <summary>ペイジェント：加盟店名</summary>
		public static string PAYMENT_PAYGENT_MERCHANTNAME = "";
		/// <summary>ペイジェントクレジット：トークン生成鍵 </summary>
		public static string PAYMENT_PAYGENT_CREDIT_GENERATE_TOKEN_KEY = "";
		/// <summary>ペイジェントクレジット：トークン受け取りハッシュ鍵 </summary>
		public static string PAYMENT_PAYGENT_CREDIT_RECEIVE_TOKEN_HASHKEY = "";
		/// <summary>ペイジェントクレジット：3Dセキュア結果受付ハッシュ鍵 </summary>
		public static string PAYMENT_PAYGENT_CREDIT_RECEIVE_3DSECURERESULT_HASHKEY = "";
		/// <summary>ペイジェントクレジット：3Dセキュア利用 </summary>
		public static bool PAYMENT_PAYGENT_CREDIT_3DSECURE = false;
		/// <summary>ペイジェントクレジット：決済方法 </summary>
		public static PaygentCreditCardPaymentMethod? PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD = null;
		/// <summary>ペイジェントクレジット：決済方法 (デジタルコンテンツ用)</summary>
		public static PaygentCreditCardPaymentMethod? PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS = null;
		/// <summary>ペイジェントクレジット：オーソリ制御区分 </summary>
		public static string PAYMENT_PAYGENT_CREDIT_AUTHORIZATION_CONTROLKBN = "";
		/// <summary>ペイジェントクレジット：トークン取得JS </summary>
		public static string PAYMENT_PAYGENT_CREDIT_GETTOKENJSURL = "";
		/// <summary>ペイジェントクレジット：セキュリティコード対応(現在trueのみ対応)</summary>
		public static bool PAYMENT_SETTING_CREDIT_PAYGENT_SECURITYCODE = true;
		/// <summary>ペイジェントクレジット 差分通知ハッシュ値生成キー</summary>
		public static string PAYMENT_PAYGENT_NOTICE_HASHKEY = string.Empty;
		/// <summary>ペイジェント銀行ネットネットバンキング決済支払期限(日)</summary>
		public static int PAYMENT_PAYGENT_BANKNET_PAYMENT_LIMIT_DAY = 0;
		/// <summary>ペイジェント銀行ネット注文内容</summary>
		public static string PAYMENT_PAYGENT_BANKNET_ORDERDETAIL_NAME = string.Empty;
		/// <summary>ペイジェント銀行ネット注文内容（カナ）</summary>
		public static string PAYMENT_PAYGENT_BANKNET_ORDERDETAIL_NAME_KANA = string.Empty;
		/// <summary>ペイジェントATM決済 ATM決済支払期限(日)</summary>
		public static int PAYMENT_PAYGENT_ATM_PAYMENT_LIMIT_DAY = 0;
		/// <summary>ペイジェントATM決済 注文内容</summary>
		public static string PAYMENT_PAYGENT_ATM_ORDERDETAIL_NAME = string.Empty;
		/// <summary>ペイジェントATM決済 注文内容（カナ）</summary>
		public static string PAYMENT_PAYGENT_ATM_ORDERDETAIL_NAME_KANA = string.Empty;
		// GMOアトカラ
		public static bool PAYMENT_GMOATOKARA_ENABLED = false;
		public static string PAYMENT_GMOATOKARA_DEFERRED_AUTHENTICATIONID = string.Empty;
		public static string PAYMENT_GMOATOKARA_DEFERRED_SHOPCODE = string.Empty;
		public static string PAYMENT_GMOATOKARA_DEFERRED_CONNECTPASSWORD = string.Empty;
		public static string PAYMENT_GMOATOKARA_DEFERRED_SMSAUTHENTICATIONPASSWORD = string.Empty;
		public static string PAYMENT_GMOATOKARA_DEFERRED_PDCOMPANYCODE = "11";
		public static string PAYMENT_GMOATOKARA_DEFERRED_URL_ORDERREGISTER = string.Empty;
		public static string PAYMENT_GMOATOKARA_DEFERRED_URL_GETAUTHRESULT = string.Empty;
		public static string PAYMENT_GMOATOKARA_DEFERRED_URL_ORDERMODIFYCANCEL = string.Empty;
		public static string PAYMENT_GMOATOKARA_DEFERRED_URL_SHIPMENT = string.Empty;
		public static string PAYMENT_GMOATOKARA_DEFERRED_URL_SHIPMENTMODIFYCANCEL = string.Empty;

		//------------------------------------------------------
		// 処理中にデータを受け渡すためのKey
		//------------------------------------------------------
		public static string FLG_ORDER_PAYMENT_API_SKIP = "order_payment_api_skip";

		//------------------------------------------------------
		// カスタムページ/パーツ/Cssファイルツリーリスト枠のサイズ
		//------------------------------------------------------
		/// <summary>カスタムページ枠の最大サイズ</summary>
		public static int CUSTOM_PAGE_FRAME_MAX_HEIGHT = 460;
		/// <summary>カスタムページ枠の最小サイズ</summary>
		public static int CUSTOM_PAGE_FRAME_MIN_HEIGHT = 115;
		/// <summary>カスタムページ枠用アップダウンサイズをリサイズ</summary>
		public static int CUSTOM_PAGE_FRAME_UPDOWN_HEIGHT = 115;
		/// <summary>カスタムパーツ枠の最大サイズ</summary>
		public static int CUSTOM_PART_FRAME_MAX_HEIGHT = 460;
		/// <summary>カスタムパーツ枠の最小サイズ</summary>
		public static int CUSTOM_PART_FRAME_MIN_HEIGHT = 115;
		/// <summary>カスタムパーツ枠用アップダウンサイズをリサイズ </summary>
		public static int CUSTOM_PART_FRAME_UPDOWN_HEIGHT = 115;
		/// <summary>Cssファイルツリーリストの最大サイズ</summary>
		public static int CSS_LIST_FRAME_MAX_HEIGHT = 500;
		/// <summary>Cssファイルツリーリストの最小サイズ </summary>
		public static int CSS_LIST_FRAME_MIN_HEIGHT = 200;
		/// <summary>Cssファイルツリーリスト用アップダウンサイズをリサイズ </summary>
		public static int CSS_LIST_FRAME_UPDOWN_HEIGHT = 100;
		/// <summary>カスタムパーツの制限件数</summary>
		public static int CUSTOM_PARTS_MAX_COUNT = 100;

		//------------------------------------------------------
		// 配送設定系
		//------------------------------------------------------
		/// <summary>配送料金優先度設定：(高い配送料を優先[HIGH],低い配送料を優先[LOW])</summary>
		public static ShippingPriority? SHIPPINGPRIORITY_SETTING = null;

		//------------------------------------------------------
		// 外部レコメンド連携設定系
		//------------------------------------------------------
		/// <summary>レコメンドエンジン区分(レコナイズ、、)</summary>
		public static RecommendEngine? RECOMMEND_ENGINE_KBN = null;

		/// <summary>シルバーエッグ：マーチャントID</summary>
		public static string RECOMMEND_SILVEREGG_MERCHANT_ID = "";
		/// <summary>シルバーエッグ：レコメンドAPIドメイン</summary>
		public static string RECOMMEND_SILVEREGG_API_DOMAIN = "";
		/// <summary>シルバーエッグ：レポートAPIのURL</summary>
		public static string RECOMMEND_SILVEREGG_REPORT_API_URL = "";
		/// <summary>シルバーエッグ：レポートAPIのアクセストークン</summary>
		public static string RECOMMEND_SILVEREGG_REPORT_API_TOKEN = "";
		/// <summary>シルバーエッグ：商品連携用FTPホスト</summary>
		public static string RECOMMEND_SILVEREGG_FTP_HOST = "";
		/// <summary>シルバーエッグ：商品連携用FTPアカウント</summary>
		public static string RECOMMEND_SILVEREGG_FTP_ID = "";
		/// <summary>シルバーエッグ：商品連携用FTPパスワード</summary>
		public static string RECOMMEND_SILVEREGG_FTP_PW = "";

		/// <summary>外部レコメンド連携：マスタアップロード時の外部レコメンド自動連携フラグ</summary>
		public static bool MASTERFILEIMPORT_AUTO_RECOMMEND_FLG = true;
		/// <summary>外部レコメンド連携：レコメンド表示設定ファイルパス</summary>
		public static string RECOMMEND_DISP_SETTING_FILE_PATH = "";
		/// <summary>外部レコメンド連携：レコメンドパターン設定ファイルパス</summary>
		public static string RECOMMEND_PATTERN_SETTING_FILE_PATH = "";

		//------------------------------------------------------
		// アフィリエイト設定系
		//------------------------------------------------------
		// リンクシェア：有効無効
		public static bool AFFILIATE_LINKSHARE_VALID = false;
		// リンクシェア：クッキータイムアウト
		public static int AFFILIATE_LINKSHARE_COOKIE_LIMIT_DAYS = 60;

		//------------------------------------------------------
		// ユーザー情報連携設定系
		//------------------------------------------------------
		// ユーザー情報連携OP：有効無効
		public static bool USER_COOPERATION_ENABLED = false;

		//------------------------------------------------------
		// リターゲティング設定系
		//------------------------------------------------------
		/// <summary>CRITEO OP：有効無効</summary>
		public static bool CRITEO_OPTION_ENABLED = false;
		/// <summary>CRITEO：加盟店アカウント</summary>
		public static string CRITEO_ACCOUNT_ID = "";
		/// <summary>CRITEO：クロスデバイス利用有無</summary>
		public static bool CRITEO_CROSS_DEVICE_ENABLED = false;

		//------------------------------------------------------
		// フレンドリーURL系
		//------------------------------------------------------
		// フレンドリーURL：有効無効
		public static bool FRIENDLY_URL_ENABLED = false;

		//------------------------------------------------------
		// パスワードリマインダー系
		//------------------------------------------------------
		// パスワードリマインダー認証項目：(生年月日[BIRTH],電話番号[TEL])
		public static PasswordReminderAuthItem? PASSWORDRIMINDER_AUTHITEM = null;

		//------------------------------------------------------
		// ディレクトリ・パス系
		//------------------------------------------------------
		/// <summary>PDFテンプレート物理パス</summary>
		public static string PHYSICALDIRPATH_PDF_TEMPLATE = "";

		/// <summary>カード保持可能枚数</summary>
		public static int MAX_NUM_REGIST_CREDITCARD = 0;

		/// <summary>注文が１カートのときも注文決済画面を表示するか</summary>
		public static bool DISPLAY_ORDERSETTLEMENTPAGE_IN_SINGLE_CART_CASE = false;

		/// <summary>CMSコンテンツルート（サイトルート）</summary>
		public static string PHYSICALDIRPATH_CONTENTS_ROOT = "";
		/// <summary>CMSコンテンツルート（各アプリのContentsフォルダ）</summary>
		public static string PHYSICALDIRPATH_CONTENTS = "";
		/// <summary>XMLルート</summary>
		public static string PHYSICALDIRPATH_FRONT_PC_XML = "";

		/// <summary>マスタファイルアップロードディレクトリ</summary>
		public static string PHYSICALDIRPATH_MASTERUPLOAD_DIR = "";

		/// <summary>フロント物理パス設定:Mobile</summary>
		public static string PHYSICALDIRPATH_FRONT_MOBILE = "";

		/// <summary>History Backup Path</summary>
		public static string PHYSICALDIRPATH_OPERATION_UPDATEHISTORY_LOGFILE = "";

		/// <summary>フロント物理パス設定:PC</summary>
		public static string PHYSICALDIRPATH_FRONT_PC = "";

		/// <summary>管理画面物理パス設定:w2cManager</summary>
		public static string PHYSICALDIRPATH_COMMERCE_MANAGER = "";

		/// <summary>管理画面物理パス設定:w2cmsManager</summary>
		public static string PHYSICALDIRPATH_CMS_MANAGER = "";

		/// <summary>管理画面メニューXML物理パス</summary>
		public static string PHYSICALDIRPATH_MANAGER_MENU_XML = "";
		/// <summary>管理メニューセッティング</summary>
		public const string FILE_XML_MANAGER_MENU_SETTING = "Xml/Menu.xml";
		/// <summary>管理画面機能一覧XML物理パス</summary>
		public static string PHYSICALDIRPATH_MANAGER_PAGE_INDEX_LIST_XML = "";
		/// <summary>機能一覧セッティング</summary>
		public const string FILE_XML_MANAGER_PAGE_INDEX_LIST = "Xml/PageIndexList.xml";

		/// <summary>シルバーエッグレコメンド連携用CSV出力先物理パス</summary>
		public static string PHYSICALDIRPATH_SILVEREGGAIGENTCSVFILEUPLOADER_CSVFILES = "";

		/// <summary>CreditErrorMessagesXML物理ファイルパス</summary>
		public static string PHYSICALFILEPATH_CREDITERRORMESSAGE = "";

		/// <summary>デコメ配信オプション：デコメファイルパス</summary>
		public static string MARKETINGPLANNER_DECOME_MOBILEHTMLMAIL_DIRPATH = "";

		/// <summary>デコメ配信オプション：デコメファイルURL</summary>
		public static string MARKETINGPLANNER_DECOME_MOBILEHTMLMAIL_URL = "";

		/// <summary>サイトマップ設定ファイル：PC</summary>
		public static string FILE_SITEMAPSETTING_PC = "";
		/// <summary>詳細検索設定ファイル</summary>
		public static string FILE_ADVANCEDSEARCHSETTING = "AdvancedSearchSetting.xml";
		/// <summary>ユーザ拡張項目ValidatorXmlファイル名</summary>
		public const string FILE_NAME_XML_USEREXTEND = "UserExtend";
		/// <summary>注文拡張項目ValidatorXmlファイル名</summary>
		public const string XML_ORDEREXTEND = "OrderExtend";
		/// <summary>マスタアップロードセッティング</summary>
		public const string FILE_XML_MASTER_UPLOAD_SETTING = "Xml/Setting/MasterUploadSetting.xml";
		/// <summary>マスタ出力定義セッティング</summary>
		public const string FILE_XML_MASTEREXPORTSETTING_SETTING = "Xml/Setting/MasterExportSetting.xml";
		/// <summary>Data migration setting</summary>
		public const string FILE_XML_DATA_MIGRATION_SETTING = "Xml/Setting/DataMigrationSetting.xml";
		/// <summary>ユーザ拡張項目ValidatorXmlファイル</summary>
		public const string PHYSICALPATH_FILE_XML_USEREXTEND = @"Xml\Validator\" + Constants.FILE_NAME_XML_USEREXTEND + ".xml";
		/// <summary>GMOクレジットカードエラーメッセージファイルパス</summary>
		public const string FILEPATH_XML_GMO_CREDIT_ERROR_MESSAGE = @"Xml\Message\GMOCreditErrorMessages.xml";
		/// <summary>SBPSクレジットカードエラーメッセージファイルパス</summary>
		public const string FILEPATH_XML_SBPS_CREDIT_ERROR_MESSAGE = @"Xml\Message\SBPSCreditErrorMessages.xml";
		/// <summary>YamatoKwcクレジットカードエラーメッセージファイルパス</summary>
		public const string FILEPATH_XML_YAMATOKWC_CREDIT_ERROR_MESSAGE = @"Xml\Message\YamatoKwcCreditErrorMessages.xml";
		/// <summary>VeriTransクレジットカードエラーメッセージファイルパス</summary>
		public const string FILEPATH_XML_VERITRANS_CREDIT_ERROR_MESSAGE = @"Xml\Message\VeriTransCreditErrorMessages.xml";
		/// <summary>e-SCOTTクレジットカードエラーメッセージファイルパス</summary>
		public const string FILEPATH_XML_ESCOTT_CREDIT_ERROR_MESSAGE = @"Xml\Message\ESCottCreditErrorMessages.xml";
		/// <summary>ZEUSクレジットカードエラーメッセージファイルパス</summary>
		public const string FILEPATH_XML_ZEUS_CREDIT_ERROR_MESSAGE = @"Xml\Message\ZEUSCreditErrorMessages.xml";
		/// <summary>Rakutenクレジットカードエラーメッセージファイルパス</summary>
		public const string FILEPATH_XML_RAKUTEN_CREDIT_ERROR_MESSAGE = @"Xml\Message\RakutenCreditErrorMessages.xml";
		/// <summary>ペイジェントクレジットカードエラーメッセージファイルパス</summary>
		public const string FILEPATH_XML_PAYGENT_CREDIT_ERROR_MESSAGE = @"Xml\Message\PaygentCreditErrorMessages.xml";
		/// <summary>外部ファイル取込バッチ用パラメタ定義ファイル</summary>
		public const string FILEPATH_XML_PARAM_DEFINE_SETTING = @"Settings\ParamDefineSetting.xml";
		/// <summary>ZEUSクレジットカードエラーメッセージファイル名</summary>
		public const string FILE_XML_ZEUS_CREDIT_ERROR_MESSAGE = "ZEUSCreditErrorMessages";
		/// <summary>SBPSクレジットカードエラーメッセージファイル名</summary>
		public const string FILE_XML_SBPS_CREDIT_ERROR_MESSAGE = "SBPSCreditErrorMessages";
		/// <summary>GMOクレジットカードエラーメッセージファイル名</summary>
		public const string FILE_XML_GMO_CREDIT_ERROR_MESSAGE = "GMOCreditErrorMessages";
		/// <summary>VERITRANSクレジットカードエラーメッセージファイル名</summary>
		public const string FILE_XML_VERITRANS_CREDIT_ERROR_MESSAGE = "VeriTransCreditErrorMessages";
		/// <summary>YamatoKwcクレジットカードエラーメッセージファイル名</summary>
		public const string FILE_XML_YAMATOKWC_CREDIT_ERROR_MESSAGE = "YamatoKwcCreditErrorMessages";
		/// <summary>e-SCOTTクレジットカードエラーメッセージファイル名</summary>
		public const string FILE_XML_ESCOTT_CREDIT_ERROR_MESSAGE = "EScottCreditErrorMessages";
		/// <summary>Rakutenクレジットカードエラーメッセージファイル名</summary>
		public const string FILE_XML_RAKUTEN_CREDIT_ERROR_MESSAGE = "RakutenCreditErrorMessages";
		/// <summary>ペイジェントクレジットカードエラーメッセージファイル名</summary>
		public const string FILE_XML_PAYGENT_CREDIT_ERROR_MESSAGE = "PaygentCreditErrorMessages";
		/// <summary>実行EXE：外部ファイル取込</summary>
		public static string PHYSICALDIRPATH_EXTERNAL_EXE = "";
		/// <summary>実行EXE：入荷通知メール送信</summary>
		public static string PHYSICALDIRPATH_ARRIVALMAILSEND_EXE = "";
		/// <summary>実行EXE：外部ファイル取込</summary>
		public static string PHYSICALDIRPATH_EXTERNALFILEUPLOAD = "";
		/// <summary>実行EXE：外部レコメンド連携</summary>
		public static string PHYSICALDIRPATH_SENDRECOMMENDITEM_EXE = "";
		/// <summary>実行EXE：CSメール取込EXE</summary>
		public static string PHYSICALDIRPATH_CSMAILRECEIVER_EXE = "";

		// パッケージルートパス
		public static string PATH_ROOT_EC = "";                   // w2Commerce
		public static string PATH_ROOT_MP = "";                   // w2MarketingPlanner
		public static string PATH_ROOT_CS = "";                   // w2CustomerSupport
		public static string PATH_ROOT_CMS = "";                  // w2Cms

		//------------------------------------------------------
		// 外部注文情報連携
		//------------------------------------------------------
		/// <summary>つくーるAPI連携：有効無効</summary>
		public static bool URERU_AD_IMPORT_ENABLED = false;
		/// <summary>ウケトルAPI連携；有効無効</summary>
		public static bool UKETORU_TRACKERS_API_ENABLED = false;
		/// <summary>ウケトルAPI連携；リクエストURL</summary>
		public static string UKETORU_TRACKERS_API_URL = "";
		/// <summary>ウケトルAPI連携；ショップトークン</summary>
		public static string UKETORU_TRACKERS_API_SHOP_TOKEN = "";

		//------------------------------------------------------
		// その他
		//------------------------------------------------------
		/// <summary>プロジェクト名</summary>
		public static string PLAN_NAME = "";
		/// <summary>プロジェクト名</summary>
		public static string PROJECT_NO = "";
		/// <summary>環境名</summary>
		public static string ENVIRONMENT_NAME = "";
		/// <summary>注文IDフォーマット</summary>
		public static string FORMAT_ORDER_ID = "";
		/// <summary>決済注文IDフォーマット（ヤマト決済など、注文時に決済用注文IDを発行する場合に利用）</summary>
		public static string FORMAT_PAYMENT_ORDER_ID = "";
		/// <summary>ヤマトKWC会員IDフォーマット（トークン生成時に必要）</summary>
		public static string FORMAT_PAYMENT_YAMATOKWC_MEMBER_ID = "";
		/// <summary>定期購入IDフォーマット</summary>
		public static string FORMAT_FIXEDPURCHASE_ID = "";
		/// <summary>領収書：領収書ファイル名フォーマット</summary>
		public static string FORMAT_RECEIPT_FILE_NAME = string.Empty;
		/// <summary>注文IDの採番方法（定期も同様）</summary>
		public static OrderNumberingType? ORDER_NUMBERING_TYPE = null;
		public enum OrderNumberingType
		{
			/// <summary>年月日＋日別連番</summary>
			DailySequence,
			/// <summary>連番</summary>
			Sequence
		}

		/// <summary>BASIC認証情報</summary>
		public static string BASIC_AUTHENTICATION_USER_ACCOUNT = "";

		/// <summary>GoogleAnalytics：プロファイルID</summary>
		public static string GOOGLEANALYTICS_PROFILE_ID = "";
		/// <summary>GoogleAnalytics：測定ID</summary>
		public static string GOOGLEANALYTICS_MEASUREMENT_ID = "";

		/// <summary>一覧系の表示件数:商品履歴保持数</summary>
		public static int CONST_PRODUCTHISTORY_HOLD_COUNT = 0;
		/// <summary>一覧系の表示件数:デフォルトで一覧に表示する件数</summary>
		public static int CONST_DISP_CONTENTS_DEFAULT_LIST = 0;
		/// <summary>一覧系の表示件数:商品一覧に表示する件数（写真あり）</summary>
		public static int CONST_DISP_CONTENTS_PRODUCT_LIST_IMG_ON = 0;
		/// <summary>一覧系の表示件数:商品一覧に表示する件数（写真なし）</summary>
		public static int CONST_DISP_CONTENTS_PRODUCT_LIST_IMG_OFF = 0;
		/// <summary>一覧系の表示件数:商品一覧に表示する件数（ウィンドウショッピング）</summary>
		public static int CONST_DISP_CONTENTS_PRODUCT_LIST_WINDOWSHOPPING = 0;
		/// <summary>一覧系の表示件数:商品バリエーション一覧に表示する件数</summary>
		public static int CONST_DISP_CONTENTS_PRODUCTVARIATION_LIST = 0;
		/// <summary>一覧系の表示件数:商品レビューに表示する件数</summary>
		public static int CONST_DISP_CONTENTS_PRODUCTREVIEW_LIST = 0;
		/// <summary>一覧系の表示件数:一覧に表示する上限数</summary>
		public static int CONST_DISP_CONTENTS_OVER_HIT_LIST = 0;
		/// <summary>一覧系の表示件数:注文ワークフロー設定一覧に表示する上限数</summary>
		public static int CONST_DISP_CONTENTS_ORDERWORKFLOWSETTING_LIST = 0;
		/// <summary>一覧系の表示件数:広告コード閲覧権限一覧に表示する件数</summary>
		public static int CONST_DISP_CONTENTS_AD_CODEAUTHORITY_REGISTER = 10;
		/// <summary>一覧系の表示件数:タグ閲覧権限一覧に表示する件数</summary>
		public static int CONST_DISP_CONTENTS_TAG_AUTHORITY_REGISTER = 10;

		/// <summary>一覧系の表示件数:外部リンクボタン件数</summary>
		public static int CONST_DISP_EXTERNAL_LINK_BUTTON = 7;

		/// <summary>ユーザーシンボル「リピーターユーザー」</summary>
		public static string USERSYMBOL_REPEATER = "";
		/// <summary>ユーザーシンボル「特記ーユーザー」</summary>
		public static string USERSYMBOL_HAS_NOTE = "";

		/// <summary>SEO設定：直下の子カテゴリの上位○数</summary>
		public static int SEOSETTING_CHILD_CATEGORY_TOP_COUNT = 0;

		/// <summary>フラグ：変更試行回数：初期値</summary>
		public static int FLG_PASSWORDREMINDER_CHANGE_TRIAL_LIMIT_COUNT_DEFAULT = 0;

		/// <summary>フラグ：住所４表示フラグ</summary>
		public static bool DISPLAY_ADDR4_ENABLED = true;
		/// <summary>フラグ：法人項目（企業名、部署名）表示フラグ</summary>
		public static bool DISPLAY_CORPORATION_ENABLED = true;

		/// <summary>配送料税率</summary>
		public static decimal CONST_SHIPPING_TAXRATE = 0;
		/// <summary>配送料税率</summary>
		public static decimal CONST_PAYMENT_TAXRATE = 0;

		/// <summary>運用地：国</summary>
		public static string OPERATIONAL_BASE_ISO_CODE = "";
		/// <summary>運用地：州(運用地：国がアメリカの場合に設定)</summary>
		public static string OPERATIONAL_BASE_PROVINCE = "";

		/// <summary>注文拡張ステータス最大数（DB定義上の最大数）</summary>
		public const int CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX = 50;
		/// <summary>注文拡張ステータスNO：与信期限切れ再与信時キャンセル失敗フラグ</summary>
		public const int ORDER_EXTEND_STATUS_NO_AUTHEXPIRED_REAUTH_CANCEL_FAILED = 31;
		/// <summary>注文拡張ステータスNO：与信期限切れ再与信NGフラグ</summary>
		public const int ORDER_EXTEND_STATUS_NO_AUTHEXPIRED_REAUTH_FAILED = 32;
		/// <summary>注文拡張ステータスNO：初回購入確認（注文者重複）</summary>
		public const int ORDER_EXTEND_STATUS_NO_CHECK_FIRSTTIME_OWNER_DUPLICATE = 39;
		/// <summary>注文拡張ステータスNO：ロハコ連携：予約注文</summary>
		public static int ORDER_EXTEND_STATUS_NO_LOHACO_RESERVE_ORDER = 0;
		/// <summary>注文拡張ステータスNO：CrossPoint ポイント確定済み</summary>
		public const int ORDER_EXTEND_STATUS_NO_CROSSPOINT_GRANTED = 40;

		/// <summary>サポートサイトURL</summary>
		public static string SUPPORT_SITE_URL = "";
		/// <summary>サポートサイト連携</summary>
		public static bool COOPERATION_SUPPORT_SITE = false;
		///<summary>W2Unified用利用規約</summary>
		public static string W2UNIFIED_TERMS_OF_SERVICE_URL = "";
		/// <summary>W2Repeat、W2RepeatFood用利用規約</summary>
		public static string W2REPEAT_TERMS_OF_SERVICE_URL = "";
		/// <summary>受注編集：自動計算適用のデフォルト（TRUE：ON、FALSE：OFF）</summary>
		public static bool ORDER_APPLYAUTOCALCULATION_DEFAULT = false;

		/// <summary>受注詳細画面でアラート表示にする注文ステータス（カンマ区切り。空白で非利用となる。）</summary>
		public static string ALERT_FOR_ORDER_STATUS_IN_ORDER_CONFIRM = "";

		/// <summary>ログインID　クッキー保持期間（day）</summary>
		public static int LOGIN_ID_COOKIE_LIMIT_DAYS = 0;
		/// <summary>カートID　クッキー保持期間（day）</summary>
		public static int CART_ID_COOKIE_LIMIT_DAYS = 0;

		/// <summary>定期購入可能な決済種別IDリスト</summary>
		public static List<string> CAN_FIXEDPURCHASE_PAYMENTIDS = new List<string>();

		/// <summary>PC・モバイルメール両方送信</summary>
		public static bool MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED = false;
		/// <summary>PC・モバイルメール片方入力許可(PC・モバイルメール両方送信の許可必須)</summary>
		public static bool EITHER_ENTER_MAIL_ADDRESS_ENABLED = false;
		/// <summary>レコメンド系商品の在庫切れ表示可否(True:表示 False:非表示)</summary>
		public static bool SHOW_OUT_OF_STOCK_ITEMS = true;
		/// <summary>ルートカテゴリ用ソート区分（0:カテゴリーID順, 1:名前順, 2:フリガナ順, 3:表示順）</summary>
		public static string ROOT_CATEGORY_SORT_KBN = "0";
		/// <summary>セットプロモーション：最大計算組合せパターン数</summary>
		public static int SETPROMOTION_MAXIMUM_NUMBER_OF_COMBINATIONS = 0;
		/// <summary>セットプロモーション：最大計算対象SKU数</summary>
		public static int SETPROMOTION_MAXIMUM_NUMBER_OF_TARGET_SKUS = 0;
		/// <summary>セットプロモーション：適用優先順OP</summary>
		public static bool SETPROMOTION_APPLY_ORDER_OPTION_ENABLED = false;
		/// <summary>注文メモ登録モード</summary>
		public static OrderMemoRegisterMode? ORDERMEMO_REGISTERMODE = null;
		public enum OrderMemoRegisterMode
		{
			/// <summary>何か変更があったら登録</summary>
			Modified,
			/// <summary>常に登録</summary>
			Always
		}
		/// <summary>注文者情報の変更をユーザー情報に反映するCheckBox：初期値</summary>
		public static bool DEFAULTUPDATE_TOUSER_FROMORDEROWNER = false;
		/// <summary>Display NotSearch Defaultt</summary>
		public static bool DISPLAY_NOT_SEARCH_DEFAULT = true;

		//  CPM（顧客ポートフォリオ・マネジメント）設定
		/// <summary>CPMオプション有効</summary>
		public static bool CPM_OPTION_ENABLED = false;
		/// <summary>CPMクラスタ設定</summary>
		public static CpmClusterSettings CPM_CLUSTER_SETTINGS = null;

		/// <summary>グローバル設定</summary>
		public static GlobalConfigs GLOBAL_CONFIGS = null;

		/// <summary>商品サブ画像最大件数</summary>
		public static int PRODUCTSUBIMAGE_MAXCOUNT = 0;

		/// <summary>モール連携：SFTP秘密鍵ファイルディレクトリ</summary>
		public static string PHYSICALDIRPATH_MALLCOOPERATION_SFTPPRIVATEKEY_FILEPATH = "";
		/// <summary>モール連携：SFTP秘密鍵ファイル名</summary>
		public static string MALLCOOPERATION_SFTPPRIVATEKEY_FILENAME = "";

		/// <summary>レコメンド設定：注文完了画面でレコメンド適用可能な決済種別</summary>
		public static string[] RECOMMENDOPTION_APPLICABLE_PAYMENTIDS_FOR_ORDER_COMPLETE = null;
		/// <summary>レコメンド設定：定期商品の配送パターンをレコメンド設定側で強制するか</summary>
		public static bool RECOMMENDOPTION_IS_FORCED_FIXEDPURCHASE_SETTING_BY_RECOMMEND = false;
		/// <summary>注文完了ページでのレコメンド商品購入時に確認画面表示するか</summary>
		public static bool ENABLES_ORDERCONFIRM_MODAL_WINDOW_ON_RECOMMENDATION_AT_ORDERCOMPLETE = false;
		/// <summary>基軸通貨コード</summary>
		public static string CONST_KEY_CURRENCY_CODE = "JPY";

		/// <summary>半角文字は入力可能か</summary>
		public static bool HALFWIDTH_CHARACTER_INPUTABLE = false;

		/// <summary>メールテンプレートのフィールド（初回配送日）</summary>
		public static string FIELD_MAIL_FIELD_FIRST_SHIPPING_DATE = "first_shipping_date";

		/// <summary>メールテンプレート用に算出した初回配送日が既に過ぎている場合に表示する値</summary>
		public static string CONST_INVALID_FIRST_SHIPPING_DATE_VALUE = "-";

		/// <summary>メールテンプレートのフィールド（定期購入配送キャンセル期限日）</summary>
		public static string FIELD_MAIL_FIELD_PURCHASE_CANCEL_DEADLINE_DATE = "cancel_deadline_date";

		/// <summary>外部ファイル（js, css）の更新用クエリ文字列（URLエンコードされ画面にセットされます）</summary>
		public static string QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED = "202007141100";

		/// <summary>出荷予定日オプション</summary>
		public static bool SCHEDULED_SHIPPING_DATE_OPTION_ENABLE = false;
		/// <summary>出荷予定日を表示するか(フロントサイトと納品書)</summary>
		public static bool SCHEDULED_SHIPPING_DATE_VISIBLE = false;

		/// <summary>メールテンプレート:商品価格 税込み・税抜き表記 表記:true 非表示false</summary>
		public static bool SETTING_MAIL_PRODUCT_PRICE_TAX_TEXT_DISPLAY = false;

		/// <summary>DM発送履歴の表示方法</summary>
		public static string DMSHIPPINGHISTORY_DISPLAY_METHOD = "ALL";
		/// <summary>DM発送履歴の表示方法(DM発送履歴タブにのみDM発送履歴を表示)</summary>
		public const string DMSHIPPINGHISTORY_DISPLAY_METHOD_TAB_ONLY = "TAB_ONLY";
		/// <summary>DM発送履歴の表示方法(DM発送履歴タブ/全体履歴に表示)</summary>
		public const string DMSHIPPINGHISTORY_DISPLAY_METHOD_ALL = "ALL";

		/// <summary>決済連携メモ アクション名 出荷報告</summary>
		public const string ACTION_NAME_SHIPPING_REPORT = "出荷報告";
		/// <summary>決済連携メモ アクション名 請求書印字データ</summary>
		public const string ACTION_NAME_INVOICE_REPORT = "請求書印字データ";
		/// <summary>タグ機能:出力箇所オプション</summary>
		public static bool TAG_TARGETPAGECHECKBOX_OPTION = false;

		/// <summary>リピートプラスONE設定</summary>
		public static RepeatPlusOneConfigs REPEATPLUSONE_CONFIGS = null;
		/// <summary>LPビルダー利用可能数</summary>
		public static int? LPBUILDER_MAXCOUNT = 0;
		/// <summary>リピートプラスONEオプションがTRUEの場合、リダイレクト先を設定</summary>
		public static string REPEATPLUSONE_REDIRECT_PAGE = "";
		/// <summary>リピートプラスONE対応ＯＰ：有効無効</summary>
		public static bool REPEATPLUSONE_OPTION_ENABLED = true;
		/// <summary>入荷通知メール管理ＯＰ：有効無効</summary>
		public static bool USERPRODUCTARRIVALMAIL_OPTION_ENABLED = true;
		/// <summary>在庫減少アラートメール管理ＯＰ：有効無効</summary>
		public static bool STOCKALERTMAIL_OPTION_ENABLED = true;
		/// <summary>コーディネート管理とスタッフ情報ＯＰ：有効無効</summary>
		public static bool COORDINATE_WITH_STAFF_OPTION_ENABLED = true;
		/// <summary>商品ランキング設定ＯＰ：有効無効</summary>
		public static bool PRODUCTRANKING_OPTION_ENABLED = true;
		/// <summary>商品一覧表示設定ＯＰ：有効無効</summary>
		public static bool PRODUCTLISTDISPSETTING_OPTION_ENABLED = true;
		/// <summary>特集管理：有効無効</summary>
		public static bool FEATUREAREASETTING_OPTION_ENABLED = true;
		/// <summary>特集ページ：有効無効</summary>
		public static bool FEATUREPAGESETTING_OPTION_ENABLED = true;
		/// <summary>お知らせ設定表示ＯＰ：有効無効</summary>
		public static bool NEWSLISTDISPSETTING_OPTION_ENABLED = true;
		/// <summary>SEOタグ設定表示ＯＰ：有効無効</summary>
		public static bool SEOTAGDISPSETTING_OPTION_ENABLED = true;
		/// <summary>ユーザ管理レベル設定表示ＯＰ：有効無効</summary>
		public static bool USERMANAGEMENTLEVELSETTING_OPTION_ENABLED = true;
		/// <summary>かんたん会員登録設定表示ＯＰ：有効無効</summary>
		public static bool USEREAZYREGISTERSETTING_OPTION_ENABLED = true;
		/// <summary>注文拡張ステータス設定表示ＯＰ：有効無効</summary>
		public static bool ORDEREXTENDSTATUSSETTING_OPTION_ENABLED = true;
		/// <summary>受注ワークフロー表示ＯＰ：有効無効</summary>
		public static bool ORDERWORKFLOW_OPTION_ENABLED = true;
		/// <summary>ユーザ拡張項目設定表示ＯＰ：有効無効</summary>
		public static bool USEREXTENDSETTING_OPTION_ENABLED = true;
		/// <summary>ターゲットリスト情報表示ＯＰ：有効無効</summary>
		public static bool MARKETINGPLANNER_TARGETLIST_OPTION_ENABLE = true;
		/// <summary>在庫管理情報表示ＯＰ：有効無効</summary>
		public static bool PRODUCT_STOCK_OPTION_ENABLE = false;
		/// <summary>ショートURL設定表示ＯＰ：有効無効</summary>
		public static bool SHORTURL_OPTION_ENABLE = false;
		/// <summary>データ結合マスタ表示ＯＰ：有効無効</summary>
		public static bool MASTEREXPORT_DATABINDING_OPTION_ENABLE = false;
		/// <summary>データ結合マスタ最大出力数</summary>
		public static int MASTEREXPORT_DATABINDING_OUTPUT_LINES_LIMIT = 0;
		/// <summary>商品カテゴリ表示ＯＰ：有効無効</summary>
		public static bool PRODUCT_CTEGORY_OPTION_ENABLE = false;
		/// <summary>商品タグ表示ＯＰ：有効無効</summary>
		public static bool PRODUCT_TAG_OPTION_ENABLE = false;
		/// <summary>商品サブ画像表示ＯＰ：有効無効</summary>
		public static bool PRODUCT_SUBIMAGE_SETTING_OPTION_ENABLE = false;
		/// <summary>2クリック決済設定表示ＯＰ：有効無効</summary>
		public static bool TWOCLICK_OPTION_ENABLE = false;
		/// <summary>督促管理表示ＯＰ：有効無効</summary>
		public static bool DEMAND_OPTION_ENABLE = false;
		/// <summary>定期購入回数割引表示ＯＰ：有効無効</summary>
		public static bool FIXED_PURCHASE_DISCOUNT_PRICE_OPTION_ENABLE = false;
		/// <summary>定期購入変更期限案内メールＯＰ：有効無効</summary>
		public static bool SEND_FIXED_PURCHASE_DEADLINE_NOTIFICATION_MAIL_TO_USER = false;
		/// <summary>ユーザー属性情報表示ＯＰ：有効無効</summary>
		public static bool USER_ATTRIBUTE_OPTION_ENABLE = false;
		/// <summary>ABテストオプション：有効無効</summary>
		public static bool AB_TEST_OPTION_ENABLED = false;
		/// <summary>特定商取引法対応：有効無効</summary>
		public static bool CORRESPONDENCE_SPECIFIEDCOMMERCIALTRANSACTIONS_ENABLE = false;
		/// <summary>Order by subscription box valid flag field</summary>
		public const string KBN_SORT_SUBSCRIPTION_BOX_VALID_FLAG = "04";

		//========================================================================
		// 台湾FLAPS連携
		//========================================================================
		/// <summary>台湾FLAPS連携ＯＰ有効無効</summary>
		public static bool FLAPS_OPTION_ENABLE = false;
		/// <summary>台湾FLAPS連携：API接続URL</summary>
		public static string FLAPS_API_URL = "";
		/// <summary>台湾FLAPS連携トークン</summary>
		public static string FLAPS_API_TOKEN = "";
		/// <summary>台湾FLAPS連携IS指令集</summary>
		public static string FLAPS_OPTION_IS = "";
		/// <summary>台湾FLAPS連携ログ出力</summary>
		public static bool FLAPS_API_LOGFILE_OUTPUT = true;
		/// <summary>台湾FLAPS連携ログファイル出力先フォルダパス</summary>
		public static string PHYSICALDIRPATH_FLAPS_API_LOGFILE = "";
		/// <summary>台湾FLAPS連携APIログファイル名 接頭辞</summary>
		public static string FLAPS_API_LOGFILE_NAME_PREFIX = "";
		/// <summary>台湾FLAPS連携APIログファイル サイズ閾値（MB）</summary>
		public static int FLAPS_API_LOGFILE_THRESHOLD = 500;
		/// <summary>レジコード(注文連携の際に必要な値)</summary>
		public static string FLAPS_ORDER_CASH_REGISTER_CODE = "";
		/// <summary>ショップカウンターコード(注文連携の際に必要な値)</summary>
		public static string FLAPS_ORDER_SALE_POINT_CODE = "";
		/// <summary>販売者コード(注文連携の際に必要な値)</summary>
		public static string FLAPS_ORDER_USER_ID_CODE = "";
		/// <summary>会員唯一番号(注文連携の際に必要な値)</summary>
		public static string FLAPS_ORDER_MEMBER_SER_NO = "";
		/// <summary>会員カード発行者コード(注文連携の際に必要な値)</summary>
		public static string FLAPS_ORDER_EMPLOYEE_CODE = "";
		/// <summary>一度に取得する商品データの数</summary>
		public static string FLAPS_THE_NUMBER_OF_RECORDS_TO_CAPTURE_AT_ONCE = "";
		/// <summary>ショップカウンター業績コード用の注文拡張項目の番号。1と10の間を指定する。 (注文キャンセル処理時必要)</summary>
		public static string FLAPS_ORDEREXTENDSETTING_ATTRNO_FOR_POSNOSERNO = "";
		/// <summary>シリアルナンバーとバーコード用の商品連携ID(cooperation_id)の番号(カンマ区切りで1と10の間の別の数字を入力)</summary>
		public static List<string> FLAPS_PRODUCT_COOPIDS_FOR_SERNO_AND_BARCODE = new List<string>();
		/// <summary>シリアルナンバーとバーコード用の商品バリエーション連携ID(cooperation_id)の番号(カンマ区切りで1と10の間の別の数字を入力)</summary>
		public static List<string> FLAPS_PRODUCTVARIATION_COOPIDS_FOR_SERNO_AND_BARCODE = new List<string>();

		/// <summary>台湾FLAPS連携API名 注文用</summary>
		public const string FLAPS_API_NAME_ORDER = "order";
		/// <summary>台湾FLAPS連携API名 商品用</summary>
		public const string FLAPS_API_NAME_PRODUCT = "product";
		/// <summary>台湾FLAPS連携API名 在庫用</summary>
		public const string FLAPS_API_NAME_PRODUCT_STOCK = "productStock";

		// 台湾FLAPS連携APIレスポンス項目名
		/// <summary>商品結果モデル</summary>
		public const string FLAPS_API_RESULT_PRODUCT = "product";
		/// <summary>商品情報</summary>
		public const string FLAPS_API_RESULT_PRODUCT_GOODS = "Goods";
		/// <summary>商品マテリア</summary>
		public const string FLAPS_API_RESULT_PRODUCT_MATERIAL = "Material";
		/// <summary>商品マテリア名</summary>
		public const string FLAPS_API_RESULT_PRODUCT_MATERIAL_NAME = "MaterialName";
		/// <summary>商品マテリアctr名</summary>
		public const string FLAPS_API_RESULT_PRODUCT_MATERIAL_CTR_NAME = "MaterialCtrName";
		/// <summary>商品マテリアパーセント</summary>
		public const string FLAPS_API_RESULT_PRODUCT_MATERIAL_PERCENT = "MaterialPercent";
		/// <summary>フェッチ状態</summary>
		public const string FLAPS_API_RESULT_PRODUCT_FINISH_FETCH = "FinishFetch";
		/// <summary>バーコード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_BARCODE = "Barcode";
		/// <summary>大単位変換率</summary>
		public const string FLAPS_API_RESULT_PRODUCT_BIG_EXCHANGE_RATE = "BigExchangeRate";
		/// <summary>大ユニットコード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_BIG_UNIT_CODE = "BigUnitCode";
		/// <summary>ブランドコード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_BRAND_CODE = "BrandCode";
		/// <summary>ブランドロゴコード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_BRAND_LOGO_CODE = "BrandLOGOCode";
		/// <summary>ブランドロゴ名</summary>
		public const string FLAPS_API_RESULT_PRODUCT_BRAND_LOGO_NAME = "BrandLOGOName";
		/// <summary>ブランド名</summary>
		public const string FLAPS_API_RESULT_PRODUCT_BRAND_NAME = "BrandName";
		/// <summary>セグメントコード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_BROKEN_ATTRIBUTE = "BrokenAttribute";
		/// <summary>セグメントコード名</summary>
		public const string FLAPS_API_RESULT_PRODUCT_BROKEN_CODE_NAME = "BrokenCodeName";
		/// <summary>商品コード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_CODE = "Code";
		/// <summary>色コード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_COLOR_CODE = "ColorCode";
		/// <summary>色名</summary>
		public const string FLAPS_API_RESULT_PRODUCT_COLOR_NAME = "ColorName";
		/// <summary>販売年</summary>
		public const string FLAPS_API_RESULT_PRODUCT_SELL_YEAR = "SellYear";
		/// <summary>商品唯一番号</summary>
		public const string FLAPS_API_RESULT_PRODUCT_SER_NO = "SerNo";
		/// <summary>スタイルコード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_STYLE_CODE = "StyleCode";
		/// <summary>商品名</summary>
		public const string FLAPS_API_RESULT_PRODUCT_NAME = "NAME";
		/// <summary>ステータス</summary>
		public const string FLAPS_API_RESULT_PRODUCT_STATUS = "Status";
		/// <summary>ブランドシリーズコード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_MIDDLE_BRAND_CODE = "MiddleBrandCode";
		/// <summary>ブランドシリーズ名</summary>
		public const string FLAPS_API_RESULT_PRODUCT_MIDDLE_BRAND_NAME = "MiddleBrandName";
		/// <summary>サイズコード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_SIZE_CODE = "SizeCode";
		/// <summary>サイズ名</summary>
		public const string FLAPS_API_RESULT_PRODUCT_SIZE_NAME = "SizeName";
		/// <summary>販売シーズン</summary>
		public const string FLAPS_API_RESULT_PRODUCT_SELL_SEASON = "SellSeason";
		/// <summary>大カテゴリコード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_LARGE_CATEGORY_CODE = "LargeCategoryCode";
		/// <summary>大カテゴリ名</summary>
		public const string FLAPS_API_RESULT_PRODUCT_LARGE_CATEGORY_NAME = "LargeCategoryName";
		/// <summary>中カテゴリコード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_MIDDLE_CATEGORY_CODE = "MiddleCategoryCode";
		/// <summary>中カテゴリ名</summary>
		public const string FLAPS_API_RESULT_PRODUCT_MIDDLE_CATEGORY_NAME = "MiddleCategoryName";
		/// <summary>小カテゴリコード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_SMALL_CATEGORY_CODE = "SmallCategoryCode";
		/// <summary>小カテゴリ名</summary>
		public const string FLAPS_API_RESULT_PRODUCT_SMALL_CATEGORY_NAME = "SmallCategoryName";
		/// <summary>国際バーコード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_INTERNATIONAL_BARCODE = "InternationalBarcode";
		/// <summary>税込標準価格</summary>
		public const string FLAPS_API_RESULT_PRODUCT_TAXED_STD_PRICE = "TaxedStdPrice";
		/// <summary>規格名称</summary>
		public const string FLAPS_API_RESULT_PRODUCT_SPEC_NAME = "SpecName";
		/// <summary>商品プロパティコード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_GOODS_PROPERTIES_CODE = "GoodsPropertiesCode";
		/// <summary>商品プロパティ名</summary>
		public const string FLAPS_API_RESULT_PRODUCT_GOODS_PROPERTIES_NAME = "GoodsPropertiesName";
		/// <summary>商品プロパティコード2</summary>
		public const string FLAPS_API_RESULT_PRODUCT_GOODS_PROPERTIES2_CODE = "GoodsProperties2Code";
		/// <summary>商品プロパティ名2</summary>
		public const string FLAPS_API_RESULT_PRODUCT_GOODS_PROPERTIES2_NAME = "GoodsProperties2Name";
		/// <summary>ユニットコード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_UNIT_CODE = "UnitCode";
		/// <summary>中ユニットコード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_MIDDLE_UNIT_CODE = "MiddleUnitCode";
		/// <summary>中単位変換率</summary>
		public const string FLAPS_API_RESULT_PRODUCT_MIDDLE_EXCHANGE_RATE = "MiddleExchangeRate";
		/// <summary>商品タイプ</summary>
		public const string FLAPS_API_RESULT_PRODUCT_GOODS_TYPE = "GoodsType";
		/// <summary>商品タイプ名</summary>
		public const string FLAPS_API_RESULT_PRODUCT_GOODS_TYPE_NAME = "GoodsTypeName";
		/// <summary>備考1</summary>
		public const string FLAPS_API_RESULT_PRODUCT_REMARK = "Remark";
		/// <summary>備考2</summary>
		public const string FLAPS_API_RESULT_PRODUCT_REMARK2 = "Remark2";
		/// <summary>備考3</summary>
		public const string FLAPS_API_RESULT_PRODUCT_REMARK3 = "Remark3";
		/// <summary>備考4</summary>
		public const string FLAPS_API_RESULT_PRODUCT_REMARK4 = "Remark4";
		/// <summary>備考5</summary>
		public const string FLAPS_API_RESULT_PRODUCT_REMARK5 = "Remark5";
		/// <summary>税抜特別価格</summary>
		public const string FLAPS_API_RESULT_PRODUCT_SALE_PRICE = "SalePrice";
		/// <summary>税込特別価格</summary>
		public const string FLAPS_API_RESULT_PRODUCT_TAXED_SALE_PRICE = "TaxedSalePrice";
		/// <summary>EC販売開始日</summary>
		public const string FLAPS_API_RESULT_PRODUCT_WEB_DATE = "WebDate";
		/// <summary>重量</summary>
		public const string FLAPS_API_RESULT_PRODUCT_WEIGHT = "Weight";
		/// <summary>商品在庫結果モデル</summary>
		public const string FLAPS_API_RESULT_PRODUCT_STOCK = "productStock";
		/// <summary>メッセージ</summary>
		public const string FLAPS_API_RESULT_PRODUCT_STOCK_MESSAGE = "Message";
		/// <summary>商品在庫情報</summary>
		public const string FLAPS_API_RESULT_PRODUCT_STOCK_GOODS_STOCK = "GoodsStock";
		/// <summary>ショップカウンターコード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_STOCK_SALE_POINT_CODE = "SalePointCode";
		/// <summary>商品唯一番号</summary>
		public const string FLAPS_API_RESULT_PRODUCT_STOCK_GOODS_SER_NO = "GoodsSerNo";
		/// <summary>商品コード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_STOCK_GOODS_CODE = "GoodsCode";
		/// <summary>在庫数</summary>
		public const string FLAPS_API_RESULT_PRODUCT_STOCK_QTY = "Qty";
		/// <summary>商品スタイルコード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_STOCK_GOODS_STYLE_CODE = "GoodsStyleCode";
		/// <summary>色コード</summary>
		public const string FLAPS_API_RESULT_PRODUCT_STOCK_COLOR_CODE = "ColorCode";
		/// <summary>受注結果モデル</summary>
		public const string FLAPS_API_RESULT_ORDER = "order";
		/// <summary>メッセージ</summary>
		public const string FLAPS_API_RESULT_ORDER_MESSAGE = "Message";
		/// <summary>ショップカウンター業績唯一番号</summary>
		public const string FLAPS_API_RESULT_ORDER_POST_NO_SER_NO = "PosNoSerNo";
		/// <summary>販売コード</summary>
		public const string FLAPS_API_RESULT_ORDER_PIS_NO = "PisNo";
		/// <summary>電子発票番号</summary>
		public const string FLAPS_API_RESULT_ORDER_INVOICE_NO = "InvoiceNo";
		/// <summary>会員唯一番号</summary>
		public const string FLAPS_API_RESULT_ORDER_MEMBER_SER_NO = "MemberSerNo";
		/// <summary>会員コード</summary>
		public const string FLAPS_API_RESULT_ORDER_MEMBER_CODE = "MemberCode";
		/// <summary>受注キャンセル結果モデル</summary>
		public const string FLAPS_API_RESULT_ORDER_CANCELLATION = "order";
		/// <summary>メッセージ</summary>
		public const string FLAPS_API_RESULT_ORDER_CANCELLATION_MESSAGE = "Message";
		/// <summary>ショップカウンター業績唯一番号</summary>
		public const string FLAPS_API_RESULT_ORDER_CANCELLATION_POST_NO_SER_NO = "PosNoSerNo";

		//========================================================================
		// 各種設定区分
		//========================================================================
		//------------------------------------------------------
		// 共通
		//------------------------------------------------------
		// 戻るボタン非表示区分
		public const string KBN_BACK_BUTTON_HIDDEN = "1";		// 隠す
		public const string KBN_BACK_BUTTON_SHOWED = "0";		// 隠さない

		//------------------------------------------------------
		// 決済設定系
		//------------------------------------------------------
		/// <summary>カード決済区分</summary>
		public enum PaymentCard
		{
			/// <summary>ゼウス</summary>
			Zeus,
			/// <summary>ソフトバンクペイメント</summary>
			SBPS,
			/// <summary>GMO</summary>
			Gmo,
			/// <summary>YamatoKwc</summary>
			YamatoKwc,
			/// <summary>Zcom</summary>
			Zcom,
			/// <summary>e-SCTTE</summary>
			EScott,
			/// <summary>ベリトランス</summary>
			VeriTrans,
			/// <summary>Rakuten</summary>
			Rakuten,
			/// <summary>後払いcom連携</summary>
			Atobaraicom,
			/// <summary> ペイジェント</summary>
			Paygent,
		}

		/// <summary>コンビニ決済区分</summary>
		public enum PaymentCvs
		{
			/// <summary>電算システム</summary>
			Dsk,
			/// <summary>ソフトバンクペイメント</summary>
			SBPS,
			/// <summary>YamatoKwc</summary>
			YamatoKwc,
			/// <summary>GMO</summary>
			Gmo,
			/// <summary>Rakuten</summary>
			Rakuten,
			/// <summary>Zeus</summary>
			Zeus,
			/// <summary>Paygent</summary>
			Paygent,
		}

		/// <summary>コンビニ後払い区分</summary>
		public enum PaymentCvsDef
		{
			/// <summary>ヤマト</summary>
			YamatoKa,
			/// <summary>GMO</summary>
			Gmo,
			/// <summary>アトディーネ</summary>
			Atodene,
			/// <summary>DSK</summary>
			Dsk,
			/// <summary>後払いcom連携</summary>
			Atobaraicom,
			/// <summary>スコア@払い</summary>
			Score,
			/// <summary>ベリトランス</summary>
			Veritrans,
		}

		/// <summary>コンビニ後払い（SMS認証）区分</summary>
		public enum PaymentSmsDef
		{
			/// <summary>ヤマト</summary>
			YamatoKa,
		}

		/// <summary>銀行振込決済区分（互換性のために残す）</summary>
		public enum PaymentEbank
		{
		}

		/// <summary>クレジットカード決済区分(即時決済の場合は仮売り・本売上を指定)</summary>
		public enum PaymentCreditCardPaymentMethod
		{
			/// <summary>与信後決済</summary>
			PAYMENT_AFTER_AUTH,
			/// <summary>仮・本売上、即時決済</summary>
			PAYMENT_WITH_AUTH
		}

		/// <summary>GMOクレジット決済区分</summary>
		public enum GmoCreditCardPaymentMethod
		{
			/// <summary>有効性チェック</summary>
			Check,
			/// <summary>即時売上</summary>
			Capture,
			/// <summary>仮売上/実売上</summary>
			Auth
			/// <summary>簡易オーソリ(未対応)</summary>
			//sAuth
		}

		/// <summary>ベリトランスクレジット決済区分</summary>
		public enum VeritransCreditCardPaymentMethod
		{
			/// <summary>即時売上</summary>
			Capture,
			/// <summary>仮売上/実売上</summary>
			Auth
		}

		/// <summary>ペイジェントクレジット決済区分</summary>
		public enum PaygentCreditCardPaymentMethod
		{
			/// <summary>即時売上</summary>
			Capture,
			/// <summary>自動売上／手動売上</summary>
			Auth
		}

		/// <summary>PayPal決済区分</summary>
		public enum PayPalPaymentMethod
		{
			/// <summary>与信だけ取り売り上げはあとでたてる</summary>
			AUTH,
			/// <summary>与信と売り上げを両方たてる</summary>
			AUTH_WITH_SUBMIT
		}

		/// <summary>ZcomのAPIパラメーター「st_code」区分</summary>
		public enum ZcomStCode
		{
			/// <summary>NEWEB</summary>
			NW,
			/// <summary>新Neweb</summary>
			NNW,
		}

		/// <summary>Payment PayPay kbn</summary>
		public enum PaymentPayPayKbn
		{
			/// <summary>SBPS</summary>
			SBPS,
			/// <summary>GMO</summary>
			GMO,
			/// <summary>VeriTrans</summary>
			VeriTrans,
		}

		//------------------------------------------------------
		// 配送設定系
		//------------------------------------------------------
		/// <summary>配送料金優先設定</summary>
		public enum ShippingPriority
		{
			/// <summary>最も高い配送料金を優先</summary>
			HIGH,
			/// <summary>最も低い配送料金を優先</summary>
			LOW
		}

		/// <summary>Rakuten payment type</summary>
		public enum RakutenPaymentType
		{
			/// <summary>Auth</summary>
			AUTH,
			/// <summary>Capture</summary>
			CAPTURE
		}

		//------------------------------------------------------
		// 外部レコメンド連携設定系
		//------------------------------------------------------
		/// <summary>レコメンドエンジン区分</summary>
		public enum RecommendEngine
		{
			/// <summary>シルバーエッグ</summary>
			Silveregg,
			/// <summary>
			/// レコナイズ
			///（削除予定だが、商品データ連携バッチ初回実行のため残す。
			/// 　レコナイズ案件をシルバーエッグへ切り替え後は削除可）
			/// </summary>
			Reconize
		}

		/// <summary>レコメンド区分</summary>
		public enum RecommendKbn
		{
			/// <summary>商品レコメンド</summary>
			ProductRecommend,
			/// <summary>カテゴリレコメンド</summary>
			CategoryRecommend
		}

		/// <summary>商品購入制限利用区分</summary>
		public enum ProductOrderLimitKbn
		{
			//商品購入制限文言のみ表示（購入可能）
			ProductOrderLimitOff,
			//商品購入制限文言表示（購入不可能）
			ProductOrderLimitOn
		}

		/// <summary>Credit Auth Atack Block Method</summary>
		public enum CreditAuthAtackBlockMethod
		{
			// IP restrictions
			IP,
			// User restrictions
			User
		}

		//------------------------------------------------------
		// 注文系ステータス更新種別設定系
		//------------------------------------------------------
		/// <summary>注文系ステータス種別</summary>
		public enum StatusType
		{
			/// <summary>注文ステータス</summary>
			Order,
			/// <summary>入金ステータス</summary>
			Payment,
			/// <summary>督促ステータス</summary>
			Demand,
			/// <summary>返品交換ステータス</summary>
			RetuenExchange,
			/// <summary>返金ステータス</summary>
			Repayment
		}

		/// <summary>決済区分</summary>
		public enum PaymentPaidyKbn
		{
			/// <summary>Direct</summary>
			Direct,
			/// <summary>Paygent</summary>
			Paygent,
		}

		/// <summary>決済区分</summary>
		public enum PaymentBanknetKbn
		{
			/// <summary>Paygent</summary>
			Paygent,
		}

		/// <summary>Payment ATM kbn</summary>
		public enum PaymentATMKbn
		{
			/// <summary>Paygent</summary>
			Paygent,
		}

		//------------------------------------------------------
		// 一覧設定系
		//------------------------------------------------------
		// 商品一覧画像表示区分
		public const string KBN_REQUEST_DISP_IMG_KBN_ON = "1";					// 画像表示
		public const string KBN_REQUEST_DISP_IMG_KBN_OFF = "0";					// 画像非表示
		public const string KBN_REQUEST_DIST_IMG_KBN_WINDOWSHOPPING = "2";		// ウィンドウショッピング
		public static string KBN_REQUEST_DISP_IMG_KBN_DEFAULT = KBN_REQUEST_DISP_IMG_KBN_ON;

		// 商品一覧ソート区分
		public const string KBN_SORT_PRODUCT_LIST_PRODUCT_ID_ASC = "00";	// 商品ID/昇順
		public const string KBN_SORT_PRODUCT_LIST_PRODUCT_ID_DESC = "01";	// 商品ID/降順
		public const string KBN_SORT_PRODUCT_LIST_NAME_ASC = "02";			// 商品名/昇順
		public const string KBN_SORT_PRODUCT_LIST_NAME_DESC = "03";			// 商品名/降順
		public const string KBN_SORT_PRODUCT_LIST_NAME_KANA_ASC = "04";		// 商品フリガナ/昇順
		public const string KBN_SORT_PRODUCT_LIST_NAME_KANA_DESC = "05";	// 商品フリガナ/降順
		public const string KBN_SORT_PRODUCT_LIST_DATE_CREATED_ASC = "06";	// 作成日/昇順
		public const string KBN_SORT_PRODUCT_LIST_DATE_CREATED_DESC = "07";	// 作成日/降順（＝新着順）
		public const string KBN_SORT_PRODUCT_LIST_DATE_CHANGED_ASC = "08";	// 更新日/昇順
		public const string KBN_SORT_PRODUCT_LIST_DATE_CHANGED_DESC = "09";	// 更新日/降順
		public const string KBN_SORT_PRODUCT_LIST_NEW_ASC = "10";			// 販売開始日時/昇順（＝発売日順）
		public const string KBN_SORT_PRODUCT_LIST_NEW_DESC = "11";			// 販売開始日時/降順
		public const string KBN_SORT_PRODUCT_LIST_PRICE_ASC = "12";			// 価格/昇順
		public const string KBN_SORT_PRODUCT_LIST_PRICE_DESC = "13";		// 価格/降順
		public const string KBN_SORT_PRODUCT_LIST_FAVORITE_CNT_DESC = "14";	// お気に入り登録数/降順
		public const string KBN_SORT_PRODUCT_LIST_PRODUCT_GROUP_ITEM_NO_ASC = "99";	// 商品グループアイテム枝番/昇順
		public static string KBN_SORT_PRODUCT_LIST_DEFAULT = "";			// デフォルト

		// 商品一覧件数表示リンク
		public static List<int> NUMBER_DISPLAY_LINKS_PRODUCT_LIST = new List<int>();		// 商品一覧件数表示リンク

		// 在庫有無検索区分（リクエストパラメータ用）
		public const string KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_DISPLAY_NOSTOCK = "0";  // 全て表示
		public const string KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_UNDISPLAY_NOSTOCK = "1";  // 在庫無し非表示
		public const string KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_DISPLAY_NOSTOCK_BOTTOM = "2";  // 全て表示（在庫無しは後ろへ）
		public const string KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_DISPLAY_NOSTOCK_ONLY = "3";  // 在庫無しのみ表示
		public static string KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_DEFAULT = "";					// デフォルト

		// 定期購入フィルタ（リクエストパラメータ用）
		public const string KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_ALL = "0";  // 全て表示
		public const string KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_NORMAL = "1";  // 通常商品
		public const string KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_FIXED_PURCHASE = "2";  // 定期商品

		/// <summary>頒布会購入フィルタ（リクエストパラメータ用） : 通常商品</summary>
		public const string KBN_PRODUCT_LIST_SUBSCRIPTION_BOX_FILTER_NORMAL = "1";

		/// <summary>セール対象フィルタ（リクエストパラメータ用） : 全て表示</summary>
		public const string KBN_PRODUCT_LIST_SALE_ALL = "0";
		/// <summary>セール対象フィルタ（リクエストパラメータ用） : セールのみ表示</summary>
		public const string KBN_PRODUCT_LIST_SALE_ONLY = "1";

		/// <summary>決済種別選択タイプ : ラジオボタン</summary>
		public const string PAYMENT_CHOOSE_TYPE_RB = "RB";
		/// <summary>決済種別選択タイプ : ドロップダウン</summary>
		public const string PAYMENT_CHOOSE_TYPE_DDL = "DDL";

		//========================================================================
		// ディレクトリ・パス系
		//========================================================================
		// デザイン設定
		public const string KBN_MANAGER_DESIGN_SETTING_W2 = "w2";
		public const string KBN_MANAGER_DESIGN_SETTING_REPEATPLUS = "Repeat";
		public const string KBN_MANAGER_DESIGN_SETTING_REPEATFOOD = "RepeatFood";

		//コンフィグファイル名
		public const string FILENAME_APPALL_CONFIG = "AppAll.Config";			// AppAll.Configファイル

		// マスタ取込バッチ用ディレクトリ
		public const string DIRNAME_MASTERIMPORT_UPLOAD = "upload";			// アップロード
		public const string DIRNAME_MASTERIMPORT_ACTIVE = "active";			// 処理用
		public const string DIRNAME_MASTERIMPORT_COMPLETE = "complete";		// 完了

		// コンテンツディレクトリ
		public const string PATH_CONTENTS = "Contents/";
		// バックアップディレクトリ
		public const string PATH_AUTO_BACK_UP = "Contents/_AutoBackup";
		// コンテンツディレクトリ（モバイル用）
		public const string PATH_CONTENTS_MOBILE = "Cnts/";
		/// <summary>コンテンツページディレクトリ</summary>
		public const string PATH_CONTENTS_PAGE = "Page/";
		/// <summary>ランディングページディレクトリ</summary>
		public const string PATH_LANDING = "Landing/";
		/// <summary>CMSランディングページディレクトリ</summary>
		public const string PATH_LANDING_FORMLP = PATH_LANDING + "formlp/";
		/// <summary>特集ページディレクトリ</summary>
		public const string PATH_FEATURE_PAGE = "Page/Feature";
		// ショップメッセージ
		public const string FILE_XML_FRONT_SHOP_MESSAGE = "Contents/ShopMessage.xml";
		/// <summary>タグ出力個所：カートリスト</summary>
		public const string CARTLIST_TAG_NAME = "CartList";
		/// <summary>カートリストLPファイル名</summary>
		public const string CARTLIST_LP_FILE_NAME = "CartListLp.aspx";

		/// <summary>XML setting path: invoice setting</summary>
		public const string FILE_XML_FRONT_INVOICE_SETTING = @"Contents\Setting\Invoice\InvoiceSetting.xml";
		/// <summary>Logo invoice image default setting path</summary>
		public const string IMG_FRONT_LOGO_INVOICE_DEFAULT = @"Contents\Setting\Invoice\Images\ロゴ.png";

		// 画像ディレクトリ
		public const string PATH_IMAGES = "Contents/ImagesPkg/";					// 画像ディレクトリ
		public const string PATH_IMAGES_COLOR = "Contents/ProductColorImages/";		// 商品カラーイメージディレクトリ
		public const string PATH_PRODUCTIMAGES = "Contents/ProductImages/";			// 商品画像ディレクトリ（+shop_id）
		public const string PATH_PRODUCTSUBIMAGES = "Contents/ProductSubImages/";	// 商品サブ画像ディレクトリ（+shop_id）
		public const string PATH_RECOMMEND_BUTTONIMAGES = "Contents/RecommendButtonImages/";		// レコメンドボタン画像ディレクトリ
		public const string PATH_NATIONAL_FLAG = "Contents/NationalFlag/";				// グローバル対応 国旗画像ディレクトリパス
		public const string PATH_STAFF = "Contents/Staff/";								// スタッフサムネイルディレクトリパス
		public const string PATH_FEATURE_IMAGE = "Contents/Feature/";					// 特集画像パス
		public const string PATH_FEATUREPAGE_IMAGE = "Contents/FeaturePage/";			// 特集ページ画像パス
		public const string PATH_COORDINATE = "Contents/Coordinate/";					// コーディネート画像ディレクトリパス
		public const string PATH_LANDINGPAGE = "Contents/LandingPage/";					// ランディングページ画像ディレクトリパス
		public const string PATH_TEMP = "Contents/Temp/";								// 一時画像ディレクトリパス
		public const string PATH_TEMP_COORDINATE = "Coordinate";								// 一時画像ディレクトリパス内のコーディネート
		public const string PATH_TEMP_LANDINGPAGE = "LandingPage";								// 一時画像ディレクトリパス内のランディングページ
		public const string PATH_FEATUREAREA_ICON_IMAGE = "Images/Icon/FeatureArea/";			// 特集エリアタイプアイコンパス
		/// <summary>OGPタグ画像パス</summary>
		public const string PATH_OGP_TAG_IMAGE = "Contents/Feature/";

		/// <summary>Path select variation kbn icon image</summary>
		public const string PATH_SELECT_VARIATION_KBN_ICON_IMAGE = "Images/Icon/SelectVariationKbn/";
		/// <summary>Scoring sale directory path</summary>
		public const string PATH_SCORING_IMAGE = "Contents/Scoring/";
		/// <summary>Scoring sale question directory path</summary>
		public const string PATH_SCORING_QUESTION_IMAGE = "Contents/Scoring/Question/";
		/// <summary>Scoring sale image color theme directory path</summary>
		public const string PATH_SCORINGSALE_IMG_COLOR_THEME = "Images/ColorTheme/";
		/// <summary>Scoring sale temp directory path</summary>
		public const string PATH_TEMP_SCORINGSALE = "Scoring";

		// 画像ディレクトリ（モバイル用）
		public const string PATH_IMAGES_MOBILE = "Cnts/imgs/";						// 画像ディレクトリ
		public const string PATH_PRODUCTIMAGES_MOBILE = "Cnts/pimgs/";				// 商品画像ディレクトリ（+shop_id）
		public const string PATH_PRODUCT_SUB_IMAGES_MOBILE = "Cnts/psimgs/";		// 商品画像サブディレクトリ（+shop_id）

		// 画像・マネージャサイド
		public const string IMG_FRONT_PRODUCT_ICON1 = PATH_PRODUCTIMAGES + "ProductIcon1.gif";
		public const string IMG_FRONT_PRODUCT_ICON2 = PATH_PRODUCTIMAGES + "ProductIcon2.gif";
		public const string IMG_FRONT_PRODUCT_ICON3 = PATH_PRODUCTIMAGES + "ProductIcon3.gif";
		public const string IMG_FRONT_PRODUCT_ICON4 = PATH_PRODUCTIMAGES + "ProductIcon4.gif";
		public const string IMG_FRONT_PRODUCT_ICON5 = PATH_PRODUCTIMAGES + "ProductIcon5.gif";
		public const string IMG_FRONT_PRODUCT_ICON6 = PATH_PRODUCTIMAGES + "ProductIcon6.gif";
		public const string IMG_FRONT_PRODUCT_ICON7 = PATH_PRODUCTIMAGES + "ProductIcon7.gif";
		public const string IMG_FRONT_PRODUCT_ICON8 = PATH_PRODUCTIMAGES + "ProductIcon8.gif";
		public const string IMG_FRONT_PRODUCT_ICON9 = PATH_PRODUCTIMAGES + "ProductIcon9.gif";
		public const string IMG_FRONT_PRODUCT_ICON10 = PATH_PRODUCTIMAGES + "ProductIcon10.gif";

		/// <summary>Path setting</summary>
		public const string PATH_SETTINGS = "Contents\\Settings\\";

		// 通常納品書テンプレートファイル名
		readonly public static string[] PDF_TEMPLATES_PATH_INVOICE_NORMAL = {
			"order_invoice.pdf",
			"order_invoice_next.pdf" };

		// 楽天納品書テンプレート
		readonly public static string[] PDF_TEMPLATES_PATH_INVOICE_RAKUKTEN = {
			"order_invoice_rakuten.pdf",
			"order_invoice_rakuten_next.pdf" };

		// ヤフー納品書テンプレート
		readonly public static string[] PDF_TEMPLATES_PATH_INVOICE_YAHOO = {
			"order_invoice_yahoo.pdf",
			"order_invoice_yahoo_next.pdf" };

		/// <summary>コンビニメッセージ SBPS(電算システム利用）</summary>
		public const string PATH_XML_CVS_SBPS_DSK = @"Xml\Message\SBPSCvsDskMessages.xml";
		/// <summary>コンビニメッセージ ヤマトKWC</summary>
		public const string PATH_XML_CVS_YAMATOKWC = @"Xml\Message\YamatoKwcCvsMessages.xml";
		/// <summary>コンビニメッセージ GMOコンビニ前払い</summary>
		public const string PATH_XML_CVS_GMO = @"Xml\Message\GMOCvsMessages.xml";
		/// <summary>Path xml cvs rakuten message</summary>
		public const string PATH_XML_CVS_RAKUTEN = @"Xml\Message\RakutenCvsMessages.xml";
		/// <summary>コンビニメッセージ ZEUSコンビニ前払い</summary>
		public const string PATH_XML_CVS_ZEUS = @"Xml\Message\ZEUSCvsMessage.xml";
		/// <summary>コンビニメッセージ Paygentコンビニ前払い</summary>
		public const string PATH_XML_CVS_PAYGENT = @"Xml\Message\PaygentCvsMessage.xml";

		/// <summary>Path xml Paygent Banknet message</summary>
		public const string PATH_XML_PAYGENT_BANKNET_PAYGENT = @"Xml\Message\PaygentBanknetMessage.xml";

		/// <summary>Xml paygent ATM message</summary>
		public const string PATH_XML_ATM_PAYGENT = @"Xml\Message\PaygentAtmMessage.xml";

		/// <summary>商品表示情報更新ファイル</summary>
		public const string FILENAME_DISPLAYPRODUCT_REFRESH = "DisplayProduct.Refresh";
		/// <summary>セットプロモーション情報更新ファイル</summary>
		public const string FILENAME_SETPROMOTION_REFRESH = "SetPromotion.Refresh";
		/// <summary>新着情報表示更新ファイル</summary>
		public const string FILENAME_NEWS_REFRESH = "News.Refresh";
		/// <summary>会員ランク情報表示更新ファイル</summary>
		public const string FILENAME_MEMBERRANK_REFRESH = "MemberRank.Refresh";
		/// <summary>ポイントルール情報表示更新ファイル</summary>
		public const string FILENAME_POINTRULES_REFRESH = "PointRules.Refresh";
		/// <summary>ノベルティ設定更新ファイル</summary>
		public const string FILENAME_NOVELTY_REFRESH = "Novelty.Refresh";
		/// <summary>レコメンド設定更新ファイル</summary>
		public const string FILENAME_RECOMMEND_REFRESH = "Recommend.Refresh";
		/// <summary>SEOメタデータ更新ファイル</summary>
		public const string FILENAME_SEO_METADATDAS_REFRESH = "SeoMetadatas.Refresh";
		/// <summary>商品一覧表示設定ファイル</summary>
		public const string FILENAME_PRODUCT_LIST_DISP_SETTING_REFRESH = "ProductListDispSetting.Refresh";
		/// <summary>ユーザー拡張項目設定ファイル</summary>
		public const string FILENAME_USER_EXTEND_SETTING_REFRESH = "UserExtendSetting.Refresh";
		/// <summary>ショートURLファイル</summary>
		public const string FILENAME_SHORTURL_REFRESH = "ShortUrl.Refresh";
		/// <summary>>定期購入キャンセル理由</summary>
		public const string FILENAME_FIXEDPURCHASE_CANCEL_REASON_REFRESH = "FixedPurchaseCancelReason .Refresh";
		/// <summary>商品グループファイル</summary>
		public const string FILENAME_PRODUCTGROUP_REFRESH = "ProductGroup.Refresh";
		/// <summary>翻訳ファイル</summary>
		public const string FILENAME_TRANSLATION_REFRESH = "Translation.Refresh";
		/// <summary>カラーマップ設定XMLファイル</summary>
		public const string FILENAME_COLOR_REFRESH = "ProductColors.xml";
		/// <summary>商品一覧表示設定ファイル</summary>
		public const string FILENAME_AUTO_TRANSLATION_WORD_REFRESH = "AutoTranslationWord.Refresh";
		/// <summary>国ISOコード用リフレッシュファイル</summary>
		public const string FILENAME_COUNTRY_LOCATION_REFRESH = "CountryLocation.Refresh";
		/// <summary>アフィリエイトタグ設定ファイル</summary>
		public const string FILENAME_AFFILIATE_TAG_SETTING_REFRESH = "AffiliateTagSetting.Refresh";
		/// <summary>広告コード設定ファイル</summary>
		public const string FILENAME_ADVCODE_REFRESH = "AdvCode.Refresh";
		/// <summary>ページ管理設定ファイル</summary>
		public const string FILENAME_PAGE_DESIGN_REFRESH = "PageDesign.Refresh";
		/// <summary>パーツ管理設定ファイル</summary>
		public const string FILENAME_PARTS_DESIGN_REFRESH = "PartsDesign.Refresh";
		/// <summary>特集ページ管理設定ファイル</summary>
		public const string FILENAME_FEATURE_PAGE_DESIGN_REFRESH = "FeaturePage.Refresh";
		/// <summary>特集エリアバナー設定ファイル</summary>
		public const string FILENAME_FEATURE_AREA_BANNER_REFRESH = "FeatureAreaBanner.Refresh";
		/// <summary>OGPタグ設定ファイル</summary>
		public const string FILENAME_OGPTAGSETTING_REFRESH = "OgpTagSetting.Refresh";
		/// <summary>ランディングページデザインリフレッシュファイル</summary>
		public const string FILENAME_LANDINGPAGEDESIGN_REFRESH = "LandingPageDesign.Refresh";
		/// <summary>配送種別リフレッシュファイル</summary>
		public const string FILENAME_SHOPSHIPPING_REFRESH = "ShopShipping.Refresh";
		/// <summary>商品サブ画像設定リフレッシュファイル</summary>
		public const string FILENAME_SUBIMAGE_SETTING_REFRESH = "ProductSubImageSetting.Refresh";
		/// <summary>決済種別リフレッシュファイル</summary>
		public const string FILENAME_PAYMENT_REFRESH = "Payment.Refresh";
		/// <summary>注文拡張項目設定リフレッシュファイル</summary>
		public const string FILENAME_ORDEREXTENDSETTING_REFRESH = "OrderExtendSetting.Refresh";
		/// <summary>File name real shop area refresh</summary>
		public const string FILENAME_REALSHOP_AREA_REFRESH = "RealShopArea.Refresh";
		/// <summary>メンテナンス情報更新ファイル</summary>
		public const string FILENAME_MAINTENANCE_REFRESH = "Maintenance.Refresh";
		/// <summary>Filename scoring sale refresh</summary>
		public const string FILENAME_SCORINGSALE_REFRESH = "ScoringSale.Refresh";
		/// <summary>頒布会設定リフレッシュファイル</summary>
		public const string FILENAME_SUBSCRIPTIONBOX_REFRESH = "SubscriptionBox.Refresh";
		/// <summary>File name exchange rate refresh</summary>
		public const string FILENAME_EXCHANGERATE_REFRESH = "ExchangeRate.Refresh";
		/// <summary>リアル店舗リフレッシュファイル</summary>
		public const string FILENAME_REALSHOP_REFRESH = "RealShop.Refresh";

		//========================================================================
		// 定数
		//========================================================================
		/// <summary>長い日付フォーマット</summary>
		public const string DATE_FORMAT_LONG = "yyyyMMddHHmmss";
		/// <summary>短い日付フォーマット</summary>
		public const string DATE_FORMAT_SHORT = "yyyyMMdd";
		/// <summary>日本の長い日付フォーマット</summary>
		public const string JAPAN_DATE_FORMAT_LONG = "yyyy年MM月dd日 HH:mm:ss";
		/// <summary>日本の短い日付フォーマット</summary>
		public const string JAPAN_DATE_FORMAT_SHORT = "yyyy年MM月dd日";

		//------------------------------------------------------
		// 共通系
		//------------------------------------------------------
		/// <summary>最終更新者：バッチ</summary>
		public static string FLG_LASTCHANGED_BATCH = "batch";
		/// <summary>最終更新者：ユーザー</summary>
		public static string FLG_LASTCHANGED_USER = "user";
		/// <summary>最終更新者：CGI（入金通知など）</summary>
		public static string FLG_LASTCHANGED_CGI = "cgi";
		/// <summary>最終更新者：システム（ログインロックなど）</summary>
		public static string FLG_LASTCHANGED_SYSTEM = "system";
		/// <summary>最終更新者 : botchan</summary>
		public static string FLG_LASTCHANGED_BOTCHAN = "botchan";
		/// <summary>最終更新者 : flaps</summary>
		public static string FLG_LASTCHANGED_FLAPS = "flaps";
		/// <summary>最終更新者 : CrossPoint</summary>
		public static string FLG_LASTCHANGED_CROSSPOINT = "crosspoint";

		//------------------------------------------------------
		// ユーザ系
		//------------------------------------------------------
		public static string CONST_USER_ID_HEADER = "N";	// ユーザーID先頭文字
		public static int CONST_USER_ID_LENGTH = 9;		// ユーザーID文字数

		public static int CONST_POSSIBLE_TRIAL_LOGIN_COUNT = 5; // ログイン試行可能回数
		public static int CONST_ACCOUNT_LOCK_VALID_MINUTES = 30; // アカウントロックの有効時間（分）

		public static int CONST_PASSWORDREMAINDER_VALID_MINUTES = 120; // パスワードリマインダー情報有効時間

		/// <summary>クレジット与信アタック試行可能回数(nullは制御しない)</summary>
		public static int? CONST_POSSIBLE_TRIAL_CREDIT_AUTH_ATTACK_COUNT = 5;
		/// <summary>クレジット与信アタックロックの有効時間（分）</summary>
		public static int CONST_CREDIT_AUTH_ATTACK_LOCK_VALID_MINUTES = 60;

		/// <summary> 顧客区分「全ての会員」「全てのゲスト」(ValueTextのキー) </summary>
		public const string VALUE_TEXT_KEY_USER_USER_KBN_ALL = "user_kbn_all";
		/// <summary> 顧客区分ツールチップ </summary>
		public const string VALUE_TEXT_FIELD_USER_KBN_TOOLTIP = "user_kbn_tooltip";

		/// <summary> クレジットカード枝番 </summary>
		public const string REQUEST_KEY_CREDITCARD_NO = "bno";

		//------------------------------------------------------
		// 商品系
		//------------------------------------------------------
		public const string CONST_PRODUCTVARIATIONNAME_PARENTHESIS_LEFT = "(";		// バリエーション名左括弧
		public const string CONST_PRODUCTVARIATIONNAME_PARENTHESIS_RIGHT = ")";		// バリエーション名右括弧
		public const string CONST_PRODUCTVARIATIONNAME_PUNCTUATION = "-";			// バリエーション名区切り文字

		// 商品検索フォーム最大文字数
		public const int CONST_PRODUCT_SEARCH_WORD_MAX_LENGTH = 100;

		// リコメンド・ランキング情報更新待ち時間（分）
		public const int CONST_DISPPRODUCT_UPDATE_INTERVAL_MINUTE = 10;

		// 商品サブ画像名フッタ
		public static string PRODUCTSUBIMAGE_FOOTER = "_sub";
		// 商品サブ画像番号フォーマット
		public static string PRODUCTSUBIMAGE_NUMBERFORMAT = "00";
		/// <summary>商品サブ画像Noの上限値(上限値 + 1した場合はメイン画像として扱う)</summary>
		public const int PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO = 99;

		/// <summary>Product image no image header</summary>
		public const string PRODUCTIMAGE_NOIMAGE_HEADER = "NowPrinting";

		// 商品画像名ヘッダ使用オプション
		public static bool PRODUCT_IMAGE_HEAD_ENABLED = false;

		/// <summary>Yahoo API - 認可コードを取得するためにアクセスするエンドポイント (Authorizationエンドポイント)</summary>
		/// <remarks>認可コード: アクセストークンを取得するために必要な文字列</remarks>
		public static string YAHOO_API_AUTH_API_URL = "";
		/// <summary>Yahoo API - アクセストークンを取得するためにアクセスするエンドポイント(Tokenエンドポイント)</summary>
		public static string YAHOO_API_TOKEN_API_URL = "";
		/// <summary>Yahoo API - 注文詳細APIを実行するためのURL</summary>
		public static string YAHOO_API_ORDERINFO_API_URL = "";

		// 商品画像名フッタ
		public const string PRODUCTIMAGE_FOOTER = "_(L|LL|M|S).jpg";
		public const string PRODUCTIMAGE_FOOTER_S = "_S.jpg";
		public const string PRODUCTIMAGE_FOOTER_M = "_M.jpg";
		public const string PRODUCTIMAGE_FOOTER_L = "_L.jpg";
		public const string PRODUCTIMAGE_FOOTER_LL = "_LL.jpg";
		/// <summary>商品画像圧縮クオリティ（10～100）</summary>
		public static int PRODUCTIMAGE_ENCODE_QUALITY = 95;

		/// <summary>CMS ページ管理・パーツ管理での最大表示件数</summary>
		public static int PAGE_PARTS_MAX_VIEW_CONTENT_COUNT = 500;
		/// <summary>CMS ページ管理画面内に表示されるパーツ一覧における最大表示件数</summary>
		public static int PARTS_IN_PAGEDESIGN_MAX_VIEW_CONTENT_COUNT = 45;
		/// <summary>CMS ツリービューにおける最大表示件数</summary>
		public static int TREEVIEW_MAX_VIEW_CONTENT_COUNT = 30000;
		/// <summary>CMS プレビューURL ELBで分散されている場合に利用 空の場合はEnvSettingの設定が利用されます</summary>
		public static string PREVIEW_URL = "";
		/// <summary>CMS プレビュー時 ELBで分散されている場合に利用 基本認証ID/PW：IDとPWは半角コロンで区切る</summary>
		public static string PREVIEW_BASIC_AUTHENTICATION_USER_ACCOUNT = "";

		/// <summary>商品連携IDのカラム数</summary>
		public const int COOPERATION_ID_COLUMNS_COUNT = 10;

		// 商品付帯情報
		/// <summary>商品付帯情報：商品付帯情報表示区分：チェックボックス</summary>
		public const string PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX = "C";
		/// <summary>商品付帯情報：商品付帯情報表示区分：セレクトメニュー</summary>
		public const string PRODUCTOPTIONVALUES_DISP_KBN_SELECTMENU = "S";
		/// <summary>商品付帯情報：商品付帯情報表示区分：ドロップダウン(価格)</summary>
		public const string PRODUCTOPTIONVALUES_DISP_KBN_PRICE_DROPDOWNMENU = "SP";
		/// <summary>商品付帯情報：商品付帯情報表示区分：チェックボックス(価格)</summary>
		public const string PRODUCTOPTIONVALUES_DISP_KBN_PRICE_CHECKBOX = "CP";
		/// <summary>商品付帯情報：商品付帯情報表示区分：テキストボックスの形式 </summary>
		public const string PRODUCTOPTIONVALUES_DISP_KBN_TEXTBOX = "T";
		/// <summary>商品付帯情報：設定値区切り文字</summary>
		public const string PRODUCTOPTIONVALUES_SEPARATING_CHAR_SETTING_VALUE = "@@";
		/// <summary>商品付帯情報：設定値カート投入URL区切り文字</summary>
		public const string PRODUCTOPTION_VALUES_URL_SEPARATING_CHAR_SETTING_VALUE = "_";
		/// <summary>商品付帯情報：選択設定値区切り文字（チェックボックス）</summary>
		public const string PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE = "$$";
		/// <summary>商品付帯情報：商品付帯情報項目名出力</summary>
		public const string PRODUCTOPTIONVALUES_HTML_TAG_NAME_HEAD = "ProductOptionValueName";
		/// <summary>商品付帯情報：HTMLタグ出力値ヘッダ</summary>
		public const string PRODUCTOPTIONVALUES_HTML_TAG_VALUE_HEAD = "ProductOptionValue";
		/// <summary>商品付帯情報：付帯情報最大数</summary>
		public static int PRODUCTOPTIONVALUES_MAX_COUNT = 5;
		/// <summary>商品付帯情報：エスケープ対象文字</summary>
		public const string PRODUCTOPTIONVALUES_ESCAPE_TARGET_CHAR = "@";
		/// <summary>商品付帯情報：エスケープ文字列</summary>
		public const string PRODUCTOPTIONVALUES_ESCAPE_STR = "&#65520;";

		/// <summary>ポイント単位</summary>
		public const string CONST_UNIT_POINT_PT = "pt";

		/// <summary>ポイント有効期間計算_追加月数</summary>
		public static int CALC_POINT_EXP_ADDMONTH = 0;

		/// <summary>会員ランク価格（バリエーション）フィールド</summary>
		public static string FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION = "member_rank_price_variation";

		/// <summary>表示期間内フラグ フィールド</summary>
		public static string FIELD_DISP_PERIOD_FLG = "disp_period_flg";
		/// <summary>表示可能会員ランク フィールド</summary>
		public static string FIELD_DISP_ENABLE_RANK = "disp_enable_rank";
		/// <summary>表示可能会員ランクフラグ フィールド</summary>
		public static string FIELD_DISP_ENABLE_RANK_FLG = "disp_enable_rank_flg";
		/// <summary>定期会員限定商品フラグ フィールド</summary>
		public static string FIELD_DISP_ONLY_FIXED_FLG = "disp_only_fixed_flg";

		/// <summary>商品セール情報</summary>
		public const int CONST_PRODUCTSALE_ID_LENGTH = 8;

		/// <summary>定期商品接頭辞</summary>
		public static string PRODUCT_FIXED_PURCHASE_STRING = "[定期] ";
		/// <summary>頒布会商品接頭辞</summary>
		public static string PRODUCT_SUBSCRIPTION_BOX_STRING = "[頒布会] ";

		/// <summary>アイコンフラグ有効終了日の配列</summary>
		public static readonly string[] ICON_FLG_TERM_END_NAMES =
		{
			FIELD_PRODUCT_ICON_TERM_END1,
			FIELD_PRODUCT_ICON_TERM_END2,
			FIELD_PRODUCT_ICON_TERM_END3,
			FIELD_PRODUCT_ICON_TERM_END4,
			FIELD_PRODUCT_ICON_TERM_END5,
			FIELD_PRODUCT_ICON_TERM_END6,
			FIELD_PRODUCT_ICON_TERM_END7,
			FIELD_PRODUCT_ICON_TERM_END8,
			FIELD_PRODUCT_ICON_TERM_END9,
			FIELD_PRODUCT_ICON_TERM_END10
		};

		//------------------------------------------------------
		// 注文系
		//------------------------------------------------------
		public const int CONST_CART_ID_LENGTH = 12;
		public static int CONST_ORDER_ID_LENGTH = 5;
		public const string CONST_DEFAULT_SHIPPING_ADDR1 = "東京都";	// デフォルト配送料計算用の都道府県（カートで表示）

		/// <summary>Default shipping address 2 Taiwan key</summary>
		public const string CONST_DEFAULT_SHIPPING_ADDRESS2_TW = "臺北市";

		public static bool DISPLAY_DEFAULTSHIPPINGDATE_ENABLED = true;	// 配送希望日のデフォルト値指定可否
		/// <summary>注文通知受信タイムアウト(秒)</summary>
		public const int CONST_RECEIVE_ORDER_NOTICE_TIMEOUT = 10;
		public const string CHAR_MASKING_FOR_TOKEN = "*";

		/// <summary>購入履歴変更(マイページ) : 変更条件に配送日付を考慮するかどうか 考慮する:true 考慮しない:false</summary>
		public static int? MYPAGE_ORDER_MODIFY_BY_DATE = null;

		/// <summary> 注文者区分「全ての会員」「全てのゲスト」(ValueTextのキー) </summary>
		public const string VALUE_TEXT_KEY_ORDEROWNER_USER_KBN_ALL = "owner_user_kbn_all";

		 /// <summary> 定期解約時アラートを表示するかどうか </summary>
		public static string[] AlertOrderStatusForCancelFixedPurchase_ENABLED = new string[] { };

        //------------------------------------------------------
        // 商品系
        //------------------------------------------------------
        /// <summary>商品IDを含まないバリエーションID</summary>
        public const string FIELD_PRODUCTVARIATION_V_ID = "v_id";

		//------------------------------------------------------
		// カテゴリ系
		//------------------------------------------------------
		public const int CONST_CATEGORY_ID_LENGTH = 3;		// カテゴリIDは3桁単位
		public const int CONST_CATEGORY_DEPTH = 10;			// カテゴリは10階層まで

		//------------------------------------------------------
		// メールテンプレート系
		//------------------------------------------------------
		public const string CONST_MAIL_ID_USER_REGIST = "00000010";				// 会員登録メールテンプレート
		public const string CONST_MAIL_ID_USER_EASY_REGIST = "00000011";		// かんたん会員登録メールテンプレート
		public const string CONST_MAIL_ID_USER_PREREGIST = "00000015";			// 会員仮登録メールテンプレート（空メールなど）
		public const string CONST_MAIL_ID_PASSWORD_REMINDER = "00000020";		// パスワードリマインダメールテンプレート
		public const string CONST_MAIL_ID_MAILMAGAZINE_CANCEL = "00000021";		// メールマガジン解除メールテンプレート
		public const string CONST_MAIL_ID_ORDER_COMPLETE = "00000050";			// 注文完了メールテンプレート
		public const string CONST_MAIL_ID_ORDER_COMPLETE_FOR_OPERATOR = "00000051";			// 注文完了メールテンプレート(管理者向け)
		public const string CONST_MAIL_ID_ORDER_SHIPPING = "00000060";			// 出荷手配済みメールテンプレート
		public const string CONST_MAIL_ID_ORDER_PAYMENT = "00000070";			// 入金確認メールテンプレート
		public const string CONST_MAIL_ID_ORDER_CANCEL = "00000080";			// キャンセルメールテンプレート
		public const string CONST_MAIL_ID_ORDER_RETURN = "00000090";			// 返品済メールテンプレート
		public const string CONST_MAIL_ID_DOCOMO_PAYMENT_REPORT = "00000100";	// ドコモから送信されるCSVレポート用テンプレート
		public const string CONST_MAIL_ID_DOCOMO_PAYMENT_NEXT = "00000101";		// ドコモケータイ払い携帯引継ぎメールテンプレート
		public const string CONST_MAIL_ID_RAKUTEN_COOP_ERROR = "00000300";		// 楽天API連携エラーメールテンプレート
		public const string CONST_MAIL_ID_MASTER_IMPORT = "10000010";			// マスタ取込メールテンプレート
		public const string CONST_MAIL_ID_MASTER_UPLOAD_STOCK_COOPERATION = "10000011";			// 定期実行マスタ取込結果メールテンプレート
		public const string CONST_MAIL_ID_EXTERNAL_IMPORT = "10000015";			// 外部ファイル取込メールテンプレート
		//public const string CONST_MAIL_ID_DELETE_CART = "10000020";				// カート削除バッチメールテンプレート
		//public const string CONST_MAIL_ID_CVSPAYMENT_CONFIRM = "10000040";		// コンビニ入金確認バッチメールテンプレート
		public const string CONST_MAIL_ID_FIXEDPURCHASE_FOR_USER = "10000050";	// 定期購入アラート(ユーザ向け)メールテンプレート
		public const string CONST_MAIL_ID_FIXEDPURCHASE_FOR_OPERATOR = "10000051";	// 定期購入アラート(管理者向け)テンプレート
		public const string CONST_MAIL_ID_CREATE_USERINTEGRATION = "10000060";	// ユーザー統合作成バッチ結果メールテンプレート
		public const string CONST_MAIL_ID_CUSTOMER_RINGS_EXPORT = "10000061";	// カスタマーリングス連携バッチ結果メールテンプレート
		public const string CONST_MAIL_ID_REAUTH_ERROR_ADMIN = "10000070";		// 再与信エラーメール（管理者向け）
		public const string CONST_MAIL_ID_REAUTH_ERROR_USER = "10000071";		// 再与信エラーメール（ユーザー向け）
		public const string CONST_MAIL_ID_URERU_AD_IMPORT_FOR_OPERATOR = "10000080";	// 外部連携注文取込エラーメールテンプレート（管理者向け）
		public const string CONST_MAIL_ID_URERU_AD_IMPORT_FOR_USER = "10000081";	// 外部連携注文取込 与信取得エラーメールテンプレート（ユーザー向け）
		public const string CONST_MAIL_ID_SILVEREGGAIGENT_FILEUPLOADER_FOR_OPERATOR = "10000090";	// シルバーエッグアイジェント商品データ連携バッチメールテンプレート（管理者向け）
		public const string CONST_MAIL_ID_NP_PAYMENT_SYSTEM_NOTIFICATION_FOR_OPERATOR = "10000091";	// NP payment system notification [for managers]
		public const string CONST_MAIL_ID_INQUIRY_INPUT_FOR_OPERATOR = "00000022";			// 問合せテンプレート(管理者向け)
		public const string CONST_MAIL_ID_INQUIRY_INPUT_FOR＿USER = "00000023";			// 問合せテンプレート(ユーザ向け)
		public const string CONST_MAIL_ID_MAILMAGAZINE_REGIST = "00000024";		// メールマガジン登録メールテンプレート
		public const string CONST_MAIL_ID_ACCEPT_PRODUCT_ARRIVAL = "00000120";	// 再入荷通知受付メールテンプレート
		public const string CONST_MAIL_ID_ACCEPT_PRODUCT_RELEASE = "00000121";	// 販売開始通知受付メールテンプレート
		public const string CONST_MAIL_ID_ACCEPT_PRODUCT_RESALE = "00000122";	// 再販売通知受付メールテンプレート
		public const string CONST_MAIL_ID_NOTICE_PRODUCT_ARRIVAL = "00000110";	// 再入荷通知メールテンプレート
		public const string CONST_MAIL_ID_NOTICE_PRODUCT_RELEASE = "00000111";	// 販売開始通知メールテンプレート
		public const string CONST_MAIL_ID_NOTICE_PRODUCT_RESALE = "00000112";	// 再販売通知メールテンプレート
		public const string CONST_MAIL_ID_STOCK_ALERT_RESALE = "00000113";	// 在庫残り僅かのお知らせメールテンプレート
		public const string CONST_MAIL_ID_CONFIRM_FIXEDPURCHASE_STATE = "00000130";		// 定期購入状況の確認メールテンプレート
		/// <summary>定期購入変更期限案内テンプレート</summary>
		public const string CONST_MAIL_ID_CHANGE_DEADLINE = "00000131";
		public const string CONST_MAIL_ID_SKIP_FIXEDPURCHASE = "00000132";				// 定期購入スキップメールテンプレート
		public const string CONST_MAIL_ID_CANCEL_FIXEDPURCHASE = "00000134";			// 定期購入停止メールテンプレート
		public const string CONST_MAIL_ID_RESUME_FIXEDPURCHASE = "00000138";			// 定期購入再開メールテンプレート
		public const string CONST_MAIL_ID_CHANGE_FIXEDPURCHASE_PRODUCT = "00000136";	// 定期購入商品の変更メールテンプレート
		public const string CONST_MAIL_ID_SUSPEND_FIXEDPURCHASE = "00000137";			// 定期購入休止メールテンプレート
		public const string CONST_MAIL_ID_CHANGE_PAYMENT_METHOD_FOR_USER = "00000140";	// 購入履歴お支払い方法変更メール
		public const string CONST_MAIL_ID_CHANGE_PAYMENT_METHOD_FOR_OPERATOR = "00000141";	// 購入履歴お支払い方法変更メール(管理者向け)
		public const string CONST_MAIL_ID_CHANGE_DELIVERY_DATE_FOR_USER = "00000142";	// 購入履歴お届け日変更メール
		public const string CONST_MAIL_ID_CHANGE_DELIVERY_DATE_FOR_OPERATOR = "00000143";	// 購入履歴お届け日変更メール(管理者向け)
		public const string CONST_MAIL_ID_CHANGE_SHIPPING_ADDRESS_FOR_USER = "00000144";	// 購入履歴お届け先変更メール
		public const string CONST_MAIL_ID_CHANGE_SHIPPING_ADDRESS_FOR_OPERATOR = "00000145";	// 購入履歴お届け先変更メール(管理者向け)
		public const string CONST_MAIL_ID_CHANGE_POINTS_FOR_USER = "00000146";	// 購入履歴利用ポイント変更メール
		public const string CONST_MAIL_ID_CHANGE_POINTS_FOR_OPERATOR = "00000147";	// 購入履歴利用ポイント変更メール(管理者向け)
		public const string CONST_MAIL_ID_CHANGE_ACCOUNT_FOR_USER = "00000150";	// ユーザー登録情報の変更メール
		public const string CONST_MAIL_ID_CHANGE_ACCOUNT_FOR_OPERATOR = "00000151";	// ユーザー登録情報の変更メール(管理者向け)
		public const string CONST_MAIL_ID_CHANGE_ACCOUNT_ADDRESS_FOR_USER = "00000152";	// アドレス帳変更メール
		public const string CONST_MAIL_ID_CHANGE_ACCOUNT_ADDRESS_FOR_OPERATOR = "00000153";	// アドレス帳変更メール(管理者向け)
		public const string CONST_MAIL_ID_CREDITCARD_REGIST_FOR_USER = "00000154";	// クレジットカード登録案内メール
		public const string CONST_MAIL_ID_CREDITCARD_REGIST_FOR_OPERATOR = "00000155";	// クレジットカード登録案内メール(管理者向け)
		public const string CONST_MAIL_ID_CHANGE_ORDERER_FROM_MANAGER_SITE = "00000156";	// 注文情報変更メール(管理画面)
		public const string CONST_MAIL_ID_CHANGE_PAYMENT_METHOD_FIXEDPURCHASE_FOR_USER = "00000157";	// 定期購入情報お支払い方法変更メール
		public const string CONST_MAIL_ID_CHANGE_PAYMENT_METHOD_FIXEDPURCHASE_FOR_OPERATOR = "00000158";	// 定期購入情報お支払い方法変更メール(管理者向け)
		public const string CONST_MAIL_ID_CHANGE_DELIVERY_DATE_FIXEDPURCHASE_FOR_USER = "00000159";	// 定期購入情報配送パターン変更メール
		public const string CONST_MAIL_ID_CHANGE_DELIVERY_DATE_FIXEDPURCHASE_FOR_OPERATOR = "00000160";	// 定期購入情報配送パターン変更メール(管理者向け)
		public const string CONST_MAIL_ID_CHANGE_SHIPPING_ADDRESS_FIXEDPURCHASE_FOR_USER = "00000161";	// 定期購入情報お届け先変更メール
		public const string CONST_MAIL_ID_CHANGE_SHIPPING_ADDRESS_FIXEDPURCHASE_FOR_OPERATOR = "00000162";	// 定期購入情報お届け先変更メール(管理者向け)
		public const string CONST_MAIL_ID_CHANGE_POINTS_FIXEDPURCHASE_FOR_USER = "00000163";	// 定期購入情報利用ポイント変更メール
		public const string CONST_MAIL_ID_CHANGE_POINTS_FIXEDPURCHASE_FOR_OPERATOR = "00000164";	// 定期購入情報利用ポイント変更メール(管理者向け)
		public const string CONST_MAIL_ID_CHANGE_SHIPPING_DATE_FIXEDPURCHASE_FOR_USER = "00000165";	// 定期購入情報次回配送日変更メール
		public const string CONST_MAIL_ID_CHANGE_SHIPPING_DATE_FIXEDPURCHASE_FOR_OPERATOR = "00000166";	// 定期購入情報次回配送日変更メール(管理者向け)
		public const string CONST_MAIL_ID_CHANGE_FIXEDPURCHASE_PRODUCT_FOR_OPERATOR = "00000167";	// 定期購入商品の変更メール(管理者向け)
		public const string CONST_MAIL_ID_ORDER_UPDATE_FOR_USER = "00000171";	// 注文更新通知メール
		public const string CONST_MAIL_ID_ORDER_UPDATE_FOR_OPERATOR = "00000172";	// 注文更新通知メール(管理者向け)
		public const string CONST_MAIL_ID_ORDER_CANCEL_BY_RECOMMEND_FOR_USER = "00000173"; // レコメンド時注文キャンセル通知メール
		public const string CONST_MAIL_ID_ORDER_CANCEL_BY_RECOMMEND_FOR_OPERATOR = "00000174"; // レコメンド時注文キャンセル通知メール(管理者向け)
		public const string CONST_MAIL_ID_ORDER_WORKFLOW_SCENARIO_EXEC = "00000180"; // 受注ワークフローシナリオ実行通知メール(管理者向け)

		public const string CONST_MAIL_ID_CS_NOTIFICATION = "00000201";			// CS系通知メール
		public const string CONST_MAIL_ID_CS_WARNING_OPERATOR = "00000202";		// CS系警告メール（オペレータ向け：本人）
		public const string CONST_MAIL_ID_CS_WARNING_GROUP = "00000203";		// CS系警告メール（オペレータ向け：グループ）
		public const string CONST_MAIL_ID_CS_WARNING_ADMIN = "00000204";		// CS系警告メール（管理者向け）

		public const string CONST_MAIL_ID_ORDERCOMBINE_ORDER_COMPLETE = "00000401";			// 注文同梱完了メールテンプレート（通常注文・定期注文）
		public const string CONST_MAIL_ID_ORDERCOMBINE_FIXEDPURCHASE_COMPLETE = "00000402";			// 注文同梱完了メールテンプレート（定期台帳）
		public const string CONST_MAIL_ID_ORDERCOMBINE_ORDER_COMPLETE_MANAGER = "00000403";			// 注文同梱配送(今回のみ)メールテンプレート
		public const string CONST_MAIL_ID_ORDERCOMBINE_FIXEDPURCHASE_COMPLETE_MANAGER = "00000404";			// 注文同梱配送（定期台帳）メールテンプレート
		public const string CONST_MAIL_ID_ORDERCOMBINE_ERROR_FOR_OPERATOR = "00000405";		// 注文同梱エラーメールテンプレート(管理者向け)
		public const string CONST_MAIL_ID_SEND_OPERATOR_FROM_BATCH_TWINVOICE = "90000057";	// Mail id send operator from batch TwInvoice
		public const string CONST_MAIL_ID_SEND_ALERT_OPERATOR_FROM_BATCH_TWINVOICE = "10000030";	// Mail id send alert operator from batch TwInvoice
		/// <summary>注文ステータス更新エラーメール(管理者向け)</summary>
		public const string CONST_MAIL_ID_SEND_UPDATE_ORDER_STATUS_FAILED = "00000190";

		public const string CONST_MAIL_ID_CONVENIENCE_STORE_FOR_USER = "00000501";		// Mail Template settting for convenience store change

		public const string CONST_MAIL_ID_INITIAL_UNRESERVED = "9";				// ユーザー任意のメールテンプレートID頭文字

		public const string CONST_MAIL_ID_SEND_PRODUCT_RECEIPT = "00000502";	// Mail Template Send Product Receipt
		public const string CONST_MAIL_ID_SEND_SHIPPING_RECEIVINGSTORE_IMPORTBATCH = "90000058";	// Mail Template Send Shipping Receiving Store Setting Import Batch

		/// <summary>Mail Template setting for external order information (for manager)</summary>
		public const string CONST_MAIL_ID_EXTERNAL_ORDER_INFO_FOR_OPERATOR = "10000092";

		/// <summary>Mail template setting for Gooddeal shipping (for manager)</summary>
		public const string CONST_MAIL_ID_GOODDEAL_SHIPPING_FOR_OPERATOR = "10000100";

		/// <summary>Mail id send operator from TwInvoice Ec Pay</summary>
		public const string CONST_MAIL_ID_SEND_OPERATOR_FROM_TWINVOICE_ECPAY = "10000101";

		/// <summary>WMS連携用メールテンプレートIDを</summary>
		public const string CONST_MAIL_ID_WMS_COOPERATION_FOR_OPERATOR = "10000083";

		/// <summary>Mail id reauth for operator</summary>
		public const string CONST_MAIL_ID_REAUTH_FOR_OPERATOR = "10000072";
		/// <summary>Mail id data migration</summary>
		public const string CONST_MAIL_ID_DATA_MIGRATION = "10000012";

		/// <summary>FLAPS連携メールテンプレート</summary>
		public const string CONST_MAIL_ID_FLAPS_INTEGRATION = "10000102";
		/// <summary>FLAPS連携エラーメールテンプレート</summary>
		public const string CONST_MAIL_ID_FLAPS_INTEGRATION_ERROR = "10000103";
		/// <summary>Send sms authentication</summary>
		public const string CONST_MAIL_ID_SEND_SMS_AUTHENTICATION = "10000110";
		/// <summary>領収書発行メールテンプレート</summary>
		public const string CONST_MAIL_ID_RECEIPT = "10000120";
		/// <summary>Cross Point cooperation mail ID for operator</summary>
		public const string CONST_MAIL_ID_CROSSPOINT_COOPERATION_FOR_OPERATOR = "10000104";
		/// <summary>CROSSPOINT連携ファイル取込エラー通知メールテンプレート</summary>
		public const string CONST_MAIL_ID_CROSSPOINT_COOPERATION_ERROR_MAIL = "10000105";
		/// <summary>Send 2-step authentication code</summary>
		public const string CONST_MAIL_ID_SEND_2_STEP_AUTHENTICATION_CODE = "10000111";

		/// <summary>Awoo商品連携バッチ完了メール【管理者向け】</summary>
		public const string CONST_MAIL_ID_AWOO_PRODUCT_SYNC_SUCCESS = "10000171";
		/// <summary>Awoo商品連携バッチ異常終了ーメール【管理者向け】</summary>
		public const string CONST_MAIL_ID_AWOO_PRODUCT_SYNC_FAILED = "10000172";

		/// <summary>ネクストエンジン：仮注文未存在メール</summary>
		public const string CONST_MAIL_ID_NO_EXIST_TMP_ORDER = "10000112";
		
		/// <summary>モール注文取込バッチ完了メール【管理者向け】</summary>
		public const string CONST_MAIL_ID_MALL_ORDER_IMPORTER_TERMINATION = "10000121";
		/// <summary>モール注文取込バッチ異常終了ーメール【管理者向け】</summary>
		public const string CONST_MAIL_ID_MALL_ORDER_IMPORTER_ABNORMAL_TERMINATION = "10000122";

		/// <summary>Const mail id to real shop</summary>
		public const string CONST_MAIL_ID_TO_REAL_SHOP = "10000200";
		/// <summary>Const mail id from real shop</summary>
		public const string CONST_MAIL_ID_FROM_REAL_SHOP = "00000600";


		/// <summary>マイページからの注文情報編集：管理者向けメール</summary>
		public const string CONST_MAIL_ID_MYPAGE_ORDER_MODIFY_FOR_OPERATOR = "00000504";
		/// <summary>マイページからの注文情報編集：ユーザー向けメール</summary>
		public const string CONST_MAIL_ID_MYPAGE_ORDER_MODIFY_FOR_USER = "00000505";
		/// <summary>マイページからの定期情報編集：管理者向けメール</summary>
		public const string CONST_MAIL_ID_MYPAGE_FIXED_PURCHASE_MODIFY_FOR_OPERATOR = "00000167";
		/// <summary>マイページからの定期情報編集：ユーザー向けメール</summary>
		public const string CONST_MAIL_ID_MYPAGE_FIXED_PURCHASE_MODIFY_FOR_USER = "00000136";

		/// <summary>ネクストエンジン：注文同梱完了メール【管理者向け】</summary>
		public const string CONST_MAIL_ID_NEXT_ENGINE_ORDER_COMBINE_COMPLETE_FOR_MANAGER = "00000406";
		/// <summary>ネクストエンジン：ネクストエンジン注文キャンセル失敗メール【管理者向け】</summary>
		public const string CONST_MAIL_ID_NEXT_ENGINE_ORDER_CANCEL_FAIL_FOR_MANAGER = "10000113";
		/// <summary>Cancel user integration async result mail id</summary>
		public const string CONST_MAIL_ID_CANCEL_USERINTEGRATION_ASYNC = "10000180";
		//------------------------------------------------------
		// CSメール振分け設定
		//------------------------------------------------------
		public const string CONST_MAIL_ASSIGN_ID_MATCH_ON_BIND = "000";			// 既存インシデント紐付くときにマッチする振分設定
		public const string CONST_MAIL_ASSIGN_ID_MATCH_ANYTHING = "001";		// すべての受信メールにマッチする振分設定

		//------------------------------------------------------
		// オペレータ系
		//------------------------------------------------------
		public const int CONST_SHOPOPERATOR_ID_LENGTH = 10;

		// コーディネート系
		public const int CONST_COORDINATE_ID_LENGTH = 10;

		// スタッフ系
		public const int CONST_STAFF_ID_LENGTH = 10;

		// 特集エリア系
		public const int CONST_FEATURE_AREA_ID_LENGTH = 10;

		//------------------------------------------------------
		// ポイント系
		//------------------------------------------------------
		// ポイント基本ルール
		public const string CONST_POINT_RULE_ID_BUY = "0000000001";				// 購入時ポイント発行
		public const string CONST_POINT_RULE_ID_USER_REGISTER = "0000000002";	// 新規登録ポイント発行
		public const string CONST_POINT_RULE_ID_LOGIN = "0000000003";			// ログイン毎ポイント発行
		public const string CONST_POINT_RULE_ID_FIRST_BUY = "0000000004";		// 初回購入ポイント発行

		// ポイント：税抜金額端数処理
		public const string FLG_POINT_TAX_EXCLUDED_FRACTION_ROUNDING_ROUND_UP = "ROUNDUP";		// 切り上げ
		public const string FLG_POINT_TAX_EXCLUDED_FRACTION_ROUNDING_ROUND_DOWN = "ROUNDDOWN";	// 切り捨て
		public const string FLG_POINT_TAX_EXCLUDED_FRACTION_ROUNDING_ROUND_OFF = "ROUNDOFF";	// 四捨五入

		public static bool ORDER_POINT_BATCH_CHANGE_TEMP_TO_COMP_ENABLED = false;	// 本ポイント移行設定
		public static int ORDER_POINT_BATCH_POINT_TEMP_TO_COMP_DAYS = 0;	// 出荷後何日で本ポイントへ移行
		public static int ORDER_POINT_BATCH_POINT_TEMP_TO_COMP_LIMITED_TERM_POINT_DAYS = 0;	// 出荷後何日で本ポイントへ移行

		//------------------------------------------------------
		// 汎用アフィリエイト
		//------------------------------------------------------
		// 汎用アフィリエイト：税抜金額端数処理
		public const string FLG_MULTIPURPOSE_AFFILIATE_TAX_EXCLUDED_FRACTION_ROUNDING_ROUND_UP = "ROUNDUP";		// 切り上げ
		public const string FLG_MULTIPURPOSE_AFFILIATE_TAX_EXCLUDED_FRACTION_ROUNDING_ROUND_DOWN = "ROUNDDOWN";	// 切り捨て
		public const string FLG_MULTIPURPOSE_AFFILIATE_TAX_EXCLUDED_FRACTION_ROUNDING_ROUND_OFF = "ROUNDOFF";	// 四捨五入

		//------------------------------------------------------
		// クーポン系
		//------------------------------------------------------
		public const int CONST_COUPON_ID_LENGTH = 10; // クーポンIDは10桁単位
		/// <summary>Value text param coupon list</summary>
		public const string VALUETEXT_PARAM_COUPON_LIST = "coupon_list";
		/// <summary>Value text param coupon list title</summary>
		public const string VALUETEXT_PARAM_COUPON_LIST_TITLE = "coupon_list_title";
		/// <summary>Value text param coupon list free shipping</summary>
		public const string VALUETEXT_PARAM_COUPON_LIST_FREE_SHIPPING = "free_shipping";
		/// <summary>Value text param coupon list free shipping</summary>
		public const string VALUETEXT_PARAM_COUPON_LIST_DISCOUNT_PRICE = "discount_price";

		//------------------------------------------------------
		// %割引
		//------------------------------------------------------
		/// <summary>%割引端数処理方法</summary>
		public static string PERCENTOFF_FRACTION_ROUNDING = "";
		/// <summary>%割引端数処理：切り上げ</summary>
		public const string FLG_PERCENTOFF_FRACTION_ROUNDING_ROUND_UP = "ROUNDUP";
		/// <summary>%割引端数処理：切り捨て</summary>
		public const string FLG_PERCENTOFF_FRACTION_ROUNDING_ROUND_DOWN = "ROUNDDOWN";
		/// <summary>%割引端数処理：四捨五入</summary>
		public const string FLG_PERCENTOFF_FRACTION_ROUNDING_ROUND_OFF = "ROUNDOFF";

		// 割引後商品価格按分計算に利用
		/// <summary>割引後金額オプション</summary>
		public static bool ORDER_ITEM_DISCOUNTED_PRICE_ENABLE = false;
		/// <summary>割引金額按分端数処理方法</summary>
		public static string DISCOUNTED_PRICE_FRACTION_ROUNDING = "";
		/// <summary>割引金額按分端数処理：切り上げ</summary>
		public const string FLG_DISCOUNTED_PRICE_FRACTION_ROUNDING_ROUND_UP = "ROUNDUP";
		/// <summary>割引金額按分端数処理：切り捨て</summary>
		public const string FLG_DISCOUNTED_PRICE_FRACTION_ROUNDING_ROUND_DOWN = "ROUNDDOWN";
		/// <summary>割引金額按分端数処理：四捨五入</summary>
		public const string FLG_DISCOUNTED_PRICE_FRACTION_ROUNDING_ROUND_OFF = "ROUNDOFF";

		// CSオペレータ権限系
		public const int CONST_CS_OPERATOR_AUTHORITY_ID_LENGTH = 10; // CSオペレータ権限IDは10桁単位

		// ブラックリスト型クーポン利用済みユーザ判定方法
		public static string COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE = string.Empty;
		public const string FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS = "MailAddress";	// メールアドレス
		public const string FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_USER_ID = "UserId";	// ユーザーID

		// ブラックリスト型クーポン利用ユーザ追加時ユーザーID既定値(判定方法がメールアドレスの際に利用)
		public const string COUPONUSEUSER_DEFAULT_BLACKLISTCOUPON_USER = "w2_DefaultBlacklistCouponUser";

		// 配送料無料発行パターン非表示
		public static bool HIDE_SHIPPINGFREECOUPON_IN_COUPONREGISTER = true;

		/// <summary>キー：返品交換ステータス</summary>
		public const string CONST_ORDER_RETURN_STATUS = "return_status";
		/// <summary>キー：返品商品情報</summary>
		public const string CONST_ORDER_RETURN_ORDERITEMS = "return_orderitems";
		/// <summary>キー：返品交換情報</summary>
		public const string CONST_ORDER_RETURN_EXCHANGE_DATA = "return_exchange_data";
		/// <summary>キー：交換商品情報</summary>
		public const string CONST_ORDER_EXCHANGE_ORDERITEMS = "exchange_orderitems";
		/// <summary>キー：画面操作時の商品通番</summary>
		public const string HASH_KEY_ORDER_ITEM_OPERATING_INDEX = "operating_index";

		/// <summary>返品交換ステータス：返品対象外</summary>
		public const string FLG_ORDER_RETURN_STATUS_RETURN_UNTARGET = "00";
		/// <summary>返品交換ステータス：返品対象</summary>
		public const string FLG_ORDER_RETURN_STATUS_RETURN_TARGET = "01";
		/// <summary>返品交換ステータス：返品済み</summary>
		public const string FLG_ORDER_RETURN_STATUS_RETURN_COMPLETE = "02";
		/// <summary>返品交換ステータス：交換元</summary>
		public const string FLG_ORDER_RETURN_STATUS_RETURN_EXCHANGED = "03";

		//------------------------------------------------------
		// SEOメタデータ：データ区分
		//------------------------------------------------------
		public const string FLG_SEOMETADATAS_DATA_KBN_DEFAULT_SETTING = "default_setting";				// 全体設定
		public const string FLG_SEOMETADATAS_DATA_KBN_PRODUCT_LIST = "product_list";					// 商品一覧画面
		public const string FLG_SEOMETADATAS_DATA_KBN_PRODUCT_DETAIL = "product_detail";				// 商品詳細画面
		public const string FLG_SEOMETADATAS_DATA_KBN_COORDINATE_TOP = "coordinate_top";				// コーディネートトップ
		public const string FLG_SEOMETADATAS_DATA_KBN_COORDINATE_LIST = "coordinate_list";				// コーディネート一覧
		public const string FLG_SEOMETADATAS_DATA_KBN_COORDINATE_DETAIL = "coordinate_detail";			// コーディネート詳細

		//------------------------------------------------------
		// PDF出力区分
		//------------------------------------------------------
		public const string KBN_PDF_OUTPUT_ORDER = "Order";						// 注文
		public const string KBN_PDF_OUTPUT_ORDER_WORKFLOW = "OrderWrokflow";	// 注文ワークフロー

		//------------------------------------------------------
		// 帳票区分
		//------------------------------------------------------
		public const string KBN_PDF_OUTPUT_ORDER_INVOICE = "OrderInvoice";		// 納品書
		public const string KBN_PDF_OUTPUT_TOTAL_PICKING_LIST = "TotalPickingList";	// トータルピッキングリスト
		public const string KBN_PDF_OUTPUT_ORDER_STATEMENT = "OrderStatement";	// 受注明細書
		public const string KBN_PDF_OUTPUT_RECEIPT = "Receipt";	// 領収書

		//------------------------------------------------------
		// カート投入区分
		//------------------------------------------------------
		public const string KBN_REQUEST_CART_ACTION_ADD = "1"; // カートページにて1商品投入を行う
		public const string KBN_REQUEST_CART_ACTION_ADD_SET = "2"; // カートページにて商品セット投入を行う
		/// <summary>頒布会初回選択画面</summary>
		public const string KBN_REQUEST_CART_ACTION_ADD_SUBSCRIPTIONBOX = "3";

		//------------------------------------------------------
		// 管理者向け注文完了メール
		//------------------------------------------------------
		/// <summary>管理者向け注文完了メール送信者区分</summary>
		public enum EnabledOrderCompleteEmailSenderType
		{
			/// <summary>フロント</summary>
			Front,
			/// <summary>マネージャー</summary>
			Manager,
			/// <summary>バッチ</summary>
			Batch
		}

		//------------------------------------------------------
		// 基幹システム連携用データ出力区分
		//------------------------------------------------------
		public enum ExportKbn
		{
			/// <summary>全て</summary>
			ALL,
			/// <summary>注文一覧画面</summary>
			OrderList
		}

		//------------------------------------------------------
		// 商品バリエーション選択方法区分
		//------------------------------------------------------
		public enum SelectVariationKbn
		{
			/// <summary>ドロップダウンリスト形式(在庫切れの場合「在庫なし」を表示)</summary>
			STANDARD,
			/// <summary>ドロップダウンリスト形式(商品在庫文言の在庫文言を表示)</summary>
			DROPDOWNLIST,
			/// <summary>ドロップダウンリスト(表示名1,表示名2)</summary>
			DOUBLEDROPDOWNLIST,
			/// <summary>マトリックス形式　(在庫文言併記なし)</summary>
			MATRIX,
			/// <summary>マトリックス形式　(在庫文言併記あり)</summary>
			MATRIXANDMESSAGE,
			/// <summary>パネル選択形式</summary>
			PANEL
		}

		//------------------------------------------------------
		/// <summary>カート追加区分</summary>
		//------------------------------------------------------
		public enum AddCartKbn
		{
			/// <summary>通常</summary>
			Normal,
			/// <summary>定期購入</summary>
			FixedPurchase,
			/// <summary>頒布会</summary>
			SubscriptionBox,
			/// <summary>ギフト購入</summary>
			GiftOrder
		}

		/// <summary>アクションタイプ</summary>
		public enum ActionTypes
		{
			/// <summary>注文クレカ登録</summary>
			RegisterOrderCreditCard,
			/// <summary>ユーザークレカ登録</summary>
			RegisterUserCreditCard,
			/// <summary>定期クレカ登録</summary>
			RegisterFixedPurchaseCreditCard,
			/// <summary>注文クレカ変更</summary>
			ChangeOrderCreditCard,
		}

		//------------------------------------------------------
		// ショートURL設定
		//------------------------------------------------------
		public static List<string> SHORTURL_DENY_EXTENSIONS = new List<string>();

		///<summary>完全再与信対応のクレジットカード一覧</summary>
		public static List<PaymentCard?> REAUTH_COMPLETE_CREDITCARD_LIST = new List<PaymentCard?>()
		{
			PaymentCard.Zeus,
			PaymentCard.Gmo,
			PaymentCard.SBPS,
			PaymentCard.YamatoKwc,
			PaymentCard.Zcom,
			PaymentCard.EScott,
			PaymentCard.VeriTrans,
			PaymentCard.Rakuten,
			PaymentCard.Paygent,
		};

		///<summary>完全再与信対応の後払い一覧</summary>
		public static List<PaymentCvsDef?> REAUTH_COMPLETE_CVSDEF_LIST = new List<PaymentCvsDef?>()
		{
			PaymentCvsDef.YamatoKa,
			PaymentCvsDef.Gmo,
			PaymentCvsDef.Atodene,
			PaymentCvsDef.Dsk,
			PaymentCvsDef.Atobaraicom,
			PaymentCvsDef.Score,
			PaymentCvsDef.Veritrans,
		};

		///<summary>キャンセル再与信対応の決済一覧（キャリア決済、PayPal決済、リクルート決済等含む）</summary>
		public static List<string> REAUTH_CANCEL_PAYMENT_LIST = new List<string>
		{
			FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS,
			FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS,
			FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS,
			FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS,
			FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS,
			FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS
		};

		//------------------------------------------------------
		// パスワードリマインダー認証項目
		//------------------------------------------------------
		public enum PasswordReminderAuthItem
		{
			/// <summary>生年月日</summary>
			Birth,
			/// <summary>電話番号</summary>
			Tel
		}

		#region レコナイズ削除予定記述
		//------------------------------------------------------
		// 外部レコメンドエンジン：レコナイズ
		//------------------------------------------------------
		// レコメンドリクエスト時の送信アイテムコード最大件数
		public const int CONST_RECONIZE_REQUEST_MAX_ITEM_CODE_COUNT = 10;
		#endregion

		//========================================================================
		// 販売時期の設定可能値
		//========================================================================
		public const string SETTING_DATE_THISMONTH = "ThisMonth";
		public const string SETTING_DATE_PREVIOUSMONTH = "PreviousMonth";
		public const string SETTING_DATE_THISWEEK = "ThisWeek";
		public const string SETTING_DATE_PREVIOUSWEEK = "PreviousWeek";

		//========================================================================
		// キー系
		//========================================================================

		//------------------------------------------------------
		// リクエストキー
		//------------------------------------------------------
		// 共通
		public const string REQUEST_KEY_SHOP_ID = "shop";				// 店舗ID
		public const string REQUEST_KEY_HIDE_BACK_BUTTON = "hide_back";	// 戻るボタン非表示
		public const string REQUEST_KEY_FRONT_NEXT_URL = "nurl";			// 画面遷移先（フロント用）
		/// <summary>戻り先URL</summary>
		public const string REQUEST_KEY_RETURN_URL = "rurl";

		public const string REQUEST_KEY_PAGE_INDEX_LIST_KEY = "key";	// 機能一覧画面の大項目

		// 共通：ユーザ系
		public const string REQUEST_KEY_FIXED_PURCHASE_MEMBER_FLG = "member";		// 定期会員フラグ

		// 共通：商品系
		public const string REQUEST_KEY_PRODUCT_ID = "pid";				// 商品ID
		public const string REQUEST_KEY_CATEGORY_ID = "cat";			// カテゴリID
		public const string REQUEST_KEY_VARIATION_ID = "vid";			// 商品バリエーションID
		public const string REQUEST_KEY_SEARCH_WORD = "swrd";			// 検索ワード
		/// <summary>注文区分</summary>
		public const string REQUEST_KEY_ORDER_ORDER_KBN = "okn";
		/// <summary>モールID</summary>
		public const string REQUEST_KEY_ORDER_MALL_ID = "omlid";
		/// <summary>注文ステータス</summary>
		public const string REQUEST_KEY_ORDER_ORDER_STATUS = "os";
		/// <summary>ステータス更新日</summary>
		public const string REQUEST_KEY_ORDER_UPDATE_DATE_STATUS = "odus";
		/// <summary>支払区分</summary>
		public const string REQUEST_KEY_ORDER_ORDER_PAYMENT_KBN = "opkn";
		/// <summary>期間指定開始日</summary>
		public const string REQUEST_KEY_ORDER_UPDATE_DATE_FROM = "oudf";
		/// <summary>期間指定終了日</summary>
		public const string REQUEST_KEY_ORDER_UPDATE_DATE_TO = "oudt";
		/// <summary>期間指定開始時間</summary>
		public const string REQUEST_KEY_ORDER_UPDATE_TIME_FROM = "outf";
		/// <summary>期間指定終了時間</summary>
		public const string REQUEST_KEY_ORDER_UPDATE_TIME_TO = "outt";
		/// <summary>期間指定終了時間</summary>
		public const string REQUEST_KEY_ORDER_STOREPICKUP_STATUS = "ospust";
		/// <summary>頒布会検索ワード</summary>
		public const string REQUEST_KEY_SUBSCRIPTION_BOX_SEARCH_WORD = "sbswrd";
		public const string REQUEST_KEY_CAMPAINGN_ICOM = "cicon";		// キャンペーンアイコン
		public const string REQUEST_KEY_BRAND_ID = "bid";				// ブランドID
		public const string REQUEST_KEY_SORT_KBN = "sort";				// ソート区分
		public const string REQUEST_KEY_DISP_IMG_KBN = "img";			// 画像表示区分
		public const string REQUEST_KEY_MIN_PRICE = "min";				// 最小価格
		public const string REQUEST_KEY_MAX_PRICE = "max";				// 最大価格
		public const string REQUEST_KEY_DISP_PRODUCT_COUNT = "dpcnt";	// 商品一覧表示商品数
		public const string REQUEST_KEY_UNDISPLAY_NOSTOCK_PRODUCT = "udns";  // 在庫無し非表示
		public const string REQUEST_KEY_DISP_ONLY_SP_PRICE = "dosp";		// 特別価格有りのみ表示
		public const string REQUEST_KEY_PRODUCT_GROUP_ID = "pgi";	// 商品グループID
		public const string REQUEST_KEY_FIXED_PURCHASE_FILTER = "fpfl";  // 定期購入フィルタ
		public const string REQUEST_KEY_PRODUCT_COLOR_ID = "col";		// 商品カラーID
		public const string REQUEST_KEY_PRODUCT_TAX_CATEGORY_ID = "ctax";		// 商品税率カテゴリID
		public const string REQUEST_KEY_BRAND_SEARCH_ALL = "bsa"; // Request key for search all brand
		public const string REQUEST_KEY_ORDER_ID_FOR_RECEIPT = "okey";    // 注文ID(領収書出力URL作成用)
		public const string REQUEST_KEY_PRODUCT_SALE_FILTER = "sfl";	// セール対象フィルタ
		/// <summary>Request key Cart Index</summary>
		public const string REQUEST_KEY_CART_INDEX = "cindex";
		/// <summary>Request key shipping receiving store type</summary>
		public const string REQUEST_KEY_SHIPPING_RECEIVING_STORE_TYPE = "srstype";
		/// <summary>ユーザID</summary>
		public const string REQUEST_KEY_USER_ID = "user_id";
		/// <summary>ユニークキー（EC管理ユーザー情報登録・更新用）</summary>
		/// <remarks>
		/// ユーザー登録：タイムスタンプ
		/// ユーザー更新：ユーザーID
		/// </remarks>
		public const string REQUEST_KEY_UNIQUE_KEY = "ukey";

		/// <summary>タグ（Awoo連携用）</summary>
		public const string REQUEST_KEY_TAGS = "tags";

		/// <summary>定期購入ID</summary>
		public const string REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_ID = "fpid";
		public const string REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_STATUS = "fpstatus";

		// 共通：商品系(モバイル)
		public const string REQUEST_KEY_MFRONT_PAGE_ID = "pgid";		// モバイル：ページID
		public const string REQUEST_KEY_MFRONT_SHOP_ID = "spid";		// モバイル：店舗ID
		public const string REQUEST_KEY_MFRONT_CATEGORY_ID = "prcid";	// モバイル：カテゴリID
		public const string REQUEST_KEY_MFRONT_PRODUCT_ID = "prid";		// モバイル：商品ID
		public const string REQUEST_KEY_MFRONT_VARIATION_ID = "prvid";	// モバイル：商品バリエーションID
		public const string REQUEST_KEY_MFRONT_PRODUCTSET_ID = Constants.REQUEST_KEY_PRODUCTSET_ID; // モバイル：商品セットID
		public const string REQUEST_KEY_MFRONT_SORT_KBN = Constants.REQUEST_KEY_SORT_KBN; // モバイル：ソート区分
		public const string REQUEST_KEY_MFRONT_DISP_IMAGE = "dimg";		// モバイル：一覧画像表示区分
		public const string REQUEST_KEY_MFRONT_SEARCH_TEXT = "text";	// モバイル：検索文字列

		public const string REQUEST_KEY_CART_ACTION = "ckbn"; // カート投入区分キー
		/// <summary>頒布会コースID</summary>
		public const string REQUEST_KEY_CART_SUBSCRIPTION_BOX_COURSE_ID = "sbcid";
		/// <summary>頒布会コース一覧からの遷移か</summary>
		public const string REQUEST_KEY_CART_SUBSCRIPTION_BOX_FOR_COURSE_LIST = "sbfcl";
		public const string REQUEST_KEY_PRODUCT_COUNT = "prdcnt"; // カート商品追加数
		public const string REQUEST_KEY_FIXED_PURCHASE = "fxpchs"; // 定期購入
		// 頒布会
		public const string REQUEST_KEY_SUBSCRIPTION_BOX = "sbc";
		public const string REQUEST_KEY_GIFT_ORDER = "gift"; // ギフト注文
		public const string REQUEST_KEY_PRODUCTSET_ID = "psid"; // 商品セットID
		public const string REQUEST_KEY_PRODUCT_SET_NO = "psno"; // 商品セットNo
		public const string REQUEST_KEY_PRODUCT_SALE_ID = "pslid";		// モバイル：セールID
		public const string REQUEST_KEY_PRODUCT_SALE_KBN = "pslkbn";		// 商品セール区分
		public const string REQUEST_KEY_PRODUCT_ADD = "prdadd"; // モバイル：カート投入区分キー ※モバイルはカート処理以外にも利用
		public const string REQUEST_KEY_MFRONT_PRODUCT_COUNT = "pcnt"; // モバイル：カート商品追加数
		public const string REQUEST_KEY_MFRONT_FIXED_PURCHASE = "fxp"; // モバイル：定期購入
		public const string REQUEST_KEY_CART_ID = "ci";	// Request Cart Id
		public const string REQUEST_KEY_NO = "no";	// Request No
		/// <summary>定期購入区分</summary>
		public const string REQUEST_KEY_FIXED_PURCHASE_KBN = "fpk";
		/// <summary>定期購入設定</summary>
		public const string REQUEST_KEY_FIXED_PURCHASE_SETTING = "fps";

		public const string REQUEST_KEY_PRODUCT_DELETE_CONFIRM = "prddel_cnf";
		public const string REQUEST_KEY_PRODUCT_DELETE = "prddel";

		// 受注ワークフロー
		/// <summary>並び順</summary>
		public const string REQUEST_KEY_RETURN_EXCHANGE_SORT_KBN = "return_exchange_sort";
		/// <summary>ワークフロー種別</summary>
		public const string REQUEST_KEY_WORKFLOW_TYPE = "workflow_type";
		/// <summary>キャンセル可能時間帯の注文を抽出しないフラグ</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_TARGET_CANCELABLE_TIME_ORDERS_FLG = "target_cancelable_time_orders_flg";
		/// <summary>キャンセル可能時間帯の注文を抽出しないフラグON</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_TARGET_CANCELABLE_TIME_ORDERS_FLG_ON = "1";
		/// <summary>キャンセル可能時間帯の注文を抽出しないフラグOFF</summary>
		public const string REQUEST_KEY_ORDERWORKFLOWSETTING_TARGET_CANCELABLE_TIME_ORDERS_FLG_OFF = "0";
		/// <summary>ダッシュボード未出荷注文表示用ワークフロー番号</summary>
		public const string CONST_SUMMARY_REPORT_UNSHIPPED_WORKFLOW_NO = "1";

		// 共通:グローバル対応
		public const string REQUEST_KEY_GLOBAL_IP = "ip";//IPアドレス
		public const string REQUEST_KEY_GLOBAL_LANGUAGE_CODE = "lc";//言語コード
		public const string REQUEST_KEY_GLOBAL_LANGUAGE_LOCALE_ID = "lli"; // 言語ロケールID
		public const string REQUEST_KEY_GLOBAL_CURRENCY_CODE = "cc";//通貨コード

		// 共通：認証キー
		public const string REQUEST_KEY_AUTHENTICATION_KEY = "akey";

		// 共通系：ポイントルール情報
		public const string REQUEST_KEY_POINT_RULE_ID = "ptrid"; // ポイントルールID

		// 共通：商品付帯情報
		public const string REQUEST_KEY_PRODUCT_OPTION_VALUE = "pov";	// 商品付帯情報
		public const string REQUEST_KEY_PRODUCT_OPTION_VALUE1 = "pov1";	// 商品付帯情報１
		public const string REQUEST_KEY_PRODUCT_OPTION_VALUE2 = "pov2";	// 商品付帯情報２
		public const string REQUEST_KEY_PRODUCT_OPTION_VALUE3 = "pov3";	// 商品付帯情報３
		public const string REQUEST_KEY_PRODUCT_OPTION_VALUE4 = "pov4";	// 商品付帯情報４
		public const string REQUEST_KEY_PRODUCT_OPTION_VALUE5 = "pov5";	// 商品付帯情報５

		// 管理共通：ログイン
		public const string REQUEST_KEY_MANAGER_LOGIN_ID = "login_id";
		public const string REQUEST_KEY_MANAGER_PASSWORD = "password";
		public const string REQUEST_KEY_MANAGER_LOGIN_FLG = "login_flg";
		public const string REQUEST_KEY_MANAGER_NEXTURL = "nurl";

		// 管理共通：ログアウト
		public const string REQUEST_KEY_COOKIE_NAME_EC = "ASP.NET_SessionId.w2cManager";
		public const string REQUEST_KEY_COOKIE_NAME_MP = "ASP.NET_SessionId.w2mpManager";
		public const string REQUEST_KEY_COOKIE_NAME_CS = "ASP.NET_SessionId.w2csManager";
		public const string REQUEST_KEY_COOKIE_NAME_CMS = "ASP.NET_SessionId.w2cmsManager";
		public const string REQUEST_REMOVE_COOKIE_PATH = "/";

		/// <summary>Request key manager: login expired flg</summary>
		public const string REQUEST_KEY_MANAGER_LOGIN_EXPIRED_FLG = "LoginExpiredFlg";
		/// <summary>Request key manager: login expired flg : valid</summary>
		public const string REQUEST_KEY_MANAGER_LOGIN_EXPIRED_FLG_VALID = "1";

		// 管理共通：シングルサインオン系
		public const string REQUEST_KEY_SINGLE_SIGN_ON_MANAGER_SITE_TYPE = "sso_mng";	// シングルサインオン管理画面タイプ
		public const string REQUEST_KEY_SINGLE_SIGN_ON_CMS_SITE_TYPE = "ManagerSiteType";	// シングルサインオン管理画面タイプ

		/// <summary>管理共通: パーツプレビュー パーツID</summary>
		public const string REQUEST_KEY_PARTS_PREVIEW_PARTS_ID = "parts_preview_parts_id";

		/// <summary>管理共通：ポップアップ呼び出し元画面</summary>
		public const string REQUEST_KEY_MANAGER_POPUP_PARENT_NAME = "popup_parent_name";

		/// <summary>ポップアップ閉じる時の確認メッセージID</summary>
		public const string REQUEST_KEY_MANAGER_POPUP_CLOSE_CONFIRM = "closeconfirm";

		// PC：SingleSignOn系
		/// <summary>SingleSignOn：ログインIDパラメタキー</summary>
		public const string REQUEST_KEY_SINGLE_SIGN_ON_LOGINID = "ssoid";
		/// <summary>SingleSignOn：パスワードパラメタキー</summary>
		public const string REQUEST_KEY_SINGLE_SIGN_ON_PASSWORD = "ssops";
		/// <summary>SingleSignOn：JAFサービスID</summary>
		public const string REQUEST_KEY_SINGLE_SIGN_ON_SERVICEID = "qServiceId";
		/// <summary>SingleSignOn：JAF遷移先URL</summary>
		public const string REQUEST_KEY_SINGLE_SIGN_ON_URL = "qUrl";

		/// <summary>マスタアップロード</summary>
		public const string REQUEST_KEY_SELECTED_MASTER = "selected_master";
		/// <summary>マスタ出力：マスタ区分</summary>
		public const string REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN = "mk";

		// CMS
		public const string REQUEST_KEY_OPERATOR_ID = "opid";						// オペレータID

		// コーディネート系
		public const string REQUEST_KEY_COORDINATE_ID = "coid";				// コーディネートID
		public const string REQUEST_KEY_COORDINATE_CATEGORY_ID = "ccid";	// コーディネートカテゴリID

		// 特集ページ系
		/// <summary>特集ページ：カテゴリID</summary>
		public const string REQUEST_KEY_FEATURE_PAGE_CATEGORY_ID = "fpcid";

		// 名称翻訳設定
		/// <summary>対象データ区分</summary>
		public const string REQUEST_KEY_NAME_TRANSLATION_SETTING_DATA_KBN = "ntsdk";

		// その他
		/// <summary>アクションステータス</summary>
		public const string REQUEST_KEY_ACTION_STATUS = "action_status";
		/// <summary>リターンフラグ</summary>
		public const string REQUEST_KEY_RETURN_FLAG = "return_flag";
		/// <summary>ウィンドウ区分</summary>
		public const string REQUEST_KEY_WINDOW_KBN = "window_kbn";
		/// <summary>アクションタイプ</summary>
		public const string REQUEST_KEY_PAYMENTCREDITCARD_ACTION_TYPE = "actype";
		/// <summary>リンクアフィリエイト：タグID</summary>
		public const string REQUEST_KEY_LINK_AFFILIATE_TAG_ID = "tagid";
		/// <summary>リンクアフィリエイト：リストID</summary>
		public const string REQUEST_KEY_LINK_AFFILIATE_LST_ID = "lstid";
		/// <summary>リンクアフィリエイト：アクセス日</summary>
		public const string REQUEST_KEY_LINK_AFFILIATE_ARRIVE_DATETIME = "arrive_datetime";
		// 電子発票管理ページ
		public const string REQUEST_KEY_INVOICE_NO = "ino";				// 電子発票管理枝番
		/// <summary>GMO後払い_デバイス情報</summary>
		public const string REQUEST_GMO_DEFERRED_DEVICE_INFO = "fraudbuster";
		/// <summary>リクエストキー：電話番号</summary>
		public const string REQUEST_KEY_TEL_NO = "telno";

		/// <summary>オペレータ名</summary>
		public const string REQUEST_KEY_OPERATOR_NAME = "operator_name";
		/// <summary>メニュー権限</summary>
		public const string REQUEST_KEY_OPERATOR_MENUACCESS_LEVEL = "mal";
		/// <summary>オペレータ有効フラグ</summary>
		public const string REQUEST_KEY_OPERATOR_VALID_FLG = "ovf";

		/// <summary>Request key area id</summary>
		public const string REQUEST_KEY_AREA_ID = "areaid";

		/// <summary>Request key: Credit mode</summary>
		public const string REQUEST_KEY_CREDIT_MODE = "crdmode";
		/// <summary>Request key: Credit date</summary>
		public const string REQUEST_KEY_CREDIT_DATE = "crddate";
		/// <summary>Request key: Credit extended status</summary>
		public const string REQUEST_KEY_CREDIT_EXTENDED_STATUS = "crdextstatus";
		/// <summary>Request key: Data migration shop id</summary>
		public const string REQUEST_KEY_DATA_MIGRATION_SHOP_ID = "dmsid";
		/// <summary>Request key: Data migration master</summary>
		public const string REQUEST_KEY_DATA_MIGRATION_MASTER = "dmmaster";
		/// <summary>Request key: Data migration output log file name</summary>
		public const string REQUEST_KEY_DATA_MIGRATION_OUTPUT_LOG_FILE_NAME = "dmolfname";

		//------------------------------------------------------
		// キーに格納する値
		//------------------------------------------------------
		/// <summary>アクションステータス（詳細）</summary>
		public const string ACTION_STATUS_DETAIL = "detail";
		/// <summary>ウィンドウ区分（ポップアップウィンドウ）</summary>
		public const string KBN_WINDOW_POPUP = "1";
		/// <summary>ウィンドウ区分（通常ウィンドウ）</summary>
		public const string KBN_WINDOW_DEFAULT = ""; // 通常ウィンドウ
		/// <summary>商品詳細ページから頒布会詳細ページへの遷移</summary>
		public const string REDIRECT_TO_SUBSCRIPTION_BOX_DETAIL_FROM_PRODUCT_DETAIL = "0";
		/// <summary>頒布会コースリストから頒布会詳細ページへの遷移</summary>
		public const string REDIRECT_TO_SUBSCRIPTION_BOX_DETAIL_FROM_COURSE_LIST  = "1";

		//------------------------------------------------------
		// 採番マスタキー
		//------------------------------------------------------
		public const string NUMBER_KEY_USER_ID = "UserId";
		public const string NUMBER_KEY_CART_ID = "CartId";
		public const string NUMBER_KEY_ORDER_ID = "OrderId";
		public const string NUMBER_KEY_PAYMENT_ORDER_ID = "PaymentOrderId";
		public const string NUMBER_KEY_FIXED_PURCHASE_ID = "FixedPurchaseId";
		public const string NUMBER_KEY_PRODUCTSALE_ID = "ProductSaleId";
		public const string NUMBER_KEY_COUPON_ID = "CouponId";
		public const string NUMBER_KEY_MENU_AUTHORITY_LEVEL = "MenuAuthorityLevel";
		public const string NUMBER_KEY_CS_OPERATOR_AUTHORITY_ID = "CSOperatorAuthorityId";
		public const string NUMBER_KEY_MAILTEMPLATE_ID = "MailTemplateId";
		public const string NUMBER_KEY_NEWS_ID = "NewsId";
		public const string NUMBER_KEY_PRODUCT_SET_ID = "ProductSetId";
		public const string NUMBER_KEY_SHOP_OPERATOR_ID = "ShopOperatorId";
		public const string NUMBER_KEY_STOCK_ORDER_ID = "StockOrderId";
		public const string NUMBER_KEY_TARGET_LIST_ID = "TargetListId";
		public const string NUMBER_KEY_RECOMMEND_PRODUCT_ID = "RecommendProductId";
		public const string NUMBER_KEY_YAMATOKWC_MEMBER_ID = "YamatoKwcMemberId";
		public const string NUMBER_KEY_CS_INCIDENT_ID = "CSIncidentId";
		public const string NUMBER_KEY_CS_MESSAGE_MAIL_ID = "CSMessageMailId";
		public const string NUMBER_KEY_CS_ANSWER_ID = "CsAnswerId";
		public const string NUMBER_KEY_CS_ANSWER_CATEGORY_ID = "CsAnswerCategoryId";
		public const string NUMBER_KEY_CS_GROUP_ID = "CsGroupId";
		public const string NUMBER_KEY_CS_INCIDENT_CATEGORY_ID = "CsIncidentCategoryId";
		public const string NUMBER_KEY_CS_INCIDENT_VOC_ID = "CsIncidentVocId";
		public const string NUMBER_KEY_CS_MAIL_ASSIGN_ID = "CsMailAssignId";
		public const string NUMBER_KEY_CS_MAIL_FROM_ID = "CsMailFromId";
		public const string NUMBER_KEY_CS_MAIL_SIGNATURE_ID = "CsMailSignatureId";
		public const string NUMBER_KEY_CS_SHARE_INFO_NO = "CsShareInfoNo";
		public const string NUMBER_KEY_CS_SUMMARY_SETTING_NO = "CsSummarySettingNo";
		public const string NUMBER_KEY_MP_MAILDISTSETTING_ID = "MailDistSettingId";
		public const string NUMBER_KEY_MP_MAILDISTTEXT_ID = "MailDistTextId";
		public const string NUMBER_KEY_MP_MEMBERRANKRULE_ID = "MemberRankRuleId";
		public const string NUMBER_KEY_MP_POINTRULE_ID = "PointRuleId";
		public const string NUMBER_KEY_MP_POINTRULESCHEDULE_ID = "PointRuleScheduleId";
		public const string NUMBER_KEY_MP_COUPONSCHEDULE_ID = "CouponScheduleId";
		public const string NUMBER_KEY_CMS_STAFF_ID = "StaffId";
		public const string NUMBER_KEY_CMS_COORDINATE_ID = "CoordinateId";
		public const string NUMBER_KEY_CMS_FEATURE_AREA_ID = "FeatureAreaId";
		public const string NUMBER_KEY_CMS_SCORING_SALE_ID = "ScoringSaleId";

		//------------------------------------------------------
		// セッション変数キー
		//------------------------------------------------------
		public const string SESSION_KEY_PARAM = "param_data";
		public const string SESSION_KEY_PARAM2 = "param_data2";
		public const string SESSION_KEY_PARAM_ORDER = "order";
		public const string SESSION_KEY_PARAM_FOR_USER_INPUT = "param_user_input";
		public const string SESSION_KEY_PARAM_FOR_USER_BUSINESS_OWNER_INPUT = "param_user_business_owner_input";
		public const string SESSION_KEY_PARAM_FOR_BACK = "param_data_for_back";
		public const string SESSION_KEY_PARAM_FOR_ORDER_REGIST_INPUT = "param_order_regist_input";
		public const string SESSION_KEY_PARAM_FOR_ORDER_WORKFLOW_LIST = "param_order_workflow_list";
		public const string SESSION_KEY_PARAM_FOR_ORDER_WORKFLOW_SCENARIO_SETTING = "param_order_workflow_scenario_setting";
		public const string SESSION_KEY_PARAM_FOR_USER_CREDITCARD_INPUT = "param_user_creditcard_input";
		public const string SESSION_KEY_PARAM_FOR_USER_SHIPPING_INPUT = "param_user_shipping_input";
		public const string SESSION_KEY_PARAM_FOR_USER_INVOICE_INPUT = "param_user_invoice_input";
		/// <summary>定期商品変更設定</summary>
		public const string SESSION_KEY_PARAM_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING = "fixed_purchase_product_change_setting";

		#region エラー系のセッションキー
		public const string SESSION_KEY_ERROR = "error";
		public const string SESSION_KEY_ERROR_MSG = "error_message";
		/// <summary>確認画面でカート情報が変更されたときに格納するセッションのキー</summary>
		public const string SESSION_KEY_ERROR_MSG_FOR_CHANGE_CART = "error_message_for_change_cart";
		public const string SESSION_KEY_ERROR_MSG_FOR_LOGINPAGE = "error_message_for_loginpage";
		public const string SESSION_KEY_ERROR_FOR_ADD_CART = "error_for_add_cart";
		public const string SESSION_KEY_ERROR_FOR_PAYPAY_PAYMENT = "error_for_paypay_payment";
		#endregion

		/// <summary>マスタ出力定義情報</summary>
		public const string SESSIONPARAM_KEY_MASTEREXPORTSETTING_INFO = "masterexportsetting_info";
		/// <summary>SMS配信テキスト</summary>
		public const string SESSIONPARAM_KEY_SMSDISTTEXT_INFO = "smsdisttext_info";
		/// <summary>LINE配信テキスト</summary>
		public const string SESSIONPARAM_KEY_LINEDISTTEXT_INFO = "linedisttext_info";
		/// <summary>Param user default setting input</summary>
		public const string SESSION_KEY_PARAM_FOR_USER_DEFAULT_SETTING = "param_user_default_setting_input";
		/// <summary>Session key param for back 2</summary>
		public const string SESSION_KEY_PARAM_FOR_BACK2 = "param_data_for_back2";

		// 商品コンバータ
		public const string SESSION_KEY_ADDETAIL_CONVERT = "ad_detail_convert";
		public const string SESSION_KEY_ADDETAIL_COLUMNS = "ad_detail_columns";

		/// <summary>注文完了後会員登録用ユーザーＩＤ</summary>
		public const string SESSION_KEY_USER_REGIST_AFTER_ORDER_USER_ID = "user_regist_after_order_user_id";
		/// <summary>注文完了後会員登録用カートリスト</summary>
		public const string SESSION_KEY_USER_REGIST_AFTER_ORDER_CART_LIST = "user_regist_after_order_cart_list";
		/// <summary>リクエストパフォーマンスチェック用OnPreRequestHandlerExecute実行時間</summary>
		public const string SESSION_KEY_ON_PRE_REQUEST_HANDLER_EXECUTE_DATETIME = "OnPreRequestHandlerExecute_DateTime";

		/// <summary>ソーシャルプラス用</summary>
		public const string SESSION_KEY_SOCIAL_LOGIN_MODEL = "social_login_model";
		/// <summary>一時保管ソーシャルログイン情報</summary>
		public const string SESSION_KEY_TEMPORARY_STORE_SOCIAL_LOGIN_MODEL = "temporary_store_social_login_model";

		/// <summary>プレビュー用HTMLリスト</summary>
		public const string SESSION_KEY_HTML_FOR_PREVIEW_LIST = "html_for_preview_list";

		/// <summary>管理画面でZEUSタブレット利用</summary>
		public const string SESSION_KEY_LOGIN_OPERTOR_USE_PAYMENT_TABLET_ZEUS = "w2cMng_loggedin_operator_use_payment_tablet_zeus";

		/// <summary>グローバル対応 リージョン保管用</summary>
		public const string SESSION_KEY_GLOBAL_REGION = "global_Region";

		/// <summary>名称翻訳設定マスタ エクスポート対象データ区分</summary>
		public const string SESSION_KEY_NAMETRANSLATIONSETTING_EXPORT_TARGET_DATAKBN = "name_translation_setting_export_target_data_kbn";

		/// <summary>グローバル：言語コード</summary>
		public const string SESSION_KEY_LANGUAGE_CODE = "language_code";
		/// <summary>グローバル：言語ロケールID</summary>
		public const string SESSION_KEY_LANGUAGE_LOCALE_ID = "language_locale_id";

		/// <summary>外部決済ID</summary>
		public const string SESSION_KEY_PAYMENT_ORDER_ID = "w2Front_payment_order_id";
		/// <summary>SMS認証携帯電話番号</summary>
		public const string SESSION_KEY_SMS_TEL_NUMBER = "w2Front_sms_tel_number";

		/// <summary>ログインしたときに会員情報から取得</summary>
		public const string SESSION_KEY_ADVCODE_FIRST = "w2cFront_advcode_first";
		/// <summary>リアルに渡ってきた広告コード / 会員登録時に登録</summary>
		public const string SESSION_KEY_ADVCODE_NOW = "w2cFront_advcode_now";
		/// <summary>有効な広告パラメータ名 / 成果報告時に使用</summary>
		public const string SESSION_KEY_ADV_PARAMETER_NAME = "w2cFront_adv_parameter_name";

		/// <summary>配送先住所反映パターン</summary>
		public const string SESSION_KEY_ADDRESSUPDATE_PATTERN = "addressupdate_pattern";
		/// <summary>更新前配送先住所</summary>
		public const string SESSION_KEY_CURRENT_FIXED_PURCHASE_ORIGIN = "current_fixed_purchase_origin";
		/// <summary>更新する対象リスト</summary>
		public const string SESSION_KEY_ADDRESSUPDATE_DO_UPDATE_TARGETS = "addressupdate_update_targets";
		/// <summary>更新した対象リスト</summary>
		public const string SESSION_KEY_ADDRESSUPDATE_UPDATE_TARGETS = "addressupdate_update_targets";
		/// <summary>外部決済連携ログ</summary>
		public const string SESSION_KEY_EXTERNAL_PAYMENT_COOPERATION_LOG = "external_payment_cooperation_log";

		/// <summary>サイト基本情報ステータス</summary>
		public const string SESSION_KEY_SITE_INFORMATION_STATUS = "site_information_status";

		/// <summary>ランディングカート入力画面絶対パス</summary>
		public const string SESSION_KEY_LANDING_CART_INPUT_ABSOLUTE_PATH = "landing_cart_input_absolutePath";

		/// <summary>ショップオペレーター</summary>
		public const string SESSION_KEY_LOGIN_SHOP_OPERTOR = "w2cMng_loggedin_shop_operator";

		/// <summary>Login user id</summary>
		public const string SESSION_KEY_FRONT_LOGIN_USER_ID = "w2cFront_login_user_id";
		/// <summary>Is cart list landing page</summary>
		public const string SESSION_KEY_FRONT_IS_CARTLIST_LP = "w2Front_is_cartlist_lp";
		/// <summary>メンバーランクID</summary>
		public const string SESSION_KEY_LOGIN_USER_MEMBER_RANK_ID = "w2cFront_login_member_rank_id";

		/// <summary>最低購入種類数</summary>
		public const string SESSION_KEY_MIN_NUMBER_OF_PRODUCTS = "min_number_of_products";
		/// <summary>最大購入種類数</summary>
		public const string SESSION_KEY_MAX_NUMBER_OF_PRODUCTS = "max_number_of_products";
		/// <summary>最低購入数量</summary>
		public const string SESSION_KEY_MIN_QUANTITY = "min_quantity";
		/// <summary>最大購入数量</summary>
		public const string SESSION_KEY_MAX_QUANTITY = "max_quantity";
		/// <summary>頒布会管理名</summary>
		public const string SESSION_KEY_SUBSCRIPTION_MANAGEMENT_NAME = "subscription_management_name";

		/// <summary>Session key payment credit Zcom order 3DSecure</summary>
		public const string SESSION_KEY_PAYMENT_CREDIT_ZCOM_ORDER_3DSECURE = "zcom_order_3dsecure";
		/// <summary>ヤマトKWC3Dセキュア用セッションキー</summary>
		public const string SESSION_KEY_PAYMENT_CREDIT_YAMATOKWC_ORDER_3DSECURE = "yamatokwc_order_3dsecure";

		/// <summary> メーテンプレートのLINE送信内容 </summary>
		public const string SESSION_KEY_MAILTEMPLATE_LINE_CONTENTS = "mail_template_line_contents";

		/// <summary>Workflow exec process</summary>
		public const string SESSION_KEY_WORKFLOW_EXEC_PROCESS = "workflow_exec_process";

		/// <summary>Session key payment boku optin id</summary>
		public const string SESSION_KEY_PAYMENT_BOKU_OPTIN_ID = "boku_optin_id";

		///<summary>特集画像：特集画像グループリスト</summary>
		public const string SESSION_KEY_FEATURE_IMAGE_GROUP_LIST = "feature_image_group_list";
		///<summary>特集画像：特集画像リスト</summary>
		public const string SESSION_KEY_FEATURE_IMAGE_LIST = "feature_image_list";

		///<summary>特集ページカテゴリ：親カテゴリID</summary>
		public const string SESSION_KEY_FEATURE_PAGE_CATEGORY_PARENT_CATEGORY_ID = "parent_category_id";
		///<summary>特集ページカテゴリ：カテゴリID</summary>
		public const string SESSION_KEY_FEATURE_PAGE_CATEGORY_CATEGORY_ID = "_category_id";
		///<summary>特集ページカテゴリ：カテゴリID</summary>
		public const string SESSION_KEY_FEATURE_PAGE_CATEGORY_CATEGORY_NAME = "_category_name";
		///<summary>特集ページカテゴリ：表示順</summary>
		public const string SESSION_KEY_FEATURE_PAGE_CATEGORY_DISPLAY_ORDER = "display_order";
		///<summary>特集ページカテゴリ：カテゴリ概要</summary>
		public const string SESSION_KEY_FEATURE_PAGE_CATEGORY_CATEGORY_OUTLINE = "category_outline";
		///<summary>特集ページカテゴリ：有効フラグ</summary>
		public const string SESSION_KEY_FEATURE_PAGE_CATEGORY_VALID_FLG = "valid_flg";
		///<summary>特集ページカテゴリ：詳細からの遷移か</summary>
		public const string SESSION_KEY_FEATURE_PAGE_CATEGORY_PARENT_IS_CATEGORY_DETAIL = "is_category_detail";
		///<summary>特集ページカテゴリ：変更前カテゴリID</summary>
		public const string SESSION_KEY_FEATURE_PAGE_CATEGORY_BEFORE_CATEGORY_ID = "before_category_id";
		///<summary>特集ページカテゴリ：カテゴリ重複フラグ</summary>
		public const string SESSION_KEY_FEATURE_PAGE_CATEGORY_DUPLICATE_CATEGORY_FLG = "duplicate_category_flg";
		///<summary>特集ページカテゴリ：カテゴリエラーメッセージ</summary>
		public const string SESSION_KEY_FEATURE_PAGE_CATEGORY_CATEGORY_ERRORMESSSAGE = "category_errormessage";
		///<summary>特集ページカテゴリ：表示順エラーメッセージ</summary>
		public const string SESSION_KEY_FEATURE_PAGE_CATEGORY_DISPLAY_ORDER_ERRORMESSSAGE = "display_order_errormessage";
		///<summary>特集ページカテゴリ：最上位カテゴリ登録フラグ</summary>
		public const string SESSION_KEY_FEATURE_PAGE_CATEGORY_ROOT_CATEGORY_REGIST_FLG = "root_category_regist_flg";

		/// <summary>Login authentication code session</summary>
		public const string SESSION_KEY_LOGIN_AUTHENTICATION_CODE = "w2cMng_login_authentication_code";
		/// <summary>ログイン失敗回数保持用</summary>
		public const string SESSION_KEY_LOGIN_ERROR_INFO = "w2cMng_loggedin_error_info";
		/// <summary>注文同梱カート情報</summary>
		public const string SESSION_KEY_ORDERCOMBINE_ORIGIN_CART = "ordercombine_origin_cart";

		/// <summary>対象リストのキー（定期台帳）</summary>
		public const string LIST_KEY_FIXED_PURCHASE = "fixed_purchase";
		/// <summary>対象リストのキー（ユーザ）</summary>
		public const string LIST_KEY_USER = "user";

		/// <summary>プレビュー対象HTML No</summary>
		public const string HTML_PREVIEW_NO = "htmlPreviewNo";

		/// <summary>商品付帯情報接頭辞キー</summary>
		public const string HASH_KEY_PRODUCTOPTIONSETTING = "product_option_setting";
		/// <summary>商品連携IDキー</summary>
		public const string HASH_KEY_COOPERATION_ID = "cooperation_id";
		/// <summary>アカウントロックデータキー：中文字列：ログインID</summary>
		public const string ACOUNT_LOCK_KEY_LOGINERRORINFO_MIDDLE_STRING_LOGIN_ID = "_login_id_";
		/// <summary>アカウントロックデータキー：中文字列：パスワード</summary>
		public const string ACOUNT_LOCK_KEY_LOGINERRORINFO_MIDDLE_STRING_PASSWORD = "_password_";
		/// <summary>Cookieキー:カートID補完用クッキー変数</summary>
		public const string COOKIE_KEY_CART_ID = "w2cFront_CartId";
		/// <summary>Cookieキー:ログインID保持用クッキー変数</summary>
		public const string COOKIE_KEY_LOGIN_ID = "w2cFront_LoginId";
		//  <summary>Cookieキー:ユーザーID保持用クッキー変数</summary>
		public const string COOKIE_KEY_USER_ID = "w2cFront_UserId";
		/// <summary>Cookieキー:Previous Url</summary>
		public const string COOKIE_KEY_PREV_URL = "w2Manager_PrevUrl";
		/// <summary>Cookieキー:グローバル対応 リージョン保管用 クッキー変数</summary>
		public const string COOKIE_KEY_GLOBAL_REGION = "global_Region";
		/// <summary>Cookie key fraudbuster</summary>
		public const string COOKIE_KEY_FRAUDBUSTER = "fraudbuster-key";
		/// <summary>領収書発行ORDER ID</summary>
		public const string HASH_KEY_ORDER_ID = "order_id_like_escaped";

		// SEO設定用キー
		public const string SEOSETTING_KEY_PRODUCT_NAME = "@@ product_name @@";
		public const string SEOSETTING_KEY_PRODUCT_NAME_KANA = "@@ product_name_kana @@";
		public const string SEOSETTING_KEY_PRODUCT_PRICE = "@@ product_price @@";
		public const string SEOSETTING_KEY_VARIATION_NAME1 = "@@ variation_name1 @@";
		public const string SEOSETTING_KEY_VARIATION_NAME2 = "@@ variation_name2 @@";
		public const string SEOSETTING_KEY_VARIATION_NAME3 = "@@ variation_name3 @@";
		public const string SEOSETTING_KEY_PARENT_CATEGORY_NAME = "@@ parent_category_name @@";
		public const string SEOSETTING_KEY_ROOT_PARENT_CATEGORY_NAME = "@@ root_parent_category_name @@";
		public const string SEOSETTING_KEY_CATEGORY_NAME = "@@ category_name @@";
		public const string SEOSETTING_KEY_PRODUCT_SEO_KEYWORDS = "@@ seo_keywords @@";
		public const string SEOSETTING_KEY_CHILD_CATEGORY_TOP = "@@ child_category_top @@";
		public const string SEOSETTING_KEY_BRAND_TITLE = "@@ brand_title @@";
		public const string SEOSETTING_KEY_BRAND_SEO_KEYWORD = "@@ brand_seo_keyword @@";
		public const string SEOSETTING_KEY_SEO_TITLE = "@@ seo_title @@";
		public const string SEOSETTING_KEY_HTML_TITLE = "@@ html_title @@";
		public const string SEOSETTING_KEY_SEO_DESCRIPTION = "@@ seo_description @@";
		public const string SEOSETTING_KEY_TITLE = "@@ title @@";
		public const string SEOSETTING_KEY_PRODUCT_NAMES = "@@ product_names @@";
		public const string SEOSETTING_KEY_PRODUCT_COLOR_KEYWORD = "@@ product_color @@";
		public const string SEOSETTING_KEY_PRODUCT_PRICE_KEYWORD = "@@ product_price @@";
		public const string SEOSETTING_KEY_PRODUCT_TAG = "@@ product_tag @@";
		public const string SEOSETTING_KEY_FREE_WORD_KEYWORD = "@@ free_word @@";
		public const string SEOSETTING_KEY_DEFAULT_TEXT = "@@ default_text @@";
		public const string SEOSETTING_KEY_PRODUCT_ICON1 = "@@ product_icon1 @@";
		public const string SEOSETTING_KEY_PRODUCT_ICON2 = "@@ product_icon2 @@";
		public const string SEOSETTING_KEY_PRODUCT_ICON3 = "@@ product_icon3 @@";
		public const string SEOSETTING_KEY_PRODUCT_ICON4 = "@@ product_icon4 @@";
		public const string SEOSETTING_KEY_PRODUCT_ICON5 = "@@ product_icon5 @@";
		public const string SEOSETTING_KEY_PRODUCT_ICON6 = "@@ product_icon6 @@";
		public const string SEOSETTING_KEY_PRODUCT_ICON7 = "@@ product_icon7 @@";
		public const string SEOSETTING_KEY_PRODUCT_ICON8 = "@@ product_icon8 @@";
		public const string SEOSETTING_KEY_PRODUCT_ICON9 = "@@ product_icon9 @@";
		public const string SEOSETTING_KEY_PRODUCT_ICON10 = "@@ product_icon10 @@";

		//------------------------------------------------------
		// メールテンプレートタグ
		//------------------------------------------------------
		public const string MAILTEMPLATE_TAG_W2MPPOINTOPTION = "W2M_PPOINTOPTION";

		/// <summary>メールテンプレートタグ：注文商品の商品ID(複数の場合はカンマ区切り)</summary>
		public const string MAILTEMPLATE_TAG_ORDER_ITEMS_PRODUCT_ID = "order_items_product_id";

		/// <summary>メールテンプレートタグ：注文商品のブランドID(複数の場合はカンマ区切り)</summary>
		public const string MAILTEMPLATE_TAG_ORDER_ITEMS_BRAND_ID = "order_items_brand_id";

		/// <summary>メールテンプレートタグ：注文商品のカテゴリID(複数の場合はカンマ区切り)</summary>
		public const string MAILTEMPLATE_TAG_ORDER_ITEMS_CATEGORY_ID = "order_items_category_id";

		/// <summary>レコメンド系商品の在庫切れ表示可否</summary>
		public const string KEY_SHOW_OUT_OF_STOCK_ITEMS = "show_out_of_stock";

		//========================================================================
		// タスクスケジューラ設定
		//========================================================================
		public const string TASKSCHEDULER_TASKNAMEPREFIX = "w2ProductConverter_";
		public static string TASKSCHEDULER_USER = "";
		public static string TASKSCHEDULER_PASSWORD = "";

		public static string BATCH_MANAGER_TASKSCHEDULER_TARGET_SERVER = string.Empty;
		public static string BATCH_MANAGER_TASKSCHEDULER_USER_NAME = string.Empty;
		public static string BATCH_MANAGER_TASKSCHEDULER_ACCOUNT_DOMAIN = string.Empty;
		public static string BATCH_MANAGER_TASKSCHEDULER_PASSWORD = string.Empty;

		public const string BATCH_MANAGER_ERROR_KBN_RUN_DOUBLE_EXECUTION = "DOUBLE_EXECUTION";
		public const string BATCH_MANAGER_ERROR_KBN_RUN_INFO_MISMATCH = "INFO_MISMATCH";
		public const string BATCH_MANAGER_ERROR_KBN_STOP_NOT_RUNNING = "NOT_RUNNING";

		//========================================================================
		// 定期購入
		//========================================================================
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_INTERVAL_WEEK_LIST = "shipping_interval_week_list";		// 定期購入設定値（週一覧・間隔）
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST = "shipping_week_list";		// 定期購入設定値（週一覧）
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST = "shipping_day_list";		// 定期購入設定値（曜日一覧）
		public const string FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DATE_LIST = "shipping_date_list";		// 定期購入設定値（日一覧）
		/// <summary> 定期購入配送パターン：月間隔日付指定の月間隔 </summary>
		public const string FIXED_PURCHASE_SETTING_MONTH = "Month";
		/// <summary> 定期購入配送パターン：月間隔日付指定の日付 </summary>
		public const string FIXED_PURCHASE_SETTING_MONTHLY_DATE = "MonthlyDate";
		/// <summary> 定期購入配送パターン：月間隔・週・曜日設定の月間隔 </summary>
		public const string FIXED_PURCHASE_SETTING_INTERVAL_MONTHS = "IntervalMonths";
		/// <summary> 定期購入配送パターン：月間隔・週・曜日指定の週 </summary>
		public const string FIXED_PURCHASE_SETTING_WEEK_OF_MONTH = "WeekOfMonth";
		/// <summary> 定期購入配送パターン：月間隔・週・曜日指定の曜日 </summary>
		public const string FIXED_PURCHASE_SETTING_DAY_OF_WEEK = "DayOfWeek";
		/// <summary> 定期購入配送パターン：配送日間隔指定 </summary>
		public const string FIXED_PURCHASE_SETTING_INTERVAL_DAYS = "IntervalDays";
		/// <summary> 定期購入配送パターン：週間隔・曜日指定の週 </summary>
		public const string FIXED_PURCHASE_SETTING_EVERYNWEEK_WEEK = "EveryNWeek_Week";
		/// <summary> 定期購入配送パターン：週間隔・曜日指定の曜日 </summary>
		public const string FIXED_PURCHASE_SETTING_EVERYNWEEK_DAY_OF_WEEK = "EveryNWeek_DayOfWeek";
		/// <summary> 定期購入配送パターン：デフォルト配送日間隔指定オプション </summary>
		public static bool FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG = false;

		/// <summary>配送先住所反映パターン：ユーザ情報も更新する</summary>
		public const string ADDRESS_UPDATE_PATTERN_USER_TOO = "0";
		/// <summary>配送先住所反映パターン：他の定期台帳も更新する</summary>
		public const string ADDRESS_UPDATE_PATTERN_OTHER_FIXED_PURCASES_TOO = "1";

		//========================================================================
		// SingleSignOn系
		//========================================================================
		/// <summary>シングルサインオン暗号化：共有キー</summary>
		public static string ENCRYPTION_SINGLE_SIGN_ON_KEY = null;
		/// <summary>シングルサインオン暗号化：初期化ベクタ</summary>
		public static string ENCRYPTION_SINGLE_SIGN_ON_IV = null;
		/// <summary>許可対象URLリファラ</summary>
		public static List<Uri> ALLOW_SINGLE_SIGN_ON_URL_REFERRER = null;

		//========================================================================
		// プラグイン系
		//========================================================================		
		/// <summary>プラグイン参照ディレクトリパス</summary>
		public static string PHYSICALDIRPATH_PLUGINS_STORAGE_LOCATION = null;
		/// <summary>送信元メールアドレス</summary>
		public static string MAILADDRESS_FROM_FOR_PLUGIN = null;
		/// <summary>メールアドレス（TO）</summary>
		public static List<string> MAILADDRESS_TO_LIST_FOR_PLUGIN = null;
		/// <summary>メールアドレス（CC）</summary>
		public static List<string> MAILADDRESS_CC_LIST_FOR_PLUGIN = null;
		/// <summary>メールアドレス（BCC）</summary>
		public static List<string> MAILADDRESS_BCC_LIST_FOR_PLUGIN = null;

		/// メール送信設定
		/// <summary>メールタイトル名</summary>
		public static string MAIL_SUBJECTHEAD = null;
		/// <summary>メール送信元アドレス</summary>
		public static MailAddress MAIL_FROM = null;
		/// <summary>メール送信To宛先アドレス</summary>
		public static List<MailAddress> MAIL_TO_LIST = new List<MailAddress>();
		/// <summary>メール送信Cc宛先アドレス</summary>
		public static List<MailAddress> MAIL_CC_LIST = new List<MailAddress>();
		/// <summary>メール送信Bcc宛先アドレス</summary>
		public static List<MailAddress> MAIL_BCC_LIST = new List<MailAddress>();
		/// <summary>自動送信対応メールテンプレートIDリスト</summary>
		public static List<string> AUTOSEND_MAIL_ID_LIST = new List<string>
		{
			CONST_MAIL_ID_USER_REGIST,	//会員登録メールテンプレート
			CONST_MAIL_ID_USER_EASY_REGIST,	//かんたん会員登録メールテンプレート
			CONST_MAIL_ID_USER_PREREGIST,	//会員仮登録メールテンプレート（空メールなど）
			CONST_MAIL_ID_PASSWORD_REMINDER,	//パスワードリマインダメールテンプレート
			CONST_MAIL_ID_MAILMAGAZINE_CANCEL,	//メールマガジン解除メールテンプレート	
			CONST_MAIL_ID_ORDER_COMPLETE,	//注文完了メールテンプレート
			CONST_MAIL_ID_ORDER_COMPLETE_FOR_OPERATOR,	//注文完了メールテンプレート(管理者向け)
			CONST_MAIL_ID_ORDER_SHIPPING,	// 出荷手配済みメールテンプレート
			CONST_MAIL_ID_ORDER_CANCEL,		// キャンセルメールテンプレート
			CONST_MAIL_ID_DOCOMO_PAYMENT_REPORT,	//ドコモから送信されるCSVレポート用テンプレート
			CONST_MAIL_ID_DOCOMO_PAYMENT_NEXT,	//ドコモケータイ払い携帯引継ぎメールテンプレート
			CONST_MAIL_ID_RAKUTEN_COOP_ERROR,	//楽天API連携エラーメールテンプレート
			CONST_MAIL_ID_MASTER_IMPORT,	//マスタ取込メールテンプレート
			CONST_MAIL_ID_MASTER_UPLOAD_STOCK_COOPERATION,	//定期実行マスタ取込結果メールテンプレート
			CONST_MAIL_ID_EXTERNAL_IMPORT,	//外部ファイル取込メールテンプレート
			CONST_MAIL_ID_FIXEDPURCHASE_FOR_USER,	//定期購入アラート(ユーザ向け)メールテンプレート
			CONST_MAIL_ID_FIXEDPURCHASE_FOR_OPERATOR,	//定期購入アラート(管理者向け)テンプレート
			CONST_MAIL_ID_CREATE_USERINTEGRATION,	//ユーザー統合作成バッチ結果メールテンプレート
			CONST_MAIL_ID_CUSTOMER_RINGS_EXPORT,	//カスタマーリングス連携バッチ結果メールテンプレート
			CONST_MAIL_ID_REAUTH_ERROR_ADMIN,	// 再与信エラーメール（管理者向け）
			CONST_MAIL_ID_REAUTH_ERROR_USER,	// 再与信エラーメール（ユーザー向け）
			CONST_MAIL_ID_URERU_AD_IMPORT_FOR_OPERATOR,	// 外部連携注文取込エラーメールテンプレート（管理者向け）
			CONST_MAIL_ID_URERU_AD_IMPORT_FOR_USER,	// 外部連携注文取込 与信取得エラーメールテンプレート（ユーザー向け）
			CONST_MAIL_ID_INQUIRY_INPUT_FOR_OPERATOR,	//問合せテンプレート(管理者向け)
			CONST_MAIL_ID_INQUIRY_INPUT_FOR＿USER,	//問合せテンプレート(ユーザ向け)
			CONST_MAIL_ID_MAILMAGAZINE_REGIST,	//メールマガジン登録メールテンプレート
			CONST_MAIL_ID_ACCEPT_PRODUCT_ARRIVAL,	// 再入荷通知受付メールテンプレート
			CONST_MAIL_ID_ACCEPT_PRODUCT_RELEASE,	// 販売開始通知受付メールテンプレート
			CONST_MAIL_ID_ACCEPT_PRODUCT_RESALE,	// 再販売通知受付メールテンプレート
			CONST_MAIL_ID_CHANGE_DEADLINE,	// 定期購入変更期限案内テンプレート
			CONST_MAIL_ID_SKIP_FIXEDPURCHASE,	//定期購入スキップメールテンプレート
			CONST_MAIL_ID_CANCEL_FIXEDPURCHASE,	//定期購入停止メールテンプレート
			CONST_MAIL_ID_RESUME_FIXEDPURCHASE, //定期購入再開メールテンプレート
			CONST_MAIL_ID_SUSPEND_FIXEDPURCHASE, //定期購入休止メールテンプレート
			CONST_MAIL_ID_CHANGE_PAYMENT_METHOD_FOR_USER,	//購入履歴お支払い方法変更メール
			CONST_MAIL_ID_CHANGE_PAYMENT_METHOD_FOR_OPERATOR,	//購入履歴お支払い方法変更メール(管理者向け)
			CONST_MAIL_ID_CHANGE_DELIVERY_DATE_FOR_USER,	//購入履歴お届け日変更メール
			CONST_MAIL_ID_CHANGE_DELIVERY_DATE_FOR_OPERATOR,	//購入履歴お届け日変更メール(管理者向け)
			CONST_MAIL_ID_CHANGE_SHIPPING_ADDRESS_FOR_USER,	//購入履歴お届け先変更メール
			CONST_MAIL_ID_CHANGE_SHIPPING_ADDRESS_FOR_OPERATOR,	//購入履歴お届け先変更メール(管理者向け)
			CONST_MAIL_ID_CHANGE_POINTS_FOR_USER,	//購入履歴利用ポイント変更メール
			CONST_MAIL_ID_CHANGE_POINTS_FOR_OPERATOR,	//購入履歴利用ポイント変更メール(管理者向け)
			CONST_MAIL_ID_CHANGE_ACCOUNT_FOR_USER,	//ユーザー登録情報の変更メール
			CONST_MAIL_ID_CHANGE_ACCOUNT_FOR_OPERATOR,	//ユーザー登録情報の変更メール(管理者向け)
			CONST_MAIL_ID_CHANGE_ACCOUNT_ADDRESS_FOR_USER,	//アドレス帳変更メール
			CONST_MAIL_ID_CHANGE_ACCOUNT_ADDRESS_FOR_OPERATOR,	//アドレス帳変更メール(管理者向け)
			CONST_MAIL_ID_CREDITCARD_REGIST_FOR_USER,	//クレジットカード登録案内メール
			CONST_MAIL_ID_CREDITCARD_REGIST_FOR_OPERATOR,	//クレジットカード登録案内メール(管理者向け)
			CONST_MAIL_ID_CHANGE_PAYMENT_METHOD_FIXEDPURCHASE_FOR_USER,	//定期購入情報お支払い方法変更メール
			CONST_MAIL_ID_CHANGE_PAYMENT_METHOD_FIXEDPURCHASE_FOR_OPERATOR,	//定期購入情報お支払い方法変更メール(管理者向け)
			CONST_MAIL_ID_CHANGE_DELIVERY_DATE_FIXEDPURCHASE_FOR_USER,	//定期購入情報配送パターン変更メール
			CONST_MAIL_ID_CHANGE_DELIVERY_DATE_FIXEDPURCHASE_FOR_OPERATOR,	//定期購入情報配送パターン変更メール(管理者向け)
			CONST_MAIL_ID_CHANGE_SHIPPING_ADDRESS_FIXEDPURCHASE_FOR_USER,	//定期購入情報お届け先変更メール
			CONST_MAIL_ID_CHANGE_SHIPPING_ADDRESS_FIXEDPURCHASE_FOR_OPERATOR,	//定期購入情報お届け先変更メール(管理者向け)
			CONST_MAIL_ID_CHANGE_POINTS_FIXEDPURCHASE_FOR_USER,	//定期購入情報次回配送日変更メール
			CONST_MAIL_ID_CHANGE_POINTS_FIXEDPURCHASE_FOR_OPERATOR,	//定期購入情報次回配送日変更メール(管理者向け)
			CONST_MAIL_ID_CHANGE_SHIPPING_DATE_FIXEDPURCHASE_FOR_USER,	//定期購入情報利用ポイント変更メール
			CONST_MAIL_ID_CHANGE_SHIPPING_DATE_FIXEDPURCHASE_FOR_OPERATOR,	//定期購入情報利用ポイント変更メール(管理者向け)
			CONST_MAIL_ID_CS_NOTIFICATION,	//CS系通知メール
			CONST_MAIL_ID_CS_WARNING_OPERATOR,	//CS系警告メール（オペレータ向け：本人）
			CONST_MAIL_ID_CS_WARNING_GROUP,	//CS系警告メール（オペレータ向け：グループ）
			CONST_MAIL_ID_CS_WARNING_ADMIN,	//CS系警告メール（管理者向け）
			CONST_MAIL_ID_ORDERCOMBINE_ORDER_COMPLETE,	// 注文同梱完了メールテンプレート（通常注文・定期注文）
			CONST_MAIL_ID_ORDERCOMBINE_ERROR_FOR_OPERATOR,	// 注文同梱エラーメールテンプレート(管理者向け)
			CONST_MAIL_ID_ORDER_UPDATE_FOR_USER,	// 注文更新通知メールテンプレート
			CONST_MAIL_ID_ORDER_UPDATE_FOR_OPERATOR,	// 注文更新通知メールテンプレート(管理者向け)
			CONST_MAIL_ID_ORDER_CANCEL_BY_RECOMMEND_FOR_USER,	// レコメンド時注文キャンセル通知メール
			CONST_MAIL_ID_ORDER_CANCEL_BY_RECOMMEND_FOR_OPERATOR,	// レコメンド時注文キャンセル通知メール(管理者向け)
			CONST_MAIL_ID_ORDER_WORKFLOW_SCENARIO_EXEC,	// シナリオワークフロー実行通知通知メール((管理者向け)
			CONST_MAIL_ID_WMS_COOPERATION_FOR_OPERATOR,	// WMS連携通知メールテンプレート(管理者向け)
			CONST_MAIL_ID_RECEIPT, //領収書発行(管理者向け)
			CONST_MAIL_ID_MALL_ORDER_IMPORTER_TERMINATION, // モール注文取込バッチ完了メール【管理者向け】
			CONST_MAIL_ID_MALL_ORDER_IMPORTER_ABNORMAL_TERMINATION // モール注文取込バッチ異常終了ーメール【管理者向け】
			,CONST_MAIL_ID_MYPAGE_ORDER_MODIFY_FOR_OPERATOR // マイページからの注文情報編集：管理者向けメール
			,CONST_MAIL_ID_MYPAGE_ORDER_MODIFY_FOR_USER // マイページからの注文情報編集：ユーザー向けメール
			,CONST_MAIL_ID_MYPAGE_FIXED_PURCHASE_MODIFY_FOR_OPERATOR // マイページからの定期情報編集：管理者向けメール
			,CONST_MAIL_ID_MYPAGE_FIXED_PURCHASE_MODIFY_FOR_USER // マイページからの定期情報編集：ユーザー向けメール
			,CONST_MAIL_ID_NEXT_ENGINE_ORDER_COMBINE_COMPLETE_FOR_MANAGER // ネクストエンジン：注文同梱完了メール【管理者向け】
			,CONST_MAIL_ID_NEXT_ENGINE_ORDER_CANCEL_FAIL_FOR_MANAGER // ネクストエンジン：ネクストエンジン注文キャンセル失敗メール【管理者向け】
			,CONST_MAIL_ID_CANCEL_USERINTEGRATION_ASYNC	// Cancel user integration async result mail id
		};
		/// <summary>メール送信方法</summary>
		public enum MailSendMethod
		{
			/// <summary>自動</summary>
			Auto,
			/// <summary>マニュアル</summary>
			Manual
		};

		/// <summary>エラーページ用のログファイルの頭文字</summary>
		public const string INITIALS_ERROR_LOG = "errorpage";
		/// <summary>外部連携APIバッチの(案件毎)DLL参照ディレクトリパス</summary>
		public static string PHYSICALDIRPATH_EXTERNALAPI_STORAGE_LOCATION = null;

		/// <summary>Physical directory path external api storage setting location</summary>
		public static string PHYSICALDIRPATH_EXTERNALAPI_STORAGE_SETTING_LOCATION = string.Empty;

		/// <summary>FreeExport連携作業ディレクトリパス</summary>
		public static string SETTING_DIRPATH_FREEEXPORT = "";
		/// <summary>FreeExport出力文字コード UTF-8 BOMなし</summary>
		public const string CONST_FREEEXPORT_ENCODING_UTF8N = "utf-8n";
		/// <summary>LetroExport連携作業ディレクトリパス</summary>
		public static string SETTING_DIRPATH_LETROEXPORT = string.Empty;

		//*****************************************************************************************
		// 画像・マネージャサイド
		//*****************************************************************************************
		public const string IMG_MANAGER_PAGING_BACK1 = "Images/Common/paging_back_01.gif";
		public const string IMG_MANAGER_PAGING_BACK2 = "Images/Common/paging_back_02.gif";
		public const string IMG_MANAGER_PAGING_NEXT1 = "Images/Common/paging_next_01.gif";
		public const string IMG_MANAGER_PAGING_NEXT2 = "Images/Common/paging_next_02.gif";
		public const string IMG_MANAGER_NO_IMAGE = "Images/Common/no_image.png";

		//=========================================================================================
		// イメージコンバータ
		//=========================================================================================
		// 画像変換コンバータ
		public const string REQUEST_KEY_IMGCNV_SIZE = "sz";
		public const string REQUEST_KEY_IMGCNV_FORMAT = "fmt";
		public const string REQUEST_KEY_IMGCNV_FILE = "file";
		public const string REQUEST_KEY_IMGCNV_IMAGE_FLG = "if";

		// 画像形式
		public const string KBN_IMAGEFORMAT_BMP = "bmp";
		public const string KBN_IMAGEFORMAT_GIF = "gif";
		public const string KBN_IMAGEFORMAT_PNG = "png";
		public const string KBN_IMAGEFORMAT_JPG = "jpg";

		// デフォルト画像
		public const string PRODUCTIMAGE_NOIMAGE_PC = "NowPrinting_M.jpg";
		public const string PRODUCTIMAGE_NOIMAGE_PC_LL = "NowPrinting_LL.jpg";
		public const string PRODUCTIMAGE_NOIMAGE_MOBILE = "NowPrinting.jpg";

		/// <summary>特集エリアタイプ画像 縦並び</summary>
		public const string FEATUREAREATYPEIMAGE_VERTICAL = "feature_area_type_vertical.png";
		/// <summary>特集エリアタイプ画像 横並び</summary>
		public const string FEATUREAREATYPEIMAGE_SIDE = "feature_area_type_side.png";
		/// <summary>特集エリアタイプ画像 スライダー</summary>
		public const string FEATUREAREATYPEIMAGE_SLIDER = "feature_area_type_slider.png";
		/// <summary>特集エリアタイプ画像 ランダム</summary>
		public const string FEATUREAREATYPEIMAGE_RANDOM = "feature_area_type_random.png";
		/// <summary>特集エリアタイプ画像 その他</summary>
		public const string FEATUREAREATYPEIMAGE_OTHER = "feature_area_type_other.png";

		// 最初の画像
		public const string CONTENTS_IMAGE_FIRST = "1.jpg";

		// コーディネート画像
		public const string COORDINATEIMAGE_PREFIX = "cord_";

		/// <summary>SQLタイムアウト値設定（各ＤＢファイルの自動拡張を考慮して多めにとる）</summary>
		public const int SQL_COMMAND_TIMEOUT = 120;

		/// <summary>ターゲット リストのインポート種別</summary>
		public static List<string> TARGET_LIST_IMPORT_TYPE_LIST = new List<string>
		{
			/// <summary>アップロードターゲットリスト</summary>
			FLG_TARGETLIST_TARGET_TYPE_CSV,
			/// <summary>ターゲットリストマージ</summary>
			FLG_TARGETLIST_TARGET_TYPE_MERGE,
			/// <summary>ユーザー情報一覧</summary>
			FLG_TARGETLIST_TARGET_TYPE_USER_LIST,
			/// <summary>受注情報一覧</summary>
			FLG_TARGETLIST_TARGET_TYPE_ORDER_LIST,
			/// <summary>受注ワークフロー一覧</summary>
			FLG_TARGETLIST_TARGET_TYPE_ORDERWORKFLOW_LIST,
			/// <summary>定期購入情報一覧</summary>
			FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_LIST,
			/// <summary>定期ワークフロー一覧</summary>
			FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_WORKFLOW_LIST,
			/// <summary>Target type: fixed purchase repeat analysis report</summary>
			FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_REPEAT_ANALYSIS_REPORT,
		};

		// ターゲットリストテンプレート
		public const string FILE_XML_TARGETLIST_TEMPLATE = "Xml/TargetList/TargetListTemplate.xml";

		//========================================================================
		// マスタ出力系
		//========================================================================		
		/// <summary>マスタ出力設定：削除不可文字</summary>
		public static string MASTEREXPORTSETTING_MASTER_UNREMOVABLE = string.Empty;
		/// <summary>マスタ出力設定：デフォルト用ID</summary>
		public const string MASTEREXPORTSETTING_MASTER_SETTING_ID = "001";
		/// <summary>マスタ出力設定：選択した設定ID</summary>
		public const string MASTEREXPORTSETTING_SELECTED_SETTING_ID = "SelectedSettingID";
		/// <summary>フィールド名：論理名</summary>
		public const string MASTEREXPORTSETTING_XML_J_NAME = "jname";
		/// <summary>フィールド名：設定文字列</summary>
		public const string MASTEREXPORTSETTING_XML_NAME = "name";
		/// <summary>フィールド名：物理名</summary>
		public const string MASTEREXPORTSETTING_XML_FIELD = "field";
		/// <summary>フィールド名：オプション</summary>
		public const string MASTEREXPORTSETTING_XML_OPTION = "option";
		/// <summary>フィールド名：データタイプ</summary>
		public const string MASTEREXPORTSETTING_XML_TYPE = "type";

		//========================================================================
		// 支払回数系
		//========================================================================
		public const string FIELD_CREDIT_INSTALLMENTS = "credit_installments";
		public const string FIELD_CREDIT_INSTALLMENTS_SBPS = "credit_installments_sbps";
		public const string FIELD_CREDIT_INSTALLMENTS_YAMATOKWC = "credit_installments_yamatokwc";
		public const string FIELD_CREDIT_INSTALLMENTS_NEWEBPAY = "credit_installments_neweb";
		public const string FIELD_CREDIT_INSTALLMENTS_ESCOTT = "credit_installments_escott";
		public const string FIELD_CREDIT_INSTALLMENTS_VERITRANS = "credit_installments_veritrans";
		public const string FIELD_CREDIT_INSTALLMENTS_RAKUTEN = "credit_installments_rakuten";
		public const string FIELD_CREDIT_INSTALLMENTS_PAYGENT = "credit_installments_paygent";

		public const string FIELD_CREDIT_INSTALLMENTS_VALUE = "01";
		public const string FIELD_CREDIT_INSTALLMENTS_SBPS_VALUE = "1";
		public const string FIELD_CREDIT_INSTALLMENTS_YAMATOKWC_VALUE = "1";
		public const string FIELD_CREDIT_INSTALLMENTS_ESCOTT_VALUE = "01";
		public const string FIELD_CREDIT_INSTALLMENTS_VERITRANS_VALUE = "01";
		public const string FIELD_CREDIT_INSTALLMENTS_RAKUTEN_VALUE = "1";
		public const string FIELD_CREDIT_INSTALLMENTS_GMO_VALUE = "01";
		public const string FIELD_CREDIT_INSTALLMENTS_PAYGENT_VALUE = "10";

		// 出荷連携配送会社
		/// <summary>ヤマト・GMO以外</summary>
		public const string DELOVERY_COMPANY_TYPE = "delivery_company_type";
		/// <summary>ヤマト</summary>
		public const string DELOVERY_COMPANY_TYPE_YAMATO = "delivery_company_type_yamato";
		/// <summary>GMO</summary>
		public const string DELOVERY_COMPANY_TYPE_GMO = "delivery_company_type_gmo";
		/// <summary>Atodene</summary>
		public const string DELOVERY_COMPANY_TYPE_ATODENE = "delivery_company_type_atodene";
		/// <summary>DSK後払い</summary>
		public const string DELOVERY_COMPANY_TYPE_DSK_DEFERRED = "delivery_company_type_dsk_deferred";
		/// <summary>NP</summary>
		public const string DELIVERY_COMPANY_TYPE_NP = "delivery_company_type_np";
		/// <summary>Gooddeal</summary>
		public const string DELIVERY_COMPANY_TYPE_GOODDEAL = "delivery_company_type_gooddeal";
		/// <summary>Atobaraicom</summary>
		public const string DELIVERY_COMPANY_TYPE_ATOBARAICOM = "delivery_company_type_atobaraicom";
		/// <summary>スコア後払い</summary>
		public const string DELIVERY_COMPANY_TYPE_SCORE_DEFERRED = "delivery_company_type_score_deferred";

		// 楽天API処理結果取得時のエラーコード
		public const string RAKUTEN_API_RESULT_ERROR_CODE_NORMAL = "N00-000";						// 正常終了
		public const string RAKUTEN_API_RESULT_ERROR_CODE_ORDER_NOT_FOUND = "E10-001";				// 検索結果が0件です
		public const string RAKUTEN_API_RESULT_ERROR_CODE_CANNOT_GET_SPECIFIED_ORDERS = "E10-301";	// 指定した受注番号を取得できません
		public const string RAKUTEN_API_RESULT_ERROR_CODE_ORDER_CANCELED = "E10-303";				// 既にキャンセルされています
		public const string RAKUTEN_API_RESULT_ERROR_CODE_HAVE_UNIT_ERROR = "W00-000";				// エラーあり

		// 楽天ペイ受注API処理結果取得時のエラーコード
		public const string RAKUTEN_PAY_API_MESSAGE_TYPE_INFO = "INFO";
		public const string RAKUTEN_PAY_API_MESSAGE_TYPE_ERROR = "ERROR";
		public const string RAKUTEN_PAY_API_MESSAGE_TYPE_WARN = "WARN";

		// 楽天APIの期間検索種別（dateType）
		public const int RAKUTEN_API_DATE_TYPE_ORDER = 1;		// 注文日
		public const int RAKUTEN_API_DATE_TYPE_PAYMENT = 2;		// 入金日
		public const int RAKUTEN_API_DATE_TYPE_SHIPPING = 3;	// 発送日

		//========================================================================
		// 楽天IDConnect連携
		//========================================================================
		/// <summary>楽天IDConnect：有効無効</summary>
		public static bool RAKUTEN_LOGIN_ENABLED = false;
		/// <summary>クライアントID（接続元を特定するためのID情報）</summary>
		public static string RAKUTEN_ID_CONNECT_CLIENT_ID = "";
		/// <summary>クライアントSecret（接続元を認証するためのキー情報）</summary>
		public static string RAKUTEN_ID_CONNECT_CLIENT_SECRET = "";
		/// <summary>楽天IDConnect連携用モックURL（※開発時は値を入れる）</summary>
		public static string RAKUTEN_ID_CONNECT_MOCK_URL = "";
		/// <summary>楽天IDConnect連携用デバッグログ出力（※ログに個人情報が含まれるため、必要な時だけTRUEにする）</summary>
		public static bool RAKUTEN_ID_CONNECT_OUTPUT_DEBUGLOG = false;
		/// <summary>楽天IDConnect連携：Open ID格納用ユーザー拡張項目</summary>
		public static string RAKUTEN_ID_CONNECT_OPEN_ID = "";
		/// <summary>楽天IDConnect連携：楽天IDConnect登録フラグユーザー拡張項目</summary>
		public static string RAKUTEN_ID_CONNECT_REGISTER_USER = "";

		// 楽天ペイ受注APIパラメータ名
		/// <summary>基本モデル</summary>
		public const string RAKUTEN_PAY_API_RESPONSE_BASE = "base";
		/// <summary>メッセージモデルリスト</summary>
		public const string RAKUTEN_PAY_API_RESPONSE_MESSAGE_MODEL_LIST = "MessageModelList";
		/// <summary>受注モデルリスト</summary>
		public const string RAKUTEN_PAY_API_RESPONSE_ORDER_MODEL_LIST = "OrderModelList";
		/// <summary>SKUモデルリスト</summary>
		public const string RAKUTEN_ORDER_API_RESPONSE_ORDER_SKU_MODEL_LIST = "SkuModelList";
		/// <summary>メッセージ種別</summary>
		public const string RAKUTEN_PAY_API_MESSAGE_MODEL_MESSAGE_TYPE = "messageType";
		/// <summary>メッセージコード</summary>
		public const string RAKUTEN_PAY_API_MESSAGE_MODEL_MESSAGE_CODE = "messageCode";
		/// <summary>メッセージ</summary>
		public const string RAKUTEN_PAY_API_MESSAGE_MODEL_MESSAGE = "message";
		/// <summary>注文番号</summary>
		public const string RAKUTEN_PAY_API_MESSAGE_MODEL_ORDER_NUMBER = "orderNumber";
		/// <summary>注文番号</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_ORDER_NUMBER = "orderNumber";
		/// <summary>ステータス </summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_ORDER_PROGRESS = "orderProgress";
		/// <summary>サブステータスID	</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_SUB_STATUS_ID = "subStatusId";
		/// <summary>サブステータス</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_SUB_STATUS_NAME = "subStatusName";
		/// <summary>注文日時（YYYY-MM-DDThh:mm:ss+0900）</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_ORDER_DATETIME = "orderDatetime";
		/// <summary>注文確認日時（YYYY-MM-DDThh:mm:ss+0900）</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_SHOP_ORDER_CFM_DATETIME = "shopOrderCfmDatetime";
		/// <summary>注文確定日時（YYYY-MM-DDThh:mm:ss+0900）</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_ORDER_FIX_DATETIME = "orderFixDatetime";
		/// <summary>発送指示日時（YYYY-MM-DDThh:mm:ss+0900）</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_SHIPPING_INST_DATETIME = "shippingInstDatetime";
		/// <summary>発送完了報告日時（YYYY-MM-DDThh:mm:ss+0900）</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_SHIPPING_CMPL_RPT_DATETIME = "shippingCmplRptDatetime";
		/// <summary>キャンセル期限日（YYYY-MM-DD）</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_CANCEL_DUE_DATE = "cancelDueDate";
		/// <summary>お届け日指定（YYYY-MM-DD）</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_DELIVERY_DATE = "deliveryDate";
		/// <summary>お届け時間帯</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_SHIPPING_TERM = "shippingTerm";
		/// <summary>コメント</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_REMARKS = "remarks";
		/// <summary>ギフト配送希望フラグ</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_GIFT_CHECK_FLAG = "giftCheckFlag";
		/// <summary>複数送付先フラグ</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_SEVERAL_SENDER_FLAG = "severalSenderFlag";
		/// <summary>送付先一致フラグ</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_EQUAL_SENDER_FLAG = "equalSenderFlag";
		/// <summary>離島フラグ</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_ISOLATED_ISLAND_FLAG = "isolatedIslandFlag";
		/// <summary>楽天会員フラグ</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_RAKUTEN_MEMBER_FLAG = "rakutenMemberFlag";
		/// <summary>利用端末</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_CARRIER_CODE = "carrierCode";
		/// <summary>メールキャリアコード</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_EMAIL_CARRIER_CODE = "emailCarrierCode";
		/// <summary>注文種別</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_ORDER_TYPE = "orderType";
		/// <summary>申込番号 </summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_RESERVE_NUMBER = "reserveNumber";
		/// <summary>申込お届け回数 （予約=1、定期購入,頒布会=確定回数）</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_RESERVE_DELIVERY_COUNT = "reserveDeliveryCount";
		/// <summary>警告表示タイプ</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_CAUTION_DISPLAY_TYPE = "cautionDisplayType";
		/// <summary>警告表示詳細タイプ</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_CAUTION_DISPLAY_DETAIL_TYPE = "cautionDisplayDetailType";
		/// <summary>楽天確認中フラグ</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_RAKUTEN_CONFIRM_FLAG = "rakutenConfirmFlag";
		/// <summary>商品合計金額</SUMMARY>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_GOODS_PRICE = "goodsPrice";
		/// <summary>消費税合計</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_GOODS_TAX = "goodsTax";
		/// <summary>送料合計</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_POSTAGE_PRICE = "postagePrice";
		/// <summary>代引料合計</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_DELIVERY_PRICE = "deliveryPrice";
		/// <summary>決済手数料合計</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_PAYMENT_CHARGE = "paymentCharge";
		/// <summary>決済手数料税率</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_PAYMENT_CHARGE_TAX_RATE = "paymentChargeTaxRate";
		/// <summary>合計金額</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_TOTAL_PRICE = "totalPrice";
		/// <summary>請求金額</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_REQUEST_PRICE = "requestPrice";
		/// <summary>クーポン利用総額</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_COUPON_ALL_TOTAL_PRICE = "couponAllTotalPrice";
		/// <summary>店舗発行クーポン利用額</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_COUPON_SHOP_PRICE = "couponShopPrice";
		/// <summary>楽天発行クーポン利用額</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_COUPON_OTHER_PRICE = "couponOtherPrice";
		/// <summary>店舗負担金合計</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_ADDITIONAL_FEE_OCCUR_AMOUNT_TO_SHOP = "additionalFeeOccurAmountToShop";
		/// <summary>あす楽希望フラグ</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_ASURAKU_FLAG = "asurakuFlag";
		/// <summary>医薬品受注フラグ</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_DRUG_FLAG = "drugFlag";
		/// <summary>楽天スーパーDEAL商品受注フラグ</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_DEAL_FLAG = "dealFlag";
		/// <summary>メンバーシッププログラム受注タイプ</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_MEMBERSHIP_TYPE = "membershipType";
		/// <summary>ひとことメモ</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_MEMO = "memo";
		/// <summary>担当者</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_OPERATOR = "operator";
		/// <summary>メール差込文(お客様へのメッセージ)</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_MAIL_PLUG_SENTENCE = "mailPlugSentence";
		/// <summary>購入履歴修正有無フラグ</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_MODIFY_FLAG = "modifyFlag";
		/// <summary>領収書発行回数</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_RCEIPT_ISSUE_COUNT = "receiptIssueCount";
		/// <summary>領収書発行履歴リスト</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_RCEIPT_ISSUE_HISTORY_LIST = "receiptIssueHistoryList";
		/// <summary>消費税再計算フラグ</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_IS_TAX_RECALC = "isTaxRecalc";
		/// <summary>注文者モデル</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_ORDERER_MODEL = "OrdererModel";
		/// <summary>支払方法モデル</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_SETTLEMENT_MODEL = "SettlementModel";
		/// <summary>支払方法モデル</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_SKU_MODEL_LIST = "SkuModelList";
		/// <summary>配送方法モデル</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_DELIVERY_MODEL = "DeliveryModel";
		/// <summary>ポイントモデル</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_POINT_MODEL = "PointModel";
		/// <summary>ラッピングモデル1</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_WRAPPING_MODEL1 = "WrappingModel1";
		/// <summary>ラッピングモデル2</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_WRAPPING_MODEL2 = "WrappingModel2";
		/// <summary>送付先モデルリスト</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_PACKAGE_MODEL_LIST = "PackageModelList";
		/// <summary>クーポンモデルリスト</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_COUPON_MODEL_LIST = "CouponModelList";
		/// <summary>郵便番号1</summary>
		public const string RAKUTEN_PAY_API_ORDERER_MODEL_ZIP_CODE1 = "zipCode1";
		/// <summary>郵便番号2</summary>
		public const string RAKUTEN_PAY_API_ORDERER_MODEL_ZIP_CODE2 = "zipCode2";
		/// <summary>都道府県</summary>
		public const string RAKUTEN_PAY_API_ORDERER_MODEL_PREFECTURE = "prefecture";
		/// <summary>郡市区</summary>
		public const string RAKUTEN_PAY_API_ORDERER_MODEL_CITY = "city";
		/// <summary>それ以降の住所</summary>
		public const string RAKUTEN_PAY_API_ORDERER_MODEL_SUB_ADDRESS = "subAddress";
		/// <summary>姓</summary>
		public const string RAKUTEN_PAY_API_ORDERER_MODEL_FAMILY_NAME = "familyName";
		/// <summary>名</summary>
		public const string RAKUTEN_PAY_API_ORDERER_MODEL_FIRST_NAME = "firstName";
		/// <summary>姓カナ</summary>
		public const string RAKUTEN_PAY_API_ORDERER_MODEL_FAMILY_NAME_KANA = "familyNameKana";
		/// <summary>名カナ</summary>
		public const string RAKUTEN_PAY_API_ORDERER_MODEL_FIRST_NAME_KANA = "firstNameKana";
		/// <summary>電話番号1</summary>
		public const string RAKUTEN_PAY_API_ORDERER_MODEL_PHONE_NUMBER1 = "phoneNumber1";
		/// <summary>電話番号2</summary>
		public const string RAKUTEN_PAY_API_ORDERER_MODEL_PHONE_NUMBER2 = "phoneNumber2";
		/// <summary>電話番号3</summary>
		public const string RAKUTEN_PAY_API_ORDERER_MODEL_PHONE_NUMBER3 = "phoneNumber3";
		/// <summary>メールアドレス ※マスキング</summary>
		public const string RAKUTEN_PAY_API_ORDERER_MODEL_EMAIL_ADDRESS = "emailAddress";
		/// <summary>性別</summary>
		public const string RAKUTEN_PAY_API_ORDERER_MODEL_SEX = "sex";
		/// <summary>誕生日(年)</summary>
		public const string RAKUTEN_PAY_API_ORDERER_MODEL_BIRTH_YEAR = "birthYear";
		/// <summary>誕生日(月)</summary>
		public const string RAKUTEN_PAY_API_ORDERER_MODEL_BIRTH_MONTH = "birthMonth";
		/// <summary>誕生日(日)</summary>
		public const string RAKUTEN_PAY_API_ORDERER_MODEL_BIRTH_DAY = "birthDay";
		/// <summary>注文者負担金合計</summary>
		public const string RAKUTEN_PAY_API_ORDERER_MODEL_ADDITIONAL_FEE_OCCUR_AMOUNT_TO_USER = "additionalFeeOccurAmountToUser";
		/// <summary>支払方法コード</summary>
		public const string RAKUTEN_PAY_API_SETTLEMENT_MODEL_SETTLEMENT_METHOD_CODE = "settlementMethodCode";
		/// <summary>支払方法名</summary>
		public const string RAKUTEN_PAY_API_SETTLEMENT_MODEL_SETTLEMENT_METHOD = "settlementMethod";
		/// <summary>楽天市場の共通決済手段フラグ</summary>
		public const string RAKUTEN_PAY_API_SETTLEMENT_MODEL_RPAY_SETTLEMENT_FLAG = "rpaySettlementFlag";
		/// <summary>クレジットカード種類</summary>
		public const string RAKUTEN_PAY_API_SETTLEMENT_MODEL_CARD_NAME = "cardName";
		/// <summary>クレジットカード番号</summary>
		public const string RAKUTEN_PAY_API_SETTLEMENT_MODEL_CARD_NUMBER = "cardNumber";
		/// <summary>クレジットカード名義人</summary>
		public const string RAKUTEN_PAY_API_SETTLEMENT_MODEL_CARD_OWNER = "cardOwner";
		/// <summary>クレジットカード有効期限</summary>
		public const string RAKUTEN_PAY_API_SETTLEMENT_MODEL_CARD_YM = "cardYm";
		/// <summary>クレジットカード支払い方法</summary>
		public const string RAKUTEN_PAY_API_SETTLEMENT_MODEL_CARD_PAY_TYPE = "cardPayType";
		/// <summary>クレジットカード支払い回数</summary>
		public const string RAKUTEN_PAY_API_SETTLEMENT_MODEL_CARD_INSTALLMENT_DESC = "cardInstallmentDesc";
		/// <summary>配送方法</summary>
		public const string RAKUTEN_PAY_API_DELIVERY_MODEL_DELIVERY_NAME = "deliveryName";
		/// <summary>配送区分</summary>
		public const string RAKUTEN_PAY_API_DELIVERY_MODEL_DELIVERY_CLASS = "deliveryClass";
		/// <summary>ポイント利用額</summary>
		public const string RAKUTEN_PAY_API_POINT_MODEL_USED_POINT = "usedPoint";
		/// <summary>ラッピングタイトル</summary>
		public const string RAKUTEN_PAY_API_WRAPPING_MODEL_TITLE = "title";
		/// <summary>ラッピング名</summary>
		public const string RAKUTEN_PAY_API_WRAPPING_MODEL_NAME = "name";
		/// <summary>料金</summary>
		public const string RAKUTEN_PAY_API_WRAPPING_MODEL_PRICE = "price";
		/// <summary>税込別</summary>
		public const string RAKUTEN_PAY_API_WRAPPING_MODEL_INCLUDE_TAX_FLAG = "includeTaxFlag";
		/// <summary>ラッピング削除フラグ</summary>
		public const string RAKUTEN_PAY_API_WRAPPING_MODEL_DELETE_WRAPPING_FLAG = "deleteWrappingFlag";
		/// <summary>ラッピング税率</summary>
		public const string RAKUTEN_PAY_API_WRAPPING_MODEL_TAX_RATE = "taxRate";
		/// <summary>ラッピング税額</summary>
		public const string RAKUTEN_PAY_API_WRAPPING_MODEL_TAX_PRICE = "taxPrice";
		/// <summary>送付先ID</summary>
		public const string RAKUTEN_PAY_API_PACKAGE_MODEL_BASKET_ID = "basketId";
		/// <summary>送料</summary>
		public const string RAKUTEN_PAY_API_PACKAGE_MODEL_POSTAGE_PRICE = "postagePrice";
		/// <summary>送料税率</summary>
		public const string RAKUTEN_PAY_API_PACKAGE_MODEL_POSTAGE_TAX_RATE = "postageTaxRate";
		/// <summary>代引料</summary>
		public const string RAKUTEN_PAY_API_PACKAGE_MODEL_DELIVERY_PRICE = "deliveryPrice";
		/// <summary>代引料税率</summary>
		public const string RAKUTEN_PAY_API_PACKAGE_MODEL_DELIVERY_TAX_RATE = "deliveryTaxRate";
		/// <summary>消費税</summary>
		public const string RAKUTEN_PAY_API_PACKAGE_MODEL_GOODS_TAX = "goodsTax";
		/// <summary>商品合計金額</summary>
		public const string RAKUTEN_PAY_API_PACKAGE_MODEL_GOODS_PRICE = "goodsPrice";
		/// <summary>合計金額</summary>
		public const string RAKUTEN_PAY_API_PACKAGE_MODEL_TOTAL_PRICE = "totalPrice";
		/// <summary>のし</summary>
		public const string RAKUTEN_PAY_API_PACKAGE_MODEL_NOSHI = "noshi";
		/// <summary>購入時配送会社</summary>
		public const string RAKUTEN_PAY_API_PACKAGE_MODEL_DEFAULT_DELIVERY_COMPANY_CODE = "defaultDeliveryCompanyCode";
		/// <summary>送付先モデル削除フラグ</summary>
		public const string RAKUTEN_PAY_API_PACKAGE_MODEL_PACKAGE_DELETE_FLAG = "packageDeleteFlag";
		/// <summary>送付者モデル</summary>
		public const string RAKUTEN_PAY_API_PACKAGE_MODEL_SENDER_MODEL = "SenderModel";
		/// <summary>商品モデルリスト</summary>
		public const string RAKUTEN_PAY_API_PACKAGE_MODEL_ITEM_MODEL_LIST = "ItemModelList";
		/// <summary>発送モデルリスト</summary>
		public const string RAKUTEN_PAY_API_PACKAGE_MODEL_SHIPPING_MODEL_LIST = "ShippingModelList";
		/// <summary>郵便番号1</summary>
		public const string RAKUTEN_PAY_API_SENDER_MODEL_ZIP_CODE1 = "zipCode1";
		/// <summary>郵便番号2</summary>
		public const string RAKUTEN_PAY_API_SENDER_MODEL_ZIP_CODE2 = "zipCode2";
		/// <summary>都道府県</summary>
		public const string RAKUTEN_PAY_API_SENDER_MODEL_PREFECTURE = "prefecture";
		/// <summary>郡市区</summary>
		public const string RAKUTEN_PAY_API_SENDER_MODEL_CITY = "city";
		/// <summary>それ以降の住所</summary>
		public const string RAKUTEN_PAY_API_SENDER_MODEL_SUBADDRESS = "subAddress";
		/// <summary>姓</summary>
		public const string RAKUTEN_PAY_API_SENDER_MODEL_FAMILY_NAME = "familyName";
		/// <summary>名</summary>
		public const string RAKUTEN_PAY_API_SENDER_MODEL_FIRST_NAME = "firstName";
		/// <summary>姓カナ</summary>
		public const string RAKUTEN_PAY_API_SENDER_MODEL_FAMILY_NAME_KANA = "familyNameKana";
		/// <summary>名カナ</summary>
		public const string RAKUTEN_PAY_API_SENDER_MODEL_FIRST_NAME_KANA = "firstNameKana";
		/// <summary>電話番号1</summary>
		public const string RAKUTEN_PAY_API_SENDER_MODEL_PHONE_NUMBER1 = "phoneNumber1";
		/// <summary>電話番号2</summary>
		public const string RAKUTEN_PAY_API_SENDER_MODEL_PHONE_NUMBER2 = "phoneNumber2";
		/// <summary>電話番号3</summary>
		public const string RAKUTEN_PAY_API_SENDER_MODEL_PHONE_NUMBER3 = "phoneNumber3";
		/// <summary>離島フラグ</summary>
		public const string RAKUTEN_PAY_API_SENDER_MODEL_ISOLATED_ISLAND_FLAG = "isolatedIslandFlag";
		/// <summary>商品明細ID</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_ITEM_DETAIL_ID = "itemDetailId";
		/// <summary>商品名</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_ITEM_NAME = "itemName";
		/// <summary>商品ID	</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_ITEM_ID = "itemId";
		/// <summary>商品番号</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_ITEM_NUMBER = "itemNumber";
		/// <summary>商品管理番号</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_MANAGE_NUMBER = "manageNumber";
		/// <summary>単価</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_PRICE = "price";
		/// <summary>個数</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_UNITS = "units";
		/// <summary>送料込別</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_INCLUDE_POSTAGE_FLAG = "includePostageFlag";
		/// <summary>税込別</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_INCLUDE_TAX_FLAG = "includeTaxFlag";
		/// <summary>代引手数料込別</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_INCLUDE_CASH_ON_DELIVERY_POSTAGE_FLAG = "includeCashOnDeliveryPostageFlag";
		/// <summary>項目・選択肢</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_SELECTED_CHOICE = "selectedChoice";
		/// <summary>ポイント倍率</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_POINT_RATE = "pointRate";
		/// <summary>ポイントタイプ</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_POINT_TYPE = "pointType";
		/// <summary>在庫タイプ</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_INVENTORY_TYPE = "inventoryType";
		/// <summary>納期情報</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_DELVDATE_INFO = "delvdateInfo";
		/// <summary>在庫連動オプション</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_RESTORE_INVENTORY_FLAG = "restoreInventoryFlag";
		/// <summary>楽天スーパーDEAL商品フラグ</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_DEAL_FLAG = "dealFlag";
		/// <summary>医薬品フラグ</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_DRUG_FLAG = "drugFlag";
		/// <summary>商品削除フラグ</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_DELETE_ITEM_FLAG = "deleteItemFlag";
		/// <summary>商品税率</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_TAX_RATE = "taxRate";
		/// <summary>商品毎税込価格</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_PRICE_TAX_INCL = "priceTaxIncl";
		/// <summary>単品配送フラグ</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_IS_SINGLE_ITEM_SHIPPING = "isSingleItemShipping";
		/// <summary>単品配送フラグ</summary>
		public const string RAKUTEN_PAY_API_ITEM_MODEL_SKU_MODEL_LIST = "SkuModelList";
		/// <summary>発送明細ID</summary>
		public const string RAKUTEN_PAY_API_SHIPPING_MODEL_SHIPPING_DETAIL_ID = "shippingDetailId";
		/// <summary>お荷物伝票番号	</summary>
		public const string RAKUTEN_PAY_API_SHIPPING_MODEL_SHIPPING_NUMBER = "shippingNumber";
		/// <summary>配送会社</summary>
		public const string RAKUTEN_PAY_API_SHIPPING_MODEL_DELIVERY_COMPANY = "deliveryCompany";
		/// <summary>配送会社名</summary>
		public const string RAKUTEN_PAY_API_SHIPPING_MODEL_DELIVERY_COMPANY_NAME = "deliveryCompanyName";
		/// <summary>発送日</summary>
		public const string RAKUTEN_PAY_API_SHIPPING_MODEL_SHIPPING_DATE = "shippingDate";
		/// <summary>クーポンコード</summary>
		public const string RAKUTEN_PAY_API_COUPON_MODEL_COUPON_CODE = "couponCode";
		/// <summary>クーポン対象の商品ID</summary>
		public const string RAKUTEN_PAY_API_COUPON_MODEL_ITEM_ID = "itemId";
		/// <summary>クーポン名</summary>
		public const string RAKUTEN_PAY_API_COUPON_MODEL_COUPON_NAME = "couponName";
		/// <summary>クーポン効果(サマリー)</summary>
		public const string RAKUTEN_PAY_API_COUPON_MODEL_COUPON_SUMMARY = "couponSummary";
		/// <summary>クーポン原資</summary>
		public const string RAKUTEN_PAY_API_COUPON_MODEL_COUPON_CAPITAL = "couponCapital";
		/// <summary>クーポン原資コード</summary>
		public const string RAKUTEN_PAY_API_COUPON_MODEL_COUPON_CAPITAL_CODE = "couponCapitalCode";
		/// <summary>有効期限</summary>
		public const string RAKUTEN_PAY_API_COUPON_MODEL_EXPIRY_DATE = "expiryDate";
		/// <summary>クーポン割引単価</summary>
		public const string RAKUTEN_PAY_API_COUPON_MODEL_COUPON_PRICE = "couponPrice";
		/// <summary>クーポン利用数</summary>
		public const string RAKUTEN_PAY_API_COUPON_MODEL_COUPON_UNIT = "couponUnit";
		/// <summary>クーポン利用金額</summary>
		public const string RAKUTEN_PAY_API_COUPON_MODEL_COUPON_TOTAL_PRICE = "couponTotalPrice";
		/// <summary>税率モデルリスト</summary>
		public const string RAKUTEN_PAY_API_ORDER_MODEL_TAX_SUMMARY_MODEL_LIST = "TaxSummaryModelList";
		/// <summary>税率</summary>
		public const string RAKUTEN_PAY_API_TAX_SUMMARY_MODEL_TAX_RATE = "taxRate";
		/// <summary>請求金額</summary>
		public const string RAKUTEN_PAY_API_TAX_SUMMARY_MODEL_REQ_PRICE = "reqPrice";
		/// <summary>請求額に対する税額</summary>
		public const string RAKUTEN_PAY_API_TAX_SUMMARY_MODEL_REQ_PRICE_TAX = "reqPriceTax";
		/// <summary>合計金額</summary>
		public const string RAKUTEN_PAY_API_TAX_SUMMARY_MODEL_TOTAL_PRICE = "totalPrice";
		/// <summary>決済手数料</summary>
		public const string RAKUTEN_PAY_API_TAX_SUMMARY_MODEL_PATMENT_CHARGE = "paymentCharge";
		/// <summary>クーポン割引額</summary>
		public const string RAKUTEN_PAY_API_TAX_SUMMARY_MODEL_COUPON_PRICE = "couponPrice";
		/// <summary>利用ポイント数</summary>
		public const string RAKUTEN_PAY_API_TAX_SUMMARY_MODEL_POINT = "point";

		// 定期注文：今すぐ注文の際、次回配送日変更チェックボックスのデフォルト値
		public static bool FIXEDPURCHASEORDERNOW_NEXT_SHIPPING_DATE_UPDATE_DEFAULT = true;
		/// <summary>定期注文：次回配送日変更の際、次々回配送日変更チェックボックスのデフォルト値</summary>
		public static bool FIXEDPURCHASEORDERNOW_NEXT_NEXT_SHIPPING_DATE_UPDATE_DEFAULT = false;
		/// <summary>為替レートのキャッシュ有効期限（分）</summary>
		public static int EXCHANGERATE_EXPIRE_MINUTES = 0;
		/// <summary>LPカート注文完了時会員登録</summary>
		public static bool LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE = false;

		//========================================================================		
		// クーポンBOX設定
		//========================================================================		
		/// <summary>クーポンBOX：有効期限切れ何か日経過のものまで表示するか</summary>
		public static int COUPONBOX_DISPLAY_PASSED_DAYS_FROM_EXPIREDATE = 0;

		/// <summary>
		/// メール便から宅配便へのエスカレーション個数
		/// 1を設定した場合、2以上で宅配便にエスカレーションされる
		/// </summary>
		public static int MAIL_ESCALATION_COUNT = 0;
		/// <summary>メール便で出荷予定日にリードタイムを考慮する</summary>
		public static bool MAIL_SHIPPINGDATE_INCLUDE_LEADTIME = false;

		/// <summary> 配送方法自動判定(配送サイズ係数)：定期注文配送方法切り替わりフラグの注文拡張ステータス番号 </summary>
		public static string MAIL_ESCALATION_TO_EXPRESS_ORDEREXTENDNO = string.Empty;

		/// <summary>メール便配送サービスエスカレーション機能</summary>
		public static bool DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED = false;
		/// <summary> メール便配送サービスエスカレーション機能：定期注文配送サービス切り替わりフラグの注文拡張ステータス番号 </summary>
		public static string DELIVERYCOMPANY_MAIL_ESCALATION_ORDEREXTENDNO = string.Empty;
		
		//========================================================================
		// ソーシャルログイン共通
		//========================================================================
		/// <summary>ソーシャルログイン共通：有効無効</summary>
		public static bool COMMON_SOCIAL_LOGIN_ENABLED = false;
		//========================================================================
		// ソーシャルプラス設定
		//========================================================================
		/// <summary>ソーシャルプラス：有効無効</summary>
		public static bool SOCIAL_LOGIN_ENABLED = false;
		/// <summary>ソーシャルプラス：HTTPSプロコトル</summary>
		public static string SOCIAL_LOGIN_PROTOCOL_HTTPS = "https://";
		/// <summary>ソーシャルプラス：FQDN</summary>
		public static string SOCIAL_LOGIN_FQDN = null;
		/// <summary>ソーシャルプラス：AppleのFQDN</summary>
		public static string SOCIAL_LOGIN_FQDN_APPLE = null;
		/// <summary>ソーシャルプラス：APIキー</summary>
		public static string SOCIAL_LOGIN_API_KEY = null;
		/// <summary>ソーシャルプラス：アカウントID</summary>
		public static string SOCIAL_LOGIN_ACCOUNT_ID = null;
		/// <summary>ソーシャルプラス：サイトID</summary>
		public static string SOCIAL_LOGIN_SITE_ID = null;
		/// <summary>ソーシャルプラス：WebAPIタイムアウト時間(ms)</summary>
		public static int SOCIAL_LOGIN_WEBAPI_TIMEOUT = 0;

		//========================================================================
		// ソーシャルプロバイダID連携
		//========================================================================
		/// <summary>ソーシャルプロバイダID連携：Facebook ID格納用ユーザー拡張項目名</summary>
		public static string SOCIAL_PROVIDER_ID_FACEBOOK = "";
		/// <summary>ソーシャルプロバイダID連携：LINE ID格納用ユーザー拡張項目名</summary>
		public static string SOCIAL_PROVIDER_ID_LINE = "";
		/// <summary>ソーシャルプロバイダID連携：Twitter ID格納用ユーザー拡張項目名</summary>
		public static string SOCIAL_PROVIDER_ID_TWITTER = "";
		/// <summary>ソーシャルプロバイダID連携：Yahoo ID格納用ユーザー拡張項目</summary>
		public static string SOCIAL_PROVIDER_ID_YAHOO = "";
		/// <summary>ソーシャルプロバイダID連携：Google ID格納用ユーザー拡張項目</summary>
		public static string SOCIAL_PROVIDER_ID_GOOGLE = "";
		/// <summary>ソーシャルプロバイダID連携：APPLE ID格納用ユーザー拡張項目</summary>
		public static string SOCIAL_PROVIDER_ID_APPLE = "";
		/// <summary>ソーシャルプロバイダID連携：ユーザー拡張項目名リスト</summary>
		public static string[] SOCIAL_PROVIDER_ID_USEREXTEND_COLUMN_NAMES = new string[0];

		// 注文同梱系
		/// <summary>注文同梱を許可する決済種別(フロント)</summary>
		public static string[] ORDERCOMBINE_ALLOW_PAYMENT_KBN_FRONT = new string[] { "" };
		/// <summary>注文同梱を許可する注文ステータス(フロント)</summary>
		public static string[] ORDERCOMBINE_ALLOW_ORDER_STATUS_FRONT = new string[] { "" };
		/// <summary>注文日から何日経過まで注文同梱を許可するか(フロント)</summary>
		public static int ORDERCOMBINE_ALLOW_ORDER_DAY_PASSED_FRONT = 0;
		/// <summary>配送希望日の何日前まで注文同梱を許可するか(フロント)</summary>
		public static int ORDERCOMBINE_ALLOW_SHIPPING_DAY_BEFORE_FRONT = 0;
		/// <summary>注文同梱を許可する入金ステータス(フロント)</summary>
		public static string[] ORDERCOMBINE_ALLOW_ORDER_PAYMENT_STATUS_FRONT = new string[] { "" };
		/// <summary>注文同梱を許可する入金ステータス(フロント)</summary>
		public static string[] ORDERCOMBINE_DENY_SHIPPING_METHOD_FRONT = new string[] { "" };
		/// <summary>注文同梱を許可しない配送種別(フロント)</summary>
		public static string[] ORDERCOMBINE_DENY_SHIPPING_ID_FRONT = new string[] { "" };
		/// <summary>注文同梱を許可する決済種別(管理画面)</summary>
		public static string[] ORDERCOMBINE_ALLOW_PAYMENT_KBN_MANAGER = new string[] { "" };
		/// <summary>注文同梱を許可する注文ステータス(管理画面)</summary>
		public static string[] ORDERCOMBINE_ALLOW_ORDER_STATUS_MANAGER = new string[] { "" };
		/// <summary>注文日から何日経過まで注文同梱を許可するか(管理画面)</summary>
		public static int ORDERCOMBINE_ALLOW_ORDER_DAY_PASSED_MANAGER = 0;
		/// <summary>配送希望日の何日前まで注文同梱を許可するか(管理画面)</summary>
		public static int ORDERCOMBINE_ALLOW_SHIPPING_DAY_BEFORE_MANAGER = 0;
		/// <summary>注文同梱を許可する入金ステータス(管理画面)</summary>
		public static string[] ORDERCOMBINE_ALLOW_ORDER_PAYMENT_STATUS_MANAGER = new string[] { "" };
		/// <summary>注文同梱を許可しない配送方法(管理画面)</summary>
		public static string[] ORDERCOMBINE_DENY_SHIPPING_METHOD_MANAGER = new string[] { "" };
		/// <summary>注文同梱を許可しない配送種別(管理画面)</summary>
		public static string[] ORDERCOMBINE_DENY_SHIPPING_ID_MANAGER = new string[] { "" };
		/// <summary>定期購入同梱を許可する定期購入ステータス(管理画面)</summary>
		public static string[] FIXEDPURCHASECOMBINE_ALLOW_FIXEDPURCHASE_STATUS = new string[] { "" };
		/// <summary>定期購入同梱を許可する決済ステータス(管理画面)</summary>
		public static string[] FIXEDPURCHASECOMBINE_ALLOW_PAYMENT_STATUS = new string[] { "" };

		// キャプチャ認証系
		/// <summary>キャプチャ認証：サイトキー</summary>
		public static string RECAPTCHA_SITE_KEY = "";
		/// <summary>キャプチャ認証：シークレットキー</summary>
		public static string RECAPTCHA_SECRET_KEY = "";

		//========================================================================
		// 2-Click決済
		//========================================================================
		/// <summary>デフォルト注文方法指定フィールド：注文者情報の住所</summary>
		public static string FIELD_USER_DEFAULT_ORDER_SETTING_OWNER_SHIPPING = "user_default_order_setting_owner_shipping";
		/// <summary>デフォルト注文方法指定フィールド：指定なし</summary>
		public static string FIELD_USER_DEFAULT_ORDER_SETTING_NONE = "user_default_order_setting_none";
		/// <summary>デフォルト注文方法指定フィールド値：注文者情報の住所</summary>
		public static string FIELD_USER_DEFAULT_ORDER_SETTING_OWNER_SHIPPING_VALUE = "0";
		/// <summary>デフォルト注文方法指定フィールド値：指定なし</summary>
		public static string FIELD_USER_DEFAULT_ORDER_SETTING_NONE_VALUE = "NONE";

		//------------------------------------------------------
		// 注文系ファイル取込
		//------------------------------------------------------
		/// <summary>注文関連ファイルの非同期取り込み</summary>
		public static bool ORDER_FILE_IMPORT_ASYNC = false;
		/// <summary>注文関連ファイル取込セッティング</summary>
		public const string FILE_XML_ORDERFILEIMPORT_SETTING = "Xml/Setting/OrderFileImportSetting.xml";
		/// <summary> e-cat2000 e-cat紐付けデータ</summary>
		public const string KBN_ORDERFILE_ECAT2000LINK = "ECAT2000LINK";
		/// <summary> w2Commerce標準配送伝票紐付けデータ</summary>
		public const string KBN_ORDERFILE_SHIPPING_NO_LINK = "SHIPPING_NO_LINK";
		/// <summary>B2配送伝票紐付けデータ（楽天注文含む）</summary>
		public const string KBN_B2_RAKUTEN_INCL_LINK = "B2_RAKUTEN_INCL_LINK";
		/// <summary>B2配送伝票紐付けデータ（楽天注文含む）（B2クラウド用）</summary>
		public const string KBN_B2_RAKUTEN_INCL_LINK_CLOUD = "B2_RAKUTEN_INCL_LINK_CLOUD";
		/// <summary>入金データ</summary>
		public const string KBN_ORDERFILE_PAYMENT_DATA = "PAYMENT_DATA";
		/// <summary>注文データ</summary>
		public const string KBN_ORDERFILE_IMPORT_ORDER = "IMPORT_ORDER";
		/// <summary>拡張ステータス更新</summary>
		public const string KBN_ORDERFILE_ORDER_EXTEND = "OrderExtend";
		/// <summary>返品受付</summary>
		public const string KBN_ORDERFILE_RETURNS_ACCEPTED = "ReturnsAccepted";
		/// <summary>定期解約</summary>
		public const string KBN_ORDERFILE_CANCEL_FIXEDPURCHASE = "FixedPurchaseExtend";
		/// <summary>税率毎価格情報データ</summary>
		public const string KBN_ORDERFILE_IMPORT_ORDER_PRICE_BY_TAX_RATE = "IMPORT_ORDER_PRICE_BY_TAX_RATE";
		// 2回目未入金者取込
		public const string KBN_ORDERFILE_IMPORT_ORDER_SECOND_TIME_NON_DEPOSIT = "IMPORT_ORDER_SECOND_TIME_NON_DEPOSIT";
		// (DSK）入金データ取込
		public const string KBN_ORDERFILE_IMPORT_PAYMENT_DEPOSIT_DSK = "IMPORT_PAYMENT_DEPOSIT_DSK";
		/// <summary>Shipping data</summary>
		public const string KBN_ORDERFILE_SHIPPING_DATA = "SHIPPING_DATA";
		/// <summary> ウケトル配送伝票紐付けデータ</summary>
		public const string KBN_ORDERFILE_UKETORU_LINK = "UKETORU_LINK";
		/// <summary>宅配通 配送実績紐付けデータ</summary>
		public const string KBN_ORDERFILE_PELICAN_RESULT_REPORT_LINK = "PELICAN_RESULT_REPORT_LINK";
		/// <summary>Kbn orderfile import order migration</summary>
		public const string KBN_ORDERFILE_IMPORT_ORDER_MIGRATION = "IMPORT_ORDER:migration";
		/// <summary>注文ステータス更新</summary>
		public const string KBN_ORDERFILE_IMPORT_ORDER_STATUS = "IMPORT_ORDER_STATUS";

		//------------------------------------------------------
		// Amazon連携
		//------------------------------------------------------
		/// <summary>Amazon注文ステータス：Pending(保留)</summary>
		public const string AMAZON_MALL_ORDER_STATUS_PENDING = "Pending";
		/// <summary>Amazon注文ステータス：Canceled(キャンセル)</summary>
		public const string AMAZON_MALL_ORDER_STATUS_CANCELED = "Canceled";
		/// <summary>Amazon注文ステータス：Unshipped(未出荷)</summary>
		public const string AMAZON_MALL_ORDER_STATUS_UNSHIPPED = "Unshipped";
		/// <summary>Amazon注文ステータス：PartiallyShipped(一部出荷済み)</summary>
		public const string AMAZON_MALL_ORDER_STATUS_PARTIALLY_SHIPPED = "PartiallyShipped";
		/// <summary>出荷方法：MFN(出品者が出荷)</summary>
		public const string AMAZON_MALL_FULFILMENT_CHANNEL_MFN = "MFN";
		/// <summary>フィードタイプ：出荷通知</summary>
		public const string AMAZON_MALL_FEED_TYPE_ORDER_FULFILLMENT = "_POST_ORDER_FULFILLMENT_DATA_";
		/// <summary>フィードタイプ：在庫</summary>
		public const string AMAZON_MALL_FEED_TYPE_INVENTORY_AVAILABILITY = "_POST_INVENTORY_AVAILABILITY_DATA_";
		/// <summary>Amazon MWSエンドポイント</summary>
		public static string AMAZON_MALL_MWS_ENDPOINT = "";

		//------------------------------------------------------
		// フォーム名系
		//------------------------------------------------------
		/// <summary>定期購入フィルタラジオボタン</summary>
		public const string FORM_NAME_FIXED_PURCHASE_RADIO_BUTTON = "fpfl";
		/// <summary>セール対象チェックボックスボタン</summary>
		public const string FORM_NAME_SALEFILTER_CHECKBOX = "sfl";

		//========================================================================
		// 為替レート取得バッチ
		//========================================================================
		/// <summary>為替レート取得区分</summary>
		public static ExchangeRateKbn? EXCHANGERATE_KBN = null;
		public enum ExchangeRateKbn
		{
			/// <summary>CurrencyLayer</summary>
			CurrencyLayer,
			/// <summary>CurrencyConverter</summary>
			CurrencyConverter,
			/// <summary>ExchangeRate</summary>
			ExchangeRate,
		}
		/// <summary>WebAPI接続先URL</summary>
		public static string EXCHANGERATE_BASEURL = null;
		/// <summary>アクセスキー</summary>
		public static string EXCHANGERATE_ACCESSKEY = null;
		/// <summary>通貨コード（元）</summary>
		public static string[] EXCHANGERATE_SRCCURRENCYCODES = new string[] { "" };
		/// <summary>通貨コード（先）</summary>
		public static string[] EXCHANGERATE_DSTCURRENCYCODES = new string[] { "" };
		/// <summary>為替レート取得データ保存先</summary>
		public static string EXCHANGERATE_SAVEDIRECTORYPATH = null;
		/// <summary>為替レート取得データ保存ファイル名</summary>
		public static string EXCHANGERATE_SAVEFILENAME = null;
		/// <summary>為替レート取得データ保存ファイル名の日付形式</summary>
		public static string EXCHANGERATE_SAVEFILENAME_DATEFORMAT = null;

		/// <summary>表示HTML区分利用カラム</summary>
		public static List<string> UseDisplayKbnColumn = new List<string>
		{
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_OUTLINE,
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL1,
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL2,
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL3,
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_DESC_DETAIL4,
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SETPROMOTION_DESCRIPTION,
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTSET_DESCRIPTION,
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_USEREXTENDSETTING_OUTLINE,
			Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_NEWS_NEWS_TEXT,
		};

		//------------------------------------------------------
		// 自動翻訳API 区分
		//------------------------------------------------------
		/// <summary>自動翻訳API区分:Google</summary>
		public const string AUTO_TRANSLATION_API_KBN_GOOGLE = "Google";

		/// <summary>メールテンプレートタグ：領収書出力ＵＲＬ</summary>
		public const string TAG_RECEIPT_URL = "receipt_url";

		/// <summary>Tag fixed purchase memo</summary>
		public static string TAG_FIXED_PURCHASE_MEMO = "fixed_purchase_memo";

		/// <summary>Order search max length url</summary>
		public static int ORDER_SEARCH_MAX_LENGTH_URL = 0;

		/// <summary>ブランド情報をセッションに持つか</summary>
		public static bool BRAND_SESSION_ENABLED = false;

		/// <summary>ブランド未指定時、デフォルトブランドが指定されている場合、デフォルトブランドのトップページに遷移するか</summary>
		public static bool REDIRECT_TO_DEFAULT_BRAND_TOP = false;

		// オーダーワークフロー実行スレッド(ValueTextのキー)
		public const string VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD = "OrderWorkflowExecThread";

		// オーダーワークフロー実行スレッド(ログタイトル)
		public const string VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_LOG_TITLE = "log_title";
		// オーダーワークフロー実行スレッド(タイトル: シナリオ実行)
		public const string VALUE_TEXT_WORKFLOW_THREAD_WRITE_LOG_TITLE_SCENARIO_EXEC = "scenarioExec";
		// オーダーワークフロー実行スレッド(タイトル: シナリオ停止)
		public const string VALUE_TEXT_WORKFLOW_THREAD_WRITE_LOG_TITLE_SCENARIO_STOP = "scenarioStop";
		// オーダーワークフロー実行スレッド(タイトル: シナリオエラー)
		public const string VALUE_TEXT_WORKFLOW_THREAD_WRITE_LOG_TITLE_SCENARIO_ERROR = "scenarioError";

		// オーダーワークフロー実行スレッド(ログステータス)
		public const string VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_LOG_STATUS = "log_status";
		// オーダーワークフロー実行スレッド(ステータス: 実行完了)
		public const string VALUE_TEXT_WORKFLOW_THREAD_WRITE_LOG_STATUS_SCENARIO_EXEC = "scenarioExec";
		// オーダーワークフロー実行スレッド(ステータス: 停止)
		public const string VALUE_TEXT_WORKFLOW_THREAD_WRITE_LOG_STATUS_SCENARIO_STOP = "scenarioStop";
		// オーダーワークフロー実行スレッド(ステータス: エラー)
		public const string VALUE_TEXT_WORKFLOW_THREAD_WRITE_LOG_STATUS_SCENARIO_ERROR = "scenarioError";

		/// <summary>シナリオ実行時のメールテンプレート用(メールタイプ)</summary>
		public const string MAILTEMPLATE_KEY_MAIL_TYPE = "mail_type";
		/// <summary>シナリオ実行時のメールテンプレート用(メールタイプ-開始)</summary>
		public const string VALUE_TEXT_IMPORT_MAILTEMPLATE_KEY_MAIL_TYPE_START = "start";
		/// <summary>シナリオ実行時のメールテンプレート用(メールタイプ-終了)</summary>
		public const string VALUE_TEXT_IMPORT_MAILTEMPLATE_KEY_MAIL_TYPE_END = "end";

		// オーダーワークフロー実行スレッド(メールフォーマット)
		public const string VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT = "mail_format";
		// オーダーワークフロー実行スレッド(メールフォーマット: 実行ステータス)
		public const string VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT_EXEC_STATUS = "execStatus";
		// オーダーワークフロー実行スレッド(メールフォーマット: 実行タイミング)
		public const string VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT_EXEC_TIMING = "execTiming";
		// オーダーワークフロー実行スレッド(メールフォーマット: 進捗)
		public const string VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT_PROGRESS = "progress";
		// オーダーワークフロー実行スレッド(メールフォーマット: 履歴詳細URL)
		public const string VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT_HISTORY_DETAILS_URL = "historyDetailsUrl";
		// オーダーワークフロー実行スレッド(メールフォーマット: 開始時間)
		public const string VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT_START_TIME = "startTime";
		// オーダーワークフロー実行スレッド(メールフォーマット: 終了時間)
		public const string VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT_END_TIME = "endTime";

		// オーダーワークフロー実行スレッド(ログフォーマット)
		public const string VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_LOG_FORMAT = "log_format";
		// オーダーワークフロー実行スレッド(ログフォーマット: シナリオ実行)
		public const string VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_LOG_FORMAT_EXEC_START = "execStart";

		/// <summary>Order payment memo for invoice reissue</summary>
		public const string VALUETEXT_PAYMENT_MEMO_CVSDEF_INVOICE_REISSUE = "payment_memo_cvsdef_invoice_reissue";
		/// <summary>Order payment settlement transaction id</summary>
		public const string VALUETEXT_ORDER_SETTLEMENT_TRANSACTION_ID = "settlement_transaction_id";
		/// <summary>Order payment invoice reissue</summary>
		public const string VALUETEXT_ORDER_INVOICE_REISSUE = "invoice_reissue";

		/// <summary>シナリオ実行時のメールテンプレート用(本文)</summary>
		public const string MAILTEMPLATE_KEY_SCENARIO_MESSAGE = "message";
		/// <summary>シナリオ実行時のメールテンプレート用(シナリオ名)</summary>
		public const string MAILTEMPLATE_KEY_SCENARIO_NAME = "scenario_name";
		/// <summary>シナリオ実行時のメールテンプレート用(実行ステータス)</summary>
		public const string MAILTEMPLATE_KEY_EXEC_STATUS = "exec_status";

		//------------------------------------------------------
		// ログ用のキー値
		//------------------------------------------------------
		/// <summary>トラッキングID</summary>
		public const string FOR_LOG_KEY_TRACKING_ID = "tracking_id";
		/// <summary>枝番</summary>
		public const string FOR_LOG_KEY_BRANCH_NO = "branch_no";
		/// <summary>AmazonPayで利用する注文ID</summary>
		public const string FOR_LOG_KEY_AMAZON_ORDER_REFERENCE_ID = "amazon_order_reference_id";
		/// <summary>PayPal向けのCustomerId</summary>
		public const string FOR_LOG_KEY_FOR_PAYPAL_CUSTOMER_ID = "for_paypal_customer_id";
		/// <summary>取引ID(主にGMOで使用)</summary>
		public const string FOR_LOG_KEY_FOR_GMO_ACCESS_ID = "for_gmo_access_id";
		/// <summary>GMO向け会員ID</summary>
		public const string FOR_LOG_KEY_FOR_GMO_MEMBER_ID = "for_gmo_member_id";
		/// <summary>決済種別ID</summary>
		public const string FOR_LOG_KEY_PAYMENT_ID = "payment_id";
		/// <summary>ResResult</summary>
		public const string FOR_LOG_KEY_RES_RESULT = "res_result";
		/// <summary>PayInfoKey</summary>
		public const string FOR_LOG_KEY_PAY_INFO_KEY = "pay_info_key";
		/// <summary>ResponseMessage</summary>
		public const string FOR_LOG_KEY_RESPONSE_MESSAGE = "response_message";
		/// <summary>パフォーマンス：SEO全体設定キャッシング時間（分。「0」はキャッシングしない）</summary>
		public static int SEO_ENTIRE_SETTING_CACHE_EXPIRE_MINUTES = 0;
		/// <summary>パフォーマンス：OGP全体設定キャッシング時間（分。「0」はキャッシングしない）</summary>
		public static int OGP_ENTIRE_SETTING_CACHE_EXPIRE_MINUTES = 0;

		// Paidy決済設定
		/// <summary>Paidyオプション（ログイン＆決済）</summary>
		public static bool PAYMENT_PAIDY_OPTION_ENABLED = false;
		/// <summary>決済区分：Paidy</summary>
		public static PaymentPaidyKbn? PAYMENT_PAIDY_KBN = null;
		/// <summary>Paidyテスト環境</summary>
		public static string PAYMENT_PAIDY_API_KEY = string.Empty;
		/// <summary>Paidyテスト環境</summary>
		public static string PAYMENT_PAIDY_SECRET_KEY = string.Empty;
		/// <summary>決済：Paidyペイメント：接続URL</summary>
		public static string PAYMENT_PAIDY_API_URL = string.Empty;
		/// <summary>決済：Paidyペイメント：版</summary>
		public static string PAYMENT_PAIDY_API_VERSION = string.Empty;
		/// <summary>決済：Paidyペイメント：商品番号</summary>
		public static string PAYMENT_PAIDY_CHECKOUT_REQUEST_ITEMS_ID = string.Empty;
		/// <summary>決済：Paidyペイメント：商品名</summary>
		public static string PAYMENT_PAIDY_CHECKOUT_REQUEST_ITEMS_TITLE = string.Empty;
		/// <summary>Paidy(Paygent)テスト環境</summary>
		public static string PAYMENT_PAYGENT_PAIDY_API_KEY = string.Empty;
		/// <summary>Paidy(Paygent)テスト環境</summary>
		public static string PAYMENT_PAYGENT_PAIDY_SECRET_KEY = string.Empty;

		//------------------------------------------------------
		// 翻訳ツール用値
		//------------------------------------------------------
		/// <summary>付きタグ先頭</summary>
		public static string FOR_TRANSLACTION_TAG_START = "<w2_tag_for_translaction>";
		/// <summary>付きタグ末尾</summary>
		public static string FOR_TRANSLACTION_TAG_END = "</w2_tag_for_translaction>";

		//------------------------------------------------------
		// 電子発票
		//------------------------------------------------------
		/// <summary>電子発票連携オプション</summary>
		public static bool TWINVOICE_ENABLED = false;
		/// <summary>OPTION：台湾電子発票API連携用統一編号</summary>
		public static string TWINVOICE_UNIFORM_ID = string.Empty;
		/// <summary>OPTION：台湾電子発票API連携用アカウント</summary>
		public static string TWINVOICE_UNIFORM_ACCOUNT = string.Empty;
		/// <summary>OPTION：台湾電子発票API連携用パスワード</summary>
		public static string TWINVOICE_UNIFORM_PASSWORD = string.Empty;
		/// <summary>OPTION：台湾電子発票番号取得日</summary>
		public static int TWINVOICE_GET_DATE = 0;
		/// <summary>OPTION：台湾電子発票番号取得枚数</summary>
		public static int TWINVOICE_GET_COUNT = 0;
		/// <summary>Invoice Url</summary>
		public static string TWINVOICE_URL = string.Empty;
		/// <summary>Invoice order invoicing</summary>
		public static bool TWINVOICE_ORDER_INVOICING = false;
		/// <summary>Regular Expression: Mobile Carry Type Option 8</summary>
		public const string REGEX_MOBILE_CARRY_TYPE_OPTION_8 = @"^/[A-Z|0-9|+-.]{7}$";
		/// <summary>Regular Expression: Certificate Carry Type Option 16</summary>
		public const string REGEX_CERTIFICATE_CARRY_TYPE_OPTION_16 = @"^[A-Z]{2}[0-9]{14}$";
		/// <summary>Convenience store map: PC URL</summary>
		public static string CONVENIENCESTOREMAP_PC_URL = string.Empty;
		/// <summary>Convenience store map: Smartphone URL</summary>
		public static string CONVENIENCESTOREMAP_SMARTPHONE_URL = string.Empty;
		/// <summary>Convenience store map: cvsname</summary>
		public static string RECEIVINGSTORE_TWPELICAN_CVSNAME = string.Empty;
		/// <summary>Convenience store map: cvstemp</summary>
		public static string CONVENIENCESTOREMAP_CVSTEMP = string.Empty;
		/// <summary>Convenience store map: cvsid</summary>
		public static string RECEIVINGSTORE_TWPELICAN_CVSID = string.Empty;
		/// <summary>Convenience store map: cvsuserip</summary>
		public static string CONVENIENCESTOREMAP_CVSUSERIP = string.Empty;
		/// <summary>Convenience store map: suid key</summary>
		public const string REQUEST_KEY_RECEIVINGSTORE_TWPELICAN_SUID = "suID";
		/// <summary>Convenience store map: processid key</summary>
		public const string REQUEST_KEY_RECEIVINGSTORE_TWPELICAN_PROCESSID = "processID";
		/// <summary>Convenience store map: rturl key</summary>
		public const string REQUEST_KEY_RECEIVINGSTORE_TWPELICAN_RTURL = "rtURL";
		/// <summary>Convenience Store Limit Price</summary>
		public static decimal RECEIVINGSTORE_TWPELICAN_CVSLIMITPRICE = 0;
		/// <summary>Convenience Store Limit Kg</summary>
		public static string[] RECEIVINGSTORE_TWPELICAN_CVSLIMITKG = new string[0];
		/// <summary>Receiving Store Twpelican Cvs Shipping Date</summary>
		public static string[] RECEIVING_STORE_TWPELICAN_CVS_SHIPPING_DATE = new string[0];
		/// <summary>コンビニ有効チェック：XMLファイルを使ってチェックする</summary>
		public static string CONVENIENCESTOREMAP_FILE_TWPELICANALLCVS = "TwPelicanAllCvs.xml";
		/// <summary>Service Shipping Convenience Store ID</summary>
		public static string TWPELICANEXPRESS_CONVENIENCE_STORE_ID = string.Empty;
		/// <summary>OPTION：台湾コンビニ受取</summary>
		public static bool RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED = false;
		/// <summary>OPTION：台湾電子発票PDF形式（1:感熱紙形式、2:A4形式、3:A5形式）</summary>
		public static int TWINVOICE_PDF_FORMAT = 0;
		/// <summary>台湾電子発票：2月</summary>
		public static int TWINVOICE_MONTH_2 = 2;
		/// <summary>台湾電子発票：4月</summary>
		public static int TWINVOICE_MONTH_4 = 4;
		/// <summary>台湾電子発票：6月</summary>
		public static int TWINVOICE_MONTH_6 = 6;
		/// <summary>台湾電子発票：8月</summary>
		public static int TWINVOICE_MONTH_8 = 8;
		/// <summary>台湾電子発票：10月</summary>
		public static int TWINVOICE_MONTH_10 = 10;
		/// <summary>台湾電子発票：12月</summary>
		public static int TWINVOICE_MONTH_12 = 12;
		/// <summary>台湾電子発票：最大期別</summary>
		public static int TWINVOICE_MAX_PERIOD = 6;
		/// <summary>台湾電子発票：デフォルト期別</summary>
		public static int TWINVOICE_PERIOD_DEFAULT = 0;
		/// <summary>コンビニの種別</summary>
		public static string RECEIVING_STORE_STORE_KEY_CATEGORY = "stCate";
		/// <summary>コンビニ支店の番号</summary>
		public static string RECEIVING_STORE_STORE_KEY_CODE = "stCode";
		/// <summary>コンビニ支店の名前</summary>
		public static string RECEIVING_STORE_STORE_KEY_NAME = "stName";
		/// <summary>コンビニ支店の住所</summary>
		public static string RECEIVING_STORE_STORE_KEY_ADDR = "stAddr";
		/// <summary>コンビニ支店の電話</summary>
		public static string RECEIVING_STORE_STORE_KEY_TEL = "stTel";

		/// <summary>TwPelicanExpress: Host</summary>
		public static string TWPELICANEXPRESS_FTP_HOST = string.Empty;
		/// <summary>TwPelicanExpress: User name</summary>
		public static string TWPELICANEXPRESS_FTP_ID = string.Empty;
		/// <summary>TwPelicanExpress: Password</summary>
		public static string TWPELICANEXPRESS_FTP_PW = string.Empty;
		/// <summary>TwPelicanExpress: Port</summary>
		public static int TWPELICANEXPRESS_FTP_PORT = 0;
		/// <summary>TwPelicanExpress: Enable SSL</summary>
		public static bool TWPELICANEXPRESS_FTP_ENABLE_SSL = false;
		/// <summary>ペリカンペリカンクライアント(顧客番号,会社名)</summary>
		public static string TWPELICANEXPRESS_CLIENT = string.Empty;
		/// <summary>OPTION：宅配通連携拡張オプション</summary>
		public static bool TWPELICAN_COOPERATION_EXTEND_ENABLED = false;

		/// <summary>Receiving store mailsend method</summary>
		public static string RECEIVINGSTORE_TWPELICAN_MAILSENDMETHOD = string.Empty;
		/// <summary>Receiving store mail order extend no</summary>
		public static string RECEIVINGSTORE_TWPELICAN_MAILORDEREXTENDNO = string.Empty;

		/// <summary>atoneオプション（ログイン＆決済）</summary>
		public static bool PAYMENT_ATONEOPTION_ENABLED = false;
		/// <summary>Atone Apikey</summary>
		public static string PAYMENT_ATONE_APIKEY = string.Empty;
		/// <summary>Atone SecretKey</summary>
		public static string PAYMENT_ATONE_SECRET_KEY = string.Empty;
		/// <summary>Atone Api Url</summary>
		public static string PAYMENT_ATONE_API_URL = string.Empty;
		/// <summary>Atone Script Url</summary>
		public static string PAYMENT_ATONE_SCRIPT_URL = string.Empty;
		/// <summary>Atone User Extend ColumnName</summary>
		public static string ATONE_USEREXTENDCOLUMNNAME_TOKENID = string.Empty;
		/// <summary>Payment Atone Terminal Id</summary>
		public static string PAYMENT_ATONE_TERMINALID = string.Empty;
		/// <summary>Payment Atone Temporary Registration Enabled</summary>
		public static bool PAYMENT_ATONE_TEMPORARYREGISTRATION_ENABLED = false;
		/// <summary>Payment Atone Update Amount Enabled</summary>
		public static bool PAYMENT_ATONE_UPDATEAMOUNT_ENABLED = false;

		/// <summary>afteeオプション（ログイン＆決済）</summary>
		public static bool PAYMENT_AFTEEOPTION_ENABLED = false;
		/// <summary>Aftee Apikey</summary>
		public static string PAYMENT_AFTEE_APIKEY = string.Empty;
		/// <summary>Aftee SecretKey</summary>
		public static string PAYMENT_AFTEE_SECRET_KEY = string.Empty;
		/// <summary>Aftee Api Url</summary>
		public static string PAYMENT_AFTEE_API_URL = string.Empty;
		/// <summary>Aftee Script Url</summary>
		public static string PAYMENT_AFTEE_SCRIPT_URL = string.Empty;
		/// <summary>Aftee User Extend ColumnName</summary>
		public static string AFTEE_USEREXTENDCOLUMNNAME_TOKENID = string.Empty;
		/// <summary>Payment Aftee Terminal Id</summary>
		public static string PAYMENT_AFTEE_TERMINALID = string.Empty;
		/// <summary>Payment Aftee Temporary Registration Enabled</summary>
		public static bool PAYMENT_AFTEE_TEMPORARYREGISTRATION_ENABLED = false;
		/// <summary>Payment Aftee Update Amount Enabled</summary>
		public static bool PAYMENT_AFTEE_UPDATEAMOUNT_ENABLED = false;
		/// <summary>Card Tran Id Option Enabled</summary>
		public static bool CARDTRANIDOPTION_ENABLED = false;

		/// <summary>LINE Payオプション（決済）</summary>
		public static bool PAYMENT_LINEPAY_OPTION_ENABLED = false;
		/// <summary>決済：LINE Pay：LINE Payと通信を行う際のチャンネルID</summary>
		public static string PAYMENT_LINEPAY_CHANNEL_ID = string.Empty;
		/// <summary>決済：LINE Pay：LINE Payと通信を行う際のシークレットキー</summary>
		public static string PAYMENT_LINEPAY_SECRET_KEY = string.Empty;
		/// <summary>決済：LINE Pay：API接続URL</summary>
		public static string PAYMENT_LINEPAY_API_URL = string.Empty;
		/// <summary>決済： LINE Pay：加盟店の国家（JP/TW/TH）</summary>
		public static string PAYMENT_LINEPAY_MERCHANT_COUNTRY = string.Empty;
		/// <summary>LINE Pay 自動決済キー格納用ユーザー拡張項目名(例：usrex_linepay_reg_key)</summary>
		public static string LINEPAY_USEREXRTEND_COLUMNNAME_REGKEY = string.Empty;
		/// <summary>決済：LINE Pay：即時売上かどうか TRUE:即時売上 FALSE:仮売上げ ※LINE Pay加盟店の国家（JP/TW/TH）がJP以外の場合はTRUEに設定</summary>
		public static bool PAYMENT_LINEPAY_PAYMENTCAPTURENOW = false;
		/// <summary>決済：LINE Pay：決済連携後に入金済みにするかどうか</summary>
		public static bool PAYMENT_LINEPAY_PAYMENTSTATUSCOMPLETE = false;

		/// <summary>Logstash連携：API接続URL</summary>
		public static string LOGSTASH_API_URL = string.Empty;

		// NP後払い決済設定
		/// <summary>NP後払いオプション（決済）</summary>
		public static bool PAYMENT_NP_AFTERPAY_OPTION_ENABLED = false;
		/// <summary>NP後払い：加盟店コード</summary>
		public static string PAYMENT_NP_AFTERPAY_MERCHANTCODE = string.Empty;
		/// <summary>NP後払い：SPコード</summary>
		public static string PAYMENT_NP_AFTERPAY_SPCODE = string.Empty;
		/// <summary>NP後払い：ターミナルID</summary>
		public static string PAYMENT_NP_AFTERPAY_TERMINALID = string.Empty;
		/// <summary>NP後払い：API接続URL</summary>
		public static string PAYMENT_NP_AFTERPAY_APIURL = string.Empty;
		/// <summary>決済：NP後払い：請求書同梱</summary>
		public static bool PAYMENT_NP_AFTERPAY_INVOICEBUNDLE = false;
		/// <summary>決済：NP後払い：マニュアルサイト</summary>
		public static string PAYMENT_NP_AFTERPAY_MANUALSITE = string.Empty;
		/// <summary>NP After Pay Custom Error Code: ER001</summary>
		public const string FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_1 = "ER001";
		/// <summary>NP After Pay Custom Error Code: ER002</summary>
		public const string FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_2 = "ER002";
		/// <summary>NP After Pay Custom Error Code: ER003</summary>
		public const string FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_3 = "ER003";
		/// <summary>NP After Pay Custom Error Code: ER004</summary>
		public const string FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_4 = "ER004";
		/// <summary>NP After Pay Custom Error Code: ER005</summary>
		public const string FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_5 = "ER005";
		/// <summary>NP After Pay Custom Error Code: ER005</summary>
		public const string FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_UNEXPECTED_ERROR = "UNEXPECTED_ERROR";

		/// <summary>定期２回目以降商品設定機能のオプション</summary>
		public static bool FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED = false;

		//========================================================================
		// JAFログイン連携
		//========================================================================
		/// <summary>JAFログイン連携：サービスID</summary>
		public static string JAF_SERVICE_ID = "";
		/// <summary>JAFログイン連携：会員ランクID</summary>
		public static string JAF_RANK_ID = "";
		/// <summary>JAFログイン連携：会員ログインURL</summary>
		public static string JAF_LOGIN_API_URL = "";
		/// <summary>JAFログイン連携：会員登録URL</summary>
		public static string JAF_REGISTER_API_URL = "";
		/// <summary>JAFログイン連携：会員情報取得URL</summary>
		public static string JAF_MEMBER_API_URL = "";
		/// <summary>JAFログイン連携：シングルサインオンからの戻りURL</summary>
		public static string JAF_RETURN_URL_FROM_SSO = "";
		/// <summary>JAFログイン連携：暗号化会員番号格納用ユーザー拡張項目名(例：usrex_jaf_encryptknno)</summary>
		public static string JAF_ENCRYPTKNNO_USEREXTEND_COLUMN_NAME = "";
		/// <summary>JAFログイン連携：ステータスコード格納用ユーザー拡張項目名(例：usrex_jaf_status)</summary>
		public static string JAF_STATUS_USEREXTEND_COLUMN_NAME = "";
		/// <summary>JAFログイン連携：エラーコード格納用ユーザー拡張項目名(例：usrex_jaf_error)</summary>
		public static string JAF_ERROR_USEREXTEND_COLUMN_NAME = "";
		/// <summary>JAFログイン連携：ユーザーId</summary>
		public const string JAF_REQUEST_KEY_USER_ID = "uid";

		// ECPayペイメント
		/// <summary>ECPayペイメント：共通：ECPayと通信を行う際の加盟店コード</summary>
		public static string PAYMENT_ECPAY_MERCHANTID = string.Empty;
		/// <summary>ECPayペイメント：共通：特約加盟店の場合</summary>
		public static bool PAYMENT_ECPAY_SPECIAL_MERCHANTFLAG = false;
		/// <summary>ECPayペイメント：共通：暗号化ハッシュキー</summary>
		public static string[] PAYMENT_ECPAY_HASHKEY = { string.Empty };
		/// <summary>ECPayペイメントオプション（コンビニ受取）</summary>
		public static bool RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED = false;
		/// <summary>ECPayペイメント：コンビニ受取：商品発送元名前(会社名ではない)</summary>
		public static string[] RECEIVINGSTORE_TWECPAY_SENDNAME = { string.Empty };
		/// <summary>ECPayペイメント：コンビニ受取：API接続URL</summary>
		public static string RECEIVINGSTORE_TWECPAY_APIURL = string.Empty;
		/// <summary>webhook：テスト用ドメイン、初期値：（ブランク、使用しない）</summary>
		public static string WEBHOOK_DOMAIN = string.Empty;
		/// <summary>Shipping service yamato for EcPay</summary>
		public static string[] SHIPPING_SERVICE_YAMATO_FOR_ECPAY = { string.Empty };
		/// <summary>Shipping service home delivery for EcPay</summary>
		public static string SHIPPING_SERVICE_HOME_DELIVERY_FOR_ECPAY = string.Empty;
		/// <summary>Constant product size limit for EcPay</summary>
		public static decimal CONST_PRODUCT_SIZE_LIMIT = 150;
		/// <summary>ECPayペイメント：コンビニ受取：コンビニ物流方法</summary>
		public static string RECEIVINGSTORE_TWECPAY_CVS_LOGISTIC_METHOD = string.Empty;
		/// <summary>OPTION：ECPayオプション（決済）</summary>
		public static bool ECPAY_PAYMENT_OPTION_ENABLED = false;
		/// <summary>API接続URL：ECPayテスト環境</summary>
		public static string ECPAY_PAYMENT_API_URL = string.Empty;
		/// <summary>決済：ECPayペイメント：分割払い回数</summary>
		public static string ECPAY_PAYMENT_CREDIT_INSTALLMENT = string.Empty;
		/// <summary>webhook：テスト用ドメイン、初期値：（ブランク、使用しない）</summary>

		/// <summary>Secret Key API Line</summary>
		public static string SECRET_KEY_API_LINE = string.Empty;
		/// <summary>LINE APIの認証キー試行回数</summary>
		public static int POSSIBLE_LINE_AUTH_KEY_ERROR_COUNT = 0;
		/// <summary>LINE APIの認証キーロックの有効時間(分単位)</summary>
		public static int POSSIBLE_LINE_AUTH_KEY_LOCK_MINUTES = 0;

		// ValueText
		/// <summary></summary>
		public const string VALUETEXT_PARAM_USER_CONFIRM_CONDITION = "user_confirm_condition";
		/// <summary></summary>
		public const string VALUETEXT_PARAM_RESULT_MESSAGE = "result_message";
		/// <summary></summary>
		public const string VALUETEXT_PARAM_USER_CONFIRM_REGIST = "regist";
		/// <summary></summary>
		public const string VALUETEXT_PARAM_USER_CONFIRM_UPDATE = "update";
		/// <summary></summary>
		public const string VALUETEXT_PARAM_USER_CONFIRM_DELETE = "delete";
		/// <summary>Value text: 注文関連ファイル：注文ステータス設定値</summary>
		public const string VALUETEXT_PARAM_FILE_IMPORT_ORDER_STATUS = "file_import_order_status";
		/// <summary>注文関連ファイル：注文ステータスヘッダー文言</summary>
		public const string VALUETEXT_PARAM_STATUS_DISP_TEXT = "order_status_disp_text";
		/// <summary>Value text: 注文関連ファイル：注文ステータス表示テーブル</summary>
		public const string VALUETEXT_PARAM_STATUS_DISP_NAME = "status_disp_name";

		/// <summary>Value text param order confirm</summary>
		public const string VALUETEXT_PARAM_ORDER_CONFIRM = "order_confirm";
		/// <summary>Value text param payment memo</summary>
		public const string VALUETEXT_PARAM_PAYMENT_MEMO = "payment_memo";
		/// <summary>Value text param sales confirmed</summary>
		public const string VALUETEXT_PARAM_SALES_CONFIRMED = "sales_confirmed";
		/// <summary>Value Text:与信</summary>
		public const string VALUETEXT_PARAM_AUTH = "auth";

		/// <summary>Botchan option</summary>
		public static bool BOTCHAN_OPTION = false;
		/// <summary>Secret key api Botchan</summary>
		public static string SECRET_KEY_API_BOTCHAN = string.Empty;
		/// <summary>Allowed ip address for webcapture</summary>
		public static string ALLOWED_IP_ADDRESS_FOR_BOTCHAN = string.Empty;
		/// <summary>Possible Botchan Login Error Count</summary>
		public static int POSSIBLE_BOTCHAN_LOGIN_ERROR_COUNT = 0;
		/// <summary>Possible Botchan Login Lock Minutes</summary>
		public static int POSSIBLE_BOTCHAN_LOGIN_LOCK_MINUTES = 0;
		/// <summary>BOTCHAN API実行ログ出力先物理パス</summary>
		public static string DIRECTORY_BOTCHAN_API_LOGFILEPATH = string.Empty;
		/// <summary>APIログファイル サイズ閾値（MB）</summary>
		public static int BOTCHAN_API_LOGFILE_THRESHOLD = 100;
		/// <summary>BOTCHAN連携クレカ表示フラグ</summary>
		public const string BOTCHAN_API_CREDIT_REGIST_FLAG_VALID = "1";
		/// <summary>BOTCHAN連携クレカ登録名補完フラグ</summary>
		public const string BOTCHAN_API_CREDIT_NAME_COMPLEMENT_FLAG_VALID = "1";
		/// <summary>BOTCHAN連携クレカ登録名最長文字数</summary>
		public static int BOTCHAN_API_CREDIT_NAME_MAX_LENGTH = 0;
		/// <summary>BOTCHAN RequestParamとResponseをログに出力するかどうか</summary>
		public static bool BOTCHAN_OUTPUT_REQUEST_PARAM_AMD_RESPONSE_TO_THE_LOG_ENABLED = false;
		/// <summary>APIログレベル（INFO:開始終了のみ、ERROR：エラー時詳細出力、TRACE:常に詳細出力）</summary>
		public static string BOTCHAN_API_LOG_LEVEL = "TRACE";
		/// <summary>APIログファイル名 接頭辞</summary>
		public const string BOTCHAN_API_LOGFILE_NAME_PREFIX = "BotchanApi";
		/// <summary>ログファイル文字コード</summary>
		public const string BOTCHAN_LOGFILE_ENCODING = "shift_jis";
		/// <summary>ログレベル 実行ログのみ</summary>
		public const string BOTCHAN_API_FLG_LOG_LEVEL_INFO = "INFO";
		/// <summary>ログレベル エラー時詳細出力</summary>
		public const string BOTCHAN_API_FLG_LOG_LEVEL_ERROR = "ERROR";
		/// <summary>ログレベル 常に詳細出力</summary>
		public const string BOTCHAN_API_FLG_LOG_LEVEL_TRACE = "TRACE";
		/// <summary>ログファイル拡張子</summary>
		public const string BOTCHAN_LOGFILE_EXTENSION = "log";
		/// <summary>BOTCHAN_ログインAPI</summary>
		public const string BOTCHAN_API_NAME_LOGIN = "Login";
		/// <summary>BOTCHAN_再計算API</summary>
		public const string BOTCHAN_API_NAME_RECALCULATION = "Recalculation";
		/// <summary>BOTCHAN_注文登録API</summary>
		public const string BOTCHAN_API_NAME_ORDER_REGISTER = "OrderRegister";
		/// <summary>BOTCHAN_商品検索API</summary>
		public const string BOTCHAN_API_NAME_PRODUCT_SEARCH = "ProductSearch";
		/// <summary>BOTCHAN_会員登録API</summary>
		public const string BOTCHAN_API_NAME_REGISTER = "Register";

		/// <summary>BOTCHAN_API実行成功メッセージID</summary>
		public const string BOTCHAN_API_SUCCESS_MESSAGE_ID = "E00-0000";
		/// <summary>BOTCHAN_API実行成功メッセージ内容</summary>
		public const string BOTCHAN_API_SUCCESS_MESSAGE = "SUCCESS";
		/// <summary>BOTCHAN_商品検索APIの最大検索件数</summary>
		public const int BOTCHAN_LIMIT_PRODUCT_ID_FOR_SEARCH = 10;

		/// <summary>レコメンド情報</summary>
		public const string CONST_RECOMMEND_INFO = "RecommendInfo";
		/// <summary>レコメンド履歴番号</summary>
		public const string CONST_RECOMMEND_HISTORY_NO = "RecommendHistoryNo";

		/// <summary>Gooddeal連携オプション</summary>
		public static bool TWSHIPPING_GOODDEAL_OPTION_ENABLED = false;
		/// <summary>Gooddeal連携EshopId</summary>
		public static string TWSHIPPING_GOODDEAL_ESHOP_ID = string.Empty;
		/// <summary>Gooddeal連携CorporationId</summary>
		public static string TWSHIPPING_GOODDEAL_CORPORATION_ID = string.Empty;
		/// <summary>Gooddeal連携越境決済区分PayType</summary>
		public static string[] TwShipping_Gooddeal_Post_PayType = new [] { string.Empty };
		/// <summary>Gooddeal連携ApiKey</summary>
		public static string TWSHIPPING_GOODDEAL_APIKEY = string.Empty;
		/// <summary>Gooddeal連携HashKey</summary>
		public static string TWSHIPPING_GOODDEAL_HASHKEY = string.Empty;
		/// <summary>Gooddeal受注出荷連携API</summary>
		public static string TWSHIPPING_GOODDEAL_DELIVER_APIURL = string.Empty;
		/// <summary>Gooddeal受注キャンセル連携API</summary>
		public static string TWSHIPPING_GOODDEAL_CANCEL_APIURL = string.Empty;
		/// <summary>Gooddeal配送伝票番号取得API</summary>
		public static string TWSHIPPING_GOODDEAL_GET_SHIPPINGNO_APIURL = string.Empty;

		/// <summary>ECPayオプション（電子発票)</summary>
		public static bool TWINVOICE_ECPAY_ENABLED = false;
		/// <summary>ハッシュキー：ECPay</summary>
		public static string TWINVOICE_ECPAY_HASH_KEY = string.Empty;
		/// <summary>ハッシュIV：ECPay</summary>
		public static string TWINVOICE_ECPAY_HASH_IV = string.Empty;
		/// <summary>ECPay電子発票：API接続URL</summary>
		public static string TWINVOICE_ECPAY_API_URL = string.Empty;
		/// <summary>電子発票： ECPay連携バージョン</summary>
		public static string TWINVOICE_ECPAY_VISION = string.Empty;
		/// <summary>ECPay電子発票字軌種別</summary>
		public static string TWINVOICE_ECPAY_INV_TYPE = string.Empty;
		/// <summary>ECPay電子発票税別</summary>
		public static int TWINVOICE_ECPAY_TAX_TYPE = 0;
		/// <summary>ECPay電子発票通関方法</summary>
		public static string TWINVOICE_ECPAY_CLEARANCE_MARK = string.Empty;
		/// <summary>ECPay電子発票商品単位</summary>
		public static string TWINVOICE_ECPAY_ITEM_WORD = string.Empty;
		/// <summary>ECPay電子発票払い戻し時消費者に通知方法</summary>
		public static string TWINVOICE_ECPAY_ALLOWANCE_NOTIFY = string.Empty;

		/// <summary>定期：マイページから変更可能な決済種別ID（カンマ区切り。Setting_CanFixPurchasePaymentIds内から選択する。空白で非利用となる。）</summary>
		public static string[] SETTING_PARTICULAR_USABLE_FIXEDPURCHASE_PAYMENT_IDS_WHEN_CHANGE_ADDITIONAL_SETTING = new string[] { string.Empty };
		/// <summary>通常：マイページから変更可能な決済種別ID（カンマ区切り。Setting_CanAddOrderPaymentIds内から選択する。空白で非利用となる）</summary>
		public static string[] SETTING_PARTICULAR_USABLE_ORDER_PAYMENT_IDS_WHEN_CHANGE_ADDITIONAL_SETTING = new string[] { string.Empty };
		/// <summary>定期：マイページから変更可能な決済種別IDを絞り込む（カンマ区切り。Setting_CanFixPurchasePaymentIds内から選択する。空白で全て対象となる。）</summary>
		public static string[] SETTING_CAN_CHANGE_FIXEDPURCHASE_PAYMENT_IDS = new string[] { string.Empty };
		/// <summary>定期：マイページから変更可能な決済種別に優先度を付ける（TRUEの場合、変更前の決済種別より優先度の高い決済種別が表示される）</summary>
		public static bool FIXEDPURCHASE_PAYMENT_IDS_PRIORITY_OPTION_ENABLED = false;
		/// <summary>通常：マイページから変更可能な決済種別IDを絞り込む（カンマ区切り。空白で全て対象となる）</summary>
		public static string[] SETTING_CAN_CHANGE_ORDER_PAYMENT_IDS = new string[] { string.Empty };
		/// <summary>通常：マイページから変更可能な決済種別に優先度を付ける（TRUEの場合、変更前の決済種別より優先度の高い決済種別が表示される）</summary>
		public static bool ORDER_PAYMENT_IDS_PRIORITY_OPTION_ENABLED = false;

		// CS問合せ機能向け設定
		/// <summary>グループタスク表示モード</summary>
		public enum GroupTaskDisplayModeType
		{
			/// <summary>所属グループのみ</summary>
			OnlyAssigned,
			/// <summary>グループ未振り分け含む</summary>
			IncludeUnassigned
		}
		/// <summary>CSトップページグループタスク表示モード</summary>
		public static GroupTaskDisplayModeType SETTING_CSTOP_GROUP_TASK_DISPLAY_MODE = GroupTaskDisplayModeType.IncludeUnassigned;

		/// <summary>Value text param: site name</summary>
		public const string VALUETEXT_PARAM_SITENAME = "SiteName";
		/// <summary>Value text param: own site name</summary>
		public const string VALUETEXT_PARAM_OWNSITENAME = "OwnSiteName";
		/// <summary>Value text param: mall id name</summary>
		public const string VALUETEXT_PARAM_MALLIDNAME = "MallIdName";

		/// <summary>Value text param: payment type</summary>
		public const string VALUETEXT_PARAM_PAYMENT_TYPE = "payment_type";

		/// <summary>決済：藍新Payペイメント：藍新Payオプション</summary>
		public static bool NEWEBPAY_PAYMENT_OPTION_ENABLED = false;
		/// <summary>決済：藍新Payペイメント：藍新Payと通信を行う際の加盟店コード</summary>
		public static string NEWEBPAY_PAYMENT_MERCHANTID = string.Empty;
		/// <summary>決済：藍新Payペイメント：暗号化ハッシュキー</summary>
		public static string NEWEBPAY_PAYMENT_HASHKEY = string.Empty;
		/// <summary>決済：藍新Payペイメント：暗号化ハッシュIV</summary>
		public static string NEWEBPAY_PAYMENT_HASHIV = string.Empty;
		/// <summary>API接続URL：藍新Payテスト環境</summary>
		public static string NEWEBPAY_PAYMENT_APIURL = string.Empty;
		/// <summary>決済：藍新Payペイメント：藍新Payにログイン必須フラグ(1:必須、0:必要ない)初期値：0</summary>
		public static int NEWEBPAY_PAYMENT_LOGINTYPE = 0;
		/// <summary>決済：藍新pay：商品名(MAX 50文字)</summary>
		public static string NEWEBPAY_PAYMENT_ITEMNAME = string.Empty;

		/// <summary>NextEngine連携：オプション</summary>
		public static bool NE_OPTION_ENABLED = false;
		/// <summary>NextEngine連携：クライアントID</summary>
		public static string NE_CLIENT_ID = string.Empty;
		/// <summary>NextEngine連携：クライアントシークレット</summary>
		public static string NE_CLIENT_SECRET = string.Empty;
		/// <summary>NextEngine連携：クライアントシークレット</summary>
		public static string NE_SHOP_ID = string.Empty;
		/// <summary>NextEngine連携：備考欄連携項目</summary>
		public static string NE_REMARKS_ENABLED_ITEM = string.Empty;
		/// <summary>NextEngine連携：uploadOrderDataをログに出力するか</summary>
		public static bool NE_WRITE_UPLOAD_ORDER_DATA_TO_LOG = false;
		/// <summary>NextEngine連携：仮注文作成・キャンセル時にネクストエンジン連携するか</summary>
		public static bool NE_REALATION_TEMP_ORDER = false;
		/// <summary>NextEngine連携：仮注文をネクストエンジン連携クラス保存用キー</summary>
		public const string NE_REALATION_TEMP_ORDER_SYNC_OBJECT = "ne_realation_temp_order_sync_object";
		/// <summary>ネクストエンジンキャンセル連携オプション</summary>
		public static bool NE_COOPERATION_CANCEL_ENABLED = false;
		/// <summary>ネクストエンジン注文同梱時連携オプション</summary>
		public static bool NE_COOPERATION_ORDERCOMBINE_ENABLED = false;

		/// <summary>イーロジット連携オプション</summary>
		public static bool ELOGIT_WMS_ENABLED = false;
		/// <summary>イーロジットアップロード連携ApiKey</summary>
		public static string ELOGIT_WMS_UPLOAD_APIKEY = string.Empty;
		/// <summary>イーロジットダウンロード連携ApiKey</summary>
		public static string ELOGIT_WMS_DOWNLOAD_APIKEY = string.Empty;
		/// <summary>イーロジット荷主コード</summary>
		public static string ELOGIT_WMS_CODE = string.Empty;
		/// <summary>イーロジットユーザーID</summary>
		public static string ELOGIT_WMS_USER_ID = string.Empty;
		/// <summary>イーロジットパスワード</summary>
		public static string ELOGIT_WMS_PASSWORD = string.Empty;
		/// <summary>Execution EXE: Wms shipping</summary>
		public static string PHYSICALDIRPATH_WMSSHIPPING_EXE = string.Empty;

		/// <summary>商品一覧：商品バリエーション検索区分画像区分 TRUE：バリエーション表示名１でグループ化　 FALSE：メイン商品のみ</summary>
		public static bool SETTING_PRODUCT_LIST_SEARCH_KBN = false;
		/// <summary>Add advc to request parameter option enabled</summary>
		public static bool ADD_ADVC_TO_REQUEST_PARAMETER_OPTION_ENABLED = false;

		/// <summary>定期売上予測集計オプション</summary>
		public static bool FIXED_PURCHASE_FORECAST_REPORT_OPTION = false;

		/// <summary>Payment credit zcom api url check auth</summary>
		public static string PAYMENT_CREDIT_ZCOM_APIURL_CHECKAUTH = string.Empty;
		/// <summary>Payment credit use Zcom 3DSecure mock</summary>
		public static bool PAYMENT_CREDIT_USE_ZCOM3DS_MOCK = false;
		/// <summary>Payment credit Zcom api url check auth mock</summary>
		public static string PAYMENT_CREDIT_ZCOM_APIURL_CHECKAUTH_MOCK = string.Empty;
		/// <summary>Payment credit Zcom api url access auth mock</summary>
		public static string PAYMENT_CREDIT_ZCOM_APIURL_ACCESSAUTH_MOCK = string.Empty;
		/// <summary>Cache key payment credit Zcom transaction info</summary>
		public const string CACHE_KEY_PAYMENT_CREDIT_ZCOM_TRANSACTION_INFO = "zcom_transaction_info";

		/// <summary>ProductView取得項目(商品バリエーション一覧用)カラム名</summary>
		public static string PRODUCT_VARIATIONLIST_SELECTCOLUMNS = "";

		/// <summary>注文拡張項目:注文拡張項目オプション</summary>
		public static bool ORDER_EXTEND_OPTION_ENABLED = false;
		/// <summary>注文拡張項目:メールテンプレート チェックボックス形式 区切り文字</summary>
		public static string ORDER_EXTEND_MAIL_CHECKBOX_SEPARATOR = string.Empty;

		/// <summary>Rakuten API URL</summary>
		public static string PAYMENT_RAKUTEN_API_URL = string.Empty;
		/// <summary>Rakuten service id</summary>
		public static string PAYMENT_RAKUTEN_CREDIT_SERVICE_ID = string.Empty;
		/// <summary>Rakuten signature key</summary>
		public static string PAYMENT_RAKUTEN_CREDIT_SIGNATURE_KEY = string.Empty;
		/// <summary>Rakuten：セキュリティーコード対応</summary>
		public static bool PAYMENT_SETTING_CREDIT_RAKUTEN_SECURITYCODE = false;
		/// <summary>GetToken Js Url</summary>
		public static string PAYMENT_RAKUTEN_CREDIT_GET_TOKEN_JS_URL = string.Empty;
		/// <summary>Rakuten payment method</summary>
		public static RakutenPaymentType PAYMENT_RAKUTEN_CREDIT_PAYMENT_METHOD = RakutenPaymentType.AUTH;
		/// <summary>Payment Credit Rakuten Credit Auth Limit Day</summary>
		public static int RAKUTEN_PAY_AUTH_EXPIRE_DAYS = 0;
		/// <summary>Datetime format yyyy-MM-dd HH:mm:ss.fff </summary>
		public static string RAKUTEN_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";
		/// <summary>Intergration Rakuten Case</summary>
		public static string RAKUTEN_INTERGRATION_CASE = "cardTokenWithCvv";
		/// <summary>Intergration Rakuten Case</summary>
		public static bool PAYMENT_SETTING_RAKUTEN_3DSECURE = false;
		/// <summary>Rakuten 3Dセキュア利用 マーチャントID(Visa)</summary>
		public static string PAYMENT_SETTING_RAKUTEN_3DSECURE_MERCHANT_ID_VISA = "";
		/// <summary>Rakuten 3Dセキュア利用 マーチャントID(MasterCard)</summary>
		public static string PAYMENT_SETTING_RAKUTEN_3DSECURE_MERCHANT_ID_MASTERCARD = "";
		/// <summary>Rakuten 3Dセキュア利用 マーチャントID(Name)</summary>
		public static string PAYMENT_SETTING_RAKUTEN_3DSECURE_MERCHANT_NAME = "";

		/// <summary>Registed tel1</summary>
		public static string REGISTED_TEL1 = "registed_tel1";
		/// <summary>Registed tel1_1</summary>
		public static string REGISTED_TEL1_1 = "registed_tel1_1";
		/// <summary>Registed tel1_2</summary>
		public static string REGISTED_TEL1_2 = "registed_tel1_2";
		/// <summary>Registed tel1_3</summary>
		public static string REGISTED_TEL1_3 = "registed_tel1_3";

		/// <summary>Credit credit attack lock method</summary>
		public static CreditAuthAtackBlockMethod? CONST_CREDIT_AUTH_ATTACK_BLOCK_METHOD = null;

		/// <summary>定期注文：定期注文の初回購入価格および回数割引の適用方法（FIXEDPURCHASE_PRODUCT_COUNT：定期商品購入回数、FIXEDPURCHASE_COUNT：定期購入回数）</summary>
		public static string FIXEDPURCHASE_ORDER_DISCOUNT_METHOD = string.Empty;
		/// <summary>定期注文：定期注文の初回購入価格および回数割引の適用方法（FIXEDPURCHASE_PRODUCT_COUNT：定期商品購入回数）</summary>
		public static string FLG_FIXEDPURCHASE_PRODUCT_COUNT = "FIXEDPURCHASE_PRODUCT_COUNT";
		/// <summary>定期注文：定期注文の初回購入価格および回数割引の適用方法（FIXEDPURCHASE_COUNT：定期購入回数）</summary>
		public static string FLG_FIXEDPURCHASE_COUNT = "FIXEDPURCHASE_COUNT";

		/// <summary>O-PLUX連携オプション</summary>
		public static bool OPLUX_ENABLED = false;
		/// <summary>加盟店ID（企業ID）</summary>
		public static string OPLUX_REQUEST_SHOP_ID = string.Empty;
		/// <summary>接続元ID</summary>
		public static string OPLUX_CONNECTION_ID = string.Empty;
		/// <summary>シークレットアクセスキー</summary>
		public static string OPLUX_SECRET_ACCESS_KEY = string.Empty;
		/// <summary>審査モデルID</summary>
		public static string OPLUX_REQUEST_EVENT_MODEL_ID = string.Empty;
		/// <summary>接続先URL：イベント登録API（POST）</summary>
		public static string OPLUX_REQUEST_EVENT_URL = string.Empty;
		/// <summary>接続先URL：氏名正規化API（GET ）</summary>
		public static string OPLUX_NORMALIZE_NAME_URL = string.Empty;
		/// <summary>決済種別　クレジットカード</summary>
		public static string OPLUX_PAYMENT_KBN = string.Empty;
		/// <summary>O-PLUX拡張ステータス</summary>
		public static string OPLUX_REVIEW_EXTEND_NO = string.Empty;
		/// <summary>Directory O-PLUX api log file path</summary>
		public static string DIRECTORY_OPLUX_API_LOGFILEPATH = string.Empty;
		/// <summary>O-PLUX api log file name prefix</summary>
		public const string OPLUX_API_LOGFILE_NAME_PREFIX = "OPluxApi";
		/// <summary>O-PLUX log file extension</summary>
		public const string OPLUX_LOGFILE_EXTENSION = "log";
		/// <summary>O-PLUX log file encoding</summary>
		public const string OPLUX_LOGFILE_ENCODING = "shift_jis";
		/// <summary>O-PLUX log file threshold（MB）</summary>
		public static int OPLUX_API_LOGFILE_THRESHOLD = 100;
		/// <summary>Register event api result</summary>
		public const string OPLUX_REGISTER_EVENT_API_RESULT = "register_event_api_result";

		/// <summary>O-MOTION連携オプション</summary>
		public static bool OMOTION_ENABLED = false;
		/// <summary>O-MOTION：加盟店コード</summary>
		public static string OMOTION_MERCHANTID = "";
		/// <summary>O-MOTION：署名コード</summary>
		public static string OMOTION_SIGNATURE = "";
		/// <summary>O-MOTION：ハッシュ用ソルト値</summary>
		public static string OMOTION_SALT = "";
		/// <summary>O-MOTION JS PATH</summary>
		public static string OMOTION_JS_PATH = string.Empty;
		/// <summary>接続先URL：ユーザー審査取得API</summary>
		public static string OMOTION_REQUEST_AUTHORI_URL = string.Empty;
		/// <summary>接続先URL：ユーザー審査ログイン成功API</summary>
		public static string OMOTION_REQUEST_AUTHORI_LOGIN_SUCCESS_URL = string.Empty;
		/// <summary>接続先URL：ユーザー審査フィードバック更新API</summary>
		public static string OMOTION_REQUEST_AUTHORI_FEEDBACK_URL = string.Empty;
		/// <summaryO-MOTION：テストログインID</summary>
		public static string OMOTION_TEST_LOGINID = string.Empty;

		/// <summary>キー：返金先銀行コード</summary>
		public static string CONST_ORDER_REPAYMENT_BANK_CODE = "repayment_bank_code";
		/// <summary>キー：返金先銀行名</summary>
		public static string CONST_ORDER_REPAYMENT_BANK_NAME = "repayment_bank_name";
		/// <summary>キー：返金先銀行支店名</summary>
		public static string CONST_ORDER_REPAYMENT_BANK_BRANCH = "repayment_bank_branch";
		/// <summary>キー：返金先銀行口座番号</summary>
		public static string CONST_ORDER_REPAYMENT_BANK_ACCOUNT_NO = "repayment_bank_account_no";
		/// <summary>キー：返金先銀行口座名</summary>
		public static string CONST_ORDER_REPAYMENT_BANK_ACCOUNT_NAME = "repayment_bank_account_name";
		/// <summary>返金メモ入力タイプ</summary>
		public const string VALUETEXT_ORDER_REPAYMENT_MEMO_TYPE = "repayment_memo_type";
		/// <summary>返金メモ入力タイプ：自由欄</summary>
		public const string FLG_ORDER_REPAYMENT_MEMO_TYPE_FREE_TEXT = "FREE_TEXT";
		/// <summary>返金メモ入力タイプ：返金先銀行口座情報入力欄</summary>
		public const string FLG_ORDER_REPAYMENT_MEMO_TYPE_REPAYMENT_BANK_TEXT = "REPAYMENT_BANK_TEXT";
		/// <summary>Shipping type list</summary>
		public const string FLG_ORDER_SHIPPING_KBN_LIST = "shipping_kbn_list";

		/// <summary>注文キャンセル時間（分）</summary>
		public static int ORDER_HISTORY_DETAIL_ORDER_CANCEL_TIME = 0;

		/// <summary>PayPayオプション（決済）</summary>
		public static bool PAYMENT_PAYPAYOPTION_ENABLED = false;
		/// <summary>決済区分：PayPay (SBPS、GMO、VeriTrans)</summary>
		public static PaymentPayPayKbn? PAYMENT_PAYPAY_KBN = null;
		/// <summary>PayPay　処理区分(AUTH:仮売上, CAPTURE:即時売上)</summary>
		public static string PAYMENT_PAYPAY_JOB_CODE = string.Empty;
		/// <summary>PayPayショップID</summary>
		public static string PAYMENT_PAYPAY_SHOP_ID = string.Empty;
		/// <summary>PayPayショップパスワード</summary>
		public static string PAYMENT_PAYPAY_SHOP_PASSWORD = string.Empty;
		/// <summary>PayPay　商品カテゴリID</summary>
		public static string PAYMENT_PAYPAY_CATEGORY_ID = string.Empty;
		/// <summary>PayPay　ペイペイ接続先API 取引登録API</summary>
		public static string PAYMENT_PAYPAY_ENTRY_TRAN_API = string.Empty;
		/// <summary>PayPay　ペイペイ接続先API 決済実行API</summary>
		public static string PAYMENT_PAYPAY_EXEC_API = string.Empty;
		/// <summary>PayPay　ペイペイ接続先API 実売上API</summary>
		public static string PAYMENT_PAYPAY_SALES_API = string.Empty;
		/// <summary>PayPay　ペイペイ接続先API キャンセルAPI</summary>
		public static string PAYMENT_PAYPAY_CANCEL_RETURN_API = string.Empty;
		/// <summary>Paypay 取引照会API接続先</summary>
		public static string PAYMENT_PAYPAY_SEARCH_TRADE_MULTI_API = string.Empty;
		/// <summary>PayPay　ペイペイ接続先API 定期取引登録API</summary>
		public static string PAYMENT_PAYPAY_ENTRY_TRAN_ACCEPT_API = string.Empty;
		/// <summary>PayPay　ペイペイ接続先API 定期決済実行API</summary>
		public static string PAYMENT_PAYPAY_EXEC_ACCEPT_API = string.Empty;
		/// <summary>PayPay　ペイペイ接続先API 利用承諾終了API</summary>
		public static string PAYMENT_PAYPAY_EXEC_ACCEPT_END_API = string.Empty;

		/// <summary>データ移行オプション</summary>
		public static bool DATAMIGRATION_OPTION_ENABLED = false;
		/// <summary>データ移行利用終了日時</summary>
		public static DateTime DATAMIGRATION_END_DATETIME = DateTime.Parse("2100/01/01");
		/// <summary>データ移行ログ出力先物理パス</summary>
		public static string DIRECTORY_DATAMIGRATION_LOG_FILEPATH = string.Empty;

		/// <summary>マスタファイル取込実行ＥＸＥ</summary>
		public static string PHYSICALDIRPATH_MASTERUPLOAD_EXE = string.Empty;
		/// <summary>ステートメントXML格納物理ディレクトリパス</summary>
		public static string DIRECTORY_W2CMANAGER_SQL_STATEMENT_XML = string.Empty;
		/// <summary>エラーログファイルURL</summary>
		public static string DATAMIGRATION_DISP_ERROR_LOG_URL = string.Empty;
		/// <summary>データ移行セッティングファイルパス（EC用）</summary>
		public static string DATAMIGRATION_SETTING_FILEPATH_EC = string.Empty;
		/// <summary>データ移行セッティングファイルパス（CS用）</summary>
		public static string DATAMIGRATION_SETTING_FILEPATH_CS = string.Empty;

		/// <summary>日別出荷予測レポート実行ＥＸＥ</summary>
		public static string PHYSICALDIRPATH_CREATEREPORT_EXE = string.Empty;

		/// <summary>Execution EXE: Reauth</summary>
		public static string PHYSICALDIRPATH_REAUTH_EXE = string.Empty;

		/// <summary>Reauth total count</summary>
		public const string CONST_REAUTH_TOTAL_COUNT = "reauth_total_count";
		/// <summary>Reauth mode</summary>
		public const string CONST_REAUTH_MODE = "reauth_mode";
		/// <summary>Reauth date</summary>
		public const string CONST_REAUTH_DATE = "reauth_date";
		/// <summary>Reauth extend</summary>
		public const string CONST_REAUTH_EXTEND = "reauth_extend";
		/// <summary>Reauth success count</summary>
		public const string CONST_REAUTH_SUCESS_COUNT = "reauth_success_count";
		/// <summary>reauth failure count</summary>
		public const string CONST_REAUTH_FAILURE_COUNT = "reauth_failure_count";
		/// <summary>reauth実行異常・正常</summary>
		public const string CONST_REAUTH_EXECUTION_RESULT = "result";
		/// <summary>reauth異常終了時理由</summary>
		public const string CONST_REAUTH_EXECUTION_FAILURE_MESSAGE = "message";

		/// <summary>Introduction coupon option enabled</summary>
		public static bool INTRODUCTION_COUPON_OPTION_ENABLED = false;

		/// <summary>Short date time 2 letter format</summary>
		public const string CONST_SHORTDATETIME_2LETTER_FORMAT = "yyyy/MM/dd HH:mm:ss";

		/// <summary>FacebookコンバージョンAPI連携オプション（利用：TRUE 非利用：FALSE）</summary>
		public static bool MARKETING_FACEBOOK_CAPI_OPTION_ENABLED = false;
		/// <summary>FacebookコンバージョンAPI連携：API接続先URL</summary>
		public static string MARKETING_FACEBOOK_CAPI_API_URL = string.Empty;
		/// <summary>FacebookコンバージョンAPI連携：API接続先バージョン</summary>
		public static string MARKETING_FACEBOOK_CAPI_API_VERSION = string.Empty;
		/// <summary>FacebookコンバージョンAPI連携：ピクセルID</summary>
		public static string MARKETING_FACEBOOK_CAPI_PIXEL_ID = string.Empty;
		/// <summary>FacebookコンバージョンAPI連携：アクセストークン</summary>
		public static string MARKETING_FACEBOOK_CAPI_ACCESS_TOKEN = string.Empty;
		/// <summary>FacebookコンバージョンAPI連携：テストイベントコード（イベントテストツール用）</summary>
		public static string MARKETING_FACEBOOK_CAPI_TEST_EVENT_CODE = string.Empty;
		/// <summary>Facebook conversion api</summary>
		public static string FACEBOOK_CONVERSION_API = "FacebookConversionAPI";
		/// <summary>APIログファイル サイズ閾値（MB）</summary>
		public static int FACEBOOK_API_LOGFILE_THRESHOLD = 100;

		/// <summary>YAHOO API</summary>
		public static string YAHOO_API = "YahooAPI";

		// 後払いcom連携 API
		/// <summary>後払いcom連携：有効無効</summary>
		public static bool ATOBARAICOM_LOGIN_ENABLED = false;
		/// <summary>クライアントID（接続元を特定するためのID情報）</summary>
		public static string ATOBARAICOM_CONNECT_CLIENT_ID = string.Empty;
		/// <summary>クライアントSecret（接続元を認証するためのキー情報）</summary>
		public static string ATOBARAICOM_CONNECT_CLIENT_SECRET = string.Empty;
		/// <summary>後払いcom連携用モックURL（※開発時は値を入れる）</summary>
		public static string ATOBARAICOM_CONNECT_MOCK_URL = string.Empty;
		/// <summary>後払いcom連携用デバッグログ出力（※ログに個人情報が含まれるため、必要な時だけTRUEにする）</summary>
		public static bool ATOBARAICOM_CONNECT_OUTPUT_DEBUGLOG = false;
		/// <summary>後払いcom連携：Open ID格納用ユーザー拡張項目</summary>
		public static string ATOBARAICOM_CONNECT_OPEN_ID = string.Empty;
		/// <summary>後払いcom連携：楽天IDConnect登録フラグユーザー拡張項目</summary>
		public static string ATOBARAICOM_CONNECT_REGISTER_USER = string.Empty;

		/// <summary>Personal authentication of user registration option enabled</summary>
		public static bool PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED = false;
		/// <summary>Personal authentication of user registration auth method</summary>
		public static string PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_AUTH_METHOD = string.Empty;
		/// <summary>Personal authentication of user registration authcode digits</summary>
		public static int PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_AUTH_CODE_DIGITS = 0;
		/// <summary>Personal authentication of user registration authcode expiration time</summary>
		public static int PERSONAL_AUTHENTICATION_OF_USERR_EGISTRATION_AUTH_CODE_EXPIRATION_TIME = 0;
		/// <summary>Personal authentication possible trial auth code send count</summary>
		public static int PERSONAL_AUTHENTICATION_POSSIBLE_TRIAL_AUTH_CODE_SEND_COUNT = 0;
		/// <summary>Personal authentication auth code send lock minutes</summary>
		public static int PERSONAL_AUTHENTICATION_AUTH_CODE_SEND_LOCK_MINUTES = 0;
		/// <summary>Personal authentication possible trial auth code try count</summary>
		public static int PERSONAL_AUTHENTICATION_POSSIBLE_TRIAL_AUTH_CODE_TRY_COUNT = 0;
		/// <summary>Personal authentication auth code try lock minutes</summary>
		public static int PERSONAL_AUTHENTICATION_AUTH_CODE_TRY_LOCK_MINUTES = 0;

		// 商品画像リサイズ設定
		/// <summary>商品画像サイズS</summary>
		public static string PRODUCTIMAGE_SIZE_S = "0";
		/// <summary>商品画像サイズM</summary>
		public static string PRODUCTIMAGE_SIZE_M = "0";
		/// <summary>商品画像サイズL</summary>
		public static string PRODUCTIMAGE_SIZE_L = "0";
		/// <summary>商品画像サイズLL</summary>
		public static string PRODUCTIMAGE_SIZE_LL = "0";

		// 商品サブ画像リサイズ設定
		/// <summary>商品サブ画像サイズM</summary>
		public static string PRODUCTSUBIMAGE_SIZE_M = "0";
		/// <summary>商品サブ画像サイズL</summary>
		public static string PRODUCTSUBIMAGE_SIZE_L = "0";
		/// <summary>商品サブ画像サイズLL</summary>
		public static string PRODUCTSUBIMAGE_SIZE_LL = "0";

		/// <summary>Product variation image footer</summary>
		public static string PRODUCTVARIATIONIMAGE_FOOTER = "_var";

		/// <summary>管理者向け注文完了メール送信オプション</summary>
		public static List<string> SEND_ORDER_COMPLETE_EMAIL_FOR_OPERATOR_ENABLED_LIST = new List<string>();

		/// <summary>スコアリング販売オプション</summary>
		public static bool SCORINGSALE_OPTION = false;
		/// <summary>Scoring sale page dir path pc</summary>
		public static string CMS_SCORINGSALE_PAGE_DIR_PATH_PC = "Form/ScoringSale/";
		/// <summary>Scoring sale page dir path sp</summary>
		public static string CMS_SCORINGSALE_PAGE_DIR_PATH_SP = "SmartPhone/Form/ScoringSale/";
		/// <summary>Partial view name scoring sale image list modal</summary>
		public static string CMS_PARTIALVIEWNAME_SCORING_SALE_IMAGE_LIST_MODAL = "_ScoringSaleImageListModal";

		// Request key scoring sale id
		public const string REQUEST_KEY_SCORINGSALE_ID = "scid";
		/// <summary>Subcription box option enabled</summary>
		public static bool SUBSCRIPTION_BOX_OPTION_ENABLED = false;
		/// <summary>頒布会コース一覧：表示件数</summary>
		public static int DISP_LIST_CONTENTS_COUNT_SUBSCRIPTION_BOX_LIST = 0;
		/// <summary>頒布会コース詳細：商品表示件数</summary>
		public static int DISP_LIST_CONTENTS_COUNT_SUBSCRIPTION_BOX_PRODUCT_LIST = 0;
		/// <summary>頒布会商品選択方法</summary>
		public static w2.App.Common.SubscriptionBox.SubscriptionBoxProductChangeMethod SUBSCRIPTION_BOX_PRODUCT_CHANGE_METHOD = SubscriptionBoxProductChangeMethod.Modal;

		/// <summary>Count the number of loops displayed</summary>
		public static int CART_DISPLAY_COUNT = 0;
		/// <summary>Require shipp day</summary>
		public static int SHIPPING_FIXED_PURCHASE_DAY_REQUIRE = 0;
		/// <summary>Get total number display</summary>
		public static int CART_TOTAL_DISPLAY_COUNT = 0;
		/// <summary> メールタグ 頒布会コース表示名 </summary>
		public const string MAILTAG_SUBSCRIPTION_BOX_DISPLAY_NAME = "subscription_box_display_name";
		/// <summary>Gmo掛け払いコンフィグ</summary>
		public static bool PAYMENT_GMO_POST_ENABLED = true;
		/// <summary>認証ID</summary>
		public static string SETTING_GMO_PAYMENT_AUTHENTICATIONID = "";
		/// <summary>加盟店コード</summary>
		public static string SETTING_GMO_PAYMENT_SHOPCODE = "";
		/// <summary>接続パスワード</summary>
		public static string SETTING_GMO_PAYMENT_CONNECTPASSWORD = "";
		/// <summary>取引登録APIのURL</summary>
		public static string SETTING_GMO_PAYMENT_URL_TRANSACTION_REGISTER = "";
		/// <summary>枠保証ステータス取得APIのURL</summary>
		public static string SETTING_GMO_PAYMENT_URL_FRAME_GUARANTEE_GET_STATUS = "";
		/// <summary>枠保証登録APIのURL</summary>
		public static string SETTING_GMO_PAYMENT_URL_FRAME_GUARANTEE_REGISTER = "";
		/// <summary>枠保証更新APIのURL/summary>
		public static string SETTING_GMO_PAYMENT_URL_FRAME_GUARANTEE_UPDATE = "";
		/// <summary>取引変更・キャンセルAPIのURL</summary>
		public static string SETTING_GMO_PAYMENT_URL_MODIFY_CANCEL_TRANSACTION = "";
		/// <summary>請求確認APIのURL</summary>
		public static string SETTING_GMO_PAYMENT_URL_BILLING_CONFIRM = "";
		/// <summary>請求変更・キャンセルAPIのURL</summary>
		public static string SETTING_GMO_PAYMENT_URL_BILLING_MODIFY_CANCEL = "";
		/// <summary>与信結果取得APIのURL</summary>
		public static string SETTING_GMO_PAYMENT_URL_GET_CREDIT_RESULT = "";
		/// <summary>請求削減APIのURL</summary>
		public static string SETTING_GMO_PAYMENT_URL_REDUCED_CLAIMS = "";
		/// <summary>ログ記録</summary>
		public static string SETTING_GMO_PAYMENT_URL_WRITE_LOG	 = "GMO Payment API Log";

		/// <summary>Const payment cvs type</summary>
		public static string CONST_PAYMENT_CVS_TYPE = "cvs_type";

		/// <summary>Mail tag: Order item product index</summary>
		public const string MAILTAG_ORDER_ITEM_PRODUCT_INDEX = "order_item_product_index";
		/// <summary>Mail tag: Order item product id</summary>
		public const string MAILTAG_ORDER_ITEM_PRODUCT_ID = "order_item_product_id";
		/// <summary>Mail tag: Order item product name</summary>
		public const string MAILTAG_ORDER_ITEM_PRODUCT_NAME = "order_item_product_name";
		/// <summary>Mail tag: Order item product image</summary>
		public const string MAILTAG_ORDER_ITEM_PRODUCT_IMAGE = "order_item_product_image";
		/// <summary>Mail tag: Order item product variation image</summary>
		public const string MAILTAG_ORDER_ITEM_PRODUCT_VARIATION_IMAGE = "order_item_product_variation_image";
		/// <summary>Mail tag: Order item product outline</summary>
		public const string MAILTAG_ORDER_ITEM_PRODUCT_OUTLINE = "order_item_product_outline";
		/// <summary>Mail tag: Order item product detail1</summary>
		public const string MAILTAG_ORDER_ITEM_PRODUCT_DETAIL1 = "order_item_product_detail1";
		/// <summary>Mail tag: Order item product detail2</summary>
		public const string MAILTAG_ORDER_ITEM_PRODUCT_DETAIL2 = "order_item_product_detail2";
		/// <summary>Mail tag: Order item product detail3</summary>
		public const string MAILTAG_ORDER_ITEM_PRODUCT_DETAIL3 = "order_item_product_detail3";
		/// <summary>Mail tag: Order item product detail4</summary>
		public const string MAILTAG_ORDER_ITEM_PRODUCT_DETAIL4 = "order_item_product_detail4";
		/// <summary>Mail tag: Order item product link</summary>
		public const string MAILTAG_ORDER_ITEM_PRODUCT_LINK = "order_item_product_link";
		/// <summary>Mail tag: Order item product quantity</summary>
		public const string MAILTAG_ORDER_ITEM_PRODUCT_QUANTITY = "order_item_product_quantity";
		/// <summary>Mail tag: Order item product price</summary>
		public const string MAILTAG_ORDER_ITEM_PRODUCT_PRICE = "order_item_product_price";
		/// <summary>Mail tag: Order item product option</summary>
		public const string MAILTAG_ORDER_ITEM_PRODUCT_OPTION = "order_item_product_option";
		/// <summary>Mail tag: Order item product set quantity</summary>
		public const string MAILTAG_ORDER_ITEM_SET_PRODUCT_TOTAL_QUANTITY = "order_item_set_product_total_quantity";
		/// <summary>Mail tag: Order item product set price</summary>
		public const string MAILTAG_ORDER_ITEM_SET_PRODUCT_TOTAL_PRICE = "order_item_set_product_total_price";
		/// <summary>Mail tag: Order item product setpromotion_disp name</summary>
		public const string MAILTAG_ORDER_ITEM_SETPROMOTION_DISP_NAME = "order_item_setpromotion_disp_name";
		/// <summary>Mail tag: Order item product serial key</summary>
		public const string MAILTAG_ORDER_ITEM_PRODUCT_SERIAL_KEY = "order_item_product_serial_key";
		/// <summary>Mail tag: Order items loop</summary>
		public const string MAILTAG_ORDER_ITEMS_LOOP = "OrderItemsLoop";
		/// <summary>Mail tag: Order product serial keys loop</summary>
		public const string MAILTAG_ORDER_PRODUCT_SERIAL_KEYS_LOOP = "OrderProductSerialKeysLoop";
		/// <summary>Mail tag: Is order product variation</summary>
		public const string MAILTAG_IS_ORDER_PRODUCT_VARIATION = "IsOrderProductVariation";
		/// <summary>Mail tag: Is not order product variation</summary>
		public const string MAILTAG_IS_NOT_ORDER_PRODUCT_VARIATION = "IsNotOrderProductVariation";
		/// <summary>Mail tag: Order set product</summary>
		public const string MAILTAG_ORDER_SET_PRODUCT = "OrderSetProduct";
		/// <summary>Mail tag: Order set product loop</summary>
		public const string MAILTAG_ORDER_SET_PRODUCTS_LOOP = "OrderSetProductsLoop";
		/// <summary>Mail tag: Order set promotion product</summary>
		public const string MAILTAG_ORDER_SET_PROMOTION_PRODUCT = "OrderSetPromotionProduct";
		/// <summary>Mail tag: Order set promotion product loop</summary>
		public const string MAILTAG_ORDER_SET_PROMOTION_PRODUCTS_LOOP = "OrderSetPromotionProductsLoop";
		/// <summary>Mail tag: Order normal products</summary>
		public const string MAILTAG_ORDER_NORMAL_PRODUCT = "OrderNormalProduct";
		/// <summary>Mail tag: Order item set promotion product index</summary>
		public const string MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_INDEX = "order_item_setpromotion_product_index";
		/// <summary>Mail tag: Order item set promotion product id</summary>
		public const string MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_ID = "order_item_setpromotion_product_id";
		/// <summary>Mail tag: Order item set promotion product name</summary>
		public const string MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_NAME = "order_item_setpromotion_product_name";
		/// <summary>Mail tag: Order item set promotion product image</summary>
		public const string MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_IMAGE = "order_item_setpromotion_product_image";
		/// <summary>Mail tag: Order item set promotion product variation image</summary>
		public const string MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_VARIATION_IMAGE = "order_item_setpromotion_product_variation_image";
		/// <summary>Mail tag: Order item set promotion product outline</summary>
		public const string MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_OUTLINE = "order_item_setpromotion_product_outline";
		/// <summary>Mail tag: Order item set promotion product detail1</summary>
		public const string MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_DETAIL1 = "order_item_setpromotion_product_detail1";
		/// <summary>Mail tag: Order item set promotion product detail2</summary>
		public const string MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_DETAIL2 = "order_item_setpromotion_product_detail2";
		/// <summary>Mail tag: Order item set promotion product detail3</summary>
		public const string MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_DETAIL3 = "order_item_setpromotion_product_detail3";
		/// <summary>Mail tag: Order item set promotion product detail4</summary>
		public const string MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_DETAIL4 = "order_item_setpromotion_product_detail4";
		/// <summary>Mail tag: Order item set promotion product link</summary>
		public const string MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_LINK = "order_item_setpromotion_product_link";
		/// <summary>Mail tag: Order item set promotion product quantity</summary>
		public const string MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_QUANTITY = "order_item_setpromotion_product_quantity";
		/// <summary>Mail tag: Order item set promotion product price</summary>
		public const string MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_PRICE = "order_item_setpromotion_product_price";
		/// <summary>Mail tag: Order item set promotion product option</summary>
		public const string MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_OPTION = "order_item_setpromotion_product_option";
		/// <summary>Mail tag: Order item set promotion product serial key</summary>
		public const string MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_SERIAL_KEY = "order_item_setpromotion_product_serial_key";
		/// <summary>Mail tag: Order item set product index</summary>
		public const string MAILTAG_ORDER_ITEM_SET_PRODUCT_INDEX = "order_item_set_product_index";
		/// <summary>Mail tag: Order item set product id</summary>
		public const string MAILTAG_ORDER_ITEM_SET_PRODUCT_ID = "order_item_set_product_id";
		/// <summary>Mail tag: Order item set product name</summary>
		public const string MAILTAG_ORDER_ITEM_SET_PRODUCT_NAME = "order_item_set_product_name";
		/// <summary>Mail tag: Order item set product image</summary>
		public const string MAILTAG_ORDER_ITEM_SET_PRODUCT_IMAGE = "order_item_set_product_image";
		/// <summary>Mail tag: Order item set product variation image</summary>
		public const string MAILTAG_ORDER_ITEM_SET_PRODUCT_VARIATION_IMAGE = "order_item_set_product_variation_image";
		/// <summary>Mail tag: Order item set product outline</summary>
		public const string MAILTAG_ORDER_ITEM_SET_PRODUCT_OUTLINE = "order_item_set_product_outline";
		/// <summary>Mail tag: Order item set product detail 1</summary>
		public const string MAILTAG_ORDER_ITEM_SET_PRODUCT_DETAIL1 = "order_item_set_product_detail1";
		/// <summary>Mail tag: Order item set product detail 2</summary>
		public const string MAILTAG_ORDER_ITEM_SET_PRODUCT_DETAIL2 = "order_item_set_product_detail2";
		/// <summary>Mail tag: Order item set product detail 3</summary>
		public const string MAILTAG_ORDER_ITEM_SET_PRODUCT_DETAIL3 = "order_item_set_product_detail3";
		/// <summary>Mail tag: Order item set product detail 4</summary>
		public const string MAILTAG_ORDER_ITEM_SET_PRODUCT_DETAIL4 = "order_item_set_product_detail4";
		/// <summary>Mail tag: Order item set product link</summary>
		public const string MAILTAG_ORDER_ITEM_SET_PRODUCT_LINK = "order_item_set_product_link";
		/// <summary>Mail tag: Order item set product price</summary>
		public const string MAILTAG_ORDER_ITEM_SET_PRODUCT_PRICE = "order_item_set_product_price";
		/// <summary>Mail tag: Order item set product quantity</summary>
		public const string MAILTAG_ORDER_ITEM_SET_PRODUCT_QUANTITY = "order_item_set_product_quantity";
		/// <summary>Mail tag: Is order set promotion product variation</summary>
		public const string MAILTAG_IS_ORDER_SET_PROMOTION_PRODUCT_VARIATION = "IsOrderSetPromotionProductVariation";
		/// <summary>Mail tag: Is not order set promotion product variation</summary>
		public const string MAILTAG_IS_NOT_ORDER_SET_PROMOTION_PRODUCT_VARIATION = "IsNotOrderSetPromotionProductVariation";
		/// <summary>Mail tag: Order set promotion product serial keys Loop</summary>
		public const string MAILTAG_ORDER_SET_PROMOTION_PRODUCT_SERIAL_KEYS_LOOP = "OrderSetPromotionProductSerialKeysLoop";
		/// <summary>Mail tag: Is order set product variation</summary>
		public const string MAILTAG_IS_ORDER_SET_PRODUCT_VARIATION = "IsOrderSetProductVariation";
		/// <summary>Mail tag: Is not order set product variation</summary>
		public const string MAILTAG_IS_NOT_ORDER_SET_PRODUCT_VARIATION = "IsNotOrderSetProductVariation";
		/// <summary>Mail tag: Order item product items</summary>
		public const string MAILTAG_ORDER_ITEM_PRODUCT_ITEMS = "items";
		/// <summary>Mail tag: Order item type</summary>
		public const string MAILTAG_ORDER_ITEM_TYPE = "type";

		/// <summary>Mail tag: Store pickup order id name</summary>
		public const string MAILTAG_STORE_PICKUP_ORDER_ID = "storepickup_order_id";
		/// <summary>Mail tag: Store pickup shop name</summary>
		public const string MAILTAG_STORE_PICKUP_SHOP_NAME = "storepickup_shop_name";
		/// <summary>Mail tag: Store pickup order count</summary>
		public const string MAILTAG_STORE_PICKUP_ORDER_COUNT = "storepickup_order_count";
		/// <summary>Mail tag: Store pickup shop zip</summary>
		public const string MAILTAG_STORE_PICKUP_SHOP_ZIP = "storepickup_shop_zip";
		/// <summary>Mail tag: Store pickup shop addr1</summary>
		public const string MAILTAG_STORE_PICKUP_SHOP_ADDR1 = "storepickup_shop_addr1";
		/// <summary>Mail tag: Store pickup shop addr2</summary>
		public const string MAILTAG_STORE_PICKUP_SHOP_ADDR2 = "storepickup_shop_addr2";
		/// <summary>Mail tag: Store pickup shop addr3</summary>
		public const string MAILTAG_STORE_PICKUP_SHOP_ADDR3 = "storepickup_shop_addr3";
		/// <summary>Mail tag: Store pickup shop addr4</summary>
		public const string MAILTAG_STORE_PICKUP_SHOP_ADDR4 = "storepickup_shop_addr4";
		/// <summary>Mail tag: Store pickup shop tel</summary>
		public const string MAILTAG_STORE_PICKUP_SHOP_TEL = "storepickup_shop_tel";
		/// <summary>Mail tag: Store pickup shop business hours</summary>
		public const string MAILTAG_STORE_PICKUP_SHOP_BUSINESS_HOURS = "storepickup_shop_business_hours";
		/// <summary>Mail tag: Store pickup is store pickup order</summary>
		public const string MAILTAG_STORE_PICKUP_IS_STOREPICKUPORDER = "is_storepickuporder";
		/// <summary>Mail tag: Store pickup order loop</summary>
		public const string MAILTAG_STORE_PICKUP_STOREPICKUP_ORDER_LOOP = "StorePickUpOrderLoop";
		/// <summary>Mail tag: Store pickup lead time</summary>
		public const string MAILTAG_STORE_PICKUP_LEAD_TIME = "storepickup_lead_time";

		/// <summary>オプション：CROSSPOINT連携</summary>
		public static bool CROSS_POINT_OPTION_ENABLED = false;
		/// <summary>CROSSPOINT連携：ログイン毎ポイント利用</summary>
		public static bool CROSS_POINT_LOGIN_POINT_ENABLED = false;
		/// <summary>CROSSPOINT連携：クリックポイント利用</summary>
		public static bool CROSS_POINT_CLICK_POINT_ENABLED = false;
		/// <summary>CROSSPOINT連携：会員情報日次連携用FTPホス</summary>
		public static string CROSS_POINT_FTP_HOST = string.Empty;
		/// <summary>CROSSPOINT連携：会員情報日次連携用FTPアカウント</summary>
		public static string CROSS_POINT_FTP_ID = string.Empty;
		/// <summary>CROSSPOINT連携：会員情報日次連携用FTPパスワード</summary>
		public static string CROSS_POINT_FTP_PW = string.Empty;
		/// <summary>CROSSPOINT連携：会員情報日次連携用FTPポート番</summary>
		public static int? CROSS_POINT_FTP_PORT = 0;
		/// <summary>CROSSPOINT連携：会員情報日次連携用FTPファイル出力先</summary>
		public static string CROSS_POINT_FTP_FILE_PATH = string.Empty;
		/// <summary>CROSS POINT オンラインショップ用店舗名</summary>
		public static string CROSS_POINT_EC_SHOP_NAME = string.Empty;
		/// <summary>CROSSPOINT連携：JANコード連携用商品連携ID（1～10）※空の場合はダミー値が連携されます</summary>
		public static int CROSS_POINT_JANCODE_PRODUCT_COOPERATION_ID_NO = 0;
		/// <summary>CROSSPOINT連携：ダミー連携用JANコード</summary>
		public static string CROSS_POINT_DUMMY_JANCODE = string.Empty;

		/// <summary>CrossPoint設定の要素名：理由ID</summary>
		public static string CROSS_POINT_SETTING_ELEMENT_REASON_ID = "ReasonId";
		/// <summary>CrossPoint設定の属性名：ポイント加算区分</summary>
		public static string CROSS_POINT_SETTING_ATTRIBUTE_POINT_INC_KBN = "PointIncKbn";
		/// <summary>CrossPoint設定の要素名：ショップ名</summary>
		public static string CROSS_POINT_SETTING_SHOP_NAME = "shop_name";
		/// <summary>CrossPoint設定の要素名：ポイント変動理由</summary>
		public static string CROSS_POINT_SETTING_POINT_CHANGE_REASON = "point_change_reason";

		/// <summary>CROSS POINT API接続URL</summary>
		public static string CROSS_POINT_API_URL_ROOT_PATH = string.Empty;
		/// <summary>CROSS POINT テナントコード</summary>
		public static string CROSS_POINT_AUTH_TENANT_CODE = string.Empty;
		/// <summary>CROSS POINT ショップコード</summary>
		public static string CROSS_POINT_AUTH_SHOP_CODE = string.Empty;
		/// <summary>CROSS POINT オンラインショップ用PosNo</summary>
		public static string CROSS_POINT_POS_NO = string.Empty;
		/// <summary>CROSS POINT 認証鍵</summary>
		public static string CROSS_POINT_AUTH_AUTHENTICATION_KEY = string.Empty;
		/// <summary>CROSS POINT アプリからPOSTされる照合用パラメータ</summary>
		public static string CROSS_POINT_APP_REQUEST_APP_KEY = string.Empty;
		/// <summary>CROSS POINT ログ出力</summary>
		public static bool CROSS_POINT_API_WRITE_LOG = true;
		/// <summary>CROSS POINT 連携開始日時</summary>
		public static DateTime? CROSS_POINT_LINK_START_DATETIME = null;

		/// <summary>CROSS POINT ログ種別</summary>
		public const string CROSS_POINT_API_LOG_TYPE = "CROSSPOINTAPI";

		/// <summary>CROSS POINT API接続URL getMemberInfoWithPinCd</summary>
		public const string CROSS_POINT_URL_GET_USER = "getMemberInfoWithPinCd";
		/// <summary>CROSS POINT API接続URL getMemberListWithPinCd</summary>
		public const string CROSS_POINT_URL_GET_USER_LIST = "getMemberListWithPinCd";
		/// <summary>CROSS POINT API接続URL getGrantPointByDetail</summary>
		public const string CROSS_POINT_URL_GET_GRANT_POINT = "getGrantPointByDetail";
		/// <summary>CROSS POINT API接続URL insMember</summary>
		public const string CROSS_POINT_URL_INSERT_USER = "insMember";
		/// <summary>CROSS POINT API接続URL updMember</summary>
		public const string CROSS_POINT_URL_UPDATE_USER = "updMember";
		/// <summary>CROSS POINT API接続URL delMember</summary>
		public const string CROSS_POINT_URL_DELETE_USER = "delMember";
		/// <summary>CROSS POINT API接続URL mergeMemberInfo</summary>
		public const string CROSS_POINT_URL_MERGE_USER = "mergeMemberInfo";
		/// <summary>CROSS POINT API接続URL updPurchasePointByDetail</summary>
		public const string CROSS_POINT_URL_UPDATE_PURCHASE_POINT_BY_DETAIL = "updPurchasePointByDetail";
		/// <summary>CROSS POINT API接続URL fixGrantPoint</summary>
		public const string CROSS_POINT_URL_FIX_GRANT_POINT = "fixGrantPoint";
		/// <summary>CROSS POINT API接続URL cancelPurchasePointByDetail</summary>
		public const string CROSS_POINT_URL_CANCEL_PURCHASE_POINT_BY_DETAIL = "cancelPurchasePointByDetail";
		/// <summary>CROSS POINT API接続URL delPurchasePoint</summary>
		public const string CROSS_POINT_URL_DELETE_PURCHASE_POINT = "delPurchasePoint";
		/// <summary>CROSS POINT API接続URL getPointHisList</summary>
		public const string CROSS_POINT_URL_GET_POINT_HISTORY_LIST = "getPointHisList";
		/// <summary>CROSS POINT API接続URL getSlipList</summary>
		public const string CROSS_POINT_URL_GET_ORDER_HISTORY_LIST = "getSlipList";
		/// <summary>CROSS POINT API接続URL getSlipDetail</summary>
		public const string CROSS_POINT_URL_GET_ORDER_HISTORY = "getSlipDetail";
		/// <summary>Cross point api connection url get point change reason setting</summary>
		public const string CROSS_POINT_URL_GET_POINT_CHANGE_REASON_SETTING = "getPointChangeReasonSetting";
		/// <summary>Cross point api connection url update recovery purchase point by detail</summary>
		public const string CROSS_POINT_URL_UPDATE_RECOVERY_PURCHASE_POINT_BY_DETAIL = "updRecoveryPurchasePointByDetail";
		/// <summary>Cross point api connection url update point</summary>
		public const string CROSS_POINT_URL_UPDATE_POINT = "updPoint";

		/// <summary>CROSS POINT 認証パラメータテナントコード</summary>
		public const string CROSS_POINT_PARAM_AUTH_TENANT_CODE = "tenantCd";
		/// <summary>CROSS POINT 認証ショップコード</summary>
		public const string CROSS_POINT_PARAM_AUTH_SHOP_CODE = "shopCd";
		/// <summary>CROSS POINT MemberInfoパラメータ 会員ID</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_MEMBER_ID = "memberId";
		/// <summary>CROSS POINT MemberInfoパラメータ PINコード</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_PIN_CODE = "pinCd";
		/// <summary>CROSS POINT MemberInfoパラメータ ネットショップ会員ID</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_NET_SHOP_MEMBER_ID = "netShopMemberId";
		/// <summary>CROSS POINT MemberInfoパラメータ 名前（氏）</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_LAST_NAME = "lastName";
		/// <summary>CROSS POINT MemberInfoパラメータ 名前（名）</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_FIRST_NAME = "firstName";
		/// <summary>CROSS POINT MemberInfoパラメータ 名前カナ（氏）</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_LAST_NAME_PHONETIC = "lastNameKana";
		/// <summary>CROSS POINT MemberInfoパラメータ 名前カナ（名）</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_FIRST_NAME_PHONETIC = "firstNameKana";
		/// <summary>CROSS POINT MemberInfo member name</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_MEMBER_NAME = "memberRankName";
		/// <summary>CROSS POINT MemberInfoパラメータ 性別</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_SEX = "sex";
		/// <summary>CROSS POINT MemberInfoパラメータ 生年月日</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_BIRTHDAY = "birthday";
		/// <summary>CROSS POINT MemberInfoパラメータ 郵便番号</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_POSTCODE = "postcode";
		/// <summary>CROSS POINT MemberInfoパラメータ 都道府県名</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_PREF_NAME = "prefName";
		/// <summary>CROSS POINT MemberInfoパラメータ 市区町村</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_CITY = "city";
		/// <summary>CROSS POINT MemberInfoパラメータ 町域</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_TOWN = "town";
		/// <summary>CROSS POINT MemberInfoパラメータ 番地</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_ADDRESS = "address";
		/// <summary>CROSS POINT MemberInfoパラメータ ビル等</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_BUILDING = "building";
		/// <summary>CROSS POINT MemberInfoパラメータ 電話番号</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_TEL = "tel";
		/// <summary>CROSS POINT MemberInfoパラメータ 携帯電話</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_MB_TEL = "mbTel";
		/// <summary>CROSS POINT MemberInfoパラメータ PCメールアドレス</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_PC_MAIL = "pcMail";
		/// <summary>CROSS POINT MemberInfoパラメータ 携帯メールアドレス</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_MB_MAIL = "mbMail";
		/// <summary>CROSS POINT MemberInfoパラメータ 郵便DM不要フラグ</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_POSTCARD_DM_UNNECESSARY_FLG = "postcardDmUnnecessaryFlg";
		/// <summary>CROSS POINT MemberInfoパラメータ メールDM不要フラグ</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_EMAIL_DM_UNNECESSARY_FLG = "emailDmUnnecessaryFlg";
		/// <summary>CROSS POINT MemberInfoパラメータ パスワード</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_PASSWORD = "password";
		/// <summary>CROSS POINT MemberInfoパラメータ 備考1</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_REMARKS_1 = "remarks1";
		/// <summary>CROSS POINT MemberInfoパラメータ 退会会員除外フラグ</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_EXCLUDE_WITHDRAW = "excludeWithdraw";
		/// <summary>CROSS POINT MemberInfoパラメータ 削除対象カード番号</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_DEL_REAL_SHOP_CARD_NO = "delRealShopCardNo";
		/// <summary>CROSS POINT MemberInfoパラメータ 削除対象PINコード</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_DEL_PIN_CODE = "delPinCd";
		/// <summary>CROSS POINT MemberInfoパラメータ 更新用カード番号</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_NEW_REAL_SHOP_CARD_NO = "realShopCardNo";
		/// <summary>CROSS POINT MemberInfoパラメータ 更新用PINコード</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_NEW_PIN_CODE = "pinCd";
		/// <summary>CROSS POINT MemberInfoパラメータ 結合用ベースネットショップ会員ID</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_BASE_NET_SHOP_MEMBER_ID = "baseNetShopMemberId";
		/// <summary>CROSS POINT MemberInfoパラメータ 結合用ベース会員ID置き換え</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_BASE_MEMBER_ID_REPLACE = "baseMemberIdReplacement";
		/// <summary>CROSS POINT MemberInfoパラメータ 会員退会</summary>
		public const string CROSS_POINT_PARAM_MEMBER_INFO_BASE_DEL_MEMBER = "delMember";

		/// <summary>CROSS POINT 発行ポイントパラメータ 会員ID</summary>
		public const string CROSS_POINT_PARAM_POINT_MEMBER_ID = "memberId";
		/// <summary>CROSS POINT 発行ポイントパラメータ 伝票日付</summary>
		public const string CROSS_POINT_PARAM_POINT_SLIP_DATE = "slipDate";
		/// <summary>CROSS POINT 発行ポイントパラメータ POS番号</summary>
		public const string CROSS_POINT_PARAM_POINT_POS_NO = "posNo";
		/// <summary>CROSS POINT 発行ポイントパラメータ 伝票番号</summary>
		public const string CROSS_POINT_PARAM_POINT_SLIP_NO = "slipNo";
		/// <summary>CROSS POINT 発行ポイントパラメータ ポイントID</summary>
		public const string CROSS_POINT_PARAM_POINT_POINT_ID = "pointId";
		/// <summary>CROSS POINT 発行ポイントパラメータ 基本付与ポイント</summary>
		public const string CROSS_POINT_PARAM_POINT_BASE_GRANT_POINT = "baseGrantPoint";
		/// <summary>CROSS POINT 発行ポイントパラメータ 特別付与ポイント</summary>
		public const string CROSS_POINT_PARAM_POINT_SPECIAL_GRANT_POINT = "specialGrantPoint";
		/// <summary>CROSS POINT 発行ポイントパラメータ 利用ポイント</summary>
		public const string CROSS_POINT_PARAM_POINT_USE_POINT = "usePoint";
		/// <summary>CROSS POINT 発行ポイントパラメータ 購入金額合計（税込）</summary>
		public const string CROSS_POINT_PARAM_POINT_AMOUNT_TOTAL_IN_TAX = "amountTotalInTax";
		/// <summary>CROSS POINT 発行ポイントパラメータ 購入金額合計（税抜）</summary>
		public const string CROSS_POINT_PARAM_POINT_AMOUNT_TOTAL_NO_TAX = "amountTotalNoTax";
		/// <summary>CROSS POINT 発行ポイントパラメータ 明細数</summary>
		public const string CROSS_POINT_PARAM_POINT_DETAIL_COUNT = "detailNum";
		/// <summary>CROSS POINT 発行ポイントパラメータ ポイントID不要</summary>
		public const string CROSS_POINT_PARAM_POINT_NO_POINT_ID_FLG = "noPointIdFlg";
		/// <summary>CROSS POINT 発行ポイントパラメータ JANコード</summary>
		public const string CROSS_POINT_PARAM_POINT_JAN_CODE = "JanCd";
		/// <summary>CROSS POINT 発行ポイントパラメータ 商品名</summary>
		public const string CROSS_POINT_PARAM_POINT_ITEM_NAME = "ItemName";
		/// <summary>CROSS POINT 発行ポイントパラメータ 商品コード</summary>
		public const string CROSS_POINT_PARAM_POINT_ITEM_CODE = "ItemCd";
		/// <summary>CROSS POINT 発行ポイントパラメータ 単価</summary>
		public const string CROSS_POINT_PARAM_POINT_UNIT_PRICE = "unitPrice";
		/// <summary>CROSS POINT 発行ポイントパラメータ 販売単価</summary>
		public const string CROSS_POINT_PARAM_POINT_SALES_UNIT_PRICE = "salesUnitPrice";
		/// <summary>CROSS POINT 発行ポイントパラメータ 販売数量</summary>
		public const string CROSS_POINT_PARAM_POINT_QUANTITY = "salesNum";
		/// <summary>CROSS POINT 発行ポイントパラメータ 税額</summary>
		public const string CROSS_POINT_PARAM_POINT_TAX = "tax";
		/// <summary>CROSS POINT 発行ポイントパラメータ 担当者コード</summary>
		public const string CROSS_POINT_PARAM_POINT_USER_CODE = "userCd";
		/// <summary>CROSS POINT 発行ポイントパラメータ ポイント</summary>
		public const string CROSS_POINT_PARAM_POINT_POINT = "point";
		/// <summary>CROSS POINT 発行ポイントパラメータ 理由 ID</summary>
		public const string CROSS_POINT_PARAM_POINT_REASON = "reason";
		/// <summary>Cross point param point unit price intax</summary>
		public const string CROSS_POINT_PARAM_POINT_UNIT_PRICE_INTAX = "UnitPriceInTax";
		/// <summary>Cross point param point unit price no tax</summary>
		public const string CROSS_POINT_PARAM_POINT_UNIT_PRICE_NO_TAX = "UnitPriceNoTax";
		/// <summary>Cross point param point sales num</summary>
		public const string CROSS_POINT_PARAM_POINT_SALES_NUM = "SalesNum";
		/// <summary>CROSS POINT 発行ポイントパラメータ 商品外売上区分</summary>
		public const string CROSS_POINT_PARAM_POINT_ITEM_KBN = "notItemSalesDiv";

		/// <summary>CROSS POINT PointHistoryInfoパラメータ 会員ID</summary>
		public const string CROSS_POINT_PARAM_POINT_HISTORY_INFO_MEMBER_ID = "memberId";
		/// <summary>CROSS POINT PointHistoryInfoパラメータ 取得開始番号</summary>
		public const string CROSS_POINT_PARAM_POINT_HISTORY_INFO_START_NO = "startNo";
		/// <summary>CROSS POINT PointHistoryInfoパラメータ 取得終了番号</summary>
		public const string CROSS_POINT_PARAM_POINT_HISTORY_INFO_END_NO = "endNo";
		/// <summary>CROSS POINT PointHistoryInfoパラメータ 順序</summary>
		public const string CROSS_POINT_PARAM_POINT_HISTORY_INFO_SORT = "sort";
		/// <summary>CROSS POINT PointHistoryInfo 順序フラグ：直近→過去</summary>
		public const string CROSS_POINT_PARAM_POINT_HISTORY_INFO_SORT_DESC = "1";
		/// <summary>CROSS POINT PointHistoryInfo 順序フラグ：過去→直近</summary>
		public const string CROSS_POINT_PARAM_POINT_HISTORY_INFO_SORT_ASC = "2";

		/// <summary>CROSS POINT OrderHistoryパラメータ 会員ID</summary>
		public const string CROSS_POINT_PARAM_ORDER_HISTORY_MEMBER_ID = "memberId";
		/// <summary>CROSS POINT OrderHistoryパラメータ 注文日時(開始)</summary>
		public const string CROSS_POINT_PARAM_ORDER_HISTORY_START_DATE = "startSlipDate";
		/// <summary>CROSS POINT OrderHistoryパラメータ 注文日時(終了)</summary>
		public const string CROSS_POINT_PARAM_ORDER_HISTORY_END_DATE = "endSlipNoDate";
		/// <summary>CROSS POINT OrderHistoryパラメータ 削除フラグ</summary>
		public const string CROSS_POINT_PARAM_ORDER_HISTORY_DELETE_FLG = "deleteFlg";
		/// <summary>CROSS POINT OrderHistoryパラメータ 受注店舗コード</summary>
		public const string CROSS_POINT_PARAM_ORDER_HISTORY_SHOP_CODE = "orderShopCd";
		/// <summary>CROSS POINT OrderHistoryパラメータ 取得開始番号</summary>
		public const string CROSS_POINT_PARAM_ORDER_HISTORY_START_NO = "startNo";
		/// <summary>CROSS POINT OrderHistoryパラメータ 取得最終番号</summary>
		public const string CROSS_POINT_PARAM_ORDER_HISTORY_END_NO = "endNo";
		/// <summary>CROSS POINT OrderHistoryパラメータ 並び順</summary>
		public const string CROSS_POINT_PARAM_ORDER_HISTORY_CONDITION = "condition";
		/// <summary>CROSS POINT OrderHistoryパラメータ ショップ区分</summary>
		public const string CROSS_POINT_PARAM_ORDER_HISTORY_SHOP_DIVISION = "shopDiv";
		/// <summary>CROSS POINT OrderHistoryパラメータ 注文番号</summary>
		public const string CROSS_POINT_PARAM_ORDER_HISTORY_ORDER_ID = "cpUniqueSlipNo";

		/// <summary>CROSS POINT MemberInfoパラメータ ネットショップ会員ID</summary>
		public const string CROSS_POINT_ELEMENT_MEMBER_INFO_NET_SHOP_MEMBER_ID = "NetShopMemberId";
		/// <summary>CROSS POINT OrderHistoryパラメータ 商品詳細</summary>
		public const string CROSS_POINT_ELEMENT_ORDER_HISTORY_DETAIL = "DetailNo";

		/// <summary>CROSS POINT ポイント更新パラメータ ポイント</summary>
		public const string CROSS_POINT_UPDATE_POINT = "point";

		/// <summary>CROSS POINT API処理結果：成功</summary>
		public const string CROSS_POINT_RESULT_STATUS_SUCCESS = "success";
		/// <summary>CROSS POINT API処理結果：失敗</summary>
		public const string CROSS_POINT_RESULT_STATUS_ERROR = "error";

		/// <summary>CROSS POINT API処理結果：エラー購買情報が特定できない</summary>
		public const string CROSS_POINT_RESULT_MODIFY_ERROR_NO_SLIP = "36700033";
		/// <summary>CROSS POINT API処理結果：エラー購買情報が特定できない</summary>
		public const string CROSS_POINT_RESULT_DELETE_ERROR_NO_SLIP = "36100008";
		/// <summary>CROSS POINT NO RESULT ERROR CODE</summary>
		public static string CROSS_POINT_NO_RESULT_ERROR_CODE = "00002";
		/// <summary>Error code: duplicate member id</summary>
		public const string CROSS_POINT_RESULT_DUPLICATE_MEMBER_ID_ERROR_CODE = "45500001";

		/// <summary>CROSS POINT 退会会員除外する</summary>
		public const string CROSS_POINT_FLG_EXCLUDE_WITHDRAW_EXCLUDE = "1";
		/// <summary>CROSS POINT カード更新時ベース会員IDを削除会員IDで置き換える</summary>
		public const string CROSS_POINT_FLG_BASE_MEMBER_ID_REPLACE = "1";
		/// <summary>CROSS POINT メール配信希望：希望する</summary>
		public const string CROSS_POINT_FLG_USER_MAIL_FLG_OK = "0";
		/// <summary>CROSS POINT メール配信希望：希望しない</summary>
		public const string CROSS_POINT_FLG_USER_MAIL_FLG_NG = "1";
		/// <summary>CROSS POINT 有効ポイントフラグ：仮付与ポイント</summary>
		public const string CROSS_POINT_FLG_POINT_TYPE_TEMP = "0";
		/// <summary>CROSS POINT 有効ポイントフラグ：有効ポイント</summary>
		public const string CROSS_POINT_FLG_POINT_TYPE_COMP = "1";
		/// <summary>CROSS POINT 退会フラグ：退会しない</summary>
		public const string CROSS_POINT_FLG_WITHDRAWAL_OFF = "0";
		/// <summary>CROSS POINT 退会フラグ：退会する</summary>
		public const string CROSS_POINT_FLG_WITHDRAWAL_ON = "1";

		/// <summary>Cross point request param member id</summary>
		public const string CROSS_POINT_REQUEST_PARAM_MEMBER_ID = "MEMBER_ID";
		/// <summary>Cross point request param pin cd</summary>
		public const string CROSS_POINT_REQUEST_PARAM_PIN_CD = "PIN_CD";
		/// <summary>Cross point request param app key</summary>
		public const string CROSS_POINT_REQUEST_PARAM_APP_KEY = "APP_KEY";

		/// <summary>Error message: CrossPoint error message storage</summary>
		public const string SESSION_KEY_CROSSPOINT_ERROR_MESSAGE = "w2cFront_crosspoint_error_message";
		/// <summary>Error message: CrossPoint API linkage error</summary>
		public static string ERRMSG_CROSSPOINT_LINKAGE_ERROR = "ERRMSG_CROSSPOINT_LINKAGE_ERROR";

		/// <summary>CROSS POINT 理由区分：新規登録ポイント発行</summary>
		public static string CROSS_POINT_REASON_KBN_REGISTER = "02";
		/// <summary>CROSS POINT 理由区分：ログイン毎ポイント発行</summary>
		public static string CROSS_POINT_REASON_KBN_LOGIN = "03";
		/// <summary>CROSS POINT 理由区分：ポイント利用変更（次回定期購入）</summary>
		public static string CROSS_POINT_REASON_KBN_MODIFY = "12";
		/// <summary>CROSS POINT 理由区分：汎用ポイントルール</summary>
		public static string CROSS_POINT_REASON_KBN_POINT_RULE = "90";
		/// <summary>CROSS POINT 理由区分：誕生日ポイント</summary>
		public static string CROSS_POINT_REASON_KBN_BIRTHDAY = "91";
		/// <summary>CROSS POINT 理由区分：レビュー投稿ポイント</summary>
		public static string CROSS_POINT_REASON_KBN_REVIEW = "92";
		/// <summary>CROSS POINT 理由区分：マスタアップロード</summary>
		public static string CROSS_POINT_REASON_KBN_UPLOAD = "97";
		/// <summary>CROSS POINT 理由区分：オペレータポイント調整</summary>
		public static string CROSS_POINT_REASON_KBN_OPERATOR = "99";

		/// <summary>CROSS POINT 商品外売上区分：商品</summary>
		public const string CROSS_POINT_FLG_ITEM_KBN_PRODUCT = "0";
		/// <summary>CROSS POINT 商品外売上区分：商品外</summary>
		public const string CROSS_POINT_FLG_ITEM_KBN_NOT_PRODUCT = "1";

		/// <summary>ポイント：ポイント利用時の最低購入金額</summary>
		public static decimal POINT_MINIMUM_PURCHASEPRICE = 0;

		/// <summary>2段階認証オプション（利用：TRUE 非利用：FALSE)</summary>
		public static bool TWO_STEP_AUTHENTICATION_OPTION_ENABLED = false;
		/// <summary>2段階認証有効期限(日)</summary>
		public static int TWO_STEP_AUTHENTICATION_EXPIRATION_DATE = 0;
		/// <summary>2段階認証コード有効期限(分)</summary>
		public static int TWO_STEP_AUTHENTICATION_DEADLINE = 0;
		/// <summary>二段階認証を行わないIPアドレス</summary>
		public static string TWO_STEP_AUTHENTICATION_EXCLUSION_IPADDRESS = string.Empty;
		/// <summary>2段階認証コード有効期限(分)</summary>
		public static string CONST_AUTHENTICATION_DEADLINE = "authentication_deadline";
		/// <summary>Authentication code</summary>
		public static string CONST_AUTHENTICATION_CODE = "auth_code";

		/// <summary>Enter key code</summary>
		public const string CONST_ENTER_KEYCODE = "13";
		/// <summary>ログイン失敗制限回数</summary>
		public const int ERROR_LOGIN_LIMITED_COUNT = 3;

		/// <summary>Payment boku option enabled</summary>
		public static bool PAYMENT_BOKU_OPTION_ENABLED = false;
		/// <summary>Payment boku optin url</summary>
		public static string PAYMENT_BOKU_OPTIN_URL = string.Empty;
		/// <summary>Payment boku validate optin url</summary>
		public static string PAYMENT_BOKU_VALIDATE_OPTIN_URL = string.Empty;
		/// <summary>Payment boku confirm optin url</summary>
		public static string PAYMENT_BOKU_CONFIRM_OPTIN_URL = string.Empty;
		/// <summary>Payment boku charge url</summary>
		public static string PAYMENT_BOKU_CHARGE_URL = string.Empty;
		/// <summary>Payment boku query charge url</summary>
		public static string PAYMENT_BOKU_QUERY_CHARGE_URL = string.Empty;
		/// <summary>Payment boku reverse charge url</summary>
		public static string PAYMENT_BOKU_REVERSE_CHARGE_URL = string.Empty;
		/// <summary>Payment boku refund charge url</summary>
		public static string PAYMENT_BOKU_REFUND_CHARGE_URL = string.Empty;
		/// <summary>Payment boku query refund url</summary>
		public static string PAYMENT_BOKU_QUERY_REFUND_URL = string.Empty;
		/// <summary>Payment boku check eligibility url</summary>
		public static string PAYMENT_BOKU_CANCEL_OPTIN_URL = string.Empty;
		/// <summary>Payment boku merchant id</summary>
		public static string PAYMENT_BOKU_MERCHANT_ID = string.Empty;
		/// <summary>Payment boku merchant request id</summary>
		public static string PAYMENT_BOKU_MERCHANT_REQUEST_ID = string.Empty;
		/// <summary>Payment boku key id</summary>
		public static string PAYMENT_BOKU_KEY_ID = string.Empty;
		/// <summary>Payment boku api key</summary>
		public static string PAYMENT_BOKU_API_KEY = string.Empty;
		/// <summary>Payment boku merchant consumer id</summary>
		public static string PAYMENT_BOKU_MERCHANT_CONSUMER_ID = string.Empty;
		/// <summary>Payment boku time out request</summary>
		public static int PAYMENT_BOKU_TIMEOUT_REQUEST = 0;
		/// <summary>Payment boku skip retry flag</summary>
		public static bool PAYMENT_BOKU_SKIP_RETRY_FLG = false;
		/// <summary>Payment boku send notification flag</summary>
		public static bool PAYMENT_BOKU_SEND_NOTIFICATION_FLG = false;
		/// <summary>Boku payment query limit time</summary>
		public static int BOKU_PAYMENT_QUERY_LIMIT_TIME = 0;

		/// <summary>月間隔日付指定 日付パラメータ [月末] Text部分</summary>
		public const int DATE_PARAM_END_OF_MONTH_TEXT = 31;
		/// <summary>月間隔日付指定 日付パラメータ [月末] value部分</summary>
		public const string DATE_PARAM_END_OF_MONTH_VALUE = "-1";

		/// <summary>Facebook catalog api 連携のON/OFFの設定</summary>
		public static bool FACEBOOK_CATALOG_API_COOPERATION_OPTION_ENABLED = false;
		/// <summary>Facebook catalog api url</summary>
		public static string FACEBOOK_CATALOG_API_URL = string.Empty;
		/// <summary>Facebook catalog api version</summary>
		public static string FACEBOOK_CATALOG_API_VERSION = string.Empty;
		/// <summary>Facebook catalog api</summary>
		public static string FACEBOOK_CATALOG_API = "FacebookAPI";
		/// <summary>Facebook catalog api logfile threshold</summary>
		public static int FACEBOOK_CATALOG_API_LOGFILE_THRESHOLD = 100;

		/// <summary>Order workflow limit update order status enabled</summary>
		public static bool ORDERWORKFLOW_LIMIT_UPDATEORDERSTATUS_ENABLED = false;

		/// <summary>決済：楽天コンビニ前払い：サブサービスID(セブンイレブン)</summary>
		public static string PAYMENT_RAKUTENCVSDEF_SUBSERVICEID_SEVEN = string.Empty;
		/// <summary>決済：楽天コンビニ前払い：サブサービスID(イーコン)</summary>
		public static string PAYMENT_RAKUTENCVSDEF_SUBSERVICEID_ECON = string.Empty;
		/// <summary>決済：楽天コンビニ前払い：サブサービスID(ウェルネット)</summary>
		public static string PAYMENT_RAKUTENCVSDEF_SUBSERVICEID_WELLNET = string.Empty;
		/// <summary>決済：楽天コンビニ前払い：支払期限日数</summary>
		public static string PAYMENT_RAKUTEN_CVS_RECEIPT_DISPLAY_NAME = string.Empty;
		/// <summary>Rakutenコンビニ前払い与信期限切れ(X日後有効期限切れ)  ※最終与信日を含まない</summary>
		public static int PAYMENT_RAKUTEN_CVSDEF_AUTHLIMITDAY = 0;
		/// <summary>Rakuten convenience type</summary>
		public static string PAYMENT_RAKUTEN_CVS_TYPE = "rakuten_cvs_type";
		/// <summary>Payment rakuten cvs mock</summary>
		public static bool PAYMENT_RAKUTEN_CVS_MOCK_OPTION_ENABLED = false;
		/// <summary>Payment rakuten cvs environment: api url authentication mock</summary>
		public static string PAYMENT_RAKUTEN_CVS_APIURL_AUTH_MOCK = string.Empty;

		/// <summary>定期購入時に初回配送日を注文日の翌月以降から選択する機能</summary>
		public static bool FIXED_PURCHASE_FIRST_SHIPPING_DATE_NEXT_MONTH_ENABLED = false;

		/// <summary>商品付帯情報の選択肢に価格情報を持たせるオプション</summary>
		public static bool PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED = false;
		/// <summary>オプション価格含まれる価格キー</summary>
		public const string KEY_OPTION_INCLUDED_ITEM_PRICE = "option_included_item_price";
		/// <summary>付帯情報のオプション価格ありの検索用正規表現（画面表示から抽出）</summary>
		/// <remarks>画面に表示されている付帯情報から、「sample(+\1,000)」表記の付帯情報が存在するか検索する際に使用</remarks>
		public const string REGEX_PATTERN_OPTION_PRICE_EXPRESSION = "^.+\\(.+\\)$";
		/// <summary>付帯情報のオプション価格ありの検索用正規表現（DBから抽出）</summary>
		/// <remarks>DBに保存されている付帯情報から、「sample{{1000}}」表記の付帯情報が存在するか検索する際に使用</remarks>
		public const string REGEX_PATTERN_OPTION_PRICE_EXPRESSION_RAW = "^.+\\{\\{.+\\}\\}$";
		/// <summary>付帯情報のオプション価格を抽出する正規表現</summary>
		/// <remarks>「sample{{1000}}」表記の付帯情報から、価格部分を抽出するする際に使用</remarks>
		public const string REGEX_PATTERN_OPTION_PRICE_SEARCH_PATTERN = @"\{\{[0-9]+\}\}";
		/// <summary>付帯情報の正規表現</summary>
		/// <remarks>付帯情報を形式など考慮せず抽出するする際に使用</remarks>
		public const string REGEX_PATTERN_PRODUCT_OPTION_BASIC = @"\[\[.*?\]\]";
		/// <summary>厳格な付帯情報の正規表現</summary>
		/// <remarks>付帯情報が形式に一致しているか判定する際に使用</remarks>
		public const string REGEX_PATTERN_PRODUCT_OPTION_STRICT = @"\[\[[C|S]@@[^@\(\)\-]+(@@[^@\(\)\-]+)+\]\]|\[\[(CP|SP)@@[^@\(\)\-]+(@@[^@\(\)\-]+{{[0-9]+}})+\]\]|\[\[T@@[^@\(\)\-]+(@@[^@\(\)\-]+=[^@\(\)\-]+)*\]\]";
		/// <summary>付帯情報に＠が含まれているか</summary>
		public const string REGEX_PATTERN_PRODUCT_OPTION_INCLUDE_ATMARK = @"\[\[[A-Z]+@@[^@]+((@@[^@]+)*)\]\]";
		/// <summary>付帯情報先頭文字</summary>
		public const string PRODUCT_OPTION_PREFIX_CHARACTER = "[[";
		/// <summary>付帯情報終端文字</summary>
		public const string PRODUCT_OPTION_TERMINATING_CHARACTER = "]]";
		/// <summary>付帯情報DB保持時の価格先頭文字</summary>
		public const string PRODUCT_OPTION_PRICE_PREFIX_FOR_DB = "{{";
		/// <summary>付帯情報DB保持時の価格末尾文字</summary>
		public const string PRODUCT_OPTION_PRICE_SUFFIX_FOR_DB = "}}";

		/// <summary>ギフト遷移短縮機能</summary>
		public static bool SHORTENING_GIFT_OPTION_ENABLED = false;
		/// <summary>Gift order option with shortening gift option enabled</summary>
		public static bool GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED = false;

		/// <summary>決済：ZEUSコンビニ決済：加盟店IPコード</summary>
		public static string PAYMENT_CVS_ZUES_CLIENT_IP = string.Empty;
		/// <summary>決済：ZEUSコンビニ決済：テスト決済フラグ</summary>
		public static bool PAYMENT_CVS_ZUES_PAYMENT_TEST_FLAG = false;
		/// <summary>決済：ZEUSコンビニ決済：テストID</summary>
		public static string PAYMENT_CVS_ZUES_TEST_ID = string.Empty;
		/// <summary>決済：ZEUSコンビニ決済：テスト決済処理区分</summary>
		public static string PAYMENT_CVS_ZUES_TEST_TYPE = "1";
		/// <summary>Zeus convenience type</summary>
		public const string PAYMENT_ZEUS_CVS_TYPE = "zeus_cvs_type";

		/// <summary>楽天モール連携SKU移行状態</summary>
		public static bool MALLCOOPERATION_RAKUTEN_SKUMIGRATION = false;
		/// <summary>楽天モール連携 ファイル転送時のSFTP利用</summary>
		public static bool MALLCOOPERATION_RAKUTEN_USE_SFTP = false;
		/// <summary>SKUレベル行ヘッダ</summary>
		public const string SKU_LEVEL_HEADER = "SKU管理番号";

		/// <summary>店舗受取オプション</summary>
		public static bool STORE_PICKUP_OPTION_ENABLED = false;
		/// <summary>店舗受取時選択可能決済種別ID(カンマ区切り)</summary>
		public static List<string> SETTING_CAN_STORE_PICKUP_OPTION_PAYMENT_IDS = new List<string>();
		/// <summary>Setting cannot store pickup option payment ids</summary>
		public static List<string> SETTING_CAN_NOT_STORE_PICKUP_OPTION_PAYMENT_IDS = new List<string>
		{
			FLG_PAYMENT_PAYMENT_ID_COLLECT,
			FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
			FLG_PAYMENT_PAYMENT_ID_BANK_DEF,
			FLG_PAYMENT_PAYMENT_ID_POST_DEF,
			FLG_PAYMENT_PAYMENT_ID_DSK_DEF
		};
		/// <summary>マスタアップロード/ダウンロード時区切り文字</summary>
		public static string MASTERFILEIMPORT_DELIMITER = string.Empty;
		/// <summary>返品の注文を除外するフラグ</summary>
		public static string EXCLUDE_RETURN_FLG = "exclude_return_flg";

		/// <summary>CROSSMALL連携オプション</summary>
		public static bool CROSS_MALL_OPTION_ENABLED = false;
		/// <summary>CROSSMALL連携 ： アカウント(会社コード)</summary>
		public static string CROSS_MALL_ACCOUNT = string.Empty;
		/// <summary>CROSSMALL連携 ： 認証鍵</summary>
		public static string CROSS_MALL_AUTHENTICATION_KEY = string.Empty;
		/// <summary>CROSSMALL連携 ： 注文伝票情報取得API</summary>
		public static string CROSS_MALL_GET_ORDER_API_URL = string.Empty;
		/// <summary>CROSSMALL連携 ： API連携時のログ排出設定</summary>
		public static string CROSS_MALL_INTEGRATION_PHASE_NAME_DELIMITER = string.Empty;
		/// <summary>CROSSMALL連携 ： 出荷完了連携処理フェーズ名(区切り文字利用により複数設定可)</summary>
		public static string CROSS_MALL_INTEGRATION_PHASE_NAME = string.Empty;
		/// <summary>CROSSMALL連携 ： API連携時のログ排出設定</summary>
		public static bool CROSS_MALL_INTEGRATION_ENABLE_LOGGING = false;

		/// <summary>Letro連携オプション</summary>
		public static bool LETRO_OPTION_ENABLED = false;
		/// <summary>Letro用の連携認証キー</summary>
		public static string LETRO_API_AUTHENTICATION_KEY = string.Empty;
		/// <summary>Letro用の許可IPアドレス</summary>
		public static List<string> LETRO_ALLOWED_IP_ADDRESS = new List<string>();
		/// <summary>Letro APIの認証キー試行回数</summary>
		public static int POSSIBLE_LETRO_AUTH_KEY_ERROR_COUNT = 0;
		/// <summary>Letro APIの認証キーロックの有効時間(分単位)</summary>
		public static int POSSIBLE_LETRO_AUTH_KEY_LOCK_MINUTES = 0;

		/// <summary>オプション：awoo連携</summary>
		public static bool AWOO_OPTION_ENABLED = false;
		/// <summary>awoo連携ID</summary>
		public static string AWOO_NUNUNIID = "";
		/// <summary>awoo連携Baererトークン</summary>
		public static string AWOO_AUTHENTICATION_BAERER_TOKEN = "";
		/// <summary>awooAPIサーバーURL</summary>
		public static string AWOO_API_SERVER = "";
		/// <summary>PAGE API</summary>
		public static string AWOO_API_PAGE_ACTION = "";
		/// <summary>TAGS API</summary>
		public static string AWOO_API_TAGS_ACTION = "";
		/// <summary>CLASSIFYPRODUCTTYPE API</summary>
		public static string AWOO_API_CLASSIFYPRODUCTTYPE_ACTION = "";
		/// <summary>awoo商品一覧ページに表示する商品数</summary>
		public static int AWOO_PAGE_PRODUCT_LIMIT = 0;
		/// <summary>商品詳細画面のAWOOレコメンド商品の設定</summary>
		public static string[] AWOO_RECOMMEND_DIRECTION = Array.Empty<string>();
		/// <summary>商品詳細画面のおすすめの商品表示数</summary>
		public static int AWOO_PRODUCT_DETAIL_PRODUCT_LIMIT = 0;
		/// <summary>Awooとの連携ログを出力するか</summary>
		public static bool AWOO_LOG_OUTPUT_FLAG = false;
		/// <summary>Awoo ログファイル分割閾値</summary>
		public static int AWOO_API_LOGFILE_THRESHOLD = 100;

		/// <summary>配送料無料適用外機能を利用するかどうか</summary>
		public static bool FREE_SHIPPING_FEE_OPTION_ENABLED = false;

		/// <summary>システム管理の許可IPアドレス</summary>
		public static List<string> ALLOWED_IP_ADDRESS_FOR_SYSTEMSETTINGS = new List<string>();
		/// <summary>バリューテキスト:サイト設定</summary>
		public const string VALUETEXT_PARAM_SITE_CONFIGURATION = "site_configuration";
		/// <summary>バリューテキスト:サイト設定:読み取り区分</summary>
		public const string VALUETEXT_PARAM_SITE_CONFIGURATION_READ_KBN = "read_kbn";
		/// <summary>バリューテキスト:サイト設定:読み取り区分:区分名</summary>
		public const string VALUETEXT_PARAM_SITE_CONFIGURATION_READ_KBN_NAME = "name";

		/// <summary>統合解除対象ユーザーの受注件数が何件以上の場合非同期実行するか</summary>
		public static int ORDER_QUANTITY_TO_EXECUTE_ASYNC_CANCEL_USER_INTEGRATE = 0;

		/// <summary>CreateReportバッチ実行エラーコード：正常終了</summary>
		public const int BATCH_CREATEREPORT_ERRORCODE_COMPLETED = 0;
		/// <summary>CreateReportバッチ実行エラーコード：二重実行</summary>
		public const int BATCH_CREATEREPORT_ERRORCODE_DUPLICATE_EXECUTION = -2;

		/// <summary>Recustomer連携オプション</summary>
		public static bool RECUSTOMER_API_OPTION_ENABLED = false;
		/// <summary>RecustomerストアKey</summary>
		public static string RECUSTOMER_API_STOER_KEY = "";
		/// <summary>Recustomer認証用トークン</summary>
		public static string RECUSTOMER_API_TOKEN = "";
		/// <summary>Recustomer連携OrderImporterURL</summary>
		public static string RECUSTOMER_API_ORDER_IMPORTER_URL = "";
		/// <summary>Recustomerリクエストログ設定</summary>
		public static bool RECUSTOMER_API_REQUEST_LOG_EXPORT_ENABLED = false;
		/// <summary>Recustomer用接頭辞</summary>
		public static string RECUSTOMER_API_PREFIX = "";
		/// <summary>Recustomerログファイル分割閾値</summary>
		public static int RECUSTOMER_API_LOGFILE_THRESHOLD = 100;
		/// <summary>Recustomer連携で利用する注文拡張ステータス番号</summary>
		public static int RECUSTOMER_ORDER_EXTEND_STATUS_NO = 47;

		/// <summary>Paygent convenience type</summary>
		public const string PAYMENT_PAYGENT_CVS_TYPE = "paygent_cvs_type";
		/// <summary>Payment message html</summary>
		public const string PAYMENT_MESSAGE_HTML = "payment_message_html";
		/// <summary>Payment message text</summary>
		public const string PAYMENT_MESSAGE_TEXT = "payment_message_text";

		/// <summary>ペイジェントクレジット コンビニ支払期限(日)</summary>
		public static int PAYMENT_PAYGENT_CVS_PAYMENT_LIMIT_DAY = 0;

		/// <summary>ネットバンキング決済オプション</summary>
		public static bool PAYMENT_NETBANKING_OPTION_ENABLED = false;
		/// <summary>決済区分</summary>
		public static PaymentBanknetKbn? PAYMENT_NETBANKING_KBN = null;

		/// <summary>ATM決済オプション</summary>
		public static bool PAYMENT_ATMOPTION_ENABLED = false;
		/// <summary>決済区分：ATM (Paygent)</summary>
		public static PaymentATMKbn? PAYMENT_ATM_KBN = null;
	}
}
