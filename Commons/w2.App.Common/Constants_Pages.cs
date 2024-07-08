/*
=========================================================================================================
  Module      : w2Commerce共通定数定義ページ関連部分(Constants_Pages.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;

namespace w2.App.Common
{
	///*********************************************************************************************
	/// <summary>
	/// 定数/設定値を管理する
	/// </summary>
	///*********************************************************************************************
	public partial class Constants : Domain.Constants
	{
		//========================================================================
		// ページ・ＰＣフロントサイド
		//========================================================================
		//------------------------------------------------------
		// システム系
		//------------------------------------------------------
		public const string PAGE_FRONT_VALIDATE_MODULE = "Form/ValidateModule.aspx";
		public const string PAGE_FRONT_VALIDATE_SCRIPT = "Js/ClientValidateScript.aspx";

		//------------------------------------------------------
		// デフォルトページ
		//------------------------------------------------------
		// トップページ
		public const string PAGE_FRONT_DEFAULT = "Default.aspx";
		// ブランドトップページ
		public const string PAGE_FRONT_DEFAULT_BRAND_TOP = "DefaultBrandTop.aspx";

		//------------------------------------------------------
		// 商品系
		//------------------------------------------------------
		// 商品一覧・詳細
		public const string PAGE_FRONT_PRODUCT_LIST = "Form/Product/ProductList.aspx";
		public const string PAGE_FRONT_PRODUCT_DETAIL = "Form/Product/ProductDetail.aspx";
		// 商品サブ画像
		public const string PAGE_FRONT_PRODUCT_DETAIL_SUB_IMAGE = "Form/Product/ProductDetailSubImage.aspx";
		// 商品在庫一覧
		public const string PAGE_FRONT_PRODUCTSTOCK_LIST = "Form/Product/ProductStockList.aspx";
		// 商品セット一覧
		public const string PAGE_FRONT_PRODUCTSET_LIST = "Form/Product/ProductSetList.aspx";
		// 商品バリエーション一覧（商品セール、闇市など）
		public const string PAGE_FRONT_PRODUCTVARIATION_LIST = "Form/Product/ProductVariationList.aspx";
		// 商品グループ
		public const string PAGE_FRONT_PRODUCT_GROUP = "Form/Product/ProductGroupPage.aspx";
		// お気に入り一覧
		public const string PAGE_FRONT_FAVORITE_LIST = "Form/Product/FavoriteList.aspx";
		// リアル店舗商品在庫一覧
		public const string PAGE_FRONT_REALSHOPPRODUCTSTOCK_LIST = "Form/Product/RealShopProductStockList.aspx";
		// AWOO商品一覧画面
		public const string PAGE_RECOMMEND_PRODUCTS_LIST = "Form/Product/RecommendProductsList.aspx";

		/// <summary>Page shop list</summary>
		public const string PAGE_FRONT_SHOP_LIST = "Form/RealShop/ShopList.aspx";
		/// <summary>Page shop detail</summary>
		public const string PAGE_FRONT_SHOP_DETAIL = "Form/RealShop/ShopDetail.aspx";

		//------------------------------------------------------
		// 注文系
		//------------------------------------------------------
		// 通常注文フロー
		public const string PAGE_FRONT_CART_LIST = "Form/Order/CartList.aspx";
		public const string PAGE_FRONT_ORDER_OWNER_DECISION = "Form/Order/OrderOwnerDecision.aspx";
		public const string PAGE_FRONT_CART_SELECT = "Form/Order/CartSelect.aspx";
		public const string PAGE_FRONT_ORDER_SHIPPING = "Form/Order/OrderShipping.aspx";
		public const string PAGE_FRONT_ORDER_SHIPPING_SELECT_SHIPPING = "Form/Order/OrderShippingSelectShipping.aspx";
		public const string PAGE_FRONT_ORDER_SHIPPING_SELECT_PRODUCT = "Form/Order/OrderShippingSelectProduct.aspx";

		/// <summary>注文配送選択画面</summary>
		public const string PAGE_FRONT_ORDER_SHIPPING_SELECT = "Form/Order/OrderShippingSelect.aspx";

		public const string PAGE_FRONT_ORDER_COMBINE_SELECT_LIST = "Form/Order/OrderCombineSelectList.aspx";
		public const string PAGE_FRONT_ORDER_PAYMENT = "Form/Order/OrderPayment.aspx";
		public const string PAGE_FRONT_ORDER_CONFIRM = "Form/Order/OrderConfirm.aspx";
		public const string PAGE_FRONT_ORDER_SETTLEMENT = "Form/Order/OrderSettlement.aspx";
		public const string PAGE_FRONT_ORDER_COMPLETE = "Form/Order/OrderComplete.aspx";
		// 注文履歴
		public const string PAGE_FRONT_ORDER_HISTORY_LIST = "Form/OrderHistory/OrderHistoryList.aspx";
		public const string PAGE_FRONT_ORDER_HISTORY_DETAIL = "Form/OrderHistory/OrderHistoryDetail.aspx";
		public const string PAGE_FRONT_ORDER_HISTORY_AMAZONPAY_CV2_CHANGE_ACTION_REDIRECT = "Form/OrderHistory/AmazonPayCv2/AmazonPayCv2ChangeActionRedirect.aspx";

		/// <summary>Store order history list</summary>
		public const string PAGE_FRONT_STORE_ORDER_HISTORY_LIST = "Form/StoreOrderHistory/StoreOrderHistoryList.aspx";
		/// <summary>Store order history detail</summary>
		public const string PAGE_FRONT_STORE_ORDER_HISTORY_DETAIL = "Form/StoreOrderHistory/StoreOrderHistoryDetail.aspx";

		// 定期購入
		public const string PAGE_FRONT_FIXED_PURCHASE_LIST = "Form/FixedPurchase/FixedPurchaseList.aspx";
		public const string PAGE_FRONT_FIXED_PURCHASE_DETAIL = "Form/FixedPurchase/FixedPurchaseDetail.aspx";
		public const string PAGE_FRONT_SUBSCRIPTIONBOX_PRODUCT_CHANGE_MODAL = "Form/FixedPurchase/SubscriptionBoxProductChangeModal.aspx";
		public const string PAGE_FRONT_FIXED_PURCHASE_PRODUCT_ADD = "Form/FixedPurchase/FixedPurchaseProductAdd.aspx";
		// 定期解約
		public const string PAGE_FRONT_FIXED_PURCHASE_CANCEL_INPUT = "Form/FixedPurchase/FixedPurchaseCancelInput.aspx";
		public const string PAGE_FRONT_FIXED_PURCHASE_CANCEL_CONFIRM = "Form/FixedPurchase/FixedPurchaseCancelConfirm.aspx";
		public const string PAGE_FRONT_FIXED_PURCHASE_CANCEL_COMPLETE = "Form/FixedPurchase/FixedPurchaseCancelComplete.aspx";
		// ゼウス3Dセキュア認証
		public const string PAGE_FRONT_PAYMENT_ZEUS_CARD_3DSECURE_POST = "Payment/Card3DSecureAuthZeus/PostCard3DSecureAuth.aspx";
		public const string PAGE_FRONT_PAYMENT_ZEUS_CARD_3DSECURE_GET_RESULT = "Payment/Card3DSecureAuthZeus/GetCard3DSecureAuthResult.aspx";
		// 楽天3Dセキュア認証
		public const string PAGE_FRONT_PAYMENT_RAKUTEN_POST_3DS_AUTH = "Payment/Rakuten/Post3DSAuth.aspx";
		public const string PAGE_FRONT_PAYMENT_RAKUTEN_3DS_RESULT = "Payment/Rakuten/3DSResult.aspx";
		// ゼウスLinkPoint
		public const string PAGE_FRONT_PAYMENT_ZEUS_LINK_POINT_POST = "Payment/ZeusLinkPoint/PaymentZeusLinkPointAuth.aspx";
		public const string PAGE_FRONT_PAYMENT_ZEUS_LINK_POINT_GET_NOTICE = "Payment/ZeusLinkPoint/PaymentZeusLinkPointAuthNotice.aspx";
		// SBPS 決済
		public const string PAGE_FRONT_PAYMENT_SBPS_MULTIPAYMENT_POST_ORDER = "Payment/SBPS/PaymentSBPSMultiPaymentExecOrder.aspx";
		public const string PAGE_FRONT_PAYMENT_SBPS_MULTIPAYMENT_ORDER_NOTICE = "Payment/SBPS/PaymentSBPSMultiPaymentReceiveOrderNotice.aspx";
		public const string PAGE_FRONT_PAYMENT_SBPS_MULTIPAYMENT_ORDER_ERROR = "Payment/SBPS/PaymentSBPSMultiPaymentReceiveOrderError.aspx";
		public const string PAGE_FRONT_PAYMENT_SBPS_MULTIPAYMENT_ORDER_CANCEL = "Payment/SBPS/PaymentSBPSMultiPaymentReceiveOrderCancel.aspx";
		public const string PAGE_FRONT_PAYMENT_SBPS_MULTIPAYMENT_ORDER_RESULT = "Payment/SBPS/PaymentSBPSMultiPaymentReceiveOrderResult.aspx";

		#region ヤマト3Dセキュア認証
		/// <summary>ヤマトKWC 3Dセキュア認証データ送信ページ処理画面</summary>
		public const string PAGE_FRONT_PAYMENT_YAMATO_POST_3DS_AUTH = "Payment/YamatoKwc/PostCard3DSecureAuth.aspx";
		/// <summary>KWC3Dセキュア認証結果取得ページ処理画面</summary>
		public const string PAGE_FRONT_PAYMENT_YAMATO_3DS_RESULT = "Payment/YamatoKwc/GetCard3DSecureAuthResult.aspx";
		#endregion

		#region ペイジェント3Dセキュア認証
		/// <summary> ペイジェント3Dセキュア認証データ送信ページ</summary>
		public const string PAGE_FRONT_PAYMENT_PAYGENT_POST_3DS_AUTH = "Payment/Card3DSecureAuthPaygent/PostCard3DSecureAuth.aspx";
		/// <summary> ペイジェント3Dセキュア認証結果取得ページ</summary>
		public const string PAGE_FRONT_PAYMENT_PAYGENT_POST_3DS_RESULT = "Payment/Paygent/GetCard3DSecureAuthResult.aspx";
		#endregion

		/// <summary>Amazon Pay Cv2コールバックページ</summary>
		public const string PAGE_FRONT_PAYMENT_AMAZONPAY_CV2_COMPLETE = "Payment/AmazonPayCv2/AmazonPayComplete.aspx";

		// Atone payment authories
		public const string PAGE_FRONT_PAYMENT_ATONE_AUTHORIES = "Payment/Atone/AtoneAuthories.aspx";
		public const string PAGE_FRONT_PAYMENT_ATONE_AUTHORIES_SMART_PHONE = "Payment/Atone/AtoneAuthoriesSmartPhone.aspx";
		// Aftee payment authories
		public const string PAGE_FRONT_PAYMENT_AFTEE_AUTHORIES = "Payment/Aftee/AfteeAuthories.aspx";
		public const string PAGE_FRONT_PAYMENT_AFTEE_AUTHORIES_SMART_PHONE = "Payment/Aftee/AfteeAuthoriesSmartPhone.aspx";

		// EcPay
		public const string PAGE_FRONT_PAYMENT_ECPAY_ORDER_RESULT = "Form/Order/EcPay/OrderResult.aspx";
		public const string PAGE_FRONT_PAYMENT_ECPAY_RECEIVE = "Payment/EcPay/Receive.ashx";
		public const string PAGE_FRONT_PAYMENT_ECPAY_BARCODE = "Form/Order/EcPay/Barcode.aspx";

		/// <summary>GMO決済：GMO3Dセキュア認証データ送信ページ処理</summary>
		public const string PAGE_FRONT_PAYMENT_GMO_CARD_3DSECURE_POST = "Payment/GMO/PostCard3DSecureAuth.aspx";
		/// <summary>GMO決済：GMO3Dセキュア認証結果取得ページ</summary>
		public const string PAGE_FRONT_PAYMENT_GMO_CARD_3DSECURE_GET_RESULT = "Payment/GMO/GetCard3DSecureAuthResult.aspx";

		// ランディングカートページ
		public const string PAGE_FRONT_LANDING_LANDING_CART_INPUT = "Landing/LandingCartInput.aspx";
		public const string PAGE_FRONT_LANDING_LANDING_CART_CONFIRM = "Landing/LandingCartConfirm.aspx";
		// Amazon系
		public const string PAGE_FRONT_ORDER_AMAZON_PAYMENT_INPUT = "Form/Order/OrderAmazonInput.aspx";
		public const string PAGE_FRONT_AMAZON_LOGIN_CALLBACK = "Form/User/Amazon/LoginCallback.aspx";
		public const string PAGE_FRONT_AMAZON_COOPERATION_CALLBACK = "Form/User/Amazon/CooperationCallback.aspx";
		public const string PAGE_FRONT_AMAZON_ORDER_CALLBACK = "Form/User/Amazon/OrderCallback.aspx";
		public const string PAGE_FRONT_AMAZON_USER_EASY_REGISTER_CALLBACK = "Form/User/Amazon/UserEasyRegisterCallback.aspx";
		public const string PAGE_FRONT_AMAZON_LANDING_PAGE_CALLBACK = "Form/User/Amazon/LandingPageCallback.aspx";
		public const string PAGE_FRONT_AMAZON_AMAZON_PAY_WIDGET_CALLBACK = "Form/User/Amazon/AmazonPayWidgetCallback.aspx";
		public const string PAGE_FRONT_COMMON_AMAZON_PAY_INPUT_WIDGET = "Form/Common/AmazonPayInputWidget.aspx";
		public const string PAGE_FRONT_COMMON_AMAZON_PAY_DETAIL_WIDGET = "Form/Common/AmazonPayDetailWidget.aspx";
		// LINE直接連携
		public const string PAGE_FRONT_LINE_LOGIN_CALLBACK = "Form/User/Line/LoginCallback.aspx";
		/// <summary>LP用Atoneトークン取得ページ</summary>
		public const string PAGE_FROMT_PAYMENT_ATONE_ATONE_EXEC_ORDER_FOR_LANDINGCART = "Payment/Atone/AtoneExecOrderForLandingCart.aspx";
		/// <summary>LP用Afteeトークン取得ページ</summary>
		public const string PAGE_FROMT_PAYMENT_AFTEE_AFTEE_EXEC_ORDER_FOR_LANDINGCART = "Payment/Aftee/AfteeExecOrderForLandingCart.aspx";

		// GMOアトカラ
		public const string PAGE_FRONT_PAYMENT_GMO_ATOKARA_POST = "Payment/GmoAtokara/PostGmoAtokaraAuth.aspx";
		public const string PAGE_FRONT_PAYMENT_GMO_ATOKARA_RESULT = "Payment/GmoAtokara/GmoAtokaraReceiveOrderResult.aspx";

		//------------------------------------------------------
		// ユーザ系
		//------------------------------------------------------
		// ログイン・ログオフ
		public const string PAGE_FRONT_LOGIN = "Form/Login.aspx";
		public const string PAGE_FRONT_LOGOFF = "Form/Logoff.aspx";
		// セッション復元
		public const string PAGE_FRONT_RESTORE_SESSION = "Form/RestoreSession.aspx";
		// パスワードリマインダ画面
		public const string PAGE_FRONT_PASSWORD_REMINDER_INPUT = "Form/User/PasswordReminderInput.aspx";
		public const string PAGE_FRONT_PASSWORD_REMINDER_COMPLETE = "Form/User/PasswordReminderComplete.aspx";
		// パスワード変更画面
		public const string PAGE_FRONT_PASSWORD_MODIFY_INPUT = "Form/User/PasswordModifyInput.aspx";
		public const string PAGE_FRONT_PASSWORD_MODIFY_COMPLETE = "Form/User/PasswordModifyComplete.aspx";
		// マイページ
		public const string PAGE_FRONT_MYPAGE = "Form/User/MyPage.aspx";
		// ユーザー新規登録画面
		public const string PAGE_FRONT_USER_REGIST_REGULATION = "Form/User/UserRegistRegulation.aspx";
		public const string PAGE_FRONT_USER_REGIST_INPUT = "Form/User/UserRegistInput.aspx";
		public const string PAGE_FRONT_USER_REGIST_CONFIRM = "Form/User/UserRegistConfirm.aspx";
		public const string PAGE_FRONT_USER_REGIST_COMPLETE = "Form/User/UserRegistComplete.aspx";
		// かんたん会員登録画面
		public const string PAGE_FRONT_USER_EASY_REGIST_INPUT = "Form/User/UserEasyRegistInput.aspx";
		// ユーザー顧客情報変更画面
		public const string PAGE_FRONT_USER_MODIFY_INPUT = "Form/User/UserModifyInput.aspx";
		public const string PAGE_FRONT_USER_MODIFY_CONFIRM = "Form/User/UserModifyConfirm.aspx";
		public const string PAGE_FRONT_USER_MODIFY_COMPLETE = "Form/User/UserModifyComplete.aspx";
		// ユーザー退会画面
		public const string PAGE_FRONT_USER_WITHDRAWAL_INPUT = "Form/User/UserWithdrawalInput.aspx";
		public const string PAGE_FRONT_USER_WITHDRAWAL_COMPLETE = "Form/User/UserWithdrawalComplete.aspx";
		// ユーザーアドレス帳画面
		public const string PAGE_FRONT_USER_SHIPPING_LIST = "Form/User/UserShippingList.aspx";
		public const string PAGE_FRONT_USER_SHIPPING_INPUT = "Form/User/UserShippingInput.aspx";
		public const string PAGE_FRONT_USER_SHIPPING_CONFIRM = "Form/User/UserShippingConfirm.aspx";
		// メールマガジン解除画面
		public const string PAGE_FRONT_MAILMAGAZINE_CANCEL_INPUT = "Form/User/MailMagazineCancelInput.aspx";
		public const string PAGE_FRONT_MAILMAGAZINE_CANCEL_COMPLETE = "Form/User/MailMagazineCancelComplete.aspx";
		// ユーザークレジットカード画面
		public const string PAGE_FRONT_USER_CREDITCARD_LIST = "Form/User/UserCreditCardList.aspx";
		public const string PAGE_FRONT_USER_CREDITCARD_INPUT = "Form/User/UserCreditCardInput.aspx";
		public const string PAGE_FRONT_USER_CREDITCARD_CONFIRM = "Form/User/UserCreditCardConfirm.aspx";
		// 受信メール履歴画面
		public const string PAGE_FRONT_USER_RECIEVE_MAIL_LIST = "Form/User/UserRecieveMailList.aspx";
		// 受信メール詳細画面
		public const string PAGE_FRONT_USER_RECIEVE_MAIL_DETAIL = "Form/User/UserRecieveMailDetail.aspx";
		// Htmlメール表示画面
		public const string PAGE_FRONT_USER_MAIL = "Form/User/HtmlMail.aspx";
		// 入荷通知メール一覧画面
		public const string PAGE_FRONT_USER_PRODUCT_ARRIVAL_MAIL_LIST = "Form/User/UserProductArrivalMailList.aspx";
		// 入荷通知メール登録ポップアップ画面
		public const string PAGE_FRONT_USER_PRODUCT_ARRIVAL_MAIL_REGIST = "Form/User/ProductArrivalMailRegist.aspx";
		// メールマガジン登録画面
		public const string PAGE_FRONT_MAILMAGAZINE_REGIST_INPUT = "Form/User/MailMagazineRegistInput.aspx";
		public const string PAGE_FRONT_MAILMAGAZINE_REGIST_CONFIRM = "Form/User/MailMagazineRegistConfirm.aspx";
		public const string PAGE_FRONT_MAILMAGAZINE_REGIST_COMPLETE = "Form/User/MailMagazineRegistComplete.aspx";
		// シリアルキー認証画面
		public const string PAGE_FRONT_SERIAL_KEY_AUTH_INPUT = "Form/User/SerialKeyAuthInput.aspx";
		public const string PAGE_FRONT_SERIAL_KEY_AUTH_COMPLETE = "Form/User/SerialKeyAuthComplete.aspx";
		// ダウンロードポップアップ画面
		public const string PAGE_FRONT_DOWNLOAD_LINK = "Form/User/DownloadLink.aspx";
		// マイページポイント履歴一覧
		public const string PAGE_FRONT_USERPOINTHISTORY_LIST = "Form/User/UserPointHistoryList.aspx";
		// マイページクーポンBOX
		public const string PAGE_FRONT_COUPON_BOX = "Form/User/UserCouponBox.aspx";
		// ソーシャルログイン
		public const string PAGE_FRONT_SOCIAL_LOGIN_LOGIN_CALLBACK = "Form/User/SocialLogin/LoginCallback.aspx";
		public const string PAGE_FRONT_SOCIAL_LOGIN_ORDER_CALLBACK = "Form/User/SocialLogin/OrderCallback.aspx";
		public const string PAGE_FRONT_SOCIAL_LOGIN_COOPERATION_CALLBACK = "Form/User/SocialLogin/CooperationCallback.aspx";
		public const string PAGE_FRONT_SOCIAL_LOGIN_USER_EASY_REGISTER_CALLBACK = "Form/User/SocialLogin/UserEasyRegisterCallback.aspx";
		public const string PAGE_FRONT_SOCIAL_LOGIN_USER_REGIST_INPUT_CALLBACK = "Form/User/SocialLogin/UserRegistInputCallback.aspx";
		public const string PAGE_FRONT_SOCIAL_LOGIN_COOPERATION = "Form/User/SocialLoginCooperation.aspx";
		public const string PAGE_FRONT_SOCIAL_LOGIN_LANDINGCART_CALLBACK = "Form/User/SocialLogin/LandingCartCallback.aspx";
		// マイページ注文方法の保存
		public const string PAGE_FRONT_USER_DEFAULT_ORDER_SETTING_LIST = "Form/User/UserDefaultOrderSettingList.aspx";
		public const string PAGE_FRONT_USER_DEFAULT_ORDER_SETTING_INPUT = "Form/User/UserDefaultOrderSettingInput.aspx";
		public const string PAGE_FRONT_USER_DEFAULT_ORDER_SETTING_CONFIRM = "Form/User/UserDefaultOrderSettingConfirm.aspx";
		public const string PAGE_FRONT_USER_FRAME_GUARANTEE = "Form/User/FrameGuarantee.aspx";
		// 楽天IDConnect連携画面
		public const string PAGE_FRONT_AUTH_RAKUTEN_ID_CONNECT = "Auth/AuthRakutenIDConnect.aspx";
		// シングルサインオン連携画面
		public const string PAGE_FRONT_SINGLE_SIGN_ON = "Form/SingleSignOn.aspx";
		// マイページ：ユーザーアクティビティリスト
		public const string PAGE_FRONT_FOLLOW_LIST = "Form/User/FollowList.aspx";
		public const string PAGE_FRONT_LIKE_LIST = "Form/User/LikeList.aspx";

		//------------------------------------------------------
		// コンテンツ系
		//------------------------------------------------------
		public const string PAGE_FRONT_NEWS_LIST = "Form/NewsList.aspx";
		public const string PAGE_FRONT_INQUIRY_INPUT = "Form/Inquiry/InquiryInput.aspx";
		public const string PAGE_FRONT_INQUIRY_CONFIRM = "Form/Inquiry/InquiryConfirm.aspx";
		public const string PAGE_FRONT_INQUIRY_COMPLETE = "Form/Inquiry/InquiryComplete.aspx";

		//------------------------------------------------------
		// CMS系
		//------------------------------------------------------
		public const string PAGE_FRONT_CUSTOMPAGE_TEMPLATE = "Form/PageTemplates/CustomPageTemplate.aspx";
		public const string PAGE_FRONT_CUSTOMPARTS_TEMPLATE = "Form/PageTemplates/CustomPartsTemplate.ascx";
		public const string PAGE_FRONT_PARTS_PREVIEW = "Form/PageTemplates/PartsPreview.aspx";
		public const string PAGE_FRONT_ADVANCEDSEARCHBOX = "Form/Common/Product/BodyProductAdvancedSearchBox.ascx";
		public const string PAGE_FRONT_FEATUREPAGE_FEATUREPAGELIST = "Form/FeaturePage/FeaturePageList.aspx";
		public const string PAGE_FRONT_FEATUREPAGE_PARTS_BODYFEATUREPAGELIST = "Form/Common/FeaturePage/BodyFeaturePageList.ascx";
		public const string PAGE_FRONT_FEATUREPAGE_TEMPLATE = "Form/PageTemplates/FeaturePageTemplate.aspx";
		public const string PAGE_FRONT_AB_TEST = "Form/AbTest/AbTestAllotoment.aspx";

		// LINEミニアプリ
		/// <summary>LINEミニアプリ：トップページ</summary>
		public const string PAGE_LINE_MINIAPP_TOP = "MiniApp/Top.aspx";
		/// <summary>LINEミニアプリ：シングルサインオン画面</summary>
		public const string PAGE_LINE_MINIAPP_SINGLE_SIGN_ON = "MiniApp/SingleSignOn.aspx";
		/// <summary>LINEミニアプリ：会員証画面</summary>
		public const string PAGE_LINE_MINIAPP_MEMBER_CARD = "MiniApp/Form/User/MemberCard.aspx";
		/// <summary>LINEミニアプリ：ログイン・アカウント連携画面</summary>
		public const string PAGE_LINE_MINIAPP_LOGIN_COOPERATION = "MiniApp/Form/User/LoginCooperation.aspx";
		/// <summary>LINEミニアプリ：JSログ出力ハンドラー</summary>
		public const string HANDLER_LINE_MINIAPP_LOG_EXPORTER = "MiniApp/Handler/LineMiniAppLogExporter.ashx";

		// ページ判定用
		/// <summary> デフォルトページ </summary>
		public const string PAGE_TYPE_FRONT_DEFAULT = "default_aspx";
		/// <summary> ブランドトップページ </summary>
		public const string PAGE_TYPE_FRONT_DEFAULT_BRAND_TOP = "defaultbrandtop_aspx";
		/// <summary> 商品一覧ページ </summary>
		public const string PAGE_TYPE_FRONT_PRODUCT_LIST = "form_product_productlist_aspx";
		/// <summary> 商品一覧プレビューページ </summary>
		public const string PAGE_TYPE_FRONT_PRODUCT_LIST_PREVIEW = "page_preview_productlist_aspx_preview_aspx";
		/// <summary> 商品詳細ページ </summary>
		public const string PAGE_TYPE_FRONT_PREVIEW_PRODUCT_DETAIL = "form_product_productdetail_aspx";
		/// <summary> 商品詳細プレビューページ </summary>
		public const string PAGE_TYPE_FRONT_PREVIEW_PRODUCT_DETAIL_PREVIEW = "page_preview_productlist_aspx_preview_aspx";
		/// <summary> 商品在庫一覧ページ </summary>
		public const string PAGE_TYPE_FRONT_PRODUCTSTOCK_LIST = "form_product_realshopproductstocklist_aspx";
		/// <summary> 商品在庫一覧プレビューページ </summary>
		public const string PAGE_TYPE_FRONT_PRODUCTSTOCK_LIST_PREVIEW = "page_preview_realshopproductstocklist_preview_aspx";
		/// <summary> 商品セット一覧ページ </summary>
		public const string PAGE_TYPE_FRONT_PRODUCTSET_LIST = "form_product_productsetlist_aspx";
		/// <summary> 商品セット一覧プレビューページ </summary>
		public const string PAGE_TYPE_FRONT_PRODUCTSET_LIST_PREVIEW = "page_preview_productsetlist_preview_aspx";
		/// <summary> 商品バリエーション一覧（商品セール、闇市など）ページ </summary>
		public const string PAGE_TYPE_FRONT_PRODUCTVARIATION_LIST = "form_product_productvariationlist_aspx";
		/// <summary> 商品バリエーション一覧（商品セール、闇市など）プレビューページ </summary>
		public const string PAGE_TYPE_FRONT_PRODUCTVARIATION_LIST_PREVIEW = "page_preview_productvariationlist_preview_aspx";
		/// <summary> リアル店舗商品在庫一覧ページ </summary>
		public const string PAGE_TYPE_FRONT_REALSHOPPRODUCTSTOCK_LIST = "form_product_realshopproductstocklist_aspx";
		/// <summary> リアル店舗商品在庫一覧ページ </summary>
		public const string PAGE_TYPE_FRONT_REALSHOPPRODUCTSTOCK_LIST_PREVIEW = "page_preview_realshopproductstocklist_preview_aspx";
		/// <summary> 注文配送先入力ページ </summary>
		public const string PAGE_TYPE_FRONT_ORDER_SHIPPING = "form_order_ordershipping_aspx";
		/// <summary> 注文配送先入力プレビューページ </summary>
		public const string PAGE_TYPE_FRONT_ORDER_SHIPPING_PREVIEW = "page_preview_ordershipping_aspx_preview_aspx";
		/// <summary> 注文配送先配送先選択ページ </summary>
		public const string PAGE_TYPE_FRONT_ORDER_SHIPPING_SELECT_SHIPPING = "form_order_ordershippingselectshipping_aspx";
		/// <summary> 注文配送先配送先選択プレビューページ </summary>
		public const string PAGE_TYPE_FRONT_ORDER_SHIPPING_SELECT_SHIPPING_PREVIEW = "page_preview_ordershippingselectshipping_aspx_preview_aspx";
		/// <summary> 注文配送先商品選択ページ </summary>
		public const string PAGE_TYPE_FRONT_ORDER_SHIPPING_SELECT_PRODUCT = "form_order_ordershippingselectproduct_aspx";
		/// <summary> 注文配送先商品選択プレビューページ </summary>
		public const string PAGE_TYPE_FRONT_ORDER_SHIPPING_SELECT_PRODUCT_PREVIEW = "page_preview_ordershippingselectproduct_aspx_preview_aspx";
		/// <summary> 注文お支払い方法選択ページ </summary>
		public const string PAGE_TYPE_FRONT_ORDER_PAYMENT = "form_order_orderpayment_aspx";
		/// <summary> 注文お支払い方法選択プレビューページ </summary>
		public const string PAGE_TYPE_FRONT_ORDER_PAYMENT_PREVIEW = "page_preview_orderpayment_aspx_preview_aspx";
		/// <summary> 注文確認ページ </summary>
		public const string PAGE_TYPE_FRONT_ORDER_CONFIRM = "form_order_orderconfirm_aspx";
		/// <summary> 注文確認プレビューページ </summary>
		public const string PAGE_TYPE_FRONT_ORDER_CONFIRM_PREVIEW = "page_preview_orderconfirm_aspx_preview_aspx";
		/// <summary> 注文決済ページ </summary>
		public const string PAGE_TYPE_FRONT_ORDER_SETTLEMENT = "form_order_ordersettlement_aspx";
		/// <summary> 注文決済プレビューページ </summary>
		public const string PAGE_TYPE_FRONT_ORDER_SETTLEMENT_PREVIEW = "page_preview_ordersettlement_aspx_preview_aspx";
		/// <summary> 注文完了ページ </summary>
		public const string PAGE_TYPE_FRONT_ORDER_COMPLETE = "form_order_ordercomplete_aspx";
		/// <summary> 注文完了プレビューページ </summary>
		public const string PAGE_TYPE_FRONT_ORDER_COMPLETE_PREVIEW = "page_preview_ordercomplete_aspx_preview_aspx";
		/// <summary> カートページ </summary>
		public const string PAGE_TYPE_FRONT_LANDING_LANDING_CART_INPUT = "landing_landingcartinput_aspx";
		/// <summary> カート注文確認ページ </summary>
		public const string PAGE_TYPE_FRONT_LANDING_LANDING_CART_CONFIRM = "landing_landingcartconfirm_aspx";
		/// <summary> カートプレビューページ </summary>
		public const string PAGE_TYPE_FRONT_LP_PREVIEW = "landing_formlp_lppreview_aspx";
		/// <summary> Amazon支払い画面 </summary>
		public const string PAGE_TYPE_FRONT_AMAZON_PAYMENT_INPUT = "form_order_orderamazoninput_aspx";
		/// <summary>注文配送選択ページ</summary>
		public const string PAGE_TYPE_FRONT_ORDER_SHIPPING_SELECT = "form_order_ordershippingselect_aspx";
		/// <summary>注文配送選択プレビューページ</summary>
		public const string PAGE_TYPE_FRONT_ORDER_SHIPPING_SELECT_PREVIEW = "page_preview_ordershippingselect_aspx_preview_aspx";
		/// <summary>LPカート用Atone与信取得ページ</summary>
		public const string PAGE_TYPE_FRONT_PAYMENT_ATONE_ATONE_EXEC_ORDER_FOR_LANDINGCART = "payment_atone_atoneexecorderforlandingcart_aspx";
		/// <summary>LPカート用Aftee与信取得ページ</summary>
		public const string PAGE_TYPE_FRONT_PAYMENT_AFTEE_AFTEE_EXEC_ORDER_FOR_LANDINGCART = "payment_aftee_afteeexecorderforlandingcart_aspx";

		// マスターファイル系
		/// <summary>デフォルトページマスタ</summary>
		public const string PAGE_FRONT_DEFAULT_MASTER = "Form/Common/DefaultPage.master";
		/// <summary>デフォルトページマスタ(ヘッダ・フッタなし)</summary>
		public const string PAGE_FRONT_SIMPLE_DEFAULT_MASTER = "Form/Common/SimpleDefaultPage.master";
		/// <summary>デフォルトページプレビューマスタ</summary>
		public const string PAGE_FRONT_DEFAULT_PREVIEW_MASTER = "Form/Common/DefaultPagePreview.master";
		/// <summary>デフォルトページマスタ(ヘッダ・フッタなし)</summary>
		public const string PAGE_FRONT_SIMPLE_DEFAULT_PREVIEW_MASTER = "Form/Common/SimpleDefaultPagePreview.master";
		/// <summary>注文ページプレビューマスタ</summary>
		public const string PAGE_FRONT_ORDER_PREVIEW_MASTER = "Form/Common/OrderPagePreview.master";
		/// <summary>LP注文ページプレビューマスタ</summary>
		public const string PAGE_FRONT_LANDING_ORDER_PREVIEW_MASTER = "Form/Common/LandingOrderPagePreview.master";
		/// <summary>デフォルトページプレビューマスタ</summary>
		public static string PAGE_FRONT_DEFAULT_PREVIEW = PAGE_FRONT_DEFAULT_PREVIEW_MASTER;
		/// <summary>デフォルトページマスタ(ヘッダ・フッタなし)</summary>
		public static string PAGE_FRONT_SIMPLE_DEFAULT_PREVIEW = PAGE_FRONT_SIMPLE_DEFAULT_PREVIEW_MASTER;
		/// <summary>デフォルトページプレビューマスタ</summary>
		public static string PAGE_FRONT_ORDER_PREVIEW = PAGE_FRONT_ORDER_PREVIEW_MASTER;

		// コーディネート系
		public const string PAGE_FRONT_COORDINATE_DETAIL = "Form/Coordinate/CoordinateDetail.aspx";
		public const string PAGE_FRONT_COORDINATE_LIST = "Form/Coordinate/CoordinateList.aspx";
		public const string PAGE_FRONT_COORDINATE_TOP = "Form/Coordinate/CoordinateTop.aspx";

		// ユーザー電子発票管理画面
		public const string PAGE_FRONT_USER_INVOICE_LIST = "Form/User/UserInvoiceList.aspx";
		public const string PAGE_FRONT_USER_INVOICE_INPUT = "Form/User/UserInvoiceInput.aspx";
		public const string PAGE_FRONT_USER_INVOICE_CONFIRM = "Form/User/UserInvoiceConfirm.aspx";

		/// <summary>Scoring sale top page</summary>
		public const string PAGE_FRONT_SCORINGSALE_TOP_PAGE = "Form/ScoringSale/" + PAGE_FRONT_SCORINGSALE_TOP_PAGE_NAME;
		/// <summary>Scoring sale question page</summary>
		public const string PAGE_FRONT_SCORINGSALE_QUESTION_PAGE = "Form/ScoringSale/" + PAGE_FRONT_SCORINGSALE_QUESTION_PAGE_NAME;
		/// <summary>Scoring sale result page</summary>
		public const string PAGE_FRONT_SCORINGSALE_RESULT_PAGE = "Form/ScoringSale/" + PAGE_FRONT_SCORINGSALE_RESULT_PAGE_NAME;
		/// <summary>Front scoring sale top page name</summary>
		public const string PAGE_FRONT_SCORINGSALE_TOP_PAGE_NAME = "Top.aspx";
		/// <summary>Front scoring sale question page</summary>
		public const string PAGE_FRONT_SCORINGSALE_QUESTION_PAGE_NAME = "Question.aspx";
		/// <summary>Front scoring sale result page</summary>
		public const string PAGE_FRONT_SCORINGSALE_RESULT_PAGE_NAME = "ResultPage.aspx";

		/// <summary>頒布会詳細ページ</summary>
		public const string PAGE_FRONT_SUBSCRIPTIONBOX_DETAIL = "Form/SubscriptionBox/SubscriptionBoxDetail.aspx";

		/// <summary>頒布会初回選択画面</summary>
		public const string PAGE_FRONT_SUBSCRIPTIONBOX_FIRSTSELECTABLE = "Form/SubscriptionBox/SubscriptionBoxFirstSelection.aspx";

		//------------------------------------------------------
		// その他系
		//------------------------------------------------------
		public const string PAGE_FRONT_ERROR = "Form/Error.aspx";
		public const string PAGE_FRONT_BLANK = "Form/Blank.aspx";
		public const string PAGE_FRONT_CLICKPOINT = "Form/ClickPoint.aspx";
		public const string PAGE_FRONT_RECEIPTDOWNLOAD = "Form/Receipt/ReceiptDownload.aspx";

		//------------------------------------------------------
		// 外部レコメンド連携系
		//------------------------------------------------------
		/// <summary>トラッキングログ送信ページ</summary>
		public const string PAGE_FRONT_RECOMMEND_SEND_TRACKING_LOG = "Form/Recommend/SendTrackingLog.aspx";
		/// <summary>外部レコメンド連携商品レコメンドリスト画面</summary>
		public const string PAGE_FRONT_COMMON_BODY_PRODUCT_RECOMMEND_BY_RECOMMEND_ENGINE = "Form/Common/Product/BodyProductRecommendByRecommendEngine.aspx";
		/// <summary>外部レコメンド連携カテゴリレコメンドリスト画面</summary>
		public const string PAGE_FRONT_COMMON_BODY_CATEGORY_RECOMMEND_BY_RECOMMEND_ENGINE = "Form/Common/Product/BodyCategoryRecommendByRecommendEngine.aspx";

		/// <summary>Page front cart list landing page</summary>
		public const string PAGE_FRONT_CART_LIST_LP = "Landing/Formlp/CartListLp.aspx";

		/// <summary>PayPay SBPS receive page</summary>
		public const string PAGE_FRONT_PAYMENT_PAYPAY_SBPS_RECEIVE = "Payment/SBPS/PaymentSBPSPayPayReceive.aspx";

		/// <summary>PayPay receive page</summary>
		public const string PAGE_FRONT_PAYMENT_PAYPAY_RECEIVE = "Payment/GMO/PayPayGMOReceive.aspx";

		/// <summary>Page front friend referral page</summary>
		public const string PAGE_FRONT_FRIEND_REFERRAL = "Form/FriendReferral/FriendReferral.aspx";

		//========================================================================
		// ページ・モバイルフロントサイド
		//========================================================================
		public const string PAGE_MFRONT_TOP = "Form/Top/Top.aspx";

		public const string PAGE_MFRONT_LOGIN_INPUT = "Form/User/LoginInput.aspx";
		public const string PAGE_MFRONT_LOGIN_COMPLETE = "Form/User/LoginComplete.aspx";
		public const string PAGE_MFRONT_REMINDER_INPUT = "Form/User/ReminderInput.aspx";
		public const string PAGE_MFRONT_REMINDER_COMPLETE = "Form/User/ReminderComplete.aspx";
		public const string PAGE_MFRONT_PASSWORDMODIFY_INPUT = "Form/User/PasswordModifyInput.aspx";
		public const string PAGE_MFRONT_PASSWORDMODIFY_COMPLETE = "Form/User/PasswordModifyComplete.aspx";
		public const string PAGE_MFRONT_MYPAGE = "Form/User/MyPage.aspx";
		public const string PAGE_MFRONT_USER_REGIST_REGULATION = "Form/User/UserRegistRegulation.aspx";
		public const string PAGE_MFRONT_USER_REGIST_INPUT = "Form/User/UserRegistInput.aspx";
		public const string PAGE_MFRONT_USER_REGIST_INPUT_ZIP = "Form/Top/ZipSearch.aspx";
		public const string PAGE_MFRONT_USER_REGIST_INPUT2 = "Form/User/UserRegistInput2.aspx";
		public const string PAGE_MFRONT_USER_REGIST_CONFIRM = "Form/User/UserRegistConfirm.aspx";
		public const string PAGE_MFRONT_USER_REGIST_COMPLETE = "Form/User/UserRegistComplete.aspx";
		public const string PAGE_MFRONT_LOGOUT_CONFIRM = "Form/User/LogoutConfirm.aspx";
		public const string PAGE_MFRONT_LOGOUT_COMPLETE = "Form/User/LogoutComplete.aspx";
		public const string PAGE_MFRONT_USER_MODIFY_INPUT = "Form/User/UserModifyInput.aspx";
		public const string PAGE_MFRONT_USER_MODIFY_INPUT2 = "Form/User/UserModifyInput2.aspx";
		public const string PAGE_MFRONT_USER_MODIFY_CONFIRM = "Form/User/UserModifyConfirm.aspx";
		public const string PAGE_MFRONT_USER_MODIFY_COMPLETE = "Form/User/UserModifyComplete.aspx";
		public const string PAGE_MFRONT_USER_MODIFY_INPUT2_ZIP = "Form/Top/ZipSearch.aspx";
		public const string PAGE_MFRONT_USER_WIDHDRAWAL_CONFIRM = "Form/User/UserWithdrawalConfirm.aspx";
		public const string PAGE_MFRONT_USER_WIDHDRAWAL_COMPLETE = "Form/User/UserWithdrawalComplete.aspx";
		public const string PAGE_MFRONT_USER_USER_SELECT_LOGIN = "Form/User/UserSelectLogin.aspx";
		public const string PAGE_MFRONT_USER_USERSHIPPING_LIST = "Form/User/UserShippingList.aspx";
		public const string PAGE_MFRONT_USER_USERSHIPPING_INPUT = "Form/User/UserShippingInput.aspx";
		public const string PAGE_MFRONT_USER_USERSHIPPING_CONFIRM = "Form/User/UserShippingConfirm.aspx";
		public const string PAGE_MFRONT_USER_USERSHIPPING_DELETE_CONFIRM = "Form/User/UserShippingDeleteConfirm.aspx";
		public const string PAGE_MFRONT_USER_PRODUCT_ARRIVAL_MAIL_LIST = "Form/User/UserProductArrivalMailList.aspx";
		public const string PAGE_MFRONT_USER_PRODUCT_ARRIVAL_MAIL_UPDATE_DELETE_CONFIRM = "Form/User/UserProductArrivalMailUpdateDeleteConfirm.aspx";
		public const string PAGE_MFRONT_MAILMAGAZINE_CANCEL_INPUT = "Form/User/MailMagazineCancelInput.aspx";
		public const string PAGE_MFRONT_MAILMAGAZINE_CANCEL_COMPLETE = "Form/User/MailMagazineCancelComplete.aspx";
		public const string PAGE_MFRONT_NEWS_LIST = "Form/Top/NewsList.aspx";
		public const string PAGE_MFRONT_INQUIRY_INPUT = "Form/Inquiry/InquiryInput.aspx";
		public const string PAGE_MFRONT_INQUIRY_CONFIRM = "Form/Inquiry/InquiryConfirm.aspx";
		public const string PAGE_MFRONT_INQUIRY_COMPLETE = "Form/Inquiry/InquiryComplete.aspx";
		public const string PAGE_MFRONT_MAILMAGAZINE_REGIST_INPUT = "Form/User/MailMagazineRegistInput.aspx";
		public const string PAGE_MFRONT_MAILMAGAZINE_REGIST_CONFIRM = "Form/User/MailMagazineRegistConfirm.aspx";
		public const string PAGE_MFRONT_MAILMAGAZINE_REGIST_COMPLETE = "Form/User/MailMagazineRegistComplete.aspx";
		public const string PAGE_MFRONT_USER_USERCREDITCARD_LIST = "Form/User/UserCreditCardList.aspx";
		public const string PAGE_MFRONT_USER_USERCREDITCARD_INPUT = "Form/User/UserCreditCardInput.aspx";
		public const string PAGE_MFRONT_USER_USERCREDITCARD_NAME_INPUT = "Form/User/UserCreditCardNameInput.aspx";
		public const string PAGE_MFRONT_USER_USERCREDITCARD_CONFIRM = "Form/User/UserCreditCardConfirm.aspx";
		public const string PAGE_MFRONT_USER_USERCREDITCARD_DELETE_CONFIRM = "Form/User/UserCreditCardDeleteConfirm.aspx";
		/// <summary>クレジットカード登録完了ページ</summary>
		public const string PAGE_MFRONT_USER_USERCREDITCARD_REGIST_COMPLETE = "Form/User/UserCreditCardComplete.aspx";

		public const string PAGE_MFRONT_CATEGORY_LIST = "Form/Product/CategoryList.aspx";
		public const string PAGE_MFRONT_PRODUCT_LIST = "Form/Product/ProductList.aspx";
		public const string PAGE_MFRONT_PRODUCT_DETAIL = "Form/Product/ProductDetail.aspx";
		public const string PAGE_MFRONT_PRODUCT_DETAIL2 = "Form/Product/ProductDetail2.aspx";
		public const string PAGE_MFRONT_PRODUCT_DETAIL_SUB_IMAGE_LIST = "Form/Product/ProductDetailSubImageList.aspx";
		public const string PAGE_MFRONT_PRODUCT_VARIATION_SELECT = "Form/Product/ProductVariationSelect.aspx";
		public const string PAGE_MFRONT_PRODUCT_STOCK_LIST = "Form/Product/ProductStockList.aspx";
		public const string PAGE_MFRONT_PRODUCT_SET_LIST = "Form/Product/ProductSetList.aspx";
		public const string PAGE_MFRONT_PRODUCT_REVIEW_INPUT = "Form/Product/ProductReviewInput.aspx";
		public const string PAGE_MFRONT_PRODUCT_REVIEW_CONFIRM = "Form/Product/ProductReviewConfirm.aspx";
		public const string PAGE_MFRONT_PRODUCT_ARRIVAL_MAIL_INPUT = "Form/Product/ProductArrivalMailInput.aspx";
		public const string PAGE_MFRONT_PRODUCT_ARRIVAL_MAIL_COMPLETE = "Form/Product/ProductArrivalMailComplete.aspx";
		public const string PAGE_MFRONT_ADVANCED_SEARCH = "Form/Product/AdvancedSearch.aspx";

		public const string PAGE_MFRONT_CART_LIST = "Form/Order/CartList.aspx";
		public const string PAGE_MFRONT_CART_CONOFIRM = "Form/Order/CartConfirm.aspx";
		public const string PAGE_MFRONT_ORDER_SELECT_LOGIN = "Form/Order/OrderSelectLogin.aspx";
		public const string PAGE_MFRONT_ORDER_OWNER_ZIP = "Form/Top/ZipSearch.aspx";
		public const string PAGE_MFRONT_ORDER_OWNER = "Form/Order/OrderOwner.aspx";
		public const string PAGE_MFRONT_ORDER_SHIPPING_ZIP = "Form/Top/ZipSearch.aspx";
		public const string PAGE_MFRONT_ORDER_SHIPPING = "Form/Order/OrderShipping.aspx";
		public const string PAGE_MFRONT_ORDER_PAYMENT1 = "Form/Order/OrderPayment1.aspx";
		public const string PAGE_MFRONT_ORDER_PAYMENT2 = "Form/Order/OrderPayment2.aspx";
		public const string PAGE_MFRONT_ORDER_MEMO = "Form/Order/OrderMemo.aspx";
		public const string PAGE_MFRONT_ORDER_CONFIRM = "Form/Order/OrderConfirm.aspx";
		public const string PAGE_MFRONT_ORDER_COMPLETE = "Form/Order/OrderComplete.aspx";
		public const string PAGE_MFRONT_ORDER_SHIPPING_PATTERN = "Form/Order/OrderShippingPattern.aspx";

		public const string PAGE_MFRONT_FAVORITE_LIST = "Form/Product/FavoriteList.aspx";
		public const string PAGE_MFRONT_FAVORITE_CONFIRM = "Form/Product/FavoriteConfirm.aspx";
		public const string PAGE_MFRONT_ORDERHISTORY_LIST = "Form/Order/OrderHistoryList.aspx";
		public const string PAGE_MFRONT_ORDERHISTORY_DETAIL = "Form/Order/OrderHistoryDetail.aspx";
		public const string PAGE_MFRONT_FIXED_PURCHASE_LIST = "Form/FixedPurchase/FixedPurchaseList.aspx";
		public const string PAGE_MFRONT_FIXED_PURCHASE_DETAIL = "Form/FixedPurchase/FixedPurchaseDetail.aspx";
		public const string PAGE_MFRONT_FIXED_PURCHASE_MODIFY_INPUT = "Form/FixedPurchase/FixedPurchaseModifyInput.aspx";
		public const string PAGE_MFRONT_FIXED_PURCHASE_MODIFY_CONFIRM = "Form/FixedPurchase/FixedPurchaseModifyConfirm.aspx";
		public const string PAGE_MFRONT_FIXED_PURCHASE_MODIFY_COMPLETE = "Form/FixedPurchase/FixedPurchaseModifyComplete.aspx";
		public const string PAGE_MFRONT_FIXED_PURCHASE_ZIP = "Form/Top/ZipSearch.aspx";

		public const string PAGE_MFRONT_CONTENTS = "Form/Top/Contents.aspx";

		public const string PAGE_MFRONT_RECOMMEND_SEND_TRACKING_LOG = "Form/Recommend/SendTrackingLog.aspx";

		// SBPS 注文時クレジットカード登録
		public const string PAGE_MFRONT_PAYMENT_SBPS_MULTIPAYMENT_POSTPAGE_FOR_EXEC_ORDER = "Payment/SBPS/PaymentSBPSMultiPaymentPostPageForExecOrder.aspx";
		public const string PAGE_MFRONT_PAYMENT_SBPS_MULTIPAYMENT_POSTPAGE_FOR_REGISTER_CREDITCARD = "Payment/SBPS/PaymentSBPSMultiPaymentPostPageForRegisterCreditard.aspx";
		public const string PAGE_MFRONT_PAYMENT_SBPS_MULTIPAYMENT_ORDER_NOTICE = "Payment/SBPS/PaymentSBPSMultiPaymentReceiveOrderNotice.aspx";
		public const string PAGE_MFRONT_PAYMENT_SBPS_MULTIPAYMENT_ORDER_ERROR = "Payment/SBPS/PaymentSBPSMultiPaymentReceiveOrderError.aspx";
		public const string PAGE_MFRONT_PAYMENT_SBPS_MULTIPAYMENT_ORDER_CANCEL = "Payment/SBPS/PaymentSBPSMultiPaymentReceiveOrderCancel.aspx";
		public const string PAGE_MFRONT_PAYMENT_SBPS_MULTIPAYMENT_ORDER_RESULT = "Payment/SBPS/PaymentSBPSMultiPaymentReceiveOrderResult.aspx";

		//ゼウス 注文時クレジットカード登録
		public const string PAGE_MFRONT_PAYMENT_ZEUS_CREDITPAYMENT_POSTPAGE_FOR_EXEC_ORDER = "Payment/CreditCardZeus/ZeusCreditCardPaymentPostPageForExecOrder.aspx";
		public const string PAGE_MFRONT_PAYMENT_ZEUS_CREDITPAYMENT_POSTPAGE_FOR_REGISTER_CREDITCARD = "Payment/CreditCardZeus/ZeusCreditCardPaymentPostPageForRegisterCreditcard.aspx";
		public const string PAGE_MFRONT_PAYMENT_ZEUS_CREDITPAYMENT_ORDER_NOTICE = "Payment/CreditCardZeus/ZeusCreditCardPaymentReceiveOrderNotice.aspx";
		public const string PAGE_MFRONT_PAYMENT_ZEUS_CREDITPAYMENT_ORDER_FAILER = "Payment/CreditCardZeus/ZeusCreditCardPaymentReceiveOrderFailer.aspx";
		public const string PAGE_MFRONT_PAYMENT_ZEUS_CREDITPAYMENT_ORDER_RESULT = "Payment/CreditCardZeus/ZeusCreditCardPaymentReceiveOrderResult.aspx";

		public const string PAGE_MFRONT_DOCOMO_PAYMENT_CONFIRM = "Form/DCMPmnt/DCMPmntConfirm.aspx";
		public const string PAGE_MFRONT_DOCOMO_PAYMENT_COMPLETE = "Form/DCMPmnt/DCMPmntComplete.aspx";
		public const string PAGE_MFRONT_DOCOMO_PAYMENT_CANCEL = "Form/DCMPmnt/DCMPmntCancel.aspx";

		public const string PAGE_MFRONT_SOFTBANK_PAYMENT_CONFIRM = "Form/SbMPmnt/SbMPmntConfirm.aspx";

		public const string PAGE_MFRONT_AU_PAYMENT_CONFIRM = "Form/AuMPmnt/AuMPmntConfirm.aspx";

		public const string PAGE_MFRONT_ZIPSEARCH = "Form/Top/ZipSearch.aspx";
		public const string PAGE_MFRONT_ERROR = "Form/Top/Error.aspx";
		public const string PAGE_MFRONT_SSLERROR = "Form/Top/SSLError.aspx";
		public const string PAGE_MFRONT_PENETRATION = "Form/GoogleAnalytics/ga.aspx";

		//------------------------------------------------------
		// モバイルAPI
		//------------------------------------------------------
		// DoCoMoマイメニューOK API
		public const string PAGE_MFRONT_MYMENU_COMP_DOCOMO = "MobAPI/Mymenu/MymenuRegistDoCoMo.aspx";
		public const string PAGE_MFRONT_MYMENU_COMP_SOFTBANK = "MobAPI/Mymenu/MymenuRegistSoftbank.aspx";

		//------------------------------------------------------
		// モバイルAPI Au認証用
		//------------------------------------------------------
		// AuマイメニューOK API
		public const string PAGE_MFRONT_MYMENU_COMP_AU = "MobAPI/Mymenu/MymenuRegistAu.aspx";
		// Auマイメニュー認証用ASPファイル
		public const string PAGE_MFRONT_MYMENU_AUTH_MODULE_URL = "MobAPI/AuAuth/AuthPage.asp";

		//========================================================================
		// モバイルページID
		//========================================================================
		public const string PAGEID_MFRONT_TOP = "R101_SITTOP";

		public const string PAGEID_MFRONT_LOGIN_INPUT = "R201_LOGIN1";
		public const string PAGEID_MFRONT_LOGIN_COMPLETE = "R202_LOGIN2";
		public const string PAGEID_MFRONT_REMINDER_INPUT = "R203_PWRMD1";
		public const string PAGEID_MFRONT_REMINDER_COMPLETE = "R204_PWRMD2";
		public const string PAGEID_MFRONT_USER_REGIST_REGULATION = "R205_USRRG1";
		public const string PAGEID_MFRONT_USER_REGIST_INPUT = "R206_USRRG2";
		public const string PAGEID_MFRONT_USER_REGIST_INPUT_ZIP = "R207_USRRG3";
		public const string PAGEID_MFRONT_USER_REGIST_INPUT2 = "R208_USRRG4";
		public const string PAGEID_MFRONT_USER_REGIST_CONFIRM = "R209_USRRG5";
		public const string PAGEID_MFRONT_USER_REGIST_COMPLETE = "R210_USRRG6";
		public const string PAGEID_MFRONT_PASSWORDMODIFY_INPUT = "R211_PWMD1";
		public const string PAGEID_MFRONT_PASSWORDMODIFY_COMPLETE = "R212_PWMD2";
		public const string PAGEID_MFRONT_USERSHIPPING_LIST = "R319_USGLST";
		public const string PAGEID_MFRONT_USERSHIPPING_INPUT = "R320_USGRG1";
		public const string PAGEID_MFRONT_USERSHIPPING_INPUT_ZIP = "R321_USGRG2";
		public const string PAGEID_MFRONT_USERSHIPPING_CONFIRM = "R322_USGRG3";
		public const string PAGEID_MFRONT_USERSHIPPING_DELETE_CONFIRM = "R323_USGRG4";
		public const string PAGEID_MFRONT_MAILMAGAZINE_CANCEL_INPUT = "R314_MMCNL1";
		public const string PAGEID_MFRONT_MAILMAGAZINE_CANCEL_COMPLETE = "R315_MMCNL2";
		public const string PAGEID_MFRONT_NEWS_LIST = "R316_NSLST1";
		public const string PAGEID_MFRONT_INQUIRY_INPUT = "R317_INQRG1";
		public const string PAGEID_MFRONT_INQUIRY_COMPLETE = "R318_INQRG2";
		public const string PAGEID_MFRONT_INQUIRY_CONFIRM = "R336_INQRG3";

		public const string PAGEID_MFRONT_MYPAGE = "R301_MYPAGE";
		public const string PAGEID_MFRONT_FAVORITE_LIST = "R302_FAVRT1";
		public const string PAGEID_MFRONT_FAVORITE_CONFIRM = "R303_FAVRT2";
		public const string PAGEID_MFRONT_ORDERHISTORY_LIST = "R304_ODHST1";
		public const string PAGEID_MFRONT_ORDERHISTORY_DETAIL = "R305_ODHST2";
		public const string PAGEID_MFRONT_USER_MODIFY_INPUT = "R306_USRMD1";
		public const string PAGEID_MFRONT_USER_MODIFY_INPUT2 = "R307_USRMD2";
		public const string PAGEID_MFRONT_USER_MODIFY_CONFIRM = "R308_USRMD3";
		public const string PAGEID_MFRONT_USER_MODIFY_COMPLETE = "R309_USRMD4";
		public const string PAGEID_MFRONT_LOGOUT_CONFIRM = "R310_LGOUT1";
		public const string PAGEID_MFRONT_LOGOUT_COMPLETE = "R311_LGOUT2";
		public const string PAGEID_MFRONT_USER_WIDHDRAWAL_CONFIRM = "R312_USRWD1";
		public const string PAGEID_MFRONT_USER_WIDHDRAWAL_COMPLETE = "R313_USRWD2";
		public const string PAGEID_MFRONT_FIXED_PURCHASE_LIST = "R324_FXPLST";
		public const string PAGEID_MFRONT_FIXED_PURCHASE_DETAIL = "R325_FXPDTL";
		public const string PAGEID_MFRONT_FIXED_PURCHASE_MODIFY_INPUT = "R326_FXPMD1";
		public const string PAGEID_MFRONT_FIXED_PURCHASE_MODIFY_CONFIRM = "R327_FXPMD2";
		public const string PAGEID_MFRONT_FIXED_PURCHASE_MODIFY_COMPLETE = "R328_FXPMD3";
		public const string PAGEID_MFRONT_FIXED_PURCHASE_SHIPPING_ZIP = "R328_FXPMD4";
		public const string PAGEID_MFRONT_USER_MODIFY_INPUT_ZIP = "R329_USRMD5";
		public const string PAGEID_MFRONT_USERCREDITCARD_LIST = "R337_USRCC1";
		public const string PAGEID_MFRONT_USERCREDITCARD_INPUT = "R338_USRCC2";
		public const string PAGEID_MFRONT_USERCREDITCARD_CONFIRM = "R339_USRCC3";
		public const string PAGEID_MFRONT_USERCREDITCARD_DELETE_CONFIRM = "R340_USRCC4";
		public const string PAGEID_MFRONT_USERCREDITCARD_REGISTER_COMPLETE = "R341_USRCC5";
		public const string PAGEID_MFRONT_USERCREDITCARD_NAME_INPUT = "R342_USRCC6";
		public const string PAGEID_MFRONT_USER_PRODUCT_ARRIVAL_MAIL_LIST = "R330_UPAML1";
		public const string PAGEID_MFRONT_USER_PRODUCT_ARRIVAL_MAIL_UPDATE_DELETE_CONFIRM = "R331_UPAML2";
		public const string PAGEID_MFRONT_USER_SELECT_LOGIN = "R332_SELLIN";
		public const string PAGEID_MFRONT_MAILMAGAZINE_REGIST_INPUT = "R333_MMRG1";
		public const string PAGEID_MFRONT_MAILMAGAZINE_REGIST_CONFIRM = "R334_MMRG2";
		public const string PAGEID_MFRONT_MAILMAGAZINE_REGIST_COMPLETE = "R335_MMRG3";
		public const string PAGEID_MFRONT_MAILMAGAZINE_REGIST_ZIP = "R335_MMRG4";

		public const string PAGEID_MFRONT_CATEGORY_LIST = "R401_CTGLST";
		public const string PAGEID_MFRONT_PRODUCT_LIST = "R402_PRDLST";
		public const string PAGEID_MFRONT_PRODUCT_DETAIL = "R403_PRDTL1";
		public const string PAGEID_MFRONT_PRODUCT_DETAILARRIVAL = "R403_PRDTLAVL";
		public const string PAGEID_MFRONT_PRODUCT_DETAIL2 = "R404_PRDTL2";
		public const string PAGEID_MFRONT_PRODUCT_VARIATION_SELECT = "R405_PRDVSL";
		public const string PAGEID_MFRONT_PRODUCT_STOCK_LIST = "R406_PRDSTK";
		public const string PAGEID_MFRONT_PRODUCT_SET_LIST = "R407_PRDSTL";
		public const string PAGEID_MFRONT_PRODUCT_SUB_IMAGE_LIST = "R408_PRDSIMGLIST";
		public const string PAGEID_MFRONT_PRODUCT_REVIEW_LIST = "R409_PRVLST";
		public const string PAGEID_MFRONT_PRODUCT_REVIEW_INPUT = "R410_PRVRG1";
		public const string PAGEID_MFRONT_PRODUCT_REVIEW_CONFIRM = "R411_PRVRG2";
		public const string PAGEID_MFRONT_PRODUCT_ARRIVAL_MAIL_INPUT = "R421_PRAVL1";
		public const string PAGEID_MFRONT_PRODUCT_ARRIVAL_MAIL_COMPLETE = "R422_PRAVL2";

		public const string PAGEID_MFRONT_CART_LIST = "R501_CRTLST";
		public const string PAGEID_MFRONT_CART_CONFIRM = "R502_CRTCFM";
		public const string PAGEID_MFRONT_ORDER_SELECT_LOGIN = "R503_SELLIN";
		public const string PAGEID_MFRONT_ORDER_OWNER_ZIP = "R504_OWNER1";
		public const string PAGEID_MFRONT_ORDER_OWNER = "R505_OWNER2";
		public const string PAGEID_MFRONT_ORDER_SHIPPING_ZIP = "R506_SHPNG1";
		public const string PAGEID_MFRONT_ORDER_SHIPPING = "R507_SHPNG2";
		public const string PAGEID_MFRONT_ORDER_PAYMENT1 = "R508_PAYMT1";
		public const string PAGEID_MFRONT_ORDER_PAYMENT2 = "R509_PAYMT2";
		public const string PAGEID_MFRONT_ORDER_MEMO = "R510_ODRMEM";
		public const string PAGEID_MFRONT_ORDER_CONFIRM = "R511_ODRCNF";
		public const string PAGEID_MFRONT_ORDER_COMPLETE = "R512_ODRCMP";
		public const string PAGEID_MFRONT_ORDER_SHIPPING_PATTERN = "R513_SPGPTN";

		public const string PAGEID_MFRONT_DOCOMO_PAYMENT_CONFIRM = "R601_DCMCFM";
		public const string PAGEID_MFRONT_DOCOMO_PAYMENT_CANCEL = "R603_DCMCN";

		public const string PAGEID_MFRONT_SOFTBANK_PAYMENT_CONFIRM = "R611_SBCFM";
		public const string PAGEID_MFRONT_SOFTBANK_PAYMENT_COMPLETE = "R611_SBCMP";

		public const string PAGEID_MFRONT_AU_PAYMENT_CONFIRM = "R621_AUCFM";
		public const string PAGEID_MFRONT_AU_PAYMENT_COMPLETE = "R622_AUCMP";

		public const string PAGEID_MFRONT_SBPS_PAYMENT_ORDER = "R611_SBPS_ODR";
		public const string PAGEID_MFRONT_SBPS_PAYMENT_CANCEL = "R612_SBPS_CNCL";
		public const string PAGEID_MFRONT_SBPS_CARD_REGISTER_ORDER = "R614_SBPS_CARD_RGST";
		public const string PAGEID_MFRONT_SBPS_CARD_REGISTER_CNCL = "R615_SBPS_CARD_RGST_CNCL";

		public const string PAGEID_MFRONT_ZEUS_PAYMENT_ORDER = "R621_ZEUS_ODR";
		public const string PAGEID_MFRONT_ZEUS_CARD_REGISTER = "R624_ZEUS_CARD_RGST";

		public const string PAGEID_MFRONT_ERROR = "R901_ERROR1";
		public const string PAGEID_MFRONT_SSLERROR = "R902_SSLERR";

		//========================================================================
		// ページ・コマースマネージャサイド
		//========================================================================
		//------------------------------------------------------
		// ログイン･メニュー系
		//------------------------------------------------------
		public const string PAGE_MANAGER_LOGIN = "Default.aspx";			// ファイル名を指定してあげないとPOSTできない。
		public const string PAGE_MANAGER_PAGE_INDEX_LIST = "Form/PageIndexList.aspx";	// 機能一覧

		//------------------------------------------------------
		// シングルサインオン系
		//------------------------------------------------------
		public const string PAGE_MANAGER_WEBFORMS_SINGLE_SIGN_ON = "Form/SingleSignOn.aspx";
		public const string PAGE_MAANGER_CMS_SINGLE_SIGN_ON = "SingleSignOn/Index";

		//------------------------------------------------------
		// サポートサイト系
		//------------------------------------------------------
		public const string PAGE_MANAGER_SUPPORT_INFORMATION = "Form/Support/SupportInformation.aspx";
		public const string PAGE_MANAGER_SUPPORT_INFORMATION_GETTER = "Form/Support/SupportInformationGetter.ashx";

		//------------------------------------------------------
		// ユーザー管理系
		//------------------------------------------------------
		// ユーザー情報
		public const string PAGE_MANAGER_USER_LIST = "Form/User/UserList.aspx";
		public const string PAGE_MANAGER_USER_CONFIRM = "Form/User/UserConfirm.aspx";
		public const string PAGE_MANAGER_USER_REGISTER = "Form/User/UserRegister.aspx";
		public const string PAGE_MANAGER_USER_CONFIRM_POPUP = "Form/User/UserConfirmPopup.aspx";
		public const string PAGE_MANAGER_USER_SHIPPING_INPUT = "Form/User/UserShippingInput.aspx";
		public const string PAGE_MANAGER_USER_SHIPPING_CONFIRM = "Form/User/UserShippingConfirm.aspx";
		public const string PAGE_MANAGER_USER_CREDITCARD_INPUT = "Form/User/UserCreditCardInput.aspx";
		public const string PAGE_MANAGER_USER_CREDITCARD_CONFIRM = "Form/User/UserCreditCardConfirm.aspx";
		public const string PAGE_MANAGER_USER_TARGETLIST = "Form/User/UserTargetList.aspx";
		public const string PAGE_MANAGER_USER_INVOICE_INPUT = "Form/User/UserInvoiceInput.aspx";
		public const string PAGE_MANAGER_USER_INVOICE_CONFIRM = "Form/User/UserInvoiceConfirm.aspx";
		// ユーザー管理レベル
		public const string PAGE_MANAGER_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_LIST = "Form/UserManagementLevel/UserManagementLevelList.aspx";
		// ユーザ拡張項目設定
		public const string PAGE_MANAGER_USEREXTENDSETTING_USER_EXTEND_SETTING_LIST = "Form/UserExtendSetting/UserExtendSettingList.aspx";
		public const string PAGE_MANAGER_USEREXTENDSETTING_USER_EXTEND_VALIDATION_SETTING = "Form/UserExtendSetting/UserExtendValidationSetting.aspx";
		// ユーザー統合
		public const string PAGE_MANAGER_USER_INTEGRATION_LIST = "Form/UserIntegration/UserIntegrationList.aspx";
		public const string PAGE_MANAGER_USER_INTEGRATION_REGISTER = "Form/UserIntegration/UserIntegrationRegister.aspx";
		public const string PAGE_MANAGER_USER_INTEGRATION_HISTORY_LIST = "Form/UserIntegration/UserIntegrationHistoryList.aspx";

		/// <summary>ユーザーファイル出力</summary>
		public const string PAGE_MANAGER_USERFILEEXPORT = "Form/UserFileExport/UserFileExport.aspx";

		/// <summary>Fixed purchase repeat analysis report target list page</summary>
		public const string PAGE_MANAGER_FIXEDPURCHASEREPEATANALYSISREPORT_TARGETLIST = "Form/FixedPurchaseRepeatAnalysisReport/FixedPurchaseRepeatAnalysisReportTargetList.aspx";

		//------------------------------------------------------
		// 受注管理系
		//------------------------------------------------------
		// 注文情報
		public const string PAGE_MANAGER_ORDER_LIST = "Form/Order/OrderList.aspx";
		public const string PAGE_MANAGER_ORDER_CONFIRM = "Form/Order/OrderConfirm.aspx";
		public const string PAGE_MANAGER_ORDER_MODIFY_INPUT = "Form/Order/OrderModifyInput.aspx";
		public const string PAGE_MANAGER_ORDER_MODIFY_CONFIRM = "Form/Order/OrderModifyConfirm.aspx";
		public const string PAGE_MANAGER_ORDER_USER_LIST = "Form/Order/OrderUserList.aspx";
		public const string PAGE_MANAGER_ORDER_USER_COUPON_LIST = "Form/OrderRegist/UserCouponList.aspx";

		public const string PAGE_MANAGER_ORDER_RETRUN_EXCHANGE_INPUT = "Form/Order/OrderReturnExchangeInput.aspx";
		public const string PAGE_MANAGER_ORDER_RETRUN_EXCHANGE_CONFIRM = "Form/Order/OrderReturnExchangeConfirm.aspx";

		public const string PAGE_MANAGER_ORDERCOMBINE_ORDER_COMBINE = "Form/OrderCombine/OrderCombine.aspx";
		public const string PAGE_MANAGER_FIXEDPURCHASECOMBINE_FIXEDPURCHASE_COMBINE = "Form/FixedPurchaseCombine/FixedPurchaseCombine.aspx";

		// 頒布会設定一覧画面
		public const string PAGE_MANAGER_SUBSCRIPTION_BOX_LIST = "Form/SubscriptionBox/SubscriptionBoxList.aspx";
		public const string PAGE_MANAGER_SUBSCRIPTION_BOX_REGISTER = "Form/SubscriptionBox/SubscriptionBoxRegister.aspx";

		// 注文登録
		public const string PAGE_MANAGER_ORDER_REGIST_INPUT = "Form/OrderRegist/OrderRegistInput.aspx";
		public const string PAGE_MANAGER_ORDER_REGIST_CONFIRM = "Form/OrderRegist/OrderRegistConfirm.aspx";
		public const string PAGE_MANAGER_ORDER_REGIST_ADVPOPUP = "Form/OrderRegist/AdvertisementCodeListPopup.aspx";

		// 注文ワークフロー情報
		public const string PAGE_MANAGER_ORDERWORKFLOW_LIST = "Form/OrderWorkflow/OrderWorkflowList.aspx";
		public const string PAGE_MANAGER_ORDERWORKFLOW_CONFIRM = "Form/OrderWorkflow/OrderWorkflowConfirm.aspx";
		public const string PAGE_MANAGER_ORDERWORKFLOW_ORDER_FILE_IMPORT = "Form/OrderWorkflow/OrderFileImport.ashx";

		// 注文ワークフロー設定情報
		public const string PAGE_MANAGER_ORDERWORKFLOWSETTING_LIST = "Form/OrderWorkflowSetting/OrderWorkflowSettingList.aspx";
		public const string PAGE_MANAGER_ORDERWORKFLOWSETTING_REGISTER = "Form/OrderWorkflowSetting/OrderWorkflowSettingRegister.aspx";

		// 注文ワークフロー自動実行(シナリオ)
		public const string PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_LIST = "Form/OrderWorkflowAutoExec/OrderWorkflowScenarioList.aspx";
		public const string PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_RESISTER = "Form/OrderWorkflowAutoExec/OrderWorkflowScenarioRegister.aspx";
		public const string PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_CONFIRM = "Form/OrderWorkflowAutoExec/OrderWorkflowScenarioConfirm.aspx";

		// 注文ワークフロー実行履歴
		public const string PAGE_MANAGER_ORDERWORKFLOW_EXEC_HISTORY_LIST = "Form/OrderWorkflowExecHistory/OrderWorkflowExecHistoryList.aspx";
		public const string PAGE_MANAGER_ORDERWORKFLOW_EXEC_HISTORY_DETAILS = "Form/OrderWorkflowExecHistory/OrderWorkflowExecHistoryDetails.aspx";

		// 注文関連ファイル取込
		public const string PAGE_MANAGER_ORDERFILEIMPORT_LIST = "Form/OrderFileImport/OrderFileImport.aspx";
		public const string PAGE_MANAGER_ORDERFILEIMPORT_COMP = "Form/OrderFileImport/OrderFileImportComplete.aspx";

		// 注文関連ファイル出力
		public const string PAGE_MANAGER_ORDERFILEEXPORT_LIST = "Form/OrderFileExport/OrderFileExport.aspx";

		// 定期購入系ページ
		public const string PAGE_MANAGER_FIXEDPURCHASE_LIST = "Form/FixedPurchase/FixedPurchaseList.aspx";
		public const string PAGE_MANAGER_FIXEDPURCHASE_CONFIRM = "Form/FixedPurchase/FixedPurchaseConfirm.aspx";
		public const string PAGE_MANAGER_FIXEDPURCHASE_MODIFY_INPUT = "Form/FixedPurchase/FixedPurchaseModifyInput.aspx";
		public const string PAGE_MANAGER_FIXEDPURCHASE_EXTERNAL_PAYMENT_COOPERATION_DETAILS = "Form/FixedPurchase/ExternalPaymentCooperationDetails.aspx";

		// モール連携設定
		public const string PAGE_MANAGER_MALL_CONFIG = "Form/MallLiaise/MallConfig.aspx";
		public const string PAGE_MANAGER_MALL_LIAISE_LIST = "Form/MallLiaise/MallLiaiseList.aspx";

		// モール商品アップロード
		public const string PAGE_MANAGER_MALL_PRODUCT_UPLOAD = "Form/MallProductUpload/MallProductUpload.aspx";
		public const string PAGE_MANAGER_MALL_PRODUCT_UPLOAD_PRODUCT_LIST = "Form/MallProductUpload/MallProductUploadProductList.aspx";
		public const string PAGE_MANAGER_MALL_PRODUCT_UPLOAD_NEXT_ENGINE_CSV_DOWNLOAD = "Form/MallProductUpload/NextEngineCsvDownload.aspx";

		// モール出品設定
		public const string PAGE_MANAGER_MALL_EXHIBITS_CONFIG_LIST = "Form/MallExhibitsConfig/MallExhibitsConfigList.aspx";

		// モールバッチ監視
		public const string PAGE_MANAGER_MALL_WATCHING_LOG_LIST = "Form/MallWatchingLog/MallWatchingLogList.aspx";
		public const string PAGE_MANAGER_MALL_WATCHING_LOG_DISPLAY_CONTENT = "Form/MallWatchingLog/MallWatchingLogDisplayContent.aspx";

		/// <summary>Yahoo API - 認可コードを取得する時のコールバック先 (Authorizationエンドポイント)</summary>
		public const string PAGE_MANAGER_MALL_YAHOO_API_AUTH_CALLBACK = "Form/MallLiaise/YahooApiAuthCallback.aspx";

		/// <summary>Page manager storepick up order list</summary>
		public const string PAGE_MANAGER_STOREPICKUP_ORDER_LIST = "Form/StorePickUp/StorePickUpOrderList.aspx";
		/// <summary>Page manager storepick up order detail</summary>
		public const string PAGE_MANAGER_STOREPICKUP_ORDER_DETAIL = "Form/StorePickUp/StorePickUpOrderDetail.aspx";

		// 定期商品変更設定
		/// <summary>定期商品変更設定一覧画面</summary>
		public const string PAGE_MANAGER_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_LIST = "Form/FixedPurchaseProductChangeSetting/FixedPurchaseProductChangeSettingList.aspx";
		/// <summary>定期商品変更設定登録/編集画面</summary>
		public const string PAGE_MANAGER_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_REGISTER = "Form/FixedPurchaseProductChangeSetting/FixedPurchaseProductChangeSettingRegister.aspx";
		/// <summary>定期商品変更設定詳細/確認画面</summary>
		public const string PAGE_MANAGER_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_CONFIRM = "Form/FixedPurchaseProductChangeSetting/FixedPurchaseProductChangeSettingConfirm.aspx";

		//------------------------------------------------------
		// 商品管理系
		//------------------------------------------------------
		// 商品情報
		public const string PAGE_MANAGER_PRODUCT_LIST = "Form/Product/ProductList.aspx";
		public const string PAGE_MANAGER_PRODUCT_REGISTER = "Form/Product/ProductRegister.aspx";
		public const string PAGE_MANAGER_PRODUCT_CONFIRM = "Form/Product/ProductConfirm.aspx";
		public const string PAGE_MANAGER_PRODUCT_VARIATION_MATRIX_REGISTER = "Form/Product/ProductVariationMatrixRegister.aspx";
		/// <summary>Page manager new product register</summary>
		public const string PAGE_MANAGER_NEW_PRODUCT_REGISTER = "Form/Product/V2/ProductRegister.aspx";
		/// <summary>Page manager product regist or update process</summary>
		public const string PAGE_MANAGER_PRODUCT_REGIST_OR_UPDATE_PROCESS = "Form/Common/Product/ProductRegistOrUpdateProcess.ashx";

		// 商品拡張項目
		public const string PAGE_MANAGER_PRODUCTEXTENDSETTING_LIST = "Form/ProductExtendSetting/ProductExtendSettingList.aspx";

		// 商品タグ設定
		public const string PAGE_MANAGER_PRODUCTTAGSETTING_LIST = "Form/ProductTagSetting/ProductTagSettingList.aspx";

		// 商品カテゴリ情報
		public const string PAGE_MANAGER_PRODUCTCATEGORY_REGISTER = "Form/ProductCategory/ProductCategoryRegister.aspx";

		// 商品税率カテゴリ情報
		public const string PAGE_MANAGER_PRODUCT_TAX_CATEGORY_REGISTER = "Form/ProductTaxCategory/ProductTaxCategoryList.aspx";

		// 商品レビュー情報
		public const string PAGE_MANAGER_PRODUCTREVIEW_LIST = "Form/ProductReview/ProductReviewList.aspx";
		public const string PAGE_MANAGER_PRODUCTREVIEW_REGISTER = "Form/ProductReview/ProductReviewRegister.aspx";
		public const string PAGE_MANAGER_PRODUCTREVIEW_CONFIRM = "Form/ProductReview/ProductReviewConfirm.aspx";

		// 商品セット
		public const string PAGE_MANAGER_PRODUCTSET_LIST = "Form/ProductSet/ProductSetList.aspx";
		public const string PAGE_MANAGER_PRODUCTSET_REGISTER = "Form/ProductSet/ProductSetRegister.aspx";
		public const string PAGE_MANAGER_PRODUCTSET_CONFIRM = "Form/ProductSet/ProductSetConfirm.aspx";

		// セットプロモーション情報
		public const string PAGE_MANAGER_SETPROMOTION_LIST = "Form/SetPromotion/SetPromotionList.aspx";
		public const string PAGE_MANAGER_SETPROMOTION_REGISTER = "Form/SetPromotion/SetPromotionRegister.aspx";

		// 商品在庫情報
		public const string PAGE_MANAGER_PRODUCTSTOCK_LIST = "Form/ProductStock/ProductStockList.aspx";
		public const string PAGE_MANAGER_PRODUCTSTOCKHISTORY_LIST = "Form/ProductStock/ProductStockHistoryList.aspx";

		// 商品在庫文言情報
		public const string PAGE_MANAGER_PRODUCTSTOCKMESSAGE_LIST = "Form/ProductStockMessage/ProductStockMessageList.aspx";
		public const string PAGE_MANAGER_PRODUCTSTOCKMESSAGE_REGISTER = "Form/ProductStockMessage/ProductStockMessageRegister.aspx";
		public const string PAGE_MANAGER_PRODUCTSTOCKMESSAGE_CONFIRM = "Form/ProductStockMessage/ProductStockMessageConfirm.aspx";

		// 商品セール情報
		public const string PAGE_MANAGER_PRODUCTSALE_LIST = "Form/ProductSale/ProductSaleList.aspx";
		public const string PAGE_MANAGER_PRODUCTSALE_REGISTER = "Form/ProductSale/ProductSaleRegister.aspx";

		/// <summary>Page manager product sub image setting list</summary>
		public const string PAGE_MANAGER_PRODUCTSUBIMAGESETTING_LIST = "Form/ProductSubImageSetting/ProductSubImageSettingList.aspx";

		// 商品仕入れ情報
		public const string PAGE_MANAGER_STOCKORDER_LIST = "Form/StockOrder/StockOrderList.aspx";
		public const string PAGE_MANAGER_STOCKORDER_REGIST = "Form/StockOrder/StockOrderRegist.aspx";
		public const string PAGE_MANAGER_STOCKDELIVERY_REGIST = "Form/StockOrder/StockDeliveryRegist.aspx";

		// ブランド情報
		public const string PAGE_MANAGER_PRODUCTBRAND_LIST = "Form/ProductBrand/ProductBrandList.aspx";
		public const string PAGE_MANAGER_PRODUCTBRAND_REGISTER = "Form/ProductBrand/ProductBrandRegister.aspx";
		public const string PAGE_MANAGER_PRODUCTBRAND_CONFIRM = "Form/ProductBrand/ProductBrandConfirm.aspx";

		// 入荷通知メール情報
		public const string PAGE_MANAGER_USERPRODUCTARRIVALMAIL_LIST = "Form/UserProductArrivalMail/UserProductArrivalMailList.aspx";

		// シリアルキー管理
		public const string PAGE_MANAGER_SERIALKEY_LIST = "Form/SerialKey/SerialKeyList.aspx";
		public const string PAGE_MANAGER_SERIALKEY_REGISTER = "Form/SerialKey/SerialKeyRegister.aspx";
		public const string PAGE_MANAGER_SERIALKEY_CONFIRM = "Form/SerialKey/SerialKeyConfirm.aspx";

		//ノベルティ設定
		public const string PAGE_MANAGER_NOVELTY_LIST = "Form/Novelty/NoveltyList.aspx";
		public const string PAGE_MANAGER_NOVELTY_REGISTER = "Form/Novelty/NoveltyRegister.aspx";

		// レコメンド設定
		public const string PAGE_MANAGER_RECOMMEND_LIST = "Form/Recommend/RecommendList.aspx";
		public const string PAGE_MANAGER_RECOMMEND_REGISTER = "Form/Recommend/RecommendRegister.aspx";
		public const string PAGE_MANAGER_RECOMMEND_BUTTONIMAGE_FILEUPLOADD = "Form/Recommend/RecommendButtonImageFileUpload.aspx";

		// 商品同梱設定
		public const string PAGE_MANAGER_PRODUCTBUNDLE_LIST = "Form/ProductBundle/ProductBundleList.aspx";
		public const string PAGE_MANAGER_PRODUCTBUNDLE_REGISTER = "Form/ProductBundle/ProductBundleRegister.aspx";

		// 商品検索ポップアップ
		public const string PAGE_MANAGER_PRODUCT_SEARCH = "Form/Common/ProductSearch.aspx";
		// カテゴリ検索ポップアップ
		public const string PAGE_MANAGER_PRODUCT_CATEGORY_SEARCH = "Form/Common/ProductCategorySearch.aspx";
		// ブランド検索ポップアップ
		public const string PAGE_MANAGER_PRODUCT_BRAND_SEARCH = "Form/Common/ProductBrandSearch.aspx";
		// 項目メモTooltip
		public const string PAGE_MANAGER_FIELD_MEMO_SETTING = "Form/Common/FieldMemoSetting/FieldMemoSettingInput.aspx";
		/// <summary>定期配送パターン設定</summary>
		public const string PAGE_MANAGER_FIXEDPURCHASE_SHIPPING_PATTERN = "Form/Common/FixedPurchaseShippingPattern.aspx";
		//------------------------------------------------------
		// リアル店舗管理系
		//------------------------------------------------------
		// リアル店舗情報
		public const string PAGE_MANAGER_REALSHOP_LIST = "Form/RealShop/RealShopList.aspx";
		public const string PAGE_MANAGER_REALSHOP_REGISTER = "Form/RealShop/RealShopRegister.aspx";

		// リアル店舗在庫情報
		public const string PAGE_MANAGER_REALSHOPPRODUCTSTOCK_LIST = "Form/RealShopProductStock/RealShopProductStockList.aspx";

		//------------------------------------------------------
		// サイト管理系
		//------------------------------------------------------
		// サイト設定画面
		public const string PAGE_MANAGER_SITE_CONFIGRATION = "Form/System/SiteConfiguration/SiteConfiguration.aspx";
		// サイト設定確認画面
		public const string PAGE_MANAGER_SITE_CONFIGRATION_CONFIRM = "Form/System/SiteConfiguration/SiteConfigurationConfirm.aspx";

		// サマリー情報
		public const string PAGE_MANAGER_SUMMARY_INFORMATION = "Form/Summary/SummaryInformation.aspx";

		// メニュー権限情報
		public const string PAGE_MANAGER_MENU_AUTHORITY_LIST = "Form/MenuAuthority/MenuAuthorityList.aspx";
		public const string PAGE_MANAGER_MENU_AUTHORITY_REGISTER = "Form/MenuAuthority/MenuAuthorityRegister.aspx";
		public const string PAGE_MANAGER_MENU_AUTHORITY_CONFIRM = "Form/MenuAuthority/MenuAuthorityConfirm.aspx";

		// オペレータ情報
		public const string PAGE_MANAGER_OPERATOR_LIST = "Form/Operator/OperatorList.aspx";
		public const string PAGE_MANAGER_OPERATOR_REGISTER = "Form/Operator/OperatorRegister.aspx";
		public const string PAGE_MANAGER_OPERATOR_CONFIRM = "Form/Operator/OperatorConfirm.aspx";

		// オペレータパスワード変更
		public const string PAGE_MANAGER_OPERATOR_PASSWORD_CHANGE_INPUT = "Form/OperatorPasswordChange/OperatorPasswordChangeInput.aspx";
		public const string PAGE_MANAGER_OPERATOR_PASSWORD_CHANGE_COMPLETE = "Form/OperatorPasswordChange/OperatorPasswordChangeComplete.aspx";

		// メールテンプレート
		public const string PAGE_MANAGER_MAIL_TEMPLETE_LIST = "Form/MailTemplate/MailTemplateList.aspx";
		public const string PAGE_MANAGER_MAIL_TEMPLETE_REGISTER = "Form/MailTemplate/MailTemplateRegister.aspx";
		public const string PAGE_MANAGER_MAIL_TEMPLETE_CONFIRM = "Form/MailTemplate/MailTemplateConfirm.aspx";

		// 配送種別情報
		public const string PAGE_MANAGER_SHIPPING_LIST = "Form/Shipping/ShippingList.aspx";
		public const string PAGE_MANAGER_SHIPPING_REGISTER = "Form/Shipping/ShippingRegister.aspx";
		public const string PAGE_MANAGER_SHIPPING_REGISTER2 = "Form/Shipping/ShippingRegister2.aspx";
		public const string PAGE_MANAGER_SHIPPING_CONFIRM = "Form/Shipping/ShippingConfirm.aspx";

		// 特別配送先情報
		public const string PAGE_MANAGER_SHIPPING_ZONE_LIST = "Form/ShippingZone/ShippingZoneList.aspx";
		public const string PAGE_MANAGER_SHIPPING_ZONE_REGISTER = "Form/ShippingZone/ShippingZoneRegister.aspx";
		public const string PAGE_MANAGER_SHIPPING_ZONE_CONFIRM = "Form/ShippingZone/ShippingZoneConfirm.aspx";

		/// <summary>海外配送先エリア一覧画面</summary>
		public const string PAGE_MANAGER_GLOBAL_SHIPPING_AREA_LIST = "Form/GlobalShipping/GlobalShippingAreaList.aspx";
		/// <summary>海外配送先エリア登録画面</summary>
		public const string PAGE_MANAGER_GLOBAL_SHIPPING_AREA_REGISTER = "Form/GlobalShipping/GlobalShippingAreaRegister.aspx";
		/// <summary>海外配送先エリア確認画面</summary>
		public const string PAGE_MANAGER_GLOBAL_SHIPPING_AREA_CONFIRM = "Form/GlobalShipping/GlobalShippingAreaConfirm.aspx";
		/// <summary>海外配送先エリア送料編集</summary>
		public const string PAGE_MANAGER_GLOBAL_SHIPPING_POSTAGE_EDIT = "Form/GlobalShipping/GlobalShippingPostageEdit.aspx";

		// 配送会社情報
		public const string PAGE_MANAGER_DELIVERY_COMPANY_LIST = "Form/DeliveryCompany/DeliveryCompanyList.aspx";
		public const string PAGE_MANAGER_DELIVERY_COMPANY_REGISTER = "Form/DeliveryCompany/DeliveryCompanyRegister.aspx";
		public const string PAGE_MANAGER_DELIVERY_COMPANY_CONFIRM = "Form/DeliveryCompany/DeliveryCompanyConfirm.aspx";

		// 決済種別情報
		public const string PAGE_MANAGER_PAYMENT_LIST = "Form/Payment/PaymentList.aspx";
		public const string PAGE_MANAGER_PAYMENT_REGISTER = "Form/Payment/PaymentRegister.aspx";
		public const string PAGE_MANAGER_PAYMENT_CONFIRM = "Form/Payment/PaymentConfirm.aspx";

		// 注文メモ情報
		public const string PAGE_MANAGER_ORDER_MEMO_LIST = "Form/OrderMemoInfo/OrderMemoList.aspx";
		public const string PAGE_MANAGER_ORDER_MEMO_REGISTER = "Form/OrderMemoInfo/OrderMemoRegister.aspx";
		public const string PAGE_MANAGER_ORDER_MEMO_CONFIRM = "Form/OrderMemoInfo/OrderMemoConfirm.aspx";

		// 定期解約理由区分設定
		public const string PAGE_MANAGER_FIXEDPURCHASECANCELREASON_LIST = "Form/FixedPurchaseCancelReason/FixedPurchaseCancelReasonList.aspx";

		// 休日設定
		public const string PAGE_MANAGER_HOLIDAY_MANAGEMENT_LIST = "Form/HolidayManagement/HolidayManagementList.aspx";
		public const string PAGE_MANAGER_HOLIDAY_REGISTER = "Form/HolidayManagement/HolidayRegister.aspx";

		// 自動翻訳設定
		public const string PAGE_MANAGER_AUTOTRANSLATIONWORD_LIST = "Form/AutoTranslationWord/AutoTranslationWordList.aspx";
		public const string PAGE_MANAGER_AUTOTRANSLATIONWORD_REGISTER = "Form/AutoTranslationWord/AutoTranslationWordRegister.aspx";

		//------------------------------------------------------
		// データ管理系
		//------------------------------------------------------
		// 名称翻訳設定
		public const string PAGE_MANAGER_NAMETRANSLATIONSETTING_LIST = "Form/NameTranslationSetting/NameTranslationSettingList.aspx";
		public const string PAGE_MANAGER_NAMETRANSLATIONSETTING_REGISTER = "Form/NameTranslationSetting/NameTranslationSettingRegister.aspx";

		// マスタアップロード
		public const string PAGE_MANAGER_MASTERIMPORT_LIST = "Form/MasterImport/MasterImportList.aspx";
		public const string PAGE_MANAGER_MASTERIMPORT_COMPLETE = "Form/MasterImport/MasterImportComplete.aspx";
		public const string PAGE_MANAGER_MASTERIMPORT_OUTPUTLOG = "Form/MasterImport/MasterImportOutputLog.aspx";

		// 外部ファイル取込
		public const string PAGE_MANAGER_EXTERNALIMPORT_LIST = "Form/ExternalImport/ExternalImportList.aspx";
		public const string PAGE_MANAGER_EXTERNALIMPORT_COMPLETE = "Form/ExternalImport/ExternalImportComplete.aspx";

		// CSVフォーマット設定
		public const string PAGE_MANAGER_MASTEREXPORTSETTING_REGISTER = "Form/MasterExportSetting/MasterExportSettingRegister.aspx";
		// マスタ情報出力
		public const string PAGE_MANAGER_MASTEREXPORT = "Form/MasterExport/MasterExport.aspx";

		// PDF出力
		public const string PAGE_MANAGER_PDF_OUTPUT = "Form/PdfOutput/PdfOutput.aspx";

		// ダウンロード待機
		public const string PAGE_MANAGER_DOWNLOAD_WAIT = "Form/Download/DownloadWait.aspx";

		// 商品コンバータ設定
		public const string PAGE_MANAGER_PRODUCTCONVERTER_LIST = "Form/ProductConverter/ProductConverterList.aspx";
		public const string PAGE_MANAGER_PRODUCTCONVERTER_DETAIL = "Form/ProductConverter/ProductConverterDetail.aspx";
		public const string PAGE_MANAGER_PRODUCTCONVERTER_FILES = "Form/ProductConverter/ProductConverterFiles.aspx";

		// 更新履歴情報
		public const string PAGE_MANAGER_UPDATEHISTORY_CONFIRM = "Form/UpdateHistory/UpdateHistoryConfirm.aspx";

		// 名称翻訳設定ダウンロード
		public const string PAGE_MANAGER_NAMETRANSLATIONSETTING_DOWNLOAD = "Form/NameTranslationSettingDownLoad/NameTranslationSettingDownLoad.aspx";
		public const string PAGE_MANAGER_NAMETRANSLATIONSETTING_EXPORT = "Form/Common/NameTranslationSettingExport.aspx";

		//	Electronic Invoice Management
		public const string PAGE_MANAGER_INVOICEMANAGEMENT = "Form/TwInvoice/InvoiceManagement.aspx";

		//------------------------------------------------------
		// その他系
		//------------------------------------------------------
		// エラーページ
		public const string PAGE_MANAGER_ERROR = "Form/Error.aspx";
		/// <summary>イメージコンバータ</summary>
		public const string PAGE_MANAGER_IMAGECONVERTER = "ImageCnv.aspx";
		/// <summary>HTMLプレビュー用ページ</summary>
		public const string PAGE_MANAGER_HTML_PREVIEW_FORM = "Form/Common/HtmlPreviewForm.aspx";

		// メール送信
		public const string PAGE_MANAGER_MAIL_SEND = "Form/MailSend.aspx";

		// Wysigyエディタ
		public const string PAGE_MANAGER_WYSIWYG_EDITOR = "Form/WysiwygEditor/WysiwygEditor.aspx";

		/// <summary>NextEngineアクセストークン取得ポップアップ</summary>
		public static string PAGE_MANAGER_NE_ACCESS_TOKEN_REGIST = "Form/Common/NextEngineAccessTokenRegist.aspx";

		// ベリトラン⇔payTG連携用モック
		public const string PAGE_MANAGER_REGISTER_CARD_VERITRANS_MOCK = "Form/PayTg/RegisterCardVeriTransMock.aspx";
		// <summary>payTG連携用モック</summary>
		public const string PAGE_MANAGER_REGISTER_CARD_MOCK = "Form/PayTgMock/RegisterPayTgMock.aspx";
		/// <summary>w2⇔PayTg端末状態確認用モック</summary>
		public const string PAGE_MANAGER_CHECK_DEVICE_STATUS_MOCK = "Form/PayTgMock/CheckDeviceStatusPayTgMock.aspx";

		/// <summary>Data migration manager page</summary>
		public const string PAGE_MANAGER_DATA_MIGRATION_MANAGER = "Form/DataMigration/DataMigrationManager.aspx";
		/// <summary>Data migration complete page</summary>
		public const string PAGE_MANAGER_DATA_MIGRATION_COMPLETE = "Form/DataMigration/DataMigrationComplete.aspx";
		/// <summary>Data migration output log page</summary>
		public const string PAGE_MANAGER_DATA_MIGRATION_OUTPUT_LOG = "Form/DataMigration/DataMigrationOutputLog.aspx";

		/// <summary>クーポンリストポップアップページ</summary>
		public const string PAGE_MANAGER_COUPON_LIST_POPUP = "Form/Common/CouponListPopup.aspx";

		//========================================================================
		// ページ・MPマネージャサイド
		//========================================================================
		//------------------------------------------------------
		// ログイン･メニュー系
		//------------------------------------------------------
		public const string PAGE_W2MP_MANAGER_LOGIN = "Default.aspx";						// Default.aspxがログイン画面
		public const string PAGE_W2MP_MANAGER_PAGE_INDEX_LIST = "Form/PageIndexList.aspx";	// 機能一覧

		//------------------------------------------------------
		// サポートサイト系
		//------------------------------------------------------
		public const string PAGE_W2MP_MANAGER_SUPPORT_INFORMATION = "Form/Support/SupportInformation.aspx";
		public const string PAGE_W2MP_MANAGER_SUPPORT_INFORMATION_GETTER = "Form/Support/SupportInformationGetter.ashx";

		//オプション訴求
		public const string PAGE_W2MP_MANAGER_OPTION_APPEAL = "Form/Option/Option.aspx";

		//------------------------------------------------------
		// ユーザー管理系
		//------------------------------------------------------
		// ユーザー情報
		public const string PAGE_W2MP_MANAGER_USER_LIST = "Form/User/UserList.aspx";
		public const string PAGE_W2MP_MANAGER_USER_CONFIRM = "Form/User/UserConfirm.aspx";
		public const string PAGE_W2MP_MANAGER_USER_REGISTER = "Form/User/UserRegister.aspx";

		// 会員ランク情報
		public const string PAGE_W2MP_MANAGER_MEMBER_RANK_LIST = "Form/MemberRank/MemberRankList.aspx";
		public const string PAGE_W2MP_MANAGER_MEMBER_RANK_CONFIRM = "Form/MemberRank/MemberRankConfirm.aspx";
		public const string PAGE_W2MP_MANAGER_MEMBER_RANK_REGISTER = "Form/MemberRank/MemberRankRegister.aspx";

		// 会員ランク変動ルール
		public const string PAGE_W2MP_MANAGER_MEMBER_RANK_RULE_LIST = "Form/MemberRankRule/MemberRankRuleList.aspx";
		public const string PAGE_W2MP_MANAGER_MEMBER_RANK_RULE_CONFIRM = "Form/MemberRankRule/MemberRankRuleConfirm.aspx";
		public const string PAGE_W2MP_MANAGER_MEMBER_RANK_RULE_REGISTER = "Form/MemberRankRule/MemberRankRuleRegister.aspx";
		public const string PAGE_W2MP_MANAGER_MEMBER_RANK_RULE_EXECUTE = "Form/MemberRankRule/MemberRankRuleExecute.aspx";

		// 会員ランク更新履歴
		public const string PAGE_W2MP_MANAGER_USER_MEMBER_RANK_HISTORY_LIST = "Form/UserMemberRank/UserMemberRankHistoryList.aspx";

		public const string PAGE_W2MP_MANAGER_MASTEREXPORTSETTING_REGISTER = "Form/MasterExportSetting/MasterExportSettingRegister.aspx";
		public const string PAGE_W2MP_MANAGER_MASTEREXPORT = "Form/MasterExport/MasterExport.aspx";

		//------------------------------------------------------
		// クーポン系
		//------------------------------------------------------
		// クーポン
		public const string PAGE_W2MP_MANAGER_COUPONT_LIST = "Form/Coupon/CouponList.aspx";
		public const string PAGE_W2MP_MANAGER_COUPON_REGISTER = "Form/Coupon/CouponRegister.aspx";
		public const string PAGE_W2MP_MANAGER_COUPON_CONFIRM = "Form/Coupon/CouponConfirm.aspx";
		public const string PAGE_W2MP_MANAGER_COUPON_COUPONUSEUSERLIST = "Form/Coupon/CouponUseUserList.aspx";
		// ユーザクーポン
		public const string PAGE_W2MP_MANAGER_USERCOUPON_USERLIST = "Form/UserCoupon/UserList.aspx";
		public const string PAGE_W2MP_MANAGER_USERCOUPON_USERCOUPONLIST = "Form/UserCoupon/UserCouponList.aspx";
		public const string PAGE_W2MP_MANAGER_USERCOUPON_USERCOUPONHISTORYLIST = "Form/UserCoupon/UserCouponHistoryList.aspx";
		// クーポン推移レポート
		public const string PAGE_W2MP_MANAGER_COUPON_TRANSITION_REPORT_LIST = "Form/CouponTransitionReport/CouponTransitionReportList.aspx";
		// クーポン発行スケジュール
		public const string PAGE_W2MP_MANAGER_COUPONSCHEDULE_LIST = "Form/CouponSchedule/CouponScheduleList.aspx";
		public const string PAGE_W2MP_MANAGER_COUPONSCHEDULE_REGISTER = "Form/CouponSchedule/CouponScheduleRegister.aspx";
		public const string PAGE_W2MP_MANAGER_COUPONSCHEDULE_CONFIRM = "Form/CouponSchedule/CouponScheduleConfirm.aspx";

		//------------------------------------------------------
		// アフィリエイトOP
		//------------------------------------------------------
		/// <summary>タグ閲覧権限ページ</summary>
		public const string PAGE_W2MP_MANAGER_TAG_AUTHORITY_REGISTER = "Form/TagAuthority/TagAuthorityRegister.aspx";
		// アフィリエイトレポート
		public const string PAGE_MANAGER_AFFILIATET_REPORT_LIST = "Form/AffiliateReport/AffiliateReportList.aspx";
		public const string PAGE_MANAGER_AFFILIATET_REPORT_DISPLAY_CONTENT = "Form/AffiliateReport/AffiliateReportDisplayContent.aspx";
		// 広告コード設定
		public const string PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_LIST = "Form/AdvertisementCode/AdvertisementCodeList.aspx";
		public const string PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_REGISTER = "Form/AdvertisementCode/AdvertisementCodeRegister.aspx";
		public const string PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_MEDIA_TYPE = "Form/AdvertisementCode/AdvertisementCodeMediaType.aspx";
		/// <summary>広告コード閲覧権限ページ</summary>
		public const string PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_AUTHORITY_REGISTER = "Form/AdvertisementCodeAuthority/AdvertisementCodeAuthorityRegister.aspx";
		// 広告媒体区分検索ページ
		public const string PAGE_W2MP_MANAGER_ADVERTISEMENT_CODE_MEDIA_TYPE_SEARCH = "Form/AdvertisementCode/AdvertisementCodeMediaTypeSearch.aspx";
		// 広告コードレポート
		public const string PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_REPORT_LIST = "Form/AdvertisementCodeReport/AdvertisementCodeReportList.aspx";
		public const string PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_REPORT_DETAIL = "Form/AdvertisementCodeReport/AdvertisementCodeReportDetail.aspx";

		//------------------------------------------------------
		// 基本分析OP
		//------------------------------------------------------
		// 顧客分析
		public const string PAGE_W2MP_MANAGER_USER_CONDITION_REPORT_LIST = "Form/UserConditionReport/UserConditionReportList.aspx";
		public const string PAGE_W2MP_MANAGER_USER_CONDITION_REPORT_DETAIL = "Form/UserConditionReport/UserConditionReportDetail.aspx";

		// ユーザー区分レポート
		public const string PAGE_W2MP_MANAGER_USER_KBN_REPORT_LIST = "Form/UserKbnReport/UserKbnReportList.aspx";
		public const string PAGE_W2MP_MANAGER_USER_EXTEND_REPORT_LIST = "Form/UserKbnReport/UserExtendReportList.aspx";

		// アクセス分析
		public const string PAGE_W2MP_MANAGER_ACCESS_REPORT = "Form/AccessReport/AccessReportList.aspx";
		public const string PAGE_W2MP_MANAGER_ACCESS_TABLE_REPORT = "Form/AccessReport/AccessReportTableList.aspx";

		// アクセスページランキングリポート
		public const string PAGE_W2MP_MANAGER_ACCESS_PAGE_RANKING_LIST = "Form/AccessRanking/AccessRankingList.aspx";

		// 商品ランキング
		public const string PAGE_W2MP_MANAGER_PRODUCT_RANKING_LIST = "Form/ProductRanking/ProductRankingList.aspx";
		public const string PAGE_W2MP_MANAGER_PRODUCT_RANKING_REPORT = "Form/ProductRanking/ProductRankingReport.aspx";

		// 売上状況レポート
		public const string PAGE_W2MP_MANAGER_ORDER_CONDITION_REPORT_LIST = "Form/OrderConditionReport/OrderConditionReportList.aspx";
		public const string PAGE_W2MP_MANAGER_PRODUCTLIST = "Form/OrderConditionReport/ProductList.aspx";
		public const string PAGE_W2MP_MANAGER_PRODUCT_SALE_RANKING_REPORT = "Form/OrderConditionReport/ProductSaleRankingReport.aspx";
		public const string PAGE_W2MP_MANAGER_TIME_REPORT = "Form/OrderConditionReport/TimeReport.aspx";

		// 日別出荷予測レポート
		public const string PAGE_W2MP_MANAGER_SHIPMENT_FORECAST = "Form/ShipmentForecastByDays/ShipmentForecastByDays.aspx";

		/// CPMレポート
		public const string PAGE_W2MP_MANAGER_CPM_REPORT_LIST = "Form/CpmReport/CpmReportList.aspx";

		// 定期回数別レポート
		public const string PAGE_W2MP_MANAGER_ORDER_REPEAT_REPORT = "Form/OrderRepeatReport/OrderRepeatReport.aspx";

		// 定期継続分析レポート
		public const string PAGE_W2MP_MANAGER_FIXED_PURCHASE_REPEAT_ANALYSIS_REPORT = "Form/FixedPurchaseRepeatAnalysisReport/FixedPurchaseRepeatAnalysisReport.aspx";

		/// <summary>定期売上予測レポート</summary>
		public const string PAGE_W2MP_MANAGER_FIXED_PURCHASE_FORECAST_REPORT = "Form/FixedPurchaseForecastReport/FixedPurchaseForecastReport.aspx";

		/// <summary>定期売上予測推移グラフ</summary>
		public const string PAGE_W2MP_MANAGER_FIXED_PURCHASE_FORECAST_REPORT_TRANSITIVE_GRAPH = "Form/FixedPurchaseForecastReport/FixedPurchaseForecastReportTransitiveGraph.aspx";

		// シルバーエッグレコメンド結果レポート
		public const string PAGE_W2MP_MANAGER_SILVEREGG_AIGENT_REPORT = "Form/SilvereggAigentReport/SilvereggAigentReport.aspx";

		// レコメンドレポート 詳細レポート対象選択
		public const string PAGE_W2MP_MANAGER_RECOMMEND_REPORT = "Form/RecommendReport/RecommendReportList.aspx";
		// レコメンドレポート レポート対象選択
		public const string PAGE_W2MP_MANAGER_RECOMMEND_TABLE_REPORT = "Form/RecommendReport/RecommendReportTableList.aspx";

		//------------------------------------------------------
		// ポイント系
		//------------------------------------------------------
		// ユーザーポイント情報
		public const string PAGE_W2MP_MANAGER_USERPOINT_LIST = "Form/UserPoint/UserPointList.aspx";

		// ユーザーポイント履歴情報
		public const string PAGE_W2MP_MANAGER_USERPOINTHISTORY_LIST = "Form/UserPoint/UserPointHistoryList.aspx";
		// ユーザーポイント履歴詳細
		public const string PAGE_W2MP_MANAGER_USERPOINTHISTORY_DETAIL = "Form/UserPoint/UserPointHistoryDetail.aspx";

		// ポイント基本ルール情報
		public const string PAGE_W2MP_MANAGER_POINTRULE_LIST = "Form/PointRule/PointRuleList.aspx";
		public const string PAGE_W2MP_MANAGER_POINTRULE_REGISTER = "Form/PointRule/PointRuleRegister.aspx";
		public const string PAGE_W2MP_MANAGER_POINTRULE_CONFIRM = "Form/PointRule/PointRuleConfirm.aspx";

		// ポイントキャンペーンルール情報
		public const string PAGE_W2MP_MANAGER_POINTRULE_CAMPAIGN_LIST = "Form/PointRuleCampaign/PointRuleCampaignList.aspx";
		public const string PAGE_W2MP_MANAGER_POINTRULE_CAMPAIGN_REGISTER = "Form/PointRuleCampaign/PointRuleCampaignRegister.aspx";
		public const string PAGE_W2MP_MANAGER_POINTRULE_CAMPAIGN_CONFIRM = "Form/PointRuleCampaign/PointRuleCampaignConfirm.aspx";

		// ポイント現状分析
		public const string PAGE_W2MP_MANAGER_POINT_REPORT_LIST = "Form/PointReport/PointReportList.aspx";

		// ポイント推移分析
		public const string PAGE_W2MP_MANAGER_POINT_TRANSITION_REPORT_LIST = "Form/PointTransitionReport/PointTransitionReportList.aspx";

		// ポイントルールスケジュール情報
		public const string PAGE_W2MP_MANAGER_POINTRULESCHEDULE_LIST = "Form/PointRuleSchedule/PointRuleScheduleList.aspx";
		public const string PAGE_W2MP_MANAGER_POINTRULESCHEDULE_REGISTER = "Form/PointRuleSchedule/PointRuleScheduleRegister.aspx";
		public const string PAGE_W2MP_MANAGER_POINTRULESCHEDULE_CONFIRM = "Form/PointRuleSchedule/PointRuleScheduleConfirm.aspx";
		public const string PAGE_W2MP_MANAGER_ACTIONWINDOW_SCHEDULEEXECUTE = "Form/Common/ActionWindowScheduleExecute.aspx";

		//------------------------------------------------------
		// メール配信系
		//------------------------------------------------------
		public const string PAGE_W2MP_MANAGER_MAILDIST_TEXT_LIST = "Form/MailDistText/MailDistTextList.aspx";
		public const string PAGE_W2MP_MANAGER_MAILDIST_TEXT_REGISTER = "Form/MailDistText/MailDistTextRegister.aspx";
		public const string PAGE_W2MP_MANAGER_MAILDIST_TEXT_CONFIRM = "Form/MailDistText/MailDistTextConfirm.aspx";
		public const string PAGE_W2MP_MANAGER_MAILDIST_TEXT_MAILCLICK = "Form/MailDistText/MailDistTextMailclick.aspx";
		public const string PAGE_W2MP_MANAGER_MAILDIST_TEXT_TAGMANUAL = "Form/MailDistText/MailDistTextTagManual.aspx";

		public const string PAGE_W2MP_MANAGER_MAILDIST_SETTING_LIST = "Form/MailDistSetting/MailDistSettingList.aspx";
		public const string PAGE_W2MP_MANAGER_MAILDIST_SETTING_CONFIRM = "Form/MailDistSetting/MailDistSettingConfirm.aspx";
		public const string PAGE_W2MP_MANAGER_MAILDIST_SETTING_REGISTER = "Form/MailDistSetting/MailDistSettingRegister.aspx";
		public const string PAGE_W2MP_MANAGER_MAILDIST_EXECUTE = "Form/MailDistSetting/MailDistExecute.aspx";

		// デコメファイル管理
		public const string PAGE_W2MP_MANAGER_MOBILEHTMLMAILFILE_MANAGER = "Form/DecomeFileManager/DecomeFileManager.aspx";
		public const string PAGE_W2MP_MANAGER_MOBILEHTMLMAILFILE_POPUP = "Form/DecomeFileManager/DecomeFilePopup.aspx";

		// 絵文字コード設定
		public const string PAGE_W2MP_MANAGER_MOBILEPICTORIALSYMBOL_LIST = "Form/MobilePictorialSymbol/MobilePictorialSymbolList.aspx";
		public const string PAGE_W2MP_MANAGER_MOBILEPICTORIALSYMBOL_REGISTER = "Form/MobilePictorialSymbol/MobilePictorialSymbolRegister.aspx";
		public const string PAGE_W2MP_MANAGER_MOBILEPICTORIALSYMBOL_POPUPLIST = "Form/MobilePictorialSymbol/MobilePictorialSymbolPopupList.aspx";

		public const string PAGE_W2MP_MANAGER_TARGETLIST_LIST = "Form/TargetList/TargetListList.aspx";
		public const string PAGE_W2MP_MANAGER_TARGETLIST_CONFIRM = "Form/TargetList/TargetListConfirm.aspx";
		public const string PAGE_W2MP_MANAGER_TARGETLIST_REGISTER = "Form/TargetList/TargetListRegister.aspx";
		public const string PAGE_W2MP_MANAGER_TARGETLIST_UPLOAD = "Form/TargetList/TargetListUpload.aspx";
		public const string PAGE_W2MP_MANAGER_TARGETLIST_MERGE = "Form/TargetListMerge/TargetListMerge.aspx";
		public const string PAGE_W2MP_MANAGER_TARGETLIST_LIST_POPUP = "Form/TargetListMerge/TargetListPopup.aspx";
		public const string PAGE_W2MP_MANAGER_TARGETLIST_MERGE_COMPLETE = "Form/TargetListMerge/TargetListMergeComplete.aspx";
		public const string PAGE_W2MP_MANAGER_TARGETLIST_TEMPLATELIST = "Form/TargetList/TargetListTemplateList.aspx";

		public const string PAGE_W2MP_MANAGER_MAILCLICKREPORT_LIST = "Form/MailClickReport/MailClickReportList.aspx";
		public const string PAGE_W2MP_MANAGER_MAILCLICKREPORT_DETAIL = "Form/MailClickReport/MailClickReportDetail.aspx";
		public const string PAGE_W2MP_MANAGER_MAILCLICKREPORT_DETAIL2 = "Form/MailClickReport/MailClickReportDetail2.aspx";

		//------------------------------------------------------
		// サイト管理系
		//------------------------------------------------------
		// メニュー権限設定
		public const string PAGE_W2MP_MANAGER_MENU_AUTHORITY_LIST = "Form/MenuAuthority/MenuAuthorityList.aspx";
		public const string PAGE_W2MP_MANAGER_MENU_AUTHORITY_REGISTER = "Form/MenuAuthority/MenuAuthorityRegister.aspx";
		public const string PAGE_W2MP_MANAGER_MENU_AUTHORITY_CONFIRM = "Form/MenuAuthority/MenuAuthorityConfirm.aspx";

		// オペレータ情報
		public const string PAGE_W2MP_MANAGER_OPERATOR_LIST = "Form/Operator/OperatorList.aspx";
		public const string PAGE_W2MP_MANAGER_OPERATOR_REGISTER = "Form/Operator/OperatorRegister.aspx";
		public const string PAGE_W2MP_MANAGER_OPERATOR_CONFIRM = "Form/Operator/OperatorConfirm.aspx";

		// オペレータパスワード変更
		public const string PAGE_W2MP_MANAGER_OPERATOR_PASSWORD_CHANGE_INPUT = "Form/OperatorPasswordChange/OperatorPasswordChangeInput.aspx";
		public const string PAGE_W2MP_MANAGER_OPERATOR_PASSWORD_CHANGE_COMPLETE = "Form/OperatorPasswordChange/OperatorPasswordChangeComplete.aspx";

		//------------------------------------------------------
		// その他系
		//------------------------------------------------------
		// エラーページ
		public const string PAGE_W2MP_MANAGER_ERROR = "Form/Error.aspx";
		// 広告コード検索ページ
		public const string PAGE_W2MP_MANAGER_ADVERTISEMENT_CODE_SEARCH = "Form/Common/AdvertisementCodeSearch.aspx";
		// HTMLプレビュー用ページ
		public const string PAGE_W2MP_MANAGER_HTML_PREVIEW_FORM = "Form/Common/HtmlPreviewForm.aspx";

		//========================================================================
		// ページ・CSマネージャサイド
		//========================================================================
		//------------------------------------------------------
		// 共通系
		//------------------------------------------------------
		// ログイン・メニュー系
		public const string PAGE_W2CS_MANAGER_LOGIN = "Default.aspx";						// Default.aspxがログイン画面
		public const string PAGE_W2CS_MANAGER_PAGE_INDEX_LIST = "Form/PageIndexList.aspx";	// 機能一覧

		// サポートサイト系
		public const string PAGE_W2CS_MANAGER_SUPPORT_INFORMATION = "Form/Support/SupportInformation.aspx";
		public const string PAGE_W2CS_MANAGER_SUPPORT_INFORMATION_GETTER = "Form/Support/SupportInformationGetter.ashx";

		// エラーページ
		public const string PAGE_W2CS_MANAGER_ERROR = "Form/Error.aspx";

		//------------------------------------------------------
		// CS問合せ管理系
		//------------------------------------------------------
		// トップページ系
		public const string PAGE_W2CS_MANAGER_TOP_PAGE = "Form/Top/TopPage.aspx";
		public const string PAGE_W2CS_MANAGER_TOP_PAGE_PATH = "Form/Top/";		// オペレーションメニュー用
		public const string PAGE_W2CS_MANAGER_TOP_PAGE_PAGE = "TopPage.aspx";	// オペレーションメニュー用
		public const string PAGE_W2CS_MANAGER_TOP_MAILACTION = "Form/Top/MailAction.aspx";
		public const string PAGE_W2CS_MANAGER_TOP_PRINT_MESSAGE = "Form/Top/PrintMessage.aspx";

		// メッセージ画面
		public const string PAGE_W2CS_MANAGER_MESSAGE_USER_SEARCH = "Form/Message/UserSearch.aspx";
		public const string PAGE_W2CS_MANAGER_MESSAGE_MESSAGE_INPUT = "Form/Message/MessageInput.aspx";
		public const string PAGE_W2CS_MANAGER_MESSAGE_MESSAGE_CONFIRM = "Form/Message/MessageConfirm.aspx";
		public const string PAGE_W2CS_MANAGER_MESSAGE_MAIL_SIGNATURE_LIST = "Form/Message/MailSignatureList.aspx";
		public const string PAGE_W2CS_MANAGER_MESSAGE_MAILSENDLOG_CONFIRM = "Form/Message/MailSendLogConfirm.aspx";

		// 共有情報画面
		public const string PAGE_W2CS_MANAGER_SHAREINFO_LIST = "Form/ShareInfo/ShareInfoList.aspx";
		public const string PAGE_W2CS_MANAGER_SHAREINFO_CONFIRM = "Form/ShareInfo/ShareInfoConfirm.aspx";

		// メール添付ファイルダウンローダー・アップローダー
		public const string PAGE_W2CS_MANAGER_MESSAGE_MAILATTACHMENTDOWNLOADER = "Form/Message/MailAttachmentDownloader.aspx";
		public const string PAGE_W2CS_MANAGER_MESSAGE_MAILATTACHMENTUPLOADER = "Form/Message/MailAttachmentUploader.aspx";

		// インシデントVOC設定
		public const string PAGE_W2CS_MANAGER_INCIDENTVOC_LIST = "Form/IncidentVoc/IncidentVocList.aspx";
		public const string PAGE_W2CS_MANAGER_INCIDENTVOC_REGISTER = "Form/IncidentVoc/IncidentVocRegister.aspx";
		public const string PAGE_W2CS_MANAGER_INCIDENTVOC_CONFIRM = "Form/IncidentVoc/IncidentVocConfirm.aspx";

		// インシデンカテゴリ管理
		public const string PAGE_W2CS_MANAGER_INCIDENTCATEGORY_REGISTER = "Form/IncidentCategory/IncidentCategoryRegister.aspx";

		// 集計区分設定
		public const string PAGE_W2CS_MANAGER_SUMMARYSETTING_LIST = "Form/SummarySetting/SummarySettingList.aspx";
		public const string PAGE_W2CS_MANAGER_SUMMARYSETTING_REGISTER = "Form/SummarySetting/SummarySettingRegister.aspx";
		public const string PAGE_W2CS_MANAGER_SUMMARYSETTING_CONFIRM = "Form/SummarySetting/SummarySettingConfirm.aspx";

		// メール署名管理
		public const string PAGE_W2CS_MANAGER_MAILSIGNATURE_LIST = "Form/MailSignature/MailSignatureList.aspx";
		public const string PAGE_W2CS_MANAGER_MAILSIGNATURE_REGISTER = "Form/MailSignature/MailSignatureRegister.aspx";
		public const string PAGE_W2CS_MANAGER_MAILSIGNATURE_CONFIRM = "Form/MailSignature/MailSignatureConfirm.aspx";

		// メール送信元管理
		public const string PAGE_W2CS_MANAGER_MAILFROM_LIST = "Form/MailFrom/MailFromList.aspx";
		public const string PAGE_W2CS_MANAGER_MAILFROM_REGISTER = "Form/MailFrom/MailFromRegister.aspx";
		public const string PAGE_W2CS_MANAGER_MAILFROM_CONFIRM = "Form/MailFrom/MailFromConfirm.aspx";

		// 回答例管理
		public const string PAGE_W2CS_MANAGER_ANSWERTEMPLATE_LIST = "Form/AnswerTemplate/AnswerTemplateList.aspx";
		public const string PAGE_W2CS_MANAGER_ANSWERTEMPLATE_REGISTER = "Form/AnswerTemplate/AnswerTemplateRegister.aspx";
		public const string PAGE_W2CS_MANAGER_ANSWERTEMPLATE_CONFIRM = "Form/AnswerTemplate/AnswerTemplateConfirm.aspx";

		// 回答例カテゴリ管理
		public const string PAGE_W2CS_MANAGER_ANSWERTEMPLATECATEGORY_REGISTER = "Form/AnswerTemplateCategory/AnswerTemplateCategoryRegister.aspx";

		// 共有情報管理
		public const string PAGE_W2CS_MANAGER_SHAREINFOSETTING_LIST = "Form/ShareInfoSetting/ShareInfoSettingList.aspx";
		public const string PAGE_W2CS_MANAGER_SHAREINFOSETTING_REGISTER = "Form/ShareInfoSetting/ShareInfoSettingRegister.aspx";
		public const string PAGE_W2CS_MANAGER_SHAREINFOSETTING_CONFIRM = "Form/ShareInfoSetting/ShareInfoSettingConfirm.aspx";

		// 外部リンク設定管理
		public const string PAGE_W2CS_MANAGER_EXTERNALLINKPREFERENCE_LIST = "Form/ExternalLinkPreference/ExternalLinkPreferenceList.aspx";
		public const string PAGE_W2CS_MANAGER_EXTERNALLINKPREFERENCE_REGISTER = "Form/ExternalLinkPreference/ExternalLinkPreferenceRegister.aspx";
		public const string PAGE_W2CS_MANAGER_EXTERNALLINKPREFERENCE_CONFIRM = "Form/ExternalLinkPreference/ExternalLinkPreferenceConfirm.aspx";

		// メール振分設定
		public const string PAGE_W2CS_MANAGER_MAILASSIGNSETTING_LIST = "Form/MailAssignSetting/MailAssignSettingList.aspx";
		public const string PAGE_W2CS_MANAGER_MAILASSIGNSETTING_REGISTER = "Form/MailAssignSetting/MailAssignSettingRegister.aspx";
		public const string PAGE_W2CS_MANAGER_MAILASSIGNSETTING_CONFIRM = "Form/MailAssignSetting/MailAssignSettingConfirm.aspx";
		public const string PAGE_W2CS_MANAGER_MAILASSIGNSETTING_COMPLETE = "Form/MailAssignSetting/MailAssignSettingComplete.aspx";

		// レポート系
		public const string PAGE_W2CS_MANAGER_INCIDENT_REPORT = "Form/ReportIncident/IncidentReport.aspx";
		public const string PAGE_W2CS_MANAGER_MESSAGE_REPORT = "Form/ReportMessage/MessageReport.aspx";
		public const string PAGE_W2CS_MANAGER_CSWORKFLOW_REPORT = "Form/ReportCsWorkflow/CsWorkflowReport.aspx";

		// オペレータ権限情報
		public const string PAGE_W2CS_MANAGER_OPERATOR_AUTHORITY_LIST = "Form/OperatorAuthority/OperatorAuthorityList.aspx";
		public const string PAGE_W2CS_MANAGER_OPERATOR_AUTHORITY_REGISTER = "Form/OperatorAuthority/OperatorAuthorityRegister.aspx";
		public const string PAGE_W2CS_MANAGER_OPERATOR_AUTHORITY_CONFIRM = "Form/OperatorAuthority/OperatorAuthorityConfirm.aspx";

		// CSグループ管理
		public const string PAGE_W2CS_MANAGER_CSGROUP_REGISTER = "Form/CsGroup/CsGroupRegister.aspx";

		// CSオペレータ所属グループ管理
		public const string PAGE_W2CS_MANAGER_CSOPERATORGROUP_LIST = "Form/CsOperatorGroup/CsOperatorGroupList.aspx";
		public const string PAGE_W2CS_MANAGER_CSOPERATORGROUP_REGISTER = "Form/CsOperatorGroup/CsOperatorGroupRegister.aspx";

		// Wysigyエディタ
		public const string PAGE_W2CS_MANAGER_WYSIWYG_EDITOR = "Form/WysiwygEditor/WysiwygEditor.aspx";

		// Incident modify url
		public const string PAGE_W2CS_MANAGER_INCIDENT_INCIDENT_MODIFY_INPUT = "Form/Incident/IncidentModifyInput.aspx";

		// 複数住所検索URL
		public const string PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON = "Form/ZipcodeSearcher.aspx/GetAddrJson";

		/// <summary>HTMLプレビュー用ページ</summary>
		public const string PAGE_W2CS_MANAGER_HTML_PREVIEW_FORM = "Form/Common/HtmlPreviewForm.aspx";

		/// <summary>Data migration manager page</summary>
		public const string PAGE_W2CS_MANAGER_DATA_MIGRATION_MANAGER = "Form/DataMigration/DataMigrationManager.aspx";
		/// <summary>Data migration complete page</summary>
		public const string PAGE_W2CS_MANAGER_DATA_MIGRATION_COMPLETE = "Form/DataMigration/DataMigrationComplete.aspx";

		/// <summary>LINE Pay payment receive page</summary>
		public const string PAGE_FRONT_PAYMENT_LINEPAY_RECEIVE = "Payment/LinePay/LinePayReceive.aspx";

		/// <summary>ベリトランスPayPayの外部遷移後ページ</summary>
		public const string PAGE_FRONT_PAYMENT_VERITRANS_PAYPAY_RECEIVE = "Payment/VeriTransPayPay/VeriTransPayPayReceive.aspx";
		/// <summary>ベリトランスPayPayの外部遷移時ページ</summary>
		public const string PAGE_FRONT_PAYMENT_VERITRANS_PAYPAY_AUTH = "Payment/VeriTransPayPay/VeriTransPayPayAuth.aspx";
		/// <summary>ベリトランスPayPayの結果通知受信ページ</summary>
		public const string PAGE_FRONT_PAYMENT_VERITRANS_PAYPAY_NOTIFICATION_RECEIVE = "Payment/VeriTransPayPay/VeriTransPayPayNotificationReceive.aspx";

		/// <summary>NewebPay Payment Receive Page</summary>
		public const string PAGE_FRONT_PAYMENT_NEWEBPAY_RECEIVE = "Payment/NewebPay/NewebPayReceive.ashx";
		/// <summary>NewebPay Payment Order Result Page</summary>
		public const string PAGE_FRONT_PAYMENT_NEWEBPAY_ORDER_RESULT = "Form/Order/NewebPay/NewebPayOrderResult.aspx";
		/// <summary>NewebPay Payment Barcode Page</summary>
		public const string PAGE_FRONT_PAYMENT_NEWEBPAY_BARCODE = "Form/Order/NewebPay/NewebPayBarcode.aspx";

		/// <summary>Page front payment Zcom card 3DSecure post</summary>
		public const string PAGE_FRONT_PAYMENT_ZCOM_CARD_3DSECURE_POST = "Payment/Card3DSecureAuthZcom/PostCard3DSecureAuth.aspx";
		/// <summary>Page front payment Zcom card 3DSecure get result</summary>
		public const string PAGE_FRONT_PAYMENT_ZCOM_CARD_3DSECURE_GET_RESULT = "Payment/Card3DSecureAuthZcom/GetCard3DSecureAuthResult.aspx";

		/// <summary>ベリトランス3Dセキュア認証_情報送信画面</summary>
		public const string PAGE_FRONT_PAYMENT_VERITRANS_CARD_3DSECURE_GET_POST = "Payment/Card3DSecureAuthVeriTrans/PostCard3DSecureAuth.aspx";
		/// <summary>ベリトランス3Dセキュア認証_結果通知画面</summary>
		public const string PAGE_FRONT_PAYMENT_VERITRANS_CARD_3DSECURE_GET_RESULT = "Payment/Card3DSecureAuthVeriTrans/GetCard3DSecureAuthResult.aspx";
		/// <summary>GMO決済：GMO3Dセキュア2.0認証結果取得ページ</summary>
		public const string PAGE_FRONT_PAYMENT_GMO_CARD_3DSECURE2_GET_RESULT = "Payment/Card3DSecureAuthVeriTrans/GetCard3DSecureAuthResultReceive.aspx";

		/// <summary>Product zoom image</summary>
		public const string PAGE_FRONT_PRODUCT_ZOOM_IMAGE = "Form/Product/ProductZoomImage.aspx";

		/// <summary>Order workflow getter</summary>
		public const string PAGE_MANAGER_ORDERWORKFLOW_GETTER = "Form/Common/OrderWorkflow/OrderWorkflowGetter.ashx";

		/// <summary>Exchange rate setting page</summary>
		public const string PAGE_MANAGER_EXCHANGERATE_SETTING = "Form/ExchangeRateSetting/ExchangeRateSetting.aspx";

		/// <summary>Rakuten cvs payment receive page</summary>
		public const string PAGE_FRONT_PAYMENT_RAKUTEN_CVS_PAYMENT_RECEIVE = "Payment/Rakuten/PaymentRakutenCvsPaymentRecv.aspx";

		//========================================================================
		// ページ・CMSマネージャサイド
		//========================================================================

		/// <summary>ログインコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_LOGIN = "Login";
		/// <summary>エラーコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_ERROR = "Error";
		/// <summary>店舗管理者コントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_OPERATOR = "ShopOperator";
		/// <summary>サポート情報コントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_SUPPORT_INFORMATION = "SupportInformation";
		/// <summary>メニュー権限設定コントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_MENU_AUTHORITY = "MenuAuthority";
		/// <summary>パスワード変更コントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_OPERATOR_PASSWORD_CHANGE = "OperatorPasswordChange";
		/// <summary>サイト情報コントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_SITE_INFORMATION = "SiteInformation";
		/// <summary>お知らせコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_NEWS = "News";
		/// <summary>HTML編集ウィンドウコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_WYSIWYG_EDITOR = "_WysiwygEditor";
		/// <summary>コンテンツマネージャーコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_Contents_Manager = "ContentsManager";
		/// <summary>タグマネージャーコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_TAG_MANAGER = "TagManager";
		/// <summary>ページインデックスリストコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_PAGE_INDEX_LIST = "PageIndexList";
		/// <summary>商品タグマネージャーポップアップウィンドウ コントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_PRODUCT_TAG_MANAGER = "_ProductTagManager";
		/// <summary>検索ポップアップウィンドウ コントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_SEARCH_POPUP = "_SearchPopup";
		/// <summary>マスタダウンロード設定コントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_MASTER_EXPORT_SETTING = "MasterExportSetting";
		/// <summary>マスタインポートコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_MASTER_IMPORT = "MasterImport";
		/// <summary>商品ランキングコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_PRODUCT_RANKING = "ProductRanking";
		/// <summary>商品一覧表示設定コントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_PRODUCT_LIST_DISP_SETTING = "ProductListDispSetting";
		/// <summary>スタッフコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_STAFF = "Staff";
		/// <summary>コーディネートカテゴリコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_COORDINATE_CATEGORY = "CoordinateCategory";
		/// <summary>ショートURLコントローラー</summary>
		public const string CONTROLLER_W2CMS_MANAGER_SHORT_URL = "ShortUrl";
		/// <summary>特集画像管理コントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_FEATURE_IMAGE = "FeatureImage";
		/// <summary>商品グループコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_PRODUCT_GROUP = "ProductGroup";
		/// <summary>ページ管理</summary>
		public const string CONTROLLER_W2CMS_MANAGER_PAGE_DESIGN = "PageDesign";
		/// <summary>パーツ管理</summary>
		public const string CONTROLLER_W2CMS_MANAGER_PARTS_DESIGN = "PartsDesign";
		/// <summary>ページパーツ管理サブ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_PAGEPARTS_DESIGN_SUB = "PagePartsDesignSub";
		/// <summary>パーシャルビュー:公開範囲設定</summary>
		public const string CONTROLLER_W2CMS_MANAGER_RELEASE_RANG_SETTING = "_ReleaseRangeSetting";
		/// <summary>特集ページコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_FEATURE_PAGE_DESIGN = "FeaturePage";
		/// <summary>特集ページカテゴリコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_FEATURE_PAGE_CATEGORY = "FeaturePageCategory";
		/// <summary>SEO設定コントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_SEO_METADATAS = "SeoMetadatas";
		/// <summary>コーディネートコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_COORDINATE_PAGE = "CoordinatePage";
		/// <summary>コンテンツマネージャーコントローラ(CSS)</summary>
		public const string CONTROLLER_W2CMS_MANAGER_CSS_DESIGN = "CSSDesign";
		/// <summary>コンテンツマネージャーコントローラ(JS)</summary>
		public const string CONTROLLER_W2CMS_MANAGER_JAVASCRIPT_DESIGN = "JavaScriptDesign";
		/// <summary>ランディングページコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_LANDING_PAGE = "LandingPage";
		/// <summary>特集エリアコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_FEATURE_AREA = "FeatureArea";
		/// <summary>特集エリアタイプコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_FEATURE_AREA_TYPE = "FeatureAreaType";
		/// <summary>サイトマップコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_SITEMAP = "Sitemap";
		/// <summary>OGP設定コントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_OGP_SETTING = "OgpTagSetting";
		/// <summary>シングルサインオンコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_SINGLE_SIGN_ON = "SingleSignOn";
		/// <summary>ABテストコントローラ</summary>
		public const string CONTROLLER_W2CMS_MANAGER_AB_TEST = "AbTest";
		/// <summary>Scoring sale controller</summary>
		public const string CONTROLLER_W2CMS_MANAGER_SCORING_SALE = "ScoringSale";
		/// <summary>Scoring sale question controller</summary>
		public const string CONTROLLER_W2CMS_MANAGER_SCORING_SALE_QUESTION = "ScoringSaleQuestion";
		/// <summary>Score axis controller</summary>
		public const string CONTROLLER_W2CMS_MANAGER_SCORE_AXIS = "ScoreAxis";

		/// <summary>プラン名のEP</summary>
		public const string PLAN_NAME_EP = "EP";
		/// <summary>ECのURL参照してメニュー名へ変換するリスト。</summary>
		public static readonly Dictionary<string, string> LIST_MENU_NAME_CHANGE_FROM_URL_EC = new Dictionary<string, string>()
		{
			{ "Default.aspx", "EC > ログイン" },
			{ "Form/Error.aspx", "EC > システムエラー" },
			{ "Form/MailSend.aspx", "EC > メール送信フォーム" },
			{ "Form/PageIndexList.aspx", "EC > 機能一覧" },
			{ "Form/SingleSignOn.aspx", "EC > シングルサインオン" },
			{ "Form/Support/", "EC > サポート＆サービス > サポートサイト" },
			{ "Form/Option/", "EC > サポート&サービス > 拡張オプション" },
			{ "Form/User/", "EC > ユーザー管理 > ユーザー情報" },
			{ "Form/UserManagementLevel/", "EC > ユーザー管理 > ユーザー管理レベル設定" },
			{ "Form/UserExtendSetting/", "EC > ユーザー管理 > ユーザー拡張項目設定" },
			{ "Form/UserIntegration/", "EC > ユーザー管理 > ユーザー統合" },
			{ "Form/UserEasyRegisterSetting/", "EC > ユーザー管理 > かんたん会員登録設定" },
			{ "Form/Order/", "EC > 受注管理 > 受注情報" },
			{ "Form/UpdateHistory/", "EC > 受注管理 > 更新履歴一覧" },
			{ "Form/FixedPurchase/", "EC > 受注管理 > 定期台帳" },
			{ "Form/OrderWorkflow/", "EC > 受注管理 > 受注ワークフロー" },
			{ "Form/OrderWorkflowAutoExec/", "EC > 受注管理 > 受注ワークフローシナリオ設定" },
			{ "Form/OrderWorkflowExecHistory/", "EC > 受注管理 > ワークフロー実行履歴" },
			{ "Form/OrderWorkflowSetting/", "EC > 受注管理 > 受注ワークフロー設定" },
			{ "Form/OrderExtendStatusSetting/", "EC > 受注管理 > 注文拡張ステータス設定" },
			{ "Form/OrderExtendSetting/", "EC > 受注管理 > 注文拡張項目設定" },
			{ "Form/OrderFileImport/", "EC > 受注管理 > 注文関連ファイル取込" },
			{ "Form/OrderCombine/", "EC > 受注管理 > 注文同梱" },
			{ "Form/FixedPurchaseProductChangeSetting/", "EC > 受注管理 > 定期商品変更" },
			{ "Form/FixedPurchaseCombine/", "EC > 受注管理 > 定期台帳同梱" },
			{ "Form/OrderRegist/", "EC > 受注管理 > 新規注文登録" },
			{ "Form/Product/V2/", "EC > 商品管理 > 新規商品登録" },
			{ "Form/Product/", "EC > 商品管理 > 商品情報" },
			{ "Form/ProductCategory/", "EC > 商品管理 > 商品カテゴリ設定" },
			{ "Form/ProductTaxCategory/", "EC > 商品管理 > 商品税率カテゴリ設定" },
			{ "Form/ProductTagSetting/", "EC > 商品管理 > 商品タグ設定" },
			{ "Form/ProductBrand/", "EC > 商品管理 > 商品ブランド設定" },
			{ "Form/ProductSubImageSetting/", "EC > 商品管理 > 商品サブ画像設定" },
			{ "Form/ProductStock/", "EC > 在庫管理 > 在庫情報" },
			{ "Form/ProductStockMessage/", "EC > 在庫管理 > 在庫文言設定" },
			{ "Form/StockOrder/", "EC > 在庫管理 > 発注入庫管理" },
			{ "Form/SerialKey/", "EC > 在庫管理 > シリアルキー情報" },
			{ "Form/SetPromotion/", "EC > プロモーション管理 > セットプロモーション設定" },
			{ "Form/ProductSet/", "EC > プロモーション管理 > 商品セット設定" },
			{ "Form/Novelty/", "EC > プロモーション管理 > ノベルティ設定" },
			{ "Form/ProductSale/", "EC > プロモーション管理 > セール設定" },
			{ "Form/ProductReview/", "EC > プロモーション管理 > レビュー管理" },
			{ "Form/UserProductArrivalMail/", "EC > プロモーション管理 > 入荷通知メール管理" },
			{ "Form/Recommend/", "EC > プロモーション管理 > レコメンド設定" },
			{ "Form/ProductBundle/", "EC > プロモーション管理 > 商品同梱設定" },
			{ "Form/SubscriptionBox/", "EC > プロモーション管理 > 頒布会コース設定" },
			{ "Form/RealShop/", "EC > リアル店舗管理 > リアル店舗情報" },
			{ "Form/RealShopProductStock/", "EC > リアル店舗管理 > リアル店舗在庫情報" },
			{ "Form/MallProductUpload/", "EC > モール管理 > モール商品アップロード" },
			{ "Form/MallExhibitsConfig/", "EC > モール管理 > モール出品設定" },
			{ "Form/MallWatchingLog/", "EC > モール管理 > モール連携監視" },
			{ "Form/MallLiaise/", "EC > モール管理 > モール連携基本設定" },
			{ "Form/ProductConverter/", "EC > モール管理 > 商品コンバータ設定" },
			{ "Form/ProductExtendSetting/", "EC > モール管理 > 商品拡張項目設定" },
			{ "Form/Summary/", "EC > サイト管理 > ダッシュボード" },
			{ "Form/Payment/", "EC > サイト管理 > 決済種別設定" },
			{ "Form/Shipping/", "EC > サイト管理 > 配送種別設定" },
			{ "Form/DeliveryCompany/", "EC > サイト管理 > 配送サービス設定" },
			{ "Form/ShippingZone/", "EC > サイト管理 > 特別配送先設定" },
			{ "Form/GlobalShipping/", "EC > サイト管理 > 配送地域設定" },
			{ "Form/OrderMemoInfo/", "EC > サイト管理 > 注文メモ設定" },
			{ "Form/OrderListDispSetting/", "EC > サイト管理 > 受注一覧表示設定" },
			{ "Form/FixedPurchaseCancelReason/", "EC > サイト管理 > 定期解約理由区分設定" },
			{ "Form/MailTemplate/", "EC > サイト管理 > メールテンプレート設定" },
			{ "Form/HolidayManagement/", "EC > サイト管理 > 休日設定" },
			{ "Form/ExchangeRateSetting/", "EC > サイト管理 > 為替レート設定" },
			{ "Form/AutoTranslationWord/", "EC > サイト管理 > 自動翻訳設定" },
			{ "Form/MaintenanceSetting/", "EC > サイト管理 > メンテナンス設定" },
			{ "Form/DataMigration/", "EC > データ管理 > データ移行管理" },
			{ "Form/MasterImport/", "EC > データ管理 > マスタアップロード" },
			{ "Form/MasterExportSetting/", "EC > データ管理 > マスタダウンロード設定" },
			{ "Form/ExternalImport/", "EC > データ管理 > 外部ファイルアップロード" },
			{ "Form/NameTranslationSetting/", "EC > データ管理 > 名称翻訳設定" },
			{ "Form/NameTranslationSettingDownLoad/", "EC > データ管理 > 名称翻訳設定ダウンロード" },
			{ "Form/TwInvoice/", "EC > データ管理 > 電子発票管理" },
			{ "Form/Operator/", "EC > オペレーター管理 > オペレータ情報" },
			{ "Form/OperatorPasswordChange/", "EC > オペレーター管理 > パスワード変更" },
			{ "Form/MenuAuthority/", "EC > オペレーター管理 > メニュー権限設定" },
			{ "Form/Common/", "EC > 共通処理" },
			{ "Form/Download/", "EC > ダウンロード > ダウンロード待機" },
			{ "Form/WysiwygEditor/", "EC > HTMLエディタ" },
			{ "Form/Pri/", "EC > サポート&サービス > サポートサイト" },
			{ "Form/StorePickUp/", "EC > 店舗スタッフ" },
			{ "Form/System/SiteConfiguration", "EC > システム管理 > サイト設定" },
			{ "Form/System/BatchManager", "EC > システム管理 > バッチ管理" },
		};
		/// <summary>MPのURL参照してメニュー名へ変換するリスト。</summary>
		public static readonly Dictionary<string, string> LIST_MENU_NAME_CHANGE_FROM_URL_MP = new Dictionary<string, string>()
		{
			{ "Default.aspx", "MP > ログイン" },
			{ "Form/Error.aspx", "MP > システムエラー" },
			{ "Form/PageIndexList.aspx", "MP > 機能一覧" },
			{ "Form/SingleSignOn.aspx", "MP > シングルサインオン" },
			{ "Form/Support/", "MP > サポート＆サービス > サポートサイト" },
			{ "Form/UserCoupon/", "MP > クーポン管理 > ユーザークーポン情報" },
			{ "Form/Coupon/", "MP > クーポン管理 > クーポン設定" },
			{ "Form/CouponTransitionReport/", "MP > クーポン管理 > クーポン推移レポート" },
			{ "Form/CouponSchedule/", "MP > クーポン管理 > クーポン発行スケジュール設定" },
			{ "Form/UserPoint/", "MP > ポイント管理 > ユーザーポイント情報" },
			{ "Form/PointRule/", "MP > ポイント管理 > 基本ルール設定" },
			{ "Form/PointRuleCampaign/", "MP > ポイント管理 > キャンペーン設定" },
			{ "Form/PointReport/", "MP > ポイント管理 > ポイント最新レポート" },
			{ "Form/PointTransitionReport/", "MP > ポイント管理 > ポイント推移レポート" },
			{ "Form/PointRuleSchedule/", "MP > ポイント管理 > ポイントルールスケジュール" },
			{ "Form/TargetList/", "MP > ターゲットリスト管理 > ターゲットリスト情報" },
			{ "Form/TargetListMerge/", "MP > ターゲットリスト管理 > ターゲットリストマージ" },
			{ "Form/MailDistSetting/", "MP > メール配信管理 > メール配信設定" },
			{ "Form/MailDistText/", "MP > メール配信管理 > メール配信文章設定" },
			{ "Form/MailClickReport/", "MP > メール配信管理 > メールクリックレポート" },
			{ "Form/MemberRank/", "MP > 会員ランク管理 > 会員ランク設定" },
			{ "Form/MemberRankRule/", "MP > 会員ランク管理 > 会員ランク変動ルール設定" },
			{ "Form/UserMemberRank/", "MP > 会員ランク管理 > 会員ランク更新履歴" },
			{ "Form/TagAuthority/", "MP > タグ管理 > タグ閲覧権限" },
			{ "Form/AffiliateReport/", "MP > タグ管理 > アフィリエイトレポート" },
			{ "Form/AdvertisementCode/", "MP > 広告管理 > 広告コード設定" },
			{ "Form/AdvertisementCodeAuthority/", "MP > 広告管理 > 広告コード閲覧権限" },
			{ "Form/AdvertisementCodeReport/", "MP > 広告管理 > 広告コードレポート" },
			{ "Form/AccessReport/", "MP > 基本分析レポート > アクセスレポート" },
			{ "Form/AccessRanking/", "MP > 基本分析レポート > アクセスランキング" },
			{ "Form/UserConditionReport/", "MP > 基本分析レポート > ユーザー状況レポート" },
			{ "Form/UserKbnReport/", "MP > 基本分析レポート > ユーザー区分レポート" },
			{ "Form/CpmReport/", "MP > 基本分析レポート > CPMレポート" },
			{ "Form/RecommendReport/", "MP > 基本分析レポート > レコメンドレポート" },
			{ "Form/ProductRanking/", "MP > 基本分析レポート > 商品ランキング" },
			{ "Form/OrderKbnReport/", "MP > 基本分析レポート > 購買区分レポート" },
			{ "Form/FixedPurchaseKbnReport/", "MP > 基本分析レポート > 定期区分レポート" },
			{ "Form/OrderRepeatReport/", "MP > 基本分析レポート > 定期回数別レポート" },
			{ "Form/FixedPurchaseRepeatAnalysisReport/", "MP > 基本分析レポート > 定期継続分析レポート" },
			{ "Form/FixedPurchaseForecastReport/", "MP > 基本分析レポート > 定期売上予測レポート" },
			{ "Form/OrderConditionReport/", "MP > 基本分析レポート > 売上状況レポート" },
			{ "Form/SilvereggAigentReport/", "MP > 基本分析レポート > レコメンドレポート" },
			{ "Form/ShipmentForecastByDays/", "MP > 基本分析レポート > 日別出荷予測レポート" },
			{ "Form/MasterImport/", "MP > データ管理 > マスタアップロード" },
			{ "Form/MasterExportSetting/", "MP > データ管理 > マスタダウンロード設定" },
			{ "Form/Operator/", "MP > オペレーター管理 > オペレータ情報" },
			{ "Form/OperatorPasswordChange/", "MP > オペレーター管理 > パスワード変更" },
			{ "Form/MenuAuthority/", "MP > オペレーター管理 > メニュー権限設定" },
			{ "Form/Common/", "MP > 共通処理" },
		};
		/// <summary>CSのURL参照してメニュー名へ変換するリスト。</summary>
		public static readonly Dictionary<string, string> LIST_MENU_NAME_CHANGE_FROM_URL_CS = new Dictionary<string, string>()
		{
			{ "Default.aspx", "CS > ログイン" },
			{ "Form/Error.aspx", "CS > システムエラー" },
			{ "Form/PageIndexList.aspx", "CS > 機能一覧" },
			{ "Form/SingleSignOn.aspx", "CS > シングルサインオン" },
			{ "Form/Support/", "CS > サポートサイト > サポート情報" },
			{ "Form/Top/", "CS > オペレーション > トップページ" },
			{ "Form/Message/", "CS > メッセージ" },
			{ "Form/ShareInfo/", "CS > 共有情報" },
			{ "Form/IncidentCategory/", "CS > インシデント設定 > インシデントカテゴリ設定" },
			{ "Form/IncidentVoc/", "CS > インシデント設定 > VOC区分設定" },
			{ "Form/SummarySetting/", "CS > インシデント設定 > 集計区分設定" },
			{ "Form/AnswerTemplateCategory/", "CS > オペレーター支援 > 回答例カテゴリ設定" },
			{ "Form/AnswerTemplate/", "CS > オペレーター支援 > 回答例文章設定" },
			{ "Form/ShareInfoSetting/", "CS > オペレーター支援 > 共有情報" },
			{ "Form/ExternalLinkPreference/", "CS > オペレーター支援 > 外部リンク設定" },
			{ "Form/ReportIncident/", "CS > 集計レポート > インシデント集計" },
			{ "Form/ReportMessage/", "CS > 集計レポート > メッセージ集計" },
			{ "Form/ReportCsWorkflow/", "CS > 集計レポート > 業務フロー集計" },
			{ "Form/MailAssignSetting/", "CS > メール設定 > 受信時振分けルール" },
			{ "Form/MailSignature/", "CS > メール設定 > メール署名管理" },
			{ "Form/MailFrom/", "CS > メール設定 > メール送信元管理" },
			{ "Form/DataMigration/", "CS > データ管理 > データ移行管理" },
			{ "Form/Operator/", "CS > オペレーター管理 > オペレータ情報" },
			{ "Form/OperatorAuthority/", "CS > オペレーター管理 > オペレータ権限情報" },
			{ "Form/MenuAuthority/", "CS > オペレーター管理 > メニュー権限設定" },
			{ "Form/OperatorPasswordChange/", "CS > オペレーター管理 > パスワード変更" },
			{ "Form/CsGroup/", "CS > 拠点管理 > 拠点グループ設定" },
			{ "Form/CsOperatorGroup/", "CS > 拠点管理 > オペレータ所属設定" },
			{ "Form/WysiwygEditor/", "CS > HTMLエディタ" },
			{ "Form/Incident/", "CS > インシデント" },
		};
		/// <summary>CMSのURL参照してメニュー名へ変換するリスト。</summary>
		public static readonly Dictionary<string, string> LIST_MENU_NAME_CHANGE_FROM_URL_CMS = new Dictionary<string, string>()
		{
			{ "Error", "CMS > システムエラー" },
			{ "PageIndexList", "CMS > 機能一覧" },
			{ "SingleSignOn", "CMS > シングルサインオン" },
			{ "SupportInformation", "CMS > サポート＆サービス > サポートサイト" },
			{ "CoordinatePage/", "CMS > コーディネート管理 > コーディネート情報" },
			{ "CoordinateCategory/", "CMS > コーディネート管理 > コーディネートカテゴリ設定" },
			{ "FeaturePage/", "CMS > 特集管理 > 特集ページ情報" },
			{ "FeaturePageCategory/", "CMS > 特集管理 > 特集ページカテゴリ設定" },
			{ "FeatureArea/", "CMS > 特集管理 > 特集エリア情報" },
			{ "FeatureAreaType/", "CMS > 特集管理 > 特集エリアタイプ設定" },
			{ "LandingPage/", "CMS > LP管理 > LPビルダー" },
			{ "AbTest/", "CMS > LP管理 > ABテスト" },
			{ "ScoringSale/", "CMS > スコアリング販売管理 > スコアリング販売ビルダー" },
			{ "ScoringSaleQuestion/", "CMS > スコアリング販売管理 > スコアリング質問設定" },
			{ "ScoreAxis/", "CMS > スコアリング販売管理 > スコア軸設定" },
			{ "ProductRanking/", "CMS > 商品表示管理 > 商品ランキング設定" },
			{ "ProductGroup/", "CMS > 商品表示管理 > 商品グループ設定" },
			{ "ProductListDispSetting/", "CMS > 商品表示管理 > 商品一覧表示設定" },
			{ "PageDesign/", "CMS > デザイン管理 > ページ管理" },
			{ "PartsDesign/", "CMS > デザイン管理 > パーツ管理" },
			{ "CSSDesign/", "CMS > デザイン管理 > CSSデザイン設定" },
			{ "JavaScriptDesign/", "CMS > デザイン管理 > JavaScriptデザイン設定" },
			{ "ContentsManager/", "CMS > デザイン管理 > コンテンツマネージャー" },
			{ "FeatureImage/", "CMS > デザイン管理 > コンテンツ画像管理" },
			{ "SeoMetadatas/", "CMS > SEO管理 > SEOタグ設定" },
			{ "OgpTagSetting/", "CMS > SEO管理 > OGPタグ設定" },
			{ "Sitemap/", "CMS > SEO管理 > サイトマップ設定" },
			{ "SiteInformation/", "CMS > サイト管理 > サイト基本情報設定" },
			{ "News/", "CMS > サイト管理 > お知らせ設定" },
			{ "ShortUrl/", "CMS > サイト管理 > ショートURL設定" },
			{ "TagManager/", "CMS > サイト管理 > タグマネージャー" },
			{ "MasterImport/", "CMS > データ管理 > マスタアップロード" },
			{ "MasterExportSetting/", "CMS > データ管理 > マスタダウンロード設定" },
			{ "ShopOperator/", "CMS > オペレーター管理 > オペレータ情報" },
			{ "Staff/", "CMS > オペレーター管理 > スタッフ情報" },
			{ "OperatorPasswordChange/", "CMS > オペレーター管理 > パスワード変更" },
			{ "MenuAuthority/", "CMS > オペレーター管理 > メニュー権限設定" },
			{ "_ProductTagManager/", "CMS > 商品タグ一覧・登録・更新・削除ページ" },
			{ "_SearchPopup/", "CMS > ポップアップウィンドウ" },
			{ "_WysiwygEditor/", "CMS > HTMLエディタ" },
			{ "Shared/", "CMS > 共通処理" },
			{ "Login/", "CMS > ログイン" },
			{ "", "CMS > ログイン" },
		};
	}
}
