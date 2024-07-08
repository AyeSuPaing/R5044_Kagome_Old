/*
=========================================================================================================
  Module      : メッセージ定数定義(Messages.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;

namespace w2.App.Common
{
	///*********************************************************************************************
	/// <summary>
	/// メッセージマネージャ
	/// </summary>
	///*********************************************************************************************
	public class CommerceMessages : w2.Common.Util.MessageManager
	{
		//========================================================================
		// 商品系エラー
		//========================================================================
		/// <summary>エラーメッセージ定数：商品販売前エラー</summary>
		public static string ERRMSG_FRONT_PRODUCT_BEFORE_SELL = "ERRMSG_FRONT_PRODUCT_BEFORE_SELL";
		/// <summary>エラーメッセージ定数：商品未販売エラー</summary>
		public static string ERRMSG_FRONT_PRODUCT_NO_SELL = "ERRMSG_FRONT_PRODUCT_NO_SELL";
		/// <summary>エラーメッセージ定数：商品在庫不足エラー</summary>
		public static string ERRMSG_FRONT_PRODUCT_NO_STOCK_BEFORE_CART = "ERRMSG_FRONT_PRODUCT_NO_STOCK_BEFORE_CART";
		/// <summary>エラーメッセージ定数：閲覧可能会員ランクエラー</summary>
		public static string ERRMSG_FRONT_PRODUCT_DISPLAY_MEMBER_RANK = "ERRMSG_FRONT_PRODUCT_DISPLAY_MEMBER_RANK";
		/// <summary>エラーメッセージ定数：購入可能会員ランクエラー</summary>
		public static string ERRMSG_FRONT_PRODUCT_BUYABLE_MEMBER_RANK = "ERRMSG_FRONT_PRODUCT_BUYABLE_MEMBER_RANK";
		/// <summary>エラーメッセージ定数：正会員のみに販売されています</summary>
		public static string ERRMSG_FRONT_PRODUCT_BUYABLE_FIXED_PURCHASE_MEMBER = "ERRMSG_FRONT_PRODUCT_BUYABLE_FIXED_PURCHASE_MEMBER";
		/// <summary>エラーメッセージ定数：商品無効</summary>
		public static string ERRMSG_FRONT_PRODUCT_INVALID = "ERRMSG_FRONT_PRODUCT_INVALID";
		/// <summary>エラーメッセージ定数：商品購入数エラー</summary>
		public static string ERRMSG_FRONT_PRODUCT_OVER_MAXSELLQUANTITY = "ERRMSG_FRONT_PRODUCT_OVER_MAXSELLQUANTITY";
		/// <summary>エラーメッセージ定数：商品在庫不足エラー</summary>
		public static string ERRMSG_FRONT_PRODUCT_NO_STOCK = "ERRMSG_FRONT_PRODUCT_NO_STOCK";
		/// <summary>エラーメッセージ定数：複数商品在庫不足エラー</summary>
		public static string ERRMSG_FRONT_MULTIPLE_PRODUCT_NO_STOCK = "ERRMSG_FRONT_MULTIPLE_PRODUCT_NO_STOCK";
		/// <summary>エラーメッセージ定数：シリアルキー不足エラー</summary>
		public static string ERRMSG_FRONT_PRODUCT_NO_NOSERIAL_KEY = "ERRMSG_FRONT_PRODUCT_NO_NOSERIAL_KEY";
		/// <summary>エラーメッセージ定数：マイページ今すぐ注文時の商品在庫不足エラー</summary>
		public static string ERRMSG_FRONT_PRODUCT_NO_STOCK_ORDER_NOW_FROM_MYPAGE = "ERRMSG_FRONT_PRODUCT_NO_STOCK_ORDER_NOW_FROM_MYPAGE";

		/// <summary>エラーメッセージ定数：商品削除済エラー</summary>
		public static string ERRMSG_FRONT_PRODUCT_DELETE = "ERRMSG_FRONT_PRODUCT_DELETE";
		/// <summary>エラーメッセージ定数：商品セール無効エラー</summary>
		public static string ERRMSG_FRONT_PRODUCT_SALES_INVALID = "ERRMSG_FRONT_PRODUCT_SALES_INVALID";
		/// <summary>エラーメッセージ定数：商品セール変更エラー</summary>
		public static string ERRMSG_FRONT_PRODUCT_SALES_CHANGE = "ERRMSG_FRONT_PRODUCT_SALES_CHANGE";
		/// <summary>エラーメッセージ定数：商品価格変更エラー</summary>
		public static string ERRMSG_FRONT_PRODUCT_PRICE_CHANGE = "ERRMSG_FRONT_PRODUCT_PRICE_CHANGE";
		/// <summary>エラーメッセージ定数：商品オプション価格変更エラー</summary>
		public static string ERRMSG_FRONT_PRODUCT_OPTION_PRICE_CHANGE = "ERRMSG_FRONT_PRODUCT_OPTION_PRICE_CHANGE";
		/// <summary>Error message for product in cart has set limit payment</summary>
		public static string ERRMSG_FRONT_PRODUCT_LIMITED_PAYMENT = "ERRMSG_FRONT_PRODUCT_LIMITED_PAYMENT";
		/// <summary>Error message for product detail has set limit payment</summary>
		public static string ERRMSG_FRONT_PRODUCTDETAIL_LIMITED_PAYMENT_DISPLAYED = "ERRMSG_FRONT_PRODUCTDETAIL_LIMITED_PAYMENT_DISPLAYED";
		/// <summary>Error message for product in cart has limit payment changed</summary>
		public static string ERRMSG_FRONT_PRODUCT_LIMITED_PAYMENT_ERROR = "ERRMSG_FRONT_PRODUCT_LIMITED_PAYMENT_ERROR";

		/// <summary>エラーメッセージ定数：商品セット購入不可エラー</summary>
		public static string ERRMSG_FRONT_PRODUCTSET_PRODUCT_UNBUYABLE = "ERRMSG_FRONT_PRODUCTSET_PRODUCT_UNBUYABLE";
		/// <summary>エラーメッセージ定数：商品セット設定可能数エラー</summary>
		public static string ERRMSG_FRONT_PRODUCTSET_UNBUYABLE_COUNT = "ERRMSG_FRONT_PRODUCTSET_UNBUYABLE_COUNT";
		/// <summary>エラーメッセージ定数：商品セット構成商品数エラー（多い）</summary>
		public static string ERRMSG_FRONT_PRODUCTSET_ITEM_UNBUYABLE_COUNT_HIGH = "ERRMSG_FRONT_PRODUCTSET_ITEM_UNBUYABLE_COUNT_HIGH";
		/// <summary>エラーメッセージ定数：商品セット構成商品数エラー（少ない）</summary>
		public static string ERRMSG_FRONT_PRODUCTSET_ITEM_UNBUYABLE_COUNT_LOW = "ERRMSG_FRONT_PRODUCTSET_ITEM_UNBUYABLE_COUNT_LOW";
		/// <summary>エラーメッセージ定数：商品セット配送種別変更エラー</summary>
		public static string ERRMSG_FRONT_PRODUCTSET_SHIPPING_TYPE_CHANGE = "ERRMSG_FRONT_PRODUCTSET_SHIPPING_TYPE_CHANGE";
		/// <summary>エラーメッセージ定数：商品セットデジタルコンテンツフラグ変更エラー</summary>
		public static string ERRMSG_FRONT_PRODUCTSET_DIGITAL_CONTENTS_FLG_CHANGE = "ERRMSG_FRONT_PRODUCTSET_DIGITAL_CONTENTS_FLG_CHANGE";
		/// <summary>エラーメッセージ定数：セットプロモーション計算対象SKU数超過</summary>
		public static string ERRMSG_FRONT_SETPROMOTION_TARGET_SKUS_OVER = "ERRMSG_FRONT_SETPROMOTION_TARGET_SKUS_OVER";
		/// <summary>エラーメッセージ定数：商品セール無効エラー</summary>
		public const string ERRMSG_FRONT_PRODUCT_SALE_INVALID = "ERRMSG_FRONT_PRODUCT_SALE_INVALID";
		/// <summary>エラーメッセージ定数：商品セール価格変更変更エラー</summary>
		public const string ERRMSG_FRONT_PRODUCT_SALE_PRICE_CHANGED = "ERRMSG_FRONT_PRODUCT_SALE_PRICE_CHANGED";

		/// <summary>エラーメッセージ定数：会員ランク制限でカテゴリ商品が見れないのを注意</summary>
		public static string ERRMSG_FRONT_PRODUCTCATEGORY_MEMBERRANK_CANT_DISPLAY_ERROR = "ERRMSG_FRONT_PRODUCTCATEGORY_MEMBERRANK_CANT_DISPLAY_ERROR";

		//========================================================================
		// 注文系エラー
		//========================================================================
		/// <summary>エラーメッセージ定数：対象の注文が見つからないエラー</summary>
		public static string ERRMSG_FRONT_ORDER_NOT_EXISTED = "ERRMSG_FRONT_ORDER_NOT_EXISTED";

		/// <summary>エラーメッセージ定数：カート不整合エラー</summary>
		public static string ERRMSG_FRONT_CART_NO_ADJUSTMENT = "ERRMSG_FRONT_CART_NO_ADJUSTMENT";
		/// <summary>エラーメッセージ定数：カート投入リクエストのキー不正エラー</summary>
		public static string ERRMSG_FRONT_ADD_CART_HTTPREQUEST = "ERRMSG_FRONT_ADD_CART_HTTPREQUEST";
		/// <summary>エラーメッセージ定数：カート内容変更エラー</summary>
		public static string ERRMSG_FRONT_CART_CHANGED = "ERRMSG_FRONT_CART_CHANGED";
		/// <summary>エラーメッセージ定数：カート内制限決済エラー </summary>
		public static string ERRMSG_FRONT_CART_LIMITED_PAYMENT = "ERRMSG_FRONT_CART_LIMITED_PAYMENT";

		/// <summary>エラーメッセージ定数：配送種別未設定エラー</summary>
		public static string ERRMSG_FRONT_SHOP_SHIPPING_UNFIND = "ERRMSG_FRONT_SHOP_SHIPPING_UNFIND";
		/// <summary>エラーメッセージ定数：商品配送種別変更エラー</summary>
		public static string ERRMSG_FRONT_SHIPPING_TYPE_CHANGE = "ERRMSG_FRONT_SHIPPING_TYPE_CHANGE";
		/// <summary>エラーメッセージ定数：配送希望日範囲外エラー</summary>
		public static string ERRMSG_FRONT_ORDERSHIPPING_NO_TERM = "ERRMSG_FRONT_ORDERSHIPPING_NO_TERM";

		/// <summary>エラーメッセージ定数：配送先未指定エラー</summary>
		public static string ERRMSG_FRONT_SHIPPING_UNSELECTED_ERROR = "ERRMSG_FRONT_SHIPPING_UNSELECTED_ERROR";

		/// <summary>エラーメッセージ定数：お支払い方法未選択エラー</summary>
		public static string ERRMSG_FRONT_PAYMENT_UNSELECTED_ERROR = "ERRMSG_FRONT_PAYMENT_UNSELECTED_ERROR";
		/// <summary>エラーメッセージ定数：お支払い方法対象金額範囲外エラー</summary>
		public static string ERRMSG_FRONT_PAYMENT_USEABLE_PRICE_OUT_OF_RANGE_ERROR = "ERRMSG_FRONT_PAYMENT_USEABLE_PRICE_OUT_OF_RANGE_ERROR";
		/// <summary>エラーメッセージ定数：お支払い方法対象金額オーバーエラー（選択不能）</summary>
		public static string ERRMSG_FRONT_PAYMENT_USEABLE_PRICE_OVER_UNSELECTABLE_ERROR = "ERRMSG_FRONT_PAYMENT_USEABLE_PRICE_OVER_UNSELECTABLE_ERROR";
		/// <summary>エラーメッセージ定数：決済手数料未設定エラー</summary>
		public static string ERRMSG_FRONT_PAYMENT_PRICE_UNFIND = "ERRMSG_FRONT_PAYMENT_PRICE_UNFIND";

		/// <summary>エラーメッセージ定数：カード決済エラー</summary>
		public static string ERRMSG_FRONT_CARDAUTH_ERROR = "ERRMSG_FRONT_CARDAUTH_ERROR";
		/// <summary>エラーメッセージ定数：カード決済詳細エラー</summary>
		public static string ERRMSG_FRONT_CARDAUTH_DETAIL_ERROR = "ERRMSG_FRONT_CARDAUTH_DETAIL_ERROR";

		/// <summary>エラーメッセージ定数：仮クレジットカード向けカード決済エラー（与信枠が足らずエラー。再登録を促す）</summary>
		public static string ERRMSG_FRONT_CARDAUTH_ERROR_FOR_PROVISIONAL_CREDITCARD_AND_URGE_REREGISTRATION = "ERRMSG_FRONT_CARDAUTH_ERROR_FOR_PROVISIONAL_CREDITCARD_AND_URGE_REREGISTRATION";
		/// <summary>エラーメッセージ定数：SBPS決済連携エラー</summary>
		public static string ERRMSG_FRONT_SBPS_PAYMENT_ERROR = "ERRMSG_FRONT_SBPS_PAYMENT_ERROR";
		/// <summary>エラーメッセージ定数：SBPS決済連携エラー（注文完了済み）</summary>
		public static string ERRMSG_FRONT_SBPS_PAYMENT_ERROR_ORDER_COMPLETED = "ERRMSG_FRONT_SBPS_PAYMENT_ERROR_ORDER_COMPLETED";
		/// <summary>エラーメッセージ定数：Zeus決済連携エラー</summary>
		public static string ERRMSG_FRONT_ZEUS_PAYMENT_ERROR = "ERRMSG_FRONT_ZEUS_PAYMENT_ERROR";
		/// <summary>エラーメッセージ定数：Zeus決済連携エラー（注文完了済み）</summary>
		public static string ERRMSG_FRONT_ZEUS_PAYMENT_ERROR_ORDER_COMPLETED = "ERRMSG_FRONT_ZEUS_PAYMENT_ERROR_ORDER_COMPLETED";
		/// <summary>エラーメッセージ定数：コンビニ決済エラー（詳細メッセージなし）</summary>
		public static string ERRMSG_FRONT_CVSAUTH_ERROR = "ERRMSG_FRONT_CVSAUTH_ERROR";
		/// <summary>エラーメッセージ定数：コンビニ決済エラー2（詳細メッセージあり）</summary>
		public static string ERRMSG_FRONT_CVSAUTH_ERROR2 = "ERRMSG_FRONT_CVSAUTH_ERROR2";
		/// <summary>エラーメッセージ定数：コンビニ後払いエラー</summary>
		public static string ERRMSG_FRONT_CVSDEFAUTH_ERROR = "ERRMSG_FRONT_CVSDEFAUTH_ERROR";
		/// <summary>エラーメッセージ定数：ヤマト後払いSMS認証失敗エラー</summary>
		public static string ERRMSG_FRONT_YAMATO_KA_SMS_FAILED = "ERRMSG_FRONT_YAMATO_KA_SMS_FAILED";
		/// <summary>エラーメッセージ定数：ヤマト後払いSMS認証不一致エラー</summary>
		public static string ERRMSG_FRONT_YAMATO_KA_SMS_MISMATCH_CODE = "ERRMSG_FRONT_YAMATO_KA_SMS_DISMATCH_CODE";
		/// <summary>エラーメッセージ定数：ヤマト後払いSMS認証電話番号エラー</summary>
		public static string ERRMSG_FRONT_YAMATO_KA_SMS_TELNUM_INVALID = "ERRMSG_FRONT_YAMATO_KA_SMS_TELNUM_INVALID";
		/// <summary>エラーメッセージ定数：ヤマト後払いSMS認証有効期限切れエラー</summary>
		public static string ERRMSG_FRONT_YAMATO_KA_SMS_EXPIRED = "ERRMSG_FRONT_YAMATO_KA_SMS_EXPIRED";
		/// <summary>エラーメッセージ定数：ヤマト後払いSMS認証数値エラー</summary>
		public static string ERRMSG_FRONT_YAMATO_KA_SMS_INVALID = "ERRMSG_FRONT_YAMATO_KA_SMS_INVALID";
		/// <summary>エラーメッセージ定数：GMOカード決済エラー(取引登録エラー)</summary>
		public static string ERRMSG_FRONT_GMO_CARDAUTH_ERROR_ENTRYTRAN = "ERRMSG_FRONT_GMO_CARDAUTH_ERROR_ENTRYTRAN";
		/// <summary>エラーメッセージ定数：YamatoKwc(クレジットカード取得エラー)</summary>
		public static string ERRMSG_FRONT_YAMATO_KWC_CREDIT_EXPIRED = "ERRMSG_FRONT_YAMATO_KWC_CREDIT_EXPIRED";
		/// <summary>エラーメッセージ定数：定期情報詳細で今すぐ注文押下時のYamatoKwc(クレジットカード取得エラー)</summary>
		public static string ERRMSG_FRONT_YAMATO_KWC_CREDIT_EXPIRED_FOR_FIXED_PURCHASE_DETAIL_ERROR = "ERRMSG_FRONT_YAMATO_KWC_CREDIT_EXPIRED_FOR_FIXED_PURCHASE_DETAIL_ERROR";
		/// <summary>エラーメッセージ定数：外部決済エラー</summary>
		public static string ERRMSG_FRONT_AUTH_ERROR = "ERRMSG_FRONT_AUTH_ERROR";
		/// <summary>エラーメッセージ定数：外部決済エラー（他の決済変更してくれというメッセージ表示）</summary>
		public static string ERRMSG_FRONT_CHANGE_TO_ANOTHER_PAYMENT_FOR_AUTH_ERROR = "ERRMSG_FRONT_CHANGE_TO_ANOTHER_PAYMENT_FOR_AUTH_ERROR";
		/// <summary>エラーメッセージ定数：外部決済エラー(例外）</summary>
		public static string ERRMSG_FRONT_AUTH_EXCEPTION = "ERRMSG_FRONT_AUTH_EXCEPTION";
		/// <summary>エラーメッセージ定数：クレジットカード3Dセキュア未対応</summary>
		public static string ERRMSG_FRONT_THREEDSECURE_UNSUPPORTED = "ERRMSG_FRONT_THREEDSECURE_UNSUPPORTED";
		/// <summary>エラーメッセージ定数：クレジットカード登録限度数オーバーエラー</summary>
		public static string ERRMSG_FRONT_CREDITCARD_REGIST_COUNT_OVER_ERROR = "ERRMSG_FRONT_CREDITCARD_REGIST_COUNT_OVER_ERROR";
		/// <summary>エラーメッセージ定数：GMOコンビニ後払いエラー</summary>
		public static string ERRMSG_FRONT_GMO_CVSDEFAUTH_ERROR = "ERRMSG_FRONT_GMO_CVSDEFAUTH_ERROR";
		/// <summary>エラーメッセージ定数：Atodeneコンビニ後払いエラー</summary>
		public static string ERRMSG_FRONT_ATODENE_CVSDEFAUTH_ERROR = "ERRMSG_FRONT_ATODENE_CVSDEFAUTH_ERROR";
		/// <summary>エラーメッセージ定数：GMOコンビニ前払いエラー</summary>
		public static string ERRMSG_FRONT_GMO_CVSPREAUTH_ERROR = "ERRMSG_FRONT_GMO_CVSPREAUTH_ERROR";
		/// <summary>エラーメッセージ定数：後付款(TriLink後払い)エラー</summary>
		public static string ERRMSG_FRONT_TRYLINKAFTERPAYAUTH_ERROR = "ERRMSG_FRONT_TRYLINKAFTERPAYAUTH_ERROR";
		/// <summary>エラーメッセージ定数：決済種別利用不可</summary>
		public static string ERRMSG_FRONT_LIMITED_PAYMENT_ERROR = "ERRMSG_FRONT_LIMITED_PAYMENT_ERROR";
		/// <summary>エラーメッセージ定数：スコア後払い決済NGエラー</summary>
		public static string ERRMSG_FRONT_SCORE_PAYMENT_NG_ERROR = "ERRMSG_FRONT_SCORE_PAYMENT_NG_ERROR";
		/// <summary>エラーメッセージ定数：スコア後払い決済変更エラー</summary>
		public static string ERRMSG_FRONT_SCORE_PAYMENT_CHANGE_ERROR = "ERRMSG_FRONT_SCORE_PAYMENT_CHANGE_ERROR";
		/// <summary>エラーメッセージ定数：スコア後払い与信中エラー</summary>
		public static string ERRMSG_FRONT_SCORE_CVSDEFAUTH_ERROR = "ERRMSG_FRONT_SCORE_CVSDEFAUTH_ERROR";
		/// <summary>エラーメッセージ定数：スコア後払いキャンセルアラート</summary>
		public static string ERRMSG_FRONT_SCORE_PAYMENT_CANCEL_ALERT = "ERRMSG_FRONT_SCORE_PAYMENT_CANCEL_ALERT";

		/// <summary>エラーメッセージ定数：ベリトランス後払い決済NGエラー</summary>
		public static string ERRMSG_FRONT_VERITRANS_PAYMENT_NG_ERROR = "ERRMSG_FRONT_VERITRANS_PAYMENT_NG_ERROR";
		/// <summary>エラーメッセージ定数：ベリトランス後払い決済変更エラー</summary>
		public static string ERRMSG_FRONT_VERITRANS_PAYMENT_CHANGE_ERROR = "ERRMSG_FRONT_VERITRANS_PAYMENT_CHANGE_ERROR";
		/// <summary>エラーメッセージ定数：ベリトランス後払い与信中エラー</summary>
		public static string ERRMSG_FRONT_VERITRANS_CVSDEFAUTH_ERROR = "ERRMSG_FRONT_VERITRANS_CVSDEFAUTH_ERROR";

		/// <summary>エラーメッセージ定数：注文情報インサートエラー</summary>
		public static string ERRMSG_FRONT_ORDER_INSERT_ERROR = "ERRMSG_FRONT_ORDER_INSERT_ERROR";
		/// <summary>エラーメッセージ定数：注文確定エラー</summary>
		public static string ERRMSG_FRONT_ORDER_STATUS_UPUDATE_ERROR = "ERRMSG_FRONT_ORDER_STATUS_UPUDATE_ERROR";
		/// <summary>エラーメッセージ定数：クレジット登録確定エラー</summary>
		public static string ERRMSG_FRONT_CREDITCARD_REGIST_DETERMINE_ERROR = "ERRMSG_FRONT_CREDITCARD_REGIST_DETERMINE_ERROR";
		/// <summary>エラーメッセージ定数：クレジットカード与信エラー</summary>
		public static string ERRMSG_FRONT_CREDITCARD_AUTH_ERROR = "ERRMSG_FRONT_CREDITCARD_AUTH_ERROR";

		/// <summary>エラーメッセージ定数：注文メール送信アラート</summary>
		public static string ERRMSG_FRONT_ORDER_MAILSEND_ALERT = "ERRMSG_FRONT_ORDER_MAILSEND_ALERT";
		/// <summary>エラーメッセージ定数：セッションポイント更新アラート</summary>
		public static string ERRMSG_FRONT_ORDER_UPDATE_SESSIONPOINT_ALERT = "ERRMSG_FRONT_ORDER_UPDATE_SESSIONPOINT_ALERT";
		/// <summary>エラーメッセージ定数：カートＤＢ削除アラート</summary>
		public static string ERRMSG_FRONT_ORDER_DELETE_CART_DB_ALERT = "ERRMSG_FRONT_ORDER_DELETE_CART_DB_ALERT";
		/// <summary>エラーメッセージ定数：未処理カート存在アラート</summary>
		public static string ERRMSG_FRONT_ORDER_UNSETTLED_CART_ALERT = "ERRMSG_FRONT_ORDER_UNSETTLED_CART_ALERT";
		/// <summary>エラーメッセージ定数：定期購入登録失敗アラート</summary>
		public static string ERRMSG_FRONT_ORDER_REGIST_FIXED_PURCHASE_ALERT = "ERRMSG_FRONT_ORDER_REGIST_FIXED_PURCHASE_ALERT";
		/// <summary>エラーメッセージ定数：定期購入登録全額無料失敗アラート</summary>
		public static string ERRMSG_FRONT_FIXED_PURCHASE_TOTAL_FREE_REGISTRATION_FAILURE_ALERT = "ERRMSG_FRONT_FIXED_PURCHASE_TOTAL_FREE_REGISTRATION_FAILURE_ALERT";

		/// <summary>エラーメッセージ定数：クレジット与信ロックメッセージ</summary>
		public static string ERRMSG_FRONT_CREDIT_AUTH_LOCK = "ERRMSG_FRONT_CREDIT_AUTH_LOCK";

		/// <summary>クレジットトークン有効期限切れメッセージ</summary>
		public static string ERRMSG_CREDIT_TOKEN_EXPIRED = "ERRMSG_CREDIT_TOKEN_EXPIRED";

		/// <summary>クレジット売上確定後キャンセルエラー</summary>
		public static string ERRMSG_CREDIT_SALES_COMPLETE_CANCEL_ERROR = "ERRMSG_CREDIT_SALES_COMPLETE_CANCEL_ERROR";

		/// <summary>エラーメッセージ定数：定期購入情報のキャンセル失敗エラー</summary>
		public static string ERRMSG_FRONT_FIXED_PURCHASE_CANCEL_ALERT = "ERRMSG_FRONT_FIXED_PURCHASE_CANCEL_ALERT";

		/// <summary>エラーメッセージ定数：定期購入情報の休止失敗エラー</summary>
		public static string ERRMSG_FRONT_FIXED_PURCHASE_SUSPEND_ALERT = "ERRMSG_FRONT_FIXED_PURCHASE_SUSPEND_ALERT";

		/// <summary>エラーメッセージ定数：定期会員割引無効エラーメッセージ</summary>
		public static string ERRMSG_FRONT_FIXED_PURCHASE_MEMBER_DISCOUNT_INVAILD = "ERRMSG_FRONT_FIXED_PURCHASE_MEMBER_DISCOUNT_INVAILD";

		/// <summary>エラーメッセージ定数：定期購入配送間隔（日・月）エラー</summary>
		public const string ERRMSG_FRONT_FIXED_PURCHASE_SPECIFIC_MONTH_DATE_INTERVAL_INVALID = "ERRMSG_FRONT_FIXED_PURCHASE_SPECIFIC_MONTH_DATE_INTERVAL_INVALID";

		/// <summary>エラーメッセージ定数：定期購入次回配送日エラー</summary>
		public static string ERRMSG_FRONT_FIXED_PURCHASE_NEXT_SHIPPING_DATE_INVALID = "ERRMSG_FRONT_FIXED_PURCHASE_NEXT_SHIPPING_DATE_INVALID";

		/// <summary>エラーメッセージ定数：定期購入商品バリエーション重複エラー</summary>
		public static string ERRMSG_FRONT_FIXED_PURCHASE_PRODUCT_VARIATION_DUPLICATE = "ERRMSG_FRONT_FIXED_PURCHASE_PRODUCT_VARIATION_DUPLICATE";

		/// <summary>エラーメッセージ定数：注文同梱 注文同梱対象間での配送種別不一致エラー</summary>
		public static string ERRMSG_FRONT_ORDERCOMBINE_SHIPPING_TYPE_DISAGREEMENT = "ERRMSG_FRONT_ORDERCOMBINE_SHIPPING_TYPE_DISAGREEMENT";
		/// <summary>エラーメッセージ定数：注文同梱 注文同梱対象間での頒布会定額コース不一致エラー</summary>
		public static string ERRMSG_FRONT_ORDERCOMBINE_SUBSCRIPTION_BOX_FIXED_AMOUNT_COURSE_DISAGREEMENT = "ERRMSG_FRONT_ORDERCOMBINE_SUBSCRIPTION_BOX_FIXED_AMOUNT_COURSE_DISAGREEMENT";
		/// <summary>エラーメッセージ：頒布会定額コースの注文同梱時のコース制限チェックで不正</summary>
		public const string ERRMSG_ORDERCOMBINE_SUBSCRIPTION_BOX_FIXED_AMOUNT_CHECK_INVALID = "ERRMSG_ORDERCOMBINE_SUBSCRIPTION_BOX_FIXED_AMOUNT_CHECK_INVALID";

		/// <summary>エラーメッセージ定数：デフォルト配送先方法無効エラー</summary>
		public static string ERRMSG_FRONT_USER_DEFAULT_SHIPPING_SETTING_INVALID = "ERRMSG_FRONT_USER_DEFAULT_SHIPPING_SETTING_INVALID";
		/// <summary>エラーメッセージ定数：デフォルト決済方法無効エラー（クレジットカード）</summary>
		public static string ERRMSG_FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_USER_CREDIT_CARD = "ERRMSG_FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_USER_CREDIT_CARD";
		/// <summary>エラーメッセージ定数：デフォルト決済方法無効エラー（後付款(TriLink後払い)）</summary>
		public static string ERRMSG_FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_USER_TRYLINK_AFTERPAY = "ERRMSG_FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_USER_TRYLINK_AFTERPAY";
		/// <summary>エラーメッセージ定数：デフォルト決済方法無効エラー2（後付款(TriLink後払い)）</summary>
		public static string ERRMSG_FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_USER_TRYLINK_AFTERPAY2 = "ERRMSG_FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_USER_TRYLINK_AFTERPAY2";
		/// <summary>エラーメッセージ定数：デフォルト決済方法無効エラー（代引き）</summary>
		public static string ERRMSG_FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_COLLECT = "ERRMSG_FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_COLLECT";
		/// <summary>エラーメッセージ定数：デフォルト決済方法無効エラー（利用不可決済）</summary>
		public static string ERRMSG_FRONT_USER_DEFAULT_LIMITED_PAYMENT = "ERRMSG_FRONT_USER_DEFAULT_LIMITED_PAYMENT";

		/// <summary>エラーメッセージ定数：定期キャリア決済使用不可エラー</summary>
		public static string ERRMSG_FIXED_PURCHASE_PAYMENT_CAREER_ERROR = "ERRMSG_FIXED_PURCHASE_PAYMENT_CAREER_ERROR";

		/// <summary>エラーメッセージ定数：ランディングカート入力画面での購入商品未選択エラー</summary>
		public static string ERRMSG_FRONT_EXIST_PURCHASE_PRODUCT_FOR_LANDING_CART_INPUT = "ERRMSG_FRONT_EXIST_PURCHASE_PRODUCT_FOR_LANDING_CART_INPUT";

		/// <summary>エラーメッセージ定数：再与信を実行する注文時に、再与信でエラー</summary>
		public static string ERRMSG_FRONT_REAUTH_ALERT = "ERRMSG_FRONT_REAUTH_ALERT";

		/// <summary>Error Message Paidy Authorize Error</summary>
		public static string ERRMSG_PAIDY_AUTHORIZE_ERROR = "ERRMSG_PAIDY_AUTHORIZE_ERROR";
		/// <summary>Error Message Country Shipping Not Japan</summary>
		public static string ERRMSG_FRONT_PAIDY_COUNTRY_SHIPPING_NOT_JAPAN_ERROR = "ERRMSG_FRONT_PAIDY_COUNTRY_SHIPPING_NOT_JAPAN_ERROR";

		/// <summary>定期購入解約時の継続課金解約失敗エラー</summary>
		public static string ERRMSG_MANAGE_PAYMENT_CONTINUOUS_CANCEL_NG_FOR_CANCEL_FP =
			"ERRMSG_MANAGE_PAYMENT_CONTINUOUS_CANCEL_NG_FOR_CANCEL_FP";

		/// <summary>エラーメッセージ定数：配送不可エリアエラー</summary>
		public static string ERRMSG_MANAGER_UNAVAILABLE_SHIPPING_AREA_ERROR = "ERRMSG_MANAGER_UNAVAILABLE_SHIPPING_AREA_ERROR";

		//========================================================================
		// AmazonPay系エラー
		//========================================================================
		/// <summary>エラーメッセージ定数：無効なトークン/無効なユーザー情報エラー</summary>
		public static string ERRMSG_FRONT_USER_LOGIN_FOR_AMAZON = "ERRMSG_FRONT_USER_LOGIN_FOR_AMAZON";
		/// <summary>エラーメッセージ定数：無効なトークン/無効なユーザー情報エラー</summary>
		public static string ERRMSG_FRONT_USER_INVALID_TOKEN_FOR_AMAZON = "ERRMSG_FRONT_USER_INVALID_TOKEN_FOR_AMAZON";
		/// <summary>エラーメッセージ定数：無効な名前エラー</summary>
		public static string ERRMSG_FRONT_USER_INVALID_NAME_FOR_AMAZON_ADDRESS_WIDGET = "ERRMSG_FRONT_USER_INVALID_NAME_FOR_AMAZON_ADDRESS_WIDGET";
		/// <summary>エラーメッセージ定数：無効な住所エラー</summary>
		public static string ERRMSG_FRONT_USER_INVALID_ADDRESS_FOR_AMAZON_ADDRESS_WIDGET = "ERRMSG_FRONT_USER_INVALID_ADDRESS_FOR_AMAZON_ADDRESS_WIDGET";
		/// <summary>エラーメッセージ定数：無効な都道府県エラー</summary>
		public static string ERRMSG_FRONT_USER_INVAILD_PREFECTURE_FOR_AMAZON_ADDRESS_WIDGET = "ERRMSG_FRONT_USER_INVAILD_PREFECTURE_FOR_AMAZON_ADDRESS_WIDGET";
		/// <summary>エラーメッセージ定数：既に他のユーザーと連携済</summary>
		public static string ERRMSG_FRONT_USER_COOPERATED_AMAZON_USER = "ERRMSG_FRONT_USER_COOPERATED_AMAZON_USER";

		/// <summary>エラーメッセージ定数：商品購入制限で購入できない(定期商品追加画面)</summary>
		public static string ERRMSG_FRONT_NOT_PRODUCT_ORDER_LIMIT_FOR_FIXED_PURCHASE_PRODUCT_ADD = "ERRMSG_FRONT_NOT_PRODUCT_ORDER_LIMIT_FOR_FIXED_PURCHASE_PRODUCT_ADD";
		/// <summary>エラーメッセージ定数：商品購入制限で購入できない可能性がある(管理画面)</summary>
		public static string ERRMSG_FRONT_NOT_PRODUCT_ORDER_LIMIT = "ERRMSG_FRONT_NOT_PRODUCT_ORDER_LIMIT";
		/// <summary>エラーメッセージ定数：商品購入制限で購入できない可能性がある(カート間)</summary>
		public static string ERRMSG_FRONT_NOT_PRODUCT_ORDER_LIMIT_CART = "ERRMSG_FRONT_NOT_PRODUCT_ORDER_LIMIT_CART";
		/// <summary>エラーメッセージ定数：注文拡張ステータス更新メッセージ</summary>
		public static string ERRMSG_FRONT_UPDATE_EXTENDSTATUS39 = "ERRMSG_FRONT_UPDATE_EXTENDSTATUS39";
		/// <summary>エラーメッセージ定数：注文管理メモ、商品を過去に購入した事がある</summary>
		public static string ERRMSG_FRONT_NOT_FIRSTTIME_CHECK_BY_ORDER = "ERRMSG_FRONT_NOT_FIRSTTIME_CHECK_BY_ORDER";
		/// <summary>エラーメッセージ定数：注文管理メモ、重複した注文IDを表示</summary>
		public static string ERRMSG_FRONT_NOT_FIRSTTIME_CHECK_BY_CART = "ERRMSG_FRONT_NOT_FIRSTTIME_CHECK_BY_CART";
		/// <summary>エラーメッセージ定数：商品購入制限で購入できない可能性がある（フロント画面）</summary>
		public static string ERRMSG_FRONT_NOT_FIXED_PRODUCT_ORDER_LIMIT = "ERRMSG_FRONT_NOT_FIXED_PRODUCT_ORDER_LIMIT";
		/// <summary>エラーメッセージ定数：商品購入制限で購入できない可能性がある（LP画面）</summary>
		public static string ERRMSG_FRONT_NOT_PRODUCT_ORDER_LP = "ERRMSG_FRONT_NOT_PRODUCT_ORDER_LP";
		/// <summary>エラーメッセージ定数：購入制限商品が2つ以上重複してカートに含まれている</summary>
		public static string ERRMSG_FRONT_NOT_PRODUCT_ORDER_BY_CART = "ERRMSG_FRONT_NOT_PRODUCT_ORDER_BY_CART";
		/// <summary>エラーメッセージ定数：購入制限商品が2つ以上重複してカートに含まれている（LP画面）</summary>
		public static string ERRMSG_FRONT_NOT_PRODUCT_ORDER_LP_BY_CART = "ERRMSG_FRONT_NOT_PRODUCT_ORDER_LP_BY_CART";
		/// <summary>エラーメッセージ定数：ユーザー管理レベル制限で定期購入できない可</summary>
		public static string ERRMSG_FRONT_PRODUCT_FIXED_PURCHASE_INVALID = "ERRMSG_FRONT_PRODUCT_FIXED_PURCHASE_INVALID";
		/// <summary>エラーメッセージ定数：定期購入のみ可の商品を同梱しようとした</summary>
		public static string ERRMSG_FRONT_PRODUCT_BUNDLE_ONLY_FIXED_PURCHASE = "ERRMSG_FRONT_PRODUCT_BUNDLE_ONLY_FIXED_PURCHASE";

		/// <summary>エラーメッセージ定数：割引上限超過</summary>
		public static string ERRMSG_MANAGER_DISCOUNT_LIMIT_ERROR = "ERRMSG_MANAGER_DISCOUNT_LIMIT_ERROR";
		/// <summary>エラーメッセージ定数：割引上限超過アラートメッセージ</summary>
		public static string ERRMSG_MANAGER_DISCOUNT_LIMIT_ALERT = "ERRMSG_MANAGER_DISCOUNT_LIMIT_ALERT";

		/// <summary>エラーメッセージ定数：定期購入割引設定入力チェックエラー 回数行</summary>
		public static string ERRMSG_MANAGER_PRODUCT_MASTER_FIXED_PURCHASE_DISCOUNT_ROW_COUNT_EMPTY = "ERRMSG_MANAGER_PRODUCT_MASTER_FIXED_PURCHASE_DISCOUNT_ROW_COUNT_EMPTY";

		/// <summary>エラーメッセージ定数：再与信時のAmazonPayレスポンスエラー</summary>
		public static string ERRMSG_RECREDIT_AMAZONPAY_RESPONSE = "ERRMSG_RECREDIT_AMAZONPAY_RESPONSE";

		//========================================================================
		// W2MP：ポイントオプションメッセージ
		//========================================================================
		/// <summary>エラーメッセージ定数：ポイント限度オーバーエラー</summary>
		public static string ERRMSG_FRONT_POINT_USE_MAX_ERROR = "ERRMSG_FRONT_POINT_USE_MAX_ERROR";
		/// <summary>エラーメッセージ定数：ポイント利用単位エラー</summary>
		public static string ERRMSG_FRONT_POINT_USABLE_UNIT_ERROR = "ERRMSG_FRONT_POINT_USABLE_UNIT_ERROR";
		/// <summary>エラーメッセージ定数：ポイント利用の商品合計金額オーバーエラー</summary>
		public static string ERRMSG_FRONT_POINT_PRICE_SUBTOTAL_MINUS_ERROR = "ERRMSG_FRONT_POINT_PRICE_SUBTOTAL_MINUS_ERROR";
		/// <summary>エラーメッセージ定数：ポイント利用を減らした際の有効期限切れ警告</summary>
		public static string ERRMSG_FRONT_POINT_RETURN_EXPIRED_ALERT = "ERRMSG_FRONT_POINT_RETURN_EXPIRED_ALERT";
		/// <summary>エラーメッセージ定数：ポイント利用を減らした際の有効期限切れエラー</summary>
		public static string ERRMSG_FRONT_POINT_RETURN_EXPIRED_ERROR = "ERRMSG_FRONT_POINT_RETURN_EXPIRED_ERROR";
		/// <summary>エラーメッセージ定数：利用ポイント数を保持ポイント数超えるエラー</summary>
		public static string ERRMSG_FRONT_NEXTSHIPPINGUSEPOINT_CHANGE_OVER_USER_POINT = "ERRMSG_FRONT_NEXTSHIPPINGUSEPOINT_CHANGE_OVER_USER_POINT";
		/// <summary>エラーメッセージ定数：次回購入の利用ポイントの更新失敗エラー</summary>
		public static string ERRMSG_FRONT_NEXT_SHIPPING_USE_POINT_UPDATE_ALERT = "ERRMSG_FRONT_NEXT_SHIPPING_USE_POINT_UPDATE_ALERT";
		/// <summary>エラーメッセージ定数：次回購入ポイント利用の変更なしエラー</summary>
		public static string ERRMSG_FRONT_NEXTSHIPPINGUSEPOINT_NO_CHANGE_ERROR = "ERRMSG_FRONT_NEXTSHIPPINGUSEPOINT_NO_CHANGE_ERROR";
		/// <summary>エラーメッセージ定数：次回購入ポイント利用が商品合計金額を上回ったエラー</summary>
		public static string ERRMSG_FRONT_EXCEEDED_AVAILABLE_POINTS_ERROR = "ERRMSG_FRONT_EXCEEDED_AVAILABLE_POINTS_ERROR";
		/// <summary>エラーメッセージ定数：次回購入利用クーポンが商品商品合計金額を上回ったため次回購入ポイント利用をリセットした警告</summary>
		public static string ERRMSG_FRONT_NEXT_SHIPPING_USE_POINT_RESET_ALERT = "ERRMSG_FRONT_NEXT_SHIPPING_USE_POINT_RESET_ALERT";
		//========================================================================
		//  W2MP：クーポンオプションエラー
		//========================================================================
		/// <summary>エラーメッセージ定数：利用クーポンチェックエラー</summary>
		public static string ERRMSG_COUPON_NO_COUPON_ERROR = "ERRMSG_COUPON_NO_COUPON_ERROR";
		/// <summary>エラーメッセージ定数：重複チェックエラー</summary>
		public static string ERRMSG_COUPON_DUPLICATION_ERROR = "ERRMSG_COUPON_DUPLICATION_ERROR";
		/// <summary>エラーメッセージ定数：クーポン有効性チェックエラー</summary>
		public static string ERRMSG_COUPON_NO_COUPON_ERRORCODE = "ERRMSG_COUPON_NO_COUPON_ERRORCODE";
		/// <summary>エラーメッセージ定数：未使用チェックエラー</summary>
		public static string ERRMSG_COUPON_USED_ERROR = "ERRMSG_COUPON_USED_ERROR";
		/// <summary>エラーメッセージ定数：有効フラグチェックエラー</summary>
		public static string ERRMSG_COUPON_INVALID_ERROR = "ERRMSG_COUPON_INVALID_ERROR";
		/// <summary>エラーメッセージ定数：有効期限・期間内チェックエラー</summary>
		public static string ERRMSG_COUPON_EXPIRED_ERROR = "ERRMSG_COUPON_EXPIRED_ERROR";
		/// <summary>エラーメッセージ定数：対象商品チェックエラー</summary>
		public static string ERRMSG_COUPON_TARGET_PRODUCT_ERROR = "ERRMSG_COUPON_TARGET_PRODUCT_ERROR";
		/// <summary>エラーメッセージ定数：対象外商品チェックエラー</summary>
		public static string ERRMSG_COUPON_EXCEPTIONAL_PRODUCT_ERROR = "ERRMSG_COUPON_EXCEPTIONAL_PRODUCT_ERROR";
		/// <summary>エラーメッセージ定数：利用時の最低購入金額チェックエラー</summary>
		public static string ERRMSG_COUPON_USABLE_PRICE_ERROR = "ERRMSG_COUPON_USABLE_PRICE_ERROR";
		/// <summary>エラーメッセージ定数：クーポン割引額変更チェックエラー</summary>
		public static string ERRMSG_COUPON_DISCOUNT_PRICE_CHANGED_ERROR = "ERRMSG_COUPON_DISCOUNT_PRICE_CHANGED_ERROR";
		/// <summary>エラーメッセージ定数：利用可能回数ゼロエラー</summary>
		public static string ERRMSG_COUPON_USABLE_COUNT_ERROR = "ERRMSG_COUPON_USABLE_COUNT_ERROR";
		/// <summary>エラーメッセージ定数：クーポン利用の商品合計金額オーバーエラー</summary>
		public static string ERRMSG_COUPON_PRICE_SUBTOTAL_MINUS_COUPON_ERROR = "ERRMSG_COUPON_PRICE_SUBTOTAL_MINUS_COUPON_ERROR";
		/// <summary>エラーメッセージ定数：ブラックリスト型クーポン利用対象チェックエラー</summary>
		public static string ERRMSG_BLACKLIST_COUPON_USE_TARGET_ERROR = "ERRMSG_BLACKLIST_COUPON_USE_TARGET_ERROR";
		/// <summary>エラーメッセージ定数：次回購入の利用クーポンの更新失敗エラー</summary>
		public static string ERRMSG_FRONT_NEXT_SHIPPING_USE_COUPON_UPDATE_ALERT = "ERRMSG_FRONT_NEXT_SHIPPING_USE_COUPON_UPDATE_ALERT";
		/// <summary>エラーメッセージ定数：次回購入利用クーポンの変更なしエラー</summary>
		public static string ERRMSG_FRONT_NEXT_SHIPPING_USE_COUPON_NO_CHANGE_ERROR = "ERRMSG_FRONT_NEXT_SHIPPING_USE_COUPON_NO_CHANGE_ERROR";
		/// <summary>注文同梱時クーポン適応外の商品あり</summary>
		public static string ERRMSG_COUPON_NOT_APPLICABLE_BY_ORDER_COMBINED = "ERRMSG_COUPON_NOT_APPLICABLE_BY_ORDER_COMBINED";
		/// <summary>領収書情報の更新失敗エラー</summary>
		public static string ERRMSG_FRONT_RECEIPT_INFO_UPDATE_ERROR = "ERRMSG_FRONT_RECEIPT_INFO_UPDATE_ERROR";
		/// <summary>出力済み領収書情報を更新する時の失敗エラー</summary>
		public static string ERRMSG_FRONT_RECEIPT_INFO_UPDATE_ERROR_SETTLED_RECEIPTDOWNLOAD = "ERRMSG_FRONT_RECEIPT_INFO_UPDATE_ERROR_SETTLED_RECEIPTDOWNLOAD";
		/// <summary>領収書発行できないお支払い方法の注文の領収書情報を更新する時の失敗エラー</summary>
		public static string ERRMSG_FRONT_RECEIPT_INFO_UPDATE_ERROR_NOT_OUTPUT_RECEIPT_PAYMENT_KBN = "ERRMSG_FRONT_RECEIPT_INFO_UPDATE_ERROR_NOT_OUTPUT_RECEIPT_PAYMENT_KBN";
		/// <summary>エラーメッセージ定数：次回購入の利用クーポンのキャンセル失敗エラー</summary>
		public static string ERRMSG_FRONT_NEXT_SHIPPING_USE_COUPON_CANCEL_ALERT = "ERRMSG_FRONT_NEXT_SHIPPING_USE_COUPON_CANCEL_ALERT";
		/// <summary>エラーメッセージ定数：クーポン割引額がゼロエラー</summary>
		public static string ERRMSG_COUPON_DISCOUNT_PRICE_ZERO = "ERRMSG_COUPON_DISCOUNT_PRICE_ZERO";

		/// <summary>CSV出力フィールドエラー</summary>
		public static string ERRMSG_MANAGER_CSV_OUTPUT_FIELD_ERROR = "ERRMSG_MANAGER_CSV_OUTPUT_FIELD_ERROR";
		/// <summary>CSV出力税率毎価格フィールドエラー</summary>
		public static string ERRMSG_MANAGER_CSV_OUTPUT_TAX_FIELD_ERROR = "ERRMSG_MANAGER_CSV_OUTPUT_TAX_FIELD_ERROR";

		/// <summary>ユーサー存在しないエラー</summary>
		public static string ERRMSG_USER_NOT_EXISTS = "ERRMSG_USER_NOT_EXISTS";
		/// <summary>管理者へご連絡メッセージ</summary>
		public static string ERRMSG_CONTACT_WITH_OPERATOR = "ERRMSG_CONTACT_WITH_OPERATOR";
		/// <summary>今すぐ注文実行エラーメッセージ</summary>
		public static string ERRMSG_FIXEDPURCHASEORDER_ERROR = "ERRMSG_FIXEDPURCHASEORDER_ERROR";

		// 注文更新処理関連
		/// <summary>外部決済連携キャンセルエラー</summary>
		public static string ERRMSG_EXTERNAL_PAYMENT_CANCEL_FAILED = "ERRMSG_EXTERNAL_PAYMENT_CANCEL_FAILED";
		/// <summary>外部決済連携与信しないアラート</summary>
		public static string ERRMSG_EXTERNAL_PAYMENT_NOT_AUTH_ALERT = "ERRMSG_EXTERNAL_PAYMENT_NOT_AUTH_ALERT";
		/// <summary>ユーザークレジットカード表示フラグ更新エラー</summary>
		public static string ERRMSG_USERCREDITCARD_CANNOT_UPDATE_DISP_FLG_ERROR = "ERRMSG_USERCREDITCARD_CANNOT_UPDATE_DISP_FLG_ERROR";
		/// <summary>在庫切れエラー</summary>
		public static string ERRMSG_PRODUCTSTOCK_OUT_OF_STOCK_ERROR = "ERRMSG_PRODUCTSTOCK_OUT_OF_STOCK_ERROR";
		/// <summary>Error message: Invalid shipping date</summary>
		public static string ERRMSG_SHIPPINGDATE_INVALID = "ERRMSG_SHIPPINGDATE_INVALID";
		/// <summary>Error message: Changed shipping date</summary>
		public static string ERRMSG_SHIPPINGDATE_CHANGED = "ERRMSG_SHIPPINGDATE_CHANGED";

		/// <summary>エラーメッセージ定数：過去日選択警告</summary>
		public static string ERRMSG_MANAGER_SELECT_PAST_DATE_ERROR = "ERRMSG_MANAGER_SELECT_PAST_DATE_ERROR";

		/// <summary>決済通貨のメッセージ</summary>
		public static string ERRMSG_MANAGER_AMOUNT_VARIES_WITH_RATE = "ERRMSG_MANAGER_AMOUNT_VARIES_WITH_RATE";

		/// <summary>注文管理カード実売上処理通信エラー</summary>
		public static string ERRMSG_MANAGER_ORDER_CARD_REALSALES_CONNECTION_ERROR = "ERRMSG_MANAGER_ORDER_CARD_REALSALES_CONNECTION_ERROR";
		/// <summary>注文管理カード実売上処理済みエラー</summary>
		public static string ERRMSG_MANAGER_ORDER_CARD_ALREADY_REALSALES_ERROR = "ERRMSG_MANAGER_ORDER_CARD_ALREADY_REALSALES_ERROR";
		/// <summary>注文管理カード実売上処理失敗エラー</summary>
		public static string ERRMSG_MANAGER_ORDER_CARD_REALSALES_ERROR = "ERRMSG_MANAGER_ORDER_CARD_REALSALES_ERROR";

		/// <summary>エラーファイル最終更新日</summary>
		private static DateTime m_dtFileLastUpdate = new DateTime(0);
		/// <summary>エラーメッセージ格納ディクショナリ</summary>
		private static Dictionary<string, string> m_dicMessages = new Dictionary<string, string>();
		/// <summary>ReaderWriterLockSlimオブジェクト</summary>
		private static System.Threading.ReaderWriterLockSlim m_lock = new System.Threading.ReaderWriterLockSlim();

		/// <summary>Error message: Too many search conditions</summary>
		public static string ERRMSG_TOO_MANY_SEARCH_CONDITION = "ERRMSG_TOO_MANY_SEARCH_CONDITION";

		/// <summary>Fixed Purchase Next Shipping Product Invaild Error Message</summary>
		public static string ERRMSG_FIXED_PURCHASE_NEXT_PRODUCT_INVALID = "ERRMSG_FIXED_PURCHASE_NEXT_PRODUCT_INVALID";
		/// <summary>Fixed Purchase Next Shipping Product Shipping Kbn Different Error Message</summary>
		public static string ERRMSG_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SHIPPING_KBN_DIFF = "ERRMSG_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SHIPPING_KBN_DIFF";
		/// <summary>Fixed Purchase Next Shipping Product Fixed Purchase Disable Error Message</summary>
		public static string ERRMSG_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_FIXED_PURCHASE_DISABLE = "ERRMSG_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_FIXED_PURCHASE_DISABLE";
		/// <summary>Fixed Purchase Next Shipping Item Quantity Necessary Error Message</summary>
		public static string ERRMSG_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY_NECESSARY = "ERRMSG_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY_NECESSARY";
		/// <summary>Fixed Purchase Next Shipping Item Quantity Number Min Error Message</summary>
		public static string ERRMSG_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY_NUMBER_MIN = "ERRMSG_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY_NUMBER_MIN";
		/// <summary>Next Shipping Item Fixed Purchase Setting Not Set Error Message</summary>
		public static string ERRMSG_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING_NOT_SET = "ERRMSG_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING_NOT_SET";

		/// <summary>配送種別が存在しないエラー</summary>
		public static string ERRMSG_SHIPPING_TYPE_NOT_EXISTS = "ERRMSG_SHIPPING_TYPE_NOT_EXISTS";

		//========================================================================
		//  ユーザー系
		//========================================================================
		/// <summary>異なるFCユーザーと連携済みのエラー (SingleSignOn - MackenyuFCサイト)</summary>
		public static string ERRMSG_MANAGER_COOPERATION_WITH_DIFFERENT_FC_USER = "ERRMSG_MANAGER_COOPERATION_WITH_DIFFERENT_FC_USER";
		/// <summary>ログイン認証失敗のエラー (SingleSignOn - Pioneerサイト)</summary>
		public static string ERRMSG_FRONT_USER_LOGIN_FOR_SINGLESIGNON = "ERRMSG_FRONT_USER_LOGIN_FOR_SINGLESIGNON";
		/// <summary>会員ランクが存在しないエラー (SingleSignOn - Pioneerサイト)</summary>
		public static string ERRMSG_FRONT_USER_RANK_NOT_EXISTS = "ERRMSG_FRONT_USER_RANK_NOT_EXISTS";
		/// <summary>ユーザー拡張項目が存在しないエラー (SingleSignOn - Pioneerサイト)</summary>
		public static string ERRMSG_FRONT_USER_EXTENDS_NOT_EXISTS = "ERRMSG_FRONT_USER_EXTENDS_NOT_EXISTS";
		/// <summary>Error message: Cart list not exist</summary>
		public static string ERRMSG_CARTLIST_NOT_EXISTS = "ERRMSG_CARTLIST_NOT_EXISTS";
		/// <summary>Error message: Order not exist</summary>
		public static string ERRMSG_ORDER_NOT_EXISTS = "ERRMSG_ORDER_NOT_EXISTS";

		/// <summary>Error message convenience store not exist</summary>
		public static string ERRMSG_CONVENIENCE_STORE_NOT_VALID = "ERRMSG_CONVENIENCE_STORE_NOT_VALID";

		/// <summary>Error message: NP After Pay Order Cannot Shipment Error</summary>
		public static string ERRMSG_MANAGER_ORDER_CANNOT_SHIPMENT_ERROR = "ERRMSG_MANAGER_ORDER_CANNOT_SHIPMENT_ERROR";
		/// <summary>Error message: NP After Pay Cannot Link Payment</summary>
		public static string ERRMSG_MANAGER_NPAFTERPAY_CANNOT_LINK_PAYMENT = "ERRMSG_MANAGER_NPAFTERPAY_CANNOT_LINK_PAYMENT";

		/// <summary>Error Message: TelNo is not Taiwan</summary>
		public static string ERRMSG_FRONT_TEL_NO_NOT_TAIWAN = "ERRMSG_FRONT_TEL_NO_NOT_TAIWAN";
		/// <summary>Error Message: The total size of the product is more than the limit</summary>
		public static string ERRMSG_FRONT_TOTAL_SIZE_OF_PRODUCT_MORE_THAN_LIMIT = "ERRMSG_FRONT_TOTAL_SIZE_OF_PRODUCT_MORE_THAN_LIMIT";
		/// <summary>Error Message: Invalid owner name for convenience store</summary>
		public static string ERRMSG_FRONT_OWNER_NAME_OF_CONVENIENCE_STORE_INVALID = "ERRMSG_FRONT_OWNER_NAME_OF_CONVENIENCE_STORE_INVALID";

		/// <summary>Error Message: Check Country For Payment EcPay</summary>
		public static string ERRMSG_CHECK_COUNTRY_FOR_PAYMENT_ECPAY = "ERRMSG_CHECK_COUNTRY_FOR_PAYMENT_ECPAY";

		/// <summary>Error Message: Send authentication code</summary>
		public static string ERRMSG_FRONT_SEND_AUTHENTICATION_CODE = "ERRMSG_FRONT_SEND_AUTHENTICATION_CODE";
		/// <summary>Error Message: User request too many authentications</summary>
		public static string ERRMSG_FRONT_USER_REQUEST_TOO_MANY_AUTH = "ERRMSG_FRONT_USER_REQUEST_TOO_MANY_AUTH";
		/// <summary>Error Message: User entered fail auth code many times</summary>
		public static string ERRMSG_FRONT_USER_ENTERED_FAIL_AUTH_CODE_MANY_TIMES = "ERRMSG_FRONT_USER_ENTERED_FAIL_AUTH_CODE_MANY_TIMES";
		/// <summary>Error Message: User entered fail auth code</summary>
		public static string ERRMSG_FRONT_USER_ENTERED_FAIL_AUTH_CODE = "ERRMSG_FRONT_USER_ENTERED_FAIL_AUTH_CODE";
		/// <summary>Error Message: Auth code has expired</summary>
		public static string ERRMSG_FRONT_AUTH_CODE_HAS_EXPIRED = "ERRMSG_FRONT_AUTH_CODE_HAS_EXPIRED";
		/// <summary>Error Message: Tel no locked fail auth_code many times</summary>
		public static string ERRMSG_FRONT_TEL_NO_LOCKED_FAIL_AUTH_CODE_MANY_TIMES = "ERRMSG_FRONT_TEL_NO_LOCKED_FAIL_AUTH_CODE_MANY_TIMES";
		/// <summary>Error Message: Reacquire the authentication code</summary>
		public static string ERRMSG_FRONT_REACQUIRE_THE_AUTHENTICATION_CODE = "ERRMSG_FRONT_REACQUIRE_THE_AUTHENTICATION_CODE";
		/// <summary>Error Message: Not exists authentication code receiver</summary>
		public static string ERRMSG_FRONT_NOT_EXISTS_AUTHENTICATION_CODE_RECEIVER = "ERRMSG_FRONT_NOT_EXISTS_AUTHENTICATION_CODE_RECEIVER";

		/// <summary>Error Message: Product search api invalid parameters</summary>
		public static string ERRMSG_FRONT_PRODUCT_SEARCH_API_INVALID_PARAMETERS = "ERRMSG_FRONT_PRODUCT_SEARCH_API_INVALID_PARAMETERS";
		/// <summary>Error Message: Search product no data</summary>
		public static string ERRMSG_FRONT_SEARCH_PRODUCT_NO_DATA = "ERRMSG_FRONT_SEARCH_PRODUCT_NO_DATA";

		//========================================================================
		//  受注ワークフロー関連
		//========================================================================
		/// <summary>出荷予定日更新エラー</summary>
		public static string ERRMSG_SCHEDULE_SHIPPING_DATE_UPDATE_NG = "ERRMSG_SCHEDULE_SHIPPING_DATE_UPDATE_NG";
		/// <summary>配送希望日更新エラー</summary>
		public static string ERRMSG_SHIPPING_DATE_UPDATE_NG = "ERRMSG_SHIPPING_DATE_UPDATE_NG";
		/// <summary>商品在庫更新エラー</summary>
		public static string ERRMSG_PRODUCTSTOCK_UPDATE_NG = "ERRMSG_PRODUCTSTOCK_UPDATE_NG";
		/// <summary>返品注文更新エラー</summary>
		public static string ERRMSG_RETURN_ORDER_UPDATE_NG = "ERRMSG_RETURN_ORDER_UPDATE_NG";
		/// <summary>注文ステータス更新エラー</summary>
		public static string ERRMSG_ORDER_STATUS_UPDATE_NG = "ERRMSG_ORDER_STATUS_UPDATE_NG";
		/// <summary>仮ポイント→本ポイント更新エラー</summary>
		public static string ERRMSG_TEMP_TO_REAL_POINT_UPDATE_NG = "ERRMSG_TEMP_TO_REAL_POINT_UPDATE_NG";
		/// <summary>Atodene決済印字データ取得エラー</summary>
		public static string ERRMSG_ATODENE_INVOICE_GET_NG = "ERRMSG_ATODENE_INVOICE_GET_NG";
		/// <summary>入金ステータス更新エラー</summary>
		public static string ERRMSG_PAYMENT_STATUS_UPDATE_NG = "ERRMSG_PAYMENT_STATUS_UPDATE_NG";
		/// <summary>督促ステータス更新エラー</summary>
		public static string ERRMSG_DEMAND_STATUS_UPDATE_NG = "ERRMSG_DEMAND_STATUS_UPDATE_NG";
		/// <summary>領収書出力フラグ更新エラー</summary>
		public static string ERRMSG_RECEIPT_OUTPUT_FLG_UPDATE_NG = "ERRMSG_RECEIPT_OUTPUT_FLG_UPDATE_NG";
		/// <summary>注文拡張ステータス更新エラー</summary>
		public static string ERRMSG_ORDER_EXTEND_STATUS_UPDATE_NG = "ERRMSG_ORDER_EXTEND_STATUS_UPDATE_NG";
		/// <summary>返金ステータス更新エラー</summary>
		public static string ERRMSG_REPAYMENT_STATUS_UPDATE_NG = "ERRMSG_REPAYMENT_STATUS_UPDATE_NG";
		/// <summary>返品交換ステータス更新エラー</summary>
		public static string ERRMSG_RETURN_EXCHANGE_STATUS_UPDATE_NG = "ERRMSG_RETURN_EXCHANGE_STATUS_UPDATE_NG";
		/// <summary>オンライン決済ステータス更新エラー</summary>
		public static string ERRMSG_ONLINE_PAYMENT_STATUS_UPDATE_NG = "ERRMSG_ONLINE_PAYMENT_STATUS_UPDATE_NG";
		/// <summary>外部決済連携エラー</summary>
		public static string ERRMSG_EXTERNAL_PAYMENT_COOPERATION_NG = "ERRMSG_EXTERNAL_PAYMENT_COOPERATION_NG";
		/// <summary>外部受注情報連携エラー</summary>
		public static string ERRMSG_EXTERNAL_ORDER_INFO_COOPERATION_NG = "ERRMSG_EXTERNAL_ORDER_INFO_COOPERATION_NG";
		/// <summary>連絡メール送信エラー</summary>
		public static string ERRMSG_SEND_ORDER_MAIL_NG = "ERRMSG_SEND_ORDER_MAIL_NG";
		/// <summary>定期購入解約更新エラー</summary>
		public static string ERRMSG_CANCEL_FIXEDPURCHASE_NG = "ERRMSG_CANCEL_FIXEDPURCHASE_NG";
		/// <summary>定期購入再開更新エラー</summary>
		public static string ERRMSG_RESUME_FIXEDPURCHASE_NG = "ERRMSG_RESUME_FIXEDPURCHASE_NG";
		/// <summary>定期購入休止更新エラー</summary>
		public static string ERRMSG_SUSPEND_FIXEDPURCHASE_NG = "ERRMSG_SUSPEND_FIXEDPURCHASE_NG";
		/// <summary>定期購入完了更新エラー</summary>
		public static string ERRMSG_COMPLETE_FIXEDPURCHASE_NG = "ERRMSG_COMPLETE_FIXEDPURCHASE_NG";
		/// <summary>定期決済スタータス更新エラー</summary>
		public static string ERRMSG_FIXEDPURCHASE_PAYMENT_STATUS_UPDATE_NG = "ERRMSG_FIXEDPURCHASE_PAYMENT_STATUS_UPDATE_NG";
		/// <summary>次回配送日更新エラー</summary>
		public static string ERRMSG_NEXT_SHIPPING_DATE_UPDATE_NG = "ERRMSG_NEXT_SHIPPING_DATE_UPDATE_NG";
		/// <summary>次々回配送日更新エラー</summary>
		public static string ERRMSG_NEXT_NEXT_SHIPPING_DATE_UPDATE_NG = "ERRMSG_NEXT_NEXT_SHIPPING_DATE_UPDATE_NG";
		/// <summary>atone払い戻しAPIエラー</summary>
		public static string ERRMSG_ATONE_REFUND_PAYMENT_API_NG = "ERRMSG_ATONE_REFUND_PAYMENT_API_NG";
		/// <summary>Atoneオーソリ結果NG：金額上限</summary>
		public static string ERRMSG_ATONE_AUTHORIZATION_RESULT_NG_EXCEED = "ERRMSG_ATONE_AUTHORIZATION_RESULT_NG_EXCEED";
		/// <summary>Atoneオーソリ結果NG：その他</summary>
		public static string ERRMSG_ATONE_AUTHORIZATION_RESULT_NG_OTHER = "ERRMSG_ATONE_AUTHORIZATION_RESULT_NG_OTHER";
		/// <summary>AFTEE払い戻しAPIエラー</summary>
		public static string ERRMSG_AFTEE_REFUND_PAYMENT_API_NG = "ERRMSG_AFTEE_REFUND_PAYMENT_API_NG";
		/// <summary>ECPay払い戻しAPIエラー</summary>
		public static string ERRMSG_ECPAY_REFUND_PAYMENT_API_NG = "ERRMSG_ECPAY_REFUND_PAYMENT_API_NG";
		/// <summary>注文電子発票APIエラー</summary>
		public static string ERRMSG_ORDER_INVOICE_API_NG = "ERRMSG_ORDER_INVOICE_API_NG";
		/// <summary>注文電子発票APIエラー</summary>
		public static string ERRMSG_ORDER_INVOICE_STATUS_UPDATE_NG = "ERRMSG_ORDER_INVOICE_STATUS_UPDATE_NG";
		/// <summary>DSK後払い決済印字データ取得エラー</summary>
		public static string ERRMSG_DSK_DEFERRED_INVOICE_GET_NG = "ERRMSG_DSK_DEFERRED_INVOICE_GET_NG";
		/// <summary>PayPay refund api error</summary>
		public static string ERRMSG_PAYPAY_REFUND_PAYMENT_API_NG = "ERRMSG_PAYPAY_REFUND_PAYMENT_API_NG";
		/// <summary>ベリトランス後払い決済印字データ取得エラー</summary>
		public static string ERRMSG_VERITRANS_INVOICE_GET_NG = "ERRMSG_VERITRANS_INVOICE_GET_NG";

		/// <summary>Error message: Mobile code not exist</summary>
		public static string ERRMSG_MOBILE_CODE_NOT_EXIST = "ERRMSG_MOBILE_CODE_NOT_EXIST";
		/// <summary>Error message: Donation code not exist</summary>
		public static string ERRMSG_DONATION_CODE_NOT_EXIST = "ERRMSG_DONATION_CODE_NOT_EXIST";
		/// <summary>Error message: EcPay invoice system maintenance</summary>
		public static string ERRMSG_ECPAY_INVOICE_SYSTEM_MAINTENANCE = "ERRMSG_ECPAY_INVOICE_SYSTEM_MAINTENANCE";

		/// <summary>エラーメッセージ定数：返品交換 通常付与ポイント 調整失敗</summary>
		public const string ERRMSG_MANAGER_RETURN_EXCHANGE_ADD_BASE_POINT_ERROR = "ERRMSG_MANAGER_RETURN_EXCHANGE_ADD_BASE_POINT_ERROR";
		/// <summary>エラーメッセージ定数：返品交換 期間限定付与ポイント 調整失敗</summary>
		public const string ERRMSG_MANAGER_RETURN_EXCHANGE_ADD_LIMIT_POINT_ERROR = "ERRMSG_MANAGER_RETURN_EXCHANGE_ADD_LIMIT_POINT_ERROR";

		/// <summary>エラーメッセージ定数：キャンセル 通常付与ポイント 調整失敗</summary>
		public const string ERRMSG_MANAGER_CANCEL_ADD_POINT_ERROR = "ERRMSG_MANAGER_CANCEL_ADD_POINT_ERROR";
		/// <summary>Error message: Specified delivery interval not availble</summary>
		public static string ERRMSG_SPECIFIED_DELIVERY_INTERVAL_NOT_AVAILABLE = "ERRMSG_SPECIFIED_DELIVERY_INTERVAL_NOT_AVAILABLE";

		/// <summary>Error message: The request to reissue the transfer form has been completed</summary>
		public static string ERRMSG_CVSDEF_INVOICE_REISSUE_COMPLETE = "ERRMSG_CVSDEF_INVOICE_REISSUE_COMPLETE";
		/// <summary>Error message: Request a reissue of the transfer form</summary>
		public static string ERRMSG_CVSDEF_INVOICE_REISSUE_CONFIRM = "ERRMSG_CVSDEF_INVOICE_REISSUE_CONFIRM";
		/// <summary>Error message: Invoice reissue</summary>
		public static string ERRMSG_CVSDEF_INVOICE_REISSUE = "ERRMSG_CVSDEF_INVOICE_REISSUE";

		/// <summary>Error message: fixed purchase shiping date range error</summary>
		public const string ERRMSG_MANAGER_FIXED_PURCHASE_SHIPPING_DATE_RANGE_ERROR = "ERRMSG_MANAGER_FIXED_PURCHASE_SHIPPING_DATE_RANGE_ERROR";

		/// <summary>他のワークフローが実行中または保留中の場合のエラーメッセージ</summary>
		public const string ERRMSG_MANAGER_ORDERWORKFLOW_OTHER_RUNNING_OR_HOLD = "ERRMSG_MANAGER_ORDERWORKFLOW_OTHER_RUNNING_OR_HOLD";
		/// <summary>他のワークフローが実行中の場合のエラーメッセージ</summary>
		public const string ERRMSG_MANAGER_ORDERWORKFLOW_OTHER_RUNNING = "ERRMSG_MANAGER_ORDERWORKFLOW_OTHER_RUNNING";

		/// <summary>Error Message: Check Country For Payment NewebPay</summary>
		public static string ERRMSG_CHECK_COUNTRY_FOR_PAYMENT_NEWEBPAY = "ERRMSG_CHECK_COUNTRY_FOR_PAYMENT_NEWEBPAY";

		/// <summary>Error message: login session expired</summary>
		public const string ERRMSG_MANAGER_LOGIN_SESSION_EXPIRED = "ERRMSG_MANAGER_LOGIN_SESSION_EXPIRED";

		/// <summary>エラーメッセージ定数：商品購入可能上限数、下限数が同値</summary>
		public static string ERRMSG_FRONT_PRODUCT_MAXSELLQUANTITY_SAME_AS_MINSELLQUANTITY = "ERRMSG_FRONT_PRODUCT_MAXSELLQUANTITY_SAME_AS_MINSELLQUANTITY";
		/// <summary>エラーメッセージ定数：商品購入数エラー時に表示する商品購入上限値</summary>
		public static string ERRMSG_FRONT_PRODUCT_DISPLAY_ON_MAXSELLQUANTITY = "ERRMSG_FRONT_PRODUCT_DISPLAY_ON_MAXSELLQUANTITY";

		/// <summary>Error message: O-PLUX register order did not pass examination</summary>
		public static string ERRMSG_FRONT_OPLUX_REGISTER_ORDER_DID_NOT_PASS_EXAMINATION = "ERRMSG_FRONT_OPLUX_REGISTER_ORDER_DID_NOT_PASS_EXAMINATION";

		/// <summary>エラーメッセージ定数：注文キャンセル時間超過_注文ステータス更新済み</summary>
		public static string ERRMSG_ORDER_CANCELABLE_TIMEOUT_ORDER_STATUS_UPDATED = "ERRMSG_ORDER_CANCELABLE_TIMEOUT_ORDER_STATUS_UPDATED";
		/// <summary>エラーメッセージ定数：注文ステータス更新失敗</summary>
		public static string ERRMSG_UPDATE_ORDER_STATUS_FAILED = "ERRMSG_UPDATE_ORDER_STATUS_FAILED";
		/// <summary>エラーメッセージ定数：決済連動キャンセルエラー</summary>
		public static string ERRMSG_CANCEL_PAYMENT_FAILED = "ERRMSG_CANCEL_PAYMENT_FAILED";

		/// <summary>PayPay auth error</summary>
		public static string ERRMSG_FRONT_PAYPAY_AUTH_ERROR = "ERRMSG_FRONT_PAYPAY_AUTH_ERROR";

		/// <summary>Message: Notes data migration credit execution</summary>
		public static string ERRMSG_DATA_MIGRATION_NOTES_CREDIT_EXECUTION_MESSAGE = "ERRMSG_DATA_MIGRATION_NOTES_CREDIT_EXECUTION_MESSAGE";
		/// <summary>Message: Notes data migration data upload</summary>
		public static string ERRMSG_DATA_MIGRATION_NOTES_DATA_UPLOAD_MESSAGE = "ERRMSG_DATA_MIGRATION_NOTES_DATA_UPLOAD_MESSAGE";
		/// <summary>Message: Please import or delete the file to upload another file</summary>
		public static string ERRMSG_DATA_MIGRATION_FILE_UPLOAD_NOTICE = "ERRMSG_DATA_MIGRATION_FILE_UPLOAD_NOTICE";
		/// <summary>Error message: File upload data migration notice</summary>
		public static string ERRMSG_DATA_MIGRATION_IMPORT_OR_DELETE_FILE_NOTICE = "ERRMSG_DATA_MIGRATION_IMPORT_OR_DELETE_FILE_NOTICE";
		/// <summary>Error message: The file already exists</summary>
		public static string ERRMSG_DATA_MIGRATION_FILE_ALREADY_EXISTS = "ERRMSG_DATA_MIGRATION_FILE_ALREADY_EXISTS";
		/// <summary>Error message: The target file was not found</summary>
		public static string ERRMSG_DATA_MIGRATION_FILE_UNFIND = "ERRMSG_DATA_MIGRATION_FILE_UNFIND";
		/// <summary>Error message: No files are currently uploaded</summary>
		public static string ERRMSG_DATA_MIGRATION_FILE_UPLOAD_ERROR = "ERRMSG_DATA_MIGRATION_FILE_UPLOAD_ERROR";
		/// <summary>Error message: File upload failed</summary>
		public static string ERRMSG_DATA_MIGRATION_NO_FILE_UPLOAD = "ERRMSG_DATA_MIGRATION_NO_FILE_UPLOAD";
		/// <summary>Error message: The only files that can be uploaded are *.csv</summary>
		public static string ERRMSG_MANAGER_DATA_MIGRATION_FILE_UPLOAD_NOT_CSV = "ERRMSG_DATA_MIGRATION_FILE_UPLOAD_NOT_CSV";
		/// <summary>Message manager: Imported migration data. We will notify you by email after the end</summary>
		public static string ERRMSG_DATA_MIGRATION_COMPLETE_MESSAGE = "ERRMSG_DATA_MIGRATION_COMPLETE_MESSAGE";
		/// <summary>Message manager: Credit complete</summary>
		public static string ERRMSG_DATA_CREDIT_COMPLETE_MESSAGE = "ERRMSG_DATA_CREDIT_COMPLETE_MESSAGE";
		/// <summary>Message manager: Need to turn on extend status message</summary>
		public static string ERRMSG_NEED_TO_TURN_ON_EXTENDED_STATUSES_MESSAGE = "ERRMSG_NEED_TO_TURN_ON_EXTENDED_STATUSES_MESSAGE";
		/// <summary>Message manager: Can re credit from here message</summary>
		public static string ERRMSG_CAN_RE_CREDIT_FROM_HERE_MESSAGE = "ERRMSG_CAN_RE_CREDIT_FROM_HERE_MESSAGE";

		/// <summary>Error message: Atobaraicom csv def auth error</summary>
		public static string ERRMSG_FRONT_ATOBARAICOM_CVSDEFAUTH_ERROR = "ERRMSG_FRONT_ATOBARAICOM_CVSDEFAUTH_ERROR";
		/// <summary>エラーメッセージ定数：頒布会コース購入数量エラー</summary>
		public const string ERRMSG_SUBSCRIPTION_BOX_PRODUCT_CART_QUANTITY = "ERRMSG_SUBSCRIPTION_BOX_PRODUCT_CART_QUANTITY";
		/// <summary>エラーメッセージ定数：頒布会コース最大購入数量エラー</summary>
		public const string ERRMSG_SUBSCRIPTION_BOX_PRODUCT_CART_QUANTITY_MAX = "ERRMSG_SUBSCRIPTION_BOX_PRODUCT_CART_QUANTITY_MAX";
		/// <summary>エラーメッセージ定数：頒布会コース最小購入数量エラー</summary>
		public const string ERRMSG_SUBSCRIPTION_BOX_PRODUCT_CART_QUANTITY_MIN = "ERRMSG_SUBSCRIPTION_BOX_PRODUCT_CART_QUANTITY_MIN";
		/// <summary>Error message: Delete all product in mypage</summary>
		public static string ERRMSG_FRONT_SUBSCRIPTION_BOX_PRODUCT_DELETE_ALL = "ERRMSG_FRONT_SUBSCRIPTION_BOX_PRODUCT_DELETE_ALL";
		/// <summary>Error message: Dulication product</summary>
		public static string ERRMSG_FRONT_MANAGER_ORDERITEM_DUPLICATION_ERROR = "ERRMSG_FRONT_MANAGER_ORDERITEM_DUPLICATION_ERROR";
		/// <summary>Error message: subscription box invalid on CartList</summary>
		public static string ERRMSG_FRONT_SUBSCRIPTION_BOX_ID_INVALID = "ERRMSG_FRONT_SUBSCRIPTION_BOX_ID_INVALID";
		/// <summary>Error message: subscription box not meet period or number time</summary>
		public static string ERRMSG_FRONT_SUBSCRIPTION_BOX_NOT_MEET_PERIOD_NUMBERTIME = "ERRMSG_FRONT_SUBSCRIPTION_BOX_NOT_MEET_PERIOD_NUMBERTIME";
		/// <summary>Error message: Product quantity is number</summary>
		public static string ERRMSG_FRONT_PRODUCT_QUANTITY_UPDATE_ALERT = "ERRMSG_FRONT_PRODUCT_QUANTITY_UPDATE_ALERT";
		/// <summary>商品購入限度数エラー</summary>
		public static string ERRMSG_FRONT_PRODUCT_MAX_SELL_QUANTITY_LIMIT_ERROR = "ERRMSG_FRONT_PRODUCT_MAX_SELL_QUANTITY_LIMIT_ERROR";
		/// <summary>Error message: Invalid next shipping date</summary>
		public static string ERRMSG_FRONT_SUBSCRIPTION_BOX_NEXT_SHIPPING_DATE_INVALID = "ERRMSG_FRONT_SUBSCRIPTION_BOX_NEXT_SHIPPING_DATE_INVALID";
		/// <summary>最低購入種類数、最大購入種類数の条件を満たしていない 最低/最大種類数の表示あり</summary>
		public const string ERRMSG_SUBSCRIPTION_BOX_NUMBER_OF_PRODUCTS_ERROR_DISPLAY_REQUIRED_NUMBER_OF_PRODUCTS = "ERRMSG_SUBSCRIPTION_BOX_NUMBER_OF_PRODUCTS_ERROR_DISPLAY_REQUIRED_NUMBER_OF_PRODUCTS";
		/// <summary>最低購入種類数の条件を満たしていない 最低種類数の表示あり</summary>
		public const string ERRMSG_SUBSCRIPTION_BOX_MIN_NUMBER_OF_PRODUCTS_ERROR = "ERRMSG_SUBSCRIPTION_BOX_MIN_NUMBER_OF_PRODUCTS_ERROR";
		/// <summary>最大購入種類数の条件を満たしていない 最大種類数の表示あり</summary>
		public const string ERRMSG_SUBSCRIPTION_BOX_MAX_NUMBER_OF_PRODUCTS_ERROR = "ERRMSG_SUBSCRIPTION_BOX_MAX_NUMBER_OF_PRODUCTS_ERROR";
		/// <summary>定期台帳に紐づく頒布会が存在しないエラー(定期台帳に紐づく頒布会コースIDから頒布会が取得できない場合)</summary>
		public const string ERRMSG_FIXED_PURCHASE_LINK_SUBSCRIPTION_BOX_NOT_EXIST = "ERRMSG_FIXED_PURCHASE_LINK_SUBSCRIPTION_BOX_NOT_EXIST";
		// <summary>エラーメッセージ定数：頒布会：次回商品が1つも取得できないエラー
		public static string ERRMSG_FRONT_SUBSCRIPTION_BOX_NO_NEXT_PRODUCT = "ERRMSG_FRONT_SUBSCRIPTION_BOX_NO_NEXT_PRODUCT";
		// <summary>エラーメッセージ定数：削除しようとした商品が頒布会に含まれているエラー
		public static string ERRMSG_DELETE_PRODUCT_INCLUDE_SUBSCRIPTION_BOX = "ERRMSG_DELETE_PRODUCT_INCLUDE_SUBSCRIPTION_BOX";
		/// <summary>定額でない頒布会コース設定の商品合計金額（税込み）を満たしていないエラー</summary>
		public const string ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_MEET_AMOUNT_SET_FOR_NON_FIXED = "ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_MEET_AMOUNT_SET_FOR_NON_FIXED";
		/// <summary>定額でない頒布会コース設定の商品合計金額（税込み）下限を満たしていないエラー</summary>
		public const string ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_MEET_MINIMUM_AMOUNT_SET_FOR_NON_FIXED = "ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_MEET_MINIMUM_AMOUNT_SET_FOR_NON_FIXED";
		/// <summary>定額でない頒布会コース設定の商品合計金額（税込み）上限を満たしていないエラー</summary>
		public const string ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_MEET_MAXIMUM_AMOUNT_SET_FOR_NON_FIXED = "ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_MEET_MAXIMUM_AMOUNT_SET_FOR_NON_FIXED";
		/// <summary>定額の頒布会コース設定の商品合計金額（税込み）を満たしていないエラー</summary>
		public const string ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_MEET_AMOUNT_SET_FOR_FIXED = "ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_MEET_AMOUNT_SET_FOR_FIXED";
		/// <summary>必須商品が選択されていないエラー</summary>
		public const string ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_CONTAIN_NECESSARY_PRODUCTS = "ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_CONTAIN_NECESSARY_PRODUCTS";
		/// <summary>配送キャンセル期限超過時の頒布会商品変更不可エラー</summary>
		public const string ERRMSG_SUBSCRIPTION_BOX_PRODUCT_CANNOT_BE_CHANGED = "ERRMSG_SUBSCRIPTION_BOX_PRODUCT_CANNOT_BE_CHANGED";
		/// <summary>Message manager:Scoring sale page template not exists</summary>
		public static string ERRMSG_MANAGER_SCORINGSALE_PAGE_TEMPLATE_NOT_EXISTS_ERROR = "ERRMSG_MANAGER_SCORINGSALE_PAGE_TEMPLATE_NOT_EXISTS_ERROR";
		/// <summary>店舗（サイト）ID 存在チェック</summary>
		public const string ERRMSG_SHOPSITE_SHOP_SITE_ID_NOT_EXIST = "ERRMSG_SHOPSITE_SHOP_SITE_ID_NOT_EXIST";
		/// <summary>指定された店舗（サイト）が商品に含まれていないエラー</summary>
		public const string ERRMSG_NOT_CONTAINS_SPECIFIED_SHOP_SITE_IN_PRODUCT = "ERRMSG_NOT_CONTAINS_SPECIFIED_SHOP_SITE_IN_PRODUCT";
		/// <summary>入力された商品IDは使用中エラー</summary>
		public static string ERRMSG_MANAGER_PRODUCT_ID_IS_USED_AS_VARIATION_ID_ERROR = "ERRMSG_MANAGER_PRODUCT_ID_IS_USED_AS_VARIATION_ID_ERROR";
		/// <summary>入力された商品ID+バリエーションIDは使用中エラー</summary>
		public static string ERRMSG_MANAGER_PRODUCTVARIATION_VARIATION_ID_IS_USED_AS_PRODUCT_ID_ERROR = "ERRMSG_MANAGER_PRODUCTVARIATION_VARIATION_ID_IS_USED_AS_PRODUCT_ID_ERROR";
		public static string ERRMSG_MANAGER_PRODUCTVARIATION_VARIATION_ID_IS_USED_AS_VARIATION_ID_ERROR = "ERRMSG_MANAGER_PRODUCTVARIATION_VARIATION_ID_IS_USED_AS_VARIATION_ID_ERROR";
		/// <summary>頒布会：1回目配送商品選択可能期間外</summary>
		public const string ERRMSG_FRONT_SUBSCRIPTION_BOX_OUTSIDE_SELECTION_PERIOD = "ERRMSG_FRONT_SUBSCRIPTION_BOX_OUTSIDE_SELECTION_PERIOD";
		/// <summary>頒布会：必須商品が在庫切れエラー</summary>
		public const string ERRMSG_FRONT_SUBSCRIPTION_BOX_SOLDOUT_NECESSARY_PRODUCT = "ERRMSG_FRONT_SUBSCRIPTION_BOX_SOLDOUT_NECESSARY_PRODUCT";

		/// <summary>Error message: point card register error</summary>
		public const string ERRMSG_FRONT_POINT_CARD_REGISTER_ERROR = "ERRMSG_FRONT_POINT_CARD_REGISTER_ERROR";
		/// <summary>Error message: user not exist</summary>
		public const string ERRMSG_FRONT_USER_NOT_EXIST = "ERRMSG_FRONT_USER_NOT_EXIST";
		/// <summary>Error message: cancel CrossPoint linkage failed</summary>
		public static string ERRMSG_MANAGER_CANCEL_CROSS_POINT_LINKAGE_FAILED = "ERRMSG_MANAGER_CANCEL_CROSS_POINT_LINKAGE_FAILED";
		/// <summary>Error message: store order history no item</summary>
		public const string ERRMSG_FRONT_STOREORDERHISTORY_NO_ITEM = "ERRMSG_FRONT_STOREORDERHISTORY_NO_ITEM";
		/// <summary>店舗会員登録失敗エラー</summary>
		public static string ERRMSG_CROSS_POINT_REGISTER_STORE_MEMBER_FAILED = "ERRMSG_CROSS_POINT_REGISTER_STORE_MEMBER_FAILED";
		/// <summary>メールアドレス登録失敗エラー</summary>
		public static string ERRMSG_CROSS_POINT_REGISTER_MAILADDRESS = "ERRMSG_CROSS_POINT_REGISTER_MAILADDRESS";
		/// <summary>PC/モバイルメールアドレス重複エラー</summary>
		public static string ERRMSG_CROSS_POINT_DUPLICATE_PC_MOBILE_MAILADDRESS = "ERRMSG_CROSS_POINT_DUPLICATE_PC_MOBILE_MAILADDRESS";

		/// <summary>Error message: check the expiration date on the CrossPoint management screen</summary>
		public const string ERRMSG_MANAGER_CHECK_THE_EXPIRATION_DATE_ON_THE_CROSSPOINT_MANAGEMENT_SCREEN = "ERRMSG_MANAGER_CHECK_THE_EXPIRATION_DATE_ON_THE_CROSSPOINT_MANAGEMENT_SCREEN";

		/// <summary>CROSS POINT連携時にポイントがゼロを下回るエラー</summary>
		public const string ERRMSG_MANAGER_CROSSPOINT_NEGATIVE_POINT_UNACCEPTABLE = "ERRMSG_MANAGER_CROSSPOINT_NEGATIVE_POINT_UNACCEPTABLE";

		/// <summary>Error message: Authentication code is invalid</summary>
		public static string ERRMSG_LOGIN_INVALID_AUTHENTICATION_CODE = "ERRMSG_LOGIN_INVALID_AUTHENTICATION_CODE";
		/// <summary>Error message: 2-step authentication is disabled</summary>
		public static string ERRMSG_MANAGER_TWO_STEP_AUTHENTICATION_DISABLED = "ERRMSG_MANAGER_TWO_STEP_AUTHENTICATION_DISABLED";
		/// <summary>Error message: Login mail address is not setting</summary>
		public static string ERRMSG_LOGIN_MAIL_ADDRESS_NOT_SETTING = "ERRMSG_LOGIN_MAIL_ADDRESS_NOT_SETTING";
		/// <summary>Error manager login limited count error</summary>
		public static string ERRMSG_MANAGER_LOGIN_LIMITED_COUNT_ERROR = "ERRMSG_MANAGER_LOGIN_LIMITED_COUNT_ERROR";
		/// <summary>Resend authentication code success</summary>
		public static string ERRMSG_RESEND_AUTHENTICATION_CODE_SUCCESS = "ERRMSG_RESEND_AUTHENTICATION_CODE_SUCCESS";

		/// <summary>Error Message: Boku payment error</summary>
		public static string ERRMSG_FRONT_BOKU_PAYMENT_ERROR = "ERRMSG_FRONT_BOKU_PAYMENT_ERROR";
		/// <summary>Error Message: Boku payment process time out</summary>
		public static string ERRMSG_BOKU_PAYMENT_PROCESS_TIME_OUT = "ERRMSG_BOKU_PAYMENT_PROCESS_TIME_OUT";

		/// <summary>GMO掛け払い: 通知メッセージ</ summary>
		public const string ERRMSG_GMO_KB_PAYMENT_ALERT = "ERRMSG_GMO_KB_PAYMENT_ALERT";
		/// <summary>GMO掛け払い: 保留中のメッセージ</ summary>
		public const string ERRMSG_GMO_KB_PAYMENT_INREVIEW = "ERRMSG_GMO_KB_PAYMENT_INREVIEW";
		/// <summary>GMO掛け払い: 電話番号のメッセージ</ summary>
		public const string ERRMSG_INPUT_GMO_KB_MOBILE_PHONE = "ERRMSG_INPUT_GMO_KB_MOBILE_PHONE";
		/// <summary>GMO掛け払い: 交換できないときのメッセージ</ summary>
		public const string ERRMSG_GMO_KB_PAYMENT_EXCHANGE = "ERRMSG_GMO_KB_PAYMENT_EXCHANGE";
		/// <summary>GMO掛け払い:Apiエラー</summary>
		public const string ERRMSG_GMO_KB_PAYMENT_API_NG = "ERRMSG_GMO_KB_PAYMENT_API_NG";

		/// <summary>Error message: Manager order cvs rakuten cancel error</summary>
		public static string ERRMSG_MANAGER_ORDER_CVS_RAKUTEN_CANCEL_ERROR = "ERRMSG_MANAGER_ORDER_CVS_RAKUTEN_CANCEL_ERROR";

		/// <summary>Error message: Gift cart invalid</summary>
		public static string ERRMSG_FRONT_GIFT_CART_INVALID = "ERRMSG_FRONT_GIFT_CART_INVALID";

		/// <summary>Yahoo API 公開鍵認証失敗エラー</summary>
		public static string ERRMSG_MANAGER_YAHOO_API_PUBLIC_KEY_AUTHORIZED_FAILED = "ERRMSG_MANAGER_YAHOO_API_PUBLIC_KEY_AUTHORIZED_FAILED";
		/// <summary>Yahoo API 公開鍵入力エラー</summary>
		public static string ERRMSG_MANAGER_YAHOO_API_PUBLIC_KEY_INPUT_CHECK_FAILED = "ERRMSG_MANAGER_YAHOO_API_PUBLIC_KEY_INPUT_CHECK_FAILED";
		/// <summary>Yahoo API 公開鍵バージョン入力エラー</summary>
		public static string ERRMSG_MANAGER_YAHOO_API_PUBLIC_KEY_VERSION_INPUT_CHECK_FAILED = "ERRMSG_MANAGER_YAHOO_API_PUBLIC_KEY_VERSION_INPUT_CHECK_FAILED";

		/// <summary>エラーメッセージ: フロント受注編集時に全商品削除</summary>
		public static string ERRMSG_FRONT_ORDER_MODIFY_NOT_PRODUCT = "ERRMSG_FRONT_ORDER_MODIFY_NOT_PRODUCT";
		/// <summary>エラーメッセージ: フロント受注編集失敗時</summary>
		public static string ERRMSG_FRONT_ORDER_MODIFY_FAILURE = "ERRMSG_FRONT_ORDER_MODIFY_FAILURE";
		/// <summary>エラーメッセージ: フロント受注編集エスカレーション失敗時</summary>
		public static string ERRMSG_FRONT_ORDER_MODIFY_ESCALATION = "ERRMSG_FRONT_ORDER_MODIFY_ESCALATION";
		/// <summary>エラーメッセージ: フロント数量変更時にノベルティ商品を外した時</summary>
		public static string ERRMSG_FRONT_ORDER_MODIFY_REMOVE_NOVELTY = "ERRMSG_FRONT_ORDER_MODIFY_REMOVE_NOVELTY";

		/// <summary>Awoo商品連携 ファイル名不正エラー</summary>
		public static string ERRMSG_MANAGER_AWOO_PRODUCT_SYNC_FILE_PATH_NAME_INVALID = "ERRMSG_MANAGER_AWOO_PRODUCT_SYNC_FILE_PATH_NAME_INVALID";
		/// <summary>Awoo商品連携 連携データ作成エラー</summary>
		public static string ERRMSG_MANAGER_AWOO_PRODUCT_SYNC_CREATE_DATE_FAILED = "ERRMSG_MANAGER_AWOO_PRODUCT_SYNC_CREATE_DATE_FAILED";

		/// <summary>Error message: Letro system error</summary>
		public const string ERRMSG_LETRO_SYSTEM_ERROR = "SYSTEM_ERROR";
		/// <summary>Error message: Letro invalid authentication key</summary>
		public const string ERRMSG_LETRO_INVALID_AUTHENTICATION_KEY = "INVALID_AUTHENTICATION_KEY";
		/// <summary>Error message: Letro api success</summary>
		public const string ERRMSG_LETRO_API_SUCCESS = "ERRMSG_LETRO_API_SUCCESS";
		/// <summary>Error message: Letro user not exists</summary>
		public const string ERRMSG_LETRO_USER_NOT_EXISTS = "USER_NOT_EXISTS";
		/// <summary>Error message: Letro api letro option disabled</summary>
		public const string ERRMSG_LETRO_API_LETRO_OPTION_DISABLED = "ERRMSG_LETRO_API_LETRO_OPTION_DISABLED";
		/// <summary>Error message: Letro api unauthorized ip address</summary>
		public const string ERRMSG_LETRO_API_UNAUTHORIZED_IP_ADDRESS = "ERRMSG_LETRO_API_UNAUTHORIZED_IP_ADDRESS";
		/// <summary>Error message: Letro api user limit over</summary>
		public const string ERRMSG_LETRO_API_USER_LIMIT_OVER = "ERRMSG_LETRO_API_USER_LIMIT_OVER";
		/// <summary>Error message: Letro api date format error</summary>
		public const string ERRMSG_LETRO_API_DATE_FORMAT_ERROR = "ERRMSG_LETRO_API_DATE_FORMAT_ERROR";
		/// <summary>Error message: Letro api user id format error</summary>
		public const string ERRMSG_LETRO_API_USER_ID_FORMAT_ERROR = "ERRMSG_LETRO_API_USER_ID_FORMAT_ERROR";
		/// <summary>Error message: Front subscription box error message</summary>
		public const string ERRMSG_FRONT_SUBSCRIPTION_BOX_ERROR_MESSAGE = "ERRMSG_FRONT_SUBSCRIPTION_BOX_ERROR_MESSAGE";

		/// <summary>エラーメッセージ定数：Paygent決済連携エラー</summary>
		public static string ERRMSG_FRONT_PAYGENT_PAYMENT_ERROR = "ERRMSG_FRONT_PAYGENT_PAYMENT_ERROR";
		/// <summary>Error message: Cannot cancel because the payment method is ATM payment</summary>
		public const string ERRMSG_FRONT_MODIFY_NG_CANCEL_ORDER_PAYMENT_STATUS_CONFIRM_ALERT = "ERRMSG_FRONT_MODIFY_NG_CANCEL_ORDER_PAYMENT_STATUS_CONFIRM_ALERT";
		/// <summary>Error message: Cannot be canceled as payment has already been made</summary>
		public const string ERRMSG_FRONT_MODIFY_NG_CANCEL_ORDER_PAYMENT_STATUS_COMPLETE_ALERT = "ERRMSG_FRONT_MODIFY_NG_CANCEL_ORDER_PAYMENT_STATUS_COMPLETE_ALERT";
		/// <summary>Error message: front order cancel not good status alert</summary>
		public const string ERRMSG_FRONT_ORDER_CANCEL_NG_STATUS_ALERT = "ERRMSG_FRONT_ORDER_CANCEL_NG_STATUS_ALERT";
		/// <summary>Error message front paidy authorized</summary>
		public const string ERRMSG_FRONT_FAIL_PAIDY_AUTHORIZED = "ERRMSG_FRONT_FAIL_PAIDY_AUTHORIZED";
	}
}
