/*
=========================================================================================================
  Module      : Rakuten Constants(RakutenConstants.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.Rakuten
{
	/// <summary>
	/// Rakuten Constants Class
	/// </summary>
	public class RakutenConstants
	{
		/// <summary>Result type is failure</summary>
		public const string RESULT_TYPE_FAILURE = "failure";
		/// <summary>Result type is failure</summary>
		public const string RESULT_TYPE_SUCCESS = "success";
		/// <summary>Result type is peding</summary>
		public const string RESULT_TYPE_PENDING = "pending";

		/// <summary>Payment Info</summary>
		public const string HTTP_PARAMETER_PAYMENT_INFO = "paymentinfo";
		/// <summary>Signature</summary>
		public const string HTTP_PARAMETER_SIGNATURE = "signature";
		/// <summary>Key Version</summary>
		public const string HTTP_PARAMETER_KEY_VERSION = "key_version";

		/// <summary>3Dセキュア認証APIレスポンスパラメータ paymentResult</summary>
		public const string THREE_D_SECURE_RESPONSE_PARAMETER_PAYMENT_RESULT = "paymentResult";

		/// <summary>HTTP method: POST</summary>
		public const string HTTP_METHOD_POST = "POST";
		/// <summary>HTTP header: Content-Type</summary>
		public const string HTTP_HEADER_CONTENT_TYPE = "Content-Type";
		/// <summary>HTTP header: Content-Type: application json</summary>
		public const string HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON = "application/json";
		/// <summary>HTTP header: Content-Type: application form</summary>
		public const string HTTP_HEADER_CONTENT_TYPE_APPLICATION_FORM = "application/x-www-form-urlencoded";

		/// <summary>Rakuten Agency code default</summary>
		public static string AGENCY_CODE_DEFAULT = "rakutencard";
		/// <summary>Rakuten Email default</summary>
		public static string EMAIL_DEFAULT = "rakuten@card";
		/// <summary>Rakuten IpAddress default</summary>
		public static string IPADDRESS_DEFAULT = "192.168.0.1";
		/// <summary>Rakuten Currency code default</summary>
		public static string CURRENCY_CODE_DEFAULT = "JPY";
		/// <summary>Rakuten Version card token</summary>
		public static int VERSION_CARD_TOKEN_DEFAULT_THREE_D_SECURE = 3;
		/// <summary>Rakuten Version card token</summary>
		public static int VERSION_CARD_TOKEN_DEFAULT = 2;
		/// <summary>Rakuten Version Order</summary>
		public static int VERSION_ORDER_DEFAULT = 1;
		/// <summary>Authentication Idicator Type default</summary>
		public static string AUTHENTICATION_INDICATOR_TYPE_DEFAULT = "payment_transaction";
		/// <summary>Authentication Idicator Type default</summary>
		public static string MESSAGE_CATEGORY_TYPE_PA = "pa";
		/// <summary>Authentication Idicator Type default</summary>
		public static string MESSAGE_CATEGORY_TYPE_NPA = "npa";
		/// <summary>Authentication Idicator Type default</summary>
		public static string TRANSACTION_TYPE_DEFAULT = "goods_service_purchase";
		/// <summary>Rakuten URL Authozire HTML Rakuten 3D Secure OFF</summary>
		public static string URL_PAYMENT_AUTHORIZE_3D_OFF = "{0}gp/Payment/V1/Authorize";
		/// <summary>Rakuten URL Authozire and Capture HTML 3D Secure OFF</summary>
		public static string URL_PAYMENT_AUTHORIZE_AND_CAPTURE_3D_OFF = "{0}gp/Payment/V1/AuthorizeAndCapture";
		/// <summary>Rakuten URL Modify</summary>
		public static string URL_PAYMENT_MODIFY = "{0}gp/Payment/V1/Modify";
		/// <summary>Rakuten URL Capture</summary>
		public static string URL_PAYMENT_CAPTURE = "{0}gp/Payment/V1/Capture";
		/// <summary>Rakuten URL Cancel Or Refund</summary>
		public static string URL_PAYMENT_CANCEL_OR_REFUND = "{0}gp/Payment/V1/CancelOrRefund";
		/// <summary>Rakuten URL Authozire HTML Rakuten 3D Secure ON</summary>
		public static string URL_PAYMENT_AUTHORIZE_3D_ON = "{0}gp/Payment/V2/Authorize/HTML";
		/// <summary>Rakuten URL Authozire and Capture HTML 3D Secure OFF</summary>
		public static string URL_PAYMENT_AUTHORIZE_AND_CAPTURE_3D_ON = "{0}gp/Payment/V2/AuthorizeAndCapture/HTML";

		/// <summary>Rakuten 認証キーバージョン</summary>
		public static string KEY_VERSION = "1";

		/// <summary>Request type capture</summary>
		public const string REQUEST_TYPE_CAPTURE = "capture";
		/// <summary>Request type authorize</summary>
		public const string REQUEST_TYPE_AUTHORIZE = "authorize";
		/// <summary>Request type cancel or refund</summary>
		public const string REQUEST_TYPE_CANCEL_OR_REFUND = "cancel_or_refund";

		/// <summary>Dummy value of product id</summary>
		public const string DUMMY_VALUE_OF_PRODUCT_ID = "DUMMY_PRODUCT_ID";
		/// <summary>Dummy value of product quantity</summary>
		public const int DUMMY_VALUE_OF_PRODUCT_QUANTITY = 1;
		/// <summary>Parameter key version</summary>
		public const string PARAMETER_KEY_VERSION = "keyversion";
		/// <summary>Date limit allow auth</summary>
		public const int DATE_LIMIT_ALLOW_AUTH = 60;
		/// <summary>Convenience store type default</summary>
		public const string CVS_TYPE_DEFAULT = "SevenEleven";
		/// <summary>Rakuten cvs email default</summary>
		public static string CVS_EMAIL_DEFAULT = "rakuten@card.cc";
		/// <summary>Payment rakuten cvs def type sub service id seven</summary>
		public const string PAYMENT_RAKUTENCVSDEF_TYPE_SUBSERVICEID_SEVEN = "SevenEleven";
		/// <summary>Payment rakuten cvs def type sub service id econ</summary>
		public const string PAYMENT_RAKUTENCVSDEF_TYPE_SUBSERVICEID_ECON = "Econ";
		/// <summary>Payment rakuten cvs def type sub service id wellnet</summary>
		public const string PAYMENT_RAKUTENCVSDEF_TYPE_SUBSERVICEID_WELLNET = "Wellnet";
		/// <summary>
		/// Rakuten Constants Error Code
		/// </summary>
		public class ErrorCode 
		{
			/// <summary>Error Code: Invalid Ppayment Method</summary>
			public const string INVALID_PAYMENT_MOTHOD = "invalid_payment_method";
			/// <summary>Error Code: Unauthorized Access</summary>
			public const string UNAUTHORIZED_ACCESS = "unauthorized_access";
			/// <summary>Error Code: Cvv Token Unavailable</summary>
			public const string CVV_TOKEN_UNAVAILABLE = "cvv_token_unavailable";
			/// <summary>Error Code: Already Completed</summary>
			public const string ALREADY_COMPLETED = "already_completed";
			/// <summary>Error Code: Invalid Payment Method</summary>
			public const string INVALID_PAYMENT_METHOD = "invalid_payment_method";
			/// <summary>Error Code: Not Found</summary>
			public const string NOT_FOUND = "not_found";
			/// <summary>Error Code: Unacceptable Request</summary>
			public const string UNACCEPTABLE_REQUEST = "unacceptable_request";
		}

		/// <summary>
		/// Rakuten Constants クレジット分割情報
		/// </summary>
		public class DealingTypes
		{
			/// <summary>取引区分：一括</summary>
			public const string ONCE = "1";
			/// <summary>取引区分：ボーナス一括</summary>
			public const string BONUS1 = "BONUS1";
			/// <summary>取引区分：リボ</summary>
			public const string REVO = "REVO";
		}
	}
}
