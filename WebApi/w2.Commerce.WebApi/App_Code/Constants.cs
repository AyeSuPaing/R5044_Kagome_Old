/*
=========================================================================================================
  Module      : WebApi定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

/// <summary>
///  WebApi定数定義
/// </summary>
public class Constants : w2.App.Common.Constants
{
	/// <summary>Id Type: EC User Id</summary>
	public const string ID_TYPE_EC_USER_ID = "0";
	/// <summary>Id Type: LINE User Id</summary>
	public const string ID_TYPE_LINE_USER_ID = "1";
	/// <summary>Id Type: Regist LINE User Id</summary>
	public const string ID_TYPE_REGISRT_LINE_ID = "2";
	/// <summary>Request Key Id Type</summary>
	public const string REQUEST_KEY_ID_TYPE = "id_type";
	/// <summary>Request Key Limit</summary>
	public const string REQUEST_KEY_LIMIT = "limit";
	/// <summary>Request Key Offset</summary>
	public const string REQUEST_KEY_OFFSET = "offset";
	/// <summary>Request Key Sort Type</summary>
	public const string REQUEST_KEY_SORT_TYPE = "sort_type";
	/// <summary>Request Key Update At</summary>
	public const string REQUEST_KEY_UPDATE_AT = "updated_at";
	/// <summary>Request Key Sort Type ASC</summary>
	public const string REQUEST_KEY_SORT_TYPE_ASC = "asc";
	/// <summary>Request Key Sort Type DESC</summary>
	public const string REQUEST_KEY_SORT_TYPE_DESC = "desc";
	/// <summary>Request Key Name</summary>
	public const string REQUEST_KEY_NAME = "name";
	/// <summary>Request Key Name Kana</summary>
	public const string REQUEST_KEY_NAME_KANA = "name_kana";
	/// <summary>Request Key Tel</summary>
	public const string REQUEST_KEY_TEL = "tel";
	/// <summary>Request Key Mail Id</summary>
	public const string REQUEST_KEY_MAIL_ID = "mail_id";
	/// <summary>Request Key Line Id</summary>
	public const string REQUEST_KEY_LINE_ID = "line_id";
	/// <summary>BOTCHAN_リクエストタイプ</summary>
	public const string BOTCHAN_RESPONSE_CONTENTTYPE = "application/json";
	/// <summary>BOTCHAN_エラーメッセージ区切り</summary>
	public const string BOTCHAN_ERROR_SPLIT = "|";
	/// <summary>Request Key Product Id</summary>
	public new const string REQUEST_KEY_PRODUCT_ID = "product_id";
	/// <summary>Check Kbn BotChan Login</summary>
	public const string CHECK_KBN_BOTCHAN_LOGIN = "BotChanLogin";
	/// <summary>Check Kbn BotChan Product Search</summary>
	public const string CHECK_KBN_BOTCHAN_PRODUCT_SEARCH = "BotChanProductSearch";
	/// <summary>Check Kbn BotChan Order Register</summary>
	public const string CHECK_KBN_BOTCHAN_ORDER_REGISTER = "BotChanOrderRegister";
	/// <summary>Check Kbn BotChan Recalculation</summary>
	public const string CHECK_KBN_BOTCHAN_RECALCULATION = "BotChanRecalculation";
	/// <summary>会員登録バリデーション区分</summary>
	public const string CHECK_KBN_BOTCHAN_USER_REGISTER = "BotChanUserRegister";
	/// <summary>Request Key Mail Address</summary>
	public const string REQUEST_KEY_MAIL_ADDRESS = "mail_address";
	/// <summary>Request Key Cart Id</summary>
	public new const string REQUEST_KEY_CART_ID = "cart_id";
	/// <summary>Request Key User Id</summary>
	public new const string REQUEST_KEY_USER_ID = "user_id";
	/// <summary>Request Key Order Division</summary>
	public const string REQUEST_KEY_ORDER_DIVISION = "order_division";
	/// <summary>Request Key Order Division</summary>
	public const string REQUEST_KEY_ORDER_KBN = "order_kbn";
	/// <summary>Request Key Add Novelty Flag</summary>
	public const string REQUEST_KEY_ADD_NOVELTY_FLAG = "add_novelty_flag";
	/// <summary>Request Key Order Shipping Info</summary>
	public const string REQUEST_KEY_ORDER_SHIPPING_INFO = "order_shipping_info";
	/// <summary>Request Key Shipping Name</summary>
	public const string REQUEST_KEY_SHIPPING_NAME = "shipping_name";
	/// <summary>Request Key Shipping Name1（姓）</summary>
	public const string REQUEST_KEY_SHIPPING_NAME1 = "shipping_name1";
	/// <summary>Request Key Shipping Name2（名）</summary>
	public const string REQUEST_KEY_SHIPPING_NAME2 = "shipping_name2";
	/// <summary>Request Key Shipping Name Kana</summary>
	public const string REQUEST_KEY_SHIPPING_NAME_KANA = "shipping_name_kana";
	/// <summary>Request Key Shipping Name Kana1（姓）</summary>
	public const string REQUEST_KEY_SHIPPING_NAME_KANA1 = "shipping_name_kana1";
	/// <summary>Request Key Shipping Name Kana2（名）</summary>
	public const string REQUEST_KEY_SHIPPING_NAME_KANA2 = "shipping_name_kana2";
	/// <summary>Request Key Owner Sex</summary>
	public const string REQUEST_KEY_OWNER_SEX = "owner_sex";
	/// <summary>Request Key Owner Birth</summary>
	public const string REQUEST_KEY_OWNER_BIRTH = "owner_birth";
	/// <summary>Request Key Shipping_Zip</summary>
	public const string REQUEST_KEY_SHIPPING_ZIP = "shipping_zip";
	/// <summary>Request Key Shipping Addr1</summary>
	public const string REQUEST_KEY_SHIPPING_ADDR1 = "shipping_addr1";
	/// <summary>Request Key Shipping Addr2</summary>
	public const string REQUEST_KEY_SHIPPING_ADDR2 = "shipping_addr2";
	/// <summary>Request Key Shipping Addr3</summary>
	public const string REQUEST_KEY_SHIPPING_ADDR3 = "shipping_addr3";
	/// <summary>Request Key Shipping Addr4</summary>
	public const string REQUEST_KEY_SHIPPING_ADDR4 = "shipping_addr4";
	/// <summary>Request Key Shipping Tel1</summary>
	public const string REQUEST_KEY_SHIPPING_TEL1 = "shipping_tel1";
	/// <summary>Request Key Shipping Company Name</summary>
	public const string REQUEST_KEY_SHIPPING_COMPANY_NAME = "shipping_company_name";
	/// <summary>Request Key Shipping Company Post Name</summary>
	public const string REQUEST_KEY_SHIPPING_COMPANY_POST_NAME = "shipping_company_post_name";
	/// <summary>Request Key Fixed Purchase Kbn</summary>
	public new const string REQUEST_KEY_FIXED_PURCHASE_KBN = "fixed_purchase_kbn";
	/// <summary>Request Key Course Buy Setting</summary>
	public const string REQUEST_KEY_COURSE_BUY_SETTING = "course_buy_setting";
	/// <summary>Request Key  Oder Payment Info</summary>
	public const string REQUEST_KEY_ORDER_PAYMENT_INFO = "order_payment_info";
	/// <summary>Request Key Payment Id</summary>
	public const string REQUEST_KEY_PAYMENT_ID = "payment_id";
	/// <summary>Request Key Receipt Flg</summary>
	public const string REQUEST_KEY_RECEIPT_FLG = "receipt_flg";
	/// <summary>Request Key Card Disp Name</summary>
	public const string REQUEST_KEY_CARD_DISP_NAME = "card_disp_name";
	/// <summary>Request Key Credit Token</summary>
	public const string REQUEST_KEY_CREDIT_TOKEN = "credit_token";
	/// <summary>Request Key Credit Card No</summary>
	public const string REQUEST_KEY_CREDIT_CARD_NO = "credit_card_no";
	/// <summary>Request Key Expiration Month</summary>
	public const string REQUEST_KEY_EXPIRATION_MONTH = "expiration_month";
	/// <summary>Request Key Expiration Year</summary>
	public const string REQUEST_KEY_EXPIRATION_YEAR = "expiration_year";
	/// <summary>Request Key Author Name</summary>
	public const string REQUEST_KEY_AUTHOR_NAME = "author_name";
	/// <summary>Request Key Credit Security Code</summary>
	public const string REQUEST_KEY_CREDIT_SECURITY_CODE = "credit_security_code";
	/// <summary>Request Key Credit Installments</summary>
	public const string REQUEST_KEY_CREDIT_INSTALLMENTS = "credit_installments";
	/// <summary>クレジットカード登録フラグ</summary>
	public const string REQUEST_KEY_CREDIT_REGIST_FLAG = "credit_regist_flag";
	/// <summary>クレジットカード登録名</summary>
	public const string REQUEST_KEY_CREDIT_REGIST_NAME = "credit_regist_name";
	/// <summary>Request Key Discount Info</summary>
	public const string REQUEST_KEY_DISCOUNT_INFO = "discount_info";
	/// <summary>Request Key Order Point Use</summary>
	public const string REQUEST_KEY_ORDER_POINT_USE = "order_point_use";
	/// <summary>Request Key Coupon Code</summary>
	public const string REQUEST_KEY_COUPON_CODE = "coupon_code";
	/// <summary>Request Key Order Product List</summary>
	public const string REQUEST_KEY_ORDER_PRODUCT_LIST = "order_product_list";
	/// <summary>Request key users</summary>
	public const string REQUEST_KEY_USERS = "users";

	/// <summary>Request Key Variation Id</summary>
	public new const string REQUEST_KEY_VARIATION_ID = "variation_id";
	/// <summary>Request Key Product Count</summary>
	public new const string REQUEST_KEY_PRODUCT_COUNT = "product_count";
	/// <summary>Request Key Recommend Flag</summary>
	public const string REQUEST_KEY_RECOMMEND_FLAG = "recommend_flag";
	/// <summary>注文後レコメンド実施フラグ</summary>
	public const string REQUEST_KEY_AFTER_ORDER_RECOMMEND_FLAG = "after_order_recommend_flag";
	/// <summary>元注文ID</summary>
	public const string REQUEST_KEY_OLD_ORDER_ID = "old_order_id";
	/// <summary>広告コード</summary>
	public const string REQUEST_KEY_ADV_CODE = "adv_code";
	/// <summary>Request Key Product Option Texts</summary>
	public const string REQUEST_KEY_PRODUCT_OPTION_TEXTS = "product_option_texts";
	/// <summary>Request Key Auth Text</summary>
	public const string REQUEST_KEY_AUTH_TEXT = "auth_text";
	/// <summary>Request Key User Ip Address</summary>
	public const string REQUEST_KEY_USER_IP_ADDRESS = "user_ip_address";

	/// <summary>氏名1</summary>
	public const string REQUEST_KEY_ORDER_OWNER_NAME1 = "name1";
	/// <summary>氏名2</summary>
	public const string REQUEST_KEY_ORDER_OWNER_NAME2 = "name2";
	/// <summary>氏名かな1</summary>
	public const string REQUEST_KEY_ORDER_OWNER_NAME_KANA1 = "name_kana1";
	/// <summary>氏名かな2</summary>
	public const string REQUEST_KEY_ORDER_OWNER_NAME_KANA2 = "name_kana2";
	/// <summary>生年月日</summary>
	public const string REQUEST_KEY_ORDER_OWNER_BIRTH = "birth";
	/// <summary>性別</summary>
	public const string REQUEST_KEY_ORDER_OWNER_SEX = "sex";
	/// <summary>メールアドレス</summary>
	public const string REQUEST_KEY_ORDER_MAIL_ADDR = "mail_addr";
	/// <summary>郵便番号</summary>
	public const string REQUEST_KEY_ORDER_ZIP = "zip";
	/// <summary>住所1</summary>
	public const string REQUEST_KEY_ORDER_ADDR1 = "addr1";
	/// <summary>住所2</summary>
	public const string REQUEST_KEY_ORDER_ADDR2 = "addr2";
	/// <summary>住所3</summary>
	public const string REQUEST_KEY_ORDER_ADDR3 = "addr3";
	/// <summary>住所4</summary>
	public const string REQUEST_KEY_ORDER_ADDR4 = "addr4";
	/// <summary>電話番号1</summary>
	public const string REQUEST_KEY_ORDER_TEL1 = "tel1";
	/// <summary>配送方法</summary>
	public const string REQUEST_KEY_SHIPPING_METHOD = "shipping_method";

	/// <summary>氏名1</summary>
	public const string REQUEST_KEY_USER_REGISTER_NAME1 = "name1";
	/// <summary>氏名2</summary>
	public const string REQUEST_KEY_USER_REGISTER_NAME2 = "name2";
	/// <summary>氏名かな1</summary>
	public const string REQUEST_KEY_USER_REGISTER_NAME_KANA1 = "name_kana1";
	/// <summary>氏名かな2</summary>
	public const string REQUEST_KEY_USER_REGISTER_NAME_KANA2 = "name_kana2";
	/// <summary>顧客区分</summary>
	public const string REQUEST_KEY_USER_REGISTER_USER_KBN = "user_kbn";
	/// <summary>生年月日</summary>
	public const string REQUEST_KEY_USER_REGISTER_BIRTH = "birth";
	/// <summary>性別</summary>
	public const string REQUEST_KEY_USER_REGISTER_SEX = "sex";
	/// <summary>メールアドレス</summary>
	public const string REQUEST_KEY_USER_REGISTER_MAIL_ADDR = "mail_addr";
	/// <summary>郵便番号</summary>
	public const string REQUEST_KEY_USER_REGISTER_ZIP = "zip";
	/// <summary>住所1</summary>
	public const string REQUEST_KEY_USER_REGISTER_ADDR1 = "addr1";
	/// <summary>住所2</summary>
	public const string REQUEST_KEY_USER_REGISTER_ADDR2 = "addr2";
	/// <summary>住所3</summary>
	public const string REQUEST_KEY_USER_REGISTER_ADDR3 = "addr3";
	/// <summary>住所4</summary>
	public const string REQUEST_KEY_USER_REGISTER_ADDR4 = "addr4";
	/// <summary>電話番号1</summary>
	public const string REQUEST_KEY_USER_REGISTER_TEL1 = "tel1";
	/// <summary>パスワード</summary>
	public const string REQUEST_KEY_USER_REGISTER_PASSWORD = "password";
	/// <summary>メール配信フラグ</summary>
	public const string REQUEST_KEY_USER_REGISTER_MAIL_FLG = "mail_flg";

	/// <summary>注文後レコメンド_実施しない</summary>
	public const string FLAG_AFTER_ORDER_RECOMMEND_NONE = "0";
	/// <summary>注文後レコメンド_予定</summary>
	public const string FLAG_AFTER_ORDER_RECOMMEND_PLAN = "1";
	/// <summary>注文後レコメンド_確定</summary>
	public const string FLAG_AFTER_ORDER_RECOMMEND_CONFIRMATION = "2";
	/// <summary>注文種類：通常</summary>
	public const string FLAG_ORDER_DIVISION_NORMAL = "1";
	/// <summary>注文種類：定期</summary>
	public const string FLAG_ORDER_DIVISION_FIXED = "2";
}
