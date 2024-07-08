/*
=========================================================================================================
  Module      : Feiler カスタマイズ定数定義(CustomConstants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/

namespace P0025_Feiler.w2.Commerce.ExternalAPI
{
	static class CustomConstants
	{
		public const string GIFT_WRAPPING_PRODUCT_ID = "99-99-99-9991";				// ギフト（ラッピングあり）商品
		public const string GIFT_NO_WRAPPING_PRODUCT_ID = "99-99-99-9990";			// ギフト（ラッピングなし）商品

		public const string FLG_ORDER_MALL_ID_OWN_SITE = "OWN_SITE";				// 自社
		public const string FLG_ORDER_MALL_ID_YAHOO = "yahoo_1";					// Yahoo
		public const string FLG_ORDER_MALL_ID_RAKUTEN = "rakuten1";					// 楽天

		public const string FLG_ORDERSHIPPING_WRAPPING_BAG_TYPE_NONE = "0";			// 無し
		public const string FLG_ORDERSHIPPING_WRAPPING_BAG_TYPE_ALL = "1";			// まとめて
		public const string FLG_ORDERSHIPPING_WRAPPING_BAG_TYPE_INDIVIDUAL = "2";	// 個別
		public const string FLG_ORDERSHIPPING_WRAPPING_BAG_TYPE_OTHERS = "3";		// その他

		public const string FLG_ORDERITEMS_WRAPPING_BAG_TYPE_VALID = "1";			// 有り
		public const string FLG_ORDERITEMS_WRAPPING_BAG_TYPE_INVALID = "0";			// 無し

		public const string FLG_SUFFIX_PRODUCT_ID = "GF";							// Suffix of Gift Product

		public const string FLG_ORDER_GIFT_FLG_VALID = "1";							// 有り
		public const string FLG_ORDER_EXTEND_STATUS_VALID = "1";					// 有り

		public const string EXPORT_ORDERSHIPPING_SETTING_FILENAME = "P0025_Feiler.w2.Commerce.ExternalAPI.ini";			// ini setting file name
	}
}
