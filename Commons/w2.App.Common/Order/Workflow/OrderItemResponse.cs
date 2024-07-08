/*
=========================================================================================================
  Module      : Order item response(OrderItemResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.Common.Util;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// Order item response
	/// </summary>
	[Serializable]
	public class OrderItemResponse
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="item">Item</param>
		/// <param name="type">Type</param>
		public OrderItemResponse(Hashtable item, string type)
		{
			if (type == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
			{
				this.OrderItemNo = (int)item[Constants.FIELD_ORDERITEM_ORDER_ITEM_NO];
				this.ProductId = StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCT_ID]);
				this.VariationId = StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_VARIATION_ID]);
				this.ProductName = StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCT_NAME]);
				this.ProductPrice = StringUtility.ToPrice(item[Constants.FIELD_ORDERITEM_PRODUCT_PRICE]);
				this.ItemQuantity = (int)item[Constants.FIELD_ORDERITEM_ITEM_QUANTITY];
				this.ItemPrice = StringUtility.ToPrice(item[Constants.FIELD_ORDERITEM_ITEM_PRICE]);
			}
			else
			{
				this.OrderItemNo = (int)item[Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ITEM_NO];
				this.ProductId = StringUtility.ToEmpty(item[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_ID]);
				this.VariationId = StringUtility.ToEmpty(item[Constants.FIELD_FIXEDPURCHASEITEM_VARIATION_ID]);
				this.ProductName = StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCT_NAME]);
				this.ProductPrice = StringUtility.ToPrice((decimal)item[Constants.FIELD_PRODUCTVARIATION_PRICE]);
				this.ItemQuantity = (int)item[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY];
				var itemPrice = (decimal)item[Constants.FIELD_PRODUCTVARIATION_PRICE]
					* (int)item[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY];
				this.ItemPrice = StringUtility.ToPrice(itemPrice);
			}
		}

		/// <summary>Order item no</summary>
		public int OrderItemNo { get; set; }
		/// <summary>Product id</summary>
		public string ProductId { get; set; }
		/// <summary>Variation id</summary>
		public string VariationId { get; set; }
		/// <summary>Product name</summary>
		public string ProductName { get; set; }
		/// <summary>Product price</summary>
		public string ProductPrice { get; set; }
		/// <summary>Item quantity</summary>
		public int ItemQuantity { get; set; }
		/// <summary>Item price</summary>
		public string ItemPrice { get; set; }
	}
}
