/*
=========================================================================================================
  Module      : Atobaraicom Constants (AtobaraicomConstants.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.Atobaraicom
{
	/// <summary>
	/// Atobaraicom constants
	/// </summary>
	public class AtobaraicomConstants
	{
		#region 基本情報
		/// <summary>注文日</summary>
		public const string ATOBARAICOM_API_REGIST_ORDER_DATE = "O_ReceiptOrderDate";
		/// <summary>加盟店ID</summary>
		public const string ATOBARAICOM_API_REGIST_RECEPTIONIST_ID = "O_EnterpriseId";
		/// <summary>受付サイトID</summary>
		public const string ATOBARAICOM_API_REGIST_RECEPTION_SITE_ID = "O_SiteId";
		/// <summary>APIユーザーID</summary>
		public const string ATOBARAICOM_API_REGIST_API_USER_ID = "O_ApiUserId";
		/// <summary>任意注文番号</summary>
		public const string ATOBARAICOM_API_REGIST_OPTIONAL_ORDER_NUMBER = "O_Ent_OrderId";
		/// <summary>エンタープライズノート</summary>
		public const string ATOBARAICOM_API_REGIST_REMARKS_MEMO = "O_Ent_Note";
		/// <summary>使用量</summary>
		public const string ATOBARAICOM_API_REGIST_TOTAL_BILLED_AMOUNT = "O_UseAmount";
		/// <summary>OEM用任意注文番号</summary>
		public const string ATOBARAICOM_API_REGIST_OPTIONAL_ORDER_NUMBER_OEM = "O_OEM_OrderId";
		#endregion

		#region 注文（請求先住所）情報
		/// <summary>郵便番号</summary>
		public const string ATOBARAICOM_API_REGIST_POSTAL_CODE_ORDER = "C_PostalCode";
		/// <summary>住所</summary>
		public const string ATOBARAICOM_API_REGIST_ADDRESS_ORDER = "C_UnitingAddress";
		/// <summary>氏名</summary>
		public const string ATOBARAICOM_API_REGIST_NAME_ORDER = "C_NameKj";
		/// <summary>氏名カナ</summary>
		public const string ATOBARAICOM_API_REGIST_NAME_ORDER_KANA = "C_NameKn";
		/// <summary>お電話番号</summary>
		public const string ATOBARAICOM_API_REGIST_PHONE_NUMBER_ORDER = "C_Phone";
		/// <summary>メールアドレス</summary>
		public const string ATOBARAICOM_API_REGIST_MAIL_ADDRESS = "C_MailAddress";
		/// <summary>職業</summary>
		public const string ATOBARAICOM_API_REGIST_OCCUPATION = "C_Occupdation";
		#endregion

		#region 個別の配送情報
		/// <summary>請求書別送</summary>
		public const string ATOBARAICOM_API_REGIST_SPEC_DELIVERY_DESTINATION = "O_AnotherDeliFlg";
		/// <summary>請求書別送</summary>
		public const string ATOBARAICOM_API_REGIST_POSTAL_CODE_SHIPPING = "D_PostalCode";
		/// <summary>請求書別送</summary>
		public const string ATOBARAICOM_API_REGIST_ADDRESS_SHIPING = "D_UnitingAddress";
		/// <summary>氏名</summary>
		public const string ATOBARAICOM_API_REGIST_NAME_SHIPPING = "D_DestNameKj";
		/// <summary>氏名</summary>
		public const string ATOBARAICOM_API_REGIST_NAME_SHIPPING_KANA = "D_DestNameKn";
		/// <summary>氏名</summary>
		public const string ATOBARAICOM_API_REGIST_PHONE_NUMBER_SHIPPING = "D_Phone";
		#endregion

		#region 商品情報
		/// <summary>購入品目</summary>
		public const string ATOBARAICOM_API_REGIST_PURCHASED_ITEM = "I_ItemNameKj_";
		/// <summary>単価</summary>
		public const string ATOBARAICOM_API_REGIST_UNIT = "I_UnitPrice_";
		/// <summary>数量</summary>
		public const string ATOBARAICOM_API_REGIST_QUANTITY = "I_ItemNum_";
		/// <summary>消費税率</summary>
		public const string ATOBARAICOM_API_REGIST_COMSUMPTION_TAX_RATE = "I_TaxRate_";
		/// <summary>商品送料</summary>
		public const string ATOBARAICOM_API_REGIST_PRODUCT_SHIPPING = "I_ItemCarriage";
		/// <summary>店舗手数料</summary>
		public const string ATOBARAICOM_API_REGIST_STORE_FEES = "I_ItemCharge";
		#endregion

		#region クレジット結果情報
		/// <summary>与信結果情報</summary>
		public const string ATOBARAICOM_API_REGIST_CREDIT_RESULT_INFORMATION = "O_RtOrderStatus";
		#endregion

		#region 新旧のAPI切り替え
		/// <summary>新旧区分</summary>
		public const string ATOBARAICOM_API_REGIST_OLD_NEW_API_SWITCHING = "O_NewSystemFlg";
		#endregion

		#region 基本情報（追加アイテム）
		/// <summary>役務提供予定日</summary>
		public const string ATOBARAICOM_API_REGIST_SCHEDULED_DATE = "O_ServicesProvidedDate";
		/// <summary>テスト注文区分</summary>
		public const string ATOBARAICOM_API_REGIST_TEST_ORDER_CATEGORY = "O_TestOrderFlg";
		/// <summary>テスト注文自動与信審査区分</summary>
		public const string ATOBARAICOM_API_REGIST_TEST_ORDER_CREDIT = "O_TestCreditResult";
		/// <summary>Atobaraicom Api Regist Ip Address</summary>
		public const string ATOBARAICOM_API_REGIST_IP_ADDRESS = "O_IpAddress";
		#endregion

		#region 注文者（請求書）情報（追加アイテム）
		/// <summary>加盟店顧客番号</summary>
		public const string ATOBARAICOM_API_REGIST_MERCHANT_CUS_NUMBER = "C_EntCustId";
		/// <summary>法人名</summary>
		public const string ATOBARAICOM_API_REGIST_CORPORATE_NAME = "C_CorporateName";
		/// <summary>部署名</summary>
		public const string ATOBARAICOM_API_REGIST_DEPARTMENT_NAME = "C_DivisionName";
		/// <summary>担当者名</summary>
		public const string ATOBARAICOM_API_REGIST_CONTACT_NAME = "C_CpNameKj";
		/// <summary>請求書別送</summary>
		public const string ATOBARAICOM_API_REGIST_SEPARATE_INVOICE = "C_SeparateShipment";
		#endregion

		#region 製品情報（追加アイテム）
		/// <summary>外税</summary>
		public const string ATOBARAICOM_API_REGIST_FOREIGN_TAX_AMOUNT = "I_OutsiteTax";
		#endregion

		#region 後払いキャンセル
		/// <summary>事業者ID</summary>
		public const string ATOBARAICOM_API_CANCEL_BUSINESS_ID = "EnterpriseId";
		/// <summary>APIユーザーID</summary>
		public const string ATOBARAICOM_API_CANCEL_API_USER_ID = "ApiUserId";
		/// <summary>注文ID</summary>
		public const string ATOBARAICOM_API_CANCEL_ORDER_NUMBER_LIST = "OrderId[]";
		/// <summary>理由</summary>
		public const string ATOBARAICOM_API_CANCEL_REASON_CANCELLATION = "Reason[]";
		#endregion

		#region 後払い変更
		/// <summary>事業者ID</summary>
		public const string ATOBARAICOM_API_MODIFY_BUSINESS_ID = "EnterpriseId";
		/// <summary>APIユーザーID</summary>
		public const string ATOBARAICOM_API_MODIFY_API_USER_ID = "ApiUserId";
		/// <summary>注文ID</summary>
		public const string ATOBARAICOM_API_MODIFY_ORDER_ID = "OrderId";
		/// <summary>任意注文番号</summary>
		public const string ATOBARAICOM_API_MODIFY_OPTIONAL_ORDER_NUMBER = "O_Ent_OrderId";
		#endregion

		#region 後払いすべてのアイテムを変更
		#region 基本情報
		/// <summary>事業者ID</summary>
		public const string ATOBARAICOM_API_MODIFY_ALL_MERCHANT_ID = "EnterpriseId";
		/// <summary>APIユーザーID</summary>
		public const string ATOBARAICOM_API_MODIFY_ALL_API_USER_ID = "ApiUserId";
		/// <summary>注文ID</summary>
		public const string ATOBARAICOM_API_MODIFY_ALL_ORDER_ID = "OrderId";
		/// <summary>注文日</summary>
		public const string ATOBARAICOM_API_MODIFY_ALL_ORDER_DAY = "O_ReceiptOrderDate";
		/// <summary>受付サイトID</summary>
		public const string ATOBARAICOM_API_MODIFY_ALL_RECEPTION_SITE_ID = "O_SiteId";
		/// <summary>任意注文番号</summary>
		public const string ATOBARAICOM_API_MODIFY_ALL_OPTIONAL_ORDER_NUMBER = "O_Ent_OrderId";
		/// <summary></summary>
		public const string ATOBARAICOM_API_MODIFY_ALL_REMARKS_MEMO = "O_EntNote";
		/// <summary></summary>
		public const string ATOBARAICOM_API_MODIFY_ALL_TOTAL_BILLED_AMOUNT = "O_UseAmount";
		/// <summary>役務提供予定日</summary>
		public const string ATOBARAICOM_API_MODIFY_ALL_SCHEDULED_DATE_OF_SERVICE_PROVISION = "O_ServicesProvidedDate";
		/// <summary>OEM用任意注文番号</summary>
		public const string ATOBARAICOM_API_MODIFY_ALL_OPTIONAL_ORDER_NUMBER_OEM = "O_OEM_OrderId";
		/// <summary>テスト注文自動与信審査区分</summary>
		public const string ATOBARAICOM_API_MODIFY_ALL_TEST_ORDER_AUTOMATIC_CREDIT_SCREENING = "O_TestCreditResult";
		#endregion

		#region 注文者（請求先住所）情報
		/// <summary>郵便番号</summary>
		public static string ATOBARAICOM_API_MODIFY_ALL_POSTAL_CODE = "C_PostalCode";
		/// <summary>住所</summary>
		public static string ATOBARAICOM_API_MODIFY_ALL_ADDRESS = "C_UnitingAddress";
		/// <summary>氏名</summary>
		public static string ATOBARAICOM_API_MODIFY_ALL_ORDER_NAME = "C_NameKj";
		/// <summary>氏名カナ</summary>
		public static string ATOBARAICOM_API_MODIFY_ALL_NAME_KANA = "C_NameKn";
		/// <summary>お電話番号</summary>
		public static string ATOBARAICOM_API_MODIFY_ALL_TEL_NO = "C_Phone";
		/// <summary>メールアドレス</summary>
		public static string ATOBARAICOM_API_MODIFY_ALL_MAIL_ADDRESS = "C_MailAddress";
		/// <summary>職業</summary>
		public static string ATOBARAICOM_API_MODIFY_ALL_OCCUPATION = "C_Occupation";
		/// <summary>加盟店顧客番号</summary>
		public static string ATOBARAICOM_API_MODIFY_ALL_MERCHANT_CUS_NUMER = "C_EntCustId";
		/// <summary>法人名</summary>
		public static string ATOBARAICOM_API_MODIFY_ALL_CORPORATE_NAME = "C_CorporateName";
		/// <summary>部署名</summary>
		public static string ATOBARAICOM_API_MODIFY_ALL_DEPARTMENT_NAME = "C_DivisionName";
		/// <summary>担当者名</summary>
		public static string ATOBARAICOM_API_MODIFY_ALL_CONTACT_NAME = "C_CpNameKj";
		/// <summary>請求書別送</summary>
		public static string ATOBARAICOM_API_MODIFY_ALL_SEPARATE_INVOICE = "C_SeparateShipment";
		#endregion

		#region 個別の配送情報
		/// <summary>請求書別送</summary>
		public static string ATOBARAICOM_API_MODIFY_ALL_SPECIFY_ANOTHER_DELIVERY_DESTINATION = "O_AnotherDeliFlg";
		/// <summary>請求書別送</summary>
		public static string ATOBARAICOM_API_MODIFY_ALL_SHIPPING_POSTAL_CODE = "D_PostalCode";
		/// <summary>請求書別送</summary>
		public static string ATOBARAICOM_API_MODIFY_ALL_SHIPPING_STREET_ADDRESS = "D_UnitingAddress";
		/// <summary>氏名</summary>
		public static string ATOBARAICOM_API_MODIFY_ALL_SHIPPING_NAME = "D_DestNameKj";
		/// <summary>氏名</summary>
		public static string ATOBARAICOM_API_MODIFY_ALL_SHIPPING_NAME_KANA = "D_DestNameKn";
		/// <summary>氏名</summary>
		public static string ATOBARAICOM_API_MODIFY_ALL_SHIPPING_TEL_NO = "D_Phone";
		#endregion

		#region 商品情報
		/// <summary>購入品目</summary>
		public static string ATOBARAICOM_API_MODIFY_PURCHASED_ITEM = "I_ItemNameKj_";
		/// <summary>単価</summary>
		public static string ATOBARAICOM_API_MODIFY_UNIT = "I_UnitPrice_";
		/// <summary>数量</summary>
		public static string ATOBARAICOM_API_MODIFY_QUANTITY = "I_ItemNum_";
		/// <summary>消費税率</summary>
		public static string ATOBARAICOM_API_MODIFY_CONSUMPTION_TAX_RATE = "I_TaxRate_";
		/// <summary>商品送料</summary>
		public static string ATOBARAICOM_API_MODIFY_PRODUCT_SHIPPING = "I_ItemCarriage";
		/// <summary>店舗手数料</summary>
		public static string ATOBARAICOM_API_MODIFY_STORE_FEES = "I_ItemCharge";
		/// <summary></summary>
		public static string ATOBARAICOM_API_MODIFY_FOREIGN_TAX_AMOUNT = "I_OutsideTax";
		#endregion

		#region クレジット結果情報
		/// <summary>与信結果情報</summary>
		public static string ATOBARAICOM_API_MODIFY_CREDIT_RESULT_INFORMATION = "O_RtOrderStatus";
		#endregion
		#endregion

		#region Voucher number registration
		/// <summary>Atobaraicom api result: Error</summary>
		public const string ATOBARAICOM_API_RESULT_STATUS_ERROR = "error";
		/// <summary>Atobaraicom api result: Success</summary>
		public const string ATOBARAICOM_API_RESULT_STATUS_SUCCESS = "success";
		/// <summary>Atobaraicom api auth result status: OK</summary>
		public const string ATOBARAICOM_API_AUTH_RESULT_STATUS_OK = "与信OK";
		/// <summary>Atobaraicom api auth result status: NG</summary>
		public const string ATOBARAICOM_API_AUTH_RESULT_STATUS_NG = "与信NG";
		/// <summary>Atobaraicom api auth result status: During authorize</summary>
		public const string ATOBARAICOM_API_AUTH_RESULT_STATUS_HOLD = "与信中";
		/// <summary>Atobaraicom api auth result status: During authorize</summary>
		public const string ATOBARAICOM_API_AUTH_RESULT_STATUS_CANCEL = "与信キャンセル";
		/// <summary>Atobaraicom api param: Enterprise id</summary>
		public const string ATOBARAICOM_API_PARAM_ENTERPRISE_ID = "EnterpriseId";
		/// <summary>Atobaraicom api param: Api user id</summary>
		public const string ATOBARAICOM_API_PARAM_API_USER_ID = "ApiUserId";
		/// <summary>Atobaraicom api param: Order id</summary>
		public const string ATOBARAICOM_API_PARAM_ORDER_ID = "OrderId";
		/// <summary>Atobaraicom api param: Delivery id</summary>
		public const string ATOBARAICOM_API_PARAM_DELIV_ID = "DelivId";
		/// <summary>Atobaraicom api param: Journal num</summary>
		public const string ATOBARAICOM_API_PARAM_JOURNAL_NUM = "JournalNum";
		/// <summary>Atobaraicom api param: C_Separate shipment </summary>
		public const string ATOBARAICOM_API_PARAM_C_SEPARATE_SHIPMENT = "C_SeparateShipment";

		/// <summary>Flag Atobaraicom api C_Separate shipment invalid</summary>
		public const string FLG_ATOBARAICOM_API_C_SEPARATE_SHIPMENT_INVALID = "0";
		/// <summary>Flag Atobaraicom api C_Separate shipment valid</summary>
		public const string FLG_ATOBARAICOM_API_C_SEPARATE_SHIPMENT_VALID = "1";
		#endregion

		#region Get order status
		/// <summary>事業者ID</summary>
		public const string ATOBARAICOM_API_GET_ORDER_STATUS_BUSINESS_ID = "EnterpriseId";
		/// <summary>APIユーザーID</summary>
		public const string ATOBARAICOM_API_GET_ORDER_STATUS_API_USER_ID = "ApiUserId";
		/// <summary>後払い注文ID</summary>
		public const string ATOBARAICOM_API_GET_ORDER_STATUS_ORDER_NUMBER_LIST = "OrderId[]";

		/// <summary>Flag Atobaraicom api order status: Hold</summary>
		public const string GET_AUTH_RESULT_AUTH_RESULT_HOLD = "0";
		/// <summary>Flag Atobaraicom api order status: OK</summary>
		public const string GET_AUTH_RESULT_AUTH_RESULT_OK = "1";
		/// <summary>Flag Atobaraicom api order status: NG</summary>
		public const string GET_AUTH_RESULT_AUTH_RESULT_NG = "2";
		/// <summary>Flag Atobaraicom api order status: Cancel</summary>
		public const string GET_AUTH_RESULT_AUTH_RESULT_CANCEL = "3";
		#endregion

		/// <summary>Atobaraicom api text regist purchased item</summary>
		public const string ATOBARAICOM_API_TEXT_REGIST_PURCHASED_ITEM = "各種お値引き";
		/// <summary>Atobaraicom api text regist quantity</summary>
		public const string ATOBARAICOM_API_TEXT_REGIST_QUANTITY = "1";
		/// <summary>Atobaraicom api text item name price regulation</summary>
		public const string ATOBARAICOM_API_TEXT_ITEM_NAME_PRICE_REGULATION = "調整金額";

		/// <summary>Atobaraicom payment memo text auth confirmed</summary>
		public const string ATOBARAICOM_PAYMENT_MEMO_TEXT_AUTH_CONFIRMED = "与信（修正）";
	}
}