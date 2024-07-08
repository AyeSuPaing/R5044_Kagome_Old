/*
=========================================================================================================
  Module      : Constants (Constants.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.Batch.WmsShippingBatch
{
	/// <summary>
	/// Constants
	/// </summary>
	public class Constants : App.Common.Constants
	{
		/// <summary>Directory path waiting for processing</summary>
		public static string DIR_PATH_WAITING_FOR_PROCESSING = string.Empty;
		/// <summary>Directory path uploading</summary>
		public static string DIR_PATH_UPLOADING = string.Empty;
		/// <summary>Directory path uploaded</summary>
		public static string DIR_PATH_UPLOADED = string.Empty;
		/// <summary>Directory path wait download file</summary>
		public static string DIR_PATH_WAIT_DOWNLOAD_FILE = string.Empty;
		/// <summary>Directory path downloaded file</summary>
		public static string DIR_PATH_DOWNLOADED_FILE = string.Empty;
		/// <summary>Path xml file Wms messages</summary>
		public static string PATH_XML_WMS_MESSAGES = string.Empty;

		/// <summary>Shipper code</summary>
		public const string SHIPPER_CODE = "shipper_code";
		/// <summary>Cash on delivery flag</summary>
		public const string CASH_ON_DELIVERY_FLG = "cash_on_delivery_flg";
		/// <summary>Total item amount</summary>
		public const string TOTAL_ITEM_AMOUNT = "total_item_amount";
		/// <summary>Shipping tax excluded</summary>
		public const string SHIPPING_TAX_EXCLUDED = "shipping_tax_excluded";
		/// <summary>Payment tax excluded</summary>
		public const string PAYMENT_TAX_EXCLUDED = "payment_tax_excluded";
		/// <summary>Total item amount rate 10</summary>
		public const string TOTAL_ITEM_AMOUNT_RATE_10 = "total_item_amount_rate_10";
		/// <summary>Tax amount rate 10</summary>
		public const string TAX_AMOUNT_RATE_10 = "tax_amount_rate_10";
		/// <summary>Total item amount rate 8</summary>
		public const string TOTAL_ITEM_AMOUNT_RATE_8 = "total_item_amount_rate_8";
		/// <summary>Tax amount rate 8</summary>
		public const string TAX_AMOUNT_RATE_8 = "tax_amount_rate_8";
		/// <summary>Paper processing flag</summary>
		public const string PAPER_PROCESSING_FLAG = "paper_processing_flag";
		/// <summary>Remarks</summary>
		public const string REMARKS = "remarks";
		/// <summary>Tax amount item</summary>
		public const string TAX_AMOUNT_ITEM = "tax_amount_item";
		/// <summary>Discount 1</summary>
		public const string DISCOUNT_1 = "discount_1";
		/// <summary>Order price discount total rate 10</summary>
		public const string ORDER_PRICE_DISCOUNT_TOTAL_RATE_10 = "order_price_discount_total_rate_10";
		/// <summary>Order price discount total rate 8</summary>
		public const string ORDER_PRICE_DISCOUNT_TOTAL_RATE_8 = "order_price_discount_total_rate_8";
		/// <summary>Item amount</summary>
		public const string ITEM_AMOUNT = "item_amount";
		/// <summary>Consumption tax</summary>
		public const string CONSUMPTION_TAX = "consumption_tax";
		/// <summary>Client change flag</summary>
		public const string CLIENT_CHANGE_FLAG = "client_change_flag";
		/// <summary>Price subtotal by rate 8</summary>
		public const string PRICE_SUBTOTAL_BY_RATE_8 = "price_subtotal_by_rate_8";
		/// <summary>Price subtotal by rate 10</summary>
		public const string PRICE_SUBTOTAL_BY_RATE_10 = "price_subtotal_by_rate_10";
		/// <summary>Tax price by rate 8</summary>
		public const string TAX_PRICE_BY_RATE_8 = "tax_price_by_rate_8";
		/// <summary>Tax price by rate 10</summary>
		public const string TAX_PRICE_BY_RATE_10 = "tax_price_by_rate_10";
		/// <summary>Price total item 10</summary>
		public const string PRICE_TOTAL_ITEM_10 = "price_total_item_10";
		/// <summary>Price total item 8</summary>
		public const string PRICE_TOTAL_ITEM_8 = "price_total_item_8";
		/// <summary>Cash on delivery flag: valid</summary>
		public const string FLG_CASH_ON_DELIVERY_FLG_VALID = "1";
		/// <summary>Cash on delivery flag: invalid</summary>
		public const string FLG_CASH_ON_DELIVERY_FLG_INVALID = "0";
		/// <summary>Client change flag: valid</summary>
		public const string FLG_CLIENT_CHANGE_FLG_VALID = "1";
		/// <summary>Client change flag: invalid</summary>
		public const string FLG_CLIENT_CHANGE_FLG_INVALID = "0";
		/// <summary>Paper processing flag: valid</summary>
		public const int FLG_PAPER_PROCESSING_FLG_VALID = 1;
		/// <summary>Paper processing flag: invalid</summary>
		public const int FLG_PAPER_PROCESSING_FLG_INVALID = 0;
		/// <summary>Time delay milliseconds</summary>
		public const int TIME_DELAY_MILLISECONDS = 60000;
		/// <summary>商品税率8%</summary>
		public const int PRODUCT_TAX_RATE_8 = 8;
		/// <summary>商品税率10%</summary>
		public const int PRODUCT_TAX_RATE_10 = 10;
	}
}
