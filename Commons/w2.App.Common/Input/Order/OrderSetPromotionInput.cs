/*
=========================================================================================================
  Module      : 注文セットプロモーションテーブル入力クラス (OrderSetPromotionInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Domain.Order;

namespace w2.App.Common.Input.Order
{
	/// <summary>
	/// 注文セットプロモーションテーブル入力クラス（登録、編集で利用）
	/// </summary>
	[Serializable]
	public class OrderSetPromotionInput : InputBase<OrderSetPromotionModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderSetPromotionInput()
		{
			this.OrderId = string.Empty;
			this.OrderSetpromotionNo = "1";
			this.SetpromotionId = String.Empty;
			this.SetpromotionName = string.Empty;
			this.SetpromotionDispName = string.Empty;
			this.UndiscountedProductSubtotal = "0";
			this.ProductDiscountFlg = Constants.FLG_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG_OFF;
			this.ProductDiscountAmount = "0";
			this.ShippingChargeFreeFlg = Constants.FLG_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG_OFF;
			this.ShippingChargeDiscountAmount = "0";
			this.PaymentChargeFreeFlg = Constants.FLG_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG_OFF;
			this.PaymentChargeDiscountAmount = "0";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public OrderSetPromotionInput(OrderSetPromotionModel model)
			: this()
		{
			this.OrderId = model.OrderId;
			this.OrderSetpromotionNo = model.OrderSetpromotionNo.ToString();
			this.SetpromotionId = model.SetpromotionId;
			this.SetpromotionName = model.SetpromotionName;
			this.SetpromotionDispName = model.SetpromotionDispName;
			this.UndiscountedProductSubtotal = model.UndiscountedProductSubtotal.ToString();
			this.ProductDiscountFlg = model.ProductDiscountFlg;
			this.ProductDiscountAmount = model.ProductDiscountAmount.ToString();
			this.ShippingChargeFreeFlg = model.ShippingChargeFreeFlg;
			this.ShippingChargeDiscountAmount = model.ShippingChargeDiscountAmount.ToString();
			this.PaymentChargeFreeFlg = model.PaymentChargeFreeFlg;
			this.PaymentChargeDiscountAmount = model.PaymentChargeDiscountAmount.ToString();
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override OrderSetPromotionModel CreateModel()
		{
			var model = new OrderSetPromotionModel
			{
				OrderId = this.OrderId,
				OrderSetpromotionNo = int.Parse(this.OrderSetpromotionNo),
				SetpromotionId = this.SetpromotionId,
				SetpromotionName = this.SetpromotionName,
				SetpromotionDispName = this.SetpromotionDispName,
				UndiscountedProductSubtotal = decimal.Parse(this.UndiscountedProductSubtotal),
				ProductDiscountFlg = this.ProductDiscountFlg,
				ProductDiscountAmount = decimal.Parse(this.ProductDiscountAmount),
				ShippingChargeFreeFlg = this.ShippingChargeFreeFlg,
				ShippingChargeDiscountAmount = decimal.Parse(this.ShippingChargeDiscountAmount),
				PaymentChargeFreeFlg = this.PaymentChargeFreeFlg,
				PaymentChargeDiscountAmount = decimal.Parse(this.PaymentChargeDiscountAmount),
			};
			return model;
		}

		/// <summary>
		/// 検証 ※利用しない
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate()
		{
			return string.Empty;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_ORDER_ID] = value; }
		}
		/// <summary>注文セットプロモーション枝番</summary>
		public string OrderSetpromotionNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_ORDER_SETPROMOTION_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_ORDER_SETPROMOTION_NO] = value; }
		}
		/// <summary>セットプロモーションID</summary>
		public string SetpromotionId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID] = value; }
		}
		/// <summary>管理用セットプロモーション名</summary>
		public string SetpromotionName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME] = value; }
		}
		/// <summary>表示用セットプロモーション名</summary>
		public string SetpromotionDispName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME] = value; }
		}
		/// <summary>割引前商品小計</summary>
		public string UndiscountedProductSubtotal
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_UNDISCOUNTED_PRODUCT_SUBTOTAL]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_UNDISCOUNTED_PRODUCT_SUBTOTAL] = value; }
		}
		/// <summary>商品金額割引フラグ</summary>
		public string ProductDiscountFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG] = value; }
		}
		/// <summary>商品割引額</summary>
		public string ProductDiscountAmount
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT] = value; }
		}
		/// <summary>配送料無料フラグ</summary>
		public string ShippingChargeFreeFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG] = value; }
		}
		/// <summary>配送料割引額</summary>
		public string ShippingChargeDiscountAmount
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT] = value; }
		}
		/// <summary>決済手数料無料フラグ</summary>
		public string PaymentChargeFreeFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG] = value; }
		}
		/// <summary>決済手数料割引額</summary>
		public string PaymentChargeDiscountAmount
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT] = value; }
		}
		/// <summary>商品金額割引か</summary>
		public bool IsDiscountTypeProductDiscount
		{
			get { return (this.ProductDiscountFlg == Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON); }
			set { this.ProductDiscountFlg = (value ? Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON : Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_OFF); }
		}
		/// <summary>配送料無料か</summary>
		public bool IsDiscountTypeShippingChargeFree
		{
			get { return (this.ShippingChargeFreeFlg == Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON); }
			set { this.ShippingChargeFreeFlg = (value ? Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON : Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_OFF); }
		}
		/// <summary>決済手数料無料か</summary>
		public bool IsDiscountTypePaymentChargeFree
		{
			get { return (this.PaymentChargeFreeFlg == Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON); }
			set { this.PaymentChargeFreeFlg = (value ? Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON : Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_OFF); }
		}
		#endregion
	}
}
