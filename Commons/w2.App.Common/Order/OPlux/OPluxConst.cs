/*
=========================================================================================================
  Module      : OPlux Const(OPluxConst.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.OPlux
{
	/// <summary>
	/// O-PLUX const
	/// </summary>
	public static class OPluxConst
	{
		/// <summary>Parameter name</summary>
		public const string PARAM_NAME = "name";
		/// <summary>Parameter fields</summary>
		public const string PARAM_FIELDS = "fields";
		/// <summary>O-PLUX payment kbn credit</summary>
		public const string OPLUX_PAYMENT_KBN_CREDIT = "Credit";
		/// <summary>O-PLUX payment kbn other</summary>
		public const string OPLUX_PAYMENT_KBN_OTHER = "Other";
		/// <summary>First name</summary>
		public const string CONST_FIELD_FIRST_NAME = "firstName";
		/// <summary>Last name</summary>
		public const string CONST_FIELD_LAST_NAME = "lastName";
		/// <summary>Version default</summary>
		public const string VERSION_DEFAULT = "1.0";
		/// <summary>Event type ec</summary>
		public const string EVENT_TYPE_EC = "EC";
		/// <summary>Hash method sha256</summary>
		public const string HASH_METHOD_SHA256 = "SHA256";
		/// <summary>Settle limit price</summary>
		public const decimal SETTLE_LIMIT_PRICE = 999999;
		/// <summary>Settle status before billing</summary>
		public const string SETTLE_STATUS_BEFORE_BILLING = "00";
		/// <summary>Settle status billing</summary>
		public const string SETTLE_STATUS_BILLING = "10";
		/// <summary>Settle status paid</summary>
		public const string SETTLE_STATUS_PAID = "20";
		/// <summary>Settle status cancel</summary>
		public const string SETTLE_STATUS_CANCEL = "99";
		/// <summary>Settle status cancel after sending</summary>
		public const string SETTLE_STATUS_CANCEL_AFTER_SENDING = "100";
		/// <summary>Settle method postpay</summary>
		public const string SETTLE_METHOD_POSTPAY = "01";
		/// <summary>Settle method credit card payment</summary>
		public const string SETTLE_METHOD_CREDIT_CARD_PAYMENT = "02";
		/// <summary>Settle method cash on delivery</summary>
		public const string SETTLE_METHOD_CASH_ON_DELIVERY = "03";
		/// <summary>Settle method prepaid</summary>
		public const string SETTLE_METHOD_PREPAID = "04";
		/// <summary>Settle method digital cash</summary>
		public const string SETTLE_METHOD_DIGITAL_CASH = "05";
		/// <summary>Settle method point payment</summary>
		public const string SETTLE_METHOD_POINT_PAYMENT = "06";
		/// <summary>Settle method direct debit</summary>
		public const string SETTLE_METHOD_DIRECT_DEBIT = "07";
		/// <summary>Settle method installment</summary>
		public const string SETTLE_METHOD_INSTALLMENT = "08";
		/// <summary>Settle method Payeasy</summary>
		public const string SETTLE_METHOD_PAYEASY = "09";
		/// <summary>Settle method PayPal</summary>
		public const string SETTLE_METHOD_PAYPAL = "10";
		/// <summary>Settle method others</summary>
		public const string SETTLE_METHOD_OTHERS = "99";
		/// <summary>Buyer type</summary>
		public const string BUYER_TYPE = "10";
		/// <summary>Delivery type</summary>
		public const string DELIVERY_TYPE = "20";
		/// <summary>Hashed name valid name exist</summary>
		public const string HASHED_NAME_VALID_NAME_EXIST = "1";
		/// <summary>Hashed name valid name not exist</summary>
		public const string HASHED_NAME_VALID_NAME_NOT_EXIST = "0";
		/// <summary>Api name normalization result OK</summary>
		public const string API_NAME_NORMALIZATION_RESULT_OK = "success";
		/// <summary>Api register event result review</summary>
		public const string API_REGISTER_EVENT_RESULT_REVIEW = "REVIEW";
		/// <summary>Api register event result NG</summary>
		public const string API_REGISTER_EVENT_RESULT_NG = "NG";
		/// <summary>配送希望有無: 値があれば</summary>
		public const string DELIVERY_SPECIFIED_EXISTENCE_VALID = "1";
		/// <summary>配送希望有無: 値が無ければ</summary>
		public const string DELIVERY_SPECIFIED_EXISTENCE_INVALID = "0";
		/// <summary>Default web request method</summary>
		public const string DEFAULT_WEB_REQUEST_METHOD = "POST";
		/// <summary>Default web request time out</summary>
		public const int DEFAULT_WEB_REQUEST_TIME_OUT = 5000;
		/// <summary>Default web request content type</summary>
		public const string DEFAULT_WEB_REQUEST_CONTENT_TYPE = "application/x-www-form-urlencoded";
	}
}
