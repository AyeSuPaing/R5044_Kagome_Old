/*
=========================================================================================================
  Module      : 注文セットプロモーション情報クラス(OrderSetPromotion.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;

namespace w2.App.Common.Order
{
	/// <summary>
	/// OrderSetPromotion の概要の説明です
	/// </summary>
	[Serializable]
	public class OrderSetPromotion
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="orderSetPromotion">注文セットプロモーション情報</param>
		public OrderSetPromotion(Hashtable orderSetPromotion)
		{
			this.Values = orderSetPromotion;
			this.IsDelete = false;
		}

		#region プロパティ
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string) this.Values[Constants.FIELD_ORDERSETPROMOTION_ORDER_ID]; }
			set { this.Values[Constants.FIELD_ORDERSETPROMOTION_ORDER_ID] = value; }
		}
		/// <summary>注文セットプロモーション枝番</summary>
		public string OrderSetPromotionNo
		{
			get { return this.Values[Constants.FIELD_ORDERSETPROMOTION_ORDER_SETPROMOTION_NO].ToString(); }
			set { this.Values[Constants.FIELD_ORDERSETPROMOTION_ORDER_SETPROMOTION_NO] = value; }
		}
		/// <summary>セットプロモーションID</summary>
		public string SetPromotionId
		{
			get { return (string) this.Values[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID]; }
			set { this.Values[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID] = value; }
		}
		/// <summary>セットプロモーション名</summary>
		public string SetPromotionName
		{
			get { return (string) this.Values[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME]; }
			set { this.Values[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME] = value; }
		}
		/// <summary>セットプロモーション表示名</summary>
		public string SetPromotionDispName
		{
			get { return (string) this.Values[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME]; }
			set { this.Values[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME] = value; }
		}
		/// <summary>割引前商品小計</summary>
		public decimal UndiscountedProductSubtotal
		{
			get { return (decimal) this.Values[Constants.FIELD_ORDERSETPROMOTION_UNDISCOUNTED_PRODUCT_SUBTOTAL]; }
			set { this.Values[Constants.FIELD_ORDERSETPROMOTION_UNDISCOUNTED_PRODUCT_SUBTOTAL] = value; }
		}
		/// <summary>プロモーション種別：商品金額割引</summary>
		public bool IsDiscountTypeProductDiscount
		{
			get
			{
				return ((string) this.Values[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG] ==
				        Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON);
			}
			set
			{
				this.Values[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG] = (value
					? Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON
					: Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_OFF);
			}
		}
		/// <summary>商品割引額</summary>
		public decimal ProductDiscountAmount
		{
			get { return (decimal) this.Values[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT]; }
			set { this.Values[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT] = value; }
		}
		/// <summary>プロモーション種別：配送料無料</summary>
		public bool IsDiscountTypeShippingChargeFree
		{
			get
			{
				return ((string) this.Values[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG] ==
				        Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON);
			}
			set
			{
				this.Values[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG] = (value
					? Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON
					: Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_OFF);
			}
		}
		/// <summary>配送料割引額</summary>
		public decimal ShippingChargeDiscountAmount
		{
			get { return (decimal) this.Values[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT]; }
			set { this.Values[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT] = value; }
		}
		/// <summary>プロモーション種別：決済手数料無料</summary>
		public bool IsDiscountTypePaymentChargeFree
		{
			get
			{
				return ((string) this.Values[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG] ==
				        Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON);
			}
			set
			{
				this.Values[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG] = (value
					? Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON
					: Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_OFF);
			}
		}
		/// <summary>決済手数料割引額</summary>
		public decimal PaymentChargeDiscountAmount
		{
			get { return (decimal) this.Values[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT]; }
			set { this.Values[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT] = value; }
		}
		/// <summary>削除対象かどうか</summary>
		public bool IsDelete
		{
			get { return (bool) this.Values["is_delete"]; }
			set { this.Values["is_delete"] = value; }
		}
		/// <summary>値セット</summary>
		public Hashtable Values { get; set; }
		#endregion
	}
}