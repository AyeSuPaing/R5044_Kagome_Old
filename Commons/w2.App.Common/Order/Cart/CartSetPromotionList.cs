/*
=========================================================================================================
  Module      : カートセットプロモーションリストクラス(CartSetPromotionList.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace w2.App.Common.Order
{
	[Serializable]
	public class CartSetPromotionList : IEnumerable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CartSetPromotionList()
		{
			this.Items = new List<CartSetPromotion>();
		}

		/// <summary>
		/// IEnumerable.GetEnumerator()の実装
		/// </summary>
		/// <returns>IEnumerator</returns>
		public IEnumerator GetEnumerator()
		{
			return this.Items.GetEnumerator();
		}

		/// <summary>
		/// セットプロモーション情報クリア
		/// </summary>
		public void Clear()
		{
			this.Items.Clear();
			this.ProductDiscountAmount = 0;
			this.ShippingChargeDiscountAmount = 0;
			this.PaymentChargeDiscountAmount = 0;
		}

		/// <summary>
		/// セットプロモーションリスト追加
		/// </summary>
		/// <param name="cartSetPromotions">カートセットプロモーションリスト</param>
		public void AddSetPromotions(IEnumerable<CartSetPromotion> cartSetPromotions)
		{
			foreach (var cartSetPromotion in cartSetPromotions)
			{
				AddSetPromotion(cartSetPromotion);
			}
		}

		/// <summary>
		/// セットプロモーション追加
		/// </summary>
		/// <param name="cartSetPromotion">カートセットプロモーション</param>
		public void AddSetPromotion(CartSetPromotion cartSetPromotion)
		{
			// カートセットプロモーション枝番
			if (this.Items.Count > 0)
			{
				cartSetPromotion.CartSetPromotionNo = this.Items.Max(setPromotion => setPromotion.CartSetPromotionNo) + 1;
			}
			// 配送料無料設定あり、かつ、他のセットプロモーションですでに配送料無料になっていない場合
			if ((cartSetPromotion.IsDiscountTypeShippingChargeFree) && (this.IsShippingChargeFree == false))
			{
				// 配送料割引額には配送料をセット
				cartSetPromotion.ShippingChargeDiscountAmount = cartSetPromotion.Cart.IsFreeShippingCouponUse()
					|| (cartSetPromotion.Cart.Coupon != null
						&& cartSetPromotion.Cart.Coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID)
						? 0
						: cartSetPromotion.Cart.PriceShipping;
			}
			// 決済手数料無料設定あり、かつ、他のセットプロモーションですでに決済手数料無料になっていない場合
			if ((cartSetPromotion.IsDiscountTypePaymentChargeFree) && (this.IsPaymentChargeFree == false))
			{
				// 決済手数料割引額に決済手数料をセット
				cartSetPromotion.PaymentChargeDiscountAmount = cartSetPromotion.Cart.Payment == null ? 0 : cartSetPromotion.Cart.Payment.PriceExchange;
			}
			this.Items.Add(cartSetPromotion);
			this.ProductDiscountAmount = this.Items.Sum(item => item.ProductDiscountAmount);
			this.ShippingChargeDiscountAmount = this.Items.Sum(item => item.ShippingChargeDiscountAmount);
			this.PaymentChargeDiscountAmount = this.Items.Sum(item => item.PaymentChargeDiscountAmount);
		}

		/// <summary>
		/// シャローコピー
		/// </summary>
		public CartSetPromotionList Clone()
		{
			CartSetPromotionList newCartSetPromotion = (CartSetPromotionList)this.MemberwiseClone();
			newCartSetPromotion.Items = new List<CartSetPromotion>(this.Items);
			return newCartSetPromotion;
		}

		/// <summary>カートセットプロモーションリスト</summary>
		public List<CartSetPromotion> Items { get; set; }
		/// <summary>商品割引額</summary>
		public decimal ProductDiscountAmount { get; private set; }
		/// <summary>配送料無料かどうか</summary>
		public bool IsShippingChargeFree { get { return this.Items.Exists(item => item.IsDiscountTypeShippingChargeFree); } }
		/// <summary>配送料割引額</summary>
		public decimal ShippingChargeDiscountAmount { get; set; }
		/// <summary>決済手数料無料かどうか</summary>
		public bool IsPaymentChargeFree { get { return this.Items.Exists(item => item.IsDiscountTypePaymentChargeFree); } }
		/// <summary>決済手数料割引額</summary>
		public decimal PaymentChargeDiscountAmount { get; set; }
		/// <summary>合計割引額</summary>
		public decimal TotalDiscountAmount { get { return this.ProductDiscountAmount + this.ShippingChargeDiscountAmount + this.PaymentChargeDiscountAmount; } }
	}
}
