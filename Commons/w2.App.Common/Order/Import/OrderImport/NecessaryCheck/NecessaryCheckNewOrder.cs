/*
=========================================================================================================
  Module      : 必須チェック(新規注文)クラス(NecessaryCheckNewOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace w2.App.Common.Order.Import.OrderImport.NecessaryCheck
{
	/// <summary>
	/// 必須チェック(新規注文)
	/// </summary>
	public class NecessaryCheckNewOrder : NecessaryCheckBase
	{
		/// <summary>
		/// 必須項目リスト作成
		/// </summary>
		internal override void CreateListNecessary()
		{
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDER_ORDER_ID);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDER_USER_ID);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDER_ORDER_KBN);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDER_ORDER_STATUS);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDER_ORDER_DATE);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDER_ORDER_PRICE_REGULATION);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDER_ORDER_PRICE_TOTAL);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDER_ORDER_POINT_ADD);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDER_SHIPPING_ID);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDER_SHIPPING_TAX_RATE);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDER_PAYMENT_TAX_RATE);

			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERITEM_ORDER_ITEM_NO);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERITEM_SHOP_ID);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERITEM_PRODUCT_ID);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERITEM_VARIATION_ID);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE);
			//this.FIELDS_NECESSARY.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE_ORG);
			//this.FIELDS_NECESSARY.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE_PRETAX);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERITEM_ITEM_QUANTITY);
			//this.FIELDS_NECESSARY.Add(Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE);
			//this.FIELDS_NECESSARY.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE);

			this.m_FieldsNecessary.Add(Constants.FIELD_ORDEROWNER_OWNER_KBN);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDEROWNER_OWNER_ZIP);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR1);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR2);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR3);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDEROWNER_OWNER_TEL1);

			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1);
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2);
		}
	}
}
