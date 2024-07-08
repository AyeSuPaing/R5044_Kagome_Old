/*
=========================================================================================================
  Module      : Neweb Pay Constants (NewebPayConstants.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.NewebPay
{
	/// <summary>
	/// NewebPay Constants Class
	/// </summary>
	public class NewebPayConstants
	{
		/// <summary>Param: Merchant ID</summary>
		public const string PARAM_MERCHANT_ID = "MerchantID";
		/// <summary>Param: Trade Info</summary>
		public const string PARAM_TRADE_INFO = "TradeInfo";
		/// <summary>Param: Trade Info Encode SHA256</summary>
		public const string PARAM_TRADE_SHA = "TradeSha";
		/// <summary>Param: Post Data</summary>
		public const string PARAM_POST_DATA = "PostData";
		/// <summary>Param: Respond type</summary>
		public const string PARAM_RESPOND_TYPE = "RespondType";
		/// <summary>Param: Time stamp</summary>
		public const string PARAM_TIME_STAMP = "TimeStamp";
		/// <summary>Param: Version</summary>
		public const string PARAM_VERSION = "Version";
		/// <summary>Param: Merchant Order No</summary>
		public const string PARAM_MERCHANT_ORDER_NO = "MerchantOrderNo";
		/// <summary>Param: Transaction amount</summary>
		public const string PARAM_AMOUNT = "Amt";
		/// <summary>Param: Item Desc</summary>
		public const string PARAM_ITEM_DESC = "ItemDesc";
		/// <summary>Param: Return URL</summary>
		public const string PARAM_RETURN_URL = "ReturnURL";
		/// <summary>Param: Notify URL</summary>
		public const string PARAM_NOTIFY_URL = "NotifyURL";
		/// <summary>Param: Customer URL</summary>
		public const string PARAM_CUSTOMER_URL = "CustomerURL";
		/// <summary>Param: Client Back URL</summary>
		public const string PARAM_CLIENT_BACK_URL = "ClientBackURL";
		/// <summary>Param: Email</summary>
		public const string PARAM_EMAIL = "Email";
		/// <summary>Param: Login Type</summary>
		public const string PARAM_LOGIN_TYPE = "LoginType";
		/// <summary>Param: Credit</summary>
		public const string PARAM_CREDIT = "CREDIT";
		/// <summary>Param: LinePay</summary>
		public const string PARAM_LINEPAY = "LINEPAY";
		/// <summary>Param: Credit Card Flag</summary>
		public const string PARAM_INST_FLAG = "InstFlag";
		/// <summary>Param: American Express Card</summary>
		/// <summary>Param: WEBATM</summary>
		public const string PARAM_WEBATM = "WEBATM";
		/// <summary>Param: ATM</summary>
		public const string PARAM_ATM = "VACC";
		/// <summary>Param: Convenience Store</summary>
		public const string PARAM_CVS = "CVS";
		/// <summary>Param: Barcode</summary>
		public const string PARAM_BARCODE = "BARCODE";
		/// <summary>Param: CVSCOM</summary>
		public const string PARAM_CVSCOM = "CVSCOM";
		/// <summary>Param: Token Term</summary>
		public const string PARAM_TOKEN_TERM = "TokenTerm";
		/// <summary>Param: Trade No</summary>
		public const string PARAM_TRADE_NO = "TradeNo";
		/// <summary>Param: Index type</summary>
		public const string PARAM_INDEX_TYPE = "IndexType";
		/// <summary>Param: Close Type</summary>
		public const string PARAM_CLOSE_TYPE = "CloseType";
		/// <summary>Param: Status</summary>
		public const string PARAM_STATUS = "Status";
		/// <summary>Param: Message</summary>
		public const string PARAM_MESSAGE = "Message";
		/// <summary>Const: Json Respond type</summary>
		public const string CONST_JSON_RESPONDTYPE = "JSON";
		/// <summary>Const: Version Api</summary>
		public const string CONST_VERSION_API = "1.5";
		/// <summary>Const: Index type</summary>
		public const string CONST_INDEX_TYPE = "2";
		/// <summary>Const: String Respond type</summary>
		public const string CONST_STRING_RESPONDTYPE = "String";
		/// <summary>Const: Hash Key</summary>
		public const string CONST_REQUEST_KEY_HASH_KEY = "HashKey";
		/// <summary>Const: Hash IV</summary>
		public const string CONST_REQUEST_KEY_HASH_IV = "HashIV";
		/// <summary>Const: Status success</summary>
		public const string CONST_STATUS_SUCCESS = "SUCCESS";

		/// <summary>Return Url Parameter No</summary>
		public const string CONST_NOTIFY_URL = "1";
		/// <summary>Payment Url Parameter No</summary>
		public const string CONST_CUSTOMER_URL = "2";
		/// <summary>Return Url Parameter No</summary>
		public const string CONST_RETURN_URL = "1";
		/// <summary>Payment Url Parameter No</summary>
		public const string CONST_CLIENT_BACK_URL = "2";

		/// <summary>HTTP method: POST</summary>
		public const string HTTP_METHOD_POST = "POST";
		/// <summary>HTTP header: Content-Type</summary>
		public const string HTTP_HEADER_CONTENT_TYPE = "Content-Type";
		/// <summary>HTTP header: Content-Type: application json</summary>
		public const string HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON = "application/json";
		/// <summary>HTTP header: Content-Type: application form</summary>
		public const string HTTP_HEADER_CONTENT_TYPE_APPLICATION_FORM = "application/x-www-form-urlencoded";

		/// <summary>Neweb Pay Payment Type On</summary>
		public const int FLG_NEWEBPAY_PAYMENT_TYPE_ON = 1;
		/// <summary>Neweb Pay Payment Type Off</summary>
		public const int FLG_NEWEBPAY_PAYMENT_TYPE_OFF = 0;

		/// <summary>Neweb Pay Payment CVSCOM Type On</summary>
		public const int FLG_NEWEBPAY_PAYMENT_CVSCOM_TYPE_ON = 2;

		/// <summary>Const: Close Type Sale</summary>
		public const int CONST_CLOSE_TYPE_SALE = 1;
		/// <summary>Const: Close Type Refund</summary>
		public const int CONST_CLOSE_TYPE_REFUND = 2;

		/// <summary>Const: Session Is Use Neweb Pay</summary>
		public const string CONST_SESSION_IS_USE_NEWEBPAY = "is_use_newebpay";

		/// <summary>Flg Credit Card Once Time</summary>
		public const string FLG_CREDIT_CARD_ONCE_TIME = "01";

		/// <summary>User Credit Card Display Name</summary>
		public const string USERCREDITCARD_CARDDISPNAME_NEWEBPAYCUSTOMER = "NewebPayCustomer";
	}
}
