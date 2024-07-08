/*
=========================================================================================================
  Module      : EC Pay Constants(ECPayConstants.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.ECPay
{
	/// <summary>
	/// EC Pay Constants Class
	/// </summary>
	public class ECPayConstants
	{
		/// <summary>Merchant Id</summary>
		public const string PARAM_MERCHANT_ID = "MerchantID";
		/// <summary>All Pay Logistics Id</summary>
		public const string PARAM_ALL_PAY_LOGISTICS_ID = "AllPayLogisticsID";
		/// <summary>Merchant Trade No</summary>
		public const string PARAM_MERCHANT_TRADE_NO = "MerchantTradeNo";
		/// <summary>Merchant Trade Date</summary>
		public const string PARAM_MERCHANT_TRADE_DATE = "MerchantTradeDate";
		/// <summary>Logistics Type</summary>
		public const string PARAM_LOGISTICS_TYPE = "LogisticsType";
		/// <summary>Logistics Sub-Type</summary>
		public const string PARAM_LOGISTICS_SUB_TYPE = "LogisticsSubType";
		/// <summary>Goods Amount</summary>
		public const string PARAM_GOODS_AMOUNT = "GoodsAmount";
		/// <summary>Collection Amount</summary>
		public const string PARAM_COLLECTION_AMOUNT = "CollectionAmount";
		/// <summary>Is Collection</summary>
		public const string PARAM_IS_COLLECTION = "IsCollection";
		/// <summary>Goods Name</summary>
		public const string PARAM_GOODS_NAME = "GoodsName";
		/// <summary>Sender Name</summary>
		public const string PARAM_SENDER_NAME = "SenderName";
		/// <summary>Sender Phone</summary>
		public const string PARAM_SENDER_PHONE = "SenderPhone";
		/// <summary>Sender Cell Phone</summary>
		public const string PARAM_SENDER_CELL_PHONE = "SenderCellPhone";
		/// <summary>Receiver Name</summary>
		public const string PARAM_RECEIVER_NAME = "ReceiverName";
		/// <summary>Receiver Phone</summary>
		public const string PARAM_RECEIVER_PHONE = "ReceiverPhone";
		/// <summary>Receiver Cell Phone</summary>
		public const string PARAM_RECEIVER_CELL_PHONE = "ReceiverCellPhone";
		/// <summary>Receiver Email</summary>
		public const string PARAM_RECEIVER_EMAIL = "ReceiverEmail";
		/// <summary>Server Reply URL</summary>
		public const string PARAM_SERVER_REPLY_URL = "ServerReplyURL";
		/// <summary>Platform Id</summary>
		public const string PARAM_PLATFORM_ID = "PlatformID";
		/// <summary>Check Mac Value</summary>
		public const string PARAM_CHECK_MAC_VALUE = "CheckMacValue";
		/// <summary>Sender Zip Code</summary>
		public const string PARAM_SENDER_ZIP_CODE = "SenderZipCode";
		/// <summary>Sender Address</summary>
		public const string PARAM_SENDER_ADDRESS = "SenderAddress";
		/// <summary>Receiver Zip Code</summary>
		public const string PARAM_RECEIVER_ZIP_CODE = "ReceiverZipCode";
		/// <summary>Receiver Address</summary>
		public const string PARAM_RECEIVER_ADDRESS = "ReceiverAddress";
		/// <summary>Temperature</summary>
		public const string PARAM_TEMPERATURE = "Temperature";
		/// <summary>Distance</summary>
		public const string PARAM_DISTANCE = "Distance";
		/// <summary>Specification</summary>
		public const string PARAM_SPECIFICATION = "Specification";
		/// <summary>Receiver Store Id</summary>
		public const string PARAM_RECEIVER_STORE_ID = "ReceiverStoreID";
		/// <summary>Service Type</summary>
		public const string PARAM_SERVICE_TYPE = "ServiceType";
		/// <summary>Return Code</summary>
		public const string PARAM_RTN_CODE = "RtnCode";
		/// <summary>Return Message</summary>
		public const string PARAM_RTN_MSG = "RtnMsg";
		/// <summary>Update Status Date</summary>
		public const string PARAM_UPDATE_STATUS_DATE = "UpdateStatusDate";
		/// <summary>Cvs Payment No</summary>
		public const string PARAM_CVS_PAYMENT_NO = "CVSPaymentNo";
		/// <summary>Booking Note</summary>
		public const string PARAM_BOOKING_NOTE = "BookingNote";
		/// <summary>Return Merchant Trade No</summary>
		public const string PARAM_RTN_MERCHANT_TRADE_NO = "RtnMerchantTradeNo";
		/// <summary>Return Order No</summary>
		public const string PARAM_RTN_ORDER_NO = "RtnOrderNo";
		/// <summary>Device</summary>
		public const string PARAM_DEVICE = "Device";
		/// <summary>Cvs Store Id</summary>
		public const string PARAM_CVSSTOREID = "CVSStoreID";
		/// <summary>Cvs Store Name</summary>
		public const string PARAM_CVSSTORENAME = "CVSStoreName";
		/// <summary>Cvs Address</summary>
		public const string PARAM_CVSADDRESS = "CVSAddress";
		/// <summary>Cvs Telephone</summary>
		public const string PARAM_CVSTELEPHONE = "CVSTelephone";
		/// <summary>Cvs Logistics C2C Reply URL</summary>
		public const string PARAM_CVS_LOGISTICS_C2C_REPLY_URL = "LogisticsC2CReplyURL";
		/// <summary>Cvs Store Type</summary>
		public const string PARAM_CVS_STORE_TYPE = "StoreType";
		/// <summary>Cvs Status</summary>
		public const string PARAM_CVS_STATUS = "Status";
		/// <summary>Cvs Store Id</summary>
		public const string PARAM_CVS_STORE_ID = "StoreID";
		/// <summary>Cvs Validation No</summary>
		public const string PARAM_CVS_VALIDATION_NO = "CVSValidationNo";

		/// <summary>Request Key: Hash Key</summary>
		public const string CONST_REQUEST_KEY_HASH_KEY = "HashKey";
		/// <summary>Request Key: Hash IV</summary>
		public const string CONST_REQUEST_KEY_HASH_IV = "HashIV";
		/// <summary>Format date api</summary>
		public const string CONST_FORMAT_DATE_API = "yyyy/MM/dd HH:mm:ss";
		/// <summary>Encrypt Type</summary>
		public const string CONST_ENCRYPT_TYPE = "1";
		/// <summary>Binding Card</summary>
		public const string CONST_BINDING_CARD = "1";
		/// <summary>User Credit Card Display Name</summary>
		public const string USERCREDITCARD_CARDDISPNAME_ECPAYCUSTOMER = "EcPayCustomer";

		/// <summary>Code success</summary>
		public const string CONST_CODE_SUCCESS = "1";
		/// <summaryCode >Payment Info Url Atm Success</summary>
		public const string CONST_CODE_PAYMENT_INFO_URL_ATM_SUCCESS = "2";
		/// <summary>Code Payment Info Url Barcode Or Cvs Success</summary>
		public const string CODE_PAYMENT_INFO_URL_BARCODE_OR_CVS_SUCCESS = "10100073";

		/// <summary>Order Id</summary>
		public const string PARAM_ORDER_ID = "orderid";
		/// <summary>Server Reply Url</summary>
		public const string SERVER_REPLY_URL = "Payment/EcPay/DeliveryStatus.ashx?no=1&orderid=";
		/// <summary>Server Reply Url (No actions are needed for us)</summary>
		public const string SERVER_REPLY_NO_ACTION_URL = "Payment/EcPay/NoActionHandler.ashx?no=1&orderid=";
		/// <summary>Server Logistics C2C Reply Url</summary>
		public const string SERVER_LOGISTICS_C2C_REPLY_URL = "Payment/EcPay/DeliveryStatus.ashx?no=2&orderid=";
		/// <summary>Page Receiving Store For Ec Pay Response</summary>
		public const string PAGE_RECEIVING_STORE_FOR_ECPAY_RESPONSE = "Payment/EcPay/ReceivingStoreForEcPayResponse.aspx";

		/// <summary>Temperature: Normal</summary>
		public const string CONST_TEMPERATURE_NORMAL = "0001";
		/// <summary>Temperature: Refrigerated</summary>
		public const string CONST_TEMPERATURE_REFRIGERATED = "0002";
		/// <summary>Temperature: Freezing</summary>
		public const string CONST_TEMPERATURE_FREEZING = "0003";

		/// <summary>Specification: Small</summary>
		public const string CONST_SPECIFICATION_SMALL = "0001";
		/// <summary>Specification: Normal</summary>
		public const string CONST_SPECIFICATION_NORMAL = "0002";
		/// <summary>Specification: Medium</summary>
		public const string CONST_SPECIFICATION_MEDIUM = "0003";
		/// <summary>Specification: Big</summary>
		public const string CONST_SPECIFICATION_BIG = "0004";

		/// <summary>Shipping service id (Yamato): Tepid</summary>
		public const string CONST_SHIPPING_SERVICE_ID_YAMATO_TEPID = "TWECPAYYT";
		/// <summary>Shipping service id (Yamato): Refrigerated</summary>
		public const string CONST_SHIPPING_SERVICE_ID_YAMATO_REFRIGERATED = "TWECPAYYR";
		/// <summary>Shipping service id (Yamato): Frozen</summary>
		public const string CONST_SHIPPING_SERVICE_ID_YAMATO_FROZEN = "TWECPAYYF";

		/// <summary>Distance: Remote island</summary>
		public const string CONST_DISTANCE_REMOTE_ISLAND = "02";
		/// <summary>Distance: different city</summary>
		public const string CONST_DISTANCE_DIFFERENT_CITY = "01";
		/// <summary>Distance: same city</summary>
		public const string CONST_DISTANCE_SAME_CITY = "00";
		/// <summary>Cvs logistic method C2C</summary>
		public const string CONST_CVS_LOGISTIC_METHOD_C2C = "C2C";

		/// <summary>Payment Type</summary>
		public const string CONST_PAYMENT_TYPE = "aio";
		/// <summary>Default store type</summary>
		public const string CONST_DEFAULT_STORE_TYPE = "01";
		/// <summary>Default service type</summary>
		public const string CONST_DEFAULT_SERVICE_TYPE = "4";

		/// <summary>Return Url Parameter No</summary>
		public const string CONST_RETURN_URL_PARAMETER_NO = "1";
		/// <summary>Payment Url Parameter No</summary>
		public const string CONST_PAYMENT_URL_PARAMETER_NO = "2";

		/// <summary>Specification size: Small</summary>
		public const int CONST_SPECIFICATION_SIZE_SMALL = 60;
		/// <summary>Specification size: Normal</summary>
		public const int CONST_SPECIFICATION_SIZE_NORMAL = 90;
		/// <summary>Specification size: Medium</summary>
		public const int CONST_SPECIFICATION_SIZE_MEDIUM = 120;
		/// <summary>Specification size: Big</summary>
		public const int CONST_SPECIFICATION_SIZE_BIG = 150;

		/// <summary>Action Cancel/summary>
		public const string CONST_ACTION_CANCEL = "N";
		/// <summary>Action Refund</summary>
		public const string CONST_ACTION_REFUND = "R";
		/// <summary>Action Capture</summary>
		public const string CONST_ACTION_CAPTURE = "C";

		/// <summary>Url express print Family mart C2C</summary>
		public const string CONST_URL_EXPRESS_PRINT_FAMI_C2C = "Express/PrintFAMIC2COrderInfo";
		/// <summary>Url express print HiLife C2C</summary>
		public const string CONST_URL_EXPRESS_PRINT_HILIFE_C2C = "Express/PrintHILIFEC2COrderInfo";
		/// <summary>Url express print 7-ELEVEN C2C</summary>
		public const string CONST_URL_EXPRESS_PRINT_UNIMART_C2C = "Express/PrintUniMartC2COrderInfo";

		/// <summary>Convenience store limit weight</summary>
		public const decimal CONST_CONVENIENCE_STORE_LIMIT_WEIGHT = 5;

		/// <summary>HTTP method: POST</summary>
		public const string HTTP_METHOD_POST = "POST";
		/// <summary>HTTP header: Content-Type</summary>
		public const string HTTP_HEADER_CONTENT_TYPE = "Content-Type";
		/// <summary>HTTP header: Content-Type: application json</summary>
		public const string HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON = "application/json";
		/// <summary>HTTP header: Content-Type: application form</summary>
		public const string HTTP_HEADER_CONTENT_TYPE_APPLICATION_FORM = "application/x-www-form-urlencoded";
	}
}
