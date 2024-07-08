/*
=========================================================================================================
  Module      : ヤマトKWC コンテンツクラス(PaymentYamatoKwcConstants.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC コンテンツクラス
	/// </summary>
	class PaymentYamatoKwcConstants
	{
		/// <summary>結果コード</summary>
		public const string PARAM_RETURN_CODE = "returnCode";
		/// <summary>エラーコード</summary>
		public const string PARAM_ERROR_CODE = "errorCode";
		/// <summary>送信日時</summary>
		public const string PARAM_RETURN_DATE = "returnDate";
		/// <summary>与信承認番号</summary>
		public const string PARAM_CRD_C_RES_CD = "crdCResCd";
		/// <summary>与信結果エラーコード</summary>
		public const string PARAM_CREDIT_ERROR_CODE = "creditErrorCode";
		/// <summary>3D 認証リダイレクト HTML</summary>
		public const string PARAM_THREE_D_AUTH_HTML = "threeDAuthHtml";
		/// <summary>3D トークン</summary>
		public const string PARAM_THREE_D_TOKEN = "threeDToken";

		#region トークン決済登録３Ｄセキュア結果用パラメータ(A09)
		/// <summary>リクエストパラメータ(A09)：被仕向会社コード</summary>
		public const string REQUEST_PARAM_A09_FUNCTION_DIV = "function_div";
		/// <summary>リクエストパラメータ(A09)：トークン</summary>
		public const string REQUEST_PARAM_A09_TRADER_CODE = "trader_code";
		/// <summary>リクエストパラメータ(A09)：カード有効期限</summary>
		public const string REQUEST_PARAM_A09_ORDER_NO = "order_no";
		/// <summary>リクエストパラメータ(A09)：被仕向会社コード</summary>
		public const string REQUEST_PARAM_A09_COMP_CD = "comp_cd";
		/// <summary>リクエストパラメータ(A09)：トークン</summary>
		public const string REQUEST_PARAM_A09_TOKEN = "token";
		/// <summary>リクエストパラメータ(A09)：カード有効期限</summary>
		public const string REQUEST_PARAM_A09_CARD_EXP = "card_exp";
		/// <summary>リクエストパラメータ(A09)：商品金額</summary>
		public const string REQUEST_PARAM_A09_ITEM_PRICE = "item_price";
		/// <summary>リクエストパラメータ(A09)：税・送料 </summary>
		public const string REQUEST_PARAM_A09_ITEM_TAX = "item_tax";
		/// <summary>リクエストパラメータ(A09)：顧客番号 </summary>
		public const string REQUEST_PARAM_A09_CUST_CD = "cust_cd";
		/// <summary>リクエストパラメータ(A09)：店舗ID </summary>
		public const string REQUEST_PARAM_A09_SHOP_ID = "shop_id";
		/// <summary>リクエストパラメータ(A09)：端末識別番号</summary>
		public const string REQUEST_PARAM_A09_TERM_CD = "term_cd";
		/// <summary>リクエストパラメータ(A09)：結果コード</summary>
		public const string REQUEST_PARAM_A09_RES_CD = "crd_res_cd";
		/// <summary>リクエストパラメータ(A09)：VERes 戻り値</summary>
		public const string REQUEST_PARAM_A09_RES_VE = "res_ve";
		/// <summary>リクエストパラメータ(A09)：PARes 戻り値</summary>
		public const string REQUEST_PARAM_A09_RES_PA = "res_pa";
		/// <summary>リクエストパラメータ(A09)：詳細コード</summary>
		public const string REQUEST_PARAM_A09_RES_CODE = "res_code";
		/// <summary>リクエストパラメータ(A09)：3D 認証情報</summary>
		public const string REQUEST_PARAM_A09_THREE_D_INF = "three_d_inf";
		/// <summary>リクエストパラメータ(A09)：3D トランザクションID</summary>
		public const string REQUEST_PARAM_A09_THREE_D_TRAN_ID = "three_d_tran_id";
		/// <summary>リクエストパラメータ(A09)：電文送信時間</summary>
		public const string REQUEST_PARAM_A09_SEND_DT = "send_dt";
		/// <summary>リクエストパラメータ(A09)：HASH値</summary>
		public const string REQUEST_PARAM_A09_HASH_VALUE = "hash_value";
		/// <summary>リクエストパラメータ(A09)：3D トークン</summary>
		public const string REQUEST_PARAM_A09_THREE_D_TOKEN = "three_d_token";
		/// <summary>被仕向会社コード</summary>
		public const string RESPONSE_PARAM_3D_SECURE_COMP_CD = "COMP_CD";
		/// <summary>トークン</summary>
		public const string RESPONSE_PARAM_3D_SECURE_TOKEN = "TOKEN";
		/// <summary>カード有効期限</summary>
		public const string RESPONSE_PARAM_3D_SECURE_CARD_EXP = "CARD_EXP";
		/// <summary>商品金額</summary>
		public const string RESPONSE_PARAM_3D_SECURE_ITEM_PRICE = "ITEM_PRICE";
		/// <summary>税・送料 </summary>
		public const string RESPONSE_PARAM_3D_SECURE_ITEM_TAX = "ITEM_TAX";
		/// <summary>顧客番号 </summary>
		public const string RESPONSE_PARAM_3D_SECURE_CUST_CD = "CUST_CD";
		/// <summary>店舗 ID </summary>
		public const string RESPONSE_PARAM_3D_SECURE_SHOP_ID = "SHOP_ID";
		/// <summary>端末識別番号</summary>
		public const string RESPONSE_PARAM_3D_SECURE_TERM_CD = "TERM_CD";
		/// <summary>結果コード</summary>
		public const string RESPONSE_PARAM_3D_SECURE_CRD_RES_CD = "CRD_RES_CD";
		/// <summary>VERes 戻り値</summary>
		public const string RESPONSE_PARAM_3D_SECURE_RES_VE = "RES_VE";
		/// <summary>PARes 戻り値</summary>
		public const string RESPONSE_PARAM_3D_SECURE_RES_PA = "RES_PA";
		/// <summary>詳細コード</summary>
		public const string RESPONSE_PARAM_3D_SECURE_RES_CODE = "RES_CODE";
		/// <summary>3D 認証情報</summary>
		public const string RESPONSE_PARAM_3D_SECURE_3D_INF = "3D_INF";
		/// <summary>3D トランザクションID</summary>
		public const string RESPONSE_PARAM_3D_SECURE_3D_TRAN_ID = "3D_TRAN_ID";
		/// <summary>電文送信時間</summary>
		public const string RESPONSE_PARAM_3D_SECURE_SEND_DT = "SEND_DT";
		/// <summary>HASH 値</summary>
		public const string RESPONSE_PARAM_3D_SECURE_HASH_VALUE = "HASH_VALUE";
		#endregion

		/// <summary>セキュリティコード無しの認証区分：0</summary>
		public static string FLG_AUTHENTICATION_DIVISION_WITHOUT_SECURITY_CODE = "0";
		/// <summary>セキュリティコード有の認証区分：2</summary>
		public static string FLG_AUTHENTICATION_DIVISION_WITH_SECURITY_CODE = "2";
		/// <summary>3Dセキュアの認証区分：3</summary>
		public static string FLG_AUTHENTICATION_DIVISION_THREE_D_SECURITY_CODE = "3";
	}
}
