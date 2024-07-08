/*
=========================================================================================================
  Module      : 注文商品モデル作成クラス(CreateModelOrderItem.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Domain;

namespace w2.App.Common.Order.Import.OrderImport.CreateModel
{
	/// <summary>
	/// 注文商品モデル作成
	/// </summary>
	public class CreateModelOrderItem : CreateModelBase
	{
		/// <summary>
		/// 日付型リスト作成
		/// </summary>
		internal override void CreateListDatetime()
		{
			// なし
		}

		/// <summary>
		/// 日付型(Null許可)リスト作成
		/// </summary>
		internal override void CreateListDatetimeNullable()
		{
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDERITEM_ITEM_CANCEL_DATE);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDERITEM_ITEM_RETURN_DATE);
		}

		/// <summary>
		/// Decimal型リスト作成
		/// </summary>
		internal override void CreateListDecimal()
		{
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE_ORG);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDERITEM_PRODUCT_POINT);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE_TAX);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE_PRETAX);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE_SHIP);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE_COST);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDERITEM_ITEM_POINT);
		}

		/// <summary>
		/// Decimal型(Null許可)リスト作成
		/// </summary>
		internal override void CreateListDecimalNullable()
		{
			this.m_FieldsDecimalNullable.Add(Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_VALUE);
		}

		/// <summary>
		/// Int型リスト作成
		/// </summary>
		internal override void CreateListInt()
		{
			this.m_FieldsInt.Add(Constants.FIELD_ORDERITEM_ORDER_ITEM_NO);
			this.m_FieldsInt.Add(Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO);
			this.m_FieldsInt.Add(Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED);
			this.m_FieldsInt.Add(Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_SHIPPED);
			this.m_FieldsInt.Add(Constants.FIELD_ORDERITEM_ITEM_QUANTITY);
			this.m_FieldsInt.Add(Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE);
		}

		/// <summary>
		/// Int型(Null許可)リスト作成
		/// </summary>
		internal override void CreateListIntNullable()
		{
			this.m_FieldsIntNullable.Add(Constants.FIELD_ORDERITEM_PRODUCT_SET_NO);
			this.m_FieldsIntNullable.Add(Constants.FIELD_ORDERITEM_PRODUCT_SET_COUNT);
			this.m_FieldsIntNullable.Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO);
			this.m_FieldsIntNullable.Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO);
		}
	}
}
