/*
=========================================================================================================
  Module      : WEBメッセージクラス(WebMessages.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;

///*********************************************************************************************
/// <summary>
/// WEBメッセージクラス
/// </summary>
///*********************************************************************************************
public class WebMessages : w2.App.Common.CommerceMessages
{
	//========================================================================
	// コマースフロントWEBメッセージ
	//========================================================================
	//------------------------------------------------------
	// 共通系エラー
	//------------------------------------------------------
	/// <summary>エラーメッセージ定数：パラメタ情報無し</summary>
	public static string ERRMSG_FRONT_NO_PARAM = "ERRMSG_FRONT_NO_PARAM";
	/// <summary>エラーメッセージ定数：不正パラメータエラー</summary>
	public static string ERRMSG_FRONT_IRREGULAR_PARAMETER_ERROR = "ERRMSG_FRONT_IRREGULAR_PARAMETER_ERROR";

	//------------------------------------------------------
	// セッション系エラー
	//------------------------------------------------------
	/// <summary>エラーメッセージ定数：ユーザセッション情報無し</summary>
	public static string ERRMSG_FRONT_NO_USER_SESSION = "ERRMSG_FRONT_NO_USER_SESSION";
	/// <summary>エラーメッセージ定数：遷移先セッション値不正</summary>
	public static string ERRMSG_FRONT_INCORRECT_NEXTURL_SESSION = "ERRMSG_FRONT_INCORRECT_NEXTURL_SESSION";
	/// <summary>エラーメッセージ定数：カート情報消失エラー</summary>
	public static string ERRMSG_FRONT_CART_SESSION_VANISHED = "ERRMSG_FRONT_CART_SESSION_VANISHED";
	/// <summary>エラーメッセージ定数：カート内商品無し（二重押しですでに注文されている可能性アリ）</summary>
	public static string ERRMSG_FRONT_CART_ALREADY_ORDERED = "ERRMSG_FRONT_CART_ALREADY_ORDERED";
	/// <summary>エラーメッセージ定数：別ブラウザーエラー（LINE PAYでWEBVIEWからLINEが呼ばれるとき）</summary>
	public static string ERRMSG_FRONT_ANOTHER_BROWSER_ERROR = "ERRMSG_FRONT_ANOTHER_BROWSER_ERROR";
	/// <summary>エラーメッセージ定数：LP入力情報消失エラー</summary>
	public static string ERRMSG_LANDINGCARTINPUT_INPUT_SESSION_VANISHED = "ERRMSG_LANDINGCARTINPUT_INPUT_SESSION_VANISHED";
	/// <summary>エラーメッセージ定数：PAYPAY不正エラー</summary>
	public static string ERRMSG_FRONT_PAYPAY_IRREGULAR_ERROR = "ERRMSG_FRONT_PAYPAY_IRREGULAR_ERROR";
	/// <summary>エラーメッセージ定数：PAYPAY重複請求エラー</summary>
	public static string ERRMSG_FRONT_PAYPAY_REPEATED_REQUEST_ERROR = "ERRMSG_FRONT_PAYPAY_REPEATED_REQUEST_ERROR";
	/// <summary>エラーメッセージ定数：PAYPAY不正ページ遷移エラー</summary>
	public static string ERRMSG_FRONT_PAYPAY_UNNORMAL_PAGE_REDIRECT_ERROR = "ERRMSG_FRONT_PAYPAY_UNNORMAL_PAGE_REDIRECT_ERROR";
	//------------------------------------------------------
	// ユーザー情報系エラー
	//------------------------------------------------------
	/// <summary>エラーメッセージ定数：ログインエラー</summary>
	public static string ERRMSG_FRONT_USER_LOGIN_ERROR = "ERRMSG_FRONT_USER_LOGIN_ERROR";
	/// <summary>エラーメッセージ定数：ログインエラー(メールアドレスのログインID利用時)</summary>
	public static string ERRMSG_FRONT_USER_LOGIN_IN_MAILADDR_ERROR = "ERRMSG_FRONT_USER_LOGIN_IN_MAILADDR_ERROR";
	/// <summary>エラーメッセージ定数：ログインエラー(ソーシャルログインで紐づけしようとしたが誰かと紐付いていたエラー)</summary>
	public static string ERRMSG_FRONT_COOPERATE_WITH_SOMEONE = "ERRMSG_FRONT_COOPERATE_WITH_SOMEONE";
	/// <summary>エラーメッセージ定数：パスワードリマインダエラー</summary>
	public static string ERRMSG_FRONT_PASSWORD_REMINDER_NO_USER = "ERRMSG_FRONT_PASSWORD_REMINDER_NO_USER";
	/// <summary>エラーメッセージ定数：アカウントロックエラー</summary>
	public static string ERRMSG_FRONT_USER_LOGIN_ACCOUNT_LOCK = "ERRMSG_FRONT_USER_LOGIN_ACCOUNT_LOCK";
	/// <summary>エラーメッセージ定数：アカウント連携時アカウントロックエラー</summary>
	public const string ERRMSG_FRONT_W2_EXISTING_USER_COOPERATION_ACCOUNT_LOCK = "ERRMSG_FRONT_W2_EXISTING_USER_COOPERATION_ACCOUNT_LOCK";
	/// <summary>エラーメッセージ定数：郵便番号住所検索エラー</summary>
	public static string ERRMSG_FRONT_ZIPCODE_NO_ADDR = "ERRMSG_FRONT_ZIPCODE_NO_ADDR";
	/// <summary>エラーメッセージ定数：郵便番号住所検索エラー</summary>
	public static string ERRMSG_FRONT_ZIPCODE_UNAVAILABLE_SHIPPING_AREA = "ERRMSG_FRONT_ZIPCODE_UNAVAILABLE_SHIPPING_AREA";
	/// <summary>エラーメッセージ定数：ユーザ配送先情報なしエラー</summary>
	public static string ERRMSG_FRONT_USERSHIPPING_NO_SHIPPING = "ERRMSG_FRONT_USERSHIPPING_NO_SHIPPING";
	/// <summary>エラーメッセージ定数：メールマガジン解除メールアドレス無しエラー</summary>
	public static string ERRMSG_FRONT_MAILMAGAZINE_CANCEL_NO_MAILADDR = "ERRMSG_FRONT_MAILMAGAZINE_CANCEL_NO_MAILADDR";
	/// <summary>エラーメッセージ定数：メールマガジン解除ユーザ情報無しエラー</summary>
	public static string ERRMSG_FRONT_MAILMAGAZINE_CANCEL_NO_USER = "ERRMSG_FRONT_MAILMAGAZINE_CANCEL_NO_USER";
	/// <summary>エラーメッセージ定数：変更前パスワードチェックエラー</summary>
	public static string ERRMSG_FRONT_PASSWORD_BEFORE_CHECK = "ERRMSG_FRONT_PASSWORD_BEFORE_CHECK";
	/// <summary>エラーメッセージ定数：認証エラー </summary>
	public static string ERRMSG_FRONT_PASSWORDREMINDER_AUTHENTICATION_NO_KEY = "ERRMSG_FRONT_PASSWORDREMINDER_AUTHENTICATION_NO_KEY";
	/// <summary>エラーメッセージ定数：試行回数オーバー</summary>
	public static string ERRMSG_FRONT_PASSWORDREMINDER_CHANGE_TRIAL_LIMIT_COUNT_OVER = "ERRMSG_FRONT_PASSWORDREMINDER_CHANGE_TRIAL_LIMIT_COUNT_OVER";
	/// <summary>エラーメッセージ定数：シリアルキー認証エラー</summary>
	public static string ERRMSG_FRONT_SERIALKEY_AUTH_FAILURE = "ERRMSG_FRONT_SERIALKEY_AUTH_FAILURE";
	/// <summary>エラーメッセージ定数：シリアルキー認証エラー（キャンセル済）</summary>
	public static string ERRMSG_FRONT_SERIALKEY_KEY_CANCELLED = "ERRMSG_FRONT_SERIALKEY_KEY_CANCELLED";
	/// <summary>エラーメッセージ定数：シリアルキー認証エラー（有効期限切れ）</summary>
	public static string ERRMSG_FRONT_SERIALKEY_KEY_EXPIRED = "ERRMSG_FRONT_SERIALKEY_KEY_EXPIRED";
	/// <summary>エラーメッセージ定数：シリアルキー認証エラー（ダウンロードURLなし）</summary>
	public static string ERRMSG_FRONT_SERIALKEY_AUTH_FAILURE_EMPTY_URL = "ERRMSG_FRONT_SERIALKEY_AUTH_FAILURE_EMPTY_URL";
	/// <summary>エラーメッセージ定数：LINEミニアプリ重複ユーザー登録エラー</summary>
	public const string ERRMSG_FRONT_TEMPORARY_USER_DUPLICATED = "ERRMSG_FRONT_TEMPORARY_USER_DUPLICATED";
	/// <summary>エラーメッセージ定数：アカウント連携エラー</summary>
	public const string ERRMSG_FRONT_W2_EXISTING_USER_COOPERATE_NOT_FOUND = "ERRMSG_FRONT_W2_EXISTING_USER_COOPERATE_NOT_FOUND";
	/// <summary>エラーメッセージ定数：メール配信登録解除：検証エラー</summary>
	public const string ERRMSG_FRONT_MAIL_UNSUBSCRIBEL_VERIFICATION_ERROR = "ERRMSG_FRONT_MAIL_UNSUBSCRIBEL_VERIFICATION_ERROR";

	//------------------------------------------------------
	// 商品系エラー
	//------------------------------------------------------
	/// <summary>エラーメッセージ定数：商品バリエーション未選択エラー</summary>
	public static string ERRMSG_FRONT_VARIATION_UNSELECTED = "ERRMSG_FRONT_VARIATION_UNSELECTED";
	/// <summary>ダブルドロップダウン使用時に組み合わせのないバリエーションが選ばれたときのエラー</summary>
	public static string ERRMSG_FRONT_USE_DOUBLEDROPDOWNLIST_COMBINATION_ERROR = "ERRMSG_FRONT_USE_DOUBLEDROPDOWNLIST_COMBINATION_ERROR";
	/// <summary>エラーメッセージ定数：商品バリエーション未選択エラー</summary>
	public static string ERRMSG_FRONT_OPTION_UNSELECTED = "ERRMSG_FRONT_OPTION_UNSELECTED";

	//------------------------------------------------------
	// 商品表示系エラー
	//------------------------------------------------------
	/// <summary>エラーメッセージ定数：商品一覧情報なしエラー</summary>
	public static string ERRMSG_FRONT_PRODUCT_NO_ITEM = "ERRMSG_FRONT_PRODUCT_NO_ITEM";
	/// <summary>エラーメッセージ定数： 商品詳細・商品表示不能エラー</summary>
	public static string ERRMSG_FRONT_PRODUCT_UNDISP = "ERRMSG_FRONT_PRODUCT_UNDISP";
	/// <summary>エラーメッセージ定数：商品詳細・商品表示不能エラー(定期会員限定)</summary>
	public static string ERRMSG_FRONT_PRODUCT_DISPLAY_FIXED_PURCHASE_MEMBER = "ERRMSG_FRONT_PRODUCT_DISPLAY_FIXED_PURCHASE_MEMBER";
	/// <summary>エラーメッセージ定数：商品検索文字列(検索ワード)数オーバーエラー</summary>
	public static string ERRMSG_FRONT_PRODUCTSEARCH_WORDS_NUMOVER = "ERRMSG_FRONT_PRODUCTSEARCH_WORDS_NUMOVER";

	/// <summary>エラーメッセージ定数：セット商品無しエラー</summary>
	public static string ERRMSG_FRONT_PRODUCTSET_NOPRODUCT = "ERRMSG_FRONT_PRODUCTSET_NOPRODUCT";
	/// <summary>エラーメッセージ定数：セット商品無しエラー</summary>
	public static string ERRMSG_FRONT_PRODUCTSET_UNSELECTED_PRODUCT = "ERRMSG_FRONT_PRODUCTSET_UNSELECTED_PRODUCT";

	/// <summary>エラーメッセージ定数：闇市ログインエラー</summary>
	public static string ERRMSG_FRONT_CLOSEDMARKET_PASSOWRD_ERROR = "ERRMSG_FRONT_CLOSEDMARKET_PASSOWRD_ERROR";
	/// <summary>エラーメッセージ定数：闇市準備中エラー</summary>
	public static string ERRMSG_FRONT_CLOSEDMARKET_PREPARING_ERROR = "ERRMSG_FRONT_CLOSEDMARKET_PREPARING_ERROR";
	/// <summary>エラーメッセージ定数：闇市終了済エラー</summary>
	public static string ERRMSG_FRONT_CLOSEDMARKET_ENDED_ERROR = "ERRMSG_FRONT_CLOSEDMARKET_ENDED_ERROR";
	/// <summary>エラーメッセージ定数：闇市無効エラー</summary>
	public static string ERRMSG_FRONT_CLOSEDMARKET_INVALID_ERROR = "ERRMSG_FRONT_CLOSEDMARKET_INVALID_ERROR";

	/// <summary>エラーメッセージ定数：商品付帯情報選択エラー</summary>
	public static string ERRMSG_FRONT_PRODUCT_PRODUCT_OPTION_VALUE_VALID_ERROR = "ERRMSG_FRONT_PRODUCT_PRODUCT_OPTION_VALUE_VALID_ERROR";

	/// <summary>エラーメッセージ定数：ランディング：商品エラー</summary>
	public static string ERRMSG_FRONT_LANDING_NO_PRODUCT_ERROR = "ERRMSG_FRONT_LANDING_NO_PRODUCT_ERROR";
	/// <summary>エラーメッセージ定数：ランディング：商品無効エラー</summary>
	public static string ERRMSG_FRONT_LANDING_NOT_VALID_PRODUCT_ERROR = "ERRMSG_FRONT_LANDING_NOT_VALID_PRODUCT_ERROR";
	/// <summary>エラーメッセージ定数：ランディング：カート内有効な商品がないエラー</summary>
	public static string ERRMSG_FRONT_LANDING_VALID_PRODUCT_NOTINCART_ERROR = "ERRMSG_FRONT_LANDING_VALID_PRODUCT_NOTINCART_ERROR";
	/// <summary>エラーメッセージ定数：ランディング：頒布会商品期間エラー</summary>
	public static string ERRMSG_FRONT_LANDING_VALID_OUT_OF_PERIOD_ERROR = "ERRMSG_FRONT_LANDING_VALID_OUT_OF_PERIOD_ERROR";
	/// <summary>エラーメッセージ定数：ランディング：未ログイン時、販売可能会員ランクエラー</summary>
	public static string ERRMSG_MANAGER_LANDING_PAGE_NOT_LOGIN_AND_PRODUCT_BUYBLE_MEMBER_RANK_ERROR = "ERRMSG_MANAGER_LANDING_PAGE_NOT_LOGIN_AND_PRODUCT_BUYBLE_MEMBER_RANK_ERROR";
	/// <summary>エラーメッセージ定数：ランディング：ログインユーザーのランクが販売可能会員ランクより小さいエラー</summary>
	public static string ERRMSG_MANAGER_LANDING_PAGE_LOWER_USER_RANK_THAN_BUYBLE_MEMBERRANK_ERROR = "ERRMSG_MANAGER_LANDING_PAGE_LOWER_USER_RANK_THAN_BUYBLE_MEMBERRANK_ERROR";

	//------------------------------------------------------
	// マイページ系エラー
	//------------------------------------------------------
	/// <summary>エラーメッセージ定数：お気に入り情報なしエラー</summary>
	public static string ERRMSG_FRONT_FAVORITE_NO_ITEM = "ERRMSG_FRONT_FAVORITE_NO_ITEM";
	/// <summary>エラーメッセージ定数：いいね情報なしエラー</summary>
	public static string ERRMSG_FRONT_LIKE_NO_ITEM = "ERRMSG_FRONT_LIKE_NO_ITEM";
	/// <summary>エラーメッセージ定数：フォロー情報なしエラー</summary>
	public static string ERRMSG_FRONT_FOLLOW_NO_ITEM = "ERRMSG_FRONT_FOLLOW_NO_ITEM";
	/// <summary>エラーメッセージ定数：注文履歴一覧情報なしエラー</summary>
	public static string ERRMSG_FRONT_ORDERHISTORY_NO_ITEM = "ERRMSG_FRONT_ORDERHISTORY_NO_ITEM";
	/// <summary>エラーメッセージ定数：注文履歴詳細情報なしエラー</summary>
	public static string ERRMSG_FRONT_ORDERHISTORY_UNDISP = "ERRMSG_FRONT_ORDERHISTORY_UNDISP";

	/// <summary>エラーメッセージ定数：定期購入情報なしエラー</summary>
	public static string ERRMSG_FRONT_FIXEDPURCHASE_UNDISP = "ERRMSG_FRONT_FIXEDPURCHASE_UNDISP";
	/// <summary>エラーメッセージ定数：定期購入カード与信成功メッセージ</summary>
	public static string ERRMSG_FRONT_CARD_AUTH_SUCCESS = "ERRMSG_FRONT_FIXEDPURCHASE_CARD_AUTH_SUCCESS";
	/// <summary>エラーメッセージ定数：定期購入カード与信失敗エラー</summary>
	public static string ERRMSG_FRONT_CARD_AUTH_FAILED = "ERRMSG_FRONT_FIXEDPURCHASE_CARD_AUTH_FAILED";
	/// <summary>エラーメッセージ定数：定期購入商品は全て存在しないエラー</summary>
	public static string ERRMSG_FRONT_FIXEDPURCHASE_NO_ITEMS = "ERRMSG_FRONT_FIXEDPURCHASE_NO_ITEMS";


	/// <summary>エラーメッセージ定数：受信メール情報なしエラー</summary>
	public static string ERRMSG_FRONT_USERRECIEVEMAIL_NO_LIST = "ERRMSG_FRONT_USERRECIEVEMAIL_NO_LIST";
	public static string ERRMSG_FRONT_USERRECIEVEMAIL_NO_ITEM = "ERRMSG_FRONT_USERRECIEVEMAIL_NO_ITEM";

	/// <summary>エラーメッセージ定数：入荷通知メール情報なしエラー</summary>
	public static string ERRMSG_FRONT_USERPRODUCTARRIVALMAIL_NO_ITEM = "ERRMSG_FRONT_USERPRODUCTARRIVALMAIL_NO_ITEM";

	/// <summary>エラーメッセージ定数：ユーザクレジット情報なしエラー</summary>
	public static string ERRMSG_FRONT_USERCREDITCARD_NO_CARD = "ERRMSG_FRONT_USERCREDITCARD_NO_CARD";

	/// <summary>エラーメッセージ定数：ポイント履歴一覧情報なしエラー</summary>
	public static string ERRMSG_FRONT_USER_POINT_HISTORY_UNDISP = "ERRMSG_FRONT_USER_POINT_HISTORY_UNDISP";

	/// <summary>エラーメッセージ定数：クーポンBOX 利用可能クーポンなしエラー</summary>
	public static string ERRMSG_FRONT_COUPONBOX_NO_ITEM = "ERRMSG_FRONT_COUPONBOX_NO_ITEM";

	/// <summary>エラーメッセージ定数：支払い方法が一つも取得できなかった場合アラート</summary>
	public static string ERRMSG_FRONT_MODIFY_NG_NO_EXIST_PAYMENT_ALERT = "ERRMSG_FRONT_MODIFY_NG_NO_EXIST_PAYMENT_ALERT";

	/// <summary>エラーメッセージ定数：支払い方法による変更NGアラート</summary>
	public static string ERRMSG_FRONT_MODIFY_NG_PAYMENT_ALERT = "ERRMSG_FRONT_MODIFY_NG_PAYMENT_ALERT";

	/// <summary>エラーメッセージ定数：注文ステータスによる変更NGアラート</summary>
	public static string ERRMSG_FRONT_MODIFY_NG_ORDER_STATUS_ALERT = "ERRMSG_FRONT_MODIFY_NG_ORDER_STATUS_ALERT";

	/// <summary>エラーメッセージ定数：配送方法による変更NGアラート</summary>
	public static string ERRMSG_FRONT_MODIFY_NG_SHIPPING_METHOD_ALERT = "ERRMSG_FRONT_MODIFY_NG_SHIPPING_METHOD_ALERT";

	/// <summary>エラーメッセージ定数：更新期限超過による変更NGアラート</summary>
	public static string ERRMSG_FRONT_MODIFY_NG_DAY_OVER_ALERT = "ERRMSG_FRONT_MODIFY_NG_DAY_OVER_ALERT";

	/// <summary>エラーメッセージ定数：モール注文による変更NGアラート</summary>
	public static string ERRMSG_FRONT_MODIFY_NG_MALL_ALERT = "ERRMSG_FRONT_MODIFY_NG_MALL_ALERT";

	/// <summary>エラーメッセージ定数：入金済みによる変更NGアラート</summary>
	public static string ERRMSG_FRONT_MODIFY_NG_PAYMENT_STATUS_COMPLETE_ALERT = "ERRMSG_FRONT_MODIFY_NG_PAYMENT_STATUS_COMPLETE_ALERT";

    /// <summary>エラーメッセージ定数：定期解約時アラート</summary>
    public static string ERRMSG_FRONT_AlertOrderStatusForCancelFixedPurchase = "ERRMSG_FRONT_AlertOrderStatusForCancelFixedPurchase";

    /// <summary>エラーメッセージ定数：定期一時停止時アラート</summary>
    public static string ERRMSG_FRONT_MYPAGE_FIXEDPURCHASE_PAUSE_ALERT = "ERRMSG_FRONT_MYPAGE_FIXEDPURCHASE_PAUSE_ALERT";

	/// <summary>注文同梱エラー：フロント注文同梱エラー</summary>
	public static string ERRMSG_FRONT_ORDERCOMBINE_ERROR = "ERRMSG_FRONT_ORDERCOMBINE_ERROR";
	/// <summary>注文同梱エラー：注文同梱内容確認メッセージ</summary>
	public static string ERRMSG_FRONT_INHERITED_ORDER_INFO = "ERRMSG_FRONT_INHERITED_ORDER_INFO";
	/// <summary>エラーメッセージ：選択された同梱親注文が注文同梱不可</summary>
	public const string ERRMSG_FRONT_ORDERCOMBINE_SELECTED_PARENT_ORDER_CANNOT_COMBINE =
		"ERRMSG_FRONT_ORDERCOMBINE_SELECTED_PARENT_ORDER_CANNOT_COMBINE";

	/// <summary>エラーメッセージ定数：注文キャンセル可能時間</summary>
	public static string ERRMSG_ORDER_CANCEL_TIME = "ERRMSG_ORDER_CANCEL_TIME";

	/// <summary>エラーメッセージ定数：入金済みでマイページからのキャンセルができないエラー</summary>
	public static string ERRMSG_FRONT_MODIFY_NG_PAYMENT_STATUS_COMPLETE_CANCEL = "ERRMSG_FRONT_MODIFY_NG_PAYMENT_STATUS_COMPLETE_CANCEL";

	//------------------------------------------------------
	// 注文系エラー
	//------------------------------------------------------
	/// <summary>エラーメッセージ定数：注文方法の保存 ユーザー配送先情報登録なしエラー</summary>
	public static string ERRMSG_FRONT_USER_DEFAULT_ORDER_SETTING_USERSHIPPING_NO_SHIPPING = "ERRMSG_FRONT_USER_DEFAULT_ORDER_SETTING_USERSHIPPING_NO_SHIPPING";
	/// <summary>エラーメッセージ定数：注文方法の保存 ユーザークレジットカード情報登録なしエラー</summary>
	public static string ERRMSG_FRONT_USER_DEFAULT_ORDER_SETTING_USERCREDITCARD_NO_CARD = "ERRMSG_FRONT_USER_DEFAULT_ORDER_SETTING_USERCREDITCARD_NO_CARD";
	/// <summary>エラーメッセージ定数：PayPal：ログイン必須エラー</summary>
	public static string ERRMSG_FRONT_PAYPAL_NEEDS_LOGIN_ERROR = "ERRMSG_FRONT_PAYPAL_NEEDS_LOGIN_ERROR";
	/// <summary>送料算出エラー</summary>
	public static string ERRMSG_FRONT_CALC_SHIPPING_ERROR = "ERRMSG_FRONT_CALC_SHIPPING_ERROR";
	/// <summary>Error Message Get Token Id</summary>
	public static string ERRMSG_FRONT_PAIDY_GET_TOKEN_ERROR = "ERRMSG_FRONT_PAIDY_GET_TOKEN_ERROR";
	/// <summary>Error Message Paidy Token Id Existed</summary>
	public static string ERRMSG_FRONT_PAIDY_TOKEN_ID_EXISTED_ERROR = "ERRMSG_FRONT_PAIDY_TOKEN_ID_EXISTED_ERROR";

	/// <summary>エラーメッセージ定数：LPカート画面公開期間外エラー</summary>
	public static string ERRMSG_FRONT_LANDING_PAGE_OUT_OF_RELEASED_HOURS_ERROR = "ERRMSG_FRONT_LANDING_PAGE_OUT_OF_RELEASED_HOURS_ERROR";

	/// <summary>エラーメッセージ定数：Amazon Pay:ログイン必須エラー</summary>
	public static string ERRMSG_FRONT_AMAZON_NEEDS_LOGIN_ERROR = "ERRMSG_FRONT_AMAZON_NEEDS_LOGIN_ERROR";

	//------------------------------------------------------
	// レコメンド系エラー
	//------------------------------------------------------
	/// <summary>レコメンド商品の注文失敗(注文情報更新失敗)エラー</summary>
	public static string ERRMSG_FRONT_RECOMMEND_CANNOT_CREATE_UPDATED_ORDER = "ERRMSG_FRONT_RECOMMEND_CANNOT_CREATE_UPDATED_ORDER";
	/// <summary>アップセルで商品がなくなった注文のキャンセル失敗エラー</summary>
	public static string ERRMSG_FRONT_RECOMMEND_CANNOT_CANCEL_UPSELL_TARGET_ORDER = "ERRMSG_FRONT_RECOMMEND_CANNOT_CANCEL_UPSELL_TARGET_ORDER";

	/// <summary>配送先に選択できない国エラー</summary>
	public static string ERRMSG_FRONT_SHIPPING_COUNTRY_UNAVAILABLE = "ERRMSG_FRONT_SHIPPING_COUNTRY_UNAVAILABLE";

	/// <summary>Error Shipping Convenience Store</summary>
	public static string ERRMSG_FRONT_SHIPPING_CONVENIENCE_STORE = "ERRMSG_FRONT_SHIPPING_CONVENIENCE_STORE";

	//------------------------------------------------------
	// 楽天IDConnnect連携
	//------------------------------------------------------
	/// <summary>楽天ID連携済エラー</summary>
	public static string ERRMSG_RAKUTEN_ID_CONNECT_CONNECTED = "ERRMSG_RAKUTEN_ID_CONNECT_CONNECTED";
	/// <summary>楽天ID未連携エラー</summary>
	public static string ERRMSG_RAKUTEN_ID_CONNECT_NOT_CONNECTED = "ERRMSG_RAKUTEN_ID_CONNECT_NOT_CONNECTED";
	/// <summary>自社会員登録済 AND 楽天IDConnect会員登録エラー</summary>
	public static string ERRMSG_RAKUTEN_ID_CONNECT_USER_REGISTERED = "ERRMSG_RAKUTEN_ID_CONNECT_USER_REGISTERED";

	//------------------------------------------------------
	// JAFログイン連携エラー
	//------------------------------------------------------
	/// <summary>JAFログイン連携：ログイン必須エラー</summary>
	public static string ERRMSG_FRONT_JAF_NEEDS_LOGIN_ERROR = "ERRMSG_FRONT_JAF_NEEDS_LOGIN_ERROR";
	/// <summary>JAFログイン連携：会員登録時の詳細説明文・注意事項</summary>
	public static string ERRMSG_FRONT_JAF_REGISTER_DESCRIPTION = "ERRMSG_FRONT_JAF_REGISTER_DESCRIPTION";

	// コーディネート
	/// <summary>エラーメッセージ定数：コーディネート検索文字列(検索ワード)数オーバーエラー</summary>
	public static string ERRMSG_FRONT_COORDINATESEARCH_WORDS_NUMOVER = "ERRMSG_FRONT_COORDINATESEARCH_WORDS_NUMOVER";
	/// <summary>エラーメッセージ定数： コーディネート詳細・コーディネート表示不能エラー</summary>
	public static string ERRMSG_FRONT_COORDINATE_UNDISP = "ERRMSG_FRONT_COORDINATE_UNDISP";
	/// <summary>エラーメッセージ定数：コーディネート一覧情報なしエラー</summary>
	public static string ERRMSG_FRONT_COORDINATE_NO_ITEM = "ERRMSG_FRONT_COORDINATE_NO_ITEM";

	// 特集ページ
	/// <summary>エラーメッセージ定数：該当する特集ページカテゴリがありません</summary>
	public static string ERRMSG_FRONT_FEATUREPAGE_NO_CATEGORY = "ERRMSG_FRONT_FEATUREPAGE_NO_CATEGORY";
	/// <summary>エラーメッセージ定数：該当する特集ページがありません</summary>
	public static string ERRMSG_FRONT_FEATUREPAGE_NO_ITEM = "ERRMSG_FRONT_FEATUREPAGE_NO_ITEM";

	/// <summary>領収書情報エラーメッセージ定数：領収書情報出力済みエラー</summary>
	public static string ERRMSG_FRONT_RECEIPT_SETTLED_RECEIPTDOWNLOAD = "ERRMSG_FRONT_RECEIPT_SETTLED_RECEIPTDOWNLOAD";
	/// <summary>領収書情報エラーメッセージ定数：領収書情報なしエラー</summary>
	public static string ERRMSG_FRONT_RECEIPT_NO_RECEIPTINFO = "ERRMSG_FRONT_RECEIPT_NO_RECEIPTINFO";
	/// <summary>領収書情報エラーメッセージ定数：領収書情報なしエラー</summary>
	public static string ERRMSG_FRONT_RECEIPT_FAILURE_RECEIPTDOWNLOAD = "ERRMSG_FRONT_RECEIPT_FAILURE_RECEIPTDOWNLOAD";
	/// <summary>領収書情報エラーメッセージ定数：注文キャンセル済みエラー</summary>
	public static string ERRMSG_FRONT_RECEIPT_ORDER_CANCELED = "ERRMSG_FRONT_RECEIPT_ORDER_CANCELED";

	/// <summary>Error Message: No User Invoice Information</summary>
	public static string ERRMSG_FRONT_USERINVOICE_NO_INVOICE = "ERRMSG_FRONT_USERINVOICE_NO_INVOICE";
	/// <summary>Error Message: Carry Type Option 8 invalid format</summary>
	public static string ERRMSG_MOBILE_CARRY_TYPE_OPTION_8 = "ERRMSG_MOBILE_CARRY_TYPE_OPTION_8";
	/// <summary>Error Message: Carry Type Option 16 invalid format</summary>
	public static string ERRMSG_CERTIFICATE_CARRY_TYPE_OPTION_16 = "ERRMSG_CERTIFICATE_CARRY_TYPE_OPTION_16";

	/// <summary>コンビニが無効の場合、エラーメッセージ</summary>
	public static string ERRMSG_FRONT_GROCERY_STORE = "ERRMSG_FRONT_GROCERY_STORE";

	/// <summary>Error Message: Check Country For Payment Atone</summary>
	public static string ERRMSG_FRONT_CHECK_COUNTRY_FOR_PAYMENT_ATONE = "ERRMSG_FRONT_CHECK_COUNTRY_FOR_PAYMENT_ATONE";
	/// <summary>Error Message: Check Tel No For Payment Atone</summary>
	public static string ERRMSG_FRONT_CHECK_TEL_NO_FOR_PAYMENT_ATONE = "ERRMSG_FRONT_CHECK_TEL_NO_FOR_PAYMENT_ATONE";
	/// <summary>Error Message: Check Country For Payment Aftee</summary>
	public static string ERRMSG_FRONT_CHECK_COUNTRY_FOR_PAYMENT_AFTEE = "ERRMSG_FRONT_CHECK_COUNTRY_FOR_PAYMENT_AFTEE";
	/// <summary>Error Message: Check Tel No For Payment Aftee</summary>
	public static string ERRMSG_FRONT_CHECK_TEL_NO_FOR_PAYMENT_AFTEE = "ERRMSG_FRONT_CHECK_TEL_NO_FOR_PAYMENT_AFTEE";
	/// <summary>Error Message: Check Owner Error Aftee</summary>
	public static string ERRMSG_FRONT_CHECK_OWNER_ERROR_AFTEE = "ERRMSG_FRONT_CHECK_OWNER_ERROR_AFTEE";
	/// <summary>Error Message: Check Owner Error Atone</summary>
	public static string ERRMSG_FRONT_CHECK_OWNER_ERROR_ATONE = "ERRMSG_FRONT_CHECK_OWNER_ERROR_ATONE";

	/// <summary>Error Message: Payment method changed to convenience store</summary>
	public static string ERRMSG_FRONT_PAYMENT_METHOD_CHANGED_TO_CONVENIENCE_STORE = "ERRMSG_FRONT_PAYMENT_METHOD_CHANGED_TO_CONVENIENCE_STORE";

	/// <summary>エラーメッセージ定数：Error Change Point Use For EcPay</summary>
	public static string ERRMSG_FRONT_CHANGE_POINT_USE_FOR_ECPAY = "ERRMSG_FRONT_CHANGE_POINT_USE_FOR_ECPAY";
	/// <summary>エラーメッセージ定数：Error Change Information Use For EcPay</summary>
	public static string ERRMSG_FRONT_CHANGE_INFORMATION_FOR_ECPAY = "ERRMSG_FRONT_CHANGE_INFORMATION_FOR_ECPAY";

	/// <summary>Error Message : Error Change Point Use For NewebPay</summary>
	public static string ERRMSG_FRONT_CHANGE_POINT_USE_FOR_NEWEBPAY = "ERRMSG_FRONT_CHANGE_POINT_USE_FOR_NEWEBPAY";
	/// <summary>Error Message：Error Change Information Use For NewebPay</summary>
	public static string ERRMSG_FRONT_CHANGE_INFORMATION_FOR_NEWEBPAY = "ERRMSG_FRONT_CHANGE_INFORMATION_FOR_NEWEBPAY";

	/// <summary>Error Message: Shipping Pattern Unselected</summary>
	public static string ERRMSG_FRONT_SHIPPING_PATTERN_UNSELECTED = "ERRMSG_FRONT_SHIPPING_PATTERN_UNSELECTED";
	/// <summary>Error Message: Resumed Fixed Purchase</summary>
	public static string ERRMSG_FRONT_RESUMED_FIXED_PURCHASE = "ERRMSG_FRONT_RESUMED_FIXED_PURCHASE";
	/// <summary>Error Message: Product Option Error</summary>
	public static string ERRMSG_FRONT_PRODUCT_OPTION_ERROR = "ERRMSG_FRONT_PRODUCT_OPTION_ERROR";
	/// <summary>Error message: 付帯情報の必須未選択エラー</summary>
	public static string ERRMSG_FRONT_PRODUCT_OPTION_NOT_SELECTED_ITEM_ERROR = "ERRMSG_FRONT_PRODUCT_OPTION_NOT_SELECTED_ITEM_ERROR";
	/// <summary>Error Message: Product Set Error</summary>
	public static string ERRMSG_FRONT_PRODUCT_SET_ERROR = "ERRMSG_FRONT_PRODUCT_SET_ERROR";
	/// <summary>Error Message: The Product Of Does Not Exist</summary>
	public static string ERRMSG_FRONT_PRODUCT_OF_DOES_NOT_EXIST = "ERRMSG_FRONT_PRODUCT_OF_DOES_NOT_EXIST";
	/// <summary>Error Message: Send Docomo Payment Mail Skip</summary>
	public static string ERRMSG_FRONT_SEND_DOCOMO_PAYMENT_MAIL_SKIP = "ERRMSG_FRONT_SEND_DOCOMO_PAYMENT_MAIL_SKIP";
	/// <summary>Error Message: Send Docomo Payment Mail Success</summary>
	public static string ERRMSG_FRONT_SEND_DOCOMO_PAYMENT_MAIL_SUCCESS = "ERRMSG_FRONT_SEND_DOCOMO_PAYMENT_MAIL_SUCCESS";

	/// <summary>定期購入の支払い方法変更時に継続課金（定期・従量）の解約失敗エラ</summary>
	public static string ERRMSG_FRONT_PAYMENT_CONTINUOUS_CANCEL_NG_FOR_CHANGE_PAYMENT_FP =
		"ERRMSG_FRONT_PAYMENT_CONTINUOUS_CANCEL_NG_FOR_CHANGE_PAYMENT_FP";
	/// <summary>定期購入の解約時に継続課金（定期・従量）の解約失敗エラー</summary>
	public static string ERRMSG_FRONT_PAYMENT_CONTINUOUS_CANCEL_NG_FOR_CANCEL_FP =
		"ERRMSG_FRONT_PAYMENT_CONTINUOUS_CANCEL_NG_FOR_CANCEL_FP";

	/// <summary>定期購入不可エラー</summary>
	public static string ERRMSG_FRONT_FIXED_PURCHASE_INVALID = "ERRMSG_FRONT_FIXED_PURCHASE_INVALID";
	/// <summary>定期商品変更済みエラー</summary>
	public static string ERRMSG_FRONT_FIXED_PURCHASE_ITEM_ALREADY_CHANGED = "ERRMSG_FRONT_FIXED_PURCHASE_ITEM_ALREADY_CHANGED";

	/// <summary>PayPay auth error</summary>
	public static string ERRMSG_FRONT_PAYPAY_AUTH_ERROR = "ERRMSG_FRONT_PAYPAY_AUTH_ERROR";
	/// <summary>Error Message: Error change point use for PayPay</summary>
	public static string ERRMSG_FRONT_CHANGE_POINT_USE_FOR_PAYPAY = "ERRMSG_FRONT_CHANGE_POINT_USE_FOR_PAYPAY";
	/// <summary>Error Message: Notice of Papay payment when canceled</summary>
	public static string ERRMSG_FRONT_PAYMENT_PAPAY_GMO_NOTIFICATION = "ERRMSG_FRONT_PAYMENT_PAPAY_GMO_NOTIFICATION";
	/// <summary>エラーメッセージ: ベリトランスPaypayの解約時</summary>
	public static string ERRMSG_FRONT_PURCHASE_CANCEL_PAYMENT_PAPAY_VERITRANS_MESSAGE = "ERRMSG_FRONT_PURCHASE_CANCEL_PAYMENT_PAPAY_VERITRANS_MESSAGE";
	/// <summary>エラーメッセージ: エラーメッセージ: ベリトランスPaypay再開できない</summary>
	public static string ERRMSG_FRONT_PURCHASE_RESUMED_PAYMENT_PAPAY_VERITRANS_MESSAGE = "ERRMSG_FRONT_PURCHASE_RESUMED_PAYMENT_PAPAY_VERITRANS_MESSAGE";
	/// <summary>Message manager: Note cancel Paypay Gmo</summary>
	public static string ERRMSG_FRONT_FIXED_PURCHASE_CANCEL_PAYPAY_GMO_MESSAGE = "ERRMSG_FRONT_FIXED_PURCHASE_CANCEL_PAYPAY_GMO_MESSAGE";
	/// <summary>エラーメッセージ：決済金額増額不可</summary>
	public static string ERRMSG_FRONT_CHANGE_INFORMATION_FOR_PAYPAY = "ERRMSG_FRONT_CHANGE_INFORMATION_FOR_PAYPAY";

	/// <summary>タイマータグ日付指定エラー</summary>
	public static string ERRMSG_TIMER_TAG_DATE_SPECIFICATION = "ERRMSG_TIMER_TAG_DATE_SPECIFICATION";
	/// <summary>タイマータグ曜日指定エラー</summary>
	public static string ERRMSG_TIMER_TAG_WEEKDAY_SPECIFICATION = "ERRMSG_TIMER_TAG_WEEKDAY_SPECIFICATION";

	/// <summary>Error Message: User verification code is incorrect</summary>
	public static string ERRMSG_FRONT_USER_VERIFICATION_CODE_IS_INCORRECT = "ERRMSG_FRONT_USER_VERIFICATION_CODE_IS_INCORRECT";

	/// <summary>Error Message: Scoring sale no data</summary>
	public static string ERRMSG_FRONT_SCORINGSALE_NO_DATA = "ERRMSG_FRONT_SCORINGSALE_NO_DATA";
	/// <summary>Error Message: Scoring sale session expired</summary>
	public static string ERRMSG_FRONT_SCORINGSALE_SESSION_EXPIRED = "ERRMSG_FRONT_SCORINGSALE_SESSION_EXPIRED";

	/// <summary>定額でない頒布会コース設定の商品合計金額（税込み）を満たしていないエラー</summary>
	public const string ERRMSG_FRONT_SUBSCRIPTION_BOX_DOES_NOT_MEET_AMOUNT_SET_FOR_NON_FIXED = "ERRMSG_FRONT_SUBSCRIPTION_BOX_DOES_NOT_MEET_AMOUNT_SET_FOR_NON_FIXED";
	/// <summary>定額でない頒布会コース設定の商品合計金額（税込み）下限を満たしていないエラー</summary>
	public const string ERRMSG_FRONT_SUBSCRIPTION_BOX_DOES_NOT_MEET_MINIMUM_AMOUNT_SET_FOR_NON_FIXED = "ERRMSG_FRONT_SUBSCRIPTION_BOX_DOES_NOT_MEET_MINIMUM_AMOUNT_SET_FOR_NON_FIXED";
	/// <summary>定額でない頒布会コース設定の商品合計金額（税込み）上限限を満たしていないエラー</summary>
	public const string ERRMSG_FRONT_SUBSCRIPTION_BOX_DOES_NOT_MEET_MAXIMUM_AMOUNT_SET_FOR_NON_FIXED = "ERRMSG_FRONT_SUBSCRIPTION_BOX_DOES_NOT_MEET_MAXIMUM_AMOUNT_SET_FOR_NON_FIXED";
	/// <summary>定額の頒布会コース設定の商品合計金額（税込み）を満たしていないエラー</summary>
	public const string ERRMSG_FRONT_SUBSCRIPTION_BOX_DOES_NOT_MEET_AMOUNT_SET_FOR_FIXED = "ERRMSG_FRONT_SUBSCRIPTION_BOX_DOES_NOT_MEET_AMOUNT_SET_FOR_FIXED";
	/// <summary>定額の頒布会コース設定の商品合計金額（税込み）下限を満たしていないエラー</summary>
	public const string ERRMSG_FRONT_SUBSCRIPTION_BOX_DOES_NOT_MEET_MINIMUM_AMOUNT_SET_FOR_FIXED = "ERRMSG_FRONT_SUBSCRIPTION_BOX_DOES_NOT_MEET_MINIMUM_AMOUNT_SET_FOR_FIXED";
	/// <summary>定額の頒布会コース設定の商品合計金額（税込み）上限を満たしていないエラー</summary>
	public const string ERRMSG_FRONT_SUBSCRIPTION_BOX_DOES_NOT_MEET_MAXIMUM_AMOUNT_SET_FOR_FIXED = "ERRMSG_FRONT_SUBSCRIPTION_BOX_DOES_NOT_MEET_MAXIMUM_AMOUNT_SET_FOR_FIXED";
	/// <summary>頒布会：1回目配送商品選択可能期間外</summary>
	public const string ERRMSG_FRONT_SUBSCRIPTION_BOX_OUTSIDE_SELECTION_PERIOD = "ERRMSG_FRONT_SUBSCRIPTION_BOX_OUTSIDE_SELECTION_PERIOD";

	/// <summary>Amazon Payの連携解除（商品選択肢を変更したときに通常購入から定期購入、あるいはその逆に変更がある場合）</summary>
	public const string ERRMSG_FRONT_AMAZON_PAY_AUTO_UNLINK = "ERRMSG_FRONT_AMAZON_PAY_AUTO_UNLINK";

	/// <summary>Error Message: Product not valid for store pickup</summary>
	public static string ERRMSG_FRONT_SHIPPING_STOREPICKUP = "ERRMSG_FRONT_SHIPPING_STOREPICKUP";

	/// <summary>Error message front: fail paidy authorized</summary>
	public const string ERRMSG_FRONT_FAIL_PAIDY_AUTHORIZED = "ERRMSG_FRONT_FAIL_PAIDY_AUTHORIZED";

	/// <summary>Error message front order cancel not good payment alert</summary>
	public const string ERRMSG_FRONT_ORDER_CANCEL_NG_PAYMENT_ALERT = "ERRMSG_FRONT_ORDER_CANCEL_NG_PAYMENT_ALERT";
	/// <summary>Error message front order cancel not good status alert</summary>
	public const string ERRMSG_FRONT_ORDER_CANCEL_NG_STATUS_ALERT = "ERRMSG_FRONT_ORDER_CANCEL_NG_STATUS_ALERT";
	/// <summary>Error message: Cancel order for combine fail</summary>
	public const string ERRMSG_FRONT_CANCEL_ORDER_FOR_COMBINE_FAIL = "ERRMSG_FRONT_CANCEL_ORDER_FOR_COMBINE_FAIL";
}
