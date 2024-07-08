/*
=========================================================================================================
  Module      : カートセットプロモーション情報クラス(CartSetPromotion.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.Domain.SetPromotion;

namespace w2.App.Common.Order
{
	/// <summary>
	/// カートセットプロモーション情報クラス
	/// </summary>
	[Serializable]
	public class CartSetPromotion : SetPromotionModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="model">セットプロモーションモデル</param>
		/// <param name="items">構成商品</param>
		public CartSetPromotion(CartObject cart, SetPromotionModel model, List<Item> items)
		{
			this.Cart = cart;
			this.CartSetPromotionNo = 1;
			this.DataSource = model.DataSource;
			this.tempItems = items;
			this.SetCount = 0;
			this.SetpromotionDispName = model.SetpromotionDispName;
			this.SetPromotionId = model.SetpromotionId;

			Calculate();
		}

		/// <summary>
		/// 商品金額計算
		/// </summary>
		private void Calculate()
		{
			// 割引前金額設定
			var isDutyFree = this.Cart.Shippings[0].IsDutyFree;
			this.UnitUndiscountedProductSubtotal = this.tempItems
				.Where(item => item.Product.IsSubscriptionBoxFixedAmount() == false)
				.Sum(item =>
					(isDutyFree ? item.Product.Price + item.Product.TotalOptionPrice : item.Product.PricePretax + item.Product.OptionPricePretax) * item.Quantity);

			// 割引額設定
			if (this.IsDiscountTypeProductDiscount == false)
			{
				this.UnitProductDiscountAmount = 0;
			}
			else
			{
				decimal tempUnitProductDiscountAmount = 0;
				switch (this.ProductDiscountKbn)
				{
					// 割引後金額指定
					case Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNTED_PRICE:
						// 割引前金額 - 設定値（セット数分考慮する）
						tempUnitProductDiscountAmount = this.UnitUndiscountedProductSubtotal - ((decimal)this.ProductDiscountSetting);
						break;

					// 割引額指定
					case Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_PRICE:
						// 設定値そのまま（セット数分考慮する）
						tempUnitProductDiscountAmount = (decimal)this.ProductDiscountSetting;
						break;

					// 割引率指定
					case Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_RATE:
						tempUnitProductDiscountAmount = RoundingCalculationUtility.GetRoundPercentDiscountFraction(this.UnitUndiscountedProductSubtotal, ((decimal)this.ProductDiscountSetting));
						break;

					default:
						tempUnitProductDiscountAmount = 0;
						break;
				}

				// 割引額の最大は商品合計額。最小は0円。
				if (tempUnitProductDiscountAmount < 0)
				{
					this.UnitProductDiscountAmount = 0;
				}
				else if (tempUnitProductDiscountAmount <= this.UnitUndiscountedProductSubtotal)
				{
					this.UnitProductDiscountAmount = tempUnitProductDiscountAmount;
				}
				else
				{
					this.UnitProductDiscountAmount = this.UnitUndiscountedProductSubtotal;
				}
			}
		}

		/// <summary>
		/// シャローコピー
		/// </summary>
		public new CartSetPromotion Clone()
		{
			return (CartSetPromotion)this.MemberwiseClone();
		}

		/// <summary>
		/// 表示用セットプロモーション名翻訳名称取得
		/// </summary>
		/// <returns>表示用セットプロモーション名翻訳名称</returns>
		private string GetSetPromotionDispNameTranslationName()
		{
			var beforeTranslationSetPromotionDispName = (this.BeforeTranslationData != null)
				? this.BeforeTranslationData.SetPromotionDispName
				: base.SetpromotionDispName;

			var setPromotionTranslationName = NameTranslationCommon.GetTranslationName(
				this.SetpromotionId,
				Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION,
				Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SETPROMOTION_SETPROMOTION_DISP_NAME,
				beforeTranslationSetPromotionDispName);

			return setPromotionTranslationName;
		}

		#region プロパティ

		/// <summary>カートオブジェクト</summary>
		public CartObject Cart { get; private set; }
		/// <summary>カートセットプロモーション枝番</summary>
		public int CartSetPromotionNo { get; set; }
		/// <summary>単位 割引前商品小計</summary>
		public decimal UnitUndiscountedProductSubtotal { get; private set; }
		/// <summary>割引前商品小計</summary>
		public decimal UndiscountedProductSubtotal { get { return this.UnitUndiscountedProductSubtotal * this.SetCount; } }
		/// <summary>単位 商品割引額</summary>
		public decimal UnitProductDiscountAmount { get; private set; }
		/// <summary>商品割引額</summary>
		public decimal ProductDiscountAmount { get { return this.UnitProductDiscountAmount * this.SetCount; } }
		/// <summary>配送料割引額</summary>
		public decimal ShippingChargeDiscountAmount { get; set; }
		/// <summary>決済手数料割引額</summary>
		public decimal PaymentChargeDiscountAmount { get; set; }
		/// <summary>セットプロモーションID</summary>
		public string SetPromotionId { get; set; }
		/// <summary>セット数</summary>
		private int m_setCount = 0;
		public int SetCount
		{
			get { return m_setCount; }
			set { this.m_setCount = value; }
		}
		/// <summary>セットプロモーションアイテム</summary>
		public new List<CartProduct> Items
		{
			get
			{
				return Cart.Items.Where(item => item.QuantityAllocatedToSet.ContainsKey(this.CartSetPromotionNo)).ToList();
			}
		}
		/// <summary>セットプロモーションアイテム(計算用)</summary>
		public List<Item> tempItems { get; private set; }

		/// <summary>表示用セットプロモーション名</summary>
		public new string SetpromotionDispName
		{
			get
			{
				if (Constants.GLOBAL_OPTION_ENABLE == false) return m_setPromotionDispName;
				return GetSetPromotionDispNameTranslationName();
			}
			private set { m_setPromotionDispName = value; }
		}
		private string m_setPromotionDispName;
		/// <summary>セットプロモーション期間を表示するか</summary>
		public bool IsDispSetPromotionTerm
		{
			get
			{
				return (string.IsNullOrEmpty(this.SetPromotionId) == false)
					&& Constants.CORRESPONDENCE_SPECIFIEDCOMMERCIALTRANSACTIONS_ENABLE;
			}
		}
		/// <summary>全ての商品が頒布会定額コース商品か</summary>
		public bool IsAllItemsSubscriptionBoxFixedAmount
		{
			get
			{
				var hasItemsNotFixedAmount = this.Items
					.Any(item => item.IsSubscriptionBoxFixedAmount() == false);
				return hasItemsNotFixedAmount == false;
			}
		}
		#endregion

		/// <summary>
		/// カートセットプロモーションアイテムクラス(セットプロモーション計算用)	
		/// </summary>
		[Serializable]
		public class Item
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="cartProduct">カート商品</param>
			/// <param name="quantity">数量</param>
			public Item(CartProduct cartProduct, int quantity)
			{
				this.Product = cartProduct;
				this.Quantity = quantity;
				this.AllocatedQuantity = 0;
			}

			/// <summary>
			/// 引当
			/// </summary>
			/// <param name="numberOfAllocation">引当数</param>
			public void Allocate(int numberOfAllocation)
			{
				this.AllocatedQuantity += numberOfAllocation;
			}

			/// <summary>
			/// 引当状態リセット
			/// </summary>
			public void ResetAllocation()
			{
				this.AllocatedQuantity = 0;
			}

			/// <summary>商品</summary>
			public CartProduct Product { get; set; }
			/// <summary>数量</summary>
			public int Quantity { get; set; }
			/// <summary>セット引当数量</summary>
			public int AllocatedQuantity { get; private set; }
			/// <summary>未引当数量</summary>
			public int UnallocatedQuantity { get { return this.Quantity - this.AllocatedQuantity; } }
			/// <summary>引当可能な数量があるか</summary>
			public bool HasRequiredQuantity { get { return this.Quantity >= this.AllocatedQuantity; } }
		}
	}
}
