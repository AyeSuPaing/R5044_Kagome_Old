/*
=========================================================================================================
  Module      : 注文モデル作成クラス(CreateModelOrder.cs)
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
	/// 注文モデル作成
	/// </summary>
	public class CreateModelOrder : CreateModelBase
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
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_ORDER_DATE);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_ORDER_RECOGNITION_DATE);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_ORDER_SHIPPING_DATE);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_ORDER_SHIPPED_DATE);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_ORDER_DELIVERING_DATE);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_ORDER_CANCEL_DATE);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_ORDER_RETURN_DATE);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_DATE);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_DEMAND_DATE);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_ARRIVAL_DATE);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_COMPLETE_DATE);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_ORDER_REPAYMENT_DATE);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE1);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE2);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE3);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE4);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE5);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE6);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE7);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE8);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE9);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE10);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE11);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE12);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE13);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE14);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE15);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE16);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE17);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE18);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE19);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE20);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE21);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE22);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE23);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE24);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE25);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE26);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE27);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE28);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE29);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE30);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE31);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE32);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE33);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE34);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE35);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE36);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE37);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE38);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE39);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE40);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_STOREPICKUP_STORE_ARRIVED_DATE);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_STOREPICKUP_DELIVERED_COMPLETE_DATE);
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDER_STOREPICKUP_RETURN_DATE);
		}

		/// <summary>
		/// Decimal型リスト作成
		/// </summary>
		internal override void CreateListDecimal()
		{
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_ORDER_PRICE_PACK);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_ORDER_PRICE_TAX);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_ORDER_PRICE_REGULATION);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_ORDER_PRICE_REPAYMENT);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_ORDER_PRICE_TOTAL);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_ORDER_DISCOUNT_SET_PRICE);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_ORDER_POINT_USE);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_ORDER_POINT_USE_YEN);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_ORDER_POINT_ADD);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_ORDER_POINT_RATE);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_ORDER_COUPON_USE);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_MEMBER_RANK_DISCOUNT_PRICE);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_ORDER_TAX_RATE);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_SETPROMOTION_PRODUCT_DISCOUNT_AMOUNT);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_SETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_SETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_DISCOUNT_PRICE);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_LAST_BILLED_AMOUNT);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_LAST_ORDER_POINT_USE);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_LAST_ORDER_POINT_USE_YEN);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_SHIPPING_TAX_RATE);
			this.m_FieldsDecimal.Add(Constants.FIELD_ORDER_PAYMENT_TAX_RATE);
		}

		/// <summary>
		/// Decimal型(Null許可)リスト作成
		/// </summary>
		internal override void CreateListDecimalNullable()
		{
			this.m_FieldsDecimalNullable.Add(Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE);
			this.m_FieldsDecimalNullable.Add(Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_PRICE);
		}

		/// <summary>
		/// Int型リスト作成
		/// </summary>
		internal override void CreateListInt()
		{
			this.m_FieldsInt.Add(Constants.FIELD_ORDER_ORDER_ITEM_COUNT);
			this.m_FieldsInt.Add(Constants.FIELD_ORDER_ORDER_PRODUCT_COUNT);
		}

		/// <summary>
		/// Int型(Null許可)リスト作成
		/// </summary>
		internal override void CreateListIntNullable()
		{
			this.m_FieldsIntNullable.Add(Constants.FIELD_ORDER_CREDIT_BRANCH_NO);
			this.m_FieldsIntNullable.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT);
			this.m_FieldsIntNullable.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT);
		}
	}
}
