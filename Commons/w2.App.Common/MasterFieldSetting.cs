/*
=========================================================================================================
  Module      : List Field MasterSetting(MasterFieldSetting.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace w2.App.Common
{
	///*********************************************************************************************
	/// <summary>
	/// MasterFieldSetting
	/// </summary>
	///*********************************************************************************************
	public class MasterFieldSetting
	{
		const string FIELD_NAME_SERIAL_KEYS = "serial_keys";
		const string FIELD_ORDER_SETPROMOTION_PRODUCT_DISCOUNT_AMOUNT_TOTAL = "setpromotion_product_discount_amount_total";
		const string FIELD_ORDER_SETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT_TOTAL = "setpromotion_shipping_charge_discount_amount_total";
		const string FIELD_ORDER_SETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT_TOTAL = "setpromotion_payment_charge_discount_amount_total";
		const string FIELD_SETPROMOTION_UNDISCOUNTED_PRODUCT_SUBTOTAL = "setpromotion_undiscounted_product_subtotal";
		const string FIELD_SETPROMOTION_PRODUCT_DISCOUNT_FLG = "setpromotion_product_discount_flg";
		const string FIELD_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG = "setpromotion_shipping_charge_free_flg";
		const string FIELD_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG = "setpromotion_payment_charge_free_flg";

		/// <summary>
		/// Get list field of Product which option is false
		/// </summary>
		/// <returns> List field</returns>
		public static List<string> GetProductFieldsDisable()
		{
			List<string> fieldsResult = new List<string>();

			if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG);
				fieldsResult.Add(Constants.FIELD_PRODUCT_DOWNLOAD_URL);
			}

			if (Constants.FIXEDPURCHASE_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG);
				fieldsResult.Add(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT);
			}

			if (Constants.GIFTORDER_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCT_GIFT_FLG);
			}

			if (Constants.MEMBER_RANK_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG);
				fieldsResult.Add(Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK);
				fieldsResult.Add(Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK);
			}

			if (Constants.PRODUCT_BRAND_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCT_BRAND_ID1);
				fieldsResult.Add(Constants.FIELD_PRODUCT_BRAND_ID2);
				fieldsResult.Add(Constants.FIELD_PRODUCT_BRAND_ID3);
				fieldsResult.Add(Constants.FIELD_PRODUCT_BRAND_ID4);
				fieldsResult.Add(Constants.FIELD_PRODUCT_BRAND_ID5);
			}

			if (Constants.MOBILEOPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCT_CATCHCOPY_MOBILE);
				fieldsResult.Add(Constants.FIELD_PRODUCT_OUTLINE_KBN_MOBILE);
				fieldsResult.Add(Constants.FIELD_PRODUCT_OUTLINE_MOBILE);
				fieldsResult.Add(Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1_MOBILE);
				fieldsResult.Add(Constants.FIELD_PRODUCT_DESC_DETAIL1_MOBILE);
				fieldsResult.Add(Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2_MOBILE);
				fieldsResult.Add(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE_MOBILE);
				fieldsResult.Add(Constants.FIELD_PRODUCT_IMAGE_MOBILE);
				fieldsResult.Add(Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN_MOBILE);
				fieldsResult.Add(Constants.FIELD_PRODUCT_DESC_DETAIL2_MOBILE);
				fieldsResult.Add(Constants.FIELD_PRODUCT_MOBILE_DISP_FLG);
			}

			if (Constants.W2MP_POINT_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCT_POINT_KBN1);
				fieldsResult.Add(Constants.FIELD_PRODUCT_POINT1);
				fieldsResult.Add(Constants.FIELD_PRODUCT_POINT_KBN2);
				fieldsResult.Add(Constants.FIELD_PRODUCT_POINT2);
				fieldsResult.Add(Constants.FIELD_PRODUCT_POINT_KBN3);
				fieldsResult.Add(Constants.FIELD_PRODUCT_POINT3);
				fieldsResult.Add(Constants.FIELD_PRODUCT_CAMPAIGN_FROM);
				fieldsResult.Add(Constants.FIELD_PRODUCT_CAMPAIGN_TO);
				fieldsResult.Add(Constants.FIELD_PRODUCT_CAMPAIGN_POINT_KBN);
				fieldsResult.Add(Constants.FIELD_PRODUCT_CAMPAIGN_POINT);
			}

			if (Constants.MALLCOOPERATION_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID);
				fieldsResult.Add(Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG);
			}

			if (Constants.GOOGLESHOPPING_COOPERATION_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG);
			}

			if (Constants.RECOMMEND_ENGINE_KBN != Constants.RecommendEngine.Silveregg)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG);
			}

			// Add ProductStock fields
			fieldsResult.AddRange(GetProductStockFieldsDisable());

			return fieldsResult;
		}

		/// <summary>
		/// Get list field of ProductVariaton which option is false
		/// </summary>
		/// <returns> List field</returns>
		public static List<string> GetProductVariationFieldsDisable()
		{
			List<string> fieldsResult = new List<string>();

			if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL);
			}

			if (Constants.MOBILEOPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_MOBILE);
			}

			if (Constants.MALLCOOPERATION_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1);
				fieldsResult.Add(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID2);
				fieldsResult.Add(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_TYPE);
				fieldsResult.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG);
			}

			return fieldsResult;
		}

		/// <summary>
		/// Get list field of ProductView which option is false
		/// </summary>
		/// <returns> List field</returns>
		public static List<string> GetProductViewFieldsDisable()
		{
			return GetProductFieldsDisable().Union(GetProductVariationFieldsDisable()).ToList();
		}

		/// <summary>
		/// Get list field of ProductStock which option is false
		/// </summary>
		/// <returns> List field</returns>
		public static List<string> GetProductStockFieldsDisable()
		{
			List<string> fieldsResult = new List<string>();

			if (Constants.REALSTOCK_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK);
				fieldsResult.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B);
				fieldsResult.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C);
				fieldsResult.Add(Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED);
			}

			return fieldsResult;
		}

		/// <summary>
		/// Get list field of ProductCategoty which option is false
		/// </summary>
		/// <returns> List field</returns>
		public static List<string> GetProductCategoryFieldsDisable()
		{
			List<string> fieldsResult = new List<string>();

			if (Constants.MEMBER_RANK_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCTCATEGORY_MEMBER_RANK_ID);
				fieldsResult.Add(Constants.FIELD_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG);
			}

			if (Constants.PRODUCT_BRAND_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCTCATEGORY_PERMITTED_BRAND_IDS);
			}

			if (Constants.MOBILEOPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCTCATEGORY_NAME_MOBILE);
				fieldsResult.Add(Constants.FIELD_PRODUCTCATEGORY_MOBILE_DISP_FLG);
			}

			if (Constants.RECOMMEND_ENGINE_KBN != Constants.RecommendEngine.Silveregg)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCTCATEGORY_USE_RECOMMEND_FLG);
			}

			if (Constants.SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG);
			}

			return fieldsResult;
		}

		/// <summary>
		/// Get list field of Order which option is false
		/// </summary>
		/// <returns> List field</returns>
		public static List<string> GetOrderFieldsDisable()
		{
			List<string> fieldsResult = new List<string>();

			if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG);
			}

			if (Constants.FIXEDPURCHASE_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_ID);
				fieldsResult.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT);
				fieldsResult.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT);
				fieldsResult.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT);
				fieldsResult.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_DISCOUNT_PRICE);
			}

			if (Constants.GIFTORDER_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_GIFT_FLG);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ZIP);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SENDER_TEL1);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE);
			}

			if (Constants.MEMBER_RANK_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_MEMBER_RANK_ID);
			}

			if (Constants.MOBILEOPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_CAREER_ID);
				fieldsResult.Add(Constants.FIELD_ORDER_MOBILE_UID);
			}

			if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2);
			}

			if (Constants.W2MP_AFFILIATE_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_ADVCODE_FIRST);
				fieldsResult.Add(Constants.FIELD_ORDER_ADVCODE_NEW);
				fieldsResult.Add(Constants.FIELD_ORDER_AFFILIATE_SESSION_NAME1);
				fieldsResult.Add(Constants.FIELD_ORDER_AFFILIATE_SESSION_VALUE1);
				fieldsResult.Add(Constants.FIELD_ORDER_AFFILIATE_SESSION_NAME2);
				fieldsResult.Add(Constants.FIELD_ORDER_AFFILIATE_SESSION_VALUE2);
			}

			if (Constants.W2MP_COUPON_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_ORDER_COUPON_USE);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_ORDER_COUPON_NO);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_DEPT_ID);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_COUPON_ID);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_COUPON_NO);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_COUPON_CODE);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_COUPON_NAME);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_COUPON_DISP_NAME);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_COUPON_TYPE);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_PRICE);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE);
			}

			if (Constants.W2MP_POINT_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_ORDER_POINT_USE);
				fieldsResult.Add(Constants.FIELD_ORDER_ORDER_POINT_USE_YEN);
				fieldsResult.Add(Constants.FIELD_ORDER_ORDER_POINT_ADD);
				fieldsResult.Add(Constants.FIELD_ORDER_LAST_ORDER_POINT_USE);
				fieldsResult.Add(Constants.FIELD_ORDER_LAST_ORDER_POINT_USE_YEN);
			}

			if (Constants.REALSTOCK_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS);
				fieldsResult.Add(Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE);
			}

			if (Constants.MALLCOOPERATION_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_MALL_ID);
			}

			if (Constants.SETPROMOTION_OPTION_ENABLED == false)
			{
				fieldsResult.Add(FIELD_ORDER_SETPROMOTION_PRODUCT_DISCOUNT_AMOUNT_TOTAL);
				fieldsResult.Add(FIELD_ORDER_SETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT_TOTAL);
				fieldsResult.Add(FIELD_ORDER_SETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT_TOTAL);
			}

			if (Constants.DISPLAY_CORPORATION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME);
			}

			return fieldsResult;
		}

		/// <summary>
		/// Get list field of OrderItem which option is false
		/// </summary>
		/// <returns> List field</returns>
		public static List<string> GetOrderItemFieldsDisable()
		{
			List<string> fieldsResult = new List<string>();

			if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG);
				fieldsResult.Add(Constants.FIELD_ORDERITEM_DOWNLOAD_URL);
				fieldsResult.Add(FIELD_NAME_SERIAL_KEYS);
			}

			if (Constants.FIXEDPURCHASE_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_ID);
				fieldsResult.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT);
				fieldsResult.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT);
				fieldsResult.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT);
				fieldsResult.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_DISCOUNT_PRICE);
			}

			if (Constants.GIFTORDER_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_GIFT_FLG);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ZIP);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SENDER_TEL1);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE);
			}

			if (Constants.MEMBER_RANK_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_MEMBER_RANK_ID);
			}

			if (Constants.PRODUCT_BRAND_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDERITEM_BRAND_ID);
			}

			if (Constants.MOBILEOPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_CAREER_ID);
				fieldsResult.Add(Constants.FIELD_ORDER_MOBILE_UID);
			}

			if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2);
			}

			if (Constants.W2MP_AFFILIATE_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_ADVCODE_FIRST);
				fieldsResult.Add(Constants.FIELD_ORDER_ADVCODE_NEW);
				fieldsResult.Add(Constants.FIELD_ORDER_AFFILIATE_SESSION_NAME1);
				fieldsResult.Add(Constants.FIELD_ORDER_AFFILIATE_SESSION_VALUE1);
				fieldsResult.Add(Constants.FIELD_ORDER_AFFILIATE_SESSION_NAME2);
				fieldsResult.Add(Constants.FIELD_ORDER_AFFILIATE_SESSION_VALUE2);
			}

			if (Constants.W2MP_COUPON_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_ORDER_COUPON_USE);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_ORDER_COUPON_NO);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_DEPT_ID);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_COUPON_ID);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_COUPON_NO);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_COUPON_CODE);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_COUPON_NAME);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_COUPON_DISP_NAME);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_COUPON_TYPE);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_PRICE);
				fieldsResult.Add(Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE);
			}

			if (Constants.W2MP_POINT_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_ORDER_POINT_USE);
				fieldsResult.Add(Constants.FIELD_ORDER_ORDER_POINT_USE_YEN);
				fieldsResult.Add(Constants.FIELD_ORDER_ORDER_POINT_ADD);
				fieldsResult.Add(Constants.FIELD_ORDER_LAST_ORDER_POINT_USE);
				fieldsResult.Add(Constants.FIELD_ORDER_LAST_ORDER_POINT_USE_YEN);
			}

			if (Constants.REALSTOCK_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS);
				fieldsResult.Add(Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE);
				fieldsResult.Add(Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED);
				fieldsResult.Add(Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_SHIPPED);
			}

			if (Constants.MALLCOOPERATION_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDER_MALL_ID);
			}

			if (Constants.SETPROMOTION_OPTION_ENABLED == false)
			{
				fieldsResult.Add(FIELD_ORDER_SETPROMOTION_PRODUCT_DISCOUNT_AMOUNT_TOTAL);
				fieldsResult.Add(FIELD_ORDER_SETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT_TOTAL);
				fieldsResult.Add(FIELD_ORDER_SETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT_TOTAL);
				fieldsResult.Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO);
				fieldsResult.Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO);
				fieldsResult.Add(Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID);
				fieldsResult.Add(Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME);
				fieldsResult.Add(Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME);
				fieldsResult.Add(FIELD_SETPROMOTION_UNDISCOUNTED_PRODUCT_SUBTOTAL);
				fieldsResult.Add(FIELD_SETPROMOTION_PRODUCT_DISCOUNT_FLG);
				fieldsResult.Add(Constants.FIELD_ORDER_SETPROMOTION_PRODUCT_DISCOUNT_AMOUNT);
				fieldsResult.Add(FIELD_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG);
				fieldsResult.Add(Constants.FIELD_ORDER_SETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT);
				fieldsResult.Add(FIELD_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG);
				fieldsResult.Add(Constants.FIELD_ORDER_SETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT);
			}

			if (Constants.DISPLAY_CORPORATION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME);
				fieldsResult.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME);
			}

			if (Constants.PRODUCT_SET_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDERITEM_PRODUCT_SET_ID);
				fieldsResult.Add(Constants.FIELD_ORDERITEM_PRODUCT_SET_NO);
				fieldsResult.Add(Constants.FIELD_ORDERITEM_PRODUCT_SET_COUNT);
			}

			if (Constants.PRODUCT_SALE_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDERITEM_PRODUCTSALE_ID);
			}

			if (Constants.NOVELTY_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDERITEM_NOVELTY_ID);
			}

			if (Constants.RECOMMEND_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDERITEM_RECOMMEND_ID);
			}

			if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID_WITH_PREFIX);
				fieldsResult.Add(Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX);
			}

			return fieldsResult;
		}

		/// <summary>
		/// Get list field of User which option is false
		/// </summary>
		/// <returns> List field</returns>
		public static List<string> GetUserFieldsDisable()
		{
			List<string> fieldsResult = new List<string>();

			if (Constants.MEMBER_RANK_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_USER_MEMBER_RANK_ID);
				fieldsResult.Add(Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG);
			}

			if (Constants.FIXEDPURCHASE_OPTION_ENABLED == false)
			{
				if (fieldsResult.Contains(Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG) == false)
				{
					fieldsResult.Add(Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG);
				}
			}

			if (Constants.MOBILEOPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_USER_CAREER_ID);
				fieldsResult.Add(Constants.FIELD_USER_MOBILE_UID);
			}

			if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_USER_MAIL_ADDR2);
			}

			if (Constants.W2MP_AFFILIATE_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_USER_ADVCODE_FIRST);
			}

			if (Constants.MALLCOOPERATION_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_USER_MALL_ID);
			}

			if (Constants.DISPLAY_CORPORATION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_USER_COMPANY_NAME);
				fieldsResult.Add(Constants.FIELD_USER_COMPANY_POST_NAME);
			}

			if (Constants.USERINTEGRATION_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_USER_INTEGRATED_FLG);
			}

			if (Constants.PAYMENT_GMO_POST_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_USER_BUSINESS_OWNER_NAME1);
				fieldsResult.Add(Constants.FIELD_USER_BUSINESS_OWNER_NAME2);
				fieldsResult.Add(Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA1);
				fieldsResult.Add(Constants.FIELD_USER_BUSINESS_OWNER_NAME_KANA2);
				fieldsResult.Add(Constants.FIELD_USER_BUSINESS_OWNER_BIRTH);
				fieldsResult.Add(Constants.FIELD_USER_BUSINESS_OWNER_REQUEST_BUDGET);
			}

			return fieldsResult;
		}

		/// <summary>
		/// Get list field of ProductPrice which option is false
		/// </summary>
		/// <returns> List field</returns>
		public static List<string> GetProductPriceFieldsDisable()
		{
			List<string> fieldsResult = new List<string>();

			if (Constants.MEMBER_RANK_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_ID);
				fieldsResult.Add(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE);
			}

			return fieldsResult;
		}

		/// <summary>
		/// Get list field of RealShop which option is false
		/// </summary>
		/// <returns> List field</returns>
		public static List<string> GetRealShopFieldsDisable()
		{
			List<string> fieldsResult = new List<string>();

			if (Constants.SMARTPHONE_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_REALSHOP_DESC1_SP);
				fieldsResult.Add(Constants.FIELD_REALSHOP_DESC2_SP);
				fieldsResult.Add(Constants.FIELD_REALSHOP_DESC1_KBN_SP);
				fieldsResult.Add(Constants.FIELD_REALSHOP_DESC2_KBN_SP);
			}

			return fieldsResult;
		}

		/// <summary>
		/// Get list field of UserProductArrivalMailDetail which option is false
		/// </summary>
		/// <returns> List field</returns>
		public static List<string> GetUserProductArrivalMailDetailFieldsDisable()
		{
			List<string> fieldsResult = new List<string>();

			if (Constants.MOBILEOPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN);
			}

			return fieldsResult;
		}

		/// <summary>
		/// Get list field of TargetListData which option is false
		/// </summary>
		/// <returns> List field</returns>
		public static List<string> GetTargetListDataFieldsDisable()
		{
			List<string> fieldsResult = new List<string>();

			if (Constants.MOBILEOPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_USER_CAREER_ID);
				fieldsResult.Add(Constants.FIELD_USER_MOBILE_UID);
			}

			if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_USER_MAIL_ADDR2);
			}

			return fieldsResult;
		}

		/// <summary>
		/// Get list field of Coupon which option is false
		/// </summary>
		/// <returns> List field</returns>
		public static List<string> GetCouponFieldsDisable()
		{
			List<string> fieldsResult = new List<string>();

			if (Constants.MOBILEOPTION_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_COUPON_COUPON_DISP_NAME_MOBILE);
				fieldsResult.Add(Constants.FIELD_COUPON_COUPON_DISCRIPTION_MOBILE);
			}

			if (Constants.PRODUCT_BRAND_ENABLED == false)
			{
				fieldsResult.Add(Constants.FIELD_COUPON_EXCEPTIONAL_BRAND_IDS);
			}

			return fieldsResult;
		}

		/// <summary>
		/// Get all master field export setting after remove files which option is false
		/// </summary>
		/// <param name="masterFields">All field</param>
		/// <param name="masterKbn">Master classification</param>
		/// <returns>All field</returns>
		public static List<Hashtable> RemoveMasterFields(List<Hashtable> masterFields, string masterKbn)
		{
			switch (masterKbn)
			{
				// Remove field in case export Product
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCT:
					RemoveData(masterFields, GetProductFieldsDisable());
					break;

				// Remove field in case export ProductVariation
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTVARIATION:
					RemoveData(masterFields, GetProductVariationFieldsDisable());
					break;

				// Remove field in case export ProducView
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTVIEW:
					RemoveData(masterFields, GetProductViewFieldsDisable());
					break;

				// Remove field in case export ProductStock
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTSTOCK:
					RemoveData(masterFields, GetProductStockFieldsDisable());
					break;

				// Remove field in case export ProductCategory
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTCATEGORY:
					RemoveData(masterFields, GetProductCategoryFieldsDisable());
					break;

				// Remove field in case export Order
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER:
					RemoveData(masterFields, GetOrderFieldsDisable());
					break;

				// Remove field in case export OrderItem
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM:
					RemoveData(masterFields, GetOrderItemFieldsDisable());
					break;

				// Remove field in case export User
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER:
					RemoveData(masterFields, GetUserFieldsDisable());
					break;

				// Remove field in case export ProductPrice
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTPRICE:
					RemoveData(masterFields, GetProductPriceFieldsDisable());
					break;

				// Remove field in case export RealShop
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_REALSHOP:
					RemoveData(masterFields, GetRealShopFieldsDisable());
					break;

				// Remove field in case export UserProductArrivalMailDetail
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPRODUCTARRIVALMAIL_DETAIL:
					RemoveData(masterFields, GetUserProductArrivalMailDetailFieldsDisable());
					break;

				// Remove field in case export TargetListData
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_TARGETLISTDATA:
					RemoveData(masterFields, GetTargetListDataFieldsDisable());
					break;

				// Remove field in case export Coupon
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON:
					RemoveData(masterFields, GetCouponFieldsDisable());
					break;
			}

			return masterFields;
		}

		/// <summary>
		/// Remove fields which option is false
		/// </summary>
		/// <param name="masterFields">All field</param>
		/// <param name="fieldRemoves"> List field remove</param>
		private static void RemoveData(List<Hashtable> masterFields, List<string> fieldRemoves)
		{
			foreach (var field in fieldRemoves)
			{
				RemoveObjectFromList(masterFields, field);
			}
		}

		/// <summary>
		/// Remove object from list
		/// </summary>
		/// <param name="listInput">List input</param>
		/// <param name="objectName">Object will be removed</param>
		/// <returns>List object after remove objectName</returns>
		private static void RemoveObjectFromList(List<Hashtable> listInput, string objectName)
		{
			listInput.Remove(listInput.FirstOrDefault(p => p["name"].ToString() == objectName));
		}
	}
}
