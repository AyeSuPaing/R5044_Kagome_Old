/*
=========================================================================================================
  Module      : 注文情報モデル (OrderModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.Common.Util;
using w2.Domain.Order.Helper;
using w2.Domain.ShopShipping;
using w2.Domain.TwOrderInvoice;

namespace w2.Domain.Order
{
	/// <summary>
	/// 注文情報モデル
	/// </summary>
	public partial class OrderModel
	{
		#region メソッド
		/// <summary>
		/// 住所情報から請求書同梱フラグを判定する
		/// </summary>
		/// <returns>請求書同梱フラグ</returns>
		/// <remarks>モデルのプロパティは変更しない</remarks>
		public string JudgmentInvoiceBundleFlg()
		{
			if ((this.Shippings.Any() == false)
				|| (this.Owner == null))
			{
				return Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			}

			// 氏名、氏名かな、郵便番号、住所、会社名、部署名、電話番号が注文者と配送先とで同じであれば同梱
			var shipping = this.Shippings.First();
			var comparison = new[]
			{
				new[] { this.Owner.OwnerName, shipping.ShippingName },
				new[] { this.Owner.OwnerNameKana, shipping.ShippingNameKana },
				new[] { this.Owner.OwnerZip, shipping.ShippingZip },
				new[]
				{
					string.Join("　", new[] { this.Owner.OwnerAddr1, this.Owner.OwnerAddr2, this.Owner.OwnerAddr3, this.Owner.OwnerAddr4 }),
					string.Join("　", new[] { shipping.ShippingAddr1, shipping.ShippingAddr2, shipping.ShippingAddr3, shipping.ShippingAddr4 }),
				},
				new[] { this.Owner.OwnerCompanyName, shipping.ShippingCompanyName },
				new[] { this.Owner.OwnerCompanyPostName, shipping.ShippingCompanyPostName },
				new[] { this.Owner.OwnerTel1, shipping.ShippingTel1 },
			};

			var result = comparison.All(c => (c[0] == c[1]))
				? Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON
				: Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			return result;
		}

		/// <summary>
		/// 住所情報から請求書同梱フラグを判定する（GMO）
		/// </summary>
		/// <returns>請求書同梱フラグ</returns>
		/// <remarks>モデルのプロパティは変更しない</remarks>
		public string JudgmentGmoInvoiceBundleFlg()
		{
			if ((this.Shippings.Any() == false)
				|| (this.Owner == null))
			{
				return Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			}

			var shippingZip = this.Shippings.First().ShippingZip;
			var shippingAddr = this.Shippings.First().ConcatenateAddressWithoutCountryName();
			var ownerZip = this.Owner.OwnerZip;
			var ownerAddr = this.Owner.ConcatenateAddressWithoutCountryName();

			return ((shippingZip == ownerZip) && (shippingAddr == ownerAddr))
				? Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON
				: Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
		}

		/// <summary>
		/// 住所情報から請求書同梱フラグを判定する (NP後払い)
		/// </summary>
		/// <returns>請求書同梱フラグ</returns>
		/// <remarks>モデルのプロパティは変更しない</remarks>
		public string JudgmentNPAfterPayInvoiceBundleFlg()
		{
			if ((this.Shippings.Any() == false)
				|| (this.Owner == null))
			{
				return Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			}

			var shippingAddr = this.Shippings.First().ConcatenateAddressWithoutCountryName();
			var ownerAddr = this.Owner.ConcatenateAddressWithoutCountryName();

			var invoiceBundleFlg = (shippingAddr == ownerAddr)
				? Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON
				: Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			return invoiceBundleFlg;
		}

		/// <summary>
		/// 注文メモ追加
		/// </summary>
		/// <param name="paymentMemo">決済メモ</param>
		public void AppendPaymentMemo(string paymentMemo)
		{
			this.PaymentMemo = OrderExternalPaymentUtility.SetExternalPaymentMemo(
				StringUtility.ToEmpty(this.PaymentMemo),
				paymentMemo);
		}

		/// <summary>
		/// 住所情報から請求書同梱フラグを判定する（Atobaraicom）
		/// </summary>
		/// <returns>請求書同梱フラグ</returns>
		/// <remarks>モデルのプロパティは変更しない</remarks>
		public string JudgmentAtobaraicomInvoiceBundleFlg()
		{
			if ((this.Shippings.Any() == false)
				|| (this.Owner == null))
			{
				return Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			}

			var shippingZip = this.Shippings.First().ShippingZip;
			var shippingAddr = this.Shippings.First().ConcatenateAddressWithoutCountryName();
			var ownerZip = this.Owner.OwnerZip;
			var ownerAddr = this.Owner.ConcatenateAddressWithoutCountryName();

			return ((shippingZip == ownerZip) && (shippingAddr == ownerAddr))
				? Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON
				: Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
		}

		/// <summary>
		/// 住所情報から請求書同梱フラグを判定する (ベリトランス後払い)
		/// </summary>
		/// <returns>請求書同梱フラグ</returns>
		/// <remarks>モデルのプロパティは変更しない</remarks>
		public string JudgmenVeritransInvoiceBundleFlg()
		{
			if ((this.Shippings.Any() == false)
				|| (this.Owner == null))
			{
				return Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			}

			var shippingAddr = this.Shippings.First().ConcatenateAddressWithoutCountryName();
			var ownerAddr = this.Owner.ConcatenateAddressWithoutCountryName();

			var invoiceBundleFlg = shippingAddr == ownerAddr
				? Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON
				: Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			return invoiceBundleFlg;
		}

		/// <summary>
		/// 対象の頒布会定額コースの注文商品数を取得
		/// </summary>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <returns>対象コースの注文商品数</returns>
		public int GetFixedAmountCourseItemCount(string subscriptionBoxCourseId)
		{
			return this.Items
				.Count(
					item => item.IsSubscriptionBoxFixedAmount
						&& (item.SubscriptionBoxCourseId == subscriptionBoxCourseId));
		}

		/// <summary>
		/// 1種類の頒布会定額コースの商品のみが含まれるか
		/// </summary>
		/// <remarks>通常の定額頒布会 or 同コース同梱の定額頒布会であるかを確認</remarks>
		/// <returns>1種類の頒布会定額コースの商品のみが含まれるであればTRUE</returns>
		public bool HaveOnlyOneSubscriptionBoxFixedAmountCourseItem()
		{
			// 注文同梱で無ければ、注文が頒布会定額コースならTRUE
			if ((this.IsOrderCombined == false) && this.IsSubscriptionBoxFixedAmount) return true;

			// 注文同梱であれば、定額コース以外が含まれないかつ、同じ頒布会定額コースでの同梱ならTRUE
			var hasItemsNotFixedAmount = this.Items.Any(item => item.IsSubscriptionBoxFixedAmount == false);
			if (hasItemsNotFixedAmount) return false;

			var fixedAmountCourseIdCount = this.Items
				.Where(item => item.IsSubscriptionBoxFixedAmount)
				.GroupBy(item => item.SubscriptionBoxCourseId)
				.Count();
			return fixedAmountCourseIdCount == 1;
		}

		/// <summary>
		/// セットプロモーション割引額再計算
		/// </summary>
		public void RecalculateSetPromotionDiscountAmount()
		{
			this.SetpromotionProductDiscountAmount
				= this.SetPromotions.Sum(setPromotion => setPromotion.ProductDiscountAmount);
			this.SetpromotionPaymentChargeDiscountAmount
				= this.SetPromotions.Sum(setPromotion => setPromotion.PaymentChargeDiscountAmount);
			this.SetpromotionShippingChargeDiscountAmount
				= this.SetPromotions.Sum(setPromotion => setPromotion.ShippingChargeDiscountAmount);
		}
		#endregion

		#region プロパティ
		/// <summary>元注文か</summary>
		public bool IsOriginalOrder
		{
			get { return string.IsNullOrEmpty(this.OrderIdOrg); }
		}
		/// <summary>定期購入注文か</summary>
		public bool IsFixedPurchaseOrder
		{
			get { return string.IsNullOrEmpty(this.FixedPurchaseId) == false; }
		}
		/// <summary>頒布会購入か</summary>
		public bool IsSubscriptionBox
		{
			get { return (string.IsNullOrEmpty(this.SubscriptionBoxCourseId) == false); }
		}
		/// <summary>ギフト注文か</summary>
		public bool IsGiftOrder
		{
			get { return this.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON; }
		}
		/// <summary>キャンセル済みか</summary>
		public bool IsCanceled
		{
			get
			{
				return ((this.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
					|| (this.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED));
			}
		}
		/// <summary>出荷済みか</summary>
		public bool IsAlreadyShipped
		{
			get
			{
				return (this.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
					|| (this.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP);
			}
		}
		/// <summary>通常注文（返品交換注文ではない）か</summary>
		public bool IsNotReturnExchangeOrder
		{
			get
			{
				return (this.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN);
			}
		}
		/// <summary>返品注文（通常・交換注文ではない）か</summary>
		public bool IsReturnOrder
		{
			get
			{
				return (this.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN);
			}
		}
		/// <summary>交換注文（通常・返品注文ではない）か</summary>
		public bool IsExchangeOrder
		{
			get
			{
				return (this.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE);
			}
		}
		/// <summary>返品交換完了済みか</summary>
		public bool IsAlreadyReturnExchangeCompleted
		{
			get
			{
				return (this.OrderReturnExchangeStatus == Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_COMPLETE);
			}
		}
		/// <summary>仮注文か</summary>
		public bool IsTempOrder
		{
			get
			{
				return (this.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP);
			}
		}
		/// <summary>注文者情報</summary>
		public OrderOwnerModel Owner
		{
			get { return (OrderOwnerModel)this.DataSource["EX_Owner"]; }
			set { this.DataSource["EX_Owner"] = value; }
		}
		/// <summary>配送先リスト</summary>
		public OrderShippingModel[] Shippings
		{
			get { return (OrderShippingModel[])this.DataSource["EX_Shippings"]; }
			set { this.DataSource["EX_Shippings"] = value; }
		}
		/// <summary>税率毎価格情報リスト</summary>
		public OrderPriceByTaxRateModel[] OrderPriceByTaxRates
		{
			get { return (OrderPriceByTaxRateModel[])this.DataSource["EX_OrderPriceByTaxRates"]; }
			set { this.DataSource["EX_OrderPriceByTaxRates"] = value; }
		}
		/// <summary>クーポンリスト</summary>
		public OrderCouponModel[] Coupons
		{
			get { return ((OrderCouponModel[])this.DataSource["EX_Coupons"] ?? new OrderCouponModel[0]); }
			set { this.DataSource["EX_Coupons"] = value; }
		}
		/// <summary>セットプロモーションリスト</summary>
		public OrderSetPromotionModel[] SetPromotions
		{
			get { return ((OrderSetPromotionModel[])this.DataSource["EX_SetPromotions"] ?? new OrderSetPromotionModel[0]); }
			set { this.DataSource["EX_SetPromotions"] = value; }
		}
		/// <summary>注文商品</summary>
		public OrderItemModel[] Items
		{
			get { return (OrderItemModel[])this.DataSource["EX_Items"]; }
			set { this.DataSource["EX_Items"] = value; }
		}
		/// <summary>Order Invoice</summary>
		public TwOrderInvoiceModel[] Invoices
		{
			get { return (TwOrderInvoiceModel[])this.DataSource["EX_Invoices"]; }
			set { this.DataSource["EX_Invoices"] = value; }
		}
		/// <summary>決済種別名</summary>
		public string PaymentName
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_NAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_NAME];
			}
			set { this.DataSource[Constants.FIELD_PAYMENT_PAYMENT_NAME] = value; }
		}
		/// <summary>外部決済か?（再与信用クレジットカード／後払い決済のみ）</summary>
		public bool IsExternalPayment
		{
			get
			{
				return ((this.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					|| (this.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					|| (this.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
					|| (this.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
					|| (this.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					|| (this.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					|| (this.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					|| (this.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
					|| (this.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					|| (this.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
					|| (this.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY));
			}
		}
		/// <summary>
		/// 拡張ステータス
		/// </summary>
		public ExtendStatusData ExtendStatus
		{
			get
			{
				// 既にデータが存在する場合、返却
				if (m_extendStatus != null) return m_extendStatus;
				m_extendStatus = new ExtendStatusData(this.DataSource);
				return m_extendStatus;
			}
		}
		/// <summary>セットプロモーション合計割引額</summary>
		public decimal SetpromotionTotalDiscountAmount
		{
			get
			{
				return this.SetpromotionProductDiscountAmount	// セットプロモーション商品割引額
				+ this.SetpromotionShippingChargeDiscountAmount	//セットプロモーション配送料割引額
				+ this.SetpromotionPaymentChargeDiscountAmount;	//セットプロモーション決済手数料割引額
			}
		}
		/// <summary>割引額合計</summary>
		public decimal OrderPriceDiscountTotal
		{
			get
			{
				return this.MemberRankDiscountPrice	// 会員ランク割引
					+ this.SetpromotionTotalDiscountAmount	// セットプロモーション合計割引
					+ this.OrderPointUseYen	// ポイント利用額
					+ this.OrderCouponUse	// クーポン割引額
					+ this.FixedPurchaseMemberDiscountAmount	// 定期会員割引額
					+ this.FixedPurchaseDiscountPrice;	// 定期購入割引額
			}
		}
		private ExtendStatusData m_extendStatus = null;
		/// <summary>配送種別名</summary>
		public string ShopShippingName
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME];
			}
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME] = value; }
		}
		/// <summary>キャンセル済みか</summary>
		public bool IsCancelled
		{
			get
			{
				return ((this.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
					|| (this.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED));
			}
		}
		/// <summary>クレジットカードを利用しているか?</summary>
		public bool UseUserCreditCard
		{
			get { return ((this.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && (this.CreditBranchNo != null)); }
		}
		/// <summary>
		/// 拡張項目_注文区分
		/// </summary>
		public string OrderKbnText
		{
			get { return ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_KBN, this.OrderKbn); }
		}
		/// <summary>
		/// 拡張項目_注文ステータス
		/// </summary>
		public string OrderStatusText
		{
			get { return ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, this.OrderStatus); }
		}
		/// <summary>
		/// ロールバック履歴情報か（trueの場合、ロールバック履歴が落ちる）
		/// </summary>
		public bool IsRollbackHistoryInfo { get; set; }
		/// <summary>定期購入区分（受注関連ファイル取込用）</summary>
		public string FixedPurchaseKbn
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN] = value; }
		}
		/// <summary>定期購入周期設定（受注関連ファイル取込用）</summary>
		public string FixedPurchaseSetting1
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1] = value; }
		}
		/// <summary>User management level id</summary>
		public string UserManagementLevelId
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID];
			}
			set { this.DataSource[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID] = value; }
		}
		/// <summary>自社サイトの注文か</summary>
		public bool IsOwnSiteOrder
		{
			get { return (this.MallId == Constants.FLG_ORDER_MALL_ID_OWN_SITE); }
		}
		/// <summary>モバイルサイトの注文か</summary>
		public bool IsMobileOrder
		{
			get { return (this.OrderKbn == Constants.FLG_ORDER_ORDER_KBN_MOBILE); }
		}
		/// <summary>クーポンを使用しているか</summary>
		public bool IsCouponUse
		{
			get { return this.Coupons.Any(); }
		}
		/// <summary>ポイントを使用しているか</summary>
		public bool IsPointUse
		{
			get { return (this.OrderPointUse > 0); }
		}
		/// <summary>ポイントを付与されたか</summary>
		public bool IsPointAdd
		{
			get { return (this.OrderPointAdd > 0); }
		}
		/// <summary>デジタルコンテンツ？</summary>
		public bool IsDigitalContents
		{
			get { return (this.DigitalContentsFlg == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON); }
		}
		/// <summary>着荷予定日</summary>
		public DateTime ArrivalScheduleDate
		{
			get
			{
				var shopShipping = ShopShippingService.GetAsStatic(this.ShopId, this.ShippingId);
				return this.Shippings[0].ShippingDate ?? this.OrderDate.Value.AddDays(shopShipping.FixedPurchaseShippingDaysRequired);
			}
		}
		/// <summary>調整金額合計(返品用金額補正を含む)</summary>
		public decimal OrderPriceRegulationTotal
		{
			get
			{
				return this.OrderPriceRegulation + this.OrderPriceByTaxRates.Sum(
					orderPriceByTaxRate => orderPriceByTaxRate.ReturnPriceCorrectionByRate);
			}
		}
		/// <summary>決済手数料無料セットプロモーションが存在するか</summary>
		public bool IsContainPaymentChargeFreeSet
		{
			get
			{
				return this.SetPromotions.Any(setPromotion => setPromotion.IsDiscountTypePaymentChargeFree);
			}
		}
		/// <summary>配送料割引金額(割引按分計算後)</summary>
		public decimal ShippingPriceDiscountAmount { get; set; }
		/// <summary>決済手数料割引金額(割引按分計算後)</summary>
		public decimal PaymentPriceDiscountAmount { get; set; }
		/// <summary>Is Update Atone Payment From My Page</summary>
		public bool IsUpdateAtonePaymentFromMyPage { get; set; }
		/// <summary>Is Update Aftee Payment From My Page</summary>
		public bool IsUpdateAfteePaymentFromMyPage { get; set; }
		/// <summary>Is Order Sales Settled</summary>
		public bool IsOrderSalesSettled { get; set; }
		/// <summary>Is Need Confirm LINE Pay Payment</summary>
		public bool IsNeedConfirmLinePayPayment { get; set; }
		/// <summary>Is Return Exchange Process At Workflow</summary>
		public bool IsReturnExchangeProcessAtWorkflow { get; set; }
		/// <summary>Is Execute Reauth From My Page</summary>
		public bool IsExecuteReauthFromMyPage { get; set; }
		/// <summary>GMO後払い_デバイス情報</summary>
		public string DeviceInfo { get; set; }
		/// <summary>Is order payment status complete</summary>
		public bool IsOrderPaymentStatusComplete
		{
			get { return this.OrderPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM; }
		}
		/// <summary>後払い請求書が再発行可能か（利用可能かは決済代行会社にもよります）</summary>
		public bool CanRequestCvsDefInvoiceReissue
		{
			get
			{
				return (this.IsAlreadyShipped
					&& (this.IsOrderPaymentStatusComplete)
					&& ((this.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
						|| (this.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF)));
			}
		}
		/// <summary>Payment Cart Id</summary>
		public string PaymentCartId { get; set; }
		/// <summary>頒布会定額コースか</summary>
		public bool IsSubscriptionBoxFixedAmount
		{
			get { return ((this.SubscriptionBoxFixedAmount.HasValue) && (this.SubscriptionBoxFixedAmount.Value != 0)); }
		}
		/// <summary>返品用金額補正</summary>
		public decimal ReturnPriceCorrection { get; set; }
		/// <summary>Purchase price total</summary>
		public decimal PurchasePriceTotal
		{
			get
			{
				var couponDiscountPrice = (this.IsCouponUse
						&& (this.Coupons[0].CouponDiscountPrice.HasValue))
					? this.Coupons[0].CouponDiscountPrice.Value
					: 0m;

				var totalDiscount = this.MemberRankDiscountPrice
					+ couponDiscountPrice
					+ this.FixedPurchaseDiscountPrice
					+ this.FixedPurchaseMemberDiscountAmount
					+ this.SetpromotionProductDiscountAmount;

				var result = (this.OrderPriceSubtotal - totalDiscount);
				return result;
			}
		}
		/// <summary>Is Update Boku Payment From My Page</summary>
		public bool IsUpdateBokuPaymentFromMyPage { get; set; }
		/// <summary>同梱注文か</summary>
		public bool IsOrderCombined => string.IsNullOrEmpty(this.CombinedOrgOrderIds) == false;
		/// <summary>頒布会商品が含まれるか</summary>
		public bool HasSubscriptionBoxItem
		{
			get
			{
				if ((this.IsOrderCombined == false) && this.IsSubscriptionBox) return true;

				var result = this.Items.Any(item => item.IsSubscriptionBox);
				return result;
			}
		}
		/// <summary>頒布会定額コース商品が含まれるか</summary>
		public bool HasSubscriptionBoxFixedAmountItem
		{
			get
			{
				if ((this.IsOrderCombined == false) && this.IsSubscriptionBoxFixedAmount) return true;

				var result = this.Shippings
					.SelectMany(shipping => shipping.Items)
					.Any(item => item.IsSubscriptionBoxFixedAmount);
				return result;
			}
		}
		/// <summary>頒布会定額金額</summary>
		public Dictionary<string, decimal> SubscriptionBoxFixedAmountList { get; set; } = new Dictionary<string, decimal>();
		/// <summary>ペイジェントクレカトークン</summary>
		public string PaygentCreditToken { get; set; }
		/// <summary>PayTg用 新規クレジットカードか</summary>
		public bool IsNewCard { get; set; }
		#endregion

		#region 拡張ステータスデータクラス
		/// <summary>
		/// 拡張ステータスデータクラス
		/// </summary>
		[Serializable]
		public class ExtendStatusData
		{
			/// <summary>拡張ステータスデータ格納用</summary>
			private List<ExtendStatusDataInner> m_inners = new List<ExtendStatusDataInner>();

			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="source">ソース</param>
			public ExtendStatusData(Hashtable source)
			{
				this.DataSource = source;
			}
			#endregion

			#region インデクサ
			/// <summary>
			/// インデクサ
			/// </summary>
			/// <param name="index">インデックス</param>
			/// <returns>拡張ステータスデータインナー</returns>
			public ExtendStatusDataInner this[int index]
			{
				get
				{
					// 注文拡張ステータスNo範囲外の場合はエラー
					var extendStatusNo = index + 1;
					if (extendStatusNo < 1 || extendStatusNo > Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX)
					{
						throw new ApplicationException(string.Format("拡張ステータスNo範囲外のためエラーが発生しました。（No.{0}）", extendStatusNo));
					}
					// 既にデータが存在する場合、返却
					var inner = m_inners.FirstOrDefault(i => i.ExtendStatusNo == extendStatusNo);
					if (inner != null) return inner;
					inner = new ExtendStatusDataInner(this.DataSource, extendStatusNo);
					m_inners.Add(inner);
					return inner;
				}
			}
			#endregion

			#region プロパティ
			/// <summary>ソース</summary>
			private Hashtable DataSource { get; set; }
			#endregion
		}
		#endregion

		#region 拡張ステータスデータインナークラス
		/// <summary>
		/// 拡張ステータスデータインナークラス
		/// </summary>
		[Serializable]
		public class ExtendStatusDataInner
		{
			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="source">ソース</param>
			/// <param name="extendStatusNo">注文拡張ステータスNo</param>
			public ExtendStatusDataInner(Hashtable source, int extendStatusNo)
			{
				this.DataSource = source;
				this.ExtendStatusNo = extendStatusNo;
			}
			#endregion

			#region プロパティ
			/// <summary>注文拡張ステータスNo</summary>
			public int ExtendStatusNo { get; set; }
			/// <summary>拡張ステータス値</summary>
			public string Value
			{
				get { return (string)this.DataSource[this.ValueKey]; }
				set { this.DataSource[this.ValueKey] = value; }
			}
			/// <summary>拡張ステータス値キー</summary>
			private string ValueKey { get { return string.Format(Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME + "{0}", this.ExtendStatusNo.ToString()); } }
			/// <summary>拡張ステータス更新日時</summary>
			public DateTime? Date
			{
				get
				{
					if (this.DataSource[this.DateKey] == DBNull.Value) return null;
					return (DateTime?)this.DataSource[this.DateKey];
				}
				set { this.DataSource[this.DateKey] = value; }
			}
			/// <summary>拡張ステータス更新日時キー</summary>
			private string DateKey { get { return string.Format(Constants.FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME + "{0}", this.ExtendStatusNo.ToString()); } }
			/// <summary>ソース</summary>
			private Hashtable DataSource { get; set; }
			#endregion
		}
		#endregion
	}
}
