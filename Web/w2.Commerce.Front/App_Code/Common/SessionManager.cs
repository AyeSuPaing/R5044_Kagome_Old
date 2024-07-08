/*
=========================================================================================================
  Module      : セッション管理クラス(SessionManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.Auth.RakutenIDConnect;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Product;
using w2.App.Common.User.SocialLogin.Helper;
using w2.Domain.SubscriptionBox;

// ReSharper disable CheckNamespace

/// <summary>
/// セッション管理クラス
/// </summary>
public class SessionManager
{
	/// <summary>
	/// LPカートの遷移先ページをセット
	/// </summary>
	/// <param name="cartList">カート</param>
	/// <returns>遷移先ページ</returns>
	public static void SetLandingCartNextPageForCheck(CartObjectList cartList, string value)
	{
		var key = Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK + cartList.LandingCartInputAbsolutePath;
		Session[key] = value;
	}

	/// <summary>エラーページ表示メッセージ</summary>
	public static string MessageForErrorPage
	{
		get { return (string)Session[Constants.SESSION_KEY_ERROR_MSG]; }
		set { Session[Constants.SESSION_KEY_ERROR_MSG] = value; }
	}
	/// <summary>PayPalログイン結果</summary>
	public static PayPalLoginResult PayPalLoginResult
	{
		get { return (PayPalLoginResult)Session[Constants.SESSION_KEY_PAYPAL_LOGIN_RESULT]; }
		set { Session[Constants.SESSION_KEY_PAYPAL_LOGIN_RESULT] = value; }
	}
	/// <summary>PayPal連携情報</summary>
	public static PayPalCooperationInfo PayPalCooperationInfo
	{
		get { return (PayPalCooperationInfo)Session[Constants.SESSION_KEY_PAYPAL_COOPERATION_INFO]; }
		set { Session[Constants.SESSION_KEY_PAYPAL_COOPERATION_INFO] = value; }
	}
	/// <summary>PayPal注文エラー発生したか</summary>
	public static bool IsPayPalOrderfailed
	{
		get { return (bool)(Session[Constants.SESSION_KEY_IS_PAYPAL_ORDER_FAILED] ?? false); }
		set { Session[Constants.SESSION_KEY_IS_PAYPAL_ORDER_FAILED] = value; }
	}
	/// <summary>カート情報</summary>
	public static CartObjectList CartList
	{
		get { return (CartObjectList)Session[Constants.SESSION_KEY_CART_LIST]; }
		set { Session[Constants.SESSION_KEY_CART_LIST] = value; }
	}
	/// <summary>画面遷移正当性チェック用</summary>
	public static string NextPageForCheck
	{
		get { return (string)Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK]; }
		set { Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = value; }
	}
	/// <summary>注文同梱後のカート情報</summary>
	public static CartObjectList OrderCombineCartList
	{
		get { return (CartObjectList)Session[Constants.SESSION_KEY_ORDERCOMBINE_CART_LIST]; }
		set { Session[Constants.SESSION_KEY_ORDERCOMBINE_CART_LIST] = value; }
	}
	/// <summary>注文同梱前のカート情報</summary>
	public static CartObjectList OrderCombineBeforeCartList
	{
		get { return (CartObjectList)Session[Constants.SESSION_KEY_ORDERCOMBINE_BEFORE_CART_LIST]; }
		set { Session[Constants.SESSION_KEY_ORDERCOMBINE_BEFORE_CART_LIST] = value; }
	}
	/// <summary>注文同梱後に決済情報の変更があったか</summary>
	public static bool OrderCombinePaymentReselect
	{
		get { return (bool?)Session[Constants.SESSION_KEY_ORDERCOMBINE_PAYMENT_RESELECT] ?? false; }
		set { Session[Constants.SESSION_KEY_ORDERCOMBINE_PAYMENT_RESELECT] = value; }
	}
	/// <summary>「Amazonアカウントでお支払」ボタンを押して注文同梱選択画面への選択か</summary>
	public static bool OrderCombineFromAmazonPayButton
	{
		get { return (bool?)Session[Constants.SESSION_KEY_ORDERCOMBINE_FROM_AMAZON_PAY_BUTTON] ?? true; }
		set { Session[Constants.SESSION_KEY_ORDERCOMBINE_FROM_AMAZON_PAY_BUTTON] = value; }
	}
	/// <summary>ログインユーザー楽天OpenID</summary>
	public static string LoginUserRakutenOpenId
	{
		get { return (string)Session[Constants.SESSION_KEY_LOGIN_USER_RAKUTEN_OPEN_ID] ?? string.Empty; }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_RAKUTEN_OPEN_ID] = value; }
	}
	/// <summary>ログインユーザー楽天IDConnectで会員登録した？</summary>
	public static bool IsRakutenIdConnectRegisterUser
	{
		get { return (bool?)Session[Constants.SESSION_KEY_LOGIN_USER_RAKUTEN_ID_CONNECT_REGISTER_USER] ?? false; }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_RAKUTEN_ID_CONNECT_REGISTER_USER] = value; }
	}
	/// <summary>ログインユーザー楽天IDConnectログインした？</summary>
	public static bool IsRakutenIdConnectLoggedIn
	{
		get { return (bool?)Session[Constants.SESSION_KEY_LOGIN_USER_RAKUTEN_ID_CONNECT_LOGGEDIN] ?? false; }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_RAKUTEN_ID_CONNECT_LOGGEDIN] = value; }
	}
	/// <summary>楽天IDConnectアクション情報</summary>
	public static RakutenIDConnectActionInfo RakutenIdConnectActionInfo
	{
		get { return (RakutenIDConnectActionInfo)Session[Constants.SESSION_KEY_RAKUTEN_ID_CONNECT_ACTION_INFO]; }
		set { Session[Constants.SESSION_KEY_RAKUTEN_ID_CONNECT_ACTION_INFO] = value; }
	}
	/// <summary>初回の広告コード</summary>
	public static string AdvCodeFirst
	{
		get
		{
			var advCodeFirst = string.Empty;
			if (Constants.W2MP_AFFILIATE_OPTION_ENABLED)
			{
				advCodeFirst = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ADVCODE_FIRST]);
				if (string.IsNullOrEmpty(advCodeFirst))
				{
					advCodeFirst = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ADVCODE_NOW]);
				}
			}
			return advCodeFirst;
		}
		set { Session[Constants.SESSION_KEY_ADVCODE_FIRST] = value; }
	}
	/// <summary>現在の広告コード</summary>
	public static string AdvCodeNow
	{
		get { return Constants.W2MP_AFFILIATE_OPTION_ENABLED ? StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ADVCODE_NOW]) : string.Empty; }
		set { Session[Constants.SESSION_KEY_ADVCODE_NOW] = value; }
	}
	/// <summary>LP注文完了時会員登録用メール情報</summary>
	public static Hashtable MailForUserRegisterWhenOrderComplete
	{
		get { return (Hashtable)Session[Constants.SESSION_KEY_MAIL_FOR_USER_REGISTER_WHEN_ORDER_COMPLETE]; }
		set { Session[Constants.SESSION_KEY_MAIL_FOR_USER_REGISTER_WHEN_ORDER_COMPLETE] = value; }
	}
	/// <summary>Cart list landing page</summary>
	public static CartObjectList CartListLp
	{
		get { return (CartObjectList)Session[Constants.SESSION_KEY_CART_LIST_LANDING]; }
		set { Session[Constants.SESSION_KEY_CART_LIST_LANDING] = value; }
	}
	/// <summary>Is only add cart first time</summary>
	public static bool IsOnlyAddCartFirstTime
	{
		get { return (bool?)Session[Constants.SESSION_KEY_IS_ONLY_ADD_CART_FIRST_TIME] ?? false; }
		set { Session[Constants.SESSION_KEY_IS_ONLY_ADD_CART_FIRST_TIME] = value; }
	}
	/// <summary>Is only add product set to cart landing page first time</summary>
	public static bool IsOnlyAddProductSetCartLpFirstTime
	{
		get { return (bool?)Session[Constants.SESSION_KEY_IS_ONLY_ADD_PRODUCT_SET_CARTLP_FIRST_TIME] ?? true; }
		set { Session[Constants.SESSION_KEY_IS_ONLY_ADD_PRODUCT_SET_CARTLP_FIRST_TIME] = value; }
	}
	/// <summary>Is cart list landing page</summary>
	public static bool IsCartListLp
	{
		get { return (bool?)Session[Constants.SESSION_KEY_IS_CARTLIST_LP] ?? false; }
		set { Session[Constants.SESSION_KEY_IS_CARTLIST_LP] = value; }
	}
	/// <summary>新LPカート確認画面からの遷移か</summary>
	public static bool IsRedirectFromLandingCartConfirm
	{
		get { return (bool?)Session[Constants.SESSION_KEY_IS_REDIRECT_FROM_LANDINGCART_CONFIRM] ?? false; }
		set { Session[Constants.SESSION_KEY_IS_REDIRECT_FROM_LANDINGCART_CONFIRM] = value; }
	}
	/// <summary>ソーシャルログイン情報</summary>
	public static SocialLoginModel SocialLogin
	{
		get { return (SocialLoginModel)Session[Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL]; }
		set { Session[Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL] = value; }
	}
	/// <summary>一時保管ソーシャルログイン情報</summary>
	public static SocialLoginModel TemporaryStoreSocialLogin
	{
		get { return (SocialLoginModel)Session[Constants.SESSION_KEY_TEMPORARY_STORE_SOCIAL_LOGIN_MODEL]; }
		set { Session[Constants.SESSION_KEY_TEMPORARY_STORE_SOCIAL_LOGIN_MODEL] = value; }
	}
	/// <summary>エラー時の遷移先要素のクライアントID</summary>
	public static string LpValidateErrorElementClientId
	{
		get { return (string)Session[Constants.SESSION_KEY_LP_VAILEDATE_ERROR_ELEMENT_CLIENT_ID]; }
		set { Session[Constants.SESSION_KEY_LP_VAILEDATE_ERROR_ELEMENT_CLIENT_ID] = value; }
	}
	/// <summary>LINE直接連携ユーザーID</summary>
	public static string LineProviderUserId
	{
		get { return StringUtility.ToEmpty(Session[Constants.SESSION_KEY_LOGIN_USER_LINE_ID]); }
		set { Session[Constants.SESSION_KEY_LOGIN_USER_LINE_ID] = value; }
	}
	/// <summary>処理後遷移URL</summary>
	public static string NextUrl
	{
		get { return (string)Session[Constants.SESSION_KEY_NEXT_URL]; }
		set { Session[Constants.SESSION_KEY_NEXT_URL] = value; }
	}
	/// <summary>セッション</summary>
	private static HttpSessionState Session
	{
		get { return HttpContext.Current.Session; }
	}
	/// <summary>アマゾン配送先住所</summary>
	public static AmazonAddressModel AmazonShippingAddress
	{
		get { return (AmazonAddressModel)Session["AmazonShippingAddress"] ?? new AmazonAddressModel(); }
		set { Session["AmazonShippingAddress"] = value; }
	}
	/// <summary>AmazonPayアカウントで新規登録するか</summary>
	public static bool IsAmazonPayRegisterForOrder
	{
		get { return (bool?)Session[Constants.SESSION_KEY_IS_AMAZON_PAY_REGISTER_FOR_ORDER] ?? false; }
		set { Session[Constants.SESSION_KEY_IS_AMAZON_PAY_REGISTER_FOR_ORDER] = value; }
	}
	/// <summary>LINE PAYの画面からの遷移か</summary>
	public static bool IsRedirectFromLinePay
	{
		get { return (bool?)Session[Constants.SESSION_KEY_IS_REDIRECT_FROM_LINEPAY] ?? false; }
		set { Session[Constants.SESSION_KEY_IS_REDIRECT_FROM_LINEPAY] = value; }
	}
	/// <summary>Referral code</summary>
	public static string ReferralCode
	{
		get { return StringUtility.ToEmpty(Session[Constants.SESSION_KEY_REFERRAL_CODE]); }
		set { Session[Constants.SESSION_KEY_REFERRAL_CODE] = value; }
	}
	/// <summary>カート種別が変わったか（レコメンドで通常→定期 or 定期→通常）</summary>
	public static bool IsChangedAmazonPayForFixedOrNormal
	{
		get { return (bool?)Session[Constants.SESSION_KEY_IS_CHANGED_AMAZON_PAY_FOR_FIXED_OR_NORMAL] ?? false; }
		set { Session[Constants.SESSION_KEY_IS_CHANGED_AMAZON_PAY_FOR_FIXED_OR_NORMAL] = value; }
	}
	/// <summary>AmazonPayで定期の同意をとっているか</summary>
	public static bool IsAmazonPayGotRecurringConsent
	{
		get { return (bool?)Session[Constants.SESSION_KEY_IS_AMAZON_PAY_GOT_RECURRING_CONSENT] ?? false; }
		set { Session[Constants.SESSION_KEY_IS_AMAZON_PAY_GOT_RECURRING_CONSENT] = value; }
	}
	/// <summary>配送方法調整後の遷移先</summary>
	public static string NextPageAfterAdjustShippingMethod
	{
		get { return StringUtility.ToEmpty(Session[Constants.SESSION_KEY_NEXT_PAGE_AFTER_ADJUST_SHIPPING_METHOD]); }
		set { Session[Constants.SESSION_KEY_NEXT_PAGE_AFTER_ADJUST_SHIPPING_METHOD] = value; }
	}
	/// <summary>頒布会商品付帯情報</summary>
	public static ProductOptionSettingList ProductOptionSettingList
	{
		get { return (ProductOptionSettingList)Session[Constants.SESSION_KEY_PRODUCT_OPTION_SETTING_LIST]; }
		set { Session[Constants.SESSION_KEY_PRODUCT_OPTION_SETTING_LIST] = value; }
	}
	/// <summary>アプリキー（クロスポイント用）</summary>
	public static string AppKeyForCrossPoint
	{
		get { return (string)Session[Constants.SESSION_KEY_CROSSPOINT_APP_KEY]; }
		set { Session[Constants.SESSION_KEY_CROSSPOINT_APP_KEY] = value; }
	}
	/// <summary>会員番号（クロスポイント用）</summary>
	public static string MemberIdForCrossPoint
	{
		get { return (string)Session[Constants.SESSION_KEY_CROSSPOINT_MEMBER_ID]; }
		set { Session[Constants.SESSION_KEY_CROSSPOINT_MEMBER_ID] = value; }
	}
	/// <summary>PINコード（クロスポイント用）</summary>
	public static string PinCodeForCrossPoint
	{
		get { return (string)Session[Constants.SESSION_KEY_CROSSPOINT_PIN_CODE]; }
		set { Session[Constants.SESSION_KEY_CROSSPOINT_PIN_CODE] = value; }
	}
	/// <summary>店舗カードNO/PIN更新フラグ</summary>
	public static bool UpdatedShopCardNoAndPinFlg
	{
		get { return (bool)Session[Constants.SESSION_KEY_CROSS_POINT_UPDATED_SHOP_CARD_NO_AND_PIN_FLG]; }
		set { Session[Constants.SESSION_KEY_CROSS_POINT_UPDATED_SHOP_CARD_NO_AND_PIN_FLG] = value; }
	}
	/// <summary>戻るボタン判定（クロスポイント用）</summary>
	public static bool IsBackForCrossPoint
	{
		get { return (bool)Session[Constants.SESSION_KEY_IS_BACK_CROSS_POINT]; }
		set { Session[Constants.SESSION_KEY_IS_BACK_CROSS_POINT] = value; }
	}
	/// <summary>頒布会商品一覧リスト保持用</summary>
	public static List<SubscriptionBoxDefaultItemModel> SubscriputionBoxProductListForTemp
	{
		get { return (List<SubscriptionBoxDefaultItemModel>)Session[Constants.SESSION_KEY_SUBSCRIPTION_BOX_LIST_FOR_TEMP]; }
		set { Session[Constants.SESSION_KEY_SUBSCRIPTION_BOX_LIST_FOR_TEMP] = value; }
	}
	/// <summary>頒布会更新商品一覧リスト保持用</summary>
	public static List<SubscriptionBoxDefaultItemModel> SubscriputionBoxProductListModifyForTemp
	{
		get { return (List<SubscriptionBoxDefaultItemModel>)Session[Constants.SESSION_KEY_SUBSCRIPTION_BOX_OPTIONAL_LIST_FOR_TEMP]; }
		set { Session[Constants.SESSION_KEY_SUBSCRIPTION_BOX_OPTIONAL_LIST_FOR_TEMP] = value; }
	}
	/// <summary>配送先不可エラーメッセージ</summary>
	public static string UnavailableShippingErrorMessage
	{
		get { return StringUtility.ToEmpty(Session[Constants.SESSION_KEY_UNAVAILABLE_SHIPPING_AREA_ERROR]); }
		set { Session[Constants.SESSION_KEY_UNAVAILABLE_SHIPPING_AREA_ERROR] = value; }
	}
	/// <summary>仮ユーザーIDを保持しているか</summary>
	/// <remarks>LINEミニアプリからアクセスしている場合に保持される</remarks>
	public static bool HasTemporaryUserId
	{
		get { return string.IsNullOrEmpty(TemporaryUserId) == false; }
	}
	/// <summary>仮ユーザーID</summary>
	/// <remarks>LINEミニアプリからの本登録で使用</remarks>
	public static string TemporaryUserId
	{
		get { return StringUtility.ToEmpty(Session[Constants.SESSION_KEY_TEMPORARY_USER_ID]); }
		set { Session[Constants.SESSION_KEY_TEMPORARY_USER_ID] = value; }
	}

	/// <summary>２クリックボタンが押下されたかどうか</summary>
	public static bool IsTwoClickButton
	{
		get { return (bool?)Session[Constants.SESSION_KEY_IS_TWO_CLICK_BUTTON] ?? false; }
		set { Session[Constants.SESSION_KEY_IS_TWO_CLICK_BUTTON] = value; }
	}
	/// <summary>定期情報詳細画面からの遷移か</summary>
	public static bool IsRedirectFromFixedPurchaseDetail
	{
		get { return (bool?)Session[Constants.SESSION_KEY_IS_REDIRECT_FROM_FIXED_PURCHASE_DETAIL] ?? false; }
		set { Session[Constants.SESSION_KEY_IS_REDIRECT_FROM_FIXED_PURCHASE_DETAIL] = value; }
	}
	/// <summary>注文確認画面からの遷移か</summary>
	public static bool IsMovedOnOrderConfirm
	{
		get { return (bool?)Session[Constants.SESSION_KEY_IS_MOVED_ON_ORDER_CONFIRM] ?? false; }
		set { Session[Constants.SESSION_KEY_IS_MOVED_ON_ORDER_CONFIRM] = value; }
	}
}
