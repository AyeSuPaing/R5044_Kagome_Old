/*
=========================================================================================================
  Module      : 注文セットプロモーションテーブルモデル (OrderSetPromotionModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Order
{
	/// <summary>
	/// 注文セットプロモーションテーブルモデル
	/// </summary>
	[Serializable]
	public partial class OrderSetPromotionModel : ModelBase<OrderSetPromotionModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderSetPromotionModel()
		{
			this.OrderSetpromotionNo = 1;
			this.UndiscountedProductSubtotal = 0;
			this.ProductDiscountFlg = Constants.FLG_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG_OFF;
			this.ProductDiscountAmount = 0;
			this.ShippingChargeFreeFlg = Constants.FLG_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG_OFF;
			this.ShippingChargeDiscountAmount = 0;
			this.PaymentChargeFreeFlg = Constants.FLG_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG_OFF;
			this.PaymentChargeDiscountAmount = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderSetPromotionModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderSetPromotionModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		[UpdateData(1, "order_id")]
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_ORDER_ID] = value; }
		}
		/// <summary>注文セットプロモーション枝番</summary>
		[UpdateData(2, "order_setpromotion_no")]
		public int OrderSetpromotionNo
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_ORDER_SETPROMOTION_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_ORDER_SETPROMOTION_NO] = value; }
		}
		/// <summary>セットプロモーションID</summary>
		[UpdateData(3, "setpromotion_id")]
		public string SetpromotionId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID] = value; }
		}
		/// <summary>管理用セットプロモーション名</summary>
		[UpdateData(4, "setpromotion_name")]
		public string SetpromotionName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME] = value; }
		}
		/// <summary>表示用セットプロモーション名</summary>
		[UpdateData(5, "setpromotion_disp_name")]
		public string SetpromotionDispName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME] = value; }
		}
		/// <summary>割引前商品小計</summary>
		[UpdateData(6, "undiscounted_product_subtotal")]
		public decimal UndiscountedProductSubtotal
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_UNDISCOUNTED_PRODUCT_SUBTOTAL]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_UNDISCOUNTED_PRODUCT_SUBTOTAL] = value; }
		}
		/// <summary>商品金額割引フラグ</summary>
		[UpdateData(7, "product_discount_flg")]
		public string ProductDiscountFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG] = value; }
		}
		/// <summary>商品割引額</summary>
		[UpdateData(8, "product_discount_amount")]
		public decimal ProductDiscountAmount
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT] = value; }
		}
		/// <summary>配送料無料フラグ</summary>
		[UpdateData(9, "shipping_charge_free_flg")]
		public string ShippingChargeFreeFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG] = value; }
		}
		/// <summary>配送料割引額</summary>
		[UpdateData(10, "shipping_charge_discount_amount")]
		public decimal ShippingChargeDiscountAmount
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT] = value; }
		}
		/// <summary>決済手数料無料フラグ</summary>
		[UpdateData(11, "payment_charge_free_flg")]
		public string PaymentChargeFreeFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG] = value; }
		}
		/// <summary>決済手数料割引額</summary>
		[UpdateData(12, "payment_charge_discount_amount")]
		public decimal PaymentChargeDiscountAmount
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT] = value; }
		}
		#endregion
	}
}
